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
using Syncfusion.Windows.Reports.Designer.Dialogs;
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Reports.Designer.Editors;

namespace Syncfusion.Windows.Reports.Designer.Dialogs
{
    /// <summary>
    /// Interaction logic for ChartAreaBackground.xaml
    /// </summary>
    internal partial class ChartAreaBackground : UserControl
    {
        public ChartAreaBackground()
        {
            InitializeComponent();
        }

        #region Public Properties

        public ExpressionComboBox FillStyle
        {
            get
            {
                return this.cmb_FillStyle;
            }

            set
            {
                this.cmb_FillStyle = value;
            }
        }      

        public CustomUIEditorDropDown ColorFill
        {
            get
            {
                return this.clrpkr_ColorFill;
            }

            set
            {
                this.clrpkr_ColorFill = value;
            }
        }

        public CustomUIEditorDropDown SecondaryColor
        {
            get
            {
                return this.clrpkr_SecondaryColor;
            }

            set
            {
                this.clrpkr_SecondaryColor = value;
            }
        }

        public ExpressionComboBox AreaGradientType
        {
            get
            {
                return this.cmb_GradientStyle;
            }

            set
            {
                this.cmb_GradientStyle = value;
            }
        }

        #endregion
      
        private void cmb_FillStyle_Changed(object sender,SelectionChangedEventArgs e)
        {
            if (this.cmb_FillStyle.Text.ToLower() == "solid")
            {
                this.clrpkr_SecondaryColor.IsEnabled = true;
                this.cmb_GradientStyle.IsEnabled = true;
            }
            else
            {
                this.clrpkr_SecondaryColor.IsEnabled = false;
                this.cmb_GradientStyle.IsEnabled = false;
            }

        }
        private void grd_ChartAreaBackground_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.cmb_FillStyle.Text.ToString() == "solid")
            {
                this.clrpkr_SecondaryColor.IsEnabled = false;
                this.cmb_GradientStyle.IsEnabled = false;
            }
            else
            {
                this.clrpkr_SecondaryColor.IsEnabled = true;
                this.cmb_GradientStyle.IsEnabled = true;
            }
        }
    }
}
