// A class for storing and changing settings used by the renderer. NOTE: Currently the settings are run-time only and do not save.

namespace RTOneWeekend.Core;

public static class Settings
{
	public static bool AntiAliasing {get; set;} = true; // Currently only crude antialiasing that is not stable during real-time rendering, especially at a low resolution.
	public static bool RealTimeRender {get; set;} = false; // Whether the render will keep updating. If false then it does a single render.
}