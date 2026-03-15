#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// Class which has the Gradient Stop
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif

    internal class GradientStopItem : Control
    {
        /// <summary>
        /// Member which has the color edit
        /// </summary>
        internal ColorEdit cedit;

        /// <summary>
        /// Dependency property which has color
        /// </summary>
        public static readonly DependencyProperty ColorProperty =
           DependencyProperty.Register("color", typeof(Color), typeof(GradientStopItem), new FrameworkPropertyMetadata(Colors.White, FrameworkPropertyMetadataOptions.AffectsArrange));

        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        /// <value>The color.</value>
        public Color color
        {
            get
            {
                return (Color)GetValue(ColorProperty);
            }

            set
            {
                SetValue(ColorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the gradientitem.
        /// </summary>
        /// <value>The gradientitem.</value>
        internal Canvas gradientitem { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="GradientStopItem"/> is isselected.
        /// </summary>
        /// <value><c>true</c> if isselected; otherwise, <c>false</c>.</value>
        internal bool isselected { get; set; }

        /// <summary>
        /// Gets or sets the offset.
        /// </summary>
        /// <value>The offset.</value>
        internal double offset { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is enabled.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is enabled; otherwise, <c>false</c>.
        /// </value>
        internal bool isEnabled { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GradientStopItem"/> class.
        /// </summary>
        /// <param name="col">The col.</param>
        /// <param name="sel">if set to <c>true</c> [sel].</param>
        /// <param name="off">The off.</param>
        /// <param name="edit">The edit.</param>
        public GradientStopItem(Color col, bool sel, double off, ColorEdit edit)
        {
            gradientitem = new Canvas();
            color = col;
            offset = off;
            cedit = edit;
            isselected = sel;
            createitem();
        }

        /// <summary>
        /// Createitems this instance.
        /// </summary>
        private void createitem()
        {
            //cedit = new ColorEdit();
            Geometry path = new PathGeometry();

            PathFigure figure1 = new PathFigure();
            figure1.StartPoint = new Point(0, 0);
            LineSegment line = new LineSegment();
            line.Point = new Point(0, 0);
            figure1.Segments.Add(line);
            line = new LineSegment();
            line.Point = new Point(-6, 6);
            figure1.Segments.Add(line);
            line = new LineSegment();
            line.Point = new Point(6, 6);
            figure1.Segments.Add(line);
            line = new LineSegment();
            line.Point = new Point(0, 0);
            figure1.Segments.Add(line);
            (path as PathGeometry).Figures.Add(figure1);
            Path pt = new Path();
            Path pt1 = new Path();
            pt.SetValue(Path.DataProperty, path);
            //pt.Stroke =new SolidColorBrush(Colors.BlueViolet);
            //pt.Fill = new SolidColorBrush(this.color);
            Thumb th = new Thumb();
            th.Height = 15;
            th.Width = 7;
            pt1.Height = 17;
            pt1.Width = 17;
            //pt1.Style = Application.Current.Resources["petal"] as Style;
            //cedit = GetBrushEditParentFromChildren(this.gradientitem);
            //pt.Style = cedit.p.Style;
            Control pt2 = new Control();

            pt2.Template = cedit.ThumbTemplate;
            gradientitem.Children.Add(pt2);

            //  th.MouseLeftButtonDown += new MouseButtonEventHandler(th_MouseLeftButtonDown);
            //  th.MouseMove += new MouseEventHandler(gradientitem_MouseMove);
            th.MouseLeftButtonDown += new MouseButtonEventHandler(gradientitem_MouseLeftButtonDown);
            gradientitem.MouseLeftButtonDown += new MouseButtonEventHandler(gradientitem_MouseLeftButtonDown);
            gradientitem.MouseMove += new MouseEventHandler(gradientitem_MouseMove);

            gradientitem.MouseLeftButtonUp += new MouseButtonEventHandler(gradientitem_MouseLeftButtonUp);
        }

        /// <summary>
        /// Gets the brush edit parent from children.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public static ColorEdit GetBrushEditParentFromChildren(FrameworkElement element)
        {
            ColorEdit item = null;
            if (element != null)
            {
                item = element as ColorEdit;
                if (item == null)
                {
                    while (element != null)
                    {
                        element = VisualTreeHelper.GetParent(element) as FrameworkElement;

                        if (element is ColorEdit)
                        {
                            item = (ColorEdit)element;
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
            try
            {
                Point p = e.GetPosition(null);

                ((UIElement)sender).CaptureMouse();
                cedit = GetBrushEditParentFromChildren(this.gradientitem);
                cedit.RemoveSelection();
                cedit.gradientItemCollection.gradientItem = this;
                cedit.rgbChanged = true;
                cedit.Color = this.color;
                //cedit.CurrentColor.Fill = cedit.SelectedColor.Fill;
                cedit.fillGradient(this);
                this.isselected = true;
                //((Path)this.gradientitem.Children[0]).Stroke = cedit.SelectedBackground;
                // ((Path)this.gradientitem.Children[0]).Stroke = new SolidColorBrush(Colors.BlueViolet);
                (this.gradientitem.Children[0] as Control).SetValue(Canvas.ZIndexProperty, 1);
                isEnabled = true;
            }
            catch { }
        }

        /// <summary>
        /// Handles the MouseMove event of the gradientitem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void gradientitem_MouseMove(object sender, MouseEventArgs e)
        {
            if (isEnabled && this.gradientitem != null)
            {
                cedit = GetBrushEditParentFromChildren(this.gradientitem);
                Canvas cd = (Canvas)gradientitem.Parent;
                Point p = e.GetPosition(cd);
                Point pt = e.GetPosition(null);
                if (cd != null && p.X < cd.Width && p.X > 0.0)
                {
                    this.gradientitem.SetValue(Canvas.LeftProperty, p.X);
                    this.offset = p.X / cd.ActualWidth;
                    //cedit.enableSelectedBrush = true;
                    if (cedit != null)
                        cedit.fillGradient(this);
                    // cedit.enableSelectedBrush = false;
                }
            }
        }

        /// <summary>
        /// Handles the MouseLeftButtonUp event of the gradientitem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void gradientitem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                isEnabled = false;
                ((UIElement)sender).ReleaseMouseCapture();
                cedit = GetBrushEditParentFromChildren(this.gradientitem);
                Canvas cd = (Canvas)gradientitem.Parent;
                Point p = e.GetPosition(cd);
                Point pt = e.GetPosition(cedit);
                if (p.X < cd.Width && p.X > 0.0)
                {
                    if (p.Y > cd.ActualHeight + 10 && cedit.gradientItemCollection.Items.Count > 2)
                    {
                        cedit.canvasBar.Children.Remove(cedit.gradientItemCollection.gradientItem.gradientitem);
                        int selindex = cedit.gradientItemCollection.Items.IndexOf(this);
                        if (selindex < 0)
                            return;

                        if (selindex > 0)
                        {
                            selindex--;
                        }

                        cedit.gradientItemCollection.gradientItem = (GradientStopItem)cedit.gradientItemCollection.Items[selindex];
                        cedit.gradientItemCollection.Items.Remove(this);
                        // cedit.enableSelectedBrush = true;
                        cedit.fillGradient(cedit.gradientItemCollection.gradientItem);
                        // cedit.enableSelectedBrush = false;
                    }
                    else
                    {
                        gradientitem.SetValue(Canvas.LeftProperty, p.X);
                        this.offset = p.X / cd.ActualWidth;
                        //((Path)this.gradientitem.Children[0]).Stroke = new SolidColorBrush(Colors.BlueViolet);
                        cedit.fillGradient(this);
                    }
                }
            }
            catch
            { }
        }
    }
}