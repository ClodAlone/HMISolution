/* Copyright © Northwoods Software Corporation, 2008-2015. All Rights Reserved. */

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Linq;
using Northwoods.GoXam;
using Northwoods.GoXam.Model;

namespace OrgChartEditor {
  public partial class OrgChartEditor : UserControl {
    public OrgChartEditor() {
      InitializeComponent();

      // create the initial model
      var model = new TreeModel<Employee, int>();
      model.ChildNodesPath = "";  // don't use each node data's collection of children
      // initialize it from data in an XML file that is an embedded resource
      String xml = Demo.MainPage.Instance.LoadText("OrgChartEditor", "xml");
      model.Load<Employee>(XElement.Parse(xml), "Employee");
      model.Modifiable = true;  // allow user modification
      model.HasUndoManager = true;  // support undo/redo
      myDiagram.Model = model;
    }

    private void Node_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
      // Only executes logic if you have double-clicked.
      if (DiagramPanel.IsDoubleClick(e)) {
        e.Handled = true;
        Node clicked = Part.FindAncestor<Node>(sender as UIElement);
        if (clicked != null) {
          Employee thisemp = (Employee)clicked.Data;
          myDiagram.StartTransaction("New Employee");

          Employee newemp = new Employee() {
            Key=1,  // can't use zero, because zero means "no boss"
            Name="(new person)",
            ParentKey=thisemp.Key,  // the clicked employee is the new boss
            Location=Spot.BottomLeft.PointInRect(clicked.Bounds)  // start underneath
          };
          myDiagram.Model.AddNode(newemp);

          myDiagram.CommitTransaction("New Employee");
        }
      }
    }

    // save and load the model data as XML, visible in the "Saved" tab of the Demo
    private void Save_Click(object sender, RoutedEventArgs e) {
      var model = myDiagram.Model as TreeModel<Employee, int>;
      if (model == null) return;
      XElement root = model.Save<Employee>("OrgChart", "Employee");
      Demo.MainPage.Instance.SavedXML = root.ToString();
      LoadButton.IsEnabled = true;
      model.IsModified = false;
    }

    private void Load_Click(object sender, RoutedEventArgs e) {
      var model = myDiagram.Model as TreeModel<Employee, int>;
      if (model == null) return;
      try {
        XElement root = XElement.Parse(Demo.MainPage.Instance.SavedXML);
        model.Load<Employee>(root, "Employee");
      } catch (Exception ex) {
        MessageBox.Show(ex.ToString());
      }
      model.IsModified = false;
      myDiagram.LayoutDiagram(); // after loading, ensure correct layout
      model.UndoManager.Clear();
    }
  }


  // define additional properties for each employee

  public class Employee : TreeModelNodeData<int> {
    public String Name {
      get { return _Name; }
      set {
        if (_Name != value) {
          String old = _Name;
          _Name = value;
          RaisePropertyChanged("Name", old, value);
        }
      }
    }
    private String _Name = "";

    public String Title {
      get { return _Title; }
      set {
        if (_Title != value) {
          String old = _Title;
          _Title = value;
          RaisePropertyChanged("Title", old, value);
        }
      }
    }
    private String _Title = "";

    public String Comments {
      get { return _Comments; }
      set {
        if (_Comments != value) {
          String old = _Comments;
          _Comments = value;
          RaisePropertyChanged("Comments", old, value);
        }
      }
    }
    private String _Comments = "";

    // write and read Linq for XML attributes for the additional Employee properties
    public override XElement MakeXElement(XName n) {
      XElement xe = base.MakeXElement(n);
      xe.Add(XHelper.Attribute("Name", this.Name, ""));
      xe.Add(XHelper.Attribute("Title", this.Title, ""));
      xe.Add(XHelper.Attribute("Comments", this.Comments, ""));
      return xe;
    }

    public override void LoadFromXElement(XElement e) {
      base.LoadFromXElement(e);
      this.Name = XHelper.Read("Name", e, "");
      this.Title = XHelper.Read("Title", e, "");
      this.Comments = XHelper.Read("Comments", e, "");
    }
  }
}
