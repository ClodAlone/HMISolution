#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Syncfusion.Silverlight.Client.Olap
{
    public class Common
    {

        /// <summary>
        /// Gets the server connection string.
        /// </summary>
        /// <param name="serverName">Name of the server.</param>
        /// <param name="databaseName">Name of the database.</param>
        /// <param name="userId">The user id.</param>
        /// <param name="passWord">The password.</param>
        /// <returns>connection string</returns>
        public static string GetServerConnectionString(string serverName, string databaseName, string userId, string passWord)
        {
            if (serverName != string.Empty && databaseName != string.Empty && userId != string.Empty && passWord != string.Empty)
            {
                return string.Format("Data Source={0}; Initial Catalog={1};User Id={2};Password={3};", serverName, databaseName, userId, passWord);
            }

            return null;
        }

        /// <summary>
        /// Gets the server connection string.
        /// </summary>
        /// <param name="serverName">Name of the server.</param>
        /// <param name="databaseName">Name of the database.</param>
        /// <returns>connection string</returns>
        public static string GetServerConnectionString(string serverName, string databaseName)
        {
            if (serverName != string.Empty && databaseName != string.Empty)
            {
                return string.Format("Data Source={0}; Initial Catalog={1};", serverName, databaseName);
            }

            return null;
        }
    }
}
