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
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Tools.Controls;

namespace Syncfusion.Windows.Reports.Viewer.Dialogs
{
    /// <summary>
    /// Interaction logic for PageSetupDialog.xaml
    /// </summary>

#if SILVERLIGHT
    internal partial class PageSetupDialog : WindowControl
#else
    internal partial class PageSetupDialog : ChromelessWindow
#endif
    {
        #region private Members

        private Dictionary<string, PaperSize> paperSizes = new Dictionary<string, PaperSize>();
        private Thickness Margins { get; set; }
        private PaperSize PaperSize { get; set; }

        #endregion

        # region Properties

        /// <summary>
        /// Gets the Changes are valid.
        /// </summary>
        /// <value>The bool value.</value>
        public bool IsChangesValid
        {
            set;
            get;
        }

        /// <summary>
        /// Gets the left margin.
        /// </summary>
        /// <value>The left margin.</value>
        public double LeftMargin
        {
            get
            {
                return double.Parse(this.updwn_Left.Value.ToString()) * 96;
            }
        }

        /// <summary>
        /// Gets the right margin.
        /// </summary>
        /// <value>The right margin.</value>
        public double RightMargin
        {
            get
            {
                return double.Parse(this.updwn_Right.Value.ToString()) * 96;
            }
        }

        /// <summary>
        /// Gets the top margin.
        /// </summary>
        /// <value>The top margin.</value>
        public double TopMargin
        {
            get
            {
                return double.Parse(this.updwn_Top.Value.ToString()) * 96;
            }
        }

        /// <summary>
        /// Gets the bottom margin.
        /// </summary>
        /// <value>The bottom margin.</value>
        public double BottomMargin
        {
            get
            {
                return double.Parse(this.updwn_Bottom.Value.ToString()) * 96;
            }
        }

        /// <summary>
        /// Gets or sets the width of the page.
        /// </summary>
        /// <value>The width of the page.</value>
        public double PageWidth
        {
            get
            {
                return double.Parse(this.updwn_Width.Value.ToString()) * 96;
            }
            set
            {
                this.updwn_Width.Value = value;
            }
        }

        /// <summary>
        /// Gets or sets the height of the page.
        /// </summary>
        /// <value>The height of the page.</value>
        public double PageHeight
        {
            get
            {
                return double.Parse(this.updwn_Height.Value.ToString()) * 96;
            }
            set
            {
                this.updwn_Height.Value = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the dimention.
        /// </summary>
        /// <value>The name of the dimention.</value>
        public string PaperName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the type of the page.
        /// </summary>
        /// <value>The type of the page.</value>
        public string PaperType
        {
            get;
            set;
        }

        # endregion


        public PageSetupDialog(Thickness margins, PaperSize paperSize, string paperName, string PaperType)
        {
            InitializeComponent();
            this.Margins = margins;
            this.PaperSize = paperSize;
            this.PaperName = paperName;
            this.PaperType = PaperType;
            this.Loaded += new RoutedEventHandler(PageSetupDialog_Loaded);
        }

        void PageSetupDialog_Loaded(object sender, RoutedEventArgs e)
        {
            this.LoadPageValues();
            this.SetPageValues();

            this.KeyDown += new KeyEventHandler(PageSetupDialog_KeyDown);
            this.btn_Ok.Click += new RoutedEventHandler(btn_Ok_Click);
            this.btn_Cancel.Click += new RoutedEventHandler(btn_Cancel_Click);
            this.rbtn_landscape.Checked += new RoutedEventHandler(OrientationChanged);
            this.rbtn_portrait.Checked += new RoutedEventHandler(OrientationChanged);
            this.btn_setDefault.Click += new RoutedEventHandler(btn_SetDefault_Click);
            this.cmb_PageSize.SelectionChanged += new SelectionChangedEventHandler(PageSizeChanged);
        }

        #region event Methods

        void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.IsChangesValid = false;
            this.Close();
        }

        void btn_Ok_Click(object sender, RoutedEventArgs e)
        {
            string type = this.cmb_PageSize.SelectedItem.ToString();
            this.PageWidth = double.Parse(this.updwn_Width.Value.ToString());
            this.PageHeight = double.Parse(this.updwn_Height.Value.ToString());
            this.PaperType = (this.rbtn_portrait.IsChecked == true) ? "Portrait" : "LandScape";

            if ((LeftMargin + RightMargin) >= this.PageWidth || (TopMargin + BottomMargin) >= PageHeight)
            {
                MessageBox.Show(Syncfusion.Windows.Reports.Viewer.Resources.SR.GetString(System.Globalization.CultureInfo.CurrentUICulture, "msgBoxMargin"),
                            Syncfusion.Windows.Reports.Viewer.Resources.SR.GetString(System.Globalization.CultureInfo.CurrentUICulture, "PageSetup"), MessageBoxButton.OK);
            }
            else
            {
                this.IsChangesValid = this.CheckChanges();
                this.PaperName = type;
                this.Close();
            }
        }

        void btn_SetDefault_Click(object sender, RoutedEventArgs e)
        {
            if (this.rbtn_landscape.IsChecked == true)
            {
                this.updwn_Height.Value = paperSizes["A4"].Width;
                this.updwn_Width.Value = paperSizes["A4"].Height;
            }
            else
            {
                this.updwn_Height.Value = paperSizes["A4"].Height;
                this.updwn_Width.Value = paperSizes["A4"].Width;
            }
            this.cmb_PageSize.SelectedIndex = 1;
        }

        void OrientationChanged(object sender, RoutedEventArgs e)
        {
            var temp = this.updwn_Height.Value;
            this.updwn_Height.Value = this.updwn_Width.Value;
            this.updwn_Width.Value = temp;
            if (rbtn_landscape.IsChecked == true)
            {
                var temp1 = this.updwn_Left.Value;
                this.updwn_Left.Value = this.updwn_Top.Value;
                var temp2 = this.updwn_Bottom.Value;
                this.updwn_Bottom.Value = temp1;
                temp1 = this.updwn_Right.Value;
                this.updwn_Right.Value = temp2;
                this.updwn_Top.Value = temp1;
            }
            else
            {
                var temp1 = this.updwn_Right.Value;
                this.updwn_Right.Value = this.updwn_Top.Value;
                var temp2 = this.updwn_Bottom.Value;
                this.updwn_Bottom.Value = temp1;
                temp1 = this.updwn_Left.Value;
                this.updwn_Left.Value = temp2;
                this.updwn_Top.Value = temp1;
            }
        }

        void PageSizeChanged(object sender, SelectionChangedEventArgs e)
        {
            string type = (sender as ComboBox).SelectedValue.ToString();

            if (type.ToLower() == "custom")
            {
#if SILVERLIGHT
                this.updwn_Height.IsEnabled = true;
                this.updwn_Width.IsEnabled = true;
#else
                this.stpnl_CustomSize.IsEnabled = true;
#endif
            }
            else
            {
#if SILVERLIGHT
                this.updwn_Height.IsEnabled = false;
                this.updwn_Width.IsEnabled = false;
#else
                this.stpnl_CustomSize.IsEnabled = false;
#endif
                if (this.rbtn_landscape.IsChecked == true)
                {
                    this.updwn_Height.Value = paperSizes[type].Width;
                    this.updwn_Width.Value = paperSizes[type].Height;
                }
                else
                {
                    this.updwn_Height.Value = paperSizes[type].Height;
                    this.updwn_Width.Value = paperSizes[type].Width;
                }
            }
        }

        void PageSetupDialog_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.IsChangesValid = false;
                this.Close();
            }
        }

