#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Drawing;

using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.XlsIO;
using Syncfusion.XlsIO.Implementation;


namespace Syncfusion.ExcelToPdfConverter
{
    /// <summary>
    /// This Class represents the Document Settings for the Excel to Pdf Converter 
    /// </summary>
    public class ExcelToPdfConverterSettings
    {
        #region Fields

        /// <summary>
        /// Represents the Pdf document object.
        /// </summary>
        private PdfDocument pdfDocument;

        /// <summary>
        /// Represents the display style of the gridlines in the output document.
        /// </summary>
        private GridLinesDisplayStyle displayGridLines;

        /// <summary>
        /// Represents the layout mode of the output sheet.
        /// </summary>
        private LayoutOptions layoutOptions = LayoutOptions.Automatic;

        /// <summary>
        /// Indicates whether to embed the fonts to the output pdf document.
        /// </summary>
        private bool embedFonts = false;

        /// <summary>
        /// Indicates whether to export the bookmarks to the output pdf document.
        /// </summary>
        private bool exportBookmarks = true;

        /// <summary>
        /// Indicates whether to export the document properties to the output pdf document.
        /// </summary>
        private bool exportDocumentProperties = true;

        /// <summary>
        /// Indicates whether the output pdf sheet should be rendered from right to left.
        /// </summary>
        private bool enableRTL = false;
        /// <summary>
        /// Represents the Header footer option of the output document.
        /// </summary>
        private HeaderFooterOption m_HFOption;
        /// <summary>
        /// TRUE - Throw exception when excel file is empty,otherwise FALSE
        /// </summary>
        private bool m_throwWhenExcelFileIsEmpty;      
        /// <summary>
        /// TRUE - Use TIFF Quality Image while converting Excel to PDF document,otherwise FALSE
        /// </summary>
        private bool m_exportQulaityImage;
        /// <summary>
        /// Represents the Need Blank Option
        /// </summary>
        private bool m_needBlankSheet = true;        
        #endregion

