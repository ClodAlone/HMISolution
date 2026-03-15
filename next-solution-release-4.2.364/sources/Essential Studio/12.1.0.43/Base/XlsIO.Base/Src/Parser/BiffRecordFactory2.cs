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


#if SyncfusionFramework2_0

namespace Syncfusion.XlsIO.Parser
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Runtime.InteropServices;

    using Syncfusion.XlsIO.Implementation;
    using Syncfusion.XlsIO.Implementation.Security;
    using Syncfusion.XlsIO.Parser.Biff_Records;

    /// <summary>
    /// This class contains information about all known biff records.
    /// Used for registering the biff record type, creating new biff records
    /// and extracting them from a stream.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude]
    [CLSCompliant( false )]
    public class BiffRecordFactory
    {
        #region Fields

        /// <summary>
        /// Default size for the internal dictionaries
        /// </summary>
        private const int DEF_RESERVE_SIZE = 200;

        /// <summary>
        /// 
        /// </summary>
        private static MemoryConverter m_converter = new MemoryConverter();

        /// <summary>
        /// code-to-construstor pair
        /// </summary>
        private static Dictionary<int, BiffRecordRaw> m_dict = new Dictionary<int, BiffRecordRaw>( DEF_RESERVE_SIZE );

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initialize internal dictionary by Records
        /// </summary>
        static BiffRecordFactory()
        {
            #if DEBUG
              DateTime now = DateTime.Now;
            #endif

              //Assembly asm = Assembly.GetExecutingAssembly();
              Type[]  types = ApplicationImpl.AssemblyTypes;//asm.GetTypes();

              for( int i = 0, len = types.Length; i < len; i++ )
              {
            Type type = types[ i ];
            object[] attribs = type.GetCustomAttributes( typeof( BiffAttribute ), false );
            int iAttributeCount = ( attribs != null ) ? attribs.Length : 0;

            if( iAttributeCount > 0 )
            {
              Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, type.FullName,
            "Records Found" );

              object o = Activator.CreateInstance( type );

              // This if was add to optimize performance, since most of records have only one BiffAttribute.
              if( iAttributeCount == 1 )
              {
            BiffAttribute attrib = ( BiffAttribute )attribs[ 0 ];
            int iCode = ( int )attrib.Code;
            m_dict[ iCode ] = ( BiffRecordRaw )o;
              }
              else
              {

            BiffRecordRaw record = ( BiffRecordRaw )o;

              for( int j = 0; j < iAttributeCount; j++ )
              {
                BiffAttribute attrib = ( BiffAttribute )attribs[ j ];
                int iCode = ( int )attrib.Code;
                record.SetRecordCode( iCode );
                m_dict[ iCode ] = record;// as BiffRecordRaw;
                record = ( BiffRecordRaw )record.Clone();
              }
              }
            }
              }

            #if DEBUG
              TimeSpan diff = DateTime.Now.Subtract( now );
              Debug.WriteLine( diff, "Class Extract Performance" );
            #endif
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Memory converter.
        /// </summary>
        public static MemoryConverter Converter
        {
            get
              {
            return m_converter;
              }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Used for adding additional BiffRecords in runtime
        /// </summary>
        /// <param name="type">BiffRecord typeof information</param>
        public static void AddCustomRecord( Type type )
        {
            InnerRegisterRecord( type );
        }

        /// <summary>
        /// Register multiple classes on one call
        /// </summary>
        /// <param name="types">array of Biff Records type information</param>
        public static void AddCustomRecord( Type[] types )
        {
            for( int i = 0, len = types.Length; i < len; i++ )
              {
            InnerRegisterRecord( types[ i ] );
              }
        }

        /// <summary>
        /// Extracts from the BinaryReader type of the next record.
        /// </summary>
        /// <param name="reader">BinaryReader that contains record to extract.</param>
        /// <returns>Type of the next record in the BinaryReader.</returns>
        /// <exception cref="System.ArgumentNullException">When reader is null.</exception>
        /// <exception cref="System.ApplicationException">
        /// When code of the extracted record is zero.
        /// </exception>
        public static int ExtractRecordType( BinaryReader reader )
        {
            if( reader == null )
            throw new ArgumentNullException( "reader" );

              Stream stream = reader.BaseStream;
              long lPos = stream.Position;
              int iCode = reader.ReadInt16();
              stream.Position = lPos;

              if( iCode == 0 )
            throw new ApplicationException( "Cannot find record identifier in stream!" );

              return iCode;
        }

        /// <summary>
        /// Extract from stream Next Record type.
        /// </summary>
        /// <param name="stream">Stream that contains record to extract.</param>
        /// <returns>Extracted record.</returns>
        public static int ExtractRecordType( Stream stream )
        {
            if( stream == null )
            throw new ArgumentNullException( "stream" );

              if( stream.CanSeek == false || stream.CanRead == false )
            throw new ApplicationException( "Stream must permit seeking and reading operations" );

              long lPos = stream.Position;
              int retValue = ( stream.ReadByte() & 0xff ) + ( ( stream.ReadByte() & 0xff ) << 8 );

              if( retValue == 0 )
            throw new ApplicationException( "Cannot find record identifier in stream!" );

              if( !m_dict.ContainsKey( retValue ) )
              {
            retValue = ( int )TBIFFRecord.Unknown;
              }

              return retValue;
        }

        /// <summary>
        /// Create empty record by specified type.
        /// </summary>
        /// <param name="type">Type of the record that should be created.</param>
        /// <returns>Created record if succeded, null otherwise.</returns>
        public static BiffRecordRaw GetRecord( TBIFFRecord type )
        {
            int iCode = ( int )type;
              return GetRecord( iCode );
        }

        /// <summary>
        /// Create empty record by specified type.
        /// </summary>
        /// <param name="type">Type of the record that should be created.</param>
        /// <returns>Created record if succeded, null otherwise.</returns>
        public static BiffRecordRaw GetRecord( int type )
        {
            object value = ( m_dict.ContainsKey( type ) )
            ? m_dict[ type ]
            : null;

              ICloneable toClone = null;

              if( value != null )
              {
            toClone = value as ICloneable;
              }
              else if( m_dict.ContainsKey( ( int )TBIFFRecord.Unknown ) )
              {
            UnknownRecord record = ( UnknownRecord )m_dict[ ( int )TBIFFRecord.Unknown ];
            record.RecordCode = type;
            toClone = record;
              }

              if( toClone != null )
              {
            return toClone.Clone() as BiffRecordRaw;
              }

              return null;
        }

        /// <summary>
        /// Extracts specified record from the reader
        /// </summary>
        /// <param name="type">Type of the record to be extracted</param>
        /// <param name="reader">Reader that contains record</param>
        /// <param name="provider">Object that provider access to the record data.</param>
        /// <param name="decryptor">
        /// Decryptor used to parse encrypted records.
        /// This argument can be null when no decryption is required.
        /// </param>
        /// <param name="arrBuffer">Temporary buffer needed for some operations.</param>
        /// <returns>Extracted record</returns>
        /// <exception cref="System.ArgumentNullException">
        /// When specified reader is null
        /// </exception>
        public static BiffRecordRaw GetRecord( int type, BinaryReader reader,
            DataProvider provider, IDecryptor decryptor, byte[] arrBuffer)
        {
            if( reader == null )
            throw new ArgumentNullException( "reader" );

              BiffRecordRaw retValue = GetRecord( type );

              // if record exists then infill it
              if( retValue != null )
              {
            retValue.FillRecord( reader, provider, decryptor, arrBuffer );
              }

              return retValue;
        }

        /// <summary>
        /// Extracts specified record from the reader.
        /// </summary>
        /// <param name="type">Type of the record to be extracted.</param>
        /// <param name="reader">Reader that contains record.</param>
        /// <param name="arrBuffer">Temporary buffer needed for some operations.</param>
        /// <returns>Extracted record.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// When specified reader is null.
        /// </exception>
        public static BiffRecordRaw GetRecord( TBIFFRecord type, BinaryReader reader,
            byte[] arrBuffer)
        {
            return GetRecord( ( int )type, reader, arrBuffer );
        }

        /// <summary>
        /// Extracts specified record from the reader.
        /// </summary>
        /// <param name="type">Type of the record to be extracted.</param>
        /// <param name="reader">Reader that contains record.</param>
        /// <param name="arrBuffer">Temporary buffer needed for some operations.</param>
        /// <returns>Extracted record.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// When specified reader is null.
        /// </exception>
        public static BiffRecordRaw GetRecord( int type, BinaryReader reader,
            byte[] arrBuffer)
        {
            if( reader == null )
            throw new ArgumentNullException( "reader" );

              BiffRecordRaw retValue = GetRecord( type );

              // if record exists then infill it
              if( retValue != null )
              {
            retValue.FillRecord( reader, null, null, arrBuffer );
              }

              return retValue;
        }

        /// <summary>
        /// Extracts record from the BinaryReader.
        /// </summary>
        /// <param name="reader">BinaryReader that contains record to extract.</param>
        /// <param name="provider">Object that provides access to the data.</param>
        /// <param name="arrBuffer">Temporary buffer needed for some operations.</param>
        /// <returns>Extracted record.</returns>
        public static BiffRecordRaw GetRecord( BinaryReader reader, DataProvider provider,
            byte[] arrBuffer)
        {
            return GetRecord( ExtractRecordType( reader ), reader, provider,
            null, arrBuffer );
        }

        /// <summary>
        /// Extracts record from the BinaryReader.
        /// </summary>
        /// <param name="reader">BinaryReader that contains record to extract.</param>
        /// <param name="provider">Object that provides access to the data.</param>
        /// <param name="decryptor">Object used to decrypt encrypted records.</param>
        /// <param name="arrBuffer">Temporary buffer needed for some operations.</param>
        /// <returns>Extracted record.</returns>
        public static BiffRecordRaw GetRecord( BinaryReader reader, DataProvider provider,
            IDecryptor decryptor, byte[] arrBuffer)
        {
            return GetRecord( ExtractRecordType( reader ), reader, provider,
            decryptor, arrBuffer );
        }

        /// <summary>
        /// Extracts record from array of bytes.
        /// </summary>
        /// <param name="provider">Object that provides access to the data.</param>
        /// <param name="iOffset">Offset to the record's start.</param>
        /// <param name="version">Excel version used for infill.</param>
        /// <returns>Extracted record.</returns>
        public static BiffRecordRaw GetRecord( DataProvider provider, int iOffset,
            ExcelVersion version)
        {
            if( provider == null )
            throw new ArgumentNullException( "provider" );

              int recordType = provider.ReadInt16( iOffset );
              iOffset += 2;

              BiffRecordRaw result = GetRecord( recordType );

              int iLength = provider.ReadInt16( iOffset );
              result.Length = iLength;
              iOffset += 2;

              result.ParseStructure( provider, iOffset, iLength, version );

              return result;
        }

        /// <summary>
        /// Extracts unknown record from the stream
        /// </summary>
        /// <param name="stream">Stream that contains needed record</param>
        /// <returns>Extracted unknown record</returns>
        public static BiffRecordRaw GetUntypedRecord( Stream stream )
        {
            int size;
              UnknownRecord raw = new UnknownRecord( stream, out size );

              return raw;
        }

        /// <summary>
        /// Extracts unknown record from the stream
        /// </summary>
        /// <param name="reader">Reader that contains record</param>
        /// <returns>Extracted unknown record</returns>
        public static BiffRecordRaw GetUntypedRecord( BinaryReader reader )
        {
            int size;
              UnknownRecord raw = new UnknownRecord( reader, out size );

              return raw;
        }

        /// <summary>
        /// Remove from Biff Factory registration of specified type.
        /// </summary>
        /// <param name="type">
        /// Type of class which can be produced by BiffFactory on demand
        /// </param>
        public static void RemoveCustomRecord( Type type )
        {
            if( type == null )
            throw new ArgumentNullException( "type" );

              object[] attribs = type.GetCustomAttributes( typeof( BiffAttribute ), true );

              if( attribs.Length > 0 )
              {
            BiffAttribute attrib = ( BiffAttribute )attribs[ 0 ];

            int iCode = ( int )attrib.Code;
            m_dict.Remove( iCode );
              }
        }

        /// <summary>
        /// Remove registration of BiffRecords from Biff Factory
        /// </summary>
        /// <param name="types">Array of BiffRecords types</param>
        public static void RemoveCustomRecord( Type[] types )
        {
            if( types == null )
            throw new ArgumentNullException( "types" );

              for( int i = 0, len = types.Length; i < len; i++ )
              {
            RemoveCustomRecord( types[ i ] );
              }
        }

        /// <summary>
        /// Registers biff record type in the inner collections.
        /// </summary>
        /// <param name="type">Type of the record to register.</param>
        /// <exception cref="System.ArgumentNullException">If type is null.</exception>
        /// <exception cref="System.ArgumentException">
        /// When specified by type class is not derived from BiffRecordRaw class
        /// or if type does not contain BiffAttribute.
        /// </exception>
        private static void InnerRegisterRecord( Type type )
        {
            if( type == null )
            throw new ArgumentNullException( "type" );

              if( type.IsSubclassOf( typeof( BiffRecordRaw ) ) == false )
            throw new ArgumentException( "class must be derived from BiffRecordRaw class", "type" );

              object[] arrAttribs = type.GetCustomAttributes( typeof( BiffAttribute ), true );

              if( arrAttribs.Length > 0 )
              {
            Debug.WriteLine( type.FullName, "Records Found" );

            BiffAttribute attrib = ( BiffAttribute )arrAttribs[ 0 ];

            // get default and with space reserve constructors
            int iCode = ( int )attrib.Code;
            m_dict[ iCode ] = Activator.CreateInstance( type ) as BiffRecordRaw;
              }
              else
              {
            throw new ArgumentException( "Type does not contain any BiffAttribute " +
              "specification. Type: " + type.FullName, "type" );
              }
        }

        #endregion Methods
    }
}

#endif