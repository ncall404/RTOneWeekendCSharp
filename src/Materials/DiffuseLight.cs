// DiffuseLight material class for representing lights in the scene.

using RTOneWeekend.Core;
using RTOneWeekend.Geometry;
using RTOneWeekend.Textures;

namespace RTOneWeekend.Materials;

class DiffuseLight : Material
{
	private readonly Texture _tex;
	public DiffuseLight(Vec3 albedo) => _tex = new SolidColor(albedo);
	public DiffuseLight(Texture texture) => _tex = texture;

	public override Vec3 Emitted(double u, double v, Vec3 p)
	{
		return _tex.Value(u, v, p);
	}
}