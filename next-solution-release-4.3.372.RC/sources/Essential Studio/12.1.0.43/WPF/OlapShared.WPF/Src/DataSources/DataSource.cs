#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Windows;
using Syncfusion.Olap.Manager;
using System.Configuration;

namespace Syncfusion.Windows.Shared.Olap
{
    /// <summary>
    /// Datasource class holds connection information for UI element.
    /// </summary>
    public class DataSource
    {
        #region Members

        private static OlapDataManager _olapDataManager; 
        
        #endregion
                
        #region Attached Properties

        /// <summary>
        /// Gets the name of the connection.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static string GetConnectionName(DependencyObject obj)
        {
            return (string)obj.GetValue(ConnectionNameProperty);
        }

        /// <summary>
        /// Sets the name of the connection.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">The value.</param>
        public static void SetConnectionName(DependencyObject obj, string value)
        {
            obj.SetValue(ConnectionNameProperty, value);
        }

        /// <summary>
        /// ConnectionNameProperty holds the connection name information of the Datasource class.
        /// </summary>
        public static readonly DependencyProperty ConnectionNameProperty =
            DependencyProperty.RegisterAttached("ConnectionName", typeof(string), typeof(DataSource), new UIPropertyMetadata(null, OnConnectionNameChanged));

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static string GetConnectionString(DependencyObject obj)
        {
            return (string)obj.GetValue(ConnectionStringProperty);
        }

        /// <summary>
        /// Sets the connection string.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">The value.</param>
        public static void SetConnectionString(DependencyObject obj, string value)
        {
            obj.SetValue(ConnectionStringProperty, value);
        }

        /// <summary>
        /// ConnectionStringProperty holds the connection string information of the DataSource.
        /// </summary>
        public static readonly DependencyProperty ConnectionStringProperty =
            DependencyProperty.RegisterAttached("ConnectionString", typeof(string), typeof(DataSource), new UIPropertyMetadata(null, OnConnectionStringChanged));

        

        /// <summary>
        /// Gets the name of the data manager.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static string GetDataManagerName(DependencyObject obj)
        {
            return (string)obj.GetValue(DataManagerNameProperty);
        }

        /// <summary>
        /// Sets the name of the data manager.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">The value.</param>
        public static void SetDataManagerName(DependencyObject obj, string value)
        {
            obj.SetValue(DataManagerNameProperty, value);
        }

        /// <summary>
        /// DataManagerNameProperty holds the shared data manager name of the DataSource.
        /// </summary>
        public static readonly DependencyProperty DataManagerNameProperty =
            DependencyProperty.RegisterAttached("DataManagerName", typeof(string), typeof(DataSource), new UIPropertyMetadata(null));
                       
        #endregion

        #region Helper Methods

        /// <summary>
        /// Called when [connection string changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnConnectionStringChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue != null)
            {
                try
                {
                    DataSource._olapDataManager = new OlapDataManager(args.NewValue.ToString());
                    SharedDataManagers.Instance.DataManagers.Add(new DataManager { Name = DataSource.GetDataManagerName(d), OlapDataManager = DataSource._olapDataManager });

                }
                catch (Exception e)
                {
                    MessageBox.Show("Unable to open the connection", "Connection failed", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        /// <summary>
        /// Called when [connection name changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnConnectionNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if (DataSource._olapDataManager == null)
            {
                ConnectionStringSettingsCollection connSettings = ConfigurationManager.ConnectionStrings;
                foreach (ConnectionStringSettings item in connSettings)
                {
                    if (item.Name.Equals(DataSource.GetConnectionName(d)))
                    {
                        DataSource._olapDataManager = new OlapDataManager(item.ConnectionString);
                        SharedDataManagers.Instance.DataManagers.Add(new DataManager { Name = GetDataManagerName(d), OlapDataManager = DataSource._olapDataManager });
                    }
                }
            }
        }

        #endregion
    }
}
