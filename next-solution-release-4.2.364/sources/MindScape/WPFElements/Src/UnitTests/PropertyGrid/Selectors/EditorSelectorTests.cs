using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

using NUnit.Framework;
using Mindscape.WpfElements.WpfPropertyGrid.UnitTests;
using Mindscape.WpfElements.WpfPropertyGrid;

namespace Mindscape.WpfElements.PropertyEditing.UnitTests
{
  [TestFixture]
  public class EditorSelectorTests
  {
    [Test]
    public void SafeToIterateExtendingEditors_Null()
    {
      EditorSelector selector = new EditorSelector(null);
      int iteratedEditorCount = (new List<Editor>(selector.ExtendingEditors)).Count;
      Assert.AreEqual(0, iteratedEditorCount);
    }

    [Test]
    public void SafeToIterateExtendingEditors_ExtendedWithNullCollection()
    {
      ExtendsWithNullCollection naughty = new ExtendsWithNullCollection();
      EditorSelector selector = new EditorSelector(naughty);
      int iteratedEditorCount = (new List<Editor>(selector.ExtendingEditors)).Count;
      Assert.AreEqual(0, iteratedEditorCount);
    }

    private static EditorSelector CreateExtendedSelector()
    {
      ExtendsWithTypeAndPropertyEditors extender = new ExtendsWithTypeAndPropertyEditors();
      return new EditorSelector(extender);
    }

    private static readonly Node _tallProperty = 
      new PropertyNode(new Person(), typeof(Person).GetProperty("Tall"), null);
    private static readonly Node _aliveProperty = 
      new PropertyNode(new Person(), typeof(Person).GetProperty("Alive"), null);
    private static readonly Node _phoneNumberProperty = 
      new PropertyNode(new Address(), typeof(Address).GetProperty("PhoneNumber"), null);
    private static readonly Node _partnerProperty = 
      new PropertyNode(new Person(), typeof(Person).GetProperty("Partner"), null);
    private static readonly Node _puppyProperty =
      new PropertyNode(new Person(), typeof(Person).GetProperty("Puppy"), null);

    [Test]
    public void CanEditInPlace()
    {
      EditorSelector selectorBasic = new EditorSelector(null);

      Assert.IsTrue(selectorBasic.GetEditSettings(_tallProperty).CanEditInPlace);
      Assert.IsTrue(selectorBasic.GetEditSettings(_aliveProperty).CanEditInPlace);
      Assert.IsFalse(selectorBasic.GetEditSettings(_phoneNumberProperty).CanEditInPlace);
      Assert.IsFalse(selectorBasic.GetEditSettings(_partnerProperty).CanEditInPlace);
      Assert.IsFalse(selectorBasic.GetEditSettings(_puppyProperty).CanEditInPlace);

      Assert.IsFalse(selectorBasic.GetEditSettings(_tallProperty).AllowExpand);
      Assert.IsFalse(selectorBasic.GetEditSettings(_aliveProperty).AllowExpand);
      Assert.IsTrue(selectorBasic.GetEditSettings(_phoneNumberProperty).AllowExpand);
      Assert.IsTrue(selectorBasic.GetEditSettings(_partnerProperty).AllowExpand);
      Assert.IsTrue(selectorBasic.GetEditSettings(_puppyProperty).AllowExpand);

      EditorSelector selectorExtended = CreateExtendedSelector();

      Assert.IsTrue(selectorExtended.GetEditSettings(_tallProperty).CanEditInPlace);
      Assert.IsTrue(selectorExtended.GetEditSettings(_aliveProperty).CanEditInPlace);
      Assert.IsTrue(selectorExtended.GetEditSettings(_phoneNumberProperty).CanEditInPlace);
      Assert.IsFalse(selectorExtended.GetEditSettings(_partnerProperty).CanEditInPlace);
      Assert.IsTrue(selectorExtended.GetEditSettings(_puppyProperty).CanEditInPlace);

      Assert.IsFalse(selectorExtended.GetEditSettings(_tallProperty).AllowExpand);
      Assert.IsFalse(selectorExtended.GetEditSettings(_aliveProperty).AllowExpand);
      Assert.IsFalse(selectorExtended.GetEditSettings(_phoneNumberProperty).AllowExpand);
      Assert.IsTrue(selectorExtended.GetEditSettings(_partnerProperty).AllowExpand);
      Assert.IsTrue(selectorExtended.GetEditSettings(_puppyProperty).AllowExpand);
    }

