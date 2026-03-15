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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

using ArrayList = System.Collections.Generic.List<object>;
using Hashtable = System.Collections.Generic.Dictionary<object, object>;

#if !WinRT
using Syncfusion.Windows.ComponentModel;
using BitVector32 = Syncfusion.Windows.Collections.BitVectorInt32;

namespace Syncfusion.Windows.Styles
#else
using Syncfusion.WinRT.ComponentModel;
using BitVector32 = Syncfusion.WinRT.Collections.BitVectorInt32;
using System.Collections.Generic;
using System.Linq;

namespace Syncfusion.WinRT.Styles
#endif
{
    /// <summary>
    /// Holds all StyleInfoProperties used by derived classes.
    /// This should go in a product specific StaticData.<para/>
    /// The concrete Style class could provide a static memory StaticData that belongs
    /// to the process and library.
    /// </summary>
    [DebuggerStepThrough()]
    public class StaticData
    {
        internal readonly ArrayList styleInfoProperties;
        internal int objectCount = 0;
        internal int expandableObjectCount = 0;
        internal int dataVectorCount;

        object dataPreviousSection;
        short dataPreviousBitCount;
        int previousIncludeBit;
        Type styleInfoType;
        string[] sortOrder = null;
        bool sortProperties = false;

        const int maxbits = 31;
        const int maxbits1 = 30;

        /// <summary>
        /// Returns a collection with <see cref="StyleInfoProperty"/> objects.
        /// </summary>
        public ICollection StyleInfoProperties
        {
            get
            {
                return styleInfoProperties;
            }
        }

        /// <summary>
        /// Initializes a new <see cref="StaticData"/> object with information about the parent style class.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="styleInfoType">Will be used to access the PropertyInfo and its custom attributes for a property.</param>
        /// <param name="sortProperties"></param>
        public StaticData(Type type, Type styleInfoType, bool sortProperties)
        {

            this.styleInfoType = styleInfoType;
            this.sortProperties = sortProperties;

#if !WinRT
            // Does baseType also have static Data?
            Type baseType = type.BaseType;

            object[] atts = baseType.GetCustomAttributes(typeof(StaticDataFieldAttribute), false);
            string fieldName;
            if (atts.Length > 0)
                fieldName = ((StaticDataFieldAttribute)atts[0]).FieldName;
            else
                fieldName = StaticDataFieldAttribute.Default.FieldName;

            FieldInfo fi = baseType.GetField(fieldName, BindingFlags.Static | BindingFlags.NonPublic);
#else
            Type baseType = type.GetTypeInfo().BaseType;
            IEnumerable<Attribute> atts = baseType.GetTypeInfo().GetCustomAttributes(typeof(StaticDataFieldAttribute), false);
            string fieldName;
            if (atts.Count() > 0)
                fieldName = ((StaticDataFieldAttribute)atts.ElementAt(0)).FieldName;
            else
                fieldName = StaticDataFieldAttribute.Default.FieldName;

            FieldInfo fi = baseType.GetRuntimeField(fieldName);
#endif

            if (fi != null)
            {
                StaticData sds = fi.GetValue(null) as StaticData;
                if (sds != null)
                {
                    // Yes, copy settings from base style
                    // for StyleInfoProperties only references will be copied.
                    styleInfoProperties = new ArrayList();
                    styleInfoProperties.AddRange(sds.styleInfoProperties);
                    dataPreviousSection = sds.dataPreviousSection;
                    dataPreviousBitCount = sds.dataPreviousBitCount;
                    dataVectorCount = sds.dataVectorCount;
                    previousIncludeBit = sds.previousIncludeBit;
                    return;
                }
            }
            styleInfoProperties = new ArrayList();
            dataPreviousSection = null;
            dataPreviousBitCount = 0;
            dataVectorCount = 0;
            previousIncludeBit = 0;
        }

        /// <internalonly/>
        public string[] CreatePropertyGridSortOrder(string[] sortOrder)
        {
            this.sortOrder = sortOrder;
            return sortOrder;
        }

        /// <internalonly/>
        public string[] PropertyGridSortOrder
        {
            get
            {
                if (sortOrder == null)
                {
                    if (sortProperties)
                    {
                        sortOrder = new string[styleInfoProperties.Count];
                        for (int n = 0; n < styleInfoProperties.Count; n++)
                        {
                            StyleInfoProperty sip = (StyleInfoProperty)styleInfoProperties[n];
                            sortOrder[n] = sip.PropertyName;
                        }
                    }
                    else
                        sortOrder = new string[0];
                }
                return sortOrder;
            }
        }

        /// <internalonly/>
        public BitVector32.Section AllocateDataVectorSection(short maxValue, out int bvi)
        {
            // check if needed bits fit into current bitvector
            if (dataVectorCount == 0)
                dataVectorCount = 1;
            short bitCount = CountBitsSet((short)maxValue);
            if (dataPreviousSection != null)
            {
                BitVector32.Section sec = (BitVector32.Section)dataPreviousSection;
                if (bitCount + dataPreviousBitCount > maxbits1)
                {
                    // allocate staticData in the next bitvector
                    dataVectorCount++;
                    dataPreviousSection = null;
                    dataPreviousBitCount = 0;
                }
            }

            // allocate section
            BitVector32.Section bvs;
            dataPreviousSection = bvs = (dataPreviousSection == null) ?
              BitVector32.CreateSection(maxValue) :
              BitVector32.CreateSection(maxValue, (BitVector32.Section)dataPreviousSection);

            // could also check here if CreateSection throws "BitVector_Full" exception
            // but the problem is BitVector32 allows cutting the last value if it does
            // not fit.

            bvi = dataVectorCount - 1;

            dataPreviousBitCount += bitCount;

            return bvs;
        }

        /// <summary>
        /// Indicates whether properties have been registered. Returns True if not registered; False otherwise.
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return this.styleInfoProperties.Count == 0;
            }
        }

        private static short CountBitsSet(short mask)
        {
            short i;
            for (i = 0; mask != 0; i++)
                mask = (short)(mask >> 1);
            return i;
        }

        /// <overload>
        /// Registers a <see cref="StyleInfoProperty"/> for the specified property.
        /// </overload>
        /// <summary>
        /// Registers a <see cref="StyleInfoProperty"/> for the specified property.
        /// </summary>
        /// <param name="type">The type of the property.</param>
        /// <param name="name">The name of the property.</param>
        /// <returns>The StyleInfoProperty with information about the property.</returns>
        public StyleInfoProperty CreateStyleInfoProperty(Type type, string name)
        {
            return CreateStyleInfoProperty(type, name, 0, false);
        }

        /// <summary>
        /// Registers a <see cref="StyleInfoProperty"/> for the specified property.
        /// </summary>
        /// <param name="type">The type of the property.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="propertyOptions">Specifies options for the property.</param>
        /// <returns>The StyleInfoProperty with information about the property.</returns>
        public StyleInfoProperty CreateStyleInfoProperty(Type type, string name, StyleInfoPropertyOptions propertyOptions)
        {
            return CreateStyleInfoProperty(type, name, 0, false, propertyOptions);
        }

        /// <summary>
        /// Registers a <see cref="StyleInfoProperty"/> for the specified property.
        /// </summary>
        /// <param name="type">The type of the property.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="maxValue">The maximal possible Int16 value for the property.</param>
        /// <returns>The StyleInfoProperty with information about the property.</returns>
        public StyleInfoProperty CreateStyleInfoProperty(Type type, string name, short maxValue)
        {
            return CreateStyleInfoProperty(type, name, maxValue, false);
        }

        private static int currentExpandableObjectKey;

        static int CreateExpandableObjectKey()
        {
            return currentExpandableObjectKey++;
        }

        /// <summary>
        /// Registers a <see cref="StyleInfoProperty"/> for the specified property.
        /// </summary>
        /// <param name="type">The type of the property.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="maxValue">The maximal possible Int16 value for the property.</param>
        /// <param name="makeBitValue">Indicates whether this StyleInfoProperty should be registered as a member of the BitArray and not to allocate
        /// an object reference.</param>
        /// <returns>The StyleInfoProperty with information about the property.</returns>
        public StyleInfoProperty CreateStyleInfoProperty(Type type, string name, short maxValue, bool makeBitValue)
        {
            return CreateStyleInfoProperty(type, name, maxValue, makeBitValue, styleInfoType,
              StyleInfoPropertyOptions.All);
        }

        /// <summary>
        /// Registers a <see cref="StyleInfoProperty"/> for the specified property.
        /// </summary>
        /// <param name="type">The type of the property.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="maxValue">The maximal possible Int16 value for the property.</param>
        /// <param name="makeBitValue">Indicates whether this StyleInfoProperty should be registered as a member of the BitArray and not to allocate
        /// an object reference.</param>
        /// <param name="propertyOptions">Specifies options for the property.</param>
        /// <returns>The StyleInfoProperty with information about the property.</returns>
        public StyleInfoProperty CreateStyleInfoProperty(Type type, string name, short maxValue, bool makeBitValue, StyleInfoPropertyOptions propertyOptions)
        {
            return CreateStyleInfoProperty(type, name, maxValue, makeBitValue, styleInfoType, propertyOptions);
        }

        /// <summary>
        /// Registers a <see cref="StyleInfoProperty"/> for the specified property.
        /// </summary>
        /// <param name="type">The type of the property.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="maxValue">The maximal possible Int16 value for the property.</param>
        /// <param name="makeBitValue">Indicates whether this StyleInfoProperty should be registered as a member of the BitArray and not to allocate
        /// an object reference.</param>
        /// <param name="propertyOptions">Specifies options for the property.</param>
        /// <param name="componentType">The component type that hosts the property.</param>
        /// <returns>The StyleInfoProperty with information about the property.</returns>
        public StyleInfoProperty CreateStyleInfoProperty(Type type, string name, short maxValue, bool makeBitValue, Type componentType, StyleInfoPropertyOptions propertyOptions)
        {
            StyleInfoProperty sip = _CreateStyleInfoProperty(type, name, maxValue, makeBitValue, componentType);

            //			if (componentType != null)
            //			{
            //				PropertyInfo propertyInfo = componentType.GetProperty(name);
            //				//sip.PropertyInfo = propertyInfo;
            //				sip.IsSerializable = propertyInfo == null ? true : SerializePropertyAttribute.IsSerializeProperty(propertyInfo);
            //				sip.IsCloneable = propertyInfo == null ? true : CloneablePropertyAttribute.IsCloneableProperty(propertyInfo);
            //				sip.IsDisposable = propertyInfo == null ? true : DisposeablePropertyAttribute.IsDisposeableProperty(propertyInfo);
            //
            //				StyleInfoPropertyOptions e = StyleInfoPropertyOptions.None;
            //				if (sip.IsSerializable)
            //					e |= StyleInfoPropertyOptions.Serializable;
            //
            //				if (sip.IsCloneable)
            //					e |= StyleInfoPropertyOptions.Cloneable;
            //
            //				if (sip.IsDisposable)
            //					e |= StyleInfoPropertyOptions.Disposable;
            //
            //				Trace.WriteLine(name + ": " + "StyleInfoPropertyOptions." + e.ToString("G"));
            //			}
            //			else
            //			{
            //				sip.IsSerializable = true;
            //				sip.IsCloneable = true;
            //				sip.IsDisposable = true;
            //			}

            sip.IsSerializable = (propertyOptions & StyleInfoPropertyOptions.Serializable) != 0;
            sip.IsCloneable = (propertyOptions & StyleInfoPropertyOptions.Cloneable) != 0;
            sip.IsDisposable = (propertyOptions & StyleInfoPropertyOptions.Disposable) != 0;

            return sip;
        }

        StyleInfoProperty _CreateStyleInfoProperty(Type type, string name, short maxValue, bool makeBitValue, Type componentType)
        {

            int found = -1;
            StyleInfoProperty sip2 = null;
            for (int n = 0; n < styleInfoProperties.Count; n++)
            {
                sip2 = (StyleInfoProperty)styleInfoProperties[n];
                if (sip2.PropertyName == name)
                {
                    found = n;
                    break;
                }
            }

            StyleInfoProperty sip = new StyleInfoProperty(type, name, maxValue, componentType);

            // maxValue specified?
            if (maxValue == 0)
            {
                // no - use default maximum value for specified type
                if (type == typeof(bool))
                    maxValue = 1;
                else if (type == typeof(byte) || type == typeof(sbyte))
                    maxValue = byte.MaxValue;
                else if (type == typeof(short))
                    maxValue = short.MaxValue;
            }
            else
            {
                // yes - let's check the value is good for specified type
                if (type == typeof(bool) && maxValue > 1
                  || (type == typeof(byte) || type == typeof(sbyte)) && maxValue > byte.MaxValue
                  || type == typeof(short) && maxValue > short.MaxValue)
                    throw new ArgumentOutOfRangeException("maxValue", maxValue.ToString() +  "too large for type " + type.Name);

                // checking range of enums is too much cost here
            }

            if (found != -1 &&
              sip2.MaxValue == maxValue &&
              sip2.IsExpandable == sip.IsExpandable &&
              makeBitValue == (sip.DataVectorIndex != -1)
              )
            {
                // nothing changed, reuse info.
                sip.MaxValue = sip2.MaxValue;
                sip.DataVectorIndex = sip2.DataVectorIndex;
                sip.DataVectorSection = sip2.DataVectorSection;
                sip.ExpandableObjectStoreKey = sip2.ExpandableObjectStoreKey;
                sip.ObjectStoreKey = sip2.ObjectStoreKey;
                sip.BitVectorIndex = sip2.BitVectorIndex;
                sip.BitVectorMask = sip2.BitVectorMask;
                sip.Index = sip2.Index;
            }
            else
            {
                // Allocate staticData for data
#if !WinRT
                if (makeBitValue && maxValue != 0 && (type.IsEnum
                  || type == typeof(bool) || type == typeof(byte)
                  || type == typeof(sbyte) || type == typeof(short)))
#else
                if (makeBitValue && maxValue != 0 && (type.GetTypeInfo().IsEnum
                  || type == typeof(bool) || type == typeof(byte)
                  || type == typeof(sbyte) || type == typeof(short)))
#endif
                {
                    // Create a bitvector entry

                    int bvi;
                    sip.DataVectorSection = AllocateDataVectorSection(maxValue, out bvi);
                    sip.DataVectorIndex = bvi;
                    sip.MaxValue = maxValue;
                }
                else if (sip.IsExpandable)
                {
                    sip.ExpandableObjectStoreKey = StaticData.CreateExpandableObjectKey();
                    expandableObjectCount++;
                }
                else
                {
                    objectCount++;
                    // Create a property store entry
                    sip.ObjectStoreKey = StyleInfoObjectStore.CreateKey();
                }

                // Allocate include bit (changed bit will use the same key)
                int count = styleInfoProperties.Count;
                if (count % maxbits == 0)
                    sip.BitVectorMask = BitVector32.CreateMask();
                else
                    sip.BitVectorMask = BitVector32.CreateMask(previousIncludeBit);

                previousIncludeBit = sip.BitVectorMask;
                sip.BitVectorIndex = count / maxbits;
                sip.Index = count;
            }

            if (found == -1)
                styleInfoProperties.Add(sip);
            else
                styleInfoProperties[found] = sip;

            // Done.
            return sip;
        }
    }

    /// <summary>
    /// Provides storage for the <see cref="StyleInfoBase"/> object.
    /// </summary>
    /// <remarks>
    /// You cannot instantiate a <see cref="StyleInfoStore"/> class directly. You have
    /// to derive a concrete class from this class that you can instantiate.<para/>
    /// In derived classes of <see cref="StyleInfoBase"/>, you always need to
    /// implement a <see cref="StyleInfoBase"/> / <see cref="StyleInfoStore"/>
    /// pair. The <see cref="StyleInfoStore"/> holds all the data that are specific
    /// to the style object and should be persisted.<para/>
    /// The <see cref="StyleInfoBase"/> is a wrapper around the <see cref="StyleInfoStore"/>.
    /// It provides type safe accessor properties to modify data of the underlying
    /// data store and can hold temporary information about the style object that
    /// does not need to be persisted.<para/>
    ///
    /// In Essential Grid for example, the GridStyleInfo class holds extensive identity
    /// information about a style object such as cached base styles, row and column index,
    /// a reference to the grid model and more. These are all the information that can be discarded
    /// when the style is no longer used (because maybe the cell is not visible anymore). Only
    /// the <see cref="StyleInfoStore"/> part needs to be kept alive.
    /// <para/>
    /// <see cref="StyleInfoStore"/> allows you to register any number of properties but keeps the data
    /// very memory efficient. Only properties that are actually used for a style
    /// object will be allocated for an object. The StyleObjectStore handles the storage of objects.
    /// For short integers, enums and Boolean values the data will be stored in a BitVector32
    /// structure to save even more memory.
    /// <para/>
    /// See the overview for <see cref="StyleInfoBase"/> for further discussion about style objects.
    /// </remarks>
    /// <seealso cref="StyleInfoIdentityBase"/>
    /// <seealso cref="StyleInfoBase"/>
    public abstract class StyleInfoStore :
      IDisposable,
      IStyleInfo,
      IXmlSerializable
    {
        /// <summary>
        /// Searches the <see cref="StyleInfoProperty"/> with the given name.
        /// </summary>
        /// <param name="name">The name of the property to look for.</param>
        /// <returns>A <see cref="StyleInfoProperty"/> that is associated with the specified name.</returns>
        public StyleInfoProperty FindStyleInfoProperty(string name)
        {
            if (m_cache == null)
            {
                StaticData sd = StaticDataStore;
                m_cache = new Hashtable(sd.styleInfoProperties.Count);

                for (int i = 0, len = sd.styleInfoProperties.Count; i < len; i++)
                {
                    StyleInfoProperty sip = sd.styleInfoProperties[i] as StyleInfoProperty;
                    m_cache[sip.PropertyName] = sip;
                }
            }

            return m_cache[name] as StyleInfoProperty;
        }

        /// <summary>
        /// Returns a collection with <see cref="StyleInfoProperty"/> objects.
        /// </summary>
        [XmlIgnore]
        public ICollection StyleInfoProperties
        {
            get
            {
                return StaticDataStore.styleInfoProperties;
            }
        }

        void FixVectorCount()
        {
            StaticData sd = StaticDataStore;
            int vectorCount = (sd.styleInfoProperties.Count + maxbits1) / maxbits;
            //if (vectorCount == 0)
            //	throw new InvalidOperationException("Static ctor has not been called.");

            if (include.Length < vectorCount)
            {
                BitVector32[] oinclude = include;
                BitVector32[] ochanged = changed;
                include = new BitVector32[vectorCount];
                Array.Copy(oinclude, 0, include, 0, oinclude.Length);
                changed = new BitVector32[vectorCount];
                Array.Copy(ochanged, 0, changed, 0, ochanged.Length);
            }

            if (data == null && sd.dataVectorCount > 0)
            {
                data = new BitVector32[sd.dataVectorCount];
            }
        }

        /// <summary>
        /// Static data must be declared static in derived classes (this avoids collisions
        /// when StyleInfoStore is used in the same project for different types of style
        /// classes).
        /// </summary>
        [XmlIgnore]
        protected abstract StaticData StaticDataStore
        {
            get;
        }

        // instance data
        BitVector32[] data;
        internal BitVector32[] include;
        BitVector32[] changed;
        StyleInfoObjectStore objects;
        StyleInfoObjectStore expandableObjects;
        Hashtable m_cache = null;
        bool recordChanges;

        const int maxbits = 30; //31; reduced because of BitVector32 - int use instead of uint
        const int maxbits1 = 29;// 30;


 
        /// <summary>
        /// Initializes an empty <see cref="StyleInfoStore"/>.
        /// </summary>
        protected StyleInfoStore()
        {
            StaticData sd = StaticDataStore;
            data = sd.dataVectorCount > 0 ? new BitVector32[sd.dataVectorCount] : null;
            int vectorCount = (sd.styleInfoProperties.Count + maxbits1) / maxbits;
            if (vectorCount == 0)
                throw new InvalidOperationException("Static ctor has not been called.");
            include = new BitVector32[vectorCount];
            changed = new BitVector32[vectorCount];
            objects = null; // sd.objectCount > 0 ? new StyleInfoObjectStore() : null;
            expandableObjects = null; // sd.expandableObjectCount > 0 ? new StyleInfoObjectStore() : null;
        }

        /// <summary>
        /// Creates an exact copy of the current object.
        /// </summary>
        /// <returns>A <see cref="StyleInfoStore"/> with same data as the current object.</returns>
        public virtual object Clone()
        {
            // Create same type as derived class.
            StyleInfoStore target = Activator.CreateInstance(this.GetType()) as StyleInfoStore;
            CopyTo(target);
            return target;
        }

        /// <summary>
        /// The <see cref="StyleInfoBaseConverter"/> class checks this property
        /// to find out about the sort order of the properties in this <see cref="StyleInfoStore"/>.
        /// </summary>
        [XmlIgnore]
        public string[] PropertyGridSortOrder
        {
            get
            {
                return StaticDataStore.PropertyGridSortOrder;
            }
        }

        /// <summary>
        /// Copies all properties to another <see cref="StyleInfoStore"/>.
        /// </summary>
        /// <param name="target">The target to copy all properties to.</param>
        public void CopyTo(StyleInfoStore target)
        {
            target.FixVectorCount();
            foreach (StyleInfoProperty sip in StyleInfoProperties)
            {
                if (this.HasValue(sip))
                    target._AssignProperty(sip, this);
            }
        }

        /// <summary>
        /// Releases all the resources used by the component.
        /// </summary>
        public void Dispose()
        {
            if (objects != null)
            {
                this.objects.Dispose();
                this.objects = null;
            }
            if (expandableObjects != null)
            {
                this.expandableObjects.Dispose();
                this.expandableObjects = null;
            }
            this.changed = null;
            this.data = null;
            this.include = null;
            GC.SuppressFinalize(this);
        }

        /// <override/>
        public override bool Equals(object obj)
        {
            StyleInfoStore style = obj as StyleInfoStore;
            if (style == null)
                return false;

            foreach (StyleInfoProperty sip in StyleInfoProperties)
            {
                if ((this.HasValue(sip) || style.HasValue(sip))
                  && !_EqualsProperty(sip, style))
                    return false;
            }

            return true;
        }

        /// <override/>
        public override int GetHashCode()
        {
            uint h = 0x7832c9f4;

            foreach (StyleInfoProperty sip in StyleInfoProperties)
            {
                int v = 0;
                if (this.HasValue(sip)) // include bit set
                {
                    if (sip.ExpandableObjectStoreKey != -1)
                    {
                        StyleInfoStore val = (StyleInfoStore)this.GetValue(sip);
                        v = val.GetHashCode();
                    }
                    else if (sip.ObjectStoreKey == -1)  // ensure value is a System.Int16
                        v = this.GetShortValue(sip);

                    else
                        continue;// don't call GetHashCode for regular objects - don't know
                    // if they properly implement GetHashCode ...

                    h ^= (h << 5) + ((uint)v) + (h >> 2);
                }
            }

            return (int)h;
        }

        /// <override/>
        public override string ToString()
        {
            return _ToString("");
        }

        private string _ToString(string prefix)
        {
            StringBuilder sb = new StringBuilder();
            foreach (StyleInfoProperty sip in StyleInfoProperties)
            {
                if (!sip.IsSerializable)
                    continue;

                if (this.HasValue(sip))
                {
                    if (sip.IsExpandable)
                    {
                        StyleInfoStore styleInfoStore = this.GetValue(sip) as StyleInfoStore;
                        if (styleInfoStore != null)
                            sb.Append(styleInfoStore._ToString(prefix + sip.PropertyName + "."));
                    }
                    else
                    {
                        sb.Append(prefix);
                        sb.Append(sip.PropertyName);
                        sb.Append(" = ");
                        sb.Append(sip.FormatValue(GetValue(sip)));
                        sb.Append(Environment.NewLine);
                    }
                }
            }
            return sb.ToString();
        }

        void IStyleInfo.ParseString(string s)
        {
            throw new InvalidOperationException();
        }

        StyleInfoStore IStyleInfo.Store
        {
            get { return this; }
        }

        /// <summary>
        /// Resets all "Changed" bits that mark certain properties as modified.
        /// </summary>
        public void ResetChangedBits()
        {
            this.FixVectorCount();
            for (int n = 0; n < changed.Length; n++)
                changed[n] = new BitVector32(0);
        }

        /// <summary>
        /// Clears out all properties for this <see cref="StyleInfoStore"/>.
        /// </summary>
        public void Clear()
        {
            this.FixVectorCount();
            for (int n = 0; n < include.Length; n++)
                include[n] = new BitVector32(0);
        }

        /// <summary>
        /// Indicates whether a specific property has been initialized for the current object.
        /// </summary>
        /// <param name="sip">A <see cref="StyleInfoProperty"/> that identifies the property to operate on.</param>
        public bool HasValue(StyleInfoProperty sip)
        {
#if DEBUG
            if (include == null || sip.BitVectorIndex >= include.Length)
                throw new InvalidOperationException("style was probably disposed or wrong StyleInfoProperty was used.");
#endif

            return include[sip.BitVectorIndex][sip.BitVectorMask];
        }
        /// <summary>
        /// Indicates whether a specific property has been modified for the current object.
        /// </summary>
        /// <param name="sip">A <see cref="StyleInfoProperty"/> that identifies the property to operate on.</param>
        public bool IsValueModified(StyleInfoProperty sip)
        {
            return sip.BitVectorIndex < changed.Length && changed[sip.BitVectorIndex][sip.BitVectorMask];
        }
        /// <summary>
        /// Marks a specific property as modified or unmodified for the current object.
        /// </summary>
        /// <param name="sip">A <see cref="StyleInfoProperty"/> that identifies the property to operate on.</param>
        /// <param name="value">The new value.</param>
        public void SetValueModified(StyleInfoProperty sip, bool value)
        {
            this.FixVectorCount();
            changed[sip.BitVectorIndex][sip.BitVectorMask] = value;
        }
        /// <summary>
        /// Marks a specific property as uninitialized for the current object.
        /// </summary>
        /// <param name="sip">A <see cref="StyleInfoProperty"/> that identifies the property to operate on.</param>
        public void ResetValue(StyleInfoProperty sip)
        {
            if (null != sip)
            {
                this.FixVectorCount();
                if (sip.ObjectStoreKey != -1)
                {
                    if (HasValue(sip))
                        _ResetObject(sip.ObjectStoreKey, sip.IsDisposable);
                }
                else if (sip.ExpandableObjectStoreKey != -1)
                {
                    if (HasValue(sip))
                        _ResetExpandableObject(sip.ExpandableObjectStoreKey);
                }

                include[sip.BitVectorIndex][sip.BitVectorMask] = false;
                if (recordChanges) changed[sip.BitVectorIndex][sip.BitVectorMask] = true;

            }
        }
        /// <summary>
        /// Queries the value for a specific property that has been initialized for the current object.
        /// </summary>
        /// <param name="sip">A <see cref="StyleInfoProperty"/> that identifies the property to operate on.</param>
        public virtual object GetValue(StyleInfoProperty sip)
        {
            if (sip == null)
                return null;
            else if (sip.ObjectStoreKey != -1)
                return _GetObject(sip.ObjectStoreKey);
            else if (sip.ExpandableObjectStoreKey != -1)
                return _GetExpandableObject(sip.ExpandableObjectStoreKey);
#if !WinRT
            else if (sip.PropertyType.IsEnum)
#else
            else if (sip.PropertyType.GetTypeInfo().IsEnum)
#endif
                return Enum.ToObject(sip.PropertyType, GetShortValue(sip));
            else
                return NullableHelper.ChangeType(GetShortValue(sip), sip.PropertyType);
            //return GetShortValue(sip);
        }
        /// <summary>
        /// Queries the <see cref="System.Int16"/> value for a specific property that has been initialized for the current object.
        /// </summary>
        /// <param name="sip">A <see cref="StyleInfoProperty"/> that identifies the property to operate on.</param>
        public short GetShortValue(StyleInfoProperty sip)
        {
            if (sip.ObjectStoreKey != -1 || sip.ExpandableObjectStoreKey != -1)
                throw new InvalidOperationException(sip.PropertyName + " is not of type Int16.");
            return (short)data[sip.DataVectorIndex][sip.DataVectorSection];
        }
        /// <summary>
        /// Overloaded. Initializes the value for a specific property.
        /// </summary>
        /// <param name="sip">A <see cref="StyleInfoProperty"/> that identifies the property to operate on.</param>
        /// <param name="value">The value to be saved for the property.</param>
        public void SetValue(StyleInfoProperty sip, object value)
        {
            if (sip.ObjectStoreKey != -1)
            {
                this.FixVectorCount();
                _SetObject(sip.ObjectStoreKey, value, sip.IsDisposable);
                include[sip.BitVectorIndex][sip.BitVectorMask] = true;
                if (recordChanges) changed[sip.BitVectorIndex][sip.BitVectorMask] = true;
            }
            else if (sip.ExpandableObjectStoreKey != -1)
            {
                this.FixVectorCount();
                _SetExpandableObject(sip.ExpandableObjectStoreKey, value);
                include[sip.BitVectorIndex][sip.BitVectorMask] = true;
                if (recordChanges) changed[sip.BitVectorIndex][sip.BitVectorMask] = true;
            }
            else
                SetValue(sip, Convert.ToInt16(value));
        }
        /// <summary>
        /// Initializes the <see cref="System.Int16"/> value for a specific property.
        /// </summary>
        /// <param name="sip">A <see cref="StyleInfoProperty"/> that identifies the property to operate on.</param>
        /// <param name="value">The value to be saved for the property.</param>
        public void SetValue(StyleInfoProperty sip, short value)
        {
            this.FixVectorCount();
            if (sip.ObjectStoreKey != -1 || sip.ExpandableObjectStoreKey != -1)
                throw new InvalidOperationException(sip.PropertyName + " is not a Int16.");
            data[sip.DataVectorIndex][sip.DataVectorSection] = value;
            include[sip.BitVectorIndex][sip.BitVectorMask] = true;
            if (recordChanges) changed[sip.BitVectorIndex][sip.BitVectorMask] = true;
        }
        private bool _IsSubsetProperty(StyleInfoProperty sip, StyleInfoStore style)
        {
            if (!style.HasValue(sip))
                return true;

            if (HasValue(sip))
            {
                if (sip.ObjectStoreKey != -1)
                {
                    object obj1 = _GetObject(sip.ObjectStoreKey);
                    object obj2 = style._GetObject(sip.ObjectStoreKey);
                    if (obj1 == obj2)
                        return true;
                    else if (obj1 == null || obj2 == null)
                        return false;
                    else if (sip.IsExpandable)
                    {
                        throw new InvalidOperationException("IsExpandable");
                        //IStyleInfo esp1 = (IStyleInfo) obj1;
                        //return esp1.IsSubset(obj2 as IStyleInfo);
                    }
                    else
                        return obj1.Equals(obj2);
                }
                else if (sip.ExpandableObjectStoreKey != -1)
                {
                    object obj1 = _GetExpandableObject(sip.ExpandableObjectStoreKey);
                    object obj2 = style._GetExpandableObject(sip.ExpandableObjectStoreKey);
                    if (obj1 == obj2)
                        return true;
                    else if (obj1 == null || obj2 == null)
                        return false;
                    else if (sip.IsExpandable)
                    {
                        IStyleInfo esp1 = (IStyleInfo)obj1;
                        return esp1.IsSubset(obj2 as IStyleInfo);
                    }
                    else
                        throw new InvalidOperationException("Not IsExpandable");
                    //return obj1.Equals(obj2);
                }
                else
                {
                    return this.GetShortValue(sip) == style.GetShortValue(sip);
                }
            }

            return false;
        }
        private void _AssignProperty(StyleInfoProperty sip, StyleInfoStore style)
        {
            if (sip.ObjectStoreKey != -1 || sip.ExpandableObjectStoreKey != -1)
            {
                object obj = style.GetValue(sip);
                if (sip.IsCloneable)
                {
                    if (obj is IStyleCloneable)
                    {
                        if (((IStyleCloneable)obj).ShouldClone())
                            obj = ((IStyleCloneable)obj).Clone();
                    }
                    //else if (obj is ICloneable)
                    //    obj = ((ICloneable)obj).Clone();
                }
                SetValue(sip, obj);
            }
            else
                SetValue(sip, style.GetShortValue(sip));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sip"></param>
        /// <param name="style"></param>
        /// <returns></returns>
        private bool _EqualsProperty(StyleInfoProperty sip, StyleInfoStore style)
        {
            if (!HasValue(sip) || !style.HasValue(sip))
                return false;
            else if (sip.ObjectStoreKey != -1 || sip.ExpandableObjectStoreKey != -1)
                return _EqualsObject(this.GetValue(sip), style.GetValue(sip));
            else
                return this.GetShortValue(sip) == style.GetShortValue(sip);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sip"></param>
        /// <param name="style"></param>
        /// <param name="mt"></param>
        private void _ModifyProperty(StyleInfoProperty sip, StyleInfoStore style, StyleModifyType mt)
        {
            switch (mt)
            {
                case StyleModifyType.Exclude:
                    if (style.HasValue(sip))
                        ResetValue(sip);
                    break;
                case StyleModifyType.ApplyNew:
                    if (!HasValue(sip) && style.HasValue(sip))
                        _AssignProperty(sip, style);
                    break;
                case StyleModifyType.Override:
                    if (style.HasValue(sip))
                        _AssignProperty(sip, style);
                    break;
                case StyleModifyType.Changes:
                    if (style == null)
                        throw new ArgumentNullException("style");
                    if (style.IsValueModified(sip))
                        goto case StyleModifyType.Copy;
                    break;
                case StyleModifyType.Copy:
                    if (!style.HasValue(sip))
                        ResetValue(sip);
                    else
                        _AssignProperty(sip, style);
                    break;
            }
        }
        private void _IStyleInfoModify(StyleInfoProperty sip, StyleInfoStore style, StyleModifyType mt)
        {
            IStyleInfo esp = (IStyleInfo)_GetExpandableObject(sip.ExpandableObjectStoreKey);
            esp.ModifyStyle(style._GetExpandableObject(sip.ExpandableObjectStoreKey) as IStyleInfo, mt);
        }
        private void _ModifyExpandableProperty(StyleInfoProperty sip, StyleInfoStore style, StyleModifyType mt)
        {
            switch (mt)
            {
                case StyleModifyType.Exclude:
                    if (style.HasValue(sip) && HasValue(sip))
                        _IStyleInfoModify(sip, style, mt);
                    break;
                case StyleModifyType.ApplyNew:
                    if (!HasValue(sip) && style.HasValue(sip))
                        _AssignProperty(sip, style);
                    else if (HasValue(sip) && style.HasValue(sip))
                        _IStyleInfoModify(sip, style, mt);
                    break;
                case StyleModifyType.Override:
                    if (style.HasValue(sip))
                    {
                        if (!this.HasValue(sip))
                            _AssignProperty(sip, style);
                        else
                            _IStyleInfoModify(sip, style, mt);
                    }
                    break;
                case StyleModifyType.Changes:
                    if (style.IsValueModified(sip))
                    {
                        if (!style.HasValue(sip))
                            ResetValue(sip);
                        else if (!this.HasValue(sip))
                        {
                            _AssignProperty(sip, style);
                        }
                        else
                            _IStyleInfoModify(sip, style, mt);
                    }
                    break;
                case StyleModifyType.Copy:
                    if (!style.HasValue(sip))
                        ResetValue(sip);
                    else
                        _AssignProperty(sip, style);
                    break;
            }
        }
        #region ObjectValues
        private static bool _EqualsObject(object obj1, object obj2)
        {
            return obj1 == obj2
              || obj1 != null && obj2 != null && obj1.Equals(obj2);
        }
        private object _GetObject(int key)
        {
            bool found;
            return objects == null ? null : objects.GetObject(key, out found);
        }
        private void _SetObject(int key, object value, bool isDisposable)
        {
            // Dispose previous object (if any).
            if (objects == null)
                objects = new StyleInfoObjectStore();
            object oldObject = objects.GetObject(key);
            if (oldObject != value)
            {
                if (isDisposable)
                    _Dispose(oldObject);
                objects.SetObject(key, value);
            }
        }
        private void _ResetObject(int key, bool isDisposable)
        {
            if (objects != null)
            {
                object obj = objects.GetObject(key) as IDisposable;
                objects.RemoveObject(key);
                if (isDisposable)
                    _Dispose(obj);
            }
        }
        void _Dispose(object oldObject)
        {
            IStyleCloneable scObject = oldObject as IStyleCloneable;
            if (scObject != null)
            {
                if (scObject.ShouldDispose())
                    scObject.Dispose();
            }
            else
            {
                IDisposable disposableObject = oldObject as IDisposable;
                if (disposableObject != null)
                    disposableObject.Dispose();
            }
        }
        private object _GetExpandableObject(int key)
        {
            bool found;
            return expandableObjects == null ? null : expandableObjects.GetObject(key, out found);
        }
        private void _SetExpandableObject(int key, object value)
        {
            // Dispose previous object (if any).
            if (expandableObjects == null)
                expandableObjects = new StyleInfoObjectStore();
            object oldObject = expandableObjects.GetObject(key);
            if (oldObject != value)
            {
                IDisposable disposableObject = oldObject as IDisposable;
                if (disposableObject != null)
                    disposableObject.Dispose();
                expandableObjects.SetObject(key, value);
            }
        }
        private void _ResetExpandableObject(int key)
        {
            if (expandableObjects != null)
            {
                IDisposable disposableObject = expandableObjects.GetObject(key) as IDisposable;
                expandableObjects.RemoveObject(key);
                if (disposableObject != null)
                    disposableObject.Dispose();
            }
        }
        #endregion

        private void _CopyStyle(StyleInfoStore style)
        {
            data = (BitVector32[])style.data.Clone();
            include = (BitVector32[])style.include.Clone();
            changed = (BitVector32[])style.changed.Clone();
            objects = style.objects != null ? new StyleInfoObjectStore() : null;
            expandableObjects = style.expandableObjects != null ? new StyleInfoObjectStore() : null;
            StaticData sd = StaticDataStore;
            for (int n = 0; n < sd.styleInfoProperties.Count; n++)
            {
                StyleInfoProperty sip = (StyleInfoProperty)sd.styleInfoProperties[n];
                if (sip.ObjectStoreKey != -1)
                {
                    if (HasValue(sip))
                        objects.SetObject(sip.ObjectStoreKey, objects.GetObject(sip.ObjectStoreKey));
                }
                else if (sip.ExpandableObjectStoreKey != -1)
                {
                    if (HasValue(sip))
                        expandableObjects.SetObject(sip.ExpandableObjectStoreKey, expandableObjects.GetObject(sip.ExpandableObjectStoreKey));
                }
            }
            this.FixVectorCount();

            // - Or -
            /*
                  data = new BitVector32[sd.dataVectorCount];
                  int count = sd.styleInfoProperties.Count;
                  include = new BitVector32[(count+maxBits1) / maxBits];
                  changed = new BitVector32[(count+maxBits1) / maxBits];
                  objects = new StyleInfoObjectStore();
                  for (int n = 0; n < sd.styleInfoProperties.Count; n++)
                  {
                    StyleInfoProperty sip = (StyleInfoProperty) sd.styleInfoProperties[n];
                    if (style.HasValue(sip))
                      _AssignProperty(sip, style);
                  }
            */
        }

        /// <summary>
        /// Indicates whether this is an empty object and no properties have been initialized.
        /// </summary>
        
        [XmlIgnore]
        public bool IsEmpty
        {
            get
            {
                // this piece of code was added to remove null reference exception.
                if (include == null) return true;

                for (int n = 0; n < include.Length; n++)
                    if (!include[n].Equals(new BitVector32(0)))
                        return false;
                return true;
            }
        }

        /// <summary>
        /// Indicates whether any properties have been changed.
        /// </summary>
        
        [XmlIgnore]
        public bool IsChanged
        {
            get
            {
                for (int n = 0; n < changed.Length; n++)
                    if (!changed[n].Equals(new BitVector32(0)))
                        return true;
                return false;
            }
        }

        /// <summary>
        /// Compares all properties with another style object and determines
        /// if the current set of initialized properties is a subset of
        /// the other style object.
        /// </summary>
        /// <param name="istyle">The other style to compare with.</param>
        /// <returns>True if this style object is a subset of the other style object.</returns>
        public bool IsSubset(IStyleInfo istyle)
        {
            StyleInfoStore style = istyle as StyleInfoStore;
            if (style != null && !style.IsEmpty)
            {
                StaticData sd = StaticDataStore;
                for (int n = 0; n < sd.styleInfoProperties.Count; n++)
                {
                    StyleInfoProperty sip = (StyleInfoProperty)sd.styleInfoProperties[n];
                    if (!this._IsSubsetProperty(sip, style))
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Applies changes to a style object as specified with <see cref="StyleModifyType"/>.
        /// </summary>
        /// <param name="istyle">The style object to be applied on the current object.</param>
        /// <param name="mt">The actual operation to be performed.</param>
        public void ModifyStyle(IStyleInfo istyle, StyleModifyType mt)
        {
            StyleInfoStore style = istyle.Store;
            style.FixVectorCount();
            StaticData sd = StaticDataStore;
            recordChanges = true;
            for (int n = 0; n < sd.styleInfoProperties.Count; n++)
            {
                StyleInfoProperty sip = (StyleInfoProperty)sd.styleInfoProperties[n];
                if (sip.IsExpandable)
                    this._ModifyExpandableProperty(sip, style, mt);
                else
                    this._ModifyProperty(sip, style, mt);
            }
            recordChanges = false;
        }
        /// <summary>
        /// Applies changes to a style object as specified with <see cref="StyleModifyType"/>. If a property is modified
        /// its Changed flag is set so that the parent style object can identify modified properties in a subsequent Changed notification.
        /// It is called from CopyFrom and keeping changed bits is needed to properly raise
        /// StyleChanged notifications.
        /// </summary>
        /// <param name="istyle">The style object to be applied on the current object.</param>
        /// <param name="mt">The actual operation to be performed.</param>
        internal void ModifyStyleKeepChanges(IStyleInfo istyle, StyleModifyType mt)
        {
            StyleInfoStore style = istyle.Store;
            style.FixVectorCount();
            StaticData sd = StaticDataStore;
            for (int n = 0; n < sd.styleInfoProperties.Count; n++)
            {
                StyleInfoProperty sip = (StyleInfoProperty)sd.styleInfoProperties[n];
                if (!this._EqualsProperty(sip, style))
                {
                    this.SetValueModified(sip, true);
                    if (sip.IsExpandable)
                        this._ModifyExpandableProperty(sip, style, mt);
                    else
                        this._ModifyProperty(sip, style, mt);
                }
            }
        }
        /// <summary>
        /// Merges two styles. Resets all properties that differ among the two style objects
        /// and keeps only those properties that are equal.
        /// </summary>
        /// <param name="istyle">The other style object this style object should merge with.</param>
        public void MergeStyle(IStyleInfo istyle)
        {
            StyleInfoStore style = istyle as StyleInfoStore;

            StaticData sd = StaticDataStore;
            for (int n = 0; n < sd.styleInfoProperties.Count; n++)
            {
                StyleInfoProperty sip = (StyleInfoProperty)sd.styleInfoProperties[n];
                if (sip.IsExpandable)
                {
                    IStyleInfo esp = (IStyleInfo)_GetExpandableObject(sip.ExpandableObjectStoreKey);
                    if (esp != null)
                    {
                        IStyleInfo esp2 = style._GetExpandableObject(sip.ExpandableObjectStoreKey) as IStyleInfo;
                        if (esp2 != null)
                        {
                            esp.MergeStyle(esp2);
                            return;
                        }
                    }
                    this.ResetValue(sip);
                }
                else if (!this._IsSubsetProperty(sip, style) || !style._IsSubsetProperty(sip, this))
                {
                    this.ResetValue(sip);
                }
            }
        }

        #region IXmlSerializable Members

        /// <summary>
        /// Allows customization of serializing the StyleInfoProperty. Returns True if you override
        /// this method and do not want default serialization behavior for this property.
        /// </summary>
        protected virtual bool ProcessWriteXml(XmlWriter writer, StyleInfoProperty sip)
        {
            return sip.ProcessWriteXml(writer, this);
        }

        /// <summary>
        /// Allows customization of serializing the StyleInfoProperty. Returns True if you override
        /// this method and do not want default serialization behavior for this property.
        /// </summary>
        protected virtual bool ProcessReadXml(XmlReader reader, StyleInfoProperty sip)
        {
            return sip.ProcessReadXml(reader, this);
        }

        void XmlSerialize(XmlWriter writer, object value, Type type, bool anyType)
        {
            XmlSerializer serializer;
            if (xmlSerializers.ContainsKey(type.FullName))
                serializer = (XmlSerializer)xmlSerializers[type.FullName];
            else
            {
                if (value == null || anyType)
                    serializer = new XmlSerializer(typeof(object), new Type[] { type });
                else
                    serializer = new XmlSerializer(type);
                xmlSerializers[type.FullName] = serializer;
            }
            serializer.Serialize(writer, value);
        }

        SerializeXmlBehavior GetDefaultSerializeXmlBehavior(StyleInfoProperty sip)
        {
            SerializeXmlBehavior sb = sip.SerializeXmlBehavior;
            if (sb == SerializeXmlBehavior.Default)
            {
                if (sip.ObjectStoreKey == -1
                  || sip.PropertyType == typeof(Type)
                  || sip.PropertyType == typeof(string)
#if !WinRT
                  || sip.PropertyType.IsPrimitive
#endif
                  )
                {
                    sb = SerializeXmlBehavior.SerializeAsString;
                }
                else
                {
                    sb = SerializeXmlBehavior.SerializeWithXmlSerializer;
                }
            }
            return sb;
        }

        /// <summary>
        /// Serializes all properties of this object to XML.
        /// </summary>
        public void WriteXml(XmlWriter writer)
        {
            foreach (StyleInfoProperty sip in StyleInfoProperties)
            {
                if (sip.IsSerializable && this.HasValue(sip))
                {
                    //Trace.WriteLine(sip.PropertyName + ": " + sip.PropertyType.Name);

                    //if (sip.PropertyName == "CellValueType")
                    //    Trace.WriteLine(sip.PropertyName + ": " + sip.PropertyType.Name);

                    if (ProcessWriteXml(writer, sip))
                    {
                    }
                    else if (sip.IsExpandable)
                    {
                        // <PropertyName>
                        writer.WriteStartElement(sip.PropertyName);

                        StyleInfoStore styleInfoStore = this.GetValue(sip) as StyleInfoStore;
                        if (styleInfoStore != null)
                            styleInfoStore.WriteXml(writer);
                        // XmlSerialize(writer, styleInfoStore, styleInfoStore.GetType(), false);

                        // <PropertyName/>
                        writer.WriteEndElement();
                    }
                    else
                    {
                        if (sip.SerializeXmlBehavior == SerializeXmlBehavior.Skip)
                            continue;

                        object value = GetValue(sip);

                        // <PropertyName>
                        writer.WriteStartElement(sip.PropertyName);

                        SerializeXmlBehavior sb = GetDefaultSerializeXmlBehavior(sip);

                        if (sb == SerializeXmlBehavior.SerializeAsString)
                        {
                            if (sip.PropertyType == typeof(Type))
                                writer.WriteString(ValueConvert.GetTypeName(value as Type));
                            else if (sip.PropertyType == typeof(bool))
#if !WinRT
                                writer.WriteString(sip.FormatValue(value).ToLower(CultureInfo.InvariantCulture));
#else
                                writer.WriteString(sip.FormatValue(value).ToLower());
#endif
                            else
                                writer.WriteString(sip.FormatValue(value));
                        }
                        else if (sb == SerializeXmlBehavior.SerializeWithXmlSerializer)
                        {
                            try
                            {
                                Type type = value != null ? value.GetType() : sip.PropertyType;
                                bool anyType = type != sip.PropertyType;
                                if (anyType)
                                {
                                    // Write out a list of external types. The types are needed when
                                    // reading back in the values. A XmlSerializer is created based on these
                                    // types when reading back in the contents of this node.
#if !WinRT
                                    if (!type.IsPrimitive && type.Module != typeof(object).Module)
#else
                                    if (!type.GetTypeInfo().IsPrimitive && type.GetTypeInfo().Module != typeof(object).GetTypeInfo().Module)
#endif
                                    {
                                        writer.WriteStartElement("Type");
                                        writer.WriteString(ValueConvert.GetTypeName(type));
                                        writer.WriteEndElement();
                                    }
                                }

                                XmlSerialize(writer, value, type, anyType);
                            }
                            catch (InvalidOperationException ex)
                            {
                                // TraceUtil.TraceExceptionCatched(ex);
                                Debug.WriteLine(ex.Message);
                                if (ex.InnerException != null)
                                    Debug.WriteLine(ex.InnerException.Message);
                                sip.SerializeXmlBehavior = SerializeXmlBehavior.Skip;
                                writer.WriteString(sip.FormatValue(value));
                            }
                        }

                        // <PropertyName/>
                        writer.WriteEndElement();
                    }
                    //Trace.WriteLine(sip.PropertyName + ": Success.");
                }
            }
        }

        static Hashtable xmlSerializers = new Hashtable();

        /// <summary>
        /// Registers the XmlSerializer for a specific type. This XmlSerializer will
        /// be used when a object of the specified type is read back in from an xml stream. 
        /// You can for example register an "ImageHolder" serializer for a custom ImageHolder type
        /// and have that serializer be used when GridStyleInfo.CellValue contains an ImageHolder object.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="xmlSerializer"></param>
        /// <example>
        /// <code>
        /// XmlSerializer imageHolderSerializer = new XmlSerializer(typeof(object), new Type[] { typeof(ImageHolder) });
        /// GridStyleInfoStore.RegisterXmlSerializer(typeof(ImageHolder), imageHolderSerializer);
        /// </code>
        /// </example>
        public static void RegisterXmlSerializer(Type type, XmlSerializer xmlSerializer)
        {
            xmlSerializers[type.FullName] = xmlSerializer;
        }

        /// <summary>
        /// Serializes all properties of this object from XML.
        /// </summary>
        public void ReadXml(XmlReader r)
        {
#if (!SILVERLIGHT && !WinRT)
            XmlTextReader xtr = r as XmlTextReader;
#endif

            if (r.IsEmptyElement)
            {
                r.Read();
                return;
            }

            //if (r.GetAttribute("xsi:nil") == "true")
            //	return;

            Hashtable externalTypes = null;

            r.Read(); // consume container start tag
            while (!r.EOF && r.NodeType != XmlNodeType.EndElement)
            {
                //Trace.WriteLine(r.Name + ": " + r.NodeType.ToString());
                if (r.NodeType == XmlNodeType.Element)
                {
                    StyleInfoProperty sip = FindStyleInfoProperty(r.Name);
                    if (sip != null)
                    {

                        if (ProcessReadXml(r, sip))
                        {
                        }
                        else if (sip.IsExpandable)
                        {
                            StyleInfoBase subObject;
                            if (sip.CreateObject != null)
                                subObject = (StyleInfoBase)sip.CreateObject(null, null);
                            // fall back on Reflection if no CreateObject method was supplied (slow !!!)
                            else
                                subObject = (StyleInfoBase)Activator.CreateInstance(sip.PropertyType,
                                  new object[] { });
                            StyleInfoStore store = subObject.Store;
                            store.ReadXml(r);
                            //Trace.WriteLine(store.ToString());
                            this.SetValue(sip, store);
                            // StyleInfoStore.ReadXml already consumes end tag
                            // --> Don't: r.Read();
                        }
                        else
                        {
                            if (sip.SerializeXmlBehavior == SerializeXmlBehavior.Skip)
                            {
                                string s = r.ReadInnerXml();
#if (!SILVERLIGHT && !WinRT)
                                r.ReadEndElement(); // consume item end tag.
#else
#endif
                                //Trace.WriteLine(sip.PropertyName + ": Skipped" + s);
                            }
                            else
                            {
                                string xmlValue;
                                XmlSerializer serializer = null;
                                object value;

                                SerializeXmlBehavior sb = GetDefaultSerializeXmlBehavior(sip);

                                if (sb == SerializeXmlBehavior.SerializeAsString)
                                {
#if (!SILVERLIGHT && !WinRT)
                                    xmlValue = r.ReadString();
#else
                                    xmlValue = r.ReadElementContentAsString();
#endif
                                    if (sip.PropertyType == typeof(Type))
                                        value = ValueConvert.GetType(xmlValue);
                                    else
                                        value = sip.ParseValue(xmlValue);
                                    this.SetValue(sip, value);
#if (!SILVERLIGHT && !WinRT)
                                    r.ReadEndElement(); // consume item end tag.
#else
#endif
                                }
                                else if (sb == SerializeXmlBehavior.SerializeWithXmlSerializer)
                                {
                                    try
                                    {
                                        while (r.NodeType == XmlNodeType.Whitespace)
                                            r.Read();
                                        r.ReadStartElement(); // consume container start tag and
                                        // skip whitespace if xml document is formatted with indents
                                        while (r.NodeType == XmlNodeType.Whitespace)
                                            r.Read();

                                        // Read external types that define the Serializer for subsequent node.
                                        while (r.Name == "Type")
                                        {
                                            serializer = null;
#if (!SILVERLIGHT && !WinRT)
                                            string typeName = r.ReadString();
#else
                                            string typeName = r.ReadElementContentAsString();
#endif
                                            if (externalTypes == null)
                                                externalTypes = new Hashtable();
                                            if (typeName.EndsWith(".exe") || typeName.EndsWith(".dll"))
                                                typeName = typeName.Substring(0, typeName.Length - 4);
                                            if (!externalTypes.ContainsKey(typeName))
                                            {
                                                externalTypes[typeName] = ValueConvert.GetType(typeName);
                                            }
                                            r.ReadEndElement();
                                            while (r.NodeType == XmlNodeType.Whitespace)
                                                r.Read();
                                        }

                                        // Create serializer based on <Type> tags that we just read in.
                                        if (r.AttributeCount > 0 && externalTypes != null)
                                        {
                                            Type[] types = new Type[externalTypes.Count];
                                            externalTypes.Values.CopyTo(types, 0);
                                            if (types.Length == 1 && xmlSerializers.ContainsKey(types[0].FullName))
                                                serializer = (XmlSerializer)xmlSerializers[types[0].FullName];
                                            else
                                                serializer = new XmlSerializer(sip.PropertyType, types);
                                        }
                                        else
                                        {
                                            if (xmlSerializers.ContainsKey(sip.PropertyType.FullName))
                                                serializer = (XmlSerializer)xmlSerializers[sip.PropertyType.FullName];
                                            else
                                                serializer = new XmlSerializer(sip.PropertyType);
                                        }

                                        value = serializer.Deserialize(r);

#if (!SILVERLIGHT && !WinRT)
                                        // Workaround for DBNull not being recognized by "AnyObject" serializer.
                                        if (value != null && value.GetType().FullName == "System.Xml.XmlNode[]")
                                        {
                                            XmlNode[] nodes = (XmlNode[])value;
                                            if (nodes.Length > 0 && nodes[0] is XmlAttribute)
                                            {
                                                XmlAttribute att = (XmlAttribute)nodes[0];
                                                if (att.Value == "DBNull")
                                                    value = DBNull.Value;
                                            }
                                        }                                        
#endif
                                        this.SetValue(sip, value);
                                        //Trace.WriteLine(sip.PropertyName + ": " + (value != null ? value.ToString() : "null"));
                                        r.ReadEndElement(); // consume object end tag.
                                        while (r.NodeType == XmlNodeType.Whitespace)
                                            r.Read();
                                        if (r.NodeType == XmlNodeType.EndElement && r.Name == sip.PropertyName)
                                        {
#if (!SILVERLIGHT && !WinRT)
                                            r.ReadEndElement(); // consume item end tag.
#else

#endif
                                        }
                                    }
                                    catch (InvalidOperationException)
                                    {
                                        // TraceUtil.TraceExceptionCatched(ex);
                                        sip.SerializeXmlBehavior = SerializeXmlBehavior.Skip;
                                        //Trace.WriteLine(sip.PropertyName + ": skipped");
                                        r.ReadEndElement(); // consume object end tag.
                                        while (r.NodeType == XmlNodeType.Whitespace)
                                            r.Read();
                                        if (r.NodeType == XmlNodeType.EndElement && r.Name == sip.PropertyName)
                                            r.ReadEndElement(); // consume item end tag.
                                    }
                                }
                            }

                        }
                    }
                    else
                    {
                        r.Read();
                    }
                }
                else
                {
                    r.Read();
                }
            }
            //Trace.WriteLine(r.Name + ": " + r.NodeType.ToString());
            if (!r.EOF)
                r.Read();
        }

        /// <internalonly/>
        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }
        #endregion

    }

    /// <summary>
    /// Specifies the options for style properties.
    /// </summary>
    public enum StyleInfoPropertyOptions
    {
        /// <summary>
        /// None.
        /// </summary>
        None = 0,
        /// <summary>
        /// The property supports serialization.
        /// </summary>
        Serializable = 1,
        /// <summary>
        /// The property should be cloned when the parent style object is copied.
        /// </summary>
        Cloneable = 2,
        /// <summary>
        /// The property should be disposed when the parent style object is disposed.
        /// </summary>
        Disposable = 4,
        /// <summary>
        /// The property should be disposed when the parent style object is disposed
        /// and cloned when the parent style object is copied.
        /// </summary>
        CloneableAndDisposable = 6,
        /// <summary>
        /// All of the above.
        /// </summary>
        All = 7,
    }

}

