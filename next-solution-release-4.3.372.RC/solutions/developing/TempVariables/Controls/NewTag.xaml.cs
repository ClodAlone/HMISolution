using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using DevExpress.Xpo;
using Opc.Ua;
using SmartTagsControl.ComponentService;
using TempVariablesManager.Document;
using UFInterfaces.Editors;
using UFUAEditor.ComponentService;
using Utilities;
using Utilities.WPF;
using TempVariablesModel;

namespace TempVariablesManager.Controls
{
    /// <summary>
    /// Interaction logic for NewTag.xaml
    /// </summary>
    public partial class NewTag : UserControl
    {
        #region Declarations
        readonly TempVariablesPersistence Document;

        Variable tag = null;
        bool cancel = false;
        string oldDynSettings = null;
        #endregion

        public NewTag(TempVariablesPersistence doc)
        {
            InitializeComponent();
            Document = doc;

            //comboModelType.ItemsSource = Enum.GetValues(typeof(UFUAModel.ModelType));
            comboDataType.ItemsSource = Enum.GetValues(typeof(UFUAModel.DataType));

            //textArrayDimension.NumberFormatInfo = new System.Globalization.NumberFormatInfo() { NumberDecimalDigits = 0 };

            Loaded += (p, q) =>
            {
                tag = DataContext as Variable;
            };
        }        

        private void comboModelType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            e.Handled = true;
            var tag = DataContext as Variable;
            
            //if (tag.ModelType == ModelType.Variable ||
            //    tag.ModelType == ModelType.Analog)
            //{
            //    textEnumStrings.Visibility = Visibility.Collapsed;
            //    //textEditEnumStrings.Visibility = Visibility.Collapsed;
            //    dockEnumStrings.Visibility = Visibility.Collapsed;

            //comboDataType.Visibility = Visibility.Visible;
            //textDataType.Visibility = Visibility.Visible;

            //    textBlockArrayDimension.Visibility = Visibility.Visible;
            //    checkBoxRetentive.Visibility = Visibility.Visible;
            //    textBlockRetentive.Visibility = Visibility.Visible;
            //    textArrayDimension.Visibility = Visibility.Visible;

            //    textBlockInitialValue.Visibility = Visibility.Visible;
            //    textInitialValue.Visibility = Visibility.Visible;

            //    checkBoxRetentive.Visibility = Visibility.Visible;
            //    textBlockRetentive.Visibility = Visibility.Visible;

            //}
            //else if (tag.ModelType == ModelType.Digital)
            //{
            //    textEnumStrings.Visibility = Visibility.Visible;
            //    //textEditEnumStrings.Visibility = Visibility.Visible;
            //    dockEnumStrings.Visibility = Visibility.Visible;

            //    comboDataType.Visibility = Visibility.Collapsed;
            //    textDataType.Visibility = Visibility.Collapsed;

            //    textBlockArrayDimension.Visibility = Visibility.Visible;
            //    checkBoxRetentive.Visibility = Visibility.Visible;
            //    textBlockRetentive.Visibility = Visibility.Visible;
            //    textArrayDimension.Visibility = Visibility.Visible;

            //    textBlockInitialValue.Visibility = Visibility.Visible;
            //    textInitialValue.Visibility = Visibility.Visible;

            //    checkBoxRetentive.Visibility = Visibility.Visible;
            //    textBlockRetentive.Visibility = Visibility.Visible;

            //}
            //else if (tag.ModelType == ModelType.Enumerated)
            //{
            //    textEnumStrings.Visibility = Visibility.Visible;
            //    //textEditEnumStrings.Visibility = Visibility.Visible;
            //    dockEnumStrings.Visibility = Visibility.Visible;

            //    textBlockArrayDimension.Visibility = Visibility.Visible;
            //    checkBoxRetentive.Visibility = Visibility.Visible;
            //    textBlockRetentive.Visibility = Visibility.Visible;
            //    textArrayDimension.Visibility = Visibility.Visible;

            //    comboDataType.Visibility = Visibility.Collapsed;
            //    textDataType.Visibility = Visibility.Collapsed;

            //    textBlockInitialValue.Visibility = Visibility.Visible;
            //    textInitialValue.Visibility = Visibility.Visible;
            //}
            //else
            //{
            //    textEnumStrings.Visibility = Visibility.Collapsed;
            //    //textEditEnumStrings.Visibility = Visibility.Collapsed;
            //    dockEnumStrings.Visibility = Visibility.Collapsed;

            //textBlockArrayDimension.Visibility = Visibility.Collapsed;
            //checkBoxRetentive.Visibility = Visibility.Collapsed;
            //textBlockRetentive.Visibility = Visibility.Collapsed;
            //textArrayDimension.Visibility = Visibility.Collapsed;

            //    comboDataType.Visibility = Visibility.Collapsed;
            //    textDataType.Visibility = Visibility.Collapsed;

            //    textBlockInitialValue.Visibility = Visibility.Collapsed;
            //    textInitialValue.Visibility = Visibility.Collapsed;
            //}

        }

        private void comboDataType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            e.Handled = true;
            var tag = DataContext as Variable;
        }
        
    }
}
