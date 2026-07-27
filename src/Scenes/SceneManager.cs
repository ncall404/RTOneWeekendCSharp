// Static class to handle scene selection and loading.

namespace RTOneWeekend.Scenes;

public static class SceneManager
{
	public static int SceneCount { get; } = 5;
	public static int SelectedScene { get; set; } = 0;
	public static int LoadedScene { get; set; } = 0;

	public static Scene LoadScene(int id)
	{
		Scene activeScene;
		switch (id)
		{
			case 0:
				activeScene = new DefaultScene(); // Empty
				break;
			case 1:
				activeScene = new Scene1(); // Three material spheres
				break;
			case 2:
				activeScene = new Scene2(); // Bouncing spheres
				break;
			case 3:
				activeScene = new Scene3(); // Checkered spheres
				break;
			case 4:
				activeScene = new Scene4(); // Earth
				break;
			case 5:
				activeScene = new Scene5(); // Perlin spheres
				break;
			default:
				activeScene = new DefaultScene(); // Empty
				break;
		}

		LoadedScene = id;
		return activeScene;
	}
}