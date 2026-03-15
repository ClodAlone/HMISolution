/* Copyright © Northwoods Software Corporation, 2008-2015. All Rights Reserved. */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Northwoods.GoXam;
using Northwoods.GoXam.Model;

namespace VisualTree {
  public partial class VisualTree : UserControl {
    public VisualTree() {
      InitializeComponent();
      // only initialize myVisualTreeDiagram after myDiagram has been fully initialized
      myDiagram.InitialLayoutCompleted += (s, e) => InitTree();
    }

    private bool InitializedTree { get; set; }

    private void InitTree() {
      if (!this.InitializedTree) {
        this.InitializedTree = true;
        Dictionary<UIElement, bool> elements = new Dictionary<UIElement, bool>();
        WalkVisualTree(myDiagram, elements);
        myVisualTreeDiagram.Model.NodesSource = elements.Keys.ToList();
      }
    }

    private void WalkVisualTree(UIElement elt, Dictionary<UIElement, bool> coll) {
      if (elt == null) return;
      if (!elt.GetType().IsVisible) return;
      coll.Add(elt, true);
      for (int i = 0; i < VisualTreeHelper.GetChildrenCount(elt); i++) {
        WalkVisualTree(VisualTreeHelper.GetChild(elt, i) as UIElement, coll);
      }
    }

    // when using a Button that explicitly expands and collapses subtree
    private void Expand_Click(object sender, RoutedEventArgs e) {
      Node node = Part.FindAncestor<Node>(sender as UIElement);
      if (node != null) {
        myVisualTreeDiagram.StartTransaction("ExpandCollapse");
        // hide/show children of this node
        node.IsExpandedTree = !node.IsExpandedTree;
        // change the appearance of the button appropriately
        FrameworkElement minusshape = node.FindNamedDescendant("MinusShape");
        if (minusshape != null) {
          minusshape.Visibility = (node.IsExpandedTree ? Visibility.Visible : Visibility.Collapsed);
        }
        FrameworkElement plusshape = node.FindNamedDescendant("PlusShape");
        if (plusshape != null) {
          plusshape.Visibility = (node.IsExpandedTree ? Visibility.Collapsed : Visibility.Visible);
        }
        myVisualTreeDiagram.CommitTransaction("ExpandCollapse");
      }
    }

    private void Refresh_Click(object sender, RoutedEventArgs e) {
      this.InitializedTree = false;
      InitTree();
    }
  }


  // this TreeModel uses the actual visual tree elements as its data
  public class VisualTreeModel : TreeModel<UIElement, UIElement> {
    public VisualTreeModel() {
      this.NodeKeyIsNodeData = true;  // direct references from UIElement to its visual children
      this.ChildNodesPath = null;  // make sure FindChildNodeKeysForNode is called
    }

    // use VisualTreeHelper to find the child elements of an element
    protected override IEnumerable FindChildNodeKeysForNode(UIElement nodedata) {
      List<UIElement> children = new List<UIElement>();
      for (int i = 0; i < VisualTreeHelper.GetChildrenCount(nodedata); i++) {
        UIElement elt = VisualTreeHelper.GetChild(nodedata, i) as UIElement;
        if (elt != null) children.Add(elt);
      }
      return children;
    }
  }


  // Show text for any UIElement
  public class ElementConverter : Converter {
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
      if (value == null) return "(null)";
      UIElement elt = value as UIElement;
      if (elt != null) {
        return elt.GetType().Name;
      }
      return value.ToString();
    }
  }

  // Visible if there are any visual children for a UIElement
  public class ChildrenVisibilityConverter : Converter {
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
      UIElement elt = value as UIElement;
      if (elt != null) {
        return (VisualTreeHelper.GetChildrenCount(elt) > 0) ? Visibility.Visible : Visibility.Collapsed;
      } else {
        return Visibility.Visible;
      }
    }
  }
}
