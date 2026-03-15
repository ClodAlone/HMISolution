using NUnit.Framework;

namespace Mindscape.WpfElements.PropertyEditing.UnitTests
{
  [TestFixture]
  public class NodeEditorTests
  {
    [Test]
    public void SimpleProperties()
    {
      DynamicNodeEditor dne = new DynamicNodeEditor();
      dne.EditorTemplateKey = "fie";

      Assert.AreEqual("fie", dne.EditorTemplateKey);
    }
  }
}
