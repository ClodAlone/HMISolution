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
using Utilities;
using Utilities.WPF;
using DevExpress.XtraReports.UI;
using DevExpress.XtraPrinting;

namespace ReportManager
{
    /// <summary>
    /// Interaction logic for Loading.xaml
    /// </summary>
    public partial class PrintingProgressBar : UserControl
    {
        #region Declarations

        Window ParentWnd;

        #endregion

        #region Constructors

        public PrintingProgressBar()
        {
            InitializeComponent();

            this.Loaded += (sender, o) => { ParentWnd = this.FindParent<Window>(); };
        }

        #endregion

        #region Properties

        XtraReport report;
        public XtraReport Report
        { 
            set
            {
                if (report == null)
                {
                    report = value;

                    report.PrintingSystem.PrintProgress += (sender, e) => 
                    {
                        if (!ParentWnd.IsVisible)
                            ParentWnd.Show();

                        busyContent.Text = String.Format(
                            Properties.Resources.PrintingPages, e.PageIndex,
                            (e.PageSettings.PrinterSettings.ToPage - e.PageSettings.PrinterSettings.FromPage));
                        
                        busyContent.Refresh();
                    };
                }
                
            }
        }

        #endregion

    }
}
