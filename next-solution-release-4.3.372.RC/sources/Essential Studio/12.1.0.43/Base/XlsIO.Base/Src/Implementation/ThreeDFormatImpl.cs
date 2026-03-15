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

using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Parser.Biff_Records.Charts;

#if ( WINRT )
using Syncfusion.XlsIO.Implementation.Exceptions;
#endif

namespace Syncfusion.XlsIO.Implementation
{
  public class ThreeDFormatImpl :
    CommonObject,
    IThreeDFormat,
    ICloneParent
  {
    #region Class Initialize
    /// <summary>
    /// Class used for Shadowformat Implementation
    /// </summary>
    private ShadowData m_chartShadowFormat = new ShadowData();//( ChartShadowFormatRecord )BiffRecordFactory.GetRecord( TBIFFRecord.ChartShadow );
    private WorkbookImpl m_parentBook;

    /// <summary>
    /// Creates the shadow and sets its Application and Parent
    /// properties to specified values.
    /// </summary>
    /// <param name="application">Application object for the shadow.</param>
    /// <param name="parent">Parent object for the shadow.</param>
    public ThreeDFormatImpl( IApplication application, object parent )
      : base( application, parent )
    {
      SetParents();
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

    #region IThreeDFormat properties
    /// <summary>
    /// Gets or sets the bevel top.
    /// </summary>
    /// <value>The bevel top.</value>
    public Excel2007ChartBevelProperties BevelTop
    {
      get
      {
        return m_chartShadowFormat.BevelTop;
      }
      set
      {
        if( value != BevelTop )
          m_chartShadowFormat.BevelTop = value;
      }
    }

    /// <summary>
    /// Gets or sets the bevel bottom.
    /// </summary>
    /// <value>The bevel bottom.</value>
    public Excel2007ChartBevelProperties BevelBottom
    {
      get
      {
        return m_chartShadowFormat.BevelBottom;
      }
      set
      {
        if( value != BevelBottom )
          m_chartShadowFormat.BevelBottom = value;
      }
    }

    /// <summary>
    /// Gets or sets the material.
    /// </summary>
    /// <value>The material.</value>
    public Excel2007ChartMaterialProperties Material
    {
      get
      {
        return m_chartShadowFormat.Material;
      }
      set
      {
        if( value != Material )
          m_chartShadowFormat.Material = value;
      }
    }

    /// <summary>
    /// Gets or sets the lighting.
    /// </summary>
    /// <value>The lighting.</value>
    public Excel2007ChartLightingProperties Lighting
    {
      get
      {
        return m_chartShadowFormat.Lighting;
      }
      set
      {
        if( value != Lighting )
          m_chartShadowFormat.Lighting = value;
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
    public ThreeDFormatImpl Clone( object parent )
    {
      if( parent == null )
        throw new ArgumentNullException( "parent" );

      ThreeDFormatImpl result = ( ThreeDFormatImpl )MemberwiseClone();

      result.m_chartShadowFormat = ( ShadowData )CloneUtils.CloneCloneable( m_chartShadowFormat );


      result.SetParent( parent );
      result.SetParents();

      return result;
    }
    #endregion
  }
}
