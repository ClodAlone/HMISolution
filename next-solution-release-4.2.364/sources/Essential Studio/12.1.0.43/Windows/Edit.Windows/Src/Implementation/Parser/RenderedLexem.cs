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

using Syncfusion.Windows.Forms.Edit.Interfaces;

namespace Syncfusion.Windows.Forms.Edit.Implementation.Parser
{
	/// <summary>
	/// Lexem, that can be used for rendering.
	/// It support position and size info.
	/// </summary>
	public class RenderedLexem
		: Lexem
		, IRenderedLexem
	{
		#region Fields
		/// <summary>
		/// Width of the lexem.
		/// </summary>
		private float m_width;
		/// <summary>
		/// If word-wrapping is enabled, then it is zero-based index of the sub line, where lexem is drawn.
		/// </summary>
		private int m_SubLine;
		/// <summary>
		/// X offset of the lexem. Relative to the lexem`s sub line.
		/// </summary>
		private float m_xOffset;
		/// <summary>
		/// Y offset of the lexem.
		/// </summary>
		private float m_yOffset;
		/// <summary>
		/// Indicates whether lexem is not independent, but is a part of lexem wrapped by char wrapping.
		/// </summary>
		private bool m_bIsPartOfCharWrap = false;
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets width of the lexem.
		/// </summary>
		public float Width
		{
			get
			{
				return m_width;
			}
			set
			{
				m_width = value;
			}
		}
		/// <summary>
		/// Gets or sets zero-based index of the sub line, where lexem is drawn.
		/// </summary>
		public int SubLine
		{
			get
			{
				return m_SubLine;
			}
			set
			{
				m_SubLine = value;
			}
		}
		/// <summary>
		/// Gets or sets X offset of the lexem. Relative to the lexem's sub line.
		/// </summary>
		public float XOffset
		{
			get
			{
				return m_xOffset;
			}
			set
			{
				m_xOffset = value;
			}
		}
		/// <summary>
		/// Gets or sets Y offset of the lexem.
		/// </summary>
		public float YOffset
		{
			get
			{
				return m_yOffset;
			}
			set
			{
				m_yOffset = value;
			}
		}
		/// <summary>
		/// Get or set bool that indicates whether lexem is not independent, but is a part of lexem wrapped by char wrapping.
		/// </summary>
		public bool IsPartOfCharWrap
		{
			get
			{
				return m_bIsPartOfCharWrap;
			}
			set
			{
				m_bIsPartOfCharWrap = value;
			}
		}
		#endregion

		#region Initialization
		/// <summary>
		/// Creates new instance of the class and initializes it.
		/// </summary>
		/// <param name="text">Text of the lexem.</param>
		/// <param name="config">Config of the lexem.</param>
		public RenderedLexem( string text, IConfigLexem config )
			: base( text, config )
		{
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Unites two lexems (adds 'lexem' to current).
		/// </summary>
		/// <param name="lexem">Lexem that has to be added.</param>
		/// <returns>Result lexem.</returns>
		public ILexem Unite( RenderedLexem lexem )
		{
			if( null == lexem ) throw new ArgumentNullException( "lexem" );
			if( !lexem.IsPartOfCharWrap ) throw new ArgumentException( "lexem" );

			this.Text += lexem.Text;
			m_width += lexem.Width;
			return this;
		}
		#endregion
	}
}