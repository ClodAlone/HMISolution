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
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Windows;

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
    ///namespace SilverlightApplication1
    /// {
    /// public partial class MainPage : UserControl
    /// {
    ///    public DiagramControl Control;
    ///    public DiagramModel Model;
    ///    public DiagramView View;
    ///    public MainPage()
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
    public partial class DiagramModel : ITree, IGraph, IModel
    {
        /// <summary>
        /// Identifies the ColumnCount dependency property.
        /// </summary>
        public static readonly DependencyProperty ColumnCountProperty = DependencyProperty.Register("ColumnCount", typeof(int), typeof(DiagramModel), new PropertyMetadata(0));

        /// <summary>
        /// Identifies the EnableCycleDetection dependency property.
        /// </summary>
        public static readonly DependencyProperty EnableCycleDetectionProperty = DependencyProperty.Register("EnableCycleDetection", typeof(bool), typeof(DiagramModel), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the EnableLayoutWithVariedSizes dependency property.
        /// </summary>
        public static readonly DependencyProperty EnableLayoutWithVariedSizesProperty = DependencyProperty.Register("EnableLayoutWithVariedSizes", typeof(bool), typeof(DiagramModel), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the RowCount dependency property.
        /// </summary>
        public static readonly DependencyProperty RowCountProperty = DependencyProperty.Register("RowCount", typeof(int), typeof(DiagramModel), new PropertyMetadata(0));

        /// <summary>
        /// Identifies the TableExpandMode dependency property.
        /// </summary>
        public static readonly DependencyProperty TableExpandModeProperty = DependencyProperty.Register("TableExpandMode", typeof(ExpandMode), typeof(DiagramModel), new PropertyMetadata(ExpandMode.Horizontal));



        /// <summary>
        /// Defines the OrderedNodes property.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty OrderedNodesProperty = DependencyProperty.Register("OrderedNodes", typeof(IEnumerable<IShape>), typeof(DiagramModel),new PropertyMetadata(null));

        public IEnumerable<IShape> OrderedNodes
        {
            get
            {
                return (IEnumerable<IShape>)GetValue(OrderedNodesProperty);
            }

            set
            {
                SetValue(OrderedNodesProperty, value);
            }
        }

        /// <summary>
        /// Used to store the orientation value.
        /// </summary>
        private TreeOrientation morientation = TreeOrientation.TopBottom;

        ///// <summary>
        ///// Used to store IsDefaultHorizontal value
        ///// </summary>
        //private bool isdefaulthor = true;

        ///// <summary>
        ///// Used to store IsDefaultSubTree value
        ///// </summary>
        //private bool isdefaultsubtree = true;

        ///// <summary>
        ///// Used to store IsDefaultVertical value
        ///// </summary>
        //private bool isdefaultver = true;

        /// <summary>
        ///  Used to store the horizontal spacing.
        /// </summary>
        private double mhorizontalSpacing = 50;

        /// <summary>
        /// Used to store IsDirected property value.
        /// </summary>
        private bool misDirected;

        /// <summary>
        ///  Used to store the layout root.
        /// </summary>
        private IShape mlayoutRoot;

        /// <summary>
        ///  Used to store the tree.
        /// </summary>
        private ITree mspanningTree;

        /// <summary>
        ///  Used to store the sub tree spacing.
        /// </summary>
        private double msubTreeSpacing = 150;

        /// <summary>
        ///  Used to store the vertical spacing.
        /// </summary>
        private double mverticalSpacing = 50;

        internal TreeOrientation BowtieOrientation;

        /// <summary>
        /// Gets or sets the Column Count for the table layout.
        /// </summary>
        /// <remarks>
        /// ColumnCount is automatically set when TableExpandMode is set to Vertical.
        /// </remarks>
        /// <example>
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
        ///                  &lt;syncfusion:DiagramModel LayoutType="TableLayout" RowCount="3" ColumnCount="3" HorizontalSpacing="50" VerticalSpacing="50" x:Name="diagramModel" &gt;
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
        /// <code language="C#">
        /// using Syncfusion.Core;
        /// using Syncfusion.Windows.Diagram;
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        ///    public DiagramControl Control;
        ///    public DiagramModel Model;
        ///    public DiagramView View;
        ///    public MainPage()
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
        ///        n.Width = 150;
        ///        n.Height = 75;
        ///         Node n1 = new Node(Guid.NewGuid(), "Decision1");
        ///         n1.Shape = Shapes.FlowChart_Process;
        ///         n1.IsLabelEditable = true;
        ///         n1.Label = "Alarm Rings";
        ///         n1.Level = 2;
        ///         n1.Width = 150;
        ///         n1.Height = 75;
        ///        Model.Nodes.Add(n);
        ///        Model.Nodes.Add(n1);
        ///        Model.LayoutType = LayoutType.TableLayout;
        ///        Model.RowCount = 3;
        ///        Model.ColumnCount = 3;
        ///        Model.SpaceBetweenSubTrees = 50;
        ///        Model.LayoutRoot=n;
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
        public int ColumnCount
        {
            get
            {
                return (int)GetValue(ColumnCountProperty);
            }

            set
            {
                SetValue(ColumnCountProperty, value);
            }
        }

        /// <summary>
        /// Gets the connections.
        /// </summary>
        /// <value>The connections.</value>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.Core;
        /// using Syncfusion.Windows.Diagram;
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        ///    public DiagramControl Control;
        ///    public DiagramModel Model;
        ///    public DiagramView View;
        ///    public MainPage()
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
        public CollectionExt Connections
        {
            get
            {
                return this.m_Connection;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Cycle detection is enabled or not.
        /// </summary>
        /// <value>
        /// <c>true</c> if cycle detection is enabled; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// In case of <see cref="HierarchicalTreeLayout"></see> , if there exists a cycle in the input , then EnableCycleDetection property should be set to true. 
        /// A cycle is said to exist when for example: say three nodes n1,n2,n3 are there such that, n1 is connected to n2, n2 is connected to n3 and n3 is again connected to n1.(n1-->n2-->n3-->n1) n1,n2,n3 are said to form a cycle then.
        /// </remarks>
        /// <example>
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
        ///                  &lt;syncfusion:DiagramModel EnableCycleDetection="True" Orientation="TopBottom" LayoutType="HierarchicalTreeLayout"  x:Name="diagramModel" &gt;
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
        /// <code language="C#">
        /// using Syncfusion.Core;
        /// using Syncfusion.Windows.Diagram;
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        ///    public DiagramControl Control;
        ///    public DiagramModel Model;
        ///    public DiagramView View;
        ///    public MainPage()
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
        ///        n.Width = 150;
        ///        n.Height = 75;
        ///         Node n1 = new Node(Guid.NewGuid(), "Decision1");
        ///         n1.Shape = Shapes.FlowChart_Process;
        ///         n1.IsLabelEditable = true;
        ///         n1.Label = "Alarm Rings";
        ///         n1.Level = 2;
        ///         n1.Width = 150;
        ///         n1.Height = 75;
        ///        Model.Nodes.Add(n);
        ///        Model.Nodes.Add(n1);
        ///        Model.LayoutType = LayoutType.HierarchicalTreeLayout;
        ///        Model.Orientation = TreeOrientation.TopBottom;
        ///        Model.EnableCycleDetection=true;
        ///        Model.HorizontalSpacing = 30;
        ///        Model.VerticalSpacing = 50;
        ///        Model.SpaceBetweenSubTrees = 50;
        ///        LineConnector o = new LineConnector();
        ///        o.ConnectorType = ConnectorType.Straight;
        ///        o.TailNode = n1;
        ///        o.HeadNode = n;
        ///        o.HeadDecoratorShape = DecoratorShape.None;
        ///        Model.Connections.Add(o);
        ///        LineConnector o1 = new LineConnector();
        ///        o1.ConnectorType = ConnectorType.Straight;
        ///        o.TailNode = n;
        ///        o.HeadNode = n1;
        ///        o.HeadDecoratorShape = DecoratorShape.None;
        ///        Model.Connections.Add(o1);        
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        public bool EnableCycleDetection
        {
            get
            {
                return (bool)GetValue(EnableCycleDetectionProperty);
            }

            set
            {
                SetValue(EnableCycleDetectionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to enable the varied size algorithm. In case the Model consists of the nodes of different sizes, this property can be set to true. This will align the differently sized nodes with respect to the centre.
        /// </summary>
        /// <value>
        /// <c>true</c> if [enable layout with varied sizes]; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// Default value is false. 
        /// </remarks>
        /// <example>
        /// <code language="XAML">
        /// &lt;Window x:Class="Sample.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:syncfusion="http://schemas.syncfusion.com/wpf"
        /// WindowState="Maximized" Name="mainwindow"
        /// xmlns:local="clr-namespace:Sample" FontWeight="Bold"
        /// Icon="Images/App.ico" &gt;
        /// &lt;syncfusion:DiagramControl Grid.Column="1" Name="diagramControl"
        /// IsSymbolPaletteEnabled="True"
        /// Background="WhiteSmoke"&gt;
        /// &lt;syncfusion:DiagramControl.Model&gt;
        /// &lt;syncfusion:DiagramModel LayoutType="TableLayout" TableExpandMode="Vertical" RowCount="3" ColumnCount="3" HorizontalSpacing="50" VerticalSpacing="50" x:Name="diagramModel" &gt;
        /// &lt;/syncfusion:DiagramModel&gt;
        /// &lt;/syncfusion:DiagramControl.Model&gt;
        /// &lt;syncfusion:DiagramControl.View&gt;
        /// &lt;syncfusion:DiagramView   Background="LightGray"
        /// Bounds="0,0,12,12"
        /// Name="diagramView"  &gt;
        /// &lt;/syncfusion:DiagramView&gt;
        /// &lt;/syncfusion:DiagramControl.View&gt;
        /// &lt;/syncfusion:DiagramControl&gt;
        /// &lt;/Window&gt;
        /// </code>
        /// <code language="C#">
        /// using Syncfusion.Core;
        /// using Syncfusion.Windows.Diagram;
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        /// public DiagramControl Control;
        /// public DiagramModel Model;
        /// public DiagramView View;
        /// public MainPage()
        /// {
        /// InitializeComponent ();
        /// Control = new DiagramControl ();
        /// Model = new DiagramModel ();
        /// View = new DiagramView ();
        /// Control.View = View;
        /// Control.Model = Model;
        /// View.Bounds = new Thickness(0, 0, 1000, 1000);
        /// Node n = new Node(Guid.NewGuid(), "Start");
        /// n.Shape = Shapes.FlowChart_Start;
        /// n.IsLabelEditable = true;
        /// n.Label = "Start";
        /// n.Level = 1;
        /// n.Width = 150;
        /// n.Height = 75;
        /// Node n1 = new Node(Guid.NewGuid(), "Decision1");
        /// n1.Shape = Shapes.FlowChart_Process;
        /// n1.IsLabelEditable = true;
        /// n1.Label = "Alarm Rings";
        /// n1.Level = 2;
        /// n1.Width = 150;
        /// n1.Height = 75;
        /// Model.Nodes.Add(n);
        /// Model.Nodes.Add(n1);
        /// Model.LayoutType = LayoutType.TableLayout;
        /// Model.TableExpandMode=ExpandMode.Vertical;
        /// Model.RowCount = 3;
        /// Model.ColumnCount = 3;
        /// Model.LayoutRoot=n;
        /// LineConnector o = new LineConnector();
        /// o.ConnectorType = ConnectorType.Straight;
        /// o.TailNode = n1;
        /// o.HeadNode = n;
        /// o.HeadDecoratorShape = DecoratorShape.None;
        /// Model.Connections.Add(o);
        /// }
        /// }
        /// }
        /// </code>
        /// </example>
        public bool EnableLayoutWithVariedSizes
        {
            get
            {
                return (bool)GetValue(EnableLayoutWithVariedSizesProperty);
            }

            set
            {
                SetValue(EnableLayoutWithVariedSizesProperty, value);
            }
        }

        bool m_selection=false;
        
        internal bool m_IsSelectionBaseLayout
        {
            get
            {
                return m_selection;
            }

            set
            {
                m_selection=value;
            }
        }

        bool _Enablebounds = false;

        public bool EnableCollectionBounds
        {
            get
            {
                return _Enablebounds;
            }

            set
            {
                _Enablebounds = value;
            }
        }

        internal double PxHorizontalSpacing
        {
            get
            {
                return MeasureUnitsConverter.ToPixels(HorizontalSpacing, this.MeasurementUnits);
            }
            //set
            //{
            //    HorizontalSpacing = MeasureUnitsConverter.FromPixels(value, this.MeasurementUnits);
            //}
        }

        /// <summary>
        /// Gets or sets the Horizontal spacing between nodes.
        /// </summary>
        /// <example>
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
        ///                  &lt;syncfusion:DiagramModel LayoutType="DirectedTreeLayout" HorizontalSpacing="30" VerticalSpacing="50" x:Name="diagramModel" &gt;
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
        /// <code language="C#">
        /// using Syncfusion.Core;
        /// using Syncfusion.Windows.Diagram;
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        ///    public DiagramControl Control;
        ///    public DiagramModel Model;
        ///    public DiagramView View;
        ///    public MainPage()
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
        ///        n.Width = 150;
        ///        n.Height = 75;
        ///         Node n1 = new Node(Guid.NewGuid(), "Decision1");
        ///         n1.Shape = Shapes.FlowChart_Process;
        ///         n1.IsLabelEditable = true;
        ///         n1.Label = "Alarm Rings";
        ///         n1.Level = 2;
        ///         n1.Width = 150;
        ///         n1.Height = 75;
        ///        Model.Nodes.Add(n);
        ///        Model.Nodes.Add(n1);
        ///        Model.LayoutType = LayoutType.DirectedTreeLayout;
        ///        Model.HorizontalSpacing = 30;
        ///        Model.VerticalSpacing = 50;
        ///        Model.SpaceBetweenSubTrees = 50;
        ///        Model.LayoutRoot=n;
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
        public double HorizontalSpacing
        {
            get
            {
                return this.mhorizontalSpacing;
            }

            set
            {
                this.mhorizontalSpacing = value;
            }
        }

        /// <summary>
        /// Gets the collection of all incident edges, those for which this node
        /// is either the source or the target.
        /// </summary>
        /// <value>The connections</value>
        CollectionExt IGraph.Edges
        {
            get
            {
                return this.Connections;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is directed.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is directed; otherwise, <c>false</c>.
        /// </value>
        bool IGraph.IsDirected
        {
            get { return this.misDirected; }
        }

        /// <summary>
        /// Gets the Spanning tree.
        /// </summary>
        /// <value>The graph .</value>
        ITree IGraph.Tree
        {
            get { return this.mspanningTree; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is directed.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is directed; otherwise, <c>false</c>.
        /// </value>
        bool ITree.IsDirected
        {
            get { return this.misDirected; }
            set { this.misDirected = value; }
        }

        /// <summary>
        /// Gets or sets the root.
        /// </summary>
        /// <value>The root node.</value>
        IShape ITree.Root
        {
            get
            {
                return this.mlayoutRoot as IShape;
            }

            set
            {
                this.mlayoutRoot = value as IShape;
            }
        }

        /// <summary>
        /// Gets or sets the layout root.
        /// </summary>
        /// <value>The layout root.</value>
        /// <remarks>
        /// The Layout Root specifies the starting point of the layout. It has to be specified when the <see cref="DirectedTreeLayout"/> is used.
        /// </remarks>
        /// <example>
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
        ///                  &lt;syncfusion:DiagramModel LayoutType="DirectedTreeLayout"  x:Name="diagramModel" &gt;
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
        /// <code language="C#">
        /// using Syncfusion.Core;
        /// using Syncfusion.Windows.Diagram;
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        ///    public DiagramControl Control;
        ///    public DiagramModel Model;
        ///    public DiagramView View;
        ///    public MainPage()
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
        ///        n.Width = 150;
        ///        n.Height = 75;
        ///         Node n1 = new Node(Guid.NewGuid(), "Decision1");
        ///         n1.Shape = Shapes.FlowChart_Process;
        ///         n1.IsLabelEditable = true;
        ///         n1.Label = "Alarm Rings";
        ///         n1.Level = 2;
        ///         n1.Width = 150;
        ///         n1.Height = 75;
        ///        Model.Nodes.Add(n);
        ///        Model.Nodes.Add(n1);
        ///         Model.LayoutType = LayoutType.DirectedTreeLayout;
        ///        Model.HorizontalSpacing = 30;
        ///        Model.VerticalSpacing = 50;
        ///        Model.SpaceBetweenSubTrees = 50;
        ///        Model.LayoutRoot=n;
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
        public IShape LayoutRoot
        {
            get { return this.mlayoutRoot; }
            set { this.mlayoutRoot = value; }
        }

        /// <summary>
        /// Gets or sets the type of Layout to be used.
        /// </summary>
        /// <value>
        /// Enum specifying the type of layout.
        /// </value>
        /// <remarks>
        /// The LayoutType selects the layout to be used. <see cref="HierarchicalTreeLayout"/>, <see cref="DirectedTreeLayout"/> and None are the supported layout types. Default type is None. 
        /// </remarks>
        /// <example>
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
        ///                  &lt;syncfusion:DiagramModel LayoutType="DirectedTreeLayout"  x:Name="diagramModel" &gt;
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
        /// <code language="C#">
        /// using Syncfusion.Core;
        /// using Syncfusion.Windows.Diagram;
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        ///    public DiagramControl Control;
        ///    public DiagramModel Model;
        ///    public DiagramView View;
        ///    public MainPage()
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
        ///        n.Width = 150;
        ///        n.Height = 75;
        ///         Node n1 = new Node(Guid.NewGuid(), "Decision1");
        ///         n1.Shape = Shapes.FlowChart_Process;
        ///         n1.IsLabelEditable = true;
        ///         n1.Label = "Alarm Rings";
        ///         n1.Level = 2;
        ///         n1.Width = 150;
        ///         n1.Height = 75;
        ///        Model.Nodes.Add(n);
        ///        Model.Nodes.Add(n1);
        ///         Model.LayoutType = LayoutType.DirectedTreeLayout;
        ///        Model.HorizontalSpacing = 30;
        ///        Model.VerticalSpacing = 50;
        ///        Model.SpaceBetweenSubTrees = 50;
        ///        Model.LayoutRoot=n;
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
        public LayoutType LayoutType
        {
            get
            {
                return (LayoutType)GetValue(LayoutTypeProperty);
            }

            set
            {
                SetValue(LayoutTypeProperty, value);
            }
        }

        /// <summary>
        /// Gets the shapes.
        /// </summary>
        /// <value>The shapes.</value>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.Core;
        /// using Syncfusion.Windows.Diagram;
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        ///    public DiagramControl Control;
        ///    public DiagramModel Model;
        ///    public DiagramView View;
        ///    public MainPage()
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
        ///        n.Width = 150;
        ///        n.Height = 75;
        ///         Node n1 = new Node(Guid.NewGuid(), "Decision1");
        ///         n1.Shape = Shapes.FlowChart_Process;
        ///         n1.IsLabelEditable = true;
        ///         n1.Label = "Alarm Rings";
        ///         n1.Level = 2;
        ///         n1.Width = 150;
        ///         n1.Height = 75;
        ///        Model.Nodes.Add(n);
        ///        Model.Nodes.Add(n1);
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        public CollectionExt Nodes
        {
            get
            {
                return this.m_Nodes;
            }
        }

        /// <summary>
        /// Gets or sets the orientation.
        /// </summary>
        /// <value>
        /// Type:<see cref="TreeOrientation"/>
        /// Enum specifying the orientation.
        /// </value>
        /// <remarks>
        /// The Layout Manager lets you orient the tree in many directions and can be used for the creation of many sophisticated arrangements. 
        ///  <para>TopBottom - Places the root node at the top and the child nodes are arranged below the root node.</para>
        ///  <para>BottomTop - Places the root node at the Bottom and the child nodes are arranged above the root node. </para>
        ///  <para>LeftRight - Places the root node at the Left and the child nodes are arranged on the right side of the root node. </para>
        ///  <para>RightLeft - Places the root node at the Right and the child nodes are arranged on the left side of the root node. </para>
        /// Default orientation is TopBottom.
        /// </remarks>
        /// <example>
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
        ///                  &lt;syncfusion:DiagramModel Orientation="TopBottom" LayoutType="DirectedTreeLayout"  x:Name="diagramModel" &gt;
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
        /// <code language="C#">
        /// using Syncfusion.Core;
        /// using Syncfusion.Windows.Diagram;
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        ///    public DiagramControl Control;
        ///    public DiagramModel Model;
        ///    public DiagramView View;
        ///    public MainPage()
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
        ///        n.Width = 150;
        ///        n.Height = 75;
        ///         Node n1 = new Node(Guid.NewGuid(), "Decision1");
        ///         n1.Shape = Shapes.FlowChart_Process;
        ///         n1.IsLabelEditable = true;
        ///         n1.Label = "Alarm Rings";
        ///         n1.Level = 2;
        ///         n1.Width = 150;
        ///         n1.Height = 75;
        ///        Model.Nodes.Add(n);
        ///        Model.Nodes.Add(n1);
        ///        Model.LayoutType = LayoutType.DirectedTreeLayout;
        ///        Model.Orientation = TreeOrientation.TopBottom;
        ///        Model.HorizontalSpacing = 30;
        ///        Model.VerticalSpacing = 50;
        ///        Model.SpaceBetweenSubTrees = 50;
        ///        Model.LayoutRoot=n;
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
        public TreeOrientation Orientation
        {
            get
            {
                return morientation;
            }

            set
            {
                morientation = value;
                this.OnPropertyChanged("Orientation");
            }
        }

        /// <summary>
        /// Gets or sets the Row Count for the table layout.
        /// </summary>
        /// <remarks>
        /// RowCount is automatically set when TableExpandMode is set to Horizontal.
        /// </remarks>
        /// <example>
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
        ///                  &lt;syncfusion:DiagramModel LayoutType="TableLayout" RowCount="3" ColumnCount="3" HorizontalSpacing="50" VerticalSpacing="50" x:Name="diagramModel" &gt;
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
        /// <code language="C#">
        /// using Syncfusion.Core;
        /// using Syncfusion.Windows.Diagram;
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        ///    public DiagramControl Control;
        ///    public DiagramModel Model;
        ///    public DiagramView View;
        ///    public MainPage()
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
        ///        n.Width = 150;
        ///        n.Height = 75;
        ///         Node n1 = new Node(Guid.NewGuid(), "Decision1");
        ///         n1.Shape = Shapes.FlowChart_Process;
        ///         n1.IsLabelEditable = true;
        ///         n1.Label = "Alarm Rings";
        ///         n1.Level = 2;
        ///         n1.Width = 150;
        ///         n1.Height = 75;
        ///        Model.Nodes.Add(n);
        ///        Model.Nodes.Add(n1);
        ///        Model.LayoutType = LayoutType.TableLayout;
        ///        Model.RowCount = 3;
        ///        Model.ColumnCount = 3;
        ///        Model.SpaceBetweenSubTrees = 50;
        ///        Model.LayoutRoot=n;
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
        public int RowCount
        {
            get
            {
                return (int)GetValue(RowCountProperty);
            }

            set
            {
                SetValue(RowCountProperty, value);
            }
        }

        internal double PxSpaceBetweenSubTrees
        {
            get { return MeasureUnitsConverter.ToPixels(SpaceBetweenSubTrees, this.MeasurementUnits); }
            //set { SpaceBetweenSubTrees = MeasureUnitsConverter.FromPixels(value, this.MeasurementUnits); }
        }

        /// <summary>
        /// Gets or sets the space between sub trees.
        /// </summary>
        /// <example>
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
        ///                  &lt;syncfusion:DiagramModel LayoutType="DirectedTreeLayout" HorizontalSpacing="30" VerticalSpacing="50" SpaceBetweenSubTrees="50" x:Name="diagramModel" &gt;
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
        /// <code language="C#">
        /// using Syncfusion.Core;
        /// using Syncfusion.Windows.Diagram;
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        ///    public DiagramControl Control;
        ///    public DiagramModel Model;
        ///    public DiagramView View;
        ///    public MainPage()
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
        ///        n.Width = 150;
        ///        n.Height = 75;
        ///         Node n1 = new Node(Guid.NewGuid(), "Decision1");
        ///         n1.Shape = Shapes.FlowChart_Process;
        ///         n1.IsLabelEditable = true;
        ///         n1.Label = "Alarm Rings";
        ///         n1.Level = 2;
        ///         n1.Width = 150;
        ///         n1.Height = 75;
        ///        Model.Nodes.Add(n);
        ///        Model.Nodes.Add(n1);
        ///        Model.LayoutType = LayoutType.DirectedTreeLayout;
        ///        Model.HorizontalSpacing = 30;
        ///        Model.VerticalSpacing = 50;
        ///        Model.SpaceBetweenSubTrees = 50;
        ///        Model.LayoutRoot=n;
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
        public double SpaceBetweenSubTrees
        {
            get
            {
                return this.msubTreeSpacing;
            }

            set
            {
                this.msubTreeSpacing = value;
            }
        }

        /// <summary>
        /// Gets or sets the table expand mode.
        /// </summary>
        /// <value>The table expand mode.</value>
        /// <remarks>
        /// Default value is  Horizontal.
        /// </remarks>
        /// <example>
        /// <code language="XAML">
        /// &lt;Window x:Class="Sample.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:syncfusion="http://schemas.syncfusion.com/wpf"
        /// WindowState="Maximized" Name="mainwindow"
        /// xmlns:local="clr-namespace:Sample" FontWeight="Bold"
        /// Icon="Images/App.ico" &gt;
        /// &lt;syncfusion:DiagramControl Grid.Column="1" Name="diagramControl"
        /// IsSymbolPaletteEnabled="True"
        /// Background="WhiteSmoke"&gt;
        /// &lt;syncfusion:DiagramControl.Model&gt;
        /// &lt;syncfusion:DiagramModel LayoutType="TableLayout" TableExpandMode="Vertical" RowCount="3" ColumnCount="3" HorizontalSpacing="50" VerticalSpacing="50" x:Name="diagramModel" &gt;
        /// &lt;/syncfusion:DiagramModel&gt;
        /// &lt;/syncfusion:DiagramControl.Model&gt;
        /// &lt;syncfusion:DiagramControl.View&gt;
        /// &lt;syncfusion:DiagramView   Background="LightGray"
        /// Bounds="0,0,12,12"
        /// Name="diagramView"  &gt;
        /// &lt;/syncfusion:DiagramView&gt;
        /// &lt;/syncfusion:DiagramControl.View&gt;
        /// &lt;/syncfusion:DiagramControl&gt;
        /// &lt;/Window&gt;
        /// </code>
        /// <code language="C#">
        /// using Syncfusion.Core;
        /// using Syncfusion.Windows.Diagram;
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        /// public DiagramControl Control;
        /// public DiagramModel Model;
        /// public DiagramView View;
        /// public MainPage()
        /// {
        /// InitializeComponent ();
        /// Control = new DiagramControl ();
        /// Model = new DiagramModel ();
        /// View = new DiagramView ();
        /// Control.View = View;
        /// Control.Model = Model;
        /// View.Bounds = new Thickness(0, 0, 1000, 1000);
        /// Node n = new Node(Guid.NewGuid(), "Start");
        /// n.Shape = Shapes.FlowChart_Start;
        /// n.IsLabelEditable = true;
        /// n.Label = "Start";
        /// n.Level = 1;
        /// n.Width = 150;
        /// n.Height = 75;
        /// Node n1 = new Node(Guid.NewGuid(), "Decision1");
        /// n1.Shape = Shapes.FlowChart_Process;
        /// n1.IsLabelEditable = true;
        /// n1.Label = "Alarm Rings";
        /// n1.Level = 2;
        /// n1.Width = 150;
        /// n1.Height = 75;
        /// Model.Nodes.Add(n);
        /// Model.Nodes.Add(n1);
        /// Model.LayoutType = LayoutType.TableLayout;
        /// Model.TableExpandMode=ExpandMode.Vertical;
        /// Model.RowCount = 3;
        /// Model.ColumnCount = 3;
        /// Model.LayoutRoot=n;
        /// LineConnector o = new LineConnector();
        /// o.ConnectorType = ConnectorType.Straight;
        /// o.TailNode = n1;
        /// o.HeadNode = n;
        /// o.HeadDecoratorShape = DecoratorShape.None;
        /// Model.Connections.Add(o);
        /// }
        /// }
        /// }
        /// </code>
        /// </example>
        public ExpandMode TableExpandMode
        {
            get
            {
                return (ExpandMode)GetValue(TableExpandModeProperty);
            }

            set
            {
                SetValue(TableExpandModeProperty, value);
            }
        }

        internal double PxVerticalSpacing
        {
            get
            {
                return MeasureUnitsConverter.ToPixels(VerticalSpacing, this.MeasurementUnits);
            }
            //set
            //{
            //    VerticalSpacing = MeasureUnitsConverter.FromPixels(value, this.MeasurementUnits);
            //}
        }

        /// <summary>
        /// Gets or sets the Vertical spacing between nodes.
        /// </summary>
        /// <example>
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
        ///                  &lt;syncfusion:DiagramModel LayoutType="DirectedTreeLayout" HorizontalSpacing="30" VerticalSpacing="50" x:Name="diagramModel" &gt;
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
        /// <code language="C#">
        /// using Syncfusion.Core;
        /// using Syncfusion.Windows.Diagram;
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        ///    public DiagramControl Control;
        ///    public DiagramModel Model;
        ///    public DiagramView View;
        ///    public MainPage()
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
        ///        n.Width = 150;
        ///        n.Height = 75;
        ///         Node n1 = new Node(Guid.NewGuid(), "Decision1");
        ///         n1.Shape = Shapes.FlowChart_Process;
        ///         n1.IsLabelEditable = true;
        ///         n1.Label = "Alarm Rings";
        ///         n1.Level = 2;
        ///         n1.Width = 150;
        ///         n1.Height = 75;
        ///        Model.Nodes.Add(n);
        ///        Model.Nodes.Add(n1);
        ///        Model.LayoutType = LayoutType.DirectedTreeLayout;
        ///        Model.HorizontalSpacing = 30;
        ///        Model.VerticalSpacing = 50;
        ///        Model.SpaceBetweenSubTrees = 50;
        ///        Model.LayoutRoot=n;
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
        public double VerticalSpacing
        {
            get
            {
                return this.mverticalSpacing;
            }

            set
            {
                this.mverticalSpacing = value;
            }
        }

        /// <summary>
        /// Given a Node upon which this Edge is incident, the opposite incident
        /// Node is returned. Throws an exception if the input node is not incident
        /// on this Edge.
        /// </summary>
        /// <param name="edge">The edge object.</param>
        /// <param name="node">The node object.</param>
        /// <returns>The adjacent node</returns>
        IShape IGraph.AdjacentNode(IEdge edge, IShape node)
        {
            if (edge.HeadNode == node)
            {
                return edge.TailNode;
            }
            else if (edge.TailNode == node)
            {
                return edge.HeadNode;
            }
            else
            {
                throw new Exception("The node is not a target or source node of the given edge.");
            }
        }

        /// <summary>
        /// Clear the traversed tree.
        /// </summary>
        void IGraph.ClearTraversing()
        {
            this.mspanningTree = null;
        }

        /// <summary>
        /// Get the degree of the node, the number of edges for which this node
        /// is either the source or the target.
        /// </summary>
        /// <param name="node">The node object.</param>
        /// <returns>The degree value.</returns>
        int IGraph.Degree(IShape node)
        {
            return (node as IShape).Degree;
        }

        /// <summary>
        /// Returns a collection of the edges of the node.
        /// </summary>
        /// <param name="node">The node object</param>
        /// <returns>The node edges</returns>
        CollectionExt IGraph.EdgesOf(IShape node)
        {
            return node.Edges;
        }

        /// <summary>
        /// Gets the edge of the Node from which the connection started. 
        /// </summary>
        /// <param name="edge">The edge of the node</param>
        /// <returns>Edge of the head node.</returns>
        IShape IGraph.FromNode(IEdge edge)
        {
            return edge.HeadNode;
        }

        /// <summary>
        /// Get the in-degree of the node, the number of edges for which this node
        /// is the target.
        /// </summary>
        /// <param name="node">The node object.</param>
        /// <returns>The in degree .</returns>
        int IGraph.InDegree(IShape node)
        {
            return (node as IShape).InDegree;
        }

        /// <summary>
        /// Gets the collection of all incoming edges, those for which this node
        /// is the source.
        /// </summary>
        /// <param name="node">The node object.</param>
        /// <returns>The outgoing edges</returns>
        CollectionExt IGraph.InEdges(IShape node)
        {
            return node.InEdges;
        }

        /// <summary>
        /// Gets the collection of all adjacent nodes connected to this node by an
        /// incoming edge (i.e., all nodes that "point" at this one).
        /// </summary>
        /// <param name="node">The node object.</param>
        /// <returns>The in coming neighbor nodes</returns>
        CollectionExt IGraph.InNeighbors(IShape node)
        {
            return node.InNeighbors;
        }

        #region layout alignment
        internal void CommonUpdate(Point? start, List<IShape> TableLayoutCollection,object layout)
        {
                TableLayoutCollection = new List<IShape>();
                if (OrderedNodes != null)
                {
                    m_IsSelectionBaseLayout = true;
                    TableLayoutCollection =OrderedNodes.ToList();
                }
                else
                {
                    m_IsSelectionBaseLayout = false;
                    TableLayoutCollection =Nodes.OfType<IShape>().ToList();
                }
                if (layout is TableLayout)
                {
                    UpdateTableLayout(layout as TableLayout, TableLayoutCollection,start);
                }
                if (layout is RadialTreeLayout)
                {
                    UpdateTableLayout(layout as RadialTreeLayout, TableLayoutCollection, start);
                }
                if (layout is DirectedTreeLayout)
                {
                    UpdateTableLayout(layout as DirectedTreeLayout, TableLayoutCollection, start);
                }
                if (layout is BowtieLayout)
                {
                    UpdateTableLayout(layout as BowtieLayout, TableLayoutCollection, start);
                }
                if (layout is HierarchicalTreeLayout)
                {
                    UpdateTableLayout(layout as HierarchicalTreeLayout, TableLayoutCollection, start);
                }
        }
        double moveX = 0;
        double moveY = 0;
        Rect _tablebounds;
        private void SetLayoutAlignMent(Point? startpoint,List<IShape> TableLayoutCollection,object layout)
        {
            if (startpoint == null)
            {
                switch (this.LayoutHorizontalAlignment)
                {
                    case LayoutHorizontalAlignment.Left:
                        Point value = nodestartdrop(TableLayoutCollection);
                        if (!(layout is TableLayout))
                        {                           
                            moveX = _tablebounds.X - value.X;
                        }
                        else
                        {
                            moveX = _tablebounds.X;
                        }
                        break;
                    case LayoutHorizontalAlignment.Center:                       
                        value = nodedropRect(TableLayoutCollection);
                        moveX = _tablebounds.Width / 2 - value.X / 2 + _tablebounds.X;
                        Point value2 = nodestartdrop(TableLayoutCollection);
                        if (!(layout is TableLayout))
                        {
                            moveX = moveX - value2.X;
                        }
                        break;
                    case LayoutHorizontalAlignment.Right:
                        value = nodedropRect(TableLayoutCollection);
                        moveX = _tablebounds.Width - value.X + _tablebounds.X;
                        value2 = nodestartdrop(TableLayoutCollection);
                        if (!(layout is TableLayout))
                        {
                            moveX = moveX - value2.X;
                        }
                        break;
                }
                switch (this.LayoutVerticalAlignment)
                {
                    case LayoutVerticalAlignment.Top:

                        Point value = nodestartdrop(TableLayoutCollection);
                        if (!(layout is TableLayout))
                        {
                            moveY = _tablebounds.Y - value.Y;
                        }
                        else
                        {
                            moveY = _tablebounds.Y;
                        }
                      
                        break;
                    case LayoutVerticalAlignment.Center:
                         value = nodedropRect(TableLayoutCollection);
                         moveY = _tablebounds.Height / 2 - value.Y / 2 + _tablebounds.Y;
                        Point value2 = nodestartdrop(TableLayoutCollection);
                        if (!(layout is TableLayout))
                        {
                            moveY = moveY - value2.Y;
                        }
                        break;
                    case LayoutVerticalAlignment.Bottom:
                        value = nodedropRect(TableLayoutCollection);
                        moveY = _tablebounds.Height - value.Y + _tablebounds.Y;
                        value2 = nodestartdrop(TableLayoutCollection);
                        if (!(layout is TableLayout))
                        {
                            moveY = moveY - value2.Y;
                        }
                        break;
                }
                UpdateNodeVlaue(moveX, moveY,TableLayoutCollection);
            }
            else
            {
                UpdateNodeVlaue(startpoint.Value.X, startpoint.Value.Y,TableLayoutCollection);
            }
        }

        private Point nodedropRect(List<IShape> _dropbox)
        {
            double offx = _dropbox.Min(p => (p as Node)._tempOffsetX);
            double offy = _dropbox.Min(p => (p as Node)._tempOffsetY);
            double offx1 = (_dropbox.Max(p => (p as Node)._tempOffsetX + (p as Node).ActualWidth));
            double offy1 = _dropbox.Max(p => (p as Node)._tempOffsetY + (p as Node).ActualHeight);
            return new Point(Math.Abs(offx1 - offx), Math.Abs(offy1 - offy));
        }
        private Point nodestartdrop(List<IShape> _dropbox)
        {
            double offx = _dropbox.Min(p => (p as Node)._tempOffsetX);
            double offy = _dropbox.Min(p => (p as Node)._tempOffsetY);
            double offx1 = (_dropbox.Max(p => (p as Node)._tempOffsetX + (p as Node).ActualWidth));
            double offy1 = _dropbox.Max(p => (p as Node)._tempOffsetY + (p as Node).ActualHeight);
            return new Point(offx, offy);
        }


        private void UpdateNodeVlaue(double x, double y, List<IShape> TableLayoutCollection)
        {

            foreach (IShape shap in TableLayoutCollection)
            {
                if (!(shap is Group))
                {
                    (shap as Node).PxOffsetX = (shap as Node)._tempOffsetX + x;
                    (shap as Node).PxOffsetY = (shap as Node)._tempOffsetY + y;
                }
                else
                {
                    if (!dc.IsLoaded)
                    {
                        (shap as Node).PxOffsetX = (shap as Node)._tempOffsetX + x;
                        (shap as Node).PxOffsetY = (shap as Node)._tempOffsetY + y;
                        (shap as Group).Width = 0;
                        (shap as Group).Height = 0;
                        //(shap as Group).Visibility = Visibility.Collapsed;
                    }
                }
            }
        }


        internal void UpdateTableLayout(object layout,List<IShape> TableLayoutCollection,Point? start)
        {

            //if (layout is TableLayout)
            //{
            //    (layout as TableLayout).PrepareActivity(layout as TableLayout);
            //    (layout as TableLayout).StartNodeArrangement();
            //}
            //if (layout is RadialTreeLayout)
            //{
            //    (layout as RadialTreeLayout).PrepareActivity(layout as RadialTreeLayout);
            //}
            //if (layout is BowtieLayout)
            //{
            //    (layout as BowtieLayout).PrepareActivity(layout as BowtieLayout);
            //    (layout as BowtieLayout).StartNodeArrangement();
            //}
            //if (layout is DirectedTreeLayout)
            //{
            //    (layout as DirectedTreeLayout).PrepareActivity(layout as DirectedTreeLayout);
            //    (layout as DirectedTreeLayout).StartNodeArrangement();
            //}
            _tablebounds = dc.View.PxLayoutBounds;
            if (dc.View.LayoutBounds == new Rect(0, 0, 0, 0))
            {
                if (this.EnableCollectionBounds && TableLayoutCollection.Count > 0)
                {
                    _tablebounds = nodedropRectValue(TableLayoutCollection);
                }
                else
                {
                    if (dc.View.Scrollviewer != null)
                    {
                        _tablebounds = new Rect(0, 0, (int)dc.View.Scrollviewer.ActualWidth, (int)dc.View.Scrollviewer.ActualHeight);
                    }
                }
            }
            if (TableLayoutCollection.Count > 0)
            {
                SetLayoutAlignMent(start,TableLayoutCollection,layout);
            }
            (dc.View.Page as DiagramPage).InvalidateMeasure();
            (dc.View.Page as DiagramPage).InvalidateArrange();
        }

        private Rect nodedropRectValue(List<IShape> _dropbox)
        {
            double offx = _dropbox.Min(p => (p as Node).OffsetX);
            double offy = _dropbox.Min(p => (p as Node).OffsetY);
            double offx1 = (_dropbox.Max(p => (p as Node).OffsetX + (p as Node).ActualWidth));
            double offy1 = _dropbox.Max(p => (p as Node).OffsetY + (p as Node).ActualHeight);
            return new Rect((int)offx, (int)offy, (int)Math.Abs(offx1 - offx), (int)Math.Abs(offy1 - offy));
        }
        #endregion

        /// <summary>
        /// Traverse all the internal nodes of the specified parent node and also their edges.
        /// </summary>
        /// <param name="node">Parent Node</param>
        void IGraph.MakeTraversing(IShape node)
        {
            LinkedList<IShape> list = new LinkedList<IShape>();
            //BitArray visit = new BitArray(this.Nodes.Count);
            Dictionary<IShape, bool> visit = new Dictionary<IShape, bool>(this.Nodes.Count);
            foreach (IShape item in this.Nodes)
            {
                visit.Add(item, false);
            }

            list.AddFirst(node);

            int index = this.Nodes.IndexOf(node as IShape);
            if(index>=0)
            {
                visit[node as IShape] = true;
            }
            //CollectionExt edges = (this as IGraph).Edges;
            IShape n;
            while (list.Count > 0&&list.First.Value!=null)
            {
                IShape f = list.First.Value;
                IShape p = list.First.Value;
                if (LayoutType == LayoutType.DirectedTreeLayout || LayoutType == Diagram.LayoutType.BowtieLayout)
                {
                   
                    p.Children = new CollectionExt();
                }

                list.RemoveFirst();
                foreach (IEdge edge in p.Edges)
                {
                    if (LayoutType == Diagram.LayoutType.BowtieLayout)
                    {
                        //if(!(p.OutEdges.Contains(edge)))
                        if (node.Edges.Contains(edge))
                        {
                            if (
                                (
                                DiagramControl.GetBowtieSubTreePlacement(edge.AdjacentNode(p) as Node) == BowtieSubTreePlacement.Right
                                && BowtieOrientation == TreeOrientation.RightLeft
                                 )
                                )
                            {
                                continue;
                            }
                            else if (
                                (
                                 DiagramControl.GetBowtieSubTreePlacement(edge.AdjacentNode(p) as Node) == BowtieSubTreePlacement.Left
                                && BowtieOrientation == TreeOrientation.LeftRight
                                 )
                                )
                            {
                                continue;
                            }
                        }
                    }
                    n = edge.AdjacentNode(p);

                    if (n == null)
                    {
                        continue;
                    }

                    try
                    {
                        if (!visit[n as IShape])
                        {
                            if (this.LayoutType == LayoutType.HierarchicalTreeLayout)
                            {
                                if (n.Parents.Count > 1 || list.Count == 0)
                                {
                                    list.AddLast(n);
                                }
                                else
                                {
                                    list.AddBefore(list.Last, n);
                                }
                            }
                            else
                            {
                                list.AddLast(n);
                            }

                            visit[n as IShape] = true;

                            //if (this.LayoutType == LayoutType.HierarchicalTreeLayout)
                            //{
                            //    if (n.Parents.Count > 1)
                            //    {
                            //        foreach (IShape parent in n.Parents)
                            //        {
                            //            if (parent.OutNeighbors.Count > 2)
                            //            {
                            //                p = parent;
                            //            }
                            //        }
                            //    }
                            //    else
                            //    {
                            //        p = n.Parents[0] as IShape;
                            //    }
                            //}

                            n.ParentNode = p;
                            n.ParentEdge = edge;
                            if (p.Children == null)
                            {
                                p.Children = new CollectionExt();
                            }

                            if (!p.Children.Contains(n))
                            {
                                p.Children.Add(n);
                            }

                            p = f;
                        }
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        continue;
                    }
                }
            }

            this.mspanningTree = this as ITree;
        }

        /// <summary>
        /// Get an iterator over all nodes connected to this node.
        /// </summary>
        /// <param name="node">The node object.</param>
        /// <returns>The neighbor nodes</returns>
        CollectionExt IGraph.Neighbors(IShape node)
        {
            return node.Neighbors;
        }

        /// <summary>
        /// Get the out-degree of the node, the number of edges for which this node
        /// is the source.
        /// </summary>
        /// <param name="node">The node object.</param>
        /// <returns>The out degree.</returns>
        int IGraph.OutDegree(IShape node)
        {
            return (node as IShape).OutDegree;
        }

        /// <summary>
        /// Gets the collection of all outgoing edges, those for which this node
        /// is the source.
        /// </summary>
        /// <param name="node">The node object.</param>
        /// <returns>The outgoing edges</returns>
        CollectionExt IGraph.OutEdges(IShape node)
        {
            return node.OutEdges;
        }

        /// <summary>
        /// Gets the collection of adjacent nodes connected to this node by an
        /// outgoing edge (i.e., all nodes "pointed" to by this one).
        /// </summary>
        /// <param name="node">The node object.</param>
        /// <returns>The out going neighbor nodes</returns>
        CollectionExt IGraph.OutNeighbors(IShape node)
        {
            return node.OutNeighbors;
        }

        /// <summary>
        /// Gets the edge of the Node to which the connection ended. 
        /// </summary>
        /// <param name="edge">The edge of the node</param>
        /// <returns>Edge of the tail node.</returns>
        IShape IGraph.ToNode(IEdge edge)
        {
            return edge.TailNode;
        }

        /// <summary>
        /// Gets a count of the number of children of the parent node.
        /// </summary>
        /// <param name="node">Parent Node</param>
        /// <returns>The child count</returns>
        int ITree.ChildCount(IShape node)
        {
            if (this.mspanningTree == null)
            {
                return 0;
            }

            return this.mspanningTree.ChildCount(node);
        }

        /// <summary>
        /// Gets a collection of Node's Children's edges.
        /// </summary>
        /// <param name="node">The node object</param>
        /// <returns>The Child edges of the node</returns>
        CollectionExt ITree.ChildEdges(IShape node)
        {
            if (this.mspanningTree == null)
            {
                return null;
            }

            return this.mspanningTree.ChildEdges(node);
        }

        /// <summary>
        /// Gets the Collection of the Node's Children.
        /// </summary>
        /// <param name="node">Parent Node</param>
        /// <returns>The Children of the node</returns>
        CollectionExt ITree.Children(IShape node)
        {
            if (this.mspanningTree == null)
            {
                return null;
            }

            return this.mspanningTree.Children(node);
        }

        /// <summary>
        /// Gets the number of levels from this Node as the Depth of the current node.
        /// </summary>
        /// <param name="node">Node object.</param>
        /// <returns>The depth value.</returns>
        int ITree.Depth(IShape node)
        {
            if (this.mspanningTree == null)
            {
                return 0;
            }

            return this.mspanningTree.Depth(node);
        }

        /// <summary>
        /// Gets the immediate child as the First child of the ParentNode.
        /// </summary>
        /// <param name="node">Parent Node.</param>
        /// <returns>The First Child</returns>
        IShape ITree.FirstChild(IShape node)
        {
            if (this.mspanningTree == null)
            {
                return null;
            }

            return this.mspanningTree.FirstChild(node);
        }

        /// <summary>
        /// Takes a specified action on each node starting from the start node.
        /// </summary>
        /// <typeparam name="T">The type of action</typeparam>
        /// <param name="action">The action.</param>
        /// <param name="startNode">The start node.</param>
        void ITree.ForEach<T>(Action<T> action, IShape startNode)
        {
            (action as Action<IShape>).Invoke(startNode);
            if (startNode.Children == null)
            {
                return;
            }

            foreach (IShape node in startNode.Children)
            {
                (this as ITree).ForEach<IShape>((action as Action<IShape>), node);
            }
        }

        /// <summary>
        /// Gets the last child of the ParentNode.
        /// </summary>
        /// <param name="node">Parent Node.</param>
        /// <returns>The last child</returns>
        IShape ITree.LastChild(IShape node)
        {
            if (this.mspanningTree == null)
            {
                return null;
            }

            return this.mspanningTree.LastChild(node);
        }

        /// <summary>
        /// Gets the node placed next to this node and at the same level as this node.
        /// </summary>
        /// <param name="node">Node object</param>
        /// <returns>The next sibling.</returns>
        IShape ITree.NextSibling(IShape node)
        {
            if (this.mspanningTree == null)
            {
                return null;
            }

            return this.mspanningTree.NextSibling(node);
        }

        /// <summary>
        /// Gets the parent edge
        /// </summary>
        /// <param name="node">The node object.</param>
        /// <returns>The parent edge</returns>
        IEdge ITree.ParentEdge(IShape node)
        {
            if (this.mspanningTree == null)
            {
                return null;
            }

            return this.mspanningTree.ParentEdge(node);
        }

        /// <summary>
        /// Gets the node placed previous to this node and at the same level as this node.
        /// </summary>
        /// <param name="node">Node object</param>
        /// <returns>The previous sibling</returns>
        IShape ITree.PreviousSibling(IShape node)
        {
            if (this.mspanningTree == null)
            {
                return null;
            }

            return this.mspanningTree.PreviousSibling(node);
        }

        #region Fields

        #endregion

        #region Properties

        #endregion

        #region DPs

        #endregion

        #region IGraph Members

        #endregion

        #region ITree Members

        #endregion

        #region IModel Memebers

        #endregion
    }
}
