using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Utilities.WPF;
using WpfKb.Controls;

namespace Pads
{
    /// <summary>
    /// Interaction logic for NumericPad.xaml
    /// </summary>
    public partial class PasswordPad : UserControl
    {
        int? max;
        bool disablePerformTextChanged;

        public PasswordPad(string value, int? max = null, string padTagDescription = null)
        {
            InitializeComponent();

            if (max.HasValue)
            {
                this.max = max.Value;
                maxLenghtPanel.Visibility = Visibility.Visible;
                maxTxt.Content = max.Value.ToString();
                oldText = value;
                txt.PasswordChanged += Txt_TextChanged;
            }

            if (!string.IsNullOrWhiteSpace(padTagDescription))
            {
                TagDescriptionPanel.Visibility = Visibility.Visible;
                TagDescriptionTxt.Content = padTagDescription;
            }

            txt.Password = value;
            txt.SelectAll();
            var keypad = new OnScreenKeyboard(Properties.Settings.Default.PasswordFontSize);
            Grid.SetRow(keypad, 1);
            grid.Children.Add(keypad);

            keypad.BeginInit();
            var keys = this.GetChildrenOfType<OnScreenKey>();
            foreach (var key in keys)
            {
                key.PreviewOnScreenKeyDown += (s, e) =>
                {
                    if (e.Handled == false)
                    {
                        txt.Focusable = true;
                        txt.Focus();
                    }
                };
            }
            keypad.EndInit();
        }

        string oldText;
        private void Txt_TextChanged(object sender, RoutedEventArgs e)
        {
            if (!disablePerformTextChanged)
            {
                string text = (sender as PasswordBox).Password;
                if (!string.IsNullOrEmpty(text) && text.Length > max)
                    BlinkOnError();
                else
                    oldText = (sender as PasswordBox).Password;
            }
        }

        private void BlinkOnError()
        {
            Storyboard story = txt.FindResource("blink") as Storyboard;
            if (story != null)
            {
                DoubleAnimation doubleAnimation = story.Children[0] as DoubleAnimation;
                doubleAnimation.RepeatBehavior = new RepeatBehavior(2);
                doubleAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(100));
                Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath("Opacity"));
                story.Begin();
            }

            disablePerformTextChanged = true;
            txt.Password = oldText;
            disablePerformTextChanged = false;
        }

        public String GetValue()
        {
            return txt.Password;
        }
    }
}
