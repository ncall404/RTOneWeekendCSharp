// Class for representing the hittable geometry of a quad (Quadrilateral or technically a parallelogram).

using RTOneWeekend.Core;
using RTOneWeekend.Materials;

namespace RTOneWeekend.Geometry;

public class Quad : Hittable
{
	private Vec3 _q; // The starting corner of the quad.
	private Vec3 _u; // The vector from the starting corner to the second corner.
	private Vec3 _v; // The vector from the starting corner to the third corner.
	private Material _material;
	public override Aabb BoundingBox { get; protected set; }

	// Planar values for the plane equation: Ax + By + Cz + D = 0
	private Vec3 _normal; // Normal vector of the plane (A, B, C)
	private Vec3 _w; // Vector by which the alpha and beta values are calculated for hit testing a point the planar shape.
	private double _d; // Dot product of normal vector with the starting corner: Dot(normal, _q); D in the equation.

	public Quad(Vec3 q, Vec3 u, Vec3 v, Material material)
	{
		_q = q;
		_u = u;
		_v = v;
		_material = material;

		Vec3 n = Vec3.Cross(_u, _v); // Get the normal vector via a cross product of the two side vectors.
		_normal = Vec3.UnitVector(n); // Normalize the normal vector.
		_d = Vec3.Dot(_normal, _q);
		_w = n / Vec3.Dot(n, n);

		// Compute the bounding box for all four vertices.
		Aabb bboxDiagonal1 = new(_q, _q + _u + _v);
		Aabb bboxDiagonal2 = new(_q + _u, _q + _v);
		BoundingBox = new(bboxDiagonal1, bboxDiagonal2);
	}

	public override bool Hit(in Ray r, Interval rayT, ref HitRecord rec)
	{
		double denom = Vec3.Dot(_normal, r.Direction);

		// No hit if the ray is parallel to the plane.
		if (Math.Abs(denom) < 1e-8)
			return false;
		
		// Return false if the hit point parameter t is outside the ray interval.
		double t = (_d - Vec3.Dot(_normal, r.Origin)) / denom;
		if (!rayT.Contains(t))
			return false;

		// Determine if the hit point lies within the planar shape using its plane coordinates.
		Vec3 intersectionPoint = r.At(t);
		Vec3 planarHitPtVector = intersectionPoint - _q;
		double alpha = Vec3.Dot(_w, Vec3.Cross(planarHitPtVector, _v));
		double beta = Vec3.Dot(_w, Vec3.Cross(_u, planarHitPtVector));

		if (!IsInterior(alpha, beta, ref rec))
			return false;

		// Ray hits the 2D shape; set the rest of the hit record and return true.
		rec.rayHitDistance = t;
		rec.p = intersectionPoint;
		rec.normal = _normal;
		rec.material = _material;

		return true;
	}

	// Given the hit point in plane coordinates, return false if it is outside the primitive, otherwise set the hit record UV coordinates and return true.
	public bool IsInterior(double a, double b, ref HitRecord rec)
	{
		Interval unitInterval = new(0, 1);

		if (!unitInterval.Contains(a) || !unitInterval.Contains(b))
			return false;

		rec.u = a;
		rec.v = b;
		return true;
	}
}