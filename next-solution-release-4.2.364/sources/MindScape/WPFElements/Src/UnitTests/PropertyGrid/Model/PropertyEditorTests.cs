using NUnit.Framework;
using System.Windows;
using System.Collections.Generic;
using Mindscape.WpfElements.WpfPropertyGrid.UnitTests;

namespace Mindscape.WpfElements.PropertyEditing.UnitTests
{
  [TestFixture]
  public class PropertyEditorTests
  {
    [Test]
    public void SimpleProperties()
    {
      DataTemplate template = new DataTemplate();
      Style style = new Style();

      PropertyEditor editor = new PropertyEditor();
      editor.DeclaringType = typeof(string);
      editor.PropertyName = "Fie";
      editor.EditorTemplate = template;
      editor.Style = style;

      Assert.AreEqual(typeof(string), editor.DeclaringType);
      Assert.AreEqual("Fie", editor.PropertyName);
      Assert.AreEqual(template, editor.EditorTemplate);
      Assert.AreEqual(style, editor.Style);

      // Attached properties
      Style hostStyle = new Style();
      DependencyObject d = new DependencyObject();
      PropertyEditor.SetHostStyle(d, hostStyle);
      Assert.AreEqual(hostStyle, PropertyEditor.GetHostStyle(d));
    }

    private static readonly Node _aliveProperty = new PropertyNode(new Person(), typeof(Person).GetProperty("Alive"), null);
    private static readonly Node _otherBooleanProperty = new PropertyNode(new Person(), typeof(Person).GetProperty("Tall"), null);
    private static readonly Node _alivePropertyOnOtherType = new PropertyNode(new Puppy(null), typeof(Puppy).GetProperty("Alive"), null);

    [Test]
    public void CanEdit()
    {
      Dictionary<string, int> dict = new Dictionary<string,int>();
      dict["Hat Size"] = 123;
      Node dictionaryItem = new CollectionElement(dict, CollectionTypeOperations.Dictionary, "Hat Size", true, null);

      PropertyEditor editor = new PropertyEditor();
      editor.DeclaringType = typeof(Person);
      editor.PropertyName = "Alive";

      Assert.IsTrue(editor.CanEdit(_aliveProperty));
      Assert.IsFalse(editor.CanEdit(_otherBooleanProperty));
      Assert.IsFalse(editor.CanEdit(_alivePropertyOnOtherType));
      Assert.IsFalse(editor.CanEdit(dictionaryItem));

      PropertyEditor editor2 = new PropertyEditor();
      editor2.DeclaringType = null;
      editor2.PropertyName = "Hat Size";

      Assert.IsTrue(editor2.CanEdit(dictionaryItem));
    }
  }
}
