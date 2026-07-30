// Class for Translating hittable geometry.

using RTOneWeekend.Core;

namespace RTOneWeekend.Geometry;

class Translate : Hittable
{
	public Hittable Obj { get; protected set; }
	public Vec3 Offset { get; set; }
	public override Aabb BoundingBox { get; protected set; }

	public Translate(Hittable obj, Vec3 offset)
	{
		Obj = obj;
		Offset = offset;
		BoundingBox = obj.BoundingBox + Offset;
	}

	public override bool Hit(in Ray r, Interval rayT, ref HitRecord rec)
	{
		// Move the ray backwards by the offset.
		Ray offsetRay = new(r.Origin - Offset, r.Direction);

		// Determine whether an intersection exists along the offset ray (and if so where).
		if (!Obj.Hit(offsetRay, rayT, ref rec))
			return false;
		
		// Move the intersection point forwards by the offset.
		rec.p += Offset;

		return true;
	}
}