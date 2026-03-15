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
    /// Interaction logic for LegandFont.xaml
    /// </summary>
    internal partial class LegendFont : UserControl
    {
        public LegendFont()
        {
            InitializeComponent();
        }

        #region Public Properties

        public ExpressionComboBox LegendFontFamily
        {
            get
            {
                return this.cmb_LegendFont;
            }

            set
            {
                this.cmb_LegendFont = value;
            }
        }

        public ExpressionComboBox LegendFontSize
        {
            get
            {
                return this.cmb_LegendFontSize;
            }

            set
            {
                this.cmb_LegendFontSize = value;
            }
        }

        public CustomUIEditorDropDown LegendFontColor
        {
            get
            {
                return this.clrpkr_LegendFontColor;
            }

            set
            {
                this.clrpkr_LegendFontColor = value;
            }
        }

        public ExpressionComboBox LegendFontStyle
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

        #endregion
    }
}
