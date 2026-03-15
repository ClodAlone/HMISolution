#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

using System.Collections.ObjectModel;
#if WINRT
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Foundation.Collections;
using Windows.Foundation;
using Windows.UI.Xaml.Media;
using Windows.Devices.Input;
using Windows.UI.Input;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI;
using Windows.UI.Xaml.Controls.Primitives;
#else
using System.Windows.Media;
using System.Windows.Data;
using System.Windows.Media.Animation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
#endif
using System.Diagnostics;
using System.Windows.Input;
using System.Windows;

namespace Syncfusion.UI.Xaml.Maps
{
    /// <summary>
    /// <toolboxitem>true</toolboxitem>
    /// <toolboxvscategory>Syncfusion controls for Metro</toolboxvscategory>
    /// <toolboxblendcategory>Syncfusion controls for Metro</toolboxblendcategory>
    /// </summary>
    [StyleTypedProperty(Property = "MapStyle", StyleTargetType = typeof(SfMap))]
    public class SfMap : Control
    {
        #region Private Fields

        private Point prevPoint;
        private Point zoomPoint;
        private bool isPinching = false;
#if WINRT
        private GestureRecognizer gesture = new GestureRecognizer();
#endif
        private Point zoompt;

        private TimeSpan zoomTime;
        private double zoomval;
#if WINRT
        private double prevDelta = 0d;
        private bool isZoomed = false;
        private bool cancelZoom = false;
        private Point offsetPoint;
#endif
        private bool isPan = false;
        private Storyboard zoominanimation = new Storyboard();
        private Storyboard zoomoutanimation = new Storyboard();
        private TextBlock coordLabel;


        #endregion

        #region Internal Fields
#if WINRT
        internal bool isMousePointerPressed;
        internal ScrollViewer scrollContent;
#endif
        internal bool isPointerPressed;
        internal int scrollFactor = 1;
        internal ShapeFileLayer currentLayer;
        internal Grid popupGrid;
        internal Popup popup;
        internal bool isLayerChanged = false;
        internal Point CurrentPoint;
        internal Point startPoint;
        internal Point panTranslatePoint;
        internal Point commandZoomPoint;
        internal Point differencePoint;
        internal bool isManipulteDelta = false;

        #endregion

        #region Events

        #region ZoomedInEvent

        /// <summary>
        /// Occurs when zooming the map. 
        /// </summary>
        /// <remarks>
        /// ZoomedIn event will be triggered when zooming in the map. Use this event handler to do any operation when zooming the map.The event argument contains the Latitude, Longitude values and ZoomLevel.
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
        ///     public sealed partial class MainPage : Page
        ///     {
        ///         public MainPage()
        ///         {
        ///             this.InitializeComponent();
        ///             SfMap syncMap = new SfMap();
        ///             ShapeFileLayer layer = new ShapeFileLayer();
        ///             layer.Uri = "MapApp.world1.shp";            
        ///             syncMap.Layers.Add(layer);
        ///             syncMap.ZoomedIn += syncMap_ZoomedIn;
        ///         }
        /// 
        ///         void syncMap_ZoomedIn(object sender, ZoomEventArgs args)
        ///         {
        ///             var latitude = args.Latitude;
        ///             var longitude = args.Longitude;
        ///             var zoomlevel = args.ZoomLevel;
        ///         }
        ///     }
        /// 
        /// }
        /// 
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public event ZoomEventHandler ZoomedIn;

        internal void OnZoomedIn(object sender, ZoomEventArgs args)
        {
            if (ZoomedIn != null)
            {
                ZoomedIn(sender, args);
            }
        }

        #endregion

        #region ZoomedOutEvent
        /// <summary>
        /// Occurs when zoomed out the map. 
        /// </summary>
        /// <remarks>
        /// ZoomedOut event will be triggered when zooming out the map. Use this event handler to do any operation when zooming out the map.The event argument contains the Latitude, Longitude values and ZoomLevel.
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
        ///     public sealed partial class MainPage : Page
        ///     {
        ///         public MainPage()
        ///         {
        ///             this.InitializeComponent();
        ///             SfMap syncMap = new SfMap();
        ///             ShapeFileLayer layer = new ShapeFileLayer();
        ///             layer.Uri = "MapApp.world1.shp";            
        ///             syncMap.Layers.Add(layer);
        ///             syncMap.ZoomedOut += syncMap_ZoomedOut;
        ///         }
        /// 
        ///         void syncMap_ZoomedOut(object sender, ZoomEventArgs args)
        ///         {
        ///             var latitude = args.Latitude;
        ///             var longitude = args.Longitude;
        ///             var zoomlevel = args.ZoomLevel;
        ///         }
        ///        
        ///     }
        /// 
        /// }
        /// 
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public event ZoomEventHandler ZoomedOut;

        internal void OnZoomedOut(object sender, ZoomEventArgs args)
        {
            if (ZoomedOut != null)
            {
                ZoomedOut(sender, args);
            }
        }

        #endregion

        #region PannedEvent

        /// <summary>
        /// Occurs after panned the map. 
        /// </summary>
        /// <remarks>
        /// Panned event will be triggered after panned the map. Use this event handler to do any operation after the map was panned.The event argument contains the Latitude, Longitude values and and PanningDirection.
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
        ///     public sealed partial class MainPage : Page
        ///     {
        ///         public MainPage()
        ///         {
        ///             this.InitializeComponent();
        ///             SfMap syncMap = new SfMap();
        ///             ShapeFileLayer layer = new ShapeFileLayer();
        ///             layer.Uri = "MapApp.world1.shp";
        ///             syncMap.Layers.Add(layer);
        ///             syncMap.Panned += syncMap_Panned;
        ///         }
        /// 
        ///         void syncMap_Panned(object sender, PanEventArgs args)
        ///         {
        ///             var latitude = args.Latitude;
        ///             var longitude = args.Longitude;
        ///             var direction = args.PanningDirection;
        ///         }
        /// 
        ///     }
        /// 
        /// }
        /// 
        /// 
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public event PanEventHandler Panned;

        internal void OnPanned(object sender, PanEventArgs args)
        {
            if (Panned != null)
            {
                Panned(sender, args);
            }
        }

        #endregion

        #region PanningEvent

        /// <summary>
        /// Occurs while panning the map. 
        /// </summary>
        /// <remarks>
        /// Panned event will be triggered while panning the map. Use this event handler to do any operation while the map is panning.The event argument contains the Latitude, Longitude values and and PanningDirection.
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
        ///     public sealed partial class MainPage : Page
        ///     {
        ///         public MainPage()
        ///         {
        ///             this.InitializeComponent();
        ///             SfMap syncMap = new SfMap();
        ///             ShapeFileLayer layer = new ShapeFileLayer();
        ///             layer.Uri = "MapApp.world1.shp";
        ///             syncMap.Layers.Add(layer);
        ///             syncMap.Panning += syncMap_Panning;
        /// 
        ///         }
        /// 
        ///         void syncMap_Panning(object sender, PanEventArgs args)
        ///         {
        ///             var latitude = args.Latitude;
        ///             var longitude = args.Longitude;
        ///             var direction = args.PanningDirection;
        ///         }       
        ///     }
        /// 
        /// }
        /// 
        /// 
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public event PanEventHandler Panning;

        internal void OnPanning(object sender, PanEventArgs args)
        {
            if (Panning != null)
            {
                Panning(sender, args);
            }
        }


        #endregion

        #endregion

        #region Properties

        #region EnableLayerChangeAnimation



