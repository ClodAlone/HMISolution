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

namespace Syncfusion.UI.Xaml.Controls.Media
{
    sealed class SR
    {
        private ResourceLoader resources;
        private ResourceLoader assemblyResources;
        private static SR loader = null;

        private SR()
        {
            ResourceLoader localizedManager = GetLocalizedResourceManager();
            assemblyResources = new ResourceLoader("Syncfusion.SfColorPickers.WinRT/Syncfusion.SfColorPickers.WinRT.Resources");

            if (localizedManager == null)
            {
                resources = new ResourceLoader("Syncfusion.SfColorPickers.WinRT/Syncfusion.SfColorPickers.WinRT.Resources");
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
                    manager = new ResourceLoader("Syncfusion.SfColorPickers.WinRT.Resources");
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

    /// <summary>
    /// Represents a class for storing the available resources
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public class ResourceWrapper
    {
        const string R_Value = "R";
        const string G_Value = "G";
        const string B_Value = "B";
        const string Colors_Value = "Colors";
        const string Swatches_value = "Swatches";
        private const string RGB_Value = "RGB";
       
        /// <summary>
        /// Gets and sets a string for Red value
        /// </summary>
        public string R { get; set; }
        /// <summary>
        /// Gets and sets a string for Green value
        /// </summary>
        public string G { get; set; }
        /// <summary>
        /// Gets and sets a string for Blue value
        /// </summary>
        public string B { get; set; }
        /// <summary>
        /// Gets and sets a string for the colors available
        /// </summary>
        public string Colors { get; set; }
        /// <summary>
        /// Gets and sets a string for the swatches
        /// </summary>
        public string Swatches { get; set; }
        /// <summary>
        /// Gets and sets a string for RGB value
        /// </summary>
        public string RGB { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="Syncfusion.UI.Xaml.Controls.Media.ResourceWrapper"/> class.
        /// </summary>  
        /// <seealso
        /// cref="N:Syncfusion.UI.Xaml.Controls.Media">Syncfusion.UI.Xaml.Controls.Media
        /// Namespace</seealso>
        [ClassReference(IsReviewed = false)]
        public ResourceWrapper()
        {
            CultureInfo ci = CultureInfo.CurrentUICulture;
            R = SR.GetString(ci, R_Value);
            G = SR.GetString(ci, G_Value);
            B = SR.GetString(ci, B_Value);
            Colors = SR.GetString(ci, Colors_Value);
            Swatches = SR.GetString(ci, Swatches_value);
            RGB = SR.GetString(ci, RGB_Value);
        }
    }

}
