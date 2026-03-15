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
using System.Windows.Input;
using System.Windows;

namespace Syncfusion.Windows.Controls.Grid
{
    public interface IGraphicSelectionController
    {

        void OnMouseDown(MouseButtonEventArgs args, GraphicCellSpanInfo spanInfo);

        void OnMouseUp(MouseButtonEventArgs args, GraphicCellSpanInfo spanInfo);

        void OnMouseLeave(MouseEventArgs args, GraphicCellSpanInfo spanInfo);

        void OnMouseEnter(MouseEventArgs args, GraphicCellSpanInfo spanInfo);

        void OnMouseMove(MouseEventArgs args, GraphicCellSpanInfo spanInfo);

        void OnKeyDown(KeyEventArgs args, GraphicCellSpanInfo spanInfo);

        void OnKeyUp(KeyEventArgs args, GraphicCellSpanInfo spanInfo);

        void OnGotFocus(RoutedEventArgs args, GraphicCellSpanInfo spanInfo);

        void OnLostFocus(RoutedEventArgs args, GraphicCellSpanInfo spanInfo);

        void SelectGraphicCell(GraphicCellSpanInfo spanInfo);

        void UnSelectGraphicCell(GraphicCellSpanInfo spanInfo);

        void SelectAllGraphicCells();

        void ClearGraphicCellSelections();

    }
}
