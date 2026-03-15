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
using CommandManager.ComponentService;
using WPFUtilities.PropertyDataTemplate;

namespace CommandManager.UserControls
{
    /// <summary>
    /// Interaction logic for CommonCommandPropertyEditor.xaml
    /// </summary>
    public partial class CommonCommandPropertyEditor : UserControl, IDisposable
    {
        public CommonCommandPropertyEditor()
        {
            InitializeComponent();

            if (CommandManagerComponent.propertyServiceAvailable)
            {
                var property = CommandManagerComponent.propertyService.controlNoSelectionPriority;
                property.ClearValue(FrameworkElement.WidthProperty);
                property.ClearValue(FrameworkElement.HeightProperty);
                content.Content = property;

                DataContextChanged += (o, e) =>
                {
                    CommandManagerComponent.propertyService.SetControlSelection(property, DataContext);
                };
            }
        }

        #region IDisposable
        bool bDisposed;
        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;

            if (content.Content is IDisposable)
                (content.Content as IDisposable).Dispose();
        }
        #endregion
    }
}
