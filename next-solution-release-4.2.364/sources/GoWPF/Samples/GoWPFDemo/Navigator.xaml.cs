/* Copyright © Northwoods Software Corporation, 2008-2015. All Rights Reserved. */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.Linq;
using Northwoods.GoXam;
using Northwoods.GoXam.Model;

namespace Navigator {

  public partial class Navigator : UserControl {
    public Navigator() {
      InitializeComponent();

      var model = new GraphLinksModel<NavModelNodeData, String, String, NavModelLinkData>();
      model.NodeKeyPath = "Key";
      model.NodeIsGroupPath = "IsSubGraph";
      model.GroupNodePath = "SubGraphKey";
      model.LinkFromPath="From";
      model.LinkToPath="To";

      using (Stream stream = Demo.MainPage.Instance.GetStream("Navigator", "xml")) {
        using (StreamReader reader = new StreamReader(stream)) {
          XElement root = XElement.Load(reader);
          // Create NavModelNodeData objects to represent the nodes in the diagram
          var modelNodes = new ObservableCollection<NavModelNodeData>();
          // Find all elements in XML file with tag "n"
          var nodeElts = from n in root.Descendants("n") select n;
          foreach (XElement xe in nodeElts) {
            modelNodes.Add(new NavModelNodeData() {
              // The name of the NavModelNodeData
              Key = XHelper.Read("name", xe, ""),
              // Parent subgraph reference; may be null
              SubGraphKey = XHelper.Read("subGraph", xe, ""),
              // If isSubGraph attribute was "True", the NavModelNodeData will be a subGraph
              IsSubGraph = XHelper.Read("isSubGraph", xe, false),
              // From "x" and "y" attribute values, sets Node's starting positon
              InitPosition = new Point(XHelper.Read("x", xe, 0.0), XHelper.Read("y", xe, 0.0))
            });
          }
          // Create highlightable NavModelLinkData objects using the information
          // about how the nodes are connected (contained in links part of XML file)
          var modelLinks = new ObservableCollection<NavModelLinkData>();
          // Find all elements in XML file with tag "l"
          var linkElts = from l in root.Descendants("l") select l;
          foreach (XElement xe in linkElts) {
            modelLinks.Add(new NavModelLinkData() {
              From = XHelper.Read("from", xe, ""),
              To = XHelper.Read("to", xe, "")
            });
          }
          // initialize the model
          model.NodesSource = modelNodes;
          model.LinksSource = modelLinks;
        }
      }
      myDiagram.Model = model;
    }

    // Called whenever mouse is clicked; updates values of Highlight for myDiagram's Parts 
    public void Update(object sender, RoutedEventArgs e) {
      if (myDiagram == null) return;
      if (sender as RadioButton != null) {
#if SILVERLIGHT  // update the HTML comment window
        Demo.MainPage.Instance.NavigateToHtml(
            String.Format("Navigator.html#{0}", (sender as RadioButton).Content.ToString()));
#else
        if ((sender as RadioButton).Content.ToString() == "Unhighlight All")
          Demo.MainPage.Instance.NavigateToHtml("Navigator");
        else
          Demo.MainPage.Instance.NavigateToHtml(
                String.Format("Navigator.{0}", (sender as RadioButton).Content.ToString()));
#endif
      }
      Clear();  // zero the Highlight value for all Nodes and Links
      // if a part is selected, update the Highlight value for related parts
      if (myDiagram.SelectedPart != null) {
        if ((bool)LinksIntoButton.IsChecked) LinksIntoButton_Checked(sender, e);
        if ((bool)LinksOutOfButton.IsChecked) LinksOutOfButton_Checked(sender, e);
        if ((bool)LinksConnectedButton.IsChecked) LinksConnectedButton_Checked(sender, e);
        if ((bool)NodesIntoButton.IsChecked) NodesIntoButton_Checked(sender, e);
        if ((bool)NodesOutOfButton.IsChecked) NodesOutOfButton_Checked(sender, e);
        if ((bool)NodesReachableButton.IsChecked) NodesReachableButton_Checked(sender, e);
        if ((bool)NodesConnectedButton.IsChecked) NodesConnectedButton_Checked(sender, e);
        if ((bool)ContainingGroupParentButton.IsChecked) ContainingGroupParentButton_Checked(sender, e);
        if ((bool)ContainingGroupAllButton.IsChecked) ContainingGroupAllButton_Checked(sender, e);
        if ((bool)MemberNodesChildrenButton.IsChecked) MemberNodesChildrenButton_Checked(sender, e);
        if ((bool)MemberNodesAllButton.IsChecked) MemberNodesAllButton_Checked(sender, e);
        if ((bool)MemberLinksChildrenButton.IsChecked) MemberLinksChildrenButton_Checked(sender, e);
        if ((bool)MemberLinksAllButton.IsChecked) MemberLinksAllButton_Checked(sender, e);
        if ((bool)DeselectButton.IsChecked) DeselectButton_Checked(sender, e);
      }
    }

