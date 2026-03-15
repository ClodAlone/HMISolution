using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Mindscape.WpfElements.WpfPropertyGrid.UnitTests
{
  public class DependencyObjectModel : DependencyObject
  {
    #region Quantity Property

    /// <summary>
    /// Gets or sets the Quantity. The default is 13.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="QuantityProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public double Quantity
    {
      get { return (double)GetValue(QuantityProperty); }
      set { SetValue(QuantityProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Quantity"/> property.
    /// </summary>
    public static readonly DependencyProperty QuantityProperty =
      DependencyProperty.Register("Quantity", typeof(double), typeof(DependencyObjectModel),
      new FrameworkPropertyMetadata(13.0, OnQuantityChanged));

    private static void OnQuantityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DependencyObjectModel)d).OnQuantityChanged();
    }

    private void OnQuantityChanged()
    {
    }

    #endregion // Quantity Property
  }
}
