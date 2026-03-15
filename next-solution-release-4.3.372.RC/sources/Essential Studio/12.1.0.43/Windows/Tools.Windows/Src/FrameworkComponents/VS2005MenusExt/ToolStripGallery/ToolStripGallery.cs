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

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using Syncfusion.Windows.Forms.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.Design;
using Syncfusion.Windows.Forms.Tools.Win32API;

namespace Syncfusion.Windows.Forms.Tools
{
	#region *** ToolStripGallery
	/// <summary>
	/// 
	/// </summary>
	[System.Windows.Forms.Design.ToolStripItemDesignerAvailability( System.Windows.Forms.Design.ToolStripItemDesignerAvailability.ToolStrip )]
	[ToolboxBitmap(typeof(ToolStripGallery), "ToolboxIcons.Gallery.bmp")]
	public class ToolStripGallery : ToolStripItem, INativeMessageFilter
	{
		#region Constants
		/// <summary>
		/// 
		/// </summary>
		private static readonly Size ITEM_SIZE = new Size( 70, 45 );
		/// <summary>
		/// 
		/// </summary>
		private static readonly Size ITEM_IMAGE_SIZE = new Size( 20, 20 );
		/// <summary>
		/// 
		/// </summary>
		private static readonly Padding ITEM_PADDING = new Padding( 4 );
		/// <summary>
		/// 
		/// </summary>
		private static readonly Padding ITEM_MARGIN = new Padding( 1 );
		/// <summary>
		/// 
		/// </summary>
		internal static readonly Padding CAPTION_MARGIN = new Padding( 2, 1, 2, 1 );
		/// <summary>
		/// 
		/// </summary>
		private const int SCROLLER_WIDTH = 15;
		/// <summary>
		/// 
		/// </summary>
		private const int TIMER_INT = 100;
		/// <summary>
		/// 
		/// </summary>
		private static readonly Size DROPDOWN_MIN_SIZE = new Size( 120, 120 );
        private bool m_showtooltipText =true;

		/// <summary>
		/// Default dimensions for one row.
		/// </summary>
		private static readonly Size DIMENSIONS = new Size(3, 1);

        ToolStripGalleryItem item;
        SuperToolTip toolTip = new SuperToolTip();
        ToolTipInfo info = new ToolTipInfo();

		#endregion

		#region Fields
		/// <summary>
		/// 
		/// </summary>
		private ObservableList<ToolStripGalleryItem> m_items;
		/// <summary>
		/// 
		/// </summary>
		private List<ToolStripGalleryItemLayoutInfo> m_itemsInfo;
		/// <summary>
		/// 
		/// </summary>
		private Size m_itemSize = ITEM_SIZE;
		/// <summary>
		/// 
		/// </summary>
		private Size m_itemImageSize = ITEM_IMAGE_SIZE;
		/// <summary>
		/// 
		/// </summary>
		private Padding m_itemPadding = ITEM_PADDING;
		/// <summary>
		/// 
		/// </summary>
		private Padding m_itemMargin = ITEM_MARGIN;
		/// <summary>
		/// 
		/// </summary>
		private ToolStripItemDisplayStyle m_itemDisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
		/// <summary>
		/// 
		/// </summary>
		private TextImageRelation m_textImageRelation = TextImageRelation.ImageAboveText;
		/// <summary>
		/// 
		/// </summary>
		private Size m_dimensions = DIMENSIONS;
		/// <summary>
		/// 
		/// </summary>
		private Size m_dropdownDimensions = Size.Empty;
		/// <summary>
		/// Currently selected item.
		/// </summary>
		private ToolStripGalleryItem m_selectedItem;
		/// <summary>
		/// Currently selected gallery item.
		/// </summary>
		private ToolStripGalleryItem m_galleryItem;
		/// <summary>
		/// Currently disabled item.
		/// </summary>
        private ToolStripGalleryItem m_disabledItem;
		/// <summary>
		/// 
		/// </summary>
		private ToolStripGalleryItem m_pressedItem;
		/// <summary>
		/// Currently checked item.
		/// </summary>
		private ToolStripGalleryItem m_checkedItem;
		/// <summary>
		/// 
		/// </summary>
		private bool m_bPressed = false;
		/// <summary>
		/// Indicates if item must be checked after user clicks on it.
		/// </summary>
		private bool m_bCheckOnClick = false;
		/// <summary>
		/// 
		/// </summary>
		private bool m_bShowCaption = false;
		/// <summary>
		/// 
		/// </summary>
		private string m_captionText = string.Empty;
		/// <summary>
		/// 
		/// </summary>
		private int m_captionHeight = -1;
		/// <summary>
		/// 
		/// </summary>
		private ToolStripGalleryScrollerType m_scrollerType = ToolStripGalleryScrollerType.None;
		/// <summary>
		/// 
		/// </summary>
		private ScrollButton m_scrollUpButton;
		/// <summary>
		/// 
		/// </summary>
		private ScrollButton m_scrollDownButton;
		/// <summary>
		/// 
		/// </summary>
		private ScrollButton m_scrollDropdownButton;
		/// <summary>
		/// Offset of scroll position. The upper position is 0.
		/// </summary>
		private int m_scrollOffset;
		/// <summary>
		/// Current real height of layouted component without scrollers.
		/// </summary>
		private int m_realHeight;
		/// <summary>
		/// 
		/// </summary>
		private ScrollButton m_scroller;
		/// <summary>
		/// 
		/// </summary>
		private bool m_bScrollerMoving;
		/// <summary>
		/// Point where scroller was pressed.
		/// </summary>
		private Point m_scrollerPrevPoint;
		/// <summary>
		/// Scroller offset at the moment when scroller was pressed.
		/// </summary>
		private int m_prevOffset;
		/// <summary>
		/// 
		/// </summary>
		private ScrollButton m_selectedScrollButton;
		/// <summary>
		/// 
		/// </summary>
		private ScrollButton m_pressedScrollButton;
		/// <summary>
		/// 
		/// </summary>
		private Rectangle m_scrollArea;
		/// <summary>
		/// 
		/// </summary>
		private ImageList m_imageList;
		/// <summary>
		/// 
		/// </summary>
		private ToolStripGalleryDropDown m_dropDown;
		/// <summary>
		/// 
		/// </summary>
		private Timer m_timer;
		/// <summary>
		/// 
		/// </summary>
		private NativeMessageHandler m_handler;
		/// <summary>
		/// 
		/// </summary>
		private ToolstripGalleryBorderStyle m_borderStyle = ToolstripGalleryBorderStyle.Single;
		/// <summary>
		/// 
		/// </summary>
		private Color m_itemBackColor = Color.Empty;
        /// <summary>
        /// Gets Currently Pointed index
        /// </summary>
        private int index = 0;
        /// <summary>
        /// Gets no. of rows based on ToolStripGallery dimension
        /// </summary>
        private int rownumbers = 0;

		#endregion

