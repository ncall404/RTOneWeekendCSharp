// A software raytracer based on RayTracing In One Weekend but in C#! (https://raytracing.github.io/books/RayTracingInOneWeekend.html)

global using RTOneWeekend.Utility;

using System.Runtime.InteropServices;
using SDL3;

using RTOneWeekend.Core;
using RTOneWeekend.Geometry;
using RTOneWeekend.Scenes;

namespace RTOneWeekend;

class Program
{
    [STAThread]
    static void Main(string[] args)
    {
		Scene activeScene;

		// Load the initial scene.
		activeScene = SceneManager.LoadScene(SceneManager.SelectedScene);

		bool sceneChanged = true; // Bool to track if the loaded scene has changed and needs a rerender even in non-realtime mode.

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
        nint texture = CreateRenderTexture(renderer, window, activeScene.Camera);



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
				// Toggles debug text on/off.
				else if (e.Type == (uint)SDL.EventType.KeyDown && e.Key.Key == SDL.Keycode.H)
				{
					Settings.HideDebugText = !Settings.HideDebugText;
				}
				// Changes what scene is being rendered.
					// Increase selected scene number
				else if (e.Type == (uint)SDL.EventType.KeyDown && e.Key.Key == SDL.Keycode.Right)
				{
					if (SceneManager.SelectedScene < SceneManager.SceneCount)
					{
						SceneManager.SelectedScene++;
					}
				}
					// Decrease selected scene number
				else if (e.Type == (uint)SDL.EventType.KeyDown && e.Key.Key == SDL.Keycode.Left)
				{
					if (SceneManager.SelectedScene > 0)
					{
						SceneManager.SelectedScene--;
					}
				}
					// Load selected scene
				else if (e.Type == (uint)SDL.EventType.KeyDown && e.Key.Key == SDL.Keycode.Down)
				{
					if (SceneManager.SelectedScene >= 0 && SceneManager.SelectedScene <= SceneManager.SceneCount)
					{
						sceneChanged = true;
						activeScene = SceneManager.LoadScene(SceneManager.SelectedScene);
					}
				}
            }

			if (Settings.RealTimeRender || sceneChanged)
			{
				activeScene.Camera.CalculateViewport();
				if (!sceneChanged)
					activeScene.ProcessScene(); // Scene-specific processing for when Settings.RealTime is true.

				if (sceneChanged)
				{
					// Recreate the texture if a different scene is loaded.
					SDL.DestroyTexture(texture);
					texture = CreateRenderTexture(renderer, window, activeScene.Camera);
					if (Settings.RealTimeRender)
						sceneChanged = false;
				}

				ulong startTime = SDL.GetTicks();
				UpdateTextureRender(activeScene.Camera, activeScene.World, texture);
				ulong endTime = SDL.GetTicks();

				// Write the render time to the console if not doing real-time rendering.
				if (!Settings.RealTimeRender)
				{
					Console.WriteLine($"Render time: {endTime - startTime}ms");
					sceneChanged = false;
				}
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
			}

			// Display debug text if it isn't hidden.
			if (!Settings.HideDebugText)
			{
				if (Settings.RealTimeRender)
				{
					SDL.RenderDebugText(renderer, 5, 25, $"fps: {currentFps:F2}");
					SDL.RenderDebugText(renderer, 5, 35, $"ms: {frameTime:F2}");
				}
				SDL.RenderDebugText(renderer, 5, 5, $"Selected Scene: {SceneManager.SelectedScene} / {SceneManager.SceneCount}");
				SDL.RenderDebugText(renderer, 5, 15, $"Loaded Scene: {SceneManager.LoadedScene} / {SceneManager.SceneCount}");
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
}
