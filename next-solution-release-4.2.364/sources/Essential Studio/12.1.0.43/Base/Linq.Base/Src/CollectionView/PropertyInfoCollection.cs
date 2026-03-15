#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Data
{
    using System.Reflection;
    using System.Collections.Generic;
    using System;
    using Syncfusion.Linq;
    using System.Linq;

    public class PropertyInfoCollection : Dictionary<string, PropertyInfo>
    {
        public static PropertyInfoCollection Empty
        {
            get
            {
                return new PropertyInfoCollection();
            }
        }

        PropertyInfoCollection()
        {
        }

        public PropertyInfoCollection(Type type)
        {
            if (type == null)
            {
                throw new InvalidOperationException("Type cannot be null");
            }

            this.Type = type;
            type.GetProperties().ForEach<PropertyInfo>(p => this.Add(p.Name, p));
        }

        //internal void Submit(PropertyInfo p)
        //{
        //    if (this.IsRIAType(p.Name))
        //    {
        //        this.Add(p.Name, p);
        //    }
        //}

        //internal bool IsRIAType(string name)
        //{
        //    var typeStrings = new string[]{"EntityConflict",
        //                                   "ValidationErrors",
        //                                   "HasValidationErrors",
        //                                   "EntityState",
        //                                   "HasChanges",
        //                                   "IsReadOnly",
        //                                   "EntityActions"};

        //    if (typeStrings.Contains(name))
        //    {
        //        return false;
        //    }

        //    return true;
        //}

        public Type Type
        {
            get;
            private set;
        }

        public PropertyInfo Find(string columnName, bool matchCase)
        {
            if (this.ContainsKey(columnName))
            {
                return this[columnName];
            }

            return null;
        }
    }

    public static class PropertyInfoExtensions
    {
        public static object GetValue(this PropertyInfo pInfo, object record)
        {
            return pInfo.GetValue(record, null);
        }

        public static object GetValue(this PropertyInfoCollection pdc, object record, string columnName)
        {
            var kvp = pdc.GetPropertyInfo(record, columnName);
            if (kvp.Key != null && kvp.Value != null)
            {
                return kvp.Key.GetValue(kvp.Value);
            }

            return null;
        }

        private static KeyValuePair<PropertyInfo, Object> GetPropertyInfo(this PropertyInfoCollection pdc, object record, string columnName)
        {
            string[] propertyNameList = columnName.Split('.');
            int itrator, complexPropertyCount = propertyNameList.Count();
            var tRecord = record;
            for (itrator = 0;itrator < complexPropertyCount - 1;itrator++)
            {
                var tempProperyDescriptor = pdc.Find(propertyNameList[itrator], true);
                if (tempProperyDescriptor != null)
                {
                    tRecord = tempProperyDescriptor.GetValue(tRecord);
                    if (tRecord != null)
                        pdc = new PropertyInfoCollection(tRecord.GetType());
                }
            }

            var pd = pdc.Find(propertyNameList[itrator], true);
            if (pd != null)
            {
                return new KeyValuePair<PropertyInfo, object>(pd, tRecord);
            }

            return new KeyValuePair<PropertyInfo, object>();
        }

        /// <summary>
        /// Generate the Property Descriptot for corresponding Property it may be simple or complex property
        /// </summary>
        /// <param name="pdc"></param>
        /// <param name="columnName"></param>
        /// <param name="columnType"></param>
        /// <returns></returns>
        public static PropertyInfo GetPropertyDescriptor(this PropertyInfoCollection pdc, string columnName)
        {
            if (columnName == null)
            {
                throw new InvalidOperationException("ColumnName cannot be NULL");
            }

            string[] propertyNameList = columnName.Split('.');
            int complexPropertyCount = propertyNameList.Count();
            var isComplex = complexPropertyCount > 1;
            for (int i = 0;i < complexPropertyCount - 1;i++)
            {
                var tempPropertyInfo = pdc.Find(propertyNameList[i], false);
                if (tempPropertyInfo != null)
                {
                    pdc = new PropertyInfoCollection(tempPropertyInfo.PropertyType);
                }
            }
            string finalColumnName = columnName;
            if (isComplex)
            {
                finalColumnName = propertyNameList[propertyNameList.Length - 1];
            }

            PropertyInfo pd = pdc.Find(finalColumnName, false);
            return pd;
        }

        public static void SetValue(this PropertyInfo pInfo, object record, object value)
        {
            pInfo.SetValue(record, value, null);
        }

        public static void SetValue(this PropertyInfoCollection pdc, object record, object value, string columnName)
        {
            var kvp = pdc.GetPropertyInfo(record, columnName);
            if (kvp.Key != null && kvp.Value != null)
            {
                kvp.Key.SetValue(kvp.Value, value);
            }
        }
    }
}
