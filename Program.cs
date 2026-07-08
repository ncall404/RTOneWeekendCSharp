// A software raytracer based on RayTracing In One Weekend but in C#! (https://raytracing.github.io/books/RayTracingInOneWeekend.html)
global using RTOneWeekend.Utility;

using System.Runtime.InteropServices;
using SDL3;

using RTOneWeekend.Core;
using RTOneWeekend.Geometry;

namespace RTOneWeekend;

class Program
{
    private static readonly double aspectRatio = 16.0 / 9.0;
    private static readonly int width = 400;
    private static int height = (int)(width / aspectRatio);

    [STAThread]
    static void Main(string[] args)
    {
        // Make sure that image height is at least 1.
        height = (height < 1) ? 1 : height;

        if (!SDL.Init(SDL.InitFlags.Video))
        {
            SDL.LogError(SDL.LogCategory.System, $"SDL could not initialize: {SDL.GetError()}");
            return;
        }

        if (!SDL.CreateWindowAndRenderer("SDL3 Create Window", width * 2, height * 2, 0, out var window, out var renderer))
        {
            SDL.LogError(SDL.LogCategory.Application, $"Error creating window and rendering: {SDL.GetError()}");
            return;
        }

        // Streaming texture for pixel data with 4 bytes per pixel (RGBA8888)
        var texture = SDL.CreateTexture(renderer, SDL.PixelFormat.RGBA8888, SDL.TextureAccess.Streaming, width, height);
        SDL.SetTextureScaleMode(texture, SDL.ScaleMode.Nearest);
        if (texture == IntPtr.Zero)
        {
            SDL.LogError(SDL.LogCategory.Error, $"Texture creation failed: {SDL.GetError()}");
            SDL.DestroyRenderer(renderer);
            SDL.DestroyWindow(window);
            return;
        }

        // SDL.SetRenderDrawColor(renderer, 100, 150, 200, 0);

		// Create the initial scene.
		HittableList world = InitializeScene();

        // Update texture (move to inside of loop once anything is dynamic/temporal)
        byte[] pixelBuffer = RenderScene(width, height, world);
        IntPtr pixelsPtr = IntPtr.Zero;
        int pitch = 0;
        if (SDL.LockTexture(texture, IntPtr.Zero, out pixelsPtr, out pitch))
        {
            Marshal.Copy(pixelBuffer, 0, pixelsPtr, pixelBuffer.Length);
            SDL.UnlockTexture(texture);
        }


        var loop = true;

        while (loop)
        {
            while (SDL.PollEvent(out var e))
            {
                if (e.Type == (uint)SDL.EventType.Quit)
                {
                    loop = false;
                }
                else if (e.Type == (uint)SDL.EventType.KeyDown && e.Key.Key == SDL.Keycode.Escape)
                {
                    loop = false;
                }
            }

            SDL.RenderClear(renderer);
            SDL.RenderTexture(renderer, texture, IntPtr.Zero, IntPtr.Zero);
            SDL.RenderPresent(renderer);
        }

        SDL.DestroyRenderer(renderer);
        SDL.DestroyWindow(window);

        SDL.Quit();
    }

    private static Vec3 RayColor(Ray r, Hittable world)
    {
        HitRecord rec = default;
		if (world.Hit(r, 0, double.PositiveInfinity, ref rec))
		{
			return 0.5 * (rec.Normal + new Vec3(1, 1, 1));
		}


        Vec3 unitDirection = Vec3.UnitVector(r.Direction);
        double a = 0.5 * (unitDirection.Y + 1.0);
        return (1.0 - a) * new Vec3(1.0, 1.0, 1.0) + a * new Vec3(0.5, 0.7, 1.0); // Lerp between light blue and white based on ray Y position.
    }

    private static byte[] RenderScene(int width, int height, HittableList world)
    {
        // Camera
        double focalLength = 1.0;
        double viewportHeight = 2.0;
        double viewportWidth = viewportHeight * (width / (double)height);
        Vec3 cameraCenter = new(0, 0, 0);

        // Calculate the vectors across the horizontal and down the vertical viewport edges.
        Vec3 viewportU = new(viewportWidth, 0, 0);
        Vec3 viewportV = new(0, -viewportHeight, 0);

        // Calculate the horizontal and vertical delta vectors from pixel to pixel.
        Vec3 pixelDeltaU = viewportU / width;
        Vec3 pixelDeltaV = viewportV / height;

        // Calculate the location of the upper left pixel of the viewport.
        Vec3 viewportUpperLeft = cameraCenter - new Vec3(0, 0, focalLength) - viewportU / 2 - viewportV / 2;
        Vec3 pixel00Location = viewportUpperLeft + 0.5 * (pixelDeltaU + pixelDeltaV);

        // Pixel buffer.
        var pixels = new byte[width * height * 4];

        // Draw to each pixel. TODO: Switch outside loop to "Parallel.For" later in the tutorials for better performance.
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int offset = (y * width + x) * 4;

                Vec3 pixelCenter = pixel00Location + (x * pixelDeltaU) + (y * pixelDeltaV);
                Vec3 rayDirection = pixelCenter - cameraCenter;
                Ray r = new(cameraCenter, rayDirection);
                Vec3 rayColor = RayColor(r, world);

                // Pack color into 32 bit uint
                uint pixelColor = (uint)(((byte)(rayColor.R * 255) << 24) | ((byte)(rayColor.G * 255) << 16) | ((byte)(rayColor.B * 255) << 8) | SDL.AlphaOpaque);
                BitConverter.TryWriteBytes(pixels.AsSpan(offset, 4), pixelColor);
            }
        }

        return pixels;
    }

	// Creates the initial scene.
	private static HittableList InitializeScene()
	{
		HittableList world = new(new Sphere(new Vec3(0, 0, -1), 0.5));

		world.Add(new Sphere(new Vec3(0, -100.5, -1), 100)); // Ground sphere

		return world;
	}
}
