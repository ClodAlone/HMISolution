using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using UFInterfaces;
using UFInterfaces.CoreHostComponents;
using UIMsgBoxAlertService.ComponentService;
using System.Windows.Threading;
using Tracing.ComponentService;
using Utilities;

namespace HelpProvider.ComponentService
{
    public class HelpProviderComponent : ComponentBase<IHelpProvider>, IHelpProvider
    {
        #region Declaration

        Object lockObject = new Object();
        HelpProviderUI pluginUI;
        IWorkspace workspace;
        ISimpleLogging simpleLogging;
        IUIMsgBoxAlertService uiInterface;
        #endregion Declaration

        #region IUFInterfaceBase Members

        void IUFInterfaceBase.Initialize()
        {
            Dispatcher.CurrentDispatcher.InvokeIfRequired(() =>
            {
                GetComponentInterfaces();
                CreatePluginUI();
            });
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            pluginUI = null;
            lockObject = null;
        }

        private void GetComponentInterfaces()
        {
            if (workspace == null)
                workspace = GetService(typeof(IWorkspace)) as IWorkspace;
            if (workspace == null)
                throw new NotImplementedException("Expecting the missing IWorkpsace Interface");

            if (simpleLogging == null)
                simpleLogging = GetService(typeof(ISimpleLogging)) as ISimpleLogging;

            if (uiInterface == null)
                uiInterface = GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
        }

        public void CreatePluginUI()
        {
            lock (lockObject)
            {
                if (workspace == null)
                    GetComponentInterfaces();

                if (pluginUI != null)
                    return;

                pluginUI = new HelpProviderUI(this);

                MenuControl menuControl = new MenuControl() { DataContext = pluginUI };
                workspace.AddBarManagerItem(menuControl, pluginUI.CommandBindings);
            }
        }

        internal string GetSelectedObject()
        {
            if (workspace.ContextObject != null)
            {
                if (workspace.ContextObject is IEntityReference &&
                    (workspace.ContextObject as IEntityReference).ContainedObject != null)
                {
                    return (workspace.ContextObject as IEntityReference).ContainedObject.ToString();
                }
                else
                {
                    return workspace.ContextObject.GetType().FullName;
                }
            }
            else if (workspace.ContextObjects != null)
            {
                var list = new List<Object>();
                foreach (Object o in workspace.ContextObjects)
                {
                    if (o == null)
                        continue;

                    if (o is IEntityReference &&
                            (o as IEntityReference).ContainedObject != null)
                    {
                        return (o as IEntityReference).ContainedObject.ToString();
                    }
                    else
                    {
                        return o.ToString();
                    }
                }
            }
            return string.Empty;
        }

        #region properties

        public IUIMsgBoxAlertService UIInterface
        {
            get
            {
                if (uiInterface == null)
                    uiInterface = GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                return uiInterface;
            }
        }

        public bool IsLocalHelpEnabled
        {
            get 
            {
                if(pluginUI != null)
                    return pluginUI.LocalHelp;
                else
                    return false;
            }
        }
        public void UseLocalHelp(bool enableLocalHelp)
        {
            if (pluginUI != null)
                pluginUI.LocalHelp = enableLocalHelp;
        }
        public void ShowAboutBox()
        {
            if (pluginUI != null)
                pluginUI.ShowAboutBox();
        }
        public void ExecuteHelp()
        {
            if (pluginUI != null)
                pluginUI.ExecuteHelp();
        }
        #endregion


        #region IPlugin Members

        public void OpenHelpPage(Uri uri)
        {
            if (pluginUI == null)
                CreatePluginUI();

            pluginUI.ShowDialogHelp(uri);
        }

        public void OpenDialogHelpPage(string prop, bool alone = false, bool newwin = false)
        {
            if (pluginUI == null)
            {
                if(!alone)
                    CreatePluginUI();
                else
                    pluginUI = new HelpProviderUI(this);
            }
            pluginUI.OpenDialogUri(prop, newwin);
        }
        #endregion

        public string LocalFilePath
        {
            get
            {
                if (pluginUI != null)
                    return pluginUI.LocalFilePath;
                else
                    return "HelpOnLine";
            }
        }

        public string WebFilePath
        {
            get
            {
                if (pluginUI != null)
                    return pluginUI.WebFilePath;
                else
                    return "http://support.progea.com/download";
            }
        }

        public void InitRootPath()
        {
            if (pluginUI != null)
                pluginUI.InitRootPath();
        }
    }
}