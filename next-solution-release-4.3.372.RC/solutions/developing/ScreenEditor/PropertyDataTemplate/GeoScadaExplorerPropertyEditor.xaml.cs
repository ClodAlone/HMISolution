using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ScreenSettings;
using DocumentManager.ComponentService.Helpers;
using DocumentManager.ComponentService;
using UFProjectManager.ComponentService;
using Utilities;

namespace ScreenManager.PropertyDataTemplate
{
    /// <summary>
    /// Interaction logic for UrlPropertyEditor.xaml
    /// </summary>
    public partial class GeoScadaExplorerPropertyEditor : UserControl
    {
        public GeoScadaExplorerPropertyEditor()
        {
            InitializeComponent();
            //Loaded += (o, e) =>
            //{
            //    var doc = ComponentService.ScreenManagerComponent.screenManagerComponent.Workspace.ContextDocument as ScreenDocument;
            //    if (doc == null)
            //        return;
            //    IDocument parent = DocumentHelper.GetRootParent(doc, false);
            //    IUFProjectManager iUFProjectManager = ComponentService.ScreenManagerComponent.screenManagerComponent.ProjectManager;
            //    if (iUFProjectManager == null)
            //        return;
            //    ControllerSettings cd = iUFProjectManager.GetControllerData(parent, new Uri(doc.FilePath, UriKind.RelativeOrAbsolute));
            //    details.DataContext = cd;
            //};
        }

        private void DlgButton_Click(object sender, RoutedEventArgs e)
        {
            var doc = ComponentService.ScreenManagerComponent.screenManagerComponent.Workspace.ContextDocument as ScreenDocument;
            if (doc == null)
                return;
            IDocument parent = DocumentHelper.GetRootParent(doc, false);
            IUFProjectManager iUFProjectManager = ComponentService.ScreenManagerComponent.screenManagerComponent.ProjectManager;
            if (iUFProjectManager == null)
                return;
            iUFProjectManager.SetControllerDataActive(parent, new Uri(doc.FilePath, UriKind.RelativeOrAbsolute));
            ComponentService.ScreenManagerComponent.screenManagerComponent.PropertyControl?.Activate();
        }
    }
}
