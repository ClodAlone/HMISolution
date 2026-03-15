using System;
using UFInterfaces;
using UFInterfaces.CoreHostComponents;
using System.Windows;
using UIMsgBoxAlertService.ComponentService;
using OPCUABrowser.ComponentService;
using PropertyControl.ComponentService;
using OPCUAViewModel.PropertyDataTemplate;
using System.Windows.Controls;
using OPCUAViewModel;
using UFUAEditor.ComponentService;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace OPCUAViewModelService.ComponentService
{
    public class OPCUAViewModelComponent : ComponentBase<IOPCUAViewModelService>, IOPCUAViewModelService
    {
        #region Declaration

        public static IUIMsgBoxAlertService uiInterface { get; protected set; }
        public static bool uiInterfaceAvailable { get { return uiInterface != null; } }

        public static IOPCUABrowser opcuaBrowserService { get; protected set; }
        public static bool opcuaBrowserServiceAvailable { get { return opcuaBrowserService != null; } }

        public static IUFUAEditorManager ufuaEditorService { get; protected set; }
        public static bool ufuaEditorServiceAvailable { get { return ufuaEditorService != null; } }

        public static IWorkspace workspaceService { get; protected set; }
        public static bool workspaceServiceAvailable { get { return workspaceService != null; } }

        public static OPCUAViewModelComponent opcuaViewModelComponent { get; protected set; }

        public static void QueryInterfaces()
        {
            if (opcuaViewModelComponent == null)
                return;

            if (uiInterface == null)
                uiInterface = opcuaViewModelComponent.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;

            if (opcuaBrowserService == null)
                opcuaBrowserService = opcuaViewModelComponent.GetService(typeof(IOPCUABrowser)) as IOPCUABrowser;

            if (ufuaEditorService == null)
                ufuaEditorService = opcuaViewModelComponent.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;

            if (workspaceService == null)
                workspaceService = opcuaViewModelComponent.GetService(typeof(IWorkspace)) as IWorkspace;
        }
        #endregion
        #region IUFInterfaceBase Members

        void IUFInterfaceBase.Initialize()
        {
            if (opcuaViewModelComponent == null)
                opcuaViewModelComponent = this;

            GetComponentInterfaces();
        }
        #endregion

        private void GetComponentInterfaces()
        {
            QueryInterfaces();

            IPropertyControl PropertyControl = GetService(typeof(IPropertyControl)) as IPropertyControl;
            if (PropertyControl != null)
            {
                var dt = new DataTemplate();
                var factory = new FrameworkElementFactory(typeof(OPCUAEntityReferencePropertyEditor));
                dt.DataType = typeof(OPCUAEntityReference);
                dt.VisualTree = factory;

                PropertyControl.AddPropertyEditor(typeof(OPCUAEntityReference), dt);

                var dtl = new DataTemplate();
                var lfactory = new FrameworkElementFactory(typeof(OPCUAEntityReferenceListPropertyEditor));
                dtl.VisualTree = lfactory;

                PropertyControl.AddPropertyEditor(typeof(OPCUAEntityReferenceList), dtl);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(OPCUAXMLEntityReferencePropertyEditor));
                dt.DataType = typeof(OPCUAXMLEntityReference);
                dt.VisualTree = factory;

                PropertyControl.AddPropertyEditor(typeof(OPCUAXMLEntityReference), dt);
            }
        }
        #region IOPCUAViewModelService
        public void QueryComponentInterfaces()
        {
            QueryInterfaces();
        }

        public bool IsUiInterfaceAvailable()
        {
            return uiInterfaceAvailable;
        }

        public void ShowError(string error)
        {
            uiInterface.ShowError(error);
        }
        public void ShowInformation(string info)
        {
            uiInterface.ShowInformation(info);
        }
        
        public CustomDialogResults ShowYesNo(string message, CustomDialogIcons icon)
        {
            return uiInterface.ShowYesNo(message, icon);
        }
        #endregion
    }
}
