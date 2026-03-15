/* Copyright © Northwoods Software Corporation, 2008-2015. All Rights Reserved. */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Linq;
using Northwoods.GoXam;
using Northwoods.GoXam.Model;

namespace DynamicPorts {
  public partial class DynamicPorts : UserControl {
    public DynamicPorts() {
      InitializeComponent();

      // because we cannot data-bind the Route.Points property,
      // we use a custom PartManager to read/write the Wire.Points data property
      myDiagram.PartManager = new CustomPartManager();

      var model = new CustomModel();
      // initialize it from data in an XML file that is an embedded resource
      String xml = Demo.MainPage.Instance.LoadText("DynamicPorts", "xml");
      // set the Route.Points after nodes have been built and the layout has finished
      myDiagram.LayoutCompleted += UpdateRoutes;
      model.Load<Unit, Wire>(XElement.Parse(xml), "Unit", "Wire");
      model.Modifiable = true;
      model.HasUndoManager = true;
      myDiagram.Model = model;

      myDiagram.MouseRightButtonUp += Port_RightButtonUp;
    }

    Random rand = new Random();
    String[] Colors = new String[] { "Black", "Red", "Orange", "Green", "Blue", "Purple", "Violet" };

    // Each of the four buttons is very similar --
    // the only difference is on which side the new port should go
    private void Button_Add(object sender, RoutedEventArgs e) {
      Node node = myDiagram.SelectedNode;
      if (node != null) {
        Unit unit = node.Data as Unit;
        if (unit != null) {
          String side = (String)((FrameworkElement)sender).Tag;
          String prefix = "";
          switch (side) {
            case "Left": prefix = "L"; break;
            case "Right": prefix = "R"; break;
            case "Top": prefix = "T"; break;
            case "Bottom": prefix = "B"; break;
          }
          // find unique socket name for the given side
          int i = 1;
          while (unit.FindSocket(prefix + i.ToString()) != null) i++;
          // modify the model
          myDiagram.StartTransaction("Add Socket");
          unit.AddSocket(side, prefix + i.ToString(), Colors[rand.Next(Colors.Length)]);
          myDiagram.CommitTransaction("Add Socket");
        }
      }
    }

    // If the element at the mouse point is a port, remove it from its Node
    private void Port_RightButtonUp(object sender, MouseButtonEventArgs e) {
      FrameworkElement elt = myDiagram.Panel.FindElementAt<FrameworkElement>(myDiagram.LastMousePointInModel,
                                                x => x as FrameworkElement, x => true, SearchLayers.Nodes);
      if (elt == null) return;
      String name = Node.GetPortId(elt);
      if (name == null) return;
      String side = elt.Tag as String;
      if (side == null) return;
      Node node = Part.FindAncestor<Node>(elt);
      if (node != null) {
        Unit u = node.Data as Unit;
        if (u != null) {
          myDiagram.StartTransaction("Remove Socket");
          RemoveConnectedLinks(node, name);
          u.RemoveSocket(u.FindSocket(name));
          myDiagram.CommitTransaction("Remove Socket");
        }
      }
    }

    private void RemoveConnectedLinks(Node node, String portname) {
      var model = myDiagram.Model as CustomModel;
      if (model == null) return;
      foreach (Link l in node.FindLinksConnectedWithPort(portname)) {
        model.RemoveLink(l.Data as Wire);
      }
    }

    // save and load the model data as XML, visible in the "Saved" tab of the Demo
    private void Save_Click(object sender, RoutedEventArgs e) {
      var model = myDiagram.Model as CustomModel;
      if (model == null) return;
      // copy the Route.Points into each Transition data
      foreach (Link link in myDiagram.Links) {
        Wire wire = link.Data as Wire;
        if (wire != null) {
          wire.Points = new List<Point>(link.Route.Points);
        }
      }
      XElement root = model.Save<Unit, Wire>("Diagram", "Unit", "Wire");
      Demo.MainPage.Instance.SavedXML = root.ToString();
      LoadButton.IsEnabled = true;
      model.IsModified = false;
    }

    private void Load_Click(object sender, RoutedEventArgs e) {
      var model = myDiagram.Model as CustomModel;
      if (model == null) return;
      try {
        XElement root = XElement.Parse(Demo.MainPage.Instance.SavedXML);
        // set the Route.Points after nodes have been built and the layout has finished
        myDiagram.LayoutCompleted += UpdateRoutes;
        // tell the CustomPartManager that we're loading
        myDiagram.PartManager.UpdatesRouteDataPoints = false;
        model.Load<Unit, Wire>(root, "Unit", "Wire");
      } catch (Exception ex) {
        MessageBox.Show(ex.ToString());
      }
      model.IsModified = false;
    }

