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
using Syncfusion.XlsIO.Implementation.Charts;
using Syncfusion.XlsIO.Interfaces;

namespace Syncfusion.XlsIO.Implementation.XmlSerialization.Charts
{
  /// <summary>
  /// Object that gives access to border, interior and fill of the ChartSerieDataFormatImpl.
  /// </summary>
  internal class ChartFillObjectGetter : IChartFillObjectGetter
  {
    #region Members
    /// <summary>
    /// Parent data format to get fill objects from.
    /// </summary>
    private ChartSerieDataFormatImpl m_parentFormat;
    #endregion

    #region Methods
    /// <summary>
    /// Initializes a new instance of the ChartFillObjectGetter class.
    /// </summary>
    /// <param name="dataFormat">Parent data format.</param>
    public ChartFillObjectGetter( ChartSerieDataFormatImpl dataFormat )
    {
      if( dataFormat == null )
        throw new ArgumentNullException( "dataFormat" );

      m_parentFormat = dataFormat;
    }
    #endregion

    #region Properties
    /// <summary>
    /// Gets chart border object. Read-only.
    /// </summary>
    public ChartBorderImpl Border
    {
      get
      {
        m_parentFormat.HasLineProperties = true;
        return m_parentFormat.LineProperties as ChartBorderImpl;
      }
    }
    /// <summary>
    /// Gets chart interior object. Read-only.
    /// </summary>
    public ChartInteriorImpl Interior
    {
      get
      {
        m_parentFormat.HasInterior = true;
        return m_parentFormat.Interior as ChartInteriorImpl;
      }
    }
    /// <summary>
    /// Gets chart fill object. Read-only.
    /// </summary>
    public IInternalFill Fill
    {
      get
      {
        m_parentFormat.HasInterior = true;
        return m_parentFormat.Fill as IInternalFill;
      }
    }
        /// <summary>
        /// Gets Shadow object.Read-only
        /// </summary>
        /// <value></value>
        public ShadowImpl Shadow
        {
            get
            {
                return m_parentFormat.Shadow as ShadowImpl;
            }
        }
        /// <summary>
        /// Gets the three_ D.Read-only
        /// </summary>
        /// <value></value>
        public ThreeDFormatImpl ThreeD
        {
            get
            {
                return m_parentFormat.ThreeD as ThreeDFormatImpl;
            }
        }
    #endregion
  }

  /// <summary>
  /// Object that gives access to border, interior and fill.
  /// </summary>
  internal class ChartFillObjectGetterAny : IChartFillObjectGetter
  {
    #region Members
    /// <summary>
    /// Border object.
    /// </summary>
    private ChartBorderImpl m_border;
    /// <summary>
    /// Interior object.
    /// </summary>
    private ChartInteriorImpl m_interior;
    /// <summary>
    /// Fill object.
    /// </summary>
    private IInternalFill m_fill;

        /// <summary>
        /// Shadow object
        /// </summary>
        private ShadowImpl m_shadow;
        /// <summary>
        /// Three_D object
        /// </summary>
        private ThreeDFormatImpl m_threeD;
    #endregion

    #region Methods
    /// <summary>
    /// Initializes a new instance of the ChartFillObjectGetterAny class.
    /// </summary>
    /// <param name="border">Border object to use.</param>
    /// <param name="interior">Interior object to use.</param>
    /// <param name="fill">Fill object to use.</param>
    public ChartFillObjectGetterAny( ChartBorderImpl border, ChartInteriorImpl interior, IInternalFill fill, ShadowImpl shadow,ThreeDFormatImpl three_d )
    {
      m_border = border;
      m_interior = interior;
      m_fill = fill;
            m_shadow = shadow;
            m_threeD = three_d;
    }
    #endregion

    #region Properties
    /// <summary>
    /// Gets border object. Read-only.
    /// </summary>
    public ChartBorderImpl Border
    {
      get
      {
        return m_border;
      }
    }
    /// <summary>
    /// Gets chart interior object. Read-only.
    /// </summary>
    public ChartInteriorImpl Interior
    {
      get
      {
        //m_parentFormat.HasInterior = true;
        return m_interior;
      }
    }
    /// <summary>
    /// Gets fill object. Read-only.
    /// </summary>
    public IInternalFill Fill
    {
      get
      {
        //m_parentFormat.HasInterior = true;
        return m_fill;
      }
    }
        /// <summary>
        /// Gets Shadow object.Read-only
        /// </summary>
        /// <value></value>
        public ShadowImpl Shadow
        {
            get
            {
                return m_shadow;
            }
        }
        /// <summary>
        /// Gets the three_ D.Read-only
        /// </summary>
        /// <value></value>
        public ThreeDFormatImpl ThreeD
        {
            get
            {
                return m_threeD;
            }
        }

    #endregion
  }
}
