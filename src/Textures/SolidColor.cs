// SolidColor class for representing a texture that is just a solid color.

using System.Runtime.CompilerServices;
using RTOneWeekend.Core;

namespace RTOneWeekend.Textures;

public class SolidColor : Texture
{
	private readonly Vec3 _albedo;
	public SolidColor(Vec3 albedo) => _albedo = albedo;
	public SolidColor(double red, double green, double blue) : this(new(red, green, blue)) {}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override Vec3 Value(double u, double v, Vec3 p) => _albedo;
}