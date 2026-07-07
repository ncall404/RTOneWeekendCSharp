// Hitrecord struct and Hittable abstract class for representing objects intersectable by rays in the raytracing.

namespace RTOneWeekend.core;

public ref struct HitRecord
{
	public Vec3 P; // Point of intersection.
	public Vec3 Normal;
	public double RayHitDistance; // Distance from ray origin to hit point; also known as t in the tutorial.
}

public abstract class Hittable
{
	public abstract bool Hit(in Ray r, double rayTMin, double rayTMax, ref HitRecord rec);
}