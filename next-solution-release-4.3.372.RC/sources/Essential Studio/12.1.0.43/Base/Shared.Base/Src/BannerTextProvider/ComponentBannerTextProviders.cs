#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using Syncfusion.Runtime.InteropServices;
using Syncfusion.Windows.Forms.Tools;


namespace Syncfusion.Windows.Forms.ComponentBannerTextProviders
{
	/// <summary>
	/// Base implementation for <see cref="IExtendableTexBox"/> interface.
	/// </summary>
	public abstract class ExtendableTextBoxBase:
		IExtendableTexBox
	{
		#region Data

		/// <summary>
		/// HandleCreated event delegate.
		/// </summary>
		private EventHandler HandleCreated;

		/// <summary>
		/// TextBoxTextChanged event delegate.
		/// </summary>
		private ValueChangedEventHandler TextBoxTextChanged;
		private EventHandler TextBoxMouseDown;
		private EventHandler TextBoxMouseDblClick;
		#endregion

		#region Implementation

		protected virtual void OnHandleCreated( object sender, EventArgs e )
		{
			if( this.HandleCreated != null )
			{
				this.HandleCreated( sender, e );
			}
		}

		protected virtual void OnMouseDown(object sender, EventArgs e)
		{
			if (this.TextBoxMouseDown != null)
			{
				this.TextBoxMouseDown(sender, e);
			}
		}
		protected virtual void OnMouseDblClick(object sender, EventArgs e)
		{
			if (this.TextBoxMouseDblClick != null)
			{
				this.TextBoxMouseDblClick(sender, e);
			}
		}
		protected virtual void OnTextBoxTextChanged( object sender, ValueChangedEventArgs e )
		{
			if( this.TextBoxTextChanged != null )
			{
				this.TextBoxTextChanged( sender, e );
			}
		}

		#endregion

		#region IExtendableTexBox implementation

		public abstract IntPtr Handle { get; }

		event EventHandler IExtendableTexBox.HandleCreated
		{
			add
			{
				this.HandleCreated = (EventHandler)Delegate.Combine( this.HandleCreated, value );				
			}
			remove
			{
				Delegate.Remove( this.HandleCreated, value );
			}
		}
		event EventHandler IExtendableTexBox.TextBoxMouseDown
		{
			add
			{
				this.TextBoxMouseDown = (EventHandler)Delegate.Combine(this.TextBoxMouseDown, value);
			}
			remove
			{
				Delegate.Remove(this.TextBoxMouseDown, value);
			}
		}
		event EventHandler IExtendableTexBox.TextBoxMouseDblClick
		{
			add
			{
				this.TextBoxMouseDblClick = (EventHandler)Delegate.Combine(this.TextBoxMouseDblClick, value);
			}
			remove
			{
				Delegate.Remove(this.TextBoxMouseDblClick, value);
			}
		}
		event ValueChangedEventHandler IExtendableTexBox.TextBoxTextChanged
		{
			add
			{
				this.TextBoxTextChanged = (ValueChangedEventHandler)Delegate.Combine( this.TextBoxTextChanged, value );
			}
			remove
			{
				Delegate.Remove( this.TextBoxTextChanged, value );
			}
		}

		public abstract bool Focused { get; }

		public abstract Color BackColor { get; }

		public abstract Rectangle ClientRectangle { get; }

		public abstract Font Font { get; }

		public abstract RightToLeft RightToLeft { get; }

		public abstract void Invalidate();

		#endregion

		#region IDisposable implementation

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources
		/// </summary>
		/// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
		protected virtual void Dispose( bool disposing )
		{
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Dispose( true );
		}

		/// <summary>
		/// Releases unmanaged resources and performs other cleanup operations before the
		/// <see cref="ExtendableTextBoxBase"/> is reclaimed by garbage collection.
		/// </summary>
		~ExtendableTextBoxBase()
		{
			Dispose( false );
			GC.SuppressFinalize( this );
		}

		#endregion
	}

	/// <summary>
	/// Extendable text box wrapper for <see cref="TextBoxBase"/>.
	/// </summary>
	public class ExtendableTextBox:
		ExtendableTextBoxBase
	{
		#region Fields

		private TextBoxBase m_textBox;

		#endregion

		#region Construction

		/// <summary>
		/// Initializes a new instance of the <see cref="ExtendableTextBox"/> class.
		/// </summary>
		/// <param name="textBox">The text box.</param>
		public ExtendableTextBox( TextBoxBase textBox )
		{
			Debug.Assert( textBox != null );

			m_textBox = textBox;

			if( m_textBox.IsHandleCreated )
			{
				TextBoxHandleCreated( this, EventArgs.Empty );
			}
			if (m_textBox != null)
			{
				m_textBox.MouseDown += new MouseEventHandler(m_textBox_MouseDown);
				m_textBox.MouseDoubleClick += new MouseEventHandler(m_textBox_MouseDoubleClick);
				m_textBox.TextChanged += new EventHandler(TextBoxTextChanged);
				m_textBox.HandleCreated += new EventHandler(TextBoxHandleCreated);
			}
		}
		void m_textBox_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			OnMouseDblClick(this, e);
		}

