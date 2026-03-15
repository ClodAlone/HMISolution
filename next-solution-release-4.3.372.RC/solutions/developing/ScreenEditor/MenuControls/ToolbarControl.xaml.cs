using DevExpress.Xpf.Bars;
using System;
using System.Windows.Media;
using System.Windows.Controls;
using Utilities;
using System.Windows.Input;
using DevExpress.Xpf.Editors;

namespace ScreenManager.MenuControls
{
    /// <summary>
    /// Interaction logic for ToolbarControl.xaml
    /// </summary>
    public partial class ToolbarControl : BarManager
    {
        bool bSelectingFont;
        ScreenEditorView editor;
        bool bInitializing = false;

        public ScreenEditorView Editor {
            get
            {
                return editor;
            }
            set
            {
                if (value == null && editor != null)
                {
                    editor.SelectionChanged -= editor_SelectionChanged;
                    return;
                }

                if (editor != value)
                {
                    if (editor != null)
                        editor.SelectionChanged -= editor_SelectionChanged;
                    editor = value;
                    if (editor != null)
                        editor.SelectionChanged += editor_SelectionChanged;
                }
            }
        }

        public ToolbarControl()
        {
            InitializeComponent();

            Unloaded += (o, e) =>
            {
                if (editor != null)
                    editor.SelectionChanged -= editor_SelectionChanged;
            };
        }

        private void editor_SelectionChanged(object sender, EventArgs e)
        {
            bSelectingFont = true;
            fontNameBox.Tag = (sender as ScreenEditorView).GetSelectedFontFamilyName();
            fontSizeBox.Tag = (sender as ScreenEditorView).GetSelectedFontSize();
            bSelectingFont = false;
        }

        private void OnFontSizeSelectionChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            if (editor == null || bInitializing || bSelectingFont)
                return;
            ComboBoxEdit combo = sender as ComboBoxEdit;
            if (combo != null && combo.SelectedItemValue != null && (combo.SelectedItemValue as ComboBoxItem)?.Content != null)
            {
                int value;
                if (int.TryParse((combo.SelectedItemValue as ComboBoxItem).Content.ToString(), out value))
                {
                    if(value > 0)
                        editor.OnFontSizeSelectionChanged(value);
                }
            }
        }

        private void OnFontNameSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (editor == null || bInitializing || bSelectingFont)
                return;
            ComboBox combo = sender as ComboBox;
            if (combo != null && combo.SelectedValue != null && (combo.SelectedValue as ComboBoxItem)?.Content != null)
            {
                if ((combo.SelectedValue as ComboBoxItem).Content is FontFamily)
                {
                    editor.OnFontNameSelectionChanged((combo.SelectedValue as ComboBoxItem).Content as FontFamily);
                }
            }
        }

        private void fontSizeComboBox_Validate(object sender, DevExpress.Xpf.Editors.ValidationEventArgs e)
        {
            string svalue = e.Value?.ToString();
            if (string.IsNullOrEmpty(svalue)) return;

            int value;
            if (int.TryParse(svalue, out value))
            {
                if (value == 0)
                {            
                    e.IsValid = false;
                    e.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Critical;
                    e.ErrorContent = Properties.Resources.FontSizeWarning;
                }
            }
        }

        private void fontSizeComboBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (editor == null || bInitializing || bSelectingFont)
                return;

            ComboBoxEdit combo = sender as ComboBoxEdit;
            if (combo != null)
            {
                int value;
                if (int.TryParse(combo.Text, out value))
                {
                    if(value > 0)
                        editor.OnFontSizeSelectionChanged(value);
                }
            }
        }

        private void fontNameComboBox_DropDownOpened(object sender, EventArgs e)
        {
            if ((sender as ComboBox).Items.Count == 0)
                FontStyleHelper.InitializeFontComboBox(sender as ComboBox);
        }


        private void fontSizeComboBox_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            var combo = sender as ComboBoxEdit;
            if (combo.Items.Count == 0)
            {
                int value;
                if (int.TryParse(combo.Text, out value))
                    bInitializing = true;
                combo.ItemsSource = FontStyleHelper.sizes;
                if (bInitializing)
                {
                    combo.Text = value.ToString();
                    bInitializing = false;
                }
            }
        }
    }
}
