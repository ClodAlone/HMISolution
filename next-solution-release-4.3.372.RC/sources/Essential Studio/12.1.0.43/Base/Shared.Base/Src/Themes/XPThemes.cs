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
using System.Drawing;
using System.Globalization;
using System.Security.Permissions;
using System.Windows.Forms;

using Syncfusion.Runtime.InteropServices;
#endregion

namespace Syncfusion.Windows.Forms
{
	/// <summary>
	/// Provides some information regarding themes support and state in the OS.
	/// </summary>
	public class XPThemes
	{
		#region Class static members
		/// <summary></summary>
		private static bool isThemedOS = false;
		/// <summary></summary>
		private static bool isAppThemed = false;
		/// <summary></summary>
		private static bool isThemeActive = false;
		/// <summary></summary>
		private static ThemeChangeListenerForm dummyForm = null;
		/// <summary></summary>
		private static Hashtable htThemedControlDrawings = new Hashtable();
		#endregion

		#region Class init
		/// <summary></summary>
		static XPThemes()
		{
			XPThemes.isThemedOS = XPThemes.GetIsThemedOS();

			if( XPThemes.IsThemedOS )
			{
				try
				{
					SecurityPermission perm = new SecurityPermission( SecurityPermissionFlag.UnmanagedCode );
					perm.Assert();

					XPThemes.isAppThemed = NativeMethods.IsAppThemed();

					// Else query uxtheme.dll.
					XPThemes.isThemeActive = NativeMethods.IsThemeActive();

					Application.ApplicationExit += new EventHandler( XPThemes.ApplicationExit );

					dummyForm = new ThemeChangeListenerForm();
					dummyForm.ThemeChanged += new EventHandler( OnThemeChanged );

					SecurityPermission.RevertAssert();
				}
				catch
				{
					XPThemes.isThemedOS = false;
				}
			}
		}
		#endregion

		#region PROPERTIES
		/// <summary>
		/// Returns the theme file name with the path.
		/// </summary>
		public static string ThemeFileName
		{
			get
			{
				if( !XPThemes.IsThemedOS )
				{
					return String.Empty;
				}

				string themeFileName = String.Empty, colorSchemeName = String.Empty, sizeName = String.Empty;
				themeFileName = themeFileName.PadRight( 257, ' ' );
				colorSchemeName = colorSchemeName.PadRight( 257, ' ' );
				sizeName = sizeName.PadRight( 257, ' ' );
				NativeMethods.GetCurrentThemeName( themeFileName, themeFileName.Length,
												   colorSchemeName, colorSchemeName.Length, sizeName, sizeName.Length );

				return themeFileName;
			}
		}
		/// <summary>
		/// Returns the current theme color scheme name.
		/// </summary>
		public static string CurrentThemeColorScheme
		{
			get
			{
				if( !XPThemes.IsThemedOS )
				{
					return String.Empty;
				}

				string themeFileName = String.Empty, colorSchemeName = String.Empty, sizeName = String.Empty;
				themeFileName = themeFileName.PadRight( 257, ' ' );
				colorSchemeName = colorSchemeName.PadRight( 257, ' ' );
				sizeName = sizeName.PadRight( 257, ' ' );
				NativeMethods.GetCurrentThemeName( themeFileName, themeFileName.Length,
												   colorSchemeName, colorSchemeName.Length, sizeName, sizeName.Length );

				return colorSchemeName;
			}
		}
		/// <summary>
		/// Indicates whether the default blue theme is on.
		/// </summary>
		public static bool IsDefaultBlueThemeOn
		{
			get
			{
				if( !XPThemes.IsThemedOS || !XPThemes.IsThemeActive || !XPThemes.IsAppThemed )
				{
					return false;
				}

				string themeFileName = ThemeFileName.ToLower( CultureInfo.CurrentCulture );

				if( themeFileName.IndexOf( "luna.msstyles" ) != -1 )
				{
					string colorSchemeName = CurrentThemeColorScheme.ToLower( CultureInfo.CurrentCulture );
					colorSchemeName = colorSchemeName.Trim();
					if( colorSchemeName.IndexOf( "normalcolor" ) == 0 )
					{
						return true;
					}
				}
				return false;
			}
		}
		/// <summary>
		/// Indicates whether the Olive Green theme is on.
		/// </summary>
		public static bool IsOliveGreenThemeOn
		{
			get
			{
				if( !XPThemes.IsThemedOS || !XPThemes.IsThemeActive || !XPThemes.IsAppThemed )
				{
					return false;
				}

				string themeFileName = ThemeFileName.ToLower( CultureInfo.CurrentCulture );

				if( themeFileName.IndexOf( "luna.msstyles" ) != -1 )
				{
					string colorSchemeName = CurrentThemeColorScheme.ToLower( CultureInfo.CurrentCulture );
					colorSchemeName = colorSchemeName.Trim();
					if( colorSchemeName.IndexOf( "homestead" ) == 0 )
					{
						return true;
					}
				}
				return false;
			}
		}
		/// <summary>
		/// Indicates whether the Silver theme is on.
		/// </summary>
		public static bool IsSilverThemeOn
		{
			get
			{
				if( !XPThemes.IsThemedOS || !XPThemes.IsThemeActive || !XPThemes.IsAppThemed )
				{
					return false;
				}

				string themeFileName = ThemeFileName.ToLower( CultureInfo.CurrentCulture );

				if( themeFileName.IndexOf( "luna.msstyles" ) != -1 )
				{
					string colorSchemeName = CurrentThemeColorScheme.ToLower( CultureInfo.CurrentCulture );
					colorSchemeName = colorSchemeName.Trim();
					if( colorSchemeName.IndexOf( "metallic" ) == 0 )
					{
						return true;
					}
				}
				return false;
			}
		}
		/// <summary>
		/// Indicates whether this OS has themes support built-in.
		/// </summary>
		/// <value>True if themes are supported; False otherwise.</value>
		/// <remarks><para>This property returns True for Windows XP and later versions.</para></remarks>
		public static bool IsThemedOS
		{
			get
			{
				return isThemedOS;
			}
		}

