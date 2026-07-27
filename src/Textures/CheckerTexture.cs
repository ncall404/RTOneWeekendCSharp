// CheckerTexture class for representing a texture that is a checkerboard pattern.

using System.Runtime.CompilerServices;
using RTOneWeekend.Core;

namespace RTOneWeekend.Textures;

public class CheckerTexture : Texture
{
	private readonly double _invScale;
	private readonly Texture _odd;
	private readonly Texture _even;

	public CheckerTexture(double scale, Texture even, Texture odd)
	{
		_odd = odd;
		_even = even;
		_invScale = 1.0 / scale;
	}

	public CheckerTexture(double scale, Vec3 color1, Vec3 color2) : this(scale, new SolidColor(color1), new SolidColor(color2)) {}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override Vec3 Value(double u, double v, Vec3 p)
	{
		int xInteger = (int)Math.Floor(p.X * _invScale);
		int yInteger = (int)Math.Floor(p.Y * _invScale);
		int zInteger = (int)Math.Floor(p.Z * _invScale);

		bool isEven = (xInteger + yInteger + zInteger) % 2 == 0;
		
		return isEven ? _even.Value(u, v, p) : _odd.Value(u, v, p);
	}
}