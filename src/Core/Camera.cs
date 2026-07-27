// Camera struct representing the camera and viewport for the renderer.

using SDL3;
using RTOneWeekend.Geometry;

namespace RTOneWeekend.Core;

public class Camera
{
	public double AspectRatio { get; set; } = 16.0 / 9.0; // Width over height ratio.
	public int Width { get; set; } = 400; // Rendered image width.
	public int Height { get; private set; } // Rendered image height.
	public int SamplesPerPixel { get; set; } = 100; // Number of samples per pixel for anti-aliasing. Could also probably be called rays-per-pixel.
	public int MaxDepth { get; set; } = 100; // Maximum number of ray bounces into a scene.

	public double VerticalFOV { get; set; } = 90; // Vertical field of view in degrees.
	public Vec3 CameraPosition { get; set; } = new(0, 0, 0); // Named lookfrom in the tutorial
	public Vec3 LookAt { get; set; } = new(0, 0, -1); // Point the camera is looking at. TODO: Replace later with ViewDirection.
	public Vec3 Up { get; set; } = new(0, 1, 0); // Camera-relatvie up direction.

	public double DefocusAngle { get; set; } = 0; // Variation angle of rays through each pixel. 0 = no depth of field.
	public double FocusDistance { get; set; } = 10; // Distance from the camera to the plane of perfect focus.

	private double _pixelSamplesScale; // Color scale factor for a sum of pixel samples.
	private Vec3 _pixel00Location; // Location of the lower left pixel.
	private Vec3 _pixelDeltaU; // Offset to the pixel to the right.
	private Vec3 _pixelDeltaV; // Offset to the pixel below.
	private Vec3 _u, _v, _w; // Camera frame basis vectors. (u = pointing right, camera right, v = pointing camera up, w = pointing opposite the view direction)
	private Vec3 _defocusDiskU; // Defocus disk horizontal radius.
	private Vec3 _defocusDiskV; // Defocus disk vertical radius.

	public Camera(double aspectRatio, int width, int samplesPerPixel, int maxDepth, double verticalFOV, Vec3 cameraPosition, Vec3 lookAt, Vec3 up, double defocusAngle = 0, double focusDistance = 10)
	{
		AspectRatio = aspectRatio;
		Width = width;
		SamplesPerPixel = samplesPerPixel;
		MaxDepth = maxDepth;
		VerticalFOV = verticalFOV;
		CameraPosition = cameraPosition;
		LookAt = lookAt;
		Up = up;
		DefocusAngle = defocusAngle;
		FocusDistance = focusDistance;

		CalculateViewport(); // Calculate the viewport based on the camera settings.
	}

	public byte[] Render(HittableList world)
	{
		CalculateViewport();

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
					uint pixelColor = Vec3.WriteColor(rayColor * _pixelSamplesScale, (byte)SDL.AlphaOpaque);
					BitConverter.TryWriteBytes(pixelBuffer.AsSpan(offset, 4), pixelColor);
				} else
				{
					Vec3 pixelCenter = _pixel00Location + (x * _pixelDeltaU) + (y * _pixelDeltaV);
					Vec3 rayDirection = pixelCenter - CameraPosition;
					Ray r = new(CameraPosition, rayDirection);
					Vec3 rayColor = RayColor(r, MaxDepth, world);

					// Pack color into 32 bit uint
					uint pixelColor = Vec3.WriteColor(rayColor, (byte)SDL.AlphaOpaque);
					BitConverter.TryWriteBytes(pixelBuffer.AsSpan(offset, 4), pixelColor);
				}
                
            }
        });

        return pixelBuffer;
	}

	// Calculates the viewport based on camera settings.
	public void CalculateViewport()
	{
		Height = (int)(Width / AspectRatio);
        Height = (Height < 1) ? 1 : Height; // Make sure that image height is at least 1.

		_pixelSamplesScale = 1.0 / SamplesPerPixel;

		// Determine viewport dimensions.
		// double focalLength = (CameraPosition - LookAt).Length();
		double theta = ConvertUnit.DegreesToRadians(VerticalFOV);
		double h = Math.Tan(theta/2);
        double viewportHeight = 2.0 * h * FocusDistance;
        double viewportWidth = viewportHeight * (Width / (double)Height);

		// Calculate the u, v, w unit basis vectors for the camera coordinate frame.
		_w = Vec3.UnitVector(CameraPosition - LookAt);
		_u = Vec3.UnitVector(Vec3.Cross(Up, _w));
		_v = Vec3.Cross(_w, _u);

		// Calculate the vectors across the horizontal and down the vertical viewport edges.
        Vec3 viewportU = viewportWidth * _u; // Vector across the viewport horizontal edge.
        Vec3 viewportV = viewportHeight * -_v; // Vector across the viewport vertical edge.

		// Calculate the horizontal and vertical delta vectors from pixel to pixel.
        _pixelDeltaU = viewportU / Width;
        _pixelDeltaV = viewportV / Height;

		// Calculate the location of the upper left pixel of the viewport.
        Vec3 viewportUpperLeft = CameraPosition - (FocusDistance * _w) - viewportU/2 - viewportV/2;
		_pixel00Location = viewportUpperLeft + 0.5 * (_pixelDeltaU + _pixelDeltaV);

		// Calculate the camera defocus (DOF) disk basis vectors.
		double defocusRadius = FocusDistance * Math.Tan(ConvertUnit.DegreesToRadians(DefocusAngle / 2));
		_defocusDiskU = _u * defocusRadius;
		_defocusDiskV = _v * defocusRadius;
	}

	private Ray GetRay(int x, int y)
	{
		// Construct a camera ray originating from the origin and directed at a randomly sampled point around the pixel location i, j.
		Vec3 offset = SampleSquare();
		Vec3 pixelSample = _pixel00Location + ((x + offset.X) * _pixelDeltaU) + ((y + offset.Y) * _pixelDeltaV);

		Vec3 rayOrigin = (DefocusAngle <= 0) ? CameraPosition : DefocusDiskSample();
		Vec3 rayDirection = pixelSample - rayOrigin;

		return new Ray(rayOrigin, rayDirection);
	}

	private Vec3 SampleSquare() {
		// Returns the vector to a random point in the [-.5, -.5] - [+.5, +.5] unit square.
		return new Vec3(RandomNum.RandomDouble() - 0.5, RandomNum.RandomDouble() - 0.5, 0);
	}

	private Vec3 DefocusDiskSample()
	{
		// Returns a random point in the camera defocus disk for DOF.
		Vec3 p = Vec3.RandomInUnitDisk();
		return CameraPosition + (p.X * _defocusDiskU) + (p.Y * _defocusDiskV);
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
			Ray scattered;
			Vec3 attenuation;

			if (rec.Material.Scatter(r, rec, out attenuation, out scattered))
				return attenuation * RayColor(scattered, depth - 1, world);

			return new Vec3(0, 0, 0);
		}


        Vec3 unitDirection = Vec3.UnitVector(r.Direction);
        double a = 0.5 * (unitDirection.Y + 1.0);
        return (1.0 - a) * new Vec3(1.0, 1.0, 1.0) + a * new Vec3(0.5, 0.7, 1.0); // Lerp between light blue and white based on ray Y position.
    }
}