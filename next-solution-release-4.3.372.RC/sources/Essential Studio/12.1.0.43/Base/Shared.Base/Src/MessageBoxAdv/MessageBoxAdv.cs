#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if !(SyncfusionFramework1_0 || SyncfusionFramework1_1)

using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;

using Syncfusion.Runtime.InteropServices;
using Syncfusion.Windows.Forms.Localization;
using System.Drawing;


namespace Syncfusion.Windows.Forms
{
	/// <summary>
	/// Displays a message box with Office2007 style that can contain text, buttons, and symbols that inform and instruct the user.
	/// </summary>
	public static class MessageBoxAdv
	{
		private static Office2007Theme s_theme = Office2007Theme.Managed;

		/// <summary>
		/// Gets or sets the Office2007 theme, which is used to show message boxes.
		/// </summary>
		/// <value>The Office2007 theme.</value>
		public static Office2007Theme Office2007Theme
		{
			get
			{
				return s_theme;
			}
			set
			{
				s_theme = value;
			}
		}

		private static bool applyAeroTheme = true ;
		public static bool ApplyAeroTheme
		{
			get
			{
				return applyAeroTheme;
			}
			set
			{
				if (applyAeroTheme != value)
				{
					applyAeroTheme = value;
				}
			}
		}

		/// <summary>
		/// Displays a message box with specified text.
		/// </summary>
		/// <param name="text">The text to display in the message box.</param>
		/// <returns>
		/// One of the <see cref="System.Windows.Forms.DialogResult"></see> values.
		/// </returns>
		public static DialogResult Show( string text )
		{
			return ShowCore( null, text, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, 0, false );
		}

		/// <summary>
		/// Displays a message box with specified text and caption.
		/// </summary>
		/// <param name="text">The text to display in the message box.</param>
		/// <param name="caption">The text to display in the title bar of the message box.</param>
		/// <returns>
		/// One of the <see cref="System.Windows.Forms.DialogResult"></see> values.
		/// </returns>
		public static DialogResult Show( string text, string caption )
		{
			return ShowCore( null, text, caption, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, 0, false );
		}

		/// <summary>
		/// Displays a message box in front of the specified object and with the specified text.
		/// </summary>
		/// <param name="owner">The owner.</param>
		/// <param name="text">The text.</param>
		/// <returns>
		/// One of the <see cref="System.Windows.Forms.DialogResult"></see> values.
		/// </returns>
		public static DialogResult Show( IWin32Window owner, string text )
		{
			return ShowCore( owner, text, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, 0, false );
		}

		/// <summary>
		/// Displays a message box with specified text, caption, and buttons.
		/// </summary>
		/// <param name="text">The text to display in the message box.</param>
		/// <param name="caption">The text to display in the title bar of the message box.</param>
		/// <param name="buttons">One of the <see cref="System.Windows.Forms.MessageBoxButtons"></see> values that specifies which buttons to display in the message box.</param>
		/// <returns>
		/// One of the <see cref="System.Windows.Forms.DialogResult"></see> values.
		/// </returns>
		public static DialogResult Show( string text, string caption, MessageBoxButtons buttons )
		{
			return ShowCore( null, text, caption, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, 0, false );
		}

		/// <summary>
		/// Displays a message box in front of the specified object and with the specified text and caption.
		/// </summary>
		/// <param name="owner">An implementation of <see cref="System.Windows.Forms.IWin32Window"></see> that will own the modal dialog box.</param>
		/// <param name="text">The text to display in the message box.</param>
		/// <param name="caption">The text to display in the title bar of the message box.</param>
		/// <returns>
		/// One of the <see cref="System.Windows.Forms.DialogResult"></see> values.
		/// </returns>
		public static DialogResult Show( IWin32Window owner, string text, string caption )
		{
			return ShowCore( owner, text, caption, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, 0, false );
		}

		/// <summary>
		/// Displays a message box with specified text, caption, buttons, and icon.
		/// </summary>
		/// <param name="text">The text to display in the message box.</param>
		/// <param name="caption">The text to display in the title bar of the message box.</param>
		/// <param name="buttons">One of the <see cref="System.Windows.Forms.MessageBoxButtons"></see> values that specifies which buttons to display in the message box.</param>
		/// <param name="icon">One of the <see cref="System.Windows.Forms.MessageBoxIcon"></see> values that specifies which icon to display in the message box.</param>
		/// <returns>
		/// One of the <see cref="System.Windows.Forms.DialogResult"></see> values.
		/// </returns>
		public static DialogResult Show( string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon )
		{
			return ShowCore( null, text, caption, buttons, icon, MessageBoxDefaultButton.Button1, 0, false );
		}

