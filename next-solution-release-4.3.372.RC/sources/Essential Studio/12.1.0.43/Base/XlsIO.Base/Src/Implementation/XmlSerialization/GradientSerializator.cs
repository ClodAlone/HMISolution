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

using Syncfusion.XlsIO.Implementation.Shapes;
using Syncfusion.XlsIO.Implementation.XmlSerialization.Shapes;
using Syncfusion.XlsIO.Implementation.XmlSerialization.Charts;
#if ( WINRT )
using Windows.UI;
using Syncfusion.XlsIO.Implementation.WINRT;
#endif
#if (SILVERLIGHT)
using System.Windows.Media;
using Syncfusion.XlsIO.Implementation.Silverlight;
#elif WP
using System.Windows.Media;
using Syncfusion.XlsIO.Implementation.WP;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;


#endif

namespace Syncfusion.XlsIO.Implementation.XmlSerialization
{
  /// <summary>
  /// This class is responsible for gradient serialization.
  /// </summary>
  class GradientSerializator
  {
    /// <summary>
    /// Serializes gradient stops collection into specified XmlWriter.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="gradientStops">Collection to serialize.</param>
    /// <param name="book">Parent workbook object.</param>
    public void Serialize( XmlWriter writer, GradientStops gradientStops, IWorkbook book )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( gradientStops == null )
        throw new ArgumentNullException( "gradientStops" );

      writer.WriteStartElement( Drawings.GradientFillTag, Drawings.ANamespace );
      SerializeGradientStops( writer, gradientStops, book );
      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes collection of gradient stops.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="gradientStops">Collection to serialize.</param>
    /// <param name="book">Parent workbook.</param>
    private void SerializeGradientStops( XmlWriter writer, GradientStops gradientStops, IWorkbook book )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( gradientStops == null )
        throw new ArgumentNullException( "gradientStops" );

      writer.WriteStartElement( Drawings.GradientStopsTag, Drawings.ANamespace );

      for( int i = 0, len = gradientStops.Count; i < len; i++ )
      {
        SerializeGradientStop( writer, gradientStops[ i ], book );
      }

      writer.WriteEndElement();

      if( gradientStops.GradientType == GradientType.Liniar )
      {
        writer.WriteStartElement( Drawings.GradientLiniarTag, Drawings.ANamespace );
        writer.WriteAttributeString( Drawings.GradientAngleAttribute, gradientStops.Angle.ToString() );
        writer.WriteAttributeString( Drawings.GradientScaledAttribute, "1" );
        writer.WriteEndElement();
      }
      else
      {
        writer.WriteStartElement( Drawings.GradientPathTag, Drawings.ANamespace );
        string strPath = gradientStops.GradientType.ToString().ToLower();
        writer.WriteAttributeString( Drawings.GradientPathAttribute, strPath );

        Rectangle rect = gradientStops.FillToRect;

        if( !( rect.Left == 0 && rect.Top == 0 && rect.Right == 0 && rect.Bottom == 0 ) )
        {
          writer.WriteStartElement( Drawings.FillToRectTag, Drawings.ANamespace );

          int iLeft = rect.Left;

          if( iLeft != 0 )
            writer.WriteAttributeString( Drawings.LeftAttribute, iLeft.ToString() );

          int iTop = rect.Top;

          if( iTop != 0 )
            writer.WriteAttributeString( Drawings.TopAttribute, iTop.ToString() );

          int iRight = rect.Right;

          if (iRight != 0)
          writer.WriteAttributeString( Drawings.RightAttribute, rect.Right.ToString() );

          int iBottom = rect.Bottom;

          if (iBottom!=0)
          writer.WriteAttributeString( Drawings.BottomAttribute, rect.Bottom.ToString() );
          writer.WriteEndElement();
        }

        writer.WriteEndElement();
      }

        //for tillrect.................
      Rectangle till = gradientStops.TileRect;
      if ((till.Left != 0 || till.Top != 0 || till.Right != 0 || till.Bottom != 0))
      {
          writer.WriteStartElement(Drawings.GradientTailTag, Drawings.ANamespace);
          int TRight = till.Right;

          if (TRight != 0)
              writer.WriteAttributeString(Drawings.RightAttribute, TRight.ToString());

          int TBottem = till.Bottom;

          if (TBottem != 0)
              writer.WriteAttributeString(Drawings.BottomAttribute, TBottem.ToString());

          int TLeft = till.Left;

          if (TLeft != 0)
              writer.WriteAttributeString(Drawings.LeftAttribute, TLeft.ToString());
          int TTop = till.Top;

          if (TTop != 0)
              writer.WriteAttributeString(Drawings.TopAttribute, TTop.ToString());


          writer.WriteEndElement();
      }
        //tilltag end............................................//
    }
    /// <summary>
    /// Serializes single gradient stop.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="gradientStop">GradientStop to serialize.</param>
    /// <param name="book">Parent workbook.</param>
    private void SerializeGradientStop( XmlWriter writer, GradientStopImpl gradientStop, IWorkbook book )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( gradientStop == null )
        throw new ArgumentNullException( "gradientStop" );

      writer.WriteStartElement( Drawings.GradientStopTag, Drawings.ANamespace );
      writer.WriteAttributeString( Drawings.GradientPositionAttribute, gradientStop.Position.ToString() );
      if (!gradientStop.ColorObject.IsSchemeColor)
          ChartSerializatorCommon.SerializeRgbColor(writer, gradientStop.ColorObject.GetRGB(book),
            gradientStop.Transparency, gradientStop.Tint, gradientStop.Shade);
      else
      {
          SerializeSchemeColor(writer, gradientStop,book);
      }
      writer.WriteEndElement();
    }
    private void SerializeSchemeColor(XmlWriter writer,GradientStopImpl gradienstop,IWorkbook book)
    {
        writer.WriteStartElement(Drawings.SchemeColorTag,Drawings.ANamespace);
        writer.WriteAttributeString(Drawings.Valueattribite, gradienstop.ColorObject.SchemaName);
        ColorObject colorObj = gradienstop.ColorObject;
        if (colorObj.Tint > 0)
            ChartSerializatorCommon.SerializeDoubleValueTag(writer,Drawings.TintTag,Drawings.ANamespace,colorObj.Tint);
        if (colorObj.Saturation > 0)
            ChartSerializatorCommon.SerializeDoubleValueTag(writer, Drawings.SaturationModulation, Drawings.ANamespace, colorObj.Saturation);
        if (colorObj.Luminance > 0)
            ChartSerializatorCommon.SerializeDoubleValueTag(writer, Drawings.LuminanceModulation, Drawings.ANamespace, colorObj.Luminance);
        if (colorObj.LuminanceOffSet > 0)
            ChartSerializatorCommon.SerializeDoubleValueTag(writer, Drawings.LuminanceOffset, Drawings.ANamespace, colorObj.LuminanceOffSet);
        if (gradienstop.Transparency > 0)
            ChartSerializatorCommon.SerializeDoubleValueTag(writer, Drawings.AlphaTag, Drawings.ANamespace, gradienstop.Transparency);
        if(gradienstop.Shade>0)
            ChartSerializatorCommon.SerializeDoubleValueTag(writer, Drawings.ShadeTag, Drawings.ANamespace, gradienstop.Shade);
        writer.WriteEndElement();
    }
  }
}
