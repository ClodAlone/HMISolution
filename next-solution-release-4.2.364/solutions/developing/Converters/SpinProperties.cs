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

namespace Converters
{

    /// <summary>
    /// Interaction logic for AttachedProperties.xaml
    /// </summary>
    public class SpinProperties
    {
        // When set to True, Enter Key will update Source
        public static readonly DependencyProperty EnterUpdatesValueSourceProperty =
        DependencyProperty.RegisterAttached("EnterUpdatesValueSource", typeof(bool),
                                            typeof(SpinProperties),
                                            new PropertyMetadata(false, EnterUpdatesValueSourcePropertyChanged));

        // Get
        public static bool GetEnterUpdatesValueSource(DependencyObject obj)
        {
            return (bool)obj.GetValue(EnterUpdatesValueSourceProperty);
        }


        // Set
        public static void SetEnterUpdatesValueSource(DependencyObject obj, bool value)
        {
            obj.SetValue(EnterUpdatesValueSourceProperty, value);
            var _obj = obj as UIElement;
            BindingExpression valueBinding = BindingOperations.GetBindingExpression(_obj, DevExpress.Xpf.Editors.SpinEdit.EditValueProperty);
            if (valueBinding != null)
                valueBinding.UpdateTarget();
        }

        // Changed Event - Attach PreviewKeyDown handler
        private static void EnterUpdatesValueSourcePropertyChanged(DependencyObject obj,
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
            if (e.Key == Key.Enter)
            {
                if (GetEnterUpdatesValueSource((DependencyObject)sender))
                {
                    e.Handled = true;

                    var obj = sender as UIElement;
                    BindingExpression valueBinding = BindingOperations.GetBindingExpression(obj, DevExpress.Xpf.Editors.SpinEdit.EditValueProperty);

                    if (valueBinding != null)
                        valueBinding.UpdateSource();
                    obj.Focus();
                }
            }
        }

    }
}
