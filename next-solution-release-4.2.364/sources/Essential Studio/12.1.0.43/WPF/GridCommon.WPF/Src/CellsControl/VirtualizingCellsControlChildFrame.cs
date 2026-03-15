#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.Windows.Controls.Scroll;
using System.Windows;
using Syncfusion.Windows.GridCommon;
using System.Diagnostics;
using System;
using System.Windows.Media;

namespace Syncfusion.Windows.Controls.Cells
{

    /// <summary>
    /// Implements a child frame in a <see cref="VirtualizingCellsControl"/> that can be placed at the top, bottom,
    /// left and right side of the control so that contents do scroll similiar to the Internet Explorer
    /// frames concept. Each frame remembers its placement.<para/>
    /// Adding and removing elements from the children collection does not trigger
    /// calls to InvalidateMeasure. This allows adding and removing elements
    /// on the fly. A derived control is responsible to call Measure and Arrange
    /// on child elements since this base class will not do this by itsself.
    /// </summary>
    public class VirtualizingCellsControlChildFrame : ScrollControlChildFrame
    {
        InternalChildFrameRenderedCellsManager renderedCells;

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualizingCellsControlChildFrame"/> class.
        /// </summary>
        public VirtualizingCellsControlChildFrame()
        {
            IsPrepareRenderCellIntialized = false;
            renderedCells = new InternalChildFrameRenderedCellsManager(this.Children);
        }

        internal InternalChildFrameRenderedCellsManager RenderedCells
        {
            get { return renderedCells; }
        }


        internal bool IsPrepareRenderCellIntialized
        {
            get;
            set;
        }

        internal DrawingContext DrawingContext { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String"/> with state information about the object.
        /// </summary>
        /// <returns>
        /// Returns a <see cref="System.String"/> with state information about the object.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0}: Row {1} Column {2} RenderedCells {3}/{4}",
                GetType().Name,
                RowSection,
                ColumnSection,
                renderedCells.aliveCellDrawingVisuals.Count,
                renderedCells.unloadCellDrawingVisuals != null ?
                    renderedCells.unloadCellDrawingVisuals.Count.ToString()
                    : "null");
        }

        //protected override Size MeasureOverride(Size constraint)
        //{
        //    //base.MeasureOverride(constraint);
        //    Rect rect = Rect.Empty;
        //    foreach (var visual in Children)
        //    {
        //        if (!(visual is NoHitTestDrawingVisual))
        //        {                    
        //            var virtualcell = VirtualizingCellsControl.GetArrangeCellArgs(visual);
        //            rect = VisualContainer.GetRenderBounds(visual);
        //            if (rect != Rect.Empty)
        //            {                        
        //                if (virtualcell != null)
        //                {
        //                    if (virtualcell.CellUIElements.IsDirty )//&& !(virtualcell.CellUIElements.IsMeasureLoadedFirstTime))
        //                    {                                
        //                        var ui = VirtualizingCellsControl.GetCellUIElement(visual);
        //                        ui.Measure(GridUtil.GetSize(rect));
        //                    }
        //                    else if (virtualcell.CellUIElements.IsMeasureLoadedFirstTime)
        //                    {                                
        //                        var ui = VirtualizingCellsControl.GetCellUIElement(visual);
        //                        ui.Measure(GridUtil.GetSize(rect));
        //                        virtualcell.CellUIElements.IsMeasureLoadedFirstTime = false;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return constraint;
        //}

        //protected override Size ArrangeOverride(Size finalSize)
        //{
        //    //base.ArrangeOverride(finalSize);
        //    Rect rect = Rect.Empty;
        //    foreach (var visual in Children)
        //    {
        //        if (!(visual is NoHitTestDrawingVisual))
        //        {
        //            var virtualcell = VirtualizingCellsControl.GetArrangeCellArgs(visual);
        //            rect = VisualContainer.GetRenderBounds(visual);
        //            if (rect != Rect.Empty && virtualcell != null)
        //            {                        
        //                var ui = VirtualizingCellsControl.GetCellUIElement(visual);
        //                ui.Arrange(rect);
        //            }                    
        //        }
        //    }
        //    return finalSize;
        //}
    }
}
