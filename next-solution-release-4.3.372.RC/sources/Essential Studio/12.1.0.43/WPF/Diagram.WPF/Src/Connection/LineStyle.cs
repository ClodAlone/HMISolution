// <copyright file="LineStyle.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows;

namespace Syncfusion.Windows.Diagram
{
    /// <summary>
    /// Represents customizable options  for <see cref="LineConnector"/>.
    /// </summary>
    public class LineStyle : DependencyObject
    {
        #region class variables

        /// <summary>
        /// Stores the StrokeStartLineCap property value.
        /// </summary>
        public static readonly DependencyProperty StrokeStartLineCapProperty = DependencyProperty.Register("StrokeStartLineCap", typeof(PenLineCap), typeof(LineStyle), new PropertyMetadata(PenLineCap.Round));
        /// <summary>
        /// Stores the StrokeEndLineCap property value.
        /// </summary>
        public static readonly DependencyProperty StrokeEndLineCapProperty = DependencyProperty.Register("StrokeEndLineCap", typeof(PenLineCap), typeof(LineStyle), new PropertyMetadata(PenLineCap.Round));
        /// <summary>
        /// Stores the Stroke property value.
        /// </summary>
        private Brush strokecolor = Brushes.Gray;
        public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register("Stroke", typeof(Brush), typeof(LineStyle), new PropertyMetadata(Brushes.Gray));
       
        /// <summary>
        /// Stores the StrokeLineJoin property value.
        /// </summary>
        public static readonly DependencyProperty StrokeLineJoinProperty = DependencyProperty.Register("StrokeLineJoin", typeof(PenLineJoin), typeof(LineStyle), new PropertyMetadata(PenLineJoin.Round));
       
        /// <summary>
        /// Stores the StrokeThickness property value.
        /// </summary>
        public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register("StrokeThickness", typeof(double), typeof(LineStyle), new PropertyMetadata(2d));

        /// <summary>
        /// Stores the Fill property value.
        /// </summary>
        public static readonly DependencyProperty FillProperty = DependencyProperty.Register("Fill", typeof(Brush), typeof(LineStyle), new PropertyMetadata(Brushes.Gray));

        /// <summary>
        /// Stores the StrokeDashArray property value.
        /// </summary>
        public static readonly DependencyProperty StrokeDashArrayProperty = DependencyProperty.Register("StrokeDashArray", typeof(DoubleCollection), typeof(LineStyle), new PropertyMetadata(null));

        /// <summary>
        /// Represents the ConnectorBase object.
        /// </summary>
        private ConnectorBase connBase;
      
        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="LineStyle"/> class.
        /// </summary>
        public LineStyle()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LineStyle"/> class.
        /// </summary>
        /// <param name="cbase"><see cref="ConnectorBase"/> reference.</param>
        public LineStyle(ConnectorBase cbase)
        {
            connBase = cbase;
        }

        #endregion
     
        #region Public Properties

