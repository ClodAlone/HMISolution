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
using System.Reflection;
using System.Text;
using Syncfusion.Data.Extensions;
using Syncfusion.Data.Helper;
using System.Runtime.InteropServices;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using System.Diagnostics;
using System.Linq.Expressions;
#if WinRT
using Syncfusion.Dynamic;
using Windows.Data.Xml.Dom;
#else
using System.Diagnostics.SymbolStore;
using System.Xml;
using System.ComponentModel;
#if !WP7
using Syncfusion.Dynamic;
#endif
#endif

#if WPF && !SyncfusionFramework3_5
using Syncfusion.Dynamic;
#endif

namespace Syncfusion.Data
{
    /// <summary>
    /// Implements <see cref="IPropertyAccessProvider"/> to Get / Set value on the underlying object.
    /// </summary>
    /// 
    public class PropertyAccessor 
    {
        private Func<object, object> m_getter;
        private Func<object, object[], object> m_setter;
        public PropertyInfo PropertyInfo { get; private set; }
        public PropertyAccessor(PropertyInfo propertyInfo)
        {
            this.PropertyInfo = propertyInfo;
            this.InitializeGet(propertyInfo);
            this.InitializeSet(propertyInfo);
        }
        private void InitializeGet(PropertyInfo propertyInfo)
        {
            var instance = System.Linq.Expressions.Expression.Parameter(typeof(object), "instance");
#if WinRT
            var instanceCast=propertyInfo.GetMethod.GetRuntimeBaseDefinition().IsStatic?null:
#else
            var instanceCast = propertyInfo.GetGetMethod(true).IsStatic ? null :
#endif
            System.Linq.Expressions.Expression.Convert(instance, propertyInfo.DeclaringType);
            var propertyAccess = System.Linq.Expressions.Expression.Property(instanceCast, propertyInfo);
            var castPropertyValue = System.Linq.Expressions.Expression.Convert(propertyAccess, typeof(object));
            var lambda = System.Linq.Expressions.Expression.Lambda<Func<object, object>>(castPropertyValue, instance);
            this.m_getter = lambda.Compile();

        }
        private void InitializeSet(PropertyInfo propertyInfo)
        {
            if (propertyInfo != null)
            {
#if WinRT
                if (propertyInfo.SetMethod == null)
                    return;
                var methodinfo = propertyInfo.SetMethod.GetRuntimeBaseDefinition();
#else
            var methodinfo = propertyInfo.GetSetMethod(true);
#endif
                if (methodinfo == null)
                    return;

                var instanceParameter = Expression.Parameter(typeof(object), "instance");
                var parametersParameter = Expression.Parameter(typeof(object[]), "parameters");
                var parameterExpressions = new List<Expression>();
                var paramInfos = methodinfo.GetParameters();
                for (int i = 0; i < paramInfos.Length; i++)
                {
                    BinaryExpression valueObj = Expression.ArrayIndex(
                        parametersParameter, Expression.Constant(i));
                    UnaryExpression valueCast = Expression.Convert(
                        valueObj, paramInfos[i].ParameterType);
                    parameterExpressions.Add(valueCast);
                }

                var instanceCast = methodinfo.IsStatic ? null :
                Expression.Convert(instanceParameter, methodinfo.DeclaringType);
                var methodCall = Expression.Call(instanceCast, methodinfo, parameterExpressions);

                if (methodCall.Type == typeof(void))
                {
                    var lambda = Expression.Lambda<Action<object, object[]>>(
                            methodCall, instanceParameter, parametersParameter);

                    Action<object, object[]> execute = lambda.Compile();
                    this.m_setter = (instance, parameters) =>
                    {
                        execute(instance, parameters);
                        return null;
                    };
                }
                else
                {
                    var castMethodCall = Expression.Convert(methodCall, typeof(object));
                    var lambda = Expression.Lambda<Func<object, object[], object>>(
                        castMethodCall, instanceParameter, parametersParameter);

                    this.m_setter = lambda.Compile();
                }
            }

        }

        public object GetValue(object o)
        {
            if (this.m_getter == null)
            {
                throw new NotSupportedException("Get method is not defined for this property.");
            }
            
            return this.m_getter(o);
        }
        public void SetValue(object o, object value)
        {
            if (this.m_setter == null)
            {
                throw new NotSupportedException("Set method is not defined for this property.");
            }

            this.m_setter(o, new object[] { value });
        }
     
    }
 
       
    public class ItemPropertiesProvider : IPropertyAccessProvider, IDisposable
    {
        protected ICollectionViewAdv view;
        Dictionary<String, PropertyAccessor> itemaccessor = new Dictionary<String, PropertyAccessor>();
        
