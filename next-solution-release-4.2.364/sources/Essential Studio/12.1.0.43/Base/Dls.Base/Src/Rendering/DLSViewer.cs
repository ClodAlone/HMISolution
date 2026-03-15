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
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

using Syncfusion.DLS;
using Syncfusion.Layouting;

using Graphics = System.Drawing.Graphics;
using PrintDocument = System.Drawing.Printing.PrintDocument;
using Size = System.Drawing.Size;
using SizeF = System.Drawing.SizeF;
#endregion

namespace Syncfusion.DLS.Rendering
{
  /// <summary>
  /// Represents a DLS veiwer that can be used to view DLS Documents.
  /// </summary>
  [ Syncfusion.Documentation.DocumentationExclude() ]
  [ToolboxItem(false)]
  public class DLSViewer : ScrollableControl
  {
    #region Class members
    /// <summary>
    /// 
    /// </summary>
    private DocumentLayouter m_docLayouter = new DocumentLayouter();
    /// <summary>
    /// 
    /// </summary>
    private IDocument m_doc = null;
    /// <summary>
    /// 
    /// </summary>
    private Graphics m_defGraphics;
    /// <summary>
    /// 
    /// </summary>
    private UnitsConvertor m_unitsConvertor;    
    /// <summary>
    /// 
    /// </summary>
    private DLSGraphics m_defCustomGraphics;
    /// <summary>
    /// 
    /// </summary>
    private Bitmap m_bitmapDefault;
    /// <summary>
    /// 
    /// </summary>
    private int m_currPageIndex = 0;
    #endregion

    #region Form controls
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private Container components = null;
    private PrintDocument m_printDoc = null;
    #endregion

