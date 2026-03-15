#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

using System;
using System.ComponentModel;
using System.Resources;
using System.Globalization;
using System.Diagnostics;
using System.Collections;
using System.Threading;
using Syncfusion.Windows.Forms;

namespace Syncfusion.GridHelperClasses
{
    /// <summary>
    ///    SR provides localized access to string resources specific 
    ///    from the assembly manifest Syncfusion.GridHelperClasses.Windows.SR.resources
    /// </summary>
    sealed class SR
    {
        #region Fields
        private ResourceManager resources;
        private static SR loader = null;
        #endregion

        #region Strings
        internal const string StartsWith = "StartsWith";
        internal const string EndsWith = "EndsWith";
        internal const string Equal = "Equals";
        internal const string NotEquals = "NotEquals";
        internal const string LessThan = "LessThan";
        internal const string LessThanOrEqualTo = "LessThanOrEqualTo";
        internal const string GreaterThan = "GreaterThan";
        internal const string GreaterThanOrEqualTo = "GreaterThanOrEqualTo";
        internal const string Like = "Like";
        internal const string Match = "Match";
        internal const string ExpressionMATCH = "ExpressionMATCH";
        internal const string All = "All";
        internal const string Custom = "Custom";
        internal const string Empty = "Empty";
        internal const string None = "None";
        internal const string FieldDialogBox = "FieldDialogBox";
        internal const string FieldTreeDialogBox = "FieldTreeDialogBox";
        internal const string SelectAll = "SelectAll";
        internal const string SortAtoZ = "SortAtoZ";
        internal const string SortZtoA = "SortZtoA";
        internal const string SortSmallesttoLargest = "SortSmallesttoLargest";
        internal const string SortLargesttoSmallest = "SortLargesttoSmallest";
        internal const string SortOldesttoNewest = "SortOldesttoNewest";
        internal const string SortNewesttoOldest = "SortNewesttoOldest";
        internal const string TextFilters="TextFilters";
        internal const string DateTimeFilters = "DateTimeFilters";
        internal const string NumberFilters = "NumberFilters";
        internal const string ClearFilterFrom = "ClearFilterFrom";
        internal const string Office2007FilterOK = "Office2007FilterOK";
        internal const string Office2007FilterCancel = "Office2007FilterCancel";
        internal const string ShowRowsWhere = "ShowRowsWhere";
        internal const string CustomAutoFilter = "CustomAutoFilter";        
        internal const string CustomAutoFilterEqual = "CustomAutoFilterEqual";
        internal const string CustomAutoFilterNotequal = "CustomAutoFilterNotequal";
        internal const string CustomAutoFilterGreaterthan = "CustomAutoFilterGreaterthan";
        internal const string CustomAutoFilterGreaterthanOrEqual = "CustomAutoFilterGreaterthanOrEqual";
        internal const string CustomAutoFilterLessthan = "CustomAutoFilterLessthan";
        internal const string CustomAutoFilterlessthanOrEqual = "CustomAutoFilterlessthanOrEqual";
        internal const string CustomAutoFilterLike = "CustomAutoFilterLike";
        internal const string CustomAutoFilterMatch = "CustomAutoFilterMatch";
        internal const string CustomAutoFilterBeginsWith = "CustomAutoFilterBeginsWith";
        internal const string CustomAutoFilterEndsWith = "CustomAutoFilterEndsWith";
        internal const string CustomAutoFilterCancel = "CustomAutoFilterCancel";
        internal const string CustomAutoFilterOK = "CustomAutoFilterOK";
        internal const string CustomAutoFilterContains = "CustomAutoFilterContains";
        internal const string Office2007FilterEquals = "Office2007FilterEquals";
        internal const string Office2007FilterNotEquals = "Office2007FilterNotEquals";
        internal const string Office2007FilterBeginswith = "Office2007FilterBeginswith";
        internal const string Office2007FilterEndswith = "Office2007FilterEndswith";
        internal const string Office2007FilterContains = "Office2007FilterContains";
        internal const string Office2007FilterCustomFilter = "Office2007FilterCustomFilter";
        internal const string Office2007FilterCustomFilteror = "Office2007FilterCustomFilteror";
        internal const string Office2007FilterCustomFilterand = "Office2007FilterCustomFilterand";

