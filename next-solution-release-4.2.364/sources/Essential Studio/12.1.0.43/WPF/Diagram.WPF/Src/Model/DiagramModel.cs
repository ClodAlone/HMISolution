// <copyright file="DiagramModel.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using System.Windows.Media;
using Syncfusion.Licensing;
using Syncfusion.Windows.Diagram;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.Windows.Data;

namespace Syncfusion.Windows.Diagram
{
    /// <summary>
    /// Represents the Diagram Model.
    /// </summary>
    /// <remarks>
    /// <para>A model represents data for an application and contains the logic for adding, accessing, and manipulating the data.
    /// Nodes and connectors are added to the Diagram Control using the Model property.
    /// A predefined layout can be applied using the LayoutType property of DiagramModel or the position of the  nodes can be  manually specified.</para>
    /// </remarks>
    /// <example>
    /// <para/>The following example shows how to create a <see cref="DiagramModel"/> in XAML.
    /// <code language="XAML">
    /// &lt;Window x:Class="Sample.Window1"
    /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    /// xmlns:syncfusion="http://schemas.syncfusion.com/wpf"
    ///  WindowState="Maximized" Name="mainwindow" 
    ///  xmlns:local="clr-namespace:Sample" FontWeight="Bold"
    ///  Icon="Images/App.ico" &gt;
    ///   &lt;syncfusion:DiagramControl Grid.Column="1" Name="diagramControl" 
    ///                                IsSymbolPaletteEnabled="True" 
    ///                                Background="WhiteSmoke"&gt;
    ///             &lt;syncfusion:DiagramControl.Model&gt;
    ///                  &lt;syncfusion:DiagramModel LayoutType="None"  x:Name="diagramModel" &gt;
    ///                  &lt;/syncfusion:DiagramModel&gt;
    ///             &lt;/syncfusion:DiagramControl.Model&gt;
    ///           &lt;syncfusion:DiagramControl.View&gt;
    ///              &lt;syncfusion:DiagramView   Background="LightGray"  
    ///                                        Bounds="0,0,12,12"  
    ///                                        Name="diagramView"  &gt;
    ///              &lt;/syncfusion:DiagramView&gt;
    ///          &lt;/syncfusion:DiagramControl.View&gt;
    ///     &lt;/syncfusion:DiagramControl&gt;
    /// &lt;/Window&gt;
    /// </code>
    /// <para/>The following example shows how to create a <see cref="DiagramModel"/> in C#.
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
    /// namespace WpfApplication1
    /// {
    /// public partial class Window1 : Window
    /// {
    ///    public DiagramControl Control;
    ///    public DiagramModel Model;
    ///    public DiagramView View;
    ///    public Window1 ()
    ///    {
    ///       InitializeComponent ();
    ///       Control = new DiagramControl ();
    ///       Model = new DiagramModel ();
    ///       View = new DiagramView ();
    ///       Control.View = View;
    ///       Control.Model = Model;
    ///       View.Bounds = new Thickness(0, 0, 1000, 1000);
    ///        Node n = new Node(Guid.NewGuid(), "Start");
    ///        n.Shape = Shapes.FlowChart_Start;
    ///        n.IsLabelEditable = true;
    ///        n.Label = "Start";
    ///        n.Level = 1;
    ///        n.OffsetX = 150;
    ///        n.OffsetY = 25;
    ///        n.Width = 150;
    ///        n.Height = 75;
    ///         Node n1 = new Node(Guid.NewGuid(), "Decision1");
    ///         n1.Shape = Shapes.FlowChart_Process;
    ///         n1.IsLabelEditable = true;
    ///         n1.Label = "Alarm Rings";
    ///         n1.Level = 2;
    ///         n1.OffsetX = 150;
    ///         n1.OffsetY = 125;
    ///         n1.Width = 150;
    ///         n1.Height = 75;
    ///        Model.Nodes.Add(n);
    ///        Model.Nodes.Add(n1);
    ///        LineConnector o = new LineConnector();
    ///        o.ConnectorType = ConnectorType.Straight;
    ///        o.TailNode = n1;
    ///        o.HeadNode = n;
    ///        o.HeadDecoratorShape = DecoratorShape.None;
    ///        Model.Connections.Add(o);
    ///    }
    ///    }
    ///    }
    /// </code>
    /// </example>
    /// <seealso cref="HierarchicalTreeLayout"/>
    /// <seealso cref="DirectedTreeLayout"/>
    public partial class DiagramModel : Freezable, IDiagramModel, INotifyPropertyChanged
    {
        #region Class variables

        internal bool refresh = true;
        internal bool init = false;
        internal Object tempobj = null;
        internal DiagramControl dc;
        /// <summary>
        /// Used to store the units.
        /// </summary>
        private MeasureUnits m_units;
        