    #region Class properties
    /// <summary>
    /// 
    /// </summary>
    public PrintDocument PrintDocument
    {
      get
      {
        if( m_printDoc == null )
        {
          m_printDoc = new PrintDocument();
          m_printDoc.PrintPage += new PrintPageEventHandler( OnPrintPage );
          m_printDoc.BeginPrint +=new PrintEventHandler(OnBeginPrint);
          m_printDoc.EndPrint += new PrintEventHandler(OnEndPrint);
          m_printDoc.PrinterSettings.MinimumPage = 1;
          m_printDoc.PrinterSettings.FromPage = 1;
          m_printDoc.PrinterSettings.ToPage = 1;
        }

        return m_printDoc;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public IDocument Document
    {
      get
      {
        return m_doc;
      }
      set
      {
        if( m_doc != value )
        {
          m_doc = value;
          UpdateDocument();
          CurrentPageIndex = 0;
        }
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public int PageCount
    {
      get
      {
        return m_docLayouter.Pages.Count;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public int CurrentPageIndex
    {
      get
      {
        return m_currPageIndex;
      }
      set
      {
        if( value != m_currPageIndex )
        {
          if( value != m_currPageIndex )
          {
            m_currPageIndex = value;
            ChangedCurrentPage();
          }
        }
      }
    }
    #endregion

    #region Class initialize/finalize methods
    /// <summary>
    /// 
    /// </summary>
    public DLSViewer()
    {
      // This call is required by the Windows.Forms Form Designer.
      InitializeComponent();
      
      SetStyle( ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, true );
      // TODO: Add any initialization after the InitializeComponent call
      m_bitmapDefault = new Bitmap( 1, 1 );
      m_defGraphics = Graphics.FromImage( m_bitmapDefault );
      m_defGraphics.PageUnit = GraphicsUnit.Point;
      m_defCustomGraphics = new DLSGraphics( m_defGraphics );
      m_unitsConvertor = new UnitsConvertor( m_defGraphics );
    }
    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
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
      // 
      // DLSViewer
      // 
      this.BackColor = System.Drawing.SystemColors.AppWorkspace;
      this.Size = new System.Drawing.Size(496, 448);

    }
    #endregion
    
    #region Class public methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="dlsGraphics"></param>
    public void AttachOtherGraphics( DLSGraphics dlsGraphics )
    {
      m_defCustomGraphics = dlsGraphics;
    }
    /// <summary>
    /// 
    /// </summary>
    public void Print()
    {
      PrintDialog dialogSettings = new PrintDialog();
      dialogSettings.Document = PrintDocument;
      dialogSettings.AllowSelection = true;
      dialogSettings.AllowSomePages = true;
      PrintDocument.PrinterSettings.PrintRange = PrintRange.AllPages;
         
      try
      {
        if( dialogSettings.ShowDialog() == DialogResult.OK )
        {
          PrintDocument.Print();
        }
      }
      catch( Exception ex )
      {
        Debug.Fail( ex.Message, "" );
      }
    }
    #endregion

    #region Class event handlers
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected virtual void OnPrintPage( object sender, PrintPageEventArgs e )
    {
      //e.Graphics.SetClip( e.MarginBounds );
      SetDPIFromGraphics( e.Graphics );
      
      if( m_docLayouter != null && m_docLayouter.Pages.Count > 0 )
      {
        Page page = m_docLayouter.Pages[ m_currPageIndex ];
        PageSetup setup = page.Setup;
        
        e.Graphics.PageUnit = GraphicsUnit.Point;

        /*
        e.Graphics.FillRectangle( new SolidBrush( Color.White ), 0, 0,
          Setup.PageSize.Width,
          Setup.PageSize.Height );
        e.Graphics.DrawRectangle( new Pen( Color.Black ), 0, 0,
          Setup.PageSize.Width,
          Setup.PageSize.Height );
        */
        
        m_defCustomGraphics.Update( e.Graphics );
        page.Draw( m_defCustomGraphics );

        if( m_currPageIndex < m_docLayouter.Pages.Count - 1 )
        {
          m_currPageIndex++;
          e.HasMorePages = true;
        }
        else
        {
          e.HasMorePages = false;
        }
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnBeginPrint( object sender, PrintEventArgs e )
    {
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnEndPrint( object sender, PrintEventArgs e )
    {
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="e"></param>
    protected override void OnPaint( PaintEventArgs e )
    {
      if( e.ClipRectangle != ClientRectangle )
      {
        Invalidate();
        return;
      }

      e.Graphics.PageUnit = GraphicsUnit.Pixel;
      e.Graphics.TranslateTransform( AutoScrollPosition.X, AutoScrollPosition.Y );
      SetDPIFromGraphics( e.Graphics );
      
      if( m_docLayouter != null && m_docLayouter.Pages.Count > 0 )
      {
        Page page = m_docLayouter.Pages[ m_currPageIndex ];
        PageSetup setup = page.Setup;
        
        e.Graphics.PageUnit = GraphicsUnit.Point;
        
        SizeF clientSize = m_unitsConvertor.ConvertFromPixels( Size, PrintUnits.Point );
        
        if( clientSize.Width > setup.PageSize.Width )
        {
          float dxLeft = ( clientSize.Width - setup.PageSize.Width ) / 2;
          e.Graphics.TranslateTransform( dxLeft, 0f );
        }

        // Draw page bounds
        DrawPageBounds( e.Graphics, setup );
        
        m_defCustomGraphics.Update( e.Graphics );
        page.Draw( m_defCustomGraphics );
        
      }
      
      base.OnPaint( e );
      
    }
    #endregion
    
    #region Class helper methods
    /// <summary>
    /// 
    /// </summary>
    private void ChangedCurrentPage()
    {
      Page page = m_docLayouter.Pages[ m_currPageIndex ];
      Size pageSize = Size.Ceiling(
        m_unitsConvertor.ConvertToPixels(
          page.Setup.PageSize,
          PrintUnits.Point
          )
        );
      
      AutoScrollMinSize = pageSize;
      Invalidate();
    }
    /// <summary>
    /// 
    /// </summary>
    private void UpdateDocument()
    {
      m_defCustomGraphics.Update( Graphics.FromImage( m_bitmapDefault ) );
      m_docLayouter.Layout( Document, m_defCustomGraphics );
      ChangedCurrentPage();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="g"></param>
    public void SetDPIFromGraphics( Graphics g )
    {
      if( g == null )
        throw new ArgumentNullException( "g" );

      if( g.DpiX == m_defGraphics.DpiX && g.DpiY == m_defGraphics.DpiY ) return;

      if( m_defGraphics != null )
        m_defGraphics.Dispose();

      if( m_bitmapDefault != null )
        m_bitmapDefault.Dispose();

      m_bitmapDefault = new Bitmap( 1, 1, g );
      m_defGraphics = Graphics.FromImage( m_bitmapDefault );
      m_defGraphics.PageUnit = GraphicsUnit.Point;
      m_defCustomGraphics = new DLSGraphics( m_defGraphics );
      m_unitsConvertor = new UnitsConvertor( m_defGraphics );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="graphics"></param>
    /// <param name="setup"></param>
    private void DrawPageBounds( Graphics graphics, PageSetup setup )
    {
      SizeF pageSize = setup.PageSize;
      
      graphics.FillRectangle( new SolidBrush( Color.White ), 0, 0,
        pageSize.Width,
        pageSize.Height );
      graphics.DrawRectangle( new Pen( Color.Black ), 0, 0,
        pageSize.Width,
        pageSize.Height );

      Pen shadowPen = new Pen( Color.Black, 2f );
      PointF[] points = new PointF[ 3 ]
        {
          new PointF( pageSize.Width + 2.5f, 2 ), 
          new PointF( pageSize.Width + 2.5f, pageSize.Height + 2 ), 
          new PointF( 2, pageSize.Height + 2 ) 
      };
      graphics.DrawLines( shadowPen, points );
    }
    #endregion
  }
}