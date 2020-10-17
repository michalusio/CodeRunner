using System.Windows;
using System.Windows.Controls;

namespace FormClient.Controls
{
    public partial class PasswordBoxWithPlaceholder : UserControl
    {
        public string Placeholder
        {
            get { return (string)GetValue(PlaceholderProperty); }
            set { SetValue(PlaceholderProperty, value); }
        }

        public static DependencyProperty PlaceholderProperty =
           DependencyProperty.Register(nameof(Placeholder), typeof(string), typeof(PasswordBoxWithPlaceholder));

        private bool _textChangedEventBusy;
        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set
            {
                SetValue(PasswordProperty, value);
                if (!_textChangedEventBusy)
                {
                    SearchTermPasswordBox.Password = value;
                }
            }
        }

        public static DependencyProperty PasswordProperty =
           DependencyProperty.Register(nameof(Password), typeof(string), typeof(PasswordBoxWithPlaceholder), new PropertyMetadata(string.Empty));

        public PasswordBoxWithPlaceholder()
        {
            InitializeComponent();
        }

        private void SearchTermPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                _textChangedEventBusy = true;
                Password = SearchTermPasswordBox.Password;
            }
            finally
            {
                _textChangedEventBusy = false;
            }
        }
    }
}
