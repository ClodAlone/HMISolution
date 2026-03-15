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
    /// Represents customizable options  for Decorator Shapes.
    /// </summary>
    public class DecoratorStyle : DependencyObject, INotifyPropertyChanged
    {
        /// <summary>
        /// Stores the Fill property value.
        /// </summary>
        private Brush mfill = new SolidColorBrush(Colors.Gray);

        /// <summary>
        /// Stores the Stroke property value.
        /// </summary>
        private Brush mstroke = new SolidColorBrush(Colors.Gray);

        /// <summary>
        /// Stores the StrokeEndLineCap property value.
        /// </summary>
        private PenLineCap mstrokeEndLineCap = PenLineCap.Round;

        /// <summary>
        /// Stores the StrokeLineJoin property value.
        /// </summary>
        private PenLineJoin mstrokeLineJoin = PenLineJoin.Round;

        /// <summary>
        /// Stores the StrokeStartLineCap property value.
        /// </summary>
        private PenLineCap mstrokeStartLineCap = PenLineCap.Round;

        /// <summary>
        /// Stores the StrokeThickness property value.
        /// </summary>
        private double mstrokeThickness = 2;

        private object mData;
        private Stretch mstretch=Stretch.Fill;
        private Size msize = new Size(10, 10);
        /// <summary>
        /// Initializes a new instance of the <see cref="DecoratorStyle"/> class.
        /// </summary>
        public DecoratorStyle()
        {
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
        ///        o2.TailDecoratorStyle.Fill = Brushes.Red;
        ///        o2.TailDecoratorStyle.Stroke = Brushes.Orange;
        ///        o2.TailDecoratorStyle.StrokeThickness = 2;
        ///        Model.Connections.Add(o2);
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        /// 
        public Stretch Stretch
        {
            get
            {
                return this.mstretch;
            }
            set
            {
                this.mstretch = value;
                this.OnPropertyChanged("Stretch");
            }
        }
        public Size Size
        {
            get
            {
                return this.msize;
            }
            set
            {
                this.msize=value;
                this.OnPropertyChanged("Size");
            }
        }
        public object Data
        {
            get
            {
                return this.mData;
            }
            set
            {
                this.mData = value;
                this.OnPropertyChanged("Data");
            }

        }
        public Brush Fill
        {
            get
            {
                return this.mfill;
            }

            set
            {
                this.mfill = value;
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
        ///        o2.TailDecoratorStyle.Fill = Brushes.Red;
        ///        o2.TailDecoratorStyle.Stroke = Brushes.Orange;
        ///        o2.TailDecoratorStyle.StrokeThickness = 2;
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
                return this.mstroke;
            }

            set
            {
                this.mstroke = value;
                this.OnPropertyChanged("Stroke");
            }
        }



        public DoubleCollection StrokeDashArray
        {
            get { return (DoubleCollection)GetValue(StrokeDashArrayProperty); }
            set { SetValue(StrokeDashArrayProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StrokeDashArray.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StrokeDashArrayProperty =
            DependencyProperty.Register("StrokeDashArray", typeof(DoubleCollection), typeof(DecoratorStyle), new PropertyMetadata(null));



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
                return this.mstrokeEndLineCap;
            }

            set
            {
                this.mstrokeEndLineCap = value;
                this.OnPropertyChanged("StrokeEndLineCap");
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
        ///        o2.TailDecoratorStyle.Fill = Brushes.Red;
        ///        o2.TailDecoratorStyle.Stroke = Brushes.Orange;
        ///        o2.TailDecoratorStyle.StrokeLineJoin = PenLineJoin.Miter;
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
                return this.mstrokeLineJoin;
            }

            set
            {
                this.mstrokeLineJoin = value;
                this.OnPropertyChanged("StrokeLineJoin");
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
        ///        o2.TailDecoratorStyle.Fill = Brushes.Red;
        ///        o2.TailDecoratorStyle.Stroke = Brushes.Orange;
        ///        o2.TailDecoratorStyle.StrokeStartLineCap = PenLineCap.Triangle;
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
                return this.mstrokeStartLineCap;
            }

            set
            {
                this.mstrokeStartLineCap = value;
                this.OnPropertyChanged("StrokeStartLineCap");
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
        ///        o2.TailDecoratorStyle.Fill = Brushes.Red;
        ///        o2.TailDecoratorStyle.Stroke = Brushes.Orange;
        ///        o2.TailDecoratorStyle.StrokeThickness = 2;
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
                return this.mstrokeThickness;
            }

            set
            {
                this.mstrokeThickness = value;
                this.OnPropertyChanged("StrokeThickness");
            }
        }

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="name">The name of the property which changed.</param>
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
