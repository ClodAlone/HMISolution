#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;

using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Parser.Biff_Records.Charts;
using Syncfusion.XlsIO.Implementation.XmlSerialization.Constants;
using System.Text;


#if ( WINRT )
using Windows.UI;
using Syncfusion.XlsIO.Implementation.Exceptions;
#endif

#if  (SILVERLIGHT || WP)
using System.Windows.Media;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;

#endif

namespace Syncfusion.XlsIO.Implementation
{
  /// <summary>
  /// Represents chart shadow object
  /// </summary>
  public class ShadowImpl : CommonObject
  , IShadow
  , ICloneParent
  {
    #region Class Initialize
    /// <summary>
    /// Class used for Shadowformat Implementation
    /// </summary>
    private ShadowData m_chartShadowFormat = new ShadowData();//( ChartShadowFormatRecord )BiffRecordFactory.GetRecord( TBIFFRecord.ChartShadow );
    /// <summary>
    /// Object of WorkbookImpl
    /// </summary>
    private WorkbookImpl m_parentBook;
    /// <summary>
    /// Indicates whether customshadowstyle is set or not
    /// </summary>
    private bool m_HasCustomShadowStyle = false;
    /// <summary>
    /// Represents the Transparency of the Shadow,the values are between(0-100)
    /// </summary>
    private int m_Transparency;
    /// <summary>
    /// Represents the Size of the Shadow,the values are between(0-200)
    /// </summary>
    private int m_Size;
    /// <summary>
    /// Represents the Blurradius of the Shadow,the values are between(0-100)
    /// </summary>
    private int m_Blur;
    /// <summary>
    /// Object of ChartMarkerFormatRecord to get Shadow color
    /// </summary>
    private ChartMarkerFormatRecord m_Shadow;
    /// <summary>
    /// Represents the Direction of the Shadow,the values are between(0-359)
    /// </summary>
    private int m_Angle;
    /// <summary>
    /// Represents the Distance of the Shadow,the values are between(0-200)
    /// </summary>
    private int m_Distance;
    /// <summary>
    /// Represents the Shadow Color
    /// </summary>
    ColorObject m_shadowColor;
    /// <summary>
    /// Creates the shadow and sets its Application and Parent
    /// properties to specified values.
    /// </summary>
    /// <param name="application">Application object for the shadow.</param>
    /// <param name="parent">Parent object for the shadow.</param>
    public ShadowImpl( IApplication application, object parent )
      : base( application, parent )
    {
      InitializeColors();
      SetParents();
    }

    /// <summary>
    /// Initializes the colors.
    /// </summary>
    private void InitializeColors()
    {
      m_shadowColor = new ColorObject( ColorExtension.Empty );
      m_shadowColor.AfterChange += ShadowColorChanged;
    }
    /// <summary>
    /// Shadows the color changed.
    /// </summary>
    private void ShadowColorChanged()
    {
      ExcelKnownColors value = m_shadowColor.GetIndexed( m_parentBook );
      ShadowFormat.FillColorIndex = ( ushort )value;
      ShadowFormat.IsNotShowInt = ( value == ExcelKnownColors.None ) ? true : false;
    }
    /// <summary>
    /// Gets the shadow format.
    /// </summary>
    /// <value>The shadow format.</value>
    [CLSCompliant( false )]
    public ChartMarkerFormatRecord ShadowFormat
    {
      get
      {
        if( m_Shadow == null )
        {
          m_Shadow = ( ChartMarkerFormatRecord )
            BiffRecordFactory.GetRecord( TBIFFRecord.ChartMarkerFormat );

          // m_bFormatted = true;

          m_Shadow.IsAutoColor = true;
        }

        return m_Shadow;
      }
    }
    /// <summary>
    /// Searches for all necessary parent objects.
    /// </summary>
    private void SetParents()
    {
      m_parentBook = ( WorkbookImpl )FindParent( typeof( WorkbookImpl ) );


      if( m_parentBook == null )
        throw new ApplicationException( "cannot find parent objects." );

    }
    #endregion
    #region IShadow Properties
    /// <summary>
    /// Gets or sets the shadow outer presets.
    /// </summary>
    /// <value>The shadow outer presets.</value>
    public Excel2007ChartPresetsOuter ShadowOuterPresets
    {
      get
      {
        return m_chartShadowFormat.ShadowOuterPresets;
      }
      set
      {
        if( value != ShadowOuterPresets )
          m_chartShadowFormat.ShadowOuterPresets = value;
      }
    }


    /// <summary>
    /// Gets or sets the shadow inner presets.
    /// </summary>
    /// <value>The shadow inner presets.</value>
    public Excel2007ChartPresetsInner ShadowInnerPresets
    {
      get
      {
        return m_chartShadowFormat.ShadowInnerPresets;
      }
      set
      {
        if( value != ShadowInnerPresets )
          m_chartShadowFormat.ShadowInnerPresets = value;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this instance has custom shadow style.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance has custom shadow style; otherwise, <c>false</c>.
    /// </value>
    public bool HasCustomShadowStyle
    {
      get
      {
        return m_HasCustomShadowStyle;
      }
      set
      {
        //if (value == false)
        //    throw new NotSupportedException("It should be set true to implement the custom shadow style");
        m_HasCustomShadowStyle = value;
      }
    }
    /// <summary>
    /// Gets or sets the shadow prespective presets.
    /// </summary>
    /// <value>The shadow prespective presets.</value>
    public Excel2007ChartPresetsPrespective ShadowPrespectivePresets
    {
      get
      {
        return m_chartShadowFormat.ShadowPrespectivePresets;
      }
      set
      {
        if( value != ShadowPrespectivePresets )
          m_chartShadowFormat.ShadowPrespectivePresets = value;

      }
    }
    /// <summary>
    /// Gets or sets the transparency of Shadow.
    /// </summary>
    /// <value>The transparency.</value>
    public int Transparency
    {
      get
      {
        return m_Transparency;
      }
      set
      {
        if( ( value < 0 ) || ( value > 100 ) )
          throw new NotSupportedException( "The Value of the transparency should be between(0-100)" );

        m_Transparency = ( 100 - value ) * ChartConstants.TransparencyValue;
      }
    }
    /// <summary>
    /// Gets or sets the size of Shadow.
    /// </summary>
    /// <value>The size.</value>
    public int Size
    {
      get
      {
        return m_Size;
      }
      set
      {
        if( ( value <= 0 ) || ( value > 200 ) )
          throw new NotSupportedException( "The value of the size should be between(0-200)" );

        m_Size = value * ChartConstants.SizeValue;

      }
    }
    /// <summary>
    /// Gets or sets the blur of Shadow.
    /// </summary>
    /// <value>The blur.</value>
    public int Blur
    {
      get
      {
        return m_Blur;
      }
      set
      {
        if( ( value < 0 ) || ( value > 100 ) )
          throw new NotSupportedException( "The Value of the blur should be between(0-100)" );

        m_Blur = value * ChartConstants.BlurValue;
      }
    }
    /// <summary>
    /// Gets or sets the angle of Shadow.
    /// </summary>
    /// <value>The angle.</value>
    public int Angle
    {
      get
      {
        return m_Angle;
      }
      set
      {
        if( ( value < 0 ) || ( value > 359 ) )
          throw new NotSupportedException( "The Value of the angle should be between(0-359)" );

        m_Angle = value * ChartConstants.Anglevalue;

      }
    }
    public Color ShadowColor
    {
      get
      {
        return m_shadowColor.GetRGB( m_parentBook );
      }
      set
      {
        m_shadowColor.SetRGB( value );
      }
    }
    /// <summary>
    /// Gets or sets the distance of Shadow.
    /// </summary>
    /// <value>The distance.</value>
    public int Distance
    {
      get
      {
        return m_Distance;
      }
      set
      {
        if( ( value < 0 ) || ( value > 200 ) )
          throw new NotSupportedException( "The Value of the distance should be between(0-200)" );

        m_Distance = value * ChartConstants.DistanceValue;

      }
    }

    #endregion

    #region Clone
    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <param name="parent">Parent object for a copy of this instance.</param>
    /// <returns>
    /// A new object that is a copy of this instance.
    /// </returns>
    object ICloneParent.Clone( object parent )
    {
      return Clone( parent );
    }
    /// <summary>
    /// Clone current Record.
    /// </summary>
    /// <param name="parent">Parent object.</param>
    /// <returns>Returns clone of current object.</returns>
    public ShadowImpl Clone( object parent )
    {
      if( parent == null )
        throw new ArgumentNullException( "parent" );

      ShadowImpl result = ( ShadowImpl )MemberwiseClone();

      result.m_chartShadowFormat = ( ShadowData )CloneUtils.CloneCloneable( m_chartShadowFormat );


      result.SetParent( parent );
      result.SetParents();

      return result;
    }
    #endregion

    #region IShadow Interface Methods
    /// <summary>
    /// Customs the outer shadow styles.
    /// </summary>
    /// <param name="iOuter"></param>
    /// <param name="iTransparency"></param>
    /// <param name="iSize"></param>
    /// <param name="iBlur"></param>
    /// <param name="iAngle"></param>
    /// <param name="iDistance"></param>
    public void CustomShadowStyles( Excel2007ChartPresetsOuter iOuter, int iTransparency, int iSize, int iBlur, int iAngle, int iDistance, bool CustomShadowStyle )
    {
      if( CustomShadowStyle == false )
        throw new NotSupportedException( "It should be set true to implement the custom shadow style" );
      if( iOuter == Excel2007ChartPresetsOuter.NoShadow )
        throw new NotSupportedException( "The method does not accept Noshadow" );
      if( ( iSize <= 0 ) || ( iSize > 200 ) )
        throw new NotSupportedException( "The value of the size should be between(0-200)" );
      if( ( iTransparency < 0 ) || ( iTransparency > 100 ) )
        throw new NotSupportedException( "The Value of the transparency should be between(0-100)" );
      if( ( iBlur < 0 ) || ( iBlur > 100 ) )
        throw new NotSupportedException( "The Value of the blur should be between(0-100)" );
      if( ( iAngle < 0 ) || ( iAngle > 359 ) )
        throw new NotSupportedException( "The Value of the angle should be between(0-359)" );
      if( ( iDistance < 0 ) || ( iDistance > 200 ) )
        throw new NotSupportedException( "The Value of the distance should be between(0-200)" );

      m_HasCustomShadowStyle = CustomShadowStyle;
      m_chartShadowFormat.ShadowOuterPresets = iOuter;
      m_Transparency = ( 100 - iTransparency ) * ChartConstants.TransparencyValue;
      m_Size = iSize * ChartConstants.SizeValue;
      m_Blur = iBlur * ChartConstants.BlurValue;
      m_Angle = iAngle * ChartConstants.Anglevalue;
      m_Distance = iDistance * ChartConstants.DistanceValue;

    }
    /// <summary>
    /// Customs the inner shadow styles.
    /// </summary>
    /// <param name="iInner"></param>
    /// <param name="iTransparency"></param>
    /// <param name="iBlur"></param>
    /// <param name="iAngle"></param>
    /// <param name="iDistance"></param>
    public void CustomShadowStyles( Excel2007ChartPresetsInner iInner, int iTransparency, int iBlur, int iAngle, int iDistance, bool CustomShadowStyle )
    {
      if( CustomShadowStyle == false )
        throw new NotSupportedException( "It should be set true to implement the custom shadow style" );
      if( iInner == Excel2007ChartPresetsInner.NoShadow )
        throw new NotSupportedException( "The method does not accept Noshadow" );
      if( ( iTransparency < 0 ) || ( iTransparency > 100 ) )
        throw new NotSupportedException( "The Value of the transparency should be between(0-100)" );
      if( ( iBlur < 0 ) || ( iBlur > 100 ) )
        throw new NotSupportedException( "The Value of the blur should be between(0-100)" );
      if( ( iAngle < 0 ) || ( iAngle > 359 ) )
        throw new NotSupportedException( "The Value of the angle should be between(0-359)" );
      if( ( iDistance < 0 ) || ( iDistance > 200 ) )
        throw new NotSupportedException( "The Value of the distance should be between(0-200)" );
      m_HasCustomShadowStyle = CustomShadowStyle;
      m_chartShadowFormat.ShadowInnerPresets = iInner;
      m_Transparency = ( 100 - iTransparency ) * ChartConstants.TransparencyValue;
      m_Blur = iBlur * ChartConstants.BlurValue;
      m_Angle = iAngle * ChartConstants.Anglevalue;
      m_Distance = iDistance * ChartConstants.DistanceValue;
    }
    /// <summary>
    /// Customs the perspective shadow styles.
    /// </summary>
    /// <param name="iPerspective"></param>
    /// <param name="iTransparency"></param>
    /// <param name="iSize"></param>
    /// <param name="iBlur"></param>
    /// <param name="iAngle"></param>
    /// <param name="iDistance"></param>
    public void CustomShadowStyles( Excel2007ChartPresetsPrespective iPerspective, int iTransparency, int iSize, int iBlur, int iAngle, int iDistance, bool CustomShadowStyle )
    {
      if( CustomShadowStyle == false )
        throw new NotSupportedException( "It should be set true to implement the custom shadow style" );
      if( iPerspective == Excel2007ChartPresetsPrespective.NoShadow )
        throw new NotSupportedException( "The method does not accept Noshadow" );
      if( ( iSize <= 0 ) || ( iSize > 200 ) )
        throw new NotSupportedException( "The value of the size should be between(0-200)" );
      if( ( iTransparency < 0 ) || ( iTransparency > 100 ) )
        throw new NotSupportedException( "The Value of the transparency should be between(0-100)" );
      if( ( iBlur < 0 ) || ( iBlur > 100 ) )
        throw new NotSupportedException( "The Value of the blur should be between(0-100)" );
      if( ( iAngle < 0 ) || ( iAngle > 359 ) )
        throw new NotSupportedException( "The Value of the angle should be between(0-359)" );
      if( ( iDistance < 0 ) || ( iDistance > 200 ) )
        throw new NotSupportedException( "The Value of the distance should be between(0-200)" );

      m_HasCustomShadowStyle = CustomShadowStyle;
      m_chartShadowFormat.ShadowPrespectivePresets = iPerspective;
      m_Transparency = ( 100 - iTransparency ) * ChartConstants.TransparencyValue;
      m_Size = iSize * ChartConstants.SizeValue;
      m_Blur = iBlur * ChartConstants.BlurValue;
      m_Angle = iAngle * ChartConstants.Anglevalue;
      m_Distance = iDistance * ChartConstants.DistanceValue;

    }

    #endregion
  }
}
