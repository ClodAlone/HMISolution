#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents the PolyGon Item Control.
    /// </summary>
    public class PolygonItem : Control
    {
        //ColorPickerPalette palette = new ColorPickerPalette();

        /// <summary>
        /// Identifies the <see cref="Points"/>  dependency property.
        /// </summary>
        public static readonly DependencyProperty PointProperty = DependencyProperty.Register("Points", typeof(PointCollection), typeof(PolygonItem), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="ColorName"/>  dependency property.
        /// </summary>
        public static readonly DependencyProperty ColorNameProperty = DependencyProperty.Register("ColorName", typeof(string), typeof(PolygonItem), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="color"/>  dependency property.
        /// </summary>
        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register("color", typeof(Brush), typeof(PolygonItem), new PropertyMetadata(new SolidColorBrush(Colors.Red), new PropertyChangedCallback(IsColorChanged)));

        /// <summary>
        /// Identifies the <see cref="RowIndex"/>  dependency property.
        /// </summary>
        public static readonly DependencyProperty RowIndexProperty = DependencyProperty.Register("RowIndex", typeof(int), typeof(PolygonItem), new PropertyMetadata(0));

        /// <summary>
        /// Identifies the <see cref="ColumnIndex"/>  dependency property.
        /// </summary>
        public static readonly DependencyProperty ColumnIndexProperty = DependencyProperty.Register("ColumnIndex", typeof(int), typeof(PolygonItem), new PropertyMetadata(0));

        /// <summary>
        /// Gets or sets the value of the ColumnIndex dependency property.
        /// </summary>
        public int ColumnIndex
        {
            get
            {
                return (int)GetValue(ColumnIndexProperty);
            }

            set
            {
                SetValue(ColumnIndexProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the name of the color.
        /// </summary>
        /// <value>The name of the color.</value>
        public string ColorName
        {
            get
            {
                return (string)GetValue(ColorNameProperty);
            }

            set
            {
                SetValue(ColorNameProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the RowIndex dependency property.
        /// </summary>
        public int RowIndex
        {
            get
            {
                return (int)GetValue(RowIndexProperty);
            }

            set
            {
                SetValue(RowIndexProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the color dependency property.
        /// </summary>
        public Brush color
        {
            get
            {
                return (Brush)GetValue(ColorProperty);
            }

            set
            {
                SetValue(ColorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the Points dependency property.
        /// </summary>
        public PointCollection Points
        {
            get
            {
                return (PointCollection)GetValue(PointProperty);
            }

            set
            {
                SetValue(PointProperty, value);
            }
        }

        /// <summary>
        /// Creates the instance of PolygonItem control
        /// </summary>
        public PolygonItem()
        {
            DefaultStyleKey = typeof(PolygonItem);
        }

        private PolygonItem more;
        private Polygon poly;
        internal MoreColorsWindow child;
        internal Path paths;

        /// <summary>
        /// Applies the Template for the control
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            poly = GetTemplateChild("polygon") as Polygon;
            poly.MouseLeftButtonDown += new MouseButtonEventHandler(polyMouseLeftButtonDown);
            child = GetBrushEditParentFromChildren(poly);
            more = new PolygonItem();
        }

        /// <summary>
        /// method to find the parent of the element
        /// </summary>
        /// <param name="element">Element for which parent is to be found</param>
        /// <returns>Returs an instance of ChildWindow1  </returns>
        internal static MoreColorsWindow GetBrushEditParentFromChildren(FrameworkElement element)
        {
            MoreColorsWindow item = null;
            if (element != null)
            {
                item = element as MoreColorsWindow;

                if (item == null)
                {
                    while (element != null)
                    {
                        element = VisualTreeHelper.GetParent(element) as FrameworkElement;

                        if (element is MoreColorsWindow)
                        {
                            item = (MoreColorsWindow)element;
                            break;
                        }
                    }
                }
            }

            return item;
        }

        /// <summary>
        /// Method called when polygon is pressed.
        /// </summary>
        /// <param name="sender">Object of the sender</param>
        /// <param name="e">Property change details, such as old value and new value</param>
        private void polyMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            int flag = 0;
            child.polygonitem = this;
            child.New.Background = this.color;
            child.palette.child.polygonitem.color = this.color;
            child.palette.child.polygonitem.Points = this.Points;
            if (child.palette.child.polygonitem.RowIndex == 7 && child.palette.child.polygonitem.ColumnIndex == 7)
            {
                var obj = from more in child.palette.child.morecolorcollection where (more.RowIndex == 14 && more.ColumnIndex == 1) select more;
                foreach (PolygonItem poly in obj)
                {
                    child.palette.child.path1.Stroke = new SolidColorBrush(Colors.Black);
                    child.palette.child.path1.Fill = new SolidColorBrush(Colors.White);
                    child.palette.child.path1.Data = DrawPath(poly.Points);
                    flag = 1;
                }
            }

            if (child.palette.child.polygonitem.RowIndex == 14 && child.palette.child.polygonitem.ColumnIndex == 1)
            {
                var obj = from more in child.palette.child.morecolorcollection where (more.RowIndex == 7 && more.ColumnIndex == 7) select more;
                foreach (PolygonItem poly in obj)
                {
                    child.palette.child.path1.Stroke = new SolidColorBrush(Colors.Black);
                    child.palette.child.path1.Fill = new SolidColorBrush(Colors.White);
                    child.palette.child.path1.Data = DrawPath(poly.Points);
                    flag = 1;
                }
            }

            if (flag != 1)
            {
                child.palette.child.path1.Data = null;
            }

            paths = child.palette.child.path;
            if (paths != null)
            {
                paths.Stroke = new SolidColorBrush(Colors.Black);
                paths.Fill = new SolidColorBrush(Colors.White);
                paths.Data = DrawPath(child.palette.child.polygonitem.Points);
            }

            child.Item.Focus();
        }

        /// <summary>
        /// Method used to calculate data for path object
        /// </summary>
        /// <param name="points">Points for Path's data</param>
        /// <returns>returns instance of PathGeometry</returns>
        internal PathGeometry DrawPath(PointCollection points)
        {
            PathGeometry geo = new PathGeometry();
            PathFigure pathfig = new PathFigure();
            pathfig.StartPoint = points[0];
            LineSegment line = new LineSegment();
            line.Point = points[1];
            pathfig.Segments.Add(line);
            line = new LineSegment();
            line.Point = points[2];
            pathfig.Segments.Add(line);
            line = new LineSegment();
            line.Point = points[3];
            pathfig.Segments.Add(line);
            line = new LineSegment();
            line.Point = points[4];
            pathfig.Segments.Add(line);
            line = new LineSegment();
            line.Point = points[5];
            pathfig.Segments.Add(line);
            line = new LineSegment();
            line.Point = points[0];
            pathfig.Segments.Add(line);
            geo.Figures.Add(pathfig);
            PathGeometry geo1 = new PathGeometry();
            PathFigure pathfig1 = new PathFigure();
            pathfig1.StartPoint = new Point(points[0].X + 2, points[0].Y + 1);
            line = new LineSegment();
            line.Point = new Point(points[1].X, points[1].Y + 3);
            pathfig1.Segments.Add(line);
            line = new LineSegment();
            line.Point = new Point(points[2].X - 3, points[2].Y + 1);
            pathfig1.Segments.Add(line);
            line = new LineSegment();
            line.Point = new Point(points[3].X - 3, points[3].Y - 1);
            pathfig1.Segments.Add(line);
            line = new LineSegment();
            line.Point = new Point(points[4].X, points[4].Y - 3);
            pathfig1.Segments.Add(line);
            line = new LineSegment();
            line.Point = new Point(points[5].X + 3, points[5].Y - 1);
            pathfig1.Segments.Add(line);
            line = new LineSegment();
            line.Point = new Point(points[0].X + 3, points[0].Y + 1);
            pathfig1.Segments.Add(line);
            geo.Figures.Add(pathfig1);
            return geo;
        }

        /// <summary>
        /// Event raised when color is changed
        /// </summary>
        /// <param name="o">PolygonItem object where the change occures on</param>
        /// <param name="e">Property change details, such as old value and new value</param>
        private static void IsColorChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            PolygonItem g = (PolygonItem)o;
        }
    }
}