// Camera struct representing the camera and viewport for the renderer.

using SDL3;
using RTOneWeekend.Geometry;

namespace RTOneWeekend.Core;

public class Camera
{
	public double AspectRatio = 16.0 / 9.0; // Width over height ratio.
	public int Width = 400; // Rendered image width.
	public int Height {get; private set;} // Rendered image height.
	public int SamplesPerPixel = 10; // Number of samples per pixel for anti-aliasing.
	public int MaxDepth = 10; // Maximum number of ray bounces into a scene.

	private double PixelSamplesScale; // Color scale factor for a sum of pixel samples.
	private Vec3 CameraCenter;
	private Vec3 Pixel00Location; // Location of the lower left pixel.
	private Vec3 PixelDeltaU; // Offset to the pixel to the right.
	private Vec3 PixelDeltaV; // Offset to the pixel below.

	public byte[] Render(HittableList world)
	{
		Initialize();

		byte[] pixelBuffer = new byte[Width * Height * 4];

		// Draw to each pixel.
		Parallel.For(0, Height, y =>
        {
            for (int x = 0; x < Width; x++)
            {
                int offset = (y * Width + x) * 4;

				if (Settings.AntiAliasing)
				{
					Vec3 rayColor = new(0, 0, 0);
					for (int sample = 0; sample < SamplesPerPixel; sample++)
					{
						Ray r = GetRay(x, y);
						rayColor += RayColor(r, MaxDepth, world);
					}

					// Pack color into 32 bit uint
					uint pixelColor = Vec3.WriteColor(rayColor * PixelSamplesScale, (byte)SDL.AlphaOpaque);
					BitConverter.TryWriteBytes(pixelBuffer.AsSpan(offset, 4), pixelColor);
				} else
				{
					Vec3 pixelCenter = Pixel00Location + (x * PixelDeltaU) + (y * PixelDeltaV);
					Vec3 rayDirection = pixelCenter - CameraCenter;
					Ray r = new(CameraCenter, rayDirection);
					Vec3 rayColor = RayColor(r, MaxDepth, world);

					// Pack color into 32 bit uint
					uint pixelColor = Vec3.WriteColor(rayColor, (byte)SDL.AlphaOpaque);
					BitConverter.TryWriteBytes(pixelBuffer.AsSpan(offset, 4), pixelColor);
				}
                
            }
        });

        return pixelBuffer;
	}

	private void Initialize()
	{
		Height = (int)(Width / AspectRatio);
        Height = (Height < 1) ? 1 : Height; // Make sure that image height is at least 1.

		PixelSamplesScale = 1.0 / SamplesPerPixel;

		CameraCenter = new Vec3(0, 0, 0);

		// Determine viewport dimensions.
		double focalLength = 1.0;
        double viewportHeight = 2.0;
        double viewportWidth = viewportHeight * (Width / (double)Height);

		// Calculate the vectors across the horizontal and down the vertical viewport edges.
        Vec3 viewportU = new(viewportWidth, 0, 0);
        Vec3 viewportV = new(0, -viewportHeight, 0);

		// Calculate the horizontal and vertical delta vectors from pixel to pixel.
        PixelDeltaU = viewportU / Width;
        PixelDeltaV = viewportV / Height;

		// Calculate the location of the upper left pixel of the viewport.
        Vec3 viewportUpperLeft = CameraCenter - new Vec3(0, 0, focalLength) - viewportU / 2 - viewportV / 2;
		Pixel00Location = viewportUpperLeft + 0.5 * (PixelDeltaU + PixelDeltaV);
	}

	private Ray GetRay(int x, int y)
	{
		// Construct a camera ray originating from the origin and directed at a randomly sampled point around the pixel location i, j.
		Vec3 offset = SampleSquare();
		Vec3 pixelSample = Pixel00Location + ((x + offset.X) * PixelDeltaU) + ((y + offset.Y) * PixelDeltaV);

		Vec3 rayOrigin = CameraCenter;
		Vec3 rayDirection = pixelSample - rayOrigin;

		return new Ray(rayOrigin, rayDirection);
	}

	private Vec3 SampleSquare() {
		// Returns the vector to a random point in the [-.5, -.5] - [+.5, +.5] unit square.
		return new Vec3(RandomNum.RandomDouble() - 0.5, RandomNum.RandomDouble() - 0.5, 0);
	}

	private static Vec3 RayColor(Ray r, int depth, Hittable world)
    {
		// If ray bounce limit is exceeded, no more light is gathered.
		if (depth <= 0)
			return new Vec3(0, 0, 0);

        HitRecord rec = default;
		// 0.001 instead of just 0 takes care of shadow acne.
		if (world.Hit(r, new Interval(0.001, double.PositiveInfinity), ref rec))
		{
			// return 0.5 * (rec.Normal + new Vec3(1, 1, 1)); // Returns color based on normal direction.

			Vec3 direction = Vec3.RandomOnHemisphere(rec.Normal);
			return 0.5 * RayColor(new Ray(rec.P, direction), depth-1, world);
		}


        Vec3 unitDirection = Vec3.UnitVector(r.Direction);
        double a = 0.5 * (unitDirection.Y + 1.0);
        return (1.0 - a) * new Vec3(1.0, 1.0, 1.0) + a * new Vec3(0.5, 0.7, 1.0); // Lerp between light blue and white based on ray Y position.
    }
}