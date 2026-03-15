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
using System.Reflection;
using System.Collections;

using Syncfusion.XlsIO.Implementation;
using System.Collections.Generic;
#endregion

namespace Syncfusion.XlsIO.Parser.Biff_Records.MsoDrawing
{
  /// <summary>
  /// Summary description for MsoRecordFactory.
  /// </summary>
  [Syncfusion.Documentation.DocumentationExclude()]
  public class MsoRecordFactory
  {
    #region Class static members
    /// <summary>
    /// 
    /// </summary>
    private static Dictionary<int, MsoBase> m_hashTypes = new Dictionary<int, MsoBase>();
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// To prevent creation without parameters
    /// </summary>
    private MsoRecordFactory()
    {
      //
      // TODO: Add constructor logic here
      //
    }

    /// <summary>
    /// 
    /// </summary>
    static MsoRecordFactory()
    {
      
#if ( WINRT )
          if(ApplicationImpl.AssemblyTypes==null)
              ApplicationImpl.InitAssemblyTypes();
          Type[] arrTypes = ApplicationImpl.AssemblyTypes;
#else
           Type[] arrTypes =  ApplicationImpl.AssemblyTypes;
#endif

      for( int i = 0, len = arrTypes.Length; i < len; i++ )
      {
        Type type = arrTypes[ i ];
        TryRegisterType( type );
      }
    }

    /// <summary>
    /// 
    /// </summary>
    private static void TryRegisterType( Type type )
    {
        MsoDrawingAttribute[] attributes;
#if ( WINRT )
        attributes = (MsoDrawingAttribute[])type.GetTypeInfo().GetCustomAttributes(typeof(MsoDrawingAttribute), false);
#else
                attributes = (MsoDrawingAttribute[])
        type.GetCustomAttributes( typeof( MsoDrawingAttribute ), false );
#endif

      if( attributes == null || attributes.Length == 0 ) return;

      MsoBase result = ( MsoBase )Activator.CreateInstance( type );
      m_hashTypes.Add( ( int )attributes[ 0 ].RecordType, result );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [ CLSCompliant( false ) ]
    public static MsoBase GetRecord( MsoRecords type )
    {
      MsoBase instance = m_hashTypes[ ( int )type ];

      if( instance != null )
      {
        instance = ( MsoBase )instance.Clone();
      }

      return instance;
    }
    #endregion
  }
}
