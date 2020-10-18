using DTOs;
using Newtonsoft.Json;
using System.Net.Http;
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
            var loginData = new LoginDTO
            {
                Email = emailBox.Text,
                Password = passwordBox.Password
            };
            var serializedLoginData = JsonConvert.SerializeObject(loginData);
            var response = await App.HttpClient.PostAsync(IpBox.Text + "account/login", new StringContent(serializedLoginData));
            if (response.IsSuccessStatusCode)
            {
                App.ServerIp = IpBox.Text;
                var mainWindow = new MainWindow();
                mainWindow.Show();
                Close();
            }
        }
    }
}
