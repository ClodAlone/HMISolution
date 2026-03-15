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
using System.Windows.Shapes;
using System.ComponentModel;
using RESX = Syncfusion.Windows.Reports.Designer.Properties.Resources;
using Syncfusion.Windows.ReportDesigner.Resources;
using System.Globalization;

namespace Syncfusion.Windows.Reports.Designer.Dialogs
{
    /// <summary>
    /// Interaction logic for ReportParameterAvailableValues.xaml
    /// </summary>
#if SyncfusionFramework4_0
        [DesignTimeVisible(false)]
#endif
    internal partial class ReportParameterAvailableValues : UserControl
    {
        #region Members
        static int m_CmbBoxCount;
        private RDL.DOM.DataSets reportDataSets = null;
        private RDL.DOM.ReportParameter reportParameter = null;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the ReportParameterAvailableValues class.
        /// </summary>
        public ReportParameterAvailableValues()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the ReportParameterAvaliableValues class with the report datasets.
        /// </summary>
        /// <param name="dataSets">Represents the Report DataSets</param>
        public ReportParameterAvailableValues(RDL.DOM.DataSets dataSets)
        {
            InitializeComponent();
            this.reportDataSets = dataSets;
            this.LoadDataSets();
        }

        /// <summary>
        /// Initializes a new instance of the ReportParameterAvaliableValues class with the report datasets.
        /// </summary>
        /// <param name="dataSets">Represents the Report DataSets</param>
        /// <param name="reportParam">Represents the Report Parameter</param>       
        public ReportParameterAvailableValues(RDL.DOM.DataSets dataSets, RDL.DOM.ReportParameter reportParam)
        {
            InitializeComponent();
            this.reportDataSets = dataSets;
            this.reportParameter = reportParam;
            this.LoadDataSets();

            if (this.reportParameter.ValidValues != null)
            {
                if (this.reportParameter.ValidValues.DataSetReference != null)
                {
                    this.rbtn_GetValueFromQuery.IsChecked = true;
                    this.cmb_QueryDataSet.SelectedItem = this.reportParameter.ValidValues.DataSetReference.DataSetName;
                    this.cmb_QueryValueField.SelectedItem = this.reportParameter.ValidValues.DataSetReference.ValueField;
                    this.cmb_QueryLabelField.SelectedItem = this.reportParameter.ValidValues.DataSetReference.LabelField;
                }
                else if (this.reportParameter.ValidValues.ParameterValues != null)
                {
                    this.rbtn_SpecifyValue.IsChecked = true;
                    foreach (RDL.DOM.ParameterValue paramValue in this.reportParameter.ValidValues.ParameterValues)
                    {
                        ++m_CmbBoxCount;
                        TextBox textBox = new TextBox();
                        if (paramValue.Label == null)
                        {
                            textBox.Text = string.Empty;
                        }
                        else
                        {
                            textBox.Text = paramValue.Label;
                        }

                        textBox.Name = "text_BoxSpecificValue" + (m_CmbBoxCount);
                        textBox.Width = 150;

                        ComboBox cBox = new ComboBox();
                        cBox.Name = "cmb_SpecificValue" + (m_CmbBoxCount);
                        cBox.Width = 150;
                        cBox.Margin = new Thickness(30, 0, 0, 0);
                        if (paramValue.Value == null)
                        {
                            cBox.Text = "(Null)";
                        }
                        else
                        {
                            cBox.Text = paramValue.Value;
                        }

                        cBox.IsEditable = true;

                        StackPanel sp = new StackPanel();
                        sp.Orientation = System.Windows.Controls.Orientation.Horizontal;

                        sp.Children.Add(textBox);
                        sp.Children.Add(cBox);
                        
                        this.listBox_Values.Items.Add(sp);
                        sp.GotFocus+=new RoutedEventHandler(sp_GotFocus);
                        if (this.listBox_Values.Items.Count == 2)
                        {
                            this.btn_Delete.IsEnabled = true;
                        }
                        if (this.listBox_Values.Items.Count == 3)
                        {
                            this.btn_UpArrow.IsEnabled = true;
                            this.btn_DownArrow.IsEnabled = true;
                        }
                    }
                }
                else
                {
                    this.rbtn_NoDefaultValue.IsChecked = true;
                }

            }
        }        
        
        #endregion        

        //#region Public Properties
        //public RadioButton NoDefaultValue
        //{
        //    get
        //    {
        //        return this.rbtn_NoDefaultValue;
        //    }
        //    set
        //    {
        //        this.rbtn_NoDefaultValue = value;
        //    }
        //}

        //public RadioButton GetValueFromQuery
        //{
        //    get
        //    {
        //        return this.rbtn_GetValueFromQuery;
        //    }
        //    set
        //    {
        //        this.rbtn_GetValueFromQuery = value;
        //    }
        //}

        //public RadioButton SpecifyValue
        //{
        //    get
        //    {
        //        return this.rbtn_SpecifyValue;
        //    }
        //    set
        //    {
        //        this.rbtn_SpecifyValue = value;
        //    }
        //}
        //#endregion

        #region Helper Methods
        
        private void rbtn_SpecifyValue_Checked(object sender, RoutedEventArgs e)
        {
            this.grd_Specific.Visibility = Visibility.Visible;
            this.grid_QueryValues.Visibility = Visibility.Hidden;
        }

