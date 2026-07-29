// Isotropic material class for representing volumetric surfaces in the scene.

using RTOneWeekend.Core;
using RTOneWeekend.Geometry;
using RTOneWeekend.Textures;

namespace RTOneWeekend.Materials;

class Isotropic : Material
{
	private readonly Texture _tex;

	public Isotropic(Vec3 albedo) => _tex = new SolidColor(albedo);
	public Isotropic(Texture texture) => _tex = texture;

	public override bool Scatter(Ray rayIn, HitRecord rec, out Vec3 attenuation, out Ray scattered)
	{
		scattered = new Ray(rec.p, Vec3.RandomUnitVector());
		attenuation = _tex.Value(rec.u, rec.v, rec.p);
		return true;
	}
}