using System;
using System.Globalization;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Represents an element within a <see cref="DateTimePicker"/> control that
  /// displays a part of the formatted DateTime as an editable numeric value
  /// (e.g. the year).
  /// </summary>
  public sealed class DateTimeNumericDisplayElement : DateTimeDisplayElement
  {
    internal DateTimeNumericDisplayElement(Token token, CultureInfo culture) 
      : base(token, culture) { }

    /// <summary>
    /// Gets the <see cref="DateTimeDisplayElementType"/> of the element.
    /// </summary>
    public override DateTimeDisplayElementType ElementType
    {
      get { return DateTimeDisplayElementType.Numeric; }
    }

    private int _value;

    /// <summary>
    /// Gets or sets the numeric value.
    /// </summary>
    public int Value
    {
      get { return _value; }
      set
      {
        bool isChanging = (_value != value);
        Set(ref _value, value, "Value");
        if (isChanging)
        {
          OnPropertyChanged("TextCore");
        }
      }
    }


    private int _minimum;

    /// <summary>
    /// Gets the minimum permitted value.
    /// </summary>
    public int Minimum
    {
      get { return _minimum; }
      private set { Set(ref _minimum, value, "Minimum"); }
    }

    private int _maximum;

    /// <summary>
    /// Gets the maximum permitted value.
    /// </summary>
    public int Maximum
    {
      get { return _maximum; }
      private set { Set(ref _maximum, value, "Maximum"); }
    }

    private int _precision;

    /// <summary>
    /// Gets the precision.  Values should, if necessary, be displayed padded with leading
    /// zeroes until there are at least as many digits as specified by the precision.
    /// </summary>
    public int Precision
    {
      get { return _precision; }
      private set { Set(ref _precision, value, "Precision"); }
    }

    internal override bool IncludeInParse
    {
      get { return true; }
    }

    internal override string ParseableText
    {
      get
      {
        string format = new string('0', Precision);
        return Value.ToString(format, Culture);
      }
    }

    private bool IsLimitedToTwoDigits
    {
      get
      {
        char formatChar = FormatStringFragment[0];
        return formatChar == 'h' || formatChar == 'H' || formatChar == 'm' || formatChar == 's';
      }
    }

    internal override string TextCore
    {
      get
      {
        return ParseableText;
      }
      set
      {
        Value = Int32.Parse(value, Culture); 
        OnPropertyChanged("TextCore");
      }
    }

    // Heuristic value to give these elements a bit of extra priority in proportion
    // calculations: they are typically fairly short and without this they tend to lose out
    // and risk getting clipped.
    private const int ProportionCorrection = 5;

    internal override void OnBind(DateTime value)
    {
      string formatString = Token.FormatString;
      if (formatString.Length == 1)
      {
        formatString = "%" + formatString;
      }

      Range<int> range = DateTimeFormatString.GetValueRange(FormatStringFragment, Culture, value);
      Minimum = range.Minimum;
      Maximum = range.Maximum;

      string extractedText = value.ToString(formatString, Culture);
      Value = Int32.Parse(extractedText, Culture);

      int digitCount = FormatStringFragment.Length;
      if (IsLimitedToTwoDigits)
      {
        digitCount = Math.Min(digitCount, 2);
      }
      Precision = digitCount;

      int estimatedProportion = Minimum.ToString(Culture).Length;
      estimatedProportion = Math.Max(estimatedProportion, Maximum.ToString(Culture).Length);
      EstimatedProportion = estimatedProportion * EstimatedProportionScale + ProportionCorrection;
    }
  }
}
