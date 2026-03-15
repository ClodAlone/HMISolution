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
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Reports.Designer.Editors;

namespace Syncfusion.Windows.Reports.Designer.Dialogs
{
    /// <summary>
    /// Interaction logic for LegandGeneral.xaml
    /// </summary>
    internal partial class LegendGeneral : UserControl
    {
        public LegendGeneral()
        {
            InitializeComponent();
        }

        void Chkbx_ShowLegend_Checked(object sender, RoutedEventArgs e)
        {
            if (Chkbx_ShowLegend.Text.ToLower() == "true")
            {
                this.cmb_LegendPosition.IsEnabled = true;
                this.clrpkr_LegendColor.IsEnabled = true;
            }
            else
            {
                this.cmb_LegendPosition.IsEnabled = false;
                this.clrpkr_LegendColor.IsEnabled = false;
            }
        }

        #region Public Properties

        public TextBox LegendName
        {
            get
            {
                return this.Txt_LegendName;
            }

            set
            {
                this.Txt_LegendName = value;
            }
        }

        public ExpressionComboBox ShowLegend
        {
            get
            {
                return this.Chkbx_ShowLegend;
            }

            set
            {
                this.Chkbx_ShowLegend = value;
            }
        }

        public ComboBox LegendPosition
        {
            get
            {
                return this.cmb_LegendPosition;
            }

            set
            {
                this.cmb_LegendPosition = value;
            }
        }

        public ComboBox LegendLayout
        {
            get
            {
                return this.cmb_LegendLayout;
            }
            set
            {
                this.cmb_LegendLayout = value;
            }
        }

        public CustomUIEditorDropDown LegendBackFill
        {
            get
            {
                return this.clrpkr_LegendColor;
            }

            set
            {
                this.clrpkr_LegendColor = value;
            }
        }

        #endregion
    }
}
