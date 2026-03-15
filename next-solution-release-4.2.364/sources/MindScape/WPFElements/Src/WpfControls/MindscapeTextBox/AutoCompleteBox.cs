using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Diagnostics;
using System.ComponentModel;
using Infralution.Licensing;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Represents a control that provides a text box for user input and 
  /// a drop-down that contains possible matches based on the input in the text box.
  /// </summary>
  public class AutoCompleteBox : TextBox
  {
    private bool _updateSuggestions;
    private ListBox _listBox;
    private String _lastUserText;
    private int _lastUserCaretIndex;
    private IAutoCompleteSuggestionProvider _suggestionProvider = new AutoCompleteNullSuggestionProvider();
    private int _previousSelectionStart = 0;

    static AutoCompleteBox()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(AutoCompleteBox),
        new FrameworkPropertyMetadata(typeof(AutoCompleteBox)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AutoCompleteBox"/> class.
    /// </summary>
    public AutoCompleteBox()
    {
      _updateSuggestions = true;
      SetValue(MatchingSuggestionsPropertyKey, new InputSuggestionCollection());

      AddHandler(TextBox.TextInputEvent, new TextCompositionEventHandler(AutoCompleteBox_TextInput), true);
      SelectionChanged += new RoutedEventHandler(AutoCompleteBox_SelectionChanged);
      PreviewKeyDown += new KeyEventHandler(AutoCompleteBox_PreviewKeyDown);
      LostFocus += new RoutedEventHandler(AutoCompleteBox_LostFocus);
      MouseDown += new MouseButtonEventHandler(AutoCompleteBox_MouseDown);
      PreviewMouseDown += new MouseButtonEventHandler(AutoCompleteBox_PreviewMouseDown);
      LostMouseCapture += new MouseEventHandler(AutoCompleteBox_LostMouseCapture);
    }

    private void AutoCompleteBox_TextInput(object sender, TextCompositionEventArgs e)
    {
      UpdateSuggestionList();
    }

    private void AutoCompleteBox_LostMouseCapture(object sender, MouseEventArgs e)
    {
      if (IsDropDownOpen && Mouse.LeftButton == MouseButtonState.Released)
      {
        Mouse.Capture(this, CaptureMode.SubTree);
      }
    }

    private void AutoCompleteBox_SelectionChanged(object sender, RoutedEventArgs e)
    {
      if (SelectionLength == 0 && _listBox != null && _listBox.SelectedItem == null && _previousSelectionStart != SelectionStart)
      {
        UpdateSuggestionList();
      }
      _previousSelectionStart = SelectionStart;
    }

    private void AutoCompleteBox_MouseDown(object sender, MouseButtonEventArgs e)
    {
      ForceValue();
      IsDropDownOpen = false;
      ReleaseMouseCapture();
    }

    private void AutoCompleteBox_LostFocus(object sender, RoutedEventArgs e)
    {
      ForceValue();
      IsDropDownOpen = false;
      ReleaseMouseCapture();
    }

    private void ForceValue()
    {
      if (IsSuggestionListExclusive)
      {
        if (_listBox.SelectedIndex == -1)
        {
          IEnumerable<string> suggestions = _suggestionProvider.GetSuggestions(Text, Int32.MaxValue);
          bool found = false;
          foreach(string str in suggestions)
          {
            found = true;
            ReplaceInputTextWithSuggestion(str);
            break;
          }
          if (!found)
          {
            ReplaceInputTextWithSuggestion("");
          }
        }
      }
    }

    private void AutoCompleteBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
      AutoCompleteBox control = (AutoCompleteBox)sender;
      
      if (Mouse.Captured == control && e.OriginalSource == control)
      {
        ForceValue();
        control.IsDropDownOpen = false;
      }
    }

    private void AutoCompleteBox_PreviewKeyDown(object sender, KeyEventArgs e)
    {
      if (IsDropDownOpen)
      {
        if (e.Key == Key.Down)
        {
          if (_listBox.SelectedItem == null)
          {
            _lastUserCaretIndex = CaretIndex;
          }
          int nextIndex = _listBox.SelectedIndex + 1;
          if (nextIndex >= _listBox.Items.Count)
          {
            _listBox.SelectedItem = null;
            Text = _lastUserText;
            KeepCaretAtEnd();
          }
          else
          {
            _listBox.SelectedIndex = nextIndex;
            _listBox.ScrollIntoView(_listBox.SelectedItem);
          }
        }
        if (e.Key == Key.Up)
        {
          if (_listBox.SelectedItem == null)
          {
            _listBox.SelectedIndex = _listBox.Items.Count - 1;
            _lastUserCaretIndex = CaretIndex;
            _listBox.ScrollIntoView(_listBox.SelectedItem);
          }
          else
          {
            int prevIndex = _listBox.SelectedIndex - 1;
            if (prevIndex < 0)
            {
              _listBox.SelectedItem = null;
              Text = _lastUserText;
              KeepCaretAtEnd();
            }
            else
            {
              _listBox.SelectedIndex = prevIndex;
              _listBox.ScrollIntoView(_listBox.SelectedItem);
            }
          }
        }
        if (e.Key == Key.Enter)
        {
          ForceValue();
          IsDropDownOpen = false;
          e.Handled = true;
        }
        if (e.Key == Key.Tab || e.Key == Key.LeftAlt || e.Key == Key.RightAlt || e.Key == Key.LWin || e.Key == Key.RWin || e.SystemKey == Key.LeftAlt || e.SystemKey == Key.RightAlt)
        {
          IsDropDownOpen = false;
        }
      }
      else if (e.Key == Key.Down && Text.Length == 0)
      {
        ShowAllSuggestions();
      }
    }

    /// <summary>
    /// Displays all the suggestions in the drop down up to the number defined by the MaxSuggestionCount property.
    /// </summary>
    public void ShowAllSuggestions()
    {
      if (!IsDropDownOpen)
      {
        IsDropDownOpen = true;
        IList<InputSuggestion> currentSuggestions = new List<InputSuggestion>();
        foreach (string s in _suggestionProvider.GetSuggestions("", Int32.MaxValue))
        {
          currentSuggestions.Add(new InputSuggestion(Text, s));
          if (currentSuggestions.Count >= MaxSuggestionCount)
          {
            break;
          }
        }
        ResetMatchingSuggestions(currentSuggestions);
      }
    }

    private void KeepCaretAtEnd()
    {
      if (MultiInputDelimiters != null)
      {
        CaretIndex = _lastUserCaretIndex;
      }
      else
      {
        SelectionStart = Text.Length;
      }
      SelectionLength = 0;
    }

    /// <summary>
    /// Called when the mouse is double clicked over the <see cref="AutoCompleteBox"/>.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
    {
      base.OnMouseDoubleClick(e);

      if (Text.Length == 0)
      {
        ShowAllSuggestions();
      }
    }

    /// <summary>
    /// Called by the framework when ApplyTemplate is called.
    /// </summary>
    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();
      _listBox = GetTemplateChild("PART_ListBox") as ListBox;
      if (_listBox != null)
      {
        _listBox.SelectionChanged += new SelectionChangedEventHandler(ListBox_SelectionChanged);
      }
    }

    private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (_listBox.SelectedItem != null)
      {
        _updateSuggestions = false;
        //Text = _listBox.SelectedItem.ToString();
        ReplaceInputTextWithSuggestion(_listBox.SelectedItem.ToString());
        _updateSuggestions = true;
      }
    }

    private void ReplaceInputTextWithSuggestion(string suggestion)
    {
      if (MultiInputDelimiters != null && Text.Length > 0)
      {
        string[] strings = Text.Split(MultiInputDelimiters);
        int index = 0;
        foreach (string str in strings)
        {
          if (str.Length > 0)
          {
            index += str.Length;
            if (index >= CaretIndex)
            {
              Text = Text.Substring(0, index - str.Length) + suggestion + Text.Substring(index, Text.Length - index);
              CaretIndex = index - str.Length + suggestion.Length;
              return;
            }
          }
          index++; // The length of a deliminator.
        }
      }
      else
      {
        Text = suggestion;
        KeepCaretAtEnd();
      }
    }

    private void UpdateSuggestionList()
    {
      if (_updateSuggestions)
      {
        _lastUserText = Text;

        IList<InputSuggestion> currentSuggestions = new List<InputSuggestion>();
        string input = GetCurrentInputText();
        if (input.Length >= MinimumPrefixLength && _suggestionProvider != null)
        {
          foreach (string s in _suggestionProvider.GetSuggestions(input, MaxSuggestionCount))
          {
            currentSuggestions.Add(new InputSuggestion(input, s));
            if (currentSuggestions.Count >= MaxSuggestionCount)
            {
              break;
            }
          }
        }

        ResetMatchingSuggestions(currentSuggestions);

        bool multipleMatches = MatchingSuggestions.Count > 1;
        bool singleIncompleteMatch = MatchingSuggestions.Count == 1 && !MatchingSuggestions[0].Suggestion.Equals(input, StringComparison.CurrentCultureIgnoreCase);

        IsDropDownOpen = multipleMatches || singleIncompleteMatch;
      }
    }

    private string GetCurrentInputText()
    {
      if(MultiInputDelimiters != null && MultiInputDelimiters.Length > 0)
      {
        string[] strings = Text.Split(MultiInputDelimiters);
        int index = 0;
        foreach (string str in strings)
        {
          if (str.Length > 0)
          {
            index += str.Length;
            if (index >= CaretIndex)
            {
              return str;
            }
          }
          index++; // The length of a deliminator.
        }
        if (strings.Length > 0)
        {
          return strings[strings.Length - 1];
        }
      }
      return Text;
    }

    #region MultiInputDelimiters Property

    /// <summary>
    /// Gets or sets the delimiters used to allow multiple individual entries to be added to the <see cref="AutoCompleteBox"/>.
    /// If this property is set to null, multiple entry input will be disabled. The default is null.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="MultiInputDelimitersProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    [TypeConverter(typeof(CharArrayTypeConverter))]
    public char[] MultiInputDelimiters
    {
      get { return (char[])GetValue(MultiInputDelimitersProperty); }
      set { SetValue(MultiInputDelimitersProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="MultiInputDelimiters"/> property.
    /// </summary>
    public static readonly DependencyProperty MultiInputDelimitersProperty =
      DependencyProperty.Register("MultiInputDelimiters", typeof(char[]), typeof(AutoCompleteBox),
      new FrameworkPropertyMetadata(OnMultiInputDelimitersChanged));

    private static void OnMultiInputDelimitersChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((AutoCompleteBox)d).OnMultiInputDelimitersChanged();
    }

    private void OnMultiInputDelimitersChanged()
    {
      //UpdateSuggestionList();
    }

    #endregion // MultiInputDelimiters Property

    /// <summary>
    /// Gets or sets a source for autocompletion suggestions.  This may be a collection
    /// of strings or an <see cref="IAutoCompleteSuggestionProvider"/>.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="SuggestionsSourceProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public object SuggestionsSource
    {
      get { return GetValue(SuggestionsSourceProperty); }
      set { SetValue(SuggestionsSourceProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="SuggestionsSource"/> property.
    /// </summary>
    public static readonly DependencyProperty SuggestionsSourceProperty =
      DependencyProperty.Register("SuggestionsSource", typeof(object), typeof(AutoCompleteBox),
      new FrameworkPropertyMetadata(AutoCompleteNullSuggestionProvider.Instance, OnSuggestionsSourceChanged),
      OnValidateSuggestionsSource);

    private static bool OnValidateSuggestionsSource(object value)
    {
      return value is IAutoCompleteSuggestionProvider
        || value is IEnumerable<string>;
    }

    private static void OnSuggestionsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((AutoCompleteBox)d).OnSuggestionsSourceChanged();
    }

    private void OnSuggestionsSourceChanged()
    {
      IAutoCompleteSuggestionProvider provider = SuggestionsSource as IAutoCompleteSuggestionProvider;
      if (provider != null)
      {
        _suggestionProvider = provider;
      }
      else
      {
        IEnumerable<string> list = SuggestionsSource as IEnumerable<string>;
        if (list != null)
        {
          _suggestionProvider = new AutoCompleteListSuggestionProvider(list);
        }
        else
        {
          _suggestionProvider = AutoCompleteNullSuggestionProvider.Instance;
        }
      }
    }

    private class InputSuggestionCollection : ReadOnlyObservableCollection<InputSuggestion>
    {
      internal InputSuggestionCollection()
        : base(new ObservableCollection<InputSuggestion>())
      {
      }

      internal void Reset(IList<InputSuggestion> suggestions)
      {
        Items.Clear();
        foreach (var s in suggestions)
        {
          Items.Add(s);
        }
      }
    }

    private void ResetMatchingSuggestions(IList<InputSuggestion> suggestions)
    {
      Debug.Assert(MatchingSuggestions is InputSuggestionCollection);

      ((InputSuggestionCollection)MatchingSuggestions).Reset(suggestions);
    }

    /// <summary>
    /// Gets the list of suggestions which match the current user input
    /// and should be displayed.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="MatchingSuggestionsProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public ReadOnlyObservableCollection<InputSuggestion> MatchingSuggestions
    {
      get { return (ReadOnlyObservableCollection<InputSuggestion>)GetValue(MatchingSuggestionsProperty); }
    }

    private static readonly DependencyPropertyKey MatchingSuggestionsPropertyKey =
        DependencyProperty.RegisterReadOnly("MatchingSuggestions", typeof(InputSuggestionCollection), typeof(AutoCompleteBox), new UIPropertyMetadata(null));

    /// <summary>
    /// Identifies the <see cref="MatchingSuggestions"/> property.
    /// </summary>
    public static readonly DependencyProperty MatchingSuggestionsProperty =
      MatchingSuggestionsPropertyKey.DependencyProperty;

    /// <summary>
    /// Gets or sets whether the suggestion drop-down is open.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsDropDownOpenProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool IsDropDownOpen
    {
      get { return (bool)GetValue(IsDropDownOpenProperty); }
      set { SetValue(IsDropDownOpenProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="IsDropDownOpen"/> property.
    /// </summary>
    public static readonly DependencyProperty IsDropDownOpenProperty =
      DependencyProperty.Register("IsDropDownOpen", typeof(bool), typeof(AutoCompleteBox),
      new FrameworkPropertyMetadata(OnIsDropDownOpenChanged));

    private static void OnIsDropDownOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((AutoCompleteBox)d).OnIsDropDownOpenChanged();
    }

    private void OnIsDropDownOpenChanged()
    {
      if (IsDropDownOpen)
      {
        Mouse.Capture(this, CaptureMode.SubTree);
        if (_listBox != null && _listBox.Items.Count > 0)
        {
          _listBox.ScrollIntoView(_listBox.Items[0]);
        }
      }
      else
      {
        Mouse.Capture(null);
      }
    }

    /// <summary>
    /// Gets or sets the minimum number of characters required to be entered 
    /// in the text box before the AutoCompleteBox displays possible matches.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="MinimumPrefixLengthProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public int MinimumPrefixLength
    {
      get { return (int)GetValue(MinimumPrefixLengthProperty); }
      set { SetValue(MinimumPrefixLengthProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="MinimumPrefixLength"/> property.
    /// </summary>
    public static readonly DependencyProperty MinimumPrefixLengthProperty =
      DependencyProperty.Register("MinimumPrefixLength", typeof(int), typeof(AutoCompleteBox),
      new FrameworkPropertyMetadata(1));

    /// <summary>
    /// Gets or sets the maximum number of autocomplete suggestions that will
    /// be displayed.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="MaxSuggestionCountProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public int MaxSuggestionCount
    {
      get { return (int)GetValue(MaxSuggestionCountProperty); }
      set { SetValue(MaxSuggestionCountProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="MaxSuggestionCount"/> property.
    /// </summary>
    public static readonly DependencyProperty MaxSuggestionCountProperty =
      DependencyProperty.Register("MaxSuggestionCount", typeof(int), typeof(AutoCompleteBox),
      new FrameworkPropertyMetadata(Int32.MaxValue));

    #region IsSuggestionListExclusive Property

    /// <summary>
    /// Gets or sets whether or not the user input must be one of the suggested items.
    /// If true, the value of the <see cref="AutoCompleteBox"/> will be set to the closest matching suggestion when it loses focus.
    /// The default is false.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsSuggestionListExclusiveProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool IsSuggestionListExclusive
    {
      get { return (bool)GetValue(IsSuggestionListExclusiveProperty); }
      set { SetValue(IsSuggestionListExclusiveProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="IsSuggestionListExclusive"/> property.
    /// </summary>
    public static readonly DependencyProperty IsSuggestionListExclusiveProperty =
      DependencyProperty.Register("IsSuggestionListExclusive", typeof(bool), typeof(AutoCompleteBox),
      new FrameworkPropertyMetadata(false, OnIsSuggestionListExclusiveChanged));

    private static void OnIsSuggestionListExclusiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((AutoCompleteBox)d).OnIsSuggestionListExclusiveChanged();
    }

    private void OnIsSuggestionListExclusiveChanged()
    {
    }

    #endregion // IsSuggestionListExclusive Property
  }
}