        internal const string Office2007FilterDateEquals = "Office2007FilterDateEquals";
        internal const string Office2007Filterbefore = "Office2007Filterbefore";
        internal const string Office2007Filterafter = "Office2007Filterafter";
        internal const string Office2007Filterbetween = "Office2007Filterbetween";
        internal const string Office2007Filtertomorrow = "Office2007Filtertomorrow";
        internal const string Office2007Filtertoday = "Office2007Filtertoday";
        internal const string Office2007Filteryesterday = "Office2007Filteryesterday";
        internal const string Office2007FilternextWeek = "Office2007FilternextWeek";
        internal const string Office2007FilterthisWeek = "Office2007FilterthisWeek";
        internal const string Office2007FilterlastWeek = "Office2007FilterlastWeek";
        internal const string Office2007FilternextMonth = "Office2007FilternextMonth";
        internal const string Office2007FilterthisMonth = "Office2007FilterthisMonth";
        internal const string Office2007FilterlastMonth = "Office2007FilterlastMonth";
        internal const string Office2007FilternextQuarter = "Office2007FilternextQuarter";
        internal const string Office2007FilterthisQuarter = "Office2007FilterthisQuarter";
        internal const string Office2007FilterlastQuarter = "Office2007FilterlastQuarter";
        internal const string Office2007FilternextYear = "Office2007FilternextYear";
        internal const string Office2007FilterthisYear = "Office2007FilterthisYear";
        internal const string Office2007FilterlastYear = "Office2007FilterlastYear";
        internal const string Office2007FilteryearToDate = "Office2007FilteryearToDate";
        internal const string Office2007FilterallDatesInThePeriod = "Office2007FilterallDatesInThePeriod";
        internal const string Office2007FiltercustomDateTimeFilter = "Office2007FiltercustomDateTimeFilter";

        internal const string  Office2007Filterquarter1 = "Office2007Filterquarter1";
        internal const string Office2007Filterquarter2 = "Office2007Filterquarter2";
        internal const string  Office2007Filterquarter3 = "Office2007Filterquarter3";
        internal const string  Office2007Filterquarter4 = "Office2007Filterquarter4";
        internal const string  Office2007Filterjanuary = "Office2007Filterjanuary";
        internal const string  Office2007Filterfebruary = "Office2007Filterfebruary";
        internal const string  Office2007Filtermarch = "Office2007Filtermarch";
        internal const string  Office2007Filterapril = "Office2007Filterapril";
        internal const string  Office2007Filtermay = "Office2007Filtermay";
        internal const string  Office2007Filterjune = "Office2007Filterjune";
        internal const string  Office2007Filterjuly = "Office2007Filterjuly";
        internal const string  Office2007Filteraugust = "Office2007Filteraugust";
        internal const string  Office2007Filterseptember = "Office2007Filterseptember";
        internal const string  Office2007Filteroctober = "Office2007Filteroctober";
        internal const string  Office2007Filternovember = "Office2007Filternovember";
        internal const string  Office2007Filterdecember = "Office2007Filterdecember";

        internal const string Office2007FilternumberEqual = "Office2007FilternumberEqual";
        internal const string Office2007FilternumberNotEqual = "Office2007FilternumberNotEqual"; 
        internal const string Office2007Filtergreater = "Office2007Filtergreater";
        internal const string Office2007FiltergreaterOrEqual = "Office2007FiltergreaterOrEqual";
        internal const string Office2007Filterlessthan = "Office2007Filterlessthan";
        internal const string Office2007FilterlessOEqual = "Office2007FilterlessOEqual";
        internal const string Office2007FilternumberBetween = "Office2007FilternumberBetween";
        internal const string Office2007Filtertop10 = "Office2007Filtertop10";
        internal const string Office2007FilteraboveAverage = "Office2007FilteraboveAverage";
        internal const string Office2007FilterbelowAverage = "Office2007FilterbelowAverage";
        internal const string Office2007FilternumberCustomFilter = "Office2007FilternumberCustomFilter";

