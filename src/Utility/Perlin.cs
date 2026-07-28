// Class to create perlin noise for a texture.

using RTOneWeekend.Core;

namespace RTOneWeekend.Utility;

public class Perlin
{
	private const int PointCount = 256;
	private readonly Vec3[] _randvec = new Vec3[PointCount];
	private readonly int[] _permX = new int[PointCount];
	private readonly int[] _permY = new int[PointCount];
	private readonly int[] _permZ = new int[PointCount];

	public Perlin()
	{
		for (int i = 0; i < PointCount; i++)
		{
			_randvec[i] = Vec3.UnitVector(Vec3.Random(-1, 1));
		}

		PerlinGeneratePerm(_permX);
		PerlinGeneratePerm(_permY);
		PerlinGeneratePerm(_permZ);
	}

	public double Noise(Vec3 p)
	{
		double u = p.X - Math.Floor(p.X);
		double v = p.Y - Math.Floor(p.Y);
		double w = p.Z - Math.Floor(p.Z);

		int i = (int)Math.Floor(p.X);
		int j = (int)Math.Floor(p.Y);
		int k = (int)Math.Floor(p.Z);

		Vec3[,,] c = new Vec3[2, 2, 2];

		for (int di = 0; di < 2; di++)
		{
			for (int dj = 0; dj < 2; dj++)
			{
				for (int dk = 0; dk < 2; dk++)
				{
					c[di, dj, dk] = _randvec[
						_permX[(i + di) & 255] ^
						_permY[(j + dj) & 255] ^
						_permZ[(k + dk) & 255]
					];
				}
			}
		}

		return PerlinInterp(c, u, v, w);
	}

	public double Turbulance(Vec3 p, int depth)
	{
		double accumulation = 0.0;
		double weight = 1.0;
		Vec3 tempP = p;

		for (int i = 0; i < depth; i++)
		{
			accumulation += weight * Noise(tempP);
			weight *= 0.5;
			tempP *= 2;
		}

		return Math.Abs(accumulation);
	}

	private static void PerlinGeneratePerm(Span<int> p)
	{
		for (int i = 0; i < PointCount; i++)
			p[i] = i;

		Permute(p, PointCount);
	}

	private static void Permute(Span<int> p, int n)
	{
		for (int i = n-1; i > 0; i--)
		{
			int target = RandomNum.RandomInt(0, i);
			(p[target], p[i]) = (p[i], p[target]);
		}
	}

	private static double PerlinInterp(Vec3[,,] c, double u, double v, double w)
	{
		// Hermitian smoothing
		double uu = u*u*(3 - 2*u);
		double vv = v*v*(3 - 2*v);
		double ww = w*w*(3 - 2*w);

		double accumulation = 0.0;

		for (int i = 0; i < 2; i++)
		{
			for (int j = 0; j < 2; j++)
			{
				for (int k = 0; k < 2; k++)
				{
					Vec3 weightValue = new(u-i, v-j, w-k);
					accumulation += (i*uu + (1-i)*(1-uu)) *
									(j*vv + (1-j)*(1-vv)) *
									(k*ww + (1-k)*(1-ww)) *
									Vec3.Dot(c[i, j, k], weightValue);
				}
			}
		}

		return accumulation;
	}
}