        private void rbtn_GetValueFromQuery_Checked(object sender, RoutedEventArgs e)
        {
            if (this.reportDataSets != null && this.reportDataSets.Count == 0)
            {
                MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxNoDataSetInReport"), SR.GetString(CultureInfo.CurrentUICulture, "titleReportDesigner"), MessageBoxButton.OK);
            }

            this.grd_Specific.Visibility = Visibility.Hidden;
            this.grid_QueryValues.Visibility = Visibility.Visible;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {          
            this.listBox_Values.SelectionChanged += new SelectionChangedEventHandler(listBox_Values_SelectionChanged);            
        }

        private void LoadDataSets()
        {            
            if (this.reportDataSets != null && this.reportDataSets.Count > 0)
            {                
                foreach (RDL.DOM.DataSet datset in this.reportDataSets)
                {
                    this.cmb_QueryDataSet.Items.Add(datset.Name.ToString());
                }
            }
            
        }

        void listBox_Values_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (this.listBox_Values.SelectedIndex == 1)
                {
                    this.btn_UpArrow.IsEnabled = false;
                }

                else
                {
                    this.btn_UpArrow.IsEnabled = true;
                }

                if (this.listBox_Values.SelectedIndex == (this.listBox_Values.Items.Count - 1))
                {
                    this.btn_DownArrow.IsEnabled = false;
                }

                else
                {
                    this.btn_DownArrow.IsEnabled = true;
                }
            }
            catch
            {
            }
        }

        private void Button_Add_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ++m_CmbBoxCount;
                TextBox textBox = new TextBox();
                textBox.Name = "text_BoxSpecificValue" + (m_CmbBoxCount);
                textBox.Width = 150;

                ComboBox cBox = new ComboBox();
                cBox.Name = "cmb_SpecificValue" + (m_CmbBoxCount);
                cBox.Width = 150;
                cBox.Margin = new Thickness(30, 0, 0, 0);
                cBox.Text = "(Null)";
                cBox.IsEditable = true;

               
                StackPanel sp = new StackPanel();
                sp.Orientation = System.Windows.Controls.Orientation.Horizontal;

                sp.Children.Add(textBox);
                sp.Children.Add(cBox);                
                                
                this.listBox_Values.Items.Add(sp);                
                sp.GotFocus += new RoutedEventHandler(sp_GotFocus);
                if (this.listBox_Values.Items.Count >= 2)
                {
                    this.btn_Delete.IsEnabled = true;
                }
                if (this.listBox_Values.Items.Count >= 3)
                {
                    this.btn_UpArrow.IsEnabled = true;
                    this.btn_DownArrow.IsEnabled = true;
                }

                this.listBox_Values.SelectedIndex = this.listBox_Values.Items.Count - 1;
            }
            catch
            {
            }
        }

        void sp_GotFocus(object sender, RoutedEventArgs e)
        {
            this.listBox_Values.SelectedItem = sender;
        }    

        private void Button_UPArrow_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int selectedIndex = this.listBox_Values.SelectedIndex;
                var temp = this.listBox_Values.SelectedItem;
                this.listBox_Values.Items.RemoveAt(selectedIndex);
                this.listBox_Values.Items.Insert(selectedIndex - 1, temp);
                this.listBox_Values.SelectedIndex = selectedIndex - 1;                
            }
            catch
            {
            }
        }

        private void Button_DownArrow_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int selectedIndex = this.listBox_Values.SelectedIndex;
                var temp = this.listBox_Values.SelectedItem;
                this.listBox_Values.Items.RemoveAt(selectedIndex);
                this.listBox_Values.Items.Insert(selectedIndex + 1, temp);
                this.listBox_Values.SelectedIndex = selectedIndex + 1;
            }
            catch
            {
            }
        }

        private void Button_Delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.listBox_Values.Items.RemoveAt(this.listBox_Values.SelectedIndex);
                this.listBox_Values.SelectedIndex = this.listBox_Values.Items.Count - 1;
            }
            catch
            {
            }
            if (this.listBox_Values.Items.Count <= 1)
            {
                this.btn_Delete.IsEnabled = false;
            }
            if (this.listBox_Values.Items.Count <= 2)
            {
                this.btn_UpArrow.IsEnabled = false;
                this.btn_DownArrow.IsEnabled = false;
            }
        }        

        private void cmb_QueryDataSet_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.LoadValueFields();

            this.cmb_QueryValueField.IsEnabled = true;
            this.cmb_QueryLabelField.IsEnabled = true;
        }

        private void LoadValueFields()
        {
            var FieldNames = (from dataSet in this.reportDataSets
                              from field in dataSet.Fields
                              where dataSet.Name == this.cmb_QueryDataSet.SelectedItem.ToString()
                              select field.Name).ToArray();
            if (FieldNames.Length > 0)
            {
                this.cmb_QueryLabelField.Items.Clear();
                this.cmb_QueryValueField.Items.Clear();
                foreach (string strField in FieldNames)
                {
                    this.cmb_QueryLabelField.Items.Add(strField);
                    this.cmb_QueryValueField.Items.Add(strField);
                }
            }

        }

        private void rbtn_NoDefaultValue_Checked(object sender, RoutedEventArgs e)
        {
            if (this.grid_QueryValues != null && this.grd_Specific != null)
            {
                this.grd_Specific.Visibility = Visibility.Collapsed;
                this.grid_QueryValues.Visibility = Visibility.Collapsed;
            }

        }

        #endregion

    }
}