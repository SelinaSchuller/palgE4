using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using DreamScape.Data;
using Microsoft.EntityFrameworkCore;
using System.Data;
using DreamScape.ContentDialogs;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DreamScape.Pages.Player
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class PlayerInventoryPage : Page
	{
		private int _loggedInUserId { get; set; }
		private List<InventoryItem> allItemsOriginal = new List<InventoryItem>();
		private List<InventoryItem> allItems = new List<InventoryItem>();
		private List<string> allStatistics = new List<string> { "Strength", "Speed", "Durability" };
		public PlayerInventoryPage()
		{
			this.InitializeComponent();

			if(Data.User.LoggedInUser != null)
			{
				_loggedInUserId = Data.User.LoggedInUser.Id;
			}
			LoadItems();
		}

		private void LoadItems()
		{
			using(var db = new AppDbContext())
			{
				allItemsOriginal = db.InventoryItems
					.Where(ii => ii.UserId == _loggedInUserId)
					.Include(ii => ii.Item)
						.ThenInclude(i => i.Type)
					.Include(ii => ii.Item)
						.ThenInclude(i => i.Statistics)
						.ThenInclude(s => s.Rarity)
					.ToList();
			}

			allItems = new List<InventoryItem>(allItemsOriginal);
			itemListview.ItemsSource = allItems;

			TypeFilterComboBox.Items.Add(new ComboBoxItem { Content = "Alle Types", Tag = "All" });
			TypeFilterComboBox.Items.Add(new ComboBoxItem { Content = "Wapen ⚔️", Tag = "Wapen" });
			TypeFilterComboBox.Items.Add(new ComboBoxItem { Content = "Accessoire 💎", Tag = "Accessoire" });
			TypeFilterComboBox.Items.Add(new ComboBoxItem { Content = "Armor 🛡️", Tag = "Armor" });

			SortFilterComboBox.Items.Add(new ComboBoxItem { Content = "Alle Statistieken", Tag = "All" });
			SortFilterComboBox.Items.Add(new ComboBoxItem { Content = "Sterkte (Hoog-Laag)", Tag = "StrengthHigh" });
			SortFilterComboBox.Items.Add(new ComboBoxItem { Content = "Sterkte (Laag-Hoog)", Tag = "StrengthLow" });
			SortFilterComboBox.Items.Add(new ComboBoxItem { Content = "Snelheid (Hoog-Laag)", Tag = "SpeedHigh" });
			SortFilterComboBox.Items.Add(new ComboBoxItem { Content = "Snelheid (Laag-Hoog)", Tag = "SpeedLow" });
			SortFilterComboBox.Items.Add(new ComboBoxItem { Content = "Duurzaamheid (Hoog-Laag)", Tag = "DurabilityHigh" });
			SortFilterComboBox.Items.Add(new ComboBoxItem { Content = "Duurzaamheid (Laag-Hoog)", Tag = "DurabilityLow" });
			SortFilterComboBox.Items.Add(new ComboBoxItem { Content = "Meest zeldzaam (Hoog-Laag)", Tag = "RarityHigh" });
			SortFilterComboBox.Items.Add(new ComboBoxItem { Content = "Minst zeldzaam (Laag-Hoog)", Tag = "RarityLow" });
			SortFilterComboBox.Items.Add(new ComboBoxItem { Content = "Aantal (Hoog-Laag)", Tag = "QuantityHigh" });
			SortFilterComboBox.Items.Add(new ComboBoxItem { Content = "Aantal (Laag-Hoog)", Tag = "QuantityLow" });

			TypeFilterComboBox.SelectedIndex = 0;
			SortFilterComboBox.SelectedIndex = 0;
		}

		private async void ItemListView_ItemClick(object sender, ItemClickEventArgs e)
		{
			if(e.ClickedItem is InventoryItem inventoryItem)
			{
				var selectedItem = inventoryItem.Item;

				if(selectedItem != null)
				{
					var dialog = new ItemDetailsDialog(selectedItem, this.XamlRoot);
					await dialog.ShowAsync();
				}
			}
		}

		private void FilterTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ApplyFilters();
		}


		private void SortFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ApplyFilters();
		}

		private void ApplyFilters()
		{
			ApplyType();
			ApplySorting();

			DispatcherQueue.TryEnqueue(() =>
			{
				itemListview.ItemsSource = allItems;
			});
		}

		private void ApplyType()
		{
			string selectedType = (TypeFilterComboBox.SelectedItem as ComboBoxItem)?.Tag as string;

			if(string.IsNullOrEmpty(selectedType) || selectedType == "All")
			{
				allItems = new List<InventoryItem>(allItemsOriginal);
			}
			else
			{
				allItems = allItemsOriginal.Where(ii => ii.Item.Type.Name == selectedType).ToList();
			}
		}

		private void ApplySorting()
		{
			string selectedSort = (SortFilterComboBox.SelectedItem as ComboBoxItem)?.Tag as string;

			if(!string.IsNullOrEmpty(selectedSort) && selectedSort != "All")
			{
				switch(selectedSort)
				{
					case "StrengthHigh":
						allItems = allItems.OrderByDescending(ii => ii.Item.Statistics.Strength).ToList();
						break;
					case "StrengthLow":
						allItems = allItems.OrderBy(ii => ii.Item.Statistics.Strength).ToList();
						break;
					case "SpeedHigh":
						allItems = allItems.OrderByDescending(ii => ii.Item.Statistics.Speed).ToList();
						break;
					case "SpeedLow":
						allItems = allItems.OrderBy(ii => ii.Item.Statistics.Speed).ToList();
						break;
					case "DurabilityHigh":
						allItems = allItems.OrderByDescending(ii => ii.Item.Statistics.Durability).ToList();
						break;
					case "DurabilityLow":
						allItems = allItems.OrderBy(ii => ii.Item.Statistics.Durability).ToList();
						break;
					case "RarityHigh":
						allItems = allItems.OrderByDescending(ii => ii.Item.Statistics.RarityId).ToList();
						break;
					case "RarityLow":
						allItems = allItems.OrderBy(ii => ii.Item.Statistics.RarityId).ToList();
						break;
					case "QuantityHigh":
						allItems = allItems.OrderByDescending(ii => ii.Quantity).ToList();
						break;
					case "QuantityLow":
						allItems = allItems.OrderBy(ii => ii.Quantity).ToList();
						break;
				}
			}
		}
	}
}