        public ItemPropertiesProvider(ICollectionViewAdv view)
        {
#if WPF
            if (!view.IsLegacyDataTable)
            {
#endif
                var itemproperties = view.GetItemProperties();

                foreach (var item in itemproperties)
                {
#if WPF
                    var name = (item as PropertyDescriptor).DisplayName;
                    var propertyinfo = (item as PropertyDescriptor).ComponentType.GetProperty(name);
#else
                 var name = item.Key;
                 var propertyinfo = item.Value;
#endif
                    if (propertyinfo != null)
                    {
                        PropertyAccessor accessor = new PropertyAccessor(propertyinfo);
                        itemaccessor.Add(name, accessor);
                    }
                }
#if WPF
            }
#endif
             this.view = view;
        }   

        #region IPropertyValue Members

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="propName">Name of the prop.</param>
        /// <returns></returns>
        /// 
        public virtual object GetValue(object record, string propName)
        {
#if WPF
            if (view.IsLegacyDataTable)
                return GetDataTableValue(record, propName);
            else
            {
#endif
                var itemproperties = view.GetItemProperties();
                if (itemaccessor.ContainsKey(propName))
                {
                    if (propName.Contains("."))
                    {
                        string[] propertyNameList = propName.Split('.');
                        int complexPropertyCount = propertyNameList.Count();
                        for (int iterator = 0; iterator < complexPropertyCount - 1; iterator++)
                        {
                            var tempProperyDescriptor = itemproperties.Find(propertyNameList[iterator], true);
                            if (tempProperyDescriptor != null)       // Fix for SD 8318
                            {
                                record = tempProperyDescriptor.GetValue(record);
#if WPF
                                itemproperties = TypeDescriptor.GetProperties(record);
#else
                            itemproperties = new PropertyInfoCollection(record.GetType());
#endif
                            }
                        }
                    }

                    return itemaccessor[propName].GetValue(record);
                }

                else
                {
                    string actualproperty = propName;
                    string[] propertyNameList = propName.Split('.');
                    int complexPropertyCount = propertyNameList.Count();
                    for (int iterator = 0; iterator < complexPropertyCount - 1; iterator++)
                    {
                        var tempProperyDescriptor = itemproperties.Find(propertyNameList[iterator], true);
                        if (tempProperyDescriptor != null)       // Fix for SD 8318
                        {
                            record = tempProperyDescriptor.GetValue(record);
#if WPF
                            itemproperties = TypeDescriptor.GetProperties(record);
#else
                        itemproperties = new PropertyInfoCollection(record.GetType());
#endif
                        }
                    }
                    actualproperty = propertyNameList[complexPropertyCount - 1];
                    var propertyinfo = record.GetType().GetProperty(actualproperty);
#if WPF
                    if (propertyinfo == null && view is CollectionViewAdv)
                    {
                        var sourceType = (view as CollectionViewAdv).SourceType;
                        if (sourceType != null && typeof(ICustomTypeDescriptor).IsAssignableFrom(sourceType))
                            return itemproperties.Find(actualproperty, true).GetValue(record);
                        return null;
                    }
#else
                    if(propertyinfo==null)
                       return null;
#endif
                    PropertyAccessor accessor = new PropertyAccessor(propertyinfo);
                    itemaccessor.Add(propName, accessor);
                    return accessor.GetValue(record);
                }
#if WPF
            }
#endif
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
#if WPF
            if (view.IsLegacyDataTable)
                return SetDataTableView(record, propName, value);
            else
            {
#endif

                var itemproperties = view.GetItemProperties();

                if (itemaccessor.ContainsKey(propName))
                {
                    if (propName.Contains("."))
                    {
                        string[] propertyNameList = propName.Split('.');
                        int complexPropertyCount = propertyNameList.Count();
                        for (int iterator = 0; iterator < complexPropertyCount - 1; iterator++)
                        {
                            var tempProperyDescriptor = itemproperties.Find(propertyNameList[iterator], true);
                            if (tempProperyDescriptor != null)       // Fix for SD 8318
                            {
                                record = tempProperyDescriptor.GetValue(record);
#if WPF
                                itemproperties = TypeDescriptor.GetProperties(record);
#else
                            itemproperties = new PropertyInfoCollection(record.GetType());
#endif
                            }
                        }
                    }
                    itemaccessor[propName].SetValue(record, value);
                    return true;
                }

                else
                {
                    string actualproperty = propName;
                    string[] propertyNameList = propName.Split('.');
                    int complexPropertyCount = propertyNameList.Count();
                    for (int iterator = 0; iterator < complexPropertyCount - 1; iterator++)
                    {
                        var tempProperyDescriptor = itemproperties.Find(propertyNameList[iterator], true);
                        if (tempProperyDescriptor != null)       // Fix for SD 8318
                        {
                            record = tempProperyDescriptor.GetValue(record);
#if WPF
                            itemproperties = TypeDescriptor.GetProperties(record);
#else
                        itemproperties = new PropertyInfoCollection(record.GetType());
#endif
                        }
                    }
                    actualproperty = propertyNameList[complexPropertyCount - 1];
                    var propertyinfo = record.GetType().GetProperty(actualproperty);
#if WPF
                    if (propertyinfo == null && view is CollectionViewAdv)
                    {
                        var sourceType = (view as CollectionViewAdv).SourceType;
                        if (sourceType != null && typeof(ICustomTypeDescriptor).IsAssignableFrom(sourceType))
                        {
                            itemproperties.Find(actualproperty, true).SetValue(record, value);
                            return true;
                        }
                        return false;
                    }
#else
                    if(propertyinfo==null)
                       return false;
#endif
                    PropertyAccessor accessor = new PropertyAccessor(propertyinfo);
                    itemaccessor.Add(propName, accessor);
                    accessor.SetValue(record, value);
                    return true;
                }
#if WPF
            }
#endif
        }

#if WPF
        private object GetDataTableValue(object record, string propertyName)
        {
            object data = null;
            var rowView = record as System.Data.DataRowView;
            if (rowView != null && rowView.Row != null && rowView.Row.RowState != System.Data.DataRowState.Detached
                && rowView.Row.RowState != System.Data.DataRowState.Deleted)
            {
                data = rowView[propertyName];
            }
            if (rowView == null)
            {
                var row = record as System.Data.DataRow;
                if (row != null && row.RowState != System.Data.DataRowState.Detached
                && row.RowState != System.Data.DataRowState.Deleted)
                {
                    data = row[propertyName];
                }
            }
            return data;
        }

