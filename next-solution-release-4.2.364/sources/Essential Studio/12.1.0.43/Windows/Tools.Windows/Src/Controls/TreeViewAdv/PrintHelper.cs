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
using System.Drawing;
using System.Drawing.Printing;

using Syncfusion.Runtime.InteropServices;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Syncfusion.Windows.Forms.Tools
{
	public class PrintHelper
	{
		#region Fields
		/// <summary>
		/// 
		/// </summary>
		private PrintDocument m_pdDocument;
		/// <summary>
		/// 
		/// </summary>
		private Point m_ptPrintPosition;
		/// <summary>
		/// 
		/// </summary>
		private Image m_imgControl = null;
		/// <summary>
		/// 
		/// </summary>
		private int m_iNodeHeight = 0;
		/// <summary>
		/// 
		/// </summary>
		private PrintDirection m_pdCurrentDirection;
		/// <summary>
		/// 
		/// </summary>
		private int m_iScrollBarHeight = 0;
		/// <summary>
		/// 
		/// </summary>
		private int m_iScrollBarWidth = 0;
		/// <summary>
		/// 
		/// </summary>
		private int m_iPageNumber = 0;
		/// <summary>
		/// 
		/// </summary>
		private DateTime m_dtDateTime;
		/// <summary>
		/// 
		/// </summary>
		private string m_sTitle = string.Empty;
		#endregion

		#region Enums
		/// <summary>
		/// 
		/// </summary>
		private enum PrintDirection
		{
			/// <summary>
			/// 
			/// </summary>
			Horizontal,
			/// <summary>
			/// 
			/// </summary>
			Vertical
		}
		#endregion

		#region Initialization
		/// <summary>
		/// 
		/// </summary>
		public PrintHelper()
		{
			m_ptPrintPosition = Point.Empty;

			m_pdDocument = new PrintDocument();
			m_pdDocument.BeginPrint += new PrintEventHandler( OnPrintDocumentBeginPrint );
			m_pdDocument.PrintPage += new PrintPageEventHandler( OnPrintDocumentPrintPage );
		}
		#endregion 

		#region Event handlers
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnPrintDocumentBeginPrint( object sender, PrintEventArgs e )
		{
			m_ptPrintPosition = new Point( 0, 0 );
			m_pdCurrentDirection = PrintDirection.Horizontal;
			m_iPageNumber = 0;
			m_dtDateTime = DateTime.Now;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnPrintDocumentPrintPage( object sender, PrintPageEventArgs e )
		{
			Graphics g = e.Graphics;
			Rectangle rcSource = new Rectangle( m_ptPrintPosition, e.MarginBounds.Size );
			Rectangle rcDestination = e.MarginBounds;

			m_iPageNumber++;

			if( ( rcSource.Height % m_iNodeHeight ) > 0 )
			{
				rcSource.Height -= ( rcSource.Height % m_iNodeHeight );
			}

			g.DrawImage( m_imgControl, rcDestination, rcSource, GraphicsUnit.Pixel );


			// Check to see if we need more pages.
			if( ( m_imgControl.Height - m_iScrollBarHeight ) > rcSource.Bottom ||
				( m_imgControl.Width - m_iScrollBarWidth ) > rcSource.Right )
			{
				e.HasMorePages = true;
			}

			if( m_pdCurrentDirection == PrintDirection.Horizontal )
			{
				if( rcSource.Right < ( m_imgControl.Width - m_iScrollBarWidth ) )
				{
					m_ptPrintPosition.X += ( rcSource.Width + 1 );
				}
				else
				{
					m_ptPrintPosition.X = 0;
					m_ptPrintPosition.Y += ( rcSource.Height + 1 );
					m_pdCurrentDirection = PrintDirection.Vertical;
				}
			}
			else if( m_pdCurrentDirection == PrintDirection.Vertical && rcSource.Right < ( m_imgControl.Width - m_iScrollBarWidth ) )
			{
				m_pdCurrentDirection = PrintDirection.Horizontal;
				m_ptPrintPosition.X += ( rcSource.Width + 1 );
			}
			else
			{
				m_ptPrintPosition.Y += ( rcSource.Height + 1 );
			}

			using( Brush brush = new SolidBrush( Color.Black ) )
			{
				// Print footer.
				string sFooter = m_iPageNumber.ToString( System.Globalization.NumberFormatInfo.CurrentInfo );
				Font fontFooter = new Font( FontFamily.GenericSansSerif, 10f );
				SizeF szfFooter = g.MeasureString( sFooter, fontFooter );

				PointF ptBottomCenter = new PointF( e.PageBounds.Width / 2, e.MarginBounds.Bottom + ( ( e.PageBounds.Bottom - e.MarginBounds.Bottom ) / 2 ) );
				PointF ptFooterLocation = new PointF( ptBottomCenter.X - ( szfFooter.Width / 2 ), ptBottomCenter.Y - ( szfFooter.Height / 2 ) );

				g.DrawString( sFooter, fontFooter, brush, ptFooterLocation );

				// Print header.
				if( m_iPageNumber == 1 && m_sTitle.Length > 0 )
				{
					Font fontHeader = new Font( FontFamily.GenericSansSerif, 24f, FontStyle.Bold, GraphicsUnit.Point );
					SizeF szfHeader = g.MeasureString( m_sTitle, fontHeader );
					PointF ptHeaderLocation = new PointF( e.MarginBounds.Left, ( ( e.MarginBounds.Top - e.PageBounds.Top ) / 2 ) - ( szfHeader.Height / 2 ) );

					g.DrawString( m_sTitle, fontHeader, brush, ptHeaderLocation );
				}
			}
		}
		#endregion

		#region Public methods
		/// <summary>
		/// Shows a PrintPreview dialog displaying the Tree control passed in.
		/// </summary>
		/// <param name="pTreeView">TreeView for print preview.</param>
		/// <param name="pTitle">Title for document.</param>
		public void PrintPreviewTree( TreeViewAdv pTreeView, string pTitle )
		{
			m_sTitle = pTitle;
			PrepareTreeImage( pTreeView );

			PrintPreviewDialog dialog = new PrintPreviewDialog();

			dialog.Document = m_pdDocument;
			dialog.ShowDialog();
		}
        public Image ToImage(TreeViewAdv treeview)
        {
            this.PrepareTreeImage(treeview);
            return m_imgControl;
        }
		/// <summary>
		/// 
		/// </summary>
		/// <param name="pTreeView"></param>
		/// <param name="pTitle"></param>
		public void PrintTree( TreeViewAdv pTreeView, string pTitle )
		{
			m_sTitle = pTitle;
			PrepareTreeImage( pTreeView );

			PrintDialog dialog = new PrintDialog();

			dialog.Document = m_pdDocument;

			if( dialog.ShowDialog() == DialogResult.OK )
			{
				m_pdDocument.Print();
			}
		}
		#endregion

		#region Implementation
		/// <summary>
		/// Gets an image that shows the entire tree, not just what is visible on the form
		/// </summary>
		/// <param name="tree"></param>
		private void PrepareTreeImage( TreeViewAdv pTreeView )
		{
			if( pTreeView != null && pTreeView.Nodes.Count > 0 )
			{
				TreeViewAdv treeView = new TreeViewAdv();

                treeView.Root = pTreeView.Root.Clone();
                treeView.BackColor = pTreeView.BackColor;
                treeView.BackgroundImage = pTreeView.BackgroundImage;
                treeView.HideSelection = pTreeView.HideSelection;
                treeView.Indent = pTreeView.Indent;
                treeView.InteractiveCheckBoxes = pTreeView.InteractiveCheckBoxes;
                treeView.ItemHeight = pTreeView.ItemHeight;
                treeView.LineColor = pTreeView.LineColor;
                treeView.LineStyle = pTreeView.LineStyle;
                treeView.RightToLeft = pTreeView.RightToLeft;
                treeView.ShowLines = pTreeView.ShowLines;
                treeView.ShowRootLines = pTreeView.ShowRootLines;
                treeView.ShowCheckBoxes = pTreeView.ShowCheckBoxes;
                treeView.ShowOptionButtons = pTreeView.ShowOptionButtons;
                treeView.ShowPlusMinus = pTreeView.ShowPlusMinus;
                treeView.ThemesEnabled = pTreeView.ThemesEnabled;
                treeView.NodeCount = pTreeView.NodeCount;
                treeView.Font = pTreeView.Font;
                treeView.ForeColor = pTreeView.ForeColor;
                foreach( string name in pTreeView.BaseStyles.Keys )
                {
                    treeView.BaseStyles[ name ] = pTreeView.BaseStyles[ name ];
                }

                treeView.NodeStateImageList = pTreeView.NodeStateImageList;
                treeView.DefaultCollapseImageIndex = pTreeView.DefaultCollapseImageIndex;
                treeView.DefaultExpandImageIndex = pTreeView.DefaultExpandImageIndex;
                treeView.LeftImageList = pTreeView.LeftImageList;
                treeView.RightImageList = pTreeView.RightImageList;
                treeView.StateImageList = pTreeView.StateImageList;  

				m_iScrollBarWidth = treeView.Width - treeView.ClientSize.Width;
				m_iScrollBarHeight = treeView.Height - treeView.ClientSize.Height;

                int iHeight = 0;
                if (treeView.Nodes[0] != null)
                    iHeight = treeView.Nodes[0].Height;
                m_iNodeHeight = treeView.ItemHeight;

				int iWidth = treeView.Nodes[ 0 ].PrintTextBounds.Right;
				TreeNodeAdv node = treeView.Nodes[ 0 ].NextSelectableNode;

				while( node != null )
				{
                    m_iNodeHeight = node.Height;
                    iHeight += m_iNodeHeight;

                    if( treeView.RightToLeft == RightToLeft.No )
					{
                        if( node.PrintTextBounds.Right > iWidth )
					    {
						    iWidth = node.PrintTextBounds.Right;
					    }
                    }
                    else
                    {
                        if( node.PrintTextBounds.Right < 0 )
					    {
						    iWidth = ( -node.PrintTextBounds.Right ) + treeView.ClientRectangle.Width;
					    }
                    }
					node = node.NextSelectableNode;
				}

				//setup the tree to take the snapshot
				treeView.SelectedNode = null;
				treeView.Height = iHeight + m_iScrollBarHeight;
                //treeViewWidth
                treeView.Width=pTreeView.Width;
                foreach (KeyValuePair<Control, Point> customControl in pTreeView.CustomControlsImage)
                {
                    if (treeView.Width < customControl.Value.X + customControl.Key.Width)
                    {
                        treeView.Width = customControl.Value.X + customControl.Key.Width + 5;
                    }
                }                
				treeView.BorderStyle = BorderStyle.None;
				treeView.Dock = DockStyle.None;
				treeView.VScroll = false;
				treeView.HScroll = false;

                //m_imgControl = GetImage(treeView.Handle, treeView.ClientSize.Width, treeView.ClientSize.Height);

                m_imgControl = DrawImageWithCustomControls(treeView.Handle, treeView.ClientSize.Width, treeView.ClientSize.Height, pTreeView.CustomControlsImage);

                pTreeView.CustomControlsImage.Clear();

				//give the window time to update
				//Application.DoEvents();
			}
		}
		/// <summary>
		/// Returns an image of the specified width and height, of a control represented by handle.
		/// </summary>
		/// <param name="pHandle"></param>
		/// <param name="pWidth"></param>
		/// <param name="pHeight"></param>
		/// <returns></returns>
		private Image GetImage( IntPtr pHandle, int pWidth, int pHeight )
		{
			IntPtr screenDC = NativeMethods.GetDC( IntPtr.Zero );
			IntPtr bmp = NativeMethods.CreateCompatibleBitmap( screenDC, pWidth, pHeight );
			Image img = Bitmap.FromHbitmap( bmp );
			Graphics g = Graphics.FromImage( img );
			IntPtr hdc = g.GetHdc();

			NativeMethods.SendMessage( pHandle, 0x0318 /*WM_PRINTCLIENT*/, hdc, ( 0x00000010 | 0x00000004 | 0x00000002 ) );

			g.ReleaseHdc( hdc );
			NativeMethods.ReleaseDC( IntPtr.Zero, screenDC );

			return img;
		}

        private Image DrawImageWithCustomControls(IntPtr pHandle, int pWidth, int pHeight, Dictionary<Control, Point> customControls)
        {
            IntPtr screenDC = NativeMethods.GetDC(IntPtr.Zero);
            IntPtr bmp = NativeMethods.CreateCompatibleBitmap(screenDC, pWidth, pHeight);
            Image img = Bitmap.FromHbitmap(bmp);
            Graphics g = Graphics.FromImage(img);
            
            IntPtr hdc = g.GetHdc();

            NativeMethods.SendMessage(pHandle, 0x0318 /*WM_PRINTCLIENT*/, hdc, (0x00000010 | 0x00000004 | 0x00000002));

            g.ReleaseHdc(hdc);
            NativeMethods.ReleaseDC(IntPtr.Zero, screenDC);

            foreach (KeyValuePair<Control, Point> customControl in customControls)
            {
                TreeViewAdv treeview = customControl.Key.Parent as TreeViewAdv;               
                TreeNodeAdv node = treeview.Nodes[0];
                Point value = new Point();
                int iHeight = 0;
                while (node != null)
                {
                    m_iNodeHeight = node.Height;
                    if (node.CustomControl == customControl.Key)
                    {
                        if (node.Visible)
                       {
                            Image image = GetImage(customControl.Key.Handle, customControl.Key.ClientSize.Width, customControl.Key.ClientSize.Height);
                            if (customControl.Key is NumericUpDown || customControl.Key is ComboBoxAdv || customControl.Key is DateTimePickerAdv)
                            {
                                Panel panel1 = new Panel();
                                panel1.Size = customControl.Key.Size;
                                using (Graphics g1 = panel1.CreateGraphics())
                                {
                                    g1.FillEllipse(new SolidBrush(customControl.Key.BackColor), customControl.Key.Bounds);
                                }
                                System.Drawing.Bitmap bitmapforimage = new System.Drawing.Bitmap(panel1.Width, panel1.Height);
                                customControl.Key.DrawToBitmap(bitmapforimage, panel1.Bounds);
                                image = (Image)bitmapforimage;
                            }
                            value.X = customControl.Value.X;
                            value.Y = iHeight;
                            g.DrawImage(image, value);  
                        }
                    }
                   iHeight += m_iNodeHeight;
                    node = node.NextSelectableNode;
                }
            }              
         

            return img;
        }

		#endregion
	}
}