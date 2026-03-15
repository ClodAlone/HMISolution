#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Syncfusion.Windows.Client.Olap
{
    #region Display Mode Enumerations

    /// <summary>
    /// Enumeration Code for Displaying Modes.
    /// </summary>
    public enum DisplayModes
    {
        /// <summary>
        /// Client will show with both Chart and Grid.
        /// </summary>
        Both,
        /// <summary>
        /// Client will show only with Chart.
        /// </summary>
        ChartOnly,
        /// <summary>
        /// Client will show only with Grid.
        /// </summary>
        GridOnly
    }
    #endregion

    #region CheckbxvisbltyConv Converter
    /// <summary>
    /// Convert the CheckBox visibility to string
    /// </summary>
    public class CheckbxvisbltyConv : IValueConverter
    {

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool val = (bool)value;
            return val ? "Visible" : "Hidden";
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    #endregion

    #region Constants Class Definition
    /// <summary>
    /// Constant Styles
    /// </summary>
    public class Contants
    {
        /// <summary>
        /// Represents Style Key as Brush
        /// </summary>
        public const string StyleKey = "Brush";
    }
    #endregion

    #region LoadWindow Enumeration
    /// <summary>
    /// Loading Windows in OLAPClient
    /// </summary>
    public enum LoadWindow
    {
        /// <summary>
        /// ConnectToServer Option
        /// </summary>
        ConnectToServer,

        /// <summary>
        /// OfflineCube Option
        /// </summary>
        LoadOfflineCube
    }
    #endregion

    #region OlapClientVisualStyle Enumeration
    /// <summary>
    /// Specifies the VisualStyle for OlapClient
    /// </summary>
    public enum OlapClientVisualStyle
    {
        /// <summary>
        /// Provides Blend Style for OlapClient
        /// </summary>
        Blend,
        /// <summary>
        /// Provides Default Style for OlapClient
        /// </summary>
        Default,
        /// <summary>
        /// Provides Metro Style for OlapClient
        /// </summary>
        Metro,
        /// <summary>
        /// Provides Office2003 Style for OlapClient
        /// </summary>
        Office2003,
        /// <summary>
        /// Provides Office2007Black Style for OlapClient
        /// </summary>
        Office2007Black,
        /// <summary>
        /// Provides Office2007Blue Style for OlapClient
        /// </summary>
        Office2007Blue,
        /// <summary>
        /// Provides Office2007Silver Style for OlapClient
        /// </summary>
        Office2007Silver,
        /// <summary>
        /// Provides Office2010Black Style for OlapClient
        /// </summary>
        Office2010Black,
        /// <summary>
        /// Provides Office2010Blue Style for OlapClient
        /// </summary>
        Office2010Blue,
        /// <summary>
        /// Provides Office2010Silver Style for OlapClient
        /// </summary>
        Office2010Silver,
        /// <summary>
        /// Provides Transparent style for OlapClient
        /// </summary>
        Transparent,
    }
    #endregion

    #region Common Class Definition
    /// <summary>
    /// Common class for internal purpose
    /// </summary>
    public class Common
    {
        /// <summary>
        /// Gets the local cube connection string.
        /// </summary>
        /// <param name="cubeLocation">The cube location.</param>
        /// <returns>Returns the LocalCubeConnectionString</returns>
        public static string GetLocalCubeConnectionString(string cubeLocation)
        {
            LocalCubeServerVersion localCubeServerVersion = LocalCubeHelper.GetAssemblyVersion();
            if (localCubeServerVersion == LocalCubeServerVersion.Invalid)
            {
                MessageBox.Show("Recommended version of Microsoft ADOMD.Net is not installed in your machine", "Loading error");
                return null;
            }

            if (cubeLocation != string.Empty)
            {
                return string.Format(@"Datasource='{0}'; Provider=msolap;", cubeLocation);
            }

            return null;
        }

        /// <summary>
        /// Gets the server connection string.
        /// </summary>
        /// <param name="serverName">Name of the server.</param>
        /// <param name="databaseName">Name of the database.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="passWord">The pass word.</param>
        /// <returns></returns>
        public static string GetServerConnectionString(string serverName, string databaseName, string userName, string passWord)
        {
            if (serverName != string.Empty && databaseName != string.Empty && userName != string.Empty && passWord != string.Empty)
            {
                return string.Format("Data Source={0}; Initial Catalog={1};User Id={2};Password={3};", serverName, databaseName, userName, passWord);
            }
            return null;
        }

        public static T GetParentWindow<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject dependencyObject = VisualTreeHelper.GetParent(child);
            if (dependencyObject != null)
            {
                T parent = dependencyObject as T;
                if (parent != null)
                {
                    return parent;
                }
                else
                {
                    return GetParentWindow<T>(dependencyObject);
                }
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// Gets the server connection string.
        /// </summary>
        /// <param name="serverName">Name of the server.</param>
        /// <param name="databaseName">Name of the database.</param>
        /// <returns>returns the ServerConnectionString</returns>
        public static string GetServerConnectionString(string serverName, string databaseName)
        {
            if (serverName != string.Empty && databaseName != string.Empty)
            {
                return string.Format("Data Source={0}; Initial Catalog={1};", serverName, databaseName);
            }

            return null;
        }
    }
    #endregion
}
