/* Copyright © Northwoods Software Corporation, 2008-2015. All Rights Reserved. */

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Northwoods.GoXam;
using Northwoods.GoXam.Layout;
using Northwoods.GoXam.Model;

namespace CLayout {
  public partial class CLayout : UserControl {
    public CLayout() {
      InitializeComponent();
      CreateModel();
    }

    private Random rand = new Random();
    private const int MAXSIZE = 65;

    private void CreateModel() {
      int num = int.Parse(this.NodeNumTextBox.Text);
      int wid = int.Parse(this.WidthTextBox.Text);
      int hei = int.Parse(this.HeightTextBox.Text);
      int min = int.Parse(this.MinLinksTextBox.Text);
      int max = int.Parse(this.MaxLinksTextBox.Text);
      bool randsizes = this.RandomSizesCheckBox.IsChecked == true;
      bool circ = this.RectangularNodesCheckBox.IsChecked == false;
      bool cyclic = this.CyclicGraphCheckBox.IsChecked == true;

      var model = new GraphModel<Data, string>();
      List<Data> ns = new List<Data>();
      for (int i = 0; i < num; i++) {
        Data d = new Data() {
          Key = i.ToString(),
          Color = String.Format("#{0:X}{1:X}{2:X}", 120+rand.Next(100), 120+rand.Next(100), 120+rand.Next(100)),
        };
        if (cyclic) {
          if (i >= num-1)
            d.ToKeys.Add("0");
          else
            d.ToKeys.Add((i+1).ToString());
        } else {
          int linknum = rand.Next(min, max + 1);
          for (int g = 0; g < linknum; g++) {
            d.ToKeys.Add(rand.Next(num).ToString());
          }
        }
        d.Width = randsizes ? rand.Next(wid, MAXSIZE) : wid;
        if (circ) d.Height = d.Width;
        else d.Height = randsizes ? rand.Next(hei, MAXSIZE) : hei;

        ns.Add(d);
      }
      model.NodesSource = ns;
      model.Modifiable = true;
      myDiagram.Model = model;
    }

    public void BuildModel(object sender, EventArgs e) {
      CreateModel();
      if (RectangularNodesCheckBox.IsChecked == true) {
        this.myDiagram.NodeTemplate = Diagram.FindResource<DataTemplate>(myDiagram, "RectangularTemplate");
      } else {
        this.myDiagram.NodeTemplate = Diagram.FindResource<DataTemplate>(myDiagram, "EllipticalTemplate");
      }
    }

    private void Button_Checked(object sender, RoutedEventArgs e) {
      RadioButton button = (RadioButton)sender;
      FrameworkElement parent = VisualTreeHelper.GetParent(button) as FrameworkElement;
      if (parent != null) parent.Tag = button.Content;
    }
  }

  public class Data : GraphModelNodeData<String> {
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

  public class AreaComparer : IComparer<CircularVertex> {
    public int Compare(CircularVertex cv1, CircularVertex cv2) {
      Node n1 = cv1.Node;
      Node n2 = cv2.Node;
      double a1 = n1.ActualWidth * n1.ActualHeight;
      double a2 = n2.ActualWidth * n2.ActualHeight;
      return a1 > a2 ? 1 : (a1 < a2 ? -1 : 0);
    }
  }
}
