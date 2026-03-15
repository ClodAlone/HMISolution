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

# if  SILVERLIGHT
using Syncfusion.Windows.Shared.Controls;
# else
using Syncfusion.Windows.Shared;
# endif

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
# if SILVERLIGHT
    internal partial class PageSetupUI : Syncfusion.Windows.Tools.Controls.WindowControl
# else
    internal partial class PageSetupUI : ChromelessWindow
# endif
    {
        private Dictionary<string, PaperSize> pages = new Dictionary<string, PaperSize>();
        private double _pageWidth;
        private double _pageHeight;
        private double _leftMargin;
        private double _topMargin;
        private double _bottomMargin;
        private double _rightMargin;

#if SILVERLIGHT
        public bool DialogResult
        {
            get;
            set;
        }
#endif

        public bool IsInternalChange
        {
            get;
            set;
        }

        public double PageWidth
        {
            get
            {
                return _pageWidth;
            }
        }

        public double PageHeight
        {
            get
            {
                return _pageHeight;
            }
        }

        public double LeftMargin
        {
            get
            {
                return _leftMargin;
            }
        }

        public double TopMargin
        {
            get
            {
                return _topMargin;
            }
        }

        public double BottomMargin
        {
            get
            {
                return _bottomMargin;
            }
        }

        public double RightMargin
        {
            get
            {
                return _rightMargin;
            }
        }

        public PageSetupUI()
        {
            InitializeComponent();
            this.LoadPageDimention();
            this.Ok_button.Click += new RoutedEventHandler(Ok_button_Click);
            pagesize.SelectionChanged += new SelectionChangedEventHandler(pagesize_SelectionChanged);
            landscape.Checked += new RoutedEventHandler(Orientation_Checked);
            portrait.Checked += new RoutedEventHandler(Orientation_Checked);
            this.Default.Click += new RoutedEventHandler(Default_Click);

            foreach (string type in pages.Keys)
            {
                pagesize.Items.Add(type);
            }

            this.pagesize.SelectedIndex = 53;
        }

        void Default_Click(object sender, RoutedEventArgs e)
        {
            this.portrait.IsChecked = true;
            this.pagesize.SelectedIndex = 53;
            this.left.Value = 1;
            this.right.Value = 1;
            this.top.Value = 1;
            this.bottom.Value = 1;
        }

        void Orientation_Checked(object sender, RoutedEventArgs e)
        {
            double widthValue = this.pageWidth.Value.Value;
            double heightValue = this.pageHeight.Value.Value;
            var temp = heightValue;
            heightValue = widthValue;
            widthValue = temp;
            this.pageHeight.Value = heightValue;
            this.pageWidth.Value = widthValue;
        }

        void pagesize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsInternalChange)
            {
                this.portrait.IsChecked = true;
                double heightValue = this.pages[this.pagesize.SelectedItem.ToString()].Height;
                double widthValue = this.pages[this.pagesize.SelectedItem.ToString()].Width;
                this.pageHeight.Value = heightValue;
                this.pageWidth.Value = widthValue;
            }
        }

        void Ok_button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this._pageWidth = this.pageWidth.Value.Value * 96;
                this._pageHeight = this.pageHeight.Value.Value * 96;
                this._leftMargin = this.left.Value.Value * 96;
                this._topMargin = this.top.Value.Value * 96;
                this._bottomMargin = this.bottom.Value.Value * 96;
                this._rightMargin = this.right.Value.Value * 96;
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void LoadPageDimention()
        {
            pages.Add("10 x 11", new PaperSize(10, 11));
            pages.Add("10 x 14", new PaperSize(10, 14));
            pages.Add("11 x 17", new PaperSize(11, 17));
            pages.Add("12 x 11", new PaperSize(15, 11));
            pages.Add("6 3/4 Envelop", new PaperSize(3.63, 6.5));
            pages.Add("9 x 11", new PaperSize(9, 11));
            pages.Add("A2", new PaperSize(16.5, 23.4));
            pages.Add("A3", new PaperSize(11.7, 16.5));
            pages.Add("A3 Extra", new PaperSize(12.68, 17.52));
            pages.Add("A3 Rotated", new PaperSize(16.54, 11.69));
            pages.Add("A4", new PaperSize(8.27, 11.69));
            pages.Add("A4 Extra", new PaperSize(9.27, 12.69));
            pages.Add("A4 Plus", new PaperSize(8.27, 12.99));
            pages.Add("A4 Rotated", new PaperSize(11.69, 8.27));
            pages.Add("A5", new PaperSize(5.83, 8.27));
            pages.Add("A5 Extra", new PaperSize(6.85, 9.25));
            pages.Add("A5 Rotated", new PaperSize(8.27, 5.83));
            pages.Add("A6", new PaperSize(4.13, 5.83));
            pages.Add("B4(ISO)", new PaperSize(9.84, 13.9));
            pages.Add("B4(JIS)", new PaperSize(10.12, 14.33));
            pages.Add("B4(JIS)Rotated", new PaperSize(14.33, 10.12));
            pages.Add("B5(JIS)", new PaperSize(7.17, 10.12));
            pages.Add("B5(JIS)Rotated", new PaperSize(10.12, 7.17));
            pages.Add("B6(JIS)", new PaperSize(5.04, 7.17));
            pages.Add("B6(JIS)Rotated", new PaperSize(7.17, 5.04));
            pages.Add("C size sheet", new PaperSize(17, 22));
            pages.Add("D size sheet", new PaperSize(22, 34));
            pages.Add("E size sheet", new PaperSize(34, 44));
            pages.Add("Envolope", new PaperSize(4.33, 9.06));
            pages.Add("Envelope #9", new PaperSize(3.875, 8.875));
            pages.Add("Envelope #10", new PaperSize(4.125, 9.5));
            pages.Add("Envelope #11", new PaperSize(4.5, 10.375));
            pages.Add("Envelope #12", new PaperSize(4.75, 11));
            pages.Add("Envelope #14", new PaperSize(5, 11.5));
            pages.Add("Envelope DL", new PaperSize(4.33, 8.66));
            pages.Add("Envelope C3", new PaperSize(12.75, 18));
            pages.Add("Envelope C4", new PaperSize(9, 12.75));
            pages.Add("Envelope C5", new PaperSize(6.38, 9.02));
            pages.Add("Envelope C6", new PaperSize(4.5, 6.4));
            pages.Add("Envelope C65", new PaperSize(4.5, 9));
            pages.Add("Envelope B4", new PaperSize(9.85, 13.9));
            pages.Add("Envelope B5", new PaperSize(6.9, 9.85));
            pages.Add("Envelope B6", new PaperSize(6.9, 4.9));
            pages.Add("Envelope Monarch", new PaperSize(3.875, 7.5));
            pages.Add("Envelope Invite", new PaperSize(8.66, 8.66));
            pages.Add("Executive", new PaperSize(7.25, 10.5));
            pages.Add("Folio", new PaperSize(8.5, 13));
            pages.Add("German LegalFanfold", new PaperSize(8.5, 13));
            pages.Add("Japanese Postcard", new PaperSize(3.94, 5.83));
            pages.Add("Japanese Double Postcard", new PaperSize(7.87, 5.83));
            pages.Add("Ledger", new PaperSize(17, 11));
            pages.Add("Legal", new PaperSize(8.5, 14));
            pages.Add("Legal Extra", new PaperSize(9.5, 15));
            pages.Add("Letter", new PaperSize(8.5, 11));
            pages.Add("Letter Plus", new PaperSize(8.5, 12.69));
            pages.Add("Letter Extra", new PaperSize(9.5, 12));
            pages.Add("Note", new PaperSize(8.5, 11));
            pages.Add("Quarto", new PaperSize(8.46, 10.83));
            pages.Add("Statement", new PaperSize(5.5, 8.5));
            pages.Add("SuperA", new PaperSize(8.94, 14.02));
            pages.Add("SuperB", new PaperSize(8.94, 14.02));
            pages.Add("Tabloid", new PaperSize(11, 17));
            pages.Add("TabloidExtra", new PaperSize(12, 18));
        }
    }
    class PaperSize
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="PaperSize"/> class.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public PaperSize(double x, double y)
        {
            this.Width = x;
            this.Height = y;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        public double Width { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
        public double Height { get; set; }

        #endregion
    }
}
