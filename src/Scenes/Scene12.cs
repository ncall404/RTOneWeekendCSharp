// Represents Scene 12: Final Scene. Or as I will call it, Book 2 Final. Also found in Ray Tracing: The Next Week; though instead of a moving sphere, I use a volumetric sphere.

using RTOneWeekend.Core;
using RTOneWeekend.Geometry;
using RTOneWeekend.Materials;
using RTOneWeekend.Textures;

namespace RTOneWeekend.Scenes;

public class Scene12 : Scene
{
	public override int ID { get; protected set; } = 12;
	public override string Name { get; protected set; } = "Book 2 Final";

	public override HittableList World { get; protected set; }
	public override Camera Camera { get; protected set; }

	public Scene12()
	{
		(World, Camera) = LoadScene();
	}

	protected override (HittableList, Camera) LoadScene()
	{
		HittableList world = new();

		HittableList boxes1 = new();
		Material ground = new Lambertian(new Vec3(0.48, 0.83, 0.53));

		int boxesPerSide = 20;
		for (int i = 0; i < boxesPerSide; i++)
		{
			for (int j = 0; j < boxesPerSide; j++)
			{
				double w = 100;
				double x0 = -1000 + i * w;
				double z0 = -1000 + j * w;
				double y0 = 0;
				double x1 = x0 + w;
				double y1 = RandomNum.RandomDouble(1, 101);
				double z1 = z0 + w;

				boxes1.Add(Quad.Box(new(x0, y0, z0), new(x1, y1, z1), ground));
			}
		}

		world.Add(new BvhNode(boxes1));

		Material light = new DiffuseLight(new Vec3(7, 7, 7));
		world.Add(new Quad(new Vec3(123, 554, 147), new Vec3(300, 0, 0), new Vec3(0, 0, 265), light));

		Material sphereMaterial = new Lambertian(new Vec3(0.7, 0.3, 0.1));
		world.Add(new Sphere(new Vec3(400, 400, 200), 50, sphereMaterial));

		world.Add(new Sphere(new Vec3(260, 150, 45), 50, new Dielectric(1.5)));
		world.Add(new Sphere(new Vec3(0, 150, 145), 50, new Metal(new Vec3(0.8, 0.8, 0.9), 1.0)));

		Sphere boundary = new Sphere(new Vec3(360, 150, 145), 70, new Dielectric(1.5));
		world.Add(boundary);
		world.Add(new ConstantMedium(boundary, 0.2, new Vec3(0.2, 0.4, 0.9)));
		boundary = new Sphere(new Vec3(0, 0, 0), 5000, new Dielectric(1.5));
		world.Add(new ConstantMedium(boundary, 0.0001, new Vec3(1, 1, 1)));

		Material earthMaterial = new Lambertian(new ImageTexture("./assets/SampleTextures/earthmap.jpg"));
		world.Add(new Sphere(new Vec3(400, 200, 400), 100, earthMaterial));
		Texture perlinTexture = new MarbleNoiseTexture(0.2, 7);
		world.Add(new Sphere(new Vec3(220, 280, 300), 80, new Lambertian(perlinTexture)));

		HittableList boxes2 = new();
		Material white = new Lambertian(new Vec3(0.73, 0.73, 0.73));
		int ns = 1000;
		for (int j = 0; j < ns; j++)
		{
			boxes2.Add(new Sphere(Vec3.Random(0, 165), 10, white));
		}

		world.Add(new Translate(new RotateY(new BvhNode(boxes2), 15), new Vec3(-100, 270, 395)));

		Camera camera = new(
			16.0 / 9.0,				// Aspect ratio
			700,					// Render width
			250,					// Samples per pixel
			40,						// Max depth (number of bounces for rays)
			40,						// Vertical field of view
			new(478, 278, -600),	// Camera position
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