using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;


namespace Monogram.WpfUtils
{
  public static class Attached
  {

    public static bool GetItemsSourceFromEnum(DependencyObject obj)
    {
      return ( bool)obj.GetValue(ItemsSourceFromEnumProperty);
    }

    public static void SetItemsSourceFromEnum(DependencyObject obj,  bool value)
    {
      obj.SetValue(ItemsSourceFromEnumProperty, value);
    }

    public static readonly DependencyProperty ItemsSourceFromEnumProperty =
        DependencyProperty.RegisterAttached("ItemsSourceFromEnum", typeof( bool), typeof(Attached), new PropertyMetadata{PropertyChangedCallback = ItemsSourceFromEnumPropertyChanged});


    public static void ItemsSourceFromEnumPropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      var list = sender as ItemsControl;
      if (list != null)
      {
        DependencyPropertyChangedEventHandler action = null;
        action = (o, args) => 
        {
          list.ItemsSource = Enum.GetValues(list.DataContext.GetType());
          list.DataContextChanged -= action;
        };
        list.DataContextChanged += action;
      }
    }
  }
}
