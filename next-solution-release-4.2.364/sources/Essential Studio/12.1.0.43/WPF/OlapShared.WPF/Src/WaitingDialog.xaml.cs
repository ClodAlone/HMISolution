//-------------------------------------------------------------------------------------------------
// <copyright file="WaitingDialog.xaml.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System.Windows;

namespace Syncfusion.Windows.Shared.Olap
{
    /// <summary>
    /// Interaction logic for WaitingDialog.xaml
    /// </summary>
    public partial class WaitingDialog : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WaitingDialog"/> class.
        /// </summary>
        public WaitingDialog()
        {
            InitializeComponent();
            this.ShowInTaskbar = false;
            this.WindowStyle = WindowStyle.None;
        }
    }
}
