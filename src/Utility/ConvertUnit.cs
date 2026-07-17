// A static class containing useful utility functions and variables for doing unit conversions.

using System.Runtime.CompilerServices;

namespace RTOneWeekend.Utility;

public static class ConvertUnit
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double DegreesToRadians(double degrees) => degrees * Math.PI / 180;
}