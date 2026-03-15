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
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using Syncfusion.Windows.ComponentModel;
using Syncfusion.Windows.GridCommon;
using BitVector32 = Syncfusion.Windows.Collections.BitVectorInt32;

namespace Syncfusion.Windows.Styles
{
    /// <summary>
    /// Provides a unique identifier for a property in a style object and stores
    /// information about the associated property.
    /// </summary>
    /// <remarks>
    /// <see cref="StyleInfoProperty"/> is allocated once on the global heap for each
    /// property in a style object. Type safe <see cref="StyleInfoBase"/> wrappers make
    /// use of StyleInfoProperty to query, set or reset specific properties.
    /// <para/>
    /// Holds information about the property: PropertyType, Name, how to load and save
    /// its state in StyleInfoStore, attributes, etc.
    /// </remarks>
    public class StyleInfoProperty
    {
        /// <summary>
        /// The type of the component this property is bound to.
        /// </summary>
        public Type ComponentType;

        /// <summary>
        /// The type of the property.
        /// </summary>
        public Type PropertyType;

        /// <summary>
        /// The name of the property.
        /// </summary>
        public string PropertyName;

        // What about Category, Description? - could be used with ICustomTypeDescriptor
        // Custom Attributes (Browsable, Editor, Visibitlty etc.)

        /// <summary>
        /// Represents a method that creates a Subobject of this type and associates it with a parent style object.
        /// </summary>
        public CreateSubObjectHandler CreateObject;

        /// <summary>
        /// Indicates whether this is an object derived from <see cref="StyleInfoSubObjectBase"/>.
        /// </summary>
        public bool IsExpandable;

        /// <summary>
        /// Indicates whether this object supports being serialized. The value can be set
        /// with a <see cref="SerializePropertyAttribute"/> in your class implementation.
        /// </summary>
        public bool IsSerializable;

        /// <summary>
        /// Indicates whether this object should call ICloneable.Clone when an assigned object implements
        /// the ICloneable or <see cref="IStyleCloneable"/> interface. The value can be set
        /// with a <see cref="CloneablePropertyAttribute"/> in your class implementation.
        /// </summary>
        public bool IsCloneable;

        /// <summary>
        /// Indicates whether this object should call ICloneable.Clone when an assigned object implements
        /// the IDisposable or <see cref="IStyleCloneable"/> interface. The value can be set
        /// also with a <see cref="DisposeablePropertyAttribute"/> in your class implementation.
        /// </summary>
        public bool IsDisposable;

        /// <summary>
        /// If property is marked with Browsable(true), custom attribute of this flag allows you
        /// to specify whether the property should appear in PropertyGrid. If property is marked
        /// with Browsable(false), then this setting will have no effect. Default is true.
        /// </summary>
        public bool IsBrowsable = true;

        /// <summary>
        /// Indicates whether type information should be included when <see cref="Format"/> is called. Use
        /// this if PropertyType is System.Obbject and you want to be able to parse written values.
        /// An example is GridStyleInfo.CellValue. Default is false.
        /// </summary>
        public bool IsAnyObject = false;

        /// <summary>
        /// Indicates whether type information should be converted to Base64 if associated
        /// Type converter supports converting value to and from Base64. An example is a bitmap assigned to GridStyleInfo.CellValue.
        /// If IsConvertibleToBase64String is true, then the grid will convert the bitmap to a Base64 string
        /// and write out the information in the GridStyleInfo.ToString() method and also in the WriteXml method. Default is false.
        /// </summary>
        public bool IsConvertibleToBase64 = false;

        /// <summary>
        /// Defines how to serialize property when style data is serialized to or from an XML stream
        /// with <see cref="XmlSerializer"/>.
        /// </summary>
        public SerializeXmlBehavior SerializeXmlBehavior = SerializeXmlBehavior.Default;


        PropertyInfo propertyInfo;

        /// <summary>
        /// Returns the <see cref="PropertyInfo"/> of this property.
        /// </summary>
        /// <returns>Property info of this property.</returns>
        public PropertyInfo GetPropertyInfo()
        {
            if (propertyInfo == null && this.ComponentType != null)
                propertyInfo = ComponentType.GetProperty(this.PropertyName);
            return propertyInfo;
        }

        PropertyDescriptor propertyDesc = null;
        public PropertyDescriptor GetPropertyDescriptor()
        {
            if (this.propertyDesc == null && this.ComponentType != null)
            {
                this.propertyDesc = TypeDescriptor.GetProperties(this).Find(this.PropertyName, false);
            }

            return this.propertyDesc;
        }

