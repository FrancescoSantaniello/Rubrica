using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Windows.Media;
using System.Windows.Input;
namespace Rubrica;

public partial class NuovaRubricaWindow : Window
{
	public NuovaRubricaWindow()
	{
		InitializeComponent();
	}

	private void textBoxNuovaRubrica_TextChanged(object sender, TextChangedEventArgs e)
	{
		textBoxNuovaRubrica.Foreground = File.Exists(Path.Combine(MainWindow.PATH_RUBRICHE, textBoxNuovaRubrica.Text) + ".db") ? Brushes.Red : Brushes.Green;
    }
	private void buttonCrea_Click(object sender, RoutedEventArgs e)
	{
		string db = Path.Combine(MainWindow.PATH_RUBRICHE, textBoxNuovaRubrica.Text) + ".db";
		
		if (File.Exists(db))
		{
			MainWindow.ShowErrorMessage($"Il database '{textBoxNuovaRubrica.Text}' esiste già", Title);
			return;
		}

		try
		{
			Cursor = Cursors.Wait;
			new ViewRubricaWindow(db).Show();
			Close();
		}
		catch(Exception ex)
		{
			MainWindow.ShowErrorMessage(ex.Message, Title);
		}

	}
}