        /// <summary>
        /// Gets or sets the fill.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Color to fill the decorator with.
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
        ///        //Creates a node
        ///        Node n = new Node(Guid.NewGuid(), "Node1");
        ///        n.Shape = Shapes.FlowChart_Start;
        ///        n.IsLabelEditable = true;
        ///        n.Label = "Start";
        ///        n.Level = 1;
        ///        n.OffsetX = 150;
        ///        n.OffsetY = 25;
        ///        n.Width = 150;
        ///        n.Height = 75;
        ///        n.ToolTip="Start Node";
        ///        Model.Nodes.Add(n);
        ///        Node n1 = new Node(Guid.NewGuid(), "Node2");
        ///        n1.Shape = Shapes.FlowChart_Process;
        ///        n1.IsLabelEditable = true;
        ///        n1.Label = "Alarm Rings";
        ///        n1.Level = 2;
        ///        n1.OffsetX = 150;
        ///        n1.OffsetY = 125;
        ///        n1.Width = 150;
        ///        n1.Height = 75;
        ///        Model.Nodes.Add(n1);
        ///        LineConnector o2 = new LineConnector();
        ///        o2.ConnectorType = ConnectorType.Straight;
        ///        o2.TailNode = n1;
        ///        o2.HeadNode = n;
        ///        o2.LabelHorizontalAlignment = HorizontalAlignment.Center;
        ///        o2.LineStyle.Fill = Brushes.Red;
        ///        o2.LineStyle.Stroke = Brushes.Orange;
        ///        o2.LineStyle.StrokeThickness = 2;
        ///        Model.Connections.Add(o2);
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        public Brush Fill
        {
            get
            {
                return (Brush) GetValue(FillProperty);
            }

            set
            {
                SetValue(FillProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the stroke thickness.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Thickness of the stroke.
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
        ///        //Creates a node
        ///        Node n = new Node(Guid.NewGuid(), "Node1");
        ///        n.Shape = Shapes.FlowChart_Start;
        ///        n.IsLabelEditable = true;
        ///        n.Label = "Start";
        ///        n.Level = 1;
        ///        n.OffsetX = 150;
        ///        n.OffsetY = 25;
        ///        n.Width = 150;
        ///        n.Height = 75;
        ///        n.ToolTip="Start Node";
        ///        Model.Nodes.Add(n);
        ///        Node n1 = new Node(Guid.NewGuid(), "Node2");
        ///        n1.Shape = Shapes.FlowChart_Process;
        ///        n1.IsLabelEditable = true;
        ///        n1.Label = "Alarm Rings";
        ///        n1.Level = 2;
        ///        n1.OffsetX = 150;
        ///        n1.OffsetY = 125;
        ///        n1.Width = 150;
        ///        n1.Height = 75;
        ///        Model.Nodes.Add(n1);
        ///        LineConnector o2 = new LineConnector();
        ///        o2.ConnectorType = ConnectorType.Straight;
        ///        o2.TailNode = n1;
        ///        o2.HeadNode = n;
        ///        o2.LabelHorizontalAlignment = HorizontalAlignment.Center;
        ///        o2.LineStyle.Fill = Brushes.Red;
        ///        o2.LineStyle.Stroke = Brushes.Orange;
        ///        o2.LineStyle.StrokeThickness = 2;
        ///        Model.Connections.Add(o2);
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        public double StrokeThickness
        {
            get
            {
                return (double)GetValue(StrokeThicknessProperty);
            }

            set
            {
                SetValue(StrokeThicknessProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the stroke.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Color to fill the decorator's border with.
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
        ///        //Creates a node
        ///        Node n = new Node(Guid.NewGuid(), "Node1");
        ///        n.Shape = Shapes.FlowChart_Start;
        ///        n.IsLabelEditable = true;
        ///        n.Label = "Start";
        ///        n.Level = 1;
        ///        n.OffsetX = 150;
        ///        n.OffsetY = 25;
        ///        n.Width = 150;
        ///        n.Height = 75;
        ///        n.ToolTip="Start Node";
        ///        Model.Nodes.Add(n);
        ///        Node n1 = new Node(Guid.NewGuid(), "Node2");
        ///        n1.Shape = Shapes.FlowChart_Process;
        ///        n1.IsLabelEditable = true;
        ///        n1.Label = "Alarm Rings";
        ///        n1.Level = 2;
        ///        n1.OffsetX = 150;
        ///        n1.OffsetY = 125;
        ///        n1.Width = 150;
        ///        n1.Height = 75;
        ///        Model.Nodes.Add(n1);
        ///        LineConnector o2 = new LineConnector();
        ///        o2.ConnectorType = ConnectorType.Straight;
        ///        o2.TailNode = n1;
        ///        o2.HeadNode = n;
        ///        o2.LabelHorizontalAlignment = HorizontalAlignment.Center;
        ///        o2.LineStyle.Fill = Brushes.Red;
        ///        o2.LineStyle.Stroke = Brushes.Orange;
        ///        o2.LineStyle.StrokeThickness = 2;
        ///        Model.Connections.Add(o2);
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        public Brush Stroke
        {
            get
            {
                return (Brush)GetValue(StrokeProperty);
            }

            set
            {
                SetValue(StrokeProperty,value);
            }
        }

        /// <summary>
        /// Gets or sets the stroke start line cap.
        /// </summary>
        /// <value>
        /// Type: <see cref="PenLineCap"/>
        /// Enum specifying the shape to use.
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
        ///        //Creates a node
        ///        Node n = new Node(Guid.NewGuid(), "Node1");
        ///        n.Shape = Shapes.FlowChart_Start;
        ///        n.IsLabelEditable = true;
        ///        n.Label = "Start";
        ///        n.Level = 1;
        ///        n.OffsetX = 150;
        ///        n.OffsetY = 25;
        ///        n.Width = 150;
        ///        n.Height = 75;
        ///        n.ToolTip="Start Node";
        ///        Model.Nodes.Add(n);
        ///        Node n1 = new Node(Guid.NewGuid(), "Node2");
        ///        n1.Shape = Shapes.FlowChart_Process;
        ///        n1.IsLabelEditable = true;
        ///        n1.Label = "Alarm Rings";
        ///        n1.Level = 2;
        ///        n1.OffsetX = 150;
        ///        n1.OffsetY = 125;
        ///        n1.Width = 150;
        ///        n1.Height = 75;
        ///        Model.Nodes.Add(n1);
        ///        LineConnector o2 = new LineConnector();
        ///        o2.ConnectorType = ConnectorType.Straight;
        ///        o2.TailNode = n1;
        ///        o2.HeadNode = n;
        ///        o2.LabelHorizontalAlignment = HorizontalAlignment.Center;
        ///        o2.LineStyle.Fill = Brushes.Red;
        ///        o2.LineStyle.Stroke = Brushes.Orange;
        ///        o2.LineStyle.StrokeStartLineCap = PenLineCap.Triangle;
        ///        Model.Connections.Add(o2);
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        public PenLineCap StrokeStartLineCap
        {
            get
            {
                return (PenLineCap) GetValue(StrokeStartLineCapProperty);
            }

            set
            {
                SetValue(StrokeStartLineCapProperty,value);
            }
        }

        /// <summary>
        /// Gets or sets the stroke end line cap.
        /// </summary>
        /// <value>
        /// Type: <see cref="PenLineCap"/>
        /// Enum specifying the shape to use.
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
        ///        //Creates a node
        ///        Node n = new Node(Guid.NewGuid(), "Node1");
        ///        n.Shape = Shapes.FlowChart_Start;
        ///        n.IsLabelEditable = true;
        ///        n.Label = "Start";
        ///        n.Level = 1;
        ///        n.OffsetX = 150;
        ///        n.OffsetY = 25;
        ///        n.Width = 150;
        ///        n.Height = 75;
        ///        n.ToolTip="Start Node";
        ///        Model.Nodes.Add(n);
        ///        Node n1 = new Node(Guid.NewGuid(), "Node2");
        ///        n1.Shape = Shapes.FlowChart_Process;
        ///        n1.IsLabelEditable = true;
        ///        n1.Label = "Alarm Rings";
        ///        n1.Level = 2;
        ///        n1.OffsetX = 150;
        ///        n1.OffsetY = 125;
        ///        n1.Width = 150;
        ///        n1.Height = 75;
        ///        Model.Nodes.Add(n1);
        ///        LineConnector o2 = new LineConnector();
        ///        o2.ConnectorType = ConnectorType.Straight;
        ///        o2.TailNode = n1;
        ///        o2.HeadNode = n;
        ///        o2.LabelHorizontalAlignment = HorizontalAlignment.Center;
        ///        o2.LineStyle.Fill = Brushes.Red;
        ///        o2.LineStyle.Stroke = Brushes.Orange;
        ///        o2.LineStyle.StrokeEndLineCap = PenLineCap.Triangle;
        ///        Model.Connections.Add(o2);
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        public PenLineCap StrokeEndLineCap
        {
            get
            {
                return (PenLineCap)GetValue(StrokeEndLineCapProperty);
            }

            set
            {
                SetValue(StrokeStartLineCapProperty,value);
            }
        }

        /// <summary>
        /// Gets or sets the stroke line join.
        /// </summary>
        /// <value>
        /// Type: <see cref="PenLineJoin"/>
        /// Enum specifying the shape to use.
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
        ///        //Creates a node
        ///        Node n = new Node(Guid.NewGuid(), "Node1");
        ///        n.Shape = Shapes.FlowChart_Start;
        ///        n.IsLabelEditable = true;
        ///        n.Label = "Start";
        ///        n.Level = 1;
        ///        n.OffsetX = 150;
        ///        n.OffsetY = 25;
        ///        n.Width = 150;
        ///        n.Height = 75;
        ///        n.ToolTip="Start Node";
        ///        Model.Nodes.Add(n);
        ///        Node n1 = new Node(Guid.NewGuid(), "Node2");
        ///        n1.Shape = Shapes.FlowChart_Process;
        ///        n1.IsLabelEditable = true;
        ///        n1.Label = "Alarm Rings";
        ///        n1.Level = 2;
        ///        n1.OffsetX = 150;
        ///        n1.OffsetY = 125;
        ///        n1.Width = 150;
        ///        n1.Height = 75;
        ///        Model.Nodes.Add(n1);
        ///        LineConnector o2 = new LineConnector();
        ///        o2.ConnectorType = ConnectorType.Straight;
        ///        o2.TailNode = n1;
        ///        o2.HeadNode = n;
        ///        o2.LabelHorizontalAlignment = HorizontalAlignment.Center;
        ///        o2.LineStyle.Fill = Brushes.Red;
        ///        o2.LineStyle.Stroke = Brushes.Orange;
        ///        o2.LineStyle.StrokeLineJoin = PenLineJoin.Miter;
        ///        Model.Connections.Add(o2);
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        public PenLineJoin StrokeLineJoin
        {
            get
            {
                return (PenLineJoin) GetValue(StrokeLineJoinProperty);
            }

            set
            {
                SetValue(StrokeLineJoinProperty,value);
            }
        }

        /// <summary>
        /// Gets or sets the stroke dash array.
        /// </summary>
        /// <value>
        /// Type: <see cref="DoubleCollection"/>
        /// Double collection values .
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
        ///        //Creates a node
        ///        Node n = new Node(Guid.NewGuid(), "Node1");
        ///        n.Shape = Shapes.FlowChart_Start;
        ///        n.IsLabelEditable = true;
        ///        n.Label = "Start";
        ///        n.Level = 1;
        ///        n.OffsetX = 150;
        ///        n.OffsetY = 25;
        ///        n.Width = 150;
        ///        n.Height = 75;
        ///        n.ToolTip="Start Node";
        ///        Model.Nodes.Add(n);
        ///        Node n1 = new Node(Guid.NewGuid(), "Node2");
        ///        n1.Shape = Shapes.FlowChart_Process;
        ///        n1.IsLabelEditable = true;
        ///        n1.Label = "Alarm Rings";
        ///        n1.Level = 2;
        ///        n1.OffsetX = 150;
        ///        n1.OffsetY = 125;
        ///        n1.Width = 150;
        ///        n1.Height = 75;
        ///        Model.Nodes.Add(n1);
        ///        LineConnector o2 = new LineConnector();
        ///        o2.ConnectorType = ConnectorType.Straight;
        ///        o2.TailNode = n1;
        ///        o2.HeadNode = n;
        ///        o2.LabelHorizontalAlignment = HorizontalAlignment.Center;
        ///        o2.LineStyle.Fill = Brushes.Red;
        ///        o2.LineStyle.Stroke = Brushes.Orange;
        ///        o2.LineStyle.StrokeLineJoin = PenLineJoin.Miter;
        ///        o2.LineStyle.StrokeDashArray=new DoubleCollection{1,2};
        ///        Model.Connections.Add(o2);
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public DoubleCollection StrokeDashArray
        {
            get
            {
                return (DoubleCollection) GetValue(StrokeDashArrayProperty);
            }

            set
            {
                SetValue(StrokeDashArrayProperty,value);
            }
        }
        #endregion
    }
}
