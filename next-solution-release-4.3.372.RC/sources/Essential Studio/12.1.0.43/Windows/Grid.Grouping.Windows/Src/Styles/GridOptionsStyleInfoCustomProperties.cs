//-------------------------------------------------------------------------------------------------
// <copyright file="GridOptionsStyleInfoCustomProperties.cs" company="syncfusion">
// Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Runtime.Serialization;
using System.Collections;

using Syncfusion.ComponentModel;
using Syncfusion.Diagnostics;
using Syncfusion.Styles;
using Syncfusion.Windows.Forms;
using Syncfusion.Grouping;

namespace Syncfusion.Windows.Forms.Grid.Grouping
{
     #region GridGroupOptionsStyleInfoCustomProperties
    /// <summary>
    /// Adds design-time support for custom properties by adding empty custom property
    /// objects and later calling <see cref="GridGroupOptionsStyleInfoCustomPropertiesCollection.Add"/>,
    /// which will result in changing the <see cref="GridGroupOptionsStyleInfo"/> property for this object
    /// and forces copying all properties of this object to the style object.
    /// </summary>
    [TypeConverter(typeof(GridOptionsStyleInfoCustomPropertiesConverter))]
    public class GridGroupOptionsStyleInfoCustomProperties
    {
        //// instance data for type safe access to style information

        /// <summary>
        /// The <see cref="GridGroupOptionsStyleInfo"/> that holds and
        /// gets the data for this custom property object.
        /// </summary>
        protected internal GridGroupOptionsStyleInfo style;