    private void Clear() {
      foreach (NavModelNodeData d in myDiagram.Model.NodesSource) d.Highlight = 0;
      foreach (NavModelLinkData d in ((ILinksModel)myDiagram.Model).LinksSource) d.Highlight = 0;
    }

    // Methods called when different RadioButtons are checked

    private void LinksIntoButton_Checked(object sender, RoutedEventArgs e) {
      // Check if the Selected Part is a Node (as a Link cannot have other Links lead into it)
      Node node = myDiagram.SelectedPart as Node;
      if (node != null) {
        foreach (Link link in node.LinksInto)        // For that Node, get LinksInto, cast all as 
          ((NavModelLinkData)(link.Data)).Highlight = 1;  // NavModelLinkData, and set Highlight to true
      }
    }

    private void LinksOutOfButton_Checked(object sender, RoutedEventArgs e) {
      // Check if the Selected Part is a Node (as a Link cannot have other Links lead out of it)
      Node node = myDiagram.SelectedPart as Node;
      if (node != null) {
        foreach (Link link in node.LinksOutOf)     // For that Node, find all LinksOutOf, cast as 
          ((NavModelLinkData)link.Data).Highlight = 1;  // NavModelLinkData, and set Highlight to true
      }
    }

    private void LinksConnectedButton_Checked(object sender, RoutedEventArgs e) {
     // Highlight all Links Connected to the Selected Node
      Node node = myDiagram.SelectedPart as Node;
      if (node != null) {
        foreach (Link link in node.LinksConnected)
          ((NavModelLinkData)link.Data).Highlight = 1;
      }
    }

    private void NodesIntoButton_Checked(object sender, RoutedEventArgs e) {
      // If the selected Part is a Link, highlight its FromNode
      Link link = myDiagram.SelectedLink;
      if (link != null) {
        ((NavModelNodeData)link.FromNode.Data).Highlight = 1;
      } else {
        // If the selected Part is a Node, highlight all the Nodes leading into it
        Node node = myDiagram.SelectedPart as Node;
        if (node != null) {
          foreach (Node other in node.NodesInto)
            ((NavModelNodeData)other.Data).Highlight = 1;
        }
      }
    }

    private void NodesOutOfButton_Checked(object sender, RoutedEventArgs e) {
      // If the selected Part is a Link, highlight its ToNode
      Link link = myDiagram.SelectedLink;
      if (link != null) {
        ((NavModelNodeData)link.ToNode.Data).Highlight = 1;
      } else {
        // If the selected Part is a Node, highlight all the Nodes coming out of it
        Node node = myDiagram.SelectedPart as Node;
        if (node != null) {
          foreach (Node other in node.NodesOutOf)
            ((NavModelNodeData)other.Data).Highlight = 1;
        }
      }
    }

    private void NodesReachableButton_Checked(object sender, RoutedEventArgs e) {
      // If Selected Part is a Link, highlight ToNode with value of 1
      Link link = myDiagram.SelectedLink;
      if (link != null) {
        ((NavModelNodeData)link.ToNode.Data).Highlight = 1;
        // Recursively find all Nodes reachable from ToNode
        RecursiveTo((myDiagram.SelectedLink).ToNode, 2);
      } else {
        // If Selected Part is a Node, recursively find all Nodes reachable from it
        Node node = myDiagram.SelectedPart as Node;
        if (node != null) {
          RecursiveTo(node, 1);
        }
      }
    }

    private void NodesConnectedButton_Checked(object sender, RoutedEventArgs e) {
      // Highlight all Nodes Connected to the Selected Node
      Node node = myDiagram.SelectedPart as Node;
      if (node != null) {
        foreach (Node other in node.NodesConnected)
          ((NavModelNodeData)other.Data).Highlight = 1;
      }
    }

