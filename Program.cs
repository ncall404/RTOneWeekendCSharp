// A software raytracer based on RayTracing In One Weekend but in C#! (https://raytracing.github.io/books/RayTracingInOneWeekend.html)

global using RTOneWeekend.Utility;

using System.Runtime.InteropServices;
using SDL3;

using RTOneWeekend.Core;
using RTOneWeekend.Geometry;

namespace RTOneWeekend;

class Program
{
    [STAThread]
    static void Main(string[] args)
    {
		// Create the initial scene.
		HittableList world = InitializeScene();

		Camera camera = new()
		{
			AspectRatio = 16.0 / 9.0,
			Width = 400
		};

		// Do an initial render of the pixel buffer to initialize the camera.
		byte[] pixelBuffer = camera.Render(world);

        if (!SDL.Init(SDL.InitFlags.Video))
        {
            SDL.LogError(SDL.LogCategory.System, $"SDL could not initialize: {SDL.GetError()}");
            return;
        }

        if (!SDL.CreateWindowAndRenderer("SDL3 Create Window", camera.Width * 2, camera.Height * 2, 0, out var window, out var renderer))
        {
            SDL.LogError(SDL.LogCategory.Application, $"Error creating window and rendering: {SDL.GetError()}");
            return;
        }

        // Streaming texture for pixel data with 4 bytes per pixel (RGBA8888)
        var texture = SDL.CreateTexture(renderer, SDL.PixelFormat.RGBA8888, SDL.TextureAccess.Streaming, camera.Width, camera.Height);
        if (texture == IntPtr.Zero)
        {
            SDL.LogError(SDL.LogCategory.Error, $"Texture creation failed: {SDL.GetError()}");
            SDL.DestroyRenderer(renderer);
            SDL.DestroyWindow(window);
            return;
        }
		SDL.SetTextureScaleMode(texture, SDL.ScaleMode.Nearest); // Set the texture to the corrent scaling mode to not be blurry if the window is a higher resolution.

        // Update texture (move to inside of loop once anything is dynamic/temporal)
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

	// Creates the initial scene.
	private static HittableList InitializeScene()
	{
		HittableList world = new(new Sphere(new Vec3(0, 0, -1), 0.5));

		world.Add(new Sphere(new Vec3(0, -100.5, -1), 100)); // Ground sphere

		return world;
	}
}
