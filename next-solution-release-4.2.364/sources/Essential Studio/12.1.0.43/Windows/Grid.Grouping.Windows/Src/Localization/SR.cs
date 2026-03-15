//-------------------------------------------------------------------------------------------------
// <copyright file="SR.cs" company="syncfusion">
// Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//
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

//// Tip:
//// To simplify rebuilding this class while working on your project
//// you should add the following External Tool in Developer Studio:
//// 
////     Title: Resgen SR.TXT
////     Command: C:\WINNT\system32\CMD.EXE
////     Arguments: /c resgen SR.txt & srgen @srgen.ini & echo Done.
////     Initial Directory: $(ProjectDir)
////     Please enable "X Use Output for Window."
//// 
//// Additionally you should add a file srgen.ini to project that 
//// contains command line arguments for your project.
////
//// Example: 
//// SRGen.ini
//// sr.txt namespace:Samples.SRGenSample classname:SR

namespace Syncfusion.Windows.Forms.Grid.Grouping.Localization
{
    /// <summary>
    ///    SR provides localized access to string resources specific 
    ///    from the assembly manifest Syncfusion.Windows.Forms.Grid.Grouping.Localization.SR.resources
    /// </summary>
    sealed class SR 
    {
        // Fields
        private ResourceManager resources;
        [ThreadStatic]
        private static SR loader = null;

        // Strings 
        internal const string FilterBarAll = "FilterBarAll";
        internal const string FilterBarCustom = "FilterBarCustom";
        internal const string FilterBarEmpty = "FilterBarEmpty";
        internal const string RecordNavigatorOF = "RecordNavigatorOF";
        internal const string DragColumnHeaderHereText = "DragColumnHeaderHere";
        internal const string FailedToFillDataset = "FailedToFillDataset";
        internal const string PasteEngineSchema = "PasteEngineSchema";
        internal const string Items = "Items";
        /// <summary>
        /// Constructor
        /// </summary>
        private SR()  
        {
            this.resources = new ResourceManager(this.GetType());
        }

        static bool isGetStringError = false;
       /// <summary>
       /// Loads the string from resource file or satellite Assemblies.
       /// </summary>
       /// <returns>Thhe localized string</returns>
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
        /// <returns>The localized string in the type of string.</returns>
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
                {
                    value = (char)obj;
                }
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
            value = (Int64) 0;
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
        /// <returns>The short value for the string.</returns>
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
    /// assembly manifest Syncfusion.Windows.Forms.Grid.Grouping.Localization.SR.resources
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
    /// assembly manifest Syncfusion.Windows.Forms.Grid.Grouping.Localization.SR.resources
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
    /// Determine the resource identifiers class
    /// </summary>
    public sealed class GroupingResourceIdentifiers
    {
        #region Strings Constants  
        /// <summary>
        /// 
        /// </summary>
        public const string FilterBarAll = "FilterBarAll";
        /// <summary>
        /// 
        /// </summary>
        public const string FilterBarCustom = "FilterBarCustom";
        /// <summary>
        /// 
        /// </summary>
        public const string FilterBarEmpty = "FilterBarEmpty";
        /// <summary>
        /// 
        /// </summary>
        public const string RecordNavigatorOF = "RecordNavigatorOF";
        /// <summary>
        /// 
        /// </summary>
        public const string DragColumnHeaderHereText = "DragColumnHeaderHere";
        /// <summary>
        /// 
        /// </summary>
        public const string FailedToFillDataset = "FailedToFillDataset";
        /// <summary>
        /// 
        /// </summary>
        public const string PasteEngineSchema = "PasteEngineSchema";

        #endregion
    }
} //// end of namespace Syncfusion.Windows.Forms.Grid.Grouping.Localization
