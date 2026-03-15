using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Utilities;
using Utilities.WPF;
using WPFUtilities.Converters;
using DocumentManager.ComponentService;

namespace PropertyControl.PropertyDataTemplate
{
    /// <summary>
    /// Interaction logic for UrlPropertyEditor.xaml
    /// </summary>
    public partial class ColorPropertyEditor : UserControl
    {
        #region DP
        #region Document
        public static readonly DependencyProperty DocumentProperty = DependencyProperty.Register("Document", typeof(IDocument), typeof(ColorPropertyEditor), new UIPropertyMetadata(null));
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

        public ColorPropertyEditor()
        {
            InitializeComponent();

            Loaded += (o, e) =>
            {
                Document = ComponentService.PropertyControlComponent.workspace.ContextDocument;
            };
        }
    }
}
