// Class for representing the hittable geometry of a sphere.

using RTOneWeekend.Core;

namespace RTOneWeekend.Geometry;

public class Sphere : Hittable
{
	private Vec3 center;
	private double radius;

	public Sphere(Vec3 center, double radius)
	{
		this.center = center;
		this.radius = Math.Max(0, radius);
	}

	public override bool Hit(in Ray r, Interval rayT, ref HitRecord rec)
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
		if (root <= rayT.Min || rayT.Max <= root)
		{
			root = (h + sqrtDiscriminant) / a;
			if (root <= rayT.Min || rayT.Max <= root)
				return false;
		}

		rec.RayHitDistance = root;
		rec.P = r.At(rec.RayHitDistance);
		Vec3 outwardNormal = (rec.P - center) / radius;
		rec.SetFaceNormal(r, outwardNormal);

		return true;
	}
}