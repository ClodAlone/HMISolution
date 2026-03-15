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


#region file using directives
using System;
using Syncfusion.CompoundFile.DocIO;
using Syncfusion.CompoundFile.DocIO.Net;
using DocumentPropertyValueType = Syncfusion.CompoundFile.DocIO.PropertyType;
using Syncfusion.CompoundFile.DocIO.Native;
using Syncfusion.DocIO.Utilities;
using System.Runtime.InteropServices;
using System.Text;
#if SILVERLIGHT && !WINRT
using Syncfusion.DocIO.Implementation.Silverlight;
#elif WP
using Syncfusion.DocIO.Implementation.WP;
#endif
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Summary description for DocumentProperty.
    /// </summary>
	public class DocumentProperty
	{
    #region Class constants
    /// <summary>
    /// Start index for Id2 PropVariant property.
    /// </summary>
    private const int DEF_START_ID2 = ( int )BuiltInProperty.Category;
    /// <summary>
    /// Start year for FILETIME structure.
    /// </summary>
    private const int DEF_FILE_TIME_START_YEAR = 1600;
    #endregion

    #region Class members
    /// <summary>
    /// Property id.
    /// </summary>
    private BuiltInProperty m_propertyId;
    /// <summary>
    /// Property name.
    /// </summary>
    private string m_strName;
    /// <summary>
    /// Property value.
    /// </summary>
    private object m_value;
    /// <summary>
    /// Property type.
    /// </summary>
    DocumentPropertyValueType m_type;
    /// <summary>
    /// The source of a linked custom document property. Read/write String.
    /// </summary>
    private string m_strLinkSource;
    /// <summary>
    /// True if the value of the custom document property is linked to the content
    /// of the container document. False if the value is static. Read/write Boolean.
    /// </summary>
    private bool m_bLinkToContent;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor.
    /// </summary>
    private DocumentProperty()
    {
    }
    /// <summary>
    /// Initializes new instance of the class.
    /// </summary>
    /// <param name="strName">Property name.</param>
    /// <param name="value">Property value.</param>
    internal DocumentProperty( string strName, object value )
    {
      if( strName == null )
        throw new ArgumentNullException( "strName" );

      if( strName.Length == 0 )
        throw new ArgumentException( "strName - string cannot be empty." );

      m_strName = strName;
      Value = value;
      m_type = DetectPropertyType(value);
    }
    /// <summary>
    /// Initializes new instance of the class.
    /// </summary>
    /// <param name="strName">Property name.</param>
    /// <param name="value">Property value.</param>
    /// <param name="type">Property type</param>
    internal DocumentProperty(string strName, object value, DocumentPropertyValueType type)
    {
        if (strName == null)
            throw new ArgumentNullException("strName");

        if (strName.Length == 0)
            throw new ArgumentException("strName - string cannot be empty.");

        m_strName = strName;
        m_value = value;
        m_type = type;
    }
    /// <summary>
    /// Initializes new instance of the class.
    /// </summary>
    /// <param name="propertyId">Property id.</param>
    /// <param name="value">Property value.</param>
    internal DocumentProperty( BuiltInProperty propertyId, object value )
    {
      m_propertyId = propertyId;
      m_value = value;
      m_type = DetectPropertyType(value);
    }
    /// <summary>
    /// Initializes new instance of the document property.
    /// </summary>
    /// <param name="variant">Variant that contains property data.</param>
    /// <param name="bSummary">
    /// Indicates whether property is from document summary or not (only for
    /// built-int properties).</param>
    internal DocumentProperty( IPropertyData variant, bool bSummary )
    {
      if( variant == null )
        throw new ArgumentNullException( "variant" );

      m_strName = variant.Name;

      if( m_strName == null )
      {
        if( bSummary )
        {
          m_propertyId = ( BuiltInProperty )variant.Id;
          //if (m_propertyId == BuiltInProperty.EditTime)
          //{
          //    variant.SetValue((object)(Convert.ToDateTime(variant.Value).Ticks / 600000000), Syncfusion.CompoundFile.DocIO.PropertyType.Double);
          //}
       }
        else
        {
          m_propertyId = ( BuiltInProperty )( variant.Id
            + DEF_START_ID2 - ( int )PIDDSI.Category );
        }
      }
      if (bSummary
          && m_propertyId == BuiltInProperty.EditTime
          && variant.Value is DateTime)
          m_value = TimeSpan.FromTicks(((DateTime)variant.Value).Ticks - PropVariant.DEF_FILETIME_TICKS_DIFFERENCE);
      else
          m_value = variant.Value;
      m_type = (DocumentPropertyValueType)variant.Type;
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Indicates whether property is built-in. Read-only.
    /// </summary>
    internal bool IsBuiltIn
    {
      get
      {
        return m_strName == null;
      }
    }
    /// <summary>
    /// Returns / sets property id for built-in properties.
    /// </summary>
    internal BuiltInProperty PropertyId
    {
      get
      {
        return m_propertyId;
      }
      set
      {
        m_propertyId = value;
      }
    }
    /// <summary>
    /// Returns property name. Read-only.
    /// </summary>
    public string Name
    {
      get
      {
        return ( m_strName == null ) ? m_propertyId.ToString() : m_strName;
      }
    }
    /// <summary>
    /// Gets / sets the property value.
    /// </summary>
    public object Value
    {
      get
      {
        return m_value;
      }
      set
      {
        m_value = value;
        DetectPropertyType();
      }
    }
    /// <summary>
    /// Gets the type of the value.
    /// </summary>
    /// <value>The type of the value.</value>
    public PropertyValueType ValueType
    {
        get
        {
            if (m_value is string)
            {
                return PropertyValueType.String;
            }
            if (m_value is bool)
            {
                return PropertyValueType.Boolean;
            }
            if (m_value is DateTime)
            {
                return PropertyValueType.Date;
            }
            if (m_value is int || m_value is Int32)
            {
                return PropertyValueType.Int;
            }
            if (m_value is double)
            {
                return PropertyValueType.Double;
            }
            if (Value is float)
            {
                return PropertyValueType.Float;
            }
            if (Value is byte[])
            {
                return PropertyValueType.ByteArray;
            }
#if !SILVERLIGHT && !WP
            if (Value is ClipDataWrapper)
            {
                return PropertyValueType.ClipData;
            }
#endif
            else
            {
                throw new Exception("Property value is of unsupported type.");
            }
        }
    }
    /// <summary>
    /// Gets / sets boolean value.
    /// </summary>
    internal bool Boolean
    {
      get
      {
          if (m_type == DocumentPropertyValueType.Bool)
          return Convert.ToBoolean( m_value );

        throw new InvalidCastException( "Can't convert value to boolean." );
      }
      set
      {
          m_type = DocumentPropertyValueType.Bool;
        m_value = value;
      }
    }
    /// <summary>
    /// Gets / sets integer value.
    /// </summary>
    internal int Integer
    {
      get
      {
          DetectPropertyType();
       if (m_type == DocumentPropertyValueType.Int)
           return int.Parse(m_value.ToString());

        throw new InvalidCastException( "Can't convert value to integer." );
      }
      set
      {
        m_type = DocumentPropertyValueType.Int;
        m_value = value;
      }
    }
    /// <summary>
    /// Gets / sets 4-bytes signed integer value.
    /// </summary>
    internal int Int32
    {
      get
      {
          DetectPropertyType();
          if (m_type == DocumentPropertyValueType.Int32)
          return Convert.ToInt32( m_value );

        throw new InvalidCastException( "Can't convert value to integer." );
      }
      set
      {
          m_type = DocumentPropertyValueType.Int32;
        m_value = value;
      }
    }
    /// <summary>
    /// Gets / sets double value.
    /// </summary>
    internal double Double
    {
      get
      {
          DetectPropertyType();
          if (m_type == DocumentPropertyValueType.Double)
          return Convert.ToDouble( m_value );

        throw new InvalidCastException( "Can't convert value to integer." );
      }
      set
      {
          m_type = DocumentPropertyValueType.Double;
        m_value = value;
      }
    }
    /// <summary>
    /// Gets / sets string value.
    /// </summary>
    internal string Text
    {
      get
      {
          if (m_type == DocumentPropertyValueType.Empty)
              DetectPropertyType();
          if (m_type == DocumentPropertyValueType.String || m_type == DocumentPropertyValueType.AsciiString)
              return Convert.ToString( m_value );

        throw new InvalidCastException( "Can't convert value to string." );
      }
      set
      {
        m_type = DetectStringType(value);
        m_value = value;
      }
    }
    /// <summary>
    /// Detects type of the string.
    /// </summary>
    /// <param name="value">String value to check.</param>
    /// <returns>Detected string type.</returns>
    private DocumentPropertyValueType DetectStringType(string value)
    {
        return (Encoding.UTF8.GetByteCount(value) == value.Length) ?
          DocumentPropertyValueType.AsciiString :
          DocumentPropertyValueType.String;
    }
    /// <summary>
    /// Gets / sets DateTime value.
    /// </summary>
    internal DateTime DateTime
    {
      get
      {
          try
          {
              DetectPropertyType();
             if (m_type == DocumentPropertyValueType.DateTime)
                  return Convert.ToDateTime(m_value);
              else
                  return DateTime.MinValue;
          }
          catch
          {
              return DateTime.MinValue;
          }
      }
      set
      {
          m_type = DocumentPropertyValueType.DateTime;
        m_value = value;
      }
    }
    /// <summary>
    /// Gets / sets TimeSpan value.
    /// </summary>
    internal TimeSpan TimeSpan
    {
      get
      {
          return (TimeSpan)Value;
      }
      set
      {
          m_value = value;
      }
    }
    /// <summary>
    /// Gets / sets Blob value.
    /// </summary>
    internal byte[] Blob
    {
      get
      {
          if (m_type == DocumentPropertyValueType.Blob)
          return ( byte[] )m_value;

        throw new InvalidCastException( "Can't convert value to Blob." );
      }
      set
      {
        if( value == null )
          throw new ArgumentNullException( "value" );

        m_type = DocumentPropertyValueType.Blob;
        m_value = value;
      }
    }
    /// <summary>
    /// Gets or sets clipboard data value.
    /// </summary>
    public ClipboardData ClipboardData
    {
        get
        {
            if (m_type == DocumentPropertyValueType.ClipboardData)
                return (ClipboardData)m_value;

            throw new InvalidCastException("Can't convert value to ClipboardData.");
        }
        set
        {
            if (value == null)
                throw new ArgumentNullException("value");

            m_type = DocumentPropertyValueType.ClipboardData;
            m_value = value;
        }
    }
    /// <summary>
    /// Gets / sets array of strings.
    /// </summary>
    internal string[] StringArray
    {
      get
      {
          if (m_type == DocumentPropertyValueType.StringArray)
          return ( string[] )m_value;

        throw new InvalidCastException( "Can't convert value to an array of strings." );
      }
      set
      {
        if( value == null )
          throw new ArgumentNullException( "value" );

        m_type = DocumentPropertyValueType.StringArray;
        m_value = value;
      }
    }
    /// <summary>
    /// Gets / sets array of objects. Supported object types are string and Int32.
    /// </summary>
    internal object[] ObjectArray
    {
      get
      {
          if (m_type == DocumentPropertyValueType.ObjectArray)
          return ( object[] )m_value;

        throw new InvalidCastException( "Can't convert value to an array of strings." );
      }
      set
      {
        if( value == null )
          throw new ArgumentNullException( "value" );

        m_type = DocumentPropertyValueType.ObjectArray;
        m_value = value;
      }
    }
    /// <summary>
    /// Gets / sets document property type.
    /// </summary>
    internal DocumentPropertyValueType PropertyType
    {
      get
      {
        return m_type;
      }
      set
      {
        m_type = value;
      }
    }
    /// <summary>
    /// Returns or sets the source of a linked custom document property. Read/write String.
    /// </summary>
    internal string LinkSource
    {
      get
      {
        return m_strLinkSource;
      }
      set
      {
        if( IsBuiltIn )
          throw new InvalidOperationException( "This operation can't be performed on built-in property." );

        m_strLinkSource = value;
        m_bLinkToContent = true;
      }
    }
    /// <summary>
    /// True if the value of the custom document property is linked to the content of the container document. False if the value is static. Read/write Boolean.
    /// </summary>
    internal bool LinkToContent
    {
      get
      {
        return m_bLinkToContent;
      }
      set
      {
        m_bLinkToContent = value;
      }
    }
    /// <summary>
    /// Internal name of the document property.
    /// </summary>
    internal string InternalName
    {
      get
      {
        return m_strName;
      }
    }
    #endregion

    #region Class methods
    /// <summary>
    /// To the bool.
    /// </summary>
    /// <returns></returns>
    public bool ToBool()
    {
        return (bool)m_value;
    }
    /// <summary>
    /// Convert the object in DateTime.
    /// </summary>
    /// <returns></returns>
    public DateTime ToDateTime()
    {
        DateTime time1 = (DateTime)m_value;
        return time1.Date;
    }
    /// <summary>
    /// Convert the object in float value.
    /// </summary>
    /// <returns></returns>
    public float ToFloat()
    {
        return Convert.ToSingle(m_value);
    }
    /// <summary>
    /// Convert the object in double.
    /// </summary>
    /// <returns></returns>
    public double ToDouble()
    {
        return (double)m_value;
    }
    /// <summary>
    /// Convert the object in Int value.
    /// </summary>
    /// <returns></returns>
    public int ToInt()
    {
        return (int)m_value;
    }
    /// <summary>
    /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
    /// </returns>
    public override string ToString()
    {
        return (string)m_value;
    }
    /// <summary>
    /// Convert the object as byte array.
    /// </summary>
    /// <returns></returns>
    public byte[] ToByteArray()
    {
        return (byte[])m_value;
    }
#if !(SILVERLIGHT || WP)
    /// <summary>
    /// Gets the clip data.
    /// </summary>
    /// <returns></returns>
    internal ClipDataWrapper ToClipData()
    {
        return (ClipDataWrapper)m_value;
    }
#endif
    /// <summary>
    /// Copies document property data into PropVariant.
    /// </summary>
    /// <param name="variant">Destination object.</param>
    /// <param name="iPropertyId">PropertyId for custom properties.</param>
    /// <returns>True if was able to fill variant; false otherwise.</returns>
    internal bool FillPropVariant(Syncfusion.CompoundFile.DocIO.Net.IPropertyData variant, int iPropertyId)
    {
      if( variant == null )
        throw new ArgumentNullException( "variant" );

      if( m_value == null )
        return false;

      if( IsBuiltIn )
      {
        bool bSummary;
        int iIndex = CorrectIndex( m_propertyId, out bSummary );

        variant.Id = iIndex;
      }
      else
      {
        //variant.SetName( m_strName );
        //variant.PropId = ( PIDSI )iPropertyId;
        variant.Id = iPropertyId;
      }
      object value = m_value;
      if (IsBuiltIn 
          && variant.Id == (int)PIDSI.EditTime
          && m_value is TimeSpan)
          value = DateTime.Now;
      return variant.SetValue(value, (Syncfusion.CompoundFile.DocIO.PropertyType)m_type);

      // Here we have to assign correct value.
      //variant.Bool = false;
    }
    /// <summary>
    /// Converts propertyId into correct index.
    /// </summary>
    /// <param name="propertyId">PropertyId to convert.</param>
    /// <param name="bSummary">[out] Indicates whether this is document summary property of simply document property.</param>
    /// <returns>Correct property index.</returns>
    internal int CorrectIndex(BuiltInProperty propertyId, out bool bSummary)
    {
      int iIndex = ( int )propertyId;

      if( iIndex >= DEF_START_ID2 )
      {
        iIndex -= DEF_START_ID2 - ( int )PIDDSI.Category;
        bSummary = false;
      }
      else
      {
        bSummary = true;
      }

      return iIndex;
    }
    internal static DocumentPropertyValueType DetectPropertyType(object value)
    {
        DocumentPropertyValueType type = DocumentPropertyValueType.Null;
        if (value is string)
        {
           type = (Encoding.UTF8.GetByteCount(value as string) == (value as string).Length) ?  DocumentPropertyValueType.AsciiString :
                                                                        DocumentPropertyValueType.String;
        }
        else if (value is double)
        {
            type = DocumentPropertyValueType.Double;
        }
        else if (value is int)
        {
#if DOCIO
            type = DocumentPropertyValueType.Int32;
#else
        type = DocumentPropertyValueType.Int;
#endif
        }
        else if (value is bool)
        {
            type = DocumentPropertyValueType.Bool;
        }
        else if (value is DateTime || value is TimeSpan)
        {
            type = DocumentPropertyValueType.DateTime;
        }
        else if (value is object[])
        {
            type = DocumentPropertyValueType.ObjectArray;
        }
        else if (value is string[])
        {
            type = DocumentPropertyValueType.StringArray;
        }
        else if (value is byte[])
        {
            type = DocumentPropertyValueType.Blob;
        }
        else if (value is ClipboardData)
        {
            type = DocumentPropertyValueType.ClipboardData;
        }
        return type;
    }
    /// <summary>
    /// Tries to detect and set property type.
    /// </summary>
    private void DetectPropertyType()
    {
      if( m_value is string )
      {
          m_type = DetectStringType((string)m_value);//PropertyType.String;
      }
      else if( m_value is double )
      {
          m_type = DocumentPropertyValueType.Double;
      }
      else if( m_value is int )
      {
          m_type = DocumentPropertyValueType.Int32;
      }
      else if( m_value is bool )
      {
          m_type = DocumentPropertyValueType.Bool;
      }
      else if( m_value is DateTime || m_value is TimeSpan)
      {
          m_type = DocumentPropertyValueType.DateTime;
      }
      else if( m_value is object[] )
      {
          m_type = DocumentPropertyValueType.ObjectArray;
      }
      else if( m_value is string[] )
      {
          m_type = DocumentPropertyValueType.StringArray;
      }
      else if( m_value is byte[] )
      {
          m_type = DocumentPropertyValueType.Blob;
      }
      else if (m_value is ClipboardData)
      {
          m_type = DocumentPropertyValueType.ClipboardData;
      }
    }
    /// <summary>
    /// Sets value of LinkSource property.
    /// </summary>
    /// <param name="variant">Variant that contains value to set.</param>
    // TODO: This should become private or protected internal after finishing implementation
    /*protected internal*/
    internal void SetLinkSource(IPropertyData variant)
    {
      if( variant == null )
        throw new ArgumentNullException( "variant" );

      if( variant.Type != VarEnum.VT_LPSTR && variant.Type != VarEnum.VT_LPWSTR )
        throw new ArgumentOutOfRangeException( "LinkSource" );

      LinkSource = variant.Value.ToString();
    }
    #endregion

    #region ICloneParent Members
    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>A new object that is a copy of this instance.</returns>
    public DocumentProperty Clone()
    {
      DocumentProperty result = ( DocumentProperty )MemberwiseClone();
      result.CloneValue();
      return result;
    }

    /// <summary>
    /// Creates copy of the internal value.
    /// </summary>
    private void CloneValue()
    {
      if( m_value == null ) return;

      switch( m_type )
      {
          case DocumentPropertyValueType.Blob:
          m_value = CloneUtils.CloneByteArray( Blob );
          break;

          case DocumentPropertyValueType.StringArray:
          m_value = CloneUtils.CloneStringArray( StringArray );
          break;

          case DocumentPropertyValueType.ObjectArray:
          m_value = CloneUtils.CloneArray( ObjectArray );
          break;

          case DocumentPropertyValueType.ClipboardData:
          m_value = CloneUtils.CloneCloneable(ClipboardData);
          break;
      }
    }
    #endregion
  }
    
}


