/* Copyright © Northwoods Software Corporation, 2008-2015. All Rights Reserved. */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml.Linq;
using Northwoods.GoXam;
using Northwoods.GoXam.Model;

namespace LogicCircuit {

  public partial class LogicCircuit : UserControl {

    public LogicCircuit() {
      InitializeComponent();

      // create a simple model of GateData and WireData (defined below)
      var model = new GraphLinksModel<GateData, String, String, WireData>();
      // tell the model how to choose a data template for the node
      model.NodeCategoryPath = "GateType";
      // initialize it from data in an XML file that is an embedded resource
      String xml = Demo.MainPage.Instance.LoadText("LogicCircuit", "xml");
      model.Load<GateData, WireData>(XElement.Parse(xml), "Gate", "Wire");
      model.Modifiable = true;  // let the user modify the graph
      model.HasUndoManager = true;  // support undo/redo
      myDiagram.Model = model;

      myDiagram.AllowDrop = true;

      // the Palette has the same kind of model; show one of each kind of node
      var paletteModel = new GraphLinksModel<GateData, String, String, WireData>();
      // tell the model how to choose a data template for the node
      paletteModel.NodeCategoryPath = "GateType";
      // here we create the node data in code rather than loading it from a file
      paletteModel.NodesSource = new List<GateData>() {
        new GateData() { Figure=NodeFigure.Circle, GateType="Input", Key="Input"},
        new GateData() { Figure=NodeFigure.Rectangle, GateType="Output", Key="Output", Value=true},
        new GateData() { Figure=NodeFigure.AndGate, GateType="TwoInOneOut", Key="And"},
        new GateData() { Figure=NodeFigure.OrGate, GateType="TwoInOneOutCurved", Key="Or"},
        new GateData() { Figure=NodeFigure.XorGate, GateType="TwoInOneOutCurved", Key="Xor"},
        new GateData() { Figure=NodeFigure.Inverter, GateType="OneInOneOut", Key="Not"},
        new GateData() { Figure=NodeFigure.NandGate, GateType="TwoInOneOut", Key="Nand"},
        new GateData() { Figure=NodeFigure.NorGate, GateType="TwoInOneOutCurved", Key="Nor"},
        new GateData() { Figure=NodeFigure.XnorGate, GateType="TwoInOneOutCurved", Key="Xnor"},
      };
      paletteModel.LinksSource = new List<WireData>();
      myPalette.Model = paletteModel;

      // Update myDiagram 4 times each second
      DispatcherTimer timer = new DispatcherTimer();
      timer.Interval = TimeSpan.FromMilliseconds(250);
      timer.Tick += UpdateValues;
      timer.Start();
    }

    // This circuit simulator is an extremely simple implementation
    private void UpdateValues(object sender, EventArgs e) {
      if (myDiagram.Model == null) return;
      // Don't record any node Value changes in the UndoManager
      bool oldundo = myDiagram.Model.SkipsUndoManager;
      myDiagram.Model.SkipsUndoManager = true;

      // Finds all starter nodes; one must start at these nodes and navigate through the graph
      List<Node> nodes = new List<Node>();
      foreach (Node n in myDiagram.Nodes) {
        GateData d = n.Data as GateData;
        if (d != null && d.GateType == "Input") {
          nodes.Add(n);
        }
      }
      // Updates the current collection of nodes until that collection is empty
      // (this occurs when all Nodes have been updated and we are at the end of the Diagram);
      // stops updating if InvalidOperationException is thrown
      try {
        // UpdateCollection will modify the collection to hold the nodes to execute next
        while (nodes.Count > 0) { UpdateCollection(nodes); }
      } catch (InvalidOperationException) {
      }
      // Once all Parts have been updated,
      // set their Tag properties to null so they can be updated again
      foreach (Node n in myDiagram.Nodes) {
        n.Tag = null;
      }

      // restore support for undo/redo
      myDiagram.Model.SkipsUndoManager = oldundo;
    }

