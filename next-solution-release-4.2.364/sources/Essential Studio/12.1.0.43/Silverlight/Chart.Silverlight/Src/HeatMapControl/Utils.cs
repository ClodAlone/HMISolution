#region Copyright Syncfusion Inc. 2001 - 2014
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
using System.Xml;
using System.ComponentModel;
using System.Globalization;
namespace Syncfusion.Windows.Chart
{
    internal static class DataUtils
    {
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
    }
}