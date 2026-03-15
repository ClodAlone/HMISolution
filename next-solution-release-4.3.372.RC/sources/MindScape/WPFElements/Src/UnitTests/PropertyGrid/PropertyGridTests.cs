using System;
using System.Collections.Generic;
using NUnit.Framework;
using System.Windows;
using System.Collections.ObjectModel;
using Mindscape.WpfElements.PropertyEditing;

namespace Mindscape.WpfElements.WpfPropertyGrid.UnitTests
{
  [TestFixture]
  public class PropertyGridTests
  {
    [Test]
    [STAThread]
    public void SimpleProperties()
    {
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      Person person = new Person();

      PropertyGrid grid = new PropertyGrid();

      grid.ItemsSource = dictionary;
      grid.SelectedObject = person;
      grid.AllowModifyCollections = false;
      grid.DefaultMargin = new Thickness(1, 2, 3, 4);

      Assert.AreEqual(dictionary, grid.ItemsSource);
      Assert.AreEqual(person, grid.SelectedObject);
      Assert.IsFalse(grid.AllowModifyCollections);
      Assert.AreEqual(new Thickness(1, 2, 3, 4), grid.DefaultMargin);

      Assert.IsNotNull(grid.EditorSelector);
    }

    [Test]
    [STAThread]
    public void SelectedObject()
    {
      PropertyGrid grid = new PropertyGrid();
      Assert.AreEqual(0, grid.Nodes.Count);
      grid.SelectedObject = Address.AndrewsAddress;
      Assert.AreEqual(5, grid.Nodes.Count);
      Assert.AreEqual(5, grid.BindingView.Count);
    }

    [Test]
    [STAThread]
    public void AddNode_Scalar()
    {
      PropertyGrid grid = new PropertyGrid();
      int nodeCountBeforeAdd = grid.Nodes.Count;

      Node node = grid.AddNode("MyInteger", 127);
      Assert.AreEqual(nodeCountBeforeAdd + 1, grid.Nodes.Count);
      Assert.AreEqual("MyInteger", node.HumanName);
      Assert.AreEqual(127, node.Value);
      Assert.AreEqual(typeof(int), node.PropertyType);
      Assert.IsFalse(node.HasOwnInPlaceEditor);
    }

    [Test]
    [STAThread]
    public void AddNode_ScalarWithEditor()
    {
      NodeEditor editor = new StaticNodeEditor();

      PropertyGrid grid = new PropertyGrid();
      int nodeCountBeforeAdd = grid.Nodes.Count;

      Node node = grid.AddNode("MyString", "Bob", editor);
      Assert.AreEqual(nodeCountBeforeAdd + 1, grid.Nodes.Count);
      Assert.AreEqual("MyString", node.HumanName);
      Assert.AreEqual("Bob", node.Value);
      Assert.AreEqual(typeof(string), node.PropertyType);
      Assert.IsTrue(node.HasOwnInPlaceEditor);
      Assert.AreEqual(editor, node.InPlaceEditor);
    }

    [Test]
    [STAThread]
    public void AddNode_ScalarWithTemplate()
    {
      DataTemplate template = new DataTemplate();

      PropertyGrid grid = new PropertyGrid();

      Node node = grid.AddNode("MyString", "Bob", template);
      Assert.IsTrue(node.HasOwnInPlaceEditor);
      Assert.IsInstanceOf<StaticNodeEditor>(node.InPlaceEditor);
      Assert.AreEqual(template, node.InPlaceEditor.EditorTemplate);
    }

    [Test]
    [STAThread]
    public void AddNode_ScalarWithResourceKey()
    {
      object resourceKey = new object();

      PropertyGrid grid = new PropertyGrid();

      Node node = grid.AddNode("MyString", "Bob", resourceKey);
      Assert.IsTrue(node.HasOwnInPlaceEditor);
      Assert.IsInstanceOf<DynamicNodeEditor>(node.InPlaceEditor);
    }

    [Test]
    [STAThread]
    public void AddedStringNodeDoesNotExpand()
    {
      PropertyGrid grid = new PropertyGrid();
      Node node = grid.AddNode("MyString", "Bob");
      Assert.AreEqual(0, node.Children.Count);
    }

    [Test]
    [STAThread]
    public void AddedRecordNodeDoesExpand()
    {
      PropertyGrid grid = new PropertyGrid();
      Node node = grid.AddNode("Alice", Person.Alice);
      Assert.AreNotEqual(0, node.Children.Count);
    }

