// Class for representing the hittable geometry of a sphere.

using RTOneWeekend.Core;
using RTOneWeekend.Materials;

namespace RTOneWeekend.Geometry;

public class Sphere : Hittable
{
	private Vec3 center;
	private double radius;
	private Material material;
	public override Aabb BoundingBox { get; protected set; }

	public Sphere(Vec3 center, double radius, Material material)
	{
		this.center = center;
		this.radius = Math.Max(0, radius);
		this.material = material;

		// Create the bounding box. NOTE: If center or radius are made dynamic in the real-time mode then send this to it's own function that is called when those are changed as well.
		Vec3 radiusVec = new(radius, radius, radius);
		BoundingBox = new(center - radiusVec, center + radiusVec);
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
		rec.Material = material;

		return true;
	}
}