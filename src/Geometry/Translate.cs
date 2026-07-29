// Class for Translating hittable geometry.

using RTOneWeekend.Core;

namespace RTOneWeekend.Geometry;

class Translate : Hittable
{
	private Hittable _obj;
	private Vec3 _offset;
	public override Aabb BoundingBox { get; protected set; }

	public Translate(Hittable obj, Vec3 offset)
	{
		_obj = obj;
		BoundingBox = obj.BoundingBox + offset;
	}

	public override bool Hit(in Ray r, Interval rayT, ref HitRecord rec)
	{
		// Move the ray backwards by the offset.
		Ray offsetRay = new(r.Origin - _offset, r.Direction);

		// Determine whether an intersection exists along the offset ray (and if so where).
		if (!_obj.Hit(offsetRay, rayT, ref rec))
			return false;
		
		// Move the intersection point forwards by the offset.
		rec.p += _offset;

		return true;
	}
}