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
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using System.Windows;

namespace Syncfusion.Windows.Chart.Olap.Converter
{
    /// <summary>
    /// Helps to export OLAP Chart into PDF document.
    /// </summary>
    public class OlapChartPdfExport
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="OlapChartPdfExport"/> class.
        /// </summary>
        /// <param name="olapChart">The olap chart.</param>
        public OlapChartPdfExport(Syncfusion.Windows.Chart.Olap.OlapChart olapChart)
        {
            this.OlapChart = olapChart;
        } 

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the olap chart.
        /// </summary>
        /// <value>The olap chart.</value>
        internal Syncfusion.Windows.Chart.Olap.OlapChart OlapChart { get; private set; }
        public bool ExportOlapChartWithZoomFactor { get; set; }
        #endregion

        #region Helper Methods

        /// <summary>
        /// Exports the into new PDF.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public void ExportIntoNewPdf(string fileName)
        {
            double zoomFactor=0.0d;
            PdfDocument pdfDoc = new PdfDocument();
            pdfDoc.Pages.Add();
            if (!this.ExportOlapChartWithZoomFactor)
            {
                zoomFactor = this.OlapChart.PrimaryAxis.ZoomFactor;
                ChartAreaCommands.ZoomReset.Execute(null, this.OlapChart);
            }
            pdfDoc.Pages[0].Graphics.DrawImage(PdfImage.FromImage(this.OlapChart.GetChartImage()), 10, 30, 500, 300);
            pdfDoc.Save(fileName);
            if (MessageBox.Show("Do you want to view the Pdf document?", "Pdf Document has been created", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
            {
                System.Diagnostics.Process.Start(fileName);
            }
            if (!this.ExportOlapChartWithZoomFactor)
                this.OlapChart.PrimaryAxis.ZoomFactor = zoomFactor;
        }

        #endregion
    }
}
