using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Represents an element within a <see cref="DateTimePicker"/> control that
  /// displays a part of the formatted DateTime as text and permits the user
  /// to select values from a predefined list.
  /// </summary>
  public sealed class DateTimeSelectDisplayElement : DateTimeDisplayElement
  {
    internal DateTimeSelectDisplayElement(Token token, CultureInfo culture) 
      : base(token, culture) { }

    /// <summary>
    /// Gets the <see cref="DateTimeDisplayElementType"/> of the element.
    /// </summary>
    public override DateTimeDisplayElementType ElementType
    {
      get { return DateTimeDisplayElementType.Select; }
    }

    private string _text;
    private string _parsableText;

    /// <summary>
    /// Gets or sets the display text.
    /// </summary>
    public string Text
    {
      get { return _text; }
      set
      {
        bool isChanging = (_text != value);
        if (_permittedValues.Contains(value))
        {
          _parsableText = value;
        }
        Set(ref _text, value, "Text");
        if (isChanging)
        {
          OnPropertyChanged("TextCore");
        }
      }
    }

    private readonly ObservableCollection<string> _permittedValues = new ObservableCollection<string>();

    /// <summary>
    /// Gets the permitted values of the <see cref="Text"/> property.
    /// </summary>
    public IEnumerable<string> PermittedValues
    {
      get { return _permittedValues; }
    }

    internal override bool IncludeInParse
    {
      get { return true; }
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

    internal override void OnBind(DateTime value)
    {
      string formatString = Token.FormatString;
      if (formatString.Length == 1)
      {
        formatString = "%" + formatString;
      }

      string text = value.ToString(formatString, Culture);

      int estimatedProportion = 1;

      IEnumerable<string> permittedValues = 
        DateTimeFormatString.GetPermittedValues(FormatStringFragment, Culture, value);

      _permittedValues.Clear();
      foreach (string permittedValue in permittedValues)
      {
        _permittedValues.Add(permittedValue);
        estimatedProportion = Math.Max(estimatedProportion, permittedValue.Length);
      }

      Text = text;

      EstimatedProportion = estimatedProportion * EstimatedProportionScale;
    }

    internal override string ParseableText
    {
      get { return _parsableText; }
    }
  }
}