		/// <summary>
		/// Displays a message box with specified text, caption, buttons, and icon.
		/// </summary>
		/// <param name="text">The text to display in the message box.</param>
		/// <param name="caption">The text to display in the title bar of the message box.</param>
		/// <param name="buttons">One of the <see cref="System.Windows.Forms.MessageBoxButtons"></see> values that specifies which buttons to display in the message box.</param>
		/// <param name="image">The image used instead of system icon.</param>
		/// <param name="imageSize">Size of the image. If empty, original image size is used.</param>
		/// <returns>
		/// One of the <see cref="System.Windows.Forms.DialogResult"></see> values.
		/// </returns>
		public static DialogResult Show( string text, string caption, MessageBoxButtons buttons, Image image, Size imageSize )
		{
			return ShowCore( null, text, caption, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, 0, false, null, image, imageSize );
		}

		/// <summary>
		/// Displays a message box in front of the specified object and with the specified text, caption, and buttons.
		/// </summary>
		/// <param name="owner">The owner.</param>
		/// <param name="text">The text to display in the message box.</param>
		/// <param name="caption">The caption.</param>
		/// <param name="buttons">One of the <see cref="System.Windows.Forms.MessageBoxButtons"></see> values that specifies which buttons to display in the message box.</param>
		/// <returns>
		/// One of the <see cref="System.Windows.Forms.DialogResult"></see> values.
		/// </returns>
		public static DialogResult Show( IWin32Window owner, string text, string caption, MessageBoxButtons buttons )
		{
			return ShowCore( owner, text, caption, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, 0, false );
		}

		/// <summary>
		/// Displays a message box with the specified text, caption, buttons, icon, and default button.
		/// </summary>
		/// <param name="text">The text to display in the message box.</param>
		/// <param name="caption">The text to display in the title bar of the message box.</param>
		/// <param name="buttons">One of the <see cref="System.Windows.Forms.MessageBoxButtons"></see> values that specifies which buttons to display in the message box.</param>
		/// <param name="icon">One of the <see cref="System.Windows.Forms.MessageBoxIcon"></see> values that specifies which icon to display in the message box.</param>
		/// <param name="defaultButton">One of the <see cref="System.Windows.Forms.MessageBoxDefaultButton"></see> values that specifies the default button for the message box.</param>
		/// <returns>
		/// One of the <see cref="System.Windows.Forms.DialogResult"></see> values.
		/// </returns>
		public static DialogResult Show( string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton )
		{
			return ShowCore( null, text, caption, buttons, icon, defaultButton, 0, false );
		}

		/// <summary>
		/// Displays a message box with the specified text, caption, buttons, icon, and default button.
		/// </summary>
		/// <param name="text">The text to display in the message box.</param>
		/// <param name="caption">The text to display in the title bar of the message box.</param>
		/// <param name="buttons">One of the <see cref="System.Windows.Forms.MessageBoxButtons"></see> values that specifies which buttons to display in the message box.</param>
		/// <param name="image">The image used instead of system icon.</param>
		/// <param name="imageSize">Size of the image. If empty, original image size is used.</param>
		/// <param name="defaultButton">One of the <see cref="System.Windows.Forms.MessageBoxDefaultButton"></see> values that specifies the default button for the message box.</param>
		/// <returns>
		/// One of the <see cref="System.Windows.Forms.DialogResult"></see> values.
		/// </returns>
		public static DialogResult Show( string text, string caption, MessageBoxButtons buttons, Image image, Size imageSize, MessageBoxDefaultButton defaultButton )
		{
			return ShowCore( null, text, caption, buttons, MessageBoxIcon.None, defaultButton, 0, false, null, image, imageSize );
		}

		/// <summary>
		/// Displays a message box in front of the specified object and with the specified text, caption, buttons, and icon.
		/// </summary>
		/// <param name="owner">The owner.</param>
		/// <param name="text">The text.</param>
		/// <param name="caption">The caption.</param>
		/// <param name="buttons">The buttons.</param>
		/// <param name="icon">The icon.</param>
		/// <returns>
		/// One of the <see cref="System.Windows.Forms.DialogResult"></see> values.
		/// </returns>
		public static DialogResult Show( IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon )
		{
			return ShowCore( owner, text, caption, buttons, icon, MessageBoxDefaultButton.Button1, 0, false );
		}

