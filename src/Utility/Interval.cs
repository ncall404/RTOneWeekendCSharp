// A class representing an interval of doubles (min, max).

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

	public double Size() => Max - Min;

	public bool Contains(double x) => Min <= x && x <= Max;

	public bool Surrounds(double x) => Min < x && x < Max;

	public static readonly Interval Empty = new();
	public static readonly Interval Universe = new(double.NegativeInfinity, double.PositiveInfinity);
}