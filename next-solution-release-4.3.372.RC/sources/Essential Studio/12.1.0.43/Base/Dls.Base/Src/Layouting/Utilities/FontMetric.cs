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
using System.Drawing;
using System.Runtime.InteropServices;

#endregion

namespace Syncfusion.Layouting
{
  /// <summary>
  /// Helper class, used for getting font ascent/descent
  /// </summary>
  public class FontMetric
  {
    #region Class members
    /// <summary>
    /// 
    /// </summary>
    private static Graphics m_bmpG;
    /// <summary>
    /// 
    /// </summary>
    private Graphics m_g;
    /// <summary>
    /// 
    /// </summary>
    private Font m_font;
    /// <summary>
    /// 
    /// </summary>
//    private OUTLINETEXTMETRIC m_metric;
    private UnitsConvertor m_convertor;
    #endregion

    #region Class properties
    /// <summary>
    /// 
    /// </summary>
    public double Ascent
    {
      get
      {
        int height = m_font.FontFamily.GetEmHeight( m_font.Style );
        int ascent = m_font.FontFamily.GetCellAscent( m_font.Style );
        double ascentInPoints = (double)( m_font.SizeInPoints * ascent ) / (double)height;
        
        switch( m_g.PageUnit )
        {
          case GraphicsUnit.Pixel:
            //return m_metric.otmMacAscent;
            return m_convertor.ConvertToPixels( (double)ascentInPoints, PrintUnits.Point );
          case GraphicsUnit.Point:
            //return m_convertor.ConvertFromPixels( m_metric.otmMacAscent, PrintUnits.Point );
            return (double)ascentInPoints;
          default:
            throw new NotImplementedException();
        }
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public double Descent
    {
      get
      {
        int height = m_font.FontFamily.GetEmHeight( m_font.Style );
        int descent = m_font.FontFamily.GetCellDescent( m_font.Style );
        double descentInPoints = (double)( m_font.SizeInPoints * descent ) / (double)height;
        
        switch( m_g.PageUnit )
        {                
          case GraphicsUnit.Pixel:
            //return m_metric.otmMacDescent;
            return m_convertor.ConvertToPixels( (double)descentInPoints, PrintUnits.Point );
          case GraphicsUnit.Point:
            //return m_convertor.ConvertFromPixels( m_metric.otmMacDescent, PrintUnits.Point );
            return (double)descentInPoints;
          default:
            throw new NotImplementedException();
        }
      }
    }
    /// <summary>
    /// 
    /// </summary>
    protected static Graphics BmpGraphics
    {
      get
      {
        if( m_bmpG == null )
        {
          Bitmap bmp = new Bitmap( 1, 1 );
          m_bmpG = Graphics.FromImage( bmp );
        }

        return m_bmpG;
      }
    }
    #endregion

    #region Class initialize/finalize methods
    /// <summary>
    /// 
    /// </summary>
    public FontMetric()
    {}
    /// <summary>
    /// 
    /// </summary>
    public FontMetric( Font font )
    {
      UpdateMetricData( null, font );
    }
    /// <summary>
    /// 
    /// </summary>
    public FontMetric( Font font, Graphics g )
    {
      UpdateMetricData( g, font );
    }
    #endregion

    #region Class public methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="g"></param>
    /// <param name="font"></param>
    public void UpdateMetricData( Graphics g, Font font )
    {
      if( font == null )
        throw new ArgumentNullException( "font" );

      m_font = font;
      UpdateMetricData( g );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="g"></param>
    public void UpdateMetricData( Graphics g )
    {
      m_g = ( g == null ) ? BmpGraphics : g;
      m_convertor = new UnitsConvertor( m_g );
      //ParseFontData();
    }
    #endregion
    
    #region Class helper methods
//    /// <summary>
//    /// 
//    /// </summary>
//    private void ParseFontData()
//    {
//      m_metric = new OUTLINETEXTMETRIC();
//      IntPtr fontDC = m_font.ToHfont();
//      IntPtr hDC = g.GetHdc();
//      
//      IntPtr prevObj = GDIApi.SelectObject( hDC, fontDC );
//
//      int size = GDIApi.GetOutlineTextMetricsEx( hDC, 0, IntPtr.Zero );
//
//      if( size != 0 )
//      {
//        IntPtr strPtr = Marshal.AllocHGlobal( size );
//
//        size = GDIApi.GetOutlineTextMetricsEx( hDC, size, strPtr );
//
//        if( size != 0 )
//        {
//          m_metric =
//            ( OUTLINETEXTMETRIC )Marshal.PtrToStructure( strPtr, typeof( OUTLINETEXTMETRIC ) );
//        }
//      }
//
//      //GDIApi.SelectObject( this.Objects.Handle, prevObj );
//      GDIApi.DeleteObject( fontDC );
//      g.ReleaseHdc( hDC );
//    }
    #endregion
  }
}