#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Syncfusion.Windows.Shared;
using System.ComponentModel;

namespace Syncfusion.Windows.Client.Olap
{
    /// <summary>
    /// Interaction logic for ConnectToServer.xaml
    /// </summary>
    #if SyncfusionFramework4_0
    [DesignTimeVisible(false)]
#endif
    public partial class ConnectToServer : ChromelessWindow
    {
        #region Variables
        private SolidColorBrush currentBrush;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectToServer"/> class.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        public ConnectToServer(Dictionary<string, object> dictionary)
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(this.ConnectToServer_Loaded);
            this.ConnectionString = string.Empty;
            SolidColorBrush brush = dictionary[Contants.StyleKey] as SolidColorBrush;
            if (brush != null)
            {
                this.currentBrush = brush;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectToServer"/> class.
        /// </summary>
        public ConnectToServer()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(this.ConnectToServer_Loaded);
            this.ConnectionString = string.Empty;
            this.currentBrush = Brushes.Blue;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        public string ConnectionString { get; set; }
        #endregion

        #region Events
        /// <summary>
        /// Handles the Click event of the btnCancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Handles the Click event of the btnOK control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (txtServerName.Text != string.Empty)
            {
                if (txtDatabaseName.Text != string.Empty)
                {
                    this.ConnectionString = Common.GetServerConnectionString(txtServerName.Text, txtDatabaseName.Text);
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Please enter the Database name");
                }
            }
            else
            {
                MessageBox.Show("Please enter the server name");
            }
        }

        /// <summary>
        /// Handles the Loaded event of the ConnectToServer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void ConnectToServer_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.currentBrush == Brushes.Blue)
            {
                SkinStorage.SetVisualStyle(this.connectToServer1, "Office2007Blue");
            }
            else if (this.currentBrush == Brushes.Black)
            {
                SkinStorage.SetVisualStyle(this.connectToServer1, "Office2007Black");
            }
            else if (this.currentBrush == Brushes.Silver)
            {
                SkinStorage.SetVisualStyle(this.connectToServer1, "Office2007Silver");
            }
            else
            {
                SkinManager.SetActiveColorScheme(this.connectToServer1, this.currentBrush);
            }
        }
        #endregion
    }
}
