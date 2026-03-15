using System;
using System.Linq;
using DevExpress.Xpo;
using System.ComponentModel;
using System.Collections.Generic;
using Utilities;
#if !NET_STANDARD
using Utilities.Xpo.UndoRedo;
#endif

namespace UFUAModel
{
    [Exportable(RequiredKeys = new string[] { "Name" })]
    [DeferredDeletion(false)]
    public class UFUAEngineeringUnit : XPObject, IDataErrorInfo
#if !NET_STANDARD
        , IUndoRedoXpo
#endif
    {
        public UFUAEngineeringUnit(Session session)
            : base(session)
        { }

        public UFUAEngineeringUnit() { }

        #region Properties Default Values
        // List of constant default values for each property where you want handle a default value.
        const double defaultEURangeLow = 0;
        const double defaultEURangeHigh = 100;
        const double defaultInstrumentRangeLow = 0;
        const double defaultInstrumentRangeHigh = 100;

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

            if (!_EURangeLow.HasValue)
                _EURangeLow = defaultEURangeLow;
            if (!_EURangeHigh.HasValue)
                _EURangeHigh = defaultEURangeHigh;
            if (!_InstrumentRangeLow.HasValue)
                _InstrumentRangeLow = defaultInstrumentRangeLow;
            if (!_InstrumentRangeHigh.HasValue)
                _InstrumentRangeHigh = defaultInstrumentRangeHigh;
        }
        #endregion

        #region Properties
        private string _Name;
        //[Indexed(Unique = false)]
        [MergablePropertyAttribute(false)]
        [Size(SizeAttribute.Unlimited)]
        [Exportable]
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                this.RaiseChangeEvent(new ObjectChangeEventArgs(Session, this, "Name", _Name, value));
                SetPropertyValue("Name", ref _Name, value);
            }
        }

        private string _UnitName;
        [Size(SizeAttribute.Unlimited)]
        [Exportable]
        public string UnitName
        {
            get
            {
                return _UnitName;
            }
            set
            {
                SetPropertyValue("UnitName", ref _UnitName, value);
            }
        }

        private double? _EURangeLow;
        [Exportable]
        public double? EURangeLow
        {
            get
            {
                return _EURangeLow;
            }
            set
            {
                SetPropertyValue("EURangeLow", ref _EURangeLow, value);
            }
        }

        private double? _EURangeHigh;
        [Exportable]
        public double? EURangeHigh
        {
            get
            {
                return _EURangeHigh;
            }
            set
            {
                SetPropertyValue("EURangeHigh", ref _EURangeHigh, value);
            }
        }

        private double? _InstrumentRangeLow;
        [Exportable]
        public double? InstrumentRangeLow
        {
            get
            {
                return _InstrumentRangeLow;
            }
            set
            {
                SetPropertyValue("InstrumentRangeLow", ref _InstrumentRangeLow, value);
            }
        }

        private double? _InstrumentRangeHigh;
        [Exportable]
        public double? InstrumentRangeHigh
        {
            get
            {
                return _InstrumentRangeHigh;
            }
            set
            {
                SetPropertyValue("InstrumentRangeHigh", ref _InstrumentRangeHigh, value);
            }
        }
        private int _UnitId;
        [Exportable]
        public int UnitId
        {
            get
            {
                return _UnitId;
            }
            set
            {
                SetPropertyValue(nameof(UnitId), ref _UnitId, value);
            }
        }
        private string _Description;
        [Exportable]
        public string Description
        {
            get
            {
                return _Description;
            }
            set
            {
                SetPropertyValue(nameof(Description), ref _Description, value);
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
        public bool IsValidInstrumentRange()
        {
            return InstrumentRangeHigh > InstrumentRangeLow;
        }
        #endregion

#if !NET_STANDARD
        [Browsable(false)]
        [NonPersistent]
        public String PathIdentifier
        {
            get
            {
                return Name;
            }
        }

        #region IUndoRedoXpo
        [Browsable(false)]
        [NonPersistent]
        public String UniqueIdentifier
        {
            get
            {
                return Name;
            }
        }

        [Browsable(false)]
        [NonPersistent]
        public String ParentIdentifier
        {
            get
            {
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
                if (!Helpers.NameValidator.IsValidEUName(Name))
                {
                    return Properties.Resources.EngineeringUnitNameInvalid;
                }
                else if ((from c in new XPQuery<UFUAModel.UFUAEngineeringUnit>(Session, true)/*.AsParallel()*/
                          where c != this
                          select c)
                            .AsEnumerable()
                            .Where(c => String.Compare(c.Name, Name, true) == 0)
                            .Any())
                {
                    return Properties.Resources.EngineeringUnitNameAlreadyExists;
                }
            }
            else if (propertyName == "EURangeLow")
            {
                if (EURangeLow >= EURangeHigh)
                    return Properties.Resources.EngineeringUnitEUHighLowInvalid;
            }
            else if (propertyName == "EURangeHigh")
            {
                if (EURangeLow >= EURangeHigh)
                    return Properties.Resources.EngineeringUnitEUHighLowInvalid;
            }
            else if (propertyName == "InstrumentRangeLow")
            {
                if (InstrumentRangeLow >= InstrumentRangeHigh)
                    return Properties.Resources.EngineeringUnitInstrumentHighLowInvalid;
            }
            else if (propertyName == "InstrumentRangeHigh")
            {
                if (InstrumentRangeLow >= InstrumentRangeHigh)
                    return Properties.Resources.EngineeringUnitInstrumentHighLowInvalid;
            }
#endif
            return null;
        }

    }
}