        internal const string Error = "Error";
        internal const string CustomComboboxAutoFilterEqual = "CustomComboboxAutoFilterEqual";
        internal const string CustomComboboxAutoFilterNotequal = "CustomComboboxAutoFilterNotequal";
        internal const string CustomComboboxAutoFilterGreaterthan = "CustomComboboxAutoFilterGreaterthan";
        internal const string CustomComboboxAutoFilterGreaterthanOrEqual = "CustomComboboxAutoFilterGreaterthanOrEqual";
        internal const string CustomComboboxAutoFilterLessthan = "CustomComboboxAutoFilterLessthan";
        internal const string CustomComboboxAutoFilterlessthanOrEqual = "CustomComboboxAutoFilterlessthanOrEqual";
        internal const string CustomComboboxAutoFilterLike = "CustomComboboxAutoFilterLike";
        internal const string CustomComboboxAutoFilterMatch = "CustomComboboxAutoFilterMatch";
        internal const string CustomComboboxAutoFilterBeginsWith = "CustomComboboxAutoFilterBeginsWith";
        internal const string CustomComboboxAutoFilterEndsWith = "CustomComboboxAutoFilterEndsWith";
        internal const string CustomAutoFilterDialogBox="CustomAutoFilterDialogBox";
        internal const string CustomTop10AutoFilterDialogBox = "CustomTop10AutoFilterDialogBox";
        internal const string CustomComboboxAutoFilterBeforethanOrEqual = "CustomComboboxAutoFilterBeforethanOrEqual";
        internal const string CustomComboboxAutoFilterAfterthanOrEqual = "CustomComboboxAutoFilterAfterthanOrEqual";
        internal const string CustomComboboxAutoFilterBefore = "CustomComboboxAutoFilterBefore";
        internal const string CustomComboboxAutoFilterafter = "CustomComboboxAutoFilterafter";

        internal const string FilterByColor = "FilterByColor";
        internal const string FilterByFontColor = "FilterByFontColor";
        internal const string FilterByCellColor = "FilterByCellColor";
        internal const string MoreCellColors = "MoreCellColors";
        internal const string MoreFontColors = "MoreFontColors";
        internal const string Automatic = "Automatic";
        internal const string NoFill = "NoFill";
        internal const string AvailableCellColors = "AvailableCellColors";
        internal const string AvailableFontColors = "AvailableFontColors"; 


        #endregion

        #region Constructor and Destructor
        private SR()
        {
            this.resources = new ResourceManager(this.GetType());
        }
        /// <summary>
        /// Releases all the resources.
        /// </summary>
        public static void ReleaseResources()
        {
            if (SR.loader != null)
                SR.loader.resources.ReleaseAllResources();
        }
        #endregion

        #region Implementations
        private static SR GetLoader()
        {
            lock (typeof(SR))
            {
                if (SR.loader == null)
                    SR.loader = new SR();
                return SR.loader;
            }
        }
        static bool isGetStringError = false;
        // Methods
        /// <summary>
        /// Gets the Localized string for the given string.
        /// </summary>
        /// <param name="culture">Specifies the culture for the given string</param>
        /// <param name="name">String to be localized.</param>
        /// <param name="args">Argument parameters</param>
        /// <returns></returns>
        public static string GetString(CultureInfo culture, string name, params object[] args)
        {
            if (isGetStringError)
                return name;
            SR sr = SR.GetLoader();
            string value = string.Empty;

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
        /// Gets the Localized string for the given string.
        /// </summary>
        /// <param name="name">String to be localised.</param>
        /// <returns>The localized string for the given string.</returns>
        public static string GetString(string name)
        {
            return SR.GetString(null, name);
        }

        /// <summary>
        /// Gets the Localized string for the given string.
        /// </summary>
        /// <param name="name">String to be localised.</param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string GetString(string name, params object[] args)
        {
            return SR.GetString(null, name, args);
        }

        /// <summary>
        ///  Gets the Localized string for the given string.
        /// </summary>
        /// <param name="culture">Culture into which the string has to be localized.</param>
        /// <param name="name">string to be localized.</param>
        /// <returns>The localized string</returns>
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
        /// Localizing the filter bar options
        /// </summary>
        /// <param name="name">The string to be localized.</param>
        /// <returns>The localized string.</returns>
        private string GetFilterOptions(string name)
        {            
            return name;
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
                return null;
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
                    value = ((bool)obj);
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
        /// <returns>The Byte value for the string.</returns>
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
                    value = ((byte)obj);
            }
            return value;
        }

