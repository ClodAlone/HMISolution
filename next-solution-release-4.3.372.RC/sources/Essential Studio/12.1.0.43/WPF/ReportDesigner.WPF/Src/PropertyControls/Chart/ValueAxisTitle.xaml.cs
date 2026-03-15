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
    /// Interaction logic for ValueAxisTitle.xaml
    /// </summary>
    internal partial class ValueAxisTitle : UserControl
    {
        public ValueAxisTitle()
        {
            InitializeComponent();
        }

        #region Public Properties

        public TextBox ValueTitleText
        {
            get
            {
                return this.Txt_ValueTitle;
            }

            set
            {
                this.Txt_ValueTitle = value;
            }
        }

        public ExpressionComboBox ValueTitleFontFamily
        {
            get
            {
                return this.cmb_ValueTitleFont;
            }

            set
            {
                this.cmb_ValueTitleFont = value;
            }
        }

        public ExpressionComboBox ValueTitleFontSize
        {
            get
            {
                return this.cmb_ValueTitleFontSize;
            }

            set
            {
                this.cmb_ValueTitleFontSize = value;
            }
        }

        public CustomUIEditorDropDown ValueTitleFontColor
        {
            get
            {
                return this.clrpkr_ValueTitleFontColor;
            }

            set
            {
                this.clrpkr_ValueTitleFontColor = value;
            }
        }

        public ExpressionComboBox ValueTitleFontStyle
        {
            get
            {
                return this.cmb_ValueFontStyle;
            }

            set
            {
                this.cmb_ValueFontStyle = value;
            }
        }

        public ExpressionComboBox ValueTitleAlignment
        {
            get
            {
                return this.cmb_TitleAllignment; ;
            }
            set
            {
                this.cmb_TitleAllignment = value;
            }
        }

        #endregion
    }
}
