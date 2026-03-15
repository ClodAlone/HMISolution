using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using Utilities;
using WPFUtilities.PropertyDataTemplate;

namespace UnitConverterManager.Controls
{
    /// <summary>
    /// Interaction logic for ExpressonEditor.xaml
    /// </summary>
    /// 

    public partial class UConverterEditor : UserControl
    {
        public ExpressionViewModel model;

        readonly UserControl VarEditor;
        public UConverterEditor(UserControl varEditor, String expression)
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
    }
}
