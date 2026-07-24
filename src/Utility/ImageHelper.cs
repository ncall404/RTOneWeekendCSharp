// Image helper class for loading an image using SDL3_image and converting it to a flat buffer for use in the ray tracer.
// The class is named rtw_image in the tutorial. This ended up being quite different due to the use of SDL3_image instead of stb_image.

using SDL3;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RTOneWeekend.Utility;

public class ImageHelper
{
	public int ImageWidth { get; protected set; } = 0;
	public int ImageHeight { get; protected set; } = 0;
	public byte[]? ImageBuffer { get; protected set; }
	private const int BYTES_PER_PIXEL = 4;
	private int BytesPerScanline = 4;
	private ImmutableArray<byte> MAGENTA = [255, 0, 255, 255];

	public ImageHelper() {}

	public ImageHelper(string imagePath)
	{
		if (!File.Exists(imagePath))
		{
			Console.WriteLine($"Image file not found at this path: {imagePath}");
			SetToMagenta();
			return;
		}
		if (!LoadImage(imagePath))
		{
			SetToMagenta();
			return;
		}
	}

	public bool LoadImage(string imagePath)
	{
		nint imageSurface = SDL.LoadSurface(imagePath);
		
		if (imageSurface == 0)
		{
			Console.WriteLine($"Image file failed to load: {SDL.GetError()}");
			return false;
		}

		// NOTE: Currently the Alpha channel is ignored when sampling the image. The reason it is still using this RGBA8888 instead of RGB24 is for speed despite the slight increase in memory usage; as well as ease of implementation of transparent textures later if I choose.
		nint convertedSurface = SDL.ConvertSurfaceAndColorspace(
			imageSurface,
			SDL.PixelFormat.RGBA8888,
			SDL.GetSurfacePalette(imageSurface),
			SDL.Colorspace.SRGBLinear,
			0
		);
		SDL.DestroySurface(imageSurface); // Free original surface.

		if (convertedSurface == 0)
		{
			Console.WriteLine($"Image file failed to convert: {SDL.GetError()}");
			return false;
		}

		SDL.Surface surface = Marshal.PtrToStructure<SDL.Surface>(convertedSurface); // Get the structure of the surface to be able to access data such as width and height.

		ImageWidth = surface.Width;
		ImageHeight = surface.Height;
		BytesPerScanline = surface.Pitch;
		int totalBytes = BytesPerScanline * ImageHeight;
		ImageBuffer = new byte[totalBytes];

		Marshal.Copy(surface.Pixels, ImageBuffer, 0, totalBytes); // Copy pixel data from surface to buffer.
		SDL.DestroySurface(convertedSurface); // Free converted surface.

		return true;
	}

	// Get the data for a pixel at a specific location on the image.
	public ReadOnlySpan<byte> PixelData(int x, int y)
	{
		if (ImageBuffer == null)
			SetToMagenta();

		x = Math.Clamp(x, 0, ImageWidth - 1);
		y = Math.Clamp(y, 0, ImageHeight - 1);
		int offset = (x * BYTES_PER_PIXEL) + (y * BytesPerScanline);
		return ImageBuffer.AsSpan(offset, BYTES_PER_PIXEL);
	}

	// Sets the image to a magenta fallback for debugging.
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void SetToMagenta()
	{
		ImageWidth = 1;
		ImageHeight = 1;
		BytesPerScanline = 4;
		ImageBuffer = [.. MAGENTA];
	}
}