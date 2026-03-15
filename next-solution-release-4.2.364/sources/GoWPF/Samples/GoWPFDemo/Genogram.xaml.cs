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
using Northwoods.GoXam.Layout;
using Northwoods.GoXam.Model;

namespace Genogram {
  public partial class Genogram : UserControl {
    public Genogram() {
      InitializeComponent();

      // load the XML data from a file that is an embedded resource
      using (Stream stream = Demo.MainPage.Instance.GetStream("Genogram", "xml")) {
        using (StreamReader reader = new StreamReader(stream)) {
          XElement root = XElement.Load(reader);

          // Iterate over all the nested elements inside the root element
          // collect a new Person for each XElement,
          // intepreting and remembering the interesting attribute values.
          var people = new List<Person>();
          foreach (var x in root.Nodes().OfType<XElement>()) {
            var person = new Person();
            person.Key = ParseInt(x.Attribute("id"));
            person.Text = x.Attribute("n").Value;
            person.Sex = x.Attribute("s").Value;
            person.Mother = ParseInt(x.Attribute("m"));
            person.Father = ParseInt(x.Attribute("f"));
            person.Wives = ParseInts(x.Attribute("ux"));
            person.Husbands = ParseInts(x.Attribute("vir"));
            person.Attributes = ParseAtts(x.Attribute("a"));
            people.Add(person);
          }

          SetupDiagram(people);
        }
      }
    }

    private int ParseInt(XAttribute v) {
      if (v != null) {
        try {
          return int.Parse(v.Value);
        } catch (Exception) {
          System.Diagnostics.Debug.WriteLine("Unable to parse person id: " + v.Value);
        }
      }
      return -1;
    }

    private List<int> ParseInts(XAttribute v) {
      if (v != null) {
        try {
          var a = v.Value.Split(' ');
          return a.Select(x => int.Parse(x)).Where(x => x != -1).ToList();
        } catch (Exception) {
          System.Diagnostics.Debug.WriteLine("Unable to parse list of person ids: " + v.Value);
        }
      }
      return new List<int>();
    }

    private List<string> ParseAtts(XAttribute v) {
      if (v != null) {
        try {
          var a = v.Value.Split(' ');
          return a.Where(x => x != "").ToList();
        } catch (Exception) {
          System.Diagnostics.Debug.WriteLine("Unable to parse list of attributes: " + v.Value);
        }
      }
      return new List<string>();
    }

    private void SetupDiagram(List<Person> people) {
      myDiagram.LayoutManager = new GenogramLayoutManager();

      var model = new Family();
      model.NodeCategoryPath = "Sex";
      model.Modifiable = true;
      model.NodesSource = people;
      SetupMarriages(model);
      SetupParents(model);
      myDiagram.Model = model;
    }

    Relationship FindMarriage(Family model, int a, int b) {
      var nodeA = model.FindNodeByKey(a);
      var nodeB = model.FindNodeByKey(b);
      if (nodeA != null && nodeB != null) {
        Relationship mrel = null;
        // look for a link from A to B
        foreach (var rel in model.GetLinksBetweenNodes(nodeA, null, nodeB, null)) {
          if (rel != null && rel.Category == "Marriage") {
            mrel = rel;
            break;
          }
        }
        if (mrel == null) {  // look for a link from B to A
          foreach (var rel in model.GetLinksBetweenNodes(nodeB, null, nodeA, null)) {
            if (rel != null && rel.Category == "Marriage") {
              mrel = rel;
              break;
            }
          }
        }
        return mrel;
      }
      return null;
    }

    // now process the node data to determine marriages
    void SetupMarriages(Family model) {
      var people = model.NodesSource as IList<Person>;
      var numpeople = people.Count;
      for (int i = 0; i < numpeople; i++) {
        var person = people[i];
        var key = person.Key;
        var uxs = person.Wives;
        if (uxs != null) {
          foreach (var wife in uxs) {
            if (key == wife) {
              // or warn no reflexive marriages
              continue;
            }
            var rel = FindMarriage(model, key, wife);
            if (rel == null) {
              // add a label node for the marriage link
              var mlab = new Person() { IsLinkLabel = true, Sex = "LinkLabel" };
              model.AddNode(mlab);
              // add the marriage link itself, also referring to the label node
              var mrel = new Relationship() { From = key, To = wife, LabelNode = mlab.Key, Category = "Marriage" };
              model.AddLink(mrel);
            }
          }
        }
        var virs = person.Husbands;
        if (virs != null) {
          foreach (var husband in virs) {
            if (key == husband) {
              // or warn no reflexive marriages
              continue;
            }
            var rel = FindMarriage(model, key, husband);
            if (rel == null) {
              // add a label node for the marriage link
              var mlab = new Person() { IsLinkLabel = true, Sex = "LinkLabel" };
              model.AddNode(mlab);
              // add the marriage link itself, also referring to the label node
              var mrel = new Relationship() { From = key, To = husband, LabelNode = mlab.Key, Category = "Marriage" };
              model.AddLink(mrel);
            }
          }
        }
      }
    }

