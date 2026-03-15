// <copyright file="Layer.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections;
using System.ComponentModel;
using System.Windows.Input;

namespace Syncfusion.Windows.Diagram
{
#if !SyncfusionFramework3_5
    [DesignTimeVisible(false)]
#endif

    /*
    /// <summary>
    /// Represents the Layer.
    /// </summary>
    /// <remarks>
    /// <para>Layer is an uielement that holds nodes and lineconnectors.
    /// </para>
    /// </remarks>
    /// <example>
    /// The following example shows how to create a <see cref="DiagramControl"/> in C#.
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
    ///       HorizontalRuler hruler = new HorizontalRuler();
    ///       View.HorizontalRuler = hruler;
    ///       View.ShowHorizontalGridLine = false;
    ///       View.ShowVerticalGridLine = false;
    ///       VerticalRuler vruler = new VerticalRuler();
    ///       View.VerticalRuler = vruler;
    ///       View.Bounds = new Thickness (0, 0, 1000, 1000);
    ///       View.IsPageEditable = true;
    ///       Node NewClient;
    ///       Node Register;
    ///       NewClient = new Node(Guid.NewGuid(), "NewClient");
    ///       NewClient.Shape = Shapes.FlowChart_Decision;
    ///       NewClient.OffsetX = 40;
    ///       NewClient.OffsetY = 100;
    ///       NewClient.ToolTip = "FlowChart_Decision";
    ///       NewClient.Label = "New Client"
    ///       Register = new Node(Guid.NewGuid(), "Register");
    ///       Register.Shape = Shapes.RoundedSquare;
    ///       Register.OffsetX = 250;
    ///       Register.OffsetY = 100;
    ///       Register.Label = "Register ";
    ///       Register.ToolTip = "RoundedSquare";
    ///       Model.Nodes.Add(NewClient);
    ///       Model.Nodes.Add(Register);
    ///       Layer l = new Layer();
    ///       l.Name = "l1";
    ///       l.Background = Brushes.Black;
    ///       l.Nodes.Add(DataEntry);
    ///       l.Nodes.Add(ClientAccountInfo);
    ///       Model.Layers.Add(l);
    ///    }
    ///    }
    ///    }
    /// </code>
    /// </example>
     */

    public class Layer : Group
    {
        /// <summary>
        /// Stores temprary offsetx.
        /// </summary>
        internal double tOffsetX;

        /// <summary>
        /// Stores temprary offsety.
        /// </summary>
        internal double tOffsetY;

        /// <summary>
        /// Stores nodes reference.
        /// </summary>
        private CollectionExt m_nodes;

        /// <summary>
        /// Stores lines reference.
        /// </summary>
        private CollectionExt m_lines;

        /// <summary>
        /// Gets or sets the nodes ref.
        /// </summary>
        /// <value>The nodes ref for serialization purpose.</value>
        public CollectionExt NodesRef
        {
            get
            {
                return m_nodes;
            }

            set
            {
                m_nodes = value;
            }
        }

