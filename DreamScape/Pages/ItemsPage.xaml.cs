using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.EntityFrameworkCore;
using DreamScape.Data;

namespace DreamScape.Pages
{
	public sealed partial class ItemsPage : Page
	{
		private List<Item> allItemsOriginal = new List<Item>();
		private List<Item> allItems = new List<Item>();
		private List<string> allStatistics = new List<string> { "Strength", "Speed", "Durability" };

		public ItemsPage()
		{
			this.InitializeComponent();
			LoadItems();
		}

		private void LoadItems()
		{
			using(var db = new AppDbContext())
			{
				allItemsOriginal = db.Items
					.Include(i => i.Type)
					.Include(i => i.Statistics)
					.OrderBy(i => i.Name)
					.ToList();
			}

			allItems = new List<Item>(allItemsOriginal);
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

			TypeFilterComboBox.SelectedIndex = 0;
			SortFilterComboBox.SelectedIndex = 0;
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
				allItems = new List<Item>(allItemsOriginal);
			}
			else
			{
				allItems = allItemsOriginal.Where(i => i.Type.Name == selectedType).ToList();
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
						allItems = allItems.OrderByDescending(i => i.Statistics.Strength).ToList();
						break;
					case "StrengthLow":
						allItems = allItems.OrderBy(i => i.Statistics.Strength).ToList();
						break;
					case "SpeedHigh":
						allItems = allItems.OrderByDescending(i => i.Statistics.Speed).ToList();
						break;
					case "SpeedLow":
						allItems = allItems.OrderBy(i => i.Statistics.Speed).ToList();
						break;
					case "DurabilityHigh":
						allItems = allItems.OrderByDescending(i => i.Statistics.Durability).ToList();
						break;
					case "DurabilityLow":
						allItems = allItems.OrderBy(i => i.Statistics.Durability).ToList();
						break;
					case "RarityHigh":
						allItems = allItems.OrderByDescending(i => i.Statistics.RarityId).ToList();
						break;
					case "RarityLow":
						allItems = allItems.OrderBy(i => i.Statistics.RarityId).ToList();
						break;
				}
			}
		}
	}
}
