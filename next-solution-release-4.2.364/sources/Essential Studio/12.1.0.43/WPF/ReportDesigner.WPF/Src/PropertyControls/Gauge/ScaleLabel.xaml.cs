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
using System.Reflection;
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.Windows.Reports.Designer.Dialogs;

namespace Syncfusion.Windows.Reports.Designer.Dialogs
{
    /// <summary>
    /// Interaction logic for ScaleLabel.xaml
    /// </summary>
    internal partial class ScaleLabel : UserControl
    {
        public ScaleLabel()
        {
            InitializeComponent();
            System.Globalization.NumberFormatInfo number = new System.Globalization.NumberFormatInfo();
            number.NumberDecimalDigits = 0;
            this.updwn_DistanceScale.NumberFormatInfo = number;
            this.updwn_LabelFont.NumberFormatInfo = number;
            this.updwn_DistanceScale.MinValue = 0;
            this.updwn_DistanceScale.MaxValue = 100;
            this.updwn_LabelFont.MinValue = 0;
            this.updwn_LabelFont.MaxValue = 72;
        }

        #region Public Properties

        public ComboBox LabelPlacement
        {
            get
            {
                return this.cmb_LabelPlacement;
            }

            set
            {
                this.cmb_LabelPlacement = value;
            }
        }

        public UpDown LabelFont
        {
            get
            {
                return this.updwn_LabelFont;
            }

            set
            {
                this.updwn_LabelFont = value;
            }
        }

        public Slider LabelAngle
        {
            get
            {
                return this.sldr_LabelAngle;
            }

            set
            {
                this.sldr_LabelAngle = value;
            }
        }

        public UpDown LabelDistanceFromScale
        {
            get
            {
                return this.updwn_DistanceScale;
            }

            set
            {
                this.updwn_DistanceScale = value;
            }
        }

        public ColorPicker LabelColor
        {
            get
            {
                return this.clrpkr_LabelColor;
            }

            set
            {
                this.clrpkr_LabelColor = value;
            }
        }

        #endregion
    }
}