        /// <summary>
        /// Initializes the <see cref="GridGroupOptionsStyleInfoCustomPropertiesCollection"/> object
        /// with a <see cref="GridGroupOptionsStyleInfo"/> that the properties of this
        /// class will belong to.
        /// </summary>
        /// <param name="style">The <see cref="GridGroupOptionsStyleInfo"/> that holds and
        /// gets the data for this custom property object.</param>
        protected GridGroupOptionsStyleInfoCustomProperties(GridGroupOptionsStyleInfo style)
        {
            this.style = style;
        }
        /// <summary>
        /// Determine the style info of custom properties.
        /// </summary>
        /// <param name="store"></param>
        [DebuggerStepThrough()]
        protected GridGroupOptionsStyleInfoCustomProperties(GridGroupOptionsStyleInfoStore store)
        {
            this.style = new GridGroupOptionsStyleInfo(store);
        }
        /// <summary>
        /// Determine the style info of custom properties.
        /// </summary>
        /// <param name="identity">Identity</param>
        [DebuggerStepThrough()]
        protected GridGroupOptionsStyleInfoCustomProperties(StyleInfoIdentityBase identity)
        {
            this.style = new GridGroupOptionsStyleInfo(identity);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="store"></param>
        [DebuggerStepThrough()]
        protected GridGroupOptionsStyleInfoCustomProperties(StyleInfoIdentityBase identity, GridGroupOptionsStyleInfoStore store)
        {
            this.style = new GridGroupOptionsStyleInfo(identity, store);
        }
        
        /// <summary>
        /// Initializes the <see cref="GridGroupOptionsStyleInfoCustomPropertiesCollection"/> object
        /// with an empty <see cref="GridGroupOptionsStyleInfo"/> object. When you later
        /// set the <see cref="GridGroupOptionsStyleInfo"/> property, the changes in this object
        /// will be copied over to the new <see cref="GridGroupOptionsStyleInfo"/> object.
        /// </summary>
        /// <remarks>
        /// The <see cref="GridGroupOptionsStyleInfo.CustomProperties"/> collection adds
        /// design-time support for custom properties by adding empty custom property
        /// objects and later calling <see cref="GridGroupOptionsStyleInfoCustomPropertiesCollection.Add"/>,
        /// which will result in changing the <see cref="GridGroupOptionsStyleInfo"/> property for this object
        /// and forces copying all properties of this object to the style object.
        /// </remarks>
        protected GridGroupOptionsStyleInfoCustomProperties()
        {
            this.style = new GridGroupOptionsStyleInfo();
        }

        /// <summary>
        /// The <see cref="GridGroupOptionsStyleInfo"/> that holds and
        /// gets the data for this custom property object. When you
        /// set the <see cref="GridGroupOptionsStyleInfo"/> property, all prior changes in this object
        /// will be copied over to the new <see cref="GridGroupOptionsStyleInfo"/> object.
        /// </summary>
        public GridGroupOptionsStyleInfo GroupOptions
        {
            get
            {
                return style;
            }

            set
            {
                if (value != style && style != null)
                {
                    value.ModifyStyle(style, Syncfusion.Styles.StyleModifyType.Override);
                }

                style = value;
                ////GroupOptions.CopyFrom(value);
            }
        }

        static StyleInfoProperty _CreateStyleInfoProperty(Type componentType, StaticData sd, Type type, string propertyName, StyleInfoPropertyOptions propertyOptions)
        {
            return sd.CreateStyleInfoProperty(type, propertyName, 0, false, componentType, propertyOptions);
        }

        /// <overload>
        /// Registers a new custom property.
        /// </overload>>
        /// <summary>
        /// Registers a new custom property.
        /// </summary>
        /// <param name="componentType">The type of your derived custom property class.</param>
        /// <param name="type">The type of the property.</param>
        /// <param name="propertyName">The name of the property. This must match a property member in your class.</param>
        /// <returns>A <see cref="StyleInfoProperty"/> object that you should use for getting and setting
        /// values.</returns>
        protected static StyleInfoProperty CreateStyleInfoProperty(Type componentType, Type type, string propertyName)
        {
            return _CreateStyleInfoProperty(componentType, GridGroupOptionsStyleInfoStore.StaticData, type, propertyName, StyleInfoPropertyOptions.All);
        }

        /// <summary>
        /// Registers a new custom property.
        /// </summary>
        /// <param name="componentType">The type of your derived custom property class.</param>
        /// <param name="propertyName">The name of the property. This must match a property member in your class.</param>
        /// <returns>A <see cref="StyleInfoProperty"/> object that you should use for getting and setting
        /// values.</returns>
        protected static StyleInfoProperty CreateStyleInfoProperty(Type componentType, string propertyName)
        {
            System.Reflection.PropertyInfo pi = componentType.GetProperty(propertyName);
            Type type = pi.PropertyType;
            return _CreateStyleInfoProperty(componentType, GridGroupOptionsStyleInfoStore.StaticData, type, propertyName, StyleInfoPropertyOptions.All);
        }

        /// <summary>
        /// Registers a new custom property.
        /// </summary>
        /// <param name="componentType">The type of your derived custom property class.</param>
        /// <param name="propertyName">The name of the property. This must match a property member in your class.</param>
        /// <param name="propertyOptions">Specifies attributes for the property.</param>
        /// <returns>A <see cref="StyleInfoProperty"/> object that you should use for getting and setting
        /// values.</returns>
        protected static StyleInfoProperty CreateStyleInfoProperty(Type componentType, string propertyName, StyleInfoPropertyOptions propertyOptions)
        {
            System.Reflection.PropertyInfo pi = componentType.GetProperty(propertyName);
            Type type = pi.PropertyType;
            return _CreateStyleInfoProperty(componentType, GridGroupOptionsStyleInfoStore.StaticData, type, propertyName, propertyOptions);
        }

        /// <summary>
        /// Registers a new custom property.
        /// </summary>
        /// <param name="componentType">The type of your derived custom property class.</param>
        /// <param name="type">The type of the property.</param>
        /// <param name="propertyName">The name of the property. This must match a property member in your class.</param>
        /// <param name="propertyOptions">Specifies attributes for the property.</param>
        /// <returns>A <see cref="StyleInfoProperty"/> object that you should use for getting and setting
        /// values.</returns>
        protected static StyleInfoProperty CreateStyleInfoProperty(Type componentType, Type type, string propertyName, StyleInfoPropertyOptions propertyOptions)
        {
            return _CreateStyleInfoProperty(componentType, GridGroupOptionsStyleInfoStore.StaticData, type, propertyName, propertyOptions);
        }

        /// <summary>
        /// Registers a <see cref="StyleInfoProperty"/> for the specified property.
        /// </summary>
        /// <param name="componentType">The type of your derived custom property class.</param>        
        /// <param name="type">The type of the property.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="maxValue">The maximal possible Int16 value for the property.</param>
        /// <param name="makeBitValue">Indicates whether this StyleInfoProperty should be registered as a member of the BitArray and not to allocate
        /// an object reference.</param>
        /// <returns>The StyleInfoProperty with information about the property.</returns>
        protected static StyleInfoProperty CreateStyleInfoProperty(Type componentType, Type type, string name, short maxValue, bool makeBitValue)
        {
            StaticData sd = GridGroupOptionsStyleInfoStore.StaticData;
            return sd.CreateStyleInfoProperty(type, name, maxValue, makeBitValue, componentType, StyleInfoPropertyOptions.All);
        }
    }