        #region constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ExcelToPdfConverterSettings"/> class.
        /// </summary>
        public ExcelToPdfConverterSettings()
        {
            this.pdfDocument = new PdfDocument();
            this.LayoutOptions = LayoutOptions.Automatic;
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets or sets the template document.
        /// </summary>
        /// <value>The template document.</value>
        public PdfDocument TemplateDocument
        {
            get
            {
                return this.pdfDocument;
            }

            set
            {
                this.pdfDocument = value;
            }
        }

        /// <summary>
        /// Gets or sets the display grid lines.
        /// </summary>
        /// <value>The display grid lines.</value>
        public GridLinesDisplayStyle DisplayGridLines
        {
            get
            {
                return this.displayGridLines;
            }

            set
            {
                this.displayGridLines = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [embed fonts].
        /// </summary>
        /// <value><c>true</c> if [embed fonts]; otherwise, <c>false</c>.</value>
        public bool EmbedFonts
        {
            get
            {
                return this.embedFonts;
            }

            set
            {
                this.embedFonts = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [export bookmarks].
        /// </summary>
        /// <value><c>true</c> if [export bookmarks]; otherwise, <c>false</c>.</value>
        public bool ExportBookmarks
        {
            get
            {
                return this.exportBookmarks;
            }

            set
            {
                this.exportBookmarks = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [export document properties].
        /// </summary>
        /// <value>
        /// <c>true</c> if [export document properties]; otherwise, <c>false</c>.
        /// </value>
        public bool ExportDocumentProperties
        {
            get
            {
                return this.exportDocumentProperties;
            }

            set
            {
                this.exportDocumentProperties = value;
            }
        }
        /// <summary>
        /// Returns the Header footer Option.
        /// </summary>    
        /// <value>The HeaderFooterOption object</value>
        public HeaderFooterOption HeaderFooterOption
        {
            get
            {
                if (m_HFOption == null)
                    m_HFOption = new HeaderFooterOption();

                return m_HFOption;
            }
            
        }

        /// <summary>
        /// Gets or sets the layout mode.
        /// </summary>
        /// <value>The layout mode.</value>
        public LayoutOptions LayoutOptions
        {
            get
            {
                return this.layoutOptions;
            }

            set
            {
                this.layoutOptions = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [enable RTL].
        /// </summary>
        /// <value><c>true</c> if [enable RTL]; otherwise, <c>false</c>.</value>
        internal bool EnableRTL
        {
            get
            {
                return this.enableRTL;
            }

            set
            {
                this.enableRTL = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether to throw exception when empty excel file is being converted to an PDF document.
        /// </summary>
        /// <value><c>true</c> if [throwWhenExcelFileNotsaved]; otherwise, <c>false</c>.</value>
        public bool ThrowWhenExcelFileIsEmpty
        {
            get
            {
                return this.m_throwWhenExcelFileIsEmpty;
            }
            set
            {
                this.m_throwWhenExcelFileIsEmpty = value;
            }
        }       
        /// <summary>
        ///  Gets or sets a value indicating whether to export quality image
        /// </summary>
        /// <value><c>true</c> if [export quality image]; otherwise, <c>false</c>.</value>
        public bool ExportQualityImage
        {
            get
            {
                return this.m_exportQulaityImage;
            }
            set
            {
                this.m_exportQulaityImage = value;
            }
        }
        /// <summary>
        ///  Gets or sets a value indicating whether to export quality image
        /// </summary>
        public bool IsConvertBlankSheet
        {
            get
            {
                return m_needBlankSheet;
            }
            set
            {
                m_needBlankSheet = value;
            }
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Gets the size of the excel sheet.
        /// </summary>
        /// <param name="paperSize">Size of the paper.</param>
        /// <returns>The Size of the output pdf page.</returns>
        internal static SizeF GetExcelSheetSize(ExcelPaperSize paperSize)
        {
            PdfUnitConvertor unitConverter = new PdfUnitConvertor();
            SizeF returnSize = new SizeF();
            switch (paperSize)
            {
                case ExcelPaperSize.A2Paper:
                    returnSize = new SizeF(unitConverter.ConvertUnits((float)16.54, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point), unitConverter.ConvertUnits((float)23.39, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point));
                    break;
                case ExcelPaperSize.A3ExtraTransversePaper:
                case ExcelPaperSize.A3ExtraPaper:
                    returnSize = new SizeF(unitConverter.ConvertUnits((float)12.68, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point), unitConverter.ConvertUnits((float)17.52, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point));
                    break;
                case ExcelPaperSize.A3TransversePaper:
                    returnSize = new SizeF(unitConverter.ConvertUnits((float)11.69, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point), unitConverter.ConvertUnits((float)16.54, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point));
                    break;
                case ExcelPaperSize.A4ExtraPaper:
                    returnSize = new SizeF(unitConverter.ConvertUnits((float)9.27, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point), unitConverter.ConvertUnits((float)12.69, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point));
                    break;
                case ExcelPaperSize.A4PlusPaper:
                    returnSize = new SizeF(unitConverter.ConvertUnits((float)8.27, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point), unitConverter.ConvertUnits((float)12.99, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point));
                    break;
                case ExcelPaperSize.A4TransversePaper:
                    returnSize = new SizeF(unitConverter.ConvertUnits((float)8.27, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point), unitConverter.ConvertUnits((float)11.69, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point));
                    break;
                case ExcelPaperSize.A5ExtraPpaper:
                    returnSize = new SizeF(unitConverter.ConvertUnits((float)6.85, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point), unitConverter.ConvertUnits((float)9.25, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point));
                    break;
                case ExcelPaperSize.A5TransversePaper:
                    returnSize = new SizeF(unitConverter.ConvertUnits((float)5.83, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point), unitConverter.ConvertUnits((float)8.27, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point));
                    break;
                case ExcelPaperSize.InviteEnvelope:
                    returnSize = new SizeF(unitConverter.ConvertUnits((float)8.66, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point), unitConverter.ConvertUnits((float)8.66, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point));
                    break;
                case ExcelPaperSize.ISOB4:
                    returnSize = new SizeF(unitConverter.ConvertUnits((float)250, PdfGraphicsUnit.Millimeter, PdfGraphicsUnit.Point), unitConverter.ConvertUnits((float)353, PdfGraphicsUnit.Millimeter, PdfGraphicsUnit.Point));
                    break;
                case ExcelPaperSize.ISOB5ExtraPaper:
                    returnSize = new SizeF(unitConverter.ConvertUnits((float)7.91, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point), unitConverter.ConvertUnits((float)10.87, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point));
                    break;
                case ExcelPaperSize.JapaneseDoublePostcard:
                    returnSize = new SizeF(unitConverter.ConvertUnits((float)7.87, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point), unitConverter.ConvertUnits((float)5.83, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point));
                    break;
                case ExcelPaperSize.JISB5TransversePaper:
                    returnSize = new SizeF(unitConverter.ConvertUnits((float)7.17, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point), unitConverter.ConvertUnits((float)10.12, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point));
                    break;
                case ExcelPaperSize.LegalExtraPaper9275By15:
                    returnSize = new SizeF(unitConverter.ConvertUnits((float)9.275, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point), unitConverter.ConvertUnits((float)15, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point));
                    break;
                case ExcelPaperSize.LetterExtraTransversePaper:
                case ExcelPaperSize.LetterExtraPaper9275By12:
                    returnSize = new SizeF(unitConverter.ConvertUnits((float)9.275, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point), unitConverter.ConvertUnits((float)12, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point));
                    break;
                case ExcelPaperSize.LetterPlusPaper:
                    returnSize = new SizeF(unitConverter.ConvertUnits((float)8.5, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point), unitConverter.ConvertUnits((float)12.69, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point));
                    break;
                case ExcelPaperSize.LetterTransversePaper:
                    returnSize = new SizeF(unitConverter.ConvertUnits((float)8.275, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point), unitConverter.ConvertUnits((float)11, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point));
                    break;
                case ExcelPaperSize.Paper10x14:
                    returnSize = new SizeF(unitConverter.ConvertUnits((float)10, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point), unitConverter.ConvertUnits((float)14, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point));
                    break;
                case ExcelPaperSize.Paper11x17:
                    returnSize = new SizeF(unitConverter.ConvertUnits((float)11, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point), unitConverter.ConvertUnits((float)17, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point));
                    break;
                case ExcelPaperSize.PaperA3:
                    returnSize = new SizeF(unitConverter.ConvertUnits((float)11.69, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point), unitConverter.ConvertUnits((float)16.54, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point));
                    break;
                case ExcelPaperSize.PaperA4Small:
                case ExcelPaperSize.PaperA4:
                    returnSize = new SizeF(unitConverter.ConvertUnits((float)8.27, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point), unitConverter.ConvertUnits((float)11.69, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point));
                    break;
                case ExcelPaperSize.PaperA5:
                    returnSize = new SizeF(unitConverter.ConvertUnits((float)5.83, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point), unitConverter.ConvertUnits((float)8.27, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point));
                    break;
                case ExcelPaperSize.PaperB4:
                    returnSize = new SizeF(PdfPageSize.B4.Width, PdfPageSize.B4.Height);
                    break;
                case ExcelPaperSize.PaperB5:
                    returnSize = new SizeF(unitConverter.ConvertUnits((float)176, PdfGraphicsUnit.Millimeter, PdfGraphicsUnit.Point), unitConverter.ConvertUnits((float)250, PdfGraphicsUnit.Millimeter, PdfGraphicsUnit.Point));
                    break;
                case ExcelPaperSize.PaperCsheet:
                    returnSize = new SizeF(unitConverter.ConvertUnits((float)17, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point), unitConverter.ConvertUnits((float)22, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point));
                    break;
                case ExcelPaperSize.PaperLetterSmall:
                case ExcelPaperSize.PaperNote:
                case ExcelPaperSize.PaperLetter:
                    returnSize = new SizeF(unitConverter.ConvertUnits((float)8.5, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point), unitConverter.ConvertUnits((float)11, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point));
                    break;
                case ExcelPaperSize.PaperDsheet:
                    returnSize = new SizeF(unitConverter.ConvertUnits((float)22, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point), unitConverter.ConvertUnits((float)34, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point));
                    break;
                case ExcelPaperSize.PaperEsheet:
                    returnSize = new SizeF(unitConverter.ConvertUnits((float)34, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point), unitConverter.ConvertUnits((float)44, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point));
                    break;
                case ExcelPaperSize.PaperEnvelope9:
                    returnSize = new SizeF(unitConverter.ConvertUnits((float)3.87, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point), unitConverter.ConvertUnits((float)8.87, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point));
                    break;
                case ExcelPaperSize.PaperEnvelope10:
                    returnSize = new SizeF(unitConverter.ConvertUnits((float)4.12, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point), unitConverter.ConvertUnits((float)9.5, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point));
                    break;
                case ExcelPaperSize.PaperEnvelope11:
                    returnSize = new SizeF(unitConverter.ConvertUnits((float)4.5, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point), unitConverter.ConvertUnits((float)10.37, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point));
                    break;
                case ExcelPaperSize.PaperEnvelope12:
                    returnSize = new SizeF(unitConverter.ConvertUnits((float)4.75, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point), unitConverter.ConvertUnits((float)11, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point));
                    break;
                case ExcelPaperSize.PaperEnvelope14:
                    returnSize = new SizeF(unitConverter.ConvertUnits((float)5, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point), unitConverter.ConvertUnits((float)11.5, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point));
                    break;
                case ExcelPaperSize.PaperQuarto:
                    returnSize = new SizeF(unitConverter.ConvertUnits((float)8.47, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point), unitConverter.ConvertUnits((float)10.83, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point));
                    break;
                case ExcelPaperSize.PaperFolio:
                    returnSize = new SizeF(unitConverter.ConvertUnits((float)8.5, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point), unitConverter.ConvertUnits((float)13, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point));
                    break;
                case ExcelPaperSize.PaperLegal:
                    returnSize = new SizeF(unitConverter.ConvertUnits((float)8.5, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point), unitConverter.ConvertUnits((float)14, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point));
                    break;
                case ExcelPaperSize.PaperLedger:
                    returnSize = new SizeF(unitConverter.ConvertUnits((float)17, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point), unitConverter.ConvertUnits((float)11, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point));
                    break;
                case ExcelPaperSize.PaperTabloid:
                    returnSize = new SizeF(unitConverter.ConvertUnits((float)11, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point), unitConverter.ConvertUnits((float)17, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point));
                    break;
                case ExcelPaperSize.PaperStatement:
                    returnSize = new SizeF(unitConverter.ConvertUnits((float)5.5, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point), unitConverter.ConvertUnits((float)8.5, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point));
                    break;
                case ExcelPaperSize.PaperExecutive:
                    returnSize = new SizeF(unitConverter.ConvertUnits((float)7.25, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point), unitConverter.ConvertUnits((float)10.5, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point));
                    break;
                case ExcelPaperSize.PaperEnvelopeDL:
                    returnSize = new SizeF(unitConverter.ConvertUnits((float)4.33, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point), unitConverter.ConvertUnits((float)8.66, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point));
                    break;
                case ExcelPaperSize.PaperEnvelopeC5:
                    returnSize = new SizeF(unitConverter.ConvertUnits((float)6.38, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point), unitConverter.ConvertUnits((float)9.02, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point));
                    break;
                case ExcelPaperSize.PaperEnvelopeC3:
                    returnSize = new SizeF(unitConverter.ConvertUnits((float)12.76, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point), unitConverter.ConvertUnits((float)18.03, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point));
                    break;
                case ExcelPaperSize.PaperEnvelopeC4:
                    returnSize = new SizeF(unitConverter.ConvertUnits((float)9.02, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point), unitConverter.ConvertUnits((float)12.76, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point));
                    break;
                case ExcelPaperSize.PaperEnvelopeC6:
                    returnSize = new SizeF(unitConverter.ConvertUnits((float)4.49, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point), unitConverter.ConvertUnits((float)6.38, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point));
                    break;
                case ExcelPaperSize.PaperEnvelopeC65:
                    returnSize = new SizeF(unitConverter.ConvertUnits((float)4.49, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point), unitConverter.ConvertUnits((float)9.02, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point));
                    break;
                case ExcelPaperSize.PaperEnvelopeB4:
                    returnSize = new SizeF(unitConverter.ConvertUnits((float)9.84, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point), unitConverter.ConvertUnits((float)13.9, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point));
                    break;
                case ExcelPaperSize.PaperEnvelopeB5:
                    returnSize = new SizeF(unitConverter.ConvertUnits((float)6.93, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point), unitConverter.ConvertUnits((float)9.84, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point));
                    break;
                case ExcelPaperSize.PaperEnvelopeB6:
                    returnSize = new SizeF(unitConverter.ConvertUnits((float)6.93, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point), unitConverter.ConvertUnits((float)4.92, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point));
                    break;
                case ExcelPaperSize.PaperEnvelopeItaly:
                    returnSize = new SizeF(unitConverter.ConvertUnits((float)4.33, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point), unitConverter.ConvertUnits((float)9.06, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point));
                    break;
                case ExcelPaperSize.PaperEnvelopeMonarch:
                    returnSize = new SizeF(unitConverter.ConvertUnits((float)3.87, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point), unitConverter.ConvertUnits((float)7.5, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point));
                    break;
                case ExcelPaperSize.PaperEnvelopePersonal:
                    returnSize = new SizeF(unitConverter.ConvertUnits((float)3.85, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point), unitConverter.ConvertUnits((float)6.5, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point));
                    break;
                case ExcelPaperSize.PaperFanfoldLegalGerman:
                    returnSize = new SizeF(unitConverter.ConvertUnits((float)8.5, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point), unitConverter.ConvertUnits((float)13, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point));
                    break;
                case ExcelPaperSize.PaperFanfoldStdGerman:
                    returnSize = new SizeF(unitConverter.ConvertUnits((float)8.5, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point), unitConverter.ConvertUnits((float)12, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point));
                    break;
                case ExcelPaperSize.PaperFanfoldUS:
                    returnSize = new SizeF(unitConverter.ConvertUnits((float)14.87, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point), unitConverter.ConvertUnits((float)11, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point));
                    break;
                case ExcelPaperSize.StandardPaper10By11:
                    returnSize = new SizeF(unitConverter.ConvertUnits((float)10, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point), unitConverter.ConvertUnits((float)11, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point));
                    break;
                case ExcelPaperSize.StandardPaper15By11:
                    returnSize = new SizeF(unitConverter.ConvertUnits((float)15, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point), unitConverter.ConvertUnits((float)11, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point));
                    break;
                case ExcelPaperSize.StandardPaper9By11:
                    returnSize = new SizeF(unitConverter.ConvertUnits((float)9, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point), unitConverter.ConvertUnits((float)11, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point));
                    break;
                case ExcelPaperSize.SuperASuperAA4Paper:
                    returnSize = new SizeF(unitConverter.ConvertUnits((float)8.94, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point), unitConverter.ConvertUnits((float)14.02, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point));
                    break;
                case ExcelPaperSize.SuperBSuperBA3Paper:
                    returnSize = new SizeF(unitConverter.ConvertUnits((float)12.01, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point), unitConverter.ConvertUnits((float)19.17, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point));
                    break;
                case ExcelPaperSize.TabloidExtraPaper:
                    returnSize = new SizeF(unitConverter.ConvertUnits((float)11.69, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point), unitConverter.ConvertUnits((float)18, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point));
                    break;
                default:
                    returnSize = new SizeF(unitConverter.ConvertUnits((float)8.5, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point), unitConverter.ConvertUnits((float)11, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point));
                    break;
            }

            return returnSize;
        }

        #endregion
    }

    #region Helper Classes

    /// <summary>
    /// The class has the functions and properties to manipulate the Header and footers of the output page.
    /// </summary>
    internal class HeaderFooter
    {
        #region Fields

        /// <summary>
        /// Indicates the template size of the HeaderFooter.
        /// </summary>
        private RectangleF templateSize;

        /// <summary>
        /// Indicates the name of the HeaderFooter.
        /// </summary>
        private string headerFooterName;

        /// <summary>
        /// Indicates the HeaderFooter Section collections.
        /// </summary>
        private List<HeaderFooterSection> headerFooterSections;

        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="HF"/> class.
        /// </summary>
        public HeaderFooter()
        {
            this.headerFooterSections = new List<HeaderFooterSection>();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the size of the template.
        /// </summary>
        /// <value>The size of the template.</value>
        internal RectangleF TemplateSize
        {
            get
            {
                return this.templateSize;
            }

            set
            {
                this.templateSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the header footer.
        /// </summary>
        /// <value>The name of the header footer.</value>
        internal string HeaderFooterName
        {
            get
            {
                return this.headerFooterName;
            }

            set
            {
                this.headerFooterName = value;
            }
        }


        /// <summary>
        /// Gets or sets the header footer sections.
        /// </summary>
        /// <value>The header footer sections.</value>
        internal List<HeaderFooterSection> HeaderFooterSections
        {
            get
            {
                return this.headerFooterSections;
            }

            set
            {
                this.headerFooterSections = value;
            }
        }
        #endregion
    }

    /// <summary>
    /// The class has the properties to hold the header and footer section settings.
    /// </summary>
    internal class HeaderFooterSection
    {
        #region Fields
        /// <summary>
        /// Represents the width of the HF section.
        /// </summary>
        private float headerFooterWidth;

        /// <summary>
        /// Represents the height of the HF Section.
        /// </summary>
        private float headerFooterHeight;

        /// <summary>
        /// Represents the HF Section name.
        /// </summary>
        private string headerFooterSectionName;
        /// <summary>
        /// Represents the collection of Richtext string.
        /// </summary>
        private List<RichTextString> m_rtf;

        /// <summary>
        /// Represents the collection of the HF.
        /// </summary>
        private Dictionary<string, HeaderFooterFontColorSettings> headerFooterCollections;
        /// <summary>
        /// Represents the rich text 
        /// </summary>
        internal int HeaderFooterFontCount;
        /// <summary>
        /// Text Alignment of Header Footer String.
        /// </summary>
        private PdfTextAlignment m_textAlignment;
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="HeaderFooterSection"/> class.
        /// </summary>
        public HeaderFooterSection()
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        internal float Width
        {
            get
            {
                return this.headerFooterWidth;
            }

            set
            {
                this.headerFooterWidth = value;
            }
        }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
        internal float Height
        {
            get
            {
                return this.headerFooterHeight;
            }

            set
            {
                this.headerFooterHeight = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the section.
        /// </summary>
        /// <value>The name of the section.</value>
        internal string SectionName
        {
            get
            {
                return this.headerFooterSectionName;
            }

            set
            {
                this.headerFooterSectionName = value;
            }
        }

        /// <summary>
        /// Gets or sets the header footer collections.
        /// </summary>
        /// <value>The header footer collections.</value>
        internal Dictionary<string, HeaderFooterFontColorSettings> HeaderFooterCollections
        {
            get
            {
                return this.headerFooterCollections;
            }

            set
            {
                this.headerFooterCollections = value;
            }
        }
        /// <summary>
        /// Gets or sets the RTF.
        /// </summary>
        /// <value>The RTF.</value>
        internal List<RichTextString> RTF
        {
            get
            {
                return m_rtf;
            }
            set
            {
                m_rtf = value;
            }
        }
        /// <summary>
        /// Represents the Header Footer Section text alignmet
        /// </summary>
        internal PdfTextAlignment TextAlignment
        {
            get
            {
                return m_textAlignment;
            }
            set
            {
                m_textAlignment = value;
            }
        }
        #endregion
    }

    /// <summary>
    /// The class has the properties and functions for the HF color and font settings.
    /// </summary>
    internal class HeaderFooterFontColorSettings :ICloneable
    {
        #region Fields
        /// <summary>
        /// Represents the font of the header and footer text.
        /// </summary>
        private Font font;

        /// <summary>
        /// Represents the font color of the header and footer text.
        /// </summary>
        private Color fontColor;
        /// <summary>
        /// Represents the Underline for the header and footer text.
        /// </summary>
        private bool hasUnderline;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="HeaderFooterFontColorSettings"/> class.
        /// </summary>
        public HeaderFooterFontColorSettings()
        {
            this.font = new Font("Calibri", 8);
            this.fontColor = Color.FromArgb(255, 0, 0, 0);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the font.
        /// </summary>
        /// <value>The font of the header and footer text.</value>
        internal Font Font
        {
            get
            {
                return this.font;
            }

            set
            {
                this.font = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether this instance has underline.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has underline; otherwise, <c>false</c>.
        /// </value>
        internal bool HasUnderline
        {
            get
            {
                return hasUnderline;
            }
            set
            {
                hasUnderline = value;
            }
        }
        /// <summary>
        /// Gets or sets the color of the font.
        /// </summary>
        /// <value>The color of the font.</value>
        internal Color FontColor
        {
            get
            {
                return this.fontColor;
            }

            set
            {
                this.fontColor = value;
            }
        }
        #endregion

        #region ICloneable Members

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public Object Clone()
        {
            HeaderFooterFontColorSettings settings = this.MemberwiseClone() as HeaderFooterFontColorSettings;

            return settings;
        }

        #endregion
    }

    #endregion
}
