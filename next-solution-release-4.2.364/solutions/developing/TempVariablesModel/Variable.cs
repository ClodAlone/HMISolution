using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Xpo;
using UFInterfaces.PropertyControl;
using UFUAModel;

namespace TempVariablesModel
{
    public class Variable : XPObject, IDataErrorInfo, INotifyPropertyVisibilityChanged, XpoHelpers.UndoRedoIXPSimpleObjectHelper.IUniqueIdentifier
    {
        #region Ctor
        public Variable(Session session)
            : base(session)
        {
           
        }
        #endregion

        #region Properties Default Values
        // List of constant default values for each property where you want handle a default value.
        const int defaultMemberOrderId = -1;

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

            if (!MemberOrderId.HasValue)
                MemberOrderId = defaultMemberOrderId;
        }
        #endregion

        #region Not Persistence Properties
        [Browsable(false)]
        [NonPersistent]
        public String FolderPath
        {
            get
            {
                String ret = String.Empty;
                Folder folder = Folder;
                while (folder != null)
                {
                    if (String.IsNullOrEmpty(ret))
                        ret = folder.Name;
                    else
                        ret = String.Format("{0}&{1}", folder.Name, ret);
                    folder = folder.FolderAss;
                }

                return ret;
            }
        }
        [Browsable(false)]
        [NonPersistent]
        public String FolderReadablePath
        {
            get
            {
                String ret = String.Empty;
                Folder folder = Folder;
                while (folder != null)
                {
                    if (String.IsNullOrEmpty(ret))
                        ret = folder.Name;
                    else
                        ret = String.Format("{0}\\{1}", folder.Name, ret);
                    folder = folder.FolderAss;
                }

                return ret;
            }
        }


        #endregion

        #region Properties

        private int? _MemberOrderId;
        [Browsable(false)]
        public int? MemberOrderId
        {
            get
            {
                return _MemberOrderId;
            }
            set
            {
                SetPropertyValue("MemberOrderId", ref _MemberOrderId, value);
            }
        }

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

        private string _Description;
        [Size(SizeAttribute.Unlimited)]
        public string Description
        {
            get
            {
                return _Description;
            }
            set
            {
                SetPropertyValue("Description", ref _Description, value);
            }
        }

        private Folder _Folder;
        [Association("Folder-Variables")]
        [Browsable(false)]
        public Folder Folder
        {
            get
            {
                return _Folder;
            }
            set
            {
                SetPropertyValue("Folder", ref _Folder, value);
            }
        }

        private Guid _NodeId;
        [Custom("Generate", "Guid")]
        [Browsable(false)]
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


        private DataType _DataType;
        public DataType DataType
        {
            get
            {
                return _DataType;
            }
            set
            {
                var bChanged = _DataType != value;
                SetPropertyValue("DataType", ref _DataType, value);
                if (IsLoading && bChanged)
                    RaisePropertyChangedEvent("DataType");
            }
        }

        private int _ArrayDimension;
        public int ArrayDimension
        {
            get 
            {
                return _ArrayDimension;
            }
            set
            {
                SetPropertyValue("ArrayDimension", ref _ArrayDimension, value);
            }
        }

        private string _InitialValue;
        [Size(SizeAttribute.Unlimited)]
        public string InitialValue
        {
            get
            {
                return _InitialValue;
            }
            set
            {
                SetPropertyValue("InitialValue", ref _InitialValue, value);
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

        public string GetRelativePath(UInt16 ns, bool bRelative = false)
        {
            if (bRelative)
            {
                string path = string.Format("{0}:{1}", ns, Name);
                if (Folder != null)
                {
                    path = string.Format("{1}:{0}", Name, ns);
                }
                return path;
            }
            else
            {
                string path = string.Format("{0}:{1}&{0}:{2}", ns, Properties.Resources.Tags, Name);
                if (Folder != null)
                {
                    path = string.Format("{0}&{2}:{1}", Folder.GetRelativePath(ns, bRelative), Name, ns);
                }
                return path;
            }
        }

        public string GetRelativeName()
        {
            string name = string.Format("{0}", Name);
            if (Folder != null)
            {
                name = string.Format("{0}&{1}", Folder.GetRelativeName(), Name);
            }
            return name;
        }

        public string GetRelative()
        {
            string name = string.Format("{0}", Name);
            if (Folder != null)
            {
                name = string.Format("{0}&{1}", Folder.GetRelativeName(), Name);
            }
            return name;
        }

        protected String PerformValidation(String propertyName, String prototypeName = null)
        {
            if (propertyName == "Name")
            {
                if (!UFUAModel.Helpers.NameValidator.IsValidName(Name))
                {
                    return Properties.Resources.TagNameInvalid;
                }
                else if (Folder != null && (from c in Folder.Variables/*.AsParallel()*/
                                                where c != this && c.Name == Name
                                                select c).ToList().Count > 0)
                {
                    return Properties.Resources.TagNameAlreadyExists;
                }
                else if ((from tag in new XPQuery<Variable>(Session, true)/*.AsParallel()*/
                          where tag.Folder == null && tag.Name == Name
                          select tag).ToList().Count > 1)
                {
                    return Properties.Resources.TagNameAlreadyExists;
                }
            }
            return null;
        }

        #endregion

        #region INotifyPropertyVisibilityChanged Members

        /// <summary>
        /// Gets the visibility state for the property with the given name.
        /// </summary>
        /// <param name="propertyName">The property name that you want konw the current visibility state.</param>
        /// <returns></returns>
        bool INotifyPropertyVisibilityChanged.this[string propertyName]
        {
            get
            {
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

        #endregion

        #region IUniqueIdentifier
        [Browsable(false)]
        [NonPersistent]
        public String UniqueIdentifier
        {
            get
            {
                return NodeId.ToString();
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
        const String NameMacth = @"^[a-zA-Z][a-zA-Z0-9]*$";
        static bool IsValidName(string name)
        {
            if (String.IsNullOrEmpty(name))
                return false;

            return System.Text.RegularExpressions.Regex.IsMatch(name, NameMacth);
        }
        protected String PerformValidation(String propertyName)
        {
            if (propertyName == "Name")
            {
                if (!UFUAModel.Helpers.NameValidator.IsValidName(Name))
                {
                    return Properties.Resources.TagNameInvalid;
                }
                else if (Folder != null && (from c in Folder.Variables/*.AsParallel()*/
                                                where c != this && c.Name == Name
                                                select c).ToList().Count > 0)
                {
                    return Properties.Resources.TagNameAlreadyExists;
                }
                else if ((from tag in new XPQuery<Variable>(Session, true)/*.AsParallel()*/
                          where tag.Folder == null && tag.Name == Name
                          select tag).ToList().Count > 1)
                {
                    return Properties.Resources.TagNameAlreadyExists;
                }
            }
            else if (propertyName == "InitialValue")
            {
                if (!String.IsNullOrEmpty(InitialValue))
                {
                    if (ArrayDimension > 0)
                    {
                        if (InitialValue[0] != '{' || InitialValue[InitialValue.Length - 1] != '}')
                            return Properties.Resources.TagInitialValueInvalidSyntaxForArray;
                        else if (ArrayDimension != InitialValue.Substring(1, InitialValue.Length - 2).Split('|').Length)
                            return Properties.Resources.TagInitialValueInvalidNumberOfElements;
                    }
                }
            }
            return null;
        }
        #endregion
    }
}
