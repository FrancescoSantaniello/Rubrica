using Rubrica.Models;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Rubrica;

public partial class MainWindow : Window
{
	public const string PATH_RUBRICHE = "./rubriche";
	private List<string> ElencoRubriche(bool onlyOpened = false)
	{
		if (!Directory.Exists(PATH_RUBRICHE))
			Directory.CreateDirectory(PATH_RUBRICHE);
		else
		{
			Thread th = new(() =>
			{
				foreach (string file in Directory.GetFiles(PATH_RUBRICHE, "*.*"))
				{
					if (Path.GetExtension(file) != ".db")
						TryDelete(file);
				}
			});
			th.Start();
		}

		if (!onlyOpened)
			return Directory.GetFiles(PATH_RUBRICHE, "*.db").Select(e => Path.GetFileNameWithoutExtension(e)).ToList();
		else
			return Directory.GetFiles(PATH_RUBRICHE, "*.db").Where(e => !IsOpened(e)).Select(e => Path.GetFileNameWithoutExtension(e)).ToList();
	}
	private bool IsOpened(string pathFile)
	{
		try
		{
			File.ReadAllText(pathFile);
		}
		catch (Exception)
		{
			return true;
		}

		return false;
	}
	public static bool TryDelete(string file)
	{
		try
		{
			File.Delete(file);
		}
		catch (Exception)
		{
			return false;
		}

		return true;
	}
	private void buttonCancella_Click(object sender, RoutedEventArgs e)
	{
		string rubrica = Path.Combine(PATH_RUBRICHE, comboBoxRubriche.Text) + ".db";

		if (!File.Exists(rubrica))
			return;

		if (MessageBox.Show($"Sei sicuro di voler cancella la rubrica '{comboBoxRubriche.Text}'", Title, MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
		{
			if (!TryDelete(rubrica))
			{
				ShowErrorMessage("Impossibile cancella la rubrica\nFile usato da un altro processo, riprovare piu tardi", Title);
				return;
			}

			SoundPlayerModel.Play();

			RefreshRubriche();
		}
	}
	
	public static void ShowErrorMessage(string msg, string title)
	{
		MessageBox.Show(msg, title, MessageBoxButton.OK, MessageBoxImage.Error);
	}
	
	public static void ShowInfoMessage(string msg, string title)
	{
		MessageBox.Show(msg, title, MessageBoxButton.OK, MessageBoxImage.Asterisk);
	}

	private void buttonApri_Click(object sender, RoutedEventArgs e)
	{
		if (string.IsNullOrEmpty(comboBoxRubriche.Text))
			return;

		if (!File.Exists($"{PATH_RUBRICHE}/{comboBoxRubriche.Text}.db"))
		{
			ElencoRubriche();
			return;
		}

		CookieManagement.UltimaRubrica = $"{PATH_RUBRICHE}/{comboBoxRubriche.Text}.db";

		new ViewRubricaWindow(new($"{PATH_RUBRICHE}/{comboBoxRubriche.Text}.db")).Show();
		Close();
	}

	private void buttonImporta_Click(object sender, RoutedEventArgs e)
	{
		new ImportWindow().Show();
		Close();
    }

	private void buttonEsporta_Click(object sender, RoutedEventArgs e)
	{
		new ExportWindow().Show();
		Close();
    }

	private void textBoxNuovaRubrica_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
	{
		textBoxNuovaRubrica.Foreground = File.Exists(Path.Combine(PATH_RUBRICHE, textBoxNuovaRubrica.Text) + ".db") ? Brushes.Red : Brushes.Green;
	}

	private void buttonCrea_Click(object sender, RoutedEventArgs e)
	{
		string db = Path.Combine(PATH_RUBRICHE, textBoxNuovaRubrica.Text) + ".db";

		if (File.Exists(db))
		{
			ShowErrorMessage($"Il database '{textBoxNuovaRubrica.Text}' esiste già", Title);
			return;
		}

		try
		{
			Cursor = Cursors.Wait;
			new ViewRubricaWindow(db).Show();
			Close();
		}
		catch (Exception ex)
		{
			ShowErrorMessage(ex.Message, Title);
		}

	}

	private void buttonSalvaModifica_Click(object sender, RoutedEventArgs e)
	{
		if (string.IsNullOrEmpty(comboBoxRubricheModifica.Text) || string.IsNullOrEmpty(textBoxNuovoNome.Text))
			return;

		string path = Path.Combine(PATH_RUBRICHE, comboBoxRubricheModifica.Text) + ".db";

		try
		{
			if (IsOpened(path))
				throw new Exception();

			File.Copy(path, path.Replace(comboBoxRubricheModifica.Text, textBoxNuovoNome.Text));
			File.Delete(path);

			ShowInfoMessage("Rubrica rinominata con successo", Title);

			RefreshRubriche();
		}
		catch (Exception)
		{
			ShowErrorMessage("Impossibile cancella la rubrica\nFile usato da un altro processo, riprovare piu tardi", Title);
		}
	}
	
	private void RefreshRubriche()
	{
		comboBoxRubriche.ItemsSource = ElencoRubriche();
		comboBoxRubricheModifica.ItemsSource = ElencoRubriche(true);

		if (comboBoxRubricheModifica.Items.IsEmpty)
			tabItemModifica.IsEnabled = false;

		comboBoxRubricheModifica.SelectedIndex = 0;

		try
		{
			comboBoxRubriche.SelectedItem = CookieManagement.UltimaRubricaNome;
		}
		catch (Exception)
		{
			CookieManagement.Dispose();
		}

		if (comboBoxRubriche.SelectedItem is null)
			comboBoxRubriche.SelectedIndex = 0;

		textBoxNuovoNome.Clear();
	}

	public MainWindow()
	{
		InitializeComponent();
		RefreshRubriche();
	}
}