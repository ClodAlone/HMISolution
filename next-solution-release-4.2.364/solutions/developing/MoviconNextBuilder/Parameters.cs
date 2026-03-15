using DocumentManager.ComponentService;
using ScreenParametersEditor.ComponentService;
using ScreenParametersSettings.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UFProjectManager.ComponentService;
using Utilities;

namespace MoviconNextBuilder
{
    public class Parameters : IDisposable
    {
        #region Ctor
        public Parameters(IDocument parent, UFProjectManagerComponent manager)
        {
            projectdoc = (UFProjectManager.UFProjectDocument)parent;
            projectman = manager;
        }
        #endregion Ctor

        #region data
        UFProjectManager.UFProjectDocument projectdoc;
        UFProjectManagerComponent projectman;

        Dictionary<Uri, ScreenParametersDocument> mapDocuments = new Dictionary<Uri, ScreenParametersDocument>();
        #endregion data

        #region Properties
        static ScreenParametersEditorComponent parametersComponent = new ScreenParametersEditorComponent();
        static public ScreenParametersEditorComponent ParametersComponent
        {
            get { return parametersComponent; }
        }
        #endregion Properties

        #region Methods
        public bool AddFolder(string folder)
        {
            var path = projectdoc.GetResourcePath("ScreenParametersEditor");
            var destPath = string.Format("{0}{1}", path.OriginalString, folder);

            if (projectdoc.fileSystemProviderBase != null)
            {
                projectdoc.fileSystemProviderBase.CreateFolder(null, destPath);
                return projectdoc.fileSystemProviderBase.Exists(new VFS.FileManagerFolder(projectdoc.fileSystemProviderBase, destPath));
            }
            else
            {
                try
                {
                    return System.IO.Directory.CreateDirectory(destPath) != null;
                }
                catch (Exception ex)
                { }
            }

            return false;
        }

        public ScreenParametersDocument AddParameterFile(string name, string subfolder = null)
        {
            var path = projectdoc.GetResourcePath("ScreenParametersEditor", subfolder);
            var paramuri = ProjectBuilder.GetUriFromName(path, name, ParametersComponent, projectdoc);
            if (paramuri == null)
                return null;

            ScreenParametersDocument doc = ParametersComponent.GetDocument(paramuri) as ScreenParametersDocument;
            if (doc == null)
            {
                ParametersComponent.CreateDefaultDocument(paramuri, projectdoc, projectdoc.IsPasswordProtected());
                doc = ParametersComponent.CreateDoc(paramuri, projectdoc);
            }

            if (doc != null)
                mapDocuments[paramuri] = doc;
            return doc;
        }

        public Uri GetParameterUri(string name, string subfolder = null)
        {
            var path = projectdoc.GetResourcePath("ScreenParametersEditor", subfolder);
            return ProjectBuilder.GetUriFromName(path, name, ParametersComponent, projectdoc, bCheckExists: true);
        }

        public ScreenParametersDocument GetParameterFile(string name, string subfolder = null)
        {
            var path = projectdoc.GetResourcePath("ScreenParametersEditor", subfolder);
            var paramuri = ProjectBuilder.GetUriFromName(path, name, ParametersComponent, projectdoc, bCheckExists: true);
            if (paramuri != null)
            {
                if (mapDocuments.ContainsKey(paramuri))
                    return mapDocuments[paramuri];
                else
                {
                    var doc = ParametersComponent.CreateDoc(paramuri, projectdoc);
                    if (doc != null)
                    {
                        mapDocuments[paramuri] = doc;
                        return doc;
                    }
                }
            }
            return null;
        }

        public bool DeleteParameterFile(string name, string subfolder = null)
        {
            var path = projectdoc.GetResourcePath("ScreenParametersEditor", subfolder);
            var paramuri = ProjectBuilder.GetUriFromName(path, name, ParametersComponent, projectdoc, bCheckExists: true);
            if (paramuri != null)
            {
                parametersComponent.Delete(paramuri, projectdoc);
                if (mapDocuments.ContainsKey(paramuri))
                    mapDocuments.Remove(paramuri);
                return true;
            }
            return false;
        }

        public void Save()
        {
            if (mapDocuments.Count > 0)
            {
                foreach (var doc in mapDocuments.Values)
                {
                    if (doc.NeedsSave)
                    {
                        doc.SaveToFile();
                    }
                }

            }
        }

        public void Dispose()
        {
            foreach (var doc in mapDocuments.Values)
                doc.Dispose();
            mapDocuments.Clear();
        }
        #endregion Methods
    }
}
