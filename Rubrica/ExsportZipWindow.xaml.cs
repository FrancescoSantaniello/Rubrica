using Microsoft.Win32;
using Rubrica.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Rubrica;
public partial class ExportWindow : Window
{
    public ExportWindow()
    {
        InitializeComponent();
    }

	private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
	{
		new MainWindow().Show();
	}

	private void buttonScegliArchivioZip_Click(object sender, RoutedEventArgs e)
	{
		SaveFileDialog saveFileDialog = new()
		{
			Title = "Salva archivio ZIP",
			Filter = "Salva | *.zip"
		};

		if (saveFileDialog.ShowDialog() == true)
			textBoxArchivioZip.Text = saveFileDialog.FileName;
	}

	private void buttonEsportaArchivioZip_Click(object sender, RoutedEventArgs e)
	{
		if (string.IsNullOrEmpty(textBoxArchivioZip.Text))
			return;

		if (File.Exists(textBoxArchivioZip.Text))
			if (MessageBox.Show("L' archivio ZIP di destinazione esiste già, sovrascrivere?", Title, MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.No)
				return;

		try
		{
			Zip.Compression(Directory.GetFiles(MainWindow.PATH_RUBRICHE,"*.db"),textBoxArchivioZip.Text);
			MainWindow.ShowInfoMessage("Rubriche importante con successo",Title);
		}
		catch(ArgumentException ex)
		{
			MainWindow.ShowErrorMessage(ex.Message,Title);
		}
		catch(Exception ex)
		{
			MainWindow.ShowErrorMessage(ex.Message, Title);
		}
	}
}
