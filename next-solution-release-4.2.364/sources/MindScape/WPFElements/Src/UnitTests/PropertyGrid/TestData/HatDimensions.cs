using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Media;

namespace Mindscape.WpfElements.WpfPropertyGrid.UnitTests
{
  //[TypeConverter(typeof(HatDimensionsTypeConverter))]
  public struct HatDimensions : INotifyPropertyChanged
  {
    public HatDimensions(int brimSize, int height, string manufacturer, Color color)
      : this()
    {
      _brimSize = brimSize;
      _height = height;
      _manufacturer = manufacturer;
      _color = color;
    }

    private int _brimSize;

    public int BrimSize
    {
      get { return _brimSize; }
      set
      {
        Set(ref _brimSize, value, "BrimSize");
        Height = _brimSize * 2;
      }
    }

    private int _height;

    public int Height
    {
      get { return _height; }
      set { Set(ref _height, value, "Height"); }
    }

    private string _manufacturer;

    public string Manufacturer
    {
      get { return _manufacturer; }
      set { Set(ref _manufacturer, value, "Manufacturer"); }
    }

    private Color _color;

    public Color Color
    {
      get { return _color; }
      set { Set(ref _color, value, "Color"); }
    }

    public override string ToString()
    {
      return String.Format("B={0}, H={1}, M={2}, C={3}", _brimSize, _height, _manufacturer, _color);
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged(string propertyName)
    {
      if (PropertyChanged != null)
      {
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }
    }

    private void Set<T>(ref T field, T value, string propertyName)
    {
      if (!Object.Equals(field, value))
      {
        field = value;
        OnPropertyChanged(propertyName);
      }
    }

    public class HatDimensionsTypeConverter : TypeConverter
    {
      public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
      {
        return true;
      }

      public override object CreateInstance(ITypeDescriptorContext context, System.Collections.IDictionary propertyValues)
      {
        //System.Diagnostics.Debug.WriteLine("Creating a new HatDimensions using TypeConverter");

        int brimSize = (int)(propertyValues["BrimSize"]);
        int height = (int)(propertyValues["Height"]);
        string manufacturer = (string)(propertyValues["Manufacturer"]);
        Color color = (Color)(propertyValues["Color"]);
        return new HatDimensions(brimSize, height, manufacturer, color);
      }
    }
  }
}
