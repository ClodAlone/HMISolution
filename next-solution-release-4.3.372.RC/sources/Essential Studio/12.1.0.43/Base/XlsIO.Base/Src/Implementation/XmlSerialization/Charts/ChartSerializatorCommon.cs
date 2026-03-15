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
using System.Xml;
using Syncfusion.XlsIO.Implementation.XmlSerialization.Constants;
using Syncfusion.XlsIO.Implementation.XmlSerialization.Shapes;
using Syncfusion.XlsIO.Implementation.Charts;
using System.IO;
using Syncfusion.XlsIO.Implementation.Shapes;
using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO.Interfaces.Charts;
using Syncfusion.XlsIO.Implementation.XmlReaders.Shapes;


#if ( WINRT )
using Windows.UI;
using Rectangle = Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
#endif

#if  (SILVERLIGHT)
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.Silverlight.Rectangle;
using System.Drawing;
#elif WP
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.WP.Rectangle;
using System.Drawing;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;


#endif

namespace Syncfusion.XlsIO.Implementation.XmlSerialization.Charts
{
  /// <summary>
  /// This class contains common code used for charts serialization.
  /// </summary>
  public class ChartSerializatorCommon
  {
    #region Members
    /// <summary>
    /// Key - ExcelChartLinePattern,
    /// Value - pair, where key is Excel 2007 pattern value, value - preset pattern of the pattern fill.
    /// </summary>
    private static Dictionary<ExcelChartLinePattern, KeyValuePair<string, string>> s_dicLinePatterns =
      new Dictionary<ExcelChartLinePattern, KeyValuePair<string, string>>();

        /// <summary>
        /// Jagged Array represents the Outer Shadow Attributes
        /// </summary>
        public static string[][] OuterAttributeArray = new string[9][];
        /// <summary>
        /// Jagged Array represents the Inner Shadow Attributes
        /// </summary>
        public static string[][] InnerAttributeArray = new string[9][];
        /// <summary>
        /// Jagged Array represents the Perspective Shadow Attributes
        /// </summary>
        public static string[][] PerspectiveAttributeArray = new string[6][];
        /// <summary>
        /// Jagged Array represents the 3D Bevel Attributes
        /// </summary>
        public static string[][] BevelProperties = new string[13][];
        /// <summary>
        /// Jagged Array represents the 3D Material Attributes
        /// </summary>
        public static string[][] MaterialProperties = new string[11][];
        /// <summary>
        /// Jagged Array represents the 3D Lighting Attributes
        /// </summary>
        public static string[][] LightingProperties = new string[15][];

    #endregion

    #region Methods
    /// <summary>
    /// Initializes static members of ChartSerializatorCommon class.
    /// </summary>
    static ChartSerializatorCommon()
    {
      s_dicLinePatterns.Add( ExcelChartLinePattern.Solid, new KeyValuePair<string, string>( "solid", string.Empty ) );
      s_dicLinePatterns.Add( ExcelChartLinePattern.Dash, new KeyValuePair<string, string>( "lgDash", string.Empty ) );
      s_dicLinePatterns.Add( ExcelChartLinePattern.Dot, new KeyValuePair<string, string>( "sysDash", string.Empty ) );
      s_dicLinePatterns.Add( ExcelChartLinePattern.CircleDot, new KeyValuePair<string, string>( "sysDot", string.Empty ) );
      s_dicLinePatterns.Add( ExcelChartLinePattern.DashDot, new KeyValuePair<string, string>( "lgDashDot", string.Empty ) );
      s_dicLinePatterns.Add( ExcelChartLinePattern.DashDotDot, new KeyValuePair<string, string>( "lgDashDotDot", string.Empty ) );
      s_dicLinePatterns.Add( ExcelChartLinePattern.DarkGray, new KeyValuePair<string, string>( "solid", "pct75" ) );
      s_dicLinePatterns.Add( ExcelChartLinePattern.MediumGray, new KeyValuePair<string, string>( "solid", "pct50" ) );
      s_dicLinePatterns.Add( ExcelChartLinePattern.LightGray, new KeyValuePair<string, string>( "solid", "pct25" ) );
            //blurRad,sx,sy,dist,dir,algn,rotwithshape

            OuterAttributeArray[0] = new string[] { "50800", "null", "null", "38100", "null", "l", "0" };
            OuterAttributeArray[1] = new string[] { "50800", "null", "null", "38100", "2700000", "tl", "0" };
            OuterAttributeArray[2] = new string[] { "50800", "null", "null", "38100", "5400000", "t", "0" };
            OuterAttributeArray[3] = new string[] { "50800", "null", "null", "38100", "13500000", "br", "0" };
            OuterAttributeArray[4] = new string[] { "63500", "102000", "102000", "null", "null", "ctr", "0" };
            OuterAttributeArray[5] = new string[] { "50800", "null", "null", "38100", "16200000", "null", "0" };
            OuterAttributeArray[6] = new string[] { "50800", "null", "null", "38100", "10800000", "r", "0" };
            OuterAttributeArray[7] = new string[] { "50800", "null", "null", "38100", "18900000", "bl", "0" };
            OuterAttributeArray[8] = new string[] { "50800", "null", "null", "38100", "8100000", "tr", "0" };

            //blurRad,dist,dir
            InnerAttributeArray[0] = new string[] { "63500", "50800", "8100000" };
            InnerAttributeArray[1] = new string[] { "63500", "50800", "16200000" };
            InnerAttributeArray[2] = new string[] { "63500", "50800", "null" };
            InnerAttributeArray[3] = new string[] { "63500", "50800", "10800000" };
            InnerAttributeArray[4] = new string[] { "63500", "50800", "18900000" };
            InnerAttributeArray[5] = new string[] { "63500", "50800", "2700000" };
            InnerAttributeArray[6] = new string[] { "114300", "null", "null" };
            InnerAttributeArray[7] = new string[] { "63500", "50800", "5400000" };
            InnerAttributeArray[8] = new string[] { "63500", "50800", "13500000" };

            //blurrad,dir,dist,sy,sx,kx,algn,rotwithshape
            PerspectiveAttributeArray[0] = new string[] { "null", "null", "null", "null", "null", "null", "null", "null" };
            PerspectiveAttributeArray[1] = new string[] { "76200", "18900000", "null", "23000", "null", "-1200000", "bl", "0" };
            PerspectiveAttributeArray[2] = new string[] { "76200", "2700000", "12700", "-23000", "null", "-800400", "bl", "0" };
            PerspectiveAttributeArray[3] = new string[] { "76200", "13500000", "null", "23000", "null", "1200000", "br", "0" };
            PerspectiveAttributeArray[4] = new string[] { "76200", "8100000", "12700", "-23000", "null", "800400", "br", "0" };
            PerspectiveAttributeArray[5] = new string[] { "152400", "5400000", "317500", "-19000", "90000", "null", "null", "0" };

            //Width,height,Presetshape
            BevelProperties[0] = new string[] { "null", "null", "null" };
            BevelProperties[1] = new string[] { "null", "null", "angle" };
            BevelProperties[2] = new string[] { "114300", "null", "artDeco" };
            BevelProperties[3] = new string[] { "null", "null", "null" };
            BevelProperties[4] = new string[] { "null", "null", "convex" };
            BevelProperties[5] = new string[] { "165100", "null", "coolSlant" };
            BevelProperties[6] = new string[] { "139700", "null", "cross" };
            BevelProperties[7] = new string[] { "139700", "139700", "divot" };
            BevelProperties[8] = new string[] { "114300", "null", "hardEdge" };
            BevelProperties[9] = new string[] { "null", "null", "relaxedInset" };
            BevelProperties[10] = new string[] { "101600", "null", "riblet" };
            BevelProperties[11] = new string[] { "null", "null", "slope" };
            BevelProperties[12] = new string[] { "152400", "50800", "softRound" };

            //preset shape
            MaterialProperties[0] = new string[] { "matte" };
            MaterialProperties[1] = new string[] { "null" };
            MaterialProperties[2] = new string[] { "plastic" };
            MaterialProperties[3] = new string[] { "metal" };
            MaterialProperties[4] = new string[] { "dkEdge" };
            MaterialProperties[5] = new string[] { "softEdge" };
            MaterialProperties[6] = new string[] { "flat" };
            MaterialProperties[7] = new string[] { "legacyWireframe" };
            MaterialProperties[8] = new string[] { "powder" };
            MaterialProperties[9] = new string[] { "translucentPowder" };
            MaterialProperties[10] = new string[] { "matte" };

            //preset shape
            LightingProperties[0] = new string[] { "threePt" };
            LightingProperties[1] = new string[] { "balanced" };
            LightingProperties[2] = new string[] { "brightRoom" };
            LightingProperties[3] = new string[] { "chilly" };
            LightingProperties[4] = new string[] { "contrasting" };
            LightingProperties[5] = new string[] { "flat" };
            LightingProperties[6] = new string[] { "flood" };
            LightingProperties[7] = new string[] { "freezing" };
            LightingProperties[8] = new string[] { "glow" };
            LightingProperties[9] = new string[] { "harsh" };
            LightingProperties[10] = new string[] { "morning" };
            LightingProperties[11] = new string[] { "soft" };
            LightingProperties[12] = new string[] { "sunrise" };
            LightingProperties[13] = new string[] { "sunset" };
            LightingProperties[14] = new string[] { "twoPt" };
    }
    /// <summary>
    /// Serializes frame format.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="format">Fill format to serialize.</param>
    /// <param name="chart">Parent chart object.</param>
    /// <param name="isRoundCorners">Indicates whether area corners should be rounded.</param>
    public static void SerializeFrameFormat( XmlWriter writer, IChartFillBorder format,
      ChartImpl chart, bool isRoundCorners )
    {
      SerializeFrameFormat( writer, format, chart, isRoundCorners, false );
    }
    /// <summary>
    /// Serializes frame format.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="format">Fill format to serialize.</param>
    /// <param name="chart">Parent chart object.</param>
    /// <param name="isRoundCorners">Indicates whether area corners should be rounded.</param>
    public static void SerializeFrameFormat( XmlWriter writer, IChartFillBorder format,
      ChartImpl chart, bool isRoundCorners, bool serializeLineAutoValues )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );

