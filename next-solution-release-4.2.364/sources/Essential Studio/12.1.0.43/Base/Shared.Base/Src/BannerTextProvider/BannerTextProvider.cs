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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

using Syncfusion.ComponentModel;
using Syncfusion.Runtime.InteropServices;
using Syncfusion.Runtime.Serialization;
using Syncfusion.Windows.Forms.ComponentBannerTextProviders;
using Syncfusion.Windows.Forms.Tools;


namespace Syncfusion.Windows.Forms
{
	/// <summary>
	/// Defines the interface for extendable text box wrappper.
	/// </summary>
	public interface IExtendableTexBox:
		IDisposable
	{
		/// <summary>
		/// Text box control native handle.
		/// </summary>
		/// <remarks>Must return <see cref="IntPtr.Zero"/> if handle isn't created yet.</remarks>
		IntPtr Handle { get; }

		/// <summary>
		/// Occurs when text box is created.
		/// </summary>
		event EventHandler HandleCreated;

		/// <summary>
		/// Occurs when text of text box is changed.
		/// </summary>
		event ValueChangedEventHandler TextBoxTextChanged;
		event EventHandler TextBoxMouseDown;
		event EventHandler TextBoxMouseDblClick;
		/// <summary>
		/// Indicates whether text box is focused.
		/// </summary>
		bool Focused { get; }

		/// <summary>
		/// Background color of text box.
		/// </summary>
		Color BackColor { get; }

		/// <summary>
		/// Client rectangle of text box
		/// </summary>
		Rectangle ClientRectangle { get; }

		/// <summary>
		/// Font of text box.
		/// </summary>
		Font Font { get; }

		/// <summary>
		/// Indicates whether text box is in RTL mode.
		/// </summary>
		RightToLeft RightToLeft { get;}

		/// <summary>
		/// Invalidates text box.
		/// </summary>
		void Invalidate();
	}

	/// <summary>
	/// Defines the interface for extending text box owned by some components.
	/// </summary>
	public interface IComponentBannerTextProvider
	{
		/// <summary>
		/// Specifies whether this component can provide banner text extender properties to the specified object.
		/// </summary>
		/// <param name="component">The <see cref="Component"/> to receive the extender properties.</param>
		/// <returns>true if this object can provide extender properties to the specified object.</returns>
		bool CanExtend( Component extendee );

		/// <summary>
		/// Retrieves extendable text box info.
		/// </summary>
		/// <param name="extendee">Extended component.</param>
		/// <returns><see cref="IExtendableTexBox"/> object</returns>
		IExtendableTexBox GetExtendableTexBox( Component extendee, BannerTextProvider provider );

		/// <summary>
		/// Type of extended component.
		/// </summary>
		Type ComponentType { get; }
	}

	public abstract class ComponentBannerTextProviderBase:
		IComponentBannerTextProvider
	{
		#region Data

		/// <summary>
		/// Component's type.
		/// </summary>
		protected Type m_type;

		#endregion

		#region IComponentBannerTextProvider Members

		public virtual bool CanExtend( Component extendee )
		{
			return (extendee != null && this.ComponentType.IsInstanceOfType( extendee ));
		}

		public abstract IExtendableTexBox GetExtendableTexBox( Component extendee, BannerTextProvider provider );

		public virtual Type ComponentType
		{
			get
			{
				return m_type;
			}
		}

		#endregion
	}


