#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Windows;

////
//// ******************************* Change Namespace here ******************************
////
namespace Syncfusion.Windows.Tools.Olap
{
    sealed class SR
    {
        // Fields
        private ResourceManager resources;
        private static SR loader = null;
        
        private SR()
        {
            System.Resources.ResourceManager localizedManager = GetLocalizedResourceManager(Assembly.GetExecutingAssembly());
            if (localizedManager == null)
            {
                ////
                //// ******************************* Change Namespace here ******************************
                ////
                this.resources = Syncfusion.Windows.Tools.Olap.Resources.Syncfusion_OlapTools_WPF.ResourceManager;
                //Or
                //this.resources = new ResourceManager("FirstNameLastNameControl.Resources.FirstNameLastNameControl", typeof(FirstNameLastNameControl.Resources.FirstNameLastNameControl).Assembly);
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
                catch (Exception)
                {
                    return null;
                }
            }
            return null;
        }

        public static void ReleaseResources()
        {
            SR.loader.resources.ReleaseAllResources();
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
        
        public static string GetString(string name)
        {
            return SR.GetString(null, name);
        }
        
        public static string GetString(string name, params object[] args)
        {
            return SR.GetString(null, name, args);
        }
        
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
                ////
                //// ******************************* Change Namespace here ******************************
                ////
                value = Syncfusion.Windows.Tools.Olap.Resources.Syncfusion_OlapTools_WPF.ResourceManager.GetString(name);
            }
            return value;
        }

        public static object GetObject(CultureInfo culture, string name)
        {
            SR sr = SR.GetLoader();
            if (sr == null)
                return null;
            return sr.resources.GetObject(name, culture);
        }
        
        public static object GetObject(string name)
        {
            return SR.GetObject(null, name);
        }
        
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
        
        public static bool GetBoolean(string name)
        {
            return SR.GetBoolean(name);
        }
        
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
                    value = (char)obj;
            }
            return value;
        }
        
        public static char GetChar(string name)
        {
            return SR.GetChar(null, name);
        }
        
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
        
        public static double GetDouble(string name)
        {
            return SR.GetDouble(null, name);
        }
        
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
        
        public static float GetFloat(string name)
        {
            return SR.GetFloat(null, name);
        }
        
        public static int GetInt(string name)
        {
            return SR.GetInt(null, name);
        }
        
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
        
        public static long GetLong(string name)
        {
            return SR.GetLong(null, name);
        }
        
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
        
        public static short GetShort(string name)
        {
            return SR.GetShort(null, name);
        }
    }
}
