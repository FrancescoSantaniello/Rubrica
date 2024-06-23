using Rubrica.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.IO;
namespace Rubrica;
public partial class ViewRubricaWindow : Window
{
	public string PathDatabase { get; init; }
	private void menuItemNuova_Click(object sender, RoutedEventArgs e)
	{
		new NuovaRubricaWindow().ShowDialog();
	}

	private void buttonCambiaRubrica_Click(object sender, RoutedEventArgs e)
	{
		new MainWindow().Show();
		buttonSalva_Click();
		Close();
	}

	private void buttonAumentaZoom_Click(object? sender = null, RoutedEventArgs? e = null)
	{
		try
		{
			dataGrid.FontSize += 2;
		}
		catch (Exception) { }
	}

	private void buttonDiminuisciZoom_Click(object? sender = null, RoutedEventArgs? e = null)
	{
		if (dataGrid.FontSize > 0)
			dataGrid.FontSize -= 2;
	}

	public void RefreshContatti()
	{
		Thread th = new(() =>
		{
			Dispatcher.Invoke(() => Cursor = Cursors.Wait);

			dataGrid.Dispatcher.Invoke(() =>
			{
				try
				{
					dataGrid.ItemsSource = RubricaDbContext.GetInstance(PathDatabase).Contatti.ToArray();
				}
				catch(Exception)
				{
					MainWindow.ShowErrorMessage("Errore, rubrica corrotta o illeggibile, scegliere un altra rubrica", "Errore");

					new MainWindow().Show();
					Close();
				}
			});
			
			Dispatcher.Invoke(() => Cursor = Cursors.Arrow);
		});
		th.Start();
	}

