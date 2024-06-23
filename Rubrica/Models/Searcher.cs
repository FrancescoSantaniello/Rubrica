using System.Windows;
using System.Windows.Controls;
namespace Rubrica.Models;
public static class Searcher
{
	public static void Search(string key, ref DataGrid dataGrid, bool approfodinta = true)
	{
		dataGrid.SelectedCells.Clear();

		if (string.IsNullOrEmpty(key))
			return;

		foreach (Contatto item in dataGrid.Items)
		{
			DependencyObject row = dataGrid.ItemContainerGenerator.ContainerFromItem(item);
			
			if (row is not null)
			{
				foreach (var column in dataGrid.Columns)
				{
					FrameworkElement cellContent = column.GetCellContent(item);
					if (approfodinta)
					{
						if (cellContent is TextBlock textBlock && textBlock.Text.ToLower().Contains(key))
						{
							dataGrid.SelectedCells.Add(new DataGridCellInfo(item, column));
							dataGrid.ScrollIntoView(item, column);
						}
					}
					else
					{
						if (cellContent is TextBlock textBlock && textBlock.Text.ToLower() == key)
						{
							dataGrid.SelectedCells.Add(new DataGridCellInfo(item, column));
							dataGrid.ScrollIntoView(item, column);
						}
					}
				}
			}
		}
	}
}
