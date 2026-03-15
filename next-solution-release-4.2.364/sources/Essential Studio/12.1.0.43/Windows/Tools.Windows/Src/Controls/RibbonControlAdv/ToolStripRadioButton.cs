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
using System.CodeDom;
using System.Collections;
using System.ComponentModel.Design.Serialization;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Syncfusion.Windows.Forms.Tools.Win32API;

namespace Syncfusion.Windows.Forms.Tools
{
	/// <summary> 
	/// ToolStripRadioButton class, that represents RadioButton item on ToolStrip.
	/// </summary>
	[ToolboxItem( false )]
	[ DesignerSerializer( typeof( ToolStripRadioButton.RadioButtonCodeDomSerializer ), typeof( CodeDomSerializer ) ) ]
	[System.Windows.Forms.Design.ToolStripItemDesignerAvailability( System.Windows.Forms.Design.ToolStripItemDesignerAvailability.ToolStrip )]
	[ToolboxBitmap(typeof(Syncfusion.Windows.Forms.Tools.ToolStripRadioButton), "ToolboxIcons.RadioButtonItem.bmp")]
	public class ToolStripRadioButton : ToolStripButton
	{
		#region Constants
		/// <summary>
		/// 
		/// </summary>
		internal const int UNCHECKED = 0;
		/// <summary>
		/// 
		/// </summary>
		internal const int UNCHECKED_SELECTED = 1;
		/// <summary>
		/// 
		/// </summary>
		internal const int UNCHECKED_PRESSED = 2;
		/// <summary>
		/// 
		/// </summary>
		internal const int CHECKED = 3;
		/// <summary>
		/// 
		/// </summary>
		internal const int CHECKED_SELECTED = 4;
		/// <summary>
		/// 
		/// </summary>
		internal const int CHECKED_PRESSED = 5;
		#endregion

		#region Initialization
		/// <summary>
		/// 
		/// </summary>
		static ToolStripRadioButton()
		{
			m_blRadioButtonUncheckedBackGround = new Blend();
			m_blRadioButtonUncheckedBackGround.Positions = new float[] { 0.0F, 0.27F, 0.27F, 1.0F };
			m_blRadioButtonUncheckedBackGround.Factors = new float[] { 0.0F, 0.5F, 0.5F, 1.0F };

			m_blRadioButtonCheckedBackGround = new Blend();
			m_blRadioButtonCheckedBackGround.Positions = new float[] { 0.0F, 0.65F, 0.65F, 1.0F };
			m_blRadioButtonCheckedBackGround.Factors = new float[] { 0.0F, 0.5F, 0.5F, 1.0F };
		}
		/// <summary>
		/// 
		/// </summary>
		public ToolStripRadioButton()
			: base()
		{
		}
		~ToolStripRadioButton()
		{
			Clear();
		}
		#endregion

		#region Nested classes
		/// <summary>
		/// Class for layout information of a <see cref="T:Syncfusion.Windows.Forms.Tools.ToolStripRadioButton"></see>.
		/// </summary>
		internal class RadioButtonInternalLayout
		{
			#region Constants
			/// <summary>
			/// Minimum CheckMark height.
			/// </summary>
			internal const int MIN_CHECKMARK_HEIGHT = 12;
            /// <summary>
            /// Minimum CheckMark height for 125 DPI.
            /// </summary>
            internal const int DPI_125_MIN_CHECKMARK_HEIGHT = 16;
            /// Minimum CheckMark height for 150 DPI.
            /// </summary>
            internal const int DPI_150_MIN_CHECKMARK_HEIGHT = 20;
			/// <summary>
			/// Minimum CheckMark width.
			/// </summary>
			internal const int MIN_CHECKMARK_WIDTH = 12;
            /// <summary>
            /// Minimum CheckMark width for 125 DPI.
            /// </summary>
            internal const int DPI_125_MIN_CHECKMARK_WIDTH = 16;
            /// Minimum CheckMark width for 150 DPI.
            /// </summary>
            internal const int DPI_150_MIN_CHECKMARK_WIDTH = 20;
			/// <summary>
			/// Control border width.
			/// </summary>
			private const int BORDER_WIDTH = 2;
			/// <summary>
			/// Offset from CheckMark to Text.
			/// </summary>
			private const int CHECKMARK_OFFSET = 2;
			#endregion

			#region Initialization
			/// <summary>
			/// 
			/// </summary>
			/// <param name="item"></param>
			public RadioButtonInternalLayout( ToolStripRadioButton item )
			{
				m_Item = item;
			}
			#endregion

