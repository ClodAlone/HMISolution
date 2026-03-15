/* Copyright © Northwoods Software Corporation, 2008-2015. All Rights Reserved. */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Northwoods.GoXam;
using Northwoods.GoXam.Model;
using Northwoods.GoXam.Layout;

namespace FriendWheel {
  public partial class FriendWheel : UserControl {
    public FriendWheel() {
      InitializeComponent();

      var nodes = new List<String>() {
        "Joshua", "Daniel", "Robert", "Noah", "Anthony",
        "Elizabeth", "Addison", "Alexis", "Ella", "Samantha",
        "Joseph", "Scott", "James", "Ryan", "Benjamin",
        "Walter", "Gabriel", "Christian", "Nathan", "Simon",
        "Isabella", "Emma", "Olivia", "Sophia", "Ava",
        "Emily", "Madison", "Tina", "Elena", "Mia",
        "Jacob", "Ethan", "Michael", "Alexander", "William",
        "Natalie", "Grace", "Lily", "Alyssa", "Ashley",
        "Sarah", "Taylor", "Hannah", "Brianna", "Hailey",
        "Christopher", "Aiden", "Matthew", "David", "Andrew",
        "Kaylee", "Juliana", "Leah", "Anna", "Allison",
        "John", "Samuel", "Tyler", "Dylan", "Jonathan",
      };

      var links = new ObservableCollection<LinkData>();
      // create a bunch of random "friend" relationships
      Random rand = new Random();
      for (int i = 0; i < nodes.Count*2; i++) {
        int a = rand.Next(nodes.Count);
        int b = rand.Next(nodes.Count/4)+1;
        links.Add(new LinkData() {
          From = nodes[a],
          To = nodes[(a+b) % nodes.Count],
          Color = String.Format("#{0:X}{1:X}{2:X}", 90+rand.Next(90), 90+rand.Next(90), 90+rand.Next(90))
        });
      }

      var model = new GraphLinksModel<String, String, String, LinkData>();
      model.NodeKeyIsNodeData = true;
      model.NodesSource = nodes;
      model.LinksSource = links;
      myDiagram.Model = model;
    }

    private Brush HighlightBrush = new SolidColorBrush(Colors.Red);

    private void Highlight(Node n, Brush b) {
      Shape shape = n.FindNamedDescendant("Icon") as Shape;
      if (shape != null) shape.Stroke = b;
    }

    // When the mouse is over a link, highlight the nodes it connects.
    // To avoid confusion with highlights, de-select any selected node.
    private void Path_MouseEnter(object sender, MouseEventArgs e) {
      Link link = Part.FindAncestor<Link>(sender as UIElement);
      if (link != null) {
        myDiagram.SelectedNode = null;
        Highlight(link.FromNode, HighlightBrush);
        Highlight(link.ToNode, HighlightBrush);
      }
    }

    private void Path_MouseLeave(object sender, MouseEventArgs e) {
      Link link = Part.FindAncestor<Link>(sender as UIElement);
      if (link != null) {
        Highlight(link.FromNode, null);
        Highlight(link.ToNode, null);
      }
    }

    // Selecting a node highlights its connected nodes
    private void myDiagram_SelectionChanged(object sender, SelectionChangedEventArgs e) {
      // first, unhighlight previously highlighted nodes
      if (this.WasSelected != null) {
        foreach (Node n in this.WasSelected.NodesConnected) {
          Highlight(n, null);
        }
      }
      this.WasSelected = myDiagram.SelectedNode;
      // now highlight Nodes connected to the selected node
      if (this.WasSelected != null) {
        foreach (Node n in this.WasSelected.NodesConnected) {
          Highlight(n, HighlightBrush);
        }
      }
    }

    private Node WasSelected { get; set; }
  }


  public class CustomLayout : CircularLayout {
    public CustomLayout() {
      this.Arrangement = CircularArrangement.ConstantDistance;
      this.NodeDiameterFormula = CircularNodeDiameterFormula.Circular;
    }

    public override CircularNetwork MakeNetwork(IEnumerable<Node> nodes, IEnumerable<Link> links) {
      CircularNetwork net = base.MakeNetwork(nodes, links);
      // assume each Node has a constant diameter, ignoring any TextBlock
      double dia = 20;
      foreach (CircularVertex v in net.Vertexes) {
        v.Diameter = dia;
      }
      return net;
    }

    protected override void LayoutNodes() {
      base.LayoutNodes();
      foreach (CircularVertex v in this.Network.Vertexes) {
        Node node = v.Node;
        if (node == null) continue;
        // rotate the whole node according to its angle
        double a = v.ActualAngle;
        node.RotationAngle = a;
        // rotate the text so that it isn't upside-down
        if (a > 90 && a < 270) {
          TextBlock tb = node.FindNamedDescendant("Text") as TextBlock;
          if (tb != null) {
            tb.RenderTransform = new RotateTransform() { Angle=180 };
            tb.RenderTransformOrigin = new Point(0.5, 0.5);
          }
        }
      }
    }
  }

  public class LinkData : GraphLinksModelLinkData<String, String> {
    public String Color {
      get { return _Color; }
      set { if (_Color != value) { String old = _Color; _Color = value; RaisePropertyChanged("Color", old, value); } }
    }
    private String _Color = "Black";
  }
}
