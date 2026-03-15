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
    /// Interaction logic for ScaleMajorTick.xaml
    /// </summary>
    internal partial class ScaleMajorTick : UserControl
    {
        public ScaleMajorTick()
        {
            InitializeComponent();
            System.Globalization.NumberFormatInfo number = new System.Globalization.NumberFormatInfo();
            number.NumberDecimalDigits = 0;
            this.updwn_MajorTickLength.NumberFormatInfo = number;
            this.updwn_MajorTickWidth.NumberFormatInfo = number;
            this.updwn_MajorTickLength.MinValue = 0;
            this.updwn_MajorTickWidth.MinValue = 0;
            this.updwn_MajorTickLength.MaxValue = 100;
            this.updwn_MajorTickWidth.MaxValue = 100;
        }

        #region Public Properties

        public UpDown MajorTickLength
        {
            get
            {
                return this.updwn_MajorTickLength;
            }

            set
            {
                this.updwn_MajorTickLength = value;
            }
        }

        public UpDown MajorTickWidth
        {
            get
            {
                return this.updwn_MajorTickWidth;
            }

            set
            {
                this.updwn_MajorTickWidth = value;
            }
        }

        public ColorPicker MajorTickColor
        {
            get
            {
                return this.clrpkr_MajorTickColor;
            }

            set
            {
                this.clrpkr_MajorTickColor = value;
            }
        }

        public ComboBox MajorTickShape
        {
            get
            {
                return this.cmb_MajorTickShape;
            }

            set
            {
                this.cmb_MajorTickShape = value;
            }
        }

        public ComboBox MajorTickPlacement
        {
            get
            {
                return this.cmb_MajorTickPlacement;
            }

            set
            {
                this.cmb_MajorTickPlacement = value;
            }
        }

        #endregion
    }
}
