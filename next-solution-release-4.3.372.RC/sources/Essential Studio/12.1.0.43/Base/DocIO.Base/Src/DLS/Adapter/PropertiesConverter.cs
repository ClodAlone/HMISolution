#region Copyright Syncfusion Inc. 2001 - 2014
//                                                                 m
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
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Threading;
using Syncfusion.DocIO.ReaderWriter;
using Syncfusion.DocIO.ReaderWriter.Biff_Records;
using Syncfusion.DocIO.ReaderWriter.Biff_Records.Structures;
using Syncfusion.DocIO.ReaderWriter.DataStreamParser;
using Syncfusion.DocIO.ReaderWriter.DataStreamParser.Escher;
using Syncfusion.DocIO.ReaderWriter.Escher;
using Syncfusion.Documentation;
using DocIOListLevel = Syncfusion.DocIO.ReaderWriter.Biff_Records.ListLevel;
using System.Collections.Generic;
#if !WINRT && !WP
using System.Drawing;
#endif
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// The CHPX converter.
    /// </summary>
    [DocumentationExclude()]
    internal class CharacterPropertiesConverter
    {
        #region Members
        private static List<int> m_incorrectOptions;
        static readonly object m_threadLocker = new object();
        #endregion

        #region Properties
        private static List<int> IncorrectOptions 
        {
            get 
            {
                if (m_incorrectOptions == null)
                {
                    // Init incorrect sprms options
                    m_incorrectOptions = new List<int>();
                    m_incorrectOptions.Add(0);
                    m_incorrectOptions.Add(10);
                    m_incorrectOptions.Add(15);
                    m_incorrectOptions.Add(16);
                    m_incorrectOptions.Add(17);
                    m_incorrectOptions.Add(30);
                    m_incorrectOptions.Add(31);
                    m_incorrectOptions.Add(32);
                    m_incorrectOptions.Add(33);
                    m_incorrectOptions.Add(35);
                    m_incorrectOptions.Add(36);
                    m_incorrectOptions.Add(40); // Unnecessary track change
                    m_incorrectOptions.Add(42);
                    m_incorrectOptions.Add(43);
                    m_incorrectOptions.Add(44);
                    m_incorrectOptions.Add(45); // Unnecessary track change
                    m_incorrectOptions.Add(46); // Unnecessary track change
                    m_incorrectOptions.Add(50);
                    m_incorrectOptions.Add(256);
                    m_incorrectOptions.Add(512);
                    //m_incorrectOptions.Add(2048); // Unnecessary track change
                    m_incorrectOptions.Add(2304);
                    m_incorrectOptions.Add(2560);
                    m_incorrectOptions.Add(3328);
                    m_incorrectOptions.Add(3968);
                    m_incorrectOptions.Add(4096);
                    m_incorrectOptions.Add(4104);
                    m_incorrectOptions.Add(4185);
                    m_incorrectOptions.Add(4460);
                    m_incorrectOptions.Add(4468);
                    m_incorrectOptions.Add(6912);
                    m_incorrectOptions.Add(7914);
                    m_incorrectOptions.Add(8163);
                    m_incorrectOptions.Add(8169);
                    m_incorrectOptions.Add(8207);
                    m_incorrectOptions.Add(8325);
                    m_incorrectOptions.Add(8482);
                    m_incorrectOptions.Add(8520);
                    m_incorrectOptions.Add(8933);
                    m_incorrectOptions.Add(8939);
                    m_incorrectOptions.Add(8960);
                    m_incorrectOptions.Add(9080);
                    m_incorrectOptions.Add(9088);
                    m_incorrectOptions.Add(9212);
                    m_incorrectOptions.Add(9253);
                    m_incorrectOptions.Add(9367);
                    m_incorrectOptions.Add(9728);
                    m_incorrectOptions.Add(9984);
                    m_incorrectOptions.Add(10446);
                    m_incorrectOptions.Add(10752);
                    m_incorrectOptions.Add(10789);
                    m_incorrectOptions.Add(10876);
                    m_incorrectOptions.Add(11015);
                    m_incorrectOptions.Add(11061);
                    m_incorrectOptions.Add(11176);
                    m_incorrectOptions.Add(11493);
                    m_incorrectOptions.Add(11603);
                    m_incorrectOptions.Add(11776);
                    m_incorrectOptions.Add(12897);
                    m_incorrectOptions.Add(12929);
                    m_incorrectOptions.Add(13028);
                    m_incorrectOptions.Add(13036);
                    m_incorrectOptions.Add(13063);
                    m_incorrectOptions.Add(13287);
                    m_incorrectOptions.Add(13298);
                    m_incorrectOptions.Add(13328);
                    m_incorrectOptions.Add(13824);
                    m_incorrectOptions.Add(17408);
                    m_incorrectOptions.Add(19968);
                    m_incorrectOptions.Add(20224);
                    m_incorrectOptions.Add(21504);
                    m_incorrectOptions.Add(21760);
                    m_incorrectOptions.Add(22016);
                    m_incorrectOptions.Add(24064);
                    m_incorrectOptions.Add(24320);
                    m_incorrectOptions.Add(26624);
                    m_incorrectOptions.Add(27136); // Incorrect character spacing
                    m_incorrectOptions.Add(27904);
                    m_incorrectOptions.Add(29952);
                    m_incorrectOptions.Add(30976);
                    m_incorrectOptions.Add(31488);
                    m_incorrectOptions.Add(32000);
                    m_incorrectOptions.Add(32280);
                    m_incorrectOptions.Add(32768);
                    m_incorrectOptions.Add(33024);
                    m_incorrectOptions.Add(33536);
                    m_incorrectOptions.Add(34816);
                    m_incorrectOptions.Add(35328);
                    m_incorrectOptions.Add(35584);
                    m_incorrectOptions.Add(35840);
                    //m_incorrectOptions.Add(38400);
                    m_incorrectOptions.Add(38656);
                    m_incorrectOptions.Add(40192);
                    m_incorrectOptions.Add(40960);
                    m_incorrectOptions.Add(41984);
                    m_incorrectOptions.Add(42240);
                    m_incorrectOptions.Add(42496);
                    m_incorrectOptions.Add(43008);
                    m_incorrectOptions.Add(43520);
                    m_incorrectOptions.Add(43776);
                    m_incorrectOptions.Add(44032);
                    m_incorrectOptions.Add(44288);
                    m_incorrectOptions.Add(45568);
                    m_incorrectOptions.Add(45824);
                    m_incorrectOptions.Add(46080);
                    m_incorrectOptions.Add(46336);
                    m_incorrectOptions.Add(47104);
                    m_incorrectOptions.Add(47360);
                    m_incorrectOptions.Add(47616);
                    m_incorrectOptions.Add(47872);
                    m_incorrectOptions.Add(48128);
                    m_incorrectOptions.Add(49408);
                    m_incorrectOptions.Add(52992);
                    m_incorrectOptions.Add(53504);
                    m_incorrectOptions.Add(58112);
                    m_incorrectOptions.Add(58368);
                    m_incorrectOptions.Add(58880);
                    m_incorrectOptions.Add(59904);
                    m_incorrectOptions.Add(60160);
                    m_incorrectOptions.Add(60672);
                    m_incorrectOptions.Add(61696);
                    m_incorrectOptions.Add(64000);
                    m_incorrectOptions.Add(64256);
                }
                return m_incorrectOptions;
            }
        }
        #endregion

        #region Class utility method
        /// <summary>
        /// CHPs to format.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="format">The format.</param>
        public static void CHPToFormat(IWordReaderBase reader, WCharacterFormat format)
        {
            lock (m_threadLocker)
            {
                CHPToFormat(reader.CharacterProperties, format);
            }
        }
        /// <summary>
        /// CHPs to format.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        public static void CHPToFormat(CharacterProperties source, WCharacterFormat destination)
        {
            lock (m_threadLocker)
            {
                if (source.CharacterPropertyException == null || source.Sprms == null)
                    return;

                destination.Sprms = source.Sprms;
                destination.SetLanguages(source);
                destination.SetFontNames(source);
                destination.SetComplexScript(source);
                //Set character style name
                SinglePropertyModifierRecord sprm;
                if (!(destination.OwnerBase is Style))
                {
                    sprm = source.Sprms[WordSprmOptions.sprmCIstd];
                    if (sprm != null)
                    {
                        destination.CharStyleName = source.StyleSheet.GetStyleByIndex(sprm.UshortValue).Name;
                    }
                    sprm = source.GetNewSprm(WordSprmOptions.sprmCIstd);
                    if (sprm != null)
                    {
                        destination.NewCharStyleName = source.StyleSheet.GetStyleByIndex(sprm.UshortValue).Name;
                    }
                }
                if (destination.Sprms.Contain(WordSprmOptions.sprmCFBold) && destination.Sprms.Contain(WordSprmOptions.sprmCFBoldBi))
                {
                    sprm = new SinglePropertyModifierRecord(WordSprmOptions.sprmCUndocumented6816);
                    sprm.IntValue = 13531259;
                    destination.Sprms.Add(sprm);
                }
            }
        }
        /// <summary>
        /// Converts CharacterFormat to CharacterProperties.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="charProps">The char props.</param>
        public static void FormatToCHP(WCharacterFormat source, CharacterProperties charProps)
        {
            lock (m_threadLocker)
            {
                source.SetCharPropFontNames(charProps.StyleSheet);

                //Update SPRM for style ID
                if (source.Sprms != null && source.Sprms.Contain(WordSprmOptions.sprmCIstd))
                {
                    SinglePropertyModifierRecord styleSprm = source.Sprms[WordSprmOptions.sprmCIstd];
                    if (source.CharStyleName != null)
                    {
                        short styleIndex = (short)charProps.StyleSheet.StyleNameToIndex(source.CharStyleName);
                        if (styleIndex > -1)
                        {
                            byte[] arrData = BitConverter.GetBytes(styleIndex);
                            styleSprm.ByteArray = arrData;
                        }
                    }
                }

                if (source.Sprms != null)
                {
                    RemovePicLocationSprm(source.Sprms);
                    RemoveIncorrectSprms(source.Sprms);
                }

                SinglePropertyModifierRecord sprm = null;
                for (int i = 0, cnt = source.CharacterProps.Sprms.Count; i < cnt; i++)
                {
                    sprm = source.CharacterProps.Sprms.GetSprmByIndex(i);
                    if (charProps.Sprms[sprm.TypedOptions] == null)
                    {
                        charProps.Sprms.Modifiers.Add(sprm);
                    }
                }
                //Sort Sprms
                charProps.Sprms.SortSprms();

                if (source.Document.GrammarSpellingData == null && charProps.LocationIdASCII == short.MaxValue)
                {
                    charProps.LocationIdASCII = 0x0409;
                    charProps.RgLid3 = 0x0409;
                    charProps.RgLid3_2 = 0x0409;
                }
            }
        }
        /// <summary>
        /// Converts property from characterFormat property to characterProperty.
        /// </summary>
        /// <param name="propertyKey">The property key.</param>
        /// <param name="value">The value.</param>
        /// <param name="charProps">The char props.</param>
        /// <param name="charFormat">The char format.</param>
        internal static void FormatToProp(int propertyKey, object value, CharacterProperties charProps, WCharacterFormat charFormat)
        {
            lock (m_threadLocker)
            {
                switch (propertyKey)
                {
                    #region Fonts
                    case WCharacterFormat.FontNameKey:
                        charProps.FontName = (string)value;
                        break;
                    case WCharacterFormat.FontNameBidiKey:
                        charProps.FontNameBi = (string)value;
                        break;
                    case WCharacterFormat.FontNameFarEastKey:
                        charProps.FontNameFarEast = (string)value;
                        break;
                    case WCharacterFormat.FontNameNonFarEastKey:
                        charProps.FontNameNonFarEast = (string)value;
                        break;
                    case WCharacterFormat.FontNameAsciiKey:
                        charProps.FontNameAscii = (string)value;
                        break;
                    #endregion

                    #region Boolean properties
                    case WCharacterFormat.BoldKey:
                        charProps.Bold = (bool)value;
                        break;
                    case WCharacterFormat.ItalicKey:
                        charProps.Italic = (bool)value;
                        break;
                    case WCharacterFormat.StrikeKey:
                        charProps.Strike = (bool)value;
                        break;
                    case WCharacterFormat.ShadowKey:
                        charProps.Shadow = (bool)value;
                        break;
                    case WCharacterFormat.EmbossKey:
                        charProps.Emboss = (bool)value;
                        break;
                    case WCharacterFormat.EngraveKey:
                        charProps.Engrave = (bool)value;
                        break;
                    case WCharacterFormat.HiddenKey:
                        charProps.Hidden = (bool)value;
                        break;
                    case WCharacterFormat.AllCapsKey:
                        charProps.AllCaps = (bool)value;
                        break;
                    case WCharacterFormat.SmallCapsKey:
                        charProps.SmallCaps = (bool)value;
                        break;
                    case WCharacterFormat.BidiKey:
                        charProps.Bidi = (bool)value;
                        break;
                    case WCharacterFormat.BoldBidiKey:
                        charProps.BoldBi = (bool)value;
                        break;
                    case WCharacterFormat.ItalicBidiKey:
                        charProps.ItalicBi = (bool)value;
                        break;
                    case WCharacterFormat.OutlineKey:
                        charProps.Outline = (bool)value;
                        break;
                    case WCharacterFormat.SpecialKey:
                        charProps.Special = (bool)value;
                        break;
                    case WCharacterFormat.DeleteRevisionKey:
                        charProps.IsDeleteRevision = (bool)value;
                        break;
                    case WCharacterFormat.InserteRevisionKey:
                        charProps.IsInsertRevision = (bool)value;
                        break;
                    case WCharacterFormat.ChangedFormatKey:
                        charProps.IsChangedFormat = (bool)value;
                        break;
                    case WCharacterFormat.FieldVanishKey:
                        charProps.FldVanish = (bool)value;
                        break;
                    case WCharacterFormat.PicLocationKey:
                        charProps.PicLocation = (int)value;
                        break;
                    case WCharacterFormat.FieldVanishCompKey:
                        charProps.FldVanishComplex = (byte)value;
                        break;
                    case WCharacterFormat.ComplexScriptKey:
                        charProps.ComplexScript = (bool)value;
                        break;
                    #endregion

                    #region Numeric, color, enum properties
                    case WCharacterFormat.TextColorKey:
                        Color textColor = (Color)value;
                        uint rgb = WordColor.ConvertColorToRGB(textColor);
                        charProps.FontColor = (byte)WordColor.ConvertRGBToId(rgb);
                        charProps.FontColorRGB = rgb;
                        break;
                    case WCharacterFormat.FontSizeKey:
                        charProps.FontSize = (float)value;
                        break;

                    case WCharacterFormat.UnderlineKey:
                        charProps.UnderlineCode = (byte)((UnderlineStyle)value);
                        break;
                    case WCharacterFormat.IdctHintKey:
                        charProps.IdctHint = (byte)((FontHintType)value);
                        break;
                    case WCharacterFormat.TextBkgColorKey:
                    case WCharacterFormat.ForeColorKey:
                    case WCharacterFormat.TextureStyleKey:
                        ShadingDescriptor shadingDescriptor = new ShadingDescriptor();
                        if (charFormat.HasKey(WCharacterFormat.TextBkgColorKey) || charFormat.HasKey(WCharacterFormat.TextBkgColorNewKey))
                            shadingDescriptor.BackColor = (Color)charFormat.TextBackgroundColor;
                        if (charFormat.HasKey(WCharacterFormat.ForeColorKey) || charFormat.HasKey(WCharacterFormat.ForeColorNewKey))
                            shadingDescriptor.ForeColor = charFormat.ForeColor;
                        if (charFormat.HasKey(WCharacterFormat.TextureStyleKey) || charFormat.HasKey(WCharacterFormat.TextureStyleNewKey))
                            shadingDescriptor.Pattern = charFormat.TextureStyle;
                        charFormat.CharacterProps.ShadingNew = shadingDescriptor;
                        break;
                    case WCharacterFormat.SubSuperScriptKey:
                        charProps.SubSuperScript = (byte)((SubSuperScript)value);
                        break;
                    case WCharacterFormat.DoubleStrikeKey:
                        charProps.DoubleStrike = (bool)value;
                        break;
                    case WCharacterFormat.PositionKey:
                        charProps.Position = (short)((float)value * DLSConstants.ShortSize);
                        break;
                    case WCharacterFormat.SpacingKey:
                        charProps.LineSpacing = (short)((float)value * DLSConstants.TwipsInOnePoint);
                        break;
                    case WCharacterFormat.FontSizeBidiKey:
                        charProps.FontSizeBi = (ushort)((float)value);
                        break;
                    case WCharacterFormat.HighlightColorKey:
                        charProps.HighlightColor = (byte)WordColor.ConvertColorToId((Color)value);
                        break;
                    case WCharacterFormat.BorderKey:
                        BorderCode brc = new BorderCode();
                        ParagraphPropertiesConverter.ImportBorder(brc, (Border)value);
                        charProps.Border = brc;
                        break;
                    case WCharacterFormat.LocaleIdASCIIKey:
                        charProps.LocationIdASCII = (short)value;
                        break;
                    case WCharacterFormat.LocaleIdFarEastKey:
                        charProps.LocationIdFarEast = (short)value;
                        break;
                    case WCharacterFormat.RgLid3Key:
                        charProps.RgLid3 = (short)value;
                        break;
                    case WCharacterFormat.RgLid3_2Key:
                        charProps.RgLid3_2 = (short)value;
                        break;
                    case WCharacterFormat.LidKey:
                        charProps.Lid = (short)value;
                        break;
                    case WCharacterFormat.LidBiKey:
                        charProps.LidBi = (short)value;
                        break;
                    case WCharacterFormat.NoProofKey:
                        charProps.NoProof = (bool)value;
                        break;
                    case WCharacterFormat.ListPicIndexKey:
                        charProps.ListPictureIndex = (int)value;
                        break;
                    case WCharacterFormat.ListHasPicKey:
                        charProps.ListHasImage = (bool)value;
                        break;
                    #endregion
                }
            }
        }
        #region Commented code
        //    /// <summary>
        //    /// Converts character property to format property.
        //    /// </summary>
        //    /// <param name="popertyKey">The poperty key.</param>
        //    /// <param name="charProps">The character props.</param>
        //    /// <param name="charFormat">The character format.</param>
        //    internal static void PropToFormat( int popertyKey, CharacterProperties charProps, WCharacterFormat charFormat )
        //    {
        //      SinglePropertyModifierRecord sprm = null;
        //
        //      switch( popertyKey )
        //      {
        //        #region Boolean properties
        //        case WCharacterFormat.TextColorKey:
        //          charFormat.TextColor = charProps.FontColorExt;
        //          break;
        //        case WCharacterFormat.FontSizeKey:
        //          charFormat.FontSize = charProps.FontSize;
        //          break;
        //        case WCharacterFormat.BoldKey:
        //          charFormat.Bold =  charProps.Bold;
        //          break;
        //        case WCharacterFormat.ItalicKey:
        //          charFormat.Italic = charProps.Italic;
        //          break;
        //        case WCharacterFormat.StrikeKey:
        //          charFormat.Strikeout = charProps.Strike;
        //          break;
        //        case WCharacterFormat.UnderlineKey:
        //          charFormat.UnderlineStyle = ( UnderlineStyle )charProps.UnderlineCode;
        //          break;
        //        case WCharacterFormat.ShadowKey:
        //          charFormat.Shadow = charProps.Shadow;
        //          break;
        //        case WCharacterFormat.EmbossKey:
        //          charFormat.Emboss = charProps.Emboss;
        //          break;
        //        case WCharacterFormat.EngraveKey:
        //          charFormat.Engrave = charProps.Engrave;
        //          break;
        //        case WCharacterFormat.HiddenKey:
        //          charFormat.Hidden = charProps.Hidden;
        //          break;
        //        case WCharacterFormat.AllCapsKey:
        //          charFormat.AllCaps = charProps.AllCaps;
        //          break;
        //        case WCharacterFormat.SmallCapsKey:
        //          charFormat.SmallCaps = charProps.SmallCaps;
        //          break;
        //        case WCharacterFormat.BidiKey:
        //          charFormat.Bidi = charProps.Bidi;
        //          break;
        //        case WCharacterFormat.BoldBidiKey:
        //          charFormat.BoldBidi = charProps.BoldBi;
        //          break;
        //        case WCharacterFormat.ItalicBidiKey:
        //          charFormat.ItalicBidi = charProps.ItalicBi;
        //          break;
        //        case WCharacterFormat.OutlineKey:
        //          charFormat.OutLine = charProps.Outline;
        //          break;
        //        case WCharacterFormat.IdctHintKey:
        //          charFormat.IdctHint = charProps.IdctHint;
        //          break;
        //        #endregion
        //
        //        #region Numeric, color, enum properties
        //        case WCharacterFormat.TextBkgColorKey:
        //        case WCharacterFormat.ForeColorKey:
        //        case WCharacterFormat.TextureStyleKey:
        //          sprm = charProps.Sprms[ WordSprmOptions.sprmCShd ];
        //          if( sprm != null )
        //          {
        //            ShadingDescriptor shading = charProps.GetShading( sprm );
        //            charFormat.TextBackgroundColor = shading.BackColor;
        //            charFormat.ForeColor = shading.ForeColor;
        //            charFormat.TextureStyle = shading.Pattern;
        //          }
        //          break;
        //        case WCharacterFormat.SubSuperScriptKey:
        //          charFormat.SubSuperScript = ( SubSuperScript )charProps.SubSuperScript;          
        //          break;
        //        case WCharacterFormat.DoubleStrikeKey:
        //          charFormat.DoubleStrike = charProps.DoubleStrike;
        //          break;
        //        case WCharacterFormat.PositionKey:
        //          charFormat.Position = ( float )charProps.Position / DLSConstants.TwipsInOnePoint;
        //          break;
        //        case WCharacterFormat.SpacingKey:
        //          charFormat.CharacterSpacing = charProps.LineSpacing / DLSConstants.TwipsInOnePoint;
        //          break;
        //        case WCharacterFormat.FontSizeBidiKey:
        //          charFormat.FontSizeBidi = charProps.FontSizeBi;
        //          break;
        //        case WCharacterFormat.HighlightColorKey:
        //          charFormat.HighlightColor = WordColor.ConvertIdToColor( charProps.HighlightColor );
        //          break;
        //        case WCharacterFormat.BorderKey:
        //          sprm = charProps.Sprms[ WordSprmOptions.sprmCBrc ];
        //          if( sprm != null )
        //          {
        //            BorderCode brc = charProps.GetBorder( sprm );
        //            ParagraphPropertiesConverter.ExportBorder( brc, charFormat.Border );
        //          }
        //          break;
        //       
        //        case WCharacterFormat.RgLid0Key:
        //          charFormat.RgLid0 = charProps.RgLid0;
        //          break;
        //        case WCharacterFormat.RgLid1Key:
        //          charFormat.RgLid1 = charProps.RgLid1;
        //          break;
        //        case WCharacterFormat.RgLid3Key:
        //          charFormat.RgLid3 = charProps.RgLid3;
        //          break;
        //        case WCharacterFormat.RgLid3_2Key:
        //          charFormat.RgLid3_2 = charProps.RgLid3_2;
        //          break;
        //        case WCharacterFormat.LidKey:
        //          charFormat.Lid = charProps.Lid;
        //          break;
        //        case WCharacterFormat.LidBiKey:
        //          charFormat.LidBi = charProps.LidBi;
        //          break;  
        //        #endregion
        //      }
        //    }

        //    /// <summary>
        //    /// CHPs to format.
        //    /// </summary>
        //    /// <param name="reader">The reader.</param>
        //    /// <param name="format">The format.</param>
        //    /// <param name="parseAll">if set to <c>true</c> [parse all].</param>
        //    public static void CHPToFormat( IWordReaderBase reader, WCharacterFormat format, bool parseAll )
        //    {
        //      CHPToFormat( reader.CharacterProperties, format, parseAll, reader );
        //    }
        //    /// <summary>
        //    /// Convert CharacterProperties to WCharacterFormat.
        //    /// </summary>
        //    public static void CHPToFormat( CharacterProperties chp, WCharacterFormat format, bool parseAll, IWordReaderBase reader )
        //    {
        //      ArrayList modifiers = chp.Sprms.Modifiers;
        //
        //      for( int i = 0, count = modifiers.Count; i < count; i++ )
        //      {
        //        SinglePropertyModifierRecord sprm = ( SinglePropertyModifierRecord )modifiers[ i ];
        //        WordSprmOptions options = sprm.TypedOptions;
        //
        //        switch( options )
        //        {
        //          // Gets character style
        //          case WordSprmOptions.sprmCIstd:
        //            format.CharStyleName = reader.StyleSheet.GetStyleByIndex( sprm.UshortValue ).Name;
        //            break;
        //
        //          #region Boolean values
        //          // ==== Boolean values ( used GetComplexBoolean internally )
        //          case WordSprmOptions.sprmCFBold:
        //            format.Bold = chp.GetBoolean( sprm );
        //            format.BoldComplex = sprm.ByteValue;
        //            break;
        //          case WordSprmOptions.sprmCFItalic:
        //            format.Italic = chp.GetBoolean( sprm );
        //            format.ItalicComplex = sprm.ByteValue;
        //            break;
        //          case WordSprmOptions.sprmCFStrike:
        //            format.Strikeout = chp.GetBoolean( sprm );
        //            format.StrikeComplex = sprm.ByteValue;
        //            break;
        //          case WordSprmOptions.sprmCFVanish:
        //            format.Hidden = chp.GetBoolean( sprm );
        //            format.HiddenComplex = sprm.ByteValue;
        //            break;
        //
        //          // ==== Boolean values ( raw value )
        //          case WordSprmOptions.sprmCFShadow:
        //            format.Shadow = sprm.BoolValue;
        //            format.ShadowComplex = sprm.ByteValue;
        //            break;
        //          case WordSprmOptions.sprmCFEmboss:
        //            format.Emboss = sprm.BoolValue;
        //            format.EmbossComplex = sprm.ByteValue;
        //            break;
        //          case WordSprmOptions.sprmCFImprint:
        //            format.Engrave = sprm.BoolValue;
        //            format.EngraveComplex = sprm.ByteValue;
        //            break;
        //          case WordSprmOptions.sprmCFDStrike:
        //            format.DoubleStrike = sprm.BoolValue;
        //            format.DoubleStrikeComplex = sprm.ByteValue;
        //            break;
        //          case WordSprmOptions.sprmCFCaps:
        //            format.AllCaps = sprm.BoolValue;
        //            format.AllCapsComplex = sprm.ByteValue;
        //            break;
        //          case WordSprmOptions.sprmCFSmallCaps:
        //            format.SmallCaps = sprm.BoolValue;
        //            format.SmallCapsComplex = sprm.ByteValue;
        //            break;
        //          case WordSprmOptions.sprmCFOutline:
        //            format.OutLine = sprm.BoolValue;
        //            break;
        //          case WordSprmOptions.sprmCFNoProof:
        //            format.NoProof = sprm.ByteValue;
        //            break;
        //          
        //          // ==== Boolean values ( bidi )
        //          case WordSprmOptions.sprmCFBiDi:
        //            format.Bidi = sprm.BoolValue;
        //            break;
        //          case WordSprmOptions.sprmCFBoldBi:
        //            format.BoldBidi = chp.GetBoolean( sprm );
        //            break;
        //          case WordSprmOptions.sprmCFItalicBi:
        //            format.ItalicBidi = chp.GetBoolean( sprm );
        //            break;
        //          #endregion Boolean values
        //
        //          #region Enum, Float, Colors values
        //          // ==== Enum values 
        //          case WordSprmOptions.sprmCKul:
        //            format.UnderlineStyle = ( UnderlineStyle )sprm.ByteValue;
        //            break;
        //          case WordSprmOptions.sprmCIss:
        //            format.SubSuperScript = ( SubSuperScript )sprm.ByteValue;
        //            break;
        //
        //          // ==== Float values 
        //          case WordSprmOptions.sprmCHps:
        //            format.FontSize = chp.FontSize;
        //            break;
        //          case WordSprmOptions.sprmCHpsBi:
        //            format.FontSizeBidi = chp.FontSizeBi;
        //            break;
        //          case WordSprmOptions.sprmCHpsPos:
        //            format.Position = ( float )sprm.ShortValue / DLSConstants.TwipsInOnePoint;
        //            break;
        //          case WordSprmOptions.sprmCDxaSpace:
        //            format.CharacterSpacing = ( float )sprm.ShortValue / DLSConstants.TwipsInOnePoint;
        //            break;
        //
        //          // ==== Colors values 
        //          case WordSprmOptions.sprmCIcoe:
        //            format.TextColor = chp.FontColorExt;
        //            break;
        //          case WordSprmOptions.sprmCIco:
        //            if( chp.Sprms[ WordSprmOptions.sprmCIcoe ] == null )
        //            {
        //              format.TextColor = WordColor.ConvertIdToColor( chp.FontColor );
        //            }
        //            break;
        //          case WordSprmOptions.sprmCHighlight:
        //            format.HighlightColor = WordColor.ConvertIdToColor( sprm.ByteValue );
        //            break;
        //          case WordSprmOptions.sprmCShd:
        //            ShadingDescriptor shading = chp.GetShading( sprm );
        //            format.TextBackgroundColor = shading.BackColor;
        //            format.ForeColor = shading.ForeColor;
        //            format.TextureStyle = shading.Pattern;
        //            break;
        //          case WordSprmOptions.sprmCShdNew:
        //            ShadingDescriptor shadingN = chp.GetShading( sprm );
        //            format.TextBackgroundColor = shadingN.BackColor;
        //            format.ForeColor = shadingN.ForeColor;
        //            format.TextureStyle = shadingN.Pattern;
        //            break;
        //          #endregion
        //
        //          #region Fonts, Borders, Languages data
        //          // ==== Fonts data 
        //          case WordSprmOptions.sprmCRgFtc0:
        //            string fontName = chp.GetFontName( sprm );
        //            format.FontName = fontName;
        //            format.FontNameAscii = fontName;
        //            break;
        //          case WordSprmOptions.sprmCRgFtc1:
        //            format.FontNameFarEast = chp.GetFontName( sprm );
        //            break;
        //          case WordSprmOptions.sprmCRgFtc2:
        //            format.FontNameNonFarEast = chp.GetFontName( sprm );
        //            break;
        //          case WordSprmOptions.sprmCFtcBi:
        //            format.FontNameBidi = chp.GetFontName( sprm );
        //            break;
        //
        //          // ==== Borders data 
        //          case WordSprmOptions.sprmCBrc:
        //            BorderCode brc = chp.GetBorder( sprm );
        //            ParagraphPropertiesConverter.ExportBorder( brc, format.Border );
        //            break;
        //
        //          // ==== Languages data
        //          case WordSprmOptions.sprmCRgLid0:
        //            format.RgLid0 = sprm.ShortValue;
        //            break;
        //          case WordSprmOptions.sprmCRgLid1:
        //            format.RgLid1 = sprm.ShortValue;
        //            break;
        //          case WordSprmOptions.sprmCRgLid3:
        //            format.RgLid3 = sprm.ShortValue;
        //            break;
        //          case WordSprmOptions.sprmCRgLid3_2:
        //            format.RgLid3_2 = sprm.ShortValue;
        //            break;
        //          case WordSprmOptions.sprmCLid:
        //            format.Lid = sprm.ShortValue;
        //            break;
        //          case WordSprmOptions.sprmCLidBi:
        //            format.LidBi = sprm.ShortValue;
        //            break;
        //          case WordSprmOptions.sprmCIdctHint:
        //            format.IdctHint = chp.IdctHint; 
        //            break;
        //        }
        //        #endregion
        //      }
        //      
        //      // Export all sprms
        //      if( parseAll )
        //      {
        //        SinglePropertyModifierArray sprms = chp.GetCopiableSprm();
        //        
        //        if( sprms.Count > 0 )
        //        {
        //          int length = sprms.Length;
        //          format.InternalData = new byte[ length ];
        //          sprms.Save( format.InternalData, 0, null );
        //        }
        //      }
        //    }
        //    /// <summary>
        //    /// Convert WCharacterFormat to CharacterProperties.
        //    /// </summary>
        //    public static void FormatToCHP( WCharacterFormat format, CharacterProperties chp, bool parseAll )
        //    {
        //      // Import all sprms
        //      if( parseAll && format.InternalData != null && format.InternalData.Length < 300 && format.InternalData.Length > 0 )
        //      {
        //        SinglePropertyModifierArray modifierArray = new SinglePropertyModifierArray( format.InternalData, 0, null );
        //        foreach( SinglePropertyModifierRecord sprm in modifierArray )
        //        {
        //          if( !chp.HasOptions( sprm.TypedOptions ) )
        //          {
        //            chp.Sprms.Modifiers.Add( sprm );
        //          }
        //        }
        //      }
        //
        //      if( format.Document.GrammarSpellingData == null )
        //      {
        //        chp.RgLid0 = 0x0409;
        //        chp.RgLid3 = 0x0409;
        //        chp.RgLid3_2 = 0x0409;
        //      }
        //
        //      IDictionaryEnumerator dEnum = format.PropertiesHash.GetEnumerator();
        //      while( dEnum.MoveNext() )
        //      {
        //        int key = ( int )dEnum.Key;
        //        object value = dEnum.Value;
        //
        //        if( key == WCharacterFormat.BoldKey )
        //        {
        //          if( !chp.HasOptions( WordSprmOptions.sprmCFBold ) )
        //          {
        ////            destination.AddSprm( WordSprmOptions.sprmCFBold, ( bool )value );
        //            chp.Bold = ( bool )value;
        //          }
        //        }
        //        //        if( key == WCharacterFormat.BoldComplexKey )
        //        //        {
        //        //          destination.BoldComplex = source.BoldComplex;
        //        //        }
        //        if( key == WCharacterFormat.ItalicKey )
        //        {
        //          if( !chp.HasOptions( WordSprmOptions.sprmCFItalic ) )
        //          {
        ////            destination.AddSprm( WordSprmOptions.sprmCFItalic, ( bool )value );
        //            chp.Italic = ( bool )value;
        //          }
        //        }
        //
        //        if( key == WCharacterFormat.FontNameKey )
        //        {
        ////          destination.SetAllFontNames( ( string )value );
        //          chp.FontName = format.FontName;
        //          //destination.FontNonFarEast = destination.FontFarEast = destination.FontAscii = ( ushort )source.FontAsciiId;
        //        }
        //        if( key == WCharacterFormat.FontSizeKey )
        //        {
        //          chp.AddSprm( WordSprmOptions.sprmCHps, ( ushort )( ( float )value * 2 ) );
        ////          destination.FontSize = source.FontSize;
        //        }
        //        if( key == WCharacterFormat.ShadowKey )
        //        {
        ////          destination.AddSprm( WordSprmOptions.sprmCFShadow, ( bool )value );
        //          chp.Shadow = ( bool )value;
        //        }
        //        if( key == WCharacterFormat.TextColorKey )
        //        {
        //          Color textColor = ( Color )value;
        //          uint rgb = WordColor.ConvertColorToRGB( textColor );
        //          chp.FontColor = ( byte )WordColor.ConvertRGBToId( rgb );
        //          chp.FontColorRGB = rgb;
        ////          destination.FontColor = (byte)WordColor.ConvertColorToId(textColor);
        ////          destination.FontColorExt = textColor;
        //        }
        //        if( key == WCharacterFormat.TextBkgColorKey )
        //        {
        //          //destination.HighlightColor = (byte)WordColor.ColorToId(source.TextBackgroundColor);
        //          ShadingDescriptor shadingDescriptor = new ShadingDescriptor();
        //          shadingDescriptor.BackColor = ( Color )value;
        //          shadingDescriptor.ForeColor = format.ForeColor;
        //          shadingDescriptor.Pattern = format.TextureStyle;
        //          chp.ShadingNew = shadingDescriptor;
        //        }
        //        if( key == WCharacterFormat.SubSuperScriptKey )
        //        {
        ////          destination.AddSprm( WordSprmOptions.sprmCIss, ( byte )( SubSuperScript )value );
        //          chp.SubSuperScript = (byte)( SubSuperScript )value;
        //        }
        //        if( key == WCharacterFormat.EmbossKey )
        //        {
        ////          destination.AddSprm( WordSprmOptions.sprmCFEmboss, ( bool )value );
        //          chp.Emboss = ( bool )value;
        //        }
        //        if( key == WCharacterFormat.EngraveKey )
        //        {
        ////          destination.AddSprm( WordSprmOptions.sprmCFImprint, ( bool )value );
        //          chp.Engrave = ( bool )value;
        //        }
        //        if( key == WCharacterFormat.DoubleStrikeKey )
        //        {
        ////          destination.DoubleStrike = ( bool )value;
        //          if( !chp.HasOptions( WordSprmOptions.sprmCFDStrike ))
        //          {
        //            //            destination.AddSprm( WordSprmOptions.sprmCFItalic, ( bool )value );
        //            chp.DoubleStrike = ( bool )value;
        //          }
        ////          destination.AddSprm( WordSprmOptions.sprmCFDStrike, ( bool )value );
        //        }
        //        if( key == WCharacterFormat.AllCapsKey )
        //        {
        ////         destination.AddSprm( WordSprmOptions.sprmCFCaps, ( bool )value );
        //          chp.AllCaps = ( bool )value;
        //        }
        //        if( key == WCharacterFormat.SmallCapsKey )
        //        {
        ////          destination.AddSprm( WordSprmOptions.sprmCFSmallCaps, ( bool )value );
        //          chp.SmallCaps = ( bool )value;
        //        }
        //        if( key == WCharacterFormat.PositionKey )
        //        {
        //          //          destination.AddSprm( WordSprmOptions.sprmCHpsPos, ( short )( ( float )value * DLSConstants.TwipsInOnePoint ) );
        //          chp.Position = ( short )Math.Round( ( float )value * DLSConstants.TwipsInOnePoint );
        //        }
        //        if( key == WCharacterFormat.SpacingKey )
        //        {
        //          //          destination.AddSprm( WordSprmOptions.sprmCDxaSpace, ( short )( ( float )value * DLSConstants.TwipsInOnePoint ) );
        //          chp.LineSpacing = ( short )Math.Round( ( float )value * DLSConstants.TwipsInOnePoint );
        //        }
        //        if( key == WCharacterFormat.StrikeKey )
        //        {
        //          //          destination.AddSprm( WordSprmOptions.sprmCFStrike, ( bool )value );
        //          chp.Strike = ( bool )value;
        //        }
        //        if( key == WCharacterFormat.DoubleStrikeKey )
        //        {
        //          //          destination.AddSprm( WordSprmOptions.sprmCFDStrike, ( bool )value );
        //          chp.DoubleStrike = ( bool )value;
        //        }
        //        if( key == WCharacterFormat.UnderlineKey )
        //        {
        //          //          destination.AddSprm( WordSprmOptions.sprmCKul, ( byte )( UnderlineStyle )value );
        //          chp.UnderlineCode = ( byte )( UnderlineStyle )value;
        //        }
        //        if( key == WCharacterFormat.BidiKey )
        //        {
        ////          destination.AddSprm( WordSprmOptions.sprmCFBiDi, ( bool )value );
        //          chp.Bidi = ( bool )value;
        //        }
        //        if( key == WCharacterFormat.BoldBidiKey )
        //        {
        ////          destination.AddSprm( WordSprmOptions.sprmCFBoldBi, ( bool )value );
        //          chp.BoldBi = ( bool )value;
        //        }
        //        if( key == WCharacterFormat.ItalicBidiKey )
        //        {
        ////          destination.AddSprm( WordSprmOptions.sprmCFItalicBi, ( bool )value );
        //          chp.ItalicBi = ( bool )value;
        //        }
        //        if( key == WCharacterFormat.FontSizeBidiKey )
        //        {
        //          //          destination.AddSprm( WordSprmOptions.sprmCHpsBi, (ushort)( float )value );
        //          chp.FontSizeBi = ( ushort )( float )value;
        //        }
        //        if( key == WCharacterFormat.FontNameBidiKey )
        //        {
        //          //          destination.SetFontName( ( string )value, WordSprmOptions.sprmCFtcBi);
        //          chp.FontNameBi = ( string )value;
        //        }
        //        if( key == WCharacterFormat.HighlightColorKey )
        //        {
        //          //          destination.AddSprm( WordSprmOptions.sprmCHighlight, ( byte )WordColor.ConvertColorToId( ( Color )value ) );
        //          chp.HighlightColor = ( byte )WordColor.ConvertColorToId( ( Color )value );
        //        }
        //        if( key == WCharacterFormat.HiddenKey )
        //        {
        //          if( !chp.HasOptions( WordSprmOptions.sprmCFVanish ) )
        //          {
        //            //            destination.AddSprm( WordSprmOptions.sprmCFVanish, ( bool )value );
        //            chp.Hidden = format.Hidden;
        //          }
        //        }
        //        if( key == WCharacterFormat.BorderKey )
        //        {
        //          if( !format.Border.IsDefault )
        //          {
        //            BorderCode brc = new BorderCode();
        //            ParagraphPropertiesConverter.ImportBorder( brc, ( Border )value );
        //            chp.Border = brc;
        //          }
        //        }
        //        if( key == WCharacterFormat.OutlineKey )
        //        {
        //          chp.Outline = format.OutLine;
        //        }
        //      }
        //      if( format.IdctHint )
        //      {
        //        chp.IdctHint = format.IdctHint;
        //      }
        //      //      if( source.FontNameAscii != "" )
        //      //      {
        //      //        destination.SetFontName( source.FontNameAscii, WordSprmOptions.sprmCRgFtc0 );
        //      ////        destination.FontNameAscii = source.FontNameAscii;
        //      //      }
        //      if( format.FontNameFarEast != "" )
        //      {
        //        //destination.SetFontName( source.FontNameFarEast, WordSprmOptions.sprmCRgFtc1 );
        //        chp.FontNameFarEast = format.FontNameFarEast;
        //      }
        //      if( format.FontNameNonFarEast != "" )
        //      {
        //        //destination.SetFontName( source.FontNameNonFarEast, WordSprmOptions.sprmCRgFtc2 );
        //        chp.FontNameNonFarEast = format.FontNameNonFarEast;
        //      }
        //      chp.RgLid0 = format.RgLid0;
        //      chp.RgLid1 = format.RgLid1;
        //      chp.RgLid3 = format.RgLid3;
        //      chp.RgLid3_2 = format.RgLid3_2;
        //      chp.Lid = format.Lid;
        //      chp.LidBi = format.LidBi;
        //      chp.BoldComplex = format.BoldComplex;
        //      chp.ItalicComplex = format.ItalicComplex;
        //      chp.HiddenComplex = format.HiddenComplex;
        //      chp.DoubleStrikeComplex = format.DoubleStrikeComplex;
        //      chp.SmallCapsComplex = format.SmallCapsComplex;
        //      chp.StrikeComplex = format.StrikeComplex;
        //      chp.AllCapsComplex = format.AllCapsComplex;
        //      chp.ShadowComplex = format.ShadowComplex;
        //      chp.EmbossComplex = format.EmbossComplex;
        //      chp.EngraveComplex = format.EngraveComplex;
        //    }
        //    
        #endregion

        #endregion

        #region Class helper methods
        /// <summary>
        /// Removes the pic location SPRM.
        /// </summary>
        /// <param name="sprms">The SPRMS.</param>
        private static void RemovePicLocationSprm(SinglePropertyModifierArray sprms)
        {
            lock (m_threadLocker)
            {
                SinglePropertyModifierRecord sprm = sprms[WordSprmOptions.sprmCPicLocation];
                if (sprm != null)
                {
                    sprms.Modifiers.Remove(sprm);
                }
            }
        }
        /// <summary>
        /// Removes the incorrect SPRMs
        /// </summary>
        /// <param name="sprms">The SPRMs</param>
        private static void RemoveIncorrectSprms(SinglePropertyModifierArray sprms)
        {
            lock (m_threadLocker)
            {
                SinglePropertyModifierArray newArray = sprms.Clone();
                sprms.Clear();

                for (int i = 0, count = newArray.Modifiers.Count; i < count; i++)
                {
                    SinglePropertyModifierRecord sprm = newArray.Modifiers[i];
                    if (!IncorrectOptions.Contains((int)sprm.Options))
                        sprms.Add(sprm);
                }

                newArray.Clear();
                newArray = null;
            }
        }
        #endregion
    }
    /// <summary>
    /// Summary description for class ParagraphPropertiesConverter
    /// </summary>
    //[ CLSCompliant( false ) ]
    [DocumentationExclude()]
    internal class ParagraphPropertiesConverter
    {
        #region Class utility methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="srcBorder"></param>
        /// <param name="destBorder"></param>
        internal static void ExportBorder(BorderCode srcBorder, Border destBorder)
        {
            //      if( !srcBorder.IsDefault )
            {
                //        destBorder.Color = WordColor.ConvertIdToColor( srcBorder.LineColor );
                destBorder.BorderType = (BorderStyle)srcBorder.BorderType;
                destBorder.Color = srcBorder.LineColorExt;
                destBorder.LineWidth = (float)srcBorder.LineWidth / DLSConstants.BorderLineFactor;
                destBorder.Space = (float)srcBorder.Space;
                destBorder.Shadow = srcBorder.Shadow;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="destBorder"></param>
        /// <param name="srcBorder"></param>
        internal static void ImportBorder(BorderCode destBorder, Border srcBorder)
        {
            if (!srcBorder.IsDefault)
            {
                if (srcBorder.BorderType == BorderStyle.Cleared)
                {
                    //Specifically handled to preserve cleared type border in doc format document.
                    destBorder.LineColor = 0;
                    destBorder.LineColorExt = Color.FromArgb(0, 255, 255, 255);
                    destBorder.BorderType = 255;
                    destBorder.LineWidth = 255;
                }
                else
                {
                    destBorder.LineColor = (byte)WordColor.ConvertColorToId(srcBorder.Color);
                    destBorder.LineColorExt = srcBorder.Color;
                    destBorder.BorderType = (byte)srcBorder.BorderType;
                    destBorder.LineWidth = (byte)Math.Round(srcBorder.LineWidth * DLSConstants.BorderLineFactor);
                }
                destBorder.Space = (byte)Math.Round(srcBorder.Space);
                destBorder.Shadow = srcBorder.Shadow;
                //        destBorder.Update();
            }
            else
            {
                destBorder.LineColor = 0;
                destBorder.LineColorExt = Color.Empty;
                destBorder.BorderType = 0;
                destBorder.LineWidth = 0;
                destBorder.Space = 0;
                destBorder.Shadow = false;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="tabs"></param>
        internal static void ExportTabs(TabsInfo info, TabCollection tabs)
        {
            for (int i = 0; i < info.TabCount; i++)
            {
                TabDescriptor descriptor = info.Descriptors[i];

                if (descriptor == null)
                    descriptor = new TabDescriptor(0);

                Tab tab = tabs.AddTab();
                tab.Position = (float)info.TabPositions[i] / DLSConstants.TwipsInOnePoint;
                tab.Justification = descriptor.Justification;
                tab.TabLeader = descriptor.TabLeader;
            }

            if (info.TabDeletePositions != null)
            {
                for (int i = 0; i < info.TabDeletePositions.Length; i++)
                {
                    Tab tab = tabs.AddTab();
                    tab.DeletePosition = info.TabDeletePositions[i];
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="destBorders"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        private static Borders GetDestBorders(Borders destBorders, WParagraphFormat destination)
        {
            if (destBorders == null)
            {
                destBorders = destination.Borders;
            }
            return destBorders;
        }
        /// <summary>
        /// Exports the specified source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        public static void Export(ParagraphProperties source, WParagraphFormat destination)
        {
            destination.Sprms = source.Sprms;
        }
        /// <summary>
        /// Imports the specified destination.
        /// </summary>
        /// <param name="destination">The destination.</param>
        /// <param name="source">The source.</param>
        public static void Import(ParagraphProperties destination, WParagraphFormat source, WParagraph para)
        {
            //Absolute tab implementation
            if (source.AbsoluteTab != null && source.AbsoluteTab.Alignment != AbsoluteTabAlignment.Left)
                source.Tabs.AddTab(source.AbsoluteTab.Position, (TabJustification)source.AbsoluteTab.Alignment, source.AbsoluteTab.TabLeader);

            if (source.Sprms == null)
            {
                source.Sprms = new SinglePropertyModifierArray();
            }

            SinglePropertyModifierRecord sprm = null;
            source.Sprms.RemoveValue(WordSprmOptions.sprmPTimeStamp);

            SinglePropertyModifierArray existingModifiers = new SinglePropertyModifierArray();
            for (int i = 0, cnt = destination.Sprms.Count; i < cnt; i++)
            {
                sprm = destination.Sprms.GetSprmByIndex(i);
                existingModifiers.Modifiers.Add(sprm);
            }
            destination.Sprms.Clear();
            for (int i = 0, cnt = source.Sprms.Count; i < cnt; i++)
            {
                sprm = source.Sprms.GetSprmByIndex(i);
                destination.Sprms.Modifiers.Add(sprm);
            }
            for (int i = 0, cnt = existingModifiers.Count; i < cnt; i++)
            {
                sprm = existingModifiers.GetSprmByIndex(i);
                SinglePropertyModifierRecord sprmtemp = destination.Sprms.TryGetSprm(sprm.TypedOptions);
                if (sprmtemp != null)
                {
                  int index=  destination.Sprms.Modifiers.IndexOf(sprmtemp);
                  destination.Sprms.Modifiers.Remove(sprmtemp);
                  destination.Sprms.Modifiers.Insert(index, sprm);
                }
                else
                    destination.Sprms.Modifiers.Insert(0,sprm);
            }
            
            if (destination.IsList && para != null)
            {
                if (!source.Bidi && !destination.Sprms.HasSprm(WordSprmOptions.sprmPJcBi))
                {
                    if (source.ParaProps.Sprms.HasSprm(WordSprmOptions.sprmPJc))
                    {
                        destination.JustificationBidi = source.ParaProps.Justification;
                    }
                    else if (source.ParaProps.Sprms.HasSprm(WordSprmOptions.sprmPJcBi))
                    {
                        destination.JustificationBidi = source.ParaProps.JustificationBidi;
                    }
                    else if (para.ParagraphFormat.Sprms.HasSprm(WordSprmOptions.sprmPJc))
                    {
                        destination.JustificationBidi = para.ParagraphFormat.ParaProps.Justification;
                    }
                    else if (para.ParagraphFormat.Sprms.HasSprm(WordSprmOptions.sprmPJcBi))
                    {
                        destination.JustificationBidi = para.ParagraphFormat.ParaProps.JustificationBidi;
                    }
                    else if (para.ParaStyle != null && para.ParaStyle.ParagraphFormat.Sprms.HasSprm(WordSprmOptions.sprmPJc))
                    {
                        destination.JustificationBidi = para.ParaStyle.ParagraphFormat.ParaProps.Justification;
                    }
                    else if (para.ParaStyle != null && para.ParaStyle.ParagraphFormat.Sprms.HasSprm(WordSprmOptions.sprmPJcBi))
                    {
                        destination.JustificationBidi = para.ParaStyle.ParagraphFormat.ParaProps.JustificationBidi;
                    }
                    else
                    {
                        destination.JustificationBidi = source.ParaProps.Justification;
                    }
                }
            }
            else if (!source.Bidi && !destination.Sprms.HasSprm(WordSprmOptions.sprmPJcBi))
                destination.JustificationBidi = source.ParaProps.Justification;

            if (!source.Bidi && destination.IsList && para != null && destination.Sprms.HasSprm(WordSprmOptions.sprmPJc) && destination.Sprms.HasSprm(WordSprmOptions.sprmPJcBi))
            {
                destination.Sprms.RemoveValue(WordSprmOptions.sprmPJcBi);
            }

            if (destination.Sprms.HasSprm(WordSprmOptions.sprmPFBiDi) && !destination.Sprms.Contain(WordSprmOptions.sprmPJc))
            {
                if (destination.Sprms[WordSprmOptions.sprmPFBiDi].BoolValue)
                {
                    switch (source.HorizontalAlignment)
                    {
                        case HorizontalAlignment.Left:
                            destination.Justification = ParagraphJustify.Right;
                            break;
                        case HorizontalAlignment.Right:
                            destination.Justification = ParagraphJustify.Left;
                            break;
                    }
                }
            }
            if (destination.Sprms.HasSprm(WordSprmOptions.sprmPJcBi) && !destination.Sprms.HasSprm(WordSprmOptions.sprmPJc))
                destination.Justification = destination.JustificationBidi;
        }
        /// <summary>
        ///Imports GridBefore and GridAfter properties of the table row
        /// </summary>
        /// <param name="destination">The destination.</param>
        /// <param name="source">The source.</param>
        public static void ImportGridBeforeAfter(ParagraphProperties destination, WTableRow source)
        {
            if ((byte)source.RowFormat.GridBeforeWidth.WidthType > (byte)FtsWidth.Auto
                && source.RowFormat.GridBeforeWidth.Width > 0)
            {
                //Writes grid before width.
                byte[] value = new byte[3];
                value[0] = (byte)source.RowFormat.GridBeforeWidth.WidthType;
                short gridValue = 0;
                if (source.RowFormat.GridBeforeWidth.WidthType == FtsWidth.Point)
                    gridValue = (short)Math.Round(source.RowFormat.GridBeforeWidth.Width * DLSConstants.TwipsInOnePoint);
                else if (source.RowFormat.GridBeforeWidth.WidthType == FtsWidth.Percentage)
                    gridValue = (short)Math.Round(source.RowFormat.GridBeforeWidth.Width * DLSConstants.PercentageFactor);
                byte[] width = BitConverter.GetBytes(gridValue);
                value[1] = width[0];
                value[2] = width[1];
                destination.Sprms.SetValue(WordSprmOptions.sprmTWidthBefore, value);
            }
            if ((byte)source.RowFormat.GridAfterWidth.WidthType > (byte)FtsWidth.Auto
                && source.RowFormat.GridAfterWidth.Width > 0)
            {
                //Writes grid after width.
                byte[] value = new byte[3];
                value[0] = (byte)source.RowFormat.GridAfterWidth.WidthType;
                short gridValue = 0;
                if (source.RowFormat.GridAfterWidth.WidthType == FtsWidth.Point)
                    gridValue = (short)Math.Round(source.RowFormat.GridAfterWidth.Width * DLSConstants.TwipsInOnePoint);
                else if (source.RowFormat.GridAfterWidth.WidthType == FtsWidth.Percentage)
                    gridValue = (short)Math.Round(source.RowFormat.GridAfterWidth.Width * DLSConstants.PercentageFactor);
                byte[] width = BitConverter.GetBytes(gridValue);
                value[1] = width[0];
                value[2] = width[1];
                destination.Sprms.SetValue(WordSprmOptions.sprmTWidthAfter, value);
            }
        }
        /// <summary>
        /// Imports the table row properties to paragraph.
        /// </summary>
        /// <param name="destination">The destination.</param>
        /// <param name="source">The source.</param>
        public static void Import(ParagraphProperties destination, WTableRow source)
        {
            // Import all sprms
            if (source.DataArray != null && source.DataArray.Length < 400 && source.DataArray.Length > 0)
            {
                SinglePropertyModifierArray modifierArray = new SinglePropertyModifierArray(source.DataArray, 0);
                for (int i = 0, cnt = modifierArray.Count; i < cnt; i++)
                {
                    if (!destination.HasOptions(modifierArray[i].TypedOptions) || destination.HasOptions(WordSprmOptions.sprmPPropRMark) || destination.HasOptions(WordSprmOptions.sprmPPropRMark90))
                    {
                        destination.Sprms.Modifiers.Add(modifierArray[i]);
                    }
                }
            }
            if (source.Height != 0)
            {
                short heightValue=(short)Math.Round(source.Height * DLSConstants.TwipsInOnePoint);
                if (source.RowFormat.Sprms != null)
                    destination.TableYRowHeight = heightValue;
                else
                    destination.TableYRowHeight = (short)(heightValue * ((source.HeightType == TableRowHeightType.AtLeast) ? 1 : -1));
            }

            if (source.RowFormat.HorizontalAlignment != RowAlignment.Left)
            {
                destination.TableAlignment = (ParagraphJustify)source.RowFormat.HorizontalAlignment;
            }

            if (!source.RowFormat.IsBreakAcrossPages)
            {
                destination.IsCantSplit = true;
                destination.IsCantSplit90 = true;
            }

            destination.TableWidthIndent = (short)(source.RowFormat.LeftIndent * DLSConstants.TwipsInOnePoint);
        }
        /// <summary>
        /// Formats to prop.
        /// </summary>
        /// <param name="propKey">The prop key.</param>
        /// <param name="value">The value.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="source">The source.</param>
        internal static void FormatToProp(int propKey, object value, ParagraphProperties destination, WParagraphFormat source)
        {
            switch (propKey)
            {
                #region Boolean properties
                case WParagraphFormat.BidiKey:
                    destination.Bidi = (bool)value;
                    break;
                case WParagraphFormat.KeepKey:
                    destination.Keep = (bool)value;
                    break;
                case WParagraphFormat.KeepFollowKey:
                    destination.KeepFollow = (bool)value;
                    break;
                case WParagraphFormat.PageBreakBeforeKey:
                    destination.PageBreakBefore = (bool)value;
                    break;
                case WParagraphFormat.MirrorIndentsKey:
                    destination.MirrorIndents = (bool)value;
                    break;
                case WParagraphFormat.WidowControlKey:
                    destination.WidowControl = (bool)value;
                    break;
                case WParagraphFormat.AdjustRightIndentKey:
                    destination.AdjustRightIndent = (bool)value;
                    break;
                case WParagraphFormat.AutoSpaceDNKey:
                    destination.AutoSpaceDN = (bool)value;
                    break;
                case WParagraphFormat.AutoSpaceDEKey:
                    destination.AutoSpaceDE = (bool)value;
                    break;
                case WParagraphFormat.SpacingBeforeAutoKey:
                    destination.SpacingBeforeAuto = (byte)((bool)value ? 1 : 0);
                    if (destination.SpacingBeforeAuto == 1)
                        destination.BeforeSpacing = 100;
                    break;
                case WParagraphFormat.SpacingAfterAutoKey:
                    destination.SpacingAfterAuto = (byte)((bool)value ? 1 : 0);
                    if (destination.SpacingAfterAuto == 1)
                        destination.AfterSpacing = 100;
                    break;
                case WParagraphFormat.ContextualSpacingKey:
                    destination.ContextualSpacing = (bool)value;
                    break;
                case WParagraphFormat.ChangedFormatKey:
                    destination.IsChangedFormat = (bool)value;
                    break;
                case WParagraphFormat.SuppressAutoHyphensKey:
                    destination.SuppressAutoHyphens = (bool)value;
                    break;
                case WParagraphFormat.WordWrapKey:
                    destination.WordWrap = (bool)value;
                    break;
                #endregion

                #region Float properties
                case WParagraphFormat.FrameXKey:
                    if (source.IsFrameXAlign((short)source.FrameX))
                        destination.FrameXCoordinate = (short)source.FrameX;
                    else
                        destination.FrameXCoordinate = (short)Math.Round((source.FrameX * DLSConstants.TwipsInOnePoint));
                    break;
                case WParagraphFormat.FrameYKey:
                    if (source.IsFrameYAlign((short)source.FrameY))
                        destination.FrameYCoordinate = (short)source.FrameY;
                    else
                        destination.FrameYCoordinate = (short)Math.Round((source.FrameY * DLSConstants.TwipsInOnePoint));
                    break;
                case WParagraphFormat.FrameHorizontalDistanceFromTextKey:
                    destination.FrameHorizontalDistanceFromText = (short)Math.Round((source.FrameHorizontalDistanceFromText * DLSConstants.TwipsInOnePoint));
                    break;
                case WParagraphFormat.FrameVerticalDistanceFromTextKey:
                    destination.FrameVerticalDistanceFromText = (short)Math.Round((source.FrameVerticalDistanceFromText * DLSConstants.TwipsInOnePoint));
                    break;
                case WParagraphFormat.FrameHeightKey:
                    destination.FrameHeight = (short)Math.Round((source.FrameHeight * DLSConstants.TwipsInOnePoint));
                    break;
                case WParagraphFormat.FrameWidthKey:
                    destination.FrameWidth = (short)Math.Round((source.FrameWidth * DLSConstants.TwipsInOnePoint));
                    break;
                case WParagraphFormat.WrapFrameAroundKey:
                    destination.WrapFrameAround = (byte)source.WrapFrameAround;
                    break;
                case WParagraphFormat.FirstLineIndentCharsKey:
                    destination.IndentLeftFirstChars = (short)Math.Round((float)value * DLSConstants.HundredthsUnit);
                    break;
                case WParagraphFormat.LeftIndentCharsKey:
                    destination.IndentLeftChars = (short)Math.Round((float)value * DLSConstants.HundredthsUnit);
                    break;
                case WParagraphFormat.RightIndentCharsKey:
                    destination.IndentRightChars = (short)Math.Round((float)value * DLSConstants.HundredthsUnit);
                    break;
                case WParagraphFormat.LeftIndentKey:
                    destination.IndentLeft = (short)Math.Round((float)value * DLSConstants.TwipsInOnePoint);
                    destination.IndentLeftBi = (short)Math.Round((float)value * DLSConstants.TwipsInOnePoint);
                    break;
                case WParagraphFormat.LeftIndentBiKey:
                    destination.IndentLeftBi = (short)Math.Round((float)value * DLSConstants.TwipsInOnePoint);
                    break;
                case WParagraphFormat.RightIndentKey:
                    destination.IndentRight = (short)Math.Round((float)value * DLSConstants.TwipsInOnePoint);
                    destination.IndentRightBi = (short)Math.Round((float)value * DLSConstants.TwipsInOnePoint);
                    break;
                case WParagraphFormat.RightIndentBiKey:
                    destination.IndentRightBi = (short)Math.Round((float)value * DLSConstants.TwipsInOnePoint);
                    break;
                case WParagraphFormat.FirstLineIndentKey:
                    destination.IndentLeftFirst = (short)Math.Round((float)value * DLSConstants.TwipsInOnePoint);
                    destination.IndentLeftFirstBi = (short)Math.Round((float)value * DLSConstants.TwipsInOnePoint);
                    break;
                case WParagraphFormat.FirstLineIndentBiKey:
                    destination.IndentLeftFirstBi = (short)Math.Round((float)value * DLSConstants.TwipsInOnePoint);
                    break;
                case WParagraphFormat.BeforeSpacingKey:
                    destination.BeforeSpacing = (ushort)Math.Round((float)value * DLSConstants.TwipsInOnePoint);
                    break;
                case WParagraphFormat.AfterSpacingKey:
                    destination.AfterSpacing = (ushort)Math.Round((float)value * DLSConstants.TwipsInOnePoint);
                    break;
                #endregion

                #region Other
                case WParagraphFormat.LineSpacingKey:
                    LineSpacingDescriptor desc = new LineSpacingDescriptor();
                    desc.LineSpacing = (short)Math.Round(((float)value * DLSConstants.TwipsInOnePoint));
                    desc.LineSpacingRule = source.LineSpacingRule;
                    destination.LineSpacingDescriptor = desc;
                    break;
                case WParagraphFormat.TabsKey:
                    ImportTabs(value as TabCollection, destination);
                    break;
                case WParagraphFormat.HrAlignmentKey:
                    if (source.Bidi)
                    {
                        if ((HorizontalAlignment)value == HorizontalAlignment.Left)
                        {
                            destination.Justification = ParagraphJustify.Right;
                            destination.JustificationBidi = ParagraphJustify.Right;
                        }
                        else if ((HorizontalAlignment)value == HorizontalAlignment.Right)
                        {
                            destination.Justification = ParagraphJustify.Left;
                            destination.JustificationBidi = ParagraphJustify.Left;
                        }
                        else
                        {
                            destination.Justification = (ParagraphJustify)((HorizontalAlignment)value);
                            destination.JustificationBidi = (ParagraphJustify)((HorizontalAlignment)value);
                        }
                    }
                    else
                    {
                        destination.Justification = (ParagraphJustify)((HorizontalAlignment)value);
                        destination.JustificationBidi = (ParagraphJustify)((HorizontalAlignment)value);
                    }
                    break;
                case WParagraphFormat.BackColorKey:
                    ImportShading(source, destination);
                    break;
                case WParagraphFormat.BordersKey:
                    ImportBorders(value as Borders, destination);
                    break;
                case WParagraphFormat.OutlineLevelKey:
                    destination.OutlineLevel = (byte)value;
                    break;
                case WParagraphFormat.LineSpacingRuleKey:
                    LineSpacingDescriptor spacingDesc = new LineSpacingDescriptor();
                    spacingDesc.LineSpacing = (short)Math.Round(source.LineSpacing * DLSConstants.TwipsInOnePoint);
                    spacingDesc.LineSpacingRule = (LineSpacingRule)value;
                    destination.LineSpacingDescriptor = spacingDesc;
                    break;
                case WParagraphFormat.TextureStyleKey :
                    ShadingDescriptor paragShading = new ShadingDescriptor();
                    paragShading.BackColor = source.BackColor;
                    paragShading.ForeColor = source.ForeColor;
                    paragShading.Pattern = (TextureStyle) value;
                    destination.Shading = paragShading;
                    destination.ShadingNew = paragShading;
                    break;
                default:
                    break;

                #endregion
            }
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Imports the tabs.
        /// </summary>
        /// <param name="tabCollection">The tab collection.</param>
        /// <param name="destination">The destination.</param>
        private static void ImportTabs(TabCollection tabCollection, ParagraphProperties destination)
        {
            bool deleted = false;
            int tabsCount = 0;
            int deleteIndex = 0;
            int tabIndex = 0;

            for (int i = 0; i < tabCollection.Count; i++)
            {
                if (tabCollection[i].DeletePosition == 0)
                    tabsCount++;
            }

            TabsInfo tabs = new TabsInfo((byte)tabsCount);
            short[] deletePositions = new short[(byte)tabCollection.Count - tabsCount];

            for (int i = 0, cnt = tabCollection.Count; i < cnt; i++)
            {
                Tab tab = tabCollection[i];

                if (tab.DeletePosition != 0)
                {
                    deleted = true;
                    deletePositions[deleteIndex] = (short)tab.DeletePosition;
                    deleteIndex++;
                }
                else if (tabsCount > 0)
                {
                    tabs.TabPositions[tabIndex] =
                      (short)Math.Round((tab.Position * DLSConstants.TwipsInOnePoint));
                    tabs.Descriptors[tabIndex] =
                      new TabDescriptor(tab.Justification, tab.TabLeader);
                    tabIndex++;
                }
            }

            if (deleted)
            {
                tabs.TabDeletePositions = deletePositions;
            }

            destination.TabsInfo = tabs;
        }
        /// <summary>
        /// Imports the shading.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        private static void ImportShading(WParagraphFormat source, ParagraphProperties destination)
        {
            if (source.BackColor != Color.Empty)
            {
                ShadingDescriptor paragShading = new ShadingDescriptor();
                paragShading.BackColor = source.BackColor;
                paragShading.ForeColor = source.ForeColor;
                paragShading.Pattern = source.TextureStyle;
                destination.Shading = paragShading;
                destination.ShadingNew = paragShading;
            }
        }
        /// <summary>
        /// Imports the borders.
        /// </summary>
        /// <param name="borders">The borders.</param>
        /// <param name="destination">The destination.</param>
        private static void ImportBorders(Borders borders, ParagraphProperties destination)
        {
            BorderCode brc = new BorderCode();

            ImportBorder(brc, borders.Top);
            destination.TopBorder = brc;
            destination.TopBorderNew = brc;

            ImportBorder(brc, borders.Left);
            destination.LeftBorder = brc;
            destination.LeftBorderNew = brc;

            ImportBorder(brc, borders.Bottom);
            destination.BottomBorder = brc;
            destination.BottomBorderNew = brc;

            ImportBorder(brc, borders.Right);
            destination.RightBorder = brc;
            destination.RightBorderNew = brc;

            ImportBorder(brc, borders.Horizontal);
            destination.BetweenBorder = brc;

            ImportBorder(brc, borders.Vertical);
            destination.BarBorder = brc;
        }
        #endregion

        #region Commented code
        //    /// <summary>
        //    /// Exports formatting from ParagraphProperties to WParagraphFormat.
        //    /// </summary>
        //    public static void Export( ParagraphProperties source, WParagraphFormat destination, bool parseAll )
        //    {
        //      Borders destBorders = null;
        ////      IEnumerator enm = source.Sprms.GetEnumerator();
        //
        //      ArrayList modifiers = source.Sprms.Modifiers;
        //      int count = modifiers.Count;
        //      for( int j = 0; j < count; j++ )
        ////      while (enm.MoveNext())
        //      {
        ////        SinglePropertyModifierRecord sprm = (SinglePropertyModifierRecord)enm.Current;
        //        SinglePropertyModifierRecord sprm = ( SinglePropertyModifierRecord )modifiers[ j ];
        //        int options = sprm.TypedOptions;
        //
        //        switch( options )
        //        {
        //          case WordSprmOptions.sprmPChgTabsPapx :
        //          {
        //            TabCollection tabs = destination.Tabs;
        //            tabs.Clear();
        //            TabsInfo info = source.TabsInfo;
        //
        //            ExportTabs( info, tabs );
        //            break;
        //          }
        //          case WordSprmOptions.sprmPChgTabs:
        //            {
        //              TabCollection tabs = destination.Tabs;
        //              TabsInfo info = source.ListTabs;
        //              
        //              ExportTabs( info, tabs );
        //              break;
        //            }
        //
        //          case  WordSprmOptions.sprmPJc:
        //            {
        //            destination.HorizontalAlignment = ( HorizontalAlignment )sprm.ByteValue;
        //            //              destination.HorizontalAlignment = ( HorizontalAlignment )source.Justification;
        //              break;
        //            }
        //          case WordSprmOptions.sprmPFKeep:
        //            {
        //            destination.Keep = sprm.BoolValue;
        ////            destination.Keep = source.Keep;
        //              break;
        //            }
        //          case WordSprmOptions.sprmPFKeepFollow:
        //            {
        //            destination.KeepFollow = sprm.BoolValue;
        ////            destination.KeepFollow = source.KeepFollow;
        //              break;
        //            }
        //          case WordSprmOptions.sprmPFPageBreakBefore:
        //            {
        //            destination.PageBreakBefore = sprm.BoolValue;
        ////            destination.PageBreakBefore = source.PageBreakBefore;
        //              break;
        //            }
        //          case WordSprmOptions.sprmPFWidowControl:
        //            {
        //            destination.WidowControl = sprm.BoolValue;
        ////            destination.WidowControl = source.WidowControl;
        //              break;
        //            }
        //          case WordSprmOptions.sprmPDxaLeft:
        //            {
        //            destination.LeftIndent = ( float )sprm.ShortValue / DLSConstants.TwipsInOnePoint;
        ////            destination.LeftIndent = ( float )source.IndentLeft / DLSConstants.TwipsInOnePoint;
        //              break;
        //            }
        //          case WordSprmOptions.sprmPDxaRight://|| source.HasOptions( WordSprmOptions.sprmPDxaRight2 ) )
        //            {
        //            destination.RightIndent = ( float )sprm.ShortValue / DLSConstants.TwipsInOnePoint;
        ////            destination.RightIndent = ( float )source.IndentRight / DLSConstants.TwipsInOnePoint;
        //              break;
        //            }
        //          case WordSprmOptions.sprmPDxaLeft1:
        //            {
        //            destination.FirstLineIndent = ( float )sprm.ShortValue / DLSConstants.TwipsInOnePoint;
        ////            destination.FirstLineIndent = ( float )source.IndentLeftFirst / DLSConstants.TwipsInOnePoint;
        //              break;
        //            }
        //          case WordSprmOptions.sprmPDyaBefore:
        //            {
        //            destination.BeforeSpacing = ( float )sprm.ShortValue / DLSConstants.TwipsInOnePoint;
        ////            destination.BeforeSpacing = ( float )source.BeforeSpacing / DLSConstants.TwipsInOnePoint;
        //              break;
        //            }
        //          case WordSprmOptions.sprmPDyaAfter:
        //            {
        //            destination.AfterSpacing = ( float )sprm.ShortValue / DLSConstants.TwipsInOnePoint;
        ////            destination.AfterSpacing = ( float )source.AfterSpacing / DLSConstants.TwipsInOnePoint;
        //              break;
        //            }
        //          case WordSprmOptions.sprmPShd:
        //            {
        //              ShadingDescriptor shading = source.GetShading( sprm );
        //              destination.BackColor = source.GetShading( sprm ).BackColor;
        //              destination.ForeColor = shading.ForeColor;
        //              destination.TextureStyle = shading.Pattern;
        //              //            destination.BackColor = source.Shading.BackColor;
        //              break;
        //            }
        //          case WordSprmOptions.sprmPShdNew:
        //            {
        //              ShadingDescriptor shading = source.GetShading( sprm );
        //              destination.BackColor = source.GetShading( sprm ).BackColor;
        //              destination.ForeColor = shading.ForeColor;
        //              destination.TextureStyle = shading.Pattern;
        //              //            destination.BackColor = source.ShadingNew.BackColor;
        //              break;
        //            }
        //          case WordSprmOptions.sprmPFBiDi:
        //            {
        //            destination.Bidi = sprm.BoolValue;
        ////            destination.Bidi = source.Bidi;
        //              break;
        //            }
        //          case WordSprmOptions.sprmPDyaLine:
        //            {
        //              LineSpacingDescriptor descriptor = source.GetLineSpacingDescriptor( sprm );
        //              destination.LineSpacing = ( float )descriptor.LineSpacing / DLSConstants.TwipsInOnePoint;
        //              destination.LineSpacingRule = descriptor.LineSpacingRule;
        //              //              destination.LineSpacing = ( float )source.LineSpacingDescriptor.LineSpacing / DLSConstants.TwipsInOnePoint;
        //              //              destination.LineSpacingRule = source.LineSpacingDescriptor.LineSpacingRule;
        //              break;
        //            }
        //          case WordSprmOptions.sprmPBrcTop:
        //            {
        //              destBorders = GetDestBorders( destBorders, destination );
        //              BorderCode srcBorder = source.GetBorder( sprm );
        //              ExportBorder( srcBorder, destBorders.Top );
        //              break;
        //            }
        //          case WordSprmOptions.sprmPBrcTopNew:
        //            {
        //            destBorders = GetDestBorders( destBorders, destination );
        //            BorderCode srcBorder = source.GetBorder( sprm );
        //            ExportBorder( srcBorder, destBorders.Top );
        //              break;
        //            }
        //          case WordSprmOptions.sprmPBrcBottom:
        //            {
        //            destBorders = GetDestBorders( destBorders, destination );
        //            BorderCode srcBorder = source.GetBorder( sprm );
        //            ExportBorder( srcBorder, destBorders.Bottom );
        //              break;
        //            }
        //          case WordSprmOptions.sprmPBrcBottomNew:
        //            {
        //            destBorders = GetDestBorders( destBorders, destination );
        //            BorderCode srcBorder = source.GetBorder( sprm );
        //            ExportBorder( srcBorder, destBorders.Bottom );
        //              break;
        //            }
        //          case WordSprmOptions.sprmPBrcLeft:
        //            {
        //            destBorders = GetDestBorders( destBorders, destination );
        //            BorderCode srcBorder = source.GetBorder( sprm );
        //            ExportBorder( srcBorder, destBorders.Left );
        //              break;
        //            }
        //          case WordSprmOptions.sprmPBrcLeftNew:
        //            {
        //            destBorders = GetDestBorders( destBorders, destination );
        //            BorderCode srcBorder = source.GetBorder( sprm );
        //            ExportBorder( srcBorder, destBorders.Left );
        //              break;
        //            }
        //          case WordSprmOptions.sprmPBrcRight:
        //            {
        //            destBorders = GetDestBorders( destBorders, destination );
        //            BorderCode srcBorder = source.GetBorder( sprm );
        //            ExportBorder( srcBorder, destBorders.Right );
        //              break;
        //            }
        //          case WordSprmOptions.sprmPBrcRightNew:
        //            {
        //            destBorders = GetDestBorders( destBorders, destination );
        //            BorderCode srcBorder = source.GetBorder( sprm );
        //            ExportBorder( srcBorder, destBorders.Right );
        //              break;
        //            }
        //        }
        //      }
        //
        //      // Export all sprms
        ////      if( parseAll )
        ////      {
        ////        SinglePropertyModifierArray sprms = source.GetCopiableSprm();
        ////        if( sprms.Count > 0 )
        ////        {
        ////          destination.DataArray = new byte[ sprms.Length ];
        ////          sprms.Save( destination.DataArray, 0, null );
        ////        }
        ////      }
        //    }
        //    /// <summary>
        //    /// Exports formatting from ParagraphProperties to ParagraphFormat
        //    /// </summary>
        //    public static void Export( ParagraphProperties source, WTableRow destination, bool parseAll )
        //    {
        //      if( source.HasOptions( WordSprmOptions.sprmTDyaRowHeight ))
        //      {
        //        destination.Height = (float)source.TableYRowHeight / DLSConstants.TwipsInOnePoint;
        //      }
        //      if( source.HasOptions( WordSprmOptions.sprmTJc ) )
        //      {
        //        destination.RowFormat.HorizontalAlignment = ( RowAlignment )source.TableAlignment;
        //      }
        //      if( source.HasOptions( WordSprmOptions.sprmTFCantSplit ) )
        //      {
        //        destination.RowFormat.IsBreakAcrossPages = !source.IsCantSplit;
        //      }
        //      // Export all sprms
        //      if( parseAll )
        //      {
        //        SinglePropertyModifierArray sprms = source.GetCopiableTableSprm();
        //        if( sprms.Count > 0 )
        //        {
        //          destination.DataArray = new byte[ sprms.Length ];
        //          sprms.Save( destination.DataArray, 0, null );
        //        }
        //      }
        //    }
        //    /// <summary>
        //    /// Imports formatting from WParagraphFormat to ParagraphProperties 
        //    /// </summary>
        //    public static void Import( ParagraphProperties destination, WParagraphFormat source, bool parseAll )
        //    {
        ////      PERFORM_UTIL.Start();
        //      bool hasBorders = false;
        //      // Import all sprms
        ////      if( parseAll && source.DataArray != null && source.DataArray.Length < 300 && source.DataArray.Length > 0 )
        ////      {
        ////        SinglePropertyModifierArray modifierArray = new SinglePropertyModifierArray( source.DataArray, 0, null );
        //////        foreach( SinglePropertyModifierRecord sprm in modifierArray )
        ////        SinglePropertyModifierRecord sprm = null;
        ////        for( int i = 0, cnt = modifierArray.Modifiers.Count; i < cnt; i++ )
        ////        {
        ////          sprm = modifierArray.GetSprmByIndex( i );
        ////          if( !destination.HasOptions( sprm.TypedOptions ))
        ////          {
        ////            destination.Sprms.Modifiers.Add( sprm );
        ////          }
        ////        }
        ////      }
        //      
        //      IDictionaryEnumerator dEnum = source.PropertiesHash.GetEnumerator();
        //      while (dEnum.MoveNext())
        //      {
        //        int key = (int)dEnum.Key;
        //
        //        switch( key )
        //        {
        //          case WParagraphFormat.HrAlignmentKey:
        //            {
        //              if(  source.HorizontalAlignment != HorizontalAlignment.Left )
        //              {
        //                destination.Justification = ( ParagraphJustify )source.HorizontalAlignment;
        //                if( !source.Bidi /*&& source.DataArray == null*/ )
        //                {
        //                  destination.JustificationBidi = ( ParagraphJustify )source.HorizontalAlignment;
        //                }
        //              }             
        //              else if( source.Bidi ) 
        //              {
        //                if( source.HorizontalAlignment == HorizontalAlignment.Left )
        //                {
        //                  destination.JustificationBidi = ParagraphJustify.Right;
        //                }
        //                else if( source.HorizontalAlignment == HorizontalAlignment.Right )
        //                {
        //                  destination.JustificationBidi = ParagraphJustify.Left;
        //                }
        //                else
        //                {
        //                  destination.JustificationBidi = ( ParagraphJustify )source.HorizontalAlignment;
        //                }
        //              }
        //              break;
        //            }
        //          case WParagraphFormat.KeepKey:
        //            {
        //              destination.Keep = source.Keep;
        //              break;
        //            }
        //          case WParagraphFormat.KeepFollowKey:
        //            {
        //              destination.KeepFollow = source.KeepFollow;
        //              break;
        //            }
        //          case WParagraphFormat.PageBreakBeforeKey:
        //            {
        //              destination.PageBreakBefore = source.PageBreakBefore;
        //              break;
        //            }
        //          case WParagraphFormat.WidowControlKey:
        //            {
        //              destination.WidowControl = source.WidowControl;
        //              break;
        //            }
        //          case WParagraphFormat.LeftIndentKey:
        //            {
        //              destination.IndentLeft = ( short )Math.Round( source.LeftIndent * DLSConstants.TwipsInOnePoint );
        //              if( source.Sprms == null )
        //              {
        //                //Word 2000 fix 
        //                destination.IndentLeftBi = ( short )Math.Round( source.LeftIndent * DLSConstants.TwipsInOnePoint );
        //              }
        //              break;
        //            }
        //          case WParagraphFormat.RightIndentKey:
        //            {
        //              destination.IndentRight = ( short )Math.Round( source.RightIndent * DLSConstants.TwipsInOnePoint );
        //              break;
        //            }
        //          case WParagraphFormat.FirstLineIndentKey:
        //            {
        //              destination.IndentLeftFirst =
        //                ( short )Math.Round( source.FirstLineIndent * DLSConstants.TwipsInOnePoint );
        //              break;
        //            }
        //          case WParagraphFormat.BeforeSpacingKey:
        //            {
        //              destination.BeforeSpacing =
        //                ( ushort )Math.Round( source.BeforeSpacing * DLSConstants.TwipsInOnePoint );
        //              break;
        //            }
        //          case WParagraphFormat.AfterSpacingKey:
        //            {
        //              destination.AfterSpacing = ( ushort )Math.Round( source.AfterSpacing * DLSConstants.TwipsInOnePoint );
        //              break;
        //            }
        //          case WParagraphFormat.BidiKey:
        //            {
        //              destination.Bidi = source.Bidi;
        //              break;
        //            }
        //          case WParagraphFormat.LineSpacingKey:
        //            {
        //              LineSpacingDescriptor desc = new LineSpacingDescriptor();
        //              desc.LineSpacing = ( short )Math.Round( ( source.LineSpacing * DLSConstants.TwipsInOnePoint ) );
        //              desc.LineSpacingRule = source.LineSpacingRule;
        //        
        //              destination.LineSpacingDescriptor = desc;
        //              break;
        //            }
        //          case WParagraphFormat.BordersKey:
        //            {
        //              if( !source.Borders.IsDefault )
        //              {
        //                hasBorders = true;
        //              }
        //              break;
        //            }
        //          case WParagraphFormat.BackColorKey:
        //            {
        //              if( source.BackColor != Color.Empty )
        //              {
        //                ShadingDescriptor paragShading = new ShadingDescriptor();
        //                paragShading.BackColor = source.BackColor;
        //                paragShading.ForeColor = source.ForeColor;
        //                paragShading.Pattern = source.TextureStyle;
        //                destination.Shading = paragShading;
        //                destination.ShadingNew = paragShading;
        //              }
        //            }
        //            break;
        //        }
        //      }
        //      if( hasBorders )
        //      {
        //        Borders borders = source.Borders;
        //        BorderCode brc = new BorderCode();
        //      
        //        if( !borders.Top.IsDefault )
        //        {
        //          ImportBorder( brc, borders.Top );
        //          destination.TopBorder = brc;
        //          destination.TopBorderNew = brc;
        //        }
        //      
        //        if( !borders.Left.IsDefault )
        //        {
        //          ImportBorder( brc, borders.Left );
        //          destination.LeftBorder = brc;
        //          destination.LeftBorderNew = brc;
        //        }
        //              
        //        if( !borders.Bottom.IsDefault )
        //        {
        //          ImportBorder( brc, borders.Bottom );
        //          destination.BottomBorder = brc;
        //          destination.BottomBorderNew = brc;
        //        }
        //              
        //        if( !borders.Right.IsDefault )
        //        {
        //          ImportBorder( brc, borders.Right );
        //          destination.RightBorder = brc;
        //          destination.RightBorderNew = brc;
        //        }
        //      }
        //      if( source.Tabs.Count > 0 )
        //      {        
        //        bool deleted = false;
        //        int tabsCount = 0;
        //        int deleteIndex = 0;
        //        int tabIndex = 0;
        //
        //        for( int i = 0; i < source.Tabs.Count; i++ )
        //        {
        //          if( source.Tabs[ i ].DeletePosition == 0 )
        //            tabsCount++;
        //        }
        //
        //        TabsInfo tabs = new TabsInfo( ( byte )tabsCount );
        //        short[] deletePositions = new short[ ( byte )source.Tabs.Count - tabsCount ];
        //
        //        for( int i = 0, cnt = source.Tabs.Count; i < cnt; i++ )
        //        {
        //          Tab tab = source.Tabs[ i ];
        //
        //          if( tab.DeletePosition != 0 )
        //          {
        //            deleted = true;
        //            deletePositions[ deleteIndex ] = ( short )tab.DeletePosition;
        //            deleteIndex++;
        //          }
        //          else if( tabsCount > 0 )
        //          {
        //            tabs.TabPositions[ tabIndex ] =
        //              ( short )Math.Round( ( tab.Position * DLSConstants.TwipsInOnePoint ) );
        //            tabs.Descriptors[ tabIndex ] =
        //              new TabDescriptor( tab.Justification, tab.TabLeader );
        //            tabIndex++;
        //          }
        //        }
        //        
        //        if( deleted )
        //        {
        //          tabs.TabDeletePositions = deletePositions;
        //        }
        //        
        //        destination.TabsInfo = tabs;
        //      }
        //    }
        //    /// <summary>
        //    /// Imports formatting from WParagraphFormat to ParagraphProperties 
        //    /// </summary>
        #endregion
    }
    /// <summary>
    /// Summary description for class SectionPropertiesConverter
    /// </summary>
    //[ CLSCompliant( false ) ]
    [DocumentationExclude()]
    internal class SectionPropertiesConverter
    {
        #region Class utility methods
        /// <summary>
        ///  Exports formatting from SectionProperties to ISection's PageSettings
        /// </summary>
        /// <param name="source">Source properties</param>
        /// <param name="destination">Destination.</param>
        /// <param name="parseAll">Prase All.</param>  
        public static void Export(SectionProperties source, WSection destination, bool parseAll)
        {
            Borders destBorders = null;
            WPageSetup pageSetup = destination.PageSetup;
            List<SinglePropertyModifierRecord> modifiers = source.Sprms.Modifiers;
            int count = modifiers.Count;
            UpdatePageOrientation(source.Sprms, pageSetup);
            for (int j = 0; j < count; j++)
            {
                SinglePropertyModifierRecord sprm = modifiers[j];
                int options = sprm.TypedOptions;

                switch (options)
                {
                    case WordSprmOptions.sprmSXaPage:
                        {
                            pageSetup.PageSize =
                              new SizeF((float)source.PageWidth / DLSConstants.TwipsInOnePoint,
                              pageSetup.PageSize.Height);
                            break;
                        }
                    case WordSprmOptions.sprmSYaPage:
                        {
                            pageSetup.PageSize =
                              new SizeF(pageSetup.PageSize.Width,
                              (float)source.PageHeight / DLSConstants.TwipsInOnePoint);
                            break;
                        }
                    case WordSprmOptions.sprmSDyaHdrTop:
                        {
                            pageSetup.HeaderDistance = (float)source.HeaderHeight / DLSConstants.TwipsInOnePoint;
                            break;
                        }
                    case WordSprmOptions.sprmSDyaHdrBottom:
                        {
                            pageSetup.FooterDistance = (float)source.FooterHeight / DLSConstants.TwipsInOnePoint;
                            break;
                        }
                    case WordSprmOptions.sprmSVjc:
                        {
                            pageSetup.VerticalAlignment = (PageAlignment)source.VerticalAlignment;
                            break;
                        }
                    case WordSprmOptions.sprmSBOrientation:
                        {
                            pageSetup.Orientation = (PageOrientation)source.Orientation;
                            break;
                        }
                    case WordSprmOptions.sprmSDyaBottom:
                        {
                            pageSetup.Margins.Bottom = (float)source.BottomMargin / DLSConstants.TwipsInOnePoint;
                            break;
                        }
                    case WordSprmOptions.sprmSDyaTop:
                        {
                            pageSetup.Margins.Top = (float)source.TopMargin / DLSConstants.TwipsInOnePoint;
                            break;
                        }
                    case WordSprmOptions.sprmSDxaLeft:
                        {
                            pageSetup.Margins.Left = (float)source.LeftMargin / DLSConstants.TwipsInOnePoint;
                            break;
                        }
                    case WordSprmOptions.sprmSDxaRight:
                        {
                            pageSetup.Margins.Right = (float)source.RightMargin / DLSConstants.TwipsInOnePoint;
                            break;
                        }
                    case WordSprmOptions.sprmSDzaGutter:
                        {
                            pageSetup.Margins.Gutter = (float)source.Gutter / DLSConstants.TwipsInOnePoint;
                            break;
                        }
                    case WordSprmOptions.sprmSBkc:
                        {
                            destination.BreakCode = (SectionBreakCode)source.BreakCode;
                            break;
                        }
                    case WordSprmOptions.sprmSFTitlePage:
                        {
                            pageSetup.DifferentFirstPage = source.TitlePage;
                            break;
                        }

                    case WordSprmOptions.sprmSLnc:
                        if (source.HasOptions(WordSprmOptions.sprmSNLnnMod) && source.HasOptions(WordSprmOptions.sprmSLnnMin) && source.HasOptions(WordSprmOptions.sprmSDxaLnn))
                        {
                            pageSetup.LineNumberingMode = source.LineNumberingMode;
                            pageSetup.LineNumberingStep = source.LineNumberingStep;
                            pageSetup.LineNumberingStartValue = source.LineNumberingStartValue;
                            pageSetup.LineNumberingDistanceFromText = (float)source.LineNumberingDistanceFromText / DLSConstants.TwipsInOnePoint;
                        }
                        break;
                    case WordSprmOptions.sprmSPgbProp:
                        {
                            pageSetup.PageBordersApplyType = source.PageBorderApply;
                            pageSetup.IsFrontPageBorder = source.PageBorderIsInFront;
                            pageSetup.PageBorderOffsetFrom = source.PageBorderOffsetFrom;
                        }
                        break;
                    case WordSprmOptions.sprmSFBiDi:
                        {
                            pageSetup.Bidi = source.Bidi;
                        }
                        break;
                    case WordSprmOptions.sprmSBrcTop:
                        {
                            destBorders = GetDestBorders(destBorders, pageSetup);
                            BorderCode srcBorder = source.GetBorder(sprm);
                            ExportBorder(srcBorder, destBorders.Top);
                            break;
                        }
                    case WordSprmOptions.sprmSBrcTopNew:
                        {
                            destBorders = GetDestBorders(destBorders, pageSetup);
                            BorderCode srcBorder = source.GetBorder(sprm);
                            ExportBorder(srcBorder, destBorders.Top);
                            break;
                        }
                    case WordSprmOptions.sprmSBrcBottom:
                        {
                            destBorders = GetDestBorders(destBorders, pageSetup);
                            BorderCode srcBorder = source.GetBorder(sprm);
                            ExportBorder(srcBorder, destBorders.Bottom);
                            break;
                        }
                    case WordSprmOptions.sprmSBrcBottomNew:
                        {
                            destBorders = GetDestBorders(destBorders, pageSetup);
                            BorderCode srcBorder = source.GetBorder(sprm);
                            ExportBorder(srcBorder, destBorders.Bottom);
                            break;
                        }
                    case WordSprmOptions.sprmSBrcLeft:
                        {
                            destBorders = GetDestBorders(destBorders, pageSetup);
                            BorderCode srcBorder = source.GetBorder(sprm);
                            ExportBorder(srcBorder, destBorders.Left);
                            break;
                        }
                    case WordSprmOptions.sprmSBrcLeftNew:
                        {
                            destBorders = GetDestBorders(destBorders, pageSetup);
                            BorderCode srcBorder = source.GetBorder(sprm);
                            ExportBorder(srcBorder, destBorders.Left);
                            break;
                        }
                    case WordSprmOptions.sprmSBrcRight:
                        {
                            destBorders = GetDestBorders(destBorders, pageSetup);
                            BorderCode srcBorder = source.GetBorder(sprm);
                            ExportBorder(srcBorder, destBorders.Right);
                            break;
                        }
                    case WordSprmOptions.sprmSBrcRightNew:
                        {
                            destBorders = GetDestBorders(destBorders, pageSetup);
                            BorderCode srcBorder = source.GetBorder(sprm);
                            ExportBorder(srcBorder, destBorders.Right);
                            break;
                        }
                    case WordSprmOptions.sprmSDyaLinePitch:
                        pageSetup.LinePitch = (float)source.LinePitch / DLSConstants.TwipsInOnePoint;
                        break;
                    case WordSprmOptions.sprmSClm:
                        pageSetup.PitchType = (GridPitchType)source.PitchType;
                        break;
                    case WordSprmOptions.sprmSLBetween:
                        pageSetup.DrawLinesBetweenCols = source.DrawLinesBetweenCols;
                        break;
                    case WordSprmOptions.sprmSFProtected:
                        destination.ProtectForm = !source.ProtectForm;
                        break;
                    case WordSprmOptions.sprmSTextFlow:
                        destination.m_textDirection = (DocTextDirection)source.TextDirection;
                        break;
                    #region Export Footnote/Endnotes formattings
                    case WordSprmOptions.sprmSRncEdn:
                        destination.PageSetup.RestartIndexForEndnote = (EndnoteRestartIndex)source.RestartIndexForEndnote;
                        break;
                    case WordSprmOptions.sprmSRncFtn:
                        destination.PageSetup.RestartIndexForFootnotes = (FootnoteRestartIndex)source.RestartIndexForFootnotes;
                        break;
                    case WordSprmOptions.sprmSNFtn:
                        destination.PageSetup.InitialFootnoteNumber = source.InitialFootnoteNumber;
                        break;
                    case WordSprmOptions.sprmSNEdn:
                        destination.PageSetup.InitialEndnoteNumber = source.InitialEndnoteNumber;
                        break;
                    case WordSprmOptions.sprmSNfcFtnRef:
                        if (destination.Document.WordVersion <= 217)
                            destination.PageSetup.FootnoteNumberFormat = destination.Document.FootnoteNumberFormat;
                        else
                            destination.PageSetup.FootnoteNumberFormat = (FootEndNoteNumberFormat)source.FootnoteNumberFormat;
                        break;
                    case WordSprmOptions.sprmSNfcEdnRef:
                        if (destination.Document.WordVersion <= 217)
                            destination.PageSetup.EndnoteNumberFormat = destination.Document.EndnoteNumberFormat;
                        else
                            destination.PageSetup.EndnoteNumberFormat = (FootEndNoteNumberFormat)source.EndnoteNumberFormat;
                        break;
                    case WordSprmOptions.sprmSFpc:
                        destination.PageSetup.FootnotePosition = (FootnotePosition)source.FootnotePosition;
                        break;
                    #endregion
                }
            }

            destination.PageSetup.EqualColumnWidth = source.Columns.ColumnsEvenlySpaced;
            destination.PageSetup.PageNumberStyle = (PageNumberStyle)source.PageNfc;
            destination.PageSetup.RestartPageNumbering = source.PageRestart;
            destination.PageSetup.PageNumbers.ChapterPageSeparator = (ChapterPageSeparatorType)source.ChapterPageSeparator;
            destination.PageSetup.PageNumbers.HeadingLevelForChapter = (HeadingLevel)source.HeadingLevelForChapter;
            if (source.PageRestart)
            {
                destination.PageSetup.PageStartingNumber = (int)source.PageStartAt;
            }
            //      foreach( ColumnDescriptor colDesc in source.Columns )
            ColumnDescriptor colDesc = null;
            for (int i = 0, cnt = source.Columns.Count; i < cnt; i++)
            {
                colDesc = source.Columns[i];
                if (colDesc.Width != 0)
                {
                    Column column = destination.AddColumn(0, 0);
                    column.Width = (float)colDesc.Width / DLSConstants.TwipsInOnePoint;
                    column.Space = (float)colDesc.Space / DLSConstants.TwipsInOnePoint;
                }
            }
            if (parseAll)
            {
                SinglePropertyModifierArray sprms = source.GetCopiableSprm();
                destination.DataArray = new byte[sprms.Length];
                sprms.Save(destination.DataArray, 0);
            }
        }
        /// <summary>
        /// Update the page orientation to portrait if the corresponding sprm is not found.
        /// </summary>
        /// <param name="sprms"></param>
        /// <param name="pageSetup"></param>
        private static void UpdatePageOrientation(SinglePropertyModifierArray sprms, WPageSetup pageSetup)
        {
            if (sprms != null && !sprms.Contain(WordSprmOptions.sprmSBOrientation) && pageSetup.Orientation != PageOrientation.Portrait)
            {
                pageSetup.Orientation = PageOrientation.Portrait;
            }
        }
        /// <summary>
        /// Imports formatting from WParagraphFormat to SectionProperties
        /// </summary>
        public static void Import(SectionProperties destination, WSection source)
        {   
            // Import all sprms
            if (source.DataArray != null && source.DataArray.Length < 300 && source.DataArray.Length > 0)
            {
                SinglePropertyModifierArray modifierArray = new SinglePropertyModifierArray(source.DataArray, 0);
                SinglePropertyModifierRecord sprm = null;
                for (int i = 0, cnt = modifierArray.Count; i < cnt; i++)
                {
                    sprm = modifierArray.GetSprmByIndex(i);
                    //          if( !destination.Sprms.Modifiers.Contains( sprm ))
                    //          {
                    //            destination.Sprms.Modifiers.Add( sprm );
                    //          }
                    if (!destination.HasOptions(sprm.TypedOptions) || destination.HasOptions(WordSprmOptions.sprmSPgbProp) || destination.HasOptions(WordSprmOptions.sprmSPropRMark))
                    {
                        destination.Sprms.Modifiers.Add(sprm);
                    }
                }
            }
            if (source.PageSetup.PageSize.Width != 0)
            {
                destination.PageWidth =
                  (ushort)Math.Round(source.PageSetup.PageSize.Width * DLSConstants.TwipsInOnePoint);
            }
            if (source.PageSetup.PageSize.Height != 0)
            {
                destination.PageHeight =
                  (ushort)Math.Round(source.PageSetup.PageSize.Height * DLSConstants.TwipsInOnePoint);
            }
            if (source.PageSetup.VerticalAlignment != 0)
            {
                destination.VerticalAlignment = (byte)source.PageSetup.VerticalAlignment;
            }
            if (source.PageSetup.Orientation != 0)
            {
                destination.Orientation = (byte)source.PageSetup.Orientation;
            }

            destination.TextDirection = (byte)source.TextDirection;
            destination.LeftMargin =
              (short)Math.Round(source.PageSetup.Margins.Left * DLSConstants.TwipsInOnePoint);
            destination.RightMargin =
              (short)Math.Round(source.PageSetup.Margins.Right * DLSConstants.TwipsInOnePoint);
            destination.TopMargin =
              (short)Math.Round(source.PageSetup.Margins.Top * DLSConstants.TwipsInOnePoint);
            destination.BottomMargin =
              (short)Math.Round(source.PageSetup.Margins.Bottom * DLSConstants.TwipsInOnePoint);
            destination.Gutter =
              (short)Math.Round(source.PageSetup.Margins.Gutter * DLSConstants.TwipsInOnePoint);
            destination.HeaderHeight =
              (short)Math.Round(source.PageSetup.HeaderDistance * DLSConstants.TwipsInOnePoint);
            destination.FooterHeight =
              (short)Math.Round((source.PageSetup.FooterDistance * DLSConstants.TwipsInOnePoint));

            if (source.PageSetup.DifferentFirstPage)
            {
                destination.TitlePage = source.PageSetup.DifferentFirstPage;
            }
            //      if( source.PageSetup.DifferentOddAndEvenPages )
            //      {
            //        destination.OddAndEvenPagesHeaderFooter = true;
            //      }

            if (source.PageSetup.LineNumberingMode != LineNumberingMode.None)
            {
                destination.LineNumberingMode = source.PageSetup.LineNumberingMode;

                if (source.PageSetup.LineNumberingStep != 0)
                {
                    destination.LineNumberingStep = (ushort)source.PageSetup.LineNumberingStep;
                }
                //        if( source.PageSetup.LineNumberingStartValue != 0 )
                {
                    destination.LineNumberingStartValue = (short)source.PageSetup.LineNumberingStartValue;
                }
                if (source.PageSetup.LineNumberingDistanceFromText != 0)
                {
                    destination.LineNumberingDistanceFromText = (short)Math.Round(source.PageSetup.LineNumberingDistanceFromText * DLSConstants.TwipsInOnePoint);
                }
            }

            if (source.PageSetup.Bidi)
            {
                destination.Bidi = source.PageSetup.Bidi;
            }

            if (!source.PageSetup.Borders.IsDefault)
            {
                Borders borders = source.PageSetup.Borders;
                BorderCode brc = new BorderCode();

                ImportBorder(brc, borders.Top);
                destination.TopBorder = brc;
                destination.TopBorderNew = brc;

                ImportBorder(brc, borders.Left);
                destination.LeftBorder = brc;
                destination.LeftBorderNew = brc;

                ImportBorder(brc, borders.Bottom);
                destination.BottomBorder = brc;
                destination.BottomBorderNew = brc;

                ImportBorder(brc, borders.Right);
                destination.RightBorder = brc;
                destination.RightBorderNew = brc;
            }

            destination.PageBorderApply = source.PageSetup.PageBordersApplyType;
            destination.PageBorderIsInFront = source.PageSetup.IsFrontPageBorder;
            destination.PageBorderOffsetFrom = source.PageSetup.PageBorderOffsetFrom;

            destination.PageNfc = (byte)source.PageSetup.PageNumberStyle;
            if (source.PageSetup.RestartPageNumbering)
            {
                destination.PageRestart = source.PageSetup.RestartPageNumbering;
                destination.PageStartAt = (ushort)source.PageSetup.PageStartingNumber;
            }
            else
            {
                destination.Sprms.RemoveValue(WordSprmOptions.sprmSFPgnRestart);
                destination.Sprms.RemoveValue(WordSprmOptions.sprmSPgnStart);
            }

            if (source.PageSetup.PageNumbers.HeadingLevelForChapter != 0)
            {
                destination.HeadingLevelForChapter = (byte)source.PageSetup.PageNumbers.HeadingLevelForChapter;
                destination.ChapterPageSeparator = (byte)source.PageSetup.PageNumbers.ChapterPageSeparator;
            }
            else
            {
                destination.Sprms.RemoveValue(WordSprmOptions.sprmScnsPgn);
                destination.Sprms.RemoveValue(WordSprmOptions.sprmSiHeadingPgn);
            }
            // Columns importing
            destination.Columns.Clear();
            #region Import Footnote/Endnotes formattings
            if (source.PageSetup.FootnoteNumberFormat != FootEndNoteNumberFormat.Arabic)
                destination.FootnoteNumberFormat = (byte)source.PageSetup.FootnoteNumberFormat;
            if (source.PageSetup.FootnotePosition == FootnotePosition.PrintImmediatelyBeneathText)
                destination.FootnotePosition = (byte)source.PageSetup.FootnotePosition;
            if (source.PageSetup.EndnoteNumberFormat != FootEndNoteNumberFormat.LowerCaseRoman)
                destination.EndnoteNumberFormat = (byte)source.PageSetup.EndnoteNumberFormat;
            if (source.PageSetup.RestartIndexForFootnotes != FootnoteRestartIndex.DoNotRestart)
                destination.RestartIndexForFootnotes = (byte)source.PageSetup.RestartIndexForFootnotes;
            if (source.PageSetup.RestartIndexForEndnote != EndnoteRestartIndex.DoNotRestart)
                destination.RestartIndexForEndnote = (byte)source.PageSetup.RestartIndexForEndnote;
            if (source.PageSetup.InitialFootnoteNumber != 1)
                destination.InitialFootnoteNumber = (ushort)source.PageSetup.InitialFootnoteNumber;
            if (source.PageSetup.InitialEndnoteNumber != 1)
                destination.InitialEndnoteNumber = (ushort)source.PageSetup.InitialEndnoteNumber;
            #endregion
            if (source.Columns.Count > 0)
            {
                //destination.Columns.DxaColumns = ( short )( source.PageSettings.SpacingBeforeColumns * DLSConstants.TwipsInOnePoint );
                Column column = null;
                for (int index = 0, counter = source.Columns.Count; index < counter; index++)
                {
                    column = source.Columns[index];
                    ColumnDescriptor colDesc = destination.Columns.AddColumn();
                    colDesc.Width = (ushort)Math.Round(column.Width * DLSConstants.TwipsInOnePoint);
                    colDesc.Space = (ushort)Math.Round(column.Space * DLSConstants.TwipsInOnePoint);
                }

                if (source.Columns.Count > 1 && !source.PageSetup.EqualColumnWidth)
                {
                    destination.Columns.ColumnsEvenlySpaced = false;
                }
            }
            else
            {
                destination.Sprms.SetValue(WordSprmOptions.sprmSDxaColumns, (short)720);
            }
            //Sort Sprms
            destination.Sprms.SortSprms();

            destination.DrawLinesBetweenCols = source.PageSetup.DrawLinesBetweenCols;
            destination.BreakCode = (byte)source.BreakCode;
            destination.ProtectForm = source.ProtectForm;
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="srcBorder"></param>
        /// <param name="destBorder"></param>
        private static void ExportBorder(BorderCode srcBorder, Border destBorder)
        {
            if (!srcBorder.IsDefault)
            {
                //        destBorder.Color = WordColor.ConvertIdToColor( srcBorder.LineColor );
                //destBorder.IsDefault = srcBorder.IsDefault;
                destBorder.Color = srcBorder.LineColorExt;
                destBorder.BorderType = (BorderStyle)srcBorder.BorderType;
                destBorder.LineWidth = (float)srcBorder.LineWidth / DLSConstants.BorderLineFactor;
                destBorder.Space = (float)srcBorder.Space;
                destBorder.Shadow = srcBorder.Shadow;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="destBorder"></param>
        /// <param name="srcBorder"></param>
        private static void ImportBorder(BorderCode destBorder, Border srcBorder)
        {
            if (!srcBorder.IsDefault)
            {
                destBorder.LineColor = (byte)WordColor.ConvertColorToId(srcBorder.Color);
                destBorder.LineColorExt = srcBorder.Color;
                destBorder.BorderType = (byte)srcBorder.BorderType;
                destBorder.LineWidth = (byte)Math.Round(srcBorder.LineWidth * DLSConstants.BorderLineFactor);
                destBorder.Space = (byte)Math.Round(srcBorder.Space);
                destBorder.Shadow = srcBorder.Shadow;
                //        destBorder.Update();
            }
            else
            {
                destBorder.LineColor = 0;
                destBorder.LineColorExt = Color.Empty;
                destBorder.BorderType = 0;
                destBorder.LineWidth = 0;
                destBorder.Space = 0;
                destBorder.Shadow = false;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="destBorders"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        private static Borders GetDestBorders(Borders destBorders, WPageSetup destination)
        {
            if (destBorders == null)
            {
                destBorders = destination.Borders;
            }
            return destBorders;
        }
        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    //[ CLSCompliant( false ) ]
    [DocumentationExclude()]
    internal class TablePropertiesConverter
    {
        #region Class utility methods
        /// <summary>
        /// Exports the specified source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        public static void Export(ParagraphProperties source, RowFormat destination)
        {
            destination.Sprms = source.Sprms;
            TableRowDescriptor rowDescriptor = destination.RowDescriptor;
            if (destination.OwnerRow != null && destination.OwnerRow.OwnerTable != null)
                rowDescriptor.Table = destination.OwnerRow.OwnerTable;
        }

        /// <summary>
        /// Imports the specified destination.
        /// </summary>
        /// <param name="destination">The destination.</param>
        /// <param name="source">The source.</param>
        public static void Import(ParagraphProperties destination, RowFormat source)
        {
            if (source.Sprms == null)
                return;
            //To Do - Need to remove this method and generalize WriteTableProps() method of DocWriterAdapter class.
            //Comment this code to avoid splitting the rows into multiple table.
            //source.Sprms.RemoveValue(WordSprmOptions.sprmPHugePapx3);
            source.Sprms.RemoveValue(WordSprmOptions.sprmPFInTable);
            source.Sprms.RemoveValue(WordSprmOptions.sprmTNestingLevel);
            source.Sprms.RemoveValue(WordSprmOptions.sprmPFTtp);

            destination.Sprms.Modifiers.AddRange(source.Sprms.Modifiers);

            //Sort Sprms
            destination.Sprms.SortSprms();

            //      foreach( SinglePropertyModifierRecord sprm in sprms )
            //      {
            //        destination.Sprms.Modifiers.Add( sprm );
            //      }
        }
        /// <summary>
        /// Exports the borders.
        /// </summary>
        /// <param name="srcBorders">The source borders.</param>
        /// <param name="destBorders">The destination borders.</param>
        internal static void ExportBorders(TableBorders srcBorders, Borders destBorders)
        {
            // Left
            BRCToBorder(srcBorders.LeftBorder, destBorders.Left);
            // Right
            BRCToBorder(srcBorders.RightBorder, destBorders.Right);
            // Top
            BRCToBorder(srcBorders.TopBorder, destBorders.Top);
            // Bottom
            BRCToBorder(srcBorders.BottomBorder, destBorders.Bottom);

            BRCToBorder(srcBorders.HorizontalBorder, destBorders.Horizontal);
            BRCToBorder(srcBorders.VerticalBorder, destBorders.Vertical);
        }
        /// <summary>
        /// Converts format to property.
        /// </summary>
        /// <param name="propertyKey">The property key.</param>
        /// <param name="rowFormat">The row format.</param>
        /// <param name="destination">The destination.</param>
        internal static void FormatToProp(int propertyKey, RowFormat rowFormat, TableRowDescriptor destination)
        {
            switch (propertyKey)
            {
                case RowFormat.CellSpacingKey:
                    float cs = rowFormat.CellSpacing;
                    if (cs >= 0)
                        destination.SpacingBetweenCells = (int)Math.Round(cs * DLSConstants.TwipsInOnePoint);
                    break;
                case RowFormat.LeftIndentKey:
                    destination.LeftIndent = (short)Math.Round(rowFormat.LeftIndent * DLSConstants.TwipsInOnePoint);
                    break;
                case RowFormat.IsAutoResizedCellsKey:
                    destination.IsAutoResized = rowFormat.IsAutoResized;
                    break;
                case RowFormat.HiddenKey:
                    destination.Hidden = rowFormat.Hidden;
                    break;
                case RowFormat.IsBreakAcrossPagesKey:
                    break;
                case RowFormat.RowAlignmentKey:
                    break;
                case RowFormat.IsHeaderRowKey:
                    destination.IsTableHeader = rowFormat.IsHeaderRow;
                    break;
                case RowFormat.BidiTableKey:
                    destination.Bidi = rowFormat.Bidi;
                    break;
                case RowFormat.ShadingColorKey:
                    if (destination.TableShading != null)
                        destination.TableShading.BackColor = rowFormat.BackColor;
                    else
                    {
                        for (int i = 0; i < destination.CellCount; i++)
                        {
                            if (destination.CellShadings[i].BackColor.IsEmpty)
                                destination.CellShadings[i].BackColor = rowFormat.BackColor;
                        }
                    }
                    break;
                case RowFormat.RowHeightKey:
                    destination.RowHeight = (short)Math.Round(rowFormat.Height * DLSConstants.TwipsInOnePoint);
                    break;
            }
        }
        /// <summary>
        /// Converts CellFormat to TableRowDescriptor properties.
        /// </summary>
        /// <param name="propertyKey">The property key.</param>
        /// <param name="cellFormat">The cell format.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="cellIndex">Index of the cell.</param>
        internal static void FormatToProp(int propertyKey, CellFormat cellFormat, TableRowDescriptor destination, int cellIndex)
        {
            WTableCell ownerCell = null;
            switch (propertyKey)
            {
                case CellFormat.VrAlignmentKey:
                    destination[cellIndex].VertAllign = (byte)cellFormat.VerticalAlignment;
                    break;
                case CellFormat.ShadingColorKey:
                    destination.CellShadings[cellIndex].BackColor = cellFormat.BackColor;
                    break;
                case CellFormat.VerticalMergeKey:
                    if (cellFormat.VerticalMerge == CellMerge.Start)
                    {
                        destination[cellIndex].VertRestart = true;
                        destination[cellIndex].VertMerge = true;
                    }
                    else if (cellFormat.VerticalMerge == CellMerge.Continue)
                    {
                        destination[cellIndex].VertMerge = true;
                    }
                    break;
                case CellFormat.HorizontalMergeKey:
                    if (cellFormat.HorizontalMerge == CellMerge.Start)
                    {
                        destination[cellIndex].FirstMerged = true;
                        destination[cellIndex].Merged = false;
                    }
                    else if (cellFormat.HorizontalMerge == CellMerge.Continue)
                    {
                        destination[cellIndex].Merged = true;
                    }
                    break;
                case CellFormat.FitTextKey:
                    destination[cellIndex].FitText = cellFormat.FitText;
                    break;
                case CellFormat.TextDirectionKey:
                    destination[cellIndex].TextDirection = cellFormat.TextDirection;
                    break;
                case CellFormat.BordersKey:
                    ImportCellBorders(destination[cellIndex], cellFormat.Borders);
                    break;
                case CellFormat.PaddingsKey:
                    ImportPaddings(destination.CellSpacings[cellIndex], cellFormat.Paddings);
                    break;
                case CellFormat.ForeColorKey:
                    ownerCell = cellFormat.OwnerBase as WTableCell;
                    if (ownerCell != null)
                    {
                        destination.CellShadings[cellIndex].ForeColor = ownerCell.ForeColor;
                    }
                    break;
                case CellFormat.TextureStyleKey:
                    ownerCell = cellFormat.OwnerBase as WTableCell;
                    if (ownerCell != null)
                    {
                        destination.CellShadings[cellIndex].Pattern = ownerCell.TextureStyle;
                    }
                    break;
                case CellFormat.PreferredWidthTypeKey:
                    destination[cellIndex].WidthUnit = (byte)cellFormat.PreferredWidth.WidthType;
                    break;
                case CellFormat.PreferredWidthKey:
                    if (cellFormat.PreferredWidth.WidthType == FtsWidth.Percentage)
                        destination[cellIndex].m_tableStruct.PreferredWidth = (ushort)Math.Round(cellFormat.PreferredWidth.Width * DLSConstants.PercentageFactor);
                    else if (cellFormat.PreferredWidth.WidthType == FtsWidth.Point)
                        destination[cellIndex].m_tableStruct.PreferredWidth = (ushort)Math.Round(cellFormat.PreferredWidth.Width * DLSConstants.TwipsInOnePoint);
                    else
                        destination[cellIndex].m_tableStruct.PreferredWidth = 0;
                    break;
                case CellFormat.CellWidthKey:
                    // To set cell width we need to set and array with "cell position grid".
                    // To do this we need the "position" of previous cell. To avoid additional
                    // complexity "cell grid position" array is set for every cell in table row. 
                    ownerCell = cellFormat.OwnerBase as WTableCell;
                    WTableRow row = ownerCell.OwnerRow;
                    for (int i = 0; i < destination.CellCount; i++)
                    {
                        destination.SetCellWidth(i, (short)Math.Round(row.Cells[i].Width * DLSConstants.TwipsInOnePoint));
                    }
                    break;
            }
        }
        /// <summary>
        /// Imports the width of the cells.
        /// </summary>
        /// <param name="rowDescr">The row descr.</param>
        /// <param name="row">The row.</param>
        /// <param name="section">The section.</param>
        public static void ImportCellsWidth(TableRowDescriptor rowDescr, WTableRow row,
          IWSection section)
        {
            short gridSpan = 0;
            bool hasJoiningCells = false;
            for (int i = 0; i < rowDescr.CellCount; i++)
            {
                float currCellWidth = row.Cells[i].Width;

                if (currCellWidth > 1638)
                {
                    gridSpan += 1;
                    rowDescr.SetCellWidth(i, (short)Math.Round(row.Cells[i].Width));
                }
                else if ((currCellWidth == 0||row.Cells[i].GridSpan > 1) && row.OwnerTable.TableGrid.Count != 0)
                {
                    short cWidth = 0;
                    if (row.Cells[i].GridSpan != 1)
                    {
                        if (row.OwnerTable.TableGrid.Count > row.Cells[i].GridSpan + gridSpan)
                        {
                            cWidth = (short)Math.Round(row.OwnerTable.TableGrid[row.Cells[i].GridSpan + gridSpan]
                                - row.OwnerTable.TableGrid[gridSpan]);
                            hasJoiningCells = true;
                        }
                    }
                    else if (hasJoiningCells)
                    {
                        if (row.OwnerTable.TableGrid.Count > gridSpan + 1)
                        {
                            cWidth = (short)Math.Round(row.OwnerTable.TableGrid[gridSpan + 1]
                                - row.OwnerTable.TableGrid[gridSpan]);
                        }
                    }
                    else
                    {
                        if (row.OwnerTable.TableGrid.Count > i + 1)
                        {
                            cWidth = (short)Math.Round(row.OwnerTable.TableGrid[i + 1]
                                - row.OwnerTable.TableGrid[i]);
                        }
                    }
                    gridSpan += row.Cells[i].GridSpan;
                    rowDescr.SetCellWidth(i, cWidth);
                }
                else
                {
                    gridSpan += 1;
                    rowDescr.SetCellWidth(i, (short)Math.Round(currCellWidth * DLSConstants.TwipsInOnePoint));
                }
            }
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="border"></param>
        /// <param name="brc"></param>
        private static void BorderToBRC(Border border, BorderCode brc)
        {
            if (border.BorderType == BorderStyle.Cleared)
            {
                border.Color = Color.Empty;
                border.LineWidth = 0;
            }
            else  if (border.BorderType == BorderStyle.None && !border.HasNoneStyle)
            {
                border.BorderType = BorderStyle.Single;
            }
            else if (border.BorderType == BorderStyle.Hairline)
            {
                border.BorderType = BorderStyle.Single;
            }

            if (!border.IsDefault)
            {
                if (border.BorderType == BorderStyle.Cleared)
                {
                    //Specifically handled to preserve cleared type border in doc format document.
                    brc.BorderType = 255;
                    brc.LineColor = 0;
                    brc.LineColorExt = Color.FromArgb(0, 255, 255, 255);
                    brc.LineWidth = 255;
                }
                else if (border.BorderType != BorderStyle.None)
                {
                    brc.BorderType = (byte)border.BorderType;
                    brc.LineColor = (byte)WordColor.ConvertColorToId(border.Color);
                    brc.LineColorExt = border.Color;
                    brc.LineWidth = (byte)(border.LineWidth * DLSConstants.BorderLineFactor);
                }
                brc.Shadow = border.Shadow;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="border"></param>
        /// <param name="brc"></param>
        private static void BorderToBRC(Border border, BorderStructure brc)
        {
            if (border.BorderType == BorderStyle.Hairline)
            {
                border.BorderType = BorderStyle.Single;
            }

            if (!border.IsDefault && border.BorderType == BorderStyle.Cleared)
            {
                brc.BorderType = 255;
                brc.LineColor = 255;
                brc.LineWidth = 255;
                brc.Props = 255;
            }
            else if (!border.IsDefault 
                && (border.BorderType != BorderStyle.None
                || border.HasNoneStyle))
            {
                //Props should be set as 255, if BorderType is 255.
                if (brc.BorderType == 255 && (byte)border.BorderType != 255)
                    brc.Props = 0;
                brc.BorderType = (byte)border.BorderType;
                brc.LineWidth = (byte)(border.LineWidth * DLSConstants.BorderLineFactor);
                brc.Shadow = border.Shadow;
                brc.LineColor = (byte)WordColor.ColorToId(border.Color);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="brc"></param>
        /// <param name="border"></param>
        internal static void BRCToBorder(BorderCode brc, Border border)
        {
            //      if( !source.LeftBorder.IsClear )
            //      border.Color = WordColor.IdToColor( brc.LineColor );
            //      border.Color = brc.LineColorExt;
            //      border.LineWidth = ( float )brc.LineWidth / DLSConstants.BorderLineFactor;
            //      border.BorderType = ( BorderStyle )brc.BorderType;
            //      border.Shadow = brc.Shadow;
            Color color = brc.LineColorExt;
            float lineWidth = (float)brc.LineWidth / (float)DLSConstants.BorderLineFactor;
            BorderStyle borderType = (BorderStyle)brc.BorderType;
            bool shadow = brc.Shadow;
            border.InitFormatting(color, lineWidth, borderType, shadow);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="brc"></param>
        /// <param name="border"></param>
        private static void BRCToBorder(BorderStructure brc, Border border)
        {
            if (IsEmpty(brc))
            {
                return;
            }

            if (!brc.IsClear)
            {
                Color color = WordColor.IdToColor(brc.LineColor);
                float lineWidth = (float)brc.LineWidth / (float)DLSConstants.BorderLineFactor;
                BorderStyle borderType = (BorderStyle)brc.BorderType;
                bool shadow = brc.Shadow;
                border.InitFormatting(color, lineWidth, borderType, shadow);
                if (border.BorderType == BorderStyle.None)
                    border.HasNoneStyle = true;
                //        border.Color = WordColor.IdToColor( brc.LineColor );
                //        border.LineWidth = ( float )brc.LineWidth / ( float )DLSConstants.BorderLineFactor;
                //        border.BorderType = ( BorderStyle )brc.BorderType;
                //        border.Shadow = brc.Shadow;
                //border.Space = ( byte )( brc.Space / DLSConstants.BorderLineFactor );
            }
            else
            {
                border.BorderType = BorderStyle.Cleared;
                border.HasNoneStyle = false;
            }
        }
        /// <summary>
        /// Determines whether the specified BRC is empty.
        /// </summary>
        /// <param name="brc">The BRC.</param>
        /// <returns>
        /// 	If the specified BRC is empty, set to <c>true</c>.
        /// </returns>
        private static bool IsEmpty(BorderStructure brc)
        {
            if (brc.LineColor == 0 && brc.LineWidth == 0 && brc.BorderType == 0)
                return true;

            return false;
        }
        /// <summary>
        /// Imports the paddings.
        /// </summary>
        /// <param name="destination">The destination.</param>
        /// <param name="source">The source.</param>
        public static void ImportPaddings(Spacings destination, Paddings source)
        {
            destination.Left = (short)Math.Round(source.Left * DLSConstants.TwipsInOnePoint);
            destination.Right = (short)Math.Round(source.Right * DLSConstants.TwipsInOnePoint);
            destination.Top = (short)Math.Round(source.Top * DLSConstants.TwipsInOnePoint);
            destination.Bottom = (short)Math.Round(source.Bottom * DLSConstants.TwipsInOnePoint);
        }
        /// <summary>
        /// Exports the paddings.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        public static void ExportPaddings(Spacings source, Paddings destination)
        {
            if (source != null)
            {
                destination.Left = (float)source.Left / DLSConstants.TwipsInOnePoint;
                destination.Right = (float)source.Right / DLSConstants.TwipsInOnePoint;
                destination.Top = (float)source.Top / DLSConstants.TwipsInOnePoint;
                destination.Bottom = (float)source.Bottom / DLSConstants.TwipsInOnePoint;
            }
            //      else
            //      {
            //        destination.Left = 0;
            //        destination.Right = 0;
            //        destination.Top = 0;
            //        destination.Bottom = 0;
            //      }
        }
        /// <summary>
        /// Exports the border colors.
        /// </summary>
        /// <param name="cellDescr">The cell descr.</param>
        /// <param name="destBorders">The destination borders.</param>
        private static void ExportCellBorderColors(TableCellDescriptor cellDescr, Borders destBorders)
        {
            if (cellDescr.BottomBorderColorExt != 0xff000000)
            {
                destBorders.Bottom.Color = WordColor.ConvertRGBToColor(cellDescr.BottomBorderColorExt);
            }
            if (cellDescr.LeftBorderColorExt != 0xff000000)
            {
                destBorders.Left.Color = WordColor.ConvertRGBToColor(cellDescr.LeftBorderColorExt);
            }
            if (cellDescr.TopBorderColorExt != 0xff000000)
            {
                destBorders.Top.Color = WordColor.ConvertRGBToColor(cellDescr.TopBorderColorExt);
            }
            if (cellDescr.RightBorderColorExt != 0xff000000)
            {
                destBorders.Right.Color = WordColor.ConvertRGBToColor(cellDescr.RightBorderColorExt);
            }
        }
        /// <summary>
        /// Exports the cell borders.
        /// </summary>
        /// <param name="cellDescr">The cell descr.</param>
        /// <param name="destBorders">The dest borders.</param>
        internal static void ExportCellBorders(TableCellDescriptor cellDescr, Borders destBorders)
        {
            // Left
            BRCToBorder(cellDescr.BRCLeft, destBorders.Left);
            // Top
            BRCToBorder(cellDescr.BRCTop, destBorders.Top);
            // Right
            BRCToBorder(cellDescr.BRCRight, destBorders.Right);
            // Bottom
            BRCToBorder(cellDescr.BRCBottom, destBorders.Bottom);
            // Export border colors.
            ExportCellBorderColors(cellDescr, destBorders);
        }
        /// <summary>
        /// Exports the type of the cell border.
        /// </summary>
        /// <param name="brTypeArray">The border type array.</param>
        /// <param name="destBorders">The destination borders.</param>
        /// <param name="startIndex">The start index.</param>
        internal static void ExportCellBorderType(byte[] brTypeArray, Borders destBorders, int startIndex)
        {
            if (startIndex < brTypeArray.Length)
            {
                destBorders.Top.BorderType = (BorderStyle)brTypeArray[startIndex];
                destBorders.Left.BorderType = (BorderStyle)brTypeArray[startIndex + 1];
                destBorders.Bottom.BorderType = (BorderStyle)brTypeArray[startIndex + 2];
                destBorders.Right.BorderType = (BorderStyle)brTypeArray[startIndex + 3];
            }
        }
        /// <summary>
        /// Imports the cell borders.
        /// </summary>
        /// <param name="cellDescr">The cell descr.</param>
        /// <param name="srcBorders">The SRC borders.</param>
        private static void ImportCellBorders(TableCellDescriptor cellDescr, Borders srcBorders)
        {
            /*if (srcBorders.Left.BorderType != BorderStyle.None
                && srcBorders.Right.BorderType != BorderStyle.None
                && srcBorders.Top.BorderType != BorderStyle.None
                && srcBorders.Bottom.BorderType != BorderStyle.None)
            {
                // Left
                BorderToBRC(srcBorders.Left, cellDescr.BRCLeft);
                // Right
                BorderToBRC(srcBorders.Right, cellDescr.BRCRight);
                // Top
                BorderToBRC(srcBorders.Top, cellDescr.BRCTop);
                // Bottom
                BorderToBRC(srcBorders.Bottom, cellDescr.BRCBottom);

                // Import colors of cell borders 
                ImportCellBorderColors(cellDescr, srcBorders);
            }*/
            //This case is handled for file validation issue - Border type thick is not support in Binary format, 
            //instead considered as Single type
            if (srcBorders.Left.BorderType == BorderStyle.Thick)
                srcBorders.Left.BorderType = BorderStyle.Single;
            if (srcBorders.Right.BorderType == BorderStyle.Thick)
                srcBorders.Right.BorderType = BorderStyle.Single;
            if (srcBorders.Top.BorderType == BorderStyle.Thick)
                srcBorders.Top.BorderType = BorderStyle.Single;
            if (srcBorders.Bottom.BorderType == BorderStyle.Thick)
                srcBorders.Bottom.BorderType = BorderStyle.Single;

            if (srcBorders.Left.BorderType != BorderStyle.None
                 || srcBorders.Right.BorderType != BorderStyle.None
                 || srcBorders.Top.BorderType != BorderStyle.None
                 || srcBorders.Bottom.BorderType != BorderStyle.None)
            {
                if (srcBorders.Left.BorderType != BorderStyle.None 
                    || srcBorders.Left.HasNoneStyle)
                    BorderToBRC(srcBorders.Left, cellDescr.BRCLeft);
                if (srcBorders.Right.BorderType != BorderStyle.None 
                    || srcBorders.Right.HasNoneStyle)
                    BorderToBRC(srcBorders.Right, cellDescr.BRCRight);
                if (srcBorders.Top.BorderType != BorderStyle.None 
                    || srcBorders.Top.HasNoneStyle)
                    BorderToBRC(srcBorders.Top, cellDescr.BRCTop);
                if (srcBorders.Bottom.BorderType != BorderStyle.None 
                    || srcBorders.Bottom.HasNoneStyle)
                    BorderToBRC(srcBorders.Bottom, cellDescr.BRCBottom);

                ImportCellBorderColors(cellDescr, srcBorders);
            }
            else
            {
                if (srcBorders.Left.HasNoneStyle && srcBorders.Left.BorderType == BorderStyle.None 
                    && !srcBorders.Left.Color.IsEmpty && srcBorders.Left.Color != Color.Black)
                {
                    cellDescr.LeftBorderColorExt = WordColor.ConvertColorToRGB(Color.White);
                }
                if (srcBorders.Right.HasNoneStyle && srcBorders.Right.BorderType == BorderStyle.None 
                    && !srcBorders.Right.Color.IsEmpty && srcBorders.Right.Color != Color.Black)
                {
                    cellDescr.RightBorderColorExt = WordColor.ConvertColorToRGB(Color.White);
                }
                if (srcBorders.Top.HasNoneStyle && srcBorders.Top.BorderType == BorderStyle.None 
                    && !srcBorders.Top.Color.IsEmpty && srcBorders.Top.Color != Color.Black)
                {
                    cellDescr.TopBorderColorExt = WordColor.ConvertColorToRGB(Color.White);
                }              
                if (srcBorders.Bottom.HasNoneStyle && srcBorders.Bottom.BorderType == BorderStyle.None 
                    && !srcBorders.Bottom.Color.IsEmpty && srcBorders.Bottom.Color != Color.Black)
                {
                    cellDescr.BottomBorderColorExt = WordColor.ConvertColorToRGB(Color.White);
                }
            }
        }
        /// <summary>
        /// Imports the color of the cell border.
        /// </summary>
        /// <param name="cellDescr">The cell descr.</param>
        /// <param name="srcBorders">The source borders.</param>
        private static void ImportCellBorderColors(TableCellDescriptor cellDescr, Borders srcBorders)
        {
            if (srcBorders.Bottom.HasKey(Border.ColorKey))
            {
                cellDescr.BottomBorderColorExt = WordColor.ConvertColorToRGB(srcBorders.Bottom.Color);
            }
            if (srcBorders.Left.HasKey(Border.ColorKey))
            {
                cellDescr.LeftBorderColorExt = WordColor.ConvertColorToRGB(srcBorders.Left.Color);
            }
            if (srcBorders.Top.HasKey(Border.ColorKey))
            {
                cellDescr.TopBorderColorExt = WordColor.ConvertColorToRGB(srcBorders.Top.Color);
            }
            if (srcBorders.Right.HasKey(Border.ColorKey))
            {
                cellDescr.RightBorderColorExt = WordColor.ConvertColorToRGB(srcBorders.Right.Color);
            }
        }

        /// <summary>
        /// Imports the cell spacing.
        /// </summary>
        /// <param name="cellSpacing">The cell spacing.</param>
        /// <param name="sprms">The SPRMS.</param>
        private static void ImportCellSpacing(float cellSpacing, SinglePropertyModifierArray sprms)
        {
            int spacingBetweenCells = (int)Math.Round(cellSpacing * DLSConstants.TwipsInOnePoint);
            if (spacingBetweenCells > 0)
            {
                byte[] operand = new byte[6] { 0, 1, 15, 3, 0, 0 };
                byte[] spacing = BitConverter.GetBytes((ushort)spacingBetweenCells);
                spacing.CopyTo(operand, 4);
                sprms.SetValue(WordSprmOptions.sprmTCellSpacing, operand);
            }
        }
        /// <summary>
        /// Imports the left indent.
        /// </summary>
        /// <param name="leftIndent">The left indent.</param>
        /// <param name="sprms">The SPRMS.</param>
        private static void ImportLeftIndent(float leftIndent, SinglePropertyModifierArray sprms)
        {
            short rowLeftIndent = (short)Math.Round(leftIndent * DLSConstants.TwipsInOnePoint);
            TableRowDescriptor rowDesc = new TableRowDescriptor();
            rowDesc.GetTableCellProperties(sprms);
            rowDesc.LeftIndent = rowLeftIndent;
            rowDesc.SetTableCellProperties(sprms);
        }
        /// <summary>
        /// Imports the height of the row.
        /// </summary>
        /// <param name="rowFormat">The row format.</param>
        /// <param name="sprms">The sprms.</param>
        private static void ImportRowHeight(RowFormat rowFormat, SinglePropertyModifierArray sprms)
        {
            WTableRow ownerRow = rowFormat.OwnerBase as WTableRow;
            short heightValue = (short)Math.Round(ownerRow.Height * DLSConstants.TwipsInOnePoint);
            heightValue = (short)((ownerRow.HeightType == TableRowHeightType.AtLeast) ?
            heightValue : (~heightValue));
            sprms.SetValue(WordSprmOptions.sprmTDyaRowHeight, heightValue);
        }
        /// <summary>
        /// Gets the SPRM.
        /// </summary>
        /// <param name="option">The option.</param>
        /// <param name="sprms">The SPRMS.</param>
        /// <returns></returns>
        private static SinglePropertyModifierRecord GetSprm(int option, SinglePropertyModifierArray sprms)
        {
            SinglePropertyModifierRecord sprm = sprms[option];
            if (sprm == null)
            {
                sprm = new SinglePropertyModifierRecord(option);
                sprms.Add(sprm);
            }

            return sprm;
        }
        /// <summary>
        /// Imports the positioning.
        /// </summary>
        /// <param name="destination">The destination.</param>
        /// <param name="source">The source.</param>
        internal static void ImportPositioning(TableRowDescriptor destination, RowFormat.TablePositioning source)
        {
            if (!source.m_ownerRowFormat.WrapTextAround)
                return;

            destination.DistanceFromTop = (short)(source.DistanceFromTop * DLSConstants.TwipsInOnePoint);
            destination.DistanceFromBottom = (short)(source.DistanceFromBottom * DLSConstants.TwipsInOnePoint);
            destination.DistanceFromLeft = (short)(source.DistanceFromLeft * DLSConstants.TwipsInOnePoint);
            destination.DistanceFromRight = (short)(source.DistanceFromRight * DLSConstants.TwipsInOnePoint);

            if (source.HorizPositionAbs != HorizontalPosition.Left)
                destination.HorizPosition = (short)source.HorizPositionAbs;
            else
                destination.HorizPosition = (short)(source.HorizPosition * DLSConstants.TwipsInOnePoint);
            if (source.VertPositionAbs != VerticalPosition.None)
                destination.VertPosition = (short)source.VertPositionAbs;
            else
                destination.VertPosition = (short)(source.VertPosition * DLSConstants.TwipsInOnePoint);

            destination.HorizRelationTo = source.HorizRelationTo;
            destination.VertRelationTo = source.VertRelationTo;

            destination.AllowOverlap = source.AllowOverlap;
        }
        /// <summary>
        /// Exports the positioning.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        internal static void ExportPositioning(TableRowDescriptor source, RowFormat.TablePositioning destination)
        {
            if (!source.WrapTextAround)
                return;

            destination.DistanceFromTop = (float)source.DistanceFromTop / DLSConstants.TwipsInOnePoint;
            destination.DistanceFromBottom = (float)source.DistanceFromBottom / DLSConstants.TwipsInOnePoint;
            destination.DistanceFromLeft = (float)source.DistanceFromLeft / DLSConstants.TwipsInOnePoint;
            destination.DistanceFromRight = (float)source.DistanceFromRight / DLSConstants.TwipsInOnePoint;
            if (source.HorizPosition == (float)HorizontalPosition.Center
                || source.HorizPosition == (float)HorizontalPosition.Right
                || source.HorizPosition == (float)HorizontalPosition.Inside
                || source.HorizPosition == (float)HorizontalPosition.Outside)
                destination.HorizPosition = (float)source.HorizPosition;
            else
                destination.HorizPosition = (float)source.HorizPosition / DLSConstants.TwipsInOnePoint;
            if (source.VertPosition == (float)VerticalPosition.Top
                || source.VertPosition == (float)VerticalPosition.Center
                || source.VertPosition == (float)VerticalPosition.Bottom
                || source.VertPosition == (float)VerticalPosition.Inside
                || source.VertPosition == (float)VerticalPosition.Outside)
                destination.VertPosition = (float)source.VertPosition;
            else
                destination.VertPosition = (float)source.VertPosition / DLSConstants.TwipsInOnePoint;

            destination.HorizRelationTo = source.HorizRelationTo;
            destination.VertRelationTo = source.VertRelationTo;

            destination.AllowOverlap = source.AllowOverlap;
        }
        #endregion

        #region Full export code
        /// <summary>
        /// Exports formatting from SectionProperties to ISection's PageSettings
        /// </summary>
        /// <param name="sourceDescriptor">The source descriptor.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="parseAll">if it is parse all, set to <c>true</c>.</param>
        public static void Export(TableRowDescriptor sourceDescriptor, WTableRow destination, bool parseAll)
        {
            //      TableRowDescriptor sourceDescriptor = paragraphProperties.TableRowProperties;      
            for (int i = 0; i < sourceDescriptor.CellCount && destination.Cells.Count > 0; i++)
            {
                if (i > destination.Cells.Count - 1)
                {
                    string errMess = string.Format("Cells count in sourceDescriptor {0} is greater than destanation {1}",
                      sourceDescriptor.CellCount, destination.Cells.Count);
                    throw new DLSException(errMess);
                }

                WTableCell cell = destination.Cells[i];
                CellFormat format = cell.CellFormat;
                Borders destBorders = format.Borders;
                TableCellDescriptor cellDescr = sourceDescriptor[i];
                //Border vertBorder = destination.TableFormat.VerticalBorder;
                //        destination.Cells[ i ].CellFormat.BackColor = WordColor.IdToColor( sourceDescriptor.CellShadings[ i ].BackColor );
                if (sourceDescriptor.CellShadings[i].BackColor != Color.Empty)
                {
                    format.BackColor = sourceDescriptor.CellShadings[i].BackColor;
                }
                cell.ForeColor = sourceDescriptor.CellShadings[i].ForeColor;
                cell.TextureStyle = sourceDescriptor.CellShadings[i].Pattern;

                format.VerticalAlignment = (VerticalAlignment)cellDescr.VertAllign;

                ExportCellBorders(cellDescr, destBorders);

                Spacings spacings = sourceDescriptor.CellSpacings[i];

                if (spacings != null)
                {
                    ExportPaddings(spacings, format.Paddings);
                    format.SamePaddingsAsTable = false;
                }

                if (cellDescr.VertMerge)
                {
                    format.VerticalMerge = CellMerge.Continue;
                }
                if (cellDescr.VertRestart)
                {
                    format.VerticalMerge = CellMerge.Start;
                }
                if (cellDescr.Merged)
                {
                    format.HorizontalMerge = CellMerge.Continue;
                }
                if (cellDescr.FirstMerged)
                {
                    format.HorizontalMerge = CellMerge.Start;
                }

                WTableCell destCell = cell;

                destCell.CellFormat.TextDirection = cellDescr.TextDirection;
                destCell.CellFormat.FitText = cellDescr.FitText;
            }

            ExportPositioning(sourceDescriptor, destination.RowFormat.Positioning);

            ExportPaddings(sourceDescriptor.TableSpacings, destination.RowFormat.Paddings);
            destination.RowFormat.LeftIndent = (float)sourceDescriptor.LeftIndent / DLSConstants.TwipsInOnePoint;

            if (sourceDescriptor.TableShading != null)
            {
                destination.RowFormat.BackColor = sourceDescriptor.TableShading.BackColor;
                destination.RowFormat.TextureStyle = sourceDescriptor.TableShading.Pattern;
            }

            if (sourceDescriptor.RowHeight != 0)
            {
                destination.HeightType = (sourceDescriptor.RowHeight > 0) ?
                  TableRowHeightType.AtLeast : TableRowHeightType.Exactly;
                destination.Height = (float)Math.Abs(sourceDescriptor.RowHeight) / DLSConstants.TwipsInOnePoint;
            }

            destination.IsHeader = sourceDescriptor.IsTableHeader;
            //      destination.OwnerTable.TableFormat.SpacingBetweenCells = ( float ) sourceDescriptor.SpacingBetweenCells /
            //                                                               DLSConstants.TwipsInOnePoint;
            float sbc = (float)sourceDescriptor.SpacingBetweenCells / DLSConstants.TwipsInOnePoint;
            if (sbc >= 0)
                destination.RowFormat.CellSpacing = sbc;

            destination.RowFormat.IsAutoResized = sourceDescriptor.IsAutoResized;
            destination.RowFormat.Bidi = sourceDescriptor.Bidi;
        }
        /// <summary>
        /// Exports formatting
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="tableFormat">The table format.</param>
        /// <param name="destRow">The dest row.</param>
        public static void Export(TableBorders source, RowFormat tableFormat, WTableRow destRow)
        {
            // Left
            Borders destBorders = tableFormat.Borders;

            BRCToBorder(source.LeftBorder, destBorders.Left);
            // Right
            BRCToBorder(source.RightBorder, destBorders.Right);
            // Top
            BRCToBorder(source.TopBorder, destBorders.Top);
            // Bottom
            BRCToBorder(source.BottomBorder, destBorders.Bottom);

            BRCToBorder(source.HorizontalBorder, destBorders.Horizontal);
            BRCToBorder(source.VerticalBorder, destBorders.Vertical);
        }
        /// <summary>
        /// Exports the width of the cells.
        /// </summary>
        /// <param name="rowDescr">The row descr.</param>
        /// <param name="row">The row.</param>
        public static void ExportCellsWidth(TableRowDescriptor rowDescr, WTableRow row)
        {
            for (int i = 0; i < row.Cells.Count && i < rowDescr.CellCount; i++)
            {
                row.Cells[i].Width = (float)rowDescr.GetCellWidth(i)
                  / DLSConstants.TwipsInOnePoint;
            }
        }
        /// <summary>
        /// Imports formatting from WTableRow to TableRowDescriptor
        /// </summary>
        /// <param name="destination">The destination.</param>
        /// <param name="source">The source.</param>
        public static void Import(TableRowDescriptor destination, WTableRow source)
        {
            for (int i = 0; i < source.Cells.Count; i++)
            {
                WTableCell cell = source.Cells[i];
                CellFormat format = cell.CellFormat;
                Borders srcBorders = format.Borders;
                TableCellDescriptor cellDescr = destination[i];
                //        destination.CellShadings[ i ].BackColor = ( byte )WordColor.ColorToId( source.Cells[i].CellFormat.BackColor );
                if (format.HasKey(4))
                {
                    destination.CellShadings[i].BackColor = format.BackColor;
                    destination.CellShadings[i].ForeColor = cell.ForeColor;
                    destination.CellShadings[i].Pattern = cell.TextureStyle;
                }
                else if (source.RowFormat.HasKey(RowFormat.ShadingColorKey))
                {
                    RowFormat rf = source.RowFormat;
                    destination.CellShadings[i].BackColor = rf.BackColor;
                    destination.CellShadings[i].ForeColor = rf.ForeColor;
                    destination.CellShadings[i].Pattern = rf.TextureStyle;
                }
                else if (source.OwnerTable.TableFormat.HasKey(RowFormat.ShadingColorKey))
                {
                    RowFormat rf = source.OwnerTable.TableFormat;
                    destination.CellShadings[i].BackColor = rf.BackColor;
                    destination.CellShadings[i].ForeColor = rf.ForeColor;
                    destination.CellShadings[i].Pattern = rf.TextureStyle;
                }
                cellDescr.VertAllign = (byte)format.VerticalAlignment;
                if (!format.SamePaddingsAsTable)
                {
                    ImportPaddings(destination.CellSpacings[i], format.Paddings);
                }

                ImportCellBorders(cellDescr, srcBorders);

                if (format.VerticalMerge == CellMerge.Start)
                {
                    destination[i].VertRestart = true;
                    destination[i].VertMerge = true;
                }
                else if (format.VerticalMerge == CellMerge.Continue)
                {
                    destination[i].VertMerge = true;
                }

                if (format.HorizontalMerge == CellMerge.Start)
                {
                    destination[i].FirstMerged = true;
                    destination[i].Merged = false;
                }
                else if (format.HorizontalMerge == CellMerge.Continue)
                {
                    destination[i].Merged = true;
                }

                if (cell.CellFormat.TextDirection != TextDirection.Horizontal)
                {
                    cellDescr.TextDirection = cell.CellFormat.TextDirection;
                }

                if (cell.CellFormat.FitText)
                {
                    cellDescr.FitText = cell.CellFormat.FitText;
                }

                cellDescr.WidthUnit = (byte)cell.PreferredWidth.WidthType;
                if (cell.PreferredWidth.WidthType == FtsWidth.Percentage)
                    cellDescr.m_tableStruct.PreferredWidth = (ushort)Math.Round(cell.PreferredWidth.Width * DLSConstants.PercentageFactor);
                else if (cell.PreferredWidth.WidthType == FtsWidth.Point)
                    cellDescr.m_tableStruct.PreferredWidth = (ushort)Math.Round(cell.PreferredWidth.Width * DLSConstants.TwipsInOnePoint);
            }
            //Updates table shading.
            destination.TableShading.BackColor = source.RowFormat.BackColor;
            destination.TableShading.ForeColor = source.RowFormat.ForeColor;
            destination.TableShading.Pattern = source.RowFormat.TextureStyle;
            if (source.Height > 0)
            {
                short heightValue = (short)Math.Round(source.Height * DLSConstants.TwipsInOnePoint);
                destination.RowHeight = (short)((source.HeightType == TableRowHeightType.AtLeast) ? heightValue : (~heightValue));
            }

            if (source.RowFormat.LeftIndent != 0)
                destination.LeftIndent = (short)Math.Round(source.RowFormat.LeftIndent * DLSConstants.TwipsInOnePoint);

            FtsWidth widthType = source.OwnerTable.PreferredTableWidth.WidthType;
            float width = source.OwnerTable.PreferredTableWidth.Width;
            if ((int)widthType >= 2)
            {
                if (width > 0)
                {
                    destination.WidthType = source.OwnerTable.PreferredTableWidth.WidthType;
                    if (widthType == FtsWidth.Percentage)
                        destination.TableWidth = (short)Math.Round(width * DLSConstants.PercentageFactor);
                    else if (widthType == FtsWidth.Point)
                        destination.TableWidth = (short)Math.Round(width * DLSConstants.TwipsInOnePoint);
                }
            }
            else
                destination.WidthType = widthType;
            ImportPaddings(destination.TableSpacings, source.RowFormat.Paddings);

            ImportPositioning(destination, source.RowFormat.Positioning);

            destination.IsTableHeader = source.IsHeader;

            float cs = source.RowFormat.CellSpacing;
            if (cs >= 0)
                destination.SpacingBetweenCells = (int)Math.Round(cs * DLSConstants.TwipsInOnePoint);
            destination.IsAutoResized = source.RowFormat.IsAutoResized;
            destination.Bidi = source.RowFormat.Bidi;
        }
        /// <summary>
        /// Imports formatting
        /// </summary>
        /// <param name="destination">The destination.</param>
        /// <param name="srcTable">The SRC table.</param>
        public static void Import(TableBorders destination, RowFormat srcTable)
        {
            Borders srcBorders = srcTable.Borders;

            // Left border
            Border leftBorder = (srcBorders.Left.BorderType != BorderStyle.None)
              ? srcBorders.Left
              : srcTable.Borders.Left;
            BorderToBRC(leftBorder, destination.LeftBorder);

            // Right border
            Border rightBorder = (srcBorders.Right.BorderType != BorderStyle.None)
              ? srcBorders.Right
              : srcTable.Borders.Right;
            BorderToBRC(rightBorder, destination.RightBorder);

            // Top border
            Border topBorder = (srcBorders.Top.BorderType != BorderStyle.None)
              ? srcBorders.Top
              : srcTable.Borders.Top;
            BorderToBRC(topBorder, destination.TopBorder);

            // Bottom border
            Border bottomBorder = (srcBorders.Bottom.BorderType != BorderStyle.None)
              ? srcBorders.Bottom
              : srcTable.Borders.Bottom;
            BorderToBRC(bottomBorder, destination.BottomBorder);

            // Horizontal border
            BorderToBRC(srcTable.Borders.Horizontal, destination.HorizontalBorder);

            // Vertical border
            BorderToBRC(srcTable.Borders.Vertical, destination.VerticalBorder);
        }
        #endregion
    }
    /// <summary>
    /// Property convertor for textboxes
    /// </summary>
    //[ CLSCompliant( false ) ]
    [DocumentationExclude()]
    internal class TextBoxPropertiesConverter
    {
        #region Class utility methods
        /// <summary>
        /// Export properties from TextBoxProps to TextBoxFormat.
        /// </summary>
        /// <param name="txbxProps">TextBoxProps item.</param>
        /// <param name="txbxFormat">TextBoxFormat item.</param>
        public static void Export(TextBoxProps txbxProps, WTextBoxFormat txbxFormat)
        {
            txbxFormat.HorizontalPosition = (float)txbxProps.XaLeft / DLSConstants.TwipsInOnePoint;
            txbxFormat.VerticalPosition = (float)txbxProps.YaTop / DLSConstants.TwipsInOnePoint;
            txbxFormat.Width = (float)txbxProps.Width / DLSConstants.TwipsInOnePoint;
            txbxFormat.Height = (float)txbxProps.Height / DLSConstants.TwipsInOnePoint;

            if (txbxProps.LeftMargin != uint.MaxValue)
                txbxFormat.InternalMargin.Left = (float)txbxProps.LeftMargin / (DLSConstants.EmusPerPoint);
            if (txbxProps.RightMargin != uint.MaxValue)
                txbxFormat.InternalMargin.Right = (float)txbxProps.RightMargin / (DLSConstants.EmusPerPoint);
            if (txbxProps.TopMargin != uint.MaxValue)
                txbxFormat.InternalMargin.Top = (float)txbxProps.TopMargin / (DLSConstants.EmusPerPoint);
            if (txbxProps.BottomMargin != uint.MaxValue)
                txbxFormat.InternalMargin.Bottom = (float)txbxProps.BottomMargin / (DLSConstants.EmusPerPoint);

            txbxFormat.HorizontalAlignment = txbxProps.HorizontalAlignment;
            txbxFormat.VerticalAlignment = txbxProps.VerticalAlignment;
            txbxFormat.HorizontalOrigin = txbxProps.RelHrzPos;
            txbxFormat.VerticalOrigin = txbxProps.RelVrtPos;

            txbxFormat.FillColor = txbxProps.FillColor;
            txbxFormat.LineColor = txbxProps.LineColor;
            txbxFormat.LineDashing = txbxProps.LineDashing;
            txbxFormat.LineStyle = txbxProps.LineStyle;
            txbxFormat.LineWidth = txbxProps.TxbxLineWidth;
            txbxFormat.NoLine = txbxProps.NoLine;

            txbxFormat.TextWrappingStyle = txbxProps.TextWrappingStyle;
            txbxFormat.TextWrappingType = txbxProps.TextWrappingType;
            txbxFormat.WrappingMode = txbxProps.WrapText;
            txbxFormat.IsBelowText = txbxProps.IsBelowText;

            txbxFormat.TextBoxIdentificator = txbxProps.TXID;
            txbxFormat.IsHeaderTextBox = txbxProps.IsHeaderShape;
            txbxFormat.TextBoxShapeID = txbxProps.Spid;
        }

        /// <summary>
        /// Import properties from TetxBoxFormat to TextBoxProps.
        /// </summary>
        /// <param name="txbxFormat">The TXBX format.</param>
        /// <param name="txbxProps">The TXBX props.</param>
        public static void Import(WTextBoxFormat txbxFormat, TextBoxProps txbxProps)
        {
            txbxProps.XaLeft = (int)Math.Round(txbxFormat.HorizontalPosition * DLSConstants.TwipsInOnePoint);
            txbxProps.YaTop = (int)Math.Round(txbxFormat.VerticalPosition * DLSConstants.TwipsInOnePoint);
            txbxProps.Width = (int)Math.Round(txbxFormat.Width * DLSConstants.TwipsInOnePoint);
            txbxProps.Height = (int)Math.Round(txbxFormat.Height * DLSConstants.TwipsInOnePoint);
            if (txbxFormat.HorizontalOrigin == HorizontalOrigin.LeftMargin || txbxFormat.HorizontalOrigin == HorizontalOrigin.RightMargin
                || txbxFormat.HorizontalOrigin == HorizontalOrigin.InsideMargin || txbxFormat.HorizontalOrigin == HorizontalOrigin.OutsideMargin)
                txbxProps.RelHrzPos = HorizontalOrigin.Margin;
            else
                txbxProps.RelHrzPos = txbxFormat.HorizontalOrigin;
            if (txbxFormat.VerticalOrigin == VerticalOrigin.TopMargin || txbxFormat.VerticalOrigin == VerticalOrigin.BottomMargin 
                || txbxFormat.VerticalOrigin == VerticalOrigin.InsideMargin || txbxFormat.VerticalOrigin == VerticalOrigin.OutsideMargin)
                txbxProps.RelVrtPos = VerticalOrigin.Page;
            else
                txbxProps.RelVrtPos = txbxFormat.VerticalOrigin;
            txbxProps.HorizontalAlignment = txbxFormat.HorizontalAlignment;
            txbxProps.VerticalAlignment = txbxFormat.VerticalAlignment;

            if (txbxFormat.InternalMargin.Left != InternalMargin.DEF_HORIZMARGIN)
                txbxProps.LeftMargin = (uint)Math.Round(txbxFormat.InternalMargin.Left * DLSConstants.EmusPerPoint);
            if (txbxFormat.InternalMargin.Right != InternalMargin.DEF_HORIZMARGIN)
                txbxProps.RightMargin = (uint)Math.Round(txbxFormat.InternalMargin.Right * DLSConstants.EmusPerPoint);
            if (txbxFormat.InternalMargin.Top != InternalMargin.DEF_VERTMARGIN)
                txbxProps.TopMargin = (uint)Math.Round(txbxFormat.InternalMargin.Top * DLSConstants.EmusPerPoint);
            if (txbxFormat.InternalMargin.Bottom != InternalMargin.DEF_VERTMARGIN)
                txbxProps.BottomMargin = (uint)Math.Round(txbxFormat.InternalMargin.Bottom * DLSConstants.EmusPerPoint);

            txbxProps.FillColor = txbxFormat.FillColor;
            txbxProps.LineColor = txbxFormat.LineColor;
            txbxProps.LineDashing = txbxFormat.LineDashing;
            txbxProps.LineStyle = txbxFormat.LineStyle;
            txbxProps.TxbxLineWidth = txbxFormat.LineWidth;
            txbxProps.NoLine = txbxFormat.NoLine;

            txbxProps.TextWrappingStyle = txbxFormat.TextWrappingStyle;
            txbxProps.TextWrappingType = txbxFormat.TextWrappingType;

            txbxProps.WrapText = txbxFormat.WrappingMode;
            txbxProps.IsBelowText = txbxFormat.IsBelowText;

            txbxProps.Spid = (int)txbxFormat.TextBoxShapeID;
            txbxProps.TXID = txbxFormat.TextBoxIdentificator;
        }
        /// <summary>
        /// Exports data from textbxo container to the specified WTextboxFormat object.
        /// </summary>
        /// <param name="txbxContainer">The textbox container.</param>
        /// <param name="fspa">The fspa.</param>
        /// <param name="txbxFormat">The textbox format.</param>
        /// <param name="skipPositionOrigins">The skip position origins.</param>
        internal static void Export(MsofbtSpContainer txbxContainer, FileShapeAddress fspa, WTextBoxFormat txbxFormat, bool skipPositionOrigins)
        {
            txbxFormat.HorizontalPosition = (float)fspa.XaLeft / DLSConstants.TwipsInOnePoint;
            txbxFormat.VerticalPosition = (float)fspa.YaTop / DLSConstants.TwipsInOnePoint;
            if (!skipPositionOrigins)
            {
                txbxFormat.HorizontalOrigin = fspa.RelHrzPos;
                txbxFormat.VerticalOrigin = fspa.RelVrtPos;
            }

            txbxFormat.Width = (float)fspa.Width / DLSConstants.TwipsInOnePoint;
            txbxFormat.Height = (float)fspa.Height / DLSConstants.TwipsInOnePoint;

            txbxFormat.TextWrappingStyle = fspa.TextWrappingStyle;
            txbxFormat.TextWrappingType = fspa.TextWrappingType;
            txbxFormat.IsHeaderTextBox = fspa.IsHeaderShape;
            txbxFormat.TextBoxShapeID = fspa.Spid;
            // read Wrap Polygon vertices.
            if (txbxFormat.TextWrappingStyle == TextWrappingStyle.Tight || txbxFormat.TextWrappingStyle == TextWrappingStyle.Through)
            {
                if (txbxContainer.ShapeOptions.Properties.Contains((int)FOPTEGroupShape.pWrapPolygonVertices))
                {
                    txbxFormat.WrapPolygon = new WrapPolygon();
                    txbxFormat.WrapPolygon.Edited = false;
                    for (int i = 0; i < txbxContainer.ShapeOptions.WrapPolygonVertices.Coords.Count; i++)
                        txbxFormat.WrapPolygon.Vertices.Add(txbxContainer.ShapeOptions.WrapPolygonVertices.Coords[i]);
                }
            }
            //Get LayoutInCell
            if (txbxContainer.ShapePosition != null && txbxContainer.ShapePosition.Properties.ContainsKey((int)FOPTEGroupShape.fPrint))
                txbxFormat.AllowInCell = txbxContainer.ShapePosition.AllowInTableCell;
            else if (txbxContainer.ShapeOptions != null && txbxContainer.ShapeOptions.Properties.ContainsKey((int)FOPTEGroupShape.fPrint))
                txbxFormat.AllowInCell = txbxContainer.ShapeOptions.AllowInTableCell;

            txbxFormat.UpdateFillEffects(txbxContainer, txbxFormat.Document);

            //Get textbox line width value
            uint prop = txbxContainer.GetPropertyValue((int)FOPTELineStyle.lineWidth);
            if (prop != uint.MaxValue)
            {
                txbxFormat.LineWidth = (float)prop / msofbtRGFOPTE.DEF_LINE_WIDTH_PT;
            }
            //Get textbox line style property
            prop = txbxContainer.GetPropertyValue((int)FOPTELineStyle.lineStyle);
            if (prop != uint.MaxValue)
            {
                txbxFormat.LineStyle = (TextBoxLineStyle)prop;
            }
            //Get textbox line dashing property
            prop = txbxContainer.GetPropertyValue((int)FOPTELineStyle.lineDashing);
            if (prop != uint.MaxValue)
            {
                txbxFormat.LineDashing = (LineDashing)prop;
            }
            //Get textbox wrap text property
            prop = txbxContainer.GetPropertyValue((int)FOPTEText.WrapText);
            if (prop != uint.MaxValue)
            {
                txbxFormat.WrappingMode = (WrapMode)prop;
            }
            //Get textbox text direction
            prop = txbxContainer.GetPropertyValue((int)FOPTEText.txflTextFlow);
            if (prop != uint.MaxValue)
            {
                txbxFormat.TextDirection = (TextDirection)prop;
            }
            //Get textbox fill color property
            prop = txbxContainer.GetPropertyValue((int)FOPTEFillStyle.fillColor);
            if (prop != uint.MaxValue && txbxFormat.FillEfects.Type == BackgroundType.NoBackground)
            {
                txbxFormat.FillColor = WordColor.ConvertRGBToColor(prop);
            }
            //Get line color
            prop = txbxContainer.GetPropertyValue((int)FOPTELineStyle.lineColor);
            if (prop != uint.MaxValue)
            {
                txbxFormat.LineColor = WordColor.ConvertRGBToColor(prop);
            }

            //Has fill color?
            prop = txbxContainer.GetPropertyValue((int)FOPTEFillStyle.fNoFillHitTest);
            bool hasFill = ((prop & 0x10) == 16);
            if (!hasFill)
            {
                txbxFormat.FillColor = Color.Empty;
            }
            //Get NoLine property
            prop = txbxContainer.GetPropertyValue((int)FOPTELineStyle.lineStyleBooleanProperties);
            if (prop != uint.MaxValue)
            {
                txbxFormat.NoLine = ((prop & 0x08) == 0);
            }
            //Get textbox TxId  property
            txbxFormat.TextBoxIdentificator = txbxContainer.GetPropertyValue((int)MsofbtOPT.DEF_TXID);

            prop = txbxContainer.GetPropertyValue((int)FOPTEGroupShape.fPrint);
            if (prop != uint.MaxValue)
                txbxFormat.IsBelowText = ((prop & 0x20) == 32);
            else
                txbxFormat.IsBelowText = false;

            // Export shape position properties
            if (txbxContainer.ShapePosition != null)
            {
                ExportPosition(txbxContainer, txbxFormat);
            }

            ExportIntMargin(txbxContainer, txbxFormat);
        }
        /// <summary>
        /// Imports data from WTextboxFormat to FileShapeAddress.
        /// </summary>
        /// <param name="fspa">The fspa.</param>
        /// <param name="txbxFormat">The textbox format.</param>
        internal static void Import(FileShapeAddress fspa, WTextBoxFormat txbxFormat)
        {
            fspa.XaLeft = (int)Math.Round(txbxFormat.HorizontalPosition * DLSConstants.TwipsInOnePoint);
            fspa.YaTop = (int)Math.Round(txbxFormat.VerticalPosition * DLSConstants.TwipsInOnePoint);
            fspa.Width = (int)Math.Round(txbxFormat.Width * DLSConstants.TwipsInOnePoint);
            fspa.Height = (int)Math.Round(txbxFormat.Height * DLSConstants.TwipsInOnePoint);
            if (txbxFormat.HorizontalOrigin == HorizontalOrigin.LeftMargin || txbxFormat.HorizontalOrigin == HorizontalOrigin.RightMargin 
                || txbxFormat.HorizontalOrigin == HorizontalOrigin.InsideMargin || txbxFormat.HorizontalOrigin == HorizontalOrigin.OutsideMargin)
                fspa.RelHrzPos = HorizontalOrigin.Margin;
            else
                fspa.RelHrzPos = txbxFormat.HorizontalOrigin;
            if (txbxFormat.VerticalOrigin == VerticalOrigin.TopMargin || txbxFormat.VerticalOrigin == VerticalOrigin.BottomMargin || txbxFormat.VerticalOrigin == VerticalOrigin.InsideMargin || txbxFormat.VerticalOrigin == VerticalOrigin.OutsideMargin )
            {
                fspa.RelVrtPos = VerticalOrigin.Page;
                if (txbxFormat.VerticalPosition < 0)
                {
                    fspa.YaTop = 0;
                }
            }
            else
                fspa.RelVrtPos = txbxFormat.VerticalOrigin;
            fspa.TextWrappingStyle = txbxFormat.TextWrappingStyle;
            fspa.TextWrappingType = txbxFormat.TextWrappingType;
            fspa.IsBelowText = txbxFormat.IsBelowText;
            fspa.Spid = (int)txbxFormat.TextBoxShapeID;
        }
        /// <summary>
        /// Exports the shape position.
        /// </summary>
        /// <param name="txbxContainer">The textbox container.</param>
        /// <param name="txbxFormat">The textbox format.</param>
        private static void ExportPosition(MsofbtSpContainer txbxContainer, WTextBoxFormat txbxFormat)
        {
            if (txbxContainer.ShapePosition.XAlign != uint.MaxValue)
            {
                txbxFormat.HorizontalAlignment = (ShapeHorizontalAlignment)txbxContainer.ShapePosition.XAlign;
            }
            if (txbxContainer.ShapePosition.YAlign != uint.MaxValue)
            {
                txbxFormat.VerticalAlignment = (ShapeVerticalAlignment)txbxContainer.ShapePosition.YAlign;
            }
            if (txbxContainer.ShapePosition.XRelTo != uint.MaxValue)
            {
                txbxFormat.HorizontalOrigin = (HorizontalOrigin)txbxContainer.ShapePosition.XRelTo;
            }
            if (txbxContainer.ShapePosition.YRelTo != uint.MaxValue)
            {
                txbxFormat.VerticalOrigin = (VerticalOrigin)txbxContainer.ShapePosition.YRelTo;
            }
        }
        /// <summary>
        /// Exports the internal margin.
        /// </summary>
        /// <param name="txbxContainer">The textbox container.</param>
        /// <param name="txbxFormat">The textbox format.</param>
        private static void ExportIntMargin(MsofbtSpContainer txbxContainer, WTextBoxFormat txbxFormat)
        {
            uint prop = txbxContainer.GetPropertyValue((int)FOPTEText.dxTextLeft);
            if (prop != uint.MaxValue)
            {
                txbxFormat.InternalMargin.Left = (float)prop / (DLSConstants.EmusPerPoint);
            }

            prop = txbxContainer.GetPropertyValue((int)FOPTEText.dxTextRight);
            if (prop != uint.MaxValue)
            {
                txbxFormat.InternalMargin.Right = (float)prop / (DLSConstants.EmusPerPoint);
            }

            prop = txbxContainer.GetPropertyValue((int)FOPTEText.dyTextTop);
            if (prop != uint.MaxValue)
            {
                txbxFormat.InternalMargin.Top = (float)prop / (DLSConstants.EmusPerPoint);
            }

            prop = txbxContainer.GetPropertyValue((int)FOPTEText.dyTextBottom);
            if (prop != uint.MaxValue)
            {
                txbxFormat.InternalMargin.Bottom = (float)prop / (DLSConstants.EmusPerPoint);
            }
        }
        #endregion
    }

    /// <summary>
    /// Summary description for class ListPropertiesConverter
    /// </summary>
    //[ CLSCompliant( false ) ]
    [DocumentationExclude()]
    internal class ListPropertiesConverter
    {
        #region Class utility methods
        /// <summary>
        /// Export paragraphs's list data to ListStyle
        /// </summary>
        /// <param name="listFormat">WListFormat item</param>
        /// <param name="reader">Current reader</param>
        public static void Export(WListFormat listFormat, WordReaderBase reader)
        {
            int formatIndex = reader.ParagraphProperties.ListFormatIndex;
            int levelIndex = reader.ParagraphProperties.ListLevelIndex;

            Export(formatIndex, levelIndex, listFormat, reader);
        }
        /// <summary>
        /// Export paragraphs's list data to ListStyle
        /// </summary>
        /// <param name="formatIndex">Index of the format.</param>
        /// <param name="levelIndex">Index of the level.</param>
        /// <param name="listFormat">WListFormat item</param>
        /// <param name="reader">Current reader</param>
        public static void Export(int formatIndex, int levelIndex, WListFormat listFormat, WordReaderBase reader)
        {
            if (formatIndex == 0)
                listFormat.IsEmptyList = true;

            ListInfo listInfo = reader.ListInfo;
            if (formatIndex > 0 && listInfo != null && listInfo.ListFormatOverrides.Count > 0)
            {
                if (listInfo.ListFormatOverrides.Count < formatIndex)
                    return;

                int id = listInfo.ListFormatOverrides[formatIndex - 1].ListID;
                ListData listData = listInfo.ListFormats.GetListFromId(id);

                ListFormatOverride lstOverride = listInfo.ListFormatOverrides[formatIndex - 1] as ListFormatOverride;
                if (lstOverride.Levels.Count > 0)
                {
                    listFormat.LFOStyleName = ExportListFormatOverrides(formatIndex, reader, lstOverride, listFormat);
                }

                ExportListFormat(listFormat, reader, id, listData, levelIndex);

                // Export list format in case when properties were changes (track changes is on).
                if (reader.ParagraphProperties.IsChangedFormat)
                {
                    ExportNewListFormat(listFormat, reader);
                }
            }
        }
        /// <summary>
        /// Import ListStyle to ListData.
        /// </summary>
        /// <param name="lstStyle">Source ListStyle</param>
        /// <param name="listFormat">Destination ListData</param>
        /// <param name="styleSheet">Stylesheet</param>
        public static void Import(ListStyle lstStyle, ListData listFormat,
          WordStyleSheet styleSheet)
        {
            for (int i = 0, cnt = lstStyle.Levels.Count; i < cnt; i++)
            {
                WListLevel dlsListLevel = lstStyle.Levels[i] as WListLevel;
                DocIOListLevel docListLevel = new DocIOListLevel();
                ImportToDocListLevel(dlsListLevel, docListLevel, styleSheet, i);
                listFormat.Levels.Add(docListLevel);
            }
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Exports the list format.
        /// </summary>
        /// <param name="listFormat">The list format.</param>
        /// <param name="reader">The reader.</param>
        /// <param name="id">The list id.</param>
        /// <param name="listData">The list data.</param>
        /// <param name="levelIndex">Index of the level.</param>
        private static void ExportListFormat(WListFormat listFormat, WordReaderBase reader, int id,
          ListData listData, int levelIndex)
        {
            if (levelIndex > listData.Levels.Count)
                levelIndex = 0;

            if (levelIndex < listData.Levels.Count)
            {
                CheckListCollection(id, listFormat);

                string lstStyleName = ExportListStyle(listFormat, reader, id, listData);

                listFormat.ListLevelNumber = levelIndex;
                listFormat.ApplyStyle(lstStyleName);
            }
        }
        /// <summary>
        /// Exports the new list format.
        /// </summary>
        /// <param name="listFormat">The list format.</param>
        /// <param name="reader">The reader.</param>
        private static void ExportNewListFormat(WListFormat listFormat, WordReaderBase reader)
        {
            int newFormatIndex = reader.ParagraphProperties.NewListFormatIndex;
            if (newFormatIndex != short.MaxValue && newFormatIndex > 0)
            {
                ListInfo listInfo = reader.ListInfo;
                int id = listInfo.ListFormatOverrides[newFormatIndex - 1].ListID;
                ListData listData = listInfo.ListFormats.GetListFromId(id);

                ListFormatOverride lstOverride = listInfo.ListFormatOverrides[newFormatIndex - 1] as ListFormatOverride;
                if (lstOverride.Levels.Count > 0)
                {
                    listFormat.NewLfoStyleName = ExportListFormatOverrides(newFormatIndex, reader, lstOverride, listFormat);
                }
                listFormat.NewStyleName = ExportListStyle(listFormat, reader, id, listData);
            }

            int newLevelIndex = reader.ParagraphProperties.NewListLevelIndex;
            if (newLevelIndex != byte.MaxValue)
            {
                listFormat.NewListLevelNumber = newLevelIndex;
            }
        }
        /// <summary>
        /// Exports the list style to the document.
        /// </summary>
        /// <param name="listFormat">The list format.</param>
        /// <param name="reader">The reader.</param>
        /// <param name="id">The id.</param>
        /// <param name="listData">The list data.</param>
        /// <returns></returns>
        private static string ExportListStyle(WListFormat listFormat, WordReaderBase reader, int id,
          ListData listData)
        {
            if (!AdapterListIDHolder.Instance.ListStyleIDtoName.ContainsKey(id))
            {
                bool isBulletPattern = IsBulletPatternType(listData.Levels);
                ListType curListType = (isBulletPattern) ? ListType.Bulleted : ListType.Numbered;
                ListStyle listStyle = ListStyle.CreateEmptyListStyle(listFormat.Document, curListType, listData.SimpleList);
                ExportToListLevelCollection(listData.Levels, listStyle.Levels, reader);

                listStyle.Name = (isBulletPattern) ? "Bulleted_" + Guid.NewGuid().ToString() :
                    "Numbered_" + Guid.NewGuid().ToString();
                listFormat.RestartNumbering = true;

                AdapterListIDHolder.Instance.ListStyleIDtoName.Add(id, listStyle.Name);
                listFormat.Document.ListStyles.Add(listStyle);
                listStyle.IsHybrid = listData.IsHybridMultilevel;
                if (listData.SimpleList)
                {
                    listStyle.IsSimple = true;
                }

                return listStyle.Name;
            }
            else
            {
                return AdapterListIDHolder.Instance.ListStyleIDtoName[id];
            }
        }
        /// <summary>
        /// Export ListLevels to ListLevelCollection.
        /// </summary>
        /// <param name="lstLevels"></param>
        /// <param name="lstLevelCol"></param>
        /// <param name="reader"></param>
        private static void ExportToListLevelCollection(ListLevels lstLevels,
          ListLevelCollection lstLevelCol, WordReaderBase reader)
        {
            for (int i = 0, cnt = lstLevels.Count; i < cnt; i++)
            {
                DocIOListLevel docListLevel = lstLevels[i] as DocIOListLevel;
                ExportToDLSListLevel(docListLevel, (WListLevel)lstLevelCol[i], reader, i);
            }
        }
        /// <summary>
        /// Convert DocIO listlevel to DLS listlevel.
        /// </summary>
        /// <param name="docListLevel"></param>
        /// <param name="dlsListLevel"></param>
        /// <param name="reader"></param>
        /// <param name="levelNumber"></param>
        private static void ExportToDLSListLevel(DocIOListLevel docListLevel, WListLevel dlsListLevel,
          WordReaderBase reader, int levelNumber)
        {
            dlsListLevel.PatternType = (ListPatternType)docListLevel.m_nfc;
            dlsListLevel.UsePrevLevelPattern = docListLevel.m_bPrev;
            dlsListLevel.StartAt = docListLevel.m_startAt;
            dlsListLevel.NumberAlignment = docListLevel.m_jc;
            dlsListLevel.FollowCharacter = docListLevel.m_ixchFollow;
            dlsListLevel.IsLegalStyleNumbering = docListLevel.m_bLegal;
            dlsListLevel.NoRestartByHigher = docListLevel.m_bNoRestart;

            //Word6 compartibility options
            dlsListLevel.Word6Legacy = docListLevel.m_bWord6;
            dlsListLevel.LegacySpace = docListLevel.m_dxaSpace;
            dlsListLevel.LegacyIndent = docListLevel.m_dxaIndent;

            char[] splitter = new char[2] { '\\', Convert.ToChar(levelNumber) };
            string[] patternArray = docListLevel.m_str.Split(splitter);
            //Export list pattern
            if (dlsListLevel.PatternType == ListPatternType.Bullet)
            {
                if (patternArray.Length > 1)
                {
                    dlsListLevel.BulletCharacter = patternArray[0];
                    dlsListLevel.BulletCharacter += patternArray[1];
                }
                else
                    dlsListLevel.BulletCharacter = docListLevel.m_str;
            }
            else
            {
                if (patternArray.Length > 1)
                {
                    dlsListLevel.NumberPrefix = patternArray[0];
                    dlsListLevel.NumberSufix = patternArray[1];
                }
                else if (patternArray[0] == string.Empty)
                {
                    dlsListLevel.NumberPrefix = dlsListLevel.NumberSufix = null;
                }
                else
                {
                    dlsListLevel.NumberPrefix = patternArray[0];
                    dlsListLevel.NoLevelText = true;
                }
                //To preserve level text for pattern type None
                if(dlsListLevel.PatternType == ListPatternType.None)
                    dlsListLevel.BulletCharacter = docListLevel.m_str;
            }

            //Export character properties
            docListLevel.m_charProps.StyleSheet = reader.StyleSheet;
            CharacterPropertiesConverter.CHPToFormat(docListLevel.m_charProps, dlsListLevel.CharacterFormat);
            //Export paragraph propeties
            ParagraphPropertiesConverter.Export(docListLevel.m_parProps, dlsListLevel.ParagraphFormat as WParagraphFormat);
        }
        /// <summary>
        /// Import data from DLS ListLevel format to 
        /// DocIO ListLevel format.
        /// </summary>
        /// <param name="dlsListLevel">Source DLS ListLevel</param>
        /// <param name="docListLevel">Destination DocIO ListLevel</param>
        /// <param name="styleSheet">Stylesheet</param>
        /// <param name="levelIndex">Stylesheet</param>
        internal static void ImportToDocListLevel(WListLevel dlsListLevel, DocIOListLevel docListLevel,
          WordStyleSheet styleSheet, int levelIndex)
        {
            docListLevel.m_nfc = dlsListLevel.PatternType;
            docListLevel.m_bPrev = dlsListLevel.UsePrevLevelPattern;
            docListLevel.m_startAt = dlsListLevel.StartAt;
            docListLevel.m_jc = dlsListLevel.NumberAlignment;
            docListLevel.m_ixchFollow = dlsListLevel.FollowCharacter;
            docListLevel.m_bLegal = dlsListLevel.IsLegalStyleNumbering;
            docListLevel.m_bNoRestart = dlsListLevel.NoRestartByHigher;

            //Word6 compartibility options
            docListLevel.m_bWord6 = dlsListLevel.Word6Legacy;
            docListLevel.m_dxaSpace = dlsListLevel.LegacySpace;
            docListLevel.m_dxaIndent = dlsListLevel.LegacyIndent;

            bool preserveLevelText = false;
            if (dlsListLevel.PatternType == ListPatternType.None && 
                    dlsListLevel.BulletCharacter != null && dlsListLevel.BulletCharacter.Length > 0 && 
                    dlsListLevel.ParaStyleName==null)
                preserveLevelText = true;
            //Import list pattern
            if (dlsListLevel.PatternType != ListPatternType.Bullet && !preserveLevelText)
            {
                char level = Convert.ToChar(levelIndex);
                if (dlsListLevel.NumberPrefix == null && dlsListLevel.NumberSufix == null)
                {
                    docListLevel.m_str = string.Empty;
                }
                else
                {
                    docListLevel.m_str = dlsListLevel.NumberPrefix;
                    if (!dlsListLevel.NoLevelText)
                    {
                        docListLevel.m_str += level.ToString() + dlsListLevel.NumberSufix;
                    }
                }

                CreateCharacterOffsets(docListLevel.m_str, ref docListLevel.m_rgbxchNums);
            }
            else
            {
                docListLevel.m_str = dlsListLevel.BulletCharacter;
            }

            //Import character properties
            if (docListLevel.m_charProps == null)
                docListLevel.m_charProps = new CharacterProperties(styleSheet);
            CharacterPropertiesConverter.FormatToCHP(dlsListLevel.CharacterFormat as WCharacterFormat, docListLevel.m_charProps);
            if (dlsListLevel.CharacterFormat.HasKey(WCharacterFormat.FontNameKey))
            {
                ushort fontIndex = (ushort)docListLevel.m_charProps.StyleSheet.FontNameToIndex(dlsListLevel.CharacterFormat.FontName);
                docListLevel.m_charProps.FontFarEast = fontIndex;
                docListLevel.m_charProps.FontNonFarEast = fontIndex;
            }

            //Import paragraph properties
            if (docListLevel.m_parProps == null)
                docListLevel.m_parProps = new ParagraphProperties();
            ParagraphPropertiesConverter.Import(docListLevel.m_parProps, dlsListLevel.ParagraphFormat, null);
        }
        /// <summary>
        /// Create list level character offsets
        /// </summary>
        /// <param name="numStr"></param>
        /// <param name="characterOffsets"></param>
        private static void CreateCharacterOffsets(string numStr, ref byte[] characterOffsets)
        {
            if (numStr == string.Empty)
                return;

            characterOffsets[0] = 1;
            int offsetIndex = 0;
            for (int i = 0; i < 9; i++)
            {
                char[] splitter = new char[2] { '\\', Convert.ToChar(i) };
                string[] patternArray = numStr.Split(splitter);
                if (patternArray.Length > 1)
                {
                    characterOffsets[offsetIndex] = (byte)(patternArray[0].Length + 1);
                    offsetIndex++;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="levels"></param>
        /// <returns></returns>
        private static bool IsBulletPatternType(ListLevels levels)
        {
            bool retVal = true;
            for (int i = 0, cnt = levels.Count; i < cnt; i++)
            {
                if ((levels[i] as DocIOListLevel).m_nfc != ListPatternType.Bullet)
                {
                    retVal = false;
                    break;
                }
            }
            return retVal;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="listId"></param>
        /// <param name="listFormat"></param>
        /// <returns></returns>
        private static void CheckListCollection(int listId, WListFormat listFormat)
        {
            if (AdapterListIDHolder.Instance.ListStyleIDtoName.ContainsKey(listId))
            {
                string listName = AdapterListIDHolder.Instance.ListStyleIDtoName[listId];
                ListStyle style = listFormat.Document.ListStyles.FindByName(listName);
                if (style == null)
                {
                    AdapterListIDHolder.Instance.ListStyleIDtoName.Remove(listId);
                }
            }
        }
        /// <summary>
        /// Does current list uses base style?
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static bool UseBaseListStyle(IWordReaderBase reader)
        {
            bool retVal = false;
            SinglePropertyModifierRecord lfoSprm = reader.ParagraphProperties.Sprms[WordSprmOptions.sprmPIlfo];
            if (lfoSprm == null)
            {
                retVal = true;
            }
            return retVal;
        }

        /// <summary>
        /// Export list format overrides.
        /// </summary>
        private static string ExportListFormatOverrides(int lfoIndex, WordReaderBase reader,
          ListFormatOverride lstOverride, WListFormat listFormat)
        {
            if (!AdapterListIDHolder.Instance.LfoStyleIDtoName.ContainsKey(lfoIndex - 1))
            {
                ListOverrideStyle lstOverrideStyle = new ListOverrideStyle(listFormat.Document);
                ExportListOverride(lstOverride, lstOverrideStyle, reader, listFormat.Document);

                lstOverrideStyle.Name = "LfoStyle_" + Guid.NewGuid().ToString();
                (listFormat.Document as WordDocument).ListOverrides.Add(lstOverrideStyle);
                AdapterListIDHolder.Instance.LfoStyleIDtoName.Add(lfoIndex - 1, lstOverrideStyle.Name);
                //listFormat.LFOStyleName = lstOverrideStyle.Name;
                return lstOverrideStyle.Name;
            }
            else
            {
                //listFormat.LFOStyleName = ( string )AdapterListIDHolder.Instance.LfoStyleIDtoName[ lfoIndex - 1 ];
                return AdapterListIDHolder.Instance.LfoStyleIDtoName[lfoIndex - 1];
            }
        }
        /// <summary>
        /// Exports the list override.
        /// </summary>
        /// <param name="sourceLfo">The source lfo.</param>
        /// <param name="listOverrideStyle">The list override style.</param>
        /// <param name="reader">The reader.</param>
        /// <param name="doc">The doc.</param>
        internal static void ExportListOverride(ListFormatOverride sourceLfo, ListOverrideStyle listOverrideStyle,
          WordReaderBase reader, IWordDocument doc)
        {
            listOverrideStyle.m_res1 = sourceLfo.m_res1;
            listOverrideStyle.m_res2 = sourceLfo.m_res2;

            //Export ListFormatOverrideLevel -> OverrideLevelFormat 
            for (int i = 0; i < sourceLfo.Levels.Count; i++)
            {
                ListFormatOverrideLevel lfoLevel = sourceLfo.Levels[i] as ListFormatOverrideLevel;
                OverrideLevelFormat overrideLevFormat = new OverrideLevelFormat((WordDocument)doc);

                overrideLevFormat.OverrideFormatting = lfoLevel.m_bFormatting;
                overrideLevFormat.OverrideStartAtValue = lfoLevel.m_bStartAt;
                overrideLevFormat.StartAt = lfoLevel.m_startAt;
                overrideLevFormat.m_reserved1 = lfoLevel.m_reserved1;
                overrideLevFormat.m_reserved2 = lfoLevel.m_reserved2;
                overrideLevFormat.m_reserved3 = lfoLevel.m_reserved3;

                if (lfoLevel.m_lvl != null && lfoLevel.m_bFormatting)
                {
                    ExportToDLSListLevel(lfoLevel.m_lvl, overrideLevFormat.OverrideListLevel, reader, lfoLevel.m_ilvl);
                }
                listOverrideStyle.OverrideLevels.Add(lfoLevel.m_ilvl, overrideLevFormat);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="listOverrideStyle"></param>
        /// <param name="lfo"></param>
        /// <param name="styleSheet"></param>
        internal static void ImportListOverride(ListOverrideStyle listOverrideStyle, ListFormatOverride lfo,
          WordStyleSheet styleSheet)
        {
            lfo.m_res1 = listOverrideStyle.m_res1;
            lfo.m_res2 = listOverrideStyle.m_res2;

            foreach (KeyValuePair<int, int> keyValuePair in listOverrideStyle.OverrideLevels.LevelIndex)
            {
                OverrideLevelFormat overrideLevFormat = listOverrideStyle.OverrideLevels[keyValuePair.Key];
                ListFormatOverrideLevel lfoLevel = new ListFormatOverrideLevel(overrideLevFormat.OverrideFormatting);
                lfoLevel.m_ilvl = keyValuePair.Key;
                lfoLevel.m_bFormatting = overrideLevFormat.OverrideFormatting;
                lfoLevel.m_bStartAt = overrideLevFormat.OverrideStartAtValue;
                lfoLevel.m_startAt = overrideLevFormat.StartAt;
                lfoLevel.m_reserved1 = overrideLevFormat.m_reserved1;
                lfoLevel.m_reserved2 = overrideLevFormat.m_reserved2;
                lfoLevel.m_reserved3 = overrideLevFormat.m_reserved3;

                if (overrideLevFormat.OverrideListLevel != null && overrideLevFormat.OverrideFormatting)
                {
                    ImportToDocListLevel(overrideLevFormat.OverrideListLevel, lfoLevel.m_lvl, styleSheet, keyValuePair.Key);
                }
                lfo.Levels.Add(lfoLevel);
            }
        }
        #region Commented code

        //    /// <summary>
        //    /// Get pattern prefix
        //    /// </summary>
        //    /// <param name="numStr"></param>
        //    /// <param name="characterOffsets"></param>
        //    /// <param name="levelNumber"></param>
        //    /// <returns></returns>
        //    private static void GetDataByOffset( string numStr, byte[] characterOffsets, int levelNumber, 
        //      Syncfusion.DocIO.DLS.ListLevel listLevel )
        //    {
        //      int start = 0;
        //      int length = 0;
        //      char[] splitter = new char[ 2 ] { '\\', Convert.ToChar( levelNumber) };
        //      string[] patternArray = numStr.Split( splitter );
        //      int numOffset = patternArray[ 0 ].Length + 1;
        //      for( int i = 0; i < 9; i++ )
        //      {
        //        if( ( int )characterOffsets[ i ] == numOffset )
        //        {
        //          //Get prefix
        //          if( i == 0 ) listLevel.NumberPrefix = numStr.Substring( 0 , numOffset - 1 );
        //          else
        //          {
        //            start = ( int )characterOffsets[ i - 1 ];
        //            length = ( numOffset - 1 ) - ( int )characterOffsets[ i-1 ]; 
        //            listLevel.NumberPrefix = numStr.Substring( start, length );
        //
        //            //Get internal prefix
        //            length = ( int )characterOffsets[ i-1 ];
        //            listLevel.InternalNumberPrefix = numStr.Substring( 0, length );
        //          }
        //
        //          //Get sufix
        //          if(( i == 8 ) || ( ( int )characterOffsets[ i + 1 ] == 0 ))
        //          {
        //            listLevel.NumberSufix = patternArray[ 1 ];
        //          }
        //          else
        //          {
        //            length = ( int )characterOffsets[ i + 1 ] - ( numOffset + 1 );
        //            start = numOffset + 1;
        //            listLevel.NumberSufix = numStr.Substring( start, length );
        // 
        //            //Get internal sufix
        //            start = ( int )characterOffsets[ i + 1 ] - 2;
        //            length = ( numStr.Length - 1 ) - start;   
        //            listLevel.InternalNumberSufix = numStr.Substring( start, length );
        //          }
        //          break;
        //        }
        //      }
        //    }
        #endregion

        #endregion
    }

    /// <summary>
    /// Summary description for class FormFieldPropertiesConverter
    /// </summary>
    internal class FormFieldPropertiesConverter
    {
        #region Class utility methods
        /// <summary>
        /// Reads the form field properties.
        /// </summary>
        /// <param name="formField">The form field.</param>
        /// <param name="frmField">The FRM field.</param>
        public static void ReadFormFieldProperties(WFormField formField, FormField frmField)
        {
            formField.Name = frmField.Title;
            if (frmField.Title != null)
            {
                formField.Help = frmField.Help;
                formField.MacroOnEnd = frmField.MacroOnEnd;
                formField.MacroOnStart = frmField.MacroOnStart;

                formField.StatusBarHelp = frmField.Tooltip;
                formField.Value = frmField.Value;
                formField.Params = frmField.Params;

                if (frmField.FieldType == FieldType.FieldFormDropDown)
                {
                    WDropDownFormField dropDown = formField as WDropDownFormField;
                    dropDown.DefaultDropDownValue = frmField.DefaultDropDownValue;
                    dropDown.DropDownSelectedIndex = frmField.DropDownIndex;
                    for (int i = 0; i < frmField.DropDownItems.Count; i++)
                    {
                        dropDown.DropDownItems.Add(frmField.DropDownItems[i]);
                    }
                    if (frmField.DropDownItems.Count > 0)
                    {
                        dropDown.DropDownValue = frmField.DropDownValue;
                    }
                }
                else if (frmField.FieldType == FieldType.FieldFormCheckBox)
                {
                    WCheckBox checkBoxField = formField as WCheckBox;
                    checkBoxField.CheckBoxSize = frmField.CheckBoxSize / 2;
                    checkBoxField.DefaultCheckBoxValue = frmField.DefaultCheckBoxValue;
                }
                else if (frmField.FieldType == FieldType.FieldFormTextInput)
                {
                    WTextFormField textFormField = formField as WTextFormField;
                    textFormField.MaximumLength = frmField.MaxLength;
                    textFormField.StringFormat = frmField.Format;
                    textFormField.Type = frmField.TextFormFieldType;
                    textFormField.DefaultText = frmField.DefaultTextInputValue;

                    if (textFormField.Type == TextFormFieldType.RegularText)
                    {
                        textFormField.TextFormat = GetTextFormat(frmField.Format);
                    }
                    //          else if( textFormField.Type == TextFormFieldType.NumberText )
                    //          {
                    //            textFormField.NumberFormat = GetNumberFormat( frmField.Format );
                    //          }
                }
            }
        }
        /// <summary>
        /// Writes the form field properties.
        /// </summary>
        /// <param name="frmField">The FRM field.</param>
        /// <param name="formField">The form field.</param>
        public static void WriteFormFieldProperties(FormField frmField, WFormField formField)
        {
            frmField.Help = formField.Help;
            frmField.MacroOnEnd = formField.MacroOnEnd;
            frmField.MacroOnStart = formField.MacroOnStart;

            frmField.Params = (short)formField.Params;
            frmField.Title = formField.Name;
            frmField.Tooltip = formField.StatusBarHelp;
            frmField.Value = formField.Value;

            if (formField.FormFieldType == FormFieldType.DropDown)
            {
                WDropDownFormField dropDown = formField as WDropDownFormField;

                for (int i = 0; i < dropDown.DropDownItems.Count; i++)
                {
                    frmField.DropDownItems.Add(dropDown.DropDownItems[i].Text);
                }
                frmField.DefaultDropDownValue = dropDown.DefaultDropDownValue;
                frmField.DropDownIndex = dropDown.DropDownSelectedIndex;
                if (dropDown.DropDownItems.Count > 0)
                {
                    if (dropDown.DropDownItems.Count <= dropDown.DropDownSelectedIndex)
                    {
                        throw new ArgumentException("DropDownItem with index "
                          + dropDown.DropDownSelectedIndex + " doesn't exist");
                    }
                    frmField.DropDownValue = dropDown.DropDownValue;
                }
            }
            else if (formField.FormFieldType == FormFieldType.CheckBox)
            {
                WCheckBox checkBox = formField as WCheckBox;
                frmField.CheckBoxSize = checkBox.CheckBoxSize * 2;
                frmField.DefaultCheckBoxValue = checkBox.DefaultCheckBoxValue;
            }
            else if (formField.FormFieldType == FormFieldType.TextInput)
            {
                WTextFormField textFormField = formField as WTextFormField;
                frmField.MaxLength = textFormField.MaximumLength;
                frmField.TextFormFieldType = textFormField.Type;
                if (textFormField.Type == TextFormFieldType.RegularText)
                {
                    frmField.Format = GetStringTextFormat(textFormField);
                    frmField.DefaultTextInputValue = FormatText(textFormField.TextFormat, textFormField.DefaultText);
                    
                }
                //        else if( textFormField.Type == TextFormFieldType.NumberText )
                //        {
                //          if( textFormField.NumberFormat == NumberFormat.None && textFormField.StringFormat != string.Empty )
                //          {            
                //            frmField.Format = textFormField.StringFormat;
                //          }
                //          else frmField.Format = GetStringNumberFormat( textFormField.NumberFormat );
                //          string format = frmField.Format;
                //          frmField.DefaultTextInputValue = FormatNumberText( format.Replace( ',' , '.' ),  textFormField.NumberFormat, textFormField.DefaultData );
                //        }
                else
                {
                    frmField.Format = textFormField.StringFormat;
                    frmField.DefaultTextInputValue = textFormField.DefaultText;
                }

            }
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Gets the text format.
        /// </summary>
        /// <param name="formFieldFormat">The form field format.</param>
        /// <returns></returns>
        private static TextFormat GetTextFormat(string formFieldFormat)
        {
            switch (formFieldFormat)
            {
                case "UPPERCASE":
                    return TextFormat.Uppercase;
                case "LOWERCASE":
                    return TextFormat.Lowercase;
                case "FIRST CAPITAL":
                    return TextFormat.FirstCapital;
                case "TITLE CASE":
                    return TextFormat.Titlecase;
                default:
                    return TextFormat.None;
            }
        }
        /// <summary>
        /// Gets the string text format.
        /// </summary>
        /// <param name="formField">The form field.</param>
        /// <returns></returns>
        private static string GetStringTextFormat(WTextFormField formField)
        {
            switch (formField.TextFormat)
            {
                case TextFormat.Uppercase:
                    return "UPPERCASE";
                case TextFormat.Lowercase:
                    return "LOWERCASE";
                case TextFormat.FirstCapital:
                    return "FIRST CAPITAL";
                case TextFormat.Titlecase:
                    return "TITLE CASE";
                default:
                    return string.Empty;
            }
        }
        /// <summary>
        /// Gets the number format.
        /// </summary>
        /// <param name="formFieldFormat">The form field format.</param>
        /// <returns></returns>
        private static NumberFormat GetNumberFormat(string formFieldFormat)
        {
            switch (formFieldFormat)
            {
                case "0":
                    return NumberFormat.WholeNumber;
                case "0,00":
                    return NumberFormat.FloatingPoint;
                case "0%":
                    return NumberFormat.WholeNumberPercent;
                case "0,00%":
                    return NumberFormat.FloatingPointPercent;
                case "#�##0":
                    return NumberFormat.WholeNumberWithSpace;
                case "#�##0,00":
                    return NumberFormat.FloatingPointWithSpace;
            }
            if (formFieldFormat.StartsWith("#�##0,00 "))
                return NumberFormat.CurrencyFormat;

            return NumberFormat.None;
        }
        /// <summary>
        /// Gets the string number format.
        /// </summary>
        /// <param name="numberFormat">The number format.</param>
        /// <returns></returns>
        private static string GetStringNumberFormat(NumberFormat numberFormat)
        {
            switch (numberFormat)
            {
                case NumberFormat.WholeNumber:
                    return "0";
                case NumberFormat.FloatingPoint:
                    return "0,00";
                case NumberFormat.WholeNumberPercent:
                    return "0%";
                case NumberFormat.FloatingPointPercent:
                    return "0,00%";
                case NumberFormat.WholeNumberWithSpace:
                    return "#�##0";
                case NumberFormat.FloatingPointWithSpace:
                    return "#�##0,00";
                case NumberFormat.CurrencyFormat:
                    return "#�##0.00 " + CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol + ";(#�##0.00 " + CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol + ")";
            }
            return string.Empty;
        }
        /// <summary>
        /// Gets the default number values.
        /// </summary>
        /// <param name="numberFormat">The number format.</param>
        /// <returns></returns>
        private static string GetDefaultNumberValue(NumberFormat numberFormat)
        {
            switch (numberFormat)
            {
                case NumberFormat.WholeNumber:
                case NumberFormat.WholeNumberWithSpace:
                    return "0";
                case NumberFormat.FloatingPoint:
                case NumberFormat.FloatingPointWithSpace:
                    return "0,00";
                case NumberFormat.WholeNumberPercent:
                    return "0%";
                case NumberFormat.FloatingPointPercent:
                    return "0,00%";
                case NumberFormat.CurrencyFormat:
                    return "0,00 " + CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol;
            }
            return string.Empty;
        }
        /// <summary>
        /// Formats the text.
        /// </summary>
        /// <param name="textFormat">The text format.</param>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        internal static string FormatText(TextFormat textFormat, string text)
        {
            if (text != string.Empty)
            {
                if (textFormat == TextFormat.Uppercase)
                {
                    return text.ToUpper();
                }
                else if (textFormat == TextFormat.Lowercase)
                {
                    return text.ToLower();
                }
                else if (textFormat == TextFormat.FirstCapital)
                {
                    return text[0].ToString().ToUpper() + text.Remove(0, 1);
                }
                else if (textFormat == TextFormat.Titlecase)
                {
                    string[] words = text.Split(new char[] { ' ' });
                    for (int i = 0; i < words.Length; i++)
                    {
                        string word = words[i];
                        string titlePart = word[0].ToString().ToUpper() + word.Remove(0, 1);
                        words[i] = word;
                    }

                    text = string.Empty;
                    int wordsCount = words.Length;
                    for (int i = 0; i < wordsCount; i++)
                    {
                        text += words[i];
                        if (i < wordsCount - 1)
                            text += " ";
                    }

                    return text;
                }
            }
            return text;
        }
        /// <summary>
        /// Formats the number text.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="numberFormat">The number format.</param>
        /// <param name="inputData">The input data.</param>
        /// <returns></returns>
        private static string FormatNumberText(string format, NumberFormat numberFormat, string inputData)
        {
            if (numberFormat == NumberFormat.None)
            {
                return inputData;
            }

            inputData = inputData.Replace('.', ',');
            double dValue = 0;
            string numberStr = string.Empty;
            try
            {
                dValue = Convert.ToDouble(inputData);
            }
            catch
            {
                return GetDefaultNumberValue(numberFormat);
            }

            switch (numberFormat)
            {
                case NumberFormat.WholeNumber:
                case NumberFormat.WholeNumberPercent:
                    if (numberFormat == NumberFormat.WholeNumberPercent)
                    {
                        dValue = dValue * 100;
                        dValue = Math.Floor(dValue);
                        dValue /= 100;
                    }
                    else
                        dValue = Math.Floor(dValue);
                    return ConvertNumberToString(format, dValue);
                case NumberFormat.FloatingPoint:
                case NumberFormat.FloatingPointPercent:
                    return ConvertNumberToString(format, dValue);
                case NumberFormat.WholeNumberWithSpace:
                case NumberFormat.FloatingPointWithSpace:
                case NumberFormat.CurrencyFormat:
                    if (numberFormat == NumberFormat.WholeNumberWithSpace)
                    {
                        dValue = Math.Floor(dValue);
                    }
                    numberStr = ConvertNumberToString(format, dValue);
                    if (dValue < 1000)
                        numberStr = numberStr.Substring(1, numberStr.Length - 1);
                    return numberStr;
            }

            if (format != string.Empty)
            {
                return ConvertNumberToString(format, dValue);
            }

            return string.Empty;
        }
        /// <summary>
        /// Convert input value to a formatted string .
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="dValue">Input value.</param>
        /// <returns></returns>
        private static string ConvertNumberToString(string format, double dValue)
        {
            double roundedVal = 0;
            if (format[format.Length - 1] == '%')
            {
                dValue *= 100;
                roundedVal = Math.Round(dValue, 2);
                if (roundedVal > dValue)
                    roundedVal -= 0.01;
                roundedVal /= 100;
            }
            else
            {
                roundedVal = Math.Round(dValue, 2);
                if (roundedVal > dValue)
                    roundedVal -= 0.01;
            }
            string spaceString = roundedVal.ToString(format, CultureInfo.InvariantCulture);


            return spaceString;
        }

        #endregion
    }
}
