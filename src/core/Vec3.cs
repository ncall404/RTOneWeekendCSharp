// Vec3 struct representing a point or vector in 3D space as well as colors that don't include an alpha value.

using System.Runtime.CompilerServices; // Using aggressive inlining since that apparently removes a lot of the function overhead that is normally there!

namespace RTOneWeekend.core;

public readonly struct Vec3(double x, double y, double z)
{
	public readonly double X = x;
	public readonly double Y = y;
	public readonly double Z = z;

	// Property aliases for RGB color usage.
	public double R => X;
	public double G => Y;
	public double B => Z;

	// Operators
		// Unary
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vec3 operator -(Vec3 v) => new(-v.X, -v.Y, -v.Z);
		// Binary
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vec3 operator +(Vec3 a, Vec3 b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vec3 operator -(Vec3 a, Vec3 b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
		// Component-wise multiplication
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vec3 operator *(Vec3 a, Vec3 b) => new(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
		// Scalar
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vec3 operator *(Vec3 v, double s) => new(v.X * s, v.Y * s, v.Z * s);
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vec3 operator *(double s, Vec3 v) => new(v.X * s, v.Y * s, v.Z * s);
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vec3 operator /(Vec3 v, double s) => new(v.X / s, v.Y / s, v.Z / s);

	// Vector-specific
	public double Length() => Math.Sqrt(LengthSquared());
	public double LengthSquared() => X * X + Y * Y + Z * Z;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double Dot(Vec3 a, Vec3 b) => (a.X * b.X) + (a.Y * b.Y) + (a.Z * b.Z);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vec3 Cross(Vec3 a, Vec3 b)
	{
		return new Vec3(
			(a.Y * b.Z) - (a.Z * b.Y),
			(a.Z * b.X) - (a.X * b.Z),
			(a.X * b.Y) - (a.Y * b.X)
		);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vec3 UnitVector(Vec3 v) => v / v.Length();

	// Utility
	public override string ToString() => $"({X}, {Y}, {Z})";
}