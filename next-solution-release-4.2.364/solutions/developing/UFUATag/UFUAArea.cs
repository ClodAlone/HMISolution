using System;
using System.Linq;
using DevExpress.Xpo;
using System.ComponentModel;
using System.Collections.Generic;
#if !NET_STANDARD
using Utilities.Xpo.UndoRedo;
#endif

namespace UFUAModel
{
    [DeferredDeletion(false)]
    public class UFUAArea : XPObject, IDataErrorInfo
#if !NET_STANDARD
        , IUndoRedoXpo
#endif
    {
        #region Constructors
        public UFUAArea(Session session)
            : base(session)
        { }
        #endregion

        #region Properties Default Values
        // List of constant default values for each property where you want handle a default value.
        //const int defaultPropertyName = -1;

        /// <summary>
        /// Adds inside this method the nullable property where you want handle a default value.
        /// </summary>
        private void EnsureDefaultValues()
        {
            // Examples of how to handle a default value
            //if (!_PropertyName.HasValue)
            //    _PropertyName = defaultPropertyName;
            //if (_TimeSpanPropertyName == TimeSpan.Zero)
            //    _TimeSpanPropertyName = TimeSpan.FromMinutes(1);
            //if (_DateTimePropertyName == DateTime.MinValue)
            //    _DateTimePropertyName = DateTime.UtcNow;
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

        #region Alarm Counter Tags
        private TagEntityReference _AlarmsNumEnabledTag;
        [ValueConverter(typeof(ConvertTagEntityReference))]
        [Size(SizeAttribute.Unlimited)]
        public TagEntityReference AlarmsNumEnabledTag
        {
            get
            {
                return _AlarmsNumEnabledTag;
            }
            set
            {
                SetPropertyValue("AlarmsNumEnabledTag", ref _AlarmsNumEnabledTag, value);
            }
        }

        private TagEntityReference _AlarmsNumActiveOnTag;
        [ValueConverter(typeof(ConvertTagEntityReference))]
        [Size(SizeAttribute.Unlimited)]
        public TagEntityReference AlarmsNumActiveOnTag
        {
            get
            {
                return _AlarmsNumActiveOnTag;
            }
            set
            {
                SetPropertyValue("AlarmsNumActiveOnTag", ref _AlarmsNumActiveOnTag, value);
            }
        }

        private TagEntityReference _AlarmsNumActiveOffTag;
        [ValueConverter(typeof(ConvertTagEntityReference))]
        [Size(SizeAttribute.Unlimited)]
        public TagEntityReference AlarmsNumActiveOffTag
        {
            get
            {
                return _AlarmsNumActiveOffTag;
            }
            set
            {
                SetPropertyValue("AlarmsNumActiveOffTag", ref _AlarmsNumActiveOffTag, value);
            }
        }

        private TagEntityReference _AlarmsNumActiveOnOffTag;
        [ValueConverter(typeof(ConvertTagEntityReference))]
        [Size(SizeAttribute.Unlimited)]
        public TagEntityReference AlarmsNumActiveOnOffTag
        {
            get
            {
                return _AlarmsNumActiveOnOffTag;
            }
            set
            {
                SetPropertyValue("AlarmsNumActiveOnOffTag", ref _AlarmsNumActiveOnOffTag, value);
            }
        }

        private TagEntityReference _AlarmsNumShelvedTag;
        [ValueConverter(typeof(ConvertTagEntityReference))]
        [Size(SizeAttribute.Unlimited)]
        public TagEntityReference AlarmsNumShelvedTag
        {
            get
            {
                return _AlarmsNumShelvedTag;
            }
            set
            {
                SetPropertyValue("AlarmsNumShelvedTag", ref _AlarmsNumShelvedTag, value);
            }
        }

        private TagEntityReference _AlarmsNumNotAckTag;
        [ValueConverter(typeof(ConvertTagEntityReference))]
        [Size(SizeAttribute.Unlimited)]
        public TagEntityReference AlarmsNumNotAckTag
        {
            get
            {
                return _AlarmsNumNotAckTag;
            }
            set
            {
                SetPropertyValue("AlarmsNumNotAckTag", ref _AlarmsNumNotAckTag, value);
            }
        }
        #endregion

        #region Message Counter Tags
        private TagEntityReference _MessagesNumEnabledTag;
        [ValueConverter(typeof(ConvertTagEntityReference))]
        [Size(SizeAttribute.Unlimited)]
        public TagEntityReference MessagesNumEnabledTag
        {
            get
            {
                return _MessagesNumEnabledTag;
            }
            set
            {
                SetPropertyValue("MessagesNumEnabledTag", ref _MessagesNumEnabledTag, value);
            }
        }

        private TagEntityReference _MessagesNumActiveOnTag;
        [ValueConverter(typeof(ConvertTagEntityReference))]
        [Size(SizeAttribute.Unlimited)]
        public TagEntityReference MessagesNumActiveOnTag
        {
            get
            {
                return _MessagesNumActiveOnTag;
            }
            set
            {
                SetPropertyValue("MessagesNumActiveOnTag", ref _MessagesNumActiveOnTag, value);
            }
        }

        private TagEntityReference _MessagesNumShelvedTag;
        [ValueConverter(typeof(ConvertTagEntityReference))]
        [Size(SizeAttribute.Unlimited)]
        public TagEntityReference MessagesNumShelvedTag
        {
            get
            {
                return _MessagesNumShelvedTag;
            }
            set
            {
                SetPropertyValue("AlarmsNumShelvedTag", ref _MessagesNumShelvedTag, value);
            }
        }
        #endregion

        [Association("UFUAArea-UFUAAlarmSources"), Aggregated]
        public XPCollection<UFUAAlarmSource> UFUAAlarmSources
        {
            get
            {
                return GetCollection<UFUAAlarmSource>("UFUAAlarmSources");
            }
        }

        private UFUAArea _UFUAArea;
        [Association("UFUAArea-UFUAAreas")]
        public UFUAArea UFUAAreaAss
        {
            get
            {
                if (_UFUAArea == this)
                    return null;

                return _UFUAArea;
            }
            set
            {
                if (_UFUAArea == this)
                    return;

                SetPropertyValue("UFUAAreaAss", ref _UFUAArea, value);
            }
        }

        [Association("UFUAArea-UFUAAreas"), Aggregated]
        public XPCollection<UFUAArea> UFUAAreas
        {
            get
            {
                return GetCollection<UFUAArea>("UFUAAreas");
            }
        }


        public static char AreaSeparator = '/';
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

        #region Methods
        
        public string GetRelativeName()
        {
            string name = string.Format("{0}", Name);
            if (UFUAAreaAss != null)
            {
                name = string.Format("{0}{2}{1}", UFUAAreaAss.GetRelativeName(), Name, AreaSeparator);
            }
            return name;
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

#if !NET_STANDARD
        #region IUndoRedoXpo
        [Browsable(false)]
        [NonPersistent]
        public String PathIdentifier
        {
            get
            {
                if (UFUAAreaAss != null)
                    return String.Format("{0}\\{1}", UFUAAreaAss.PathIdentifier, Name);
                else
                    return Name;
            }
        }

        [Browsable(false)]
        [NonPersistent]
        public String UniqueIdentifier
        {
            get
            {
                return NodeId.ToString();
            }
        }

        [Browsable(false)]
        [NonPersistent]
        public String ParentIdentifier
        {
            get
            {
                if (UFUAAreaAss != null)
                    return UFUAAreaAss.UniqueIdentifier;
                return String.Empty;
            }
        }

        [Browsable(false)]
        [NonPersistent]
        public String OwnerIdentifier
        {
            get
            {
                return String.Empty;
            }
        }
        #endregion
#endif

        protected String PerformValidation(String propertyName)
        {
#if !NET_STANDARD
            if (propertyName == "Name")
            {
                if (!Helpers.NameValidator.IsValidAlarmName(Name))
                {
                    return Properties.Resources.AlarmAreaNameInvalid;
                }
                else if (UFUAAreaAss != null)
                {
                    if ((from c in UFUAAreaAss.UFUAAreas/*.AsParallel()*/
                         where c != this && c.Name == Name
                         select c).ToList().Count > 0)
                    {
                        return Properties.Resources.AlarmAreaNameAlreadyExists;
                    }
                }
                else
                {
                    if ((from c in new XPQuery<UFUAModel.UFUAArea>(Session, true)/*.AsParallel()*/
                         where c != this && UFUAAreaAss == null && c.Name == Name
                         select c).ToList().Count > 0)
                    {
                        return Properties.Resources.AlarmAreaNameAlreadyExists;
                    }
                }
            }
            else if (propertyName == "AlarmsNumEnabledTag")
            {
                if (AlarmsNumEnabledTag != null && !AlarmsNumEnabledTag.IsEmpty())
                {
                    var listfound = (from tag in new XPQuery<UFUAModel.UFUATag>(Session, true)/*.AsParallel()*/
                                     where tag.NodeId == AlarmsNumEnabledTag.Guid
                                     select tag).ToList();
                    if (listfound.Count == 0)
                        return Properties.Resources.TagNotFound;
                }
            }
            else if (propertyName == "AlarmsNumActiveOnTag")
            {
                if (AlarmsNumActiveOnTag != null && !AlarmsNumActiveOnTag.IsEmpty())
                {
                    var listfound = (from tag in new XPQuery<UFUAModel.UFUATag>(Session, true)/*.AsParallel()*/
                                     where tag.NodeId == AlarmsNumActiveOnTag.Guid
                                     select tag).ToList();
                    if (listfound.Count == 0)
                        return Properties.Resources.TagNotFound;
                }
            }
            else if (propertyName == "AlarmsNumActiveOffTag")
            {
                if (AlarmsNumActiveOffTag != null && !AlarmsNumActiveOffTag.IsEmpty())
                {
                    var listfound = (from tag in new XPQuery<UFUAModel.UFUATag>(Session, true)/*.AsParallel()*/
                                     where tag.NodeId == AlarmsNumActiveOffTag.Guid
                                     select tag).ToList();
                    if (listfound.Count == 0)
                        return Properties.Resources.TagNotFound;
                }
            }
            else if (propertyName == "AlarmsNumActiveOnOffTag")
            {
                if (AlarmsNumActiveOnOffTag != null && !AlarmsNumActiveOnOffTag.IsEmpty())
                {
                    var listfound = (from tag in new XPQuery<UFUAModel.UFUATag>(Session, true)/*.AsParallel()*/
                                     where tag.NodeId == AlarmsNumActiveOnOffTag.Guid
                                     select tag).ToList();
                    if (listfound.Count == 0)
                        return Properties.Resources.TagNotFound;
                }
            }
            else if (propertyName == "AlarmsNumShelvedTag")
            {
                if (AlarmsNumShelvedTag != null && !AlarmsNumShelvedTag.IsEmpty())
                {
                    var listfound = (from tag in new XPQuery<UFUAModel.UFUATag>(Session, true)/*.AsParallel()*/
                                     where tag.NodeId == AlarmsNumShelvedTag.Guid
                                     select tag).ToList();
                    if (listfound.Count == 0)
                        return Properties.Resources.TagNotFound;
                }
            }
            else if (propertyName == "AlarmsNumNotAckTag")
            {
                if (AlarmsNumNotAckTag != null && !AlarmsNumNotAckTag.IsEmpty())
                {
                    var listfound = (from tag in new XPQuery<UFUAModel.UFUATag>(Session, true)/*.AsParallel()*/
                                     where tag.NodeId == AlarmsNumNotAckTag.Guid
                                     select tag).ToList();
                    if (listfound.Count == 0)
                        return Properties.Resources.TagNotFound;
                }
            }
            else if (propertyName == "MessagesNumEnabledTag")
            {
                if (MessagesNumEnabledTag != null && !MessagesNumEnabledTag.IsEmpty())
                {
                    var listfound = (from tag in new XPQuery<UFUAModel.UFUATag>(Session, true)/*.AsParallel()*/
                                     where tag.NodeId == MessagesNumEnabledTag.Guid
                                     select tag).ToList();
                    if (listfound.Count == 0)
                        return Properties.Resources.TagNotFound;
                }
            }
            else if (propertyName == "MessagesNumActiveOnTag")
            {
                if (MessagesNumActiveOnTag != null && !MessagesNumActiveOnTag.IsEmpty())
                {
                    var listfound = (from tag in new XPQuery<UFUAModel.UFUATag>(Session, true)/*.AsParallel()*/
                                     where tag.NodeId == MessagesNumActiveOnTag.Guid
                                     select tag).ToList();
                    if (listfound.Count == 0)
                        return Properties.Resources.TagNotFound;
                }
            }
            else if (propertyName == "MessagesNumShelvedTag")
            {
                if (MessagesNumShelvedTag != null && !MessagesNumShelvedTag.IsEmpty())
                {
                    var listfound = (from tag in new XPQuery<UFUAModel.UFUATag>(Session, true)/*.AsParallel()*/
                                     where tag.NodeId == MessagesNumShelvedTag.Guid
                                     select tag).ToList();
                    if (listfound.Count == 0)
                        return Properties.Resources.TagNotFound;
                }
            }
#endif
            return null;
        }
    }
}
