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
    /// Interaction logic for AxisCategoryGeneral.xaml
    /// </summary>
    internal partial class AxisCategoryGeneral : UserControl
    {
        public AxisCategoryGeneral()
        {
            InitializeComponent();
        }

        #region Public Properties

        public ExpressionComboBox CategoryReverse
        {
            get
            {
                return this.cmb_ReverseDirection;
            }
            set
            {
                this.cmb_ReverseDirection = value;
            }
        }
       
        public ExpressionComboBox CategoryLineStyle
        {
            get
            {
                return this.cmb_LineStyle;
            }
            set
            {
                this.cmb_LineStyle = value;
            }
        }

        public ExpressionComboBox CategoryLineWidth
        {
            get
            {
                return this.updwn_LineWidth;
            }
            set
            {
                this.updwn_LineWidth = value;
            }
        }

        public CustomUIEditorDropDown CategoryLineColor
        {
            get
            {
                return this.clrpkr_LineColor;
            }
            set
            {
                this.clrpkr_LineColor = value;
            }
        }

        #endregion
    }
}
