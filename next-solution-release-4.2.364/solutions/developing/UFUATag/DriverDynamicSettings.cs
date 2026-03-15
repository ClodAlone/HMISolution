using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UFInterfaces.Editors;

namespace UFUAModel
{
    public class DriverDynamicSettings : IDynamicSettingsEditing, IDataErrorInfo, INotifyPropertyChanged
    {
        #region Declarations
        readonly UFUAModel.UFUATag tag;
        readonly String driverName;
        String oldDynamicSettings;
        #endregion

        #region Constructors
        public DriverDynamicSettings(UFUAModel.UFUATag tag, String driverName) : 
            this(tag, driverName, String.Empty)
        { }

        public DriverDynamicSettings(UFUAModel.UFUATag tag, String driverName, String dynamicSettings)
        {
            this.tag = tag;
            this.driverName = driverName;
            this.dynamicSettings = oldDynamicSettings = dynamicSettings;
        }
        #endregion

        #region Properties
        public string DriverName
        {
            get
            {
                return driverName;
            }
        }

        public UFUAModel.UFUATag Tag
        {
            get
            {
                return tag;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return tag == null || tag.ExcludeDynamicSettings;
            }
        }
        #endregion

        #region Methods
        public void ApplyChanges()
        {
            if (oldDynamicSettings != dynamicSettings)
            {
                if (!String.IsNullOrEmpty(dynamicSettings))
                {
                    var driverName = DynamicSettingsForEditing;
                    if (driverName.Contains('.'))
                        driverName = driverName.Substring(0, driverName.IndexOf('.'));
		    // force driver name to lowercase for back compatibility with old Movicon project
                    if (this.driverName.ToLower() == driverName.ToLower())
                    {
                        if (!String.IsNullOrEmpty(oldDynamicSettings))
                            tag.DynamicSettings = tag.DynamicSettings.Replace(oldDynamicSettings, dynamicSettings);
                        else if (!String.IsNullOrEmpty(dynamicSettings))
                        {
                            if (!String.IsNullOrEmpty(tag.DynamicSettings))
                                tag.DynamicSettings = String.Format("{0}{1}{2}", tag.DynamicSettings, UFUAServerInfo.UFUAServerInfo.GetDynSettingsSeparator(), dynamicSettings);
                            else
                                tag.DynamicSettings = dynamicSettings;
                        }
                        oldDynamicSettings = dynamicSettings;
                    }
                }
                else if (!String.IsNullOrEmpty(oldDynamicSettings))
                {
                    var replaceString = String.Format("{0}{1}", oldDynamicSettings, UFUAServerInfo.UFUAServerInfo.GetDynSettingsSeparator());
                    if (tag.DynamicSettings.IndexOf(replaceString) != -1)
                        tag.DynamicSettings = tag.DynamicSettings.Replace(replaceString, dynamicSettings);
                    else
                    {
                        replaceString = String.Format("{1}{0}", oldDynamicSettings, UFUAServerInfo.UFUAServerInfo.GetDynSettingsSeparator());
                        if (tag.DynamicSettings.IndexOf(replaceString) != -1)
                            tag.DynamicSettings = tag.DynamicSettings.Replace(replaceString, dynamicSettings);
                        else
                            tag.DynamicSettings = tag.DynamicSettings.Replace(oldDynamicSettings, dynamicSettings);
                    }
                    oldDynamicSettings = dynamicSettings;
                }
                //oldDynamicSettings = dynamicSettings;
            }
        }

        public void DischargeChanges()
        {
            DynamicSettingsForEditing = oldDynamicSettings;
        }
        #endregion

        #region IDynamicSettingsEditing Members
        public string Name
        {
            get
            {
                return tag.Name;
            }
        }
        public string FolderPath
        {
            get
            {
                return tag.FolderPath;
            }
        }
        public string TagOwnerPath
        {
            get
            {
                return tag.TagOwnerPath;
            }
        }

        public bool IsMethod
        {
            get
            {
                return tag.IsMethod;
            }
        }

        public bool IsObjectType
        {
            get
            {
                return tag.IsObjectType;
            }
        }

        string dynamicSettings;
        public string DynamicSettingsForEditing
        {
            get
            {
                return dynamicSettings;
            }
            set
            {
                if (dynamicSettings == value)
                    return;
                dynamicSettings = value;
                OnPropertyChanged("DynamicSettingsForEditing");
            }
        }

        public int DataType
        {
            get
            {
                if (!tag.DataType.HasValue)
                    return -1;

                return (int)tag.DataType;
            }
            set
            {
                // do nothing
            }
        }

        public uint ArrayDimension
        {
            get
            {
                return tag.ArrayDimension;
            }
            set
            {
                // do nothing
            }
        }

        public IList<IDynamicSettingsEditing> Members
        {
            get
            {
                return tag.Members;

            }
        }
        #endregion

        #region Private Methods
        protected String PerformValidation(String propertyName, String prototypeName = null)
        {
#if !NET_STANDARD
            if (propertyName == "DynamicSettingsForEditing")
            {
                if (!String.IsNullOrEmpty(DynamicSettingsForEditing))
                {
                    var driverName = DynamicSettingsForEditing;
                    if (driverName.Contains('.'))
                        driverName = driverName.Substring(0, driverName.IndexOf('.'));
		    // force driver name to lowercase for back compatibility with old Movicon project
                    if (this.driverName.ToLower() != driverName.ToLower())
                        return String.Format(Properties.Resources.InvalidDynamcSettingsDriverNoMatch, driverName);

                    String error = tag.DynSettingsValidation(DynamicSettingsForEditing);
                    if (!String.IsNullOrEmpty(error))
                        return error;
                }
            }
#endif
            return null;
        }
        #endregion

        #region IDataErrorInfo
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
