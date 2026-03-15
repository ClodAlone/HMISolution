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
using Syncfusion.Windows.Reports.Wizard;

namespace Syncfusion.Windows.Reports.PropertyControls
{
    /// <summary>
    /// Interaction logic for GaugeFrameFill.xaml
    /// </summary>
    public partial class GaugeFrameFill : UserControl
    {
        public GaugeFrameFill()
        {
            InitializeComponent();            
        }   

        #region Public Properties

        public ColorPicker FrameFill
        {
            get
            {
                return this.clrpkr_FrameColor;
            }
            set
            {
                this.clrpkr_FrameColor = value;
            }
        }

        #endregion
        
    }
}