			#region Implementation
			/// <summary>
			/// Gets Y-coordinate of check mark part of the <see cref="T:Syncfusion.Windows.Forms.Tools.ToolStripRadioButton"></see>.
			/// </summary>
			/// <returns></returns>
			private int GetCoordinateYOfCheckMark()
			{
				int iY = 0;

				switch( m_Item.CheckAlign )
				{
					case ContentAlignment.TopCenter:
					case ContentAlignment.TopLeft:
					case ContentAlignment.TopRight:
						iY = BORDER_WIDTH + m_Item.Padding.Top;
						break;

					case ContentAlignment.MiddleCenter:
					case ContentAlignment.MiddleLeft:
					case ContentAlignment.MiddleRight:
						int iHeight = m_Item.Height - 2 * BORDER_WIDTH - m_Item.Padding.Vertical;
						iY = m_Item.Padding.Top + BORDER_WIDTH + ( iHeight - MIN_CHECKMARK_HEIGHT ) / 2;
						break;

					case ContentAlignment.BottomCenter:
					case ContentAlignment.BottomLeft:
					case ContentAlignment.BottomRight:
						iY = m_Item.Height - BORDER_WIDTH - m_Item.Padding.Bottom - MIN_CHECKMARK_HEIGHT;
						break;
				}

				return iY;
			}
			/// <summary>
			/// Gets X-coordinate of check mark part of the <see cref="T:Syncfusion.Windows.Forms.Tools.ToolStripRadioButton"></see>.
			/// </summary>
			/// <returns></returns>
			private int GetCoordinateXOfCheckMark()
			{
				int iX = 0;
				bool bRightToLeft = m_Item.RightToLeft == RightToLeft.Yes;

				switch( m_Item.CheckAlign )
				{
					case ContentAlignment.TopLeft:
					case ContentAlignment.MiddleLeft:
					case ContentAlignment.BottomLeft:
						iX = bRightToLeft ?
							m_Item.Width - BORDER_WIDTH - m_Item.Padding.Right - MIN_CHECKMARK_WIDTH :
							BORDER_WIDTH + m_Item.Padding.Left;
						break;

					case ContentAlignment.TopCenter:
					case ContentAlignment.MiddleCenter:
					case ContentAlignment.BottomCenter:
						int iWidth = m_Item.Width - 2 * BORDER_WIDTH - m_Item.Padding.Horizontal;
						iX = m_Item.Padding.Left + BORDER_WIDTH + ( iWidth - MIN_CHECKMARK_WIDTH ) / 2;
						break;

					case ContentAlignment.TopRight:
					case ContentAlignment.MiddleRight:
					case ContentAlignment.BottomRight:
						iX = bRightToLeft ?
							BORDER_WIDTH + m_Item.Padding.Left :
							m_Item.Width - BORDER_WIDTH - m_Item.Padding.Right - MIN_CHECKMARK_WIDTH;
						break;
				}

				return iX;
			}
			/// <summary>
			/// Gets Y-coordinate of text part of the <see cref="T:Syncfusion.Windows.Forms.Tools.ToolStripRadioButton"></see>.
			/// </summary>
			/// <returns></returns>
			private int GetCoordinateYOfText()
			{
				int iY = BORDER_WIDTH + m_Item.Padding.Top;

				if( m_Item.CheckAlign == ContentAlignment.TopCenter )
				{
					iY += CheckMarkSize.Height + CHECKMARK_OFFSET;
				}

				return iY;
			}
			/// <summary>
			/// Gets X-coordinate of text part of the <see cref="T:Syncfusion.Windows.Forms.Tools.ToolStripRadioButton"></see>.
			/// </summary>
			/// <returns></returns>
			private int GetCoordinateXOfText()
			{
				int iX = BORDER_WIDTH + m_Item.Padding.Left;

				if( m_Item.RightToLeft == RightToLeft.Yes )
				{
					switch( m_Item.CheckAlign )
					{
						case ContentAlignment.BottomRight:
						case ContentAlignment.MiddleRight:
						case ContentAlignment.TopRight:
							iX += CheckMarkSize.Width + CHECKMARK_OFFSET;
							break;
					}
				}
				else
				{
					switch( m_Item.CheckAlign )
					{
						case ContentAlignment.BottomLeft:
						case ContentAlignment.MiddleLeft:
						case ContentAlignment.TopLeft:
							iX += CheckMarkSize.Width + CHECKMARK_OFFSET;
							break;
					}
				}

				return iX;
			}
			/// <summary>
			/// Gets height of text part of the <see cref="T:Syncfusion.Windows.Forms.Tools.ToolStripRadioButton"></see>.
			/// </summary>
			/// <returns></returns>
			private int GetHeightOfText()
			{
				int iHeight = m_Item.Height - 2 * BORDER_WIDTH - m_Item.Padding.Vertical;

				if( GetIsAlignmentInTwoLines() )
				{
					iHeight -= ( CheckMarkSize.Height + CHECKMARK_OFFSET );
				}

				return iHeight;
			}
			/// <summary>
			/// Gets width of text part of the <see cref="T:Syncfusion.Windows.Forms.Tools.ToolStripRadioButton"></see>.
			/// </summary>
			/// <returns></returns>
			private int GetWidthOfText()
			{
				int iWidth = m_Item.Width - 2 * BORDER_WIDTH - m_Item.Padding.Horizontal;

				if( !( GetIsAlignmentInTwoLines() || m_Item.CheckAlign == ContentAlignment.MiddleCenter ) )
				{
					iWidth -= ( CheckMarkSize.Width + CHECKMARK_OFFSET );
				}

				return iWidth;
			}
			#endregion

			#region Properties
			/// <summary>
			/// Get preferred size of a <see cref="T:Syncfusion.Windows.Forms.Tools.ToolStripRadioButton"></see>. 
			/// </summary>
			public Size PreferredSize
			{
				get
				{
					if( m_Item != null )
					{
						if( m_szLastPreferred == Size.Empty )
						{
							int iWidth = 2 * BORDER_WIDTH + m_Item.Padding.Horizontal;
							int iHeight = 2 * BORDER_WIDTH + m_Item.Padding.Vertical;

							int iCheckMarkHeight = CheckMarkSize.Height;
							int iCheckMarkWidth = CheckMarkSize.Width;
							int iTextHeight = TextSize.Height;

							if( GetIsAlignmentInTwoLines() )
							{
								iHeight += iCheckMarkHeight + iTextHeight + CHECKMARK_OFFSET;
								iWidth += Math.Max( TextSize.Width, iCheckMarkWidth );
							}
							else
							{
								iWidth += iCheckMarkWidth + TextSize.Width + CHECKMARK_OFFSET;
								iHeight += Math.Max( iTextHeight, iCheckMarkHeight );
							}

							m_szLastPreferred = new Size( iWidth, iHeight );
						}
					}

					return m_szLastPreferred;
				}
			}
			/// <summary>
			/// Gets if CheckMark is positioned above or under the Text in <see cref="T:Syncfusion.Windows.Forms.Tools.ToolStripRadioButton"></see>.
			/// </summary>
			/// <returns></returns>
			private bool GetIsAlignmentInTwoLines()
			{
				bool bResult = false;

				if( m_Item.CheckAlign == ContentAlignment.TopCenter ||
					m_Item.CheckAlign == ContentAlignment.BottomCenter )
				{
					bResult = true;
				}

				return bResult;
			}
			/// <summary>
			/// Gets bounds of check mark part of the <see cref="T:Syncfusion.Windows.Forms.Tools.ToolStripRadioButton"></see>.
			/// </summary>
			internal Rectangle CheckMarkRectangle
			{
				get
				{
					if( m_Item != null && m_rcCheckMark == Rectangle.Empty )
					{
						int iTop = GetCoordinateYOfCheckMark();
						int iLeft = GetCoordinateXOfCheckMark();

						m_rcCheckMark = new Rectangle( new Point( iLeft, iTop ), CheckMarkSize );
					}

					return m_rcCheckMark;
				}
			}
			/// <summary>
			/// Gets bounds of text part of the <see cref="T:Syncfusion.Windows.Forms.Tools.ToolStripRadioButton"></see>.
			/// </summary>
			internal Rectangle TextRectangle
			{
				get
				{
					if( m_Item != null && m_rcText == Rectangle.Empty )
					{
						int iTop = GetCoordinateYOfText();
						int iHeight = GetHeightOfText();
						int iLeft = GetCoordinateXOfText();
						int iWidth = GetWidthOfText();

						m_rcText = new Rectangle( new Point( iLeft, iTop ), new Size( iWidth, iHeight ) );
					}

					return m_rcText;
				}
			}
			/// <summary>
			/// Gets size of check mark part of the <see cref="T:Syncfusion.Windows.Forms.Tools.ToolStripRadioButton"></see>.
			/// </summary>
			internal Size CheckMarkSize
			{
                get
                {
                    if (m_Item != null && m_szCheckMark == Size.Empty)
                    {
                        using (Graphics g = Graphics.FromImage(new Bitmap(10, 10)))
                        {
                            if (g.DpiX > 96)
                            {
                                m_szCheckMark = RadioButtonRenderer.GetGlyphSize(g, System.Windows.Forms.VisualStyles.RadioButtonState.CheckedNormal);
                            }
                            else
                            {
                                m_szCheckMark = new Size(MIN_CHECKMARK_WIDTH, MIN_CHECKMARK_HEIGHT);
                            }
                        }
                    }

                    return m_szCheckMark;
                }
			}
			/// <summary>
			/// Gets size of text part of the <see cref="T:Syncfusion.Windows.Forms.Tools.ToolStripRadioButton"></see>.
			/// </summary>
			internal Size TextSize
			{
				get
				{
					if( m_Item != null && m_szText == Size.Empty )
					{
						m_szText = TextRenderer.MeasureText( m_Item.Text, m_Item.Font );
					}

					return m_szText;
				}
			}
			#endregion

