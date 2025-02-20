using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using DreamScape.Data;

namespace DreamScape.ContentDialogs
{
	public sealed partial class ItemDetailsDialog : ContentDialog
	{
		public Item SelectedItem { get; }

		public ItemDetailsDialog(Item selectedItem, XamlRoot xamlRoot)
		{
			this.InitializeComponent();
			this.SelectedItem = selectedItem;
			this.XamlRoot = xamlRoot;
			this.DataContext = this; // ✅ Explicitly set DataContext
		}
	}
}
