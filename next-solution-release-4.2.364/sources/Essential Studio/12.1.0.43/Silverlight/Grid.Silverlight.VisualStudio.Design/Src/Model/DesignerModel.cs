#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows;
using Microsoft.Windows.Design.Model;
using Syncfusion.Windows.Controls.Grid;
//using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Collections;
using Syncfusion.Windows.Data;
using System.Collections.Specialized;
using System.Windows.Data;
using Syncfusion.Linq;
namespace Syncfusion.Grid.WPF.VisualStudio.Design
{
    public class DesignerModel : INotifyPropertyChanged
    {

        private ModelItem selectedControl;

        public bool AutoPopulateColumns
        {
            get
            {
                return (bool)selectedControl.Properties["AutoPopulateColumns"].ComputedValue;
            }
            set
            {
                selectedControl.Properties["AutoPopulateColumns"].SetValue(value);
                RaisePropertyChanged("AutoPopulateColumns");
            }
        }

        public bool AutoPopulateRelations
        {
            get
            {
                return (bool)selectedControl.Properties["AutoPopulateRelations"].ComputedValue;
            }
            set
            {
                selectedControl.Properties["AutoPopulateRelations"].SetValue(value);
                RaisePropertyChanged("AutoPopulateRelations");
            }
        }

        public bool AllowSort
        {
            get
            {
                return (bool)selectedControl.Properties["AllowSort"].ComputedValue;
            }
            set
            {
                selectedControl.Properties["AllowSort"].SetValue(value);
                RaisePropertyChanged("AllowSort");
            }
        }

        public bool AllowDragColumns
        {
            get
            {
                return (bool)selectedControl.Properties["AllowDragColumns"].ComputedValue;
            }
            set
            {
                selectedControl.Properties["AllowDragColumns"].SetValue(value);
                RaisePropertyChanged("AllowDragColumns");
            }
        }

        public bool AllowResizeColumns
        {
            get
            {
                return (bool)selectedControl.Properties["AllowResizeColumns"].ComputedValue;
            }
            set
            {
                selectedControl.Properties["AllowResizeColumns"].SetValue(value);
                RaisePropertyChanged("AllowResizeColumns");
            }
        }

        public bool ShowColumnOptions
        {
            get;
            set;
        }

        public bool ShowFilters
        {
            get
            {
                return (bool)selectedControl.Properties["ShowFilters"].ComputedValue;
            }
            set
            {
                selectedControl.Properties["ShowFilters"].SetValue(value);
                RaisePropertyChanged("ShowFilters");
            }
        }

        public bool ShowGroupDropArea
        {
            get
            {
                return (bool)selectedControl.Properties["ShowGroupDropArea"].ComputedValue;
            }
            set
            {
                selectedControl.Properties["ShowGroupDropArea"].SetValue(value);
                RaisePropertyChanged("ShowGroupDropArea");
            }
        }

        public bool ShowAddNewRow
        {
            get
            {
                return (bool)selectedControl.Properties["ShowAddNewRow"].ComputedValue;
            }
            set
            {
                selectedControl.Properties["ShowAddNewRow"].SetValue(value);
                RaisePropertyChanged("ShowAddNewRow");
            }
        }

        public bool ShowRowHeader
        {
            get
            {
                return (bool)selectedControl.Properties["ShowRowHeader"].ComputedValue;
            }
            set
            {
                selectedControl.Properties["ShowRowHeader"].SetValue(value);
                RaisePropertyChanged("ShowRowHeader");
            }
        }

        public bool AllowResizeRows
        {
            get
            {
                return (bool)selectedControl.Properties["AllowResizeRows"].ComputedValue;
            }
            set
            {
                selectedControl.Properties["AllowResizeRows"].SetValue(value);
                RaisePropertyChanged("AllowResizeRows");
            }
        }

        public bool AllowEdit
        {
            get;
            set;
        }

        public bool ShowErrorTooltips
        {
            get;
            set;
        }

        public bool ShowTooltips
        {
            get;
            set;
        }

        public bool AllowDelete
        {
            get;
            set;
        }

      
        private bool _generateVisibleColumns = false;

        public bool GenerateVisibleColumns
        {
            get
            {
                return _generateVisibleColumns;
            }

            set
            {
                _generateVisibleColumns = value;

                if (GenerateColumnsText == "Generate Columns")
                {
                    GenerateColumns();
                    GenerateColumnsText = "Clear Columns";
                    this.VisibleColumnsListBox = Visibility.Visible;
                }
                else
                {
                    ClearColumns();
                    GenerateColumnsText = "Generate Columns";
                    this.VisibleColumnsListBox = Visibility.Collapsed;
                    this.InfoText = "Click the Button to generate Visible Columns";
                }
            }
        }

