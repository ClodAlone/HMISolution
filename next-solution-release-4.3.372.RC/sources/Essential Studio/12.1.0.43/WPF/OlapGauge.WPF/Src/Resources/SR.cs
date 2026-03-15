#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Resources;
using System.Reflection;
using System.Globalization;
using System.Diagnostics;

namespace Syncfusion.Windows.Gauge.Olap.Resources
{
    
    sealed class SR
    {
        #region Private variables

        private ResourceManager resources;
        private static SR loader = null;

        #endregion

        #region Constructor

        /// <summary>
        /// Intialize the type SR
        /// </summary>
        private SR()
        {
            System.Resources.ResourceManager localizedManager = GetLocalizedResourceManager(Assembly.GetExecutingAssembly());
            if (localizedManager == null)
            {
                this.resources = Syncfusion.Windows.Gauge.Olap.Resources.Syncfusion_OlapGauge_wpf.ResourceManager;
            }
            else
            {
                this.resources = localizedManager;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns the local ResourceManager according to the input assembly
        /// </summary>
        /// <param name="controlAssembly">Assembly</param>
        /// <returns>ResourceManager</returns>
        private static System.Resources.ResourceManager GetLocalizedResourceManager(System.Reflection.Assembly controlAssembly)
        {
            if (Application.Current != null)
            {
                try
                {
                    Assembly assembly = Application.Current.GetType().Assembly;
                    System.Resources.ResourceManager manager = new System.Resources.ResourceManager(string.Format("{0}.Resources.{1}", assembly.FullName.Split(new char[] { ',' })[0],
                        controlAssembly.FullName.Split(new char[] { ',' })[0]), assembly);
                    if (manager != null)
                    {
                        var currentUICulture = CultureInfo.CurrentUICulture;
                        if (manager.GetResourceSet(currentUICulture, true, true) != null)
                        {
                            return manager;
                        }
                    }
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
            return null;
        }

        /// <summary>
        /// Free up all the resource related instances.
        /// </summary>
        public static void ReleaseResources()
        {
            SR.loader.resources.ReleaseAllResources();
        }

        /// <summary>
        /// Returns the instances of own.
        /// </summary>
        /// <returns></returns>
        public static SR GetLoader()
        {
            lock (typeof(SR))
            {
                if (SR.loader == null)
                    SR.loader = new SR();
                return SR.loader;
            }
        }

        /// <summary>
        /// Returns the localized string to the corresponding culture and name.
        /// </summary>
        /// <param name="culture">CultureInfo</param>
        /// <param name="name">string</param>
        /// <param name="args">object</param>
        /// <returns>string</returns>
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
        /// Returns the localized string according to the name.
        /// </summary>
        /// <param name="name">string</param>
        /// <returns>string</returns>
        public static string GetString(string name)
        {
            return SR.GetString(null, name);
        }

        /// <summary>
        /// Returns the localized string according the name
        /// </summary>
        /// <param name="name">string</param>
        /// <param name="args">object</param>
        /// <returns></returns>
        public static string GetString(string name, params object[] args)
        {
            return SR.GetString(null, name, args);
        }

        /// <summary>
        /// Returns the localized string
        /// </summary>
        /// <param name="culture">Culture information of the current thread</param>
        /// <param name="name">Name of the resource key</param>
        /// <returns>Localized String</returns>
        public static string GetString(CultureInfo culture, string name)
        {
            SR sr = SR.GetLoader();
            if (sr == null)
                return null;
            string value = "";
            try
            {
                value = sr.resources.GetString(name, culture);
            }
            catch
            {
                value = Syncfusion.Windows.Gauge.Olap.Resources.Syncfusion_OlapGauge_wpf.ResourceManager.GetString(name);
            }
            return value;
        }

        /// <summary>
        /// Returns the localized object
        /// </summary>
        /// <param name="culture">Culture information of the current thread</param>
        /// <param name="name">Name of the resource key</param>
        /// <returns>Localized Object</returns>
        public static object GetObject(CultureInfo culture, string name)
        {
            SR sr = SR.GetLoader();
            if (sr == null)
                return null;
            return sr.resources.GetObject(name, culture);
        }

        /// <summary>
        /// Returns the localized object
        /// </summary>
        /// <param name="name">Name of the resource key</param>
        /// <returns>Localized Object</returns>
        public static object GetObject(string name)
        {
            return SR.GetObject(null, name);
        }

        /// <summary>
        /// Returns the localized boolean
        /// </summary>
        /// <param name="culture">Culture information of the current thread</param>
        /// <param name="name">Name of the resource key</param>
        /// <returns>Localized Boolean</returns>
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
        /// Returns the localized boolean
        /// </summary>
        /// <param name="name">Name of the resource key</param>
        /// <returns>Localized Boolean</returns>
        public static bool GetBoolean(string name)
        {
            return SR.GetBoolean(name);
        }

        /// <summary>
        /// Returns the localized byte
        /// </summary>
        /// <param name="culture">Culture information of the current thread</param>
        /// <param name="name">Name of the resource key</param>
        /// <returns>Localized byte</returns>
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
        /// Returns the localized byte
        /// </summary>
        /// <param name="name">Name of the resource key</param>
        /// <returns>Localized byte</returns>
        public static byte GetByte(string name)
        {
            return SR.GetByte(null, name);
        }

        /// <summary>
        /// Returns the localized character
        /// </summary>
        /// <param name="culture">Culture information of the current thread</param>
        /// <param name="name">Name of the resource key</param>
        /// <returns>Localized character</returns>
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
        /// Returns the localized character
        /// </summary>
        /// <param name="name">Name of the resource key</param>
        /// <returns>Localized character</returns>
        public static char GetChar(string name)
        {
            return SR.GetChar(null, name);
        }

        /// <summary>
        /// Returns the localized double value
        /// </summary>
        /// <param name="culture">Culture information of the current thread</param>
        /// <param name="name">Name of the resource key</param>
        /// <returns>Localized double</returns>
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
        /// Returns the localized double value
        /// </summary>
        /// <param name="name">Name of the resource key</param>
        /// <returns>Localized double</returns>
        public static double GetDouble(string name)
        {
            return SR.GetDouble(null, name);
        }

        /// <summary>
        /// Returns the localized float value
        /// </summary>
        /// <param name="culture">Culture information of the current thread</param>
        /// <param name="name">Name of the resource key</param>
        /// <returns>Localized float</returns>
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
        /// Returns the localized float value
        /// </summary>
        /// <param name="name">Name of the resource key</param>
        /// <returns>Localized float</returns>
        public static float GetFloat(string name)
        {
            return SR.GetFloat(null, name);
        }

        /// <summary>
        /// Returns the localized int value
        /// </summary>
        /// <param name="name">Name of the resource key</param>
        /// <returns>Localized int</returns>
        public static int GetInt(string name)
        {
            return SR.GetInt(null, name);
        }


        /// <summary>
        /// Returns the localized int value
        /// </summary>
        /// <param name="culture">Culture information of the current thread</param>
        /// <param name="name">Name of the resource key</param>
        /// <returns>Localized int</returns>
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
        /// Returns the localized long value
        /// </summary>
        /// <param name="name">Name of the resource key</param>
        /// <returns>Localized long</returns>
        public static long GetLong(string name)
        {
            return SR.GetLong(null, name);
        }


        /// <summary>
        /// Returns the localized long value
        /// </summary>
        /// <param name="culture">Culture information of the current thread</param>
        /// <param name="name">Name of the resource key</param>
        /// <returns>Localized long</returns>
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
        /// Returns the localized short value
        /// </summary>
        /// <param name="culture">Culture information of the current thread</param>
        /// <param name="name">Name of the resource key</param>
        /// <returns>Localized short</returns>
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
        /// Returns the localized short value
        /// </summary>
        /// <param name="name">Name of the resource key</param>
        /// <returns>Localized short</returns>
        public static short GetShort(string name)
        {
            return SR.GetShort(null, name);
        }

        #endregion
    }
}