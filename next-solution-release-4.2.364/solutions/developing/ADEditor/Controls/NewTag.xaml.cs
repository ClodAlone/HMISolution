using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ADEditor.Document;

namespace ADEditor.Controls
{
    /// <summary>
    /// Interaction logic for NewTag.xaml
    /// </summary>
    public partial class NewTag : UserControl
    {
        #region Declarations
        readonly ADEditorDocument Document;
        #endregion

        public NewTag(ADEditorDocument doc)
        {
            InitializeComponent();

            Document = doc;


            comboModelType.ItemsSource = Enum.GetValues(typeof(UFUAModel.ModelType));
            comboDataType.ItemsSource = Enum.GetValues(typeof(UFUAModel.DataType));

        }

        private void comboModelType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var tag = DataContext as UFUAModel.UFUATag;
            if (tag.ModelType == UFUAModel.ModelType.Variable || tag.ModelType == UFUAModel.ModelType.Analog)
            {
                comboDataType.Visibility = Visibility.Visible;
                textDataType.Visibility = Visibility.Visible;
            }
            else if (tag.ModelType == UFUAModel.ModelType.Method)
            {
                comboDataType.Visibility = Visibility.Collapsed;
                textDataType.Visibility = Visibility.Collapsed;

            }
            else
            {
                comboDataType.Visibility = Visibility.Collapsed;
                textDataType.Visibility = Visibility.Collapsed;

            }
        }

    }
}
