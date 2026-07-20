// A static class containing useful utility functions and variables for getting random numbers.

using System.Runtime.CompilerServices;

namespace RTOneWeekend.Utility;

public static class RandomNum
{
	// Thread static variable to store a local instance of the Random object for each thread. Useful for whenever I multithread my code.
	[ThreadStatic]
	private static Random? _threadRandom;
	private static Random ThreadRandom => _threadRandom ??= new Random();

	// Random double from 0 to 1.
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double RandomDouble()
	{
		return ThreadRandom.NextDouble();
	}

	// Random double from min to max.
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double RandomDouble(double min, double max)
	{
		return min + (max - min) * RandomDouble();
	}

	// Random int from min to max.
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int RandomInt(int min, int max)
	{
		return ThreadRandom.Next(min, max);
	}
}