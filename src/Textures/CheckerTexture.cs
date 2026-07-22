// CheckerTexture class for representing a texture that is a checkerboard pattern.

using System.Runtime.CompilerServices;
using RTOneWeekend.Core;

namespace RTOneWeekend.Textures;

public class CheckerTexture : Texture
{
	private double InvScale;
	private Texture Odd;
	private Texture Even;

	public CheckerTexture(double scale, Texture even, Texture odd)
	{
		Odd = odd;
		Even = even;
		InvScale = 1.0 / scale;
	}

	public CheckerTexture(double scale, Vec3 color1, Vec3 color2) : this(scale, new SolidColor(color1), new SolidColor(color2)) {}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override Vec3 Value(double u, double v, Vec3 p)
	{
		int xInteger = (int)Math.Floor(p.X * InvScale);
		int yInteger = (int)Math.Floor(p.Y * InvScale);
		int zInteger = (int)Math.Floor(p.Z * InvScale);

		bool isEven = (xInteger + yInteger + zInteger) % 2 == 0;
		
		return isEven ? Even.Value(u, v, p) : Odd.Value(u, v, p);
	}
}