			#region Fields
			/// <summary>
			/// Last preferred size of <see cref="T:Syncfusion.Windows.Forms.Tools.ToolStripRadioButton"></see>.
			/// </summary>
			private Size m_szLastPreferred;
			/// <summary>
			/// <summary> Instance of <see cref="T:Syncfusion.Windows.Forms.Tools.ToolStripRadioButton"></see>. </summary>
			/// </summary>
			private ToolStripRadioButton m_Item;
			/// <summary>
			///  Gets bounds of check mark part of the <see cref="T:Syncfusion.Windows.Forms.Tools.ToolStripRadioButton"></see>.
			/// </summary>
			private Rectangle m_rcCheckMark;
			/// <summary>
			///  Gets bounds of text part of the <see cref="T:Syncfusion.Windows.Forms.Tools.ToolStripRadioButton"></see>.
			/// </summary>
			private Rectangle m_rcText;
			/// <summary>
			/// Size of check mark part of the <see cref="T:Syncfusion.Windows.Forms.Tools.ToolStripRadioButton"></see>.
			/// </summary>
			private Size m_szCheckMark;
			/// <summary>
			/// Size of text part of the <see cref="T:Syncfusion.Windows.Forms.Tools.ToolStripRadioButton"></see>.
			/// </summary>
			private Size m_szText;
			#endregion
		}
		/// <summary>
		/// 
		/// </summary>
		internal class RadioButtonCodeDomSerializer :	CodeDomSerializer
		{
			#region Class overrides
			/// <summary>
			/// 
			/// </summary>
			/// <param name="manager"></param>
			/// <param name="value"></param>
			/// <returns></returns>
			public override object Serialize( IDesignerSerializationManager manager, object value )
			{
				CodeDomSerializer baseClassSerializer = ( CodeDomSerializer ) manager.GetSerializer( typeof( ToolStripRadioButton ).BaseType, typeof( CodeDomSerializer ) );

				object objCode = baseClassSerializer.Serialize( manager, value );

				if( objCode is CodeStatementCollection )
				{
					CodeStatementCollection collStatements = objCode as CodeStatementCollection;
					int iGroupIDIndex = -1;
					int iCheckedIndex = -1;

					if( collStatements.Count > 0 )
					{
						for( int i = 0, count = collStatements.Count; i < count; i++ )
						{
							CodeAssignStatement casCodeStatement = collStatements[ i ] as CodeAssignStatement;

							if( casCodeStatement != null )
							{
								CodePropertyReferenceExpression refexprProperty = casCodeStatement.Left as CodePropertyReferenceExpression;

								if( refexprProperty != null )
								{
									if( refexprProperty.PropertyName == "Checked" )
										iCheckedIndex = i;
									else if( refexprProperty.PropertyName == "GroupID" )
										iGroupIDIndex = i;
								}
							}
						}

						if( iCheckedIndex != -1 && iGroupIDIndex != -1 )
						{
							CodeAssignStatement casTemp = collStatements[ iCheckedIndex ] as CodeAssignStatement;
							collStatements[ iCheckedIndex ] = collStatements[ iGroupIDIndex ];
							collStatements[ iGroupIDIndex ] = casTemp;
						}
					}
				}

				return objCode;
			}
			#endregion
		}
		#endregion

