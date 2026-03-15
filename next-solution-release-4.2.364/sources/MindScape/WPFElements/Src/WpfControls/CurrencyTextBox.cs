using System;
using System.ComponentModel;
using System.Windows;
using Infralution.Licensing;
using System.Windows.Controls.Primitives;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// A text box for editing currency values.
  /// </summary>
  [LicenseProvider(typeof(PublicEncryptedLicenseProvider))]
  public class CurrencyTextBox : NumericTextBoxBase<decimal>
  {
    private readonly CurrencyTextBoxModel _ct = new CurrencyTextBoxModel();

    internal override NumericTextBoxBaseModel<decimal> NumericModel
    {
      get { return _ct; }
    }

    static CurrencyTextBox()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(CurrencyTextBox), 
        new FrameworkPropertyMetadata(typeof(CurrencyTextBox)));
      MinimumProperty.OverrideMetadata(typeof(CurrencyTextBox),
        new FrameworkPropertyMetadata(Decimal.MinValue));
      MaximumProperty.OverrideMetadata(typeof(CurrencyTextBox),
        new FrameworkPropertyMetadata(Decimal.MaxValue));
    }

    /// <summary>
    /// Gets or sets the <see cref="Style"/> used to display negative values.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="NegativeStyleProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public Style NegativeStyle
    {
      get { return (Style)GetValue(NegativeStyleProperty); }
      set { SetValue(NegativeStyleProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="NegativeStyle"/> property.
    /// </summary>
    public static readonly DependencyProperty NegativeStyleProperty =
        DependencyProperty.Register("NegativeStyle", typeof(Style), typeof(CurrencyTextBox));

    /// <summary>
    /// Gets or sets whether the user is prevented from typing beyond the permitted
    /// number of decimal places (as determined by the culture).
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="EnforceDecimalPlacesProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool EnforceDecimalPlaces
    {
      get { return (bool)GetValue(EnforceDecimalPlacesProperty); }
      set { SetValue(EnforceDecimalPlacesProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="EnforceDecimalPlaces"/> property.
    /// </summary>
    public static readonly DependencyProperty EnforceDecimalPlacesProperty =
      DependencyProperty.Register("EnforceDecimalPlaces", typeof(bool), typeof(CurrencyTextBox),
      new FrameworkPropertyMetadata(true, OnEnforceDecimalPlacesChanged));

    private static void OnEnforceDecimalPlacesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((CurrencyTextBox)d).OnEnforceDecimalPlacesChanged();
    }

    private void OnEnforceDecimalPlacesChanged()
    {
      _ct.EnforceDecimalPlaces = EnforceDecimalPlaces;
    }

    #region DecimalPlaces Property

    /// <summary>
    /// Gets or sets the number of decimal places to display. Set to -1
    /// to display the default number of decimal places for the culture.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="DecimalPlacesProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public int DecimalPlaces
    {
      get { return (int)GetValue(DecimalPlacesProperty); }
      set { SetValue(DecimalPlacesProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="DecimalPlaces"/> property.
    /// </summary>
    public static readonly DependencyProperty DecimalPlacesProperty =
      DependencyProperty.Register("DecimalPlaces", typeof(int), typeof(CurrencyTextBox),
      new FrameworkPropertyMetadata(-1, OnDecimalPlacesChanged));

    private static void OnDecimalPlacesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((CurrencyTextBox)d).OnDecimalPlacesChanged();
    }

    private void OnDecimalPlacesChanged()
    {
      _ct.DecimalPlaces = DecimalPlaces;
      UpdateIfInitialised();
    }

    #endregion // DecimalPlaces Property



    internal override bool OnEndUserInteraction()
    {
      _ct.ForceRefreshTextFromValue();

      base.OnEndUserInteraction();

      return true;
    }

    internal override void OnRangeCheck()
    {
      _ct.ForceRefreshTextFromValue();
    }
  }
}
