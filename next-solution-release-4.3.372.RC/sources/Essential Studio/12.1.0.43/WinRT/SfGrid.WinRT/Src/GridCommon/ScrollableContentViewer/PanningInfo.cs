#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if WinRT
using Windows.Foundation;
#else
using System.Windows;
#endif
using Syncfusion.UI.Xaml.Grid;

namespace Syncfusion.UI.Xaml.Grid
{
    [ClassReference(IsReviewed = false)]
    public class PanningInfo
    {
        public double DeltaPerHorizontalOffet { get; set; }

        public double DeltaPerVerticalOffset { get; set; }

        public bool IsPanning { get; set; }

        public double OriginalHorizontalOffset { get; set; }

        public double OriginalVerticalOffset { get; set; }

        public Point UnusedTranslation { get; set; }

        public bool InHorizontalFeedback { get; set; }

        public bool InVerticalFeedback { get; set; }
    }
}
