using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandManager;
using DevExpress.Xpo;
#if !NET_STANDARD
using UFInterfaces.Commandable;
using Utilities.WPF;
#endif
using UFInterfaces.PropertyControl;
using Utilities;
using OPCUAViewModel;
using DocumentManager.ComponentService;
using System.Reflection;
using System.Windows;

namespace UFEventModel
{
    public class UFEventObject : XPObject, IDataErrorInfo
#if !NET_STANDARD
        , ICommandable, INotifyPropertyVisibilityChanged, XpoHelpers.UndoRedoIXPSimpleObjectHelper.IUniqueIdentifier
#endif
    {
        #region Ctor
        public UFEventObject(Session session)
            : base(session)
        { }
        #endregion

        #region Properties Default Values
        // List of constant default values for each property where you want handle a default value.
        const bool defaultEnable = true;
        const EventType defaultType = UFEventModel.EventType.Tag;
        const ConditionType defaultConditionType = UFEventModel.ConditionType.Equals;
        const double defaultActivationValue = 1;

        /// <summary>
        /// Adds inside this method the nullable property where you want handle a default value.
        /// </summary>
        private void EnsureDefaultValues()
        {
            // Examples of how to handle a default value
            //if (!PropertyName.HasValue)
            //    PropertyName = defaultPropertyName;
            //if (TimeSpanPropertyName == TimeSpan.Zero)
            //    TimeSpanPropertyName = TimeSpan.FromMinutes(1);
            //if (DateTimePropertyName == DateTime.MinValue)
            //    DateTimePropertyName = DateTime.UtcNow;

            if (!Enable.HasValue)
                Enable = defaultEnable;
            if (!Type.HasValue)
                Type = defaultType;
            if (!ConditionType.HasValue)
                ConditionType = defaultConditionType;
            if (!ActivationValue.HasValue)
                ActivationValue = defaultActivationValue;
            if (EventCommandList == null)
                EventCommandList = String.Empty;
        }
        #endregion

        #region Properties
        private string _Name;
        //[Indexed(Unique = false)]
        [MergablePropertyAttribute(false)]
        [Size(SizeAttribute.Unlimited)]
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                SetPropertyValue("Name", ref _Name, value);
            }
        }

        private Guid _NodeId;
        [Custom("Generate", "Guid")]
        [ReadOnly(true)]
        public Guid NodeId
        {
            get
            {
                return _NodeId;
            }
            set
            {
                SetPropertyValue("NodeId", ref _NodeId, value);
            }
        }

        private bool? _Enable;
        public bool? Enable
        {
            get
            {
                return _Enable;
            }
            set
            {
                SetPropertyValue("Enable", ref _Enable, value);
            }
        }
        private EventType? _Type;
        public EventType? Type
        {
            get 
            { 
                return _Type; 
            }
            set 
            { 
                if (SetPropertyValue("Type", ref _Type, value))
                {
#if !NET_STANDARD
                    OnPropertyVisiblityChanged("Type");
#endif
                }
            }
        }
        private string _Tag;
        [Size(SizeAttribute.Unlimited)]
#if !NETSTANDARD
        [DisplayNameExtension]
#endif
        public string Tag
        {
            get { return _Tag; }
            set
            {
                try
                {
                    if (value == null)
                        TagName = string.Empty;
                    else
                    {
                        OPCUAEntityReference item = value.FromXml<OPCUAEntityReference>();
                        TagName = item.HumanReadable;
                    }
                }
                catch (Exception)
                {
                    TagName = string.Empty;
                }

                SetPropertyValue("Tag", ref _Tag, value);
            }
        }
        private string _TagName;
        [Browsable(false)]
        [Size(SizeAttribute.Unlimited)]
        public string TagName
        {
            get { return _TagName; }
            set
            {
                SetPropertyValue("TagName", ref _TagName, value);
                this.RaisePropertyChangedEvent("ReadableTagName");
            }
        }

        private ConditionType? _ConditionType;
        public ConditionType? ConditionType
        {
            get
            {
                return _ConditionType;
            }
            set
            {
                if(SetPropertyValue("ConditionType", ref _ConditionType, value))
                {
#if !NET_STANDARD
                    OnPropertyVisiblityChanged("ConditionType");
#endif
                }
            }
        }

        private double? _ActivationValue;
        public double?  ActivationValue
        {
            get
            {
                return _ActivationValue;
            }
            set
            {
                SetPropertyValue("ActivationValue", ref _ActivationValue, value);
            }
        }
        private string _ActivationStringValue;
        [Size(SizeAttribute.Unlimited)]
        public string ActivationStringValue
        {
            get
            {
                return _ActivationStringValue;
            }
            set
            {
                SetPropertyValue("ActivationStringValue", ref _ActivationStringValue, value);
            }
        }
        private string _EnableTag;
        [Size(SizeAttribute.Unlimited)]
