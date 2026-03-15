using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;


namespace Monogram.WpfUtils
{
 

  public static class DataBinder
  {
    private static readonly DependencyProperty DummyProperty = DependencyProperty.RegisterAttached(
        "Dummy",
        typeof(Object),
        typeof(DependencyObject),
        new UIPropertyMetadata(null));

    public static Object Eval(DependencyObject container, Binding binding)
    {
      try
      {
        binding.IsAsync = false;
      }
      catch { };
      BindingOperations.SetBinding(container, DummyProperty, binding);
      return container.GetValue(DummyProperty);
    }
  }
}
