using System;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Mindscape.WpfElements
{
  internal abstract class NumericTextBoxBaseModel<TNumber> : IFilteringTextBoxModel
    where TNumber : IFormattable, IComparable<TNumber>
  {
    internal abstract string FormatString { get; }
    internal abstract string FormatStringNoDecimalPlaces { get; }

    public NumericTextBoxBaseModel()
    {
      _value = default(TNumber);
      _text = default(TNumber).ToString(FormatString, CultureInfo.CurrentCulture);
    }

    #region Culture and culture-bound members

    private CultureInfo _culture;
    private Regex _wholeNumberPortionRegex;

    public CultureInfo Culture
    {
      get
      {
        // defer default initialisation to avoid calling a virtual method
        // from within the constructor
        if (_culture == null)
        {
          Culture = CultureInfo.CurrentCulture;
        }

        return _culture;
      }

      set
      {
        Invariant.ArgumentNotNull(value, "value");

        // Since the formatting may be about to change, if we currently have
        // a valid value, transfer control to the Value property so that we
        // can reload the text from it.
        TNumber result;
        if (_controllingProperty == ControllingProperty.Text 
          && _culture != null 
          && Parse(Text, false, out result))
        {
          _value = result;
          _controllingProperty = ControllingProperty.Value;
        }

        _culture = value;

        UpdateWholeNumberPortionRegex();

        if (_controllingProperty == ControllingProperty.Value)
        {
          RefreshTextFromValue();
        }
      }
    }

    private void UpdateWholeNumberPortionRegex()
    {
      string regexSafeSeparator = Regex.Escape(GroupSeparator);
      string digitOrSeparatorSpan = StringUtils.FormatInvariant(@"(\d|{0})+", regexSafeSeparator);
      _wholeNumberPortionRegex = new Regex(digitOrSeparatorSpan);
    }

    #endregion

    #region Data model

    private TNumber _value;
    private string _text;
    private bool _showSeparators = true;

    public TNumber Value
    {
      get { return _value; }
      set
      {
        if (_minimum.CompareTo(value) > 0 && !Double.NaN.Equals(value))
        {
          _value = _minimum;
        }
        else if (_maximum.CompareTo(value) < 0 && !Double.NaN.Equals(value))
        {
          _value = _maximum;
        }
        else
        {
          _value = value;
        }
        if (_value.CompareTo(Zero) != 0)
        {
          _hasValue = true;
        }
        _controllingProperty = ControllingProperty.Value;
        RefreshTextFromValue();
      }
    }

    internal protected abstract TNumber Zero { get; }

    internal protected virtual TNumber NoValue
    {
      get { return Zero; }
    }

    public string Text
    {
      get
      {
        return _text;
      }
      set
      {
        if (value != _text)
        {
          _text = value;
          if (_text == null)
          {
            _text = String.Empty;
          }
          _hasValue = !(AllowsNoValue && _text.Length == 0);
          
          _controllingProperty = ControllingProperty.Text;
          RefreshValueFromText();
        }
      }
    }

    public bool ShowSeparators
    {
      get { return _showSeparators; }
      set
      {
        _showSeparators = value;
        if (_controllingProperty == ControllingProperty.Value)
        {
          RefreshTextFromValue();
        }
        else
        {
          RationaliseGroupSeparators(_showSeparators);
        }
      }
    }

    private TNumber _minimum;
    private TNumber _maximum;

    public TNumber Minimum
    {
      get { return _minimum; }
      set
      {
        _minimum = value;
        CoerceMaximum();
        if (_controllingProperty == ControllingProperty.Value)
        {
          CoerceValue();
        }
      }
    }

    private void CoerceMinimum()
    {
      if (_minimum.CompareTo(_maximum) > 0)
      {
        _minimum = _maximum;
      }
    }

    private void CoerceMaximum()
    {
      if (_maximum.CompareTo(_minimum) < 0)
      {
        _maximum = _minimum;
      }
    }

    public TNumber Maximum
    {
      get { return _maximum; }
      set
      {
        _maximum = value;
        CoerceMinimum();
        if (_controllingProperty == ControllingProperty.Value)
        {
          CoerceValue();
        }
      }
    }

    private void CoerceValue()
    {
      Debug.Assert(_controllingProperty == ControllingProperty.Value);

      if (_value.CompareTo(_minimum) < 0 && !Double.NaN.Equals(Value))
      {
        Value = Minimum;
      }
      if (Value.CompareTo(Maximum) > 0 && !Double.NaN.Equals(Value))
      {
        Value = Maximum;
      }
    }

    private bool _isRangeCheckingSuspended;

    public void SuspendRangeChecking()
    {
      _isRangeCheckingSuspended = true;
    }

    public void ResumeRangeChecking()
    {
      _isRangeCheckingSuspended = false;
      if (!IsInRange(Value))
      {
        _controllingProperty = ControllingProperty.Value;
        CoerceValue();
        RefreshTextFromValue();
      }
    }

    public void CoerceDefault()
    {
      _value = default(TNumber);
      _controllingProperty = ControllingProperty.Value;
      CoerceValue();
      RefreshTextFromValue();
    }

    private bool _allowsNoValue = false;
    private bool _hasValue = true;

    public bool AllowsNoValue
    {
      get { return _allowsNoValue; }
      set
      {
        _allowsNoValue = value;

      }
    }

    public bool HasValue
    {
      get { return _hasValue; }
      set
      {
        if (_hasValue != value)
        {
          _hasValue = value;
          RefreshTextFromValue();
        }
      }
    }

    #region Synchronisation between data members

    private ControllingProperty _controllingProperty = ControllingProperty.Value;

    private enum ControllingProperty
    {
      Value,
      Text
    }

    protected bool ValueIsControllingProperty
    {
      get { return _controllingProperty == ControllingProperty.Value; }
    }

    internal void RefreshTextFromValue()
    {
      //Debug.Assert(_controllingProperty == ControllingProperty.Value);
      ForceRefreshTextFromValue();
    }

    internal void ForceRefreshTextFromValue()
    {
      if (!HasValue && AllowsNoValue)
      {
        _text = "";
        _value = NoValue;
        return;
      }
      _text = Value.ToString(FormatString, Culture);

      if (!_showSeparators)
      {
        _text = _text.Replace(GroupSeparator, String.Empty);
      }
    }

    internal void RefreshTextFromValueIgnoreDecimalPlaces()
    {
      if (!HasValue && AllowsNoValue)
      {
        _text = "";
        _value = NoValue;
        return;
      }
      //Debug.Assert(_controllingProperty == ControllingProperty.Value);
      _text = Value.ToString("#,0.####################", Culture);

      if (!_showSeparators)
      {
        _text = _text.Replace(GroupSeparator, String.Empty);
      }
    }

    protected void RefreshValueFromText()
    {
      Debug.Assert(_controllingProperty == ControllingProperty.Text);
      TNumber result;
      if (Parse(_text, false, out result))
      {
        _value = result;
      }
      if (!_hasValue && AllowsNoValue)
      {
        _value = NoValue;
      }
    }

    #endregion

    private bool Parse(string text, bool allowInterimParenthesis, out TNumber result)
    {
      if (String.IsNullOrEmpty(text))
      {
        result = default(TNumber);
        return true;
      }

      // Special case for when the user is just typing prefix characters
      bool containsDigits = false;
      foreach (char ch in text)
      {
        if (Char.IsDigit(ch))
        {
          containsDigits = true;
          break;
        }
      }

      if (allowInterimParenthesis && !containsDigits)
      {
        bool couldBecomeValid = TryParse(text + default(TNumber).ToString(FormatStringNoDecimalPlaces, Culture), out result);
        return couldBecomeValid;
      }

      if (TryParse(text, out result))
      {
        return IsInRange(result);
      }

      Match wholeNumberPortionMatch = _wholeNumberPortionRegex.Match(text);
      if (wholeNumberPortionMatch.Success)
      {
        string toLeftOfWholeNumberPortion = text.Substring(0, wholeNumberPortionMatch.Index);
        string wholeNumberPortion = text.Substring(wholeNumberPortionMatch.Index, wholeNumberPortionMatch.Length);
        string toRightOfWholeNumberPortion = text.Substring(wholeNumberPortionMatch.Index + wholeNumberPortionMatch.Length);

        wholeNumberPortion = wholeNumberPortion.Replace(GroupSeparator, String.Empty);
        text = toLeftOfWholeNumberPortion + wholeNumberPortion + toRightOfWholeNumberPortion;

        if (TryParse(text, out result))
        {
          return IsInRange(result);
        }
      }

      if (allowInterimParenthesis)
      {
        string stripSingleParenthesis = StripSingleParenthesis(text);
        return (TryParse(stripSingleParenthesis, out result) && IsInRange(result));
      }
      else
      {
        return false;
      }
    }

    private bool IsInRange(TNumber value)
    {
      if (_isRangeCheckingSuspended)
      {
        return true;
      }
      return _minimum.CompareTo(value) <= 0 && _maximum.CompareTo(value) >= 0;
    }

    internal abstract bool TryParse(string text, out TNumber result);

    private static string StripSingleParenthesis(string text)
    {
      Debug.Assert(text != null);

      char first = text[0];
      char last = text[text.Length - 1];
      if (first == '(' && last != ')')
      {
        return text.Substring(1);
      }
      else if (first != '(' && last == ')')
      {
        return text.Substring(0, text.Length - 1);
      }
      else
      {
        return text;
      }
    }

    #endregion

    #region Text edit querying

    #region Entry point

    internal bool CanInsert(IPlainTextPresenter where, string input)
    {
      if (input.Contains("\r"))
      {
        return false;
      }

      if (where.SelectionLength == 0)
      {
        return CanInsert(input, where.CaretPosition);
      }
      else
      {
        return CanReplace(input, where.SelectionStart, where.SelectionLength);
      }
    }

    internal bool CanDelete(IPlainTextPresenter where, DeleteDirection deleteDirection)
    {
      if (where.SelectionLength == 0)
      {
        int deletePos = where.CaretPosition;
        if (deleteDirection == DeleteDirection.Backward)
        {
          deletePos = where.CaretPosition - 1;
        }
        return CanReplace(String.Empty, deletePos, 1);
      }
      else
      {
        return CanReplace(String.Empty, where.SelectionStart, where.SelectionLength);
      }
    }

    #endregion

    #region Implementation

    private bool CanInsert(string input, int position)
    {
      try
      {
        string candidateText = Text.Insert(position, input);
        return IsValid(candidateText);
      }
      catch (ArgumentOutOfRangeException)
      {
        return false;
      }
    }

    private bool CanReplace(string input, int replacementStart, int replacementLength)
    {
      try
      {
        string interimText = Text.Remove(replacementStart, replacementLength);
        string candidateText = interimText.Insert(replacementStart, input);
        return IsValid(candidateText);
      }
      catch (ArgumentOutOfRangeException)
      {
        // This can happen if we have an empty string and the user presses a delete
        // key.  It's benign.
        return false;
      }
    }

    private bool IsValid(string candidateText)
    {
      return IsValid(candidateText, true);
    }

    private bool IsValid(string text, bool allowInterimParenthesis)
    {
      TNumber result;
      return Parse(text, allowInterimParenthesis, out result);
    }

    #endregion

    #endregion

    #region Text editing model

    #region Entry points

    internal int DeleteText(IPlainTextPresenter where, DeleteDirection deleteDirection)
    {
      if (where.SelectionLength == 0)
      {
        return DeleteTextAtCaret(where.CaretPosition, deleteDirection);
      }
      else
      {
        return DeleteTextRange(where.SelectionStart, where.SelectionLength);
      }
    }

    internal int InsertText(IPlainTextPresenter where, string text)
    {
      Invariant.ArgumentNotNull(text, "text");

      if (where.SelectionLength == 0)
      {
        return Insert(text, where.CaretPosition);
      }
      else
      {
        return Replace(text, where.SelectionStart, where.SelectionLength);
      }
    }

    #endregion

    #region Implementation

    private int DeleteTextRange(int start, int length)
    {
      return Replace(String.Empty, start, length);
    }

    private int DeleteTextAtCaret(int caretPosition, DeleteDirection deleteDirection)
    {
      if (caretPosition == 0 && deleteDirection == DeleteDirection.Backward)
      {
        return 0;
      }
      if (caretPosition == Text.Length && deleteDirection == DeleteDirection.Forward)
      {
        return caretPosition;
      }

      int delta = -1;
      if (deleteDirection == DeleteDirection.Forward)
      {
        delta = 0;
      }

      string textBefore = Text;
      int newPosition = Replace(String.Empty, caretPosition + delta, 1);
      string textAfter = Text;

      // If the user deleted a group separator and we restored it, then
      // advance across it.  (TODO: In this case perhaps we should probably
      // delete the separator rather than autoformat it back in.)
      if (textBefore == textAfter && deleteDirection == DeleteDirection.Forward)
      {
        ++newPosition;
      }

      return newPosition;
    }

    private int Insert(string text, int caretPosition)
    {
      return Replace(text, caretPosition, 0);
    }

    private int Replace(string text, int start, int length)
    {
      Debug.Assert(text != null);

      bool previouslyHadSeparators = TextHasGroupSeparators;

      int expectedTextLengthChange = text.Length - length;
      int expectedNewCaretPosition = start + text.Length;

      int lengthBefore = Text.Length;
      string textBefore = Text;

      Text = Text.Remove(start, length).Insert(start, text);
      RationaliseGroupSeparators(previouslyHadSeparators || _showSeparators);

      int lengthAfter = Text.Length;
      string textAfter = Text;

      int actualTextLengthChange = lengthAfter - lengthBefore;
      int caretPositionCorrection = actualTextLengthChange - expectedTextLengthChange;
      int actualNewCaretPosition = expectedNewCaretPosition + caretPositionCorrection;

      int testPosition = SafeCaretPosition(expectedNewCaretPosition);

      if (TextUtils.AreSameUpTo(textBefore, textAfter, testPosition))
      {
        return expectedNewCaretPosition;
      }

      return SafeCaretPosition(actualNewCaretPosition);
    }

    #endregion

    #region Helpers

    private int SafeCaretPosition(int candidatePosition)
    {
      return Math.Min(Text.Length, candidatePosition);
    }

    #endregion

    #endregion

    #region Formatting and display

    internal void RationaliseGroupSeparators(bool force)
    {
      if ((force || TextHasGroupSeparators) && IsValid(Text, false) && NumericalUtils.ConvertToDouble(Value) >= 1)
      {
        if (_showSeparators)
        {
          Match wholeNumberPortion = _wholeNumberPortionRegex.Match(Text);

          if (wholeNumberPortion.Success)
          {
            TNumber wholeNumberValue;
            if (Parse(wholeNumberPortion.Value, false, out wholeNumberValue))
            {
              string wholeNumberAsFullCurrency = wholeNumberValue.ToString(FormatStringNoDecimalPlaces, Culture);
              string wholeNumberAmountOnly = _wholeNumberPortionRegex.Match(wholeNumberAsFullCurrency).Value;

              int start = wholeNumberPortion.Index;
              int length = wholeNumberPortion.Length;
              Text = Text.Remove(start, length).Insert(start, wholeNumberAmountOnly);
            }
          }
        }
        else
        {
          Text = Text.Replace(GroupSeparator, String.Empty);
        }
      }
    }

    internal abstract string GroupSeparator { get; }

    private bool TextHasGroupSeparators
    {
      get
      {
        return Text.Contains(GroupSeparator);
      }
    }

    #endregion
  }
}
