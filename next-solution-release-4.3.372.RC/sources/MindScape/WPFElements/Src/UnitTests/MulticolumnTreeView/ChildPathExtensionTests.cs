using NUnit.Framework;
using System.Windows.Data;
using System.Windows;
using System;

namespace Mindscape.WpfElements.UnitTests
{
  [TestFixture]
  public class ChildPathExtensionTests
  {
    [Test]
    public void UsingPath()
    {
      ChildPathExtension extension = new ChildPathExtension("MyChildren");

      object result = extension.ProvideValue(null);
      Assert.IsInstanceOf<HierarchicalDataTemplate>(result);

      Binding hdtItemsSource = (Binding)(((HierarchicalDataTemplate)result).ItemsSource);
      Assert.AreEqual("MyChildren", hdtItemsSource.Path.Path);
    }

    [Test]
    public void UsingBinding()
    {
      Binding binding = new Binding("YourChildren");

      ChildPathExtension extension = new ChildPathExtension();

      extension.Binding = binding;

      object result = extension.ProvideValue(null);
      Assert.IsInstanceOf<HierarchicalDataTemplate>(result);

      Binding hdtItemsSource = (Binding)(((HierarchicalDataTemplate)result).ItemsSource);
      Assert.AreEqual(binding, hdtItemsSource);
      Assert.AreEqual("YourChildren", hdtItemsSource.Path.Path);
    }

    [Test]
    public void BindingOverridesConstructorPath()
    {
      Binding binding = new Binding("YourChildren");

      ChildPathExtension extension = new ChildPathExtension("MyChildren");

      extension.Binding = binding;

      object result = extension.ProvideValue(null);
      Assert.IsInstanceOf<HierarchicalDataTemplate>(result);

      Binding hdtItemsSource = (Binding)(((HierarchicalDataTemplate)result).ItemsSource);
      Assert.AreEqual(binding, hdtItemsSource);
      Assert.AreEqual("YourChildren", hdtItemsSource.Path.Path);
    }

    [Test]
    [STAThread]
    public void Xaml_UsingPath()
    {
      string xaml =
        @"<ms:MulticolumnTreeView 
        xmlns:ms='http://namespaces.mindscape.co.nz/wpf'
        ItemTemplate='{ms:ChildPath MyChildren}' />";

      Binding itemsSource = GetMctvItemsSource(xaml);

      Assert.AreEqual("MyChildren", itemsSource.Path.Path);
    }

    [Test]
    [STAThread]
    public void Xaml_UsingBinding()
    {
      string xaml =
        @"<ms:MulticolumnTreeView 
        xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
        xmlns:ms='http://namespaces.mindscape.co.nz/wpf'
        ItemTemplate='{ms:ChildPath Binding={Binding YourChildren}}' />";

      Binding itemsSource = GetMctvItemsSource(xaml);

      Assert.AreEqual("YourChildren", itemsSource.Path.Path);
    }

    private static Binding GetMctvItemsSource(string xaml)
    {
      MulticolumnTreeView mctv = MarkupUtils.LoadXaml<MulticolumnTreeView>(xaml);
      HierarchicalDataTemplate hdt = (HierarchicalDataTemplate)(mctv.ItemTemplate);
      Binding hdtItemsSource = (Binding)(hdt.ItemsSource);
      return hdtItemsSource;
    }
  }
}