    private void ContainingGroupParentButton_Checked(object sender, RoutedEventArgs e) {
      // For the Selected Part, highlight ContainingSubGraph (if one exists)
      if (myDiagram.SelectedPart != null && myDiagram.SelectedPart.ContainingSubGraph != null) {
        ((NavModelNodeData)myDiagram.SelectedPart.ContainingSubGraph.Data).Highlight = 1;
      }
    }

    private void ContainingGroupAllButton_Checked(object sender, RoutedEventArgs e) {
      // For the Selected Part, highlight all higher generations of ContainingSubGraphs
      if (myDiagram.SelectedPart != null && myDiagram.SelectedPart.ContainingSubGraph != null) {
        RecursiveParent(myDiagram.SelectedPart.ContainingSubGraph, 1);
      }
    }

    private void MemberNodesChildrenButton_Checked(object sender, RoutedEventArgs e) {
      // If the Selected Part is a Group, highlight all member Nodes
      if (myDiagram.SelectedGroup != null) {
        foreach (Node child in myDiagram.SelectedGroup.MemberNodes)
          ((NavModelNodeData)child.Data).Highlight = 1;
      }
    }

    private void MemberNodesAllButton_Checked(object sender, RoutedEventArgs e) {
      // If the Selected Part is a Group, highlight all member Nodes and their descendants
      if (myDiagram.SelectedGroup != null) {
        RecursiveChildren(myDiagram.SelectedGroup, typeof(Node), 1);
      }
    }

    private void MemberLinksChildrenButton_Checked(object sender, RoutedEventArgs e) {
      // If the Selected Part is a Group, highlight all member Links
      if (myDiagram.SelectedGroup != null) {
        foreach (Link child in (myDiagram.SelectedGroup).MemberLinks)
          ((NavModelLinkData)child.Data).Highlight = 1;
      }
    }

    private void MemberLinksAllButton_Checked(object sender, RoutedEventArgs e) {
      // If the Selected Part is a Group, highlight all member Links,
      // as well as member Links of descendant Groups
      if (myDiagram.SelectedGroup != null) {
        RecursiveChildren(myDiagram.SelectedGroup, typeof(Link), 1);
      }
    }

    private void DeselectButton_Checked(object sender, RoutedEventArgs e) {
      // nothing to do
    }

   // Helper functions for certain "Button_Checked" methods; they are needed as 
   // separate methods because they sometimes must be recursive.

   // This method "follows" a Node down its Links until the "flow diagram"
   // comes to an end, Highlighting all Nodes passed on the way
    private void RecursiveTo(Node n, int generationNum) {
      foreach (Node nodeBeingTested in n.NodesOutOf) {
        NavModelNodeData data = (NavModelNodeData)nodeBeingTested.Data;
        // If a Node is highlighted, then RecursiveTo has already been called on it.
        // Thus, a Node only need be checked if it is not highlighted (also
        // necessary to prevent StackOverflowException, in some cases)
        if (data.Highlight > generationNum || data.Highlight == 0) {
          data.Highlight = generationNum;
          RecursiveTo(nodeBeingTested, generationNum + 1);
        }
      }
    }

    private void RecursiveParent(Group grp, int generationNum) {
      // If the Group being tested has a subGraph, re-calls RecursiveParent on it
      (grp.Data as NavModelNodeData).Highlight = generationNum;
      if (grp.ContainingSubGraph != null)
        RecursiveParent(grp.ContainingSubGraph, generationNum + 1);
    }

    private void RecursiveChildren(Group grp, Type manipulatingType, int generationNum) {
      // Highlights memberNodes in grp if specified by the Type param.
      if (manipulatingType == typeof(Node)) {
        foreach (Node nd in grp.MemberNodes)
          (nd.Data as NavModelNodeData).Highlight = generationNum;
      }
      // Highlights memberLinks in grp if specified by the Type param.
      if (manipulatingType == typeof(Link)) {
        foreach (Link lnk in grp.MemberLinks)
          (lnk.Data as NavModelLinkData).Highlight = generationNum;
      }
      // Re-calls method on all Groups that are member of grp
      foreach (Node nde in grp.MemberNodes) {
        if (nde as Group != null)
          RecursiveChildren(nde as Group, manipulatingType, generationNum + 1);
      }
    }
  }


  // Extension of GraphModelNodeData<T> with Highlight property to allow for visual emphasis 
  // on a specific Node, as well as InitPosition for the starting arrangement of the diagram
  public class NavModelNodeData : GraphLinksModelNodeData<String> {
    public int Highlight {
      get { return _Highlight; }
      set {
        int old = _Highlight;
        if (old != value) {
          _Highlight = value;
          RaisePropertyChanged("Highlight", old, value);
        }
      }
    }
    private int _Highlight = 0;

