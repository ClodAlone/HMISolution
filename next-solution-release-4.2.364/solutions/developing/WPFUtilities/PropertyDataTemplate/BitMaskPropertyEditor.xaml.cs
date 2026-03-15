using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
//using PropertyControl.ComponentService;
using Utilities;
using Utilities.WPF;
using static WPFUtilities.PropertyDataTemplate.BitMaskEditor;

namespace WPFUtilities.PropertyDataTemplate
{
    public class model
    {
        public int? Value { get; set; }
    }

    /// <summary>
    /// Interaction logic for BitMaskPropertyEditor.xaml
    /// </summary>
    public partial class BitMaskPropertyEditor : UserControl
    {
        
        #region ShowInheritedButton
        public static readonly DependencyProperty ShowInheritedButtonProperty = DependencyProperty.Register("ShowInheritedButton", typeof(bool), typeof(BitMaskPropertyEditor), new UIPropertyMetadata(false, new PropertyChangedCallback(OnShowInheritedButtonChanged), new CoerceValueCallback(OnCoerceShowInheritedButton)));

        private static object OnCoerceShowInheritedButton(DependencyObject o, object value)
        {
            BitMaskPropertyEditor control = o as BitMaskPropertyEditor;
            if (control != null)
                return control.OnCoerceShowInheritedButton((bool)value);
            else
                return value;
        }

        private static void OnShowInheritedButtonChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            BitMaskPropertyEditor control = o as BitMaskPropertyEditor;
            if (control != null)
                control.OnShowInheritedButtonChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceShowInheritedButton(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnShowInheritedButtonChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public bool ShowInheritedButton
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(ShowInheritedButtonProperty);
            }
            set
            {
                SetValue(ShowInheritedButtonProperty, value);
            }
        }
        #endregion

        public BitMaskType bType = BitMaskType.Area;
        public BitMaskPropertyEditor()
        {
            InitializeComponent();
        }

        private void DlgButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            int? value = 0;
            if (button.Tag is int)
            {
                try
                {
                    value = (int)button.Tag;
                }
                catch (Exception ex)
                {
                    
                }
            }

            if (value == -1)
            {
                if (ShowInheritedButton)
                    value = null;
                else
                    value = 0;
            }
            var mdl = new model() { Value = value };
            var editor = new BitMaskEditor() { DataContext = mdl, Type = bType, ShowInheritedButton = ShowInheritedButton };
            var ColorDialog = new GeneralDialogContent(editor)
            {
                Owner = button.FindParent<Window>(),
                Title = (bType == BitMaskType.Area ? Properties.Resources.AccessMaskEditor : Properties.Resources.LevelMaskEditor),
                HelpLink = "AccessMaskEditor"
            };
            if (ColorDialog.ShowDialog() != true)
            {
                return;
            }
            if (mdl.Value.HasValue)
                button.Tag = mdl.Value;
            else
                button.Tag = -1;
        }
    }
}
