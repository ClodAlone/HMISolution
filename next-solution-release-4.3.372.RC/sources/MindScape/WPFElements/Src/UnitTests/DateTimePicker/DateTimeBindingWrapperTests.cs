using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.Globalization;
using System.ComponentModel;

namespace Mindscape.WpfElements.UnitTests
{
  [TestFixture]
  public class DateTimeBindingWrapperTests
  {
    //[Test]
    //public void Binding()
    //{
    //  DateTimeValue binding = new DateTimeValue();
    //  binding.Value = new DateTime(1900, 1, 2, 3, 4, 5);

    //  DateTimeBindingWrapper wrapper = new DateTimeBindingWrapper(binding);
    //  List<string> changedProperties = new List<string>();

    //  wrapper.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
    //  {
    //    changedProperties.Add(e.PropertyName);
    //  };

    //  Assert.AreEqual(binding.Value.Year, wrapper.Year);
    //  Assert.AreEqual(binding.Value.Month, wrapper.Month);
    //  Assert.AreEqual(binding.Value.Day, wrapper.Day);
    //  Assert.AreEqual(binding.Value.Hour, wrapper.Hour);
    //  Assert.AreEqual(binding.Value.Minute, wrapper.Minute);
    //  Assert.AreEqual(binding.Value.Second, wrapper.Second);

    //  wrapper.Year = 2000;

    //  Assert.AreEqual(2000, binding.Value.Year);
    //  Assert.AreEqual(1, binding.Value.Month);
    //  Assert.IsTrue(changedProperties.Contains("Value"));
    //  Assert.IsTrue(changedProperties.Contains("Year"));
    //  Assert.IsTrue(changedProperties.Contains("DayOfWeek"));
    //  changedProperties.Clear();

    //  wrapper.Month = 12;

    //  Assert.AreEqual(2000, binding.Value.Year);
    //  Assert.AreEqual(12, binding.Value.Month);
    //  Assert.IsTrue(changedProperties.Contains("Value"));
    //  Assert.IsTrue(changedProperties.Contains("Month"));
    //  Assert.IsTrue(changedProperties.Contains("DayOfWeek"));
    //  changedProperties.Clear();

    //  wrapper.Day = 25;
    //  Assert.AreEqual(25, binding.Value.Day);
    //  Assert.IsTrue(changedProperties.Contains("Value"));
    //  Assert.IsTrue(changedProperties.Contains("Day"));
    //  Assert.IsTrue(changedProperties.Contains("DayOfWeek"));
    //  changedProperties.Clear();

    //  wrapper.Hour = 13;
    //  Assert.AreEqual(13, binding.Value.Hour);
    //  Assert.IsTrue(changedProperties.Contains("Value"));
    //  Assert.IsTrue(changedProperties.Contains("Hour"));
    //  changedProperties.Clear();

    //  wrapper.Minute = 14;
    //  Assert.AreEqual(14, binding.Value.Minute);
    //  Assert.IsTrue(changedProperties.Contains("Value"));
    //  Assert.IsTrue(changedProperties.Contains("Minute"));
    //  changedProperties.Clear();

    //  wrapper.Second = 15;
    //  Assert.AreEqual(15, binding.Value.Second);
    //  Assert.IsTrue(changedProperties.Contains("Value"));
    //  Assert.IsTrue(changedProperties.Contains("Second"));
    //  changedProperties.Clear();
    //}

    //[Test]
    //public void DerivedProperties()
    //{
    //  DateTimeValue binding = new DateTimeValue();
    //  binding.Value = new DateTime(2008, 3, 11, 3, 4, 5);

    //  DateTimeBindingWrapper wrapper = new DateTimeBindingWrapper(binding);
    //  wrapper.Culture = new CultureInfo("fr-FR", false);

    //  List<string> changedProperties = new List<string>();

    //  wrapper.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
    //  {
    //    changedProperties.Add(e.PropertyName);
    //  };

    //  Assert.AreEqual("mardi", wrapper.DayOfWeek);
    //  wrapper.Day = 12;
    //  Assert.AreEqual("mercredi", wrapper.DayOfWeek);
    //  Assert.IsTrue(changedProperties.Contains("DayOfWeek"));

    //  changedProperties.Clear();

    //  binding.Value = new DateTime(2008, 3, 13, 3, 4, 5);
    //  Assert.AreEqual("jeudi", wrapper.DayOfWeek);
    //  Assert.IsTrue(changedProperties.Contains("DayOfWeek"));
    //}

    //[Test]
    //public void BindingChangesDetected()
    //{
    //  DateTimeValue binding = new DateTimeValue();
    //  binding.Value = new DateTime(1900, 1, 2, 3, 4, 5);

    //  DateTimeBindingWrapper wrapper = new DateTimeBindingWrapper(binding);

    //  Assert.AreEqual(binding.Value.Year, wrapper.Year);
    //  Assert.AreEqual(binding.Value.Month, wrapper.Month);
    //  Assert.AreEqual(binding.Value.Day, wrapper.Day);
    //  Assert.AreEqual(binding.Value.Hour, wrapper.Hour);
    //  Assert.AreEqual(binding.Value.Minute, wrapper.Minute);
    //  Assert.AreEqual(binding.Value.Second, wrapper.Second);

    //  binding.Value = new DateTime(2000, 11, 12, 13, 14, 15);

    //  Assert.AreEqual(binding.Value.Year, wrapper.Year);
    //  Assert.AreEqual(binding.Value.Month, wrapper.Month);
    //  Assert.AreEqual(binding.Value.Day, wrapper.Day);
    //  Assert.AreEqual(binding.Value.Hour, wrapper.Hour);
    //  Assert.AreEqual(binding.Value.Minute, wrapper.Minute);
    //  Assert.AreEqual(binding.Value.Second, wrapper.Second);
    //}

    //[Test]
    //public void BadPartialSetCausesArgumentExceptionButDoesNotChangeData()
    //{
    //  bool gotException = false;

    //  DateTimeValue binding = new DateTimeValue();
    //  binding.Value = new DateTime(1900, 1, 2, 3, 4, 5);

    //  DateTimeBindingWrapper wrapper = new DateTimeBindingWrapper(binding);

    //  try
    //  {
    //    wrapper.Month = 92385;
    //  }
    //  catch (ArgumentException)
    //  {
    //    gotException = true;
    //  }

    //  Assert.IsTrue(gotException);

    //  Assert.AreEqual(1, binding.Value.Month);
    //  Assert.AreEqual(1, wrapper.Month);
    //}
  }

  //internal class DateTimeValue : IImmutableStructValue<DateTime>
  //{
  //  private DateTime _value;

  //  public DateTime Value
  //  {
  //    get { return _value; }
  //    set
  //    {
  //      _value = value;
  //      OnPropertyChanged("Value");
  //    }
  //  }

  //  public event PropertyChangedEventHandler PropertyChanged;

  //  protected virtual void OnPropertyChanged(string propertyName)
  //  {
  //    if (PropertyChanged != null)
  //    {
  //      PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
  //    }
  //  }
  //}
}
