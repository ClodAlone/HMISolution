using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Utilities.Converters;

namespace WPFUtilities.PropertyDataTemplate
{
    /// <summary>
    /// Interaction logic for ExpressonEditor.xaml
    /// </summary>
    /// 

    public class ExpressionViewModel : Utilities.Observable, IDataErrorInfo
    {
        private string _Expression;
        public string Expression
        {
            get { return _Expression; }
            set
            {
                if (value != null && !value.StartsWith("="))
                    value = string.Format("={0}", value);
                Set(ref _Expression, value, "Expression");
            }
        }

        protected string PerformValidation(string propertyName)
        {
            if (propertyName == "Expression")
            {
                using (var expressor = new ExpressionValueConverter())
                {
                    if (Expression != null && !Expression.StartsWith("="))
                        return Properties.Resources.ExpressionStartEqual;
                    expressor.Formula = Expression;
                    expressor.ParseFormula();
                    var error = expressor.GetParserError();
                    if (!string.IsNullOrEmpty(error))
                        return error;
                }
            }

            return null;
        }

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
                string s = PerformValidation(propertyName);
                if (!string.IsNullOrEmpty(s))
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