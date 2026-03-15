/* Copyright © Northwoods Software Corporation, 2008-2015. All Rights Reserved. */

using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Northwoods.GoXam;
using Northwoods.GoXam.Model;

namespace GLayout {
  public partial class GLayout : UserControl {
    public GLayout() {
      InitializeComponent();

      myDiagram.Model = new GraphModel<SimpleData, String>();
      myDiagram.Model.NodesSource = GenerateNodes();
      myDiagram.Model.Modifiable = true;
    }

    Random rand = new Random();

    // Creates a collection of randomly colored and sized nodes.
    private ObservableCollection<SimpleData> GenerateNodes() {
      var nodedata = new ObservableCollection<SimpleData>();
      for (int i = 0; i < 100; i++) {
        nodedata.Add(new SimpleData() {
          Key = "Node " + i.ToString(),
          Color = String.Format("#{0:X}{1:X}{2:X}", 120+rand.Next(100), 120+rand.Next(100), 120+rand.Next(100)),
          Width = 20+rand.Next(30),
          Height = 20+rand.Next(30),
        });
      }
      // Randomize the nodes a little:
      for (int i = 0; i < nodedata.Count; i++) {
        int swap = rand.Next(0, nodedata.Count);
        SimpleData temp = nodedata[swap];
        nodedata[swap] = nodedata[i];
        nodedata[i] = temp;
      }
      return nodedata;
    }

    // when a RadioButton becomes checked, set the Tag of the button container to the button's content (string)
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

  // additional properties for each node
  public class SimpleData : GraphModelNodeData<String> {
    public String Color {
      get { return _Color; }
      set { if (_Color != value) { String old = _Color; _Color = value; RaisePropertyChanged("Color", old, value); } }
    }
    private String _Color = "White";

    public double Width {
      get { return _Width; }
      set { if (_Width != value) { double old = _Width; _Width = value; RaisePropertyChanged("Width", old, value); } }
    }
    private double _Width = 50;

    public double Height {
      get { return _Height; }
      set { if (_Height != value) { double old = _Height; _Height = value; RaisePropertyChanged("Height", old, value); } }
    }
    private double _Height = 50;
  }
}