        private void OnGenerateVisibleColumnChanged(bool value)
        {
            if (GenerateColumnsText == "Generate Columns")
            {
                GenerateColumns();
                GenerateColumnsText = "Clear Columns";
                this.AutoPopulateColumns = false;
                this.VisibleColumnsListBox = Visibility.Visible;
            }
            else
            {
                ClearColumns();
                GenerateColumnsText = "Generate Columns";
                this.AutoPopulateColumns = true;
                this.VisibleColumnsListBox = Visibility.Collapsed;
                this.InfoText = "Click the Button to generate Visible Columns";
            }
        }

        IEnumerable SourceList { get; set; }

        public virtual IEnumerable GetSourceList(object source)
        {
            IEnumerable result = null;
            if (source != null)
            {
#if !SILVERLIGHT
                if (source is CollectionViewSource)
                {
#else
                if (source is CollectionViewSource)
                {
#endif
                    var cvs = source as CollectionViewSource;
                    if (cvs.View != null)
                    {
                        result = GetSourceList(cvs.View.SourceCollection);
                    }
                }


                else if (source is ICollectionView)
                {
                    var sourceList = ((ICollectionView)source).SourceCollection;
                    result = GetSourceList(sourceList);
                }
#if !SILVERLIGHT
                else
                {
                    result = source as IEnumerable;
                }
#else
                else
                {
                    result = source as IEnumerable;
                }
#endif
            }

            return result;
        }

        public void GenerateColumns()
        {
            selectedControl.Properties["VisibleColumns"].Collection.Clear();
            var gridModel = selectedControl.Properties["Model"].ComputedValue as GridDataTableModel;
            gridModel.TableProperties.SuspendEvents();
           // gridModel.BeginInit();
            PropertyDescriptorCollection pdc = null;
            if (SourceList != null)
            {
                pdc = TypeDescriptor.GetProperties(SourceList.ToList<object>().First());
            }

            if (pdc == null)
            {
                return;
            }
            if (selectedControl.Properties["VisibleColumns"].Collection.Count == 0)
            {
                if (GeneratedVisibleColumns.Count != 0)
                {
                    for (int colum = 0; colum < GeneratedVisibleColumns.Count; colum++)
                    {
                        var vcModel = selectedControl.Properties["VisibleColumns"].Collection.Add(GeneratedVisibleColumns[colum]);
                        vcModel.Properties["Binding"].ClearValue();
                        vcModel.Properties["Width"].ClearValue();
                        var pd = pdc[((GridDataVisibleColumn)GeneratedVisibleColumns[colum]).MappingName];
                        var specialCellType = GetCellType(pd.PropertyType.Name);
                        VisibleColumnModel mod = new VisibleColumnModel(vcModel, specialCellType, pd.PropertyType.Name) { MappingNameType = ((GridDataVisibleColumn)GeneratedVisibleColumns[colum]).MappingName + "    { " + pd.PropertyType.Name + " }" };
                        this.VisibleColumnsModel.Add(mod);
                        gridModel.ColumnCount++;
                    }
                }
                else
                {
                    for (int colum = 0; colum < pdc.Count; colum++)
                    {
                        var pd = pdc[colum];
                        if (pd.GetType().IsClass)
                        {
                            if (!CheckType(pd.PropertyType.Name) && !pd.PropertyType.IsEnum)
                                continue;
                        }
                        GridDataVisibleColumn col = new GridDataVisibleColumn() { MappingName = pdc[colum].Name };
                        var vcModel = selectedControl.Properties["VisibleColumns"].Collection.Add(col);
                        var specialCellType = GetCellType(pd.PropertyType.Name);
                        VisibleColumnModel mod = new VisibleColumnModel(vcModel, specialCellType, pd.PropertyType.Name) { MappingNameType = col.MappingName + "    { " + pd.PropertyType.Name + " }" };
                        this.VisibleColumnsModel.Add(mod);
                    }
                }
            }
            else
            {

                for (int colum = 0;colum < GeneratedVisibleColumns.Count;colum++)
                {
                    var vcModel = selectedControl.Properties["VisibleColumns"].Collection[colum];
                    vcModel.Properties["Binding"].ClearValue();
                    vcModel.Properties["Width"].ClearValue();
                    var pd = pdc[((GridDataVisibleColumn)GeneratedVisibleColumns[colum]).MappingName];
                    var specialCellType = GetCellType(pd.PropertyType.Name);
                    VisibleColumnModel mod = new VisibleColumnModel(vcModel, specialCellType, pd.PropertyType.Name) { MappingNameType = ((GridDataVisibleColumn)GeneratedVisibleColumns[colum]).MappingName + "    { " + pd.PropertyType.Name + " }" };
                    this.VisibleColumnsModel.Add(mod);
                }
            }

            gridModel.TableProperties.ResumeEvents();
            this.InfoText = "Set properties for Visible Columns";
            this.VisibleColumnsListBox = Visibility.Visible;
            //this.AutoPopulateColumns = false;
            RaisePropertyChanged("VisibleColumnsModel");
        }

        private bool CheckType(string type)
        {
            switch (type)
            {
                case "String":
                case "Int16":
                case "Int32":
                case "Int64":
                case "Boolean":
                case "Double":
                case "Decimal":
                case "DateTime":
                    return true;
            }
            return false;
        }

      
        private void InvalidateDisplay(GridDataTableModel model)
        {
            if (model.Grid != null)
            {
                if (model.Grid.CurrentCell != null)
                {
                    model.Grid.CurrentCell.Deactivate();
                }

                model.Grid.RenderStyles.Clear();
                model.Grid.ArrangedCellUIElements.UnloadAll();
            }

            model.VolatileCellStyles.Clear();
            model.Data.Clear();
            model.InvalidateVisual(true);
        }

        public void ClearColumns()
        {
            this.VisibleColumnsModel.Clear();
            selectedControl.Properties["VisibleColumns"].Collection.Clear();
        }

        public IList AllowSelectionValues
        {
            get
            {
                return Enum.GetNames(typeof(GridSelectionFlagsView));
            }
        }

        public GridSelectionFlagsView AllowSelection
        {
            get
            {
                return (GridSelectionFlagsView)selectedControl.Properties["AllowSelection"].ComputedValue; ;
            }
            set
            {
                selectedControl.Properties["AllowSelection"].SetValue(value.ToString());
                RaisePropertyChanged("AllowSelection");
            }
        }

        public IList GridSelectionModeValues
        {
            get
            {
                return Enum.GetNames(typeof(GridSelectionModeView));
            }
        }

        public GridSelectionModeView ListBoxSelectionMode
        {
            get
            {
                return (GridSelectionModeView)selectedControl.Properties["ListBoxSelectionMode"].ComputedValue; ;
            }
            set
            {
                selectedControl.Properties["ListBoxSelectionMode"].SetValue(value.ToString());
                RaisePropertyChanged("ListBoxSelectionMode");
            }
        }

        public IList ActivateCurrentCellBehaviorValues
        {
            get
            {
                return Enum.GetNames(typeof(GridCellActivateActionView));
            }
        }

        public GridCellActivateActionView ActivateCurrentCellBehavior
        {
            get
            {
                return (GridCellActivateActionView)selectedControl.Properties["ActivateCurrentCellBehavior"].ComputedValue; ;
            }
            set
            {
                selectedControl.Properties["ActivateCurrentCellBehavior"].SetValue(value.ToString());
                RaisePropertyChanged("ActivateCurrentCellBehavior");
            }
        }
       
        public IList GridControlLengthUnitTypeValues
        {
            get
            {
                return Enum.GetNames(typeof(GridControlLengthUnitTypeView));
            }
        }

        public GridControlLengthUnitTypeView ColumnSizer
        {
            get
            {
                return (GridControlLengthUnitTypeView)selectedControl.Properties["ColumnSizer"].ComputedValue; 
            }
            set
            {
                selectedControl.Properties["ColumnSizer"].SetValue(value.ToString());
                RaisePropertyChanged("ColumnSizer");
            }
        }


        public VisualStyleView VisualStyle
        {
            get
            {

                return (VisualStyleView)selectedControl.Properties["VisualStyle"].ComputedValue;
            }
            set
            {
                selectedControl.Properties["VisualStyle"].SetValue(value.ToString());
                RaisePropertyChanged("VisualStyle");
            }
        }

       
        public IList VisibleColumns
        {
            get
            {
                var visibleColumn = selectedControl.Properties["VisibleColumns"].ComputedValue;

                if (visibleColumn != null)
                {
                    return visibleColumn as IList;

                    // return list as List<GridDataVisibleColumn>;
                }
                else
                {
                    return new List<object>();
                }
            }
            set
            {

                selectedControl.Properties["VisibleColumns"].SetValue(value);
                RaisePropertyChanged("VisibleColumns");
            }
        }

        public IList _visibleColumnsModel;

        public IList VisibleColumnsModel
        {
            get
            {
                return _visibleColumnsModel;
            }
            set
            {
                _visibleColumnsModel = value;
                RaisePropertyChanged("VisibleColumnsModel");
            }
        }

        public IList GeneratedVisibleColumns
        {
            get;
            set;
        }

        public IList coll
        {
            get;
            set;
        }

        private string _infoText;
        public string InfoText
        {
            get
            {
                return _infoText;
            }
            set
            {
                _infoText = value;
                RaisePropertyChanged("InfoText");
            }
        }

        private bool _generateColumnsEnabled = true;

        public bool GenerateColumnsEnabled
        {
            get
            {
                return _generateColumnsEnabled;
            }

            set
            {
                _generateColumnsEnabled = value;
                RaisePropertyChanged("GenerateColumnsEnabled");
            }
        }

        private string _generateColumnsText = "Generate Columns";

        public string GenerateColumnsText
        {
            get
            {
                return _generateColumnsText;
            }

            set
            {
                _generateColumnsText = value;
                RaisePropertyChanged("GenerateColumnsText");
            }
        }


        private Visibility _visibleColumnsListBox = Visibility.Collapsed;

        public Visibility VisibleColumnsListBox
        {
            get
            {
                return _visibleColumnsListBox;
            }

            set
            {
                _visibleColumnsListBox = value;
                RaisePropertyChanged("VisibleColumnsListBox");
            }
        }

        public DesignerModel(ModelItem selectedControl)
        {
            this.selectedControl = selectedControl;
            this.GeneratedVisibleColumns = new List<object>();
            for (int i = 0; i < VisibleColumns.Count; i++)
            {
                this.GeneratedVisibleColumns.Add(VisibleColumns[i]);
            }
            this.VisibleColumnsModel = new List<VisibleColumnModel>();

            if (selectedControl.Properties["VisibleColumns"].Collection.Count != 0)
            {
                var gridModel = selectedControl.Properties["Model"].ComputedValue as GridDataTableModel;
                var list = selectedControl.Properties["ItemsSource"].ComputedValue as IList;
                PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(list[0]);
                for (int colum = 0; colum < GeneratedVisibleColumns.Count; colum++)
                {
                    var vcModel = selectedControl.Properties["VisibleColumns"].Collection[colum];
                    vcModel.Properties["Binding"].ClearValue();
                    vcModel.Properties["Width"].ClearValue();
                    var pd = pdc[((GridDataVisibleColumn)GeneratedVisibleColumns[colum]).MappingName];
                    var specialCellType = GetCellType(pd.PropertyType.Name);
                    VisibleColumnModel mod = new VisibleColumnModel(vcModel, specialCellType, pd.PropertyType.Name) { MappingNameType = ((GridDataVisibleColumn)GeneratedVisibleColumns[colum]).MappingName + "    { " + pd.PropertyType.Name + " }" };
                    this.VisibleColumnsModel.Add(mod);
                }

                this.InfoText = "Set properties for Visible Columns";
                this.VisibleColumnsListBox = Visibility.Visible;
            }
            else
            {
                this.GenerateColumnsEnabled = true;
                this.InfoText = "Click the Button to generate Visible Columns";
            }

            var source = selectedControl.Properties["ItemsSource"].ComputedValue as object;


            if (source != null)
            {
                SourceList = GetSourceList(source);
                if (SourceList.ToList<object>().Count() == 0)
                {
                    this.GenerateColumnsEnabled = false;
                    this.InfoText = "ItemsSource should not be Empty";
                }
            }

            else
            {
                this.GenerateColumnsEnabled = false;
                this.InfoText = "Bind the ItemsSource property in the Grid";

            }
        }

        private CellTypes GetCellType(string ColumnType)
        {
            switch (ColumnType)
            {
                case "Boolean":
                    return CellTypes.CheckBox;

            }
            return CellTypes.TextBox;
        }

        public void ShowWindow()
        {
            Window1 window1 = new Window1();
            window1.RemoveColumn += new EventHandler<RemoveVisibleColumn>(window1_RemoveColumn);
            window1.SetModel(this);
        }

        void window1_RemoveColumn(object sender, RemoveVisibleColumn e)
        {
            var model = e.DataContext as VisibleColumnModel;
            var index = this.VisibleColumnsModel.IndexOf(model);
            var data = selectedControl.Properties["VisibleColumns"].Collection.Where(d => d.Properties["MappingName"].ComputedValue.ToString() == model.MappingName).FirstOrDefault();
            selectedControl.Properties["VisibleColumns"].Collection.Remove(data);
            if (selectedControl.Properties["VisibleColumns"].Collection.Count == 0)
            {
                this.GenerateColumnsText = "Generate Columns";
                this.AutoPopulateColumns = true;
                this.VisibleColumnsListBox = Visibility.Collapsed;
                this.InfoText = "Click the Button to generate Visible Columns";
            }
        }

        #region INotifyPropertyChanged Members

        private void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
