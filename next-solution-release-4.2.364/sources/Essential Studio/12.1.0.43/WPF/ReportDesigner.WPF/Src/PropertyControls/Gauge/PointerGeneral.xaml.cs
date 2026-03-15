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
using Syncfusion.Windows.Gauge;

namespace Syncfusion.Windows.Reports.Designer.Dialogs
{
    /// <summary>
    /// Interaction logic for PointerGeneral.xaml
    /// </summary>
    internal partial class PointerGeneral : UserControl
    {
        public PointerGeneral()
        {
            InitializeComponent();
            System.Globalization.NumberFormatInfo number = new System.Globalization.NumberFormatInfo();
            number.NumberDecimalDigits = 0;
           // this.PointerValue.NumberFormatInfo = number;
            this.updwn_PointerWidth.NumberFormatInfo = number;
            this.updwn_CapRadius.NumberFormatInfo = number;
           // this.updwn_PointerValue.MinValue = 0;
            this.updwn_PointerWidth.MinValue = 0;
            this.updwn_CapRadius.MinValue = 0;
            this.cmb_PointerType.SelectionChanged += new SelectionChangedEventHandler(cmb_PointerType_SelectionChanged);
        }

        void cmb_PointerType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (this.cmb_PointerType.SelectedIndex)
            {
                case 0:
                    {
                        this.cmb_NeedleType.IsEnabled = true;
                        this.cmb_MarkerPlacement.IsEnabled = false;
                        this.cmb_MarkerStyle.IsEnabled = false;
                        this.updwn_CapRadius.IsEnabled = true;
                        this.clrpkr_CapColor.IsEnabled = true;
                        break;
                    }

                case 1:
                    {
                        this.cmb_NeedleType.IsEnabled = false;
                        this.cmb_MarkerPlacement.IsEnabled = true;
                        this.cmb_MarkerStyle.IsEnabled = true;
                        this.updwn_CapRadius.IsEnabled = false;
                        this.clrpkr_CapColor.IsEnabled = false;
                      //  this.updwn_PointerValue.IsEnabled = true;
                        break;
                    }

                case 2:
                    {
                        this.cmb_NeedleType.IsEnabled = false;
                        this.cmb_MarkerPlacement.IsEnabled = false;
                        this.cmb_MarkerStyle.IsEnabled = false;
                      //  this.updwn_PointerValue.IsEnabled = false;
                        this.updwn_CapRadius.IsEnabled = false;
                        this.clrpkr_CapColor.IsEnabled = false;
                        break;
                    }
            }
        }

        #region Public Properties

        public TextBox PointerValue
        {
            get
            {
                return this.pointervalue;
            }

            set
            {
                this.pointervalue = value;
            }
        }

        public UpDown PointerWidth
        {
            get
            {
                return this.updwn_PointerWidth;
            }

            set
            {
                this.updwn_PointerWidth = value;
            }
        }

        public ComboBox PointerType
        {
            get
            {
                return this.cmb_PointerType;
            }

            set
            {
                this.cmb_PointerType = value;
            }
        }

        public ComboBox NeedleType
        {
            get
            {
                return this.cmb_NeedleType;
            }

            set
            {
                this.cmb_NeedleType = value;
            }
        }

        public ComboBox MarkerStyle
        {
            get
            {
                return this.cmb_MarkerStyle;
            }

            set
            {
                this.cmb_MarkerStyle = value;
            }
        }

        public ComboBox PointerPlacement
        {
            get
            {
                return this.cmb_MarkerPlacement;
            }

            set
            {
                this.cmb_MarkerPlacement = value;
            }
        }

        public UpDown CapRadius
        {
            get
            {
                return this.updwn_CapRadius;
            }

            set
            {
                this.updwn_CapRadius = value;
            }
        }

        public ColorPicker CapColor
        {
            get
            {
                return this.clrpkr_CapColor;
            }

            set
            {
                this.clrpkr_CapColor = value;
            }
        }

        #endregion

        
    }
}
