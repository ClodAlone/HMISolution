using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Threading;
using Utilities;
using WPFUtilities.PropertyDataTemplate;

namespace OPCUAViewModel.PropertyDataTemplate
{
    public partial class ExpressionEditor
    {
        private readonly OPCUAEntityReference _entityReference;

        public ExpressionEditor(string expression)
        {
            InitializeComponent();

            _entityReference = new OPCUAEntityReference(null);
            
            Model = new ExpressionViewModel { Expression = expression };
            DataContext = Model;

            FillListFormulas();
            // cmbVariables.ItemsSource = variables;
            if (string.IsNullOrEmpty(expression))
                expression = "= ";
            Model.Expression = expression;
            Loaded += (o, e) =>
            {
                txtExpression.CaretIndex += Model.Expression.Length;
            };
        }

        public ExpressionViewModel Model { get; }

        private void FillListFormulas()
        {
            using (new WaitCursor())
            {
                cmbFunctions.ItemsSource = Utilities.Converters.ExpressionValueConverterHelper.LibraryFunctions;
            }
        }

        private void AddVariable(object sender, RoutedEventArgs e)
        {
            var variable = $"[{txtVariables.Text}]";
            Model.Expression = Model.Expression.Insert(txtExpression.CaretIndex, variable);
            txtExpression.CaretIndex += Model.Expression.Length;
            txtExpression.SelectionLength = 0;
        }

        private static bool IsAlphaNumeric(string strToCheck)
        {
            return new Regex("[^a-zA-Z0-9]").IsMatch(strToCheck) != true;
        }

        private void AddFunction(object sender, RoutedEventArgs e)
        {
            var function = cmbFunctions.SelectedValue as string;
            if (IsAlphaNumeric(function))
                function = $"{function}(";
            Model.Expression = Model.Expression.Insert(txtExpression.CaretIndex, function);
            txtExpression.CaretIndex += Model.Expression.Length;
            txtExpression.SelectionLength = 0;
        }

        private void BrowseVariable(object sender, RoutedEventArgs e)
        {
            if (_entityReference.Edit(sync: true, localserver: true, useChildSelector: false))
            {
                if (_entityReference.HasValidValue)
                {
                    Dispatcher.CurrentDispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                    {
                        var newValue = new OPCUAEntityReference(_entityReference);
                        txtVariables.Tag = newValue;
                        txtVariables.Text = _entityReference.ToExpressionString();
                    });
                }
            }

            //var dialog = new GeneralDialogContent(_varEditor)
            //{
            //    Owner = this.FindParent<Window>(),
            //    Title = WPFUtilities.Properties.Resources.ExpressionEditor,
            //    DialogKeepContent = true,
            //    HelpLink = "BrowseVariable"
            //};
            //if (dialog.ShowDialog() == false || _varEditor.DataContext == null)
            //    return;
            //var text = String.Format("{0}", _varEditor.DataContext);
            //// var text = VarEditor.DataContext.ToString();
            //txtVariables.Text = text;
        }
    }
}
