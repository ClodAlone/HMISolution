#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws.
#endregion Copyright

using System.Collections.Generic;
using System.Drawing;

using Syncfusion.XlsIO;
using Syncfusion.XlsIO.Implementation.Collections;

namespace Syncfusion.ExcelToPdfConverter
{
    /// <summary>
    /// Class used for Table style rendering in the pdf page.
    /// </summary>
    internal class TableStyleRenderer
    {
        #region Fields

        /// <summary>
        /// Sheet List objects.
        /// </summary>
        private IListObjects listObjects;

        /// <summary>
        /// List object built in style.
        /// </summary>
        private TableBuiltInStyles builtInStyle;

        /// <summary>
        /// Collection of font colors of listobjects.
        /// </summary>
        private List<TableBuiltInStyles> colorFonts;

        /// <summary>
        /// Collection of built in styles bordercolors.
        /// </summary>
        private Dictionary<TableBuiltInStyles, Color> borderList;

        /// <summary>
        /// Collection of double border built in styles.
        /// </summary>
        private Dictionary<TableBuiltInStyles, Color> doubleBorderList;

        /// <summary>
        /// Collection of built in styles that has top border as solid.
        /// </summary>
        private Dictionary<TableBuiltInStyles, Color> topSolidList;

        /// <summary>
        /// Collection of List object ranges and their respective borders and bordercolors.
        /// </summary>
        private Dictionary<IRange, Dictionary<ExcelBordersIndex, Color>> borderColorList;

        /// <summary>
        /// Collection of list object ranges and their font colors.
        /// </summary>
        private Dictionary<IRange, Color> fontColorCollections;

        /// <summary>
        /// Collection of Dark built in styles for the Last and first column.
        /// </summary>
        private List<TableBuiltInStyles> darkStyles;

        /// <summary>
        /// Collection of Medium built in Styles for the Last and first column that has border settings.
        /// </summary>
        private List<TableBuiltInStyles> mediumStyleWithBorder;

        /// <summary>
        /// Collection of Medium built in Styles for the Last and first column that has no border settings.
        /// </summary>
        private List<TableBuiltInStyles> mediumStyleWithoutBorder;

        /// <summary>
        /// Collection of Light buit in Styles for the Row and column stripes.
        /// </summary>
        private List<TableBuiltInStyles> lightStyleBorder;

        /// <summary>
        /// Collection of Medium styles for the column stripes.
        /// </summary>
        private List<TableBuiltInStyles> mediumStyleBorder;

        /// <summary>
        /// List Object Header color 
        /// </summary>
        private Color headerColor = Color.Empty;

        /// <summary>
        /// Indicates whether First Column is shown.
        /// </summary>
        private bool firstColumn;

        /// <summary>
        /// Indicates whether Last Column is shown.
        /// </summary>
        private bool lastColumn;

        /// <summary>
        /// Indicates whether Header Row is shown.
        /// </summary>
        private bool headerRow;

        /// <summary>
        /// Indicates whether the Row stripes are shown.
        /// </summary>
        private bool rowStripes;

