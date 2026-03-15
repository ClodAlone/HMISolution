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
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Reports.Designer.Dialogs;
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.Windows.Reports.Designer.Editors;

namespace Syncfusion.Windows.Reports.Designer.Dialogs
{
    /// <summary>
    /// Interaction logic for Border.xaml
    /// </summary>
    internal partial class ChartBorder : UserControl
    {
        public ChartBorder()
        {
            InitializeComponent();
            System.Globalization.NumberFormatInfo number = new System.Globalization.NumberFormatInfo();
            number.NumberDecimalDigits = 0;            
        }

        #region Public Properties

        public ExpressionComboBox ChartBorderWidth
        {
            get
            {
                return this.updwn_BorderWidth;
            }

            set
            {
                this.updwn_BorderWidth = value;
            }
        }

        public CustomUIEditorDropDown ChartBorderColor
        {
            get
            {
                return this.clrpkr_BorderColor;
            }

            set
            {
                this.clrpkr_BorderColor = value;
            }
        }

        #endregion
    }
}
