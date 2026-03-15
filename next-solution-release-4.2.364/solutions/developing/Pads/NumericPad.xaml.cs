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
    public partial class NumericPad : UserControl
    {
        double? min;
        double? max;
        public NumericPad(String value, double? min = null, double? max = null)
        {
            InitializeComponent();

            if (min.HasValue || max.HasValue)
            {
                if(min.HasValue)
                {
                    minTxt.Content = min.Value.ToString();
                    minValuePanel.Visibility = Visibility.Visible;
                    this.min = min.Value;
                }
                if (max.HasValue)
                {
                    maxTxt.Content = max.Value.ToString();
                    maxValuePanel.Visibility = Visibility.Visible;
                    this.max = max.Value;
                }
            }

            oldText = value;
            txt.TextChanged += Txt_TextChanged;
            txt.KeyUp += Txt_KeyUp;

            txt.Text = value;
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
                txt.Text = String.Empty;
            }
        }

        string oldText;
        private void Txt_TextChanged(object sender, TextChangedEventArgs e)
        {
            if ((sender as TextBox).Text == "-" || String.IsNullOrEmpty((sender as TextBox).Text))
            {
                oldText = (sender as TextBox).Text;
                return;
            }
            CheckErrors();
        }
        public bool CheckErrors(bool bUseRange = false)
        {
            Double value;
            if (!Double.TryParse(txt.Text, out value))
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
                oldText = txt.Text;
            return false;
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

            txt.Focusable = true;
            txt.Focus();
            txt.Text = oldText;
            txt.SelectAll();
        }

        public String GetValue()
        {
            return txt.Text;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            txt.Focusable = true;
            txt.Focus();
            if (txt.Text.Length == 0 || txt.Text.Length == 1 && txt.Text[0] == '0')
                return;
            if (txt.Text[0] == '-')
            {
                var s = txt.Text.Remove(0, 1);
                txt.Text = s.Replace("-", "");
            }
            else
            {
                var s = txt.Text.Replace("-", ""); ;
                txt.Text = "-" + s;
            }
            txt.CaretIndex = txt.Text.Length;
        }
    }
}
