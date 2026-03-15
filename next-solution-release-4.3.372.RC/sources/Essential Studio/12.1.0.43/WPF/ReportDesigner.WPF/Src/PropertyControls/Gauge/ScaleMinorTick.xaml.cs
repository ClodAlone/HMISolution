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
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.Windows.Reports.Designer.Dialogs;

namespace Syncfusion.Windows.Reports.Designer.Dialogs
{
    /// <summary>
    /// Interaction logic for ScaleMinorTick.xaml
    /// </summary>
    internal partial class ScaleMinorTick : UserControl
    {
        public ScaleMinorTick()
        {
            InitializeComponent();
            System.Globalization.NumberFormatInfo number = new System.Globalization.NumberFormatInfo();
            number.NumberDecimalDigits = 0;
            this.updwn_MinorTickLength.NumberFormatInfo = number;
            this.updwn_MinorTickWidth.NumberFormatInfo = number;
            this.updwn_MinorTickLength.MinValue = 0;
            this.updwn_MinorTickWidth.MinValue = 0;
            this.updwn_MinorTickLength.MaxValue = 100;
            this.updwn_MinorTickWidth.MaxValue = 100;
        }

        #region Public Properties

        public UpDown MinorTickLength

        {
            get
            {
                return this.updwn_MinorTickLength;
            }

            set
            {
                this.updwn_MinorTickLength = value;
            }
        }

        public UpDown MinorTickWidth
        {
            get
            {
                return this.updwn_MinorTickWidth;
            }

            set
            {
                this.updwn_MinorTickWidth = value;
            }
        }

        public ColorPicker MinorTickColor
        {
            get
            {
                return this.clrpkr_MinorTickColor;
            }

            set
            {
                this.clrpkr_MinorTickColor = value;
            }
        }

        public ComboBox MinorTickShape
        {
            get
            {
                return this.cmb_MinorTickShape;
            }

            set
            {
                this.cmb_MinorTickShape = value;
            }
        }

        public ComboBox MinorTickPlacement
        {
            get
            {
                return this.cmb_MinorTickPlacement;
            }

            set
            {
                this.cmb_MinorTickPlacement = value;
            }
        }

        #endregion
    }
}
