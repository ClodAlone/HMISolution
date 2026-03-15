//#define INCLUDE_UI_TESTS

using NUnit.Framework;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Automation;
using System.Collections.Generic;
using System;
using Mindscape.WpfElements.WpfPropertyGrid.UnitTests;

namespace Mindscape.WpfElements.PropertyEditing.UnitTests
{
  [TestFixture]
  public class TypeEditorTests
  {
    [Test]
    public void SimpleProperties()
    {
      DataTemplate template = new DataTemplate();

      TypeEditor editor = new TypeEditor();
      editor.EditedType = typeof(string);
      editor.EditorTemplate = template;

      Assert.AreEqual(typeof(string), editor.EditedType);
      Assert.AreEqual(template, editor.EditorTemplate);
    }

    private static readonly Node _stringProperty = new PropertyNode(new Person(), typeof(Person).GetProperty("FirstName"), null);
    private static readonly Node _boolProperty = new PropertyNode(new Person(), typeof(Person).GetProperty("Alive"), null);
    private static readonly Node _addressProperty = new PropertyNode(new Person(), typeof(Person).GetProperty("Address"), null);

    [Test]
    public void CanEdit()
    {
      TypeEditor editor = new TypeEditor();
      editor.EditedType = typeof(string);

      Assert.IsTrue(editor.CanEdit(_stringProperty));
      Assert.IsFalse(editor.CanEdit(_boolProperty));
      Assert.IsFalse(editor.CanEdit(_addressProperty));
    }

    [Test]
    public void CreateEditor()
    {
      DataTemplate template = new DataTemplate();

      TypeEditor editor = new TypeEditor();
      editor.EditedType = typeof(string);
      editor.EditorTemplate = template;

      // ENHANCEMENT: Port this test to the UI test framework and check that
      // the correct DataTemplate is being applied with the correct binding.
      FrameworkElementFactory generatedTemplate = editor.BuildTemplate(_stringProperty).VisualTree;
      Assert.AreEqual(typeof(ContentControl), generatedTemplate.Type);
    }

#if INCLUDE_UI_TESTS

    [Test]
    public void PhoneNumberEditor()
    {
      List<string> messages = TemplateInstantiationHelper.RunUITest(GetType(), "PhoneNumberEditor_SetupCallback", delegate(AutomationElement window)
      {
        AutomationElement countryCodeBox = AutomationHelper.FindFirstDescendant(window, AutomationClassName.TextBox);
        AutomationHelper.AssertTextBoxValueAndUpdate(countryCodeBox, "64", "NewCC");

        AutomationElement regionCodeBox = AutomationHelper.GetNextSibling(countryCodeBox, AutomationClassName.TextBox);
        AutomationHelper.AssertTextBoxValueAndUpdate(regionCodeBox, "(0)4", "NewRC");

        AutomationElement numberBox = AutomationHelper.GetNextSibling(regionCodeBox, AutomationClassName.TextBox);
        AutomationHelper.AssertTextBoxValueAndUpdate(numberBox, "914 1384", "NewNumber");
      });

      Assert.AreEqual("CountryCode=NewCC", messages[0]);
      Assert.AreEqual("RegionCode=NewRC", messages[1]);
      Assert.AreEqual("Number=NewNumber", messages[2]);
    }

    private const string PhoneNumberEditorXaml = @"<DataTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>
      <StackPanel Orientation='Horizontal'>
        <TextBlock>+</TextBlock>
        <TextBox Text='{Binding Path=CountryCode, UpdateSourceTrigger=PropertyChanged}' BorderThickness='0' />
        <TextBlock> (</TextBlock>
        <TextBox Text='{Binding Path=RegionCode, UpdateSourceTrigger=PropertyChanged}' BorderThickness='0' />
        <TextBlock>) </TextBlock>
        <TextBox Text='{Binding Path=Number, UpdateSourceTrigger=PropertyChanged}' BorderThickness='0' />
      </StackPanel>
    </DataTemplate>";

    public static void PhoneNumberEditor_SetupCallback(PropertyGrid grid)
    {
      TypeEditor phoneNumberEditor = new TypeEditor();
      phoneNumberEditor.EditedType = typeof(PhoneNumber);
      phoneNumberEditor.EditorTemplate = TemplateInstantiationHelper.LoadFromXaml(PhoneNumberEditorXaml);
      grid.Editors.Add(phoneNumberEditor);

      PhoneNumber number = new PhoneNumber();
      number.CountryCode = "64";
      number.RegionCode = "(0)4";
      number.Number = "914 1384";

      grid.SelectedObject = new TestHolder<PhoneNumber>(number);
    }

#endif
  }
}
