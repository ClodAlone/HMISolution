/* Copyright © Northwoods Software Corporation, 2008-2015. All Rights Reserved. */

using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using Northwoods.GoXam;
using Northwoods.GoXam.Model;

namespace BeatPaths {
  public partial class BeatPaths : UserControl {
    public BeatPaths() {
      InitializeComponent();

      // create a model where the node data is the abbreviated team name
      // and the link data are instances of Beat, defined below
      var model = new GraphLinksModel<String, String, String, Beat>();
      // assume this model is not Modifiable
      // link references to nodes are the same as the node data itself (the team names)
      model.NodeKeyIsNodeData = true;
      // automatically initialize NodesSource from the references in the link data
      model.NodeKeyReferenceAutoInserts = true;
      // specify the two properties of the Beat link data class
      model.LinkFromPath = "Winner";
      model.LinkToPath = "Loser";

      // load the XML data from a file that is an embedded resource
      using (Stream stream = Demo.MainPage.Instance.GetStream("BeatPaths", "xml")) {
        using (StreamReader reader = new StreamReader(stream)) {
          XElement root = XElement.Load(reader);

          // Iterate over all the nested elements inside the root element
          // collect a new Beat() for each XElement,
          // remembering the interesting attribute values.
          // Call ToList() to avoid recomputation of deferred Linq Select operation
          model.LinksSource = root.Nodes()
            .OfType<XElement>()
            .Select(x => new Beat() {
              Winner=x.Attribute("w").Value,
              Loser=x.Attribute("l").Value
            })
            .ToList();

          // don't forget to have the Diagram use this model!
          myDiagram.Model = model;
        }
      }
    }
  }

  // need this regular class for link data to avoid MethodAccessExceptions
  // trying to read properties of anonymous types in Silverlight
  public class Beat {
    public String Winner { get; set; }
    public String Loser { get; set; }
  }

  // convert the simple name of a team to a BitmapImage referring to a particular icon
  public class TextImageSourceConverter : Converter {
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
      // because the images are stored on our web site, if this is running
      // with limited permissions, you will not see these images unless this
      // application was hosted by our web site
      return new BitmapImage(new Uri("http://www.goxam.com/go/beatpaths/" +
                                     (value ?? "NE").ToString() + "_logo-50x50.png"));
    }
  }

}
