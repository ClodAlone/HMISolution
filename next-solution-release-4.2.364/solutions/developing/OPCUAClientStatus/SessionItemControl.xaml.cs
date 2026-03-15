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
using OPCUAEventViewers;
using OPCUAViewModel;
using StringManager.ComponentService;
using Utilities;
using System.ComponentModel;
using System.Globalization;

namespace OPCUAClientStatus
{
    /// <summary>
    /// Interaction logic for SessionItemControl.xaml
    /// </summary>
    public partial class SessionItemControl : UserControl, IDisposable
    {
        OPCUASystemEventViewer systemEventViewer;
        bool bLoaded;
        readonly IStringEditorManager stringManager;
        public SessionItemControl(IStringEditorManager s)
        {
            InitializeComponent();

            stringManager = s;
            Loaded += (o, e) =>
            {
                if (!bLoaded)
                {
                    bLoaded = true;

                    if (stringManager != null)
                    {
                        stringManager.CultureChanged += StringManager_CultureChanged;
                    }
                }
            };
        }

        private void StringManager_CultureChanged(object sender, EventArgs e)
        {
            Dispatcher.BeginInvokeInBackgroundIfRequired(() =>
            {
                dateTime.MaskCulture = System.Globalization.CultureInfo.CurrentCulture;
            });
        }
        private void expanderAudit_Expanded(object sender, RoutedEventArgs e)
        {
            if (systemEventViewer != null)
                return;

            var sessionViewModel = DataContext as SessionViewModel;
            if (sessionViewModel == null)
                return;

            systemEventViewer = new OPCUASystemEventViewer(sessionViewModel,stringManager);
            // systemEventViewer.ClearValue(FrameworkElement.WidthProperty);
            //auditEventViewer.ClearValue(FrameworkElement.HeightProperty);

            expanderAudit.Content = systemEventViewer;
        }
        bool bDisposed;
        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;
            if (stringManager != null)
                stringManager.CultureChanged -= StringManager_CultureChanged;
            if (systemEventViewer != null)
            {
                systemEventViewer.Dispose();
                systemEventViewer = null;
            }
        }
    }
}
