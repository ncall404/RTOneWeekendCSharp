// Material abstract class for representing materials in the scene.

using RTOneWeekend.Core;
using RTOneWeekend.Geometry;

namespace RTOneWeekend.Materials;

public abstract class Material
{
	public Material() {}
	public abstract bool Scatter(Ray rayIn, HitRecord rec, out Vec3 attenuation, out Ray scattered);
}