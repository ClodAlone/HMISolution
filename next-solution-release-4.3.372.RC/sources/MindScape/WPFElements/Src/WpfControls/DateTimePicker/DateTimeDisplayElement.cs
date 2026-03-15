using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Windows.Media;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Represents an element within a <see cref="DateTimePicker"/> control that
  /// displays a single part of the formatted DateTime.
  /// </summary>
  public abstract class DateTimeDisplayElement : INotifyPropertyChanged
  {
    /// <summary>
    /// Gets the <see cref="DateTimeDisplayElementType"/> of the element.
    /// </summary>
    public abstract DateTimeDisplayElementType ElementType { get; }

    internal const int EstimatedProportionScale = 10;

    private int _estimatedProportion;
    private Brush _foreground;

    /// <summary>
    /// Gets an estimated size requirement relative to other elements in the same control.
    /// This can be used by panels to size the user interface elements.
    /// </summary>
    public int EstimatedProportion
    {
      get { return _estimatedProportion; }
      internal set { Set(ref _estimatedProportion, value, "EstimatedProportion"); }
    }

    /// <summary>
    /// Gets or sets the foreground brush for this <see cref="DateTimeDisplayElement"/>.
    /// </summary>
    public Brush Foreground
    {
      get { return _foreground; }
      set { Set<Brush>(ref _foreground, value, "Foreground"); }
    }

    private readonly CultureInfo _culture;

    /// <summary>
    /// Gets the culture of the containing <see cref="DateTimePicker"/> control.
    /// </summary>
    public CultureInfo Culture
    {
      get { return _culture; }
    }

    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <param name="propertyName">The name of the property which has changed.</param>
    protected virtual void OnPropertyChanged(string propertyName)
    {
      if (PropertyChanged != null)
      {
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }
    }

    internal void Set<T>(ref T field, T value, string propertyName)
    {
      if (!Object.Equals(field, value))
      {
        field = value;
        OnPropertyChanged(propertyName);
      }
    }

    internal static DateTimeDisplayElement Create(Token token, CultureInfo culture)
    {
      string formatString = token.FormatString;

      if (token.IsLiteral 
        || formatString.StartsWith("ddd", StringComparison.Ordinal)
        || formatString.StartsWith("g", StringComparison.Ordinal))
      {
        return new DateTimeReadOnlyDisplayElement(token, culture);
      }

      if (DateTimeFormatString.IsSelect(formatString))
      {
        return new DateTimeSelectDisplayElement(token, culture);
      }

      return new DateTimeNumericDisplayElement(token, culture);
    }

    internal abstract string TextCore { get; set; }

    internal abstract bool IncludeInParse { get; }
    internal abstract string ParseableText { get; }

    private readonly Token _token;

    internal DateTimeDisplayElement(Token token, CultureInfo culture)
    {
      _token = token;
      _culture = culture;
    }

    internal Token Token
    {
      get { return _token; }
    }

    internal string FormatStringFragment
    {
      get { return _token.OriginalText; }
    }

    internal void Bind(DateTime value)
    {
      OnBind(value);
    }

    internal abstract void OnBind(DateTime value);
  }

  internal static class Composer
  {
    internal static DateTime Compose(IEnumerable<DateTimeDisplayElement> elements, DateTime defaultElememtSource, CultureInfo culture)
    {
      StringBuilder formatStringBuilder = new StringBuilder();
      StringBuilder dateTextBuilder = new StringBuilder();

      bool gotYearElement = false;
      bool gotMonthElement = false;
      bool gotDayElement = false;
      bool gotHourElement = false;
      bool gotMinuteElement = false;
      bool gotSecondElement = false;

      foreach (DateTimeDisplayElement element in elements)
      {
        if (element.IncludeInParse)
        {
          formatStringBuilder.Append(element.FormatStringFragment + " ");
          dateTextBuilder.Append(element.ParseableText + " ");

          switch (element.FormatStringFragment[0])
          {
            case 'y':
              gotYearElement = true;
              break;
            case 'M':
              gotMonthElement = true;
              break;
            case 'd':
              gotDayElement = true;
              break;
            case 'h':
            case 'H':
              gotHourElement = true;
              break;
            case 'm':
              gotMinuteElement = true;
              break;
            case 's':
              gotSecondElement = true;
              break;
          }
        }
      }

      if (!gotYearElement)
      {
        formatStringBuilder.Append("yyyy ");
        dateTextBuilder.Append(defaultElememtSource.ToString("yyyy ", culture));
      }

      if (!gotMonthElement)
      {
        formatStringBuilder.Append("MM ");
        dateTextBuilder.Append(defaultElememtSource.ToString("MM ", culture));
      }

      if (!gotDayElement)
      {
        formatStringBuilder.Append("dd ");
        dateTextBuilder.Append(defaultElememtSource.ToString("dd ", culture));
      }

      if (!gotHourElement)
      {
        formatStringBuilder.Append("HH ");
        dateTextBuilder.Append(defaultElememtSource.ToString("HH ", culture));
      }

      if (!gotMinuteElement)
      {
        formatStringBuilder.Append("mm ");
        dateTextBuilder.Append(defaultElememtSource.ToString("mm ", culture));
      }

      if (!gotSecondElement)
      {
        formatStringBuilder.Append("ss ");
        dateTextBuilder.Append(defaultElememtSource.ToString("ss ", culture));
      }

      string formatString = formatStringBuilder.ToString();
      string dateText = dateTextBuilder.ToString();

      //Debug.WriteLine(formatString + " > " + dateText);

      return DateTime.ParseExact(dateText, formatString, culture, DateTimeStyles.None);
    }
  }
}
