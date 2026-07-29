// Classes for Rotating hittable geometry.

using RTOneWeekend.Core;

namespace RTOneWeekend.Geometry;

class RotateY : Hittable
{
	private Hittable _obj;
	private double sinTheta;
	private double cosTheta;
	public override Aabb BoundingBox { get; protected set; }

	public RotateY(Hittable obj, double angle)
	{
		_obj = obj;

		double radians = ConvertUnit.DegreesToRadians(angle);
		sinTheta = Math.Sin(radians);
		cosTheta = Math.Cos(radians);

		BoundingBox = obj.BoundingBox;

		Vec3 min = new(double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity);
		Vec3 max = new(double.NegativeInfinity, double.NegativeInfinity, double.NegativeInfinity);

		for (int i = 0; i < 2; i++)
		{
			for (int j = 0; j < 2; j++)
			{
				for (int k = 0; k < 2; k++)
				{
					double x = i * BoundingBox.X.max + (1 - i) * BoundingBox.X.min;
					double y = j * BoundingBox.Y.max + (1 - j) * BoundingBox.Y.min;
					double z = k * BoundingBox.Z.max + (1 - k) * BoundingBox.Z.min;

					double newX = (cosTheta * x) - (sinTheta * z);
					double newZ = (sinTheta * x) + (cosTheta * z);

					Vec3 tester = new(newX, y, newZ);

					for (int c = 0; c < 3; c++)
					{
						min[c] = Math.Min(min[c], tester[c]);
						max[c] = Math.Max(max[c], tester[c]);
					}
				}
			}
		}

		BoundingBox = new Aabb(min, max);
	}

	public override bool Hit(in Ray r, Interval rayT, ref HitRecord rec)
	{
		// Transform the ray from world space to object space.
		Vec3 origin = new Vec3(
			(cosTheta * r.Origin.X) - (sinTheta * r.Origin.Z),
			r.Origin.Y,
			(sinTheta * r.Origin.X) + (cosTheta * r.Origin.Z)
		);
		Vec3 direction = new Vec3(
			(cosTheta * r.Direction.X) - (sinTheta * r.Direction.Z),
			r.Direction.Y,
			(sinTheta * r.Direction.X) + (cosTheta * r.Direction.Z)
		);

		Ray rotatedRay = new(origin, direction);

		// Determine whether an intersection exists in object space (and if so where).
		if (!_obj.Hit(rotatedRay, rayT, ref rec))
			return false;

		// Transform the intersection from object space back to world space.
		rec.p = new Vec3(
			(cosTheta * rec.p.X) + (sinTheta * rec.p.Z),
			rec.p.Y,
			(-sinTheta * rec.p.X) + (cosTheta * rec.p.Z)
		);

		rec.normal = new Vec3(
			(cosTheta * rec.normal.X) + (sinTheta * rec.normal.Z),
			rec.normal.Y,
			(-sinTheta * rec.normal.X) + (cosTheta * rec.normal.Z)
		);

		return true;
	}
}