	private void buttonCancellaContatti_Click(object sender, RoutedEventArgs e)
	{
		if (MessageBox.Show($"Sei sicuro di voler cancellare tutti i contatti di '{Title}'?", Title, MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
			try
			{
				RubricaDbContext.GetInstance(PathDatabase).Contatti.RemoveRange(RubricaDbContext.GetInstance(PathDatabase).Contatti);
				RubricaDbContext.GetInstance(PathDatabase).SaveChanges();

				SoundPlayerModel.Play();

				RefreshContatti();
			}
			catch(Exception ex)
			{
				MainWindow.ShowErrorMessage(ex.Message, Title);
			}
	}

	private void DeleteContatto(Contatto contatto)
	{
		Cursor = Cursors.Wait;
		Thread th = new Thread(() =>
		{
			RubricaDbContext.GetInstance(PathDatabase).Contatti.Remove(contatto);
			RubricaDbContext.GetInstance(PathDatabase).SaveChanges();

			RefreshContatti();

			Dispatcher.Invoke(() => Cursor = Cursors.Arrow);

			SoundPlayerModel.Play();
		});
		th.Start();
	}

	private void DeleteContatti(List<Contatto> contatto)
	{
		if (contatto.Count <= 0)
			return;

		Cursor = Cursors.Wait;
		Thread th = new(() =>
		{
			RubricaDbContext.GetInstance(PathDatabase).Contatti.RemoveRange(contatto);
			RubricaDbContext.GetInstance(PathDatabase).SaveChanges();

			RefreshContatti();

			Dispatcher.Invoke(() => Cursor = Cursors.Arrow);

			SoundPlayerModel.Play();
		});
		th.Start();
	}

	private void buttonNuovoContatto_Click(object sender, RoutedEventArgs e)
	{
		new AddContattoWindow(this).ShowDialog();
		RefreshContatti();
	}

	private void buttonSalva_Click(object? sender = null, RoutedEventArgs? e  = null)
	{
		Cursor = Cursors.Wait;
		
		RubricaDbContext.GetInstance(PathDatabase).Contatti.UpdateRange(dataGrid.Items.OfType<Contatto>().ToList());
		RubricaDbContext.GetInstance(PathDatabase).SaveChanges();
		
		Cursor = Cursors.Arrow;
	}

	private void buttonCancella_Click(object sender, RoutedEventArgs e)
	{
		if (dataGrid.SelectionUnit != DataGridSelectionUnit.FullRow)
			dataGrid.SelectionUnit = DataGridSelectionUnit.FullRow;

		if (dataGrid.SelectedItem is not null && dataGrid.SelectedItem is Contatto)
			if (MessageBox.Show("Sei sicuro di voler cancellare il contatto selezionato?",Title,MessageBoxButton.YesNo,MessageBoxImage.Information) == MessageBoxResult.Yes)
				DeleteContatto((Contatto)dataGrid.SelectedItem);
	}

	private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
	{
		buttonSalva_Click();
	}
	private void buttonCancellazioneMultipla_Click(object sender, RoutedEventArgs e)
	{
		if (dataGrid.SelectionUnit != DataGridSelectionUnit.FullRow)
		{
			dataGrid.SelectionUnit = DataGridSelectionUnit.FullRow;
			return;
		}

		if (MessageBox.Show("Sei sicuro di voler cancellare i contatti selezionati?",Title,MessageBoxButton.YesNo,MessageBoxImage.Information) == MessageBoxResult.Yes)
			DeleteContatti(dataGrid.SelectedItems.OfType<Contatto>().ToList());
	}

	private void textBoxKey_TextChanged(object sender, TextChangedEventArgs e)
	{
		if (dataGrid.SelectionUnit != DataGridSelectionUnit.Cell)
		{
			dataGrid.SelectionUnit = DataGridSelectionUnit.Cell;
			textBoxKey_TextChanged(sender, e);
		}

		Searcher.Search(textBoxKey.Text.ToLower(), ref dataGrid, toggleButtonRicercaApprofondita.IsChecked == true);
	}

	private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
	{
		if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.OemPlus)
			buttonAumentaZoom_Click();
		else if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.OemMinus)
			buttonDiminuisciZoom_Click();
		else if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && (e.Key == Key.NumPad0) || (e.Key == Key.D0))
			dataGrid.FontSize = 14;
	}

	private void Window_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
	{
		if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
		{
			if (e.Delta >= 0)
				buttonAumentaZoom_Click();
			else
				buttonDiminuisciZoom_Click();
		}
	}

	private void dataGrid_DragEnter(object sender, DragEventArgs e)
	{
		if (e.Data.GetDataPresent(DataFormats.FileDrop))
			e.Effects = DragDropEffects.Copy;
		else
			e.Effects = DragDropEffects.None;
	}

	private void dataGrid_DragOver(object sender, DragEventArgs e)
	{
		if (e.Data.GetDataPresent(DataFormats.FileDrop))
			e.Effects = DragDropEffects.Copy;
		else
			e.Effects = DragDropEffects.None;
	}

	private void dataGrid_Drop(object sender, DragEventArgs e)
	{
		if (e.Data.GetDataPresent(DataFormats.FileDrop))
		{
			try
			{
				foreach(string file in (string[])e.Data.GetData(DataFormats.FileDrop))
				{
					if (File.Exists(file))
					{
						switch (Path.GetExtension(file))
						{
							case ".db":
								try
								{
									ImportWindow.ImportFromDb(file);
								}
								catch (ArgumentException error)
								{
									MainWindow.ShowErrorMessage(error.Message, Title);
								}
								catch (Exception error)
								{
									MainWindow.ShowErrorMessage(error.Message, Title);
									break;
								}

								new ViewRubricaWindow(Path.Combine(MainWindow.PATH_RUBRICHE, Path.GetFileName(file))).Show();
								Close();
								break;

							case ".csv":
								try
								{
									ImportWindow.ImportFromFileCSV(file);
								}
								catch (ArgumentException error)
								{
									MainWindow.ShowErrorMessage(error.Message, Title);
								}
								catch(Exception error)
								{
									MainWindow.ShowErrorMessage(error.Message, Title);
									break;
								}

								new ViewRubricaWindow(Path.Combine(MainWindow.PATH_RUBRICHE, Path.GetFileNameWithoutExtension(file)) + ".db").Show();
								Close();
								break;
						}
					}
				}
			}
			catch (Exception ex)
			{
				MainWindow.ShowErrorMessage(ex.Message,Title);
			}
		}
	}

	private void dataGrid_PreviewDragOver(object sender, DragEventArgs e)
	{
		e.Handled = true;
	}

	private void buttonEsportaCSV_Click(object sender, RoutedEventArgs e)
	{
		new ExsportCSVWindow(PathDatabase).ShowDialog();
	}
	public ViewRubricaWindow(string dbPath)
	{
		if (string.IsNullOrEmpty(dbPath))
			throw new ArgumentException("Percorso database non valido");

		InitializeComponent();

		PathDatabase = dbPath;

		Title = "Rubrica - " + Path.GetFileNameWithoutExtension(dbPath);

		Width = SystemParameters.PrimaryScreenWidth / 1.1;
		Height = SystemParameters.PrimaryScreenHeight / 1.1;

		RefreshContatti();
	}
}