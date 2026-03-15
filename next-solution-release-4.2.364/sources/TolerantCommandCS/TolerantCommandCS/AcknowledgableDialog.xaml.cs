using System;
using System.Windows;
using System.Windows.Media.Imaging;
using Bornander.UI.Commands.Tolerant;

namespace Bornander.UI.Commands.Test
{
    public partial class AcknowledgableDialog : Window
    {
        public DialogResult Result { get; private set; }

        public AcknowledgableDialog(string title, string message, bool isWarning)
        {
            InitializeComponent();
            Result = Tolerant.DialogResult.No;

            Title = title;
            this.message.Text = message;

            if (isWarning)
                ok.Visibility = Visibility.Collapsed;
            else
            {
                yes.Visibility = Visibility.Collapsed;
                no.Visibility = Visibility.Collapsed;
                doNotShowAgain.Visibility = Visibility.Collapsed;
            }

            icon.Source = new BitmapImage(isWarning ? new Uri("pack://application:,,,/Resources/Icons/Warning.png") : new Uri("pack://application:,,,/Resources/Icons/Error.png"));
        }

        private void yes_Click(object sender, RoutedEventArgs e)
        {
            Result = doNotShowAgain.IsChecked.Value ? Tolerant.DialogResult.YesAndRememberMyDecision : Tolerant.DialogResult.Yes;

            Close();
        }

        private void no_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ok_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
