/* Copyright © Northwoods Software Corporation, 2008-2015. All Rights Reserved. */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Northwoods.GoXam;
using Northwoods.GoXam.Model;

namespace LocalView {
  public partial class LocalView : UserControl {
    public LocalView() {
      InitializeComponent();

      // myWholeView shows everything, but at a small scale
      this.WholeModel = CreateModel();
      myWholeView.Model = this.WholeModel;
      BuildWholeModel();

      // myLocalView is initially empty, its model only populated with what is near a selected node
      this.LocalModel = CreateModel();
      this.LocalModel.Modifiable = true;
      myLocalView.Model = this.LocalModel;

      myWholeView.InitialLayoutCompleted += (s, e) => {
        Node aNode = myWholeView.Nodes.ElementAtOrDefault(1);
        if (aNode != null) aNode.IsSelected = true;
      };
    }

    GraphLinksModel<NodeData, String, String, LinkData> WholeModel { get; set; }
    GraphLinksModel<NodeData, String, String, LinkData> LocalModel { get; set; }

    private GraphLinksModel<NodeData, String, String, LinkData> CreateModel() {
      var model = new GraphLinksModel<NodeData, String, String, LinkData>();
      model.MemberNodesPath = "";
      model.GroupNodePath = "";
      return model;
    }

    // parameters for building a tree-structure graph
    private const int minLinks = 1;
    private const int maxLinks = 3;
    private const int numberOfNodes = 100;
    private Random rand = new Random();

    // create a tree-structed graph in this.WholeModel
    private void BuildWholeModel() {
      var nodes = new ObservableCollection<NodeData>();
      for (int i = 0; i < numberOfNodes; i++) {
        nodes.Add(new NodeData() {
          Key = "Node" + i.ToString(),
          // randomly colored nodes
          Color = String.Format("#{0:X}{1:X}{2:X}", 120+rand.Next(100), 120+rand.Next(100), 120+rand.Next(100))
        });
      }

      var links = new ObservableCollection<LinkData>();
      int next = 1;
      for (int i = 0; i < nodes.Count; i++) {
        NodeData from = nodes[i];
        int children = rand.Next(minLinks, maxLinks + 1);
        for (int c = 1; c <= children; c++) {
          if (next >= nodes.Count) break;
          NodeData to = nodes[next++];
          links.Add(new LinkData() { From = from.Key, To = to.Key });
        }
      }

      this.WholeModel.NodesSource = nodes;
      this.WholeModel.LinksSource = links;
    }

    // whenever any Node is selected, make sure we focus on its data
    private void myDiagram_SelectionChanged(object sender, SelectionChangedEventArgs e) {
      Diagram diagram = (Diagram)sender;
      if (diagram.SelectedNode != null) FocusOn(diagram.SelectedNode.Data as NodeData);
    }

    // make sure a given node data is in the LocalModel along with all of the nodes and links
    // connected to it up to some number of links in distance
    private void FocusOn(NodeData data) {
      // make sure it's visible and selected in myWholeView
      Node overviewnode = myWholeView.PartManager.FindNodeForData(data, myWholeView.Model);
      if (overviewnode != null) {
        myWholeView.Panel.MakeVisible(overviewnode, Rect.Empty);
        myWholeView.Select(overviewnode);
        Node highlight = myWholeView.PartsModel.FindNodeByKey("Highlight");
        if (highlight != null) highlight.Location = overviewnode.Location;
      }

      // find all connected node data and link data, and make sure there are Parts for them
      this.LocalModel.StartTransaction("add local graph");
      HashSet<NodeData> wanted = new HashSet<NodeData>();
      CollectLocals(data, null, 2, wanted);
      // and remove all node and link data that are not connected to the focussed node data
      foreach (NodeData existing in this.LocalModel.NodesSource.OfType<NodeData>().ToList()) {
        if (!wanted.Contains(existing)) this.LocalModel.RemoveNode(existing);
      }
      this.LocalModel.CommitTransaction("add local graph");

      // center and select that Node
      myLocalView.Dispatcher.BeginInvoke((Action)(() => {
        Node localnode = myLocalView.PartManager.FindNodeForData(data, myLocalView.Model);
        myLocalView.Panel.CenterPart(localnode);
        myLocalView.Select(localnode);
      }));
    }

    // this collects connected node data up to LEVEL levels away, and adds the node link data to the model
    private void CollectLocals(NodeData data, LinkData linkdata, int level, HashSet<NodeData> wanted) {
      if (data == null) return;
      // stop recursing beyond the number of levels
      if (level < 0) return;

      // always add the link data (but it might be null, which is OK)
      this.LocalModel.AddLink(linkdata);

      // stop if we've already seen the node
      if (wanted.Contains(data)) return;
      // otherwise add it to our collection
      wanted.Add(data);
      this.LocalModel.AddNode(data);

      // recurse through all connected nodes
      foreach (LinkData l in this.WholeModel.GetLinksForNode(data)) {
        CollectLocals(this.WholeModel.GetFromNodeForLink(l), l, level-1, wanted);
        CollectLocals(this.WholeModel.GetToNodeForLink(l), l, level-1, wanted);
      }
    }
  }


  public class NodeData : GraphLinksModelNodeData<String> {
    public String Color {
      get { return _Color; }
      set { if (_Color != value) { String old = _Color; _Color = value; RaisePropertyChanged("Color", old, value); } }
    }
    private String _Color = "White";
  }

  public class LinkData : GraphLinksModelLinkData<String, String> { }
}
