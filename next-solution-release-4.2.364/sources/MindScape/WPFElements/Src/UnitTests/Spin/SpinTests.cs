using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows;
using System.Windows.Input;

namespace Mindscape.WpfElements.UnitTests
{
  [TestFixture]
  public class SpinTests
  {
    public class Spinnable : INotifyPropertyChanged
    {
      public event PropertyChangedEventHandler PropertyChanged;

      protected virtual void OnPropertyChanged(string propertyName)
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

      private decimal _amount;

      public decimal Amount
      {
        get { return _amount; }
        set { Set(ref _amount, value, "Amount"); }
      }
    }

    [Test]
    [STAThread]
    public void SpinValue()
    {
      Spinnable spinnable = new Spinnable();
      spinnable.Amount = 123.45m;

      Spin spinner = new Spin();
      spinner.Change = 11.1m;
      spinner.DataContext = spinnable;

      spinner.SetBinding(Spin.ValueProperty, "Amount");

      Assert.AreEqual(123.45m, spinner.Value);

      SpinCommands.Increase.Execute(null, spinner);
      Assert.AreEqual(134.55m, spinner.Value);
      Assert.AreEqual(134.55m, spinnable.Amount);

      SpinCommands.Decrease.Execute(null, spinner);
      Assert.AreEqual(123.45m, spinner.Value);
      Assert.AreEqual(123.45m, spinnable.Amount);
    }

    [Test]
    [STAThread]
    public void MaximumAndMinimum()
    {
      Spinnable spinnable = new Spinnable();
      spinnable.Amount = 123.45m;

      Spin spinner = new Spin();
      spinner.Maximum = 130.00m;
      spinner.Minimum = 120.00m;
      spinner.DataContext = spinnable;

      spinner.SetBinding(Spin.ValueProperty, "Amount");

      spinner.Change = 10m;
      SpinCommands.Increase.Execute(null, spinner);
      Assert.AreEqual(123.45m, spinner.Value);

      spinner.Change = 5m;
      SpinCommands.Increase.Execute(null, spinner);
      Assert.AreEqual(128.45m, spinner.Value);

      spinner.Change = 10m;
      SpinCommands.Decrease.Execute(null, spinner);
      Assert.AreEqual(128.45m, spinner.Value);

      spinner.Change = 5m;
      SpinCommands.Decrease.Execute(null, spinner);
      Assert.AreEqual(123.45m, spinner.Value);
    }

    private static FrameworkElement FindDescendant(FrameworkElement startFrom, Type elementType, string elementName)
    {
      if (startFrom.GetType() == elementType
        && startFrom.Name == elementName)
      {
        return startFrom;
      }

      for (int i = 0; i < VisualTreeHelper.GetChildrenCount(startFrom); ++i)
      {
        FrameworkElement child = VisualTreeHelper.GetChild(startFrom, i) as FrameworkElement;
        FrameworkElement foundDescendant = FindDescendant(child, elementType, elementName);
        if (foundDescendant != null)
        {
          return foundDescendant;
        }
      }

      return null;
    }

    [Test]
    [STAThread]
    public void Decorator()
    {
      NumericTextBox ntb = new NumericTextBox();
      ntb.Value = 123m;

      SpinDecorator decorator = new SpinDecorator();
      decorator.Change = 10;
      decorator.Content = ntb;

      decorator.ApplyTemplate();

      FrameworkElement spinner = FindDescendant(decorator, typeof(Spin), "PART_Spin");
      Assert.IsInstanceOf<Spin>(spinner);

      SpinCommands.Increase.Execute(null, spinner);
      Assert.AreEqual(133m, ntb.Value);
    }

    public class SpinnableTestObject : FrameworkElement
    {
      public decimal MyValue
      {
        get { return (decimal)GetValue(MyValueProperty); }
        set { SetValue(MyValueProperty, value); }
      }

      public static readonly DependencyProperty MyValueProperty =
          DependencyProperty.Register("MyValue", typeof(decimal), typeof(SpinnableTestObject));

      public decimal MyMaximum
      {
        get { return (decimal)GetValue(MyMaximumProperty); }
        set { SetValue(MyMaximumProperty, value); }
      }

      public static readonly DependencyProperty MyMaximumProperty =
          DependencyProperty.Register("MyMaximum", typeof(decimal), typeof(SpinnableTestObject));

      public decimal MyMinimum
      {
        get { return (decimal)GetValue(MyMinimumProperty); }
        set { SetValue(MyMinimumProperty, value); }
      }

      public static readonly DependencyProperty MyMinimumProperty =
          DependencyProperty.Register("MyMinimum", typeof(decimal), typeof(SpinnableTestObject));
    }

    [Test]
    [STAThread]
    public void Decorator_PropertyNames()
    {
      SpinnableTestObject obj = new SpinnableTestObject();
      obj.MyMinimum = 123m;
      obj.MyMaximum = 456m;
      obj.MyValue = 200m;

      SpinDecorator decorator = new SpinDecorator();
      decorator.ValueProperty = "MyValue";
      decorator.Content = obj;
      decorator.MinimumProperty = "MyMinimum";
      decorator.MaximumProperty = "MyMaximum";

      decorator.ApplyTemplate();

      Spin spinner = (Spin)FindDescendant(decorator, typeof(Spin), "PART_Spin");

      Assert.AreEqual(123m, spinner.Minimum);
      Assert.AreEqual(456m, spinner.Maximum);
      Assert.AreEqual(200m, spinner.Value);
    }
  }
}
