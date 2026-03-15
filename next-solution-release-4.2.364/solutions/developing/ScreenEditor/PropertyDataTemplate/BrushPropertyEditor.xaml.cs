using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using ScreenSettings;
using DocumentManager.ComponentService;

namespace ScreenManager.PropertyDataTemplate
{
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum EditOptionsEnum
    {
        Background,
        Foreground,
        BorderBrush
    }
    /// <summary>
    /// Interaction logic for UrlPropertyEditor.xaml
    /// </summary>
    public partial class BrushPropertyEditor : UserControl
    {
        #region DP
        #region EditOptions
        public static readonly DependencyProperty EditOptionProperty = DependencyProperty.Register("EditOption", typeof(EditOptionsEnum), typeof(BrushPropertyEditor), new UIPropertyMetadata(EditOptionsEnum.Background));
        public EditOptionsEnum EditOption
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (EditOptionsEnum)GetValue(EditOptionProperty);
            }
            set
            {
                SetValue(EditOptionProperty, value);
            }
        }
        #endregion
        #region Document
        public static readonly DependencyProperty DocumentProperty = DependencyProperty.Register("Document", typeof(IDocument), typeof(BrushPropertyEditor), new UIPropertyMetadata(null));
        public IDocument Document
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (IDocument)GetValue(DocumentProperty);
            }
            set
            {
                SetValue(DocumentProperty, value);
            }
        }
        #endregion
        #endregion

        #region Declarationd
        String type = String.Empty;
        bool bComposed = false;
        bool bSymbol = false;
        bool bSpread = false;
        bool bEditBrush = false;
        bool bForeground = false;
        BrushEditorOptions options;
        ScreenEditorView screenEditorView;
        #endregion

        public BrushPropertyEditor()
        {
            InitializeComponent();

            Loaded += (o, e) =>
            {
                Document = ComponentService.ScreenManagerComponent.screenManagerComponent.Workspace.ContextDocument as ScreenDocument;
                if (Document == null)
                    return;
                screenEditorView = Document.ActiveView as ScreenEditorView;
                if (screenEditorView == null)
                    return;

                options = new BrushEditorOptions();
                bEditBrush = EditOption == EditOptionsEnum.Background;
                bForeground = EditOption == EditOptionsEnum.Foreground;
                var brush = screenEditorView.GetSelectedBrush(bEditBrush, bForeground, ref options);
                popupColorEdit.SelectedBrush = brush;
            };
        }

        private void OnMoreColorsClicked(object sender, RoutedEventArgs e)
        {
            if (screenEditorView == null)
                return;

            e.Handled = true;
            if (bEditBrush)
                screenEditorView.OpenBrushEditor();
            else
                screenEditorView.OpenPenEditor(bForeground: bForeground);

            var brush = screenEditorView.GetSelectedBrush(bEditBrush, bForeground, ref options);

            if (popupColorEdit.SelectedBrush != brush)
            {
                if (brush is System.Windows.Media.SolidColorBrush)
                {
                    var color = (brush as System.Windows.Media.SolidColorBrush).Color;
                    Utilities.RecentColorsHelper.AddRecentColor(sender, color);
                }
                popupColorEdit.SelectedBrush = brush;
            }
        }

        private void OnBrushSelected(object sender, EventArgs e)
        {
            if (screenEditorView == null)
                return;

            if (bEditBrush)
                screenEditorView.SetSelectedBrush(popupColorEdit.SelectedBrush, bSpreadColor: options.SpreadOnChild);
            else
                screenEditorView.SetSelectedPen(popupColorEdit.SelectedBrush, bForeground: bForeground, bSpreadColor: options.SpreadOnChild);
        }
    }
}
