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
using Utilities;
using Utilities.WPF;
using DataReader;
using ScreenSettings.Entities;
using ScreenManager.ComponentService;
using UIMsgBoxAlertService.ComponentService;

namespace ScreenManager.Popups
{
    /// <summary>
    /// Interaction logic for ItemControlSourceProperties.xaml
    /// </summary>
    public partial class ItemControlSourceProperties : UserControl
    {
        readonly ScreenManagerComponent EditorComponent;
        readonly new DocumentManager.ComponentService.IDocument Parent;
        readonly IUIMsgBoxAlertService ui;

        public ItemControlSourceProperties(ScreenManagerComponent c, DocumentManager.ComponentService.IDocument parent)
        {
            InitializeComponent();
            EditorComponent = c;
            Parent = parent;
            ui = Parent.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
        }

        private void DataEditor_Click(object sender, RoutedEventArgs e)
        {
            var entity = DataContext as ScreenEntity;
            var dataReaderModel = new DataReaderModel(entity.ReaderItemSources);
            if (dataReaderModel == null)
                dataReaderModel = new DataReaderModel();
            var dataReaderEditor = new DataReaderEditor.DataReaderEditor(dataReaderModel, Parent?.rootBase, false);
            var Dialog = new GeneralDialogContent(dataReaderEditor)
            {
                Owner = this.FindParent<Window>(),
                Title = Properties.Resources.DataSourceSelector,
                HelpLink = "DataReaderEditor"
            };
            Dialog.Closing += (o, ev) =>
            {
                if (Dialog.DialogResult == true)
                {
                    if (String.IsNullOrEmpty(dataReaderModel.Select))
                    {
                        if (ui != null)
                            ui.ShowWarning(Properties.Resources.InvalidDataSource);
                        else
                            MessageBox.Show(Properties.Resources.InvalidDataSource,
                                Properties.Resources.PropertiesEditor, MessageBoxButton.OK, MessageBoxImage.Warning);
                        ev.Cancel = true;
                    }
                }
            };
            if (Dialog.ShowDialog() != true)
            {
                return;
            }
            if (!entity.ReaderItemSources.Equals(dataReaderModel))
                entity.ReaderItemSources = new DataReaderModel(dataReaderModel);
        }

        private void ClearDataSource_Click(object sender, RoutedEventArgs e)
        {
            var entity = DataContext as ScreenEntity;
            entity.ReaderItemSources = null;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var stringEditor = EditorComponent.StringEditor.GetStringEditor(Parent);
            stringEditor.DataContext = TextEditor.Text;

            var Dialog = new GeneralDialogContent(stringEditor)
            {
                Owner = this.FindParent<Window>(),
                DialogKeepContent = true,
                Title = Properties.Resources.SelectStringEditor,
                HelpLink = "StringEditor"
            };
            if (Dialog.ShowDialog() != true)
                return;

            TextEditor.Text = stringEditor.DataContext as String;
            TextEditor.Focus();
            TextEditor.SelectAll();
        }
    }
}
