using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UFInterfaces.CoreHostComponents;
using UFInterfaces;
using System.Windows.Threading;
using System.Collections;
using System.Windows;
using OPCUABrowser.ComponentService;
using Tracing.ComponentService;
using System.Windows.Media.Imaging;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows.Controls;
using Utilities;
using Utilities.WPF;
using UIMsgBoxAlertService.ComponentService;

namespace OPCUABrowser.ComponentService
{
    public class OPCUABrowserComponent : ComponentBase<IOPCUABrowser>, IOPCUABrowser, IDisposable
    {
        #region Declaration
        OPCUABrowserUI OPCUABrowserUIcontrol;
        Object lockObject = new Object();
        #endregion

        #region IOPCUABrowser Members

        private void GetComponentInterfaces()
        {
            if (workspace == null)
                workspace = GetService(typeof(IWorkspace)) as IWorkspace;

            if (simpleLogging == null)
                simpleLogging = GetService(typeof(ISimpleLogging)) as ISimpleLogging;
        }

        [Browsable(false)]
        PopupBrowser InnerEditor;
        public UserControl Editor
        {
            get
            {
                if (InnerEditor == null)
                    InnerEditor = new PopupBrowser(this);
                return InnerEditor; 
            }
        }

        [Browsable(false)]
        PopupBrowser InnerMultiSelectionEditor;
        public UserControl MultiSelectionEditor
        {
            get
            {
                if (InnerMultiSelectionEditor == null)
                    InnerMultiSelectionEditor = new PopupBrowser(this, allowMultiSelection: true);
                return InnerMultiSelectionEditor;
            }
        }

        public bool BrowseEndpoint(String endpoint)
        {
            CreateDockedControl();
            return OPCUABrowserUIcontrol.BrowseEndpoint(endpoint);
        }

        #endregion

        internal static BitmapImage GetControlImage(string image, bool bShared = false)
        {
            var bm = SharedResources.Helpers.ResourceManager.GetCommonImage("OPCUABrowser", image, bShared);
            return bm;
        }

        private void CreateDockedControl()
        {
            lock (lockObject)
            {
                if (OPCUABrowserUIcontrol != null || workspace == null)
                    return;

                OPCUABrowserUIcontrol = new OPCUABrowserUI(this);

                workspace.SetDesiredHeightAndWidthInDockedMode(OPCUABrowserUIcontrol, OPCUABrowserUIcontrol.Height, OPCUABrowserUIcontrol.Width);
                OPCUABrowserUIcontrol.ClearValue(FrameworkElement.WidthProperty);
                OPCUABrowserUIcontrol.ClearValue(FrameworkElement.HeightProperty);

                BitmapImage bm = GetControlImage("OPCBEditor");

                workspace.AddDockingChildren(OPCUABrowserUIcontrol, Properties.Resources.OPCUABrowser_Title, UFInterfaces.DockState.AutoHidden, UFInterfaces.DockSide.Left, false, itemID: nameof(OPCUABrowserUI));
                workspace.SetDockedElementIcon(OPCUABrowserUIcontrol, new ImageBrush(bm));
            }
        }

        #region IUFInterfaceBase Members

        public void Initialize()
        {
            GetComponentInterfaces();
            CreateDockedControl();
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            if (InnerEditor != null)
            {
                InnerEditor.Dispose();
                InnerEditor = null;
            }
            if (InnerMultiSelectionEditor != null)
            {
                InnerMultiSelectionEditor.Dispose();
                InnerMultiSelectionEditor = null;
            }

            OPCUABrowserUIcontrol = null;
            lockObject = null;
        }

        #endregion

        #region Properties

        IWorkspace workspace;
        public IWorkspace Workspace
        {
            get
            {
                return workspace;
            }
        }

        ISimpleLogging simpleLogging;
        public ISimpleLogging SimpleLogging
        {
            get
            {
                if (simpleLogging == null)
                    simpleLogging = GetService(typeof(ISimpleLogging)) as ISimpleLogging;
                return simpleLogging;
            }
        }

        IUIMsgBoxAlertService uiInterface;
        public IUIMsgBoxAlertService UIInterface
        {
            get
            {
                if (uiInterface == null)
                    uiInterface = GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                return uiInterface;
            }
        }

        #endregion
    }
}
