using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.Windows;

namespace Mindscape.WpfElements.PropertyEditing.UnitTests
{
  [TestFixture]
  public class BuiltInEditorStyleTests
  {
    [Test]
    public void SimpleProperties()
    {
      object key = new object();
      Style style = new Style();

      BuiltInEditorStyle bies = new BuiltInEditorStyle();
      bies.EditorKey = key;
      bies.Style = style;

      Assert.AreEqual(key, bies.EditorKey);
      Assert.AreEqual(style, bies.Style);
    }

    [Test]
    public void FindStyle()
    {
      BuiltInEditorStyle s1 = new BuiltInEditorStyle();
      s1.EditorKey = PropertyGrid.CheckBoxEditorKey;
      s1.Style = new Style();

      BuiltInEditorStyle s2 = new BuiltInEditorStyle();
      s2.EditorKey = PropertyGrid.SimpleTextEditorKey;
      s2.Style = new Style();

      BuiltInEditorStyleCollection styles = new BuiltInEditorStyleCollection();
      styles.Add(s1);
      styles.Add(s2);

      Assert.AreEqual(s1.Style, styles.FindStyle(PropertyGrid.CheckBoxEditorKey));
      Assert.AreEqual(s2.Style, styles.FindStyle(PropertyGrid.SimpleTextEditorKey));
      Assert.IsNull(styles.FindStyle(PropertyGrid.SliderEditorKey));
    }
  }
}
