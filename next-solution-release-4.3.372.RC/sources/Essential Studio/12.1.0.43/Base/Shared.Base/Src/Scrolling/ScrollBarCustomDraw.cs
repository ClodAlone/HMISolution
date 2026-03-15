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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using Syncfusion.Windows.Forms.Collections;
using Syncfusion.Windows.Forms.Localization;
using Syncfusion.Windows.Forms.Renderers;
using Syncfusion.Windows.Forms.Scrolling.Renderers;
#endregion

namespace Syncfusion.Windows.Forms
{
	/// <summary>Base class for scrollers control that support visual styles.</summary>
	[
	ToolboxItem( false ),
	Designer( typeof( ScrollBarCustomDrawDesigner ), typeof( IDesigner ) ),
	TypeConverter( typeof( ExpandableObjectConverter ) )
	]
	public class ScrollBarCustomDraw:
		ContainerControl
	{
		#region class constant
		/// <summary></summary>
		private const string DEF_COLOR_SCHEME = "";
		/// <summary>Key for LargeChangeChanged event.</summary>
		private static readonly object EventLargeChangeChanged = new object();
		/// <summary>Key for MaximumChanged event.</summary>
		private static readonly object EventMaximumChanged = new object();
		/// <summary>Key for MinimumChanged event.</summary>
		private static readonly object EventMinimumChanged = new object();
		/// <summary>Key for SmallChangeChanged event.</summary>
		private static readonly object EventSmallChangeChanged = new object();
		/// <summary>Key for ValueChanged event.</summary>
		private static readonly object EventValueChanged = new object();
#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
    /// <summary>Key for Scroll event.</summary>
    private static readonly object EventScroll = new object();
#endif
		/// <summary>Key for VisualStyleChanged event.</summary>
		private static readonly object EventVisualStyleChanged = new object();
		/// <summary>Key for ColorSchemeChanged event.</summary>
		private static readonly object EventColorSchemeChanged = new object();
		/// <summary>Key for ThemeEnabledChanged event.</summary>
		private static readonly object EventThemeEnabledChanged = new object();
		/// <summary>
		/// Used by threading timer.
		/// </summary>
		private readonly TimerCallback timerDelegate;
		#endregion

		#region class members
		/// <summary>
		/// A value to be added to or subtracted from the System.Windows.Forms.ScrollBar.Value
		/// property when the scroll box is moved a large distance.
		/// </summary>
		protected int m_largeChange = 10;
		/// <summary>
		/// Cached m_largeChange
		/// </summary>
		protected int m_cachedLargeChange = 10;
		/// <summary>
		/// The upper limit of values of the scrollable range.
		/// </summary>
		protected int m_max = 100;
		/// <summary>
		/// The lower limit of values of the scrollable range.
		/// </summary>
		protected int m_min = 0;
		/// <summary>
		/// A value to be added to or subtracted from the Syncfusion.Windows.Forms.ScrollBarCustomDraw.Value
		/// property when the scroll box is moved a small distance.
		/// </summary>
		protected int m_smallChange = 1;
		/// <summary>
		/// A numeric value that represents the current position of the
		/// scroll box on the scroll bar control.
		/// </summary>
		protected int m_value = 10;
		/// <summary>
		/// Visual style of the ScrollBarCustomDraw.
		/// </summary>
		private ScrollBarCustomDrawStyles m_styles;
		/// <summary>
		/// Indicates whether to use visual styles.
		/// </summary>
		private bool m_bThemeEnabled = true;
		/// <summary>
		/// Indicates whether thumb is disabled.
		/// </summary>
		private bool m_bDisableThumb = false;
		/// <summary>
		/// Indicates whether minimum arrow is disabled.
		/// </summary>
		private bool m_bDisableMin = false;
		/// <summary>
		/// Indicates whether maximum arrow is disabled.
		/// </summary>
		private bool m_bDisableMax = false;
		/// <summary>
		/// Collection of controls that locates above the minimum arrow.
		/// </summary>
		private ControlsCollection m_controlsBefore;
		/// <summary>
		/// Collection of controls that locates under the maximum arrow.
		/// </summary>
		private ControlsCollection m_controlsAfter;
		/// <summary>
		/// True - say control to keep System settings instead of user defined, otherwise False.
		/// </summary>
		private bool m_bKeepSystemmetrics = false;
		/// <summary>
		/// Renderer which draws the control.
		/// </summary>
		private IRenderer m_renderer;
		/// <summary>
		/// Array of rectangles that represents regions of the ScrollBarCustomDraw.
		/// </summary>
		protected Rectangle[] m_rects = new Rectangle[(int)PressedZone.Length];
		/// <summary>
		/// Used when user holds the arrow button clicked.
		/// </summary>
		private System.Threading.Timer m_timer;
		/// <summary>
		/// Position where was last click.
		/// </summary>
		protected Point m_lastPressedPoint = Point.Empty;
		/// <summary>
		/// Zone where was last click .
		/// </summary>
		private PressedZone m_pressedZone = PressedZone.None;
		/// <summary>
		/// Zone where mouse is over. 
		/// </summary>
		private PressedZone m_selectedZone = PressedZone.None;
		/// <summary>
		/// Cached m_selectedZone.
		/// </summary>
		private PressedZone m_cachedSelectedZone = PressedZone.None;
		/// <summary>
		/// Zone where is mouse position.
		/// </summary>
		private MovedZone m_movedZone = MovedZone.Out;
		/// <summary>
		/// False if all states is default? in other case value is false. 
		/// </summary>
		private bool m_bUpdateStates = false;
		/// <summary>
		/// Color scheme that used in Rendering. 
		/// </summary>
		private Office2007ColorScheme m_OfficeColorScheme = Office2007ColorScheme.Blue;
        /// <summary>
        /// Color scheme that used in Rendering. 
        /// </summary>
        private MetroColorScheme m_MetroColorScheme =MetroColorScheme.Magenta;
		/// <summary>
        /// Color scheme for Office2010 that used in Rendering. 
        /// </summary>
        private Office2010ColorScheme m_Office2010ColorScheme = Office2010ColorScheme.Blue;
        /// <summary>
		/// Instance of ContextMenu provider.
		/// </summary>
		protected IContextMenuProvider m_MenuProvider;
		/// <summary>
		/// To avoid compile error CS0197 in VS2002.
		/// </summary>
		protected Point m_ptLocation = Point.Empty;
		/// <summary>
		/// To prevent validation controls visibility in <see cref="Synfusion.Windows.Forms.ScrollBarCustomDraw.OnControlAdded"/>
		/// and in <see cref="Synfusion.Windows.Forms.ScrollBarCustomDraw.OnControlRemoved"/>.
		/// </summary>
		protected bool m_IsSetControlsVisiblity = false;
		/// <summary>
		/// Indicates whether scrollbar should be refreshed on each value change.
		/// If set to false, scrollbar is invalidated only and therefore is visually refreshed after processing all scrolling messages.
		/// </summary>
		private bool m_bRefreshOnValueChange = false;
		/// <summary>
		/// Scroll bar's owner.
		/// </summary>
		protected ScrollersFrame m_owner = null;
        /// <summary>
        /// Metro color table for metro visual style.
        /// </summary>
        private MetroColorTable m_metroColorTable;
		#endregion

		#region class Properties
		/// <summary>
		/// Gets or sets a value to be added to or subtracted from the System.Windows.Forms.ScrollBar.Value
		/// property when the scroll box is moved a large distance.
		/// </summary>
		[
		Description( "ScrollBarLargeChangeDescr" ),
		Category( "CatBehavior" ),
		DefaultValue( 10 ),
		RefreshProperties( RefreshProperties.Repaint )
		]
		public int LargeChange
		{
			get
			{
				return m_largeChange;
			}
			set
			{
				if( m_largeChange != value )
				{
					if( value > m_max - m_min + 1 )
					{
						m_largeChange = m_max - m_min + 1;
						m_value = m_min;
					}
					else
					{
						m_largeChange = value;
						m_cachedLargeChange = value;
					}

					OnLargeChangeChanged( EventArgs.Empty );

					RecalculateThumb();
					Invalidate();
				}
			}
		}

		/// <summary>
		/// Gets or sets the upper limit of values of the scrollable range.
		/// </summary>
		[
		Description( "ScrollBarMaximumDescr" ),
		DefaultValue( 100 ),
		Category( "Behavior" ),
		RefreshProperties( RefreshProperties.Repaint )
		]
		public int Maximum
		{
			get
			{
				return m_max;
			}
			set
			{
				if( m_max != value )
				{
					m_max = value;

					if( m_max < m_min )
					{
						m_min = m_max;
					}

					if( m_max < m_value + m_largeChange - 1 )
					{
						m_value = m_max - m_largeChange + 1;
					}

					if( m_max < m_min + m_largeChange - 1 || m_largeChange < m_cachedLargeChange )
					{
						if( m_max - m_min == m_cachedLargeChange )
						{
							m_largeChange = m_max - m_min;
							m_value = m_min + 1;
						}
						else
						{
							m_largeChange = ( m_max - m_min + 1 < m_cachedLargeChange ) ?
								m_max - m_min + 1 : m_cachedLargeChange;
							m_value = m_min;
						}
					}
					else
					{
						m_largeChange = m_cachedLargeChange;
					}

					OnMaximumChanged( EventArgs.Empty );

					RecalculateThumb();
					Invalidate();
				}
			}
		}

		/// <summary>
		/// Gets or sets the lower limit of values of the scrollable range.
		/// </summary>
		[
		Category( "Behavior" ),
		DefaultValue( 0 ),
		Description( "ScrollBarMinimumDescr" ),
		RefreshProperties( RefreshProperties.Repaint )
		]
		public int Minimum
		{
			get
			{
				return m_min;
			}
			set
			{
				if( m_min != value )
				{
					m_min = value;

					if( m_min > m_max )
					{
						m_max = m_min;
					}

					if( m_min > m_value )
					{
						m_value = m_min;
					}

					if( m_min > m_max - m_largeChange + 1 || m_largeChange < m_cachedLargeChange )
					{
						if( m_max - m_min == m_cachedLargeChange )
						{
							m_largeChange = m_max - m_min;
							m_value = m_min + 1;
						}
						else
						{
							m_largeChange = ( m_max - m_min + 1 < m_cachedLargeChange ) ?
								m_max - m_min + 1 : m_cachedLargeChange;
							m_value = m_min;
						}
					}
					else
					{
						m_largeChange = m_cachedLargeChange;
					}

					OnMinimumChanged( EventArgs.Empty );

					RecalculateThumb();
					Invalidate();
				}
			}
		}

		/// <summary>
		/// Gets or sets a value to be added to or subtracted from the Syncfusion.Windows.Forms.ScrollBarCustomDraw.Value
		/// property when the scroll box is moved a small distance.
		/// </summary>
		[
		Category( "Behavior" ),
		DefaultValue( 1 ),
		Description( "ScrollBarSmallChangeDescr" )
		]
		public int SmallChange
		{
			get
			{
				return m_smallChange;
			}
			set
			{
				if( m_smallChange != value )
				{
					m_smallChange = value;
					OnSmallChangeChanged( EventArgs.Empty );
					Invalidate();
				}
			}
		}

		/// <summary>
		/// Gets or sets a numeric value that represents the current position of the
		/// scroll box on the scroll bar control.
		/// </summary>
		[
		Category( "Behavior" ),
		Bindable( true ),
		DefaultValue( 0 ),
		Description( "ScrollBarValueDescr" )
		]
		public int Value
		{
			get
			{
				return m_value;
			}
			set
			{
				if( m_value != value )
				{
					SetInnerValue( value );
					OnValueChanged( EventArgs.Empty );
					if( this.RefreshOnValueChange )
					{
						Refresh();
					}
					else
					{
						Invalidate();
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the text associated with this control.
		/// </summary>
		[
		Bindable( false ),
		Browsable( false ),
		EditorBrowsable( EditorBrowsableState.Never ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )
		]
		public override string Text
		{
			get
			{
				return base.Text;
			}
			set
			{
				base.Text = value;
			}
		}

		/// <summary>
		/// Visual style of the ScrollBarCustomDraw.
		/// </summary>
		[
		Category( "Appearance - Styles" )
		]
		public ScrollBarCustomDrawStyles VisualStyle
		{
			get
			{
				return m_styles;
			}
			set
			{
				if( m_styles != value )
				{
					m_styles = value;

					OnVisualStyleChanged( EventArgs.Empty );
				}
			}
		}

		/// <summary>
		/// Gets or sets a value that indicates whether to use visual styles.
		/// </summary>
		[
		Category( "Appearance - Styles" ),
		DefaultValue( true ),
		RefreshProperties( RefreshProperties.Repaint )
		]
		public bool ThemeEnabled
		{
			get
			{
				return m_bThemeEnabled;
			}
			set
			{
				if( m_bThemeEnabled != value )
				{
					m_bThemeEnabled = value;

					OnThemeEnabledChanged( EventArgs.Empty );

					Invalidate();
				}
			}
		}

		/// <summary>
		/// Gets or sets a value that indicates whether thumb is disabled.
		/// </summary>
		[
		Category( "Behavior" ),
		DefaultValue( false ),
		RefreshProperties( RefreshProperties.Repaint ),
		]
		public bool DisableThumb
		{
			get
			{
				return m_bDisableThumb;
			}
			set
			{
				if( m_bDisableThumb != value )
				{
					m_bDisableThumb = value;
					Invalidate();
				}

			}
		}

		/// <summary>
		/// Gets or sets a value that indicates whether minimum arrow is disabled.
		/// </summary>
		[
		Category( "Behavior" ),
		DefaultValue( false ),
		RefreshProperties( RefreshProperties.Repaint ),
		]
		public bool DisableMinimumArrow
		{
			get
			{
				return m_bDisableMin;
			}
			set
			{
				if( m_bDisableMin != value )
				{
					m_bDisableMin = value;
					Invalidate();
				}
			}
		}

		/// <summary>
		/// Gets or sets a value that indicates whether maximum arrow is disabled.
		/// </summary>
		[
		Category( "Behavior" ),
		DefaultValue( false ),
		RefreshProperties( RefreshProperties.Repaint ),
		]
		public bool DisableMaximumArrow
		{
			get
			{
				return m_bDisableMax;
			}
			set
			{
				if( m_bDisableMax != value )
				{
					m_bDisableMax = value;
					Invalidate();
				}
			}
		}

		/// <summary>
		/// Collection of controls that locates above the minimum arrow.
		/// </summary>
		[
		Category( "Behavior" ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Content )
		]
		public ControlsCollection ControlsBefore
		{
			get
			{
				return m_controlsBefore;
			}
		}

		/// <summary>
		/// Collection of controls that locates under the maximum arrow.
		/// </summary>
		[
		Category( "Behavior" ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Content )
		]
		public ControlsCollection ControlsAfter
		{
			get
			{
				return m_controlsAfter;
			}
		}

		/// <summary>
		/// True - say control to keep System settings instead of user defined, otherwise False.
		/// </summary>
		[
		Category( "Behavior" ),
		Description( "True - say control to keep System settings instead of user defined, otherwise False." ),
		DefaultValue( false ),
		RefreshProperties( RefreshProperties.Repaint ),
		]
		public bool KeepSystemMetrics
		{
			get
			{
				return m_bKeepSystemmetrics;
			}
			set
			{
				if( value != m_bKeepSystemmetrics )
				{
					m_bKeepSystemmetrics = value;

					if( this.KeepSystemMetrics )
					{
						ResetToSystemMetrics();
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the renderer which draws the control.
		/// </summary>
		internal virtual IRenderer InternalRender
		{
			get
			{
				return m_renderer;
			}
			set
			{
				if( null != value && value != m_renderer )
				{
					m_renderer = value;
				}
			}
		}

		/// <summary>
		/// Returns the value that indicates whether RightToLeft is RightToLeft.Yes
		/// </summary>
		protected bool IsRtl
		{
			get
			{
				return ( RightToLeft == RightToLeft.Yes );
			}
		}

		/// <summary>
		/// Hide Controls collection from CodeDom serialization.
		/// </summary>
		[
		Browsable( false ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )
		]
		public new Control.ControlCollection Controls
		{
			get
			{
				return base.Controls;
			}
		}

        /// <summary>
        /// Gets or sets the metro color table.
        /// </summary>
        [Description("Specifies the color table for metro style")]
        public MetroColorTable MetroColorTable
        {
            get
            {
                if (m_metroColorTable == null)
                {
                    m_metroColorTable = new MetroColorTable();
                }
                return m_metroColorTable;
            }
            set
            {
                if (value != m_metroColorTable)
                {
                    m_metroColorTable = value;
                    OnVisualStyleChanged(EventArgs.Empty);
                }
            }
        }

		/// <summary>
		/// Gets or sets whether the Office color scheme should be Silver or Blue or Black.
		/// </summary>
		[
			Description( "Specifies the color scheme (Silver, Blue, Black)." )
		]
		public Office2007ColorScheme OfficeColorScheme
		{
			get
			{
				return m_OfficeColorScheme;
			}
			set
			{
				if( m_OfficeColorScheme != value )
				{
					m_OfficeColorScheme = value;

					OnVisualStyleChanged( EventArgs.Empty );
				}
			}
		}

        /// <summary>
        /// Gets or sets whether the Metro color scheme should be user defined color.
        /// </summary>
        [
            Description("Specifies the color scheme (Magenta, Purple,Teal,Orange,Pink,Red,Blue,Green,etc.).")
        ]
        public MetroColorScheme MetroColorScheme
        {
            get
            {
                return m_MetroColorScheme;
            }
            set
            {
                if (m_MetroColorScheme != value)
                {
                    m_MetroColorScheme = value;

                    OnVisualStyleChanged(EventArgs.Empty);
                }
            }
        }
        /// <summary>
        /// Gets or sets whether the Office2010 color scheme should be Silver or Blue or Black.
        /// </summary>
        [
            Description("Specifies the Office2010 color scheme (Silver, Blue, Black).")
        ]
        public Office2010ColorScheme Office2010ColorScheme
        {
            get
            {
                return m_Office2010ColorScheme;
            }
            set
            {
                if (m_Office2010ColorScheme != value)
                {
                    m_Office2010ColorScheme = value;

                    OnVisualStyleChanged(EventArgs.Empty);
                }
            }
        }

		/// <summary>
		/// Gets / sets the menu provider object that will implement the <see cref="ScrollBarCustomDraw"/>'s contextmenu. 
		/// </summary>
		/// <remarks>
		/// The ScrollBarCustomDraw control automatically initializes this property depending on the presence 
		/// of the Syncfusion Essential Tools library. If Essential Tools is available, then the menu provider 
		/// object will be an instance of the <see cref="Syncfusion.Windows.Forms.ScrollBarCustomDraw.ContextMenuProvider"/> 
		/// type. If not, the <see cref="Syncfusion.Windows.Forms.StandardMenusProvider"/> class is used for 
		/// implementing the standard .NET context menu. <p>The ScrollersFrame's automatic initialization 
		/// should suffice for most applications and you should explicitly set this property only when you 
		/// want to override the default menu provider assignment.</p></remarks>
		/// <value>A <see cref="Syncfusion.Windows.Forms.IContextMenuProvider"/> implementation; the default 
		/// is <see cref="Syncfusion.Windows.Forms.StandardMenusProvider"/>.</value>
		[
		Description( "Specifies the context menu provider for the ScrollBarCustomDraw control." ),
		Category( "Behavior" ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
		Browsable( false )
		]
		public IContextMenuProvider ContextMenuProvider
		{
			get
			{
				return m_MenuProvider;
			}

			set
			{
				if( m_MenuProvider != value )
				{
					m_MenuProvider = value;
				}
			}
		}
		/// <summary>
		/// Gets or sets value indicating whether scrollbar should be refreshed on each value change.
		/// If set to false, scrollbar is invalidated only and therefore is visually refreshed after processing all scrolling messages.
		/// </summary>
		[
		Category( "Behavior" ),
		Description( @"Indicates whether scrollbar should be refreshed on each value change. If set to false, scrollbar is invalidated only 
			and therefore is visually refreshed after processing all scrolling messages." ),
		DefaultValue( false ),
		]
		public bool RefreshOnValueChange
		{
			get
			{
				return m_bRefreshOnValueChange;
			}
			set
			{
				m_bRefreshOnValueChange = value;
			}
		}

		protected bool AttachedToListView
		{
			get
			{
				ListView lv = ( m_owner != null ) ? m_owner.AttachedTo as ListView : null;

				return ( lv != null && lv.View == View.List );
			}
		}
		#endregion

		#region Class events
		/// <summary></summary>
		[
		Category( "Property changed" )
		]
		public event EventHandler LargeChangeChanged
		{
			add
			{
				this.Events.AddHandler( EventLargeChangeChanged, value );
			}
			remove
			{
				this.Events.RemoveHandler( EventLargeChangeChanged, value );
			}
		}
		/// <summary></summary>
		[
		Category( "Property changed" )
		]
		public event EventHandler MaximumChanged
		{
			add
			{
				this.Events.AddHandler( EventMaximumChanged, value );
			}
			remove
			{
				this.Events.RemoveHandler( EventMaximumChanged, value );
			}
		}

		/// <summary></summary>
		[
		Category( "Property changed" )
		]
		public event EventHandler MinimumChanged
		{
			add
			{
				this.Events.AddHandler( EventMinimumChanged, value );
			}
			remove
			{
				this.Events.RemoveHandler( EventMinimumChanged, value );
			}
		}

		/// <summary></summary>
		[
		Category( "Property changed" )
		]
		public event EventHandler SmallChangeChanged
		{
			add
			{
				this.Events.AddHandler( EventSmallChangeChanged, value );
			}
			remove
			{
				this.Events.RemoveHandler( EventSmallChangeChanged, value );
			}
		}
		/// <summary></summary>
		[
		Category( "Property changed" )
		]
		public event EventHandler ValueChanged
		{
			add
			{
				this.Events.AddHandler( EventValueChanged, value );
			}
			remove
			{
				this.Events.RemoveHandler( EventValueChanged, value );
			}
		}

#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
    /// <summary></summary>
    [ 
    Category( "Action" ) 
    ]
    public event ScrollEventHandler Scroll
    {
      add
      {
        this.Events.AddHandler( EventScroll, value );
      }
      remove
      {
        this.Events.RemoveHandler( EventScroll, value );
      }
    }
#endif
		/// <summary></summary>
		[
		Category( "Property changed" )
		]
		public event EventHandler VisualStyleChanged
		{
			add
			{
				this.Events.AddHandler( EventVisualStyleChanged, value );
			}
			remove
			{
				this.Events.RemoveHandler( EventVisualStyleChanged, value );
			}
		}

		/// <summary></summary>
		[
		Category( "Property changed" )
		]
		public event EventHandler ColorSchemeChanged
		{
			add
			{
				this.Events.AddHandler( EventColorSchemeChanged, value );
			}
			remove
			{
				this.Events.RemoveHandler( EventColorSchemeChanged, value );
			}
		}

		/// <summary></summary>
		[
		Category( "Property changed" )
		]
		public event EventHandler ThemeEnabledChanged
		{
			add
			{
				this.Events.AddHandler( EventThemeEnabledChanged, value );
			}
			remove
			{
				this.Events.RemoveHandler( EventThemeEnabledChanged, value );
			}
		}
		#endregion

		#region class initialize\finalize methods
		/// <summary>
		/// Initializes a new instance of the <see cref="ScrollBarCustomDraw"/> class.
		/// </summary>
		/// <param name="owner">The owner.</param>
		internal ScrollBarCustomDraw( ScrollersFrame owner )
		{
			m_owner = owner;

			const ControlStyles flags = ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw |
					ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.Selectable;

			SetStyle( flags, true );

			m_renderer = new BasicRenderer( this );
			timerDelegate = new TimerCallback( timer_Tick );

			m_controlsAfter = new ControlsCollection( this, new ConfigureControlEventHandler( OnAfterControlConfigure ) );
			m_controlsBefore = new ControlsCollection( this, new ConfigureControlEventHandler( OnBeforeControlConfigure ) );

			// Create ContextMenu provider for control.
			m_MenuProvider = MenuProviderFactory.CreateContextMenuProvider();

			OnVisualStyleChanged( EventArgs.Empty );
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="ScrollBarCustomDraw"/> class.
		/// </summary>
		public ScrollBarCustomDraw() :
			this( null )
		{
		}
		/// <summary></summary>
		/// <param name="disposing"></param>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				m_renderer = null;

				if( m_timer != null )
				{
					m_timer.Dispose();
					m_timer = null;
				}

				if( m_controlsAfter != null )
				{  // cleanup collections
					DisposeControls( m_controlsAfter );

					m_controlsAfter.Clear();
					m_controlsAfter = null;
				}

				if( m_controlsBefore != null )
				{  // cleanup collections
					DisposeControls( m_controlsBefore );

					m_controlsBefore.Clear();
					m_controlsBefore = null;
				}

				m_owner = null;
			}

			base.Dispose( disposing );
		}
		#endregion

		#region Class codedom serialization
		/// <summary></summary>
		/// <returns></returns>
		protected bool ShouldSerializeControlsBefore()
		{
			return ( this.ControlsBefore != null ? ( this.ControlsBefore.Count > 0 ) : false );
		}
		/// <summary></summary>
		/// <returns></returns>
		protected bool ShouldSerializeControlsAfter()
		{
			return ( this.ControlsAfter != null ? (this.ControlsAfter.Count > 0) : false );
		}
		#endregion

		#region Class overrides
		/// <summary>
		/// Initialize the context menu.
		/// </summary>
		/// <param name="menu"></param>
		private void InitializeMenu( IContextMenuProvider menu )
		{
			bool IsVerticalScrollBar = ( this is VScrollBarCustomDraw );

			menu.AddContextMenuItem(String.Empty, SR.GetString(SR.ScrollHere, this), new EventHandler(OnScrollHereClick));
			menu.AddContextMenuItem(String.Empty, IsVerticalScrollBar ? SR.GetString(SR.Top, this) : SR.GetString(SR.LeftEdge, this), new EventHandler(OnMinimumClick));
			menu.SetContextMenuItemSeparator(IsVerticalScrollBar ? SR.GetString(SR.Top, this) : SR.GetString(SR.LeftEdge, this), true);
			menu.AddContextMenuItem(String.Empty, IsVerticalScrollBar ? SR.GetString(SR.Bottom, this) : SR.GetString(SR.RightEdge, this), new EventHandler(OnMaximumClick));
			menu.AddContextMenuItem(String.Empty, IsVerticalScrollBar ? SR.GetString(SR.PageUp, this) : SR.GetString(SR.PageLeft, this), new EventHandler(OnPageUpLeftClick));
			menu.SetContextMenuItemSeparator(IsVerticalScrollBar ? SR.GetString(SR.PageUp, this) : SR.GetString(SR.PageLeft, this), true);
			menu.AddContextMenuItem(String.Empty, IsVerticalScrollBar ? SR.GetString(SR.PageDown, this) : SR.GetString(SR.PageRight, this), new EventHandler(OnPageDownRightClick));
			menu.AddContextMenuItem(String.Empty, IsVerticalScrollBar ? SR.GetString(SR.ScrollUp, this) : SR.GetString(SR.ScrollLeft, this), new EventHandler(OnScrollUpLeftClick));
			menu.SetContextMenuItemSeparator(IsVerticalScrollBar ? SR.GetString(SR.ScrollUp, this) : SR.GetString(SR.ScrollLeft, this), true);
			menu.AddContextMenuItem(String.Empty, IsVerticalScrollBar ? SR.GetString(SR.ScrollDown, this) : SR.GetString(SR.ScrollRight, this), new EventHandler(OnScrollDownRightClick));
		}

		/// <summary></summary>
		/// <param name="control"></param>
		protected virtual void OnBeforeControlConfigure( Control control )
		{
			control.Dock = GetBeforeControlsDockStyle();
		}

		/// <summary></summary>
		/// <param name="control"></param>
		protected virtual void OnAfterControlConfigure( Control control )
		{
			control.Dock = GetAfterControlsDockStyle();
		}

		/// <override/>
		/// <summary>Override. Force recalculation of Scroller elements.</summary>
		/// <param name="levent">Layout arguments.</param>
		/// <remarks>To force Layout logic call <see cref="PerformLayout"/>.</remarks>
		protected override void OnLayout( LayoutEventArgs levent )
		{
			Layout();
			base.OnLayout( levent );
		}

		/// <summary>
		/// Forces the laying out of combobox elements.
		/// </summary>
		/// <remarks>
		/// Advanced method. You do not have to call this directly.
		/// </remarks>
		public new virtual void Layout()
		{
			if( !m_IsSetControlsVisiblity )
			{
				ValidateControlsVisibility();
			}
			RecalculateScroll();
			Invalidate();
		}

		/// <summary>If outside code will work with <see cref="Controls"/> collection instead 
		/// of our collections <see cref="ControlsBefore"/> and <see cref="ControlsAfter"/>, 
		/// then this method will help us in synchronization.</summary>
		/// <param name="e"></param>
		protected override void OnControlAdded( ControlEventArgs e )
		{
			if( !m_IsSetControlsVisiblity )
			{
				ValidateControlsVisibility();
			}

			base.OnControlAdded( e );
		}

		/// <summary>If outside code will work with <see cref="Controls"/> collection instead 
		/// of our collections <see cref="ControlsBefore"/> and <see cref="ControlsAfter"/>, 
		/// then this method will help us in synchronization.</summary>
		/// <param name="e"></param>
		protected override void OnControlRemoved( ControlEventArgs e )
		{
			if( !m_IsSetControlsVisiblity )
			{
				// if user remove control check if it was not other wrapper collections control
				if( m_controlsAfter != null )
				{
					m_controlsAfter.Remove( e.Control );
				}

				if( m_controlsBefore != null )
				{
					m_controlsBefore.Remove( e.Control );
				}
				ValidateControlsVisibility();
			}

			base.OnControlRemoved( e );
		}

		/// <summary>
		/// Overridden. See <see cref="M:System.Windows.Forms.Control.OnPaint"/>.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPaint( PaintEventArgs e )
		{
			base.OnPaint( e );

			Graphics g = e.Graphics;

			ButtonState minArea = ( !this.Enabled || this.DisableThumb ) ? ButtonState.Inactive :
				( m_pressedZone == PressedZone.ThumbLeftZone &&
				m_movedZone == MovedZone.ThumbLeftZone ) ? ButtonState.Pushed : ButtonState.Normal;

			ButtonState maxArea = ( !this.Enabled || this.DisableThumb ) ? ButtonState.Inactive :
				( m_pressedZone == PressedZone.ThumbRightZone &&
				m_movedZone == MovedZone.ThumbRightZone ) ? ButtonState.Pushed : ButtonState.Normal;

			InternalRender.DrawBackground( g, m_rects[(int)PressedZone.ThumbLeftZone], minArea );
			InternalRender.DrawBackground( g, m_rects[(int)PressedZone.ThumbRightZone], maxArea );

			bool bOffice2007BehaviourForArrow = false;
			bool bMinArrow = false;
			bool bMaxArrow = false;

			if( this.VisualStyle == ScrollBarCustomDrawStyles.Office2007 || this.VisualStyle == ScrollBarCustomDrawStyles.Office2007Generic )
			{
				bOffice2007BehaviourForArrow = ( m_selectedZone == PressedZone.Thumb ||
					m_selectedZone == PressedZone.ThumbLeftZone ||
					m_selectedZone == PressedZone.ThumbRightZone );

				bMinArrow = bOffice2007BehaviourForArrow || m_selectedZone == PressedZone.MaxButton;
				bMaxArrow = bOffice2007BehaviourForArrow || m_selectedZone == PressedZone.MinButton;
			}

			// draw arrows
			ButtonState minArrow = ( !this.Enabled || this.DisableMinimumArrow || bMinArrow ) ? ButtonState.Inactive :
				( ( m_pressedZone == PressedZone.MinButton && m_movedZone == MovedZone.MinButton ) ? ButtonState.Pushed :
				( ( m_selectedZone == PressedZone.MinButton ) ? ButtonState.Checked : ButtonState.Normal ) );

			InternalRender.DrawArrowButton( g, m_rects[(int)PressedZone.MinButton], GetMinButton(), minArrow );

			ButtonState maxArrow = ( !this.Enabled || this.DisableMaximumArrow || bMaxArrow ) ? ButtonState.Inactive :
				( ( m_pressedZone == PressedZone.MaxButton && m_movedZone == MovedZone.MaxButton ) ? ButtonState.Pushed :
				( ( m_selectedZone == PressedZone.MaxButton ) ? ButtonState.Checked : ButtonState.Normal ) );

			InternalRender.DrawArrowButton( g, m_rects[(int)PressedZone.MaxButton], GetMaxButton(), maxArrow );

			m_bUpdateStates = !( minArea == ButtonState.Normal && maxArea == ButtonState.Normal
				&& minArrow == ButtonState.Normal && maxArrow == ButtonState.Normal );

			// draw thumb only when control is enabled
			ButtonState thumbState = ( !this.Enabled || this.DisableThumb ) ? ButtonState.Inactive :
				( m_pressedZone == PressedZone.Thumb ) ? ButtonState.Pushed :
				( ( m_selectedZone == PressedZone.Thumb ) ? ButtonState.Checked : ButtonState.Normal );

			InternalRender.DrawThumb( g, m_rects[(int)PressedZone.Thumb], thumbState );
		}

		/// <summary>
		/// Occurs when control size changed.
		/// </summary>
		/// <param name="e">The event data.</param>
		protected override void OnSizeChanged( EventArgs e )
		{
			base.OnSizeChanged( e );

			RecalculateScroll();

			if( this.KeepSystemMetrics )
			{
				ResetToSystemMetrics();
			}
		}

		/// <summary>
		/// Reset control Width or Heigh to system settings
		/// </summary>
		protected virtual void ResetToSystemMetrics()
		{
			// do nothing here
		}

		/// <summary>
		/// Raises the OnLargeChangeChanged event.
		/// </summary>
		/// <param name="e">The event data.</param>
		protected virtual void OnLargeChangeChanged( EventArgs e )
		{
			EventHandler handler = this.Events[EventLargeChangeChanged] as EventHandler;

			if( null != handler )
			{
				handler( this, e );
			}
		}

		/// <summary>
		/// Raises the OnSmallChangeChanged event.
		/// </summary>
		/// <param name="e">The event data.</param>
		protected virtual void OnSmallChangeChanged( EventArgs e )
		{
			EventHandler handler = this.Events[EventSmallChangeChanged] as EventHandler;

			if( null != handler )
			{
				handler( this, e );
			}
		}

		/// <summary>
		/// Raises the OnMaximumChanged event.
		/// </summary>
		/// <param name="e">The event data.</param>
		protected virtual void OnMaximumChanged( EventArgs e )
		{
			EventHandler handler = this.Events[EventMaximumChanged] as EventHandler;

			if( null != handler )
			{
				handler( this, e );
			}
		}

		/// <summary>
		/// Raises the OnMinimumChanged event.
		/// </summary>
		/// <param name="e">The event data.</param>
		protected virtual void OnMinimumChanged( EventArgs e )
		{
			EventHandler handler = this.Events[EventMinimumChanged] as EventHandler;

			if( null != handler )
			{
				handler( this, e );
			}
		}

		/// <summary>
		/// Raises the OnVisualStyleChanged event.
		/// </summary>
		/// <param name="e">The event data.</param>
		internal virtual void OnVisualStyleChanged( EventArgs e )
		{
			switch( this.VisualStyle )
			{
				case ScrollBarCustomDrawStyles.Classic:
				this.InternalRender = new ClassicRenderer( this );
				break;

				case ScrollBarCustomDrawStyles.WindowsXP:
				this.InternalRender = new WindowsXPRenderer( this );
				break;

				case ScrollBarCustomDrawStyles.Office2007:
				case ScrollBarCustomDrawStyles.Office2007Generic:
				this.InternalRender =
						new Office2007Renderer( this, ColorTableOffice2007.GetColorTable( this.VisualStyle, this.OfficeColorScheme ) );
				break;

                case ScrollBarCustomDrawStyles.Office2010:
                    this.InternalRender = new Office2010Renderer(this, ColorTableOffice2010.GetColorTable(this.VisualStyle, this.Office2010ColorScheme));
                    break;
                case ScrollBarCustomDrawStyles.Metro:
                    this.InternalRender = new MetroRenderer(this, this.MetroColorTable);
                    break;
            }
			this.Invalidate();

			EventHandler handler = this.Events[EventVisualStyleChanged] as EventHandler;

			if( null != handler )
			{
				handler( this, e );
			}
		}

		/// <summary>
		/// Raises the OnMinimumChanged event.
		/// </summary>
		/// <param name="e">The event data.</param>
		protected virtual void OnThemeEnabledChanged( EventArgs e )
		{
			EventHandler handler = this.Events[EventThemeEnabledChanged] as EventHandler;

			if( null != handler )
			{
				handler( this, e );
			}
		}

		/// <summary>
		/// Raises the OnValueChanged event.
		/// </summary>
		/// <param name="e">The event data.</param>
		protected virtual void OnValueChanged( EventArgs e )
		{
			EventHandler handler = this.Events[EventValueChanged] as EventHandler;

			if( null != handler )
			{
				handler( this, e );
			}
		}

		/// <summary>
		/// Used for validating visibility of the ControlsAfter and ControlsBefore
		/// </summary>
		/// <returns></returns>
		protected virtual void ValidateControlsVisibility()
		{
		}
#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
    /// <summary></summary>
    /// <param name="e"></param>
    protected virtual void OnScroll( ScrollEventArgs e )
    {
      ScrollEventHandler handler = this.Events[ EventScroll ] as ScrollEventHandler;

      if( null != handler )
      {
        handler( this, e );
      }
    }
#endif
		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.OnMouseDown"/>.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseDown( MouseEventArgs e )
		{
			if( e.Button == MouseButtons.Left )
			{
				// Calculating main helper variables
				m_pressedZone = HitTest( e.X, e.Y );
				DefineMovedZone( e.X, e.Y );
				int factor = 0;

				switch( m_pressedZone )
				{
					case PressedZone.MinButton:
					case PressedZone.MaxButton:
					ScrollEventType type = ( m_pressedZone == PressedZone.MaxButton )
							? ScrollEventType.SmallIncrement
							: ScrollEventType.SmallDecrement;

					factor = ( m_pressedZone == PressedZone.MaxButton ) ? 1 : -1;

					if( IsRtl )
					{
						factor = -factor;
					}

					Value += factor * m_smallChange;
					// Raise event with old value
					OnScroll( new ScrollEventArgs( type, m_value ) );

					// Run timer
					m_timer = new System.Threading.Timer( timerDelegate, null, 500, 50 );
					break;

					case PressedZone.ThumbLeftZone:
					case PressedZone.ThumbRightZone:
					ScrollEventType typeZ = ( m_pressedZone == PressedZone.ThumbLeftZone )
							? ScrollEventType.LargeDecrement
							: ScrollEventType.LargeIncrement;

					factor = ( m_pressedZone == PressedZone.ThumbRightZone ) ? 1 : -1;

					if( IsRtl )
					{
						factor *= -1;
					}

					Value += factor * m_largeChange;

					// Raise event with old value
					OnScroll( new ScrollEventArgs( typeZ, m_value ) );

					// Run timer.
					m_timer = new System.Threading.Timer( timerDelegate, null, 500, 50 );
					break;

					case PressedZone.Thumb:
					CalculatedDelta( e.X, e.Y );
					break;
				}
			}

			base.OnMouseDown( e );
		}

		/// <summary>
		/// Redraws scroll control when RightToLeft is changed.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnRightToLeftChanged( EventArgs e )
		{
			base.OnRightToLeftChanged( e );

			SuspendLayout();

			foreach( Control control in this.ControlsAfter )
			{
				control.Dock = GetAfterControlsDockStyle();
			}

			foreach( Control control in this.ControlsBefore )
			{
				control.Dock = GetBeforeControlsDockStyle();
			}

			ResumeLayout( true );

			RecalculateScroll();

		}

		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.OnMouseUp"/>.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseUp( MouseEventArgs e )
		{
			if( e.Button == MouseButtons.Left )
			{
                if (m_pressedZone == PressedZone.Thumb || m_pressedZone == PressedZone.MaxButton || m_pressedZone == PressedZone.MinButton
                    || m_pressedZone == PressedZone.ThumbLeftZone || m_pressedZone == PressedZone.ThumbRightZone)
                {
                    if (!this.IsRtl || !this.AttachedToListView)
                    {
                        // Invalidate splitter with recalculated all bounds. Used for align thumb.
                        SetInnerValue(m_value);

                        OnScroll(new ScrollEventArgs(ScrollEventType.EndScroll, m_value));
                        Invalidate();
                    }
                }

				m_pressedZone = PressedZone.None;

				// free unnecessary object
				if( null != m_timer )
				{
					m_timer.Dispose();
				}

				Invalidate();
			}

			// Show context menu if needed.
			Rectangle rcBounds = new Rectangle( Point.Empty, this.Size );
			if( e.Button == MouseButtons.Right && rcBounds.Contains( new Point( e.X, e.Y ) ) )
			{
				m_ptLocation = new Point( e.X, e.Y );
				ShowContextMenu( this );
			}

			base.OnMouseUp( e );
		}

		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.OnMouseMove"/>.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseMove( MouseEventArgs e )
		{
			base.OnMouseMove( e );

			if( m_pressedZone == PressedZone.Thumb && e.Button == MouseButtons.Left )
			{
				OnMovedPositionChanged( e.X, e.Y );

				int newValue = PointToValue( m_rects[(int)PressedZone.Thumb].X, m_rects[(int)PressedZone.Thumb].Y );

				if( m_value != newValue )
				{
					m_value = newValue;

					if( this.IsRtl && this.AttachedToListView )
					{
						newValue = m_min + m_max - newValue;
						OnScroll( new ScrollEventArgs( ScrollEventType.ThumbPosition, newValue ) );
					}
					else
					{
						OnScroll( new ScrollEventArgs( ScrollEventType.ThumbPosition, m_value ) );
						OnValueChanged( EventArgs.Empty );
					}
				}

				Invalidate();
			}
			else if( m_pressedZone == PressedZone.ThumbLeftZone || m_pressedZone == PressedZone.ThumbRightZone )
			{
				DefineMovedZone( e.X, e.Y );
			}
			else if( m_pressedZone == PressedZone.None )
			{
				m_selectedZone = HitTest( e.X, e.Y );

				if( m_cachedSelectedZone == PressedZone.None )
				{
					m_cachedSelectedZone = m_selectedZone;
				}
				else
				{
					Invalidate( m_rects[(int)m_cachedSelectedZone] );
				}

				m_cachedSelectedZone = m_selectedZone;
			}

			if( this.VisualStyle == ScrollBarCustomDrawStyles.Office2007 || this.VisualStyle == ScrollBarCustomDrawStyles.Office2007Generic )
			{
				Invalidate( m_rects[(int)PressedZone.MinButton] );
				Invalidate( m_rects[(int)PressedZone.MaxButton] );
			}
		}

		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.OnMouseLeave"/>.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseLeave( EventArgs e )
		{
			if( m_cachedSelectedZone != PressedZone.None )
			{
				int index = (int)m_selectedZone;
				if( index > 0 && index < m_rects.Length )
				{
					Invalidate( m_rects[(int)m_selectedZone] );
				}
			}

			if( this.VisualStyle == ScrollBarCustomDrawStyles.Office2007 || this.VisualStyle == ScrollBarCustomDrawStyles.Office2007Generic )
			{
				Invalidate( m_rects[(int)PressedZone.MinButton] );
				Invalidate( m_rects[(int)PressedZone.MaxButton] );
			}

			m_selectedZone = PressedZone.None;
			base.OnMouseLeave( e );
		}

		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.OnSystemColorsChanged"/>.
		/// </summary>
		/// <param name="e"/>
		protected override void OnSystemColorsChanged( EventArgs e )
		{
			switch( VisualStyle )
			{
				case ScrollBarCustomDrawStyles.WindowsXP:
				InternalRender = new WindowsXPRenderer( this );
				( InternalRender as WindowsXPRenderer ).SetColorScheme();
				break;
			}

			base.OnSystemColorsChanged( e );
		}

		/// <summary>
		/// Occurs when mouse down and cursor change position.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		protected virtual void OnMovedPositionChanged( int x, int y )
		{
		}

		/// <summary>
		/// Gets min button. Possible variants: Left or Down.
		/// </summary>
		/// <returns></returns>
		protected virtual ScrollButton GetMinButton()
		{
			return ScrollButton.Min;
		}

		/// <summary>
		/// Gets max button. Possible variants: Right or Up.
		/// </summary>
		/// <returns></returns>
		protected virtual ScrollButton GetMaxButton()
		{
			return ScrollButton.Max;
		}

		/// <summary>
		/// Defines pressed zone.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		protected virtual PressedZone HitTest( int x, int y )
		{
			for( int i = 0, len = m_rects.Length; i < len; i++ )
			{
				if( m_rects[i].Contains( x, y ) )
				{
					return (PressedZone)i;
				}
			}

			return PressedZone.None;
		}

		/// <summary>
		/// Recalculates bounds of the ScrollBarCustomDraw.
		/// </summary>
		protected internal virtual void RecalculateScroll()
		{
			RecalculateArrow();
			RecalculateThumb();
		}

		/// <summary>
		/// Recalculates bounds of the arrow buttons.
		/// </summary>
		protected virtual void RecalculateArrow()
		{
			// must be implemented in inheritor class
		}

		/// <summary>
		/// Recalculates bounds of the thumb.
		/// </summary>
		protected virtual void RecalculateThumb()
		{
			// must be implemented in inheritor class
		}

		/// <summary>
		/// If scroll contain all controls than controls is visible, in other case value is false.
		/// </summary>
		/// <returns></returns>
		protected virtual bool IsControlsVisible()
		{
			return true;
			// must ne overrided in inheritor
		}

		/// <summary>
		/// Gets DockStyle of afterControls.
		/// </summary>
		/// <returns></returns>
		protected virtual DockStyle GetAfterControlsDockStyle()
		{
			return DockStyle.Right;
		}

		/// <summary>
		/// Gets DockStyle of beforeControls.
		/// </summary>
		/// <returns></returns>
		protected virtual DockStyle GetBeforeControlsDockStyle()
		{
			return DockStyle.Left;
		}

		/// <summary>
		/// used for calculates thumb offset.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		protected virtual void CalculatedDelta( int x, int y )
		{
			// must be overrided by inheritors
		}
		#endregion

		#region Class public methods
		/// <summary></summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		[EditorBrowsable( EditorBrowsableState.Never )]
		public virtual int PointToValue( int x, int y )
		{
			return 0;
		}
		/// <summary></summary>
		/// <param name="value"></param>
		/// <returns></returns>
		[Obsolete(), EditorBrowsable( EditorBrowsableState.Never )]
		public virtual Point ValueToPoint( int value )
		{
			return Point.Empty;
		}
		/// <summary>
		/// Utility API that open to user opportunity to destroy handle in runtime.
		/// Very usefull for runtime resource cleanup.
		/// </summary>
		public void DropHandle()
		{
			this.DestroyHandle();
		}
		/// <summary>
		/// Sets small change if value is less than large change; otherwise sets small change to large change.
		/// </summary>
		/// <param name="value">Value to set.</param>
		public void SetSafeSmallChange( int value )
		{
			if( m_smallChange != value )
			{
				if( value > m_largeChange )
				{
					m_smallChange = m_largeChange;
				}
				else
				{
					m_smallChange = value;
				}

				OnSmallChangeChanged( EventArgs.Empty );

				Invalidate();
			}
		}
		#endregion

		#region class event handlers
		/// <summary></summary>
		/// <param name="sender"></param>
		private void timer_Tick( object sender )
		{
			EventHandler handler = null;

			switch( m_pressedZone )
			{
				case PressedZone.ThumbLeftZone:
				handler = new EventHandler( OnTickForThumbMin );
				break;

				case PressedZone.ThumbRightZone:
				handler = new EventHandler( OnTickForThumbMaxZone );
				break;

				case PressedZone.MinButton:
				handler = new EventHandler( OnTickForMinButton );
				break;

				case PressedZone.MaxButton:
				handler = new EventHandler( OnTickForMaxButton );
				break;
			}

			// for Async calls from Different threads required BeginInvoke instead of direct call
			if( handler != null )
			{
				if( this.InvokeRequired )
				{
					this.BeginInvoke( handler, new object[] { this, EventArgs.Empty } );
				}
				else
				{
					handler( this, EventArgs.Empty );
				}
			}
		}
		/// <summary></summary>
		/// <param name="e"></param>
		/// <param name="sender"/>
		private void OnTickForThumbMin( object sender, EventArgs e )
		{
			DefineMovedZone( PointToClient( Cursor.Position ) );

			if( m_movedZone == MovedZone.ThumbLeftZone )
			{
				int factor = 1;
				if( IsRtl )
				{
					factor *= -1;
				}
				Value -= factor * m_largeChange;
				OnScroll( new ScrollEventArgs( ScrollEventType.LargeDecrement, m_value ) );
			}
			else
			{
				if( m_bUpdateStates )
				{
					Invalidate();
				}
			}
		}
		/// <summary></summary>
		/// <param name="e"></param>
		/// <param name="sender"/>
		private void OnTickForThumbMaxZone( object sender, EventArgs e )
		{
			DefineMovedZone( PointToClient( Cursor.Position ) );

			if( m_movedZone == MovedZone.ThumbRightZone )
			{
				int factor = 1;
				if( IsRtl )
				{
					factor *= -1;
				}
				Value += factor * m_largeChange;
				OnScroll( new ScrollEventArgs( ScrollEventType.LargeIncrement, m_value ) );
			}
			else
			{
				if( m_bUpdateStates )
				{
					Invalidate();
				}
			}
		}
		/// <summary></summary>
		/// <param name="e"></param>
		/// <param name="sender"/>
		private void OnTickForMinButton( object sender, EventArgs e )
		{
			DefineMovedZone( PointToClient( Cursor.Position ) );

			if( m_movedZone == MovedZone.MinButton )
			{
				int factor = 1;
				if( IsRtl )
				{
					factor *= -1;
				}
				Value -= factor * m_smallChange;
				OnScroll( new ScrollEventArgs( ScrollEventType.SmallDecrement, m_value ) );
			}
			else
			{
				if( m_bUpdateStates )
				{
					Invalidate();
				}
			}
		}
		/// <summary></summary>
		/// <param name="e"></param>
		/// <param name="sender"/>
		private void OnTickForMaxButton( object sender, EventArgs e )
		{
			DefineMovedZone( PointToClient( Cursor.Position ) );

			if( m_movedZone == MovedZone.MaxButton )
			{
				int factor = 1;
				if( IsRtl )
				{
					factor *= -1;
				}
				Value += factor * m_smallChange;
				OnScroll( new ScrollEventArgs( ScrollEventType.SmallIncrement, m_value ) );
			}
			else
			{
				if( m_bUpdateStates )
				{
					Invalidate();
				}
			}
		}
		/// <summary></summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void OnScrollHereClick( object sender, EventArgs e )
		{
		}
		/// <summary></summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void OnMinimumClick( object sender, EventArgs e )
		{
			this.Value = this.Minimum;
			OnScroll( new ScrollEventArgs( ScrollEventType.First, m_value ) );
		}
		/// <summary></summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void OnMaximumClick( object sender, EventArgs e )
		{
			this.Value = this.Maximum;
			OnScroll( new ScrollEventArgs( ScrollEventType.First, m_value ) );
		}
		/// <summary></summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void OnPageUpLeftClick( object sender, EventArgs e )
		{
			OnScroll( new ScrollEventArgs( ScrollEventType.LargeDecrement, m_value ) );
		}
		/// <summary></summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void OnPageDownRightClick( object sender, EventArgs e )
		{
			OnScroll( new ScrollEventArgs( ScrollEventType.LargeIncrement, m_value ) );
		}
		/// <summary></summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void OnScrollUpLeftClick( object sender, EventArgs e )
		{
			Value -= m_smallChange;
			OnScroll( new ScrollEventArgs( ScrollEventType.SmallDecrement, m_value ) );
		}
		/// <summary></summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void OnScrollDownRightClick( object sender, EventArgs e )
		{
			Value += m_smallChange;
			OnScroll( new ScrollEventArgs( ScrollEventType.SmallIncrement, m_value ) );
		}
		/// <summary></summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void OnBeforeControlConfigure( object sender, ConfigureControlEventArgs e )
		{
			if( e != null && e.Control != null )
			{
				e.Control.Dock = GetBeforeControlsDockStyle();
			}
		}
		/// <summary></summary>
		/// <param name="e"></param>
		/// <param name="sender"></param>
		protected virtual void OnAfterControlConfigure( object sender, ConfigureControlEventArgs e )
		{
			if( e != null && e.Control != null )
			{
				e.Control.Dock = GetAfterControlsDockStyle();
			}
		}
		#endregion

		#region class helper methods
		/// <summary>
		/// Method accumulate width and height of the controls in 
		/// specified collection.
		/// </summary>
		/// <summary>Method accumulate width and height of the controls in 
		/// specified collection.</summary>
		/// <param name="collection">collection of controls.</param>
		/// <returns>Accumulated values.</returns>
		protected Size Accumulate( ControlsCollection collection )
		{
			Size szArea = new Size();

			if( collection != null )
			{
				foreach( Control control in collection )
				{
					if( DesignMode || control.Visible )
					{
						szArea.Width += control.Width;
						szArea.Height += control.Height;
					}
				}
			}

			return szArea;
		}
		/// <summary>
		/// Excludes/includes controls in <see cref="Controls"/> collection.
		/// </summary>
		/// <param name="hide">If true - excludes, else includes controls in <see cref="Controls"/> collection.</param>
		protected void SetControlsLayoutVisibility( bool hide )
		{
			if( ControlsAfter != null )
			{
				ControlsAfter.HideControls( hide );
			}
			if( ControlsAfter != null )
			{
				ControlsBefore.HideControls( hide );
			}
		}
		/// <summary></summary>
		/// <param name="p"></param>
		private void DefineMovedZone( Point p )
		{
			DefineMovedZone( p.X, p.Y );
		}
		/// <summary></summary>
		/// <param name="x">X coordinate of mouse.</param>
		/// <param name="y">Y coordinate of mouse.</param>
		private void DefineMovedZone( int x, int y )
		{
			m_movedZone = MovedZone.Out;

			for( int i = 0, len = m_rects.Length; i < len; i++ )
			{
				if( m_rects[i].Contains( x, y ) )
				{
					m_movedZone = (MovedZone)i;
				}
			}
		}
		/// <summary>
		/// Sets value, but OnValueChanged don't raise. 
		/// </summary>
		/// <param name="value"></param>
		private void SetInnerValue( int value )
		{
			if( value <= m_min )
			{
				m_value = m_min;
			}
			else
			{
				m_value = ( value <= m_max - m_largeChange) ? value : m_max - m_largeChange + 1;
			}

            RecalculateScroll();
		}
		/// <summary>
		/// Dispose controls of ScrollBarCustomDraw.
		/// </summary>
		/// <param name="controls">Collection of controls to be disposed.</param>
		private void DisposeControls( ControlsCollection controls )
		{
			int count = controls.Count;
			for( int i = 0; i < count; i++ )
			{
				controls[0].Dispose();
			}
		}
		#endregion

		#region Implementation
		/// <summary>
		/// Creates and displays the context menu for the control.
		/// </summary>
		/// <param name="scrollBar"/>
		private void ShowContextMenu( ScrollBarCustomDraw scrollBar )
		{
			IContextMenuProvider menu = this.ContextMenuProvider;
			menu.InitializeContextMenu();

			if( XPThemes.IsThemedOS && XPThemes.IsThemeActive && VisualStyle == ScrollBarCustomDrawStyles.Office2007 )
			{
				menu.SetVisualStyle( Syncfusion.Windows.Forms.VisualStyle.Office2007 );
			}

			InitializeMenu( menu );
			menu.ShowContextMenu( scrollBar, this.PointToClient( Cursor.Position ) );
		}
		#endregion
	}

	#region Scrollers designers
	/// <summary>Design time helper class. Do not allow resizing of scrollers
	/// when set KeepSystmeMetrics property to TRUE value.</summary>
	public class ScrollBarCustomDrawDesigner: ParentControlDesigner
	{
		/// <summary>Typed version of control reference extracting.</summary>
		protected new ScrollBarCustomDraw Control
		{
			get
			{
				if( base.Component is ScrollBarCustomDraw )
				{
					return base.Component as ScrollBarCustomDraw;
				}
				else if( base.Control is ScrollBarCustomDraw )
				{
					return base.Control as ScrollBarCustomDraw;
				}

				return null;
			}
		}

		/// <summary>Override selection rule only in case of KeepSystemMetrics value set to True.</summary>
		public override SelectionRules SelectionRules
		{
			get
			{
				if( this.Control != null )
				{
					if( this.Control.KeepSystemMetrics )
					{
						if( this.Control is VScrollBarCustomDraw )
						{
							return base.SelectionRules & ~( SelectionRules.RightSizeable | SelectionRules.LeftSizeable );
						}
						else if( this.Control is HScrollBarCustomDraw )
						{
							return base.SelectionRules & ~( SelectionRules.TopSizeable | SelectionRules.BottomSizeable );
						}
					}
				}

				return base.SelectionRules;
			}
		}

	}
	#endregion
}