using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Mindscape.WpfElements.PropertyEditing
{
  /// <summary>
  /// An interface for controls that host other controls.
  /// </summary>
  public interface IProvideEditorHosting
  {
    /// <summary>
    /// Gets the default margin around hosted controls.
    /// </summary>
    Thickness DefaultMargin { get; }

    /// <summary>
    /// Gets the collection of built in editor styles.
    /// </summary>
    BuiltInEditorStyleCollection BuiltInEditorStyles { get; }

    /// <summary>
    /// Finds the resource for the given resource key.
    /// </summary>
    /// <param name="resourceKey">The resource key.</param>
    /// <returns>The object mapped to the given resource key.</returns>
    object FindResource(object resourceKey);

    /// <summary>
    /// Gets whether or not default margin compensation is required.
    /// </summary>
    bool DefaultMarginCompensationRequired { get; }
  }
}