		/// <summary>
		/// Displays a message box in front of the specified object and with the specified text, caption, buttons, and icon.
		/// </summary>
		/// <param name="owner">The owner.</param>
		/// <param name="text">The text.</param>
		/// <param name="caption">The caption.</param>
		/// <param name="buttons">The buttons.</param>
		/// <param name="image">The image used instead of system icon.</param>
		/// <param name="imageSize">Size of the image. If empty, original image size is used.</param>
		/// <returns>
		/// One of the <see cref="System.Windows.Forms.DialogResult"></see> values.
		/// </returns>
		public static DialogResult Show( IWin32Window owner, string text, string caption, MessageBoxButtons buttons, Image image, Size imageSize )
		{
			return ShowCore( owner, text, caption, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, 0, false, null, image, imageSize );
		}

		/// <summary>
		/// Displays a message box with the specified text, caption, buttons, icon, default button, and options.
		/// </summary>
		/// <param name="text">The text to display in the message box.</param>
		/// <param name="caption">The text to display in the title bar of the message box.</param>
		/// <param name="buttons">One of the <see cref="System.Windows.Forms.MessageBoxButtons"></see> values that specifies which buttons to display in the message box.</param>
		/// <param name="icon">One of the <see cref="System.Windows.Forms.MessageBoxIcon"></see> values that specifies which icon to display in the message box.</param>
		/// <param name="defaultButton">One of the <see cref="System.Windows.Forms.MessageBoxDefaultButton"></see> values that specifies the default button for the message box.</param>
		/// <param name="options">One of the <see cref="System.Windows.Forms.MessageBoxOptions"></see> values that specifies which display and association options will be used for the message box. You may pass in 0 if you wish to use the defaults.</param>
		/// <returns>
		/// One of the <see cref="System.Windows.Forms.DialogResult"></see> values.
		/// </returns>
		public static DialogResult Show( string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options )
		{
			return ShowCore( null, text, caption, buttons, icon, defaultButton, options, false );
		}

		/// <summary>
		/// Displays a message box with the specified text, caption, buttons, icon, default button, and options.
		/// </summary>
		/// <param name="text">The text to display in the message box.</param>
		/// <param name="caption">The text to display in the title bar of the message box.</param>
		/// <param name="buttons">One of the <see cref="System.Windows.Forms.MessageBoxButtons"></see> values that specifies which buttons to display in the message box.</param>
		/// <param name="image">The image used instead of system icon.</param>
		/// <param name="imageSize">Size of the image. If empty, original image size is used.</param>
		/// <param name="defaultButton">One of the <see cref="System.Windows.Forms.MessageBoxDefaultButton"></see> values that specifies the default button for the message box.</param>
		/// <param name="options">One of the <see cref="System.Windows.Forms.MessageBoxOptions"></see> values that specifies which display and association options will be used for the message box. You may pass in 0 if you wish to use the defaults.</param>
		/// <returns>
		/// One of the <see cref="System.Windows.Forms.DialogResult"></see> values.
		/// </returns>
		public static DialogResult Show( string text, string caption, MessageBoxButtons buttons, Image image, Size imageSize, MessageBoxDefaultButton defaultButton, MessageBoxOptions options )
		{
			return ShowCore( null, text, caption, buttons, MessageBoxIcon.None, defaultButton, options, false, null, image, imageSize );
		}

		/// <summary>
		/// Displays a message box in front of the specified object and with the specified text, caption, buttons, icon, and default button.
		/// </summary>
		/// <param name="owner">The owner.</param>
		/// <param name="text">The text to display in the message box.</param>
		/// <param name="caption">The caption.</param>
		/// <param name="buttons">One of the <see cref="System.Windows.Forms.MessageBoxButtons"></see> values that specifies which buttons to display in the message box.</param>
		/// <param name="icon">The icon.</param>
		/// <param name="defaultButton">One of the <see cref="System.Windows.Forms.MessageBoxDefaultButton"></see> values that specifies the default button for the message box.</param>
		/// <returns>
		/// One of the <see cref="System.Windows.Forms.DialogResult"></see> values.
		/// </returns>
		public static DialogResult Show( IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton )
		{
			return ShowCore( owner, text, caption, buttons, icon, defaultButton, 0, false );
		}

