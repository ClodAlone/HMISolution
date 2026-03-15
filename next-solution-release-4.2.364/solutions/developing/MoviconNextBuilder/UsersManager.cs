using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentManager.ComponentService;
using TranslationHelpers;
using UFUserEditor.ComponentService;
using UFUserEditor.Document;
using UFUserModel;
using Utilities;

namespace MoviconNextBuilder
{
    public class UsersManager : IDisposable
    {
        public enum UsersReturnCode : int
        {
            Ok = 0,
            NoObject = -1,
            NoForce = -2,
            NoDocument = -3
        }

        #region Ctor
        public UsersManager(IDocument parent, UFUserEditorManagerComponent editormanager)
        {
            userManagerComponent = editormanager;
            var uri = new Uri(parent.rootBase, UriKind.RelativeOrAbsolute);
            document = UFUserDocument.FromFile(uri.GetPathString(), editormanager, parent);
        }
        #endregion Ctor

        #region data
        
        UFUserDocument document;
        #endregion data

        #region Properties
        UFUserEditorManagerComponent userManagerComponent;
        public UFUserEditorManagerComponent UserManagerComponent
        {
            get { return userManagerComponent; }
        }
        #endregion Properties

        #region Methods
        //public void AddDefaultRoles(Dictionary<string, string> stringlist)
        //{
        //    //dictionary<string, string> stringlist = stringManager.GetListStringForCulture(Document, stringManager.GetActiveCulture(Document));
        //    var aa1 = document.AddNewRole();
        //    aa1.Name = TranslationHelper.TranlslateText($"_{UFUserEditorManagerComponent.StringPlaceolder}_AdminRoleName", stringlist, Properties.Settings.Default.AdminRoleName);
        //    aa1.AccessLevel =  Properties.Settings.Default.AdminRoleLevel;
        //    var aa2 = document.AddNewRole();
        //    aa2.Name = TranslationHelper.TranlslateText($"_{UFUserEditorManagerComponent.StringPlaceolder}_PowerUserRoleName", stringlist, Properties.Settings.Default.PowerUserRoleName);
        //    aa2.AccessLevel = Properties.Settings.Default.PowerUserRoleLevel;
        //    var aa3 = document.AddNewRole();
        //    aa3.Name = TranslationHelper.TranlslateText($"_{UFUserEditorManagerComponent.StringPlaceolder}_GuestRoleName", stringlist, Properties.Settings.Default.GuestRoleName);
        //    aa3.AccessLevel = Properties.Settings.Default.GuestRoleLevel;
        //}
        public UFRole AddNewRole(String name)
        {
            if(document != null)
                return document.AddNewRole(name);
            return null;
        }
        public UFRole GetRole(String name)
        {
            if (document != null)
                return document.GetRoleByName(name);
            return null;
        }
        public UsersReturnCode DeleteRole(String name, bool forceDelete = false)
        {
            if (document == null)
                return UsersReturnCode.NoDocument;
            var roleToDelete = document.GetRoleByName(name);
            if(roleToDelete != null)
            {
                if (roleToDelete.UFUsers.Count > 0 && !forceDelete)
                    return UsersReturnCode.NoForce;

                roleToDelete.Delete();
                return UsersReturnCode.Ok;
            }
            return UsersReturnCode.NoObject;
        }

        public List<UFRole> GetRoleList()
        {
            if(document != null)
                return document.GetRoles().ToList();
            return null;
        }
        public UFUser AddUser(String name, UFRole role)
        {
            if (document != null)
                return document.AddNewUser(role, name);
            return null;
        }
        public UFUser GetUser(String name)
        {
            if (document != null)
                return document.GetUserByName(name);
            return null;
        }
        public UsersReturnCode DeleteUser(String name)
        {
            if (document == null)
                return UsersReturnCode.NoDocument;
            var userTodelete = document.GetUserByName(name);
            if (userTodelete != null)
            {
                userTodelete.Delete();
                return UsersReturnCode.Ok;
            }
            return UsersReturnCode.NoObject;
        }
        public List<UFUser> GetUserList(UFRole role = null)
        {
            if (document != null)
                return document.GetUserCollection(role).ToList();
            return null;
        }
        public UFUserSettings GetGeneralSettings()
        {
            if (document != null)
                return document.GetGeneralUserSettings();
            return null;
        }

        public void Save()
        {
            if (document != null && document.NeedsSave)
                document.SaveToFile();
        }

        public void Dispose()
        {
            if (document != null)
            {
                document.Dispose();
                document = null;
            }
        }
        #endregion Methods

    }
}
