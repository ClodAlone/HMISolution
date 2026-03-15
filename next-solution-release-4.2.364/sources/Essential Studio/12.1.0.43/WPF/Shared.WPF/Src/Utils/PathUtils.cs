// <copyright file="PathUtils.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Data;
using System.ComponentModel;
using System.Globalization;

namespace Syncfusion.Windows.Shared.Utils
{
    /// <summary>
    /// Class to get data using path.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public static class DataUtils
    {
        #region Implementation
        /// <summary>
        /// Gets the object by path.
        /// </summary>
        /// <param name="obj">The object value.</param>
        /// <param name="path">The path value.</param>
        /// <returns>Xml element attribute.</returns>
        public static object GetObjectByPath(object obj, string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                if (obj is XmlElement)
                {
                    obj = ((XmlElement)obj).GetAttribute(path);
                }
                else if (obj is DataRow)
                {
                    obj = ((DataRow)obj)[path];
                }
                else
                {
                    obj = TypeDescriptor.GetProperties(obj)[path].GetValue(obj);
                }
            }

            return obj;
        }

        /// <summary>
        /// Converts to double.
        /// </summary>
        /// <param name="obj">The object value.</param>
        /// <returns>double value date time</returns>
        public static double ConvertToDouble(object obj)
        {
            double value = 0;

            try
            {
                if (obj is DateTime)
                {
                    value = ((DateTime)obj).ToOADate();
                }
                else
                {
                    value = Convert.ToDouble(obj, CultureInfo.InvariantCulture);
                }
            }
            catch
            {
                value = double.NaN;
            }

            return value;
        }

        /// <summary>
        /// Gets the double by path.
        /// </summary>
        /// <param name="obj">The object value.</param>
        /// <param name="path">The path value.</param>
        /// <returns>double value from path</returns>
        public static double GetDoubleByPath(object obj, string path)
        {
            return ConvertToDouble(GetObjectByPath(obj, path));
        }
        #endregion
    }
}
