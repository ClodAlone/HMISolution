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
using Northwoods.GoXam.Model;

namespace LDLayout {
  public partial class LDLayout : UserControl {
    public LDLayout() {
      InitializeComponent();

      myDiagram.Model = new GraphLinksModel<SimpleData, String, String, LinkData>();
      myDiagram.Model.Modifiable = true;

      GenerateGraph_Click(null, null);
    }

    Random rand = new Random();

    // Takes the random collection of nodes and creates a random graph with them.
    private ObservableCollection<LinkData> GenerateLinks(ObservableCollection<SimpleData> nodes) {
      var linkSource = new ObservableCollection<LinkData>();
      if (nodes.Count < 2) return linkSource;
      for (int i = 0; i < nodes.Count-1; i++) {
        SimpleData from = nodes[i];
        int numto = 1+rand.Next(3)/2;
        for (int j = 0; j < numto; j++) {
          int idx = i+5+rand.Next(10);
          if (idx >= nodes.Count) idx = i+rand.Next(nodes.Count-i);
          SimpleData to = nodes[idx];
          linkSource.Add(new LinkData() { From = from.Key, To = to.Key });
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
      return nodeSource;
    }

    // Generates a random graph respecting MinNodes/MaxNodes/MinLinks/MaxLinks
    private void GenerateGraph_Click(object sender, RoutedEventArgs e) {
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

    // Assume the CheckBoxes control independent flags of an enumeration,
    // with the flag value given by CheckBox.Tag being a power of two
    private void CheckBox_Checked(object sender, RoutedEventArgs e) {
      CheckBox cb = sender as CheckBox;
      if (cb != null) {
        FrameworkElement group = VisualTreeHelper.GetParent(cb) as FrameworkElement;
        if (group != null) {
          if (!(group.Tag is int)) group.Tag = 0;
          group.Tag = (int)group.Tag | int.Parse((String)cb.Tag);
        }
      }
    }

    private void CheckBox_Unchecked(object sender, RoutedEventArgs e) {
      CheckBox cb = sender as CheckBox;
      if (cb != null) {
        FrameworkElement group = VisualTreeHelper.GetParent(cb) as FrameworkElement;
        if (group != null) {
          if (!(group.Tag is int)) group.Tag = 0;
          group.Tag = (int)group.Tag & ~ int.Parse((String)cb.Tag);
        }
      }
    }
  }

  // this converts an enumeration name (string) to an enum value of type TARGETTYPE
  public class StringEnumConverter : Converter {
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
      if (value != null) {
        try {
          return Enum.Parse(targetType, value.ToString(), true);
        } catch (Exception) {
        }
      }
      return DependencyProperty.UnsetValue;
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
