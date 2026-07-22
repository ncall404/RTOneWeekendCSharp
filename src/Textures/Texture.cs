// Texture abstract class for representing textures in the scene.

using RTOneWeekend.Core;

namespace RTOneWeekend.Textures;

public abstract class Texture
{
	public Texture() {}

	// Provides the color value of the texture at a particular coordinate.
	public abstract Vec3 Value(double u, double v, Vec3 p);
}