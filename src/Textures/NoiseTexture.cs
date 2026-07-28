// Class for representing a generated noise texture. Currently supports Perlin noise.

using RTOneWeekend.Core;

namespace RTOneWeekend.Textures;

public class NoiseTexture : Texture
{
	private readonly Perlin _noise;
	private readonly double _scale;
	private readonly int _turbulance;
 
	public NoiseTexture(double scale = 1, int turbulance = 0)
	{
		_noise = new Perlin();
		_scale = scale;
		_turbulance = turbulance;
	}

	public override Vec3 Value(double u, double v, Vec3 p)
	{
		if (_turbulance > 0)
			return new Vec3(1, 1, 1) * _noise.Turbulance(p, _turbulance);

		return new Vec3(1, 1, 1) * 0.5 * (1 + _noise.Noise(p * _scale)); // Maps values to a range of 0 to 1 to keep from negative numbers being passed to LinearToGamma()
	}
}