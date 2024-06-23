using System.IO;

namespace Rubrica.Models;
public static class CookieManagement
{
	public const string CookiePath = MainWindow.PATH_RUBRICHE + "/Cookie/" + "cookie.txt";
	public static string? UltimaRubrica
	{
		set
		{
			if (string.IsNullOrEmpty(value))
				return;

			string dirPath = CookiePath.Replace(Path.GetFileName(CookiePath), "");

			if (!Directory.Exists(dirPath))
				Directory.CreateDirectory(dirPath);

			File.WriteAllText(CookiePath, value);
		}
		get
		{
			if (!File.Exists(CookiePath))
			{
				File.CreateText(CookiePath);
				return null;
			}

			return File.ReadAllLines(CookiePath)[0].Trim();
		}
	}
	public static string? UltimaRubricaNome
	{
		get
		{
			return Path.GetFileNameWithoutExtension(UltimaRubrica);
		}
	}
	public static bool Dispose()
	{
		try
		{
			File.Delete(CookiePath);
		}
		catch (Exception)
		{
			return false;
		}

		return true;
	}
}
