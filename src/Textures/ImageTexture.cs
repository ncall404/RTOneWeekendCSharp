// CheckerTexture class for representing a texture that uses colors from a loaded image.

using System.Runtime.CompilerServices;
using RTOneWeekend.Core;

namespace RTOneWeekend.Textures;

public class ImageTexture : Texture
{
	private ImageHelper Image;

	public ImageTexture(string imagePath)
	{
		Image = new(imagePath);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override Vec3 Value(double u, double v, Vec3 p)
	{
		// If there is no texture data, return a solid cyan color as a debugging aid.
		if (Image.ImageHeight <= 0) return new(0.0, 1.0, 1.0);

		// Clamp input texture coordinates to [0, 1] x [1, 0]
		u = new Interval(0, 1).Clamp(u);
		v = 1.0 - new Interval(0, 1).Clamp(v);

		int i = (int)(u * Image.ImageWidth);
		int j = (int)(v * Image.ImageHeight);
		byte[] pixel = Image.PixelData(i, j).ToArray();

		double colorScale = 1.0 / 255.0;
		return new(colorScale * pixel[3], colorScale * pixel[2], colorScale * pixel[1]); // It seems that in the surface conversion the color channels switch form RGBA to ABGR.
	}
}