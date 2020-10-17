using System;
using System.Windows;
using System.Windows.Controls;

namespace FormClient.Controls
{
    public partial class WindowTopPanel : UserControl
    {
        private Window window;

        public Visibility MaximizeBox
        {
            get { return (Visibility)GetValue(MaximizeBoxProperty); }
            set { SetValue(MaximizeBoxProperty, value); }
        }

        public static DependencyProperty MaximizeBoxProperty =
           DependencyProperty.Register(nameof(MaximizeBox), typeof(Visibility), typeof(WindowTopPanel), new PropertyMetadata(Visibility.Visible));

        public Visibility MinimizeBox
        {
            get { return (Visibility)GetValue(MinimizeBoxProperty); }
            set { SetValue(MinimizeBoxProperty, value); }
        }

        public static DependencyProperty MinimizeBoxProperty =
           DependencyProperty.Register(nameof(MinimizeBox), typeof(Visibility), typeof(WindowTopPanel), new PropertyMetadata(Visibility.Visible));

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
