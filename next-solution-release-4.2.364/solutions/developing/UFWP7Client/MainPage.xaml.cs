using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace UFWP7Client
{
    public partial class MainPage : PhoneApplicationPage
    {
        private AppSettings settings = new AppSettings();

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Copy the current settings into the text boxes as the new values
            // entered in will not be saved until the user clicks the 'done' button.
            textBoxUsername.Text = settings.UsernameSetting;
            passwordBoxPassword.Password = settings.PasswordSetting;
            textBoxServer.Text = settings.ServerSetting;

            // Set the data context of the listbox control to the sample data
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        // Load data for the ViewModel Items
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void ApplicationBarMenuItem_Click(object sender, EventArgs e)
        {
            settings.UsernameSetting = textBoxUsername.Text;
            settings.PasswordSetting = passwordBoxPassword.Password;
            settings.ServerSetting = textBoxServer.Text;

            NavigationService.Navigate(new Uri("/Views/ScreenContainer.xaml", UriKind.Relative));
        }
    }
}