// Material abstract class for representing materials in the scene.

using RTOneWeekend.Core;
using RTOneWeekend.Geometry;

namespace RTOneWeekend.Materials;

public class Material
{
	public Material() {}

	public virtual Vec3 Emitted(double u, double v, Vec3 p)
	{
		return new Vec3(0, 0, 0);
	}

	public virtual bool Scatter(Ray rayIn, HitRecord rec, out Vec3 attenuation, out Ray scattered)
	{
		scattered = new Ray(rec.p, new(0, 0, 0));
		attenuation = new(0, 0, 0);
		return false;
	}
}