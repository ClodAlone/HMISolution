#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Controls.Map
{
    using System;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;
    using System.Collections.Generic;
    using System.Linq;
    using System.Collections.ObjectModel;

    /// <summary>
    ///  MapPath Class for Paths in the Map
    /// </summary>
    [System.ComponentModel.Description("MapPath Class for Paths in the Map")]
    public class MapPath : Control
    {

#if SILVERLIGHT
        private ResourceDictionary mapResources = new ResourceDictionary();
#endif

        internal ShapeFileLayer shpLayer;

        #region LabelMargin(Internal Property)
        internal Thickness LabelMargin
        {
            get { return (Thickness)GetValue(LabelMarginProperty); }
            set { SetValue(LabelMarginProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for LabelMargin.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LabelMarginProperty =
            DependencyProperty.Register("LabelMargin", typeof(Thickness), typeof(MapPath), new PropertyMetadata(null));
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Windows.Controls.Map.MapPath">MapPath</see> class. 
        /// </summary>
        [System.ComponentModel.Description("Initializes a new instance of the MapPath class.")]
        public MapPath()
        {
            this.DefaultStyleKey = typeof(MapPath);
            this.PathPoints = new ObservableCollection<Point>();
            this.PathPoints.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(PathPoints_CollectionChanged);
#if SILVERLIGHT
            mapResources.Source = new Uri("/Syncfusion.Maps.Silverlight;component/Themes/Generic.xaml", UriKind.RelativeOrAbsolute);
            if (mapResources["PART_MapPathStyle"] != null)
            {
                this.Style = mapResources["PART_MapPathStyle"] as Style;
            }
#endif
         
        }

        void PathPoints_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
          
        }

        #endregion

        #region Private Fields
        #endregion

        #region Properties

        #region PathLabelVisibility(Depedency Property)

        /// <summary>
        /// Gets or sets Visibility for Map Path Label.
        /// </summary>
        /// <value>
        /// Visibility
        /// </value>
        public Visibility PathLabelVisibility
        {
            get { return (Visibility)GetValue(PathLabelVisibilityProperty); }
            set { SetValue(PathLabelVisibilityProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for PathLabelVisibility.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty PathLabelVisibilityProperty =
            DependencyProperty.Register("PathLabelVisibility", typeof(Visibility), typeof(MapPath), new PropertyMetadata(Visibility.Collapsed));

        #endregion          

        #region PathPoints(Dependency Property)

        /// <summary>
        /// Gets or sets list of Points to Draw Path on the Map.
        /// </summary>
        /// <value>List of Points</value>
        /// <remarks>Property that holds the List points to draw the Line on the Map</remarks>
        [System.ComponentModel.Description("Gets or sets list of Points to Draw Path on the Map.")]
        public ObservableCollection<Point> PathPoints
        {
            get { return (ObservableCollection<Point>)GetValue(PathPointsProperty); }
            set { SetValue(PathPointsProperty, value); }
        }

        /// <summary>
        /// Identifies PathPoints Dependency Property
        /// </summary>
        public static readonly DependencyProperty PathPointsProperty =
         DependencyProperty.Register("PathPoints", typeof(ObservableCollection<Point>), typeof(MapPath), new PropertyMetadata(null));

        #endregion

        #region PathLabel(Dependency Property)

        /// <summary>
        /// Gets or Sets the Label for a MapPath
        /// </summary>
        /// <value>String for MapPath Label</value>
        /// <remarks>MapPath Property that sets or gets the Label for the Path elements in the Map</remarks>
        [System.ComponentModel.Description("Gets or Sets the Label for a MapPath")]
        public string PathLabel
        {
            get { return (string)GetValue(PathLabelProperty); }
            set { SetValue(PathLabelProperty, value); }
        }

        /// <summary>
        /// Identifies PathLabel Dependency Property
        /// </summary>
        public static readonly DependencyProperty PathLabelProperty =
          DependencyProperty.Register("PathLabel", typeof(string), typeof(MapPath), new PropertyMetadata(""));


        #endregion

        #region PathLabelPosition(Depedency Property)

        /// <summary>
        /// Gets or sets the path label position.
        /// </summary>
        /// <value>The path label position.</value>
        /// <remarks>MapPath Property that sets or gets the PathLabelPosition for the SelectedPath in the Map</remarks>
        public PathLabelPosition PathLabelPosition
        {
            get { return (PathLabelPosition)GetValue(PathLabelPositionProperty); }
            set { SetValue(PathLabelPositionProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for PathLabelPosition.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty PathLabelPositionProperty =
            DependencyProperty.Register("PathLabelPosition", typeof(PathLabelPosition), typeof(MapPath), new PropertyMetadata(PathLabelPosition.OnMiddlePoint, new PropertyChangedCallback(OnPathLabelPositionChanged)));

        private static void OnPathLabelPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MapPath mapPath = d as MapPath;
            if (e.NewValue != null)
            {
                if (mapPath.shpLayer != null)
                {
                    mapPath.LabelMargin = mapPath.shpLayer.GetLabelMargin(mapPath.LabelPoint, mapPath);

                }
            }
        }


        #endregion

        #region LabelPoint(DepedencyProperty)

        /// <summary>
        /// Gets or sets the label point.
        /// </summary>
        /// <value>The label point.</value>
        /// <remarks>LabelPoint which is used to get and set the selected path in the Map.</remarks>
        public Point LabelPoint
        {
            get { return (Point)GetValue(LabelPointProperty); }
            set { SetValue(LabelPointProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for LabelPoint.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LabelPointProperty =
            DependencyProperty.Register("LabelPoint", typeof(Point), typeof(MapPath), new PropertyMetadata(new Point(), new PropertyChangedCallback(OnLabelPointChanged)));

        private static void OnLabelPointChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            MapPath mapPath = d as MapPath;
            if (args.NewValue != null)
            {
                if (mapPath.shpLayer != null)
                {
                    mapPath.LabelMargin = mapPath.shpLayer.GetLabelMargin((Point)args.NewValue, mapPath);

                }
            }
        }

        #endregion

        #region PathLabelFontSize(Dependency Property)

        /// <summary>
        /// Gets or sets the font size of the label in the Map.
        /// </summary>
        /// <value>The size of the label font.</value>
        public double PathLabelFontSize
        {
            get { return (double)GetValue(PathLabelFontSizeProperty); }
            set { SetValue(PathLabelFontSizeProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for LabelFontSize.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty PathLabelFontSizeProperty =
            DependencyProperty.Register("PathLabelFontSize", typeof(double), typeof(MapPath), new PropertyMetadata(12d));

        #endregion

        #region PathColor(Depencency Property)

        /// <summary>
        /// Gets or sets the Color for the MapPath.
        /// </summary>
        /// <value>ColorBrush to set the Color for the Map </value>
        /// <remarks>MapPath Property that sets or gets the color of a Path Element in the Map.</remarks>
        [System.ComponentModel.Description("Gets or sets the Color for the MapPath.")]
        public Brush PathColor
        {
            get { return (Brush)GetValue(PathColorProperty); }
            set { SetValue(PathColorProperty, value); }
        }

        /// <summary>
        /// Identifies the PathColor Dependency Property
        /// </summary>
        public static readonly DependencyProperty PathColorProperty =
           DependencyProperty.Register("PathColor", typeof(Brush), typeof(MapPath), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        #endregion

        #region PathLabelFontStyle
        /// <summary>
        /// Gets or sets FontStyle for Path Label.
        /// </summary>
        /// <value>
        /// FontStyle
        /// </value>
        public FontStyle PathLabelFontStyle
        {
            get { return (FontStyle)GetValue(PathLabelFontStyleProperty); }
            set { SetValue(PathLabelFontStyleProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for PathLabelFontStyle.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty PathLabelFontStyleProperty =
            DependencyProperty.Register("PathLabelFontStyle", typeof(FontStyle), typeof(MapPath), new PropertyMetadata(FontStyles.Normal));

        #endregion               
        
        #region PathLabelFontFamily
        /// <summary>
        /// Gets or sets FontFamily for Path Label.
        /// </summary>
        /// <value>
        /// FontFamily
        /// </value>
        public FontFamily PathLabelFontFamily
        {
            get { return (FontFamily)GetValue(PathLabelFontFamilyProperty); }
            set { SetValue(PathLabelFontFamilyProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for PathLabelFontFamily.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty PathLabelFontFamilyProperty =
            DependencyProperty.Register("PathLabelFontFamily", typeof(FontFamily), typeof(MapPath), new PropertyMetadata(new FontFamily("Times New Roman")));

        #endregion        

        #region PathlabelForeground
        /// <summary>
        /// Gets or sets Foreground for Path Label.
        /// </summary>
        /// <value>
        /// Foreground
        /// </value>
        public Brush PathLabelForeground
        {
            get { return (Brush)GetValue(PathLabelForegroundProperty); }
            set { SetValue(PathLabelForegroundProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for PathLabelForeground.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty PathLabelForegroundProperty =
            DependencyProperty.Register("PathLabelForeground", typeof(Brush), typeof(MapPath), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        #endregion

        #region PathStroke(Dependency Property)

        /// <summary>
        /// Gets or sets the PathStroke for the MapPath.
        /// </summary>
        /// <value>Color Brush for stroke in the MapPath</value>
        /// <remarks>MapPath property that gets or sets stroke color of a Path Element in tha Map.</remarks>
        [System.ComponentModel.Description("Gets or sets the PathStroke for the MapPath.")]
        public Brush PathStroke
        {
            get { return (Brush)GetValue(PathStrokeProperty); }
            set { SetValue(PathStrokeProperty, value); }
        }

        /// <summary>
        /// Identifies the PathStroke Dependency Property
        /// </summary>
        public static readonly DependencyProperty PathStrokeProperty =
           DependencyProperty.Register("PathStroke", typeof(Brush), typeof(MapPath), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        #endregion

        #region PathStrokeThickness Property

        /// <summary>
        /// Gets or sets the thickness of the Path Stroke.
        /// </summary>
        /// <value>Double value to set the stroke thickness of the MapPath</value>
        /// <remarks>MapPath property that sets or set the Stroke thickness of the MapPath</remarks>
        public double PathStrokeThickness
        {
            get { return (double)GetValue(PathStrokeThicknessProperty); }
            set { SetValue(PathStrokeThicknessProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for PathStrokeThickness.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty PathStrokeThicknessProperty =
            DependencyProperty.Register("PathStrokeThickness", typeof(double), typeof(MapPath), new PropertyMetadata(1d));

        #endregion

        #region MapPathContent (Internal Dependency Property)

        /// <summary>
        /// Gets or sets the Content of the MapPath.
        /// </summary>
        /// <value>Path element.</value>
        /// <remarks>MapPath Property which is used to set the content of the Path element in the map.Its a internal Property</remarks>
        [System.ComponentModel.Description("Gets or sets the Content of the MapPath.")]
        internal object MapPathContent
        {
            get { return (Path)GetValue(MapPathContentProperty); }
            set { SetValue(MapPathContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MapPathContent.  This enables animation, styling, binding, etc...
        /// <summary>
        /// Identifies the MapPathContent Dependency Property
        /// </summary>
        public static readonly DependencyProperty MapPathContentProperty =
       DependencyProperty.Register("MapPathContent", typeof(Path), typeof(MapPath), new PropertyMetadata(null));

        #endregion

        #region Angle
        /// <summary>
        /// Gets or sets Angle for Path Label.
        /// </summary>
        /// <value>
        /// Double
        /// </value>
        public double Angle
        {
            get { return (double)GetValue(AngleProperty); }
            set { SetValue(AngleProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for Angle.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AngleProperty =
            DependencyProperty.Register("Angle", typeof(double), typeof(MapPath), new PropertyMetadata(0d));

        #endregion       

        #endregion

        #region HelperMethods

        /// <summary>
        /// Convert the given points to Geometry
        /// </summary>
        /// <remarks>
        /// A function that converts the given points to geometry
        /// </remarks>
        /// <param name="points">Points</param>
        /// <param name="shapeLayer">ShapeFileLayer</param>
        /// <returns>
        /// The Geometry value to create a path
        /// </returns>
        


        public Geometry PointsToGeomentry(ObservableCollection<Point> points, ShapeFileLayer shapeLayer)
        {
            this.shpLayer = shapeLayer;
            var i = 0;

            var geomenty = new PathGeometry();
            var pathFigure = new PathFigure();
            var panxval = shapeLayer.PanTransform.X;
            var panyval = shapeLayer.PanTransform.Y;
            var zoom = shapeLayer.ZoomFactor;

            Point temp_point; //temporary point variable to make changes when zooming or panning occurs

            foreach (Point latlonpoint in points)
            {
                var point = shapeLayer.GetMapElementsPosition(latlonpoint); //Convert the Latitude and longitude values to Points

                //assign the point values to the temp_point according to the zooming and panning values.
                temp_point = this.shpLayer.ZoomPanPointValue(point);

                if (i == 0)
                {
                    pathFigure.StartPoint = new Point(temp_point.X, temp_point.Y);
                    i++;
                }
                else
                {
                    var lineSegment = new LineSegment();
                    lineSegment.Point = new Point(temp_point.X, temp_point.Y);
                    pathFigure.Segments.Add(lineSegment);
                }

            }
            geomenty.Figures.Add(pathFigure);
            return geomenty;
        }
        #endregion
        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or
        /// internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();           
        }


        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.UIElement.MouseLeftButtonDown"/> routed event is raised
        /// on this element. Implement this method to add class handling for this event. 
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/>
        /// that contains the event data. The event data reports that the left mouse button
        /// was pressed.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            foreach (MapPath mpath in this.shpLayer.MapPathCollection)
            {
                if (mpath.MapPathContent.Equals(e.OriginalSource))
                {
                    //Get the Selected MapPath
                    this.shpLayer.SelectedMapPath = mpath;
                    break;
                }
                else
                {
                    this.shpLayer.SelectedMapPath = null;
                }
            }

            base.OnMouseLeftButtonDown(e);
        }
    }
}