    [Test]
    [STAThread]
    [ExpectedException(typeof(ArgumentNullException))]
    public void AddNode_BadCaption_Null()
    {
      (new PropertyGrid()).AddNode(null, 127);
    }

    [Test]
    [STAThread]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void AddNode_BadCaption_Empty()
    {
      (new PropertyGrid()).AddNode(String.Empty, 127);
    }

    [Test]
    [STAThread]
    public void AddPropertyNode()
    {
      PropertyGrid grid = new PropertyGrid();
      int nodeCountBeforeAdd = grid.Nodes.Count;

      Node node = grid.AddPropertyNode("Bob", "Length");
      Assert.AreEqual(nodeCountBeforeAdd + 1, grid.Nodes.Count);

      PropertyNode property = node as PropertyNode;
      Assert.IsNotNull(property);
      Assert.AreEqual("Length", property.Name);
    }

    [Test]
    [STAThread]
    public void AddPropertyNode2()
    {
      PropertyGrid grid = new PropertyGrid();
      Node node = grid.AddPropertyNode("Bob", "Length", "StringLength");
      Assert.AreEqual("StringLength", node.HumanName);
    }

    [Test]
    [STAThread]
    public void RemoveNode()
    {
      PropertyGrid grid = new PropertyGrid();
      int initialNodeCount = grid.Nodes.Count;

      Node n1 = grid.AddNode("N1", 1);
      Node n2 = grid.AddNode("N2", 2);
      Node n3 = grid.AddPropertyNode("N3", "Length");

      Assert.AreEqual(initialNodeCount + 3, grid.Nodes.Count);
      grid.RemoveNode(n1);
      Assert.AreEqual(initialNodeCount + 2, grid.Nodes.Count);
      grid.RemoveNode(n2);
      Assert.AreEqual(initialNodeCount + 1, grid.Nodes.Count);
      grid.RemoveNode(n3);
      Assert.AreEqual(initialNodeCount, grid.Nodes.Count);
    }

    [Test]
    [STAThread]
    public void BindingView()
    {
      PropertyGrid grid = new PropertyGrid();
      PropertyGridBindingView view = new PropertyGridBindingView(grid.Nodes);
      int initialNodeCount = view.Count;

      grid.AddNode("N1", 1);
      Assert.AreEqual(initialNodeCount + 1, view.Count);
    }

    [Test]
    [STAThread]
    public void ItemsSourceOverridesNodes()
    {
      PropertyGrid grid = new PropertyGrid();
      int initialNodeCount = grid.Nodes.Count;

      grid.AddNode("N1", 1);
      grid.AddNode("N2", 2);
      grid.AddPropertyNode("N3", "Length");
      grid.AddNode("N4", 4);

      Assert.AreEqual(initialNodeCount + 4, grid.Nodes.Count);
      Assert.AreEqual(initialNodeCount + 4, grid.BindingView.Count);

      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      dictionary.Add("fie", "mediaeval curse word");
      dictionary.Add("faugh", "Victorian curse word");

      grid.ItemsSource = dictionary;

      Assert.AreEqual(initialNodeCount + 4, grid.Nodes.Count);
      Assert.AreEqual(dictionary.Keys.Count, grid.BindingView.Count);

      grid.ItemsSource = null;

      Assert.AreEqual(initialNodeCount + 4, grid.BindingView.Count);
    }

    [Test]
    [STAThread]
    public void EditorKeysDefined()
    {
      AssertDataTemplateDefinedInTheme(PropertyGrid.CheckBoxEditorKey);
      AssertDataTemplateDefinedInTheme(PropertyGrid.CollectionDisplayKey);
      AssertDataTemplateDefinedInTheme(PropertyGrid.CollectionElementEditorKey);
      AssertDataTemplateDefinedInTheme(PropertyGrid.ColorEditorKey);
      AssertDataTemplateDefinedInTheme(PropertyGrid.DateEditorKey);
      AssertDataTemplateDefinedInTheme(PropertyGrid.ListSelectEditorKey);
      AssertDataTemplateDefinedInTheme(PropertyGrid.ListSelectNoTextEntryEditorKey);
      AssertDataTemplateDefinedInTheme(PropertyGrid.NumericUpDownEditorKey);
      AssertDataTemplateDefinedInTheme(PropertyGrid.RadioSelectEditorKey);
      AssertDataTemplateDefinedInTheme(PropertyGrid.ReadOnlyDisplayKey);
      AssertDataTemplateDefinedInTheme(PropertyGrid.SimpleTextEditorKey);
      AssertDataTemplateDefinedInTheme(PropertyGrid.SliderEditorKey);
    }

