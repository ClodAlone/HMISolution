// <copyright file="PortStyle.cs" company="Syncfusion">
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
using System.Windows;
using System.Windows.Media;

namespace Syncfusion.Windows.Diagram
{
    /// <summary>
    /// Represents customizable options  for the <see cref="ConnectionPort"/>.
    /// </summary>
    public class PortStyle : DependencyObject
    {
        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="PortStyle"/> class.
        /// </summary>
        public PortStyle()
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the fill. This is a dependency property.
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
        ///        Node n = new Node(Guid.NewGuid(), "Start");
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
        ///        //Define a Custom port for the node.
        ///        ConnectionPort port = new ConnectionPort();
        ///        port.Node=n;
        ///        port.Left=75;
        ///        port.Top=10;
        ///        port.PortShape = PortShapes.Arrow;
        ///        //Specifies the port style.
        ///        port.PortStyle.Fill = Brushes.Red;
        ///        port.PortStyle.Stroke = Brushes.Orange;
        ///        port.PortStyle.StrokeThickness = 2;
        ///        n.Ports.Add(port);
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }

        /// <summary>
        /// Gets or sets the stroke thickness. This is a dependency property.
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
        ///        Node n = new Node(Guid.NewGuid(), "Start");
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
        ///        //Define a Custom port for the node.
        ///        ConnectionPort port = new ConnectionPort();
        ///        port.Node=n;
        ///        port.Left=75;
        ///        port.Top=10;
        ///        port.PortShape = PortShapes.Arrow;
        ///        //Specifies the port style.
        ///        port.PortStyle.Fill = Brushes.Red;
        ///        port.PortStyle.Stroke = Brushes.Orange;
        ///        port.PortStyle.StrokeThickness = 2;
        ///        n.Ports.Add(port);
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        /// <summary>
        /// Gets or sets the stroke.This is a dependency property.
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
        ///        Node n = new Node(Guid.NewGuid(), "Start");
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
        ///        //Define a Custom port for the node.
        ///        ConnectionPort port = new ConnectionPort();
        ///        port.Node=n;
        ///        port.Left=75;
        ///        port.Top=10;
        ///        port.PortShape = PortShapes.Arrow;
        ///        //Specifies the port style.
        ///        port.PortStyle.Fill = Brushes.Red;
        ///        port.PortStyle.Stroke = Brushes.Orange;
        ///        port.PortStyle.StrokeThickness = 2;
        ///        n.Ports.Add(port);
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the stroke start line cap.This is a dependency property.
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
        ///        Node n = new Node(Guid.NewGuid(), "Start");
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
        ///        //Define a Custom port for the node.
        ///        ConnectionPort port = new ConnectionPort();
        ///        port.Node=n;
        ///        port.Left=75;
        ///        port.Top=10;
        ///        port.PortShape = PortShapes.Arrow;
        ///        //Specifies the port style.
        ///        port.PortStyle.Fill = Brushes.Red;
        ///        port.PortStyle.Stroke = Brushes.Orange;
        ///        port.PortStyle.StrokeStartLineCap = PenLineCap.Round;
        ///        n.Ports.Add(port);
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        public PenLineCap StrokeStartLineCap
        {
            get { return (PenLineCap)GetValue(StrokeStartLineCapProperty); }
            set { SetValue(StrokeStartLineCapProperty, value); }
        }

        /// <summary>
        /// Gets or sets the stroke end line cap. This is a dependency property.
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
        ///        Node n = new Node(Guid.NewGuid(), "Start");
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
        ///        //Define a Custom port for the node.
        ///        ConnectionPort port = new ConnectionPort();
        ///        port.Node=n;
        ///        port.Left=75;
        ///        port.Top=10;
        ///        port.PortShape = PortShapes.Arrow;
        ///        //Specifies the port style.
        ///        port.PortStyle.Fill = Brushes.Red;
        ///        port.PortStyle.Stroke = Brushes.Orange;
        ///        port.PortStyle.StrokeEndLineCap = PenLineCap.Triangle;
        ///        n.Ports.Add(port);
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        public PenLineCap StrokeEndLineCap
        {
            get { return (PenLineCap)GetValue(StrokeEndLineCapProperty); }
            set { SetValue(StrokeEndLineCapProperty, value); }
        }

        /// <summary>
        /// Gets or sets the stroke line join.This is a dependency property.
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
        ///        Node n = new Node(Guid.NewGuid(), "Start");
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
        ///        //Define a Custom port for the node.
        ///        ConnectionPort port = new ConnectionPort();
        ///        port.Node=n;
        ///        port.Left=75;
        ///        port.Top=10;
        ///        port.PortShape = PortShapes.Arrow;
        ///        //Specifies the port style.
        ///        port.PortStyle.Fill = Brushes.Red;
        ///        port.PortStyle.Stroke = Brushes.Orange;
        ///        port.PortStyle.StrokeLineJoin = PenLineJoin.Miter;
        ///        n.Ports.Add(port);
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        public PenLineJoin StrokeLineJoin
        {
            get { return (PenLineJoin)GetValue(StrokeLineJoinProperty); }
            set { SetValue(StrokeLineJoinProperty, value); }
        }

        #endregion

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Occurs when [property changed].
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="name">The name of the property which has changed.</param>
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion

        #region DP's
       
        /// <summary>
        /// Identifies the Fill dependency property.
        /// </summary>
        public static readonly DependencyProperty FillProperty = DependencyProperty.Register("Fill", typeof(Brush), typeof(PortStyle), new FrameworkPropertyMetadata(Brushes.Gray));

        /// <summary>
        /// Identifies the Stroke dependency property.
        /// </summary>
        public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register("Stroke", typeof(Brush), typeof(PortStyle), new FrameworkPropertyMetadata(Brushes.Gray));

        /// <summary>
        /// Identifies the StrokeStartLineCap dependency property.
        /// </summary>
        public static readonly DependencyProperty StrokeStartLineCapProperty = DependencyProperty.Register("StrokeStartLineCap", typeof(PenLineCap), typeof(PortStyle), new FrameworkPropertyMetadata(PenLineCap.Round));
     
        /// <summary>
        /// Identifies the StrokeEndLineCap dependency property.
        /// </summary>
        public static readonly DependencyProperty StrokeEndLineCapProperty = DependencyProperty.Register("StrokeEndLineCap", typeof(PenLineCap), typeof(PortStyle), new FrameworkPropertyMetadata(PenLineCap.Round));
      
        /// <summary>
        /// Identifies the StrokeLineJoin dependency property.
        /// </summary>
        public static readonly DependencyProperty StrokeLineJoinProperty = DependencyProperty.Register("StrokeLineJoin", typeof(PenLineJoin), typeof(PortStyle), new FrameworkPropertyMetadata(PenLineJoin.Round));
       
        /// <summary>
        /// Identifies the StrokeThickness dependency property.
        /// </summary>
        public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register("StrokeThickness", typeof(double), typeof(PortStyle), new FrameworkPropertyMetadata(2d));

        #endregion
    }
}
