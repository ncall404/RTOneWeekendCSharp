// Dielectric material class for representing refractive surfaces in the scene.

using RTOneWeekend.Core;
using RTOneWeekend.Geometry;

namespace RTOneWeekend.Materials;

class Dielectric : Material
{
    private double RefractionIndex;

	public Dielectric(double refractionIndex)
	{
		RefractionIndex = refractionIndex;
	}

	public override bool Scatter(Ray rayIn, HitRecord rec, out Vec3 attenuation, out Ray scattered)
	{
		attenuation = new Vec3(1.0, 1.0, 1.0);
		double ri = rec.FrontFace ? (1.0/RefractionIndex) : RefractionIndex;

		Vec3 unitDirection = Vec3.UnitVector(rayIn.Direction);
		Vec3 refracted = Vec3.Refract(unitDirection, rec.Normal, ri);

		scattered = new Ray(rec.P, refracted);
		return true;
	}
}