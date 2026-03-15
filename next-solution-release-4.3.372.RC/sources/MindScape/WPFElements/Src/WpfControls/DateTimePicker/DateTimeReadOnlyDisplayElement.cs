using System;
using System.Globalization;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Represents an element within a <see cref="DateTimePicker"/> control that
  /// may not be edited by the user, such as literal text.
  /// </summary>
  public sealed class DateTimeReadOnlyDisplayElement : DateTimeDisplayElement
  {
    internal DateTimeReadOnlyDisplayElement(Token token, CultureInfo culture) 
      : base(token, culture) { }

    /// <summary>
    /// Gets the <see cref="DateTimeDisplayElementType"/> of the element.
    /// </summary>
    public override DateTimeDisplayElementType ElementType
    {
      get { return DateTimeDisplayElementType.ReadOnly; }
    }

    private string _text;

    /// <summary>
    /// Gets the display text.
    /// </summary>
    public string Text
    {
      get { return _text; }
      private set { Set(ref _text, value, "Text"); }
    }

    internal override string TextCore
    {
      get
      {
        return Text;
      }
      set
      {
        Text = value;
        OnPropertyChanged("TextCore");
      }
    }

    internal override bool IncludeInParse
    {
      get { return false; }
    }

    internal override string ParseableText
    {
      get { return Text; }
    }

    internal override void OnBind(DateTime value)
    {
      if (Token.IsLiteral)
      {
        Text = Token.FormatString;

        if (Token.FormatString == "/")
        {
          Text = Culture.DateTimeFormat.DateSeparator;
        }
        if (Token.FormatString == ":")
        {
          Text = Culture.DateTimeFormat.TimeSeparator;
        }

        EstimatedProportion = Text.Length * EstimatedProportionScale;
      }
      else
      {
        string formatString = Token.FormatString;
        if (formatString.Length == 1)
        {
          formatString = "%" + formatString;
        }

        Text = value.ToString(formatString, Culture);

        int estimatedProportion = 1;
        foreach (string possibleValue in DateTimeFormatString.GetPermittedValues(FormatStringFragment, Culture, value))
        {
          estimatedProportion = Math.Max(estimatedProportion, possibleValue.Length);
        }
        EstimatedProportion = estimatedProportion * EstimatedProportionScale;
      }
    }
  }
}
