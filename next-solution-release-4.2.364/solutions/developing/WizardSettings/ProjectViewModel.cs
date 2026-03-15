using System;
using System.Collections.Generic;
using System.Linq;
using UriResolver.ComponentService;
using ScreenManager.ComponentService;
using UIMsgBoxAlertService.ComponentService;
using UFUAEditor.ComponentService;
using UFProjectManager.ComponentService;
using StringManager.ComponentService;
using UFRecipeEditor.ComponentService;
using HelpProvider.ComponentService;
using Toolbox.ComponentService;

namespace WizardSettings
{
    public class ProjectViewModel
    {
        public IUriRisolver UriRisolver { get; set; }
        public IUIMsgBoxAlertService UIInterface { get;  set; }
        public IScreenManager screenManagerService { get;  set; }
        public IUFUAEditorManager UFUAEditorManager { get;  set; }
        public IUFProjectManager projectManagerService { get;  set; }
        public IStringEditorManager stringEditorManager { get; set; }
        public IRecipeEditorManager recipeEditorManager { get; set; }
        public IToolbox toolboxManager { get; set; }
        public IHelpProvider helpProvider { get; set; }

        public String GetServerIOConnectionStringFromUri(Uri uri)
        {
            if (XpoHelpers.XpoHelper.IsDataSource(uri.LocalPath))
                return uri.LocalPath;
            else
            {
                var manager = UFUAEditorManager as DocumentManager.ComponentService.IDocumentManager;
                if (manager != null)
                {
                    var xmlfile = String.Format("{0}\\{1}\\{2}\\{3}{4}",
                        System.IO.Path.GetDirectoryName(uri.LocalPath),
                        System.IO.Path.GetFileNameWithoutExtension(uri.LocalPath),
                        manager.TypeLabel,
                        manager.FileName,
                        manager.FileType);
                    return DevExpress.Xpo.DB.InMemoryDataStore.GetConnectionString(xmlfile);
                }
            }

            return null;
        }
    }
}
