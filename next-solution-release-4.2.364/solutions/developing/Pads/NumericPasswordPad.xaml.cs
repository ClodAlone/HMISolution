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
using WpfKb.Controls;

namespace Pads
{
    /// <summary>
    /// Interaction logic for NumericPad.xaml
    /// </summary>
    public partial class NumericPasswordPad : UserControl
    {
        double? max;
        double? min;
        public NumericPasswordPad(String value, double? min = null, double? max = null)
        {
            InitializeComponent();

            if (min.HasValue || max.HasValue)
            {
                if (min.HasValue)
                {
                    minTxt.Content = min.Value.ToString();
                    minLenghtPanel.Visibility = Visibility.Visible;
                    this.min = min.Value;
                }
                if (max.HasValue)
                {
                    maxTxt.Content = max.Value.ToString();
                    maxLenghtPanel.Visibility = Visibility.Visible;
                    this.max = max.Value;
                }
            }

            oldText = value;
            txt.FontSize = Properties.Settings.Default.NumericFontSize;
            txt.PasswordChanged += Txt_TextChanged;

            txt.Password = value;
            txt.SelectAll();
            var keypad = new OnScreenKeypad(Properties.Settings.Default.NumericFontSize);
            foreach (var key in keypad.Keys)
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
            Grid.SetRow(keypad, 1);
            grid.Children.Add(keypad);
        }

        private void Txt_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Clear)
            {
                e.Handled = true;
                txt.Focusable = true;
                txt.Focus();
                txt.Password = String.Empty;
            }
        }

        string oldText;
        private void Txt_TextChanged(object sender, RoutedEventArgs e)
        {
            string text = (sender as PasswordBox).Password;
            if (!string.IsNullOrEmpty(text) && text.Length > max)
                BlinkOnError();
            else
                oldText = (sender as PasswordBox).Password;
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

            txt.Password = oldText;
        }

        public bool CheckErrors(bool bUseRange = false)
        {
            Double value;
            if (!Double.TryParse(txt.Password, out value))
            {
                BlinkOnError();
                return true;
            }
            else if (bUseRange && ((min.HasValue && value < min.Value) || (max.HasValue && value > max.Value)))
            {
                BlinkOnError();
                return true;
            }
            else
                oldText = txt.Password;
            return false;
        }

        public String GetValue()
        {
            return txt.Password;
        }
    }
}