		/// <summary>
		/// Indicates whether themes are enabled in the current OS.
		/// </summary>
		public static bool IsThemeActive
		{
			get
			{
				return XPThemes.isThemeActive;
			}
		}

		/// <summary>
		/// Indicates whether the current application is themed.
		/// </summary>
		public static bool IsAppThemed
		{
			get
			{
				return isAppThemed;
			}
		}
		#endregion PROPERTIES

		#region Class utility methods
		/// <summary></summary>
		/// <returns></returns>
		private static bool GetIsThemedOS()
		{
            return OSFeature.Feature.IsPresent(OSFeature.Themes);
		}
		#endregion

		#region EVENTS
		/// <summary></summary>
		public static event EventHandler ThemeChanged;
		#endregion EVENTS

		#region THEMEDDRAWINGHASH
		/// <summary></summary>
		/// <param name="tcd"/>
		public static void RegisterControlDrawing( ThemedControlDrawing tcd )
		{
			htThemedControlDrawings[tcd] = 1;
		}

		/// <summary></summary>
		/// <param name="tcd"/>
		public static void UnregisterControlDrawing( ThemedControlDrawing tcd )
		{
			if( htThemedControlDrawings.Contains( tcd ) )
			{
				htThemedControlDrawings.Remove( tcd );
                tcd.ResetThemeHandle();
                if (htThemedControlDrawings.Count == 0 && dummyForm != null)
                {
                    dummyForm.Dispose();
                    dummyForm = null;
                }
			}
            htThemedControlDrawings.Clear();
		}

		/// <summary></summary>
		private static void ResetThemeHandles()
		{
			foreach( ThemedControlDrawing tcd in htThemedControlDrawings.Keys )
			{
				tcd.ResetThemeHandle();
			}
		}

		/// <summary></summary>
		private static void RefreshThemeHandles()
		{
			foreach( ThemedControlDrawing tcd in htThemedControlDrawings.Keys )
			{
				tcd.RefreshThemeHandle();
			}
		}
		#endregion THEMEDDRAWINGHASH

		#region EVENTLISTENERS
		/// <summary></summary>
		/// <param name="sender"/>
		/// <param name="e"/>
		private static void OnThemeChanged( object sender, EventArgs e )
		{
			XPThemes.isThemeActive = NativeMethods.IsThemeActive();
			XPThemes.isAppThemed = NativeMethods.IsAppThemed();

			XPThemes.RefreshThemeHandles();

			if( XPThemes.ThemeChanged != null )
			{
				XPThemes.ThemeChanged( null, e );
			}
		}

		/// <summary></summary>
		/// <param name="sender"/>
		/// <param name="e"/>
		private static void ApplicationExit( object sender, EventArgs e )
		{
			Application.ApplicationExit -= new EventHandler( ApplicationExit );

			XPThemes.ResetThemeHandles();
			UnWireDummyForm();
		}

