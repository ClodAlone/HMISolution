/* Copyright © Northwoods Software Corporation, 2008-2015. All Rights Reserved. */

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Northwoods.GoXam;
using Northwoods.GoXam.Model;

namespace IncrementalTree {
  public partial class IncrementalTree : UserControl {
    public IncrementalTree() {
      InitializeComponent();

      // the model is a tree of SimpleData, indexed by strings
      var model = new TreeModel<SimpleData, String>();
      // create the root node
      model.NodesSource = new ObservableCollection<SimpleData>() {
        new SimpleData() { Key="/", Color="White" }
      };
      model.Modifiable = true;

      myDiagram.Model = model;
    }

    private void CollapseExpandButton_Click(object sender, RoutedEventArgs e) {
      // the Button is in the visual tree of a Node
      Button button = (Button)sender;
      Node n = Part.FindAncestor<Node>(button);
      if (n != null) {
        SimpleData parentdata = (SimpleData)n.Data;
        // always make changes within a transaction
        myDiagram.StartTransaction("CollapseExpand");
        // if needed, create the child data for this node
        if (!parentdata.EverExpanded) {
          parentdata.EverExpanded = true;  // only create children once per node!
          int numchildren = CreateSubTree(parentdata);
          if (numchildren == 0) {  // now known no children: don't need Button!
            button.Visibility = Visibility.Collapsed;
          }
        }
        // toggle whether this node is expanded or collapsed
        n.IsExpandedTree = !n.IsExpandedTree;
        if (n.IsExpandedTree)
          myDiagram.Panel.CenterPart(n);
        else
          myDiagram.Panel.CenterPart(n.NodesInto.FirstOrDefault());
        myDiagram.CommitTransaction("CollapseExpand");
      }
    }

    private int CreateSubTree(SimpleData parentdata) {
      int numchildren = rand.Next(10);
      if (myDiagram.PartManager.NodesCount <= 1) {
        numchildren += 1;  // make sure the root node has at least one child
      }
      // create several SimpleData objects and add them to the model
      for (int i = 0; i < numchildren; i++) {
        SimpleData childdata = new SimpleData();
        childdata.Color = String.Format("#{0:X}{1:X}{2:X}",
              120+rand.Next(100), 120+rand.Next(100), 120+rand.Next(100));
        childdata.Key = parentdata.Key + "/" + childdata.Color;
        childdata.ParentKey = parentdata.Key;
        myDiagram.Model.AddNode(childdata);
      }
      return numchildren;
    }

    Random rand = new Random();
  }


  // Add properties indicating whether we need to find/create child nodes
  // and what color the node should be.
  // Because these properties are only set at initialization,
  // their setters do not need to call RaisePropertyChanged.
  public class SimpleData : TreeModelNodeData<String> {
    public bool EverExpanded { get; set; }

    public String Color { get; set; }
  }
}
