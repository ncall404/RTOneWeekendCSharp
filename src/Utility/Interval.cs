// A class representing an interval of doubles (min, max).

using System.Runtime.CompilerServices;

namespace RTOneWeekend.Utility;

public class Interval
{
	public double Min;
	public double Max;

	// Default interval is empty.
	public Interval()
	{
		Min = double.PositiveInfinity;
		Max = double.NegativeInfinity;
	}

	public Interval(double min, double max)
	{
		Min = min;
		Max = max;
	}

	// Create the interval that tightly surrounds two input intervals.
	public Interval(Interval a, Interval b)
	{
		Min = a.Min <= b.Min ? a.Min : b.Min;
		Max = a.Max >= b.Max ? a.Max : b.Max;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public double Size() => Max - Min;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Contains(double x) => Min <= x && x <= Max;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Surrounds(double x) => Min < x && x < Max;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public double Clamp(double x)
	{
		return Math.Clamp(x, Min, Max);
	}

	// Pad an interval by a given amount (delta).
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Interval Expand(double delta)
	{
		double padding = delta/2;
		return new(Min - padding, Max + padding);
	}

	public static readonly Interval Empty = new();
	public static readonly Interval Universe = new(double.NegativeInfinity, double.PositiveInfinity);
}