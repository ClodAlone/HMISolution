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
    /// Interaction logic for AxisValueLabel.xaml
    /// </summary>
    internal partial class AxisValueLabel : UserControl
    {
        public AxisValueLabel()
        {
            InitializeComponent();
            System.Globalization.NumberFormatInfo number = new System.Globalization.NumberFormatInfo();
            number.NumberDecimalDigits = 0;
            this.updwn_LabelAngle.NumberFormatInfo = number;
            this.updwn_LabelAngle.MinValue = 0;
            this.updwn_LabelAngle.MaxValue = 90;
        }

        #region Public Properties

        public ExpressionComboBox HideValueAxisLabels
        {
            get
            {
                return this.cmb_HideAxisLabel;
            }
            set
            {
                this.cmb_HideAxisLabel = value;
            }
        }

        public ExpressionComboBox ValueLabelFontFamily
        {
            get
            {
                return this.cmb_LabelFontFamily;
            }

            set
            {
                this.cmb_LabelFontFamily = value;
            }
        }

        public ExpressionComboBox ValueLabelFontSize
        {
            get
            {
                return this.updwn_LabelFontSize;
            }

            set
            {
                this.updwn_LabelFontSize = value;
            }
        }

        public CustomUIEditorDropDown ValueLabelFontColor
        {
            get
            {
                return this.clrpkr_LabelFontColor;
            }

            set
            {
                this.clrpkr_LabelFontColor = value;
            }
        }

        public ExpressionComboBox ValueLabelFontWeight
        {
            get
            {
                return this.cmb_FontWeight;
            }

            set
            {
                this.cmb_FontWeight = value;
            }
        }

        public UpDown ValueLabelFontAngle
        {
            get
            {
                return this.updwn_LabelAngle;
            }

            set
            {
                this.updwn_LabelAngle = value;
            }
        }

        #endregion
       
    }
}
