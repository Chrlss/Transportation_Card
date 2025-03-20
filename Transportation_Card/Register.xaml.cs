using System.Windows;
using System.Windows.Controls;
using Transportation_Card.Services;

namespace Transportation_Card
{
    public partial class Register : Window
    {
        private AuthService _authService;

        public Register()
        {
            InitializeComponent();
            _authService = new AuthService();
        }

        private void CbCardType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbCardType.SelectedItem != null)
            {
                var selectedCardType = (cbCardType.SelectedItem as ComboBoxItem).Content.ToString();
                if (selectedCardType == "Senior Citizen" || selectedCardType == "PWD")
                {
                    if (lblCardNumber != null && txtCardNumber != null)
                    {
                        lblCardNumber.Visibility = Visibility.Visible;
                        txtCardNumber.Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    if (lblCardNumber != null && txtCardNumber != null)
                    {
                        lblCardNumber.Visibility = Visibility.Collapsed;
                        txtCardNumber.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        private void TxtCardNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (cbCardType.SelectedItem != null)
            {
                var selectedCardType = (cbCardType.SelectedItem as ComboBoxItem).Content.ToString();
                if (selectedCardType == "Senior Citizen")
                {
                    ApplyFormat(txtCardNumber, "##-###-####");
                }
                else if (selectedCardType == "PWD")
                {
                    ApplyFormat(txtCardNumber, "####-####-####");
                }
            }
        }

        private void ApplyFormat(TextBox textBox, string format)
        {
            string text = new string(textBox.Text.Where(char.IsDigit).ToArray());
            int textIndex = 0;
            string formattedText = "";

            foreach (char c in format)
            {
                if (c == '#')
                {
                    if (textIndex < text.Length)
                    {
                        formattedText += text[textIndex];
                        textIndex++;
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    if (textIndex < text.Length)
                    {
                        formattedText += c;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            textBox.Text = formattedText;
            textBox.CaretIndex = formattedText.Length;
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var fullName = txtFullName.Text;
                var username = txtUsername.Text;
                var email = txtEmail.Text;
                var password = txtPassword.Password;
                var address = txtAddress.Text;
                var dateOfBirth = dpDateOfBirth.SelectedDate ?? DateTime.Now;
                var mobileNumber = txtMobileNumber.Text;
                var cardType = (cbCardType.SelectedItem as ComboBoxItem)?.Content?.ToString();
                var cardNumber = txtCardNumber.Text;
                var SeniorCitizenCard = txtCardNumber.Text;
                var PwdId = txtCardNumber.Text;

                if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) ||
                    string.IsNullOrWhiteSpace(address) || string.IsNullOrWhiteSpace(mobileNumber))
                {
                    throw new ArgumentException("All fields are required.");
                }

                if (cardType == "Senior Citizen" || cardType == "PWD")
                {
                    if (string.IsNullOrWhiteSpace(cardNumber))
                    {
                        throw new ArgumentException("Card number is required for Senior Citizen and PWD card types.");
                    }

                    _authService.RegisterDiscountedCard(fullName, username, email, password, address, dateOfBirth, mobileNumber, cardType, cardNumber, SeniorCitizenCard, PwdId);
                }
                else
                {
                    _authService.Register(fullName, username, email, password, address, dateOfBirth, mobileNumber);
                }

                MessageBoxResult result = MessageBox.Show("Registration successful!", "Success", MessageBoxButton.OK);

                if (result == MessageBoxResult.OK)
                {
                    MainWindow loginWindow = new MainWindow();
                    loginWindow.Show();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Registration failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            MainWindow newWindow = new MainWindow(); /
            newWindow.Show();
            this.Close();
        }
    }
}
