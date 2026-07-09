// Camera struct representing the camera and viewport for the renderer.

using SDL3;
using RTOneWeekend.Geometry;

namespace RTOneWeekend.Core;

public class Camera
{
	public double AspectRatio = 16.0 / 9.0; // Width over height ratio.
	public int Width = 400; // Rendered image width.
	public int Height {get; private set;} // Rendered image height.
	private Vec3 CameraCenter;
	private Vec3 Pixel00Location; // Location of the lower left pixel.
	private Vec3 PixelDeltaU; // Offset to the pixel to the right.
	private Vec3 PixelDeltaV; // Offset to the pixel below.

	public byte[] Render(HittableList world)
	{
		Initialize();

		byte[] pixelBuffer = new byte[Width * Height * 4];

		// Draw to each pixel. TODO: Switch outside loop to "Parallel.For" later in the tutorials for better performance.
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                int offset = (y * Width + x) * 4;

                Vec3 pixelCenter = Pixel00Location + (x * PixelDeltaU) + (y * PixelDeltaV);
                Vec3 rayDirection = pixelCenter - CameraCenter;
                Ray r = new(CameraCenter, rayDirection);
                Vec3 rayColor = RayColor(r, world);

                // Pack color into 32 bit uint
                uint pixelColor = Vec3.WriteColor(rayColor, (byte)SDL.AlphaOpaque);
                BitConverter.TryWriteBytes(pixelBuffer.AsSpan(offset, 4), pixelColor);
            }
        }

        return pixelBuffer;
	}

	private void Initialize()
	{
		Height = (int)(Width / AspectRatio);
        Height = (Height < 1) ? 1 : Height; // Make sure that image height is at least 1.

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

	private static Vec3 RayColor(Ray r, Hittable world)
    {
        HitRecord rec = default;
		if (world.Hit(r, new Interval(0, double.PositiveInfinity), ref rec))
		{
			return 0.5 * (rec.Normal + new Vec3(1, 1, 1));
		}


        Vec3 unitDirection = Vec3.UnitVector(r.Direction);
        double a = 0.5 * (unitDirection.Y + 1.0);
        return (1.0 - a) * new Vec3(1.0, 1.0, 1.0) + a * new Vec3(0.5, 0.7, 1.0); // Lerp between light blue and white based on ray Y position.
    }
}