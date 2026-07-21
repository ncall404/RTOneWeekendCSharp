// AABB struct representing an axis-aligned bounding box.

using System.Runtime.CompilerServices;

namespace RTOneWeekend.Core;

public readonly struct Aabb
{
	public readonly Interval X;
	public readonly Interval Y;
	public readonly Interval Z;

	// The default AABB is empty, since intervals are empty by default.
	public Aabb()
	{
		X = Interval.Empty;
		Y = Interval.Empty;
		Z = Interval.Empty;
	}

	public Aabb(Interval x, Interval y, Interval z)
	{
		X = x;
		Y = y;
		Z = z;
	}

	// Treats the two Vec3s as extrema (Max/Min for interval across entire domain) for the bounding box so that a particular minimum/maximum coordinatre order is not required.
	public Aabb(Vec3 a, Vec3 b)
	{
		X = (a[0] <= b[0]) ? new(a[0], b[0]) : new(b[0], a[0]);
		Y = (a[1] <= b[1]) ? new(a[1], b[1]) : new(b[1], a[1]);
		Z = (a[2] <= b[2]) ? new(a[2], b[2]) : new(b[2], a[2]);
	}

	// Create the bounding box that tightly surrounds two input bounding boxes.
	public Aabb(Aabb box0, Aabb box1)
	{
		X = new Interval(box0.X, box1.X);
		Y = new Interval(box0.Y, box1.Y);
		Z = new Interval(box0.Z, box1.Z);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Interval AxisInterval(int n)
	{
		if (n == 1) return Y;
		if (n == 2) return Z;
		return X;
	}

	public bool Hit(Ray r, Interval rayT)
	{
		Vec3 rayOrigin = r.Origin;
		Vec3 rayDirection = r.Direction;
		Interval localRayT = new(rayT.Min, rayT.Max); // Create a copy of the input interval to avoid state leaking across function calls making it so that objects of the scene are not rendered.

		for (int axis = 0; axis < 3; axis++)
		{
			Interval ax = AxisInterval(axis);
			double adinv = 1.0 / rayDirection[axis]; // Axis direction inverse; AKA reciprocal ray direction.

			double t0 = (ax.Min - rayOrigin[axis]) * adinv;
			double t1 = (ax.Max - rayOrigin[axis]) * adinv;

			if (t0 < t1)
			{
				if (t0 > localRayT.Min) localRayT.Min = t0;
				if (t1 < localRayT.Max) localRayT.Max = t1;
			}
			else
			{
				if (t1 > localRayT.Min) localRayT.Min = t1;
				if (t0 < localRayT.Max) localRayT.Max = t0;
			}

			if (localRayT.Max <= localRayT.Min) return false;
		}
		return true;
	}
}