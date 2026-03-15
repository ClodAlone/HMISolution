using System;
using System.Globalization;

namespace Mindscape.WpfElements
{
  internal class NumericTextBoxModel : NumericTextBoxBaseModel<decimal>
  {
    public NumericTextBoxModel()
    {
      Minimum = Decimal.MinValue;
      Maximum = Decimal.MaxValue;
    }

    internal override string GroupSeparator
    {
      get { return Culture.NumberFormat.NumberGroupSeparator; }
    }

    internal override string FormatString
    {
      get
      {
        string formatString = "N";
        if (DecimalPlaces >= 0)
        {
          formatString += DecimalPlaces.ToString(CultureInfo.InvariantCulture);
        }
        return formatString;
      }
    }

    internal override string FormatStringNoDecimalPlaces
    {
      get { return "N0"; }
    }

    internal override bool TryParse(string text, out decimal result)
    {
      if (text.EndsWith("e") && text.Length > 1)
      {
        text = text.Substring(0, text.Length - 1);
      }
      else if (text.EndsWith("e-") && text.Length > 2)
      {
        text = text.Substring(0, text.Length - 2);
      }
      bool canParse = Decimal.TryParse(text, NumberStyles.Number | NumberStyles.AllowParentheses | NumberStyles.AllowExponent, Culture, out result);
      if (canParse && EnforceDecimalPlaces && DecimalPlaces >= 0)
      {
        decimal truncated = Decimal.Round(result, DecimalPlaces);
        canParse = (truncated == result);
      }
      return canParse;
    }

    private int _decimalPlaces = -1;
    private bool _enforceDecimalPlaces = true;

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

  internal class DoubleTextBoxModel : NumericTextBoxBaseModel<double>
  {
    public DoubleTextBoxModel()
    {
      Minimum = Double.MinValue;
      Maximum = Double.MaxValue;
    }

    internal override string GroupSeparator
    {
      get { return Culture.NumberFormat.NumberGroupSeparator; }
    }

    internal override string FormatString
    {
      get
      {
        string formatString = "N";
        if (DecimalPlaces >= 0)
        {
          formatString += DecimalPlaces.ToString(CultureInfo.InvariantCulture);
        }
        return formatString;
      }
    }

    internal override string FormatStringNoDecimalPlaces
    {
      get { return "N0"; }
    }

    internal override bool TryParse(string text, out double result)
    {
      if (text.EndsWith("e") && text.Length > 1)
      {
        text = text.Substring(0, text.Length - 1);
      }
      else if (text.EndsWith("e-") && text.Length > 2)
      {
        text = text.Substring(0, text.Length - 2);
      }
      bool canParse = Double.TryParse(text, NumberStyles.Number | NumberStyles.AllowParentheses | NumberStyles.AllowExponent, Culture, out result);
      if (canParse && EnforceDecimalPlaces && DecimalPlaces >= 0)
      {
        double truncated = Math.Round(result, DecimalPlaces);
        canParse = (truncated == result);
      }
      return canParse;
    }

    private int _decimalPlaces = -1;
    private bool _enforceDecimalPlaces = true;

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

    internal bool EnforceDecimalPlaces
    {
      get { return _enforceDecimalPlaces; }
      set { _enforceDecimalPlaces = value; }
    }

    protected internal override double Zero
    {
      get { return 0.0; }
    }

    protected internal override double NoValue
    {
      get { return Double.NaN; }
    }
  }

  internal class IntegerTextBoxModel : NumericTextBoxBaseModel<int>
  {
    public IntegerTextBoxModel()
    {
      Minimum = Int32.MinValue;
      Maximum = Int32.MaxValue;
    }

    internal override string FormatString
    {
      get
      {
        string formatString = "N0";
        if (Precision > 0)
        {
          formatString = "D" + Precision.ToString(CultureInfo.InvariantCulture);
        }
        return formatString;
      }
    }

    internal override string FormatStringNoDecimalPlaces
    {
      get { return FormatString; }
    }

    internal override bool TryParse(string text, out int result)
    {
      bool canParse = Int32.TryParse(text, NumberStyles.Integer, Culture, out result);
      if (canParse && EnforcePrecision && Precision > 0)
      {
        string digits = result.ToString("#", CultureInfo.InvariantCulture);
        if (digits.Length > Precision)
        {
          canParse = false;
        }
      }
      return canParse;
    }

    internal override string GroupSeparator
    {
      get { return Culture.NumberFormat.NumberGroupSeparator; }
    }

    private int _precision;
    private bool _enforcePrecision = true;

    public int Precision
    {
      get { return _precision; }
      set
      {
        Invariant.ArgumentNotNegative(value, "value");

        _precision = value;

        if (ValueIsControllingProperty)
        {
          RefreshTextFromValue();
        }
      }
    }

    internal bool EnforcePrecision
    {
      get { return _enforcePrecision; }
      set { _enforcePrecision = value; }
    }

    protected internal override int Zero
    {
      get { return 0; }
    }
  }
}
