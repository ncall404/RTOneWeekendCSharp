// Lambertian material class for representing reflective surfaces in the scene.

using RTOneWeekend.Core;
using RTOneWeekend.Geometry;

namespace RTOneWeekend.Materials;

class Metal: Material
{
    private Vec3 Albedo;
	public Metal(Vec3 albedo) => Albedo = albedo;
    
	public override bool Scatter(Ray rayIn, HitRecord rec, out Vec3 attenuation, out Ray scattered)
	{
		Vec3 reflected = Vec3.Reflect(rayIn.Direction, rec.Normal);
		scattered = new Ray(rec.P, reflected);
		attenuation = Albedo;
		return true;
	}
}