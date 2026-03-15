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
using Syncfusion.DLS.XML;
using System;
#endregion

namespace Syncfusion.DLS
{
	/// <summary>
	/// Summary description for Tab.
	/// </summary>
  public class Tab
    : XDLSSerializableBase,
    ICloneable
  {
    #region Class members
    /// <summary>
    /// 
    /// </summary>
    private TabJustification m_jc = TabJustification.Left;
    /// <summary>
    /// 
    /// </summary>
    private TabLeader m_tlc = DLS.TabLeader.NoLeader;
    /// <summary>
    /// 
    /// </summary>
    private float m_tabPosition = 0;
	  /// <summary>
	  /// 
	  /// </summary>
	  private float m_tabDeletePosition;
    #endregion

    #region Class properties
    /// <summary>
    /// Gets or sets the justification.
    /// </summary>
    /// <value>The justification.</value>
    public TabJustification Justification
    {
      get
      {
        return m_jc;
      }
      set
      {
        if( value != m_jc )
        {
          m_jc = value;
        }
      }
    }
    /// <summary>
    /// Gets or sets the tab leader.
    /// </summary>
    /// <value>The tab leader.</value>
    public TabLeader TabLeader
    {
      get
      {
        return m_tlc;
      }
      set
      {
        if( value != m_tlc )
        {
          m_tlc = value;
        }
      }
    }
    /// <summary>
    /// Gets or sets the position.
    /// </summary>
    /// <value>The position.</value>
    public float Position
    {
      get
      {
        return m_tabPosition;
      }
      set
      {
        if( value != m_tabPosition )
        {
          m_tabPosition = value;
        }
      }
    }
	  
    /// <summary>
    /// Gets or sets the delete position.
    /// </summary>
    /// <value>The delete position.</value>
	  public float DeletePosition
	  {
	    get
	    {
	      return m_tabDeletePosition;
	    }
	    set
	    {
	      m_tabDeletePosition = value;
	    }
	  }
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Initializes a new instance of the <see cref="Tab"/> class.
    /// </summary>
    internal Tab( IDocument doc )
      : base( doc )
    {}
    /// <summary>
    /// Initializes a new instance of the <see cref="Tab"/> class.
    /// </summary>
    /// <param name="doc">The doc.</param>
    /// <param name="position">The position.</param>
    /// <param name="justification">The justification.</param>
    /// <param name="leader">The leader.</param>
	  internal Tab( IDocument doc, float position, TabJustification justification, TabLeader leader  )
	  : this( doc, position, 0, justification, leader )
	  {
	  }
    /// <summary>
    /// Initializes a new instance of the <see cref="Tab"/> class.
    /// </summary>
    /// <param name="doc">The doc.</param>
    /// <param name="position">The position.</param>
    /// <param name="deletePosition"></param>
    /// <param name="justification">The justification.</param>
    /// <param name="leader">The leader.</param>
    internal Tab( IDocument doc, float position, float deletePosition, TabJustification justification, TabLeader leader  )
      : this( doc )
    {
      m_tabPosition = position;
      m_jc = justification;
      m_tlc = leader;
      m_tabDeletePosition = deletePosition;
    }
    #endregion

    #region Class overrides
    /// <summary>
    /// </summary>
    protected override void InitXDLSHolder()
    {
      XDLSHolder.SkipID = true;
    }
    /// <summary>
    /// </summary>
    /// <param name="writer"></param>
    protected override void WriteXmlAttributes( IXDLSAttributeWriter writer )
    {
      writer.WriteValue( XDLSConstants.TabPositionAttr, Position );
      writer.WriteValue( XDLSConstants.TabJustificationAttr, Justification );
      writer.WriteValue( XDLSConstants.TabLeaderAttr, TabLeader );
      writer.WriteValue( XDLSConstants.TabDeleteAttr, DeletePosition );
    }
    /// <summary>
    /// </summary>
    /// <param name="reader"></param>
    protected override void ReadXmlAttributes( IXDLSAttributeReader reader )
    {
      if( reader.HasAttribute( XDLSConstants.TabPositionAttr ))
      {
        Position = reader.ReadFloat( XDLSConstants.TabPositionAttr );
      }
      if( reader.HasAttribute( XDLSConstants.TabJustificationAttr ) )
      {
        Justification =
          ( TabJustification )
          reader.ReadEnum( XDLSConstants.TabJustificationAttr, typeof( TabJustification ) );
      }
      if( reader.HasAttribute( XDLSConstants.TabLeaderAttr ) )
      {
        TabLeader = ( TabLeader )reader.ReadEnum( XDLSConstants.TabLeaderAttr, typeof( TabLeader ) );
      }
      if( reader.HasAttribute( XDLSConstants.TabDeleteAttr ) )
      {
        DeletePosition = reader.ReadFloat( XDLSConstants.TabDeleteAttr );
      }
    }    
    #endregion

    #region ICloneable Members
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public object Clone()
    {
      Tab tab = new Tab( Document );
      tab.Position = Position;
      tab.TabLeader = TabLeader;
      tab.Justification = Justification;
      tab.DeletePosition = DeletePosition;

      return tab;
    }

    #endregion
  }
}