        #endregion

        #region Helper Methods

        private bool CheckChanges()
        {
            if (this.Margins.Left != (this.LeftMargin) || this.Margins.Right != (this.RightMargin) ||
                this.Margins.Top != (this.TopMargin) || this.Margins.Bottom != (this.BottomMargin) ||
                this.PaperSize.Height != (this.PageHeight) || this.PaperSize.Width != (this.PageWidth))
            {
                return true;
            }

            return false;
        }

        private void LoadPageValues()
        {
            this.UpdatePageDimention();
            foreach (var paper in paperSizes.Keys)
            {
                this.cmb_PageSize.Items.Add(paper);
            }
            this.cmb_PageSize.Items.Add("Custom");
        }

        private void UpdatePageDimention()
        {
            paperSizes.Add("A3", new PaperSize(11.7, 16.5));
            paperSizes.Add("A4", new PaperSize(8.27, 11.69));
            paperSizes.Add("B4(JIS)", new PaperSize(10.12, 14.33));
            paperSizes.Add("B5(JIS)", new PaperSize(7.17, 10.12));
            paperSizes.Add("Envelope #10", new PaperSize(4.125, 9.5));
            paperSizes.Add("Envelope Monarch", new PaperSize(3.875, 7.5));
            paperSizes.Add("Executive", new PaperSize(7.25, 10.5));
            paperSizes.Add("Legal", new PaperSize(8.5, 14));
            paperSizes.Add("Letter", new PaperSize(8.5, 11));
            paperSizes.Add("Tabloid", new PaperSize(11, 17));
        }

        private void SetPageValues()
        {
            this.updwn_Left.Value = (this.Margins.Left / 96);
            this.updwn_Right.Value = (this.Margins.Right / 96);
            this.updwn_Top.Value = (this.Margins.Top / 96);
            this.updwn_Bottom.Value = (this.Margins.Bottom / 96);
            this.updwn_Height.Value = this.PaperSize.Height / 96;
            this.updwn_Width.Value = this.PaperSize.Width / 96;

            if (this.PaperName.ToLower() == "custom")
            {
#if SILVERLIGHT
                this.updwn_Height.IsEnabled = true;
                this.updwn_Width.IsEnabled = true;
#else
                this.stpnl_CustomSize.IsEnabled = true;
#endif
            }

            this.rbtn_landscape.IsChecked = (this.PaperType.ToLower() == "landscape") ? true : false;
            int index = 0;
            foreach (var key in paperSizes.Keys)
            {
                if (key == PaperName)
                {
                    break;
                }
                index++;
            }
            this.cmb_PageSize.SelectedIndex = index;
        }

        #endregion
    }

    class PaperSize
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="PaperSize"/> class.
        /// </summary>
        /// <param name="width">The x.</param>
        /// <param name="height">The y.</param>
        public PaperSize(double width, double height)
        {
            this.Width = width;
            this.Height = height;
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
