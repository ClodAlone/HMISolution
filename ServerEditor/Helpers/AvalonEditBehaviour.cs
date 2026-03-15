using System;
using System.Windows;
using ICSharpCode.AvalonEdit;

namespace ServerEditor.Helpers
{
    public class AvalonEditBehaviour
    {
        public static readonly DependencyProperty AvalonEditTextProperty =
            DependencyProperty.RegisterAttached("AvalonEditText", typeof(string), typeof(AvalonEditBehaviour),
            new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, PropertyChangedCallback));

        public static string GetAvalonEditText(DependencyObject obj)
        {
            return (string)obj.GetValue(AvalonEditTextProperty);
        }

        public static void SetAvalonEditText(DependencyObject obj, string value)
        {
            obj.SetValue(AvalonEditTextProperty, value);
        }

        public static readonly DependencyProperty SelectionStartProperty =
            DependencyProperty.RegisterAttached("SelectionStart", typeof(int), typeof(AvalonEditBehaviour),
            new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectionChanged));

        public static readonly DependencyProperty SelectionLengthProperty =
            DependencyProperty.RegisterAttached("SelectionLength", typeof(int), typeof(AvalonEditBehaviour),
            new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectionChanged));

        public static int GetSelectionStart(DependencyObject obj) => (int)obj.GetValue(SelectionStartProperty);
        public static void SetSelectionStart(DependencyObject obj, int value) => obj.SetValue(SelectionStartProperty, value);

        public static int GetSelectionLength(DependencyObject obj) => (int)obj.GetValue(SelectionLengthProperty);
        public static void SetSelectionLength(DependencyObject obj, int value) => obj.SetValue(SelectionLengthProperty, value);

        private static void OnSelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextEditor editor)
            {
                int start = GetSelectionStart(editor);
                int length = GetSelectionLength(editor);
                
                if (start >= 0 && length >= 0 && start + length <= editor.Document.TextLength)
                {
                    if (editor.SelectionStart != start || editor.SelectionLength != length)
                    {
                        editor.Select(start, length);
                        editor.ScrollTo(editor.TextArea.Selection.StartPosition.Line, 0); 
                    }
                }
            }
        }

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextEditor editor)
            {
                if ((bool)d.GetValue(IsHandlerAttachedProperty) == false)
                {
                     editor.TextChanged += Editor_TextChanged;
                     d.SetValue(IsHandlerAttachedProperty, true);
                }

                string newText = (string)e.NewValue ?? "";
                
                if (editor.Text != newText)
                {
                    editor.Text = newText;
                }
            }
        }

        private static void Editor_TextChanged(object? sender, EventArgs e)
        {
             if (sender is TextEditor editor)
             {
                 SetAvalonEditText(editor, editor.Text);
             }
        }

        private static readonly DependencyProperty IsHandlerAttachedProperty =
            DependencyProperty.RegisterAttached("IsHandlerAttached", typeof(bool), typeof(AvalonEditBehaviour), new PropertyMetadata(false));
    }
}
