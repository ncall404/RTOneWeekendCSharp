// Represents Scene 11: Floating Cube. This scene isn't in the book, but I thought it would be fun to make. While it looks nice static, it is best in real time mode.

using SDL3;
using RTOneWeekend.Core;
using RTOneWeekend.Geometry;
using RTOneWeekend.Materials;
using RTOneWeekend.Textures;

namespace RTOneWeekend.Scenes;

public class Scene11 : Scene
{
	public override int ID { get; protected set; } = 11;
	public override string Name { get; protected set; } = "Floating Cube";

	public override HittableList World { get; protected set; }
	public override Camera Camera { get; protected set; }

	public Scene11()
	{
		(World, Camera) = LoadScene();
	}

	protected override (HittableList, Camera) LoadScene()
	{
		HittableList world = new();

		// Textures
		CheckerTexture checker = new(15, new Vec3(0.2, 0.3, 0.1), new Vec3(0.9, 0.2, 0.2));

		// Material
		Material checkerMat = new Lambertian(checker);
		Material blue = new Lambertian(new Vec3(0.12, 0.15, 0.45));
		Material metal = new Metal(new Vec3(0.7, 0.7, 0.7), 0.1);
		Material glass = new Dielectric(1.5);
		Material light = new DiffuseLight(new Vec3(2, 2, 2));

		// Box
		world.Add(new RotateY(Quad.Box(new(-1, -1, -1), new(1, 1, 1), metal), 45)); // Floating cube
		world.Add(new Sphere(new Vec3(0, -1010, 0), 1000, checkerMat)); // Ground sphere
		world.Add(new Sphere(new Vec3(3, 0.5, 0), 1, blue)); // Solid accent sphere
		world.Add(new Sphere(new Vec3(-3, 0.5, 0), 1, glass)); // Glass accent sphere
		world.Add(new Sphere(new Vec3(0, 6, 10), 3, light)); // Light sphere

		Camera camera = new(
			16.0 / 9.0,				// Aspect ratio
			700,					// Render width
			100,					// Samples per pixel
			20,						// Max depth (number of bounces for rays)
			70,						// Vertical field of view
			new(0, 2, -5),			// Camera position
			new(0, 0, 0),			// Look at point
			new(0, 1, 0),			// Up vector
			new(0.7, 0.8, 1)		// Day Background color
		);

		return (world, camera);
	}

	public override void ProcessScene()
	{
		if (Settings.RealTimeRender)
		{
			if (Settings.AntiAliasing)
				Settings.AntiAliasing = false;

			Hittable box = World[0];
			if (box is RotateY rotateBox)
			{
				Translate newBox;
				if (rotateBox.Obj is Translate translateBox)
				{
					newBox = new Translate(translateBox.Obj, new Vec3(0, Math.Sin(SDL.GetTicks() * 0.001) * 0.5, 0));
				}
				else
				{
					newBox = new Translate(rotateBox.Obj, new Vec3(0, Math.Sin(SDL.GetTicks() * 0.05 % 1), 0));
				}
				World[0] = new RotateY(newBox, SDL.GetTicks() * 0.05 % 360);
			}
		}
	}
}