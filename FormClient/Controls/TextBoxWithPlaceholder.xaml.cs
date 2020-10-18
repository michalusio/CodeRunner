using System.Windows;
using System.Windows.Controls;

namespace FormClient.Controls
{
    public partial class TextBoxWithPlaceholder : UserControl
    {
        public string Placeholder
        {
            get { return (string)GetValue(PlaceholderProperty); }
            set { SetValue(PlaceholderProperty, value); }
        }

        public static DependencyProperty PlaceholderProperty =
           DependencyProperty.Register(nameof(Placeholder), typeof(string), typeof(TextBoxWithPlaceholder));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static DependencyProperty TextProperty =
           DependencyProperty.Register(nameof(Text), typeof(string), typeof(TextBoxWithPlaceholder));

        public TextBoxWithPlaceholder()
        {
            InitializeComponent();
        }


    }
}
