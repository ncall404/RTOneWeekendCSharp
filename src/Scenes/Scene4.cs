// Represents Scene 4: Earth. Also found in Ray Tracing: The Next Week. My version adds spinning capabilities when Settings.Realtime is true.

using SDL3;
using RTOneWeekend.Core;
using RTOneWeekend.Geometry;
using RTOneWeekend.Materials;
using RTOneWeekend.Textures;

namespace RTOneWeekend.Scenes;

public class Scene4 : Scene
{
	public override int ID { get; protected set; } = 4;
	public override string Name { get; protected set; } = "Earth";

	public override HittableList World { get; protected set; }
	public override Camera Camera { get; protected set; }

	public Scene4()
	{
		(World, Camera) = LoadScene();
	}

	protected override (HittableList, Camera) LoadScene()
	{
		HittableList world = new();

		ImageTexture earthTexture = new ImageTexture("./assets/SampleTextures/earthmap.jpg");
		Lambertian earthSurface = new Lambertian(earthTexture);
		Sphere earth = new Sphere(new(0, 0, 0), 2, earthSurface)
		{
			RotationOffset = new(-0.5, 0, 0)
		};
		world.Add(earth);

		Camera camera = new(
			16.0 / 9.0,				// Aspect ratio
			1280,					// Render width
			100,					// Samples per pixel
			50,						// Max depth (number of bounces for rays)
			20,						// Vertical field of view
			new(0, 0, 12),			// Camera position.
			new(0, 0, 0),			// Look at point.
			new(0, 1, 0),			// Up vector.
			new(0.7, 0.8, 1)		// Background color.
		);

		return (world, camera);
	}

	public override void ProcessScene()
	{
		// Update the rotation of the earth sphere.
		if (World[0] is Sphere sphere)
		{
			if (Settings.RealTimeRender)
			{
				if (Settings.AntiAliasing)
					Settings.AntiAliasing = false;
					
				sphere.UpdateRotationOffset(SDL.GetTicks() * 0.0001);
			}
			else
			{
				sphere.UpdateRotationOffset(0);
			}
		}
	}
}