using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Provides common functionality for text boxes that restrict user input.
  /// </summary>
  public abstract class FilteringTextBoxBase : Control
  {
    internal abstract IFilteringTextBoxModel Model { get; }
    internal abstract IUndoManager UndoManager { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FilteringTextBoxBase"/> class.
    /// </summary>
    protected FilteringTextBoxBase()
    {
      BindCommands();
    }

    #region Initialisation

    private void BindCommands()
    {
      BindCommand(ApplicationCommands.Undo, CanExecuteUndo, ExecuteUndo);
      //BindCommand(ApplicationCommands.Paste, CanExecutePaste, ExecutePaste);
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

    /// <summary>
    /// Indicates that the initialization process for the element is complete.
    /// </summary>
    public override void EndInit()
    {
      base.EndInit();
      UndoManager.Clear();
    }

    #endregion

    /// <summary>
    /// Gets or sets the culture used for parsing and formatting of user input.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="CultureProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public CultureInfo Culture
    {
      get { return (CultureInfo)GetValue(CultureProperty); }
      set { SetValue(CultureProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Culture"/> property.
    /// </summary>
    public static readonly DependencyProperty CultureProperty =
        DependencyProperty.Register("Culture", typeof(CultureInfo), 
        typeof(FilteringTextBoxBase),
        new FrameworkPropertyMetadata(
          CultureInfo.CurrentCulture,
          OnCultureChanged));

    /// <summary>
    /// Gets or sets the text displayed to represent the value.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="TextProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>BindsTwoWayByDefault</td></tr>
    /// </table>
    /// </remarks>
    public string Text
    {
      get { return (string)GetValue(TextProperty); }
      set { SetValue(TextProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Text"/> property.
    /// </summary>
    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register("Text", typeof(string), 
        typeof(FilteringTextBoxBase),
        new FrameworkPropertyMetadata(
          String.Empty,
          FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
          OnTextChanged));

    private static void OnCultureChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((FilteringTextBoxBase)d).OnCultureChanged();
    }

    private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((FilteringTextBoxBase)d).OnTextChanged(e);
    }


    /// <summary>
    /// Gets or sets the text alignment.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="TextAlignmentProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public TextAlignment TextAlignment
    {
      get { return (TextAlignment)GetValue(TextAlignmentProperty); }
      set { SetValue(TextAlignmentProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="TextAlignment"/> property.
    /// </summary>
    public static readonly DependencyProperty TextAlignmentProperty =
      DependencyProperty.Register("TextAlignment", typeof(TextAlignment), typeof(FilteringTextBoxBase),
      new FrameworkPropertyMetadata(TextAlignment.Left));

    private void OnCultureChanged()
    {
      Model.Culture = Culture;
      //Text = Model.Text;
      UpdateDisplay();
    }

    private void OnTextChanged(DependencyPropertyChangedEventArgs e)
    {
      if (UndoManager != null && !UndoManager.Undoing)
      {
        PushUndo((string)(e.OldValue));
      }
      
      Model.Text = Text;
      AfterTextChanged();
      UpdateDisplay();
    }

    internal virtual void AfterTextChanged()
    {
      // no-op in base class
    }

    /// <summary>
    /// Called by the framework when a template is applied to the control.
    /// </summary>
    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      InitialiseDisplayMembers();
      UpdateDisplay();
    }

    internal abstract void InitialiseDisplayMembers();
    internal abstract void UpdateDisplay();

    private void CanExecuteUndo(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = UndoManager.CanUndo;
      e.Handled = true;
    }

    private void ExecuteUndo(object sender, ExecutedRoutedEventArgs e)
    {
      if (UndoManager.CanUndo)
      {
        UndoManager.Undo();
      }
      e.Handled = true;
    }

    internal abstract void PushUndo(string oldText);

    /// <summary>
    /// Gets the <see cref="ResourceKey"/> for the brush used to draw text box
    /// borders.
    /// </summary>
    public static object BorderBrushKey
    {
      get { return new ComponentResourceKey(typeof(FilteringTextBoxBase), "BorderBrush"); }
    }
  }
}
