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
using Syncfusion.Windows.Reports.Designer.Dialogs;
using System.ComponentModel;
using Syncfusion.Windows.Reports.Designer.Editors;

namespace Syncfusion.Windows.Reports.Designer.Dialogs
{
    /// <summary>
    /// Interaction logic for LayerGeneral.xaml
    /// </summary>
#if SyncfusionFramework4_0
        [DesignTimeVisible(false)]
#endif
    internal partial class LayerGeneral
        : UserControl
    {
        public LayerGeneral()
        {
            InitializeComponent();
            this.btn_Browse.Click += new RoutedEventHandler(btn_Browse_Click);
        }

        private void btn_Browse_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Filter = "Shape Files(*.shp)|*.shp";

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.txt_ShapeFile.Text = openFileDialog.FileName;
            }
        }

        #region Public Properties
        public new System.Windows.Controls.TextBox Name
        {
            get
            {
                return this.txt_ShapeFile;
            }
            set
            {
                this.txt_ShapeFile = value;
            }
        }

        public System.Windows.Controls.Button Browse
        {
            get
            {
                return this.btn_browse;
            }
            set
            {
                this.btn_browse = value;
            }
        }
        #endregion
    }
}
