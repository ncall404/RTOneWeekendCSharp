// Static class to handle scene selection and loading.

namespace RTOneWeekend.Scenes;

public static class SceneManager
{
	public static int SceneCount { get; } = 7;
	public static int SelectedScene { get; set; } = 0;
	public static int LoadedScene { get; set; } = 0;

	public static Scene LoadScene(int id)
	{
		Scene activeScene = id switch
		{
			0 => new DefaultScene(),		// Empty
			1 => new Scene1(),				// Three material spheres
			2 => new Scene2(),				// Bouncing spheres
			3 => new Scene3(),				// Checkered spheres
			4 => new Scene4(),				// Earth
			5 => new Scene5(),				// Perlin spheres
			6 => new Scene6(),				// Quads
			7 => new Scene7(),				// Triangles
			_ => new DefaultScene(),		// Empty
		};
		LoadedScene = id;
		return activeScene;
	}
}