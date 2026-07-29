// Class for representing the hittable geometry of a volume with a constant density.

using RTOneWeekend.Core;
using RTOneWeekend.Materials;
using RTOneWeekend.Textures;

namespace RTOneWeekend.Geometry;

class ConstantMedium : Hittable
{
	private Hittable _boundary;
	private Material _phaseFunction;
	private double negInvDensity;
	public override Aabb BoundingBox { get; protected set; }

	public ConstantMedium(Hittable boundary, double density, Texture texture)
	{
		_boundary = boundary;
		negInvDensity = -1.0 / density;
		_phaseFunction = new Isotropic(texture);
		BoundingBox = _boundary.BoundingBox;
	}

	public ConstantMedium(Hittable boundary, double density, Vec3 albedo)
	{
		_boundary = boundary;
		negInvDensity = -1.0 / density;
		_phaseFunction = new Isotropic(albedo);
	}

	public override bool Hit(in Ray r, Interval rayT, ref HitRecord rec)
	{
		HitRecord rec1 = new();
		HitRecord rec2 = new();

		if (!_boundary.Hit(r, Interval.Universe, ref rec1))
			return false;

		if (!_boundary.Hit(r, new Interval(rec1.rayHitDistance+0.0001, double.PositiveInfinity), ref rec2))
			return false;

		if (rec1.rayHitDistance < rayT.min) rec1.rayHitDistance = rayT.min;
		if (rec2.rayHitDistance > rayT.max) rec2.rayHitDistance = rayT.max;

		if (rec1.rayHitDistance >= rec2.rayHitDistance) return false;

		if (rec1.rayHitDistance < 0) rec1.rayHitDistance = 0;

		double rayLength = r.Direction.Length();
		double distanceInsideBoundary = (rec2.rayHitDistance - rec1.rayHitDistance) * rayLength;
		double hitDistance = negInvDensity * Math.Log(RandomNum.RandomDouble());

		if (hitDistance > distanceInsideBoundary)
			return false;

		rec.rayHitDistance = rec1.rayHitDistance + hitDistance / rayLength;
		rec.p = r.At(rec.rayHitDistance);

		rec.normal = new Vec3(1, 0, 0); // Arbitrary
		rec.frontFace = true; // Also arbitrary
		rec.material = _phaseFunction;

		return true;
	}
}