		/// <summary></summary>
		private static void UnWireDummyForm()
		{
			if( XPThemes.dummyForm == null )
			{
				return;
			}

			if( XPThemes.dummyForm.InvokeRequired )
			{
				// Change for 4.1.0.62: We originally called
				//XPThemes.dummyForm.Invoke(new MethodInvoker(UnWireDummyFormInternal));
				// here.
				// But the Invoke could possibly results in a deadlock if the main UI thread is exiting and gets blocked waiting for any worker 
				// threads to terminate. In such situations ApplicationExit is most likely called twice and therefore just skipping the InvokeRequired case should do.
				// However, if a leak is detected in this case I would suggest
				// calling BeginInvoke instead. 
			}
			else
			{
				UnWireDummyFormInternal();
			}
		}

		/// <summary></summary>
		private static void UnWireDummyFormInternal()
		{
			if( XPThemes.dummyForm != null )
			{
				XPThemes.dummyForm.ThemeChanged -= new EventHandler( OnThemeChanged );
				XPThemes.dummyForm.Close();
                XPThemes.dummyForm.Dispose();
                XPThemes.dummyForm = null;
			}
		}
		#endregion

		/// <summary></summary>
		private class ThemeChangeListenerForm: Form
		{
			/// <summary></summary>
			public event EventHandler ThemeChanged;

			/// <summary></summary>
			public ThemeChangeListenerForm()
			{
				this.ControlBox = false;
				this.MinimizeBox = false;
				this.MaximizeBox = false;

				this.MinimumSize = new Size( 0, 0 );
				this.SetStyle( ControlStyles.EnableNotifyMessage | ControlStyles.CacheText, true );
				this.SetStyle( ControlStyles.ContainerControl | ControlStyles.Selectable | ControlStyles.StandardClick, false );

				this.ShowInTaskbar = false;
				this.FormBorderStyle = FormBorderStyle.None;

				this.Visible = false;
				this.Text = "ThemeChangeListenerForm";

				this.Size = new Size( 0, 0 );

				CreateHandle();
			}

			/// <summary>
			/// 
			/// </summary>
			protected override CreateParams CreateParams
			{
				get
				{
					CreateParams cp = base.CreateParams;

					cp.Style &= ~( NativeMethods.WS_VISIBLE|NativeMethods.WS_CAPTION );
					cp.Width = 0;
					cp.Height = 0;

					return cp;
				}
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name="m"></param>
			protected override void WndProc( ref Message m )
			{
				base.WndProc( ref m );

				if( m.Msg == 0x031A /*WM_THEMECHANGED*/ )
				{
					this.OnThemeChanged( EventArgs.Empty );
				}
			}

			/// <summary></summary>
			/// <param name="e"/>
			protected virtual void OnThemeChanged( EventArgs e )
			{
				if( this.ThemeChanged != null )
				{
					this.ThemeChanged( this, e );
				}
			}
		}
	}

	/// <summary>
	/// Specifies that this object subscribes to <see cref="XPThemes.ThemeChanged"/> event.
	/// </summary>
	public interface ISupportThemeChanged
	{
		void ThemeChanged( object sender, EventArgs e );
	}

	/// <summary>
	/// Class for automatic subscription management of <see cref="XPThemes.ThemeChanged"/> event.
	/// </summary>
	public class XPThemesThemeChangedWeakContainer:
		WeakReference
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="XPThemesThemeChangedWeakContainer"/> class.
		/// </summary>
		/// <param name="target">The target.</param>
		public XPThemesThemeChangedWeakContainer( ISupportThemeChanged target ) :
			base( target )
		{
			XPThemes.ThemeChanged += new EventHandler( this.ThemeChanged );
		}

		/// <summary>
		/// Handles theme change.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the <see cref="XPThemes.ThemeChanged"/> event data.</param>
		public void ThemeChanged( object sender, EventArgs e )
		{
			if( this.IsAlive )
			{
				ISupportThemeChanged target = (ISupportThemeChanged)this.Target;

				target.ThemeChanged( sender, e );
			}
			else
			{
				XPThemes.ThemeChanged -= new EventHandler( this.ThemeChanged );
			}
		}

		/// <summary>
		/// Gets or sets the object (the target) referenced by the current <see cref="T:System.WeakReference"/> object.
		/// </summary>
		/// <value></value>
		/// <returns>null if the object referenced by the current <see cref="T:System.WeakReference"/> object has been garbage collected; otherwise, a reference to the object referenced by the current <see cref="T:System.WeakReference"/> object.
		/// </returns>
		/// <exception cref="T:System.InvalidOperationException">
		/// The reference to the target object is invalid. This exception can be thrown while setting this property if the value is a null reference or if the object has been finalized during the set operation.
		/// </exception>
		public override object Target
		{
			get
			{
				return base.Target;
			}
			set
			{
				base.Target = value;

				if( value == null )
				{
					XPThemes.ThemeChanged -= new EventHandler( this.ThemeChanged );
				}
			}
		}
	}
}