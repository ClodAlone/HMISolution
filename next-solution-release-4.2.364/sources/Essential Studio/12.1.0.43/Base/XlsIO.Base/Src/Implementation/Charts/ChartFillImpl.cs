#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.IO;
#if ( WINRT )
using Syncfusion.XlsIO.Security.Cryptography;
#else
using System.Security.Cryptography;
#endif

using Syncfusion.XlsIO.Implementation.Shapes;
using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Parser.Biff_Records.Charts;
using Syncfusion.XlsIO.Parser.Biff_Records.MsoDrawing;
using FOPTE = Syncfusion.XlsIO.Parser.Biff_Records.MsoDrawing.MsofbtOPT.FOPTE;
using System.Collections.Generic;

#if ( WINRT )
using Windows.UI;
using Syncfusion.XlsIO;
#endif
#if  (SILVERLIGHT || WP)
using System.Windows.Media;
#elif !(WINRT )
using System.Drawing;
#endif

namespace Syncfusion.XlsIO.Implementation.Charts
{
  /// <summary>
  /// Represents fill format in chart object.
  /// </summary>
  public class ChartFillImpl
    : ShapeFillImpl
  {
    #region Class members
    /// <summary>
    /// Represents gel record.
    /// </summary>
    private ChartGelFrameRecord m_gel;
    /// <summary>
    ///It's define the series color with invertifnegative attribute 
    /// </summary>
    private bool m_invertIfNegative;
    #endregion

    #region Class initalize methods
    /// <summary>
    /// Initialize new instance of fill class.
    /// </summary>
    /// <param name="application">Represents current application.</param>
    /// <param name="parent">Represents parent object.</param>
    public ChartFillImpl( IApplication application, object parent )
      : base( application, parent )
    {
      m_gel = ( ChartGelFrameRecord )BiffRecordFactory.GetRecord( TBIFFRecord.ChartGelFrame );
      m_bIsShapeFill = false;
    }

    /// <summary>
    /// Initialize new instance of fill class.
    /// </summary>
    /// <param name="application">Represents current application.</param>
    /// <param name="parent">Represents parent object.</param>
    /// <param name="gel">Represents gel record.</param>
    [ CLSCompliant( false ) ]
    public ChartFillImpl( IApplication application, object parent, ChartGelFrameRecord gel )
      : base( application, parent )
    {
      if( gel == null )
        throw new ArgumentNullException( "gel" );

      m_gel = gel;
      m_bIsShapeFill = false;

      Parse();
    }
    #endregion

    #region Parse methods
    /// <summary>
    /// Parses current properties.
    /// </summary>
    private void Parse()
    {
      IList list = m_gel.OptionList;

      for( int i = 0, iLen = list.Count; i < iLen; i++ )
      {
        ParseOption( list[ i ] as FOPTE );
      }

      FOPTE picture = ParsePictureData;

      if( picture != null )
      {
        ParsePictureOrUserDefinedTexture( m_fillType == ExcelFillType.Picture );
      }
    }
    #endregion

    #region Serialize methods
    /// <summary>
    /// Serialize fill properties.
    /// </summary>
    /// <param name="records">Represents records to serialize.</param>
    [ CLSCompliant( false ) ]
    public void Serialize( IList<IBiffStorage> records )
    {
      if( /*FillType == ExcelFillType.SolidColor ||*/ ( Parent as IFillColor ).IsAutomaticFormat || !Visible ) 
        return;

      m_gel.UpdateToSerialize();
      FopteOptionWrapper options = new FopteOptionWrapper( m_gel.OptionList );

      Serialize( options );
      //records.Add( m_gel );
      ChartGelFrameRecord result = ( ChartGelFrameRecord )m_gel.Clone();

      List<BiffRecordRaw> toAdd = result.UpdatesToAddInStream();

      for( int i = 0, iLen = toAdd.Count; i < iLen; i++ )
      {
        records.Add( toAdd[ i ] );
      }
    }
    #endregion

    #region Class overrided properties
    /// <summary>
    /// Returns or sets the degree of transparency of the specified fill as
    ///  a value from 0.0 (opaque) through 1.0 (clear).
    /// </summary>
    public override double TransparencyFrom
    {
      get
      {
        throw new NotSupportedException( "This property doesnt support in chart fill format." );
      }
      set
      {
        throw new NotSupportedException( "This property doesnt support in chart fill format." );
      }
    }
    /// <summary>
    /// Returns or sets the degree of transparency of the specified fill as
    ///  a value from 0.0 (opaque) through 1.0 (clear).
    /// </summary>
    public override double TransparencyTo
    {
      get
      {
        throw new NotSupportedException( "This property doesnt support in chart fill format." );
      }
      set
      {
        throw new NotSupportedException( "This property doesnt support in chart fill format." );
      }
    }

    /// <summary>
    /// Represents foreground color.
    /// </summary>
    public override ColorObject ForeColorObject
    {
      get
      {
        return ( Parent as IFillColor ).ForeGroundColorObject;
        // Change event Visible = true;
      }
    }
    /// <summary>
    /// Represents background color.
    /// </summary>
    public override ColorObject BackColorObject
    {
      get
      {
        return ( Parent as IFillColor ).BackGroundColorObject;
        // Change event Visible = true;
      }
    }

    /// <summary>
    /// Represents if fill format is visible.
    /// </summary>
    public override bool Visible
    {
      get
      {
        return ( Parent as IFillColor ).Pattern != ExcelPattern.None;
      }
      set
      {
        IFillColor fill = ( Parent as IFillColor );
        fill.IsAutomaticFormat = false;

        if( value )
        {
          if( fill.Pattern == ExcelPattern.None )
            fill.Pattern = ExcelPattern.Solid;
        }
        else
        {
          fill.Pattern = ExcelPattern.None;
        }
      }
    }
    /// <summary>
    /// Represent the invert option's
    /// </summary>
    public bool InvertIfNegative
    {
        get
        {
            return m_invertIfNegative;
        }
        set
        {
            m_invertIfNegative = value;
        }
    }
    #endregion

    #region Class overrided methods
    /// <summary>
    /// Sets picture to option storage.
    /// </summary>
    /// <param name="opt">Represents option storage.</param>
    /// <returns>Returns updated option storage.</returns>
    [ CLSCompliant( false )]
    protected override IFopteOptionWrapper SetPicture( IFopteOptionWrapper opt )
    {
      if( opt == null )
        throw new ArgumentNullException( "opt" );

      MemoryStream ms = new MemoryStream();
#if !SILVERLIGHT && !WINRT && !WP
      m_picture.Save( ms, m_picture.RawFormat);
#endif
      byte[] image = ms.GetBuffer();
      byte[] result = new byte[ image.Length + 25 ];
      byte[] first = { 0xa0, 0x46, 0x1d, 0xf0 };

      try
      {
#if !SILVERLIGHT && !WINRT && !WP
          (new MD5CryptoServiceProvider().ComputeHash(ms)).CopyTo(result, 8);
#else
          ( new SHA1Managed() ).ComputeHash( ms ).CopyTo( result, 8 );
#endif
      }
      catch (InvalidOperationException)
      {
#if !SILVERLIGHT && !WINRT && !WP
          (new MACTripleDES().ComputeHash(ms)).CopyTo(result, 8);
#endif
      }
      result[ 24 ] = 0xff;
      image.CopyTo( result, 25 );
      BitConverter.GetBytes( image.Length + 17 ).CopyTo( result, 4 );
      first.CopyTo( result, 0 );

      ShapeImpl.SerializeForte( opt, MsoOptions.PatternTexture, 0, result, true );

      return opt;
    }
#if !SILVERLIGHT && !WINRT && !WP
    /// <summary>
    /// Sets picture to bse collection.
    /// </summary>
    /// <param name="im">Image to set.</param>
    /// <param name="strName">Represent name of image.</param>
    /// <returns>Return index of image.</returns>
    protected override int SetPictureToBse( Image im, string strName )
    {
      return 0;
    }
#endif
    /// <summary>
    /// Serialize transparency to option holder.
    /// </summary>
    /// <param name="opt">Represents option holder.</param>
    /// <returns>Returns updated option holder.</returns>
    [ CLSCompliant( false ) ]
    protected override IFopteOptionWrapper SerializeTransparency( IFopteOptionWrapper opt )
    {
      return opt;
    }
    /// <summary>
    /// Changes if need visible.
    /// </summary>
    protected override void ChangeVisible()
    {
      if( ( Parent as IFillColor ).IsAutomaticFormat || Parent is ChartFrameFormatImpl )
        Visible = true;
      if (Parent is ChartWallOrFloorImpl)
          (Parent as ChartWallOrFloorImpl).HasShapeProperties = true;
    }
    #endregion
  }
}