    // process parent-child relationships once all marriages are known
    void SetupParents(Family model) {
      var people = model.NodesSource as IList<Person>;
      foreach (var person in people) {
        var key = person.Key;
        var mother = person.Mother;
        var father = person.Father;
        if (mother != -1 && father != -1) {
          var mrel = FindMarriage(model, mother, father);
          if (mrel == null) {
            // or warn no known mother or no known father or no known marriage between them
            continue;
          }
          var mlabkey = mrel.LabelNode;
          var crel = new Relationship() { From = mlabkey, To = key, Category = "ParentChild" };
          model.AddLink(crel);
        }
      }
    }
  }

  public class Family : GraphLinksModel<Person, int, string, Relationship> {
  }

  public class Person : GraphLinksModelNodeData<int> {
    public Person() {
      this.Key = -1;
      this.Sex = "?";
      this.Mother = -1;
      this.Father = -1;
      this.Wives = new List<int>();
      this.Husbands = new List<int>();
      this.Attributes = new List<string>();
    }
    public String Sex { get; set; }
    public int Mother { get; set; }
    public int Father { get; set; }
    public IList<int> Wives { get; set; }
    public IList<int> Husbands { get; set; }
    public IList<string> Attributes { get; set; }
  }

  public class Relationship : GraphLinksModelLinkData<int, string> {
  }

