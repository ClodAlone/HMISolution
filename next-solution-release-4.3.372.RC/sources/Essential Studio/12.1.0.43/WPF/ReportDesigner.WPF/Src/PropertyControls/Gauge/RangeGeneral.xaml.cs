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
    /// Interaction logic for RangeGeneral.xaml
    /// </summary>
    internal partial class RangeGeneral : UserControl
    {
        public RangeGeneral()
        {
            InitializeComponent();
            System.Globalization.NumberFormatInfo number = new System.Globalization.NumberFormatInfo();
            number.NumberDecimalDigits = 0;
            this.updwn_StartRange.NumberFormatInfo = number;
            this.updwn_EndRange.NumberFormatInfo = number;
            this.updwn_RangeStartWidth.NumberFormatInfo = number;
            this.updwn_RangeEndWidth.NumberFormatInfo = number;
            this.updwn_RangeDistanceScale.NumberFormatInfo = number;
            this.updwn_StartRange.MinValue = 0;
            this.updwn_EndRange.MinValue = 0;
            this.updwn_RangeStartWidth.MinValue = 0;
            this.updwn_RangeEndWidth.MinValue = 0;
            this.updwn_RangeStartWidth.MaxValue = 100;
            this.updwn_RangeEndWidth.MaxValue = 100;
            this.updwn_RangeDistanceScale.MinValue = 0;
            this.updwn_RangeDistanceScale.MaxValue = 100;
        }

        #region Public Properties

        public UpDown RangeStartValue
        {
            get
            {
                return this.updwn_StartRange;
            }

            set
            {
                this.updwn_StartRange = value;
            }
        }

        public UpDown RangeEndValue
        {
            get
            {
                return this.updwn_EndRange;
            }

            set
            {
                this.updwn_EndRange = value;
            }
        }

        public UpDown RangeStartWidth
        {
            get
            {
                return this.updwn_RangeStartWidth;
            }

            set
            {
                this.updwn_RangeStartWidth = value;
            }
        }

        public UpDown RangeEndWidth
        {
            get
            {
                return this.updwn_RangeEndWidth;
            }

            set
            {
                this.updwn_RangeEndWidth = value;
            }
        }

        public ColorPicker RangeColor
        {
            get
            {
                return this.clrpkr_RangeColor;
            }

            set
            {
                this.clrpkr_RangeColor = value;
            }
        }

        public ComboBox RangePlacement
        {
            get
            {
                return this.cmb_RangePlacement;
            }

            set
            {
                this.cmb_RangePlacement = value;
            }
        }

        public UpDown RangeDistanceFromScale
        {
            get
            {
                return this.updwn_RangeDistanceScale;
            }

            set
            {
                this.updwn_RangeDistanceScale = value;
            }
        }

        #endregion
    }
}
