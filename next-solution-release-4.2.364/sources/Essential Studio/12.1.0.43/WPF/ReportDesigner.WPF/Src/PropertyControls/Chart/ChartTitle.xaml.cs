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
    /// Interaction logic for ChartTitle.xaml
    /// </summary>
    internal partial class ChartTitle : UserControl
    {
        public ChartTitle()
        {
            InitializeComponent();
        }

        #region Public Properties

        public TextBox TitleText
        {
            get
            {
                return this.Txt_ChartTitle;
            }

            set
            {
                this.Txt_ChartTitle = value;
            }
        }

        public ExpressionComboBox TitleFontFamily
        {
            get
            {
                return this.cmb_ChartTitleFont;
            }

            set
            {
                this.cmb_ChartTitleFont = value;
            }
        }

        public ExpressionComboBox TitleFontSize
        {
            get
            {
                return this.cmb_ChartTitleFontSize;
            }

            set
            {
                this.cmb_ChartTitleFontSize = value;
            }
        }

        public new ExpressionComboBox FontStyle
        {
            get
            {
                return this.cmb_FontStyle;
            }

            set
            {
                this.cmb_FontStyle = value;
            }
        }

        public CustomUIEditorDropDown TitleFontColor
        {
            get
            {
                return this.clrpkr_ChartFontColor;
            }

            set
            {
                this.clrpkr_ChartFontColor = value;
            }
        }      

        public CustomUIEditorDropDown TitleBackground
        {
            get
            {
                return this.clrpkr_ChartTitleBackground;
            }

            set
            {
                this.clrpkr_ChartTitleBackground = value;
            }
        }

        #endregion
    }
}