    public Point InitPosition { get; set; }
  }


  // Extension of GraphLinksModelLinkData<String, String> with Highlight property to allow for
  // visual emphasis on a specific Link; implements INotifyPropertyChanged to allow for DataBinding
  public class NavModelLinkData : GraphLinksModelLinkData<String, String> {
    public int Highlight {
      get { return _Highlight; }
      set {
        int old = _Highlight;
        if (old != value) {
          _Highlight = value;
          RaisePropertyChanged("Highlight", old, value);
        }
      }
    }
    private int _Highlight = 0; // All parts are by default not highlighted
  }


  // Keep a list of color information for various "relationship distances"
  public class ColorList : List<ColorInfo> {
    public ColorList() {}
  }

  // For each relationship distance, have a color (actually a brush) and a text description
  public class ColorInfo {
    public int Index { get; set; }
    public SolidColorBrush Brush { get; set; }
    public String Description { get; set; }
  }

  // Allows Color of a Part to depend on the value of a Part's Data's Highlight property
  public class ColorConverter : Converter {
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
      if (this.Objects != null) {
        int idx = (value != null ? (int)value : 0);
        int len = this.Objects.Count();
        if (idx >= len) idx = len-1;
        if (idx >= 0) return this.Objects.ElementAt(idx).Brush;
      }
      return null;
    }
    public ColorList Objects { get; set; }
  }


  // Allows thickness of a line or shape depend on the value of a Part's Data's Highlight property
  public class ThicknessConverter : Converter {
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
      // In Silverlight, a Border takes a Thickness - not an int - as its value
      if (targetType == typeof(Thickness)) {
        if (value != null && (int)value != 0) return new Thickness(3);
        return new Thickness(1);
      } else if (targetType == typeof(double)) {
        if (value != null && (int)value != 0) return 3.0;
        return 1.0;
      } else {
        return base.Convert(value, targetType, parameter, culture);
      }
    }
  }


  // Generate a tooltip description for a part
  public class PartTextConverter : Converter {
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
      if (value == null) return "";
      Part part = value as Part;
      Node node = value as Node;
      Group group = value as Group;
      Link link = value as Link;
      String typename = "";
      if (node != null) typename = "Node";
      if (group != null) typename = "Group";
      if (link != null) typename = "Link";
      String returnString = String.Format("{0}: {1}", typename, part.Data.ToString());
      if (link != null) {
        returnString += String.Format("\nNode To: {0}\nNode From: {1}",
                                      link.ToNode.Data.ToString(), link.FromNode.Data.ToString());
        Group sg = link.ContainingSubGraph;
        if (sg != null) returnString += String.Format("\nContaining SubGraph: {0}", sg.Data.ToString());
      }
      if (node != null) {
        if (node.NodesInto.Count() > 0) {
          returnString += "\nNodes into:";
          foreach (Node intoNode in node.NodesInto)
            returnString += String.Format(" {0},", intoNode.Data.ToString());
          // previous iteration leaves comma on end of list of nodes; remove it
          returnString = returnString.TrimEnd(new char[] { ',' });
        }
        if (node.NodesOutOf.Count() > 0) {
          returnString += "\nNodes out of:";
          foreach (Node outOfNode in node.NodesOutOf)
            returnString += String.Format(" {0},", outOfNode.Data.ToString());
          // previous iteration leaves comma on end of list of nodes; remove it
          returnString = returnString.TrimEnd(new char[] { ',' });
        }
        Group sg = node.ContainingSubGraph;
        if (sg != null)
          returnString += String.Format("\nContaining SubGraph: {0}", sg.Data.ToString());
        if (group != null) {
          if (group.MemberNodes.Count() > 0) {
            returnString += "\nMember Nodes:";
            foreach (Node memberNode in group.MemberNodes)
              returnString += String.Format(" {0},", memberNode.Data.ToString());
            returnString = returnString.TrimEnd(new char[] { ',' });
          }
          if (group.MemberLinks.Count() > 0) {
            returnString += "\nMember Links:";
            foreach (Link memberLink in group.MemberLinks)
              returnString += String.Format(" {0},", memberLink.Data.ToString());
            returnString = returnString.TrimEnd(new char[] { ',' });
          }
        }
      }
      return returnString;
    }
  }
}