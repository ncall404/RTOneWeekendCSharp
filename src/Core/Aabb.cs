// AABB struct representing an axis-aligned bounding box.

using System.Runtime.CompilerServices;

namespace RTOneWeekend.Core;

public struct Aabb
{
	public Interval X;
	public Interval Y;
	public Interval Z;

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
		PadToMinimums();
	}

	// Treats the two Vec3s as extrema (Max/Min for interval across entire domain) for the bounding box so that a particular minimum/maximum coordinatre order is not required.
	public Aabb(Vec3 a, Vec3 b)
	{
		X = (a[0] <= b[0]) ? new(a[0], b[0]) : new(b[0], a[0]);
		Y = (a[1] <= b[1]) ? new(a[1], b[1]) : new(b[1], a[1]);
		Z = (a[2] <= b[2]) ? new(a[2], b[2]) : new(b[2], a[2]);

		PadToMinimums();
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
		Interval localRayT = new(rayT.min, rayT.max); // Create a copy of the input interval to avoid state leaking across function calls making it so that objects of the scene are not rendered.

		for (int axis = 0; axis < 3; axis++)
		{
			Interval ax = AxisInterval(axis);
			double adinv = 1.0 / rayDirection[axis]; // Axis direction inverse; AKA reciprocal ray direction.

			double t0 = (ax.min - rayOrigin[axis]) * adinv;
			double t1 = (ax.max - rayOrigin[axis]) * adinv;

			if (t0 < t1)
			{
				if (t0 > localRayT.min) localRayT.min = t0;
				if (t1 < localRayT.max) localRayT.max = t1;
			}
			else
			{
				if (t1 > localRayT.min) localRayT.min = t1;
				if (t0 < localRayT.max) localRayT.max = t0;
			}

			if (localRayT.max <= localRayT.min) return false;
		}
		return true;
	}

	// Returns the index of the longest axis of the bounding box.
	public int LongestAxis()
	{
		if (X.Size() > Y.Size())
		{
			return X.Size() > Z.Size() ? 0 : 2;
		}
		else
		{
			return Y.Size() > Z.Size() ? 1 : 2;
		}
	}

	public static readonly Aabb Empty = new();
	public static readonly Aabb Universe = new(Interval.Universe, Interval.Universe, Interval.Universe);

	// Adjust the AABB so that no side is narrower than some delta, padding if necessary. This helps avoid numerical issues with ray intersection.
	private void PadToMinimums()
	{
		double delta = 0.0001;
		if (X.Size() < delta) X = X.Expand(delta);
		if (Y.Size() < delta) Y = Y.Expand(delta);
		if (Z.Size() < delta) Z = Z.Expand(delta);
	}
}