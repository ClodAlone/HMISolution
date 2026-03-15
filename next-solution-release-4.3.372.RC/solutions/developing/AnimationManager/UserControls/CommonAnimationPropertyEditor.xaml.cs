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
using AnimationManager.ComponentService;

namespace AnimationManager.UserControls
{
    /// <summary>
    /// Interaction logic for CommonAnimationPropertyEditor.xaml
    /// </summary>
    public partial class CommonAnimationPropertyEditor : UserControl, IDisposable
    {
        public CommonAnimationPropertyEditor()
        {
            InitializeComponent();

            if (AnimationManagerComponent.propertyServiceAvailable)
            {
                var property = AnimationManagerComponent.propertyService.controlNoSelectionPriority;
                property.ClearValue(FrameworkElement.WidthProperty);
                property.ClearValue(FrameworkElement.HeightProperty);
                content.Content = property;

                DataContextChanged += (o, e) =>
                    {
                        AnimationManagerComponent.propertyService.SetControlSelection(property, DataContext);
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
