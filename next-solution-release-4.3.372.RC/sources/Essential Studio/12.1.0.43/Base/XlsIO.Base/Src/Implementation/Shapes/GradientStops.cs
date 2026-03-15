#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.IO;

#if  (SILVERLIGHT)
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.Silverlight.Rectangle;
#elif (WP)
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.WP.Rectangle;
#elif ( WINRT )
using Rectangle = Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
#else
using System.Drawing;
#endif

namespace Syncfusion.XlsIO.Implementation.Shapes
{
  /// <summary>
  /// Represents collection of gradient stops.
  /// </summary>
  public class GradientStops : List<GradientStopImpl>
  {
    #region Constants
    /// <summary>
    /// Maximum possible gradient position.
    /// </summary>
    internal const int MaxPosition = 100000;
    #endregion

    #region Members
    /// <summary>
    /// Gradient angle.
    /// </summary>
    private int m_iAngle;
    /// <summary>
    /// Gradient type.
    /// </summary>
    private GradientType m_gradientType;
    /// <summary>
    /// This element defines the "focus" rectangle for the center shade,
    /// specified relative to the fill tile rectangle.
    /// </summary>
    private Rectangle m_fillToRect;
    /// <summary>
    /// Gradient fill rect element
    /// </summary>
    private Rectangle m_tileRect;
    #endregion

