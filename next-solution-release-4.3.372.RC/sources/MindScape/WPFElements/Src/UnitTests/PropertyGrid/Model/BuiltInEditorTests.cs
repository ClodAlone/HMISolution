//#define INCLUDE_UI_TESTS

using NUnit.Framework;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Automation;
using System;
using Mindscape.WpfElements.WpfPropertyGrid.UnitTests;

namespace Mindscape.WpfElements.PropertyEditing.UnitTests
{
  [TestFixture]
  public class BuiltInEditorTests
  {
    private static readonly Node _booleanProperty = new PropertyNode(new Person(), typeof(Person).GetProperty("Alive"), null);
    private static readonly Node _intProperty = new PropertyNode(new Address(), typeof(Address).GetProperty("StreetNumber"), null);
    private static readonly Node _enumProperty = new PropertyNode(new Person(), typeof(Person).GetProperty("Status"), null);
    private static readonly Node _stringProperty = new PropertyNode(new Person(), typeof(Person).GetProperty("FirstName"), null);
    private static readonly Node _addressProperty = new PropertyNode(new Person(), typeof(Person).GetProperty("Address"), null);
    private static readonly Node _dobProperty = new PropertyNode(new Person(), typeof(Person).GetProperty("DateOfBirth"), null);
    private static readonly Node _genericCollectionProperty = new PropertyNode(new Person(), typeof(Person).GetProperty("Friends"), null);
    private static readonly Node _nonGenericCollectionProperty = new PropertyNode(new Person(), typeof(Person).GetProperty("FavouriteNumbers"), null);
    private static readonly Node _referenceTypeWithFixedSetOfValues = new PropertyNode(new Department(), typeof(Department).GetProperty("Location"), null);
    private static readonly BuiltInEditor _editor = new BuiltInEditor(new BuiltInEditorStyleCollection());

    [Test]
    public void CanEditInPlace()
    {
      Assert.IsTrue(BuiltInEditor.GetEditSettings(_booleanProperty).CanEditInPlace);
      Assert.IsTrue(BuiltInEditor.GetEditSettings(_intProperty).CanEditInPlace);
      Assert.IsTrue(BuiltInEditor.GetEditSettings(_enumProperty).CanEditInPlace);
      Assert.IsTrue(BuiltInEditor.GetEditSettings(_stringProperty).CanEditInPlace);
      Assert.IsFalse(BuiltInEditor.GetEditSettings(_addressProperty).CanEditInPlace);
      Assert.IsTrue(BuiltInEditor.GetEditSettings(_dobProperty).CanEditInPlace);
      Assert.IsTrue(BuiltInEditor.GetEditSettings(_genericCollectionProperty).CanEditInPlace);
      Assert.IsTrue(BuiltInEditor.GetEditSettings(_nonGenericCollectionProperty).CanEditInPlace);
      Assert.IsTrue(BuiltInEditor.GetEditSettings(_referenceTypeWithFixedSetOfValues).CanEditInPlace);

      Assert.IsFalse(BuiltInEditor.GetEditSettings(_booleanProperty).AllowExpand);
      Assert.IsFalse(BuiltInEditor.GetEditSettings(_intProperty).AllowExpand);
      Assert.IsFalse(BuiltInEditor.GetEditSettings(_enumProperty).AllowExpand);
      Assert.IsFalse(BuiltInEditor.GetEditSettings(_stringProperty).AllowExpand);
      Assert.IsTrue(BuiltInEditor.GetEditSettings(_addressProperty).AllowExpand);
      Assert.IsFalse(BuiltInEditor.GetEditSettings(_dobProperty).AllowExpand);
      Assert.IsTrue(BuiltInEditor.GetEditSettings(_genericCollectionProperty).AllowExpand);
      Assert.IsTrue(BuiltInEditor.GetEditSettings(_nonGenericCollectionProperty).AllowExpand);
      Assert.IsFalse (BuiltInEditor.GetEditSettings(_referenceTypeWithFixedSetOfValues).AllowExpand);
    }

    [Test]
    public void BooleanEditor()
    {
      DataTemplate template = _editor.BuildTemplate(_booleanProperty);
      FrameworkElementFactory templateImpl = template.VisualTree;
      Assert.AreEqual(typeof(ContentControl), templateImpl.Type);  // Smoke: define INCLUDE_UI_TESTS to check correct template instantiated
    }

#if INCLUDE_UI_TESTS

    [Test]
    public void BooleanEditor_CheckBox()
    {
      List<string> messages = TemplateInstantiationHelper.RunUITest(GetType(), "BooleanEditor_CheckBox_SetupCallback", delegate(AutomationElement window)
      {
        AutomationElement checkBox = AutomationHelper.FindFirstDescendant(window, AutomationClassName.CheckBox);
        Assert.IsNotNull(checkBox);

        TogglePattern toggler = AutomationHelper.AsTogglePattern(checkBox);
        Assert.IsNotNull(toggler);

        toggler.Toggle();
      });

      Assert.AreEqual("MyProperty=False", messages[messages.Count - 1]);
    }

    public static void BooleanEditor_CheckBox_SetupCallback(PropertyGrid grid)
    {
      PropertyEditor checkBoxEditor = new PropertyEditor();
      checkBoxEditor.DeclaringType = typeof(TestHolder<bool>);
      checkBoxEditor.PropertyName = "MyProperty";
      checkBoxEditor.EditorTemplate = grid.FindResource(PropertyGrid.CheckBoxEditorKey) as DataTemplate;
      grid.Editors.Add(checkBoxEditor);

      grid.SelectedObject = new TestHolder<bool>(true);
    }

