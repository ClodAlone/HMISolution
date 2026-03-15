//-------------------------------------------------------------------------------------------------
// <copyright file="SR.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Resources;
using System.Globalization;
using System.Diagnostics;

using Syncfusion.Diagnostics;

// Tip:
// To simplify rebuilding this class while working on your project
// you should add the following External Tool in Developer Studio:
// 
//     Title: Resgen SR.TXT
//     Command: C:\WINNT\system32\CMD.EXE
//     Arguments: /c resgen SR.txt & srgen @srgen.ini & echo Done.
//     Initial Directory: $(ProjectDir)
//     Please enable "X Use Output for Window."
// 
// Additionally you should add a file srgen.ini to project that 
// contains command line arguments for your project.
//
// Example: 
// SRGen.ini
//// sr.txt namespace:Samples.SRGenSample classname:SR

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    ///    SR provides localized access to string resources specific 
    ///    from the assembly manifest Syncfusion.Windows.Forms.Grid.SR.resources
    /// </summary>
    sealed class SR
    {
        // Fields
        private ResourceManager resources;
        private static SR loader = null;

        // Strings 
        internal const string BitVectorFull = "BitVectorFull";
        internal const string ColorEditorPaletteTab = "ColorEditorPaletteTab";
        internal const string ColorEditorStandardTab = "ColorEditorStandardTab";
        internal const string ColorEditorSystemTab = "ColorEditorSystemTab";
        internal const string CommandAddCoveredRanges = "CommandAddCoveredRanges";
        internal const string CommandChangeCells = "CommandChangeCells";
        internal const string CommandColumnWidth = "CommandColumnWidth";
        internal const string CommandDefaultSizeColumn = "CommandDefaultSizeColumn";
        internal const string CommandDefaultSizeRow = "CommandDefaultSizeRow";
        internal const string CommandFixedCountColumn = "CommandFixedCountColumn";
        internal const string CommandFixedCountRow = "CommandFixedCountRow";
        internal const string CommandHeaderCountColumn = "CommandHeaderCountColumn";
        internal const string CommandHeaderCountRow = "CommandHeaderCountRow";
        internal const string CommandHideColumn = "CommandHideColumn";
        internal const string CommandHideRow = "CommandHideRow";
        internal const string CommandInsertColumn = "CommandInsertColumn";
        internal const string CommandInsertRow = "CommandInsertRow";
        internal const string CommandMoveColumn = "CommandMoveColumn";
        internal const string CommandMoveRow = "CommandMoveRow";
        internal const string CommandRemoveColumn = "CommandRemoveColumn";
        internal const string CommandRemoveCoveredRanges = "CommandRemoveCoveredRanges";
        internal const string CommandRemoveRow = "CommandRemoveRow";
        internal const string CommandRowHeight = "CommandRowHeight";
        internal const string DateTimeFormat_D = "DateTimeFormat_D";
        internal const string DateTimeFormat_dd = "DateTimeFormat_dd";
        internal const string DateTimeFormat_F = "DateTimeFormat_F";
        internal const string DateTimeFormat_ff = "DateTimeFormat_ff";
        internal const string DateTimeFormat_G = "DateTimeFormat_G";
        internal const string DateTimeFormat_gg = "DateTimeFormat_gg";
        internal const string DateTimeFormat_M = "DateTimeFormat_M";
        internal const string DateTimeFormat_R = "DateTimeFormat_R";
        internal const string DateTimeFormat_ss = "DateTimeFormat_ss";
        internal const string DateTimeFormat_T = "DateTimeFormat_T";
        internal const string DateTimeFormat_tt = "DateTimeFormat_tt";
        internal const string DateTimeFormat_U = "DateTimeFormat_U";
        internal const string DateTimeFormat_uu = "DateTimeFormat_uu";
        internal const string DateTimeFormat_Y = "DateTimeFormat_Y";
        internal const string DescriptionChangeCells = "DescriptionChangeCells";
        internal const string DescriptionInsertColumn = "DescriptionInsertColumn";
        internal const string DescriptionInsertRow = "DescriptionInsertRow";
        internal const string DescriptionMoveColumn = "DescriptionMoveColumn";
        internal const string DescriptionMoveRow = "DescriptionMoveRow";
        internal const string DescriptionRemoveColumn = "DescriptionRemoveColumn";
        internal const string DescriptionRemoveRow = "DescriptionRemoveRow";
        internal const string EnumFormat_D = "EnumFormat_D";
        internal const string EnumFormat_G = "EnumFormat_G";
        internal const string EnumFormat_X = "EnumFormat_X";
        internal const string Error = "Error";
        internal const string ExceptionDataRangeDiffColCount = "ExceptionDataRangeDiffColCount";
        internal const string ExceptionDataRangeDiffRowCount = "ExceptionDataRangeDiffRowCount";
        internal const string GeneralFormat_G = "GeneralFormat_G";
        internal const string GRID_IDM_CLEARDATA = "GRID_IDM_CLEARDATA";
        internal const string GRID_IDM_COPYINTERNAL = "GRID_IDM_COPYINTERNAL";
        internal const string GRID_IDM_CUTDATA = "GRID_IDM_CUTDATA";
        internal const string GRID_IDM_DRAGDROP_COPY = "GRID_IDM_DRAGDROP_COPY";
        internal const string GRID_IDM_DRAGDROP_MOVE = "GRID_IDM_DRAGDROP_MOVE";
        internal const string GRID_IDM_PASTEDATA = "GRID_IDM_PASTEDATA";
        internal const string GRID_IDM_PASTEDIFFRANGE = "GRID_IDM_PASTEDIFFRANGE";
        internal const string GRID_IDM_PASTINGDATA = "GRID_IDM_PASTINGDATA";
        internal const string GRID_IDM_REMOVECOLS = "GRID_IDM_REMOVECOLS";
        internal const string GRID_IDM_REMOVEROWS = "GRID_IDM_REMOVEROWS";
        internal const string GRID_IDM_RESIZECOLS = "GRID_IDM_RESIZECOLS";
        internal const string GRID_IDM_RESIZEROWS = "GRID_IDM_RESIZEROWS";
        internal const string GRID_IDS_COLOR_BACKGROUND = "GRID_IDS_COLOR_BACKGROUND";
        internal const string GRID_IDS_COLOR_DRAGGINGLINE = "GRID_IDS_COLOR_DRAGGINGLINE";
        internal const string GRID_IDS_COLOR_FIXEDLINES = "GRID_IDS_COLOR_FIXEDLINES";
        internal const string GRID_IDS_COLOR_GRIDLINES = "GRID_IDS_COLOR_GRIDLINES";
        internal const string GRID_IDS_COLOR_TRACKINGLINE = "GRID_IDS_COLOR_TRACKINGLINE";
        internal const string GRID_IDS_INVERTDRAWBORDER = "GRID_IDS_INVERTDRAWBORDER";
        internal const string GRID_IDS_INVERTNOBORDER = "GRID_IDS_INVERTNOBORDER";
        internal const string GRID_IDS_INVERTNORMAL = "GRID_IDS_INVERTNORMAL";
        internal const string GRID_IDS_INVERTTHICK = "GRID_IDS_INVERTTHICK";
        internal const string GRID_IDS_INVERTTHICKBORDER = "GRID_IDS_INVERTTHICKBORDER";
        internal const string GRID_IDS_OUTLINECURRENTCELL = "GRID_IDS_OUTLINECURRENTCELL";
        internal const string GridCheckBoxCellModel = "GridCheckBoxCellModel";
        internal const string GridCheckBoxCellModelDesc = "GridCheckBoxCellModelDesc";
        internal const string GridComboBoxCellModel = "GridComboBoxCellModel";
        internal const string GridComboBoxCellModelDesc = "GridComboBoxCellModelDesc";
        internal const string GridControlName = "GridControlName";
        internal const string GridCustomStyleProperty_CheckBoxState = "GridCustomStyleProperty_CheckBoxState";
        internal const string GridCustomStyleProperty_NumberFormatInfo = "GridCustomStyleProperty_NumberFormatInfo";
        internal const string GridCustomStyleProperty_NumericUpDown = "GridCustomStyleProperty_NumericUpDown";
        internal const string GridCustomStyleProperty_ParseInfo = "GridCustomStyleProperty_ParseInfo";
        internal const string GridCustomStyleProperty_Validation = "GridCustomStyleProperty_Validation";
        internal const string GridDesignerCodeGenFailed = "GridDesignerCodeGenFailed";
        internal const string GridDesignerGeneratedCode = "GridDesignerGeneratedCode";
        internal const string GridDesignerNoCode = "GridDesignerNoCode";
        internal const string GridDesignerNoSelection = "GridDesignerNoSelection";
        internal const string GridDesignerStateChangeFailed = "GridDesignerStateChangeFailed";
        internal const string GridDesignerTemplateExportFailed = "GridDesignerTemplateExportFailed";
        internal const string GridDesignerTemplateImportFailed = "GridDesignerTemplateImportFailed";
        internal const string GridDragButtonCellModel = "GridDragButtonCellModel";
        internal const string GridDragButtonCellModelDesc = "GridDragButtonCellModelDesc";
        internal const string GridDropDownCellModel = "GridDropDownCellModel";
        internal const string GridDropDownCellModelDesc = "GridDropDownCellModelDesc";
        internal const string GridDropDownColorUICellModel = "GridDropDownColorUICellModel";
        internal const string GridDropDownColorUICellModelDesc = "GridDropDownColorUICellModelDesc";
        internal const string GridDropDownGridCellModel = "GridDropDownGridCellModel";
        internal const string GridDropDownGridCellModelDesc = "GridDropDownGridCellModelDesc";
        internal const string GridDropDownHeaderCellModel = "GridDropDownHeaderCellModel";
        internal const string GridDropDownHeaderCellModelDesc = "GridDropDownHeaderCellModelDesc";
        internal const string GridDropDownMonthCalendarCellModel = "GridDropDownMonthCalendarCellModel";
        internal const string GridDropDownMonthCalendarCellModelDesc = "GridDropDownMonthCalendarCellModelDesc";
        internal const string GridDropDownRichTextBoxCellModel = "GridDropDownRichTextBoxCellModel";
        internal const string GridDropDownRichTextBoxCellModelDesc = "GridDropDownRichTextBoxCellModelDesc";
        internal const string GridHeaderCellModel = "GridHeaderCellModel";
        internal const string GridHeaderCellModelDesc = "GridHeaderCellModelDesc";
        internal const string GridNumericUpDownCellModel = "GridNumericUpDownCellModel";
        internal const string GridNumericUpDownCellModelDesc = "GridNumericUpDownCellModelDesc";
        internal const string GridPushButtonCellModel = "GridPushButtonCellModel";
        internal const string GridPushButtonCellModelDesc = "GridPushButtonCellModelDesc";
        internal const string GridStaticCellModel = "GridStaticCellModel";
        internal const string GridStaticCellModelDesc = "GridStaticCellModelDesc";
        internal const string GridStyleCategoryAlignment = "GridStyleCategoryAlignment";
        internal const string GridStyleCategoryAppearance = "GridStyleCategoryAppearance";
        internal const string GridStyleCategoryBehavior = "GridStyleCategoryBehavior";
        internal const string GridStyleCategoryDisplay = "GridStyleCategoryDisplay";
        internal const string GridStyleCategoryGraphics = "GridStyleCategoryGraphics";
        internal const string GridStyleCategoryStyle = "GridStyleCategoryStyle";
        internal const string GridStyleCategoryUser = "GridStyleCategoryUser";
        internal const string GridStyleCategoryValue = "GridStyleCategoryValue";
        internal const string GridTextBoxCellModel = "GridTextBoxCellModel";
        internal const string GridTextBoxCellModelDesc = "GridTextBoxCellModelDesc";
        internal const string InvalidOperation_GridCellNotInitialized = "InvalidOperation_GridCellNotInitialized";
        internal const string ModName = "ModName";
        internal const string NumberFormat_C = "NumberFormat_C";
        internal const string NumberFormat_D = "NumberFormat_D";
        internal const string NumberFormat_E = "NumberFormat_E";
        internal const string NumberFormat_F = "NumberFormat_F";
        internal const string NumberFormat_G = "NumberFormat_G";
        internal const string NumberFormat_N = "NumberFormat_N";
        internal const string NumberFormat_P = "NumberFormat_P";
        internal const string NumberFormat_R = "NumberFormat_R";
        internal const string NumberFormat_X = "NumberFormat_X";
        internal const string StyleCategoryAppearance = "StyleCategoryAppearance";
        internal const string StyleCategoryBehavior = "StyleCategoryBehavior";
        internal const string StyleCategoryStyle = "StyleCategoryStyle";
        internal const string StyleCategoryValue = "StyleCategoryValue";
        internal const string Test = "Test";
        internal const string TextParseFailedFormat = "TextParseFailedFormat";
        internal const string TopLeftBottomRight = "TopLeftBottomRight";
        internal const string Cannotchangepartofamergedcell = "Cannotchangepartofamergedcell";
        internal const string SpecifiedTextWasNotFound = "SpecifiedTextWasNotFound";
        internal const string SearchResults = "SearchResults";
        internal const string FindReachedTheStartingPointOfTheSearch="FindReachedTheStartingPointOfTheSearch";
        internal const string GridFindAndReplaceReplaceAll = "GridFindAndReplaceReplaceAll";
        internal const string GridFindAndReplaceReplaceWith="GridFindAndReplaceReplaceWith";
        internal const string GridFindAndReplaceSearchUp = "GridFindAndReplaceSearchUp";
        internal const string GridFindAndReplaceMatchCase = "GridFindAndReplaceMatchCase";
        internal const string GridFindAndReplaceFindWhat = "GridFindAndReplaceFindWhat";
        internal const string GridFindAndReplaceMatchWholeCell = "GridFindAndReplaceMatchWholeCell";
        internal const string Close = "Close";
        internal const string GridFindAndReplace = "GridFindAndReplace";
        internal const string GridFindAndReplaceFindNext="GridFindAndReplaceFindNext";
        internal const string DoYouWantToReplaceTheContentsOfTheDestinationCellsIfAny = "DoYouWantToReplaceTheContentsOfTheDestinationCellsIfAny";
        internal const string Request = "Request";
        internal const string UnabletoLoadImage = "UnabletoLoadImage";
        internal const string LoadFailed = "LoadFailed";
        internal const string AnErrorOccurredAttemptingToPreviewtheFiletoprint = "AnErrorOccurredAttemptingToPreviewtheFiletoprint";
        internal const string AnErrorOccurred = "AnErrorOccurred";
        internal const string FileSaved="FileSaved";
        internal const string UnabletoLoadtheSavedTemplate = "UnabletoLoadtheSavedTemplate";
        internal const string LoadFailure="LoadFailure";

        private SR()
        {
            this.resources = new ResourceManager(this.GetType());
        }

        /// <summary>
        /// Loads the Localized string for the given strings.
        /// </summary>
        /// <returns>Localized string for the given string.</returns>
        private static SR GetLoader()
        {
            lock (typeof(SR))
            {
                if (SR.loader == null)
                {
                    SR.loader = new SR();
                }

                return SR.loader;
            }
        }

        static bool isGetStringError = false;
        /// <summary>
        /// This reads the string from the satellite assemblies and used it in the program. If the satellite Assemblies are not present, then the resources are read from SR.resources.
        /// </summary>
        /// <param name="culture">Specifies the culture of the Application.</param>
        /// <param name="name">Specifies the string for which localization has to be applied.</param>
        /// <param name="args">Arguments in the type of object.</param>
        /// <returns></returns>
        public static string GetString(CultureInfo culture, string name, params object[] args)
        {
            if (isGetStringError)
                return name;

            SR sr = SR.GetLoader();
            string value=string.Empty;

            if (sr == null)
            {
                return null;
            }

            try
            {
                if (LocalizationProvider.Provider != null)
                {
                    String result = string.Empty;

                    result = LocalizationProvider.Provider.GetLocalizedString(culture, name, args);

                    if (result != string.Empty)
                        return result;
                }

                if (name != null)
                    value = sr.resources.GetString(name, culture);
                if (value != null && args != null && args.Length > 0)
                {
                    return String.Format(value, args);
                }

                return value;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                //TraceUtil.TraceExceptionCatched(ex);
                //if (!ExceptionManager.RaiseExceptionCatched(null, ex))
                //{
                //    throw;
                //}
                isGetStringError = true;
                return name;
            }
        }
        /// <summary>
        /// Gets the localized string for the given string.
        /// </summary>
        /// <param name="name">The string that has to be localized.</param>
        /// <returns>Returns the localized string in the type string.</returns>
        public static string GetString(string name)
        {
            return SR.GetString(null, name);
        }
        /// <summary>
        ///  Gets the localized string.
        /// </summary>
        /// <param name="name">The string that has to be localized.</param>
        /// <param name="args">Arguments in the type of object.</param>
        /// <returns>The localized string in the type of string.</returns>
        public static string GetString(string name, params object[] args)
        {
            return SR.GetString(null, name, args);
        }
        /// <summary>
        /// Gets the localized string for the given string.
        /// </summary>
        /// <param name="culture">Specified the culture into which the string has to be localized.</param>
        /// <param name="name">Arguments in the type of object.</param>
        /// <returns>The localized string in the type of string.</returns
        public static string GetString(CultureInfo culture, string name)
        {
            SR sr = SR.GetLoader();
            if (sr == null)
                return null;
            try
            {
                if (LocalizationProvider.Provider != null)
                {
                    String result = string.Empty;

                    result = LocalizationProvider.Provider.GetLocalizedString(culture, name, null);

                    if (result != string.Empty)
                        return result;
                }
                return sr.resources.GetString(name, culture);
            }
            catch
            {
                return name;
            }
        }
        /// <summary>
        /// Gets the localized string for the given string.
        /// </summary>
        /// <param name="culture">Specifies the culture into which the string has to be localized.</param>
        /// <param name="name">Arguments in the type of object.</param>
        /// <returns>The localized string in the type of string.</returns>
        public static object GetObject(CultureInfo culture, string name)
        {
            SR sr = SR.GetLoader();
            if (sr == null)
            {
                return null;
            }

            return sr.resources.GetObject(name, culture);
        }
        /// <summary>
        /// Gets the localized string for the given string.
        /// </summary>
        /// <param name="name">string that has to be localized.</param>
        /// <returns></returns>
        public static object GetObject(string name)
        {
            return SR.GetObject(null, name);
        }
        /// <summary>
        /// Returns the boolean value for the string. 
        /// </summary>
        /// <param name="culture">Specifies the culture into which the string has to be localized.</param>
        /// <param name="name">The string that has to be localized.</param>
        /// <returns>The boolean value for the string.</returns>
        public static bool GetBoolean(CultureInfo culture, string name)
        {
            bool value;
            SR sr = SR.GetLoader();
            object obj;
            value = false;
            if (sr != null)
            {
                obj = sr.resources.GetObject(name, culture);
                if (obj is System.Boolean)
                {
                    value = (bool)obj;
                }
            }

            return value;
        }
        /// <summary>
        /// Returns the boolean value for the string. 
        /// </summary>
        /// <param name="name">The string that has to be localized.</param>
        /// <returns>The boolean value for the string.</returns>
        public static bool GetBoolean(string name)
        {
            return SR.GetBoolean(name);
        }

        /// <summary>
        /// Returns the Byte value for the string. 
        /// </summary>
        /// <param name="culture">Specifies the culture into which the string has to be localized.</param>
        /// <param name="name">The string that has to be localized.</param>
        /// <returns>The byte value for the string.</returns>
        public static byte GetByte(CultureInfo culture, string name)
        {
            byte value;
            SR sr = SR.GetLoader();
            object obj;
            value = (byte)0;
            if (sr != null)
            {
                obj = sr.resources.GetObject(name, culture);
                if (obj is System.Byte)
                {
                    value = (byte)obj;
                }
            }

            return value;
        }

        /// <summary>
        /// Gets the localized string for the given string.
        /// </summary>
        /// <param name="name">string that has to be localized.</param>
        /// <returns>The byte value for the string.</returns>
        public static byte GetByte(string name)
        {
            return SR.GetByte(null, name);
        }
        
        public static char GetChar(CultureInfo culture, string name)
        {
            char value;
            SR sr = SR.GetLoader();
            object obj;
            value = (char)0;
            if (sr != null)
            {
                obj = sr.resources.GetObject(name, culture);
                if (obj is System.Char)
                {
                    value = (char)obj;
                }
            }

            return value;
        }

        /// <summary>
        /// Returns the Char value for the string. 
        /// </summary>
        /// <param name="culture">Specifies the culture into which the string has to be localized.</param>
        /// <param name="name">The string that has to be localized.</param>
        /// <returns>The char value for the string.</returns>
        public static char GetChar(string name)
        {
            return SR.GetChar(null, name);
        }
        /// <summary>
        /// Returns the Double value for the string. 
        /// </summary>
        /// <param name="culture">Specifies the culture into which the string has to be localized.</param>
        /// <param name="name">The string that has to be localized.</param>
        /// <returns>The double value for the string.</returns>
        public static double GetDouble(CultureInfo culture, string name)
        {
            double value;
            SR sr = SR.GetLoader();
            object obj;
            value = 0.0;
            if (sr != null)
            {
                obj = sr.resources.GetObject(name, culture);
                if (obj is System.Double)
                {
                    value = (double)obj;
                }
            }

            return value;
        }

        /// <summary>
        /// Gets the localized string for the given string.
        /// </summary>
        /// <param name="name">string that has to be localized.</param>
        /// <returns>The double value for the string.</returns>
        public static double GetDouble(string name)
        {
            return SR.GetDouble(null, name);
        }

        /// <summary>
        /// Returns the Float value for the string. 
        /// </summary>
        /// <param name="culture">Specifies the culture into which the string has to be localized.</param>
        /// <param name="name">The string that has to be localized.</param>
        /// <returns>The float value for the string.</returns>
        public static float GetFloat(CultureInfo culture, string name)
        {
            float value;
            SR sr = SR.GetLoader();
            object obj;
            value = 0.0f;
            if (sr != null)
            {
                obj = sr.resources.GetObject(name, culture);
                if (obj is System.Single)
                {
                    value = (float)obj;
                }
            }

            return value;
        }

        /// <summary>
        /// Gets the localized string for the given string.
        /// </summary>
        /// <param name="name">string that has to be localized.</param>
        /// <returns>The float value for the string.</returns>
        public static float GetFloat(string name)
        {
            return SR.GetFloat(null, name);
        }

        /// <summary>
        /// Returns the integer value for the string. 
        /// </summary>
        /// <param name="name">The string that has to be localized.</param>
        /// <returns>The integer value for the string.</returns>
        public static int GetInt(string name)
        {
            return SR.GetInt(null, name);
        }

        /// <summary>
        /// Returns the integer value for the string. 
        /// </summary>
        /// <param name="culture">Specifies the culture into which the string has to be localized.</param>
        /// <param name="name">The string that has to be localized.</param>
        /// <returns>The integer value for the string.</returns>
        public static int GetInt(CultureInfo culture, string name)
        {
            int value;
            SR sr = SR.GetLoader();
            object obj;
            value = 0;
            if (sr != null)
            {
                obj = sr.resources.GetObject(name, culture);
                if (obj is System.Int32)
                {
                    value = (int)obj;
                }
            }

            return value;
        }

        /// <summary>
        /// Gets the localized string for the given string.
        /// </summary>
        /// <param name="name">string that has to be localized.</param>
        /// <returns>The long value for the string.</returns>
        public static long GetLong(string name)
        {
            return SR.GetLong(null, name);
        }

        /// <summary>
        /// Returns the Long value for the string. 
        /// </summary>
        /// <param name="culture">Specifies the culture into which the string has to be localized.</param>
        /// <param name="name">The string that has to be localized.</param>
        /// <returns>The long value for the string.</returns>
        public static long GetLong(CultureInfo culture, string name)
        {
            Int64 value;
            SR sr = SR.GetLoader();
            object obj;
            value = (Int64)0;
            if (sr != null)
            {
                obj = sr.resources.GetObject(name, culture);
                if (obj is System.Int64)
                {
                    value = (Int64)obj;
                }
            }
            
            return value;
        }

        /// <summary>
        /// Returns the short value for the string. 
        /// </summary>
        /// <param name="culture">Specifies the culture into which the string has to be localized.</param>
        /// <param name="name">The string that has to be localized.</param>
        /// <returns>The short value for the string.</returns>
        public static short GetShort(CultureInfo culture, string name)
        {
            short value;
            SR sr = SR.GetLoader();
            object obj;
            value = (short)0;
            if (sr != null)
            {
                obj = sr.resources.GetObject(name, culture);
                if (obj is System.Int16)
                {
                    value = (short)obj;
                }
            }

            return value;
        }

        /// <summary>
        /// Gets the localized string for the given string.
        /// </summary>
        /// <param name="name">string that has to be localized.</param>
        /// <returns>The short value for the string.</returns
        public static short GetShort(string name)
        {
            return SR.GetShort(null, name);
        }
    }

    /// <summary>
    /// Specifies the category in which the property or event will be displayed in a visual designer.
    /// </summary>
    /// <remarks>
    /// This is a localized version of CategoryAttribute. The localized string will be loaded from the 
    /// assembly manifest Syncfusion.Windows.Forms.Grid.SR.resources
    /// </remarks>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    sealed class SRCategoryAttribute : CategoryAttribute
    {
        public SRCategoryAttribute(string category)
            : base(category)
        {
        }

        protected override string GetLocalizedString(string value)
        {
            return SR.GetString(value);
        }
    }

    /// <summary>
    /// Specifies a description for a property or event.
    /// </summary>
    /// <remarks>
    /// This is a localized version of DescriptionAttribute. The localized string will be loaded from the 
    /// assembly manifest Syncfusion.Windows.Forms.Grid.SR.resources
    /// </remarks>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    sealed class SRDescriptionAttribute : DescriptionAttribute
    {
        private bool replaced = false;

        public SRDescriptionAttribute(string description)
            : base(description)
        {
        }

        public override string Description
        {
            get
            {
                if (!this.replaced)
                {
                    this.replaced = true;
                    this.DescriptionValue = SR.GetString(base.Description);
                }

                return base.Description;
            }
        }
    }
    /// <summary>
    /// class for the constant string value of GridResourceIdentifiers
    /// </summary>
    public sealed class GridResourceIdentifiers
    {
        #region Strings Constants
        /// <summary>
        /// FilterBarAll
        /// </summary>
        public const string FilterBarAll = "FilterBarAll";
        /// <summary>
        /// string value for BitVectorFull
        /// </summary>
        public const string BitVectorFull = "BitVectorFull";
        /// <summary>
        /// ColorEditorPaletteTab
        /// </summary>
        public const string ColorEditorPaletteTab = "ColorEditorPaletteTab";
        /// <summary>
        /// ColorEditorStandardTab
        /// </summary>
        public const string ColorEditorStandardTab = "ColorEditorStandardTab";
        /// <summary>
        /// ColorEditorSystemTab
        /// </summary>
        public const string ColorEditorSystemTab = "ColorEditorSystemTab";
        /// <summary>
        /// CommandAddCoveredRanges
        /// </summary>
        public const string CommandAddCoveredRanges = "CommandAddCoveredRanges";
        /// <summary>
        /// CommandChangeCells
        /// </summary>
        public const string CommandChangeCells = "CommandChangeCells";
        /// <summary>
        /// CommandColumnWidth
        /// </summary>
        public const string CommandColumnWidth = "CommandColumnWidth";
        /// <summary>
        /// CommandDefaultSizeColumn
        /// </summary>
        public const string CommandDefaultSizeColumn = "CommandDefaultSizeColumn";
        /// <summary>
        /// CommandDefaultSizeRow
        /// </summary>
        public const string CommandDefaultSizeRow = "CommandDefaultSizeRow";
        /// <summary>
        /// CommandFixedCountColumn
        /// </summary>
        public const string CommandFixedCountColumn = "CommandFixedCountColumn";
        /// <summary>
        /// CommandFixedCountRow
        /// </summary>
        public const string CommandFixedCountRow = "CommandFixedCountRow";
        /// <summary>
        /// CommandHeaderCountColumn
        /// </summary>
        public const string CommandHeaderCountColumn = "CommandHeaderCountColumn";
        /// <summary>
        /// CommandHeaderCountRow
        /// </summary>
        public const string CommandHeaderCountRow = "CommandHeaderCountRow";
        /// <summary>
        /// CommandHideColumn
        /// </summary>
        public const string CommandHideColumn = "CommandHideColumn";
        /// <summary>
        /// CommandHideRow
        /// </summary>
        public const string CommandHideRow = "CommandHideRow";
        /// <summary>
        /// CommandInsertColumn
        /// </summary>
        public const string CommandInsertColumn = "CommandInsertColumn";
        /// <summary>
        /// CommandInsertRow
        /// </summary>
        public const string CommandInsertRow = "CommandInsertRow";
        /// <summary>
        /// CommandMoveColumn
        /// </summary>
        public const string CommandMoveColumn = "CommandMoveColumn";
        /// <summary>
        /// CommandMoveRow
        /// </summary>
        public const string CommandMoveRow = "CommandMoveRow";
        /// <summary>
        /// CommandRemoveColumn
        /// </summary>
        public const string CommandRemoveColumn = "CommandRemoveColumn";
        /// <summary>
        /// CommandRemoveCoveredRanges
        /// </summary>
        public const string CommandRemoveCoveredRanges = "CommandRemoveCoveredRanges";
        /// <summary>
        /// CommandRemoveRow
        /// </summary>
        public const string CommandRemoveRow = "CommandRemoveRow";
        /// <summary>
        /// CommandRowHeight
        /// </summary>
        public const string CommandRowHeight = "CommandRowHeight";
        /// <summary>
        /// DateTimeFormat_D
        /// </summary>
        public const string DateTimeFormat_D = "DateTimeFormat_D";
        /// <summary>
        /// DateTimeFormat_dd
        /// </summary>
        public const string DateTimeFormat_dd = "DateTimeFormat_dd";
        /// <summary>
        /// DateTimeFormat_F
        /// </summary>
        public const string DateTimeFormat_F = "DateTimeFormat_F";
        /// <summary>
        /// DateTimeFormat_ff
        /// </summary>
        public const string DateTimeFormat_ff = "DateTimeFormat_ff";
        /// <summary>
        /// DateTimeFormat_G
        /// </summary>
        public const string DateTimeFormat_G = "DateTimeFormat_G";
        /// <summary>
        /// DateTimeFormat_gg
        /// </summary>
        public const string DateTimeFormat_gg = "DateTimeFormat_gg";
        /// <summary>
        /// DateTimeFormat_M
        /// </summary>
        public const string DateTimeFormat_M = "DateTimeFormat_M";
        /// <summary>
        /// DateTimeFormat_R
        /// </summary>
        public const string DateTimeFormat_R = "DateTimeFormat_R";
        /// <summary>
        /// DateTimeFormat_ss
        /// </summary>
        public const string DateTimeFormat_ss = "DateTimeFormat_ss";
        /// <summary>
        /// DateTimeFormat_T
        /// </summary>
        public const string DateTimeFormat_T = "DateTimeFormat_T";
        /// <summary>
        /// DateTimeFormat_tt
        /// </summary>
        public const string DateTimeFormat_tt = "DateTimeFormat_tt";
        /// <summary>
        /// DateTimeFormat_U
        /// </summary>
        public const string DateTimeFormat_U = "DateTimeFormat_U";
        /// <summary>
        /// DateTimeFormat_uu
        /// </summary>
        public const string DateTimeFormat_uu = "DateTimeFormat_uu";
        /// <summary>
        /// DateTimeFormat_Y
        /// </summary>
        public const string DateTimeFormat_Y = "DateTimeFormat_Y";
        /// <summary>
        /// DescriptionChangeCells
        /// </summary>
        public const string DescriptionChangeCells = "DescriptionChangeCells";
        /// <summary>
        /// DescriptionInsertColumn
        /// </summary>
        public const string DescriptionInsertColumn = "DescriptionInsertColumn";
        /// <summary>
        /// DescriptionInsertRow
        /// </summary>
        public const string DescriptionInsertRow = "DescriptionInsertRow";
        /// <summary>
        /// DescriptionMoveColumn
        /// </summary>
        public const string DescriptionMoveColumn = "DescriptionMoveColumn";
        /// <summary>
        /// DescriptionMoveRow
        /// </summary>
        public const string DescriptionMoveRow = "DescriptionMoveRow";

        /// <summary>
        /// DescriptionRemoveColumn
        /// </summary>
        public const string DescriptionRemoveColumn = "DescriptionRemoveColumn";

        /// <summary>
        /// DescriptionRemoveRow
        /// </summary>
        public const string DescriptionRemoveRow = "DescriptionRemoveRow";
        /// <summary>
        /// EnumFormat_D
        /// </summary>
        public const string EnumFormat_D = "EnumFormat_D";
        /// <summary>
        /// EnumFormat_G
        /// </summary>
        public const string EnumFormat_G = "EnumFormat_G";
        /// <summary>
        /// EnumFormat_X
        /// </summary>
        public const string EnumFormat_X = "EnumFormat_X";
        /// <summary>
        /// Error
        /// </summary>
        public const string Error = "Error";
        /// <summary>
        /// ExceptionDataRangeDiffColCount
        /// </summary>
        public const string ExceptionDataRangeDiffColCount = "ExceptionDataRangeDiffColCount";
        /// <summary>
        /// ExceptionDataRangeDiffRowCount
        /// </summary>
        public const string ExceptionDataRangeDiffRowCount = "ExceptionDataRangeDiffRowCount";
        /// <summary>
        /// GeneralFormat_G
        /// </summary>
        public const string GeneralFormat_G = "GeneralFormat_G";
        /// <summary>
        /// GRID_IDM_CLEARDATA
        /// </summary>
        public const string GRID_IDM_CLEARDATA = "GRID_IDM_CLEARDATA";
        /// <summary>
        /// GRID_IDM_COPYINTERNAL
        /// </summary>
        public const string GRID_IDM_COPYINTERNAL = "GRID_IDM_COPYINTERNAL";
        /// <summary>
        /// GRID_IDM_CUTDATA
        /// </summary>
        public const string GRID_IDM_CUTDATA = "GRID_IDM_CUTDATA";
        /// <summary>
        /// GRID_IDM_DRAGDROP_COPY
        /// </summary>
        public const string GRID_IDM_DRAGDROP_COPY = "GRID_IDM_DRAGDROP_COPY";
        /// <summary>
        /// GRID_IDM_DRAGDROP_MOVE
        /// </summary>
        public const string GRID_IDM_DRAGDROP_MOVE = "GRID_IDM_DRAGDROP_MOVE";
        /// <summary>
        /// GRID_IDM_PASTEDATA
        /// </summary>
        public const string GRID_IDM_PASTEDATA = "GRID_IDM_PASTEDATA";
        /// <summary>
        /// GRID_IDM_PASTEDIFFRANGE
        /// </summary>
        public const string GRID_IDM_PASTEDIFFRANGE = "GRID_IDM_PASTEDIFFRANGE";
        /// <summary>
        /// GRID_IDM_PASTINGDATA
        /// </summary>
        public const string GRID_IDM_PASTINGDATA = "GRID_IDM_PASTINGDATA";
        /// <summary>
        /// GRID_IDM_REMOVECOLS
        /// </summary>
        public const string GRID_IDM_REMOVECOLS = "GRID_IDM_REMOVECOLS";
        /// <summary>
        /// GRID_IDM_REMOVEROWS
        /// </summary>
        public const string GRID_IDM_REMOVEROWS = "GRID_IDM_REMOVEROWS";
        /// <summary>
        /// GRID_IDM_RESIZECOLS
        /// </summary>
        public const string GRID_IDM_RESIZECOLS = "GRID_IDM_RESIZECOLS";
        /// <summary>
        /// GRID_IDM_RESIZEROWS
        /// </summary>
        public const string GRID_IDM_RESIZEROWS = "GRID_IDM_RESIZEROWS";
        /// <summary>
        /// GRID_IDS_COLOR_BACKGROUND
        /// </summary>
        public const string GRID_IDS_COLOR_BACKGROUND = "GRID_IDS_COLOR_BACKGROUND";
        /// <summary>
        /// GRID_IDS_COLOR_DRAGGINGLINE
        /// </summary>
        public const string GRID_IDS_COLOR_DRAGGINGLINE = "GRID_IDS_COLOR_DRAGGINGLINE";
        /// <summary>
        /// GRID_IDS_COLOR_FIXEDLINES
        /// </summary>
        public const string GRID_IDS_COLOR_FIXEDLINES = "GRID_IDS_COLOR_FIXEDLINES";
        /// <summary>
        /// GRID_IDS_COLOR_GRIDLINES
        /// </summary>
        public const string GRID_IDS_COLOR_GRIDLINES = "GRID_IDS_COLOR_GRIDLINES";
        /// <summary>
        /// GRID_IDS_COLOR_TRACKINGLINE
        /// </summary>
        public const string GRID_IDS_COLOR_TRACKINGLINE = "GRID_IDS_COLOR_TRACKINGLINE";
        /// <summary>
        /// GRID_IDS_INVERTDRAWBORDER
        /// </summary>
        public const string GRID_IDS_INVERTDRAWBORDER = "GRID_IDS_INVERTDRAWBORDER";
        /// <summary>
        /// GRID_IDS_INVERTNOBORDER
        /// </summary>
        public const string GRID_IDS_INVERTNOBORDER = "GRID_IDS_INVERTNOBORDER";
        /// <summary>
        /// GRID_IDS_INVERTNORMAL
        /// </summary>
        public const string GRID_IDS_INVERTNORMAL = "GRID_IDS_INVERTNORMAL";
        /// <summary>
        /// GRID_IDS_INVERTTHICK
        /// </summary>
        public const string GRID_IDS_INVERTTHICK = "GRID_IDS_INVERTTHICK";
        /// <summary>
        /// GRID_IDS_INVERTTHICK
        /// </summary>
        public const string GRID_IDS_INVERTTHICKBORDER = "GRID_IDS_INVERTTHICK";
        /// <summary>
        /// GRID_IDS_OUTLINECURRENTCELL
        /// </summary>
        public const string GRID_IDS_OUTLINECURRENTCELL = "GRID_IDS_OUTLINECURRENTCELL";
        /// <summary>
        /// GridCheckBoxCellModel
        /// </summary>
        public const string GridCheckBoxCellModel = "GridCheckBoxCellModel";
        /// <summary>
        /// GridCheckBoxCellModelDesc
        /// </summary>
        public const string GridCheckBoxCellModelDesc = "GridCheckBoxCellModelDesc";
        /// <summary>
        /// GridComboBoxCellModel
        /// </summary>
        public const string GridComboBoxCellModel = "GridComboBoxCellModel";
        /// <summary>
        /// GridComboBoxCellModelDesc
        /// </summary>
        public const string GridComboBoxCellModelDesc = "GridComboBoxCellModelDesc";
        /// <summary>
        /// GridControlName
        /// </summary>
        public const string GridControlName = "GridControlName";
        /// <summary>
        /// GridCustomStyleProperty_CheckBoxState
        /// </summary>
        public const string GridCustomStyleProperty_CheckBoxState = 
            "GridCustomStyleProperty_CheckBoxState";
        /// <summary>
        /// GridCustomStyleProperty_NumberFormatInfo
        /// </summary>
        public const string GridCustomStyleProperty_NumberFormatInfo = "GridCustomStyleProperty_NumberFormatInfo";
        /// <summary>
        /// GridCustomStyleProperty_NumericUpDown
        /// </summary>
        public const string GridCustomStyleProperty_NumericUpDown = "GridCustomStyleProperty_NumericUpDown";
        /// <summary>
        /// GridCustomStyleProperty_ParseInfo
        /// </summary>
        public const string GridCustomStyleProperty_ParseInfo = "GridCustomStyleProperty_ParseInfo";
        /// <summary>
        /// GridCustomStyleProperty_Validation
        /// </summary>
        public const string GridCustomStyleProperty_Validation = "GridCustomStyleProperty_Validation";
        /// <summary>
        /// GridDesignerCodeGenFailed
        /// </summary>
        public const string GridDesignerCodeGenFailed = "GridDesignerCodeGenFailed";
        /// <summary>
        /// GridDesignerGeneratedCode
        /// </summary>
        public const string GridDesignerGeneratedCode = "GridDesignerGeneratedCode";
        /// <summary>
        /// GridDesignerNoCode
        /// </summary>
        public const string GridDesignerNoCode = "GridDesignerNoCode";
        /// <summary>
        /// GridDesignerNoSelection
        /// </summary>
        public const string GridDesignerNoSelection = "GridDesignerNoSelection";
        /// <summary>
        /// GridDesignerStateChangeFailed
        /// </summary>
        public const string GridDesignerStateChangeFailed = "GridDesignerStateChangeFailed";
        /// <summary>
        /// GridDesignerTemplateExportFailed
        /// </summary>
        public const string GridDesignerTemplateExportFailed = "GridDesignerTemplateExportFailed";
        /// <summary>
        /// GridDesignerTemplateImportFailed
        /// </summary>
        public const string GridDesignerTemplateImportFailed = "GridDesignerTemplateImportFailed";
        /// <summary>
        /// GridDragButtonCellModel
        /// </summary>
        public const string GridDragButtonCellModel = "GridDragButtonCellModel";
        /// <summary>
        /// GridDragButtonCellModelDesc
        /// </summary>
        public const string GridDragButtonCellModelDesc = "GridDragButtonCellModelDesc";
        /// <summary>
        /// GridDropDownCellModel
        /// </summary>
        public const string GridDropDownCellModel = "GridDropDownCellModel";
        /// <summary>
        /// GridDropDownCellModelDesc
        /// </summary>
        public const string GridDropDownCellModelDesc = "GridDropDownCellModelDesc";
        /// <summary>
        /// GridDropDownColorUICellModel
        /// </summary>
        public const string GridDropDownColorUICellModel = "GridDropDownColorUICellModel";
        /// <summary>
        /// GridDropDownColorUICellModelDesc
        /// </summary>
        public const string GridDropDownColorUICellModelDesc = "GridDropDownColorUICellModelDesc";
        /// <summary>
        /// GridDropDownGridCellModel
        /// </summary>
        public const string GridDropDownGridCellModel = "GridDropDownGridCellModel";
        /// <summary>
        /// GridDropDownGridCellModelDesc
        /// </summary>
        public const string GridDropDownGridCellModelDesc = "GridDropDownGridCellModelDesc";
        /// <summary>
        /// GridDropDownHeaderCellModel
        /// </summary>
        public const string GridDropDownHeaderCellModel = "GridDropDownHeaderCellModel";
        /// <summary>
        /// GridDropDownHeaderCellModelDesc
        /// </summary>
        public const string GridDropDownHeaderCellModelDesc = "GridDropDownHeaderCellModelDesc";
        /// <summary>
        /// GridDropDownMonthCalendarCellModel
        /// </summary>
        public const string GridDropDownMonthCalendarCellModel = 
            "GridDropDownMonthCalendarCellModel";
        /// <summary>
        /// GridDropDownMonthCalendarCellModelDesc
        /// </summary>
        public const string GridDropDownMonthCalendarCellModelDesc = "GridDropDownMonthCalendarCellModelDesc";
        /// <summary>
        /// GridDropDownRichTextBoxCellModel
        /// </summary>
        public const string GridDropDownRichTextBoxCellModel = "GridDropDownRichTextBoxCellModel";
        /// <summary>
        /// GridDropDownRichTextBoxCellModelDesc
        /// </summary>
        public const string GridDropDownRichTextBoxCellModelDesc = "GridDropDownRichTextBoxCellModelDesc";
        /// <summary>
        /// GridHeaderCellModel
        /// </summary>
        public const string GridHeaderCellModel = "GridHeaderCellModel";
        /// <summary>
        /// GridHeaderCellModelDesc
        /// </summary>
        public const string GridHeaderCellModelDesc = "GridHeaderCellModelDesc";
        /// <summary>
        /// GridNumericUpDownCellModel
        /// </summary>
        public const string GridNumericUpDownCellModel = "GridNumericUpDownCellModel";
        /// <summary>
        /// GridNumericUpDownCellModelDesc
        /// </summary>
        public const string GridNumericUpDownCellModelDesc = "GridNumericUpDownCellModelDesc";
        /// <summary>
        /// GridPushButtonCellModel
        /// </summary>
        public const string GridPushButtonCellModel = "GridPushButtonCellModel";
        /// <summary>
        /// GridPushButtonCellModelDesc
        /// </summary>
        public const string GridPushButtonCellModelDesc = "GridPushButtonCellModelDesc";
        /// <summary>
        /// GridStaticCellModel
        /// </summary>
        public const string GridStaticCellModel = "GridStaticCellModel";
        /// <summary>
        /// GridStaticCellModelDesc
        /// </summary>
        public const string GridStaticCellModelDesc = "GridStaticCellModelDesc";
        /// <summary>
        /// GridStyleCategoryAlignment
        /// </summary>
        public const string GridStyleCategoryAlignment = "GridStyleCategoryAlignment";
        /// <summary>
        /// GridStyleCategoryAppearance
        /// </summary>
        public const string GridStyleCategoryAppearance = "GridStyleCategoryAppearance";
        /// <summary>
        /// GridStyleCategoryBehavior
        /// </summary>
        public const string GridStyleCategoryBehavior = "GridStyleCategoryBehavior";
        /// <summary>
        /// GridStyleCategoryDisplay
        /// </summary>
        public const string GridStyleCategoryDisplay = "GridStyleCategoryDisplay";
        /// <summary>
        /// GridStyleCategoryGraphics
        /// </summary>
        public const string GridStyleCategoryGraphics = "GridStyleCategoryGraphics";
        /// <summary>
        /// GridStyleCategoryStyle
        /// </summary>
        public const string GridStyleCategoryStyle = "GridStyleCategoryStyle";
        /// <summary>
        /// GridStyleCategoryUser
        /// </summary>
        public const string GridStyleCategoryUser = "GridStyleCategoryUser";
        /// <summary>
        /// GridStyleCategoryValue
        /// </summary>
        public const string GridStyleCategoryValue = "GridStyleCategoryValue";
        /// <summary>
        /// GridTextBoxCellModel
        /// </summary>
        public const string GridTextBoxCellModel = "GridTextBoxCellModel";
        /// <summary>
        /// GridTextBoxCellModelDesc
        /// </summary>
        public const string GridTextBoxCellModelDesc = "GridTextBoxCellModelDesc";
        /// <summary>
        /// InvalidOperation_GridCellNotInitialized
        /// </summary>
        public const string InvalidOperation_GridCellNotInitialized =
"InvalidOperation_GridCellNotInitialized";
        /// <summary>
        /// ModName
        /// </summary>
        public const string ModName = "ModName";
        /// <summary>
        /// NumberFormat_C
        /// </summary>
        public const string NumberFormat_C = "NumberFormat_C";
        /// <summary>
        /// NumberFormat_D
        /// </summary>
        public const string NumberFormat_D = "NumberFormat_D";
        /// <summary>
        /// NumberFormat_E
        /// </summary>
        public const string NumberFormat_E = "NumberFormat_E";
        /// <summary>
        /// NumberFormat_F
        /// </summary>
        public const string NumberFormat_F = "NumberFormat_F";
        /// <summary>
        /// NumberFormat_G
        /// </summary>
        public const string NumberFormat_G = "NumberFormat_G";
        /// <summary>
        /// NumberFormat_N
        /// </summary>
        public const string NumberFormat_N = "NumberFormat_N";
        /// <summary>
        /// NumberFormat_P
        /// </summary>
        public const string NumberFormat_P = "NumberFormat_P";
        /// <summary>
        /// NumberFormat_R
        /// </summary>
        public const string NumberFormat_R = "NumberFormat_R";
        /// <summary>
        /// NumberFormat_X
        /// </summary>
        public const string NumberFormat_X = "NumberFormat_X";
        /// <summary>
        /// StyleCategoryAppearance
        /// </summary>
        public const string StyleCategoryAppearance = "StyleCategoryAppearance";
        /// <summary>
        /// StyleCategoryBehavior
        /// </summary>
        public const string StyleCategoryBehavior = "StyleCategoryBehavior";
        /// <summary>
        /// StyleCategoryStyle
        /// </summary>
        public const string StyleCategoryStyle = "StyleCategoryStyle";
        /// <summary>
        /// StyleCategoryValue
        /// </summary>
        public const string StyleCategoryValue = "StyleCategoryValue";
        /// <summary>
        /// Test
        /// </summary>
        public const string Test = "Test";
        /// <summary>
        /// TextParseFailedFormat
        /// </summary>
        public const string TextParseFailedFormat = "TextParseFailedFormat";
        /// <summary>
        /// TopLeftBottomRight
        /// </summary>
        public const string TopLeftBottomRight = "TopLeftBottomRight";
        /// <summary>
        /// Cannotchangepartofamergedcell
        /// </summary>
        public const string Cannotchangepartofamergedcell = "Cannotchangepartofamergedcell";
        /// <summary>
        /// SpecifiedTextWasNotFound
        /// </summary>
        public const string SpecifiedTextWasNotFound = "SpecifiedTextWasNotFound";
        /// <summary>
        /// SearchResults
        /// </summary>
        public const string SearchResults = "SearchResults";
        /// <summary>
        /// FindReachedTheStartingPointOfTheSearch
        /// </summary>
        public const string FindReachedTheStartingPointOfTheSearch = "FindReachedTheStartingPointOfTheSearch";
        /// <summary>
        /// GridFindAndReplaceReplaceAll
        /// </summary>
        public const string GridFindAndReplaceReplaceAll="GridFindAndReplaceReplaceAll";
        /// <summary>
        /// GridFindAndReplaceReplaceWith
        /// </summary>
        public const string GridFindAndReplaceReplaceWith = "GridFindAndReplaceReplaceWith";
        /// <summary>
        /// GridFindAndReplaceSearchUp
        /// </summary>
        public const string GridFindAndReplaceSearchUp = "GridFindAndReplaceSearchUp";
        /// <summary>
        /// GridFindAndReplaceMatchCase
        /// </summary>
        public const string GridFindAndReplaceMatchCase = "GridFindAndReplaceMatchCase";
        /// <summary>
        /// GridFindAndReplaceFindWhat
        /// </summary>
        public const string GridFindAndReplaceFindWhat = "GridFindAndReplaceFindWhat";
        /// <summary>
        /// GridFindAndReplaceMatchWholeCell
        /// </summary>
        public const string GridFindAndReplaceMatchWholeCell="GridFindAndReplaceMatchWholeCell";
        /// <summary>
        /// Close
        /// </summary>
        public const string Close = "Close";
        /// <summary>
        /// GridFindAndReplace
        /// </summary>
        public const string GridFindAndReplace="GridFindAndReplace";
        /// <summary>
        /// GridFindAndReplaceFindNext
        /// </summary>
        public const string GridFindAndReplaceFindNext = "GridFindAndReplaceFindNext";
        /// <summary>
        /// DoYouWantToReplaceTheContentsOfTheDestinationCellsIfAny
        /// </summary>
        public const string DoYouWantToReplaceTheContentsOfTheDestinationCellsIfAny = 
            "DoYouWantToReplaceTheContentsOfTheDestinationCellsIfAny";
        /// <summary>
        /// Request
        /// </summary>
        public const string Request="Request";
        /// <summary>
        /// UnabletoLoadImage
        /// </summary>
        public const string UnabletoLoadImage = "UnabletoLoadImage";
        /// <summary>
        /// LoadFailed
        /// </summary>
        public const string LoadFailed = "LoadFailed";
        /// <summary>
        /// AnErrorOccurredAttemptingToPreviewtheFiletoprint
        /// </summary>
        public const string AnErrorOccurredAttemptingToPreviewtheFiletoprint = "AnErrorOccurredAttemptingToPreviewtheFiletoprint";
        /// <summary>
        /// AnErrorOccurred
        /// </summary>
        public const string AnErrorOccurred = "AnErrorOccurred";
        /// <summary>
        /// FileSaved
        /// </summary>
        public const string FileSaved = "FileSaved";
        /// <summary>
        /// UnabletoLoadtheSavedTemplate
        /// </summary>
        public const string UnabletoLoadtheSavedTemplate = "UnabletoLoadtheSavedTemplate";
        /// <summary>
        /// LoadFailure
        /// </summary>
        public const string LoadFailure = "LoadFailure";
        #endregion
    }
} // end of namespace Syncfusion.Windows.Forms.Grid