        /// <summary>
        /// Indicates whether the Column Stripes are shown.
        /// </summary>
        private bool columnStripes;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TableStyleRenderer"/> class.
        /// </summary>
        public TableStyleRenderer()
        {
            this.colorFonts = new List<TableBuiltInStyles>();
            this.borderList = new Dictionary<TableBuiltInStyles, Color>();
            this.doubleBorderList = new Dictionary<TableBuiltInStyles, Color>();
            this.topSolidList = new Dictionary<TableBuiltInStyles, Color>();
            this.borderColorList = new Dictionary<IRange, Dictionary<ExcelBordersIndex, Color>>();
            this.fontColorCollections = new Dictionary<IRange, Color>();
            this.darkStyles = new List<TableBuiltInStyles>();
            this.mediumStyleWithBorder = new List<TableBuiltInStyles>();
            this.mediumStyleWithoutBorder = new List<TableBuiltInStyles>();
            this.lightStyleBorder = new List<TableBuiltInStyles>();
            this.mediumStyleBorder = new List<TableBuiltInStyles>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TableStyleRenderer"/> class.
        /// </summary>
        /// <param name="book">The IWorkbook object.</param>
        /// <param name="sheetListObjects">The sheet list objects.</param>
        public TableStyleRenderer(IListObjects sheetListObjects)
            : this()
        {
            this.listObjects = sheetListObjects;
            this.InitializeBorders();
            this.InitializeColorFonts();
            this.InitializeLightStyle();
            this.InitializeDoubleBorders();
            this.InitializeTopSolid();

        }

        #endregion

        #region Enums
        /// <summary>
        /// Enumeration of the Solid style in Borders
        /// </summary>
        private enum SolidStyle
        {
            /// <summary>
            /// Represents the No Solidstyle.
            /// </summary>
            None,

            /// <summary>
            /// Represents the solid style for the Top ExcelBorderIndex.
            /// </summary>
            Top,

            /// <summary>
            /// Represents the solid style for the Bottom ExcelBorderIndex.
            /// </summary>
            Bottom,

            /// <summary>
            /// Represents the solid style for the Right ExcelBorderIndex.
            /// </summary>
            Right,

            /// <summary>
            /// Represents the solid style for the Left ExcelBorderIndex.
            /// </summary>
            Left,

            /// <summary>
            /// Represents the solid style for both Top and Bottom ExcelBorderIndex.
            /// </summary>
            TopBottom,

            /// <summary>
            /// Represents the solid style for both Right and Left ExcelBorderIndex.
            /// </summary>
            LeftRight
        }

        /// <summary>
        /// Enumeration of the Border Styles.
        /// </summary>
        private enum BorderStyle
        {
            /// <summary>
            /// Represents that no ExcelBorderIndex is set.
            /// </summary>
            None,

            /// <summary>
            /// Represents that Top ExcelBorderIndex is set.
            /// </summary>
            Top,

            /// <summary>
            /// Represents that Bottom ExcelBorderIndex is set.
            /// </summary>
            Bottom,

            /// <summary>
            /// Represents that Left ExcelBorderIndex is set.
            /// </summary>
            Left,

            /// <summary>
            /// Represents that Right ExcelBorderIndex is set.
            /// </summary>
            Right,

            /// <summary>
            /// Represents that both Top and Bottom ExcelBorderIndex is set.
            /// </summary>
            TopBottom,

            /// <summary>
            /// Represents that both Left and Right ExcelBorderIndex is set.
            /// </summary>
            LeftRight,

            /// <summary>
            /// Represents that all ExcelBorderIndex is set.
            /// </summary>
            All
        }
        #endregion

        #region HelperMethods

        /// <summary>
        /// Applies the styles.
        /// </summary>
        /// <param name="sheet">The sheet.</param>
        /// <param name="fontColorCollection">The font color collection.</param>
        /// <returns>Range collection with the borders and their respective colors</returns>
        internal Dictionary<IRange, Dictionary<ExcelBordersIndex, Color>> ApplyStyles(IWorksheet sheet,
                                                                                      out Dictionary<IRange, Color> fontColorCollection)
        {
            foreach (IListObject listObject in this.listObjects)
            {
                
                this.columnStripes = listObject.ShowTableStyleColumnStripes;
                this.rowStripes = listObject.ShowTableStyleRowStripes;
                this.firstColumn = listObject.ShowFirstColumn;
                this.lastColumn = listObject.ShowLastColumn;
                this.headerRow = listObject.ShowHeaderRow;
                if (this.firstColumn || this.lastColumn)
                {
                    this.InitializeColumnSettings();
                }

                this.InitializeMediumStyle();
                this.DrawLocationAndStyle(sheet, listObject.BuiltInTableStyle, listObject.Location,
                                       listObject.ShowTotals);
            }

            fontColorCollection = this.fontColorCollections;
            return this.borderColorList;
        }

        /// <summary>
        /// Initializes the light style.
        /// </summary>
        private void InitializeLightStyle()
        {
            this.lightStyleBorder.Add(TableBuiltInStyles.TableStyleLight8);
            this.lightStyleBorder.Add(TableBuiltInStyles.TableStyleLight9);
            this.lightStyleBorder.Add(TableBuiltInStyles.TableStyleLight10);
            this.lightStyleBorder.Add(TableBuiltInStyles.TableStyleLight11);
            this.lightStyleBorder.Add(TableBuiltInStyles.TableStyleLight12);
            this.lightStyleBorder.Add(TableBuiltInStyles.TableStyleLight13);
            this.lightStyleBorder.Add(TableBuiltInStyles.TableStyleLight14);
        }

        /// <summary>
        /// Initializes the borders.
        /// </summary>
        private void InitializeBorders()
        {
            this.borderList.Add(TableBuiltInStyles.TableStyleLight1, Color.FromArgb(0, 0, 0));
            this.borderList.Add(TableBuiltInStyles.TableStyleLight2, Color.FromArgb(79, 129, 189));
            this.borderList.Add(TableBuiltInStyles.TableStyleLight3, Color.FromArgb(192, 80, 77));
            this.borderList.Add(TableBuiltInStyles.TableStyleLight4, Color.FromArgb(155, 187, 89));
            this.borderList.Add(TableBuiltInStyles.TableStyleLight5, Color.FromArgb(128, 100, 162));
            this.borderList.Add(TableBuiltInStyles.TableStyleLight6, Color.FromArgb(75, 172, 198));
            this.borderList.Add(TableBuiltInStyles.TableStyleLight7, Color.FromArgb(247, 150, 70));
            this.borderList.Add(TableBuiltInStyles.TableStyleMedium16, Color.Black);
            this.borderList.Add(TableBuiltInStyles.TableStyleMedium17, Color.Black);
            this.borderList.Add(TableBuiltInStyles.TableStyleMedium18, Color.Black);
            this.borderList.Add(TableBuiltInStyles.TableStyleMedium19, Color.Black);
            this.borderList.Add(TableBuiltInStyles.TableStyleMedium20, Color.Black);
            this.borderList.Add(TableBuiltInStyles.TableStyleMedium21, Color.Black);
        }

        /// <summary>
        /// Initializes the color fonts.
        /// </summary>
        private void InitializeColorFonts()
        {
            this.colorFonts.Add(TableBuiltInStyles.TableStyleLight2);
            this.colorFonts.Add(TableBuiltInStyles.TableStyleLight3);
            this.colorFonts.Add(TableBuiltInStyles.TableStyleLight4);
            this.colorFonts.Add(TableBuiltInStyles.TableStyleLight5);
            this.colorFonts.Add(TableBuiltInStyles.TableStyleLight6);
            this.colorFonts.Add(TableBuiltInStyles.TableStyleLight7);
        }

        /// <summary>
        /// Initializes the double borders.
        /// </summary>
        private void InitializeDoubleBorders()
        {
            this.doubleBorderList.Add(TableBuiltInStyles.TableStyleLight8, Color.FromArgb(0, 0, 0));
            this.doubleBorderList.Add(TableBuiltInStyles.TableStyleLight9, Color.FromArgb(79, 129, 189));
            this.doubleBorderList.Add(TableBuiltInStyles.TableStyleLight10, Color.FromArgb(192, 80, 77));
            this.doubleBorderList.Add(TableBuiltInStyles.TableStyleLight11, Color.FromArgb(155, 187, 89));
            this.doubleBorderList.Add(TableBuiltInStyles.TableStyleLight12, Color.FromArgb(128, 100, 162));
            this.doubleBorderList.Add(TableBuiltInStyles.TableStyleLight13, Color.FromArgb(75, 172, 198));
            this.doubleBorderList.Add(TableBuiltInStyles.TableStyleLight14, Color.FromArgb(247, 150, 70));
            this.doubleBorderList.Add(TableBuiltInStyles.TableStyleLight15, Color.FromArgb(0, 0, 0));
            this.doubleBorderList.Add(TableBuiltInStyles.TableStyleLight16, Color.FromArgb(79, 129, 189));
            this.doubleBorderList.Add(TableBuiltInStyles.TableStyleLight17, Color.FromArgb(192, 80, 77));
            this.doubleBorderList.Add(TableBuiltInStyles.TableStyleLight18, Color.FromArgb(155, 187, 89));
            this.doubleBorderList.Add(TableBuiltInStyles.TableStyleLight19, Color.FromArgb(128, 100, 162));
            this.doubleBorderList.Add(TableBuiltInStyles.TableStyleLight20, Color.FromArgb(75, 172, 198));
            this.doubleBorderList.Add(TableBuiltInStyles.TableStyleLight21, Color.FromArgb(247, 150, 70));
            this.doubleBorderList.Add(TableBuiltInStyles.TableStyleMedium1, Color.FromArgb(0, 0, 0));
            this.doubleBorderList.Add(TableBuiltInStyles.TableStyleMedium2, Color.FromArgb(79, 129, 189));
            this.doubleBorderList.Add(TableBuiltInStyles.TableStyleMedium3, Color.FromArgb(192, 80, 77));
            this.doubleBorderList.Add(TableBuiltInStyles.TableStyleMedium4, Color.FromArgb(155, 187, 89));
            this.doubleBorderList.Add(TableBuiltInStyles.TableStyleMedium5, Color.FromArgb(128, 100, 162));
            this.doubleBorderList.Add(TableBuiltInStyles.TableStyleMedium6, Color.FromArgb(75, 172, 198));
            this.doubleBorderList.Add(TableBuiltInStyles.TableStyleMedium7, Color.FromArgb(247, 150, 70));
            this.doubleBorderList.Add(TableBuiltInStyles.TableStyleMedium15, Color.FromArgb(0, 0, 0));
            this.doubleBorderList.Add(TableBuiltInStyles.TableStyleMedium16, Color.FromArgb(0, 0, 0));
            this.doubleBorderList.Add(TableBuiltInStyles.TableStyleMedium17, Color.FromArgb(0, 0, 0));
            this.doubleBorderList.Add(TableBuiltInStyles.TableStyleMedium18, Color.FromArgb(0, 0, 0));
            this.doubleBorderList.Add(TableBuiltInStyles.TableStyleMedium19, Color.FromArgb(0, 0, 0));
            this.doubleBorderList.Add(TableBuiltInStyles.TableStyleMedium20, Color.FromArgb(0, 0, 0));
            this.doubleBorderList.Add(TableBuiltInStyles.TableStyleMedium21, Color.FromArgb(0, 0, 0));
            this.doubleBorderList.Add(TableBuiltInStyles.TableStyleDark8, Color.FromArgb(0, 0, 0));
            this.doubleBorderList.Add(TableBuiltInStyles.TableStyleDark9, Color.FromArgb(0, 0, 0));
            this.doubleBorderList.Add(TableBuiltInStyles.TableStyleDark10, Color.FromArgb(0, 0, 0));
            this.doubleBorderList.Add(TableBuiltInStyles.TableStyleDark11, Color.FromArgb(0, 0, 0));
        }

        /// <summary>
        /// Initializes the top solid.
        /// </summary>
        private void InitializeTopSolid()
        {
            this.topSolidList.Add(TableBuiltInStyles.TableStyleMedium22, Color.FromArgb(0, 0, 0));
            this.topSolidList.Add(TableBuiltInStyles.TableStyleMedium23, Color.FromArgb(79, 129, 189));
            this.topSolidList.Add(TableBuiltInStyles.TableStyleMedium24, Color.FromArgb(192, 80, 77));
            this.topSolidList.Add(TableBuiltInStyles.TableStyleMedium25, Color.FromArgb(155, 187, 89));
            this.topSolidList.Add(TableBuiltInStyles.TableStyleMedium26, Color.FromArgb(128, 100, 162));
            this.topSolidList.Add(TableBuiltInStyles.TableStyleMedium27, Color.FromArgb(75, 172, 198));
            this.topSolidList.Add(TableBuiltInStyles.TableStyleMedium28, Color.FromArgb(247, 150, 70));
        }

        /// <summary>
        /// Initializes the column settings.
        /// </summary>
        private void InitializeColumnSettings()
        {
            this.darkStyles.Add(TableBuiltInStyles.TableStyleDark1);
            this.darkStyles.Add(TableBuiltInStyles.TableStyleDark2);
            this.darkStyles.Add(TableBuiltInStyles.TableStyleDark3);
            this.darkStyles.Add(TableBuiltInStyles.TableStyleDark4);
            this.darkStyles.Add(TableBuiltInStyles.TableStyleDark5);
            this.darkStyles.Add(TableBuiltInStyles.TableStyleDark6);
            this.darkStyles.Add(TableBuiltInStyles.TableStyleDark7);

            this.mediumStyleWithBorder.Add(TableBuiltInStyles.TableStyleMedium8);
            this.mediumStyleWithBorder.Add(TableBuiltInStyles.TableStyleMedium9);
            this.mediumStyleWithBorder.Add(TableBuiltInStyles.TableStyleMedium10);
            this.mediumStyleWithBorder.Add(TableBuiltInStyles.TableStyleMedium11);
            this.mediumStyleWithBorder.Add(TableBuiltInStyles.TableStyleMedium12);
            this.mediumStyleWithBorder.Add(TableBuiltInStyles.TableStyleMedium13);
            this.mediumStyleWithBorder.Add(TableBuiltInStyles.TableStyleMedium14);

            this.mediumStyleWithoutBorder.Add(TableBuiltInStyles.TableStyleMedium15);
            this.mediumStyleWithoutBorder.Add(TableBuiltInStyles.TableStyleMedium16);
            this.mediumStyleWithoutBorder.Add(TableBuiltInStyles.TableStyleMedium17);
            this.mediumStyleWithoutBorder.Add(TableBuiltInStyles.TableStyleMedium18);
            this.mediumStyleWithoutBorder.Add(TableBuiltInStyles.TableStyleMedium19);
            this.mediumStyleWithoutBorder.Add(TableBuiltInStyles.TableStyleMedium20);
            this.mediumStyleWithoutBorder.Add(TableBuiltInStyles.TableStyleMedium21);
        }

        /// <summary>
        /// Initializes the medium style.
        /// </summary>
        private void InitializeMediumStyle()
        {
            this.mediumStyleBorder.Add(TableBuiltInStyles.TableStyleMedium1);
            this.mediumStyleBorder.Add(TableBuiltInStyles.TableStyleMedium2);
            this.mediumStyleBorder.Add(TableBuiltInStyles.TableStyleMedium3);
            this.mediumStyleBorder.Add(TableBuiltInStyles.TableStyleMedium4);
            this.mediumStyleBorder.Add(TableBuiltInStyles.TableStyleMedium5);
            this.mediumStyleBorder.Add(TableBuiltInStyles.TableStyleMedium6);
            this.mediumStyleBorder.Add(TableBuiltInStyles.TableStyleMedium7);
            this.mediumStyleBorder.Add(TableBuiltInStyles.TableStyleMedium15);
        }

        /// <summary>
        /// Draws the table style.
        /// </summary>
        /// <param name="sheet">The worksheet.</param>
        /// <param name="listObjectBuiltInStyle">The built in style.</param>
        /// <param name="objectLocation">The list object location.</param>
        /// <param name="showTotals">if set to <c>true</c> [show totals].</param>
        private void DrawLocationAndStyle(IWorksheet sheet, TableBuiltInStyles listObjectBuiltInStyle,
                                       IRange objectLocation, bool showTotals)
        {
            this.builtInStyle = listObjectBuiltInStyle;
            switch (this.builtInStyle)
            {
                case TableBuiltInStyles.TableStyleLight1:
                    this.HeaderStyle(sheet, sheet.Range[objectLocation.Row, objectLocation.Column,
                                     objectLocation.Row, objectLocation.LastColumn].AddressLocal,
                                     BorderStyle.TopBottom, Color.Black, true, Color.Empty,
                                     SolidStyle.None, Color.Black);
                    this.ContentStyle(sheet, objectLocation, BorderStyle.None, Color.Empty,
                                      true, Color.FromArgb(217, 217, 217), Color.Empty,
                                      SolidStyle.None, Color.Black, showTotals);
                    this.TotalStyle(sheet, objectLocation, BorderStyle.TopBottom, Color.FromArgb(0, 0, 0), true,
                                    Color.FromArgb(255, 255, 255), SolidStyle.None, showTotals, Color.Black, true);
                    break;

                case TableBuiltInStyles.TableStyleLight2:
                    this.HeaderStyle(sheet, sheet.Range[objectLocation.Row, objectLocation.Column,
                                     objectLocation.Row, objectLocation.LastColumn].AddressLocal,
                                     BorderStyle.TopBottom, Color.FromArgb(79, 129, 189), true, Color.Empty,
                                     SolidStyle.None, Color.FromArgb(54, 96, 146));
                    this.ContentStyle(sheet, objectLocation, BorderStyle.None, Color.Empty, true,
                                      Color.FromArgb(220, 230, 241), Color.Empty, SolidStyle.None,
                                      Color.FromArgb(54, 96, 146), showTotals);
                    this.TotalStyle(sheet, objectLocation, BorderStyle.TopBottom, Color.FromArgb(79, 129, 189),
                                    true, Color.FromArgb(255, 255, 255), SolidStyle.None, showTotals,
                                    Color.FromArgb(54, 96, 146), true);
                    break;

                case TableBuiltInStyles.TableStyleLight3:
                    this.HeaderStyle(sheet, sheet.Range[objectLocation.Row, objectLocation.Column,
                                     objectLocation.Row, objectLocation.LastColumn].AddressLocal,
                                     BorderStyle.TopBottom, Color.FromArgb(192, 80, 77), true, Color.Empty,
                                     SolidStyle.None, Color.FromArgb(150, 54, 52));
                    this.ContentStyle(sheet, objectLocation, BorderStyle.None, Color.Empty, true,
                                      Color.FromArgb(242, 220, 219), Color.Empty, SolidStyle.None,
                                      Color.FromArgb(150, 54, 52), showTotals);
                    this.TotalStyle(sheet, objectLocation, BorderStyle.TopBottom, Color.FromArgb(192, 80, 77),
                                    true, Color.FromArgb(255, 255, 255), SolidStyle.None, showTotals,
                                    Color.FromArgb(150, 54, 52), true);
                    break;

                case TableBuiltInStyles.TableStyleLight4:
                    this.HeaderStyle(sheet, sheet.Range[objectLocation.Row, objectLocation.Column,
                                     objectLocation.Row, objectLocation.LastColumn].AddressLocal,
                                     BorderStyle.TopBottom, Color.FromArgb(155, 187, 89), true, Color.Empty,
                                     SolidStyle.None, Color.FromArgb(118, 147, 60));
                    this.ContentStyle(sheet, objectLocation, BorderStyle.None, Color.Empty, true,
                                      Color.FromArgb(235, 241, 222), Color.Empty, SolidStyle.None,
                                      Color.FromArgb(118, 147, 60), showTotals);
                    this.TotalStyle(sheet, objectLocation, BorderStyle.TopBottom, Color.FromArgb(155, 187, 89),
                                   true, Color.FromArgb(255, 255, 255), SolidStyle.None, showTotals,
                                   Color.FromArgb(118, 147, 60), true);
                    break;

                case TableBuiltInStyles.TableStyleLight5:
                    this.HeaderStyle(sheet, sheet.Range[objectLocation.Row, objectLocation.Column,
                                     objectLocation.Row, objectLocation.LastColumn].AddressLocal,
                                     BorderStyle.TopBottom, Color.FromArgb(128, 100, 162), true, Color.Empty,
                                     SolidStyle.None, Color.FromArgb(96, 73, 122));
                    this.ContentStyle(sheet, objectLocation, BorderStyle.None, Color.Empty, true,
                                      Color.FromArgb(228, 223, 236), Color.Empty, SolidStyle.None,
                                      Color.FromArgb(96, 73, 122), showTotals);
                    this.TotalStyle(sheet, objectLocation, BorderStyle.TopBottom, Color.FromArgb(128, 100, 162),
                                    true, Color.FromArgb(255, 255, 255), SolidStyle.None, showTotals,
                                    Color.FromArgb(96, 73, 122), true);
                    break;

                case TableBuiltInStyles.TableStyleLight6:
                    this.HeaderStyle(sheet, sheet.Range[objectLocation.Row, objectLocation.Column,
                                     objectLocation.Row, objectLocation.LastColumn].AddressLocal,
                                     BorderStyle.TopBottom, Color.FromArgb(75, 172, 198), true, Color.Empty,
                                     SolidStyle.None, Color.FromArgb(49, 134, 155));
                    this.ContentStyle(sheet, objectLocation, BorderStyle.None, Color.Empty, true,
                                      Color.FromArgb(218, 238, 243), Color.Empty, SolidStyle.None,
                                      Color.FromArgb(49, 134, 155), showTotals);
                    this.TotalStyle(sheet, objectLocation, BorderStyle.TopBottom, Color.FromArgb(75, 172, 198),
                                    true, Color.FromArgb(255, 255, 255), SolidStyle.None, showTotals,
                                    Color.FromArgb(49, 134, 155), true);
                    break;

                case TableBuiltInStyles.TableStyleLight7:
                    this.HeaderStyle(sheet, sheet.Range[objectLocation.Row, objectLocation.Column,
                                     objectLocation.Row, objectLocation.LastColumn].AddressLocal,
                                     BorderStyle.TopBottom, Color.FromArgb(247, 150, 70), true, Color.Empty,
                                     SolidStyle.None, Color.FromArgb(226, 107, 10));
                    this.ContentStyle(sheet, objectLocation, BorderStyle.None, Color.Empty, true,
                                      Color.FromArgb(253, 233, 217), Color.Empty, SolidStyle.None,
                                      Color.FromArgb(226, 107, 10), showTotals);
                    this.TotalStyle(sheet, objectLocation, BorderStyle.TopBottom, Color.FromArgb(247, 150, 70),
                                    true, Color.Empty, SolidStyle.None, showTotals, Color.FromArgb(226, 107, 10),
                                    true);
                    break;

                case TableBuiltInStyles.TableStyleLight8:
                    this.HeaderStyle(sheet, sheet.Range[objectLocation.Row, objectLocation.Column,
                                     objectLocation.Row, objectLocation.LastColumn].AddressLocal,
                                     BorderStyle.All, Color.FromArgb(0, 0, 0), true, Color.Black,
                                     SolidStyle.None, Color.White);
                    this.ContentStyle(sheet, objectLocation, BorderStyle.All, Color.Black, true, Color.Empty,
                                      Color.Empty, SolidStyle.None, Color.Black, showTotals);
                    this.TotalStyle(sheet, objectLocation, BorderStyle.All, Color.FromArgb(0, 0, 0), true,
                                    Color.Empty, SolidStyle.None, showTotals, Color.Black, true);
                    break;

                case TableBuiltInStyles.TableStyleLight9:
                    this.HeaderStyle(sheet, sheet.Range[objectLocation.Row, objectLocation.Column,
                                     objectLocation.Row, objectLocation.LastColumn].AddressLocal,
                                     BorderStyle.All, Color.FromArgb(79, 129, 189), true,
                                     Color.FromArgb(79, 129, 189), SolidStyle.None, Color.White);
                    this.ContentStyle(sheet, objectLocation, BorderStyle.All, Color.FromArgb(79, 129, 189), true,
                                      Color.Empty, Color.Empty, SolidStyle.None, Color.Black, showTotals);
                    this.TotalStyle(sheet, objectLocation, BorderStyle.All, Color.FromArgb(79, 129, 189), true,
                                    Color.Empty, SolidStyle.None, showTotals, Color.Black, true);
                    break;

                case TableBuiltInStyles.TableStyleLight10:
                    this.HeaderStyle(sheet, sheet.Range[objectLocation.Row, objectLocation.Column,
                                     objectLocation.Row, objectLocation.LastColumn].AddressLocal,
                                     BorderStyle.All, Color.FromArgb(192, 80, 77), true,
                                     Color.FromArgb(192, 80, 77), SolidStyle.None, Color.White);
                    this.ContentStyle(sheet, objectLocation, BorderStyle.All, Color.FromArgb(192, 80, 77), true,
                                      Color.Empty, Color.Empty, SolidStyle.None, Color.Black, showTotals);
                    this.TotalStyle(sheet, objectLocation, BorderStyle.All, Color.FromArgb(192, 80, 77), true,
                                    Color.Empty, SolidStyle.None, showTotals, Color.Black, true);
                    break;

                case TableBuiltInStyles.TableStyleLight11:
                    this.HeaderStyle(sheet, sheet.Range[objectLocation.Row, objectLocation.Column,
                                     objectLocation.Row, objectLocation.LastColumn].AddressLocal,
                                     BorderStyle.All, Color.FromArgb(155, 187, 89), true,
                                     Color.FromArgb(155, 187, 89), SolidStyle.None, Color.White);
                    this.ContentStyle(sheet, objectLocation, BorderStyle.All, Color.FromArgb(155, 187, 89), true,
                                      Color.Empty, Color.Empty, SolidStyle.None, Color.Black, showTotals);
                    this.TotalStyle(sheet, objectLocation, BorderStyle.All, Color.FromArgb(155, 187, 89), true,
                                    Color.Empty, SolidStyle.None, showTotals, Color.Black, true);
                    break;

                case TableBuiltInStyles.TableStyleLight12:
                    this.HeaderStyle(sheet, sheet.Range[objectLocation.Row, objectLocation.Column,
                                     objectLocation.Row, objectLocation.LastColumn].AddressLocal, BorderStyle.All,
                                     Color.FromArgb(128, 100, 162), true, Color.FromArgb(128, 100, 162),
                                     SolidStyle.None, Color.White);
                    this.ContentStyle(sheet, objectLocation, BorderStyle.All, Color.FromArgb(128, 100, 162), true,
                                      Color.Empty, Color.Empty, SolidStyle.None, Color.Black, showTotals);
                    this.TotalStyle(sheet, objectLocation, BorderStyle.All, Color.FromArgb(128, 100, 162), true,
                                    Color.Empty, SolidStyle.None, showTotals, Color.Black, true);
                    break;

                case TableBuiltInStyles.TableStyleLight13:
                    this.HeaderStyle(sheet, sheet.Range[objectLocation.Row, objectLocation.Column,
                                     objectLocation.Row, objectLocation.LastColumn].AddressLocal, BorderStyle.All,
                                     Color.FromArgb(75, 172, 198), true, Color.FromArgb(75, 172, 198),
                                     SolidStyle.None, Color.White);
                    this.ContentStyle(sheet, objectLocation, BorderStyle.All, Color.FromArgb(75, 172, 198), true,
                                      Color.Empty, Color.Empty, SolidStyle.None, Color.Black, showTotals);
                    this.TotalStyle(sheet, objectLocation, BorderStyle.All, Color.FromArgb(75, 172, 198), true,
                                    Color.Empty, SolidStyle.None, showTotals, Color.Black, true);
                    break;

                case TableBuiltInStyles.TableStyleLight14:
                    this.HeaderStyle(sheet, sheet.Range[objectLocation.Row, objectLocation.Column,
                                     objectLocation.Row, objectLocation.LastColumn].AddressLocal, BorderStyle.All,
                                     Color.FromArgb(247, 150, 70), true, Color.FromArgb(247, 150, 70),
                                     SolidStyle.None, Color.White);
                    this.ContentStyle(sheet, objectLocation, BorderStyle.All, Color.FromArgb(247, 150, 70), true,
                                      Color.Empty, Color.Empty, SolidStyle.None, Color.Black, showTotals);
                    this.TotalStyle(sheet, objectLocation, BorderStyle.All, Color.FromArgb(247, 150, 70), true,
                                    Color.Empty, SolidStyle.None, showTotals, Color.Black, true);
                    break;

                case TableBuiltInStyles.TableStyleLight15:
                    this.HeaderStyle(sheet, sheet.Range[objectLocation.Row, objectLocation.Column,
                                     objectLocation.Row, objectLocation.LastColumn].AddressLocal, BorderStyle.All,
                                     Color.FromArgb(0, 0, 0), false, Color.FromArgb(255, 255, 255),
                                     SolidStyle.None, Color.Black);
                    this.TotalStyle(sheet, objectLocation, BorderStyle.All, Color.FromArgb(0, 0, 0), false,
                                    Color.Empty, SolidStyle.None, showTotals, Color.Black, true);
                    this.ContentStyle(sheet, objectLocation, BorderStyle.All, Color.FromArgb(0, 0, 0), false,
                                      Color.FromArgb(217, 217, 217), Color.Empty, SolidStyle.None, Color.Black,
                                      showTotals);
                    break;

                case TableBuiltInStyles.TableStyleLight16:
                    this.HeaderStyle(sheet, sheet.Range[objectLocation.Row, objectLocation.Column,
                                     objectLocation.Row, objectLocation.LastColumn].AddressLocal, BorderStyle.All,
                                     Color.FromArgb(79, 129, 189), false, Color.FromArgb(255, 255, 255), SolidStyle.Bottom, Color.Black);
                    this.TotalStyle(sheet, objectLocation, BorderStyle.All, Color.FromArgb(79, 129, 189), false,
                                    Color.Empty, SolidStyle.None, showTotals, Color.Black, true);
                    this.ContentStyle(sheet, objectLocation, BorderStyle.All, Color.FromArgb(79, 129, 189), false,
                                      Color.FromArgb(220, 230, 241), Color.Empty, SolidStyle.None, Color.Black,
                                      showTotals);
                    break;

                case TableBuiltInStyles.TableStyleLight17:
                    this.HeaderStyle(sheet, sheet.Range[objectLocation.Row, objectLocation.Column,
                                     objectLocation.Row, objectLocation.LastColumn].AddressLocal, BorderStyle.All,
                                     Color.FromArgb(192, 80, 77), false, Color.FromArgb(255, 255, 255),
                                     SolidStyle.Bottom, Color.Black);
                    this.TotalStyle(sheet, objectLocation, BorderStyle.All, Color.FromArgb(192, 80, 77), false,
                                    Color.Empty, SolidStyle.None, showTotals, Color.Black, true);
                    this.ContentStyle(sheet, objectLocation, BorderStyle.All, Color.FromArgb(192, 80, 77), false,
                                      Color.FromArgb(242, 220, 219), Color.Empty, SolidStyle.None, Color.Black,
                                      showTotals);
                    break;

                case TableBuiltInStyles.TableStyleLight18:
                    this.HeaderStyle(sheet, sheet.Range[objectLocation.Row, objectLocation.Column,
                                     objectLocation.Row, objectLocation.LastColumn].AddressLocal, BorderStyle.All,
                                     Color.FromArgb(155, 187, 89), false, Color.FromArgb(255, 255, 255),
                                     SolidStyle.Bottom, Color.Black);
                    this.TotalStyle(sheet, objectLocation, BorderStyle.All, Color.FromArgb(155, 187, 89), false,
                                    Color.Empty, SolidStyle.None, showTotals, Color.Black, true);
                    this.ContentStyle(sheet, objectLocation, BorderStyle.All, Color.FromArgb(155, 187, 89), false,
                                      Color.FromArgb(235, 241, 222), Color.Empty, SolidStyle.None, Color.Black,
                                      showTotals);
                    break;

                case TableBuiltInStyles.TableStyleLight19:
                    this.HeaderStyle(sheet, sheet.Range[objectLocation.Row, objectLocation.Column,
                                     objectLocation.Row, objectLocation.LastColumn].AddressLocal, BorderStyle.All,
                                     Color.FromArgb(128, 100, 162), false, Color.FromArgb(255, 255, 255),
                                     SolidStyle.Bottom, Color.Black);
                    this.TotalStyle(sheet, objectLocation, BorderStyle.All, Color.FromArgb(128, 100, 162), false,
                                    Color.Empty, SolidStyle.None, showTotals, Color.Black, true);
                    this.ContentStyle(sheet, objectLocation, BorderStyle.All, Color.FromArgb(128, 100, 162), false,
                                      Color.FromArgb(228, 223, 236), Color.Empty, SolidStyle.None, Color.Black,
                                      showTotals);
                    break;

                case TableBuiltInStyles.TableStyleLight20:
                    this.HeaderStyle(sheet, sheet.Range[objectLocation.Row, objectLocation.Column,
                                     objectLocation.Row, objectLocation.LastColumn].AddressLocal, BorderStyle.All,
                                     Color.FromArgb(75, 172, 198), false, Color.FromArgb(255, 255, 255),
                                     SolidStyle.Bottom, Color.Black);
                    this.TotalStyle(sheet, objectLocation, BorderStyle.All, Color.FromArgb(75, 172, 198), false,
                                    Color.Empty, SolidStyle.None, showTotals, Color.Black, true);
                    this.ContentStyle(sheet, objectLocation, BorderStyle.All, Color.FromArgb(75, 172, 198), false,
                                      Color.FromArgb(218, 238, 243), Color.Empty, SolidStyle.None, Color.Black,
                                      showTotals);
                    break;

                case TableBuiltInStyles.TableStyleLight21:
                    this.HeaderStyle(sheet, sheet.Range[objectLocation.Row, objectLocation.Column,
                                     objectLocation.Row, objectLocation.LastColumn].AddressLocal, BorderStyle.All,
                                     Color.FromArgb(247, 150, 70), false, Color.FromArgb(255, 255, 255),
                                     SolidStyle.Bottom, Color.Black);
                    this.TotalStyle(sheet, objectLocation, BorderStyle.All, Color.FromArgb(247, 150, 70), false,
                                    Color.Empty, SolidStyle.None, showTotals, Color.Black, true);
                    this.ContentStyle(sheet, objectLocation, BorderStyle.All, Color.FromArgb(247, 150, 70), false,
                                      Color.FromArgb(253, 233, 217), Color.Empty, SolidStyle.None, Color.Black,
                                      showTotals);
                    break;

                case TableBuiltInStyles.TableStyleMedium1:
                    this.HeaderStyle(sheet, sheet.Range[objectLocation.Row, objectLocation.Column,
                                     objectLocation.Row, objectLocation.LastColumn].AddressLocal, BorderStyle.All,
                                     Color.FromArgb(0, 0, 0), true, Color.FromArgb(0, 0, 0), SolidStyle.None,
                                     Color.White);
                    this.TotalStyle(sheet, objectLocation, BorderStyle.All, Color.FromArgb(0, 0, 0), true,
                                    Color.Empty, SolidStyle.None, showTotals, Color.Black, true);
                    this.ContentStyle(sheet, objectLocation, BorderStyle.All, Color.FromArgb(0, 0, 0), true,
                                      Color.FromArgb(217, 217, 217), Color.Empty, SolidStyle.None, Color.Black,
                                      showTotals);
                    break;

                case TableBuiltInStyles.TableStyleMedium2:
                    this.HeaderStyle(sheet, sheet.Range[objectLocation.Row, objectLocation.Column,
                                     objectLocation.Row, objectLocation.LastColumn].AddressLocal, BorderStyle.All,
                                     Color.FromArgb(149, 179, 215), true, Color.FromArgb(79, 129, 189),
                                     SolidStyle.None, Color.White);
                    this.TotalStyle(sheet, objectLocation, BorderStyle.All, Color.FromArgb(149, 179, 215), true,
                                    Color.Empty, SolidStyle.None, showTotals, Color.Black, true);
                    this.ContentStyle(sheet, objectLocation, BorderStyle.All, Color.FromArgb(149, 179, 215), true,
                                      Color.FromArgb(220, 230, 241), Color.Empty, SolidStyle.None, Color.Black,
                                      showTotals);
                    break;

                case TableBuiltInStyles.TableStyleMedium3:
                    this.HeaderStyle(sheet, sheet.Range[objectLocation.Row, objectLocation.Column,
                                     objectLocation.Row, objectLocation.LastColumn].AddressLocal,
                                     BorderStyle.All, Color.FromArgb(218, 150, 148), true,
                                     Color.FromArgb(192, 80, 77), SolidStyle.None, Color.White);
                    this.TotalStyle(sheet, objectLocation, BorderStyle.All, Color.FromArgb(218, 150, 148), true,
                                    Color.Empty, SolidStyle.None, showTotals, Color.Black, true);
                    this.ContentStyle(sheet, objectLocation, BorderStyle.All, Color.FromArgb(218, 150, 148), true,
                                      Color.FromArgb(242, 220, 219), Color.Empty, SolidStyle.None, Color.Black,
                                      showTotals);
                    break;

                case TableBuiltInStyles.TableStyleMedium4:
                    this.HeaderStyle(sheet, sheet.Range[objectLocation.Row, objectLocation.Column,
                                     objectLocation.Row, objectLocation.LastColumn].AddressLocal, BorderStyle.All,
                                     Color.FromArgb(196, 215, 155), true, Color.FromArgb(155, 187, 89),
                                     SolidStyle.None, Color.White);
                    this.TotalStyle(sheet, objectLocation, BorderStyle.All, Color.FromArgb(196, 215, 155), true,
                                    Color.Empty, SolidStyle.None, showTotals, Color.Black, true);
                    this.ContentStyle(sheet, objectLocation, BorderStyle.All, Color.FromArgb(196, 215, 155), true,
                                      Color.FromArgb(235, 241, 222), Color.Empty, SolidStyle.None, Color.Black,
                                      showTotals);
                    break;

                case TableBuiltInStyles.TableStyleMedium5:
                    this.HeaderStyle(sheet, sheet.Range[objectLocation.Row, objectLocation.Column,
                                     objectLocation.Row, objectLocation.LastColumn].AddressLocal, BorderStyle.All,
                                     Color.FromArgb(177, 160, 199), true, Color.FromArgb(128, 100, 162),
                                     SolidStyle.None, Color.White);
                    this.TotalStyle(sheet, objectLocation, BorderStyle.All, Color.FromArgb(177, 160, 199), true,
                                    Color.Empty, SolidStyle.None, showTotals, Color.Black, true);
                    this.ContentStyle(sheet, objectLocation, BorderStyle.All, Color.FromArgb(177, 160, 199), true,
                                      Color.FromArgb(228, 223, 236), Color.Empty, SolidStyle.None, Color.Black,
                                      showTotals);
                    break;

                case TableBuiltInStyles.TableStyleMedium6:
                    this.HeaderStyle(sheet, sheet.Range[objectLocation.Row, objectLocation.Column,
                                     objectLocation.Row, objectLocation.LastColumn].AddressLocal, BorderStyle.All,
                                     Color.FromArgb(146, 205, 220), true, Color.FromArgb(75, 172, 198),
                                     SolidStyle.None, Color.White);
                    this.TotalStyle(sheet, objectLocation, BorderStyle.All, Color.FromArgb(146, 205, 220), true,
                                    Color.Empty, SolidStyle.None, showTotals, Color.Black, true);
                    this.ContentStyle(sheet, objectLocation, BorderStyle.All, Color.FromArgb(146, 205, 220), true,
                                      Color.FromArgb(218, 238, 243), Color.Empty, SolidStyle.None, Color.Black,
                                      showTotals);
                    break;

                case TableBuiltInStyles.TableStyleMedium7:
                    this.HeaderStyle(sheet, sheet.Range[objectLocation.Row, objectLocation.Column,
                                     objectLocation.Row, objectLocation.LastColumn].AddressLocal, BorderStyle.All,
                                     Color.FromArgb(250, 191, 143), true, Color.FromArgb(247, 150, 70),
                                     SolidStyle.None, Color.White);
                    this.TotalStyle(sheet, objectLocation, BorderStyle.All, Color.FromArgb(250, 191, 143), true,
                                    Color.Empty, SolidStyle.None, showTotals, Color.Black, true);
                    this.ContentStyle(sheet, objectLocation, BorderStyle.All, Color.FromArgb(250, 191, 143), true,
                                      Color.FromArgb(253, 233, 217), Color.Empty, SolidStyle.None, Color.Black,
                                      showTotals);
                    break;

                case TableBuiltInStyles.TableStyleMedium8:
                    this.HeaderStyle(sheet, sheet.Range[objectLocation.Row, objectLocation.Column,
                                     objectLocation.Row, objectLocation.LastColumn].AddressLocal, BorderStyle.All,
                                     Color.FromArgb(255, 255, 255), false, Color.FromArgb(0, 0, 0),
                                     SolidStyle.Bottom, Color.White);
                    this.ContentStyle(sheet, objectLocation, BorderStyle.All, Color.White, false,
                                      Color.FromArgb(166, 166, 166), Color.FromArgb(217, 217, 217),
                                      SolidStyle.None, Color.Black, showTotals);
                    this.TotalStyle(sheet, objectLocation, BorderStyle.All, Color.FromArgb(255, 255, 255), false,
                                    Color.FromArgb(0, 0, 0), SolidStyle.Top, showTotals, Color.White, true);
                    break;

                case TableBuiltInStyles.TableStyleMedium9:
                    this.HeaderStyle(sheet, sheet.Range[objectLocation.Row, objectLocation.Column,
                                     objectLocation.Row, objectLocation.LastColumn].AddressLocal, BorderStyle.All,
                                     Color.FromArgb(255, 255, 255), false, Color.FromArgb(79, 129, 189),
                                     SolidStyle.Bottom, Color.White);
                    this.ContentStyle(sheet, objectLocation, BorderStyle.All, Color.White, false,
                                      Color.FromArgb(184, 204, 228), Color.FromArgb(220, 230, 241),
                                      SolidStyle.None, Color.Black, showTotals);
                    this.TotalStyle(sheet, objectLocation, BorderStyle.All, Color.FromArgb(255, 255, 255), false,
                                    Color.FromArgb(79, 129, 189), SolidStyle.Top, showTotals, Color.White, true);
                    break;

                case TableBuiltInStyles.TableStyleMedium10:
                    this.HeaderStyle(sheet, sheet.Range[objectLocation.Row, objectLocation.Column,
                                     objectLocation.Row, objectLocation.LastColumn].AddressLocal, BorderStyle.All,
                                     Color.FromArgb(255, 255, 255), false, Color.FromArgb(192, 80, 77), SolidStyle.Bottom, Color.White);
                    this.ContentStyle(sheet, objectLocation, BorderStyle.All, Color.White, false,
                                      Color.FromArgb(230, 184, 183), Color.FromArgb(242, 220, 219),
                                      SolidStyle.None, Color.Black, showTotals);
                    this.TotalStyle(sheet, objectLocation, BorderStyle.All, Color.FromArgb(255, 255, 255), false,
                                    Color.FromArgb(192, 80, 77), SolidStyle.Top, showTotals, Color.White, true);
                    break;

                case TableBuiltInStyles.TableStyleMedium11:
                    this.HeaderStyle(sheet, sheet.Range[objectLocation.Row, objectLocation.Column,
                                     objectLocation.Row, objectLocation.LastColumn].AddressLocal, BorderStyle.All,
                                     Color.FromArgb(255, 255, 255), false, Color.FromArgb(155, 187, 89),
                                     SolidStyle.Bottom, Color.White);
                    this.ContentStyle(sheet, objectLocation, BorderStyle.All, Color.White, false,
                                      Color.FromArgb(216, 228, 188), Color.FromArgb(235, 241, 222),
                                      SolidStyle.None, Color.Black, showTotals);
                    this.TotalStyle(sheet, objectLocation, BorderStyle.All, Color.FromArgb(255, 255, 255),
                                    false, Color.FromArgb(155, 187, 89), SolidStyle.Top, showTotals,
                                    Color.White, true);
                    break;

                case TableBuiltInStyles.TableStyleMedium12:
                    this.HeaderStyle(sheet, sheet.Range[objectLocation.Row, objectLocation.Column,
                                     objectLocation.Row, objectLocation.LastColumn].AddressLocal, BorderStyle.All,
                                     Color.FromArgb(255, 255, 255), false, Color.FromArgb(128, 100, 162),
                                     SolidStyle.Bottom, Color.White);
                    this.ContentStyle(sheet, objectLocation, BorderStyle.All, Color.White, false,
                                      Color.FromArgb(204, 192, 218), Color.FromArgb(228, 223, 236),
                                      SolidStyle.None, Color.Black, showTotals);
                    this.TotalStyle(sheet, objectLocation, BorderStyle.All, Color.FromArgb(255, 255, 255), false,
                                    Color.FromArgb(128, 100, 162), SolidStyle.Top, showTotals, Color.White, true);
                    break;

                case TableBuiltInStyles.TableStyleMedium13:
                    this.HeaderStyle(sheet, sheet.Range[objectLocation.Row, objectLocation.Column,
                                     objectLocation.Row, objectLocation.LastColumn].AddressLocal, BorderStyle.All,
                                     Color.FromArgb(255, 255, 255), false, Color.FromArgb(75, 172, 198),
                                     SolidStyle.Bottom, Color.White);
                    this.ContentStyle(sheet, objectLocation, BorderStyle.All, Color.White, false,
                                      Color.FromArgb(183, 222, 232), Color.FromArgb(218, 238, 243),
                                      SolidStyle.None, Color.Black, showTotals);
                    this.TotalStyle(sheet, objectLocation, BorderStyle.All, Color.FromArgb(255, 255, 255), false,
                                    Color.FromArgb(75, 172, 198), SolidStyle.Top, showTotals, Color.White, true);
                    break;

                case TableBuiltInStyles.TableStyleMedium14:
                    this.HeaderStyle(sheet, sheet.Range[objectLocation.Row, objectLocation.Column,
                                     objectLocation.Row, objectLocation.LastColumn].AddressLocal, BorderStyle.All,
                                     Color.FromArgb(255, 255, 255), false, Color.FromArgb(247, 150, 70),
                                     SolidStyle.Bottom, Color.White);
                    this.ContentStyle(sheet, objectLocation, BorderStyle.All, Color.White, false,
                                      Color.FromArgb(252, 213, 180), Color.FromArgb(253, 233, 217),
                                      SolidStyle.None, Color.Black, showTotals);
                    this.TotalStyle(sheet, objectLocation, BorderStyle.All, Color.FromArgb(255, 255, 255), false,
                                    Color.FromArgb(247, 150, 70), SolidStyle.Top, showTotals, Color.White, true);
                    break;

                case TableBuiltInStyles.TableStyleMedium15:
                    this.HeaderStyle(sheet, sheet.Range[objectLocation.Row, objectLocation.Column,
                                     objectLocation.Row, objectLocation.LastColumn].AddressLocal, BorderStyle.All,
                                     Color.FromArgb(0, 0, 0), false, Color.FromArgb(0, 0, 0), SolidStyle.TopBottom,
                                     Color.White);
                    this.TotalStyle(sheet, objectLocation, BorderStyle.All, Color.FromArgb(0, 0, 0), false,
                                    Color.FromArgb(255, 255, 255), SolidStyle.Bottom, showTotals, Color.Black,
                                    false);
                    this.ContentStyle(sheet, objectLocation, BorderStyle.All, Color.Black, false,
                                      Color.FromArgb(217, 217, 217), Color.Empty, SolidStyle.None, Color.Black,
                                      showTotals);
                    break;

                case TableBuiltInStyles.TableStyleMedium16:
                    this.HeaderStyle(sheet, sheet.Range[objectLocation.Row, objectLocation.Column,
                                     objectLocation.Row, objectLocation.LastColumn].AddressLocal,
                                     BorderStyle.TopBottom, Color.FromArgb(0, 0, 0), true,
                                     Color.FromArgb(79, 129, 189), SolidStyle.TopBottom, Color.White);
                    this.ContentStyle(sheet, objectLocation, BorderStyle.None, Color.Empty, false,
                                      Color.FromArgb(217, 217, 217), Color.Empty, SolidStyle.None, Color.Black,
                                      showTotals);
                    this.TotalStyle(sheet, objectLocation, BorderStyle.TopBottom, Color.FromArgb(0, 0, 0), true,
                                    Color.FromArgb(255, 255, 255), SolidStyle.Bottom, showTotals, Color.Black,
                                    false);
                    break;

                case TableBuiltInStyles.TableStyleMedium17:
                    this.HeaderStyle(sheet, sheet.Range[objectLocation.Row, objectLocation.Column,
                                     objectLocation.Row, objectLocation.LastColumn].AddressLocal,
                                     BorderStyle.TopBottom, Color.FromArgb(0, 0, 0), true,
                                     Color.FromArgb(192, 80, 77), SolidStyle.TopBottom, Color.White);
                    this.ContentStyle(sheet, objectLocation, BorderStyle.None, Color.Empty, false,
                                      Color.FromArgb(217, 217, 217), Color.Empty, SolidStyle.None,
                                      Color.Black, showTotals);
                    this.TotalStyle(sheet, objectLocation, BorderStyle.TopBottom, Color.FromArgb(0, 0, 0), true,
                                    Color.FromArgb(255, 255, 255), SolidStyle.Bottom, showTotals, Color.Black,
                                    false);
                    break;

                case TableBuiltInStyles.TableStyleMedium18:
                    this.HeaderStyle(sheet, sheet.Range[objectLocation.Row, objectLocation.Column,
                                     objectLocation.Row, objectLocation.LastColumn].AddressLocal,
                                     BorderStyle.TopBottom, Color.FromArgb(0, 0, 0), true,
                                     Color.FromArgb(155, 187, 89), SolidStyle.TopBottom, Color.White);
                    this.ContentStyle(sheet, objectLocation, BorderStyle.None, Color.Empty, false,
                                      Color.FromArgb(217, 217, 217), Color.Empty, SolidStyle.None, Color.Black,
                                      showTotals);
                    this.TotalStyle(sheet, objectLocation, BorderStyle.TopBottom, Color.FromArgb(0, 0, 0), true,
                                    Color.FromArgb(255, 255, 255), SolidStyle.Bottom, showTotals, Color.Black,
                                    false);
                    break;

                case TableBuiltInStyles.TableStyleMedium19:
                    this.HeaderStyle(sheet, sheet.Range[objectLocation.Row, objectLocation.Column,
                                     objectLocation.Row, objectLocation.LastColumn].AddressLocal,
                                     BorderStyle.TopBottom, Color.FromArgb(0, 0, 0), true,
                                     Color.FromArgb(128, 100, 162), SolidStyle.TopBottom, Color.White);
                    this.ContentStyle(sheet, objectLocation, BorderStyle.None, Color.Empty, false,
                                      Color.FromArgb(217, 217, 217), Color.Empty, SolidStyle.None, Color.Black,
                                      showTotals);
                    this.TotalStyle(sheet, objectLocation, BorderStyle.TopBottom, Color.FromArgb(0, 0, 0), true,
                                    Color.FromArgb(255, 255, 255), SolidStyle.Bottom, showTotals, Color.Black,
                                    false);
                    break;

                case TableBuiltInStyles.TableStyleMedium20:
                    this.HeaderStyle(sheet, sheet.Range[objectLocation.Row, objectLocation.Column,
                                     objectLocation.Row, objectLocation.LastColumn].AddressLocal,
                                     BorderStyle.TopBottom, Color.FromArgb(0, 0, 0), true,
                                     Color.FromArgb(75, 172, 198), SolidStyle.TopBottom, Color.White);
                    this.ContentStyle(sheet, objectLocation, BorderStyle.None, Color.Empty, false,
                                      Color.FromArgb(217, 217, 217), Color.Empty, SolidStyle.None,
                                      Color.Black, showTotals);
                    this.TotalStyle(sheet, objectLocation, BorderStyle.TopBottom, Color.FromArgb(0, 0, 0),
                                    true, Color.FromArgb(255, 255, 255), SolidStyle.Bottom, showTotals,
                                    Color.Black, false);
                    break;

                case TableBuiltInStyles.TableStyleMedium21:
                    this.HeaderStyle(sheet, sheet.Range[objectLocation.Row, objectLocation.Column,
                                     objectLocation.Row, objectLocation.LastColumn].AddressLocal,
                                     BorderStyle.TopBottom, Color.FromArgb(0, 0, 0), true,
                                     Color.FromArgb(247, 150, 70), SolidStyle.TopBottom, Color.White);
                    this.ContentStyle(sheet, objectLocation, BorderStyle.None, Color.Empty, false,
                                      Color.FromArgb(217, 217, 217), Color.Empty, SolidStyle.None, Color.Black,
                                      showTotals);
                    this.TotalStyle(sheet, objectLocation, BorderStyle.TopBottom, Color.FromArgb(0, 0, 0), true,
                                    Color.FromArgb(255, 255, 255), SolidStyle.Bottom, showTotals, Color.Black,
                                    true);
                    break;

                case TableBuiltInStyles.TableStyleMedium22:
                    this.HeaderStyle(sheet, sheet.Range[objectLocation.Row, objectLocation.Column,
                                     objectLocation.Row, objectLocation.LastColumn].AddressLocal, BorderStyle.All,
                                     Color.FromArgb(0, 0, 0), false, Color.FromArgb(217, 217, 217),
                                     SolidStyle.None, Color.Black);
                    this.TotalStyle(sheet, objectLocation, BorderStyle.All, Color.FromArgb(0, 0, 0), false,
                                    Color.FromArgb(217, 217, 217), SolidStyle.Top, showTotals, Color.Black, true);
                    this.ContentStyle(sheet, objectLocation, BorderStyle.All, Color.FromArgb(0, 0, 0), false,
                                      Color.FromArgb(166, 166, 166), Color.FromArgb(217, 217, 217),
                                      SolidStyle.None, Color.Black, showTotals);
                    break;

                case TableBuiltInStyles.TableStyleMedium23:
                    this.HeaderStyle(sheet, sheet.Range[objectLocation.Row, objectLocation.Column,
                                     objectLocation.Row, objectLocation.LastColumn].AddressLocal, BorderStyle.All,
                                     Color.FromArgb(149, 179, 215), false, Color.FromArgb(220, 230, 241),
                                     SolidStyle.None, Color.Black);
                    this.TotalStyle(sheet, objectLocation, BorderStyle.All, Color.FromArgb(149, 179, 215), false,
                                    Color.FromArgb(220, 230, 241), SolidStyle.None, showTotals, Color.Black, true);
                    this.ContentStyle(sheet, objectLocation, BorderStyle.All, Color.FromArgb(149, 179, 215), false,
                                      Color.FromArgb(184, 201, 228), Color.FromArgb(220, 230, 241),
                                      SolidStyle.None, Color.Black, showTotals);
                    break;

                case TableBuiltInStyles.TableStyleMedium24:
                    this.HeaderStyle(sheet, sheet.Range[objectLocation.Row, objectLocation.Column,
                                     objectLocation.Row, objectLocation.LastColumn].AddressLocal, BorderStyle.All,
                                     Color.FromArgb(218, 150, 148), false, Color.FromArgb(242, 220, 219),
                                     SolidStyle.None, Color.Black);
                    this.TotalStyle(sheet, objectLocation, BorderStyle.All, Color.FromArgb(218, 150, 148), false,
                                    Color.FromArgb(242, 220, 219), SolidStyle.None, showTotals, Color.Black, true);
                    this.ContentStyle(sheet, objectLocation, BorderStyle.All, Color.FromArgb(218, 150, 148), false,
                                      Color.FromArgb(230, 184, 183), Color.FromArgb(242, 220, 219),
                                      SolidStyle.None, Color.Black, showTotals);
                    break;

                case TableBuiltInStyles.TableStyleMedium25:
                    this.HeaderStyle(sheet, sheet.Range[objectLocation.Row, objectLocation.Column,
                                     objectLocation.Row, objectLocation.LastColumn].AddressLocal, BorderStyle.All,
                                     Color.FromArgb(196, 215, 155), false, Color.FromArgb(235, 241, 222),
                                     SolidStyle.None, Color.Black);
                    this.TotalStyle(sheet, objectLocation, BorderStyle.All, Color.FromArgb(196, 215, 155), false,
                                    Color.FromArgb(235, 241, 222), SolidStyle.None, showTotals, Color.Black, true);
                    this.ContentStyle(sheet, objectLocation, BorderStyle.All, Color.FromArgb(196, 215, 155),
                                      false, Color.FromArgb(216, 228, 188), Color.FromArgb(235, 241, 222),
                                      SolidStyle.None, Color.Black, showTotals);
                    break;

                case TableBuiltInStyles.TableStyleMedium26:
                    this.HeaderStyle(sheet, sheet.Range[objectLocation.Row, objectLocation.Column,
                                     objectLocation.Row, objectLocation.LastColumn].AddressLocal, BorderStyle.All,
                                     Color.FromArgb(177, 160, 199), false, Color.FromArgb(228, 223, 236),
                                     SolidStyle.None, Color.Black);
                    this.TotalStyle(sheet, objectLocation, BorderStyle.All, Color.FromArgb(177, 160, 199), false,
                                    Color.FromArgb(228, 223, 236), SolidStyle.None, showTotals, Color.Black, true);
                    this.ContentStyle(sheet, objectLocation, BorderStyle.All, Color.FromArgb(177, 160, 199),
                                      false, Color.FromArgb(204, 192, 218), Color.FromArgb(228, 223, 236),
                                      SolidStyle.None, Color.Black, showTotals);
                    break;

                case TableBuiltInStyles.TableStyleMedium27:
                    this.HeaderStyle(sheet, sheet.Range[objectLocation.Row, objectLocation.Column,
                                     objectLocation.Row, objectLocation.LastColumn].AddressLocal, BorderStyle.All,
                                     Color.FromArgb(146, 205, 220), false, Color.FromArgb(218, 238, 243),
                                     SolidStyle.None, Color.Black);
                    this.TotalStyle(sheet, objectLocation, BorderStyle.All, Color.FromArgb(146, 205, 220), false,
                                    Color.FromArgb(218, 238, 243), SolidStyle.None, showTotals, Color.Black, true);
                    this.ContentStyle(sheet, objectLocation, BorderStyle.All, Color.FromArgb(146, 205, 220),
                                      false, Color.FromArgb(183, 222, 232), Color.FromArgb(218, 238, 243),
                                      SolidStyle.None, Color.Black, showTotals);
                    break;

                case TableBuiltInStyles.TableStyleMedium28:
                    this.HeaderStyle(sheet, sheet.Range[objectLocation.Row, objectLocation.Column,
                                     objectLocation.Row, objectLocation.LastColumn].AddressLocal, BorderStyle.All,
                                     Color.FromArgb(250, 191, 143), false, Color.FromArgb(253, 233, 217),
                                     SolidStyle.None, Color.Black);
                    this.TotalStyle(sheet, objectLocation, BorderStyle.All, Color.FromArgb(250, 191, 143), false,
                                    Color.FromArgb(253, 233, 217), SolidStyle.None, showTotals, Color.Black, true);
                    this.ContentStyle(sheet, objectLocation, BorderStyle.All, Color.FromArgb(250, 191, 143),
                                      false, Color.FromArgb(252, 213, 180), Color.FromArgb(253, 233, 217),
                                      SolidStyle.None, Color.Black, showTotals);
                    break;

                case TableBuiltInStyles.TableStyleDark1:
                    this.HeaderStyle(sheet, sheet.Range[objectLocation.Row, objectLocation.Column,
                                     objectLocation.Row, objectLocation.LastColumn].AddressLocal,
                                     BorderStyle.Bottom, Color.FromArgb(255, 255, 255), true,
                                     Color.FromArgb(0, 0, 0), SolidStyle.Bottom, Color.White);
                    this.ContentStyle(sheet, objectLocation, BorderStyle.None, Color.Empty, false,
                                      Color.FromArgb(64, 64, 64), Color.FromArgb(115, 115, 115),
                                      SolidStyle.None, Color.White, showTotals);
                    this.TotalStyle(sheet, objectLocation, BorderStyle.Top, Color.FromArgb(255, 255, 255),
                                    true, Color.FromArgb(38, 38, 38), SolidStyle.Top, showTotals, Color.White,
                                    true);
                    break;

                case TableBuiltInStyles.TableStyleDark2:
                    this.HeaderStyle(sheet, sheet.Range[objectLocation.Row, objectLocation.Column,
                                     objectLocation.Row, objectLocation.LastColumn].AddressLocal,
                                     BorderStyle.Bottom, Color.FromArgb(255, 255, 255), true,
                                     Color.FromArgb(0, 0, 0), SolidStyle.Bottom, Color.White);
                    this.ContentStyle(sheet, objectLocation, BorderStyle.None, Color.Empty, false,
                                      Color.FromArgb(54, 96, 146), Color.FromArgb(79, 129, 189), SolidStyle.None,
                                      Color.White, showTotals);
                    this.TotalStyle(sheet, objectLocation, BorderStyle.Top, Color.FromArgb(255, 255, 255), true,
                                    Color.FromArgb(36, 64, 98), SolidStyle.Top, showTotals, Color.White, true);
                    break;

                case TableBuiltInStyles.TableStyleDark3:
                    this.HeaderStyle(sheet, sheet.Range[objectLocation.Row, objectLocation.Column,
                                     objectLocation.Row, objectLocation.LastColumn].AddressLocal,
                                     BorderStyle.Bottom, Color.FromArgb(255, 255, 255), true,
                                     Color.FromArgb(0, 0, 0), SolidStyle.Bottom, Color.White);
                    this.ContentStyle(sheet, objectLocation, BorderStyle.None, Color.Empty, false,
                                      Color.FromArgb(150, 54, 52), Color.FromArgb(192, 80, 77), SolidStyle.None,
                                      Color.White, showTotals);
                    this.TotalStyle(sheet, objectLocation, BorderStyle.Top, Color.FromArgb(255, 255, 255), true,
                                    Color.FromArgb(99, 37, 35), SolidStyle.Top, showTotals, Color.White, true);
                    break;

                case TableBuiltInStyles.TableStyleDark4:
                    this.HeaderStyle(sheet, sheet.Range[objectLocation.Row, objectLocation.Column,
                                     objectLocation.Row, objectLocation.LastColumn].AddressLocal,
                                     BorderStyle.Bottom, Color.FromArgb(255, 255, 255), true,
                                     Color.FromArgb(0, 0, 0), SolidStyle.Bottom, Color.White);
                    this.ContentStyle(sheet, objectLocation, BorderStyle.None, Color.Empty, false,
                                      Color.FromArgb(118, 147, 60), Color.FromArgb(155, 187, 89),
                                      SolidStyle.None, Color.White, showTotals);
                    this.TotalStyle(sheet, objectLocation, BorderStyle.Top, Color.FromArgb(255, 255, 255),
                                    true, Color.FromArgb(79, 98, 40), SolidStyle.Top, showTotals, Color.White,
                                    true);
                    break;

                case TableBuiltInStyles.TableStyleDark5:
                    this.HeaderStyle(sheet, sheet.Range[objectLocation.Row, objectLocation.Column,
                                     objectLocation.Row, objectLocation.LastColumn].AddressLocal,
                                     BorderStyle.Bottom, Color.FromArgb(255, 255, 255), true,
                                     Color.FromArgb(0, 0, 0), SolidStyle.Bottom, Color.White);
                    this.ContentStyle(sheet, objectLocation, BorderStyle.None, Color.Empty, false,
                                      Color.FromArgb(96, 73, 122), Color.FromArgb(128, 100, 162), SolidStyle.None,
                                      Color.White, showTotals);
                    this.TotalStyle(sheet, objectLocation, BorderStyle.Top, Color.FromArgb(255, 255, 255), true,
                                    Color.FromArgb(64, 49, 81), SolidStyle.Top, showTotals, Color.White, true);
                    break;

                case TableBuiltInStyles.TableStyleDark6:
                    this.HeaderStyle(sheet, sheet.Range[objectLocation.Row, objectLocation.Column,
                                     objectLocation.Row, objectLocation.LastColumn].AddressLocal,
                                     BorderStyle.Bottom, Color.FromArgb(255, 255, 255), true,
                                     Color.FromArgb(0, 0, 0), SolidStyle.Bottom, Color.White);
                    this.ContentStyle(sheet, objectLocation, BorderStyle.None, Color.Empty, false,
                                      Color.FromArgb(49, 134, 155), Color.FromArgb(75, 172, 198),
                                      SolidStyle.None, Color.White, showTotals);
                    this.TotalStyle(sheet, objectLocation, BorderStyle.Top, Color.FromArgb(255, 255, 255), true,
                                    Color.FromArgb(33, 89, 103), SolidStyle.Top, showTotals, Color.White, true);
                    break;

                case TableBuiltInStyles.TableStyleDark7:
                    this.HeaderStyle(sheet, sheet.Range[objectLocation.Row, objectLocation.Column,
                                     objectLocation.Row, objectLocation.LastColumn].AddressLocal,
                                     BorderStyle.Bottom, Color.FromArgb(255, 255, 255), true,
                                     Color.FromArgb(0, 0, 0), SolidStyle.Bottom, Color.White);
                    this.ContentStyle(sheet, objectLocation, BorderStyle.None, Color.Empty, false,
                                      Color.FromArgb(226, 107, 10), Color.FromArgb(247, 150, 70), SolidStyle.None,
                                      Color.White, showTotals);
                    this.TotalStyle(sheet, objectLocation, BorderStyle.Top, Color.FromArgb(255, 255, 255), true,
                                    Color.FromArgb(151, 71, 6), SolidStyle.Top, showTotals, Color.White, true);
                    break;

                case TableBuiltInStyles.TableStyleDark8:
                    this.HeaderStyle(sheet, sheet.Range[objectLocation.Row, objectLocation.Column,
                                     objectLocation.Row, objectLocation.LastColumn].AddressLocal,
                                     BorderStyle.None, Color.Empty, true, Color.FromArgb(0, 0, 0),
                                     SolidStyle.None, Color.White);
                    this.ContentStyle(sheet, objectLocation, BorderStyle.None, Color.Empty, false,
                                      Color.FromArgb(166, 166, 166), Color.FromArgb(217, 217, 217),
                                      SolidStyle.None, Color.Black, showTotals);
                    this.TotalStyle(sheet, objectLocation, BorderStyle.Top, Color.FromArgb(0, 0, 0), true,
                                    Color.FromArgb(217, 217, 217), SolidStyle.None, showTotals, Color.Black, true);
                    break;

                case TableBuiltInStyles.TableStyleDark9:
                    this.HeaderStyle(sheet, sheet.Range[objectLocation.Row, objectLocation.Column,
                                     objectLocation.Row, objectLocation.LastColumn].AddressLocal,
                                     BorderStyle.None, Color.Empty, true, Color.FromArgb(192, 80, 77),
                                     SolidStyle.None, Color.White);
                    this.ContentStyle(sheet, objectLocation, BorderStyle.None, Color.Empty, false,
                                      Color.FromArgb(184, 204, 228), Color.FromArgb(220, 230, 241),
                                      SolidStyle.None, Color.Black, showTotals);
                    this.TotalStyle(sheet, objectLocation, BorderStyle.Top, Color.FromArgb(0, 0, 0), true,
                                    Color.FromArgb(220, 230, 241), SolidStyle.None, showTotals, Color.Black, true);
                    break;

                case TableBuiltInStyles.TableStyleDark10:
                    this.HeaderStyle(sheet, sheet.Range[objectLocation.Row, objectLocation.Column,
                                     objectLocation.Row, objectLocation.LastColumn].AddressLocal,
                                     BorderStyle.None, Color.Empty, true, Color.FromArgb(128, 100, 162),
                                     SolidStyle.None, Color.White);
                    this.ContentStyle(sheet, objectLocation, BorderStyle.None, Color.Empty, false,
                                      Color.FromArgb(216, 228, 188), Color.FromArgb(235, 241, 222),
                                      SolidStyle.None, Color.Black, showTotals);
                    this.TotalStyle(sheet, objectLocation, BorderStyle.Top, Color.FromArgb(0, 0, 0), true,
                                    Color.FromArgb(235, 241, 222), SolidStyle.None, showTotals, Color.Black, true);
                    break;

                case TableBuiltInStyles.TableStyleDark11:
                    this.HeaderStyle(sheet, sheet.Range[objectLocation.Row, objectLocation.Column,
                                     objectLocation.Row, objectLocation.LastColumn].AddressLocal, BorderStyle.None,
                                     Color.Empty, true, Color.FromArgb(247, 150, 70), SolidStyle.None,
                                     Color.White);
                    this.ContentStyle(sheet, objectLocation, BorderStyle.None, Color.Empty, false,
                                      Color.FromArgb(183, 222, 232), Color.FromArgb(218, 238, 243),
                                      SolidStyle.None, Color.Black, showTotals);
                    this.TotalStyle(sheet, objectLocation, BorderStyle.Top, Color.FromArgb(0, 0, 0), true,
                                    Color.FromArgb(218, 238, 243), SolidStyle.None, showTotals, Color.Black, true);
                    break;

                default:
                    this.HeaderStyle(sheet, sheet.Range[objectLocation.Row, objectLocation.Column,
                                     objectLocation.Row, objectLocation.LastColumn].AddressLocal, BorderStyle.All,
                                     Color.FromArgb(149, 179, 215), true, Color.FromArgb(79, 129, 189),
                                     SolidStyle.None, Color.White);
                    this.ContentStyle(sheet, objectLocation, BorderStyle.All, Color.FromArgb(149, 179, 215), true,
                                      Color.FromArgb(220, 230, 241), Color.Empty, SolidStyle.None, Color.Black,
                                      showTotals);
                    this.TotalStyle(sheet, objectLocation, BorderStyle.All, Color.FromArgb(149, 179, 215), true,
                                    Color.Empty, SolidStyle.None, showTotals, Color.Black, true);
                    break;
            }
        }

        /// <summary>
        /// Renders the style to the total part of the table.
        /// </summary>
        /// <param name="sheet">The worksheet.</param>
        /// <param name="objectLocation">The listobject location.</param>
        /// <param name="borderStyle">The border style.</param>
        /// <param name="borderColor">Color of the border.</param>
        /// <param name="entireRow">if set to <c>true</c> [entire row].</param>
        /// <param name="backgroundColor">Color of the background.</param>
        /// <param name="solidStyle">The solid style.</param>
        /// <param name="showTotals">if set to <c>true</c> [show totals].</param>
        /// <param name="fontColor">Color of the font.</param>
        /// <param name="boldFont">if set to <c>true</c> [bold font].</param>
        private void TotalStyle(IWorksheet sheet, IRange objectLocation, BorderStyle borderStyle,
                                Color borderColor, bool entireRow, Color backgroundColor, SolidStyle solidStyle,
                                bool showTotals, Color fontColor, bool boldFont)
        {
            if (showTotals)
            {
                IRange totalRange = sheet.Range[objectLocation.LastRow, objectLocation.Column,
                                                objectLocation.LastRow, objectLocation.LastColumn];
                if (this.mediumStyleWithoutBorder.Contains(this.builtInStyle))
                {
                    if (this.lastColumn && this.firstColumn)
                    {
                        totalRange = sheet.Range[totalRange.Row, totalRange.Column + 1, totalRange.LastRow,
                                                 totalRange.LastColumn - 1];
                    }
                    else if (this.firstColumn)
                    {
                        totalRange = sheet.Range[totalRange.Row, totalRange.Column + 1, totalRange.LastRow,
                                                 totalRange.LastColumn];
                    }
                    else if (this.lastColumn)
                    {
                        totalRange = sheet.Range[totalRange.Row, totalRange.Column, totalRange.LastRow,
                                                 totalRange.LastColumn - 1];
                    }
                }

                this.ApplyFirstLastColumnBorder(sheet, totalRange, solidStyle, borderStyle, borderColor,
                                                entireRow);
                totalRange.CellStyle.Font.Bold = boldFont;
                if (this.colorFonts.Contains(this.builtInStyle))
                {
                    this.fontColorCollections.Add(totalRange, fontColor);
                }
                else
                {
                    totalRange.CellStyle.Font.RGBColor = fontColor;
                }

                if (!backgroundColor.IsEmpty)
                {
                    totalRange.CellStyle.Color = backgroundColor;
                }
            }
        }

        /// <summary>
        /// Renders the style to the header part of the table.
        /// </summary>
        /// <param name="sheet">The Worksheet.</param>
        /// <param name="headerRange">The header range.</param>
        /// <param name="headerBorderStyle">The header border style.</param>
        /// <param name="borderColor">Color of the border.</param>
        /// <param name="entireRow">if set to <c>true</c> [entire row].</param>
        /// <param name="backgroundColor">Color of the background.</param>
        /// <param name="solidLineStyle">The solid line style.</param>
        /// <param name="fontColor">Color of the font.</param>
        private void HeaderStyle(IWorksheet sheet, string headerRange, BorderStyle headerBorderStyle,
                                 Color borderColor, bool entireRow, Color backgroundColor,
                                 SolidStyle solidLineStyle, Color fontColor)
        {
            IRange range = sheet.Range[headerRange];
            if (this.mediumStyleWithoutBorder != null)
            {
                if (this.mediumStyleWithoutBorder.Contains(this.builtInStyle) ||
                    this.mediumStyleWithBorder.Contains(this.builtInStyle))
                {
                    this.headerColor = backgroundColor;
                }
            }

            if (!this.headerRow)
            {
                range.Text = string.Empty;
                this.ApplyBorder(sheet, range, solidLineStyle, BorderStyle.Bottom, borderColor, entireRow);
                return;
            }

            this.ApplyBorder(sheet, range, solidLineStyle, headerBorderStyle, borderColor, entireRow);
            if (this.colorFonts.Contains(this.builtInStyle))
            {
                this.fontColorCollections.Add(range, fontColor);
            }
            else
            {
                range.CellStyle.Font.RGBColor = fontColor;
            }

            range.CellStyle.Font.Bold = true;
            if (backgroundColor != Color.Empty && range.CellStyle.FillPattern == ExcelPattern.None)
            {
                range.CellStyle.Color = backgroundColor;
            }
        }

        /// <summary>
        /// Renders the style to the content part of the table.
        /// </summary>
        /// <param name="sheet">The Worksheet.</param>
        /// <param name="headerRange">The header range.</param>
        /// <param name="contentBorderStyle">The content border style.</param>
        /// <param name="borderColor">Color of the border.</param>
        /// <param name="entireRow">if set to <c>true</c> [entire row].</param>
        /// <param name="backgroundFirstColor">First color of the Background.</param>
        /// <param name="backgroundSecondColor">Second color of the Background.</param>
        /// <param name="solidLineStyle">The solid line style.</param>
        /// <param name="fontColor">Color of the font.</param>
        /// <param name="showTotals">if set to <c>true</c> [show totals].</param>
        private void ContentStyle(IWorksheet sheet, IRange headerRange, BorderStyle contentBorderStyle,
                                  Color borderColor, bool entireRow, Color backgroundFirstColor,
                                  Color backgroundSecondColor, SolidStyle solidLineStyle, Color fontColor,
                                  bool showTotals)
        {
            IRange contentRange;
            contentRange = showTotals == true
                                        ? sheet.Range[headerRange.Row + 1, headerRange.Column, headerRange.LastRow - 1, headerRange.LastColumn]
                                        : sheet.Range[headerRange.Row + 1, headerRange.Column, headerRange.LastRow, headerRange.LastColumn];

            if (this.firstColumn || this.lastColumn)
            {
                contentRange = this.ApplyFirstLastColumnStyle(sheet, contentRange, fontColor,
                                                              backgroundFirstColor, showTotals);
            }

            if (this.colorFonts.Contains(this.builtInStyle))
            {
                this.fontColorCollections.Add(contentRange, fontColor);
            }
            else
            {
                contentRange.CellStyle.Font.RGBColor = fontColor;
            }

            bool odd = contentRange.Row % 2 == 0 ? false : true;
            if (!this.columnStripes && !this.rowStripes)
            {
                if (this.lightStyleBorder.Contains(this.builtInStyle))
                {
                    this.ApplyBorder(sheet, sheet.Range[contentRange.Row, contentRange.Column,
                                     contentRange.LastRow, contentRange.Column], SolidStyle.None, BorderStyle.Left,
                                     borderColor, false);
                    this.ApplyBorder(sheet, sheet.Range[contentRange.Row, contentRange.LastColumn,
                                     contentRange.LastRow, contentRange.LastColumn], SolidStyle.None,
                                     BorderStyle.Right, borderColor, false);
                    this.ApplyBorder(sheet, sheet.Range[contentRange.LastRow, contentRange.Column,
                                     contentRange.LastRow, contentRange.LastColumn], SolidStyle.None,
                                     BorderStyle.Bottom, borderColor, false);
                }
                else
                {
                    this.ApplyBorder(sheet, contentRange, solidLineStyle, contentBorderStyle, borderColor,
                                      entireRow);
                }

                if (backgroundSecondColor != Color.Empty)
                {
                    contentRange.CellStyle.Color = backgroundSecondColor;
                }
            }

            if (this.columnStripes && this.rowStripes)
            {
                if (this.lightStyleBorder.Contains(this.builtInStyle))
                {
                    entireRow = false;
                }

                if (this.mediumStyleBorder.Contains(this.builtInStyle))
                {
                    if (odd)
                    {
                        for (int i = contentRange.Row; i <= contentRange.LastRow; i++)
                        {
                            if (i % 2 == 0)
                            {
                                IRange entireSecondRange = sheet.Range[i, contentRange.Column, i,
                                                                       contentRange.LastColumn];
                                this.ApplyBorder(sheet, entireSecondRange, solidLineStyle, contentBorderStyle,
                                                 borderColor, entireRow);
                            }
                            else
                            {
                                IRange entireFirstRange = sheet.Range[i, contentRange.Column, i,
                                                                      contentRange.LastColumn];
                                this.ApplyBorder(sheet, entireFirstRange, solidLineStyle, contentBorderStyle,
                                                 borderColor, entireRow);
                            }
                        }

                    }
                    else
                    {
                        for (int i = contentRange.Row; i <= contentRange.LastRow; i++)
                        {
                            if (i % 2 == 0)
                            {
                                IRange entireFirstRange = sheet.Range[i, contentRange.Column, i,
                                                                      contentRange.LastColumn];
                                this.ApplyBorder(sheet, entireFirstRange, solidLineStyle, contentBorderStyle,
                                                 borderColor, entireRow);
                            }
                            else
                            {
                                IRange entireSecondRange = sheet.Range[i, contentRange.Column, i,
                                                                       contentRange.LastColumn];
                                this.ApplyBorder(sheet, entireSecondRange, solidLineStyle, contentBorderStyle,
                                                 borderColor, entireRow);
                            }
                        }
                    }
                }

                if (this.firstColumn)
                {
                    odd = (contentRange.Column - 1) % 2 == 0
                                        ? false
                                        : true;
                }
                else
                {
                    odd = contentRange.Column % 2 == 0
                                        ? false
                                        : true;
                }

                if (odd)
                {
                    for (int i = contentRange.Column; i <= contentRange.LastColumn; i++)
                    {
                        if (i % 2 == 0)
                        {
                            IRange entireSecondRange = sheet.Range[contentRange.Row, i, contentRange.LastRow, i];
                            if (!this.mediumStyleBorder.Contains(this.builtInStyle))
                            {
                                this.ApplyBorder(sheet, entireSecondRange, solidLineStyle, contentBorderStyle,
                                                 borderColor, entireRow);
                            }

                            if (entireSecondRange.Row % 2 == 0)
                            {
                                for (int index = entireSecondRange.Row; index <= entireSecondRange.LastRow; index++)
                                {
                                    IRange stripedRC = sheet.Range[index, entireSecondRange.Column, index,
                                                                   entireSecondRange.LastColumn];
                                    if (index % 2 == 0)
                                    {
                                        if (backgroundFirstColor != Color.Empty)
                                        {
                                            stripedRC.CellStyle.Color = backgroundFirstColor;
                                        }
                                    }
                                    else
                                    {
                                        if (backgroundSecondColor != Color.Empty)
                                        {
                                            stripedRC.CellStyle.Color = backgroundSecondColor;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                for (int index = entireSecondRange.Row; index <= entireSecondRange.LastRow; index++)
                                {
                                    IRange stripedRC = sheet.Range[index, entireSecondRange.Column, index,
                                                                   entireSecondRange.LastColumn];
                                    if (index % 2 == 0)
                                    {
                                        if (backgroundSecondColor != Color.Empty)
                                        {
                                            stripedRC.CellStyle.Color = backgroundSecondColor;
                                        }
                                    }
                                    else
                                    {
                                        if (backgroundFirstColor != Color.Empty)
                                        {
                                            stripedRC.CellStyle.Color = backgroundFirstColor;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            IRange entireFirstRange = sheet.Range[contentRange.Row, i, contentRange.LastRow, i];
                            if (!this.mediumStyleBorder.Contains(this.builtInStyle))
                            {
                                this.ApplyBorder(sheet, entireFirstRange, solidLineStyle, contentBorderStyle,
                                                 borderColor, entireRow);
                            }

                            if (backgroundFirstColor != Color.Empty)
                            {
                                entireFirstRange.CellStyle.Color = backgroundFirstColor;
                            }
                        }
                    }
                }
                else
                {
                    for (int i = contentRange.Column; i <= contentRange.LastColumn; i++)
                    {
                        if (i % 2 == 0)
                        {
                            IRange entireFirstRange = sheet.Range[contentRange.Row, i, contentRange.LastRow, i];
                            if (!this.mediumStyleBorder.Contains(this.builtInStyle))
                            {
                                this.ApplyBorder(sheet, entireFirstRange, solidLineStyle, contentBorderStyle,
                                                 borderColor, entireRow);
                            }

                            if (backgroundFirstColor != Color.Empty)
                            {
                                entireFirstRange.CellStyle.Color = backgroundFirstColor;
                            }
                        }
                        else
                        {
                            IRange entireSecondRange = sheet.Range[contentRange.Row, i, contentRange.LastRow, i];
                            if (!this.mediumStyleBorder.Contains(this.builtInStyle))
                            {
                                this.ApplyBorder(sheet, entireSecondRange, solidLineStyle, contentBorderStyle,
                                                 borderColor, entireRow);
                            }

                            if (entireSecondRange.Row % 2 == 0)
                            {
                                for (int index = entireSecondRange.Row; index <= entireSecondRange.LastRow; index++)
                                {
                                    IRange stripedRC = sheet.Range[index, entireSecondRange.Column, index,
                                                                   entireSecondRange.LastColumn];
                                    if (index % 2 == 0)
                                    {
                                        if (backgroundFirstColor != Color.Empty)
                                        {
                                            stripedRC.CellStyle.Color = backgroundFirstColor;
                                        }
                                    }
                                    else
                                    {
                                        if (backgroundSecondColor != Color.Empty)
                                        {
                                            stripedRC.CellStyle.Color = backgroundSecondColor;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                for (int index = entireSecondRange.Row; index <= entireSecondRange.LastRow; index++)
                                {
                                    IRange stripedRC = sheet.Range[index, entireSecondRange.Column, index,
                                                                   entireSecondRange.LastColumn];
                                    if (index % 2 == 0)
                                    {
                                        if (backgroundSecondColor != Color.Empty)
                                        {
                                            stripedRC.CellStyle.Color = backgroundSecondColor;
                                        }
                                    }
                                    else
                                    {
                                        if (backgroundFirstColor != Color.Empty)
                                        {
                                            stripedRC.CellStyle.Color = backgroundFirstColor;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if (this.rowStripes)
            {
                if (odd)
                {
                    for (int i = contentRange.Row; i <= contentRange.LastRow; i++)
                    {
                        if (i % 2 == 0)
                        {
                            IRange entireSecondRange = sheet.Range[i, contentRange.Column, i,
                                                                   contentRange.LastColumn];
                            this.ApplyBorder(sheet, entireSecondRange, solidLineStyle, contentBorderStyle,
                                             borderColor, entireRow);
                            if (backgroundSecondColor != Color.Empty)
                            {
                                entireSecondRange.CellStyle.Color = backgroundSecondColor;
                            }
                        }
                        else
                        {
                            IRange entireFirstRange = sheet.Range[i, contentRange.Column, i,
                                                                  contentRange.LastColumn];
                            this.ApplyBorder(sheet, entireFirstRange, solidLineStyle, contentBorderStyle,
                                             borderColor, entireRow);
                            if (backgroundFirstColor != Color.Empty)
                            {
                                entireFirstRange.CellStyle.Color = backgroundFirstColor;
                            }
                        }
                    }
                }
                else
                {
                    for (int i = contentRange.Row; i <= contentRange.LastRow; i++)
                    {
                        if (i % 2 == 0)
                        {
                            IRange entireFirstRange = sheet.Range[i, contentRange.Column, i,
                                                                  contentRange.LastColumn];
                            this.ApplyBorder(sheet, entireFirstRange, solidLineStyle, contentBorderStyle,
                                             borderColor, entireRow);
                            if (backgroundFirstColor != Color.Empty)
                            {
                                entireFirstRange.CellStyle.Color = backgroundFirstColor;
                            }
                        }
                        else
                        {
                            IRange entireSecondRange = sheet.Range[i, contentRange.Column, i,
                                                                   contentRange.LastColumn];
                            this.ApplyBorder(sheet, entireSecondRange, solidLineStyle, contentBorderStyle,
                                             borderColor, entireRow);
                            if (backgroundSecondColor != Color.Empty)
                            {
                                entireSecondRange.CellStyle.Color = backgroundSecondColor;
                            }
                        }
                    }
                }
            }
            else if (this.columnStripes)
            {
                if (this.lightStyleBorder.Contains(this.builtInStyle))
                {
                    contentBorderStyle = BorderStyle.LeftRight;
                }

                if (this.mediumStyleBorder.Contains(this.builtInStyle))
                {
                    if (odd)
                    {

                        for (int i = contentRange.Row; i <= contentRange.LastRow; i++)
                        {
                            if (i % 2 == 0)
                            {
                                IRange entireSecondRange = sheet.Range[i, contentRange.Column, i,
                                                                       contentRange.LastColumn];
                                this.ApplyBorder(sheet, entireSecondRange, solidLineStyle, contentBorderStyle,
                                                 borderColor, entireRow);
                            }
                            else
                            {
                                IRange entireFirstRange = sheet.Range[i, contentRange.Column, i,
                                                                      contentRange.LastColumn];
                                this.ApplyBorder(sheet, entireFirstRange, solidLineStyle, contentBorderStyle,
                                                 borderColor, entireRow);
                            }
                        }
                    }
                    else
                    {
                        for (int i = contentRange.Row; i <= contentRange.LastRow; i++)
                        {
                            if (i % 2 == 0)
                            {
                                IRange entireFirstRange = sheet.Range[i, contentRange.Column, i,
                                                                      contentRange.LastColumn];
                                this.ApplyBorder(sheet, entireFirstRange, solidLineStyle, contentBorderStyle,
                                                 borderColor, entireRow);
                            }
                            else
                            {
                                IRange entireSecondRange = sheet.Range[i, contentRange.Column, i,
                                                                       contentRange.LastColumn];
                                this.ApplyBorder(sheet, entireSecondRange, solidLineStyle, contentBorderStyle,
                                                 borderColor, entireRow);
                            }
                        }
                    }
                }

                if (this.firstColumn)
                {
                    odd = (contentRange.Column - 1) % 2 == 0
                                                ? false
                                                : true;
                }
                else
                {
                    odd = contentRange.Column % 2 == 0
                                            ? false
                                            : true;
                }

                if (odd)
                {
                    for (int i = contentRange.Column; i <= contentRange.LastColumn; i++)
                    {
                        if (i % 2 == 0)
                        {
                            IRange entireSecondRange = sheet.Range[contentRange.Row, i, contentRange.LastRow, i];
                            if (!this.mediumStyleBorder.Contains(this.builtInStyle))
                            {
                                this.ApplyBorder(sheet, entireSecondRange, solidLineStyle, contentBorderStyle,
                                                 borderColor, entireRow);
                            }

                            if (backgroundSecondColor != Color.Empty)
                            {
                                entireSecondRange.CellStyle.Color = backgroundSecondColor;
                            }
                        }
                        else
                        {
                            IRange entireFirstRange = sheet.Range[contentRange.Row, i, contentRange.LastRow, i];
                            if (!this.mediumStyleBorder.Contains(this.builtInStyle))
                            {
                                this.ApplyBorder(sheet, entireFirstRange, solidLineStyle, contentBorderStyle,
                                                 borderColor, entireRow);
                            }

                            if (backgroundFirstColor != Color.Empty)
                            {
                                entireFirstRange.CellStyle.Color = backgroundFirstColor;
                            }
                        }
                    }
                }
                else
                {
                    for (int i = contentRange.Column; i <= contentRange.LastColumn; i++)
                    {
                        if (i % 2 == 0)
                        {
                            IRange entireFirstRange = sheet.Range[contentRange.Row, i, contentRange.LastRow, i];
                            if (!this.mediumStyleBorder.Contains(this.builtInStyle))
                            {
                                this.ApplyBorder(sheet, entireFirstRange, solidLineStyle, contentBorderStyle,
                                                 borderColor, entireRow);
                            }

                            if (backgroundFirstColor != Color.Empty)
                            {
                                entireFirstRange.CellStyle.Color = backgroundFirstColor;
                            }
                        }
                        else
                        {
                            IRange entireSecondRange = sheet.Range[contentRange.Row, i, contentRange.LastRow, i];
                            if (!this.mediumStyleBorder.Contains(this.builtInStyle))
                            {
                                this.ApplyBorder(sheet, entireSecondRange, solidLineStyle, contentBorderStyle,
                                                 borderColor, entireRow);
                            }

                            if (backgroundSecondColor != Color.Empty)
                            {
                                entireSecondRange.CellStyle.Color = backgroundSecondColor;
                            }
                        }
                    }
                }
            }

            if (!showTotals)
            {
                if (this.borderList.ContainsKey(this.builtInStyle) ||
                    this.doubleBorderList.ContainsKey(this.builtInStyle))
                {
                    Color borderTotalColor;
                    if (this.borderList.TryGetValue(this.builtInStyle, out borderTotalColor))
                    {
                        this.ApplyBorder(sheet, sheet.Range[headerRange.LastRow, headerRange.Column,
                                         headerRange.LastRow, headerRange.LastColumn], SolidStyle.None,
                                         BorderStyle.Bottom, borderTotalColor, true);
                    }
                    else if (this.doubleBorderList.TryGetValue(this.builtInStyle, out borderTotalColor))
                    {
                        this.ApplyBorder(sheet, sheet.Range[headerRange.LastRow, headerRange.Column,
                                         headerRange.LastRow, headerRange.LastColumn], SolidStyle.None,
                                         BorderStyle.Bottom, borderTotalColor, true);
                    }
                }
            }
        }

        /// <summary>
        /// Applies the border.
        /// </summary>
        /// <param name="sheet">The Worksheet.</param>
        /// <param name="borderRange">The border range.</param>
        /// <param name="solidStyle">The solid style.</param>
        /// <param name="borderStyle">The border style.</param>
        /// <param name="borderColor">Color of the border.</param>
        /// <param name="entireRow">if set to <c>true</c> [entire row].</param>
        private void ApplyBorder(IWorksheet sheet, IRange borderRange, SolidStyle solidStyle,
                                 BorderStyle borderStyle, Color borderColor, bool entireRow)
        {
            Dictionary<ExcelBordersIndex, Color> borderCollections = new Dictionary<ExcelBordersIndex, Color>();
            if (this.borderColorList.Count == 0)
            {
                this.borderColorList.Add(borderRange, borderCollections);
            }

            if (entireRow)
            {
                if (borderStyle == BorderStyle.All)
                {
                    borderRange.CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = solidStyle == SolidStyle.Top
                                                                                        || solidStyle == SolidStyle.TopBottom
                                                                                               ? ExcelLineStyle.Medium
                                                                                               : ExcelLineStyle.Thin;
                    if (this.borderColorList.ContainsKey(borderRange))
                    {
                        Dictionary<ExcelBordersIndex, Color> tempBorderCollections;
                        this.borderColorList.TryGetValue(borderRange, out tempBorderCollections);
                        tempBorderCollections.Add(ExcelBordersIndex.EdgeTop, borderColor);
                    }
                    else
                    {
                        borderCollections = new Dictionary<ExcelBordersIndex, Color>();
                        borderCollections.Add(ExcelBordersIndex.EdgeTop, borderColor);
                        this.borderColorList.Add(borderRange, borderCollections);
                    }

                    borderRange.CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = solidStyle == SolidStyle.Bottom
                                                                                            || solidStyle == SolidStyle.TopBottom
                                                                                                        ? ExcelLineStyle.Medium
                                                                                                        : ExcelLineStyle.Thin;
                    if (this.borderColorList.ContainsKey(borderRange))
                    {
                        Dictionary<ExcelBordersIndex, Color> tempBorderCollections;
                        this.borderColorList.TryGetValue(borderRange, out tempBorderCollections);
                        tempBorderCollections.Add(ExcelBordersIndex.EdgeBottom, borderColor);
                    }
                    else
                    {
                        borderCollections = new Dictionary<ExcelBordersIndex, Color>();
                        borderCollections.Add(ExcelBordersIndex.EdgeBottom, borderColor);
                        this.borderColorList.Add(borderRange, borderCollections);
                    }

                    sheet.Range[borderRange.Row, borderRange.Column].CellStyle.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = solidStyle == SolidStyle.Left
                                                                                                                               || solidStyle == SolidStyle.LeftRight
                                                                                                                                              ? ExcelLineStyle.Medium
                                                                                                                                              : ExcelLineStyle.Thin;
                    if (this.borderColorList.ContainsKey(sheet.Range[borderRange.Row, borderRange.Column]))
                    {
                        Dictionary<ExcelBordersIndex, Color> tempBorderCollections;
                        this.borderColorList.TryGetValue(sheet.Range[borderRange.Row, borderRange.Column],
                                                         out tempBorderCollections);
                        tempBorderCollections.Add(ExcelBordersIndex.EdgeLeft, borderColor);
                    }
                    else
                    {
                        borderCollections = new Dictionary<ExcelBordersIndex, Color>();
                        borderCollections.Add(ExcelBordersIndex.EdgeLeft, borderColor);
                        this.borderColorList.Add(sheet.Range[borderRange.Row, borderRange.Column],
                                                 borderCollections);
                    }

                    sheet.Range[borderRange.LastRow, borderRange.LastColumn].CellStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = solidStyle == SolidStyle.Right
                                                                                                                                        || solidStyle == SolidStyle.LeftRight
                                                                                                                                        ? ExcelLineStyle.Medium
                                                                                                                                        : ExcelLineStyle.Thin;
                    if (this.borderColorList.ContainsKey(sheet.Range[borderRange.LastRow, borderRange.LastColumn]))
                    {
                        Dictionary<ExcelBordersIndex, Color> tempBorderCollections;
                        this.borderColorList.TryGetValue(sheet.Range[borderRange.LastRow, borderRange.LastColumn],
                                                         out tempBorderCollections);
                        tempBorderCollections.Add(ExcelBordersIndex.EdgeRight, borderColor);
                    }
                    else
                    {
                        borderCollections = new Dictionary<ExcelBordersIndex, Color>();
                        borderCollections.Add(ExcelBordersIndex.EdgeRight, borderColor);
                        this.borderColorList.Add(sheet.Range[borderRange.LastRow, borderRange.LastColumn],
                                                 borderCollections);
                    }
                }
                else if (borderStyle == BorderStyle.TopBottom)
                {
                    borderRange.CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = solidStyle == SolidStyle.Top
                                                                                         || solidStyle == SolidStyle.TopBottom
                                                                                                        ? ExcelLineStyle.Medium
                                                                                                        : ExcelLineStyle.Thin;
                    if (this.borderColorList.ContainsKey(borderRange))
                    {
                        Dictionary<ExcelBordersIndex, Color> tempBorderCollections;
                        this.borderColorList.TryGetValue(borderRange, out tempBorderCollections);
                        tempBorderCollections.Add(ExcelBordersIndex.EdgeTop, borderColor);
                    }
                    else
                    {
                        borderCollections = new Dictionary<ExcelBordersIndex, Color>();
                        borderCollections.Add(ExcelBordersIndex.EdgeTop, borderColor);
                        this.borderColorList.Add(borderRange, borderCollections);
                    }

                    borderRange.CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = solidStyle == SolidStyle.Bottom
                                                                                            || solidStyle == SolidStyle.TopBottom
                                                                                                           ? ExcelLineStyle.Medium
                                                                                                           : ExcelLineStyle.Thin;
                    if (this.borderColorList.ContainsKey(borderRange))
                    {
                        Dictionary<ExcelBordersIndex, Color> tempBorderCollections;
                        this.borderColorList.TryGetValue(borderRange, out tempBorderCollections);
                        tempBorderCollections.Add(ExcelBordersIndex.EdgeBottom, borderColor);
                    }
                    else
                    {
                        borderCollections = new Dictionary<ExcelBordersIndex, Color>();
                        borderCollections.Add(ExcelBordersIndex.EdgeBottom, borderColor);
                        this.borderColorList.Add(borderRange, borderCollections);
                    }
                }
                else if (borderStyle == BorderStyle.LeftRight)
                {
                    sheet.Range[borderRange.Row, borderRange.Column, borderRange.LastRow, borderRange.LastColumn].CellStyle.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = solidStyle == SolidStyle.Left
                                                                                                                                                                            || solidStyle == SolidStyle.LeftRight
                                                                                                                                                                                           ? ExcelLineStyle.Medium
                                                                                                                                                                                           : ExcelLineStyle.Thin;
                    if (this.borderColorList.ContainsKey(sheet.Range[borderRange.Row, borderRange.Column]))
                    {
                        Dictionary<ExcelBordersIndex, Color> tempBorderCollections;
                        this.borderColorList.TryGetValue(sheet.Range[borderRange.Row, borderRange.Column,
                                                         borderRange.LastRow, borderRange.LastColumn],
                                                         out tempBorderCollections);
                        tempBorderCollections.Add(ExcelBordersIndex.EdgeLeft, borderColor);
                    }
                    else
                    {
                        borderCollections = new Dictionary<ExcelBordersIndex, Color>();
                        borderCollections.Add(ExcelBordersIndex.EdgeLeft, borderColor);
                        this.borderColorList.Add(sheet.Range[borderRange.Row, borderRange.Column,
                                                 borderRange.LastRow, borderRange.LastColumn], borderCollections);
                    }

                    sheet.Range[borderRange.Row, borderRange.Column, borderRange.LastRow, borderRange.LastColumn].CellStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = solidStyle == SolidStyle.Right
                                                                                                                                                                             || solidStyle == SolidStyle.LeftRight
                                                                                                                                                                                           ? ExcelLineStyle.Medium
                                                                                                                                                                                           : ExcelLineStyle.Thin;
                    if (this.borderColorList.ContainsKey(sheet.Range[borderRange.LastRow, borderRange.LastColumn]))
                    {
                        Dictionary<ExcelBordersIndex, Color> tempBorderCollections;
                        this.borderColorList.TryGetValue(sheet.Range[borderRange.Row, borderRange.Column,
                                                         borderRange.LastRow, borderRange.LastColumn],
                                                         out tempBorderCollections);
                        tempBorderCollections.Add(ExcelBordersIndex.EdgeRight, borderColor);
                    }
                    else
                    {
                        borderCollections = new Dictionary<ExcelBordersIndex, Color>();
                        borderCollections.Add(ExcelBordersIndex.EdgeRight, borderColor);
                        this.borderColorList.Add(sheet.Range[borderRange.Row, borderRange.Column,
                                                 borderRange.LastRow, borderRange.LastColumn], borderCollections);
                    }
                }
                else if (borderStyle == BorderStyle.Top)
                {
                    borderRange.CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = solidStyle == SolidStyle.Top
                                                                                         || solidStyle == SolidStyle.TopBottom
                                                                                                        ? ExcelLineStyle.Medium
                                                                                                        : ExcelLineStyle.Thin;
                    if (this.borderColorList.ContainsKey(borderRange))
                    {
                        Dictionary<ExcelBordersIndex, Color> tempBorderCollections;
                        this.borderColorList.TryGetValue(borderRange, out tempBorderCollections);
                        tempBorderCollections.Add(ExcelBordersIndex.EdgeTop, borderColor);
                    }
                    else
                    {
                        borderCollections = new Dictionary<ExcelBordersIndex, Color>();
                        borderCollections.Add(ExcelBordersIndex.EdgeTop, borderColor);
                        this.borderColorList.Add(borderRange, borderCollections);
                    }
                }
                else if (borderStyle == BorderStyle.Bottom)
                {
                    borderRange.CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = solidStyle == SolidStyle.Bottom
                                                                                            || solidStyle == SolidStyle.TopBottom
                                                                                                           ? ExcelLineStyle.Medium
                                                                                                           : ExcelLineStyle.Thin;
                    if (this.borderColorList.ContainsKey(borderRange))
                    {
                        Dictionary<ExcelBordersIndex, Color> tempBorderCollections;
                        this.borderColorList.TryGetValue(borderRange, out tempBorderCollections);
                        tempBorderCollections.Add(ExcelBordersIndex.EdgeBottom, borderColor);
                    }
                    else
                    {
                        borderCollections = new Dictionary<ExcelBordersIndex, Color>();
                        borderCollections.Add(ExcelBordersIndex.EdgeBottom, borderColor);
                        this.borderColorList.Add(borderRange, borderCollections);
                    }
                }
                else if (borderStyle == BorderStyle.Left)
                {
                    sheet.Range[borderRange.Row, borderRange.Column, borderRange.LastRow, borderRange.LastColumn].CellStyle.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = solidStyle == SolidStyle.Left
                                                                                                                                                                            || solidStyle == SolidStyle.LeftRight
                                                                                                                                                                                           ? ExcelLineStyle.Medium
                                                                                                                                                                                           : ExcelLineStyle.Thin;
                    if (this.borderColorList.ContainsKey(sheet.Range[borderRange.Row, borderRange.Column]))
                    {
                        Dictionary<ExcelBordersIndex, Color> tempBorderCollections;
                        this.borderColorList.TryGetValue(sheet.Range[borderRange.Row, borderRange.Column,
                                                         borderRange.LastRow, borderRange.LastColumn],
                                                         out tempBorderCollections);
                        tempBorderCollections.Add(ExcelBordersIndex.EdgeLeft, borderColor);
                    }
                    else
                    {
                        borderCollections = new Dictionary<ExcelBordersIndex, Color>();
                        borderCollections.Add(ExcelBordersIndex.EdgeLeft, borderColor);
                        this.borderColorList.Add(sheet.Range[borderRange.Row, borderRange.Column,
                                                 borderRange.LastRow, borderRange.LastColumn], borderCollections);
                    }
                }
                else if (borderStyle == BorderStyle.Right)
                {
                    sheet.Range[borderRange.Row, borderRange.Column, borderRange.LastRow, borderRange.LastColumn].CellStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = solidStyle == SolidStyle.Right
                                                                                                                                                                             || solidStyle == SolidStyle.LeftRight
                                                                                                                                                                                            ? ExcelLineStyle.Medium
                                                                                                                                                                                            : ExcelLineStyle.Thin;
                    if (this.borderColorList.ContainsKey(sheet.Range[borderRange.LastRow, borderRange.LastColumn]))
                    {
                        Dictionary<ExcelBordersIndex, Color> tempBorderCollections;
                        this.borderColorList.TryGetValue(sheet.Range[borderRange.Row, borderRange.Column,
                                                         borderRange.LastRow, borderRange.LastColumn],
                                                         out tempBorderCollections);
                        tempBorderCollections.Add(ExcelBordersIndex.EdgeRight, borderColor);
                    }
                    else
                    {
                        borderCollections = new Dictionary<ExcelBordersIndex, Color>();
                        borderCollections.Add(ExcelBordersIndex.EdgeRight, borderColor);
                        this.borderColorList.Add(sheet.Range[borderRange.Row, borderRange.Column,
                                                 borderRange.LastRow, borderRange.LastColumn], borderCollections);
                    }
                }
            }
            else
            {
                if (borderStyle == BorderStyle.All && borderRange.CellStyle.Borders.Count!=0)
                {
                    borderRange.CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = solidStyle == SolidStyle.Top
                                                                                         || solidStyle == SolidStyle.TopBottom
                                                                                                        ? ExcelLineStyle.Medium
                                                                                                        : ExcelLineStyle.Thin;
                    if (this.borderColorList.ContainsKey(borderRange))
                    {
                        Dictionary<ExcelBordersIndex, Color> tempBorderCollections;
                        this.borderColorList.TryGetValue(borderRange, out tempBorderCollections);
                        tempBorderCollections.Add(ExcelBordersIndex.EdgeTop, borderColor);
                    }
                    else
                    {
                        borderCollections = new Dictionary<ExcelBordersIndex, Color>();
                        borderCollections.Add(ExcelBordersIndex.EdgeTop, borderColor);
                        this.borderColorList.Add(borderRange, borderCollections);
                    }

                    borderRange.CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = solidStyle == SolidStyle.Bottom
                                                                                            || solidStyle == SolidStyle.TopBottom
                                                                                                           ? ExcelLineStyle.Medium
                                                                                                           : ExcelLineStyle.Thin;
                    if (this.borderColorList.ContainsKey(borderRange))
                    {
                        Dictionary<ExcelBordersIndex, Color> tempBorderCollections;
                        this.borderColorList.TryGetValue(borderRange, out tempBorderCollections);
                        tempBorderCollections.Add(ExcelBordersIndex.EdgeBottom, borderColor);
                    }
                    else
                    {
                        borderCollections = new Dictionary<ExcelBordersIndex, Color>();
                        borderCollections.Add(ExcelBordersIndex.EdgeBottom, borderColor);
                        this.borderColorList.Add(borderRange, borderCollections);
                    }

                    borderRange.CellStyle.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = solidStyle == SolidStyle.Left
                                                                                          || solidStyle == SolidStyle.LeftRight
                                                                                                         ? ExcelLineStyle.Medium
                                                                                                         : ExcelLineStyle.Thin;
                    if (this.borderColorList.ContainsKey(borderRange))
                    {
                        Dictionary<ExcelBordersIndex, Color> tempBorderCollections;
                        this.borderColorList.TryGetValue(borderRange, out tempBorderCollections);
                        tempBorderCollections.Add(ExcelBordersIndex.EdgeLeft, borderColor);
                    }
                    else
                    {
                        borderCollections = new Dictionary<ExcelBordersIndex, Color>();
                        borderCollections.Add(ExcelBordersIndex.EdgeLeft, borderColor);
                        this.borderColorList.Add(borderRange, borderCollections);
                    }

                    borderRange.CellStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = solidStyle == SolidStyle.Right
                                                                                           || solidStyle == SolidStyle.LeftRight
                                                                                                          ? ExcelLineStyle.Medium
                                                                                                          : ExcelLineStyle.Thin;
                    if (this.borderColorList.ContainsKey(borderRange))
                    {
                        Dictionary<ExcelBordersIndex, Color> tempBorderCollections;
                        this.borderColorList.TryGetValue(borderRange, out tempBorderCollections);
                        tempBorderCollections.Add(ExcelBordersIndex.EdgeRight, borderColor);
                    }
                    else
                    {
                        borderCollections = new Dictionary<ExcelBordersIndex, Color>();
                        borderCollections.Add(ExcelBordersIndex.EdgeRight, borderColor);
                        this.borderColorList.Add(borderRange, borderCollections);
                    }
                }
                else if (borderStyle == BorderStyle.TopBottom)
                {
                    borderRange.CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = solidStyle == SolidStyle.Top
                                                                                         || solidStyle == SolidStyle.TopBottom
                                                                                                        ? ExcelLineStyle.Medium
                                                                                                        : ExcelLineStyle.Thin;
                    if (this.borderColorList.ContainsKey(borderRange))
                    {
                        Dictionary<ExcelBordersIndex, Color> tempBorderCollections;
                        this.borderColorList.TryGetValue(borderRange, out tempBorderCollections);
                        tempBorderCollections.Add(ExcelBordersIndex.EdgeTop, borderColor);
                    }
                    else
                    {
                        borderCollections = new Dictionary<ExcelBordersIndex, Color>();
                        borderCollections.Add(ExcelBordersIndex.EdgeTop, borderColor);
                        this.borderColorList.Add(borderRange, borderCollections);
                    }

                    borderRange.CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = solidStyle == SolidStyle.Bottom
                                                                                            || solidStyle == SolidStyle.TopBottom
                                                                                                            ? ExcelLineStyle.Medium
                                                                                                            : ExcelLineStyle.Thin;
                    if (this.borderColorList.ContainsKey(borderRange))
                    {
                        Dictionary<ExcelBordersIndex, Color> tempBorderCollections;
                        this.borderColorList.TryGetValue(borderRange, out tempBorderCollections);
                        tempBorderCollections.Add(ExcelBordersIndex.EdgeBottom, borderColor);
                    }
                    else
                    {
                        borderCollections = new Dictionary<ExcelBordersIndex, Color>();
                        borderCollections.Add(ExcelBordersIndex.EdgeBottom, borderColor);
                        this.borderColorList.Add(borderRange, borderCollections);
                    }
                }
                else if (borderStyle == BorderStyle.LeftRight)
                {
                    borderRange.CellStyle.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = solidStyle == SolidStyle.Left
                                                                                          || solidStyle == SolidStyle.LeftRight
                                                                                                         ? ExcelLineStyle.Medium
                                                                                                         : ExcelLineStyle.Thin;
                    if (this.borderColorList.ContainsKey(borderRange))
                    {
                        Dictionary<ExcelBordersIndex, Color> tempBorderCollections;
                        this.borderColorList.TryGetValue(borderRange, out tempBorderCollections);
                        tempBorderCollections.Add(ExcelBordersIndex.EdgeLeft, borderColor);
                    }
                    else
                    {
                        borderCollections = new Dictionary<ExcelBordersIndex, Color>();
                        borderCollections.Add(ExcelBordersIndex.EdgeLeft, borderColor);
                        this.borderColorList.Add(borderRange, borderCollections);
                    }

                    borderRange.CellStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = solidStyle == SolidStyle.Right
                                                                                           || solidStyle == SolidStyle.LeftRight
                                                                                                          ? ExcelLineStyle.Medium
                                                                                                          : ExcelLineStyle.Thin;
                    if (this.borderColorList.ContainsKey(borderRange))
                    {
                        Dictionary<ExcelBordersIndex, Color> tempBorderCollections;
                        this.borderColorList.TryGetValue(borderRange, out tempBorderCollections);
                        tempBorderCollections.Add(ExcelBordersIndex.EdgeRight, borderColor);
                    }
                    else
                    {
                        borderCollections = new Dictionary<ExcelBordersIndex, Color>();
                        borderCollections.Add(ExcelBordersIndex.EdgeRight, borderColor);
                        this.borderColorList.Add(borderRange, borderCollections);
                    }
                }
                else if (borderStyle == BorderStyle.Top)
                {
                    borderRange.CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = solidStyle == SolidStyle.Top
                                                                                         || solidStyle == SolidStyle.TopBottom
                                                                                                        ? ExcelLineStyle.Medium
                                                                                                        : ExcelLineStyle.Thin;
                    if (this.borderColorList.ContainsKey(borderRange))
                    {
                        Dictionary<ExcelBordersIndex, Color> tempBorderCollections;
                        this.borderColorList.TryGetValue(borderRange, out tempBorderCollections);
                        tempBorderCollections.Add(ExcelBordersIndex.EdgeTop, borderColor);
                    }
                    else
                    {
                        borderCollections = new Dictionary<ExcelBordersIndex, Color>();
                        borderCollections.Add(ExcelBordersIndex.EdgeTop, borderColor);
                        this.borderColorList.Add(borderRange, borderCollections);
                    }
                }
                else if (borderStyle == BorderStyle.Bottom)
                {
                    borderRange.CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = solidStyle == SolidStyle.Bottom
                                                                                            || solidStyle == SolidStyle.TopBottom
                                                                                                           ? ExcelLineStyle.Medium
                                                                                                           : ExcelLineStyle.Thin;
                    if (this.borderColorList.ContainsKey(borderRange))
                    {
                        Dictionary<ExcelBordersIndex, Color> tempBorderCollections;
                        this.borderColorList.TryGetValue(borderRange, out tempBorderCollections);
                        tempBorderCollections.Add(ExcelBordersIndex.EdgeBottom, borderColor);
                    }
                    else
                    {
                        borderCollections = new Dictionary<ExcelBordersIndex, Color>();
                        borderCollections.Add(ExcelBordersIndex.EdgeBottom, borderColor);
                        this.borderColorList.Add(borderRange, borderCollections);
                    }
                }
                else if (borderStyle == BorderStyle.Left)
                {
                    borderRange.CellStyle.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = solidStyle == SolidStyle.Left
                                                                                          || solidStyle == SolidStyle.LeftRight
                                                                                                         ? ExcelLineStyle.Medium
                                                                                                         : ExcelLineStyle.Thin;
                    if (this.borderColorList.ContainsKey(borderRange))
                    {
                        Dictionary<ExcelBordersIndex, Color> tempBorderCollections;
                        this.borderColorList.TryGetValue(borderRange, out tempBorderCollections);
                        tempBorderCollections.Add(ExcelBordersIndex.EdgeLeft, borderColor);
                    }
                    else
                    {
                        borderCollections = new Dictionary<ExcelBordersIndex, Color>();
                        borderCollections.Add(ExcelBordersIndex.EdgeLeft, borderColor);
                        this.borderColorList.Add(borderRange, borderCollections);
                    }
                }
                else if (borderStyle == BorderStyle.Right)
                {
                    borderRange.CellStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = solidStyle == SolidStyle.Right
                                                                                           || solidStyle == SolidStyle.LeftRight
                                                                                                          ? ExcelLineStyle.Medium
                                                                                                          : ExcelLineStyle.Thin;
                    if (this.borderColorList.ContainsKey(borderRange))
                    {
                        Dictionary<ExcelBordersIndex, Color> tempBorderCollections;
                        this.borderColorList.TryGetValue(borderRange, out tempBorderCollections);
                        tempBorderCollections.Add(ExcelBordersIndex.EdgeRight, borderColor);
                    }
                    else
                    {
                        borderCollections = new Dictionary<ExcelBordersIndex, Color>();
                        borderCollections.Add(ExcelBordersIndex.EdgeRight, borderColor);
                        this.borderColorList.Add(borderRange, borderCollections);
                    }
                }
            }
        }

        /// <summary>
        /// Applies the border to the first and last column of the table.
        /// </summary>
        /// <param name="sheet">The Worksheet.</param>
        /// <param name="borderRange">The border range.</param>
        /// <param name="solidStyle">The solid style.</param>
        /// <param name="borderStyle">The border style.</param>
        /// <param name="borderColor">Color of the border.</param>
        /// <param name="entireRow">if set to <c>true</c> [entire row].</param>
        /// <param name="showTotal">if set to <c>true</c> [show total].</param>
        private void ApplyFirstLastColumnBorder(IWorksheet sheet, IRange borderRange, SolidStyle solidStyle,
                                                BorderStyle borderStyle, Color borderColor, bool entireRow)
        {
            Dictionary<ExcelBordersIndex, Color> borderCollections = new Dictionary<ExcelBordersIndex, Color>();
            if (this.borderColorList.Count == 0)
            {
                this.borderColorList.Add(borderRange, borderCollections);
            }

            if (entireRow)
            {
                if (borderStyle == BorderStyle.All)
                {
                    Color topBorderColor;
                    if (this.doubleBorderList.ContainsKey(this.builtInStyle))
                    {
                        sheet.Range[borderRange.Row - 1, borderRange.Column, borderRange.LastRow - 1,
                                    borderRange.LastColumn].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.None;
                        this.doubleBorderList.TryGetValue(this.builtInStyle, out topBorderColor);
                        borderRange.CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Double;
                    }
                    else if (this.topSolidList.ContainsKey(this.builtInStyle))
                    {
                        sheet.Range[borderRange.Row - 1, borderRange.Column, borderRange.LastRow - 1,
                                    borderRange.LastColumn].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.None;
                        this.topSolidList.TryGetValue(this.builtInStyle, out topBorderColor);
                        borderRange.CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Medium;
                    }
                    else
                    {
                        borderRange.CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = solidStyle == SolidStyle.Bottom
                                                                                             || solidStyle == SolidStyle.TopBottom
                                                                                                            ? ExcelLineStyle.Medium
                                                                                                            : ExcelLineStyle.Thin;
                        topBorderColor = borderColor;
                    }

                    if (this.borderColorList.ContainsKey(borderRange))
                    {
                        Dictionary<ExcelBordersIndex, Color> tempBorderCollections;
                        this.borderColorList.TryGetValue(borderRange, out tempBorderCollections);
                        tempBorderCollections.Add(ExcelBordersIndex.EdgeTop, topBorderColor);
                    }
                    else
                    {
                        borderCollections = new Dictionary<ExcelBordersIndex, Color>();
                        borderCollections.Add(ExcelBordersIndex.EdgeTop, topBorderColor);
                        this.borderColorList.Add(borderRange, borderCollections);
                    }

                    borderRange.CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = solidStyle == SolidStyle.Bottom
                                                                                            || solidStyle == SolidStyle.TopBottom
                                                                                                          ? ExcelLineStyle.Medium
                                                                                                          : ExcelLineStyle.Thin;
                    if (this.borderColorList.ContainsKey(borderRange))
                    {
                        Dictionary<ExcelBordersIndex, Color> tempBorderCollections;
                        this.borderColorList.TryGetValue(borderRange, out tempBorderCollections);
                        tempBorderCollections.Add(ExcelBordersIndex.EdgeBottom, borderColor);
                    }
                    else
                    {
                        borderCollections = new Dictionary<ExcelBordersIndex, Color>();
                        borderCollections.Add(ExcelBordersIndex.EdgeBottom, borderColor);
                        this.borderColorList.Add(borderRange, borderCollections);
                    }

                    sheet.Range[borderRange.Row, borderRange.Column].CellStyle.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = solidStyle == SolidStyle.Left
                                                                                                                               || solidStyle == SolidStyle.LeftRight
                                                                                                                                              ? ExcelLineStyle.Medium
                                                                                                                                              : ExcelLineStyle.Thin;
                    if (this.borderColorList.ContainsKey(sheet.Range[borderRange.Row, borderRange.Column]))
                    {
                        Dictionary<ExcelBordersIndex, Color> tempBorderCollections;
                        this.borderColorList.TryGetValue(sheet.Range[borderRange.Row, borderRange.Column], out tempBorderCollections);
                        tempBorderCollections.Add(ExcelBordersIndex.EdgeLeft, borderColor);
                    }
                    else
                    {
                        borderCollections = new Dictionary<ExcelBordersIndex, Color>();
                        borderCollections.Add(ExcelBordersIndex.EdgeLeft, borderColor);
                        this.borderColorList.Add(sheet.Range[borderRange.Row, borderRange.Column], borderCollections);
                    }

                    sheet.Range[borderRange.LastRow, borderRange.LastColumn].CellStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = solidStyle == SolidStyle.Right
                                                                                                                                        || solidStyle == SolidStyle.LeftRight
                                                                                                                                                       ? ExcelLineStyle.Medium
                                                                                                                                                       : ExcelLineStyle.Thin;
                    if (this.borderColorList.ContainsKey(sheet.Range[borderRange.LastRow, borderRange.LastColumn]))
                    {
                        Dictionary<ExcelBordersIndex, Color> tempBorderCollections;
                        this.borderColorList.TryGetValue(sheet.Range[borderRange.LastRow, borderRange.LastColumn], out tempBorderCollections);
                        tempBorderCollections.Add(ExcelBordersIndex.EdgeRight, borderColor);
                    }
                    else
                    {
                        borderCollections = new Dictionary<ExcelBordersIndex, Color>();
                        borderCollections.Add(ExcelBordersIndex.EdgeRight, borderColor);
                        this.borderColorList.Add(sheet.Range[borderRange.LastRow, borderRange.LastColumn],
                                                 borderCollections);
                    }
                }
                else if (borderStyle == BorderStyle.TopBottom)
                {
                    Color topBorderColor;
                    if (this.doubleBorderList.ContainsKey(this.builtInStyle))
                    {
                        sheet.Range[borderRange.Row - 1, borderRange.Column, borderRange.LastRow - 1,
                                    borderRange.LastColumn].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.None;
                        this.doubleBorderList.TryGetValue(this.builtInStyle, out topBorderColor);
                        borderRange.CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Double;
                    }
                    else if (this.topSolidList.ContainsKey(this.builtInStyle))
                    {
                        sheet.Range[borderRange.Row - 1, borderRange.Column, borderRange.LastRow - 1,
                                    borderRange.LastColumn].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.None;
                        this.topSolidList.TryGetValue(this.builtInStyle, out topBorderColor);
                        borderRange.CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Medium;
                    }
                    else
                    {
                        borderRange.CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = solidStyle == SolidStyle.Bottom
                                                                                             || solidStyle == SolidStyle.TopBottom
                                                                                                            ? ExcelLineStyle.Medium
                                                                                                            : ExcelLineStyle.Thin;
                        topBorderColor = borderColor;
                    }

                    if (this.borderColorList.ContainsKey(borderRange))
                    {
                        Dictionary<ExcelBordersIndex, Color> tempBorderCollections;
                        this.borderColorList.TryGetValue(borderRange, out tempBorderCollections);
                        tempBorderCollections.Add(ExcelBordersIndex.EdgeTop, topBorderColor);
                    }
                    else
                    {
                        borderCollections = new Dictionary<ExcelBordersIndex, Color>();
                        borderCollections.Add(ExcelBordersIndex.EdgeTop, topBorderColor);
                        this.borderColorList.Add(borderRange, borderCollections);
                    }

                    borderRange.CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = solidStyle == SolidStyle.Bottom
                                                                                            || solidStyle == SolidStyle.TopBottom
                                                                                                           ? ExcelLineStyle.Medium
                                                                                                           : ExcelLineStyle.Thin;
                    if (this.borderColorList.ContainsKey(borderRange))
                    {
                        Dictionary<ExcelBordersIndex, Color> tempBorderCollections;
                        this.borderColorList.TryGetValue(borderRange, out tempBorderCollections);
                        tempBorderCollections.Add(ExcelBordersIndex.EdgeBottom, borderColor);
                    }
                    else
                    {
                        borderCollections = new Dictionary<ExcelBordersIndex, Color>();
                        borderCollections.Add(ExcelBordersIndex.EdgeBottom, borderColor);
                        this.borderColorList.Add(borderRange, borderCollections);
                    }
                }
                else if (borderStyle == BorderStyle.LeftRight)
                {
                    sheet.Range[borderRange.Row, borderRange.Column].CellStyle.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = solidStyle == SolidStyle.Left
                                                                                                                               || solidStyle == SolidStyle.LeftRight
                                                                                                                                              ? ExcelLineStyle.Medium
                                                                                                                                              : ExcelLineStyle.Thin;
                    if (this.borderColorList.ContainsKey(sheet.Range[borderRange.Row, borderRange.Column]))
                    {
                        Dictionary<ExcelBordersIndex, Color> tempBorderCollections;
                        this.borderColorList.TryGetValue(sheet.Range[borderRange.Row, borderRange.Column],
                                                         out tempBorderCollections);
                        tempBorderCollections.Add(ExcelBordersIndex.EdgeLeft, borderColor);
                    }
                    else
                    {
                        borderCollections = new Dictionary<ExcelBordersIndex, Color>();
                        borderCollections.Add(ExcelBordersIndex.EdgeTop, borderColor);
                        this.borderColorList.Add(sheet.Range[borderRange.Row, borderRange.Column],
                                                 borderCollections);
                    }

                    sheet.Range[borderRange.LastRow, borderRange.LastColumn].CellStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = solidStyle == SolidStyle.Right
                                                                                                                                        || solidStyle == SolidStyle.LeftRight
                                                                                                                                                       ? ExcelLineStyle.Medium
                                                                                                                                                       : ExcelLineStyle.Thin;
                    if (this.borderColorList.ContainsKey(sheet.Range[borderRange.LastRow, borderRange.LastColumn]))
                    {
                        Dictionary<ExcelBordersIndex, Color> tempBorderCollections;
                        this.borderColorList.TryGetValue(sheet.Range[borderRange.LastRow, borderRange.LastColumn], out tempBorderCollections);
                        tempBorderCollections.Add(ExcelBordersIndex.EdgeRight, borderColor);
                    }
                    else
                    {
                        borderCollections = new Dictionary<ExcelBordersIndex, Color>();
                        borderCollections.Add(ExcelBordersIndex.EdgeRight, borderColor);
                        this.borderColorList.Add(sheet.Range[borderRange.LastRow, borderRange.LastColumn],
                                                 borderCollections);
                    }
                }
                else if (borderStyle == BorderStyle.Top)
                {
                    Color topBorderColor;
                    if (this.doubleBorderList.ContainsKey(this.builtInStyle))
                    {
                        sheet.Range[borderRange.Row - 1, borderRange.Column, borderRange.LastRow - 1,
                                    borderRange.LastColumn].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.None;

                        this.doubleBorderList.TryGetValue(this.builtInStyle, out topBorderColor);
                        borderRange.CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Double;
                    }
                    else if (this.topSolidList.ContainsKey(this.builtInStyle))
                    {
                        sheet.Range[borderRange.Row - 1, borderRange.Column, borderRange.LastRow - 1,
                                    borderRange.LastColumn].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.None;

                        this.topSolidList.TryGetValue(this.builtInStyle, out topBorderColor);
                        borderRange.CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Medium;
                    }
                    else
                    {
                        borderRange.CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = solidStyle == SolidStyle.Bottom
                                                                                             || solidStyle == SolidStyle.TopBottom
                                                                                                            ? ExcelLineStyle.Medium
                                                                                                            : ExcelLineStyle.Thin;
                        topBorderColor = borderColor;
                    }

                    if (this.borderColorList.ContainsKey(borderRange))
                    {
                        Dictionary<ExcelBordersIndex, Color> tempBorderCollections;
                        this.borderColorList.TryGetValue(borderRange, out tempBorderCollections);
                        tempBorderCollections.Add(ExcelBordersIndex.EdgeTop, topBorderColor);
                    }
                    else
                    {
                        borderCollections = new Dictionary<ExcelBordersIndex, Color>();
                        borderCollections.Add(ExcelBordersIndex.EdgeTop, topBorderColor);
                        this.borderColorList.Add(borderRange, borderCollections);
                    }
                }
                else if (borderStyle == BorderStyle.Bottom)
                {
                    borderRange.CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = solidStyle == SolidStyle.Bottom
                                                                                            || solidStyle == SolidStyle.TopBottom
                                                                                                           ? ExcelLineStyle.Medium
                                                                                                           : ExcelLineStyle.Thin;
                    if (this.borderColorList.ContainsKey(borderRange))
                    {
                        Dictionary<ExcelBordersIndex, Color> tempBorderCollections;
                        this.borderColorList.TryGetValue(borderRange, out tempBorderCollections);
                        tempBorderCollections.Add(ExcelBordersIndex.EdgeBottom, borderColor);
                    }
                    else
                    {
                        borderCollections = new Dictionary<ExcelBordersIndex, Color>();
                        borderCollections.Add(ExcelBordersIndex.EdgeBottom, borderColor);
                        this.borderColorList.Add(borderRange, borderCollections);
                    }
                }
                else if (borderStyle == BorderStyle.Left)
                {
                    sheet.Range[borderRange.Row, borderRange.Column].CellStyle.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = solidStyle == SolidStyle.Left
                                                                                                                               || solidStyle == SolidStyle.LeftRight
                                                                                                                                              ? ExcelLineStyle.Medium
                                                                                                                                              : ExcelLineStyle.Thin;
                    if (this.borderColorList.ContainsKey(sheet.Range[borderRange.Row, borderRange.Column]))
                    {
                        Dictionary<ExcelBordersIndex, Color> tempBorderCollections;
                        this.borderColorList.TryGetValue(sheet.Range[borderRange.Row, borderRange.Column],
                                                         out tempBorderCollections);
                        tempBorderCollections.Add(ExcelBordersIndex.EdgeLeft, borderColor);
                    }
                    else
                    {
                        borderCollections = new Dictionary<ExcelBordersIndex, Color>();
                        borderCollections.Add(ExcelBordersIndex.EdgeLeft, borderColor);
                        this.borderColorList.Add(sheet.Range[borderRange.Row, borderRange.Column],
                                                 borderCollections);
                    }
                }
                else if (borderStyle == BorderStyle.Right)
                {
                    sheet.Range[borderRange.LastRow, borderRange.LastColumn].CellStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = solidStyle == SolidStyle.Right
                                                                                                                                        || solidStyle == SolidStyle.LeftRight
                                                                                                                                                       ? ExcelLineStyle.Medium
                                                                                                                                                       : ExcelLineStyle.Thin;
                    if (this.borderColorList.ContainsKey(sheet.Range[borderRange.LastRow, borderRange.LastColumn]))
                    {
                        Dictionary<ExcelBordersIndex, Color> tempBorderCollections;
                        this.borderColorList.TryGetValue(sheet.Range[borderRange.LastRow, borderRange.LastColumn],
                                                         out tempBorderCollections);
                        tempBorderCollections.Add(ExcelBordersIndex.EdgeRight, borderColor);
                    }
                    else
                    {
                        borderCollections = new Dictionary<ExcelBordersIndex, Color>();
                        borderCollections.Add(ExcelBordersIndex.EdgeRight, borderColor);
                        this.borderColorList.Add(sheet.Range[borderRange.LastRow, borderRange.LastColumn],
                                                 borderCollections);
                    }
                }
            }
            else
            {
                if (borderStyle == BorderStyle.All)
                {
                    Color topBorderColor;
                    if (this.doubleBorderList.ContainsKey(this.builtInStyle))
                    {
                        sheet.Range[borderRange.Row - 1, borderRange.Column, borderRange.LastRow - 1,
                                    borderRange.LastColumn].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.None;
                        this.doubleBorderList.TryGetValue(this.builtInStyle, out topBorderColor);
                        borderRange.CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Double;
                    }
                    else if (this.topSolidList.ContainsKey(this.builtInStyle))
                    {
                        sheet.Range[borderRange.Row - 1, borderRange.Column, borderRange.LastRow - 1,
                                    borderRange.LastColumn].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.None;
                        this.topSolidList.TryGetValue(this.builtInStyle, out topBorderColor);
                        borderRange.CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Medium;
                    }
                    else
                    {
                        borderRange.CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = solidStyle == SolidStyle.Top
                                                                                             || solidStyle == SolidStyle.TopBottom
                                                                                                            ? ExcelLineStyle.Medium : ExcelLineStyle.Thin;
                        topBorderColor = borderColor;
                    }

                    if (this.borderColorList.ContainsKey(borderRange))
                    {
                        Dictionary<ExcelBordersIndex, Color> tempBorderCollections;
                        this.borderColorList.TryGetValue(borderRange, out tempBorderCollections);
                        tempBorderCollections.Add(ExcelBordersIndex.EdgeTop, topBorderColor);
                    }
                    else
                    {
                        borderCollections = new Dictionary<ExcelBordersIndex, Color>();
                        borderCollections.Add(ExcelBordersIndex.EdgeTop, topBorderColor);
                        this.borderColorList.Add(borderRange, borderCollections);
                    }

                    borderRange.CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = solidStyle == SolidStyle.Bottom
                                                                                            || solidStyle == SolidStyle.TopBottom
                                                                                                           ? ExcelLineStyle.Medium
                                                                                                           : ExcelLineStyle.Thin;
                    if (this.borderColorList.ContainsKey(borderRange))
                    {
                        Dictionary<ExcelBordersIndex, Color> tempBorderCollections;
                        this.borderColorList.TryGetValue(borderRange, out tempBorderCollections);
                        tempBorderCollections.Add(ExcelBordersIndex.EdgeBottom, borderColor);
                    }
                    else
                    {
                        borderCollections = new Dictionary<ExcelBordersIndex, Color>();
                        borderCollections.Add(ExcelBordersIndex.EdgeBottom, borderColor);
                        this.borderColorList.Add(borderRange, borderCollections);
                    }

                    borderRange.CellStyle.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = solidStyle == SolidStyle.Left
                                                                                          || solidStyle == SolidStyle.LeftRight
                                                                                                         ? ExcelLineStyle.Medium
                                                                                                         : ExcelLineStyle.Thin;
                    if (this.borderColorList.ContainsKey(borderRange))
                    {
                        Dictionary<ExcelBordersIndex, Color> tempBorderCollections;
                        this.borderColorList.TryGetValue(borderRange, out tempBorderCollections);
                        tempBorderCollections.Add(ExcelBordersIndex.EdgeLeft, borderColor);
                    }
                    else
                    {
                        borderCollections = new Dictionary<ExcelBordersIndex, Color>();
                        borderCollections.Add(ExcelBordersIndex.EdgeLeft, borderColor);
                        this.borderColorList.Add(borderRange, borderCollections);
                    }

                    borderRange.CellStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = solidStyle == SolidStyle.Right
                                                                                           || solidStyle == SolidStyle.LeftRight
                                                                                                          ? ExcelLineStyle.Medium
                                                                                                          : ExcelLineStyle.Thin;
                    if (this.borderColorList.ContainsKey(borderRange))
                    {
                        Dictionary<ExcelBordersIndex, Color> tempBorderCollections;
                        this.borderColorList.TryGetValue(borderRange, out tempBorderCollections);
                        tempBorderCollections.Add(ExcelBordersIndex.EdgeRight, borderColor);
                    }
                    else
                    {
                        borderCollections = new Dictionary<ExcelBordersIndex, Color>();
                        borderCollections.Add(ExcelBordersIndex.EdgeRight, borderColor);
                        this.borderColorList.Add(borderRange, borderCollections);
                    }
                }
                else if (borderStyle == BorderStyle.TopBottom)
                {
                    Color topBorderColor;
                    if (this.doubleBorderList.ContainsKey(this.builtInStyle))
                    {
                        sheet.Range[borderRange.Row - 1, borderRange.Column, borderRange.LastRow - 1,
                                    borderRange.LastColumn].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.None;
                        this.doubleBorderList.TryGetValue(this.builtInStyle, out topBorderColor);
                        borderRange.CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Double;
                    }
                    else if (this.topSolidList.ContainsKey(this.builtInStyle))
                    {
                        sheet.Range[borderRange.Row - 1, borderRange.Column, borderRange.LastRow - 1,
                                    borderRange.LastColumn].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.None;
                        this.topSolidList.TryGetValue(this.builtInStyle, out topBorderColor);
                        borderRange.CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Medium;
                    }
                    else
                    {
                        borderRange.CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = solidStyle == SolidStyle.Bottom
                                                                                             || solidStyle == SolidStyle.TopBottom
                                                                                                            ? ExcelLineStyle.Medium
                                                                                                            : ExcelLineStyle.Thin;
                        topBorderColor = borderColor;
                    }

                    if (this.borderColorList.ContainsKey(borderRange))
                    {
                        Dictionary<ExcelBordersIndex, Color> tempBorderCollections;
                        this.borderColorList.TryGetValue(borderRange, out tempBorderCollections);
                        tempBorderCollections.Add(ExcelBordersIndex.EdgeTop, topBorderColor);
                    }
                    else
                    {
                        borderCollections = new Dictionary<ExcelBordersIndex, Color>();
                        borderCollections.Add(ExcelBordersIndex.EdgeTop, topBorderColor);
                        this.borderColorList.Add(borderRange, borderCollections);
                    }

                    borderRange.CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = solidStyle == SolidStyle.Bottom
                                                                                            || solidStyle == SolidStyle.TopBottom
                                                                                                           ? ExcelLineStyle.Medium
                                                                                                           : ExcelLineStyle.Thin;
                    if (this.borderColorList.ContainsKey(borderRange))
                    {
                        Dictionary<ExcelBordersIndex, Color> tempBorderCollections;
                        this.borderColorList.TryGetValue(borderRange, out tempBorderCollections);
                        tempBorderCollections.Add(ExcelBordersIndex.EdgeBottom, borderColor);
                    }
                    else
                    {
                        borderCollections = new Dictionary<ExcelBordersIndex, Color>();
                        borderCollections.Add(ExcelBordersIndex.EdgeBottom, borderColor);
                        this.borderColorList.Add(borderRange, borderCollections);
                    }
                }
                else if (borderStyle == BorderStyle.LeftRight)
                {
                    borderRange.CellStyle.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = solidStyle == SolidStyle.Left
                                                                                          || solidStyle == SolidStyle.LeftRight
                                                                                                         ? ExcelLineStyle.Medium
                                                                                                         : ExcelLineStyle.Thin;
                    if (this.borderColorList.ContainsKey(borderRange))
                    {
                        Dictionary<ExcelBordersIndex, Color> tempBorderCollections;
                        this.borderColorList.TryGetValue(borderRange, out tempBorderCollections);
                        tempBorderCollections.Add(ExcelBordersIndex.EdgeLeft, borderColor);
                    }
                    else
                    {
                        borderCollections = new Dictionary<ExcelBordersIndex, Color>();
                        borderCollections.Add(ExcelBordersIndex.EdgeLeft, borderColor);
                        this.borderColorList.Add(borderRange, borderCollections);
                    }

                    borderRange.CellStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = solidStyle == SolidStyle.Right
                                                                                           || solidStyle == SolidStyle.LeftRight
                                                                                                          ? ExcelLineStyle.Medium
                                                                                                          : ExcelLineStyle.Thin;
                    if (this.borderColorList.ContainsKey(borderRange))
                    {
                        Dictionary<ExcelBordersIndex, Color> tempBorderCollections;
                        this.borderColorList.TryGetValue(borderRange, out tempBorderCollections);
                        tempBorderCollections.Add(ExcelBordersIndex.EdgeRight, borderColor);
                    }
                    else
                    {
                        borderCollections = new Dictionary<ExcelBordersIndex, Color>();
                        borderCollections.Add(ExcelBordersIndex.EdgeRight, borderColor);
                        this.borderColorList.Add(borderRange, borderCollections);
                    }
                }
                else if (borderStyle == BorderStyle.Top)
                {
                    Color topBorderColor;
                    if (this.doubleBorderList.ContainsKey(this.builtInStyle))
                    {
                        sheet.Range[borderRange.Row - 1, borderRange.Column, borderRange.LastRow - 1,
                                    borderRange.LastColumn].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.None;
                        this.doubleBorderList.TryGetValue(this.builtInStyle, out topBorderColor);
                        borderRange.CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Double;
                    }
                    else if (this.topSolidList.ContainsKey(this.builtInStyle))
                    {
                        sheet.Range[borderRange.Row - 1, borderRange.Column, borderRange.LastRow - 1,
                                    borderRange.LastColumn].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.None;
                        this.topSolidList.TryGetValue(this.builtInStyle, out topBorderColor);
                        borderRange.CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Medium;
                    }
                    else
                    {
                        borderRange.CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = solidStyle == SolidStyle.Bottom
                                                                                             || solidStyle == SolidStyle.TopBottom
                                                                                                            ? ExcelLineStyle.Medium
                                                                                                            : ExcelLineStyle.Thin;
                        topBorderColor = borderColor;
                    }

                    if (this.borderColorList.ContainsKey(borderRange))
                    {
                        Dictionary<ExcelBordersIndex, Color> tempBorderCollections;
                        this.borderColorList.TryGetValue(borderRange, out tempBorderCollections);
                        tempBorderCollections.Add(ExcelBordersIndex.EdgeTop, topBorderColor);
                    }
                    else
                    {
                        borderCollections = new Dictionary<ExcelBordersIndex, Color>();
                        borderCollections.Add(ExcelBordersIndex.EdgeTop, topBorderColor);
                        this.borderColorList.Add(borderRange, borderCollections);
                    }
                }
                else if (borderStyle == BorderStyle.Bottom)
                {
                    borderRange.CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = solidStyle == SolidStyle.Bottom
                                                                                            || solidStyle == SolidStyle.TopBottom
                                                                                                           ? ExcelLineStyle.Medium
                                                                                                           : ExcelLineStyle.Thin;
                    if (this.borderColorList.ContainsKey(borderRange))
                    {
                        Dictionary<ExcelBordersIndex, Color> tempBorderCollections;
                        this.borderColorList.TryGetValue(borderRange, out tempBorderCollections);
                        tempBorderCollections.Add(ExcelBordersIndex.EdgeBottom, borderColor);
                    }
                    else
                    {
                        borderCollections = new Dictionary<ExcelBordersIndex, Color>();
                        borderCollections.Add(ExcelBordersIndex.EdgeBottom, borderColor);
                        this.borderColorList.Add(borderRange, borderCollections);
                    }
                }
                else if (borderStyle == BorderStyle.Left)
                {
                    borderRange.CellStyle.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = solidStyle == SolidStyle.Left
                                                                                          || solidStyle == SolidStyle.LeftRight
                                                                                                         ? ExcelLineStyle.Medium
                                                                                                         : ExcelLineStyle.Thin;
                    if (this.borderColorList.ContainsKey(borderRange))
                    {
                        Dictionary<ExcelBordersIndex, Color> tempBorderCollections;
                        this.borderColorList.TryGetValue(borderRange, out tempBorderCollections);
                        tempBorderCollections.Add(ExcelBordersIndex.EdgeLeft, borderColor);
                    }
                    else
                    {
                        borderCollections = new Dictionary<ExcelBordersIndex, Color>();
                        borderCollections.Add(ExcelBordersIndex.EdgeLeft, borderColor);
                        this.borderColorList.Add(borderRange, borderCollections);
                    }
                }
                else if (borderStyle == BorderStyle.Right)
                {
                    borderRange.CellStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = solidStyle == SolidStyle.Right
                                                                                           || solidStyle == SolidStyle.LeftRight
                                                                                                          ? ExcelLineStyle.Medium
                                                                                                          : ExcelLineStyle.Thin;
                    if (this.borderColorList.ContainsKey(borderRange))
                    {
                        Dictionary<ExcelBordersIndex, Color> tempBorderCollections;
                        this.borderColorList.TryGetValue(borderRange, out tempBorderCollections);
                        tempBorderCollections.Add(ExcelBordersIndex.EdgeRight, borderColor);
                    }
                    else
                    {
                        borderCollections = new Dictionary<ExcelBordersIndex, Color>();
                        borderCollections.Add(ExcelBordersIndex.EdgeRight, borderColor);
                        this.borderColorList.Add(borderRange, borderCollections);
                    }
                }
            }
        }

        /// <summary>
        /// Applies the first and last column style.
        /// </summary>
        /// <param name="sheet">The Worksheet.</param>
        /// <param name="contentRange">The content range.</param>
        /// <param name="fontColor">Color of the font.</param>
        /// <param name="backgroundFirstColor">First color of the bg.</param>
        /// <param name="showTotals">if set to <c>true</c> [show totals].</param>
        /// <returns>IRange Object</returns>
        private IRange ApplyFirstLastColumnStyle(IWorksheet sheet, IRange contentRange, Color fontColor,
                                                 Color backgroundFirstColor, bool showTotals)
        {
            RangesCollection columnRanges = null;
            if (this.firstColumn && this.lastColumn)
            {
                columnRanges = new RangesCollection(sheet.Application, sheet);
                columnRanges.Add(sheet.Range[contentRange.Row, contentRange.Column, contentRange.LastRow,
                                             contentRange.Column]);
                columnRanges.Add(sheet.Range[contentRange.Row, contentRange.LastColumn, contentRange.LastRow,
                                             contentRange.LastColumn]);
                if (!this.CheckStyle())
                {
                    columnRanges.CellStyle.Font.Bold = true;
                    this.firstColumn = false;
                    this.lastColumn = false;
                    return contentRange;
                }

                contentRange = sheet.Range[contentRange.Row, contentRange.Column + 1, contentRange.LastRow,
                                           contentRange.LastColumn - 1];
            }
            else if (this.firstColumn)
            {
                columnRanges = new RangesCollection(sheet.Application, sheet);
                columnRanges.Add(sheet.Range[contentRange.Row, contentRange.Column, contentRange.LastRow,
                                             contentRange.Column]);
                if (!this.CheckStyle())
                {
                    columnRanges.CellStyle.Font.Bold = true;
                    this.firstColumn = false;
                    return contentRange;
                }

                contentRange = sheet.Range[contentRange.Row, contentRange.Column + 1, contentRange.LastRow,
                                           contentRange.LastColumn];
            }
            else if (this.lastColumn)
            {
                columnRanges = new RangesCollection(sheet.Application, sheet);
                columnRanges.Add(sheet.Range[contentRange.Row, contentRange.LastColumn, contentRange.LastRow,
                                             contentRange.LastColumn]);
                if (!this.CheckStyle())
                {
                    columnRanges.CellStyle.Font.Bold = true;
                    this.lastColumn = false;
                    return contentRange;
                }

                contentRange = sheet.Range[contentRange.Row, contentRange.Column, contentRange.LastRow,
                                           contentRange.LastColumn - 1];
            }

            if (this.colorFonts.Contains(this.builtInStyle))
            {
                this.fontColorCollections.Add(columnRanges[0], fontColor);
            }
            else
            {
                columnRanges.CellStyle.Font.RGBColor = fontColor;
            }

            columnRanges.CellStyle.Font.Bold = true;
            if (this.darkStyles.Contains(this.builtInStyle))
            {
                if (this.firstColumn && this.lastColumn)
                {
                    this.ApplyBorder(sheet, columnRanges[0], SolidStyle.None, BorderStyle.Right, Color.White,
                                     true);
                    this.ApplyBorder(sheet, columnRanges[1], SolidStyle.None, BorderStyle.Left, Color.White,
                                     true);
                }
                else if (this.firstColumn)
                {
                    this.ApplyBorder(sheet, columnRanges[0], SolidStyle.None, BorderStyle.Right, Color.White,
                                     true);
                }
                else if (this.lastColumn)
                {
                    this.ApplyBorder(sheet, columnRanges[0], SolidStyle.None, BorderStyle.Left, Color.White,
                                     true);
                }

                columnRanges.CellStyle.Color = backgroundFirstColor;
            }
            else if (this.mediumStyleWithBorder.Contains(this.builtInStyle))
            {
                if (this.firstColumn && this.lastColumn)
                {
                    this.ApplyFirstLastColumnBorder(sheet, columnRanges[0], SolidStyle.None, BorderStyle.All,
                                                    Color.White, false);
                    this.ApplyFirstLastColumnBorder(sheet, columnRanges[1], SolidStyle.None, BorderStyle.All,
                                                    Color.White, false);
                }
                else
                {
                    this.ApplyFirstLastColumnBorder(sheet, columnRanges[0], SolidStyle.None, BorderStyle.All,
                                                    Color.White, false);
                }

                columnRanges.CellStyle.Font.RGBColor = Color.White;
                columnRanges.CellStyle.Color = this.headerColor;
            }
            else if (this.mediumStyleWithoutBorder.Contains(this.builtInStyle))
            {
                RangesCollection rangeWithTotal = new RangesCollection(sheet.Application, sheet);
                if (showTotals)
                {
                    if (this.firstColumn && this.lastColumn)
                    {
                        rangeWithTotal.Add(sheet.Range[columnRanges[0].Row, columnRanges[0].Column,
                                                       columnRanges[0].LastRow + 1, columnRanges[0].Column]);
                        rangeWithTotal.Add(sheet.Range[columnRanges[1].Row, columnRanges[1].Column,
                                                       columnRanges[1].LastRow + 1, columnRanges[1].Column]);
                        this.ApplyFirstLastColumnBorder(sheet, sheet.Range[rangeWithTotal[0].LastRow,
                                                        rangeWithTotal[0].LastColumn], SolidStyle.Bottom,
                                                        BorderStyle.TopBottom, Color.Black, true);
                        this.ApplyFirstLastColumnBorder(sheet, sheet.Range[rangeWithTotal[1].LastRow,
                                                        rangeWithTotal[1].LastColumn], SolidStyle.Bottom,
                                                        BorderStyle.TopBottom, Color.Black, true);
                    }
                    else
                    {
                        rangeWithTotal.Add(sheet.Range[columnRanges[0].Row, columnRanges[0].Column,
                                                       columnRanges[0].LastRow + 1, columnRanges[0].Column]);
                        this.ApplyFirstLastColumnBorder(sheet, sheet.Range[rangeWithTotal[0].LastRow,
                                                        rangeWithTotal[0].LastColumn], SolidStyle.Bottom,
                                                        BorderStyle.TopBottom, Color.Black, true);
                    }
                }
                else
                {
                    rangeWithTotal = columnRanges;
                }

                rangeWithTotal.CellStyle.Font.RGBColor = Color.White;
                rangeWithTotal.CellStyle.Color = this.headerColor;
            }

            return contentRange;
        }

        /// <summary>
        /// Checks the style.
        /// </summary> 
        /// <returns>
        /// Returns true if the built in style is within the this.lastColumn and first column list else false will be returned.
        /// </returns>
        private bool CheckStyle()
        {
            return this.darkStyles.Contains(this.builtInStyle) 
                ||this.mediumStyleWithBorder.Contains(this.builtInStyle) 
                ||this.mediumStyleWithoutBorder.Contains(this.builtInStyle);
        }

        #endregion
    }
}