      if( format == null )
        return;

      WorksheetDataHolder sheetHolder = chart.DataHolder;
      FileDataHolder holder = sheetHolder.ParentHolder;
      RelationCollection relations = chart.Relations;
      SerializeFrameFormat( writer, format, holder, relations, isRoundCorners, serializeLineAutoValues );
    }
    /// <summary>
    /// Serializes frame format.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="format">Fill format to serialize.</param>
    /// <param name="holder">Parent file data holder.</param>
    /// <param name="relations">Chart's relations collection</param>
    /// <param name="isRoundCorners">Indicates whether area corners should be rounded.</param>
    public static void SerializeFrameFormat( XmlWriter writer, IChartFillBorder format,
      FileDataHolder holder, RelationCollection relations, bool isRoundCorners, bool serilaizeLineAutoValues )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( format == null )
        return;

      writer.WriteStartElement( Drawings.ShapePropertiesTag, ChartConstants.CNamespace );

      if( format.HasInterior )
      {
        if( !format.Interior.UseAutomaticFormat &&
          ( format.Interior.Pattern != ExcelPattern.None ||
          ( format.Fill.FillType != ExcelFillType.Pattern && format.Fill.FillType != ExcelFillType.SolidColor ) ) )
        {
          IInternalFill fill = format.Fill as IInternalFill;

          if( fill != null )
            SerializeFill( writer, fill, holder, relations );
        }
        else if( format.Interior.Pattern == ExcelPattern.None )
        {
          writer.WriteElementString( Drawings.NoFillTag, Drawings.ANamespace, string.Empty );
        }
      }

      if( format.HasLineProperties )
      {
        IChartBorder border = format.LineProperties;

        if( !border.AutoFormat )
          SerializeLineProperties( writer, border, isRoundCorners, holder.Workbook, serilaizeLineAutoValues );
      }
            if ((format.HasShadowProperties) && (format.Shadow.HasCustomShadowStyle == false))
            {
                IShadow shadow = format.Shadow;

                SerializeShadow(writer, shadow, format.Shadow.HasCustomShadowStyle);

            }
            else
            {
                IShadow shadow = format.Shadow;
                SerializeShadow(writer, shadow, format.Shadow.HasCustomShadowStyle);

            }
            if (format.Has3dProperties)
            {
                IThreeDFormat Three_D = format.ThreeD;
                Serialize3D(writer, Three_D);

            }
            writer.WriteEndElement();
        }
        /// <summary>
        /// This method serailizes the Shadow propeties
        /// </summary>
        /// <param name="writer">XmlWriter to serialize into.</param>
        /// <param name="shadow">Shadow format to serialize into</param>
        /// <param name="CustomShadow">Whether the current format contains the Custom Shadow Style</param>
        public static void SerializeShadow(XmlWriter writer, IShadow shadow, bool CustomShadow)
        {
            if (shadow.ShadowInnerPresets != 0)
            {
                int inner = (int)shadow.ShadowInnerPresets - 1;
                SerializeInner(writer, inner, CustomShadow, shadow);
            }
            else if (shadow.ShadowOuterPresets != 0)
            {
                int outer = (int)shadow.ShadowOuterPresets - 1;
                SerailizeOuter(writer, outer, CustomShadow, shadow);
            }
            else if (shadow.ShadowPrespectivePresets != 0)
            {
                int perspective = (int)shadow.ShadowPrespectivePresets;
                SerializePerspective(writer, perspective, CustomShadow, shadow);
            }
            else if (shadow.HasCustomShadowStyle)
            {
                writer.WriteElementString(Drawings.EffectListTag, Drawings.ANamespace, string.Empty);
            }
        }

        /// <summary>
        /// Serializes the inner shadow.
        /// </summary>
        /// <param name="writer">XmlWriter to serialize into.</param>
        /// <param name="inner">The jagged array index of the InnerAttributeArry.</param>
        /// <param name="CustomShadow">if set to <c>true</c> [custom shadow style].</param>
        /// <param name="Shadow">The ShadowImpl object.</param>
        public static void SerializeInner( XmlWriter writer, int inner, bool CustomShadow, IShadow Shadow )
        {
            writer.WriteStartElement(Drawings.EffectListTag, Drawings.ANamespace);
            writer.WriteStartElement(Drawings.InnerShadowTag, Drawings.ANamespace);
            if (CustomShadow == true)
                writer.WriteAttributeString(Drawings.BlurRadiusTag, Shadow.Blur.ToString());
            else
                writer.WriteAttributeString(Drawings.BlurRadiusTag, InnerAttributeArray[inner][0].ToString());
            if (CustomShadow == true)
                writer.WriteAttributeString(Drawings.DistanceTag, Shadow.Distance.ToString());
            else
            {
                if (!InnerAttributeArray[inner][1].Equals("null"))
                {
                    writer.WriteAttributeString(Drawings.DistanceTag, InnerAttributeArray[inner][1].ToString());
                }
            }
            if (CustomShadow == true)
                writer.WriteAttributeString(Drawings.DirectionTag, Shadow.Angle.ToString());
            else
            {
                if (!InnerAttributeArray[inner][2].Equals("null"))
                {
                    writer.WriteAttributeString(Drawings.DirectionTag, InnerAttributeArray[inner][2].ToString());
                }
            }

            writer.WriteStartElement(Drawings.SRGBColorTag, Drawings.ANamespace);
            writer.WriteAttributeString("val", (Shadow.ShadowColor.ToArgb() & 0xFFFFFF).ToString("X6"));

            if (CustomShadow == true)
            {
                if (Shadow.Transparency != 0)
                {
                    writer.WriteStartElement(Drawings.AlphaTag, Drawings.ANamespace);
                    writer.WriteAttributeString("val", Shadow.Transparency.ToString());
                    writer.WriteEndElement();
                }
            }
            else
            {
                if (inner != 6)
                {
                    writer.WriteStartElement(Drawings.AlphaTag, Drawings.ANamespace);
                    writer.WriteAttributeString("val", Drawings.InnerAlphaTag);
                    writer.WriteEndElement();
                }
            }
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();
        }

        /// <summary>
        /// This method Serailizes the Outer Shadow
        /// </summary>
        /// <param name="writer">XmlWriter to serialize into.</param>
        /// <param name="outer">the jagged array index of OuterAttributeArry</param>
        /// <param name="CustomShadow">if set to <c>true</c> [custom shadow style].</param>
        /// <param name="Shadow">The ShadowImpl object</param>
        public static void SerailizeOuter(XmlWriter writer, int outer, bool CustomShadow, IShadow Shadow)
        {
            writer.WriteStartElement(Drawings.EffectListTag, Drawings.ANamespace);
            writer.WriteStartElement(Drawings.OuterShadowTag, Drawings.ANamespace);
            if (CustomShadow == true)
                writer.WriteAttributeString(Drawings.BlurRadiusTag, Shadow.Blur.ToString());
            else
                writer.WriteAttributeString(Drawings.BlurRadiusTag, OuterAttributeArray[outer][0].ToString());
            if (CustomShadow == true)
            {
                writer.WriteAttributeString(Drawings.SizeX, Shadow.Size.ToString());
                writer.WriteAttributeString(Drawings.SizeY, Shadow.Size.ToString());
            }
            else
            {
                if (!OuterAttributeArray[outer][1].Equals("null"))
                {
                    writer.WriteAttributeString(Drawings.SizeX, OuterAttributeArray[outer][1].ToString());
                }
                if (!OuterAttributeArray[outer][2].Equals("null"))
                {
                    writer.WriteAttributeString(Drawings.SizeY, OuterAttributeArray[outer][2].ToString());
                }
            }
            if (CustomShadow == true)
                writer.WriteAttributeString(Drawings.DistanceTag, Shadow.Distance.ToString());
            else
            {
                if (!OuterAttributeArray[outer][3].Equals("null"))
                {
                    writer.WriteAttributeString(Drawings.DistanceTag, OuterAttributeArray[outer][3].ToString());
                }
            }
            if (CustomShadow == true)
                writer.WriteAttributeString(Drawings.DirectionTag, Shadow.Angle.ToString());
            else
            {
                if (!OuterAttributeArray[outer][4].Equals("null"))
                {
                    writer.WriteAttributeString(Drawings.DirectionTag, OuterAttributeArray[outer][4].ToString());
                }
            }

            if (!OuterAttributeArray[outer][5].Equals("null"))
            {
                writer.WriteAttributeString(Drawings.AlignmentTag, OuterAttributeArray[outer][5].ToString());
            }
            writer.WriteAttributeString(Drawings.RotationwithShapeTag, OuterAttributeArray[outer][6].ToString());
            writer.WriteStartElement(Drawings.SRGBColorTag, Drawings.ANamespace);

            writer.WriteAttributeString("val", (Shadow.ShadowColor.ToArgb() & 0xFFFFFF).ToString("X6"));
            if (CustomShadow == true)
            {
                if (Shadow.Transparency != 0)
                {
                    writer.WriteStartElement(Drawings.AlphaTag, Drawings.ANamespace);
                    writer.WriteAttributeString("val", Shadow.Transparency.ToString());
                    writer.WriteEndElement();
                }
            }
            else
            {
                writer.WriteStartElement(Drawings.AlphaTag, Drawings.ANamespace);
                writer.WriteAttributeString("val", "40000");
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();
        }
        /// <summary>
        /// This method serializes the Perspective shadow properties
        /// </summary>
        /// <param name="writer">XmlWriter to serialize into.</param>
        /// <param name="perspective">the jagged array index of PerspectiveAttributeArry </param>
        /// <param name="CustomShadow">if set to <c>true</c> [custom shadow style].</param>
        /// <param name="Shadow">The ShadowImpl object</param>
        public static void SerializePerspective(XmlWriter writer, int perspective, bool CustomShadow, IShadow Shadow)
        {
            writer.WriteStartElement(Drawings.EffectListTag, Drawings.ANamespace);
            writer.WriteStartElement(Drawings.OuterShadowTag, Drawings.ANamespace);
            if (CustomShadow == true)
            {
                writer.WriteAttributeString(Drawings.BlurRadiusTag, Shadow.Blur.ToString());
                writer.WriteAttributeString(Drawings.DirectionTag, Shadow.Angle.ToString());
            }
            else
            {
                writer.WriteAttributeString(Drawings.BlurRadiusTag, PerspectiveAttributeArray[perspective][0].ToString());
                writer.WriteAttributeString(Drawings.DirectionTag, PerspectiveAttributeArray[perspective][1].ToString());
            }
            if (CustomShadow == true)
                writer.WriteAttributeString(Drawings.DistanceTag, Shadow.Distance.ToString());
            else
            {
                if (!PerspectiveAttributeArray[perspective][2].Equals("null"))
                {
                    writer.WriteAttributeString(Drawings.DistanceTag, PerspectiveAttributeArray[perspective][2].ToString());
                }
            }
            if (CustomShadow == true)
            {
                writer.WriteAttributeString(Drawings.SizeY, Shadow.Size.ToString());
                writer.WriteAttributeString(Drawings.SizeX, Shadow.Size.ToString());
            }
            else
            {
                if (!PerspectiveAttributeArray[perspective][3].Equals("null"))
                {
                    writer.WriteAttributeString(Drawings.SizeY, PerspectiveAttributeArray[perspective][3].ToString());
                }
                if (!PerspectiveAttributeArray[perspective][4].Equals("null"))
                {
                    writer.WriteAttributeString(Drawings.SizeX, PerspectiveAttributeArray[perspective][4].ToString());
                }
            }
            if (!PerspectiveAttributeArray[perspective][5].Equals("null"))
            {
                writer.WriteAttributeString(Drawings.KXTag, PerspectiveAttributeArray[perspective][5].ToString());
            }
            if (!PerspectiveAttributeArray[perspective][6].Equals("null"))
            {
                writer.WriteAttributeString(Drawings.AlignmentTag, PerspectiveAttributeArray[perspective][6].ToString());
            }
            writer.WriteAttributeString(Drawings.RotationwithShapeTag, PerspectiveAttributeArray[perspective][7].ToString());
            writer.WriteStartElement(Drawings.SRGBColorTag, Drawings.ANamespace);
            writer.WriteAttributeString("val", (Shadow.ShadowColor.ToArgb() & 0xFFFFFF).ToString("X6"));
            if (CustomShadow == true)
            {
                if (Shadow.Transparency != 0)
                {
                    writer.WriteStartElement(Drawings.AlphaTag, Drawings.ANamespace);
                    writer.WriteAttributeString("val", Shadow.Transparency.ToString());
                    writer.WriteEndElement();
                }
            }
            else
            {
                writer.WriteStartElement(Drawings.AlphaTag, Drawings.ANamespace);
                if (perspective != 4)
                {
                    writer.WriteAttributeString("val", Drawings.PerspectiveAlphaTag);
                }
                else
                {
                    writer.WriteAttributeString("val", Drawings.BelowAlphaTag);
                }
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();
        }
        /// <summary>
        /// This method Serailizes the 3DProperties
        /// </summary>
        /// <param name="writer">XmlWriter to serialize into.</param>
        /// <param name="shadow">Sahdow Format to serailize into</param>
        /// <param name="CustomShadow">if set to <c>true</c> [custom shadow style].</param>
        /// <param name="Shadow">The ShadowImpl object</param>
        public static void Serialize3D(XmlWriter writer, IThreeDFormat Three_D)
        {
            bool hasNoBottom = Three_D.BevelBottom == Excel2007ChartBevelProperties.NoAngle;
            bool hasNoTop = Three_D.BevelTop == Excel2007ChartBevelProperties.NoAngle;
            bool hasNoEffect = Three_D.Material == Excel2007ChartMaterialProperties.NoEffect;
            if (hasNoBottom && hasNoTop && hasNoEffect)
                return;

            writer.WriteStartElement(Drawings.Scene3DTag, Drawings.ANamespace);
            writer.WriteStartElement(Drawings.CameraTag, Drawings.ANamespace);
            writer.WriteAttributeString(Drawings.PresetShapeAttribute, Drawings.ViewTag);
            writer.WriteEndElement();
            writer.WriteStartElement(Drawings.LightingTag, Drawings.ANamespace);
            if (Three_D.Lighting != 0)
            {
                int light = (int)Three_D.Lighting;
                SerializeLight(writer, light);
            }
            else
            {
                writer.WriteAttributeString(Drawings.LightingRightTag, LightingProperties[0][0].ToString());
            }
            writer.WriteAttributeString(Drawings.DirectionTag, "t");
            writer.WriteEndElement();
            writer.WriteEndElement();
            if (Three_D.Material != 0)
            {
                writer.WriteStartElement(Drawings.Special3DTag, Drawings.ANamespace);
                int material = (int)Three_D.Material - 1;
                SerializeMaterial(writer, material);
            }
            else
            {
                writer.WriteStartElement(Drawings.Special3DTag, Drawings.ANamespace);
            }
            if (Three_D.BevelTop != 0)
            {
                int bevel = (int)Three_D.BevelTop;
                SerializeTopBevel(writer, bevel);
            }
            if (Three_D.BevelBottom != 0)
            {
                int bevel = (int)Three_D.BevelBottom;
                SerializeBottomBevel(writer, bevel);
            }
            writer.WriteEndElement();
        }
        /// <summary>
        /// This method serailize the Lighting properties
        /// </summary>
        /// <param name="writer">XmlWriter to serialize into.</param>
        /// <param name="light">the jagged array index of LightingPropertiesArray</param>
        public static void SerializeLight(XmlWriter writer, int light)
        {
            writer.WriteAttributeString(Drawings.LightingRightTag, LightingProperties[light][0].ToString());
        }
        /// <summary>
        /// This method serialize the Material properties
        /// </summary>
        /// <param name="writer">XmlWriter to serialize into.</param>
        /// <param name="material">the jagged array index of MaterialPropertiesArray</param>
        public static void SerializeMaterial(XmlWriter writer, int material)
        {
            writer.WriteAttributeString("prstMaterial", MaterialProperties[material][0].ToString());
        }
        /// <summary>
        /// This method serialize the Bevel top
        /// </summary>
        /// <param name="writer">XmlWriter to serialize into.</param>
        /// <param name="bevel">the jagged array index of bevelpropertiesArray</param>
        public static void SerializeTopBevel(XmlWriter writer, int bevel)
        {
            writer.WriteStartElement(Drawings.BevelTopTag, Drawings.ANamespace);
            if (!BevelProperties[bevel][0].Equals("null"))
            {
                writer.WriteAttributeString(Drawings.LineWidthAttribute, BevelProperties[bevel][0].ToString());
            }
            if (!BevelProperties[bevel][1].Equals("null"))
            {
                writer.WriteAttributeString(Drawings.LineHeightAttribute, BevelProperties[bevel][1].ToString());
            }
            if (!BevelProperties[bevel][2].Equals("null"))
            {
                writer.WriteAttributeString(Drawings.PresetShapeAttribute, BevelProperties[bevel][2].ToString());
            }
            writer.WriteEndElement();
        }

        /// <summary>
        /// This method serializes the Bevel Bottom
        /// </summary>
        /// <param name="writer">XmlWriter to serialize into.</param>
        /// <param name="bevel">the jagged array index of Bevelproperties Array</param>
        public static void SerializeBottomBevel(XmlWriter writer, int bevel)
        {
            writer.WriteStartElement(Drawings.BevelBottomTag, Drawings.ANamespace);
            if (!BevelProperties[bevel][0].Equals("null"))
            {
                writer.WriteAttributeString(Drawings.LineWidthAttribute, BevelProperties[bevel][0].ToString());
            }
            if (!BevelProperties[bevel][1].Equals("null"))
            {
                writer.WriteAttributeString(Drawings.LineHeightAttribute, BevelProperties[bevel][1].ToString());
            }
            if (!BevelProperties[bevel][2].Equals("null"))
            {
                writer.WriteAttributeString(Drawings.PresetShapeAttribute, BevelProperties[bevel][2].ToString());
            }
      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes fill object.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="fill">Fill to serialize.</param>
    /// <param name="holder">Parent file data holder.</param>
    /// <param name="relations">Chart's relations collection</param>
    internal static void SerializeFill( XmlWriter writer, IInternalFill fill, FileDataHolder holder,
      RelationCollection relations )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( fill == null )
        return;

      switch( fill.FillType )
      {
        case ExcelFillType.SolidColor:
          SerializeSolidFill( writer, fill.ForeColorObject, false, holder.Workbook, 1 - fill.Transparency );
          break;

        case ExcelFillType.Pattern:
          SerializePatternFill( writer, fill.ForeColorObject, false,
            fill.BackColorObject, false, fill.Pattern, holder.Workbook );
          break;

        case ExcelFillType.Picture:          
          SerializePictureFill(writer, fill.Picture, holder, relations, (fill as ShapeFillImpl));
          break;

        case ExcelFillType.Texture:
          SerializeTextureFill( writer, fill, holder, relations );
          break;

        case ExcelFillType.Gradient:
          SerializeGradientFill( writer, fill, holder.Workbook );
          break;

        default:
          throw new NotImplementedException();
      }
    }
    /// <summary>
    /// Serializes text area.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="textArea">Text area to serialize.</param>
    /// <param name="book">Parent workbook.</param>
    /// <param name="relations">Chart's relations.</param>
    public static void SerializeTextArea( XmlWriter writer, IChartTextArea textArea,
      WorkbookImpl book, RelationCollection relations, double defaultFontSize )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( textArea == null )
        throw new ArgumentNullException( "textArea" );

      ChartTextAreaImpl text = textArea as ChartTextAreaImpl;
      FileDataHolder holder = book.DataHolder;
      //throw new NotImplementedException();
        
      writer.WriteStartElement( ChartConstants.TitleTag, ChartConstants.CNamespace );
      (textArea as ChartTextAreaImpl).ShowBoldProperties = true;
        if(text.HasText)
            SerializeTextAreaText( writer, textArea, book, defaultFontSize );
      SerializeLayout( writer, textArea );
      SerializeOverlay( writer, textArea );
      SerializeFrameFormat( writer, textArea.FrameFormat, holder, relations, false, false );
      //SerializeShapeProperites( writer, textArea );
      if(text.ParagraphType == ChartParagraphType.CustomDefault)
          SerializeDefaultTextFormatting(writer, textArea, book, defaultFontSize);
      writer.WriteEndElement();
    }
    /// <summary>
    /// Serialize xml tag that contains value attribute with tag value.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="tagName">Tag name to serialize.</param>
    /// <param name="value">Value to serialize.</param>
    public static void SerializeValueTag( XmlWriter writer, string tagName, string value )
    {
      SerializeValueTag( writer, tagName, ChartConstants.CNamespace, value );
    }
      /// <summary>
      /// Serialize the double value attributes.
      /// </summary>
      /// <param name="writer"></param>
      /// <param name="tagName"></param>
      /// <param name="value"></param>
    public static void SerializeDoubleValueTag(XmlWriter writer, string tagName, double value)
    {
        SerializeDoubleValueTag(writer, tagName, ChartConstants.CNamespace, value);
    }
    /// <summary>
    /// Serialize xml tag that contains value attribute with tag value.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="tagName">Tag name to serialize.</param>
    /// <param name="tagNamespace">Namespace of the xml tag.</param>
    /// <param name="value">Value to serialize.</param>
    public static void SerializeValueTag( XmlWriter writer, string tagName, string tagNamespace, string value )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( tagName == null )
        throw new ArgumentNullException( "tagName" );

      if( value == null )
        throw new ArgumentNullException( "value" );

      writer.WriteStartElement( tagName, tagNamespace );
      writer.WriteAttributeString( ChartConstants.ValueAttribute, value );
      writer.WriteEndElement();
    }
      /// <summary>
      /// Seralize the double value 
      /// </summary>
      /// <param name="writer"></param>
      /// <param name="tagName"></param>
      /// <param name="tagNamespace"></param>
      /// <param name="value"></param>
    public static void SerializeDoubleValueTag(XmlWriter writer, string tagName, string tagNamespace, double value)
    {
        if (writer == null)
            throw new ArgumentNullException("writer");

        if (tagName == null)
            throw new ArgumentNullException("tagName");

        if (value == null)
            throw new ArgumentNullException("value");

        writer.WriteStartElement(tagName, tagNamespace);
        writer.WriteStartAttribute(ChartConstants.ValueAttribute);
        writer.WriteValue(value);
        writer.WriteEndAttribute();
        writer.WriteEndElement();
    }
    /// <summary>
    /// Serialize xml tag that contains value attribute with tag value.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="tagName">Tag name to serialize.</param>
    /// <param name="value">Value to serialize.</param>
    public static void SerializeBoolValueTag( XmlWriter writer, string tagName, bool value )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( tagName == null )
        throw new ArgumentNullException( "tagName" );


      writer.WriteStartElement( tagName, ChartConstants.CNamespace );

      string strValue = value ?
        Excel2007Serializator.TrueValue :
        Excel2007Serializator.FalseValue;

      writer.WriteAttributeString( ChartConstants.ValueAttribute, strValue );
      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes chart line properties.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="border">Border to serialize.</param>
    /// <param name="book">Parent workbook.</param>
    public static void SerializeLineProperties( XmlWriter writer, IChartBorder border, IWorkbook book )
    {
      SerializeLineProperties( writer, border, false, book, false );
    }
    /// <summary>
    /// Serializes pattern fill.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="color">Color to serialize.</param>
    /// <param name="bAutoColor">Indicates whether color is automatic and should be ignored or not.</param>
    /// <param name="strDash2007">Preset dash value.</param>
    /// <param name="strPreset">Preset pattern value.</param>
    /// <param name="book">Parent workbook.</param>
    public static void SerializePatternFill( XmlWriter writer, ColorObject color, bool bAutoColor,
      string strDash2007, string strPreset, IWorkbook book, double Alphavalue )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( strPreset == null || strPreset.Length == 0 )
      {
        SerializeSolidFill( writer, color, bAutoColor, book, Alphavalue );
      }
      else
      {
        ColorObject white = new ColorObject( ColorExtension.White );
        SerializePatternFill( writer, color, bAutoColor,
          white, bAutoColor, strPreset, book, Alphavalue );
      }

      //writer.WriteStartElement( Drawings.PresetDashTag,
      ChartSerializatorCommon.SerializeValueTag( writer, Drawings.PresetDashTag, Drawings.ANamespace, strDash2007 );
    }
    /// <summary>
    /// Serializes pattern fill.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="foreColor">Foreground color.</param>
    /// <param name="isAutoFore">Indicates whether foreground color is auto and foreColor argument should be ignored.</param>
    /// <param name="backColor">Background color.</param>
    /// <param name="isAutoBack">Indicates whether background color is auto and backColor argument should be ignored.</param>
    /// <param name="strPreset">Preset pattern value.</param>
    /// <param name="book">Parent workbook.</param>
    public static void SerializePatternFill( XmlWriter writer, ColorObject foreColor, bool isAutoFore,
      ColorObject backColor, bool isAutoBack, string strPreset, IWorkbook book, double Alphavalue )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( strPreset == null || strPreset.Length == 0 )
        throw new ArgumentOutOfRangeException( "strPreset" );

      writer.WriteStartElement( Drawings.PatternFillTag, Drawings.ANamespace );
      writer.WriteAttributeString( Drawings.PresetPattern, strPreset );

      if( !isAutoFore )
      {
        writer.WriteStartElement( Drawings.ForegroundColorTag, Drawings.ANamespace );
        SerializeRgbColor( writer, foreColor.GetRGB( book ), Alphavalue );
        writer.WriteEndElement();
      }

      if( !isAutoBack )
      {
        writer.WriteStartElement( Drawings.BackgroundColorTag, Drawings.ANamespace );
        SerializeRgbColor( writer, backColor.GetRGB( book ), Alphavalue );
        writer.WriteEndElement();
      }

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes pattern fill.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="foreColor">Foreground color.</param>
    /// <param name="isAutoFore">Indicates whether foreground color is auto and foreColor argument should be ignored.</param>
    /// <param name="backColor">Background color.</param>
    /// <param name="isAutoBack">Indicates whether background color is auto and backColor argument should be ignored.</param>
    /// <param name="pattern">Pattern to serialize.</param>
    /// <param name="book">Parent workbook.</param>
    public static void SerializePatternFill( XmlWriter writer, ColorObject foreColor, bool isAutoFore,
      ColorObject backColor, bool isAutoBack, ExcelGradientPattern pattern, IWorkbook book )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      writer.WriteStartElement( Drawings.PatternFillTag, Drawings.ANamespace );
      Excel2007GradientPattern pattern2007 = ( Excel2007GradientPattern )pattern;
      writer.WriteAttributeString( Drawings.PresetPattern, pattern2007.ToString() );

      if( !isAutoFore )
      {
        writer.WriteStartElement( Drawings.ForegroundColorTag, Drawings.ANamespace );
        SerializeRgbColor( writer, foreColor.GetRGB( book ) );
        writer.WriteEndElement();
      }

      if( !isAutoBack )
      {
        writer.WriteStartElement( Drawings.BackgroundColorTag, Drawings.ANamespace );
        SerializeRgbColor( writer, backColor.GetRGB( book ) );
        writer.WriteEndElement();
      }

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes solid fill.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="color">Color that is used for filling.</param>
    /// <param name="isAutoColor">Indicates whether color is automatic or was manually changed.</param>
    /// <param name="book">Parent workbook.</param>
    public static void SerializeSolidFill( XmlWriter writer, ColorObject color,
      bool isAutoColor, IWorkbook book, double alphavalue )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      writer.WriteStartElement( Drawings.SolidFillTag, Drawings.ANamespace );

      if( !isAutoColor )
        SerializeRgbColor( writer, color.GetRGB( book ), alphavalue );

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes rgb color.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="color">Color to serialize.</param>
    public static void SerializeRgbColor( XmlWriter writer, Color color )
    {
      SerializeRgbColor( writer, color, -1 );
    }
    /// <summary>
    /// Serializes rgb color.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="colorIndex">Color index to serialize.</param>
    /// <param name="book">Parent workbook.</param>
    public static void SerializeRgbColor( XmlWriter writer, ExcelKnownColors colorIndex, IWorkbook book )
    {
      SerializeRgbColor( writer, book.GetPaletteColor( colorIndex ), -1 );
    }
    /// <summary>
    /// Serializes rgb color.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="color">Color to serialize.</param>
    /// <param name="alphaValue">Alpha component of the color to serialize, 0 - 100000.</param>
    public static void SerializeRgbColor( XmlWriter writer, Color color, double alphaValue )
    {
      int alpha = Convert.ToInt32( alphaValue * ShapeFillImpl.MaxValue );
      SerializeRgbColor( writer, color, alpha, -1, -1 );
    }
    /// <summary>
    /// Serializes rgb color.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="color">Color to serialize.</param>
    /// <param name="alpha">Alpha component of the color to serialize, 0-100000.</param>
    /// <param name="tint">Tint value of the color to serialize, 0-100000.</param>
    /// <param name="shade">Shape value of the color to serialize, 0-100000.</param>
    public static void SerializeRgbColor( XmlWriter writer, Color color,
      int alpha, int tint, int shade )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );
      int Max_alpha = 100000;
      writer.WriteStartElement( Drawings.SRGBColorTag, Drawings.ANamespace );
      writer.WriteAttributeString( ChartConstants.ValueAttribute, ( color.ToArgb() & 0xFFFFFF ).ToString( "X6" ) );

      if ( alpha != ShapeFillImpl.MaxValue && alpha >= 0 && alpha <= Max_alpha)
      {
          writer.WriteStartElement(Drawings.AlphaTag, Drawings.ANamespace);
          writer.WriteAttributeString(ChartConstants.ValueAttribute, alpha.ToString());
          writer.WriteEndElement();     
      }

      if( shade >= 0 )
      {
        writer.WriteElementString( Drawings.GammaTag, Drawings.ANamespace, string.Empty );
        writer.WriteStartElement( Drawings.ShadeTag, Drawings.ANamespace );
        writer.WriteAttributeString( ChartConstants.ValueAttribute, shade.ToString() );
        writer.WriteEndElement();
        writer.WriteElementString( Drawings.InverseGammaTag, Drawings.ANamespace, string.Empty );
      }
      else if( tint >= 0 )
      {
        writer.WriteElementString( Drawings.GammaTag, Drawings.ANamespace, string.Empty );
        writer.WriteStartElement( Drawings.TintTag, Drawings.ANamespace );
        writer.WriteAttributeString( ChartConstants.ValueAttribute, tint.ToString() );
        writer.WriteEndElement();
        writer.WriteElementString( Drawings.InverseGammaTag, Drawings.ANamespace, string.Empty );
      }

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serialize line properties.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="border">Chart line properties to serialize.</param>
    /// <param name="bRoundCorners">Indicates whether border is rounded or not</param>
    /// <param name="book">Parent workbook.</param>
    private static void SerializeLineProperties( XmlWriter writer, IChartBorder border,
      bool bRoundCorners, IWorkbook book, bool serializeAutoFormat )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( border == null )//|| border.AutoFormat )
        return;

      ChartBorderImpl chartBorderImpl = border as ChartBorderImpl;
      writer.WriteStartElement( Drawings.LineTag, Drawings.ANamespace );

      if( !chartBorderImpl.AutoFormat || serializeAutoFormat )
      {
        int iLineWeight = ( short )border.LineWeight;

        if (iLineWeight != -1 && chartBorderImpl.LineWeightString == null)
        {
          iLineWeight = ( ( short )border.LineWeight + ( int )1 ) * 12700;

          if( iLineWeight == 0 )
          {
            iLineWeight = 12700;
          }

          writer.WriteAttributeString( Drawings.LineWidthAttribute, iLineWeight.ToString() );
        }
        else if( chartBorderImpl.LineWeightString != null )
        {
          writer.WriteAttributeString( Drawings.LineWidthAttribute, chartBorderImpl.LineWeightString );
        }
        else if (iLineWeight == -1 && !chartBorderImpl.HasLineProperties)
        {
            iLineWeight = 3175;
            writer.WriteAttributeString(Drawings.LineWidthAttribute, iLineWeight.ToString());
        }

        ExcelChartLinePattern linePattern = border.LinePattern;
        if (((ChartBorderImpl)border).HasLineProperties || ((WorkbookImpl)book).IsCreated || ((WorkbookImpl )book).IsConverted)
        {
            if (chartBorderImpl.HasGradientFill)
            {
                SerializeGradientFill(writer, chartBorderImpl.Fill, book);
            }
            else if (linePattern == ExcelChartLinePattern.None)
            {
                writer.WriteElementString(Drawings.NoFillTag, Drawings.ANamespace, string.Empty);
            }
            else if (linePattern == ExcelChartLinePattern.Solid)
            {
                SerializeSolidFill(writer, border.LineColor, border.IsAutoLineColor, book, 1 - border.Transparency);
                writer.WriteStartElement("prstDash", Drawings.ANamespace);
                writer.WriteAttributeString("val", "solid");
                writer.WriteEndElement();
            }
            else
            {
                KeyValuePair<string, string> pair = s_dicLinePatterns[linePattern];
                string strDash2007 = pair.Key;
                string strPreset = pair.Value;
                SerializePatternFill(writer, border.LineColor, border.IsAutoLineColor,
                  strDash2007, strPreset, book, 1 - border.Transparency);
                //throw new NotImplementedException();
            }
        }
        SerializeJoinType( writer, chartBorderImpl.JoinType );
      }

      //if( bRoundCorners )
      //  //writer.WriteElementString( Drawings.RoundTag, Drawings.ANamespace );
      //  SerializeBoolValueTag( writer, Drawings.RoundTag, true );

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serailize Line Join Type
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="joinType">Line Join Type</param>
    private static void SerializeJoinType( XmlWriter writer, Excel2007BorderJoinType joinType )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      string tagName = null;

      switch( joinType )
      {
        case Excel2007BorderJoinType.Bevel:
          tagName = Drawings.BevelJoinTag;
          break;

        case Excel2007BorderJoinType.Mitter:
          tagName = Drawings.MiterJoinTag;
          break;

        case Excel2007BorderJoinType.Round:
          tagName = Drawings.RoundTag;
          break;
      }

      if( tagName != null )
      {
        writer.WriteElementString( tagName, Drawings.ANamespace, string.Empty );
      }
    }
    /// <summary>
    /// Serializes picture fill.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="image">Picture to serialize.</param>
    /// <param name="holder">Parent file data holder object.</param>
    /// <param name="relations">Relations collection to add relation to.</param>
    /// <param name="tile">Indicates whether image should be tiled.</param>
    private static void SerializePictureFill( XmlWriter writer, Image image,
      FileDataHolder holder, RelationCollection relations, bool tile )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( image == null )
        throw new ArgumentNullException( "image" );

      if( holder == null )
        throw new ArgumentNullException( "holder" );

      if( relations == null )
        throw new ArgumentNullException( "relations" );

      string strLocation = holder.SaveImage( image, null );
      string strRelationId = relations.GenerateRelationId();
      relations[ strRelationId ] = new Relation( '/' + strLocation, RelationTypes.Image );

      writer.WriteStartElement( Drawings.BlipFillTagName, Drawings.ANamespace );

      writer.WriteStartElement( Drawings.BlipTagName, Drawings.ANamespace );
      writer.WriteAttributeString( Drawings.EmbeddedPicture, Excel2007Serializator.RelationNamespace,
        strRelationId );
      writer.WriteEndElement();

      if( tile )
      {
        //<a:tile tx="0" ty="0" sx="100000" sy="100000" flip="none" algn="tl" /> 
        writer.WriteStartElement( Drawings.TileTagName, Drawings.ANamespace );
        writer.WriteAttributeString( "tx", "0" );
        writer.WriteAttributeString( "ty", "0" );
        writer.WriteAttributeString( "sx", "100000" );
        writer.WriteAttributeString( "sy", "100000" );
        writer.WriteAttributeString( "flip", "none" );
        writer.WriteAttributeString( "algn", "tl" );
        writer.WriteEndElement();
      }
      else
      {
        writer.WriteStartElement( Drawings.StretchTagName, Drawings.ANamespace );
        writer.WriteElementString( Drawings.FillRectTagName, Drawings.ANamespace, string.Empty );
        writer.WriteEndElement();
      }

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serialize the picture with fillrect and source rect.
    /// </summary>     
    private static void SerializePictureFill(XmlWriter writer, Image image,
        FileDataHolder holder, RelationCollection relations,IInternalFill fill )
    {
        if (writer == null)
            throw new ArgumentNullException("writer");

        if (image == null)
            throw new ArgumentNullException("image");

        if (holder == null)
            throw new ArgumentNullException("holder");

        if (relations == null)
            throw new ArgumentNullException("relations");

        string strLocation = holder.SaveImage(image, null);
        string strRelationId = relations.GenerateRelationId();
        relations[strRelationId] = new Relation('/' + strLocation, RelationTypes.Image);

        writer.WriteStartElement(Drawings.BlipFillTagName, Drawings.ANamespace);

        writer.WriteStartElement(Drawings.BlipTagName, Drawings.ANamespace);
        writer.WriteAttributeString(Drawings.EmbeddedPicture, Excel2007Serializator.RelationNamespace,
          strRelationId);
        //check the alphamodfix
        if (fill.TransparencyColor != 0)
        {
            writer.WriteStartElement(Drawings.AlphaModFixTag, Drawings.ANamespace);
            writer.WriteAttributeString(Drawings.AlphaModFixattribute,((int) Math.Round((1-fill.TransparencyColor)*100000)).ToString());
            writer.WriteEndElement();
        }
        writer.WriteEndElement();

        if (fill.Tile)
        {
            //<a:tile tx="0" ty="0" sx="100000" sy="100000" flip="none" algn="tl" /> 
            writer.WriteStartElement(Drawings.TileTagName, Drawings.ANamespace);
            writer.WriteAttributeString(Drawings.HorizontalOffsetTag, (Math.Round(fill.TextureOffsetX)*12700).ToString());
            writer.WriteAttributeString(Drawings.VerticalOffsetTag, (Math.Round(fill.TextureOffsetY) * 12700).ToString());
            writer.WriteAttributeString(Drawings.HorizontalRatioTag, (Math.Round(fill.TextureHorizontalScale) * 100000).ToString());
            writer.WriteAttributeString(Drawings.VerticalRatioTag, (Math.Round(fill.TextureVerticalScale) * 100000).ToString());
            writer.WriteAttributeString(Drawings.TileFlippingTag, fill.TileFlipping);
            writer.WriteAttributeString(Drawings.AlignmentTag,fill.Alignment);
            writer.WriteEndElement();
        }
        else
        {
            //Serialize the source rectangle and fillrectangle
            writer.WriteStartElement(Drawings.SourceRectangleTagName, Drawings.ANamespace);
            Rectangle SourceRect = (fill as ShapeFillImpl).SourceRect;
            if (SourceRect.Right != 0)
                writer.WriteAttributeString(Drawings.RightAttribute, SourceRect.Right.ToString());
            if (SourceRect.Bottom != 0)
                writer.WriteAttributeString(Drawings.BottomAttribute, SourceRect.Bottom.ToString());
            if (SourceRect.Left != 0)
                writer.WriteAttributeString(Drawings.LeftAttribute, SourceRect.Left.ToString());
            if (SourceRect.Top != 0)
                writer.WriteAttributeString(Drawings.TopAttribute, SourceRect.Top.ToString());
            writer.WriteEndElement();
            writer.WriteStartElement(Drawings.StretchTagName, Drawings.ANamespace);            
            writer.WriteStartElement(Drawings.FillRectTagName, Drawings.ANamespace);
            Rectangle FillRect = (fill as ShapeFillImpl).FillRect;
            if (FillRect.Right != 0)
                writer.WriteAttributeString(Drawings.RightAttribute, FillRect.Right.ToString());
            if (FillRect.Bottom != 0)
                writer.WriteAttributeString(Drawings.BottomAttribute, FillRect.Bottom.ToString());
            if (FillRect.Left != 0)
                writer.WriteAttributeString(Drawings.LeftAttribute, FillRect.Left.ToString());
            if (FillRect.Top != 0)
                writer.WriteAttributeString(Drawings.TopAttribute, FillRect.Top.ToString());
            writer.WriteEndElement();
            writer.WriteEndElement();
        }

        writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes texture fill.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="fill">Fill object that contains settings to serialize.</param>
    /// <param name="holder">Parent file data holder object.</param>
    /// <param name="relations">Relations collection to add relation to.</param>
    private static void SerializeTextureFill( XmlWriter writer, IFill fill,
      FileDataHolder holder, RelationCollection relations )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( fill == null )
        throw new ArgumentNullException( "fill" );

      if( holder == null )
        throw new ArgumentNullException( "holder" );

      if( relations == null )
        throw new ArgumentNullException( "relations" );

      Image picture;
      ExcelTexture texture = fill.Texture;

      if( texture != ExcelTexture.User_Defined )
      {
#if ( WINRT )
        ResourceHandler resource = new ResourceHandler();
        byte[] arrData = resource.TextureArray[ShapeFillImpl.DEF_TEXTURE_PREFIX + ((int)texture).ToString()];
#else
        byte[] arrData = ShapeFillImpl.GetResData( ShapeFillImpl.DEF_TEXTURE_PREFIX + ( ( int )texture ).ToString() );
#endif
        byte[] arrPicture = new byte[ arrData.Length - 25 ];

        Array.Copy( arrData, 25, arrPicture, 0, arrPicture.Length );
        MemoryStream ms = new MemoryStream();

        ShapeFillImpl.UpdateBitMapHederToStream( ms, arrData );
        ms.Write( arrPicture, 0, arrPicture.Length );

        picture = ApplicationImpl.CreateImage( ms );
      }
      else
      {
        picture = fill.Picture;
      }

      SerializePictureFill( writer, picture, holder, relations, ( fill as IInternalFill ).Tile );
    }
    /// <summary>
    /// Serializes gradient fill.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="fill">Fill to serialize.</param>
    /// <param name="book">Parent workbook.</param>
    private static void SerializeGradientFill( XmlWriter writer, IFill fill, IWorkbook book )
    {
      // TODO: add required property to the interface.
      ShapeFillImpl shapeFill = ( ShapeFillImpl )fill;
      GradientStops gradientStops = shapeFill.GradientStops;
      GradientStops preservedGradient = shapeFill.PreservedGradient;
      GradientSerializator serializator = new GradientSerializator();

      if( gradientStops == null && !( fill as IInternalFill ).IsGradientSupported ||
        preservedGradient != null && preservedGradient[0].Position > 10000 || (preservedGradient!=null && HasSchemaColor(preservedGradient)))
      {
        gradientStops = preservedGradient;
        if (gradientStops.TileRect!=null)
        gradientStops.TileRect = shapeFill.PreservedGradient.TileRect;
      }
      
      serializator.Serialize(writer, gradientStops , book);
    }
    private static bool HasSchemaColor(GradientStops stops)
    {
        for (int i = 0; i < stops.Count; i++)
        {
            if (stops[i].ColorObject.IsSchemeColor)
                return true;
        }
            return false;
    }
    /// <summary>
    /// Serializes text properties.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="textArea">Text area to serialize text properties for.</param>
    private void SerializeTextProperties( XmlWriter writer, IChartTextArea textArea )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Serializes default text formatting.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="textFormatting">Text formatting to serialize.</param>
    /// <param name="book">Parent workbook.</param>
    private static void SerializeDefaultTextFormatting(XmlWriter writer,
      IChartTextArea textFormatting, IWorkbook book, double defaultFontSize)
    {
        if (writer == null)
            throw new ArgumentNullException("writer");

        if (book == null)
            throw new ArgumentNullException("book");

        if (textFormatting == null)
            return;

        writer.WriteStartElement(ChartConstants.TextPropertiesTag, ChartConstants.CNamespace);

        writer.WriteStartElement(Drawings.TextBodyPropertiesTag, Drawings.ANamespace);
        writer.WriteEndElement();

        writer.WriteStartElement(Drawings.ListStylesTag, Drawings.ANamespace);
        writer.WriteEndElement();

        writer.WriteStartElement(Drawings.Paragraphs, Drawings.ANamespace);
        writer.WriteStartElement(Drawings.ParagraphProperties, Drawings.ANamespace);

        ChartSerializatorCommon.SerializeParagraphRunProperites(writer, textFormatting,
          Drawings.DefaultParagraphProperites, book, defaultFontSize );

        writer.WriteEndElement();
        writer.WriteEndElement();

        writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes text from text area.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="textArea">Text area to serialize values for.</param>
    /// <param name="book">Parent workbook.</param>
    public static void SerializeTextAreaText( XmlWriter writer, IChartTextArea textArea, IWorkbook book, double defaultFontSize )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( textArea == null )
        throw new ArgumentNullException( "textArea" );

      writer.WriteStartElement(ChartConstants.ChartTextTag, ChartConstants.CNamespace);

      bool isFormula = false;

      if ((textArea is ChartTextAreaImpl) && (textArea as ChartTextAreaImpl).IsFormula)
          isFormula = true;
      else if ((textArea is ChartDataLabelsImpl) && (textArea as ChartDataLabelsImpl).IsFormula)
          isFormula = true;

      if (isFormula)
      {
          SerializeStringReference(writer, textArea);
      }
      else
          SerializeRichText(writer, textArea, book, ChartConstants.RichTextTag, defaultFontSize);
      
      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes rich text.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="textArea">Text area that contains rich text to serialize.</param>
    /// <param name="book">Parent workbook.</param>
    /// <param name="tagName">Name of the main xml tag.</param>
    public static void SerializeRichText( XmlWriter writer, IChartTextArea textArea,
      IWorkbook book, string tagName, double defaultFontSize )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( textArea == null )
        throw new ArgumentNullException( "textArea" );

      //writer.WriteStartElement( ChartConstants.RichTextTag, ChartConstants.CNamespace );
      writer.WriteStartElement( tagName, ChartConstants.CNamespace );
      SerializeBodyProperties( writer, textArea );
      SerializeListStyles( writer, textArea );
      SerializeParagraphs( writer, textArea, book, defaultFontSize );
      writer.WriteEndElement();
    }
    /// <summary>
    /// Serialize text area body properties.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="textArea">Text area to serialize body properties for.</param>
    private static void SerializeBodyProperties( XmlWriter writer, IChartTextArea textArea )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( textArea == null )
        throw new ArgumentNullException( "textArea" );

      writer.WriteStartElement( Drawings.TextBodyPropertiesTag, Drawings.ANamespace );
      IInternalChartTextArea typedArea = textArea as IInternalChartTextArea;

      if( typedArea.HasTextRotation )
      {
        int iAngle = typedArea.TextRotationAngle * ChartAxisSerializator.TextRotationMultiplier;
        writer.WriteAttributeString( Drawings.TextRotationAttribute, iAngle.ToString() );
        if (typedArea as ChartTextAreaImpl != null)
        {
            writer.WriteAttributeString(Drawings.TextBoxRotationAttribute, (typedArea as ChartTextAreaImpl).TextRotation.ToString());
        }
        else if (textArea is ChartDataLabelsImpl)
        {
            writer.WriteAttributeString(Drawings.TextBoxRotationAttribute, (textArea as ChartDataLabelsImpl).TextRotation.ToString());
        }
      }

      // TODO: on the current moment we don't support body properties.
      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes list styles for a text area.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="textArea">Text area to serialize list styles for.</param>
    private static void SerializeListStyles( XmlWriter writer, IChartTextArea textArea )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( textArea == null )
        throw new ArgumentNullException( "textArea" );

      writer.WriteStartElement( Drawings.ListStylesTag, Drawings.ANamespace );
      // TODO: on the current moment we don't support list styles.
      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes paragraph.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize paragraph tag into.</param>
    /// <param name="textArea">Text area that contains paragraph information (formatting and text).</param>
    /// <param name="book">Parent workbook.</param>
    private static void SerializeParagraphs( XmlWriter writer, IChartTextArea textArea, IWorkbook book, double defaultFontSize )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( textArea == null )
        throw new ArgumentNullException( "textArea" );

      //Setting rich-text from 2003 to 2007 and above formats
      if ((textArea as ChartTextAreaImpl) != null &&
          (textArea as ChartTextAreaImpl).ChartAlRuns != null &&
          (textArea as ChartTextAreaImpl).ChartAlRuns.Runs != null &&
          (textArea as ChartTextAreaImpl).ChartAlRuns.Runs.Length > 0)
      {
          Serialize_TextArea_RichTextParagraph(writer, textArea, book, defaultFontSize);
      }
      else if ((textArea as ChartDataLabelsImpl) != null &&
               (textArea as ChartDataLabelsImpl).TextArea.ChartAlRuns != null &&
               (textArea as ChartDataLabelsImpl).TextArea.ChartAlRuns.Runs != null &&
               (textArea as ChartDataLabelsImpl).TextArea.ChartAlRuns.Runs.Length > 0)
      {
          Serialize_DataLabel_RichTextParagraph(writer, textArea, book, defaultFontSize);
      }
      else
      {
          string[] textItems = textArea.Text.Split('\n');

          for (int i = 0, len = textItems.Length; i < len; i++)
          {
              SerializeSingleParagraph(writer, textArea, textItems[i], book, defaultFontSize);
          }
      }
    }

    /// <summary>
    /// Serializing the rich-text formatting 
    /// </summary>
    /// <param name="writer">XmlWriter to serialize paragraph tag into.</param>
    /// <param name="textArea">Text area that contains paragraph information (formatting and text) of text area.</param>
    /// <param name="book">Parent workbook.</param>
    /// <param name="defaultFontSize">Default font size</param>
    private static void Serialize_TextArea_RichTextParagraph(XmlWriter writer, IChartTextArea textArea, 
        IWorkbook book, double defaultFontSize)
    {
        if (writer == null)
            throw new ArgumentNullException("writer");

        if (textArea == null)
            throw new ArgumentNullException("textArea");

        if (book == null)
            throw new ArgumentNullException("book");

        writer.WriteStartElement(Drawings.Paragraphs, Drawings.ANamespace);
        writer.WriteStartElement(Drawings.ParagraphProperties, Drawings.ANamespace);
        ChartSerializatorCommon.SerializeParagraphRunProperites(writer, textArea,
         Drawings.DefaultParagraphProperites, book, defaultFontSize);
        writer.WriteEndElement();

        //Setting rich-text from 2003 to 2007 and above formats
        for (int i = 0; i < (textArea as ChartTextAreaImpl).ChartAlRuns.Runs.Length; i++)
        {
            writer.WriteStartElement(Drawings.ParagraphRun, Drawings.ANamespace);

            int startIndex = 0;
            int endIndex = 0;

            startIndex = (textArea as ChartTextAreaImpl).ChartAlRuns.Runs[i].FirstCharIndex;

            if (i < (textArea as ChartTextAreaImpl).ChartAlRuns.Runs.Length - 1)
                endIndex = ((textArea as ChartTextAreaImpl).ChartAlRuns.Runs[i + 1].FirstCharIndex) - startIndex;
            else
                endIndex = (textArea.Text.Length - (textArea as ChartTextAreaImpl).ChartAlRuns.Runs[i].FirstCharIndex);

            string text = null;

            text = textArea.Text.Substring(startIndex, endIndex);
            if (i==0 && (textArea as ChartTextAreaImpl).ChartAlRuns.Runs.Length < 2)
            {
                int index = (textArea as IInternalFont).Index;
                (textArea as ChartTextAreaImpl).SetFontIndex(index);
            }
            else
            (textArea as ChartTextAreaImpl).SetFontIndex((textArea as ChartTextAreaImpl).ChartAlRuns.Runs[i].FontIndex);

            SerializeParagraphRunProperites(writer, textArea, Drawings.TextRunProperites, book, defaultFontSize);

            writer.WriteStartElement(Drawings.ParagraphText, Drawings.ANamespace);
            writer.WriteString(text);
            writer.WriteEndElement();

            writer.WriteEndElement();
        }

        writer.WriteEndElement();
    }
    /// <summary>
    /// Serializing the rich-text formatting 
    /// </summary>
    /// <param name="writer">XmlWriter to serialize paragraph tag into.</param>
    /// <param name="textArea">Text area that contains paragraph information (formatting and text) of data labels.</param>
    /// <param name="book">Parent workbook.</param>
    /// <param name="defaultFontSize">Default font size</param>
    private static void Serialize_DataLabel_RichTextParagraph(XmlWriter writer, IChartTextArea chartTextArea,
        IWorkbook book, double defaultFontSize)
    {
        if (writer == null)
            throw new ArgumentNullException("writer");

        if (chartTextArea == null)
            throw new ArgumentNullException("textArea");

        if (book == null)
            throw new ArgumentNullException("book");

        writer.WriteStartElement(Drawings.Paragraphs, Drawings.ANamespace);
        writer.WriteStartElement(Drawings.ParagraphProperties, Drawings.ANamespace);
        writer.WriteStartElement(Drawings.DefaultParagraphProperites, Drawings.ANamespace);
        writer.WriteEndElement();
        writer.WriteEndElement();

        //Setting rich-text from 2003 to 2007 and above formats
        ChartTextAreaImpl textArea = (chartTextArea as ChartDataLabelsImpl).TextArea;
        int runsLength = textArea.ChartAlRuns.Runs.Length;

        for (int i = 0; i < runsLength; i++)
        {
            writer.WriteStartElement(Drawings.ParagraphRun, Drawings.ANamespace);

            int startIndex = 0;
            int endIndex = 0;

            startIndex = textArea.ChartAlRuns.Runs[i].FirstCharIndex;

            if (i < runsLength - 1)
                endIndex = (textArea.ChartAlRuns.Runs[i + 1].FirstCharIndex) - startIndex;
            else
                endIndex = (textArea.Text.Length - textArea.ChartAlRuns.Runs[i].FirstCharIndex);

            string text = null;

            text = textArea.Text.Substring(startIndex, endIndex);

            if (runsLength > 1)
                textArea.SetFontIndex(textArea.ChartAlRuns.Runs[i].FontIndex);

            SerializeParagraphRunProperites(writer, textArea, Drawings.TextRunProperites, book, defaultFontSize);

            writer.WriteStartElement(Drawings.ParagraphText, Drawings.ANamespace);
            writer.WriteString(text);
            writer.WriteEndElement();

            writer.WriteEndElement();
        }

        writer.WriteEndElement();
    }
    private static void SerializeSingleParagraph( XmlWriter writer, IChartTextArea textArea,
      string paragraphText, IWorkbook book, double defaultFontSize )
    {
      writer.WriteStartElement( Drawings.Paragraphs, Drawings.ANamespace );
      writer.WriteStartElement( Drawings.ParagraphProperties, Drawings.ANamespace );
      writer.WriteStartElement( Drawings.DefaultParagraphProperites, Drawings.ANamespace );
      writer.WriteEndElement();
      writer.WriteEndElement();

      writer.WriteStartElement( Drawings.ParagraphRun, Drawings.ANamespace );

      SerializeParagraphRunProperites(writer, textArea, Drawings.TextRunProperites, book, defaultFontSize);

      writer.WriteStartElement( Drawings.ParagraphText, Drawings.ANamespace );
      writer.WriteString( paragraphText );
      writer.WriteEndElement();

      writer.WriteEndElement();
      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes paragraph run.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize paragraph tag into.</param>
    /// <param name="textArea">Text area that contains paragraph run information (formatting and text).</param>
    /// <param name="mainTagName">Name of the main xml tag.</param>
    /// <param name="book">Parent workbook.</param>
    public static void SerializeParagraphRunProperites( XmlWriter writer, IFont textArea,
      string mainTagName, IWorkbook book, double defaultFontSize )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( textArea == null )
        throw new ArgumentNullException( "textArea" );

      if( mainTagName == null || mainTagName.Length == 0 )
        throw new ArgumentException( "mainTagName" );

      writer.WriteStartElement( mainTagName, Drawings.ANamespace );

      string strBold = textArea.Bold ?
        Excel2007Serializator.TrueValue :
        Excel2007Serializator.FalseValue;

      string strItalic = textArea.Italic ?
        Excel2007Serializator.TrueValue :
        Excel2007Serializator.FalseValue;

      IInternalFont chartTextArea = ( textArea as IInternalFont );

      if( chartTextArea != null )
      {
        string strLanguage = chartTextArea.Font.Language;

        if( strLanguage != null )
        {
          writer.WriteAttributeString( Drawings.FontLanguage, strLanguage );
        }
      }

      if ((textArea.Bold) || 
          (chartTextArea.Font.m_textSettings != null && chartTextArea.Font.m_textSettings.Bold != null) ||
          ((chartTextArea as ChartDataLabelsImpl) != null && (chartTextArea as ChartDataLabelsImpl).ShowBoldProperties) ||
          ((chartTextArea as ChartTextAreaImpl) != null && (chartTextArea as ChartTextAreaImpl).ShowBoldProperties))
           writer.WriteAttributeString(Drawings.FontBoldAttribute, strBold);

      if ((textArea.Italic) || (chartTextArea.Font.m_textSettings != null && chartTextArea.Font.m_textSettings.Italic != null))
          writer.WriteAttributeString(Drawings.FontItalicAttribute, strItalic);

      if( textArea.Strikethrough )
        writer.WriteAttributeString( Drawings.FontStrikeAttribute, ChartConstants.StrikeThroughSingle );

      if (textArea.Size != defaultFontSize ||
          (chartTextArea.Font.m_textSettings != null && chartTextArea.Font.m_textSettings.ShowSizeProperties == true) ||
          ((chartTextArea as ChartDataLabelsImpl) != null && (chartTextArea as ChartDataLabelsImpl).ShowSizeProperties) ||
          ((chartTextArea as ChartTextAreaImpl) != null && (chartTextArea as ChartTextAreaImpl).ShowSizeProperties))
      {
          int iSize = (int)(textArea.Size * 100);
          writer.WriteAttributeString(Drawings.FontSizeAttribute, iSize.ToString());
      }

      if( textArea.Underline != ExcelUnderline.None )
      {
        string strUnderline = ( textArea.Underline == ExcelUnderline.Single ) ?
          ChartConstants.UnderlineSingle :
          ChartConstants.UnderlineDouble;
        writer.WriteAttributeString( Drawings.FontUnterlineAttribute, strUnderline );
      }

      int iBaseline = 0;

      if (textArea.Superscript || textArea.Subscript)
      {
          if ((textArea as ChartTextAreaImpl) != null && (textArea as ChartTextAreaImpl).Font != null)
              iBaseline = (textArea as ChartTextAreaImpl).Font.BaseLine;
          else if ((textArea as FontWrapper) != null)
              iBaseline = (textArea as FontWrapper).Baseline;
      }

      writer.WriteAttributeString( Drawings.Baseline, iBaseline.ToString() );

      // TODO: add color serialization later.
      
      if (!textArea.IsAutoColor || ((textArea is FontWrapper)&&(book as WorkbookImpl).IsConverted))//&& textArea.RGBColor.ToArgb()!=Color.Black.ToArgb()&&textArea.RGBColor.ToArgb()!=Color.Empty.ToArgb())
      {
        writer.WriteStartElement( Drawings.SolidFillTag, Drawings.ANamespace );
        SerializeRgbColor(writer, textArea.RGBColor, (-1 * ShapeFillImpl.MaxValue), -1, -1);
        writer.WriteEndElement();
      }

      if (textArea.FontName != ChartAxisParser.DefaultFont)
      {
          writer.WriteStartElement(Drawings.LatinTag, Drawings.ANamespace);
          writer.WriteAttributeString(Drawings.TypefaceTag, textArea.FontName);
          writer.WriteEndElement();

          writer.WriteStartElement("ea", Drawings.ANamespace);
          writer.WriteAttributeString(Drawings.TypefaceTag, textArea.FontName);
          writer.WriteEndElement();

          writer.WriteStartElement("cs", Drawings.ANamespace);
          writer.WriteAttributeString(Drawings.TypefaceTag, textArea.FontName);
          writer.WriteEndElement();
      }
      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes string reference settings.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="textArea">Text area to get settings from.</param>
    private static void SerializeStringReference( XmlWriter writer, IChartTextArea textArea )
    {
        string range = textArea.Text;
        if (writer == null)
            throw new ArgumentNullException("writer");

        if (range[0] == '=')
        {
           range  = UtilityMethods.RemoveFirstCharUnsafe(range);
        }

        writer.WriteStartElement(ChartConstants.StringReferenceTag, ChartConstants.CNamespace);
        writer.WriteElementString(ChartConstants.Formula, ChartConstants.CNamespace, range);      
        writer.WriteElementString(ChartConstants.StringCacheTag, ChartConstants.CNamespace, string.Empty);
        
        writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes text area layout settings.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="textArea">Text area to serialize layout settings for.</param>
    public static void SerializeLayout( XmlWriter writer, object textArea )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( textArea == null )
        throw new ArgumentNullException( "textArea" );

      IChartLayout layout = null;

      if (textArea is ChartTextAreaImpl)
          layout = (textArea as ChartTextAreaImpl).Layout;
      else if (textArea is ChartPlotAreaImpl)
          layout = (textArea as ChartPlotAreaImpl).Layout;
      else if (textArea is ChartDataLabelsImpl)
          layout = (textArea as ChartDataLabelsImpl).Layout;
      else if (textArea is ChartLegendImpl)
          layout = (textArea as ChartLegendImpl).Layout;
              
      if (layout != null)
      {
          writer.WriteStartElement(ChartConstants.LayoutTag, ChartConstants.CNamespace);

          if (layout.ManualLayout != null)
              SerializeManualLayout(writer, layout.ManualLayout);

          writer.WriteEndElement();
      }
    }
    /// <summary>
    /// Serializes text area manual layout settings.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="textArea">Manual layout to serialize manual layout settings for.</param>
    public static void SerializeManualLayout(XmlWriter writer, IChartManualLayout manualLayout)
    {
        if (writer == null)
            throw new ArgumentNullException("writer");

        if (manualLayout == null)
            throw new ArgumentNullException("manualLayout");

        if (manualLayout != null)
        {
            writer.WriteStartElement(ChartConstants.ManualLayoutTag, ChartConstants.CNamespace);

            if (manualLayout.LayoutTarget != null && manualLayout.LayoutTarget != LayoutTargets.auto)
                SerializeValueTag(writer, ChartConstants.LayoutTargetTag, manualLayout.LayoutTarget.ToString());

            if (manualLayout.LeftMode != null && manualLayout.LeftMode != LayoutModes.auto)
                SerializeValueTag(writer, ChartConstants.LeftModeTag, manualLayout.LeftMode.ToString());

            if (manualLayout.TopMode != null && manualLayout.TopMode != LayoutModes.auto)
                SerializeValueTag(writer, ChartConstants.TopModeTag, manualLayout.TopMode.ToString());

            if (manualLayout.WidthMode != null && manualLayout.WidthMode != LayoutModes.auto)
                SerializeValueTag(writer, ChartConstants.WidthModeTag, manualLayout.WidthMode.ToString());

            if (manualLayout.HeightMode != null && manualLayout.HeightMode != LayoutModes.auto)
                SerializeValueTag(writer, ChartConstants.HeightModeTag, manualLayout.HeightMode.ToString());

            if ((manualLayout.Left != null && manualLayout.Top != null)&&(manualLayout.Left != 0||manualLayout.Top != 0))
            {
                SerializeDoubleValueTag(writer, ChartConstants.LeftTag, manualLayout.Left);           
                SerializeDoubleValueTag(writer, ChartConstants.TopTag, manualLayout.Top);
            }

            if ((manualLayout.Width != null && manualLayout.Height != null) &&(manualLayout.Width != 0|| manualLayout.Height != 0))
            {
                SerializeDoubleValueTag(writer, ChartConstants.WidthTag, manualLayout.Width);           
                SerializeDoubleValueTag(writer, ChartConstants.HeightTag, manualLayout.Height);
            }

            writer.WriteEndElement();
        }
    }
    /// <summary>
    /// Serializes overlay setting of the text area.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize overlay into.</param>
    /// <param name="textArea">Text area to get overlay from.</param>
    private static void SerializeOverlay( XmlWriter writer, IChartTextArea textArea )
    {
        if (textArea == null)
            throw new ArgumentNullException("textArea");

        Stream overlayStream= ((ChartTextAreaImpl)textArea).OverlayStream;

        if ( overlayStream !=null)            
        {
            overlayStream.Position = 0;
            ShapeParser.WriteNodeFromStream(writer, overlayStream);
        }
        else
        {
            ChartSerializatorCommon.SerializeValueTag(writer, ChartConstants.OverlayTag, "0");
        }
     

    }
    #endregion
  }
}
