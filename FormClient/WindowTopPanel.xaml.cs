using System;
using System.Windows;
using System.Windows.Controls;

namespace FormClient
{
    public partial class WindowTopPanel : UserControl
    {
        private Window window;

        public WindowTopPanel()
        {
            InitializeComponent();
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            window.WindowState = window.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            window.WindowState = WindowState.Minimized;
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            window = Window.GetWindow(this);
        }
    }
}
