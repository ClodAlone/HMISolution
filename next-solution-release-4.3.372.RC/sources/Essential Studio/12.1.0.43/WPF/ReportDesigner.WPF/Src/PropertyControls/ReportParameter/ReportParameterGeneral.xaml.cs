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

namespace Syncfusion.Windows.Reports.Designer.Dialogs
{
    /// <summary>
    /// Interaction logic for ReportParameterGeneral.xaml
    /// </summary>
#if SyncfusionFramework4_0
        [DesignTimeVisible(false)]
#endif
    internal partial class ReportParameterGeneral : UserControl
    {
        #region Members
        /// <summary>
        /// Local member for store the Report Datasets
        /// </summary>
        private RDL.DOM.DataSets reportDatasets = null;
        private RDL.DOM.ReportParameter reportParameter = null;
        #endregion

        #region Constructors
        /// <summary>
        /// Initialize a new instance of the ReportParameterGeneral class.
        /// </summary>
        public ReportParameterGeneral()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initialize a new instance of the ReportParameterGeneral class.
        /// </summary>
        /// <param name="dataSets">Represents the Report Dataset</param>
        public ReportParameterGeneral(RDL.DOM.DataSets dataSets, string[] reportParamNames)
        {
            InitializeComponent();
            this.reportDatasets = dataSets;
            int countOfParameter = 1; // Initially the parameter count 1 for its unique name
            string tempName = "ReportParameter"+countOfParameter;

            if (reportParamNames != null)
            {
                bool available;

                do
                {
                    available=((from name in reportParamNames
                               where name.Equals(tempName)
                               select name).Count()) > 0 ? true : false;

                    if(available)
                    {
                        tempName = "ReportParameter" + ++countOfParameter;
                    }
                }while(available);
            }

            this.text_ParamName.Text = tempName;
            this.text_ParamPromptName.Text = tempName;
            this.rBtn_Visible.IsChecked = true;
        }

        /// <summary>
        /// Initialize a new instance of the ReportParameterGeneral class.
        /// </summary>
        /// <param name="dataSets">Represents the Report Dataset</param>
        /// <param name="reportParam">Represents the Report Parameter</param>        
        public ReportParameterGeneral(RDL.DOM.DataSets dataSets, RDL.DOM.ReportParameter reportParam)
        {
            InitializeComponent();
            this.reportParameter = reportParam;
            this.reportDatasets = dataSets;
            this.text_ParamName.Text = this.reportParameter.Name;
            if (this.reportParameter.Prompt == null)
            {
                this.text_ParamPromptName.IsEnabled = false;
                this.rBtn_Internal.IsChecked = true;
            }
            else
            {
                this.text_ParamPromptName.IsEnabled = true;
                this.text_ParamPromptName.Text = this.reportParameter.Prompt;
                this.rBtn_Visible.IsChecked = true;
            }
            this.cmbBox_ParamType.SelectedItem = this.reportParameter.DataType.ToString();
            this.chkBox_AllowBlank.IsChecked = this.reportParameter.AllowBlank;
            this.chkBox_AllowMultiple.IsChecked = this.reportParameter.MultiValue;
            this.chkBox_AllowNull.IsChecked = this.reportParameter.Nullable;
            this.rBtn_Hidden.IsChecked = this.reportParameter.Hidden;
        }         
        #endregion

        #region Helper Methods
        private void cmbBox_ParamType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.cmbBox_ParamType.SelectedItem.ToString().Equals("Text"))
            {
                this.chkBox_AllowBlank.IsEnabled = true;
            }
            else
            {
                this.chkBox_AllowBlank.IsEnabled = false;
            }       
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.cmbBox_ParamType.Items.Add("Text");
            this.cmbBox_ParamType.Items.Add("Boolean");
            this.cmbBox_ParamType.Items.Add("Date/Time");
            this.cmbBox_ParamType.Items.Add("Integer");
            this.cmbBox_ParamType.Items.Add("Float");

            if ((sender as ReportParameterGeneral).reportParameter != null)
            {
                if ((sender as ReportParameterGeneral).reportParameter.DataType == Syncfusion.RDL.DOM.DataTypes.String)
                {
                    this.cmbBox_ParamType.Text = this.cmbBox_ParamType.Items[0].ToString();
                }
                else if ((sender as ReportParameterGeneral).reportParameter.DataType == Syncfusion.RDL.DOM.DataTypes.DateTime)
                {
                    this.cmbBox_ParamType.Text = this.cmbBox_ParamType.Items[2].ToString();
                }
                else if ((sender as ReportParameterGeneral).reportParameter.DataType == Syncfusion.RDL.DOM.DataTypes.Integer)
                {
                    this.cmbBox_ParamType.Text = this.cmbBox_ParamType.Items[3].ToString();
                }
                else
                {
                    this.cmbBox_ParamType.Text = (sender as ReportParameterGeneral).reportParameter.DataType.ToString();
                }

                if ((sender as ReportParameterGeneral).reportParameter.AllowBlank == true)
                {
                    this.chkBox_AllowBlank.IsChecked = true;
                }
                if ((sender as ReportParameterGeneral).reportParameter.Nullable == true)
                {
                    this.chkBox_AllowNull.IsChecked = true;
                }
                if ((sender as ReportParameterGeneral).reportParameter.MultiValue == true)
                {
                    this.chkBox_AllowMultiple.IsChecked = true;
                }
            }
            else
            {
                this.cmbBox_ParamType.Text = this.cmbBox_ParamType.Items[0].ToString();
                this.chkBox_AllowBlank.IsChecked = false;
                this.chkBox_AllowNull.IsChecked = false;
                this.chkBox_AllowMultiple.IsChecked =false;
            }
        }

        private void rBtn_Internal_Checked(object sender, RoutedEventArgs e)
        {
            this.text_ParamPromptName.IsEnabled = false;            
        }        

        private void rBtn_Visible_Checked(object sender, RoutedEventArgs e)
        {
            this.text_ParamPromptName.IsEnabled = true;
        }       

        private void rBtn_Hidden_Checked(object sender, RoutedEventArgs e)
        {
            this.text_ParamPromptName.IsEnabled = true;
        }

        #endregion
    }
}
