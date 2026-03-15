#region Copyright Syncfusion Inc. 2001 - 2014

////  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
////  Use of this code is subject to the terms of our license.
////  A copy of the current license can be obtained at any time by e-mailing
////  licensing@syncfusion.com. Re-distribution in any form is strictly
////  prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;

//// Tip:
//// To simplify rebuilding this class while working on your project
//// you should add the following External Tool in Developer Studio:
//// Title: Resgen SR.TXT
//// Command: C:\WINNT\system32\CMD.EXE
//// Arguments: /c resgen SR.txt & srgen @srgen.ini & echo Done.
//// Initial Directory: $(ProjectDir)
//// Please enable "X Use Output for Window."
//// Additionally you should add a file srgen.ini to project that 
//// contains command line arguments for your project.
//// Example: 
//// SRGen.ini
//// sr.txt namespace:Samples.SRGenSample classname:SR

namespace Syncfusion.Windows.Forms.HTMLUI.Localization
{
    /// <summary>
    ///    SR provides localized access to string resources specific 
    ///    from the assembly manifest Syncfusion.Windows.Forms.HTMLUI.Localization.SR.resources
    /// </summary>
    public sealed class SR
    {
        // Fields
        private ResourceManager resources;
        private static SR loader = null;

        // Strings 
        internal const string Testing = "Testing";

        private SR()
        {
            this.resources = new ResourceManager(this.GetType());
        }

        // Methods
        private static SR GetLoader()
        {
            lock (typeof(SR))
            {
                if (SR.loader == null)
                    SR.loader = new SR();
                return SR.loader;
            }
        }

        // Methods
        /// <summary>
        /// Gets the string.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="name">The name.</param>
        /// <param name="args">The args.</param>
        /// <returns>Returns string value</returns>
        public static string GetString(CultureInfo culture, string name, params object[] args)
        {
            SR sr = SR.GetLoader();
            string value;

            if (sr == null)
                return null;

            try
            {
                value = sr.resources.GetString(name, culture);
                if (value != null && args != null && args.Length > 0)
                    return String.Format(value, args);

                return value;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return name;
            }
        }

        /// <summary>
        /// Gets the string.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>Returns string value</returns>
        public static string GetString(string name)
        {
            return SR.GetString(null, name);
        }

        /// <summary>
        /// Gets the string.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="args">The args.</param>
        /// <returns>Returns string value</returns>
        public static string GetString(string name, params object[] args)
        {
            return SR.GetString(null, name, args);
        }

        /// <summary>
        /// Gets the string.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="name">The name.</param>
        /// <returns>Returns string value</returns>
        public static string GetString(CultureInfo culture, string name)
        {
            SR sr = SR.GetLoader();
            if (sr == null)
                return null;
            return sr.resources.GetString(name, culture);
        }

        /// <summary>
        /// Gets the object.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="name">The name.</param>
        /// <returns>Returns the object</returns>
        public static object GetObject(CultureInfo culture, string name)
        {
            SR sr = SR.GetLoader();
            if (sr == null)
                return null;
            return sr.resources.GetObject(name, culture);
        }

        /// <summary>
        /// Gets the object.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>Returns the object</returns>
        public static object GetObject(string name)
        {
            return SR.GetObject(null, name);
        }

        /// <summary>
        /// Gets the boolean.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="name">The name.</param>
        /// <returns>Returns the bool value</returns>
        public static bool GetBoolean(CultureInfo culture, string name)
        {
            bool value;
            SR sr = SR.GetLoader();
            object obj;
            value = false;
            if (sr != null)
            {
                obj = sr.resources.GetObject(name, culture);
                if (obj is bool)
                    value = (bool)obj;
            }
            return value;
        }

        /// <summary>
        /// Gets the boolean.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>Returns the bool value</returns>
        public static bool GetBoolean(string name)
        {
            return SR.GetBoolean(name);
        }

        /// <summary>
        /// Gets the byte.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="name">The name.</param>
        /// <returns>Returns the byte</returns>
        public static byte GetByte(CultureInfo culture, string name)
        {
            byte value;
            SR sr = SR.GetLoader();
            object obj;
            value = (byte)0;
            if (sr != null)
            {
                obj = sr.resources.GetObject(name, culture);
                if (obj is byte)
                    value = (byte)obj;
            }
            return value;
        }

        /// <summary>
        /// Gets the byte.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>Returns the byte</returns>
        public static byte GetByte(string name)
        {
           return SR.GetByte(null, name);
        }

