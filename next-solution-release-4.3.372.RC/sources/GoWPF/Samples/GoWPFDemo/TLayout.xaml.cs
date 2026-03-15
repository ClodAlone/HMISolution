/* Copyright © Northwoods Software Corporation, 2008-2015. All Rights Reserved. */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Northwoods.GoXam;
using Northwoods.GoXam.Layout;
using Northwoods.GoXam.Model;

namespace TLayout {
  public partial class TLayout : UserControl {
    public TLayout() {
      InitializeComponent();

      myDiagram.Model = new GraphLinksModel<SimpleData, String, String, LinkData>();
      myDiagram.Model.Modifiable = true;

      GenerateTree_Click(null, null);
    }

    Random rand = new Random();

    // Takes the random collection of nodes and creates a random tree with them.
    // Respects the minimum and maximum number of links from each node.
    // (The minimum can be disregarded if we run out of nodes to link to)
    private ObservableCollection<LinkData> GenerateLinks(ObservableCollection<SimpleData> nodes) {
      var linkSource = new ObservableCollection<LinkData>();
      if (nodes.Count == 0) return linkSource;
      int minLinks, maxLinks;
      if (!int.TryParse(txtMinLinks.Text, out minLinks))
        minLinks = 1;
      if (!int.TryParse(txtMaxLinks.Text, out maxLinks) || minLinks > maxLinks)
        maxLinks = minLinks;

      List<SimpleData> available = nodes.ToList<SimpleData>();
      foreach (SimpleData next in nodes) {
        available.Remove(next);
        int children = rand.Next(minLinks, maxLinks + 1);
        for (int i = 1; i <= children; i++) {
          if (available.Count == 0) break;
          SimpleData to = available[0];
          available.Remove(to);
          linkSource.Add(new LinkData() { From = next.Key, To = to.Key });
        }
      }
      return linkSource;
    }

    // Creates a collection of randomly colored nodes.
    // Respects the maximum and minimum number of nodes.
    private ObservableCollection<SimpleData> GenerateNodes() {
      var nodeSource = new ObservableCollection<SimpleData>();
      int minNodes, maxNodes;
      if (!int.TryParse(txtMinNodes.Text, out minNodes))
        minNodes = 0;
      if (!int.TryParse(txtMaxNodes.Text, out maxNodes) || minNodes > maxNodes)
        maxNodes = minNodes;
      int numberOfNodes = rand.Next(minNodes, maxNodes + 1);

      for (int i = 0; i < numberOfNodes; i++) {
        nodeSource.Add(new SimpleData() {
          Key = "Node" + i.ToString(),
          Color = String.Format("#{0:X}{1:X}{2:X}", 120+rand.Next(100), 120+rand.Next(100), 120+rand.Next(100))
        });
      }
      // Randomize the nodes a little:
      for (int i = 0; i < nodeSource.Count; i++) {
        int swap = rand.Next(0, nodeSource.Count);
        SimpleData temp = nodeSource[swap];
        nodeSource[swap] = nodeSource[i];
        nodeSource[i] = temp;
      }
      return nodeSource;
    }

    // Generates a random tree respecting MinNodes/MaxNodes/MinLinks/MaxLinks
    private void GenerateTree_Click(object sender, RoutedEventArgs e) {
      var nodes = GenerateNodes();
      myDiagram.Model.NodesSource = nodes;
      var lmodel = myDiagram.Model as GraphLinksModel<SimpleData, String, String, LinkData>;
      lmodel.LinksSource = GenerateLinks(nodes);
    }

    // When a RadioButton becomes checked, set the Tag of the button container to the button's content (string)
    // For internationalization, this should use RadioButton.Tag to hold the value, not the RadioButton.Content
    private void RadioButton_Checked(object sender, RoutedEventArgs e) {
      RadioButton rb = sender as RadioButton;
      if (rb != null) {
        FrameworkElement group = VisualTreeHelper.GetParent(rb) as FrameworkElement;
        if (group != null) {
          group.Tag = rb.Content;
        }
      }
    }
  }

  // Compare vertexes by comparing the suffix of each node data key,
  // treated as integer values rather than as strings.
  public class SimpleComparer : IComparer<TreeVertex> {
    public int Compare(TreeVertex x, TreeVertex y) {
      SimpleData a = x.Node.Data as SimpleData;
      SimpleData b = y.Node.Data as SimpleData;
      int num1 = int.Parse(a.Key.Replace("Node", ""));
      int num2 = int.Parse(b.Key.Replace("Node", ""));
      return num1.CompareTo(num2);
    }
  }


  // add some properties for each node data
  public class SimpleData : GraphLinksModelNodeData<String> {
    public String Color {
      get { return _Color; }
      set { if (_Color != value) { String old = _Color; _Color = value; RaisePropertyChanged("Color", old, value); } }
    }
    private String _Color = "White";
  }

  // no additional properties are needed for link data
  public class LinkData : Northwoods.GoXam.Model.GraphLinksModelLinkData<String, String> {}
}
