using System;
using System.Globalization;

namespace Mindscape.WpfElements
{
  internal class CurrencyTextBoxModel : NumericTextBoxBaseModel<decimal>
  {
    private int _decimalPlaces = -1;

    public CurrencyTextBoxModel()
    {
      Minimum = Decimal.MinValue;
      Maximum = Decimal.MaxValue;
    }

    internal override string FormatString
    {
      get
      {
        string formatString = "C";
        if (_decimalPlaces >= 0)
        {
          formatString += DecimalPlaces.ToString(CultureInfo.InvariantCulture);
        }
        return formatString;
      }
    }

    internal override string FormatStringNoDecimalPlaces
    {
      get { return "C0"; }
    }

    internal override string GroupSeparator
    {
      get { return Culture.NumberFormat.CurrencyGroupSeparator; }
    }

    internal override bool TryParse(string text, out decimal result)
    {
      bool canParse = Decimal.TryParse(text, NumberStyles.Currency, Culture, out result);

      if (canParse && EnforceDecimalPlaces)
      {
        int decimalPlaces = Culture.NumberFormat.CurrencyDecimalDigits;
        decimal truncated = Decimal.Round(result, decimalPlaces);
        canParse = (truncated == result);
      }

      return canParse;
    }

    internal int DecimalPlaces
    {
      get { return _decimalPlaces; }
      set
      {
        if (value < -1)
        {
          throw new ArgumentOutOfRangeException("value", "DecimalPlaces must be non-negative, or -1 for culture default");
        }

        _decimalPlaces = value;

        if (ValueIsControllingProperty)
        {
          RefreshTextFromValue();
        }
      }
    }

    private bool _enforceDecimalPlaces = true;

    internal bool EnforceDecimalPlaces
    {
      get { return _enforceDecimalPlaces; }
      set { _enforceDecimalPlaces = value; }
    }

    protected internal override decimal Zero
    {
      get { return 0; }
    }
  }
}
