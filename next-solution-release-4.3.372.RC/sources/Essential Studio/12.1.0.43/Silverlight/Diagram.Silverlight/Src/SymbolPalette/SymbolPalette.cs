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
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;
    using System.ComponentModel;
    using Syncfusion.Windows.Shared;
    using System.Diagnostics;    

    /// <summary>
    /// 
    /// </summary>
#if !SyncfusionFramework3_5
    [DesignTimeVisible(false)]
#endif
      
    public class SymbolPalette : ItemsControl
    {        
#if WPF
        internal Brush nodeFillBrush;

        internal Brush nodeStrokeBrush;
#endif
        /// <summary>
        /// Gets or sets the pop up border brush.
        /// </summary>
        /// <value>The pop up border brush.</value>
        /// <remarks>
        /// Default value is Chocolate.
        /// </remarks>
        /// <example>
        /// C#:
        /// <code language="C#">
        /// DiagramControl diagramControl=new DiagramControl();
        /// diagramControl.SymbolPalette.PopUpBorderBrush=Brushes.Violet;
        /// </code>
        /// </example>
        public Brush PopUpBorderBrush
        {
            get
            {
                return (Brush)GetValue(PopUpBorderBrushProperty);
            }

            set
            {
                SetValue(PopUpBorderBrushProperty, value);
            }
        }
#if WPF
        #region Properties

        /// <summary>
        /// Gets or sets the padding for the SymbolPaletteItem.
        /// </summary>
        /// <value>The padding for the SymbolPaletteItem</value>
        /// <remarks>
        /// Default value is 0.
        /// </remarks>
        /// <example>
        /// C#:
        /// <code language="C#">
        /// DiagramControl diagramControl=new DiagramControl();
        /// diagramControl.SymbolPalette.ItemPadding=new Thickness(2);
        /// </code>
        /// </example>
        public Thickness ItemPadding
        {
            get
            {
                return (Thickness)GetValue(ItemPaddingProperty);
            }

            set
            {
                SetValue(ItemPaddingProperty, value);
            }
        }


        /// <summary>
        /// Gets or sets the pop up border thickness.
        /// </summary>
        /// <value>The pop up border thickness.</value>
        /// <remarks>
        /// Default value is (0,1,1,1).
        /// </remarks>
        /// <example>
        /// C#:
        /// <code language="C#">
        /// DiagramControl diagramControl=new DiagramControl();
        /// diagramControl.SymbolPalette.PopUpBorderThickness=new Thickness(2);
        /// </code>
        /// </example>
        public Thickness PopUpBorderThickness
        {
            get
            {
                return (Thickness)GetValue(PopUpBorderThicknessProperty);
            }

            set
            {
                SetValue(PopUpBorderThicknessProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the SymbolPaletteItem border thickness.
        /// </summary>
        /// <value>The item border thickness.</value>
        /// <remarks>
        /// Default value is 1.
        /// </remarks>
        /// <example>
        /// C#:
        /// <code language="C#">
        /// DiagramControl diagramControl=new DiagramControl();
        /// diagramControl.SymbolPalette.ItemBorderThickness=new Thickness(2);
        /// </code>
        /// </example>
        public Thickness ItemBorderThickness
        {
            get
            {
                return (Thickness)GetValue(ItemBorderThicknessProperty);
            }

            set
            {
                SetValue(ItemBorderThicknessProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the SymbolPaletteItem corner radius.
        /// </summary>
        /// <value>The item corner radius.</value>
        /// <remarks>
        /// Default value is 2.
        /// </remarks>
        /// <example>
        /// C#:
        /// <code language="C#">
        /// DiagramControl diagramControl=new DiagramControl();
        /// diagramControl.SymbolPalette.ItemCornerRadius=new CornerRadius(4);
        /// </code>
        /// </example>
        public CornerRadius ItemCornerRadius
        {
            get
            {
                return (CornerRadius)GetValue(ItemCornerRadiusProperty);
            }

            set
            {
                SetValue(ItemCornerRadiusProperty, value);
            }
        }

        
        /// <summary>
        /// Gets or sets a value indicating whether this instance has filters.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has filters; otherwise, <c>false</c>.
        /// </value>
        /// <example>
        /// C#:
        /// <code language="C#">
        /// using Syncfusion.Windows.Diagram;
        /// namespace WpfApplication1
        /// {
        /// public partial class Window1 : Window
        /// {
        ///    public DiagramControl Control;
        ///    public DiagramModel Model;
        ///    public DiagramView View;
        ///    public Window1 ()
        ///    {
        ///        InitializeComponent ();
        ///        Control = new DiagramControl ();
        ///        Model = new DiagramModel ();
        ///        View = new DiagramView ();
        ///        Control.View = View;
        ///        Control.Model = Model;
        ///        View.Bounds = new Thickness(0, 0, 1000, 1000);
        ///        bool value = Control.SymbolPalette.HasFilters; 
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        /// <remarks>This property cannot be set. Only the value can be obtained to check if <see cref="SymbolPalette"/> has any filters.</remarks>
        //public bool HasFilters
        //{
        //    get
        //    {
        //        return (bool)GetValue(HasFiltersProperty);
        //    }

        //    protected set
        //    {
        //        SetValue(HasFiltersPropertyKey, value);
        //    }
        //}

        /// <summary>
        /// Gets or sets the SymbolPalette item width.
        /// </summary>
        /// <value>
        /// Type: <see cref="int"/>
        /// Width of the item in pixels.
        /// </value>
        /// <example>
        /// C#:
        /// <code language="C#">
        /// using Syncfusion.Windows.Diagram;
        /// namespace WpfApplication1
        /// {
        /// public partial class Window1 : Window
        /// {
        ///    public DiagramControl Control;
        ///    public DiagramModel Model;
        ///    public DiagramView View;
        ///    public Window1 ()
        ///    {
        ///        InitializeComponent ();
        ///        Control = new DiagramControl ();
        ///        Model = new DiagramModel ();
        ///        View = new DiagramView ();
        ///        Control.View = View;
        ///        Control.Model = Model;
        ///        View.Bounds = new Thickness(0, 0, 1000, 1000);
        ///        Control.SymbolPalette.ItemWidth = 40;
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        public double ItemWidth
        {
            get
            {
                return (double)GetValue(ItemWidthProperty);
            }

            set
            {
                SetValue(ItemWidthProperty, value);
            }
        }

        /// <summary>
        /// Defines the SymbolPalette item width.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemWidthProperty =
            DependencyProperty.Register("ItemWidth", typeof(double), typeof(SymbolPalette), new FrameworkPropertyMetadata(40d));

        /// <summary>
        /// Gets or sets the SymbolPalette item height.
        /// </summary>
        /// <value>
        /// Type: <see cref="int"/>
        /// Height of the item in pixels.
        /// </value>
        /// <example>
        /// C#:
        /// <code language="C#">
        /// using Syncfusion.Windows.Diagram;
        /// namespace WpfApplication1
        /// {
        /// public partial class Window1 : Window
        /// {
        ///    public DiagramControl Control;
        ///    public DiagramModel Model;
        ///    public DiagramView View;
        ///    public Window1 ()
        ///    {
        ///        InitializeComponent ();
        ///        Control = new DiagramControl ();
        ///        Model = new DiagramModel ();
        ///        View = new DiagramView ();
        ///        Control.View = View;
        ///        Control.Model = Model;
        ///        View.Bounds = new Thickness(0, 0, 1000, 1000);
        ///        Control.SymbolPalette.ItemHeight = 40;
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        public double ItemHeight
        {
            get
            {
                return (double)GetValue(ItemHeightProperty);
            }

            set
            {
                SetValue(ItemHeightProperty, value);
            }
        }

        /// <summary>
        /// Defines the SymbolPalette item height.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemHeightProperty =
            DependencyProperty.Register("ItemHeight", typeof(double), typeof(SymbolPalette), new FrameworkPropertyMetadata(40d));

        /// <summary>
        /// Gets or sets the internal symbol filters.
        /// </summary>
        /// <value>The internal symbol filters.</value>
        /*internal ObservableCollection<SymbolPaletteFilter> InternalSymbolFilters
        {
            get
            {
                return (ObservableCollection<SymbolPaletteFilter>)GetValue(InternalGalleryFiltersProperty);
            }

            set
            {
                SetValue(InternalGalleryFiltersProperty, value);
            }
        }*/

        /// <summary>
        /// Gets or sets a value indicating whether <see cref="SymbolPalette"/> has any <see cref="SymbolPaletteGroup"/>.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has groups; otherwise, <c>false</c>.
        /// </value>
        /// <example>
        /// C#:
        /// <code language="C#">
        /// using Syncfusion.Windows.Diagram;
        /// namespace WpfApplication1
        /// {
        /// public partial class Window1 : Window
        /// {
        /// public DiagramControl Control;
        /// public DiagramModel Model;
        /// public DiagramView View;
        /// public Window1 ()
        /// {
        /// InitializeComponent ();
        /// Control = new DiagramControl ();
        /// Model = new DiagramModel ();
        /// View = new DiagramView ();
        /// Control.View = View;
        /// Control.Model = Model;
        /// View.Bounds = new Thickness(0, 0, 1000, 1000);
        /// bool value = Control.SymbolPalette.HasGroups;
        /// }
        /// }
        /// }
        /// </code>
        /// </example>
        //public bool HasGroups
        //{
        //    get
        //    {
        //        return (bool)GetValue(HasGroupsProperty);
        //    }

        //    protected set
        //    {
        //        SetValue(HasGroupsPropertyKey, value);
        //    }
        //}

        public Brush ItemMouseOverForegroundBrush
        {
            get { return (Brush)GetValue(ItemMouseOverForegroundBrushProperty); }
            set { SetValue(ItemMouseOverForegroundBrushProperty, value); }
        }

        /// <summary>
        /// Identifies the ItemMouseOverForegroundBrush dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemMouseOverForegroundBrushProperty =
            DependencyProperty.Register("ItemMouseOverForegroundBrush", typeof(Brush), typeof(SymbolPalette), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 245, 245, 245))));



        #endregion
#endif
        /// <summary>
        /// Identifies the BorderHeight dependency property.
        /// </summary>
        internal static readonly DependencyProperty BorderHeightProperty = DependencyProperty.Register("BorderHeight", typeof(double), typeof(SymbolPalette), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the BorderWidth dependency property.
        /// </summary>
        internal static readonly DependencyProperty BorderWidthProperty = DependencyProperty.Register("BorderWidth", typeof(double), typeof(SymbolPalette), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the CustomHeaderText dependency property.
        /// </summary>
        public static readonly DependencyProperty CustomHeaderTextProperty = DependencyProperty.Register("CustomHeaderText", typeof(string), typeof(SymbolPalette), new PropertyMetadata("CustomColors"));

        /// <summary>
        /// Defines SymbolPalette filter indexes.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty FilterIndexesProperty =
            DependencyProperty.RegisterAttached("FilterIndexes", typeof(IList<int>), typeof(SymbolPalette), new PropertyMetadata(null, new PropertyChangedCallback(OnFilterIndexesChanged)));

        /// <summary>
        /// Defines the collection of InternalSymbolPalette filters.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty InternalFiltersProperty =
           DependencyProperty.Register("InternalSymbolFilters", typeof(ObservableCollection<SymbolPaletteFilter>), typeof(SymbolPalette), new PropertyMetadata(null));


        public static readonly DependencyProperty CurrentFilterProperty =
            DependencyProperty.Register("CurrentFilter", typeof(SymbolPaletteFilter), typeof(SymbolPalette), new PropertyMetadata(null, new PropertyChangedCallback(OnCurrentFilterChanged)));

        internal bool IsChecked = false;
        internal bool Isloaded = false;

        internal bool IsSelected = false;

        /// <summary>
        /// Identifies the PopUpBackground dependency property.
        /// </summary>
        public static readonly DependencyProperty PopUpBackgroundProperty =
        DependencyProperty.Register("PopUpBackground", typeof(Brush), typeof(SymbolPalette), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 245, 245, 245))));

        /// <summary>
        /// Identifies the PopUpForeground dependency property.
        /// </summary>
        public static readonly DependencyProperty PopUpForegroundProperty =
        DependencyProperty.Register("PopUpForeground", typeof(Brush), typeof(SymbolPalette), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 47, 79, 79))));

        /// <summary>
        /// Identifies the PopUpBorderBrush dependency property.
        /// </summary>
        public static readonly DependencyProperty PopUpBorderBrushProperty =
        DependencyProperty.Register("PopUpBorderBrush", typeof(Brush), typeof(SymbolPalette), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 210, 105, 30))));

        /// <summary>
        /// Identifies the PopUpItemMouseOverBrush dependency property.
        /// </summary>
        public static readonly DependencyProperty PopUpItemMouseOverBrushProperty =
        DependencyProperty.Register("PopUpItemMouseOverBrush", typeof(Brush), typeof(SymbolPalette), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 255, 160, 122))));



        public Thickness SymbolPaletteGroupBorderThickness
        {
            get { return (Thickness)GetValue(SymbolPaletteGroupBorderThicknessProperty); }
            set { SetValue(SymbolPaletteGroupBorderThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SymbolPaletteGroupBorderThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SymbolPaletteGroupBorderThicknessProperty =
            DependencyProperty.Register("SymbolPaletteGroupBorderThickness", typeof(Thickness), typeof(SymbolPalette), new PropertyMetadata(new Thickness(0,0,0,1)));

        

        /// <summary>
        /// Identifies the PopUpItemMouseOverBorderBrush dependency property.
        /// </summary>
        public static readonly DependencyProperty PopUpItemMouseOverBorderBrushProperty =
        DependencyProperty.Register("PopUpItemMouseOverBorderBrush", typeof(Brush), typeof(SymbolPalette), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 255, 160, 122))));

        /// <summary>
        /// Identifies the PopUpBorderThickness dependency property.
        /// </summary>
        public static readonly DependencyProperty PopUpBorderThicknessProperty =
        DependencyProperty.Register("PopUpBorderThickness", typeof(Thickness), typeof(SymbolPalette), new PropertyMetadata(new Thickness(0, 1, 1, 1)));

        /// <summary>
        /// Identifies the PopUpLeftColumnBackground dependency property.
        /// </summary>
        public static readonly DependencyProperty PopUpLeftColumnBackgroundProperty =
        DependencyProperty.Register("PopUpLeftColumnBackground", typeof(Brush), typeof(SymbolPalette), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 211,211,211))));

        /// <summary>
        /// Identifies the CheckerBorderBrush dependency property.
        /// </summary>
        public static readonly DependencyProperty CheckerBorderBrushProperty =
        DependencyProperty.Register("CheckerBorderBrush", typeof(Brush), typeof(SymbolPalette), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 47, 79, 79))));

        /// <summary>
        /// Identifies the CheckerBackground dependency property.
        /// </summary>
        public static readonly DependencyProperty CheckerBackgroundProperty =
       DependencyProperty.Register("CheckerBackground", typeof(Brush), typeof(SymbolPalette), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 255, 228, 196))));

        /// <summary>
        /// Identifies the CheckerTickBrush dependency property.
        /// </summary>
        public static readonly DependencyProperty CheckerTickBrushProperty =
       DependencyProperty.Register("CheckerTickBrush", typeof(Brush), typeof(SymbolPalette), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 47, 79, 79))));

         ///<summary>
         ///Identifies the FilterSelectorBackground dependency property.
         ///</summary>
        public static readonly DependencyProperty FilterSelectorBackgroundProperty =
        DependencyProperty.Register("FilterSelectorBackground", typeof(Brush), typeof(SymbolPalette), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 210, 105, 30))));

        /// <summary>
        /// Identifies the FilterSelectorForeground dependency property.
        /// </summary>
        public static readonly DependencyProperty FilterSelectorForegroundProperty =
        DependencyProperty.Register("FilterSelectorForeground", typeof(Brush), typeof(SymbolPalette), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 47, 79, 79))));

        /// <summary>
        /// Identifies the FilterSelectorMouseOverForeground dependency property.
        /// </summary>
        public static readonly DependencyProperty FilterSelectorMouseOverForegroundProperty =
        DependencyProperty.Register("FilterSelectorMouseOverForeground", typeof(Brush), typeof(SymbolPalette), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 255, 255, 255))));

        /// <summary>
        /// Identifies the FilterSelectorBorderBrush dependency property.
        /// </summary>
        public static readonly DependencyProperty FilterSelectorBorderBrushProperty =
        DependencyProperty.Register("FilterSelectorBorderBrush", typeof(Brush), typeof(SymbolPalette), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 210, 105, 30))));

        /// <summary>
        /// Identifies the FilterSelectorBorderThickness dependency property.
        /// </summary>
        public static readonly DependencyProperty FilterSelectorBorderThicknessProperty =
        DependencyProperty.Register("FilterSelectorBorderThickness", typeof(Thickness), typeof(SymbolPalette), new PropertyMetadata(new Thickness(0, 0, 0, 1)));





       

        // Using a DependencyProperty as the backing store for FilterSelectorDropdownBackgroundBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FilterSelectorDropdownBackgroundBrushProperty =
            DependencyProperty.Register("FilterSelectorDropdownBackgroundBrush", typeof(Brush), typeof(SymbolPalette), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));

        

        // Using a DependencyProperty as the backing store for FilterSelectorDropdownBorderBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FilterSelectorDropdownBorderBrushProperty =
            DependencyProperty.Register("FilterSelectorDropdownBorderBrush", typeof(Brush), typeof(SymbolPalette), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));

        

        /// <summary>
        /// Identifies the ItemBorderThickness dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemBorderThicknessProperty =
        DependencyProperty.Register("ItemBorderThickness", typeof(Thickness), typeof(SymbolPalette), new PropertyMetadata(new Thickness(1)));

        /// <summary>
        /// Identifies the ItemCheckedBorderBrush dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemCheckedBorderBrushProperty =
        DependencyProperty.Register("ItemCheckedBorderBrush", typeof(Brush), typeof(SymbolPalette), new PropertyMetadata(new SolidColorBrush(Colors.Red)));

        /// <summary>
        /// Identifies the ItemCheckedMouseOverBorderBrush dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemCheckedMouseOverBorderBrushProperty =
        DependencyProperty.Register("ItemCheckedMouseOverBorderBrush", typeof(Brush), typeof(SymbolPalette), new PropertyMetadata(new SolidColorBrush(Colors.Green)));

        /// <summary>
        /// Identifies the ItemMouseOverBorderBrush dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemMouseOverBorderBrushProperty =
        DependencyProperty.Register("ItemMouseOverBorderBrush", typeof(Brush), typeof(SymbolPalette), new PropertyMetadata(new SolidColorBrush(Colors.Orange)));

        /// <summary>
        /// Identifies the ItemCheckedBorderBrush dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemCheckedBackgroundBrushProperty =
        DependencyProperty.Register("ItemCheckedBackgroundBrush", typeof(Brush), typeof(SymbolPalette), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));

        /// <summary>
        /// Identifies the ItemCheckedMouseOverBorderBrush dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemCheckedMouseOverBackgroundBrushProperty =
        DependencyProperty.Register("ItemCheckedMouseOverBackgroundBrush", typeof(Brush), typeof(SymbolPalette), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));

        /// <summary>
        /// Identifies the ItemMouseOverBorderBrush dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemMouseOverBackgroundBrushProperty =
        DependencyProperty.Register("ItemMouseOverBackgroundBrush", typeof(Brush), typeof(SymbolPalette), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));

        /// <summary>
        /// Identifies the ItemCornerRadius dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemCornerRadiusProperty =
        DependencyProperty.Register("ItemCornerRadius", typeof(CornerRadius), typeof(SymbolPalette), new PropertyMetadata(new CornerRadius(2)));

        /// <summary>
        /// Identifies the ItemPadding dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemPaddingProperty =
        DependencyProperty.Register("ItemPadding", typeof(Thickness), typeof(SymbolPalette), new PropertyMetadata(new Thickness(0)));

        /// <summary>
        /// Defines the selected SymbolPalette item.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(SymbolPaletteItem), typeof(SymbolPalette), new PropertyMetadata(null, new PropertyChangedCallback(OnSelectedItemChanged)));

        public static readonly DependencyProperty SymbolGroupsProperty =
           DependencyProperty.Register("SymbolGroups", typeof(ObservableCollection<SymbolPaletteGroup>), typeof(SymbolPalette), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the SymbolPaletteGroupBackground dependency property.
        /// </summary>
        public static readonly DependencyProperty SymbolPaletteGroupBackgroundProperty =
        DependencyProperty.Register("SymbolPaletteGroupBackground", typeof(Brush), typeof(SymbolPalette), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255,255, 228, 196))));

        /// <summary>
        /// Identifies the SymbolPaletteGroupBorderBrush dependency property.
        /// </summary>
        public static readonly DependencyProperty SymbolPaletteGroupBorderBrushProperty =
        DependencyProperty.Register("SymbolPaletteGroupBorderBrush", typeof(Brush), typeof(SymbolPalette), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255,210,105,30))));

        /// <summary>
        /// Identifies the SymbolPaletteGroupForeground dependency property.
        /// </summary>
        public static readonly DependencyProperty SymbolPaletteGroupForegroundProperty =
        DependencyProperty.Register("SymbolPaletteGroupForeground", typeof(Brush), typeof(SymbolPalette), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 139, 69, 19))));

        public static readonly DependencyProperty FiltersProperty =
           DependencyProperty.Register("SymbolFilters", typeof(ObservableCollection<SymbolPaletteFilter>), typeof(SymbolPalette), new PropertyMetadata(null));
        
        /// <summary>
        /// Identifies the Preview of the SymbolPaletteItem dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowPreviewProperty = DependencyProperty.Register("ShowPreview", typeof(bool), typeof(SymbolPalette), new PropertyMetadata(true));

        /// <summary>
        /// Identifies the ItemGenerateMode dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemGenerateModeProperty = DependencyProperty.Register("ItemGenerateMode", typeof(ItemGenerateMode), typeof(SymbolPalette), new PropertyMetadata(ItemGenerateMode.Manual, OnItemGenerateModeChanged));

        /// <summary>
        /// Identifies the ShowFilter dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowFilterProperty =
            DependencyProperty.Register("ShowFilter", typeof(bool), typeof(SymbolPalette), new PropertyMetadata(true));


        internal ResourceDictionary symbols;
        internal bool updownclick = false;
        
        static SymbolPalette()
        {
#if WPF
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SymbolPalette), new FrameworkPropertyMetadata(typeof(SymbolPalette)));
#endif
        }

        /// <summary>
        /// Initialiazes a new instance of SymbolPalette control
        /// </summary>
        public SymbolPalette()
        {
#if SILVERLIGHT
            DefaultStyleKey = typeof(SymbolPalette);
#endif
            this.symbols = new ResourceDictionary();
#if SILVERLIGHT
            this.symbols.Source = new Uri("/Syncfusion.Diagram.Silverlight;component/Themes/NodeShapes.xaml", UriKind.RelativeOrAbsolute);
#endif
#if WPF
            
            this.symbols.Source = new Uri("/Syncfusion.Diagram.WPF;component/Themes/NodeShapes.xaml", UriKind.RelativeOrAbsolute);
            PresentationTraceSources.DataBindingSource.Switch.Level = SourceLevels.Off;
#endif
            this.SymbolGroups = new ObservableCollection<SymbolPaletteGroup>();
            SymbolFilters = new ObservableCollection<SymbolPaletteFilter>();
            InternalSymbolFilters = new ObservableCollection<SymbolPaletteFilter>();
            SymbolFilters.CollectionChanged += new NotifyCollectionChangedEventHandler(SymbolPaletteFilters_CollectionChanged);
            this.SymbolGroups.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(this.SymbolGroups_CollectionChanged);
            this.LoadPalette();
            this.LoadFilters();
        }

        /// <summary>
        /// Event that is raised when SelectedItem property is changed.
        /// </summary>
        public event PropertyChangedCallback SelectedItemChanged;

        /// <summary>
        /// Event that is raised when CurrentFilter property is changed.
        /// </summary>
        public event PropertyChangedCallback CurrentFilterChanged;
        
        /// <summary>
        /// Gets or sets the value of the ShowPreview dependency property.
        /// </summary>       
        /// <value>
        /// Type : bool
        /// </value>
        public bool ShowPreview
        {
            get
            {
                return (bool)GetValue(ShowPreviewProperty);
            }

            set
            {
                SetValue(ShowPreviewProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the ItemGenerateMode dependency property.
        /// </summary>       
        /// <value>
        /// Type : ItemGenerateMode
        /// </value>
        public ItemGenerateMode ItemGenerateMode
        {
            get
            {
                return (ItemGenerateMode)GetValue(ItemGenerateModeProperty);
            }

            set
            {
                SetValue(ItemGenerateModeProperty, value);
            }
        }
        
        /// <summary>
        /// Gets or sets the value of the BorderHeight dependency property.
        /// </summary>       
        /// <value>
        /// Type : Double
        /// </value>
        internal double BorderHeight
        {
            get
            {
                return (double)GetValue(BorderHeightProperty);
            }

            set
            {
                SetValue(BorderHeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the BorderWidth dependency property.
        /// </summary>       
        /// <value>
        /// Type : Double
        /// </value>
        internal double BorderWidth
        {
            get
            {
                return (double)GetValue(BorderWidthProperty);
            }

            set
            {
                SetValue(BorderWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the pop up background.
        /// </summary>
        /// <value>The pop up background.</value>
        /// <remarks>
        /// Default value is WhiteSmoke.
        /// </remarks>
        /// <example>
        /// C#:
        /// <code language="C#">
        /// DiagramControl diagramControl=new DiagramControl();
        /// diagramControl.SymbolPalette.PopUpBackground=Brushes.White;
        /// </code>
        /// </example>
        public Brush PopUpBackground
        {
            get
            {
                return (Brush)GetValue(PopUpBackgroundProperty);
            }

            set
            {
                SetValue(PopUpBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the pop up foreground.
        /// </summary>
        /// <value>The pop up foreground.</value>
        /// <remarks>
        /// Default value is DarkSlateGray.
        /// </remarks>
        /// <example>
        /// C#:
        /// <code language="C#">
        /// DiagramControl diagramControl=new DiagramControl();
        /// diagramControl.SymbolPalette.PopUpForeground=Brushes.Black;
        /// </code>
        /// </example>
        public Brush PopUpForeground
        {
            get
            {
                return (Brush)GetValue(PopUpForegroundProperty);
            }

            set
            {
                SetValue(PopUpForegroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the pop up item mouse over brush.
        /// </summary>
        /// <value>The pop up item mouse over brush.</value>
        /// <remarks>
        /// Default value is LightSalmon.
        /// </remarks>
        /// <example>
        /// C#:
        /// <code language="C#">
        /// DiagramControl diagramControl=new DiagramControl();
        /// diagramControl.SymbolPalette.PopUpItemMouseOverBrush=Brushes.Blue;
        /// </code>
        /// </example>
        public Brush PopUpItemMouseOverBrush
        {
            get
            {
                return (Brush)GetValue(PopUpItemMouseOverBrushProperty);
            }

            set
            {
                SetValue(PopUpItemMouseOverBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the pop up item mouse over brush.
        /// </summary>
        /// <value>The pop up item mouse over brush.</value>
        /// <remarks>
        /// Default value is LightSalmon.
        /// </remarks>
        /// <example>
        /// C#:
        /// <code language="C#">
        /// DiagramControl diagramControl=new DiagramControl();
        /// diagramControl.SymbolPalette.PopUpItemMouseOverBorderBrush=Brushes.Blue;
        /// </code>
        /// </example>
        public Brush PopUpItemMouseOverBorderBrush
        {
            get
            {
                return (Brush)GetValue(PopUpItemMouseOverBorderBrushProperty);
            }

            set
            {
                SetValue(PopUpItemMouseOverBorderBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the checker background.
        /// </summary>
        /// <value>The checker background.</value>
        /// <remarks>
        /// Default value is Beige.
        /// </remarks>
        /// <example>
        /// C#:
        /// <code language="C#">
        /// DiagramControl diagramControl=new DiagramControl();
        /// diagramControl.SymbolPalette.CheckerBackground=Brushes.Beige;
        /// </code>
        /// </example>
        public Brush CheckerBackground
        {
            get
            {
                return (Brush)GetValue(CheckerBackgroundProperty);
            }

            set
            {
                SetValue(CheckerBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the checker border brush.
        /// </summary>
        /// <value>The checker border brush.</value>
        /// <remarks>
        /// Default value is DarkSlateGray.
        /// </remarks>
        /// <example>
        /// C#:
        /// <code language="C#">
        /// DiagramControl diagramControl=new DiagramControl();
        /// diagramControl.SymbolPalette.CheckerBorderBrush=Brushes.Blue;
        /// </code>
        /// </example>
        public Brush CheckerBorderBrush
        {
            get
            {
                return (Brush)GetValue(CheckerBorderBrushProperty);
            }

            set
            {
                SetValue(CheckerBorderBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the checker tick brush.
        /// </summary>
        /// <value>The checker tick brush.</value>
        /// <remarks>
        /// Default value is DarkSlateGray.
        /// </remarks>
        /// <example>
        /// C#:
        /// <code language="C#">
        /// DiagramControl diagramControl=new DiagramControl();
        /// diagramControl.SymbolPalette.CheckerTickBrush=Brushes.Blue;
        /// </code>
        /// </example>
        public Brush CheckerTickBrush
        {
            get
            {
                return (Brush)GetValue(CheckerTickBrushProperty);
            }

            set
            {
                SetValue(CheckerTickBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the pop up left column background.
        /// </summary>
        /// <value>The pop up left column background.</value>
        /// <remarks>
        /// Default value is LightGray.
        /// </remarks>
        /// <example>
        /// C#:
        /// <code language="C#">
        /// DiagramControl diagramControl=new DiagramControl();
        /// diagramControl.SymbolPalette.PopUpLeftColumnBackground=Brushes.Gray;
        /// </code>
        /// </example>
        public Brush PopUpLeftColumnBackground
        {
            get
            {
                return (Brush)GetValue(PopUpLeftColumnBackgroundProperty);
            }

            set
            {
                SetValue(PopUpLeftColumnBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the CustomHeaderText dependency property.
        /// </summary>       
        /// <value>
        /// Type : String
        /// </value>
        public string CustomHeaderText
        {
            get
            {
                return (string)GetValue(CustomHeaderTextProperty);
            }

            set
            {
                SetValue(CustomHeaderTextProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the filter selector background.
        /// </summary>
        /// <value>The filter selector background.</value>
        /// <remarks>
        /// Default value is Chocolate.
        /// </remarks>
        /// <example>
        /// C#:
        /// <code language="C#">
        /// DiagramControl diagramControl=new DiagramControl();
        /// diagramControl.SymbolPalette.FilterSelectorBackground=Brushes.Salmon;
        /// </code>
        /// </example>
        public Brush FilterSelectorBackground
        {
            get
            {
                return (Brush)GetValue(FilterSelectorBackgroundProperty);
            }

            set
            {
                SetValue(FilterSelectorBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the filter selector foreground.
        /// </summary>
        /// <value>The filter selector foreground.</value>
        /// <remarks>
        /// Default value is DarkSlateGray.
        /// </remarks>
        /// <example>
        /// C#:
        /// <code language="C#">
        /// DiagramControl diagramControl=new DiagramControl();
        /// diagramControl.SymbolPalette.FilterSelectorForeground=Brushes.Black;
        /// </code>
        /// </example>
        public Brush FilterSelectorForeground
        {
            get
            {
                return (Brush)GetValue(FilterSelectorForegroundProperty);
            }

            set
            {
                SetValue(FilterSelectorForegroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the filter selector border thickness.
        /// </summary>
        /// <value>The filter selector border thickness.</value>
        /// <remarks>
        /// Default value is (0,0,0,1).
        /// </remarks>
        /// <example>
        /// C#:
        /// <code language="C#">
        /// DiagramControl diagramControl=new DiagramControl();
        /// diagramControl.SymbolPalette.FilterSelectorBorderThickness=new Thickness(2);
        /// </code>
        /// </example>
        public Thickness FilterSelectorBorderThickness
        {
            get
            {
                return (Thickness)GetValue(FilterSelectorBorderThicknessProperty);
            }

            set
            {
                SetValue(FilterSelectorBorderThicknessProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the filter selector mouse over foreground.
        /// </summary>
        /// <value>The filter selector mouse over foreground.</value>
        /// <remarks>
        /// Default value is OldLace.
        /// </remarks>
        /// <example>
        /// C#:
        /// <code language="C#">
        /// DiagramControl diagramControl=new DiagramControl();
        /// diagramControl.SymbolPalette.FilterSelectorMouseOverForeground=Brushes.White;
        /// </code>
        /// </example>
        public Brush FilterSelectorMouseOverForeground
        {
            get
            {
                return (Brush)GetValue(FilterSelectorMouseOverForegroundProperty);
            }

            set
            {
                SetValue(FilterSelectorMouseOverForegroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the filter selector border brush.
        /// </summary>
        /// <value>The filter selector border brush.</value>
        /// <remarks>
        /// Default value is Chocolate.
        /// </remarks>
        /// <example>
        /// C#:
        /// <code language="C#">
        /// DiagramControl diagramControl=new DiagramControl();
        /// diagramControl.SymbolPalette.FilterSelectorBorderBrush=Brushes.Beige;
        /// </code>
        /// </example>
        public Brush FilterSelectorBorderBrush
        {
            get
            {
                return (Brush)GetValue(FilterSelectorBorderBrushProperty);
            }

            set
            {
                SetValue(FilterSelectorBorderBrushProperty, value);
            }
        }


         /// <summary>
        /// Gets or sets the filter selector dropdown border brush.
        /// </summary>
        /// <value>The filter selector dropdown border brush.</value>
        public Brush FilterSelectorDropdownBorderBrush
        {
            get
            { 
                return (Brush)GetValue(FilterSelectorDropdownBorderBrushProperty);
            }
            set 
            {
                SetValue(FilterSelectorDropdownBorderBrushProperty, value); 
            }
        }


         /// <summary>
         /// Gets or sets the filter selector dropdown background brush.
         /// </summary>
         /// <value>The filter selector dropdown background brush.</value>
         public Brush FilterSelectorDropdownBackgroundBrush
        {
            get
            {
                return (Brush)GetValue(FilterSelectorDropdownBackgroundBrushProperty); 
            }
            set
            { 
                SetValue(FilterSelectorDropdownBackgroundBrushProperty, value); 
            }
        }

        /// <summary>
        /// Gets or sets the border brush when the item is checked.
        /// </summary>
        /// <value>The item checked border brush.</value>
        /// <remarks>
        /// Default value is Red.
        /// </remarks>
        /// <example>
        /// C#:
        /// <code language="C#">
        /// DiagramControl diagramControl=new DiagramControl();
        /// diagramControl.SymbolPalette.ItemCheckedBorderBrush=Brushes.Blue;
        /// </code>
        /// </example>
        public Brush ItemCheckedBorderBrush
        {
            get
            {
                return (Brush)GetValue(ItemCheckedBorderBrushProperty);
            }

            set
            {
                SetValue(ItemCheckedBorderBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets border brush when the mouse is over the checked item.
        /// </summary>
        /// <value>The item checked mouse over border brush.</value>
        /// <remarks>
        /// Default value is Green.
        /// </remarks>
        /// <example>
        /// C#:
        /// <code language="C#">
        /// DiagramControl diagramControl=new DiagramControl();
        /// diagramControl.SymbolPalette.ItemCheckedMouseOverBorderBrush=Brushes.Violet;
        /// </code>
        /// </example>
        public Brush ItemCheckedMouseOverBorderBrush
        {
            get
            {
                return (Brush)GetValue(ItemCheckedMouseOverBorderBrushProperty);
            }

            set
            {
                SetValue(ItemCheckedMouseOverBorderBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the border brush when the mouse is over the checked item..
        /// </summary>
        /// <value>The item mouse over border brush.</value>
        /// <remarks>
        /// Default value is Orange.
        /// </remarks>
        /// <example>
        /// C#:
        /// <code language="C#">
        /// DiagramControl diagramControl=new DiagramControl();
        /// diagramControl.SymbolPalette.ItemMouseOverBorderBrush=Brushes.Beige;
        /// </code>
        /// </example>
        public Brush ItemMouseOverBorderBrush
        {
            get
            {
                return (Brush)GetValue(ItemMouseOverBorderBrushProperty);
            }

            set
            {
                SetValue(ItemMouseOverBorderBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the border brush when the item is checked.
        /// </summary>
        /// <value>The item checked border brush.</value>
        /// <remarks>
        /// Default value is Red.
        /// </remarks>
        /// <example>
        /// C#:
        /// <code language="C#">
        /// DiagramControl diagramControl=new DiagramControl();
        /// diagramControl.SymbolPalette.ItemCheckedBorderBrush=Brushes.Blue;
        /// </code>
        /// </example>
        public Brush ItemCheckedBackgroundBrush
        {
            get
            {
                return (Brush)GetValue(ItemCheckedBackgroundBrushProperty);
            }

            set
            {
                SetValue(ItemCheckedBackgroundBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets border brush when the mouse is over the checked item.
        /// </summary>
        /// <value>The item checked mouse over border brush.</value>
        /// <remarks>
        /// Default value is Green.
        /// </remarks>
        /// <example>
        /// C#:
        /// <code language="C#">
        /// DiagramControl diagramControl=new DiagramControl();
        /// diagramControl.SymbolPalette.ItemCheckedMouseOverBorderBrush=Brushes.Violet;
        /// </code>
        /// </example>
        public Brush ItemCheckedMouseOverBackgroundBrush
        {
            get
            {
                return (Brush)GetValue(ItemCheckedMouseOverBackgroundBrushProperty);
            }

            set
            {
                SetValue(ItemCheckedMouseOverBackgroundBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the border brush when the mouse is over the checked item..
        /// </summary>
        /// <value>The item mouse over border brush.</value>
        /// <remarks>
        /// Default value is Orange.
        /// </remarks>
        /// <example>
        /// C#:
        /// <code language="C#">
        /// DiagramControl diagramControl=new DiagramControl();
        /// diagramControl.SymbolPalette.ItemMouseOverBorderBrush=Brushes.Beige;
        /// </code>
        /// </example>
        public Brush ItemMouseOverBackgroundBrush
        {
            get
            {
                return (Brush)GetValue(ItemMouseOverBackgroundBrushProperty);
            }

            set
            {
                SetValue(ItemMouseOverBackgroundBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the SelectedItem dependency property.
        /// </summary>       
        /// <value>
        /// Type : SymbolPaletteItem
        /// </value>
        public SymbolPaletteItem SelectedItem
        {
            get
            {
                return (SymbolPaletteItem)GetValue(SelectedItemProperty);
            }

            set
            {
                SetValue(SelectedItemProperty, value);
            }
        }

        public ObservableCollection<SymbolPaletteGroup> SymbolGroups
        {
            get
            {
                return (ObservableCollection<SymbolPaletteGroup>)GetValue(SymbolGroupsProperty);
            }
            
            set
            {
                SetValue(SymbolGroupsProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the internal symbol filters.
        /// </summary>
        /// <value>The internal symbol filters.</value>
        internal ObservableCollection<SymbolPaletteFilter> InternalSymbolFilters
        {
            get
            {
                return (ObservableCollection<SymbolPaletteFilter>)GetValue(InternalFiltersProperty);
            }

            set
            {
                SetValue(InternalFiltersProperty, value);
            }
        }

        public ObservableCollection<SymbolPaletteFilter> SymbolFilters
        {
            get
            {
                return (ObservableCollection<SymbolPaletteFilter>)GetValue(FiltersProperty);
            }

            set
            {
                SetValue(FiltersProperty, value);
            }
        }

        public SymbolPaletteFilter CurrentFilter
        {
            get
            {
                return (SymbolPaletteFilter)GetValue(CurrentFilterProperty);
            }

            internal set
            {
                SetValue(CurrentFilterProperty, value);
            }
        }


        /// <summary>
        /// Gets or sets the SymbolPaletteGroup background.
        /// </summary>
        /// <value>The symbol palette group background.</value>
        /// <remarks>
        /// Default value is Bisque
        /// </remarks>
        /// <example>
        /// C#:
        /// <code language="C#">
        /// DiagramControl diagramControl=new DiagramControl();
        /// diagramControl.SymbolPalette.SymbolPaletteGroupBackground=Brushes.Beige;
        /// </code>
        /// </example>
        public Brush SymbolPaletteGroupBackground
        {
            get
            {
                return (Brush)GetValue(SymbolPaletteGroupBackgroundProperty);
            }

            set
            {
                SetValue(SymbolPaletteGroupBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the SymbolPaletteGroup border brush.
        /// </summary>
        /// <value>The symbol palette group border brush.</value>
        /// <remarks>
        /// Default value is Chocolate.
        /// </remarks>
        /// <example>
        /// C#:
        /// <code language="C#">
        /// DiagramControl diagramControl=new DiagramControl();
        /// diagramControl.SymbolPalette.SymbolPaletteGroupBorderBrush=Brushes.Pink;
        /// </code>
        /// </example>
        public Brush SymbolPaletteGroupBorderBrush
        {
            get
            {
                return (Brush)GetValue(SymbolPaletteGroupBorderBrushProperty);
            }

            set
            {
                SetValue(SymbolPaletteGroupBorderBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the SymbolPaletteGroup foreground.
        /// </summary>
        /// <value>The symbol palette group foreground.</value>
        /// <remarks>
        /// Default value is SaddleBrown.
        /// </remarks>
        /// <example>
        /// C#:
        /// <code language="C#">
        /// DiagramControl diagramControl=new DiagramControl();
        /// diagramControl.SymbolPalette.SymbolPaletteGroupForeground=Brushes.Gray;
        /// </code>
        /// </example>
        public Brush SymbolPaletteGroupForeground
        {
            get
            {
                return (Brush)GetValue(SymbolPaletteGroupForegroundProperty);
            }

            set
            {
                SetValue(SymbolPaletteGroupForegroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the ShowFilter.
        /// </summary>
        /// <value>To toggle symbol palette filter.</value>
        /// <remarks>
        /// Default value is True.
        /// </remarks>
        /// <example>
        /// C#:
        /// <code language="C#">
        /// DiagramControl diagramControl=new DiagramControl();
        /// diagramControl.SymbolPalette.ShowFilter=True;
        /// </code>
        /// </example>
        public bool ShowFilter
        {
            get { return (bool)GetValue(ShowFilterProperty); }
            set { SetValue(ShowFilterProperty, value); }
        }

        /// <summary>
        /// Adds the SymbolPaletteItem to the SymbolPaletteGroup
        /// </summary>
        /// <param name="group">SymbolPaletteGroup</param>
        /// <param name="path">Path string </param>
        void AddSymbolPaletteItem(SymbolPaletteGroup group, string path)
        {
            SymbolPaletteItem item = new SymbolPaletteItem();
            item.ItemName = path;             
            Path p = new Path();
#if SILVERLIGHT
            item.Width = 50;
            item.Height = 50;
            p.Width = 40;
            p.Height = 40;
#endif
            p.Style = (this.symbols[path] as Style);
            p.Fill = (this.symbols["ItemBrush"] as LinearGradientBrush);
            p.Stretch = Stretch.Fill;
            p.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            p.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            item.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Stretch;
            item.VerticalContentAlignment = System.Windows.VerticalAlignment.Stretch;            
            p.Stroke = new SolidColorBrush(Colors.Blue);
           
#if WPF
			p.Stroke = new SolidColorBrush(Colors.MidnightBlue);
            if (path.Contains("Electrical"))
            {
                p.Fill = new SolidColorBrush(new Color() {A=255, R = 85, G = 123, B = 198 });
                p.Margin = new Thickness(0, 8, 0, 8);
                p.Stroke = null;
            }
            item.ToolTip = path.Substring(path.LastIndexOf('_') + 1);
#endif
            p.StrokeThickness = 1;
            p.Margin = new Thickness(3);
            item.Content = p;
            group.Items.Add(item);
        }

#if WPF

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            ResourceDictionary resource = new ResourceDictionary();
            resource.Source = new Uri("/Syncfusion.Diagram.WPF;component/Themes/NodeShapes.xaml", UriKind.RelativeOrAbsolute);
            if (e.Property == SkinStorage.VisualStyleProperty)
            {
                if (e.NewValue.ToString() == "Office2007Blue" || e.NewValue.ToString() == "Office2010Blue")
                {
                    nodeFillBrush = resource["Office2007bluePathFill"] as Brush;
                    nodeStrokeBrush = resource["Office2007bluePathStroke"] as Brush;
                    resource.Source = new Uri("/Syncfusion.Diagram.WPF;component/Themes/Office2007BlueStyle.xaml", UriKind.RelativeOrAbsolute);
                    this.FillBrushes(resource);
                }
                else if (e.NewValue.ToString() == "Office2007Black" || e.NewValue.ToString() == "Office2010Black")
                {
                    nodeFillBrush = resource["Office2007blackPathFill"] as Brush;
                    nodeStrokeBrush = resource["Office2007blackPathStroke"] as Brush;
                    resource.Source = new Uri("/Syncfusion.Diagram.WPF;component/Themes/Office2007BlackStyle.xaml", UriKind.RelativeOrAbsolute);
                    this.FillBrushes(resource);
                }
                else if (e.NewValue.ToString() == "Office2007Silver" || e.NewValue.ToString() == "Office2010Silver")
                {
                    nodeFillBrush = resource["Office2007silverPathFill"] as Brush;
                    nodeStrokeBrush = resource["Office2007silverPathStroke"] as Brush;
                    resource.Source = new Uri("/Syncfusion.Diagram.WPF;component/Themes/Office2007SilverStyle.xaml", UriKind.RelativeOrAbsolute);
                    this.FillBrushes(resource);
                }
                else if (e.NewValue.ToString() == "Blend")
                {
                    nodeFillBrush = resource["BlendPathFill1"] as Brush;
                    nodeStrokeBrush = resource["BlendPathStroke"] as Brush;
                    resource.Source = new Uri("/Syncfusion.Diagram.WPF;component/Themes/BlendStyle.xaml", UriKind.RelativeOrAbsolute);
                    this.FillBrushes(resource);
                }
                else if (e.NewValue.ToString() == "VS2010")
                {
                    nodeFillBrush = resource["vs2010PathFill"] as Brush;
                    nodeStrokeBrush = resource["vs2010PathStroke"] as Brush;
                    resource.Source = new Uri("/Syncfusion.Diagram.WPF;component/Themes/VS2010Style.xaml", UriKind.RelativeOrAbsolute);
                    this.FillBrushes(resource);
                }
                else if (e.NewValue.ToString() == "Metro")
                {
                    nodeFillBrush = resource["MetroPathFill"] as Brush;
                    nodeStrokeBrush = resource["MetroPathStroke"] as Brush;
                    resource.Source = new Uri("/Syncfusion.Diagram.WPF;component/Themes/MetroStyle.xaml", UriKind.RelativeOrAbsolute);
                    this.FillBrushes(resource);
                }
                else if (e.NewValue.ToString() == "ShinyBlue")
                {
                    nodeFillBrush = resource["ShinyBluePathFill"] as Brush;
                    nodeStrokeBrush = resource["ShinyBluePathStroke"] as Brush;
                }
                else if (e.NewValue.ToString() == "ShinyRed")
                {
                    nodeFillBrush = resource["ShinyRedPathFill"] as Brush;
                    nodeStrokeBrush = resource["ShinyRedStroke"] as Brush;
                }
                else if (e.NewValue.ToString() == "Office2003")
                {
                    nodeFillBrush = resource["Office2003PathFill"] as Brush;
                    nodeStrokeBrush = resource["Office2003PathStroke"] as Brush;
                }                
                else
                {
                    nodeFillBrush = symbols["ItemBrush"] as Brush;
                    nodeStrokeBrush = new SolidColorBrush(Colors.MidnightBlue);
                    resource.Source = new Uri("/Syncfusion.Diagram.WPF;component/Themes/DefaultStyle.xaml", UriKind.RelativeOrAbsolute);
                    this.FillBrushes(resource);
                }

                foreach (SymbolPaletteGroup paletteGroup in this.SymbolGroups)
                {
                    foreach (SymbolPaletteItem paletteItem in paletteGroup.Items.OfType <SymbolPaletteItem>())
                    {
                        Path stylePath = paletteItem.Content as Path;
                        if (stylePath != null)
                        {
                            stylePath.Fill = nodeFillBrush;
                            stylePath.Stroke = nodeStrokeBrush;
                        }
                    }
                }

            }
        }
#endif
#if SILVERLIGHT
        //protected void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        //{
        //     ResourceDictionary resource = new ResourceDictionary();
        //    resource.Source = new Uri("/Syncfusion.Diagram.WPF;component/Themes/NodeShapes.xaml", UriKind.RelativeOrAbsolute);
        //    if ((Syncfusion.Windows.Controls.Theming.VisualStyle)e.NewValue==SkinManager.GetVisualStyle(this))
        //    {
        //        if (e.NewValue.ToString() == "Office2007Blue")
        //        {
        //            nodeFillBrush = resource["Office2007bluePathFill"] as Brush;
        //            nodeStrokeBrush = resource["Office2007bluePathStroke"] as Brush;
        //        }
        //    }
        //}
#endif
        /// <summary>
        /// Called when changes in visual state of automatic button takes place
        /// </summary>
        /// <param name="useTransitions">Indicate whether to apply transition or not</param>
        /// <param name="stateNames">Contain the state name</param>
        internal void GoToState(bool useTransitions, params string[] stateNames)
        {
            if (stateNames != null)
            {
                foreach (string str in stateNames)
                {
                    if (VisualStateManager.GoToState(this, str, useTransitions))
                    {
                        return;
                    }
                }
            }
            }



        /// <summary>
        /// Calls OnFilterIndexesChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnFilterIndexesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SymbolPalette sym = (d as SymbolPaletteGroup)._parent as SymbolPalette;
            if (sym != null)
            {
                sym.RefreshFilter(sym.CurrentFilter);
            }
        }

        /// <summary>
        /// Gets the value of the FilterIndexes property for a given element.
        /// </summary>
        /// <param name="obj">The element for which to retrieve the FilterIndexes value.</param>
        /// <returns>the filter indices.</returns>
        public static IList<int> GetFilterIndexes(DependencyObject obj)
        {
            return (IList<int>)obj.GetValue(FilterIndexesProperty);
        }

        /// <summary>
        /// Sets the value of the FilterIndexes property for a given element.
        /// </summary>
        /// <param name="obj">The element on which to apply the property value.</param>
        /// <param name="value">FilterIndexes value</param>
        public static void SetFilterIndexes(DependencyObject obj, IList<int> value)
        {
            obj.SetValue(FilterIndexesProperty, value);
        }

        internal SymbolPaletteGroup SPGShapes;
        internal SymbolPaletteGroup SPGConnectors;
        internal SymbolPaletteGroup SPGFlowchart;
        internal SymbolPaletteGroup SPGCustom;
        internal SymbolPaletteGroup SPGElectrical;

        internal ResourceWrapper m_ResourceWrapper;
        internal bool _ItemMode = false;
        protected override DependencyObject GetContainerForItemOverride()
        {
            if (ItemGenerateMode == ItemGenerateMode.ItemsSource && SymbolGroups != null)
            {
                SymbolGroups.Add(new SymbolPaletteGroup());
                return SymbolGroups[SymbolGroups.Count - 1];
            }
            else
                return base.GetContainerForItemOverride();

           
        }
        //protected override bool IsItemItsOwnContainerOverride(object item)
        //{
        //    if(item is SymbolPaletteGroup)
        //    {
        //        return false;
        //    }
        //    else
        //    {
        //        return true;
        //    }

        //    //return (!(if(item is SymbolPaletteGroup)));
        //}
        public string LocalizationPath
        {
            get;
            set;
        }
      
        /// <summary>
        /// Loads the palette items.
        /// </summary>
        void LoadPalette()
        {
            m_ResourceWrapper = new ResourceWrapper(LocalizationPath);

            SPGShapes = new SymbolPaletteGroup();           
            this.SymbolGroups.Add(SPGShapes);
            this.AddSymbolPaletteItem(SPGShapes, "PART_Hexagon");
            this.AddSymbolPaletteItem(SPGShapes, "PART_Octagon");
            this.AddSymbolPaletteItem(SPGShapes, "PART_Pentagon");
            this.AddSymbolPaletteItem(SPGShapes, "PART_Triangle");
            this.AddSymbolPaletteItem(SPGShapes, "PART_Star6");
            this.AddSymbolPaletteItem(SPGShapes, "PART_Star7");
            this.AddSymbolPaletteItem(SPGShapes, "PART_Ellipse");
            this.AddSymbolPaletteItem(SPGShapes, "PART_Plus");
            this.AddSymbolPaletteItem(SPGShapes, "PART_RoundedSquare");
            this.AddSymbolPaletteItem(SPGShapes, "PART_RightTriangle");
            this.AddSymbolPaletteItem(SPGShapes, "PART_Star");
            this.AddSymbolPaletteItem(SPGShapes, "PART_ThreeDBox");

            SPGShapes.HeaderName = m_ResourceWrapper.SymbolPaletteGroup_Shapes;

#if SILVERLIGHT
			List<int> n = new List<int>() {0, 1 };
#endif
#if WPF
            Int32Collection n = new Int32Collection() { 0, 1 };
#endif
            SymbolPalette.SetFilterIndexes(SPGShapes, n);

            SPGConnectors = new SymbolPaletteGroup();
            this.SymbolGroups.Add(SPGConnectors);
            this.AddSymbolPaletteItem(SPGConnectors, "PART_Orthogonal");
            this.AddSymbolPaletteItem(SPGConnectors, "PART_Straight");
            this.AddSymbolPaletteItem(SPGConnectors, "PART_Bezier");
            this.AddSymbolPaletteItem(SPGConnectors, "PART_Arc");

            SPGConnectors.HeaderName = m_ResourceWrapper.SymbolPaletteGroup_Connectors;

#if SILVERLIGHT
			n = new List<int>() { 0, 2 };
#endif
#if WPF
            n = new Int32Collection() { 0, 2 };
#endif
            SymbolPalette.SetFilterIndexes(SPGConnectors, n);

            SPGFlowchart = new SymbolPaletteGroup();          
            this.SymbolGroups.Add(SPGFlowchart);
            this.AddSymbolPaletteItem(SPGFlowchart, "PART_FlowChart_Decision");
            this.AddSymbolPaletteItem(SPGFlowchart, "PART_FlowChart_Predefined");
            this.AddSymbolPaletteItem(SPGFlowchart, "PART_FlowChart_StoredData");
            this.AddSymbolPaletteItem(SPGFlowchart, "PART_FlowChart_Document");
            this.AddSymbolPaletteItem(SPGFlowchart, "PART_FlowChart_Data");
            this.AddSymbolPaletteItem(SPGFlowchart, "PART_FlowChart_InternalStorage");
            this.AddSymbolPaletteItem(SPGFlowchart, "PART_FlowChart_PaperTape");
            this.AddSymbolPaletteItem(SPGFlowchart, "PART_FlowChart_SequentialData");
            this.AddSymbolPaletteItem(SPGFlowchart, "PART_FlowChart_DirectData");
            this.AddSymbolPaletteItem(SPGFlowchart, "PART_FlowChart_ManualInput");
            this.AddSymbolPaletteItem(SPGFlowchart, "PART_FlowChart_Card");
            this.AddSymbolPaletteItem(SPGFlowchart, "PART_FlowChart_Delay");
            this.AddSymbolPaletteItem(SPGFlowchart, "PART_FlowChart_Terminator");
            this.AddSymbolPaletteItem(SPGFlowchart, "PART_FlowChart_Display");
            this.AddSymbolPaletteItem(SPGFlowchart, "PART_FlowChart_LoopLimit");
            this.AddSymbolPaletteItem(SPGFlowchart, "PART_FlowChart_Preparation");
            this.AddSymbolPaletteItem(SPGFlowchart, "PART_FlowChart_ManualOperation");
            this.AddSymbolPaletteItem(SPGFlowchart, "PART_FlowChart_OffPageReference");

            SPGFlowchart.HeaderName = m_ResourceWrapper.SymbolPaletteFilter_Flowchart;

#if SILVERLIGHT
			n = new List<int>() { 0, 3 };
#endif
#if WPF
            n = new Int32Collection() { 0, 3 };
#endif
            SymbolPalette.SetFilterIndexes(SPGFlowchart, n);
            SPGCustom = new SymbolPaletteGroup();           
            this.SymbolGroups.Add(SPGCustom);
            this.AddSymbolPaletteItem(SPGCustom, "DataTransmission");
            this.AddSymbolPaletteItem(SPGCustom, "Collapse");
            this.AddSymbolPaletteItem(SPGCustom, "MultiDocuments");
            this.AddSymbolPaletteItem(SPGCustom, "MultiProcess");
            this.AddSymbolPaletteItem(SPGCustom, "RoundCallout");
            this.AddSymbolPaletteItem(SPGCustom, "TitleStyle");
            this.AddSymbolPaletteItem(SPGCustom, "CircleWithCross");
            this.AddSymbolPaletteItem(SPGCustom, "Start_End");
            this.AddSymbolPaletteItem(SPGCustom, "Developer");
            this.AddSymbolPaletteItem(SPGCustom, "DataStorageType");
            this.AddSymbolPaletteItem(SPGCustom, "NewDocument");

            SPGCustom.HeaderName = m_ResourceWrapper.SymbolPaletteGroup_CustomShapes;

#if SILVERLIGHT
			n = new List<int>() { 0, 4 };
#endif
#if WPF
            n = new Int32Collection() { 0, 4 };
#endif
            SymbolPalette.SetFilterIndexes(SPGCustom, n);

            SPGElectrical = new SymbolPaletteGroup();           
            this.SymbolGroups.Add(SPGElectrical);
            this.AddSymbolPaletteItem(SPGElectrical, "Part_Electrical_Resistor");
            this.AddSymbolPaletteItem(SPGElectrical, "Part_Electrical_Capacitor");
            this.AddSymbolPaletteItem(SPGElectrical, "Part_Electrical_Inductor");
            this.AddSymbolPaletteItem(SPGElectrical, "Part_Electrical_Switch");
            this.AddSymbolPaletteItem(SPGElectrical, "Part_Electrical_Ground");
            this.AddSymbolPaletteItem(SPGElectrical, "Part_Electrical_Battery");
            this.AddSymbolPaletteItem(SPGElectrical, "Part_Electrical_Transformer");
            this.AddSymbolPaletteItem(SPGElectrical, "Part_Electrical_Bipolar");
            this.AddSymbolPaletteItem(SPGElectrical, "Part_Electrical_Diode");

            SPGElectrical.HeaderName = m_ResourceWrapper.SymbolPaletteGroup_ElectricalShapes;

#if SILVERLIGHT
			n = new List<int>() { 0, 5 };
#endif
#if WPF
            n = new Int32Collection() { 0, 5 };
#endif
            SymbolPalette.SetFilterIndexes(SPGElectrical, n);
        }

        internal SymbolPaletteFilter SPFAll;
        internal SymbolPaletteFilter SPFShapes;
        internal SymbolPaletteFilter SPFConnectors;
        internal SymbolPaletteFilter SPFFlowchart;
        internal SymbolPaletteFilter SPFCustom;
        internal SymbolPaletteFilter SPFElectrical;

        private void LoadFilters()
        {
            SPFAll = new SymbolPaletteFilter();
            SPFAll.Label = m_ResourceWrapper.SymbolPaletteFilter_All;          
            this.SymbolFilters.Add(SPFAll);
           
            SPFShapes = new SymbolPaletteFilter();
            SPFShapes.Label = m_ResourceWrapper.SymbolPaletteFilter_Shapes;
            this.SymbolFilters.Add(SPFShapes);           
           
            SPFConnectors = new SymbolPaletteFilter();
            SPFConnectors.Label = m_ResourceWrapper.SymbolPaletteFilter_Connectors;
            this.SymbolFilters.Add(SPFConnectors);

            SPFFlowchart = new SymbolPaletteFilter();
            SPFFlowchart.Label = m_ResourceWrapper.SymbolPaletteFilter_Flowchart;
            this.SymbolFilters.Add(SPFFlowchart);

            SPFCustom = new SymbolPaletteFilter();
            SPFCustom.Label = m_ResourceWrapper.SymbolPaletteFilter_CustomShapes;
            this.SymbolFilters.Add(SPFCustom);

            SPFElectrical = new SymbolPaletteFilter();
            SPFElectrical.Label = m_ResourceWrapper.SymbolPaletteFilter_ElectricalShapes;
            this.SymbolFilters.Add(SPFElectrical);
           
        }

        /// <summary>
        /// Applies the Template for the control
        /// </summary>
        public override void OnApplyTemplate()
        {            
            foreach (SymbolPaletteGroup group in this.SymbolGroups)
            {
                Binding binding = new Binding();
                binding.Source = this;
                binding.Path = new PropertyPath("SymbolPaletteGroupBackground");
                group.SetBinding(SymbolPaletteGroup.SymbolPaletteGroupBackgroundProperty, binding);
                binding = new Binding();
                binding.Source = this;
                binding.Path = new PropertyPath("SymbolPaletteGroupForeground");
                group.SetBinding(SymbolPaletteGroup.SymbolPaletteGroupForegroundProperty, binding);
                binding = new Binding();
                binding.Source = this;
                binding.Path = new PropertyPath("SymbolPaletteGroupBorderThickness");
                group.SetBinding(SymbolPaletteGroup.SymbolPaletteGroupBorderThicknessProperty, binding);
                #if SILVERLIGHT
                foreach (SymbolPaletteItem item in group.Items)
                {
                    Binding binding1 = new Binding();
                    binding1.Source = this;
                    binding1.Path = new PropertyPath("ItemCheckedBorderBrush");
                    item.SetBinding(SymbolPaletteItem.ItemCheckedBorderBrushProperty, binding1);
                    binding1 = new Binding();
                    binding1.Source = this;
                    binding1.Path = new PropertyPath("ItemCheckedBackgroundBrush");
                    item.SetBinding(SymbolPaletteItem.ItemCheckedBackgroundBrushProperty, binding1);
                    binding1 = new Binding();
                    binding1.Source = this;
                    binding1.Path = new PropertyPath("ItemMouseOverBackgroundBrush");
                    item.SetBinding(SymbolPaletteItem.ItemMouseOverBackgroundBrushProperty, binding1);
                    binding1 = new Binding();
                    binding1.Source = this;
                    binding1.Path = new PropertyPath("ItemMouseOverBorderBrush");
                    item.SetBinding(SymbolPaletteItem.ItemMouseOverBorderBrushProperty, binding1);
                    binding1 = new Binding();
                    binding1.Source = this;
                    binding1.Path = new PropertyPath("ItemCheckedMouseOverBackgroundBrush");
                    item.SetBinding(SymbolPaletteItem.ItemCheckedMouseOverBackgroundBrushProperty, binding1);
                    binding1 = new Binding();
                    binding1.Source = this;
                    binding1.Path = new PropertyPath("ItemCheckedMouseOverBorderBrush");
                    item.SetBinding(SymbolPaletteItem.ItemCheckedMouseOverBorderBrushProperty, binding1);

                    if (item.Background == null)
                    {
                        binding1 = new Binding();
                        binding1.Source = this;
                        binding1.Path = new PropertyPath("Background");
                        item.SetBinding(SymbolPaletteItem.BackgroundProperty, binding1);
                    }
                }
#endif
            }         
        }

        /// <summary>
        /// Updates property value cache and raises SelectedItemChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnSelectedItemChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.SelectedItemChanged != null)
            {
                this.SelectedItemChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnSelectedItemChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SymbolPalette instance = (SymbolPalette)d;
            instance.OnSelectedItemChanged(e);
        }

        /// <summary>
        /// Calls OnCurrentFilterChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnCurrentFilterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SymbolPalette instance = (SymbolPalette)d;
            instance.OnCurrentFilterChanged(e);
        }


        /// <summary>
        /// Calls OnItemGenerateModeChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnItemGenerateModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SymbolPalette instance = (SymbolPalette)d;
            if (instance.ItemGenerateMode==ItemGenerateMode.ItemsSource)
            {
                instance._ItemMode = true;
            }
        }
        /// <summary>
        /// Updates property value cache and raises CurrentFilterChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnCurrentFilterChanged(DependencyPropertyChangedEventArgs e)
        {
            SymbolPaletteFilter filter = e.NewValue as SymbolPaletteFilter;

            foreach (SymbolPaletteGroup group in this.SymbolGroups)
            {
                IList<int> coll = SymbolPalette.GetFilterIndexes(group);
                if (coll != null)
                {
                    if (coll.Contains(this.SymbolFilters.IndexOf(filter)))
                    {
                        group.PanelVisibility = Visibility.Visible;
                    }
                    else
                    {
                        group.PanelVisibility = Visibility.Collapsed;
                    }
                }
            }
           
            if (CurrentFilterChanged != null)
            {
                CurrentFilterChanged(this, e);
            }
        }

        /// <summary>
        /// Checks if the group filter contains items or not .
        /// </summary>
        /// <param name="item">SymbolPalette item</param>
        /// <returns>True if it has ,false otherwise.</returns>
        private bool FilterItems(object item)
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                SymbolPaletteFilter filter = CurrentFilter;
                int filterIndex = this.InternalSymbolFilters.IndexOf(CurrentFilter);

                foreach (SymbolPaletteGroup group in SymbolGroups)
                {
                    IList<int> c = GetFilterIndexes(group);

                    if (c.Contains(filterIndex))
                    {
                        bool result = group.Items.Contains(item);

                        if (result)
                        {
                            return result;
                        }
                    }
                }

                return false;
            }
            else
            {
                return true;
            }
        }


        /// <summary>
        /// Calls OnCollectionChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        void SymbolGroups_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (SymbolPaletteGroup group in e.NewItems)
                {
                    Binding binding = new Binding();
                    binding.Source = this;
                    binding.Path = new PropertyPath("SymbolPaletteGroupBackground");
                    group.SetBinding(SymbolPaletteGroup.SymbolPaletteGroupBackgroundProperty, binding);
                    binding = new Binding();
                    binding.Source = this;
                    binding.Path = new PropertyPath("SymbolPaletteGroupForeground");
                    group.SetBinding(SymbolPaletteGroup.SymbolPaletteGroupForegroundProperty, binding);
                    binding = new Binding();
                    binding.Source = this;
                    binding.Path = new PropertyPath("SymbolPaletteGroupBorderThickness");
                    group.SetBinding(SymbolPaletteGroup.SymbolPaletteGroupBorderThicknessProperty, binding);
                    if (!_ItemMode)
                    {
                        try
                        {
                            this.Items.Add(group);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }

                    }
                    else
                    {
                        //this.SymbolGroups.Remove(e.NewItems[0] as SymbolPaletteGroup);
                    }

                  
                    foreach (SymbolPaletteItem item in group.Items)
                    {
                        Binding binding1 = new Binding();
                        binding1.Source = this;
                        binding1.Path = new PropertyPath("ItemCheckedBorderBrush");
                        item.SetBinding(SymbolPaletteItem.ItemCheckedBorderBrushProperty, binding1);
                        binding1 = new Binding();
                        binding1.Source = this;
                        binding1.Path = new PropertyPath("ItemCheckedBackgroundBrush");
                        item.SetBinding(SymbolPaletteItem.ItemCheckedBackgroundBrushProperty, binding1);
                        binding1 = new Binding();
                        binding1.Source = this;
                        binding1.Path = new PropertyPath("ItemCheckedMouseOverBorderBrush");
                        item.SetBinding(SymbolPaletteItem.ItemCheckedMouseOverBorderBrushProperty, binding1);
                        binding1 = new Binding();
                        binding1.Source = this;
                        binding1.Path = new PropertyPath("ItemCheckedMouseOverBackgroundBrush");
                        item.SetBinding(SymbolPaletteItem.ItemCheckedMouseOverBackgroundBrushProperty, binding1);
                        binding1 = new Binding();
                        binding1.Source = this;
                        binding1.Path = new PropertyPath("ItemMouseOverBorderBrush");
                        item.SetBinding(SymbolPaletteItem.ItemMouseOverBorderBrushProperty, binding1);
                        binding1 = new Binding();
                        binding1.Source = this;
                        binding1.Path = new PropertyPath("ItemMouseOverBackgroundBrush");
                        item.SetBinding(SymbolPaletteItem.ItemMouseOverBackgroundBrushProperty, binding1);
                        if (item.Background == null)
                        {
                            item.Background = this.Background;
                        }
                    }
                }
            }

            if (e.OldItems != null)
            {
                foreach (SymbolPaletteGroup group in e.OldItems)
                {
                    ObservableCollection<SymbolPaletteItem> internalcollection = new ObservableCollection<SymbolPaletteItem>();

                    foreach (SymbolPaletteItem item in group.Items)
                    {
                        internalcollection.Add(item);
                    }

                    foreach (SymbolPaletteItem item in internalcollection)
                    {
                        group.Items.Remove(item);
                    }

                    this.Items.Remove(group);
                }
            }
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
               
                ObservableCollection<SymbolPaletteGroup> internalgroupcollection = new ObservableCollection<SymbolPaletteGroup>();

                foreach (SymbolPaletteGroup group in this.Items)
                {
                    internalgroupcollection.Add(group);
                }

                foreach (SymbolPaletteGroup group in internalgroupcollection)
                {
                    ObservableCollection<SymbolPaletteItem> internalcollection = new ObservableCollection<SymbolPaletteItem>();

                    foreach (SymbolPaletteItem item in group.Items)
                    {
                        internalcollection.Add(item);
                    }

                    foreach (SymbolPaletteItem item in internalcollection)
                    {
                        group.Items.Remove(item);
                    }

                    this.Items.Remove(group);
                }
            }

            RefreshFilter(CurrentFilter);
        }

        /// <summary>
        /// Calls GalleryFilters_CollectionChanged method of the instance, notifies of the sender value changes.
        /// </summary>
        /// <param name="sender">object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private void SymbolPaletteFilters_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
           // HasFilters = false;
            if (SymbolFilters.Count > 0)
            {
             //   HasFilters = true;

                if (CurrentFilter == null)
                {
                    CurrentFilter = SymbolFilters[0];
                }
            }
            else
            {
             //   HasFilters = false;
                CurrentFilter = null;
            }
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                if (e.OldItems[0] == CurrentFilter)
                {
                    CurrentFilter = SymbolFilters[0];
                }
            }

            if (e.NewItems != null)
            {
                foreach (SymbolPaletteFilter fil in e.NewItems)
                {
                    InternalSymbolFilters.Add(fil);
                }
            }

            RefreshFilter(CurrentFilter);
        }

        private void RefreshFilter(SymbolPaletteFilter filter)
        {
            foreach (SymbolPaletteGroup group in this.SymbolGroups)
            {
                IList<int> coll = SymbolPalette.GetFilterIndexes(group);

                if (coll != null)
                {
                    if (coll.Contains(this.SymbolFilters.IndexOf(filter)))
                    {
                        group.PanelVisibility = Visibility.Visible;
                    }
                    else
                    {
                        group.PanelVisibility = Visibility.Collapsed;
                    }
                }
            }
        }

        /// <summary>
        /// Method is used to update the state of automatic button
        /// </summary>
        /// <param name="useTransitions">Update the state</param>
        internal void UpdateVisualState(bool useTransitions)
        {
            if (this.IsSelected)
            {
                this.GoToState(useTransitions, new string[] { "Selected" });
            }
            else
            {
                this.GoToState(useTransitions, new string[] { "Normal" });
            }

            //if (this.IsMouseOver)
            //{
            //    if (!this.IsSelected)
            //    {
            //        this.GoToState(useTransitions, new string[] { "MouseOver" });
            //    }
            //    else
            //    {
            //        this.GoToState(useTransitions, new string[] { "MouseOverAutomatic" });
            //    }
            //}
        }

#if WPF

        internal void FillBrushes(ResourceDictionary brushResource)
        {
            this.Background = brushResource["SymbolPaletteBackground"] as Brush;
            this.BorderBrush = brushResource["SymbolPaletteBorderBrush"] as Brush;         
            this.SymbolPaletteGroupBackground = brushResource["SymbolPaleteGroupBackground"] as Brush;
            this.SymbolPaletteGroupBorderBrush = brushResource["SymbolPaleteGroupBorderBrush"] as Brush;
            this.SymbolPaletteGroupForeground = brushResource["SymbolPaleteGroupForeground"] as Brush;
            this.FilterSelectorBackground = brushResource["FilterSelectorBackgroundBrush"] as Brush;
            this.FilterSelectorBorderBrush = brushResource["SymbolPaleteGroupBorderBrush"] as Brush;
            this.FilterSelectorMouseOverForeground = brushResource["FilterSelectorForeground"] as Brush;
            this.FilterSelectorForeground = brushResource["FilterSelectorForeground"] as Brush;
            this.PopUpBackground = brushResource["PopupBackgroundBrush"] as Brush;
            this.PopUpBorderBrush = brushResource["PopupBorderBrush"] as Brush;
            this.PopUpForeground = brushResource["PopupForegroundBrush"] as Brush;
            this.PopUpItemMouseOverBrush = brushResource["PopupItemMouseOverBrush"] as Brush;
            this.PopUpItemMouseOverBorderBrush = brushResource["PopupItemMouseOverBorderBrush"] as Brush;
            this.PopUpLeftColumnBackground = brushResource["PopupLeftColunBackgroundBrush"] as Brush;
            this.ItemCheckedBackgroundBrush = brushResource["itemCheckedBackgroundBrush"] as Brush;
            this.ItemCheckedBorderBrush = brushResource["itemCheckedBorderBrush"] as Brush;
            this.ItemCheckedMouseOverBackgroundBrush = brushResource["itemCheckedMouseOverBackgroundBrush"] as Brush;
            this.ItemCheckedMouseOverBorderBrush = brushResource["itemCheckedMouseOverBorderBrush"] as Brush;
            this.ItemMouseOverBackgroundBrush = brushResource["itemMouseOverBackgroundBrush"] as Brush;
            this.ItemMouseOverBorderBrush = brushResource["itemMouseOverBorderBrush"] as Brush;
            this.ItemMouseOverForegroundBrush = brushResource["itemMouseOverForegroundBrush"] as Brush;
            this.CheckerBackground = brushResource["CheckerBackgroundBrush"] as Brush;
            this.CheckerBorderBrush = brushResource["CheckerBorderBrush"] as Brush;
            this.CheckerTickBrush = brushResource["CheckerTick"] as Brush;
        }
#endif
    }
}