		void m_textBox_MouseDown(object sender, MouseEventArgs e)
		{
			OnMouseDown(this, e);
		}

		#endregion

		#region Implementation

		private void TextBoxHandleCreated( object sender, EventArgs e )
		{
			OnHandleCreated( this, e );
			TextBoxTextChanged( this, e );
		}

		private void TextBoxTextChanged( object sender, EventArgs e )
		{
			OnTextBoxTextChanged( this, new ValueChangedEventArgs( null, m_textBox.Text ) );
		}

		#endregion

		#region IExtendableTexBox implementation

		/// <summary>
		/// Gets the handle.
		/// </summary>
		/// <value>The handle.</value>
		public override IntPtr Handle
		{
			get
			{
				return (m_textBox.IsHandleCreated ? m_textBox.Handle : IntPtr.Zero);
			}
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="ExtendableTextBox"/> is focused.
		/// </summary>
		/// <value><c>true</c> if focused; otherwise, <c>false</c>.</value>
		public override bool Focused
		{
			get
			{
				return m_textBox.Focused;
			}
		}

		/// <summary>
		/// Gets the color of the back.
		/// </summary>
		/// <value>The color of the back.</value>
		public override Color BackColor
		{
			get
			{
				return m_textBox.BackColor;
			}
		}

		/// <summary>
		/// Gets the client rectangle.
		/// </summary>
		/// <value>The client rectangle.</value>
		public override Rectangle ClientRectangle
		{
			get
			{
				return m_textBox.ClientRectangle;
			}
		}

		/// <summary>
		/// Sets the cursor position to initial state
		/// </summary>
		/// <value>The font.</value>
		public void ResetCursorPosition()
		{
			 m_textBox.SelectionStart = 0;
		}

		/// <summary>
		/// Gets the font.
		/// </summary>
		/// <value>The font.</value>
		public override Font Font
		{
			get
			{
				return m_textBox.Font;
			}
		}

		/// <summary>
		/// Gets the right to left.
		/// </summary>
		/// <value>The right to left.</value>
		public override RightToLeft RightToLeft
		{
			get
			{
				return m_textBox.RightToLeft;
			}
		}

		/// <summary>
		/// Invalidates this instance.
		/// </summary>
		public override void Invalidate()
		{
			m_textBox.Invalidate();
		}

		#endregion

		#region Overrides

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources
		/// </summary>
		/// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				m_textBox.TextChanged -= new EventHandler( TextBoxTextChanged );
				m_textBox.HandleCreated -= new EventHandler( TextBoxHandleCreated );
			}

