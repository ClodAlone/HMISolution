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

using ArrayList = System.Collections.Generic.List<object>;
using Hashtable = System.Collections.Generic.Dictionary<object, object>;

#if !WinRT
using Syncfusion.Windows.ComponentModel;
using Syncfusion.Windows.GridCommon;
using BitVector32 = Syncfusion.Windows.Collections.BitVectorInt32;

namespace Syncfusion.Windows.Styles
#else
using Syncfusion.WinRT.ComponentModel;
using Syncfusion.WinRT.GridCommon;
using BitVector32 = Syncfusion.WinRT.Collections.BitVectorInt32;

namespace Syncfusion.WinRT.Styles
#endif
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
#if !WinRT
                propertyInfo = ComponentType.GetProperty(this.PropertyName);
#else
                propertyInfo = ComponentType.GetRuntimeProperty(this.PropertyName);
#endif
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
#if !WinRT
            IsExpandable = !type.IsPrimitive && typeof(IStyleInfo).IsAssignableFrom(type);
#else
            IsExpandable = !type.GetTypeInfo().IsPrimitive && typeof(IStyleInfo).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo());
#endif
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
#if !WinRT
            else if (PropertyType.IsEnum)
#else
            else if (PropertyType.GetTypeInfo().IsEnum)
#endif
            {
                return Convert.ToString(value);
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
#if !WinRT
                result = Convert.DBNull;
#else
                result = null;
#endif
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
#if !WinRT
            MethodInfo parseMethod = resultType.GetMethod("Parse", new Type[] { typeof(string), typeof(IFormatProvider) });
#else
            MethodInfo parseMethod = resultType.GetRuntimeMethod("Parse", new Type[] { typeof(string), typeof(IFormatProvider) });
#endif

            if (parseMethod != null)
            {
                if (resultType.FullName == "System.Double" && s == "NaN")
                    return double.NaN;
                result = parseMethod.Invoke(null, new object[] { s, provider });
                return result;
            }

            result = Convert.ChangeType(s, resultType, provider);

            // throw new InvalidCastException(OGSR.GetString("InvalidCast_IConvertible"));
#if !WinRT
            return result != null ? result : Convert.DBNull;
#else
            return result != null ? result : null;
#endif
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
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
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
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
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
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
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

        public StyleInfoProperty Sip
        {
            get
            {
                return sip;
            }
        }
    }


    /// <summary>
    /// <see cref="ValueConvert"/> provides conversion routines for values
    /// to convert them to another type and routines for formatting values.
    /// </summary>
    public class ValueConvert
    {
        ValueConvert()
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
#if !WinRT
            if (value != null && !type.IsAssignableFrom(value.GetType()))
#else
            if (value != null && !type.GetTypeInfo().IsAssignableFrom(value.GetType().GetTypeInfo()))
#endif
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
#if !WinRT
                    else if (value is System.DBNull)
                    {
                        // value = null; changed after 4.1.0.50: do not set it to null - this causes then issues
                        // if you have a DataTable and the key is used for lookups, e.g.
                        // see sample in http://www.syncfusion.com/support/forums/message.aspx?MessageID=40207
                        // For NullableTypes the above call to NullableHelper.FixDbNUllasNull will
                        // take care of converting DbNull to null for nullable types only.
                    }

                    else if (type.IsEnum)
#else
                    else if (type.GetTypeInfo().IsEnum)
#endif
                    {
                        value = Convert.ChangeType(value, typeof(int), provider);
                        value = Enum.ToObject(type, (int)value);
                    }
#if !WinRT
                    else if (type == typeof(string) && !(value is IConvertible))
                    {
                        value = value != null ? value.ToString() : "";
                    }
#endif
                    else
                        value = NullableHelper.ChangeType(value, type, provider);
                }
                catch
                {
#if !WinRT
                    if (returnDbNUllIfNotValid)
                        return Convert.DBNull;
#endif

                    throw;
                }
            }
#if !WinRT
            if ((value == null || value is DBNull) && type == typeof(string))
                return "";
#else
            if ((value == null) && type == typeof(string))
                return "";
#endif

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
#if !WinRT
                if (typeof(double).IsAssignableFrom(resultType))
#else
                if (typeof(double).GetTypeInfo().IsAssignableFrom(resultType.GetTypeInfo()))
#endif
                {
                    if (IsEmpty(s))
#if !WinRT
                        return Convert.DBNull;
#else
                        return null;
#endif

                    double d;
                    if (double.TryParse(s, NumberStyles.Any, provider, out d))
                    {
                        result = Convert.ChangeType(d, resultType, provider);
                        return result;
                    }

                    if (returnDbNUllIfNotValid)
                    {
                        if (resultType == typeof(double) || resultType == typeof(float))
#if !WinRT
                            return Convert.DBNull;
#else
                            return null;
#endif
                    }
                }
