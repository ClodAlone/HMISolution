#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.ComponentModel;
using System.Windows;
#if WinRT
using Windows.UI.Xaml;
#endif

namespace Syncfusion.UI.Xaml.Grid
{

    public class PrintSettings : INotifyPropertyChanged
    {
        private bool allowColumnWidthFitToPrintPage = true;
        private bool allowRepeatHeaders = true;
        private double printPageHeight = 1122.52;
        private double printPageWidth = 793.70;
        private double printPageHeaderHeight;
        private double printPageFooterHeight;
        private Thickness printPageMargin = new Thickness(96);
        private DataTemplate printPageHeaderTemplate;
        private DataTemplate printPageFooterTemplate;
        private PrintManagerBase printManagerBase;
#if WPF
        private Style printPreviewWindowStyle;
#endif

        #region Ctor

        public PrintSettings()
        {

        }

        #endregion 

        #region Public Properties

        /// <summary>
        /// Gets or Sets the PrintManagerBase for Printing. 
        /// PrintManagerBase can be override for custamization and it need to be assign in this.
        /// </summary>
        public PrintManagerBase PrintManagerBase
        {
            get { return printManagerBase; }
            set
            {
                printManagerBase = value;
                OnPropertyChanged("PrintManagerBase");
            }
        }

        /// <summary>
        /// Gets or Sets the height of the printable page.
        /// </summary>
        public double PrintPageHeight
        {
            get { return printPageHeight; }
            set
            {
                printPageHeight = value;
                OnPropertyChanged("PrintPageHeight");
            }
        }

        /// <summary>
        /// Gets or Sets the Width of printable page.
        /// </summary>
        public double PrintPageWidth
        {
            get { return printPageWidth; }
            set
            {
                printPageWidth = value;
                OnPropertyChanged("PrintPageWidth");
            }
        }

        /// <summary>
        /// Gets or Sets the Height of Header Template for the printable page.
        /// </summary>
        public double PrintPageHeaderHeight
        {
            get { return printPageHeaderHeight; }
            set
            {
                printPageHeaderHeight = value;
                OnPropertyChanged("PrintPageHeaderHeight");
            }
        }

        /// <summary>
        /// Gets or Sets the Height of Footer Template for the printable page.
        /// </summary>
        public double PrintPageFooterHeight
        {
            get { return printPageFooterHeight; }
            set
            {
                printPageFooterHeight = value;
                OnPropertyChanged("PrintPageFooterHeight");
            }
        }

        /// <summary>
        /// Gets or Sets the Allow Column Width Fit To Printable page.
        /// Width of the page exclude margins, shared by the number of columns.
        /// </summary>
        public bool AllowColumnWidthFitToPrintPage
        {
            get { return allowColumnWidthFitToPrintPage; }
            set
            {
                allowColumnWidthFitToPrintPage = value;
                OnPropertyChanged("AllowColumnWidthFitToPrintPage");
            }
        }

        /// <summary>
        /// Gets or Sets the AllowRepeatHeaders of the Print page.
        /// Determins the headers to repeat for all the pages.
        /// </summary>
        public bool AllowRepeatHeaders
        {
            get { return allowRepeatHeaders; }
            set
            {
                allowRepeatHeaders = value;
                OnPropertyChanged("AllowRepeatHeaders");
            }
        }

        /// <summary>
        /// Gets or Sets the Print Page Margins
        /// </summary>
        public Thickness PrintPageMargin
        {
            get { return printPageMargin; }
            set
            {
                printPageMargin = value;
                OnPropertyChanged("PrintPageMargin");
            }
        }

        /// <summary>
        /// Gets or Sets the Header Template for the Printable page.
        /// </summary>
        public DataTemplate PrintPageHeaderTemplate
        {
            get { return printPageHeaderTemplate; }
            set
            {
                printPageHeaderTemplate = value;
                OnPropertyChanged("PrintPageHeaderTemplate");
            }
        }

        /// <summary>
        /// Gets or sets the Footer Template for the Printable Pa
        /// </summary>
        public DataTemplate PrintPageFooterTemplate
        {
            get { return printPageFooterTemplate; }
            set
            {
                printPageFooterTemplate = value;
                OnPropertyChanged("PrintPageFooterTemplate");
            }
        }

#if WPF

        /// <summary>
        /// Gets or Sets the Chromeless Window Style for the Preview window.
        /// </summary>
        public Style PrintPreviewWindowStyle
        {
            get { return printPreviewWindowStyle; }
            set
            {
                printPreviewWindowStyle = value;
                OnPropertyChanged("PrintPreviewWindowStyle");
            }
        }

#endif

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

    }
    
}

