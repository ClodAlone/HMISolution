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
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Windows.Media;
    using System.Windows;

    /// <summary>
    /// Represents customizable options  for <see cref="LineConnector"/>.
    /// </summary>
    public class LineStyle : DependencyObject,INotifyPropertyChanged
    {
        /// <summary>
        /// Represents the ConnectorBase object.
        /// </summary>
        private ConnectorBase connBase;

        /// <summary>
        /// Stores the Fill property value.
        /// </summary>
        private Brush fillcolor = new SolidColorBrush(Colors.Blue);

        /// <summary>
        /// Stores the Stroke property value.
        /// </summary>
        private Brush strokecolor = new SolidColorBrush(Colors.Gray);

        /// <summary>
        /// Stores the StrokeDashArray property value.
        /// </summary>
        private DoubleCollection strokeDashArray;

        /// <summary>
        /// Stores the StrokeEndLineCap property value.
        /// </summary>
        private PenLineCap strokeEndLineCap = PenLineCap.Round;

        /// <summary>
        /// Stores the StrokeLineJoin property value.
        /// </summary>
        private PenLineJoin strokeLineJoin = PenLineJoin.Round;

        /// <summary>
        /// Stores the StrokeStartLineCap property value.
        /// </summary>
        private PenLineCap strokeStartLineCap = PenLineCap.Round;

        /// <summary>
        /// Stores the StrokeThickness property value.
        /// </summary>
        private double strokeThickness = 2;

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
            this.connBase = cbase;
        }
        
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

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
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        ///    public DiagramControl Control;
        ///    public DiagramModel Model;
        ///    public DiagramView View;
        ///    public MainPage()
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
                return this.fillcolor;
            }

            set
            {
                this.fillcolor = value;
                this.OnPropertyChanged("Fill");
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
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        ///    public DiagramControl Control;
        ///    public DiagramModel Model;
        ///    public DiagramView View;
        ///    public MainPage()
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
                return this.strokecolor;
            }

            set
            {
                this.strokecolor = value;
                this.OnPropertyChanged("Stroke");
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
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        ///    public DiagramControl Control;
        ///    public DiagramModel Model;
        ///    public DiagramView View;
        ///    public MainPage()
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
        public DoubleCollection StrokeDashArray
        {
            get
            {
                return this.strokeDashArray;
            }

            set
            {
                if (this.strokeDashArray != value)
                {
                    this.strokeDashArray = value;
                    this.OnPropertyChanged("StrokeDashArray");
                }
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
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        ///    public DiagramControl Control;
        ///    public DiagramModel Model;
        ///    public DiagramView View;
        ///    public MainPage()
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
                return this.strokeEndLineCap;
            }

            set
            {
                this.strokeEndLineCap = value;
                this.OnPropertyChanged("strokeEndLineCap");
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
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        ///    public DiagramControl Control;
        ///    public DiagramModel Model;
        ///    public DiagramView View;
        ///    public MainPage()
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
                return this.strokeLineJoin;
            }

            set
            {
                this.strokeLineJoin = value;
                this.OnPropertyChanged("strokeLineJoin");
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
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        ///    public DiagramControl Control;
        ///    public DiagramModel Model;
        ///    public DiagramView View;
        ///    public MainPage()
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
                return this.strokeStartLineCap;
            }

            set
            {
                this.strokeStartLineCap = value;
                this.OnPropertyChanged("strokeStartLineCap");
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
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        ///    public DiagramControl Control;
        ///    public DiagramModel Model;
        ///    public DiagramView View;
        ///    public MainPage()
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
                return this.strokeThickness;
            }

            set
            {
                this.strokeThickness = value;

                this.OnPropertyChanged("StrokeThickness");
            }
        }

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="name">The name of the property that has changed.</param>
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
        #region class variables

        #endregion

        #region Initialization

        #endregion

        #region Public Properties

        #endregion

        #region INotifyPropertyChanged Members

        #endregion
    }
}
