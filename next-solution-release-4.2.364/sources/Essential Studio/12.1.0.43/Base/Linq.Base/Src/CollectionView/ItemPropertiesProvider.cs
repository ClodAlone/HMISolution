#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Syncfusion.Linq;
using System.ComponentModel;
using System.Reflection;

namespace Syncfusion.Windows.Data
{
    /// <summary>
    /// Implements <see cref="IPropertyAccessProvider"/> to Get / Set value on the underlying object.
    /// </summary>
    public class ItemPropertiesProvider : IPropertyAccessProvider, IDisposable
    {
        private ICollectionViewAdv view;

        public ItemPropertiesProvider(ICollectionViewAdv view)
        {
            this.view = view;
        }

        #region IPropertyValue Members

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="propName">Name of the prop.</param>
        /// <returns></returns>
        public virtual object GetValue(object record, string propName)
        {
            var itemProperties = this.view.GetItemProperties();
            object result = itemProperties.GetValue(record, propName);
            return result;
        }

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="propName">Name of the prop.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public virtual bool SetValue(object record, string propName, object value)
        {
            var result = false;
            var itemProperties = this.view.GetItemProperties();
            var pd = itemProperties.GetPropertyDescriptor(propName);

            //For Complex property enable editing
            string[] propertyNameList = propName.Split('.');
            int complexPropertyCount = propertyNameList.Count();
            var isComplex = complexPropertyCount > 1;
            if (isComplex)
            {
                for (int iterator = 0; iterator < complexPropertyCount - 1; iterator++)
                {
                    var tempProperyDescriptor = itemProperties.Find(propertyNameList[iterator], true);
                    if (tempProperyDescriptor != null)
                    {
                        record = tempProperyDescriptor.GetValue(record);
#if SILVERLIGHT
                        itemProperties = new PropertyInfoCollection(tempProperyDescriptor.PropertyType);
#else
                        itemProperties = TypeDescriptor.GetProperties(tempProperyDescriptor.PropertyType);
#endif
                    }
                }
            }

            try
            {
                if (pd != null)
                {
                    value = NullableHelperInternal.FixDbNUllasNull(value, pd.PropertyType);
                    value = NullableHelperInternal.ChangeType(value, pd.PropertyType);
                    if ((value != null && pd.GetValue(record) != null && !(pd.GetValue(record).Equals(value)))
                        || (pd.GetValue(record) == null && value != null)
                        || (pd.GetValue(record) != null && value == null))
                        pd.SetValue(record, value);
                    result = true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

            return result;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            this.view = null;
        }

        #endregion
    }

    public class InterfacePropertiesProvider : IPropertyAccessProvider, IDisposable
    {
        private ICollectionViewAdv view;

        public InterfacePropertiesProvider(ICollectionViewAdv view)
        {
            this.view = view;
        }

        public object GetValue(object record, string propName)
        {
            PropertyInfo[] properties;
            string[] propertyNameList = propName.Split('.');
            int iterator, complexPropertyCount = propertyNameList.Count();
            object tRecord = record;
            for (iterator = 0; iterator < complexPropertyCount - 1; iterator++)
            {
                properties = tRecord.GetType().GetProperties();

                foreach (PropertyInfo pinfo in properties)
                {
                    if (pinfo.Name == propertyNameList[iterator])
                    {
                        tRecord = pinfo.GetValue(tRecord, null);
                        break;
                    }
                }
            }

            properties = tRecord.GetType().GetProperties();

            foreach (PropertyInfo pinfo in properties)
            {
                if (pinfo.Name == propertyNameList[complexPropertyCount - 1])
                {
                    var obj = pinfo.GetValue(tRecord, null);
                    return obj;
                }
            }

            return null;
        }

        public bool SetValue(object record, string propName, object value)
        {
            PropertyInfo[] properties;
            string[] propertyNameList = propName.Split('.');
            int iterator, complexPropertyCount = propertyNameList.Count();
            object tRecord = record;
            for (iterator = 0; iterator < complexPropertyCount - 1; iterator++)
            {
                properties = tRecord.GetType().GetProperties();

                foreach (PropertyInfo pinfo in properties)
                {
                    if (pinfo.Name == propertyNameList[iterator])
                    {
                        tRecord = pinfo.GetValue(tRecord, null);
                        break;
                    }
                }
            }

            properties = tRecord.GetType().GetProperties();

            foreach (PropertyInfo pinfo in properties)
            {
                if (pinfo.Name == propertyNameList[complexPropertyCount - 1])
                {
                    value = NullableHelperInternal.FixDbNUllasNull(value, pinfo.PropertyType);
                    value = NullableHelperInternal.ChangeType(value, pinfo.PropertyType);
                    if ((value != null && pinfo.GetValue(tRecord, null) != null && !(pinfo.GetValue(tRecord, null).Equals(value)))
                    || (pinfo.GetValue(tRecord, null) == null && value != null)
                    || (pinfo.GetValue(tRecord, null) != null && value == null))
                    pinfo.SetValue(tRecord, value, null);
                    return true;
                }
            }
            return false;
        }

        public void Dispose()
        {
          
        }
    }
}
