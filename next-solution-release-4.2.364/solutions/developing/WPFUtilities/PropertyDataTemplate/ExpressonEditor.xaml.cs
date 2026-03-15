using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using Utilities;
using WPFUtilities.Converters;
using Utilities.WPF;
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
                    value = String.Format("={0}", value);
                Set<String>(ref _Expression, value, "Expression");
            }
        }

        protected String PerformValidation(String propertyName)
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
                    if (!String.IsNullOrEmpty(error))
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

    public partial class ExpressonEditor : UserControl
    {
        public ExpressionViewModel model;

        readonly UserControl VarEditor;
        public ExpressonEditor(UserControl varEditor, String expression)
        {
            InitializeComponent();

            VarEditor = varEditor;
            model = new ExpressionViewModel() { Expression = expression };
            DataContext = model;

            FillListFormulas();
            // cmbVariables.ItemsSource = variables;
            if (String.IsNullOrEmpty(expression))
                expression = "= ";
            model.Expression = expression;
            Loaded += (o, e) =>
            {
                txtExpression.CaretIndex += model.Expression.Length;
            };
        }

        void FillListFormulas()
        {
            using (new WaitCursor())
            {
                cmbFunctions.ItemsSource = Utilities.Converters.ExpressionValueConverterHelper.LibraryFunctions;
            }
        }

        private void AddVariable(object sender, RoutedEventArgs e)
        {
            var variable = String.Format("[{0}]", txtVariables.Text as String);
            model.Expression = model.Expression.Insert(txtExpression.CaretIndex, variable);
            txtExpression.CaretIndex += model.Expression.Length;
            txtExpression.SelectionLength = 0;
        }

        static Boolean isAlphaNumeric(string strToCheck)
        {
            Regex rg = new Regex("[^a-zA-Z0-9]");

            //if has non AlpahNumeric char, return false, else return true.
            return rg.IsMatch(strToCheck) == true ? false : true;
        }

        private void AddFunction(object sender, RoutedEventArgs e)
        {
            var function = cmbFunctions.SelectedValue as String;
            if (isAlphaNumeric(function))
                function = String.Format("{0}(", function);
            model.Expression = model.Expression.Insert(txtExpression.CaretIndex, function);
            txtExpression.CaretIndex += model.Expression.Length;
            txtExpression.SelectionLength = 0;
        }

        private void BrowseVariable(object sender, RoutedEventArgs e)
        {
            var Dialog = new GeneralDialogContent(VarEditor)
            {
                Owner = this.FindParent<Window>(),
                Title = Properties.Resources.ExpressionEditor, 
                DialogKeepContent = true,
                HelpLink = "BrowseVariable"
            };
            if (Dialog.ShowDialog() == false || VarEditor.DataContext == null)
                return;
            var text = String.Format("{0}", VarEditor.DataContext);
            // var text = VarEditor.DataContext.ToString();
            txtVariables.Text = text;
        }
    }
}
