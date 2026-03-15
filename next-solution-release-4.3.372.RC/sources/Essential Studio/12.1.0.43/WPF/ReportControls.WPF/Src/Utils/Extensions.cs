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
using System.Collections;
using System.Diagnostics;
using Syncfusion.RDL.DOM;

#if !WINRT
using Syncfusion.Linq;
using Syncfusion.Windows.Chart;
#endif

namespace Syncfusion.RDL.Internal
{
#if MVC
    internal static class PixelUtil
    {
        public static int GetPixel(this double value)
        {
            int intValue = (int)value;
            return intValue;
        }

        public static string GetPixelString(this double value)
        {
            int intValue = (int)value;
            return intValue.ToString() + "px";
        }

        public static string GetIntPixelString(this int value)
        {
            return value.ToString() + "px";
        }
    }
#endif
}
