using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Abstract base class for text boxes editing numeric values.
  /// </summary>
  /// <typeparam name="TNumber">The type of numeric value.</typeparam>
  [TemplatePart(Name = TextBoxPartName, Type = typeof(TextBox))]
  public abstract class NumericTextBoxBase<TNumber> : FilteringTextBoxBase
    where TNumber : IFormattable, IComparable<TNumber>
  {
    private const string TextBoxPartName = "PART_TextBox";

    /// <summary>
    /// Initializes a new instance of the <see cref="NumericTextBoxBase{TNumber}"/> class.
    /// </summary>
    protected NumericTextBoxBase()
    {
      _undoManager = new UndoManager<PlainTextUndoInfo>(OnUndo);
      BindCommands();
      NumericModel.SuspendRangeChecking();
    }

    #region Initialisation

    private void BindCommands()
    {
      BindCommand(ApplicationCommands.Paste, CanExecutePaste, ExecutePaste);
      // No special handling required for Cut or Copy; pass through to text box for handling
    }

    private void BindCommand(ICommand command,
      CanExecuteRoutedEventHandler canExecuteHandler, ExecutedRoutedEventHandler handler)
    {
      CommandBinding binding = new CommandBinding(command);
      binding.PreviewCanExecute += canExecuteHandler;
      binding.PreviewExecuted += handler;
      binding.Executed += handler;
      CommandBindings.Add(binding);
    }

    #endregion

    /// <summary>
    /// Indicates that the initialization process for the element is complete.
    /// </summary>
    public override void EndInit()
    {
      base.EndInit();
      UpdateIfInitialised();
      NumericModel.ResumeRangeChecking();
    }

    internal override IUndoManager UndoManager
    {
      get { return _undoManager; }
    }

    internal override IFilteringTextBoxModel Model
    {
      get { return NumericModel; }
    }

    internal abstract NumericTextBoxBaseModel<TNumber> NumericModel { get; }

    /// <summary>
    /// Gets or sets the numeric value.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="ValueProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>BindsTwoWayByDefault</td></tr>
    /// </table>
    /// </remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Justification = "By analogy with RangeBase")]
    public TNumber Value
    {
      get { return (TNumber)GetValue(ValueProperty); }
      set { SetValue(ValueProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Value"/> property.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", Justification = "Dependency property not usually referenced directly")]
    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register("Value", typeof(TNumber), typeof(NumericTextBoxBase<TNumber>),
        new FrameworkPropertyMetadata(
          default(TNumber),
          FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
          OnValueChanged));


    /// <summary>
    /// Gets or sets whether to show group separators.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="ShowSeparatorsProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool ShowSeparators
    {
      get { return (bool)GetValue(ShowSeparatorsProperty); }
      set { SetValue(ShowSeparatorsProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ShowSeparators"/> property.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", Justification="Dependency property not usually referenced directly")]
    public static readonly DependencyProperty ShowSeparatorsProperty =
        DependencyProperty.Register("ShowSeparators", typeof(bool), typeof(NumericTextBoxBase<TNumber>),
        new FrameworkPropertyMetadata(true, OnShowSeparatorsChanged));


    /// <summary>
    /// Gets or sets the minimum value.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="MinimumProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public TNumber Minimum
    {
      get { return (TNumber)GetValue(MinimumProperty); }
      set { SetValue(MinimumProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Minimum"/> property.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", Justification = "Dependency property not usually referenced directly")]
    public static readonly DependencyProperty MinimumProperty =
        DependencyProperty.Register("Minimum", typeof(TNumber), typeof(NumericTextBoxBase<TNumber>), 
        new FrameworkPropertyMetadata(OnMinimumChanged));

    /// <summary>
    /// Gets or sets the maximum value.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="MaximumProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public TNumber Maximum
    {
      get { return (TNumber)GetValue(MaximumProperty); }
      set { SetValue(MaximumProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Maximum"/> property.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", Justification = "Dependency property not usually referenced directly")]
    public static readonly DependencyProperty MaximumProperty =
        DependencyProperty.Register("Maximum", typeof(TNumber), typeof(NumericTextBoxBase<TNumber>), 
        new FrameworkPropertyMetadata(OnMaximumChanged));


    /// <summary>
    /// Gets or sets when the control enforces range (minimum and maximum) constraints.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="RangeConstraintModeProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public RangeConstraintMode RangeConstraintMode
    {
      get { return (RangeConstraintMode)GetValue(RangeConstraintModeProperty); }
      set { SetValue(RangeConstraintModeProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="RangeConstraintMode"/> property.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", Justification = "Dependency property not usually referenced directly")]
    public static readonly DependencyProperty RangeConstraintModeProperty =
      DependencyProperty.Register("RangeConstraintMode", typeof(RangeConstraintMode), typeof(NumericTextBoxBase<TNumber>),
      new FrameworkPropertyMetadata(RangeConstraintMode.Always));

    /// <summary>
    /// Gets or sets the whether the control should select all text when it gains focus.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="SelectAllOnEntryProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool SelectAllOnEntry
    {
      get { return (bool)GetValue(SelectAllOnEntryProperty); }
      set { SetValue(SelectAllOnEntryProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="SelectAllOnEntry"/> property.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", Justification = "Dependency property not usually referenced directly")]
    public static readonly DependencyProperty SelectAllOnEntryProperty =
        DependencyProperty.Register("SelectAllOnEntry", typeof(bool), typeof(NumericTextBoxBase<TNumber>), 
        new FrameworkPropertyMetadata(true));

    private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((NumericTextBoxBase<TNumber>)d).OnValueChanged((TNumber)(e.OldValue));
    }

    private static void OnShowSeparatorsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((NumericTextBoxBase<TNumber>)d).OnShowSeparatorsChanged();
    }

    private static void OnMinimumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((NumericTextBoxBase<TNumber>)d).OnMinimumChanged();
    }

    private static void OnMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((NumericTextBoxBase<TNumber>)d).OnMaximumChanged();
    }

    private void OnValueChanged(TNumber oldValue)
    {
      int caretPosition = 0;
      if (_display != null)
      {
        caretPosition = _display.CaretPosition;
      }

      if (!_inInternalValueUpdate)
      {
        NumericModel.Value = Value;
      }
      Text = Model.Text;
      HasValue = NumericModel.HasValue;
      UpdateDisplay();

      if (_display != null)
      {
        _display.CaretPosition = caretPosition;
      }

      RaiseEvent(new RoutedPropertyChangedEventArgs<TNumber>(oldValue, Value, ValueChangedEvent));
    }

    internal void UpdateIfInitialised()
    {
      if (IsInitialized)
      {
        Text = Model.Text;
        UpdateDisplay();
      }
    }

    private void OnShowSeparatorsChanged()
    {
      NumericModel.ShowSeparators = ShowSeparators;
      UpdateIfInitialised();
    }

    private void OnMinimumChanged()
    {
      NumericModel.Minimum = Minimum;
      UpdateIfInitialised();
    }

    private void OnMaximumChanged()
    {
      NumericModel.Maximum = Maximum;
      UpdateIfInitialised();
    }

    /// <summary>
    /// Identifies the <see cref="ValueChanged"/> routed event.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", Justification = "Routed event not usually referenced directly")]
    public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent("ValueChanged",
      RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<TNumber>), typeof(NumericTextBoxBase<TNumber>));

    /// <summary>
    /// Occurs when the Value changes.
    /// </summary>
    public event RoutedPropertyChangedEventHandler<TNumber> ValueChanged
    {
      add { AddHandler(ValueChangedEvent, value); }
      remove { RemoveHandler(ValueChangedEvent, value); }
    }

    /// <summary>
    /// Gets or sets whether the text box is read-only.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsReadOnlyProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool IsReadOnly
    {
      get { return (bool)GetValue(IsReadOnlyProperty); }
      set { SetValue(IsReadOnlyProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="IsReadOnly"/> property.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
    public static readonly DependencyProperty IsReadOnlyProperty =
      DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(NumericTextBoxBase<TNumber>),
      new FrameworkPropertyMetadata(false));

    #region Low-level text handling overrides

    /// <summary>
    /// Overrides default handling for the PreviewTextInput event.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnPreviewTextInput(TextCompositionEventArgs e)
    {
      base.OnPreviewTextInput(e);

      if (IsReadOnly)
      {
        e.Handled = true;
        return;
      }

      if (e.Text.Contains("\r"))
      {
        e.Handled = true;
        return;
      }

      InsertText(e.Text);
      e.Handled = true;
    }

    /// <summary>
    /// Overrides default handling for the PreviewKeyDown event.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnPreviewKeyDown(KeyEventArgs e)
    {
      base.OnPreviewKeyDown(e);

      if (IsReadOnly)
      {
        e.Handled = true;
        return;
      }

      if (e.Key == Key.Space)
      {
        e.Handled = true;
      }
      else if (e.Key == Key.Return)
      {
        if (RangeConstraintMode == RangeConstraintMode.OnLostFocusOrReturn)
        {
          NumericModel.ResumeRangeChecking();
          OnRangeCheck();
          LoadPostTextOperationState();
          NumericModel.SuspendRangeChecking();
          e.Handled = true;
        }
      }
      else if (e.Key == Key.Back)
      {
        DeleteText(DeleteDirection.Backward);
        e.Handled = true;
      }
      else if (e.Key == Key.Delete)
      {
        DeleteText(DeleteDirection.Forward);
        e.Handled = true;
      }
    }

    internal virtual void OnRangeCheck()
    {
    }

    private void InsertText(string text)
    {
      Debug.Assert(!IsReadOnly);

      if (NumericModel.CanInsert(_display, text))
      {
        int newPos = NumericModel.InsertText(_display, text);
        LoadPostTextOperationState(newPos);
      }
    }

    private void DeleteText(DeleteDirection deleteDirection)
    {
      Debug.Assert(!IsReadOnly);

      if (NumericModel.CanDelete(_display, deleteDirection))
      {
        int newPos = NumericModel.DeleteText(_display, deleteDirection);
        LoadPostTextOperationState(newPos);
      }
    }

    private bool _inInternalValueUpdate;

    private void LoadPostTextOperationState(int newCaretPosition)
    {
      LoadPostTextOperationState();
      _display.CaretPosition = newCaretPosition;
    }

    internal void LoadPostTextOperationState()
    {
      try
      {
        _inInternalValueUpdate = true;
        if (!EqualityComparer<TNumber>.Default.Equals(Value, NumericModel.Value))
        {
          Value = NumericModel.Value;
        }
      }
      finally
      {
        _inInternalValueUpdate = false;
      }
      if (Text != Model.Text)
      {
        Text = Model.Text;
      }
      HasValue = NumericModel.HasValue;
    }

    internal override void AfterTextChanged()
    {
      LoadPostTextOperationState();
    }

    #endregion

    #region Focus handling

    private bool SuspendRangeCheckingWhileFocused
    {
      get
      {
        return RangeConstraintMode == RangeConstraintMode.OnLostFocus || RangeConstraintMode == RangeConstraintMode.OnLostFocusOrReturn;
      }
    }

    /// <summary>
    /// Adds class handling for the <see cref="UIElement.GotKeyboardFocus"/> event.
    /// </summary>
    /// <param name="e">Provides data about the event.</param>
    protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
    {
      if (_displayControl != null)
      {
        _displayControl.Focus();
        if (SelectAllOnEntry)
        {
          _displayControl.SelectAll();
        }
      }
      if (SuspendRangeCheckingWhileFocused)
      {
        NumericModel.SuspendRangeChecking();
      }

      base.OnGotKeyboardFocus(e);
    }

    /// <summary>
    /// Adds class handling for the <see cref="UIElement.LostKeyboardFocus"/> event.
    /// </summary>
    /// <param name="e">Provides data about the event.</param>
    protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
    {
      bool performedTextOperation = OnEndUserInteraction();

      if (performedTextOperation)
      {
        LoadPostTextOperationState();
      }

      base.OnLostKeyboardFocus(e);
    }

    internal virtual bool OnEndUserInteraction()
    {
      bool performedTextOperation = false;

      if (SuspendRangeCheckingWhileFocused)
      {
        NumericModel.ResumeRangeChecking();
        performedTextOperation = true;
      }
      else if (String.IsNullOrEmpty(Text))
      {
        NumericModel.CoerceDefault();
        performedTextOperation = true;
      }

      return performedTextOperation;
    }

    /// <summary>
    /// Adds class handling for the <see cref="UIElement.PreviewMouseLeftButtonDown"/> event.
    /// </summary>
    /// <param name="e">Provides data about the event.</param>
    protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
    {
      if (SelectAllOnEntry && _displayControl != null 
        && _displayControl.Focusable && !_displayControl.IsFocused)
      {
        _displayControl.Focus();

        e.Handled = true;
      }

      base.OnPreviewMouseLeftButtonDown(e);
    }

    /// <summary>
    /// Called when the left mouse button is released over the numeric text box.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
    {
      base.OnPreviewMouseLeftButtonUp(e);
      if (_displayControl != null)
      {
        _displayControl.ReleaseMouseCapture();
      }
      e.Handled = true;
    }

    #endregion

    #region Display

    internal override void UpdateDisplay()
    {
      if (_displayControl != null)
      {
        _displayControl.Text = Model.Text;
      }
    }

    private TextBox _displayControl;
    private IPlainTextPresenter _display = NullPlainTextPresenter.Instance;

    internal override void InitialiseDisplayMembers()
    {
      _displayControl = GetTemplateChild(TextBoxPartName) as TextBox;
      _display = (_displayControl == null ?
        NullPlainTextPresenter.Instance :
        new TextBoxTextPresenter(_displayControl));
    }

    #endregion

    #region Undo support

    private readonly UndoManager<PlainTextUndoInfo> _undoManager;

    internal override void PushUndo(string oldText)
    {
      PlainTextUndoInfo undoInfo = PlainTextUndoInfo.Create(oldText, _display);
      _undoManager.PushUndo(undoInfo);
    }

    private void OnUndo(PlainTextUndoInfo undoInfo)
    {
      Text = undoInfo.Text;
      undoInfo.SetCursorPosition(_display);
      LoadPostTextOperationState();
    }

    #endregion
    
    #region Edit commands (cut and paste) support

    private void CanExecutePaste(object sender, CanExecuteRoutedEventArgs e)
    {
      bool isTextOnClipboard = Clipboard.ContainsText(TextDataFormat.Text) || Clipboard.ContainsText(TextDataFormat.UnicodeText);
      bool gotUsableText = false;
      if (isTextOnClipboard)
      {
        string text = Clipboard.GetText(TextDataFormat.Text);
        gotUsableText = NumericModel.CanInsert(_display, text);
      }

      bool gotPasteTarget = (_displayControl != null);

      e.CanExecute = gotUsableText && gotPasteTarget && !IsReadOnly;
      e.Handled = true;
    }

    private void ExecutePaste(object sender, ExecutedRoutedEventArgs e)
    {
      Debug.Assert(!IsReadOnly);

      string text = Clipboard.GetText(TextDataFormat.Text);
      if (String.IsNullOrEmpty(text))
      {
        text = Clipboard.GetText(TextDataFormat.UnicodeText);
      }

      InsertText(text);
      e.Handled = true;
    }

    #endregion

    #region AllowsNoValue Property

    /// <summary>
    /// Gets or sets whther or not the numeric text box allows the value to be deleted entirely, rather than defaulting to 0.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="AllowsNoValueProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool AllowsNoValue
    {
      get { return (bool)GetValue(AllowsNoValueProperty); }
      set { SetValue(AllowsNoValueProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="AllowsNoValue"/> property.
    /// </summary>
    public static readonly DependencyProperty AllowsNoValueProperty =
      DependencyProperty.Register("AllowsNoValue", typeof(bool), typeof(NumericTextBoxBase<TNumber>),
      new FrameworkPropertyMetadata(OnAllowsNoValueChanged));

    private static void OnAllowsNoValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((NumericTextBoxBase<TNumber>)d).OnAllowsNoValueChanged();
    }

    private void OnAllowsNoValueChanged()
    {
      NumericModel.AllowsNoValue = AllowsNoValue;
    }

    #endregion // AllowsNoValue Property

    #region HasValue Property

    /// <summary>
    /// Gets or sets whether or not the numeric text box has a value. This only has use if the Value is zero.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="HasValueProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool HasValue
    {
      get { return (bool)GetValue(HasValueProperty); }
      set { SetValue(HasValueProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="HasValue"/> property.
    /// </summary>
    public static readonly DependencyProperty HasValueProperty =
      DependencyProperty.Register("HasValue", typeof(bool), typeof(NumericTextBoxBase<TNumber>),
      new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnHasValueChanged));

    private static void OnHasValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((NumericTextBoxBase<TNumber>)d).OnHasValueChanged();
    }

    private void OnHasValueChanged()
    {
      NumericModel.HasValue = HasValue;
    }

    #endregion // HasValue Property

    /// <summary>
    /// Invoked when the parent element of this UIElement reports a change to its underlying visual parent.
    /// </summary>
    /// <param name="oldParent">The previous parent.</param>
    protected override void OnVisualParentChanged(DependencyObject oldParent)
    {
      // We have an issue in the NumericUpDown where it seems to instantiate an IntegerTextBox
      // even if the control template tells it not to -- and then detaches that IntegerTextBox
      // and for some reason zeroes out the value of that IntegerTextBox, causing range violations.
      // We work around this by suspending range checking when the IntegerTextBox is removed from
      // the visual tree.
      if (oldParent != null && Parent == null)
      {
        NumericModel.SuspendRangeChecking();
      }
      if (oldParent == null && Parent != null)
      {
        NumericModel.ResumeRangeChecking();
      }
      base.OnVisualParentChanged(oldParent);
    }
  }
}
