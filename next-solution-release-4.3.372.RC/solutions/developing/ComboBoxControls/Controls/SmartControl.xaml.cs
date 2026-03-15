using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using DevExpress.Xpf.Grid;
using DocumentManager.ComponentService;
using OPCUAViewModel;
using StringManager.ComponentService;
using UFInterfaces;
using Utilities;
using Utilities.WPF;

namespace ComboBoxControls.Controls
{
    /// <summary>
    /// Interaction logic for SmartControl.xaml
    /// </summary>
    public partial class SmartControl : UserControl
    {
        #region Declarations
        readonly OptionButtonControl control;
        OptionItemList datalist;
        IDocument document;
        Window parentWindow;

        bool bLoaded;

        IWorkspace workspace;
        public IWorkspace Workspace
        {
            get
            {
                if (workspace == null && document != null)
                    workspace = document.GetService(typeof(IWorkspace)) as IWorkspace;
                
                return workspace;
            }
        }
        #endregion

        #region Constructors
        public SmartControl(OptionButtonControl c)
        {
            InitializeComponent();

            control = c;
            DataContext = c;

            Loaded += (o, e) =>
            {
                if (bLoaded)
                    return;
                bLoaded = true;

                if (document == null)
                    document = ScreenSettings.ScreenDocument.GetScreenDocument(control);

                parentWindow = this.FindParent<Window>();
                if (parentWindow != null)
                {
                    parentWindow.Closing += (s, ev) =>
                    {
                        ApplyChanges();
                    };
                }

                datalist = new OptionItemList(control.OptionList);
                gridControl.ItemsSource = datalist;
            };
        }
        #endregion

        #region Methods

        private void OnMoveUp(object sender, RoutedEventArgs e)
        {
            MoveUpRow(tableView.FocusedRowHandle);
            gridControl.RefreshRow(tableView.FocusedRowHandle);
            ApplyChanges();
        }

        private void MoveUpRow(int rowHandle)
        {
            int listIndex = gridControl.GetListIndexByRowHandle(rowHandle);
            if (listIndex <= 0 || listIndex >= datalist.Count)
                return;

            datalist.Move(listIndex, listIndex - 1);

            listIndex = listIndex - 1;
            moveDown.IsEnabled = datalist != null && listIndex >= 0 && listIndex < datalist.Count - 1;
            moveUp.IsEnabled = datalist != null && listIndex > 0;
        }

        private void OnMoveDown(object sender, RoutedEventArgs e)
        {
            MoveDownRow(tableView.FocusedRowHandle);
            gridControl.RefreshRow(tableView.FocusedRowHandle);
            ApplyChanges();
        }

        private void MoveDownRow(int rowHandle)
        {
            int listIndex = gridControl.GetListIndexByRowHandle(rowHandle);
            if (listIndex < 0 || listIndex >= datalist.Count)
                return;

            datalist.Move(listIndex, listIndex + 1);

            listIndex = listIndex + 1;
            moveDown.IsEnabled = datalist != null && listIndex >= 0 && listIndex < datalist.Count - 1;
            moveUp.IsEnabled = datalist != null && listIndex > 0;
        }

        private void OnAdd(object sender, RoutedEventArgs e)
        {
            int _value = 0;
            string optioncontent = string.Format("{0} {1}",Properties.Resources.OptionDefTitle,_value);

            while ((from c in datalist where c.UntranslatedOptionContent == optioncontent select c).ToList().FirstOrDefault() != null)
            {
                _value++;
	            optioncontent = string.Format("{0} {1}",Properties.Resources.OptionDefTitle,_value);
	        }

            var newData = new OptionItem { UntranslatedOptionContent = optioncontent, OptionValue = _value.ToString() };

            InsertRow(tableView.FocusedRowHandle, newData);
            ApplyChanges();
        }

        private void textChanged(object sender, TextChangedEventArgs e)
        {
            ApplyChanges();
        }

        private void OnDelete(object sender, RoutedEventArgs e)
        {
            DeleteRow(tableView.FocusedRowHandle);
        }

        private void DeleteRow(int rowHandle)
        {
            if (datalist.Count < 1)
                return;

            if (tableView.IsEditing)
                return;

            int listIndex = gridControl.GetListIndexByRowHandle(rowHandle);
            if (listIndex >= 0)
                datalist.RemoveAt(listIndex);

            ApplyChanges();
        }

        private void InsertRow(int rowHandle, OptionItem data)
        {
            //int listIndex = gridControl.GetListIndexByRowHandle(rowHandle);
            //if (listIndex < 0 || listIndex >= datalist.Count) listIndex = -1;

            //listIndex = -1;
            int listIndex = datalist.Count - 1;
            datalist.Insert(listIndex + 1, data);
        }

        private void tableView_FocusedRowChanged(object sender, DevExpress.Xpf.Grid.FocusedRowChangedEventArgs e)
        {
            int listIndex = gridControl.GetListIndexByRowHandle(tableView.FocusedRowHandle);
            moveDown.IsEnabled = datalist != null && listIndex >= 0 && listIndex < datalist.Count - 1;
            moveUp.IsEnabled = datalist != null && listIndex > 0;
            btnDelete.IsEnabled = listIndex >= 0;
            ApplyChanges();
        }
        
        private void tableView_ValidateRow(object sender, DevExpress.Xpf.Grid.GridRowValidationEventArgs e)
        {
            var _value = ((OptionItem)e.Row).OptionValue;
            e.IsValid = true; // _value != Double.NaN;
            ApplyChanges();
        }

        private void tableView_InvalidRowException(object sender, DevExpress.Xpf.Grid.InvalidRowExceptionEventArgs e)
        {
            e.ExceptionMode = ExceptionMode.NoAction;
        }

        private void GridColumn_Validate(object sender, GridCellValidationEventArgs e)
        {
            bool _thvalue = (bool)((DataRowView)e.Row)["OptionValue"];
            if (_thvalue)
            {
                try
                {
                    //double _value = Convert.ToDouble(e.Value);
                }
                catch
                {
                    e.IsValid = false;
                    e.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Critical;
                    e.ErrorContent = string.Format(Properties.Resources.ThValueError);
                }
            }
        }
     
        private void tableView_CellValueChanged_1(object sender, CellValueChangedEventArgs e)
        {
            ApplyChanges();
        }

        void ApplyChanges()
        {
            if (parentWindow == null || parentWindow.DialogResult == true)
            {
                control.OptionList = datalist;
            }
        }
        #endregion
    }
}
