#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Syncfusion.Windows.Controls.Spreadsheet.Commands
{
    /// <summary>
    /// Interaction logic for GroupOptionsWindow.xaml
    /// </summary>
    [DesignTimeVisible(false)]
    public partial class GroupOptionsWindow : Window,INotifyPropertyChanged
    {
        public GroupOptionsWindow()
        {
            InitializeComponent();

            if (Application.Current != null)
            {
                mainWindow = Application.Current.MainWindow as Window;

                if (mainWindow != null)
                    this.Owner = mainWindow;
            }
            this.Loaded += (s, e) =>
            {
                Syncfusion.Windows.Shared.SkinStorage.SetVisualStyle(this, this.AssociatedSpreadsheet.VisualStyle);
            };
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            IconHelper.RemoveIcon(this);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
            else if (e.Key == Key.Enter)
                OnOkButtonClick(this, new RoutedEventArgs());
            base.OnKeyDown(e);
        }

        private void OnOkButtonClick(object sender, RoutedEventArgs e)
        {
            Close();

            if (IsSummaryRowBelow != this.AssociatedSpreadsheet.ExcelProperties.WorkBook.ActiveSheet.PageSetup.IsSummaryRowBelow)
            {
                this.AssociatedSpreadsheet.ExcelProperties.WorkBook.ActiveSheet.PageSetup.IsSummaryRowBelow = IsSummaryRowBelow;
                this.AssociatedSpreadsheet.RefreshRowGroupPanel();
            }
            else if(IsSummaryColumnAtRight!=this.AssociatedSpreadsheet.ExcelProperties.WorkBook.ActiveSheet.PageSetup.IsSummaryColumnRight)
            {
                this.AssociatedSpreadsheet.ExcelProperties.WorkBook.ActiveSheet.PageSetup.IsSummaryColumnRight = IsSummaryColumnAtRight;
                this.AssociatedSpreadsheet.RefreshColumnGroupPanel();
            }
            
        }

        private void OnCancelButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        internal Window mainWindow
        {
            get;
            private set;
        }

        private SpreadsheetControl _associatedExcelEditor;
        public SpreadsheetControl AssociatedSpreadsheet
        {
            get { return _associatedExcelEditor; }
            set
            {
                _associatedExcelEditor = value;
            }
        }

        private bool isSummaryRowBelow;
        public bool IsSummaryRowBelow
        {
            get { return isSummaryRowBelow; }
            set 
            {
                isSummaryRowBelow = value;
                this.OnPropertyChanged("IsSummaryRowBelow");
            }
        }

        private bool isSummaryColumnAtRight;
        public bool IsSummaryColumnAtRight
        {
            get { return isSummaryColumnAtRight; }
            set 
            {
                isSummaryColumnAtRight = value;
                this.OnPropertyChanged("IsSummaryColumnAtRight");
            }
        }


        private void OnPropertyChanged(string PropertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
