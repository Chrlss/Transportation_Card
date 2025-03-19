using System.Windows;
using System;
using System.Linq;
using System.Windows;
using Transportation_Card.Data;
using Transportation_Card.Models;
using MaterialDesignThemes.Wpf;
using Transportation_Card.Services;
using System.Globalization;



namespace Transportation_Card
{
    public partial class Dashboard : Window
    {
        private readonly AuthService _authService;
        private readonly int _loggedInUserId;
        private User _loggedInUser;
        public SnackbarMessageQueue MessageQueue { get; } = new SnackbarMessageQueue(TimeSpan.FromSeconds(3));
        public Dashboard(User user)
        {

            InitializeComponent();

            _authService = new AuthService();
            _loggedInUser = user;
            _loggedInUserId = user.ID;
            txtUsername.Text = user.Email;
            txtFullName.Text = user.FullName;
            txtCardType.Text = user.CardNumber;

            MainSnackbar.MessageQueue = MessageQueue;

            ShowSnackbar();
            DisplayCardType();
            LoadUserDetails();
        }

        private void LoadUserDetails()
        {
            var user = _loggedInUser; 
            if (user != null)
            {
                txtUsername.Text = user.Email;
                txtFullName.Text = user.FullName;
                txtCardType.Text = user.CardNumber;
                txtInitialLoad.Text = string.Format(new CultureInfo("en-PH"), "{0:C}", user.InitialLoad);
                txtCreatedAt.Text = user.CreatedAt.ToString("f", CultureInfo.CurrentCulture); 
                txtExpirationDate.Text = user.ExpirationDate?.ToString("f", CultureInfo.CurrentCulture) ?? "N/A"; 
            }
        }


        private void DisplayCardType()
        {
            lblCardType.Content = $"{_loggedInUser.GetCardType()}";
        }

        private void ReloadCard_Click(object sender, RoutedEventArgs e)
        {
            if (decimal.TryParse(txtReloadAmount.Text, out decimal amount))
            {
                try
                {
                    if (_loggedInUser == null)
                    {
                        MessageBox.Show("User is not logged in.");
                        return;
                    }

                    if (_loggedInUserId == 0)
                    {
                        MessageBox.Show("Invalid user ID.");
                        return;
                    }

                    _authService.ReloadCard(_loggedInUserId, amount);
                    MessageBox.Show("Card reloaded successfully!");

                    // Update the displayed initial load
                    _loggedInUser.InitialLoad += amount;
                    txtInitialLoad.Text = string.Format(new CultureInfo("en-PH"), "{0:C}", _loggedInUser.InitialLoad);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Please enter a valid amount.");
            }
        }



        private void ShowSnackbar()
        {
            MessageQueue.Enqueue("Login Successful");
        }
    }
}
