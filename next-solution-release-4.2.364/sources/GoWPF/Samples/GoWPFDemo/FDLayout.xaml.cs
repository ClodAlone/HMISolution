/* Copyright © Northwoods Software Corporation, 2008-2015. All Rights Reserved. */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Northwoods.GoXam;
using Northwoods.GoXam.Layout;
using Northwoods.GoXam.Model;

namespace FDLayout {
  public partial class FDLayout : UserControl {
    public FDLayout() {
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

    // Generates a random tree and sets the diagrams nodesource and linksource to it.
    private void GenerateTree_Click(object sender, RoutedEventArgs e) {
      var nodes = GenerateNodes();
      myDiagram.Model.NodesSource = nodes;
      var lmodel = myDiagram.Model as GraphLinksModel<SimpleData, String, String, LinkData>;
      lmodel.LinksSource = GenerateLinks(nodes);
    }
  }

  // Implements a custom ForceDirectedLayout for purposes of initializing
  // the value of the ForceDirectedVertex.IsFixed property based on Node.IsSelected.
  public class CustomForceDirectedLayout : ForceDirectedLayout {
    public override ForceDirectedNetwork MakeNetwork(IEnumerable<Node> nodes, IEnumerable<Link> links) {
      ForceDirectedNetwork net = base.MakeNetwork(nodes, links);
      foreach (ForceDirectedVertex v in net.Vertexes) {
        Node node = v.Node;
        if (node != null) v.IsFixed = node.IsSelected;
      }
      return net;
    }
  }

  // This class holds sufficient information for all nodes in this sample.
  public class SimpleData : GraphLinksModelNodeData<String> {
    public String Color {
      get { return _Color; }
      set { if (_Color != value) { String old = _Color; _Color = value; RaisePropertyChanged("Color", old, value); } }
    }
    private String _Color = "White";
  }

  // For this sample there no properties to add to link data.
  public class LinkData : GraphLinksModelLinkData<String, String> { }
}