    private void AssertDataTemplateDefinedInTheme(object key)
    {
      Assert.IsNotNull(key);
      object resource = new PropertyGrid().TryFindResource(key);
      Assert.IsNotNull(resource);
      Assert.IsInstanceOf<DataTemplate>(resource);
    }

    [Test]
    [STAThread]
    public void SettingSelectedObjectSetsSelectedObjectsToOneElementArray()
    {
      PropertyGrid grid = new PropertyGrid();
      grid.SelectedObject = Person.Alice;
      Assert.AreEqual(1, grid.SelectedObjects.Count);
      Assert.AreEqual(Person.Alice, grid.SelectedObjects[0]);
      Assert.AreEqual(Person.Alice, grid.SelectedObject);
    }

    [Test]
    [STAThread]
    public void SelectedObjectsPropertyRoundtrips()
    {
      object[] objects = new object[] { Person.Alice, Person.Bob };

      PropertyGrid grid = new PropertyGrid();
      grid.SelectedObjects = objects;
      Assert.AreSame(objects, grid.SelectedObjects);
    }

    [Test]
    [STAThread]
    public void SettingSelectedObjectsSetsSelectedObjectToMultipleObjectWrapper()
    {
      object[] objects = new object[] { Person.Alice, Person.Bob };

      PropertyGrid grid = new PropertyGrid();
      grid.SelectedObjects = objects;
      Assert.IsInstanceOf<MultipleObjectWrapper>(grid.SelectedObject);

      MultipleObjectWrapper mow = (MultipleObjectWrapper)(grid.SelectedObject);
      List<object> wrapped = new List<object>();
      foreach (object obj in mow.Objects)
      {
        wrapped.Add(obj);
      }

      Assert.AreEqual(objects.Length, wrapped.Count);
      Assert.AreEqual(Person.Alice, wrapped[0]);
      Assert.AreEqual(Person.Bob, wrapped[1]);
    }

    [Test]
    [STAThread]
    public void HomogeneousSelectedObjectsGiveCorrectNodeCount()
    {
      PropertyGrid grid = new PropertyGrid();
      grid.SelectedObjects = new Animal[] { Animal.Fang, Animal.Kiki };
      Assert.AreEqual(3, grid.Nodes.Count);
    }

    [Test]
    [STAThread]
    public void HeterogeneousSelectedObjectsGiveCorrectNodeCount()
    {
      PropertyGrid grid = new PropertyGrid();
      grid.SelectedObjects = new object[] { Animal.Fang, Plant.ZogTheDevourer };
      Assert.AreEqual(1, grid.Nodes.Count);
    }

    [Test]
    [STAThread]
    public void NodeCountTracksAddingSelectedObjects()
    {
      PropertyGrid grid = new PropertyGrid();
      ObservableCollection<object> selection = new ObservableCollection<object>();
      selection.Add(Animal.Fang);
      grid.SelectedObject = new MultipleObjectWrapper(selection);
      Assert.AreEqual(3, grid.Nodes.Count);

      selection.Add(Animal.Kiki);
      Assert.AreEqual(3, grid.Nodes.Count);

      selection.Add(Alien.Anastasia);
      Assert.AreEqual(2, grid.Nodes.Count);

      selection.Add(Plant.ZogTheDevourer);
      Assert.AreEqual(1, grid.Nodes.Count);
    }

    [Test]
    [STAThread]
    public void NodeCountTracksRemovingSelectedObjects()
    {
      PropertyGrid grid = new PropertyGrid();
      ObservableCollection<object> selection = new ObservableCollection<object>();
      selection.Add(Animal.Fang);
      selection.Add(Animal.Kiki);
      selection.Add(Alien.Anastasia);
      selection.Add(Plant.ZogTheDevourer);
      grid.SelectedObject = new MultipleObjectWrapper(selection);
      Assert.AreEqual(1, grid.Nodes.Count);

      selection.Remove(Plant.ZogTheDevourer);
      Assert.AreEqual(2, grid.Nodes.Count);

      selection.Remove(Alien.Anastasia);
      Assert.AreEqual(3, grid.Nodes.Count);

      selection.Remove(Animal.Kiki);
      Assert.AreEqual(3, grid.Nodes.Count);
    }

