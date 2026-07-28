// Represents Scene 1: Three Material Spheres. Also found in Ray Tracing in One Weekend.
// This creates the scene from book 1 with 3 balls of different materials.

using RTOneWeekend.Core;
using RTOneWeekend.Geometry;
using RTOneWeekend.Materials;

namespace RTOneWeekend.Scenes;

public class Scene1 : Scene
{
	public override int ID { get; protected set; } = 1;
	public override string Name { get; protected set; } = "Three Material Spheres";

	public override HittableList World { get; protected set; }
	public override Camera Camera { get; protected set; }

	public Scene1()
	{
		(World, Camera) = LoadScene();
	}

	protected override (HittableList, Camera) LoadScene()
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
			700,					// Render width
			50,						// Samples per pixel
			100,					// Max depth (number of bounces for rays)
			40,						// Vertical field of view
			new(-2, 2, 1),			// Camera position.
			new(0, 0, -1),			// Look at point.
			new(0, 1, 0),			// Up vector.
			new(0.7, 0.8, 1),		// Background color.
			10,						// Defocus Angle (for depth of field, 0 = no depth of field)
			3.5						// Focus  distance (for depth of field)
		);

		return (world, camera);
	}

	public override void ProcessScene()
	{
		
	}
}