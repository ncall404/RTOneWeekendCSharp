// Class for representing the hittable geometry of a Triangle. Implemented as suggested in Ray Tracing: The Next Week.

using RTOneWeekend.Core;
using RTOneWeekend.Materials;

namespace RTOneWeekend.Geometry;

public class Triangle : Quad
{
	public Triangle(Vec3 q, Vec3 u, Vec3 v, Material material) : base(q, u, v, material)
	{
	}

	// Given the hit point in plane coordinates, return false if it is outside the primitive, otherwise set the hit record UV coordinates and return true.
	public override bool IsInterior(double a, double b, ref HitRecord rec)
	{
		if (a <= 0 || b <= 0 || (a + b) >= 1)
			return false;

		rec.u = a;
		rec.v = b;
		return true;
	}
}