	[ProvideProperty( "BannerText", typeof( Component ) )]
	[Description( "Provides ability to show banner text (text cue) for text box" )]
	[Designer( typeof( Design.BannerTextProviderDesigner ) )]
	[ToolboxBitmap( typeof( BannerTextProvider ), "ToolboxIcons.BannerTextProvider.bmp" )]
	public class BannerTextProvider:
		Component,
		IExtenderProvider
	{
		#region Data

		/// <summary>
		/// Collection of extended components.
		/// </summary>
		private static IDictionary s_extendedComponents = new Hashtable();

		/// <summary>
		/// Stores text box to component backaward mapping. 
		/// </summary>
		private static IDictionary s_etb2component = null;

		/// <summary>
		/// Stores map of bannet text info to extenders <see cref="ArrayList"/>.
		/// </summary>
		private static IDictionary s_info2extenders = new Hashtable();

		/// <summary>
		/// Default banner text providers.
		/// </summary>
		private static IDictionary s_bannerProviders = new Hashtable();

		#endregion

		#region Construction

		/// <summary>
		/// Default constructor.
		/// </summary>
		public BannerTextProvider()
		{
		}

		/// <summary>
		/// Creates instance of <see cref="BannerTextProvider"/> class and registers it in owner's container.
		/// </summary>
		/// <param name="container">Owner's container.</param>
		public BannerTextProvider( IContainer container ):
			this()
		{
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(BannerTextProvider));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
			container.Add( this );
		}

		/// <summary>
		/// Static constructor.
		/// </summary>
		/// <remarks>Registers default banner text providers.</remarks>
		static BannerTextProvider()
		{
			RegisterDefaultProviders();			
		}

		private static void RegisterDefaultProviders()
		{
			RegisterProvider( new TextBoxBannerTextProvider() );
			RegisterProvider( new ComboDropDownBannerTextProvider() );
			RegisterProvider( new ComboBoxBannerTextProvider() );

			RegisterMoreProviders();
		}