        /// <summary>
        /// Gets or sets the lines ref.
        /// </summary>
        /// <value>The lines ref for serialization purpose.</value>
        public CollectionExt LinesRef
        {
            get { return m_lines; }
            set { m_lines = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Layer"/> is visible.
        /// </summary>
        /// <value><c>true</c> if visible; otherwise, <c>false</c>.</value>
        /// <example>
        /// The following example shows how to create a <see cref="DiagramControl"/> in C#.
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
        ///       HorizontalRuler hruler = new HorizontalRuler();
        ///       View.HorizontalRuler = hruler;
        ///       View.ShowHorizontalGridLine = false;
        ///       View.ShowVerticalGridLine = false;
        ///       VerticalRuler vruler = new VerticalRuler();
        ///       View.VerticalRuler = vruler;
        ///       View.Bounds = new Thickness (0, 0, 1000, 1000);
        ///       View.IsPageEditable = true;
        ///       Node NewClient;
        ///       Node Register;
        ///       NewClient = new Node(Guid.NewGuid(), "NewClient");
        ///       NewClient.Shape = Shapes.FlowChart_Decision;
        ///       NewClient.OffsetX = 40;
        ///       NewClient.OffsetY = 100;
        ///       NewClient.ToolTip = "FlowChart_Decision";
        ///       NewClient.Label = "New Client"
        ///       Register = new Node(Guid.NewGuid(), "Register");
        ///       Register.Shape = Shapes.RoundedSquare;
        ///       Register.OffsetX = 250;
        ///       Register.OffsetY = 100;
        ///       Register.Label = "Register ";
        ///       Register.ToolTip = "RoundedSquare";
        ///       Model.Nodes.Add(NewClient);
        ///       Model.Nodes.Add(Register);
        ///       Layer l = new Layer();
        ///       l.Name = "l1";
        ///       l.Background = Brushes.Black;
        ///       l.Nodes.Add(DataEntry);
        ///       l.Nodes.Add(ClientAccountInfo);
        ///       Model.Layers.Add(l);
        ///       Model.Visible = flase;
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        public bool Visible
        {
            get
            {
                return (bool)GetValue(VisibleProperty);
            }

            set
            {
                SetValue(VisibleProperty, value);
            }
        }

        /// <summary>
        /// Identifies the Visible property. This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty VisibleProperty = DependencyProperty.Register("Visible", typeof(bool), typeof(Layer), new UIPropertyMetadata(true, OnVisibilityChanged));

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Layer"/> is active.
        /// </summary>
        /// <value><c>true</c> if active; otherwise, <c>false</c>.</value>
        /// /// <example>
        /// The following example shows how to create a <see cref="DiagramControl"/> in C#.
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
        ///       HorizontalRuler hruler = new HorizontalRuler();
        ///       View.HorizontalRuler = hruler;
        ///       View.ShowHorizontalGridLine = false;
        ///       View.ShowVerticalGridLine = false;
        ///       VerticalRuler vruler = new VerticalRuler();
        ///       View.VerticalRuler = vruler;
        ///       View.Bounds = new Thickness (0, 0, 1000, 1000);
        ///       View.IsPageEditable = true;
        ///       Node NewClient;
        ///       Node Register;
        ///       NewClient = new Node(Guid.NewGuid(), "NewClient");
        ///       NewClient.Shape = Shapes.FlowChart_Decision;
        ///       NewClient.OffsetX = 40;
        ///       NewClient.OffsetY = 100;
        ///       NewClient.ToolTip = "FlowChart_Decision";
        ///       NewClient.Label = "New Client"
        ///       Register = new Node(Guid.NewGuid(), "Register");
        ///       Register.Shape = Shapes.RoundedSquare;
        ///       Register.OffsetX = 250;
        ///       Register.OffsetY = 100;
        ///       Register.Label = "Register ";
        ///       Register.ToolTip = "RoundedSquare";
        ///       Layer l = new Layer();
        ///       l.Name = "l1";
        ///       l.Background = Brushes.Black;
        ///       l.Nodes.Add(DataEntry);
        ///       l.Nodes.Add(ClientAccountInfo);
        ///       Model.Layers.Add(l);
        ///       Model.Active = false;
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        public bool Active
        {
            get
            {
                return (bool)GetValue(ActiveProperty);
            }

            set
            {
                SetValue(ActiveProperty, value);
                if (value == true)
                {
                    Visible = true;
                }
            }
        }

        /// <summary>
        /// Identifies the Acive property. This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty ActiveProperty = DependencyProperty.Register("Active", typeof(bool), typeof(Layer), new UIPropertyMetadata(true, OnActiveChanged));

        /// <summary>
        /// Gets or sets the node collection.
        /// </summary>
        /// <value>The collection of nodes.</value>
        /// /// <example>
        /// The following example shows how to create a <see cref="DiagramControl"/> in C#.
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
        ///       HorizontalRuler hruler = new HorizontalRuler();
        ///       View.HorizontalRuler = hruler;
        ///       View.ShowHorizontalGridLine = false;
        ///       View.ShowVerticalGridLine = false;
        ///       VerticalRuler vruler = new VerticalRuler();
        ///       View.VerticalRuler = vruler;
        ///       View.Bounds = new Thickness (0, 0, 1000, 1000);
        ///       View.IsPageEditable = true;
        ///       Node NewClient;
        ///       Node Register;
        ///       NewClient = new Node(Guid.NewGuid(), "NewClient");
        ///       NewClient.Shape = Shapes.FlowChart_Decision;
        ///       NewClient.OffsetX = 40;
        ///       NewClient.OffsetY = 100;
        ///       NewClient.ToolTip = "FlowChart_Decision";
        ///       NewClient.Label = "New Client"
        ///       Register = new Node(Guid.NewGuid(), "Register");
        ///       Register.Shape = Shapes.RoundedSquare;
        ///       Register.OffsetX = 250;
        ///       Register.OffsetY = 100;
        ///       Register.Label = "Register ";
        ///       Register.ToolTip = "RoundedSquare";
        ///       Model.Nodes.Add(NewClient);
        ///       Model.Nodes.Add(Register);
        ///       Layer l = new Layer();
        ///       l.Name = "l1";
        ///       l.Background = Brushes.Black;
        ///       l.Nodes.Add(DataEntry);
        ///       l.Nodes.Add(ClientAccountInfo);
        ///       Model.Layers.Add(l);
        ///       Model.Visible = flase;
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ObservableCollection<Node> Nodes
        {
            get
            {
                return (ObservableCollection<Node>)GetValue(NodesProperty);
            }

            set
            {
                if (Nodes != null)
                {
                    LayerPropertyChangedEventArgs newEventArgs3 = new LayerPropertyChangedEventArgs("NodeRemoved", Nodes);
                    newEventArgs3.RoutedEvent = Layer.LayerPropertyChangedEvent;
                    this.RaiseEvent(newEventArgs3);
                }

                SetValue(NodesProperty, value);
                if (Nodes != null)
                {
                    Nodes.CollectionChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Nodes_CollectionChanged);
                    Nodes.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Nodes_CollectionChanged);
                }
            }
        }

