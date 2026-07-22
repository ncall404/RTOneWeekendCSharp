// Lambertian material class for representing diffuse surfaces in the scene.

using RTOneWeekend.Core;
using RTOneWeekend.Geometry;
using RTOneWeekend.Textures;

namespace RTOneWeekend.Materials;

class Lambertian : Material
{
	private Texture Tex;
	public Lambertian(Vec3 albedo) => Tex = new SolidColor(albedo);
	public Lambertian(Texture texture) => Tex = texture;

	public override bool Scatter(Ray rayIn, HitRecord rec, out Vec3 attenuation, out Ray scattered)
	{
		Vec3 scatterDirection = rec.Normal + Vec3.RandomUnitVector();

		// Catch scatter direction if too close to zero.
		if (scatterDirection.NearZero())
			scatterDirection = rec.Normal;

		scattered = new Ray(rec.P, scatterDirection);
		attenuation = Tex.Value(rec.U, rec.V, rec.P);
		return true;
	}
}