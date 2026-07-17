// A class for storing and changing settings used by the renderer. NOTE: Currently the settings are run-time only and do not save.

namespace RTOneWeekend.Core;

public static class Settings
{
	public static bool AntiAliasing {get; set;} = true; // Currently only crude antialiasing that is not stable during real-time rendering, especially at a low resolution.
	public static bool RealTimeRender {get; set;} = false; // Whether the render will keep updating. If false then it does a single render.

	// Sizing for the SDL window; not the render target that is written to for raytracing.
	public static int WindowWidth { get; set; } = 1280;
	public static int WindowHeight { get; set; } = 720;

	// Currently selected scene.
	public static int NumScenes {get;} = 2;
	public static int SelectedScene {get; set;} = 1;
}