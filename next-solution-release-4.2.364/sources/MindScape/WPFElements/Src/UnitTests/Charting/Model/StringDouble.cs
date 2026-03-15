using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace Mindscape.WpfElements.UnitTests
{
  // TODO: this should probably be deleted since WPF Elements also has this class.
  public class StringDouble : INotifyPropertyChanged
  {
    private string _string;
    private double _double;

    public StringDouble(string s, double d)
    {
      String = s;
      Double = d;
    }

    public string String
    {
      get { return _string; }
      set
      {
        if (_string != value)
        {
          _string = value;
          OnPropertyChanged("String");
        }
      }
    }

    public double Double
    {
      get { return _double; }
      set
      {
        if (_double != value)
        {
          _double = value;
          OnPropertyChanged("Double");
        }
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged(string propertyName)
    {
      PropertyChangedEventHandler handler = PropertyChanged;
      if (handler != null)
      {
        handler(this, new PropertyChangedEventArgs(propertyName));
      }
    }
  }
}