        public bool EnableLayerChangeAnimation
        {
            get { return (bool)GetValue(EnableLayerChangeAnimationProperty); }
            set { SetValue(EnableLayerChangeAnimationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnableLayerChangeAnimation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnableLayerChangeAnimationProperty =
            DependencyProperty.Register("EnableLayerChangeAnimation", typeof(bool), typeof(SfMap), new PropertyMetadata(false));



        #endregion

        #region MapStyle


        /// <summary>
        /// Gets or sets the style for the map.
        /// </summary>
        /// <value>
        /// Type :<see cref="Style"/>
        /// </value>
        public Style MapStyle
        {
            get { return (Style)GetValue(MapStyleProperty); }
            set { SetValue(MapStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MapStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MapStyleProperty =
            DependencyProperty.Register("MapStyle", typeof(Style), typeof(SfMap), new PropertyMetadata(null));



        #endregion

        #region Header


        /// <summary>
        /// Gets or sets the Header of the SfMap. 
        /// </summary>
        /// <remarks>
        /// Use this property to set the header for the SfMap. Header is the title which shown on top of the SfMap.
        /// </remarks>
        /// <value>
        /// Type :<see cref="object"/>
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
        ///             syncMap.Header = new TextBlock { Text="World SfMap", HorizontalAlignment=HorizontalAlignment.Center,VerticalAlignment=VerticalAlignment.Center };
        ///             ShapeFileLayer layer = new ShapeFileLayer();          
        ///             layer.Uri = "MapApp.world1.shp";          
        ///             syncMap.Layers.Add(layer);            
        ///         }
        ///     }
        ///    
        /// }
        /// 
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public object Header
        {
            get { return (object)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(object), typeof(SfMap), new PropertyMetadata(null));



        #endregion

        #region EnableZoom


        /// <summary>
        /// Enables or disables the Zooming feature of the map.
        /// </summary>
        /// <remarks>
        /// Use this property to enable or disable the zooming feature of the map. "True" value will enable the zoom feature."False" value will disable the zooming feature.
        /// </remarks>
        /// <value>
        /// Type :<see cref="bool"/>
        /// "True" value enables the Zooming;And "False" will disable the Zooming.
        /// Default Value is "True".
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
        ///             syncMap.EnableZoom = false;
        ///             ShapeFileLayer layer = new ShapeFileLayer();          
        ///             layer.Uri = "MapApp.world1.shp";          
        ///             syncMap.Layers.Add(layer);            
        ///         }
        ///     }
        ///    
        /// }
        /// 
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public bool EnableZoom
        {
            get { return (bool)GetValue(EnableZoomProperty); }
            set { SetValue(EnableZoomProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnableZoom.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnableZoomProperty =
            DependencyProperty.Register("EnableZoom", typeof(bool), typeof(SfMap), new PropertyMetadata(true, new PropertyChangedCallback(OnEnableZoomChanged)));

        private static void OnEnableZoomChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as SfMap;
            if (instance != null)
                instance.Zoom(instance.ZoomLevel);
        }



        #endregion

        #region EnablePan


        /// <summary>
        /// Enables or disables the Panning feature of the map.
        /// </summary>
        /// <remarks>
        /// Use this property to enable or disable the panning feature of the map. "True" value will enable the panning feature."False" value will disable the panning feature.
        /// </remarks>
        /// <value>
        /// Type :<see cref="bool"/>
        /// "True" value enables the Panning;And "False" will disable the Panning.
        /// Default Value is "True".
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
        ///             syncMap.EnablePan = false;
        ///             ShapeFileLayer layer = new ShapeFileLayer();          
        ///             layer.Uri = "MapApp.world1.shp";          
        ///             syncMap.Layers.Add(layer);            
        ///         }
        ///     }
        ///    
        /// }
        /// 
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public bool EnablePan
        {
            get { return (bool)GetValue(EnablePanProperty); }
            set { SetValue(EnablePanProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnablePan.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnablePanProperty =
            DependencyProperty.Register("EnablePan", typeof(bool), typeof(SfMap), new PropertyMetadata(true));



        #endregion

        #region MinZoom


        /// <summary>
        /// Gets or sets the minimum zoom level of the map.
        /// </summary>
        /// <remarks>
        /// Use this property to set the minimum zoom level of the map. The map cannot be zoomed out below the MinZoom.
        /// </remarks>
        /// <value>
        /// Type :<see cref="Int32"/>
        /// <para>
        /// Default value will be 1.
        /// </para>
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
        ///             syncMap.MinZoom = 3;
        ///             ShapeFileLayer layer = new ShapeFileLayer();          
        ///             layer.Uri = "MapApp.world1.shp";          
        ///             syncMap.Layers.Add(layer);            
        ///         }
        ///     }
        ///    
        /// }
        /// 
        /// </code>        
        /// </example>
        [ClassReference(IsReviewed = false)]
        public Int32 MinZoom
        {
            get { return (Int32)GetValue(MinZoomProperty); }
            set
            {
                if (value > 0)
                {
                    SetValue(MinZoomProperty, value);
                }
            }
        }

        // Using a DependencyProperty as the backing store for MinZoom.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinZoomProperty =
            DependencyProperty.Register("MinZoom", typeof(Int32), typeof(SfMap), new PropertyMetadata(1, new PropertyChangedCallback(OnMinZoomChanged)));

        private static void OnMinZoomChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SfMap map = d as SfMap;
            if (d != null)
            {
                if (map.ZoomLevel < (int)e.NewValue)
                {
                    map.ZoomLevel = (int)e.NewValue;
                }
            }
        }




        #endregion

        #region MaxZoom

        /// <summary>
        /// Gets or sets the maximum zoom level of the map.
        /// </summary>
        /// <remarks>
        /// Use this property to set the maximum zoom level of the map. The map cannot be zoomed beyond the MaxZoom value.
        /// </remarks>
        /// <value>
        /// Type : <see cref="Int32"/>
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
        ///             syncMap.MaxZoom = 30;
        ///             ShapeFileLayer layer = new ShapeFileLayer();          
        ///             layer.Uri = "MapApp.world1.shp";          
        ///             syncMap.Layers.Add(layer);            
        ///         }
        ///     }
        ///    
        /// }
        /// 
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public Int32 MaxZoom
        {
            get { return (Int32)GetValue(MaxZoomProperty); }
            set
            {
                if (value > 0)
                {
                    SetValue(MaxZoomProperty, value);
                }
            }
        }

        // Using a DependencyProperty as the backing store for MaxZoom.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxZoomProperty =
            DependencyProperty.Register("MaxZoom", typeof(Int32), typeof(SfMap), new PropertyMetadata(100, new PropertyChangedCallback(OnMaxZoomChanged)));

        private static void OnMaxZoomChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SfMap map = d as SfMap;
            if (map.ZoomLevel > (int)e.NewValue)
            {
                map.ZoomLevel = (int)e.NewValue;
            }
        }


        #endregion

        #region ZoomLevel

        /// <summary>
        /// Gets or sets the ZoomLevel for the map.Based the ZoomLevel value the map will be zoomed.
        /// </summary>
        /// <remarks>Use this property to zoom in or zoom out the map. If the ZoomLavel is increased then map will be zoomed in.If value is decreases the map will be zoomed out.</remarks>
        /// <value>
        /// Type :<see cref="Int32"/>
        /// <para>The default value is 1</para>
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
        ///             syncMap.ZoomLevel = 3;
        ///             ShapeFileLayer layer = new ShapeFileLayer();          
        ///             layer.Uri = "MapApp.world1.shp";          
        ///             syncMap.Layers.Add(layer);            
        ///         }
        ///     }
        ///    
        /// }
        /// 
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public Int32 ZoomLevel
        {
            get { return (Int32)GetValue(ZoomLevelProperty); }
            set
            {
                if (value >= this.MinZoom && value <= this.MaxZoom)
                {
                    SetValue(ZoomLevelProperty, value);
                }
            }
        }

        // Using a DependencyProperty as the backing store for ZoomLevel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ZoomLevelProperty =
            DependencyProperty.Register("ZoomLevel", typeof(Int32), typeof(SfMap), new PropertyMetadata(1, new PropertyChangedCallback(OnZoomLevelChanged)));

        private static void OnZoomLevelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SfMap map = d as SfMap;
            if (map.currentLayer != null)
            {
#if WINRT
                if (map.MapView == MapViews.NormalView)
                {
#endif
                    var factor = (map.LayeredContent as ShapeFileLayer).ZoomFactor;
                    var layerZoom = (Int32)e.NewValue - (map.LayeredContent as ShapeFileLayer).TranslateZoomLevel;
                    if ((Int32)e.NewValue == 1)
                    {
                        map.Zoom(1);
                    }
                    else
                    {
                        map.Zoom(1 + ((Int32)e.NewValue * factor));
                    }

                    if (e.OldValue != null && e.NewValue != null)
                    {
                        if ((Int32)e.OldValue < (Int32)e.NewValue)
                        {
                            ZoomEventArgs args = new ZoomEventArgs(map.MapCoords.X, map.MapCoords.Y, (Int32)e.NewValue);
                            map.OnZoomedIn(map, args);
                        }
                        else
                        {
                            ZoomEventArgs args = new ZoomEventArgs(map.MapCoords.X, map.MapCoords.Y, (Int32)e.NewValue);
                            map.OnZoomedOut(map, args);
                        }
#if WINRT
                    }
#endif
                    if (map != null && map.LayeredContent != null && !map.LayeredContent.DesiredSize.IsEmpty && map.LayeredContent.DesiredSize != new Size(0, 0) && map.Padding != null)
                        map.RectClip = new Rect(0, 0, map.LayeredContent.DesiredSize.Width - (map.Padding.Left + map.Padding.Right), map.LayeredContent.DesiredSize.Height - (map.Padding.Bottom + map.Padding.Top));
                }

#if WINRT
                else
                {
                    var hor = map.scrollContent.HorizontalOffset;
                    var ver = map.scrollContent.VerticalOffset;
                    map.scrollContent.HorizontalScrollMode = ScrollMode.Enabled;
                    map.scrollContent.VerticalScrollMode = ScrollMode.Enabled;
                    if (e.OldValue != null && e.NewValue != null)
                    {
                        map.scrollFactor = map.ZoomLevel;
                        if ((Int32)e.OldValue < (Int32)e.NewValue)
                        {
                            if (map.scrollFactor <= map.MaxZoom)
                            {
#if SyncfusionFramework4_5_1
                                map.scrollContent.ChangeView(null, null, map.scrollFactor);
#else
                                map.scrollContent.ZoomToFactor(map.scrollFactor);
#endif
                                map.scrollContent.UpdateLayout();
                                var transferPoint = map.scrollContent.TransformToVisual(map.ScrollContent).Inverse.TransformPoint(map.zoomPoint);
#if SyncfusionFramework4_5_1
                                map.scrollContent.ChangeView(hor + transferPoint.X - map.zoomPoint.X - (transferPoint.X - map.zoomPoint.X * 2), ver + transferPoint.Y - map.zoomPoint.Y - (transferPoint.Y - map.zoomPoint.Y * 2), null);
#else
                                map.scrollContent.ScrollToHorizontalOffset(hor + transferPoint.X - map.zoomPoint.X - (transferPoint.X - map.zoomPoint.X * 2));
                                map.scrollContent.ScrollToVerticalOffset(ver + transferPoint.Y - map.zoomPoint.Y - (transferPoint.Y - map.zoomPoint.Y * 2));
#endif
                            }
                            else
                            {
                                map.scrollFactor = map.MaxZoom;
                            }
                            ZoomEventArgs args = new ZoomEventArgs(map.MapCoords.X, map.MapCoords.Y, (Int32)e.NewValue);
                            map.OnZoomedIn(map, args);
                        }
                        else
                        {
                            if (map.scrollFactor >= map.MinZoom)
                            {
#if SyncfusionFramework4_5_1
                                map.scrollContent.ChangeView(null, null, map.scrollFactor);
#else
                                map.scrollContent.ZoomToFactor(map.scrollFactor);
#endif
                                map.scrollContent.UpdateLayout();
                                var transferPoint = map.scrollContent.TransformToVisual(map.ScrollContent).Inverse.TransformPoint(map.zoomPoint);
#if SyncfusionFramework4_5_1
                                map.scrollContent.ChangeView(hor - transferPoint.X + map.zoomPoint.X + (transferPoint.X - map.zoomPoint.X * 2), ver - transferPoint.Y + map.zoomPoint.Y + (transferPoint.Y - map.zoomPoint.Y * 2), null);
#else
                                map.scrollContent.ScrollToHorizontalOffset(hor - transferPoint.X + map.zoomPoint.X + (transferPoint.X - map.zoomPoint.X * 2));
                                map.scrollContent.ScrollToVerticalOffset(ver - transferPoint.Y + map.zoomPoint.Y + (transferPoint.Y - map.zoomPoint.Y * 2));
#endif
                            }
                            else
                            {
                                map.scrollFactor = map.MinZoom;
                            }
                            ZoomEventArgs args = new ZoomEventArgs(map.MapCoords.X, map.MapCoords.Y, (Int32)e.NewValue);
                            map.OnZoomedOut(map, args);
                        }
                    }

                }
#endif
                if (map.Layers.Count > 1)
                {
                    if (map.currentLayer is ShapeFileLayer)
                    {
                        var desireLayer = map.GetLayeredContent() as List<ShapeFileLayer>;
                        if (desireLayer != null)
                        {
                            if (map.LayeredContent != (desireLayer.ElementAt(0) as UIElement))
                            {
#if WINRT
                                if (map.MapView == MapViews.NormalView)
                                {
                                    map.LayeredContent = (desireLayer.ElementAt(0) as UIElement);
                                }
                                else
                                {
                                    map.ScrollContent = (desireLayer.ElementAt(0) as UIElement);
                                }
#else
                                map.LayeredContent = (desireLayer.ElementAt(0) as UIElement);
#endif

                            }

                        }
                    }
                }
            }

        }


        #endregion

        #region LayerChangeMode




        internal LayerChangeMode LayerChangeMode
        {
            get { return (LayerChangeMode)GetValue(LayerChangeModeProperty); }
            set { SetValue(LayerChangeModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LayerChangeMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LayerChangeModeProperty =
            DependencyProperty.Register("LayerChangeMode", typeof(LayerChangeMode), typeof(SfMap), new PropertyMetadata(LayerChangeMode.Automatic));



        #endregion

        #region LayeredContent


#if WINRT

        public MapViews MapView
        {
            get { return (MapViews)GetValue(MapViewProperty); }
            set { SetValue(MapViewProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MapView.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MapViewProperty =
            DependencyProperty.Register("MapView", typeof(MapViews), typeof(SfMap), new PropertyMetadata(MapViews.SmartView, new PropertyChangedCallback(OnMapViewChanged)));

        private static void OnMapViewChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SfMap map = d as SfMap;
            if (e.NewValue.Equals(MapViews.SmartView))
            {
                if (map.LayeredContent != null)
                {
                    var layer = map.LayeredContent as ShapeFileLayer;
                    var scaleX = layer.PanTransform.X;
                    var scaleY = layer.PanTransform.Y;
                    layer.PanTransform.X = 0;
                    layer.ZoomTransform.ScaleX = 1;
                    layer.PanTransform.Y = 0;
                    layer.ZoomTransform.ScaleY = 1;
                    map.SetBinding(SfMap.ZoomLevelProperty, new Binding { Source = map.scrollContent, Path = new PropertyPath("ZoomFactor") });
                    map.scrollContent.SetBinding(ScrollViewer.HorizontalScrollModeProperty, new Binding { Source = map, Path = new PropertyPath("EnablePan"), Converter = new ScrollModeConverter() });
                    map.scrollContent.SetBinding(ScrollViewer.VerticalScrollModeProperty, new Binding { Source = map, Path = new PropertyPath("EnablePan"), Converter = new ScrollModeConverter() });
                    map.scrollContent.SetBinding(ScrollViewer.ZoomModeProperty, new Binding { Source = map, Path = new PropertyPath("EnableZoom") });
                    map.SetBinding(SfMap.ScrollContentProperty, new Binding { Source = map, Path = new PropertyPath("BaseMapIndex"), Converter = new LayerContentConverter(), ConverterParameter = map.Layers });
#if SyncfusionFramework4_5_1
                    map.scrollContent.ChangeView(null, null, (float)map.zoomval);
#else
                    map.scrollContent.ZoomToFactor((float)map.zoomval);
#endif
                    map.scrollContent.UpdateLayout();
#if SyncfusionFramework4_5_1
                    map.scrollContent.ChangeView(scaleX, scaleX, null);
#else
                    map.scrollContent.ScrollToHorizontalOffset(scaleX);
                    map.scrollContent.ScrollToVerticalOffset(scaleX);
#endif
                    map.LayeredContent = null;
                    map.ScrollContent = layer;
                    layer.Refresh();
                    map.currentLayer = layer;

                }
            }
            else
            {
                if (map.ScrollContent != null)
                {
                    var layer = map.ScrollContent as ShapeFileLayer;
                    map.ScrollContent = null;
                    map.LayeredContent = layer;
                    map.currentLayer = layer;
                    map.SetBinding(SfMap.LayeredContentProperty, new Binding { Source = map, Path = new PropertyPath("BaseMapIndex"), Converter = new LayerContentConverter(), ConverterParameter = map.Layers });
                    layer.Refresh();
                    map.ZoomLevel = 1;
                }
            }

        }

        internal UIElement ScrollContent
        {
            get { return (UIElement)GetValue(ScrollContentProperty); }
            set { SetValue(ScrollContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ScrollContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ScrollContentProperty =
            DependencyProperty.Register("ScrollContent", typeof(UIElement), typeof(SfMap), new PropertyMetadata(null, OnScrollContentChanged));

        private static void OnScrollContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                (d as SfMap).currentLayer = e.NewValue as ShapeFileLayer;
            }
        }

#endif

        internal UIElement LayeredContent
        {
            get { return (UIElement)GetValue(LayeredContentProperty); }
            set { SetValue(LayeredContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LayeredContent.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty LayeredContentProperty =
            DependencyProperty.Register("LayeredContent", typeof(UIElement), typeof(SfMap), new PropertyMetadata(null, new PropertyChangedCallback(OnLayeredContentChanged)));

        private static void OnLayeredContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SfMap map = (d as SfMap);

            if (e.NewValue != null)
            {
#if WINRT
                if (map.MapView == MapViews.SmartView)
                {
                    map.ScrollContent = e.NewValue as ShapeFileLayer;
                    if (map.scrollContent != null)
                    {
                        map.SetBinding(SfMap.ZoomLevelProperty, new Binding { Source = map.scrollContent, Path = new PropertyPath("ZoomFactor") });
                        map.scrollContent.SetBinding(ScrollViewer.HorizontalScrollModeProperty, new Binding { Source = map, Path = new PropertyPath("EnablePan"), Converter = new ScrollModeConverter() });
                        map.scrollContent.SetBinding(ScrollViewer.VerticalScrollModeProperty, new Binding { Source = map, Path = new PropertyPath("EnablePan"), Converter = new ScrollModeConverter() });
                        map.scrollContent.SetBinding(ScrollViewer.ZoomModeProperty, new Binding { Source = map, Path = new PropertyPath("EnableZoom") });

                    }
                    map.LayeredContent = null;
                }
                else
                {
                    map.currentLayer = e.NewValue as ShapeFileLayer;
                }
#else
                map.currentLayer = e.NewValue as ShapeFileLayer;
#endif
            }


            if ((d as SfMap).EnableLayerChangeAnimation && e.NewValue != null)
            {
                Storyboard sb = (d as SfMap).zoominanimation;
                Storyboard sb1 = (d as SfMap).zoominanimation;

                if (e.OldValue != null)
                {
                    sb1 = ShapeFileLayer.resources["Storyboard2"] as Storyboard;
                    sb1.Stop();
                    foreach (Timeline anis in sb1.Children)
                    {
                        Storyboard.SetTarget(anis, (e.NewValue as ShapeFileLayer));
                    }
                    sb1.Begin();
                }
                if (e.NewValue != null)
                {

                    if (ShapeFileLayer.resources != null)
                    {
                        sb = ShapeFileLayer.resources["Storyboard1"] as Storyboard;
                        sb.Stop();
                        foreach (Timeline anis in sb.Children)
                        {
                            if (sb.Children.IndexOf(anis) == 4)
                            {
                                Storyboard.SetTarget(anis, (e.NewValue as ShapeFileLayer));
                            }
                            Storyboard.SetTarget(anis, (e.NewValue as ShapeFileLayer).AnimationTransfrom);
                        }

                        sb.Begin();
                    }
                }
            }
        }


        #endregion

        #region Layers

        /// <summary>
        /// Gets or sets the Layers for the map. Layers are the content for the map.
        /// </summary>
        /// <remarks>
        /// Use this property to add  or remove the layers of the map. Each layer in Layers are the container of the map elements.
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
        ///     public sealed partial class MainPage : Page
        ///     {
        ///         public MainPage()
        ///         {
        ///             this.InitializeComponent();           
        ///             SfMap syncMap = new SfMap();           
        ///             ShapeFileLayer layer = new ShapeFileLayer();          
        ///             layer.Uri = "MapApp.world1.shp";          
        ///             syncMap.Layers.Add(layer);            
        ///         }
        ///     }
        ///    
        /// }
        /// 
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public ObservableCollection<MapLayer> Layers
        {
            get { return (ObservableCollection<MapLayer>)GetValue(LayersProperty); }
            set { SetValue(LayersProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Layers.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LayersProperty =
            DependencyProperty.Register("Layers", typeof(ObservableCollection<MapLayer>), typeof(SfMap), new PropertyMetadata(null));


        #endregion

        #region BaseMapIndex


        /// <summary>
        /// Gets or sets the index of the layer which is loaded initially.
        /// </summary>
        /// <remarks>
        /// Use this property to set the initial layer to be loaded in the map.
        /// </remarks>
        /// <value>
        /// Type :<see cref="int"/>
        /// </value>
        /// <example>
        /// <code langeage="C#">
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
        ///     public sealed partial class MainPage : Page
        ///     {
        ///         public MainPage()
        ///         {
        ///             this.InitializeComponent();           
        ///             SfMap syncMap = new SfMap();           
        ///             ShapeFileLayer layer = new ShapeFileLayer();          
        ///             layer.Uri = "MapApp.world1.shp";
        ///             ShapeFileLayer layer1 = new ShapeFileLayer();
        ///             layer.Uri = "MapApp.US.shp";
        ///             syncMap.Layers.Add(layer);   
        ///             syncMap.Layers.Add(layer1);
        ///             syncMap.BaseMapIndex = 1;
        ///         }
        ///     }
        ///    
        /// }
        /// 
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public int BaseMapIndex
        {
            get { return (int)GetValue(BaseMapIndexProperty); }
            set { SetValue(BaseMapIndexProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BaseMapIndex.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BaseMapIndexProperty =
            DependencyProperty.Register("BaseMapIndex", typeof(int), typeof(SfMap), new PropertyMetadata(0));




        #endregion

        #region MapCoords

        /// <summary>
        /// Gets the co ordinates of the map in terms of latitude and longitude values. 
        /// </summary>
        /// <remarks>
        /// This is read only property to get the current latitude and longitude values when hovering on the map.
        /// </remarks>
        /// <value>
        /// Type :<see cref="Point"/>
        /// </value>
        [ClassReference(IsReviewed = false)]
        public Point MapCoords
        {
            get { return (Point)GetValue(MapCoordsProperty); }
            internal set { SetValue(MapCoordsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MapCoords.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MapCoordsProperty =
            DependencyProperty.Register("MapCoords", typeof(Point), typeof(SfMap), new PropertyMetadata(new Point(0, 0)));

        #endregion

        #region ShowCoords

        /// <summary>
        /// Gets or sets a value indicating whether the latitude and longitude value can be viewed or not.
        /// </summary>
        /// <value><see langword="true"/> if ; otherwise, <see langword="false"/>.</value>
        /// <remarks>Use this property to enables or disables the visibility of the Latitude and Longitude values on the top right of the SfMap</remarks>
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
        ///     public sealed partial class MainPage : Page
        ///     {
        ///         public MainPage()
        ///         {
        ///             this.InitializeComponent();
        ///             SfMap syncMap = new SfMap();
        ///             ShapeFileLayer layer = new ShapeFileLayer();
        ///             layer.Uri = "MapApp.world1.shp";
        ///             syncMap.Layers.Add(layer);
        ///             syncMap.ShowCoords = false;
        /// 
        ///         }
        ///     }
        /// 
        /// }
        /// 
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public bool ShowCoords
        {
            get { return (bool)GetValue(ShowCoordsProperty); }
            set { SetValue(ShowCoordsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowCoords.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowCoordsProperty =
             DependencyProperty.Register("ShowCoords", typeof(bool), typeof(SfMap), new PropertyMetadata(false));

        #endregion

        #region LatitudeLongitudeType

        /// <summary>
        /// Gets or sets display type of LatitudeLangitude values on the top right of the map.
        /// </summary>
        /// <value>Type :<see cref="LatLonType"/></value>
        /// <remarks>
        /// Use this property to determine how to display the latitude values. There are two types,
        /// <para>DMS and Decimal</para>
        /// <para>Decimal will be shown as normal numbers.</para>
        /// <para>DMS will be shown as Degrees,Minutes and Seconds.</para>
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
        ///     public sealed partial class MainPage : Page
        ///     {
        ///         public MainPage()
        ///         {
        ///             this.InitializeComponent();
        ///             SfMap syncMap = new SfMap();
        ///             ShapeFileLayer layer = new ShapeFileLayer();
        ///             layer.Uri = "MapApp.world1.shp";
        ///             syncMap.LatitudeLongitudeType = LatLonType.Decimal;
        ///             syncMap.LatitudeLongitudeType = LatLonType.DMS;
        ///             syncMap.Layers.Add(layer);
        ///             
        ///         }
        ///     }
        /// 
        /// }
        /// 
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public LatLonType LatitudeLongitudeType
        {
            get { return (LatLonType)GetValue(LatitudeLongitudeTypeProperty); }
            set { SetValue(LatitudeLongitudeTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LatitudeLongitudeType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LatitudeLongitudeTypeProperty =
            DependencyProperty.Register("LatitudeLongitudeType", typeof(LatLonType), typeof(SfMap), new PropertyMetadata(LatLonType.DMS));
        private static void OnLatitudeLongitudeTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as SfMap;
            SetLatLonTypeBinding(instance);
        }

        private static void SetLatLonTypeBinding(SfMap instance)
        {
            if (instance != null)
            {
                if (instance.LatitudeLongitudeType == LatLonType.DMS)
                    instance.coordLabel.SetBinding(TextBlock.TextProperty, new Binding { Source = instance, Path = new PropertyPath("MapCoords"), Converter = new LatitudeLongitudeDegreeToTextConverter() });
                else
                    instance.coordLabel.SetBinding(TextBlock.TextProperty, new Binding { Source = instance, Path = new PropertyPath("MapCoords"), Converter = new LatitudeLongitudeToTextConverter() });
            }
        }

        #endregion

        #endregion

        #region Internal Properties

        #region RectClip



        internal Rect RectClip
        {
            get { return (Rect)GetValue(RectClipProperty); }
            set { SetValue(RectClipProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RectClip.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RectClipProperty =
            DependencyProperty.Register("RectClip", typeof(Rect), typeof(SfMap), new PropertyMetadata(null));



        #endregion


        internal Grid MapGrid
        {
            get;
            set;
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new Instance for the SfMap.And initialize its values.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public SfMap()
        {
            this.DefaultStyleKey = typeof(SfMap);
            this.Layers = new ObservableCollection<MapLayer>();
            this.panResetCommand = new PanResetCommand(this);
            this.zoomResetCommand = new ZoomResetCommand(this);
            this.panCommand = new PanCommand(this);
            this.zoomInCommand = new ZoomInCommand(this);
            this.zoomOutCommand = new ZoomOutCommand(this);
            this.resetCommand = new ShapeFileResetCommand(this);
            this.refreshCommand = new ShapeFileRefreshCommand(this);
#if WINRT
            this.ManipulationMode = ManipulationModes.Scale
                | ManipulationModes.TranslateX
                | ManipulationModes.TranslateY
                | ManipulationModes.TranslateInertia
                | ManipulationModes.ScaleInertia;
#endif
            this.Layers.CollectionChanged += Layers_CollectionChanged;


        }

        void Layers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                var layer = (e.NewItems[0] as ShapeFileLayer);
                if (layer != null)
                    this.LayeredContent = layer;
            }
        }

#if WINRT

        protected override void OnManipulationCompleted(ManipulationCompletedRoutedEventArgs e)
        {
            if (this.MapView == MapViews.NormalView)
            {
                if (this.isZoomed)
                {
                    this.isZoomed = false;
                }
                (this.LayeredContent as ShapeFileLayer).canSelect = true;
                this.isPinching = false;
                this.updateAreaAction = null;
            }
            base.OnManipulationCompleted(e);
        }

        private int zoomLevel = 0;
        protected override void OnManipulationStarted(ManipulationStartedRoutedEventArgs e)
        {
            if (this.MapView == MapViews.NormalView)
            {
                this.zoomLevel = this.ZoomLevel;
                zoomLevel = ZoomLevel;
                isInertiaStarted = false;
                this.zoomPoint = e.Position;
                this.updateAreaAction = null;
                zoompt = (this.LayeredContent as ShapeFileLayer).LatitudeLongitudeToPoint((this.LayeredContent as ShapeFileLayer).PointToLatitudeLongitude(this.zoomPoint));
                isZoomed = true;
            }
            base.OnManipulationStarted(e);
        }

        IAsyncAction updateAreaAction;

        internal void ScheduleUpdate()
        {
            if (updateAreaAction == null)
            {
                updateAreaAction = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, UpdateZoom);
            }
        }


        protected override void OnManipulationDelta(ManipulationDeltaRoutedEventArgs e)
        {
            if (this.MapView == MapViews.NormalView)
            {
                if (e.Cumulative.Expansion == 0)
                {
                    if ((this.LayeredContent as ShapeFileLayer).EnableMultiSelection)
                    {
                        if (this.isPointerPressed)
                        {
                            foreach (MapShape shp in (this.LayeredContent as ShapeFileLayer).MapShapes)
                            {
                                Rect rect = new Rect((this.LayeredContent as ShapeFileLayer).dragStartPoint, (this.LayeredContent as ShapeFileLayer).draggingPoint);
                                (this.LayeredContent as ShapeFileLayer).SelectionRect = rect;
                                rect.Intersect((this.LayeredContent as ShapeFileLayer).ViewTransform.TransformBounds(shp.Shape.Data.Bounds));
                                if (!rect.IsEmpty)
                                {
                                    if (!(this.LayeredContent as ShapeFileLayer).tempSelectedShapes.Contains(shp))
                                    {
                                        (this.LayeredContent as ShapeFileLayer).tempSelectedShapes.Add(shp);
                                    }
                                }
                                else
                                {
                                    if ((this.LayeredContent as ShapeFileLayer).tempSelectedShapes.Contains(shp))
                                    {
                                        (this.LayeredContent as ShapeFileLayer).tempSelectedShapes.Remove(shp);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (this.EnablePan)
                        {
                            if (!(this.LayeredContent as ShapeFileLayer).EnableMultiSelection)
                            {
                                this.CurrentPoint = e.Position;
                                if (this.isPointerPressed)
                                {
                                    this.Pan(this.CurrentPoint.X - this.startPoint.X + this.panTranslatePoint.X, this.CurrentPoint.Y - this.startPoint.Y + this.panTranslatePoint.Y);
                                    PanEventArgs args = GetPanningEvent();
                                    this.OnPanning(this, args);
                                }
                            }
                        }
                    }
                }
                else if (!isInertiaStarted)
                {
                    if (this.updateAreaAction == null)
                    {
                        if (this.EnableZoom)
                        {
                            (this.LayeredContent as ShapeFileLayer).canSelect = false;
                            if (e.Cumulative.Scale >= 1 && prevDelta < e.Cumulative.Scale)
                            {
                                zoomLevel++;
                                ScheduleUpdate();
                                prevDelta = e.Cumulative.Scale;
                            }
                            else
                            {
                                if (this.zoomLevel >= 1)
                                {
                                    zoomLevel--;
                                    ScheduleUpdate();
                                    prevDelta = e.Cumulative.Scale;
                                }
                            }
                        }
                        this.isPinching = true;
                    }
                }
            }
            base.OnManipulationDelta(e);
        }

        private void UpdateZoom()
        {
            if (!cancelZoom)
            {
                ZoomLevel = zoomLevel;
                updateAreaAction = null;
                cancelZoom = false;
            }
        }

        bool isInertiaStarted = false;
        protected override void OnManipulationInertiaStarting(ManipulationInertiaStartingRoutedEventArgs e)
        {
            base.OnManipulationInertiaStarting(e);
            isInertiaStarted = true;
            e.TranslationBehavior.DesiredDeceleration = 0.001;
            e.ExpansionBehavior.DesiredDeceleration = 0.001;

        }
#else
        private int zoomLevel = 0;
        protected override void OnManipulationStarted(ManipulationStartedEventArgs e)
        {
            zoomLevel = ZoomLevel;
            this.zoomPoint = e.ManipulationOrigin;
            zoompt = (this.LayeredContent as ShapeFileLayer).LatitudeLongitudeToPoint((this.LayeredContent as ShapeFileLayer).PointToLatitudeLongitude(this.zoomPoint));
            base.OnManipulationStarted(e);
        }
#if !WPF
        
        protected override void OnDoubleTap(GestureEventArgs e)
        {
            if (this.EnableZoom)
            {
                this.zoomPoint = e.GetPosition(this.LayeredContent);
                zoompt = (this.LayeredContent as ShapeFileLayer).LatitudeLongitudeToPoint((this.LayeredContent as ShapeFileLayer).PointToLatitudeLongitude(this.zoomPoint));
                this.ZoomLevel++;
            }
            base.OnDoubleTap(e);
        }
        

#endif
        protected override void OnManipulationCompleted(ManipulationCompletedEventArgs e)
        {
            base.OnManipulationCompleted(e);
            this.isManipulteDelta = false;
            this.panTranslatePoint = new Point((this.LayeredContent as ShapeFileLayer).PanTransform.X, (this.LayeredContent as ShapeFileLayer).PanTransform.Y);

        }
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {

            this.zoomPoint.X = 0;
            this.zoomPoint.Y = 0;
            this.startPoint = new Point(0, 0);
            this.isPointerPressed = false;
            this.panTranslatePoint = new Point((this.LayeredContent as ShapeFileLayer).PanTransform.X, (this.LayeredContent as ShapeFileLayer).PanTransform.Y);
            if (this.isPan)
            {
                PanEventArgs args = new PanEventArgs(0d, 0d, PanMode.Bottom);
                this.OnPanned(this, args);
                this.isPan = false;
            }
            this.ReleaseMouseCapture();
            base.OnMouseLeftButtonUp(e);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {   
            this.isPointerPressed = true;
            this.startPoint = e.GetPosition(this.LayeredContent);
            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (this.LayeredContent is ShapeFileLayer)
            {
                this.MapCoords = (this.LayeredContent as ShapeFileLayer).PointToLatitudeLongitude(e.GetPosition(this.LayeredContent));
            }
            if (!this.isManipulteDelta)
            {
                if ((this.LayeredContent as ShapeFileLayer).EnableMultiSelection)
                {
                    if (this.isPointerPressed)
                    {
                        foreach (MapShape shp in (this.LayeredContent as ShapeFileLayer).MapShapes)
                        {
                            Rect rect = new Rect((this.LayeredContent as ShapeFileLayer).dragStartPoint, (this.LayeredContent as ShapeFileLayer).draggingPoint);
                            (this.LayeredContent as ShapeFileLayer).SelectionRect = rect;
                            rect.Intersect((this.LayeredContent as ShapeFileLayer).ViewTransform.TransformBounds(shp.Shape.Data.Bounds));
                            if (!rect.IsEmpty)
                            {
                                if (!(this.LayeredContent as ShapeFileLayer).tempSelectedShapes.Contains(shp))
                                {
                                    (this.LayeredContent as ShapeFileLayer).tempSelectedShapes.Add(shp);
                                }
                            }
                            else
                            {
                                if ((this.LayeredContent as ShapeFileLayer).tempSelectedShapes.Contains(shp))
                                {
                                    (this.LayeredContent as ShapeFileLayer).tempSelectedShapes.Remove(shp);
                                    Debug.WriteLine("Removed");
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (this.EnablePan)
                    {
                        if (!(this.LayeredContent as ShapeFileLayer).EnableMultiSelection)
                        {
                            this.CurrentPoint = e.GetPosition(this.LayeredContent);
                            if (this.isPointerPressed)
                            {
                                this.Pan(this.CurrentPoint.X - this.startPoint.X + this.panTranslatePoint.X, this.CurrentPoint.Y - this.startPoint.Y + this.panTranslatePoint.Y);
                                this.CaptureMouse();
                            }                           
                        }
                        
                    }
                }
            }
            base.OnMouseMove(e);
        }

       

        protected override void OnManipulationDelta(ManipulationDeltaEventArgs e)
        {

            if (e.DeltaManipulation.Translation.X == 0 && e.DeltaManipulation.Translation.Y == 0)
            {
                if (this.EnableZoom)
                {
#if WINDOWSPHONE_8
                    if (e.PinchManipulation != null)
                    {
#endif
                        this.isManipulteDelta = true;
                        (this.LayeredContent as ShapeFileLayer).canSelect = false;
#if WINDOWSPHONE_8
                        if (e.PinchManipulation.DeltaScale > 1)
                        {
                            ZoomLevel++;
                        }

#else
                          ZoomLevel++;
#endif
#if WINDOWSPHONE_8
                        else
                        {
                            if (this.ZoomLevel >= 1)
                            {
                                ZoomLevel--;
                            }
                        }
                    }
#endif
                }
            }

            base.OnManipulationDelta(e);
        }

#endif
        #endregion

        #region Override Functions

#if WINRT
        protected override void OnApplyTemplate()
        {
#else
        public override void OnApplyTemplate()
        {
#endif
            base.OnApplyTemplate();
            popup = GetTemplateChild("Part_BalloonPopup") as Popup;
            popupGrid = GetTemplateChild("Part_BalloonPopupGrid") as Grid;
            var closeButton = GetTemplateChild("Part_ClosePopupButton") as Button;
            if (closeButton != null)
                closeButton.Click += ClosePopup_Click;
            this.coordLabel = this.GetTemplateChild("PART_MapCoords") as TextBlock;
            SetLatLonTypeBinding(this);
            this.SizeChanged += MapControl_SizeChanged;
#if WINRT
            this.scrollContent = this.GetTemplateChild("PART_ScrollContent") as ScrollViewer;
            this.scrollContent.PointerWheelChanged += scrollContent_PointerWheelChanged;
#endif

        }

        void ClosePopup_Click(object sender, RoutedEventArgs e)
        {
            popup.IsOpen = false;
        }

#if WINRT
        void scrollContent_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            if (this.EnableZoom)
            {
                this.zoomPoint = e.GetCurrentPoint(this.scrollContent).Position;
                if (e.GetIntermediatePoints(this.ScrollContent)[0].Properties.MouseWheelDelta >= 120)
                {
                    ZoomLevel++;
                }
                else
                {
                    ZoomLevel--;
                }
            }
            e.Handled = true;
        }
#endif

        void MapControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var grid = this.GetTemplateChild("PART_MapGrid");
            this.RectClip = new Rect(0, 0, e.NewSize.Width - (Padding.Left + Padding.Right), e.NewSize.Height - (Padding.Bottom + Padding.Top));
#if WINRT
            if (this.MapView.Equals(MapViews.NormalView))
            {
                this.SetBinding(SfMap.LayeredContentProperty, new Binding { Source = this, Path = new PropertyPath("BaseMapIndex"), Converter = new LayerContentConverter(), ConverterParameter = this.Layers });
            }
            else
            {
                this.SetBinding(SfMap.ScrollContentProperty, new Binding { Source = this, Path = new PropertyPath("BaseMapIndex"), Converter = new LayerContentConverter(), ConverterParameter = this.Layers });

            }
            this.scrollContent.Height = e.NewSize.Height;
            this.scrollContent.Width = e.NewSize.Width;
            if (this.MapView.Equals(MapViews.NormalView))
            {
                this.currentLayer = this.LayeredContent as ShapeFileLayer;
            }
#else           
            this.SetBinding(SfMap.LayeredContentProperty, new Binding { Source = this, Path = new PropertyPath("BaseMapIndex"), Converter = new LayerContentConverter(), ConverterParameter = this.Layers });
#endif
#if !WINRT
            this.currentLayer = this.LayeredContent as ShapeFileLayer;
            
#else
            if (this.MapView == MapViews.NormalView)
            {
                this.currentLayer = this.LayeredContent as ShapeFileLayer;
            }
            else
            {
                this.currentLayer = this.ScrollContent as ShapeFileLayer;
            }
#endif
            if (this.currentLayer != null && currentLayer.isLoaded)
            {
                this.currentLayer.grid.Height = e.NewSize.Height;
                this.currentLayer.grid.Width = e.NewSize.Width;
                (currentLayer as ShapeFileLayer).PanTransform.X = currentLayer.Margin.Left;
                (currentLayer as ShapeFileLayer).PanTransform.Y = currentLayer.Margin.Top;
                (currentLayer as ShapeFileLayer).SetElementsMargin();
                foreach (var sublayer in this.currentLayer.SubShapeFileLayers)
                {
                    if (sublayer.grid != null)
                    {
                        sublayer.grid.Height = e.NewSize.Height;
                        sublayer.grid.Width = e.NewSize.Width;
                        sublayer.SetElementsMargin();
                    }
                }
            }

        }

#if WINRT
        protected override void OnPointerWheelChanged(PointerRoutedEventArgs e)
        {
            this.zoomPoint = e.GetCurrentPoint(this.LayeredContent).Position;
            zoompt = (this.LayeredContent as ShapeFileLayer).LatitudeLongitudeToPoint((this.LayeredContent as ShapeFileLayer).PointToLatitudeLongitude(this.zoomPoint));
            if (this.EnableZoom)
            {
                if (e.GetIntermediatePoints(this.LayeredContent)[0].Properties.MouseWheelDelta >= 120)
                {
                    this.ZoomLevel++;
                }
                else
                {
                    this.ZoomLevel--;
                }
                this.panTranslatePoint = new Point((this.LayeredContent as ShapeFileLayer).PanTransform.X, (this.LayeredContent as ShapeFileLayer).PanTransform.Y);
            }
            base.OnPointerWheelChanged(e);
        }

        protected override void OnTapped(Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            base.OnTapped(e);
            // var pt = (this.LayeredContent as ShapeFileLayer).PointToLatitudeLongitude(e.GetPosition(this.LayeredContent));
        }

        protected override void OnDoubleTapped(Windows.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
        {
            base.OnDoubleTapped(e);
            if (this.EnableZoom)
            {
                this.zoomPoint = e.GetPosition(this.LayeredContent);
                zoompt = (this.currentLayer as ShapeFileLayer).LatitudeLongitudeToPoint((this.currentLayer as ShapeFileLayer).PointToLatitudeLongitude(this.zoomPoint));
                this.ZoomLevel++;
            }
        }

        protected override void OnPointerPressed(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            this.isPointerPressed = true;
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse)
            {
                this.isMousePointerPressed = true;
                offsetPoint = new Point(this.scrollContent.HorizontalOffset, this.scrollContent.VerticalOffset);

            }
            if (this.EnablePan)
            {
                this.scrollContent.HorizontalScrollMode = ScrollMode.Enabled;
                this.scrollContent.VerticalScrollMode = ScrollMode.Enabled;
            }
            else
            {
                this.scrollContent.HorizontalScrollMode = ScrollMode.Disabled;
                this.scrollContent.VerticalScrollMode = ScrollMode.Disabled;
            }
            if (this.EnableZoom)
            {
                this.scrollContent.ZoomMode = ZoomMode.Enabled;
            }
            else
            {
                this.scrollContent.ZoomMode = ZoomMode.Disabled;

            }
            this.startPoint = e.GetCurrentPoint(this).Position;
            base.OnPointerPressed(e);

        }

        protected override void OnPointerReleased(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            this.zoomPoint.X = 0;
            this.zoomPoint.Y = 0;
            this.prevDelta = 0.0;
            this.prevPoint = new Point();
            this.isPointerPressed = false;
            offsetPoint = new Point(this.scrollContent.HorizontalOffset, this.scrollContent.VerticalOffset);
            this.panTranslatePoint = new Point((this.currentLayer as ShapeFileLayer).PanTransform.X, (this.currentLayer as ShapeFileLayer).PanTransform.Y);
            if (this.isPan)
            {

                PanEventArgs args = this.GetPanningEvent();
                this.OnPanned(this, args);
                this.isPan = false;
            }
            this.startPoint = new Point(0, 0);
            base.OnPointerReleased(e);
        }

        PanEventArgs GetPanningEvent()
        {
            PanMode mode = PanMode.Top;
            double xdiff = this.CurrentPoint.X - this.startPoint.X;
            double ydiff = this.CurrentPoint.Y - this.startPoint.Y;
            if (xdiff > 0 && xdiff > Math.Abs(ydiff))
            {
                mode = PanMode.Right;
            }
            else if (xdiff < 0 && Math.Abs(xdiff) > Math.Abs(ydiff))
            {
                mode = PanMode.Left;
            }
            else if (ydiff > 0)
            {
                mode = PanMode.Bottom;
            }

            PanEventArgs args = new PanEventArgs(this.MapCoords.X, this.MapCoords.Y, mode);
            return args;

        }

        protected override void OnPointerMoved(PointerRoutedEventArgs e)
        {
            if (this.LayeredContent is ShapeFileLayer)
            {
                this.MapCoords = (this.LayeredContent as ShapeFileLayer).PointToLatitudeLongitude(e.GetCurrentPoint(this.LayeredContent).Position);
            }
            else if (this.ScrollContent != null && this.MapView == MapViews.SmartView)
            {
                this.MapCoords = (this.ScrollContent as ShapeFileLayer).PointToLatitudeLongitude(e.GetCurrentPoint(this.ScrollContent).Position);
            }
            if (isPointerPressed && isMousePointerPressed)
            {
                if ((this.currentLayer as ShapeFileLayer).EnableMultiSelection != true)
                {
                    if (this.EnablePan)
                    {
                        this.zoomPoint = e.GetCurrentPoint(this).Position;
                        var Xoff = ((-zoomPoint.X + this.startPoint.X));
                        var Yoff = ((-zoomPoint.Y + startPoint.Y));
                        Xoff += offsetPoint.X;
                        Yoff += offsetPoint.Y;
#if SyncfusionFramework4_5_1
                        this.scrollContent.ChangeView(Xoff, Yoff, null);
#else
                        this.scrollContent.ScrollToHorizontalOffset(Xoff);
                        this.scrollContent.ScrollToVerticalOffset(Yoff);
#endif
                    }
                }
                else
                {
                    foreach (MapShape shp in (this.currentLayer as ShapeFileLayer).MapShapes)
                    {
                        Rect rect = new Rect((this.currentLayer as ShapeFileLayer).dragStartPoint, (this.currentLayer as ShapeFileLayer).draggingPoint);
                        (this.currentLayer as ShapeFileLayer).SelectionRect = rect;
                        rect.Intersect((this.currentLayer as ShapeFileLayer).ViewTransform.TransformBounds(shp.Shape.Data.Bounds));
                        if (!rect.IsEmpty)
                        {
                            if (!(this.currentLayer as ShapeFileLayer).tempSelectedShapes.Contains(shp))
                            {
                                (this.currentLayer as ShapeFileLayer).tempSelectedShapes.Add(shp);
                            }
                        }
                        else
                        {
                            if ((this.currentLayer as ShapeFileLayer).tempSelectedShapes.Contains(shp))
                            {
                                (this.currentLayer as ShapeFileLayer).tempSelectedShapes.Remove(shp);
                            }
                        }
                    }
                }


            }
            base.OnPointerMoved(e);
        }
#endif

#if !WINRT
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            if (this.EnableZoom)
            {
                this.zoomPoint = e.GetPosition(this.LayeredContent);
                zoompt = (this.LayeredContent as ShapeFileLayer).LatitudeLongitudeToPoint((this.LayeredContent as ShapeFileLayer).PointToLatitudeLongitude(this.zoomPoint));
                if (e.Delta >= 120)
                {
                    this.ZoomLevel++;
                }
                else
                {
                    this.ZoomLevel--;
                }
                this.panTranslatePoint = new Point((this.LayeredContent as ShapeFileLayer).PanTransform.X, (this.LayeredContent as ShapeFileLayer).PanTransform.Y);

                base.OnMouseWheel(e);
            }
        }
#endif

        #endregion

        #region Helpers

        internal static T FindChild<T>(DependencyObject parent, string childName)
        where T : DependencyObject
        {
            // Confirm parent and childName are valid. 
            if (parent == null)
            {
                return null;
            }

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                // If the child is not of the request child type child
                var childType = child as T;
                if (childType == null)
                {
                    // recursively drill down the tree
                    foundChild = FindChild<T>(child, childName);

                    // If the child is found, break so we do not overwrite the found child. 
                    if (foundChild != null)
                    {
                        break;
                    }
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;
                    // If the child's name is set for search
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        // if the child's name is of the request name
                        foundChild = (T)child;
                        break;
                    }

                    // Need this in case the element we want is nested
                    // in another element of the same type
                    foundChild = FindChild<T>(child, childName);
                }
                else
                {
                    // child element found.
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }


        internal static T FindParent<T>(UIElement control) where T : UIElement
        {
            if (control != null)
            {
                UIElement p = VisualTreeHelper.GetParent(control) as UIElement;
                if (p != null)
                {
                    if (p is T)
                        return p as T;
                    else
                        return SfMap.FindParent<T>(p);
                }
            }
            return null;
        }
        #endregion

        #region PublicMethods

        /// <summary>
        /// Zoom the map with given zoom value.
        /// </summary>
        /// <param name="zoomValue">Scale value of the map.</param>
        /// <remarks>
        /// Use this method to zoom in or zoom out the map based on the zoomValue parameter. zoomValue is the scale value of the zoom feature.
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
        ///     public sealed partial class MainPage : Page
        ///     {
        ///         public MainPage()
        ///         {
        ///             this.InitializeComponent();
        ///             SfMap syncMap = new SfMap();
        ///             ShapeFileLayer layer = new ShapeFileLayer();
        ///             layer.Uri = "MapApp.world1.shp";            
        ///             syncMap.Layers.Add(layer);
        ///             syncMap.Zoom(2);
        ///             
        ///         }
        ///     }
        /// 
        /// }
        /// 
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public void Zoom(double zoomValue)
        {
#if WINRT
            if (this.MapView == MapViews.NormalView)
            {
#endif
                if (this.zoomPoint.X == 0 && this.zoomPoint.Y == 0)
                {
                    if (this.isPinching)
                    {
                        return;
                    }
                }

                if (this.EnableZoom)
                {

                    var time = DateTime.Now.TimeOfDay;
                    var mapLayer = this.LayeredContent;
                    if (this.ZoomLevel >= this.MinZoom && this.ZoomLevel <= this.MaxZoom)
                    {
                        this.isPan = false;
                        if (mapLayer is ShapeFileLayer)
                        {
                            (mapLayer as ShapeFileLayer).ZoomTransform.ScaleX = zoomValue;
                            (mapLayer as ShapeFileLayer).ZoomTransform.ScaleY = zoomValue;
                            double height = ((this.LayeredContent as ShapeFileLayer).LayerHeight / 2);
                            double width = ((this.LayeredContent as ShapeFileLayer).LayerWidth / 2);
                            this.commandZoomPoint = new Point(width, height);
                            this.differencePoint = (this.LayeredContent as ShapeFileLayer).LatitudeLongitudeToPoint((this.LayeredContent as ShapeFileLayer).PointToLatitudeLongitude(this.commandZoomPoint));

                            if (this.zoomPoint.X == 0 && this.zoomPoint.Y == 0)
                            {
                                if (!(this.isPinching))
                                {
                                    (mapLayer as ShapeFileLayer).PanTransform.X = -(differencePoint.X * (zoomValue - 1)) - (differencePoint.X - commandZoomPoint.X);
                                    (mapLayer as ShapeFileLayer).PanTransform.Y = -(differencePoint.Y * (zoomValue - 1)) - (differencePoint.Y - commandZoomPoint.Y);
                                }
                            }
                            else
                            {
                                (mapLayer as ShapeFileLayer).PanTransform.X = -(zoompt.X * (zoomValue - 1)) - (zoompt.X - zoomPoint.X);
                                (mapLayer as ShapeFileLayer).PanTransform.Y = -(zoompt.Y * (zoomValue - 1)) - (zoompt.Y - zoomPoint.Y);
                            }
                            (mapLayer as ShapeFileLayer).SetElementsMargin();
                            this.zoomTime = DateTime.Now.TimeOfDay;
                            this.zoomval = zoomValue;
                        }
                    }
                    (mapLayer as ShapeFileLayer).SetElementsMargin();
                    foreach (MapAnnotations symbol in (this.currentLayer as ShapeFileLayer).Annotations)
                    {
                        (this.currentLayer as ShapeFileLayer).SetCustomSymbolMargin(symbol, symbol.midpoint);

                    }
                    foreach (SubShapeFileLayer layer in this.currentLayer.SubShapeFileLayers)
                    {
                        foreach (MapAnnotations symbol in layer.Annotations)
                        {
                            (this.currentLayer as ShapeFileLayer).SetCustomSymbolMargin(symbol, symbol.midpoint);
                        }
                    }
                    if ((this.currentLayer as ShapeFileLayer).mapItemsPanel != null)
                    {
                        (this.currentLayer as ShapeFileLayer).mapItemsPanel.InvalidateArrange();
                        (this.currentLayer as ShapeFileLayer).mapItemsPanel.InvalidateMeasure();

                    }
                    foreach (SubShapeFileLayer layer in this.currentLayer.SubShapeFileLayers)
                    {
                        if (layer.mapItemsPanel != null)
                        {
                            layer.mapItemsPanel.InvalidateArrange();
                            layer.mapItemsPanel.InvalidateMeasure();

                        }
                    }
                    (this.currentLayer as ShapeFileLayer).UpdateLayout();
                    this.VirtualizeMap();
                }
#if WINRT
            }
#endif
        }
        private object GetLayeredContent()
        {
            List<object> lst = new List<object>();
            int mapIndex = this.BaseMapIndex;
            foreach (object obj in this.Layers)
            {
                lst.Add(obj);
            }
            var orderedLayers = from layers in this.Layers
                                where (layers as ShapeFileLayer).TranslateZoomLevel <= this.ZoomLevel
                                select (layers as ShapeFileLayer).TranslateZoomLevel;
            if (orderedLayers.Count() > 0)
            {
                List<ShapeFileLayer> desiredLayer;

                desiredLayer = (from layer in lst
                                where (layer as ShapeFileLayer).TranslateZoomLevel == orderedLayers.Max()
                                select (layer as ShapeFileLayer)).ToList();

                return desiredLayer;

            }

            return null;
        }

        /// <summary>
        /// Pans the map up to specific x and y values
        /// </summary>
        /// <remarks>
        /// Use this function to pan the map to given x and y values
        /// </remarks>
        /// <param name="x">X coordinate values to be Panned.</param>
        /// <param name="y">Y coordinate value to be panned.</param>
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
        ///     public sealed partial class MainPage : Page
        ///     {
        ///         public MainPage()
        ///         {
        ///             this.InitializeComponent();
        ///             SfMap syncMap = new SfMap();
        ///             ShapeFileLayer layer = new ShapeFileLayer();
        ///             layer.Uri = "MapApp.world1.shp";            
        ///             syncMap.Layers.Add(layer);
        ///             syncMap.Pan(100,200);
        ///             
        ///         }
        ///     }
        /// 
        /// }
        /// 
        /// </code>
        /// </example>
        /// 
        [ClassReference(IsReviewed = false)]
        public void Pan(double x, double y)
        {
            if (this.EnablePan)
            {
                this.isPan = true;
                var mapLayer = this.LayeredContent;
                var maxPoint = new Point(0, 0);
                if (mapLayer is ShapeFileLayer)
                {
                    if ((mapLayer as ShapeFileLayer).ShapeTransform != null)
                    {
#if WINRT
                        maxPoint = (mapLayer as ShapeFileLayer).ShapeTransform.TransformPoint(new Point((mapLayer as ShapeFileLayer).maxX, (mapLayer as ShapeFileLayer).minY));

#else
                        maxPoint = (mapLayer as ShapeFileLayer).ShapeTransform.Transform(new Point((mapLayer as ShapeFileLayer).maxX, (mapLayer as ShapeFileLayer).minY));

#endif
                    }
#if WINRT
                    var tpoint1 = (mapLayer as ShapeFileLayer).ViewTransform.TransformPoint(maxPoint);
#else
                    var tpoint1 = (mapLayer as ShapeFileLayer).ViewTransform.Transform(maxPoint);
#endif

#if WINRT
                    var tpoint2 = (mapLayer as ShapeFileLayer).ViewTransform.TransformPoint(new Point(0, 0));
#else
                    var tpoint2 = (mapLayer as ShapeFileLayer).ViewTransform.Transform(new Point(0, 0));
#endif

                    if (this.startPoint.X > this.CurrentPoint.X && this.startPoint.X != 0)
                    {
                        if (tpoint1.X < this.DesiredSize.Width)
                        {
                            x = (mapLayer as ShapeFileLayer).PanTransform.X;
                            if (prevPoint.X != 0)
                            {
                                if (prevPoint.X < this.CurrentPoint.X)
                                {
                                    this.startPoint = prevPoint;
                                    this.panTranslatePoint = new Point((this.LayeredContent as ShapeFileLayer).PanTransform.X, (this.LayeredContent as ShapeFileLayer).PanTransform.Y);
                                }
                            }
                        }
                    }
                    else if (this.startPoint.X == 0 && this.panTranslatePoint.X > x)
                    {
                        if (tpoint1.X < this.DesiredSize.Width)
                        {
                            x = (mapLayer as ShapeFileLayer).PanTransform.X;

                        }
                    }
                    if (this.startPoint.X < this.CurrentPoint.X && this.startPoint.X != 0)
                    {
                        if (tpoint2.X > 0)
                        {
                            x = (mapLayer as ShapeFileLayer).PanTransform.X;
                            if (prevPoint.X != 0)
                            {
                                if (prevPoint.X > this.CurrentPoint.X)
                                {
                                    this.startPoint = CurrentPoint;
                                    this.panTranslatePoint = new Point((this.LayeredContent as ShapeFileLayer).PanTransform.X, (this.LayeredContent as ShapeFileLayer).PanTransform.Y);
                                }
                            }
                        }
                    }
                    else if (this.startPoint.X == 0 && this.panTranslatePoint.X < x)
                    {
                        if (tpoint2.X > 0)
                        {
                            x = (mapLayer as ShapeFileLayer).PanTransform.X;
                        }
                    }
                    if (this.startPoint.Y > this.CurrentPoint.Y && this.startPoint.Y != 0)
                    {
                        if (tpoint1.Y < this.DesiredSize.Height)
                        {
                            y = (mapLayer as ShapeFileLayer).PanTransform.Y;
                            if (prevPoint.Y != 0)
                            {
                                if (prevPoint.Y < this.CurrentPoint.Y)
                                {
                                    this.startPoint = prevPoint;
                                    this.panTranslatePoint = new Point((this.LayeredContent as ShapeFileLayer).PanTransform.X, (this.LayeredContent as ShapeFileLayer).PanTransform.Y);
                                }
                            }
                        }
                    }
                    else if (this.startPoint.Y == 0 && this.panTranslatePoint.Y > y)
                    {
                        if (tpoint1.Y < this.DesiredSize.Height)
                        {
                            y = (mapLayer as ShapeFileLayer).PanTransform.Y;
                        }
                    }
                    if (this.startPoint.Y < this.CurrentPoint.Y && this.startPoint.Y != 0)
                    {
                        if (tpoint2.Y > 0)
                        {
                            y = (mapLayer as ShapeFileLayer).PanTransform.Y;
                            if (prevPoint.Y != 0)
                            {
                                if (prevPoint.Y > this.CurrentPoint.Y)
                                {
                                    this.startPoint = CurrentPoint;
                                    this.panTranslatePoint = new Point((this.LayeredContent as ShapeFileLayer).PanTransform.X, (this.LayeredContent as ShapeFileLayer).PanTransform.Y);
                                }
                            }
                        }
                    }
                    else if (this.startPoint.Y == 0 && this.panTranslatePoint.Y < y)
                    {
                        if (tpoint2.Y > 0)
                        {
                            y = (mapLayer as ShapeFileLayer).PanTransform.Y;
                        }
                    }
                    (mapLayer as ShapeFileLayer).PanTransform.X = x;
                    (mapLayer as ShapeFileLayer).PanTransform.Y = y;
                    (mapLayer as ShapeFileLayer).SetElementsMargin();
                    foreach (MapAnnotations symbol in (this.LayeredContent as ShapeFileLayer).Annotations)
                    {
                        (this.LayeredContent as ShapeFileLayer).SetCustomSymbolMargin(symbol, symbol.midpoint);
                    }

                }
                if ((this.currentLayer as ShapeFileLayer).mapItemsPanel != null)
                {
                    (this.currentLayer as ShapeFileLayer).mapItemsPanel.InvalidateArrange();
                    (this.currentLayer as ShapeFileLayer).mapItemsPanel.InvalidateMeasure();
                }
                foreach (SubShapeFileLayer layer in this.currentLayer.SubShapeFileLayers)
                {
                    if (layer.mapItemsPanel != null)
                    {
                        layer.mapItemsPanel.InvalidateArrange();
                        layer.mapItemsPanel.InvalidateMeasure();

                    }
                }

                // (this.currentLayer as ShapeFileLayer).UpdateLayout();
                this.VirtualizeMap();

#if !WPF
                if (!this.isPointerPressed)
                {
                    this.panTranslatePoint = new Point((this.LayeredContent as ShapeFileLayer).PanTransform.X, (this.LayeredContent as ShapeFileLayer).PanTransform.Y);

                }
#endif
                prevPoint = this.CurrentPoint;
            }
        }
        internal void VirtualizeMap()
        {
            foreach (MapShape shp in (this.LayeredContent as ShapeFileLayer).MapShapes)
            {
                Rect mapRect = new Rect(new Point(0, 0), new Size(this.RectClip.Width, this.RectClip.Height));
                mapRect.Intersect((this.LayeredContent as ShapeFileLayer).ViewTransform.TransformBounds(shp.Shape.Data.Bounds));
                if (mapRect == Rect.Empty)
                {
                    if ((this.LayeredContent as ShapeFileLayer).VisibleShapes.Count > 1)
                    {
                        (this.LayeredContent as ShapeFileLayer).VisibleShapes.Remove(shp);
                    }
                }
                else
                {
                    if (!(this.LayeredContent as ShapeFileLayer).VisibleShapes.Contains(shp))
                    {
                        (this.LayeredContent as ShapeFileLayer).VisibleShapes.Add(shp);
                    }
                }
            }

        }


        internal static double ConvertLattideLongitudeValues(string point)
        {
            double result = 0d;
            var dir = point.Substring(point.Length - 1, 1).ToString().ToLower();
            switch (dir)
            {
                case "n":
                    double.TryParse(point.Substring(0, point.Length - 1).ToString(), out result);
                    break;
                case "e":
                    double.TryParse(point.Substring(0, point.Length - 1).ToString(), out result);
                    break;
                case "w":
                    double.TryParse(point.Substring(0, point.Length - 1).ToString(), out result);
                    result = (-result);
                    break;
                case "s":
                    double.TryParse(point.Substring(0, point.Length - 1).ToString(), out result);
                    result = (-result);
                    break;
            }
            return result;
        }
        #endregion

        #region Commands

        private ICommand resetCommand;

        /// <summary>
        /// Gets the ResetCommand for SfMap..
        /// </summary>
        /// <value>
        /// Type <see cref="ICommand"/>
        /// </value>
        /// <remarks>
        /// Use this property to invoke Reset command of the map. This command will clear all the elements of the map.
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
        ///     public sealed partial class MainPage : Page
        ///     {
        ///         public MainPage()
        ///         {
        ///             this.InitializeComponent();
        ///             SfMap syncMap = new SfMap();
        ///             ShapeFileLayer layer = new ShapeFileLayer();
        ///             layer.Uri = "MapApp.world1.shp";
        ///             Button cmdButton = new Button();
        ///             cmdButton.Command = syncMap.ResetCommand;
        ///             syncMap.Layers.Add(layer);
        ///         }
        ///     }    
        /// }
        /// 
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public ICommand ResetCommand
        {
            get
            {
                return this.resetCommand;
            }
        }

        private ICommand refreshCommand;

        /// <summary>
        /// Gets the Refresh Command for the Maps.Invokes refresh functionality of the map.
        /// </summary>
        /// <value>
        /// Type :<see cref="ICommand"/>
        /// </value>
        /// <remarks>
        /// Use this command to refresh the map.While refreshing the map, all elements will be cleared then added again.
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
        ///     public sealed partial class MainPage : Page
        ///     {
        ///         public MainPage()
        ///         {
        ///             this.InitializeComponent();
        ///             SfMap syncMap = new SfMap();
        ///             ShapeFileLayer layer = new ShapeFileLayer();
        ///             layer.Uri = "MapApp.world1.shp";
        ///             Button cmdButton = new Button();
        ///             cmdButton.Command = syncMap.RefreshCommand;
        ///             syncMap.Layers.Add(layer);
        ///         }
        ///     }    
        /// }
        /// 
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public ICommand RefreshCommand
        {
            get
            {
                return this.refreshCommand;
            }
        }


        private ICommand panResetCommand;

        /// <summary>
        /// Gets the command for resetting the pan values.
        /// </summary>
        /// <remarks>
        /// Use this command to reset the pan values of the map.
        /// </remarks>
        /// <value>
        /// Type :<see cref="ICommand"/>
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
        ///             Button cmdButton = new Button();
        ///             cmdButton.Command = syncMap.PanResetCommand;
        ///             syncMap.Layers.Add(layer);
        ///         }
        ///     }    
        /// }
        /// 
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public ICommand PanResetCommand
        {
            get
            {
                return this.panResetCommand;
            }
        }

        private ICommand zoomResetCommand;

        /// <summary>
        /// Gets the command for reset the zoom of the map.
        /// </summary>
        /// <remarks>
        /// Use this command to reset the zoom of the map.
        /// </remarks>
        /// <value>
        /// Type :<see cref="ICommand"/>
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
        ///             Button cmdButton = new Button();
        ///             cmdButton.Command = syncMap.ZoomResetCommand;
        ///             syncMap.Layers.Add(layer);
        ///         }
        ///     }    
        /// }
        /// 
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public ICommand ZoomResetCommand
        {
            get
            {
                return this.zoomResetCommand;
            }
        }

        private ICommand zoomInCommand;

        /// <summary>
        /// Gets the command for Zoom in the map.
        /// </summary>
        /// <remarks>
        /// Use this command to zoom in the map.
        /// </remarks>
        /// <value>
        /// Type :<see cref="ICommand"/>
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
        ///             Button cmdButton = new Button();
        ///             cmdButton.Command = syncMap.ZoomInCommand;
        ///             syncMap.Layers.Add(layer);
        ///         }
        ///     }    
        /// }
        /// 
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public ICommand ZoomInCommand
        {
            get
            {
                return this.zoomInCommand;
            }
        }

        private ICommand zoomOutCommand;

        /// <summary>
        /// Gets the Command for zoom out the map.
        /// </summary>
        /// <value>
        /// Type :<see cref="ICommand"/>
        /// </value>
        /// <remarks>
        /// Use this command to zoom out the map.
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
        ///     public sealed partial class MainPage : Page
        ///     {
        ///         public MainPage()
        ///         {
        ///             this.InitializeComponent();
        ///             SfMap syncMap = new SfMap();
        ///             ShapeFileLayer layer = new ShapeFileLayer();
        ///             layer.Uri = "MapApp.world1.shp";
        ///             Button cmdButton = new Button();
        ///             cmdButton.Command = syncMap.ZoomOutCommand;
        ///             syncMap.Layers.Add(layer);
        ///         }
        ///     }    
        /// }
        /// 
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public ICommand ZoomOutCommand
        {
            get
            {
                return this.zoomOutCommand;
            }
        }

        private ICommand panCommand;

        /// <summary>
        /// Gets the command for panning the map.
        /// </summary>
        /// <value>
        /// Type :<see cref="ICommand"/>
        /// </value>
        /// <remarks>
        /// Use this command to pan the map. Command parameter will specify the direction to be panned.
        /// <para>There are four command parameters can be given,</para>
        /// <para>Left,Right,Top and Bottom</para>
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
        ///     public sealed partial class MainPage : Page
        ///     {
        ///         public MainPage()
        ///         {
        ///             this.InitializeComponent();
        ///             SfMap syncMap = new SfMap();
        ///             ShapeFileLayer layer = new ShapeFileLayer();
        ///             layer.Uri = "MapApp.world1.shp";
        ///             Button topPan = new Button();
        ///             topPan.Command = syncMap.PanCommand;
        ///             topPan.CommandParameter = "Top";
        ///             Button bottomPan = new Button();
        ///             bottomPan.Command = syncMap.PanCommand;
        ///             bottomPan.CommandParameter = "Bottom";
        ///             Button leftPan = new Button();
        ///             leftPan.Command = syncMap.PanCommand;
        ///             leftPan.CommandParameter = "Left";
        ///             Button rightPan = new Button();
        ///             rightPan.Command = syncMap.PanCommand;
        ///             rightPan.CommandParameter = "Right";
        ///             syncMap.Layers.Add(layer);
        ///         }
        ///     }    
        /// }
        /// 
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public ICommand PanCommand
        {
            get
            {
                return this.panCommand;
            }
        }

        #endregion

        #region LayerChangeAnimation




        #endregion

        internal static byte InterpolarateColor(byte colorvalue1, byte colorvalue2, double actualvalue)
        {
            return (byte)(colorvalue1 * (1 - colorvalue2) + colorvalue2 * actualvalue);
        }

        internal Color GetColor(object value, ObservableCollection<ColorMapping> colorMapping, Color DefaultColor)
        {
            try
            {
                double v = double.Parse(value.ToString());

#if SILVERLIGHT || WINDOWSPHONE || SILVERLIGHT_5
                Dictionary<double, Color> d = new Dictionary<double, Color>();
#else
                SortedDictionary<double, Color> d = new SortedDictionary<double, Color>();
#endif
                var blue = (byte)((Colors.Red.R * 30 / 100) + (Colors.Red.R * 70 / 100));
                var order = new ObservableCollection<ColorMapping>(colorMapping.OrderBy(color => (color as RangeColorMapping).Range));

                foreach (RangeColorMapping map in order)
                {
                    if (!d.Keys.Contains(map.Range))
                    {
                        d.Add(map.Range, map.Color);
                    }
                }

                KeyValuePair<double, Color> kvp_previous = new KeyValuePair<double, Color>(-1d, Colors.Black);
                foreach (KeyValuePair<double, Color> kvp in d)
                {
                    if (kvp.Key > v)
                    {
                        double p = (v - kvp_previous.Key) / (double)(kvp.Key - kvp_previous.Key);
                        var r = (byte)((((Color)kvp_previous.Value).R * (1 - p)) + (((Color)kvp.Value).R * p));
                        var g = (byte)((((Color)kvp_previous.Value).G * (1 - p)) + (((Color)kvp.Value).G * p));
                        var b = (byte)((((Color)kvp_previous.Value).B * (1 - p)) + (((Color)kvp.Value).B * p));
                        Color c = Color.FromArgb(255, r, g, b);
                        return c;
                    }
                    else if (kvp.Key == v)
                    {
                        return kvp.Value;
                    }
                    else
                    {
                        kvp_previous = kvp;
                    }
                }

                return DefaultColor;
            }
            catch
            {
            }

            return DefaultColor;
        }

        internal Color GetValueColor(object str, ObservableCollection<ColorMapping> colorMapping, Color DefaultColor)
        {
            Color color = DefaultColor;

            foreach (var colormap in colorMapping)
            {
                if (colormap.Validate(str))
                {
                    return colormap.Color;
                }
            }

            return color;
        }
    }
}
