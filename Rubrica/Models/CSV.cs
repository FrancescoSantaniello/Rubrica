using System.IO;
namespace Rubrica.Models;
public static class CSV
{
	public static List<Contatto> ReadFromFile(string filePath) 
	{
		if (!File.Exists(filePath))
			throw new ArgumentException("Il file CSV non esiste");

		if (Path.GetExtension(filePath) != ".csv")
			throw new ArgumentException("Il file non è un file CSV");

		List<Contatto> contatti = new();

		foreach (string line in File.ReadAllLines(filePath))
		{
			if (!string.IsNullOrEmpty(line))
			{
				string[] splited = line.Split(';');

				try
				{
					if (splited.Length == 6)
					{
						contatti.Add(new()
						{
							Id = int.Parse(splited[0].Trim()),
							UserId = splited[1].Trim(),
							Nome = splited[2].Trim(),
							Cognome = splited[3].Trim(),
							Telefono = splited[4].Trim(),
							Altro = splited[5].Trim(),
						});
					}
					else
					{
						contatti.Add(new()
						{
							UserId = splited[0].Trim(),
							Nome = splited[1].Trim(),
							Cognome = splited[2].Trim(),
							Telefono = splited[3].Trim(),
							Altro = splited[4].Trim(),
						});
					}
				}
				catch (Exception){ }
			}
		}

		return contatti;
	}
	
	public static void WriteToFile(List<Contatto> contatti, string outputFile)
	{
		File.WriteAllLines(outputFile, contatti.Select(e => e.ToString()));
	}
}
