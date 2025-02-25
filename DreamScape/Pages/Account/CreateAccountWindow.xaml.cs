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
using System.Threading.Tasks;
using DreamScape.Services;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DreamScape.Pages.Account
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CreateAccountWindow : Window
    {
		private int _userId { get; set; }
        public CreateAccountWindow()
        {
            this.InitializeComponent();

			this.Title = "Registreer Pagina";
			Fullscreen fullscreenService = new Fullscreen();
			fullscreenService.SetFullscreen(this);
		}

		private void CreateAccountButton_Click(object sender, RoutedEventArgs e)
		{
			using(var db = new AppDbContext())
			{
				string username = usernameTextBox.Text.Trim();
				username = StringFormats.CapitalizeFirstLetter(username);
				string email = mailTextBox.Text.Trim();
				string password = PasswordTextBox.Password.Trim();
				string confirmPassword = ConfirmPasswordTextBox.Password.Trim();

				if(string.IsNullOrWhiteSpace(username) || username.Length < 3)
				{
					ShowError("Gebruikersnaam moet minimaal 3 karakters lang zijn.");
					return;
				}

				if(!IsValidEmail(email))
				{
					ShowError("Voer een geldig e-mailadres in.");
					return;
				}

				if(!IsValidPassword(password))
				{
					ShowError("Wachtwoord moet minimaal 5 karakters bevatten en ten minste 1 cijfer.");
					return;
				}

				if(password != confirmPassword)
				{
					ShowError("❌ Wachtwoorden komen niet overeen.");
					return;
				}

				var existingUser = db.Users.FirstOrDefault(u => u.Username == username);
				if(existingUser != null)
				{
					ShowError("Deze gebruikersnaam bestaat al. Kies een andere.");
					return;
				}

				string hashedPassword = AppDbContext.HashPassword(password);

				var newUser = new User
				{
					Username = username,
					Email = email,
					Password = hashedPassword,
					RoleId = 1 // Standaardrol: Speler
				};

				db.Users.Add(newUser);
				db.SaveChanges();

				Data.User.LoggedInUser = newUser;

				ShowSuccess("✅ Account succesvol aangemaakt! U wordt nu ingelogd...");

				Task.Delay(2000).ContinueWith(t =>
				{
					DispatcherQueue.TryEnqueue(() =>
					{
						var playerDashboard = new Pages.Player.PlayerDashboardWindow();
						playerDashboard.Activate();
						this.Close();
					});
				});
			}
		}

		private bool IsValidEmail(string email)
		{
			try
			{
				var addr = new System.Net.Mail.MailAddress(email);
				return addr.Address == email;
			}
			catch
			{
				return false;
			}
		}

		private bool IsValidPassword(string password)
		{
			return password.Length >= 5 && password.Any(char.IsDigit);
		}

		private void ShowError(string message)
		{
			ErrorInfoBar.Title = "Fout bij account aanmaken";
			ErrorInfoBar.Message = message;
			ErrorInfoBar.Severity = InfoBarSeverity.Error;
			ErrorInfoBar.IsOpen = true;
		}

		private void ShowSuccess(string message)
		{
			ErrorInfoBar.Title = "Succes!";
			ErrorInfoBar.Message = message;
			ErrorInfoBar.Severity = InfoBarSeverity.Success;
			ErrorInfoBar.IsOpen = true;
		}

		private void LoginWindowButton_Click(object sender, RoutedEventArgs e)
		{
			var loginWindow = new Pages.LoginWindow();
			loginWindow.Activate();
			this.Close();
		}
	}
}
