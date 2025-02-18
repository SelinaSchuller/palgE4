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
		private List<Item> allItemsOriginal = new List<Item>(); // Stores all original items
		private List<Item> allItems = new List<Item>(); // Stores currently filtered items
		private List<Data.Type> allTypes = new List<Data.Type>(); // Stores all item types
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

			// Initially, show all items
			allItems = new List<Item>(allItemsOriginal);
			itemListview.ItemsSource = allItems;

			// Populate Type Filter
			TypeFilterComboBox.Items.Add(new ComboBoxItem { Content = "Alle Types", Tag = "All" });
			TypeFilterComboBox.Items.Add(new ComboBoxItem { Content = "Wapen ⚔️", Tag = "Wapen" });
			TypeFilterComboBox.Items.Add(new ComboBoxItem { Content = "Accessoire 💎", Tag = "Accessoire" });
			TypeFilterComboBox.Items.Add(new ComboBoxItem { Content = "Armor 🛡️", Tag = "Armor" });

			// Populate Statistics Filter
			StatisticsFilterComboBox.Items.Add("Alle Statistieken");
			foreach(var stat in allStatistics)
			{
				StatisticsFilterComboBox.Items.Add(stat);
			}

			TypeFilterComboBox.SelectedIndex = 0;
			StatisticsFilterComboBox.SelectedIndex = 0;
		}

		private void FilterTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			string selectedType = (TypeFilterComboBox.SelectedItem as ComboBoxItem)?.Tag as string;

			if(string.IsNullOrEmpty(selectedType) || selectedType == "All")
			{
				allItems = new List<Item>(allItemsOriginal); // Reset to all items
			}
			else
			{
				allItems = allItemsOriginal.Where(i => i.Type.Name == selectedType).ToList();
			}

			ApplyFilters();
		}

		private void FilterStatisticComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			string selectedStat = StatisticsFilterComboBox.SelectedItem as string;

			if(!string.IsNullOrEmpty(selectedStat) && selectedStat != "Alle Statistieken")
			{
				switch(selectedStat)
				{
					case "Strength":
						allItems = allItems.Where(i => i.Statistics.Strength > 0).ToList();
						break;
					case "Speed":
						allItems = allItems.Where(i => i.Statistics.Speed > 0).ToList();
						break;
					case "Durability":
						allItems = allItems.Where(i => i.Statistics.Durability > 0).ToList();
						break;
				}
			}

			ApplyFilters();
		}

		private void ApplyFilters()
		{
			DispatcherQueue.TryEnqueue(() =>
			{
				itemListview.ItemsSource = allItems;
			});
		}
	}
}
