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
using System.Windows;
using Syncfusion.Windows.Controls.Cells;
using System.Windows.Input;

namespace Syncfusion.Windows.Controls.Grid
{
    public interface IGraphicCellRenderer
    {
        GraphicCellModelBase CellModel { get; }
        GridControlBase GridControl { get; set; }
        bool IsEditable { get; }
        UIElement CurrentUIElement { get; }
        void RaiseCreated(GraphicCellModelBase cellModel);
        bool ShouldTryToHandlePreviewKeyDown(KeyEventArgs e);
        void Arrange(UIElement uiElements, Rect cellRect, GraphicStyleInfo style);
        UIElement PrepareUIElements(GraphicStyleInfo cellInfo, GraphicCellSpanInfo cellSpanInfo);
        void SetBounds(UIElement el, Rect rect, bool forceMeasure, bool forceArrange);
        void UnloadUIElements(int index, GraphicCellUIElement uiElement);
    }
}
