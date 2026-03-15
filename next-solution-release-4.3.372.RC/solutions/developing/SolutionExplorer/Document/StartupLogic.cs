using DocumentManager.ComponentService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace UFProjectManager.Document
{
    public class StartupLogic : Observable
#if !WINDOWS_UWP && !NET_STANDARD
        , IDataErrorInfo
#endif
    {
#region Properties
        Uri uri;
        public Uri Uri
        {
            get
            {
                return uri;
            }
            set
            {
                Set<Uri>(ref uri, value, "Uri");
                OnPropertyChanged("Uri");
            }
        }

        ExecutionMode executionMode;
        public ExecutionMode ExecutionMode
        {
            get
            {
                return executionMode;
            }
            set
            {
                Set<ExecutionMode>(ref executionMode, value, "ExecutionMode");
                OnPropertyChanged("ExecutionMode");
                OnPropertyChanged("ExecuteAsService");
            }
        }

        bool executeAsService;
        public bool ExecuteAsService
        {
            get
            {
                return executeAsService;
            }
            set
            {
                Set<bool>(ref executeAsService, value, "ExecuteAsService");
                OnPropertyChanged("ExecuteAsService");
                OnPropertyChanged("ExecutionMode");
            }
        }
#endregion

#region IDataErrorInfo
#if !WINDOWS_UWP && !NET_STANDARD
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
#endif
#endregion

#region Methods
#if !WINDOWS_UWP && !NET_STANDARD
        protected String PerformValidation(String propertyName, String prototypeName = null)
        {
            if (propertyName == "ExecutionMode" ||
                propertyName == "ExecuteAsService")
            {
                if (ExecuteAsService && ExecutionMode == ExecutionMode.Shared)
                    return Properties.Resources.LogicSharedExecutionNotAllowed;
            }

            return null;
        }
#endif
#endregion
    }
}
