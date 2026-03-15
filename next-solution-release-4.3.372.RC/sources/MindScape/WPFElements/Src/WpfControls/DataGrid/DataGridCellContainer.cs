using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Data;
using Mindscape.WpfElements.PropertyEditing;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Mindscape.WpfElements.WpfDataGrid
{
  /// <summary>
  /// A control representing a single cell in a <see cref="DataGrid"/>.
  /// </summary>
  public class DataGridCellContainer : ContentControl
  {
    private bool _isSettingValueBindingInternal;

    static DataGridCellContainer()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(DataGridCellContainer), new FrameworkPropertyMetadata(typeof(DataGridCellContainer)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataGridCellContainer"/> class.
    /// </summary>
    public DataGridCellContainer()
    {
      AddHandler(DataGridCellContainer.PreviewMouseDownEvent, new MouseButtonEventHandler(DataGridCellContainer_MouseDown), true);
      AddHandler(DataGridCellContainer.PreviewMouseDoubleClickEvent, new MouseButtonEventHandler(DataGridCellContainer_MouseDoubleClick), true);
      AddHandler(PreviewMouseMoveEvent, new MouseEventHandler(DataGridCellContainer_MouseMove), true);

      CommandBindings.Add(new CommandBinding(DataGridCommands.ToggleExpandedState, ToggleExpandedState_Executed, ToggleExpandedState_CanExecute));

      Validation.SetErrorTemplate(this, null);
    }

    private void ToggleExpandedState_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      //IsExpanded = !IsExpanded;
    }

    // TODO: the command should be removed and this logic should be placed in the Row.IsExpanded property.
    //       Although, note that this logic in the command can be used to disable the toggle. Moving this logic will no longer do this.
    private void ToggleExpandedState_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = true;
      DataGrid grid = VisualTreeUtils.FindContaining<DataGrid>(this);
      if (grid != null)
      {
        bool canChangeState = grid.OnHierarchicalStateChanging(Row.Content, !Row.IsExpanded);
        if (!canChangeState)
        {
          e.CanExecute = false;
        }
      }
    }

    private void DataGridCellContainer_MouseMove(object sender, MouseEventArgs e)
    {
      DataGrid grid = VisualTreeUtils.FindContaining<DataGrid>(this);
      if (grid != null)
      {
        grid.OnCellMouseOver(this);
      }
    }

    private void DataGridCellContainer_MouseDown(object sender, MouseButtonEventArgs e)
    {
      DataGrid grid = VisualTreeUtils.FindContaining<DataGrid>(this);
      if (grid != null)
      {
        if (e.ChangedButton == MouseButton.Right)
        {
          grid.OnCellRightMouseDown(this);
        }
        else
        {
          grid.OnCellMouseDown(this);
        }
      }
      if (IsEditing)
      {
        SelectionChanged = true;
      }
    }

    private void DataGridCellContainer_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
      if (Row != null && Row.ItemWrapper != null)
      {
        DataGrid grid = VisualTreeUtils.FindContaining<DataGrid>(this);
        if (grid != null)
        {
          bool canChangeState = grid.OnHierarchicalStateChanging(Row.Content, !Row.IsExpanded);
          if (canChangeState && Row.ItemWrapper.HasChildren)
          {
            Row.ItemWrapper.IsExpanded = !Row.ItemWrapper.IsExpanded;
          }
        }
      }
      WasDoubleClicked = true;
    }

    /*
    /// <summary>
    /// Called when a mouse button is pressed on this <see cref="DataGridCellContainer"/>.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
      DataGrid grid = VisualTreeUtils.FindContaining<DataGrid>(this);
      if (grid != null)
      {
        grid.OnCellMouseDown(this);
      }
    }*/

    /*/// <summary>
    /// Called when the mouse enters this <see cref="DataGridCellContainer"/>.
    /// </summary>
    /// <param name="e">The event args.</param>
    protected override void OnMouseEnter(MouseEventArgs e)
    {
      base.OnMouseEnter(e);
      if (Column != null && Column.DisplayTemplate != null && !IsEditing)
      {
        CheckBox checkBox = VisualTreeUtils.GetChild<CheckBox>(this);
        if (checkBox != null)
        {
          IPropertyInfo info = Column.PropertyInfo;
          if (info != null)
          {
            _isSettingValueBindingInternal = true;
            BindingOperations.SetBinding(this, ValueProperty, new Binding { Source = Content, Path = new PropertyPath(info.Name) });
            _isSettingValueBindingInternal = false;
          }
        }
      }
    }*/

    /// <summary>
    /// Called when the content changes.
    /// </summary>
    /// <param name="oldContent">The old content.</param>
    /// <param name="newContent">The new content.</param>
    protected override void OnContentChanged(object oldContent, object newContent)
    {
      base.OnContentChanged(oldContent, newContent);

      CommandManager.InvalidateRequerySuggested();
    }

    // Gets a cell model for this cell container.
    internal DataGridCell DataGridCell
    {
      get
      {
        return new DataGridCell(Row == null ? null : Row.Content, Column);
      }
    }

    #region IsSelected Property

    /// <summary>
    /// Gets the whether this cell is selected.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsSelectedProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool IsSelected
    {
      get { return (bool)GetValue(IsSelectedProperty); }
      internal set
      {
        if (IsSelected != value)
        {
          SetValue(IsSelectedPropertyKey, value);
        }
      }
    }

    private static readonly DependencyPropertyKey IsSelectedPropertyKey =
        DependencyProperty.RegisterReadOnly("IsSelected", typeof(bool), typeof(DataGridCellContainer), new UIPropertyMetadata(false));

    /// <summary>
    /// Identifies the <see cref="IsSelected"/> property.
    /// </summary>
    public static readonly DependencyProperty IsSelectedProperty =
        IsSelectedPropertyKey.DependencyProperty;

    #endregion // IsSelected Property

    #region IsHighlighted Property

    /// <summary>
    /// Gets whether this cell is the highlighted cell.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsHighlightedProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool IsHighlighted
    {
      get { return (bool)GetValue(IsHighlightedProperty); }
    }

    internal void SetIsHighlighted(bool value)
    {
      if (IsHighlighted != value)
      {
        if (value == false)
        {
          SetIsEditing(false);
        }

        SetValue(IsHighlightedPropertyKey, value);
      }
    }

    private static readonly DependencyPropertyKey IsHighlightedPropertyKey =
        DependencyProperty.RegisterReadOnly("IsHighlighted", typeof(bool), typeof(DataGridCellContainer), new UIPropertyMetadata(false));

    /// <summary>
    /// Identifies the <see cref="IsHighlighted"/> property.
    /// </summary>
    public static readonly DependencyProperty IsHighlightedProperty =
        IsHighlightedPropertyKey.DependencyProperty;

    #endregion // IsHighlighted Property

    #region IsEditing Property

    /// <summary>
    /// Gets whether this cell is in edit mode.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsEditingProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool IsEditing
    {
      get { return (bool)GetValue(IsEditingProperty); }
    }

    internal void SetIsEditing(bool value)
    {
      SetIsEditing(value, null);
    }

    internal void SetIsEditing(bool value, string input)
    {
      if (Column != null && Column.IsAlwaysInEditMode)
      {
        value = true;
      }
      if (_presenter != null)
      {
        _presenter.SizeChanged -= new SizeChangedEventHandler(Presenter_SizeChanged);
      }

      DataGrid grid = VisualTreeUtils.FindContaining<DataGrid>(this);
      if (grid != null)
      {
        if (!grid.CanEdit(DataGridCell.GetCell(this)))
        {
          value = false;
        }
      }

      if (IsEditing != value)
      {
        SetValue(IsEditingPropertyKey, value);

        //DataTemplate editorTemplate = DataGridColumn.GetEditorTemplate(Column);
        //DataTemplateSelector editorSelector = DataGridColumn.GetEditorTemplateSelector(Column);
        DataTemplate editorTemplate = Column.EditorTemplate;
        DataTemplateSelector editorSelector = Column.EditorTemplateSelector;
        if (editorTemplate != null && IsEditing)
        {
          ContentTemplateSelector = null;
          ContentTemplate = editorTemplate;
          Dispatcher.BeginInvoke(new AttemptToFocusEditorDelegate(AttemptToFocusEditor), DispatcherPriority.Loaded, input);
          if (Column == null || !Column.IsAlwaysInEditMode)
          {
            Row.SetValue(DataGridRow.IsEditingPropertyKey, true);
          }
          Row.EditingCell = this;
        }
        else if (editorSelector != null && IsEditing)
        {
          ContentTemplate = null;
          ContentTemplateSelector = editorSelector;
          Dispatcher.BeginInvoke(new AttemptToFocusEditorDelegate(AttemptToFocusEditor), DispatcherPriority.Loaded, input);
          if (Column == null || !Column.IsAlwaysInEditMode)
          {
            Row.SetValue(DataGridRow.IsEditingPropertyKey, true);
          }
          Row.EditingCell = this;
        }
        else if (!IsEditing)
        {
          if (_textBox != null)
          {
            _previouslyFocusedElement = _textBox;
            _textBox.RaiseEvent(new RoutedEventArgs(Control.LostFocusEvent));
          }
          if (Keyboard.FocusedElement != null)
          {
            _previouslyFocusedElement = Keyboard.FocusedElement as DependencyObject;
            Keyboard.FocusedElement.RaiseEvent(new RoutedEventArgs(Control.LostFocusEvent));
          }
          Focus();
          Validate();
          Focus();

          EnterDisplayMode();
          
          if (Row.EditingCell == this)
          {
            Row.SetValue(DataGridRow.IsEditingPropertyKey, false);
            Row.EditingCell = null;
          }
        }

        if (IsEditing)
        {
          //Mindscape.WpfElements.WpfPropertyGrid.IPropertyInfo info = DataGridColumn.GetPropertyInfo(Column);
          IPropertyInfo info = Column.PropertyInfo;
          if (info != null)
          {
            _isSettingValueBindingInternal = true;
            Binding binding = null;
            BindingPropertyInfoAdapter adapter = info as BindingPropertyInfoAdapter;
            if (adapter != null)
            {
              DataContext = Content;
              binding = adapter.Binding;
              if (binding.ValidatesOnExceptions || binding.ValidatesOnDataErrors)
              {
                adapter.ValueChanged += new EventHandler(BindingPropertyInfoAdapter_ValueChanged);
              }
            }
            else
            {
              binding = new Binding { Source = Content, Path = new PropertyPath(info.Name), Mode = info.CanWrite ? BindingMode.TwoWay : BindingMode.OneWay };
            }
            BindingOperations.SetBinding(this, ValueProperty, binding);
            _isSettingValueBindingInternal = false;
            _enterValue = Value;
          }
        }
        else
        {
          WasDoubleClicked = false;
          SelectionChanged = false;
          if (_textBox != null)
          {
            _textBox.PreviewTextInput -= new TextCompositionEventHandler(TextBox_PreviewTextInput);
            _textBox.TextChanged -= new TextChangedEventHandler(TextBox_TextChanged);
            //_textBox.PreviewLostKeyboardFocus -= new KeyboardFocusChangedEventHandler(TextBox_PreviewLostKeyboardFocus);
          }

          IPropertyInfo info = Column.PropertyInfo;
          if (info != null)
          {
            BindingPropertyInfoAdapter adapter = info as BindingPropertyInfoAdapter;
            if (adapter != null)
            {
              if (adapter.Binding.UpdateSourceTrigger == UpdateSourceTrigger.LostFocus)
              {
                adapter.Extractor.loseFocus();
              }
              adapter.ValueChanged -= new EventHandler(BindingPropertyInfoAdapter_ValueChanged);
            }
          }

          _isSettingValueBindingInternal = true;
          BindingOperations.ClearBinding(this, ValueProperty);
          _isSettingValueBindingInternal = false;
        }
      }
    }

    private void EnterDisplayMode()
    {
      if (Column.DisplayTemplate != null)
      {
        ContentTemplateSelector = null;
        ContentTemplate = Column.DisplayTemplate;
      }
      else if (Column.DisplayTemplateSelector != null)
      {
        ContentTemplate = null;
        ContentTemplateSelector = Column.DisplayTemplateSelector;
      }
    }

    private void BindingPropertyInfoAdapter_ValueChanged(object sender, EventArgs e)
    {
      BindingPropertyInfoAdapter adapter = sender as BindingPropertyInfoAdapter;
      if (adapter != null)
      {
        Extractor extractor = adapter.Extractor;
        ReadOnlyObservableCollection<ValidationError> errors = Validation.GetErrors(extractor);
        if (errors.Count > 0 || adapter.ValidationCache.ContainsKey(DataContext))
        {
          IsValid = false;
          if (errors.Count > 0)
          {
            ToolTip = errors[0].Exception == null ? errors[0].ErrorContent : GetInnerExceptionMessage(errors[0].Exception);
          }
        }
        else
        {
          IsValid = true;
          ToolTip = null;
        }
      }
    }

    private string GetInnerExceptionMessage(Exception exception)
    {
      if (exception != null)
      {
        if (exception.InnerException == null)
        {
          return exception.Message;
        }
        return GetInnerExceptionMessage(exception.InnerException);
      }
      return null;
    }

    private static readonly DependencyPropertyKey IsEditingPropertyKey =
        DependencyProperty.RegisterReadOnly("IsEditing", typeof(bool), typeof(DataGridCellContainer), new UIPropertyMetadata(false));

    /// <summary>
    /// Identifies the <see cref="IsEditing"/> property.
    /// </summary>
    public static readonly DependencyProperty IsEditingProperty =
        IsEditingPropertyKey.DependencyProperty;

    private delegate void AttemptToFocusEditorDelegate(string input);

    private void AttemptToFocusEditor(string input)
    {
      // TODO: refactor and add support for other editor types. DateTimePicker, ColorPicker, etc
      /*ContentPresenter presenter = VisualTreeUtils.GetChild<ContentPresenter>(this);
      if (presenter != null)
      {
        if (VisualTreeHelper.GetChildrenCount(presenter) > 0)
        {
          UIElement editor = VisualTreeHelper.GetChild(presenter, 0) as UIElement;
          if (editor != null)
          {
            editor.Focus();
          }
        }
      }*/
      DropDownDatePicker datePicker = VisualTreeUtils.GetChild<DropDownDatePicker>(this);
      if (datePicker != null)
      {
        // TODO: there is a slight issue with this. Holding down Enter or Shift+Enter on a DatTime column can cause minor problems.
        datePicker.Focus();
      }
      else
      {
        FilteringTextBoxBase filteringTextBox = VisualTreeUtils.GetChild<FilteringTextBoxBase>(this);
        if (filteringTextBox != null && input != null)
        {
          filteringTextBox.Focus();
          filteringTextBox.Text = input;
        }
        TextBox textBox = VisualTreeUtils.GetChild<TextBox>(this);
        if (textBox != null)
        {
          textBox.PreviewTextInput += new TextCompositionEventHandler(TextBox_PreviewTextInput);
          textBox.TextChanged += new TextChangedEventHandler(TextBox_TextChanged);
          //textBox.PreviewLostKeyboardFocus += new KeyboardFocusChangedEventHandler(TextBox_PreviewLostKeyboardFocus);
          _textBox = textBox;
          textBox.Focus();
          if (input == null)
          {
            textBox.SelectionStart = 0;
            textBox.SelectionLength = textBox.Text.Length;
          }
          else
          {
            textBox.Text = input;
            textBox.SelectionStart = input.Length;
            textBox.SelectionLength = 0;
          }
        }
      }
      // If this cell is inside an auto-sizing column, then invalidate measure when switching in and out of edit mode:
      if (Row != null && Row.DataGrid != null && Column != null && Column.Width.IsAuto && Row.DataGrid.AutoColumnWidthBehavior == AutoColumnWidthBehavior.Dynamic)
      {
        Row.DataGrid.DataGridPanel.InvalidateMeasure();
      }
      _presenter = VisualTreeUtils.GetChild<ContentPresenter>(this);
      if (_presenter != null)
      {
        _presenter.SizeChanged += new SizeChangedEventHandler(Presenter_SizeChanged);
      }
    }

    /*private void TextBox_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
      if (Column != null)
      {
        IPropertyInfo info = Column.PropertyInfo;
        if (info != null)
        {
          BindingPropertyInfoAdapter adapter = info as BindingPropertyInfoAdapter;
          if (adapter != null)
          {
            if (adapter.Binding.UpdateSourceTrigger == UpdateSourceTrigger.LostFocus)
            {
              adapter.Extractor.loseFocus();
            }
            //adapter.ValueChanged -= new EventHandler(BindingPropertyInfoAdapter_ValueChanged);
          }
        }
      }
    }*/

    private TextBox _textBox;

    private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
      TextBox textBox = sender as TextBox;
      _typed = true;
      _userInput = e.Text.StartsWith("0") ? "0" : textBox.Text;
    }

    private bool _typed;
    private string _userInput;

    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
      TextBox textBox = sender as TextBox;
      if (_typed)
      {
        if (textBox.Text.StartsWith("0") && !_userInput.StartsWith("0"))
        {
          textBox.SelectionStart = Math.Min(textBox.Text.Length, textBox.SelectionStart + 1);
        }
      }
      _typed = false;
    }

    private ContentPresenter _presenter;

    private void Presenter_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      Row.DataGrid.DataGridPanel.InvalidateMeasure();
    }

    private object _enterValue;

    internal void CancelEditMode()
    {
      Value = _enterValue;
      SetIsEditing(false);
    }

    #endregion // IsEditing Property

    //internal bool IsQuickEditing { get; set; }
    internal bool WasDoubleClicked { get; set; }
    internal bool SelectionChanged { get; set; } // So that when edit mode navigation is enabled, using the mouse to change the caret position temporarily disbales edit mode navigation.

    #region IsValid Property

    /// <summary>
    /// Gets whether the current cell value is valid.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsValidProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool IsValid
    {
      get { return (bool)GetValue(IsValidProperty); }
      internal set
      {
        if (IsValid != value)
        {
          SetValue(IsValidPropertyKey, value);
        }
      }
    }

    private static readonly DependencyPropertyKey IsValidPropertyKey =
        DependencyProperty.RegisterReadOnly("IsValid", typeof(bool), typeof(DataGridCellContainer), new UIPropertyMetadata(true));

    /// <summary>
    /// Identifies the <see cref="IsValid"/> property.
    /// </summary>
    public static readonly DependencyProperty IsValidProperty =
        IsValidPropertyKey.DependencyProperty;

    #endregion // IsValid Property

    #region Value Property

    // This is the actual value that is displayed in this cell.
    // This value is based on the Content property (that is the object held by the row) and a binding based on the Column.
    // This value is for listening to changes in the cell data to check validation.

    /// <summary>
    /// Gets or sets the cell value. This property is only set when the cell is in edit mode.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="ValueProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    internal object Value
    {
      get { return GetValue(ValueProperty); }
      set { SetValue(ValueProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Value"/> property.
    /// </summary>
    internal static readonly DependencyProperty ValueProperty =
      DependencyProperty.Register("Value", typeof(object), typeof(DataGridCellContainer),
      new FrameworkPropertyMetadata(OnValueChanged));

    private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataGridCellContainer)d).OnValueChanged();
    }

    private void OnValueChanged()
    {
      if (!_isSettingValueBindingInternal)
      {
        // If this cell is in an auto-sizing column, then invalidate measure when the value changes:
        if (Row != null && Row.DataGrid != null && Column != null && Column.Width.IsAuto && Row.DataGrid.AutoColumnWidthBehavior == AutoColumnWidthBehavior.Dynamic)
        {
          Row.DataGrid.DataGridPanel.InvalidateMeasure();
        }

        _previouslyFocusedElement = Keyboard.FocusedElement as DependencyObject;
        if (_previouslyFocusedElement == null)
        {
          _previouslyFocusedElement = _textBox;
        }
        Validate();

        DataGrid grid = VisualTreeUtils.FindContaining<DataGrid>(this);
        if (grid != null)
        {
          grid.UpdateAggregate(Column);
          DataGridValidateCellEventArgs args = grid.OnCellValueChanged(this);
          if (args.Handled)
          {
            IsValid = args.IsValid;
            ToolTip = args.ValidationMessage;
          }
        }
      }
    }

    #endregion // Value Property

    private DependencyObject _previouslyFocusedElement;
    private bool _hasErrorsLostFocus;

    private void Validate()
    {
      if (_previouslyFocusedElement != null)
      {
        ReadOnlyObservableCollection<ValidationError> errors = Validation.GetErrors(_previouslyFocusedElement);
        if (errors.Count > 0)
        {
          IsValid = false;
          if (errors.Count > 0)
          {
            BindingExpression bindingExpression = errors[0].BindingInError as BindingExpression;
            if (bindingExpression != null)
            {
              Binding binding = bindingExpression.ParentBinding;
              if (binding != null && (binding.UpdateSourceTrigger == UpdateSourceTrigger.LostFocus || binding.UpdateSourceTrigger == UpdateSourceTrigger.Default))
              {
                _hasErrorsLostFocus = true;
              }
            }
            ToolTip = errors[0].Exception == null ? errors[0].ErrorContent : GetInnerExceptionMessage(errors[0].Exception);
          }
        }
        else if (_hasErrorsLostFocus)
        {
          _hasErrorsLostFocus = false;
          IsValid = true;
          ToolTip = null;
        }
      }
    }

    #region Column Property

    /// <summary>
    /// Gets or sets the <see cref="DataGridColumn"/> that this <see cref="DataGridCellContainer"/> belongs to.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="ColumnProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public DataGridColumn Column
    {
      get { return (DataGridColumn)GetValue(ColumnProperty); }
      set { SetValue(ColumnProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Column"/> property.
    /// </summary>
    public static readonly DependencyProperty ColumnProperty =
      DependencyProperty.Register("Column", typeof(DataGridColumn), typeof(DataGridCellContainer));

    #endregion // Column Property

    #region Row Property

    /// <summary>
    /// Gets or sets the <see cref="DataGridRow"/> that this <see cref="DataGridCellContainer"/> is displayed in.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="RowProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public DataGridRow Row
    {
      get { return (DataGridRow)GetValue(RowProperty); }
      set { SetValue(RowProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Row"/> property.
    /// </summary>
    public static readonly DependencyProperty RowProperty =
      DependencyProperty.Register("Row", typeof(DataGridRow), typeof(DataGridCellContainer));

    #endregion // Row Property

    #region IsExpandableCell Property

    /// <summary>
    /// Gets whether or not this <see cref="DataGridCellContainer"/> supports expandable content.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsExpandableCellProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool IsExpandableCell
    {
      get { return (bool)GetValue(IsExpandableCellProperty); }
    }

    // For now this needs to be a dependancy property because it is used in a trigger.
    // By having this redundant field, the performance is improved.
    private bool _isExpandableCell;

    internal void SetIsExpandableCell(bool isExpandable)
    {
      if (_isExpandableCell != isExpandable)
      {
        _isExpandableCell = isExpandable;
        SetValue(IsExpandableCellPropertyKey, isExpandable);
      }
    }

    private static readonly DependencyPropertyKey IsExpandableCellPropertyKey =
        DependencyProperty.RegisterReadOnly("IsExpandableCell", typeof(bool), typeof(DataGridCellContainer), new UIPropertyMetadata(false));

    /// <summary>
    /// Identifies the <see cref="IsExpandableCell"/> property.
    /// </summary>
    public static readonly DependencyProperty IsExpandableCellProperty =
        IsExpandableCellPropertyKey.DependencyProperty;

    #endregion // IsExpandableCell Property
    
    /*
    #region HasChildren Property

    /// <summary>
    /// Gets whether or not the expander toggle button should be visible.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="HasChildrenProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool HasChildren
    {
      get { return (bool)GetValue(HasChildrenProperty); }
    }

    internal void SetHasChildren(bool hasChildren)
    {
      SetValue(HasChildrenPropertyKey, hasChildren);
    }

    private static readonly DependencyPropertyKey HasChildrenPropertyKey =
        DependencyProperty.RegisterReadOnly("HasChildren", typeof(bool), typeof(DataGridCellContainer), new UIPropertyMetadata(false));

    /// <summary>
    /// Identifies the <see cref="HasChildren"/> property.
    /// </summary>
    public static readonly DependencyProperty HasChildrenProperty =
        HasChildrenPropertyKey.DependencyProperty;

    #endregion // HasChildren Property

    #region IsExpanded Property

    /// <summary>
    /// Gets or sets whether or not this <see cref="DataGridCellContainer"/> is expanded.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsExpandedProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool IsExpanded
    {
      get { return (bool)GetValue(IsExpandedProperty); }
      set { SetValue(IsExpandedProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="IsExpanded"/> property.
    /// </summary>
    public static readonly DependencyProperty IsExpandedProperty =
      DependencyProperty.Register("IsExpanded", typeof(bool), typeof(DataGridCellContainer),
      new FrameworkPropertyMetadata(OnIsExpandedChanged));

    private static void OnIsExpandedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataGridCellContainer)d).OnIsExpandedChanged();
    }

    private void OnIsExpandedChanged()
    {
      if (Row != null && Row.ItemWrapper != null)
      {
        Row.ItemWrapper.IsExpanded = IsExpanded;
      }
    }

    #endregion // IsExpanded Property

    #region Level Property

    /// <summary>
    /// Gets the hierarchical level of this <see cref="DataGridCellContainer"/>.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="LevelProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public int Level
    {
      get { return (int)GetValue(LevelProperty); }
    }

    internal void SetLevel(int level)
    {
      SetValue(LevelPropertyKey, level);
    }

    private static readonly DependencyPropertyKey LevelPropertyKey =
        DependencyProperty.RegisterReadOnly("Level", typeof(int), typeof(DataGridCellContainer), new UIPropertyMetadata(0));

    /// <summary>
    /// Identifies the <see cref="Level"/> property.
    /// </summary>
    public static readonly DependencyProperty LevelProperty =
        LevelPropertyKey.DependencyProperty;

    #endregion // Level Property
    */
  }
}