        // Property can be stored in a bitvectormanaging your documentation projects

        // BitVector specific ->
        /// <exclude/>
        public int DataVectorIndex = -1;
        /// <exclude/>
        public BitVector32.Section DataVectorSection;
        /// <exclude/>
        public short MaxValue = 0;
        // <-

        // or PropertyStore

        // PropertyStore specific ->
        /// <exclude/>
        public int ObjectStoreKey = -1;
        /// <exclude/>
        public int ExpandableObjectStoreKey = -1;
        /// <exclude/>
        public int PropertyKey = -1;
        // <-

        // Unique id for include bit, changed bit.
        /// <exclude/>
        public int BitVectorMask = -1;
        /// <exclude/>
        public int BitVectorIndex = 0;

        /// <summary>
        /// Set this for "behind the scenes" properties that when changed
        /// do not affect the appearance of a cell.
        /// </summary>
        public bool DoNotInvalidateCellWhenChanged = false;

        // Index in ArrayListmanaging your documentation projects
        /// <exclude/>
        public int Index = -1;

        /// <summary>
        /// Handles parse requests for this property.
        /// </summary>
        public event StyleInfoPropertyConvertEventHandler Parse;

        /// <summary>
        /// Handles formatting requests for this property.
        /// </summary>
        public event StyleInfoPropertyConvertEventHandler Format;

        /// <summary>
        /// Initializes a new StyleInfoProperty.
        /// </summary>
        internal StyleInfoProperty(Type type, string name, short maxValue, Type componentType)
        {
            PropertyType = type;
            PropertyName = name;
            MaxValue = maxValue;
            IsExpandable = !type.IsPrimitive && typeof(IStyleInfo).IsAssignableFrom(type);
            ComponentType = componentType;
            // TODO: PropertyType could be determined by looking up PropertyInfo.PropertyType.
            PropertyKey = CreatePropertyKey();
        }

        static int currentKey = 0;

        private int CreatePropertyKey()
        {
            return currentKey++;
        }

        /// <override/>
        public override string ToString()
        {
            if (PropertyName != null && PropertyType != null)
                return GetType().Name + " { " + PropertyName + " (" + PropertyType.Name + ") }";
            return base.ToString();
        }


        /// <summary>
        /// Formats a given value that is of the same type as the <see cref="PropertyType"/>.
        /// </summary>
        /// <param name="value">The value to be formatted.</param>
        /// <returns>A string with formatted text.</returns>
        /// <remarks><see cref="StyleInfoStore.ToString"/> calls this method.
        /// <para/>
        /// The <see cref="Format"/> event lets you customize the formatting of this property but care
        /// should be taken that the formatted text can be consumed by the <see cref="ParseValue(string)"/>
        /// method.
        /// </remarks>
        public string FormatValue(object value)
        {
            if (Format != null)
            {
                StyleInfoPropertyConvertEventArgs cea = new StyleInfoPropertyConvertEventArgs(value, typeof(string));
                Format(this, cea);
                if (cea.Handled)
                    return cea.Value.ToString();
            }

            if (IsAnyObject)
                return ValueConvert.FormatValueWithTypeInformation(value, IsConvertibleToBase64);
            else if (value == null)
                return "";
            else if (PropertyType.IsEnum)
            {
                if (value is int)
                    return Enum.Format(PropertyType, (int) value, "G");
                else if (value is Enum)
                    return Enum.Format(PropertyType, (Enum) value, "G");
                else
                    return Enum.Format(PropertyType, (int) (short) value, "G");
            }
            else if (PropertyType == typeof(bool))
                return Convert.ToString(Convert.ToBoolean(value));
            else if (PropertyType == typeof(string))
                return "\"" + value.ToString() + "\"";
            else
                return value.ToString();
        }

        /// <summary>
        /// Overloaded. Parses a given value that is of the same type as the <see cref="PropertyType"/>.
        /// </summary>
        /// <param name="s">The string to be parsed.</param>
        /// <returns>An object of the same type as the <see cref="PropertyType"/> created from the parsed string information.</returns>
        /// <remarks>
        /// <see cref="StyleInfoBase.ParseString"/> calls this method.
        /// <para/>
        /// The <see cref="Parse"/> event lets you customize the parsing of this property.
        /// </remarks>
        public object ParseValue(string s)
        {
            if (Parse != null)
            {
                StyleInfoPropertyConvertEventArgs cea = new StyleInfoPropertyConvertEventArgs(s, this.PropertyType);
                Parse(this, cea);
                if (cea.Handled)
                    return cea.Value;
            }

