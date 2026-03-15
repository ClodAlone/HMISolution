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
using UFRecipeSettings;

namespace UFRecipeEditor.PropertyDataTemplate
{
    /// <summary>
    /// Interaction logic for DataConnectionPropertyEditor.xaml
    /// </summary>
    public partial class ConnectionSourcePropertyEditor : UserControl
    {
        public ConnectionSourcePropertyEditor()
        {
            InitializeComponent();
        }

        private void DlgButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            ConnectionSourceReference value = (ConnectionSourceReference)(button.Tag);

            if (value != null)
            {
                var model = new DataReader.DataReaderModel()
                {
                    DataProvider = value.RecipeDocument.RecipeEntity.DataProvider,
                    DataProviderDescription = value.RecipeDocument.RecipeEntity.DataProviderDescription,
                    DataProviderShortDisplayName = value.RecipeDocument.RecipeEntity.DataProviderShortDisplayName,
                    DataProviderDisplayName = value.RecipeDocument.RecipeEntity.DataProviderDisplayName,
                    DataSourceName = value.RecipeDocument.RecipeEntity.DataSourceName,
                    DataSourceDisplayName = value.RecipeDocument.RecipeEntity.DataSourceDisplayName,
                    Connection = value.RecipeDocument.RecipeEntity.ConnectionString
                };

                var viewModel = new DataReaderEditor.DataReaderModelView(model, value.RecipeDocument.rootBase);
                if (viewModel.EditConnection.CanExecute(null))
                {
                    viewModel.EditConnection.Execute(null);

                    value.RecipeDocument.RecipeEntity.DataProvider = model.DataProvider;
                    value.RecipeDocument.RecipeEntity.DataProviderDescription = model.DataProviderDescription;
                    value.RecipeDocument.RecipeEntity.DataProviderShortDisplayName = model.DataProviderShortDisplayName;
                    value.RecipeDocument.RecipeEntity.DataProviderDisplayName = model.DataProviderDisplayName;
                    value.RecipeDocument.RecipeEntity.DataSourceName = model.DataSourceName;
                    value.RecipeDocument.RecipeEntity.DataSourceDisplayName = model.DataSourceDisplayName;
                    value.RecipeDocument.RecipeEntity.ConnectionString = model.Connection;
                }
            }

            BindingOperations.GetMultiBindingExpression(textEditConnection, TextBox.TextProperty).UpdateTarget();
        }

        private void DlgButton_Clear(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            button.Tag = String.Empty;
            BindingOperations.GetMultiBindingExpression(textEditConnection, TextBox.TextProperty).UpdateTarget();
        }
    }
}