    // only use the saved route points after the layout has completed,
    // because links will get the default routing
    private void UpdateRoutes(object sender, DiagramEventArgs e) {
      // just set the Route points once per Load
      myDiagram.LayoutCompleted -= UpdateRoutes;
      foreach (Link link in myDiagram.Links) {
        Wire wire = link.Data as Wire;
        if (wire != null && wire.Points != null && wire.Points.Count() > 1) {
          link.Route.Points = (IList<Point>)wire.Points;
        }
      }
      myDiagram.PartManager.UpdatesRouteDataPoints = true;  // OK for CustomPartManager to update Transition.Points automatically
    }
  }

  public class CustomPartManager : PartManager {
    public CustomPartManager() {
      this.UpdatesRouteDataPoints = true;  // call UpdateRouteDataPoints when Link.Route.Points has changed
    }

    // this supports undo/redo of link route reshaping
    protected override void UpdateRouteDataPoints(Link link) {
      if (!this.UpdatesRouteDataPoints) return;   // in coordination with Load_Click and UpdateRoutes, above
      var data = link.Data as Wire;
      if (data != null) {
        data.Points = new List<Point>(link.Route.Points);
      }
    }
  }

  public class InertListBox : ListBox {
    protected override DependencyObject GetContainerForItemOverride() {
      return new InertListBoxItem();
    }
  }

  public class InertListBoxItem : ListBoxItem {
    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
      base.OnMouseLeftButtonDown(e);
      e.Handled = false;
    }
  }

  public class CustomModel : GraphLinksModel<Unit, String, String, Wire> {
    // When a Unit gets an extra Socket or when a Socket is removed,
    // tell the Diagram.PartManager that some (or all) of the port FrameworkElements
    // in the Node corresponding to a unit may have moved or changed size.
    protected override void HandleNodePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
      base.HandleNodePropertyChanged(sender, e);
      Unit unit = sender as Unit;
      if (unit != null && (e.PropertyName == "AddedSocket" || e.PropertyName == "RemovedSocket")) {
        RaiseChanged(new ModelChangedEventArgs() { Model=this, Change=ModelChange.InvalidateRelationships, Data=unit });
      }
    }
  }


  // Each set of "Sockets" has to be a property that is an ObservableCollection
#if !SILVERLIGHT
  [Serializable]
