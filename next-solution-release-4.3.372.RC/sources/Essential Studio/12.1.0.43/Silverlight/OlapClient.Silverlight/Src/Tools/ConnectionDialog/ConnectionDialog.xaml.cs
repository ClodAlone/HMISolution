#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.Silverlight.Client.Olap;
using System.ComponentModel;
using Syncfusion.Silverlight.Client.Olap.Resources;
using System.Globalization;
using Syncfusion.OlapSilverlight.Manager;

namespace Syncfusion.Silverlight.Tools.Olap
{
    [DesignTimeVisible(false)]
    public partial class ConnectionDialog : WindowControl
    {
        #region Internal Members

        /// <summary>
        /// Holds the current connection string.
        /// </summary>
        internal string ConnectionString;

        /// <summary>
        /// Holds the Provider Name for current instance.
        /// </summary>
        internal Providers ProviderName;
        
        /// <summary>
        /// Holds the dialog result whether true or false.
        /// </summary>
        internal bool DialogResult; 

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionDialog"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public ConnectionDialog(string connectionString)
        {
            InitializeComponent();
            if (connectionString == null)
                connectionString = string.Empty;

            this.connectionStringBox.Text = this.ConnectionString = connectionString;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionDialog"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="providerName">The provider name. </param>
        public ConnectionDialog(string connectionString, Providers providerName)
        {
            InitializeComponent();
            if (connectionString == null)
                connectionString = string.Empty;

            this.connectionStringBox.Text = this.ConnectionString = connectionString;
            switch (providerName)
            {
                case Providers.SSAS:
                    this.providerSSAS.IsChecked = true;
                    break;
                case Providers.Mondrian:
                    this.providerMondrian.IsChecked = true;                    
                    break;
                case Providers.ActivePivot:
                    this.providerActivePivot.IsChecked = true;
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Helper Methods

        private void UpdateServerCubeOptions(bool enabled)
        {
            this.databaseNameBox.IsEnabled = enabled;
            this.serverNameBox.IsEnabled = enabled;
            this.userIdBox.IsEnabled = enabled;
            this.passwordBox.IsEnabled = enabled;
        }

        private void UpdateConnectionStringOptions(bool enabled)
        {
            this.connectionStringBox.IsEnabled = enabled;
        }

        private void serverCubeOption_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this.serverCubeOption.IsChecked.HasValue && this.serverCubeOption.IsChecked.Value)
            {
                this.UpdateServerCubeOptions(true);
                this.UpdateConnectionStringOptions(false);
            }
        }

        private void connectionStringOption_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this.connectionStringOption.IsChecked.HasValue && this.connectionStringOption.IsChecked.Value)
            {
                this.UpdateServerCubeOptions(false);
                this.UpdateConnectionStringOptions(true);
            }
        }

        private void okButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (Validate())
            {
                this.DialogResult = true;
                this.Close();
            }
        }

        private void cancelButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void credentialOption_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this.credentialOption.IsChecked.HasValue && this.credentialOption.IsChecked.Value)
            {
                this.credentialContainer.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                this.credentialContainer.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Validates this Connection Dialog window.
        /// </summary>
        /// <returns></returns>
        bool Validate()
        {
            if (this.serverCubeOption.IsChecked.Value == true)
            {
                if (this.serverNameBox.Text != null && this.serverNameBox.Text.Trim() != string.Empty)
                {
                    if (this.databaseNameBox.Text != null && this.databaseNameBox.Text.Trim() != string.Empty)
                    {
                        if (this.userIdBox.Text != string.Empty && this.passwordBox.Password != string.Empty)
                        {
                            this.ConnectionString = Common.GetServerConnectionString(this.serverNameBox.Text, this.databaseNameBox.Text, this.userIdBox.Text, this.passwordBox.Password);
                        }
                        else
                        {
                            this.ConnectionString = Common.GetServerConnectionString(this.serverNameBox.Text, this.databaseNameBox.Text);
                        }

                        if (this.providerSSAS.IsChecked.HasValue && this.providerSSAS.IsChecked.Value)
                            this.ProviderName = Providers.SSAS;
                        else if (this.providerMondrian.IsChecked.HasValue && this.providerMondrian.IsChecked.Value)
                            this.ProviderName = Providers.Mondrian;
                        else if (this.providerActivePivot.IsChecked.HasValue && this.providerActivePivot.IsChecked.Value)
                            this.ProviderName = Providers.ActivePivot;

                        return true;
                    }
                    else
                    {
                        WindowControl.ShowAlert(SR.GetString(CultureInfo.CurrentUICulture, "Message_PleaseEnterTheDatabaseName"), SR.GetString(CultureInfo.CurrentUICulture, "Message_Error"), DialogIcon.Error, DialogButton.OK, null, Windows.Tools.Controls.AnimationType.Zoom);
                        return false;
                    }
                }
                else
                {
                    WindowControl.ShowAlert(SR.GetString(CultureInfo.CurrentUICulture, "Message_PleaseEnterTheServerName"), SR.GetString(CultureInfo.CurrentUICulture, "Message_Error"), DialogIcon.Error, DialogButton.OK, null, Windows.Tools.Controls.AnimationType.Zoom);
                    return false;
                }
            }
            else if (this.connectionStringOption.IsChecked == true)
            {
                if (this.connectionStringBox.Text != string.Empty)
                {
                    this.ConnectionString = this.connectionStringBox.Text;
                    if (this.providerSSAS.IsChecked.HasValue && this.providerSSAS.IsChecked.Value)
                        this.ProviderName = Providers.SSAS;
                    else if (this.providerMondrian.IsChecked.HasValue && this.providerMondrian.IsChecked.Value)
                        this.ProviderName = Providers.Mondrian;
                    else if (this.providerActivePivot.IsChecked.HasValue && this.providerActivePivot.IsChecked.Value)
                        this.ProviderName = Providers.ActivePivot;

                    return true;
                }
                else
                {
                    WindowControl.ShowAlert(SR.GetString(CultureInfo.CurrentUICulture, "Message_PleaseEnterTheConnectionString"), SR.GetString(CultureInfo.CurrentUICulture, "Message_Error"), DialogIcon.Error, DialogButton.OK, null, Windows.Tools.Controls.AnimationType.Zoom);
                    return false;
                }
            }

            return false;
        }       

        #endregion

    }
}
