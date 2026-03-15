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
using System.ComponentModel;

namespace Syncfusion.Windows.Reports.Designer.Dialogs
{
    /// <summary>
    /// Interaction logic for HeaderGeneral.xaml
    /// </summary>    
#if SyncfusionFramework4_0
        [DesignTimeVisible(false)]
#endif
    internal partial class HeaderGeneral
        : UserControl
    {
        #region Constructor
        public HeaderGeneral()
        {
            InitializeComponent();
            System.Globalization.NumberFormatInfo number = new System.Globalization.NumberFormatInfo();
            number.NumberDecimalDigits = 0;
            number.PercentSymbol = "pt";
            this.updwn_Height.NumberFormatInfo = number;
            this.updwn_Height.MinValue = 0;
        }
        #endregion

        #region Public Properties
        public CheckBox DisplayHeader
        {
            get
            {
                return this.chk_DisplayHeader;
            }
            set
            {
                this.chk_DisplayHeader = value;
            }
        }

        public CheckBox PrintOnFirstPage
        {
            get
            {
                return this.chk_PrintOnFirstPage;
            }
            set
            {
                this.chk_PrintOnFirstPage = value;
            }
        }

        public CheckBox PrintOnLastPage
        {
            get
            {
                return this.chk_PringOnLastPage;
            }
            set
            {
                this.chk_PringOnLastPage = value;
            }
        }

        public new UpDown Height
        {
            get
            {
                return this.updwn_Height;
            }
            set
            {
                this.updwn_Height = value;
            }
        }

        public TextBlock HeightUnit
        {
            get
            {
                return this.txt_HeightUnit;
            }
            set
            {
                this.txt_HeightUnit = value;
            }
        }
        #endregion
    }
}
