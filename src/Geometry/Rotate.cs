// Classes for Rotating hittable geometry.

using RTOneWeekend.Core;

namespace RTOneWeekend.Geometry;

class RotateY : Hittable
{
	public Hittable Obj { get; protected set; }
	private double _sinTheta;
	private double _cosTheta;
	public override Aabb BoundingBox { get; protected set; }

	public RotateY(Hittable obj, double angle)
	{
		Obj = obj;

		double radians = ConvertUnit.DegreesToRadians(angle);
		_sinTheta = Math.Sin(radians);
		_cosTheta = Math.Cos(radians);

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

					double newX = (_cosTheta * x) - (_sinTheta * z);
					double newZ = (_sinTheta * x) + (_cosTheta * z);

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
			(_cosTheta * r.Origin.X) - (_sinTheta * r.Origin.Z),
			r.Origin.Y,
			(_sinTheta * r.Origin.X) + (_cosTheta * r.Origin.Z)
		);
		Vec3 direction = new Vec3(
			(_cosTheta * r.Direction.X) - (_sinTheta * r.Direction.Z),
			r.Direction.Y,
			(_sinTheta * r.Direction.X) + (_cosTheta * r.Direction.Z)
		);

		Ray rotatedRay = new(origin, direction);

		// Determine whether an intersection exists in object space (and if so where).
		if (!Obj.Hit(rotatedRay, rayT, ref rec))
			return false;

		// Transform the intersection from object space back to world space.
		rec.p = new Vec3(
			(_cosTheta * rec.p.X) + (_sinTheta * rec.p.Z),
			rec.p.Y,
			(-_sinTheta * rec.p.X) + (_cosTheta * rec.p.Z)
		);

		rec.normal = new Vec3(
			(_cosTheta * rec.normal.X) + (_sinTheta * rec.normal.Z),
			rec.normal.Y,
			(-_sinTheta * rec.normal.X) + (_cosTheta * rec.normal.Z)
		);

		return true;
	}
}