using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using DreamScape.Data;
using DreamScape.Services;
using DreamScape.Pages.Player;
using Microsoft.UI.Xaml.Navigation;

namespace DreamScape.Pages.Account
{
	public sealed partial class EditAccountPage : Page
	{
		private User _loggedInUser;
		private PlayerDashboardWindow _parentWindow;

		public EditAccountPage()
		{
			this.InitializeComponent();
			LoadUserData();
		}

		public void LoadUserData()
		{
			if(User.LoggedInUser != null)
			{
				_loggedInUser = User.LoggedInUser;
				UsernameTextBox.Text = _loggedInUser.Username;
				EmailTextBox.Text = _loggedInUser.Email;
			}
			else
			{
				ShowError("❌ Geen gebruiker ingelogd.");
			}
		}

		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);
			_parentWindow = e.Parameter as PlayerDashboardWindow;
		}

		private void SaveButton_Click(object sender, RoutedEventArgs e)
		{
			if(_loggedInUser == null)
				return;

			using(var db = new AppDbContext())
			{
				string newUsername = UsernameTextBox.Text.Trim();
				newUsername = StringFormats.CapitalizeFirstLetter(newUsername);
				string newEmail = EmailTextBox.Text.Trim();
				string newPassword = PasswordBox.Password.Trim();
				string confirmPassword = ConfirmPasswordBox.Password.Trim();

				if(string.IsNullOrWhiteSpace(newUsername) || newUsername.Length < 3)
				{
					ShowError("Gebruikersnaam moet minimaal 3 karakters lang zijn.");
					return;
				}

				if(!IsValidEmail(newEmail))
				{
					ShowError("Voer een geldig e-mailadres in.");
					return;
				}

				if(!string.IsNullOrEmpty(newPassword) && !IsValidPassword(newPassword))
				{
					ShowError("Wachtwoord moet minimaal 5 karakters bevatten en ten minste 1 cijfer.");
					return;
				}

				if(!string.IsNullOrEmpty(newPassword) && newPassword != confirmPassword)
				{
					ShowError("❌ Wachtwoorden komen niet overeen.");
					return;
				}

				var existingUser = db.Users.FirstOrDefault(u => u.Username == newUsername && u.Id != _loggedInUser.Id);
				if(existingUser != null)
				{
					ShowError("Deze gebruikersnaam bestaat al. Kies een andere.");
					return;
				}

				var user = db.Users.FirstOrDefault(u => u.Id == _loggedInUser.Id);
				if(user != null)
				{
					user.Username = newUsername;
					user.Email = newEmail;

					if(!string.IsNullOrEmpty(newPassword))
						user.Password = AppDbContext.HashPassword(newPassword);

					db.SaveChanges();
					User.LoggedInUser = user;

					ShowSuccess("✅ Accountgegevens bijgewerkt!");
					Task.Delay(2000).ContinueWith(t =>
					{
						DispatcherQueue.TryEnqueue(() =>
						{
							_parentWindow.LoadData();
						});
					});
				}
				else
				{
					ShowError("❌ Gebruiker niet gevonden.");
				}
			}
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			_parentWindow.LoadData();
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
			ErrorInfoBar.Title = "❌ Fout bij bijwerken";
			ErrorInfoBar.Message = message;
			ErrorInfoBar.Severity = InfoBarSeverity.Error;
			ErrorInfoBar.IsOpen = true;
		}

		private void ShowSuccess(string message)
		{
			ErrorInfoBar.Title = "✅ Succes!";
			ErrorInfoBar.Message = message;
			ErrorInfoBar.Severity = InfoBarSeverity.Success;
			ErrorInfoBar.IsOpen = true;
		}
	}
}
