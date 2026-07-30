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