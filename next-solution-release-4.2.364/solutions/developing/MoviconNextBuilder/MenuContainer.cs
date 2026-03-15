using MenuSettings.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UFProjectManager.ComponentService;
using DocumentManager.ComponentService;
using UFMenuEditor.ComponentService;
using MenuSettings.MenuModel;

namespace MoviconNextBuilder
{
    public class MenuContainer : IDisposable
    {
        #region Ctor
        public MenuContainer(IDocument parent, UFProjectManagerComponent manager)
        {
            projectdoc = (UFProjectManager.UFProjectDocument)parent;
            projectman = manager;
        }
        #endregion Ctor

        #region data
        UFProjectManager.UFProjectDocument projectdoc;
        UFProjectManagerComponent projectman;
        Dictionary<Uri, UFMenuDocument> mapDocuments = new Dictionary<Uri, UFMenuDocument>();
        #endregion data

        #region Properties
        static MenuEditorManagerComponent menuEditorComponent = new MenuEditorManagerComponent();
        static MenuEditorManagerComponent MenuEditorComponent
        {
            get { return menuEditorComponent; }
        }
        #endregion Properties

        #region Methods
        /// <summary>
        /// Adds a menu to the project
        /// </summary>
        /// <param name="name">name of the menu</param>
        /// <param name="subfolder">folder containing the menu (optional)</param>
        /// <returns>The UFMenuDocument object relative to the menu</returns>
        public UFMenuDocument AddMenu(string name, string subfolder = null)
        {

            var path = projectdoc.GetResourcePath("UFMenuEditor", subfolder);
            Uri uri = ProjectBuilder.GetUriFromName(path, name, MenuEditorComponent, projectdoc);
            if (uri == null)
                return null;

            UFMenuDocument doc = MenuEditorComponent.GetDocument(uri) as UFMenuDocument;
            if(doc == null)
            {
                MenuEditorComponent.CreateDefaultDocument(uri, projectdoc, projectdoc.IsPasswordProtected());
                doc = MenuEditorComponent.CreateDoc(uri, projectdoc);
            }
                
            if (doc != null)
                mapDocuments[uri] = doc;
            return doc;
        }
        /// <summary>
        /// Gets a menu object
        /// </summary>
        /// <param name="name">name of the menu</param>
        /// <param name="subfolder">folder containing the menu</param>
        /// <returns>UFMenuDocument relative to the menu</returns>
        public UFMenuDocument GetMenu(string name, string subfolder = null)
        {
            var path = projectdoc.GetResourcePath("UFMenuEditor", subfolder);
            Uri uri = ProjectBuilder.GetUriFromName(path, name, MenuEditorComponent, projectdoc, bCheckExists: true);
            if (uri == null)
                return null;

            if (mapDocuments.ContainsKey(uri))
                return mapDocuments[uri];
            else
            {
                var doc = MenuEditorComponent.CreateDoc(uri, projectdoc);
                if (doc != null)
                {
                    mapDocuments[uri] = doc;
                    return doc;
                }
            }
                
            return null;
        }
        /// <summary>
        /// Delete a menu
        /// </summary>
        /// <param name="name">name of the menu</param>
        /// <param name="subfolder">folder containing the menu</param>
        /// <returns>returns true upon successful deletion</returns>
        public bool DeleteMenu(string name, string subfolder = null)
        {
            var path = projectdoc.GetResourcePath("UFMenuEditor", subfolder);
            Uri uri = ProjectBuilder.GetUriFromName(path, name, MenuEditorComponent, projectdoc, bCheckExists: true);
            if(uri != null)
            {
                MenuEditorComponent.Delete(uri, projectdoc);
                return true;
            }
            return false;
        }


        public UFMenuItemEntity AddMenuItem(UFMenuDocument menu, string name, UFMenuItemEntity root = null)
        {
            if(menu != null)
            {
                var item = menu.GetMenuItem(name, root);
                if (item != null)
                    return item;
                item = menu.AddNewMenuItem(root, name);
                menu.NeedsSave = true;
                if(item.UFItemAss ==  null)
                {
                    //root
                    item.UFMenuAss = menu.MenuEntity;
                    if (!item.UFMenuAss.MenuItems.Contains(item))
                        item.UFMenuAss.MenuItems.Add(item);
                }
                else

                if (!item.UFItemAss.MenuItems.Contains(item))
                    item.UFItemAss.MenuItems.Add(item);
                return item;
            }
            return null;
        }

        public UFMenuItemEntity GetMenuItem(UFMenuDocument menu, string name, UFMenuItemEntity root = null)
        {
            if (menu == null)
                return null;
            menu.NeedsSave = true;
            return menu.GetMenuItem(name, root);
        }

        public UFMenuItemEntity GetMenuItem(UFMenuDocument menu, Guid nodeid, UFMenuItemEntity root = null)
        {
            if (menu == null)
                return null;
            menu.NeedsSave = true;
            return menu.GetMenuItem(nodeid, root);
        }

        public bool MoveMenuItemUp(UFMenuDocument menu, UFMenuItemEntity item)
        {
            if(menu != null && item != null)
            {
                menu.NeedsSave = true;
                return menu.MoveEntityUp(item);
            }
            return false;
        }

        public bool MoveMenuItemDown(UFMenuDocument menu, UFMenuItemEntity item)
        {
            if (menu != null && item != null)
            {
                menu.NeedsSave = true;
                return menu.MoveEntityDown(item);
            }
            return false;
        }
        public bool DeleteMenuItem(UFMenuDocument menu, string name, UFMenuItemEntity root = null)
        {
            if (menu == null)
                return false;
            var itemtodelete = menu.GetMenuItem(name, root);

            if (itemtodelete.UFMenuAss != null)
            {
                itemtodelete.UFMenuAss.MenuItems.Remove(itemtodelete);
                menu.NeedsSave = true;
            }
            if (itemtodelete.UFItemAss != null)
            {
                itemtodelete.UFItemAss.MenuItems.Remove(itemtodelete);
                menu.NeedsSave = true;
                if (itemtodelete.UFItemAss.MenuItems.Count == 0)
                    itemtodelete.UFItemAss.MenuItemType = MenuType.Item;
            }
            return false;
        }
        public bool AddFolder(string folder)
        {
            var path = projectdoc.GetResourcePath("UFMenuEditor");
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
