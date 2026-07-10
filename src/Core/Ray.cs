// Ray struct representing a ray that shoots from an origin point towards a direction.

using System.Runtime.CompilerServices;

namespace RTOneWeekend.Core;

public readonly struct Ray(Vec3 origin, Vec3 direction)
{
	public readonly Vec3 Origin = origin;
	public readonly Vec3 Direction = direction;

	public Vec3 At(double t)
	{
		return Origin + (Direction * t);
	}
}