using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using DevExpress.Xpf.Grid;
using OPCUAViewModel;
using OPCUAViewModelService.ComponentService;
using ViewModelLib;
using Utilities;
using DocumentManager.ComponentService;
using UFUAEditor.ComponentService;
using DevExpress.Xpf.Editors;
using System.ComponentModel;
using System.Collections.Generic;

namespace ReportParameters.UserControls
{
    #region Helper Classes

    public class CellTemplateSelector : DataTemplateSelector
    {
        public DataTemplate TemplateBoolean { get; set; }
        public DataTemplate TemplateInteger { get; set; }
        public DataTemplate TemplateDecimal { get; set; }
        public DataTemplate TemplateString { get; set; }
        public DataTemplate TemplateDateTime { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var cellData = item as EditGridCellData;
            var row = cellData.RowData.Row as ReportParameters.Parameter;
            if (row.Type == ReportParameters.ParameterType.Boolean)
                return TemplateBoolean;
            else if (row.Type == ReportParameters.ParameterType.DateTime)
                return TemplateDateTime;
            else if (row.Type == ReportParameters.ParameterType.Integer)
                return TemplateInteger;
            else if (row.Type == ReportParameters.ParameterType.Decimal)
                return TemplateDecimal;
            else
                return TemplateString;
        }
    }

    #endregion

    /// <summary>
    /// Interaction logic for EditReportCallParameters.xaml
    /// </summary>
    public partial class EditReportCallParameters : UserControl
    {
        #region Declarations

        bool bLoaded;
        //readonly ExpandoObject parameters;
        ReportParameters.ParameterCollection parameters;

        #endregion

        #region Constructors

        public EditReportCallParameters()
        {
            InitializeComponent();

            if (DataContext is ReportParameters.ParameterCollection)
                parameters = DataContext as ReportParameters.ParameterCollection;
            
            gridControl.ItemsSource = parameters;

            DataContextChanged += (o, e) =>
            {
                if (e.NewValue is ReportParameters.ParameterCollection)
                {
                    parameters = e.NewValue as ReportParameters.ParameterCollection;
                    gridControl.ItemsSource = null;
                    gridControl.ItemsSource = parameters;
                }
            };

            Loaded += (ob, ev) =>
            {
                if (bLoaded)
                    return;
                bLoaded = true;
            };
        }

        #endregion

        #region Commands
        RelayCommand addNew;
        public ICommand AddNew
        {
            get
            {
                if (addNew == null)
                {
                    addNew = new RelayCommand(
                        param =>
                        {
                            if (parameters == null)
                                parameters = new ReportParameters.ParameterCollection();
                            parameters.AddNewDefault();
                            gridControl.ItemsSource = null;
                            gridControl.ItemsSource = parameters;
                        });
                }
                return addNew;
            }
        }

        RelayCommand removeSelected;
        public ICommand RemoveSelected
        {
            get
            {
                if (removeSelected == null)
                {
                    removeSelected = new RelayCommand(
                        param =>
                        {
                            if (gridControl.SelectedItem != null && gridControl.SelectedItem is ReportParameters.Parameter)
                            {
                                var selected = gridControl.SelectedItem as ReportParameters.Parameter;
                                parameters.Remove(selected);
                                gridControl.ItemsSource = null;
                                gridControl.ItemsSource = parameters;
                            }
                        },
                        param => parameters != null && parameters.Count > 0 && gridControl.SelectedItem != null
                        );
                }
                return removeSelected;
            }
        }

        RelayCommand removeAll;
        public ICommand RemoveAll
        {
            get
            {
                if (removeAll == null)
                {
                    removeAll = new RelayCommand(
                        param =>
                        {
                            parameters.Clear();
                            gridControl.ItemsSource = null;
                            gridControl.ItemsSource = parameters;
                        },
                        param => parameters != null && parameters.Count > 0
                        );
                }
                return removeAll;
            }
        }
        #endregion
        #region Methods
        private void ButtonEditSettings_DefaultButtonClick(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            var button = (Button)sender;
            var selected = gridControl.SelectedItem as ReportParameters.Parameter;
            if (selected == null)
                return;

            Dispatcher.BeginInvokeAsynchronously(() => 
            {
                OPCUAEntityReference value = selected.TagRef;
                if (value == null)
                    value = new OPCUAEntityReference(null);
                if (OPCUAViewModelComponent.ufuaEditorServiceAvailable &&
                    OPCUAViewModelComponent.workspaceServiceAvailable)
                {
                    value.Editor = OPCUAViewModelComponent.ufuaEditorService;
                    value.Document = OPCUAViewModelComponent.workspaceService.ContextDocument;
                }

                if (value.Edit())
                {
                    Dispatcher.CurrentDispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                    {
                        selected.TagRef = value;
                        gridControl.SelectedItem = null;
                    });
                }
            });
        }
        private void DlgButton_Click_Clear(object sender, RoutedEventArgs e)
        {
            Parameter p = ((Button)sender).Tag as Parameter;
            p.TagRef = null;
            p.StringRef = string.Empty;
        }