#if !WinRT
                else if (typeof(decimal).IsAssignableFrom(resultType))
                {
                    if (IsEmpty(s))
                        return Convert.DBNull;
#else
                else if (typeof(decimal).GetTypeInfo().IsAssignableFrom(resultType.GetTypeInfo()))
                {
                    if (IsEmpty(s))
                        return null;
#endif

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
#if !WinRT
                else if (typeof(DateTime).IsAssignableFrom(resultType))
                {
                    if (IsEmpty(s))
                        return Convert.DBNull;
#else
                else if (typeof(DateTime).GetTypeInfo().IsAssignableFrom(resultType.GetTypeInfo()))
                {
                    if (IsEmpty(s))
                        return null;
#endif

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
#if !WinRT
                else if (typeof(bool).IsAssignableFrom(resultType))
                {
                    if (IsEmpty(s))
                        return Convert.DBNull;
#else
                else if (typeof(bool).GetTypeInfo().IsAssignableFrom(resultType.GetTypeInfo()))
                {
                    if (IsEmpty(s))
                        return null;
#endif

                    if (s == "1" || s.ToUpper() == bool.TrueString.ToUpper())
                        return true;
                    else if (s == "0" || s.ToUpper() == bool.TrueString.ToUpper())
                        return false;
                }
#if !WinRT
                else if (typeof(long).IsAssignableFrom(resultType))
                {
                    if (IsEmpty(s))
                        return Convert.DBNull;
#else
                else if (typeof(long).GetTypeInfo().IsAssignableFrom(resultType.GetTypeInfo()))
                {
                    if (IsEmpty(s))
                        return null;
#endif
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
#if !WinRT
                        if (resultType.IsPrimitive && !resultType.IsEnum)
                            return Convert.DBNull;
#else
                        if (resultType.GetTypeInfo().IsPrimitive && !resultType.GetTypeInfo().IsEnum)
                            return null;
#endif
                    }
                }
#if !WinRT
                else if (typeof(ulong).IsAssignableFrom(resultType))
                {
                    if (IsEmpty(s))
                        return Convert.DBNull;
#else
                else if (typeof(ulong).GetTypeInfo().IsAssignableFrom(resultType.GetTypeInfo()))
                {
                    if (IsEmpty(s))
                        return null;
#endif
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
#if !WinRT
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
#else
                    if (returnDbNUllIfNotValid)
                    {
                        if (resultType.GetTypeInfo().IsPrimitive && !resultType.GetTypeInfo().IsEnum)
                            return null;
                    }
                }
                else if (typeof(int).GetTypeInfo().IsAssignableFrom(resultType.GetTypeInfo())
                    || typeof(short).GetTypeInfo().IsAssignableFrom(resultType.GetTypeInfo())
                    || typeof(float).GetTypeInfo().IsAssignableFrom(resultType.GetTypeInfo())
                    || typeof(uint).GetTypeInfo().IsAssignableFrom(resultType.GetTypeInfo())
                    || typeof(ushort).GetTypeInfo().IsAssignableFrom(resultType.GetTypeInfo())
                    || typeof(byte).GetTypeInfo().IsAssignableFrom(resultType.GetTypeInfo()))
                {
                    if (IsEmpty(s))
                        return null;
#endif
                    double d;
                    if (double.TryParse(s, NumberStyles.Any, provider, out d))
                    {
                        result = Convert.ChangeType(d, resultType, provider);
                        return result;
                    }

                    if (returnDbNUllIfNotValid)
                    {
#if !WinRT
                        if (resultType.IsPrimitive && !resultType.IsEnum)
                            return Convert.DBNull;
#else
                        if (resultType.GetTypeInfo().IsPrimitive && !resultType.GetTypeInfo().IsEnum)
                            return null;
#endif
                    }
                }
                else if (resultType == typeof(Type))
                {
                    result = Type.GetType(s);
                    return result;
                }


                result = Convert.ChangeType(s, resultType, provider);
                return result;
                //TypeConverter typeConverter = TypeDescriptor.GetConverter(resultType);
                //if (typeConverter != null &&
                //    typeConverter.CanConvertFrom(typeof(System.String)) &&
                //    s != null && s.Length > 0
                //    )
                //{
                //    if (provider is CultureInfo)
                //        result = typeConverter.ConvertFrom(null, (CultureInfo) provider, s);
                //    else
                //        result = typeConverter.ConvertFrom(s);
                //    return result;
                //}
            }
            catch
            {
                if (returnDbNUllIfNotValid)
#if !WinRT
                    return Convert.DBNull;
#else
                    return null;
#endif


                throw;
            }

            // throw new InvalidCastException(SR.GetString("InvalidCast_IConvertible"));
#if !WinRT
/*
            return Convert.DBNull;
*/
#else
            return null;
#endif
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
        public static string FormatValue(object value, Type valueType, string format, CultureInfo ci, NumberFormatInfo nfi, IFormatProvider FormatProvider)
        {
            string strResult;
            object obj;
            try
            {
                if (value is string)
                {
                    if(!string.IsNullOrEmpty((string)value)&& ((string)value)[0]=='\'')
                    {
                        //To remove the apostrophe symbol in the text as like excel
                        return ((string)value).Substring(1);
                    }
                    if (format == ";;;")
                        return string.Empty;
                    return (string)value;
                }
                else if (value is byte[]) // Picture
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

                ///To change the double value into Date and Time object for convert into specified format.
                if (!(obj is DateTime) && System.Text.RegularExpressions.Regex.IsMatch(format, @"([mdMy][,][ mdMy])|([mdMy][-/][mdMy])|([mdMy][-/](.*)[-/][mdMy])|([Mdy][\s][Mdy])", System.Text.RegularExpressions.RegexOptions.IgnorePatternWhitespace))
                {
#if !WinRT
                    double d;
                    if (double.TryParse(value.ToString(), out d))
                        obj = DateTime.FromOADate(d);
#else
                    long d;
                    if (long.TryParse(value.ToString(), out d))
                        obj = DateTime.FromBinary(d);
#endif
                }
                else if (format == ";;;")
                    return string.Empty;
#if !WinRT
                if (obj == null || obj is System.DBNull)
#else
                if (obj == null)
#endif
                    strResult = String.Empty;	// or "NullString"
                else
                {
                    if (FormatProvider != null)
                    {
                        strResult = string.Format(FormatProvider, format, obj);
                    }
                    else if (obj is IFormattable)
                    {
                        IFormattable formattableValue = (IFormattable)obj;
                        IFormatProvider provider = null;
                        if (nfi != null && !(obj is DateTime))
                            provider = nfi;
                        else if (ci != null)
                            provider = obj is DateTime ? (IFormatProvider)ci.DateTimeFormat : (IFormatProvider)ci.NumberFormat;

                        if (format.Length > 0 || nfi != null)
                            strResult = formattableValue.ToString(format, provider);
                        else
                            strResult = formattableValue.ToString();
                    }
                    else
                    {
                        /*TypeConverter tc = TypeDescriptor.GetConverter(obj.GetType());
                        if (tc.CanConvertTo(typeof(string)))
                        {
                            strResult = (string) tc.ConvertTo(null, ci, obj, typeof(string));
                        }
                        else */
#if !WinRT
                        if (obj is IConvertible)
                            strResult = Convert.ToString(obj, ci);
                        else
#endif
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

            if (obj is DateTime && strResult.Equals(format))
                strResult = obj.ToString();
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
        /// Returns a representative value for any given type. 
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
                if (cachedDefaultValues.ContainsKey(type))
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
#if !WinRT
                            value = Convert.DBNull;
#else
                            value = null;
#endif
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
#if !WinRT
                        retVal = System.DBNull.Value;
#else
                        retVal = null;
#endif
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
            retVal = null;
            return false;
            //bool handled = false;
            //retVal = null;
            //TypeConverter tc = TypeDescriptor.GetConverter(type);
            //if (tc != null)
            //{
            //    // e.g. an Image
            //    if (tc.CanConvertFrom(typeof(byte[])))
            //    {
            //        byte[] byteArray = (byte[]) Convert.FromBase64String(valueAsString);
            //        retVal = tc.ConvertFrom(byteArray);
            //        handled = true;
            //    }
            //    else if (tc.CanConvertFrom(typeof(MemoryStream)))
            //    {
            //        MemoryStream ms = new MemoryStream((byte[]) Convert.FromBase64String(valueAsString));
            //        retVal = tc.ConvertFrom(ms);
            //        handled = true;
            //    }

            //}
            //return handled;
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
#if !WinRT
            else if (value is DBNull)
            {
                return "<System.DBNull>";
            }
#endif
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
                    valueAsString = ValueConvert.FormatValue(value, typeof(string), "", System.Globalization.CultureInfo.InvariantCulture, null, null);

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
            return valueAsString;
        }

        /// <summary>
        /// Returns the type name. If type is not in mscorlib, the assembly name is appended.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetTypeName(Type type)
        {
            //if (!type.IsPrimitive && type.Module != typeof(object).Module)
            //    return type.FullName + ", " + System.IO.Path.GetFileNameWithoutExtension(type.Module.ScopeName);
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
            //if (parts.Length == 2)
            //{
            //    // Module name without version information.
            //    ResolveEventArgs e = new ResolveEventArgs(parts[1].Trim());
            //    Assembly assembly = AssemblyInfo.AssemblyResolver(null, e);
            //    if (assembly != null)
            //        return assembly.GetType(parts[0]);
            //}

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


