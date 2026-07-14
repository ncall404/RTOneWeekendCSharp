// Lambertian material class for representing reflective surfaces in the scene.

using RTOneWeekend.Core;
using RTOneWeekend.Geometry;

namespace RTOneWeekend.Materials;

class Metal: Material
{
    private Vec3 Albedo;
	private double Fuzz; // Reduces reflectivity. Higher = more matte.
	public Metal(Vec3 albedo, double fuzz)
	{
		Albedo = albedo;
		Fuzz = fuzz < 1 ? fuzz : 1;
	}
    
	public override bool Scatter(Ray rayIn, HitRecord rec, out Vec3 attenuation, out Ray scattered)
	{
		Vec3 reflected = Vec3.Reflect(rayIn.Direction, rec.Normal);
		reflected = Vec3.UnitVector(reflected) + (Fuzz * Vec3.RandomUnitVector());
		scattered = new Ray(rec.P, reflected);
		attenuation = Albedo;
		return Vec3.Dot(scattered.Direction, rec.Normal) > 0;
	}
}