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
using RESX = Syncfusion.Windows.Reports.Designer.Properties.Resources;
using Syncfusion.Windows.ReportDesigner.Resources;
using System.Globalization;

namespace Syncfusion.Windows.Reports.Designer.Dialogs
{
    /// <summary>
    /// Interaction logic for ScaleGeneral.xaml
    /// </summary>
    internal partial class ScaleGeneral : UserControl
    {
        private double oldMinimumValue = 0;
        private double oldMaximumValue = 100;

        public ScaleGeneral()
        {
            InitializeComponent();
            System.Globalization.NumberFormatInfo number = new System.Globalization.NumberFormatInfo();
            number.NumberDecimalDigits = 0;
            this.updwn_MinimumValue.NumberFormatInfo = number;
            this.updwn_MaximumValue.NumberFormatInfo = number;
            this.updwn_MultLabel.NumberFormatInfo = number;
            this.updwn_ScaleRadius.NumberFormatInfo = number;
            this.updwn_MinimumValue.MinValue = 0;
            this.updwn_MaximumValue.MinValue = 1;
            this.updwn_MultLabel.MinValue = 1;
            this.updwn_ScaleRadius.MinValue = 0;
            this.updwn_ScaleRadius.MaxValue = 100;
            this.updwn_MinimumValue.ValueChanged += new PropertyChangedCallback(updwn_MinimumValue_ValueChanged);
            this.updwn_MaximumValue.ValueChanged += new PropertyChangedCallback(updwn_MaximumValue_ValueChanged);
        }

        void updwn_MaximumValue_ValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (this.updwn_MaximumValue.Value <= this.updwn_MinimumValue.Value)
            {
                MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxScaleValuesLimit"), SR.GetString(CultureInfo.CurrentUICulture, "titleReportDesigner"));
                this.updwn_MaximumValue.Value = oldMaximumValue;
            }
            else
            {
                oldMaximumValue = (double)this.updwn_MaximumValue.Value;
            }
        }

        void updwn_MinimumValue_ValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (this.updwn_MaximumValue.Value <= this.updwn_MinimumValue.Value)
            {
                MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxScaleValuesLimit"), SR.GetString(CultureInfo.CurrentUICulture, "titleReportDesigner"));
                this.updwn_MinimumValue.Value = oldMinimumValue;
            }
            else
            {
                oldMinimumValue = (double)this.updwn_MinimumValue.Value;
            }
        }

        #region Public Properties

        public UpDown ScaleMinimumValue
        {
            get
            {
                return this.updwn_MinimumValue;
            }

            set
            {
                this.updwn_MinimumValue = value;
            }
        }

        public UpDown ScaleMaximumValue
        {
            get
            {
                return this.updwn_MaximumValue;
            }

            set
            {
                this.updwn_MaximumValue = value;
            }
        }

        public UpDown ScaleLabelsMultiple
        {
            get
            {
                return this.updwn_MultLabel;
            }

            set
            {
                this.updwn_MultLabel = value;
            }
        }

        public UpDown ScaleRadius
        {
            get
            {
                return this.updwn_ScaleRadius;
            }

            set
            {
                this.updwn_ScaleRadius = value;
            }
        }

        public Slider ScaleStartAngle
        {
            get
            {
                return this.sldr_StartAngle;
            }

            set
            {
                this.sldr_StartAngle = value;
            }
        }

        public Slider ScaleSweepAngle
        {
            get
            {
                return this.sldr_SweepAngle;
            }

            set
            {
                this.sldr_SweepAngle = value;
            }
        }

        #endregion
    }
}
