// Represents Scene 8: Simple Light. Also found in Ray Tracing: The Next Week.

using RTOneWeekend.Core;
using RTOneWeekend.Geometry;
using RTOneWeekend.Materials;
using RTOneWeekend.Textures;

namespace RTOneWeekend.Scenes;

public class Scene8 : Scene
{
	public override int ID { get; protected set; } = 5;
	public override string Name { get; protected set; } = "Simple Light";

	public override HittableList World { get; protected set; }
	public override Camera Camera { get; protected set; }

	public Scene8()
	{
		(World, Camera) = LoadScene();
	}

	protected override (HittableList, Camera) LoadScene()
	{
		HittableList world = new();

		MarbleNoiseTexture marbleTexture = new(4, 7);

		world.Add(new Sphere(new(0, -1000, 0), 1000, new Lambertian(marbleTexture)));
		world.Add(new Sphere(new(0, 2, 0), 2, new Lambertian(marbleTexture)));

		DiffuseLight diffuseLight = new(new Vec3(4, 4, 4));
		world.Add(new Quad(new(3, 1, -2), new(2, 0, 0), new(0, 2, 0), diffuseLight));

		Camera camera = new(
			16.0 / 9.0,				// Aspect ratio
			1280,					// Render width
			100,					// Samples per pixel
			50,						// Max depth (number of bounces for rays)
			20,						// Vertical field of view
			new(26, 3, 6),			// Camera position.
			new(0, 2, 0),			// Look at point.
			new(0, 1, 0),			// Up vector.
			new(0, 0, 0)			// Background color.
		);

		return (world, camera);
	}

	public override void ProcessScene()
	{
		
	}
}