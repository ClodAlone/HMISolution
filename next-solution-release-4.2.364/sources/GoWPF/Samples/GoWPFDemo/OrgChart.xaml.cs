/* Copyright © Northwoods Software Corporation, 2008-2015. All Rights Reserved. */

using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using Northwoods.GoXam;
using Northwoods.GoXam.Model;

namespace OrgChart {
  public partial class OrgChart : UserControl {

    public OrgChart() {
      InitializeComponent();
      var model = new TreeModel<OrgMember, String>();
      model.NodeKeyPath = "Key";
      model.ParentNodePath = "Parent";

      using (Stream stream = Demo.MainPage.Instance.GetStream("OrgChart", "xml")) {
        using (StreamReader reader = new StreamReader(stream)) {
          XElement root = XElement.Load(reader);
          XAttribute xa = root.Attribute("Date");
          if (xa != null) this.Date = xa.Value;
          model.NodesSource = root.Descendants().OfType<XElement>().Select(x => new OrgMember(x)).ToList();
        }
      }
      myDiagram.Model = model;

      // Data-bind Slider.Value to Diagram.Panel.Scale, once the Diagram.Panel
      // has been created by expanding the Diagram's ControlTemplate.
      // The Slider uses a logarithmic scale via the LogConverter defined below.
      myDiagram.TemplateApplied += (s, e) => {
        var b = new System.Windows.Data.Binding("Scale");
        b.Source = myDiagram.Panel;
        b.Mode = System.Windows.Data.BindingMode.TwoWay;
        b.Converter = new LogConverter();
        myLogScaleSlider.SetBinding(Slider.ValueProperty, b);
      };

      myInfo.DataContext = this;
    }

    public String Date { get; set; }
  }


  public class LogConverter : Converter {
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
      return Math.Log((double)value, 2);
    }

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
      return Math.Pow(2, (double)value);
    }
  }

   
  public class OrgMember : GraphModelNodeData<String> {
    public OrgMember(XElement xe) {
      XAttribute xa = xe.Attribute("Name");
      if (xa != null) {
        // Set a Node Data's Key to the Name attribute in the XML; assumed to be unique
        this.Key = xa.Value;
        this.Name = xa.Value;
      }
      xa = xe.Attribute("Nation");
      if (xa != null) {
        String filename = "Demo.source.Flags." + xa.Value.ToLower().Replace(' ', '-') + "-flag.Png";
        using (Stream strm = typeof(OrgChart).Assembly.GetManifestResourceStream(filename)) {
#if SILVERLIGHT  // Cannot use PngBitmapDecoder in Silverlight
          BitmapImage bmpi = new BitmapImage();
          bmpi.SetSource(strm);
#else
          BitmapSource bmpi = null;
          PngBitmapDecoder bd = new PngBitmapDecoder(strm,
              BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.None);
          bmpi = bd.Frames[0];
#endif
          this.Flag = bmpi;
        }
      }

      // PersonData will be displayed in the Node.
      // We add each attribute to this String, along with a new line, so that the
      // Node contains all the information arranged in a standard list format
      xa = xe.Attribute("Title");
      if (xa != null) this.PersonData += string.Format("\nTitle: {0}", xa.Value);
      xa = xe.Attribute("HeadOf");
      if (xa != null) this.PersonData += string.Format("\nHead Of: {0}", xa.Value);
      xa = xe.Attribute("RankNumber");
      if (xa != null) this.PersonData += string.Format("\nRank: {0}", xa.Value);
      if (xe.Parent != null) {
        xa = xe.Parent.Attribute("Name");
        if (xa != null) {
          this.Parent = xa.Value;
          this.PersonData += string.Format("\nReporting To: {0}", xa.Value);
        }
      }
    }

    public String Name {get; set;}
    public String Parent { get; set; }
    public String PersonData { get; set; }
    public BitmapSource Flag { get; set; }
  }


  // Takes a BitmapSource from the Node Data and converts it into a double for the
  // Image's width in the Node. This converter assumes a constant height for all Images
  // (flags in this case) and returns the appropriate width so that the flag's ratio is maintained.
  public class ImageSizeConverter : Converter {
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
      BitmapSource bmp = value as BitmapSource;
      if (bmp != null) {
#if SILVERLIGHT  // BitmapSource.Width and .Height do not exist in Silverlight
        return ((double)(bmp.PixelWidth) / (double)(bmp.PixelHeight) * 30 + 4);
#else
        return (bmp.Width / bmp.Height) * 30 + 4;
#endif
      }
      return 0;
    }
  }
}