// Class for representing the hittable geometry of a sphere.

using RTOneWeekend.core;

namespace RTOneWeekend.geometry;

public class Sphere : Hittable
{
	private Vec3 center;
	private double radius;

	public Sphere(Vec3 center, double radius)
	{
		this.center = center;
		this.radius = Math.Max(0, radius);
	}

	public override bool Hit(in Ray r, double rayTMin, double rayTMax, ref HitRecord rec)
	{
		Vec3 oc = center - r.Origin;
        double a = r.Direction.LengthSquared();
        double h = Vec3.Dot(r.Direction, oc);
        double c = oc.LengthSquared() - radius*radius;

        double discriminant = h*h - a*c;

        if (discriminant < 0)
			return false;

		double sqrtDiscriminant = Math.Sqrt(discriminant);

		// Find the nearest root that lies in the acceptable range of rayTMin and rayTMax.
		double root = (h - sqrtDiscriminant) / a;
		if (root <= rayTMin || rayTMax <= root)
		{
			root = (h + sqrtDiscriminant) / a;
			if (root <= rayTMin || rayTMax <= root)
				return false;
		}

		rec.RayHitDistance = root;
		rec.P = r.At(rec.RayHitDistance);
		rec.Normal = (rec.P - center) / radius;

		return true;
	}
}