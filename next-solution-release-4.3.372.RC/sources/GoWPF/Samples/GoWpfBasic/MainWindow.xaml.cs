/* Copyright © Northwoods Software Corporation, 2008-2015. All Rights Reserved. */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Northwoods.GoXam;
using Northwoods.GoXam.Model;

namespace Basic {
  public partial class MainWindow : Window {
    public MainWindow() {
      InitializeComponent();

      // model is a GraphLinksModel using instances of MyNodeData as the node data
      // and MyLinkData as the link data
      var model = new GraphLinksModel<MyNodeData, String, String, MyLinkData>();

      model.NodesSource = new ObservableCollection<MyNodeData>() {
          new MyNodeData() { Key="Alpha", Color="LightBlue" },
          new MyNodeData() { Key="Beta", Color="Orange" },
          new MyNodeData() { Key="Gamma", Color="LightGreen" },
          new MyNodeData() { Key="Delta", Color="Pink" }
        };

      model.LinksSource = new ObservableCollection<MyLinkData>() {
          new MyLinkData() { From="Alpha", To="Beta" },
          new MyLinkData() { From="Alpha", To="Gamma" },
          new MyLinkData() { From="Beta", To="Beta" },
          new MyLinkData() { From="Gamma", To="Delta" },
          new MyLinkData() { From="Delta", To="Alpha" }
      };

      model.Modifiable = true;
      model.HasUndoManager = true;

      myDiagram.Model = model;
      myDiagram.AllowDrop = true;

      myPalette.Model = new GraphLinksModel<MyNodeData, String, String, MyLinkData>();
      myPalette.Model.NodesSource = new List<MyNodeData>() {
        new MyNodeData() { Key="One", Color="PapayaWhip" },
        new MyNodeData() { Key="Two", Color="Lavender" },
        new MyNodeData() { Key="Three", Color="PaleGreen" },
      };

#if DOTNET4
      Touch.FrameReported += Touch_FrameReported;
#endif
    }

#if DOTNET4
    // support pinch zooming
    private void Touch_FrameReported(object sender, TouchFrameEventArgs e) {
      var touchpoints = e.GetTouchPoints(myDiagram.Panel);
      if (touchpoints.Count > 1) {
        var tp0 = touchpoints[0].Position;
        var tp1 = touchpoints[1].Position;
        var newDist = Math.Sqrt((tp0.X-tp1.X)*(tp0.X-tp1.X) + (tp0.Y-tp1.Y)*(tp0.Y-tp1.Y));
        if (Double.IsNaN(startDist)) {
          startDist = newDist;
          startScale = myDiagram.Panel.Scale;
          var primary = e.GetPrimaryTouchPoint(myDiagram.Panel);
          if (primary != null) myDiagram.Panel.ZoomPoint = primary.Position;
        } else {
          myDiagram.Panel.Scale = startScale * newDist / startDist;
        }
      } else {
        startDist = Double.NaN;
      }
    }
    private double startDist = Double.NaN;
    private double startScale = 1;
#endif  // DOTNET4

    private void NodeMenuClick(object sender, RoutedEventArgs e) {
      var elt = sender as FrameworkElement;
      if (elt != null && elt.DataContext != null) {
        var data = ((PartManager.PartBinding)elt.DataContext).Data as MyNodeData;
        MessageBox.Show("Node color: " + data.Color);
      }
    }

    private void LinkMenuClick(object sender, RoutedEventArgs e) {
      var elt = sender as FrameworkElement;
      if (elt != null && elt.DataContext != null) {
        var data = ((PartManager.PartBinding)elt.DataContext).Data as MyLinkData;
        MessageBox.Show("Link: " + data.ToString());
      }
    }

    private void BackgroundMenuClick(object sender, RoutedEventArgs e) {
      Point p = myDiagram.LastMousePointInModel;
      MessageBox.Show("Clicked at: " + p.ToString());
    }
  }


  // Define custom node data; the node key is of type String.
  // Add a property named Color that might change.
  [Serializable]  // serializable in WPF to support the clipboard
  public class MyNodeData : GraphLinksModelNodeData<String> {
    public String Color {
      get { return _Color; }
      set {
        if (_Color != value) {
          String old = _Color;
          _Color = value;
          RaisePropertyChanged("Color", old, value);
        }
      }
    }
    private String _Color = "White";
  }

  // Define custom link data; the node key is of type String,
  // the port key should be of type String but is unused in this app.
  [Serializable]  // serializable in WPF to support the clipboard
  public class MyLinkData : GraphLinksModelLinkData<String, String> {
    // nothing to add
  }
}
