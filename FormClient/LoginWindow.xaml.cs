using DTOs;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;

namespace FormClient
{
    public partial class LoginWindow : Window
    {

        public LoginWindow()
        {
            InitializeComponent();
            IpBox.Text = App.ServerIp;
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (usernameBox.Visibility != Visibility.Collapsed)
            {
                usernameBox.Visibility = Visibility.Collapsed;
                return;
            }
            var loginData = new LoginDTO
            {
                Email = emailBox.Text,
                Password = passwordBox.Password
            };
            if (await Login(loginData))
            {
                App.ServerIp = IpBox.Text;
                var mainWindow = new MainWindow();
                mainWindow.Show();
                Close();
            }
        }

        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            if (usernameBox.Visibility != Visibility.Visible)
            {
                usernameBox.Visibility = Visibility.Visible;
                return;
            }
            var registerData = new RegisterDTO
            {
                Username = usernameBox.Text,
                Email = emailBox.Text,
                Password = passwordBox.Password
            };
            if (await Register(registerData))
            {
                App.ServerIp = IpBox.Text;
                var mainWindow = new MainWindow();
                mainWindow.Show();
                Close();
            }
        }

        private async Task<bool> Login(LoginDTO loginData)
        {
            var serializedLoginData = JsonConvert.SerializeObject(loginData);
            var response = await App.HttpClient.PostAsync(IpBox.Text + "account/login", new StringContent(serializedLoginData));
            return response.IsSuccessStatusCode;
        }

        private async Task<bool> Register(RegisterDTO registerData)
        {
            var serializedRegisterData = JsonConvert.SerializeObject(registerData);
            var response = await App.HttpClient.PostAsync(IpBox.Text + "account/register", new StringContent(serializedRegisterData));
            if (response.IsSuccessStatusCode)
            {
                return await Login(new LoginDTO { Email = registerData.Email, Password = registerData.Password });
            }
            return false;
        }
    }
}
