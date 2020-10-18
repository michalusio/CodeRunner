using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;

namespace FormClient
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void PostButton_Click(object sender, RoutedEventArgs e)
        {
            var response = await App.HttpClient.PutAsync(App.ServerIp + "code", new StringContent(textBox.Text));
            if (!response.IsSuccessStatusCode)
            {
                Close();
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var response = await App.HttpClient.GetAsync(App.ServerIp + "code");
            var responseProgram = string.Empty;
            if (response.IsSuccessStatusCode)
            {
                responseProgram = await response.Content.ReadAsStringAsync();
            }
            else
            {
                responseProgram = Application.Current.Resources["DefaultProgram"] as string;
            }
            await textBox.Dispatcher.InvokeAsync(() => textBox.Text = responseProgram);
        }
    }
}
