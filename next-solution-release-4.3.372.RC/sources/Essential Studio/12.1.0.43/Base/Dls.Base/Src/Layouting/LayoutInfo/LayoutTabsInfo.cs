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
	/// Summary description for LayoutTabsInfo.
	/// </summary>
	public class LayoutTabsInfo
	  : LayoutInfo
	{
    #region Members
	  /// <summary>
	  /// 
	  /// </summary>
    protected double m_defaultTabWidth;
	  /// <summary>
	  /// 
	  /// </summary>
    protected double m_pageMarginLeft;
	  /// <summary>
	  /// 
	  /// </summary>
	  internal ArrayList m_list = new ArrayList();
	  /// <summary>
	  /// 
	  /// </summary>
	  internal LayoutTab m_currTab = new LayoutTab( );
    #endregion

    #region Properties
    /// <summary>
    /// Gets the width of the default tab.
    /// </summary>
    /// <value>The width of the default tab.</value>
    public double DefaultTabWidth
    {
      get
      {
        return m_defaultTabWidth;
      }
    }
    /// <summary>
    /// Gets the curr tab leader.
    /// </summary>
    /// <value>The curr tab leader.</value>
    public TabLeader CurrTabLeader
    {
      get
      {
        return m_currTab.TabLeader;
      }
    }
    /// <summary>
    /// Gets the curr tab justification.
    /// </summary>
    /// <value>The curr tab justification.</value>
    public TabJustification CurrTabJustification
    {
      get
      {
        return m_currTab.Justification;
      }
    }
    #endregion
	  
	  #region Public Methods
    /// <summary>
    /// Gets the next tab position.
    /// </summary>
    /// <param name="position">The position.</param>
    /// <returns></returns>
    public double GetNextTabPosition( double position )
    {
      double fPosition = position;
      bool breaked = false;

      if( m_list.Count > 0 )
      {
        for( int i = m_list.Count - 1; i > -1; i-- )
        {
          if( !( ( ( LayoutTab )m_list[ i ] ).Position > position ) )
          {
            if( i > 0 && ( ( ( LayoutTab )m_list[ i - 1 ] ).Position > position ) )
              continue;
            
            if( i != m_list.Count - 1 )
            {
              if( ( ( LayoutTab )m_list[ i + 1 ] ).Justification != TabJustification.Bar )
              {
                fPosition = ( ( LayoutTab )m_list[ i + 1 ] ).Position;
                m_currTab = ( LayoutTab )m_list[ i + 1 ];
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

        if( !breaked && ( ( LayoutTab )m_list[ 0 ] ).Justification != TabJustification.Bar )
        {
          fPosition = ( ( LayoutTab )m_list[ 0 ] ).Position;
          m_currTab = ( LayoutTab )m_list[ 0 ];
        }
      }

      // if position > max tabs position
      if( fPosition == position )
      {
        m_currTab = new LayoutTab();
        position -= m_pageMarginLeft;
        double cnt = ( position - ( position % DefaultTabWidth ) ) / DefaultTabWidth;
        fPosition = ( cnt + 1 ) * DefaultTabWidth;
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
      m_list.Add( new LayoutTab( position, justification, leader ) );
    }
    #endregion
	     
    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="LayoutTabsInfo"/> class.
    /// </summary>
    /// <param name="childLayoutDirection">if set to <c>true</c> [b top subtract area].</param>
	  public LayoutTabsInfo( ChildrenLayoutDirection childLayoutDirection )
	    :base( childLayoutDirection )
	  {}
    #endregion

    #region Internal declarations
    /// <summary>
    /// 
    /// </summary>
    internal class LayoutTab
    {
      #region Members
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

      #region Properties
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
      
      #region Constructors
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