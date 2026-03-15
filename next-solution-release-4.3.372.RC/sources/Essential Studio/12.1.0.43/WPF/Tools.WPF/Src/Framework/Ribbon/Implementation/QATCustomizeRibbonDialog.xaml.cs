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
using System.Collections.ObjectModel;

namespace Syncfusion.Windows.Tools.Controls
{   
    /// <summary>
    /// Interaction logic for QATCustomizeRibbonDialog.xaml
    /// </summary>
    public partial class QATCustomizeRibbonDialog : Window
    {
        public QATCustomizeRibbonDialog()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        RibbonTabs ribbonTab;
        RibbonBars ribbonBar;
        IRibbonControl control;

        public QATCustomizeRibbonDialog(RibbonTabs tab)
        {
            InitializeComponent();
            ribbonTab = tab;
            this.DataContext = this;
        }

        public QATCustomizeRibbonDialog(RibbonBars bar)
        {
            InitializeComponent();
            ribbonBar = bar;
            isRibbonBar = true;
            this.DataContext = this;
        }

        public QATCustomizeRibbonDialog(IRibbonControl ribbonControl)
        {
            InitializeComponent();
            this.control = ribbonControl;
            isRibbonControl = true;
            this.DataContext = this;        
        }

        public string DisplayName { get; set; }
        bool isRibbonBar = false;
        bool isRibbonControl = false;
      
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (isRibbonBar)
            {
                ribbonBar.Header = this.renameTextBox.Text;
                ribbonBar.IsRenamed = true;
            }
            else if (isRibbonControl)
                (control as RibbonButton).Label = this.renameTextBox.Text;
            else
                ribbonTab.Caption = this.renameTextBox.Text;

            CloseWindow();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            CloseWindow();
        }

        void CloseWindow()
        {
            this.Close();
        }
       
        
    }
}
