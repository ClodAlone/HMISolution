using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Infralution.Licensing;
using System.Reflection;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Provides a user interface for a <see cref="RichTextBox"/>.
  /// </summary>
  [LicenseProvider(typeof(PublicEncryptedLicenseProvider))]
  public class RichTextToolBar : Control, INotifyPropertyChanged
  {
    private bool _internallyChangingProperty;
    private IList<FontFamily> _fontFamilies;
    private IList<double> _fontSizes;
    private ReadOnlyCollection<NamedColor> _foregrounds;
    private ReadOnlyCollection<NamedColor> _backgrounds;

    static RichTextToolBar()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(RichTextToolBar),
        new FrameworkPropertyMetadata(typeof(RichTextToolBar)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RichTextToolBar"/> class.
    /// </summary>
    public RichTextToolBar()
    {
      // Licensing
      //new WpfElementsCore(Assembly.GetCallingAssembly());
      //LicenseHelper.Attach(this, Assembly.GetCallingAssembly());
      // End licensing

      CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, Cut_Executed, Cut_CanExecute));
      CommandBindings.Add(new CommandBinding(ApplicationCommands.Copy, Copy_Executed, Copy_CanExecute));
      CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, Paste_Executed, Paste_CanExecute));
      CommandBindings.Add(new CommandBinding(ApplicationCommands.Undo, Undo_Executed, Undo_CanExecute));
      CommandBindings.Add(new CommandBinding(ApplicationCommands.Redo, Redo_Executed, Redo_CanExecute));

      CommandBindings.Add(new CommandBinding(RichTextCommands.ChangeTextAlignment, ChangeTextAlignment_Executed, ChangeTextAlignment_CanExecute));

      CommandBindings.Add(new CommandBinding(RichTextCommands.ToggleFontWeight, ToggleFontWeight_Executed, ToggleFontWeight_CanExecute));
      CommandBindings.Add(new CommandBinding(RichTextCommands.ToggleFontStyle, ToggleFontStyle_Executed, ToggleFontStyle_CanExecute));
      CommandBindings.Add(new CommandBinding(RichTextCommands.ToggleTextDecorations, ToggleTextDecorations_Executed, ToggleTextDecorations_CanExecute));

      CommandBindings.Add(new CommandBinding(RichTextCommands.ApplyForegroundColor, ApplyForegroundColor_Executed, ApplyForegroundColor_CanExecute));
      CommandBindings.Add(new CommandBinding(RichTextCommands.ApplyBackgroundColor, ApplyBackgroundColor_Executed, ApplyBackgroundColor_CanExecute));
    }

    /// <summary>
    /// Gets or sets the RichTextBox linked to this <see cref="RichTextToolBar"/>.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="RichTextBoxProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public RichTextBox RichTextBox
    {
      get { return (RichTextBox)GetValue(RichTextBoxProperty); }
      set { SetValue(RichTextBoxProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="RichTextBox"/> property.
    /// </summary>
    public static readonly DependencyProperty RichTextBoxProperty =
      DependencyProperty.Register("RichTextBox", typeof(RichTextBox), typeof(RichTextToolBar),
      new FrameworkPropertyMetadata(OnRichTextBoxChanged));

    private static void OnRichTextBoxChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((RichTextToolBar)d).OnRichTextBoxChanged(e);
    }

    private void OnRichTextBoxChanged(DependencyPropertyChangedEventArgs e)
    {
      RichTextBox old = e.OldValue as RichTextBox;
      if (old != null)
      {
        RichTextBox.SelectionChanged -= new RoutedEventHandler(RichTextBox_SelectionChanged);
        RichTextBox.RemoveHandler(Control.KeyDownEvent, new KeyEventHandler(RichTextBox_KeyDown));
      }
      if (RichTextBox != null)
      {
        RichTextBox.SelectionChanged += new RoutedEventHandler(RichTextBox_SelectionChanged);
        RichTextBox.AddHandler(Control.KeyDownEvent, new KeyEventHandler(RichTextBox_KeyDown), true);
        RichTextBox.AppendText("");
      }
      UpdateFormatProperties();
    }

    private void RichTextBox_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyboardDevice.Modifiers == ModifierKeys.Control)
      {
        UpdateFormatProperties();
      }
    }

    private void RichTextBox_SelectionChanged(object sender, RoutedEventArgs e)
    {
      UpdateFormatProperties();
    }

    private void UpdateFormatProperties()
    {
      if (RichTextBox != null)
      {
        _internallyChangingProperty = true;
        object propertyValue = RichTextBox.Selection.GetPropertyValue(RichTextBox.FontWeightProperty);
        CurrentFontWeight = propertyValue.Equals(DependencyProperty.UnsetValue) ? FontWeights.Normal : (FontWeight)propertyValue;
        OnPropertyChanged("CurrentFontWeight");

        propertyValue = RichTextBox.Selection.GetPropertyValue(RichTextBox.FontStyleProperty);
        CurrentFontStyle = propertyValue.Equals(DependencyProperty.UnsetValue) ? FontStyles.Normal : (FontStyle)propertyValue;
        OnPropertyChanged("CurrentFontStyle");

        propertyValue = RichTextBox.Selection.GetPropertyValue(TextBox.TextDecorationsProperty);
        CurrentTextDecorations = propertyValue.Equals(DependencyProperty.UnsetValue) ? new TextDecorationCollection() : (TextDecorationCollection)propertyValue;
        OnPropertyChanged("CurrentTextDecorations");

        propertyValue = RichTextBox.Selection.GetPropertyValue(RichTextBox.FontSizeProperty);
        CurrentFontSize = propertyValue.Equals(DependencyProperty.UnsetValue) ? 0 : (double)propertyValue;
        OnPropertyChanged("CurrentFontSize");

        propertyValue = RichTextBox.Selection.GetPropertyValue(RichTextBox.FontFamilyProperty);
        CurrentFontFamily = propertyValue.Equals(DependencyProperty.UnsetValue) ? new FontFamily() : (FontFamily)propertyValue;
        OnPropertyChanged("CurrentFontFamily");

        UpdateCurrentTextAlignment();
        _internallyChangingProperty = false;
      }
    }

    private void UpdateCurrentTextAlignment()
    {
      object propertyValue = RichTextBox.Selection.GetPropertyValue(TextBox.TextAlignmentProperty);
      if (propertyValue.Equals(DependencyProperty.UnsetValue))
      {
        CurrentTextAlignment = null;
      }
      else
      {
        CurrentTextAlignment = (TextAlignment)propertyValue;
      }
    }

    #region Cut

    private void Cut_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = RichTextBox == null ? false : !RichTextBox.Selection.IsEmpty;
    }

    private void Cut_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      if (RichTextBox != null)
      {
        RichTextBox.Cut();
        RichTextBox.Focus();
      }
    }

    #endregion // Cut

    #region Copy

    private void Copy_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = RichTextBox == null ? false : !RichTextBox.Selection.IsEmpty;
    }

    private void Copy_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      if (RichTextBox != null)
      {
        RichTextBox.Copy();
        RichTextBox.Focus();
      }
    }

    #endregion // Copy

    #region Paste

    private void Paste_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = RichTextBox == null ? false : true;
    }

    private void Paste_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      if (RichTextBox != null)
      {
        RichTextBox.Paste();
        RichTextBox.Focus();
      }
    }

    #endregion // Undo

    #region Undo

    private void Undo_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = RichTextBox == null ? false : RichTextBox.CanUndo;
    }

    private void Undo_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      if (RichTextBox != null)
      {
        RichTextBox.Undo();
        RichTextBox.Focus();
      }
    }

    #endregion // Undo

    #region Redo

    private void Redo_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = RichTextBox == null ? false : RichTextBox.CanRedo;
    }

    private void Redo_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      if (RichTextBox != null)
      {
        RichTextBox.Redo();
        RichTextBox.Focus();
      }
    }

    #endregion // Redo

    #region ChangeTextAlignment

    private void ChangeTextAlignment_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = RichTextBox == null ? false : true;
    }

    private void ChangeTextAlignment_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      if (RichTextBox != null)
      {
        object value = e.Parameter;
        object propertyValue = RichTextBox.Selection.GetPropertyValue(TextBox.TextAlignmentProperty);
        if (!propertyValue.Equals(DependencyProperty.UnsetValue))
        {
          TextAlignment align = (TextAlignment)propertyValue;
          if (align.Equals(e.Parameter))
          {
            value = TextAlignment.Left;
            if (align.Equals(TextAlignment.Left))
            {
              value = TextAlignment.Justify;
            }
          }
        }
        RichTextBox.Selection.ApplyPropertyValue(TextBox.TextAlignmentProperty, value);
        UpdateCurrentTextAlignment();
        RichTextBox.Focus();
      }
    }

    /// <summary>
    /// Gets or sets the TextAlignment of the selected text. If the different parts of the selected text have
    /// different alignments, then the value of this property will be null.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="CurrentTextAlignmentProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public TextAlignment? CurrentTextAlignment
    {
      get { return (TextAlignment?)GetValue(CurrentTextAlignmentProperty); }
      set { SetValue(CurrentTextAlignmentProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="CurrentTextAlignment"/> property.
    /// </summary>
    public static readonly DependencyProperty CurrentTextAlignmentProperty =
      DependencyProperty.Register("CurrentTextAlignment", typeof(TextAlignment?), typeof(RichTextToolBar),
      new FrameworkPropertyMetadata(null));

    #endregion // ChangeTextAlignment

    #region ChangeFontSize

    /// <summary>
    /// Gets or sets the font size of the selected text. Setting this property will apply a font size
    /// to the selected text.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="CurrentFontSizeProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public double CurrentFontSize
    {
      get { return (double)GetValue(CurrentFontSizeProperty); }
      set { SetValue(CurrentFontSizeProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="CurrentFontSize"/> property.
    /// </summary>
    public static readonly DependencyProperty CurrentFontSizeProperty =
      DependencyProperty.Register("CurrentFontSize", typeof(double), typeof(RichTextToolBar),
      new FrameworkPropertyMetadata(OnCurrentFontSizeChanged));

    private static void OnCurrentFontSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((RichTextToolBar)d).OnCurrentFontSizeChanged();
    }

    private void OnCurrentFontSizeChanged()
    {
      if (RichTextBox != null && !_internallyChangingProperty)
      {
        double fontSize = CurrentFontSize;
        bool isEmpty = PrepareEmptyCase();
        RichTextBox.Selection.ApplyPropertyValue(RichTextBox.FontSizeProperty, fontSize);
        if (isEmpty)
        {
          RichTextBox.Selection.Select(RichTextBox.Selection.Start, RichTextBox.Selection.Start);
        }
        EnsureEditorFocus();
      }
    }

    #endregion // ChangeFontSize

    #region ChangeFontFamily

    /// <summary>
    /// Gets or sets the FontFamily of the selected text. Setting this property will apply a FontFamily
    /// to the selected text.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="CurrentFontFamilyProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public FontFamily CurrentFontFamily
    {
      get { return (FontFamily)GetValue(CurrentFontFamilyProperty); }
      set { SetValue(CurrentFontFamilyProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="CurrentFontFamily"/> property.
    /// </summary>
    public static readonly DependencyProperty CurrentFontFamilyProperty =
      DependencyProperty.Register("CurrentFontFamily", typeof(FontFamily), typeof(RichTextToolBar),
      new FrameworkPropertyMetadata(OnCurrentFontFamilyChanged));

    private static void OnCurrentFontFamilyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((RichTextToolBar)d).OnCurrentFontFamilyChanged();
    }

    private void OnCurrentFontFamilyChanged()
    {
      if (RichTextBox != null && !_internallyChangingProperty)
      {
        Dispatcher.BeginInvoke(new Action(ApplyFontFamilyChange));
      }
    }

    private void ApplyFontFamilyChange()
    {
      if (RichTextBox != null && !_internallyChangingProperty)
      {
        FontFamily family = CurrentFontFamily;
        if (family != null)
        {
          bool isEmpty = PrepareEmptyCase();
          RichTextBox.Selection.ApplyPropertyValue(RichTextBox.FontFamilyProperty, family);
          if (isEmpty)
          {
            RichTextBox.Selection.Select(RichTextBox.Selection.Start, RichTextBox.Selection.Start);
          }
        }
        RichTextBox.Focus();
      }
    }

    #endregion // ChangeFontFamily

    #region ChangeForeground

    private void ApplyForegroundColor_CanExecute(object sender, CanExecuteRoutedEventArgs args)
    {
      args.CanExecute = true;
    }

    private void ApplyForegroundColor_Executed(object sender, ExecutedRoutedEventArgs args)
    {
      OnCurrentForegroundChanged();
    }

    /// <summary>
    /// Gets or sets the foreground brush of the selected text. Setting this property will apply a
    /// foreground brush to the selected text.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="CurrentForegroundProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public Brush CurrentForeground
    {
      get { return (Brush)GetValue(CurrentForegroundProperty); }
      set { SetValue(CurrentForegroundProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="CurrentForeground"/> property.
    /// </summary>
    public static readonly DependencyProperty CurrentForegroundProperty =
      DependencyProperty.Register("CurrentForeground", typeof(Brush), typeof(RichTextToolBar),
      new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Black), OnCurrentForegroundChanged));

    private static void OnCurrentForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      SolidColorBrush oldBrush = e.OldValue as SolidColorBrush;
      SolidColorBrush newBrush = e.NewValue as SolidColorBrush;
      if (oldBrush == null || newBrush == null || !oldBrush.Color.Equals(newBrush.Color))
      {
        ((RichTextToolBar)d).OnCurrentForegroundChanged();
      }
    }

    private void OnCurrentForegroundChanged()
    {
      if (RichTextBox != null)
      {
        bool isEmpty = PrepareEmptyCase();
        RichTextBox.Selection.ApplyPropertyValue(RichTextBox.ForegroundProperty, CurrentForeground);
        if (isEmpty)
        {
          RichTextBox.Selection.Select(RichTextBox.Selection.Start, RichTextBox.Selection.Start);
        }
        EnsureEditorFocus();
      }
    }

    #endregion // ChangeForeground

    #region ChangeBackground

    private void ApplyBackgroundColor_CanExecute(object sender, CanExecuteRoutedEventArgs args)
    {
      args.CanExecute = true;
    }

    private void ApplyBackgroundColor_Executed(object sender, ExecutedRoutedEventArgs args)
    {
      OnCurrentBackgroundChanged();
    }

    /// <summary>
    /// Gets or sets the background brush of the selected text. Setting this property will apply a
    /// background brush to the selected text.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="CurrentBackgroundProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public Brush CurrentBackground
    {
      get { return (Brush)GetValue(CurrentBackgroundProperty); }
      set { SetValue(CurrentBackgroundProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="CurrentBackground"/> property.
    /// </summary>
    public static readonly DependencyProperty CurrentBackgroundProperty =
      DependencyProperty.Register("CurrentBackground", typeof(Brush), typeof(RichTextToolBar),
      new FrameworkPropertyMetadata(new SolidColorBrush(Colors.White), OnCurrentBackgroundChanged));

    private static void OnCurrentBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      SolidColorBrush oldBrush = e.OldValue as SolidColorBrush;
      SolidColorBrush newBrush = e.NewValue as SolidColorBrush;
      if(oldBrush == null || newBrush == null || !oldBrush.Color.Equals(newBrush.Color))
      {
        ((RichTextToolBar)d).OnCurrentBackgroundChanged();
      }
    }

    private void OnCurrentBackgroundChanged()
    {
      if (RichTextBox != null)
      {
        bool isEmpty = PrepareEmptyCase();
        RichTextBox.Selection.ApplyPropertyValue(Span.BackgroundProperty, CurrentBackground);
        if (isEmpty)
        {
          RichTextBox.Selection.Select(RichTextBox.Selection.Start, RichTextBox.Selection.Start);
        }
        EnsureEditorFocus();
      }
    }

    #endregion // ChangeBackground

    #region ToggleFontWeight

    private void ToggleFontWeight_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = RichTextBox == null ? false : true;
    }

    private void ToggleFontWeight_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      if (RichTextBox != null)
      {
        bool isEmpty = PrepareEmptyCase();
        if (RichTextBox.Selection.GetPropertyValue(RichTextBox.FontWeightProperty).Equals(e.Parameter))
        {
          RichTextBox.Selection.ApplyPropertyValue(RichTextBox.FontWeightProperty, FontWeights.Normal);
        }
        else
        {
          RichTextBox.Selection.ApplyPropertyValue(RichTextBox.FontWeightProperty, e.Parameter);
        }
        if (isEmpty)
        {
          RichTextBox.Selection.Select(RichTextBox.Selection.Start, RichTextBox.Selection.Start);
        }
        
      }
    }

    /// <summary>
    /// Gets or sets the font weight of the selected text.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="CurrentFontWeightProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public FontWeight CurrentFontWeight
    {
      get { return (FontWeight)GetValue(CurrentFontWeightProperty); }
      set { SetValue(CurrentFontWeightProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="CurrentFontWeight"/> property.
    /// </summary>
    public static readonly DependencyProperty CurrentFontWeightProperty =
      DependencyProperty.Register("CurrentFontWeight", typeof(FontWeight), typeof(RichTextToolBar),
      new FrameworkPropertyMetadata(null));

    #endregion // ToggleFontWeight

    #region ToggleFontStyle

    private void ToggleFontStyle_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = RichTextBox == null ? false : true;
    }

    private void ToggleFontStyle_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      if (RichTextBox != null)
      {
        bool isEmpty = PrepareEmptyCase();
        if (RichTextBox.Selection.GetPropertyValue(RichTextBox.FontStyleProperty).Equals(e.Parameter))
        {
          RichTextBox.Selection.ApplyPropertyValue(RichTextBox.FontStyleProperty, FontStyles.Normal);
        }
        else
        {
          RichTextBox.Selection.ApplyPropertyValue(RichTextBox.FontStyleProperty, e.Parameter);
        }
        if (isEmpty)
        {
          RichTextBox.Selection.Select(RichTextBox.Selection.Start, RichTextBox.Selection.Start);
        }
      }
    }

    /// <summary>
    /// Gets or sets the font style of the selected text.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="CurrentFontStyleProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public FontStyle CurrentFontStyle
    {
      get { return (FontStyle)GetValue(CurrentFontStyleProperty); }
      set { SetValue(CurrentFontStyleProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="CurrentFontStyle"/> property.
    /// </summary>
    public static readonly DependencyProperty CurrentFontStyleProperty =
      DependencyProperty.Register("CurrentFontStyle", typeof(FontStyle), typeof(RichTextToolBar),
      new FrameworkPropertyMetadata(null));

    #endregion // ToggleFontStyle

    #region ToggleTextDecorations

    private void ToggleTextDecorations_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = RichTextBox == null ? false : true;
    }

    private void ToggleTextDecorations_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      if (RichTextBox != null)
      {
        bool isEmpty = PrepareEmptyCase();
        object propertyValue = RichTextBox.Selection.GetPropertyValue(TextBox.TextDecorationsProperty);
        TextDecorationCollection decorations = new TextDecorationCollection();
        if(propertyValue is TextDecorationCollection)
        {
          decorations = propertyValue as TextDecorationCollection;
        }
        if (decorations.Contains(e.Parameter as TextDecoration))
        {
          RichTextBox.Selection.ApplyPropertyValue(TextBox.TextDecorationsProperty, new TextDecorationCollection());
        }
        else
        {
          TextDecorationCollection collection = new TextDecorationCollection();
          collection.Add((TextDecoration)e.Parameter);
          RichTextBox.Selection.ApplyPropertyValue(TextBox.TextDecorationsProperty, collection);
        }
        if (isEmpty)
        {
          RichTextBox.Selection.Select(RichTextBox.Selection.Start, RichTextBox.Selection.Start);
        }
      }
    }

    /// <summary>
    /// Gets or sets the text decorartions of the selected text.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="CurrentTextDecorationsProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public TextDecorationCollection CurrentTextDecorations
    {
      get { return (TextDecorationCollection)GetValue(CurrentTextDecorationsProperty); }
      set { SetValue(CurrentTextDecorationsProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="CurrentTextDecorations"/> property.
    /// </summary>
    public static readonly DependencyProperty CurrentTextDecorationsProperty =
      DependencyProperty.Register("CurrentTextDecorations", typeof(TextDecorationCollection), typeof(RichTextToolBar),
      new FrameworkPropertyMetadata(null));

    #endregion // ToggleTextDecorations

    /// <summary>
    /// Gets or sets how font names should be displayed in the font selector.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="FontSelectorDisplayModeProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public FontSelectorDisplayMode FontSelectorDisplayMode
    {
      get { return (FontSelectorDisplayMode)GetValue(FontSelectorDisplayModeProperty); }
      set { SetValue(FontSelectorDisplayModeProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="FontSelectorDisplayMode"/> property.
    /// </summary>
    public static readonly DependencyProperty FontSelectorDisplayModeProperty =
      DependencyProperty.Register("FontSelectorDisplayMode", typeof(FontSelectorDisplayMode), typeof(RichTextToolBar),
      new FrameworkPropertyMetadata(FontSelectorDisplayMode.Preview));

    private bool PrepareEmptyCase()
    {
      bool isEmpty = IsRichTextBoxEmpty;
      RichTextBox.Focus();
      if (isEmpty)
      {
        RichTextBox.SelectAll();
      }
      return isEmpty;
    }

    private bool IsRichTextBoxEmpty
    {
      get
      {
        TextRange range = new TextRange(RichTextBox.Document.ContentStart, RichTextBox.Document.ContentEnd);
        return range.Text.Length == 2;
      }
    }

    private void EnsureEditorFocus()
    {
      if (RichTextBox != null)
      {
        Dispatcher.BeginInvoke(new Action(FocusEditor));
      }
    }

    private void FocusEditor()
    {
      if (RichTextBox != null)
      {
        RichTextBox.Focus();
      }
    }

    /// <summary>
    /// Gets or sets a list of font sizes available to the user.
    /// </summary>
    public IList<double> FontSizes
    {
      get
      {
        if (_fontSizes == null)
        {
          _fontSizes = new List<double>();
          _fontSizes.Add(8);
          _fontSizes.Add(9);
          _fontSizes.Add(10);
          _fontSizes.Add(11);
          _fontSizes.Add(12);
          _fontSizes.Add(14);
          _fontSizes.Add(16);
          _fontSizes.Add(18);
          _fontSizes.Add(20);
          _fontSizes.Add(22);
          _fontSizes.Add(24);
          _fontSizes.Add(26);
          _fontSizes.Add(28);
          _fontSizes.Add(36);
          _fontSizes.Add(48);
          _fontSizes.Add(72);
        }
        return _fontSizes;
      }
      set
      {
        _fontSizes = value;
        OnPropertyChanged("FontSizes");
      }
    }

    /// <summary>
    /// Gets or sets a list of font families available to the user.
    /// </summary>
    public IList<FontFamily> FontFamilies
    {
      get
      {
        if (_fontFamilies == null)
        {
          _fontFamilies = Fonts.SystemFontFamilies.ToList();
        }
        return _fontFamilies;
      }
      set
      {
        _fontFamilies = value;
        OnPropertyChanged("FontFamilies");
      }
    }

    /// <summary>
    /// Gets or sets the foreground color palette.
    /// </summary>
    public ReadOnlyCollection<NamedColor> ForegroundPalette
    {
      get
      {
        if (_foregrounds == null)
        {
          _foregrounds = StandardPalettes.OfficePalette;
        }
        return _foregrounds;
      }
      set
      {
        _foregrounds = value;
        OnPropertyChanged("ForegroundPalette");
      }
    }

    /// <summary>
    /// Gets or sets the background color palette.
    /// </summary>
    public ReadOnlyCollection<NamedColor> BackgroundPalette
    {
      get
      {
        if (_backgrounds == null)
        {
          _backgrounds = StandardPalettes.OfficePalette;
        }
        return _backgrounds;
      }
      set
      {
        _backgrounds = value;
        OnPropertyChanged("BackgroundPalette");
      }
    }

    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <param name="name">The name of the property that changed.</param>
    protected virtual void OnPropertyChanged(string name)
    {
      PropertyChangedEventHandler handler = PropertyChanged;
      if (handler != null)
      {
        handler(this, new PropertyChangedEventArgs(name));
      }
    }
  }
}