		/// <summary>
		/// Displays a message box in front of the specified object and with the specified text, caption, buttons, icon, and default button.
		/// </summary>
		/// <param name="owner">The owner.</param>
		/// <param name="text">The text to display in the message box.</param>
		/// <param name="caption">The caption.</param>
		/// <param name="buttons">One of the <see cref="System.Windows.Forms.MessageBoxButtons"></see> values that specifies which buttons to display in the message box.</param>
		/// <param name="image">The image used instead of system icon.</param>
		/// <param name="imageSize">Size of the image. If empty, original image size is used.</param>
		/// <param name="defaultButton">One of the <see cref="System.Windows.Forms.MessageBoxDefaultButton"></see> values that specifies the default button for the message box.</param>
		/// <returns>
		/// One of the <see cref="System.Windows.Forms.DialogResult"></see> values.
		/// </returns>
		public static DialogResult Show( IWin32Window owner, string text, string caption, MessageBoxButtons buttons, Image image, Size imageSize, MessageBoxDefaultButton defaultButton )
		{
			return ShowCore( owner, text, caption, buttons, MessageBoxIcon.None, defaultButton, 0, false, null, image, imageSize );
		}

		/// <summary>
		/// Displays a message box with the specified text, caption, buttons, icon, default button, options, and Help button.
		/// </summary>
		/// <param name="text">The text to display in the message box.</param>
		/// <param name="caption">The text to display in the title bar of the message box.</param>
		/// <param name="buttons">One of the <see cref="System.Windows.Forms.MessageBoxButtons"></see> values that specifies which buttons to display in the message box.</param>
		/// <param name="icon">One of the <see cref="System.Windows.Forms.MessageBoxIcon"></see> values that specifies which icon to display in the message box.</param>
		/// <param name="defaultButton">One of the <see cref="System.Windows.Forms.MessageBoxDefaultButton"></see> values that specifies the default button for the message box.</param>
		/// <param name="options">One of the <see cref="System.Windows.Forms.MessageBoxOptions"></see> values that specifies which display and association options will be used for the message box. You may pass in 0 if you wish to use the defaults.</param>
		/// <param name="displayHelpButton">true to show the Help button; otherwise, false. The default is false.</param>
		/// <returns>
		/// One of the <see cref="System.Windows.Forms.DialogResult"></see> values.
		/// </returns>
		public static DialogResult Show( string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options, bool displayHelpButton )
		{
			return ShowCore( null, text, caption, buttons, icon, defaultButton, options, displayHelpButton );
		}

		/// <summary>
		/// Displays a message box with the specified text, caption, buttons, icon, default button, options, and Help button.
		/// </summary>
		/// <param name="text">The text to display in the message box.</param>
		/// <param name="caption">The text to display in the title bar of the message box.</param>
		/// <param name="buttons">One of the <see cref="System.Windows.Forms.MessageBoxButtons"></see> values that specifies which buttons to display in the message box.</param>
		/// <param name="image">The image used instead of system icon.</param>
		/// <param name="imageSize">Size of the image. If empty, original image size is used.</param>
		/// <param name="defaultButton">One of the <see cref="System.Windows.Forms.MessageBoxDefaultButton"></see> values that specifies the default button for the message box.</param>
		/// <param name="options">One of the <see cref="System.Windows.Forms.MessageBoxOptions"></see> values that specifies which display and association options will be used for the message box. You may pass in 0 if you wish to use the defaults.</param>
		/// <param name="displayHelpButton">true to show the Help button; otherwise, false. The default is false.</param>
		/// <returns>
		/// One of the <see cref="System.Windows.Forms.DialogResult"></see> values.
		/// </returns>
		public static DialogResult Show( string text, string caption, MessageBoxButtons buttons, Image image, Size imageSize, MessageBoxDefaultButton defaultButton, MessageBoxOptions options, bool displayHelpButton )
		{
			return ShowCore( null, text, caption, buttons, MessageBoxIcon.None, defaultButton, options, displayHelpButton, null, image, imageSize );
		}