        /// <summary>
        /// Resets the position.
        /// </summary>
        /// <param name="n">The node to be reset.</param>
        internal void ResetPosition(Node n)
        {
            if (Nodes.IndexOf(n) >= 0)
            {
                if (tOffsetX > n.PxOffsetX)
                {
                    tOffsetX = n.PxOffsetX;
                }

                if (tOffsetY > n.PxOffsetY)
                {
                    tOffsetY = n.PxOffsetY;
                }

                if (tOffsetX + Width < n.PxOffsetX + n.Width)
                {
                    Width = n.PxOffsetX + n.Width - tOffsetX;
                }

                if (tOffsetY + Height < n.PxOffsetY + n.Height)
                {
                    Height = n.PxOffsetY + n.Height - tOffsetY;
                }
            }
        }

        /// <summary>
        /// Resets the position.
        /// </summary>
        /// <param name="l">Line to be reset.</param>
        internal void ResetPosition(LineConnector l)
        {
            if (Lines.IndexOf(l) >= 0)
            {
                if (tOffsetX > l.minx)
                {
                    tOffsetX = l.minx;
                }

                if (tOffsetY > l.miny)
                {
                    tOffsetY = l.miny;
                }

                if (tOffsetX + Width < l.minx)
                {
                    Width = l.minx - tOffsetX;
                }

                if (tOffsetY + Height < l.miny)
                {
                    Height = l.miny - tOffsetY;
                }
            }
        }

