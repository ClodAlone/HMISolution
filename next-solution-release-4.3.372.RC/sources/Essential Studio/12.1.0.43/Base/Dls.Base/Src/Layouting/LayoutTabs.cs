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
#endregion

namespace Syncfusion.Layouting
{
	/// <summary>
	/// Summary description for LayoutTabs.
	/// </summary>
  /// <summary>
  /// 
  /// </summary>
  public class LayoutTabs : ArrayList
  {
    #region Class members
    /// <summary>
    /// 
    /// </summary>
    private float m_fDefaultTabWidth;
    /// <summary>
    /// 
    /// </summary>
    private bool m_bIsTab = false;
    /// <summary>
    /// 
    /// </summary>
    private float m_fPageMarginLeft;
    /// <summary>
    /// 
    /// </summary>
    private LayoutTab m_currLayoutTab = new LayoutTab();
    #endregion
    
    #region Class properties
    /// <summary>
    /// Gets or sets the page margin left.
    /// </summary>
    /// <value>The page margin left.</value>
    public float PageMarginLeft
    {
      get
      {
        return m_fPageMarginLeft;
      }
      set
      {
        m_fPageMarginLeft = value;
      }
    }
    /// <summary>
    /// Gets or sets a value indicating whether this instance is tab.
    /// </summary>
    /// <value><c>true</c> if this instance is tab; otherwise, <c>false</c>.</value>
    public bool IsTab
    {
      get
      {
        return m_bIsTab;
      }
      set
      {
        if( value != m_bIsTab )
        {
          m_bIsTab = value;
        }
      }
    }
    /// <summary>
    /// Gets or sets the length of the auto tab.
    /// </summary>
    /// <value>The length of the auto tab.</value>
    public float DefaultTabWidth
    {
      get
      {
        return m_fDefaultTabWidth;
      }
      set
      {
        if( value != m_fDefaultTabWidth )
          m_fDefaultTabWidth = value;
      }
    }
    /// <summary>
    /// Gets or sets the <see cref="LayoutTab"/> at the specified index.
    /// </summary>
    /// <value></value>
    new internal LayoutTab this[ int index ]
    {
      get
      {
        return (LayoutTab)base[index];
      }
      set
      {
        base[index] = value;
      }
    }
    
    /// <summary>
    /// Gets or sets the curr tab leader.
    /// </summary>
    /// <value>The curr tab leader.</value>
    public TabLeader CurrTabLeader
    {
      get
      {
        return m_currLayoutTab.TabLeader;
      }
      set
      {
        m_currLayoutTab.TabLeader = value;
      }
    }
    /// <summary>
    /// Gets or sets the curr tab justification.
    /// </summary>
    /// <value>The curr tab justification.</value>
    public TabJustification CurrTabJustification
    {
      get
      {
        return m_currLayoutTab.Justification;
      }
      set
      {
        m_currLayoutTab.Justification = value;
      }
    }
    #endregion

    #region Class Public Methods
    /// <summary>
    /// Gets the next tab position.
    /// </summary>
    /// <param name="position">The position.</param>
    /// <returns></returns>
    public float GetNextTabPosition( float position )
    {
      float fPosition = position;
      bool breaked = false;
      
      if( Count > 0 )
      {
        for( int i = Count - 1; i > -1; i-- )
        {
          if( !(this[ i ].Position > position) )
          {
            if( i != Count - 1 )
            {
              if( this[ i + 1 ].Justification != TabJustification.Bar )
              {
                fPosition = this[ i + 1 ].Position;
                m_currLayoutTab = this[ i + 1 ];
              }
              else
              {
                fPosition = position;
              }
            }
            breaked = true;
            break;
          }
        }

        if( !breaked && this[ 0 ].Justification != TabJustification.Bar )
        {
          fPosition = this[ 0 ].Position;
          m_currLayoutTab = this[ 0 ];
        }
      }
      
      // if position > max tabs position
      if( fPosition == position )
      {
        m_currLayoutTab = new LayoutTab();
        position -= PageMarginLeft;
        float cnt = ( position - ( position % m_fDefaultTabWidth ) ) / m_fDefaultTabWidth ;
        fPosition = ( cnt + 1 ) * m_fDefaultTabWidth;
      }
      
      return fPosition - position;
    }
    
    /// <summary>
    /// Adds the tab.
    /// </summary>
    /// <param name="position">The position.</param>
    /// <param name="justification">The justification.</param>
    /// <param name="leader">The leader.</param>
    public void AddTab( float position, TabJustification justification, TabLeader leader )
    {
      base.Add( new LayoutTab( position, justification, leader ) );
    }
    #endregion
    
    #region Class internal declarations
    /// <summary>
    /// 
    /// </summary>
    internal class LayoutTab
    {
      #region Class members
      /// <summary>
      /// 
      /// </summary>
      private TabJustification m_jc;
      /// <summary>
      /// 
      /// </summary>
      private TabLeader m_tlc;
      /// <summary>
      /// 
      /// </summary>
      private float m_tabPosition;
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
      #endregion
    
      #region Class Initialize/Finalize methods
      /// <summary>
      /// Initializes a new instance of the <see cref="LayoutTab"/> class.
      /// </summary>
      internal LayoutTab()
        : this( 0, TabJustification.Left, TabLeader.NoLeader )
      {}
      /// <summary>
      /// Initializes a new instance of the <see cref="LayoutTab"/> class.
      /// </summary>
      /// <param name="position">The position.</param>
      /// <param name="justification">The justification.</param>
      /// <param name="leader">The leader.</param>
      internal LayoutTab( float position, TabJustification justification, TabLeader leader )
      {
        m_tabPosition = position;
        m_jc = justification;
        m_tlc = leader;
      }    
      #endregion
    }
    #endregion
  }
}
