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
using System.Windows.Shapes;
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Tools.Controls.Resources;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Interaction logic for QATResetDialog.xaml
    /// </summary>
    public partial class QATResetDialog : ChromelessWindow
    {
        ResourceWrapper wrapper = new ResourceWrapper();

        /// <summary>
        /// Initializes a new instance of the <see cref="QATResetDialog"/> class.
        /// </summary>
        public QATResetDialog()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(QATResetDialog_Loaded);
        }

        /// <summary>
        /// Handles the Loaded event of the QATResetDialog control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void QATResetDialog_Loaded(object sender, RoutedEventArgs e)
        {
            this.Title = wrapper.QATResetTitle;
        }

        /// <summary>
        /// Handles the Click event of the Ok control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        /// <summary>
        /// Handles the Click event of the Cancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
