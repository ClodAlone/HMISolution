#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

using System;
using System.Collections;
using System.Diagnostics;
using System.Reflection;

using Syncfusion.Styles;

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    /// This class holds arbitrary style information.
    /// </summary>
    public class ChartStyleInfoCustomProperties
    {
        // Instance data for type safe access to style information.

        /// <summary>
        /// The <see cref="ChartStyleInfo"/> object that holds and
        /// gets the data for this custom property object.
        /// </summary>
        protected internal ChartStyleInfo style;

        /// <summary>
        /// Overloaded. Initializes the <see cref="ChartStyleInfoCustomPropertiesCollection"/> object
        /// with a <see cref="ChartStyleInfo"/> that the properties of this
        /// class will belong to.
        /// </summary>
        /// <param name="style">The <see cref="ChartStyleInfo"/> object that holds and
        /// gets the data for this custom property object.</param>
        protected ChartStyleInfoCustomProperties(ChartStyleInfo style)
        {
            this.style = style;
        }

        /// <summary>
        /// Initializes the <see cref="ChartStyleInfoCustomPropertiesCollection"/> object
        /// with an empty <see cref="ChartStyleInfo"/> object. When you later
        /// set the <see cref="StyleInfo"/> property, the changes in this object
        /// will be copied over to the new <see cref="ChartStyleInfo"/> object.
        /// </summary>
        protected ChartStyleInfoCustomProperties()
        {
            this.style = new ChartStyleInfo();
        }

        /// <summary>
        /// Gets or sets the <see cref="ChartStyleInfo"/> that holds and
        /// gets the data for this custom property object. When you
        /// set the <see cref="StyleInfo"/> property all prior changes in this object
        /// will be copied over to the new <see cref="ChartStyleInfo"/> object.
        /// </summary>
        public ChartStyleInfo StyleInfo
        {
            get
            {
                return style;
            }

            set
            {
                if (value != style && style != null)
                {
                    value.ModifyStyle(style, StyleModifyType.Override);
                }

                style = value;
            }
        }

        /// <summary>
        /// _s the create style info property.
        /// </summary>
        /// <param name="componentType">Type of the component.</param>
        /// <param name="sd">The sd.</param>
        /// <param name="type">The type.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>Returns StyleInfoProperty object.</returns>
        private static StyleInfoProperty _CreateStyleInfoProperty(Type componentType, StaticData sd, Type type, string propertyName)
        {
            // TODO
            return sd.CreateStyleInfoProperty(type, propertyName, 0, false, componentType, StyleInfoPropertyOptions.All);
        }

        /// <overload>
        /// Overloaded. Registers a new custom property.
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
            return _CreateStyleInfoProperty(componentType, ChartStyleInfoStore.StaticData, type, propertyName);
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
            PropertyInfo pi = componentType.GetProperty(propertyName);
            Type type = pi.PropertyType;
            return _CreateStyleInfoProperty(componentType, ChartStyleInfoStore.StaticData, type, propertyName);
        }
    }

    /// <summary>
    /// Implements a collection of custom property objects that have
    /// at least one initialized value. The primary purpose of this
    /// collection is to support design-time code serialization of
    /// custom properties.
    /// </summary>
    public class ChartStyleInfoCustomPropertiesCollection : ICollection
    {
        private StyleInfoBase styleInfo;
        private Hashtable types = new Hashtable();

        /// <summary>
        /// Initializes a <see cref="ChartStyleInfoCustomPropertiesCollection"/> with a reference
        /// to the parent style object.
        /// </summary>
        /// <param name="styleInfo"></param>
        internal ChartStyleInfoCustomPropertiesCollection(StyleInfoBase styleInfo)
        {
            this.styleInfo = styleInfo;
            ICollection sipsc = styleInfo.Store.StyleInfoProperties;
            Type styleInfoType = styleInfo.GetType();

            foreach (StyleInfoProperty sip in sipsc)
            {
                // Check if ComponentType is a custom property type and if property is initialized.
                if (!sip.ComponentType.IsAssignableFrom(styleInfoType) && styleInfo.HasValue(sip))
                {
                    // Only add one object even and ignore subsequent properties.
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
        /// <param name="value">A ChartStyleInfoCustomProperties with
        /// custom properties.</param>
        public void Add(ChartStyleInfoCustomProperties value)
        {
            value.StyleInfo = (ChartStyleInfo)this.styleInfo;
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
        /// Gets the number of objects in this collection.
        /// </summary>
        public int Count
        {
            get
            {
                return types.Count;
            }
        }

        /// <summary>
        ///   <para>Copies the <see cref="ChartStyleInfoCustomPropertiesCollection" /> elements to a one-dimensional <see cref="System.Array" /> at the specified index.</para>
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="System.Array" /> which is the destination of the objects from the instance. The <see cref="System.Array" /> must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        public void CopyTo(ChartStyleInfoCustomProperties[] array, int index)
        {
            this.types.Values.CopyTo(array, index);
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.ICollection"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.ICollection"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="array"/> is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// 	<paramref name="index"/> is less than zero.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// 	<paramref name="array"/> is multidimensional.
        /// -or-
        /// <paramref name="index"/> is equal to or greater than the length of <paramref name="array"/>.
        /// -or-
        /// The number of elements in the source <see cref="T:System.Collections.ICollection"/> is greater than the available space from <paramref name="index"/> to the end of the destination <paramref name="array"/>.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// The type of the source <see cref="T:System.Collections.ICollection"/> cannot be cast automatically to the type of the destination <paramref name="array"/>.
        /// </exception>
        void ICollection.CopyTo(Array array, int index)
        {
            CopyTo((ChartStyleInfoCustomProperties[])array, index);
        }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"/>.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// An object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"/>.
        /// </returns>
        object ICollection.SyncRoot
        {
            get
            {
                return types.Values.SyncRoot;
            }
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return types.Values.GetEnumerator();
        }

        #endregion
    }

    /// <summary>
    /// <see cref="ChartStyleInfoSubObject"/> is an abstract base class for classes
    /// to be used as sub-objects in a <see cref="ChartStyleInfo"/>.
    /// </summary>
    /// <remarks>
    /// <see cref="ChartStyleInfoSubObject"/> is derived from <see cref="StyleInfoBase"/>
    /// and thus provides the same easy way to provide properties that can inherit values
    /// from base styles at run-time.<para/>
    /// The difference is that <see cref="ChartStyleInfoSubObject"/> supports this inheritance
    /// mechanism as a sub-object from a <see cref="ChartStyleInfo"/>. A sub-object needs to
    /// have knowledge about its parent object and be able to walk the base styles from the
    /// parent object.<para/>
    /// Examples for implementation of <see cref="ChartStyleInfoSubObject"/> is the font
    /// class in Essential Chart.<para/>
    /// Programmers can derive their own style classes from <see cref="ChartStyleInfoSubObject"/>
    /// and add type-safe (and Intelli-sense)
    /// supported custom properties to the style class.
    /// <para/>
    /// See the overview of <see cref="StyleInfoBase"/> for further discussion about style objects.
    /// </remarks>
    public abstract class ChartStyleInfoSubObject : ChartSubStyleInfoBase
    {
        /// <summary>
        /// Initializes a new <see cref="ChartStyleInfoSubObject"/> object and associates it with an existing <see cref="StyleInfoStore"/>.
        /// </summary>
        /// <param name="identity">A <see cref="StyleInfoSubObjectIdentity"/> that holds the identity for this <see cref="StyleInfoBase"/>.
        /// <param name="store">A <see cref="StyleInfoStore"/> that holds data for this object.</param>
        /// All changes made in this style object will be saved in the <see cref="StyleInfoStore"/> object.
        /// </param>
        [DebuggerStepThrough()]
        public ChartStyleInfoSubObject(StyleInfoSubObjectIdentity identity, StyleInfoStore store)
            : base(identity, store)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="ChartStyleInfoSubObject"/> object and associates it with an existing <see cref="StyleInfoStore"/>.
        /// </summary>
        /// <param name="store">A <see cref="StyleInfoStore"/> that holds data for this object.
        /// All changes made in this style object will be saved in the <see cref="StyleInfoStore"/> object.
        /// </param>
        [DebuggerStepThrough()]
        public ChartStyleInfoSubObject(StyleInfoStore store)
            : base(store)
        {
        }

        /// <summary>
        /// Returns the <see cref="ChartStyleInfo"/> that this sub-object belongs to.
        /// </summary>
        /// <returns>The parent style object.</returns>
        public ChartStyleInfo GetChartStyleInfo()
        {
            StyleInfoSubObjectIdentity id = this.Identity as StyleInfoSubObjectIdentity;
            return (id != null) ? id.Owner as ChartStyleInfo : null;
        }
    }
}