            if (IsAnyObject)
            {
                string valueAsString = s != null ? s.Trim() : "";
                object value;
                if (ValueConvert.ParseValueWithTypeInformation(valueAsString, out value, IsConvertibleToBase64))
                    return value;
            }

            return ParseValue(s, this.PropertyType, null);
        }

        /// <summary>
        /// Parses a given value and converts it to a requested <see cref="Type"/>.
        /// </summary>
        /// <param name="s">The string to be parsed.</param>
        /// <param name="resultType">The <see cref="Type"/> for the resulting object.</param>
        /// <param name="provider">A format provider.</param>
        /// <returns>An object of type "resultType" created from the parsed string information.</returns>
        /// <remarks>
        /// The <see cref="Parse"/> event lets you customize the parsing of this property.
        /// </remarks>
        public static object ParseValue(string s, Type resultType, IFormatProvider provider)
        {
            object result;

            if (s == null || s.Length == 0)
            {
                result = Convert.DBNull;
                return result;
            }

            if (resultType == typeof(string))
            {
                if (s.Length >= 2 && s.StartsWith("\"") && s.EndsWith("\""))
                    return s.Length > 2 ? s.Substring(1, s.Length - 2) : "";
                return s;
            }

            if (resultType == typeof(Type))
            {
                result = Type.GetType(s);
                return result;
            }

            MethodInfo parseMethod = resultType.GetMethod("Parse", new Type[] { typeof(string), typeof(IFormatProvider) });

            if (parseMethod != null)
            {
                if (resultType.FullName == "System.Double" && s == "NaN")
                    return double.NaN;
                result = parseMethod.Invoke(null, new object[] { s, provider });
                return result;
            }

            TypeConverter typeConverter = TypeDescriptor.GetConverter(resultType);
            if (typeConverter != null &&
                typeConverter.CanConvertFrom(typeof(System.String)))
            {
                result = typeConverter.ConvertFrom(s);
                return result;
            }

            // throw new InvalidCastException(OGSR.GetString("InvalidCast_IConvertible"));
            return Convert.DBNull;
        }

        /// <summary>
        /// Handles requests to serialize this property to an XML stream during an
        /// <see cref="XmlSerializer.Serialize"/> operation of the <see cref="XmlSerializer"/>
        /// class.
        /// </summary>
        /// <remarks>
        /// This event allows you to implement a different serialization mechanism if many style objects
        /// reference and share the same object, (e.g. if you assign a DataSet to
        /// several objects DataSource property). With such a scenario, you could write out an
        /// identifier and when the property is deserialized from an XML stream, you could
        /// reconstruct a reference to a datasource object based on the identifier.
        /// </remarks>
        public event StyleInfoPropertyWriteXmlEventHandler WriteXml;

        /// <summary>
        /// Handles requests to deserialize this property from an XML stream during an
        /// <see cref="XmlSerializer.Deserialize"/> operation of the <see cref="XmlSerializer"/>
        /// class.
        /// </summary>
        /// <remarks>
        /// This event allows you to implement a different serialization mechanism if many style objects
        /// reference and share the same object, (e.g. if you assign a DataSet to
        /// several objects DataSource property). With such a scenario, you could write out an
        /// identifier and when the property is deserialized from an XML stream you could
        /// reconstruct a reference to a datasource object based on the identifier.
        /// </remarks>
        public event StyleInfoPropertyReadXmlEventHandler ReadXml;

        internal bool ProcessWriteXml(XmlWriter writer, StyleInfoStore store)
        {
            if (WriteXml != null)
            {
                StyleInfoPropertyWriteXmlEventArgs e = new StyleInfoPropertyWriteXmlEventArgs(writer, store, this);
                WriteXml(this, e);
                return e.Handled;
            }
            return false;
        }

        internal bool ProcessReadXml(XmlReader reader, StyleInfoStore store)
        {
            if (ReadXml != null)
            {
                StyleInfoPropertyReadXmlEventArgs e = new StyleInfoPropertyReadXmlEventArgs(reader, store, this);
                ReadXml(this, e);
                return e.Handled;
            }
            return false;
        }

    }


    /// <summary>
    /// Defines how to serialize property when style data is serialized to or from an XML stream.
    /// with <see cref="XmlSerializer"/>.
    /// </summary>
    public enum SerializeXmlBehavior
    {
        /// <summary>
        /// Default. Serialize as string when type is simple. Using XmlSerializer for complex types
        /// or properties where the actual type is not known at compile-time (e.g. CellValue).
        /// </summary>
        Default,

