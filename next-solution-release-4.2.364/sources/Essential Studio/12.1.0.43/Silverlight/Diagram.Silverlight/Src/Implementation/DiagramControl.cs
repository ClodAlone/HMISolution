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
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.IO.IsolatedStorage;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Interop;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Shapes;
    using System.Windows.Threading;
    using System.Xml;
    using System.Xml.Serialization;
    using Microsoft.Win32;
    using System.Xml.Linq;
    using System.Reflection;
    using Syncfusion.Windows.Shared;
    using System.Windows.Resources;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Represents the Diagram control.
    /// </summary>
    /// <remarks>
    /// <para>The Diagram control is the base class which contains the view and the this.Model. 
    /// It receives user input and translates it into actions and commands on the this.Model and view.  
    /// It also implements symbol palette and scrolling, and enables horizontal and vertical scrollbars when the size of the view exceeds the size of the window. 
    /// </para>
    /// </remarks>
    /// <example>
    /// <para/>The following example shows how to create a <see cref="DiagramControl"/> in XAML.
    /// <code language="XAML">
    /// &lt;Window x:Class="RulersAndUnits.Window1"
    /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    /// xmlns:syncfusion="http://schemas.syncfusion.com/wpf"
    /// Title="Rulers And Units Demo"  WindowState="Maximized" Name="mainwindow" 
    ///  xmlns:local="clr-namespace:Sample" FontWeight="Bold"
    ///  Icon="Images/App.ico" &gt;
    ///   &lt;syncfusion:DiagramControl Grid.Column="1" Name="diagramControl" 
    ///                                IsSymbolPaletteEnabled="True" 
    ///                                Background="WhiteSmoke"&gt;
    ///              lt;syncfusion:DiagramControl.this.Model&gt;
    ///                  &lt;syncfusion:DiagramModel LayoutType="None"  x:Name="diagramModel" &gt;
    ///                  &lt;/syncfusion:DiagramModel&gt;
    ///             &lt;/syncfusion:DiagramControl.this.Model&gt;
    ///           &lt;syncfusion:DiagramControl.View&gt;
    ///              &lt;syncfusion:DiagramView  IsPageEditable="True" 
    ///                                        Background="LightGray"  
    ///                                        Bounds="0,0,12,12"  
    ///                                        ShowHorizontalGridLine="False" 
    ///                                        ShowVerticalGridLine="False"
    ///                                        Name="diagramView"  &gt;
    ///             &lt;syncfusion:HorizontalRuler Name="horizontalRuler" /&gt;
    ///          &lt;/syncfusion:DiagramView.HorizontalRuler&gt;
    ///           &lt;syncfusion:DiagramView.VerticalRuler&gt;
    ///               &lt;syncfusion:VerticalRuler    Name="verticalRuler" /&gt;
    ///           &lt;/syncfusion:DiagramView.VerticalRuler &gt;
    ///       &lt;/syncfusion:DiagramView&gt;
    ///    &lt;/syncfusion:DiagramControl.View&gt;
    /// &lt;/syncfusion:DiagramControl&gt;
    /// &lt;/Window&gt;
    /// </code>
    /// <para/>The following example shows how to create a <see cref="DiagramControl"/> in C#.
    /// <code language="C#">
    /// using System;
    /// using System.Collections.Generic;
    /// using System.Linq;
    /// using System.Text;
    /// using System.Windows;
    /// using System.Windows.Controls;
    /// using System.Windows.Data;
    /// using System.Windows.Documents;
    /// using System.Windows.Input;
    /// using System.Windows.Media;
    /// using System.Windows.Media.Imaging;
    /// using System.Windows.Navigation;
    /// using System.Windows.Shapes;
    /// using System.ComponentModel;
    /// using Syncfusion.Core;
    /// using Syncfusion.Windows.Diagram;
    ///namespace SilverlightApplication1
    /// {
    /// public partial class MainPage : UserControl
    /// {
    ///    public DiagramControl Control;
    ///    public DiagramModel this.Model;
    ///    public DiagramView View;
    ///    public MainPage()
    ///    {
    ///       InitializeComponent ();
    ///       Control = new DiagramControl ();
    ///       this.Model = new DiagramModel ();
    ///       View = new DiagramView ();
    ///       Control.View = View;
    ///       Control.this.Model = this.Model;
    ///       HorizontalRuler hruler = new HorizontalRuler();
    ///       View.HorizontalRuler = hruler;
    ///       View.ShowHorizontalGridLine = false;
    ///       View.ShowVerticalGridLine = false;
    ///       VerticalRuler vruler = new VerticalRuler();
    ///       View.VerticalRuler = vruler;
    ///       View.Bounds = new Thickness (0, 0, 1000, 1000);
    ///       View.IsPageEditable = true;
    ///    }
    ///    }
    ///    }
    /// </code>
    /// </example>
    /// <seealso cref="DiagramView"/>
    ///     
    public partial class DiagramControl : ContentControl
    {
        /// <summary>
        /// Used to check if connector is removed or not.
        /// </summary>
        private bool connremoved = false;

        /// <summary>
        /// Used to store DiagramControl instance.
        /// </summary>
        private DiagramControl dc;

        /// <summary>
        /// Used to check if executed once.
        /// </summary>
        private bool exe = false;

        /// <summary>
        /// Checks if any node object is available for deletion.
        /// </summary>
        private bool isnodepresent = true;

        /// <summary>
        /// Used to check if page is saved or not.
        /// </summary>
        private bool ispagesaved = false;

        /// <summary>
        /// Identifies whether the SymbolPalette is enabled or not. This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty IsSymbolPaletteEnabledProperty = DependencyProperty.Register("IsSymbolPaletteEnabled", typeof(bool), typeof(DiagramControl), new PropertyMetadata(false, new PropertyChangedCallback(OnShowPalleteChanged)));

        /// <summary>
        /// Used to check if Diagram Control is unloaded.
        /// </summary>
        private bool isunloaded = false;

        internal bool IsLoaded;
        /// <summary>
        /// Used to check if page is loaded.
        /// </summary>
        private static bool mispageloaded = false;

        /// <summary>
        /// Used to store SymbolPalette visibility changed value.
        /// </summary>
        private bool mispalettechanged = false;

        /// <summary>
        /// Identifies the this.Model .  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty ModelProperty = DependencyProperty.Register("Model", typeof(DiagramModel), typeof(DiagramControl), new PropertyMetadata(new DiagramModel(), new PropertyChangedCallback(OnModelChanged)));

        /// <summary>
        /// Used to check if Nodes.Clear() is called.
        /// </summary>
        private bool nodecleared = false;

        /// <summary>
        /// Used to check if node is removed or not.
        /// </summary>
        private bool noderemoved = false;


        public ICommand Undo { get; set; }
        public ICommand ZoomIn { get; set; }
        public ICommand ZoomOut { get; set; }
        public ICommand Reset { get; set; }
        public ICommand SelectAll { get; set; }

        public ICommand Redo { get; set; }

        /// <summary>
        /// Group Command
        /// </summary>
        public ICommand Group { get; set; }

        /// <summary>
        /// UnGroup Command
        /// </summary>
        public ICommand UnGroup { get; set; }

        /// <summary>
        /// SameSize Command
        /// </summary>
        public ICommand SameSize { get; set; }

        /// <summary>
        /// SameHeight Command
        /// </summary>
        public ICommand SameHeight { get; set; }

        /// <summary>
        /// SameWidth Command
        /// </summary>
        public ICommand SameWidth { get; set; }

        /// <summary>
        /// AlignBottom Command
        /// </summary>
        public ICommand AlignBottom { get; set; }

        /// <summary>
        /// AlignTop Command
        /// </summary>
        public ICommand AlignTop { get; set; }

        /// <summary>
        /// AlignLeft Command
        /// </summary>
        public ICommand AlignLeft { get; set; }

        /// <summary>
        /// AlignCenter Command
        /// </summary>
        public ICommand AlignCenter { get; set; }

        /// <summary>
        /// AlignRight Command
        /// </summary>
        public ICommand AlignRight { get; set; }

        /// <summary>
        /// AlignMiddle Command
        /// </summary>
        public ICommand AlignMiddle { get; set; }

        /// <summary>
        /// SpaceAcross Command
        /// </summary>
        public ICommand SpaceAcross { get; set; }

        /// <summary>
        /// SpaceDown Command
        /// </summary>
        public ICommand SpaceDown { get; set; }

        /// <summary>
        /// SendToBack Command
        /// </summary>
        public ICommand SendToBack { get; set; }

        /// <summary>
        /// SendBackward Command
        /// </summary>
        public ICommand SendBackward { get; set; }

        /// <summary>
        /// BringToFront Command
        /// </summary>
        public ICommand BringToFront { get; set; }

        /// <summary>
        /// BringForward Command
        /// </summary>
        public ICommand BringForward { get; set; }

        /// <summary>
        /// MoveDown Command
        /// </summary>
        public ICommand MoveDown { get; set; }

        /// <summary>
        /// MoveDown Command
        /// </summary>
        public ICommand MoveUp { get; set; }

        /// <summary>
        /// MoveDown Command
        /// </summary>
        public ICommand MoveLeft { get; set; }

        /// <summary>
        /// MoveDown Command
        /// </summary>
        public ICommand MoveRight { get; set; }

        /// <summary>
        /// Cut Command
        /// </summary>
        public ICommand Cut { get; set; }

        /// <summary>
        /// Copy Command
        /// </summary>
        public ICommand Copy { get; set; }

        /// <summary>
        /// Paste Command
        /// </summary>
        public ICommand Paste { get; set; }
        /// <summary>
        /// Delete Command
        /// </summary>
        public ICommand Delete { get; set; }

        private DiagramCommandManager d_commands;

        /// <summary>
        /// Identifies  the SymbolPalette . This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty SymbolPaletteProperty = DependencyProperty.Register("SymbolPalette", typeof(SymbolPalette), typeof(DiagramControl), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the View.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty ViewProperty = DependencyProperty.Register("View", typeof(DiagramView), typeof(DiagramControl), new PropertyMetadata(null, new PropertyChangedCallback(OnViewChanged)));



        public static BowtieSubTreePlacement GetBowtieSubTreePlacement(DependencyObject obj)
        {
            return (BowtieSubTreePlacement)obj.GetValue(BowtieSubTreePlacementProperty);
        }

        public static void SetBowtieSubTreePlacement(DependencyObject obj, BowtieSubTreePlacement value)
        {
            obj.SetValue(BowtieSubTreePlacementProperty, value);
        }

        // Using a DependencyProperty as the backing store for BowtieSubTreePlacement.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BowtieSubTreePlacementProperty =
            DependencyProperty.RegisterAttached("BowtieSubTreePlacement", typeof(BowtieSubTreePlacement), typeof(DiagramControl), new PropertyMetadata(BowtieSubTreePlacement.Right));



        /// <summary>
        /// Initializes a new instance of the <see cref="DiagramControl"/> class.
        /// </summary>
        public DiagramControl()
        {
            this.DefaultStyleKey = typeof(DiagramControl);
            this.LocalizationPath = "Resources";
            this.Loaded += new RoutedEventHandler(this.ParseItemsSource);
            this.Loaded += new RoutedEventHandler(this.DiagramControl_Loaded);
            this.Unloaded += new RoutedEventHandler(DiagramControl_Unloaded);
            //To resolve the Assemblies issue while drag and drop from the Toolbox 
            Overview overview = new Overview();
            //DiagramControl.dc = this;
            this.dc = this;
            
            d_commands = new DiagramCommandManager();
            SelectAll = new Command(d_commands.OnSelectAllCommand, CanExecute);
            Reset = new Command(d_commands.ResetCommand, CanExecute);
            ZoomOut = new Command(d_commands.ZoomOutCommand, CanExecute);
            ZoomIn = new Command(d_commands.ZoomInCommand, CanExecute);
            Undo = new Command(d_commands.OnUndoCommand, CanExecute);
            Redo = new Command(d_commands.OnRedoCommand, CanExecute);
            MoveDown = new Command(d_commands.OnMoveDownCommand, CanExecute);
            MoveUp = new Command(d_commands.OnMoveUpCommand, CanExecute);
            MoveLeft = new Command(d_commands.OnMoveLeftCommand, CanExecute);
            MoveRight = new Command(d_commands.OnMoveRightCommand, CanExecute);
            Group = new Command(d_commands.GroupNodes, CanExecute);
            UnGroup = new Command(d_commands.UnGroupNodes, CanExecute);
            SameSize = new Command(d_commands.SameSizeCommand, CanExecute);
            SameWidth = new Command(d_commands.SameWidthCommand, CanExecute);
            SameHeight = new Command(d_commands.SameHeightCommand, CanExecute);
            AlignBottom = new Command(d_commands.OnAlignBottomCommand, CanExecute);
            AlignTop = new Command(d_commands.OnAlignTopCommand, CanExecute);
            AlignMiddle = new Command(d_commands.OnAlignMiddleCommand, CanExecute);
            AlignLeft = new Command(d_commands.OnAlignLeftCommand, CanExecute);
            AlignRight = new Command(d_commands.OnAlignRightCommand, CanExecute);
            AlignCenter = new Command(d_commands.OnAlignCenterCommand, CanExecute);
            SpaceAcross = new Command(d_commands.OnSpaceAcrossCommand, CanExecute);
            SpaceDown = new Command(d_commands.OnSpaceDownCommand, CanExecute);
            SendToBack = new Command(d_commands.OnSendToBackCommand, CanExecute);
            SendBackward = new Command(d_commands.OnSendBackwardCommand, CanExecute);
            BringForward = new Command(d_commands.OnMoveForwardCommand, CanExecute);
            BringToFront = new Command(d_commands.OnBringToFrontCommand, CanExecute);
            Cut = new Command(d_commands.OnCutCommand, CanExecute);
            Copy = new Command(d_commands.OnCopyCommand, CanExecute);
            Paste = new Command(d_commands.OnPasteCommand, CanExecute);
            Delete = new Command(d_commands.OnDeleteCommand, CanExecute);
            
        }

        private void ConstructPalette()
        {
            this.SymbolPalette = new SymbolPalette();
            this.SymbolPalette.Loaded += new RoutedEventHandler(palette_Loaded);
        }

        void palette_Loaded(object sender, RoutedEventArgs e)
        {
            this.SymbolPalette.Loaded -= new RoutedEventHandler(palette_Loaded);
            LocalizePalette();
        }

        private void LocalizePalette()
        {
            if (this.SymbolPalette.SPGConnectors.ReadLocalValue(SymbolPaletteGroup.HeadNameProperty) == DependencyProperty.UnsetValue)
            {
                this.SymbolPalette.SPGConnectors.HeaderName = m_ResourceWrapper.SymbolPaletteGroup_Connectors;
            }
            if (this.SymbolPalette.SPGElectrical.ReadLocalValue(SymbolPaletteGroup.HeadNameProperty) == DependencyProperty.UnsetValue)
            {
                this.SymbolPalette.SPGElectrical.HeaderName = m_ResourceWrapper.SymbolPaletteGroup_ElectricalShapes;
            }
            if (this.SymbolPalette.SPGCustom.ReadLocalValue(SymbolPaletteGroup.HeadNameProperty) == DependencyProperty.UnsetValue)
            {

                this.SymbolPalette.SPGCustom.HeaderName = m_ResourceWrapper.SymbolPaletteGroup_CustomShapes;
            }
            if (this.SymbolPalette.SPGFlowchart.ReadLocalValue(SymbolPaletteGroup.HeadNameProperty) == DependencyProperty.UnsetValue)
            {
                this.SymbolPalette.SPGFlowchart.HeaderName = m_ResourceWrapper.SymbolPaletteGroup_Flowchart;
            }
            if (this.SymbolPalette.SPGShapes.ReadLocalValue(SymbolPaletteGroup.HeadNameProperty) == DependencyProperty.UnsetValue)
            {
                this.SymbolPalette.SPGShapes.HeaderName = m_ResourceWrapper.SymbolPaletteGroup_Shapes;
            }


            //SymbolPaletteFilter
            if (this.SymbolPalette.SPFCustom.ReadLocalValue(SymbolPaletteFilter.LabelProperty) == DependencyProperty.UnsetValue)
            {
                this.SymbolPalette.SPFCustom.Label = m_ResourceWrapper.SymbolPaletteFilter_CustomShapes;
            }
            if (this.SymbolPalette.SPFConnectors.ReadLocalValue(SymbolPaletteFilter.LabelProperty) == DependencyProperty.UnsetValue)
            {
                this.SymbolPalette.SPFConnectors.Label = m_ResourceWrapper.SymbolPaletteFilter_Connectors;
            }
            if (this.SymbolPalette.SPFAll.ReadLocalValue(SymbolPaletteFilter.LabelProperty) == DependencyProperty.UnsetValue)
            {
                this.SymbolPalette.SPFAll.Label = m_ResourceWrapper.SymbolPaletteFilter_All;
            }
            if (this.SymbolPalette.SPFElectrical.ReadLocalValue(SymbolPaletteFilter.LabelProperty) == DependencyProperty.UnsetValue)
            {
                this.SymbolPalette.SPFElectrical.Label = m_ResourceWrapper.SymbolPaletteFilter_ElectricalShapes;
            }
            if (this.SymbolPalette.SPFFlowchart.ReadLocalValue(SymbolPaletteFilter.LabelProperty) == DependencyProperty.UnsetValue)
            {
                this.SymbolPalette.SPFFlowchart.Label = m_ResourceWrapper.SymbolPaletteFilter_Flowchart;
            }
            if (this.SymbolPalette.SPFShapes.ReadLocalValue(SymbolPaletteFilter.LabelProperty) == DependencyProperty.UnsetValue)
            {
                this.SymbolPalette.SPFShapes.Label = m_ResourceWrapper.SymbolPaletteFilter_Shapes;
            }
        }

        void ParseItemsSource(object sender, RoutedEventArgs e)
        {
            this.IsLoaded = true;
            if (this.IsLoaded)
            {
                if (this.Model != null)
                {
                    this.Model.PrepareItems();
                    this.Loaded -= ParseItemsSource;
                }
            }
        }

        double FindWidth()
        {
            var offx = from Node nod in Model.Nodes select nod.OffsetX + nod.Width;
            var items = offx.ToList<double>();
            double width = 0;
            if (items.Any())
            {
                width = items.Max();
            }
            return width;
        }

        double FindHeight()
        {
            var offy = from Node nod in Model.Nodes select nod.OffsetY + nod.Height;
            var items = offy.ToList<double>();
            double height = 0;
            if (items.Any())
            {
                height = items.Max();
            }
            return height;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (DesignerProperties.GetIsInDesignMode(this))
            {
                if (double.IsPositiveInfinity(availableSize.Width))
                {
                    //  availableSize.Width = 600;
                }

                if (double.IsPositiveInfinity(availableSize.Height))
                {
                    // availableSize.Height = 600;
                }

            }

            if (IsLoaded)
            {
                if (double.IsPositiveInfinity(availableSize.Width))
                {
                    availableSize.Width = FindWidth();
                }

                if (double.IsPositiveInfinity(availableSize.Height))
                {
                    availableSize.Height = FindHeight();
                }
            }

            return base.MeasureOverride(availableSize);
        }

        public DiagramControl(string LocalizationPath)
            : this()
        {
            this.LocalizationPath = LocalizationPath;
        }

        

        void DiagramControl_Unloaded(object sender, RoutedEventArgs e)
        {
            
            if (View != null)
            {
                View.IsLoaded = false;
            }
        }

        public string LocalizationPath
        {
            get;
            set;
        }


        /// <summary>
        /// Gets or sets a value indicating whether this instance is page loaded.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is page loaded; otherwise, <c>false</c>.
        /// </value>
        internal static bool IsPageLoaded
        {
            get { return mispageloaded; }
            set { mispageloaded = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether page is saved.
        /// </summary>
        /// <value><c>true</c> if page is saved; otherwise, <c>false</c>.</value>
        internal bool IspageSaved
        {
            get
            {
                return this.ispagesaved;
            }

            set
            {
                this.ispagesaved = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is symbol palette enabled.
        /// </summary>
        /// <value>
        /// Type: <see cref="Boolean"/>
        /// True, if SymbolPalette is enabled, false otherwise.
        /// </value>
        /// <example>
        /// <code language="XAML">
        /// &lt;UserControl x:Class=&quot;Sample.MainPage&quot;
        /// xmlns=&quot;http://schemas.microsoft.com/winfx/2006/xaml/presentation&quot; 
        ///     xmlns:x=&quot;http://schemas.microsoft.com/winfx/2006/xaml&quot; 
        ///     xmlns:syncfusion=&quot;clr-namespace:Syncfusion.Windows.Diagram;assembly=Syncfusion.Diagram.Silverlight&quot;
        ///    xmlns:vsm=&quot;clr-namespace:System.Windows;assembly=System.Windows&quot; 
        ///             &gt;
        ///   &lt;syncfusion:DiagramControl Grid.Column="1" Name="diagramControl" 
        ///                                IsSymbolPaletteEnabled="True" 
        ///                                Background="WhiteSmoke"&gt;
        ///              lt;syncfusion:DiagramControl.this.Model&gt;
        ///                  &lt;syncfusion:DiagramModel LayoutType="None"  x:Name="diagramModel" &gt;
        ///                  &lt;/syncfusion:DiagramModel&gt;
        ///             &lt;/syncfusion:DiagramControl.this.Model&gt;
        ///           &lt;syncfusion:DiagramControl.View&gt;
        ///              &lt;syncfusion:DiagramView Bounds="0,0,12,12"  
        ///                                        Name="diagramView"  &gt;
        ///             &lt;syncfusion:HorizontalRuler Name="horizontalRuler" /&gt;
        ///          &lt;/syncfusion:DiagramView.HorizontalRuler&gt;
        ///           &lt;syncfusion:DiagramView.VerticalRuler&gt;
        ///               &lt;syncfusion:VerticalRuler    Name="verticalRuler" /&gt;
        ///           &lt;/syncfusion:DiagramView.VerticalRuler &gt;
        ///       &lt;/syncfusion:DiagramView&gt;
        ///    &lt;/syncfusion:DiagramControl.View&gt;
        /// &lt;/syncfusion:DiagramControl&gt;
        /// &lt;/UserControl&gt;
        /// </code>
        /// <code language="C#">
        /// using System;
        /// using System.Collections.Generic;
        /// using System.Linq;
        /// using System.Text;
        /// using System.Windows;
        /// using System.Windows.Controls;
        /// using System.Windows.Data;
        /// using System.Windows.Documents;
        /// using System.Windows.Input;
        /// using System.Windows.Media;
        /// using System.Windows.Media.Imaging;
        /// using System.Windows.Navigation;
        /// using System.Windows.Shapes;
        /// using System.ComponentModel;
        /// using Syncfusion.Core;
        /// using Syncfusion.Windows.Diagram;
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        ///    public DiagramControl Control;
        ///    public DiagramModel this.Model;
        ///    public DiagramView View;
        ///    public MainPage()
        ///    {
        ///       InitializeComponent ();
        ///       Control = new DiagramControl ();
        ///       this.Model = new DiagramModel ();
        ///       View = new DiagramView ();
        ///       Control.View = View;
        ///       Control.this.Model = this.Model;
        ///       SymbolPaletteFilter sfilter = new SymbolPaletteFilter();
        ///       sfilter.Label = "Custom";
        ///        Control.SymbolPalette.SymbolFilters.Add(sfilter);
        ///       SymbolPaletteGroup s = new SymbolPaletteGroup();
        ///       s.Label = "Custom";
        ///       SymbolPalette.SetFilterIndexes(s, new Int32Collection(new int[] { 0, 5}));
        ///       Control.SymbolPalette.SymbolGroups.Add(s);
        ///       SymbolPaletteItem ss = new SymbolPaletteItem();
        ///       Path path = this.Resources["CustomShape"] as Path;
        ///       ss.Content = path;
        ///       s.Items.Add(ss);
        ///       HorizontalRuler hruler = new HorizontalRuler();
        ///       View.HorizontalRuler = hruler;
        ///       View.ShowHorizontalGridLine = false;
        ///       View.ShowVerticalGridLine = false;
        ///       VerticalRuler vruler = new VerticalRuler();
        ///       View.VerticalRuler = vruler;
        ///       View.Bounds = new Thickness (0, 0, 1000, 1000);
        ///       View.IsPageEditable = true;
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        /// <seealso cref="SymbolPalette"/>
        /// <seealso cref="SymbolPaletteGroup"/>
        /// <seealso cref="SymbolPaletteItem"/>
        public bool IsSymbolPaletteEnabled
        {
            get
            {
                return (bool)GetValue(IsSymbolPaletteEnabledProperty);
            }

            set
            {
                SetValue(IsSymbolPaletteEnabledProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Symbol Palette
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is symbol palette visibility changed; otherwise, <c>false</c>.
        /// </value>
        internal bool IsSymbolPaletteVisibilityChanged
        {
            get { return this.mispalettechanged; }
            set { this.mispalettechanged = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Diagram Control is unloaded.
        /// </summary>
        /// <value>
        /// <c>true</c> if the Diagram Control is unloaded; otherwise, <c>false</c>.
        /// </value>
        internal bool IsUnloaded
        {
            get
            {
                return this.isunloaded;
            }

            set
            {
                this.isunloaded = value;
            }
        }

        /// <summary>
        /// Gets or sets the this.Model property.
        /// </summary>
        /// <value>
        /// Type: <see cref="DiagramModel"/>
        /// </value>
        /// <example>
        /// <para/>The following example shows how to create a <see cref="DiagramControl"/> in XAML.
        /// <code language="XAML">
        /// &lt;UserControl x:Class=&quot;Sample.MainPage&quot;
        /// xmlns=&quot;http://schemas.microsoft.com/winfx/2006/xaml/presentation&quot; 
        ///     xmlns:x=&quot;http://schemas.microsoft.com/winfx/2006/xaml&quot; 
        ///     xmlns:syncfusion=&quot;clr-namespace:Syncfusion.Windows.Diagram;assembly=Syncfusion.Diagram.Silverlight&quot;
        ///    xmlns:vsm=&quot;clr-namespace:System.Windows;assembly=System.Windows&quot; 
        ///             &gt;
        ///   &lt;syncfusion:DiagramControl Grid.Column="1" Name="diagramControl" 
        ///                                IsSymbolPaletteEnabled="True" 
        ///                                Background="WhiteSmoke"&gt;
        ///              lt;syncfusion:DiagramControl.this.Model&gt;
        ///                  &lt;syncfusion:DiagramModel LayoutType="None"  x:Name="diagramModel" &gt;
        ///                  &lt;/syncfusion:DiagramModel&gt;
        ///             &lt;/syncfusion:DiagramControl.this.Model&gt;
        ///           &lt;syncfusion:DiagramControl.View&gt;
        ///              &lt;syncfusion:DiagramView  IsPageEditable="True" 
        ///                                        Background="LightGray"  
        ///                                        Bounds="0,0,12,12"  
        ///                                        ShowHorizontalGridLine="False" 
        ///                                        ShowVerticalGridLine="False"
        ///                                        Name="diagramView"  &gt;
        ///             &lt;syncfusion:HorizontalRuler Name="horizontalRuler" /&gt;
        ///          &lt;/syncfusion:DiagramView.HorizontalRuler&gt;
        ///           &lt;syncfusion:DiagramView.VerticalRuler&gt;
        ///               &lt;syncfusion:VerticalRuler    Name="verticalRuler" /&gt;
        ///           &lt;/syncfusion:DiagramView.VerticalRuler &gt;
        ///       &lt;/syncfusion:DiagramView&gt;
        ///    &lt;/syncfusion:DiagramControl.View&gt;
        /// &lt;/syncfusion:DiagramControl&gt;
        /// &lt;/UserControl&gt;
        /// </code>
        /// <para/>The following example shows how to create a <see cref="DiagramControl"/> in C#.
        /// <code language="C#">
        /// using System;
        /// using System.Collections.Generic;
        /// using System.Linq;
        /// using System.Text;
        /// using System.Windows;
        /// using System.Windows.Controls;
        /// using System.Windows.Data;
        /// using System.Windows.Documents;
        /// using System.Windows.Input;
        /// using System.Windows.Media;
        /// using System.Windows.Media.Imaging;
        /// using System.Windows.Navigation;
        /// using System.Windows.Shapes;
        /// using System.ComponentModel;
        /// using Syncfusion.Core;
        /// using Syncfusion.Windows.Diagram;
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        ///    public DiagramControl Control;
        ///    public DiagramModel this.Model;
        ///    public DiagramView View;
        ///    public MainPage()
        ///    {
        ///       InitializeComponent ();
        ///       Control = new DiagramControl ();
        ///       this.Model = new DiagramModel ();
        ///       View = new DiagramView ();
        ///       Control.View = View;
        ///       Control.this.Model = this.Model;
        ///       HorizontalRuler hruler = new HorizontalRuler();
        ///       View.HorizontalRuler = hruler;
        ///       View.ShowHorizontalGridLine = false;
        ///       View.ShowVerticalGridLine = false;
        ///       VerticalRuler vruler = new VerticalRuler();
        ///       View.VerticalRuler = vruler;
        ///       View.Bounds = new Thickness (0, 0, 1000, 1000);
        ///       View.IsPageEditable = true;
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        /// <seealso cref="DiagramModel"/>
        public DiagramModel Model
        {
            get
            {
                return (DiagramModel)GetValue(ModelProperty);
            }

            set
            {
                SetValue(ModelProperty, value);
            }
        }

        /// <summary>
        /// Gets the SymbolPalette property.
        /// </summary>
        /// <value>
        /// Type: <see cref="SymbolPalette"/>
        /// </value>
        /// <example>
        /// <code language="XAML">
        /// &lt;UserControl x:Class=&quot;Sample.MainPage&quot;
        /// xmlns=&quot;http://schemas.microsoft.com/winfx/2006/xaml/presentation&quot; 
        ///     xmlns:x=&quot;http://schemas.microsoft.com/winfx/2006/xaml&quot; 
        ///     xmlns:syncfusion=&quot;clr-namespace:Syncfusion.Windows.Diagram;assembly=Syncfusion.Diagram.Silverlight&quot;
        ///    xmlns:vsm=&quot;clr-namespace:System.Windows;assembly=System.Windows&quot; 
        ///             &gt;
        ///   &lt;syncfusion:DiagramControl Grid.Column="1" Name="diagramControl" 
        ///                                IsSymbolPaletteEnabled="True" 
        ///                                Background="WhiteSmoke"&gt;
        ///              lt;syncfusion:DiagramControl.this.Model&gt;
        ///                  &lt;syncfusion:DiagramModel LayoutType="None"  x:Name="diagramModel" &gt;
        ///                  &lt;/syncfusion:DiagramModel&gt;
        ///             &lt;/syncfusion:DiagramControl.this.Model&gt;
        ///           &lt;syncfusion:DiagramControl.View&gt;
        ///              &lt;syncfusion:DiagramView Bounds="0,0,12,12"  
        ///                                        Name="diagramView"  &gt;
        ///             &lt;syncfusion:HorizontalRuler Name="horizontalRuler" /&gt;
        ///          &lt;/syncfusion:DiagramView.HorizontalRuler&gt;
        ///           &lt;syncfusion:DiagramView.VerticalRuler&gt;
        ///               &lt;syncfusion:VerticalRuler    Name="verticalRuler" /&gt;
        ///           &lt;/syncfusion:DiagramView.VerticalRuler &gt;
        ///       &lt;/syncfusion:DiagramView&gt;
        ///    &lt;/syncfusion:DiagramControl.View&gt;
        /// &lt;/syncfusion:DiagramControl&gt;
        /// &lt;/UserControl&gt;
        /// </code>
        /// <code language="C#">
        /// using System;
        /// using System.Collections.Generic;
        /// using System.Linq;
        /// using System.Text;
        /// using System.Windows;
        /// using System.Windows.Controls;
        /// using System.Windows.Data;
        /// using System.Windows.Documents;
        /// using System.Windows.Input;
        /// using System.Windows.Media;
        /// using System.Windows.Media.Imaging;
        /// using System.Windows.Navigation;
        /// using System.Windows.Shapes;
        /// using System.ComponentModel;
        /// using Syncfusion.Core;
        /// using Syncfusion.Windows.Diagram;
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        ///    public DiagramControl Control;
        ///    public DiagramModel this.Model;
        ///    public DiagramView View;
        ///    public MainPage()
        ///    {
        ///       InitializeComponent ();
        ///       Control = new DiagramControl ();
        ///       this.Model = new DiagramModel ();
        ///       View = new DiagramView ();
        ///       Control.View = View;
        ///       Control.this.Model = this.Model;
        ///       SymbolPaletteFilter sfilter = new SymbolPaletteFilter();
        ///       sfilter.Label = "Custom";
        ///        Control.SymbolPalette.SymbolFilters.Add(sfilter);
        ///       SymbolPaletteGroup s = new SymbolPaletteGroup();
        ///       s.Label = "Custom";
        ///       SymbolPalette.SetFilterIndexes(s, new Int32Collection(new int[] { 0, 5}));
        ///       Control.SymbolPalette.SymbolGroups.Add(s);
        ///       SymbolPaletteItem ss = new SymbolPaletteItem();
        ///       Path path = this.Resources["CustomShape"] as Path;
        ///       ss.Content = path;
        ///       s.Items.Add(ss);
        ///       HorizontalRuler hruler = new HorizontalRuler();
        ///       View.HorizontalRuler = hruler;
        ///       View.ShowHorizontalGridLine = false;
        ///       View.ShowVerticalGridLine = false;
        ///       VerticalRuler vruler = new VerticalRuler();
        ///       View.VerticalRuler = vruler;
        ///       View.Bounds = new Thickness (0, 0, 1000, 1000);
        ///       View.IsPageEditable = true;
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        /// <seealso cref="SymbolPalette"/>
        /// <seealso cref="SymbolPaletteGroup"/>
        /// <seealso cref="SymbolPaletteItem"/>
        public SymbolPalette SymbolPalette
        {
            get
            {
                return (SymbolPalette)GetValue(SymbolPaletteProperty);
            }

            internal set
            {
                SetValue(SymbolPaletteProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the view.
        /// </summary>
        /// <value>Type: <see cref="DiagramView"/></value>
        /// <example>
        /// <para/>The following example shows how to create a <see cref="DiagramView"/> in XAML.
        /// <code language="XAML">
        /// &lt;UserControl x:Class=&quot;Sample.MainPage&quot;
        /// xmlns=&quot;http://schemas.microsoft.com/winfx/2006/xaml/presentation&quot; 
        ///     xmlns:x=&quot;http://schemas.microsoft.com/winfx/2006/xaml&quot; 
        ///     xmlns:syncfusion=&quot;clr-namespace:Syncfusion.Windows.Diagram;assembly=Syncfusion.Diagram.Silverlight&quot;
        ///    xmlns:vsm=&quot;clr-namespace:System.Windows;assembly=System.Windows&quot; 
        ///             &gt;
        /// &lt;syncfusion:DiagramControl Grid.Column="1" Name="diagramControl"
        /// IsSymbolPaletteEnabled="True"
        /// Background="WhiteSmoke"&gt;
        /// lt;syncfusion:DiagramControl.this.Model&gt;
        /// &lt;syncfusion:DiagramModel LayoutType="None"  x:Name="diagramModel" &gt;
        /// &lt;/syncfusion:DiagramModel&gt;
        /// &lt;/syncfusion:DiagramControl.this.Model&gt;
        /// &lt;syncfusion:DiagramControl.View&gt;
        /// &lt;syncfusion:DiagramView  IsPageEditable="True"
        /// Background="LightGray"
        /// Bounds="0,0,12,12"
        /// ShowHorizontalGridLine="False"
        /// ShowVerticalGridLine="False"
        /// Name="diagramView"  &gt;
        /// &lt;syncfusion:HorizontalRuler Name="horizontalRuler" /&gt;
        /// &lt;/syncfusion:DiagramView.HorizontalRuler&gt;
        /// &lt;syncfusion:DiagramView.VerticalRuler&gt;
        /// &lt;syncfusion:VerticalRuler    Name="verticalRuler" /&gt;
        /// &lt;/syncfusion:DiagramView.VerticalRuler &gt;
        /// &lt;/syncfusion:DiagramView&gt;
        /// &lt;/syncfusion:DiagramControl.View&gt;
        /// &lt;/syncfusion:DiagramControl&gt;
        /// &lt;/USerControl&gt;
        /// </code>
        /// <para/>The following example shows how to create a <see cref="DiagramView"/> in C#.
        /// <code language="C#">
        /// using System;
        /// using System.Collections.Generic;
        /// using System.Linq;
        /// using System.Text;
        /// using System.Windows;
        /// using System.Windows.Controls;
        /// using System.Windows.Data;
        /// using System.Windows.Documents;
        /// using System.Windows.Input;
        /// using System.Windows.Media;
        /// using System.Windows.Media.Imaging;
        /// using System.Windows.Navigation;
        /// using System.Windows.Shapes;
        /// using System.ComponentModel;
        /// using Syncfusion.Core;
        /// using Syncfusion.Windows.Diagram;
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        /// public DiagramControl Control;
        /// public DiagramModel this.Model;
        /// public DiagramView View;
        /// public MainPage()
        /// {
        /// InitializeComponent ();
        /// Control = new DiagramControl ();
        /// this.Model = new DiagramModel ();
        /// View = new DiagramView ();
        /// Control.View = View;
        /// Control.this.Model = this.Model;
        /// HorizontalRuler hruler = new HorizontalRuler();
        /// View.HorizontalRuler = hruler;
        /// View.ShowHorizontalGridLine = false;
        /// View.ShowVerticalGridLine = false;
        /// VerticalRuler vruler = new VerticalRuler();
        /// View.VerticalRuler = vruler;
        /// View.Bounds = new Thickness (0, 0, 1000, 1000);
        /// View.IsPageEditable = true;
        /// }
        /// }
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="DiagramView"/>
        public DiagramView View
        {
            get
            {
                return (DiagramView)GetValue(ViewProperty);
            }

            set
            {
                SetValue(ViewProperty, value);
            }
        }

        /// <summary>
        /// Calls Connections_CollectionChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="sender"> object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private void Connections_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            dc.View.UndoStack.Clear();
            bool islinepresent = false;
            if (CollectionExt.Cleared && !DiagramControl.IsPageLoaded && !this.nodecleared)
            {
                var conn = this.View.Page.Children.OfType<LineConnector>();
                if (conn.Count() != 0)
                {
                    islinepresent = true;
                    ConnectionDeleteRoutedEventArgs newEventArgs = new ConnectionDeleteRoutedEventArgs(conn as LineConnector);
                    this.View.OnConnectionDeleting(conn as LineConnector, newEventArgs);
                }

                //this.View.Page.Children.Clear();

                List<ICommon> children = this.View.Page.Children.OfType<ICommon>().ToList<ICommon>();
                foreach (ICommon ele in children)
                {
                    (this.View.Page as DiagramPage).AllChildren.Remove(ele as UIElement);
                    (this.View.Page as DiagramPage).RemovingChildren(ele as UIElement);
                    // this.View.Page.Children.Remove(ele as UIElement);
                }

                this.View.SelectionList.Clear();
                //foreach (Node n in this.Model.Nodes)
                //{
                //    this.View.Page.Children.Add(n);
                //}

                if (islinepresent)
                {
                }

                if (this.Model.LayoutType == LayoutType.DirectedTreeLayout || this.Model.LayoutType == LayoutType.HierarchicalTreeLayout || this.Model.LayoutType == LayoutType.BowtieLayout)
                {
                    foreach (Node node in this.Model.Nodes)
                    {
                        node.PxOffsetX = 0;
                        node.PxOffsetY = 0;
                    }
                }

                CollectionExt.Cleared = false;
            }

            try
            {
                if (e.NewItems != null)
                {
                    foreach (IEdge iconn in e.NewItems)
                    {
                        if (!this.View.Undone && !this.View.Redone && !this.View.IsLayout)
                        {
                            this.View.tUndoStack.Push(new LineOperation(LineOperations.Added, iconn as LineConnector));
                        }
                        if (!View.Page.Children.Contains(iconn as Control))
                        {
                            (View.Page as DiagramPage).AllChildren.Add(iconn as Control);
                            if (View.EnableVirtualization)
                            {
                                View.ScrollGrid.VirtualizingLine(iconn as LineConnector);
                            }
                            else
                            {
                                (View.Page as DiagramPage).AddingChildren(iconn as Control);
                            }
                            Canvas.SetZIndex(iconn as Control, View.Page.Children.Count);
                        }
                    }
                }
            }
            catch
            {
            }

            if (e.OldItems != null)
            {
                foreach (IEdge line in e.OldItems)
                {
                    if (!this.View.Islinedeleted && !this.nodecleared)
                    {

                        if (((line.HeadNode as Node) != null && (line.HeadNode as Node).Content != null && (line.HeadNode as Node).Content.ToString() != "VirtualNode") || (line.HeadNode as Node) != null || (line.TailNode as Node) != null)
                        {
                            if (!dc.View.SelectionList.Contains(line as LineConnector))
                            {
                                ConnectionDeleteRoutedEventArgs newEventArgs = new ConnectionDeleteRoutedEventArgs(line as LineConnector);
                                dc.View.OnConnectionDeleting(line as LineConnector, newEventArgs);

                            }
                        }
                    }

                    if (line.TailNode != null)
                    {
                        if (this.View.Isnodedeleted)
                        {
                            if (!this.View.InternalEdges.Contains(line as LineConnector))
                            {
                                this.View.InternalEdges.Add(line as LineConnector);
                            }
                        }
                    }

                    if (line.HeadNode != null)
                    {
                        if (this.View.Isnodedeleted)
                        {
                            if (!this.View.InternalEdges.Contains(line as LineConnector))
                            {
                                this.View.InternalEdges.Add(line as LineConnector);
                            }
                        }
                    }


                    if (!this.View.Undone && !this.View.Redone && !this.View.IsLayout)
                    {
                        this.View.tUndoStack.Push(new LineOperation(LineOperations.Deleted, line as LineConnector));
                    }

                    //if (!this.View.Undone && !this.View.IsLayout)
                    //{
                    //    this.View.UndoStack.Push(line as LineConnector);
                    //    this.View.UndoStack.Push("Deleted");
                    //}

                    if (!this.View.Redone)
                    {
                        this.View.DeleteCount++;
                    }

                    // this.View.Page.Children.Remove(line as LineConnector);
                    (this.View.Page as DiagramPage).RemovingChildren(line as LineConnector);
                    (this.View.Page as DiagramPage).AllChildren.Remove(line as LineConnector);
                    if (!this.nodecleared)
                    {
                        if (((line.HeadNode as Node) != null && (line.HeadNode as Node).Content != null && (line.HeadNode as Node).Content.ToString() != "VirtualNode") || (line.HeadNode as Node) != null || (line.TailNode as Node) != null)
                        {
                            if (!dc.View.SelectionList.Contains(line as LineConnector))
                            {
                                ConnectionDeleteRoutedEventArgs delEventArgs = new ConnectionDeleteRoutedEventArgs(line as LineConnector);
                                this.View.OnConnectionDeleted(line as LineConnector, delEventArgs);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Calls DiagramControl_Loaded method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="sender"> object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private void DiagramControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.IsLoaded = true;
            if (View != null)
            {
                View.IsLoaded = true;
                if (View.ScrollGrid != null)
                {
                    View.ScrollGrid.MakeVirtualization();
                }
            }
            int n = 1;
            int j;
            if (View != null)
            {
                foreach (UIElement element in View.Page.Children.OfType<ICommon>())
                {
                    if ((element.ReadLocalValue(Canvas.ZIndexProperty)==DependencyProperty.UnsetValue))
                    {

                        Canvas.SetZIndex(element, n++);
                        j = Canvas.GetZIndex(element);
                    }

                }
            }
            

            CollectionExt.Cleared = false;
            if (this.IsUnloaded)
            {
            }
            if (this.Model != null)
            {
                if (Model.parser != null)
                {
                    Model.parser.UpdateLayout();
                }
                if (this.Model.LayoutType == LayoutType.DirectedTreeLayout)
                {
                    DirectedTreeLayout tree = new DirectedTreeLayout(this.Model, this.View);
                    tree.RefreshLayout();
                    this.exe = true;
                }
                else if (this.Model.LayoutType == LayoutType.HierarchicalTreeLayout)
                {
                    HierarchicalTreeLayout tree = new HierarchicalTreeLayout(this.Model, this.View);
                    tree.RefreshLayout();
                    this.exe = true;
                }
                else if (this.Model.LayoutType == LayoutType.TableLayout)
                {
                    TableLayout tree = new TableLayout(this.Model, this.View);
                    tree.RefreshLayout();
                    this.exe = true;
                }
                else if (this.Model.LayoutType == LayoutType.RadialTreeLayout)
                {
                    RadialTreeLayout tree = new RadialTreeLayout(this.Model, this.View);
                    tree.RefreshLayout();
                    this.exe = true;
                }
                else if (this.Model.LayoutType == LayoutType.BowtieLayout)
                {
                    BowtieLayout tree = new BowtieLayout(this.Model, this.View);
                    tree.RefreshLayout();
                    this.exe = true;
                }
                if (this.Model.LayoutType == LayoutType.DirectedTreeLayout)
                {
                    DirectedTreeLayout tree = new DirectedTreeLayout(this.Model, this.View);
                    tree.RefreshLayout();
                    this.exe = true;
                }
            }

            InvalidateMeasure();
        }

        /// <summary>
        /// Calls DiagramView_LayoutUpdated method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="sender"> object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private void DiagramView_LayoutUpdated(object sender, EventArgs e)
        {
            this.View.Page.LayoutUpdated -= new EventHandler(this.DiagramView_LayoutUpdated);
            this.InvalidateMeasure();
        }

        /// <summary>
        /// Draws the connections on the DiagramPage.
        /// </summary>
        private void DrawConnectios()
        {
            if (!CollectionExt.Cleared)
            {
                foreach (IEdge connection in this.Model.Connections)
                {
                    if (connection.HeadNode != null && connection.TailNode != null)
                    {
                        (connection as LineConnector).HeadNodeReferenceNo = ((connection as LineConnector).HeadNode as Node).ReferenceNo;
                        (connection as LineConnector).TailNodeReferenceNo = ((connection as LineConnector).TailNode as Node).ReferenceNo;
                    }
                    
                }
                var temp = Model.Connections.OfType<UIElement>();
                if (View != null)
                {
                    var intersect = (View.Page as DiagramPage).AllChildren.Intersect(Model.Connections.OfType<UIElement>());
                    (View.Page as DiagramPage).AllChildren.AddRange(temp.Except(intersect));

                }
            }
        }

        /// <summary>
        /// Draws the nodes on the DiagramPage.
        /// </summary>
        private void DrawNodes()
        {
            if (this.View != null && this.View.Page != null)
            {
                List<ICommon> children = this.View.Page.Children.OfType<ICommon>().ToList<ICommon>();
                foreach (ICommon ele in children)
                {
                    this.View.Page.Children.Remove(ele as UIElement);
                }

                foreach (Node item in this.Model.Nodes)
                {
                    (item as Node).Page = this.View.Page;
                }
                var temp = Model.Nodes.OfType<UIElement>();
                var intersect = (View.Page as DiagramPage).AllChildren.Intersect(Model.Nodes.OfType<UIElement>());
                (View.Page as DiagramPage).AllChildren.AddRange(temp.Except(intersect));
            }
        }

        /// <summary>
        /// Searches for the specified node based on its name and returns the one with the same name.
        /// </summary>
        /// <param name="name">Unique identifier .</param>
        /// <returns>Matched Node.</returns>
        public Node FindByName(string name)
        {
            foreach (Node node in this.Model.Nodes)
            {
                if (node.Name.Equals(name))
                {
                    return node;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the isolated file contents .
        /// </summary>
        /// <param name="style">The style of the node or line connector.</param>
        /// <returns>The contents stored in the isolated file.</returns>
        private string GetIsolatedFileContents(Style style)
        {
            string s = string.Empty;
            return s;
        }
        /// <summary>
        /// Used to store the group count.
        /// </summary>



        SaveFileDialog dialog = new SaveFileDialog();
        public void Save()
        {
            dialog.Filter = "XAML File (*.xaml)|*.xaml";
            if (dialog.ShowDialog() == true)
            {
                if (dialog.SafeFileName != string.Empty)
                {
                    string extension = dialog.SafeFileName;

                    if (extension.ToLower(CultureInfo.InvariantCulture).Contains(".xaml"))
                    {
                        foreach (Node n in Model.Nodes)
                        {
                            n.NewZIndex = Canvas.GetZIndex(n);
                        }
                        this.Save(dialog.SafeFileName);                        
                    }
                    else
                    {
                        //View.Save(dialog.File.Name);
                    }
                }
            }

        }

        internal List<LineConnector> connections;

        void PerformLoadOperations(DiagramPage page)
        {
            List<DiagramViewGrid> dvgs = page.Children.OfType<DiagramViewGrid>().ToList<DiagramViewGrid>();
            while (dvgs.Count > 1)
            {
                page.Children.Remove(dvgs[0]);
                dvgs.RemoveAt(0);
            }
            connections = new List<LineConnector>();
            View.Page = page;
            Model.Orientation = (View.Page as DiagramPage).OrientationRef;
            Model.HorizontalSpacing = (View.Page as DiagramPage).HorizontalSpacingref;
            Model.VerticalSpacing = (View.Page as DiagramPage).VerticalSpacingref;
            Model.SpaceBetweenSubTrees = (View.Page as DiagramPage).SubTreeSpacingref;
            Model.LayoutType = (View.Page as DiagramPage).LayoutTyperef;
            for (int i = 0; i < page.Children.Count; i++)
            {
                UIElement element = page.Children[i];
                if (element is DiagramViewGrid)
                {
                    continue;
                }
                if (element is LineConnector)
                {
                    if (!Model.Connections.Contains(element))
                    {
                        LineConnector ln = element as LineConnector;
                        (element as LineConnector).ID = new Guid((element as LineConnector).ReferenceID);
                        ln.RetrieveSerializationData();
                        //connections.Add(element as LineConnector);
                        //(page as DiagramPage).AllChildren.Add(element);
                        (Model as DiagramModel).Connections.Add(element);
                        if (ln.ConnectorType == ConnectorType.Straight || ln.ConnectorType == ConnectorType.Orthogonal)
                        {
                            try
                            {


                            }
                            catch { }
                        }
                    }

                }

                if ((element is Group))
                {
                    foreach (int no in (element as Group).GroupChildrenRef)
                    {
                        foreach (UIElement child in page.Children)
                        {
                            if ((child is INodeGroup) && (child as INodeGroup).IsGrouped && (child as INodeGroup).ReferenceNo == no)
                            {
                                if (!(element as Group).NodeChildren.Contains(child as INodeGroup))
                                {
                                    (element as Group).AddChild(child as INodeGroup);
                                }

                                if (!(child as INodeGroup).Groups.Contains(element as Group))
                                {
                                    (child as INodeGroup).Groups.Add(element as Group);
                                }

                                break;
                            }
                        }
                    }
                }
                if (element is IShape)
                {
                    if (!Model.Nodes.Contains(element))
                    {
                        if ((element as Node).ContentHitTestVisible)
                        {
                            if ((element as Node).Content != null && (element as Node).Content is UIElement)
                                ((element as Node).Content as UIElement).IsHitTestVisible = true;
                        }

                        (element as Node).MeasurementUnits = page.MeasurementUnits;
                        (element as Node).Width = (element as Node).Width;// MeasureUnitsConverter.FromPixels((element as Node).Width, (this.View.Page as DiagramPage).MeasurementUnits);
                        (element as Node).Height = (element as Node).Height;// MeasureUnitsConverter.FromPixels((element as Node).Height, (this.View.Page as DiagramPage).MeasurementUnits);
                        (element as Node).ID = new Guid((element as Node).ReferenceID);
                        if (page.StyleRef != null)
                        {
                            Node n = element as Node;
                            (element as Node).Style = page.StyleRef as Style;
                            n.Style = page.StyleRef as Style;
                            Setter s = page.StyleRef.Setters[1] as Setter;
                        }
                        if (page != null)
                        {
                            (element as Node).Page = page;

                        }
                        if ((element as Node).IsSelected)
                        {
                            (element as Node).IsSelected = false;
                        }

                        if ((element as Node).PathStyle.PathObject != null)
                        {
                            (element as Node).NodeShape = (element as Node).PathStyle.PathObject as System.Windows.Shapes.Path;
                            if ((element as Node).Shape == Shapes.CustomPath)
                            {
                                Style CPS = new Style(typeof(System.Windows.Shapes.Path));
                                CPS.Setters.Add(new Setter(System.Windows.Shapes.Path.StrokeProperty, ((element as Node).PathStyle.PathObject as System.Windows.Shapes.Path).Stroke));
                                CPS.Setters.Add(new Setter(System.Windows.Shapes.Path.FillProperty, ((element as Node).PathStyle.PathObject as System.Windows.Shapes.Path).Fill));
                                CPS.Setters.Add(new Setter(System.Windows.Shapes.Path.StrokeThicknessProperty, ((element as Node).PathStyle.PathObject as System.Windows.Shapes.Path).StrokeThickness));
                                CPS.Setters.Add(new Setter(System.Windows.Shapes.Path.StretchProperty, ((element as Node).PathStyle.PathObject as System.Windows.Shapes.Path).Stretch));
                                (element as Node).CustomPathStyle = CPS;
                            }
                        }
                        (page as DiagramPage).AllChildren.Add(element);
                        Model.Nodes.Add(element);

                        foreach (ConnectionPort port in (element as Node).Ports)
                        {
                            if (port.CenterPortReferenceNo == 0)
                            {
                                port.Name = "PART_Sync_CenterPort";
                                (element as Node).centerport = port;
                            }

                            port.Node = element as Node;

                        }
                    }
                }

                else                  
                    {
                        foreach (UIElement node in page.Children)
                        {
                            if (node is Node)
                            {


                                if (page.LineStyleRef != null)
                                {
                                    (element as LineConnector).Style = page.LineStyleRef;
                                }

                                if ((element as LineConnector).HeadNodeReferenceNo == (node as Node).ReferenceNo)
                                {
                                    (element as LineConnector).HeadNode = node as Node;
                                    foreach (ConnectionPort port in (node as Node).Ports)
                                    {
                                        if (port.Name != "PART_Sync_CenterPort")
                                        {
                                            if ((element as LineConnector).HeadPortReferenceNo == port.PortReferenceNo)
                                            {
                                                (element as LineConnector).ConnectionHeadPort = port;
                                            }
                                        }
                                    }
                                }
                                else if ((element as LineConnector).TailNodeReferenceNo == (node as Node).ReferenceNo)
                                {
                                    (element as LineConnector).TailNode = node as Node;
                                    foreach (ConnectionPort port in (node as Node).Ports)
                                    {
                                        if (port.Name != "PART_Sync_CenterPort")
                                        {

                                            if ((element as LineConnector).TailPortReferenceNo == port.PortReferenceNo)
                                            {
                                                (element as LineConnector).ConnectionTailPort = port;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                    }


                //if (page.Children.Count != 0)
                //{

                //    UIElement ui = page.Children[0] as UIElement;


                //    if (ui is FrameworkElement)
                //    {
                //        (ui as FrameworkElement).Loaded += new RoutedEventHandler(ss);

                //    }
                //}
                View.IsJustScrolled = false;


                View.ViewGridOrigin = new Point(0, 0);
                View.X = 0;
                View.Y = 0;
                double b = (View.Page as DiagramPage).Dragleft;
                CollectionExt.Cleared = false;
                foreach (LineConnector line in Model.Connections)
                {
                    (page as DiagramPage).AllChildren.Add(line);
                    //line.UpdateLayout();
                }
            }
            foreach (UIElement ele in page.Children)
            {
                if (ele is Node)
                {
                    (ele as Node).IsInternallyLoaded = true;
                }
                else if (ele is LineConnector)
                {
                    (ele as LineConnector).IsInternallyLoaded = true;
                }
            }
            foreach (LineConnector line in Model.Connections)
            {
                (page as DiagramPage).AllChildren.Add(line);
                line.UpdateLayout();
            }
            View.VerifyVirtualization();
        }

        //void ss(object sender, RoutedEventArgs e)
        //{
        //    if (connections != null)
        //    {
        //        foreach (var connector in connections)
        //        {

        //            //(Model as DiagramModel).Connections.Add(connector);
        //        }


        //        if (sender is FrameworkElement)
        //        {
        //            (sender as FrameworkElement).Loaded -= ss;

        //        }
        //        connections.Clear();
        //        connections = null;
        //    }
        //}





        /// <summary>
        /// Clones the specified obj.
        /// </summary>
        /// <param name="obj">The object to be cloned.</param>
        /// <returns>The cloned object.</returns>
        internal object Clone(object obj, int isPath)
        {
            PropertyInfo[] properties = obj.GetType().GetProperties();
            object cloneObj = obj.GetType().GetConstructors()[0].Invoke(null);
            foreach (PropertyInfo property in properties)
            {
                object value = property.GetValue(obj, null);
                if (value != null)
                {
                    try
                    {
                        if (IsPresentationFrameworkCollection(value.GetType()))
                        {
                            object collection = property.GetValue(obj, null);
                            int count = (int)collection.GetType().GetProperty("Count").GetValue(collection, null);
                            for (int i = 0; i < count; i++)
                            {
                                object child = collection.GetType().GetProperty("Item").GetValue(collection, new object[] { i });
                                object cloneChild = this.Clone(child, 0);
                                object cloneCollection = property.GetValue(cloneObj, null);
                                collection.GetType().InvokeMember("Add", BindingFlags.InvokeMethod, null, cloneCollection, new object[] { cloneChild });
                            }
                        }

                        if (value is UIElement)
                        {
                            object obj2 = property.PropertyType.GetConstructors()[0].Invoke(null);
                            Clone(obj2, 0);
                            property.SetValue(cloneObj, obj2, null);
                        }
                        else if (property.CanWrite)
                        {
                            if (property.ToString().Contains("Data") && isPath > 0)
                            {
                                PathGeometry geo = value as PathGeometry;
                                PathGeometry pathgeo = (PathGeometry)this.Clone(geo, 0);
                                property.SetValue(cloneObj, pathgeo, null);
                            }
                            else
                            {
                                property.SetValue(cloneObj, value, null);
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }

            return cloneObj;
        }

        /// <summary>
        /// Determines whether [is presentation framework collection] [the specified type].
        /// </summary>
        /// <param name="type">The type of the object.</param>
        /// <returns>
        /// <c>true</c> if [is presentation framework collection] [the specified type]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsPresentationFrameworkCollection(Type type)
        {
            if (type == typeof(object))
            {
                return false;
            }

            if (type.Name.StartsWith("PresentationFrameworkCollection"))
            {
                return true;
            }

            return this.IsPresentationFrameworkCollection(type.BaseType);
        }

        /// <summary>
        /// Clones the Path object
        /// </summary>
        /// <param name="element">Element to be clone</param>
        /// <param name="obj">object that is cloned</param>
        /// <returns></returns>
        internal object ClonePath(object element, object obj)
        {
            PropertyInfo[] properties = element.GetType().GetProperties();
            foreach (PropertyInfo property in properties)
            {
                object value = property.GetValue(element, null);
                if (value != null)
                {
                    try
                    {
                        if (this.IsPresentationFrameworkCollection(value.GetType()))
                        {
                            object collection = property.GetValue(element, null);
                            int count = (int)collection.GetType().GetProperty("Count").GetValue(collection, null);
                            for (int i = 0; i < count; i++)
                            {
                                object child = collection.GetType().GetProperty("Item").GetValue(collection, new object[] { i });
                                object cloneChild = Clone(child, 0);
                                object cloneCollection = property.GetValue(obj, null);
                                collection.GetType().InvokeMember("Add", BindingFlags.InvokeMethod, null, cloneCollection, new object[] { cloneChild });
                            }
                        }

                        if (value is UIElement)
                        {
                            object obj2 = property.PropertyType.GetConstructors()[0].Invoke(null);
                            this.Clone(obj2, 0);
                            property.SetValue(element, obj2, null);
                        }
                        else if (property.CanWrite)
                        {
                            //if (property.ToString().Contains("Data"))
                            //{
                            property.SetValue(obj, value, null);
                            //}
                        }
                    }
                    catch
                    {
                    }
                }
            }

            return obj;
        }

        public void Load(StreamReader filename)
        {
            View.UndoStack.Clear();
            View.RedoStack.Clear();
            DiagramControl.IsPageLoaded = true;
            //View.Page.Children.Clear();
            List<ICommon> children = this.View.Page.Children.OfType<ICommon>().ToList<ICommon>();
            foreach (ICommon ele in children)
            {
                (this.View.Page as DiagramPage).RemovingChildren(ele as UIElement);
            }
            Model.Nodes.Clear();
            Model.Connections.Clear();
            Model.Nodes.Clear();
            foreach (UIElement ele in (this.View.Page as DiagramPage).AllChildren)
            {
                if (ele is Node)
                {
                    (ele as Node).IsInternallyLoaded = false;
                }
                else if (ele is LineConnector)
                {
                    (ele as LineConnector).IsInternallyLoaded = false;
                }
            }
            string str = filename.ReadToEnd();
            Load(str);
        }

        DependencyObject targetBlank;



        public void Load(string str)
        {
            if (str.Contains("http://schemas.syncfusion.com/wpf"))
            {
                str = str.Replace("http://schemas.syncfusion.com/wpf", "clr-namespace:Syncfusion.Windows.Diagram;assembly=Syncfusion.Diagram.Silverlight");
                LoadWPFXAML(str,false);

            }
            else
            {
                string s1 = "PortVisibility=" + @"""Visible""";
                string s2 = "PortVisibility=" + @"""Collapsed""";

                if (str.Contains(s1))
                {
                    str = str.Replace(s1, "PortVisibility=" + @"""AlwaysVisible""");
                }
                if (str.Contains(s2))
                {
                    str = str.Replace(s2, "PortVisibility=" + @"""MouseOverNode""");
                }

                string re1 = "(LogicalOffset)";
                string re2 = ".*?";
                string re3 = "(\\s+)";
                string re4 = ".*?";
                string re5 = "(\".*?\")";
                Regex r = new Regex(re1 + re2 + re3 + re4 + re5, RegexOptions.Singleline);
                str = r.Replace(str, "");

                DiagramPage page = XamlReader.Load(str) as DiagramPage;
                PerformLoadOperations(page);
                page.InvalidateMeasure();
                this.View.ScrollGrid.InvalidateMeasure();
                page.dview = this.View;
                (page as DiagramPage).RulerUpdate();
                page.dview.UpdateLayout();
            }
        }


        private void LoadWPFXAML(string str,bool symbol)
        {
            XDocument document = XDocument.Load(new StringReader(str.ToString()));
            Assembly ass = Assembly.Load(Assembly.GetExecutingAssembly().FullName);
            Assembly asse = Assembly.Load("System.Windows, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e");
            Type[] types = asse.GetTypes();
            int d1 = document.Descendants().Count();
            IEnumerable<XName> sname = from c in document.Descendants() select c.Name;
            sname = sname.Distinct();
            int s = sname.Count();

            IEnumerable<XName> stringcheck = sname.Where<XName>(delegate(XName c)
            {
                targetBlank = null;
                object obj = null;
                if (c.NamespaceName == "clr-namespace:Syncfusion.Windows.Diagram;assembly=Syncfusion.Diagram.Silverlight")
                {
                    string stri = "Syncfusion.Windows.Diagram.";
                    targetBlank = ass.CreateInstance(stri + c.LocalName) as DependencyObject;
                    if (targetBlank == null)
                    {
                        try
                        {
                            targetBlank = ass.CreateInstance(stri + c.LocalName.Substring(0, c.LocalName.IndexOf("."))) as DependencyObject;
                            string d = c.LocalName.Substring(c.LocalName.IndexOf(".") + 1);
                            PropertyInfo info = targetBlank.GetType().GetProperty(d);
                            if (info == null)
                                targetBlank = null;

                        }
                        catch
                        {

                        }
                    }
                }
                else
                {
                    if (c.NamespaceName.Contains("presentation"))
                    {

                        int temp = c.LocalName.IndexOf(".");
                        string stri = (temp == -1) ? c.LocalName : c.LocalName.Substring(0, c.LocalName.IndexOf("."));
                        IEnumerable<Type> on = from tt in types where tt.FullName.EndsWith("." + stri) select tt;
                        int ct = on.Count();
                        if (ct > 0)
                        {
                            Type t = on.ElementAt(0) as Type;
                            try
                            {
                                targetBlank = asse.CreateInstance(t.FullName) as DependencyObject;
                                if (targetBlank == null)
                                {
                                    obj = asse.CreateInstance(t.FullName) as object;
                                }
                                if (temp > 0)
                                {
                                    try
                                    {
                                        string d = c.LocalName.Substring(c.LocalName.IndexOf(".") + 1);
                                        PropertyInfo info = targetBlank.GetType().GetProperty(d);
                                        if (info == null)
                                            targetBlank = null;

                                    }
                                    catch
                                    {

                                    }

                                }

                            }

                            catch
                            {
                                targetBlank = null;

                            }
                        }
                        else
                            return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                if (targetBlank == null && obj == null && !c.LocalName.Contains("Font") && !c.LocalName.Contains("IsPortEnabled") && !c.LocalName.Contains("LineBridgingEnabled") && !c.LocalName.Contains("UIElement.SnapsToDevicePixels"))
                {
                    return true;
                }
                return false;

            });

            while (s > 0)
            {
                foreach (XName name in stringcheck)
                {
                    try
                    {

                        document.Descendants().Elements(stringcheck.ElementAt(stringcheck.Count() - s) as XName).Remove();
                        break;
                    }
                    catch
                    {
                        break;
                    }
                }
                s--;

            }

            List<XElement> ele = new List<XElement>();
            foreach (XElement element in document.Descendants())
            {
                if (element.Name.LocalName.Contains("Static"))
                {
                    ele.Add(element);
                    element.Parent.Add(element.FirstAttribute.Value.Substring(element.FirstAttribute.Value.IndexOf(".") + 1));

                }
                else if (element.Name.LocalName.Contains("ConnectorPathGeometry"))
                {
                    ele.Add(element);
                }

                List<XAttribute> attr = new List<XAttribute>();
                foreach (XAttribute xattr in element.Attributes())
                {
                    if (xattr.Name.LocalName == "Panel.ZIndex" || xattr.Name.LocalName == "ConnectorPathGeometry" || xattr.Name.LocalName == "ToolTip" || xattr.Name.LocalName == "DecoratorAdornerStyle" || xattr.Name.LocalName == "Focusable" || xattr.Name.LocalName == "FocusManager.IsFocusScope" || xattr.Name.LocalName == "LineBridgingEnabled" || xattr.Name.LocalName == "UIElement.SnapsToDevicePixels" || xattr.Name.LocalName == "IsPolyLineEnabled" || xattr.Name.LocalName == "SerializationData" || xattr.Name.LocalName == "IsManipulationEnabled" || xattr.Name.LocalName == "MinimumSegementLength")
                    {
                        attr.Add(xattr);
                    }
                }

                foreach (XAttribute xa in attr)
                {
                    if (xa != null)
                        element.Attributes(xa.Name).Remove();
                }

                List<XNode> nod = new List<XNode>();
                int k = 0;
                foreach (XNode node in element.DescendantNodes())
                {
                    k++;
                    if (node.ToString().Contains("UIElement.SnapsToDevicePixels"))
                    {
                        element.DescendantNodes().ElementAt(k).Remove();
                        // nod[k] = node;
                        // k++;
                    }
                }


            }

            IEnumerable<XElement> source = document.Descendants().Where<XElement>(delegate(XElement c)
            {
                if (ele.Contains(c))
                {
                    return true;
                }
                return false;
            });
            while (source.Any<XElement>())
            {
                Extensions.Remove<XElement>(source);
            }
            StringWriter strwri = new StringWriter();
            document.Save(strwri);
            str = strwri.ToString();
            if (symbol)
            {
                SymbolPaletteGroup page = XamlReader.Load(str) as SymbolPaletteGroup;
                this.SymbolPalette.SymbolGroups.Add(page);
            }
            else
            {
                DiagramPage page = XamlReader.Load(str) as DiagramPage;
                page.RenderTransform = this.View.zoomTransform;
                PerformLoadOperations(page);
            }
        }

        public void Load(Stream stream)
        {
            DiagramControl.IsPageLoaded = true;
            //View.Page.Children.Clear();

            List<ICommon> children = this.View.Page.Children.OfType<ICommon>().ToList<ICommon>();
            foreach (ICommon ele in children)
            {
                this.View.Page.Children.Remove(ele as UIElement);
            }
            Model.Nodes.Clear();
            Model.Nodes.Clear();
            Model.Connections.Clear();
            StreamReader sr = new StreamReader(stream);
            stream.Position = 0;
            string str = sr.ReadToEnd();
            Load(str);
            foreach (Node n in Model.Nodes)
            {
                Canvas.SetZIndex(n, n.NewZIndex);
            }
        }

        public void Load()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "XAML Files (*.xaml)|*.xaml";

            if (dialog.ShowDialog() == true)
            {                
                Load(dialog.File.OpenText());
                foreach (Node n in Model.Nodes)
                {
                    Canvas.SetZIndex(n, n.NewZIndex);
                }
            }
        }

        private void PreSave()
        {
            foreach (UIElement element in View.Page.Children)
            {
                if (element is Group)
                {
                    foreach (INodeGroup child in (element as Group).NodeChildren)
                    {
                        (element as Group).GroupChildrenRef.Add(child.ReferenceNo);
                    }
                }
                else if (element is LineConnector)
                {
                    (element as LineConnector).SetSerializationData();
                    //(element as LineConnector).MeasurementUnit = (View.Page as DiagramPage).MeasurementUnits;
                }
                else if (element is Node)
                {
                    if ((element as Node).Content != null)
                    {
                        if ((element as Node).Content.GetType() != typeof(string) && ((element as Node).Content is UIElement) && ((element as Node).Content as UIElement).IsHitTestVisible)
                        {
                            (element as Node).ContentHitTestVisible = true;
                        }

                        //this.PathStyle.Data = this.NodeShape.Data;
                    }
                    Node n = element as Node;
                    n.PathStyle.Fill = n.NodePathFill;
                    n.PathStyle.PathObject = n.NodeShape as System.Windows.Shapes.Path;
                    n.NewZIndex = Canvas.GetZIndex(n);
                }
            }
            (View.Page as DiagramPage).OrientationRef = Model.Orientation;
            (View.Page as DiagramPage).HorizontalSpacingref = Model.HorizontalSpacing;
            (View.Page as DiagramPage).VerticalSpacingref = Model.VerticalSpacing;
            (View.Page as DiagramPage).SubTreeSpacingref = Model.SpaceBetweenSubTrees;
            (View.Page as DiagramPage).LayoutTyperef = Model.LayoutType;
        }


        public void Save(Stream stream)
        {
            PreSave();
            DiagramPageXamlWriter xamlWriter = new DiagramPageXamlWriter();
            string xaml = xamlWriter.WriteXaml(this.View.Page);
            stream.Position = 0;
            StreamWriter sw = new StreamWriter(stream);
            sw.Write(xaml);
            sw.Flush();
        }

        public void Save(string filename)
        {
            PreSave();
            DiagramPageXamlWriter xamlWriter = new DiagramPageXamlWriter();
            string xaml = xamlWriter.WriteXaml(this.View.Page);
            StreamWriter str = new StreamWriter(dialog.OpenFile());
            str.Write(xaml);
            str.Close();
        }

        #region SymbolPalette Save and Load

        #region SymbolPalette SaveandLoad

        public void SaveSymbolPalette()
        {
            try
            {
                dialog.Filter = "XAML File (*.xaml)|*.xaml";
                if (dialog.ShowDialog() == true)
                {
                    if (dialog.SafeFileName != string.Empty)
                    {
                        string extension = dialog.SafeFileName;
                        if (extension.ToLower(CultureInfo.InvariantCulture).Contains(".xaml"))
                        {
                            this.SaveSymbolPalette(dialog.SafeFileName);
                        }
                    }
                }
            }
            catch { }
        }

        internal void SaveSymbolPalette(string filename)
        {
            DiagramPageXamlWriter xamlWriter = new DiagramPageXamlWriter();
            StreamWriter str = new StreamWriter(dialog.OpenFile());
            foreach (SymbolPaletteGroup group in this.SymbolPalette.SymbolGroups)
            {
                string xaml = xamlWriter.WriteXaml(group);
                str.Write(xaml);
            }
            str.Close();
        }
        public void LoadSymbolPalette()
        {
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "XAML Files (*.xaml)|*.xaml";
                if (dialog.ShowDialog() == true)
                {
                    LoadSymbolPalette(dialog.File.OpenText());
                }
            }
            catch { }
        }
        public bool LoadSymbolPalette(StreamReader file)
        {
            try
            {
                string str = file.ReadToEnd();
                string[] GroupofSymbol = Regex.Split(str, "</SymbolPaletteGroup>");
                this.SymbolPalette.SymbolGroups.Clear();
                foreach (string str1 in GroupofSymbol)
                {
                    if (str1.Length > 1)
                    {
                        string ass = str1.Insert(str1.Length, "</SymbolPaletteGroup>");
                        LoadSymbolPalette(ass);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        internal void LoadSymbolPalette(string str)
        {
            if (str.Contains("http://schemas.syncfusion.com/wpf"))
            {
                str = str.Replace("http://schemas.syncfusion.com/wpf", "clr-namespace:Syncfusion.Windows.Diagram;assembly=Syncfusion.Diagram.Silverlight");
                LoadWPFXAML(str, true);

            }
            else
            {
                SymbolPaletteGroup symbol = (SymbolPaletteGroup)XamlReader.Load(str) as SymbolPaletteGroup;
                this.SymbolPalette.SymbolGroups.Add(symbol);
            }
        }
        #endregion


        #region SymbolPaletteGroup Save and Load

        public void SaveSymbolPaletteGroup(SymbolPaletteGroup SymbolGroup)
        {
            try
            {
                if (SymbolGroup != null)
                {
                    dialog.Filter = "XAML File (*.xaml)|*.xaml";
                    if (dialog.ShowDialog() == true)
                    {
                        if (dialog.SafeFileName != string.Empty)
                        {
                            string extension = dialog.SafeFileName;

                            if (extension.ToLower(CultureInfo.InvariantCulture).Contains(".xaml"))
                            {
                                SaveSymbolPaletteGroup(dialog.SafeFileName, SymbolGroup);
                            }
                        }
                    }
                }
            }
            catch { }
        }

        internal void SaveSymbolPaletteGroup(string filename, SymbolPaletteGroup SymbolPaletteGroup)
        {

            DiagramPageXamlWriter xamlWriter = new DiagramPageXamlWriter();
            string xaml = xamlWriter.WriteXaml(SymbolPaletteGroup);
            StreamWriter str = new StreamWriter(dialog.OpenFile());
            str.Write(xaml);
            str.Close();
        }

        public void LoadSymbolPaletteGroup()
        {
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "XAML Files (*.xaml)|*.xaml";
                if (dialog.ShowDialog() == true)
                {
                    LoadSymbolGroup(dialog.File.OpenText(), -1);
                }
            }
            catch { }
        }

        public bool AppendSymbolPaletteGroup(SymbolPaletteGroup ReplaceGroup)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "XAML Files (*.xaml)|*.xaml";
            if (dialog.ShowDialog() == true)
            {
                int i = this.SymbolPalette.SymbolGroups.IndexOf(ReplaceGroup);
                this.SymbolPalette.SymbolGroups.Remove(ReplaceGroup);
                return LoadSymbolGroup(dialog.File.OpenText(),i);
            }
            else
            {
                return false;
            }
        }

        internal bool LoadSymbolGroup(StreamReader stream,int index)
        {
            try
            {
                string str = stream.ReadToEnd();
                LoadGroup(str, index);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        private void LoadGroup(string str,int index)
        {
            if (index >= 0)
            {
                SymbolPaletteGroup page = XamlReader.Load(str) as SymbolPaletteGroup;
                this.SymbolPalette.SymbolGroups.Insert(index, page);
            }
            else
            {
                SymbolPaletteGroup page = XamlReader.Load(str) as SymbolPaletteGroup;
                this.SymbolPalette.SymbolGroups.Add(page);
            }
        }
        #endregion


        #region SymbolPaletteItem SaveAndLoad

        public void SaveSymbolPaletteItem(SymbolPaletteItem symbolPaletteItem)
        {
            try
            {
                if (symbolPaletteItem != null)
                {
                    dialog.Filter = "XAML File (*.xaml)|*.xaml";
                    if (dialog.ShowDialog() == true)
                    {
                        if (dialog.SafeFileName != string.Empty)
                        {
                            string extension = dialog.SafeFileName;

                            if (extension.ToLower(CultureInfo.InvariantCulture).Contains(".xaml"))
                            {
                                SaveSymbolPaletteItem(dialog.SafeFileName, symbolPaletteItem);
                            }
                        }
                    }
                }
            }
            catch { }
        }

        internal void SaveSymbolPaletteItem(string filename, SymbolPaletteItem SymbolPaletteItem)
        {
            DiagramPageXamlWriter xamlWriter = new DiagramPageXamlWriter();
            string xaml = xamlWriter.WriteXaml(SymbolPaletteItem);
            StreamWriter str = new StreamWriter(dialog.OpenFile());
            str.Write(xaml);
            str.Close();
        }
        public void LoadSymbolPaletteItem(SymbolPaletteGroup SymbolPaletteGroup)
        {
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "XAML Files (*.xaml)|*.xaml";
                if (dialog.ShowDialog() == true)
                {
                    LoadSymbolItem(dialog.File.OpenText(), SymbolPaletteGroup);
                }
            }
            catch { }
        }
        internal bool LoadSymbolItem(StreamReader stream, SymbolPaletteGroup symbolgroup)
        {
            string str = stream.ReadToEnd();
            return LoadSymbolItem(str,symbolgroup);
        }

        internal bool LoadSymbolItem(string str,SymbolPaletteGroup group)
        {
            try
            {
                SymbolPaletteItem item = XamlReader.Load(str) as SymbolPaletteItem;
                group.Items.Add(item);
                return true;
            }
            catch
            {
                return false;
            }

            
        }
        #endregion


        #endregion

        /// <summary>
        /// Calls InternalNodes_CollectionChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="sender"> object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        public void InternalNodes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (CollectionExt.Cleared && !DiagramControl.IsPageLoaded)
            {
                if (this.Model.Connections.Count != 0)
                {
                    foreach (LineConnector line in Model.Connections)
                    {
                        if (line.HeadNode != null || line.TailNode != null)
                        {
                            ConnectionDeleteRoutedEventArgs newEventArgs3 = new ConnectionDeleteRoutedEventArgs(line as LineConnector);
                            this.View.OnConnectionDeleting(line as LineConnector, newEventArgs3);
                        }

                        break;
                    }
                }

                this.View.SelectionList.Clear();
                CollectionExt connections = new CollectionExt();
                foreach (ICommon obj in this.View.Page.Children.OfType<ICommon>())
                {
                    connections.Add(obj);
                }

                foreach (ICommon shape in connections)
                {
                    if (shape is Node)
                    {
                        if (this.isnodepresent)
                        {
                            NodeDeleteRoutedEventArgs newEventArgs = new NodeDeleteRoutedEventArgs(shape as Node);
                            View.OnNodeDeleting(shape as Node, newEventArgs);
                        }

                        this.isnodepresent = false;
                        (this.View.Page as DiagramPage).RemovingChildren(shape as Node);
                        (this.View.Page as DiagramPage).AllChildren.Remove(shape as Node);
                        //this.View.Page.Children.Remove(shape as Node);
                        this.noderemoved = true;
                    }
                }

                CollectionExt.Cleared = false;
                this.View.X = 0;
                this.View.Y = 0;
                this.View.DupHorthumbdrag = 0;
                this.View.DupVerthumbdrag = 0;
                this.View.ViewGridOrigin = new Point(0, 0);
                if (this.noderemoved)
                {
                }

                if (this.connremoved)
                {
                }

                this.connremoved = false;
                this.noderemoved = false;
            }

            try
            {
                if (e.NewItems != null)
                {
                    foreach (IShape inode in e.NewItems)
                    {
                        if (!this.View.Undone && !this.View.Redone && !this.View.IsLayout)
                        {
                            this.View.tUndoStack.Push(new NodeOperation(NodeOperations.Added, inode as Node));
                        }
                        (inode as Node).Page = this.View.Page;
                        if (!View.Page.Children.Contains(inode as Node))
                        {

                            (View.Page as DiagramPage).AllChildren.Add(inode as Node);
                            if (View.EnableVirtualization)
                            {
                                View.ScrollGrid.VirtualzingNode(inode as Node);
                            }
                            else
                            {
                                (View.Page as DiagramPage).AddingChildren(inode as Node);
                            }
                            Canvas.SetZIndex(inode as Node, View.Page.Children.Count);
                        }

                    }
                }
            }
            catch
            {
            }

            if (e.OldItems != null)
            {
                foreach (IShape inode in e.OldItems)
                {
                    if (!this.View.Isnodedeleted)
                    {
                    }

                    this.View.Isnodedeleted = true;

                    if ((inode as Node).Content == null)
                    {
                        this.View.DupDeleted = true;
                    }

                    if ((inode as Node).Content != null)
                    {
                        if ((inode as Node).Content.ToString() != "VirtualNode")
                        {
                            this.View.DupDeleted = true;
                        }
                    }

                    CollectionExt head = new CollectionExt();
                    CollectionExt tail = new CollectionExt();

                    List<IEdge> edges = (inode as Node).Edges.OfType<IEdge>().ToList();
                    foreach (IEdge line in edges)
                    {
                        if ((inode as Node).DeletingMode == DeletingMode.DeleteDependentEdges)
                        {
                            ConnectionDeleteRoutedEventArgs newEventArgs3 = new ConnectionDeleteRoutedEventArgs(line as LineConnector);
                            this.View.OnConnectionDeleting(line as LineConnector, newEventArgs3);
                            if (newEventArgs3.Cancel == false)
                            {
                                // this.View.Page.Children.Remove(line as LineConnector);
                                (this.View.Page as DiagramPage).AllChildren.Remove(line as LineConnector);
                                (this.View.Page as DiagramPage).RemovingChildren(line as LineConnector);
                                this.Model.Connections.Remove(line as LineConnector);
                            }
                        }
                        else
                        {
                            if ((inode as Node).InEdges.Contains(line))
                            {
                                tail.Add(line as LineConnector);
                            }
                            if ((inode as Node).OutEdges.Contains(line))
                            {
                                head.Add(line as LineConnector);
                            }
                        }
                    }

                    foreach (LineConnector l in head)
                    {
                        (l as LineConnector).PxStartPointPosition = (l as LineConnector).m_TempStart;
                        (l as LineConnector).HeadNode = null;
                    }

                    foreach (LineConnector l in tail)
                    {
                        (l as LineConnector).PxEndPointPosition = (l as LineConnector).m_TempEnd;
                        (l as LineConnector).TailNode = null;
                    }
                    
                    this.View.InternalEdges.Clear();
                    CollectionExt.Cleared = false;

                    if (!this.View.Redone)
                    {
                        this.View.DeleteCount++;
                    }

                    if (!this.View.Undone && !this.View.Redone && !this.View.IsLayout)
                    {
                        this.View.tUndoStack.Push(new NodeOperation(NodeOperations.Deleted, inode as Node));
                        //this.View.UndoStack.Push(inode);
                        //this.View.UndoStack.Push("xDeleted");
                    }
                    (this.View.Page as DiagramPage).AllChildren.Remove(inode as Node);
                    (this.View.Page as DiagramPage).RemovingChildren(inode as Node);
                    // this.View.Page.Children.Remove(inode as Node);
                    if ((inode as Node).Content != null && (inode as Node).Content.ToString() != "VirtualNode" && !this.View.Isnodedeleted)
                    {
                        NodeDeleteRoutedEventArgs newEventArgs = new NodeDeleteRoutedEventArgs(inode as Node);
                        this.View.OnNodeDeleted(this, newEventArgs);
                    }

                    this.View.Isnodedeleted = false;
                }
            }
        }

        internal ResourceWrapper m_ResourceWrapper;

        /// <summary>
        /// Overrides the OnApplyTemplate().
        /// </summary>
        public override void OnApplyTemplate()
        {
            m_ResourceWrapper = new ResourceWrapper(LocalizationPath);
            

            

            CollectionExt.Cleared = false;
            d_commands.View = this.View;
            d_commands.Model = this.Model;
            if (this.View != null)
            {
                this.View.Page.LayoutUpdated += new EventHandler(this.DiagramView_LayoutUpdated);
            }

            if (this.Model != null)
            {
                if (this.Model.Nodes != null)
                {
                    if (this.Model.Nodes.Count == 0)
                    {
                        this.Model.LayoutRoot = null;
                    }

                    if (this.Model.LayoutRoot != null)
                    {
                        if (this.Model.Nodes.Contains(this.Model.LayoutRoot))
                        {
                            if (this.Model.RootNodes.Count > 0 && this.Model.LayoutRoot.Edges.Count == 0)
                            {
                                foreach (Node node in this.Model.RootNodes)
                                {
                                    LineConnector ortho = new LineConnector();
                                    ortho.HeadNode = this.Model.LayoutRoot;
                                    ortho.TailNode = node;
                                    ortho.Content = node.Content;
                                    this.Model.Connections.Add(ortho);
                                }
                            }
                        }
                        else
                        {
                        }
                    }
                    else
                    {
                        if (this.Model.RootNodes.Count > 0)
                        {
                            this.Model.LayoutRoot = this.Model.RootNodes[0] as IShape;
                        }
                    }

                    this.DrawNodes();
                    this.Model.Nodes.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(this.InternalNodes_CollectionChanged);
                }

                if (this.Model.Connections != null)
                {
                    this.DrawConnectios();
                    this.Model.Connections.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(this.Connections_CollectionChanged);
                }

            }

            if (!this.exe)
            {
                if (this.Model != null)
                {

                }
            }

            
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == Key.Z && (Keyboard.Modifiers & ModifierKeys.Control) == (ModifierKeys.Control))
            {
                this.Undo.Execute(this.View);
                //DiagramView.Undo(this.View);
            }
            else if (e.Key == Key.Y && (Keyboard.Modifiers & ModifierKeys.Control) == (ModifierKeys.Control))
            {
                DiagramView.Redo(this.View);
            }
            else if (e.Key == Key.A && (Keyboard.Modifiers & ModifierKeys.Control) == (ModifierKeys.Control))
            {
                DiagramView.SelectAll(this.View);
            }
            else if (e.Key == Key.X && (Keyboard.Modifiers & ModifierKeys.Control) == (ModifierKeys.Control))
            {
                if (View.IsPageEditable && View.IsCutEnabled)
                {
                    if (View.SelectionList.Count > 0)
                    {
                        CopyPasteManager cpm = new CopyPasteManager(View as DiagramView);
                        cpm.cut();
                    }
                }
            }
            else if (e.Key == Key.C && (Keyboard.Modifiers & ModifierKeys.Control) == (ModifierKeys.Control))
            {
                if (View.IsPageEditable && View.IsCopyEnabled)
                {
                    if (View.SelectionList.Count > 0)
                    {
                        CopyPasteManager cpm = new CopyPasteManager(View as DiagramView);
                        cpm.copy();
                    }
                }
            }
            else if (e.Key == Key.V && (Keyboard.Modifiers & ModifierKeys.Control) == (ModifierKeys.Control))
            {
                if (View.IsPageEditable && View.IsPasteEnabled)
                {
                    if (CopyPasteManager.IsValidStringContent)
                    {
                        CopyPasteManager cpm = new CopyPasteManager(View as DiagramView);
                        cpm.paste();
                    }
                }
            }
        }
        /// <summary>
        /// Calls OnModelChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DiagramControl d_Control = d as DiagramControl;
            if (e.NewValue != null && e.NewValue is DiagramModel)
            {
                (e.NewValue as DiagramModel).dc = d_Control;
            }

            if (e.OldValue != null)
            {
                (e.OldValue as DiagramModel).Connections.CollectionChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(d_Control.Connections_CollectionChanged2);
            }
            if (e.NewValue != null)
            {
                (e.NewValue as DiagramModel).Connections.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(d_Control.Connections_CollectionChanged2);
            }
        }

        private void Connections_CollectionChanged2(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (LineConnector lc in e.NewItems.OfType<LineConnector>())
                {
                    lc.setMode(Model);
                    lc.Add();
                }
            }

            if (e.OldItems != null)
            {
                foreach (LineConnector lc in e.OldItems.OfType<LineConnector>())
                {
                    lc.setMode(null);
                    lc.DeleteConnector();
                }
            }
        }

        /// <summary>
        /// Calls OnShowPalleteChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnShowPalleteChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DiagramControl dcontrol = d as DiagramControl;
            if (dcontrol.IsSymbolPaletteEnabled)
            {
		        if(dcontrol.SymbolPalette == null)
		        {
                    dcontrol.ConstructPalette();
		        }
                dcontrol.SymbolPalette.Visibility = Visibility.Visible;
            }
            else
            {
                if (dcontrol.SymbolPalette != null)
                {
                    dcontrol.SymbolPalette.Visibility = Visibility.Collapsed;
                }
            }

            dcontrol.IsSymbolPaletteVisibilityChanged = true;
            
        }

        /// <summary>
        /// Calls OnViewChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnViewChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                DiagramControl dcontrol = d as DiagramControl;
                DiagramView view = dcontrol.View;
            }
        }

        /// <summary>
        /// Calls SymbolPalette_DoubleClick method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="sender"> object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private void SymbolPalette_DoubleClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Click");
        }

        /// <summary>
        /// Determines whether this instance [command] the specified parameter.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>
        /// 	<c>true</c> if this instance [command] the specified parameter; 
        /// 	otherwise, <c>false</c>.
        /// </returns>
        private bool CanExecute(object parameter)
        {
            return true;
        }

        #region Class Variables

        #endregion

        #region Initialization

        #endregion

        #region Implementation

        #endregion

        #region Class Properties

        #endregion

        #region Dependency Properties

        #endregion

        #region Events

        #endregion
    }

}

