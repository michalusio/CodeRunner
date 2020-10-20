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
            var loginResult = await Login(loginData);
            switch (loginResult)
            {
                case ConnectionState.Ok:
                    App.ServerIp = IpBox.Text;
                    var mainWindow = new MainWindow();
                    mainWindow.Show();
                    Close();
                    break;

                case ConnectionState.NotOk:
                    await errorBox.Dispatcher.InvokeAsync(() => { errorBox.Text = "Cannot log in - Maybe the password is wrong?"; });
                    break;
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
            var registerResult = await Register(registerData);
            switch (registerResult)
            {
                case ConnectionState.Ok:
                    App.ServerIp = IpBox.Text;
                    var mainWindow = new MainWindow();
                    mainWindow.Show();
                    Close();
                    break;

                case ConnectionState.NotOk:
                    await errorBox.Dispatcher.InvokeAsync(() => { errorBox.Text = "Cannot register - Maybe the email is already registered?"; });
                    break;
            }
        }

        private async Task<ConnectionState> Login(LoginDTO loginData)
        {
            try
            {
                var serializedLoginData = JsonConvert.SerializeObject(loginData);
                var response = await App.HttpClient.PostAsync(IpBox.Text + "account/login", new StringContent(serializedLoginData));
                return response.IsSuccessStatusCode ? ConnectionState.Ok : ConnectionState.NotOk;
            }
            catch (HttpRequestException e)
            {
                await errorBox.Dispatcher.InvokeAsync(() => { errorBox.Text = e.Message; });
                return ConnectionState.Failed;
            }
        }

        private async Task<ConnectionState> Register(RegisterDTO registerData)
        {
            try
            {
                var serializedRegisterData = JsonConvert.SerializeObject(registerData);
                var response = await App.HttpClient.PostAsync(IpBox.Text + "account/register", new StringContent(serializedRegisterData));
                if (response.IsSuccessStatusCode)
                {
                    return await Login(new LoginDTO { Email = registerData.Email, Password = registerData.Password });
                }
                return ConnectionState.NotOk;
            }
            catch (HttpRequestException e)
            {
                await errorBox.Dispatcher.InvokeAsync(() => { errorBox.Text = e.Message; });
                return ConnectionState.Failed;
            }
        }

        private enum ConnectionState
        {
            Failed,
            NotOk,
            Ok
        }
    }
}
