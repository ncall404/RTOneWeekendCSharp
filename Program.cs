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
		HittableList world;
		Camera camera;

		(world, camera) = SceneLoader(); // Load the initial scene.
		bool sceneChanged = true; // Bool to track if the selected scene has changed and needs a rerender even in non-realtime mode.

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
        nint texture = CreateRenderTexture(renderer, window, camera);



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
					sceneChanged = true;
				}
				// Toggles real-time rendering on/off.
				else if (e.Type == (uint)SDL.EventType.KeyDown && e.Key.Key == SDL.Keycode.Alpha2)
				{
					Settings.RealTimeRender = !Settings.RealTimeRender;
				}
				// Changes what scene is being rendered.
					// Increase selected scene number
				else if (e.Type == (uint)SDL.EventType.KeyDown && e.Key.Key == SDL.Keycode.Right)
				{
					if (Settings.SelectedScene < Settings.NumScenes)
					{
						Settings.SelectedScene++;
						Console.WriteLine("Selected scene: " + Settings.SelectedScene);
					}
				}
					// Decrease selected scene number
				else if (e.Type == (uint)SDL.EventType.KeyDown && e.Key.Key == SDL.Keycode.Left)
				{
					if (Settings.SelectedScene > 1)
					{
						Settings.SelectedScene--;
						Console.WriteLine("Selected scene: " + Settings.SelectedScene);
					}
				}
					// Load selected scene
				else if (e.Type == (uint)SDL.EventType.KeyDown && e.Key.Key == SDL.Keycode.Down)
				{
					if (Settings.SelectedScene >= 1 && Settings.SelectedScene <= Settings.NumScenes)
					{
						sceneChanged = true;
						(world, camera) = SceneLoader();
					}
				}
            }

			if (Settings.RealTimeRender || sceneChanged)
			{
				camera.CalculateViewport();
				if (sceneChanged)
				{
					// Recreate the texture if a different scene is loaded.
					SDL.DestroyTexture(texture);
					texture = CreateRenderTexture(renderer, window, camera);
				}
				UpdateTextureRender(camera, world, texture);

				if (!Settings.RealTimeRender)
					sceneChanged = false;
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

	// Renders the viewport image of the camera and updates the texture with the rendered image.
	private static void UpdateTextureRender(Camera camera, HittableList world, nint texture)
	{
		byte[] pixelBuffer = camera.Render(world);
		IntPtr pixelsPtr = IntPtr.Zero;
        int pitch = 0;
		if (SDL.LockTexture(texture, IntPtr.Zero, out pixelsPtr, out pitch))
		{
			Marshal.Copy(pixelBuffer, 0, pixelsPtr, pixelBuffer.Length);
			SDL.UnlockTexture(texture);
		}
	}

	// Creates and validates the render texture that is displayed in the window.
	private static nint CreateRenderTexture(nint renderer, nint window, Camera camera)
	{
		nint texture = SDL.CreateTexture(renderer, SDL.PixelFormat.RGBA8888, SDL.TextureAccess.Streaming, camera.Width, camera.Height);
        if (texture == IntPtr.Zero)
        {
            SDL.LogError(SDL.LogCategory.Error, $"Texture creation failed: {SDL.GetError()}");
            SDL.DestroyRenderer(renderer);
            SDL.DestroyWindow(window);
        }
		SDL.SetTextureScaleMode(texture, SDL.ScaleMode.Nearest); // Set the texture to the correct scaling mode to not be blurry if the window is a higher resolution.
		return texture;
	}

	// Scene Loading Functions ========================================================================= TODO: Make an actual scene loader class at some point.

	private static (HittableList, Camera) SceneLoader()
	{
		HittableList world;
		Camera camera;
		switch (Settings.SelectedScene)
		{
			case 1:
				(world, camera) = LoadScene1();
				break;
			case 2:
				(world, camera) = LoadScene2();
				break;
			default:
				(world, camera) = LoadScene1();
				break;
		}
		return (world, camera);
	}

	// This creates the scene from book 1 with 3 balls of different materials.
	private static (HittableList, Camera) LoadScene1()
	{
		HittableList world = new(new Sphere(new(0, -100.5, -1), 100, new Lambertian(new Vec3(0.8, 0.8, 0.0)))); // Ground sphere

		// Lambertian spheres
		world.Add(new Sphere(new(0, 0, -1.2), 0.5, new Lambertian(new Vec3(0.1, 0.2, 0.5))));

		// Metal spheres
		world.Add(new Sphere(new(1.0, 0.0, -1.0), 0.5, new Metal(new Vec3(0.8, 0.6, 0.2), 1.0))); // Right

		// Dielectric spheres
			// Hollow glass sphere
		world.Add(new Sphere(new(-1.0, 0.0, -1.0), 0.5, new Dielectric(1.5))); // Left outer
		world.Add(new Sphere(new(-1.0, 0.0, -1.0), 0.4, new Dielectric(1.0 / 1.5))); // Left inner

		Camera camera = new(
			16.0 / 9.0,				// Aspect ratio
			1280,					// Render width
			100,					// Samples per pixel
			100,					// Max depth (number of bounces for rays)
			40,						// Vertical field of view
			new(-2, 2, 1),			// Camera position.
			new(0, 0, -1),			// Look at point.
			new(0, 1, 0),			// Up vector.
			10,						// Defocus Angle (for depth of field, 0 = no depth of field)
			3.5						// Focus  distance (for depth of field)
		);

		Console.WriteLine("Scene 1 Loaded");

		return (world, camera);
	}

	// This creates the scene from book 1 that is used for the final render.
	private static (HittableList, Camera) LoadScene2()
	{
		HittableList world = new();

		Lambertian matGround = new Lambertian(new Vec3(0.5, 0.5, 0.5));
		world.Add(new Sphere(new(0, -1000, 0), 1000, matGround));

		for (int a = -11; a < 11; a++)
		{
			for (int b = -11; b < 11; b++)
			{
				double chooseMat = RandomNum.RandomDouble();
				Vec3 center = new(a + 0.9*RandomNum.RandomDouble(), 0.2, b + 0.9*RandomNum.RandomDouble());

				if ((center - new Vec3(4, 0.2, 0)).Length() > 0.9)
				{
					Material matSphere;

					if (chooseMat < 0.8)
					{
						// Diffuse
						Vec3 albedo = Vec3.Random() * Vec3.Random();
						matSphere = new Lambertian(albedo);
						world.Add(new Sphere(center, 0.2, matSphere));
					}
					else if (chooseMat < 0.95)
					{
						// Metal
						Vec3 albedo = Vec3.Random(0.5, 1);
						double fuzz = RandomNum.RandomDouble(0, 0.5);
						matSphere = new Metal(albedo, fuzz);
						world.Add(new Sphere(center, 0.2, matSphere));
					}
					else
					{
						// Glass
						matSphere = new Dielectric(1.5);
						world.Add(new Sphere(center, 0.2, matSphere));
					}
				}
			}
		}
		Material mat1 = new Dielectric(1.5);
		world.Add(new Sphere(new(0, 1, 0), 1.0, mat1));

		Material mat2 = new Lambertian(new(0.4, 0.2, 0.1));
		world.Add(new Sphere(new(-4, 1, 0), 1.0, mat2));

		Material mat3 = new Metal(new(0.7, 0.6, 0.5), 0.0);
		world.Add(new Sphere(new(4, 1, 0), 1.0, mat3));

		Camera camera = new(
			16.0 / 9.0,				// Aspect ratio
			1280,					// Render width
			30,						// Samples per pixel
			50,						// Max depth (number of bounces for rays)
			20,						// Vertical field of view
			new(13, 2, 3),			// Camera position.
			new(0, 0, 0),			// Look at point.
			new(0, 1, 0),			// Up vector.
			0.6,					// Defocus Angle (for depth of field, 0 = no depth of field)
			10.0					// Focus  distance (for depth of field)
		);

		Console.WriteLine("Scene 2 Loaded");

		return (world, camera);
	}
}
