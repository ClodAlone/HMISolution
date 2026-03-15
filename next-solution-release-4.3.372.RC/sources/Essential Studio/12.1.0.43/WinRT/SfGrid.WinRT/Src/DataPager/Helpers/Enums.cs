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

namespace Syncfusion.UI.Xaml.Controls.DataPager
{
    [Flags]
    public enum PageDisplayMode
    {
        None = 0,
        First = 1,
        Last = 2,
        Previous = 4,
        Next = 8,
        Numeric = 16,
        FirstLast = First | Last,
        PreviousNext = Previous | Next,
        FirstLastNumeric = FirstLast | Numeric,
        PreviousNextNumeric = PreviousNext | Numeric,
        FirstLastPreviousNext = FirstLast | PreviousNext,
        FirstLastPreviousNextNumeric = FirstLast | Numeric | PreviousNext,
    }

    public enum AutoEllipsisMode
    {
        None,
        Before,
        After,
        Both,
    }

    public enum ElipsisPosition
    {
        None = 0,
        Left = -1,
        Right = 1
    }
}