        OPCUAEntityReference original;
        private async void uriLabel_LostFocus(object sender, RoutedEventArgs e)
        {
            ComboBoxEdit combo = sender as ComboBoxEdit;
            if (combo == null || combo.Text == null || !bEditing || !OPCUAViewModelComponent.workspaceServiceAvailable)
            {
                bEditing = false;
                return;
            }
            bEditing = false;
            IDocument doc = OPCUAViewModelComponent.workspaceService.ContextDocument as IDocument;
            if (doc == null)
                return;

            var editor = doc.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
            if (editor == null)
                return;

            var split = combo.Text.Split(':');
            var instance = split[0];
            var name = split[0];
            if (split.Length > 1)
                name = split[1];
            else
                instance = null;

            if (original == null)
                original = combo.Tag as OPCUAEntityReference;
            var xml = editor.GetTagEntityReference(doc, name, instance);
            if (String.IsNullOrEmpty(xml))
            {
                if (original == null)
                {
                    original = new OPCUAEntityReference(null);
                    original.HumanReadable = original.RelativePath = original.ReadablePath = combo.Text;
                }
                else
                    original.HumanReadable = original.RelativePath = original.ReadablePath = combo.Text;

                original.ResolvedNodeId = null;
                combo.Tag = original;

                var oldColor = combo.Foreground;
                combo.Foreground = Brushes.Red;
                await Task.Delay(TimeSpan.FromMilliseconds(100));
                combo.Foreground = oldColor;
                await Task.Delay(TimeSpan.FromMilliseconds(100));
                combo.Foreground = Brushes.Red;
                await Task.Delay(TimeSpan.FromMilliseconds(100));
                combo.Foreground = oldColor;
            }
            else
            {
                var tag = xml.FromXml<OPCUAEntityReference>();
                combo.Tag = tag;
                var s = tag.StringRepresentation;
                if (!String.IsNullOrEmpty(s))
                    combo.Text = s;

                if (original != null)
                    original.UpdateValue(tag);
            }
        }

        bool bEditing;
        private void uriLabel_KeyDown(object sender, KeyEventArgs e)
        {
            bEditing = true;
            FillComboBox(sender as ComboBoxEdit, e.Key == Key.F5);
        }

        private void uriLabel_PopupOpening(object sender, OpenPopupEventArgs e)
        {
            FillComboBox(sender as ComboBoxEdit);
        }

        private void uriLabel_GotFocus(object sender, RoutedEventArgs e)
        {
            (sender as ComboBoxEdit)?.SelectAll();
        }

        private void uriLabel_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            bEditing = true;
        }

        List<ComboBoxEdit> filledCombos = new List<ComboBoxEdit>();
        void FillComboBox(ComboBoxEdit sender, bool bForceRefresh = false)
        {
            if (sender == null || (filledCombos.Contains(sender) && !bForceRefresh) || !OPCUAViewModelComponent.workspaceServiceAvailable)
                return;
            IDocument doc = OPCUAViewModelComponent.workspaceService.ContextDocument as IDocument;
            if (doc == null)
                return;
            var editor = doc.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
            if (editor == null)
                return;

            if (!filledCombos.Contains(sender))
                filledCombos.Add(sender);
            var selected = gridControl.SelectedItem as Parameter;
            selected.Loading = true;
            var task = Task.Factory.StartNew(() =>
            {
                return editor.GetFlatFullTagNameCollectionOrderByName(doc, false, bForceRefresh);
            });
            task.ContinueWith(ret =>
            {
                sender.ItemsSource = ret.Result;
                selected.Loading = false;
                sender.IsPopupOpen = true;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
        #endregion
    }
}
