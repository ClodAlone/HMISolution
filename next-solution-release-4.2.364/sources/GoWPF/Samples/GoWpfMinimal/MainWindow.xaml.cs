/* Copyright © Northwoods Software Corporation, 2008-2015. All Rights Reserved. */

using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Northwoods.GoXam.Model;

namespace Minimal {
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