        /// <summary>
        /// Collection of all nodes that are to be managed by the container.
        /// </summary>
        internal CollectionExt m_Nodes = new CollectionExt();

        /// <summary>
        /// Collection of all connections that are to be linked between nodes.
        /// </summary>
        private readonly CollectionExt m_Connection = new CollectionExt();

        /// <summary>
        /// List of the elements that are treated as logical children of control.
        /// </summary>
        private readonly List<UIElement> m_LogicalChildren = new List<UIElement>();

        /// <summary>
        /// Maintaining Root node list from node collection
        /// </summary>
        private CollectionExt m_rootNodes = new CollectionExt();
        
        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="DiagramModel"/> class.
        /// </summary>
        public DiagramModel()
        {
            //m_Children.CollectionChanged += OnChildrenCollectionChanged;
            Nodes.CollectionChanged += new NotifyCollectionChangedEventHandler(InternalChildren_CollectionChanged);
            if (Layers == null)
            {
                Layers = new ObservableCollection<Layer>();
            }
            Layers.CollectionChanged -= new NotifyCollectionChangedEventHandler(Layers_CollectionChanged);
            Layers.CollectionChanged += new NotifyCollectionChangedEventHandler(Layers_CollectionChanged);
        }

        /// <summary>
        /// Handles the CollectionChanged event of the m_internalChildren control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void InternalChildren_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
        }

        #endregion

        #region Class Properties

