#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Diagram
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using System.Windows.Media;

    /// <summary>
    /// Represents Resize helper class which helps in the resizing operation.
    /// </summary>
    internal class HandleRenderer
    {
        /// <summary>
        /// Used to store the connector type.
        /// </summary>
        private static ConnectorType mconnectionType = ConnectorType.Orthogonal;

        /// <summary>
        /// Gets or sets the type of connection to be used.
        /// </summary>
        /// <value>
        /// Type: <see cref="ConnectorType"/>
        /// Enum specifying the type of the connector to be used.
        /// </value>
        /// <example>
        /// <para/>This example shows how to set ConnectorType in C#.
        /// <code language="C#">
        ///  connObject.ConnectorType = ConnectorType.Orthogonal;
        /// </code>
        /// </example>
        internal static ConnectorType ConnectorType
        {
            get
            {
                return mconnectionType;
            }

            set
            {
                if (value != mconnectionType)
                {
                    mconnectionType = value;
                }
            }
        }

        /// <summary>
        /// Computes the drag boundaries.
        /// </summary>
        /// <param name="gnode">The group node.</param>
        /// <param name="minoffx">The minoffx.</param>
        /// <param name="minoffy">The minoffy.</param>
        /// <param name="minimumHorDelta">The minimum hor delta.</param>
        /// <param name="minimumVertDelta">The minimum vert delta.</param>
        internal static void ComputeDragBoundaries(Group gnode, out double minoffx, out double minoffy, out double minimumHorDelta, out double minimumVertDelta)
        {
            minoffx = double.MaxValue;
            minoffy = double.MaxValue;
            minimumHorDelta = double.MaxValue;
            minimumVertDelta = double.MaxValue;
            CollectionExt gnodechildren = new CollectionExt();
            for (int i = 0; i < gnode.NodeChildren.Count; i++)
            {
                gnodechildren.Add(gnode.NodeChildren[i] as Node);
            }

            gnodechildren.Add(gnode);
            foreach (Node item in gnodechildren)
            {
                double offx = item.PxOffsetX;
                double offy = item.PxOffsetY;
                minoffx = double.IsNaN(offx) ? 0 : Math.Min(offx, minoffx);
                minoffy = double.IsNaN(offy) ? 0 : Math.Min(offy, minoffy);
                minimumVertDelta = Math.Min(minimumVertDelta, item.ActualHeight - item.MinHeight);
                minimumHorDelta = Math.Min(minimumHorDelta, item.ActualWidth - item.MinWidth);
            }
        }

        /// <summary>
        /// Calculates the drag limits.
        /// </summary>
        /// <param name="selectedItems">IEnumerable Collection instance.</param>
        /// <param name="minoffx">The minoffx.</param>
        /// <param name="minoffy">The minoffy.</param>
        /// <param name="minimumHorDelta">The minimum horizontal delta.</param>
        /// <param name="minimumVertDelta">The minimum vertical delta.</param>
        internal static void ComputeDragBoundaries(IEnumerable<Node> selectedItems, out double minoffx, out double minoffy, out double minimumHorDelta, out double minimumVertDelta)
        {
            minoffx = double.MaxValue;
            minoffy = double.MaxValue;
            minimumHorDelta = double.MaxValue;
            minimumVertDelta = double.MaxValue;

            foreach (Node item in selectedItems)
            {
                Rect noderect=item.TransformToVisual((item.Page as DiagramPage).dview.ScrollGrid).TransformBounds(new Rect());//Math.Abs(item.PxOffsetX);
                double offx = noderect.Left;
                double offy = noderect.Top;

                minoffx = double.IsNaN(offx) ? 0 : Math.Min(offx, minoffx);
                minoffy = double.IsNaN(offy) ? 0 : Math.Min(offy, minoffy);
                minimumVertDelta = Math.Min(minimumVertDelta, item.ActualHeight - item.MinHeight);
                minimumHorDelta = Math.Min(minimumHorDelta, item.ActualWidth - item.MinWidth);
            }
        }

        /// <summary>
        /// Resizes the bottom side of the node.
        /// </summary>
        /// <param name="scale">Scale factor.</param>
        /// <param name="item">Node instance.</param>
        /// <param name="designer">IDiagramPage instance.</param>
        /// <param name="munits">The measure units..</param>
        internal static void ResizeBottom(double scale, Node item, IDiagramPage designer, MeasureUnits munits)
        {
            IEnumerable<Node> items = designer.SelectionList.GetGroupList(item).Cast<Node>();
            double top = item.PxOffsetY;
            double tempTop = item.PxOffsetY;
            double delta = (tempTop - top) * (scale - 1);
            item.PxOffsetY = tempTop + delta;
            item.m_TempSize.Height = item.m_TempSize.Height * scale;
            item.SnapHeight(false);

            //item.PxOffsetY = tempTop + delta;
            //item.Height = item.Height * scale;
            if(item is Group)
                foreach (Node n in (item as Group).NodeChildren.OfType<Node>())
                {
                    top = n.PxOffsetY;
                    tempTop = n.PxOffsetY;
                    delta = (tempTop - top) * (scale - 1);
                    n.PxOffsetY = tempTop + delta;
                    //n.OffsetY = tempTop + delta;
                    n.m_TempSize.Height = n.m_TempSize.Height * scale;
                    n.SnapHeight(false);
                }
        }

        /// <summary>
        /// Resizes the left side of the node.
        /// </summary>
        /// <param name="scale">Scale factor.</param>
        /// <param name="item">Node instance.</param>
        /// <param name="designer">IDiagramPage instance.</param>
        /// <param name="munits">The measure units.</param>
        internal static void ResizeLeft(double scale, Node item, IDiagramPage designer, MeasureUnits munits)
        {
            double left = item.m_TempPosition.X + item.m_TempSize.Width;
            double tempLeft = item.m_TempPosition.X;
            double delta = (left - tempLeft) * (scale - 1);
            item.m_TempPosition.X = tempLeft - delta;
            item.m_TempSize.Width = item.m_TempSize.Width * scale;
            item.SnapWidth(true);
            if (item is Group)
                foreach (Node n in (item as Group).NodeChildren.OfType<Node>())
                {
                    left = n.m_TempPosition.X + n.m_TempSize.Width;
                    tempLeft = n.m_TempPosition.X;
                    delta = (left - tempLeft) * (scale - 1);
                    n.m_TempPosition.X = tempLeft - delta;
                    //n.OffsetX = tempLeft - delta;
                    n.m_TempSize.Width = n.m_TempSize.Width * scale;
                    n.SnapWidth(true);
                }
        }

        /// <summary>
        /// Resizes the right side of the node.
        /// </summary>
        /// <param name="scale">Scale factor.</param>
        /// <param name="item">Node instance.</param>
        /// <param name="designer">IDiagramPage instance.</param>
        /// <param name="munits">The measure units..</param>
        internal static void ResizeRight(double scale, Node item, IDiagramPage designer, MeasureUnits munits)
        {
            IEnumerable<Node> items = designer.SelectionList.GetGroupList(item).Cast<Node>();
            double left = item.PxOffsetX;
            double tempLeft = item.PxOffsetX;
            double delta = (tempLeft - left) * (scale - 1);
            item.PxOffsetX = tempLeft + delta;
            item.StartPointDragging = new Point(item.PxOffsetX, item.PxOffsetY);
            item.m_TempSize.Width = item.m_TempSize.Width * scale;
            item.SnapWidth(false);
            if (item is Group)
                foreach (Node n in (item as Group).NodeChildren.OfType<Node>())
                {
                    left = n.PxOffsetX;
                    tempLeft = n.PxOffsetX;
                    delta = (tempLeft - left) * (scale - 1);
                    n.PxOffsetX = tempLeft + delta;
                    //n.OffsetX = tempLeft + delta;
                    n.m_TempSize.Width = n.m_TempSize.Width * scale;
                    n.SnapWidth(false);
                }
        }

        /// <summary>
        /// Resizes the top side of the node.
        /// </summary>
        /// <param name="scale">Scale factor.</param>
        /// <param name="item">Node instance.</param>
        /// <param name="designer">IDiagramPage instance.</param>
        /// <param name="munits">The measure units..</param>
        internal static void ResizeTop(double scale, Node item, IDiagramPage designer, MeasureUnits munits)
        {
           
            double top = item.m_TempPosition.Y + item.m_TempSize.Height;
            double tempTop = item.m_TempPosition.Y;
            double delta = (top - tempTop) * (scale - 1);
            item.m_TempPosition.Y = tempTop - delta;
            item.m_TempSize.Height = item.m_TempSize.Height * scale;
            item.SnapHeight(true);
            if (item is Group)
                foreach (Node n in (item as Group).NodeChildren.OfType<Node>())
                {
                    top = n.m_TempPosition.Y + n.m_TempSize.Height;
                    tempTop = n.m_TempPosition.Y;
                    delta = (top - tempTop) * (scale - 1);
                    n.m_TempPosition.Y = tempTop - delta;
                    //n.OffsetX = tempLeft - delta;
                    n.m_TempSize.Height = n.m_TempSize.Height * scale;
                    n.SnapHeight(true);
                }
        }

        #region Class fields

        #endregion

        #region Class properties

        #endregion

        #region Implementation
        
        #endregion

        internal  static readonly DependencyProperty ResizerVerticalAlignmentProperty = DependencyProperty.RegisterAttached("ResizerVerticalAlignment", typeof(VerticalAlignment), typeof(Thumb), null);
        internal  static readonly DependencyProperty ResizerHorizontalAlignmentProperty = DependencyProperty.RegisterAttached("ResizerHorizontalAlignment", typeof(HorizontalAlignment), typeof(Thumb), null);


        internal static void SetVerticalProperty(DependencyObject obj, VerticalAlignment valign)
        {
            obj.SetValue(ResizerVerticalAlignmentProperty, valign);
        }

        internal static VerticalAlignment GetVerticalProperty(DependencyObject obj)
        {
            return (VerticalAlignment)obj.GetValue(ResizerVerticalAlignmentProperty);
        }


        internal static void SetHorizontalProperty(DependencyObject obj, HorizontalAlignment halign)
        {
            obj.SetValue(ResizerHorizontalAlignmentProperty, halign);
        }

        internal static HorizontalAlignment GetHorizontalProperty(DependencyObject obj)
        {
            return (HorizontalAlignment)obj.GetValue(ResizerHorizontalAlignmentProperty);
        }
    }
}
