// Metal material class for representing reflective surfaces in the scene.

using RTOneWeekend.Core;
using RTOneWeekend.Geometry;

namespace RTOneWeekend.Materials;

class Metal: Material
{
    private readonly Vec3 _albedo;
	private readonly double _fuzz; // Reduces reflectivity. Higher = more matte.
	public Metal(Vec3 albedo, double fuzz)
	{
		_albedo = albedo;
		_fuzz = fuzz < 1 ? fuzz : 1;
	}
    
	public override bool Scatter(Ray rayIn, HitRecord rec, out Vec3 attenuation, out Ray scattered)
	{
		Vec3 reflected = Vec3.Reflect(rayIn.Direction, rec.Normal);
		reflected = Vec3.UnitVector(reflected) + (_fuzz * Vec3.RandomUnitVector());
		scattered = new Ray(rec.P, reflected);
		attenuation = _albedo;
		return Vec3.Dot(scattered.Direction, rec.Normal) > 0;
	}
}