		#region Overrides
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnClick( EventArgs e )
		{
			this.CheckOnClick = false;
			this.Checked = true;

			base.OnClick( e );
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="constrainingSize"></param>
		/// <returns></returns>
		public override Size GetPreferredSize( Size constrainingSize )
		{
			return InternalLayout.PreferredSize;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnLayout( LayoutEventArgs e )
		{
			m_InternalLayout = new RadioButtonInternalLayout( this );

			base.OnLayout( e );
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnRightToLeftChanged( EventArgs e )
		{
			m_InternalLayout = new RadioButtonInternalLayout( this );

			base.OnRightToLeftChanged( e );
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPaint( PaintEventArgs e )
		{
			if( base.Owner != null )
			{
				PaintRadioButtonBackground( new ToolStripItemRenderEventArgs( e.Graphics, this ) );
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnOwnerChanged( EventArgs e )
		{
			base.OnOwnerChanged( e );

			if( !this.Checked )
				CheckCurrentUncheckedButton();
			else
				CheckCurrentButton();
		}
		#endregion

		#region Implemetation
		/// <summary>
		/// Clean up specified resources being used.
		/// </summary>
		private void Clear()
		{
		}
		/// <summary>
		/// Method updates state of all <see cref="T:System.Windows.Forms.RadioButton"></see> controls which
		/// have the same owner as current <see cref="T:System.Windows.Forms.RadioButton"></see> control.
		/// </summary>
		protected internal virtual void CheckCurrentButton()
		{
			if( this.Checked )
			{
				ToolStrip tsParent = this.Owner;

				if( tsParent != null )
				{
					ToolStripItemCollection collItems = tsParent.Items;

					for( int i = 0, count = collItems.Count; i < count; i++ )
					{
						ToolStripItem tsItem = collItems[ i ];

						if( ( tsItem != this ) && ( tsItem is ToolStripRadioButton ) )
						{
							ToolStripRadioButton tsRadioButton = tsItem as ToolStripRadioButton;

							if( tsRadioButton.Checked && m_iGroupID == tsRadioButton.GroupID )
							{
								tsRadioButton.Checked = false;
							}
						}
					}
				}
			}
		}
		/// <summary>
		/// Methods checks <see cref="T:System.Windows.Forms.RadioButton"></see> control
		/// if there are no checked <see cref="T:System.Windows.Forms.RadioButton"></see> controls with same GroupID and owner.
		/// </summary>
		protected internal virtual void CheckCurrentUncheckedButton()
		{
			bool bIsCheckedExists = false;
			ToolStrip tsParent = this.Owner;

			if( tsParent != null )
			{
				ToolStripItemCollection collItems = tsParent.Items;

				for( int i = 0, count = collItems.Count; i < count; i++ )
				{
					ToolStripItem tsItem = collItems[ i ];

					if( ( tsItem != this ) && ( tsItem is ToolStripRadioButton ) )
					{
						ToolStripRadioButton tsRadioButton = tsItem as ToolStripRadioButton;

						if( tsRadioButton.Checked && m_iGroupID == tsRadioButton.GroupID )
						{
							bIsCheckedExists = true;
							break;
						}
					}
				}

				if( !bIsCheckedExists )
					this.Checked = true;
			}
		}
		/// <summary>
		/// Methods checks first <see cref="T:System.Windows.Forms.RadioButton"></see> control
		/// if there are no checked <see cref="T:System.Windows.Forms.RadioButton"></see> controls with same GroupID and owner among the remaining controls.
		/// </summary>
		protected internal virtual void CheckRemainingUncheckedButtons()
		{
			ToolStrip tsParent = this.Owner;

			if( this.Checked && tsParent != null )
			{
				ToolStripItemCollection collItems = tsParent.Items;

				for( int i = 0, count = collItems.Count; i < count; i++ )
				{
					ToolStripItem tsItem = collItems[ i ];

					if( ( tsItem != this ) && ( tsItem is ToolStripRadioButton ) )
					{
						ToolStripRadioButton tsRadioButton = tsItem as ToolStripRadioButton;

						if( !tsRadioButton.Checked && m_iGroupID == tsRadioButton.GroupID )
						{
							tsRadioButton.Checked = true;
							break;
						}
					}
				}
			}
		}
		/// <summary>
		/// Draws the background of the ToolStripRadioButton.
		/// </summary>
		/// <param name="e"></param>
		private void PaintRadioButtonBackground( ToolStripItemRenderEventArgs e )
		{
			ToolStripRadioButton tsRadioButton = e.Item as ToolStripRadioButton;

			if( tsRadioButton != null )
			{
				Rectangle rc = new Rectangle( Point.Empty, e.Item.Size );

				if( rc.Width > 0 && rc.Height > 0 )
				{
					Rectangle rcBackground = GetButtonBackgroundRect( e.Item, rc );

					if( rcBackground.Width > 0 && rcBackground.Height > 0 )
					{
						Graphics g = e.Graphics;

						int iCheckMarkID = -1;

						Color clText = Color.Empty;
						Color clExternalBorder = Color.Empty;
						Color clInternalBorder = Color.Empty;

						Rectangle rcBorder = tsRadioButton.InternalLayout.CheckMarkRectangle;
						Rectangle rcRadioButtonInScreen = tsRadioButton.GetCurrentParent().RectangleToScreen( tsRadioButton.Bounds );

						bool bSelected = tsRadioButton.Selected;
						bool bPressed = tsRadioButton.Pressed;
						bool bChecked = tsRadioButton.Checked;
						bool bPressedAndNotSelected = !rcRadioButtonInScreen.Contains( Cursor.Position );
						bool bDisabled = GetIsDisabled( tsRadioButton );

						rcBorder.Width -= 1;
						rcBorder.Height -= 1;

						if( !bDisabled )
						{
							if( bSelected && bPressed )
							{
								clExternalBorder = this.ColorTable.RadioButtonBorderSelected;
								clInternalBorder = this.ColorTable.RadioButtonInternalBorderPressed;
								iCheckMarkID = bChecked ? ToolStripRadioButton.CHECKED_PRESSED : ToolStripRadioButton.UNCHECKED_PRESSED;
							}
							else if( bPressedAndNotSelected || !bSelected )
							{
								clExternalBorder = this.ColorTable.RadioButtonExternalBorder;
								clInternalBorder = this.ColorTable.RadioButtonInternalBorder;
								iCheckMarkID = bChecked ? ToolStripRadioButton.CHECKED : ToolStripRadioButton.UNCHECKED;
							}
							else
							{
								clExternalBorder = this.ColorTable.RadioButtonBorderSelected;
								clInternalBorder = this.ColorTable.RadioButtonInternalBorderSelected;
								iCheckMarkID = bChecked ? ToolStripRadioButton.CHECKED_SELECTED : ToolStripRadioButton.UNCHECKED_SELECTED;
							}
						}
						else
						{
							clExternalBorder = SystemColors.GrayText;
							clInternalBorder = Color.White;
							iCheckMarkID = bChecked ? ToolStripRadioButton.CHECKED : ToolStripRadioButton.UNCHECKED;
						}

						SmoothingMode smOldMode = g.SmoothingMode;
						g.SmoothingMode = SmoothingMode.AntiAlias;

						// Draw ToolStripRadioButton state.
						Image imgCheckMark = GetBitmapForCheckMark( iCheckMarkID );
						bool bDisposeImage = false;

						if( bDisabled )
						{
							imgCheckMark = Office12ToolStripRenderer.CreateDisabledImage( imgCheckMark );
							bDisposeImage = true;
						}

						rcBorder.Inflate( -1, -1 );
						g.DrawImage( imgCheckMark, rcBorder.X, rcBorder.Y );

						if( bDisposeImage )
						{
							imgCheckMark.Dispose();
						}

						// Draw External border.
						rcBorder.Inflate( 1, 1 );
						using( Pen penExternal = new Pen( clExternalBorder ) )
						{
							g.DrawEllipse( penExternal, rcBorder );
						}

						// Draw Internal border.
						rcBorder.Inflate( -1, -1 );
						using( Pen penInternal = new Pen( clInternalBorder ) )
						{
							g.DrawEllipse( penInternal, rcBorder );
						}


						g.SmoothingMode = smOldMode;

						// Draw text.
						clText = bDisabled ? SystemColors.GrayText : this.ColorTable.RibbonTabText;
						TextRenderer.DrawText( g, tsRadioButton.Text, tsRadioButton.Font, tsRadioButton.InternalLayout.TextRectangle, clText, GetTextFormatFlags( tsRadioButton.TextAlign ) );
					}
				}
			}
		}
		/// <summary>
		/// Indicates if ToolStripItem is disabled.
		/// </summary>
		/// <param name="item"></param>
		private bool GetIsDisabled( ToolStripItem item )
		{
			return ( !item.Enabled || !item.GetCurrentParent().Enabled );
		}
		/// <summary>
		/// Gets TextFormatFlags according to Text alignment.
		/// </summary>
		/// <param name="alignment"></param>
		/// <returns></returns>
		private TextFormatFlags GetTextFormatFlags( ContentAlignment alignment )
		{
			TextFormatFlags flags;

			switch( alignment )
			{
				case ContentAlignment.BottomCenter:
					flags = TextFormatFlags.Bottom | TextFormatFlags.HorizontalCenter;
					break;

				case ContentAlignment.BottomLeft:
					flags = TextFormatFlags.Bottom | TextFormatFlags.Left;
					break;

				case ContentAlignment.BottomRight:
					flags = TextFormatFlags.Bottom | TextFormatFlags.Right;
					break;

				case ContentAlignment.MiddleCenter:
					flags = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter;
					break;

				case ContentAlignment.MiddleLeft:
					flags = TextFormatFlags.VerticalCenter | TextFormatFlags.Left;
					break;

				case ContentAlignment.MiddleRight:
					flags = TextFormatFlags.VerticalCenter | TextFormatFlags.Right;
					break;

				case ContentAlignment.TopCenter:
					flags = TextFormatFlags.Top | TextFormatFlags.HorizontalCenter;
					break;

				case ContentAlignment.TopLeft:
					flags = TextFormatFlags.Top | TextFormatFlags.Left;
					break;

				case ContentAlignment.TopRight:
					flags = TextFormatFlags.Top | TextFormatFlags.Right;
					break;

				default:
					flags = TextFormatFlags.Default;
					break;
			}

			return flags;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		/// <param name="rc"></param>
		/// <returns></returns>
		protected Rectangle GetButtonBackgroundRect( ToolStripItem item, Rectangle rc )
		{
			Rectangle rcResult = rc;

			if( !( item is ToolStripOverflowButton ) )
			{
				rcResult.Inflate( -1, -1 );
			}

			return rcResult;
		}
		/// <summary>
		/// Gets image for ToolStripRadioButton by specific ID.
		/// </summary>
		/// <param name="iCheckMarkID"> Image identifier. </param>
		/// <returns> Image that have just created or image from hashtable. </returns>
		private Image GetBitmapForCheckMark( int iCheckMarkID )
		{
			Bitmap bmp = null;

			switch( iCheckMarkID )
			{
				case ToolStripRadioButton.CHECKED:
					bmp = RadioButtonChecked;
					break;
				case ToolStripRadioButton.CHECKED_SELECTED:
					bmp = RadioButtonCheckedSelected;
					break;
				case ToolStripRadioButton.CHECKED_PRESSED:
					bmp = RadioButtonCheckedPressed;
					break;
				case ToolStripRadioButton.UNCHECKED:
					bmp = RadioButtonUnchecked;
					break;
				case ToolStripRadioButton.UNCHECKED_SELECTED:
					bmp = RadioButtonUncheckedSelected;
					break;
				case ToolStripRadioButton.UNCHECKED_PRESSED:
					bmp = RadioButtonUncheckedPressed;
					break;
				default:
					bmp = RadioButtonChecked;
					break;
			}

			return bmp;
		}
		/// <summary>
		/// Method creates image for checked ToolStripRadioButton.
		/// Image varies on several ToolStripRadioButton checked states.
		/// </summary>
		/// <param name="pRadioButtonBorder"> The border color of ToolStripRadioButton border. </param>
		/// <param name="pRadiobButtonBackground"> The starting color of the gradient used to draw the background of ToolStripRadioButton. </param>
		/// <param name="pDiluteCoefficient"> The dilute coefficient of inside border of ToolStripRadiobButton. </param>
		/// <returns></returns>
		private Bitmap CreateImageChecked( Color pRadioButtonBorder, Color pRadioButtonBackground, int pDiluteCoefficient, bool pRadioButtonSelected )
		{
            Bitmap bmp = new Bitmap(ToolStripRadioButton.RadioButtonInternalLayout.MIN_CHECKMARK_WIDTH - 3, ToolStripRadioButton.RadioButtonInternalLayout.MIN_CHECKMARK_HEIGHT - 3);
                  using (Graphics g = Graphics.FromImage(new Bitmap(10, 10)))
                  {
                      if (g.DpiX > 120)
                      {
                          bmp = new Bitmap(ToolStripRadioButton.RadioButtonInternalLayout.DPI_150_MIN_CHECKMARK_WIDTH - 3, ToolStripRadioButton.RadioButtonInternalLayout.DPI_150_MIN_CHECKMARK_HEIGHT - 3);
                      }
                      else if (g.DpiX > 96)
                      {
                          bmp = new Bitmap(ToolStripRadioButton.RadioButtonInternalLayout.DPI_125_MIN_CHECKMARK_WIDTH - 3, ToolStripRadioButton.RadioButtonInternalLayout.DPI_125_MIN_CHECKMARK_HEIGHT - 3);
                      }
                  }
                  using (Graphics g = Graphics.FromImage(bmp))
                  {
                      Rectangle rcBorder;
                      if (g.DpiX > 120)
                      {
                          rcBorder = new Rectangle(0, 0, ToolStripRadioButton.RadioButtonInternalLayout.DPI_150_MIN_CHECKMARK_WIDTH - 3, ToolStripRadioButton.RadioButtonInternalLayout.DPI_150_MIN_CHECKMARK_HEIGHT - 3);
                      }
                      else if (g.DpiX > 96)
                      {
                          rcBorder = new Rectangle(0, 0, ToolStripRadioButton.RadioButtonInternalLayout.DPI_125_MIN_CHECKMARK_WIDTH - 3, ToolStripRadioButton.RadioButtonInternalLayout.DPI_125_MIN_CHECKMARK_HEIGHT - 3);
                      }
                      else
                      {
                          rcBorder = new Rectangle(0, 0, ToolStripRadioButton.RadioButtonInternalLayout.MIN_CHECKMARK_WIDTH - 3, ToolStripRadioButton.RadioButtonInternalLayout.MIN_CHECKMARK_HEIGHT - 3);
                      }

                      GraphicsState gState = g.Save();

                      g.Clear(Color.Transparent);
                      g.SmoothingMode = SmoothingMode.AntiAlias;

                      rcBorder.Inflate(-1, -1);
                      using (Pen pen = new Pen(Office12ColorTable.GetAlphaBlendedColor(pRadioButtonBorder, Color.White, pDiluteCoefficient)))
                      {
                          g.DrawEllipse(pen, rcBorder);
                      }

                      rcBorder.Inflate(-1, -1);
                      using (GraphicsPath path = GetCircularPath(rcBorder))
                      {
                          using (PathGradientBrush brush = new PathGradientBrush(path))
                          {
                              brush.CenterColor = Color.White;
                              brush.CenterPoint = new PointF((float)rcBorder.X + 1, (float)rcBorder.Y);
                              brush.SurroundColors = new Color[] { pRadioButtonBackground };
                              if (!pRadioButtonSelected)
                              {
                                  brush.Blend = m_blRadioButtonCheckedBackGround;
                              }

                              g.FillPath(brush, path);
                          }
                      }

                      if (g.DpiX > 96)
                      {
                          using (GraphicsPath path = GetCircularPath(rcBorder))
                          {
                              using (Pen pen = new Pen(this.ColorTable.RadioButtonCheckedInsideBorder))
                              {
                                  g.DrawPath(pen, path);
                              }
                          }
                      }
                      else
                      {
                          using (GraphicsPath path = GetRectangularPath(rcBorder))
                          {
                              using (Pen pen = new Pen(this.ColorTable.RadioButtonCheckedInsideBorder))
                              {
                                  g.DrawPath(pen, path);
                              }
                          }
                      }
                  }

			return bmp;
		}
		/// <summary>
		/// Method creates image for unchecked ToolStripRadioButton.
		/// Image varies on several ToolStripRadioButton unchecked states.
		/// </summary>
		/// <param name="pRadioButtonBorder"> The border color of ToolStripRadioButton border. </param>
		/// <returns></returns>
		private Bitmap CreateImageUnchecked( Color pRadioButtonBorder, Color pRadioButtonGradientBegin )
		{
			Bitmap bmp = new Bitmap( ToolStripRadioButton.RadioButtonInternalLayout.MIN_CHECKMARK_WIDTH - 3, ToolStripRadioButton.RadioButtonInternalLayout.MIN_CHECKMARK_HEIGHT - 3 );

			using( Graphics g = Graphics.FromImage( bmp ) )
			{
				Rectangle rcBorder = new Rectangle( 0, 0, ToolStripRadioButton.RadioButtonInternalLayout.MIN_CHECKMARK_WIDTH - 3, ToolStripRadioButton.RadioButtonInternalLayout.MIN_CHECKMARK_HEIGHT - 3 );

				g.Clear( Color.Transparent );

				using( GraphicsPath path = GetCircularPath( rcBorder ) )
				{
					using( PathGradientBrush brush = new PathGradientBrush( path ) )
					{
						brush.CenterColor = pRadioButtonGradientBegin;
						brush.CenterPoint = new PointF( ( float ) rcBorder.Right - 1, ( float ) rcBorder.Bottom - 1 );
						brush.SurroundColors = new Color[] { pRadioButtonBorder };
						brush.Blend = m_blRadioButtonUncheckedBackGround;

						g.FillPath( brush, path );
					}
				}
			}

			return bmp;
		}
		/// <summary>
		/// Gets circular path by specific rectangle.
		/// </summary>
		/// <param name="rc"> Rectangle for circular path. </param>
		/// <returns> Path based on ellipse. </returns>
		private GraphicsPath GetCircularPath( Rectangle rc )
		{
			GraphicsPath path = new GraphicsPath();
			path.AddEllipse( rc );
			path.CloseFigure();

			return path;
		}
		/// <summary>
		/// Gets rectangular path by specific rectangle.
		/// </summary>
		/// <param name="rc"> Rectangle for rectangular path. </param>
		/// <returns> Path based on rectangular area. </returns>
		private GraphicsPath GetRectangularPath( Rectangle rc )
		{
			GraphicsPath path = new GraphicsPath();

			path.AddLine( rc.X + 1, rc.Y, rc.Right - 1, rc.Y );
			path.AddLine( rc.Right - 1, rc.Y, rc.Right - 1, rc.Y + 1 );
			path.AddLine( rc.Right - 1, rc.Y + 1, rc.Right, rc.Y + 1 );
			path.AddLine( rc.Right, rc.Y + 1, rc.Right, rc.Bottom - 1 );
			path.AddLine( rc.Right, rc.Bottom - 1, rc.Right - 1, rc.Bottom - 1 );
			path.AddLine( rc.Right - 1, rc.Bottom - 1, rc.Right - 1, rc.Bottom );
			path.AddLine( rc.Right - 1, rc.Bottom, rc.X + 1, rc.Bottom );
			path.AddLine( rc.X + 1, rc.Bottom, rc.X + 1, rc.Bottom - 1 );
			path.AddLine( rc.X + 1, rc.Bottom - 1, rc.X, rc.Bottom - 1 );
			path.AddLine( rc.X, rc.Bottom - 1, rc.X, rc.Y + 1 );
			path.AddLine( rc.X, rc.Y + 1, rc.X + 1, rc.Y + 1 );
			path.AddLine( rc.X + 1, rc.Y + 1, rc.X + 1, rc.Y );
			path.CloseFigure();

			return path;
		}
		#endregion

		#region Properties
		/// <summary>
		/// 
		/// </summary>
		[Browsable( false )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public override Image BackgroundImage
		{
			get
			{
				return base.BackgroundImage;
			}
			set
			{
				base.BackgroundImage = value;
			}
		}
		/// <summary>
		/// Gets or sets the background image layout used for the <see cref="T:System.Windows.Forms.ToolStripItem"></see>.
		/// </summary>
		/// <returns> One of the <see cref="T:System.Windows.Forms.ImageLayout"></see> values. The default value is <see cref="F:System.Windows.Forms.ImageLayout.Tile"></see>. </returns>
		[Browsable( false )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public override ImageLayout BackgroundImageLayout
		{
			get
			{
				return base.BackgroundImageLayout;
			}
			set
			{
				base.BackgroundImageLayout = value;
			}
		}
		/// <summary>
		/// Gets or sets the background color for the item.
		/// </summary>
		/// <returns>A <see cref="T:System.Drawing.Color"></see> that represents the background color of the item. The default is the value of the <see cref="P:System.Windows.Forms.Control.DefaultBackColor"></see> property.</returns>
		[Browsable( false )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public override Color BackColor
		{
			get
			{
				return base.BackColor;
			}
			set
			{
				base.BackColor = value;
			}
		}
		/// <summary>Gets or sets the horizontal and vertical alignment of the check mark on a <see cref="T:Syncfusion.Windows.Forms.Tools.ToolStripRadioButton"></see> control.</summary>
		/// <returns>One of the <see cref="T:System.Drawing.ContentAlignment"></see> values. The default value is MiddleLeft.</returns>
		/// <exception cref="T:System.ComponentModel.InvalidEnumArgumentException">The value assigned is not one of the <see cref="T:System.Drawing.ContentAlignment"></see> enumeration values. </exception>
		/// <PermissionSet><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" /></PermissionSet>
		[Description( "Gets or sets the horizontal and vertical alignment of the check mark on a ToolStripRadioButton control." )]
		[Category( "Appearance" )]
		[Bindable( true )]
		[DefaultValue( ContentAlignment.MiddleLeft )]
		public ContentAlignment CheckAlign
		{
			get
			{
				return m_calignCheckAlignment;
			}
			set
			{
				if( m_calignCheckAlignment != value )
				{
					m_calignCheckAlignment = value;
					m_InternalLayout = new RadioButtonInternalLayout( this );

					this.Size = GetPreferredSize( Size.Empty );
					Invalidate();
				}
			}
		}
		/// <summary>
		/// Gets instance of Office12ColorTable class.
		/// If <see cref="T:Syncfusion.Windows.Forms.Tools.ToolStripCheckBox"></see> owner's renderer has Office12ToolStripRenderer type, 
		/// property returns current Office12ColorTable instance, otherwise - returns default Office12ColorTable one.
		/// </summary>
		internal Office12ColorTable ColorTable
		{
			get
			{
				Office12ColorTable cltblColorTable = null;
				ToolStrip tsOwner = this.Owner;

				if( tsOwner != null )
				{
					Office12ToolStripRenderer renderer = tsOwner.Renderer as Office12ToolStripRenderer;

					if( renderer != null )
					{
						cltblColorTable = renderer.OfficeColorTable;
					}
				}

				if( cltblColorTable == null )
				{
					cltblColorTable = new Office12ColorTable();
				}

				return cltblColorTable;
			}
		}
		/// <summary> 
		/// Gets or sets a value indicating whether the <see cref="T:Syncfusion.Windows.Forms.Tools.ToolStripRadioButton"></see> is pressed or not pressed.
		/// </summary>
		/// <returns> true if the <see cref="T:Syncfusion.Windows.Forms.Tools.ToolStripRadioButton"></see> is pressed in or not pressed in; otherwise, false. The default is false. </returns>
		[Description( " Gets or sets a value indicating whether the ToolStripRadioButton is pressed or not pressed." )]
		[Category( "Appearance" )]
		[DefaultValue( false )]
		public new bool Checked
		{
			get
			{
				return base.Checked;
			}
			set
			{
				base.Checked = value;
				CheckCurrentButton();
			}
		}
		/// <summary> 
		/// Gets or sets a value indicating whether the <see cref="T:Syncfusion.Windows.Forms.Tools.ToolStripRadioButton"></see> is in the pressed or not pressed state by default, or is in an indeterminate state.
		/// </summary>
		/// <returns> One of the <see cref="T:System.Windows.Forms.CheckState"></see> values. The default is Unchecked. </returns>
		[Description( "Gets or sets a value indicating whether the ToolStripRadio is in the pressed or not pressed state by default, or is in an indeterminate state." )]
		[Category( "Appearance" )]
		[DefaultValue( 0 )]
		[Browsable( false )]
		public new CheckState CheckState
		{
			get { return base.CheckState; }
			set { base.CheckState = value; }
		}
		/// <summary>
		/// This property is not used, because <see cref="T:Syncfusion.Windows.Forms.Tools.ToolStripRadioButton"></see> automatically appears pressed in and not pressed in when clicked.
		/// </summary>
		[Browsable( false )]
		[Category( "Behavior" )]
		[DefaultValue( false )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public new bool CheckOnClick
		{
			get { return false; }
			set
			{
				value = false;
			}
		}
		/// <summary>
		/// Gets or sets whether text and images are displayed on a <see cref="T:Syncfusion.Windows.Forms.Tools.ToolStripRadioButton"></see>.
		/// </summary>
		/// <returns> One of the <see cref="T:System.Windows.Forms.ToolStripItemDisplayStyle"></see> values. The default is <see cref="F:System.Windows.Forms.ToolStripItemDisplayStyle.ImageAndText"></see>. </returns>
		[Browsable( false )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public new ToolStripItemDisplayStyle DisplayStyle
		{
			get { return base.DisplayStyle; }
			set { base.DisplayStyle = value; }
		}
		/// <summary>
		/// Gets or sets the foreground color of the item.
		/// </summary>
		/// <returns> The foreground <see cref="T:System.Drawing.Color"></see> of the item. The default is the value of the <see cref="P:System.Windows.Forms.Control.DefaultForeColor"></see> property. </returns>
		[Browsable( false )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public override Color ForeColor
		{
			get
			{
				return base.ForeColor;
			}
			set
			{
				base.ForeColor = value;
			}
		}
		/// <summary>
		/// Gets or sets group's identifier which is used to create groups of <see cref="T:Syncfusion.Windows.Forms.Tools.ToolStripRadioButton"></see> controls
		/// on the same parent.
		/// </summary>
		[Category( "Behavior" )]
		[Description( "Gets or sets group's identifier which is used to create groups of ToolStripRadioButton controls on the same parent." )]
		[DefaultValue( 0 )]
		public virtual int GroupID
		{
			get
			{
				return m_iGroupID;
			}
			set
			{
				if( m_iGroupID != value )
				{
					CheckRemainingUncheckedButtons();

					m_iGroupID = value;

					if( !this.Checked )
						CheckCurrentUncheckedButton();
					else
						CheckCurrentButton();
				}
			}
		}
		/// <summary>
		/// Gets or sets the image that is displayed on a <see cref="T:Syncfusion.Windows.Forms.Tools.ToolStripRadioButton"></see>.
		/// </summary>
		/// <returns> The <see cref="T:System.Drawing.Image"></see> to be displayed. </returns>
		[Browsable( false )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public override Image Image
		{
			get
			{
				return base.Image;
			}
			set
			{
				base.Image = value;
			}
		}
		/// <summary>
		/// Gets or sets the alignment of the image on a <see cref="T:Syncfusion.Windows.Forms.Tools.ToolStripRadioButton"></see>.
		/// </summary>
		/// <returns> One of the <see cref="T:System.Drawing.ContentAlignment"></see> values. The default is <see cref="F:System.Drawing.ContentAlignment.MiddleLeft"></see>. </returns>
		[Browsable( false )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public new ContentAlignment ImageAlign
		{
			get
			{
				return base.ImageAlign;
			}
			set
			{
				base.ImageAlign = value;
			}
		}
		/// <summary>
		/// Gets or sets a value indicating whether an image on a <see cref="T:Syncfusion.Windows.Forms.Tools.ToolStripRadioButton"></see> is automatically resized to fit in a container.
		/// </summary>
		/// <returns> One of the <see cref="T:System.Windows.Forms.ToolStripItemImageScaling"></see> values. The default is <see cref="F:System.Windows.Forms.ToolStripItemImageScaling.SizeToFit"></see>. </returns>
		[Browsable( false )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public new ToolStripItemImageScaling ImageScaling
		{
			get
			{
				return base.ImageScaling;
			}
			set
			{
				base.ImageScaling = value;
			}
		}
		/// <summary>
		/// Gets or sets the color to treat as transparent in a <see cref="T:Syncfusion.Windows.Forms.Tools.ToolStripRadioButton"></see> image.
		/// </summary>
		/// <returns> One of the <see cref="T:System.Drawing.Color"></see> values. </returns>
		[Browsable( false )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public new Color ImageTransparentColor
		{
			get
			{
				return base.ImageTransparentColor;
			}
			set
			{
				base.ImageTransparentColor = value;
			}
		}
		/// <summary>
		/// Gets or sets Internal layout instance.
		/// </summary>
		internal RadioButtonInternalLayout InternalLayout
		{
			get
			{
				if( m_InternalLayout == null )
				{
					m_InternalLayout = new RadioButtonInternalLayout( this );
				}

				return m_InternalLayout;
			}
			set
			{
				m_InternalLayout = value;
			}
		}
		/// <summary>
		/// Gets bitmap for checked ToolStripRadioButton.
		/// </summary>
		internal Bitmap RadioButtonChecked
		{
			get
			{
				return CreateImageChecked(this.ColorTable.RadioButtonExternalBorder, this.ColorTable.RadioButtonCheckedBackgroundGradientEnd, 200, false);
			}
		}
		/// <summary>
		/// Gets bitmap for checked selected ToolStripRadioButton.
		/// </summary>
		internal Bitmap RadioButtonCheckedSelected
		{
			get
			{
				return CreateImageChecked( this.ColorTable.RadioButtonBorderSelected, this.ColorTable.RadioButtonCheckedSelectedBackgroundGradientEnd, 180, true );
			}
		}
		/// <summary>
		/// Gets bitmap for checked pressed ToolStripRadioButton.
		/// </summary>
		internal Bitmap RadioButtonCheckedPressed
		{
			get
			{
				return CreateImageChecked( this.ColorTable.RadioButtonBorderSelected, this.ColorTable.RadioButtonCheckedPressedBackgroundGradientEnd, 180, false );
			}
		}
		/// <summary>
		/// Gets bitmap for unchecked ToolStripRadioButton.
		/// </summary>
		internal Bitmap RadioButtonUnchecked
		{
			get
			{
				return CreateImageUnchecked( this.ColorTable.RadioButtonUncheckedGradientBegin, Color.White );
			}
		}
		/// <summary>
		/// Gets bitmap for unchecked selected ToolStripRadioButton.
		/// </summary>
		internal Bitmap RadioButtonUncheckedSelected
		{
			get
			{
				return CreateImageUnchecked( this.ColorTable.RadioButtonUncheckedSelectedGradientBegin, this.ColorTable.RadioButtonUncheckedSelectedGradientEnd );
			}
		}
		/// <summary>
		/// Gets bitmap for unchecked pressed ToolStripRadioButton.
		/// </summary>
		internal Bitmap RadioButtonUncheckedPressed
		{
			get
			{
				return CreateImageUnchecked( this.ColorTable.RadioButtonUncheckedPressedGradientBegin, this.ColorTable.RadioButtonUncheckedSelectedGradientEnd );
			}
		}	
		/// <summary>
		/// Mirrors automatically the <see cref="T:Syncfusion.Windows.Forms.Tools.ToolStripRadioButton"></see> image when the <see cref="P:System.Windows.Forms.ToolStripItem.RightToLeft"></see> property is set to <see cref="F:System.Windows.Forms.RightToLeft.Yes"></see>.
		/// </summary>
		/// <returns> true to automatically mirror the image; otherwise, false. The default is false. </returns>
		[Browsable( false )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public new bool RightToLeftAutoMirrorImage
		{
			get
			{
				return base.RightToLeftAutoMirrorImage;
			}
			set
			{
				base.RightToLeftAutoMirrorImage = value;
			}
		}
		/// <summary>
		/// Gets the orientation of text used on a <see cref="T:Syncfusion.Windows.Forms.Tools.ToolStripRadioButton"></see>.
		/// </summary>
		/// <returns> One of the <see cref="T:System.Windows.Forms.ToolStripTextDirection"></see> values. </returns>
		[Browsable( false )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public override ToolStripTextDirection TextDirection
		{
			get
			{
				return base.TextDirection;
			}
			set
			{
				base.TextDirection = value;
			}
		}
		/// <summary>
		/// Gets or sets the position of <see cref="T:Syncfusion.Windows.Forms.Tools.ToolStripRadioButton"></see> text and image relative to each other.
		/// </summary>
		/// <returns> One of the <see cref="T:System.Windows.Forms.TextImageRelation"></see> values. The default is <see cref="F:System.Windows.Forms.TextImageRelation.ImageBeforeText"></see>. </returns>
		[Browsable( false )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public new TextImageRelation TextImageRelation
		{
			get
			{
				return base.TextImageRelation;
			}
			set
			{
				base.TextImageRelation = value;
			}
		}
		#endregion

		#region Fields
		/// <summary>
		/// Internal layout instance.
		/// </summary>
		private RadioButtonInternalLayout m_InternalLayout;
		/// <summary>
		/// Horizontal and vertical alignment of the check mark on a <see cref="T:Syncfusion.Windows.Forms.Tools.ToolStripRadioButton"></see> control.
		/// </summary>
		private ContentAlignment m_calignCheckAlignment = ContentAlignment.MiddleLeft;
		/// <summary>
		/// Group's identifier which is used to create groups of <see cref="T:Syncfusion.Windows.Forms.Tools.ToolStripRadioButton"></see> controls
		/// on the same parent.
		/// </summary>
		private int m_iGroupID = 0;
		/// <summary>
		/// Blend for unchecked ToolStripRadioButton.
		/// </summary>
		private static Blend m_blRadioButtonUncheckedBackGround;
		/// <summary>
		/// Blend for checked ToolStripRadioButton.
		/// </summary>
		private static Blend m_blRadioButtonCheckedBackGround;
		#endregion
	}
}

#endif