using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using Utilities;
using Utilities.WPF;

namespace Converters
{
    /// <summary>
    /// Interaction logic for AttachedProperties.xaml
    /// </summary>
    public class TextBoxProperties
    {
        #region UnderlineTextSourceProperty
        // When set to True, Enter Key will update Source
        public static readonly DependencyProperty UnderlineTextSourceProperty =
            DependencyProperty.RegisterAttached("UnderlineTextSource", typeof(bool),
                                                typeof(TextBoxProperties),
                                                new UIPropertyMetadata(false));

        // Get
        [Obsolete("UnderlineTextSourceProperty is never used")]
        public static bool GetUnderlineTextSource(DependencyObject obj)
        {
            return (bool)obj.GetValue(UnderlineTextSourceProperty);
        }


        // Set
        [Obsolete("UnderlineTextSourceProperty is never used")]
        public static void SetUnderlineTextSource(DependencyObject obj, bool value)
        {
            obj.SetValue(UnderlineTextSourceProperty, value);
        }

        public static void ApplyUnderlineTextToChilds(DependencyObject obj)
        {
            var fe = obj as FrameworkElement;
            if (fe != null)
            {
                if (!fe.IsLoaded)
                {
                    fe.Loaded += OnControlLoaded;
                    return;
                }

                fe.ApplyTemplate();
            }

            DependencyObjectExtensions.CleanChildrenOfTypeCache(obj);
            var listControl = obj.GetVisualChildrenOfType<TextBlock>().ToList();
            listControl.ForEach(control =>
            {
                if (control.TextDecorations != null)
                {
                    bool bFound = false;
                    foreach (var item in control.TextDecorations)
                    {
                        if (item.Location == TextDecorationLocation.Underline)
                        {
                            bFound = true;
                            break;
                        }
                    }

                    if (!bFound)
                    {
                        foreach (var item in TextDecorations.Underline)
                            control.TextDecorations.Add(item);
                    }
                }
            });

            var listControlBoxes = obj.GetVisualChildrenOfType<TextBox>().ToList();
            listControlBoxes.ForEach(control =>
            {
                if (control.TextDecorations != null)
                {
                    bool bFound = false;
                    foreach (var item in control.TextDecorations)
                    {
                        if (item.Location == TextDecorationLocation.Underline)
                        {
                            bFound = true;
                            break;
                        }
                    }

                    if (!bFound)
                    {
                        foreach (var item in TextDecorations.Underline)
                            control.TextDecorations.Add(item);
                    }
                }
            });

            //if (fe != null)
            //{
            //    fe.InvalidateArrange();
            //    fe.InvalidateMeasure();
            //}
        }

        private static void OnControlLoaded(object sender, RoutedEventArgs e)
        {
            var fe = sender as FrameworkElement;
            fe.Loaded -= OnControlLoaded;
            ApplyUnderlineTextToChilds(fe);
        }
        #endregion

        #region EnterUpdatesTextSourceProperty

        // When set to True, Enter Key will update Source
        public static readonly DependencyProperty EnterUpdatesTextSourceProperty =
            DependencyProperty.RegisterAttached("EnterUpdatesTextSource", typeof(bool),
                                                typeof(TextBoxProperties),
                                                new PropertyMetadata(false, EnterUpdatesTextSourcePropertyChanged));


        // Get
        public static bool GetEnterUpdatesTextSource(DependencyObject obj)
        {
            return (bool)obj.GetValue(EnterUpdatesTextSourceProperty);
        }

        // Set
        public static void SetEnterUpdatesTextSource(DependencyObject obj, bool value)
        {
            obj.SetValue(EnterUpdatesTextSourceProperty, value);
            var _obj = obj as UIElement;
            BindingExpression textBinding = BindingOperations.GetBindingExpression(
                _obj, TextBox.TextProperty);
            if (textBinding != null)
                textBinding.UpdateTarget();
        }


        // Changed Event - Attach PreviewKeyDown handler
        private static void EnterUpdatesTextSourcePropertyChanged(DependencyObject obj,
                                                                  DependencyPropertyChangedEventArgs e)
        {
            var sender = obj as UIElement;
            if (obj != null)
            {
                if ((bool)e.NewValue)
                {
                    sender.PreviewKeyDown += OnPreviewKeyDownUpdateSourceIfEnter;
                }
                else
                {
                    sender.PreviewKeyDown -= OnPreviewKeyDownUpdateSourceIfEnter;
                }
            }
        }

        // If key being pressed is the Enter key, and EnterUpdatesTextSource is set to true, then update source for Text property
        private static void OnPreviewKeyDownUpdateSourceIfEnter(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && (Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.Control)
            {
                if (GetEnterUpdatesTextSource((DependencyObject)sender))
                {
                    e.Handled = true;

                    var obj = sender as UIElement;
                    BindingExpression textBinding = BindingOperations.GetBindingExpression(
                        obj, TextBox.TextProperty);

                    if (textBinding != null)
                        textBinding.UpdateSource();
                  
                    obj.Focus();
                    
                }
            }
        }

#endregion

        #region CTRLEnterUpdatesTextSourceProperty

        // When set to True, CTRL+Enter Key will update Source
        public static readonly DependencyProperty CTRLEnterUpdatesTextProperty =
            DependencyProperty.RegisterAttached("CTRLEnterUpdatesText", typeof(bool),
                                                typeof(TextBoxProperties),
                                                new PropertyMetadata(false, CTRLEnterUpdatesTextPropertyChanged));


        // Get
        public static bool GetCTRLEnterUpdatesText(DependencyObject obj)
        {
            return (bool)obj.GetValue(CTRLEnterUpdatesTextProperty);
        }

        // Set
        public static void SetCTRLEnterUpdatesText(DependencyObject obj, bool value)
        {
            obj.SetValue(CTRLEnterUpdatesTextProperty, value);
            var _obj = obj as UIElement;
            BindingExpression textBinding = BindingOperations.GetBindingExpression(
                _obj, TextBox.TextProperty);
            if (textBinding != null)
                textBinding.UpdateTarget();
        }


        // Changed Event - Attach PreviewKeyDown handler
        private static void CTRLEnterUpdatesTextPropertyChanged(DependencyObject obj,
                                                                  DependencyPropertyChangedEventArgs e)
        {
            var sender = obj as UIElement;
            if (obj != null)
            {
                if ((bool)e.NewValue)
                {
                    sender.PreviewKeyDown += OnPreviewKeyDownUpdateIfCTRLEnter;
                }
                else
                {
                    sender.PreviewKeyDown -= OnPreviewKeyDownUpdateIfCTRLEnter;
                }
            }
        }

        // If key being pressed is the Enter key, and CTRLEnterUpdatesText is set to true, then update source for Text property
        private static void OnPreviewKeyDownUpdateIfCTRLEnter(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                if (GetCTRLEnterUpdatesText((DependencyObject)sender))
                {
                    e.Handled = true;

                    var obj = sender as TextBox;
                    obj.AppendText("\r");
                    obj.CaretIndex = obj.Text.Length;
                    obj.Focus();
                }
            }
        }
#endregion

    }
}
