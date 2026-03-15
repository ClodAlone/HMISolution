#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows.Input;
using System.Windows;
using System.Resources;
using System.Reflection;
using System.Globalization;
using Windows.UI.Xaml;
using Windows.ApplicationModel.Resources;

namespace Syncfusion.UI.Xaml.Controls.Input
{
    sealed class SR
    {

        private ResourceLoader resources;
        private ResourceLoader assemblyResources;
        private static SR loader = null;

        private SR()
        {
            ResourceLoader localizedManager = GetLocalizedResourceManager();
            assemblyResources = new ResourceLoader("Syncfusion.SfInput.WinRT/Syncfusion.SfInput.WinRT.Resources");

            if (localizedManager == null)
            {
                resources = new ResourceLoader("Syncfusion.SfInput.WinRT/Syncfusion.SfInput.WinRT.Resources");
            }
            else
            {
                this.resources = localizedManager;
            }
        }

        private static ResourceLoader GetLocalizedResourceManager()
        {
            try
            {
                if (Application.Current != null)
                {
                    ResourceLoader manager = null;
                    manager = new ResourceLoader("Syncfusion.SfInput.WinRT.Resources");
                    return manager;
                }
            }
            catch
            {

            }
            return null;
        }


        // Methods
        private static SR GetLoader()
        {
            lock (typeof(SR))
            {
                return SR.loader ?? (SR.loader = new SR());
            }
        }


        public static string GetString(CultureInfo culture, string name)
        {
            SR sr = SR.GetLoader();
            if (sr == null)
                return null;

            string localizedString = sr.resources.GetString(name);
            if (localizedString != string.Empty)
                return localizedString;
            else
            {
               return sr.assemblyResources.GetString(name);
            }
        }
    }

    internal class ResourceWrapper
    {
        const string Cut_Value = "Cut";
        const string Copy_Value = "Copy";
        const string Paste_Value = "Paste";
        const string Redo_Value = "Redo";
        const string Undo_Value = "Undo";
        const string SelectAll_Value = "SelectAll";
       
        public string Cut { get; set; }
        public string Copy { get; set; }
        public string Paste { get; set; }
        public string Redo { get; set; }
        public string Undo { get; set; }
        public string SelectAll { get; set; }

        public ResourceWrapper()
        {
            CultureInfo ci = CultureInfo.CurrentUICulture;
            Cut = SR.GetString(ci, Cut_Value);
            Copy = SR.GetString(ci, Copy_Value);
            Paste = SR.GetString(ci, Paste_Value);
            Redo = SR.GetString(ci, Redo_Value);
            Undo = SR.GetString(ci, Undo_Value);
            SelectAll = SR.GetString(ci, SelectAll_Value);
        }
    }

}
