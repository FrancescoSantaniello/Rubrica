using Rubrica.Models;
using System.Windows;
using System.Media;
namespace Rubrica;

public partial class AddContattoWindow : Window
{
	private ViewRubricaWindow _window { get; init; }
	public AddContattoWindow(ViewRubricaWindow _viewWindow)
	{
		InitializeComponent();
		_window = _viewWindow;
	}
	private void buttonAggiungi_Click(object sender, RoutedEventArgs e)
	{
		try
		{
			RubricaDbContext.GetInstance(_window.PathDatabase).Contatti.Add(new()
			{
				Nome = textBoxNome.Text,
				Cognome = textBoxCognome.Text,
				Telefono = textBoxTelefono.Text,
				UserId = textBoxUserID.Text,
				Altro = textBoxAltro.Text
			});

			RubricaDbContext.GetInstance(_window.PathDatabase).SaveChanges();

			_window.RefreshContatti();

			SystemSounds.Asterisk.Play();
		}
		catch(ArgumentException ex)
		{
			MainWindow.ShowErrorMessage(ex.Message, Title);
		}
	}
}
