using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Infralution.Licensing;
using System.Windows.Threading;
using System.Reflection;
using System.Diagnostics;
using System.Windows.Documents;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// A text box that uses a mask to prevent incorrect user input.
  /// </summary>
  [LicenseProvider(typeof(PublicEncryptedLicenseProvider))]
  [TemplatePart(Name = RichTextBoxPartName, Type = typeof(RichTextBox))]
  public class MaskedTextBox : FilteringTextBoxBase, IDisplayElementStyleProvider
  {
    private const string RichTextBoxPartName = "PART_RichTextBox";

    private readonly MaskedTextBoxModel _mt = new MaskedTextBoxModel();

    internal override IFilteringTextBoxModel Model
    {
      get { return _mt; }
    }

    internal override IUndoManager UndoManager
    {
      get { return _undoManager; }
    }

    static MaskedTextBox()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(MaskedTextBox), 
        new FrameworkPropertyMetadata(typeof(MaskedTextBox)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MaskedTextBox"/> class.
    /// </summary>
    public MaskedTextBox()
    {
      // Licensing
      //new WpfElementsCore(Assembly.GetCallingAssembly());
      //LicenseHelper.Attach(this, Assembly.GetCallingAssembly());
      // End licensing
      
      _undoManager = new UndoManager<MaskedTextUndoInfo>(OnUndo);

      _mt.AutoSkipLiterals = AutoSkipLiterals;

      BindCommands();
    }

    /// <summary>
    /// Called when this <see cref="MaskedTextBox"/> gets focus;
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnGotFocus(RoutedEventArgs e)
    {
      base.OnGotFocus(e);
      if (_displayControl != null)
      {
        _displayControl.Focus();
      }
    }

    #region Initialisation

    private void BindCommands()
    {
      BindCommand(ApplicationCommands.Paste, CanExecutePaste, ExecutePaste);
      BindCommand(ApplicationCommands.Cut, CanExecuteCut, ExecuteCut);
      // No special handling required for Copy; pass through to text box for handling
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
    /// Measures the <see cref="MaskedTextBox"/> and returns the desired size.
    /// </summary>
    /// <param name="constraint">The available size for this <see cref="MaskedTextBox"/>.</param>
    /// <returns>The desired size of the <see cref="MaskedTextBox"/>.</returns>
    protected override Size MeasureOverride(Size constraint)
    {
      Size size = base.MeasureOverride(constraint);
      if ((Double.IsInfinity(constraint.Width) || Double.IsNaN(constraint.Width)) && !IsTextWrappingEnabled)
      {
        size.Width = _width + Padding.Right + BorderThickness.Right + BorderThickness.Left;
      }
      Dispatcher.BeginInvoke(new Action(CalculateWidth));
      return size;
    }

    private double _width = 0;

    private void CalculateWidth()
    {
      if (_displayControl != null && _displayControl.Document != null && _displayControl.Document.Blocks != null && _displayControl.Document.Blocks.LastBlock != null)
      {
        TextPointer tp = _displayControl.Document.Blocks.LastBlock.ContentEnd;
        if (tp != null)
        {
          Rect rect = tp.GetCharacterRect(LogicalDirection.Forward);
          if (!Double.IsInfinity(rect.X) && _width != rect.X)
          {
            _width = rect.X;
            InvalidateMeasure();
          }
        }
      }
    }

    #region Dependency property declarations

    /// <summary>
    /// Gets or sets the mask.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="MaskProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public string Mask
    {
      get { return (string)GetValue(MaskProperty); }
      set { SetValue(MaskProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Mask"/> property.
    /// </summary>
    public static readonly DependencyProperty MaskProperty =
        DependencyProperty.Register("Mask", typeof(string), typeof(MaskedTextBox),
        new FrameworkPropertyMetadata(OnMaskChanged));

    #region Styling for different display elements

    /// <summary>
    /// Gets or sets the style used to display characters entered by the user.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="InputStyleProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public Style InputStyle
    {
      get { return (Style)GetValue(InputStyleProperty); }
      set { SetValue(InputStyleProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="InputStyle"/> property.
    /// </summary>
    public static readonly DependencyProperty InputStyleProperty =
        DependencyProperty.Register("InputStyle", typeof(Style), typeof(MaskedTextBox));

    /// <summary>
    /// Gets or sets the style used to display literal characters from the mask.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="LiteralStyleProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public Style LiteralStyle
    {
      get { return (Style)GetValue(LiteralStyleProperty); }
      set { SetValue(LiteralStyleProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="LiteralStyle"/> property.
    /// </summary>
    public static readonly DependencyProperty LiteralStyleProperty =
        DependencyProperty.Register("LiteralStyle", typeof(Style), typeof(MaskedTextBox));

    /// <summary>
    /// Gets or sets the style used to display prompts (mask positions where input is
    /// permitted but the user has not yet entered a character).
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="PromptStyleProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public Style PromptStyle
    {
      get { return (Style)GetValue(PromptStyleProperty); }
      set { SetValue(PromptStyleProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="PromptStyle"/> property.
    /// </summary>
    public static readonly DependencyProperty PromptStyleProperty =
        DependencyProperty.Register("PromptStyle", typeof(Style), typeof(MaskedTextBox));


    /// <summary>
    /// Gets or sets the object used to customize the appearance of prompt elements.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="PromptCharDisplaySelectorProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public IPromptCharDisplaySelector PromptCharDisplaySelector
    {
      get { return (IPromptCharDisplaySelector)GetValue(PromptCharDisplaySelectorProperty); }
      set { SetValue(PromptCharDisplaySelectorProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="PromptCharDisplaySelector"/> property.
    /// </summary>
    public static readonly DependencyProperty PromptCharDisplaySelectorProperty =
        DependencyProperty.Register("PromptCharDisplaySelector", typeof(IPromptCharDisplaySelector), 
        typeof(MaskedTextBox), new FrameworkPropertyMetadata(OnPromptCharDisplaySelectorChanged));

    #endregion

    #region Status of last operation

    /// <summary>
    /// Gets the result of the last operation (user text entry) into the <see cref="MaskedTextBox"/>.
    /// This can be used to control the visibility of user assistance or error indicators.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="LastOperationSucceededProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool LastOperationSucceeded
    {
      get { return (bool)GetValue(LastOperationSucceededProperty); }
    }

    private static readonly DependencyPropertyKey LastOperationSucceededPropertyKey =
      DependencyProperty.RegisterReadOnly("LastOperationSucceeded", typeof(bool),
      typeof(MaskedTextBox), new FrameworkPropertyMetadata(true));

    /// <summary>
    /// Identifies the <see cref="LastOperationSucceeded"/> property.
    /// </summary>
    public static readonly DependencyProperty LastOperationSucceededProperty =
      LastOperationSucceededPropertyKey.DependencyProperty;

    /// <summary>
    /// Gets the result of the last operation (user text entry) into the <see cref="MaskedTextBox"/>.
    /// This can be used to control the content of user assistance or error indicators.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="LastOperationResultProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public MaskedTextResultHint LastOperationResult
    {
      get { return (MaskedTextResultHint)GetValue(LastOperationResultProperty); }
    }

    private static readonly DependencyPropertyKey LastOperationResultPropertyKey =
        DependencyProperty.RegisterReadOnly("LastOperationResult", typeof(MaskedTextResultHint), 
        typeof(MaskedTextBox), new FrameworkPropertyMetadata(MaskedTextResultHint.Success));

    /// <summary>
    /// Identifies the <see cref="LastOperationResult"/> property.
    /// </summary>
    public static readonly DependencyProperty LastOperationResultProperty =
        LastOperationResultPropertyKey.DependencyProperty;

    #endregion

    #region Mask completion status

    /// <summary>
    /// Gets whether the mask has been completed, i.e. all mandatory positions have
    /// been filled.  Input into a completed mask may still be possible if optional
    /// positions are unfilled.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsMaskCompletedProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool IsMaskCompleted
    {
      get { return (bool)GetValue(IsMaskCompletedProperty); }
    }

    private static readonly DependencyPropertyKey IsMaskCompletedPropertyKey =
        DependencyProperty.RegisterReadOnly("IsMaskCompleted", typeof(bool), 
        typeof(MaskedTextBox), new FrameworkPropertyMetadata(false));

    /// <summary>
    /// Identifies the <see cref="IsMaskCompleted"/> property.
    /// </summary>
    public static readonly DependencyProperty IsMaskCompletedProperty =
        IsMaskCompletedPropertyKey.DependencyProperty;

    /// <summary>
    /// Gets whether the mask is full, i.e. all available positions have been filled and no
    /// further input is possible.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsMaskFullProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool IsMaskFull
    {
      get { return (bool)GetValue(IsMaskFullProperty); }
    }

    private static readonly DependencyPropertyKey IsMaskFullPropertyKey =
        DependencyProperty.RegisterReadOnly("IsMaskFull", typeof(bool), 
        typeof(MaskedTextBox), new FrameworkPropertyMetadata(false));

    /// <summary>
    /// Identifies the <see cref="IsMaskFull"/> property.
    /// </summary>
    public static readonly DependencyProperty IsMaskFullProperty =
        IsMaskFullPropertyKey.DependencyProperty;

    #endregion


    /// <summary>
    /// Gets or sets whether literals are automatically skipped during text entry.
    /// If true, user input (and deletion) is automatically sent to the next edit
    /// position, so the user can type past literals; if false, the user must type
    /// or cursor over literals.  The default is true.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="AutoSkipLiteralsProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool AutoSkipLiterals
    {
      get { return (bool)GetValue(AutoSkipLiteralsProperty); }
      set { SetValue(AutoSkipLiteralsProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="AutoSkipLiterals"/> property.
    /// </summary>
    public static readonly DependencyProperty AutoSkipLiteralsProperty =
        DependencyProperty.Register("AutoSkipLiterals", typeof(bool), typeof(MaskedTextBox), 
        new FrameworkPropertyMetadata(true, OnAutoSkipLiteralsChanged));

    #region IsTextWrappingEnabled Property

    /// <summary>
    /// Gets or sets whether or not to wrap overflowing text. The default is true.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsTextWrappingEnabledProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool IsTextWrappingEnabled
    {
      get { return (bool)GetValue(IsTextWrappingEnabledProperty); }
      set { SetValue(IsTextWrappingEnabledProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="IsTextWrappingEnabled"/> property.
    /// </summary>
    public static readonly DependencyProperty IsTextWrappingEnabledProperty =
      DependencyProperty.Register("IsTextWrappingEnabled", typeof(bool), typeof(MaskedTextBox),
      new FrameworkPropertyMetadata(true, OnIsTextWrappingEnabledChanged));

    private static void OnIsTextWrappingEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((MaskedTextBox)d).OnIsTextWrappingEnabledChanged();
    }

    private void OnIsTextWrappingEnabledChanged()
    {
      UpdateTextWrapping();
    }

    private void UpdateTextWrapping()
    {
      if (_displayControl != null && _displayControl.Document != null)
      {
        _displayControl.Document.PageWidth = IsTextWrappingEnabled ? Double.NaN : 1000000;
      }
    }

    #endregion // IsTextWrappingEnabled Property

    #endregion

    #region Dependency property change notification dispatchers

    private static void OnMaskChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((MaskedTextBox)d).OnMaskChanged(e);
    }

    private static void OnPromptCharDisplaySelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((MaskedTextBox)d).OnPromptCharDisplaySelectorChanged();
    }

    private static void OnAutoSkipLiteralsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((MaskedTextBox)d).OnAutoSkipLiteralsChanged();
    }

    #endregion

    #region Dependency property change notification handlers

    private void OnMaskChanged(DependencyPropertyChangedEventArgs e)
    {
      _mt.Mask = e.NewValue as string;
      Dispatcher.BeginInvoke(new Action(() =>
      {
        Text = _mt.Text;
        UpdateDisplay();
      }), DispatcherPriority.ApplicationIdle);
    }

    private void OnPromptCharDisplaySelectorChanged()
    {
      UpdateDisplay();
    }

    private void OnAutoSkipLiteralsChanged()
    {
      _mt.AutoSkipLiterals = AutoSkipLiterals;
    }

    #endregion

    #region Low-level text handling overrides

    /// <summary>
    /// Overrides default handling for the PreviewTextInput event.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnPreviewTextInput(TextCompositionEventArgs e)
    {
      base.OnPreviewTextInput(e);
      if (_displayControl != null && _displayControl.IsReadOnly)
      {
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
      if (_displayControl != null && _displayControl.IsReadOnly)
      {
        return;
      }
      if (e.Key == Key.Space || e.Key == Key.Back || e.Key == Key.Delete)
      {
        if (e.Key == Key.Space)
        {
          InsertText(" ");
        }
        else if (e.Key == Key.Back)
        {
          DeleteText(DeleteDirection.Backward);
        }
        else if (e.Key == Key.Delete)
        {
          DeleteText(DeleteDirection.Forward);
        }

        e.Handled = true;
      }
    }

    private void InsertText(string text)
    {
      int newPos = _mt.InsertText(_display, text);
      LoadPostTextOperationState(newPos);
    }

    private void DeleteText(DeleteDirection deleteDirection)
    {
      int newPos = _mt.DeleteText(_display, deleteDirection);
      LoadPostTextOperationState(newPos);
    }

    private void LoadPostTextOperationState(int newCaretPosition)
    {
      SetValue(LastOperationResultPropertyKey, _mt.LastOperationResult);
      SetValue(LastOperationSucceededPropertyKey, _mt.LastOperationSucceeded);
      SetValue(IsMaskCompletedPropertyKey, _mt.IsMaskCompleted);
      SetValue(IsMaskFullPropertyKey, _mt.IsMaskFull);

      if (_mt.LastOperationSucceeded)
      {
        Text = _mt.Text;
        TextUtils.MoveCaretToTextOffset(_display, newCaretPosition);
      }
    }

    #endregion

    #region Display

    internal override void UpdateDisplay()
    {
      if (_displayControl != null)
      {
        _displayControl.Document = _mt.CreateDisplayDocument(this);
        UpdateTextWrapping();
      }
    }

    private RichTextBox _displayControl;
    private ITextPresenter _display = NullTextPresenter.Instance;

    internal override void InitialiseDisplayMembers()
    {
      _displayControl = GetTemplateChild(RichTextBoxPartName) as RichTextBox;
      _display = (_displayControl == null ?
        NullTextPresenter.Instance :
        new RichTextBoxTextPresenter(_displayControl));
      _displayControl.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
      _displayControl.SelectionChanged += new RoutedEventHandler(DisplayControl_SelectionChanged);
      _displayControl.TextChanged += new TextChangedEventHandler(DisplayControl_TextChanged);
    }

    private void DisplayControl_TextChanged(object sender, TextChangedEventArgs e)
    {
      CalculateWidth();
      InvalidateMeasure();
    }

    private void DisplayControl_SelectionChanged(object sender, RoutedEventArgs e)
    {
      if (_displayControl.Selection.Text.EndsWith("\r\n"))
      {
        TextPointer start = _displayControl.Selection.Start;
        TextPointer end = _displayControl.Selection.End.DocumentEnd.GetPositionAtOffset(-1);
        _displayControl.Selection.Select(start, end);
      }
    }

    #endregion

    #region Undo support

    private readonly UndoManager<MaskedTextUndoInfo> _undoManager;

    private void OnUndo(MaskedTextUndoInfo undoInfo)
    {
      Text = undoInfo.Text;
      undoInfo.SetCursorPosition(_display);
    }

    internal override void PushUndo(string oldText)
    {
      MaskedTextUndoInfo undoInfo = MaskedTextUndoInfo.Create(oldText, _display);
      _undoManager.PushUndo(undoInfo);
    }

    #endregion

    #region Edit commands (cut and paste) support

    private void CanExecutePaste(object sender, CanExecuteRoutedEventArgs e)
    {
      bool isTextOnClipboard = Clipboard.ContainsText(TextDataFormat.Text) || Clipboard.ContainsText(TextDataFormat.UnicodeText);
      bool gotPasteTarget = (_displayControl != null);
      e.CanExecute = isTextOnClipboard && gotPasteTarget;
      e.Handled = true;
    }

    private void ExecutePaste(object sender, ExecutedRoutedEventArgs e)
    {
      string text = Clipboard.GetText(TextDataFormat.Text);
      if (String.IsNullOrEmpty(text))
      {
        text = Clipboard.GetText(TextDataFormat.UnicodeText);
      }

      InsertText(text);
      e.Handled = true;
    }

    private void CanExecuteCut(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = (_displayControl != null) && (!_displayControl.Selection.IsEmpty);
      e.Handled = true;
    }

    private void ExecuteCut(object sender, ExecutedRoutedEventArgs e)
    {
      string text = _display.Selection.Text;
      DeleteText(DeleteDirection.Backward);
      Clipboard.SetText(text);
    }

    #endregion
  }
}
