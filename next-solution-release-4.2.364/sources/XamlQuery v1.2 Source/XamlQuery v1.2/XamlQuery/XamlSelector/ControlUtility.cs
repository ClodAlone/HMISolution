//ControlUtility.cs

/*
 * XamlQuery Silverlight Library v1.2
 * http://XamlQuery.codeplex.com/
 * 
 * Author: cincoutprabu (cincoutprabu@gmail.com)
 * Copyright 2010 cincoutprabu. All rights reserved.
 * 
 * Released under Microsoft Public License.
 * http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
 *
 * Date: 2010-Sep-10 06:00 IST
 * Revision: 1
 */

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Linq;
using System.Reflection;

namespace XamlQuery.XamlSelector
{
    internal class ControlUtility
    {
        public const string AssemblyString = "System.Windows, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e";

        #region Methods

        public static Type ResolveControlType(string controlTypeName)
        {
            List<Type> allControlTypes = GetAllControlTypes();

            foreach (Type type in allControlTypes)
            {
                if (type.FullName.EndsWith("." + controlTypeName))
                {
                    return (type);
                }
            }

            return (null);
        }

        public static FieldInfo ResolveDependencyProperty(string propertyNameDescriptor)
        {
            string[] propertyNameParts = propertyNameDescriptor.Split('_');
            if (propertyNameParts.Length != 2) return (null);

            string controlTypeName = propertyNameParts[0];
            string propertyName = propertyNameParts[1];

            Type controlType = ResolveControlType(propertyNameParts[0]);
            if (controlType == null)
            {
                return (null);
            }

            FieldInfo[] allFields = controlType.GetFields(BindingFlags.Public | BindingFlags.Static);
            Common.AddToLog(allFields.Length + " public static properties in " + controlType);
            FieldInfo foundField = null;
            Common.AddToLog(string.Join("\r\n", (from field in allFields select field.Name).ToArray()));
            foreach (FieldInfo field in allFields)
            {
                if (field.Name.Equals(propertyName + "Property"))
                {
                    Common.AddToLog("found: " + field.Name);
                    foundField = field;
                    break;
                }
            }

            if (foundField != null)
            {
                return (foundField);
            }

            return (null);
        }

        public static Style GetStyleByName(string styleName)
        {
            Dictionary<string, Style> allStyles = ControlUtility.GetAllStyles(Selector.SourceControl);
            if (allStyles.ContainsKey(styleName))
            {
                return (allStyles[styleName]);
            }

            return (null);
        }

        public static bool IsStyleEqualsOrBasedOn(Style childStyle, Style parentStyle)
        {
            bool flag = false;

            Style currentStyle = childStyle;
            while (true)
            {
                if (currentStyle == null) break;
                if (currentStyle.Equals(parentStyle))
                {
                    flag = true;
                    break;
                }
                currentStyle = currentStyle.BasedOn;
            }

            return (flag);
        }

        #endregion

        #region Internal-Methods

        private static List<Type> GetAllControlTypes()
        {
            List<Type> allControlTypes = new List<Type>();

            try
            {
                Assembly assembly = Assembly.Load(AssemblyString);
                Common.AddToLog(assembly.FullName);
                Type[] allTypes = assembly.GetTypes();

                foreach (Type type in allTypes)
                {
                    if (type.IsSubclassOf(typeof(DependencyObject)))
                    {
                        allControlTypes.Add(type);
                    }
                }
            }
            catch (Exception)
            {
                Common.Error("Failed loading assembly [System.Windows].");
            }

            return (allControlTypes);
        }

        private static Dictionary<string, Style> GetAllStyles(DependencyObject control)
        {
            //find all user-controls and pages
            ControlSet allUserControls = XamlQuery.ParentsByType<UserControl>(control);
            ControlSet allPages = XamlQuery.ParentsByType<Page>(control);

            Common.AddToLog(allUserControls.Count + " user-controls");
            Common.AddToLog(allPages.Count + " pages");

            ControlSet allContainers = new ControlSet();
            allContainers.AddRange(allUserControls);
            allContainers.AddRange(allPages);

            //get styles from all user-controls and pages and their merged-dictionaries
            Dictionary<string, Style> allStyles = new Dictionary<string, Style>();

            foreach (UserControl container in allContainers)
            {
                List<ResourceDictionary> allDictionaries = GetResourceDictionaryTree(container.Resources);
                foreach (ResourceDictionary dictionary in allDictionaries)
                {
                    foreach (object key in dictionary.Keys)
                    {
                        object resource = container.Resources[key];
                        if (key is string && resource is Style)
                        {
                            allStyles.Add((string)key, (Style)resource);
                        }
                    }
                }
            }

            //get styles defined in App and its merged-dictionaries
            if (Application.Current != null)
            {
                List<ResourceDictionary> allDictionaries = GetResourceDictionaryTree(Application.Current.Resources);
                foreach (ResourceDictionary dictionary in allDictionaries)
                {
                    foreach (object key in dictionary.Keys)
                    {
                        object resource = Application.Current.Resources[key];
                        if (key is string && resource is Style)
                        {
                            allStyles.Add((string)key, (Style)resource);
                        }
                    }
                }
            }

            Common.AddToLog(allStyles.Count + " styles");
            foreach (string key in allStyles.Keys)
            {
                Common.AddToLog("found-style: " + key + ", " + allStyles[key].TargetType);
            }

            return (allStyles);
        }

        private static List<ResourceDictionary> GetResourceDictionaryTree(ResourceDictionary dictionary)
        {
            List<ResourceDictionary> allDictionaries = new List<ResourceDictionary>();
            GetResourceDictionary_Recursive(dictionary, ref allDictionaries);
            return (allDictionaries);
        }

        private static void GetResourceDictionary_Recursive(ResourceDictionary dictionary, ref List<ResourceDictionary> dictionaryList)
        {
            dictionaryList.Add(dictionary);

            foreach (ResourceDictionary child in dictionary.MergedDictionaries)
            {
                GetResourceDictionary_Recursive(child, ref dictionaryList);
            }
        }

        #endregion
    }
}
