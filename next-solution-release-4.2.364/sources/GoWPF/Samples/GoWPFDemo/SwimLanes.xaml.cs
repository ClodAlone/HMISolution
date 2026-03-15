using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Northwoods.GoXam;
using Northwoods.GoXam.Layout;
using Northwoods.GoXam.Model;

namespace SwimLanes {
  public partial class SwimLanes : UserControl {
    public SwimLanes() {
      InitializeComponent();

      var rand = new Random();
      var model = new GraphModel<SimpleData, String>();
      var nodes = new ObservableCollection<SimpleData>();
      var subgraphs = new List<SimpleData>();

      // create some subgraph data, one per "swim lane"
      for (int i = 0; i < 3; i++) {
        SimpleData g = new SimpleData();
        g.Key = String.Format("Group {0:D}", i);
        g.Color = String.Format("#{0:X}{1:X}{2:X}", 80+rand.Next(100), 80+rand.Next(100), 80+rand.Next(100));
        g.IsSubGraph = true;
        subgraphs.Add(g);
        nodes.Add(g);
      }

      // create a lot of regular node data that are members of those subgraphs
      for (int i = 0; i < subgraphs.Count; i++) {
        var members = new List<SimpleData>();

        for (int j = 0; j < 5+rand.Next(20); j++) {
          SimpleData d = new SimpleData();
          d.Color = String.Format("#{0:X}{1:X}{2:X}", 120+rand.Next(100), 120+rand.Next(100), 120+rand.Next(100));
          d.Key = d.Color;
          d.SubGraphKey = subgraphs[i].Key;
          members.Add(d);
          nodes.Add(d);
        }

        // connect members amongst themselves
        for (int j = 0; j < members.Count; j++) {
          var k = j+1+rand.Next(members.Count-j-1);
          if (k < members.Count) {
            var m = members[j];
            m.FromKeys.Add(members[k].Key);
          }
        }
      }

      model.NodesSource = nodes;
      model.Modifiable = true;
      myDiagram.Model = model;
      model.HasUndoManager = true;
    }
  }

  public class VerticalLayout : DiagramLayout {
    public override void DoLayout(IEnumerable<Node> nodes, IEnumerable<Link> links) {
      // Make sure each node has a Position, defaulting to 0,0
      foreach (Node n in nodes) {
        if (!n.Visible || !n.IsBoundToData) continue;
        Point pos = n.Position;
        if (Double.IsNaN(pos.Y)) n.Move(new Point(0, 0), false);
      }
      // Now set their Y positions so that they are all stacked above each other,
      // while setting their X positions to zero.
      double y = 0;
      foreach (Node n in nodes.OrderBy(n => ((SimpleData)n.Data).Key)) {
        if (!n.Visible || !n.IsBoundToData) continue;
        Rect b = n.Bounds;
        n.Move(new Point(0, y), true);
        y += b.Height;
        // for some kinds of nodes you might also want to add some spacing
      }
    }
  }

  public class SimpleData : GraphModelNodeData<String> {
    public String Color { get; set; }
  }
}
