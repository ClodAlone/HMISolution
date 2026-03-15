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
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
#endregion

namespace Syncfusion.Shared.Utils.KeyBinding.Implementation
{
	/// <summary>
	/// Summary description for KeysBindingAttribute.
	/// </summary>
	[AttributeUsage( AttributeTargets.Method, AllowMultiple = true )]
	public class KeysBindingAttribute : Attribute
	{
		#region Class members
		/// <summary>
		/// Keys, assigned to the current instance.
		/// </summary>
		private Keys[] m_keys;
		#endregion

		#region Class properties
		/// <summary>
		/// GET keys, assigned to the current instance.
		/// </summary>
		public Keys[] Keys
		{
			get
			{
				return m_keys;
			}
		}
		#endregion

		#region Class Initialize/Finalize methods
		/// <summary>
		/// Hides default constructor.
		/// </summary>
		private KeysBindingAttribute()
			: base()
		{
		}
		/// <summary>
		/// Initalizes attribute instance with a key sequence.
		/// </summary>
		/// <param name="keys">Key sequence.</param>
		private void InitBindings( Keys[] keys )
		{
			if( keys == null )
				throw new ArgumentNullException( "keys" );

			if( keys.Length == 0 )
				throw new ArgumentException( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_96, "keys" );

			m_keys = keys;

		}
		/// <summary>
		/// Creates and initalizes attribute with a key sequence.
		/// </summary>
		/// <param name="keys">Key sequence.</param>
		public KeysBindingAttribute( params Keys[] keys )
			: this()
		{
			InitBindings( keys );
		}
		/// <summary>
		/// Creates and initalizes attribute with a key sequence.
		/// </summary>
		/// <param name="key">Key sequence.</param>
		public KeysBindingAttribute( Keys key )
		{
			Keys[] keys = new Keys[] { key };
			InitBindings( keys );
		}
		/// <summary>
		/// Creates and initalizes attribute with a key sequence.
		/// </summary>
		/// <param name="key1">First key in the sequence.</param>
		/// <param name="key2">Second key in the sequence.</param>
		public KeysBindingAttribute( Keys key1, Keys key2 )
		{
			Keys[] keys = new Keys[] { key1, key2 };
			InitBindings( keys );
		}
		#endregion
	}
}