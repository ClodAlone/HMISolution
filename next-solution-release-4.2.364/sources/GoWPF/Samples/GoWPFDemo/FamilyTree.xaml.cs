/* Copyright © Northwoods Software Corporation, 2008-2015. All Rights Reserved. */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.Linq;
using Northwoods.GoXam;
using Northwoods.GoXam.Model;

namespace FamilyTree {

  public partial class FamilyTree : UserControl {
    public FamilyTree() {
      InitializeComponent();

      var model = new GraphLinksModel<PersonData, String, String, UniversalLinkData>();
      model.NodeKeyPath = "Name";
      model.LinkFromPath = "From";
      model.LinkToPath = "To";
      model.LinkCategoryPath = "Category";
      List<PersonData> modelNodes = new List<PersonData>();
      using (Stream stream = Demo.MainPage.Instance.GetStream("FamilyTree", "xml")) {
        using (StreamReader reader = new StreamReader(stream)) {
          XElement root = XElement.Load(reader);
          foreach (PersonData pn in root.Descendants().OfType<XElement>().Select(x => new PersonData(x))) {
            modelNodes.Add(pn);
          }
          model.NodesSource = modelNodes;
        }
      }
      // Using the node data, create the appropriate list of link data types.
      List<UniversalLinkData> familyLinks = new List<UniversalLinkData>();
      foreach (PersonData pn in model.NodesSource) {
        if (pn.Parent != null)
          familyLinks.Add(new UniversalLinkData() { To = pn.Name, From = pn.Parent, Category = "Child" });
        if (pn.Spouses != null)
          foreach (String str in pn.Spouses)
            familyLinks.Add(new UniversalLinkData() { To = pn.Name, From = str, Category = "Marriage" });
      }

      model.LinksSource = familyLinks;
      myDiagram.Model = model;
    }
  }


  public class PersonData : GraphModelNodeData<String> {
    // this constructor initializes the Node from the given XElement node;
    // the code is necessary to provide appropriate default values
    public PersonData(XElement x) {
      this.Name = "";
      this.Details = "";
      XAttribute a;
      a = x.Attribute("note");
      if (a != null) { this.Name += a.Value; }
      a = x.Attribute("name");
      if (a != null) {
        this.Name += a.Value;
        this.Details += string.Format("Name: {0}", this.Name);
      }
      a = x.Attribute("kanjiName");
      if (a != null) {
        this.Name += string.Format("\n{0}", a.Value);
        this.Details += string.Format("\nKanji: {0}", a.Value);
      }
      a = x.Attribute("fullTitle");
      if (a != null) { this.Details += string.Format("\nTitle: {0}", a.Value); }
      a = x.Attribute("birthYear");
      if (a != null) { this.Details += string.Format("\n{0}-", a.Value); }
      a = x.Attribute("deathYear");
      if (a != null) { this.Details += string.Format("{0}", a.Value); }
      a = x.Attribute("statusChange");
      if (a != null) { this.Details += string.Format("\n{0}", a.Value); }
      a = x.Attribute("gender");
      if (a != null) { this.Gender = a.Value.ToUpper(); }
     // see if this is not the root node
      XElement parent = x.Parent;
      if (parent != null && parent.Name == "p") {
        a = parent.Attribute("name");  // get the parent node's text, assumed to be unique
        if (a != null) this.Parent = a.Value;
        a = parent.Attribute("kanjiName");
        if (a != null) this.Parent += string.Format("\n{0}", a.Value);
      }
      a = x.Attribute("spouse");
      if (a != null) {
        if (this.Spouses == null) {
          if (this.Spouses == null) this.Spouses = new List<String>() { (a.Value) };
        } else {
          this.Spouses.Add(a.Value);
        }
      }
      a = x.Attribute("spouseKanji");
      if (a != null) { this.Spouses[Spouses.Count - 1] += string.Format("\n" + a.Value); }
    }

    // the model is static, so these properties don't need to raise
    // a PropertyChanged event in their setters
    public String Gender { get; set; }  // "M" or "F"; otherwise use GenderBrushConverter.NullBrush
    public String Name { get; set; }
    public String Parent { get; set; }
    public List<String> Spouses { get; set; }
    public String Details { get; set; }
  }


  // decide which of three background colors to show for each node
  public class GenderBrushConverter : BooleanBrushConverter {
    public Brush NullBrush { get; set; }

    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
      String s = (String)value;
      if (s == "M")
        return this.TrueBrush;
      else if (s == "F")
        return this.FalseBrush;
      else
        return this.NullBrush;
    }
  }
}