//-------------------------------------------------------------------------------------------------
// <copyright file="CollectionHelper.cs" company="syncfusion">
// Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using Syncfusion.ComponentModel;
using Syncfusion.Windows.Forms;

namespace Syncfusion.Grouping
{
    /// <summary>
    /// This interface is used by <see cref="GroupingCollectionEditor"/> to initialize
    /// collection and determine if changes were made inside the collection editor.
    /// If changes are detected, the collection will raise Changing and Changed events
    /// from within its <see cref="InitializeFrom"/> method.
    /// </summary>
    public interface IInsideCollectionEditorProperty : ICloneable
    {
        /// <summary>
        /// Gets / sets whether the collection is manipulated inside a collection editor.
        /// </summary>
        bool InsideCollectionEditor { get; set; }

        /// <summary>
        /// Initializes this object and copies properties from another object. 
        /// </summary>
        /// <param name="value">The source object.</param>
        void InitializeFrom(object value);
    }

    /// <summary>
    /// Provides a user interface that can edit collections of descriptor elements at design-time.
    /// </summary>
    /// <remarks>
    /// GroupingCollectionEditor checks for the IInsideCollectionEditorProperty of an associated collection.
    /// This allows it to make a copy of the original collection and compare the two collections at the
    /// time the user closes the collection editor. Only if changes are detected is the underlying component
    /// updated with changes.
    /// </remarks>
    public class GroupingCollectionEditor : CollectionEditor
    {
        private PropertyGridContextMenu pgMenu = null;
#if SyncfusionFramework2_0
        private bool m_bIsCancel = false;
#endif //SyncfusionFramework2_0

        /// <summary>
        /// Initializes a new <see cref="GroupingCollectionEditor"/> with a
        /// type to create instances for collention items.
        /// </summary>
        /// <param name="type">Type for collection item.</param>
        public GroupingCollectionEditor(Type type)
            : base(type)
        {
        }
#if SyncfusionFramework2_0
        /// <override/>
        protected override void CancelChanges()
        {
            base.CancelChanges();
            m_bIsCancel = true;
        }
#endif //SyncfusionFramework2_0

        /// <summary>
        /// Creates a new form to display and edit the current collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.ComponentModel.Design.CollectionEditor.CollectionForm"/> to provide as the user interface for editing the collection.
        /// </returns>
        /// <override/>
        protected override CollectionForm CreateCollectionForm()
        {
            CollectionForm collectionForm = base.CreateCollectionForm();

            PropertyGrid pg = WinFormsUtils.GetPropertyGridInControl(collectionForm);

            if (pg != null)
            {
                this.pgMenu = new PropertyGridContextMenu(pg);
            }

            return collectionForm;
        }

        /// <summary>Edits the value of the specified object using the given service provider and context.</summary>
        /// <param name="context">Format context.</param>
        /// <param name="provider">Service provider.</param>
        /// <param name="value">The object to edit.</param>
        /// <returns>Edited value.</returns>
        /// <override/>
        public override /*UITypeEditor*/ object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            IInsideCollectionEditorProperty collection = value as IInsideCollectionEditorProperty;

            if (collection == null)
            {
                return base.EditValue(context, provider, value);
            }

            if (context is GridItem)
            {
                ((GridItem)context).Expanded = false;
            }

            collection.InsideCollectionEditor = true;
            IInsideCollectionEditorProperty copy = (IInsideCollectionEditorProperty)collection.Clone();
            copy.InsideCollectionEditor = true;
            if (base.EditValue(context, provider, copy) != null)
            {
                collection.InsideCollectionEditor = false;
#if SyncfusionFramework2_0
                if (!m_bIsCancel)
#endif //SyncfusionFramework2_0
                {
                    collection.InitializeFrom(copy);
                }
#if SyncfusionFramework2_0
                m_bIsCancel = false;
#endif //SyncfusionFramework2_0
                TypeDescriptor.Refresh(collection);
                return value;
            }

