// Class for representing a generated noise texture. Currently supports Perlin noise.

using RTOneWeekend.Core;

namespace RTOneWeekend.Textures;

public class MarbleNoiseTexture : Texture
{
	private readonly Perlin _noise;
	private readonly double _scale;
	private readonly int _turbulance;

	public MarbleNoiseTexture()
	{
		_noise = new Perlin();
		_scale = 1;
		_turbulance = 0;
	}
 
	public MarbleNoiseTexture(double scale = 1, int turbulance = 0)
	{
		_noise = new Perlin();
		_scale = scale;
		_turbulance = turbulance;
	}

	public override Vec3 Value(double u, double v, Vec3 p)
	{
		return new Vec3(0.5, 0.5, 0.5) * (1 + Math.Sin(_scale * p.Z + 10 * _noise.Turbulance(p, _turbulance)));
	}
}