		private static void RegisterMoreProviders()
		{
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

			foreach( Assembly asm in assemblies )
			{
				string name = asm.GetName().Name;
                if (name == "Syncfusion.Tools.Windows" || name == "System.Tools.Windows")
				{
					// Retrieve type info to invoke static constructor of BannerTextProvider2 class, where additional providers are registered.
					Type bannerTextProvider2 = asm.GetType( "Syncfusion.Windows.Forms.ComponentBannerTextProviders.BannerTextProvider2" );

					if( bannerTextProvider2 != null )
					{
						ConstructorInfo ci = bannerTextProvider2.GetConstructor( new Type[] { } );
						Component bannerTextProvider = (Component)ci.Invoke( null );

						bannerTextProvider.Dispose();
					}
				}
			}
		}
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            s_etb2component = null;
        }
		/// <summary>
		/// Register additional <see cref="IComponentBannerTextProvider"/>.
		/// </summary>
		/// <param name="provider">Provider to register.</param>
		public static void RegisterProvider( IComponentBannerTextProvider provider )
		{
			if( provider != null && !s_bannerProviders.Contains( provider ) )
			{
				s_bannerProviders.Add( provider.ComponentType, provider );
			}
		}

		#endregion Construction

		#region IExtenderProvider implementation

		bool IExtenderProvider.CanExtend( object extendee )
		{
			bool bCanExtend = false;
			Component component = extendee as Component;

			if( component != null )
			{
				IComponentBannerTextProvider provider = GetProviderForType( component.GetType() );

				bCanExtend = (provider != null && provider.CanExtend( component ));
			}

			return bCanExtend;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Returns <see cref="BannerTextInfo"/> for given component.
		/// </summary>
		/// <param name="textBox">Component to retirive info for.</param>
		/// <returns>Associated <see cref="BannerTextInfo"/> object.</returns>
		/// <remarks>Returns a copy of <see cref="BannerTextInfo"/> object.</remarks>
		[
			Category("Banner text"),
			Localizable(true)
		]
		public BannerTextInfo GetBannerText( Component component )
		{
			BannerTextInfo info = null;

			if( component != null )
			{
				if( s_extendedComponents.Contains( component ) )
				{
					info = (BannerTextInfo)s_extendedComponents[component];
				}
				else
				{
					info = new BannerTextInfo();

					SetBannerText( component, info );
				}
			}

			return info;
		}

		protected bool ShouldSerializeBannerText( Component component )
		{
			return GetBannerText( component ) != BannerTextInfo.Empty;
		}

		protected void ResetBannerText( Component component )
		{
			SetBannerText( component, new BannerTextInfo() );
		}

		/// <summary>
		/// Associates <see cref="BannerTextInfo"/> with component.
		/// </summary>
		/// <param name="textBox">Component.</param>
		/// <param name="info"><see cref="BannerText"/> info object.</param>
		public void SetBannerText( Component component, BannerTextInfo info )
		{
			if( component != null && (this as IExtenderProvider).CanExtend( component ) )
			{			
				if( s_extendedComponents.Contains( component ) )
				{
					if( info != null )
					{
						s_extendedComponents[component] = info;
					}
					else
					{
						info = (BannerTextInfo)s_extendedComponents[component];

						s_extendedComponents.Remove( component );						

						if( s_info2extenders.Contains( info ) )
						{
							ArrayList extenders = (ArrayList)s_info2extenders[info];

							foreach( TextBoxExtender ext in extenders )
							{
								IExtendableTexBox etb = ext.ETB;

								etb.HandleCreated -= new EventHandler( TextBoxHandleCreated );
								etb.Dispose();

								BannerTextProvider.ETB2Component.Remove( etb );
								ext.ReleaseHandle();
							}

							s_info2extenders.Remove( info );
						}
					}
				}
				else if( info != null )
				{
					s_extendedComponents[component] = info;

					IComponentBannerTextProvider provider =	GetProviderForType( component.GetType() );
					IExtendableTexBox etb = provider.GetExtendableTexBox( component, this );

					BannerTextProvider.ETB2Component.Add( etb, component );

					if( etb.Handle != IntPtr.Zero )
					{
						Extend( component, etb );
					}

					etb.HandleCreated += new EventHandler( TextBoxHandleCreated );
					component.Disposed += new EventHandler( OnComponentDisposed );
				}
			}
		}

		/// <summary>
		/// Retrieves text box to component backward map.
		/// </summary>
		/// <remarks>Mapping is created on demand.</remarks>
		protected static IDictionary ETB2Component
		{
			get
			{
				if( s_etb2component == null )
				{
					s_etb2component = new Hashtable();
				}

				return s_etb2component;
			}
		}

		#endregion Properties

		#region Implementation

		protected void Extend( Component component, IExtendableTexBox etb )
		{
			if( !this.DesignMode && etb != null && component != null )
			{
				if( s_extendedComponents.Contains( component ) )
				{
					BannerTextInfo info = (BannerTextInfo)s_extendedComponents[component];
					TextBoxExtender extender = new TextBoxExtender( info, etb );
					ArrayList extenders = null;

					if( s_info2extenders.Contains( info ) )
					{
						extenders = (ArrayList)s_info2extenders[info];
					}
					else
					{
						extenders = new ArrayList();
						s_info2extenders[info] = extenders;
					}

					extenders.Add( extender );
				}
			}
		}

		protected void Extend( IExtendableTexBox mainETB, IExtendableTexBox subETB )
		{
			IDictionary etb2Component = BannerTextProvider.ETB2Component;

			if( etb2Component.Contains( mainETB ) )
			{
				Component component = (Component)etb2Component[mainETB];
				BannerTextInfo info = (BannerTextInfo)s_extendedComponents[component];

				etb2Component.Add( subETB, component );
				Extend( component, subETB );
			}
		}

		protected static void Extend( BannerTextProvider provider, IExtendableTexBox mainETB, IExtendableTexBox subETB )
		{
			provider.Extend( mainETB, subETB );
		}

		private IComponentBannerTextProvider GetProviderForType( Type type )
		{
			IComponentBannerTextProvider provider = null;

			if( s_bannerProviders.Contains( type ) )
			{
				provider = (IComponentBannerTextProvider)s_bannerProviders[type];
			}
			else
			{
				foreach( DictionaryEntry entry in s_bannerProviders )
				{
					Type baseType = (Type)entry.Key;

					if( type.IsSubclassOf( baseType ) )
					{
						provider = (IComponentBannerTextProvider)entry.Value;
						break;
					}
				}
			}

			return provider;
		}

		private void OnComponentDisposed( object sender, EventArgs e )
		{
			Component component = sender as Component;

			if( component != null )
			{
				component.Disposed -= new EventHandler( OnComponentDisposed );
				SetBannerText( component, null );
			}
		}

		#endregion

		#region Event handlers

		void TextBoxHandleCreated( object sender, EventArgs e )
		{
			IExtendableTexBox etb = (IExtendableTexBox)sender;
			Component component = (Component)BannerTextProvider.ETB2Component[etb];

			Extend( component, etb );
		}

		void TextBoxHandleDestroyed( object sender, EventArgs e )
		{
			throw new Exception( "The method or operation is not implemented." );
		}

		#endregion

		/// <summary>
		/// Renders banner text for <see cref="TextBox"/>-derived control.
		/// </summary>
		public class TextBoxExtender:
			NativeWindow
		{
			#region Data

			private BannerTextInfo m_info;
			private IExtendableTexBox m_etb;
			private bool m_bIsTextEmpty = false;

			#endregion

			#region Construction

			public TextBoxExtender( BannerTextInfo info, IExtendableTexBox etb )
			{
				Debug.Assert( info != null );
				Debug.Assert( etb != null );

				m_info = info;
				m_etb = etb;

				Initialize();
			}

			#endregion

			#region Implementation

			private void Initialize()
			{
				if( m_etb.Handle != IntPtr.Zero )
				{
					SubclassControl();
				}
				else
				{
					m_etb.HandleCreated += new EventHandler( TextBoxHandleCreated );
				}

				if (m_etb != null)
				{
					m_etb.TextBoxTextChanged += new ValueChangedEventHandler( TextBoxTextChanged );
					m_etb.TextBoxMouseDblClick += new EventHandler(m_etb_TextBoxMouseDblClick);
					m_etb.TextBoxMouseDown += new EventHandler(m_etb_TextBoxMouseDown);
				}
			}

            void m_etb_TextBoxMouseDblClick(object sender, EventArgs e)
            {
                m_etb.Invalidate();
            }
            void m_etb_TextBoxMouseDown(object sender, EventArgs e)
            {
                m_etb.Invalidate();
            }

			private void SubclassControl()
			{
				m_bIsTextEmpty = (NativeMethods.GetWindowTextLength( this.Handle ) == 0);

				AssignHandle( m_etb.Handle );				
			}

			private void TextBoxHandleCreated( object sender, EventArgs e )
			{
				SubclassControl();
			}

			private void TextBoxTextChanged( object sender, ValueChangedEventArgs e )
			{
				SetIsTextEmpty( (string)e.newValue );

                if (m_bIsTextEmpty && sender is ExtendableComboBoxTextBox)
                {
                    ((ExtendableComboBoxTextBox)sender).ResetCursorPosition();
                }
             
                if (m_bIsTextEmpty && sender is ExtendableTextBox)
				{
					((ExtendableTextBox)sender).ResetCursorPosition();
				}
				m_etb.Invalidate();
			}

			private void SetIsTextEmpty( string sText )
			{
				m_bIsTextEmpty = (sText.Length == 0);
			}

			private bool OnWmPaint()
			{
				bool bHandled = false;

				if( this.ShouldDrawBanner )
				{
					NativeMethods.PAINTSTRUCT paintStruct = new NativeMethods.PAINTSTRUCT();
					IntPtr hDC = NativeMethods.BeginPaint( this.Handle, ref paintStruct );
					Graphics g = Graphics.FromHdc( hDC );

					OnPaint( g );

					NativeMethods.EndPaint( this.Handle, ref paintStruct );

					bHandled = true;
				}

				return bHandled;
			}

			protected virtual void OnPaint( Graphics g )
			{
				DrawBannerText( g, m_info, m_etb.BackColor, m_etb.Font, m_etb.RightToLeft, m_etb.ClientRectangle );
			}

			/// <summary>
			/// Draws banner text.
			/// </summary>
			/// <param name="g"><see cref="Graphics"/> to draw at.</param>
			/// <param name="info">Specifies banner text appearance.</param>
			/// <param name="bgColor">Banner text background color.</param>
			/// <param name="font">Fallback font for the case when <see cref="info"/> doesn't contain valid font.</param>
			/// <param name="rtl">Text flow layout.</param>
			/// <param name="clientRect">Rectangle to draw within.</param>
			public static void DrawBannerText( Graphics g, BannerTextInfo info, Color bgColor, Font font, RightToLeft rtl, Rectangle clientRect )
            {
				g.FillRectangle(new SolidBrush(bgColor), clientRect);
				clientRect = new Rectangle(clientRect.X, clientRect.Y - 1, clientRect.Width, clientRect.Height);


				using (StringFormat sf = new StringFormat(StringFormat.GenericTypographic))
				{
					sf.Alignment = StringAlignment.Near;
					sf.FormatFlags = StringFormatFlags.MeasureTrailingSpaces | StringFormatFlags.NoWrap;
					sf.LineAlignment = StringAlignment.Center;
					sf.Trimming = StringTrimming.EllipsisCharacter;

					if (rtl == RightToLeft.Yes)
					{
						sf.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
					}

					if (info.Font != null)
					{
						font = info.Font;
					}

					using(Brush brush = new SolidBrush(info.Color))
					{
						g.DrawString(info.Text, font, brush, clientRect, sf);
					}
				}
			}

			/// <summary>
			/// Determines whether draw text is visible and must be drawn.
			/// </summary>
			/// <param name="info">Describes banner text appearance.</param>
			/// <param name="bFocused">Indicates whether extended text box is focused.</param>
			/// <param name="bEmptyText">Indicates whether extended text box's text is empty.</param>
			/// <returns>True if banner text is visible.</returns>
			public static bool IsBannerTextVisible( BannerTextInfo info, bool bFocused, bool bEmptyText )
			{
				bool bDraw = (info.Visible && info.Text.Length > 0);

				if( bDraw )
				{
					switch( info.Mode )
					{
						case BannerTextMode.FocusMode:
							bDraw = !bFocused && bEmptyText;
							break;

						case BannerTextMode.EditMode:
							bDraw = bEmptyText;
							break;
					}
				}

				return bDraw;
			}

			#endregion

			#region Properties

			private void OnInfoChanged()
			{
				if( this.ShouldDrawBanner )
				{
					m_etb.Invalidate();
				}
			}

			protected bool IsTextEmpty
			{
				get
				{
					m_bIsTextEmpty = (NativeMethods.GetWindowTextLength( this.Handle ) == 0);
					return m_bIsTextEmpty;
				}
			}

			protected bool ShouldDrawBanner
			{
				get
				{
					return IsBannerTextVisible( m_info, m_etb.Focused, this.IsTextEmpty );
				}
			}

			public IExtendableTexBox ETB
			{
				get
				{
					return m_etb;
				}
			}

			#endregion

			#region Overrides

			public override void ReleaseHandle()
			{
				if( m_etb != null )
				{
					m_etb.TextBoxTextChanged -= new ValueChangedEventHandler( TextBoxTextChanged );
					m_etb.HandleCreated -= new EventHandler( TextBoxHandleCreated );
				}

				m_info = null;
				m_etb = null;

				base.ReleaseHandle();				
			}

			protected override void WndProc( ref Message m )
			{
				bool bHandled = false;

				if( m.Msg != NativeMethods.WM_GETTEXTLENGTH && m_info != null && m_etb != null )
				{
					if( m_info.Visible )
					{
						switch( m.Msg )
						{
							case NativeMethods.WM_PAINT:
								bHandled = OnWmPaint();
								break;
							case NativeMethods.WM_MOUSEFIRST:
							case NativeMethods.WM_MOUSEHOVER:
							case NativeMethods.WM_MOUSELEAVE:
								bHandled = true;
								break;

							case NativeMethods.WM_SETFOCUS:
							case NativeMethods.WM_KILLFOCUS:
								m_etb.Invalidate();
								break;
						}
					}
				}

				if( !bHandled )
				{
					base.WndProc( ref m );
				}
			}

			protected override void OnHandleChange()
			{
				base.OnHandleChange();
			}

			#endregion
		}
	}

	/// <summary>
	/// Describes banner text rendering behavior.
	/// </summary>
	public enum BannerTextMode
	{
		/// <summary>
		/// Banner text disappears when the control gets focus.
		/// </summary>
		FocusMode,
		/// <summary>
		/// Banner text disappears only when associated text box is not empty.
		/// </summary>
		EditMode
	}

	/// <summary>
	/// Describes banner text appearance and behavior.
	/// </summary>
	[
		TypeConverter( typeof( BannerTextInfoTypeConverter ) )
	]
	public class BannerTextInfo
	{
		#region Data

		private string m_sText = String.Empty;
		private bool m_bVisible = false;
		private Font m_font = null;
		private BannerTextMode m_mode = BannerTextMode.FocusMode;
		private Color m_color = SystemColors.ControlDark;

		public static readonly BannerTextInfo Empty;

		#endregion

		#region Construction

		/// <summary>
		/// Static constructor.
		/// </summary>
		static BannerTextInfo()
		{
			BannerTextInfo.Empty = new BannerTextInfo();
		}

		/// <summary>
		/// Default constructor.
		/// </summary>
		public BannerTextInfo()
		{
		}

		/// <summary>
		/// Copy constructor.
		/// </summary>
		public BannerTextInfo( BannerTextInfo other )
		{
			this.Color = other.Color;
			this.Font = other.Font;
			this.Mode = other.Mode;
			this.Text = other.Text;
			this.Visible = other.Visible;
		}

		public BannerTextInfo( string text, bool bVisible, Font font, Color color, BannerTextMode mode )
		{
			this.Text = text;
			this.Visible = bVisible;
			this.Font = font;
			this.Color = color;
			this.Mode = mode;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Specifies banner text.
		/// </summary>
		[
			Description( "Specifies banner text." ),
			DefaultValue( "" ),
			Localizable( true )
		]
		public string Text
		{
			get
			{
				return m_sText;
			}
			set
			{
				m_sText = value;
			}
		}

		/// <summary>
		/// Specifies whether banner text is visible.
		/// </summary>
		[
			Description("Specifies whether banner text is visible."),
			DefaultValue(false)
		]
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
		/// Specifies font of banner text.
		/// </summary>
		[
			Description( "Specifies font of banner text." ),
			DefaultValue(null)
		]
		public Font Font
		{
			get
			{
				return m_font;
			}
			set
			{
				m_font = value;
			}
		}

		/// <summary>
		/// Specifies banner text rendering mode. See <see cref="BannerTextMode"/> for details.
		/// </summary>
		[
			Description( "Specifies banner text rendering mode. See BannerTextMode for details." ),
			DefaultValue( BannerTextMode.FocusMode )
		]
		public BannerTextMode Mode
		{
			get
			{
				return m_mode;
			}
			set
			{
				m_mode = value;
			}
		}

		/// <summary>
		/// Specifies banner text color.
		/// </summary>
		[
			Description( "Specifies banner text color." ),
		]
		public Color Color
		{
			get
			{
				return m_color;
			}
			set
			{
				m_color = value;
			}
		}

		protected bool ShouldSerializeColor()
		{
			return this.Color != BannerTextInfo.Empty.Color;
		}

		protected void ResetColor()
		{
			this.Color = BannerTextInfo.Empty.Color;
		}

		#endregion

		#region Overrides

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool Equals( object obj )
		{
			bool bEquals = false;

			if( obj != null  )
			{
				bEquals = Object.ReferenceEquals( this, obj );

				if( !bEquals )
				{
					BannerTextInfo other = obj as BannerTextInfo;

					if( other != null )
					{
						bEquals =	(this.Text == other.Text)
								&&	(this.Visible == other.Visible)
								&&	(this.Color == other.Color)
								&&	(this.Mode == other.Mode);

						if( bEquals )
						{
							if( this.Font == null )
							{
								bEquals = (other.Font == null);
							}
							else if( other.Font == null )
							{
								bEquals = (this.Font == null);
							}
							else
							{
								bEquals = (this.Font.ToString() == other.Font.ToString());
							}
						}
					}
				}
			}

			return bEquals;
		}

		#endregion

		#region Operators

		public static bool operator==( BannerTextInfo lVal, BannerTextInfo rVal )
		{
			return Object.ReferenceEquals( lVal, null ) ? Object.ReferenceEquals( rVal, null ) : lVal.Equals( rVal );
		}

		public static bool operator!=( BannerTextInfo lVal, BannerTextInfo rVal )
		{
			return !(lVal == rVal);
		}

		#endregion
	}

	/// <summary>
	/// Type converter for <see cref="BannerTextInfo"/>
	/// </summary>
	internal class BannerTextInfoTypeConverter:
		ExpandableObjectConverter
	{
		#region Overrides

		public override bool CanConvertTo( ITypeDescriptorContext context, Type destinationType )
		{
			if( destinationType == typeof( InstanceDescriptor ) )
			{
				return true;
			}

			return base.CanConvertTo( context, destinationType );
		}

		public override object ConvertTo( ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType )
		{
			BannerTextInfo info = value as BannerTextInfo;

			if( info != null )
			{
				if( destinationType == typeof( InstanceDescriptor ) )
				{
					ConstructorInfo ci = typeof( BannerTextInfo ).GetConstructor( new Type[] {} );
					return new InstanceDescriptor( ci, new object[] {}, false );
				}
			}

			return base.ConvertTo( context, culture, value, destinationType );
		}

		#endregion
	}

	namespace Design
	{
		/// <summary>
		/// Designer for <see cref="BannerTextProvider"/>
		/// </summary>
		internal class BannerTextProviderDesigner:
			ComponentDesigner
		{
			#region Data

			private IComponentChangeService m_changeSvc;

			#endregion

			#region Overrides

			public override void Initialize( IComponent component )
			{
				base.Initialize( component );

				m_changeSvc = (IComponentChangeService)this.GetService( typeof( IComponentChangeService ) );
				m_changeSvc.ComponentAdded += new ComponentEventHandler( ComponentAdded );
			}

			protected override void Dispose( bool disposing )
			{
				base.Dispose( disposing );

				if( disposing )
				{
					m_changeSvc.ComponentAdded -= new ComponentEventHandler( ComponentAdded );
					m_changeSvc = null;
				}
			}

			#endregion

			#region Implementation
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
			private delegate void DestroyComponentAsyncHandler( IComponent component );
#endif
			private void ComponentAdded( object sender, ComponentEventArgs e )
			{
				IDesignerHost host = (IDesignerHost)this.GetService( typeof( IDesignerHost ) );
				IComponent component = e.Component;

				if( host != null && component is BannerTextProvider && component != this.Component )
				{
					MessageBox.Show( "Multiple instances of BannerTextProvider are not allowed.\nThe new instance will be removed.",
						"Multiple BannerTextProvider instances warning", MessageBoxButtons.OK, MessageBoxIcon.Information );
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
					DestroyComponentAsyncHandler handler = new DestroyComponentAsyncHandler( host.DestroyComponent );

					handler.BeginInvoke( component, null, null );
#else
					host.DestroyComponent( component );
#endif
					ISelectionService selSvc = (ISelectionService)this.GetService( typeof( ISelectionService ) );

					selSvc.SetSelectedComponents( new object[] { this.Component } );
				}
			}

			#endregion
		}
	}
}