  public class AttributeFillConverter : Converter {
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
      if (value is string) {
        switch ((string)value) {
          case "A": return "green";
          case "B": return "orange";
          case "C": return "red";
          case "D": return "cyan";
          case "E": return "gold";
          case "F": return "pink";
          case "G": return "blue";
          case "H": return "brown";
          case "I": return "purple";
          case "J": return "chartreuse";
          case "K": return "lightgray";
          case "L": return "magenta";
          default: break;
        }
      }
      return "Transparent";
    }
  }

  public class MaleGeometryConverter : Converter {
    private Geometry TL() { return new RectangleGeometry() { Rect = new Rect(1, 1, 19, 19) }; }
    private Geometry TR() { return new RectangleGeometry() { Rect = new Rect(20, 1, 19, 19) }; }
    private Geometry BR() { return new RectangleGeometry() { Rect = new Rect(20, 20, 19, 19) }; }
    private Geometry BL() { return new RectangleGeometry() { Rect = new Rect(1, 20, 19, 19) }; }

    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
      if (value is string) {
        switch ((string)value) {
          case "A": return TL();
          case "B": return TL();
          case "C": return TL();
          case "D": return TR();
          case "E": return TR();
          case "F": return TR();
          case "G": return BR();
          case "H": return BR();
          case "I": return BR();
          case "J": return BL();
          case "K": return BL();
          case "L": return BL();
          default: break;
        }
      }
      return "";
    }
  }

  public class FemaleGeometryConverter : Converter {
    private Geometry TL() {
      return new PathGeometry() {
        Figures = new PathFigureCollection() {
          new PathFigure() {
            StartPoint = new Point(20, 20),
            Segments = new PathSegmentCollection() {
              new LineSegment() { Point = new Point(1, 20) },
              new ArcSegment() { Size = new Size(19, 19), Point = new Point(20, 1), SweepDirection = SweepDirection.Clockwise }
            },
            IsClosed = true
          }
        }
      };
    }

    private Geometry TR() {
      return new PathGeometry() {
        Figures = new PathFigureCollection() {
          new PathFigure() {
            StartPoint = new Point(20, 20),
            Segments = new PathSegmentCollection() {
              new LineSegment() { Point = new Point(20, 1) },
              new ArcSegment() { Size = new Size(19, 19), Point = new Point(39, 20), SweepDirection = SweepDirection.Clockwise }
            },
            IsClosed = true
          }
        }
      };
    }

    private Geometry BR() {
      return new PathGeometry() {
        Figures = new PathFigureCollection() {
          new PathFigure() {
            StartPoint = new Point(20, 20),
            Segments = new PathSegmentCollection() {
              new LineSegment() { Point = new Point(39, 20) },
              new ArcSegment() { Size = new Size(19, 19), Point = new Point(20, 39), SweepDirection = SweepDirection.Clockwise }
            },
            IsClosed = true
          }
        }
      };
    }

    private Geometry BL() {
      return new PathGeometry() {
        Figures = new PathFigureCollection() {
          new PathFigure() {
            StartPoint = new Point(20, 20),
            Segments = new PathSegmentCollection() {
              new LineSegment() { Point = new Point(20, 39) },
              new ArcSegment() { Size = new Size(19, 19), Point = new Point(1, 20), SweepDirection = SweepDirection.Clockwise }
            },
            IsClosed = true
          }
        }
      };
    }

    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
      if (value is string) {
        switch ((string)value) {
          case "A": return TL();
          case "B": return TL();
          case "C": return TL();
          case "D": return TR();
          case "E": return TR();
          case "F": return TR();
          case "G": return BR();
          case "H": return BR();
          case "I": return BR();
          case "J": return BL();
          case "K": return BL();
          case "L": return BL();
          default: break;
        }
      }
      return "";
    }
  }

  public class GenogramLayoutManager : LayoutManager {
    public override bool CanLayoutPart(Part p, IDiagramLayout lay) {
      // allow label nodes to be laid out
      var n = p as Node;
      if (n != null && n.IsLinkLabel) return true;
      return base.CanLayoutPart(p, lay);
    }
  }

  public class GenogramLayout : TreeLayout {
    public override TreeNetwork MakeNetwork(IEnumerable<Node> nodes, IEnumerable<Link> links) {
      //return base.MakeNetwork(nodes, links);
      var net = CreateNetwork();
      foreach (var node in nodes) {
        // if it's an unmarried Node, or if it's a Link Label Node, create a LayoutVertex for it
        if (node.IsLinkLabel) {
          // get marriage Link
          var link = node.LabeledLink;
          var spouseA = link.FromNode;
          var spouseB = link.ToNode;
          // create vertex representing both husband and wife
          var vertex = net.AddNode(node);
          // now define the vertex size to be big enough to hold both spouses
          vertex.Bounds = new Rect(0, 0, spouseA.Bounds.Width + 30 + spouseB.Bounds.Width,
                                         Math.Max(spouseA.Bounds.Height, spouseB.Bounds.Height));
          vertex.Focus = new Point(spouseA.Bounds.Width + 30/2, vertex.Height/2);
        } else {
          var anymarriage = false;
          foreach (var link in node.LinksConnected) {
            if (link.LabelNode != null) {  // assume a marriage Link has a label Node
              anymarriage = true;
              break;
            }
          }
          if (!anymarriage) {
            var vertex = net.AddNode(node);
          }
        }
      }
      foreach (var link in links) {
        // if it's a parent-child link, add a LayoutEdge for it
        if (link.LabelNode == null) {
          var parent = net.FindVertex(link.FromNode);  // should be a label node
          var child = net.FindVertex(link.ToNode);
          if (child != null) {
            net.LinkVertexes(parent, child, link);
          } else {  // to a married person
            foreach (var l in link.ToNode.LinksConnected) {
              if (((Relationship)l.Data).Category == "Marriage") {
                if (l.LabelNode != null) {
                  var mlabvert = net.FindVertex(l.LabelNode);
                  if (mlabvert != null) {
                    net.LinkVertexes(parent, mlabvert, link);
                  }
                }
              }
            }
          }
        }
      }
      return net;
    }

    protected override void LayoutNodes() {
      foreach (var v in this.Network.Vertexes) {
        if (((Person)v.Node.Data).IsLinkLabel) {
          var labnode = v.Node;
          var lablink = labnode.LabeledLink;
          var spouseA = lablink.FromNode;
          var spouseB = lablink.ToNode;
          spouseA.Position = v.Position;
          spouseB.Position = new Point(v.Bounds.X + spouseA.Bounds.Width + 30, v.Bounds.Y);
        } else {
          v.Node.Position = v.Position;
        }
      }
    }
  }
}