    #region Properties
    /// <summary>
    /// Gets / sets gradient angle.
    /// </summary>
    public int Angle
    {
      get
      {
        return m_iAngle;
      }
      set
      {
        m_iAngle = value;
      }
    }
    /// <summary>
    /// Gets / sets type of the gradient.
    /// </summary>
    public GradientType GradientType
    {
      get
      {
        return m_gradientType;
      }
      set
      {
        m_gradientType = value;
      }
    }
    /// <summary>
    /// This element defines the "focus" rectangle for the center shade,
    /// specified relative to the fill tile rectangle.
    /// </summary>
    public Rectangle FillToRect
    {
      get
      {
        return m_fillToRect;
      }
      set
      {
        m_fillToRect = value;
      }
    }
    /// <summary>
    /// it's define the tilerect property.
    /// </summary>
    public Rectangle TileRect
    {
        get
        {
            return m_tileRect;
        }
        set
        {
            m_tileRect = value;
        }
    }
    /// <summary>
    /// Returns true if gradient stops are symmetric. Read-only.
    /// </summary>
    public bool IsDoubled
    {
      get
      {
        int iCount = Count;
        bool bResult = true;

        if( iCount <= 2 )
        {
          bResult = false;
        }
        else
        {
          for( int i = 0, j = iCount - 1; i <= j; i++, j-- )
          {
            GradientStopImpl stop1 = this[ i ];
            GradientStopImpl stop2 = this[ j ];

            if( stop1.ColorObject != stop2.ColorObject || stop1.Position != MaxPosition - stop2.Position )
            {
              bResult = false;
              break;
            }
          }
        }

        return bResult;
      }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Default constructor.
    /// </summary>
    public GradientStops()
    {
    }
    /// <summary>
    /// Initializes new instance of the gradient stops collection and extracts settings from byte array.
    /// </summary>
    /// <param name="data">Byte array to parse.</param>
    public GradientStops( byte[] data )
    {
      Parse( data );
    }
    /// <summary>
    /// Saves gradient stops into stream in binary format.
    /// </summary>
    /// <param name="stream">Stream to serialize into.</param>
    public void Serialize( Stream stream )
    {
      if( stream == null )
        throw new ArgumentNullException( "stream" );

      byte[] arrBuffer = BitConverter.GetBytes( Count );
      stream.Write( arrBuffer, 0, arrBuffer.Length );

      arrBuffer = BitConverter.GetBytes( m_iAngle );
      stream.Write( arrBuffer, 0, arrBuffer.Length );

      //arrBuffer = BitConverter.GetBytes( ( byte )m_gradientType );
      //stream.Write( arrBuffer, 0, arrBuffer.Length );
      stream.WriteByte( ( byte )m_gradientType );

      arrBuffer = BitConverter.GetBytes( m_fillToRect.Left );
      stream.Write( arrBuffer, 0, arrBuffer.Length );

      arrBuffer = BitConverter.GetBytes( m_fillToRect.Top );
      stream.Write( arrBuffer, 0, arrBuffer.Length );

      arrBuffer = BitConverter.GetBytes( m_fillToRect.Right );
      stream.Write( arrBuffer, 0, arrBuffer.Length );

      arrBuffer = BitConverter.GetBytes( m_fillToRect.Bottom );
      stream.Write( arrBuffer, 0, arrBuffer.Length );

      for( int i = 0, len = Count; i < len; i++ )
      {
        this[ i ].Serialize( stream );
      }
    }
    /// <summary>
    /// Parse byte array.
    /// </summary>
    /// <param name="data">Array to parse.</param>
    private void Parse( byte[] data )
    {
      if( data == null )
        throw new ArgumentNullException( "data" );

      //MemoryStream stream = new MemoryStream( data );
      //StreamReader reader = new StreamReader( Stream );
      int iOffset = 0;
      int iLength = data.Length;
      int iCount = BitConverter.ToInt32( data, iOffset );
      iOffset += ExcelConstants.IntSize;

      m_iAngle = BitConverter.ToInt32( data, iOffset );
      iOffset += ExcelConstants.IntSize;

      m_gradientType = ( GradientType )data[ iOffset ];
      iOffset++;

      int iLeft = BitConverter.ToInt32( data, iOffset );
      iOffset += ExcelConstants.IntSize;

      int iTop = BitConverter.ToInt32( data, iOffset );
      iOffset += ExcelConstants.IntSize;

      int iRight = BitConverter.ToInt32( data, iOffset );
      iOffset += ExcelConstants.IntSize;

      int iBottom = BitConverter.ToInt32( data, iOffset );
      iOffset += ExcelConstants.IntSize;

      m_fillToRect = Rectangle.FromLTRB( iLeft, iTop, iRight, iBottom );

      for( int i = 0; i < iCount; i++ )
      {
        GradientStopImpl gradientStop = new GradientStopImpl( data, iOffset );
        iOffset += GradientStopImpl.Size;
        Add( gradientStop );
      }
    }
    /// <summary>
    /// Doubles gradient stops in the collection and updates their positions.
    /// </summary>
    public void DoubleGradientStops()
    {
      int iCount = Count;

      if( iCount == 0 )
        return;

      GradientStopImpl last = this[ iCount - 1 ];
      int iPos = last.Position;
      int iNewPos = iPos >> 1;
      last.Position = iNewPos;

      if( iPos != MaxPosition )
      {
        last = last.Clone();
        last.Position = MaxPosition - iNewPos;
        Add( last );
      }

      for( int i = iCount - 2; i >= 0; i-- )
      {
        GradientStopImpl stop = this[ i ];
        iPos = stop.Position >> 1;
        stop.Position = iPos;
        stop = stop.Clone();
        stop.Position = MaxPosition - iPos;
        Add( stop );
      }
    }
    /// <summary>
    /// Inverts gradient stops order and updates their positions correctly.
    /// </summary>
    public void InvertGradientStops()
    {
      int iCount = Count;

      if( iCount == 0 )
        return;

      Reverse();

      for( int i = 0; i < iCount; i++ )
      {
        GradientStopImpl gradientStop = this[ i ];
        int iPosition = gradientStop.Position;
        gradientStop.Position = MaxPosition - iPosition;
      }
    }
    /// <summary>
    /// Shrinks gradient stop.
    /// </summary>
    /// <returns>Shrinked Gradient stops.</returns>
    public GradientStops ShrinkGradientStops()
    {
      GradientStops result = new GradientStops();
      result.m_iAngle = m_iAngle;
      result.m_gradientType = m_gradientType;
      result.m_fillToRect = m_fillToRect;

      const int iPositionHalf = MaxPosition / 2;

      for( int i = 0, len = Count; i < len; i++ )
      {
        GradientStopImpl stop = this[ i ];

        if( stop.Position > iPositionHalf )
          break;

        stop = stop.Clone();
        stop.Position <<= 1;
        result.Add( stop );
      }

      return result;
    }
    /// <summary>
    /// Creates copy of the current instance.
    /// </summary>
    /// <returns>Copy of the current instance.</returns>
    public GradientStops Clone()
    {
      GradientStops result = new GradientStops();
      result.m_iAngle = m_iAngle;
      result.m_gradientType = m_gradientType;
      result.m_fillToRect = m_fillToRect;
      result.m_tileRect = m_tileRect;
      for( int i = 0, len = Count; i < len; i++ )
      {
        result.Add( this[ i ].Clone() );
      }

      return result;
    }
    #endregion

   /// <summary>
   /// Checks whether specified Gradient stops has colors equal to this gradient stops.
   /// </summary>
   /// <param name="gradientStops">Represents gradient stop</param>
   /// <returns>Value indicating whether gradient stop has color.</returns>
    internal bool EqualColors( GradientStops gradientStops )
    {
      if( gradientStops == null )
        return false;

      bool bResult = false;
      int iCount = Count;

      if( gradientStops.Count == iCount )
      {
        bResult = true;

        for( int i = 0; i < iCount; i++ )
        {
          GradientStopImpl stop1 = this[ i ];
          GradientStopImpl stop2 = gradientStops[ i ];

          if( !stop1.EqualsWithoutTransparency( stop2 ) )
          {
            bResult = false;
            break;
          }
        }
      }

      return bResult;
    }

    internal void Dispose()
    {
        foreach (GradientStopImpl gradientStopImpl in this)
        {
            gradientStopImpl.Dispose();
        }
    }
  }

  /// <summary>
  /// This enumeration specifies all possible gradient types.
  /// </summary>
  public enum GradientType
  {
    /// <summary>
    /// This element specifies a linear gradient.
    /// </summary>
    Liniar,
    /// <summary>
    /// Gradient follows a circular path.
    /// </summary>
    Circle,
    /// <summary>
    /// Gradient follows a rectangular path.
    /// </summary>
    Rect,
    /// <summary>
    /// Gradient follows the shape.
    /// </summary>
    Shape,
  }
}
