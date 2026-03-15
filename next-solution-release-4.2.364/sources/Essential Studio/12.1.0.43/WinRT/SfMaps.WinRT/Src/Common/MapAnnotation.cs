#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.UI.Xaml.Maps
{
    using System;
#if WINRT
    using Windows.UI;
    using Windows.UI.Text;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Media;
    using Windows.Foundation;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media.Imaging;
    using Windows.UI.Xaml.Shapes;
#else
    using System.Windows.Media;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media.Imaging;
    using System.Windows.Shapes;
#endif

    /// <summary>
    /// Represents the MapAnnotation class in the SfMap.
    /// </summary>
    /// <example>
    /// <code>
    /// using Syncfusion.UI.Xaml.Maps;
    /// using System;
    /// using System.Collections.Generic;
    /// using System.IO;
    /// using System.Linq;
    /// using Windows.Foundation;
    /// using Windows.Foundation.Collections;
    /// using Windows.UI;
    /// using Windows.UI.Xaml;
    /// using Windows.UI.Xaml.Controls;
    /// using Windows.UI.Xaml.Controls.Primitives;
    /// using Windows.UI.Xaml.Data;
    /// using Windows.UI.Xaml.Input;
    /// using Windows.UI.Xaml.Media;
    /// using Windows.UI.Xaml.Navigation;
    /// using Windows.UI.Xaml.Shapes;
    /// 
    /// 
    /// namespace MapApp
    /// {
    ///     public sealed partial class MainPage : Page
    ///     {
    ///         public MainPage()
    ///         {
    ///             this.InitializeComponent();
    ///             SfMap syncMap = new SfMap();
    ///             ShapeFileLayer layer = new ShapeFileLayer();
    ///             layer.Uri = "MapApp.world1.shp";
    ///             MapAnnotations annotation = new MapAnnotations();
    ///             annotation.AnnotationSymbol = new Rectangle { Height = 50, Width = 50, Fill = new SolidColorBrush(Colors.Aqua) };
    ///             annotation.AnnotationLabel = "SfMap Annotation";
    ///             annotation.Latitude = 30;
    ///             annotation.Longitude = 20;
    ///             annotation.AnnotationLabelBackground = new SolidColorBrush(Colors.Azure);
    ///             annotation.AnnotationLabelFontFamily = new Windows.UI.Xaml.Media.FontFamily("Comic Sans MS");
    ///             annotation.AnnotationLabelFontSize = 10d;
    ///             annotation.AnnotationLabelFontStyle = FontStyle.Normal;
    ///             annotation.AnnotationLabelForeground = new SolidColorBrush(Colors.Black);
    ///             layer.Annotations.Add(annotation);
    ///             syncMap.Layers.Add(layer);
    /// 
    ///         }
    /// 
    ///     }
    /// 
    /// 
    /// }
    /// 
    /// </code>
    /// </example>
    [ClassReference(IsReviewed = false)]
    public class MapAnnotations : DependencyObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.UI.Xaml.Maps.MapAnnotations">MapAnnotations</see> class. 
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public MapAnnotations()
        {
        }
        #region Properties

        internal Point midpoint = new Point();
        #region Symbol(Dependency Property)

        /// <summary>
        /// Gets or sets the Symbol to be displayed on the Annotation.
        /// </summary>
        /// <value>
        /// Type :<see cref="UIElement"/>
        /// </value>
        /// <remarks>
        /// Use this property to set the symbol for the annotation.
        /// </remarks>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.UI.Xaml.Maps;
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// using Windows.UI.Xaml.Shapes;
        /// 
        /// 
        /// namespace MapApp
        /// {
        ///     public sealed partial class MainPage : Page
        ///     {
        ///         public MainPage()
        ///         {
        ///             this.InitializeComponent();
        ///             SfMap syncMap = new SfMap();           
        ///             ShapeFileLayer layer = new ShapeFileLayer();
        ///             layer.Uri = "MapApp.world1.shp";
        ///             MapAnnotations annotation = new MapAnnotations();
        ///             annotation.AnnotationSymbol = new Rectangle { Height=50, Width=50, Fill=new SolidColorBrush(Colors.Aqua) };
        ///             annotation.AnnotationLabel = "SfMap Annotation";
        ///             annotation.Latitude = 30;
        ///             annotation.Longitude = 20;
        ///             annotation.AnnotationLabelBackground = new SolidColorBrush(Colors.Azure);
        ///             annotation.AnnotationLabelFontFamily = new Windows.UI.Xaml.Media.FontFamily("Comic Sans MS");
        ///             annotation.AnnotationLabelFontSize = 10d;
        ///             annotation.AnnotationLabelFontStyle = FontStyle.Normal;
        ///             annotation.AnnotationLabelForeground = new SolidColorBrush(Colors.Black);
        ///             layer.Annotations.Add(annotation);
        ///             syncMap.Layers.Add(layer);
        /// 
        ///         }
        /// 
        ///     }
        ///     
        /// 
        /// }
        /// 
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public UIElement AnnotationSymbol
        {
            get { return (UIElement)GetValue(AnnotationSymbolProperty); }
            set { SetValue(AnnotationSymbolProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Symbol.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AnnotationSymbolProperty =
            DependencyProperty.Register("AnnotationSymbol", typeof(UIElement), typeof(MapAnnotations), new PropertyMetadata(null));

        #endregion

        #region Latitude(Dependency Property)

        /// <summary>
        /// Gets or sets the Latitude coordinate of the Annotation.
        /// </summary>
        /// <value>
        /// Type :<see cref="double"/>
        /// </value>
        /// <remarks>
        /// Use this property to arrange the annotation in the particular latitude coordinate.
        /// </remarks>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.UI.Xaml.Maps;
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// using Windows.UI.Xaml.Shapes;
        /// 
        /// 
        /// namespace MapApp
        /// {
        ///     public sealed partial class MainPage : Page
        ///     {
        ///         public MainPage()
        ///         {
        ///             this.InitializeComponent();
        ///             SfMap syncMap = new SfMap();           
        ///             ShapeFileLayer layer = new ShapeFileLayer();
        ///             layer.Uri = "MapApp.world1.shp";
        ///             MapAnnotations annotation = new MapAnnotations();
        ///             annotation.AnnotationSymbol = new Rectangle { Height=50, Width=50, Fill=new SolidColorBrush(Colors.Aqua) };
        ///             annotation.AnnotationLabel = "SfMap Annotation";
        ///             annotation.Latitude = 30;
        ///             annotation.Longitude = 20;
        ///             annotation.AnnotationLabelBackground = new SolidColorBrush(Colors.Azure);
        ///             annotation.AnnotationLabelFontFamily = new Windows.UI.Xaml.Media.FontFamily("Comic Sans MS");
        ///             annotation.AnnotationLabelFontSize = 10d;
        ///             annotation.AnnotationLabelFontStyle = FontStyle.Normal;
        ///             annotation.AnnotationLabelForeground = new SolidColorBrush(Colors.Black);
        ///             layer.Annotations.Add(annotation);
        ///             syncMap.Layers.Add(layer);
        /// 
        ///         }
        /// 
        ///     }
        ///         
        /// }
        /// 
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public double Latitude
        {
            get { return (double)GetValue(LatitudeProperty); }
            set { SetValue(LatitudeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Latitude.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LatitudeProperty =
            DependencyProperty.Register("Latitude", typeof(double), typeof(MapAnnotations), new PropertyMetadata(0d, OnLatitudeChanged));

        private static void OnLatitudeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }
        #endregion

        #region Longitude(Dependency Property)
        /// <summary>
        /// Gets or set the Longitude coordinate of the Annotation.
        /// </summary>
        /// <remarks>
        /// Use this property to arrange the annotation in the particular longitude coordinate.
        /// </remarks>
        /// <value>
        /// Type :<see cref="double"/>
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.UI.Xaml.Maps;
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// using Windows.UI.Xaml.Shapes;
        /// 
        /// 
        /// namespace MapApp
        /// {
        ///     public sealed partial class MainPage : Page
        ///     {
        ///         public MainPage()
        ///         {
        ///             this.InitializeComponent();
        ///             SfMap syncMap = new SfMap();           
        ///             ShapeFileLayer layer = new ShapeFileLayer();
        ///             layer.Uri = "MapApp.world1.shp";
        ///             MapAnnotations annotation = new MapAnnotations();
        ///             annotation.AnnotationSymbol = new Rectangle { Height=50, Width=50, Fill=new SolidColorBrush(Colors.Aqua) };
        ///             annotation.AnnotationLabel = "SfMap Annotation";
        ///             annotation.Latitude = 30;
        ///             annotation.Longitude = 20;
        ///             annotation.AnnotationLabelBackground = new SolidColorBrush(Colors.Azure);
        ///             annotation.AnnotationLabelFontFamily = new Windows.UI.Xaml.Media.FontFamily("Comic Sans MS");
        ///             annotation.AnnotationLabelFontSize = 10d;
        ///             annotation.AnnotationLabelFontStyle = FontStyle.Normal;
        ///             annotation.AnnotationLabelForeground = new SolidColorBrush(Colors.Black);
        ///             layer.Annotations.Add(annotation);
        ///             syncMap.Layers.Add(layer);
        ///         }
        ///     }
        /// }
        /// 
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public double Longitude
        {
            get { return (double)GetValue(LongitudeProperty); }
            set { SetValue(LongitudeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Longitude.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LongitudeProperty =
            DependencyProperty.Register("Longitude", typeof(double), typeof(MapAnnotations), new PropertyMetadata(0d));

        #endregion

        #region AnnotationLabel
        /// <summary>
        /// Gets or set the display label for the Annotations.
        /// </summary>
        /// <value>
        /// Type :<see cref="String"/>
        /// </value>
        /// <remarks>
        /// Use this property to set the display any text on the annotation.
        /// </remarks>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.UI.Xaml.Maps;
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// using Windows.UI.Xaml.Shapes;
        /// 
        /// 
        /// namespace MapApp
        /// {
        ///     public sealed partial class MainPage : Page
        ///     {
        ///         public MainPage()
        ///         {
        ///             this.InitializeComponent();
        ///             SfMap syncMap = new SfMap();           
        ///             ShapeFileLayer layer = new ShapeFileLayer();
        ///             layer.Uri = "MapApp.world1.shp";
        ///             MapAnnotations annotation = new MapAnnotations();
        ///             annotation.AnnotationSymbol = new Rectangle { Height=50, Width=50, Fill=new SolidColorBrush(Colors.Aqua) };
        ///             annotation.AnnotationLabel = "SfMap Annotation";
        ///             annotation.Latitude = 30;
        ///             annotation.Longitude = 20;
        ///             annotation.AnnotationLabelBackground = new SolidColorBrush(Colors.Azure);
        ///             annotation.AnnotationLabelFontFamily = new Windows.UI.Xaml.Media.FontFamily("Comic Sans MS");
        ///             annotation.AnnotationLabelFontSize = 10d;
        ///             annotation.AnnotationLabelFontStyle = FontStyle.Normal;
        ///             annotation.AnnotationLabelForeground = new SolidColorBrush(Colors.Black);
        ///             layer.Annotations.Add(annotation);
        ///             syncMap.Layers.Add(layer);
        ///         }
        ///     }
        /// }
        /// 
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public string AnnotationLabel
        {
            get { return (string)GetValue(AnnotationLabelProperty); }
            set { SetValue(AnnotationLabelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SymbolText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AnnotationLabelProperty =
            DependencyProperty.Register("AnnotationLabel", typeof(string), typeof(MapAnnotations), new PropertyMetadata(""));

        #endregion

        #region LabelForeground(Dependency Property)

        /// <summary>
        /// Gets or sets the Foreground Color for the Labels on the annotation.
        /// </summary>
        /// <value>
        /// Type :<see cref="Brush"/>
        /// </value>
        /// <remarks>
        /// Use this property to set the foreground color for the annotation label.
        /// </remarks>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.UI.Xaml.Maps;
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// using Windows.UI.Xaml.Shapes;
        /// 
        /// 
        /// namespace MapApp
        /// {
        ///     public sealed partial class MainPage : Page
        ///     {
        ///         public MainPage()
        ///         {
        ///             this.InitializeComponent();
        ///             SfMap syncMap = new SfMap();           
        ///             ShapeFileLayer layer = new ShapeFileLayer();
        ///             layer.Uri = "MapApp.world1.shp";
        ///             MapAnnotations annotation = new MapAnnotations();
        ///             annotation.AnnotationSymbol = new Rectangle { Height=50, Width=50, Fill=new SolidColorBrush(Colors.Aqua) };
        ///             annotation.AnnotationLabel = "SfMap Annotation";
        ///             annotation.Latitude = 30;
        ///             annotation.Longitude = 20;
        ///             annotation.AnnotationLabelBackground = new SolidColorBrush(Colors.Azure);
        ///             annotation.AnnotationLabelFontFamily = new Windows.UI.Xaml.Media.FontFamily("Comic Sans MS");
        ///             annotation.AnnotationLabelFontSize = 10d;
        ///             annotation.AnnotationLabelFontStyle = FontStyle.Normal;
        ///             annotation.AnnotationLabelForeground = new SolidColorBrush(Colors.Black);
        ///             layer.Annotations.Add(annotation);
        ///             syncMap.Layers.Add(layer);
        ///         }
        ///     }
        /// }
        /// 
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public Brush AnnotationLabelForeground
        {
            get { return (Brush)GetValue(AnnotationLabelForegroundProperty); }
            set { SetValue(AnnotationLabelForegroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LabelForeground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AnnotationLabelForegroundProperty =
            DependencyProperty.Register("AnnotationLabelForeground", typeof(Brush), typeof(MapAnnotations), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        #endregion

        #region AnnotationLabelFontFamily(Dependency Property)

        /// <summary>
        /// Gets or sets the font family of the Labels annotation.
        /// </summary>
        /// <value>
        /// Type :<see cref="FontFamily"/>
        /// </value>
        /// <remarks>
        /// Use this property to set the Font family for the map annotations.
        /// </remarks>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.UI.Xaml.Maps;
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// using Windows.UI.Xaml.Shapes;
        /// 
        /// 
        /// namespace MapApp
        /// {
        ///     public sealed partial class MainPage : Page
        ///     {
        ///         public MainPage()
        ///         {
        ///             this.InitializeComponent();
        ///             SfMap syncMap = new SfMap();           
        ///             ShapeFileLayer layer = new ShapeFileLayer();
        ///             layer.Uri = "MapApp.world1.shp";
        ///             MapAnnotations annotation = new MapAnnotations();
        ///             annotation.AnnotationSymbol = new Rectangle { Height=50, Width=50, Fill=new SolidColorBrush(Colors.Aqua) };
        ///             annotation.AnnotationLabel = "SfMap Annotation";
        ///             annotation.Latitude = 30;
        ///             annotation.Longitude = 20;
        ///             annotation.AnnotationLabelBackground = new SolidColorBrush(Colors.Azure);
        ///             annotation.AnnotationLabelFontFamily = new Windows.UI.Xaml.Media.FontFamily("Comic Sans MS");
        ///             annotation.AnnotationLabelFontSize = 10d;
        ///             annotation.AnnotationLabelFontStyle = FontStyle.Normal;
        ///             annotation.AnnotationLabelForeground = new SolidColorBrush(Colors.Black);
        ///             layer.Annotations.Add(annotation);
        ///             syncMap.Layers.Add(layer);
        ///         }
        ///     }
        /// }
        /// 
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public FontFamily AnnotationLabelFontFamily
        {
            get { return (FontFamily)GetValue(AnnotationLabelFontFamilyProperty); }
            set { SetValue(AnnotationLabelFontFamilyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LabelFontFamily.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AnnotationLabelFontFamilyProperty =
            DependencyProperty.Register("AnnotationLabelFontFamily", typeof(FontFamily), typeof(MapAnnotations), new PropertyMetadata(new FontFamily("Times New Roman")));


        #endregion

        #region AnnotationLabelBackground(Dependency Property)

        /// <summary>
        /// Gets or sets Background color of the annotation label.
        /// </summary>
        /// <value>
        /// Type :<see cref="Brush"/>
        /// </value>
        /// <remarks>
        /// Use this property to set the background color of the annotation label.
        /// </remarks>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.UI.Xaml.Maps;
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// using Windows.UI.Xaml.Shapes;
        /// 
        /// 
        /// namespace MapApp
        /// {
        ///     public sealed partial class MainPage : Page
        ///     {
        ///         public MainPage()
        ///         {
        ///             this.InitializeComponent();
        ///             SfMap syncMap = new SfMap();           
        ///             ShapeFileLayer layer = new ShapeFileLayer();
        ///             layer.Uri = "MapApp.world1.shp";
        ///             MapAnnotations annotation = new MapAnnotations();
        ///             annotation.AnnotationSymbol = new Rectangle { Height=50, Width=50, Fill=new SolidColorBrush(Colors.Aqua) };
        ///             annotation.AnnotationLabel = "SfMap Annotation";
        ///             annotation.Latitude = 30;
        ///             annotation.Longitude = 20;
        ///             annotation.AnnotationLabelBackground = new SolidColorBrush(Colors.Azure);
        ///             annotation.AnnotationLabelFontFamily = new Windows.UI.Xaml.Media.FontFamily("Comic Sans MS");
        ///             annotation.AnnotationLabelFontSize = 10d;
        ///             annotation.AnnotationLabelFontStyle = FontStyle.Normal;
        ///             annotation.AnnotationLabelForeground = new SolidColorBrush(Colors.Black);
        ///             layer.Annotations.Add(annotation);
        ///             syncMap.Layers.Add(layer);
        ///         }
        ///     }
        /// }
        /// 
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public Brush AnnotationLabelBackground
        {
            get { return (Brush)GetValue(AnnotationLabelBackgroundProperty); }
            set { SetValue(AnnotationLabelBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LabelBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AnnotationLabelBackgroundProperty =
            DependencyProperty.Register("AnnotationLabelBackground", typeof(Brush), typeof(MapAnnotations), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));

        #endregion

        #region AnnotationLabelFontStyle(Dependency Property)

        /// <summary>
        /// Gets or sets Font Style of the  annotation label in the SfMap.
        /// </summary>
        /// <value>
        /// Type :<see cref="FontStyle"/>
        /// </value>
        /// <remarks>
        /// Use this property to set the font style of the annotation label.
        /// </remarks>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.UI.Xaml.Maps;
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// using Windows.UI.Xaml.Shapes;
        /// 
        /// 
        /// namespace MapApp
        /// {
        ///     public sealed partial class MainPage : Page
        ///     {
        ///         public MainPage()
        ///         {
        ///             this.InitializeComponent();
        ///             SfMap syncMap = new SfMap();           
        ///             ShapeFileLayer layer = new ShapeFileLayer();
        ///             layer.Uri = "MapApp.world1.shp";
        ///             MapAnnotations annotation = new MapAnnotations();
        ///             annotation.AnnotationSymbol = new Rectangle { Height=50, Width=50, Fill=new SolidColorBrush(Colors.Aqua) };
        ///             annotation.AnnotationLabel = "SfMap Annotation";
        ///             annotation.Latitude = 30;
        ///             annotation.Longitude = 20;
        ///             annotation.AnnotationLabelBackground = new SolidColorBrush(Colors.Azure);
        ///             annotation.AnnotationLabelFontFamily = new Windows.UI.Xaml.Media.FontFamily("Comic Sans MS");
        ///             annotation.AnnotationLabelFontSize = 10d;
        ///             annotation.AnnotationLabelFontStyle = FontStyle.Normal;
        ///             annotation.AnnotationLabelForeground = new SolidColorBrush(Colors.Black);
        ///             layer.Annotations.Add(annotation);
        ///             syncMap.Layers.Add(layer);
        ///         }
        ///     }
        /// }
        /// 
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public FontStyle AnnotationLabelFontStyle
        {
            get { return (FontStyle)GetValue(AnnotationLabelFontStyleProperty); }
            set { SetValue(AnnotationLabelFontStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LabelFontStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AnnotationLabelFontStyleProperty =
            DependencyProperty.Register("AnnotationLabelFontStyle", typeof(FontStyle), typeof(MapAnnotations), new PropertyMetadata(null));

        #endregion

        #region AnnotationLabelFontSize(Dependency Property)

        /// <summary>
        /// Gets or sets the font size of the annotation label in the SfMap.
        /// </summary>
        /// <value>
        /// <code language="C#">
        /// using Syncfusion.UI.Xaml.Maps;
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// using Windows.UI.Xaml.Shapes;
        /// 
        /// 
        /// namespace MapApp
        /// {
        ///     public sealed partial class MainPage : Page
        ///     {
        ///         public MainPage()
        ///         {
        ///             this.InitializeComponent();
        ///             SfMap syncMap = new SfMap();           
        ///             ShapeFileLayer layer = new ShapeFileLayer();
        ///             layer.Uri = "MapApp.world1.shp";
        ///             MapAnnotations annotation = new MapAnnotations();
        ///             annotation.AnnotationSymbol = new Rectangle { Height=50, Width=50, Fill=new SolidColorBrush(Colors.Aqua) };
        ///             annotation.AnnotationLabel = "SfMap Annotation";
        ///             annotation.Latitude = 30;
        ///             annotation.Longitude = 20;
        ///             annotation.AnnotationLabelBackground = new SolidColorBrush(Colors.Azure);
        ///             annotation.AnnotationLabelFontFamily = new Windows.UI.Xaml.Media.FontFamily("Comic Sans MS");
        ///             annotation.AnnotationLabelFontSize = 10d;
        ///             annotation.AnnotationLabelFontStyle = FontStyle.Normal;
        ///             annotation.AnnotationLabelForeground = new SolidColorBrush(Colors.Black);
        ///             layer.Annotations.Add(annotation);
        ///             syncMap.Layers.Add(layer);
        ///         }
        ///     }
        /// }
        /// 
        /// </code>
        /// </value>
        [ClassReference(IsReviewed = false)]
        public double AnnotationLabelFontSize
        {
            get { return (double)GetValue(AnnotationLabelFontSizeProperty); }
            set { SetValue(AnnotationLabelFontSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LabelFontSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AnnotationLabelFontSizeProperty =
            DependencyProperty.Register("AnnotationLabelFontSize", typeof(double), typeof(MapAnnotations), new PropertyMetadata(12d));

        #endregion

        #region AnnotationMargin



        internal Thickness AnnotationMargin
        {
            get { return (Thickness)GetValue(AnnotationMarginProperty); }
            set { SetValue(AnnotationMarginProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SymbolMargin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AnnotationMarginProperty =
            DependencyProperty.Register("AnnotationMargin", typeof(Thickness), typeof(MapAnnotations), new PropertyMetadata(new Thickness(0, 0, 0, 0)));




        #endregion

        #region SymbolTemplate


        /// <summary>
        /// Gets  template for the map annotations.
        /// </summary>
        /// <remarks>
        /// This is read-only property to read the template for annotations.
        /// </remarks>
        /// <value>
        /// Type :<see cref="DataTemplate"/>
        /// </value>
        [ClassReference(IsReviewed = false)]
        public DataTemplate AnnotationTemplate
        {
            get { return (DataTemplate)GetValue(AnnotationTemplateProperty); }
            internal set { SetValue(AnnotationTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SymbolTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AnnotationTemplateProperty =
            DependencyProperty.Register("SymbolTemplate", typeof(DataTemplate), typeof(MapAnnotations), new PropertyMetadata(null));




        #endregion

        #endregion

        #region Implementation

        internal void SetAnnotationSymbolWithKmlIcon(KmlStyle kmlStyle)
        {
            if (kmlStyle != null)
            {
                AnnotationSymbol = new Image
                {
                    Stretch = Stretch.None,
#if SILVERLIGHT
                    Source = new BitmapImage(new Uri(kmlStyle.IconStyle.IconUrl)),
#else
                    Source = new BitmapImage(new Uri(kmlStyle.IconStyle.IconUrl, UriKind.RelativeOrAbsolute)),
#endif
                    IsHitTestVisible = false,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top
                };
            }
            else
            {
                AnnotationSymbol = new Ellipse
                {
                    Height = 20,
                    Width = 20,
                    Fill = new SolidColorBrush(Colors.Yellow),
                    Stroke = new SolidColorBrush(Colors.Black),
                    StrokeThickness = 2
                };
                //AnnotationLabel = "Longitude :" + Longitude + "\nLatitude" + Latitude;
            }
        }

        #endregion
    }
}