    [Test]
    [STAThread]
    public void SelectedObjects_NodeCountTracksAddingSelectedObjects()
    {
      PropertyGrid grid = new PropertyGrid();
      ObservableCollection<object> selection = new ObservableCollection<object>();
      selection.Add(Animal.Fang);
      grid.SelectedObjects = selection;
      Assert.AreEqual(3, grid.Nodes.Count);

      selection.Add(Animal.Kiki);
      Assert.AreEqual(3, grid.Nodes.Count);

      grid.SelectedObjects.Add(Alien.Anastasia);
      Assert.AreEqual(2, grid.Nodes.Count);

      selection.Add(Plant.ZogTheDevourer);
      Assert.AreEqual(1, grid.Nodes.Count);
    }

    [Test]
    [STAThread]
    public void SelectedObjects_NodeCountTracksRemovingSelectedObjects()
    {
      PropertyGrid grid = new PropertyGrid();
      ObservableCollection<object> selection = new ObservableCollection<object>();
      selection.Add(Animal.Fang);
      selection.Add(Animal.Kiki);
      selection.Add(Alien.Anastasia);
      selection.Add(Plant.ZogTheDevourer);
      grid.SelectedObjects = selection;
      Assert.AreEqual(1, grid.Nodes.Count);

      selection.Remove(Plant.ZogTheDevourer);
      Assert.AreEqual(2, grid.Nodes.Count);

      grid.SelectedObjects.Remove(Alien.Anastasia);
      Assert.AreEqual(3, grid.Nodes.Count);

      selection.Remove(Animal.Kiki);
      Assert.AreEqual(3, grid.Nodes.Count);
    }

    [Test]
    [STAThread]
    public void RemovingAllSelectedObjectsGivesEmptyGrid()
    {
      PropertyGrid grid = new PropertyGrid();
      ObservableCollection<object> selection = new ObservableCollection<object>();
      selection.Add(Animal.Fang);
      selection.Add(Animal.Kiki);

      grid.SelectedObject = new MultipleObjectWrapper(selection);
      Assert.AreEqual(3, grid.Nodes.Count);

      selection.Remove(Animal.Kiki);
      selection.Remove(Animal.Fang);
      Assert.AreEqual(0, grid.Nodes.Count);

      selection.Add(Plant.ZogTheDevourer);
      Assert.AreEqual(4, grid.Nodes.Count);
    }

    [Test]
    [STAThread]
    public void SettingSelectedObjectsToNullSetsSelectedObjectToNull()
    {
      PropertyGrid grid = new PropertyGrid();
      grid.SelectedObject = Alien.Bill;
      grid.SelectedObjects = null;

      Assert.AreEqual(null, grid.SelectedObject);
      Assert.AreEqual(0, grid.Nodes.Count);
    }

    [Test]
    [STAThread]
    public void SettingSelectedObjectToNullSetsSelectedObjectsToEmptyArray()
    {
      PropertyGrid grid = new PropertyGrid();
      grid.SelectedObjects = new object[] { Alien.Bill };
      grid.SelectedObject = null;

      Assert.AreEqual(0, grid.SelectedObjects.Count);
      Assert.AreEqual(0, grid.Nodes.Count);
    }

    [Test]
    [STAThread]
    public void ModifyingOldSelectionDoesNotAffectGrid()
    {
      PropertyGrid grid = new PropertyGrid();
      ObservableCollection<object> oldSelection = new ObservableCollection<object>();
      oldSelection.Add(Animal.Fang);
      oldSelection.Add(Animal.Kiki);
      grid.SelectedObject = new MultipleObjectWrapper(oldSelection);

      ObservableCollection<object> newSelection = new ObservableCollection<object>();
      newSelection.Add(Animal.Boris);
      newSelection.Add(Animal.Butch);
      grid.SelectedObject = new MultipleObjectWrapper(newSelection);

      Assert.AreEqual(3, grid.Nodes.Count);
      oldSelection.Add(Alien.Anastasia);
      Assert.AreEqual(3, grid.Nodes.Count);
    }

    [Test]
    [STAThread]
    public void SelectedObjectsCanStartOutEmpty()
    {
      PropertyGrid grid = new PropertyGrid();
      grid.SelectedObjects = new ObservableCollection<object>();

      Assert.AreEqual(0, grid.Nodes.Count);

      grid.SelectedObjects.Add(Animal.Fang);
      Assert.AreEqual(3, grid.Nodes.Count);
    }
  }
}
