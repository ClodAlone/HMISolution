using System;
using System.Windows;
using System.Windows.Controls;
using DocumentManager.ComponentService;

namespace PropertyControl.PropertyDataTemplate
{
    /// <summary>
    /// Interaction logic for UrlPropertyEditor.xaml
    /// </summary>
    public partial class BrushPropertyEditor : UserControl
    {
        #region DP
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

        public BrushPropertyEditor()
        {
            InitializeComponent();

            Loaded += (o, e) =>
            {
                Document = ComponentService.PropertyControlComponent.workspace.ContextDocument;
            };
        }
    }

    

    
}
