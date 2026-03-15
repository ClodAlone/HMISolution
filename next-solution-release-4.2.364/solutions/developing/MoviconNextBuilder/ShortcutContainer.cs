using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentManager.ComponentService;
using UFProjectManager.ComponentService;
using UFShortcutEditor.ComponentService;
using UFShortcutSettings.Documents;
using UFShortcutSettings.ShortcutModel;

namespace MoviconNextBuilder
{
    public class ShortcutContainer : IDisposable
    {
        #region Ctor
        public ShortcutContainer(IDocument parent, UFProjectManagerComponent manager)
        {
            projectdoc = (UFProjectManager.UFProjectDocument)parent;
            projectman = manager;
        }
        #endregion Ctor

        #region data
        UFProjectManager.UFProjectDocument projectdoc;
        UFProjectManagerComponent projectman;
        Dictionary<Uri, UFShortcutDocument> mapDocuments = new Dictionary<Uri, UFShortcutDocument>();
        #endregion data

        #region Properties
        static ShortcutEditorManagerComponent shortcutEditorComponent = new ShortcutEditorManagerComponent();
        static ShortcutEditorManagerComponent ShortcutEditorComponent
        {
            get { return shortcutEditorComponent; }
        }
        #endregion Properties


        #region Methods
        public UFShortcutDocument AddShortcut(string name, string subfolder = null)
        {

            var path = projectdoc.GetResourcePath("UFShortcutEditor", subfolder);
            Uri uri = ProjectBuilder.GetUriFromName(path, name, ShortcutEditorComponent, projectdoc);
            if (uri == null)
                return null;

            UFShortcutDocument doc = ShortcutEditorComponent.GetDocument(uri) as UFShortcutDocument;
            if (doc == null)
            {
                shortcutEditorComponent.CreateDefaultDocument(uri, projectdoc, projectdoc.IsPasswordProtected());
                doc = ShortcutEditorComponent.CreateDoc(uri, projectdoc);
            }
            if (doc != null)
                mapDocuments[uri] = doc;
            return doc;
        }
        public UFShortcutDocument GetShortcut(string name, string subfolder = null)
        {
            var path = projectdoc.GetResourcePath("UFShortcutEditor", subfolder);
            Uri uri = ProjectBuilder.GetUriFromName(path, name, ShortcutEditorComponent, projectdoc, bCheckExists: true);
            if (uri == null)
                return null;

            if (mapDocuments.ContainsKey(uri))
                return mapDocuments[uri];
            else
            {
                var doc = ShortcutEditorComponent.CreateDoc(uri, projectdoc);
                if (doc != null)
                {
                    mapDocuments[uri] = doc;
                    return doc;
                }
            }

            return null;
        }
        public bool DeleteShortcut(string name, string subfolder = null)
        {
            var path = projectdoc.GetResourcePath("UFShortcutEditor", subfolder);
            Uri uri = ProjectBuilder.GetUriFromName(path, name, ShortcutEditorComponent, projectdoc, bCheckExists: true);
            if (uri != null)
            {
                ShortcutEditorComponent.Delete(uri, projectdoc);
                return true;
            }
            return false;
        }

        public UFKeyCommandEntity AddShortcutKey(UFShortcutDocument scut, string name, string key = null)
        {
            if (scut != null)
            {
                var item = scut.GetShortcutKey(name);
                if (item != null)
                {
                    if (key != null)
                        item.ShortcutKey = key;
                    return item;
                }
                    
                scut.NeedsSave = true;
                item = scut.AddNewKeyCommand(name);
                if (key != null)
                    item.ShortcutKey = key;

                if (!item.UFShortcutAss.KeyCommands.Contains(item))
                    item.UFShortcutAss.KeyCommands.Add(item);

                return item;
            }
            return null;
        }

        public UFKeyCommandEntity GetShorcutKey(UFShortcutDocument scut, string name)
        {
            if (scut == null)
                return null;
            scut.NeedsSave = true;
            return scut.GetShortcutKey(name);
        }

        public bool DeleteShortcutKey(UFShortcutDocument scut, string name)
        {
            if (scut == null)
                return false;

            var itemtodelete = scut.GetShortcutKey(name);

            if (itemtodelete.UFShortcutAss != null)
            {
                itemtodelete.UFShortcutAss.KeyCommands.Remove(itemtodelete);
                scut.NeedsSave = true;
                return true;
            }
            return false;
        }

        public bool AddFolder(string folder)
        {
            var path = projectdoc.GetResourcePath("UFShortcutEditor");
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
