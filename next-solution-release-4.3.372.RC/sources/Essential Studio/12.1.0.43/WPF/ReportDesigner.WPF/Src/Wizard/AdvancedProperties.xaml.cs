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
using System.ComponentModel;

namespace Syncfusion.Windows.Reports.Designer.Dialogs
{
    /// <summary>
    /// Interaction logic for Essential WPF RibbonWindow1.xaml
    /// </summary>
#if SyncfusionFramework4_0
        [DesignTimeVisible(false)]
#endif
    internal partial class AdvancedProperties
        : ChromelessWindow
    {
        #region Private Properties
        private ConnectionProperties ConnectionProperties { get; set; }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Windows.Reports.Designer.Dialogs.AdvancedProperties">AdvancedProperties</see> class. 
        /// </summary>
        /// <remarks></remarks>
        public AdvancedProperties()
        {
            InitializeComponent();
        }

        public AdvancedProperties(ConnectionProperties connectionProperties)
        {
            InitializeComponent();
            this.ConnectionProperties = connectionProperties;
            System.Windows.Forms.PropertyGrid propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            propertyGrid1.CommandsVisibleIfAvailable = true;
            propertyGrid1.Location = new System.Drawing.Point(10, 20);
            propertyGrid1.Size = new System.Drawing.Size(400, 300);
            propertyGrid1.TabIndex = 1;
            propertyGrid1.Text = "Property Grid";
            propertyGrid1.SelectedObject = connectionProperties;
            //this.Content = propertyGrid1;
            //grd_AdvancedProperties.Children.Add(propertyGrid1);
            //Grid.SetRow(propertyGrid1, 1);
        }
    }
}
