#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Windows;
using System.Resources;
using System.Reflection;
using System.Globalization;
using System.Diagnostics;

namespace Syncfusion.UI.Xaml.Schedule
{
    sealed class SRSchedule
    {
        #region Constructor

        private SRSchedule()
        {
            var localizedManager = GetLocalizedResourceManager(Assembly.GetExecutingAssembly());
#if SILVERLIGHT
            resources = localizedManager ?? Syncfusion_Schedule_Silverlight.ResourceManager;
#else
            resources = localizedManager ?? Syncfusion_Schedule_WPF.ResourceManager;
#endif
        }

        #endregion

        #region Private Fields

        private readonly ResourceManager resources;
        private static SRSchedule loader;

        #endregion

        #region Methods

        private static ResourceManager GetLocalizedResourceManager(Assembly controlAssembly)
        {
            if (Application.Current != null)
            {
                try
                {
                    Assembly assembly = Application.Current.GetType().Assembly;
                    var manager = new ResourceManager(string.Format("{0}.Resources.{1}", assembly.FullName.Split(new[] { ',' })[0],
                                    controlAssembly.FullName.Split(new[] { ',' })[0]), assembly);
                    var currentUICulture = CultureInfo.CurrentUICulture;
                    if (manager.GetResourceSet(currentUICulture, true, true) != null)
                    {
                        return manager;
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }
            return null;
        }

        public static void ReleaseResources()
        {
            loader.resources.ReleaseAllResources();
        }

        private static SRSchedule GetLoader()
        {
            lock (typeof(SRSchedule))
            {
                return loader ?? (loader = new SRSchedule());
            }
        }

        public static string GetString(CultureInfo culture, string name, params object[] args)
        {
            var sr = GetLoader();
            if (sr == null)
                return null;
            try
            {
                string value = sr.resources.GetString(name, culture);
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

        public static string GetString(string name)
        {
            return GetString(null, name);
        }

        public static string GetString(string name, params object[] args)
        {
            return GetString(null, name, args);
        }

        public static string GetString(CultureInfo culture, string name)
        {
            SRSchedule sr = GetLoader();
            if (sr == null)
                return null;
            string value;
            try
            {
                value = sr.resources.GetString(name, culture);
            }
            catch
            {
#if SILVERLIGHT
                value = Syncfusion_Schedule_Silverlight.ResourceManager.GetString(name);
#else
                value = Syncfusion_Schedule_WPF.ResourceManager.GetString(name);
#endif
            }
            return value;
        }

        public static object GetObject(CultureInfo culture, string name)
        {
            SRSchedule sr = GetLoader();
            if (sr == null)
                return null;
            return sr.resources.GetObject(name, culture);
        }

        public static object GetObject(string name)
        {
            return GetObject(null, name);
        }

        public static bool GetBoolean(CultureInfo culture, string name)
        {
            SRSchedule sr = GetLoader();
            bool value = false;
            if (sr != null)
            {
                object obj = sr.resources.GetObject(name, culture);
                if (obj is Boolean)
                    value = ((bool)obj);
            }
            return value;
        }

        public static bool GetBoolean(string name)
        {
            return GetBoolean(null, name);
        }

        public static byte GetByte(CultureInfo culture, string name)
        {
            SRSchedule sr = GetLoader();
            byte value = 0;
            if (sr != null)
            {
                object obj = sr.resources.GetObject(name, culture);
                if (obj is Byte)
                    value = ((byte)obj);
            }
            return value;
        }

        public static byte GetByte(string name)
        {
            return GetByte(null, name);
        }

        public static char GetChar(CultureInfo culture, string name)
        {
            SRSchedule sr = GetLoader();
            var value = (char)0;
            if (sr != null)
            {
                object obj = sr.resources.GetObject(name, culture);
                if (obj is Char)
                    value = (char)obj;
            }
            return value;
        }

        public static char GetChar(string name)
        {
            return GetChar(null, name);
        }

        public static double GetDouble(CultureInfo culture, string name)
        {
            SRSchedule sr = GetLoader();
            double value = 0.0;
            if (sr != null)
            {
                object obj = sr.resources.GetObject(name, culture);
                if (obj is Double)
                    value = ((double)obj);
            }
            return value;
        }

        public static double GetDouble(string name)
        {
            return GetDouble(null, name);
        }

        public static float GetFloat(CultureInfo culture, string name)
        {
            SRSchedule sr = GetLoader();
            float value = 0.0f;
            if (sr != null)
            {
                object obj = sr.resources.GetObject(name, culture);
                if (obj is Single)
                    value = ((float)obj);
            }
            return value;
        }

        public static float GetFloat(string name)
        {
            return GetFloat(null, name);
        }

        public static int GetInt(string name)
        {
            return GetInt(null, name);
        }

        public static int GetInt(CultureInfo culture, string name)
        {
            SRSchedule sr = GetLoader();
            int value = 0;
            if (sr != null)
            {
                object obj = sr.resources.GetObject(name, culture);
                if (obj is Int32)
                    value = ((int)obj);
            }
            return value;
        }

        public static long GetLong(string name)
        {
            return GetLong(null, name);
        }

        public static long GetLong(CultureInfo culture, string name)
        {
            SRSchedule sr = GetLoader();
            long value = 0;
            if (sr != null)
            {
                object obj = sr.resources.GetObject(name, culture);
                if (obj is Int64)
                    value = ((Int64)obj);
            }
            return value;
        }

        public static short GetShort(CultureInfo culture, string name)
        {
            SRSchedule sr = GetLoader();
            short value = 0;
            if (sr != null)
            {
                object obj = sr.resources.GetObject(name, culture);
                if (obj is Int16)
                    value = ((short)obj);
            }
            return value;
        }

        public static short GetShort(string name)
        {
            return GetShort(null, name);
        }

        #endregion
    }
}
