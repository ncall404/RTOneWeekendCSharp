// Class for representing the hittable geometry of a sphere.

using RTOneWeekend.Core;
using RTOneWeekend.Materials;

namespace RTOneWeekend.Geometry;

public class Sphere : Hittable
{
	private readonly Vec3 _center;
	private readonly double _radius;
	private readonly Material _material;
	public override Aabb BoundingBox { get; protected set; }
	public Vec3 RotationOffset { get; set; } = new(0, 0, 0); // U, V, Time.

	public Sphere(Vec3 center, double radius, Material material)
	{
		_center = center;
		_radius = Math.Max(0, radius);
		_material = material;

		// Create the bounding box. NOTE: If center or radius are made dynamic in the real-time mode then send this to it's own function that is called when those are changed as well.
		Vec3 radiusVec = new(radius, radius, radius);
		BoundingBox = new(center - radiusVec, center + radiusVec);
	}

	public override bool Hit(in Ray r, Interval rayT, ref HitRecord rec)
	{
		Vec3 oc = _center - r.Origin;
        double a = r.Direction.LengthSquared();
        double h = Vec3.Dot(r.Direction, oc);
        double c = oc.LengthSquared() - _radius*_radius;

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
		Vec3 outwardNormal = (rec.P - _center) / _radius;
		rec.SetFaceNormal(r, outwardNormal);
		(rec.U, rec.V) = GetSphereUV(outwardNormal); // Update hitrecord uv coordinates.
		rec.Material = _material;

		return true;
	}

	// Get the sampled point on a sphere.
	private (double, double) GetSphereUV(Vec3 p)
	{
		// p: a given point on the sphere of radius one, centered at the origin.
		// u: returned value [0,1] of angle around the Y axis from X=-1.
		// v: returned value [0,1] of angle from Y=-1 to Y=+1.
		// <1 0 0> yields <0.50, 0.50>	<-1 0 0> yields <0.00, 0.50>
		// <0 1 0> yields <0.50, 1.00>	<0 -1 0> yields <0.50, 0.00>
		// <0 0 1> yields <0.25, 0.50>	<0 0 -1> yields <0.75, 0.50>

		double theta = Math.Acos(-p.Y);
		double phi = Math.Atan2(-p.Z, p.X) + Math.PI;

		double u = (phi / (2 * Math.PI)) + (RotationOffset.X * RotationOffset.Z);
		double v = (theta / Math.PI) + (RotationOffset.Y * RotationOffset.Z);

		// Wrap the UV coordinates to be between 0 and 1 to prevent smearing in motion.
		u -= Math.Floor(u);
		v -= Math.Floor(v);

		return (u, v);
	}

	public void UpdateRotationOffset(double time)
	{
		RotationOffset = new Vec3(RotationOffset.X, RotationOffset.Y, time);
	}
}