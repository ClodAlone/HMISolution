using System;
using System.Linq;
using DevExpress.Xpo;
using System.ComponentModel;
using System.Collections.Generic;
using UFInterfaces.Scriptable;
using System.Collections;
using System.Text;
using UFInterfaces.Editors;
using System.Text.RegularExpressions;
using System.Reflection;
using UFInterfaces.PropertyControl;


namespace UnitConverterModel
{
    [DeferredDeletion(false)]
    public class UFConverterItem : XPObject, ICloneable, IDataErrorInfo, IComparable<UFConverterItem>, IComparable
    {
        #region Constructors
        public UFConverterItem(Session session)
            : base(session)
        { }

        public UFConverterItem(UFConverterItem source)
        {
            Name = source.Name;
            InputExpression = source.InputExpression;
            OutputExpression = source.OutputExpression;
            InputUnit = source.InputUnit;
            Description = source.Description;
            Locale = source.Locale;
            Culture = source.Culture;
        }
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
            //if (!PropertyName.HasValue)
            //    PropertyName = defaultPropertyName;
        }
        #endregion

        #region Non Persistent Properties
        [NonPersistent()]
        [Browsable(false)]
        public string ConverterSummary
        {
            get
            {
                return ToString();
            }
        }
        #endregion

        #region Properties
        private string _Name;
        [Size(SizeAttribute.Unlimited)]
        [Browsable(false)]
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

        private string _inputexpression = string.Empty;
        [Size(SizeAttribute.Unlimited)]
        public string InputExpression
        {
            get
            {
                return _inputexpression;
            }
            set
            {
                if (SetPropertyValue("InputExpression", ref _inputexpression, value))
                    RaisePropertyChangedEvent("ConverterSummary");
            }
        }

        private string _outputexpression = string.Empty;
        [Size(SizeAttribute.Unlimited)]
        public string OutputExpression
        {
            get
            {
                return _outputexpression;
            }
            set
            {
                if (SetPropertyValue("OutputExpression", ref _outputexpression, value))
                    RaisePropertyChangedEvent("ConverterSummary");
            }
        }
        private string _inputunit = string.Empty;
        [Size(SizeAttribute.Unlimited)]
        public string InputUnit
        {
            get
            {
                return _inputunit;
            }
            set
            {
                if (SetPropertyValue("InputUnit", ref _inputunit, value))
                    RaisePropertyChangedEvent("ConverterSummary");
            }
        }

        private string _description = string.Empty;
        [Size(SizeAttribute.Unlimited)]
        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                if (SetPropertyValue("OutputUnit", ref _description, value))
                    RaisePropertyChangedEvent("ConverterSummary");
            }
        }

        private string _Locale;
        [Size(SizeAttribute.Unlimited)]
        [Browsable(false)]
        public string Locale
        {
            get
            {
                return _Locale;
            }
            set
            {
                SetPropertyValue("Locale", ref _Locale, value);
            }
        }

        private string _Culture;
        [Size(SizeAttribute.Unlimited)]
        [Browsable(false)]
        public string Culture
        {
            get
            {
                return _Culture;
            }
            set
            {
                SetPropertyValue("Culture", ref _Culture, value);
            }
        }

        private UFConverter _UFConverter;
        [Association("UFConverter-UFConverterItems")]
        [Browsable(false)]
        public UFConverter UFConverter
        {
            get
            {
                return _UFConverter;
            }
            set
            {
                SetPropertyValue("UFConverter", ref _UFConverter, value);
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

        public override string ToString()
        {
            string ret = $"{InputExpression}{ExportUtils.csvseparator} {OutputExpression}{ExportUtils.csvseparator} {InputUnit}{ExportUtils.csvseparator} {Description}";
            return ret;
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

        protected String PerformValidation(String propertyName, String prototypeName = null)
        {
            if (propertyName == "Name")
            {
                if (!Helpers.NameValidator.IsValidName(Name))
                {
                    return Properties.Resource.TagNameInvalid;
                }
            }
            else if (propertyName == "InputExpression")
            {
                using (var expressor = new Utilities.Converters.ExpressionValueConverter())
                {
                    if (InputExpression != null && !InputExpression.StartsWith("="))
                        return Properties.Resource.ExpressionStartEqual;
                    expressor.Formula = InputExpression;
                    expressor.ParseFormula();
                    var error = expressor.GetParserError();
                    if (!String.IsNullOrEmpty(error))
                        return error;
                }
            }
            else if (propertyName == "OutputExpression")
            {
                using (var expressor = new Utilities.Converters.ExpressionValueConverter())
                {
                    if (OutputExpression != null && !OutputExpression.StartsWith("="))
                        return Properties.Resource.ExpressionStartEqual;
                    expressor.Formula = OutputExpression;
                    expressor.ParseFormula();
                    var error = expressor.GetParserError();
                    if (!String.IsNullOrEmpty(error))
                        return error;
                }
            }
            return null;
        }

        public object Clone()
        {
            return new UFConverterItem(this);
        }

        public int CompareTo(object obj)
        {
            if (object.ReferenceEquals(null, obj))
                return 1;
            UnitConverterModel.UFConverterItem _obj = obj as UnitConverterModel.UFConverterItem;
            if (_obj == null)
                throw new ArgumentException("Object is not of type UFConverterItem");
            return this.ToString().CompareTo(_obj.ToString());
        }

        public int CompareTo(UFConverterItem other)
        {
            if (object.ReferenceEquals(null, other))
                return 1;
            if (other == null)
                throw new ArgumentException("Object is not of type UFConverterItem");
            return this.ToString().CompareTo(other.ToString());
        }
        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(null, obj))
                return false;
            UnitConverterModel.UFConverterItem _obj = obj as UnitConverterModel.UFConverterItem;
            if (_obj == null)
                throw new ArgumentException("Object is not of type UFConverterItem");
            return this.ToString().Equals(_obj.ToString());
        }
    }
}
