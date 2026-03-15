#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Tools.Olap
{
    using System.Windows.Controls;
    using Syncfusion.Olap.Reports;
    using Syncfusion.Windows.Shared;

    /// <summary>
    /// Lists the available reports in the report definition file
    /// </summary>

   // [SkinType(SkinVisualStyle = Skin.Office2007Blue,
   //Type = typeof(ReportList), XamlResource = "/Syncfusion.OlapTools.WPF;component/Themes/Generic.xaml")]
   // [SkinType(SkinVisualStyle = Skin.Office2007Black,
   // Type = typeof(ReportList), XamlResource = "/Syncfusion.OlapTools.WPF;component/Themes/Generic.xaml")]
   // [SkinType(SkinVisualStyle = Skin.Office2007Silver,
   // Type = typeof(ReportList), XamlResource = "/Syncfusion.OlapTools.WPF;component/Themes/Generic.xaml")]
   // [SkinType(SkinVisualStyle = Skin.Office2003,
   // Type = typeof(ReportList), XamlResource = "/Syncfusion.OlapTools.WPF;component/Themes/Generic.xaml")]
   // [SkinType(SkinVisualStyle = Skin.Blend,
   // Type = typeof(ReportList), XamlResource = "/Syncfusion.OlapTools.WPF;component/Themes/Generic.xaml")]   
   // [SkinType(SkinVisualStyle = Skin.Default,
   // Type = typeof(ReportList), XamlResource = "/Syncfusion.OlapTools.WPF;component/Themes/Generic.xaml")] 
    public class ReportList : ComboBox
    {
        #region Private Variables

        private OlapReportCollection _Reports;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportList"/> class.
        /// </summary>
        public ReportList()
        {
            this.IsEnabled = false;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the reports.
        /// </summary>
        /// <value>The reports.</value>
        public OlapReportCollection Reports
        {
            get
            {
                return this._Reports;
            }

            set
            {
                this._Reports = value;
                if (this._Reports != null)
                {
                    this.IsEnabled = true;
                    this.Items.Clear();
                    foreach (OlapReport report in this._Reports)
                    {
                        this.Items.Add(report.Name);
                    }
                }
                else
                {
                    this.IsEnabled = false;
                    this.Items.Clear();
                }
            }
        }

        #endregion
    }
}
