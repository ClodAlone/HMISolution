using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataValidation
{
    public class BackupFileViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        #region Declarations
        readonly bool isReadOnly;
        #endregion

        #region Constructors
        public BackupFileViewModel(string filePath) :
            this(filePath, false)
        { }

        public BackupFileViewModel(string filePath, bool isReadOnly)
        {
            this.filePath = filePath;
            this.isReadOnly = isReadOnly;
        }
        #endregion

        #region Properties
        string filePath;
        public string FilePath
        {
            get
            {
                return filePath;
            }
            set
            {
                if (filePath == value)
                    return;
                filePath = value;
                OnPropertyChanged("FilePath");
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return isReadOnly;
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

        protected String PerformValidation(String propertyName, String prototypeName = null)
        {
            //if (propertyName == "FilePath")
            //{
            //    if (!System.IO.File.Exists(FilePath))
            //        return Properties.Resources.BakcupFileNotFound;
            //}

            return null;
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
    }
}
