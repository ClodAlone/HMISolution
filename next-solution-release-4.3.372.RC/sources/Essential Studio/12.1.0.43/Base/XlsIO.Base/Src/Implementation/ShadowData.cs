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
using System.Collections.Generic;
using System.Text;

#if ( WINRT )
using Windows.UI;
using Syncfusion.XlsIO.Implementation.WINRT;
using Syncfusion.XlsIO.Interfaces;
#endif

#if  (SILVERLIGHT) 
using System.Windows.Media;
using Syncfusion.XlsIO.Implementation.Silverlight;
using Syncfusion.XlsIO.Interfaces;
#elif WP
using System.Windows.Media;
using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO.Implementation.WP;
#endif
#endregion

namespace Syncfusion.XlsIO.Parser.Biff_Records.Charts
{
    /// <summary>
    /// The Shadow record defines the Shadow properties and 
    /// the 3D features(bevel top and bevel bottom),Lignting and
    /// Material properties
    /// </summary>
    //[Biff(TBIFFRecord.ChartShadow)]
    [Syncfusion.Documentation.DocumentationExclude()]
    //[CLSCompliant(false)]
    public class ShadowData : ICloneable// : BiffRecordRaw
    {
        #region Class Members
        /// <summary>
        /// NoShadow=0
        /// OffsetRight=1
        /// OffsetDiagonalBottomRight=2
        /// OffsetBottom=3
        /// OffsetDiagonalTopLeft=4
        /// OffsetCenter=5
        /// OffsetTop=6
        /// OffsetLeft=7
        /// OffsetDiagonalTopRight=8
        /// OffsetDiagonalBottomLeft=9  
        /// </summary>

        private ushort m_ShadowOuterPresets;
        /// <summary>
        /// NoShadow=0
        ///InsideDiagonalBottomLeft=1
        ///InsideTop=2
        ///InsideRight=3
        ///InsideLeft=4
        ///InsideDiagonalTopRight=5
        ///InsideDiagonalBottomRight=6
        ///InsideCenter=7
        ///InsideBottom=8       
        ///InsideDiagonalTopLeft=9
        /// </summary>
        private ushort m_ShadowInnerPresets;
        /// <summary>
        ///NoShadow=0,
        ///PrespectiveDiagonalUpperRight=1,
        ///PrespectiveDiagonalLowerRight=2,
        ///PrespectiveDiagonalUpperLeft=3,
        ///PrespectiveDiagonalLowerLeft=4,
        ///Below=5
        /// </summary>
        private ushort m_ShadowPrespectivePresets;
        /// <summary>
        ///  NoAngle=0,
        ///Angle=1,
        ///ArtDeco=2,
        ///Circle=3,
        ///Convex=4,
        ///CoolSlant=5,
        ///Cross=6,
        ///Divot=7,
        ///HardEdge=8,
        ///RelaxedInset=9,
        ///Riblet=10,
        ///Slope=11,
        ///SoftRound=12
        /// </summary>
        private ushort m_BevelTop;
        private ushort m_BevelBottom;
        /// <summary>
        /// NoEffect=0,
        ///Matte=1,
        ///WarmMatte=2,
        ///Plastic=3,
        ///Metal=4,
        ///DarkEdge=5,
        ///SoftEdge=6,
        ///Flat=7,
        ///WireFrame=8,
        ///Powder=9,
        ///TranslucentPowder=10,
        ///Clear=11
        /// </summary>
        private ushort m_Material;
        /// <summary>
        ///ThreePoint=0,
        ///Balance=1,
        ///BrightRoom=2,
        ///Chilly=3,
        ///Contrasting=4,
        ///Flat=5,
        ///Flood=6,
        ///Freezing=7,
        ///Glow=8,
        ///Harsh=9,
        ///Morning=10,
        ///Soft=11,
        ///Sunrise=12,
        ///SunSet=13,
        ///TwoPoint=14
        /// </summary>
        private ushort m_Lighting;
        #endregion

        #region Class Properties
        /// <summary>
        /// Gets or sets the shadow outer presets.
        /// </summary>
        /// <value>The shadow outer presets.</value>
        public Excel2007ChartPresetsOuter ShadowOuterPresets
        {
            get
            {
                return (Excel2007ChartPresetsOuter)m_ShadowOuterPresets;
            }
            set
            {
                m_ShadowOuterPresets = (ushort)value;
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
                return (Excel2007ChartPresetsInner)m_ShadowInnerPresets;
            }
            set
            {
                m_ShadowInnerPresets = (ushort)value;
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
                return (Excel2007ChartPresetsPrespective)m_ShadowPrespectivePresets;
            }
            set
            {
                m_ShadowPrespectivePresets = (ushort)value;
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
                return (Excel2007ChartMaterialProperties)m_Material;
            }
            set
            {
                m_Material = (ushort)value;
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
                return (Excel2007ChartLightingProperties)m_Lighting;
            }
            set
            {
                m_Lighting = (ushort)value;
            }
        }

        /// <summary>
        /// Gets or sets the bevel top.
        /// </summary>
        /// <value>The bevel top.</value>
        public Excel2007ChartBevelProperties BevelTop
        {
            get
            {
                return (Excel2007ChartBevelProperties)m_BevelTop;
            }
            set
            {
                m_BevelTop = (ushort)value;
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
                return (Excel2007ChartBevelProperties)m_BevelBottom;
            }
            set
            {
                m_BevelBottom = (ushort)value;
            }
        }
        #endregion

        #region ICloneable Members

        public object Clone()
        {
          return MemberwiseClone();
        }

        #endregion
    }
}
