using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UFInterfaces;

namespace DataReaderEditor.PropertyDataTemplate
{
    /// <summary>
    /// Interaction logic for ConnectionStringPropertyEditor.xaml
    /// </summary>
    public partial class DataReaderModelPropertyEditor : UserControl
    {
        #region Workspace

        public static readonly DependencyProperty WorkspaceProperty = DependencyProperty.Register("Workspace", typeof(IWorkspace), typeof(DataReaderModelPropertyEditor), new UIPropertyMetadata(null));

        public IWorkspace Workspace
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (IWorkspace)GetValue(WorkspaceProperty);
            }
            set
            {
                SetValue(WorkspaceProperty, value);
            }
        }

        #endregion

        public DataReaderModelPropertyEditor()
        {
            InitializeComponent();
        }

        private void DlgButton_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            Button button = (Button)sender;

            var dataReaderModel = text.Tag as DataReader.DataReaderModel;
            if (dataReaderModel == null)
                dataReaderModel = new DataReader.DataReaderModel();
            var doc = Workspace?.ContextDocument;
            var viewModel = new DataReaderModelView(dataReaderModel, doc?.rootBase);
            viewModel.EditConnection.Execute(null);
            text.Tag = new DataReader.DataReaderModel(dataReaderModel);
            BindingOperations.GetMultiBindingExpression(text, TextBox.TextProperty).UpdateTarget();
        }

        private void DlgButton_Clear(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            Button button = (Button)sender;
            text.Tag = null;
            BindingOperations.GetMultiBindingExpression(text, TextBox.TextProperty).UpdateTarget();
        }
    }
}
