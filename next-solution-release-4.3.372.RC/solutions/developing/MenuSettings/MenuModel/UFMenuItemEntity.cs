using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using CommandManager;
using MenuSettings.MenuModel;
using OPCUAViewModel;
using UFInterfaces;
using UFInterfaces.Commandable;
using UFInterfaces.Constants;
using Utilities;
using ViewModelLib;
using System.Windows.Threading;
using DocumentManager.ComponentService;
using UFUAEditor.ComponentService;
using System.Reflection;
using System.Windows;
#if !WINDOWS_UWP
using log4net;
#endif
namespace MenuSettings.MenuModel
{
    [DataContract(Name = "UFMenuItemEntity", Namespace = Namespaces.UriProgea)]
    public class UFMenuItemEntity : INotifyPropertyChanged, IDataErrorInfo, ICloneable, ICommandable, IEntityReference
    {
        #region Persistance
        [DataMember]
        Guid _NodeId;
        [DataMember]
        int _OID = -1;
        [DataMember]
        string _Description;
        [DataMember]
        String _Name;
        [DataMember]
        [Category("Access Level")]
        string _AccessRole;
        [DataMember]
        [Category("Access Level")]
        int _WritableAccessLevel;
        [DataMember]
        [Category("Access Level")]
        int _WritableAccessMask;
        [DataMember]
        MenuType _MenuItemType;
        String _EnableTagName;
        [DataMember]
        OPCUAEntityReference _EnableTag;
        String _MarkTagName;
        [DataMember]
        OPCUAEntityReference _MarkTag;
        [DataMember]
        String _Image;
        [DataMember]
        Uri _MenuItemImage;
        [DataMember]
        CommandManagerList listCommands;
        [DataMember]
        List<UFMenuItemEntity> _MenuItems;
        #endregion

        internal OPCUAEntityReference RuntimeEnableTag;
        internal OPCUAEntityReference RuntimeMarkTag;
#if !WINDOWS_UWP
        private static readonly ILog log = LogManager.GetLogger(Properties.Resources.MenuManager);
#endif
        #region Constructors
        public UFMenuItemEntity(UFMenuItemEntity source)
        {
            if (source == null)
                return;

            CopyAll(source);
        }
        
        public UFMenuItemEntity()
        {
            _MenuItemType = MenuType.Item;
        }
        #endregion

        #region Properties
        [ReadOnly(true)]
        public Guid NodeId
        {
            get
            {
                return _NodeId;
            }
            set
            {
                if (_NodeId == value)
                    return;

                _NodeId = value;
                OnPropertyChanged("NodeId");
            }
        }

        [Browsable(false)]
        public int OID
        {
            get
            {
                return _OID;
            }
            set
            {
                if (_OID == value)
                    return;
                _OID = value;
                OnPropertyChanged("OID");
            }
        }

        public string Description
        {
            get
            {
                return _Description;
            }
            set
            {
                if (_Description == value)
                    return;

                _Description = value;
                OnPropertyChanged("Description");
            }
        }
        [Browsable(false)]
        public string Image
        {
            get
            {
                return _Image;
            }
        }

        public Uri MenuItemImage
        {
            get
            {
                return _MenuItemImage;
            }
            set
            {
                if (_MenuItemImage == value)
                    return;

                _MenuItemImage = value;
                OnPropertyChanged("MenuItemImage");
            }
        }

        public string Name
        {
            get
            {
                if (MenuItemType == MenuType.Separator)
                    return Properties.Resources.SeparatorName;
                return _Name;
            }
            set
            {
                if (MenuItemType == MenuType.Separator)
                {
                    _Name = Properties.Resources.SeparatorName;
                    return;
                }

                if (_Name == value)
                    return;

                _Name = value;
                OnPropertyChanged("Name");
            }
        }
        public string AccessRole
        {
            get
            {
                return _AccessRole;
            }
            set
            {
                if (_AccessRole == value)
                    return;

                _AccessRole = value;
                OnPropertyChanged("AccessRole");
            }
        }