		/// <summary>
		/// Displays a message box with the specified text, caption, buttons, icon, default button, options, and Help button, using the specified Help file.
		/// </summary>
		/// <param name="text">The text to display in the message box.</param>
		/// <param name="caption">The text to display in the title bar of the message box.</param>
		/// <param name="buttons">One of the <see cref="System.Windows.Forms.MessageBoxButtons"></see> values that specifies which buttons to display in the message box.</param>
		/// <param name="icon">One of the <see cref="System.Windows.Forms.MessageBoxIcon"></see> values that specifies which icon to display in the message box.</param>
		/// <param name="defaultButton">One of the <see cref="System.Windows.Forms.MessageBoxDefaultButton"></see> values that specifies the default button for the message box.</param>
		/// <param name="options">One of the <see cref="System.Windows.Forms.MessageBoxOptions"></see> values that specifies which display and association options will be used for the message box. You may pass in 0 if you wish to use the defaults.</param>
		/// <param name="helpButtonClickHandler">The help button click handler.</param>
		/// <returns>
		/// One of the <see cref="System.Windows.Forms.DialogResult"></see> values.
		/// </returns>
		public static DialogResult Show( string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options, CancelEventHandler helpButtonClickHandler )
		{
			return ShowCore( null, text, caption, buttons, icon, defaultButton, options, true, helpButtonClickHandler );
		}

		/// <summary>
		/// Displays a message box with the specified text, caption, buttons, icon, default button, options, and Help button, using the specified Help file.
		/// </summary>
		/// <param name="text">The text to display in the message box.</param>
		/// <param name="caption">The text to display in the title bar of the message box.</param>
		/// <param name="buttons">One of the <see cref="System.Windows.Forms.MessageBoxButtons"></see> values that specifies which buttons to display in the message box.</param>
		/// <param name="image">The image used instead of system icon.</param>
		/// <param name="imageSize">Size of the image. If empty, original image size is used.</param>
		/// <param name="defaultButton">One of the <see cref="System.Windows.Forms.MessageBoxDefaultButton"></see> values that specifies the default button for the message box.</param>
		/// <param name="options">One of the <see cref="System.Windows.Forms.MessageBoxOptions"></see> values that specifies which display and association options will be used for the message box. You may pass in 0 if you wish to use the defaults.</param>
		/// <param name="helpButtonClickHandler">The help button click handler.</param>
		/// <returns>
		/// One of the <see cref="System.Windows.Forms.DialogResult"></see> values.
		/// </returns>
		public static DialogResult Show( string text, string caption, MessageBoxButtons buttons, Image image, Size imageSize, MessageBoxDefaultButton defaultButton, MessageBoxOptions options, CancelEventHandler helpButtonClickHandler )
		{
			return ShowCore( null, text, caption, buttons, MessageBoxIcon.None, defaultButton, options, true, helpButtonClickHandler, image, imageSize );
		}

		/// <summary>
		/// Displays a message box in front of the specified object and with the specified text, caption, buttons, icon, default button, and options.
		/// </summary>
		/// <param name="owner">The owner.</param>
		/// <param name="text">The text.</param>
		/// <param name="caption">The caption.</param>
		/// <param name="buttons">The buttons.</param>
		/// <param name="icon">The icon.</param>
		/// <param name="defaultButton">One of the <see cref="System.Windows.Forms.MessageBoxDefaultButton"></see> values the specifies the default button for the message box.</param>
		/// <param name="options">The options.</param>
		/// <returns>
		/// One of the <see cref="System.Windows.Forms.DialogResult"></see> values.
		/// </returns>
		public static DialogResult Show( IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options )
		{
			return ShowCore( owner, text, caption, buttons, icon, defaultButton, options, false );
		}

		/// <summary>
		/// Displays a message box in front of the specified object and with the specified text, caption, buttons, icon, default button, and options.
		/// </summary>
		/// <param name="owner">The owner.</param>
		/// <param name="text">The text.</param>
		/// <param name="caption">The caption.</param>
		/// <param name="buttons">The buttons.</param>
		/// <param name="image">The image used instead of system icon.</param>
		/// <param name="imageSize">Size of the image. If empty, original image size is used.</param>
		/// <param name="defaultButton">One of the <see cref="System.Windows.Forms.MessageBoxDefaultButton"></see> values the specifies the default button for the message box.</param>
		/// <param name="options">The options.</param>
		/// <returns>
		/// One of the <see cref="System.Windows.Forms.DialogResult"></see> values.
		/// </returns>
		public static DialogResult Show( IWin32Window owner, string text, string caption, MessageBoxButtons buttons, Image image, Size imageSize, MessageBoxDefaultButton defaultButton, MessageBoxOptions options )
		{
			return ShowCore( owner, text, caption, buttons, MessageBoxIcon.None, defaultButton, options, false, null, image, imageSize );
		}

