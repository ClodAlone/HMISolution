using NUnit.Framework;
using System.Windows;
using System.Windows.Controls;
using System;

namespace Mindscape.WpfElements.UnitTests
{
  [TestFixture]
  public class ControlTests
  {
    [Test]
    [STAThread]
    public void LoadControls()
    {
      string xaml =
        @"<Window 
        xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
        xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'
        xmlns:ms='http://namespaces.mindscape.co.nz/wpf'
        Title='WPF Elements Dummy XAML'>

      <StackPanel>

        <ms:CurrencyTextBox />
        <ms:DateTimePicker />
        <ms:DropDownDatePicker />
        <ms:DropDownEditBox />
        <ms:IntegerTextBox />
        <ms:MaskedTextBox />
        <ms:MonthCalendar />
        <ms:MulticolumnTreeView />
        <ms:NumericTextBox />
        <ms:ProportionalStackPanel />
        <ms:SpinDecorator />
        <ms:Spin />

      </StackPanel>
      
      </Window>";

      Window window = MarkupUtils.LoadXaml<Window>(xaml);

      Assert.IsNotNull(window);

      StackPanel panel = (StackPanel)(window.Content);
      Assert.AreEqual(12, panel.Children.Count);
    }
  }
}
