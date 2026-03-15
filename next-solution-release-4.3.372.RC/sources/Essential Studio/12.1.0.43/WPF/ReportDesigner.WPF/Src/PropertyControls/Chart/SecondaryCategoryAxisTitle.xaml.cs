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
using Syncfusion.Windows.Reports.Designer.Editors;

namespace Syncfusion.Windows.Reports.Designer.Dialogs
{
    /// <summary>
    /// Interaction logic for SecondaryCategoryAxisTitle.xaml
    /// </summary>
    internal partial class SecondaryCategoryAxisTitle : UserControl
    {
        public SecondaryCategoryAxisTitle()
        {
            InitializeComponent();
        }

        #region Public Properties

        public TextBox CategoryTitleText
        {
            get
            {
                return this.Txt_CategoryTitle;
            }

            set
            {
                this.Txt_CategoryTitle = value;
            }
        }

        public ExpressionComboBox CategoryTitleFontFamily
        {
            get
            {
                return this.cmb_CategoryTitleFont;
            }

            set
            {
                this.cmb_CategoryTitleFont = value;
            }
        }

        public ExpressionComboBox CategoryTitleFontSize
        {
            get
            {
                return this.cmb_CategoryTitleFontSize;
            }

            set
            {
                this.cmb_CategoryTitleFontSize = value;
            }
        }

        public CustomUIEditorDropDown CategoryTitleFontColor
        {
            get
            {
                return this.clrpkr_CategoryFontColor;
            }

            set
            {
                this.clrpkr_CategoryFontColor = value;
            }
        }

        public ExpressionComboBox CategoryTitleStyle
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


        public ExpressionComboBox CategoryTitleAllignment
        {
            get
            {
                return this.cmb_TitleAllignment;
            }
            set
            {
                this.cmb_TitleAllignment = value;
            }
        }

        #endregion
    }
}