#if !NETSTANDARD
        [DisplayNameExtension]
#endif
        public string EnableTag
        {
            get { return _EnableTag; }
            set
            {
                try
                {
                    if (value == null)
                        EnableTagName = string.Empty;
                    else
                    {
                        OPCUAEntityReference item = value.FromXml<OPCUAEntityReference>();
                        EnableTagName = item.HumanReadable;
                    }
                }
                catch (Exception)
                {
                    EnableTagName = string.Empty;
                }

                SetPropertyValue("EnableTag", ref _EnableTag, value);
            }
        }
        private string _EnableTagName;
        [Browsable(false)]
        [Size(SizeAttribute.Unlimited)]
        public string EnableTagName
        {
            get { return _EnableTagName; }
            set
            {
                SetPropertyValue("EnableTagName", ref _EnableTagName, value);
                this.RaisePropertyChangedEvent("ReadableEnableTagName");
            }
        }
        private string _ValueTag;
        [Size(SizeAttribute.Unlimited)]
#if !NETSTANDARD
        [DisplayNameExtension]
#endif
        public string ValueTag
        {
            get { return _ValueTag; }
            set
            {
                try
                {
                    if(value == null)
                        ValueTagName = string.Empty;
                    else
                    {
                        OPCUAEntityReference item = value.FromXml<OPCUAEntityReference>();
                        ValueTagName = item.HumanReadable;
                    }
                }
                catch (Exception)
                {
                    ValueTagName = string.Empty;
                }
                SetPropertyValue("ValueTag", ref _ValueTag, value);
            }
        }
        private string _ValueTagName;
        [Browsable(false)]
        [Size(SizeAttribute.Unlimited)]
        public string ValueTagName
        {
            get { return _ValueTagName; }
            set
            {
                SetPropertyValue("ValueTagName", ref _ValueTagName, value);
            }
        }
        private ScheduleType _SchedType;
        public ScheduleType SchedType
        {
            get
            {
                return _SchedType;
            }
            set
            {
                SetPropertyValue("SchedType", ref _SchedType, value);
                this.RaisePropertyChangedEvent("Time");
            }
        }
        private DateTime _Time;
        public DateTime Time
        {
            get
            {
                return _Time;
            }
            set
            {
                SetPropertyValue("Time", ref _Time, value);
            }
        }
        private DateTime _Date;
        public DateTime Date
        {
            get
            {
                return _Date;
            }
            set
            {
                SetPropertyValue("Date", ref _Date, value);
            }
        }


        private string _EventCommandList;
        [Size(SizeAttribute.Unlimited)]
        [Browsable(false)]
        public string EventCommandList
        {
            get { return _EventCommandList; }
            set
            {
                SetPropertyValue("EventCommandList", ref _EventCommandList, value);
            }
        }

        [NonPersistent]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Category("Commands")]
        public bool Commands
        {
            get { return false; }
            set
            {
            }
        }

        private UFEventFolder _UFEventFolder;
        [Association("UFEventFolder-UFEventObjects")]
        [Browsable(false)]
        public UFEventFolder UFEventFolder
        {
            get
            {
                return _UFEventFolder;
            }
            set
            {
                SetPropertyValue("UFEventFolder", ref _UFEventFolder, value);
            }
        }


        IEnumerable listCommands;



        private String _Expression;
        public String Expression
        {
            get { return _Expression; }
            set
            {
                SetPropertyValue("Expression", ref _Expression, value);
            }
        }
        private String _EnableExpression;
        public String EnableExpression
        {
            get { return _EnableExpression; }
            set
            {
                SetPropertyValue("EnableExpression", ref _EnableExpression, value);
            }
        }

        private String _ValueExpression;
        public String ValueExpression
        {
            get { return _ValueExpression; }
            set
            {
                SetPropertyValue("ValueExpression", ref _ValueExpression, value);
            }
        }
        #endregion

        #region Override Methods

        public override void AfterConstruction()
        {
            base.AfterConstruction();

            EnsureDefaultValues();
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();

            EnsureDefaultValues();
        }

        #endregion
        #region IUniqueIdentifier
