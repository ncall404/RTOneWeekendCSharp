// A software raytracer based on RayTracing In One Weekend but in C#! (https://raytracing.github.io/books/RayTracingInOneWeekend.html)

global using RTOneWeekend.Utility;

using System.Runtime.InteropServices;
using SDL3;

using RTOneWeekend.Core;
using RTOneWeekend.Geometry;
using RTOneWeekend.Materials;

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

        if (!SDL.CreateWindowAndRenderer("CSharp Ray Tracer", Settings.WindowWidth, Settings.WindowHeight, 0, out var window, out var renderer))
        {
            SDL.LogError(SDL.LogCategory.Application, $"Error creating window and rendering: {SDL.GetError()}");
            return;
        }
		SDL.SetRenderDrawColor(renderer, 0, 150, 0, 255); // Set render draw color for debug text.

        // Streaming texture for pixel data with 4 bytes per pixel (RGBA8888)
        var texture = SDL.CreateTexture(renderer, SDL.PixelFormat.RGBA8888, SDL.TextureAccess.Streaming, camera.Width, camera.Height);
        if (texture == IntPtr.Zero)
        {
            SDL.LogError(SDL.LogCategory.Error, $"Texture creation failed: {SDL.GetError()}");
            SDL.DestroyRenderer(renderer);
            SDL.DestroyWindow(window);
            return;
        }
		SDL.SetTextureScaleMode(texture, SDL.ScaleMode.Nearest); // Set the texture to the correct scaling mode to not be blurry if the window is a higher resolution.

		// Do initial render to the texture.
		UpdateTextureRender(pixelBuffer, camera, world, texture);

		// Peformance monitoring variables for loop.
		ulong lastCounter = SDL.GetTicks();
		float currentFps = 0f;
		float frameTime = 0f;
		float frameCount = 0f;

		var loop = true;
        while (loop)
        {
            while (SDL.PollEvent(out var e))
            {
                if (e.Type == (uint)SDL.EventType.Quit)
                {
                    loop = false;
                }
				// Stops the program.
                else if (e.Type == (uint)SDL.EventType.KeyDown && e.Key.Key == SDL.Keycode.Escape)
                {
					loop = false;
                }
				// Toggles anti-aliasing on/off.
				else if (e.Type == (uint)SDL.EventType.KeyDown && e.Key.Key == SDL.Keycode.Alpha1)
				{
					Settings.AntiAliasing = !Settings.AntiAliasing;
					if (!Settings.RealTimeRender)
					{
						UpdateTextureRender(pixelBuffer, camera, world, texture);
					}
				}
				// Toggles real-time rendering on/off.
				else if (e.Type == (uint)SDL.EventType.KeyDown && e.Key.Key == SDL.Keycode.Alpha2)
				{
					Settings.RealTimeRender = !Settings.RealTimeRender;
				}
            }

			if (Settings.RealTimeRender)
			{
				pixelBuffer = camera.Render(world);
				UpdateTextureRender(pixelBuffer, camera, world, texture);
			}
			

            SDL.RenderClear(renderer);

			// Display the render.
            SDL.RenderTexture(renderer, texture, IntPtr.Zero, IntPtr.Zero);

			// Display performance when doing real-time rendering.
			if (Settings.RealTimeRender)
			{
				ulong currentCounter = SDL.GetTicks();
				ulong elapsed = currentCounter - lastCounter;
				frameCount++;

				// Updates every 150 milliseconds
				if (elapsed >= 150)
				{
					currentFps = frameCount / (elapsed / 1000f);
					frameTime = elapsed / frameCount;

					frameCount = 0f;
					lastCounter = currentCounter;
				}
				SDL.RenderDebugText(renderer, 5, 5, $"fps: {currentFps:F2}");
				SDL.RenderDebugText(renderer, 5, 15, $"ms: {frameTime:F2}");
			}

            SDL.RenderPresent(renderer);
        }

        SDL.DestroyRenderer(renderer);
        SDL.DestroyWindow(window);

        SDL.Quit();
    }

	// Creates the initial scene.
	private static HittableList InitializeScene()
	{
		HittableList world = new(new Sphere(new Vec3(0, -100.5, -1), 100, new Lambertian(new Vec3(0.8, 0.8, 0.0)))); // Ground sphere

		// Lambertian spheres
		world.Add(new Sphere(new Vec3(0, 0, -1), 0.5, new Lambertian(new Vec3(0.1, 0.2, 0.5))));

		// Metal spheres
		world.Add(new Sphere(new Vec3(-1.0, 0.0, -1.0), 0.5, new Metal(new Vec3(0.8, 0.8, 0.8), 0.3))); // Left
		world.Add(new Sphere(new Vec3(1.0, 0.0, -1.0), 0.5, new Metal(new Vec3(0.8, 0.6, 0.2), 1.0))); // Right

		return world;
	}

	private static void UpdateTextureRender(byte[] pixelBuffer, Camera camera, HittableList world, nint texture)
	{
		pixelBuffer = camera.Render(world);
		// Update texture (move to inside of loop once anything is dynamic/temporal)
		IntPtr pixelsPtr = IntPtr.Zero;
        int pitch = 0;
		if (SDL.LockTexture(texture, IntPtr.Zero, out pixelsPtr, out pitch))
		{
			Marshal.Copy(pixelBuffer, 0, pixelsPtr, pixelBuffer.Length);
			SDL.UnlockTexture(texture);
		}
	}
}
