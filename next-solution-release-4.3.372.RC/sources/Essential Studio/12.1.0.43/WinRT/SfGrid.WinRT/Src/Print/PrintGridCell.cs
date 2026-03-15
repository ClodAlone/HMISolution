#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
#if WinRT
using Windows.UI.Xaml.Controls;
#else
using System.Windows.Controls;
#endif


namespace Syncfusion.UI.Xaml.Grid
{
    public class PrintGridCell : ContentControl, IDisposable
    {

        public PrintGridCell()
        {
            DefaultStyleKey = typeof(PrintGridCell);
        }

        #region Dispose

        public void Dispose()
        {
            
        }

        #endregion
                
    }

    public class PrintHeaderCell : PrintGridCell
    {
        #region Ctor

        public PrintHeaderCell()
        {
            DefaultStyleKey = typeof(PrintHeaderCell);
        }

        #endregion
    }

    public class PrintCaptionSummaryCell : PrintGridCell
    {
        #region Ctor

        public PrintCaptionSummaryCell()
        {
            DefaultStyleKey = typeof(PrintCaptionSummaryCell);
        }

        #endregion
    }

    public class PrintGroupSummaryCell : PrintGridCell
    {
        #region Ctor

        public PrintGroupSummaryCell()
        {
            DefaultStyleKey = typeof(PrintGroupSummaryCell);
        }

        #endregion
    }

    public class PrintTableSummaryCell : PrintGridCell
    {
        #region Ctor

        public PrintTableSummaryCell()
        {
            DefaultStyleKey = typeof(PrintTableSummaryCell);
        }

        #endregion
    }
}
