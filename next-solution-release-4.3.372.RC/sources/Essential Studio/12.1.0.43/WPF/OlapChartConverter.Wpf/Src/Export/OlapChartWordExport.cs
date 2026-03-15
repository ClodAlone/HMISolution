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
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO;
using System.Windows;
using System.Text.RegularExpressions;

namespace Syncfusion.Windows.Chart.Olap.Converter
{
    /// <summary>
    /// Helps to export OLAP Chart into Word document
    /// </summary>
    public class OlapChartWordExport
    {
        #region Members

        /// <summary>
        /// 
        /// </summary>
        private const string Defaultmarker = "#BI_CHART_PLACEHOLDER#";

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="OlapChartWordExport"/> class.
        /// </summary>
        /// <param name="olapChart">The olap chart.</param>
        public OlapChartWordExport(Syncfusion.Windows.Chart.Olap.OlapChart olapChart)
        {
            this.OlapChart = olapChart;
        } 

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the olap chart.
        /// </summary>
        /// <value>The olap chart.</value>
        internal Syncfusion.Windows.Chart.Olap.OlapChart OlapChart { get; set; }
        /// <summary>
        /// Gets or sets the OlapChart Export with ZoomFactor
        /// </summary>
        public bool ExportOlapChartWithZoomFactor { get; set; }
        #endregion

        #region Helper Methods

        /// <summary>
        /// Exportintoes the new doc.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public void ExportintoNewDoc(string fileName)
        {
            //Create a new document
            WordDocument document = new WordDocument();
            double zoomFactor=0.0d;
            //Adding a new section to the document.
            IWSection section = document.AddSection();
            //Adding a paragraph to the section
            //Adding a new paragraph.
            IWParagraph paragraph = section.AddParagraph();
            //Setting Alignment for the image.
            paragraph.ParagraphFormat.HorizontalAlignment = Syncfusion.DocIO.DLS.HorizontalAlignment.Center;
            //Inserting .wmf Image to the document.
            if (!this.ExportOlapChartWithZoomFactor)
            {
                zoomFactor = this.OlapChart.PrimaryAxis.ZoomFactor;
                ChartAreaCommands.ZoomReset.Execute(null, this.OlapChart);
            }
            WPicture mImage = (WPicture)paragraph.AppendPicture((System.Drawing.Image)this.OlapChart.GetChartImage());
            if (mImage.Height > document.LastSection.PageSetup.PageSize.Height)
                mImage.Height = document.LastSection.PageSetup.PageSize.Height;

            if (mImage.Width > document.LastSection.PageSetup.PageSize.Width)
                mImage.Width = document.LastSection.PageSetup.PageSize.Width;

            //Scaling Image
            mImage.HeightScale = 80f;
            mImage.WidthScale = 80f;

            mImage.AddCaption("Essential BI OLAP Chart for WPF", CaptionNumberingFormat.Roman, CaptionPosition.AboveImage);
            document.Save(fileName);
            if (MessageBox.Show("Do you want to view the MS Word document?", "Document has been created", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
            {
                System.Diagnostics.Process.Start(fileName);
            }
            if (!this.ExportOlapChartWithZoomFactor)
            {
                this.OlapChart.PrimaryAxis.ZoomFactor = zoomFactor;
            }
           
        }

        /// <summary>
        /// Exports the into template doc.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public void ExportIntoTemplateDoc(string fileName)
        {
            try
            {
                WordDocument document = new WordDocument(fileName);
                TextBodyPart m_textBodyPart = new TextBodyPart(document);
                WPicture m_image = new WPicture(document);
                m_image.LoadImage(this.OlapChart.GetChartImage());
                if (m_image.Height > document.LastSection.PageSetup.PageSize.Height)
                    m_image.Height = document.LastSection.PageSetup.PageSize.Height;

                if (m_image.Width > document.LastSection.PageSetup.PageSize.Width)
                    m_image.Width = document.LastSection.PageSetup.PageSize.Width;

                //Scaling Image
                m_image.HeightScale = 80f;
                m_image.WidthScale = 80f;

                WParagraph paragraph = new WParagraph(document);
                paragraph.ParagraphFormat.HorizontalAlignment = Syncfusion.DocIO.DLS.HorizontalAlignment.Center;
                paragraph.Items.Add(m_image);
                m_textBodyPart.BodyItems.Insert(0, paragraph);
                int markerCount = document.Replace(new Regex(Defaultmarker), m_textBodyPart);
                if (markerCount == 0)
                {
                    document.LastParagraph.AppendPicture(this.OlapChart.GetChartImage());
                }
                document.Save(fileName, Syncfusion.DocIO.FormatType.Doc);
                if (MessageBox.Show("Do you want to view the MS Word document?", "Document has been created", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                {
                    System.Diagnostics.Process.Start(fileName);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Exports the into template doc.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="markerString">The marker string.</param>
        public void ExportIntoTemplateDoc(string fileName, string markerString)
        {
            try
            {
                WordDocument m_document = new WordDocument(fileName);
                TextBodyPart m_textBodyPart = new TextBodyPart(m_document);
                WPicture m_image = new WPicture(m_document);
                m_image.LoadImage(this.OlapChart.GetChartImage());
                if (m_image.Height > m_document.LastSection.PageSetup.PageSize.Height)
                    m_image.Height = m_document.LastSection.PageSetup.PageSize.Height;

                if (m_image.Width > m_document.LastSection.PageSetup.PageSize.Width)
                    m_image.Width = m_document.LastSection.PageSetup.PageSize.Width;

                //Scaling Image
                m_image.HeightScale = 80f;
                m_image.WidthScale = 80f;

                WParagraph para = new WParagraph(m_document);
                para.Items.Add(m_image);
                m_textBodyPart.BodyItems.Insert(0, para);
                int markerCount = m_document.Replace(new Regex(markerString), m_textBodyPart);
                if (markerCount == 0)
                {
                    throw new Exception("Marker is not found");
                }

                m_document.Save(fileName, Syncfusion.DocIO.FormatType.Doc);
                if (MessageBox.Show("Do you want to view the MS Word document?", "Document has been created", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                {
                    System.Diagnostics.Process.Start(fileName);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        } 

        #endregion
    }
}
