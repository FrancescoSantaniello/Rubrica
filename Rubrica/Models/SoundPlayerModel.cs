using System.Media;

namespace Rubrica.Models;
public static class SoundPlayerModel
{
	private static Lazy<SoundPlayer> Instance = new(() => new("./sounds/delete.wav"));
	public static void Play()
	{
		Instance.Value.Play();
	}
}
