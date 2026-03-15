using NUnit.Framework;
using System.Collections.ObjectModel;
using System.Reflection;
using System.ComponentModel;
using Mindscape.WpfElements.PropertyEditing;

namespace Mindscape.WpfElements.WpfPropertyGrid.UnitTests
{
  [TestFixture]
  public class PropertyGridBindingViewTests
  {
    private static ObservableCollection<Node> CreateSamplePropertyCollection()
    {
      PropertyInfo info = typeof(string).GetProperty("Length");

      ObservableCollection<Node> properties = new ObservableCollection<Node>();
      properties.Add(new PropertyNode("Fred", info, null));
      properties.Add(new PropertyNode("Bob", info, null));

      Node nonLeaf = new PropertyNode("Dad", info, null);
      nonLeaf.Children.Add(new PropertyNode("Kid A", info, null));
      nonLeaf.Children.Add(new PropertyNode("Kid B", info, null));
      properties.Add(nonLeaf);

      properties.Add(new PropertyNode("Amelia", info, null));

      return properties;
    }

    [Test]
    public void NewView()
    {
      ObservableCollection<Node> sample = CreateSamplePropertyCollection();

      PropertyGridBindingView view = new PropertyGridBindingView(sample);
      Assert.AreEqual(sample.Count, view.Count);
      Assert.IsTrue(view[0].IsLeaf);
      Assert.AreEqual(sample[0], view[0].Node);
      Assert.IsFalse(view[2].IsLeaf);
      Assert.AreEqual(sample[2], view[2].Node);
    }

    [Test]
    public void DefaultView()
    {
      ObservableCollection<Node> sample = CreateSamplePropertyCollection();
      PropertyGridBindingView view = new PropertyGridBindingView(sample);
      ICollectionView defaultView = view.DefaultView;
      Assert.AreEqual(view, defaultView.SourceCollection);
    }

    [Test]
    public void Add()
    {
      ObservableCollection<Node> sample = CreateSamplePropertyCollection();

      PropertyGridBindingView view = new PropertyGridBindingView(sample);
      int viewCountBeforeAdd = view.Count;

      sample.Add(new PropertyNode("Mystery Guest", typeof(string).GetProperty("Length"), null));

      Assert.AreEqual(viewCountBeforeAdd + 1, view.Count);
    }

    [Test]
    public void Remove()
    {
      ObservableCollection<Node> sample = CreateSamplePropertyCollection();

      PropertyGridBindingView view = new PropertyGridBindingView(sample);
      int viewCountBeforeRemove = view.Count;

      sample.Remove(sample[1]);

      Assert.AreEqual(viewCountBeforeRemove - 1, view.Count);
    }

    [Test]
    public void Clear()
    {
      ObservableCollection<Node> sample = CreateSamplePropertyCollection();

      PropertyGridBindingView view = new PropertyGridBindingView(sample);
      sample.Clear();
      Assert.AreEqual(0, view.Count);
    }
  }
}