        /// <summary>
        /// Resets the position.
        /// </summary>
        internal void ResetPosition()
        {
            double minx = 1e100d, miny = 1e100d;
            double maxx = -1e100d, maxy = -1e100d;
            int minz = 99999;
            bool flag = false;
            if (Nodes != null && Nodes.Count > 0)
            {
                foreach (Node n in Nodes)
                {
                    if (Panel.GetZIndex(n) < minz)
                    {
                        minz = Panel.GetZIndex(n);
                    }

                    double lx = n.PxOffsetX;// MeasureUnitsConverter.ToPixels(n.LogicalOffsetX, n.MeasurementUnits);
                    double ly = n.PxOffsetY;// MeasureUnitsConverter.ToPixels(n.LogicalOffsetY, n.MeasurementUnits);
                    if ((this.Parent as DiagramPage).Children.Contains(n))
                    {
                        if (minx > lx)
                        {
                            minx = lx;
                        }

                        if (miny > ly)
                        {
                            miny = ly;
                        }

                        if (maxx < lx + n.Width)
                        {
                            maxx = lx + n.Width;
                        }

                        if (maxy < ly + n.Height)
                        {
                            maxy = ly + n.Height;
                        }

                        flag = true;
                    }
                }

                if (flag)
                {
                    this.tOffsetX = minx;
                    this.tOffsetY = miny;
                    this.Width = maxx - this.tOffsetX;
                    this.Height = maxy - this.tOffsetY;
                }
            }

            if (Lines != null && Lines.Count > 0)
            {
                foreach (LineConnector l in Lines)
                {
                    LineConnector lc = l;
                    if (lc.HeadNode != null)
                    {
                        if (!this.Nodes.Contains(lc.HeadNode as Node))
                        {
                            continue;
                        }
                    }

                    if (lc.TailNode != null)
                    {
                        if (!this.Nodes.Contains(lc.TailNode as Node))
                        {
                            continue;
                        }
                    }

                    if (l.ConnectorType == ConnectorType.Bezier || l.ConnectorType == ConnectorType.Arc)
                    {
                        if (Panel.GetZIndex(l) < minz)
                        {
                            minz = Panel.GetZIndex(l);
                        }

                        Rect r = l.ConnectorPathGeometry.Bounds;
                        if (r != Rect.Empty)
                        {
                            if (minx > r.Left)
                            {
                                minx = r.Left;
                            }

                            if (miny > r.Top)
                            {
                                miny = r.Top;
                            }

                            if (maxx < r.Right)
                            {
                                maxx = r.Right;
                            }

                            if (maxy < r.Bottom)
                            {
                                maxy = r.Bottom;
                            }
                        }
                    }
                    else
                    {
                        if (Panel.GetZIndex(l) < minz)
                        {
                            minz = Panel.GetZIndex(l);
                        }

                        if (minx > l.minx)
                        {
                            minx = l.minx;
                        }

                        if (miny > l.miny)
                        {
                            miny = l.miny;
                        }

                        if (maxx < l.maxx)
                        {
                            maxx = l.maxx;
                        }

                        if (maxy < l.maxy)
                        {
                            maxy = l.maxy;
                        }
                    }

                    flag = true;
                }

                if (flag)
                {
                    this.tOffsetX = minx;
                    this.tOffsetY = miny;
                    Width = maxx - tOffsetX;
                    Height = maxy - tOffsetY;
                }
            }

            if (!flag)
            {
                Height = 0; 
                Width = 0;
            }
            else
            {
                if (minz < 1)
                {
                    Panel.SetZIndex(this, 0);
                    foreach (Node n in Nodes)
                    {
                        int i = Panel.GetZIndex(n);
                        Panel.SetZIndex(n, i + 1);
                    }
                }
                else
                {
                    Panel.SetZIndex(this, minz - 1);
                }
            }
        }

        /// <summary>
        /// Handles the CollectionChanged event of the Nodes control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void Nodes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            LayerPropertyChangedEventArgs evt = new LayerPropertyChangedEventArgs(String.Empty, true);
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    evt = new LayerPropertyChangedEventArgs("NodeAdded", e.NewItems);
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Remove:
                    evt = new LayerPropertyChangedEventArgs("NodeRemoved", e.OldItems);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    LayerPropertyChangedEventArgs temp1 = new LayerPropertyChangedEventArgs("NodeRA", true);
                    temp1.RoutedEvent = Layer.LayerPropertyChangedEvent;
                    this.RaiseEvent(temp1);

                    LayerPropertyChangedEventArgs temp2 = new LayerPropertyChangedEventArgs("NodeRemoved", e.OldItems);
                    temp2.RoutedEvent = Layer.LayerPropertyChangedEvent;
                    this.RaiseEvent(temp2);
                    evt = new LayerPropertyChangedEventArgs("NodeAdded", e.NewItems);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
            }

