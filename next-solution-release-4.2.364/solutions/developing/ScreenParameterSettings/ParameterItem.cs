using ScreenParametersSettings.Documents;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenParameterSettings
{
    public class ParameterItem : INotifyPropertyChanged, IDataErrorInfo, ICloneable
    {
        #region Declarations
        ScreenParametersDocument document;
        #endregion

        #region Properties
        public readonly Guid Guid;

        // Alias
        string _text;
        public string text
        {
            get
            {
                return _text;
            }
            set
            {
                if (_text == value)
                    return;

                _text = value;

                if (document != null)
                    document.NeedsSave = true;

                OnPropertyChanged("text");
            }
        }

        // Variable (Tag)
        string _ID;
        public string ID
        {
            get
            {
                return _ID;
            }
            set
            {
                if (_ID == value)
                    return;

                _ID = value;

                if (document != null)
                    document.NeedsSave = true;

                OnPropertyChanged("ID");
            }
        }

        #endregion

        #region Ctors

        public ParameterItem(ScreenParametersDocument doc) : this(null, null, doc)
        { }

        public ParameterItem(string id, string txt, ScreenParametersDocument doc) : 
            this()
        {
            ID = id;
            text = txt;
            document = doc;
        }

        public ParameterItem()
        {
            Guid = Guid.NewGuid();
        }

        public ParameterItem(ParameterItem p)
        {
            ID = p.ID;
            text = p.text;
            Guid = p.Guid;
        }

        #endregion

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
                var e = new PropertyChangedEventArgs(propertyName);
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

        #region Methods
        protected String PerformValidation(String propertyName)
        {
            if (propertyName == "ID")
            {
                if (String.IsNullOrWhiteSpace(ID))
                    return Properties.Resources.IDCantBeVoid;
                
            }
            else if (propertyName == "text")
            {
                if (String.IsNullOrWhiteSpace(text))
                    return Properties.Resources.textCantBeVoid;

            }

            return null;
        }
        #endregion

        #region ICloneable Members

        public object Clone()
        {
            var clone = new ParameterItem(this);
            clone.document = this.document;
            return clone;
        }

        #endregion
    }
}