        /// <summary>
        /// Gets the char.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="name">The name.</param>
        /// <returns>Returns the character</returns>
        public static char GetChar(CultureInfo culture, string name)
        {
            char value;
            SR sr = SR.GetLoader();
            object obj;
            value = (char)0;
            if (sr != null)
            {
                obj = sr.resources.GetObject(name, culture);
                if (obj is char)
                    value = (char)obj;
            }
            return value;
        }

        /// <summary>
        /// Gets the char.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>Returns the character</returns>
        public static char GetChar(string name)
        {
            return SR.GetChar(null, name);
        }

        /// <summary>
        /// Gets the double.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="name">The name.</param>
        /// <returns>Returns double value</returns>
        public static double GetDouble(CultureInfo culture, string name)
        {
            double value;
            SR sr = SR.GetLoader();
            object obj;
            value = 0.0;
            if (sr != null)
            {
                obj = sr.resources.GetObject(name, culture);
                if (obj is double)
                    value = (double)obj;
            }
            return value;
        }

        /// <summary>
        /// Gets the double.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>Returns double value</returns>
        public static double GetDouble(string name)
        {
            return SR.GetDouble(null, name);
        }

        /// <summary>
        /// Gets the float.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="name">The name.</param>
        /// <returns>Returns the float value</returns>
        public static float GetFloat(CultureInfo culture, string name)
        {
            float value;
            SR sr = SR.GetLoader();
            object obj;
            value = 0.0f;
            if (sr != null)
            {
                obj = sr.resources.GetObject(name, culture);
                if (obj is float)
                    value = (float)obj;
            }
            return value;
        }

        /// <summary>
        /// Gets the float.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>Returns the float value</returns>
        public static float GetFloat(string name)
        {
            return SR.GetFloat(null, name);
        }

        /// <summary>
        /// Gets the int.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>Returns the number as integer</returns>
        public static int GetInt(string name)
        {
            return SR.GetInt(null, name);
        }

        /// <summary>
        /// Gets the int.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="name">The name.</param>
        /// <returns>Returns the number as integer</returns>
        public static int GetInt(CultureInfo culture, string name)
        {
            int value;
            SR sr = SR.GetLoader();
            object obj;
            value = 0;
            if (sr != null)
            {
                obj = sr.resources.GetObject(name, culture);
                if (obj is int)
                    value = (int)obj;
            }
            return value;
        }

        /// <summary>
        /// Gets the long.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>Returns the number as long</returns>
        public static long GetLong(string name)
        {
            return SR.GetLong(null, name);
        }

        /// <summary>
        /// Gets the long.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="name">The name.</param>
        /// <returns>Returns the number as long</returns>
        public static long GetLong(CultureInfo culture, string name)
        {
            long value;
            SR sr = SR.GetLoader();
            object obj;
            value = (long)0;
            if (sr != null)
            {
                obj = sr.resources.GetObject(name, culture);
                if (obj is long)
                    value = (long)obj;
            }
            return value;
        }

        /// <summary>
        /// Gets the short.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="name">The name.</param>
        /// <returns>Returns the value as short</returns>
        public static short GetShort(CultureInfo culture, string name)
        {
            short value;
            SR sr = SR.GetLoader();
            object obj;
            value = (short)0;
            if (sr != null)
            {
                obj = sr.resources.GetObject(name, culture);
                if (obj is short)
                    value = (short)obj;
            }
            return value;
        }

        /// <summary>
        /// Gets the short.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>Returns the value as short</returns>
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
    /// assembly manifest Syncfusion.Windows.Forms.HTMLUI.Localization.SR.resources
    /// </remarks>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public sealed class SRCategoryAttribute : CategoryAttribute
    {
        /// <summary>
        /// SRCategoryAttribute
        /// </summary>
        /// <param name="category"></param>
        public SRCategoryAttribute(string category)
            : base(category)
        {
        }
        /// <summary>
        ///  overriding string GetLocalizedString
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
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
    /// assembly manifest Syncfusion.Windows.Forms.HTMLUI.Localization.SR.resources
    /// </remarks>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public sealed class SRDescriptionAttribute : DescriptionAttribute
    {
        private bool replaced = false;
        /// <summary>
        /// SRDescriptionAttribute
        /// </summary>
        /// <param name="description"></param>
        public SRDescriptionAttribute(string description)
            : base(description)
        {
        }

        /// <summary>
        /// Gets the Description value stored in the attribute
        /// </summary>
        /// <returns>
        /// The description stored in this attribute.
        /// </returns>
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
} // end of namespace Syncfusion.Windows.Forms.HTMLUI.Localization
