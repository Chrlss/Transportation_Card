using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Transportation_Card.Models;
using Transportation_Card.Services;
using Wpf.Ui.Controls;

namespace Transportation_Card;

public partial class MainWindow : Window
{
    private AuthService _authService;
    private User loggedInUser;

    public MainWindow()
    {
        InitializeComponent();
        _authService = new AuthService();
    }

    private void Login_Click(object sender, RoutedEventArgs e)
    {
        string username = txtUsername.Text;
        string password = txtPassword.Password;

        // Call the authentication service
        User loggedInUser = _authService.Login(username, password);

        // Ensure login is successful
        if (loggedInUser != null) // Check if login was successful
        {
            Dashboard dashboard = new Dashboard(loggedInUser); // Pass user object
            dashboard.Show();
            this.Close();
        }
        else
        {
            System.Windows.MessageBox.Show("Invalid email or username and password", "Error", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }


    private void Hyperlink_Click(object sender, RoutedEventArgs e)
    {
        Register newWindow = new Register(); // Assuming you have a NewWindow.xaml
        newWindow.Show();
        this.Close();
    }
}