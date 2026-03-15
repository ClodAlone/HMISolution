using System;
using System.Windows;
using System.Windows.Controls;
using UFRecipeLayout.LayoutItemControls;
using UFUAModel.Extensions;
using DevExpress.Xpf.Grid;
using UFRecipeSettings.UFRecipeModel;
using System.Collections.Generic;

namespace RecipeViewerControl.ViewModel
{
    internal class RecipeGridEditorTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ContentControlTemplate { get; set; }

        DataTemplate ItemControlBoolDataTemplate;
        DataTemplate ItemControlComboDataTemplate;
        DataTemplate ItemControlNumericDataTemplate;
        DataTemplate ItemControlStringDataTemplate;
        DataTemplate ItemControlRadioButtonDataTemplate;
        DataTemplate ItemControlListViewDataTemplate;

        public RecipeGridEditorTemplateSelector()
        {
            ItemControlBoolDataTemplate = new DataTemplate();
            var factory = new FrameworkElementFactory(typeof(ItemControlBoolDataValue), "PART_Editor");
            factory.AddHandler(FrameworkElement.LoadedEvent, new RoutedEventHandler(CellTemplate_Loaded));
            ItemControlBoolDataTemplate.VisualTree = factory;

            ItemControlComboDataTemplate = new DataTemplate();
            factory = new FrameworkElementFactory(typeof(ItemControlComboDataValue), "PART_Editor");
            factory.AddHandler(FrameworkElement.LoadedEvent, new RoutedEventHandler(CellTemplate_Loaded));
            ItemControlComboDataTemplate.VisualTree = factory;

            ItemControlNumericDataTemplate = new DataTemplate();
            factory = new FrameworkElementFactory(typeof(ItemControlNumericDataValue), "PART_Editor");
            factory.AddHandler(FrameworkElement.LoadedEvent, new RoutedEventHandler(CellTemplate_Loaded));
            ItemControlNumericDataTemplate.VisualTree = factory;

            ItemControlStringDataTemplate = new DataTemplate();
            factory = new FrameworkElementFactory(typeof(ItemControlStringDataValue), "PART_Editor");
            factory.AddHandler(FrameworkElement.LoadedEvent, new RoutedEventHandler(CellTemplate_Loaded));
            ItemControlStringDataTemplate.VisualTree = factory;

            ItemControlRadioButtonDataTemplate = new DataTemplate();
            factory = new FrameworkElementFactory(typeof(ItemControlRadioButtonDataValue), "PART_Editor");
            factory.AddHandler(FrameworkElement.LoadedEvent, new RoutedEventHandler(CellTemplate_Loaded));
            ItemControlRadioButtonDataTemplate.VisualTree = factory;

            ItemControlListViewDataTemplate = new DataTemplate();
            factory = new FrameworkElementFactory(typeof(ItemControlListViewDataValue), "PART_Editor");
            factory.AddHandler(FrameworkElement.LoadedEvent, new RoutedEventHandler(CellTemplate_Loaded));
            ItemControlListViewDataTemplate.VisualTree = factory;
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            GridCellData data = (GridCellData)item;
            var dataItem = data.RowData.Row as RecipeValueViewModel;

            if (dataItem != null)
            {
                var controlType = dataItem.DataValue.EditControlType;
                if (controlType == EditValueControlTypeEnum.CheckBox)
                    return ItemControlBoolDataTemplate;
                else if (controlType == EditValueControlTypeEnum.ComboBox)
                    return ItemControlComboDataTemplate;
                else if (controlType == EditValueControlTypeEnum.EditDisplay)
                {
                    Type type = dataItem.DataValue.DataType.ToNetType();
                    if (type.IsValueType)
                        return ItemControlNumericDataTemplate;
                    else
                        return ItemControlStringDataTemplate;
                }
                else if (controlType == EditValueControlTypeEnum.RadioButton)
                    return ItemControlRadioButtonDataTemplate;
                else if (controlType == EditValueControlTypeEnum.ListView)
                    return ItemControlListViewDataTemplate;
                else
                    return ItemControlStringDataTemplate;
            }

            return null;
        }

        void CellTemplate_Loaded(object sender, RoutedEventArgs e)
        {
            var fe = sender as FrameworkElement;
            var data = fe.DataContext as GridCellData;
            var dataItem = data?.RowData.Row as RecipeValueViewModel;
            
            if (dataItem != null)
            {
                fe.DataContext = dataItem.Parent.DataSet;
                if (fe is IDataValueUI)
                {
                    var dataValueUI = fe as IDataValueUI;
                    SetEditorSettings(dataValueUI, dataItem);
                    data.ContentChanged += (s1, e2) =>
                    {
                        var newitem = data.RowData.Row as RecipeValueViewModel;
                        dataValueUI.ClearBinding();
                        if (newitem != null)
                            SetEditorSettings(dataValueUI, newitem);
                    };
                }
                if (fe is IPadSupport)
                {
                    fe.PreviewTouchUp += (s1, e1) =>
                    {
                        var newitem = data.RowData.Row as RecipeValueViewModel;
                        if (newitem == null || !newitem.Parent.PromptPad)
                            return;

                        //e1.Handled = true;
                        newitem.Parent.ExecuteAsync(fe.Dispatcher, new Action(() => (fe as IPadSupport).ShowPad()));
                    };

                    fe.PreviewMouseDown += (s1, e1) =>
                    {
                        var newitem = data.RowData.Row as RecipeValueViewModel;
                        if (newitem == null || !newitem.Parent.PromptPad)
                            return;

                        //e1.Handled = true;
                        newitem.Parent.ExecuteAsync(fe.Dispatcher, new Action(() => (fe as IPadSupport).ShowPad()));
                    };
                }
            }
        }

        void SetEditorSettings(IDataValueUI dataValueUI, RecipeValueViewModel dataItem)
        {
            dataValueUI.UnitName = dataItem.UnitName;
            dataValueUI.DecimalDigits = dataItem.DecimalDigits;
            dataValueUI.MinValue = (decimal)dataItem.MinValue;
            dataValueUI.MaxValue = (decimal)dataItem.MaxValue;
            dataValueUI.MaxLength = dataItem.DataValue.MaxLength;
            dataValueUI.SetEnumOptions(dataItem.DataValue.EnumOptions);
            dataValueUI.MapWrongValues = dataItem.MapWrongValues;
            var path = UFRecipeLayout.Helpers.DataBindingHelper.GetDataValuePath(dataItem.DataValue);
            dataValueUI.SetBinding(path, dataItem.DataType, dataItem.ValueConverter);
        }
    }
}
