// Lambertian material class for representing diffuse surfaces in the scene.

using RTOneWeekend.Core;
using RTOneWeekend.Geometry;

namespace RTOneWeekend.Materials;

class Lambertian : Material
{
	private Vec3 Albedo;
	public Lambertian(Vec3 albedo) => Albedo = albedo;

	public override bool Scatter(Ray rayIn, HitRecord rec, out Vec3 attenuation, out Ray scattered)
	{
		Vec3 scatterDirection = rec.Normal + Vec3.RandomUnitVector();

		// Catch scatter direction if too close to zero.
		if (scatterDirection.NearZero())
			scatterDirection = rec.Normal;

		scattered = new Ray(rec.P, scatterDirection);
		attenuation = Albedo;
		return true;
	}
}