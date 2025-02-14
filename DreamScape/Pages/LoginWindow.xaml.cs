using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using DreamScape.Services;
using DreamScape.Data;
using Microsoft.VisualBasic;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DreamScape.Pages
{
	/// <summary>
	/// An empty window that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class LoginWindow : Window
	{
		private int _userId { get; set; }
		public LoginWindow()
		{
			this.InitializeComponent();
			this.Title = "Login Pagina";
			Fullscreen fullscreenService = new Fullscreen();
			fullscreenService.SetFullscreen(this);
		}

		private string HashPassword(string password)
		{
			using(var sha256 = System.Security.Cryptography.SHA256.Create())
			{
				var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
				return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
			}
		}

		private void LoginButton_Click(object sender, RoutedEventArgs e)
		{
			using(var db = new AppDbContext())
			{
				string email = mailTextBox.Text;
				string password = PasswordTextBox.Password;
				string hashedPassword = HashPassword(password);

				var user = db.Users.FirstOrDefault(u => u.Email == email && u.Password == hashedPassword);

				if(user != null)
				{
					int roleId = user.RoleId;
					int userId = user.Id;
					_userId = userId;
					Data.User.LoggedInUser = user;

					switch(roleId)
					{
						case 1:
							var playerDashboardWindow = new Pages.Player.PlayerDashboardWindow();
							playerDashboardWindow.Activate();
							this.Close();
							break;

						case 2:
							var onderhoudDashboard = new Pages.Admin.AdminDashboardWindow();
							onderhoudDashboard.Activate();
							this.Close();
							break;

						default:
							ShowError("Er is geen rol aan gebruiker toegevoegd");
							return;
					}
				}
				else
				{
					ShowError("E-mail of wachtwoord is onjuist");
				}
			}
		}
		private void ShowError(string message)
		{
			ErrorInfoBar.Message = message;
			ErrorInfoBar.IsOpen = true;
		}

		private void RegistreerButton_Click(object sender, RoutedEventArgs e)
		{
			var createAccountWindow = new Pages.Account.CreateAccountWindow();
			createAccountWindow.Activate();
			this.Close();
		}
	}
}
