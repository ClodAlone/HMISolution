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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace Syncfusion.Windows.Reports.Designer.Dialogs
{
    /// <summary>
    /// Interaction logic for Tablix.xaml
    /// </summary>
#if SyncfusionFramework4_0
    [DesignTimeVisible(false)]
#endif

    internal partial class TablixFilters : UserControl
    {
        #region private properties

        private RDL.DOM.DataSet dataSet { get; set; }
        
        RDL.DOM.Filters filters { get; set; }

        internal List<string> DataSetFields { get; set; }
        
        internal List<object> DataTypes { get; set; }
        
        internal List<string> Operators { get; set; }
        
        #endregion

        #region constructor

        public TablixFilters()
        {
            InitializeComponent();
            this.UpdateDataType();
            this.UpdateOperators();
        }

        public TablixFilters(RDL.DOM.DataSet dataSet, RDL.DOM.Filters filters)
        {
            InitializeComponent();
            this.dataSet = dataSet;
            this.filters = filters;
            this.UpdateDataType();
            this.UpdateOperators();
            this.UpdateFilterValues();
        }

        internal void UpdateFilterValues()
        {
            UpdateDataSetFields();
            
            if (filters != null)
            {
                foreach (var filter in filters)
                {
                    Grid grd_filter = new Grid();
                    grd_filter.Width = this.Width - 60;
                    grd_filter.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                    ColumnDefinition col1 = new ColumnDefinition();
                    col1.Width = new GridLength(70, GridUnitType.Pixel);
                    ColumnDefinition col2 = new ColumnDefinition();
                    col2.Width = new GridLength(170, GridUnitType.Star);
                    ColumnDefinition col3 = new ColumnDefinition();
                    col3.Width = new GridLength(90, GridUnitType.Pixel);
                    grd_filter.ColumnDefinitions.Add(col1);
                    grd_filter.ColumnDefinitions.Add(col2);
                    grd_filter.ColumnDefinitions.Add(col3);
                    grd_filter.Height = 75;

                    RowDefinition row1 = new RowDefinition();
                    row1.Height = new GridLength(25, GridUnitType.Pixel);
                    grd_filter.RowDefinitions.Add(row1);
                    TextBlock lb_Epr = new TextBlock();
                    lb_Epr.Height = 25;
                    lb_Epr.Text = "Expression";
                    Grid.SetRow(lb_Epr, 0);
                    ComboBox fields = new ComboBox();
                    fields.Height = 22;
                    fields.IsEditable = true;
                    fields.ItemsSource = this.DataSetFields;

                    if (!string.IsNullOrEmpty(filter.FilterExpression))
                    {
                        fields.Text = this.ConvertExpressionToField(filter.FilterExpression);
                    }

                    Grid.SetRow(fields, 0);
                    Grid.SetColumn(fields, 1);
                    fields.SelectionChanged += new SelectionChangedEventHandler(fields_SelectionChanged);
                    ComboBox type = new ComboBox();
                    type.Height = 22;
                    type.Width = 80;
                    type.Margin = new Thickness(0, 0, 5, 0);
                    type.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                    type.ItemsSource = this.DataTypes;

                    if (!string.IsNullOrEmpty(filter.FilterValues.First().Value))
                    {
                        type.SelectedValue = filter.FilterValues.First().DataType;
                        type.IsEnabled = false;
                    }

                    Grid.SetRow(type, 0);
                    Grid.SetColumn(type, 2);
                    grd_filter.Children.Add(lb_Epr);
                    grd_filter.Children.Add(fields);
                    grd_filter.Children.Add(type);

                    RowDefinition row2 = new RowDefinition();
                    row1.Height = new GridLength(25, GridUnitType.Pixel);
                    grd_filter.RowDefinitions.Add(row2);
                    TextBlock lb_operator = new TextBlock();
                    lb_operator.Height = 25;
                    lb_operator.Width = 70;
                    lb_operator.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                    lb_operator.Text = "Operator";
                    Grid.SetRow(lb_operator, 1);
                    ComboBox cmb_operator = new ComboBox();
                    cmb_operator.Height = 22;
                    cmb_operator.Width = 170;
                    cmb_operator.ItemsSource = this.Operators;
                    cmb_operator.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                    cmb_operator.Text = this.UpdateSelection(filter.Operator);
                    Grid.SetRow(cmb_operator, 1);
                    Grid.SetColumn(cmb_operator, 1);
                    grd_filter.Children.Add(lb_operator);
                    grd_filter.Children.Add(cmb_operator);

                    RowDefinition row3 = new RowDefinition();
                    row1.Height = new GridLength(25, GridUnitType.Pixel);
                    grd_filter.RowDefinitions.Add(row3);
                    TextBlock lb_value = new TextBlock();
                    lb_value.Height = 25;
                    lb_value.Width = 70;
                    lb_value.Text = "Value";
                    Grid.SetRow(lb_value, 2);
                    TextBox txt_Value = new TextBox();
                    txt_Value.Height = 22;
                    txt_Value.Margin = new Thickness(0, 0, 5, 0);

                    if (!string.IsNullOrEmpty(filter.FilterValues.First().Value))
                    {
                        txt_Value.Text = filter.FilterValues.First().Value;
                    }

                    Grid.SetRow(txt_Value, 2);
                    Grid.SetColumn(txt_Value, 1);
                    Grid.SetColumnSpan(txt_Value, 2);
                    grd_filter.Children.Add(lb_value);
                    grd_filter.Children.Add(txt_Value);
                    this.lbx_Filters.Items.Add(grd_filter);
                    grd_filter.GotFocus += new RoutedEventHandler(grd_filter_GotFocus);

                    if (this.lbx_Filters.Items.Count >= 1)
                    {
                        this.btn_Delete.IsEnabled = true;
                    }
                    if (this.lbx_Filters.Items.Count >= 2)
                    {
                        this.btn_UpArrow.IsEnabled = true;
                        this.btn_DownArrow.IsEnabled = true;
                    }

                    this.lbx_Filters.SelectedIndex = this.lbx_Filters.Items.Count - 1;
                }
            }
        }

        private string UpdateSelection(RDL.DOM.FilterOperators filterOperators)
        {
            string selection = null;

            switch (filterOperators)
            {
                case RDL.DOM.FilterOperators.Equal:
                    selection = "=";
                    break;
                case RDL.DOM.FilterOperators.GreaterThan:
                    selection = ">";
                    break;
                case RDL.DOM.FilterOperators.LessThan:
                    selection = "<";
                    break;
                case RDL.DOM.FilterOperators.LessThanOrEqual:
                    selection = "<=";
                    break;
                case RDL.DOM.FilterOperators.GreaterThanOrEqual:
                    selection = ">=";
                    break;
                case RDL.DOM.FilterOperators.In:
                    selection = "In";
                    break;
                case RDL.DOM.FilterOperators.Like:
                    selection = "Like";
                    break;
                case RDL.DOM.FilterOperators.NotEqual:
                    selection = "<>";
                    break;
                case RDL.DOM.FilterOperators.TopN:
                    selection = "Top N";
                    break;
                case RDL.DOM.FilterOperators.BottomN:
                    selection = "Bottom N";
                    break;
                case RDL.DOM.FilterOperators.Between:
                    selection = "Between";
                    break;
                case RDL.DOM.FilterOperators.BottomPercent:
                    selection = "Bottom %";
                    break;
                case RDL.DOM.FilterOperators.TopPercent:
                    selection = "Top %";
                    break;
                default:
                    selection = "=";
                    break;
            }

            return selection;
        }

        private string ConvertExpressionToField(string expression)
        {
            if (expression.StartsWith("=Fields!") && expression.EndsWith(".Value"))
            {
                expression = expression.Replace("=Fields!", "[");
                expression = expression.Replace(".Value", "]");
            }

            return expression;
        }

        #endregion

        #region event methohds

        private void btn_Add_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Grid grd_filter = new Grid();
                grd_filter.Width = this.Width - 60;
                grd_filter.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                ColumnDefinition col1 = new ColumnDefinition();
                col1.Width = new GridLength(70, GridUnitType.Pixel);
                ColumnDefinition col2 = new ColumnDefinition();
                col2.Width = new GridLength(170, GridUnitType.Star);
                ColumnDefinition col3 = new ColumnDefinition();
                col3.Width = new GridLength(90, GridUnitType.Pixel);
                grd_filter.ColumnDefinitions.Add(col1);
                grd_filter.ColumnDefinitions.Add(col2);
                grd_filter.ColumnDefinitions.Add(col3);
                grd_filter.Height = 75;

                RowDefinition row1 = new RowDefinition();
                row1.Height = new GridLength(25, GridUnitType.Pixel);
                grd_filter.RowDefinitions.Add(row1);
                TextBlock lb_Epr = new TextBlock();
                lb_Epr.Height = 22;
                lb_Epr.Text = "Expression";
                Grid.SetRow(lb_Epr, 0);
                ComboBox fields = new ComboBox();
                fields.Height = 22;
                fields.IsEditable = true;

                Binding bind = new Binding() { Source = DataSetFields,Mode= BindingMode.OneWay };
                fields.SetBinding(ComboBox.ItemsSourceProperty, bind);
                Grid.SetRow(fields, 0);
                Grid.SetColumn(fields, 1);
                fields.SelectionChanged += new SelectionChangedEventHandler(fields_SelectionChanged);
                ComboBox type = new ComboBox();
                type.Height = 22;
                type.Width = 80;
                type.Margin = new Thickness(0, 0, 5, 0);
                type.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                type.ItemsSource = this.DataTypes;
                type.SelectedItem = type.Items[0];
                Grid.SetRow(type, 0);
                Grid.SetColumn(type, 2);
                grd_filter.Children.Add(lb_Epr);
                grd_filter.Children.Add(fields);
                grd_filter.Children.Add(type);

                RowDefinition row2 = new RowDefinition();
                row1.Height = new GridLength(25, GridUnitType.Pixel);
                grd_filter.RowDefinitions.Add(row2);
                TextBlock lb_operator = new TextBlock();
                lb_operator.Height = 25;
                lb_operator.Width = 70;
                lb_operator.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                lb_operator.Text = "Operator";
                Grid.SetRow(lb_operator, 1);
                ComboBox cmb_operator = new ComboBox();
                cmb_operator.Height = 22;
                cmb_operator.Width = 170;
                cmb_operator.ItemsSource = this.Operators;
                cmb_operator.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                cmb_operator.SelectedItem = cmb_operator.Items[0];
                Grid.SetRow(cmb_operator, 1);
                Grid.SetColumn(cmb_operator, 1);
                grd_filter.Children.Add(lb_operator);
                grd_filter.Children.Add(cmb_operator);

                RowDefinition row3 = new RowDefinition();
                row1.Height = new GridLength(25, GridUnitType.Pixel);
                grd_filter.RowDefinitions.Add(row3);
                TextBlock lb_value = new TextBlock();
                lb_value.Height = 22;
                lb_value.Width = 70;
                lb_value.Text = "Value";
                Grid.SetRow(lb_value, 2);
                TextBox txt_Value = new TextBox();
                txt_Value.Height = 22;
                txt_Value.Margin = new Thickness(0, 0, 5, 0);

                Grid.SetRow(txt_Value, 2);
                Grid.SetColumn(txt_Value, 1);
                Grid.SetColumnSpan(txt_Value, 2);
                grd_filter.Children.Add(lb_value);
                grd_filter.Children.Add(txt_Value);
                this.lbx_Filters.Items.Add(grd_filter);
                grd_filter.GotFocus += new RoutedEventHandler(grd_filter_GotFocus);

                if (this.lbx_Filters.Items.Count >= 1)
                {
                    this.btn_Delete.IsEnabled = true;
                }
                if (this.lbx_Filters.Items.Count >= 2)
                {
                    this.btn_UpArrow.IsEnabled = true;
                    this.btn_DownArrow.IsEnabled = true;
                }

                this.lbx_Filters.SelectedIndex = this.lbx_Filters.Items.Count - 1;
            }
            catch { }
        }

        void fields_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var selected = (sender as ComboBox).SelectedValue.ToString();
                selected = selected.Substring(1, selected.Length - 2);
                var DataType = (from field in this.dataSet.Fields
                                where field.Name == selected
                                select field.TypeName).FirstOrDefault();

                if (!string.IsNullOrEmpty(DataType) && DataType.StartsWith("System."))
                {
                    int index = this.lbx_Filters.SelectedIndex;
                    Grid grid = (Grid)this.lbx_Filters.Items[index];
                    ComboBox cmb_type = grid.Children.OfType<ComboBox>().ToList().ElementAt(1);
                    cmb_type.IsEnabled = false;

                    if (DataType.StartsWith("System.String"))
                    {
                        cmb_type.SelectedItem = cmb_type.Items[0];
                    }
                    else if (DataType.StartsWith("System.Boolean"))
                    {
                        cmb_type.SelectedItem = cmb_type.Items[1];
                    }
                    else if (DataType.StartsWith("System.DateTime"))
                    {
                        cmb_type.SelectedItem = cmb_type.Items[2];
                    }
                    else if (DataType.StartsWith("System.Float"))
                    {
                        cmb_type.SelectedItem = cmb_type.Items[3];
                    }
                    else if (DataType.StartsWith("System.Int"))
                    {
                        cmb_type.SelectedItem = cmb_type.Items[4];
                    }
                    else
                    {
                        cmb_type.SelectedItem = cmb_type.Items[0];
                        cmb_type.IsEnabled = true;
                    }

                    this.lbx_Filters.Items.RemoveAt(index);
                    this.lbx_Filters.Items.Insert(index, grid);
                }
            }
            catch { }
        }

        private void btn_Delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.lbx_Filters.Items.RemoveAt(this.lbx_Filters.SelectedIndex);
                this.lbx_Filters.SelectedIndex = this.lbx_Filters.Items.Count - 1;
            }
            catch { }
            if (this.lbx_Filters.Items.Count < 1)
            {
                this.btn_Delete.IsEnabled = false;
            }
            if (this.lbx_Filters.Items.Count < 2)
            {
                this.btn_UpArrow.IsEnabled = false;
                this.btn_DownArrow.IsEnabled = false;
            }
        }
        private void btn_UpArrow_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.lbx_Filters.SelectedIndex > 0)
                {
                    int index = this.lbx_Filters.SelectedIndex;
                    var tempItem = this.lbx_Filters.SelectedItem;
                    this.lbx_Filters.Items.RemoveAt(index);
                    this.lbx_Filters.Items.Insert(index - 1, tempItem);
                    this.lbx_Filters.SelectedIndex = index - 1;
                }
            }
            catch { }
        }

        private void btn_DownArrow_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.lbx_Filters.SelectedIndex != this.lbx_Filters.Items.Count - 1)
                {
                    int index = this.lbx_Filters.SelectedIndex;
                    var tempItem = this.lbx_Filters.SelectedItem;
                    this.lbx_Filters.Items.RemoveAt(index);
                    this.lbx_Filters.Items.Insert(index + 1, tempItem);
                    this.lbx_Filters.SelectedIndex = index + 1;
                }
            }
            catch { }
        }

        void grd_filter_GotFocus(object sender, RoutedEventArgs e)
        {
            this.lbx_Filters.SelectedItem = sender;
        }

        private void UpdateOperators()
        {
            if (this.Operators == null)
            {
                this.Operators = new List<string>();
            }

            this.Operators.Clear();
            this.Operators.Add(">");
            this.Operators.Add(">=");
            this.Operators.Add("<");
            this.Operators.Add("<=");
            this.Operators.Add("=");
            this.Operators.Add("<>");
            this.Operators.Add("Like");
            this.Operators.Add("In");
            this.Operators.Add("Between");
            this.Operators.Add("Top N");
            this.Operators.Add("Bottom N");
            this.Operators.Add("Top %");
            this.Operators.Add("Bottom %");
        }

        private void UpdateDataType()
        {
            if (this.DataTypes == null)
            {
                this.DataTypes = new List<object>();
            }
            this.DataTypes.Clear();
            this.DataTypes.Add(RDL.DOM.DataTypes.String);
            this.DataTypes.Add(RDL.DOM.DataTypes.Boolean);
            this.DataTypes.Add(RDL.DOM.DataTypes.DateTime);
            this.DataTypes.Add(RDL.DOM.DataTypes.Float);
            this.DataTypes.Add(RDL.DOM.DataTypes.Integer);
        }

        private void UpdateDataSetFields()
        {
            if (this.dataSet != null)
            {
                if (this.DataSetFields == null)
                {
                    this.DataSetFields = new List<string>();
                }
                this.DataSetFields.Clear();

                if (this.dataSet.Fields != null)
                {
                    foreach (var field in this.dataSet.Fields)
                    {
                        this.DataSetFields.Add("[" + field.Name + "]");
                    }
                }
            }
        }

        internal void ModifyDataSetFields(RDL.DOM.Fields fields)
        {
            this.dataSet.Fields = fields;
            UpdateDataSetFields();
        }

        #endregion

        internal void PopulateFilterWindow(RDL.DOM.DataSet dataSet, RDL.DOM.Filters filters)
        {
            this.dataSet = dataSet;
            this.filters = filters;
            this.UpdateFilterValues();
        }
    }
}
