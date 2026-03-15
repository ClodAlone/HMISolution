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
using Syncfusion.Windows.Forms.Edit.Implementation.Parser;

namespace Syncfusion.Windows.Forms.Edit.Utils.AutoFormatting
{
	/// <summary>
	/// Lexem wrapping class used in autoformatting.
	/// </summary>
	public class LexemWrapper
		: ILexemWrapper
	{
		#region Class Members
		/// <summary>
		/// Text of the lexem.
		/// </summary>
		private string m_strText;
		/// <summary>
		/// Configuration of the lexem.
		/// </summary>
		private IConfigLexem m_config;
		/// <summary>
		/// Configuration stack of the lexem.
		/// </summary>
		private ConfigStack m_stack;
		#endregion

		#region Class Initialization
		/// <summary>
		/// Creates and initializes new instance of LexemWrapper.
		/// </summary>
		/// <param name="lexem">Lexem to create wrapper for.</param>
		/// <param name="stack"></param>
		public LexemWrapper( ILexem lexem, ConfigStack stack )
		{
      if( null == lexem )
				throw new ArgumentNullException( "lexem" );
			if( null == stack )
				throw new ArgumentNullException( "stack" );

			m_strText = lexem.Text;
			m_config = lexem.Config;
			m_stack = stack;
		}
		/// <summary>
		/// Creates and initializes new instance of LexemWrapper.
		/// </summary>
		/// <param name="text">Text of the lexem.</param>
		/// <param name="config">Configuration of the lexem.</param>
		/// <param name="stack">Configuration stack of the lexem.</param>
		public LexemWrapper( string text, IConfigLexem config, ConfigStack stack )
		{
			if( null == text )
				throw new ArgumentNullException( "text" );
			if( string.Empty == text )
				throw new ArgumentOutOfRangeException( "text" );
			if( null == config )
				throw new ArgumentNullException( "config" );
			if( null == stack )
				throw new ArgumentNullException( "stack" );

			m_strText = text;
			m_config = config;
			m_stack = (ConfigStack)stack.Clone();
		}
		#endregion

		#region ILexemWrapper Members
		/// <summary>
		/// Gets text of the lexem.
		/// </summary>
		public string Text
		{
			get
			{
				return m_strText;
			}
		}
		/// <summary>
		/// Gets configuration of the lexem.
		/// </summary>
		public IConfigLexem Config
		{
			get
			{
				return m_config;
			}
		}
		/// <summary>
		/// Gets configuration stack of the lexem.
		/// </summary>
		public ConfigStack Stack
		{
			get
			{
				return m_stack;
			}
		}
		#endregion
	}
}