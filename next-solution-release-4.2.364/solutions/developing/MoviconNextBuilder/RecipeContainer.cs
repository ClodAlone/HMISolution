using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UFProjectManager.ComponentService;
using DocumentManager.ComponentService;
using UFRecipeSettings.Documents;
using UFRecipeEditor.ComponentService;
using UFRecipeSettings.UFRecipeModel;

namespace MoviconNextBuilder
{
    public enum RecipeReturnCode : int
    {
        Ok = 0,
        RecipeNull = -1,
        GroupNotEmpty = -2,
        GroupNull = -3,
        ValueNull = -4
    }

    public class RecipeContainer : IDisposable
    {

        #region Ctor
        public RecipeContainer(IDocument parent, UFProjectManagerComponent manager)
        {
            projectdoc = (UFProjectManager.UFProjectDocument)parent;
            projectman = manager;
        }
        #endregion Ctor

        #region data
        UFProjectManager.UFProjectDocument projectdoc;
        UFProjectManagerComponent projectman;
        Dictionary<Uri, UFRecipeDocument> mapDocuments = new Dictionary<Uri, UFRecipeDocument>();
        
        #endregion data

        #region Properties
        static RecipeEditorManagerComponent recipeEditorComponent = new RecipeEditorManagerComponent();
        static RecipeEditorManagerComponent RecipeEditorComponent
        {
            get { return recipeEditorComponent; }
        }
        #endregion Properties

        #region Methods
        public UFRecipeDocument AddRecipe(string name, string folder = null)
        {
            var path = projectdoc.GetResourcePath("UFRecipeEditor", folder);
            Uri uri = ProjectBuilder.GetUriFromName(path, name, RecipeEditorComponent, projectdoc);
            if (uri == null)
                return null;
            
            UFRecipeDocument doc = RecipeEditorComponent.GetDocument(uri) as UFRecipeDocument;
            if (doc == null)
            {
                RecipeEditorComponent.CreateDefaultDocument(uri, projectdoc, projectdoc.IsPasswordProtected());
                doc = RecipeEditorComponent.CreateDocEx(uri, projectdoc);
            }

            if (doc != null)
                mapDocuments[uri] = doc;
            return doc;
        }

        public UFRecipeDocument GetRecipe(string name, string subfolder = null)
        {
            var path = projectdoc.GetResourcePath("UFRecipeEditor", subfolder);
            Uri uri = ProjectBuilder.GetUriFromName(path, name, RecipeEditorComponent, projectdoc, bCheckExists: true);
            if (uri == null)
                return null;

            if (mapDocuments.ContainsKey(uri))
                return mapDocuments[uri];
            else
            {
                var doc = RecipeEditorComponent.CreateDocEx(uri, projectdoc);
                if (doc != null)
                {
                    mapDocuments[uri] = doc;
                    return doc;
                }
            }

            return null;
        }
        public bool DeleteRecipe(string name, string subfolder = null)
        {
            var path = projectdoc.GetResourcePath("UFRecipeEditor", subfolder);
            Uri uri = ProjectBuilder.GetUriFromName(path, name, RecipeEditorComponent, projectdoc, bCheckExists: true);
            if (uri != null)
            {
                RecipeEditorComponent.Delete(uri, projectdoc);
                return true;
            }
            else
                return false;
        }


        public UFGroupEntity AddDataGroup(UFRecipeDocument recipe, string name)
        {
            if (recipe == null)
                return null;
            var newGroup = recipe.GetDataGroup(name);
            if (newGroup != null)
                return newGroup;
            newGroup = recipe.AddNewGroup(name);
            recipe.RecipeEntity.Groups.Add(newGroup);
            recipe.NeedsSave = true;

            return newGroup;
        }

        public UFGroupEntity GetDataGroup(UFRecipeDocument recipe, string name)
        {
            if (recipe == null)
                return null;
            return recipe.GetDataGroup(name);
        }

        public RecipeReturnCode DeleteDataGroup(UFRecipeDocument recipe, string name, bool forcedelete = false)
        {
            if (recipe == null)
                return RecipeReturnCode.RecipeNull;

            var group = recipe.GetDataGroup(name);
            if (group == null)
                return RecipeReturnCode.GroupNull;
            if (group.DataValues.Count > 0)
            {
                if(!forcedelete)
                    return RecipeReturnCode.GroupNotEmpty;
                //delete the data item
                while(group.DataValues.Count > 0)
                {
                    var i = group.DataValues[0];
                    DeleteDataItem(recipe, i.Name, group);
                }
            }
            
            if (group.UFRecipeAss != null)
            {
                group.UFRecipeAss.Groups.Remove(group);
                recipe.NeedsSave = true;
            }
            return RecipeReturnCode.Ok;
        }

        public UFDataValueEntity AddDataItem(UFRecipeDocument recipe, string name, UFGroupEntity group = null)
        {
            if (recipe == null)
                return null;
            var datavalue = recipe.GetDataValue(name, group);
            if (datavalue != null)
                return datavalue;
            
            datavalue = recipe.AddNewDataValue(group, name);
            if (group == null)
                recipe.RecipeEntity.DataValues.Add(datavalue);
            else
                group.DataValues.Add(datavalue);
            recipe.NeedsSave = true;
            return datavalue;
        }

        public UFDataValueEntity GetDataItem(UFRecipeDocument recipe, string name, UFGroupEntity group = null)
        {
            if (recipe == null)
                return null;
            return recipe.GetDataValue(name, group);
        }

        public RecipeReturnCode DeleteDataItem(UFRecipeDocument recipe, string name, UFGroupEntity group = null)
        {
            if (recipe == null)
                return RecipeReturnCode.RecipeNull;
            var value = recipe.GetDataValue(name, group);
            if (value == null)
                return RecipeReturnCode.ValueNull;
            if (value.UFRecipeAss != null)
            {
                value.UFRecipeAss.DataValues.Remove(value);
                recipe.NeedsSave = true;
            }
            if (value.UFGroupAss != null)
            {
                value.UFGroupAss.DataValues.Remove(value);
                recipe.NeedsSave = true;
            }
            return RecipeReturnCode.Ok;
        }

        public bool AddFolder(string folder)
        {
            var path = projectdoc.GetResourcePath("UFRecipeEditor");
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
