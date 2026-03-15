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
using Syncfusion.Windows.Reports.Designer.Editors;

namespace Syncfusion.Windows.Reports.Designer.Dialogs
{
    /// <summary>
    /// Interaction logic for TextBoxAlignment.xaml
    /// </summary>
#if SyncfusionFramework4_0
        [DesignTimeVisible(false)]
#endif
    internal partial class TextBoxAlignment
        : UserControl
    {
        #region Constructor
        public TextBoxAlignment()
        {
            InitializeComponent();
            System.Globalization.NumberFormatInfo number = new System.Globalization.NumberFormatInfo();
            number.NumberDecimalDigits = 0;
            number.PercentSymbol = "pt";
            //this.updwn_Left.NumberFormatInfo = number;
            //this.updwn_Left.MinValue = 0;
        }
        #endregion

        #region Event Declaration
        private void UpDown_StepChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            System.Globalization.NumberFormatInfo number = new System.Globalization.NumberFormatInfo();
            number.NumberDecimalDigits = 0;
            number.PercentSymbol = "pt";
            //this.updwn_Top.NumberFormatInfo = number;

        }

        private void updwn_Top_ValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }
        #endregion

        #region Public Properties
        public ExpressionComboBox HorizontalValue
        {
            get
            {
                return this.cmb_HorizontalValue;
            }
            set
            {
                this.cmb_HorizontalValue = value;
            }
        }

        public ExpressionComboBox VerticalValue
        {
            get
            {
                return this.cmb_VerticalValue;
            }
            set
            {
                this.cmb_VerticalValue = value;
            }
        }

        public ExpressionComboBox Left
        {
            get
            {
                return this.updwn_Left;
            }
            set
            {
                this.updwn_Left = value;
            }
        }

        public ExpressionComboBox Right
        {
            get
            {
                return this.updwn_Right;
            }
            set
            {
                this.updwn_Right = value;
            }
        }

        public ExpressionComboBox Top
        {
            get
            {
                return this.updwn_Top;
            }
            set
            {
                this.updwn_Top = value;
            }
        }

        public ExpressionComboBox Bottom
        {
            get
            {
                return this.updwn_Bottom;
            }
            set
            {
                this.updwn_Bottom = value;
            }
        }

        #endregion
    }
}
