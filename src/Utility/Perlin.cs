// Class to create perlin noise for a texture.

using RTOneWeekend.Core;

namespace RTOneWeekend.Utility;

public class Perlin
{
	private const int PointCount = 256;
	private double[] _randfloat = new double[PointCount];
	private int[] _permX = new int[PointCount];
	private int[] _permY = new int[PointCount];
	private int[] _permZ = new int[PointCount];

	public Perlin()
	{
		for (int i = 0; i < PointCount; i++)
		{
			_randfloat[i] = RandomNum.RandomDouble();
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

		double[,,] c = new double[2, 2, 2];

		for (int di = 0; di < 2; di++)
		{
			for (int dj = 0; dj < 2; dj++)
			{
				for (int dk = 0; dk < 2; dk++)
				{
					c[di, dj, dk] = _randfloat[
						_permX[(i + di) & 255] ^
						_permY[(j + dj) & 255] ^
						_permZ[(k + dk) & 255]
					];
				}
			}
		}

		return TrilinearInterp(c, u, v, w);
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

	private static double TrilinearInterp(double[,,] c, double u, double v, double w)
	{
		double accumulation = 0.0;

		for (int i = 0; i < 2; i++)
		{
			for (int j = 0; j < 2; j++)
			{
				for (int k = 0; k < 2; k++)
				{
					accumulation += (i*u + (1-i)*(1-u)) *
									(j*v + (1-j)*(1-v)) *
									(k*w + (1-k)*(1-w)) *
									c[i, j, k];
				}
			}
		}

		return accumulation;
	}
}