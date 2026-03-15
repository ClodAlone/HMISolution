#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Data.Extensions
{
    using System.ComponentModel;
    using System.Linq;

#if !SILVERLIGHT
    using System;
    using System.Collections.Generic;
    using System.Collections;
#else
    using ArrayList = System.Collections.Generic.List<object>;
#endif

    public static class PropertyDescriptorExtensions
    {
        public static object GetValue(this PropertyDescriptorCollection pdc, object record, string columnName)
        {
            if (columnName == null)
            {
                return null;
            }

            var kvp = pdc.GetPropertyDescriptor(record, columnName);
            if (kvp.Key != null && kvp.Value != null)
            {
                if (columnName.IndexOf('[') > 0)
                {
                    return kvp.Value;
                }
                else
                {
                    return kvp.Key.GetValue(kvp.Value);
                }
            }

            return null;
        }

        internal static KeyValuePair<PropertyDescriptor, Object> GetPropertyDescriptor(this PropertyDescriptorCollection pdc, object record, string columnName)
        {
            var isArrayProperty = false;
            int index = -1;
            var pd = pdc.Find(columnName, true);
            var tRecord = record;
            if (pd == null)
            {
                string[] propertyNameList = columnName.Split('.');
                int iterator, complexPropertyCount = propertyNameList.Count();
                for (iterator = 0; iterator < complexPropertyCount - 1; iterator++)
                {
                    var tempProperyDescriptor = pdc.Find(propertyNameList[iterator], true);
                    if (tempProperyDescriptor != null) 
                    {
                        tRecord = tempProperyDescriptor.GetValue(tRecord);
                        pdc = TypeDescriptor.GetProperties(tRecord);
                    }
                }

                string actualproperty = propertyNameList[iterator];
                isArrayProperty = actualproperty.IndexOf('[') != -1;
                if (isArrayProperty)
                {
                    string property = actualproperty.Substring(0, actualproperty.IndexOf('['));
                    string strindex = actualproperty.Substring(actualproperty.IndexOf('[') + 1,
                                                               actualproperty.IndexOf(']') - actualproperty.IndexOf('[') -
                                                               1);
                    index = Convert.ToInt32(strindex);
                    actualproperty = property;
                }

                pd = pdc.Find(actualproperty, true);
                //var pd = pdc.Find(propertyNameList[iterator], true);
            }
            if (pd != null)
            {
                if (isArrayProperty && index >= 0)
                {
                    var array = pd.GetValue(tRecord) as IList;
                    return new KeyValuePair<PropertyDescriptor, object>(pd, array[index]);
                }
                return new KeyValuePair<PropertyDescriptor, object>(pd, tRecord);
            }

            return new KeyValuePair<PropertyDescriptor, object>();
        }

        /// <summary>
        /// Sets the value for the corresponding object available in the PropertyDescriptorCollection
        /// </summary>
        /// <param name="pdc">ItemProperties</param>
        /// <param name="record">Record</param>
        /// <param name="value">Value</param>
        /// <param name="columnName">Mapping name of the column(Includeing complex mapping names)</param>
        public static void SetValue(this PropertyDescriptorCollection pdc, object record, object value, string columnName)
        {
            var kvp = pdc.GetPropertyDescriptor(record, columnName);
            if (kvp.Key != null && kvp.Value != null)
            {
                kvp.Key.SetValue(kvp.Value, value);
            }
        }

        /// <summary>
        /// Generate the Property Descriptor for corresponding Property it may be simple or complex property
        /// </summary>
        /// <param name="pdc"></param>
        /// <param name="columnName"></param>
        /// <param name="columnType"></param>
        /// <returns></returns>
        public static PropertyDescriptor GetPropertyDescriptor(this PropertyDescriptorCollection pdc, string columnName)
        {
            if (columnName == null)
            {
                throw new InvalidOperationException("ColumnName cannot be NULL");
            }

            var temppd = pdc.Find(columnName, false);
            if (temppd != null)
                return temppd;

            string[] propertyNameList = columnName.Split('.');
            int complexPropertyCount = propertyNameList.Count();

            bool IsDictionary = false;

            var ColName = from pName in propertyNameList where (pName.Contains('[') && pName.Contains(']')) select pName;
            if (ColName.Count() > 0)
            {
                IsDictionary = true;
            }

            if (!IsDictionary)
            {
                var isComplex = complexPropertyCount > 1;
                for (int i = 0; i < complexPropertyCount - 1; i++)
                {
                    var tempPropertyInfo = pdc.Find(propertyNameList[i], false);
                    if (tempPropertyInfo != null)
                    {
                        pdc = TypeDescriptor.GetProperties(tempPropertyInfo.PropertyType);
                    }
                }
                string finalColumnName = columnName;
                if (isComplex)
                {
                    finalColumnName = propertyNameList[propertyNameList.Length - 1];
                }

                var pd = pdc.Find(finalColumnName, false);
                return pd;
            }
            else
            {
                int iterator;
                for (iterator = 0; iterator < complexPropertyCount - 1; iterator++)
                {
                    if (propertyNameList[iterator].Contains('[') && propertyNameList[iterator].Contains(']'))
                        break;

                    var tempPropertyInfo = pdc.Find(propertyNameList[iterator], false);
                    if (tempPropertyInfo != null)
                        pdc = TypeDescriptor.GetProperties(tempPropertyInfo.PropertyType);
                }

                string CollectionProperty = propertyNameList[iterator].Substring(0, propertyNameList[iterator].IndexOf('['));
                var collPropertyInfo = pdc.Find(CollectionProperty, false);

                var props = collPropertyInfo.PropertyType.GetProperties();
                PropertyDescriptor pd = null;
                foreach (var item in props)
                {
                    var containerPdc = TypeDescriptor.GetProperties(item.PropertyType);
                    pd = containerPdc.Find(propertyNameList[complexPropertyCount - 1], false);
                    if (pd != null)
                    {
                        return pd;
                    }
                }
                return pd;

            }
        }
    }
}
