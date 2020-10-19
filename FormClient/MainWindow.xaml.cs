﻿using System.Net.Http;
using System.Windows;

namespace FormClient
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
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
            var responseProgram = Application.Current.Resources["DefaultProgram"] as string;
            try
            {
                var response = await App.HttpClient.GetAsync(App.ServerIp + "code");
                if (response.IsSuccessStatusCode)
                {
                    responseProgram = await response.Content.ReadAsStringAsync();
                }
            }
            catch (HttpRequestException e)
            {
                await errorBox.Dispatcher.InvokeAsync(() => { errorBox.Text = e.Message; });
            }
            
            await textBox.Dispatcher.InvokeAsync(() => textBox.Text = responseProgram);
        }
    }
}
