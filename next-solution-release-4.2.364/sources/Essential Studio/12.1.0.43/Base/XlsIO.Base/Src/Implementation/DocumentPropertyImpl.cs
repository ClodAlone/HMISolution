#region Header

//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
//

#endregion Header

namespace Syncfusion.XlsIO.Implementation
{
    using System;
    using System.Runtime.InteropServices;

    using Syncfusion.XlsIO.IO.Stream.Win32;
    using Syncfusion.XlsIO.Interfaces;
    using Syncfusion.XlsIO.Parser.Biff_Records;

    /// <summary>
    /// Summary description for DocumentPropertyImpl.
    /// </summary>
    public class DocumentPropertyImpl : IDocumentProperty, ICloneParent
    {
        #region Fields

        /// <summary>
        /// Start year for FILETIME structure.
        /// </summary>
        public const int DEF_FILE_TIME_START_YEAR = 1600;

        /// <summary>
        /// Start index for Id2 PropVariant property.
        /// </summary>
        private const int DEF_START_ID2 = ( int )ExcelBuiltInProperty.Category;

        /// <summary>
        /// True if the value of the custom document property is linked to the content
        /// of the container document. False if the value is static. Read/write Boolean.
        /// </summary>
        private bool m_bLinkToContent;

        /// <summary>
        /// Property id.
        /// </summary>
        private ExcelBuiltInProperty m_propertyId;

        /// <summary>
        /// The source of a linked custom document property. Read/write String.
        /// </summary>
        private string m_strLinkSource;

        /// <summary>
        /// Property name.
        /// </summary>
        private string m_strName;

        /// <summary>
        /// Property type.
        /// </summary>
        ExcelPropertyType m_type;

        /// <summary>
        /// Property value.
        /// </summary>
        private object m_value;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes new instance of the class.
        /// </summary>
        /// <param name="strName">Property name.</param>
        /// <param name="value">Property value.</param>
        public DocumentPropertyImpl( string strName, object value )
        {
            if( strName == null )
            throw new ArgumentNullException( "strName" );

              if( strName.Length == 0 )
            throw new ArgumentException( "strName - string cannot be empty." );

              m_strName = strName;
              Value = value;
        }

        /// <summary>
        /// Initializes new instance of the class.
        /// </summary>
        /// <param name="propertyId">Property id.</param>
        /// <param name="value">Property value.</param>
        public DocumentPropertyImpl( ExcelBuiltInProperty propertyId, object value )
        {
            m_propertyId = propertyId;
              m_value = value;
        }

