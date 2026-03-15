using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Utilities;
using Utilities.WPF;

namespace ReportParameters.PropertyDataTemplate
{
    /// <summary>
    /// Interaction logic for ReportParametersPropertyEditor.xaml
    /// </summary>
    public partial class ReportParametersPropertyEditor : UserControl
    {
        public ReportParametersPropertyEditor()
        {
            InitializeComponent();
        }

        private void DlgButton_Click_Set(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            var parameters = new ReportParameters.ParameterCollection();
            var context = button.Tag as ReportParameters.ParameterCollection;
            if (context != null)
                parameters.AddRange(context);
//#if DEBUG
//            if (parameters.Count == 0)
//            {
//                parameters.Add(new ReportParameters.Parameter() { Name = "Parameter1", Type = ReportParameters.ParameterType.DateTime, Value = DateTime.Now });
//                parameters.Add(new ReportParameters.Parameter() { Name = "Parameter2", Type = ReportParameters.ParameterType.Boolean, Value = true });
//                parameters.Add(new ReportParameters.Parameter() { Name = "Parameter3", Type = ReportParameters.ParameterType.Numeric, Value = (double)10.5 });
//                parameters.Add(new ReportParameters.Parameter() { Name = "Parameter4", Type = ReportParameters.ParameterType.String, Value = "ABC" });
//            }
//#endif
            var userControl = new UserControls.EditReportCallParameters();
            userControl.DataContext = parameters;

            GeneralDialogContent Dialog = new GeneralDialogContent(userControl)
            {
                Owner = this.FindParent<Window>(),
                HelpLink = "ReportParametersPropertyEditor"
            };
            if (Dialog.ShowDialog() == true)
            {
                button.Tag = parameters;
                counterLabel.GetBindingExpression(Label.ContentProperty).UpdateTarget();
            }
        }

        private void DlgButton_Click_Clear(object sender, RoutedEventArgs e)
        {
            setCommand.Tag = new ReportParameters.ParameterCollection();
            counterLabel.GetBindingExpression(Label.ContentProperty).UpdateTarget();
        }
    }
}