    [Test]
    public void IntegerEditor_TextBox()
    {
      List<string> messages = TemplateInstantiationHelper.RunUITest(GetType(), "IntegerEditor_TextBox_SetupCallback", delegate(AutomationElement window)
      {
        AutomationElement textBox = AutomationHelper.FindFirstDescendant(window, AutomationClassName.TextBox);
        Assert.IsNotNull(textBox);

        AutomationHelper.AssertTextBoxValueAndUpdate(textBox, "123", "456");
      });

      Assert.AreEqual("MyProperty=456", messages[messages.Count - 1]);
    }

    public static void IntegerEditor_TextBox_SetupCallback(PropertyGrid grid)
    {
      grid.SelectedObject = new TestHolder<int>(123);
    }

    [Test]
    public void StringEditor_TextBox()
    {
      List<string> messages = TemplateInstantiationHelper.RunUITest(GetType(), "StringEditor_TextBox_SetupCallback", delegate(AutomationElement window)
      {
        AutomationElement textBox = AutomationHelper.FindFirstDescendant(window, AutomationClassName.TextBox);
        Assert.IsNotNull(textBox);

        AutomationHelper.AssertTextBoxValueAndUpdate(textBox, "Alice", "Bob");
      });

      Assert.AreEqual("MyProperty=Bob", messages[messages.Count - 1]);
    }

    public static void StringEditor_TextBox_SetupCallback(PropertyGrid grid)
    {
      grid.SelectedObject = new TestHolder<string>("Alice");
    }

    [Test]
    public void EnumEditor_ComboBox()
    {
      List<string> messages = TemplateInstantiationHelper.RunUITest(GetType(), "EnumEditor_ComboBox_SetupCallback", delegate(AutomationElement window)
      {
        AutomationElement comboBox = AutomationHelper.FindFirstDescendant(window, AutomationClassName.ComboBox);
        Assert.IsNotNull(comboBox);

        Assert.AreEqual("Alien", AutomationHelper.GetValue(comboBox));

        AutomationElementCollection comboBoxItems = AutomationHelper.FindAllChildren(comboBox, AutomationClassName.ListBoxItem, true);
        Assert.AreEqual(4, comboBoxItems.Count);

        ExpandCollapsePattern expander = AutomationHelper.AsExpandCollapsePattern(comboBox);
        expander.Expand();  // the Name property of the combo box items doesn't get populated until they are actually shown

        AutomationElement resident = null;

        foreach (AutomationElement comboBoxItem in comboBoxItems)
        {
          string itemText = comboBoxItem.GetCurrentPropertyValue(AutomationElement.NameProperty) as string;

          CitizenshipStatus enumValue = (CitizenshipStatus)Enum.Parse(typeof(CitizenshipStatus), itemText);

          if (enumValue == CitizenshipStatus.Alien)
          {
            SelectionItemPattern selector = AutomationHelper.AsSelectionItemPattern(comboBoxItem);
            Assert.IsTrue(selector.Current.IsSelected);
          }

          if (enumValue == CitizenshipStatus.Resident)
          {
            resident = comboBoxItem;
          }
        }

        Assert.IsNotNull(resident);
        SelectionItemPattern residentSelector = AutomationHelper.AsSelectionItemPattern(resident);
        residentSelector.Select();
      });

      Assert.AreEqual("MyProperty=Resident", messages[messages.Count - 1]);
    }

    public static void EnumEditor_ComboBox_SetupCallback(PropertyGrid grid)
    {
      grid.SelectedObject = new TestHolder<CitizenshipStatus>(CitizenshipStatus.Alien);
    }

#endif

    [Test]
    public void IntegerEditor()
    {
      DataTemplate template = _editor.BuildTemplate(_intProperty);
      FrameworkElementFactory templateImpl = template.VisualTree;
      Assert.AreEqual(typeof(ContentControl), templateImpl.Type);  // Smoke: define INCLUDE_UI_TESTS to check correct template instantiated
    }

    [Test]
    public void EnumEditor()
    {
      DataTemplate template = _editor.BuildTemplate(_enumProperty);
      FrameworkElementFactory templateImpl = template.VisualTree;
      Assert.AreEqual(typeof(ContentControl), templateImpl.Type);  // Smoke: define INCLUDE_UI_TESTS to check correct template instantiated
    }

    [Test]
    public void StringEditor()
    {
      DataTemplate template = _editor.BuildTemplate(_stringProperty);
      FrameworkElementFactory templateImpl = template.VisualTree;
      Assert.AreEqual(typeof(ContentControl), templateImpl.Type);  // Smoke: define INCLUDE_UI_TESTS to check correct template instantiated
    }

    [Test]
    public void DateEditor()
    {
      DataTemplate template = _editor.BuildTemplate(_dobProperty);
      FrameworkElementFactory templateImpl = template.VisualTree;
      Assert.AreEqual(typeof(ContentControl), templateImpl.Type);  // Smoke: define INCLUDE_UI_TESTS to check correct template instantiated
    }

    [Test]
    public void NonInPlaceTypesGetTextEditor()
    {
      DataTemplate template = _editor.BuildTemplate(_addressProperty);
      FrameworkElementFactory templateImpl = template.VisualTree;
      Assert.AreEqual(typeof(ContentControl), templateImpl.Type);  // Smoke: define INCLUDE_UI_TESTS to check correct template instantiated
    }
  }
}
