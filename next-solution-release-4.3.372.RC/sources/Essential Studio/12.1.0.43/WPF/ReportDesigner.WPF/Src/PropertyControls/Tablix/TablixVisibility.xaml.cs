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
using System.ComponentModel;

namespace Syncfusion.Windows.Reports.Designer.Dialogs
{
    /// <summary>
    /// Interaction logic for TablixVisibility.xaml
    /// </summary>
#if SyncfusionFramework4_0
        [DesignTimeVisible(false)]
#endif
    internal partial class TablixVisibility
        : UserControl
    {
        #region Constructor
        public TablixVisibility()
        {
            InitializeComponent();
        }
        #endregion

        #region Public Properties
        public RadioButton Show
        {
            get
            {
                return this.rbtn_Show;
            }
            set
            {
                this.rbtn_Show = value;
            }
        }

        public RadioButton Hide
        {
            get
            {
                return this.rbtn_Hide;
            }
            set
            {
                this.rbtn_Hide = value;
            }
        }
        #endregion
    }
}
