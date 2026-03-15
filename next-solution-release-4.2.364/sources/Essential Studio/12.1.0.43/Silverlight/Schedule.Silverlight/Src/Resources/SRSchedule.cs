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

namespace Syncfusion.Windows.Controls.Schedule.Resources
{
    sealed class SRSchedule
    {
        // Fields
        private ResourceManager resources;
        private static SRSchedule loader = null;

        private SRSchedule()
        {
            System.Resources.ResourceManager localizedManager = GetLocalizedResourceManager(Assembly.GetExecutingAssembly());
            if (localizedManager == null)
            {
#if SILVERLIGHT
                this.resources = Syncfusion.Windows.Controls.Schedule.Resources.Syncfusion_Schedule_Silverlight.ResourceManager;                
#else
                this.resources = Syncfusion.Windows.Controls.Schedule.Resources.Syncfusion_Schedule_WPF.ResourceManager;                
#endif
                //Or
                //this.resources = new ResourceManager("SyncLocalizationLibrary.Resources.SyncLocalizationLibrary", typeof(SyncLocalizationLibrary.Resources.SyncLocalizationLibrary).Assembly);
            }
            else
            {
                this.resources = localizedManager;
            }
        }

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
                    throw (ex); 
                }
            }
            return null;
        }

        public static void ReleaseResources()
        {
            SRSchedule.loader.resources.ReleaseAllResources();
        }

        // Methods
        private static SRSchedule GetLoader()
        {
            lock (typeof(SRSchedule))
            {
                if (SRSchedule.loader == null)
                    SRSchedule.loader = new SRSchedule();
                return SRSchedule.loader;
            }
        }

        public static string GetString(CultureInfo culture, string name, params object[] args)
        {
            SRSchedule sr = SRSchedule.GetLoader();
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

        public static string GetString(string name)
        {
            return SRSchedule.GetString(null, name);
        }

        public static string GetString(string name, params object[] args)
        {
            return SRSchedule.GetString(null, name, args);
        }

        public static string GetString(CultureInfo culture, string name)
        {
            SRSchedule sr = SRSchedule.GetLoader();
            if (sr == null)
                return null;
            string value = "";
            try
            {
                value = sr.resources.GetString(name, culture);
            }
            catch
            {
#if SILVERLIGHT
                value = Syncfusion.Windows.Controls.Schedule.Resources.Syncfusion_Schedule_Silverlight.ResourceManager.GetString(name);
#else
                 value = Syncfusion.Windows.Controls.Schedule.Resources.Syncfusion_Schedule_WPF.ResourceManager.GetString(name);
#endif
            }
            return value;
        }

        public static object GetObject(CultureInfo culture, string name)
        {
            SRSchedule sr = SRSchedule.GetLoader();
            if (sr == null)
                return null;
            return sr.resources.GetObject(name, culture);
        }

        public static object GetObject(string name)
        {
            return SRSchedule.GetObject(null, name);
        }

        public static bool GetBoolean(CultureInfo culture, string name)
        {
            bool value;
            SRSchedule sr = SRSchedule.GetLoader();
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

        public static bool GetBoolean(string name)
        {
            return SRSchedule.GetBoolean(name);
        }

        public static byte GetByte(CultureInfo culture, string name)
        {
            byte value;
            SRSchedule sr = SRSchedule.GetLoader();
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

        public static byte GetByte(string name)
        {
            return SRSchedule.GetByte(null, name);
        }

        public static char GetChar(CultureInfo culture, string name)
        {
            char value;
            SRSchedule sr = SRSchedule.GetLoader();
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

        public static char GetChar(string name)
        {
            return SRSchedule.GetChar(null, name);
        }

        public static double GetDouble(CultureInfo culture, string name)
        {
            double value;
            SRSchedule sr = SRSchedule.GetLoader();
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

        public static double GetDouble(string name)
        {
            return SRSchedule.GetDouble(null, name);
        }

        public static float GetFloat(CultureInfo culture, string name)
        {
            float value;
            SRSchedule sr = SRSchedule.GetLoader();
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

        public static float GetFloat(string name)
        {
            return SRSchedule.GetFloat(null, name);
        }

        public static int GetInt(string name)
        {
            return SRSchedule.GetInt(null, name);
        }

        public static int GetInt(CultureInfo culture, string name)
        {
            int value;
            SRSchedule sr = SRSchedule.GetLoader();
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

        public static long GetLong(string name)
        {
            return SRSchedule.GetLong(null, name);
        }

        public static long GetLong(CultureInfo culture, string name)
        {
            Int64 value;
            SRSchedule sr = SRSchedule.GetLoader();
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

        public static short GetShort(CultureInfo culture, string name)
        {
            short value;
            SRSchedule sr = SRSchedule.GetLoader();
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

        public static short GetShort(string name)
        {
            return SRSchedule.GetShort(null, name);
        }
    }
}
