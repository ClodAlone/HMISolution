#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

//#define SKIP_OTHER_FONTS
//#define SKIP_OTHER_FONTS2
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
using System.Collections;
using Syncfusion.DocIO.DLS;
using System.Collections.Generic;
#if !WINRT && !WP
using System.Drawing;
#endif
#endregion

namespace Syncfusion.DocIO.ReaderWriter.Biff_Records
{
    /// <summary>
    /// Summary description for CharacterProperty.
    /// </summary>
    [CLSCompliant(false)]
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class CharacterProperties
    {
        #region Class members
        /// <summary>
        /// 
        /// </summary>
        //private SinglePropertyModifierArray m_arrSprms;
        private CharacterPropertyException m_chpx = null;

        /// <summary>
        /// 
        /// </summary>
        private CharacterProperties m_baseProps = null;

        /// <summary>
        /// 
        /// </summary>
        //    private SinglePropertyModifierArray m_emptySprms = new SinglePropertyModifierArray();

        /// <summary>
        /// 
        /// </summary>
        private WordStyleSheet m_styleSheet = null;

        /// <summary>
        /// 
        /// </summary>
        private bool m_stickProperties = true;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// 
        /// </summary>
        internal CharacterProperties(WordStyleSheet styleSheet)
        {
            m_chpx = new CharacterPropertyException();
            m_styleSheet = styleSheet;
        }

        /// <summary>
        /// Initializing constructor
        /// </summary>
        internal CharacterProperties(CharacterPropertyException chpx, WordStyleSheet styleSheet)
        {
            m_chpx = chpx;
            m_styleSheet = styleSheet;
        }

        /// <summary>
        /// Initializing constructor
        /// </summary>
        internal CharacterProperties(
          CharacterPropertyException chpx,
          WordStyleSheet styleSheet,
          CharacterProperties baseCharacterProperties)
        {
            m_chpx = chpx;
            m_baseProps = baseCharacterProperties;
            m_styleSheet = styleSheet;
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets Sprm array
        /// </summary>
        internal SinglePropertyModifierArray Sprms
        {
            get
            {
                if (m_chpx == null)
                {
                    //          return m_emptySprms;
                    return null;
                }

                return m_chpx.PropertyModifiers;
            }
        }
        /// <summary>
        /// Gets CharacterPropertyException.
        /// </summary>
        internal CharacterPropertyException CharacterPropertyException
        {
            get
            {
                return m_chpx;
            }
        }
        /// <summary>
        /// Gets or sets the base properties.
        /// </summary>
        /// <value>The base properties.</value>
        internal CharacterProperties BaseProperties
        {
            get
            {
                return m_baseProps;
            }
            set
            {
                m_baseProps = value;
            }
        }
        /// <summary>
        /// Gets/Sets the Complex script property
        /// </summary>
        internal bool ComplexScript
        {
            get
            {
                bool defVal = (m_baseProps != null) ? m_baseProps.ComplexScript : false;
                return Sprms.GetBoolean(WordSprmOptions.sprmCFComplexScripts, defVal);
            }
            set
            {

                Sprms.SetValue(WordSprmOptions.sprmCFComplexScripts, value);
            }
        }
        /// <summary>
        /// Gets/sets Bold property
        /// </summary>
        internal bool Bold
        {
            get
            {
                //        bool styleValue = ( m_baseProps != null )
                //                            ? m_baseProps.Sprms.GetBoolean( WordSprmOptions.sprmCFBold, false )
                //                            : false;
                //        byte defVal = 0;
                //        return GetComplexBoolean( Sprms.GetByte( WordSprmOptions.sprmCFBold, defVal ), styleValue );
                ////        bool defVal = ( m_baseProps != null ) ? m_baseProps.Bold : false;
                ////        return Sprms.GetBoolean( WordSprmOptions.sprmCFBold, defVal );
                // if( m_baseProps != null )
                {
                    bool styleValue = (m_baseProps != null)
                      ? m_baseProps.Sprms.GetBoolean(WordSprmOptions.sprmCFBold, false)
                      : false;
                    byte defVal = 0;
                    return GetComplexBoolean(Sprms.GetByte(WordSprmOptions.sprmCFBold, defVal), styleValue);
                }
            }
            set
            {
                Sprms.SetValue(WordSprmOptions.sprmCFBold, value);
            }
        }
        /// <summary>
        /// Gets/sets Italic property
        /// </summary>
        internal byte BoldComplex
        {
            get
            {
                return Sprms.GetByte(WordSprmOptions.sprmCFBold, 0);
            }
            set
            {
                if (value != byte.MaxValue)
                {
                    Sprms.SetValue(WordSprmOptions.sprmCFBold, value);
                }
            }
        }
        /// <summary>
        /// Gets/sets Italic property
        /// </summary>
        internal bool Italic
        {
            get
            {
                //        bool defVal = ( m_baseProps != null ) ? m_baseProps.Italic : false;
                //        return Sprms.GetBoolean( WordSprmOptions.sprmCFItalic, defVal );
                if (m_baseProps != null)
                {
                    bool styleValue = (m_baseProps != null)
                                        ? m_baseProps.Sprms.GetBoolean(WordSprmOptions.sprmCFItalic, false)
                                        : false;
                    byte defVal = 0;
                    return GetComplexBoolean(Sprms.GetByte(WordSprmOptions.sprmCFItalic, defVal), styleValue);
                }
                else
                {
                    bool defVal = false;
                    return Sprms.GetBoolean(WordSprmOptions.sprmCFItalic, defVal);
                }
            }
            set
            {
                Sprms.SetValue(WordSprmOptions.sprmCFItalic, value);
            }
        }
        /// <summary>
        /// Gets/sets Italic property
        /// </summary>
        internal byte ItalicComplex
        {
            get
            {
                return Sprms.GetByte(WordSprmOptions.sprmCFItalic, 0);
            }
            set
            {
                if (value != byte.MaxValue)
                {
                    Sprms.SetValue(WordSprmOptions.sprmCFItalic, value);
                }
            }
        }
        /// <summary>
        /// Gets/sets Strike property
        /// </summary>
        internal bool Strike
        {
            get
            {
                bool defVal = (m_baseProps != null) ? m_baseProps.Strike : false;
                return Sprms.GetBoolean(WordSprmOptions.sprmCFStrike, defVal);
            }
            set
            {
                Sprms.SetValue(WordSprmOptions.sprmCFStrike, value);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal byte ShadowComplex
        {
            get
            {
                return Sprms.GetByte(WordSprmOptions.sprmCFShadow, 0);
            }
            set
            {
                if (value != byte.MaxValue)
                {
                    Sprms.SetValue(WordSprmOptions.sprmCFShadow, value);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal byte StrikeComplex
        {
            get
            {
                return Sprms.GetByte(WordSprmOptions.sprmCFStrike, 0);
            }
            set
            {
                if (value != byte.MaxValue)
                {
                    Sprms.SetValue(WordSprmOptions.sprmCFStrike, value);
                }
            }
        }
        /// <summary>
        /// Gets/sets DoubleStrtike property
        /// </summary>
        internal bool DoubleStrike
        {
            get
            {
                bool defVal = (m_baseProps != null) ? m_baseProps.DoubleStrike : false;
                return Sprms.GetBoolean(WordSprmOptions.sprmCFDStrike, defVal);
            }
            set
            {
                if (value == true)
                {
                    Strike = false;
                }
                Sprms.SetValue(WordSprmOptions.sprmCFDStrike, value);
            }
        }
        /// <summary>
        /// Gets/sets Underline property
        /// </summary>
        internal byte UnderlineCode
        {
            get
            {
                byte defVal = (m_baseProps != null) ? m_baseProps.UnderlineCode : (byte)0;
                return Sprms.GetByte(WordSprmOptions.sprmCKul, defVal);
            }
            set
            {
                Sprms.SetValue(WordSprmOptions.sprmCKul, value);
            }
        }
        /// <summary>
        /// Gets/sets Font name property
        /// </summary>
        internal string FontName
        {
            get
            {
                if (m_styleSheet.FontNamesList.Count == 0)
                {
                    return string.Empty;
                }

                return m_styleSheet.FontNamesList[FontAscii];
            }
            set
            {
                int fontIndex = m_styleSheet.FontNameToIndex(value);

                if (fontIndex >= 0)
                {
                    FontAscii =
#if !SKIP_OTHER_FONTS
 FontFarEast = FontNonFarEast =
#endif
 (ushort)fontIndex;
                }
                else
                {
                    // TODO: Optimize later!!!!!!!!!
                    FontAscii =
#if !SKIP_OTHER_FONTS2
 FontFarEast = FontNonFarEast =
#endif
 (ushort)m_styleSheet.FontNamesList.Count;
                    m_styleSheet.UpdateFontName(value);
                }
            }
        }
        /// <summary>
        /// Gets/sets Font name property
        /// </summary>
        internal string FontNameAscii
        {
            get
            {
                if (m_styleSheet.FontNamesList.Count == 0)
                {
                    return string.Empty;
                }

                return m_styleSheet.FontNamesList[FontAscii];
            }
            set
            {
                int fontIndex = m_styleSheet.FontNameToIndex(value);

                if (fontIndex >= 0)
                {
                    FontAscii = (ushort)fontIndex;
                }
                else
                {
                    FontAscii = (ushort)m_styleSheet.FontNamesList.Count;
                    m_styleSheet.UpdateFontName(value);
                }
            }
        }
        /// <summary>
        /// Gets/sets Font name property
        /// </summary>
        internal string FontNameFarEast
        {
            get
            {
                if (m_styleSheet.FontNamesList.Count == 0)
                {
                    return string.Empty;
                }

                return m_styleSheet.FontNamesList[FontFarEast];
            }
            set
            {
                int fontIndex = m_styleSheet.FontNameToIndex(value);

                if (fontIndex >= 0)
                {
                    FontFarEast = (ushort)fontIndex;
                }
                else
                {
                    FontFarEast = (ushort)m_styleSheet.FontNamesList.Count;
                    m_styleSheet.UpdateFontName(value);
                }
            }
        }
        /// <summary>
        /// Gets/sets Font name property
        /// </summary>
        internal string FontNameNonFarEast
        {
            get
            {
                if (m_styleSheet.FontNamesList.Count == 0)
                {
                    return string.Empty;
                }

                return m_styleSheet.FontNamesList[FontNonFarEast];
            }
            set
            {
                int fontIndex = m_styleSheet.FontNameToIndex(value);

                if (fontIndex >= 0)
                {
                    FontNonFarEast = (ushort)fontIndex;
                }
                else
                {
                    FontNonFarEast = (ushort)m_styleSheet.FontNamesList.Count;
                    m_styleSheet.UpdateFontName(value);
                }
            }
        }
        /// <summary>
        /// Gets/sets Font name property
        /// </summary>
        internal string FontNameBi
        {
            get
            {
                if (m_styleSheet.FontNamesList.Count == 0)
                {
                    return string.Empty;
                }

                return m_styleSheet.FontNamesList[FontBi];
            }
            set
            {
                int fontIndex = m_styleSheet.FontNameToIndex(value);

                if (fontIndex >= 0)
                {
                    FontBi = (ushort)fontIndex;
                }
                else
                {
                    // TODO: Optimize later!!!!!!!!!
                    FontBi = (ushort)m_styleSheet.FontNamesList.Count;
                    m_styleSheet.UpdateFontName(value);
                }
            }
        }
        /// <summary>
        /// Gets/sets FontAscii property
        /// </summary>
        internal ushort FontAscii
        {
            get
            {
                ushort defVal = (m_baseProps != null) ? m_baseProps.FontAscii : (ushort)0;
                return Sprms.GetUShort(WordSprmOptions.sprmCRgFtc0, defVal);
            }
            set
            {
                // TODO: Checks if font is exists...
                Sprms.SetValue(WordSprmOptions.sprmCRgFtc0, value);
            }
        }
        /// <summary>
        /// Gets/sets FontFarEast property
        /// </summary>
        internal ushort FontFarEast
        {
            get
            {
                ushort defVal = (m_baseProps != null) ? m_baseProps.FontFarEast : (ushort)0;
                return Sprms.GetUShort(WordSprmOptions.sprmCRgFtc1, defVal);
            }
            set
            {
                // TODO: Checks if font is exists...
                Sprms.SetValue(WordSprmOptions.sprmCRgFtc1, value);
            }
        }
        /// <summary>
        /// Gets/sets Non FontFarEast property
        /// </summary>
        internal ushort FontNonFarEast
        {
            get
            {
                ushort defVal = (m_baseProps != null) ? m_baseProps.FontNonFarEast : (ushort)0;
                return Sprms.GetUShort(WordSprmOptions.sprmCRgFtc2, defVal);
            }
            set
            {
                // TODO: Checks if font is exists...
                Sprms.SetValue(WordSprmOptions.sprmCRgFtc2, value);
            }
        }
        /// <summary>
        /// Gets/sets Non FontFarEast property
        /// </summary>
        internal ushort FontBi
        {
            get
            {
                ushort defVal = (m_baseProps != null) ? m_baseProps.FontBi : (ushort)0;
                return Sprms.GetUShort(WordSprmOptions.sprmCFtcBi, defVal);
            }
            set
            {
                // TODO: Checks if font is exists...
                Sprms.SetValue(WordSprmOptions.sprmCFtcBi, value);
            }
        }
        /// <summary>
        /// Gets/sets FontSize property
        /// </summary>
        internal float FontSize
        {
            get
            {
                return (float)FontSizeHP / 2;
            }
            set
            {
                FontSizeHP = (ushort)(value * 2);
            }
        }
        /// <summary>
        /// Gets/sets FontSizeHP property
        /// </summary>
        internal ushort FontSizeHP
        {
            get
            {
                ushort defVal = (m_baseProps != null) ? m_baseProps.FontSizeHP : (ushort)20;
                return Sprms.GetUShort(WordSprmOptions.sprmCHps, defVal);
            }
            set
            {
                // TODO: Checks if font is exists...
                Sprms.SetValue(WordSprmOptions.sprmCHps, value);
            }
        }
        /// <summary>
        /// Gets/sets FontColor property
        /// </summary>
        internal byte FontColor
        {
            get
            {
                byte defVal = (m_baseProps != null) ? m_baseProps.FontColor : (byte)0;
                return Sprms.GetByte(WordSprmOptions.sprmCIco, defVal);
            }
            set
            {
                Sprms.SetValue(WordSprmOptions.sprmCIco, value);
            }
        }
        /// <summary>
        /// Gets/sets Extended FontColor property
        /// </summary>
        internal Color FontColorExt
        {
            get
            {
                //        byte defVal = ( m_baseProps != null ) ? m_baseProps.FontColor : ( byte )0;
                //        int intRes = Sprms.GetInt( WordSprmOptions.sprmCIcoe, defVal );
                //
                //        if( Sprms[ WordSprmOptions.sprmCIcoe ] != null )
                //        {
                //          byte[] Operand = Sprms[ WordSprmOptions.sprmCIcoe ].Operand;
                //          if( ( ( Operand[ 0 ] == 0 ) || ( Operand[ 0 ] == 255 ) ) &&
                //              ( ( Operand[ 1 ] == 0 ) || ( Operand[ 1 ] == 255 ) ) &&
                //              ( ( Operand[ 2 ] == 0 ) || ( Operand[ 2 ] == 255 ) ) &&
                //              ( ( Operand[ 3 ] == 0 ) || ( Operand[ 3 ] == 255 ) ) )
                //          {
                //            return Color.FromArgb( BaseWordRecord.WordKnownColors[ FontColor ] );
                //          }
                //
                //          else
                //          {
                //            return Color.FromArgb( ConvertColor( intRes ) );
                //          }
                //        }
                //          //else return Color.FromArgb( ConvertColor( BaseWordRecord.WordKnownColors[ FontColor ] ));
                //        else
                //        {
                //          return Color.FromArgb( BaseWordRecord.WordKnownColors[ FontColor ] );
                //        }
                uint defVal = (m_baseProps != null) ? WordColor.ConvertColorToRGB(m_baseProps.FontColorExt) : uint.MaxValue;
                uint intRes = Sprms.GetUInt(WordSprmOptions.sprmCIcoe, defVal);
                if (intRes == uint.MaxValue)
                {
                    return WordColor.ConvertIdToColor(FontColor);
                }
                else
                {
                    return WordColor.ConvertRGBToColor(intRes);
                }
            }
            set
            {
                //        int intVal = ConvertColor( value.ToArgb() );
                uint uintVal = WordColor.ConvertColorToRGB(value);
                Sprms.SetValue(WordSprmOptions.sprmCIcoe, uintVal);
            }
        }
        /// <summary>
        /// Gets/sets Extended FontColor property
        /// </summary>
        internal uint FontColorRGB
        {
            get
            {
                uint defVal = (m_baseProps != null) ? m_baseProps.FontColorRGB : uint.MaxValue;
                uint intRes = Sprms.GetUInt(WordSprmOptions.sprmCIcoe, defVal);
                if (intRes == uint.MaxValue)
                {
                    return WordColor.ConvertIdToRGB(FontColor);
                }
                else
                {
                    return intRes;
                }
            }
            set
            {
                Sprms.SetValue(WordSprmOptions.sprmCIcoe, value);
            }
        }
        /// <summary>
        /// Gets/sets BackGround color property
        /// </summary>
        internal byte HighlightColor
        {
            get
            {
                byte defVal = (m_baseProps != null) ? m_baseProps.HighlightColor : (byte)0;
                return Sprms.GetByte(WordSprmOptions.sprmCHighlight, defVal);

            }
            set
            {
                Sprms.SetValue(WordSprmOptions.sprmCHighlight, value);
            }
        }
        /// <summary>
        /// Gets/sets Sub/Super/Non script property
        /// </summary>
        internal byte SubSuperScript
        {
            get
            {
                byte defVal = (m_baseProps != null) ? m_baseProps.SubSuperScript : (byte)0;
                return Sprms.GetByte(WordSprmOptions.sprmCIss, defVal);
            }
            set
            {
                byte res = value;
                if (res >= 0 && res <= 2)
                {
                    Sprms.SetValue(WordSprmOptions.sprmCIss, res);
                }

            }
        }
        /// <summary>
        /// Gets/sets Picture Location
        /// </summary>
        internal int PicLocation
        {
            get
            {
                int defVal = (m_baseProps != null) ? m_baseProps.PicLocation : 0;
                int intRes = 0;
                if (Sprms[WordSprmOptions.sprmCPicLocation] != null)
                {
                    intRes = Sprms.GetInt(WordSprmOptions.sprmCPicLocation, defVal);
                }
                return intRes;
            }
            set
            {
                Sprms.SetValue(WordSprmOptions.sprmCPicLocation, value);
            }
        }
        /// <summary>
        /// Gets/sets Outline property
        /// </summary>
        internal bool Outline
        {
            get
            {
                bool defVal = (m_baseProps != null) ? m_baseProps.Outline : false;
                return Sprms.GetBoolean(WordSprmOptions.sprmCFOutline, defVal);
            }
            set
            {
                Sprms.SetValue(WordSprmOptions.sprmCFOutline, value);
            }
        }
        /// <summary>
        /// Gets/sets Shadow property
        /// </summary>
        internal bool Shadow
        {
            get
            {
                bool defVal = (m_baseProps != null) ? m_baseProps.Shadow : false;
                return Sprms.GetBoolean(WordSprmOptions.sprmCFShadow, defVal);
            }
            set
            {
                Sprms.SetValue(WordSprmOptions.sprmCFShadow, value);
            }
        }
        /// <summary>
        /// Gets/sets Emboss property
        /// </summary>
        internal bool Emboss
        {
            get
            {
                bool defVal = (m_baseProps != null) ? m_baseProps.Emboss : false;
                return Sprms.GetBoolean(WordSprmOptions.sprmCFEmboss, defVal);
            }
            set
            {
                Sprms.SetValue(WordSprmOptions.sprmCFEmboss, value);
            }
        }
        /// <summary>
        /// Gets/sets Emboss property (Complex).
        /// </summary>
        internal byte EmbossComplex
        {
            get
            {
                byte defVal = (m_baseProps != null) ? m_baseProps.EmbossComplex : byte.MaxValue;
                return Sprms.GetByte(WordSprmOptions.sprmCFEmboss, defVal);
            }
            set
            {
                if (value != byte.MaxValue)
                {
                    Sprms.SetValue(WordSprmOptions.sprmCFEmboss, value);
                }
            }
        }
        /// <summary>
        /// Gets/sets Engrave property
        /// </summary>
        internal bool Engrave
        {
            get
            {
                bool defVal = (m_baseProps != null) ? m_baseProps.Engrave : false;
                return Sprms.GetBoolean(WordSprmOptions.sprmCFImprint, defVal);
            }
            set
            {
                Sprms.SetValue(WordSprmOptions.sprmCFImprint, value);
            }
        }
        /// <summary>
        /// Gets/sets Engrave property ( Complex )
        /// </summary>
        internal byte EngraveComplex
        {
            get
            {
                byte defVal = (m_baseProps != null) ? m_baseProps.EngraveComplex : byte.MaxValue;
                return Sprms.GetByte(WordSprmOptions.sprmCFImprint, defVal);
            }
            set
            {
                if (value != byte.MaxValue)
                {
                    Sprms.SetValue(WordSprmOptions.sprmCFImprint, value);
                }
            }
        }
        /// <summary>
        /// Gets/sets Hidden property
        /// </summary>
        internal bool Hidden
        {
            get
            {
                bool defVal = (m_baseProps != null) ? m_baseProps.Hidden : false;
                return Sprms.GetBoolean(WordSprmOptions.sprmCFVanish, defVal);
            }
            set
            {
                Sprms.SetValue(WordSprmOptions.sprmCFVanish, value);
            }
        }
        /// <summary>
        /// Gets/sets SmallCaps property
        /// </summary>
        internal bool SmallCaps
        {
            get
            {
                bool defVal = (m_baseProps != null) ? m_baseProps.SmallCaps : false;
                return Sprms.GetBoolean(WordSprmOptions.sprmCFSmallCaps, defVal);
            }
            set
            {
                if (value == true)
                {
                    AllCaps = false;
                }
                Sprms.SetValue(WordSprmOptions.sprmCFSmallCaps, value);
            }
        }
        /// <summary>
        /// Gets/sets AllCapitalized property
        /// </summary>
        internal bool AllCaps
        {
            //      get
            //      {
            //        bool defVal = ( m_baseProps != null ) ? m_baseProps.AllCaps : false;
            //        return Sprms.GetBoolean( WordSprmOptions.sprmCFCaps, defVal );
            //      }
            get
            {
                bool styleValue = (m_baseProps != null)
                  ? m_baseProps.Sprms.GetBoolean(WordSprmOptions.sprmCFCaps, false)
                  : false;
                byte defVal = 0;
                return GetComplexBoolean(Sprms.GetByte(WordSprmOptions.sprmCFCaps, defVal), styleValue);
            }
            set
            {
                if (value == true)
                {
                    SmallCaps = false;
                }
                Sprms.SetValue(WordSprmOptions.sprmCFCaps, value);
            }
        }
        /// <summary>
        /// Gets/sets AllCapitalized property ( Complex ).
        /// </summary>
        internal byte AllCapsComplex
        {
            get
            {
                return Sprms.GetByte(WordSprmOptions.sprmCFCaps, 0);
            }
            set
            {
                if (value != byte.MaxValue)
                {
                    Sprms.SetValue(WordSprmOptions.sprmCFCaps, value);
                }
            }
        }
        /// <summary>
        /// Gets/sets Font Position( raised/lowered ) property
        /// </summary>
        internal short Position
        {
            get
            {
                short defVal = (m_baseProps != null) ? m_baseProps.Position : (short)0;
                return Sprms.GetShort(WordSprmOptions.sprmCHpsPos, defVal);
                ;

            }
            set
            {
                short res = value;
                Sprms.SetValue(WordSprmOptions.sprmCHpsPos, res);
            }
        }
        /// <summary>
        /// Gets/sets Font Spacing property
        /// </summary>
        internal short LineSpacing
        {
            get
            {
                short defVal = (m_baseProps != null) ? m_baseProps.LineSpacing : (short)0;
                return Sprms.GetShort(WordSprmOptions.sprmCDxaSpace, defVal);

            }
            set
            {
                short res = value;
                Sprms.SetValue(WordSprmOptions.sprmCDxaSpace, res);
            }
        }
        /// <summary>
        /// Gets/sets Font Shading property
        /// </summary>
        internal ShadingDescriptor Shading
        {
            get
            {
                short defVal = 0;
                short shortRes = Sprms.GetShort(WordSprmOptions.sprmCShd, defVal);
                return new ShadingDescriptor(shortRes);

            }
            set
            {
                short res = value.Save();
                Sprms.SetValue(WordSprmOptions.sprmCShd, res);
            }
        }
        /// <summary>
        /// Gets/sets Font Shading property
        /// </summary>
        internal ShadingDescriptor ShadingNew
        {
            get
            {
                byte[] res = Sprms.GetByteArray(WordSprmOptions.sprmCShdNew);
                ShadingDescriptor shading = new ShadingDescriptor();
                shading.ReadNewShd(res, 0);
                return shading;
            }
            set
            {
                byte[] res = value.SaveNewShd();
                Sprms.SetValue(WordSprmOptions.sprmCShdNew, res);
            }
        }
        /// <summary>
        /// Gets/sets Border property
        /// </summary>
        internal BorderCode Border
        {
            get
            {
                byte[] iRes = Sprms.GetByteArray(WordSprmOptions.sprmCBrc);
                return new BorderCode(iRes, 0);

            }
            set
            {
                byte[] iRes = new byte[4];
                value.Save(iRes, 0);
                Sprms.SetValue(WordSprmOptions.sprmCBrc, iRes);
            }
        }
        /// <summary>
        /// Gets/sets coping of sprms or not
        /// </summary>
        internal bool StickProperties
        {
            get
            {
                return m_stickProperties;
            }
            set
            {
                m_stickProperties = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal bool Special
        {
            get
            {
                bool defVal = (m_baseProps != null) ? m_baseProps.Special : false;
                return Sprms.GetBoolean(WordSprmOptions.sprmCFSpec, defVal);
            }
            set
            {
                if (value)
                {
                    Sprms.SetValue(WordSprmOptions.sprmCFSpec, value);
                }
                else
                {
                    Sprms.RemoveValue(WordSprmOptions.sprmCFSpec);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal SymbolDescriptor Symbol
        {
            get
            {
                byte[] operand = Sprms.GetByteArray(WordSprmOptions.sprmCSymbol);
                SymbolDescriptor symbol = new SymbolDescriptor();
                if (operand != null)
                {
                    symbol.Parse(operand);
                }
                return symbol;

            }
            set
            {
                byte[] operand = value.Save();
                Sprms.SetValue(WordSprmOptions.sprmCSymbol, operand);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal byte HiddenComplex
        {
            get
            {
                byte defVal = (m_baseProps != null) ? m_baseProps.HiddenComplex : (byte)0;
                return Sprms.GetByte(WordSprmOptions.sprmCFVanish, defVal);
            }
            set
            {
                if (value != byte.MaxValue)
                {
                    Sprms.SetValue(WordSprmOptions.sprmCFVanish, value);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal byte DoubleStrikeComplex
        {
            get
            {
                byte defVal = (m_baseProps != null) ? m_baseProps.DoubleStrikeComplex : (byte)0;
                return Sprms.GetByte(WordSprmOptions.sprmCFDStrike, defVal);
            }
            set
            {
                if (value != byte.MaxValue)
                {
                    Sprms.SetValue(WordSprmOptions.sprmCFDStrike, value);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal byte SmallCapsComplex
        {
            get
            {
                byte defVal = (m_baseProps != null) ? m_baseProps.SmallCapsComplex : (byte)0;
                return Sprms.GetByte(WordSprmOptions.sprmCFSmallCaps, defVal);
            }
            set
            {
                if (value != byte.MaxValue)
                {
                    Sprms.SetValue(WordSprmOptions.sprmCFSmallCaps, value);
                }
            }
        }
        /// <summary>
        /// Gets/sets Bold property
        /// </summary>
        internal bool FldVanish
        {
            get
            {
                bool defVal = (m_baseProps != null) ? m_baseProps.FldVanish : false;
                return Sprms.GetBoolean(WordSprmOptions.sprmCFFldVanish, defVal);
            }
            set
            {
                Sprms.SetValue(WordSprmOptions.sprmCFFldVanish, value);
            }
        }
        /// <summary>
        /// Gets/sets Bold property
        /// </summary>
        internal byte FldVanishComplex
        {
            get
            {
                byte defVal = ( m_baseProps != null ) ? m_baseProps.FldVanishComplex : ( byte )0;
                return Sprms.GetByte( WordSprmOptions.sprmCFFldVanish, defVal );
            }
            set
            {
                if( value != byte.MaxValue )
                {
                    Sprms.SetValue( WordSprmOptions.sprmCFFldVanish, value );
                }
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether no proof.
        /// </summary>
        /// <value>if it is no proof, set to <c>true</c>.</value>
        internal bool NoProof
        {
            get
            {
                bool defVal = (m_baseProps != null) ? m_baseProps.NoProof : false;
                return Sprms.GetBoolean(WordSprmOptions.sprmCFNoProof, defVal);
            }
            set
            {
                Sprms.SetValue(WordSprmOptions.sprmCFNoProof, value);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal byte IdctHint
        {
            get
            {
                byte defVal = (m_baseProps != null) ? m_baseProps.IdctHint : (byte)0;
                return Sprms.GetByte(WordSprmOptions.sprmCIdctHint, defVal);
            }
            set
            {
                Sprms.SetValue(WordSprmOptions.sprmCIdctHint, value);
            }
        }
        /// <summary>
        /// Gets a value indicating whether this instance is inserted change.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is inserted change; otherwise, <c>false</c>.
        /// </value>
        internal bool IsInsertRevision
        {
            get
            {
                bool defVal = (m_baseProps != null) ? m_baseProps.IsInsertRevision : false;
                return Sprms.GetBoolean(WordSprmOptions.sprmCFRMark, defVal);
            }
            set
            {
                Sprms.SetValue(WordSprmOptions.sprmCFRMark, value);
            }
        }
        /// <summary>
        /// Gets a value indicating whether this instance is deleted change.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is deleted change; otherwise, <c>false</c>.
        /// </value>
        internal bool IsDeleteRevision
        {
            get
            {
                bool defVal = (m_baseProps != null) ? m_baseProps.IsDeleteRevision : false;
                return Sprms.GetBoolean(WordSprmOptions.sprmCFRMarkDel, defVal);
            }
            set
            {
                Sprms.SetValue(WordSprmOptions.sprmCFRMarkDel, value);
            }
        }
        /// <summary>
        /// Defines whether formatting was changed.
        /// </summary>
        internal bool IsChangedFormat
        {
            get
            {
                bool defVal = (m_baseProps != null) ? m_baseProps.IsChangedFormat : false;
                byte[] data = Sprms.GetByteArray(WordSprmOptions.sprmCPropRMark);
                if (data == null)
                {
                    data = Sprms.GetByteArray(WordSprmOptions.sprmCPropRMark1);
                    if (data != null)
                    {
                        return (data[0] == 1);
                    }
                }
                return false;
            }
            set
            {
                byte[] res = new byte[7];
                res[0] = 1;
                Sprms.SetValue(WordSprmOptions.sprmCPropRMark1, res);
            }
        }
        /// <summary>
        /// Gets or sets the index of the list pictture.
        /// </summary>
        /// <value>The index of the list pictture.</value>
        internal int ListPictureIndex
        {
            get
            {
                bool hasPic = ListHasImage;
                int index = int.MaxValue;
                if (hasPic)
                {
                    int defVal = (m_baseProps != null) ? m_baseProps.ListPictureIndex : int.MaxValue;
                    index = Sprms.GetInt(WordSprmOptions.sprmCPbiImageIndex, defVal);
                }
                return index;
            }
            set
            {
                if (value != int.MaxValue)
                {
                    Sprms.SetValue(WordSprmOptions.sprmCPbiImageIndex, value);
                }
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [list has image].
        /// </summary>
        /// <value><c>true</c> if [list has image]; otherwise, <c>false</c>.</value>
        internal bool ListHasImage
        {
            get
            {
                bool defVal = (m_baseProps != null) ? m_baseProps.ListHasImage : false;
                return Sprms.GetBoolean(WordSprmOptions.sprmCPbiHasImage, defVal);
            }
            set
            {
                Sprms.SetValue(WordSprmOptions.sprmCPbiHasImage, value);
            }
        }
        #endregion

        #region Class properties/bidi
        /// <summary>
        /// Gets/sets Bold property
        /// </summary>
        internal bool BoldBi
        {
            get
            {
                bool styleValue = (m_baseProps != null)
                  ? m_baseProps.Sprms.GetBoolean(WordSprmOptions.sprmCFBoldBi, false)
                  : false;
                byte defVal = 0;
                return GetComplexBoolean(Sprms.GetByte(WordSprmOptions.sprmCFBoldBi, defVal), styleValue);
                //        bool defVal = ( m_baseProps != null ) ? m_baseProps.Bold : false;
                //        return Sprms.GetBoolean( WordSprmOptions.sprmCFBold, defVal );
            }
            set
            {
                Sprms.SetValue(WordSprmOptions.sprmCFBoldBi, value);
            }
        }
        /// <summary>
        /// Gets/sets Italic property
        /// </summary>
        internal bool ItalicBi
        {
            get
            {
                bool defVal = (m_baseProps != null) ? m_baseProps.ItalicBi : false;
                return Sprms.GetBoolean(WordSprmOptions.sprmCFItalicBi, defVal);
            }
            set
            {
                Sprms.SetValue(WordSprmOptions.sprmCFItalicBi, value);
            }
        }
        /// <summary>
        /// Gets/sets Bold property
        /// </summary>
        internal bool Bidi
        {
            get
            {
                bool defVal = (m_baseProps != null) ? m_baseProps.Bidi : false;
                return Sprms.GetBoolean(WordSprmOptions.sprmCFBiDi, defVal);
            }
            set
            {
                Sprms.SetValue(WordSprmOptions.sprmCFBiDi, value);
            }
        }
        /// <summary>
        /// Gets/sets FontSizeHP property
        /// </summary>
        internal ushort FontSizeBi
        {
            get
            {
                ushort defVal = (m_baseProps != null) ? m_baseProps.FontSizeBi : (ushort)1;
                return (ushort)(Sprms.GetUShort(WordSprmOptions.sprmCHpsBi, defVal) / 2);
            }
            set
            {
                // TODO: Checks if font is exists...
                Sprms.SetValue(WordSprmOptions.sprmCHpsBi, (ushort)(value * 2));
            }
        }
        //    /// <summary>
        //    /// Gets/sets FontColor property
        //    /// </summary>
        //    internal byte FontColorBi
        //    {
        //      get
        //      {
        //        byte defVal = ( m_baseProps != null ) ? m_baseProps.FontColorBi : ( byte )0;
        //        return Sprms.GetByte( WordSprmOptions.sprmCIcoBi, defVal );
        //      }
        //      set
        //      {
        //        Sprms.SetValue( WordSprmOptions.sprmCIcoBi, value );
        //      }
        //    }
        #endregion

        #region Class properties/special
        /// <summary>
        /// 
        /// </summary>
        internal WordStyleSheet StyleSheet
        {
            get
            {
                return m_styleSheet;
            }
            set
            {
                m_styleSheet = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal bool IsOle2
        {
            get
            {
                SinglePropertyModifierRecord sprmOle = Sprms[WordSprmOptions.sprmCFOle2];

                return (sprmOle != null && sprmOle.BoolValue);
            }
            set
            {
                Sprms.SetValue(WordSprmOptions.sprmCFOle2, value);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal bool IsData
        {
            get
            {
                return Sprms.GetBoolean(WordSprmOptions.sprmCFData, false);
            }
            set
            {
                Sprms.SetValue(WordSprmOptions.sprmCFData, value);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal ushort CharacterStyleId
        {
            get
            {
                return Sprms.GetUShort(WordSprmOptions.sprmCIstd, 0);
            }
            set
            {
                if (value != 0)
                {
                    Sprms.SetValue(WordSprmOptions.sprmCIstd, value);
                }
            }
        }
        #endregion

        #region Class properties/lid
        /// <summary>
        /// 
        /// </summary>
        internal short LocationIdASCII
        {
            get
            {
                return Sprms.GetShort(WordSprmOptions.sprmCRgLid3, 1033);
            }
            set
            {
                if (value != short.MaxValue)
                    Sprms.SetValue(WordSprmOptions.sprmCRgLid3, value);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal short LocationIdFarEast
        {
            get
            {
                return Sprms.GetShort(WordSprmOptions.sprmCRgLid3_2, 1033);
            }
            set
            {
                if (value != short.MaxValue)
                    Sprms.SetValue(WordSprmOptions.sprmCRgLid3_2, value);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal short RgLid3
        {
            get
            {
                return Sprms.GetShort(WordSprmOptions.sprmCRgLid3, 0);
            }
            set
            {
                if (value != short.MaxValue)
                    Sprms.SetValue(WordSprmOptions.sprmCRgLid3, value);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal short RgLid3_2
        {
            get
            {
                return Sprms.GetShort(WordSprmOptions.sprmCRgLid3_2, 0);
            }
            set
            {
                if (value != short.MaxValue)
                    Sprms.SetValue(WordSprmOptions.sprmCRgLid3_2, value);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal short Lid
        {
            get
            {
                return Sprms.GetShort(WordSprmOptions.sprmCLid, 0);
            }
            set
            {
                if (value != short.MaxValue)
                    Sprms.SetValue(WordSprmOptions.sprmCLid, value);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal short LidBi
        {
            get
            {
                return Sprms.GetShort(WordSprmOptions.sprmCLidBi, 0);
            }
            set
            {
                if (value != short.MaxValue)
                    Sprms.SetValue(WordSprmOptions.sprmCLidBi, value);
            }
        }
        #endregion

        #region Class internal methods
        /// <summary>
        /// Determines whether the specified option has options.
        /// </summary>
        /// <param name="option">The option.</param>
        /// <returns>
        /// if the specified option has options, set to <c>true</c>.
        /// </returns>
        internal bool HasOptions(int option)
        {
            return (Sprms[option] != null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal SinglePropertyModifierArray GetCopiableSprm()
        {
            SinglePropertyModifierArray cloned = new SinglePropertyModifierArray();
            int count = Sprms.Modifiers.Count;
            for (int i = 0; i < count; i++)
            {
                SinglePropertyModifierRecord record = Sprms.GetSprmByIndex(i);
                int options = record.TypedOptions;
                if (
                    // ==== Sharacter style
                  options != WordSprmOptions.sprmCIstd &&
                    // ==== Boolean
                  options != WordSprmOptions.sprmCFBold &&
                  options != WordSprmOptions.sprmCFItalic &&
                  options != WordSprmOptions.sprmCFStrike &&
                  options != WordSprmOptions.sprmCFVanish &&
                  options != WordSprmOptions.sprmCFShadow &&
                  options != WordSprmOptions.sprmCFEmboss &&
                  options != WordSprmOptions.sprmCFImprint &&
                  options != WordSprmOptions.sprmCFDStrike &&
                  options != WordSprmOptions.sprmCFCaps &&
                  options != WordSprmOptions.sprmCFSmallCaps &&
                  options != WordSprmOptions.sprmCFOutline &&
                  options != WordSprmOptions.sprmCFNoProof &&

                  // ==== Boolean bidi
                  options != WordSprmOptions.sprmCFBiDi &&
                  options != WordSprmOptions.sprmCFBoldBi &&
                  options != WordSprmOptions.sprmCFItalicBi &&

                  // ==== Enum, Float, Colors
                  options != WordSprmOptions.sprmCKul &&
                  options != WordSprmOptions.sprmCIss &&
                  options != WordSprmOptions.sprmCHps &&
                  options != WordSprmOptions.sprmCHpsBi &&
                  options != WordSprmOptions.sprmCHpsPos &&
                  options != WordSprmOptions.sprmCDxaSpace &&
                  options != WordSprmOptions.sprmCIco &&
                  options != WordSprmOptions.sprmCIcoe &&
                  options != WordSprmOptions.sprmCHighlight &&
                  options != WordSprmOptions.sprmCShd &&
                  options != WordSprmOptions.sprmCShdNew &&

                  // ==== Fonts, Borders, Languages
                  options != WordSprmOptions.sprmCRgFtc0 &&
                  options != WordSprmOptions.sprmCRgFtc1 &&
                  options != WordSprmOptions.sprmCRgFtc2 &&
                  options != WordSprmOptions.sprmCFtcBi &&
                  options != WordSprmOptions.sprmCBrc &&
                  options != WordSprmOptions.sprmCRgLid0 &&
                  options != WordSprmOptions.sprmCRgLid1 &&
                  options != WordSprmOptions.sprmCRgLid3 &&
                  options != WordSprmOptions.sprmCRgLid3_2 &&
                  options != WordSprmOptions.sprmCLid &&
                  options != WordSprmOptions.sprmCLidBi &&
                  options != WordSprmOptions.sprmCIdctHint &&

                  // ===== other filtered sprms
                  options != WordSprmOptions.sprmCFSpec &&
                  options != WordSprmOptions.sprmCPicLocation &&
                  options != WordSprmOptions.sprmCSymbol &&

                  options != WordSprmOptions.sprmNone &&
                  options != WordSprmOptions.sprmUnknown1 &&
                  options != WordSprmOptions.sprmUnknown2 &&
                  options != WordSprmOptions.sprmCUndocumentedSpacing &&
                  options != WordSprmOptions.sprmCUndocumentedRevisionProblem &&

                  // ===== Word 2000 undocumented sprms 
                  options != WordSprmOptions.sprmCUndocumented1 &&
                  options != WordSprmOptions.sprmCUndocumented2 &&
                  options != WordSprmOptions.sprmCUndocumented3 &&
                  options != WordSprmOptions.sprmCUndocumented4 &&
                  options != WordSprmOptions.sprmCUndocumented5 &&
                  options != WordSprmOptions.sprmCUndocumented6 &&
                  options != WordSprmOptions.sprmCUndocumented7 &&
                  options != WordSprmOptions.sprmCUndocumented8

        //          options != ( WordSprmOptions )12 &&
                    //          options != ( WordSprmOptions )0x10aa &&
                    //          options != ( WordSprmOptions )0x10 &&
                    //          options != ( WordSprmOptions )0xac00 &&
                    //          options != ( WordSprmOptions )0xc000 &&
                    //          options != ( WordSprmOptions )0x10d6 &&
                    //          options != ( WordSprmOptions )0xea00 &&
                    //          options != ( WordSprmOptions )0x10ec &&
                    //          options != ( WordSprmOptions )0xee00 &&
                    //          options != ( WordSprmOptions )0x10f8 &&

        //          options != ( WordSprmOptions )0xfa00 &&
                    //          options != ( WordSprmOptions )0x10fe &&
                    //          options != ( WordSprmOptions )0x11 &&
                    //          options != ( WordSprmOptions )0x1116 &&
                    //          options != ( WordSprmOptions )0x1800 &&
                    //          options != ( WordSprmOptions )0x1124 &&
                    //          options != ( WordSprmOptions )0x2800 &&
                    //          options != ( WordSprmOptions )0x113e &&
                    //          options != ( WordSprmOptions )0x4000 &&

        //          options != ( WordSprmOptions )0x4200 &&
                    //          options != ( WordSprmOptions )0x4c00 &&
                    //          options != ( WordSprmOptions )0x4e00 &&
                    //          options != ( WordSprmOptions )0x5200 &&
                    //          options != ( WordSprmOptions )0x8 &&
                    //          options != ( WordSprmOptions )0x862 &&
                    //          options != ( WordSprmOptions )0x8200 &&
                    //          options != ( WordSprmOptions )0x8400 &&
                    //          options != ( WordSprmOptions )0x9800 &&
                    //          options != ( WordSprmOptions )0x9a00 &&
                    //          options != ( WordSprmOptions )0x9c00 &&
                    //          options != ( WordSprmOptions )0xa600 &&
                    //          options != ( WordSprmOptions )0xa800 &&
                    //          options != ( WordSprmOptions )0xa800 &&
                    //          options != ( WordSprmOptions )0xcc00 &&
                    //          options != ( WordSprmOptions )0x8ee &&
                    //          options != ( WordSprmOptions )0x9 &&
                    //          options != ( WordSprmOptions )0x928 &&
                    //          options != ( WordSprmOptions )0x2a00 &&
                    //          options != ( WordSprmOptions )0x954 &&
                    //          options != ( WordSprmOptions )0x5600 &&
                    //          options != ( WordSprmOptions )0x7200 &&
                    //          options != ( WordSprmOptions )0x976 &&
                    //          options != ( WordSprmOptions )0x8cc0 &&
                    //          options != WordSprmOptions.sprmCFUsePgsuSettings &&
                    //          options != WordSprmOptions.sprmCHpsKern
                   )
                {
                    cloned.Modifiers.Add(record);
                }
            }
            return cloned;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        internal bool GetBoolean(SinglePropertyModifierRecord record)
        {
            if (m_baseProps != null)
            {
                bool styleValue = (m_baseProps != null)
                  ? m_baseProps.Sprms.GetBoolean(record.TypedOptions, false)
                  : false;
                return GetComplexBoolean(record.ByteValue, styleValue);
            }
            else
            {
                return record.BoolValue;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        internal string GetFontName(SinglePropertyModifierRecord record)
        {
            if (record.UshortValue >= m_styleSheet.FontNamesList.Count)
                return WordStyleSheet.DEF_FONT_NAME;

            return m_styleSheet.FontNamesList[record.UshortValue];
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        internal BorderCode GetBorder(SinglePropertyModifierRecord record)
        {
            return new BorderCode(record.ByteArray, 0);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        internal ShadingDescriptor GetShading(SinglePropertyModifierRecord record)
        {
            ShadingDescriptor shading;
            byte[] arr = record.ByteArray;

            if (arr.Length == 2)
            {
                shading = new ShadingDescriptor(record.ShortValue);
            }
            else
            {
                shading = new ShadingDescriptor();
                shading.ReadNewShd(arr, 0);
            }

            return shading;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        internal SymbolDescriptor GetSymbol(SinglePropertyModifierRecord record)
        {
            SymbolDescriptor symbol = new SymbolDescriptor();
            byte[] operand = record.ByteArray;

            if (operand != null)
            {
                symbol.Parse(operand);
            }

            return symbol;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        internal Color GetColor(SinglePropertyModifierRecord record)
        {

            uint defVal = (m_baseProps != null) ? WordColor.ConvertColorToRGB(m_baseProps.FontColorExt) : uint.MaxValue;
            uint intRes = record.UIntValue;
            if (intRes == uint.MaxValue)
            {
                return WordColor.ConvertIdToColor(FontColor);
            }
            else
            {
                return WordColor.ConvertRGBToColor(intRes);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fontName"></param>      
        internal void SetAllFontNames(string fontName)
        {
            int fontIndex = m_styleSheet.FontNameToIndex(fontName);

            if (fontIndex >= 0)
            {
                SinglePropertyModifierRecord record = new SinglePropertyModifierRecord(WordSprmOptions.sprmCRgFtc0);
                record.UshortValue = (ushort)fontIndex;
                Sprms.Add(record);

                record = new SinglePropertyModifierRecord(WordSprmOptions.sprmCRgFtc1);
                record.UshortValue = (ushort)fontIndex;
                Sprms.Add(record);

                record = new SinglePropertyModifierRecord(WordSprmOptions.sprmCRgFtc2);
                record.UshortValue = (ushort)fontIndex;
                Sprms.Add(record);
            }
            else
            {
                // TODO: Optimize later!!!!!!!!!
                fontIndex = (ushort)m_styleSheet.FontNamesList.Count;

                SinglePropertyModifierRecord record = new SinglePropertyModifierRecord(WordSprmOptions.sprmCRgFtc0);
                record.UshortValue = (ushort)fontIndex;
                Sprms.Add(record);

                SinglePropertyModifierRecord record2 = record.Clone();
                record2.TypedOptions = WordSprmOptions.sprmCRgFtc1;
                Sprms.Add(record2);

                SinglePropertyModifierRecord record3 = record.Clone();
                record3.TypedOptions = WordSprmOptions.sprmCRgFtc2;
                Sprms.Add(record3);

                m_styleSheet.UpdateFontName(fontName);
            }
        }
        /// <summary>
        /// Sets the name of the font.
        /// </summary>
        /// <param name="fontName">Name of the font.</param>
        /// <param name="option">The option.</param>
        internal void SetFontName(string fontName, int option)
        {
            int fontIndex = m_styleSheet.FontNameToIndex(fontName);

            if (fontIndex >= 0)
            {
                SinglePropertyModifierRecord record = new SinglePropertyModifierRecord(option);
                record.UshortValue = (ushort)fontIndex;
                Sprms.Add(record);
            }
            else
            {
                // TODO: Optimize later!!!!!!!!!
                fontIndex = (ushort)m_styleSheet.FontNamesList.Count;

                SinglePropertyModifierRecord record = new SinglePropertyModifierRecord(option);
                record.UshortValue = (ushort)fontIndex;
                Sprms.Add(record);

                m_styleSheet.UpdateFontName(fontName);
            }
        }
        /// <summary>
        /// Adds the SPRM.
        /// </summary>
        /// <param name="option">The option.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        internal void AddSprm(int option, bool value)
        {
            SinglePropertyModifierRecord record = new SinglePropertyModifierRecord(option);
            record.BoolValue = value;
            Sprms.Add(record);
        }
        /// <summary>
        /// Adds the SPRM.
        /// </summary>
        /// <param name="option">The option.</param>
        /// <param name="value">The value.</param>
        internal void AddSprm(int option, byte value)
        {
            SinglePropertyModifierRecord record = new SinglePropertyModifierRecord(option);
            record.ByteValue = value;
            Sprms.Add(record);
        }
        /// <summary>
        /// Adds the SPRM.
        /// </summary>
        /// <param name="option">The option.</param>
        /// <param name="value">The value.</param>
        internal void AddSprm(int option, ushort value)
        {
            SinglePropertyModifierRecord record = new SinglePropertyModifierRecord(option);
            record.UshortValue = value;
            Sprms.Add(record);
        }
        /// <summary>
        /// Adds the SPRM.
        /// </summary>
        /// <param name="option">The option.</param>
        /// <param name="value">The value.</param>
        internal void AddSprm(int option, short value)
        {
            SinglePropertyModifierRecord record = new SinglePropertyModifierRecord(option);
            record.ShortValue = value;
            Sprms.Add(record);
        }
        /// <summary>
        /// Adds the SPRM.
        /// </summary>
        /// <param name="option">The option.</param>
        /// <param name="value">The value.</param>
        internal void AddSprm(int option, int value)
        {
            SinglePropertyModifierRecord record = new SinglePropertyModifierRecord(option);
            record.IntValue = value;
            Sprms.Add(record);
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Updates base CharacterProperties.
        /// </summary>
        /// <param name="baseProps"></param>
        internal void UpdateBaseCharacterProperties(CharacterProperties baseProps)
        {
            if (baseProps == null)
            {
                throw new ArgumentNullException("baseProps");
            }
            m_baseProps = baseProps;

        }
        /// <summary>
        /// Clones internal chpx.
        /// </summary>
        internal CharacterPropertyException CloneChpx()
        {
            CharacterPropertyException cloneChpx = m_chpx;
            m_chpx = new CharacterPropertyException();

            if (StickProperties && cloneChpx != null)
            {
                for (int i = 0, len = cloneChpx.ModifiersCount; i < len; i++)
                {
                    if (cloneChpx.PropertyModifiers.GetSprmByIndex(i).Operand != null)
                    {
                        SinglePropertyModifierRecord sprm = cloneChpx.PropertyModifiers.GetSprmByIndex(i).Clone();
                        if (sprm != null)
                        {
                            m_chpx.PropertyModifiers.Add(sprm);
                        }
                    }
                }
            }

            return cloneChpx;
        }

        /// <summary>
        /// Removes the SPRM.
        /// </summary>
        /// <param name="option">The option.</param>
        internal void RemoveSprm(int option)
        {
            List<SinglePropertyModifierRecord> modifiers = Sprms.Modifiers;

            for (int i = 0, count = modifiers.Count; i < count; i++)
            {
                if (modifiers[i].TypedOptions == option)
                {
                    modifiers.RemoveAt(i);
                    return;
                }
            }
        }
        /// <summary>
        /// Determines whether this instance has Single Property Modifier Record Array.
        /// </summary>
        /// <returns>
        /// if this instance has Single Property Modifier Record Array, set to <c>true</c>.
        /// </returns>
        internal bool HasSprms()
        {
            return (m_chpx == null) ? false : m_chpx.HasSprms();
        }
        /// <summary>
        /// Gets the new (added when track changes property is on) Single Property Modifier Record.
        /// </summary>
        /// <param name="option">The option.</param>
        /// <returns></returns>
        internal SinglePropertyModifierRecord GetNewSprm(int option)
        {
            SinglePropertyModifierRecord sprm = null;

            int index = GetNewPropsStartIndex();
            if (index == -1)
                return sprm;

            for (int i = index, cnt = m_chpx.ModifiersCount; i < cnt; i++)
            {
                sprm = m_chpx.PropertyModifiers.GetSprmByIndex(i);
                if (sprm.OptionType == (WordSprmOptionType)option)
                {
                    return sprm;
                }
            }

            return null;
        }
        /// <summary>
        /// Gets the index of the new (changed) character properties.
        /// </summary>
        private int GetNewPropsStartIndex()
        {
            SinglePropertyModifierRecord sprm = m_chpx.PropertyModifiers[WordSprmOptions.sprmCWall];
            if (sprm != null)
            {
                return m_chpx.PropertyModifiers.Modifiers.IndexOf(sprm) + 1;
            }
            return -1;
        }

        /// <summary>
        /// Converts color to word color format
        /// </summary>
        /// <param name="brg"></param>
        /// <returns></returns>
        private int ConvertColor(int brg)
        {
            byte[] rgb = BitConverter.GetBytes(brg);
            byte tmp;
            tmp = rgb[0];
            rgb[0] = rgb[2];
            rgb[2] = tmp;
            return BitConverter.ToInt32(rgb, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sprmValue"></param>
        /// <param name="styleSheetValue"></param>
        /// <returns></returns>
        private static bool GetComplexBoolean(byte sprmValue, bool styleSheetValue)
        {
            if (sprmValue < 128)
            {
                return (sprmValue == 1);
            }
            else if (sprmValue == 128)
            {
                return styleSheetValue;
            }
            else if (sprmValue == 129)
            {
                return !styleSheetValue;
            }
            else
            {
                throw new Exception("Complex boolean value is expected.");
            }
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
#if DEBUG_TRACE
            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            builder.AppendLine("All Caps : " + this.AllCaps.ToString());
            builder.AppendLine("AllCapsComplex : " + this.AllCapsComplex.ToString());
            builder.AppendLine("Bidi : " + this.Bidi.ToString());
            builder.AppendLine("Bold : " + this.Bold.ToString());
            builder.AppendLine("BoldBi : " + this.BoldBi.ToString());
            builder.AppendLine("BoldComplex : " + this.BoldComplex.ToString());
            builder.AppendLine("DoubleStrike : " + this.DoubleStrike.ToString());
            builder.AppendLine("DoubleStrikeComplex : " + this.DoubleStrikeComplex.ToString());
            builder.AppendLine("Emboss : " + this.Emboss.ToString());
            builder.AppendLine("EmbossComplex : " + this.EmbossComplex.ToString());
            builder.AppendLine("Engrave : " + this.Engrave.ToString());
            builder.AppendLine("EngraveComplex : " + this.EngraveComplex.ToString());
            builder.AppendLine("FldVanish : " + this.FldVanish.ToString());
            builder.AppendLine("CharacterStyleId : " + this.CharacterStyleId.ToString());
            builder.AppendLine("FontColor : " + this.FontColor.ToString());
            builder.AppendLine("FontBi : " + this.FontBi.ToString());
            builder.AppendLine("FontColorExt : " + this.FontColorExt.ToString());
            builder.AppendLine("FontColorRGB : " + this.FontColorRGB.ToString());
            builder.AppendLine("FontFarEast : " + this.FontFarEast.ToString());
            builder.AppendLine("FontName : " + this.FontName.ToString());
            builder.AppendLine("FontNameAscii : " + this.FontNameAscii.ToString());
            builder.AppendLine("FontNameBi : " + this.FontNameBi.ToString());
            builder.AppendLine("FontNameFarEast : " + this.FontNameFarEast.ToString());
            builder.AppendLine("FontNameNonFarEast : " + this.FontNameNonFarEast.ToString());
            builder.AppendLine("FontNonFarEast : " + this.FontNonFarEast.ToString());
            builder.AppendLine("FontSize : " + this.FontSize.ToString());
            builder.AppendLine("FontSizeBi : " + this.FontSizeBi.ToString());
            builder.AppendLine("FontSizeHP : " + this.FontSizeHP.ToString());
            builder.AppendLine("HasSprms : " + this.HasSprms().ToString());
            builder.AppendLine("Hidden : " + this.Hidden.ToString());
            builder.AppendLine("HiddenComplex : " + this.HiddenComplex.ToString());
            builder.AppendLine("HighlightColor : " + this.HighlightColor.ToString());
            builder.AppendLine("IdctHint : " + this.IdctHint.ToString());
            builder.AppendLine("IsData : " + this.IsData.ToString());
            builder.AppendLine("IsDeleteRevision : " + this.IsDeleteRevision.ToString());
            builder.AppendLine("IsInsertRevision : " + this.IsInsertRevision.ToString());
            builder.AppendLine("IsOle2 : " + this.IsOle2.ToString());
            builder.AppendLine("Italic : " + this.Italic.ToString());
            builder.AppendLine("ItalicBi : " + this.ItalicBi.ToString());
            builder.AppendLine("ItalicComplex : " + this.ItalicComplex.ToString());
            builder.AppendLine("Lid : " + this.Lid.ToString());
            builder.AppendLine("LidBi : " + this.LidBi.ToString());
            builder.AppendLine("LineSpacing : " + this.LineSpacing.ToString());
            builder.AppendLine("ListHasImage : " + this.ListHasImage.ToString());
            builder.AppendLine("ListPictureIndex : " + this.ListPictureIndex.ToString());
            builder.AppendLine("NoProof : " + this.NoProof.ToString());
            builder.AppendLine("Outline : " + this.Outline.ToString());
            builder.AppendLine("PicLocation : " + this.PicLocation.ToString());
            builder.AppendLine("Position : " + this.Position.ToString());
            builder.AppendLine("RgLid0 : " + this.RgLid0.ToString());
            builder.AppendLine("RgLid1 : " + this.RgLid1.ToString());
            builder.AppendLine("RgLid3 : " + this.RgLid3.ToString());
            builder.AppendLine("RgLid3_2 : " + this.RgLid3_2.ToString());
            builder.AppendLine("Shading : " + this.Shading.ToString());
            //builder.AppendLine("ShadingNew : " + this.ShadingNew.ToString());
            builder.AppendLine("Shadow : " + this.Shadow.ToString());
            builder.AppendLine("ShadowComplex : " + this.ShadowComplex.ToString());
            builder.AppendLine("SmallCaps : " + this.SmallCaps.ToString());
            builder.AppendLine("SmallCapsComplex : " + this.SmallCapsComplex.ToString());
            builder.AppendLine("Sprms : " + this.Sprms.Count.ToString());
            builder.AppendLine("StickProperties : " + this.StickProperties.ToString());
            builder.AppendLine("Strike : " + this.Strike.ToString());
            builder.AppendLine("StrikeComplex : " + this.StrikeComplex.ToString());
            builder.AppendLine("StyleSheet : " + this.StyleSheet.ToString());
            builder.AppendLine("SubSuperScript : " + this.SubSuperScript.ToString());
            builder.AppendLine("Symbol : " + this.Symbol.ToString());
            builder.AppendLine("UnderlineCode : " + this.UnderlineCode.ToString());

            return builder.ToString();
#else
            return base.ToString();
#endif
        }
        #endregion
    }
}