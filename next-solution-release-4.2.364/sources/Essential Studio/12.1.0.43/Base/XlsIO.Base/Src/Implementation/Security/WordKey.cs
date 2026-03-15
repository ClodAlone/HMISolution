#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.XlsIO.Implementation.Security
{
	/// <summary>
	/// Summary description for Word_key.
	/// </summary>
	public class WordKey
	{
    #region Class members
    /// <summary>
    /// 
    /// </summary>
    private byte[] m_baState = new byte[ 256 ];
    /// <summary>
    /// 
    /// </summary>
    private byte m_bX;
    /// <summary>
    /// 
    /// </summary>
    private byte m_bY;
    #endregion

    #region Class properties
    /// <summary>
    /// Gets or sets the status.
    /// </summary>
    /// <value>The status.</value>
    public byte[] Status
    {
      get
      {
        return m_baState;
      }
      set
      {
        if( m_baState != value )
          m_baState = value;
      }
    }
	  
    /// <summary>
    /// Gets or sets the x.
    /// </summary>
    /// <value>The x.</value>
    public byte X
    {
      get
      {
        return m_bX;
      }
      set
      {
        if( m_bX != value )
          m_bX = value;
      }
    }
	  
    /// <summary>
    /// Gets or sets the y.
    /// </summary>
    /// <value>The y.</value>
    public byte Y
    {
      get
      {
        return m_bY;
      }
      set
      {
        if( m_bY != value )
          m_bY = value;
      }
    }
	  
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Initializes a new instance of the <see cref="WordKey"/> class.
    /// </summary>
    public WordKey()
    {
    }
    #endregion 
	}
}
