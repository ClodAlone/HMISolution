using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.Windows.Controls;
using System.Windows;

namespace Mindscape.WpfElements.UnitTests
{
  [TestFixture]
  public class ProportionalStackPanelTests
  {
    [Test]
    [Ignore]
    public void Proportion_Vertical()
    {
      TextBlock block1 = new TextBlock();
      ProportionalStackPanel.SetProportion(block1, 10);

      TextBlock block2 = new TextBlock();
      ProportionalStackPanel.SetProportion(block2, 20);

      TextBlock block3 = new TextBlock();
      ProportionalStackPanel.SetProportion(block3, 30);

      ProportionalStackPanel panel = new ProportionalStackPanel();
      panel.Children.Add(block1);
      panel.Children.Add(block2);
      panel.Children.Add(block3);

      panel.Measure(new Size(90, 90));
      panel.Arrange(new Rect(0, 0, 90, 90));

      Assert.AreEqual(15, block1.ActualHeight);
      Assert.AreEqual(30, block2.ActualHeight);
      Assert.AreEqual(45, block3.ActualHeight);

      Assert.AreEqual(90, block1.ActualWidth);
      Assert.AreEqual(90, block2.ActualWidth);
      Assert.AreEqual(90, block3.ActualWidth);
    }

    [Test]
    [STAThread]
    public void Proportion_Horizontal()
    {
      TextBlock block1 = new TextBlock();
      ProportionalStackPanel.SetProportion(block1, 10);

      TextBlock block2 = new TextBlock();
      ProportionalStackPanel.SetProportion(block2, 20);

      TextBlock block3 = new TextBlock();
      ProportionalStackPanel.SetProportion(block3, 30);

      ProportionalStackPanel panel = new ProportionalStackPanel();
      panel.Orientation = Orientation.Horizontal;
      panel.Children.Add(block1);
      panel.Children.Add(block2);
      panel.Children.Add(block3);

      panel.Measure(new Size(90, 90));
      panel.Arrange(new Rect(0, 0, 90, 90));

      Assert.AreEqual(90, block1.ActualHeight);
      Assert.AreEqual(90, block2.ActualHeight);
      Assert.AreEqual(90, block3.ActualHeight);

      Assert.AreEqual(15, block1.ActualWidth);
      Assert.AreEqual(30, block2.ActualWidth);
      Assert.AreEqual(45, block3.ActualWidth);
    }
  }
}