        public int WritableAccessLevel
        {
            get
            {
                return _WritableAccessLevel;
            }
            set
            {
                if (_WritableAccessLevel == value)
                    return;

                _WritableAccessLevel = value;
                OnPropertyChanged("WritableAccessLevel");
            }
        }
        public int WritableAccessMask
        {
            get
            {
                return _WritableAccessMask;
            }
            set
            {
                if (_WritableAccessMask == value)
                    return;

                _WritableAccessMask = value;
                OnPropertyChanged("WritableAccessMask");
            }
        }
        public MenuType MenuItemType
        {
            get
            {
                return _MenuItemType;
            }
            set
            {
                if (_MenuItemType == value)
                    return;
                _MenuItemType = value;
                OnPropertyChanged("MenuItemType");
            }
        }
        [Browsable(false)]
        public string EnableTagName
        {
            get { return _EnableTagName; }
            set
            {
                if (_EnableTagName == value)
                    return;

                _EnableTagName = value;
                OnPropertyChanged("EnableTagName");
            }
        }
        [DisplayNameExtension]
        public OPCUAEntityReference EnableTag
        {
            get
            {
                return _EnableTag;
            }
            set
            {
                if (_EnableTag == value)
                    return;

                _EnableTag = value;

                try
                {
                    OPCUAEntityReference item = value;
                    Opc.Ua.NamespaceTable n = new Opc.Ua.NamespaceTable();
                    UInt16 ns = (UInt16)(n.Count + 2 - 1);
                    string oldChars = string.Format("{0}:", ns);
                    EnableTagName = string.Format("{0} ({1})", (item.ReadablePath).Replace(oldChars, ""), item.AppName)/*HumanReadable*/;
                }
                catch (Exception)
                {
                    EnableTagName = string.Empty;
                }

                OnPropertyChanged("EnableTag");
            }
        }

        [Browsable(false)]
        public string MarkTagName
        {
            get { return _MarkTagName; }
            set
            {
                if (_MarkTagName == value)
                    return;

                _MarkTagName = value;
                OnPropertyChanged("MarkTagName");
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Category("Commands")]
        public bool Commands
        {
            get { return false; }
            set
            {
            }
        }
        [DisplayNameExtension]
        public OPCUAEntityReference MarkTag
        {
            get
            {
                return _MarkTag;
            }
            set
            {
                if (_MarkTag == value)
                    return;

                _MarkTag = value;
                try
                {
                    OPCUAEntityReference item = value;
                    Opc.Ua.NamespaceTable n = new Opc.Ua.NamespaceTable();
                    UInt16 ns = (UInt16)(n.Count + 2 - 1);
                    string oldChars = string.Format("{0}:", ns);
                    MarkTagName = string.Format("{0} ({1})", (item.ReadablePath).Replace(oldChars, ""), item.AppName)/*HumanReadable*/;
                }
                catch (Exception)
                {
                    MarkTagName = string.Empty;
                }

                OnPropertyChanged("MarkTag");
            }
        }

        UFMenuEntity _UFMenuAss;
        [Browsable(false)]
        public UFMenuEntity UFMenuAss
        {
            get
            {
                return _UFMenuAss;
            }
            set
            {
                if (_UFMenuAss == value)
                    return;
                _UFMenuAss = value;
                OnPropertyChanged("UFMenuAss");
            }
        }
        [Browsable(false)]
        public List<UFMenuItemEntity> MenuItems
        {
            get
            {
                if (_MenuItems == null)
                    _MenuItems = new List<UFMenuItemEntity>();

                return _MenuItems;
            }
        }

        UFMenuItemEntity _UFItemAss;
        [Browsable(false)]
        public UFMenuItemEntity UFItemAss
        {
            get
            {
                return _UFItemAss;
            }
            set
            {
                if (_UFItemAss == value)
                    return;
                _UFItemAss = value;
                OnPropertyChanged("UFItemAss");
            }
        }
        #endregion

        #region ICloneable Members

        public object Clone()
        {
            return new UFMenuItemEntity(this);
        }

        #endregion ICloneable Members

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                //DispatcherObject dispatcherObject = handler.Target as DispatcherObject;

                var e = new PropertyChangedEventArgs(propertyName);
                //// If the subscriber is a DispatcherObject and different thread
                //if (dispatcherObject != null && dispatcherObject.CheckAccess() == false)
                //{
                //    // Invoke handler in the target dispatcher's thread
                //    dispatcherObject.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, handler, this, e);
                //}
                //else // Execute handler as is
                handler(this, e);
            }
        }

