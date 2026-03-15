using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocumentManager.ComponentService;
#if !NET_STANDARD
using UFUAEditor.ComponentService;
using System.Windows.Controls;
using VFS;
using UIMsgBoxAlertService.ComponentService;
using UFInterfaces.AuthenticationCredentialsProvider;
using System.Web.Security;
using WPFUtilities.Extensions;
using TranslationHelpers;
using DevExpress.Xpf.Core;
using UFUserEditor.Controls;
using System.Reactive.Concurrency;
#endif
using System.ComponentModel;
using ViewModelLib;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using System.Diagnostics;
using System.ServiceModel;
using Utilities;
using XpoHelpers;
using System.Threading.Tasks;
using System.IO;
using UFUserEditor.ComponentService;
using System.Text.RegularExpressions;
using System.Xml;
using log4net;
using System.Windows;
using System.Security;
using System.DirectoryServices.AccountManagement;


namespace UFUserEditor.Document
{
    public class UFUserDocument : ViewModelBase,
#if !NET_STANDARD
        ICloneable,
#endif
        IDocument, IXpoDocument
    {
        #region Declarations

        InMemoryDataStore InMemory;
#if !NET_STANDARD
        InMemoryDataStore InMemoryClipboard;
#endif
        UnitOfWork uow;
#if !NET_STANDARD
        UnitOfWork uowClipboard;
        UnitOfWork uowCloner;
#endif
        IDataLayer dl;
#if !NET_STANDARD
        IDataLayer dlClipboard;
#endif
        String connectionString;
        String connectionStringBackup;
        String fileBase;
        bool isSharedConnectionRepository;
        String sharedApplicationName;
        bool isBackupDatabaseConnected;
        IDisposable sharedRepositoriesSynchronizationTimerDisposable;
        readonly CachedUnitOfWorks cachedUnitOfWorks;
        
#if !NET_STANDARD
        List<UserAuditChange> userAuditChanges;
        readonly Dictionary<UserControl, XpoHelpers.UndoRedoIXPSimpleObjectHelper> UndoRedoHelper = new Dictionary<UserControl, XpoHelpers.UndoRedoIXPSimpleObjectHelper>();
        readonly Dictionary<XpoHelpers.UndoRedoIXPSimpleObjectHelper, IDataLayer> mapdlUndoRedo = new Dictionary<XpoHelpers.UndoRedoIXPSimpleObjectHelper, IDataLayer>();
        readonly Dictionary<IDataLayer, UnitOfWork> mapuowUndoRedo = new Dictionary<IDataLayer, UnitOfWork>();

        private IScheduler _scheduler;
        public IScheduler Scheduler
        {
            private get
            {
                return _scheduler ?? (_scheduler = DefaultScheduler.Instance);
            }
            set
            {
                _scheduler = value;
            }
        }
#endif

        bool needToReloadRoleList;
        List<string> listCachedRoleNames;
        object tagsListLock = new object();
        static object updateSharedRepositoryLock = new object();

#if !NET_STANDARD
        internal static readonly ILog log = LogManager.GetLogger(Properties.Resources.UserManager);
#else
        internal static readonly ILog log = LogManager.GetLogger(System.Reflection.Assembly.GetEntryAssembly(), Properties.Resources.UserManager);
#endif

        #endregion

        #region Constructors
        public UFUserDocument()
        {
            cachedUnitOfWorks = new CachedUnitOfWorks(this);
        }
        #endregion


        #region Methods
#if !NET_STANDARD
        internal NestedUnitOfWork BeginNestedUnitOfWork()
        {
            return uow.BeginNestedUnitOfWork();
        }

        internal XPObject GetNestedObject(object obj)
        {
            if (obj == null || !(obj is XPObject) || (obj as XPObject).IsLoading || (obj as XPObject).IsDeleted)
                return null;

            UowContext = BeginNestedUnitOfWork();
            return UowContext.GetNestedObject(obj as XPObject);
        }

        internal List<XPObject> GetNestedObjects(System.Collections.IList objects)
        {
            var list = (from c in objects.OfType<XPObject>() where !c.IsLoading && !c.IsDeleted select c).ToList();
            if (list.Count() == 0)
                return null;

            UowContext = BeginNestedUnitOfWork();

            var ret = new List<XPObject>(list.Count());
            list.ForEach(obj =>
            {
                ret.Add(UowContext.GetNestedObject(obj));
            });

            return ret;
        }

        List<XPObject> GetParentObjects(System.Collections.IList objects)
        {
            var list = (from c in objects.OfType<XPObject>() where !c.IsLoading && !c.IsDeleted select c).ToList();
            if (list.Count == 0 || UowContext == null)
                return null;

            var ret = new List<XPObject>(list.Count);
            list.ForEach(obj =>
            {
                if (XpoHelpers.XpoHelper.IsSessionObject(obj, UowContext))
                    ret.Add(UowContext.GetParentObject(obj));
            });

            return ret;
        }
#endif
        public IDataLayer GetDataLayer()
        {
            DevExpress.Xpo.Metadata.XPDictionary dict = new DevExpress.Xpo.Metadata.ReflectionDictionary();
            dict.GetDataStoreSchema(typeof(UFUserModel.UFUser).Assembly);
            dict.GetDataStoreSchema(typeof(XpoHelpers.ProtectionFile));

            if (String.IsNullOrEmpty(fileBase))
            {
                try
                {
                    return XpoDefault.GetDataLayer(connectionString, DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema);
                }
                catch (Exception ex)
                {
                    if (isSharedConnectionRepository && !String.IsNullOrEmpty(connectionStringBackup) &&
                        String.Compare(connectionString, connectionStringBackup, true) != 0)
                    {
                        var prevConnectionString = connectionString;
                        connectionString = connectionStringBackup;
                        connectionStringBackup = prevConnectionString;
                        try
                        {
                            return XpoDefault.GetDataLayer(connectionString, DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema);
                        }
                        catch (Exception ex2)
                        {
                            throw new Exception(String.Format(Properties.Resources.ErrorOpeningSharedRepository, connectionString), ex2);
                        }
                    }

                    if (isSharedConnectionRepository)
                        throw new Exception(String.Format(Properties.Resources.ErrorOpeningSharedRepository, connectionString), ex);
                    else
                        throw;
                }
            }
            else
            {
                if (InMemory == null)
                {
                    InMemory = new InMemoryDataStore(DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema, true);
                    if (File.Exists(fileBase))
                    {
                        if (Protected || !Utilities.IO.FileSystem.IsXmlFile(fileBase))
                        {
                            var data = WPFUtilities.CryptString.CryptString.DecryptString(File.ReadAllText(fileBase));
                            using (var reader = new MemoryStream(Convert.FromBase64String(data)))
                            {
                                var xmlreader = XmlReader.Create(reader);
                                try
                                {
                                    InMemory.ReadXml(xmlreader);
                                }
                                catch (Exception ex)
                                {

                                }
                            }
                        }
                        else
                            try
                            {
                                InMemory.ReadXml(fileBase);
                            }
                            catch (Exception ex)
                            {

                            }
                    }
                }

                return new ThreadSafeDataLayer(dict, InMemory);
            }
        }

        void CreateDataLayer()
        {
            dl = GetDataLayer();
            uow = new UnitOfWork(dl);
#if !NET_STANDARD
            uowCloner = new UnitOfWork(dl);
            uow.ObjectChanged += (o, e) =>
            {
                OnPropertyChanged("NeedsSave");
                if (e.Object is UFUserModel.UFRole)
                    needToReloadRoleList = true;
            };

            InMemoryClipboard = new InMemoryDataStore(DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema, true);
            dlClipboard = new SimpleDataLayer(InMemoryClipboard);
            uowClipboard = new UnitOfWork(dlClipboard);
#endif
        }

        bool IsBelongFromParent(IDocument parent)
        {
            var id = XpoHelpers.XpoHelper.GetProtectionCode(uow);
            return id == Guid.Empty || id == parent.Id;
        }

        UnitOfWork GetSession()
        {
            return uow;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public UFUserModel.UFUserSettings GetGeneralUserSettings()
        {
            return GetGeneralUserSettings(false);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public UFUserModel.UFUserSettings GetGeneralUserSettings(bool threadsafe)
        {
            if (threadsafe)
            {
                {
                    using (var task = cachedUnitOfWorks.BeginUnitOfWork())
                    {
                        return GetGeneralUserSettings(task.UnitOfWork);
                    }
                }
            }
            else
            {
                return GetGeneralUserSettings(uow);
            }
        }

        internal UFUserModel.UFUserSettings GetGeneralUserSettings(UnitOfWork uow)
        {
            var list = (from tag in new XPQuery<UFUserModel.UFUserSettings>(uow, true) select tag).ToList();
            if (list.Count == 0)
                return new UFUserModel.UFUserSettings(uow);
            return list[0];
        }

        void EnsureDefaultSettings()
        {
            var configuration = GetGeneralUserSettings();
            if (isSharedConnectionRepository)
            {
                configuration.SharedConnectionRepository = null;
                configuration.SharedConnectionRepositoryBackup = null;
                uow.LockingOption = LockingOption.None;
            }
        }

        void EnsureUniqueSharedUsersAndGroups(UnitOfWork uow)
        {
            if (!isSharedConnectionRepository)
                return;

            var mapRoles = new Dictionary<String, UFUserModel.UFRole>();
            var roles = GetRoles(uow);
            foreach (var role in roles)
            {
                if (mapRoles.ContainsKey(role.Name))
                {
                    var duplicated = mapRoles[role.Name];
                    if (uow.IsNewObject(duplicated))
                        duplicated = role;
                    uow.Delete(duplicated);
                }
                else
                {
                    mapRoles.Add(role.Name, role);
                }
            }

            var mapUsers = new Dictionary<String, UFUserModel.UFUser>();
            var users = GetFlatUserCollection(uow);
            foreach (var user in users)
            {
                if (mapUsers.ContainsKey(user.Name))
                {
                    var duplicated = mapUsers[user.Name];
                    if (uow.IsNewObject(duplicated))
                        duplicated = user;
                    uow.Delete(duplicated);
                }
                else
                {
                    mapUsers.Add(user.Name, user);
                }
            }
        }

        #region Users and Roles
        [EditorBrowsable(EditorBrowsableState.Never)]
        public UFUserModel.UFUser GetUserByName(string name)
        {
            return (from user in new XPQuery<UFUserModel.UFUser>(uow, true)// .AsParallel()
                    where user.Name == name
                    select user).SingleOrDefault();
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public UFUserModel.UFRole GetRoleByName(string name)
        {
            return (from role in new XPQuery<UFUserModel.UFRole>(uow, true)// .AsParallel()
                    where role.Name == name
                    select role).SingleOrDefault();
        }

        internal IList<String> GetRoleNames()
        {
            lock (tagsListLock)
            {
                if (listCachedRoleNames != null)
                    return listCachedRoleNames;

                //using (var d = GetDataLayer())
                {
                    using (var task = cachedUnitOfWorks.BeginUnitOfWork())
                    {
                        var lRole = (from p in new XPQuery<UFUserModel.UFRole>(task.UnitOfWork, true)// .AsParallel()
                                orderby p.Name
                                select p.Name).ToList();
                        if (listCachedRoleNames == null)
                            listCachedRoleNames = new List<string>();
                        listCachedRoleNames.AddRange(lRole);

                        return lRole;
                    }
                }
            }
        }

        internal IList<String> GetUserNames()
        {
            //using (var d = GetDataLayer())
            {
                using (var task = cachedUnitOfWorks.BeginUnitOfWork())
                {
                    return (from p in new XPQuery<UFUserModel.UFUser>(task.UnitOfWork, true)// .AsParallel()
                            orderby p.Name
                            select p.Name).ToList();
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public IList<UFUserModel.UFRole> GetRoles(UnitOfWork u = null)
        {
            var currentuow = u;
            if (currentuow == null)
                currentuow = uow;

            return (from p in new XPQuery<UFUserModel.UFRole>(currentuow, true)
                    orderby p.Name
                    select p).ToList();              
        }

        internal int GetRoleAccessLevel(String roleName)
        {
            return (from role in new XPQuery<UFUserModel.UFRole>(uow, true)// .AsParallel()
                    where role.Name == roleName
                    select role.AccessLevel).FirstOrDefault(); //default for int is 0
        }

        internal String GetUserRole(String user)
        {
            //using (var d = GetDataLayer())
            {
                using (var task = cachedUnitOfWorks.BeginUnitOfWork())
                {
                    var list = (from p in new XPQuery<UFUserModel.UFUser>(task.UnitOfWork, true).AsParallel()
                                where (String.Compare(p.Name, user, true) == 0 || String.Compare(p.Name, user.Trim(), true) == 0) &&
                                p.UFRoleAss != null
                                select p).ToList();
                    if (list.Count > 0)
                        return list[0].UFRoleAss.Name;

                    var roles = Utilities.CurrentUser.GetGroups(user, true);
                    if (roles != null)
                    {
                        foreach (var role in roles)
                        {
                            var listRoles = (from p in new XPQuery<UFUserModel.UFRole>(task.UnitOfWork, true)// .AsParallel()
                                             where p.Name.ToLower() == role.ToLower()
                                             select p).ToList();
                            if (listRoles.Count > 0)
                                return listRoles[0].Name;
                        }
                    }
                }
            }

            return String.Empty;
        }

        internal String GetUserElectronicSignature(String user)
        {
            //using (var d = GetDataLayer())
            {
                using (var task = cachedUnitOfWorks.BeginUnitOfWork())
                {
                    var list = (from p in new XPQuery<UFUserModel.UFUser>(task.UnitOfWork, true).AsParallel()
                                where String.Compare(p.Name, user, true) == 0 || String.Compare(p.Name, user.Trim(), true) == 0

                                select p).ToList();
                    if (list.Count > 0)
                        return list[0].ElectronicSignature;
                }
            }

            return String.Empty;
        }

        internal bool GetIsOldPassword(String user, String password)
        {
            //using (var d = GetDataLayer())
            {
                using (var task = cachedUnitOfWorks.BeginUnitOfWork())
                {
                    var list = (from p in new XPQuery<UFUserModel.UFUser>(task.UnitOfWork, true).AsParallel()
                                where String.Compare(p.Name, user, true) == 0 || String.Compare(p.Name, user.Trim(), true) == 0

                                select p).ToList();
                    if (list.Count > 0)
                    {
                        return GetGeneralUserSettings(task.UnitOfWork).CheckUserPasswordInHistory(list[0], password);
                    }
                }
            }

            return false;
        }

        internal int VerifyUser(String user, String password)
        {
            //using (var d = GetDataLayer())
            {
                using (var task = cachedUnitOfWorks.BeginUnitOfWork())
                {
                    var list = (from p in new XPQuery<UFUserModel.UFUser>(task.UnitOfWork, true).AsParallel()
                                where !p.Disabled &&
                                	  (String.Compare(p.Name, user, true) == 0 || String.Compare(p.Name, user.Trim(), true) == 0 ) &&
                                      String.Compare(p.Password, password, false) == 0
                                select p).ToList();
                    if (list.Count > 0)
                    {
                        bool bExpired = list[0].ForcePasswordChangeFirstLogin;
                        if (bExpired)
                            return -1;
                        if (list[0].PasswordExpiresInDays > 0)
                        {
                            var diff = DateTime.UtcNow - list[0].LastDateTimeChanged;
                            bExpired = diff.TotalDays >= list[0].PasswordExpiresInDays;
                            if (bExpired)
                                return -1;
                            var settings = GetGeneralUserSettings(task.UnitOfWork);
                            if (settings.NotifyPasswordExpiresInDays > 0)
                            {
                                var daysleft = list[0].PasswordExpiresInDays - diff.TotalDays;
                                if (settings.NotifyPasswordExpiresInDays > daysleft)
                                    return 2;
                            }
                        }

                        return 1;
                    }
                }
            }

            return 0;
        }

        static int nCounter = 0;
        internal bool UpdateUser(String user, String oldpassword, String newpassword)
        {
            //using (var d = GetDataLayer())
            {
                using (var task = cachedUnitOfWorks.BeginUnitOfWork())
                {
                    var userSettings = GetGeneralUserSettings(task.UnitOfWork);
                    if (String.IsNullOrEmpty(newpassword) && newpassword.Length < userSettings.MinRequiredPasswordLength)
                        return false;

                    var list = (from p in new XPQuery<UFUserModel.UFUser>(task.UnitOfWork, true).AsParallel()
                                where !p.Disabled &&
                                	  (String.Compare(p.Name, user, true) == 0 || String.Compare(p.Name, user.Trim(), true) == 0) &&
                                      String.Compare(p.Password, oldpassword, false) == 0
                                select p).ToList();
                    if (list.Count > 0)
                    {
                        if (!userSettings.AddUserPasswordInHistory(list[0], oldpassword, newpassword))
                            return false;

                        nCounter = 0;
                        list[0].Password = newpassword;
                        list[0].LastDateTimeChanged = DateTime.UtcNow;
                        list[0].ForcePasswordChangeFirstLogin = false;
                        SaveToFile(u: task.UnitOfWork, bForceSave: true);
                        return true;
                    }
                }
            }

            if (++nCounter > 3)
                System.Threading.Thread.Sleep(nCounter * 1000);

            return false;
        }

        internal bool VerifyPasswordExpired(String user, bool bForce = false)
        {
#if !NET_STANDARD
            var uiInterface = GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
#endif

            //using (var d = GetDataLayer())
            {
                using (var task = cachedUnitOfWorks.BeginUnitOfWork())
                {
                    var list = (from p in new XPQuery<UFUserModel.UFUser>(task.UnitOfWork, true).AsParallel()
                                where !p.Disabled && (String.Compare(p.Name, user, true) == 0 || String.Compare(p.Name, user.Trim(), true) == 0)
                                select p).ToList();

                    string details = null;
                    if (list.Count > 0)
                    {
                        bool bExpired = list[0].ForcePasswordChangeFirstLogin || bForce;
                        if (bExpired)
                            details = bForce ? Properties.Resources.AuditPasswordCommand : Properties.Resources.AuditPasswordFirstLogin;
                        if (!bExpired && list[0].PasswordExpiresInDays > 0)
                        {
                            var diff = DateTime.UtcNow - list[0].LastDateTimeChanged;
                            bExpired = diff.TotalDays >= list[0].PasswordExpiresInDays;
                            if (bExpired)
                                details = Properties.Resources.AuditPasswordExpired;
#if !NET_STANDARD
                            else if (uiInterface != null)
                            {
                                var settings = GetGeneralUserSettings(task.UnitOfWork);
                                if (settings.NotifyPasswordExpiresInDays > 0)
                                {
                                    var daysleft = list[0].PasswordExpiresInDays - diff.TotalDays;
                                    if (settings.NotifyPasswordExpiresInDays > daysleft)
                                    {
                                        bExpired = bForce = uiInterface.ShowYesNo(String.Format(Properties.Resources.PasswordIsExpiring.Replace("--newline--", Environment.NewLine), Math.Ceiling(daysleft)), CustomDialogIcons.Question) == CustomDialogResults.Yes;
                                        details = Properties.Resources.AuditPasswordChangedOnLogin;
                                    }
                                }
                            }
#endif
                        }

#if !NET_STANDARD
                        var oldPassword = list[0].Password;
                        if (bExpired)
                        {
                            var verifyPassword = new VerifyPasswordEventArgs()
                            {
                                User = list[0].Name,
                                bForce = bForce
                            };
                            list[0].Password = list[0].PasswordConfirm = String.Empty;
                            while (true)
                            {
                                UFUserEditorManagerComponent.userEditorManagerComponent.OnPromptVerifyPassword(verifyPassword);
                                if (!verifyPassword.requestingVerifyPassword)
                                    break;

                                if (bForce && verifyPassword.dialogResult == false)
                                    return false;
                                {
                                    var settings = GetGeneralUserSettings(task.UnitOfWork);
                                    list[0].Password = list[0].PasswordConfirm = verifyPassword.Password;

                                    var error = list[0].Error;
                                    if (!String.IsNullOrEmpty(error))
                                    {
                                        verifyPassword.Password = String.Empty;
                                        list[0].Password = list[0].PasswordConfirm = String.Empty;
                                        if (uiInterface != null)
                                            uiInterface.ShowError(error);
                                        continue;
                                    }
                                    else if (oldPassword != verifyPassword.OldPassword)
                                    {
                                        verifyPassword.Password = String.Empty;
                                        list[0].Password = list[0].PasswordConfirm = String.Empty;
                                        //if (++nCounter > 3)
                                        //{
                                        //    list[0].Password = oldPassword;
                                        //    return false;
                                        //}
                                        if (uiInterface != null)
                                            uiInterface.ShowError(Properties.Resources.OldPasswordWrong);
                                        continue;
                                    }
                                    else if (list[0].Password == oldPassword)
                                    {
                                        verifyPassword.Password = String.Empty;
                                        list[0].Password = list[0].PasswordConfirm = String.Empty;
                                        if (uiInterface != null)
                                            uiInterface.ShowError(Properties.Resources.NewPasswordCannotBeOldPassword);
                                        continue;
                                    }
                                    else if (!settings.AddUserPasswordInHistory(list[0], oldPassword, list[0].Password))
                                    {
                                        if (uiInterface != null)
                                            uiInterface.ShowError(Properties.Resources.NewPasswordCannotBeReusedOldPassword);
                                        continue;
                                    }

                                    list[0].Password = list[0].PasswordConfirm = verifyPassword.Password;
                                    list[0].LastDateTimeChanged = DateTime.UtcNow;
                                    list[0].ForcePasswordChangeFirstLogin = false;
                                    AddAuditMessage(String.Format(Properties.Resources.AuditUserPasswordChanged, list[0].Name), null, null, list[0].Name, details);
                                    SaveToFile(u: task.UnitOfWork, bForceSave: true);
                                    return true;
                                }
                            }
                        }

                        Window curwnd = null;
                        if (System.Windows.Input.Keyboard.FocusedElement is DependencyObject)
                            curwnd = Window.GetWindow(System.Windows.Input.Keyboard.FocusedElement as DependencyObject);

                        if (bExpired)
                        {
                            var dlg = new UFUserEditor.Controls.PaswordExpired(this)
                            {
                                DataContext = list[0]
                            };
                            // int nCounter = 0;
                            list[0].Password = list[0].PasswordConfirm = String.Empty;

                            IDictionary<String, String> stringlist = null;
                            string stringPlaceolder = "UserDocument";
                            var stringManager = UFUserEditorManagerComponent.userEditorManagerComponent.StringEditorManager;
                            if (stringManager != null)
                                stringlist = stringManager.GetListStringForCulture(this, stringManager.GetActiveCulture(this));
                            var captions = new Dictionary<GeneralDialogButtons, String>()
                            {
                                { GeneralDialogButtons.OkButton, TranslationHelper.TranlslateText("_UserDialog_OkLabel", stringlist, WPFUtilities.Properties.Resources.LabelOk) },
                                { GeneralDialogButtons.CloseButton, TranslationHelper.TranlslateText("_UserDialog_CloseLabel", stringlist, WPFUtilities.Properties.Resources.LabelClose) },
                                { GeneralDialogButtons.CancelButton, TranslationHelper.TranlslateText("_UserDialog_CancelLabel", stringlist, WPFUtilities.Properties.Resources.LabelCancel) }
                            };

                            string title = bForce ? TranslationHelper.TranlslateText($"_{stringPlaceolder}_ChangePassword", stringlist, Properties.Resources.ChangePassword) : TranslationHelper.TranlslateText($"_{stringPlaceolder}_PasswordExpired", stringlist, Properties.Resources.PasswordExpired);
                            while (true)
                            {
                                GeneralDialogContent Dialog = new GeneralDialogContent(dlg, WPFUtilities.Properties.Settings.Default.DialogFontSize, WPFUtilities.Properties.Settings.Default.ButtonsWidth, WPFUtilities.Properties.Settings.Default.ButtonsHeight,
                                    bForce ? GeneralDialogButtons.OkCancelButtons : 
                                             GeneralDialogButtons.CloseButton | GeneralDialogButtons.CannotCancel, captions)
                                {
                                    Owner = curwnd,
                                    Title = title,
                                    HelpLink = "PasswordExpired"
                                };
                                var ret = Dialog.ShowDialog();
                                if (bForce && ret == false)
                                    return false;
                                {
                                    var settings = GetGeneralUserSettings(task.UnitOfWork);
                                    if (dlg.textOldPassword.Password != oldPassword)
                                    {
                                        dlg.textOldPassword.Password = String.Empty;
                                        list[0].Password = list[0].PasswordConfirm = String.Empty;
                                        //if (++nCounter > 3)
                                        //{
                                        //    list[0].Password = oldPassword;
                                        //    return false;
                                        //}
                                        if (uiInterface != null)
                                            uiInterface.ShowError(Properties.Resources.OldPasswordWrong);
                                        continue;
                                    }
                                    else if (list[0].Password == oldPassword)
                                    {
                                        dlg.textOldPassword.Password = String.Empty;
                                        list[0].Password = list[0].PasswordConfirm = String.Empty;
                                        if (uiInterface != null)
                                            uiInterface.ShowError(Properties.Resources.NewPasswordCannotBeOldPassword);
                                        continue;
                                    }
                                    else if (!settings.AddUserPasswordInHistory(list[0], oldPassword, list[0].Password))
                                    {
                                        if (uiInterface != null)
                                            uiInterface.ShowError(Properties.Resources.NewPasswordCannotBeReusedOldPassword);
                                        continue;
                                    }

                                    list[0].LastDateTimeChanged = DateTime.UtcNow;
                                    list[0].ForcePasswordChangeFirstLogin = false;
                                    AddAuditMessage(String.Format(Properties.Resources.AuditUserPasswordChanged, list[0].Name), null, null, list[0].Name, details);
                                    SaveToFile(u: task.UnitOfWork, bForceSave: true);
                                    return true;
                                }
                                //else
                                //{
                                //    list[0].Password = oldPassword;
                                //    return false;
                                //}
                            }
                        }
#else
                        return !bExpired;
#endif
                    }
                }
            }

            return true;
        }

        internal Dictionary<String, String> GetUsersEmail(String NodeId)
        {
            //using (var d = GetDataLayer())
            {
                using (var task = cachedUnitOfWorks.BeginUnitOfWork())
                {
                    Dictionary<String, String> _dictionary = new Dictionary<String, String>();

                    var rlist = (from p in new XPQuery<UFUserModel.UFRole>(task.UnitOfWork, true)// .AsParallel()
                                 where p.NodeId == new Guid(NodeId)
                                 select p).ToList();
                    if (rlist.Count != 0)
                    {
                        var ulist = (from p in new XPQuery<UFUserModel.UFUser>(task.UnitOfWork, true)// .AsParallel()
                                     where p.UFRoleAss == rlist[0]
                                     select p).ToList();
                        ulist.ForEach(x => _dictionary.Add(x.Name, x.Email));
                        return _dictionary;
                    }

                    var sulist = (from p in new XPQuery<UFUserModel.UFUser>(task.UnitOfWork, true)// .AsParallel()
                                  where p.NodeId == new Guid(NodeId) && p.UFRoleAss != null
                                  select p).ToList();

                    sulist.ForEach(x => _dictionary.Add(x.Name, x.Email));
                    return _dictionary;
                }
            }
        }
        internal IList<String> GetSMTPSettings()
        {
            //using (var d = GetDataLayer())
            {
                using (var task = cachedUnitOfWorks.BeginUnitOfWork())
                {
                    List<String> _list = new List<String>();
                    UFUserModel.UFUserSettings _configuration;

                    var list = (from tag in new XPQuery<UFUserModel.UFUserSettings>(task.UnitOfWork, true)/*.AsParallel() */select tag).ToList();
                    if (list.Count == 0)
                        _configuration = new UFUserModel.UFUserSettings(task.UnitOfWork);
                    else
                        _configuration = list[0];

                    _list.Add(_configuration.StaticFromAddress);
                    _list.Add(_configuration.ServerAddress);
                    _list.Add(_configuration.PortNumber.ToString());
                    _list.Add(_configuration.EnAutentication.ToString());
                    _list.Add(_configuration.UserName);
                    _list.Add(_configuration.Password);

                    return _list;
                }
            }
        }

        internal int GetSharedRepositoriesSynchronizationIntervalInMinutes()
        {
            using (var task = cachedUnitOfWorks.BeginUnitOfWork())
            {
                return GetGeneralUserSettings(task.UnitOfWork).SharedRepositoriesSynchIntervalInMinutes;
            }
        }

        internal int GetUserAccessLevel(String user)
        {
            //using (var d = GetDataLayer())
            {
                using (var task = cachedUnitOfWorks.BeginUnitOfWork())
                {
                    var list = (from p in new XPQuery<UFUserModel.UFUser>(task.UnitOfWork, true).AsParallel()
                                where String.Compare(p.Name, user, true) == 0 || String.Compare(p.Name, user.Trim(), true) == 0
                                select p).ToList();
                    if (list.Count > 0)
                        return list[0].GlobalAccessLevel;

                    //var groups = UserPrincipal.Current.GetGroups();
                    //var roles = groups.Select(x => x.SamAccountName).ToList();
                    var roles = Utilities.CurrentUser.GetGroups(user, true);
                    if (roles != null)
                    {
                        foreach (var role in roles)
                        {
                            var listRoles = (from p in new XPQuery<UFUserModel.UFRole>(task.UnitOfWork, true)// .AsParallel()
                                             where p.Name.ToLower() == role.ToLower()
                                             select p).ToList();
                            if (listRoles.Count > 0)
                                return listRoles[0].AccessLevel;
                        }
                    }
                }
            }

            return 0;
        }

        internal int GetAutoLogoutSeconds(String user)
        {
            //using (var d = GetDataLayer())
            {
                using (var task = cachedUnitOfWorks.BeginUnitOfWork())
                {
                    var list = (from p in new XPQuery<UFUserModel.UFUser>(task.UnitOfWork, true).AsParallel()
                                where String.Compare(p.Name, user, true) == 0 || String.Compare(p.Name, user.Trim(), true) == 0
                                select p).ToList();
                    if (list.Count > 0)
                    {
                        if (list[0].AutoLogoutSeconds > 0)
                            return list[0].AutoLogoutSeconds;
                        if (list[0].UFRoleAss != null && list[0].UFRoleAss.AutoLogoutSeconds > 0)
                            return list[0].UFRoleAss.AutoLogoutSeconds;
                        else
                            return 0;
                    }

                    //var groups = UserPrincipal.Current.GetGroups();
                    //var roles = groups.Select(x => x.SamAccountName).ToList();
                    var roles = Utilities.CurrentUser.GetGroups(user, true);
                    if (roles != null)
                    {
                        foreach (var role in roles)
                        {
                            var listRoles = (from p in new XPQuery<UFUserModel.UFRole>(task.UnitOfWork, true)// .AsParallel()
                                             where p.Name.ToLower() == role.ToLower()
                                             select p).ToList();
                            if (listRoles.Count > 0)
                                return listRoles[0].AutoLogoutSeconds;
                        }
                    }
                }
            }

            return 0;
        }

        internal int GetDaysLeftPasswordExpires(String user)
        {
            //using (var d = GetDataLayer())
            {
                using (var task = cachedUnitOfWorks.BeginUnitOfWork())
                {
                    var list = (from p in new XPQuery<UFUserModel.UFUser>(task.UnitOfWork, true).AsParallel()
                                where String.Compare(p.Name, user, true) == 0 || String.Compare(p.Name, user.Trim(), true) == 0
                                select p).ToList();
                    if (list.Count > 0)
                    {
                        if (list[0].PasswordExpiresInDays > 0)
                        {
                            var diff = DateTime.UtcNow - list[0].LastDateTimeChanged;
                            return (int)Math.Ceiling(list[0].PasswordExpiresInDays - diff.TotalDays);
                        }

                        return 0;
                    }
                }
            }

            return 0;
        }

        internal int GetUserAccessMask(String user)
        {
            //using (var d = GetDataLayer())
            {
                using (var task = cachedUnitOfWorks.BeginUnitOfWork())
                {
                    var list = (from p in new XPQuery<UFUserModel.UFUser>(task.UnitOfWork, true).AsParallel()
                                where String.Compare(p.Name, user, true) == 0 || String.Compare(p.Name, user.Trim(), true) == 0
                                select p).ToList();
                    if (list.Count > 0)
                        return list[0].GlobalAccessMask;

                    //var groups = UserPrincipal.Current.GetGroups();
                    //var roles = groups.Select(x => x.SamAccountName).ToList();
                    var roles = Utilities.CurrentUser.GetGroups(user, true);
                    if (roles != null)
                    {
                        foreach (var role in roles)
                        {
                            var listRoles = (from p in new XPQuery<UFUserModel.UFRole>(task.UnitOfWork, true)// .AsParallel()
                                             where p.Name.ToLower() == role.ToLower()
                                             select p).ToList();
                            if (listRoles.Count > 0 && listRoles[0].AccessMask != null)
                                return listRoles[0].AccessMask.Value;
                        }
                    }
                }
            }

            return 0;
        }

        internal String GetUserCultureName(String user)
        {
            //using (var d = GetDataLayer())
            {
                using (var task = cachedUnitOfWorks.BeginUnitOfWork())
                {
                    var list = (from p in new XPQuery<UFUserModel.UFUser>(task.UnitOfWork, true).AsParallel()
                                where String.Compare(p.Name, user, true) == 0 || String.Compare(p.Name, user.Trim(), true) == 0
                                select p).ToList();
                    if (list.Count > 0)
                        return list[0].GlobalCultureName;
                }
            }

            return String.Empty;
        }

        internal String GetUserConverterName(String user)
        {
            //using (var d = GetDataLayer())
            {
                using (var task = cachedUnitOfWorks.BeginUnitOfWork())
                {
                    var list = (from p in new XPQuery<UFUserModel.UFUser>(task.UnitOfWork, true).AsParallel()
                                where String.Compare(p.Name, user, true) == 0 || String.Compare(p.Name, user.Trim(), true) == 0
                                select p).ToList();
                    if (list.Count > 0)
                        return list[0].GlobalConverterName;
                }
            }

            return String.Empty;
        }
#if !NET_STANDARD
        String NewRoleName(String name = null, String format = "{0}{1}")
        {
            if (String.IsNullOrEmpty(name))
                name = Properties.Settings.Default.DefaultRoleName;

            ulong counter = 1;
            string fmtzero = "0";
            string baseName = name;
            Utilities.NewNameHelper.ParseName(name, out baseName, out fmtzero, out counter);

            while (RoleNameExists(name))
                name = String.Format(format, baseName, (counter++).ToString(fmtzero));
            return name;
        }

        internal bool RoleNameExists(String name)
        {
            return (from role in new XPQuery<UFUserModel.UFRole>(uow, true)// .AsParallel()
                    where role.Name == name
                    select role).ToList().Count > 0;
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public UFUserModel.UFRole AddNewRole(String name = null)
        {
            var role = new UFUserModel.UFRole(uow) { Name = NewRoleName(name), NodeId = Guid.NewGuid(), AccessLevel = Properties.Settings.Default.GuestRoleLevel };
            return role;
        }

        UFUserModel.UFRole FindRoleByNodeId(Guid guid)
        {
            return (from folder in new XPQuery<UFUserModel.UFRole>(uow, true).AsParallel()
                    where folder.NodeId == guid
                    select folder).FirstOrDefault();
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public UFUserModel.UFUser AddNewUser(UFUserModel.UFRole root, String name = null)
        {
            var tag = new UFUserModel.UFUser(uow) { Name = NewUserName(name), NodeId = Guid.NewGuid(), AccessLevel = -1, AccessMask = -1, LastDateTimeChanged = DateTime.UtcNow };
            if (root != null)
                root.UFUsers.Add(tag);
            return tag;
        }

        String NewUserName(/*UFUserModel.UFRole root,*/ String name = null, IDictionary<string, ulong> mapcounter = null, IList<String> listname = null, String format = "{0}{1}")
        {
            if (String.IsNullOrEmpty(name))
                name = Properties.Settings.Default.DefaultUserName;

            if (listname == null)
                listname = GetUsersNameList();

            return Utilities.NewNameHelper.FindNewName(name, listname, mapcounter, format);
        }

        internal bool UserNameExists(String name/*, UFUserModel.UFRole root*/)
        {
            //if (root == null)
            {
                return (from user in new XPQuery<UFUserModel.UFUser>(uow, true)// .AsParallel()
                        where /*user.UFRoleAss == null &&*/ user.Name == name
                        select user).ToList().Count > 0;
            }

            //return (from c in root.UFUsers/*.AsParallel()*/ where c.Name == name select c).ToList().Count > 0;
        }
#endif

        internal IList<UFUserModel.UFUser> GetFlatUserCollection(UnitOfWork u = null)
        {
            var currentuow = u;
            if (currentuow == null)
                currentuow = uow;

            return (from tag in new XPQuery<UFUserModel.UFUser>(currentuow, true)// .AsParallel()
                    orderby tag.Oid
                    select tag).ToList();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public IList<UFUserModel.UFUser> GetUserCollection(UFUserModel.UFRole root = null)
        {
            if (root == null)
            {
                return (from user in new XPQuery<UFUserModel.UFUser>(uow, true)
                        orderby user.Oid
                        select user).ToList();
            }

            var folders = (from folder in new XPQuery<UFUserModel.UFRole>(uow, true)
                            where folder.Oid == root.Oid
                            select folder).ToList();

            if (folders.Count == 0)
                return new List<UFUserModel.UFUser>();
            else
                return folders[0].UFUsers;   
        }

        internal IList<String> GetUserCollection(String root)
        {
            if (String.IsNullOrEmpty(root))
            {
                return (from user in new XPQuery<UFUserModel.UFUser>(uow, true)
                        where user.UFRoleAss == null
                        orderby user.Name
                        select user.Name).ToList();
            }

            var folders = (from folder in new XPQuery<UFUserModel.UFRole>(uow, true)
                           where folder.Name == root
                           select folder).ToList();

            if (folders.Count == 0)
                return new List<String>();
            else
                return (from user in folders[0].UFUsers
                        orderby user.Name
                        select user.Name).ToList();
        }

#if !NET_STANDARD
        IList<String> GetRolesNodeIdList()
        {
            return (from p in new XPQuery<UFUserModel.UFRole>(uow, true)// .AsParallel()
                    select p.NodeId.ToString()).ToList();
        }
        IList<String> GetRolesNameList()
        {
            return (from folder in new XPQuery<UFUserModel.UFRole>(uow, true)// .AsParallel()
                    select folder.Name).ToList();
        }
#endif
        #endregion

        static String GetConnectionString(String path
#if !NET_STANDARD
            , FileSystemProviderBase vfs
#endif
            )
        {
            String connString = null;
#if !NET_STANDARD
            if (vfs != null && vfs is DataSourceFileSystemProvider)
            {
                connString = (vfs as DataSourceFileSystemProvider).ConnectionString;
            }
            else
#endif
            {
                var xmlfile = String.Format("{0}/{1}/{2}{3}", path,
                                    Properties.Settings.Default.TypeLabel,
                                    Properties.Settings.Default.DefaultProjectName,
                                    Properties.Settings.Default.DefaultFileExt);

                connString = InMemoryDataStore.GetConnectionString(String.Format("\"{0}\"", xmlfile));
            }

            return connString;
        }

        internal static String GetBaseFilename(String path
#if !NET_STANDARD
            , FileSystemProviderBase vfs
#endif
            )
        {
#if !NET_STANDARD
            if (vfs != null)
                return null;
#endif

            return String.Format("{0}/{1}/{2}{3}", path,
                                Properties.Settings.Default.TypeLabel,
                                Properties.Settings.Default.DefaultProjectName,
                                Properties.Settings.Default.DefaultFileExt);
        }

#if !NET_STANDARD
        public static void CopyFile(String fullPath, String newPath, bool bCopy,
            IDocument parent, IDocumentManager manager = null)
        {
            using (var sourceDoc = FromFile(fullPath, manager, parent, bCreateNew: false, bCheckEmpty: false, bCheckShared: false))
            {
                if (sourceDoc == null)
                    return;

                CopyFile(sourceDoc, newPath);
            }

            if (!bCopy)
                RemoveFile(fullPath, parent);
        }

        internal static void MergeFile(String sourcePath, String targetPath,
                                        MergeRepositoryModel sharedRepositoryViewModel,
                                        IDocument parent, IDocumentManager manager = null)
        {
            if (!sharedRepositoryViewModel.CopyAllSettings && !sharedRepositoryViewModel.CopyUsersAndGroups)
                return;

            using (var sourceDoc = FromFile(sourcePath, manager, parent, bCreateNew: false, bCheckEmpty: false, bCheckShared: false))
            {
                if (sourceDoc == null)
                    return;

                MergeFile(sourceDoc, targetPath, sharedRepositoryViewModel.CopyAllSettings, sharedRepositoryViewModel.CopyUsersAndGroups, sharedRepositoryViewModel.ReplaceExistingsUsers);
            }
        }
#endif

        internal static void CopyFile(UFUserDocument sourceDoc, String newPath)
        {
            var targetConn = newPath;
            if (!XpoHelpers.XpoHelper.IsDataSource(targetConn))
                targetConn = GetConnectionString(newPath
#if !NET_STANDARD
                    , null
#endif
                    );
            using (var dlTarget = XpoDefault.GetDataLayer(targetConn, DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema))
            {
                using (var uowTarget = new UnitOfWork(dlTarget))
                {
                    var cloneHelper = new XpoHelpers.CloneIXPSimpleObjectHelper(sourceDoc.GetSession(), uowTarget, false, true, true);

                    ////////////////////////////////////////////////////////////////////////////
                    // delete all first
                    (from p in new XPQuery<UFUserModel.UFUser>(uowTarget, true)
                     select p).ToList().ForEach(tag => tag.Delete());

                    (from p in new XPQuery<UFUserModel.UFRole>(uowTarget, true)
                     select p).ToList().ForEach(tag => tag.Delete());

                    (from p in new XPQuery<UFUserModel.UFUserSettings>(uowTarget, true)
                     select p).ToList().ForEach(item => item.Delete());
                    ////////////////////////////////////////////////////////////////////////////

                    var roles = sourceDoc.GetRoles();
                    foreach (var role in roles)
                    {
                        cloneHelper.Clone(role, false);
                    }

                    var sourceUsers = sourceDoc.GetUserCollection();
                    foreach (var sourceUser in sourceUsers)
                    {
                        cloneHelper.Clone(sourceUser, false);
                    }

                    cloneHelper.Clone(sourceDoc.GetGeneralUserSettings(), false);

                    uowTarget.CommitChanges();
                    uowTarget.PurgeDeletedObjects();
                }
            }
        }

        internal static void MergeFile(UFUserDocument sourceDoc, String targetPath, bool bCopyAllSettings = true, bool bCopyUsersAndGroups = true, bool bReplaceExistingsUsers = true)
        {
            var targetConn = targetPath;
            if (!XpoHelpers.XpoHelper.IsDataSource(targetConn))
                targetConn = GetConnectionString(targetPath
#if !NET_STANDARD
                        , null
#endif
                        );
            using (var dlTarget = XpoDefault.GetDataLayer(targetConn, DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema))
            {
                using (var uowTarget = new UnitOfWork(dlTarget))
                {
                    var cloneHelper = new XpoHelpers.CloneIXPSimpleObjectHelper(sourceDoc.GetSession(), uowTarget,
                        checkattributes: true, copyaggregated: false, copyassociation: false);

                    if (bCopyUsersAndGroups)
                    {
                        var existingRoles = (from c in new XPQuery<UFUserModel.UFRole>(uowTarget, true)
                                             select c).ToList();

                        var roles = sourceDoc.GetRoles();
                        foreach (var role in roles)
                        {
                            var found = (from c in existingRoles where c.Name == role.Name select c).ToList();
                            if (found.Count == 0)
                                cloneHelper.Clone(role, false);
                        }

                        var existingUsers = (from c in new XPQuery<UFUserModel.UFUser>(uowTarget, true)
                                             select c).ToList();
                        var users = sourceDoc.GetFlatUserCollection();
                        foreach (var user in users)
                        {
                            var found = (from c in existingUsers where c.Name == user.Name select c).ToList();
                            if (bReplaceExistingsUsers)
                            {
                                while (found.Count > 0)
                                {
                                    found[0].Delete();
                                    found.RemoveAt(0);
                                }
                            }

                            if (found.Count == 0)
                            {
                                var name = sourceDoc.GetUserRole(user.Name);
                                var role = (from c in new XPQuery<UFUserModel.UFRole>(uowTarget, true)
                                            where c.Name == name
                                            select c).FirstOrDefault();
                                if (role != null)
                                    role.UFUsers.Add(cloneHelper.Clone(user, false));
                            }
                        }
                    }

                    if (bCopyAllSettings)
                    {
                        (from p in new XPQuery<UFUserModel.UFUserSettings>(uowTarget, true)
                         select p).ToList().ForEach(item => item.Delete());

                        cloneHelper.Clone(sourceDoc.GetGeneralUserSettings(), false);
                    }

                    uowTarget.CommitChanges();
                    uowTarget.PurgeDeletedObjects();
                }
            }
        }

#if !NET_STANDARD
        public static void RenameFile(String fullPath, String oldName, String newName,
            FileSystemProviderBase fileSystemProvider = null)
        {
        }

        public static void RemoveFile(string fullPath, IDocument parent, IDocumentManager manager = null)
        {
            FileSystemProviderBase fileSystemProvider = parent.fileSystemProviderBase;
            var file = GetBaseFilename(fullPath, fileSystemProvider);
            if (!String.IsNullOrEmpty(file))
            {
                try
                {
                    File.Delete(file);
                }
                catch (Exception ex)
                {

                }
            }
            else
            {
                using (var sourceDoc = FromFile(fullPath, manager, parent, bCreateNew: false, bCheckEmpty: false, bCheckShared: false))
                {
                    if (sourceDoc == null)
                        return;
                    var roles = sourceDoc.GetRoles();
                    foreach (var role in roles)
                    {
                        role.Delete();
                    }
                    var users = sourceDoc.GetFlatUserCollection();
                    foreach (var user in users)
                    {
                        user.Delete();
                    }

                    sourceDoc.GetSession().CommitChanges();
                }
            }
        }

        IList<String> GetUsersNodeIdList()
        {
            return (from p in new XPQuery<UFUserModel.UFUser>(uow, true)// .AsParallel()
                    select p.NodeId.ToString()).ToList();
        }

        IList<String> GetUsersNameList()
        {
                return (from tag in new XPQuery<UFUserModel.UFUser>(uow, true)// .AsParallel()
                        select tag.Name).ToList();
        }
#endif
        public static UFUserDocument FromFile(String path, IDocumentManager c, IDocument parent, 
            bool bCreateNew = true, bool bCheckEmpty = true, bool bCheckShared = true)
        {
            try
            {
                if (String.IsNullOrEmpty(path))
                    return null;

#if !NET_STANDARD
                FileSystemProviderBase vfs = parent.fileSystemProviderBase;
#endif
                String connString = GetConnectionString(path
#if !NET_STANDARD
                    , vfs
#endif
                    );
                String xmlfile = GetBaseFilename(path
#if !NET_STANDARD
                    , vfs
#endif
                    );

#if NET_STANDARD
                String fileCore = xmlfile + Properties.Settings.Default.CoreExt;
                try
                {
                    if (!File.Exists(fileCore))
                        File.Copy(xmlfile, fileCore);
                    xmlfile = fileCore;
                }
                catch {}
#endif

                if (!bCreateNew && !String.IsNullOrEmpty(xmlfile) && !File.Exists(xmlfile))
                    return null;

                //if (!String.IsNullOrEmpty(xmlfile))
                //{
                //    if (!Utilities.IO.FileSystem.IsXmlFile(fullPath))
                //    {
                //    }
                //}

                var doc = new UFUserDocument(null)
                {
                    connectionString = connString,
#if !NET_STANDARD
                    EditorManagerComponent = c as UFUserEditorManagerComponent,
#endif
                    fileBase = xmlfile,
                    Parent = parent
                };

                doc.CreateDataLayer();
                if (!bCreateNew && bCheckEmpty && doc.IsEmpty)
                {
                    doc.Dispose();
                    return null;
                }
                else if (
#if !NET_STANDARD
                    vfs == null && 
#endif
                    !doc.IsBelongFromParent(parent))
                {
                    doc.Dispose();
#if !NET_STANDARD
                    var uiMsgBox = parent.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                    if (uiMsgBox != null)
                        uiMsgBox.ShowError(String.Format(Properties.Resources.ErrorValidatingDocument, path));
#else
                    log.ErrorFormat(Properties.Resources.ErrorValidatingDocument, path);
#endif
                    return null;
                }

                doc.EnsureDefaultSettings();

                var sharedConnectionRepository = XpoHelpers.XpoHelper.NormalizeConnectionString(doc.GetGeneralUserSettings().SharedConnectionRepository, parent.rootBase);
                var sharedConnectionRepositoryBackup = XpoHelpers.XpoHelper.NormalizeConnectionString(doc.GetGeneralUserSettings().SharedConnectionRepositoryBackup, parent.rootBase);
                var sharedApplicationName = XpoHelpers.XpoHelper.GetDataSourceTitle(sharedConnectionRepository, onlytitle: true);
                if (bCheckShared && !String.IsNullOrEmpty(sharedConnectionRepository))
                {
                    UFUserDocument newDoc = null;
                    try
                    {
                        newDoc = new UFUserDocument(null)
                        {
                            connectionString = sharedConnectionRepository,
                            isSharedConnectionRepository = true,
                            sharedApplicationName = sharedApplicationName,
#if !NET_STANDARD
                            EditorManagerComponent = c as UFUserEditorManagerComponent
#endif
                        };

                        newDoc.CreateDataLayer();
                        newDoc.EnsureDefaultSettings();
                        newDoc.connectionStringBackup = sharedConnectionRepositoryBackup;
                        var settings = newDoc.GetGeneralUserSettings();
                        settings.SharedConnectionRepository = XpoHelpers.XpoHelper.AddPlaceholderToConnectionString(sharedConnectionRepository, parent.rootBase);
                        settings.SharedConnectionRepositoryBackup = XpoHelpers.XpoHelper.AddPlaceholderToConnectionString(sharedConnectionRepositoryBackup, parent.rootBase);

                        try
                        {
                            newDoc.GetSession().FlushChanges();
                        }
                        catch
                        { }

                        doc.Dispose();
                        doc = newDoc;
                    }
                    catch (Exception ex)
                    {
                        log.Error(String.Format(Properties.Resources.ErrorOpeningSharedRepository, sharedConnectionRepository), ex);
                        if (newDoc != null)
                        {
                            newDoc.Dispose();
                            newDoc = null;
                        }
                    }

                    if (newDoc == null && !String.IsNullOrEmpty(sharedConnectionRepositoryBackup))
                    {
                        try
                        {
                            newDoc = new UFUserDocument(null)
                            {
                                connectionString = sharedConnectionRepositoryBackup,
                                isSharedConnectionRepository = true,
                                sharedApplicationName = sharedApplicationName,
#if !NET_STANDARD
                                EditorManagerComponent = c as UFUserEditorManagerComponent
#endif
                            };

                            newDoc.CreateDataLayer();
                            newDoc.EnsureDefaultSettings();
                            var settings = newDoc.GetGeneralUserSettings();
                            newDoc.connectionStringBackup = sharedConnectionRepository;
                            settings.SharedConnectionRepository = XpoHelpers.XpoHelper.AddPlaceholderToConnectionString(sharedConnectionRepository, parent.rootBase);
                            settings.SharedConnectionRepositoryBackup = XpoHelpers.XpoHelper.AddPlaceholderToConnectionString(sharedConnectionRepositoryBackup, parent.rootBase);

                            try
                            {
                                newDoc.GetSession().FlushChanges();
                            }
                            catch
                            { }

                            doc.Dispose();
                            doc = newDoc;
                        }
                        catch (Exception ex)
                        {
                            log.Error(String.Format(Properties.Resources.ErrorOpeningSharedRepository, sharedConnectionRepositoryBackup), ex);
                            if (newDoc != null)
                            {
                                newDoc.Dispose();
                                newDoc = null;
                            }
                        }
                    }

                    if (newDoc == null)
                    {
#if !NET_STANDARD
                        var uiMsgBox = parent.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                        if (uiMsgBox != null)
                            uiMsgBox.ShowError(String.Format(Properties.Resources.ErrorOpeningSharedRepository, sharedConnectionRepository));
#endif
                    }
                    else if (!bCreateNew && bCheckEmpty && newDoc.IsEmpty)
                    {
                        newDoc.Dispose();
                        return null;
                    }
                }

                return doc;
            }
            catch
            {
#if !NET_STANDARD
                var uiMsgBox = parent.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                if (uiMsgBox != null)
                    uiMsgBox.ShowError(String.Format(Properties.Resources.ErrorReadingDocument, path));
#else
                log.ErrorFormat(Properties.Resources.ErrorReadingDocument, path);
#endif
                return null;
            }
        }

        public bool SaveToFile(bool discargechanges = false, bool bForceSave = false, UnitOfWork u = null, bool forceEncryption = false)
        {
            var currentuow = u;
            if (currentuow == null)
                currentuow = uow;
            if (currentuow == null)
                return false;

#if !NET_STANDARD
            List<XPObject> parentObjects = null;
            if (ActiveView != null && currentuow == uow && 
                EditorManagerComponent.Workspace != null &&
                EditorManagerComponent.Workspace.ActiveWindow == ActiveView)
            {
                EditorManagerComponent.Workspace.UpdateContextNow();
                var objects = EditorManagerComponent.Workspace.ContextObjects;
                if (EditorManagerComponent.Workspace.ContextObject != null)
                {
                    if (objects == null)
                        objects = new List<object>();
                    objects.Add(EditorManagerComponent.Workspace.ContextObject);
                }

                if (objects != null)
                    parentObjects = GetParentObjects(objects);
            }
#endif

            try
            {
#if !NET_STANDARD
                bool bSave = NeedsSave;
                if (discargechanges &&
                    currentuow.TryPurgeDeletedObjects(log) > 0)
                    bSave = true;

                if (!bForceSave && !bSave)
                    return false;
#endif
                EnsureUniqueSharedUsersAndGroups(currentuow);
                GetGeneralUserSettings(currentuow).LastChangedTime = DateTime.UtcNow;

                if (!String.IsNullOrEmpty(fileBase))
                {
                    if (forceEncryption || Protected)
                        XpoHelpers.XpoHelper.AddProtectionCode(currentuow, Id);
                    else
                        XpoHelpers.XpoHelper.RemoveProtectionCode(currentuow);
                }
#if !NET_STANDARD
                CheckIfBackupDatabaseIsConnectedAndManageTriggers();
#endif
                currentuow.CommitChanges();

                if (!String.IsNullOrEmpty(fileBase))
                {
                    if (File.Exists(fileBase))
                    {
                        try
                        {
                            File.Delete(fileBase);
                        }
                        catch (Exception ex)
                        {
                        }
                    }

                    if (forceEncryption || Protected)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            var writer = XmlWriter.Create(memoryStream);
                            InMemory.WriteXml(writer);
                            writer.Flush();
                            writer.Close();
                            var str = Convert.ToBase64String(memoryStream.ToArray());
                            var toWrite = WPFUtilities.CryptString.CryptString.EncryptString(str);
                            File.WriteAllText(fileBase, toWrite);
                        }
                    }
                    else
                        InMemory.WriteXml(fileBase);
                }

                if (needToReloadRoleList)
                {
                    needToReloadRoleList = false;
                    lock (tagsListLock)
                    {
                        if(listCachedRoleNames != null)
                            listCachedRoleNames.Clear();
                        listCachedRoleNames = null;
                    }
                }

#if !NET_STANDARD
                if(EditorManagerComponent.AuthenticationCredentialsProvider != null)
                    MapToCredentialProvider(u);

                if (EditorManagerComponent.ProjectManager != null && userAuditChanges != null && userAuditChanges.Count > 0)
                { 
                    var sourceName = System.Net.Dns.GetHostName();
                    foreach (var audit in userAuditChanges)
                    {
                        EditorManagerComponent.ProjectManager.AddLogEntity(parent, sourceName, audit.UtcTime, audit.Message,
                            audit.Details, String.Empty, audit.UserName, System.Diagnostics.EventLogEntryType.Information);
                    }
                    userAuditChanges.Clear();
                }
#endif
            }
            catch (Exception ex)
            {
#if !NET_STANDARD
                var uiInterface = GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                if (uiInterface != null)
                    uiInterface.ShowError(String.Format(Properties.Resources.ErrorSavingDocument, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
#endif
                return false;
            }

#if !NET_STANDARD
            if (parentObjects != null && parentObjects.Count > 0)
            {
                if (parentObjects.Count == 1)
                    EditorManagerComponent.Workspace.ContextObject = GetNestedObject(parentObjects[0]);
                else
                    EditorManagerComponent.Workspace.ContextObject = GetNestedObjects(parentObjects);
            }
#endif

            if (isSharedConnectionRepository &&
                !String.IsNullOrEmpty(connectionStringBackup) && 
                String.Compare(connectionString, connectionStringBackup, true) != 0 &&
                isBackupDatabaseConnected)
            {
                try
                {
                    UFUserDocument.CopyFile(this, connectionStringBackup);
                }
                catch (Exception ex)
                {
                    log.ErrorFormat(Properties.Resources.ErrorSavingDocument, ex.Message);
                }
            }

            return true;
        }

        #endregion

        #region Properties
#if !NET_STANDARD
        [Browsable(false)]
        public bool NeedsSave
        {
            get { return uow != null && uow.TrackingChanges; }
        }

        [Browsable(false)]
        public bool IsDisposed
        {
            get { return bObjectDisposed; }
        }

        NestedUnitOfWork uowContext;
        [Browsable(false)]
        internal NestedUnitOfWork UowContext
        {
            get
            {
                return uowContext;
            }
            set
            {
                if (uowContext == value)
                    return;

                uowContext = value;
                OnPropertyChanged("UowContext");
            }
        }

        UFUserEditorManagerComponent editorManagerComponent;
        [Browsable(false)]
        public UFUserEditorManagerComponent EditorManagerComponent
        {
            get
            {
                return editorManagerComponent;
            }
            private set
                    {
                        editorManagerComponent = value;
                    }
        }

        public String ConnectionString
        {
            get
            {
                return connectionString;
            }
        }
#endif
        [Browsable(false)]
        public bool IsSharedConnectionRepository
        {
            get
            {
                return isSharedConnectionRepository;
            }
        }

        [Browsable(false)]
        public String SharedApplicationName
        {
            get
            {
                return sharedApplicationName;
            }
        }
        #endregion

        #region ICloneable Members
        UFUserDocument(UFUserDocument template) : this()
        {
            if (template == null)
                return;

            throw new NotImplementedException();
        }
#if !NET_STANDARD
        public object Clone()
        {
            return new UFUserDocument(this);
        }
#endif
        #endregion

        #region IDocument

        public event EventHandler Disposing;
        virtual public void OnDisposing(Object sender)
        {
            EventHandler temp = Disposing;
            if (temp != null)
                temp(sender, EventArgs.Empty);
        }
#if !NET_STANDARD
        UserControl activeView;
        [Browsable(false)]
        public UserControl ActiveView
        {
            get
            {
                return activeView;
            }
            set
            {
                activeView = value;
            }
        }

        [Browsable(false)]
        public UserControl View
        {
            get
            {
                return activeView;
            }
        }
#endif
        IDocument parent;
        [Browsable(false)]
        public IDocument Parent
        {
            get
            {
                return parent;
            }
            set
            {
                parent = value;
            }
        }

        [Browsable(false)]
        public IList<IDocument> Childs
        {
            get
            {
                return null;
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public String ProjectType
        {
            get
            {
                if (Parent != null)
                    return Parent.ProjectType;
                return String.Empty;
            }
        }

        [Browsable(false)]
        public String Theme
        {
            get
            {
                if (Parent != null)
                    return Parent.Theme;
                return String.Empty;
            }
        }

        public Uri GetSpecialFolder(SpecialFolders specialFolder)
        {
            if (Parent != null)
                return Parent.GetSpecialFolder(specialFolder);
            return null;
        }

        public Uri MakeAbosoluteUri(Uri relative)
        {
            if (relative == null)
                return null;

            if (Parent != null)
                return Parent.MakeAbosoluteUri(relative);
            if (relative.IsAbsoluteUri)
                return relative;

            // return new Uri(Path.GetDirectoryName(FullPath) + "\\").MakeRelativeUri(absolute);
            return null;
        }

        public IDocument UpdateParentFromUri(Uri relative)
        {
            if (Parent != null)
                return Parent.UpdateParentFromUri(relative);
            return this;
        }

        public Uri MakeRelativeUri(Uri absolute)
        {
            if (absolute == null)
                return null;

            if (Parent != null)
                return Parent.MakeRelativeUri(absolute);
            if (!absolute.IsAbsoluteUri)
                return absolute;

            // return absolute.MakeRelativeUri(new Uri(Path.GetDirectoryName(FullPath) + "\\"));
            return null;
        }

#if !NET_STANDARD
        [Browsable(false)]
        public FileSystemProviderBase fileSystemProviderBase
        {
            get
            {
                if (Parent != null)
                    return Parent.fileSystemProviderBase;
                return new PhysicalFileSystemProvider("");
            }
        }
#endif

        [Browsable(false)]
        public String rootBase
        {
            get
            {
                if (Parent != null)
                    return Parent.rootBase;
                return Path.GetDirectoryName(fileBase);
            }
        }

        [Browsable(false)]
        public String rootBaseDB
        {
            get
            {
                if (Parent != null)
                    return Parent.rootBaseDB;
                return String.Empty;
            }
        }

        [Browsable(false)]
        public bool Protected
        {
            get
            {
                if (Parent != null)
                    return Parent.Protected;
                return false;
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Guid Id
        {
            get
            {
                if (Parent != null)
                    return Parent.Id;
                return Guid.Empty;
            }
        }

        public String Title
        {
            get
            {
                return Path.GetFileNameWithoutExtension(fileBase);
            }
        }

        [Browsable(false)]
        public String FilePath
        {
            get
            {
                return fileBase;
            }
        }

        [Browsable(false)]
        public bool IsEmpty
        {
            get
            {
                return GetFlatUserCollection().Count == 0;
            }
        }

        [Browsable(false)]
        public bool IsRoot
        {
            get
            {
                return false;
            }
        }

        public Object GetService(Type type)
        {
            if (Parent != null)
                return Parent.GetService(type);
            return null;
        }

        #endregion

        #region IDisposable
        protected bool bObjectDisposed;
        protected override void OnDispose()
        {
            if (bObjectDisposed)
                return;
            bObjectDisposed = true;

            if (sharedRepositoriesSynchronizationTimerDisposable != null)
            {
                sharedRepositoriesSynchronizationTimerDisposable.Dispose();
                sharedRepositoriesSynchronizationTimerDisposable = null;
            }

            OnDisposing(this);

            base.OnDispose();

#if !NET_STANDARD

            if (uowContext != null)
            {
                uowContext.Dispose();
                uowContext = null;
            }
#endif
            if (uow != null)
            {
                uow.Disconnect();
                uow.Dispose();
                uow = null;
            }
#if !NET_STANDARD
            if (uowCloner != null)
            {
                uowCloner.Disconnect();
                uowCloner.Dispose();
                uowCloner = null;
            }
            if (uowClipboard != null)
            {
                uowClipboard.Disconnect();
                uowClipboard.Dispose();
                uowClipboard = null;
            }
#endif
            if (dl != null)
            {
                dl.Dispose();
                dl = null;
            }
#if !NET_STANDARD
            UndoRedoHelper.Clear();
            foreach (var entry in mapuowUndoRedo.Values)
            {
                entry.Disconnect();
                entry.Dispose();
            }
            mapuowUndoRedo.Clear();

            foreach (var entry in mapdlUndoRedo.Values)
            {
                entry.Dispose();
            }
            mapdlUndoRedo.Clear();

#endif
            lock (tagsListLock)
            {
                if(listCachedRoleNames != null)
                    listCachedRoleNames.Clear();
                listCachedRoleNames = null;
            }
        }

        #endregion

        #region Methods
#if !NET_STANDARD
        internal void TerminateAndLogoff()
        {
            try
            {
                EditorManagerComponent.AuthenticationCredentialsProvider.Logout(parent.Title);
            }
            catch (Exception ex)
            {
                log.Error(Properties.Resources.ErrorTerminating, ex);
            }
        }

        internal void MapToCredentialProvider(UnitOfWork u = null)
        {
            using (new WaitCursor())
            {
                if (isSharedConnectionRepository)
                    EditorManagerComponent.AuthenticationCredentialsProvider.SetSharedApplicationName(parent.Title, sharedApplicationName);
                else
                    EditorManagerComponent.AuthenticationCredentialsProvider.RemoveSharedApplicationName(parent.Title);

                var users = GetFlatUserCollection(u);
                var roles = GetRoles(u);
                var helper = new CredentialsProviderHelper(parent.Title, EditorManagerComponent.AuthenticationCredentialsProvider, users, roles);
                helper.ErrorMapping += (s, e) =>
                {
                    log.Error(e.Message, e.Exception);
                };
                helper.MapToCredentialProvider();

                var settings = GetGeneralUserSettings(u ?? uow);
                EditorManagerComponent.AuthenticationCredentialsProvider.EnableValidationOnOS(parent.Title, settings.EnableValidationOnOS);
                MembershipProviderSettings.MembershipProviderSettings.SetMaxInvalidPasswordAttempts(parent.Title, settings.MaxInvalidPasswordAttempts);

                MembershipProviderSettings.MembershipProviderSettings.ClearUnlockableUsers(parent.Title);
                foreach (var user in users)
                {
                    if (settings.UserLockMode == UFUserModel.UserLockType.None ||
                        (settings.UserLockMode == UFUserModel.UserLockType.OnlyEditableUsers &&
                        user.GlobalAccessLevel > settings.MaxRuntimeEditAccessLevel))
                        MembershipProviderSettings.MembershipProviderSettings.AddUnlockableUser(parent.Title, user.Name);
                }
            }
        }

        internal void UpdateSharedRepository()
        {
            lock (updateSharedRepositoryLock)
            {
                if (isSharedConnectionRepository &&
                !String.IsNullOrEmpty(connectionStringBackup) &&
                String.Compare(connectionString, connectionStringBackup, true) != 0)
                {
                    try
                    {
                        using (var doc = new UFUserDocument(null))
                        {
                            doc.connectionString = connectionStringBackup;
                            doc.isSharedConnectionRepository = true;
                            doc.sharedApplicationName = sharedApplicationName;
                            doc.CreateDataLayer();

                            if (doc.GetGeneralUserSettings().LastChangedTime > GetGeneralUserSettings().LastChangedTime)
                                UFUserDocument.CopyFile(doc, connectionString);
                            else if (doc.GetGeneralUserSettings().LastChangedTime < GetGeneralUserSettings().LastChangedTime)
                                UFUserDocument.CopyFile(this, connectionStringBackup);
                        }
                    }
                    catch (Exception ex)
                    {
                        log.ErrorFormat(Properties.Resources.ErrorSavingDocument, ex.Message);
                    }
                }
            }    
        }
        
        internal void RemoveSharedRepositoryTriggers()
        {
            var sqlQueries = PrepareQueriesForSharedRepositoryTriggersRemoval();

            RemoveTriggersRelativeToSelectedDatabase(connectionString, sqlQueries);
            RemoveTriggersRelativeToSelectedDatabase(connectionStringBackup, sqlQueries);
        }

        internal void AddSharedRepositoryTriggers()
        {
            if (!isSharedConnectionRepository)
                return;

            String membershipConnectionString = null;
            String serverName = null;

            try
            {
                membershipConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["LocalSqlServer"].ConnectionString;
            }
            catch
            { }

            if (membershipConnectionString == null)
            {
                log.Warn(Properties.Resources.MSSQLSharedRepositoryWarning);
                return;
            }
            
            var helper = new DevExpress.Xpo.DB.Helpers.ConnectionStringParser(membershipConnectionString);
            if (helper.PartExists("server"))
                serverName = helper.GetPartByName("server");
            else if (helper.PartExists("data source"))
                serverName = helper.GetPartByName("data source");

            if (serverName == null)
            {
                log.Warn(Properties.Resources.MSSQLSharedRepositoryWarning);
                return;
            }
                   
            var normalizedServerName = NormalizeServerName(serverName);

            String userName = null;
            if (helper.PartExists("uid"))
                userName = helper.GetPartByName("uid");
            else if (helper.PartExists("user id"))
                userName = helper.GetPartByName("user id");

            String password = null;
            if (helper.PartExists("pwd"))
                password = helper.GetPartByName("pwd");
            else if (helper.PartExists("password"))
                password = helper.GetPartByName("password");

            var splitString = new String[] { "--newline--" };
            var sqlQueriesDictionary = new Dictionary<string, string[]>();

            var sqlQuery = Properties.Settings.Default.sp_removemembership_drop;
            sqlQueriesDictionary.Add(Properties.Settings.Default.sp_removemembership_drop, sqlQuery.Split(splitString, StringSplitOptions.None));

            sqlQuery = Properties.Settings.Default.sp_removemembership;
            sqlQueriesDictionary.Add(Properties.Settings.Default.sp_removemembership, sqlQuery.Split(splitString, StringSplitOptions.None));

            sqlQuery = String.Format(Properties.Settings.Default.sp_addlinkedserver, normalizedServerName, userName != null ? "FALSE" : "TRUE", userName ?? "NULL", password ?? "NULL");
            sqlQueriesDictionary.Add(Properties.Settings.Default.sp_addlinkedserver, sqlQuery.Split(splitString, StringSplitOptions.None));

            sqlQuery = String.Format(Properties.Settings.Default.tr_removemembership_drop, normalizedServerName);
            sqlQueriesDictionary.Add(Properties.Settings.Default.tr_removemembership_drop, sqlQuery.Split(splitString, StringSplitOptions.None));

            sqlQuery = String.Format(Properties.Settings.Default.tr_removemembership, normalizedServerName, sharedApplicationName.ToLower());
            sqlQueriesDictionary.Add(Properties.Settings.Default.tr_removemembership, sqlQuery.Split(splitString, StringSplitOptions.None));

            if (String.IsNullOrEmpty(connectionString) || !XpoHelpers.XpoHelper.IsMSSQlDataProvider(connectionString))
                sqlQueriesDictionary.Clear();
            if (String.IsNullOrEmpty(connectionStringBackup) || !XpoHelpers.XpoHelper.IsMSSQlDataProvider(connectionStringBackup))
                sqlQueriesDictionary.Clear();

            if (sqlQueriesDictionary.Count > 0)
            {
                if (!String.IsNullOrEmpty(connectionString))
                {
                    var sqlQueriesToExecuteOnMainDb = 
                        BuildSqlQueriesArrayForTriggersAddition(
                            sqlQueriesDictionary, 
                            serverName, 
                            normalizedServerName, 
                            connectionString);

                    ExecuteNonQuery(connectionString, sqlQueriesToExecuteOnMainDb);
                }
                    
                if (!String.IsNullOrEmpty(connectionStringBackup))
                {
                    var sqlQueriesToExecuteOnBackupDb =
                        BuildSqlQueriesArrayForTriggersAddition(
                            sqlQueriesDictionary,
                            serverName,
                            normalizedServerName,
                            connectionStringBackup);

                    ExecuteNonQuery(connectionStringBackup, sqlQueriesToExecuteOnBackupDb);
                }  
            }
            else
                log.Warn(Properties.Resources.MSSQLSharedRepositoryWarning);
        }

        private string [] BuildSqlQueriesArrayForTriggersAddition(
            Dictionary<string, string[]> sqlQueriesDictionary,
            string originalServerName,
            string normalizedServerName,
            string connectionString)
        {
            var currentSqlQueriesDictionary = new Dictionary<string, string[]>(sqlQueriesDictionary);
            List<string> updatedQueriesListToReturn = new List<string>();
           
            if (connectionString.ToLower().Contains(originalServerName.ToLower()) || connectionString.ToLower().Contains(normalizedServerName.ToLower()))
            {
                currentSqlQueriesDictionary.Remove(Properties.Settings.Default.sp_addlinkedserver);
            }

            foreach (KeyValuePair<string, string[]> kvp in currentSqlQueriesDictionary)
            {
                updatedQueriesListToReturn.AddRange(kvp.Value);
            }

            return updatedQueriesListToReturn.ToArray();
        }

        void ExecuteNonQuery(string connectionString, string[] sqlQueries)
        {
            using (var d = XpoDefault.GetDataLayer(connectionString, DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema))
            {
                using (var uow = new UnitOfWork(d))
                {
                    for (int ii = 0; ii < sqlQueries.Length; ii++)
                        uow.ExecuteNonQuery(sqlQueries[ii]);
                }
            }
        }

        internal void CreateUndoRedoHelper(UserControl owner)
        {
            CreateUndoRedoHelper(owner, Properties.Settings.Default.MaxUndoRedoActions);
        }

        internal void CreateUndoRedoHelper(UserControl owner, short numactions)
        {
            if (UndoRedoHelper.ContainsKey(owner))
                return;

            var dlundoredo = XpoDefault.GetDataLayer(InMemoryDataStore.GetConnectionStringInMemory(true), DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema);
            var uowundoredo = new UnitOfWork(dlundoredo);
            var helper = numactions > 0 ? new XpoHelpers.UndoRedoIXPSimpleObjectHelper(uow, uowundoredo, numactions) : new XpoHelpers.UndoRedoIXPSimpleObjectHelper(uow, uowundoredo);
            UndoRedoHelper[owner] = helper;
            mapdlUndoRedo[helper] = dlundoredo;
            mapuowUndoRedo[dlundoredo] = uowundoredo;
        }

        internal void AddAuditMessage(String message, XPObject oldObj, XPObject newObj, String currentUserName, String details = null)
        {
            if (userAuditChanges == null)
                userAuditChanges = new List<UserAuditChange>();

            if (newObj == null && oldObj is UFUserModel.UFRole)
            {
                var role = oldObj as UFUserModel.UFRole;
                foreach (var user in role.UFUsers)
                    AddAuditMessage(String.Format(Properties.Resources.AuditUserRemoved, user.Name), user, null, currentUserName);
            }

            if (details == null && oldObj != null && newObj != null)
                details = GetPropertiesChangedFlatString(oldObj, newObj);

            userAuditChanges.Add(new UserAuditChange()
            {
                UtcTime = DateTime.UtcNow,
                Message = message,
                Details = details,
                UserName = currentUserName
            });

            var count = userAuditChanges.Count;
            while (userAuditChanges.Count > Properties.Settings.Default.AuditMessageCache)
                userAuditChanges.RemoveAt(0);
            if (count != userAuditChanges.Count)
                log.WarnFormat(Properties.Resources.AuditMessageCacheExceeded, Properties.Settings.Default.AuditMessageCache);

            if (oldObj == null && newObj is UFUserModel.UFRole)
            {
                var role = newObj as UFUserModel.UFRole;
                foreach (var user in role.UFUsers)
                    AddAuditMessage(String.Format(Properties.Resources.AuditUserAdded, user.Name), null, user, currentUserName);
            }
        }

        String GetPropertiesChangedFlatString(XPObject oldObj, XPObject newObj)
        {
            var flatChanges = new StringBuilder();
            foreach (DevExpress.Xpo.Metadata.XPMemberInfo m in oldObj.ClassInfo.PersistentProperties)
            {
                if (m.IsKey || m.IsReadOnly || m.ReferenceType != null)
                    continue;

                ValueConverterAttribute converter = null;
                if (m.Attributes != null)
                {
                    converter = (from c in m.Attributes
                                 where c is ValueConverterAttribute
                                 select (c as ValueConverterAttribute)).FirstOrDefault();
                }

                var oldVal = m.GetValue(oldObj);
                var newVal = m.GetValue(newObj);
                if (!Object.Equals(oldVal, newVal))
                {
                    var displayName = m.DisplayName;
                    if (String.IsNullOrEmpty(displayName))
                        displayName = m.Name;

                    if (flatChanges.Length > 0)
                        flatChanges.Append(", ");
                    //flatChanges.AppendFormat("{0}", displayName);

                    if (converter != null)
                    {
                        if (converter.Converter != null && converter.Converter.StorageType == typeof(String))
                        {
                            if (converter.Converter is UFUserModel.EncryptedValueConverter)
                                flatChanges.AppendFormat("{0}=***", displayName);
                            else
                                flatChanges.AppendFormat("{0}={1}", displayName, converter.Converter.ConvertToStorageType(newVal));
                        }
                        else
                            flatChanges.AppendFormat("{0}", displayName);
                    }
                    else
                        flatChanges.AppendFormat(System.Globalization.CultureInfo.InvariantCulture, "{0}={1}", displayName, newVal);
                }
            }

            return flatChanges.ToString();
        }

        internal void StartSharedRepositoriesPollingSyncronization()
        {
            if (isSharedConnectionRepository &&
                !String.IsNullOrEmpty(connectionStringBackup) &&
                String.Compare(connectionString, connectionStringBackup, true) != 0)
            {
                var intervalInMinutes = GetSharedRepositoriesSynchronizationIntervalInMinutes();

                if (intervalInMinutes < 1)
                    intervalInMinutes = 1;

                var intervalInSeconds = intervalInMinutes * 60;
                var isUpdateRunning = false;

                sharedRepositoriesSynchronizationTimerDisposable =
                    System.Reactive.Linq.Observable.Interval(TimeSpan.FromSeconds(intervalInSeconds), Scheduler)
                    .Subscribe(x =>
                    {
                        if (isUpdateRunning || bObjectDisposed)
                        {
                            return;
                        }

                        isUpdateRunning = true;

                        try
                        {      
                            UpdateSharedRepository();
                        }
                        finally
                        {
                            isUpdateRunning = false;
                        }
                    });
            }   
        }

        private string NormalizeServerName(string serverName)
        {
            string[] serverNameArray = Array.Empty<string>();
            serverName = serverName.Replace(".\\", "localhost\\");

            if (serverName.Contains('\\'))
            {
                serverNameArray = serverName.Split(new char[] {'\\'}, 2);
                serverName = serverNameArray[0];    
            }
           
            serverName = serverName.Replace("(local)", "localhost");
            serverName = LanExtensions.NormalizeHostname(serverName);

            if (serverNameArray.Length > 1 && serverNameArray[1].Trim() != string.Empty)
            {
                serverName = (serverName + "\\" + serverNameArray[1]).Trim();
            }

            return serverName;
        }

        private List<String> PrepareQueriesForSharedRepositoryTriggersRemoval(string connectionString = null)
        {
            if (!isSharedConnectionRepository)
                return null;

            var sqlQueries = new List<String>();
            var splitString = new String[] { "--newline--" };

            try
            {
                if (string.IsNullOrEmpty(connectionString))
                {
                    connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["LocalSqlServer"].ConnectionString;
                }
            }
            catch
            { }

            if (connectionString != null)
            {
                String serverName = null;
                var helper = new DevExpress.Xpo.DB.Helpers.ConnectionStringParser(connectionString);
                if (helper.PartExists("server"))
                    serverName = helper.GetPartByName("server");
                else if (helper.PartExists("data source"))
                    serverName = helper.GetPartByName("data source");

                if (serverName != null)
                {
                    serverName = NormalizeServerName(serverName);
                    var sqlQuery = String.Format(Properties.Settings.Default.tr_removemembership_drop, serverName);
                    sqlQueries.AddRange(sqlQuery.Split(splitString, StringSplitOptions.None));
                }
            }

            return sqlQueries;
        }

        private void RemoveTriggersRelativeToSelectedDatabase(string connectionString, List<String> sqlQueries)
        {
            if (sqlQueries != null && sqlQueries.Count > 0)
            {
                if (!String.IsNullOrEmpty(connectionString) && XpoHelper.IsMSSQlDataProvider(connectionString))
                    ExecuteNonQuery(connectionString, sqlQueries.ToArray());
            }
        }

        private void CheckIfBackupDatabaseIsConnectedAndManageTriggers()
        {
            if (isSharedConnectionRepository &&
                String.IsNullOrEmpty(connectionStringBackup) == false &&
                String.Compare(connectionString, connectionStringBackup, true) != 0)
            {
                isBackupDatabaseConnected = CheckIfDatabaseIsConnected(connectionStringBackup);

                if (isBackupDatabaseConnected == false)
                {
                    var sqlQueriesRelativeToMainDb = PrepareQueriesForSharedRepositoryTriggersRemoval(connectionString);
                    var sqlQueriesRelativeToBackupDb = PrepareQueriesForSharedRepositoryTriggersRemoval(connectionStringBackup);
                    RemoveTriggersRelativeToSelectedDatabase(connectionString, sqlQueriesRelativeToMainDb);
                    RemoveTriggersRelativeToSelectedDatabase(connectionString, sqlQueriesRelativeToBackupDb);
                }
            }
        }

        private bool CheckIfDatabaseIsConnected(string connectionString)
        {
            try
            {
                var dataLayer = XpoDefault.GetDataLayer(connectionString, AutoCreateOption.None);

                if (dataLayer != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
#endif
        #endregion

#if !NET_STANDARD
        #region WinClipboard
        internal void CopyInMemoryDataToWinClipboard()
        {
            Clipboard.Clear();
            using (var xml = new StringWriter())
            {
                using (var xmlTextWriter = new XmlTextWriter(xml) { Formatting = System.Xml.Formatting.Indented })
                {
                    InMemoryClipboard.WriteXml(xmlTextWriter);
                    xmlTextWriter.Flush();
                    xmlTextWriter.Close();
                }

                Clipboard.SetText(xml.ToString());
            }
        }

        string LastClipboardUnicodeText = String.Empty;
        internal void CopyWinClipboardToInMemoryData(bool force = false)
        {
            try
            {
                if (force || Clipboard.ContainsText(TextDataFormat.UnicodeText) &&
                    Clipboard.GetText(TextDataFormat.UnicodeText) != LastClipboardUnicodeText)
                {
                    CleanClipbaord();
                    LastClipboardUnicodeText = Clipboard.GetText(TextDataFormat.UnicodeText);
                    using (var xml = new StringReader(Clipboard.GetText(TextDataFormat.UnicodeText)))
                    {
                        using (var xmlTextReader = new XmlTextReader(xml))
                        {
                            var tempds = new InMemoryDataStore(DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema, true);
                            //using (var tempdl = new SimpleDataLayer(tempds))
                            {
                                bool bValid = true;
                                try
                                {
                                    tempds.ReadXml(xmlTextReader);
                                }
                                catch
                                {
                                    bValid = false;
                                }

                                if (bValid)
                                {
                                    InMemoryClipboard.ReadFromInMemoryDataStore(tempds);
                                }
                            }
                        }
                    }
                }
            }
            catch
            {

            }
        }

        internal void CleanClipbaord()
        {
            if (uowClipboard == null)
                return;

            uowClipboard.ClearDatabase();
        }
        #endregion

        #region Clipboard
        internal XPObject CloneToClipboard(XPObject obj, bool checkattributes)
        {
            XpoHelpers.CloneIXPSimpleObjectHelper cloneHelper = new XpoHelpers.CloneIXPSimpleObjectHelper(uow, uowClipboard, checkattributes, true, true);
            return cloneHelper.Clone(obj, false);
        }

        internal XPObject CloneFromClipboard(XPObject obj, bool checkattributes)
        {
            XpoHelpers.CloneIXPSimpleObjectHelper cloneHelper = new XpoHelpers.CloneIXPSimpleObjectHelper(uowClipboard, uow, checkattributes);
            return cloneHelper.Clone(obj, false);
        }
        internal void CopyListRolesToClipbaord(List<UFUserModel.UFRole> list)
        {
            list.ForEach(role =>
            {
                CloneToClipboard(role, false);
            });
            uowClipboard.CommitChanges();
        }

        internal void CopyListUsersToClipbaord(List<UFUserModel.UFUser> list)
        {
            list.ForEach(user =>
            {
                CloneToClipboard(user, false);
            });
            uowClipboard.CommitChanges();
        }

        internal bool ClipboardContainsRoles()
        {
            if (uowClipboard == null)
                return false;

            try
            {
                return (from c in new XPQuery<UFUserModel.UFRole>(uowClipboard)/*.AsParallel() */select c).ToList().Count > 0;
            }
            catch
            {
                return false;
            }
        }

        internal bool ClipboardContainsUsers()
        {
            if (uowClipboard == null)
                return false;

            try
            {
                return (from c in new XPQuery<UFUserModel.UFUser>(uowClipboard)/*.AsParallel() */select c).ToList().Count > 0;
            }
            catch
            {
                return false;
            }
        }

        internal List<UFUserModel.UFRole> PasteClipboardRoles(/*UFUserModel.UFRole role*/)
        {
            var ret = new List<UFUserModel.UFRole>();
            var mapStartCounter = new Dictionary<string, ulong>();
            var listNodeId = GetRolesNodeIdList();
            var listName = GetUsersNameList();
            var listToCopy = (from c in new XPQuery<UFUserModel.UFRole>(uowClipboard)/*.AsParallel() */orderby c.Oid select c).ToList();
            listToCopy.ForEach(dir =>
            {
                var exist = listNodeId.Contains(dir.NodeId.ToString());
                var newName = NewRoleName(dir.Name);
                var newfolder = CloneFromClipboard(dir, exist) as UFUserModel.UFRole;
                newfolder.Name = newName;
                if (newfolder.UFUsers.Count > 0)
                {
                    foreach (var user in newfolder.UFUsers)
                    {
                        user.Name = NewUserName(user.Name, mapStartCounter, listName);
                    }
                }
                //if (role != null)
                //    role.UFUAFolders.Add(newfolder);
                ret.Add(newfolder);
            });
            return ret;
        }

        internal List<UFUserModel.UFUser> PasteClipboardUsers(UFUserModel.UFRole role, UFUserModel.UFRole parent = null)
        {
            var ret = new List<UFUserModel.UFUser>();
            var mapStartCounter = new Dictionary<string, ulong>();
            var listNodeId = GetUsersNodeIdList();
            var listName = GetUsersNameList();
            var listToCopy = (from c in new XPQuery<UFUserModel.UFUser>(uowClipboard)/*.AsParallel() */where c.UFRoleAss == parent orderby c.Oid select c).ToList();
            listToCopy.ForEach(tag =>
            {
                var exist = listNodeId.Contains(tag.NodeId.ToString());
                var newName = NewUserName(/*role,*/ tag.Name, mapStartCounter, listName);
                var newtag = CloneFromClipboard(tag, exist) as UFUserModel.UFUser;
                newtag.Name = newName;
                newtag.PasswordConfirm = newtag.Password;
                if (role != null)
                    role.UFUsers.Add(newtag);
                ret.Add(newtag);
            });
            return ret;
        }
        #endregion

        #region Undo/Redo

        static String UndoRedoNotInizialized = "The operation cannot be performed because the undo-redo manager has not been initialized for this control.";
        internal void AddUndoAction(UserControl owner, IList<IXPSimpleObject> list, XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions action)
        {
            if (!UndoRedoHelper.ContainsKey(owner))
                throw new KeyNotFoundException(UndoRedoNotInizialized);

            UndoRedoHelper[owner].AddUndoAction(list, action);
        }

        internal void AddRedoAction(UserControl owner, IList<IXPSimpleObject> list, XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions action)
        {
            if (!UndoRedoHelper.ContainsKey(owner))
                throw new KeyNotFoundException(UndoRedoNotInizialized);

            UndoRedoHelper[owner].AddRedoAction(list, action);
        }

        internal void AddUndoAction(UserControl owner, XPObject obj, XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions action)
        {
            if (!UndoRedoHelper.ContainsKey(owner))
                throw new KeyNotFoundException(UndoRedoNotInizialized);

            UndoRedoHelper[owner].AddUndoAction(obj, action);
        }

        internal void AddRedoAction(UserControl owner, XPObject obj, XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions action)
        {
            if (!UndoRedoHelper.ContainsKey(owner))
                throw new KeyNotFoundException(UndoRedoNotInizialized);

            UndoRedoHelper[owner].AddRedoAction(obj, action);
        }

        internal bool UndoContainsSomething(UserControl owner)
        {
            if (bObjectDisposed || !UndoRedoHelper.ContainsKey(owner))
                return false;

            return UndoRedoHelper[owner].CanUndo();
        }

        internal bool RedoContainsSomething(UserControl owner)
        {
            if (bObjectDisposed || !UndoRedoHelper.ContainsKey(owner))
                return false;

            return UndoRedoHelper[owner].CanRedo();
        }

        internal void CleanUndoActions(UserControl owner)
        {
            if (!UndoRedoHelper.ContainsKey(owner))
                throw new KeyNotFoundException(UndoRedoNotInizialized);

            UndoRedoHelper[owner].PurgeUndoActions(true);
        }

        internal void CleanRedoActions(UserControl owner)
        {
            if (!UndoRedoHelper.ContainsKey(owner))
                throw new KeyNotFoundException(UndoRedoNotInizialized);

            UndoRedoHelper[owner].PurgeRedoActions(true);
        }

        internal List<XPObject> UndoAction(UserControl owner, out XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions action)
        {
            if (!UndoRedoHelper.ContainsKey(owner))
                throw new KeyNotFoundException(UndoRedoNotInizialized);

            if (UndoRedoHelper[owner].CanUndo())
                return UndoRedoHelper[owner].Undo(out action);

            action = XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions.None;
            return null;
        }

        internal List<XPObject> RedoAction(UserControl owner, out XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions action)
        {
            if (!UndoRedoHelper.ContainsKey(owner))
                throw new KeyNotFoundException(UndoRedoNotInizialized);

            if (UndoRedoHelper[owner].CanRedo())
                return UndoRedoHelper[owner].Redo(out action);

            action = XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions.None;
            return null;
        }

        internal void AddExistingObject(XPObject obj, object parent)
        {
            if (obj.IsDeleted)
                obj.SetMemberValue("GCRecord", null);

            if (obj is UFUserModel.UFUser)
            {
                var tag = obj as UFUserModel.UFUser;
                if (parent is UFUserModel.UFRole)
                {
                    var root = parent as UFUserModel.UFRole;
                    AddExistingObject(tag, FindRoleByNodeId(root.NodeId));
                }
            }
        }

        void AddExistingObject(UFUserModel.UFUser tag, UFUserModel.UFRole root)
        {
            if (root != null)
                root.UFUsers.Add(tag);
        }

        #endregion
#endif
    }
}
