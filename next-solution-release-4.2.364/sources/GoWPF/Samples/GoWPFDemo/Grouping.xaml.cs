/* Copyright © Northwoods Software Corporation, 2008-2015. All Rights Reserved. */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Northwoods.GoXam;
using Northwoods.GoXam.Model;

namespace Grouping {
  public partial class Grouping : UserControl {
    public Grouping() {
      InitializeComponent();
      // disable scroll animation by modifying the value of DiagramPanel.ZoomTime,
      // but only after the DiagramPanel has been created when the Diagram.Template is applied:
      myDiagram.TemplateApplied += (s, e) => { myDiagram.Panel.ZoomTime = 0; };

      var model = new GraphModel<SimpleData, String>();
      model.NodesSource = new ObservableCollection<SimpleData>();
      myDiagram.Model = model;
      model.Modifiable = true;
      CreateSubGraph(null);
    }

    private void CollapseExpandButton_Click(object sender, RoutedEventArgs e) {
      // the Button is in the visual tree of a Node
      Button button = (Button)sender;
      Group sg = Part.FindAncestor<Group>(button);
      if (sg != null) {
        SimpleData subgraphdata = (SimpleData)sg.Data;
        if (!subgraphdata.IsSubGraph) return;
        // always make changes within a transaction
        myDiagram.StartTransaction("CollapseExpand");
        // if needed, create the child data for this node
        if (!subgraphdata.EverExpanded && !sg.IsExpandedSubGraph) {
          subgraphdata.EverExpanded = true;  // only create children once per node
          int nummembers = CreateSubGraph(subgraphdata);
          if (nummembers == 0) {  // now known no children: don't need Button
            button.Visibility = Visibility.Collapsed;
          }
        }
        // toggle whether this node is expanded or collapsed
        sg.IsExpandedSubGraph = !sg.IsExpandedSubGraph;
        myDiagram.Panel.CenterPart(sg);
        myDiagram.CommitTransaction("CollapseExpand");
      }
    }

    // Create some member nodes, including nested subgraphs, for PARENTDATA.
    // PARENTDATA may be null when creating the initial, top-level nodes
    private int CreateSubGraph(SimpleData group) {
      ISubGraphModel sgmodel = myDiagram.Model as ISubGraphModel;
      if (sgmodel == null) return 0;
      myDiagram.StartTransaction("Add Group contents");

      var nodes = new List<SimpleData>();  // this will contain all of the member nodes created
      int groupCount = myDiagram.Nodes.OfType<Group>().Count();

      // create a random number of subgraphs
      // ensure there are at least 10 groups in the diagram
      var numgroups = rand.Next(2);
      if (groupCount < 10) numgroups += 1;
      for (int i = 0; i < numgroups; i++) {
        var g = new SimpleData();
        g.Color = String.Format("#{0:X}{1:X}{2:X}", 80+rand.Next(100), 80+rand.Next(100), 80+rand.Next(100));
        g.Key = "group" + (i + groupCount).ToString();
        g.IsSubGraph = true;
        if (group != null) g.SubGraphKey = group.Key; // declare that it's a member of nodedata
        sgmodel.AddNode(g);
        nodes.Add(g);
      }
      var numnodes = rand.Next(4) + 2;

      // create a random number of non-group nodes
      for (int i = 0; i < numnodes; i++) {
        var n = new SimpleData();
        n.Color = String.Format("#{0:X}{1:X}{2:X}", 80+rand.Next(100), 80+rand.Next(100), 80+rand.Next(100));
        n.Key = n.Color;
        if (group != null) n.SubGraphKey = group.Key; // declare that it's a member of nodedata
        sgmodel.AddNode(n);
        nodes.Add(n);
      }

      // add at least one link from each node to another
      // this could result in clusters of nodes unreachable from each other, but no lone nodes
      nodes.Sort((x, y) => x.Color.CompareTo(y.Color));
      for (int i = 0; i < nodes.Count; i++) {
        var d = nodes[i];
        int from = rand.Next(nodes.Count-i) + i;
        if (from != i) {
          d.FromKeys.Add(nodes[from].Key);
        }
      }

      myDiagram.CommitTransaction("Add Group contents");
      return nodes.Count;
    }

    Random rand = new Random();
  }

  // Add properties indicating whether we need to find/create member nodes
  // and what color the node should be.
  // Because these properties are only set at initialization,
  // their setters do not need to call RaisePropertyChanged.
  public class SimpleData : GraphModelNodeData<String> {
    public bool EverExpanded { get; set; }

    public String Color { get; set; }
  }
}
