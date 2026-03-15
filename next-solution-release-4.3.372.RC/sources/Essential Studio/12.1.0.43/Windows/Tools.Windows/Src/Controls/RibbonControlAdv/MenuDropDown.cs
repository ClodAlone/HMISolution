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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Windows.Forms.Layout;

using Syncfusion.Windows.Forms.Tools.Win32API;

namespace Syncfusion.Windows.Forms.Tools
{
	#region MenuDropDown
	[Designer(typeof(Design.MenuDropDownDesigner))]
    [ToolboxItem(false)]
	public class MenuDropDown : ToolStripOverflow
	{
		#region Constants
		/// <summary> Width of scroll button that used for scrolling items in Panel. </summary>
		private const int SCROLL_BUTTON_HEIGHT = 12;
		/// <summary> Separator width. </summary>
		private const int SEPARATOR_WIDTH = 2;
		#endregion

		#region *** Enums
		internal enum PanelType
		{
			Main,
			Auxiliary,
			System
		}
		#endregion

		#region *** IPanel
		internal interface IPanel
		{
			/// <summary>
			/// 
			/// </summary>
			ToolStrip Owner { get; }
			/// <summary>
			/// 
			/// </summary>
			PanelType PanelType { get; }
			/// <summary>
			/// 
			/// </summary>
			Size MinimumSize { get; set; }
			/// <summary>
			/// 
			/// </summary>
			/// <param name="item"></param>
			/// <param name="location"></param>
			void SetItemLocation( ToolStripItem item, Point location );
			/// <summary>
			/// 
			/// </summary>
			/// <param name="item"></param>
			/// <returns></returns>
			ToolStripItemPlacement GetItemPlacement(ToolStripItem item);
		}
		#endregion

		#region *** PanelSite
		class PanelSite : ISite
		{
			#region Constructors
			/// <summary>
			/// 
			/// </summary>
			/// <param name="parentSite"></param>
			public PanelSite(IComponent component, ISite parentSite)
			{
				m_component = component;
				m_parentSite = parentSite;
			}
			#endregion

			#region ISite Members
			public IComponent Component
			{
				get { return m_component; }
			}
			public IContainer Container
			{
				get { return null; }
			}
			public bool DesignMode
			{
				get { return m_parentSite.DesignMode; }
			}
			public string Name
			{
				get
				{
					IPanel panel = m_component as IPanel;
					
					switch (panel.PanelType)
					{
						case PanelType.Auxiliary:
							return "Auxiliary items";
						case PanelType.System:
							return "System items";
					}
					return "Main items";
				}
				set
				{
				}
			}
		
			public object GetService(Type serviceType)
			{
				return m_parentSite.GetService(serviceType);
			}
			#endregion

			#region Fields
			IComponent m_component;
			ISite m_parentSite;
			#endregion

		}
		#endregion

		#region *** Panel
		public class Panel
		{
			#region Constants
			/// <summary> Default image width. </summary>
			private const int IMAGE_WIDTH = 16;
			/// <summary> Default image height. </summary>
			private const int IMAGE_HEIGHT = 16;
			/// <summary> Separator width. </summary>
			private const int SEPARATOR_WIDTH = 2;
			/// <summary> Width of toolstrip separator. </summary>
			private const int TOOLSTRIP_SEPARATOR_WIDTH = 4;
			/// <summary> Interval for timer. </summary>
			private const int TIMER_INT = 200;
			/// <summary> Width of scroll button that used for scrolling items in Panel. </summary>
			private const int SCROLL_BUTTON_HEIGHT = 12;
			/// <summary> Width of scroll button that used for scrolling items in Panel. </summary>
			private const int MIN_CAPTION_HEIGHT = 21;
			#endregion

			#region Enums
			/// <summary>
			/// Different areas of the control.
			/// </summary>
			protected enum ScrollButtonsArea
			{
				/// <summary> Out of scroll buttons. </summary>
				None,
				/// <summary> Down scroll button. </summary>
				DownScrollButton,
				/// <summary> Up scroll button. </summary>
				UpScrollButton
			}
			#endregion

			#region Constructors
			internal Panel( MenuDropDown parent, PanelType type )
			{
				m_type = type;
				m_parent = parent;
				m_panelStrip = new PanelStrip( this );

				m_timer = new Timer();
				m_timer.Tick += new EventHandler( OnTimerTick );
			}
			#endregion

