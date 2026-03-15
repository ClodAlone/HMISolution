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
using Syncfusion.Windows.Reports.Designer.Dialogs;

namespace Syncfusion.Windows.Reports.Designer.Dialogs
{
    /// <summary>
    /// Interaction logic for RangeBorder.xaml
    /// </summary>
    internal partial class RangeBorder : UserControl
    {
        public RangeBorder()
        {
            InitializeComponent();
            System.Globalization.NumberFormatInfo number = new System.Globalization.NumberFormatInfo();
            number.NumberDecimalDigits = 0;
            this.updwn_RangeBorderWidth.NumberFormatInfo = number;
            this.updwn_RangeBorderWidth.Value = 0;
            this.updwn_RangeBorderWidth.MaxValue = 20;
        }

        #region Public Properties

        public UpDown RangeBorderWidth
        {
            get
            {
                return this.updwn_RangeBorderWidth;
            }

            set
            {
                this.updwn_RangeBorderWidth = value;
            }
        }

        public ColorPicker RangeBorderColor
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
