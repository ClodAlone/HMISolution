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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Security.Permissions;

using System.Text;
using System.Text.RegularExpressions;
using Syncfusion.Windows.Forms.Edit.Utils;

namespace Syncfusion.Windows.Forms.Edit.Dialogs
{
	/// <summary>
	/// Text box for key shortcuts.
	/// </summary>
	[ToolboxItem( false )]
	public class ShortcutTextBox
		: TextBox
	{
		#region Classes
		/// <summary>
		/// Structure that describes key state.
		/// </summary>
		public struct KeyState
		{
			/// <summary>
			/// Pressed Key.
			/// </summary>
			public Keys Key;
			/// <summary>
			/// Windows key state.
			/// </summary>
			public bool Win;
			/// <summary>
			/// Alt key state.
			/// </summary>
			public bool Alt;
			/// <summary>
			/// Ctrl key state.
			/// </summary>
			public bool Ctrl;
			/// <summary>
			/// Shift key state.
			/// </summary>
			public bool Shift;
			/// <summary>
			/// Gets key combination.
			/// </summary>
			/// <remarks>
			/// Win key is not modifier so it can not be set as a modifier in resulting combination.
			/// </remarks>
			public Keys KeyCombination
			{
				get
				{
					Keys result = Key;
					if( Alt ) result |= Keys.Alt;
					if( Ctrl ) result |= Keys.Control;
					if( Shift ) result |= Keys.Shift;

					return result;
				}
			}
		}
		#endregion

		#region Constants
		/// <summary>
		/// Message index for keyboard events.
		/// </summary>
		private const int WM_KEYBOARD_MESSAGE = ( int )Msg.WM_USER + 131;
		/// <summary>
		/// Numbers regular expression.
		/// </summary>
		private const string DEF_NUMBERS = "D[0-9]";
		/// <summary>
		/// Key modifiers regular expression.
		/// </summary>
		private const string DEF_MODIFIER = "(Control)|(Alt)|(Shift)|(Menu)";
		/// <summary>
		/// RegEx default options.
		/// </summary>
		private const RegexOptions DEF_REGEX = RegexOptions.IgnoreCase | Syncfusion.Windows.Forms.Edit.Implementation.Config.Config.DEF_COMPILED_REGEX;
		#endregion

		#region Static Members
		/// <summary>
		/// 
		/// </summary>
		static private Hashtable m_keys = new Hashtable();
		/// <summary>
		/// 
		/// </summary>
		static private Regex _numbers = new Regex( DEF_NUMBERS, DEF_REGEX );
		/// <summary>
		/// 
		/// </summary>
		static private Regex _modifier = new Regex( DEF_MODIFIER, DEF_REGEX );
		#endregion

		#region Fields
		private IList m_enteredKeys = new ArrayList();
		private KeysConverter convert = new KeysConverter();
		#endregion

		#region Form controls
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		#endregion

		#region Initialize/Finalize methods
		/// <summary>
		/// 
		/// </summary>
		static ShortcutTextBox()
		{
			Keys[] keyValues = ( Keys[] )Enum.GetValues( typeof( Keys ) );
			string[] keyNames = Enum.GetNames( typeof( Keys ) );

			for( int i = 0, len = keyValues.Length; i < len; i++ )
			{
				m_keys[ keyNames[ i ] ] = keyValues[ i ];
			}
		}
		/// <summary>
		/// Creates new instance of ShortcutTextBox.
		/// </summary>
		public ShortcutTextBox()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

		}
		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">True to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if( components != null )
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#endregion

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
		}
		#endregion

		#region Overrides
		/// <summary>
		/// Processes dialog key.
		/// </summary>
		/// <param name="keyData">Keys to process.</param>
		/// <returns>True if the key was processed by the control; otherwise, false.</returns>
		protected override bool ProcessDialogKey( Keys keyData )
		{
#if VERBOSE && DEBUG
	Debug.WriteLine( keyData.ToString(), "ProcessDialogKey" );
#endif

			// if user press Tab key run base
			if( ( keyData & Keys.KeyCode ) == Keys.Tab )
			{
				return base.ProcessDialogKey( keyData );
			}

			// If there is no modifiers
			if( ( keyData & Keys.KeyCode ) == keyData )
				ResetSequence();

			if( m_enteredKeys.Count >= 2 )
				ResetSequence();

			bool disallowed = false;
			Keys checkKey = keyData & Keys.KeyCode;
			disallowed |= ( checkKey == Keys.ControlKey );
			disallowed |= ( checkKey == Keys.ShiftKey );
			disallowed |= ( checkKey == Keys.Menu );
			disallowed |= ( checkKey == Keys.Capital );
			disallowed |= ( checkKey == Keys.Scroll );
			disallowed |= ( checkKey == Keys.NumLock );

			KeyState state = new KeyState();
			state.Key = keyData & Keys.KeyCode;
			bool bLWin = WinAPI.GetAsyncKeyState( Keys.LWin ) > 0;
			bool bRWin = WinAPI.GetAsyncKeyState( Keys.RWin ) > 0;
			bool bAlt = WinAPI.GetAsyncKeyState( Keys.Menu ) > 0;
			bool bCtrl = WinAPI.GetAsyncKeyState( Keys.ControlKey ) > 0;
			bool bShift = WinAPI.GetAsyncKeyState( Keys.ShiftKey ) > 0;
			state.Alt = bAlt;
			state.Ctrl = bCtrl;
			state.Shift = bShift;
			state.Win = ( ( bLWin || bRWin ) && !( checkKey == Keys.LWin || checkKey == Keys.RWin ) );

			if( !disallowed )
			{
				if( ( keyData == Keys.Delete || keyData == Keys.Back ) && m_enteredKeys.Count > 0 )
					ResetSequence();
				else
					m_enteredKeys.Add( state );
			}
#if VERBOSE && DEBUG
			else
						Debug.WriteLine( "Unprocessed", keyData.ToString() );
#endif

			UpdateText();

			return true;
		}
		/// <summary>
		/// Processes message from the keyboard hook.
		/// </summary>
		/// <param name="m">The Windows Message to process.</param>
		protected override void WndProc( ref Message m )
		{
			if( m.Msg == WM_KEYBOARD_MESSAGE )
			{
				Keys key = ( Keys )m.WParam.ToInt32();
				bool bProcess = ProcessDialogKey( key );

				m.Result = bProcess ? ( IntPtr )1 : IntPtr.Zero;
			}
			else
				base.WndProc( ref m );
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets entered sequence.
		/// </summary>
		public KeyState[] EnteredKeys
		{
			get
			{
				KeyState[] keys = new KeyState[ m_enteredKeys.Count ];
				m_enteredKeys.CopyTo( keys, 0 );
				return keys;
			}
		}
		#endregion

		#region Utility Methods
		/// <summary>
		/// Resets key sequence.
		/// </summary>
		protected void ResetSequence()
		{
			m_enteredKeys.Clear();
		}
		/// <summary>
		/// Updates text according to the currently entered sequence.
		/// </summary>
		protected void UpdateText()
		{
			KeyState[] keys = new KeyState[ m_enteredKeys.Count ];
			m_enteredKeys.CopyTo( keys, 0 );
			string result = string.Empty;

			for( int i = 0; i < keys.Length; i++ )
			{
				KeyState state = keys[ i ];
				if( state.Win )
					result += "Win + ";
				if( state.Ctrl )
					result += "Ctrl + ";
				if( state.Alt )
					result += "Alt + ";
				if( state.Shift )
					result += "Shift + ";

				result += convert.ConvertToString( state.Key );

				if( i + 1 < keys.Length )
					result += ", ";
			}

			this.Text = result;
		}
		/// <summary>
		/// Gets friendly named keys.
		/// </summary>
		/// <param name="keys">Keys.</param>
		/// <returns>String with friendly named keys.</returns>
		protected string FriendlyNamedKeys( Keys keys )
		{
			return convert.ConvertToString( keys );
		}
		/// <summary>
		/// Checks whether string is digit.
		/// </summary>
		/// <param name="value">String to check.</param>
		/// <returns>True if string is digit; otherwise false.</returns>
		protected bool IsDigit( string value )
		{
			return _numbers.Match( value ).Success;
		}
		/// <summary>
		/// Checks whether string is modifier.
		/// </summary>
		/// <param name="value">String to check.</param>
		/// <returns>True if string is modifier; otherwise false.</returns>
		protected bool IsModifierKey( string value )
		{
			return _modifier.Match( value ).Success;
		}
		#endregion
	}
}