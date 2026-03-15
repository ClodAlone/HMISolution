using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Markup;
using System.Xml;
using System.IO;

namespace Mindscape.WpfElements.UnitTests
{
  internal static class MarkupUtils
  {
    internal static T LoadXaml<T>(string xaml)
      where T : class
    {
      ParserContext context = new ParserContext();
      context.XamlTypeMapper = new XamlTypeMapper(new string[] { typeof(MaskedTextBox).Assembly.FullName });
      XmlReader source = XmlReader.Create(new StringReader(xaml));
      T result = XamlReader.Load(source) as T;
      return result;
    }
  }
}