		private static bool IsEnumValid( Enum enumValue, int value, int minValue, int maxValue )
		{
			return (value >= minValue) && (value <= maxValue);
		}

		private static bool IsEnumWithinShiftedRange( Enum enumValue, int numBitsToShift, int minValAfterShift, int maxValAfterShift )
		{
			int num = Convert.ToInt32( enumValue, CultureInfo.InvariantCulture );
			int num2 = num >> numBitsToShift;

			if( (num2 << numBitsToShift) != num )
			{
				return false;
			}

			return (num2 >= minValAfterShift) && (num2 <= maxValAfterShift);
		}

		private static DialogResult ShowCore( IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
			MessageBoxDefaultButton defaultButton, MessageBoxOptions options, bool showHelp, CancelEventHandler helpButtonClickHandler,
			Image image, Size imageSize )
		{
			DialogResult result;
			if( !IsEnumValid( buttons, (int)buttons, 0, 5 ) )
			{
				throw new InvalidEnumArgumentException( "buttons", (int)buttons, typeof( MessageBoxButtons ) );
			}
			if( !IsEnumWithinShiftedRange( icon, 4, 0, 4 ) )
			{
				throw new InvalidEnumArgumentException( "icon", (int)icon, typeof( MessageBoxIcon ) );
			}
			if( !IsEnumWithinShiftedRange( defaultButton, 8, 0, 2 ) )
			{
				throw new InvalidEnumArgumentException( "defaultButton", (int)defaultButton, typeof( DialogResult ) );
			}
			if( !SystemInformation.UserInteractive && ((options & (MessageBoxOptions.ServiceNotification | MessageBoxOptions.DefaultDesktopOnly)) == 0) )
			{
				throw new InvalidOperationException( "Can't show modal non-interactive service message box." );
			}
			if( (owner != null) && ((options & (MessageBoxOptions.ServiceNotification | MessageBoxOptions.DefaultDesktopOnly)) != 0) )
			{
				throw new ArgumentException( "Can't show service message box with owner", "options" );
			}
			//if( showHelp && ((options & (MessageBoxOptions.ServiceNotification | MessageBoxOptions.DefaultDesktopOnly)) != 0) )
			//{
			//    throw new ArgumentException( "Can't show service message box with help", "options" );
			//}

			int type = showHelp ? 0x4000 : 0;
			type |= (int)(buttons | (MessageBoxButtons)icon | (MessageBoxButtons)defaultButton | (MessageBoxButtons)options);
			IntPtr hOwner = IntPtr.Zero;

			if( showHelp || ((options & (MessageBoxOptions.ServiceNotification | MessageBoxOptions.DefaultDesktopOnly)) == 0) )
			{
				if( options == MessageBoxOptions.DefaultDesktopOnly || options == MessageBoxOptions.ServiceNotification )
				{
					hOwner = NativeMethods.GetDesktopWindow();
				}
				else
				{
					if( owner == null )
					{

						hOwner = NativeMethods.GetActiveWindow();
					}
					else
					{
						hOwner = owner.Handle;
					}
				}
			}

			MessageBoxFormAdv mb = new MessageBoxFormAdv( owner, text, caption, buttons, icon, defaultButton, options, showHelp,
				helpButtonClickHandler, image, imageSize );

            //Centers the MessageBoxAdv to its parent.
            if (owner != null)
                mb.StartPosition = FormStartPosition.CenterParent;

			result = mb.ShowDialog( owner );

			if( hOwner != IntPtr.Zero )
			{
				NativeMethods.SendMessage( hOwner, NativeMethods.WM_SETFOCUS, 0, 0 );
			}

			return result;
		}

		private static DialogResult ShowCore( IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
			MessageBoxDefaultButton defaultButton, MessageBoxOptions options, bool showHelp, CancelEventHandler helpButtonClickHandler )
		{
			return ShowCore( owner, text, caption, buttons, icon, defaultButton, options, showHelp, null, null, Size.Empty );
		}

		private static DialogResult ShowCore( IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
			MessageBoxDefaultButton defaultButton, MessageBoxOptions options, bool showHelp )
		{
			return ShowCore( owner, text, caption, buttons, icon, defaultButton, options, showHelp, null );
		}
	}
}

#endif