			#region Properties
			/// <summary>
			/// Gets all the items that belong to a panel.
			/// </summary>
			[Description("Gets all the items that belong to a panel.")]
			[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
			[Editor( typeof( Design.MenuDropDownItemsEditor ), typeof( System.Drawing.Design.UITypeEditor ) )]
			public ToolStripItemCollection Items
			{
				get
				{
					return m_panelStrip.Items;
				}
			}
			/// <summary>
			/// Gets or sets the size, in pixels, of images used in panel items.
			/// </summary>
			[Description("Gets or sets the size, in pixels, of images used in panel items.")]
			[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
			public Size ImageScalingSize
			{
				get
				{
					return m_panelStrip.ImageScalingSize;
				}
				set
				{
					if( m_panelStrip.ImageScalingSize != value )
					{
						m_panelStrip.SuspendLayout();

						m_panelStrip.ImageScalingSize = value;

						foreach( ToolStripItem item in this.Items )
						{
							if( item.ImageScaling == ToolStripItemImageScaling.SizeToFit )
							{
								item.ImageScaling = ToolStripItemImageScaling.None;
								item.ImageScaling = ToolStripItemImageScaling.SizeToFit;
							}
						}

						m_panelStrip.ResumeLayout();
					}
				}
			}
			/// <summary>
			/// Gets or sets the amount of spaces to indent separators in the panel.
			/// </summary>
			[Description("Gets or sets the amount of spaces to indent separators in the panel.")]
			[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
			public int SeparatorIndent
			{
				get
				{
					if( m_separatorIndent < 0 )
					{
						return this.ImageScalingSize.Width;
					}
					return m_separatorIndent;
				}
				set
				{
					if( m_separatorIndent != value )
					{
						m_parent.SuspendLayout();

						m_separatorIndent = value;

						m_parent.ResumeLayout();
					}
				}
			}
			/// <summary> Gets or sets the panel title's text. </summary>
			[Description("Gets or sets the panel title's text.")]
			[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
			[ DefaultValue( ( string )null ) ]
			public string Text
			{
				get
				{
					return m_sText;
				}
				set
				{
					m_sText = value;

					OnLayoutChanged();
				}
			}
			/// <summary> Gets or sets the font of the text displayed in panel items. </summary>
			[Description("Gets or sets the font of the text displayed in panel items.")]
			[DesignerSerializationVisibility( DesignerSerializationVisibility.Visible )]
			public Font Font
			{
				get
				{
					if( m_font == null )
					{
						return m_DefaultFont;
					}

					return m_font;
				}
				set
				{
					m_font = value;

					OnLayoutChanged();
				}
			}
			/// <summary> Gets or sets caption height. </summary>
			internal int CaptionHeight
			{
				get
				{
					if( m_type != PanelType.Auxiliary )
					{
						return 0;
					}

					if( m_iCaptionHeight == -1 )
					{
						int height = TextRenderer.MeasureText( this.Text, this.Font ).Height;

						if( height > 0 )
						{
							if( height < MIN_CAPTION_HEIGHT )
							{
								height = MIN_CAPTION_HEIGHT;
							}

							height += SEPARATOR_WIDTH;
						}

						m_iCaptionHeight = height;
					}

					return m_iCaptionHeight;
				}
				set
				{
					m_iCaptionHeight = value;
				}
			}
			/// <summary>
			/// 
			/// </summary>
			[Browsable( false )]
			public Size PreferredSize
			{
				get
				{
					Size szResult = Size.Empty;

					foreach( ToolStripItem item in this.Items )
					{
						if( item.Available )
						{
							Size szItem = GetItemSize( item );

							if( m_type == PanelType.System )
							{
								// Horizontal layout
								szResult.Width += szItem.Width;

								if( szResult.Height < szItem.Height )
								{
									szResult.Height = szItem.Height;
								}
							}
							else
							{
								szResult.Height += szItem.Height;

								if( szResult.Width < szItem.Width )
								{
									szResult.Width = szItem.Width;
								}
							}
						}
					}

					szResult.Height += CaptionHeight;

					if( m_type != PanelType.System )
					{
						int iMinimumWidth = MinimumSize.Width;
						int iMinimumHeight = MinimumSize.Height;

						// Set width according to Minimum width.
						if( iMinimumWidth > 0 && iMinimumWidth > szResult.Width )
						{
							szResult.Width = iMinimumWidth;
						}

						// Set height according to Minimum height.
						if( iMinimumHeight > 0 && iMinimumHeight > szResult.Height )
						{
							szResult.Height = iMinimumHeight;
						}
					}

					return szResult;
				}
			}
			/// <summary> Gets or sets minimum size of panel. </summary>
			[Description("Gets or sets minimum size of panel.")]
			public Size MinimumSize
			{
				get
				{
					return m_panelStrip.MinimumSize;
				}
				set
				{
					m_panelStrip.MinimumSize = value;
				}
			}
			/// <summary>
			/// 
			/// </summary>
			internal ToolStrip ToolStrip
			{
				get
				{
					return m_panelStrip;
				}
			}
			/// <summary>
			/// 
			/// </summary>
			internal PanelType PanelType
			{
				get
				{
					return m_type;
				}
			}
			/// <summary>
			/// 
			/// </summary>
			ToolStripRenderer Renderer
			{
				get
				{
					return m_parent.Renderer;
				}
			}
			/// <summary>
			/// 
			/// </summary>
			ToolStripItem OwnerItem
			{
				get
				{
					return m_parent.OwnerItem;
				}
			}
			/// <summary>
			/// 
			/// </summary>
			protected Rectangle Bounds
			{
				get
				{
					if( m_type == PanelType.Main )
					{
						return m_parent.MainItemsBounds;
					}
					else if( m_type == PanelType.Auxiliary )
					{
						return m_parent.AuxItemsBounds;
					}
					else
					{
						return Rectangle.Empty;
					}
				}
			}
			/// <summary> Area in which user pushed mouse button. </summary>
			protected ScrollButtonsArea PushedButton
			{
				get
				{
					return m_pushedButton;
				}
				set
				{
					if( m_pushedButton != value )
					{
						m_pushedButton = value;
					}
				}
			}
			/// <summary> Get bounds of up scroll button. </summary>
			internal Rectangle UpScrollBounds
			{
				get
				{
					Rectangle rc = Rectangle.Empty;

					if( m_bIsUpScroll )
					{
						Rectangle rcBounds = Bounds;

						rc = new Rectangle( rcBounds.X, rcBounds.Y + CaptionHeight, rcBounds.Width, SCROLL_BUTTON_HEIGHT );
					}

					return rc;
				}
			}
			/// <summary> Get bounds of down scroll button. </summary>
			internal Rectangle DownScrollBounds
			{
				get
				{
					Rectangle rc = Rectangle.Empty;

					if( m_bIsDownScroll )
					{
						Rectangle rcBounds = Bounds;

						rc = new Rectangle( rcBounds.X, rcBounds.Bottom - SCROLL_BUTTON_HEIGHT, rcBounds.Width, SCROLL_BUTTON_HEIGHT );
					}

					return rc;
				}
			}
			/// <summary> Gets or sets position of TopMost Item. </summary>
			internal int ScrollPositionInternal
			{
				get
				{
					return m_iScrollPosition;
				}
				set
				{
					m_iScrollPosition = value;
					m_parent.PerformLayout();
				}
			}
			/// <summary> Gets value that indicates if down scroll button is selected. </summary>
			internal bool DownScrollSelected
			{
				get
				{
					return m_bDownScrollSelected;
				}
				set
				{
					m_bDownScrollSelected = value;
				}
			}
			/// <summary> Gets value that indicates if up scroll button is selected. </summary>
			internal bool UpScrollSelected
			{
				get
				{
					return m_bUpScrollSelected;
				}
				set
				{
					m_bUpScrollSelected = value;
				}
			}
			#endregion

			#region Methods
			public void PerformLayout( Rectangle rc )
			{
				IPanel iPanel = m_panelStrip as IPanel;

				switch( m_type )
				{
					case PanelType.System:
						{
							if( m_parent.RightToLeft == RightToLeft.Yes )
							{
								int x = rc.Left;
								int y = rc.Top;

								foreach( ToolStripItem item in m_panelStrip.Items )
								{
									if( item.Available )
									{
										if( item is ToolStripSeparator )
										{
											item.Height = rc.Height;
											item.Width = SEPARATOR_WIDTH;

											iPanel.SetItemLocation( item, new Point( x, y ) );
											x += SEPARATOR_WIDTH;
										}
										else
										{
											if( item.AutoSize )
											{
												item.Size = item.GetPreferredSize( Size.Empty );
											}

											iPanel.SetItemLocation( item, new Point( x + item.Margin.Left, y + item.Margin.Top ) );

											x += ( item.Width + item.Margin.Horizontal );
										}
									}
								}
							}
							else
							{
								int x = rc.Right;
								int y = rc.Top;

								foreach( ToolStripItem item in m_panelStrip.Items )
								{
									if( item.Available )
									{
										if( item is ToolStripSeparator )
										{
											item.Height = rc.Height;
											item.Width = SEPARATOR_WIDTH;

											iPanel.SetItemLocation( item, new Point( x - SEPARATOR_WIDTH, y ) );
											x -= SEPARATOR_WIDTH;
										}
										else
										{
											if( item.AutoSize )
											{
												item.Size = item.GetPreferredSize( Size.Empty );
											}

											iPanel.SetItemLocation( item, new Point( x - item.Margin.Right - item.Width, y + item.Margin.Top ) );

											x -= ( item.Width + item.Margin.Horizontal );
										}
									}
								}
							}
						}
						break;
					default:
						{
							int x = rc.X;
							int y = rc.Y + CaptionHeight - ScrollPositionInternal;

							int indent = this.SeparatorIndent;
                            int iSeparatorX = ( m_parent.RightToLeft == RightToLeft.Yes ) ? x : x + indent;

							foreach( ToolStripItem item in m_panelStrip.Items )
							{
								if( item.Available )
								{
									if( item is ToolStripSeparator )
									{
										item.Height = SEPARATOR_WIDTH;
										item.Width = rc.Width - indent;

										if( y > rc.Bottom )
										{
											iPanel.SetItemLocation( item, new Point( -item.Width - 1, y ) );
										}
										else
										{
											iPanel.SetItemLocation( item, new Point( iSeparatorX, y ) );
											y += SEPARATOR_WIDTH;
										}
									}
									else
									{
										if( item.AutoSize )
										{
											item.Height = item.GetPreferredSize( Size.Empty ).Height;
										}

										item.Width = rc.Width - item.Margin.Horizontal;

										if( y > rc.Bottom )
										{
											iPanel.SetItemLocation( item, new Point( -item.Width - 1, y + item.Margin.Top ) );
										}
										else
										{
											iPanel.SetItemLocation( item, new Point( x + item.Margin.Left, y + item.Margin.Top ) );
											y += item.Height + item.Margin.Vertical;
										}
									}
								}
							}
						}
						break;
				}
			}
			#endregion

			#region Implementation
			/// <summary>
			/// 
			/// </summary>
			/// <param name="item"></param>
			/// <returns></returns>
			Size GetItemSize( ToolStripItem item )
			{
				Size szItem;

				if( item is ToolStripSeparator )
				{
					if( m_type == PanelType.System )
					{
						// Horizontal layout
						szItem = new Size( SEPARATOR_WIDTH, 1 );
					}
					else
					{
						szItem = new Size( 1, SEPARATOR_WIDTH );
					}
				}
				else
				{
					szItem = item.AutoSize ? item.GetPreferredSize( Size.Empty ) : item.Size;

					szItem.Height += item.Margin.Vertical;
					szItem.Width += item.Margin.Horizontal;
				}

				return szItem;
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="e"></param>
			void OnLayout( LayoutEventArgs e )
			{
				m_parent.PerformLayout();
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="e"></param>
			void OnItemAdded( ToolStripItemEventArgs e )
			{
				ToolStrip.SetItemParent( e.Item, m_parent );

				m_parent.OnItemAdded( e );
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="e"></param>
			void OnItemRemoved( ToolStripItemEventArgs e )
			{
				m_parent.OnItemRemoved( e );
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="e"></param>
			void OnItemClicked( ToolStripItemClickedEventArgs e )
			{
				m_parent.OnItemClicked( e );

			}
			/// <summary>
			/// 
			/// </summary>
			private void OnLayoutChanged()
			{
				m_iCaptionHeight = -1;

				if( m_parent.IsHandleCreated )
				{
					m_parent.PerformLayout();
				}
			}
			/// <summary>
			/// 
			/// </summary>
			/// <returns></returns>
			internal int GetItemsHeight()
			{
				int iHeight = 0;

				foreach( ToolStripItem item in this.Items )
				{
					if( item.Available )
					{
						Size szItem = GetItemSize( item );

						iHeight += szItem.Height;
					}
				}

				iHeight += CaptionHeight;

				return iHeight;
			}
			/// <summary> Process position in Layout for items n Panel and if it not in right bounds that set correct value to it. </summary>
			/// <param name="position"> Position to process. </param>
			/// <returns></returns>
			private int GetValidScrollPosition( int position )
			{
				int iValue = position;

				if( iValue != 0 )
				{
					int iItemHeight = GetItemsHeight();
					int iHeight = Bounds.Height;

					if( iValue < 0 || iItemHeight < iHeight )
					{
						iValue = 0;
					}
				}

				return iValue;
			}
			/// <summary> Move controls to down according to scroll position and their location. </summary>
			private void ScrollToDown()
			{
				if( m_iItemIndex + 1 > Items.Count - 1 )
				{
					return;
				}
				else
				{
					int iScrollButtonHeight = 0;
					if( !m_bIsUpScroll )
					{
						iScrollButtonHeight = SCROLL_BUTTON_HEIGHT;
					}

					ScrollPositionInternal = GetValidScrollPosition( ScrollPositionInternal + Items[ m_iItemIndex ].Height + Items[ m_iItemIndex ].Margin.Vertical - iScrollButtonHeight );
					m_iItemIndex++;
				}
			}
			/// <summary> Move controls to up according to scroll position and their location. </summary>
			private void ScrollToUp()
			{
				if( m_iItemIndex - 1 < 0 )
				{
					return;
				}
				else
				{
					ScrollPositionInternal = GetValidScrollPosition( ScrollPositionInternal - Items[ m_iItemIndex - 1 ].Height - Items[ m_iItemIndex - 1 ].Margin.Vertical );
					m_iItemIndex--;
				}
			}
			/// <summary> Initializes & starts timer. </summary>
			/// <param name="mousePushedArea">Area where mouse was pushed and caused timer to start.</param>
			private void StartTimer( ScrollButtonsArea mousePushedArea )
			{
				m_timer.Interval = m_timerInt * 4;
				m_timer.Tag = mousePushedArea;
				m_timer.Start();
			}
			/// <summary> Handles mouse keeping pushed. </summary>
			/// <param name="sender"></param>
			/// <param name="e"></param>
			private void OnTimerTick( object sender, EventArgs e )
			{
				ScrollButtonsArea area = ( ScrollButtonsArea )( ( ( Timer )sender ).Tag );

				Point p = m_parent.PointToClient( Control.MousePosition );

				switch( area )
				{
					case ScrollButtonsArea.DownScrollButton:
						if( DownScrollBounds.IsEmpty )
						{
							m_parent.Capture = false;
						}
						else if( DownScrollBounds.Contains( p ) )
						{
							ScrollToDown();
						}
						break;

					case ScrollButtonsArea.UpScrollButton:
						if( UpScrollBounds.IsEmpty )
						{
							m_parent.Capture = false;
						}
						else if( UpScrollBounds.Contains( p ) )
						{
							ScrollToUp();
						}
						break;
				}

				m_timer.Interval = m_timerInt;
			}
			#endregion

			#region Event handlers
			
			internal void OnMouseMove( MouseEventArgs e )
			{
				// Do not process Mouse Move if cursor not in panel bounds
				// and unselect selected Scroll button.
				if( !Bounds.Contains( e.Location ) )
				{
					if( m_bDownScrollSelected )
					{
						m_bDownScrollSelected = false;
						m_parent.RefreshScroll();
					}

					if( m_bUpScrollSelected )
					{
						m_bUpScrollSelected = false;
						m_parent.RefreshScroll();
					}

					return;
				}

				bool bDownScrollSelected = false;
				bool bUpScrollSelected = false;

				// If mouse over down scroll button than highlight it.
				if( m_bIsDownScroll )
				{
					bDownScrollSelected = DownScrollBounds.Contains( e.Location );

					if( bDownScrollSelected != m_bDownScrollSelected )
					{
						m_bDownScrollSelected = bDownScrollSelected;
						m_parent.RefreshScroll();
					}
				}
				// If mouse over left scroll button than highlight it.
				if( m_bIsUpScroll && !bDownScrollSelected )
				{
					bUpScrollSelected = UpScrollBounds.Contains( e.Location );

					if( bUpScrollSelected != m_bUpScrollSelected )
					{
						m_bUpScrollSelected = bUpScrollSelected;
						m_parent.RefreshScroll();
					}
				}
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="e"></param>
			internal void OnMouseLeave( EventArgs e )
			{
				if( m_bDownScrollSelected )
				{
					m_bDownScrollSelected = false;
					m_parent.RefreshScroll();
				}
				else if( m_bUpScrollSelected )
				{
					m_bUpScrollSelected = false;
					m_parent.RefreshScroll();
				}
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="e"></param>
			internal void OnMouseDown( MouseEventArgs e )
			{
				// Do not process Mouse Down if cursor not in panel bounds.
				if( !Bounds.Contains( e.Location ) )
				{
					return;
				}

				if( e.Button == MouseButtons.Left )
				{
					bool bMouseDown = false;

					if( m_bIsDownScroll && DownScrollBounds.Contains( e.Location ) )
					{
						bMouseDown = true;
						m_parent.Capture = true;

						PushedButton = ScrollButtonsArea.DownScrollButton;
						ScrollToDown();
						StartTimer( ScrollButtonsArea.DownScrollButton );
					}
					if( !bMouseDown && ( m_bIsUpScroll && UpScrollBounds.Contains( e.Location ) ) )
					{
						m_parent.Capture = true;

						PushedButton = ScrollButtonsArea.UpScrollButton;
						ScrollToUp();
						StartTimer( ScrollButtonsArea.UpScrollButton );
					}
				}
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="e"></param>
			internal void OnMouseCaptureChanged( EventArgs e )
			{
				m_timer.Stop();
				PushedButton = ScrollButtonsArea.None;
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="e"></param>
			internal void OnPaint( PaintEventArgs e, RibbonControlAdvHeader.RibbonControlAdvHeaderRenderer renderer )
			{
				int iItemsHeight = GetItemsHeight();
				int iItemsNeededHeight = Bounds.Height;

				if( iItemsHeight > iItemsNeededHeight )
				{
					// Choose needed Scroll buttons to further painting.
					m_bIsUpScroll = ( ScrollPositionInternal > 0 );
					m_bIsDownScroll = ( iItemsHeight - ScrollPositionInternal > iItemsNeededHeight );

					if( m_bIsDownScroll )
					{
						// Draw down scroll button over the Auxiliary items.
						renderer.DrawUpDownScrollButton( this, e.Graphics, DownScrollBounds, true );
					}
					else
					{
						m_bDownScrollSelected = false;
					}

					if( m_bIsUpScroll )
					{
						// Draw up scroll button over the Auxiliary items.
						renderer.DrawUpDownScrollButton( this, e.Graphics, UpScrollBounds, false );
					}
					else
					{
						m_bUpScrollSelected = false;
					}
				}
				else
				{
					m_bIsDownScroll = false;
					m_bIsUpScroll = false;
				}
			}
			#endregion

			#region ShouldSerialize & Reset methods
			/// <summary>
			/// 
			/// </summary>
			/// <returns></returns>
			bool ShouldSerializeImageScalingSize()
			{
				return m_panelStrip.ImageScalingSize.Width != IMAGE_WIDTH || m_panelStrip.ImageScalingSize.Height != IMAGE_HEIGHT;
			}
			/// <summary>
			/// 
			/// </summary>
			void ResetImageScalingSize()
			{
				this.ImageScalingSize = new Size( IMAGE_WIDTH, IMAGE_HEIGHT );
			}
			/// <summary>
			/// 
			/// </summary>
			/// <returns></returns>
			bool ShouldSerializeSeparatorIndent()
			{
				return m_separatorIndent >= 0;
			}
			/// <summary>
			/// 
			/// </summary>
			void ResetImageSeparatorIndent()
			{
				m_separatorIndent = -1;
			}
			/// <summary>
			/// 
			/// </summary>
			void ResetFont()
			{
				this.Font = null;
			}
			/// <summary>
			/// 
			/// </summary>
			/// <returns></returns>
			bool ShouldSerializeFont()
			{
				return ( m_font != null );
			}
			/// <summary>
			/// 
			/// </summary>
			void ResetMinimumSize()
			{
				MinimumSize = Size.Empty;
			}
			/// <summary>
			/// 
			/// </summary>
			/// <returns></returns>
			bool ShouldSerializeMinimumSize()
			{
				return ( MinimumSize != Size.Empty );
			}		
			#endregion

			#region Fields
			/// <summary>
			/// 
			/// </summary>
			PanelType m_type;
			/// <summary>
			/// 
			/// </summary>
			MenuDropDown m_parent;
			/// <summary>
			/// 
			/// </summary>
			PanelStrip m_panelStrip;
			/// <summary>
			/// 
			/// </summary>
			int m_separatorIndent = -1;
			/// <summary> Font for panel caption </summary>
			private Font m_font;
			/// <summary> Text for panel caption. </summary>
			private string m_sText;
			/// <summary> Caption height. </summary>
			private int m_iCaptionHeight = -1;
			/// <summary> Position of rightmost tab Item. </summary>
			private int m_iScrollPosition = 0;
			/// <summary> </summary>
			private bool m_bIsUpScroll;
			/// <summary> </summary>
			private bool m_bIsDownScroll;
			/// <summary> Indicates if up scroll button is selected. </summary>
			private bool m_bUpScrollSelected = false;
			/// <summary> Indicates if down scroll button is selected. </summary>
			private bool m_bDownScrollSelected = false;
			/// <summary> Timer for handling mouse keeping pushed. </summary>
			private Timer m_timer;
			/// <summary> Interval for timer. </summary>
			private int m_timerInt = TIMER_INT;
			/// <summary> Currently pushed button. </summary>
			private ScrollButtonsArea m_pushedButton;
			/// <summary> Bounds of panel. </summary>
			private Rectangle m_rcBounds = Rectangle.Empty;
			/// <summary> Index of first showed item. </summary>
			private int m_iItemIndex = 0;
			#endregion

			#region Static fields
			/// <summary> Default font. </summary>
			private static Font m_DefaultFont = new Font( Control.DefaultFont, FontStyle.Bold );
			#endregion

			#region *** PanelStrip
			internal class PanelStrip : ToolStripDropDown, IPanel
			{
				#region *** PanelStripLayout
				class PanelStripLayout : LayoutEngine
				{
					/// <summary>
					/// 
					/// </summary>
					public PanelStripLayout()
					{ }
					/// <summary>
					/// 
					/// </summary>
					/// <param name="container"></param>
					/// <param name="layoutEventArgs"></param>
					/// <returns></returns>
					public override bool Layout(object container, LayoutEventArgs layoutEventArgs)
					{
						PanelStrip panelStrip = container as PanelStrip;

						if (panelStrip.m_panel  != null)
						{
							panelStrip.m_panel.OnLayout(layoutEventArgs);
						}
						
						return false;
					}
				}
				#endregion

				#region Constructors
				/// <summary>
				/// 
				/// </summary>
				static PanelStrip()
				{
					m_layout = new PanelStripLayout();
				}
				/// <summary>
				/// 
				/// </summary>
				/// <param name="panel"></param>
				public PanelStrip(Panel panel) : base()
				{
					m_panel = panel;

					//this.OwnerItem = panel.OwnerItem;
					this.Renderer = panel.Renderer;

					this.ImageScalingSize = new Size( IMAGE_WIDTH, IMAGE_HEIGHT );
				}
				#endregion

				#region Properties
				/// <summary>
				/// 
				/// </summary>
				public override LayoutEngine LayoutEngine
				{
					get { return m_layout; }
				}
				/// <summary>
				/// Gets or sets minimum size of PanelStrip.
				/// </summary>
				public override Size MinimumSize
				{
					get
					{
						return m_szMinimum;
					}
					set
					{
						if (m_szMinimum != value)
						{
							if (value.Width < 0)
							{
								value.Width = 0;
							}

							if (value.Height < 0)
							{
								value.Height = 0;
							}

							m_szMinimum = value;

							PerformLayout();
						}
					}
				}
				#endregion

				#region Overrides
				/// <summary>Control should not be created</summary>
				protected override void CreateHandle()
				{
					throw new Exception( "Control should not be created." );
				}
				/// <summary>
				/// Handles changes in layout
				/// </summary>
				/// <param name="e"></param>
				protected override void OnItemAdded( ToolStripItemEventArgs e )
				{
					m_panel.OnItemAdded( e );
				}
				/// <summary>
				/// Handles changes in layout
				/// </summary>
				/// <param name="e"></param>
				protected override void OnItemRemoved( ToolStripItemEventArgs e )
				{
					m_panel.OnItemRemoved( e );
				}
				/// <summary>
				/// Forwards ItemClicked event to parent
				/// </summary>
				/// <param name="e"></param>
				protected override void OnItemClicked( ToolStripItemClickedEventArgs e )
				{
					m_panel.OnItemClicked( e );
				}
				/// <summary>
				/// Don't call base implementation to prevent parent's changing
				/// </summary>
				protected override void SetDisplayedItems()
				{
				}
				/// <summary>
				/// 
				/// </summary>
				public override RightToLeft RightToLeft
				{
					get
					{
                        if (m_panel != null)
						{
							return m_panel.m_parent.RightToLeft;
						}
						else
						{
							return RightToLeft.No;
						}
					}
					set
					{
					}
				}
				#endregion

				#region IPanel Members
				/// <summary>
				/// 
				/// </summary>
				ToolStrip IPanel.Owner
				{
					get { return m_panel.m_parent;  }
				}
				/// <summary>
				/// 
				/// </summary>
				PanelType IPanel.PanelType
				{
					get
					{
						return m_panel.PanelType;
					}
				}
				/// <summary>
				/// 
				/// </summary>
				/// <param name="item"></param>
				/// <param name="location"></param>
				void IPanel.SetItemLocation( ToolStripItem item, Point location )
				{
					this.SetItemLocation( item, location );
				}
				/// <summary>
				/// 
				/// </summary>
				/// <param name="item"></param>
				/// <returns></returns>
				ToolStripItemPlacement IPanel.GetItemPlacement(ToolStripItem item)
				{
					if (item != null)
					{
						MenuDropDown parent = m_panel.m_parent;

						if(parent!=null && parent.DisplayedItems.Contains(item))
						{
							return ToolStripItemPlacement.Main;
						}
					}
					return ToolStripItemPlacement.None;
				}
				#endregion

				#region Fields
				/// <summary>
				/// Owner panel
				/// </summary>
				Panel m_panel;
				/// <summary>
				/// Layout engine
				/// </summary>
				static PanelStripLayout m_layout;
				/// <summary> Minimum size of PanelStrip. </summary>
				private Size m_szMinimum;
				#endregion
			}
			#endregion
		}
		#endregion

		#region *** MenuDropDownLayout
		class MenuDropDownLayout : LayoutEngine
		{
			#region Constructors
			public MenuDropDownLayout()
				: base()
			{
			}
			#endregion

			#region Overrides
			public override bool Layout( object container, LayoutEventArgs layoutEventArgs )
			{
				bool bResult = false;

				MenuDropDown dropDown = container as MenuDropDown;

				if( dropDown != null )
				{
					dropDown.MainPanel.PerformLayout( dropDown.MainItemsBounds );
					dropDown.AuxPanel.PerformLayout( dropDown.AuxItemsBounds );
					dropDown.SystemPanel.PerformLayout( dropDown.SystemItemsBounds );

					bResult = dropDown.AutoSize;
				}

				return bResult;
			}
			#endregion
		}
		#endregion

		#region Constructors
		public MenuDropDown( ToolStripItem ownerItem )
			: base( ownerItem )
		{
			m_borders = new Padding( 6, 18, 6, 27 );
			m_systemPadding = new Padding( 2, 4, 2, 4 );

			base.ShowItemToolTips = false;
		}
		#endregion

		#region Properties
		/// <summary>
		/// 
		/// </summary>
		public override ISite Site
		{
			get
			{
				return base.Site;
			}
			set
			{
				base.Site = value;

				if (value != null)
				{
					this.MainPanel.ToolStrip.Site = new PanelSite(this.MainPanel.ToolStrip, value);
					this.AuxPanel.ToolStrip.Site = new PanelSite(this.AuxPanel.ToolStrip, value);
					this.SystemPanel.ToolStrip.Site = new PanelSite(this.SystemPanel.ToolStrip, value);
				}
				else
				{
					this.MainPanel.ToolStrip.Site = null;
					this.AuxPanel.ToolStrip.Site = null;
					this.SystemPanel.ToolStrip.Site = null;
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public override LayoutEngine LayoutEngine
		{
			get
			{
				if( m_layoutEngine == null )
				{
					m_layoutEngine = new MenuDropDownLayout();
				}
				return m_layoutEngine;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public override Size MinimumSize
		{
			get
			{
				Size size = base.MinimumSize;
				return size;
			}
			set
			{
				base.MinimumSize = value;
				PerformLayout();
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public override Size MaximumSize
		{
			get
			{
				return base.MaximumSize;
			}
			set
			{
				base.MaximumSize = value;
				PerformLayout();
			}
		}
		/// <summary>
		/// 
		/// </summary>
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ), Browsable( false )]
		public override ToolStripItemCollection Items
		{
			get
			{
				return this.MainItems;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		[Browsable( false ), DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public ToolStripItemCollection MainItems
		{
			get
			{
				return this.MainPanel.Items;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		[Browsable( false ), DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public ToolStripItemCollection AuxItems
		{
			get
			{
				return this.AuxPanel.Items;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		[Browsable( false ), DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public ToolStripItemCollection SystemItems
		{
			get
			{
				return this.SystemPanel.Items;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		internal Padding Borders
		{
			get
			{
				int bottom = Math.Max( m_borders.Bottom, this.SysItemsSize.Height );
				return new Padding( m_borders.Left, m_borders.Top, m_borders.Right, bottom );// m_borders;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		internal Padding SystemPadding
		{
			get
			{
				return m_systemPadding;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		[Category( "Menu panels" )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Content )]
		[TypeConverter( typeof( ExpandableObjectConverter ) )]
		public Panel MainPanel
		{
			get
			{
				if( m_pMain == null )
				{
					m_pMain = new Panel( this, PanelType.Main );
				}
				return m_pMain;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		[Category( "Menu panels" )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Content )]
		[TypeConverter( typeof( ExpandableObjectConverter ) )]
		public Panel AuxPanel
		{
			get
			{
				if( m_pAux == null )
				{
					m_pAux = new Panel( this, PanelType.Auxiliary );
				}

				return m_pAux;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		[Category( "Menu panels" )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Content )]
		[TypeConverter( typeof( ExpandableObjectConverter ) )]
		public Panel SystemPanel
		{
			get
			{
				if( m_pSystem == null )
				{
					m_pSystem = new Panel( this, PanelType.System );
				}
				return m_pSystem;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		internal ToolStrip ParentToolStrip
		{
			get
			{
				if (this.OwnerItem != null)
				{
					return this.OwnerItem.GetCurrentParent();
				}
				return null;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		Size MainItemsSize
		{
			get
			{
				if( m_szMainItems.IsEmpty )
				{
					m_szMainItems = this.MainPanel.PreferredSize;
				}

				return m_szMainItems;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		Size AuxItemsSize
		{
			get
			{
				if( m_szAuxItems.IsEmpty )
				{
					m_szAuxItems = this.AuxPanel.PreferredSize;
				}
				
				return m_szAuxItems;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		Size SysItemsSize
		{
			get
			{
				if( m_szSysItems.IsEmpty && this.SystemItems.Count > 0 )
				{
					m_szSysItems = this.SystemPanel.PreferredSize;

					m_szSysItems.Width += m_systemPadding.Horizontal;
					m_szSysItems.Height += m_systemPadding.Vertical;
				}
				return m_szSysItems;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		internal Rectangle MainItemsBounds
		{
			get
			{
				if( m_rcMain.IsEmpty && MainItemsSize.Width > 0 )
				{
					Size szClient = this.ClientSize;
					Padding borders = this.Borders;

					if( this.RightToLeft == RightToLeft.Yes )
					{
						m_rcMain.X = borders.Left;

						int iAuxItemsWidth = AuxItemsSize.Width;

						if( iAuxItemsWidth > 0 )
						{
							m_rcMain.X += SEPARATOR_WIDTH + iAuxItemsWidth;
						}

						m_rcMain.Width = szClient.Width - m_rcMain.X - borders.Right;
					}
					else
					{
						m_rcMain.X = borders.Left;
						m_rcMain.Width = this.MainItemsSize.Width;
					}

					m_rcMain.Y = borders.Top;
					m_rcMain.Height = this.ClientSize.Height - borders.Vertical;
				}

				return m_rcMain;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		internal Rectangle AuxItemsBounds
		{
			get
			{
				if( m_rcAux.IsEmpty && AuxItemsSize.Width > 0 )
				{
					Size szClient = this.ClientSize;
					Padding borders = this.Borders;

					if( this.RightToLeft == RightToLeft.Yes )
					{
						m_rcAux.X = borders.Left;
						m_rcAux.Width = this.AuxItemsSize.Width;
					}
					else
					{
						m_rcAux.X = borders.Left;

						int iMainItemsWidth = MainItemsSize.Width;

						if( iMainItemsWidth > 0 )
						{
							m_rcAux.X += iMainItemsWidth + SEPARATOR_WIDTH;
						}

						m_rcAux.Width = szClient.Width - m_rcAux.X - borders.Right;
					}

					m_rcAux.Y = borders.Top;
					m_rcAux.Height = szClient.Height - borders.Vertical;
				}
				
				return m_rcAux;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		internal Rectangle SystemItemsBounds
		{
			get
			{
				if( m_rcSys.IsEmpty )
				{
					Size szClient = this.ClientSize;
					Padding pdSystem = this.SystemPadding;

					m_rcSys.Width = szClient.Width - pdSystem.Horizontal;
					m_rcSys.Height = this.Borders.Bottom - pdSystem.Vertical;

					m_rcSys.X = pdSystem.Left;
					m_rcSys.Y = szClient.Height - pdSystem.Bottom - m_rcSys.Height;
				}
				return m_rcSys;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		protected override ToolStripItemCollection DisplayedItems
		{
			get
			{
				if( m_displayedItems == null )
				{
					m_displayedItems = new ToolStripItemCollection( this, new ToolStripItem[] { } );
				}
				return m_displayedItems;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		[ DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ), Browsable( false ) ]
		public override RightToLeft RightToLeft
		{
			get
			{
				return base.RightToLeft;
			}
			set
			{
				base.RightToLeft = value;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		[ DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		[ Browsable( false )]
		public new ToolStripLayoutStyle LayoutStyle
		{
			get
			{
				return base.LayoutStyle;
			}
			set
			{
				base.LayoutStyle = value;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		[ Browsable( false ) ]
		[ DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ) ]
		public new Padding Padding
		{
			get
			{
				return base.Padding;	
			}
			set
			{
				base.Padding = value;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		[DefaultValue(false)]
		public new bool ShowItemToolTips
		{
			get
			{
				return base.ShowItemToolTips;
			}
			set
			{
				base.ShowItemToolTips = value;
			}
		}
		#endregion

		#region Overrides
		/// <summary>
		/// 
		/// </summary>
		/// <param name="proposedSize"></param>
		/// <returns></returns>
		public override Size GetPreferredSize( Size proposedSize )
		{
			Size szResult = Size.Empty;

			Size szMainItems = this.MainItemsSize;
			Size szAuxItems = this.AuxItemsSize;
			Size szSysItems = this.SysItemsSize;

			int iAuxItemsWidth = szAuxItems.Width;
			int iMainItemsWidth = szMainItems.Width;

			if( iAuxItemsWidth > 0 && iMainItemsWidth > 0 )
			{
				szResult.Width += SEPARATOR_WIDTH;
			}
			szResult.Width += ( iAuxItemsWidth + iMainItemsWidth );

			szResult.Height = Math.Max( szMainItems.Height, szAuxItems.Height );

			if( szResult.Height <= this.Padding.Vertical )
			{
				szResult.Height = 20;
			}

			if( szResult.Width < szSysItems.Width )
			{
				szResult.Width = szSysItems.Width;
			}

			Padding borders = this.Borders;
			szResult.Width += borders.Horizontal;
			szResult.Height += borders.Vertical;

			return szResult;
		}
		/// <summary>
		/// 
		/// </summary>
		protected override void SetDisplayedItems()
		{
			this.DisplayedItems.Clear();

			AddDisplayedItems( this.MainPanel, this.MainItems, this.MainItemsBounds );
			AddDisplayedItems( this.AuxPanel, this.AuxItems, this.AuxItemsBounds );
			AddDisplayedItems( this.SystemPanel, this.SystemItems, this.SystemItemsBounds );

			base.SetDisplayedItems();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnSizeChanged( EventArgs e )
		{
			m_rcAux = Rectangle.Empty;
			m_rcMain = Rectangle.Empty;

			base.OnSizeChanged( e );

			this.Region = RendererUtils.GetRoundedRegion( new Rectangle( Point.Empty, this.Size ), 2 );
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnLayout( LayoutEventArgs e )
		{
			m_szMainItems = Size.Empty;
			m_szAuxItems = Size.Empty;
			m_szSysItems = Size.Empty;

			m_rcMain = Rectangle.Empty;
			m_rcAux = Rectangle.Empty;
			m_rcSys = Rectangle.Empty;

			base.OnLayout( e );
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPaint( PaintEventArgs e )
		{
			base.OnPaint( e );

			RibbonControlAdvHeader.RibbonControlAdvHeaderRenderer renderer = this.Renderer as RibbonControlAdvHeader.RibbonControlAdvHeaderRenderer;

			if( renderer != null )
			{
				MainPanel.OnPaint( e, renderer );
				AuxPanel.OnPaint( e, renderer );
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="mea"></param>
		protected override void OnMouseMove( MouseEventArgs mea )
		{
			base.OnMouseMove( mea );

			MainPanel.OnMouseMove( mea );
			AuxPanel.OnMouseMove( mea );
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseLeave( EventArgs e )
		{
			base.OnMouseLeave( e );

			MainPanel.OnMouseLeave( e );
			AuxPanel.OnMouseLeave( e );
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected override void OnOpening(CancelEventArgs e)
		{
			e.Cancel = (this.OwnerItem.Owner is RibbonControlAdvHeader &&((this.OwnerItem.Owner as RibbonControlAdvHeader).RibbonStyle == RibbonStyle.Office2013||
							(this.OwnerItem.Owner as RibbonControlAdvHeader).RibbonStyle == RibbonStyle.Office2010));

			base.OnOpening(e);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="mea"></param>
		protected override void OnMouseDown( MouseEventArgs mea )
		{
			base.OnMouseDown( mea );

			MainPanel.OnMouseDown( mea );
			AuxPanel.OnMouseDown( mea );
		}
		/// <summary> Resets timers and releases mouse capture. </summary>
		/// <param name="mea"></param>
		protected override void OnMouseUp( MouseEventArgs mea )
		{
			base.OnMouseUp( mea );

			this.Capture = false;
		}
		/// <summary> Handles release of mouse capture. </summary>
		/// <param name="e"></param>
		protected override void OnMouseCaptureChanged( EventArgs e )
		{
			base.OnMouseCaptureChanged( e );

			MainPanel.OnMouseCaptureChanged( e );
			AuxPanel.OnMouseCaptureChanged( e );
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnRendererChanged( EventArgs e )
		{
			base.OnRendererChanged( e );

			this.MainPanel.ToolStrip.Renderer = this.Renderer;
			this.AuxPanel.ToolStrip.Renderer = this.Renderer;
			this.SystemPanel.ToolStrip.Renderer = this.Renderer;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnItemAdded( ToolStripItemEventArgs e )
		{
			base.OnItemAdded( e );
		}
		#endregion

		#region ShouldSerialize & Reset methods
		/// <summary>
		/// 
		/// </summary>
		new void ResetMinimumSize()
		{
			MinimumSize = Size.Empty;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		bool ShouldSerializeMinimumSize()
		{
			return ( MinimumSize != Size.Empty );
		}
		#endregion

		#region Implementation
		/// <summary>
		/// 
		/// </summary>
		/// <param name="items"></param>
		void AddDisplayedItems( Panel panel, ICollection items, Rectangle rect )
		{
			int iItemsHeight = panel.GetItemsHeight();
			int iItemsNeededHeight = rect.Height;

			bool bIsScroll = false;

			if( iItemsHeight > iItemsNeededHeight )
			{
				bIsScroll = ( iItemsHeight - panel.ScrollPositionInternal > iItemsNeededHeight );
			}

			if( bIsScroll && panel.PanelType != PanelType.System )
			{
				rect.Height -= SCROLL_BUTTON_HEIGHT;
			}

			foreach( ToolStripItem item in items )
			{
				if (item.Available && rect.Contains(item.Bounds))
				{
					this.DisplayedItems.Add( item );
				}
			}
		}
		/// <summary> Call RedrawWindow method to Repaint Scroll buttons. </summary>
		internal void RefreshScroll()
		{
			RedrawWindowFlags flags = RedrawWindowFlags.RDW_FRAME | RedrawWindowFlags.RDW_INVALIDATE;
			WindowsAPI.RedrawWindow( this.Handle, IntPtr.Zero, IntPtr.Zero, flags );
		}
		#endregion

		#region Fields
		Padding m_borders;
		Padding m_systemPadding;

		Panel m_pMain;
		Panel m_pAux;
		Panel m_pSystem;

		MenuDropDownLayout m_layoutEngine;

		Size m_szMainItems = Size.Empty;
		Size m_szAuxItems = Size.Empty;
		Size m_szSysItems = Size.Empty;

		Rectangle m_rcMain = Rectangle.Empty;
		Rectangle m_rcAux = Rectangle.Empty;
		Rectangle m_rcSys = Rectangle.Empty;

		ToolStripItemCollection m_displayedItems;
		#endregion
	}
	#endregion
}
#endif