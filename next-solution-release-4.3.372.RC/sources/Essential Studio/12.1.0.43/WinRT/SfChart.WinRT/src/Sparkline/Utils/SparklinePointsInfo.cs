#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Windows;

#if !WINDOWS_PHONE
using Windows.Foundation;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    public class SparklinePointsInfo
    {
        #region ctor

        public SparklinePointsInfo()
        {

        }

        #endregion

        #region properties

        private Point coordinate;

        public Point Coordinate
        {
            get { return coordinate; }
            set { coordinate = value; }
        }
        private Point values;

        public Point Value
        {
            get { return values; }
            set { values = value; }
        }

        #endregion
    }
}
