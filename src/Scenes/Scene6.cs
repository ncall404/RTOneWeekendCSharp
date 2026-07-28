// Represents Scene 6: Quads. Also found in Ray Tracing: The Next Week.

using RTOneWeekend.Core;
using RTOneWeekend.Geometry;
using RTOneWeekend.Materials;
using RTOneWeekend.Textures;

namespace RTOneWeekend.Scenes;

public class Scene6 : Scene
{
	public override int ID { get; protected set; } = 6;
	public override string Name { get; protected set; } = "Quads";

	public override HittableList World { get; protected set; }
	public override Camera Camera { get; protected set; }

	public Scene6()
	{
		(World, Camera) = LoadScene();
	}

	protected override (HittableList, Camera) LoadScene()
	{
		HittableList world = new();

		// Materials
		Material leftRed = new Lambertian(new Vec3(1.0, 0.2, 0.2));
		Material backGreen = new Lambertian(new Vec3(0.2, 1.0, 0.2));
		Material rightBlue = new Lambertian(new Vec3(0.2, 0.2, 1.0));
		Material upperOrange = new Lambertian(new Vec3(1.0, 0.5, 0.0));
		Material lowerTeal = new Lambertian(new Vec3(0.2, 0.8, 0.8));

		// Quads
		world.Add(new Quad(new(-3, -2, 5), new(0, 0, -4), new(0, 4, 0), leftRed));
		world.Add(new Quad(new(-2, -2, 0), new(4, 0, 0), new(0, 4, 0), backGreen));
		world.Add(new Quad(new(3, -2, 1), new(0, 0, 4), new(0, 4, 0), rightBlue));
		world.Add(new Quad(new(-2, 3, 1), new(4, 0, 0), new(0, 0, 4), upperOrange));
		world.Add(new Quad(new(-2, -3, 5), new(4, 0, 0), new(0, 0, -4), lowerTeal));

		Camera camera = new(
			16.0 / 9.0,				// Aspect ratio
			1280,					// Render width
			100,					// Samples per pixel
			50,						// Max depth (number of bounces for rays)
			80,						// Vertical field of view
			new(0, 0, 9),			// Camera position.
			new(0, 0, 0),			// Look at point.
			new(0, 1, 0)			// Up vector.
		);

		return (world, camera);
	}

	public override void ProcessScene()
	{
		
	}
}