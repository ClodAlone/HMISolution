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
    using System.Windows.Controls.Primitives;
    using System.Linq;
    /// <summary>
    ///  Map symbol is a symbolism used on a map usually containing swatches of symbols
    /// with descriptions
    /// </summary>
    public class MapSymbols : ContentControl
    {
        #region Private Fields

        private ShapeFileLayer ShapeLayer;
        private string templabel;
        private ResourceDictionary mapResources = new ResourceDictionary();
        private bool IsShiftPressed = false;
        private MapSymbols mapsym;
        private MapLabel maplbl;
        #endregion

        #region Internal Fields

        internal TextBox symbollabelTextBox;
        internal TextBlock symbolMapLabelTextBlock;
        internal Point mousePosition;
        internal bool isMousedown;
        internal double startlat;
        internal double startlong;
        internal bool isDragged;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Windows.Controls.Map.MapSymbols">MapSymbols</see> class. 
        /// </summary>
        /// <remarks></remarks>
        public MapSymbols()
        {
            this.DefaultStyleKey = typeof(MapSymbols);
#if SILVERLIGHT
            mapResources.Source = new Uri("/Syncfusion.Maps.Silverlight;component/Themes/Generic.xaml", UriKind.RelativeOrAbsolute);
#endif
#if WPF

            mapResources.Source = new Uri("/Syncfusion.Maps.Wpf;component/Themes/Generic.xaml", UriKind.RelativeOrAbsolute);
#endif
            this.Style = mapResources["PART_SymbolStyle"] as Style;
            this.MouseLeftButtonDown += new MouseButtonEventHandler(MapSymbols_MouseLeftButtonDown);
            this.MouseLeftButtonUp += new MouseButtonEventHandler(MapSymbols_MouseLeftButtonUp);
            this.MouseMove += new MouseEventHandler(MapSymbols_MouseMove);
            this.Loaded += new RoutedEventHandler(MapSymbols_Loaded);
            this.MouseLeave += new MouseEventHandler(MapSymbols_MouseLeave);

        }

        void MapSymbols_MouseLeave(object sender, MouseEventArgs e)
        {
            this.IsHover = false;
        }

        #region Event Handlers



        #endregion

        #endregion

        #region Properties

        #region Symbol(Dependency Property)

        /// <summary>
        /// Gets or sets Symbol for Maps.
        /// </summary>
        /// <value>
        /// UIElement
        /// </value>
        public UIElement Symbol
        {
            get { return (UIElement)GetValue(SymbolProperty); }
            set { SetValue(SymbolProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for Symbol.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SymbolProperty =
            DependencyProperty.Register("Symbol", typeof(UIElement), typeof(MapSymbols), new PropertyMetadata(null));

        #endregion

        #region Symbol Description (Dependency Property)

        /// <summary>
        /// Gets or sets Description of the Symbols.
        /// </summary>
        /// <value>
        /// String
        /// </value>
        public string SymbolDescription
        {
            get { return (string)GetValue(SymbolDescriptionProperty); }
            set { SetValue(SymbolDescriptionProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for SymbolDescription.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SymbolDescriptionProperty =
            DependencyProperty.Register("SymbolDescription", typeof(string), typeof(MapSymbols), new PropertyMetadata(""));

        #endregion

        #region Latitude(Dependency Property)
        /// <summary>
        /// Gets or sets Latitude of the Symbols.
        /// </summary>
        /// <value>
        /// double
        /// </value>
        public double Latitude
        {
            get { return (double)GetValue(LatitudeProperty); }
            set { SetValue(LatitudeProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for Latitude.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LatitudeProperty =
            DependencyProperty.Register("Latitude", typeof(double), typeof(MapSymbols), new PropertyMetadata(0d, new PropertyChangedCallback(OnLatitudeChanged)));
        private static void OnLatitudeChanged(DependencyObject dobj, DependencyPropertyChangedEventArgs args)
        {
            ShapeFileCanvas.SetLatitude(dobj, (double)args.NewValue);
            if ((dobj as MapSymbols).ShapeLayer != null)
            {
                (dobj as MapSymbols).ShapeLayer.drawingCanvas.InvalidateArrange();
            }
        }
        #endregion

        #region Longitude(Dependency Property)

        /// <summary>
        /// Gets or sets Longitude of the Symbols.
        /// </summary>
        /// <value>
        /// double
        /// </value>
        public double Longitude
        {
            get { return (double)GetValue(LongitudeProperty); }
            set { SetValue(LongitudeProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for Longitude.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LongitudeProperty =
            DependencyProperty.Register("Longitude", typeof(double), typeof(MapSymbols), new PropertyMetadata(0d, new PropertyChangedCallback(OnLongitudeChanged)));

        private static void OnLongitudeChanged(DependencyObject dobj, DependencyPropertyChangedEventArgs args)
        {
            ShapeFileCanvas.SetLongitude(dobj, (double)args.NewValue);
            if ((dobj as MapSymbols).ShapeLayer != null)
            {
                (dobj as MapSymbols).ShapeLayer.drawingCanvas.InvalidateArrange();
            }
        }
        #endregion

        #region SymbolText
        /// <summary>
        /// Gets or sets Text of the Symbols.
        /// </summary>
        /// <value>
        /// String
        /// </value>
        public string SymbolText
        {
            get { return (string)GetValue(SymbolTextProperty); }
            set { SetValue(SymbolTextProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for SymbolText.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SymbolTextProperty =
            DependencyProperty.Register("SymbolText", typeof(string), typeof(MapSymbols), new PropertyMetadata(""));

        #endregion

        #region LabelForeground(Dependency Property)

        /// <summary>
        /// Gets or sets the Foreground Color for the Labels on the Map.
        /// </summary>
        /// <value>Foreground Color.</value>
        public Brush SymbolLabelForeground
        {
            get { return (Brush)GetValue(SymbolLabelForegroundProperty); }
            set { SetValue(SymbolLabelForegroundProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for LabelForeground.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SymbolLabelForegroundProperty =
            DependencyProperty.Register("SymbolLabelForeground", typeof(Brush), typeof(MapSymbols), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        #endregion

        #region SymbolLabelFontFamily(Dependency Property)

        /// <summary>
        /// Gets or sets the font family of the Labels in the Map.
        /// </summary>
        /// <value>Font Family</value>
        public FontFamily SymbolLabelFontFamily
        {
            get { return (FontFamily)GetValue(SymbolLabelFontFamilyProperty); }
            set { SetValue(SymbolLabelFontFamilyProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for LabelFontFamily.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SymbolLabelFontFamilyProperty =
            DependencyProperty.Register("SymbolLabelFontFamily", typeof(FontFamily), typeof(MapSymbols), new PropertyMetadata(new FontFamily("Times New Roman")));


        #endregion

        #region SymbolLabelBackground(Dependency Property)

        /// <summary>
        /// Gets or sets Background color of the Label on the Map.
        /// </summary>
        /// <value>The Background Color.</value>
        public Brush SymbolLabelBackground
        {
            get { return (Brush)GetValue(SymbolLabelBackgroundProperty); }
            set { SetValue(SymbolLabelBackgroundProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for LabelBackground.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SymbolLabelBackgroundProperty =
            DependencyProperty.Register("SymbolLabelBackground", typeof(Brush), typeof(MapSymbols), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));

        #endregion

        #region SymbolLabelFontStyle(Dependency Property)

        /// <summary>
        /// Gets or sets Font Style of the Label in the Map.
        /// </summary>
        /// <value>Font Style of the Label.</value>
        public FontStyle SymbolLabelFontStyle
        {
            get { return (FontStyle)GetValue(SymbolLabelFontStyleProperty); }
            set { SetValue(SymbolLabelFontStyleProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for LabelFontStyle.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SymbolLabelFontStyleProperty =
            DependencyProperty.Register("SymbolLabelFontStyle", typeof(FontStyle), typeof(MapSymbols), new PropertyMetadata(FontStyles.Normal));

        #endregion

        #region SymbolLabelTextWrapping(Dependency Property)

        /// <summary>
        /// Gets or sets wrapping of the label text in the Map.
        /// </summary>
        /// <value>The label text warpping.</value>
        public TextWrapping SymbolLabelTextWarpping
        {
            get { return (TextWrapping)GetValue(SymbolLabelTextWarppingProperty); }
            set { SetValue(SymbolLabelTextWarppingProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for LabelTextWarpping.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SymbolLabelTextWarppingProperty =
            DependencyProperty.Register("SymbolLabelTextWarpping", typeof(TextWrapping), typeof(MapSymbols), new PropertyMetadata(TextWrapping.NoWrap));


        #endregion

        #region SymbolLabelVisibility(Dependency Property)

        /// <summary>
        /// Gets or sets visibility of the Label in the Map.
        /// </summary>
        /// <value>The label visibility.</value>
        public Visibility SymbolLabelVisibility
        {
            get { return (Visibility)GetValue(SymbolLabelVisibilityProperty); }
            set { SetValue(SymbolLabelVisibilityProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for LabelVisibility.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SymbolLabelVisibilityProperty =
            DependencyProperty.Register("SymbolLabelVisibility", typeof(Visibility), typeof(MapSymbols), new PropertyMetadata(Visibility.Visible));

        #endregion

        #region SymbolLabelFontSize(Dependency Property)

        /// <summary>
        /// Gets or sets the font size of the label in the Map.
        /// </summary>
        /// <value>The size of the label font.</value>
        public double SymbolLabelFontSize
        {
            get { return (double)GetValue(SymbolLabelFontSizeProperty); }
            set { SetValue(SymbolLabelFontSizeProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for LabelFontSize.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SymbolLabelFontSizeProperty =
            DependencyProperty.Register("SymbolLabelFontSize", typeof(double), typeof(MapSymbols), new PropertyMetadata(12d));

        #endregion

        #region IsLabelEditing


        internal bool IsLabelEditing
        {
            get { return (bool)GetValue(IsLabelEditingProperty); }
            set { SetValue(IsLabelEditingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsLabelEditing.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty IsLabelEditingProperty =
            DependencyProperty.Register("IsLabelEditing", typeof(bool), typeof(MapSymbols), new PropertyMetadata(false));


        #endregion

        #region Symbol Label Width

        /// <summary>
        /// Gets or sets LabelWidth of the Symbols.
        /// </summary>
        /// <value>
        /// double
        /// </value>
        public double SymbolLabelWidth
        {
            get { return (double)GetValue(SymbolLabelWidthProperty); }
            set { SetValue(SymbolLabelWidthProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for SymbolLabelWidth.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SymbolLabelWidthProperty =
            DependencyProperty.Register("SymbolLabelWidth", typeof(double), typeof(MapSymbols), new PropertyMetadata(double.NaN));


        #endregion

        #region IsSelected

        /// <summary>
        /// Gets or sets a value indicating whether the symbol is selected or not .
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
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(MapSymbols), new PropertyMetadata(false));


        #endregion

        #region IsHover

        internal bool IsHover
        {
            get { return (bool)GetValue(IsHoverProperty); }
            set { SetValue(IsHoverProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsHover.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty IsHoverProperty =
            DependencyProperty.Register("IsHover", typeof(bool), typeof(MapSymbols), new PropertyMetadata(false));

        #endregion

        #endregion

        #region Override Functions

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or
        /// internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.ShapeLayer = MapControl.FindParent<ShapeFileLayer>(this);
            this.symbollabelTextBox = this.GetTemplateChild("PART_SymbolLabelTextBox") as TextBox;
            this.symbollabelTextBox.KeyDown += new KeyEventHandler(symbollabelTextBox_KeyDown);
            this.symbolMapLabelTextBlock = this.GetTemplateChild("PART_SymbolTextBlock") as TextBlock;
            this.symbollabelTextBox.LostFocus += new RoutedEventHandler(symbollabelTextBox_LostFocus);
        }

        void symbollabelTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            this.IsLabelEditing = false;
            this.ShapeLayer.ClickedItem = null;
            this.ShapeLayer.mapControl.isLabelEdited = true; 
        }

        #endregion

        #region Event Handlers

        void MapSymbols_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.isDragged = true;
            this.isMousedown = true;
            this.mousePosition = e.GetPosition(this);
            this.startlong = this.Longitude;
            this.startlat = this.Latitude;
            if (this.ShapeLayer.SelectedMapSymbol != null)
            {
                if (!sender.Equals(this.ShapeLayer.SelectedMapSymbol))
                {
                    this.CheckSelection();
                    this.ShapeLayer.SelectedMapSymbol = sender as MapSymbols;
                    this.IsSelected = true;
                }
            }
            else
            {
                this.ShapeLayer.SelectedMapSymbol = sender as MapSymbols;
                this.IsSelected = true;
            }

            if (Keyboard.Modifiers == ModifierKeys.Shift && this.ShapeLayer.ClickedItem == null)
            {
                this.ShapeLayer.ClickedItem = "Symbol";
                this.templabel = this.SymbolText;
                this.IsShiftPressed = true;
                this.IsLabelEditing = true;
                this.symbollabelTextBox.Focus();
                this.symbollabelTextBox.SelectAll();
            }
            else if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                this.ShapeLayer.SymbolCollection.Remove(sender as MapSymbols);
                this.ShapeLayer.SelectedMapSymbol = null;
            }
            e.Handled = true;
        }       

        void MapSymbols_MouseMove(object sender, MouseEventArgs e)
        {
            this.IsHover = true;
        }

        void MapSymbols_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.isMousedown = false;
            this.isDragged = true;
            if (this.ShapeLayer.SelectedMapSymbol != null)
            {
                if (this.ShapeLayer.SelectedMapSymbol.CheckSymbolIntersection())
                {
                    this.ShapeLayer.SelectedMapSymbol.Latitude = this.ShapeLayer.SelectedMapSymbol.startlat;
                    this.ShapeLayer.SelectedMapSymbol.Longitude = this.ShapeLayer.SelectedMapSymbol.startlong;
                }
            }

        }

        void symbollabelTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.SymbolDescription = (sender as TextBox).Text;
                this.ShapeLayer.ClickedItem = null;
                this.IsLabelEditing = false;
                this.CheckSymbolIntersection();
            }
            else if (e.Key == Key.Escape)
            {
                if (this.IsShiftPressed)
                {
                    (sender as TextBox).Text = this.templabel;
                    this.SymbolText = this.templabel;
                    this.IsShiftPressed = false;
                }
                else
                {
                    this.ShapeLayer.SymbolCollection.Remove(this);
                }
                this.IsLabelEditing = false;
                this.ShapeLayer.ClickedItem = null;
            }
            this.isMousedown = false;

        }

        void MapSymbols_Loaded(object sender, RoutedEventArgs e)
        {
            this.CheckSymbolIntersection();
        }

        #endregion

        #region Helper Methods


        internal void CheckSelection(ShapeFileLayer ShapeLayer)
        {

            var prevSeleted = from symbols in ShapeLayer.SymbolCollection
                              where symbols.IsSelected
                              select symbols;
            if (prevSeleted.ToList().Count > 0)
            {
                foreach (MapSymbols sym in prevSeleted.ToList())
                {
                    sym.IsSelected = false;
                }
            }
            ShapeLayer.SelectedMapSymbol = null;

        }

        internal void CheckSelection()
        {
            this.CheckSelection(this.ShapeLayer);
        }

        private Rect Findintersect(Rect r1, Rect r2)
        {
            r1.Intersect(r2);
            return r1;
        }
        internal bool CheckSymbolIntersection()
        {
            if (this.ShapeLayer == null)
            {
                this.ShapeLayer = MapControl.FindParent<ShapeFileLayer>(this);
            }
            if (this.ShapeLayer != null)
            {
                Rect r4 = Rect.Empty;
                Rect r3 = Rect.Empty;
                Rect r5 = Rect.Empty;
                Rect r6 = Rect.Empty;
                Rect r = new Rect(this.ShapeLayer.LatitudeLongitudeToPoint(new Point(this.Longitude, this.Latitude)), new Size(this.DesiredSize.Width * this.ShapeLayer.ZoomFactor, this.DesiredSize.Height * this.ShapeLayer.ZoomFactor));
                var element = from maplabel in this.ShapeLayer.SymbolCollection
                              where (this.Findintersect(r, new Rect(this.ShapeLayer.LatitudeLongitudeToPoint(new Point((maplabel as MapSymbols).Longitude, (maplabel as MapSymbols).Latitude)), new Size((maplabel as MapSymbols).DesiredSize.Width * this.ShapeLayer.ZoomFactor, (maplabel as MapSymbols).DesiredSize.Height * this.ShapeLayer.ZoomFactor))) != Rect.Empty && !maplabel.Equals(this))
                              select maplabel;
                var labelelement = from maplabel in this.ShapeLayer.LabelCollections
                                   where (this.Findintersect(r, new Rect(this.ShapeLayer.LatitudeLongitudeToPoint(new Point((maplabel as MapLabel).Longitude, (maplabel as MapLabel).Latitude)), new Size((maplabel as MapLabel).DesiredSize.Width * this.ShapeLayer.ZoomFactor, (maplabel as MapLabel).DesiredSize.Height * this.ShapeLayer.ZoomFactor))) != Rect.Empty)
                                   select maplabel;

                if (labelelement.ToList().Count > 0)
                {
                    this.maplbl = labelelement.ToList()[0] as MapLabel;
                }
                if (element.ToList().Count > 0)
                {
                    mapsym = element.ToList()[0] as MapSymbols;
                }
                if (maplbl != null)
                {
                    r5 = new Rect(this.ShapeLayer.LatitudeLongitudeToPoint(new Point((maplbl as MapLabel).Longitude, (maplbl as MapLabel).Latitude)), new Size((maplbl as MapLabel).DesiredSize.Width * this.ShapeLayer.ZoomFactor, (maplbl as MapLabel).DesiredSize.Height * this.ShapeLayer.ZoomFactor));
                    r6 = this.Findintersect(r, r5);
                }
                if (mapsym != null)
                {
                    r3 = new Rect(this.ShapeLayer.LatitudeLongitudeToPoint(new Point((mapsym as MapSymbols).Longitude, (mapsym as MapSymbols).Latitude)), new Size((mapsym as MapSymbols).DesiredSize.Width * this.ShapeLayer.ZoomFactor, (mapsym as MapSymbols).DesiredSize.Height * this.ShapeLayer.ZoomFactor));
                    r4 = this.Findintersect(r, r3);
                }

                if ((r4 != Rect.Empty && r3 != Rect.Empty) || (r5 != Rect.Empty && r6 != Rect.Empty))
                {
                    if (!this.ShapeLayer.AllowLabelIntersection)
                    {
                        if (!this.isDragged)
                        {
                            if (this.IsShiftPressed)
                            {
                                this.SymbolText = this.templabel;
                            }
                            else
                            {
                                this.ShapeLayer.SymbolCollection.Remove(this.ShapeLayer.SelectedMapSymbol);
                                this.ShapeLayer.drawingCanvas.Children.Remove(this.ShapeLayer.SelectedMapSymbol);
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
            else
            {
                return true;
            }
        }

        #endregion

    }
}