        /// <summary>
        /// Skip this property. Do not serialize.
        /// </summary>
        Skip,

        /// <summary>
        /// Serialize this property as string using <see cref="StyleInfoProperty.FormatValue"/> and
        /// <see cref="StyleInfoProperty.ParseValue(string)"/>.
        /// </summary>
        SerializeAsString,

        /// <summary>
        /// Serialize this property using <see cref="XmlSerializer"/>.
        /// </summary>
        SerializeWithXmlSerializer,
    }

    /// <summary>
    /// Provides data for the <see cref="StyleInfoProperty.Format"/> and <see cref="StyleInfoProperty.Parse"/> events.
    /// </summary>
    /// <remarks>
    /// The <see cref="StyleInfoPropertyConvertEventArgs"/> is used to format and unformat values represented by
    /// a property in a <see cref="StyleInfoStore"/> object. The Format event occurs whenever a property
    /// is written out as string and the Parse event occurs whenever the value is read back in from a string.
    /// <para/>
    /// If you handle this event, store the resulting value into <see cref="Value"/> and
    /// set <see cref="SyncfusionHandledEventArgs.Handled"/> to True.
    /// </remarks>
    public sealed class StyleInfoPropertyConvertEventArgs : SyncfusionHandledEventArgs
    {
        Type desiredType;
        object value;

        /// <summary>
        /// Initializes a new instance of the <see cref="StyleInfoPropertyConvertEventArgs"/> class.
        /// </summary>
        /// <param name="value">An Object that contains the value of the current property.</param>
        /// <param name="desiredType">The Type of the value.</param>
        public StyleInfoPropertyConvertEventArgs(object value, Type desiredType)
        {
            this.desiredType = desiredType;
            this.value = value;
        }

