using DocumentManager.ComponentService;
using Mindscape.WpfElements.PropertyEditing;
using ScreenManager.ComponentService;
using StringManager.ComponentService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using TranslationHelpers;
using UFInterfaces;
using UFInterfaces.Editors;
using Utilities;
using Utilities.WPF;

namespace WPFUtilities.PropertyDataTemplate
{
    /// <summary>
    /// Interaction logic for TextPropertyEditor.xaml
    /// </summary>
    public partial class TextPropertyEditor : UserControl
    {
        #region Dependency Properties
        #region Workspace
        public static readonly DependencyProperty WorkspaceProperty = DependencyProperty.Register("Workspace", typeof(IWorkspace), typeof(TextPropertyEditor), new UIPropertyMetadata(null, new PropertyChangedCallback(OnWorkspaceChanged), new CoerceValueCallback(OnCoerceWorkspace)));

        private static object OnCoerceWorkspace(DependencyObject o, object value)
        {
            TextPropertyEditor control = o as TextPropertyEditor;
            if (control != null)
                return control.OnCoerceWorkspace((IWorkspace)value);
            else
                return value;
        }

        private static void OnWorkspaceChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            TextPropertyEditor control = o as TextPropertyEditor;
            if (control != null)
                control.OnWorkspaceChanged((IWorkspace)e.OldValue, (IWorkspace)e.NewValue);
        }

        protected virtual IWorkspace OnCoerceWorkspace(IWorkspace value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnWorkspaceChanged(IWorkspace oldValue, IWorkspace newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public IWorkspace Workspace
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (IWorkspace)GetValue(WorkspaceProperty);
            }
            set
            {
                SetValue(WorkspaceProperty, value);
            }
        }

        #endregion

        #region BindingPath
        public static readonly DependencyProperty BindingPathProperty = DependencyProperty.Register("BindingPath", typeof(string), typeof(TextPropertyEditor), new UIPropertyMetadata(null, new PropertyChangedCallback(OnBindingPathChanged), new CoerceValueCallback(OnCoerceBindingPath)));

        private static object OnCoerceBindingPath(DependencyObject o, object value)
        {
            TextPropertyEditor control = o as TextPropertyEditor;
            if (control != null)
                return control.OnCoerceBindingPath((string)value);
            else
                return value;
        }

        private static void OnBindingPathChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            TextPropertyEditor control = o as TextPropertyEditor;
            if (control != null)
                control.OnBindingPathChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual string OnCoerceBindingPath(string value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnBindingPathChanged(string oldValue, string newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue)
            {
                var bindingValue = new Binding()
                {
                    Path = new PropertyPath(newValue ?? "Value"),
                    Mode = BindingMode.TwoWay,
                    ValidatesOnDataErrors = true
                };
                BindingOperations.SetBinding(uri, TagProperty, bindingValue);
            }
        }

