using System;
using System.Windows.Markup;
using System.Windows;
using System.Windows.Data;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Provides a shortcut XAML syntax for specifying the child items of an
  /// item in a <see cref="MulticolumnTreeView"/>.
  /// </summary>
  public class ChildPathExtension : MarkupExtension
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ChildPathExtension"/> class.
    /// </summary>
    /// <remarks>Use this constructor if you intend to set the <see cref="Binding"/>
    /// property explicitly.</remarks>
    public ChildPathExtension() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ChildPathExtension"/> class.
    /// </summary>
    /// <param name="path">The child collection property path.</param>
    /// <remarks>Use this constructor if you do not intend to set the <see cref="Binding"/>
    /// property explicitly.  If you set the Binding property, it will override the path
    /// set in the constructor.</remarks>
    public ChildPathExtension(string path)
    {
      Invariant.ArgumentNotNull(path, "path");

      Binding = new Binding(path);
    }

    /// <summary>
    /// Gets or sets the data binding for the child collection.
    /// </summary>
    public BindingBase Binding { get; set; }

    /// <summary>
    /// Returns an object that is set as the value of the target property for this markup extension.
    /// </summary>
    /// <param name="serviceProvider">Object that can provide services for the markup extension.</param>
    /// <returns>The object value to set on the property where the extension is applied.</returns>
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      HierarchicalDataTemplate template = new HierarchicalDataTemplate();
      template.ItemsSource = Binding;
      return template;
    }
  }
}
