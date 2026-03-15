using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Windows;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// A text box for editing percentage values.
  /// </summary>
  public class PercentageTextBox : NumericTextBoxBase<decimal>
  {
    static PercentageTextBox()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(PercentageTextBox),
        new FrameworkPropertyMetadata(typeof(PercentageTextBox)));
      MinimumProperty.OverrideMetadata(typeof(PercentageTextBox),
        new FrameworkPropertyMetadata(0.0m));
      MaximumProperty.OverrideMetadata(typeof(PercentageTextBox),
        new FrameworkPropertyMetadata(1.0m));
    }

    private PercentageTextBoxModel _model = new PercentageTextBoxModel();

    internal override NumericTextBoxBaseModel<decimal> NumericModel
    {
      get { return _model; }
    }

    /// <summary>
    /// Gets or sets the number of decimal places to display.  Set to -1
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
      DependencyProperty.Register("DecimalPlaces", typeof(int), typeof(PercentageTextBox),
      new FrameworkPropertyMetadata(-1, OnDecimalPlacesChanged));

    private static void OnDecimalPlacesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((PercentageTextBox)d).OnDecimalPlacesChanged();
    }

    private void OnDecimalPlacesChanged()
    {
      _model.DecimalPlaces = DecimalPlaces;
      UpdateIfInitialised();
    }

    internal override bool OnEndUserInteraction()
    {
      _model.ForceRefreshTextFromValue();

      base.OnEndUserInteraction();

      return true;
    }

    internal override void OnRangeCheck()
    {
      _model.ForceRefreshTextFromValue();
    }
  }

  internal class PercentageTextBoxModel : NumericTextBoxModel
  {
    public PercentageTextBoxModel()
    {
      Minimum = 0;
      Maximum = 1;
    }

    internal override string FormatString
    {
      get
      {
        string formatString = "P";
        if (DecimalPlaces >= 0)
        {
          formatString += DecimalPlaces.ToString(CultureInfo.InvariantCulture);
        }
        return formatString;
      }
    }

    internal override string FormatStringNoDecimalPlaces
    {
      get { return "P0"; }
    }

    internal override bool TryParse(string text, out decimal result)
    {
      decimal percentage;
      if (base.TryParse(text.Replace("%", ""), out percentage))
      {
        result = percentage / 100;
        return true;
      }
      result = 0;
      return false;
    }
  }
}
