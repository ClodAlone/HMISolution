#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.UI.Xaml.ScrollAxis;
using System.Collections.Generic;
#if WinRT
using Windows.Foundation;
using Windows.UI.Xaml;
#else
using System.Windows;
#endif


namespace Syncfusion.UI.Xaml.Grid
{
    [ClassReference(IsReviewed = false)]
    public interface IRowGenerator
    {
        IList<IRowElement> Items { get; }
        void ApplyColumnSizeronInitial(double availableWidth);
        void PregenerateRows(VisibleLinesCollection visibleRows, VisibleLinesCollection visibleColumns);
        void EnsureRows(VisibleLinesCollection visibleRows);
        void EnsureColumns(VisibleLinesCollection visibleColumns);
        void OnItemSourceChanged(DependencyPropertyChangedEventArgs args);
        void ColumnHiddenChanged(HiddenRangeChangedEventArgs args);
        void RowHiddenChanged(HiddenRangeChangedEventArgs args);
        void ColumnInserted(int index,int count);
        void ColumnRemoved(int index, int count);
        void RowsArranged(Size finalSize);
        void ApplyFixedRowVisualState(int index, bool canapply);
        void LineSizeChanged();
    }
}