        /// <summary>
        /// Gets / sets the value of the <see cref="StyleInfoPropertyConvertEventArgs"/> object.
        /// </summary>
        [TraceProperty(true)]
        public object Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = value;
            }
        }

        /// <summary>
        /// Returns the data type of the desired value.
        /// </summary>
        /// <remarks>
        /// The DesiredType property enables you to check the type of the property that the value is being converted to.
        /// </remarks>
        [TraceProperty(true)]
        public Type DesiredType
        {
            get
            {
                return desiredType;
            }
        }
    }

    /// <summary>
    /// Handles the <see cref="StyleInfoProperty.Format"/> or <see cref="StyleInfoProperty.Parse"/> event.
    /// </summary>
    public delegate void StyleInfoPropertyConvertEventHandler(object sender, StyleInfoPropertyConvertEventArgs e);


    /// <summary>
    /// Handles the <see cref="StyleInfoProperty.WriteXml"/> event of <see cref="StyleInfoProperty"/>.
    /// </summary>
    public delegate void StyleInfoPropertyWriteXmlEventHandler(object sender, StyleInfoPropertyWriteXmlEventArgs e);

    /// <summary>
	/// Provides data for the <see cref="StyleInfoProperty.WriteXml"/> event.
    /// </summary>
    /// <remarks>
    /// The <see cref="StyleInfoPropertyWriteXmlEventArgs"/> is used to serialize a property of a
    /// <see cref="StyleInfoStore"/> object. The WriteXml event occurs whenever a property
    /// is serialized to an XML stream during an <see cref="XmlSerializer.Serialize"/> operation
    /// of the <see cref="XmlSerializer"/> class.
    /// <para/>
    /// If you handle this event, you set <see cref="SyncfusionHandledEventArgs.Handled"/> to True.
    /// Handling this event allows you to customize the way the object is serialized or skip
    /// serialization.
    /// <para/>
    /// This event allows you to implement a different serialization mechanism if many style objects
    /// reference and share the same object, (e.g. if you assign a DataSet to
    /// several objects' DataSource property). With such a scenario, you could write out a
    /// identifier and when the property is deserialized from an XML stream, you could
    /// reconstruct a reference to a datasource object based on the identifier.
    /// </remarks>
    public sealed class StyleInfoPropertyWriteXmlEventArgs : SyncfusionHandledEventArgs
    {
        XmlWriter writer;
        StyleInfoStore store;
        StyleInfoProperty sip;

        /// <summary>
        /// Initializes a new <see cref="StyleInfoPropertyWriteXmlEventArgs"/> object.
        /// </summary>
        /// <param name="writer">The <see cref="XmlWriter"/> for the XML stream.</param>
        /// <param name="store">The style object that is being serialized.</param>
        /// <param name="sip">The property that is being serialized.</param>
        public StyleInfoPropertyWriteXmlEventArgs(XmlWriter writer, StyleInfoStore store, StyleInfoProperty sip)
        {
            this.writer = writer;
            this.store = store;
            this.sip = sip;
        }

        /// <summary>
        /// Returns the <see cref="XmlWriter"/> for the XML stream.
        /// </summary>
        public XmlWriter Writer
        {
            get
            {
                return writer;
            }
        }

        /// <summary>
        /// Returns the style object that is being serialized.
        /// </summary>
        public StyleInfoStore Store
        {
            get
            {
                return store;
            }
        }

        /// <summary>
        /// Returns the property that is being serialized. Call <see cref="StyleInfoStore.GetValue"/> of
        /// the <see cref="Store"/> to get the value for this property.
        /// </summary>
        [TraceProperty(true)]
        public StyleInfoProperty Sip
        {
            get
            {
                return sip;
            }
        }
    }

    /// <summary>
    /// Handles the <see cref="StyleInfoProperty.ReadXml"/> event of <see cref="StyleInfoProperty"/>.
    /// </summary>
    public delegate void StyleInfoPropertyReadXmlEventHandler(object sender, StyleInfoPropertyReadXmlEventArgs e);

    /// <summary>
	/// Provides data for the <see cref="StyleInfoProperty.ReadXml"/> event.
    /// </summary>
    /// <remarks>
    /// The <see cref="StyleInfoPropertyReadXmlEventArgs"/> is used to deserialize a property of a
    /// <see cref="StyleInfoStore"/> object. The ReadXml event occurs whenever a property
    /// is deserialized from an XML stream during an <see cref="XmlSerializer.Deserialize"/>
    /// operation of the <see cref="XmlSerializer"/> class.
    /// <para/>
    /// If you handle this event, you set <see cref="SyncfusionHandledEventArgs.Handled"/> to True.
    /// Call <see cref="StyleInfoStore.SetValue"/> of
    /// the <see cref="Store"/> to save the value for this property into the style object.
    /// Handling this event allows you to customize the way the object is deserialized or skip
    /// serialization.
    /// <para/>
    /// This event allows you to implement a different serialization mechanism if many style objects
    /// reference and share the same object, (e.g. if you assign a DataSet to
    /// several objects DataSource property). With such a scenario, you could write out an
    /// identifier and when the property is deserialized from an XML stream you could
    /// reconstruct a reference to a datasource object based on the identifier.
    /// </remarks>
    public sealed class StyleInfoPropertyReadXmlEventArgs : SyncfusionHandledEventArgs
    {
        XmlReader reader;
        StyleInfoStore store;
        StyleInfoProperty sip;

        /// <summary>
        /// Initializes a new <see cref="StyleInfoPropertyReadXmlEventArgs"/> object.
        /// </summary>
        /// <param name="reader">The <see cref="XmlReader"/> for the XML stream.</param>
        /// <param name="store">The style object that is deserialized.</param>
        /// <param name="sip">The property that is being deserialized.</param>
        public StyleInfoPropertyReadXmlEventArgs(XmlReader reader, StyleInfoStore store, StyleInfoProperty sip)
        {
            this.reader = reader;
            this.store = store;
            this.sip = sip;
        }

        /// <summary>
        /// Returns the <see cref="XmlReader"/> for the XML stream.
        /// </summary>
        public XmlReader Reader
        {
            get
            {
                return reader;
            }
        }

        /// <summary>
        /// Returns the style object that is deserialized.
        /// </summary>
        public StyleInfoStore Store
        {
            get
            {
                return store;
            }
        }

        /// <summary>
        /// Returns the property that is being deserialized. Call <see cref="StyleInfoStore.SetValue"/> of
        /// the <see cref="Store"/> to save the value for this property into the style object.
        /// </summary>
        [TraceProperty(true)]
        public StyleInfoProperty Sip
        {
            get
            {
                return sip;
            }
        }
    }


    internal class StyleInfoPropertyPropertyDescriptor : PropertyDescriptor
    {
        // Fields
        private StyleInfoProperty sip;
        Type type;

        // Constructor
        public StyleInfoPropertyPropertyDescriptor(StyleInfoProperty sip, Type type, Attribute[] atts)
            : base(sip.PropertyName, atts)
        {
            this.type = type;
            this.sip = sip;
        }

        // Methods

        /// <exclude/>
        /// <summary>
        ///    Indicates whether the component will allow its value to be reset.
        /// </summary>
        /// <param name="comp">The component to reset.</param>
        /// <returns>
        ///    True if the component supports resetting of its value.
        /// <seealso cref="System.ComponentModel.PropertyDescriptor"/>
        /// </returns>
        public override bool CanResetValue(object comp)
        {
            StyleInfoBase style = comp as StyleInfoBase;
            return style != null ? style.HasValue(sip) : false;
        }

        /// <exclude/>
        /// <summary>
        ///    Retrieves the value of the property for the given component. This will
        ///    throw an exception if the component does not have this property.
        /// </summary>
        /// <param name="comp">The component.</param>
        /// <returns>
        ///    The value of the property. This can be cast
        ///    to the property type.
        /// <seealso cref="System.ComponentModel.PropertyDescriptor"/>
        /// </returns>
        public override object GetValue(object comp)
        {
            StyleInfoBase style = comp as StyleInfoBase;
            return style != null ? style.GetValue(sip) : null;
        }

        /// <exclude/>
        /// <summary>
        ///    Resets the value of this property on the specified component to the default value.
        /// </summary>
        /// <param name="comp">The component whose property is to be reset.</param>
        /// <seealso cref="System.ComponentModel.PropertyDescriptor"/>
        public override void ResetValue(object comp)
        {
            StyleInfoBase style = comp as StyleInfoBase;
            if (style != null)
                style.ResetValue(sip);
        }

        /// <exclude/>
        /// <summary>
        ///    Sets the value of this property on the specified component.
        /// </summary>
        /// <param name="comp">The component whose property is to be set.</param>
        /// <param name="value">The new value of the property.</param>
        /// <seealso cref="System.ComponentModel.PropertyDescriptor"/>
        public override void SetValue(object comp, object value)
        {
            StyleInfoBase style = comp as StyleInfoBase;
            if (style != null)
                style.SetValue(sip, value);
        }

        /// <exclude/>
        /// <summary>
        ///    <para>Indicates whether this property should be persisted. A property is
        ///       to be persisted if it is marked as persistable through a
        ///       PersistableAttribute and if the property contains something other
        ///       than the default value. Note, however, that this method will
        ///       return True for design-time properties as well, so callers
        ///       should also check to see if a property is design-time only before
        ///       persisting to run-time storage.</para>
        /// </summary>
        /// <param name='comp'>The component on which the property resides.</param>
        /// <returns>
        ///    <para>True if the property should be persisted to either
        ///       design-time or run-time storage.</para>
        /// </returns>
        public override bool ShouldSerializeValue(object comp)
        {
            StyleInfoBase style = comp as StyleInfoBase;
            return style != null ? style.HasValue(sip) : false;
        }

        // Properties

        /// <exclude/>
        /// <summary>
        ///    Retrieves the type of the component this PropertyDescriptor is bound to.
        /// </summary>
        /// <returns>
        ///    The type of component.
        /// </returns>
        public override Type ComponentType
        {
            get
            {
                return type;
            }
        }

        /// <exclude/>
        /// <summary>
        ///    Retrieves the display name of the property. This is the name that will
        ///    be displayed in a property browser. This will be the same as the property
        ///    name for most properties.
        /// </summary>
        /// <returns>
        ///    A string containing the name to display in the property browser.
        /// <seealso cref="System.ComponentModel.PropertyDescriptor"/>
        /// </returns>
        public override string DisplayName
        {
            get
            {
                return sip.PropertyName;
            }
        }

        /// <exclude/>
        /// <summary>
        ///    Indicates whether the property can be written to.
        /// </summary>
        /// <returns>
        ///    True if the property can be written to.
        /// <seealso cref="System.ComponentModel.PropertyDescriptor"/>
        /// </returns>
        public override bool IsReadOnly
        {
            get
            {
                return this.Attributes[typeof(System.ComponentModel.ReadOnlyAttribute)].Equals((object) ReadOnlyAttribute.Yes);
            }
        }

        /// <exclude/>
        /// <summary>
        ///    Retrieves the data type of the property.
        /// </summary>
        /// <returns>
        ///    A class representing the data type of the property.
        /// <seealso cref="System.ComponentModel.PropertyDescriptor"/>
        /// </returns>
        public override Type PropertyType
        {
            get
            {
                return sip.PropertyType;
            }
        }
    }
}