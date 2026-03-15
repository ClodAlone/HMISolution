//-------------------------------------------------------------------------------------------------
// <copyright file="LoadingIndicator.xaml.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------
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

namespace Syncfusion.Windows.Reports.Viewer.Utils
{
    /// <summary>
    /// Represents the loading indicator.
    /// </summary>
#if SyncfusionFramework4_0
        [DesignTimeVisible(false)]
#endif
    public partial class LoadingIndicator
        : UserControl
    {

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadingIndicator"/> class.
        /// </summary>
        public LoadingIndicator()
        {
            InitializeComponent();
        }

        #endregion

    }
}
