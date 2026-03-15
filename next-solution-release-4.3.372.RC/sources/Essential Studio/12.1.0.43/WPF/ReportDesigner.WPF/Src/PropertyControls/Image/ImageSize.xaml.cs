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
    /// Interaction logic for ImageSize.xaml
    /// </summary>
#if SyncfusionFramework4_0
        [DesignTimeVisible(false)]
#endif
    internal partial class ImageSize
        : UserControl
    {
        #region Constructor
        public ImageSize()
        {
            InitializeComponent();
            InitializeComponent();
            System.Globalization.NumberFormatInfo number = new System.Globalization.NumberFormatInfo();
            number.NumberDecimalDigits = 0;
            number.PercentSymbol = "pt";
           // this.updwn_Top.NumberFormatInfo = number;
            //this.updwn_Bottom.NumberFormatInfo = number;
            //this.updwn_Left.NumberFormatInfo = number;
            //this.updwn_Right.NumberFormatInfo = number;
            //this.updwn_Top.MinValue = 0;
            //this.updwn_Bottom.MinValue = 0;
            //this.updwn_Left.MinValue = 0;
            //this.updwn_Right.MinValue = 0;
        }
        #endregion

        #region Public Properties
        public RadioButton OriginalSize
        {
            get
            {
                return this.rbtn_OriginalSize;
            }
            set
            {
                this.rbtn_OriginalSize = value;
            }
        }

        public RadioButton FitToSize
        {
            get
            {
                return this.rbtn_FitToSize;
            }
            set
            {
                this.rbtn_FitToSize = value;
            }
        }

        public RadioButton FitProportional
        {
            get
            {
                return this.rbtn_FitProportional;
            }
            set
            {
                this.rbtn_FitProportional = value;
            }
        }

        public new RadioButton Clip
        {
            get
            {
                return this.rbtn_Clip;
            }
            set
            {
                this.rbtn_Clip = value;
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
        #endregion
    }
}
