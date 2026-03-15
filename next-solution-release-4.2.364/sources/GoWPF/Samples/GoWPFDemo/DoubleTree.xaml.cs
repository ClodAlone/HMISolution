/* Copyright © Northwoods Software Corporation, 2008-2015. All Rights Reserved. */

using System;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Xml.Linq;
using Northwoods.GoXam.Model;

namespace DoubleTree {
  public partial class DoubleTree : UserControl {
    public DoubleTree() {
      InitializeComponent();

      // create a TreeModel holding data mirroring nested XML data
      var model = new TreeModel<Info, String>();
      model.NodeKeyPath = "Text";
      model.ParentNodePath = "Parent";

      // load the XML data from a file that is an embedded resource
      using (Stream stream = Demo.MainPage.Instance.GetStream("DoubleTree", "xml")) {
        using (StreamReader reader = new StreamReader(stream)) {
          XElement root = XElement.Load(reader);
          // iterate over all the nested elements inside the root element
          // collect a new Info() for each XElement, remembering the interesting attribute values
          // need to call ToList() to avoid recomputation of deferred Linq Select operation
          model.NodesSource = root.Descendants().OfType<XElement>().Select(x => new Info(x)).ToList();
          myDiagram.Model = model;
        }
      }
    }
  }

  // need this regular class to avoid MethodAccessExceptions trying to
  // read properties of anonymous types in Silverlight
  public class Info {
    // this constructor initializes the Info from the given XElement node;
    // the code is necessary to provide appropriate default values
    public Info(XElement x) {
      XAttribute a;
      a = x.Attribute("text");
      if (a != null) this.Text = a.Value; else this.Text = "";
      a = x.Attribute("color");
      if (a != null) this.Color = a.Value; else this.Color = "Transparent";
      // see if this is not the root node
      XElement parent = x.Parent;
      if (parent != null && parent.Name == "node") {
        a = parent.Attribute("text");  // get the parent node's text, assumed to be unique
        if (a != null) this.Parent = a.Value;
        this.LayoutId = Dir(x);  // laid out by the TreeLayout identified by the direction
      } else {
        this.LayoutId = "All";  // the root node participates in all layouts
      }
    }

    // recurse up the XElement tree to find a <node> with a "dir" attribute
    private String Dir(XElement x) {
      if (x != null && x.Name == "node") {
        // if it has a "dir" attribute, return its value
        XAttribute a = x.Attribute("dir");
        if (a != null) return a.Value;
      }
      // try going up the parent chain
      if (x.Parent != null) return Dir(x.Parent);
      return "Right";  // unknown direction: default towards the right
    }

    // the model is assumed to be static,
    // so these properties don't need to raise a PropertyChanged event in their setters
    public String Text { get; set; }
    public String Color { get; set; }
    public String LayoutId { get; set; }
    public String Parent { get; set; }
  }
}
