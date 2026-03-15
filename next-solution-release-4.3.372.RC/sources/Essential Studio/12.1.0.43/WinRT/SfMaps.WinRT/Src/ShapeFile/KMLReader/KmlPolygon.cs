#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections.Generic;
#if WINRT
using Windows.Foundation;
#else
using System.Windows;
#endif

namespace Syncfusion.UI.Xaml.Maps
{
    #region KmlPolygon

    internal class KmlPolygon
    {
        internal KmlBoundary OuterBoundary { get; set; }
        internal List<KmlBoundary> InnerBoundaryList { get; set; }
        internal KmlPlacemark Placemark { get; set; }
    } 

    #endregion

    #region KmlPoint

    internal class KmlPoint
    {
        internal Point Point { get; set; }
        internal KmlPlacemark Placemark { get; set; }
    } 

    #endregion
}
