// Dielectric material class for representing refractive surfaces in the scene.

using RTOneWeekend.Core;
using RTOneWeekend.Geometry;

namespace RTOneWeekend.Materials;

class Dielectric : Material
{
    private readonly double _refractionIndex; // Either the refractive index in vacuum/air or the ratio of the materials refractive index over the refractive index of the enclosing material.

	public Dielectric(double refractionIndex)
	{
		_refractionIndex = refractionIndex;
	}

	public override bool Scatter(Ray rayIn, HitRecord rec, out Vec3 attenuation, out Ray scattered)
	{
		attenuation = new Vec3(1.0, 1.0, 1.0);
		double ri = rec.frontFace ? (1.0/_refractionIndex) : _refractionIndex; // Refractive index ratio depending on if ray is hitting the front face of the object.

		Vec3 unitDirection = Vec3.UnitVector(rayIn.Direction);
		double cosTheta = Math.Min(Vec3.Dot(-unitDirection, rec.normal), 1.0);
		double sinTheta = Math.Sqrt(1.0 - cosTheta*cosTheta);

		bool cannotRefract = ri * sinTheta > 1.0;
		Vec3 direction;

		if (cannotRefract || Reflectance(cosTheta, ri) > RandomNum.RandomDouble())
			direction = Vec3.Reflect(unitDirection, rec.normal);
		else
			direction = Vec3.Refract(unitDirection, rec.normal, ri);

		scattered = new Ray(rec.p, direction);
		return true;
	}

	// Use Schlick's approximation for reflectance.
	private static double Reflectance(double cosine, double refractionIndex)
	{
		double r0 = (1 - refractionIndex) / (1 + refractionIndex);
		r0 *= r0;
		return r0 + (1-r0)*Math.Pow(1-cosine, 5);
	}
}