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
using Syncfusion.Windows.Reports.Wizard;

namespace Syncfusion.Windows.Reports.PropertyControls
{
    /// <summary>
    /// Interaction logic for GaugeFrameBorder.xaml
    /// </summary>
    public partial class GaugeFrameShadow : UserControl
    {
        public GaugeFrameShadow()
        {
            InitializeComponent();
            this.updwn_ShadowOffset.MinValue = 0;            
        }               

        #region Public Properties

        public UpDown ShadowOffset
        {
            get
            {
                return this.updwn_ShadowOffset;
            }

            set
            {
                this.updwn_ShadowOffset = value;
            }
        }

        #endregion
    }
}
