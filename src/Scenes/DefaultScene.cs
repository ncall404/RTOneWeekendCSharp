// Represents a defualt scene to load to. This is used when the user has not selected a scene and allows a quick startup without having to select a scene that will take time to render.

using RTOneWeekend.Core;
using RTOneWeekend.Geometry;

namespace RTOneWeekend.Scenes;

public class DefaultScene : Scene
{
	public override int ID { get; protected set; } = 0;
	public override string Name { get; protected set; } = "Empty";

	public override HittableList World { get; protected set; }
	public override Camera Camera { get; protected set; }

	public DefaultScene()
	{
		(World, Camera) = LoadScene();
	}

	protected override (HittableList, Camera) LoadScene()
	{
		// Returns an emptyr world and standard camera.
		return (
			new(), 
			new(16.0/9.0, 10, 1, 1, 40, new(0, 0, 0), new(0, 0, -1), new(0, 1, 0), new(0.7, 0.8, 1))
		);
	}

	public override void ProcessScene()
	{
		
	}
}