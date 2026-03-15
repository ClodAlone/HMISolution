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
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

using Syncfusion.ComponentModel;
using Syncfusion.Drawing;

namespace Syncfusion.Styles
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
                    return Enum.Format(PropertyType, (int)value, "G");
                else if (value is Enum)
                    return Enum.Format(PropertyType, (Enum)value, "G");
                else
                    return Enum.Format(PropertyType, (int)(short)value, "G");
            }
            else if (PropertyType == typeof(bool))
                return Convert.ToString(Convert.ToBoolean(value));
            else if (PropertyType == typeof(string))
                return "\"" + value.ToString() + "\"";
            else if (PropertyType == typeof(Color))
                return ColorConvert.ColorToString((Color)value, true);
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
                return this.Attributes[typeof(System.ComponentModel.ReadOnlyAttribute)].Equals((object)ReadOnlyAttribute.Yes);
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

    /// <summary>
    /// <see cref="ValueConvert"/> provides conversion routines for values
    /// to convert them to another type and routines for formatting values.
    /// </summary>
    public class ValueConvert
    {
        protected ValueConvert()
        {
        }

        /// <overload>
        /// Converts value from one type to another using an optional <see cref="IFormatProvider"/>.
        /// </overload>
        /// <summary>
        /// Converts value from one type to another using an optional <see cref="IFormatProvider"/>.
        /// </summary>
        /// <param name="value">The original value.</param>
        /// <param name="type">The target type.</param>
        /// <param name="provider">A <see cref="IFormatProvider"/> used to format or parse the value.</param>
        /// <returns>The new value in the target type.</returns>
        public static object ChangeType(object value, Type type, IFormatProvider provider)
        {
            return ChangeType(value, type, provider, false);
        }

        /// <summary>
        /// Converts value from one type to another using an optional <see cref="IFormatProvider"/>.
        /// </summary>
        /// <param name="value">The original value.</param>
        /// <param name="type">The target type.</param>
        /// <param name="provider">A <see cref="IFormatProvider"/> used to format or parse the value.</param>
        /// <param name="returnDbNUllIfNotValid">Indicates whether exceptions should be avoided or catched and return value should be DBNull if
        /// it cannot be converted to the target type.</param>
        /// <returns>The new value in the target type.</returns>
        public static object ChangeType(object value, Type type, IFormatProvider provider, bool returnDbNUllIfNotValid)
        {

            return ChangeType(value, type, provider, "", returnDbNUllIfNotValid);
        }

        /// <summary>
        /// Converts value from one type to another using an optional <see cref="IFormatProvider"/>.
        /// </summary>
        /// <param name="value">The original value.</param>
        /// <param name="type">The target type.</param>
        /// <param name="provider">A <see cref="IFormatProvider"/> used to format or parse the value.</param>
        /// <param name="format">Format string.</param>
        /// <param name="returnDbNUllIfNotValid">Indicates whether exceptions should be avoided or catched and return value should be DBNull if
        /// it cannot be converted to the target type.</param>
        /// <returns>The new value in the target type.</returns>
        public static object ChangeType(object value, Type type, IFormatProvider provider, string format, bool returnDbNUllIfNotValid)
        {
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
            Type nullableUnderlyingType = Nullable.GetUnderlyingType(type);
            if (nullableUnderlyingType != null)
            {
                value = ChangeType(value, nullableUnderlyingType, provider, true);
                return NullableHelper.FixDbNUllasNull(value, type);
            }
#endif

            if (value != null && !type.IsAssignableFrom(value.GetType()))
            {
                try
                {
                    if (value is string)
                    {
                        if (format != null && format.Length > 0)
                            value = Parse((string)value, type, provider, format, returnDbNUllIfNotValid);
                        else
                            value = Parse((string)value, type, provider, "", returnDbNUllIfNotValid);

                    }
                    else if (value is System.DBNull)
                    {
                        // value = null; changed after 4.1.0.50: do not set it to null - this causes then issues
                        // if you have a DataTable and the key is used for lookups, e.g.
                        // see sample in http://www.syncfusion.com/support/forums/message.aspx?MessageID=40207
                        // For NullableTypes the above call to NullableHelper.FixDbNUllasNull will
                        // take care of converting DbNull to null for nullable types only.
                    }
                    else if (type.IsEnum)
                    {
                        value = Convert.ChangeType(value, typeof(int), provider);
                        value = Enum.ToObject(type, (int)value);
                    }
                    else if (type == typeof(string) && !(value is IConvertible))
                    {
                        value = value != null ? value.ToString() : "";
                    }
                    else
                        value = NullableHelper.ChangeType(value, type, provider);
                }
                catch
                {
                    if (returnDbNUllIfNotValid)
                        return Convert.DBNull;

                    throw;
                }
            }

            if ((value == null || value is DBNull) && type == typeof(string))
                return "";

            return value;
        }

        static Hashtable cachedDefaultValues = new Hashtable();

        /// <summary>
        /// Overloaded. Parses the given text using the resultTypes "Parse" method or using a type converter.
        /// </summary>
        /// <param name="s">The text to parse.</param>
        /// <param name="resultType">The requested result type.</param>
        /// <param name="provider">A <see cref="IFormatProvider"/> used to format or parse the value. Can be NULL.</param>
        /// <returns>The new value in the target type.</returns>
        static object Parse(string s, Type resultType, IFormatProvider provider)
        {
            return Parse(s, resultType, provider, "");
        }

        /// <summary>
        /// Parses the given text using the resultTypes "Parse" method or using a type converter.
        /// </summary>
        /// <param name="s">The text to parse.</param>
        /// <param name="resultType">The requested result type.</param>
        /// <param name="provider">A <see cref="IFormatProvider"/> used to format or parse the value. Can be NULL.</param>
        /// <param name="format">A format string used in a <see cref="System.Object.ToString"/> call. Right now
        /// format is only interpreted to enable roundtripping for formatted dates.
        /// </param>
        /// <returns>The new value in the target type.</returns>
        public static object Parse(string s, Type resultType, IFormatProvider provider, string format)
        {
            return Parse(s, resultType, provider, format, false);
        }

        /// <summary>
        /// Parse the given text using the resultTypes "Parse" method or using a type converter.
        /// </summary>
        /// <param name="s">The text to parse.</param>
        /// <param name="resultType">The requested result type.</param>
        /// <param name="provider">A <see cref="IFormatProvider"/> used to format or parse the value. Can be NULL.</param>
        /// <param name="format">A format string used in a <see cref="System.Object.ToString"/> call. Right now
        /// format is only interpreted to enable roundtripping for formatted dates.
        /// </param>
        /// <param name="returnDbNUllIfNotValid">Indicates whether DbNull should be returned if value cannot be parsed. Otherwise an exception is thrown.</param>
        /// <returns>The new value in the target type.</returns>
        public static object Parse(string s, Type resultType, IFormatProvider provider, string format, bool returnDbNUllIfNotValid)
        {
            object value = _Parse(s, resultType, provider, format, returnDbNUllIfNotValid);
            return NullableHelper.FixDbNUllasNull(value, resultType);
        }

        /// <summary>
        /// Parse the given text using the resultTypes "Parse" method or using a type converter.
        /// </summary>
        /// <param name="s">The text to parse.</param>
        /// <param name="resultType">The requested result type.</param>
        /// <param name="provider">A <see cref="IFormatProvider"/> used to format or parse the value. Can be NULL.</param>
        /// <param name="formats">A string array holding permissible formats used in a <see cref="System.Object.ToString"/> call. Right now
        /// formats is only interpreted to enable roundtripping for formatted dates.
        /// </param>
        /// <param name="returnDbNUllIfNotValid">Indicates whether DbNull should be returned if value cannot be parsed. Otherwise an exception is thrown.</param>
        /// <returns>The new value in the target type.</returns>
        public static object Parse(string s, Type resultType, IFormatProvider provider, string[] formats, bool returnDbNUllIfNotValid)
        {
            object value = _Parse(s, resultType, provider, "", formats, returnDbNUllIfNotValid);
            return NullableHelper.FixDbNUllasNull(value, resultType);
        }

        static object _Parse(string s, Type resultType, IFormatProvider provider, string format, bool returnDbNUllIfNotValid)
        {
            return _Parse(s, resultType, provider, format, null, returnDbNUllIfNotValid);
        }
        static object _Parse(string s, Type resultType, IFormatProvider provider, string format, string[] formats, bool returnDbNUllIfNotValid)
        {
        
			//Fix for defect #12619.
            if (resultType == null) //|| resultType == typeof(string))
                return s;

            object result;

            try
            {
                if (typeof(double).IsAssignableFrom(resultType))
                {
                    if (IsEmpty(s))
                        return Convert.DBNull;

                    double d;
                    if (double.TryParse(s, NumberStyles.Any, provider, out d))
                    {
                        result = Convert.ChangeType(d, resultType, provider);
                        return result;
                    }

                    if (returnDbNUllIfNotValid)
                    {
                        if (resultType == typeof(double) || resultType == typeof(float))
                            return Convert.DBNull;
                    }
                }
                else if (typeof(decimal).IsAssignableFrom(resultType))
                {
                    if (IsEmpty(s))
                        return Convert.DBNull;

                    decimal d;
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
                    if (decimal.TryParse(s, NumberStyles.Any, provider, out d))
                    {
                        result = Convert.ChangeType(d, resultType, provider);
                        return result;
                    }
#else
                    d = decimal.Parse(s, NumberStyles.Any, provider);
                    result = Convert.ChangeType(d, resultType, provider);
#endif
                }
                else if (typeof(DateTime).IsAssignableFrom(resultType))
                {
                    if (IsEmpty(s))
                        return Convert.DBNull;

                    if (formats == null || formats.GetLength(0) == 0 && format.Length > 0)
                        formats = new string[] { format, "G", "g", "f", "F", "d", "D" };

                    if (formats != null && formats.GetLength(0) > 0)
                    {
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
                        DateTime dtresult;
                        if (DateTime.TryParseExact(s, formats, provider, DateTimeStyles.AllowInnerWhite | DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite | DateTimeStyles.AllowWhiteSpaces, out dtresult))
                            return dtresult;
#else
                        try
                        {
                        return DateTime.ParseExact(s, formats, provider, DateTimeStyles.AllowInnerWhite | DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite | DateTimeStyles.AllowWhiteSpaces);
                    }
                        catch
                    {
                        }
#endif
                    }

                        return DateTime.Parse(s, provider, DateTimeStyles.AllowInnerWhite | DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite | DateTimeStyles.AllowWhiteSpaces);
                }
                else if (typeof(bool).IsAssignableFrom(resultType))
                {
                    if (IsEmpty(s))
                        return Convert.DBNull;

                    if (s == "1" || s.ToUpper() == bool.TrueString.ToUpper())
                        return true;
                    else if (s == "0" || s.ToUpper() == bool.TrueString.ToUpper())
                        return false;
                }
                else if (typeof(long).IsAssignableFrom(resultType))
                {
                    if (IsEmpty(s))
                        return Convert.DBNull;

                    long d;
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
                    if (long.TryParse(s, NumberStyles.Any, provider, out d))
                    {
                        result = Convert.ChangeType(d, resultType, provider);
                        return result;
                    }
#else
                    try
                    {
                        d = long.Parse(s, NumberStyles.Any, provider);
                        result = Convert.ChangeType(d, resultType, provider);
                        return result;
                    }
                    catch
                    {
                    }
#endif
                    if (returnDbNUllIfNotValid)
                    {
                        if (resultType.IsPrimitive && !resultType.IsEnum)
                            return Convert.DBNull;
                    }
                }
                else if (typeof(ulong).IsAssignableFrom(resultType))
                {
                    if (IsEmpty(s))
                        return Convert.DBNull;

                    ulong d;
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
                    if (ulong.TryParse(s, NumberStyles.Any, provider, out d))
                    {
                        result = Convert.ChangeType(d, resultType, provider);
                        return result;
                    }
#else
                    try
                    {
                        d = ulong.Parse(s, NumberStyles.Any, provider);
                        result = Convert.ChangeType(d, resultType, provider);
                        return result;
                    }
                    catch
                    {
                    }
#endif

                    if (returnDbNUllIfNotValid)
                    {
                        if (resultType.IsPrimitive && !resultType.IsEnum)
                            return Convert.DBNull;
                    }
                }
                else if (typeof(int).IsAssignableFrom(resultType)
                    || typeof(short).IsAssignableFrom(resultType)
                    || typeof(float).IsAssignableFrom(resultType)
                    || typeof(uint).IsAssignableFrom(resultType)
                    || typeof(ushort).IsAssignableFrom(resultType)
                    || typeof(byte).IsAssignableFrom(resultType))
                {
                    if (IsEmpty(s))
                        return Convert.DBNull;

                    double d;
                    if (double.TryParse(s, NumberStyles.Any, provider, out d))
                    {
                        result = Convert.ChangeType(d, resultType, provider);
                        return result;
                    }

                    if (returnDbNUllIfNotValid)
                    {
                        if (resultType.IsPrimitive && !resultType.IsEnum)
                            return Convert.DBNull;
                    }
                }
                else if (resultType == typeof(Type))
                {
                    result = Type.GetType(s);
                    return result;
                }


                TypeConverter typeConverter = TypeDescriptor.GetConverter(resultType);

                if (typeConverter is NullableConverter)
                {
                    Type nullableUnderlyingType = NullableHelper.GetUnderlyingType(resultType);
                    if (nullableUnderlyingType != null)
                        return _Parse(s, nullableUnderlyingType, provider, format, formats, returnDbNUllIfNotValid);
                }

                if (typeConverter != null &&
                    typeConverter.CanConvertFrom(typeof(System.String)) &&
                    s != null && s.Length > 0
                    )
                {
                    if (provider is CultureInfo)
                        result = typeConverter.ConvertFrom(null, (CultureInfo)provider, s);
                    else
                        result = typeConverter.ConvertFrom(s);
                    return result;
                }
            }
            catch
            {
                if (returnDbNUllIfNotValid)
                    return Convert.DBNull;

                throw;
            }

            // throw new InvalidCastException(SR.GetString("InvalidCast_IConvertible"));
            return Convert.DBNull;
        }

        /// <summary>
        /// Generates display text using the specified format, culture info and number format.
        /// </summary>
        /// <param name="value">The value to format.</param>
        /// <param name="valueType">The value type on which formatting is based. The original value will first be converted to this type.</param>
        /// <param name="format">The format like in ToString(string format).</param>
        /// <param name="ci">The <see cref="CultureInfo"/> for formatting the value.</param>
        /// <param name="nfi">The <see cref="NumberFormatInfo"/> for formatting the value.</param>
        /// <returns>The string with the formatted text for the value.</returns>
        public static string FormatValue(object value, Type valueType, string format, CultureInfo ci, NumberFormatInfo nfi)
        {
            string strResult;
            object obj;
            try
            {
                if (value is string)
                    return (string)value;
                else if (value is byte[] || value is System.Drawing.Image) // Picture
                    return "";
                else if (value == null || valueType == null || value.GetType() == valueType)
                    obj = value;
                else
                {
                    try
                    {
                        obj = ValueConvert.ChangeType(value, valueType, ci, true);
                    }
                    catch (Exception ex)
                    {
                        obj = value;
                        if (!(ex is FormatException || ex.InnerException is FormatException))
                            throw;
                    }
                }

                if (obj == null || obj is System.DBNull)
                    strResult = String.Empty;	// or "NullString"
                else
                {
                    if (obj is IFormattable)
                    {
                        IFormattable formattableValue = (IFormattable) obj;
                        IFormatProvider provider = null;
                        if (nfi != null && !(obj is DateTime))
                            provider = nfi;
                        else if (ci != null)
                            provider = obj is DateTime ? (IFormatProvider) ci.DateTimeFormat : (IFormatProvider) ci.NumberFormat;

                        if (format.Length > 0 || nfi != null)
                            strResult = formattableValue.ToString(format, provider);
                        else
                            strResult = formattableValue.ToString();
                    }
                    else
                    {
                        TypeConverter tc = TypeDescriptor.GetConverter(obj.GetType());
                        if (tc.CanConvertTo(typeof(string)))
                        {
                            strResult = (string)tc.ConvertTo(null, ci, obj, typeof(string));
                        }
                        else if (obj is IConvertible)
                            strResult = Convert.ToString(obj, ci);
                        else
                            strResult = obj.ToString();
                    }
                }
            }
            catch
            {
                strResult = String.Empty;
                throw;   // TODO: should I throw a more specific instead?
            }
            if (strResult == null)
                strResult = String.Empty;

            if (allowFormatValueTrimEnd)
                strResult = strResult.TrimEnd();
            return strResult;
        }

        static bool allowFormatValueTrimEnd = false;

        /// <summary>
        /// Indicates whether <see cref="FormatValue"/> should trim whitespace characters from
        /// the end of the formatted text.
        /// </summary>
        public static bool AllowFormatValueTrimEnd
        {
            get
            {
                return allowFormatValueTrimEnd;
            }
            set
            {
                allowFormatValueTrimEnd = value;
            }
        }

        /// <summary>
        /// Returns a representative value for any given type. Is useful to preview the
        /// result of a format in <see cref="System.Windows.Forms.PropertyGrid"/>. See <see cref="Syncfusion.Windows.Forms.Grid.GridStyleInfo.FormatPreview"/>
        /// </summary>
        /// <param name="type">The <see cref="Type"/>.</param>
        /// <returns>A value with the specified type.</returns>
        public static object GetDefaultValue(Type type)
        {
            object value;

            if (type == null)
                return "0";

            lock (cachedDefaultValues)
            {
                if (cachedDefaultValues.Contains(type))
                    value = cachedDefaultValues[type];
                else
                {
                    switch (type.FullName)
                    {
                        case "System.Double":
                        case "System.Single":
                        case "System.Decimal":
                            value = 123.4567;
                            break;

                        case "System.Boolean":
                            value = true;
                            break;

                        case "System.Drawing.Color":
                            value = System.Drawing.Color.Black;
                            break;

                        case "System.String":
                            value = String.Empty;
                            break;

                        case "System.DateTime":
                            value = DateTime.Now;
                            break;

                        case "System.Int32":
                        case "System.Int16":
                        case "System.Int64":
                        case "System.SByte":
                        case "System.Byte":
                        case "System.UInt16":
                        case "System.UInt32":
                        case "System.UInt64":
                            value = 123;
                            break;

                        case "System.Char":
                            value = 'A';
                            break;

                        case "System.DBNull":
                            value = Convert.DBNull;
                            break;

                        default:
                            value = "";
                            break;
                    }
                    cachedDefaultValues[type] = value;
                }
                return value;
            }

        }

        /// <summary>
        /// Overloaded. Parses the given string including type information. String can be in format %lt;type&gt; 'value'
        /// </summary>
        /// <param name="valueAsString"></param>
        /// <param name="retVal"></param>
        /// <returns></returns>
        static bool ParseValueWithTypeInformation(string valueAsString, out object retVal)
        {
            return ParseValueWithTypeInformation(valueAsString, out retVal);
        }

        /// <summary>
        /// Parses the given string including type information. String can be in format %lt;type&gt; 'value'
        /// </summary>
        /// <param name="valueAsString"></param>
        /// <param name="retVal"></param>
        /// <param name="allowConvertFromBase64">Indicates whether TypeConverter should be checked whether the type to be
        /// parsed supports conversion to/from byte array (e.g. an Image)</param>
        /// <returns></returns>
        public static bool ParseValueWithTypeInformation(string valueAsString, out object retVal, bool allowConvertFromBase64)
        {
            retVal = null;
            if (valueAsString.StartsWith("'") && valueAsString.EndsWith("'"))
            {
                retVal = valueAsString.Substring(1, valueAsString.Length - 2);
                return true;
            }
            else if (valueAsString.StartsWith("<"))
            {
                int closeBracket = valueAsString.IndexOf(">");
                if (closeBracket > 1)
                {
                    string typeName = valueAsString.Substring(1, closeBracket - 1);
                    if (typeName == "null")
                    {
                        retVal = null;
                        return true;
                    }
                    else if (typeName == "System.DBNull")
                    {
                        retVal = System.DBNull.Value;
                        return true;
                    }
                    else
                    {
                        valueAsString = valueAsString.Substring(closeBracket + 1).Trim();
                        if (valueAsString.StartsWith("'") && valueAsString.EndsWith("'"))
                        {
                            valueAsString = valueAsString.Substring(1, valueAsString.Length - 2);
                            Type type = ValueConvert.GetType(typeName);
                            if (type != null)
                            {
                                bool handled = false;
                                if (allowConvertFromBase64)
                                    handled = TryConvertFromBase64String(type, valueAsString, out retVal);

                                if (!handled)
                                    retVal = ValueConvert.Parse(valueAsString, type, System.Globalization.CultureInfo.InvariantCulture, "");
                                return true;
                            }
                        }
                    }
                }
            }

            retVal = valueAsString;
            return false;
        }

        /// <summary>
        /// Indicates whether the TypeConverter associated with the type supports conversion to/from a byte array (e.g. an Image). 
        /// If that is the case the string is converted to a byte array from a base64 string.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="valueAsString"></param>
        /// <param name="retVal"></param>
        /// <returns></returns>
        public static bool TryConvertFromBase64String(Type type, string valueAsString, out object retVal)
        {
            bool handled = false;
            retVal = null;
            TypeConverter tc = TypeDescriptor.GetConverter(type);
            if (tc != null)
            {
                // e.g. an Image
                if (tc.CanConvertFrom(typeof(byte[])))
                {
                    byte[] byteArray = (byte[])Convert.FromBase64String(valueAsString);
                    retVal = tc.ConvertFrom(byteArray);
                    handled = true;
                }
                else if (tc.CanConvertFrom(typeof(MemoryStream)))
                {
                    MemoryStream ms = new MemoryStream((byte[])Convert.FromBase64String(valueAsString));
                    retVal = tc.ConvertFrom(ms);
                    handled = true;
                }

            }
            return handled;
        }

        /// <summary>
        /// Overloaded. Formats the given value as string including type information. String will be in format %lt;type&gt; 'value'
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        static string FormatValueWithTypeInformation(object value)
        {
            return FormatValueWithTypeInformation(value, false);
        }

        /// <summary>
        /// Formats the given value as string including type information. String will be in format %lt;type&gt; 'value'
        /// </summary>
        /// <param name="value"></param>
        /// <param name="allowConvertToBase64">Indicates whether TypeConverter should be checked whether the type to be
        /// parsed supports conversion to/from byte array (e.g. an Image)</param>
        /// <returns></returns>
        public static string FormatValueWithTypeInformation(object value, bool allowConvertToBase64)
        {
            if (value is string)
                return "'" + (string)value + "'";
            else if (value is DBNull)
            {
                return "<System.DBNull>";
            }
            else if (value == null)
            {
                return "<null>";
            }
            else
            {
                string valueAsString = null;
                if (allowConvertToBase64)
                    valueAsString = TryConvertToBase64String(value);

                if (valueAsString == null)
                    valueAsString = ValueConvert.FormatValue(value, typeof(string), "", System.Globalization.CultureInfo.InvariantCulture, null);

                return "<" + GetTypeName(value.GetType()) + "> '" + valueAsString + "'";
            }
        }

        /// <summary>
        /// Indicates whether the TypeConverter associated with the type supports conversion to/from a byte array (e.g. an Image). 
        /// If that is the case the string is converted to a base64 string from a byte array.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string TryConvertToBase64String(object value)
        {
            string valueAsString = null;
            TypeConverter tc = TypeDescriptor.GetConverter(value);
            if (tc != null)
            {
                // e.g. an Image
                if (tc.CanConvertTo(typeof(byte[])))
                {
                    byte[] byteArray = (byte[])tc.ConvertTo(value, typeof(byte[]));
                    valueAsString = Convert.ToBase64String(byteArray);
                }
                else if (tc.CanConvertTo(typeof(MemoryStream)))
                {
                    MemoryStream ms = (MemoryStream)tc.ConvertTo(value, typeof(MemoryStream));
                    valueAsString = Convert.ToBase64String(ms.ToArray());
                }
            }
            return valueAsString;
        }

        /// <summary>
        /// Returns the type name. If type is not in mscorlib, the assembly name is appended.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetTypeName(Type type)
        {
            if (!type.IsPrimitive && type.Module != typeof(object).Module)
                return type.FullName + ", " + System.IO.Path.GetFileNameWithoutExtension(type.Module.ScopeName);
            return type.FullName;
        }

        /// <summary>
        /// Returns the type from the specified name. If an assembly name is appended the list of currently loaded
        /// assemblies in the current AppDomain are checked.
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static Type GetType(string typeName)
        {
            string[] parts = typeName.Split(',');
            if (parts.Length == 2)
            {
                // Module name without version information.
                ResolveEventArgs e = new ResolveEventArgs(parts[1].Trim());
                Assembly assembly = AssemblyInfo.AssemblyResolver(null, e);
                if (assembly != null)
                    return assembly.GetType(parts[0]);
            }

            return Type.GetType(typeName);
        }

        /// <summary>
        /// Indicates whether string is null or empty.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsEmpty(string str)
        {
            return str == null || str.Length == 0;
        }

    }
}


