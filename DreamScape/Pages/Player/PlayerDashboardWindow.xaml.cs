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
using DreamScape.Services;
using Windows.UI.ApplicationSettings;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DreamScape.Pages.Player
{
	/// <summary>
	/// An empty window that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class PlayerDashboardWindow : Window
	{
		public PlayerDashboardWindow()
		{
			this.InitializeComponent();

			this.Title = "Speler Dashboard";
			Fullscreen fullscreenService = new Fullscreen();
			fullscreenService.SetFullscreen(this);

			LoadData();
		}

		private void LoadData()
		{
			UsernameTextBlock.Text = User.LoggedInUser.Username;

			DoubleFrames.Visibility = Visibility.Visible;
			SingleFrame.Visibility = Visibility.Collapsed;
			MainFrame.Navigate(typeof(ItemsPage), this);
			MainFrame2.Navigate(typeof(PlayerInventoryPage), this);
		}

		private void DashboardButton_Click(object sender, RoutedEventArgs e)
		{
			DoubleFrames.Visibility = Visibility.Visible;
			SingleFrame.Visibility = Visibility.Collapsed;
			MainFrame.Navigate(typeof(ItemsPage), this);
			MainFrame2.Navigate(typeof(PlayerInventoryPage), this);
		}

		private void ItemsButton_Click(object sender, RoutedEventArgs e)
		{
			DoubleFrames.Visibility = Visibility.Collapsed;
			SingleFrame.Visibility = Visibility.Visible;
			SingleFrame.Navigate(typeof(ItemsPage), this);
		}

		private void InventoryButton_Click(object sender, RoutedEventArgs e)
		{
			DoubleFrames.Visibility = Visibility.Collapsed;
			SingleFrame.Visibility = Visibility.Visible;
			SingleFrame.Navigate(typeof(PlayerInventoryPage), this);
		}

		private void SettingsButton_Click(object sender, RoutedEventArgs e)
		{
			//MainFrame.Navigate(typeof(AccountSettingsPage), this);
		}

		private void TradeButton_Click(object sender, RoutedEventArgs e)
		{
			//MainFrame.Navigate(typeof(TradePage), this);
		}
	}
}

