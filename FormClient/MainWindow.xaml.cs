using Reactive;
using Reactive.Observables;
using Reactive.Operators;
using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Windows;
using System.Windows.Shapes;

namespace FormClient
{
    public partial class MainWindow : Window
    {
        
        public MainWindow()
        {
            InitializeComponent();
            var random = new Random();
            TimeSpan.FromMilliseconds(100)
                .Observable()
                .Subscribe(_ =>
                    {
                        var rand1 = random.NextDouble();
                        var rand2 = random.NextDouble();
                        GameField.Children.Clear();
                        GameField.Children.Add(
                            new Line
                            {
                                Stroke = System.Windows.Media.Brushes.LightSteelBlue,
                                X1 = 1,
                                X2 = rand1 * 100,
                                Y1 = 1,
                                Y2 = rand2 * 100,
                                HorizontalAlignment = HorizontalAlignment.Left,
                                VerticalAlignment = VerticalAlignment.Center,
                                StrokeThickness = 2
                            });
                    });
        }

        private async void PostButton_Click(object sender, RoutedEventArgs evnt)
        {
            try
            {
                var response = await App.HttpClient.PutAsync(App.ServerIp + "code", new StringContent(textBox.Text));
                if (!response.IsSuccessStatusCode)
                {
                    Close();
                }
            }
            catch (HttpRequestException e)
            {
                await errorBox.Dispatcher.InvokeAsync(() => { errorBox.Text = e.Message; });
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs evnt)
        {
            var codeEndpoint = App.ServerIp + "code";
            var defaultProgram = Application.Current.Resources["DefaultProgram"] as string;

            App.HttpClient.GetObservable(codeEndpoint)
                .Filter(response => response.IsSuccessStatusCode)
                .Map(response => response.Content.ReadAsStringAsync().Result)
                .StartWith(defaultProgram)
                .Subscribe(response => textBox.Dispatcher.Invoke(() => textBox.Text = response));

            try
            {
                var response = await App.HttpClient.GetAsync(App.ServerIp + "map/0,0");
                if (response.IsSuccessStatusCode)
                {
                    using (var decompressionStream = new DeflateStream(await response.Content.ReadAsStreamAsync(), CompressionMode.Decompress))
                    {
                        using (var stringStream = new StreamReader(decompressionStream))
                        {
                            Console.WriteLine(stringStream.ReadToEnd());
                        }
                    }
                }
            }
            catch (HttpRequestException e)
            {
                await errorBox.Dispatcher.InvokeAsync(() => { errorBox.Text = e.Message; });
            }
        }
    }
}
