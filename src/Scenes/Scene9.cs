// Represents Scene 9: Cornell Box. Also found in Ray Tracing: The Next Week.

using RTOneWeekend.Core;
using RTOneWeekend.Geometry;
using RTOneWeekend.Materials;

namespace RTOneWeekend.Scenes;

public class Scene9 : Scene
{
	public override int ID { get; protected set; } = 9;
	public override string Name { get; protected set; } = "Cornell Box";

	public override HittableList World { get; protected set; }
	public override Camera Camera { get; protected set; }

	public Scene9()
	{
		(World, Camera) = LoadScene();
	}

	protected override (HittableList, Camera) LoadScene()
	{
		HittableList world = new();

		// Materials
		Material red = new Lambertian(new Vec3(0.65, 0.05, 0.05));
		Material white = new Lambertian(new Vec3(0.73, 0.73, 0.73));
		Material green = new Lambertian(new Vec3(0.12, 0.45, 0.15));
		Material light = new DiffuseLight(new Vec3(15, 15, 15));

		// Quads
		world.Add(new Quad(new(555, 0, 0), new(0, 555, 0), new(0, 0, 555), green));
		world.Add(new Quad(new(0, 0, 0), new(0, 555, 0), new(0, 0, 555), red));
		world.Add(new Quad(new(343, 554, 332), new(-130, 0, 0), new(0, 0, -105), light));
		world.Add(new Quad(new(0, 0, 0), new(555, 0, 0), new(0, 0, 555), white));
		world.Add(new Quad(new(555, 555, 555), new(-555, 0, 0), new(0, 0, -555), white));
		world.Add(new Quad(new(0, 0, 555), new(555, 0, 0), new(0, 555, 0), white));

		Hittable box1 = Quad.Box(new(0, 0, 0), new(165, 330, 165), white);
		box1 = new RotateY(box1, 15);
		box1 = new Translate(box1, new(265, 0, 295));
		world.Add(box1);

		Hittable box2 = Quad.Box(new(0, 0, 0), new(165, 165, 165), white);
		box2 = new RotateY(box2, -18);
		box2 = new Translate(box2, new(130, 0, 65));
		world.Add(box2);

		Camera camera = new(
			16.0 / 9.0,				// Aspect ratio
			1280,					// Render width
			200,					// Samples per pixel
			50,						// Max depth (number of bounces for rays)
			40,						// Vertical field of view
			new(278, 278, -800),	// Camera position
			new(278, 278, 0),		// Look at point
			new(0, 1, 0),			// Up vector
			new(0, 0, 0)			// Background color
		);

		return (world, camera);
	}

	public override void ProcessScene()
	{
		
	}
}