        #endregion

        #region IDataErrorInfo

        [Browsable(false)]
        public string Error
        {
            get
            {
                var context = new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null);
                var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();

                return !System.ComponentModel.DataAnnotations.Validator.TryValidateObject(this, context, results)
                    ? string.Join(Environment.NewLine, results.Select(x => x.ErrorMessage))
                    : null;
            }
        }

        public string this[string propertyName]
        {
            get
            {
                String s = PerformValidation(propertyName);
                if (!String.IsNullOrEmpty(s))
                    return s;
                var context = new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null)
                {
                    MemberName = propertyName
                };

                var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
                var propertyInfo = GetType().GetProperty(propertyName);
                if (propertyInfo != null)
                {
                    var value = propertyInfo.GetValue(this, null);

                    return !System.ComponentModel.DataAnnotations.Validator.TryValidateProperty(value, context, results)
                        ? string.Join(Environment.NewLine, results.Select(x => x.ErrorMessage))
                        : null;
                }

                return null;
            }
        }
        #endregion

        #region runtime commands

        CommandManagerList runtimeCommandList;
        [Browsable(false)]
        internal IEnumerable RuntimeCommandList
        {
            get
            {
                if (runtimeCommandList == null)
                {
                    if (listCommands != null)
                    {
                        var sourceString = listCommands.ToXml();
                        runtimeCommandList = sourceString.FromXml<CommandManagerList>();
                    }
                    else
                        runtimeCommandList = new CommandManagerList();
                }

                return runtimeCommandList;
            }
        }

        internal void FreeRuntimeCommandList()
        {
            if (runtimeCommandList == null)
                return;
            runtimeCommandList.Clear();
            runtimeCommandList = null;

            FreeRuntimeTags();
        }

        #endregion

        #region ICommandable Members

        [Browsable(false)]
        String ICommandable.Name
        {
            get
            {
                return _Name;
            }
        }

        [Browsable(false)]
        public IEnumerable CommandList
        {
            get
            {
                if (listCommands == null)
                    listCommands = new CommandManagerList();
                return listCommands;
            }

            set
            {
                if (listCommands == null)
                    listCommands = new CommandManagerList();
                listCommands.Clear();
                foreach (var v in value)
                    listCommands.Add(v as CommandManager.CommandManager);

                OnPropertyChanged("CommandList");
            }
        }

        [Browsable(false)]
        public bool WebHMISupported
        {
            get
            {
                return true;
            }
        }

        [Browsable(false)]
        public UIElement Control
        {
            get
            {
                return null;
            }
        }
        #endregion

        #region Methods
        [OnDeserialized]
        private void PostInitialize(StreamingContext context)
        {
            if (!String.IsNullOrEmpty(_Image))
            {
                try
                {
                    _MenuItemImage = new Uri(_Image, UriKind.RelativeOrAbsolute);
                    _Image = string.Empty;
                }
                catch (Exception)
                {
                }
            }
        }

        public void GetAllSourceEntityReferencesDetails(IDocument doc, IUFUAEditorManager service, CRMapsHelper cRMapsHeler, bool getScreen, bool getTags)
        {
            var listCommand = CommandList as CommandManagerList;
            foreach (var command in listCommand)
            {
                if (getScreen)
                {
                    var properties = CommandManager.Extensions.CommandManagerExtensions.GetBrowsablePropertiesOfType<Uri>(command);
                    foreach (PropertyInfo prop in properties)
                    {
                        var uri = prop.GetValue(command) as Uri;
                        if (uri != null && uri.GetPathString() != null)
                        {
                            var dictionary = new Dictionary<string, string>();
                            dictionary[string.Format("{0}\\{1}", Properties.Resources.CommandTagHeader, command.Name)] = uri.GetPathString();
                            cRMapsHeler.ScreenLinks.Add(dictionary);
                        }
                    }
                }
                if(getTags)
                {
                    if (!string.IsNullOrEmpty(command.Expression))
                    {
                        var listDynamic = GetExpressionTags(doc, service, command);
                        cRMapsHeler.Tags.AddRange(listDynamic);
                    }
                    var listTagDynamic = GetCommandTags(command);
                    cRMapsHeler.Tags.AddRange(listTagDynamic);
                }
            }
        }

        public List<object> GetExpressionTags(IDocument doc, IUFUAEditorManager service, CommandManager.CommandManager command)
        {
            List<object> list = new List<object>();
            var ufuaEditorService = doc.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
            var appName = ufuaEditorService.GetAplicationName(doc);
            List<string> expressionTags = Utilities.Converters.ExpressionValueConverterHelper.GetListVarInExpression(command.Expression);
            for (int j = 0; j < expressionTags.Count; j++)
            {
                var expression = expressionTags[j];
                OPCUAEntityReference tag = GetReferenceTag(doc, service, expression);

                if (tag == null)
                    tag = new OPCUAEntityReference() { RelativePath = expression, AppName = appName };

                var dictionary = new Dictionary<string, OPCUAEntityReference>();
                dictionary[string.Format("{0}\\{1} - {2} {3}", Properties.Resources.CommandTagHeader, command.Name, Properties.Resources.ItemsExpressionTagHeader, expressionTags.IndexOf(expression))] = tag;
                if (dictionary != null)
                    list.Add(dictionary);
            }
            return list;
        }

        public List<object> GetCommandTags(CommandManager.CommandManager command)
        {
            List<object> list = new List<object>();
            foreach (var tag in command.ListTags)
            {
                var dictionary = new Dictionary<string, OPCUAEntityReference>();
                dictionary[string.Format("{0}\\{1}", Properties.Resources.CommandTagHeader, command.Name)] = tag;
                list.Add(dictionary);
            }
            return list;
        }

        private OPCUAEntityReference GetReferenceTag(IDocument doc, IUFUAEditorManager service, string reference)
        {
            OPCUAEntityReference tag = null;
            var split = reference.Split('-');
            var instance = split[0];
            var name = split[0];
            if (split.Length > 1)
                name = split[1];
            else
                instance = null;
            var xml = service.GetTagEntityReference(doc, name, instance, useCachedUow: true);
            if (!String.IsNullOrEmpty(xml))
                tag = xml.FromXml<OPCUAEntityReference>();
            return tag;
        }

        PropertyObserver<OPCUAEntityReference> observerMark;
        PropertyObserver<MonitoredItemViewModel> observerMarkMonitoredModel;
        PropertyObserver<OPCUAEntityReference> observerEnable;
        PropertyObserver<MonitoredItemViewModel> observerEnableMonitoredModel;
        DispatcherOperation dpEnable;
        DispatcherOperation dpMark;
        bool bPrepared = false;

        public void PrepareExecution(string sessionname, MenuItem mi, IDocument parent)
        {
            if (bPrepared)
                return;
            bPrepared = true;

            if (EnableTag != null && EnableTag.IsValid)
            {
                mi.Dispatcher.BeginInvoke((Action)(() => { mi.IsEnabled = false; }));

                RuntimeEnableTag = new OPCUAEntityReference(EnableTag);
                observerEnable = new PropertyObserver<OPCUAEntityReference>(RuntimeEnableTag);
                observerEnable.RegisterHandler(n => n.MonitoredItemViewModel, n =>
                {
                    observerEnable.UnregisterHandler(p => p.MonitoredItemViewModel);

                    mi.Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                    {
                        try
                        { 
                            if (n.MonitoredItemViewModel.DataValue != null &&
                            n.MonitoredItemViewModel.DataValue.Value != null)
                                mi.IsEnabled = Convert.ToBoolean(n.MonitoredItemViewModel.DataValue.Value);
                        }
                        catch(Exception ex)
                        {
#if !WINDOWS_UWP
                            log.Error(string.Format(Properties.Resources.ExceptionEnableTag, ex.Message), ex);
#endif
                        }
                    });
                        

                    if (observerEnableMonitoredModel != null)
                        observerEnableMonitoredModel.Dispose();
                    observerEnableMonitoredModel =
                    new PropertyObserver<MonitoredItemViewModel>(n.MonitoredItemViewModel);
                    observerEnableMonitoredModel.RegisterHandler(m => m.DataValue, m =>
                    {
                        if (dpEnable == null || dpEnable.Status == DispatcherOperationStatus.Completed ||
                            dpEnable.Status == DispatcherOperationStatus.Aborted)
                        {
                            dpEnable = mi.Dispatcher.BeginInvokeAsynchronouslyInBackground(mi, () =>
                            {
                                try
                                {
                                    if (m.DataValue != null)
                                        mi.IsEnabled = Convert.ToBoolean(m.DataValue.Value);
                                }
                                catch (Exception ex)
                                {
#if !WINDOWS_UWP
                                    log.Error(string.Format(Properties.Resources.ExceptionEnableTag, ex.Message), ex);
#endif
                                }
                            });
                        }
                    });
                });

                RuntimeEnableTag.Resolve(sessionname, parent);
                RuntimeEnableTag.SetInUse(this, true);
            }

            if (MarkTag != null && MarkTag.IsValid)
            {
                
                RuntimeMarkTag = new OPCUAEntityReference(MarkTag);
                mi.IsCheckable = true;
                
                { 
                    observerMark = new PropertyObserver<OPCUAEntityReference>(RuntimeMarkTag);

                    observerMark.RegisterHandler(n => n.MonitoredItemViewModel, n =>
                    {
                        mi.Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                        {
                            try
                            {
                                if (n.MonitoredItemViewModel.DataValue != null &&
                                    n.MonitoredItemViewModel.DataValue.Value != null)
                                        mi.IsChecked = Convert.ToBoolean(n.MonitoredItemViewModel.DataValue.Value);
                            }
                            catch (Exception ex)
                            {
#if !WINDOWS_UWP
                                log.Error(string.Format(Properties.Resources.ExceptionMarkTag, ex.Message), ex);
#endif
                            }
                        });
                        observerMark.UnregisterHandler(p => p.MonitoredItemViewModel);
                        observerMarkMonitoredModel =
                            new PropertyObserver<MonitoredItemViewModel>(n.MonitoredItemViewModel);

                        observerMarkMonitoredModel.RegisterHandler(m => m.DataValue, m =>
                        {
                            if (dpMark == null || dpMark.Status == DispatcherOperationStatus.Completed ||
                                dpMark.Status == DispatcherOperationStatus.Aborted)
                            {
                                dpMark = mi.Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                                {
                                    try
                                    {
                                        if (m.DataValue != null)
                                            mi.IsChecked = Convert.ToBoolean(m.DataValue.Value);
                                    }
                                    catch (Exception ex)
                                    {
#if !WINDOWS_UWP
                                        log.Error(string.Format(Properties.Resources.ExceptionMarkTag, ex.Message), ex);
#endif
                                    }
                                });
                            }
                        });

                    });

                    RuntimeMarkTag.Resolve(sessionname, parent);
                    RuntimeMarkTag.SetInUse(this, true);
                }
            }
        }

        public bool IsPopup()
        {
            return MenuItems.Count > 0;
        }

        public void CopyAll(UFMenuItemEntity source, bool copyname = true)
        {
            _Description = source._Description;
            //_KeyName = source._KeyName;
            //_Enabled = source._Enabled;
            if (copyname)
            {
                _Name = source._Name;
                _OID = source.OID;
                _NodeId = source.NodeId;
            }
            _AccessRole = source._AccessRole;
            _WritableAccessLevel = source._WritableAccessLevel;
            _WritableAccessMask = source._WritableAccessMask;
            _MenuItemType = source.MenuItemType;
            //_EnableTagName = source.EnableTagName;
            //_MarkTagName = source.MarkTagName;
            _MenuItemImage = source.MenuItemImage;

            //dynamics
            if (source._EnableTag != null)
            {
                var sourceString = source._EnableTag.ToXml();
                _EnableTag = sourceString.FromXml<OPCUAEntityReference>();
            }
            else
                _EnableTag = null;

            if (source._MarkTag != null)
            {
                var sourceString = source._MarkTag.ToXml();
                _MarkTag = sourceString.FromXml<OPCUAEntityReference>();
            }
            else
                _MarkTag = null;

            if (source.listCommands != null)
            {
                var sourceString = source.listCommands.ToXml();
                listCommands = sourceString.FromXml<CommandManagerList>();
            }
            else
                listCommands = null;
            if (source.MenuItems != null)
            {
                var sourcestring = source.MenuItems.ToXml();
                _MenuItems = sourcestring.FromXml<List<UFMenuItemEntity>>();
            }
            else
                _MenuItems = null;

            UFMenuAss = source.UFMenuAss;
            UFItemAss = source.UFItemAss;
        }

        protected String PerformValidation(String propertyName)
        {
            if (propertyName == "Name")
            {
                if (String.IsNullOrWhiteSpace(Name))
                {
                    return Properties.Resources.ObjectNameMissing;
                }
                else if (UFMenuAss != null)
                {
                    if ((from c in UFMenuAss.MenuItems.AsParallel()
                         where c != this && c.Name == Name && c.NodeId != NodeId
                         select c).ToList().Count > 0)
                    {
                        return Properties.Resources.NameAlreadyExists;
                    }
                }
                else if(UFItemAss != null)
                {
                    if ((from c in UFItemAss.MenuItems.AsParallel()
                         where c != this && c.Name == Name && c.NodeId != NodeId
                         select c).ToList().Count > 0)
                    {
                        return Properties.Resources.NameAlreadyExists;
                    }
                }

            }
            else if(propertyName == "MenuItemImage")
            {
                if (MenuItemImage != null && !MenuItemImage.IsValidFile())
                {
                    return Properties.Resources.InvalidSource;
                }
            }

            return null;
        }

        internal void FreeRuntimeTags()
        {
            if(RuntimeEnableTag != null)
            {
                RuntimeEnableTag.SetInUse(this, false);
                if (observerEnable != null)
                    observerEnable.Dispose();
                observerEnable = null;
                if (observerEnableMonitoredModel != null)
                    observerEnableMonitoredModel.Dispose();
                observerEnableMonitoredModel = null;
            }
            if(RuntimeMarkTag != null)
            {
                RuntimeMarkTag.SetInUse(this, false);
                if (observerMark != null)
                    observerMark.Dispose();
                observerMark = null;
                if (observerMarkMonitoredModel != null)
                    observerMarkMonitoredModel.Dispose();
                observerMarkMonitoredModel = null;
            }

            if (dpEnable != null)
            {
                dpEnable.Abort();
                dpEnable = null;
            }
            if(dpMark != null)
            {
                dpMark.Abort();
                dpMark = null;
            }
        }
        #endregion

        #region IEntityReference Members
        [Browsable(false)]
        public System.Windows.Media.ImageSource CollapsedImageSource
        {
            get{ return null; }
        }
        [Browsable(false)]
        public System.Windows.Media.ImageSource ExpandedImageSource
        {
            get { return null; }
        }
        [Browsable(false)]
        public System.Windows.Controls.ContextMenu contextMenu
        {
            get { return null; }
        }
        [Browsable(false)]
        public object Tooltip
        {
            get { return null; }
        }
        [Browsable(false)]
        public object ContainedObject
        {
            get { return null; }
        }
        [Browsable(false)]
        public object EntityParent
        {
            get { return null; }
        }
        [Browsable(false)]
        public string TypeDefinitionString
        {
            get { return null; }
        }

        #endregion
    }
}