#endif
  public class Unit : GraphLinksModelNodeData<String> {

    // When a Unit is copied, it needs separate collections of Sockets
    public override object Clone() {
      Unit unit = (Unit)base.Clone();
      unit._LeftSockets = new ObservableCollection<Socket>();
      foreach (Socket s in this.LeftSockets) unit._LeftSockets.Add((Socket)s.Clone());
      unit._RightSockets = new ObservableCollection<Socket>();
      foreach (Socket s in this.RightSockets) unit._RightSockets.Add((Socket)s.Clone());
      unit._TopSockets = new ObservableCollection<Socket>();
      foreach (Socket s in this.TopSockets) unit._TopSockets.Add((Socket)s.Clone());
      unit._BottomSockets = new ObservableCollection<Socket>();
      foreach (Socket s in this.BottomSockets) unit._BottomSockets.Add((Socket)s.Clone());
      // if you add properties that are not supposed to be shared, deal with them here
      return unit;
    }

    // Property change for undo/redo:
    // We treat adding and removing socket as property changes, and
    // there are settable properties of Socket for each socket to handle.
    public override void ChangeDataValue(ModelChangedEventArgs e, bool undo) {
      // Data might be either a Unit or a Socket
      Socket sock = e.Data as Socket;
      if (sock != null) {  // if it's a Socket, let it handle undo/redo changes
        sock.ChangeDataValue(e, undo);
      } else {
        // assume we're dealing with a change to this Unit
        switch (e.PropertyName) {
          case "AddedSocket":
            sock = e.OldValue as Socket;
            if (undo)
              RemoveSocket(sock);
            else
              InsertSocket(sock);
            break;
          case "RemovedSocket":
            sock = e.OldValue as Socket;
            if (undo)
              InsertSocket(sock);
            else
              RemoveSocket(sock);
            break;
          // if you add undo-able properties to Unit, handle them here
          default:
            base.ChangeDataValue(e, undo);
            break;
        }
      }
    }

    // write the Sockets as child elements
    public override XElement MakeXElement(XName n) {
      XElement e = base.MakeXElement(n);
      e.Add(this.LeftSockets.Select(s => s.MakeXElement()));
      e.Add(this.RightSockets.Select(s => s.MakeXElement()));
      e.Add(this.TopSockets.Select(s => s.MakeXElement()));
      e.Add(this.BottomSockets.Select(s => s.MakeXElement()));
      return e;
    }

    // read the child elements as Sockets
    public override void LoadFromXElement(XElement e) {
      base.LoadFromXElement(e);
      foreach (XElement c in e.Elements("Socket")) {
        InsertSocket(new Socket().LoadFromXElement(c));
      }
    }

    public IEnumerable<Socket> LeftSockets {
      get { return _LeftSockets; }
    }
    private ObservableCollection<Socket> _LeftSockets = new ObservableCollection<Socket>();

    public IEnumerable<Socket> RightSockets {
      get { return _RightSockets; }
    }
    private ObservableCollection<Socket> _RightSockets = new ObservableCollection<Socket>();

    public IEnumerable<Socket> TopSockets {
      get { return _TopSockets; }
    }
    private ObservableCollection<Socket> _TopSockets = new ObservableCollection<Socket>();

    public IEnumerable<Socket> BottomSockets {
      get { return _BottomSockets; }
    }
    private ObservableCollection<Socket> _BottomSockets = new ObservableCollection<Socket>();


    // used to find whether a Socket exists for a name
    public Socket FindSocket(String name) {
      int i = IndexOf(_LeftSockets, name);
      if (i >= 0) return _LeftSockets[i];
      i = IndexOf(_RightSockets, name);
      if (i >= 0) return _RightSockets[i];
      i = IndexOf(_TopSockets, name);
      if (i >= 0) return _TopSockets[i];
      i = IndexOf(_BottomSockets, name);
      if (i >= 0) return _BottomSockets[i];
      return null;
    }

    // create a new Socket
    public void AddSocket(String side, String name, String color) {
      Add(Find(side), new Socket() { Unit = this, Side = side, Index = Find(side).Count, Name = name, Color = color });
    }

    // insert an existing Socket
    public void InsertSocket(Socket sock) {
      Add(Find(sock.Side), sock);
    }

    // remove an existing Socket
    public void RemoveSocket(Socket sock) {
      Remove(Find(sock.Side), sock.Name);
    }

    public ObservableCollection<Socket> Find(String side) {
      switch (side) {
        case "Left": return _LeftSockets;
        case "Right": return _RightSockets;
        case "Top": return _TopSockets;
        case "Bottom": return _BottomSockets;
      }
      return null;
    }
    private void Add(ObservableCollection<Socket> socks, Socket s) {
      // don't do anything if it's already there
      if (socks.Contains(s)) return;
      // update the collection
      socks.Insert(s.Index, s);
      int n = socks.Count;
      for (int j = 0; j < n; j++) {
        socks[j].Index = j;  // always update the Socket.Index
      }
      // notify about the change
      RaisePropertyChanged("AddedSocket", s, null);
    }
    private int IndexOf(ObservableCollection<Socket> socks, String name) {
      for (int i = 0; i < socks.Count; i++) {
        if (socks[i].Name == name) return i;
      }
      return -1;
    }
    private void Remove(ObservableCollection<Socket> socks, String name) {
      int i = IndexOf(socks, name);
      if (i >= 0) {  // don't do anything unless it's actually removed
        Socket s = socks[i];
        // update the collection
        socks.RemoveAt(i);
        // always update the Socket.Index
        for (int j = 0; j < socks.Count; j++) socks[j].Index = j;
        // notify about the change
        RaisePropertyChanged("RemovedSocket", s, null);
      }
    }
  }

#if !SILVERLIGHT
  [Serializable]
#endif
  public class Socket : ICloneable, INotifyPropertyChanged, IChangeDataValue {
    // implement ICloneable for copying
    public Object Clone() {
      return MemberwiseClone() as Socket;
    }

    // implement INotifyPropertyChanged for data-binding
#if !SILVERLIGHT
    [field: NonSerializedAttribute()]
#endif
    public event PropertyChangedEventHandler PropertyChanged;

    protected void RaisePropertyChanged(String pname, Object oldval, Object newval) {
      ModelChangedEventArgs e = new ModelChangedEventArgs(pname, this, oldval, newval);
      // implement INotifyPropertyChanged:
      if (this.PropertyChanged != null) this.PropertyChanged(this, e);
      // implement support for model and undo/redo:
      if (this.Unit != null) this.Unit.OnPropertyChanged(e);
    }

    // implement IChangeDataValue for undo/redo
    public void ChangeDataValue(ModelChangedEventArgs e, bool undo) {
      switch (e.PropertyName) {
        case "Color": this.Color = (String)e.GetValue(undo); break;
        default: throw new NotImplementedException("Socket change: " + e.ToString());
      }
    }

    public XElement MakeXElement() {
      XElement e = new XElement("Socket");
      e.Add(XHelper.Attribute("Name", this.Name, ""));
      e.Add(XHelper.Attribute("Side", this.Side, ""));
      e.Add(XHelper.Attribute("Index", this.Index, 0));
      e.Add(XHelper.Attribute("Color", this.Color, "Black"));
      return e;
    }

    public Socket LoadFromXElement(XElement e) {
      if (e == null) return this;
      this.Name = XHelper.Read("Name", e, "");
      this.Side = XHelper.Read("Side", e, "");
      this.Index = XHelper.Read("Index", e, 0);
      this.Color = XHelper.Read("Color", e, "Black");
      return this;
    }

    // these properties aren't expected to change after initialization
    public Unit Unit { get; set; }  // parent pointer
    public String Name { get; set; }
    public String Side { get; set; }
    public int Index { get; set; }

    // these property may change dynamically, so they implement change notification
    public String Color {
      get { return _Color; }
      set {
        if (_Color != value) {
          String old = _Color;
          _Color = value;
          RaisePropertyChanged("Color", old, value);
        }
      }
    }
    private String _Color = "Black";
  }