    [Test]
    public void GetEditor()
    {
      EditorSelector selectorBasic = new EditorSelector(null);
      Assert.IsInstanceOf<BuiltInEditor>(selectorBasic.GetEditor(_tallProperty));
      Assert.IsInstanceOf<BuiltInEditor>(selectorBasic.GetEditor(_aliveProperty));
      Assert.IsInstanceOf<BuiltInEditor>(selectorBasic.GetEditor(_phoneNumberProperty));
      Assert.IsInstanceOf<BuiltInEditor>(selectorBasic.GetEditor(_partnerProperty));

      EditorSelector selectorExtended = CreateExtendedSelector();
      Assert.IsInstanceOf<PropertyEditor>(selectorExtended.GetEditor(_tallProperty));
      Assert.IsInstanceOf<BuiltInEditor>(selectorExtended.GetEditor(_aliveProperty));
      Assert.IsInstanceOf<TypeEditor>(selectorExtended.GetEditor(_phoneNumberProperty));
      Assert.IsInstanceOf<BuiltInEditor>(selectorExtended.GetEditor(_partnerProperty));

      TypeEditor phoneNumberEditor = (TypeEditor)(selectorExtended.GetEditor(_phoneNumberProperty));
      Assert.IsTrue(phoneNumberEditor.CanEdit(_phoneNumberProperty));
      Assert.AreEqual(ExtendsWithTypeAndPropertyEditors.PhoneNumberEditorTemplate, phoneNumberEditor.EditorTemplate);

      PropertyEditor tallEditor = (PropertyEditor)(selectorExtended.GetEditor(_tallProperty));
      Assert.IsTrue(tallEditor.CanEdit(_tallProperty));
      Assert.AreEqual(ExtendsWithTypeAndPropertyEditors.PersonTallnessEditorTemplate, tallEditor.EditorTemplate);
    }

    [Test]
    public void RecogniseSelfEditingNodes()
    {
      EditorSelector selector = new EditorSelector(null);

      DataTemplate emptyTemplate = new DataTemplate();
      StaticNodeEditor staticEditor = new StaticNodeEditor();
      staticEditor.EditorTemplate = emptyTemplate;
      Node node1 = new PropertyNode(new Person(), "tall", typeof(Person).GetProperty("Tall"), null, staticEditor);

      Assert.IsTrue(selector.GetEditSettings(node1).CanEditInPlace);
      Assert.AreEqual(staticEditor, selector.GetEditor(node1));
      Assert.IsNotNull(selector.SelectTemplate(node1, null));

      // ENHANCEMENT: This is a smoke test; it would be useful to add
      // a UI test to prove that the correct template is being instantiated.

      DynamicNodeEditor dynamicEditor = new DynamicNodeEditor();
      dynamicEditor.EditorTemplateKey = PropertyGrid.SimpleTextEditorKey;
      Node node2 = new PropertyNode(new Person(), "address", typeof(Person).GetProperty("Address"), null, dynamicEditor);

      Assert.IsTrue(selector.GetEditSettings(node2).CanEditInPlace);
      Assert.AreEqual(dynamicEditor, selector.GetEditor(node2));
      Assert.IsNotNull(selector.SelectTemplate(node2, null));
    }

    [Test]
    public void HandlesPropertyGridRow()
    {
      Node property = new PropertyNode("Bob", typeof(string).GetProperty("Length"), null);
      PropertyGridRow row = new PropertyGridRow(property);
      EditorSelector selector = new EditorSelector(null);
      Assert.IsNotNull(selector.SelectTemplate(row, null));
    }

    [Test]
    public void HandlesNull()
    {
      Assert.IsNull(new EditorSelector(null).SelectTemplate(null, null));
    }

    #region Helper implementations of IExtendInPlaceEditors

    private class ExtendsWithNullCollection : IExtendInPlaceEditors
    {
      public EditorCollection Editors
      {
        get { return null; }
      }

      public EditorDecorationCollection EditorDecorations
      {
        get { return null; }
      }

      public BuiltInEditorStyleCollection BuiltInEditorStyles
      {
        get { return null; }
      }
    }

    private class ExtendsWithTypeAndPropertyEditors : IExtendInPlaceEditors
    {
      public static readonly DataTemplate PhoneNumberEditorTemplate = new DataTemplate();
      public static readonly DataTemplate PersonTallnessEditorTemplate = new DataTemplate();

      private readonly EditorCollection _editors = new EditorCollection();

      public ExtendsWithTypeAndPropertyEditors()
      {
        TypeEditor phoneNumberEditor = new TypeEditor();
        phoneNumberEditor.EditedType = typeof(PhoneNumber);
        phoneNumberEditor.EditorTemplate = PhoneNumberEditorTemplate;
        _editors.Add(phoneNumberEditor);

        TypeEditor puppyEditor = new TypeEditor();
        puppyEditor.EditedType = typeof(Puppy);
        puppyEditor.EditorTemplate = new DataTemplate();
        puppyEditor.AllowExpand = true;
        _editors.Add(puppyEditor);

        PropertyEditor personTallnessEditor = new PropertyEditor();
        personTallnessEditor.DeclaringType = typeof(Person);
        personTallnessEditor.PropertyName = "Tall";
        personTallnessEditor.EditorTemplate = PersonTallnessEditorTemplate;
        _editors.Add(personTallnessEditor);
      }

      public EditorCollection Editors
      {
        get { return _editors; }
      }

      public EditorDecorationCollection EditorDecorations
      {
        get { return null; }
      }

      public BuiltInEditorStyleCollection BuiltInEditorStyles
      {
        get { return null; }
      }
    }

    #endregion
  }
}