    /// <summary>
    /// Implements a collection of custom property objects that have
    /// at least one initialized value. The primary purpose of this
    /// collection is to support design-time code serialization of
    /// custom properties.
    /// </summary>
    public class GridGroupOptionsStyleInfoCustomPropertiesCollection : ICollection
    {
        StyleInfoBase styleInfo;
        Hashtable types = new Hashtable();

        /// <summary>
        /// Initializes a <see cref="GridGroupOptionsStyleInfoCustomPropertiesCollection"/> with a reference
        /// to the parent style object.
        /// </summary>
        /// <param name="styleInfo">The StyleInfoBase</param>
        internal GridGroupOptionsStyleInfoCustomPropertiesCollection(StyleInfoBase styleInfo)
        {
            this.styleInfo = styleInfo;
            ICollection sipsc = styleInfo.Store.StyleInfoProperties;
            Type styleInfoType = styleInfo.GetType();

            foreach (StyleInfoProperty sip in sipsc)
            {
                //// Check if ComponentType is a custom property types and if property is initialized.
                if (!sip.ComponentType.IsAssignableFrom(styleInfoType) && styleInfo.HasValue(sip))
                {
                    //// Only add one object even and ignore subsequent properties
                    if (!types.ContainsKey(sip.ComponentType))
                    {
                        types.Add(sip.ComponentType, Activator.CreateInstance(sip.ComponentType, new object[] { styleInfo }));
                    }
                    ////System.Diagnostics.Trace.WriteLine(sip.PropertyName + ":" + sip.ComponentType.Name + " != " + styleInfoType.Name);
                }
            }
        }

        /// <summary>
        /// Copies the initialized properties of the specified custom property
        /// to the parent style object and attaches the custom property object
        /// with the parent style object.
        /// </summary>
        /// <param name="value">A GridGroupOptionsStyleInfoCustomProperties with
        /// custom properties.</param>
        public void Add(GridGroupOptionsStyleInfoCustomProperties value)
        {
            value.GroupOptions = (GridGroupOptionsStyleInfo)this.styleInfo;
        }

        #region ICollection Members
        bool ICollection.IsSynchronized
        {
            get
            {
                return types.Values.IsSynchronized;
            }
        }

        /// <summary>
        /// The number of objects in this collection.
        /// </summary>
        public int Count
        {
            get
            {
                return types.Count;
            }
        }

        /// <summary>
        ///   <para>Copies the <see cref="GridGroupOptionsStyleInfoCustomPropertiesCollection" /> elements to a one-dimensional <see cref="System.Array" /> at the specified index.</para>
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="System.Array" /> that is the destination of the object's from instance. The <see cref="System.Array" /> must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        public void CopyTo(GridGroupOptionsStyleInfoCustomProperties[] array, int index)
        {
            this.types.Values.CopyTo(array, index);
        }
        
        void ICollection.CopyTo(Array array, int index)
        {
            CopyTo((GridGroupOptionsStyleInfoCustomProperties[])array, index);
        }

        object ICollection.SyncRoot
        {
            get
            {
                return types.Values.SyncRoot;
            }
        }
        #endregion
        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return types.Values.GetEnumerator();
        }
        #endregion
    }
    #endregion

    #region GridTableOptionsStyleInfoCustomProperties

