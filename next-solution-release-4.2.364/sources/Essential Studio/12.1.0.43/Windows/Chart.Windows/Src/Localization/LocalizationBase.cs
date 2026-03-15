#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;

using System.Text;
using System.Globalization;
using System.Resources;
using System.Web;
using System.IO;
using System.Collections;

namespace Syncfusion.Windows.Forms.Chart
{
    public abstract class LocalizationBase
    {

        private string resourcePath;
        private bool defaultResource;
        private string resourceName;
        private CultureInfo currentCulture;
        private string defaultPath;
        protected virtual bool IsDefaultResource
        {
            get
            {
                return defaultResource;
            }
            set
            {
                defaultResource = value;
            }
        }
       
        private Dictionary<string, string> resourcesCollection;
        
        protected Dictionary<string, string> Resources
        {
           get
            {

                if (this.resourcesCollection.Count == 0)
                {
                    this.LoadResource(this.ResourcePath, this.ResourceName, this.CurrentCulture);
                }

                
                return this.resourcesCollection;
            }
            private set
            {
                this.resourcesCollection = value;
            }
        }

        private string ResourcePath
        {
            get
            {
                return this.resourcePath;
            }
            set
            {   
               this.resourcePath= value;
            }
        }

        private string ResourceName
        {
            get
            {
                return resourceName;
            }
            set
            {
                resourceName = value;
            }
        }

        private CultureInfo CurrentCulture
        {
            get
            {
                return currentCulture;
            }
            set
            {
                currentCulture = value;
            }
        }

        protected virtual string DefaultPath
        {
            get
            {
                return defaultPath;
            }
            set
            {
                defaultPath = value;
            }
        }

        public LocalizationBase(string path, string resourceName, CultureInfo culture)
        {
            this.Resources = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            this.ResourcePath=path;
            this.ResourceName=resourceName;
            this.CurrentCulture=culture ?? CultureInfo.CurrentUICulture ;
            

        }
        
        protected virtual string GetLocalizedString(string key)
        {
            return Resources[key];
        }

        protected virtual Dictionary<string,string> GetAllValues()
        {
            return Resources;
        }

       
        
        private void LoadResource(string resourcePath,string resourceName,CultureInfo culture)
        {
            string targetPath = resourceName +"."+ culture.ToString()+".resx";
            if (CheckFile(targetPath))
            {
                ReadFromResxFile(targetPath);
            }
            else
            {
                ReadFromDefaultSource(resourceName,culture);
            }
        }

        private bool CheckFile(string path)
        {

            string targetpath = Path.GetFullPath(path);          
            if (File.Exists(targetpath))
                return true;
            return false;
        }

        private void ReadFromResxFile(string resourcePath)
        {
            ResXResourceReader reader = new ResXResourceReader(Path.GetFullPath(resourcePath));
            IDictionaryEnumerator iterator = reader.GetEnumerator();
          
            while (iterator.MoveNext())
            {
                resourcesCollection.Add(iterator.Key.ToString(), iterator.Value.ToString());
            }
        }

       
        private void ReadFromDefaultSource(string resourceName, CultureInfo culture)
        {
            ResourceManager rm = new ResourceManager(DefaultPath + resourceName, GetType().Assembly);
            using (ResourceSet set = rm.GetResourceSet(culture ?? CultureInfo.CurrentCulture, true, true))
            {
                IDictionaryEnumerator iterator = set.GetEnumerator();
              
                while (iterator.MoveNext())
                {
                    resourcesCollection.Add(iterator.Key.ToString(), iterator.Value.ToString());
                }
            }
            IsDefaultResource = true;
        }
    }
    [Serializable]
    public sealed class ChartLocalizationStrings : LocalizationBase
    {
        #region localizestrings
        
        public ChartLocalizationStrings()
            : this(null)
        {

        }
        public ChartLocalizationStrings(CultureInfo culture)
            : this(null, culture)
        {

        }

        public ChartLocalizationStrings(string resourcePath, CultureInfo culture)
            : base(resourcePath, "ChartControl", culture)
        {
            base.DefaultPath = "Syncfusion.Windows.Forms.Chart.Resources.";
        }

       
        public bool IsDefault
        {
            get
            {
                return this.IsDefaultResource;
            }

        }
        public string Series
        {
            get
            {
                return this.GetLocalizedString("Series");
            }
        }

        public string AllowAlignment
        {
            get
            {
                return this.GetLocalizedString("AllowAlignment");
            }
        }

        public string Area
        {
            get
            {
                return this.GetLocalizedString("Area");
            }
        }

        public string Zooming
        {
            get
            {
                return this.GetLocalizedString("Zooming");
            }
        }

        public string EnableXZooming
        {
            get
            {
                return this.GetLocalizedString("EnableXZooming");
            }
        }
        public string EnableYZooming
        {
            get
            {
                return this.GetLocalizedString("EnableYZooming");
            }
        }

        public string ResetZoom
        {
            get
            {
                return this.GetLocalizedString("ResetZoom");
            }
        }
        public string AutoHighlight
        {
            get
            {
                return this.GetLocalizedString("AutoHighlight");
            }
        }

        public string Mode
        {
            get
            {
                return this.GetLocalizedString("Mode");
            }
        }

        public string TwoD
        {
            get
            {
                return this.GetLocalizedString("TwoD");
            }
        }

        public string ThreeD
        {
            get
            {
                return this.GetLocalizedString("ThreeD");
            }
        }

        public string Real3D
        {
            get
            {
                return this.GetLocalizedString("Real3D");
            }
        }
        public string EditStyle
        {
            get
            {
                return this.GetLocalizedString("EditStyle");
            }
        }
        public string Types
        {
            get
            {
                return this.GetLocalizedString("Types");
            }
        }
        public string Palettes
        {
            get
            {
                return this.GetLocalizedString("Palettes");
            }
        }

        public string EditPalette
        {
            get
            {
                return this.GetLocalizedString("EditPalette");
            }
        }
   
      
        public string UnknownAppearanceStyleException
        {
            get
            {
                return this.GetLocalizedString("UnknownAppearanceStyleException");
            }
        }
        public string ChartNullException
        {
            get
            {
                return this.GetLocalizedString("ChartNullException");
            }
        }
        #endregion

    }
}