            evt.RoutedEvent = Layer.LayerPropertyChangedEvent;
            this.RaiseEvent(evt);
            EventHandler ev = new EventHandler(Layer_LayoutUpdated);
            EventArgs ea = new EventArgs();
            ev.Invoke(this, ea);
        }

        /// <summary>
        /// Handles the CollectionChanged event of the Lines control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void Lines_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            LayerPropertyChangedEventArgs evt = new LayerPropertyChangedEventArgs(String.Empty, true);
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    evt = new LayerPropertyChangedEventArgs("LineAdded", e.NewItems);
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Remove:
                    evt = new LayerPropertyChangedEventArgs("LineRemoved", e.OldItems);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    LayerPropertyChangedEventArgs temp1 = new LayerPropertyChangedEventArgs("LineRA", true);
                    temp1.RoutedEvent = Layer.LayerPropertyChangedEvent;
                    this.RaiseEvent(temp1);

                    LayerPropertyChangedEventArgs temp2 = new LayerPropertyChangedEventArgs("LineRemoved", e.OldItems);
                    temp2.RoutedEvent = Layer.LayerPropertyChangedEvent;
                    this.RaiseEvent(temp2);
                    evt = new LayerPropertyChangedEventArgs("LineAdded", e.NewItems);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
            }

            evt.RoutedEvent = Layer.LayerPropertyChangedEvent;
            this.RaiseEvent(evt);
            EventHandler ev = new EventHandler(Layer_LayoutUpdated);
            EventArgs ea = new EventArgs();
            ev.Invoke(this, ea);
        }

        /// <summary>
        /// Identifies the Node collection. This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty NodesProperty = DependencyProperty.Register("Nodes", typeof(ObservableCollection<Node>), typeof(Layer), new PropertyMetadata(OnNodesChanged));

        /// <summary>
        /// Gets or sets the lines.
        /// </summary>
        /// <value>The lines.</value>
        /// /// <example>
        /// The following example shows how to create a <see cref="DiagramControl"/> in C#.
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
        ///       HorizontalRuler hruler = new HorizontalRuler();
        ///       View.HorizontalRuler = hruler;
        ///       View.ShowHorizontalGridLine = false;
        ///       View.ShowVerticalGridLine = false;
        ///       VerticalRuler vruler = new VerticalRuler();
        ///       View.VerticalRuler = vruler;
        ///       View.Bounds = new Thickness (0, 0, 1000, 1000);
        ///       View.IsPageEditable = true;
        ///       Node NewClient;
        ///       Node Register;
        ///       NewClient = new Node(Guid.NewGuid(), "NewClient");
        ///       NewClient.Shape = Shapes.FlowChart_Decision;
        ///       NewClient.OffsetX = 40;
        ///       NewClient.OffsetY = 100;
        ///       NewClient.ToolTip = "FlowChart_Decision";
        ///       NewClient.Label = "New Client"
        ///       Register = new Node(Guid.NewGuid(), "Register");
        ///       Register.Shape = Shapes.RoundedSquare;
        ///       Register.OffsetX = 250;
        ///       Register.OffsetY = 100;
        ///       Register.Label = "Register ";
        ///       Register.ToolTip = "RoundedSquare";
        ///       Model.Nodes.Add(NewClient);
        ///       Model.Nodes.Add(Register);
        ///       LineConnector o1 = new LineConnector();
        ///       o1.ConnectorType = ConnectorType.Straight;
        ///       o1.TailNode = Register;
        ///       o1.HeadNode = NewClient;
        ///       o1.HeadDecoratorShape = DecoratorShape.Circle;
        ///       o1.TailDecoratorShape = DecoratorShape.Circle;
        ///       Model.Connections.Add(o1);
        ///       Layer l = new Layer();
        ///       l.Name = "l1";
        ///       l.Background = Brushes.Black;
        ///       l.Lines.Add(o1);
        ///       Model.Layers.Add(l);
        ///       Model.Visible = flase;
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ObservableCollection<LineConnector> Lines
        {
            get
            {
                return (ObservableCollection<LineConnector>)GetValue(LinesProperty);
            }

            set
            {
                if (Lines != null)
                {
                    LayerPropertyChangedEventArgs newEventArgs3 = new LayerPropertyChangedEventArgs("LineRemoved", Lines);
                    newEventArgs3.RoutedEvent = Layer.LayerPropertyChangedEvent;
                    this.RaiseEvent(newEventArgs3);
                }

                SetValue(LinesProperty, value);
                if (Lines != null)
                {
                    Lines.CollectionChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Lines_CollectionChanged);
                    Lines.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Lines_CollectionChanged);
                }
            }
        }

        /// <summary>
        /// Indentifies the Line collection. This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty LinesProperty = DependencyProperty.Register("Lines", typeof(ObservableCollection<LineConnector>), typeof(Layer), new PropertyMetadata(OnLinesChanged));

        /// <summary>
        /// Register routed event for layer property changed
        /// </summary>
        internal static readonly RoutedEvent LayerPropertyChangedEvent = EventManager.RegisterRoutedEvent(
        "LayerPropertyChanged", RoutingStrategy.Bubble, typeof(LayerPropertyChangedEventHandler), typeof(DiagramView));

        /// <summary>
        /// Occurs when [layer property changed].
        /// </summary>
        internal event LayerPropertyChangedEventHandler LayerPropertyChanged
        {
            add { AddHandler(LayerPropertyChangedEvent, value); }
            remove { RemoveHandler(LayerPropertyChangedEvent, value); }
        }

        /// <summary>
        ///  Initializes static members of the <see cref="Layer"/> class.
        /// </summary>
        static Layer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Layer), new FrameworkPropertyMetadata(typeof(Layer)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Layer"/> class.
        /// </summary>
        public Layer()
        {
            if (Nodes == null)
            {
                Nodes = new ObservableCollection<Node>();
            }

            Nodes.CollectionChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Nodes_CollectionChanged);
            Nodes.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Nodes_CollectionChanged);

            if (Lines == null)
            {
                Lines = new ObservableCollection<LineConnector>();
            }

            Lines.CollectionChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Lines_CollectionChanged);
            Lines.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Lines_CollectionChanged);
            this.LayoutUpdated += new EventHandler(Layer_LayoutUpdated);
        }

        /// <summary>
        /// Handles the LayoutUpdated event of the Layer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        internal void Layer_LayoutUpdated(object sender, EventArgs e)
        {
            if (this.Parent != null)
            {
                ResetPosition();

                //double x = 0, y = 0;
                //try
                //{
                //    x = (this.Parent as DiagramPage).ConstMinX;
                //    y = (this.Parent as DiagramPage).ConstMinY;
                //}
                //catch
                //{
                //}

                this.PxOffsetX = tOffsetX;
                this.PxOffsetY = tOffsetY;
                this.Arrange(new Rect(PxOffsetX, PxOffsetY, Width, Height));
            }
        }

        /// <summary>
        /// Called when [nodes changed].
        /// </summary>
        /// <param name="d">The DependencyObject.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs<see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnNodesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Called when [lines changed].
        /// </summary>
        /// <param name="d">The DependencyObject.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs<see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnLinesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Called when [visibility changed].
        /// </summary>
        /// <param name="d">The DependencyObject.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs<see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue == false)
            {
                (d as Layer).Visibility = Visibility.Hidden;
            }
            else
            {
                (d as Layer).Visibility = Visibility.Visible;
            }

            LayerPropertyChangedEventArgs newEventArgs3 = new LayerPropertyChangedEventArgs("Visible", (bool)e.OldValue);
            newEventArgs3.RoutedEvent = Layer.LayerPropertyChangedEvent;
            (d as Layer).RaiseEvent(newEventArgs3);
        }

        /// <summary>
        /// Called when [active changed].
        /// </summary>
        /// <param name="d">The DependencyObject.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs<see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LayerPropertyChangedEventArgs newEventArgs3 = new LayerPropertyChangedEventArgs("Active", (bool)e.OldValue);
            newEventArgs3.RoutedEvent = Layer.LayerPropertyChangedEvent;
            (d as Layer).RaiseEvent(newEventArgs3);
        }
    }
}