			base.Dispose( disposing );
		}

		#endregion
	}

	/// <summary>
	/// Banner text provider for TextBox-derived classes.
	/// </summary>
	public class TextBoxBannerTextProvider:
		ComponentBannerTextProviderBase
	{
		public TextBoxBannerTextProvider()
		{
			m_type = typeof( TextBoxBase );
		}

		#region IComponentBannerTextProvider implementation

		public override bool CanExtend( Component extendee )
		{
			bool bCanExtend = false;
			TextBoxBase textBox = extendee as TextBox;

			if( textBox != null && base.CanExtend( extendee ) )
			{
				bCanExtend = !textBox.Multiline;
			}

			return bCanExtend;
		}

		public override IExtendableTexBox GetExtendableTexBox( Component extendee, BannerTextProvider provider )
		{
			return new ExtendableTextBox( (TextBox)extendee );
		}

		#endregion
	}

	/// <summary>
	/// Banner text provider for ComboDropDown-derived classes.
	/// </summary>
	public class ComboDropDownBannerTextProvider:
		ComponentBannerTextProviderBase
	{
		public ComboDropDownBannerTextProvider()
		{
			m_type = typeof( ComboDropDown );
		}

		#region IComponentBannerTextProvider Members

		public override bool CanExtend( Component extendee )
		{
			bool bCanExtend = false;
			ComboDropDown combo = extendee as ComboDropDown;

			if( combo != null && base.CanExtend( extendee ) )
			{
				// Text portion of the constorl is editable with Simple and DropDown style set.
				bCanExtend = (combo.DropDownStyle != ComboBoxStyle.DropDownList);
			}

			return bCanExtend;
		}

		public override IExtendableTexBox GetExtendableTexBox( Component extendee, BannerTextProvider provider )
		{
			IExtendableTexBox etb = null;
			ComboDropDown combo = extendee as ComboDropDown;

			if( combo != null )
			{
				etb = new ExtendableTextBox( combo.TextBox );
			}

			return etb;
		}

		#endregion
	}

	/// <summary>
	/// Extendable text box wrapper for <see cref="ComboBox"/>.
	/// </summary>
	public class ExtendableComboBoxTextBox:
		ExtendableTextBoxBase
	{
		#region Data

		private ComboBox m_combo;

		#endregion

		#region Construction

		public ExtendableComboBoxTextBox( ComboBox combo )
		{
			Debug.Assert( combo != null );

			m_combo = combo;

			if( m_combo.IsHandleCreated )
			{
				ComboBoxHandleCreated( this, EventArgs.Empty );
			}

			m_combo.TextChanged += new EventHandler( ComboBoxTextChanged );			
			m_combo.HandleCreated += new EventHandler( ComboBoxHandleCreated );			
			m_combo.MouseDown += new MouseEventHandler(m_combo_MouseDown);
			m_combo.MouseDoubleClick += new MouseEventHandler(m_combo_MouseDoubleClick);
		}

		void m_combo_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			OnMouseDblClick(this, e);
		}

		void m_combo_MouseDown(object sender, MouseEventArgs e)
		{
			OnMouseDown(this, e);
		}

		#endregion
        public void ResetCursorPosition()
        {
            m_combo.SelectionStart = 0;
        }
		#region Event handlers

		void ComboBoxHandleCreated( object sender, EventArgs e )
		{
			OnHandleCreated( this, e );
            if (sender is ExtendableComboBoxTextBox)
			ComboBoxTextChanged( this, e );
		}

		private void ComboBoxTextChanged( object sender, EventArgs e )
		{
			OnTextBoxTextChanged( this, new ValueChangedEventArgs( null, m_combo.Text ) );
		}

		#endregion

		#region Implementation

		private bool GetComboBoxInfo( ref NativeMethods.COMBOBOXINFO cbInfo )
		{
			return m_combo.IsHandleCreated && NativeMethods.GetComboBoxInfo( m_combo.Handle, ref cbInfo );
		}

		#endregion

		#region IExtendableTexBox implementation

		public override IntPtr Handle
		{
			get
			{
				IntPtr handle = IntPtr.Zero;
				NativeMethods.COMBOBOXINFO cbInfo = new NativeMethods.COMBOBOXINFO();
				cbInfo.cbSize = Marshal.SizeOf( cbInfo );

				if( GetComboBoxInfo( ref cbInfo ) )
				{
					handle = cbInfo.hwndEdit;
				}

				return handle;
			}
		}

		public override bool Focused
		{
			get
			{
				bool bFocused = false;
				IntPtr handle = this.Handle;

				if( handle != IntPtr.Zero )
				{
					bFocused = (NativeMethods.GetFocus() == handle);
				}

				return bFocused;
			}
		}

		public override Color BackColor
		{
			get
			{
				return m_combo.BackColor;
			}
		}

		public override Rectangle ClientRectangle
		{
			get
			{
				Rectangle clienRect = Rectangle.Empty;
				IntPtr handle = this.Handle;

				if( handle != IntPtr.Zero )
				{
					NativeMethods.RECT rcClient = new NativeMethods.RECT();

					if( NativeMethods.GetClientRect( handle, ref rcClient ) )
					{
						clienRect = Rectangle.FromLTRB( rcClient.left, rcClient.top, rcClient.right, rcClient.bottom );
					}
				}

				return clienRect;
			}
		}

		public override Font Font
		{
			get
			{
				return m_combo.Font;
			}
		}

		public override RightToLeft RightToLeft
		{
			get
			{
				return m_combo.RightToLeft;
			}
		}

		public override void Invalidate()
		{
			IntPtr handle = this.Handle;

			if( handle != IntPtr.Zero )
			{
				NativeMethods.RECT rcClient = new NativeMethods.RECT();

				if( NativeMethods.GetClientRect( handle, ref rcClient ) )
				{
					NativeMethods.InvalidateRect( handle, ref rcClient, false );
				}
			}
		}

		#endregion
	}

	/// <summary>
	/// Banner text provider for ComboBox-derived classes.
	/// </summary>
	public class ComboBoxBannerTextProvider:
		ComponentBannerTextProviderBase
	{
		public ComboBoxBannerTextProvider()
		{
			m_type = typeof( ComboBox );
		}

		#region IComponentBannerTextProvider implementation

		public override bool CanExtend( Component extendee )
		{
			bool bCanExtend = false;
			ComboBox combo = extendee as ComboBox;

			if( combo != null && base.CanExtend( extendee ) )
			{
				// Text portion of the constorl is editable with Simple and DropDown style set.
				bCanExtend = (combo.DropDownStyle != ComboBoxStyle.DropDownList);
			}

			return bCanExtend;
		}

		public override IExtendableTexBox GetExtendableTexBox( Component extendee, BannerTextProvider provider )
		{
			IExtendableTexBox etb = null;
			ComboBox combo = extendee as ComboBox;

			if( combo != null )
			{
				etb = new ExtendableComboBoxTextBox( combo );
			}

			return etb;
		}

		#endregion
	}
}
