using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyControl.Localization
{
    public class LocalizablePropertyDescriptorCollection : PropertyDescriptorCollection
    {
        #region Declarations
        static Object lockCache = new Object();
        static Dictionary<string, PropertyResourceManager> mapTypeResourceManager;
        Type attributeToFind = null;
        #endregion

        #region Constructors
        public LocalizablePropertyDescriptorCollection() :
            base(null)
        {
        }

        public LocalizablePropertyDescriptorCollection(bool projectHasChildren) :
            base(null)
        {
            if (projectHasChildren)
                attributeToFind = typeof(Utilities.DisplayNameExtension);
        }

        // Summary:
        //     Initializes a new instance of the System.ComponentModel.PropertyDescriptorCollection
        //     class.
        //
        // Parameters:
        //   properties:
        //     An array of type System.ComponentModel.PropertyDescriptor that provides the
        //     properties for this collection.
        public LocalizablePropertyDescriptorCollection(PropertyDescriptor[] properties) : 
            base(null)
        {
            if (properties != null)
            { 
                foreach (var prop in properties)
                {
                    base.Add(new LocalizablePropertyDescriptor(prop, attributeToFind));
                }
            }
        }
        
        //
        // Summary:
        //     Initializes a new instance of the System.ComponentModel.PropertyDescriptorCollection
        //     class, which is optionally read-only.
        //
        // Parameters:
        //   properties:
        //     An array of type System.ComponentModel.PropertyDescriptor that provides the
        //     properties for this collection.
        //
        //   readOnly:
        //     If true, specifies that the collection cannot be modified.
        public LocalizablePropertyDescriptorCollection(PropertyDescriptor[] properties, bool readOnly) : 
            base(null, readOnly)
        {
            if (properties != null)
            {
                foreach (var prop in properties)
                {
                    base.Add(new LocalizablePropertyDescriptor(prop, attributeToFind));
                }
            }
        }
        #endregion

        #region Public Methods
        // Summary:
        //     Adds the specified System.ComponentModel.PropertyDescriptor to the collection.
        //
        // Parameters:
        //   value:
        //     The System.ComponentModel.PropertyDescriptor to add to the collection.
        //
        // Returns:
        //     The index of the System.ComponentModel.PropertyDescriptor that was added
        //     to the collection.
        //
        // Exceptions:
        //   System.NotSupportedException:
        //     The collection is read-only.
        public int Insert(PropertyDescriptor value, bool forceReadOnly = false)
        {
            return base.Add(new LocalizablePropertyDescriptor(value, attributeToFind) { ForceReadOnly = forceReadOnly });
        }

        /// <summary>
        /// Get localized string from resource file name linked to property descriptor.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetString(LocalizablePropertyDescriptor value, string name)
        {
            lock (lockCache)
            {
                if (mapTypeResourceManager == null)
                    mapTypeResourceManager = new Dictionary<string, PropertyResourceManager>();

                if (!mapTypeResourceManager.ContainsKey(value.ResourceFileName))
                    mapTypeResourceManager[value.ResourceFileName] = new PropertyResourceManager(value.ResourceFileName);

                return mapTypeResourceManager[value.ResourceFileName].GetString(name);
            }
        }
        internal static void CleanTypeResourceCache()
        {
            lock (lockCache)
            {
                mapTypeResourceManager.Clear();
            }
        }

        /// <summary>
        /// Create the resource file name. 
        /// </summary>
        internal void CreateLocalizationResourceFiles()
        {
            var mapItems = new Dictionary<string, Dictionary<string, string>>();

            foreach (LocalizablePropertyDescriptor value in this)
            {
                if (!mapItems.ContainsKey(value.ResourceFileName))
                    mapItems[value.ResourceFileName] = new Dictionary<string, string>();

                var resourceManager = new PropertyResourceManager(value.ResourceFileName);

                var key = value.BasePropertyDescriptor.Name;
                if (!String.IsNullOrEmpty(key) && !mapItems[value.ResourceFileName].ContainsKey(key))
                    mapItems[value.ResourceFileName][key] = value.DisplayName;

                key = String.Format("{0}_Help", value.BasePropertyDescriptor.Name);
                if (!mapItems[value.ResourceFileName].ContainsKey(key))
                {
                    var description = value.Description;
                    if (description == value.BasePropertyDescriptor.DisplayName &&
                        !String.IsNullOrEmpty(value.BasePropertyDescriptor.Description))
                        description = value.BasePropertyDescriptor.Description;
                    if (description == null)
                        description = String.Empty;
                    mapItems[value.ResourceFileName][key] = description;
                }

                key = String.Format("{0}_Category", value.BasePropertyDescriptor.Name);
                var category = resourceManager.GetString(key);
                if (!String.IsNullOrEmpty(category))
                    mapItems[value.ResourceFileName][key] = category;
                else
                {
                    key = value.BasePropertyDescriptor.Category;
                    if (!String.IsNullOrEmpty(key) &&
                        (key != value.Category || resourceManager.GetString(key) != null) &&
                        value.Category != Properties.Resources.DefaultCategoryDisplayName)
                    {
                        if (resourceManager.GetString(key) == null)
                            key = String.Format("{0}_Category", value.BasePropertyDescriptor.DisplayName);
                        if (!String.IsNullOrEmpty(key) && !mapItems[value.ResourceFileName].ContainsKey(key))
                            mapItems[value.ResourceFileName][key] = value.Category;
                    }
                }

                foreach (Attribute attribute in value.Attributes)
                {
                    if (attribute is Utilities.DisplayNameExtension)
                    {
                        var prevMapItems = new Dictionary<string, Dictionary<string, PropertyResourceManager>>();
                        string pExtName = value.BasePropertyDescriptor.Name + "_" + attribute.ToString();
                        string pExtDesc = pExtName + "_Help";
                        string pExtCat = pExtName + "_Category";

                        if (mapTypeResourceManager != null)
                        {
                            if (mapTypeResourceManager[value.ComponentType.ToString()].GetElementFromMapItems(pExtName, out string foundPExtName))
                                mapItems[value.ResourceFileName][pExtName] = foundPExtName;

                            if (mapTypeResourceManager[value.ComponentType.ToString()].GetElementFromMapItems(pExtDesc, out string foundPExtDesc))
                                mapItems[value.ResourceFileName][pExtDesc] = foundPExtDesc;

                            if (mapTypeResourceManager[value.ComponentType.ToString()].GetElementFromMapItems(pExtCat, out string foundPExtCat))
                                mapItems[value.ResourceFileName][pExtCat] = foundPExtCat;
                        }
                    }
                }
            }

            foreach (var key in mapItems.Keys)
            {
                var resourceManager = new PropertyResourceManager(key);
                if (mapTypeResourceManager.ContainsKey(key))
                {
                    var currentItems = mapTypeResourceManager[key].LoadFromXml();
                    foreach (var id in currentItems.Keys)
                    {
                        if (id.Contains('.') && !mapItems[key].ContainsKey(id))
                            mapItems[key].Add(id, currentItems[id]);
                    }
                }
                resourceManager.WriteToXml(mapItems[key]);
            }
        }
        #endregion


    }
}