        /// <summary>
        /// Initialezes new instance of the document property.
        /// </summary>
        /// <param name="variant">Variant that contains property data.</param>
        /// <param name="bSummary">
        /// Indicates whether property is from document summary or not (only for
        /// built-int properties).</param>
        public DocumentPropertyImpl( PropVariant variant, bool bSummary )
        {
            if( variant == null )
            throw new ArgumentNullException( "variant" );

              m_strName = variant.Name;

              if( m_strName == null )
              {
            if( bSummary )
            {
              m_propertyId = ( ExcelBuiltInProperty )variant.PropId;
            }
            else
            {
              m_propertyId = ( ExcelBuiltInProperty )( ( int )variant.PropId2
            + DEF_START_ID2 - ( int )PIDDSI.Category );
            }
              }

              m_value = variant.Value;
              m_type = ( ExcelPropertyType )variant.Type;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        private DocumentPropertyImpl()
        {
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets / sets Blob value.
        /// </summary>
        public byte[] Blob
        {
            get
              {
            if( m_type == ExcelPropertyType.Blob )
              return ( byte[] )m_value;

            throw new InvalidCastException( "Can't convert value to Blob." );
              }
              set
              {
            if( value == null )
              throw new ArgumentNullException( "value" );

            m_type = ExcelPropertyType.Blob;
            m_value = value;
              }
        }

        /// <summary>
        /// Gets / sets boolean value.
        /// </summary>
        public bool Boolean
        {
            get
              {
            if( m_type == ExcelPropertyType.Bool )
              return Convert.ToBoolean( m_value );

            throw new InvalidCastException( "Can't convert value to boolean." );
              }
              set
              {
            m_type = ExcelPropertyType.Bool;
            m_value = value;
              }
        }

        /// <summary>
        /// Gets / sets DateTime value.
        /// </summary>
        public DateTime DateTime
        {
            get
              {
            if( m_type == ExcelPropertyType.DateTime )
              return Convert.ToDateTime( m_value );

            throw new InvalidCastException( "Can't convert value to DateTime." );
              }
              set
              {
            m_type = ExcelPropertyType.DateTime;
            m_value = value;
              }
        }

        /// <summary>
        /// Gets / sets double value.
        /// </summary>
        public double Double
        {
            get
              {
            if( m_type == ExcelPropertyType.Double )
              return Convert.ToDouble( m_value );

            throw new InvalidCastException( "Can't convert value to integer." );
              }
              set
              {
            m_type = ExcelPropertyType.Double;
            m_value = value;
              }
        }

        /// <summary>
        /// Gets / sets 4-bytes signed integer value.
        /// </summary>
        public int Int32
        {
            get
              {
            if( m_type == ExcelPropertyType.Int32 )
              return Convert.ToInt32( m_value );

            throw new InvalidCastException( "Can't convert value to integer." );
              }
              set
              {
            m_type = ExcelPropertyType.Int32;
            m_value = value;
              }
        }

        /// <summary>
        /// Gets / sets integer value.
        /// </summary>
        public int Integer
        {
            get
              {
            if( m_type == ExcelPropertyType.Int )
              return Convert.ToInt32( m_value );

            throw new InvalidCastException( "Can't convert value to integer." );
              }
              set
              {
            m_type = ExcelPropertyType.Int;
            m_value = value;
              }
        }

        /// <summary>
        /// Indicates whether property is built-in. Read-only.
        /// </summary>
        public bool IsBuiltIn
        {
            get
              {
            return m_strName == null;
              }
        }

        /// <summary>
        /// Returns or sets the source of a linked custom document property. Read/write String.
        /// </summary>
        public string LinkSource
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
        public bool LinkToContent
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
        /// Gets / sets array of objects. Supported object types are string and Int32.
        /// </summary>
        public object[] ObjectArray
        {
            get
              {
            if( m_type == ExcelPropertyType.ObjectArray )
              return ( object[] )m_value;

            throw new InvalidCastException( "Can't convert value to an array of strings." );
              }
              set
              {
            if( value == null )
              throw new ArgumentNullException( "value" );

            m_type = ExcelPropertyType.ObjectArray;
            m_value = value;
              }
        }

        /// <summary>
        /// Returns / sets property id for built-in properties.
        /// </summary>
        public ExcelBuiltInProperty PropertyId
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
        /// Gets / sets document property type.
        /// </summary>
        public ExcelPropertyType PropertyType
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
        /// Gets / sets array of strings.
        /// </summary>
        public string[] StringArray
        {
            get
              {
            if( m_type == ExcelPropertyType.StringArray )
              return ( string[] )m_value;

            throw new InvalidCastException( "Can't convert value to an array of strings." );
              }
              set
              {
            if( value == null )
              throw new ArgumentNullException( "value" );

            m_type = ExcelPropertyType.StringArray;
            m_value = value;
              }
        }

        /// <summary>
        /// Gets / sets string value.
        /// </summary>
        public string Text
        {
            get
              {
            if( m_type == ExcelPropertyType.String )
              return Convert.ToString( m_value );

            throw new InvalidCastException( "Can't convert value to string." );
              }
              set
              {
            m_type = ExcelPropertyType.String;
            m_value = value;
              }
        }

        /// <summary>
        /// Gets / sets TimeSpan value.
        /// </summary>
        public TimeSpan TimeSpan
        {
            get
              {
            DateTime date = DateTime;
            date = date.AddYears( -DEF_FILE_TIME_START_YEAR );
            return new TimeSpan( date.Ticks );
              }
              set
              {
            DateTime date = new DateTime( value.Ticks );
            date = date.AddYears( DEF_FILE_TIME_START_YEAR );
            DateTime = date;
              }
        }

        /// <summary>
        /// Gets / sets property value.
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

        #endregion Properties

        #region Methods

        /// <summary>
        /// Converts propertyId into correct index.
        /// </summary>
        /// <param name="propertyId">PropertyId to convert.</param>
        /// <param name="bSummary">[out] Indicates whether this is documnet summary property of simply document property.</param>
        /// <returns>Correct property index.</returns>
        public static int CorrectIndex( ExcelBuiltInProperty propertyId, out bool bSummary )
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

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <param name="parent">Parent object for a copy of this instance.</param>
        /// <returns>A new object that is a copy of this instance.</returns>
        public object Clone( object parent )
        {
            DocumentPropertyImpl result = ( DocumentPropertyImpl )MemberwiseClone();
              result.CloneValue();
              return result;
        }

        /// <summary>
        /// Copies document property data into PropVariant.
        /// </summary>
        /// <param name="variant">Destination object.</param>
        /// <param name="iPropertyId">PropertyId for custom properties.</param>
        /// <returns>True if was able to fill variant; false otherwise.</returns>
        public bool FillPropVariant( PropVariant variant, int iPropertyId )
        {
            if( variant == null )
            throw new ArgumentNullException( "variant" );

              if( m_value == null ) return false;

              if( IsBuiltIn )
              {
            bool bSummary;
            int iIndex = CorrectIndex( m_propertyId, out bSummary );

            if( bSummary )
            {
              variant.PropId = ( PIDSI )iIndex;
            }
            else
            {
              variant.PropId2 = ( PIDDSI )iIndex;
            }
              }
              else
              {
            //variant.SetName( m_strName );
            variant.PropId = ( PIDSI )iPropertyId;
              }

              switch( m_type )
              {
            case ExcelPropertyType.Bool:
              variant.Bool = ( bool )m_value;
              break;

            case ExcelPropertyType.Int:
              variant.Int = ( int )m_value;
              break;

            case ExcelPropertyType.Int32:
              variant.Int32 = ( int )m_value;
              break;

            case ExcelPropertyType.Double:
              variant.Double = ( double )m_value;
              break;

            case ExcelPropertyType.DateTime:
              variant.DateTime = ( DateTime )m_value;
              break;

            case ExcelPropertyType.String:
              variant.String = m_value.ToString();
              break;

            case ExcelPropertyType.Blob:
              variant.SetBlob( ( byte[] )m_value );
              break;

            case ExcelPropertyType.StringArray:
              variant.SetStringArray( ( string[] )m_value );
              break;

            case ExcelPropertyType.ObjectArray:
              variant.SetObjectArray( ( object[] )m_value );
              break;

            default: return false;
              }

              return true;
              // Here we have to assign correct value.
              //variant.Bool = false;
        }

        /// <summary>
        /// Saves property into IPropertyStorage.
        /// </summary>
        /// <param name="storProp">Storage to save into.</param>
        /// <param name="variant">Property variant used as buffer.</param>
        /// <param name="iPropertyId">Property id for custom properties.</param>
        [CLSCompliant( false )]
        public void Write( IPropertyStorage storProp, PropVariant variant, int iPropertyId )
        {
            if( storProp == null )
            throw new ArgumentNullException( "storProp" );

              if( variant == null )
            throw new ArgumentNullException( "variant" );

              FillPropVariant( variant, iPropertyId );
              variant.Write( storProp );

              if( !IsBuiltIn )
              {
            uint uiPropId = ( uint )iPropertyId;
            storProp.WritePropertyNames( 1, ref uiPropId, ref m_strName );
              }

              if( LinkToContent )
              {
            // First step - we have to find out what id was used for the property.
            //storProp.ReadMultiple( 1,
            //variant.Read( storProp, true );
            variant.PropId = ( PIDSI )iPropertyId + PropVariant.DEF_LINK_BIT;
            variant.String = m_strLinkSource;
            variant.Write( storProp );
              }
        }

        /// <summary>
        /// Sets value of LinkSource property.
        /// </summary>
        /// <param name="variant">Variant that contains value to set.</param>
        protected internal void SetLinkSource( PropVariant variant )
        {
            if( variant == null )
            throw new ArgumentNullException( "variant" );

              if( variant.Type != VarEnum.VT_LPSTR && variant.Type != VarEnum.VT_LPWSTR )
            throw new ArgumentOutOfRangeException( "LinkSource" );

              LinkSource = variant.Value.ToString();
        }

        /// <summary>
        /// Creates copy of the internal value.
        /// </summary>
        private void CloneValue()
        {
            if( m_value == null ) return;

              switch( m_type )
              {
            case ExcelPropertyType.Blob:
              m_value = CloneUtils.CloneByteArray( Blob );
              break;

            case ExcelPropertyType.StringArray:
              m_value = CloneUtils.CloneStringArray( StringArray );
              break;

            case ExcelPropertyType.ObjectArray:
              m_value = CloneUtils.CloneArray( ObjectArray );
              break;
              }
        }

        /// <summary>
        /// Tries to detect and set property type.
        /// </summary>
        private void DetectPropertyType()
        {
            if( m_value is string )
              {
            m_type = ExcelPropertyType.String;
              }
              else if( m_value is double )
              {
            m_type = ExcelPropertyType.Double;
              }
              else if( m_value is int )
              {
            m_type = ExcelPropertyType.Int;
              }
              else if( m_value is bool )
              {
            m_type = ExcelPropertyType.Bool;
              }
              else if( m_value is DateTime )
              {
            m_type = ExcelPropertyType.DateTime;
              }
              else if( m_value is object[] )
              {
            m_type = ExcelPropertyType.ObjectArray;
              }
              else if( m_value is string[] )
              {
            m_type = ExcelPropertyType.StringArray;
              }
              else if( m_value is byte[] )
              {
            m_type = ExcelPropertyType.Blob;
              }
        }

        #endregion Methods
    }
}