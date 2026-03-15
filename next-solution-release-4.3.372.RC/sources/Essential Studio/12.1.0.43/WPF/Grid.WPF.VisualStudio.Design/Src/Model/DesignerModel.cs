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
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Collections;
using Syncfusion.Windows.Data;
using Syncfusion.Linq;
using Syncfusion.Linq.Data;
using System.Data;
using System.Collections.Specialized;
namespace Syncfusion.Grid.WPF.VisualStudio.Design
{
    public class DesignerModel : DependencyObject, INotifyPropertyChanged
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
            get
            {
                return (bool)selectedControl.Properties["ShowColumnOptions"].ComputedValue;
            }
            set
            {
                selectedControl.Properties["ShowColumnOptions"].SetValue(value);
                RaisePropertyChanged("ShowColumnOptions");
            }
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
            get
            {
                return (bool)selectedControl.Properties["AllowEdit"].ComputedValue;
            }
            set
            {
                selectedControl.Properties["AllowEdit"].SetValue(value);
                RaisePropertyChanged("AllowEdit");
            }
        }

        public bool ShowErrorTooltips
        {
            get
            {
                return (bool)selectedControl.Properties["ShowErrorTooltips"].ComputedValue;
            }
            set
            {
                selectedControl.Properties["ShowErrorTooltips"].SetValue(value);
                RaisePropertyChanged("ShowErrorTooltips");
            }
        }

        public bool ShowTooltips
        {
            get
            {
                return (bool)selectedControl.Properties["ShowTooltips"].ComputedValue;
            }
            set
            {
                selectedControl.Properties["ShowTooltips"].SetValue(value);
                RaisePropertyChanged("ShowTooltips");
            }
        }

        public bool AllowDelete
        {
            get
            {
                return (bool)selectedControl.Properties["AllowDelete"].ComputedValue;
            }
            set
            {
                selectedControl.Properties["AllowDelete"].SetValue(value);
                RaisePropertyChanged("AllowDelete");
            }
        }

        public static readonly DependencyProperty GenerateVisibleColumnsProperty = DependencyProperty.Register(
           "GenerateVisibleColumns",
           typeof(bool),
           typeof(DesignerModel), new FrameworkPropertyMetadata(OnGenerateVisibleColumnChanged));

        public bool GenerateVisibleColumns
        {
            get
            {
                return (bool)this.GetValue(DesignerModel.GenerateVisibleColumnsProperty);
            }

            set
            {
                this.SetValue(DesignerModel.GenerateVisibleColumnsProperty, value);
            }
        }

        private static void OnGenerateVisibleColumnChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            DesignerModel grid = d as DesignerModel;
            grid.OnGenerateVisibleColumnChanged((bool)args.NewValue);

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
                if (source is DataTable)
                {
                    result = ((DataTable)source).DefaultView;
                }
                else if (source is DataView)
                {
                    result = source as DataView;
                }
                else if (source is CollectionViewSource)
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
                    result = ((IEnumerable)source).ToTypedSource();
                    // sometimes the non-generic source would be null initially, we simply check for IBindingList / INotifyCollectionChanged based interfaces
                    if (result == null)
                    {
                        if (source is IBindingList)
                        {
                            var bindingList = source as IBindingList;
                        }
                        else if (source is INotifyCollectionChanged)
                        {
                            var notifyCollection = source as INotifyCollectionChanged;
                        }
                    }
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
            var gridModel = selectedControl.Properties["Model"].ComputedValue as GridDataTableModel;
            var isItemsSourceSet = gridModel.SourceList != null;
            if (!isItemsSourceSet)
            {
                return;
            }

            PropertyDescriptorCollection pdc = null;
            if (SourceList != null)
            {
                if(SourceList is DataView)
                    pdc = TypeDescriptor.GetProperties(SourceList as DataView);
                else
                    pdc = TypeDescriptor.GetProperties(SourceList.AsQueryable().ElementAt(0));
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
                        var col = GeneratedVisibleColumns[colum];
                        var vcModel = selectedControl.Properties["VisibleColumns"].Collection.Add(col);
                        var pd = gridModel.View.GetItemProperties()[GeneratedVisibleColumns[colum].MappingName];
                        var specialCellType = GetCellType(pd.PropertyType.Name);
                        VisibleColumnModel mod = new VisibleColumnModel(vcModel, specialCellType, pd.PropertyType.Name) { MappingNameType = GeneratedVisibleColumns[colum].MappingName + "    { " + pd.PropertyType.Name + " }" };
                        this.VisibleColumnsModel.Add(mod);
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
                for (int colum = 0; colum < selectedControl.Properties["VisibleColumns"].Collection.Count; colum++)
                {
                    var vcModel = selectedControl.Properties["VisibleColumns"].Collection[colum];
                    var pd = gridModel.View.GetItemProperties()[GeneratedVisibleColumns[colum].MappingName];
                    var specialCellType = GetCellType(pd.PropertyType.Name);
                    specialCellType = GetCellType(pd.PropertyType.Name);
                    VisibleColumnModel mod = new VisibleColumnModel(vcModel, specialCellType, pd.PropertyType.Name) { SpecialCellType = specialCellType, MappingNameType = GeneratedVisibleColumns[colum].MappingName + "    { " + pd.PropertyType.Name + " }" };
                    this.VisibleColumnsModel.Add(mod);
                }
            }
            this.InfoText = "Set properties for Visible Columns";
            this.VisibleColumnsListBox = Visibility.Visible;
            this.AutoPopulateColumns = false;
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

        private CellTypes GetCellType(string ColumnType)
        {
            switch (ColumnType)
            {
                case "Double":
                    return CellTypes.DoubleEdit;
                case "DateTime":
                    return CellTypes.DateTimeEdit;
                case "Boolean":
                case "bool":
                    return CellTypes.CheckBox;

            }
            return CellTypes.TextBox;
        }

        public void ClearColumns()
        {
            this.VisibleColumnsModel.Clear();
            selectedControl.Properties["VisibleColumns"].Collection.Clear();
            this.AutoPopulateColumns = true;
        }

        public GridSelectionFlags AllowSelection
        {
            get
            {
                return (GridSelectionFlags)selectedControl.Properties["AllowSelection"].ComputedValue; ;
            }
            set
            {
                selectedControl.Properties["AllowSelection"].SetValue(value);
                RaisePropertyChanged("AllowSelection");
            }
        }

        public GridSelectionMode ListBoxSelectionMode
        {
            get
            {
                return (GridSelectionMode)selectedControl.Properties["ListBoxSelectionMode"].ComputedValue; ;
            }
            set
            {
                selectedControl.Properties["ListBoxSelectionMode"].SetValue(value);
                RaisePropertyChanged("ListBoxSelectionMode");
            }
        }

        public GridCellActivateAction ActivateCurrentCellBehavior
        {
            get
            {
                return (GridCellActivateAction)selectedControl.Properties["ActivateCurrentCellBehavior"].ComputedValue; ;
            }
            set
            {
                selectedControl.Properties["ActivateCurrentCellBehavior"].SetValue(value);
                RaisePropertyChanged("ActivateCurrentCellBehavior");
            }
        }

        public GridControlLengthUnitType ColumnSizer
        {
            get
            {
                return (GridControlLengthUnitType)selectedControl.Properties["ColumnSizer"].ComputedValue; ;
            }
            set
            {
                selectedControl.Properties["ColumnSizer"].SetValue(value);
                RaisePropertyChanged("ColumnSizer");
            }
        }

        public VisualStyle VisualStyle
        {
            get
            {
                return (VisualStyle)selectedControl.Properties["VisualStyle"].ComputedValue;
            }
            set
            {
                selectedControl.Properties["VisualStyle"].SetValue(value);
                RaisePropertyChanged("VisualStyle");
            }
        }

        public FreezableCollection<GridDataVisibleColumn> VisibleColumns
        {
            get
            {
                var visibleColumn = (FreezableCollection<GridDataVisibleColumn>)selectedControl.Properties["VisibleColumns"].ComputedValue;

                if (visibleColumn != null)
                {
                    return visibleColumn;
                }
                else
                {
                    return new FreezableCollection<GridDataVisibleColumn>();
                }
            }
            set
            {

                selectedControl.Properties["VisibleColumns"].SetValue(value);
                RaisePropertyChanged("VisibleColumns");
            }
        }

        public ObservableCollection<VisibleColumnModel> VisibleColumnsModel
        {
            get;
            set;
        }

        public ObservableCollection<GridDataVisibleColumn> GeneratedVisibleColumns
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
            this.GeneratedVisibleColumns = new ObservableCollection<GridDataVisibleColumn>();

            foreach (var vc in VisibleColumns)
            {
                this.GeneratedVisibleColumns.Add(vc);
            }
            this.VisibleColumnsModel = new ObservableCollection<VisibleColumnModel>();
            var gridModel = selectedControl.Properties["Model"].ComputedValue as GridDataTableModel;
            if (selectedControl.Properties["VisibleColumns"].Collection.Count != 0 && gridModel.View != null)
            {
                for (int colum = 0; colum < selectedControl.Properties["VisibleColumns"].Collection.Count; colum++)
                {
                    var vcModel = selectedControl.Properties["VisibleColumns"].Collection[colum];
                    var pd = gridModel.View.GetItemProperties()[GeneratedVisibleColumns[colum].MappingName];
                    var specialCellType = GetCellType(pd.PropertyType.Name);
                    specialCellType = GetCellType(pd.PropertyType.Name);
                    VisibleColumnModel mod = new VisibleColumnModel(vcModel, specialCellType, pd.PropertyType.Name) { SpecialCellType = specialCellType, MappingNameType = GeneratedVisibleColumns[colum].MappingName + "    { " + pd.PropertyType.Name + " }" };
                    this.VisibleColumnsModel.Add(mod);
                }

                this.InfoText = "Set properties for Visible Columns";
                this.VisibleColumnsListBox = Visibility.Visible;
                this.GenerateColumnsText = "Clear Columns";
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
                if (SourceList is DataView)
                {
                    if ((SourceList as DataView).Count == 0)
                    {
                        this.GenerateColumnsEnabled = false;
                        this.InfoText = "ItemsSource should not be Empty";
                    }
                }
                else if (SourceList.AsQueryable().Count() == 0)
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

        public void ShowWindow()
        {
            Window1 window1 = new Window1();
            window1.View.SetModel(this);
            window1.View.VisibleColn.RemoveColumn += new EventHandler<RemoveVisibleColumn>(VisibleColn_RemoveColumn);
            window1.ShowDialog();

        }

        void VisibleColn_RemoveColumn(object sender, RemoveVisibleColumn e)
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
