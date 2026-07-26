// Represents Scene 3: Checkered Spheres. Also found in Ray Tracing: The Next Week.

using RTOneWeekend.Core;
using RTOneWeekend.Geometry;
using RTOneWeekend.Materials;
using RTOneWeekend.Textures;

namespace RTOneWeekend.Scenes;

public class Scene3 : Scene
{
	public override int ID { get; protected set; } = 3;
	public override string Name { get; protected set; } = "Checkered Spheres";

	public override HittableList World { get; protected set; }
	public override Camera Camera { get; protected set; }

	public Scene3()
	{
		(World, Camera) = LoadScene();
	}

	protected override (HittableList, Camera) LoadScene()
	{
		HittableList world = new();

		CheckerTexture checker = new(0.32, new Vec3(0.2, 0.3, 0.1), new Vec3(0.9, 0.9, 0.9));

		world.Add(new Sphere(new(0, -10, 0), 10, new Lambertian(checker)));
		world.Add(new Sphere(new(0, 10, 0), 10, new Lambertian(checker)));

		Camera camera = new(
			16.0 / 9.0,				// Aspect ratio
			700,					// Render width
			50,						// Samples per pixel
			50,						// Max depth (number of bounces for rays)
			20,						// Vertical field of view
			new(13, 2, 3),			// Camera position.
			new(0, 0, 0),			// Look at point.
			new(0, 1, 0)			// Up vector.
		);

		return (world, camera);
	}

	public override void ProcessScene()
	{
		
	}
}