// An abstract class representing a scene.

using RTOneWeekend.Core;
using RTOneWeekend.Geometry;

namespace RTOneWeekend.Scenes;

public abstract class Scene
{
	public abstract int ID { get; protected set; }
	public abstract string Name { get; protected set; }
	public abstract HittableList World { get; protected set; }
	public abstract Camera Camera { get; protected set; }

	public Scene() {}

	protected abstract (HittableList, Camera) LoadScene();
	public abstract void ProcessScene();
}