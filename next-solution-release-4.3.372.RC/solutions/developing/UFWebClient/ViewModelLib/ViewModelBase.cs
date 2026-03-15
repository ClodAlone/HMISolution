using System;
using System.Collections.Generic;
using System.ComponentModel;
#if !WINDOWS_PHONE
using System.ComponentModel.DataAnnotations;
#endif
using System.Linq;
using System.Linq.Expressions;
using UFWebClient.Helpers;

namespace UFWebClient.ViewModelLib
{
    public class ViewModelBase : INotifyPropertyChanged
#if !WINDOWS_PHONE
                                , IDataErrorInfo
#endif
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public bool IsValid
        {
            get { return string.IsNullOrEmpty(Error); }
        }

        public string Error
        {
            get
            {
#if !WINDOWS_PHONE
                var context = new ValidationContext(this, null, null);
                var results = new List<ValidationResult>();

                return !Validator.TryValidateObject(this, context, results)
                    ? string.Join(Environment.NewLine, results.Select(x => x.ErrorMessage))
                    : null;
#else
                return null;
#endif
            }
        }

#if !WINDOWS_PHONE
        public string this[string propertyName]
        {
            get
            {
                var context = new ValidationContext(this, null, null)
                {
                    MemberName = propertyName
                };

                var results = new List<ValidationResult>();
                var propertyInfo = GetType().GetProperty(propertyName);
                if (propertyInfo != null)
                {
                    var value = propertyInfo.GetValue(this, null);

                    return !Validator.TryValidateProperty(value, context, results)
                        ? string.Join(Environment.NewLine, results.Select(x => x.ErrorMessage))
                        : null;
                }

                return null;
            }
        }
#endif

        public void NotifyOfPropertyChange(string propertyName)
        {
            Execute.OnUIThread(() => PropertyChanged(this, new PropertyChangedEventArgs(propertyName)));
        }

        public void NotifyOfPropertyChange<TProperty>(Expression<Func<TProperty>> property)
        {
            var lambda = (LambdaExpression)property;

            MemberExpression memberExpression;
            if (lambda.Body is UnaryExpression)
            {
                var unaryExpression = (UnaryExpression)lambda.Body;
                memberExpression = (MemberExpression)unaryExpression.Operand;
            }
            else memberExpression = (MemberExpression)lambda.Body;

            NotifyOfPropertyChange(memberExpression.Member.Name);
        }
    }
}