    // When updating, we will need to update the links leading out of every given node
    // and update the collection so that it only contains those nodes
    // that must likewise be updated next time this method is called.
    private void UpdateCollection(List<Node> nodes) {
      List<Node> newnodes = new List<Node>();
      foreach (Node n in nodes) {
        GateData data = n.Data as GateData;
        // One of the starter nodes might have had its value changed by the user.
        // So, update the values of the links leading out of it to reflect that change.
        if (data != null && data.GateType == "Input") {
          foreach (Link l in n.LinksOutOf) {
            WireData d = l.Data as WireData;
            if (d != null) d.Value = data.Value;
          }
        }

        foreach (Node node in n.NodesOutOf) {
          GateData outdata = node.Data as GateData;
          // ignore nodes already "visited"
          if (node.Tag == null && outdata != null) {
            node.Tag = node;  // declare "visited"
            newnodes.Add(node);
            // Checks that the node has the correct number of inputs
            int numinto = node.LinksInto.Count();
            if ((numinto == 1 && outdata.GateType == "OneInOneOut") ||
                (numinto == 2 && outdata.GateType == "TwoInOneOut") ||
                (numinto == 2 && outdata.GateType == "TwoInOneOutCurved") ||
                (numinto == 1 && outdata.GateType == "Output")) {
              Link[] linksInto = node.LinksInto.ToArray();
              WireData link1 = linksInto[0].Data as WireData;
              WireData link2 = null;
              if (numinto > 1) link2 = linksInto[1].Data as WireData;
              switch (outdata.Figure) {
                // Sets new Value depending on the values of the 
                // links leading into it and the kind of NodeFigure
                case NodeFigure.OrGate:
                  outdata.Value = link1.Value | link2.Value;
                  break;
                case NodeFigure.NorGate:
                  outdata.Value = !(link1.Value | link2.Value);
                  break;
                case NodeFigure.AndGate:
                  outdata.Value = link1.Value & link2.Value;
                  break;
                case NodeFigure.NandGate:
                  outdata.Value = !(link1.Value & link2.Value);
                  break;
                case NodeFigure.XorGate:
                  outdata.Value = link1.Value ^ link2.Value;
                  break;
                case NodeFigure.XnorGate:
                  outdata.Value = !(link1.Value ^ link2.Value);
                  break;
                case NodeFigure.Inverter:
                  outdata.Value = !link1.Value;
                  break;
                default:
                  outdata.Value = link1.Value;
                  break;
              }
              // Once the value of a Node has been updated,
              // set the values of the links leading out of it
              foreach (Link outLink in node.LinksOutOf) {
                WireData d = outLink.Data as WireData;
                if (d != null) d.Value = outdata.Value;
              }
            } else {
              // If the Node has the incorrect number of inputs, stop updating
              throw new InvalidOperationException();
            }
          }
        }
      }
      // modify the collection to reflect new nodes that need to be executed
      nodes.Clear();
      nodes.AddRange(newnodes);
    }


    // handle a mouse-down on an input node: toggle its Value if it's a double-click
    private void StartNodeDoubleClick(object sender, MouseButtonEventArgs e) {
      // Only executes logic if you have double-clicked.
      if (DiagramPanel.IsDoubleClick(e)) {
        myDiagram.StartTransaction("Toggle");
        // When you double-click, toggles value of Input node
        Node input = Part.FindAncestor<Node>(sender as UIElement);
        if (input != null) {
          GateData d = input.Data as GateData;
          if (d != null && d.GateType == "Input") {
            d.Value = !d.Value;
          }
        }
        myDiagram.CommitTransaction("Toggle");
      }
    }


    // save and load the model data as XML, visible in the "Saved" tab of the Demo
    private void Save_Click(object sender, RoutedEventArgs e) {
      var model = myDiagram.Model as GraphLinksModel<GateData, String, String, WireData>;
      if (model == null) return;
      XElement root = model.Save<GateData, WireData>("LogicCircuit", "Gate", "Wire");
      Demo.MainPage.Instance.SavedXML = root.ToString();
      LoadButton.IsEnabled = true;
      model.IsModified = false;
    }

    private void Load_Click(object sender, RoutedEventArgs e) {
      var model = myDiagram.Model as GraphLinksModel<GateData, String, String, WireData>;
      if (model == null) return;
      try {
        XElement root = XElement.Parse(Demo.MainPage.Instance.SavedXML);
        model.Load<GateData, WireData>(root, "Gate", "Wire");
      } catch (Exception ex) {
        MessageBox.Show(ex.ToString());
      }
      model.IsModified = false;
    }
  }


#if !SILVERLIGHT
  [Serializable]
#endif
  public class GateData : GraphLinksModelNodeData<String> {
    // the type of gate that this node represents; only set on initialization
    public String GateType { get; set; }

    // the shape of the node figure is bound to this property; only set on initialization
    public NodeFigure Figure { get; set; }

    // the current value of this gate;
    // the node color is bound to its Data's Value property and must update with it
    public Boolean Value {
      get { return _Value; }
      set {
        Boolean old = _Value;
        if (old != value) {
          _Value = value;
          RaisePropertyChanged("Value", old, value);
        }
      }
    }
    private Boolean _Value;

    // support standard reading/writing via Linq for XML
    public override XElement MakeXElement(XName n) {
      XElement e = base.MakeXElement(n);
      e.Add(XHelper.Attribute("GateType", this.GateType, ""));
      e.Add(XHelper.AttributeEnum<NodeFigure>("Figure", this.Figure, NodeFigure.Rectangle));
      e.Add(XHelper.Attribute("Value", this.Value, false));
      return e;
    }

    public override void LoadFromXElement(XElement e) {
      base.LoadFromXElement(e);
      this.GateType = XHelper.Read("GateType", e, "");
      this.Figure = XHelper.ReadEnum<NodeFigure>("Figure", e, NodeFigure.Rectangle);
      this.Value = XHelper.Read("Value", e, false);
    }
  }


#if !SILVERLIGHT
  [Serializable]
#endif
  public class WireData : GraphLinksModelLinkData<String, String> {
    // the link color is bound to its Data's Value property and must update with it
    public Boolean Value {
      get { return _Value; }
      set {
        Boolean old = _Value;
        if (old != value) {
          _Value = value;
          RaisePropertyChanged("Value", old, value);
        }
      }
    }
    private Boolean _Value;

    // the only additional property, Value, is not meant to be persistent
    // on "wires", so we don't need to override MakeXElement and LoadFromXElement
  }
}