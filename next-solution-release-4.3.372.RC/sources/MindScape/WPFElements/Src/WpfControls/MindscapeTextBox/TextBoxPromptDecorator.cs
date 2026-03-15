using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Diagnostics;
using System.Windows.Data;
using System.ComponentModel;
using Infralution.Licensing;
using System.Windows.Input;
using System.Windows.Documents;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Adds a prompt to another control, typically a TextBox.  The prompt is displayed when 
  /// the contained control is empty, and disappears when the user clicks into the
  /// contained control.
  /// </summary>
  public class PromptDecorator : ContentControl
  {
    private const string DefaultChildContentPropertyName = "Text";
    private bool _childHasFocus;
    private bool _childHasDefaultProperty;

    static PromptDecorator()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(PromptDecorator),
        new FrameworkPropertyMetadata(typeof(PromptDecorator)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PromptDecorator"/> class.
    /// </summary>
    public PromptDecorator()
    {
    }

    /// <summary>
    /// Gets whether the prompt is currently displayed.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsPromptVisibleProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool IsPromptVisible
    {
      get { return (bool)GetValue(IsPromptVisibleProperty); }
    }

    private static readonly DependencyPropertyKey IsPromptVisiblePropertyKey =
        DependencyProperty.RegisterReadOnly("IsPromptVisible", typeof(bool), typeof(PromptDecorator), new UIPropertyMetadata(true));

    /// <summary>
    /// Identifies the <see cref="IsPromptVisible"/> property.
    /// </summary>
    public static readonly DependencyProperty IsPromptVisibleProperty =
        IsPromptVisiblePropertyKey.DependencyProperty;

    /// <summary>
    /// Gets or sets the prompt to be displayed.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="PromptProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public string Prompt
    {
      get { return (string)GetValue(PromptProperty); }
      set { SetValue(PromptProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Prompt"/> property.
    /// </summary>
    public static readonly DependencyProperty PromptProperty =
      DependencyProperty.Register("Prompt", typeof(string), typeof(PromptDecorator),
      new FrameworkPropertyMetadata(String.Empty));

    /// <summary>
    /// Gets or sets a DataTemplate to format the prompt or display additional
    /// prompt content such as a graphic.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="PromptTemplateProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public DataTemplate PromptTemplate
    {
      get { return (DataTemplate)GetValue(PromptTemplateProperty); }
      set { SetValue(PromptTemplateProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="PromptTemplate"/> property.
    /// </summary>
    public static readonly DependencyProperty PromptTemplateProperty =
      DependencyProperty.Register("PromptTemplate", typeof(DataTemplate), typeof(PromptDecorator),
      new FrameworkPropertyMetadata(null));


    /// <summary>
    /// Gets or sets whether the prompt remain shown when the contained element
    /// has focus.  By default, the prompt is hidden when the user gives focus to
    /// the contained element.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="ShowIfFocusedProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool ShowIfFocused
    {
      get { return (bool)GetValue(ShowIfFocusedProperty); }
      set { SetValue(ShowIfFocusedProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ShowIfFocused"/> property.
    /// </summary>
    public static readonly DependencyProperty ShowIfFocusedProperty =
      DependencyProperty.Register("ShowIfFocused", typeof(bool), typeof(PromptDecorator),
      new FrameworkPropertyMetadata(false));

    /// <summary>
    /// Gets or sets the property of the contained element which is used to determine
    /// if the contained element is empty (for purposes of displaying the prompt).
    /// If this is not specified, and the contained element has a Text property, then
    /// the Text property is used.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="ChildContentPropertyNameProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public string ChildContentPropertyName
    {
      get { return (string)GetValue(ChildContentPropertyNameProperty); }
      set { SetValue(ChildContentPropertyNameProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ChildContentPropertyName"/> property.
    /// </summary>
    public static readonly DependencyProperty ChildContentPropertyNameProperty =
      DependencyProperty.Register("ChildContentPropertyName", typeof(string), typeof(PromptDecorator),
      new FrameworkPropertyMetadata(null, OnChildContentPropertyNameChanged));

    private static void OnChildContentPropertyNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((PromptDecorator)d).OnChildContentPropertyNameChanged();
    }

    private void OnChildContentPropertyNameChanged()
    {
      BindPropertyToChildControlProperty();
    }

    /// <summary>
    /// Called by the framework when the Content property changes.
    /// </summary>
    /// <param name="oldContent">The old content.</param>
    /// <param name="newContent">The new content.</param>
    protected override void OnContentChanged(object oldContent, object newContent)
    {
      base.OnContentChanged(oldContent, newContent);

      UIElement oldElement = oldContent as UIElement;
      if (oldElement != null)
      {
        oldElement.GotFocus -= new RoutedEventHandler(Element_FocusChanged);
        oldElement.LostFocus -= new RoutedEventHandler(Element_FocusChanged);
        oldElement.GotKeyboardFocus -= new KeyboardFocusChangedEventHandler(Element_KeyboardFocusChanged);
        oldElement.LostKeyboardFocus -= new KeyboardFocusChangedEventHandler(Element_KeyboardFocusChanged);
        RichTextBox box = oldElement as RichTextBox;
        if (box != null)
        {
          box.TextChanged -= new TextChangedEventHandler(Box_TextChanged);
        }
      }

      _childHasFocus = false;
      _childHasDefaultProperty = false;

      UIElement newElement = newContent as UIElement;
      if (newElement != null)
      {
        newElement.GotFocus += new RoutedEventHandler(Element_FocusChanged);
        newElement.LostFocus += new RoutedEventHandler(Element_FocusChanged);
        newElement.GotKeyboardFocus += new KeyboardFocusChangedEventHandler(Element_KeyboardFocusChanged);
        newElement.LostKeyboardFocus += new KeyboardFocusChangedEventHandler(Element_KeyboardFocusChanged);
        RichTextBox box = newElement as RichTextBox;
        if (box != null)
        {
          box.TextChanged += new TextChangedEventHandler(Box_TextChanged);
        }
        _childHasFocus = newElement.IsFocused;
        _childHasDefaultProperty = newElement.GetType().GetProperty(DefaultChildContentPropertyName) != null;
        BindPropertyToChildControlProperty();
        UpdateIsPromptVisible();
      }
    }

    private void Box_TextChanged(object sender, TextChangedEventArgs e)
    {
      UpdateIsPromptVisible();
    }

    private void Element_KeyboardFocusChanged(object sender, KeyboardFocusChangedEventArgs e)
    {
      HandleFocusChanged(sender);
    }

    private void Element_FocusChanged(object sender, RoutedEventArgs e)
    {
      HandleFocusChanged(sender);
    }

    private void HandleFocusChanged(object sender)
    {
      UIElement element = sender as UIElement;
      if (element is ComboBox)
      {
        ComboBox box = element as ComboBox;
        TextBox textBox = VisualTreeUtils.GetChild<TextBox>(box);
        element = textBox;
      }
      if (element != null)
      {
        _childHasFocus = element.IsKeyboardFocused || element.IsFocused;
        UpdateIsPromptVisible();
      }
    }

    // ChildContent: We need this for binding purposes (since we can't
    // rely on PropertyChanged), but we don't want the child text etc.
    // to appear on the decorator, so we declare the DP as private and don't
    // provide a CLR wrapper property.

    private static readonly DependencyProperty ChildContentProperty =
      DependencyProperty.Register("ChildContent", typeof(object), typeof(PromptDecorator),
      new FrameworkPropertyMetadata(OnChildContentChanged));

    private static void OnChildContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((PromptDecorator)d).OnChildContentChanged();
    }

    private void OnChildContentChanged()
    {
      UpdateIsPromptVisible();
    }

    private bool IsChildEmpty()
    {
      RichTextBox box = Content as RichTextBox;
      if (box != null)
      {
        TextRange range = new TextRange(box.Document.ContentStart.DocumentStart, box.Document.ContentEnd.DocumentEnd);
        string text = range.Text;
        bool visible = text.IndexOf("\n") == text.LastIndexOf("\n");
        visible = visible && text.IndexOf("\r") == text.LastIndexOf("\r");
        text = text.Replace("\n", "");
        text = text.Replace("\r", "");
        return (!_childHasFocus || ShowIfFocused) && text.Length == 0 && visible;
      }
      object childContent = GetValue(ChildContentProperty);
      return (childContent == null || String.Empty.Equals(childContent));
    }

    private void UpdateIsPromptVisible()
    {
      bool shouldShowPrompt = ((!_childHasFocus || ShowIfFocused) && IsChildEmpty());
      SetValue(IsPromptVisiblePropertyKey, shouldShowPrompt);
    }

    private void BindPropertyToChildControlProperty()
    {
      string property = ChildContentPropertyName;
      if (String.IsNullOrEmpty(property) && _childHasDefaultProperty)
      {
        property = DefaultChildContentPropertyName;
      }

      if (String.IsNullOrEmpty(property))
      {
        BindingOperations.ClearBinding(this, ChildContentProperty);
      }
      else
      {
        Binding binding = new Binding(property)
        {
          Source = Content,
          Mode = BindingMode.OneWay
        };
        BindingOperations.SetBinding(this, ChildContentProperty, binding);
      }
    }
  }
}
