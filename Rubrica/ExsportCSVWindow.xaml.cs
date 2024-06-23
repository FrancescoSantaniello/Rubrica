using Microsoft.Win32;
using Rubrica.Models;
using System.IO;
using System.Windows;

namespace Rubrica;

public partial class ExsportCSVWindow : Window
{
    private string PathDb { get; init; }
	public ExsportCSVWindow(string pathDb)
    {
		if (string.IsNullOrEmpty(pathDb))
			MainWindow.ShowErrorMessage("Rubrica non valida",Title);

		PathDb = pathDb;

        InitializeComponent();
    }

	private void buttonEsportaCSV_Click(object sender, RoutedEventArgs e)
	{
		if (string.IsNullOrEmpty(textBoxFileCSV.Text))
			return;

		if (File.Exists(textBoxFileCSV.Text))
			if (MessageBox.Show("Il file esiste già, vuoi sovrascriverlo?", Title, MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.No)
				return;

		try
		{
			CSV.WriteToFile(RubricaDbContext.GetInstance(PathDb).Contatti.ToList(), textBoxFileCSV.Text);
			MainWindow.ShowInfoMessage("CSV creato con successo!",Title);
		}
		catch(ArgumentException ex)
		{
			MainWindow.ShowErrorMessage(ex.Message, Title);
		}
		catch(Exception ex)
		{
			MainWindow.ShowErrorMessage(ex.Message, Title);
		}
    }

	private void buttonScegliCSV_Click(object sender, RoutedEventArgs e)
	{
		SaveFileDialog saveFileDialog = new()
		{
			Title = "Salva archivio ZIP",
			Filter = "Salva | *.csv",
		};

		if (saveFileDialog.ShowDialog() == true)
			textBoxFileCSV.Text = saveFileDialog.FileName;
	}
}
