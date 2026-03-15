#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion



using Syncfusion.Windows.Controls.Cells;
namespace Syncfusion.Windows.Controls.VirtualTreeView
{
    /// <summary>
    /// The <see cref="TreeCellRenderer"/> class provides a default implementation of 
    /// the <see cref="ICellRenderer"/> interface for a cell renderer in a 
    /// <see cref="VirtualTreeView"/>.<para/>
    /// You should derive from this class to implement custom cell renderer classes. 
    /// <para/>
    /// If you want to implement a renderer with support for live UIElement visuals 
    /// inside the cell you should derive from the generic <see cref="TreeVirtualizingCellRenderer{T}"/>
    /// class.
    /// </summary>
    /// <exclude/>
    public class TreeCellRenderer : CellRendererBase<TreeRenderStyleInfo>
    {
    }
}