#if !SILVERLIGHT
  [Serializable]
#endif
  public class Wire : GraphLinksModelLinkData<String, String> {
    // no additional properties defined
  }


  public class CustomRoute : Route {
    protected override double GetEndSegmentLength(Node node, FrameworkElement port, Spot spot, bool from) {
      double esl = base.GetEndSegmentLength(node, port, spot, from);
      Unit unit = node.Data as Unit;
      if (unit != null) {
        Socket sock = unit.FindSocket(Node.GetPortId(port));
        if (sock != null) {
          var socks = unit.Find(sock.Side);
          Point thispt = node.GetElementPoint(port, from ? GetFromSpot() : GetToSpot());
          Point otherpt = this.Link.GetOtherNode(node).GetElementPoint(this.Link.GetOtherPort(port),
                                                                       from ? GetToSpot() : GetFromSpot());
          if (Math.Abs(thispt.X-otherpt.X) > 20 || Math.Abs(thispt.Y-otherpt.Y) > 20) {
            if (sock.Side == "Top" || sock.Side == "Bottom") {
              if (otherpt.X < thispt.X) {
                return esl + 4 + sock.Index*8;
              } else {
                return esl + (socks.Count-sock.Index-1)*8;
              }
            } else {
              if (otherpt.Y < thispt.Y) {
                return esl + 4 + sock.Index*8;
              } else {
                return esl + (socks.Count-sock.Index-1)*8;
              }
            }
          }
        }
      }
      return esl;
    }

    protected override bool HasCurviness() {
      if (Double.IsNaN(this.Curviness)) return true;
      return base.HasCurviness();
    }

    protected override double ComputeCurviness() {
      if (Double.IsNaN(this.Curviness)) {
        var fromnode = this.Link.FromNode;
        var fromport = this.Link.FromPort;
        var fromspot = GetFromSpot();
        var frompt = fromnode.GetElementPoint(fromport, fromspot);
        var tonode = this.Link.ToNode;
        var toport = this.Link.ToPort;
        var tospot = GetToSpot();
        var topt = tonode.GetElementPoint(toport, tospot);
        if (Math.Abs(frompt.X-topt.X) > 20 || Math.Abs(frompt.Y-topt.Y) > 20) {
          if ((fromspot == Spot.MiddleLeft || fromspot == Spot.MiddleRight) &&
              (tospot == Spot.MiddleLeft || tospot == Spot.MiddleRight)) {
            double fromseglen = GetEndSegmentLength(fromnode, fromport, fromspot, true);
            double toseglen = GetEndSegmentLength(tonode, toport, tospot, false);
            var c = (fromseglen - toseglen)/2;
            if (frompt.X + fromseglen >= topt.X - toseglen) {
              if (frompt.Y < topt.Y) return c;
              if (frompt.Y > topt.Y) return -c;
            }
          } else if ((fromspot == Spot.MiddleTop || fromspot == Spot.MiddleBottom) &&
                     (tospot == Spot.MiddleTop || tospot == Spot.MiddleBottom)) {
            double fromseglen = GetEndSegmentLength(fromnode, fromport, fromspot, true);
            double toseglen = GetEndSegmentLength(tonode, toport, tospot, false);
            var c = (fromseglen - toseglen)/2;
            if (frompt.Y + fromseglen >= topt.Y - toseglen) {
              if (frompt.X < topt.X) return c;
              if (frompt.X > topt.X) return -c;
            }
          }
        }
      }
      return base.ComputeCurviness();
    }
    internal Spot GetFromSpot() {
      Spot s = this.FromSpot;
      if (s.IsDefault) {
        Link link = this.Link;
        if (link != null) {
          FrameworkElement port = link.FromPort;
          if (port != null) {
            s = Node.GetFromSpot(port);  // normally, get Spot from the port
          }
        }
      }
      return s;
    }

    internal Spot GetToSpot() {
      Spot s = this.ToSpot;
      if (s.IsDefault) {
        Link link = this.Link;
        if (link != null) {
          FrameworkElement port = link.ToPort;
          if (port != null) {
            s = Node.GetToSpot(port);  // normally, get Spot from the port
          }
        }
      }
      return s;
    }
  }
}
