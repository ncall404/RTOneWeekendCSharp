// Represents Scene 2: Bouncing Spheres. Also found in Ray Tracing in One Weekend. Notably missing the bouncing in my version because I didn't care to implement motion blur from book 2 at the moment. :)
// This creates the scene from book 1 that is used for the final render. Named "Bouncing Spheres" in book 2.

using RTOneWeekend.Core;
using RTOneWeekend.Geometry;
using RTOneWeekend.Materials;
using RTOneWeekend.Textures;

namespace RTOneWeekend.Scenes;

public class Scene2 : Scene
{
	public override int ID { get; protected set; } = 2;
	public override string Name { get; protected set; } = "Bouncing Spheres";

	public override HittableList World { get; protected set; }
	public override Camera Camera { get; protected set; }

	public Scene2()
	{
		(World, Camera) = LoadScene();
	}

	protected override (HittableList, Camera) LoadScene()
	{
		HittableList world = new();

		Lambertian matGround = new Lambertian(new CheckerTexture(0.32, new Vec3(0.2, 0.3, 0.1), new Vec3(0.9, 0.9, 0.9)));
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

		Material mat2 = new Lambertian(new Vec3(0.4, 0.2, 0.1));
		world.Add(new Sphere(new(-4, 1, 0), 1.0, mat2));

		Material mat3 = new Metal(new(0.7, 0.6, 0.5), 0.0);
		world.Add(new Sphere(new(4, 1, 0), 1.0, mat3));

		world = new HittableList(new BvhNode(world));

		Camera camera = new(
			16.0 / 9.0,				// Aspect ratio
			700,					// Render width
			50,						// Samples per pixel
			50,						// Max depth (number of bounces for rays)
			20,						// Vertical field of view
			new(13, 2, 3),			// Camera position.
			new(0, 0, 0),			// Look at point.
			new(0, 1, 0),			// Up vector.
			0.6,					// Defocus Angle (for depth of field, 0 = no depth of field)
			10.0					// Focus  distance (for depth of field)
		);

		return (world, camera);
	}

	public override void ProcessScene()
	{
		
	}
}