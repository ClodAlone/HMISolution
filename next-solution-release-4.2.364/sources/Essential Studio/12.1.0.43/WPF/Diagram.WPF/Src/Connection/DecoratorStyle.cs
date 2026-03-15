// <copyright file="DecoratorStyle.cs" company="Syncfusion">
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
using System.Windows.Shapes;

namespace Syncfusion.Windows.Diagram
{
    /// <summary>
    /// Represents customizable options  for Decorator Shapes.
    /// </summary>
     public class DecoratorStyle : DependencyObject
    {
        #region class variables

        /// <summary>
        /// Stores the StrokeStartLineCap property value.
        /// </summary>
         public static readonly DependencyProperty StrokeStartLineCapProperty = DependencyProperty.Register("StrokeStartLineCap", typeof(PenLineCap), typeof(DecoratorStyle), new PropertyMetadata(PenLineCap.Round));

        /// <summary>
        /// Stores the StrokeEndLineCap property value.
        /// </summary>
         public static readonly DependencyProperty StrokeEndLineCapProperty = DependencyProperty.Register("StrokeEndLineCap", typeof(PenLineCap), typeof(DecoratorStyle), new PropertyMetadata(PenLineCap.Round));

        /// <summary>
        /// Stores the Stroke property value.
        /// </summary>
         public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register("Stroke", typeof(Brush), typeof(DecoratorStyle), new PropertyMetadata(Brushes.Gray));

         /// <summary>
         /// Stores the StrokeLineJoin property value.
         /// </summary>
         public static readonly DependencyProperty StrokeLineJoinProperty = DependencyProperty.Register("StrokeLineJoin", typeof(PenLineJoin), typeof(DecoratorStyle), new PropertyMetadata(PenLineJoin.Round));

        /// <summary>
        /// Stores the StrokeThickness property value.
        /// </summary>
         public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register("StrokeThickness", typeof(double), typeof(DecoratorStyle), new PropertyMetadata(2d));

        /// <summary>
        /// Stores the Fill property value.
        /// </summary>
         public static readonly DependencyProperty FillProperty = DependencyProperty.Register("Fill", typeof(Brush), typeof(DecoratorStyle), new PropertyMetadata(Brushes.Gray));

         /// <summary>
         /// Stores the Path of the Decorator
         /// </summary>
         public static readonly DependencyProperty DataProperty = DependencyProperty.Register("Data", typeof(Geometry), typeof(DecoratorStyle), new PropertyMetadata(null));

         /// <summary>
         /// Stores the Width of the Decorator
         /// </summary>
         public static readonly DependencyProperty WidthProperty = DependencyProperty.Register("Width", typeof(double), typeof(DecoratorStyle), new PropertyMetadata(10d));

         /// <summary>
         /// Stores the Height of the Decorator
         /// </summary>
         public static readonly DependencyProperty HeightProperty = DependencyProperty.Register("Height", typeof(double), typeof(DecoratorStyle), new PropertyMetadata(10d));
   
        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="DecoratorStyle"/> class.
        /// </summary>
        public DecoratorStyle()
        {
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
        ///        o2.TailDecoratorStyle.Fill = Brushes.Red;
        ///        o2.TailDecoratorStyle.Stroke = Brushes.Orange;
        ///        o2.TailDecoratorStyle.StrokeThickness = 2;
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
                return (Brush)GetValue(FillProperty);
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
                return (Brush)GetValue(StrokeProperty);
            }

            set
            {
                SetValue(StrokeProperty, value);
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
                return (PenLineCap)GetValue(StrokeStartLineCapProperty);
            }

            set
            {
                SetValue(StrokeStartLineCapProperty, value);
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
                SetValue(StrokeStartLineCapProperty, value);
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
                return (PenLineJoin)GetValue(StrokeLineJoinProperty);
            }

            set
            {
                SetValue(StrokeLineJoinProperty, value);
            }
        }
        public Geometry Data
        {
            get
            {
                return (Geometry)GetValue(DataProperty);
            }
            set
            {
                SetValue(DataProperty, value);
            }
        }

        public double Width
        {
            get
            {
                return (double)GetValue(WidthProperty);
            }
            set
            {
                SetValue(WidthProperty, value);
            }
        }

        public double Height
        {
            get
            {
                return (double)GetValue(HeightProperty);
            }
            set
            {
                SetValue(HeightProperty, value);
            }
        }

        #endregion

    }
}
