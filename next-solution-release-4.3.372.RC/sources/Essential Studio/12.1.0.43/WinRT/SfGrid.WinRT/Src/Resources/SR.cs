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
using System.Windows.Input;
using System.Resources;
using System.Globalization;
using System.Diagnostics;
using System.Reflection;
#if WinRT
using Windows.UI.Xaml;
using Windows.ApplicationModel.Resources;
#endif

namespace Syncfusion.UI.Xaml.Grid
{
    sealed class SR
    {
#if WinRT
        private ResourceLoader resources;
#else
        private ResourceManager resources;
#endif
        private static SR loader = null;
        
        private SR()
        {
#if WinRT
            //http://msdn.microsoft.com/en-us/library/windows/apps/xaml/Hh965329%28v=win.10%29.aspx
            ResourceLoader localizedManager = GetLocalizedResourceManager();
#else
            System.Resources.ResourceManager localizedManager = GetLocalizedResourceManager(this.GetType().Assembly);
#endif

            if (localizedManager == null)
            {
#if WinRT
                resources = new ResourceLoader("Syncfusion.SfGrid.WinRT/Syncfusion.SfGrid.WinRT.Resources");
#elif WPF
                this.resources = Syncfusion.UI.Xaml.Grid.Resources.Syncfusion_SfGrid_Wpf.ResourceManager;
#elif SILVERLIGHT
                this.resources = Syncfusion.UI.Xaml.Grid.Resources.Syncfusion_SfGrid_Silverlight.ResourceManager;
#else
                this.resources = Syncfusion.UI.Xaml.Grid.Resources.Syncfusion_SfGrid_WP.ResourceManager;
#endif
            }
            else
            {
                this.resources = localizedManager;
            }
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

#if WinRT
        private static ResourceLoader GetLocalizedResourceManager()
        {
            try
            {
                if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
                {
                    return null;
                }

                if (Application.Current != null)
                {
                    ResourceLoader manager = null;
                    manager = new ResourceLoader("Syncfusion.SfGrid.WinRT.Resources");
                    return manager;
                }
            }
            catch
            {
            }
            return null;
        }

        public static string GetString(CultureInfo culture, string name)
        {
            SR sr = SR.GetLoader();
            if (sr == null)
                return null;
            return sr.resources.GetString(name);
        }
#else

        private static System.Resources.ResourceManager GetLocalizedResourceManager(System.Reflection.Assembly controlAssembly)
        {
            if (Application.Current != null)
            {
                try
                {
                    Assembly assembly = Application.Current.GetType().Assembly;
                    System.Resources.ResourceManager manager = null; 
                    bool found = false;

                    var sampleresourcename = string.Format("{0}.Resources.{1}.resources", assembly.FullName.Split(new char[] { ',' })[0], controlAssembly.FullName.Split(new char[] { ',' })[0]);
                    foreach (var resourceName in assembly.GetManifestResourceNames())
                    {
                        if (resourceName.Equals(sampleresourcename))
                        {
                            if (resourceName.Equals(string.Format("{0}.Resources.{1}.resources", assembly.FullName.Split(new char[] { ',' })[0],
                   controlAssembly.FullName.Split(new char[] { ',' })[0])))
                            { found = true; break; }
                        }
                    }
                    if (found)
                    {
                        
                        var resourcemanagerName = string.Format("{0}.Resources.{1}", assembly.FullName.Split(new char[] { ',' })[0], controlAssembly.FullName.Split(new char[] { ',' })[0]);
                        //var resourcemanagerName = string.Format("{0}.{1}", assembly.FullName.Split(new char[] { ',' })[0], controlAssembly.FullName.Split(new char[] { ',' })[0]);
                        manager = new System.Resources.ResourceManager(resourcemanagerName, assembly);

                    }
                    else
                    {
#if WPF
                        var resourcemanagerName = string.Format("Syncfusion.UI.Xaml.Grid.Resources.{0}",controlAssembly.FullName.Split(new char[] { ',' })[0]);
#elif SILVERLIGHT
                        var resourcemanagerName = string.Format("Syncfusion.UI.Xaml.Grid.Resources.{0}",controlAssembly.FullName.Split(new char[] { ',' })[0]);
#elif WP 
                        var resourcemanagerName = string.Format("Syncfusion.UI.Xaml.Grid.Resources.{0}", controlAssembly.FullName.Split(new char[] { ',' })[0]);
#endif
                        manager = new System.Resources.ResourceManager(resourcemanagerName, controlAssembly);
                    }
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
#if WPF
                value = Syncfusion.UI.Xaml.Grid.Resources.Syncfusion_SfGrid_Wpf.ResourceManager.GetString(name);
#elif SILVERLIGHT
                value = Syncfusion.UI.Xaml.Grid.Resources.Syncfusion_SfGrid_Silverlight.ResourceManager.GetString(name);
#else 
                value = Syncfusion.UI.Xaml.Grid.Resources.Syncfusion_SfGrid_WP.ResourceManager.GetString(name);
#endif

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
#endif

    }
}

