// Vec3 struct representing a point or vector in 3D space as well as colors that don't include an alpha value.

using System.Runtime.CompilerServices; // Using aggressive inlining since that apparently removes a lot of the function overhead that is normally there!

namespace RTOneWeekend.Core;

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
	public static Vec3 operator -(Vec3 v) => new(-v.X, -v.Y, -v.Z);
		// Binary
	public static Vec3 operator +(Vec3 a, Vec3 b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
	public static Vec3 operator -(Vec3 a, Vec3 b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
		// Component-wise multiplication
	public static Vec3 operator *(Vec3 a, Vec3 b) => new(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
		// Scalar
	public static Vec3 operator *(Vec3 v, double s) => new(v.X * s, v.Y * s, v.Z * s);
	public static Vec3 operator *(double s, Vec3 v) => new(v.X * s, v.Y * s, v.Z * s);
	public static Vec3 operator /(Vec3 v, double s) => new(v.X / s, v.Y / s, v.Z / s);

	// Vector-specific functions
	public double Length() => Math.Sqrt(LengthSquared());
	public double LengthSquared() => X * X + Y * Y + Z * Z;

	public static Vec3 Random()
	{
		return new Vec3(RandomNum.RandomDouble(), RandomNum.RandomDouble(), RandomNum.RandomDouble());
	}
	public static Vec3 Random(double min, double max)
	{
		return new Vec3(RandomNum.RandomDouble(min, max), RandomNum.RandomDouble(min, max), RandomNum.RandomDouble(min, max));
	}

	public static double Dot(Vec3 a, Vec3 b) => (a.X * b.X) + (a.Y * b.Y) + (a.Z * b.Z);

	public static Vec3 Cross(Vec3 a, Vec3 b)
	{
		return new Vec3(
			(a.Y * b.Z) - (a.Z * b.Y),
			(a.Z * b.X) - (a.X * b.Z),
			(a.X * b.Y) - (a.Y * b.X)
		);
	}

	public static Vec3 UnitVector(Vec3 v) => v / v.Length();

	// Uses the rejection method to generate random unit vectors. Simple but I imagine inefficient.
	// For reference, the rejection method keeps generating vectors until one is valid. Explained better here: https://raytracing.github.io/books/RayTracingInOneWeekend.html#diffusematerials/asimplediffusematerial
	public static Vec3 RandomUnitVector()
	{
		while (true)
		{
			Vec3 p = Random(-1, 1);
			double lengthSquared = p.LengthSquared();
			// 1e-160 < lengthSquared avoids a "small floating-point abstraction leak" that can happen from squaring too small of a floating point number.
			if (1e-160 < lengthSquared && lengthSquared <= 1)
			{
				return p / Math.Sqrt(lengthSquared);
			}
		}
	}

	// Determines if the random vector is in the same hemisphere as the normal. If not then it returns the opposite vector.
	public static Vec3 RandomOnHemisphere(Vec3 normal)
	{
		Vec3 onUnitSphere = RandomUnitVector();
		if (Dot(onUnitSphere, normal) > 0.0) // In the same hemisphere as the normal.
			return onUnitSphere;
		else
			return -onUnitSphere;
	}

	// Color-specific functions
		// Packs a color into a uint format compatible with the texture format being used for SDL.
	public static uint WriteColor(Vec3 color, byte alpha = 255)
	{
		// Translate from [0,1] to [0,255]
		Interval intensity = new(0.000, 0.999);
		byte r = (byte)(256 * intensity.Clamp(color.R));
		byte g = (byte)(256 * intensity.Clamp(color.G));
		byte b = (byte)(256 * intensity.Clamp(color.B));
		
		return (uint)((r << 24) | (g << 16) | (b << 8) | alpha);
	}

	// Utility
	public override string ToString() => $"({X}, {Y}, {Z})";
}