        public string BindingPath
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(BindingPathProperty);
            }
            set
            {
                SetValue(BindingPathProperty, value);
            }
        }
        #endregion

        #region TextUIElement
        public static readonly DependencyProperty TextUIElementProperty = DependencyProperty.Register("TextUIElement", typeof(string), typeof(TextPropertyEditor), new UIPropertyMetadata(null, new PropertyChangedCallback(OnTextUIElementChanged), new CoerceValueCallback(OnCoerceTextUIElement)));

        private static object OnCoerceTextUIElement(DependencyObject o, object value)
        {
            TextPropertyEditor TextPropertyEditor = o as TextPropertyEditor;
            if (TextPropertyEditor != null)
                return TextPropertyEditor.OnCoerceTextUIElement((string)value);
            else
                return value;
        }

        private static void OnTextUIElementChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            TextPropertyEditor TextPropertyEditor = o as TextPropertyEditor;
            if (TextPropertyEditor != null)
                TextPropertyEditor.OnTextUIElementChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual string OnCoerceTextUIElement(string value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnTextUIElementChanged(string oldValue, string newValue)
        {
            if (newValue != null && oldValue != newValue)
            {
                var bindingValue = new Binding()
                {
                    Path = new PropertyPath(newValue),
                    Mode = BindingMode.OneWay,
                    ValidatesOnDataErrors = true
                };
                BindingOperations.SetBinding(text, TagProperty, bindingValue);
            }
        }

        public string TextUIElement
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(TextUIElementProperty);
            }
            set
            {
                SetValue(TextUIElementProperty, value);
            }
        }

        UIElement element;
        #endregion

        #region PropertyName
        public static readonly DependencyProperty PropertyNameProperty = DependencyProperty.Register("PropertyName", typeof(string), typeof(TextPropertyEditor), new UIPropertyMetadata(null, new PropertyChangedCallback(OnPropertyNameChanged), new CoerceValueCallback(OnCoercePropertyName)));

        private static object OnCoercePropertyName(DependencyObject o, object value)
        {
            TextPropertyEditor TextPropertyEditor = o as TextPropertyEditor;
            if (TextPropertyEditor != null)
                return TextPropertyEditor.OnCoercePropertyName((string)value);
            else
                return value;
        }

        private static void OnPropertyNameChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            TextPropertyEditor TextPropertyEditor = o as TextPropertyEditor;
            if (TextPropertyEditor != null)
                TextPropertyEditor.OnPropertyNameChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual string OnCoercePropertyName(string value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnPropertyNameChanged(string oldValue, string newValue)
        {
            if (newValue != null && oldValue != newValue)
            {
                var bindingValue = new Binding()
                {
                    Path = new PropertyPath(newValue),
                    Mode = BindingMode.OneWay,
                    ValidatesOnDataErrors = true
                };
                BindingOperations.SetBinding(clear, TagProperty, bindingValue);
            }
        }

        public string PropertyName
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(PropertyNameProperty);
            }
            set
            {
                SetValue(PropertyNameProperty, value);
            }
        }

        string property;
        #endregion

        #region HideEditButton
        public static readonly DependencyProperty HideEditButtonProperty = DependencyProperty.Register("HideEditButton", typeof(bool), typeof(TextPropertyEditor), new UIPropertyMetadata(false, new PropertyChangedCallback(OnHideEditButtonChanged), new CoerceValueCallback(OnCoerceHideEditButton)));

        private static object OnCoerceHideEditButton(DependencyObject o, object value)
        {
            TextPropertyEditor TextPropertyEditor = o as TextPropertyEditor;
            if (TextPropertyEditor != null)
                return TextPropertyEditor.OnCoerceHideEditButton((bool)value);
            else
                return value;
        }

        private static void OnHideEditButtonChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            TextPropertyEditor TextPropertyEditor = o as TextPropertyEditor;
            if (TextPropertyEditor != null)
                TextPropertyEditor.OnHideEditButtonChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceHideEditButton(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnHideEditButtonChanged(bool oldValue, bool newValue)
        {
            
        }        

        public bool HideEditButton
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(HideEditButtonProperty);
            }
            set
            {
                SetValue(HideEditButtonProperty, value);
            }
        }
        #endregion
        #endregion

        #region Declarations
        List<UIElement> uiElements = new List<UIElement>();
        IScreenManager screenManager;
        bool bDirty;
        bool bInit;
        bool bIsToolTip;
        bool bCustomStrings;
        bool bHideEditButton;
        private object newValue;
        #endregion

        #region Constructors

        public TextPropertyEditor()
        {
            InitializeComponent();

            //DataContextChanged += (o, e) =>
            //{ if (e.NewValue != null && e.NewValue is ObjectWrapper 
            Loaded += (o, e) =>
            {
                uiElements.Clear();
                element = text.Tag as UIElement;
                property = clear.Tag as string;
                bCustomStrings = element != null && !String.IsNullOrEmpty(property);

                DataObject.AddPastingHandler(text, OnPasteHandler);

                if (bCustomStrings)
                {
                    if (Workspace != null && Workspace.ContextDocument is IDocumentTranslator)
                    {
                        uiElements.Add(element);
                        var stringID = (Workspace.ContextDocument as IDocumentTranslator).GetStringID(uiElements[0], property);
                        if (stringID != null)
                        {
                            if (stringID.Item1 != property)
                            {
                                uri.SetBinding(TagProperty, new Binding
                                {
                                    Source = uiElements[0],
                                    Path = new PropertyPath(stringID.Item1),
                                    Mode = BindingMode.TwoWay
                                });
                            }

                            uri.Tag = stringID.Item2;
                            text.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
                            text.SelectAll();
                        }
                    }
                }
                else {
                    if (VisualParent is ContentPresenter && String.IsNullOrEmpty(BindingPath))
                    {
                        var wrapper = (VisualParent as ContentPresenter).Content as ObjectWrapper;
                        if (wrapper == null || (wrapper.PropertyName == "Content" && uri.Tag != null && !(uri.Tag is String)))
                        {
                            mainGrid.IsEnabled = false;
                            BindingOperations.ClearBinding(text, TextBox.TextProperty);
                            text.Text = String.Empty;
                            return;
                        }
                        else
                        {
                            if (Workspace != null && Workspace.ContextDocument is IDocumentTranslator)
                            {
                                if (!(wrapper.UnderlyingObject is Many))
                                {
                                    bIsToolTip = wrapper.PropertyName == "ToolTip";
                                    var elem = (wrapper.UnderlyingObject as ICustomTypeDescriptor)?.GetPropertyOwner(wrapper.Property.Property.AsPropertyDescriptor) as UIElement;
                                    if (elem != null)
                                        uiElements.Add(elem);
                                }
                                else
                                {
                                    var many = (wrapper.UnderlyingObject as ICustomTypeDescriptor)?.GetPropertyOwner(wrapper.Property.Property.AsPropertyDescriptor) as Many;
                                    if (many != null)
                                    {
                                        bIsToolTip = many.PropertyName == "ToolTip";
                                        foreach (var wrapped in many.Properties)
                                        {
                                            var elem = wrapped.Wrapped as UIElement;
                                            if (elem != null)
                                                uiElements.Add(elem);
                                        }
                                    }
                                }
                                if (uiElements.Count == 1)
                                {
                                    var stringID = (Workspace.ContextDocument as IDocumentTranslator).GetStringID(uiElements[0], wrapper.PropertyName);
                                    if (stringID != null)
                                    {
                                        if (stringID.Item1 != wrapper.PropertyName)
                                        {
                                            uri.SetBinding(TagProperty, new Binding
                                            {
                                                Source = uiElements[0],
                                                Path = new PropertyPath(stringID.Item1),
                                                Mode = BindingMode.TwoWay
                                            });
                                        }

                                        uri.Tag = stringID.Item2;
                                        text.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
                                        text.SelectAll();
                                    }
                                }
                            }
                        }
                    }
                }
                
                if(HideEditButton)
                {
                    ShowHideEditButton();
                }

                bInit = true;
                
            };
            DataContextChanged += (o, e) =>
            {
                if (bInit)
                    UpdateStringId(false);
                newValue = e.NewValue;
            };            
        }

        #endregion

        #region Commands

        private void DlgButton_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            Button button = (Button)sender;

            if (Workspace != null)
            {
                var doc = Workspace.ContextDocument as IDocument;
                if (doc == null)
                    return;

                IStringEditorManager stringEditorManager = doc.GetService(typeof(IStringEditorManager)) as IStringEditorManager;
                if (stringEditorManager == null)
                    return;

                var value = button.Tag as String;
                var stringEditor = stringEditorManager.GetStringEditor(doc, true);
                stringEditor.DataContext = value;

                var Dialog = new GeneralDialogContent(stringEditor)
                {
                    Owner = this.FindParent<Window>(),
                    DialogKeepContent = true,
                    Title = Properties.Resources.SelectStringEditor,
                    HelpLink = "StringEditor"
                };
                if (Dialog.ShowDialog() != true)
                    return;


                int cursorPosition = _cursorPosition;
                int selectionLength = _selectionLength;
                _cursorPosition = -1;
                _selectionLength = -1;
                bSartSelection = false;

                if (stringEditor.DataContext != null && stringEditor.DataContext is List<string> && 
                    (stringEditor.DataContext as List<string>).Count > 0)
                {
                    var list = stringEditor.DataContext as List<string>;
                    string newValue =  string.Empty;
                    list.ForEach(item => newValue = $"{newValue}{{{item}}}");

                    if (cursorPosition == -1)
                    {
                        value = string.IsNullOrEmpty(value) ? list.Count > 1 ? newValue : list.FirstOrDefault() : !TranslationHelper.IsComposed(value) ? $"{{{value}}}{newValue}" : $"{value}{newValue}";
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(value))
                            value = list.Count > 1 ? newValue : list.FirstOrDefault();
                        else if (cursorPosition < value.Length)
                        {
                            if(selectionLength > 0)
                                value = value.Remove(cursorPosition, Math.Min(value.Length - cursorPosition, selectionLength));
                            if (string.IsNullOrEmpty(value))
                                value = list.Count > 1 ? newValue : list.FirstOrDefault();
                            else
                                value = value.Insert(cursorPosition, newValue);
                        }
                        else
                            value = !TranslationHelper.IsComposed(value) ? $"{{{value}}}{newValue}" : $"{value}{newValue}";
                    }

                    button.Tag = value;
                    text.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
                    text.SelectAll();
                    text.Focus();
                    UpdateStringId(true);
                }
                else
                    text.Focus();
            }
        }

        private void DlgButton_Click_Clear(object sender, RoutedEventArgs e)
        {
            uri.Tag = null;
            text.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
            UpdateStringId(true);
            _cursorPosition = -1;
            _selectionLength = -1;
            bSartSelection = false;
        }
        bool bSartSelection;
        int _cursorPosition = -1;
        int _selectionLength = -1;
        void OnGotFocus(object sender, RoutedEventArgs e)
        {
            bDirty = false;
            bSartSelection = true;
        }

        void OnTextChanged(object sender, RoutedEventArgs e)
        {
            if (bInit)
                bDirty = true;
        }

        void OnLostFocus(object sender, RoutedEventArgs e)
        {
            if (bInit)
                UpdateStringId(bDirty);
            if(bSartSelection)
            {
                bSartSelection = false;
                _cursorPosition = text.SelectionStart;
                _selectionLength = text.SelectionLength;
            }
            uri.Tag = uri?.Tag?.ToString().Replace("\\n", System.Environment.NewLine);
        }

        void UpdateStringId(bool bAlreadyUntranslated)
        {
            if (Workspace == null || !(Workspace.ContextDocument is IDocumentTranslator) || uiElements.Count == 0)
                return;

            if (screenManager == null)
                screenManager = Workspace.ContextDocument.GetService(typeof(IScreenManager)) as IScreenManager;
            foreach (var elem in uiElements)
            {
                screenManager.UpdateStringId(elem, bAlreadyUntranslated, !bIsToolTip, bIsToolTip);
            }
            bDirty = false;
        }

        private void OnPreviewKeyDownNewLine(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
            {
                e.Handled = true;

                var obj = sender as TextBox;
                if(obj != null)
                {
                    var selStart = obj.SelectionStart;
                    obj.Text = obj.Text.Insert(selStart, "\n");
                    obj.CaretIndex = selStart + 1;
                    obj.Focus();
                }
            }
        }

        private void OnPasteHandler(object sender, DataObjectPastingEventArgs e)
        {
            e.CancelCommand();

            var isText = e.SourceDataObject.GetDataPresent(DataFormats.UnicodeText, true);
            if (!isText)
                return;
            var pasteText = e.SourceDataObject.GetData(DataFormats.UnicodeText) as string;

            var selStart = text.SelectionStart;
            if (text.SelectionLength > 0)
            {
                text.Text = text.Text.Remove(selStart, text.SelectionLength);
            }
            Tag = text.Text = text.Text.Insert(selStart, pasteText);
            text.CaretIndex = selStart + pasteText.Length;
        }

        #endregion

        private void ShowHideEditButton()
        {            
            var wrapper = newValue as Mindscape.WpfElements.PropertyEditing.ObjectWrapper;
            if (wrapper.UnderlyingObject is ICustomTypeDescriptor)
            {
                var typeDesc = wrapper.UnderlyingObject as ICustomTypeDescriptor;
                var properties = typeDesc.GetProperties();
                var found = properties.Find("Type", true);
                if (found != null)
                {
                    var type = found.GetValue(typeDesc);

                    if (type.ToString() == "AppendValueFromString")
                    {
                        uri.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        uri.Visibility = Visibility.Collapsed;
                    }
                }
            }           
        }
    }
}
