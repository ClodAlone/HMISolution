using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Mindscape.WpfElements.PropertyEditing
{
  /// <summary>
  /// Represents an object that encapsulates a <see cref="PropertyDescriptor"/>.
  /// </summary>
  public interface IPropertyDescriptorWrapper
  {
    /// <summary>
    /// Gets the encapsulated <see cref="PropertyDescriptor"/>.
    /// </summary>
    PropertyDescriptor PropertyDescriptor { get; }
  }
}