        private bool SetDataTableView(object record, string propName, object value)
        {
            var rowView = record as System.Data.DataRowView;
            if (rowView != null && rowView.Row != null && rowView.Row.RowState != System.Data.DataRowState.Detached)
            {
                rowView[propName] = value;
                return true;
            }
            return false;
        }
#endif
        /// <summary>
        /// Gets the Formatted value.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="propName">Name of the prop.</param>
        /// <returns></returns>
        /// 
        public virtual object GetFormattedValue(object record, string propName)
        {
            return GetValue(record, propName,true);
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="propName">Name of the prop.</param>
        /// <param name="useValueBinding">If true,then use binding value</param>
        /// <returns></returns>
        public virtual object GetValue(object record, string propName, bool useBindingValue)
        {
            return GetValue(record, propName);
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

   
#if !WP

    public class DynamicPropertiesProvider : IPropertyAccessProvider, IDisposable
    {
        ICollectionViewAdv view;
        internal DynamicHelper dynamicHelper;

        public DynamicPropertiesProvider(ICollectionViewAdv view)
        {
            this.view = view;
            this.dynamicHelper = new DynamicHelper();
        }

        public virtual object GetValue(object record, string propName)
        {
            var result = this.dynamicHelper.GetValue(record, propName);
            return result;
        }

        public virtual bool SetValue(object record, string propName, object value)
        {
            var result = true;
            try
            {
                this.dynamicHelper.SetValue(record, propName, value);
            }
            catch
            {
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Gets the Formatted value.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="propName">Name of the prop.</param>
        /// <returns></returns>
        public virtual object GetFormattedValue(object record, string propName)
        {
            return GetValue(record, propName,true);
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="propName">Name of the prop.</param>
        /// <param name="useValueBinding">If true,then use binding value</param>
        /// <returns></returns>
        public virtual object GetValue(object record, string propName, bool useBindingValue)
        {
            return GetValue(record, propName);
        }

        public void Dispose()
        {
            this.dynamicHelper.Dispose();
        }
    }
#endif
#if !SILVERLIGHT && !SyncfusionFramework3_5 && !WP
    public class XMLAttributesProvider : IPropertyAccessProvider
    {
        private ICollectionViewAdv view;

        public XMLAttributesProvider(ICollectionViewAdv view)
        {
            this.view = view;
        }

        public virtual object GetValue(object record, string propName)
        {
            object result = null;
            var element = record as XmlElement;
            if (element != null)
            {
                result = element.GetAttribute(propName);
            }
            return result;
        }

        public virtual bool SetValue(object record, string propName, object value)
        {
            var element = record as XmlElement;
            if (element != null)
            {
                element.SetAttribute(propName, value.ToString());
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the Formatted value.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="propName">Name of the prop.</param>
        /// <returns></returns>
        /// 
        public virtual object GetFormattedValue(object record, string propName)
        {
            return GetValue(record, propName, true);
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="propName">Name of the prop.</param>
        /// <param name="useValueBinding">If true,then use binding value</param>
        /// <returns></returns>
        public virtual object GetValue(object record, string propName, bool useBindingValue)
        {
            return GetValue(record, propName);
        }
    }
#endif
}