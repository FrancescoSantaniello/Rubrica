using System.IO;
using System.IO.Compression;
using System.Windows;

namespace Rubrica.Models;

public static class Zip
{
	public static void Decompression(string zipPath, string outputDirectory = MainWindow.PATH_RUBRICHE)
	{
		if (!File.Exists(zipPath))
			throw new ArgumentException("L' archivio ZIP non esiste");

		if (!Directory.Exists(outputDirectory))
			throw new ArgumentException("La cartella di destinazione non esiste");


		foreach (ZipArchiveEntry entry in ZipFile.OpenRead(zipPath).Entries.Where((c) => Path.GetExtension(c.Name) == ".db"))
		{
			string path = Path.Combine(outputDirectory, entry.Name);

			if (File.Exists(path))
			{
				if (MessageBox.Show("E' gia presente un altra rubrica con lo stesso nome\nSovrascrivere?", "Sovrascrivere", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
					entry.ExtractToFile(path, true);
			}
			else
				entry.ExtractToFile(path);
		}
	}
	public static void Compression(string[] dbPaths, string outZipFile)
	{
		using ZipArchive archive = new ZipArchive(new FileStream(outZipFile, FileMode.Create), ZipArchiveMode.Create);

		foreach (string file in dbPaths)
			if (File.Exists(file))
				archive.CreateEntryFromFile(file, Path.GetFileName(file));
	}
}