        /// <summary>
        /// Gets or sets the Measurement unit property.
        /// <value>
        /// Type: <see cref="MeasureUnits"/>
        /// Enum specifying the unit to be used.
        /// </value>
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal MeasureUnits MeasurementUnits
        {
            get
            {
                return m_units;
            }

            set
            {
                if (m_units != value)
                {
                    m_units = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the root nodes.
        /// </summary>
        /// <value>The root nodes.</value>
        internal CollectionExt RootNodes
        {
            get
            {
                return m_rootNodes;
            }

            set
            {
                m_rootNodes = value;
            }
        }

        /// <summary>
        /// Gets or sets the source for the list of the items, the containers about to represent.
        /// </summary>
        /// <remarks>
        /// Items source can not be set while the Items collection contains manually added items.
        /// </remarks>
        /// <example>
        /// <para/>The following example shows how to create a <see cref="DiagramModel"/> in XAML.
        /// <code language="XAML">
        /// &lt;Window x:Class="Sample.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:syncfusion="http://schemas.syncfusion.com/wpf"
        ///  WindowState="Maximized" Name="mainwindow" 
        ///  xmlns:local="clr-namespace:Sample" FontWeight="Bold"
        ///  Icon="Images/App.ico" &gt;
        ///  &lt;Window.Resources&gt;
        ///    &lt;!--The business object which contains the data to be binded.--&gt;
        ///    &lt;local:CountrySalesList x:Key="myList"/&gt;
        ///    &lt;!--Creating a hierarchical data from the XML data for generating a tree view--&gt;
        ///    &lt;HierarchicalDataTemplate x:Key="dataTemplate" ItemsSource="{Binding Path=RegionSales}" DataType="{x:Type local:CountrySale}"&gt;
        ///        &lt;HierarchicalDataTemplate.ItemTemplate&gt;
        ///           &lt;HierarchicalDataTemplate ItemsSource="{Binding Path=Earnings}"   DataType="{x:Type local:RegionSale}"&gt;
        ///               &lt;TextBlock Text="{Binding Path=Name}" /&gt;
        ///              &lt;HierarchicalDataTemplate.ItemTemplate&gt;
        ///                   &lt;DataTemplate DataType="{x:Type local:Sale}"&gt;
        ///                       &lt;TextBlock Text="{Binding Path=Name}" /&gt;
        ///                  &lt;/DataTemplate&gt;
        ///              &lt;/HierarchicalDataTemplate.ItemTemplate&gt;
        ///           &lt;/HierarchicalDataTemplate&gt;
        ///       &lt;/HierarchicalDataTemplate.ItemTemplate&gt;
        ///   &lt;/HierarchicalDataTemplate&gt;
        ///  &lt;/Window.Resources>
        ///   &lt;syncfusion:DiagramControl Grid.Column="1" Name="diagramControl" 
        ///                                IsSymbolPaletteEnabled="True" 
        ///                                Background="WhiteSmoke"&gt;
        ///             &lt;syncfusion:DiagramControl.Model&gt;
        ///                  &lt;syncfusion:DiagramModel LayoutType="None" "  ItemsSource="{Binding Source={StaticResource myList}}" ItemTemplate="{StaticResource dataTemplate}"  x:Name="diagramModel" &gt;
        ///                  &lt;/syncfusion:DiagramModel&gt;
        ///             &lt;/syncfusion:DiagramControl.Model&gt;
        ///           &lt;syncfusion:DiagramControl.View&gt;
        ///              &lt;syncfusion:DiagramView   Background="LightGray"  
        ///                                        Bounds="0,0,12,12"  
        ///                                        Name="diagramView"  &gt;
        ///              &lt;/syncfusion:DiagramView&gt;
        ///          &lt;/syncfusion:DiagramControl.View&gt;
        ///     &lt;/syncfusion:DiagramControl&gt;
        /// &lt;/Window&gt;
        /// </code>
        /// <para/>The following example shows how to create a <see cref="DiagramModel"/> in C#.
        /// <code language="C#">
        /// using Syncfusion.Core;
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
        ///       InitializeComponent ();
        ///       Control = new DiagramControl ();
        ///       Model = new DiagramModel ();
        ///       View = new DiagramView ();
        ///       Control.View = View;
        ///       Control.Model = Model;
        ///       View.Bounds = new Thickness(0, 0, 1000, 1000);
        ///        CountrySalesList list= this.Resources["myList"] as CountrySalesList;
        ///        Model.ItemsSource=list;
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        /// <seealso cref="HierarchicalTreeLayout"/>
        /// <seealso cref="DirectedTreeLayout"/>
        public IEnumerable ItemsSource
        {
            get
            {
                return (IEnumerable)GetValue(ItemsSourceProperty);
            }

            set
            {
                SetValue(ItemsSourceProperty, value);
            }
        }

        internal void PrepareItems()
        {
                if (dc != null && dc.IsLoaded && dc.View != null && this.ItemsSource != null)
                {
                    ParseItemSourceIntoParser(dc);
                }
                if (this.popup != null)
                {
                    object obj = this.getNullObject(this.dc);
                    this.popup.PlacementTarget = obj as UIElement;
                }
        }

        /// <summary>
        /// Gets or sets the HierarchicalDataTemplate for items.
        /// </summary>
        /// <example>
        /// <para/>The following example shows how to create a <see cref="DiagramModel"/> in XAML.
        /// <code language="XAML">
        /// &lt;Window x:Class="Sample.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:syncfusion="http://schemas.syncfusion.com/wpf"
        ///  WindowState="Maximized" Name="mainwindow" 
        ///  xmlns:local="clr-namespace:Sample" FontWeight="Bold"
        ///  Icon="Images/App.ico" &gt;
        ///  &lt;Window.Resources&gt;
        ///    &lt;!--The business object which contains the data to be binded.--&gt;
        ///    &lt;local:CountrySalesList x:Key="myList"/&gt;
        ///    &lt;!--Creating a hierarchical data from the XML data for generating a tree view--&gt;
        ///    &lt;HierarchicalDataTemplate x:Key="dataTemplate" ItemsSource="{Binding Path=RegionSales}" DataType="{x:Type local:CountrySale}"&gt;
        ///        &lt;HierarchicalDataTemplate.ItemTemplate&gt;
        ///           &lt;HierarchicalDataTemplate ItemsSource="{Binding Path=Earnings}"   DataType="{x:Type local:RegionSale}"&gt;
        ///               &lt;TextBlock Text="{Binding Path=Name}" /&gt;
        ///              &lt;HierarchicalDataTemplate.ItemTemplate&gt;
        ///                   &lt;DataTemplate DataType="{x:Type local:Sale}"&gt;
        ///                       &lt;TextBlock Text="{Binding Path=Name}" /&gt;
        ///                  &lt;/DataTemplate&gt;
        ///              &lt;/HierarchicalDataTemplate.ItemTemplate&gt;
        ///           &lt;/HierarchicalDataTemplate&gt;
        ///       &lt;/HierarchicalDataTemplate.ItemTemplate&gt;
        ///   &lt;/HierarchicalDataTemplate&gt;
        ///  &lt;/Window.Resources>
        ///   &lt;syncfusion:DiagramControl Grid.Column="1" Name="diagramControl" 
        ///                                IsSymbolPaletteEnabled="True" 
        ///                                Background="WhiteSmoke"&gt;
        ///             &lt;syncfusion:DiagramControl.Model&gt;
        ///                  &lt;syncfusion:DiagramModel LayoutType="None" "  ItemsSource="{Binding Source={StaticResource myList}}" HierarchicalDataTemplate="{StaticResource dataTemplate}"  x:Name="diagramModel" &gt;
        ///                  &lt;/syncfusion:DiagramModel&gt;
        ///             &lt;/syncfusion:DiagramControl.Model&gt;
        ///           &lt;syncfusion:DiagramControl.View&gt;
        ///              &lt;syncfusion:DiagramView   Background="LightGray"  
        ///                                        Bounds="0,0,12,12"  
        ///                                        Name="diagramView"  &gt;
        ///              &lt;/syncfusion:DiagramView&gt;
        ///          &lt;/syncfusion:DiagramControl.View&gt;
        ///     &lt;/syncfusion:DiagramControl&gt;
        /// &lt;/Window&gt;
        /// </code>
        /// <para/>The following example shows how to create a <see cref="DiagramModel"/> in C#.
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
        ///       InitializeComponent ();
        ///       Control = new DiagramControl ();
        ///       Model = new DiagramModel ();
        ///       View = new DiagramView ();
        ///       Control.View = View;
        ///       Control.Model = Model;
        ///       View.Bounds = new Thickness(0, 0, 1000, 1000);
        ///        CountrySalesList list= this.Resources["myList"] as CountrySalesList;
        ///        Model.ItemsSource=list;
        ///        Model.HierarchicalDataTemplate=this.Resources["dataTemplate"] as DataTemplate;
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        /// <seealso cref="HierarchicalTreeLayout"/>
        /// <seealso cref="DirectedTreeLayout"/>
        public HierarchicalDataTemplate HierarchicalDataTemplate
        {
            get
            {
                return (HierarchicalDataTemplate)GetValue(HierarchicalDataTemplateProperty);
            }

            set
            {
                SetValue(HierarchicalDataTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the ItemTemplate for items.
        /// </summary>
        /// <example>
        /// <para/>The following example shows how to create a <see cref="DiagramModel"/> in XAML.
        /// <code language="XAML">
        /// &lt;Window x:Class="Sample.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:syncfusion="http://schemas.syncfusion.com/wpf"
        ///  WindowState="Maximized" Name="mainwindow" 
        ///  xmlns:local="clr-namespace:Sample" FontWeight="Bold"
        ///  Icon="Images/App.ico" &gt;
        ///  &lt;Window.Resources&gt;
        ///    &lt;!--The business object which contains the data to be binded . --&gt;
        ///    &lt;local:CountrySalesList x:Key="myList"/&gt;
        ///    &lt;!--Creating a hierarchical data from the XML data for generating a tree view--&gt;
        ///    &lt;HierarchicalDataTemplate x:Key="dataTemplate" ItemsSource="{Binding Path=RegionSales}" DataType="{x:Type local:CountrySale}"&gt;
        ///        &lt;HierarchicalDataTemplate.ItemTemplate&gt;
        ///           &lt;HierarchicalDataTemplate ItemsSource="{Binding Path=Earnings}"   DataType="{x:Type local:RegionSale}"&gt;
        ///               &lt;TextBlock Text="{Binding Path=Name}" /&gt;
        ///              &lt;HierarchicalDataTemplate.ItemTemplate&gt;
        ///                   &lt;DataTemplate DataType="{x:Type local:Sale}"&gt;
        ///                       &lt;TextBlock Text="{Binding Path=Name}" /&gt;
        ///                  &lt;/DataTemplate&gt;
        ///              &lt;/HierarchicalDataTemplate.ItemTemplate&gt;
        ///           &lt;/HierarchicalDataTemplate&gt;
        ///       &lt;/HierarchicalDataTemplate.ItemTemplate&gt;
        ///   &lt;/HierarchicalDataTemplate&gt;
        ///  &lt;/Window.Resources>
        ///   &lt;syncfusion:DiagramControl Grid.Column="1" Name="diagramControl" 
        ///                                IsSymbolPaletteEnabled="True" 
        ///                                Background="WhiteSmoke"&gt;
        ///             &lt;syncfusion:DiagramControl.Model&gt;
        ///                  &lt;syncfusion:DiagramModel LayoutType="None" "  ItemsSource="{Binding Source={StaticResource myList}}" ItemTemplate="{StaticResource dataTemplate}"  x:Name="diagramModel" &gt;
        ///                  &lt;/syncfusion:DiagramModel&gt;
        ///             &lt;/syncfusion:DiagramControl.Model&gt;
        ///           &lt;syncfusion:DiagramControl.View&gt;
        ///              &lt;syncfusion:DiagramView   Background="LightGray"  
        ///                                        Bounds="0,0,12,12"  
        ///                                        Name="diagramView"  &gt;
        ///              &lt;/syncfusion:DiagramView&gt;
        ///          &lt;/syncfusion:DiagramControl.View&gt;
        ///     &lt;/syncfusion:DiagramControl&gt;
        /// &lt;/Window&gt;
        /// </code>
        /// <para/>The following example shows how to create a <see cref="DiagramModel"/> in C#.
        /// <code language="C#">
        /// using Syncfusion.Core;
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
        ///       InitializeComponent ();
        ///       Control = new DiagramControl ();
        ///       Model = new DiagramModel ();
        ///       View = new DiagramView ();
        ///       Control.View = View;
        ///       Control.Model = Model;
        ///       View.Bounds = new Thickness(0, 0, 1000, 1000);
        ///        CountrySalesList list= this.Resources["myList"] as CountrySalesList;
        ///        Model.ItemsSource=list;
        ///        Model.ItemTemplate=this.Resources["dataTemplate"] as DataTemplate;
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        /// <seealso cref="HierarchicalTreeLayout"/>
        /// <seealso cref="DirectedTreeLayout"/>
        public DataTemplate ItemTemplate
        {
            get
            {
                return (DataTemplate)GetValue(ItemTemplateProperty);
            }

            set
            {
                SetValue(ItemTemplateProperty, value);
            }
        }


        #endregion

        #region Dependency Properties

        /// <summary>
        ///  Defines the ItemsSource dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(DiagramModel), new UIPropertyMetadata(OnItemSourceChanged));

        /// <summary>
        ///  Defines the ItemTemplate property.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(DiagramModel), new UIPropertyMetadata(null));

        /// <summary>
        /// Defines the LayoutType property.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty LayoutTypeProperty = DependencyProperty.Register("LayoutType", typeof(LayoutType), typeof(DiagramModel), new UIPropertyMetadata(LayoutType.None));

        



        /// <summary>
        /// Gets or sets LayoutHorizontalAlignment.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public LayoutHorizontalAlignment LayoutHorizontalAlignment
        {
            get
            {
                return (LayoutHorizontalAlignment)GetValue(LayoutHorizontalAlignmentProperty);
            }
            set
            {
                SetValue(LayoutHorizontalAlignmentProperty, value);
            }
        }

        /// <summary>
        /// Identifies the LayoutHorizontalAlignment dependency property.
        /// </summary>
        public static readonly DependencyProperty LayoutHorizontalAlignmentProperty =
            DependencyProperty.Register("LayoutHorizontalAlignment", typeof(LayoutHorizontalAlignment), typeof(DiagramModel), new UIPropertyMetadata(LayoutHorizontalAlignment.Center));

        /// <summary>
        /// Gets or sets LayoutVerticalAlignment.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public LayoutVerticalAlignment LayoutVerticalAlignment
        {
            get
            {
                return (LayoutVerticalAlignment)GetValue(LayoutVerticalAlignmentProperty);
            }

            set
            {
                SetValue(LayoutVerticalAlignmentProperty, value);
            }
        }

        /// <summary>
        /// Identifies the LayoutVerticalAlignment dependency property.
        /// </summary>
        public static readonly DependencyProperty LayoutVerticalAlignmentProperty =
            DependencyProperty.Register("LayoutVerticalAlignment", typeof(LayoutVerticalAlignment), typeof(DiagramModel), new UIPropertyMetadata(LayoutVerticalAlignment.Center));

       
        ///<summary>
        /// Defines the HierarchicalDataTemplate property.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty HierarchicalDataTemplateProperty = DependencyProperty.Register("HierarchicalDataTemplate", typeof(HierarchicalDataTemplate), typeof(DiagramModel), new UIPropertyMetadata(null));

        #endregion

        #region Implementation

        internal void RefreshLayout()
        {
            if (dc != null && dc.View != null)
            {
                (dc.View.Page as DiagramPage).LayoutUpdated -= new EventHandler(DiagramModel_LayoutUpdated);
                (dc.View.Page as DiagramPage).LayoutUpdated += new EventHandler(DiagramModel_LayoutUpdated);
            }
        }

        void DiagramModel_LayoutUpdated(object sender, EventArgs e)
        {
            popup.IsOpen = true;
            popup.IsOpen = false;
            if (dc != null && dc.View != null)
            {
                (dc.View.Page as DiagramPage).LayoutUpdated -= new EventHandler(DiagramModel_LayoutUpdated);
                if (this.LayoutType == LayoutType.HierarchicalTreeLayout)
                {
                    HierarchicalTreeLayout layout = new HierarchicalTreeLayout(this, dc.View);
                    layout.RefreshLayout();
                }
                else if (this.LayoutType == LayoutType.DirectedTreeLayout)
                {
                    DirectedTreeLayout layout = new DirectedTreeLayout(this, dc.View);
                    layout.RefreshLayout();
                }
                else if (this.LayoutType == Diagram.LayoutType.RadialTreeLayout)
                {
                    RadialTreeLayout layout = new RadialTreeLayout(this, dc.View);
                    layout.RefreshLayout();
                }
                else if (this.LayoutType == Diagram.LayoutType.TableLayout)
                {
                    TableLayout layout = new TableLayout(this, dc.View);
                    layout.RefreshLayout();
                }
                else if (this.LayoutType == LayoutType.BowtieLayout)
                {
                    BowtieLayout layout = new BowtieLayout(this, dc.View);
                    layout.RefreshLayout();
                }
            }
        }

        internal ChildDataParser parser;
        internal Grid grid;
        internal Popup popup;
        /// <summary>
        /// Parses the items from the items source.
        /// </summary>
        private void ParseItemSourceIntoParser(UIElement element)
        {          
            init = false;
            refresh = false;
            if (parser == null)
            {
                parser = new ChildDataParser(this);
                grid = new Grid();
                grid.Width = 10;
                grid.Height = 10;
                grid.Children.Add(parser);
                popup = new Popup();               
                popup.Width = 0;
                popup.Height = 0;
                popup.Child = grid;
                parser.PropertyChanged -= new PropertyChangedEventHandler(parser_PropertyChanged);
                parser.PropertyChanged += new PropertyChangedEventHandler(parser_PropertyChanged);

                Binding bin = new Binding("ItemsSource");
                bin.Source = this;
                parser.SetBinding(ChildDataParser.ItemsSourceProperty, bin);

                Binding binn = new Binding("ItemTemplate");
                binn.Source = this;
                parser.SetBinding(ChildDataParser.ItemTemplateProperty, binn);
            }
            if (BrowserInteropHelper.IsBrowserHosted)
            {
                popup.PlacementTarget = dc;
            }
            popup.IsOpen = true;
            popup.IsOpen = false;
            refresh = true;
            init = true;
        }

        void parser_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (refresh && popup!=null)
            {
                refresh = false;
                if (!popup.IsOpen)
                {
                    this.RefreshLayout();
                }
                
            }
            refresh = true;
        }

        public delegate void Initialize();

        /// <summary>
        /// Adds the logical children.
        /// </summary>
        /// <param name="iList">The IList collection.</param>
        private void AddLogicalChildren(IList iList)
        {
        }

        /// <summary>
        /// Converts the ChildDataParser items to nodes.
        /// </summary>
        /// <param name="items">ChildDataParser items.</param>
        /// <returns>The converted nodes</returns>
        private CollectionExt ConvertToNodes(List<ChildDataParser> items,DataTemplate dt)
        {
            CollectionExt nodes = new CollectionExt();
            CollectionExt childs = new CollectionExt();
            foreach (ChildDataParser child in items)
            {
                Node node = new Node();
                node.Content = child.Header;
                if (dt!=null && dt.HasContent)
                {
                    node.ContentTemplate = dt;
                }
                child.node = node;
                node.Model = this;
                Node.setINodeBinding(node, node.Content as INode);
                childs = this.ConvertToNodes(child.ChildItems,child.ItemTemplate);
                nodes.Add(node);
                HookConnection(node, childs);
            }

            return nodes;
        }

        /// <summary>
        /// Hooks a connection between the specified head node and tail node.
        /// </summary>
        /// <param name="parent">Head Node object</param>
        /// <param name="childs">The Tail Nodes </param>
        private void HookConnection(Node parent, CollectionExt childs)
        {
            this.Nodes.Add(parent);
            foreach (Node node in childs)
            {
                LineConnector ortho;
                if (dc != null && dc.View != null)
                {
                    ortho = new LineConnector(dc.View);
                }
                else
                {
                    ortho = new LineConnector(null);
                }
                ortho.HeadNode = parent;
                ortho.TailNode = node;
                ortho.Content = node.Content;
                this.Connections.Add(ortho);
            }
        }
        private Object getNullObject(DependencyObject ele)
        {
            Object obj = LogicalTreeHelper.GetParent(ele as DependencyObject);
            if (obj == null)
            {
                obj = VisualTreeHelper.GetParent(ele as DependencyObject);
                if (obj == null)
                {
                    tempobj = ele;
                }
                else
                {
                    getNullObject(obj as DependencyObject);
                }
            }
            else
            {
                getNullObject(obj as DependencyObject);
            }
            return tempobj as UIElement;
        }
        /// <summary>
        /// Removes the logical children.
        /// </summary>
        /// <param name="iList">The IList collection.</param>
        private void RemoveLogicalChildren(IList iList)
        {
            RemoveLogicalChildren(iList, true);
        }

        /// <summary>
        /// Removes the logical children.
        /// </summary>
        /// <param name="iList">The IList collection.</param>
        /// <param name="detach">if set to <c>true</c> [detach].</param>
        private void RemoveLogicalChildren(IList iList, bool detach)
        {
        }

        /// <summary>
        /// Called when [ItemSource  changed].
        /// </summary>
        /// <param name="d">Dependency object.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private static void OnItemSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DiagramModel d_Model = d as DiagramModel;
            d_Model.PrepareItems();
        }

        /// <summary>
        /// Called when [connections changed].
        /// </summary>
        /// <param name="d">The DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnConnectionsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DiagramModel d_Control = d as DiagramModel;
        }

        /// <summary>
        /// Called when [nodes changed].
        /// </summary>
        /// <param name="d">The DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnNodesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DiagramModel d_Control = d as DiagramModel;
        }

        #endregion

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Calls propertychanged event handler.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raised when the appropriate property changes.
        /// </summary>
        /// <param name="name">The property name.</param>
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion

        protected override Freezable CreateInstanceCore()
        {
            throw new NotImplementedException();
        }
        
#if WPF
        #region WPF Layers and LineBridging
        /// <summary>
        /// Gets or sets a value indicating whether [line bridging enabled].
        /// </summary>
        /// <value><c>true</c> if [line bridging enabled]; otherwise, <c>false</c>.</value>
        [Obsolete("This Property is deprecated, please use DiagramView's LineBridgingEnabled property")]
        public bool LineBridgingEnabled
        {
            get
            {
                return (bool)GetValue(LineBridgingEnabledProperty);
            }

            set
            {
                SetValue(LineBridgingEnabledProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the layers.
        /// </summary>
        /// <value>The layers.</value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public ObservableCollection<Layer> Layers
        {
            get
            {
                return (ObservableCollection<Layer>)GetValue(LayersProperty);
            }
            set
            {
                SetValue(LayersProperty, value);
                Layers.CollectionChanged -= new NotifyCollectionChangedEventHandler(Layers_CollectionChanged);
                Layers.CollectionChanged += new NotifyCollectionChangedEventHandler(Layers_CollectionChanged);
            }
        }

        public static readonly DependencyProperty LayersProperty = DependencyProperty.Register("Layers", typeof(ObservableCollection<Layer>), typeof(DiagramModel), new PropertyMetadata(OnLayersChanged));

        [Obsolete("This Property is deprecated, please use DiagramView's LineBridgingEnableProperty")]
        public static readonly DependencyProperty LineBridgingEnabledProperty = DependencyProperty.Register("LineBridgingEnabled", typeof(bool), typeof(DiagramModel), new PropertyMetadata(true, new PropertyChangedCallback(OnLineBridgingEnabledChanged)));

        private static void OnLineBridgingEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DiagramModel model = (d as DiagramModel);
            if ((bool)e.NewValue == true)
            {
                List<UIElement> ordered = (from UIElement item in model.Connections
                                           orderby Panel.GetZIndex(item as UIElement)
                                           select item as UIElement).ToList();
                foreach (UIElement element in ordered)
                {
                    if (element is LineConnector)
                    {
                        (element as LineConnector).SetLineBridging();
                    }
                }
            }
        }
        private static void OnLayersChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        void Layers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    ProcessAdd(e);
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Remove:
                    ProcessRemove(e);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    ProcessRemove(e);
                    ProcessAdd(e);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    IList l = new List<Layer>();
                    foreach (object o in this.Nodes)
                    {
                        if (o is Layer)
                        {
                            (o as Layer).LayerPropertyChanged -= new LayerPropertyChangedEventHandler(DiagramView_LayerPropertyChanged);
                            l.Add(o);
                        }
                    }
                    foreach (object layer in l)
                    {
                        if (layer is Layer)
                        {
                            this.Nodes.Remove(layer);
                        }
                    }

                    if (dc != null && dc.View != null)
                    {
                        if (!(dc.View.undo || dc.View.redo) && l.Count > 0)
                        {
                            if (dc.View.UndoRedoEnabled)
                            {
                                dc.View.UndoStack.Push(Layers);
                                dc.View.UndoStack.Push(l);
                                dc.View.UndoStack.Push("LayerRemoved");
                            }
                        }
                    }

                    break;
            }
        }

        private void ProcessAdd(NotifyCollectionChangedEventArgs e)
        {
            foreach (object o in e.NewItems)
            {
                if (o is Layer)
                {
                    this.Nodes.Add(o);
                    (o as Layer).LayerPropertyChanged -= new LayerPropertyChangedEventHandler(DiagramView_LayerPropertyChanged);
                    (o as Layer).LayerPropertyChanged += new LayerPropertyChangedEventHandler(DiagramView_LayerPropertyChanged);
                    DiagramModel dm = this;
                    if ((o as Layer).Visible)
                    {
                        dm.ShowLayer(o as Layer);
                    }
                    else
                    {
                        dm.HideLayer(o as Layer);
                    }
                }
            }
            if (dc != null && dc.View != null)
            {
                if (!(dc.View.undo || dc.View.redo))
                {
                    if (dc.View.UndoRedoEnabled)
                    {
                        dc.View.UndoStack.Push(Layers);
                        dc.View.UndoStack.Push(toOCL(e.NewItems));
                        dc.View.UndoStack.Push("LayerAdded");
                    }
                }
            }
        }

        private void ProcessRemove(NotifyCollectionChangedEventArgs e)
        {
            foreach (object o in e.OldItems)
            {
                if (o is Layer)
                {
                    this.Nodes.Remove(o);
                    (o as Layer).LayerPropertyChanged -= new LayerPropertyChangedEventHandler(DiagramView_LayerPropertyChanged);
                }

                if (dc != null && dc.View != null)
                {
                    if (!(dc.View.undo || dc.View.redo))
                    {
                        if (dc.View.UndoRedoEnabled)
                        {
                            dc.View.UndoStack.Push(Layers);
                            dc.View.UndoStack.Push(e.OldItems);
                            dc.View.UndoStack.Push("LayerRemoved");
                        }
                    }
                }
            }
        }

        internal ObservableCollection<Layer> toOCL(IList a)
        {
            ObservableCollection<Layer> l;
            object[] o = a.SyncRoot as object[];
            IEnumerable<Layer> n = o.Cast<Layer>();
            l = new ObservableCollection<Layer>(n);
            return l;
        }
        internal void HideLayer(Layer lay)
        {
            lay.Active = false;
            if (lay.Nodes != null)
            {
                foreach (Node n in lay.Nodes)
                {
                    n.Visibility = Visibility.Hidden;
                }
            }
            if (lay.Lines != null)
            {
                foreach (LineConnector l in lay.Lines)
                {
                    l.Visibility = Visibility.Hidden;
                }
            }
        }

        internal void ShowLayer(Layer lay)
        {
            if (lay.Nodes != null)
            {
                foreach (Node n in lay.Nodes)
                {
                    n.Visibility = Visibility.Visible;
                    foreach (Layer la in Layers)
                    {
                        if (!la.Visible && la.Nodes.Contains(n))
                        {
                            n.Visibility = Visibility.Hidden;
                        }
                    }
                }
            }
            if (lay.Lines != null)
            {
                foreach (LineConnector l in lay.Lines)
                {
                    l.Visibility = Visibility.Visible;
                    foreach (Layer la in Layers)
                    {
                        if (!la.Visible && la.Lines.Contains(l))
                        {
                            l.Visibility = Visibility.Hidden;
                        }
                    }
                }
            }
        }

        internal void RefreshNodeVisibility(Node n)
        {
            n.Visibility = Visibility.Visible;
            foreach (Layer la in Layers)
            {
                if (!la.Visible && la.Nodes.Contains(n))
                {
                    n.Visibility = Visibility.Hidden;
                }
            }
        }

        internal void RefreshNodeVisibility(LineConnector l)
        {
            l.Visibility = Visibility.Visible;
            foreach (Layer la in Layers)
            {
                if (!la.Visible && la.Lines.Contains(l))
                {
                    l.Visibility = Visibility.Hidden;
                }
            }
        }

        void DiagramView_LayerPropertyChanged(object sender, LayerPropertyChangedEventArgs evtArgs)
        {
            DiagramModel dm = this;// (sender as Layer).Parent as DiagramView;
            DiagramView dv = dc.View;
            Layer l = sender as Layer;
            if (!(dv.undo || dv.redo))
            {
                dv.RedoStack.Clear();
                if (dv.UndoRedoEnabled)
                {
                    dv.UndoStack.Push(l);
                    switch (evtArgs.PropertyName)
                    {
                        case "":
                            dv.UndoStack.Pop();
                            return;
                        case "Active":
                            dv.UndoStack.Pop();
                            return;
                        case "Visible":
                            dv.UndoStack.Push(evtArgs.PreState);
                            break;
                        case "LineRA":
                            break;
                        case "NodeRA":
                            break;
                        case "NodeAdded":
                        case "NodeRemoved":
                            dv.UndoStack.Push(evtArgs.Nodes);
                            break;
                        case "LineAdded":
                        case "LineRemoved":
                            dv.UndoStack.Push(evtArgs.Lines);
                            break;
                    }
                    dv.UndoStack.Push(evtArgs.PropertyName);
                }
            }
            switch (evtArgs.PropertyName)
            {
                case "Visible":
                    if (l.Visible)
                    {
                        dm.ShowLayer(l);
                    }
                    else
                    {
                        dm.HideLayer(l);
                    }
                    break;

                case "NodeAdded":
                    if (!l.Visible)
                    {
                        if (evtArgs.Nodes != null)
                        {
                            foreach (Node n in evtArgs.Nodes)
                            {
                                n.Visibility = Visibility.Hidden;
                            }
                        }
                    }
                    break;
                case "NodeRemoved":
                    if (evtArgs.Nodes != null)
                    {
                        foreach (Node n in evtArgs.Nodes)
                        {
                            RefreshNodeVisibility(n);
                        }
                    }
                    break;

                case "LineAdded":
                    if (!l.Visible)
                    {
                        if (evtArgs.Lines != null)
                        {
                            foreach (LineConnector li in evtArgs.Lines)
                            {
                                li.Visibility = Visibility.Hidden;
                            }
                        }
                    }
                    break;
                case "LineRemoved":
                    if (evtArgs.Lines != null)
                    {
                        foreach (LineConnector li in evtArgs.Lines)
                        {
                            RefreshNodeVisibility(li);
                        }
                    }
                    break;
            }
        }
#endregion
#endif

    }
}
