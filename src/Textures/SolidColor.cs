// SolidColor class for representing a texture that is just a solid color.

using System.Runtime.CompilerServices;
using RTOneWeekend.Core;

namespace RTOneWeekend.Textures;

public class SolidColor : Texture
{
	private Vec3 Albedo;
	public SolidColor(Vec3 albedo) => Albedo = albedo;
	public SolidColor(double red, double green, double blue) : this(new(red, green, blue)) {}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override Vec3 Value(double u, double v, Vec3 p) => Albedo;
}