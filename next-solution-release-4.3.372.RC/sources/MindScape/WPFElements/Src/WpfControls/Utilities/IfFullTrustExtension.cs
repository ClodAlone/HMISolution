using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Security.Permissions;
using System.IO;
using System.Xml;
using System.Diagnostics;

namespace Mindscape.WpfElements
{
  // Based on: http://joshsmithonwpf.wordpress.com/category/xbap/

  /// <summary>
  /// Provides conditional logic for creating objects in xaml that are not supported in partial trust mode.
  /// </summary>
  [ContentProperty("Xaml")]
  public class IfFullTrustExtension : MarkupExtension
  {
    private static bool _isFullTrust;

    static IfFullTrustExtension()
    {
      try
      {
        PermissionState state = PermissionState.Unrestricted;
        new UIPermission(state).Assert();
        _isFullTrust = true;
      }
      catch { }
    }

    /// <summary>
    /// The xaml code for creating the object.
    /// </summary>
    public string Xaml { get; set; }

    /// <summary>
    /// Creates an object using the Xaml property if the current environment is in full trust mode.
    /// </summary>
    /// <param name="serviceProvider">The <see cref="IServiceProvider"/>.</param>
    /// <returns>An object based on the Xaml property if full trust is enabled. Otherwise returns null.</returns>
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      object o = null;
      if (_isFullTrust)
      {
        try
        {
          using (StringReader stringReader = new StringReader(Xaml))
          {
            using (XmlReader xmlReader = XmlReader.Create(stringReader))
            {
              o = XamlReader.Load(xmlReader);
            }
          }
        }
        catch (Exception e)
        {
          Debug.Fail("Invalid XAML: " + e);
        }
      }
      return o;
    }
  }
}
