// Lambertian material class for representing diffuse surfaces in the scene.

using RTOneWeekend.Core;
using RTOneWeekend.Geometry;
using RTOneWeekend.Textures;

namespace RTOneWeekend.Materials;

class Lambertian : Material
{
	private readonly Texture _tex;
	public Lambertian(Vec3 albedo) => _tex = new SolidColor(albedo);
	public Lambertian(Texture texture) => _tex = texture;

	public override bool Scatter(Ray rayIn, HitRecord rec, out Vec3 attenuation, out Ray scattered)
	{
		Vec3 scatterDirection = rec.normal + Vec3.RandomUnitVector();

		// Catch scatter direction if too close to zero.
		if (scatterDirection.NearZero())
			scatterDirection = rec.normal;

		scattered = new Ray(rec.p, scatterDirection);
		attenuation = _tex.Value(rec.u, rec.v, rec.p);
		return true;
	}
}