// Class for representing a generated noise texture. Currently supports Perlin noise.

using RTOneWeekend.Core;

namespace RTOneWeekend.Textures;

public class NoiseTexture : Texture
{
	private readonly Perlin _noise;

	public NoiseTexture()
	{
		_noise = new Perlin();
	}

	public override Vec3 Value(double u, double v, Vec3 p)
	{
		return new Vec3(1, 1, 1) * _noise.Noise(p);
	}
}