		#region Properties
        /// <summary>
        /// Gets or Sets the ToolTip visiblity for Gallery
        /// </summary>
        [DefaultValue(false),
         Description("Gets or Sets the ToolTip visiblity for Gallery")
        ]
        public bool ShowToolTip
        {
            get
            {
                return this.m_showtooltipText;
            }
            set
            {
                if (this.m_showtooltipText != value)
                {
                    this.m_showtooltipText = value;
                }
            }
        }
        bool m_fittosize = false;
        /// <summary>
        /// Gets or Sets value indicating whether toolstripgallary can Fit to its parent height. This property will take effect when AutoSize is enabled
        /// </summary>
        [DefaultValue(false),
         Description("Gets or Sets value indicating whether toolstripgallary can Fit to its parent height. This property will take effect when AutoSize is enabled")
        ]
        public bool FitToSize
        {
            get
            {
                return this.m_fittosize;
            }
            set
            {
                if (this.m_fittosize != value)
                {
                    this.m_fittosize = value;
                }
            }
        }
		/// <summary>
		/// 
		/// </summary>
		[ DesignerSerializationVisibility( DesignerSerializationVisibility.Content ) ]
		public ObservableList<ToolStripGalleryItem> Items
		{
			get
			{
				return m_items;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		internal List<ToolStripGalleryItemLayoutInfo> ItemsInfo
		{
			get
			{
				return m_itemsInfo;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public Size Dimensions
		{
			get
			{
				return m_dimensions;
			}
			set
			{
				if( m_dimensions != value )
				{
					m_dimensions = value;

                    if (m_scrollUpButton.State != ScrollButtonState.Disabled)
                    {
                        index = 2 + ScrollOffset / (m_itemSize.Height + m_itemMargin.Vertical); 
                        rownumbers = this.m_itemsInfo.Count / m_dimensions.Width;
                        if (index > rownumbers)
                            this.ScrollOffset -= (index - rownumbers) * (m_itemSize.Height + m_itemMargin.Vertical);
                    }

                    PerformLayout();
                }
            }
		}
		/// <summary>
		/// 
		/// </summary>
		public Size DropDownDimensions
		{
			get
			{
				Size result = m_dropdownDimensions;

				if (result.Width == 0)
				{
					result.Width = Math.Max(this.Width/this.ItemSize.Width,1);
				}
				
				return result;
			}
			set
			{
				m_dropdownDimensions = value;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public Size ItemSize
		{
			get
			{
				return m_itemSize;
			}
			set
			{
				if( m_itemSize != value )
				{
					m_itemSize = value;
					PerformLayout();

					if( m_dropDown != null )
					{
						m_dropDown.Gallery.ItemSize = value;
					}
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public Size DropDownMinimumSize
		{
			get
			{
				if( m_dropDown != null )
				{
					return m_dropDown.DropDownMinimumSize;
				}

				return Size.Empty;
			}
			set
			{
				if( m_dropDown.DropDownMinimumSize != value )
				{
					m_dropDown.DropDownMinimumSize = value;
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public Size ItemImageSize
		{
			get
			{
				return m_itemImageSize;
			}
			set
			{
				if( m_itemImageSize != value )
				{
					m_itemImageSize = value;
					PerformLayout();

					if( m_dropDown != null )
					{
						m_dropDown.Gallery.ItemImageSize = value;
					}
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public Padding ItemPadding
		{
			get
			{
				return m_itemPadding;
			}
			set
			{
				if( m_itemPadding != value )
				{
					m_itemPadding = value;
					PerformLayout();

					if( m_dropDown != null )
					{
						m_dropDown.Gallery.ItemPadding = value;
					}
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public Padding ItemMargin
		{
			get
			{
				return m_itemMargin;
			}
			set
			{
				if( m_itemMargin != value )
				{
					m_itemMargin = value;
					PerformLayout();

					if( m_dropDown != null )
					{
						m_dropDown.Gallery.ItemMargin = value;
					}
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		[DefaultValue( typeof( ToolStripItemDisplayStyle ), "ImageAndText" )]
		public ToolStripItemDisplayStyle ItemDisplayStyle
		{
			get
			{
				return m_itemDisplayStyle;
			}
			set
			{
				if( m_itemDisplayStyle != value )
				{
					m_itemDisplayStyle = value;
					PerformLayout();

					if( m_dropDown != null )
					{
						m_dropDown.Gallery.ItemDisplayStyle = value;
					}
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		[DefaultValue( typeof( TextImageRelation ), "ImageAboveText" )]
		public TextImageRelation ItemTextImageRelation
		{
			get
			{
				return m_textImageRelation;
			}
			set
			{
				if( m_textImageRelation != value )
				{
					m_textImageRelation = value;
					PerformLayout();

					if( m_dropDown != null )
					{
						m_dropDown.Gallery.ItemTextImageRelation = value;
					}
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		[ DefaultValue( false ) ]
		public bool ShowCaption
		{
			get
			{
				return m_bShowCaption;
			}
			set
			{
				if( m_bShowCaption != value )
				{
					m_bShowCaption = value;

					if( value )
					{
						m_captionHeight = -1;
					}
					
					PerformLayout();
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public string CaptionText
		{
			get
			{
				return m_captionText;
			}
			set
			{
				if( m_captionText != value )
				{
					m_captionText = value;
					m_captionHeight = -1;
					PerformLayout();
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		[DefaultValue( typeof( ToolStripGalleryScrollerType ), "None" )]
		public ToolStripGalleryScrollerType ScrollerType
		{
			get
			{
				return m_scrollerType;
			}
			set
			{
				if( m_scrollerType != value )
				{
					m_scrollerType = value;
					PerformLayout();
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		internal int CaptionHeight
		{
			get
			{
				if (m_captionHeight < 0)
				{
					string measureStr = (m_captionText != string.Empty) ? (m_captionText) : ("$$$");
					m_captionHeight = TextRenderer.MeasureText(measureStr, this.Font).Height + CAPTION_MARGIN.Vertical + 1;
				}
				return m_captionHeight;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		internal int ScrollOffset
		{
			get
			{
				return m_scrollOffset;
			}
			set
			{
				value = Math.Max(0, Math.Min(value, m_realHeight - this.Height));

				if( m_scrollOffset != value )
				{
					m_scrollOffset = value;

					switch(m_scrollerType)
					{
						case ToolStripGalleryScrollerType.Standard: 
							UpdateScrollerBounds();
							break;
						case ToolStripGalleryScrollerType.Compact: 
							UpdateCompactScrollBar();
							break;
					}

					Invalidate();
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		internal ScrollButton ScrollUpButton
		{
			get
			{
				return m_scrollUpButton;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		internal ScrollButton ScrollDownButton
		{
			get
			{
				return m_scrollDownButton;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		internal ScrollButton ScrollDropdownButton
		{
			get
			{
				return m_scrollDropdownButton;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		internal ScrollButton Scroller
		{
			get
			{
				return m_scroller;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		internal Rectangle ScrollArea
		{
			get
			{
				return m_scrollArea;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		[ DefaultValue( null ) ]
		public ImageList ImageList
		{
			get
			{
				return m_imageList;
			}
			set
			{
				if( m_imageList != value )
				{
					m_imageList = value;
					foreach( ToolStripGalleryItem item in m_items )
					{
						item.ImageList = value;
					}
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		[DefaultValue( typeof( ToolstripGalleryBorderStyle ), "Single" )]
		public ToolstripGalleryBorderStyle BorderStyle
		{
			get
			{
				return m_borderStyle;
			}
			set
			{
				if( m_borderStyle != value )
				{
					m_borderStyle = value;
					Invalidate();
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		[ DefaultValue( typeof( Color ), "Empty" ) ]
		public Color ItemBackColor
		{
			get
			{
				return m_itemBackColor;
			}
			set
			{
				if( m_itemBackColor != value )
				{
					m_itemBackColor = value;
					Invalidate();

					if( m_dropDown != null )
					{
						m_dropDown.Gallery.ItemBackColor = value;
					}
				}
			}
		}
		/// <summary>
		/// Gets or sets checked item.
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ToolStripGalleryItem CheckedItem
		{
			get
			{
				return m_checkedItem;
			}
			set
			{
				if (m_checkedItem != value)
				{
					// Gallery items must contain checked item.
					int index = this.Items.IndexOf( value );

					if( index != -1 )
					{
						m_checkedItem = value;

						// If checked item is not visible than show it to user by changing scroll offset.
						ShowItem( index );

						if( m_dropDown != null )
						{
							m_dropDown.Gallery.CheckedItem = value;
						}

						Invalidate();
					}
				}
			}
		}
		/// <summary>
		/// Gets or sets if item must be checked after user clicks on it. 
		/// </summary>
		[DefaultValue(false)]
		public bool CheckOnClick
		{
			get
			{
				return m_bCheckOnClick;
			}
			set
			{
				m_bCheckOnClick = value;
			}
		}
		/// <summary>
		/// Gets the current highlighted ToolStripGalleryItem in the ToolStripGallery.
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ToolStripGalleryItem SelectedItem
		{
			get { return m_selectedItem; }
		}
		/// <summary>
		/// Gets the current highlighted ToolStripGalleryItem in the ToolStripGallery.
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ToolStripGalleryItem GalleryItem
		{
			get { return m_galleryItem; }
			set
			{
				if (m_galleryItem != value)
				{
					m_galleryItem = value;
				}
			}
		}
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ToolStripGalleryItem DisabledItem
        {
            get { return m_disabledItem; }
        }
		/// <summary>
		/// 
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		internal ToolStripGalleryItem PressedItem
		{
			get { return m_pressedItem; }
		}
		#endregion

		#region Methods
		public void PerformClick(ToolStripGalleryItem item)
		{
			OnGalleryItemClicked(new ToolStripGalleryItemEventArgs(item));
		}
		#endregion

		#region Initialization
		/// <summary>
		/// 
		/// </summary>
		public ToolStripGallery()
			: this( new ObservableList<ToolStripGalleryItem>(), true )
		{
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="items"></param>
		protected ToolStripGallery( ObservableList<ToolStripGalleryItem> items, bool bCreateDropDown )
		{
			m_items = items;
			m_items.ItemAdded += new EventHandler<ListItemEventArgs<ToolStripGalleryItem>>( OnItemAdded );
			m_items.ItemRemoved += new EventHandler<ListItemEventArgs<ToolStripGalleryItem>>( OnItemRemoved );

			m_itemsInfo = new List<ToolStripGalleryItemLayoutInfo>();

			if( this.Parent != null )
			{
				this.Parent.MouseCaptureChanged += new EventHandler( OnParentMouseCaptureChanged );
			}

			m_scrollUpButton = new ScrollButton();
			m_scrollUpButton.State = ScrollButtonState.Disabled;
			m_scrollDownButton = new ScrollButton();
			m_scrollDropdownButton = new ScrollButton();
			m_scroller = new ScrollButton();

			if( bCreateDropDown )
			{
				ToolStripGallery gallery = new ToolStripGallery( m_items, false );
				gallery.ScrollerType = ToolStripGalleryScrollerType.Standard;
				gallery.BorderStyle = ToolstripGalleryBorderStyle.None;
				gallery.GalleryItemClicked += new ToolStripGalleryItemEventHandler( OnDropdownGalleryItemClicked );
				gallery.GalleryMouseMove += new ToolStripGalleryItemEventHandler(OnDropDownGalleryMouseMove);
                gallery.GalleryMouseLeave += new ToolStripGalleryItemEventHandler(OnDropDownGalleryMouseLeave);
				m_dropDown = new ToolStripGalleryDropDown( gallery );
			}
			
			m_timer = new Timer();
			m_timer.Tick += new EventHandler(OnTimerTick);

			OnParentChanged(null, this.GetCurrentParent());
		}

		void OnDropDownGalleryMouseMove(object sender, ToolStripGalleryItemEventArgs args)
		{
			OnGalleryMouseMove(args);
		}
        void OnDropDownGalleryMouseLeave(object sender, ToolStripGalleryItemEventArgs args)
        {
            OnGalleryMouseLeave(args);
        }
		#endregion

		#region Overrides
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnLayout( LayoutEventArgs e )
		{
			base.OnLayout( e );
			Layout();
		}
		/// <summary>
		/// 
		/// </summary>
		protected override void OnBoundsChanged()
		{
			base.OnBoundsChanged();

			// Calculate quantity of items in row.
			int x = ( int )Math.Floor( ( double )this.Bounds.Width / m_itemSize.Width );
			
			if( x > 0 )
			{
				// Current gallery height.
				int iGalleryHeight = this.Bounds.Height;
				// Calculate quantity of rows.
				int iRows = ( int )Math.Ceiling( ( double )this.Items.Count / x );
				// Gallery real height with hidden items.
				int iRealHeight = iRows * ( m_itemSize.Height + m_itemMargin.Vertical );
				// Gallery height accrding to scrollOffset value.
				int iHeight = ScrollOffset + iGalleryHeight;
				// Change scroll offset according to gallery bounds.

				if( iGalleryHeight > iRealHeight )
				{
					ScrollOffset = 0;
				}
				else if( iHeight > iRealHeight )
				{
					ScrollOffset += iHeight - iRealHeight;
				}
			}

			Layout();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="constrainingSize"></param>
		/// <returns></returns>
		public override Size GetPreferredSize( Size constrainingSize )
		{
			int count = Math.Max(this.Items.Count, 1);

			int x = m_dimensions.Width;
			int y = m_dimensions.Height;

			if (x == 0)
			{
				if (y == 0)
				{
					y = (int)Math.Sqrt(count);
				}
				x = (count + (y - 1)) / y;
			}
			else
			{
				if (y == 0)
				{
					y = (count + (x - 1)) / x;
				}
			}

			int width = x * ( m_itemSize.Width + m_itemMargin.Horizontal );

			if (m_scrollerType != ToolStripGalleryScrollerType.None)
			{
				width += SCROLLER_WIDTH;
			}

			int height = y * ( m_itemSize.Height + m_itemMargin.Vertical );

			if( m_bShowCaption )
			{
				height += this.CaptionHeight;
			}
			
			return new Size(width,height);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPaint( PaintEventArgs e )
		{
			base.OnPaint( e );
			
			if (this.Owner != null)
			{
				this.Owner.Renderer.DrawItemBackground(new ToolStripItemRenderEventArgs(e.Graphics, this));

				if (!(this.Owner.Renderer is Office12ToolStripRenderer))
				{
					Office12ToolStripRenderer renderer = new Office12ToolStripRenderer();
					renderer.PaintGallery(new ToolStripItemRenderEventArgs(e.Graphics, this));
				}
			}
		}
		/// <summary>
		/// Highlights items.
		/// </summary>
		/// <param name="mea"></param>
		protected override void OnMouseMove( MouseEventArgs mea )
		{
			base.OnMouseMove( mea );

			if( m_bScrollerMoving )
			{
				int offset = mea.Y - m_scrollerPrevPoint.Y;
				Rectangle workingRect = GetWorkingArea();
				int freeScrollHeight = workingRect.Height - SCROLLER_WIDTH * 2 - 1;
				int prefWorkingHeight = m_realHeight - ((m_bShowCaption) ? (this.CaptionHeight) : (0));
				int scrollOffset = ( int )Math.Round( prefWorkingHeight * ( float )offset / freeScrollHeight );
				this.ScrollOffset = m_prevOffset + scrollOffset;
			}
            item = GetItemAt(mea.Location);
            int hoverIndex = this.Items.IndexOf(item);
            bool setToolTipInfo = false;
            if (this.m_showtooltipText)
            {
                if ( (hoverIndex >= 0 && hoverIndex < this.Items.Count) &&
                  (this.item.ToolTipText != null && this.item.ToolTipText != String.Empty))
                {
                    ToolTipInfo tipInfo = this.toolTip.GetToolTip(this);
                    if (tipInfo != null && !tipInfo.Body.Text.Equals(this.item.ToolTipText))
                    {
                        this.toolTip.Hide();
                    }
                    this.info.Body.Text = item.ToolTipText;
                    this.toolTip.SetToolTip(this, this.info);
                }
                else
                {
                    setToolTipInfo = true;
                } 
            }
            else
            {
                setToolTipInfo = true;
            }
            if (setToolTipInfo && this.toolTip.GetToolTip(this) != null)
            {
                this.toolTip.SetToolTip(this, null);
            }
        	UpdateItemsState( mea.Location );
			UpdateScrollButtonsState( mea.Location );
			if (item != null)
            {
                if (GalleryItem != item)
                    OnGalleryMouseLeave(new ToolStripGalleryItemEventArgs(GalleryItem));
				GalleryItem  = item; 
				OnGalleryMouseMove(new ToolStripGalleryItemEventArgs(item));
			}
            else
            {
                if (GalleryItem != null)
                    OnGalleryMouseLeave(new ToolStripGalleryItemEventArgs(GalleryItem));
            }
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseLeave( EventArgs e )
		{
			base.OnMouseLeave( e );

			m_selectedItem = null;
            if (GalleryItem != null)
            {
                OnGalleryMouseLeave(new ToolStripGalleryItemEventArgs(GalleryItem));
            }
			if( m_selectedScrollButton != null )
			{
				m_selectedScrollButton.State = ScrollButtonState.Normal;
				m_selectedScrollButton = null;
			}

			if( !m_scroller.Bounds.IsEmpty )
			{
				m_scrollUpButton.State = ScrollButtonState.Normal;
				m_scrollDownButton.State = ScrollButtonState.Normal;
			}

			Invalidate();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="oldParent"></param>
		/// <param name="newParent"></param>
		protected override void OnParentChanged( ToolStrip oldParent, ToolStrip newParent )
		{
			base.OnParentChanged( oldParent, newParent );

			if( oldParent != null )
			{
				oldParent.MouseCaptureChanged -= new EventHandler(OnParentMouseCaptureChanged);
				oldParent.HandleCreated -= new EventHandler(OnParentHandleCreated);
				if (m_handler != null)
				{
					m_handler.MessageFilter = null;
					m_handler = null;
				}
			}

			if( newParent != null )
			{
				newParent.MouseCaptureChanged += new EventHandler( OnParentMouseCaptureChanged );
				newParent.HandleCreated += new EventHandler(OnParentHandleCreated);
				if (newParent.IsHandleCreated)
				{
					OnParentHandleCreated(newParent, EventArgs.Empty);
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="bounds"></param>
		protected override void SetBounds(Rectangle bounds)
		{
			if (this.AutoSize && !this .FitToSize )
			{
				// Gallery should display integer number of rows.
				int itemHeght = this.ItemSize.Height + this.ItemMargin.Vertical;
				bounds.Height = (Math.Max(bounds.Height / itemHeght, 1)) * itemHeght;
			}			
			base.SetBounds(bounds);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		protected virtual void OnGalleryItemClicked(ToolStripGalleryItemEventArgs args)
		{
			ToolStripDropDown ts = GetDropDownParent();
			if( ts != null )
			{
				SetCapture(false);
				ts.Close( ToolStripDropDownCloseReason.ItemClicked );
			}

			if (this.CheckOnClick)
			{
				this.CheckedItem = args.GalleryItem;
			}

			if( GalleryItemClicked != null )
			{
				GalleryItemClicked( this, args );
			}
		}
		protected virtual void OnGalleryMouseMove(ToolStripGalleryItemEventArgs args)
		{
			this.GalleryItem  = args.GalleryItem;
			if (GalleryMouseMove != null)
			{
				GalleryMouseMove(this, args);
			}
		}
        protected virtual void OnGalleryMouseLeave(ToolStripGalleryItemEventArgs args)
        {
            this.GalleryItem = args.GalleryItem;
            if (GalleryMouseLeave != null)
            {
                GalleryMouseLeave(this, args);
            }
        }
		#endregion

		#region Events
		/// <summary>
		/// 
		/// </summary>
		public event ToolStripGalleryItemEventHandler GalleryItemClicked;
		public event ToolStripGalleryItemEventHandler GalleryMouseMove;
        public event ToolStripGalleryItemEventHandler GalleryMouseLeave;
		/// <summary>
		/// 
		/// </summary>
		public event CancelEventHandler DropDownOpening
		{
			add { m_dropDown.Opening += value; }
			remove { m_dropDown.Opening -= value; }
		}
		/// <summary>
		/// 
		/// </summary>
		public event ToolStripDropDownClosedEventHandler DropDownClosed
		{
			add { m_dropDown.Closed += value; }
			remove { m_dropDown.Closed -= value; }
		}
		#endregion

		#region Implementation
		/// <summary>
		/// 
		/// </summary>
		/// <param name="itemInfo"></param>
		/// <returns></returns>
		internal Rectangle GetItemWorkingRect( ToolStripGalleryItemLayoutInfo itemInfo )
		{
			return new Rectangle( itemInfo.Bounds.Left + m_itemPadding.Left, itemInfo.Bounds.Top + m_itemPadding.Top,
							itemInfo.Bounds.Width - m_itemPadding.Horizontal, itemInfo.Bounds.Height - m_itemPadding.Vertical );
		}
		/// <summary>
		/// 
		/// </summary>
		private void ScrollUp()
		{
			this.ScrollOffset -= ( m_itemSize.Height + m_itemMargin.Vertical );
		}
		/// <summary>
		/// 
		/// </summary>
		private void ScrollDown()
		{
			this.ScrollOffset += ( m_itemSize.Height + m_itemMargin.Vertical );
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="mousePoint"></param>
		private void UpdateScrollButtonsState( Point mousePoint )
		{
			if( m_scrollerType == ToolStripGalleryScrollerType.Standard && !m_scroller.Bounds.IsEmpty )
			{
				if( m_scrollArea.Contains( mousePoint ) )
				{
					if( m_scrollUpButton.State == ScrollButtonState.Normal )
					{
						m_scrollUpButton.State = ScrollButtonState.Highlighted;
					}

					if( m_scrollDownButton.State == ScrollButtonState.Normal )
					{
						m_scrollDownButton.State = ScrollButtonState.Highlighted;
					}
				}
				else
				{
					m_scrollUpButton.State = ScrollButtonState.Normal;
					m_scrollDownButton.State = ScrollButtonState.Normal;
				}
			}

			switch( m_scrollerType )
			{
				case ToolStripGalleryScrollerType.Compact:
					if( m_pressedScrollButton != null )
					{
						if( m_pressedScrollButton.State != ScrollButtonState.Disabled )
						{
							if( !m_pressedScrollButton.Bounds.Contains( mousePoint ) )
							{
								m_pressedScrollButton.State = ScrollButtonState.Normal;
							}
							else
							{
								m_pressedScrollButton.State = ScrollButtonState.Pressed;
							}
						}
					}
					else
					{
						ScrollButton scrollButton = GetScrollButtonAt( mousePoint );

						if( m_selectedScrollButton != null && m_selectedScrollButton != scrollButton )
						{
							m_selectedScrollButton.State = ScrollButtonState.Normal;
						}

						if( scrollButton != null && scrollButton.State != ScrollButtonState.Disabled )
						{
							scrollButton.State = ScrollButtonState.Selected;
							m_selectedScrollButton = scrollButton;
						}
						else
						{
							m_selectedScrollButton = null;
						}
					}

					Invalidate();
					break;

				case ToolStripGalleryScrollerType.Standard:
					if( m_pressedScrollButton != null )
					{
						if( !m_pressedScrollButton.Bounds.Contains( mousePoint ) )
						{
							m_pressedScrollButton.State = ScrollButtonState.Normal;
						}
						else
						{
							m_pressedScrollButton.State = ScrollButtonState.Pressed;
						}
					}
					else
					{
						ScrollButton scrollButton = GetScrollButtonAt( mousePoint );

						if( m_selectedScrollButton != null && m_selectedScrollButton != scrollButton )
						{
							m_selectedScrollButton.State =
								m_scrollArea.Contains( mousePoint ) ? ( ScrollButtonState.Highlighted ) : ( ScrollButtonState.Normal );
						}

						if( scrollButton != null && scrollButton.State != ScrollButtonState.Disabled )
						{
							scrollButton.State = ScrollButtonState.Selected;
							m_selectedScrollButton = scrollButton;
						}
					}

					Invalidate();
					break;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="mousePoint"></param>
		private void UpdateItemsState( Point mousePoint )
		{
			ToolStripGalleryItem item = GetItemAt( mousePoint );

            if (item != null)
            {
                if (item.Enabled)
                {
                    if (m_selectedItem != item)
                    {
                        m_selectedItem = item;
                        Invalidate();
                    }
                }
                else
                {
                    m_selectedItem = null;
                    if (m_disabledItem != item)
                    {
                        m_disabledItem = item;
                        Invalidate();
                    }
                }
            }
            else
                m_selectedItem = null;
		}
		/// <summary>
		/// 
		/// </summary>
		private void LayoutItems()
		{
			m_itemsInfo.Clear();
			Rectangle rect = GetWorkingArea();
			int top = rect.Top;
			int left = rect.Left;
			foreach( ToolStripGalleryItem item in m_items )
			{
				if( left + m_itemMargin.Horizontal + m_itemSize.Width > rect.Right )
				{
					top += m_itemSize.Height + m_itemMargin.Vertical;
					left = rect.Left;
				}

				ToolStripGalleryItemLayoutInfo itemInfo = new ToolStripGalleryItemLayoutInfo( item );

				itemInfo.Bounds = new Rectangle( new Point( left + m_itemMargin.Left, top + m_itemMargin.Top ), m_itemSize );
				m_itemsInfo.Add( itemInfo );
				left += m_itemSize.Width + m_itemMargin.Horizontal;

				MeasureItemParameters( itemInfo );
			}

			if( m_items.Count > 0 )
			{
				m_realHeight = m_itemsInfo[ m_items.Count - 1 ].Bounds.Bottom;
			}
			else
			{
				m_realHeight = m_itemSize.Height + m_itemMargin.Vertical + ((m_bShowCaption) ? (this.CaptionHeight) : (0));
			}

			int offset = m_realHeight - this.Height;
			if( offset <= 0 )
			{
				m_scrollUpButton.State = ScrollButtonState.Disabled;
				m_scrollDownButton.State = ScrollButtonState.Disabled;
			}
			else
			{
				if( m_scrollOffset != 0 )
				{
					m_scrollUpButton.State = ScrollButtonState.Normal;
				}

				if( m_scrollOffset != offset )
				{
					m_scrollDownButton.State = ScrollButtonState.Normal;
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="itemInfo"></param>
		private void MeasureItemParameters( ToolStripGalleryItemLayoutInfo itemInfo )
		{
			Rectangle itemWorkingRect = GetItemWorkingRect( itemInfo );
			itemWorkingRect.Inflate( -1, -1 );
			int imageLeft = ( m_itemImageSize.Width > itemWorkingRect.Width ) ?
				( itemWorkingRect.Left ) : ( itemWorkingRect.Left + ( itemWorkingRect.Width - m_itemImageSize.Width ) / 2 );
			int imageTop = ( m_itemImageSize.Height > itemWorkingRect.Height ) ?
				( itemWorkingRect.Top ) : ( itemWorkingRect.Top + ( itemWorkingRect.Height - m_itemImageSize.Height ) / 2 );

			switch( m_itemDisplayStyle )
			{
				case ToolStripItemDisplayStyle.Text:
					itemInfo.TextBounds = itemWorkingRect;
					break;

				case ToolStripItemDisplayStyle.Image:
					Point loc = new Point( imageLeft, imageTop );
					itemInfo.ImageBounds = new Rectangle( loc, m_itemImageSize );
					break;

				case ToolStripItemDisplayStyle.ImageAndText:
					switch( m_textImageRelation )
					{
						case TextImageRelation.ImageAboveText:
							itemInfo.ImageBounds = new Rectangle( new Point( imageLeft, itemWorkingRect.Top ), m_itemImageSize );
							itemInfo.TextBounds = new Rectangle( itemWorkingRect.Left, itemWorkingRect.Top + m_itemImageSize.Height,
								itemWorkingRect.Width, itemWorkingRect.Height - m_itemImageSize.Height );
							break;

						case TextImageRelation.TextAboveImage:
							itemInfo.ImageBounds =
								new Rectangle( new Point( imageLeft, itemWorkingRect.Bottom - m_itemImageSize.Height ), m_itemImageSize );
							itemInfo.TextBounds = new Rectangle(
								itemWorkingRect.Location, new Size( itemWorkingRect.Width, itemWorkingRect.Height - m_itemImageSize.Height ) );
							break;

						case TextImageRelation.ImageBeforeText:
							itemInfo.ImageBounds = new Rectangle( new Point( itemWorkingRect.Left, imageTop ), m_itemImageSize );
							itemInfo.TextBounds = new Rectangle( itemWorkingRect.Left + m_itemImageSize.Width, itemWorkingRect.Top,
								itemWorkingRect.Width - m_itemImageSize.Width, itemWorkingRect.Height );
							break;

						case TextImageRelation.TextBeforeImage:
							itemInfo.ImageBounds =
								new Rectangle( new Point( itemWorkingRect.Right - m_itemImageSize.Width, imageTop ), m_itemImageSize );
							itemInfo.TextBounds = new Rectangle(
								itemWorkingRect.Location, new Size( itemWorkingRect.Width - m_itemImageSize.Width, itemWorkingRect.Height ) );
							break;
					}
					break;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		private void LayoutScrollers()
		{
			Rectangle workingRect = GetWorkingArea();

			if( m_scrollerType == ToolStripGalleryScrollerType.None )
			{
				m_scrollArea = Rectangle.Empty;
			}
			else
			{
				m_scrollArea = new Rectangle( workingRect.Right, workingRect.Top, SCROLLER_WIDTH, workingRect.Height );
			}

			switch( m_scrollerType )
			{
				case ToolStripGalleryScrollerType.Compact:
					int buttonHeight = workingRect.Height / 3;
					m_scrollUpButton.Bounds = new Rectangle( m_scrollArea.Left, m_scrollArea.Top, SCROLLER_WIDTH, buttonHeight );
					m_scrollDownButton.Bounds = new Rectangle( m_scrollArea.Left, m_scrollArea.Top + buttonHeight, SCROLLER_WIDTH, buttonHeight );
					m_scrollDropdownButton.Bounds = new Rectangle(
						m_scrollArea.Left, m_scrollArea.Top + buttonHeight * 2, SCROLLER_WIDTH, m_scrollArea.Height - buttonHeight * 2 - 1 );
					break;

				case ToolStripGalleryScrollerType.Standard:
					m_scrollUpButton.Bounds = new Rectangle( m_scrollArea.Left, m_scrollArea.Top, SCROLLER_WIDTH, SCROLLER_WIDTH );
					m_scrollDownButton.Bounds =
						new Rectangle( m_scrollArea.Left, m_scrollArea.Bottom - SCROLLER_WIDTH - 1, SCROLLER_WIDTH, SCROLLER_WIDTH );
					UpdateScrollerBounds();
					break;
			}
		}
		/// <summary>
		/// Gets area for laying out items. Excludes scrollers and caption.
		/// </summary>
		/// <returns></returns>
		internal Rectangle GetWorkingArea()
		{
			return new Rectangle(0, (m_bShowCaption) ? (this.CaptionHeight + 1) : (0),
				( m_scrollerType == ToolStripGalleryScrollerType.None ) ? ( this.Width ) : ( this.Width - SCROLLER_WIDTH ),
				(m_bShowCaption) ? (this.Height - this.CaptionHeight) : (this.Height));
		}
		/// <summary>
		/// Gets item at point.
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		private ToolStripGalleryItem GetItemAt( Point point )
		{
			ToolStripGalleryItem result = null;

			point.Y += m_scrollOffset;

			foreach( ToolStripGalleryItemLayoutInfo itemInfo in m_itemsInfo )
			{
				if( itemInfo.Bounds.Contains( point ) )
				{
					result = itemInfo.Item;
					break;
				}
			}

			return result;
		}
		/// <summary>
		/// 
		/// </summary>
		private void UpdateScrollerBounds()
		{
			Rectangle workingRect = GetWorkingArea();
			int freeScrollHeight = workingRect.Height - SCROLLER_WIDTH * 2 - 1;
			int prefWorkingHeight = m_realHeight - ((m_bShowCaption) ? (this.CaptionHeight) : (0));
			int scrollerHeight = ( workingRect.Height < prefWorkingHeight ) ?
				( ( int )Math.Round( freeScrollHeight * ( float )workingRect.Height / prefWorkingHeight ) ) : ( 0 );
			if( scrollerHeight > 0 )
			{
				int pos = ( int )Math.Round( freeScrollHeight * ( float )m_scrollOffset / prefWorkingHeight );
				m_scroller.Bounds = new Rectangle( workingRect.Right, workingRect.Top + SCROLLER_WIDTH + pos, SCROLLER_WIDTH, scrollerHeight );

				if( m_scrollUpButton.State == ScrollButtonState.Disabled )
				{
					m_scrollUpButton.State = ScrollButtonState.Normal;
				}

				if( m_scrollDownButton.State == ScrollButtonState.Disabled )
				{
					m_scrollDownButton.State = ScrollButtonState.Normal;
				}
			}
			else
			{
				m_scroller.Bounds = Rectangle.Empty;
				m_scrollUpButton.State = ScrollButtonState.Disabled;
				m_scrollDownButton.State = ScrollButtonState.Disabled;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		private void UpdateCompactScrollBar()
		{
			if(m_scrollOffset == 0)
			{
				m_scrollUpButton.State =  ScrollButtonState.Disabled;
			}
			else if (m_scrollUpButton.State == ScrollButtonState.Disabled)
			{
				m_scrollUpButton.State = ScrollButtonState.Normal;
			}
			if(m_scrollOffset >= (m_realHeight - this.Height))
			{
				m_scrollDownButton.State =  ScrollButtonState.Disabled;
			}
			else if (m_scrollDownButton.State == ScrollButtonState.Disabled)
			{
				m_scrollDownButton.State = ScrollButtonState.Normal;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		private void PerformLayout()
		{
			if( this.Parent != null )
			{
				this.Parent.PerformLayout();
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		private ScrollButton GetScrollButtonAt( Point point )
		{
			ScrollButton result = null;

			if( m_scrollUpButton.Bounds.Contains( point ) )
			{
				result = m_scrollUpButton;
			}
			else if( m_scrollDownButton.Bounds.Contains( point ) )
			{
				result = m_scrollDownButton;
			}
			else if( m_scrollDropdownButton.Bounds.Contains( point ) )
			{
				result = m_scrollDropdownButton;
			}
			else if( m_scroller.Bounds.Contains( point ) )
			{
				result = m_scroller;
			}

			return result;
		}
		/// <summary>
		/// 
		/// </summary>
		private void Layout()
		{
			LayoutItems();
			LayoutScrollers();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private ToolStripDropDown GetDropDownParent()
		{
			ToolStrip ts = this.GetCurrentParent();

			while (ts != null && !ts.IsDropDown)
			{
				ts = ts.Parent as ToolStrip;
			}
			return ts as ToolStripDropDown;
		}
		/// <summary>
		/// Method updates scroll offset value to show item entirely.
		/// </summary>
		/// <param name="index"> Item's index. </param>
		internal void ShowItem( int index )
		{
			// Calculate gallery's visible dimensions.
			int dispWidth = this.Width;
			int dispHeight = this.Height;
			
			if (this.ScrollerType != ToolStripGalleryScrollerType.None)
			{
				dispWidth -= SCROLLER_WIDTH;
			}
			if(this.ShowCaption)
			{
				dispHeight -= this.CaptionHeight;
			}

			int x = Math.Max(dispWidth / (this.ItemSize.Width + this.ItemMargin.Horizontal),1);
			int y = Math.Max(dispHeight / (this.ItemSize.Height+this.ItemMargin.Vertical),1);

			// Calculate current row in which item is located.
			int iCurrentRow = index / x;

			// Calculate item Top and Bottom values.
			int iItemTop = iCurrentRow * ( m_itemSize.Height + m_itemMargin.Vertical );
			int iItemBottom = ( iCurrentRow + 1 ) * ( m_itemSize.Height + m_itemMargin.Vertical );

			if (iItemTop < this.ScrollOffset)
			{
				this.ScrollOffset = iItemTop;
			}
			else
			{
				// Gallery's bottom offset.
				int iBottom = this.ScrollOffset + this.Height;
				if(iItemBottom > iBottom)
				{
					this.ScrollOffset += iItemBottom - iBottom;
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		internal bool ShowItem(ToolStripGalleryItem item)
		{
			if (item != null)
			{
				int i = this.Items.IndexOf(item);
				if (i >= 0)
				{
					ShowItem(i);
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		private void SetCapture(bool value)
		{
			Control parent = this.GetCurrentParent();
			if (parent != null)
			{
				parent.Capture = value;
			}
		}
		#endregion

		#region ShouldSerialize & Reset Methods
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected bool ShouldSerializeItemSize()
		{
			return m_itemSize != ITEM_SIZE;
		}
		/// <summary>
		/// 
		/// </summary>
		protected void ResetItemSize()
		{
			this.ItemSize = ITEM_SIZE;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected bool ShouldSerializeItemImageSize()
		{
			return m_itemImageSize != ITEM_IMAGE_SIZE;
		}
		/// <summary>
		/// 
		/// </summary>
		protected void ResetItemImageSize()
		{
			this.ItemImageSize = ITEM_IMAGE_SIZE;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected bool ShouldSerializeItemMargin()
		{
			return m_itemMargin != ITEM_MARGIN;
		}
		/// <summary>
		/// 
		/// </summary>
		protected void ResetItemMargin()
		{
			this.ItemMargin = ITEM_MARGIN;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected bool ShouldSerializeItemPadding()
		{
			return m_itemPadding != ITEM_PADDING;
		}
		/// <summary>
		/// 
		/// </summary>
		protected void ResetItemPadding()
		{
			this.ItemPadding = ITEM_PADDING;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected bool ShouldSerializeDropDownMinimumSize()
		{
			return this.DropDownMinimumSize != ToolStripGalleryDropDown.DROPDOWN_MIN_SIZE;
		}
		/// <summary>
		/// 
		/// </summary>
		protected void ResetDropDownMinimumSize()
		{
			this.DropDownMinimumSize = ToolStripGalleryDropDown.DROPDOWN_MIN_SIZE;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		bool ShouldSerializeDropDownDimensions()
		{
			return m_dropdownDimensions.Width>0 || m_dropdownDimensions.Height > 0;
		}
		/// <summary>
		/// 
		/// </summary>
		void ResetDropDownDimensions()
		{
			m_dropdownDimensions = Size.Empty;
		}
		protected bool ShouldSerializeDimensions()
		{
			return m_dimensions != DIMENSIONS;
		}
		/// <summary>
		/// Resets Dimensions to its default value.
		/// </summary>
		protected void ResetDimensions()
		{
			m_dimensions = DIMENSIONS;
		}
		#endregion 

		#region Event Handlers
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnItemRemoved( object sender, ListItemEventArgs<ToolStripGalleryItem> e )
		{
			PerformLayout();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnItemAdded( object sender, ListItemEventArgs<ToolStripGalleryItem> e )
		{
			if( m_imageList != null )
			{
				e.Item.ImageList = m_imageList;
			}
			
			PerformLayout();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnParentHandleCreated(object sender, EventArgs e)
		{
			Control c = sender as Control;

			m_handler = new NativeMessageHandler();
			m_handler.MessageFilter = this;
			m_handler.Assign(c.Handle);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnParentMouseCaptureChanged( object sender, EventArgs e )
		{
			m_timer.Stop();

			m_bPressed = false;
			m_bScrollerMoving = false;
			m_pressedItem = null;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnTimerTick(object sender, EventArgs e)
		{
			if (m_pressedScrollButton != null)
			{
				Control parent = this.GetCurrentParent();
				if(parent!=null)
				{
					Rectangle bounds = parent.RectangleToScreen(m_pressedScrollButton.Bounds);
					if(bounds.Contains(Cursor.Position))
					{
						if (m_pressedScrollButton == m_scrollUpButton)
						{
							ScrollUp();
						}
						else 
						{
							ScrollDown();
						}
					}
				}
			}
			m_timer.Interval = TIMER_INT;
			m_timer.Start();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void OnDropdownGalleryItemClicked( object sender, ToolStripGalleryItemEventArgs args )
		{
			OnGalleryItemClicked( args );
		}
		#endregion

		#region INativeMessageFilter Implementation
		/// <summary>
		/// 
		/// </summary>
		/// <param name="m"></param>
		/// <returns></returns>
		bool INativeMessageFilter.ProcessMessage(ref Message m)
		{
			switch ((Msg)m.Msg)
			{
				case Msg.WM_LBUTTONDOWN:
					if (IsParentMessage(ref m))
					{
						Point p = WindowsAPI.GetPointFromLPARAM((int)m.LParam);
						if (this.Bounds.Contains(p))
						{
							OnWmMouseDown(ref p);

							m.Result = IntPtr.Zero;
							m_bPressed = true;
							
							return true;
						}
					}
					break;
				case Msg.WM_LBUTTONUP:
					if (IsParentMessage(ref m))
					{
						if (m_bPressed)
						{
							Point p = WindowsAPI.GetPointFromLPARAM((int)m.LParam);
							OnWmMouseUp(ref p);

							m.Result = IntPtr.Zero;
							m_bPressed = false;

							return true;
						}
					}
					break;
				case Msg.WM_LBUTTONDBLCLK:
					if (IsParentMessage(ref m))
					{
						Point p = WindowsAPI.GetPointFromLPARAM((int)m.LParam);
						if (this.Bounds.Contains(p))
						{
							m.Result = IntPtr.Zero;
							return true;
						}
					}
					break;
				case Msg.WM_MOUSEWHEEL:
					return OnMouseWheel(ref m);
			}
			return false;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="m"></param>
		/// <returns></returns>
		bool IsParentMessage(ref Message m)
		{
			Control parent = GetCurrentParent();
			return parent != null && parent.Handle == m.HWnd;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="location"></param>
		void OnWmMouseDown(ref Point location)
		{
			SetCapture(true);

			if (m_selectedItem != null)
			{
				m_pressedItem = m_selectedItem;
				Invalidate();
			}
			else 
			{
				Point pt = new Point(location.X - this.Bounds.X, location.Y - this.Bounds.Y);

				if (m_scrollArea.Contains(pt))
				{
					if (m_scrollUpButton.Bounds.Contains(pt))
					{
						if (m_scrollUpButton.State != ScrollButtonState.Disabled)
						{
							ScrollUp();

							if (m_scrollUpButton.State != ScrollButtonState.Disabled)
							{
								m_scrollUpButton.State = ScrollButtonState.Pressed;
								m_pressedScrollButton = m_scrollUpButton;

								m_timer.Interval = TIMER_INT * 4;
								m_timer.Start();
							}
						}
					}
					else if (m_scrollDownButton.Bounds.Contains(pt))
					{
						if (m_scrollDownButton.State != ScrollButtonState.Disabled)
						{
							ScrollDown();

							if (m_scrollDownButton.State != ScrollButtonState.Disabled)
							{
								m_scrollDownButton.State = ScrollButtonState.Pressed;
								m_pressedScrollButton = m_scrollDownButton;

								m_timer.Interval = TIMER_INT * 4;
								m_timer.Start();
							}
						}
					}
					else
					{
						switch (m_scrollerType)
						{
							case ToolStripGalleryScrollerType.Compact:
								if (m_scrollDropdownButton.Bounds.Contains(pt))
								{
									if (m_dropDown != null)
									{
										m_dropDown.OwnerItem = this;
										m_dropDown.Renderer = this.Owner.Renderer;
										m_dropDown.Gallery.Dimensions = this.DropDownDimensions;

										Point dropDownLocation = this.Parent.PointToScreen(this.Bounds.Location);
										m_dropDown.Show(dropDownLocation);
									}
									else m_pressedScrollButton = m_scrollDropdownButton;
								}
								break;

							case ToolStripGalleryScrollerType.Standard:
								if (m_scroller.Bounds.Contains(pt))
								{
									m_scroller.State = ScrollButtonState.Pressed;
									m_pressedScrollButton = m_scroller;
									m_bScrollerMoving = true;
									m_scrollerPrevPoint = location;
									m_prevOffset = this.ScrollOffset;
								}
								else
								{
									Rectangle rectScrollBar = new Rectangle(m_scrollUpButton.Bounds.Location,
										new Size(m_scrollUpButton.Bounds.Width,m_scrollDownButton.Bounds.Bottom - m_scrollUpButton.Bounds.Top));
									if (m_scroller.State != ScrollButtonState.Disabled && rectScrollBar.Contains(pt))
									{
										if (pt.Y > m_scroller.Bounds.Y)
											ScrollDown();
										else
											ScrollUp();
									}
								}
								break;
						}
					}
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="location"></param>
		void OnWmMouseUp(ref Point location)
		{
			m_bScrollerMoving = false;
			m_pressedScrollButton = null;

			if (m_pressedItem != null)
			{
				if (m_pressedItem == m_selectedItem)
				{
					PerformClick(m_pressedItem);
				}
				m_pressedItem = null;
			}

			Invalidate();

			SetCapture(false);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="m"></param>
		private bool OnMouseWheel(ref Message m)
		{
			Control parent = GetCurrentParent();
			if (parent != null && parent.Handle == m.HWnd)
			{
				Point p = parent.PointToClient(WindowsAPI.GetPointFromLPARAM((int)m.LParam));
				if (this.Bounds.Contains(p))
				{
					int delta = WindowsAPI.HIGH_ORDER(m.WParam) / SystemInformation.MouseWheelScrollDelta;
					this.ScrollOffset -= delta * (m_itemSize.Height + m_itemMargin.Vertical);

					m.Result = IntPtr.Zero;
					return true;
				}
			}
			return false;
		}
		#endregion
	}
	#endregion

	#region *** ToolStripGalleryScrollerType
	/// <summary>
	/// 
	/// </summary>
	public enum ToolStripGalleryScrollerType
	{
		/// <summary>
		/// 
		/// </summary>
		None,
		/// <summary>
		/// 
		/// </summary>
		Compact,
		/// <summary>
		/// 
		/// </summary>
		Standard
	}
	#endregion

	#region *** ToolStripGalleryScrollButtonState
	/// <summary>
	/// 
	/// </summary>
	public enum ScrollButtonState
	{
		/// <summary>
		/// 
		/// </summary>
		Normal,
		/// <summary>
		/// 
		/// </summary>
		Disabled, 
		/// <summary>
		/// 
		/// </summary>
		Selected,
		/// <summary>
		/// 
		/// </summary>
		Pressed,
		/// <summary>
		/// 
		/// </summary>
		Highlighted
	}
	#endregion

	#region *** ScrollButton
	/// <summary>
	/// 
	/// </summary>
	internal class ScrollButton
	{
		#region Public Fields
		/// <summary>
		/// 
		/// </summary>
		public Rectangle Bounds;
		/// <summary>
		/// 
		/// </summary>
		public ScrollButtonState State = ScrollButtonState.Normal;
		#endregion
	}
	#endregion

	#region *** BorderStyle
	/// <summary>
	/// 
	/// </summary>
	public enum ToolstripGalleryBorderStyle
	{
		/// <summary>
		/// 
		/// </summary>
		None,
		/// <summary>
		/// 
		/// </summary>
		Single
	}
	#endregion
}

#endif