        /// <summary>
        /// Gets the localized string for the given string.
        /// </summary>
        /// <param name="name">string that has to be localized.</param>
        /// <returns>The Byte value for the string.</returns>
        public static byte GetByte(string name)
        {
            return SR.GetByte(null, name);
        }

        /// <summary>
        /// Returns the Char value for the string. 
        /// </summary>
        /// <param name="culture">Specifies the culture into which the string has to be localized.</param>
        /// <param name="name">The string that has to be localized.</param>
        /// <returns>The Char value for the string.</returns>
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
                    value = (char)obj;
            }
            return value;
        }

        /// <summary>
        /// Gets the localized string for the given string.
        /// </summary>
        /// <param name="name">string that has to be localized.</param>
        /// <returns>The Char value for the string.</returns>
        public static char GetChar(string name)
        {
            return SR.GetChar(null, name);
        }

        /// <summary>
        /// Returns the Double value for the string. 
        /// </summary>
        /// <param name="culture">Specifies the culture into which the string has to be localized.</param>
        /// <param name="name">The string that has to be localized.</param>
        /// <returns>The Double value for the string.</returns>
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
                    value = ((double)obj);
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
                    value = ((float)obj);
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
                    value = ((int)obj);
            }
            return value;
        }

        /// <summary>
        /// Returns the Long value for the string. 
        /// </summary>
        /// <param name="name">The string that has to be localized.</param>
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
            value = ((Int64)0);
            if (sr != null)
            {
                obj = sr.resources.GetObject(name, culture);
                if (obj is System.Int64)
                    value = ((Int64)obj);
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
                    value = ((short)obj);
            }
            return value;
        }

        /// <summary>
        /// Returns the short value for the string. 
        /// </summary>
        /// <param name="name">The string that has to be localized.</param>
        /// <returns>The short value for the string.</returns>
        public static short GetShort(string name)
        {
            return SR.GetShort(null, name);
        }
        #endregion
    }

    /// <summary>
    /// Specifies the category in which the property or event will be displayed in a visual designer.
    /// </summary>
    /// <remarks>
    /// This is a localized version of CategoryAttribute. The localized string will be loaded from the 
    /// assembly manifest Syncfusion.GridHelperClasses.Windows.SR.resources
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
    /// assembly manifest Syncfusion.GridHelperClasses.Windows.SR.resources
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
    /// DynamicFilterResourceIdentifiers contains Ids specific to the Syncfusion.GridHelperClasses.Windows namespace. 
    /// </summary>
    public sealed class DynamicFilterResourceIdentifiers
    {
        #region Strings Constants
        /// <summary>
        /// Filter that satisfies the expression StartsWith
        /// </summary>
        public const string StartsWith = "StartsWith";
        /// <summary>
        /// Filter that satisfies the expression EndsWith
        /// </summary>
        public const string EndsWith = "EndsWith";
        /// <summary>
        /// Filter that satisfies the expression EndsWith
        /// </summary>
        ///  /// <summary>
        /// Filter that satisfies the expression Equals
        /// </summary>
        public const string Equal = "Equals";
        /// <summary>
        /// Filter that satisfies the expression NotEquals
        /// </summary>
        public const string NotEquals = "NotEquals";
        /// <summary>
        /// Filter that satisfies the expression LessThan
        /// </summary>
        public const string LessThan = "LessThan";
        /// <summary>
        /// Filter that satisfies the expression LessThanOrEqualTo
        /// </summary>
        public const string LessThanOrEqualTo = "LessThanOrEqualTo";
        /// <summary>
        /// Filter that satisfies the expression GreaterThan
        /// </summary>
        public const string GreaterThan = "GreaterThan";
        /// <summary>
        /// Filter that satisfies the expression GreaterThanOrEqualTo
        /// </summary>
        public const string GreaterThanOrEqualTo = "GreaterThanOrEqualTo";
        /// <summary>
        /// Filter that satisfies the expression Like
        /// </summary>
        public const string Like = "Like";
        /// <summary>
        /// Filter that satisfies the expression Match
        /// </summary>
        public const string Match = "Match";
        /// <summary>
        /// Filter that satisfies the expression ExpressionMATCH
        /// </summary>
        public const string ExpressionMATCH = "ExpressionMATCH";
        /// <summary>
        /// Displays all data in column
        /// </summary>
        public const string All = "All";
        /// <summary>
        /// Filter that satisfies the expression Custom
        /// </summary>
        public const string Custom = "Custom";
        /// <summary>
        /// Filter that satisfies the expression Empty
        /// </summary>
        public const string Empty = "Empty";
        /// <summary>
        /// when no condition is applied to filter
        /// </summary>
        public const string None = "None";
        /// <summary>
        /// Displays FieldDialogBox
        /// </summary>
        public const string FieldDialogBox = "FieldDialogBox";
        /// <summary>
        /// Displays FieldTreeDialogBox
        /// </summary>
        public const string FieldTreeDialogBox = "FieldTreeDialogBox";
        /// <summary>
        /// Selects all the data and displays them
        /// </summary>
        public const string SelectAll= "SelectAll";
        /// <summary>
        /// Sorts in Ascending order
        /// </summary>
        public const string SortAtoZ = "SortAtoZ";
        /// <summary>
        /// Sorts in Descending order
        /// </summary>
        public const string SortZtoA = "SortZtoA";
        /// <summary>
        /// Applies text filter
        /// </summary>
        public const string TextFilters = "TextFilters";
        /// <summary>
        /// Clears the filter from grid
        /// </summary>
        public const string ClearFilterFrom = "ClearFilterFrom";
        /// <summary>
        /// Applies office2007 filter
        /// </summary>
        public const string Office2007FilterOK = "Office2007FilterOK";
        /// <summary>
        /// Cancels the office2007 filter
        /// </summary>
        public const string Office2007FilterCancel = "Office2007FilterCancel";
        /// <summary>
        /// Shows the rows based on the condition.
        /// </summary>
        public const string ShowRowsWhere = "ShowRowsWhere";
        /// <summary>
        /// Applies custom Auto Filter.
        /// </summary>
        public const string CustomAutoFilter = "CustomAutoFilter";    
        /// <summary>
        /// Applies CustomAutoFilter based on Equal condition
        /// </summary>
        public const string CustomAutoFilterEqual="CustomAutoFilterEqual";
        /// <summary>
        /// Applies CustomAutoFilter based on Notequal condition
        /// </summary>
        public const string CustomAutoFilterNotequal="CustomAutoFilterNotequal";
        /// <summary>
        /// Applies CustomAutoFilter based on Greaterthan condition
        /// </summary>
        public const string CustomAutoFilterGreaterthan="CustomAutoFilterGreaterthan";
        /// <summary>
        /// Applies CustomAutoFilter based on GreaterthanOrEqual condition
        /// </summary>
        public const string CustomAutoFilterGreaterthanOrEqual="CustomAutoFilterGreaterthanOrEqual";
        /// <summary>
        /// Applies CustomAutoFilter based on Lessthan condition
        /// </summary>
        public const string CustomAutoFilterLessthan="CustomAutoFilterLessthan";
        /// <summary>
        /// Applies CustomAutoFilter based on lessthanOrEqual condition
        /// </summary>
        public const string CustomAutoFilterlessthanOrEqual="CustomAutoFilterlessthanOrEqual";
        /// <summary>
        /// Applies CustomAutoFilter based on Like condition
        /// </summary>
        public const string CustomAutoFilterLike="CustomAutoFilterLike";
        /// <summary>
        /// Applies CustomAutoFilter based on Match condition
        /// </summary>
        public const string CustomAutoFilterMatch="CustomAutoFilterMatch";
        /// <summary>
        /// Applies CustomAutoFilter based on BeginsWith condition
        /// </summary>
        public const string CustomAutoFilterBeginsWith="CustomAutoFilterBeginsWith";
        /// <summary>
        /// Applies CustomAutoFilter based on EndsWith condition
        /// </summary>
        public const string CustomAutoFilterEndsWith = "CustomAutoFilterEndsWith";
        /// <summary>
        /// Applies CustomAutoFilter based on Cancel condition
        /// </summary>
        public const string CustomAutoFilterCancel = "CustomAutoFilterCancel";
        /// <summary>
        /// Applies CustomAutoFilter based on Contains condition
        /// </summary>
        public const string CustomAutoFilterContains = "CustomAutoFilterContains";
        /// <summary>
        /// Applies CustomAutoFilter based on OK condition
        /// </summary>
        public const string CustomAutoFilterOK = "CustomAutoFilterOK";
        /// <summary>
        /// Applies Office2007Filter based on Equals condition
        /// </summary>
        public const string Office2007FilterEquals="Office2007FilterEquals";
        /// <summary>
        /// Applies Office2007Filter based on NotEquals condition
        /// </summary>
        public const string Office2007FilterNotEquals="Office2007FilterNotEquals";
        /// <summary>
        /// Applies Office2007Filter based on Beginswith condition
        /// </summary>
        public const string Office2007FilterBeginswith="Office2007FilterBeginswith";
        /// <summary>
        /// Applies Office2007Filter based on Endswith condition
        /// </summary>
        public const string Office2007FilterEndswith="Office2007FilterEndswith";
        /// <summary>
        /// Applies Office2007Filter based on Contains condition
        /// </summary>
        public const string Office2007FilterContains="Office2007FilterContains";
        /// <summary>
        /// Applies Office2007Filter based on CustomFilter condition
        /// </summary>
        public const string Office2007FilterCustomFilter = "Office2007FilterCustomFilter";
        /// <summary>
        /// Applies Office2007Filter based on CustomFilteror condition
        /// </summary>
        public const string Office2007FilterCustomFilteror = "Office2007FilterCustomFilteror";
        /// <summary>
        /// Applies Office2007Filter based on CustomFilterand condition
        /// </summary>
        public const string Office2007FilterCustomFilterand = "Office2007FilterCustomFilterand";
        /// <summary>
        /// Is showed when error occurs in filter
        /// </summary>
        public const string Error = "Error";
        /// <summary>
        /// Applies DialogBox to CustomAutoFilter
        /// </summary>
        public const string CustomAutoFilterDialogBox = "CustomAutoFilterDialogBox";
        /// <summary>
        /// Applies CustomComboboxAutoFilter based on Equal condition
        /// </summary>
        public const string CustomComboboxAutoFilterEqual = "CustomComboboxAutoFilterEqual";
        /// <summary>
        /// Applies CustomComboboxAutoFilter based on Notequal condition
        /// </summary>
        public const string CustomComboboxAutoFilterNotequal = "CustomComboboxAutoFilterNotequal";
        /// <summary>
        /// Applies CustomComboboxAutoFilter based on Greaterthan condition
        /// </summary>
        public const string CustomComboboxAutoFilterGreaterthan = "CustomComboboxAutoFilterGreaterthan";
        /// <summary>
        /// Applies CustomComboboxAutoFilter based on GreaterthanOrEqual condition
        /// </summary>
        public const string CustomComboboxAutoFilterGreaterthanOrEqual = "CustomComboboxAutoFilterGreaterthanOrEqual";
        /// <summary>
        /// Applies CustomComboboxAutoFilter based on Lessthan condition
        /// </summary>
        public const string CustomComboboxAutoFilterLessthan = "CustomComboboxAutoFilterLessthan";
        /// <summary>
        /// Applies CustomComboboxAutoFilter based on lessthanOrEqual condition
        /// </summary>
        public const string CustomComboboxAutoFilterlessthanOrEqual = "CustomComboboxAutoFilterlessthanOrEqual";
        /// <summary>
        /// Applies CustomComboboxAutoFilter based on Like condition
        /// </summary>
        public const string CustomComboboxAutoFilterLike = "CustomComboboxAutoFilterLike";
        /// <summary>
        /// Applies CustomComboboxAutoFilter based on Match condition
        /// </summary>
        public const string CustomComboboxAutoFilterMatch = "CustomComboboxAutoFilterMatch";
        /// <summary>
        /// Applies CustomComboboxAutoFilter based on BeginsWith condition
        /// </summary>
        public const string CustomComboboxAutoFilterBeginsWith = "CustomComboboxAutoFilterBeginsWith";
        /// <summary>
        /// Applies CustomComboboxAutoFilter based on EndsWith condition
        /// </summary>
        public const string CustomComboboxAutoFilterEndsWith = "CustomComboboxAutoFilterEndsWith";
       
        #endregion        
    }


} 