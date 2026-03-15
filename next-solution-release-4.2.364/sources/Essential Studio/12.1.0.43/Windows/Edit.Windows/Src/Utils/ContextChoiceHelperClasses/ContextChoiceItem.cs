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
using System.Drawing;
using System.Windows.Forms;

using Syncfusion.Windows.Forms.Edit.Interfaces;
using Syncfusion.Windows.Forms.Edit.Enums;

namespace Syncfusion.Windows.Forms.Edit.Utils
{
  /// <summary>
  /// Single item in the context choice list.
  /// </summary>
  internal class ContextChoiceItem
    : TreeNode
    , IContextChoiceItem
  {
    #region Class Private Members
    /// <summary>
    /// Specifies tooltip of the item.
    /// </summary>
    private string m_sTip;
    /// <summary>
    /// Specifies item's image.
    /// </summary>
    private INamedImage m_image;
    /// <summary>
    /// Specifies whether item should be visible.
    /// </summary>
    private bool m_bVisible = true;
    /// <summary>
    /// Specifies ID of the item.
    /// </summary>
    private int m_id;
		/// <summary>
		/// Type of item.
		/// </summary>
		private ContextChoiceItemType m_type = ContextChoiceItemType.Default;
    #endregion

    #region Class Static Members
    /// <summary>
    /// ID of the last item.
    /// </summary>
    private static int _idLast;
    #endregion
  
    #region Class Initialization
    /// <summary>
    /// Creates and initializes new ContextChoiceItem object.
    /// </summary>
    /// <param name="text">Text of the item</param>
    /// <param name="tooltip">Tooltip of the item.</param>
    /// <param name="image">Image of the item.</param>
    public ContextChoiceItem( string text, string tooltip, INamedImage image )
			: this( text, tooltip, image, ContextChoiceItemType.Default )
    {
    }
		/// <summary>
		/// Creates and initializes new ContextChoiceItem object.
		/// </summary>
		/// <param name="text">Text of the item</param>
		/// <param name="tooltip">Tooltip of the item.</param>
		/// <param name="image">Image of the item.</param>
		/// <param name="type">Type of item.</param>
		public ContextChoiceItem( string text, string tooltip, INamedImage image, ContextChoiceItemType type )
		{
			if( null == text || text.Length == 0 )
				throw new ArgumentException( "Text can not be empty.", "text" );

			if( tooltip == null )
				tooltip = string.Empty;

			m_image = image;
			m_sTip = tooltip;
			Text = text;
			m_type = type;

			m_id = _idLast++;
		}
		#endregion

    #region Class Properties
    /// <summary>
    /// Gets tooltip, assigned to the context choice item.
    /// </summary>
    public string ToolTip
    {
      get
      {
        return m_sTip;
      }
    }
    /// <summary>
    /// Gets or sets named image, assigned to the context choice item.
    /// </summary>
    public INamedImage Image
    {
      get
      {
        return m_image;
      }
      set
      {
        m_image = value;
      }
    }
    /// <summary>
    /// Gets or sets value that indicates whether context choice item is visible.
    /// </summary>
    public bool Visible
    {
      get
      {
        return m_bVisible;
      }
      set
      {
        m_bVisible = value;
      }
    }
    /// <summary>
    /// Gets ID of the item.
    /// </summary>
    public int ID
    {
      get
      {
        return m_id;
      }
    }
		/// <summary>
		/// Gets or sets type of item.
		/// </summary>
		public ContextChoiceItemType Type
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
    #endregion

    #region Class Overrides
    /// <summary>
    /// Clones items.
    /// </summary>
    /// <returns>Clone of the item.</returns>
    /// <remarks>Such properties are copied: Text, ToolTip, Image, ID, ForeColor and BackColor.</remarks>
    public override object Clone()
    {
      ContextChoiceItem item = new ContextChoiceItem( Text, ToolTip, Image );
      item.m_id = m_id;
      item.ForeColor = ForeColor;
      item.BackColor = BackColor;
      return item;
    }
    #endregion

  }
}