            return null;
        }
    }
    
    /// <summary>
    /// Provides support for the <see cref="GetItemProperties"/> method that returns a <see cref="PropertyDescriptorCollection"/>.
    /// </summary>
    public interface IGetItemProperties
    {
        /// <summary>
        /// Returns a <see cref="PropertyDescriptorCollection"/>.
        /// </summary>
        /// <returns>A <see cref="PropertyDescriptorCollection"/>.</returns>
        PropertyDescriptorCollection GetItemProperties();
    }

    /// <summary>
    /// A type converter that utilizes the IGetItemProperties interface and provides standard
    /// values based on the properties returned by IGetItemProperties.GetItemProperties. These
    /// standard values can be dropped down in the PropertyGrid at design-time.
    /// </summary>
    public class GetItemPropertiesNameConverter : TypeConverter
    {
        /// <summary>
        /// Returns a collection of standard values for the data type this type converter is
        /// designed for when provided with a format context.
        /// </summary>
        /// <param name="context">Format context.</param>
        /// <returns>
        /// A <see cref="TypeConverter.StandardValuesCollection" />
        /// that holds a standard set of valid values, or null if the data type does not
        /// support a standard set of values.
        /// </returns>
        /// <override/>
        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            if (context != null)
            {
                PropertyDescriptorCollection pdc = PropertyDescriptorCollection.Empty;
                IGetItemProperties ips = context.Instance as IGetItemProperties;
                if (ips != null)
                {
                    pdc = ips.GetItemProperties();
                }

                if (pdc.Count > 0)
                {
                    ArrayList keys = new ArrayList();
                    int count = pdc.Count;
                    for (int index = 0; index < count; index++)
                    {
                        PropertyDescriptor pd = pdc[index];
                        if (pd.IsBrowsable)
                        {
                            keys.Add(pd.Name);
                        }
                    }

                    return new TypeConverter.StandardValuesCollection(keys);
                }
            }

            return new TypeConverter.StandardValuesCollection(new string[] { string.Empty });
        }
        
        /// <summary>
        /// Returns whether the collection of standard values returned from <see
        /// cref="TypeConverter.GetStandardValues" /> is an
        /// exclusive list of possible values, using the specified context.
        /// </summary>
        /// <param name="context">Format context. </param>
        /// <returns>
        /// true if the <see
        /// cref="TypeConverter.StandardValuesCollection" />
        /// returned from <see
        /// cref="TypeConverter.GetStandardValues" /> is an
        /// exhaustive list of possible values; false if other values are possible.
        /// </returns>
        /// <override/>
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return false;    // enables support for late bound scenario
        }

        /// <summary>
        /// Returns whether this object supports a standard set of values that can be picked
        /// from a list, using the specified context.
        /// </summary>
        /// <param name="context">Format context. </param>
        /// <returns>
        /// true if <see cref="TypeConverter.GetStandardValues" />
        /// should be called to find a common set of values the object supports; otherwise,
        /// false.
        /// </returns>
        /// <override/>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        /// <summary>
        /// Returns whether this converter can convert an object of the given type to the
        /// type of this converter, using the specified context.
        /// </summary>
        /// <param name="context">Format context. </param>
        /// <param name="sourceType">The type you want to convert from. </param>      
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        /// <override/>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        /// Converts the given object to the type of this converter, using the specified
        /// context and culture information.
        /// </summary>
        /// <param name="context">Format context. </param>
        /// <param name="culture">The culture information. </param>
        /// <param name="value">The object value to convert. </param>       
        /// <returns>
        /// The converted value.
        /// </returns>
        /// <override/>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                return (string)value;
            }

            return base.ConvertFrom(context, culture, value);
        }
    }

    /// <summary>
    /// Provides support for the <see cref="GetStandardValues"/> method that returns an array of standard values used by <see cref="StandardValuesCollectionConverter"/>.
    /// </summary>
    public interface IStandardValuesProvider
    {
        /// <summary>
        /// Returns an array of standard values used by <see cref="StandardValuesCollectionConverter"/>.
        /// </summary>
        /// <param name="pd">The context.PropertyDescriptor of a TypeConverter.GetStandardValues method.</param>
        /// <returns>An array of standard values used by <see cref="StandardValuesCollectionConverter"/>.</returns>
        ICollection GetStandardValues(PropertyDescriptor pd);
    }

    /// <summary>
    /// A type converter that utilizes the IStandardValuesProvider interface and provides standard
    /// values based on the names returned by IStandardValuesProvider.GetStandardValues. These
    /// standard values can be dropped down in the PropertyGrid at design-time.
    /// </summary>
    public class StandardValuesCollectionConverter : TypeConverter
    {
        /// <summary>
        /// Determines whether the standard values should be sorted alphabetically by
        /// checking the Engine.SortMappingNames property.
        /// </summary>
        /// <param name="ips">The object instance</param>
        /// <returns>returns boolean value</returns>
        protected virtual bool ShouldSortStandardValues(object ips)
        {
            PropertyInfo pi = ips.GetType().GetProperty("TableDescriptor");
            if (pi != null)
            {
                TableDescriptor td = pi.GetValue(ips, null) as TableDescriptor;
                if (td != null && td.Engine != null)
                {
                    return td.Engine.SortMappingNames;
                }
            }

            return false;
        }

        /// <override/>
        /// <summary>
        /// Returns a collection of standard values for the data type this type converter is
        /// designed for when provided with a format context.
        /// </summary>
        /// <param name="context">An <see
        /// cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format
        /// context that can be used to extract additional information about the environment
        /// from which this converter is invoked. This parameter or properties of this
        /// parameter can be null. </param>
        /// <returns>
        /// A <see cref="T:System.ComponentModel.TypeConverter.StandardValuesCollection" />
        /// that holds a standard set of valid values, or null if the data type does not
        /// support a standard set of values.
        /// </returns>
        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            if (context != null)
            {
                IStandardValuesProvider ips = context.Instance as IStandardValuesProvider;
                if (ips != null)
                {
                    ICollection coll = ips.GetStandardValues(context.PropertyDescriptor);
                    if (ShouldSortStandardValues(ips))
                    {
                        ArrayList al = new ArrayList(coll);
                        al.Sort();
                        coll = al;
                    }

                    return new TypeConverter.StandardValuesCollection(coll);
                }
            }

            return new TypeConverter.StandardValuesCollection(new string[] { string.Empty });
        }
        
        /// <override/>
        /// <summary>
        /// Returns whether the collection of standard values returned from <see
        /// cref="M:System.ComponentModel.TypeConverter.GetStandardValues" /> is an
        /// exclusive list of possible values, using the specified context.
        /// </summary>
        /// <param name="context">An <see
        /// cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format
        /// context. </param>
        /// <returns>
        /// true if the <see
        /// cref="T:System.ComponentModel.TypeConverter.StandardValuesCollection" />
        /// returned from <see
        /// cref="M:System.ComponentModel.TypeConverter.GetStandardValues" /> is an
        /// exhaustive list of possible values; false if other values are possible.
        /// </returns>
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return false;    // enables support for late bound scenario
        }

        /// <override/>
        /// <summary>
        /// Returns whether this object supports a standard set of values that can be picked
        /// from a list, using the specified context.
        /// </summary>
        /// <param name="context">An <see
        /// cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format
        /// context. </param>
        /// <returns>
        /// true if <see cref="M:System.ComponentModel.TypeConverter.GetStandardValues" />
        /// should be called to find a common set of values the object supports; otherwise,
        /// false.
        /// </returns>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        /// <override/>
        /// <summary>
        /// Returns whether this converter can convert an object of the given type to the
        /// type of this converter, using the specified context.
        /// </summary>
        /// <param name="context">An <see
        /// cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format
        /// context. </param>
        /// <param name="sourceType">A <see cref="T:System.Type" /> that represents the type
        /// you want to convert from. </param>       
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        /// <override/>
        /// <summary>
        /// Converts the given object to the type of this converter, using the specified
        /// context and culture information.
        /// </summary>
        /// <param name="context">An <see
        /// cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format
        /// context. </param>
        /// <param name="culture">The <see cref="T:System.Globalization.CultureInfo" /> to
        /// use as the current culture. </param>
        /// <param name="value">The <see cref="T:System.Object" /> to convert. </param>
        /// <returns>
        /// An <see cref="T:System.Object" /> that represents the converted value.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">The conversion cannot be
        /// performed. </exception>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                return (string)value;
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}
