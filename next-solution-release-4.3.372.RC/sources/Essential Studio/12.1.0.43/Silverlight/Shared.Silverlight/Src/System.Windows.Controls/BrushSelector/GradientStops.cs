#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Tools.Controls
{
    using System;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    /// <summary>
    /// Represents the Gradient Stops Class.
    /// </summary>
    internal class GradientStops
    {
        private BrushEdit cedit;
        internal Color color { get; set; }
        private bool enable;
        internal Canvas gradientitem { get; set; }
        internal bool isselected { get; set; }
        internal double offset { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.Windows.Tools.Controls.GradientStops">GradientStops</see> class
        /// </summary>
        public GradientStops(Color col, bool sel, double off)
        {
            gradientitem = new Canvas();
            color = col;
            offset = off;
            isselected = sel;
            createitem();
        }

        /// <summary>
        /// Invokes this method to create a gradient stop item.
        /// </summary>
        private void createitem()
        {
            cedit = new BrushEdit();
            Geometry path = new PathGeometry();
            PathFigure figure1 = new PathFigure();
            figure1.StartPoint = new Point(0, 0);
            LineSegment line = new LineSegment();
            line.Point = new Point(0, 0);
            figure1.Segments.Add(line);
            line = new LineSegment();
            line.Point = new Point(-5, 5);
            figure1.Segments.Add(line);
            line = new LineSegment();
            line.Point = new Point(5, 5);
            figure1.Segments.Add(line);
            line = new LineSegment();
            line.Point = new Point(0, 0);
            figure1.Segments.Add(line);
            (path as PathGeometry).Figures.Add(figure1);
            Path pt = new Path();
            pt.SetValue(Path.DataProperty, path);
            pt.Stroke = cedit.SelectedBackground;
            pt.Fill = new SolidColorBrush(this.color);
            gradientitem.Children.Add(pt);
            gradientitem.MouseLeftButtonDown += new MouseButtonEventHandler(gradientitem_MouseLeftButtonDown);
            gradientitem.MouseMove += new MouseEventHandler(gradientitem_MouseMove);
            gradientitem.MouseLeftButtonUp += new MouseButtonEventHandler(gradientitem_MouseLeftButtonUp);
        }

        /// <summary>
        /// Method to find the parent of the element.
        /// </summary>
        /// <param name="element">FrameworkElement whose parent is to be find out</param>
        /// <returns>BrushEdit which is the parent of the FrameworkElement </returns>
        public static BrushEdit GetBrushEditParentFromChildren(FrameworkElement element)
        {
            BrushEdit item = null;
            if (element != null)
            {
                item = element as BrushEdit;
                if (item == null)
                {
                    while (element != null)
                    {
                        element = VisualTreeHelper.GetParent(element) as FrameworkElement;

                        if (element is BrushEdit)
                        {
                            item = (BrushEdit)element;
                            break;
                        }
                    }
                }
            }

            return item;
        }

        /// <summary>
        /// Handles the MouseLeftButtonDown event of the gradientitem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void gradientitem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(null);
            ((UIElement)sender).CaptureMouse();
            cedit = GetBrushEditParentFromChildren(this.gradientitem);
            cedit.clearSelection();
            cedit.items.SelectedItem = this;
            cedit.CurrentColor.Fill = cedit.SelectedColor.Fill;
            cedit.ApplyGradient(this);
            this.isselected = true;
            ((Path)this.gradientitem.Children[0]).Stroke = cedit.SelectedBackground;
            this.gradientitem.SetValue(Canvas.ZIndexProperty, 1);
            enable = true;
        }

        /// <summary>
        /// Handles the MouseLeftButtonUp event of the gradientitem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void gradientitem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            enable = false;
            cedit = GetBrushEditParentFromChildren(this.gradientitem);
            Canvas cd = (Canvas)gradientitem.Parent;
            Point p = e.GetPosition(cd);
            Point pt = e.GetPosition(cedit);
            if (p.X < cd.ActualWidth && p.X > 0.0)
            {
                if (pt.Y > cedit.ActualHeight && cedit.items.Items.Count > 2)
                {
                    cedit.gradientBar.Children.Remove(cedit.items.SelectedItem.gradientitem);
                    int selindex = cedit.items.Items.IndexOf(this);
                    if (selindex > 0)
                    {
                        selindex--;
                    }

                    cedit.items.SelectedItem = (GradientStops)cedit.items.Items[selindex];
                    cedit.items.Items.Remove(this);
                    cedit.enableSelectedBrush = true;
                    cedit.ApplyGradient(cedit.items.SelectedItem);
                    cedit.enableSelectedBrush = false;
                }
                else
                {
                    gradientitem.SetValue(Canvas.LeftProperty, p.X);
                    this.offset = p.X / cd.ActualWidth;
                    cedit.ApplyGradient(this);
                }
            }
        }

        /// <summary>
        /// Handles the MouseMove event of the gradientitem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void gradientitem_MouseMove(object sender, MouseEventArgs e)
        {
            if (enable)
            {
                cedit = GetBrushEditParentFromChildren(this.gradientitem);
                Canvas cd = (Canvas)gradientitem.Parent;
                Point p = e.GetPosition(cd);
                Point pt = e.GetPosition(null);
                if (p.X < cd.ActualWidth && p.X > 0.0)
                {
                    this.gradientitem.SetValue(Canvas.LeftProperty, p.X);
                    this.offset = p.X / cd.ActualWidth;
                    cedit.enableSelectedBrush = true;
                    cedit.ApplyGradient(this);
                    cedit.enableSelectedBrush = false;
                }
            }
        }
    }
}
