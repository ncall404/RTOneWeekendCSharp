// A class representing an interval of doubles (min, max).

using System.Runtime.CompilerServices;

namespace RTOneWeekend.Utility;

public struct Interval
{
	public double min;
	public double max;

	// Default interval is empty.
	public Interval()
	{
		min = double.PositiveInfinity;
		max = double.NegativeInfinity;
	}

	public Interval(double min, double max)
	{
		this.min = min;
		this.max = max;
	}

	// Create the interval that tightly surrounds two input intervals.
	public Interval(Interval a, Interval b)
	{
		min = a.min <= b.min ? a.min : b.min;
		max = a.max >= b.max ? a.max : b.max;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public double Size() => max - min;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Contains(double x) => min <= x && x <= max;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Surrounds(double x) => min < x && x < max;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public double Clamp(double x)
	{
		return Math.Clamp(x, min, max);
	}

	// Pad an interval by a given amount (delta).
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Interval Expand(double delta)
	{
		double padding = delta/2;
		return new(min - padding, max + padding);
	}

	public static readonly Interval Empty = new();
	public static readonly Interval Universe = new(double.NegativeInfinity, double.PositiveInfinity);
}