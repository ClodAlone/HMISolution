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
    public class UFUAAlarmSource : XPObject, IDataErrorInfo
#if !NET_STANDARD
        , IUndoRedoXpo
#endif
    {
        #region Constructors
        public UFUAAlarmSource(Session session)
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

        //private ushort _MaxBranches;
        //public ushort MaxBranches
        //{
        //    get
        //    {
        //        return _MaxBranches;
        //    }
        //    set
        //    {
        //        SetPropertyValue("MaxBranches", ref _MaxBranches, value);
        //    }
        //}

        [Association("UFUAAlarmSource-UFUAAlarmDefinitions"), Aggregated]
        public XPCollection<UFUAAlarmDefinition> UFUAAlarmDefinitions
        {
            get
            {
                return GetCollection<UFUAAlarmDefinition>("UFUAAlarmDefinitions");
            }
        }

        private UFUAArea _UFUAArea;
        [Association("UFUAArea-UFUAAlarmSources")]
        public UFUAArea UFUAArea
        {
            get
            {
                return _UFUAArea;
            }
            set
            {
                SetPropertyValue("UFUAArea", ref _UFUAArea, value);
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

        #region Methods

        public string GetRelativeName()
        {
            string name = string.Format("{0}", Name);
            if (UFUAArea != null)
            {
                name = string.Format("{0}{2}{1}", UFUAArea.GetRelativeName(), Name, UFUAArea.AreaSeparator);
            }
            return name;
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
                if (UFUAArea != null)
                    return String.Format("{0}\\{1}", UFUAArea.PathIdentifier, Name);
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
                if (UFUAArea != null)
                    return UFUAArea.UniqueIdentifier;
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

        protected String PerformValidation(String propertyName)
        {
#if !NET_STANDARD
            if (propertyName == "Name")
            {
                if (!Helpers.NameValidator.IsValidAlarmName(Name))
                {
                    return Properties.Resources.AlarmSourceNameInvalid;
                }
                else if (UFUAArea != null)
                {
                    if ((from c in UFUAArea.UFUAAlarmSources/*.AsParallel()*/
                         where c != this && c.Name == Name
                         select c).ToList().Count > 0)
                    {
                        return Properties.Resources.AlarmSourceNameAlreadyExists;
                    }
                }
                else
                {
                    if ((from c in new XPQuery<UFUAModel.UFUAAlarmSource>(Session, true)/*.AsParallel()*/
                         where c != this && UFUAArea == null && c.Name == Name
                         select c).ToList().Count > 0)
                    {
                        return Properties.Resources.AlarmSourceNameAlreadyExists;
                    }
                }
            }
#endif
            return null;
        }
    }
}
