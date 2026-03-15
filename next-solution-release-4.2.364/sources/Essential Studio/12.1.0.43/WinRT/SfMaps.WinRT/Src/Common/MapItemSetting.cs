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
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    
#if WINRT
    using Windows.UI;
    using Windows.UI.Text;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Shapes;
    using System.Threading.Tasks;
#else
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Shapes;
#endif


    /// <summary>
    /// Represents the MapItemSetting in the map.
    /// </summary>
    /// <remarks>
    /// MapItemSetting class contains the properties to customize the map items, when default template is applied on the MapItem.
    /// </remarks>
    /// <example>
    /// <para>Refer the following code to know how MapItemSetting is defined.</para>
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
    /// 
    /// 
    /// namespace MapApp
    /// {
    /// 
    ///     public sealed partial class MainPage : Page
    ///     {
    ///         public MainPage()
    ///         {
    ///             this.InitializeComponent();
    ///             SfMap syncMap = new SfMap();
    ///             MapItemSetting mapItemSetting = new MapItemSetting();
    ///             mapItemSetting.MapItemFontFamily = new Windows.UI.Xaml.Media.FontFamily("Times New Roman");
    ///             mapItemSetting.MapItemFontSize = 10d;
    ///             mapItemSetting.MapItemForeground = new SolidColorBrush(Colors.OldLace);
    ///             ShapeFileLayer layer = new ShapeFileLayer();
    ///             layer.MapItemSetting = mapItemSetting;
    ///             layer.Uri = "MapApp.world.shp";
    ///             syncMap.Layers.Add(layer);          
    /// 
    ///         }
    /// 
    ///     }
    /// } 
    /// </code>
    /// </example>
    [ClassReference(IsReviewed = false)]
    public class MapItemSetting : DependencyObject
    {
        #region Internal Fields

        internal bool canSizeSymbol = false;

        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.UI.Xaml.Maps.MapItemSetting">MapItemSetting</see> class. 
        /// </summary>        
        [ClassReference(IsReviewed = false)]
        public MapItemSetting()
        {

        }
        #endregion

        #region Properties

        #region LabelForeground(Dependency Property)

        /// <summary>
        /// Gets or sets the Foreground Color for the Labels on the SfMap.
        /// </summary>
        /// <value>
        /// Type :<see cref="Brush"/>
        /// </value>
        /// <remarks>
        /// Use MapItemForeground property to set the foreground color for the map item.
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
        /// 
        /// 
        /// namespace MapApp
        /// {
        /// 
        ///     public sealed partial class MainPage : Page
        ///     {
        ///         public MainPage()
        ///         {
        ///             this.InitializeComponent();
        ///             SfMap syncMap = new SfMap();
        ///             MapItemSetting mapItemSetting = new MapItemSetting();        
        ///             mapItemSetting.MapItemForeground = new SolidColorBrush(Colors.OldLace);
        ///             ShapeFileLayer layer = new ShapeFileLayer();
        ///             layer.MapItemSetting = mapItemSetting;
        ///             layer.Uri = "MapApp.world.shp";
        ///             syncMap.Layers.Add(layer);          
        /// 
        ///         }
        /// 
        ///     }
        /// } 
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public Brush MapItemForeground
        {
            get { return (Brush)GetValue(MapItemForegroundProperty); }
            set { SetValue(MapItemForegroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LabelForeground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MapItemForegroundProperty =
            DependencyProperty.Register("MapItemForeground", typeof(Brush), typeof(MapItemSetting), new PropertyMetadata(new SolidColorBrush(Colors.White)));

        #endregion

        #region MapItemFontFamily(Dependency Property)

        /// <summary>
        /// Gets or sets the font family for the map item Labels on the SfMap.
        /// </summary>
        /// <value>
        /// Type :<see cref="FontFamily"/>
        /// </value>
        /// <remarks>
        /// Use MapItemForeground property to set the font family for the map item.
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
        /// 
        /// 
        /// namespace MapApp
        /// {
        /// 
        ///     public sealed partial class MainPage : Page
        ///     {
        ///         public MainPage()
        ///         {
        ///             this.InitializeComponent();
        ///             SfMap syncMap = new SfMap();
        ///             MapItemSetting mapItemSetting = new MapItemSetting();        
        ///             mapItemSetting.MapItemFontFamily = new Windows.UI.Xaml.Media.FontFamily("Times New Roman");
        ///             ShapeFileLayer layer = new ShapeFileLayer();
        ///             layer.MapItemSetting = mapItemSetting;
        ///             layer.Uri = "MapApp.world.shp";
        ///             syncMap.Layers.Add(layer);          
        /// 
        ///         }
        /// 
        ///     }
        /// } 
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public FontFamily MapItemFontFamily
        {
            get { return (FontFamily)GetValue(SymbolLabelFontFamilyProperty); }
            set { SetValue(SymbolLabelFontFamilyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LabelFontFamily.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SymbolLabelFontFamilyProperty =
            DependencyProperty.Register("SymbolLabelFontFamily", typeof(FontFamily), typeof(MapItemSetting), new PropertyMetadata(new FontFamily("Segoe UI")));


        #endregion

        #region MapItemFontStyle(Dependency Property)

        /// <summary>
        /// Gets or sets the font style for the map item Labels on the SfMap.
        /// </summary>
        /// <value>
        /// Type :<see cref="FontStyle"/>
        /// </value>
        /// <remarks>
        /// Use MapItemForeground property to set the font style color for the map item.
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
        /// 
        /// 
        /// namespace MapApp
        /// {
        /// 
        ///     public sealed partial class MainPage : Page
        ///     {
        ///         public MainPage()
        ///         {
        ///             this.InitializeComponent();
        ///             SfMap syncMap = new SfMap();
        ///             MapItemSetting mapItemSetting = new MapItemSetting();        
        ///             mapItemSetting.MapItemFontStyle = FontStyle.Normal;
        ///             ShapeFileLayer layer = new ShapeFileLayer();
        ///             layer.MapItemSetting = mapItemSetting;
        ///             layer.Uri = "MapApp.world.shp";
        ///             syncMap.Layers.Add(layer);          
        /// 
        ///         }
        /// 
        ///     }
        /// } 
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public FontStyle MapItemFontStyle
        {
            get { return (FontStyle)GetValue(MapItemFontStyleProperty); }
            set { SetValue(MapItemFontStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LabelFontStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MapItemFontStyleProperty =
            DependencyProperty.Register("MapItemFontStyle", typeof(FontStyle), typeof(MapItemSetting), new PropertyMetadata(null));

        #endregion

        #region MapItemFontSize(Dependency Property)

        /// <summary>
        /// Gets or sets the font size for the map item Labels on the SfMap.
        /// </summary>
        /// <value>
        /// Type :<see cref="double"/>
        /// </value>
        /// <remarks>
        /// Use MapItemForeground property to set the font size  for the map item.
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
        /// 
        /// 
        /// namespace MapApp
        /// {
        /// 
        ///     public sealed partial class MainPage : Page
        ///     {
        ///         public MainPage()
        ///         {
        ///             this.InitializeComponent();
        ///             SfMap syncMap = new SfMap();
        ///             MapItemSetting mapItemSetting = new MapItemSetting();        
        ///             mapItemSetting.MapItemFontSize = 20d;
        ///             ShapeFileLayer layer = new ShapeFileLayer();
        ///             layer.MapItemSetting = mapItemSetting;
        ///             layer.Uri = "MapApp.world.shp";
        ///             syncMap.Layers.Add(layer);          
        /// 
        ///         }
        /// 
        ///     }
        /// } 
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public double MapItemFontSize
        {
            get { return (double)GetValue(MapItemFontSizeProperty); }
            set { SetValue(MapItemFontSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LabelFontSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MapItemFontSizeProperty =
            DependencyProperty.Register("MapItemFontSize", typeof(double), typeof(MapItemSetting), new PropertyMetadata(12d));

        #endregion



        #endregion
    }
}
