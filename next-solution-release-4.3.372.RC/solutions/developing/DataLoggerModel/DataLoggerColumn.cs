using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLoggerModel.Helpers;
using DevExpress.Xpo;
using Utilities;
using Utilities.Converters;
#if !NET_STANDARD
using Utilities.Xpo.UndoRedo;
#endif

namespace DataLoggerModel
{
    [Exportable(RequiredKeys = new string[] { "FolderPath", "ColumnName" }, ImportFolderInfo = "FolderPath", AggregatedProperties = new string[] { "ColumnTagName" })]
    [DeferredDeletion(false)]
    public class DataLoggerColumn : XPObject, IDataErrorInfo
#if !NET_STANDARD
        , IUndoRedoXpo
#endif
    {
        #region Constructors
        /// <summary>
        /// Initializes the XPObject by passing the session.
        /// </summary>
        /// <param name="session"></param>
        public DataLoggerColumn(Session session)
            : base(session)
        {
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
            //if (TimeSpanPropertyName == TimeSpan.Zero)
            //    TimeSpanPropertyName = TimeSpan.FromMinutes(1);
            //if (DateTimePropertyName == DateTime.MinValue)
            //    DateTimePropertyName = DateTime.UtcNow;
        }
        #endregion

        #region Not Persistance Properties
        [NonPersistent]
        [Browsable(false)]
        public string Name
        {
            get
            {
                return _ColumnName;
            }
        }

        [NonPersistent]
        public bool IsValid
        {
            get
            {
                return !String.IsNullOrEmpty(ColumnName) && ColumnTag != null && !ColumnTag.IsEmpty();
            }
        }

        private UFUAModel.UFUATag _UFUATagReference;
        /// <summary>
        /// The UFUATag reference to use for recording value.
        /// </summary>
        [NonPersistent]
        [Browsable(false)]
        public UFUAModel.UFUATag UFUATagReference
        {
            get
            {
                return _UFUATagReference;
            }
            set
            {
                if (_UFUATagReference == value)
                    return;
                _UFUATagReference = value;
            }
        }

        [Browsable(false)]
        [NonPersistent]
        [Exportable]
        public String ColumnTagName
        {
            get
            {
                if (ColumnTag != null && !string.IsNullOrEmpty(ColumnTag.ToString()))
                {
                    var path = Utilities.NamespaceTableConverter.GetRelativePath(ColumnTag.Name);
                    return path;
                }
                else
                    return string.Empty;
            }
        }

        [Browsable(false)]
        [NonPersistent]
        public String ColumnFullName
        {
            get
            {
                return String.Format("{1} - {0}", String.IsNullOrEmpty(ColumnTagName) ? Properties.Resources.DataLoggerColumnTagEmpty : ColumnTagName, Name);
            }
        }
        #endregion

        #region Properties
        private string _ColumnName;
        /// <summary>
        /// The name used to define the column.
        /// </summary>
        /// <remarks>
        /// The name is unique for each column in the data logger.
        /// </remarks>
        [MergablePropertyAttribute(false)]
        [Exportable]
        public string ColumnName
        {
            get
            {
                return _ColumnName;
            }
            set
            {
                if (SetPropertyValue("ColumnName", ref _ColumnName, value))
                {
                    RaisePropertyChangedEvent("ColumnFullName");
                }
            }
        }

        private bool _SkipDataChange;
        /// <summary>
        /// Allow to skip this data change colum's value for adding a new record.
        /// </summary>
        [Exportable]
        public bool SkipDataChange
        {
            get
            {
                return _SkipDataChange;
            }
            set
            {
                SetPropertyValue("SkipDataChange", ref _SkipDataChange, value);
            }
        }

        private bool _AddSourceTimeStampColumn;
        /// <summary>
        /// Allow to add an optional column for recording the Tag's source time stamp.
        /// </summary>
        [Exportable]
        public bool AddSourceTimeStampColumn
        {
            get
            {
                return _AddSourceTimeStampColumn;
            }
            set
            {
                SetPropertyValue("AddSourceTimeStampColumn", ref _AddSourceTimeStampColumn, value);
            }
        }

        private string _SourceTimeStampSuffixColumnName;
        /// <summary>
        /// The column name used for recording the Tag's source time stamp.
        /// </summary>
        /// <remarks>
        /// The name is optional. An empty or null value force the data logger to use a default value.
        /// </remarks>
        [Exportable]
        public string SourceTimeStampSuffixColumnName
        {
            get
            {
                return _SourceTimeStampSuffixColumnName;
            }
            set
            {
                SetPropertyValue("SourceTimeStampSuffixColumnName", ref _SourceTimeStampSuffixColumnName, value);
            }
        }

        private bool _AddServerTimeStampColumn;
        /// <summary>
        /// Allow to add an optional column for recording the Tag's server time stamp.
        /// </summary>
        [Exportable]
        public bool AddServerTimeStampColumn
        {
            get
            {
                return _AddServerTimeStampColumn;
            }
            set
            {
                SetPropertyValue("AddServerTimeStampColumn", ref _AddServerTimeStampColumn, value);
            }
        }

        private string _ServerTimeStampSuffixColumnName;
        /// <summary>
        /// The column name used for recording the Tag's server time stamp.
        /// </summary>
        /// <remarks>
        /// The name is optional. An empty or null value force the data logger to use a default value.
        /// </remarks>
        [Exportable]
        public string ServerTimeStampSuffixColumnName
        {
            get
            {
                return _ServerTimeStampSuffixColumnName;
            }
            set
            {
                SetPropertyValue("ServerTimeStampSuffixColumnName", ref _ServerTimeStampSuffixColumnName, value);
            }
        }

        private bool _AddStatusCodeColumn;
        /// <summary>
        /// Allow to add an optional column for recording the Tag's quality.
        /// </summary>
        [Exportable]
        public bool AddStatusCodeColumn
        {
            get
            {
                return _AddStatusCodeColumn;
            }
            set
            {
                SetPropertyValue("AddStatusCodeColumn", ref _AddStatusCodeColumn, value);
            }
        }

        private string _StatusCodeSuffixColumnName;
        /// <summary>
        /// The column name used for recording the Tag's quality.
        /// </summary>
        /// <remarks>
        /// The name is optional. An empty or null value force the data logger to use a default value.
        /// </remarks>
        [Exportable]
        public string StatusCodeSuffixColumnName
        {
            get
            {
                return _StatusCodeSuffixColumnName;
            }
            set
            {
                SetPropertyValue("StatusCodeSuffixColumnName", ref _StatusCodeSuffixColumnName, value);
            }
        }

        private bool _AddUserColumn;
        /// <summary>
        /// Allow to add an optional column for recording the latest user who changes the tag's value.
        /// </summary>
        [Exportable]
        public bool AddUserColumn
        {
            get
            {
                return _AddUserColumn;
            }
            set
            {
                SetPropertyValue("AddUserColumn", ref _AddUserColumn, value);
            }
        }

        private string _UserSuffixColumnName;
        /// <summary>
        /// The column name used for recording the latest user who changes the tag's value.
        /// </summary>
        /// <remarks>
        /// The name is optional. An empty or null value force the data logger to use a default value.
        /// </remarks>
        [Exportable]
        public string UserSuffixColumnName
        {
            get
            {
                return _UserSuffixColumnName;
            }
            set
            {
                SetPropertyValue("UserSuffixColumnName", ref _UserSuffixColumnName, value);
            }
        }

        private bool _AddStringValueColumn;
        /// <summary>
        /// Allow to add an optional column for recording the string rapresentation of the tag's value.
        /// </summary>
        [Exportable]
        public bool AddStringValueColumn
        {
            get
            {
                return _AddStringValueColumn;
            }
            set
            {
                SetPropertyValue("AddStringValueColumn", ref _AddStringValueColumn, value);
            }
        }

        private string _StringValueSuffixColumnName;
        /// <summary>
        /// The column name used for recording the string rapresentation of the tag's value.
        /// </summary>
        /// <remarks>
        /// The name is optional. An empty or null value force the data logger to use a default value.
        /// </remarks>
        [Exportable]
        public string StringValueSuffixColumnName
        {
            get
            {
                return _StringValueSuffixColumnName;
            }
            set
            {
                SetPropertyValue("StringValueSuffixColumnName", ref _StringValueSuffixColumnName, value);
            }
        }

        private UFUAModel.TagEntityReference _ColumnTag;
        /// <summary>
        /// The tag entity reference linked to this column.
        /// </summary>
        /// <remarks>
        /// The tag is automatically reset after the operation has been performed.
        /// </remarks>
        [ValueConverter(typeof(UFUAModel.ConvertTagEntityReference))]
        [Size(SizeAttribute.Unlimited)]
        public UFUAModel.TagEntityReference ColumnTag
        {
            get
            {
                return _ColumnTag;
            }
            set
            {
                if (SetPropertyValue("ColumnTag", ref _ColumnTag, value))
                {
                    RaisePropertyChangedEvent("ColumnFullName");
                }
            }
        }

        private string _Expression;
        [Size(SizeAttribute.Unlimited)]
        [Exportable]
        public string Expression
        {
            get
            {
                return _Expression;
            }
            set
            {
                SetPropertyValue("Expression", ref _Expression, value);
            }
        }

        private DataLoggerSettings _DataLoggerReference;
        /// <summary>
        /// The data logger reference to which column refers.
        /// </summary>
        [Association("DataLoggerSettings-Columns")]
        public DataLoggerSettings DataLoggerReference
        {
            get
            {
                return _DataLoggerReference;
            }
            set
            {
                SetPropertyValue("DataLoggerReference", ref _DataLoggerReference, value);
            }
        }
        [Browsable(false)]
        [NonPersistent]
        [Exportable]
        public String FolderPath
        {
            get
            {
                if (DataLoggerReference != null)
                    return DataLoggerReference.GetDataloggerName();
                else return string.Empty;
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

        #region Private Methods
        protected String PerformValidation(String propertyName)
        {
            if (propertyName == "ColumnName")
            {
                if (String.IsNullOrEmpty(ColumnName))
                {
                    return Properties.Resources.DataLoggerColumnNameEmpty;
                }
                else if (!DBNameValidator.IsValidName(ColumnName))
                {
                    return Properties.Resources.DataLoggerColumnNameInvalidChars;
                }
                else if (DataLoggerSettingsHelper.ReservedColumnNames.ToList().Contains(ColumnName))
                {
                    return Properties.Resources.DataLoggerColumnNameReservedName;
                }
                else if ((from c in new XPQuery<DataLoggerModel.DataLoggerColumn>(Session, true)/*.AsParallel()*/
                     where c != this && c.ColumnName == ColumnName && c.DataLoggerReference == DataLoggerReference
                     select c).ToList().Count > 0)
                {
                    return Properties.Resources.DataLoggerColumnNameAlreadyExists;
                }
            }
            else if (propertyName == "SourceTimeStampSuffixColumnName")
            {
                if (AddSourceTimeStampColumn && !String.IsNullOrEmpty(SourceTimeStampSuffixColumnName) && !DBNameValidator.IsValidName(SourceTimeStampSuffixColumnName))
                {
                    return Properties.Resources.DataLoggerColumnNameInvalidChars;
                }
            }
            else if (propertyName == "ServerTimeStampSuffixColumnName")
            {
                if (AddServerTimeStampColumn && !String.IsNullOrEmpty(ServerTimeStampSuffixColumnName) && !DBNameValidator.IsValidName(ServerTimeStampSuffixColumnName))
                {
                    return Properties.Resources.DataLoggerColumnNameInvalidChars;
                }
            }
            else if (propertyName == "StatusCodeSuffixColumnName")
            {
                if (AddStatusCodeColumn && !String.IsNullOrEmpty(StatusCodeSuffixColumnName) && !DBNameValidator.IsValidName(StatusCodeSuffixColumnName))
                {
                    return Properties.Resources.DataLoggerColumnNameInvalidChars;
                }
            }
            else if (propertyName == "UserSuffixColumnName")
            {
                if (AddUserColumn && !String.IsNullOrEmpty(UserSuffixColumnName) && !DBNameValidator.IsValidName(UserSuffixColumnName))
                {
                    return Properties.Resources.DataLoggerColumnNameInvalidChars;
                }
            }
            else if (propertyName == "StringValueSuffixColumnName")
            {
                if (AddStringValueColumn && !String.IsNullOrEmpty(StringValueSuffixColumnName) && !DBNameValidator.IsValidName(StringValueSuffixColumnName))
                {
                    return Properties.Resources.DataLoggerColumnNameInvalidChars;
                }
            }
            else if (propertyName == "ColumnTag")
            {
                if (ColumnTag != null && !ColumnTag.IsEmpty())
                {
                    var listfound = (from tag in new XPQuery<UFUAModel.UFUATag>(Session, true)/*.AsParallel()*/
                                     where tag.NodeId == ColumnTag.Guid
                                     select tag).ToList();
                    if (listfound.Count == 0)
                        return Properties.Resources.TagEntityReferenceNotFound;
                }
            }
            else if (propertyName == "Expression")
            {
                if (!String.IsNullOrEmpty(Expression) && AliasHelper.GetAliasCount(Expression) == 0)
                {
                    var type = ExpressionValueConverter.GetFormulaType(Expression);
                    if (type == ExpressionType.none || type == ExpressionType.error)
                        return Properties.Resources.DataLoggerColumnExpressionInvalid;
                    else if (type == ExpressionType.Expression)
                    {
                        using (var expressor = new ExpressionValueConverter(Expression))
                        {
                            expressor.ParseFormula();
                            var error = expressor.GetParserError();
                            if (!String.IsNullOrEmpty(error))
                                return String.Format(Properties.Resources.DataLoggerColumnExpressionError, error);
                            else
                            {
                                var listVariables = expressor.GetFormulaVariables(Expression);
                                if (listVariables.Count > 0)
                                    return Properties.Resources.DataLoggerColumnExpressionInvalid;
                            }
                        }
                    }
                }
            }

            return null;
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
                if (DataLoggerReference != null)
                    return String.Format("{0}\\{1}", DataLoggerReference.PathIdentifier, Name);
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
                return ColumnName;
            }
        }

        [Browsable(false)]
        [NonPersistent]
        public String ParentIdentifier
        {
            get
            {
                if (DataLoggerReference != null)
                    return DataLoggerReference.UniqueIdentifier;
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
    }
}
