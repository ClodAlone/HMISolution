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
using System.Collections;
using System.Xml;
#endregion  

namespace Syncfusion.DLS.XML
{
  /// <summary>
  /// Summary description for DLSXmlHolder.
  /// </summary>
  [ Syncfusion.Documentation.DocumentationExclude() ]
  public class XDLSHolder
  {
    #region Class members
    private int m_id = -1;
    private Hashtable m_hashElements = null;
    private Hashtable m_hashRefElements = null;
    private bool m_bCleared = true;
    private bool m_bSkipID = false;
    private bool m_bSkipMe = false;
    #endregion

    #region Class properties
    /// <summary>
    /// 
    /// </summary>
    public int ID
    {
      get
      {
        return m_id;
      }
      set
      {
        m_id = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public bool Cleared
    {
      get
      {
        return m_bCleared;
      }
      set
      {
        if( value != m_bCleared )
        {
          if( value )
          {
            Clear();
          }
          else
          {
            m_bCleared = false;
          }
        }
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public bool SkipID
    {
      get
      {
        return m_bSkipID;
      }
      set
      {
        m_bSkipID = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public bool SkipMe
    {
      get
      {
        return m_bSkipMe;
      }
      set
      {
        m_bSkipMe = value;
      }
    }
    #endregion

    #region Class initialize/finalize methods
    /// <summary>
    /// 
    /// </summary>
    public XDLSHolder()
    {
    }
    #endregion

    #region Class public methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="tagName"></param>
    /// <param name="value"></param>
    public void AddElement( string tagName, object value )
    {
      if( m_hashElements == null )
      {
        m_hashElements = new Hashtable();
      }

      m_hashElements[ tagName ] = value;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="tagName"></param>
    /// <param name="value"></param>
    public void AddRefElement( string tagName, object value )
    {
      if( m_hashRefElements == null )
      {
        m_hashRefElements = new Hashtable();
      }

      m_hashRefElements[ tagName ] = value;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="writer"></param>
    public void WriteHolder( IXDLSContentWriter writer )
    {
      if( m_hashElements != null )
      {
        foreach( string keyTagName in m_hashElements.Keys )
        {
          writer.WriteChildElement( keyTagName, m_hashElements[ keyTagName ] );
        }
      }

      if( m_hashRefElements != null )
      {
        foreach( string keyTagName in m_hashRefElements.Keys )
        {
          IXDLSSerializable refElement = m_hashRefElements[ keyTagName ] as IXDLSSerializable;
          if( refElement != null )
          {
            writer.WriteChildRefElement( keyTagName, refElement.XDLSHolder.ID );
          }
          else
          {
            //writer.WriteChildRefElement( keyTagName, -1 );
          }
        }
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    public bool ReadHolder( IXDLSContentReader reader )
    {
      if( reader.NodeType == XmlNodeType.Element )
      {
        string tagName = reader.TagName;

        if( m_hashElements != null )
        {
          object value = m_hashElements[ tagName ];

          if( value != null )
          {
            IXDLSFactory factory = value as IXDLSFactory;
            if( factory != null )
            {
              value = factory.Create( reader );
              m_hashElements[ tagName ] = value;
            }

            return reader.ReadChildElement( value );
          }
        }

        if( m_hashRefElements != null )
        {
          if( m_hashRefElements.ContainsKey( tagName ) )
          {
            string sRef = reader.GetAttributeValue( "ref" );

            if( sRef == null )
            {
              m_hashRefElements[ reader.TagName ] = -1;
            }
            else
            {
              m_hashRefElements[ reader.TagName ] = XmlConvert.ToInt32( sRef );
            }

            return false;
          }
        }
      }

      return false;
    }
    /// <summary>
    /// 
    /// </summary>
    public void AfterDeserialization( IXDLSSerializable owner )
    {
      if( m_hashElements != null )
      {
        // Recursive calls AfterDeserialization method for all subitems
        foreach( string keyTagName in m_hashElements.Keys )
        {
          IXDLSSerializable dlsSer = m_hashElements[ keyTagName ] as IXDLSSerializable;

          if( dlsSer != null )
          {
            dlsSer.XDLSHolder.AfterDeserialization( dlsSer );
          }
          else
          {
            IXDLSSerializableCollection dlsSerColl =
              m_hashElements[ keyTagName ] as IXDLSSerializableCollection;

            if( dlsSerColl != null )
            {
              for( int i = 0; i < dlsSerColl.Count; i++ )
              {
                IXDLSSerializable dlsSerItem = dlsSerColl[ i ] as IXDLSSerializable;

                if( dlsSerItem != null )
                {
                  dlsSerItem.XDLSHolder.AfterDeserialization( dlsSerItem );
                }
              }
            }
          }
        }
      }

      // Restore references
      if( m_hashRefElements != null )
      {
        foreach( string keyTagName in m_hashRefElements.Keys )
        {
          int refValue = -1;

          if( m_hashRefElements[ keyTagName ] != null )
          {
            refValue = ( int )m_hashRefElements[ keyTagName ];
          }

          owner.RestoreReference( keyTagName, refValue );
        }
      }
      Clear();
    }
    /// <summary>
    /// 
    /// </summary>
    public void BeforeSerialization()
    {
      if( m_hashElements != null )
      {
        // Recursive calls BeforeSerialization method for all subitems
        foreach( string keyTagName in m_hashElements.Keys )
        {
          IXDLSSerializable dlsSer = m_hashElements[ keyTagName ] as IXDLSSerializable;

          if( dlsSer != null )
          {
            dlsSer.XDLSHolder.Cleared = true;
            dlsSer.XDLSHolder.BeforeSerialization();
          }
          else
          {
            IXDLSSerializableCollection dlsSerColl =
              m_hashElements[ keyTagName ] as IXDLSSerializableCollection;

            if( dlsSerColl != null )
            {
              for( int i = 0; i < dlsSerColl.Count; i++ )
              {
                IXDLSSerializable dlsSerItem = dlsSerColl[ i ] as IXDLSSerializable;

                if( dlsSerItem != null )
                {
                  dlsSerItem.XDLSHolder.Cleared = true;
                  dlsSerItem.XDLSHolder.ID = i; // Update item ID.
                  dlsSerItem.XDLSHolder.BeforeSerialization();
                }
              }
            }
          }
        }
      }
    }
    /// <summary>
    /// 
    /// </summary>
    private void Clear()
    {
      if( m_hashElements != null )
      {
        m_hashElements.Clear();
      }

      if( m_hashRefElements != null )
      {
        m_hashRefElements.Clear();
      }

      m_bCleared = true;
    }
    #endregion
  }
}