    /// <summary>
    /// Adds design-time support for custom properties by adding empty custom property
    /// objects and later calling <see cref="GridTableOptionsStyleInfoCustomPropertiesCollection.Add"/>,
    /// which will result in changing the <see cref="GridTableOptionsStyleInfo"/> property for this object
    /// and forces copying all properties of this object to the style object.
    /// </summary>
    [TypeConverter(typeof(GridOptionsStyleInfoCustomPropertiesConverter))]
    public class GridTableOptionsStyleInfoCustomProperties
    {
        // instance data for type safe access to style information

        /// <summary>
        /// The <see cref="GridTableOptionsStyleInfo"/> that holds and
        /// gets the data for this custom property object.
        /// </summary>
        protected internal GridTableOptionsStyleInfo style;

        /// <summary>
        /// Initializes the <see cref="GridTableOptionsStyleInfoCustomPropertiesCollection"/> object
        /// with a <see cref="GridTableOptionsStyleInfo"/> that the properties of this
        /// class will belong to.
        /// </summary>
        /// <param name="style">The <see cref="GridTableOptionsStyleInfo"/> that holds and
        /// gets the data for this custom property object.</param>
        protected GridTableOptionsStyleInfoCustomProperties(GridTableOptionsStyleInfo style)
        {
            this.style = style;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="store"></param>
        [DebuggerStepThrough()]
        protected GridTableOptionsStyleInfoCustomProperties(GridTableOptionsStyleInfoStore store)
        {
            this.style = new GridTableOptionsStyleInfo(store);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="identity"></param>
        [DebuggerStepThrough()]
        protected GridTableOptionsStyleInfoCustomProperties(StyleInfoIdentityBase identity)
        {
            this.style = new GridTableOptionsStyleInfo(identity);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="store"></param>
        [DebuggerStepThrough()]
        protected GridTableOptionsStyleInfoCustomProperties(StyleInfoIdentityBase identity, GridTableOptionsStyleInfoStore store)
        {
            this.style = new GridTableOptionsStyleInfo(identity, store);
        }

        /// <summary>
        /// Initializes the <see cref="GridTableOptionsStyleInfoCustomPropertiesCollection"/> object
        /// with an empty <see cref="GridTableOptionsStyleInfo"/> object. When you later
        /// set the <see cref="GridTableOptionsStyleInfo"/> property, the changes in this object
        /// will be copied over to the new <see cref="GridTableOptionsStyleInfo"/> object.
        /// </summary>
        /// <remarks>
        /// The <see cref="GridTableOptionsStyleInfo.CustomProperties"/> collection adds
        /// design-time support for custom properties by adding empty custom property
        /// objects and later calling <see cref="GridTableOptionsStyleInfoCustomPropertiesCollection.Add"/>,
        /// which will result in changing the <see cref="GridTableOptionsStyleInfo"/> property for this object
        /// and forces copying all properties of this object to the style object.
        /// </remarks>
        protected GridTableOptionsStyleInfoCustomProperties()
        {
            this.style = new GridTableOptionsStyleInfo();
        }

        /// <summary>
        /// The <see cref="GridTableOptionsStyleInfo"/> that holds and
        /// gets the data for this custom property object. When you
        /// set the <see cref="GridTableOptionsStyleInfo"/> property, all prior changes in this object
        /// will be copied over to the new <see cref="GridTableOptionsStyleInfo"/> object.
        /// </summary>
        public GridTableOptionsStyleInfo TableOptions
        {
            get
            {
                return style;
            }

            set
            {
                if (value != style && style != null)
                {
                    value.ModifyStyle(style, Syncfusion.Styles.StyleModifyType.Override);
                }

                style = value;
                ////TableOptions.CopyFrom(value);
            }
        }

        static StyleInfoProperty _CreateStyleInfoProperty(Type componentType, StaticData sd, Type type, string propertyName, StyleInfoPropertyOptions propertyOptions)
        {
            return sd.CreateStyleInfoProperty(type, propertyName, 0, false, componentType, propertyOptions);
        }

        /// <overload>
        /// Registers a new custom property.
        /// </overload>>
        /// <summary>
        /// Registers a new custom property.
        /// </summary>
        /// <param name="componentType">The type of your derived custom property class.</param>
        /// <param name="type">The type of the property.</param>
        /// <param name="propertyName">The name of the property. This must match a property member in your class.</param>
        /// <returns>A <see cref="StyleInfoProperty"/> object that you should use for getting and setting
        /// values.</returns>
        protected static StyleInfoProperty CreateStyleInfoProperty(Type componentType, Type type, string propertyName)
        {
            return _CreateStyleInfoProperty(componentType, GridTableOptionsStyleInfoStore.StaticData, type, propertyName, StyleInfoPropertyOptions.All);
        }

        /// <summary>
        /// Registers a new custom property.
        /// </summary>
        /// <param name="componentType">The type of your derived custom property class.</param>
        /// <param name="propertyName">The name of the property. This must match a property member in your class.</param>
        /// <returns>A <see cref="StyleInfoProperty"/> object that you should use for getting and setting
        /// values.</returns>
        protected static StyleInfoProperty CreateStyleInfoProperty(Type componentType, string propertyName)
        {
            System.Reflection.PropertyInfo pi = componentType.GetProperty(propertyName);
            Type type = pi.PropertyType;
            return _CreateStyleInfoProperty(componentType, GridTableOptionsStyleInfoStore.StaticData, type, propertyName, StyleInfoPropertyOptions.All);
        }

        /// <summary>
        /// Registers a new custom property.
        /// </summary>
        /// <param name="componentType">The type of your derived custom property class.</param>
        /// <param name="propertyName">The name of the property. This must match a property member in your class.</param>
        /// <param name="propertyOptions">Specifies attributes for the property.</param>
        /// <returns>A <see cref="StyleInfoProperty"/> object that you should use for getting and setting
        /// values.</returns>
        protected static StyleInfoProperty CreateStyleInfoProperty(Type componentType, string propertyName, StyleInfoPropertyOptions propertyOptions)
        {
            System.Reflection.PropertyInfo pi = componentType.GetProperty(propertyName);
            Type type = pi.PropertyType;
            return _CreateStyleInfoProperty(componentType, GridTableOptionsStyleInfoStore.StaticData, type, propertyName, propertyOptions);
        }

        /// <summary>
        /// Registers a new custom property.
        /// </summary>
        /// <param name="componentType">The type of your derived custom property class.</param>
        /// <param name="type">The type of the property.</param>
        /// <param name="propertyName">The name of the property. This must match a property member in your class.</param>
        /// <param name="propertyOptions">Specifies attributes for the property.</param>
        /// <returns>A <see cref="StyleInfoProperty"/> object that you should use for getting and setting
        /// values.</returns>
        protected static StyleInfoProperty CreateStyleInfoProperty(Type componentType, Type type, string propertyName, StyleInfoPropertyOptions propertyOptions)
        {
            return _CreateStyleInfoProperty(componentType, GridTableOptionsStyleInfoStore.StaticData, type, propertyName, propertyOptions);
        }

        /// <summary>
        /// Registers a <see cref="StyleInfoProperty"/> for the specified property.
        /// </summary>
        /// <param name="componentType">The type of your derived custom property class.</param>        
        /// <param name="type">The type of the property.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="maxValue">The maximal possible Int16 value for the property.</param>
        /// <param name="makeBitValue">Indicates whether this StyleInfoProperty should be registered as a member of the BitArray and not to allocate
        /// an object reference.</param>
        /// <returns>The StyleInfoProperty with information about the property.</returns>
        protected static StyleInfoProperty CreateStyleInfoProperty(Type componentType, Type type, string name, short maxValue, bool makeBitValue)
        {
            StaticData sd = GridTableOptionsStyleInfoStore.StaticData;
            return sd.CreateStyleInfoProperty(type, name, maxValue, makeBitValue, componentType, StyleInfoPropertyOptions.All);
        }
    }

    /// <summary>
    /// Implements a collection of custom property objects that have
    /// at least one initialized value. The primary purpose of this
    /// collection is to support design-time code serialization of
    /// custom properties.
    /// </summary>
    public class GridTableOptionsStyleInfoCustomPropertiesCollection : ICollection
    {
        StyleInfoBase styleInfo;
        Hashtable types = new Hashtable();

        /// <summary>
        /// Initializes a <see cref="GridTableOptionsStyleInfoCustomPropertiesCollection"/> with a reference
        /// to the parent style object.
        /// </summary>
        /// <param name="styleInfo">The StyleInfoBase</param>
        internal GridTableOptionsStyleInfoCustomPropertiesCollection(StyleInfoBase styleInfo)
        {
            this.styleInfo = styleInfo;
            ICollection sipsc = styleInfo.Store.StyleInfoProperties;
            Type styleInfoType = styleInfo.GetType();

            foreach (StyleInfoProperty sip in sipsc)
            {
                //// Check if ComponentType is a custom property types and if property is initialized.
                if (!sip.ComponentType.IsAssignableFrom(styleInfoType) && styleInfo.HasValue(sip))
                {
                    //// Only add one object even and ignore subsequent properties
                    if (!types.ContainsKey(sip.ComponentType))
                    {
                        types.Add(sip.ComponentType, Activator.CreateInstance(sip.ComponentType, new object[] { styleInfo }));
                    }
                    ////System.Diagnostics.Trace.WriteLine(sip.PropertyName + ":" + sip.ComponentType.Name + " != " + styleInfoType.Name);
                }
            }
        }

        /// <summary>
        /// Copies the initialized properties of the specified custom property
        /// to the parent style object and attaches the custom property object
        /// with the parent style object.
        /// </summary>
        /// <param name="value">A GridTableOptionsStyleInfoCustomProperties with
        /// custom properties.</param>
        public void Add(GridTableOptionsStyleInfoCustomProperties value)
        {
            value.TableOptions = (GridTableOptionsStyleInfo)this.styleInfo;
        }

        #region ICollection Members
        bool ICollection.IsSynchronized
        {
            get
            {
                return types.Values.IsSynchronized;
            }
        }

        /// <summary>
        /// The number of objects in this collection.
        /// </summary>
        public int Count
        {
            get
            {
                return types.Count;
            }
        }

        /// <summary>
        ///   <para>Copies the <see cref="GridTableOptionsStyleInfoCustomPropertiesCollection" /> elements to a one-dimensional <see cref="System.Array" /> at the specified index.</para>
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="System.Array" /> that is the destination of the object's from instance. The <see cref="System.Array" /> must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        public void CopyTo(GridTableOptionsStyleInfoCustomProperties[] array, int index)
        {
            this.types.Values.CopyTo(array, index);
        }
        
        void ICollection.CopyTo(Array array, int index)
        {
            CopyTo((GridTableOptionsStyleInfoCustomProperties[])array, index);
        }

        object ICollection.SyncRoot
        {
            get
            {
                return types.Values.SyncRoot;
            }
        }
        #endregion
        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return types.Values.GetEnumerator();
        }
        #endregion
    }
    #endregion

    #region GridOptionsStyleInfoCustomPropertiesConverter
    /// <summary>
    ///    <para>Provides
    ///       a type converter to convert expandable objects to and from various
    ///       other representations.</para>
    /// </summary>
    internal class GridOptionsStyleInfoCustomPropertiesConverter : ExpandableObjectConverter
    {
        /// <summary>
        /// Returns whether this converter can convert the object to the specified type, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="destinationType">A <see cref="T:System.Type"/> that represents the type you want to convert to.</param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        /// <override/>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(System.ComponentModel.Design.Serialization.InstanceDescriptor))
            {
                return true;
            }
            else
            {
                return base.CanConvertTo(context, destinationType);
            }
        } //// end of method CanConvertTo

        /// <summary>
        /// Converts the given value object to the specified type, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="culture">A <see cref="T:System.Globalization.CultureInfo"/>. If null is passed, the current culture is assumed.</param>
        /// <param name="value">The <see cref="T:System.Object"/> to convert.</param>
        /// <param name="destinationType">The <see cref="T:System.Type"/> to convert the <paramref name="value"/> parameter to.</param>
        /// <returns>
        /// An <see cref="T:System.Object"/> that represents the converted value.
        /// </returns>
        /// <override/>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value != null && destinationType == typeof(System.ComponentModel.Design.Serialization.InstanceDescriptor))
            {
                System.Reflection.ConstructorInfo constructorInfo = value.GetType().GetConstructor(new Type[] { });
                if (constructorInfo != null)
                {
                    return new System.ComponentModel.Design.Serialization.InstanceDescriptor(constructorInfo, new object[] { }, false);
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        } // end of method ConvertTo
    }
    #endregion
}
