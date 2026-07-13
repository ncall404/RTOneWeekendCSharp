// Hitrecord struct and Hittable abstract class for representing objects intersectable by rays in the raytracing.

using RTOneWeekend.Core;
using RTOneWeekend.Materials;

namespace RTOneWeekend.Geometry;

public ref struct HitRecord
{
	public Vec3 P; // Point of intersection.
	public Vec3 Normal;
	public Material Material;
	public double RayHitDistance; // Distance from ray origin to hit point; also known as t in the tutorial.

	public bool FrontFace;

	// Sets the HitRecord normal vector based on if the ray is inside or outside of the hittable object.
	public void SetFaceNormal(in Ray r, in Vec3 outwardNormal)
	{
		FrontFace = Vec3.Dot(r.Direction, outwardNormal) < 0;
		Normal = FrontFace ? outwardNormal : -outwardNormal; // Outside of object : Inside of object
	}
}

public abstract class Hittable
{
	public abstract bool Hit(in Ray r, Interval rayT, ref HitRecord rec);
}