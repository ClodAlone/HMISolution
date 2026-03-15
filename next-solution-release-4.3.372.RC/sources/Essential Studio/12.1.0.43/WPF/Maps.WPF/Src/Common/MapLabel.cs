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
    using System.Windows.Data;
    using System.Linq;
    using System.Diagnostics;

    /// <summary>
    ///  MapLabel is used to show the descriptions of the map
    /// </summary>
    public class MapLabel : ContentControl
    {
        #region Private Fields

        private ShapeFileLayer ShapeLayer;
        private ResourceDictionary mapResources = new ResourceDictionary();
        private bool IsShiftPressed = false;
        private MapLabel maplbl;
        private MapSymbols mapsym;
        private bool escPressed = false;

        #endregion

        #region Internal Fields

        internal TextBox labelTextBox;
        internal TextBlock MapLabelTextBlock;
        internal bool isMouseDown = false;
        internal Point mousePosition;
        internal double startlat;
        internal double startlong;
        internal bool isDragged = false;
        internal string templabel;
        internal bool lockUnselect = false;

        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MapLabel"/> class.
        /// </summary>
        public MapLabel()
        {
            this.DefaultStyleKey = typeof(MapLabel);
#if SILVERLIGHT
            mapResources.Source = new Uri("/Syncfusion.Maps.Silverlight;component/Themes/Generic.xaml", UriKind.RelativeOrAbsolute);
#endif
#if WPF
            mapResources.Source = new Uri("/Syncfusion.Maps.Wpf;component/Themes/Generic.xaml", UriKind.RelativeOrAbsolute);
#endif

            if (mapResources["PART_MapLabelStyle"] != null)
            {
                this.Style = mapResources["PART_MapLabelStyle"] as Style;
            }
        }

        #endregion

        #region Override Methods
        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>. In simplest terms, this means the method is called just before a UI element displays in an application. For more information, see Remarks.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.labelTextBox = this.GetTemplateChild("PART_LabelTextBox") as TextBox;
            this.MapLabelTextBlock = this.GetTemplateChild("PART_LabelTextBlock") as TextBlock;
            this.labelTextBox.LostFocus += new RoutedEventHandler(labelTextBox_LostFocus);
            this.labelTextBox.KeyDown += new KeyEventHandler(txtbox_KeyDown);
            this.MouseLeftButtonDown += new MouseButtonEventHandler(MapLabel_MouseLeftButtonDown);
            this.MouseLeftButtonUp += new MouseButtonEventHandler(MapLabel_MouseLeftButtonUp);
            this.MouseLeave += new MouseEventHandler(MapLabel_MouseLeave);
            this.MouseMove += new MouseEventHandler(MapLabel_MouseMove);
            this.ShapeLayer = MapControl.FindParent<ShapeFileLayer>(this);
            if (this.LabelDescription == "Empty")
            {
                this.LabelDescription = this.LabelText;
            }
            this.labelTextBox.Focus();
            this.labelTextBox.SelectAll();
        }

        void labelTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            this.IsLabelEditing = false;
            if (this.escPressed)
            {
                this.lockUnselect = true;
                this.ShapeLayer.mapControl.isLabelEdited = true; 
            }
            this.ShapeLayer.ClickedItem = null;
            
        }

        void MapLabel_MouseLeave(object sender, MouseEventArgs e)
        {
            this.IsHover = false;
        }

        void MapLabel_MouseMove(object sender, MouseEventArgs e)
        {
            this.IsHover = true;
        }

        #endregion

        #region Properties

        #region Latitude(Dependency Property)


        /// <summary>
        /// Gets or sets the latitude value for the Label on the Map.
        /// </summary>
        /// <value>The latitude.</value>
        public double Latitude
        {
            get { return (double)GetValue(LatitudeProperty); }
            set { SetValue(LatitudeProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for Latitude.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LatitudeProperty =
            DependencyProperty.Register("Latitude", typeof(double), typeof(MapLabel), new PropertyMetadata(0d, new PropertyChangedCallback(OnLatitudeChanged)));

        private static void OnLatitudeChanged(DependencyObject dobj, DependencyPropertyChangedEventArgs args)
        {
            MapLabel mapLabel = dobj as MapLabel;
            ShapeFileCanvas.SetLatitude(mapLabel, (double)args.NewValue);
            if (mapLabel.ShapeLayer != null)
            {
                mapLabel.ShapeLayer.drawingCanvas.InvalidateArrange();
            }
        }


        #endregion

        #region Longitude(Dependency Property)

        /// <summary>
        /// Gets or sets the longitude vlaue for the Label On the Map.
        /// </summary>
        /// <value>The longitude.</value>
        public double Longitude
        {
            get { return (double)GetValue(LongitudeProperty); }
            set { SetValue(LongitudeProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for Longitude.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LongitudeProperty =
            DependencyProperty.Register("Longitude", typeof(double), typeof(MapLabel), new PropertyMetadata(0d, new PropertyChangedCallback(OnLongitudetudeChanged)));

        private static void OnLongitudetudeChanged(DependencyObject dobj, DependencyPropertyChangedEventArgs args)
        {
            MapLabel mapLabel = dobj as MapLabel;
            ShapeFileCanvas.SetLongitude(mapLabel, (double)args.NewValue);
            if (mapLabel.ShapeLayer != null)
            {
                mapLabel.ShapeLayer.drawingCanvas.InvalidateArrange();
            }
        }

        #endregion

        #region LabelText(Dependency Property)

        /// <summary>
        /// Gets or sets the Label on the Map.
        /// </summary>
        /// <value>Label Text</value>
        public string LabelText
        {
            get { return (string)GetValue(LabelTextProperty); }
            set { SetValue(LabelTextProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for Label.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LabelTextProperty =
            DependencyProperty.Register("LabelText", typeof(string), typeof(MapLabel), new PropertyMetadata(""));

        #endregion

        #region LabelForeground(Dependency Property)

        /// <summary>
        /// Gets or sets the Foreground Color for the Labels on the Map.
        /// </summary>
        /// <value>Foreground Color.</value>
        public Brush LabelForeground
        {
            get { return (Brush)GetValue(LabelForegroundProperty); }
            set { SetValue(LabelForegroundProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for LabelForeground.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LabelForegroundProperty =
            DependencyProperty.Register("LabelForeground", typeof(Brush), typeof(MapLabel), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        #endregion

        #region LabelFontFamily(Dependency Property)

        /// <summary>
        /// Gets or sets the font family of the Labels in the Map.
        /// </summary>
        /// <value>Font Family</value>
        public FontFamily LabelFontFamily
        {
            get { return (FontFamily)GetValue(LabelFontFamilyProperty); }
            set { SetValue(LabelFontFamilyProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for LabelFontFamily.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LabelFontFamilyProperty =
            DependencyProperty.Register("LabelFontFamily", typeof(FontFamily), typeof(MapLabel), new PropertyMetadata(new FontFamily("Times New Roman")));


        #endregion

        #region LabelBackground(Dependency Property)

        /// <summary>
        /// Gets or sets Background color of the Label on the Map.
        /// </summary>
        /// <value>The Background Color.</value>
        public Brush LabelBackground
        {
            get { return (Brush)GetValue(LabelBackgroundProperty); }
            set { SetValue(LabelBackgroundProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for LabelBackground.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LabelBackgroundProperty =
            DependencyProperty.Register("LabelBackground", typeof(Brush), typeof(MapLabel), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));

        #endregion

        #region LabelFontStyle(Dependency Property)

        /// <summary>
        /// Gets or sets Font Style of the Label in the Map.
        /// </summary>
        /// <value>Font Style of the Label.</value>
        public FontStyle LabelFontStyle
        {
            get { return (FontStyle)GetValue(LabelFontStyleProperty); }
            set { SetValue(LabelFontStyleProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for LabelFontStyle.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LabelFontStyleProperty =
            DependencyProperty.Register("LabelFontStyle", typeof(FontStyle), typeof(MapLabel), new PropertyMetadata(FontStyles.Normal));

        #endregion

        #region LabelTextWrapping(Dependency Property)

        /// <summary>
        /// Gets or sets wrapping of the label text in the Map.
        /// </summary>
        /// <value>The label text warpping.</value>
        public TextWrapping LabelTextWarpping
        {
            get { return (TextWrapping)GetValue(LabelTextWarppingProperty); }
            set { SetValue(LabelTextWarppingProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for LabelTextWarpping.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LabelTextWarppingProperty =
            DependencyProperty.Register("LabelTextWarpping", typeof(TextWrapping), typeof(MapLabel), new PropertyMetadata(TextWrapping.NoWrap));


        #endregion

        #region LabelVisibility(Dependency Property)

        /// <summary>
        /// Gets or sets visibility of the Label in the Map.
        /// </summary>
        /// <value>The label visibility.</value>
        public Visibility LabelVisibility
        {
            get { return (Visibility)GetValue(LabelVisibilityProperty); }
            set { SetValue(LabelVisibilityProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for LabelVisibility.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LabelVisibilityProperty =
            DependencyProperty.Register("LabelVisibility", typeof(Visibility), typeof(MapLabel), new PropertyMetadata(Visibility.Visible));

        #endregion

        #region LabelFontSize(Dependency Property)

        /// <summary>
        /// Gets or sets the font size of the label in the Map.
        /// </summary>
        /// <value>The size of the label font.</value>
        public double LabelFontSize
        {
            get { return (double)GetValue(LabelFontSizeProperty); }
            set { SetValue(LabelFontSizeProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for LabelFontSize.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LabelFontSizeProperty =
            DependencyProperty.Register("LabelFontSize", typeof(double), typeof(MapLabel), new PropertyMetadata(12d));

        #endregion

        #region IsLabelEditing(Dependency Property - Internal Property)


        internal bool IsLabelEditing
        {
            get { return (bool)GetValue(IsLabelEditingProperty); }
            set { SetValue(IsLabelEditingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsLabelEditing.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty IsLabelEditingProperty =
            DependencyProperty.Register("IsLabelEditing", typeof(bool), typeof(MapLabel), new PropertyMetadata(false));


        #endregion

        #region LabelDescription


        /// <summary>
        /// Gets or sets Description of the label in the Map.
        /// </summary>
        /// <value>The description of the label font.</value>
        public string LabelDescription
        {
            get { return (string)GetValue(LabelDescriptionProperty); }
            set { SetValue(LabelDescriptionProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for LabelDescription.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LabelDescriptionProperty =
            DependencyProperty.Register("LabelDescription", typeof(string), typeof(MapLabel), new PropertyMetadata("Empty"));

        #endregion

        #region LabelWidth(Dependency Property)

        /// <summary>
        /// Gets or sets Width of the label in the Map.
        /// </summary>
        /// <value>The Width of the label font.</value>
        public double LabelWidth
        {
            get { return (double)GetValue(LabelWidthProperty); }
            set { SetValue(LabelWidthProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for LabelWidth.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LabelWidthProperty =
            DependencyProperty.Register("LabelWidth", typeof(double), typeof(MapLabel), new PropertyMetadata(double.NaN));

        #endregion

        #region IsSelected

        /// <summary>
        /// Gets or sets a value indicating whether label is selected or not.
        /// </summary>
        /// <value>
        /// 	<see langword="true"/> if this instance ; otherwise, <see langword="false"/>.
        /// </value>
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            internal set { SetValue(IsSelectedProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for IsSelected.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(MapLabel), new PropertyMetadata(false));


        #endregion

        #region IsHover

        internal bool IsHover
        {
            get { return (bool)GetValue(IsHoverProperty); }
            set { SetValue(IsHoverProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsHover.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty IsHoverProperty =
            DependencyProperty.Register("IsHover", typeof(bool), typeof(MapLabel), new PropertyMetadata(false));

        #endregion

        #endregion

        #region Event Handlers

        void MapLabel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.isMouseDown = true;
            this.isDragged = true;
            this.mousePosition = e.GetPosition(this);
            this.startlat = this.Latitude;
            this.startlong = this.Longitude;
            this.templabel = this.LabelText;
            if (this.ShapeLayer.SelectedMapLabel != null)
            {
                if (! sender.Equals(this.ShapeLayer.SelectedMapLabel))
                {
                    this.CheckSelection();
                    this.ShapeLayer.SelectedMapLabel = sender as MapLabel;
                    this.IsSelected = true;
                }
            }
            else
            {
                this.ShapeLayer.SelectedMapLabel = sender as MapLabel;
                this.IsSelected = true;
            }
          
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                this.ShapeLayer.LabelCollections.Remove(this.ShapeLayer.SelectedMapLabel);
                this.ShapeLayer.SelectedMapLabel = null;
                e.Handled = true;
            }
            else if (Keyboard.Modifiers == (ModifierKeys.Shift) && this.ShapeLayer.ClickedItem == null)
            {
                this.ShapeLayer.ClickedItem = "Label";
                this.IsLabelEditing = true;
                this.IsShiftPressed = true;
                this.labelTextBox.Focus();
                e.Handled = true;
            }
            this.labelTextBox.Focus();
            this.labelTextBox.SelectAll();
            e.Handled = true;
        }


        void txtbox_KeyDown(object sender, KeyEventArgs e)
        {
            this.escPressed = false;
            if (e.Key == Key.Enter)
            {
                if (this.LabelDescription == string.Empty)
                {
                    this.LabelDescription = (sender as TextBox).Text;
                }
                this.IsLabelEditing = false;
                this.lockUnselect = false;
                this.CheckLabelIntersection();

            }
            else if (e.Key == Key.Escape)
            {
                if (this.IsShiftPressed)
                {
                    (sender as TextBox).Text = this.templabel;
                    this.LabelText = this.templabel;
                    this.escPressed = true;
                    this.IsShiftPressed = false;
                }
                else
                {
                    this.ShapeLayer.LabelCollections.Remove(this);
                }
                this.lockUnselect = true;
                this.IsLabelEditing = false;
                this.CheckLabelIntersection();
            }
            this.ShapeLayer.ClickedItem = null;
            this.isMouseDown = false;
        }

        void MapLabel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.isMouseDown = false;
            this.isDragged = true;
            if (this.ShapeLayer.SelectedMapLabel != null)
            {
                if (this.ShapeLayer.SelectedMapLabel.CheckLabelIntersection())
                {
                    this.ShapeLayer.SelectedMapLabel.Latitude = this.ShapeLayer.SelectedMapLabel.startlat;
                    this.ShapeLayer.SelectedMapLabel.Longitude = this.ShapeLayer.SelectedMapLabel.startlong;
                }
            }

        }
        #endregion

        #region HelperMethods


        internal void CheckSelection(ShapeFileLayer ShapeLayer)
        {
            var prevSeleted = from labels in ShapeLayer.LabelCollections
                              where labels.IsSelected
                              select labels;
            if (prevSeleted.ToList().Count > 0)
            {
                foreach (MapLabel lbl in prevSeleted.ToList())
                {
                    lbl.IsSelected = false;
                }
            }
           ShapeLayer.SelectedMapLabel = null;
        }

        private void CheckSelection()
        {
            this.CheckSelection(this.ShapeLayer);
        }


        /// <summary>
        /// Findintersects the specified r1.
        /// </summary>
        /// <param name="r1">The r1.</param>
        /// <param name="r2">The r2.</param>
        /// <returns></returns>
        private Rect Findintersect(Rect r1, Rect r2)
        {
            r1.Intersect(r2);
            return r1;
        }

        internal bool CheckLabelIntersection()
        {
            Rect r4 = Rect.Empty;
            Rect r3 = Rect.Empty;
            Rect r5 = Rect.Empty;
            Rect r6 = Rect.Empty;
            Rect r = new Rect(this.ShapeLayer.LatitudeLongitudeToPoint(new Point(this.Longitude, this.Latitude)), new Size(this.DesiredSize.Width * this.ShapeLayer.ZoomFactor, this.DesiredSize.Height * this.ShapeLayer.ZoomFactor));
            var element = from maplabel in this.ShapeLayer.LabelCollections
                          where (this.Findintersect(r, new Rect(this.ShapeLayer.LatitudeLongitudeToPoint(new Point((maplabel as MapLabel).Longitude, (maplabel as MapLabel).Latitude)), new Size((maplabel as MapLabel).DesiredSize.Width * this.ShapeLayer.ZoomFactor, (maplabel as MapLabel).DesiredSize.Height * this.ShapeLayer.ZoomFactor))) != Rect.Empty && !maplabel.Equals(this))
                          select maplabel;
            var symelement = from maplabel in this.ShapeLayer.SymbolCollection
                             where (this.Findintersect(r, new Rect(this.ShapeLayer.LatitudeLongitudeToPoint(new Point((maplabel as MapSymbols).Longitude, (maplabel as MapSymbols).Latitude)), new Size((maplabel as MapSymbols).DesiredSize.Width * this.ShapeLayer.ZoomFactor, (maplabel as MapSymbols).DesiredSize.Height * this.ShapeLayer.ZoomFactor))) != Rect.Empty && !maplabel.Equals(this))
                             select maplabel;
            maplbl = new MapLabel();
            if (symelement.ToList().Count > 0)
            {
                mapsym = symelement.ToList()[0] as MapSymbols;
            }
            if (element.ToList().Count > 0)
            {
                maplbl = element.ToList()[0] as MapLabel;
            }
            if (maplbl.MapLabelTextBlock != null)
            {
                r3 = new Rect(this.ShapeLayer.LatitudeLongitudeToPoint(new Point((maplbl as MapLabel).Longitude, (maplbl as MapLabel).Latitude)), new Size((maplbl as MapLabel).DesiredSize.Width * this.ShapeLayer.ZoomFactor, (maplbl as MapLabel).DesiredSize.Height * this.ShapeLayer.ZoomFactor));
                r4 = this.Findintersect(r, r3);
            }
            if (mapsym != null)
            {
                r5 = new Rect(this.ShapeLayer.LatitudeLongitudeToPoint(new Point((mapsym as MapSymbols).Longitude, (mapsym as MapSymbols).Latitude)), new Size((mapsym as MapSymbols).DesiredSize.Width * this.ShapeLayer.ZoomFactor, (mapsym as MapSymbols).DesiredSize.Height * this.ShapeLayer.ZoomFactor));
                r6 = this.Findintersect(r, r5);
            }

            if ((r4 != Rect.Empty && r3 != Rect.Empty) || (r5 != Rect.Empty && r6 != Rect.Empty))
            {

                if (!this.ShapeLayer.AllowLabelIntersection)
                {
                    if (!this.isDragged)
                    {
                        if (this.IsShiftPressed)
                        {
                            this.LabelText = this.templabel;
                        }
                        else
                        {
                            this.ShapeLayer.LabelCollections.Remove(this.ShapeLayer.SelectedMapLabel);
                        }
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        internal void InitializeMapLabel(ShapeFileLayer shapeLayer)
        {
            this.ShapeLayer = shapeLayer;
        }


        #endregion
    }
}