#if !NET_STANDARD
        [Browsable(false)]
        [NonPersistent]
        public String UniqueIdentifier
        {
            get
            {
                return NodeId.ToString();
            }
        }
#endif
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

        #region INotifyPropertyVisibilityChanged Members
#if !NET_STANDARD
        /// <summary>
        /// Gets the visibility state for the property with the given name.
        /// </summary>
        /// <param name="propertyName">The property name that you want konw the current visibility state.</param>
        /// <returns></returns>
        bool INotifyPropertyVisibilityChanged.this[string propertyName]
        {
            get
            {
                if (propertyName == "SchedType" || propertyName == "Time" || propertyName == "Date")
                {
                    return Type == EventType.Schedule;
                }
                else if (propertyName == "TagName" || propertyName == "ConditionType" || propertyName == "Tag"
                    || propertyName == "Expression")
                {
                    return Type == EventType.Tag;
                }
                else if(propertyName == "ActivationValue")
                {
                    return Type == EventType.Tag && ConditionType != UFEventModel.ConditionType.OnChange;
                }
                else if(propertyName == "ActivationStringValue" || propertyName == "ValueTag" || propertyName == "ValueExpression")
                {
                    if ((Type == EventType.Tag && ConditionType == UFEventModel.ConditionType.OnChange) || Type == EventType.Schedule)
                        return false;
                }

                    return true;
            }
        }

        /// <summary>
        /// Raised when a property visibility state on this object has a new value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyVisiblityChanged;

        /// <summary>
        /// Raises this object's PropertyVisiblityChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has changed his value and has triggered the change of visibility.</param>
        protected void OnPropertyVisiblityChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyVisiblityChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }
#endif
        #endregion
        protected String PerformValidation(String propertyName)
        {
            if (propertyName == "Name")
            {
                if (!UFUAModel.Helpers.NameValidator.IsValidName(Name))
                {
                    return Properties.Resources.EventNameInvalid;
                }
                else if (UFEventFolder != null && (from c in UFEventFolder.UFEventObjects
                                                where c != this && c.Name == Name
                                                select c).ToList().Count > 0)
                {
                    return Properties.Resources.EventNameAlreadyExists;
                }
                 else if ((from tag in new XPQuery<UFEventObject>(Session, true)/*.AsParallel()*/
                          where tag.UFEventFolder == null && tag.Name == Name
                          select tag).ToList().Count > 1)
                    return Properties.Resources.EventNameAlreadyExists;
            }
            return null;
        }

#if !NET_STANDARD
        public void GetAllSourceEntityReferencesDetails(CRMapsHelper cRMapsHeler, bool bForceOp = false)
        {
            bool getTags = cRMapsHeler.CrossReferenceTypes.Contains(CrossReferenceType.Tags);
            bool getScreens = cRMapsHeler.CrossReferenceTypes.Contains(CrossReferenceType.Resources);
            var commands = CommandList as CommandManagerList;
            foreach (var command in commands)
            {
                if (getScreens)
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
                if(getTags || bForceOp)
                {
                    var listTagDynamic = GetCommandTags(command);
                    cRMapsHeler.Tags.AddRange(listTagDynamic);
                }
            }
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
#endif
        #region runtime commands

        IEnumerable runtimeCommandList;
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

            var commands = runtimeCommandList as CommandManagerList;
            if (commands != null)
                commands.Clear();

            runtimeCommandList = null;
        }

        #endregion

        #region ICommandable Members
#if !NET_STANDARD
        [Browsable(false)]
        String ICommandable.Name
        {
            get
            {
                return _Name;
            }
        }
#endif

        [Browsable(false)]
        public IEnumerable CommandList
        {
            get
            {
                var commands = listCommands as CommandManagerList;
                if (commands == null)
                {
                    commands = new CommandManagerList();
                    if (EventCommandList != null && EventCommandList.Length > 0)
                        commands = EventCommandList.FromXml<CommandManagerList>();
                    listCommands = commands;
                }
                
                return listCommands;
            }

            set
            {
                var commands = listCommands as CommandManagerList;
                if (commands == null)
                    commands = new CommandManagerList();

                commands.Clear();
                foreach (var v in value)
                    commands.Add(v as CommandManager.CommandManager);

                listCommands = commands;
                EventCommandList = listCommands.ToXml();
            }
        }

#if !NET_STANDARD
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
#endif
        #endregion
    }
}
