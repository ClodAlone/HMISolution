using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ComponentModel;
using OPCUAViewModel;
using CommandManager;
using UFInterfaces.Constants;
using UFInterfaces.Commandable;
using System.Collections;
using Utilities;
using System.Windows.Input;
using ViewModelLib;
using UFInterfaces;
using DocumentManager.ComponentService;
using UFUAEditor.ComponentService;
using System.Reflection;
using System.Windows;

namespace UFShortcutSettings.ShortcutModel
{
    [DataContract(Name = "UFKeyCommandEntity", Namespace = Namespaces.UriProgea)]
    public class UFKeyCommandEntity : INotifyPropertyChanged, IDataErrorInfo, ICloneable, ICommandable, IEntityReference
    {
        #region Persistance
        [DataMember]
        Guid _NodeId;
        [DataMember]
        int _OID = -1;
        [DataMember]
        string _Description;
        [DataMember]
        String _KeyName;
        [DataMember]
        String _SpeechCommand;
        [DataMember]
        String _ShortcutKey;
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
        bool _Enabled = true;
        [DataMember]
        bool _Down = true;
        String _EnableTagName;
        [DataMember]
        OPCUAEntityReference _EnableTag;
        [DataMember]
        CommandManagerList listCommands;
        #endregion

        internal ModifierKeys modifiers = ModifierKeys.None;
        internal Key keycode = Key.None;
        internal bool IsValid = false;

        #region Constructors
        public UFKeyCommandEntity(UFKeyCommandEntity source)
        {
            if (source == null)
                return;

            CopyAll(source);
        }
        
        public UFKeyCommandEntity()
        { }
        #endregion

        #region Properties
        [Browsable(false)]
        public String Name { get { return KeyName; } }

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

        [MergablePropertyAttribute(false)]
        public string KeyName
        {
            get
            {
                return _KeyName;
            }
            set
            {
                if (_KeyName == value)
                    return;

                _KeyName = value;
                OnPropertyChanged("KeyName");
                OnPropertyChanged("Name");
            }
        }

        [MergablePropertyAttribute(false)]
        public string SpeechCommand
        {
            get
            {
                return _SpeechCommand;
            }
            set
            {
                if (_SpeechCommand == value)
                    return;

                _SpeechCommand = value;
                OnPropertyChanged("SpeechCommand");
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

        public string ShortcutKey
        {
            get
            {
                return _ShortcutKey;
            }
            set
            {
                if (_ShortcutKey == value)
                    return;

                _ShortcutKey = value;
                OnPropertyChanged("ShortcutKey");
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

        public bool Enabled
        {
            get
            {
                return _Enabled;
            }
            set
            {
                if (_Enabled == value)
                    return;

                _Enabled = value;
                OnPropertyChanged("Enabled");
            }
        }

        public bool Down
        {
            get
            {
                return _Down;
            }
            set
            {
                if (_Down == value)
                    return;

                _Down = value;
                OnPropertyChanged("Down");
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
        public string EnableTagName
        {
            get {
                return _EnableTagName; 
            }
            set
            {
                if (_EnableTagName == value)
                    return;

                _EnableTagName = value;
                OnPropertyChanged("EnableTagName");
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


        UFShortcutEntity _UFShortcutAss;
        [Browsable(false)]
        public UFShortcutEntity UFShortcutAss
        {
            get
            {
                return _UFShortcutAss;
            }
            set
            {
                if (_UFShortcutAss == value)
                    return;
                _UFShortcutAss = value;
                OnPropertyChanged("UFShortcutAss");
            }
        }
        #endregion

        #region ICloneable Members

        public object Clone()
        {
            return new UFKeyCommandEntity(this);
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
        }

        #endregion

        #region ICommandable Members

        [Browsable(false)]
        String ICommandable.Name
        {
            get
            {
                return _KeyName;
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
        public void GetAllSourceEntityReferencesDetails(IDocument doc, IUFUAEditorManager service, CRMapsHelper cRMapsHeler)
        {
            bool getScreen = cRMapsHeler.CrossReferenceTypes.Contains(CrossReferenceType.Resources);
            bool getTags = cRMapsHeler.CrossReferenceTypes.Contains(CrossReferenceType.Tags);
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
            List<Dictionary<string, OPCUAEntityReference>> dictionaries = new List<Dictionary<string, OPCUAEntityReference>>();
            for (int j = 0; j < expressionTags.Count; j++)
            {
                var expression = expressionTags[j];
                OPCUAEntityReference tag = GetReferenceTag(doc, service, expression);

                if (tag == null)
                    tag = new OPCUAEntityReference() { RelativePath = expression, AppName = appName };

                var dictionary = new Dictionary<string, OPCUAEntityReference>();
                dictionary[string.Format("{0}\\{1} - {2} {3}", Properties.Resources.CommandTagHeader, command.Name, Properties.Resources.ItemsExpressionTagHeader, expressionTags.IndexOf(expression))] = tag;
                if (dictionary != null)
                    lock (dictionaries)
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
        public void CopyAll(UFKeyCommandEntity source, bool copyname = true)
        {
            if(copyname)
            _KeyName = source._KeyName;
            _Description = source._Description;
            _ShortcutKey = source._ShortcutKey;
            _AccessRole = source._AccessRole;
            _WritableAccessLevel = source._WritableAccessLevel;
            _WritableAccessMask = source._WritableAccessMask;
            _SpeechCommand = source._SpeechCommand;
            _Enabled = source._Enabled;

            //dynamics
            if (source._EnableTag != null)
            {
                var sourceString = source._EnableTag.ToXml();
                _EnableTag = sourceString.FromXml<OPCUAEntityReference>();
            }
            else
                _EnableTag = null;

            if (source.listCommands != null)
            {
                var sourceString = source.listCommands.ToXml();
                listCommands = sourceString.FromXml<CommandManagerList>();
            }
            else
                listCommands = null;
        }

        protected String PerformValidation(String propertyName)
        {
            if (propertyName == "KeyName")
            {
                if (String.IsNullOrWhiteSpace(KeyName))
                {
                    return Properties.Resources.ObjectNameMissing;
                }
                else if (UFShortcutAss != null)
                {
                    if ((from c in UFShortcutAss.KeyCommands.AsParallel()
                         where c != this && c.KeyName == KeyName && c.NodeId != NodeId
                         select c).ToList().Count > 0)
                    {
                        return Properties.Resources.CommandNameAlreadyExists;
                    }
                }
            }

            
            return null;
        }

        internal OPCUAEntityReference RuntimeEnableTag;
        
        public void PrepareExecution(string sessionname, IDocument parent)
        {
            if (EnableTag != null && EnableTag.IsValid)
            {
                RuntimeEnableTag = new OPCUAEntityReference(EnableTag);

                RuntimeEnableTag.Resolve(sessionname, parent);
                RuntimeEnableTag.SetInUse(this, true);
            }

        }

        public void TerminateExecution()
        {
            if (RuntimeEnableTag != null && RuntimeEnableTag.IsValid)
                RuntimeEnableTag.SetInUse(this, false);
        }
        #endregion

        #region IEntityReference Members
        [Browsable(false)]
        public System.Windows.Media.ImageSource CollapsedImageSource
        {
            get { return null; }
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
