#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Syncfusion.Windows.Controls.Gantt.Chart
{
    /// <summary>
    /// Represents a panel that will present the Gantt Chart Row Items.
    /// </summary>
    public class GanttChartRowItemsPresenter : Panel
    {
        #region Properties
        internal GanttChartRow ParentRow { get; set; }

        #endregion

        #region Constructors and overrides

        public GanttChartRowItemsPresenter()
        {
#if !SyncfusionFramework3_5
            UseLayoutRounding = false;
#endif
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double nodewidth = 0d;
            double nodeX = 0d;
            double nodeHeight = 0d;

            foreach (var t in Children)
            {
                if (t is GanttNode || t is MileStone || t is HeaderNode)
                {
                    var node = (GanttNode)t;

                    if (this.ParentRow.ParentControl.ParentControl.ScheduleType != ScheduleType.CustomNumeric)
                    {
                        // This is to avoid rendering the non schedule task, by changing the date and time node will get rendered
                        if (node.StartTime.Equals(DateTime.MinValue) || node.StartTime.Equals(DateTime.MaxValue) ||
                            node.EndTime.Equals(DateTime.MinValue) || node.EndTime.Equals(DateTime.MaxValue))
                            continue;

                        // Calucation of X1 and X2 should be here moving this to time change call back of node or some where else into the node class will 
                        // create the issue on updating the date and time on resizing/drag and droping the node.
                        node.X1 = this.ParentRow.ParentControl.GetStartPositionOfDate(node.StartTime);
                        node.X2 = this.ParentRow.ParentControl.GetEndPositionOfDate(node.EndTime);
                    }
                    else
                    {
                        node.X1 = this.ParentRow.ParentControl.GetPositionOfPoint(node.StartPoint);
                        node.X2 = this.ParentRow.ParentControl.GetPositionOfPoint(node.EndPoint);
                    }

                    double width  = (node.X2 - node.X1);
                    node.Width = width;

                    if (node is MileStone)
                    {
                        node.Arrange(new Rect(node.X1 - 8, 0, node.DesiredSize.Width, ParentRow.Height));

                        // To render the resource text box based on the width
                        width = node.DesiredSize.Width;
                    }
                    else
                        node.Arrange(new Rect(node.X1, 0, width, ParentRow.Height));
                    
                    nodewidth = width;
                    nodeHeight = node is MileStone ? node.DesiredSize.Height-3 : node.DesiredSize.Height;
                    nodeX = node.X1;
                }
                else if (t is ContentControl)
                {
                    var tb = (ContentControl)t;
                    var parentwidth = ParentRow.ActualWidth - nodeX;
                    tb.Arrange(nodewidth > 0
                        ? GetRect(nodeX,nodewidth,nodeHeight,tb)
                                   : new Rect(0, 0, 0, ParentRow.MinHeight));
                    nodeX = 0;
                    nodewidth = 0;
                }
            }

            return base.ArrangeOverride(finalSize);
        }
        
        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (var t in Children)
            {
                if (t is GanttNode || t is MileStone || t is HeaderNode)
                {
                    var gi = (GanttNode)t;
                    gi.Measure(new Size(availableSize.Width, ParentRow.MinHeight));
                }
                else if (t is ContentControl)
                {
                    var tb = (ContentControl)t;
                    tb.Measure(new Size(ParentRow.MaxWidth, ParentRow.MinHeight));
                }
            }

            return base.MeasureOverride(availableSize);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Gets the rect.
        /// </summary>
        /// <param name="xPos">The x pos.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="resourceContatiner">The resource contatiner.</param>
        /// <returns></returns>
        private Rect GetRect(double xPos, double width,double height, ContentControl resourceContatiner)
        {
            double x = 0d, y = 0d;
            switch (this.ParentRow.ParentControl.ResourceNamePlacement)
            {
                case PlacementMode.Left:
                    x = xPos - resourceContatiner.DesiredSize.Width - 15;
                    break;
                case PlacementMode.Top:
                    x = xPos;
                    y = -(height+2);
                    break;
                case PlacementMode.Bottom:
                    x = xPos;
                    y = height + 2;
                    break;
                default:
                    x = width + xPos + 15;
                    break;
            }
            return new Rect(x, y, resourceContatiner.DesiredSize.Width, this.ParentRow.Height);
        }

        #endregion
    }

}