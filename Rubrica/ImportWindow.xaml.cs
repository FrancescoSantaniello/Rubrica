using Microsoft.Win32;
using System.IO;
using System.Windows;
using Rubrica.Models;

namespace Rubrica;

public partial class ImportWindow : Window
{
	public static void ImportFromDb(string dbPath)
	{
		if (!File.Exists(dbPath))
			throw new ArgumentException("La rubrica non esiste");

		File.Copy(dbPath, Path.Combine(MainWindow.PATH_RUBRICHE, Path.GetFileName(dbPath)));
	}
	public static void ImportFromFileCSV(string csvFile)
	{
		string dbPath = Path.Combine(MainWindow.PATH_RUBRICHE, Path.GetFileNameWithoutExtension(csvFile)) + ".db";
		RubricaDbContext.GetInstance(dbPath).Contatti.AddRange(CSV.ReadFromFile(csvFile));
		RubricaDbContext.GetInstance(dbPath).SaveChanges();
	}
	private void buttonScegliRubrica_Click(object sender, RoutedEventArgs e)
	{
		OpenFileDialog openFileDialog = new()
		{
			Title = "Scegli rubrica",
			Filter = "Rubrica | *.db",
			Multiselect = false
		};

		if (openFileDialog.ShowDialog() == true)
			textBoxRubrica.Text = openFileDialog.FileName;
	}

	private void buttonImportaRubrica_Click(object sender, RoutedEventArgs e)
	{
		try
		{
			ImportFromDb(textBoxRubrica.Text);
			MainWindow.ShowInfoMessage("Rubrica importata!", Title);
		}
		catch (ArgumentException ex)
		{
			MainWindow.ShowErrorMessage(ex.Message, Title);
		}
		catch (Exception ex)
		{
			MainWindow.ShowErrorMessage(ex.Message, Title);
		}
	}

	private void buttonScegliFileCSV_Click(object sender, RoutedEventArgs e)
	{

		OpenFileDialog openFileDialog = new()
		{
			Title = "Scegli CSV",
			Filter = "Rubrica | *.csv",
			Multiselect = false
		};

		if (openFileDialog.ShowDialog() == true)
			textBoxFileCSV.Text = openFileDialog.FileName;
	}

	private void buttonScegliArchivio_Click(object sender, RoutedEventArgs e)
	{
		OpenFileDialog openFileDialog = new()
		{
			Title = "Scegli archivio ZIP",
			Filter = "Rubrica | *.zip",
			Multiselect = false
		};

		if (openFileDialog.ShowDialog() == true)
			textBoxArchivio.Text = openFileDialog.FileName;
	}

	private void buttonImportaArchivio_Click(object sender, RoutedEventArgs e)
	{
		try
		{
			Zip.Decompression(textBoxArchivio.Text);
			MainWindow.ShowInfoMessage("Archivio ZIP importato!", Title);
		}
		catch (ArgumentException ex)
		{
			MainWindow.ShowErrorMessage(ex.Message, Title);
		}
		catch (Exception ex)
		{
			MainWindow.ShowErrorMessage(ex.Message, Title);
		}
	}

	private void buttonImportaFileCSV_Click(object sender, RoutedEventArgs e)
	{
		try
		{
			ImportFromFileCSV(textBoxFileCSV.Text);
			MainWindow.ShowInfoMessage("File CSV importato!", Title);
		}
		catch (ArgumentException ex)
		{
			MainWindow.ShowErrorMessage(ex.Message, Title);
		}
		catch (Exception ex)
		{
			MainWindow.ShowErrorMessage(ex.Message, Title);
		}
	}
	private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
	{
		new MainWindow().Show();
	}
	public ImportWindow()
	{
		InitializeComponent();
	}
}
