using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Threading;

namespace Mindscape.WpfElements
{
  internal class DateTimePickerModel
  {
    public DateTimePickerModel()
    {
      Reformat();
    }

    internal event EventHandler ValueChanged;

    private DateTime _value = DateTime.Now;
    private DateTime _minimum = DateTime.MinValue;
    private DateTime _maximum = DateTime.MaxValue;

    public DateTime Value
    {
      get { return _value; }
      set
      {
        try
        {
          _inInternalValueUpdate = true;
          DateTime previousValue = _value;
          _value = value;

          foreach (DateTimeDisplayElement element in _text)
          {
            element.Bind(Value);
          }

          if (previousValue != value && ValueChanged != null)
          {
            ValueChanged(this, EventArgs.Empty);
          }
          Constrain();
        }
        finally
        {
          _inInternalValueUpdate = false;
        }
      }
    }

    public DateTime Minimum
    {
      get { return _minimum; }
      set
      {
        if (_minimum != value)
        {
          _minimum = value;
          Constrain();
        }
      }
    }

    public DateTime Maximum
    {
      get { return _maximum; }
      set
      {
        if (_maximum != value)
        {
          _maximum = value;
          Constrain();
        }
      }
    }

    private void Constrain()
    {
      // TODO It would be nice to remove the need for using a Dispatcher here.
      // The problem is that the numeric displays of a DateTimePicker are binding to the Value property which can cause
      // this constrint method to be called. Doing this fails to send the value back through the binding to the numeric display
      // because we are already in the property change caused by the binding.
      // The best option may be to set the minimum and maximum properties on the numeric display based on the minimum and maximum here.
      // But this would be tricky to get right.
      Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
      {
        DateTime constrained = Value;
        if (Value < Minimum)
        {
          constrained = Minimum;
        }
        if (Value > Maximum)
        {
          constrained = Maximum;
        }
        if (constrained != Value)
        {
          Value = constrained;
        }
      }));
    }

    private bool _inInternalValueUpdate;

    private CultureInfo _culture = CultureInfo.CurrentCulture;

    public CultureInfo Culture
    {
      get { return _culture; }
      set
      {
        _culture = value;
        Reformat();
      }
    }

    private string _customFormat = String.Empty;

    public string CustomFormat
    {
      get { return _customFormat; }
      set
      {
        if (value == null)
        {
          value = "";
        }
        //Invariant.ArgumentNotNull(value, "value");

        _customFormat = value;
        Reformat();
      }
    }

    private DateTimePickerFormat _format;

    public DateTimePickerFormat Format
    {
      get { return _format; }
      set
      { 
        _format = value; 
        Reformat();
      }
    }

    internal string GetFormatString()
    {
      switch (Format)
      {
        case DateTimePickerFormat.LongDate: return Culture.DateTimeFormat.LongDatePattern;
        case DateTimePickerFormat.ShortDate: return Culture.DateTimeFormat.ShortDatePattern;
        case DateTimePickerFormat.LongTime: return Culture.DateTimeFormat.LongTimePattern;
        case DateTimePickerFormat.ShortTime: return Culture.DateTimeFormat.ShortTimePattern;
        case DateTimePickerFormat.Custom: return CustomFormat;
        default: throw new InvalidOperationException();
      }
    }

    private readonly ObservableCollection<DateTimeDisplayElement> _text = new ObservableCollection<DateTimeDisplayElement>();

    internal ObservableCollection<DateTimeDisplayElement> Text
    {
      get { return _text; }
    }

    internal void Reformat()
    {
      _text.Clear();
      foreach (DateTimeDisplayElement element in GetDisplayElements())
      {
        _text.Add(element);
        element.PropertyChanged += ElementTextChanged;
      }
    }

    private void ElementTextChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "TextCore" && !_inInternalValueUpdate)
      {
        Value = Composer.Compose(_text, Value, Culture);
      }
    }

    internal IEnumerable<DateTimeDisplayElement> GetDisplayElements()
    {
      if (Value == null)
      {
        yield break;
      }

      foreach (Token token in GetFormatStringTokens())
      {
        DateTimeDisplayElement element = DateTimeDisplayElement.Create(token, Culture);
        element.Bind(Value);
        yield return element;
      }
    }

    internal IEnumerable<Token> GetFormatStringTokens()
    {
      bool insideSingleQuotes = false;
      bool insideDoubleQuotes = false;
      bool escapeNext = false;

      string format = GetFormatString();

      CharEnumerator enumerator = format.GetEnumerator();

      Token currentToken = new Token(String.Empty, false);
      while (enumerator.MoveNext())
      {
        char ch = enumerator.Current;

        if (!escapeNext)
        {
          if (ch == '\'')
          {
            if (insideSingleQuotes)
            {
              currentToken.OriginalText += ch;
              yield return currentToken;
              insideSingleQuotes = false;
              currentToken = new Token(String.Empty, false);
            }
            else
            {
              if (!String.IsNullOrEmpty(currentToken.OriginalText))
              {
                yield return currentToken;
              }
              insideSingleQuotes = true;
              currentToken = new Token(String.Empty, true);
              currentToken.OriginalText = "'";
            }
            continue;
          }
          if (ch == '"')
          {
            if (insideDoubleQuotes)
            {
              currentToken.OriginalText += ch;
              yield return currentToken;
              insideDoubleQuotes = false;
              currentToken = new Token(String.Empty, false);
            }
            else
            {
              if (!String.IsNullOrEmpty(currentToken.OriginalText))
              {
                yield return currentToken;
              }
              insideDoubleQuotes = true;
              currentToken = new Token(String.Empty, true);
              currentToken.OriginalText = "\"";
            }
            continue;
          }
          if (ch == '\\')
          {
            escapeNext = true;
            continue;
          }
        }

        bool escaping = insideSingleQuotes || insideDoubleQuotes || escapeNext;

        if (IsCompatibleWith(ch, escaping, currentToken))
        {
          currentToken.FormatString += ch;
          if (escapeNext)
          {
            currentToken.OriginalText += "\\";
          }
          currentToken.OriginalText += ch;
        }
        else
        {
          yield return currentToken;
          currentToken = new Token(ch.ToString(), escaping);
          if (escapeNext)
          {
            currentToken.OriginalText = "\\" + ch;
          }
        }

        escapeNext = false;
      }

      if (!String.IsNullOrEmpty(currentToken.FormatString))
      {
        yield return currentToken;
      }
    }

    internal static bool IsCompatibleWith(char ch, bool isEscaped, Token token)
    {
      if (token.FormatString.Length == 0)
      {
        return true;
      }

      string knownChars = "dMyghHmst:/";
      bool currentTokenIsLiteral = token.ForceLiteral || knownChars.IndexOf(token.FormatString[0]) < 0;
      bool charIsSpecialChar = knownChars.IndexOf(ch) >= 0 && !isEscaped;

      if (currentTokenIsLiteral)
      {
        return !charIsSpecialChar;
      }

      if (!charIsSpecialChar)
      {
        return false;
      }

      if (token.FormatString == ":" || token.FormatString == "/")
      {
        return false;
      }

      return ch == token.FormatString[0];
    }
  }

  [DebuggerDisplay("Token {OriginalText}")]
  internal class Token
  {
    private string _formatString;

    public string FormatString
    {
      get { return _formatString; }
      set { _formatString = value; }
    }

    public string OriginalText { get; set; }

    private readonly bool _forceLiteral;

    public bool ForceLiteral
    {
      get { return _forceLiteral; }
    }

    public Token(string content, bool forceLiteral)
    {
      _formatString = content;
      OriginalText = content;
      _forceLiteral = forceLiteral;
    }

    public bool IsLiteral
    {
      get
      {
        return _forceLiteral 
          || String.IsNullOrEmpty(_formatString)
          || "dMyghHmst".IndexOf(_formatString[0]) < 0;
      }
    }
  }
}
