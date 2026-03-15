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
using System.Collections.Generic;
using System.Collections.Specialized;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO.DLS.XML;
using Syncfusion.DocIO.ReaderWriter;
using Syncfusion.DocIO.ReaderWriter.Biff_Records;
using Syncfusion.Documentation;
using Syncfusion.DocIO.DLS.Entities;
#if !WINRT && !WP
using System.Drawing;
#endif
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// WCharacterFormat is used for representation of Character properties. 
    /// </summary>
    [DocumentationExclude()]
    public class WCharacterFormat : FormatBase
    {
        #region Class constants
        /// <summary>
        /// 
        /// </summary>
        internal const string DEF_FONTFAMILY = "Times New Roman";
        internal const float DEF_FONTSIZE = 10f;
        internal const byte DEF_NEGCOMPLEX_VALUE = 129;
        internal const byte DEF_POSCOMPLEX_VALUE = 128;
        /// <summary>
        /// Keys.
        /// </summary>
        internal const short FontKey = 0;
        internal const short TextColorKey = 1;
        internal const short FontNameKey = 2;
        internal const short FontSizeKey = 3;
        internal const short BoldKey = 4;
        internal const short ItalicKey = 5;
        internal const short StrikeKey = 6;
        internal const short UnderlineKey = 7;
        internal const short TextBkgColorKey = 9;
        internal const short SubSuperScriptKey = 10;
        internal const short DoubleStrikeKey = 14;
        internal const short PositionKey = 17;
        internal const short SpacingKey = 18;
        internal const short LineBreakKey = 20;
        internal const short ShadowKey = 50;
        internal const short EmbossKey = 51;
        internal const short EngraveKey = 52;
        internal const short HiddenKey = 53;
        internal const short AllCapsKey = 54;
        internal const short SmallCapsKey = 55;
        //public const int ShadingKey = 56;
        //    public const int DataKey = 57;
        internal const short BidiKey = 58;
        internal const short BoldBidiKey = 59;
        internal const short ItalicBidiKey = 60;
        internal const short FontNameBidiKey = 61;
        internal const short FontSizeBidiKey = 62;
        internal const short HighlightColorKey = 63;
        internal const short BorderKey = 67;
        internal const short FontNameAsciiKey = 68;
        internal const short FontNameFarEastKey = 69;
        internal const short FontNameNonFarEastKey = 70;
        internal const short OutlineKey = 71;

        internal const short IdctHintKey = 72;
        internal const short LocaleIdASCIIKey = 73;
        internal const short LocaleIdFarEastKey = 74;
        internal const short RgLid3Key = 75;
        internal const short RgLid3_2Key = 76;
        internal const short LidKey = 77;
        internal const short LidBiKey = 78;
        internal const short NoProofKey = 79;
        internal const short ForeColorKey = 80;
        internal const short TextureStyleKey = 81;
        internal const short FieldVanishKey = 109;
        internal const short FieldVanishCompKey = 110;
        internal const short PicLocationKey = 111;

        //Complex Keys
        internal const short ComplexScriptKey = 99;
        internal const short ItalicComplexKey = 65;
        internal const short BoldComplexKey = 64;
        internal const short HiddenComplexKey = 66;
        internal const short EngraveComplexKey = 82;
        internal const short EmbossComplexKey = 83;
        internal const short ShadowComplexKey = 84;
        internal const short StrikeComplexKey = 85;
        internal const short SmallCapsComplexKey = 86;
        internal const short DoubleStrikeComplexKey = 87;
        internal const short AllCapsComplexKey = 88;
        internal const short TextColorExtKey = 89;

        internal const short TextBkgColorNewKey = 100;
        internal const short ForeColorNewKey = 101;
        internal const short TextureStyleNewKey = 102;
        internal const short InserteRevisionKey = 103;
        internal const short DeleteRevisionKey = 104;
        internal const short ChangedFormatKey = 105;

        internal const short SpecialKey = 106;
        internal const short ListPicIndexKey = 107;
        internal const short ListHasPicKey = 108;

        // Word-2010 specific properties key
        internal const short ContextualAlternatesKey = 120;
        internal const short LigaturesKey = 121;
        internal const short NumberFormKey = 122;
        internal const short NumberSpacingKey = 123;
        internal const short StylisticSetKey = 124;
        #endregion

        #region Class members
        /// <summary>
        /// 
        /// </summary>
        protected string m_charStyleName = null;
        /// <summary>
        /// 
        /// </summary>
        private string m_newCharStyleName;
        private WCharacterFormat m_tableStyleCharacterFormat;
        /// <summary>
        /// 
        /// </summary>
        private CharacterProperties m_charProps;
        /// <summary>
        /// 
        /// </summary>
        private bool m_cancelOnChange;
        /// <summary>
        /// During mail merge single object of character properties is used.
        /// Field defines whether current CharacterFormat resets to this
        /// character properties.
        /// </summary>
        internal bool m_hasClonedProps;
        /// <summary>
        /// Flag which defines whether character format passed test on cross
        /// reference. 
        /// </summary>
        internal bool m_crossRefChecked;
        #endregion

        #region Class properties
#if !SILVERLIGHT && !WP 
        public System.Drawing.Font Font
        {
            get
            {
                FontStyle style = FontStyle.Regular;
                float fontSize = (FontSize == 0) ? 0.5f : FontSize;
                if (Bold)
                {
                    style |= FontStyle.Bold;
                }
                if (Italic)
                {
                    style |= FontStyle.Italic;
                }
                if (UnderlineStyle != UnderlineStyle.None)
                {
                    style |= FontStyle.Underline;
                }
                if (Strikeout)
                {
                    style |= FontStyle.Strikeout;
                }
                try
                {
                    // NOTE: Here we can cache Font object at least during layout.
                    return new System.Drawing.Font(FontName, fontSize, style);
                }
                catch(Exception ex)
                {
                    FontFamily fontFamily = new FontFamily(FontName);
                    if (fontFamily.IsStyleAvailable(FontStyle.Bold))
                        style |= FontStyle.Bold;
                    if (fontFamily.IsStyleAvailable(FontStyle.Italic))
                        style |= FontStyle.Italic;
                    if (fontFamily.IsStyleAvailable(FontStyle.Underline))
                        style |= FontStyle.Underline;
                    if (fontFamily.IsStyleAvailable(FontStyle.Strikeout))
                        style |= FontStyle.Strikeout;

                    return new System.Drawing.Font(FontName, fontSize, style);
                }
            }
            set  
            {
                FontName = value.Name;
                FontSize = value.SizeInPoints;
                Bold = value.Bold;
                Italic = value.Italic;
                Strikeout = value.Strikeout;
                UnderlineStyle = value.Underline ? UnderlineStyle.Single : UnderlineStyle.None;            
            }
        }
#else

        /// <summary>
        /// Gets / sets font 
        /// </summary>
        public Font Font
        {
            get
            {

                Font font = new Font(FontName, FontSize);
                font.Italic = Italic;
                font.Bold = Bold;
                font.UnderlineStyle = UnderlineStyle;
                font.Strikeout = Strikeout;
                return font;

            }
            set
            {
                FontName = value.FontFamilyName;
                FontSize = value.SizeInPoints;
                Bold = value.Bold;
                Italic = value.Italic;
                Strikeout = value.Strikeout;
                UnderlineStyle = value.Underline ? UnderlineStyle.Single : UnderlineStyle.None;
            }
        }
#endif
        /// <summary>
        /// Gets / sets font name
        /// </summary>
        public string FontName
        {
            get
            {
                return GetFontName(FontNameKey); // ( string )this[ FontNameKey ];
            }
            set
            {
                this[FontNameKey] = value;
                if (!HasValue(FontNameAsciiKey))
                    FontNameAscii = value;
                if (!HasValue(FontNameBidiKey))
                    FontNameBidi = value;
                if (!HasValue(FontNameFarEastKey))
                    FontNameFarEast = value;
                if (!HasValue(FontNameNonFarEastKey))
                    FontNameNonFarEast = value;
                CheckCrossRef();
                SetPropUpdateFlag(FontNameKey);
# if !SILVERLIGHT && !WP
                UpdateUsedFontsCollection();
#endif
            }
        }
        /// <summary>
        /// Gets / sets font size
        /// </summary>
        public float FontSize
        {
            get
            {
                return (float)GetPropertyValue(FontSizeKey);
            }
            set
            {
                SetPropertyValue(FontSizeKey, value);
            }
        }
        /// <summary>
        /// specifies whether the contents of this textrange shall be treated as complex script text regardless of their Unicode character values when determining the formatting for this textrange
        /// </summary>
        public bool ComplexScript
        {
            get
            {
                return GetBoolPropertyValue(ComplexScriptKey);
            }
            set
            {
                SetPropertyValue(ComplexScriptKey, value);
            }
        }
        /// <summary>
        /// Gets / sets bold style
        /// </summary>
        public bool Bold
        {
            get
            {
                return GetBoolPropertyValue(BoldKey);// ( bool )GetPropertyValue( BoldKey );
            }
            set
            {
                SetPropertyValue(BoldKey, value);
            }
        }
        /// <summary>
        /// Gets / sets italic style
        /// </summary>
        public bool Italic
        {
            get
            {
                return GetBoolPropertyValue(ItalicKey); //( bool )GetPropertyValue( ItalicKey );
            }
            set
            {
                SetPropertyValue(ItalicKey, value);
            }
        }
        /// <summary>
        /// Gets / sets strikeout style
        /// </summary>
        public bool Strikeout
        {
            get
            {
                return GetBoolPropertyValue(StrikeKey);//( bool )GetPropertyValue( StrikeKey );
            }
            set
            {
                SetPropertyValue(StrikeKey, value);
            }
        }
        /// <summary>
        /// Gets / sets doublestrikeout style
        /// </summary>
        /// <remarks>Not supported by Essential PDF</remarks>
        public bool DoubleStrike
        {
            get
            {
                return GetBoolPropertyValue(DoubleStrikeKey);//( bool )GetPropertyValue( DoubleStrikeKey );
            }
            set
            {
                SetPropertyValue(DoubleStrikeKey, value);
            }
        }
        /// <summary>
        /// Gets / sets underline style
        /// </summary>
        /// <remarks>Essential PDF only supports single underline.</remarks>
        public UnderlineStyle UnderlineStyle
        {
            get
            {
                return (UnderlineStyle)GetPropertyValue(UnderlineKey);
            }
            set
            {
                //Underline style DotDot property has been deprecated.Use None style instead of DotDot
                if (value == UnderlineStyle.DotDot)
                    value = UnderlineStyle.None;
                if ((value.ToString()).Length > 3)
                {
                    SetPropertyValue(UnderlineKey, value);
                }
            }
        }
        /// <summary>
        /// Gets / sets text color
        /// </summary>
        public Color TextColor
        {
            get
            {
                return (Color)GetPropertyValue(TextColorKey);
            }
            set
            {
                SetPropertyValue(TextColorKey, value);
            }
        }
        /// <summary>
        /// Gets / sets text background color
        /// </summary>
        /// <remarks>Not supported by Essential PDF</remarks>
        public Color TextBackgroundColor
        {
            get
            {
                return (Color)GetPropertyValue(TextBkgColorKey);
            }
            set
            {
                SetPropertyValue(TextBkgColorKey, value);
            }
        }
        /// <summary>
        /// Gets / sets subscript/superscript mode
        /// </summary>
        /// <remarks>Not supported by Essential PDF</remarks>
        public SubSuperScript SubSuperScript
        {
            get
            {
                return (SubSuperScript)GetPropertyValue(SubSuperScriptKey);
            }
            set
            {
                SetPropertyValue(SubSuperScriptKey, value);
            }
        }
        /// <summary>
        /// Gets / sets space width between characters.
        /// </summary>
        /// <remarks>Not supported by Essential PDF</remarks>
        public float CharacterSpacing
        {
            get
            {
                return (float)GetPropertyValue(SpacingKey);
            }
            set
            {
                SetPropertyValue(SpacingKey, value);
            }
        }
        /// <summary>
        /// Gets / sets text vertical position.
        /// </summary>
        /// <remarks>Not supported by Essential PDF</remarks>
        public float Position
        {
            get
            {
                return (float)GetPropertyValue(PositionKey);
            }
            set
            {
                SetPropertyValue(PositionKey, value);
            }
        }
        /// <summary>
        /// Gets / sets line break after.
        /// </summary>
        internal bool LineBreak
        {
            get
            {
                return IsLineBreakNext();
            }
            set
            {
                SetLineBreakNext();
            }
        }
        /// <summary>
        /// Gets/sets shadow property of text.
        /// </summary>
        public bool Shadow
        {
            get
            {
                return GetBoolPropertyValue(ShadowKey);// ( bool )GetPropertyValue( ShadowKey );
            }
            set
            {
                SetPropertyValue(ShadowKey, value);
            }
        }
        /// <summary>
        /// Gets/sets emboss property of text.
        /// </summary>
        public bool Emboss
        {
            get
            {
                return GetBoolPropertyValue(EmbossKey); //( bool )GetPropertyValue( EmbossKey );
            }
            set
            {
                SetPropertyValue(EmbossKey, value);
            }
        }
        /// <summary>
        /// Gets/sets Engrave property of text.
        /// </summary>
        public bool Engrave
        {
            get
            {
                return GetBoolPropertyValue(EngraveKey); //( bool )GetPropertyValue( EngraveKey );
            }
            set
            {
                SetPropertyValue(EngraveKey, value);
            }
        }
        /// <summary>
        /// Gets/sets Hidden property of text.
        /// </summary>
        public bool Hidden
        {
            get
            {
                return GetBoolPropertyValue(HiddenKey); //( bool )GetPropertyValue( HiddenKey );
            }
            set
            {
                SetPropertyValue(HiddenKey, value);
                //        HiddenComplex = byte.MaxValue;
            }
        }
        /// <summary>
        /// Gets/sets AllCaps property of text.
        /// </summary>
        public bool AllCaps
        {
            get
            {
                return GetBoolPropertyValue(AllCapsKey);// ( bool )GetPropertyValue( AllCapsKey );
            }
            set
            {
                SetPropertyValue(AllCapsKey, value);
            }
        }
        /// <summary>
        /// Gets/sets SmallCaps property of text.
        /// </summary>
        public bool SmallCaps
        {
            get
            {
                return GetBoolPropertyValue(SmallCapsKey); //( bool )GetPropertyValue( SmallCapsKey );
            }
            set
            {
                SetPropertyValue(SmallCapsKey, value);
            }
        }
        /// <summary>
        /// Gets / sets right-to-left property of text.
        /// </summary>
        public bool Bidi
        {
            get
            {
                return GetBoolPropertyValue(BidiKey); //( bool )GetPropertyValue( BidiKey );
            }
            set
            {
                SetPropertyValue(BidiKey, value);
            }
        }
        /// <summary>
        /// Gets / sets bold property for right-to-left text.
        /// </summary>
        public bool BoldBidi
        {
            get
            {
                return GetBoolPropertyValue(BoldBidiKey);// ( bool )GetPropertyValue( BoldBidiKey );
            }
            set
            {
                SetPropertyValue(BoldBidiKey, value);
            }
        }
        /// <summary>
        /// Gets / sets italic property for right-to-left text.
        /// </summary>
        public bool ItalicBidi
        {
            get
            {
                return GetBoolPropertyValue(ItalicBidiKey); //( bool )GetPropertyValue( ItalicBidiKey );
            }
            set
            {
                SetPropertyValue(ItalicBidiKey, value);
            }
        }
        /// <summary>
        /// Gets / sets font size of the right-to-left text.
        /// </summary>
        public float FontSizeBidi
        {
            get
            {
                return (float)GetPropertyValue(FontSizeBidiKey);
            }
            set
            {
                SetPropertyValue(FontSizeBidiKey, value);
            }
        }
        /// <summary>
        /// Gets / sets font name for right-to-left text.
        /// </summary>
        public string FontNameBidi
        {
            get
            {
                return (string)this[FontNameBidiKey];
            }
            set
            {
                this[FontNameBidiKey] = value;
                CheckCrossRef();
                SetPropUpdateFlag(FontNameBidiKey);
            }
        }
        /// <summary>
        /// Gets/sets highlight color of text.
        /// </summary>
        public Color HighlightColor
        {
            get
            {
                return (Color)GetPropertyValue(HighlightColorKey);
            }
            set
            {
                SetPropertyValue(HighlightColorKey, value);
            }
        }
        /// <summary>
        /// Gets border.
        /// </summary>
        public Border Border
        {
            get
            {
                return GetPropertyValue(BorderKey) as Border;
            }
        }
        /// <summary>
        /// Gets / sets the font used for Latin text (characters with character codes 
        /// from 0 through 127). 
        /// </summary>
        internal string FontNameAscii
        {
            get
            {
                return (string)this[FontNameAsciiKey];
            }
            set
            {
                this[FontNameAsciiKey] = value;
                SetPropUpdateFlag(FontNameAsciiKey);
            }
        }
        /// <summary>
        /// Gets / sets East Asian font name.
        /// </summary>
        internal string FontNameFarEast
        {
            get
            {
                return (string)this[FontNameFarEastKey];
            }
            set
            {
                this[FontNameFarEastKey] = value;
                SetPropUpdateFlag(FontNameFarEastKey);
            }
        }
        /// <summary>
        /// Gets / sets font used for characters with character codes from 128 through 255.
        /// </summary>
        internal string FontNameNonFarEast
        {
            get
            {
                return (string)this[FontNameNonFarEastKey];
            }
            set
            {
                this[FontNameNonFarEastKey] = value;
                SetPropUpdateFlag(FontNameNonFarEastKey);
            }
        }
        /// <summary>
        /// Get/set IdctHint property.
        /// </summary>
        internal FontHintType IdctHint
        {
            get
            {
                return (FontHintType)Convert.ToInt16(GetPropertyValue(IdctHintKey));
            }
            set
            {
                SetPropertyValue(IdctHintKey, value);
            }
        }
        /// <summary>
        /// Gets or sets the ASCII locale id .
        /// </summary>
        /// <value>The ASCII locale id .</value>
        public short LocaleIdASCII
        {
            get
            {
                //return (short)GetPropertyValue(LocaleIdASCIIKey);
                return GetLocateId(LocaleIdASCIIKey);
            }
            set
            {
                SetPropertyValue(LocaleIdASCIIKey, value);
            }
        }
        /// <summary>
        /// Gets or sets the far east locale id .
        /// </summary>
        /// <value>The far east locale id .</value>
        public short LocaleIdFarEast
        {
            get
            {
                //return (short)GetPropertyValue(LocaleIdFarEastKey);
                return GetLocateId(LocaleIdFarEastKey);
            }
            set
            {
                SetPropertyValue(LocaleIdFarEastKey, value);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal short RgLid3
        {
            get
            {
                return (short)GetPropertyValue(RgLid3Key);
            }
            set
            {
                SetPropertyValue(RgLid3Key, value);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal short RgLid3_2
        {
            get
            {
                return (short)GetPropertyValue(RgLid3_2Key);
            }
            set
            {
                SetPropertyValue(RgLid3_2Key, value);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal short Lid
        {
            get
            {
                return (short)GetPropertyValue(LidKey);
            }
            set
            {
                SetPropertyValue(LidKey, value);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal short LidBi
        {
            get
            {
                return (short)GetPropertyValue(LidBiKey);
            }
            set
            {
                SetPropertyValue(LidBiKey, value);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal bool NoProof
        {
            get
            {
                return (bool)GetPropertyValue(NoProofKey);
            }
            set
            {
                SetPropertyValue(NoProofKey, value);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal Color ForeColor
        {
            get
            {
                return (Color)GetPropertyValue(ForeColorKey);
            }
            set
            {
                SetPropertyValue(ForeColorKey, value);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal TextureStyle TextureStyle
        {
            get
            {
                return (TextureStyle)GetPropertyValue(TextureStyleKey);
            }
            set
            {
                SetPropertyValue(TextureStyleKey, value);
            }
        }
        /// <summary>
        /// Get/set outline character property.
        /// </summary>
        public bool OutLine
        {
            get
            {
                return GetBoolPropertyValue(OutlineKey);//( bool )GetPropertyValue( OutlineKey );
            }
            set
            {
                SetPropertyValue(OutlineKey, value);
            }
        }
        /// <summary>
        /// Gets / sets special style.
        /// </summary>
        internal bool Special
        {
            get
            {
                return GetBoolPropertyValue(SpecialKey);//( bool )GetPropertyValue( SpecialKey );
            }
            set
            {
                SetPropertyValue(SpecialKey, value);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public string CharStyleName
        {
            get
            {
                return m_charStyleName;
            }
            internal set
            {
                m_charStyleName = value;
                IsDefault = false;
                OnStateChange(this);
            }
        }
        /// <summary>
        /// Gets or sets the new name of the character style (style name after document change).
        /// </summary>
        /// <value>The new name of the char style.</value>
        internal string NewCharStyleName
        {
            get
            {
                return m_newCharStyleName;
            }
            set
            {
                m_newCharStyleName = value;
            }
        }
        /// <summary>
        /// Gets a value indicating whether this instance is inserted change.
        /// </summary>
        /// <value>
        /// 	if this instance is inserted change, set to <c>true</c>.
        /// </value>
        internal bool IsInsertRevision
        {
            get
            {
                return (bool)GetPropertyValue(InserteRevisionKey);
            }
            set
            {
                SetPropertyValue(InserteRevisionKey, value);
            }
        }
        /// <summary>
        /// Gets a value indicating whether this instance is deleted change.
        /// </summary>
        /// <value>
        /// 	if this instance is deleted change, set to <c>true</c>.
        /// </value>
        internal bool IsDeleteRevision
        {
            get
            {
                return (bool)GetPropertyValue(DeleteRevisionKey);
            }
            set
            {
                SetPropertyValue(DeleteRevisionKey, value);
            }
        }
        /// <summary>
        /// Gets a value indicating whether this instance has changed format.
        /// </summary>
        /// <value>
        /// 	if this instance has changed format, set to <c>true</c>.
        /// </value>
        internal bool IsChangedFormat
        {
            get
            {
                return (bool)GetPropertyValue(ChangedFormatKey);
            }
            set
            {
                if (value)
                    SetPropertyValue(ChangedFormatKey, value);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal CharacterProperties CharacterProps
        {
            get
            {
                CheckCharProps();
                return m_charProps;
            }
        }
        /// <summary>
        /// Gets or sets the SPRMS.
        /// </summary>
        /// <value>The SPRMS.</value>
        internal SinglePropertyModifierArray Sprms
        {
            get
            {
                return m_sprms;
            }
            set
            {
                m_charProps = null;
                m_sprms = value;
            }
        }
        /// <summary>
        /// Gets or sets the index of the list picture.
        /// </summary>
        /// <value>The index of the list picture.</value>
        internal int ListPictureIndex
        {
            get
            {
                return (int)GetPropertyValue(ListPicIndexKey);
            }
            set
            {
                SetPropertyValue(ListPicIndexKey, value);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal bool ListHasPicture
        {
            get
            {
                return (bool)GetPropertyValue(ListHasPicKey);
            }
            set
            {
                SetPropertyValue(ListHasPicKey, value);
            }
        }
        /// <summary>
        /// Gets the character style.
        /// </summary>
        /// <value>The character style.</value>
        internal CharacterStyle CharStyle
        {
            get
            {
                return GetCharStyle();
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether field should be vanished.
        /// </summary>
        /// <value><c>true</c> if field vanish; otherwise, <c>false</c>.</value>
        /// <returns></returns>
        internal bool FieldVanish
        {
            get
            {
                return GetBoolPropertyValue(FieldVanishKey);
            }
            set
            {
                SetPropertyValue(FieldVanishKey, value);
            }
        }
        /// <summary>
        /// Gets or sets the field vanish complex.
        /// </summary>
        /// <value>The field vanish complex.</value>
        internal byte FieldVanishComplex
        {
            get
            {
                return (byte)GetPropertyValue(FieldVanishCompKey);
            }
            set
            {
                SetPropertyValue(FieldVanishCompKey, value);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal int PicLocation
        {
            get
            {
                return (int)GetPropertyValue(PicLocationKey);
            }
            set
            {
                SetPropertyValue(PicLocationKey, value);
            }
        }
        /// <summary>
        /// Gets or sets the table style character format.
        /// </summary>
        /// <value>The table style character format.</value>
        internal WCharacterFormat TableStyleCharacterFormat
        {
            get
            {
                return m_tableStyleCharacterFormat;
            }
            set
            {
                m_tableStyleCharacterFormat = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether to use contextual alternates.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if use contextual alternates; otherwise, <c>false</c>.
        /// </value>
        public bool UseContextualAlternates
        {
            get
            {
                return (bool)GetPropertyValue(ContextualAlternatesKey);
            }
            set
            {
                SetPropertyValue(ContextualAlternatesKey, value);
            }
        }
        /// <summary>
        /// Gets or sets the ligatures type.
        /// </summary>
        /// <value>The ligatures.</value>
        public LigatureType Ligatures
        {
            get
            {
                return (LigatureType)GetPropertyValue(LigaturesKey);
            }
            set
            {
                SetPropertyValue(LigaturesKey, value);
            }
        }
        /// <summary>
        /// Gets or sets the number form type.
        /// </summary>
        /// <value>The number form.</value>
        public NumberFormType NumberForm
        {
            get
            {
                return (NumberFormType)GetPropertyValue(NumberFormKey);
            }
            set
            {
                SetPropertyValue(NumberFormKey, value);
            }
        }
        /// <summary>
        /// Gets or sets the number spacing type.
        /// </summary>
        /// <value>The number spacing.</value>
        public NumberSpacingType NumberSpacing
        {
            get
            {
                return (NumberSpacingType)GetPropertyValue(NumberSpacingKey);
            }
            set
            {
                SetPropertyValue(NumberSpacingKey, value);
            }
        }
        /// <summary>
        /// Gets or sets the stylistic set type.
        /// </summary>
        /// <value>The stylistic set.</value>
        public StylisticSetType StylisticSet
        {
            get
            {
                return (StylisticSetType)GetPropertyValue(StylisticSetKey);
            }
            set
            {
                SetPropertyValue(StylisticSetKey, value);
            }
        }
        #endregion

        #region Class initialize/finalize methods
        private WCharacterFormat()
        { }
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <summary>
        /// Default constructor
        /// </summary>
        public WCharacterFormat(IWordDocument doc)
            : base(doc)
        { }
        #endregion

        #region Implementation
        /// <summary>
        /// Determines whether line break is next to this paragraph item.
        /// </summary>
        /// <returns>
        /// 	if it specifies next element is line break, set to <c>true</c>.
        /// </returns>
        private bool IsLineBreakNext()
        {
            bool isLineBreakNext = false;
            OwnerHolder ownerParaItem = this.OwnerBase;
            if (ownerParaItem != null && ownerParaItem.OwnerBase is WParagraph)
            {
                WParagraph para = ownerParaItem.OwnerBase as WParagraph;
                int parItemIndex = para.Items.IndexOf(ownerParaItem as IEntity);
                if (parItemIndex < para.Items.Count - 1 && para.Items[parItemIndex + 1] is Break)
                {
                    Break nextBreak = para.Items[parItemIndex + 1] as Break;
                    isLineBreakNext = (nextBreak.BreakType == BreakType.LineBreak) ? true : false;
                }
            }
            return isLineBreakNext;
        }
        /// <summary>
        /// Sets the line break next.
        /// </summary>
        /// <returns></returns>
        private void SetLineBreakNext()
        {
            OwnerHolder ownerParaItem = this.OwnerBase;
            if (ownerParaItem != null && ownerParaItem.OwnerBase is WParagraph)
            {
                WParagraph para = ownerParaItem.OwnerBase as WParagraph;
                int lineBreakItemIndex = para.Items.IndexOf(ownerParaItem as IEntity) + 1;
                para.Items.Insert(lineBreakItemIndex, new Break(para.Document, BreakType.LineBreak));
            }
        }
        /// <summary>
        /// Checks the character properties object.
        /// </summary>
        private void CheckCharProps()
        {
            if (m_charProps == null)
            {
                if (m_sprms != null)
                {
                    m_charProps = new CharacterProperties(null);
                    m_charProps.CharacterPropertyException.PropertyModifiers = m_sprms;
                }
                else
                {
                    m_charProps = new CharacterProperties(null);
                    m_sprms = m_charProps.Sprms;
                }

                if (!Document.IsOpening)
                    m_crossRefChecked = true;
            }
        }
        /// <summary>
        /// Gets the property value.
        /// </summary>
        /// <param name="propKey">The prop key.</param>
        /// <returns></returns>
        internal object GetPropertyValue(int propKey)
        {
            UpdateCharFormat(propKey);
            return this[propKey];
        }
        /// <summary>
        /// Gets the boolean property value.
        /// </summary>
        /// <param name="propKey">The prop key.</param>
        /// <returns></returns>
        internal bool GetBoolPropertyValue(short propKey)
        {
            if (!HasKey(propKey) && !IsPropertyUpdated(propKey) || ContainsSprm(propKey))
            {
                CheckCharProps();
                SetPropUpdateFlag(propKey);
                if (ContainsSprm(propKey))
                    this[propKey] = GetComplexBoolValue(propKey);
            }
            bool value = (bool)this[propKey];
            if (m_tableStyleCharacterFormat != null && (bool)m_tableStyleCharacterFormat[propKey])
            {
                return !value;
            }
            return value;
        }
        /// <summary>
        /// Determines whether the specified option key contains SPRM.
        /// </summary>
        /// <param name="optionKey">The option key.</param>
        /// <returns>
        /// 	<c>true</c> if the specified option key contains SPRM; otherwise, <c>false</c>.
        /// </returns>
        private bool ContainsSprm(short optionKey)
        {
            if (m_sprms != null)
            {
                int sprmOptionKey = GetSprmOption(optionKey);
                if (m_sprms[sprmOptionKey] != null)
                    return true;
            }
            return false;
        }
        /// <summary>
        /// Sets the property value.
        /// </summary>
        /// <param name="propKey">The property key.</param>
        /// <param name="value">The value.</param>
        private void SetPropertyValue(int propKey, object value)
        {
            this[propKey] = value;
            UpdateCharProps(propKey, value);
            OnStateChange(this);
        }
        /// <summary>
        /// Updates the character properties.
        /// </summary>
        /// <param name="propertyKey">The property key.</param>
        /// <param name="value">The value.</param>
        private void UpdateCharProps(int propertyKey, object value)
        {
            CheckCrossRef();
            CheckCharProps();

            SetPropUpdateFlag(propertyKey);
            CharacterPropertiesConverter.FormatToProp(propertyKey, value, m_charProps, this);
        }
        /// <summary>
        /// Updates character format options.
        /// </summary>
        /// <param name="propertyKey">The property key.</param>
        private void UpdateCharFormat(int propertyKey)
        {
            if (HasKey(propertyKey) && IsPropertyUpdated(propertyKey))
                return;

            CheckCharProps();

            SetPropUpdateFlag(propertyKey);
            PropToFormat(propertyKey);
        }
        /// <summary>
        /// Updates character format options.
        /// </summary>
        /// <param name="propertyKey">The property key.</param>
        internal void UpdateCharacterFormat(int propertyKey)
        {
            if (IsBooleanProperty(propertyKey))
            {
                if (!HasKey(propertyKey) && !IsPropertyUpdated(propertyKey) || ContainsSprm((short)propertyKey))
                {
                    CheckCharProps();
                    SetPropUpdateFlag(propertyKey);
                    if (ContainsSprm((short)propertyKey))
                        this[propertyKey] = GetComplexBoolValue((short)propertyKey);
                }
            }
            else
                UpdateCharFormat(propertyKey);
        }
        /// <summary>
        /// Updates the changed format.
        /// </summary>
        private void UpdateChangedFormat()
        {
            SinglePropertyModifierRecord revsprm = m_charProps.Sprms[WordSprmOptions.sprmCPropRMark];

            if (revsprm == null)
                revsprm = m_charProps.Sprms[WordSprmOptions.sprmCPropRMark1];

            if (revsprm != null)
            {
                this[ChangedFormatKey] = m_charProps.IsChangedFormat;
            }
        }
        /// <summary>
        /// Determines whether the specified key is boolean property.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        /// 	<c>true</c> if the specified key is boolean property; otherwise, <c>false</c>.
        /// </returns>
        private bool IsBooleanProperty(int key)
        {
            switch (key)
            {
                case WCharacterFormat.ComplexScriptKey:
                case WCharacterFormat.BoldKey:
                case WCharacterFormat.ItalicKey:
                case WCharacterFormat.StrikeKey:
                case WCharacterFormat.DoubleStrikeKey:
                case WCharacterFormat.ShadowKey:
                case WCharacterFormat.EmbossKey:
                case WCharacterFormat.EngraveKey:
                case WCharacterFormat.HiddenKey:
                case WCharacterFormat.AllCapsKey:
                case WCharacterFormat.SmallCapsKey:
                case WCharacterFormat.BidiKey:
                case WCharacterFormat.BoldBidiKey:
                case WCharacterFormat.ItalicBidiKey:
                case WCharacterFormat.OutlineKey:
                case WCharacterFormat.SpecialKey:
                case WCharacterFormat.FieldVanishKey:
                    return true;
            }
            return false;
        }
        /// <summary>
        /// Sets the font names for character format.
        /// </summary>
        /// <param name="charProps">The character props.</param>
        internal void SetFontNames(CharacterProperties charProps)
        {
            SetFontName(WordSprmOptions.sprmCRgFtc0, FontNameAsciiKey, charProps);
            SetFontName(WordSprmOptions.sprmCRgFtc1, FontNameFarEastKey, charProps);
            SetFontName(WordSprmOptions.sprmCRgFtc2, FontNameNonFarEastKey, charProps);
            SetFontName(WordSprmOptions.sprmCFtcBi, FontNameBidiKey, charProps);

            if (HasKey(FontNameAsciiKey))
            {
                this[FontNameKey] = this[FontNameAsciiKey];
            }
        }
        /// <summary>
        /// Set the complex script value
        /// </summary>
        /// <param name="charProps"></param>
        internal void SetComplexScript(CharacterProperties charProps)
        {
            SinglePropertyModifierRecord sprm = charProps.Sprms[WordSprmOptions.sprmCFComplexScripts];
            if (sprm != null)
            {
                this[ComplexScriptKey] = charProps.ComplexScript;
            }
        }
        /// <summary>
        /// Set the language sprms
        /// </summary>
        /// <param name="charProps"></param>
        internal void SetLanguages(CharacterProperties charProps)
        {
            SetLanguageId(WordSprmOptions.sprmCLidBi, LidBiKey, charProps);
            SetLanguageId(WordSprmOptions.sprmCLid, LidKey, charProps);
            SetLanguageId(WordSprmOptions.sprmCRgLid3_2, LocaleIdFarEastKey, charProps);
        }
        /// <summary>
        /// Set the language sprms
        /// </summary>
        /// <param name="sprmOptions"></param>
        /// <param name="Key"></param>
        /// <param name="charProps"></param>
        private void SetLanguageId(int sprmOptions, short Key, CharacterProperties charProps)
        {
            SinglePropertyModifierRecord sprm = charProps.Sprms[sprmOptions];
            if (sprm != null)
            {
                this[Key] = sprm.ShortValue;
            }
        }
        /// <summary>
        /// Updates the name of the font.
        /// </summary>
        /// <param name="option">The option.</param>
        /// <param name="fontKey">The font key.</param>
        /// <param name="charProps">The character props.</param>
        private void SetFontName(int option, int fontKey, CharacterProperties charProps)
        {
            SinglePropertyModifierRecord sprm = charProps.Sprms[option];
            if (sprm != null)
            {
                this[fontKey] = charProps.GetFontName(sprm);
            }
        }
        /// <summary>
        /// Sets character properties' font names.
        /// </summary>
        /// <param name="styleSheet">The stylesheet.</param>
        internal void SetCharPropFontNames(WordStyleSheet styleSheet)
        {
            if (HasKey(FontNameKey))
                SetCharPropFontName(FontNameKey, styleSheet);
            if (HasKey(FontNameAsciiKey))
                SetCharPropFontName(FontNameAsciiKey, styleSheet);
            if (HasKey(FontNameFarEastKey))
                SetCharPropFontName(FontNameFarEastKey, styleSheet);
            if (HasKey(FontNameNonFarEastKey))
                SetCharPropFontName(FontNameNonFarEastKey, styleSheet);
            if (HasKey(FontNameBidiKey))
                SetCharPropFontName(FontNameBidiKey, styleSheet);
        }
        /// <summary>
        /// Sets character properties' font name.
        /// </summary>
        /// <param name="fontKey">The font key.</param>
        /// <param name="styleSheet">The stylesheet.</param>
        private void SetCharPropFontName(int fontKey, WordStyleSheet styleSheet)
        {
            //If font name was updated by user.
            CharacterProps.StyleSheet = styleSheet;
            CharacterPropertiesConverter.FormatToProp(fontKey, this[fontKey], m_charProps, this);
        }
        /// <summary>
        /// Chars the props has SPRMS.
        /// </summary>
        /// <returns></returns>
        internal bool CharPropsHasSprms()
        {
            return (m_charProps != null && m_charProps.Sprms.Count > 0) ? true : false;
        }
        /// <summary>
        /// Defines whether to serialize all data. 
        /// </summary>
        /// <returns></returns>
        private bool SerializeAllData()
        {
            if (Document == null)
                return false;

            if (m_sprms != null)
                return false;

            return true;
        }
        /// <summary>
        /// Gets complex value of Single Property Modifier Record.
        /// </summary>
        /// <param name="optionKey">The option key.</param>
        /// <returns></returns>
        internal byte GetComplexValue(short optionKey)
        {
            int sprmOptionKey = GetSprmOption(optionKey);

            try
            {
                if (m_sprms != null)
                {
                    SinglePropertyModifierRecord sprm = m_sprms[sprmOptionKey];
                    if (sprm != null)
                    {
                        return sprm.ByteValue;
                    }
                }
            }
            catch
            {
                return byte.MaxValue;
            }

            return byte.MaxValue;
        }
        /// <summary>
        /// Gets the complex bool value.
        /// </summary>
        /// <param name="optionKey">The option key.</param>
        /// <returns></returns>
        internal bool GetComplexBoolValue(short optionKey)
        {
            byte complexVal = GetComplexValue(optionKey);
            if (complexVal == DEF_NEGCOMPLEX_VALUE && this.BaseFormat != null)
            {
                return !GetComplexBoolValue(m_charStyleName, this.BaseFormat as WCharacterFormat, optionKey);
            }
            else if (complexVal == DEF_POSCOMPLEX_VALUE && this.BaseFormat != null)
            {
                return GetComplexBoolValue(m_charStyleName, this.BaseFormat as WCharacterFormat, optionKey);
            }
            else if (complexVal == DEF_NEGCOMPLEX_VALUE && this.BaseFormat == null)
            {
                return true;
            }
            else if (complexVal == 1)
            {
                return true;
            }
            else if (complexVal == byte.MaxValue && this.BaseFormat != null)
            {
                return GetComplexBoolValue(m_charStyleName, this.BaseFormat as WCharacterFormat, optionKey);
            }

            return false;
        }
        /// <summary>
        /// Gets the complex bool value.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="optionKey">The option key.</param>
        /// <returns></returns>
        private byte GetComplexBoolValue(WCharacterFormat format, short optionKey)
        {
            byte val = 0;

            if (format == null)
                return val;


            if (format.HasValue(optionKey))
            {
                bool boolVal = format.GetComplexBoolValue(optionKey);
                val = (byte)(boolVal ? 1 : 0);
            }

            return val;
        }
        /// <summary>
        /// Gets the complex bool value.
        /// </summary>
        /// <param name="chStyleName">Name of the character style.</param>
        /// <param name="pStyleForm">The character format if paragraph style.</param>
        /// <param name="optionKey">The option key.</param>
        /// <returns></returns>
        private bool GetComplexBoolValue(string chStyleName, WCharacterFormat pStyleForm, short optionKey)
        {
            byte val = 0;
            if (chStyleName != null)
            {
                CharacterStyle style = Document.Styles.FindByName(chStyleName, StyleType.CharacterStyle) as CharacterStyle;
                if (style != null)
                {
                    val = GetComplexBoolValue(style.CharacterFormat, optionKey);
                    if (val == DEF_NEGCOMPLEX_VALUE || val == 1)
                        return true;
                }
            }

            val = GetComplexBoolValue(pStyleForm, optionKey);
            return (val == DEF_NEGCOMPLEX_VALUE || val == 1);
        }
        /// <summary>
        /// Gets the base format.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns></returns>
        private WCharacterFormat GetBaseFormat(WCharacterFormat format)
        {
            if (format == null)
                return null;

            if (format.CharStyleName != null)
            {
                CharacterStyle style = Document.Styles.FindByName(format.CharStyleName, StyleType.CharacterStyle) as CharacterStyle;
                if (style != null)
                {
                    return style.CharacterFormat;
                }
            }

            return format.BaseFormat as WCharacterFormat;
        }
        /// <summary>
        /// Removes the format changes.
        /// </summary>
        internal override void RemoveChanges()
        {
            CheckCrossRef();
            base.RemoveChanges();
            if (m_sprms != null && m_sprms[WordSprmOptions.sprmCIstd] == null)
            {
                m_charStyleName = null;
                m_newCharStyleName = null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal override void AcceptChanges()
        {
            this[DeleteRevisionKey] = false;
            this[InserteRevisionKey] = false;
            this[ChangedFormatKey] = false;

            if (m_sprms != null && m_sprms.Length > 0)
            {
                CheckCrossRef();

                m_sprms.RemoveValue(WordSprmOptions.sprmCFRMark);
                m_sprms.RemoveValue(WordSprmOptions.sprmCFRMarkDel);
                m_sprms.RemoveValue(WordSprmOptions.sprmCIbstRMark);
                m_sprms.RemoveValue(WordSprmOptions.sprmCIbstRMarkDel);
                m_sprms.RemoveValue(WordSprmOptions.sprmCDttmRMark);
                m_sprms.RemoveValue(WordSprmOptions.sprmCDttmRMarkDel);
                m_sprms.RemoveValue(WordSprmOptions.sprmCPropRMark);
                m_sprms.RemoveValue(WordSprmOptions.sprmCPropRMark1);

                if (m_newCharStyleName != null && m_charStyleName != m_newCharStyleName)
                {
                    m_charStyleName = m_newCharStyleName;
                }

                Dictionary<int, object> fontNames = new Dictionary<int, object>();
                if (HasKey(FontNameAsciiKey))
                    fontNames.Add(FontNameAsciiKey, this[FontNameAsciiKey]);
                if (HasKey(FontNameBidiKey))
                    fontNames.Add(FontNameBidiKey, this[FontNameBidiKey]);
                if (HasKey(FontNameFarEastKey))
                    fontNames.Add(FontNameFarEastKey, this[FontNameFarEastKey]);
                if (HasKey(FontNameNonFarEastKey))
                    fontNames.Add(FontNameNonFarEastKey, this[FontNameNonFarEastKey]);
                //Updates the format to latest revision.
                base.AcceptChanges();
                //Update the font names based on sprms after revision mark.
                if (m_sprms.Contain(WordSprmOptions.sprmCRgFtc0)
                    && fontNames.ContainsKey(FontNameAsciiKey))
                {
                    this[FontNameAsciiKey] = fontNames[FontNameAsciiKey];
                    this[FontNameKey] = fontNames[FontNameAsciiKey];
                }
                if (m_sprms.Contain(WordSprmOptions.sprmCRgFtc1)
                    && fontNames.ContainsKey(FontNameFarEastKey))
                    this[FontNameFarEastKey] = fontNames[FontNameFarEastKey];
                if (m_sprms.Contain(WordSprmOptions.sprmCRgFtc2)
                    && fontNames.ContainsKey(FontNameNonFarEastKey))
                    this[FontNameNonFarEastKey] = fontNames[FontNameNonFarEastKey];
                if (m_sprms.Contain(WordSprmOptions.sprmCFtcBi)
                    && fontNames.ContainsKey(FontNameBidiKey))
                    this[FontNameBidiKey] = fontNames[FontNameBidiKey];
                fontNames.Clear();
            }
        }
        /// <summary>
        /// Checks whether character format sprms have cross reference
        /// </summary>
        /// <returns></returns>
        internal void CheckCrossRef()
        {
            if (Document.IsOpening)
                return;

            if (m_sprms == null)
                return;

            if (m_crossRefChecked)
                return;

            if (m_hasClonedProps)
            {
                m_hasClonedProps = false;
            }

            m_crossRefChecked = true;
            m_sprms = m_sprms.Clone();
            m_charProps = null;

            // ToDo: apply "deep" check for cross reference!      
            //      else if( this.OwnerBase != null && this.OwnerBase is IEntity )
            //      {
            //        IEntity ent = this.OwnerBase as IEntity;
            //        // Check previous paragraph item
            //        if( ent.PreviousSibling == null && ( ent is ParagraphItem ) && ( ent as ParagraphItem ).OwnerParagraph != null )
            //        {
            //          WParagraph prevPara = ( ent as ParagraphItem ).OwnerParagraph.PreviousSibling as WParagraph;
            //          if( prevPara != null && prevPara.Items.Count > 0 )
            //          {
            //            ent = prevPara.Items[ prevPara.Items.Count - 1 ] as IEntity;
            //            cloneSprms = HasSameSprms( ent );
            //          }
            //        }
            //        else
            //        {
            //          cloneSprms = HasSameSprms( ent.PreviousSibling );
            //        }
            //
            //        if( !cloneSprms )
            //        {
            //          // Check next paragraph item
            //          if( ent.NextSibling == null && ent is ParagraphItem && ( ent as ParagraphItem ).OwnerParagraph != null )
            //          {
            //            WParagraph nextPara = ( ent as ParagraphItem ).OwnerParagraph.NextTextBodyItem as WParagraph;
            //            if( nextPara != null && nextPara.Items.Count > 0 )
            //            {
            //              cloneSprms = HasSameSprms( nextPara.Items[ 0 ] as IEntity );
            //            }
            //          }
            //          else
            //          {
            //            cloneSprms = HasSameSprms( ent.NextSibling );
            //          }
            //        }
            //      }     

            //      if( cloneSprms )
            //      {
            //        m_sprms = m_sprms.Clone();       
            //        
            //      }      
        }
        /// <summary>
        /// Determines whether has same srms.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>
        /// 	if has same srpms, set to <c>true</c>.
        /// </returns>
        private bool HasSameSprms(IEntity entity)
        {
            if (entity is WParagraph)
            {
                if ((entity as WParagraph).BreakCharacterFormat.Sprms == m_sprms)
                    return true;
            }
            else if (entity is ParagraphItem)
            {
                WCharacterFormat format = (entity as ParagraphItem).GetCharFormat();
                if (format != null && format.Sprms == m_sprms)
                    return true;
            }

            return false;
        }
        /// <summary>
        /// Determines whether the specified option is complex.
        /// </summary>
        /// <param name="optionKey">The option key.</param>
        /// <returns>
        /// 	if the specified option is complex, set to <c>true</c>.
        /// </returns>
        internal bool IsComplex(short optionKey)
        {
            if (m_sprms != null)
            {
                int sprmOptionKey = GetSprmOption(optionKey);
                SinglePropertyModifierRecord sprm = m_sprms[sprmOptionKey];
                if (sprm != null)
                {
                    return (sprm.ByteValue >= 128) ? true : false;
                }
            }
            return false;
        }
        /// <summary>
        /// Gets the char style.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        private CharacterStyle GetCharStyle()
        {
            CharacterStyle style = null;
            if (!String.IsNullOrEmpty(m_charStyleName) && Document != null)
            {
                style = Document.Styles.FindByName(m_charStyleName, StyleType.CharacterStyle) as CharacterStyle;
            }

            return style;
        }
        /// <summary>
        /// Gets the name of the font.
        /// </summary>
        /// <param name="fontKey">The font key.</param>
        /// <returns></returns>
        internal string GetFontName(short fontKey)
        {
            return (string)this[fontKey];
        }
        /// <summary>
        /// Get LocateId
        /// </summary>
        /// <param name="locateIdKey">locate id key</param>
        /// <returns>LocateId</returns>
        private short GetLocateId(short locateIdKey)
        {
            if (!HasValue(locateIdKey))
            {
                WCharacterFormat baseFormat = (CharStyle != null && CharStyle.CharacterFormat.HasValue(locateIdKey)) ?
                  CharStyle.CharacterFormat : this.BaseFormat as WCharacterFormat;
                if (baseFormat != null)
                {
                    switch (locateIdKey)
                    {
                        case LocaleIdASCIIKey:
                            if (baseFormat.HasValue(LocaleIdASCIIKey))
                                this[locateIdKey] = baseFormat.GetPropertyValue(LocaleIdASCIIKey);
                            break;
                        case LocaleIdFarEastKey:
                            if (baseFormat.HasValue(LocaleIdFarEastKey))
                            {
                                short value = (short)baseFormat.GetPropertyValue(LocaleIdFarEastKey);
                                if (value == 1033 && baseFormat.Sprms.Contain(WordSprmOptions.sprmCLidBi))
                                    value = baseFormat.Sprms[WordSprmOptions.sprmCLidBi].ShortValue;
                                this[locateIdKey] = value;
                            }
                            break;
                    }
                }
            }
            if (HasValue(locateIdKey) && HasKey(locateIdKey))
                return (short)this[locateIdKey];
            else
                return (short)GetPropertyValue(locateIdKey);
        }
        /// <summary>
        /// Updates the default formats.
        /// Updates the document default formattings while cloning the document.
        /// </summary>
        internal void UpdateDefaultFormats()
        {
            if (m_doc.DefCharFormat == null)
                return;

            foreach (KeyValuePair<int, object> keyValuePair in m_doc.DefCharFormat.PropertiesHash)
            {
                if (keyValuePair.Key != BorderKey
                    && !this.ContainsValue(keyValuePair.Key))
                {
                    PropertiesHash.Add(keyValuePair.Key, keyValuePair.Value);

                    if (m_doc.DefCharFormat.Sprms != null)
                    {
                        int option = GetSprmOption(keyValuePair.Key);
                        SinglePropertyModifierRecord sprm = m_doc.DefCharFormat.Sprms[option];
                        if (sprm != null)
                        {
                            if (m_sprms == null)
                                m_sprms = new SinglePropertyModifierArray();
                            m_sprms.Add(sprm);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Determines whether the specified key contains value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        /// 	<c>true</c> if the specified key contains value; otherwise, <c>false</c>.
        /// </returns>
        internal bool ContainsValue(int key)
        {
            return (HasValue(key)
                || (CharStyle != null
                && CharStyle.CharacterFormat.HasValue(key))
                || (BaseFormat != null
                && (BaseFormat as WCharacterFormat).ContainsValue(key)));
        }
        /// <summary>
        /// Sets the default properties.
        /// </summary>
        internal void SetDefaultProperties()
        {
            // Only handled properties are set here as of now
            PropertiesHash.Add(WCharacterFormat.FontSizeKey, DEF_FONTSIZE);
            PropertiesHash.Add(WCharacterFormat.TextColorKey, Color.Empty);
            PropertiesHash.Add(WCharacterFormat.FontNameKey, DEF_FONTFAMILY);
            PropertiesHash.Add(WCharacterFormat.BoldKey, false);
            PropertiesHash.Add(WCharacterFormat.ItalicKey, false);
            PropertiesHash.Add(WCharacterFormat.UnderlineKey, UnderlineStyle.None);
            PropertiesHash.Add(WCharacterFormat.HighlightColorKey, Color.Empty);
            PropertiesHash.Add(WCharacterFormat.ShadowKey, false);
            PropertiesHash.Add(WCharacterFormat.DoubleStrikeKey, false);
            PropertiesHash.Add(WCharacterFormat.EmbossKey, false);
            PropertiesHash.Add(WCharacterFormat.EngraveKey, false);
            PropertiesHash.Add(WCharacterFormat.SubSuperScriptKey, SubSuperScript.None);
            PropertiesHash.Add(WCharacterFormat.TextBkgColorKey, Color.Empty);
            PropertiesHash.Add(WCharacterFormat.AllCapsKey, false);
            PropertiesHash.Add(WCharacterFormat.BoldBidiKey, false);
            PropertiesHash.Add(WCharacterFormat.FieldVanishKey, false);
            PropertiesHash.Add(WCharacterFormat.HiddenKey, false);
            PropertiesHash.Add(WCharacterFormat.SmallCapsKey, false);
            PropertiesHash.Add(WCharacterFormat.SpacingKey, 0f);
        }
        #endregion

        #region Class overrides
//#if !SILVERLIGHT
        /// <summary>
        /// 
        /// </summary>
        [DocumentationExclude()]
        protected override void InitXDLSHolder()
        {
            if (m_sprms == null)
            {
                XDLSHolder.AddElement(XDLSConstants.TextBorderTag, Border);
            }
        }
//#endif
        /// <summary>
        /// Overrides method : Get default value
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected override object GetDefValue(int key)
        {
            if (m_doc.DefCharFormat != null && m_doc.DefCharFormat != this)
            {
                return m_doc.DefCharFormat[key];
            }
            else
            {
                switch (key)
                {
                    case FontKey:
                        //if (m_doc.DefCharFormat != null)
                        //    return m_doc.DefCharFormat.Font;
                        //else
                            return new Font(DEF_FONTFAMILY, DEF_FONTSIZE);
                    case TextBkgColorKey:
                    case TextColorKey:
                    case HighlightColorKey:
                    case ForeColorKey:
                        return Color.Empty;
                    case FontSizeKey:
                    case FontSizeBidiKey:
                        return DEF_FONTSIZE;
                    case UnderlineKey:
                        return UnderlineStyle.None;
                    case SubSuperScriptKey:
                        return SubSuperScript.None;
                    case PositionKey:
                    case SpacingKey:
                        return 0f;
                    case DoubleStrikeKey:
                    case LineBreakKey:
                    case EmbossKey:
                    case ShadowKey:
                    case EngraveKey:
                    case AllCapsKey:
                    case SmallCapsKey:
                    case BidiKey:
                    case BoldBidiKey:
                    case ItalicBidiKey:
                    case HiddenKey:
                    case OutlineKey:
                    case BoldKey:
                    case ItalicKey:
                    case StrikeKey:
                    case InserteRevisionKey:
                    case DeleteRevisionKey:
                    case ChangedFormatKey:
                    case SpecialKey:
                    case NoProofKey:
                    case ListHasPicKey:
                    case FieldVanishKey:
                    case ComplexScriptKey:
                    case ContextualAlternatesKey:
                        return false;
                    case FontNameKey:
                    case FontNameAsciiKey:
                        if (!string.IsNullOrEmpty(m_doc.StandardAsciiFont))
                            return m_doc.StandardAsciiFont;
                        else
                            return DEF_FONTFAMILY;
                    case FontNameFarEastKey:
                        if (!string.IsNullOrEmpty(m_doc.StandardFarEastFont))
                            return m_doc.StandardFarEastFont;
                        else
                            return DEF_FONTFAMILY;
                    case FontNameBidiKey:
                        if (!string.IsNullOrEmpty(m_doc.StandardBidiFont))
                            return m_doc.StandardBidiFont;
                        else
                            return DEF_FONTFAMILY;
                    case FontNameNonFarEastKey:
                        if (!string.IsNullOrEmpty(m_doc.StandardNonFarEastFont))
                            return m_doc.StandardNonFarEastFont;
                        else
                            return DEF_FONTFAMILY;
                    case ItalicComplexKey:
                    case BoldComplexKey:
                    case HiddenComplexKey:
                        return (byte)0;
                    case LocaleIdASCIIKey:
                    case LocaleIdFarEastKey:
                        return (short)1033;
                    case RgLid3Key:
                    case RgLid3_2Key:
                    case LidKey:
                    case LidBiKey:
                        return short.MaxValue;
                    case TextureStyleKey:
                        return TextureStyle.TextureNone;
                    case ListPicIndexKey:
                        return int.MaxValue;
                    case FieldVanishCompKey:
                        return (byte)0;
                    case PicLocationKey:
                        return (int)0;
                    case LigaturesKey:
                        return LigatureType.None;
                    case NumberFormKey:
                        return NumberFormType.Default;
                    case NumberSpacingKey:
                        return NumberSpacingType.Default;
                    case StylisticSetKey:
                        return StylisticSetType.StylisticSetDefault;
                    case IdctHintKey:
                        return FontHintType.Default;
                    default:
                        throw new ArgumentException("key has invalid value");
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [DocumentationExclude()]
        protected override FormatBase GetDefComposite(int key)
        {
            switch (key)
            {
                case BorderKey:
                    return GetDefComposite(BorderKey, new Border(this, BorderKey));
            }
            return null;
        }
//#if !SILVERLIGHT
        /// <summary>
        /// Overrides method : Read attributes from xml
        /// </summary>
        /// <param name="reader"></param>
        protected override void ReadXmlAttributes(IXDLSAttributeReader reader)
        {
            base.ReadXmlAttributes(reader);

            if (reader.HasAttribute(XDLSConstants.TextFontNameAttr))
            {
                this[FontNameKey] = reader.ReadString(XDLSConstants.TextFontNameAttr);
            }
            if (reader.HasAttribute(XDLSConstants.TextFontNameBidiAttr))
            {
                this[FontNameBidiKey] = reader.ReadString(XDLSConstants.TextFontNameBidiAttr);
            }
            if (reader.HasAttribute(XDLSConstants.TextFontNameAsciiAttr))
            {
                this[FontNameAsciiKey] = reader.ReadString(XDLSConstants.TextFontNameAsciiAttr);
            }
            if (reader.HasAttribute(XDLSConstants.TextFontNameFarEastAttr))
            {
                this[FontNameFarEastKey] = reader.ReadString(XDLSConstants.TextFontNameFarEastAttr);
            }
            if (reader.HasAttribute(XDLSConstants.TextFontNameNonFarEastAttr))
            {
                this[FontNameNonFarEastKey] = reader.ReadString(XDLSConstants.TextFontNameNonFarEastAttr);
            }
            if (reader.HasAttribute(XDLSConstants.TextCharStyleName))
            {
                m_charStyleName = reader.ReadString(XDLSConstants.TextCharStyleName);
            }

            //      if( m_sprms != null )
            //        return;

            if (reader.HasAttribute(XDLSConstants.TextUnderlineAttr))
            {
                UnderlineStyle = (UnderlineStyle)reader.ReadEnum(XDLSConstants.TextUnderlineAttr, typeof(UnderlineStyle));
            }
            if (reader.HasAttribute(XDLSConstants.TextColorAttr))
            {
                TextColor = reader.ReadColor(XDLSConstants.TextColorAttr);
            }
            if (reader.HasAttribute(XDLSConstants.TextFontSizeAttr))
            {
                FontSize = reader.ReadFloat(XDLSConstants.TextFontSizeAttr);
            }
            if (reader.HasAttribute(XDLSConstants.TextBoldAttr))
            {
                Bold = reader.ReadBoolean(XDLSConstants.TextBoldAttr);
            }
            if (reader.HasAttribute(XDLSConstants.TextItalicAttr))
            {
                Italic = reader.ReadBoolean(XDLSConstants.TextItalicAttr);

            }
            if (reader.HasAttribute(XDLSConstants.TextStrikeAttr))
            {
                Strikeout = reader.ReadBoolean(XDLSConstants.TextStrikeAttr);
            }
            if (reader.HasAttribute(XDLSConstants.TextDoubleStrikeAttr))
            {
                DoubleStrike = reader.ReadBoolean(XDLSConstants.TextDoubleStrikeAttr);
            }
            if (reader.HasAttribute(XDLSConstants.TextLineSpacingAttr))
            {
                CharacterSpacing = reader.ReadFloat(XDLSConstants.TextLineSpacingAttr);
            }
            if (reader.HasAttribute(XDLSConstants.TextPositionAttr))
            {
                Position = reader.ReadFloat(XDLSConstants.TextPositionAttr);
            }
            if (reader.HasAttribute(XDLSConstants.TextSubSuperScriptAttr))
            {
                SubSuperScript = (SubSuperScript)reader.ReadEnum(XDLSConstants.TextSubSuperScriptAttr, typeof(SubSuperScript));
            }
            if (reader.HasAttribute(XDLSConstants.TextBackgroundColorAttr))
            {
                TextBackgroundColor = reader.ReadColor(XDLSConstants.TextBackgroundColorAttr);
            }
            if (reader.HasAttribute(XDLSConstants.TextLineBreakAttr))
            {
                LineBreak = reader.ReadBoolean(XDLSConstants.TextLineBreakAttr);
            }


            if (reader.HasAttribute(XDLSConstants.TextShadowAttr))
            {
                Shadow = reader.ReadBoolean(XDLSConstants.TextShadowAttr);
            }
            if (reader.HasAttribute(XDLSConstants.TextEmbossAttr))
            {
                Emboss = reader.ReadBoolean(XDLSConstants.TextEmbossAttr);
            }
            if (reader.HasAttribute(XDLSConstants.TextEngraveAttr))
            {
                Engrave = reader.ReadBoolean(XDLSConstants.TextEngraveAttr);
            }
            if (reader.HasAttribute(XDLSConstants.TextHiddenAttr))
            {
                Hidden = reader.ReadBoolean(XDLSConstants.TextHiddenAttr);
            }
            if (reader.HasAttribute(XDLSConstants.TextAllCapsAttr))
            {
                AllCaps = reader.ReadBoolean(XDLSConstants.TextAllCapsAttr);
            }
            if (reader.HasAttribute(XDLSConstants.TextSmallCapsAttr))
            {
                SmallCaps = reader.ReadBoolean(XDLSConstants.TextSmallCapsAttr);
            }
            if (reader.HasAttribute(XDLSConstants.TextBidiAttr))
            {
                Bidi = reader.ReadBoolean(XDLSConstants.TextBidiAttr);
            }
            if (reader.HasAttribute(XDLSConstants.TextBoldBidiAttr))
            {
                BoldBidi = reader.ReadBoolean(XDLSConstants.TextBoldBidiAttr);
            }
            if (reader.HasAttribute(XDLSConstants.TextItalicBidiAttr))
            {
                ItalicBidi = reader.ReadBoolean(XDLSConstants.TextItalicBidiAttr);
            }
            if (reader.HasAttribute(XDLSConstants.TextFontSizeBidiAttr))
            {
                FontSizeBidi = reader.ReadFloat(XDLSConstants.TextFontSizeBidiAttr);
            }
            if (reader.HasAttribute(XDLSConstants.TextHighlightColorKey))
            {
                HighlightColor = reader.ReadColor(XDLSConstants.TextHighlightColorKey);
            }
            if (reader.HasAttribute(XDLSConstants.TextIdctHintAttr))
            {
                string hint = reader.ReadString(XDLSConstants.TextIdctHintAttr);
                switch (hint)
                {
                    case "cs":
                        IdctHint = FontHintType.CS;
                        break;
                    case "eastAsia":
                        IdctHint = FontHintType.EastAsia;
                        break;
                    default:
                        IdctHint = FontHintType.Default;
                        break;
                }
            }
            if (reader.HasAttribute(XDLSConstants.TextRgLid0Attr))
            {
                LocaleIdASCII = reader.ReadShort(XDLSConstants.TextRgLid0Attr);
            }
            if (reader.HasAttribute(XDLSConstants.TextRgLid1Attr))
            {
                LocaleIdFarEast = reader.ReadShort(XDLSConstants.TextRgLid1Attr);
            }
            if (reader.HasAttribute(XDLSConstants.TextRgLid3Attr))
            {
                RgLid3 = reader.ReadShort(XDLSConstants.TextRgLid3Attr);
            }
            if (reader.HasAttribute(XDLSConstants.TextRgLid3_2Attr))
            {
                RgLid3_2 = reader.ReadShort(XDLSConstants.TextRgLid3_2Attr);
            }
            if (reader.HasAttribute(XDLSConstants.TextLidAttr))
            {
                Lid = reader.ReadShort(XDLSConstants.TextLidAttr);
            }
            if (reader.HasAttribute(XDLSConstants.TextLidBiAttr))
            {
                LidBi = reader.ReadShort(XDLSConstants.TextLidBiAttr);
            }
            if (reader.HasAttribute(XDLSConstants.TextNoProofAttr))
            {
                NoProof = reader.ReadBoolean(XDLSConstants.TextNoProofAttr);
            }
            if (reader.HasAttribute(XDLSConstants.TextForeColorAttr))
            {
                ForeColor = reader.ReadColor(XDLSConstants.TextForeColorAttr);
            }
            if (reader.HasAttribute(XDLSConstants.TextTextureAttr))
            {
                TextureStyle = (TextureStyle)reader.ReadEnum(XDLSConstants.TextTextureAttr, typeof(TextureStyle));
            }
            if (reader.HasAttribute(XDLSConstants.TextOutLineAttr))
            {
                OutLine = reader.ReadBoolean(XDLSConstants.TextOutLineAttr);
            }
            #region Complex values
            //      if( reader.HasAttribute( XDLSConstants.TextAllCapsComplexKey ) )
            //      {
            //        m_allCapsComplex = reader.ReadByte( XDLSConstants.TextAllCapsComplexKey );
            //      }
            //      if( reader.HasAttribute( XDLSConstants.TextShadowComplexAttr ) )
            //      {
            //        m_shadowComplex = reader.ReadByte( XDLSConstants.TextShadowComplexAttr );
            //      }      
            //      if( reader.HasAttribute( XDLSConstants.TextEmbossComplex ) )
            //      {
            //        m_embossComplex = reader.ReadByte( XDLSConstants.TextEmbossComplex );
            //      }
            //      if( reader.HasAttribute( XDLSConstants.TextEngraveComplex ) )
            //      {
            //        m_engraveComplex = reader.ReadByte( XDLSConstants.TextEngraveComplex );
            //      }
            //      if( reader.HasAttribute( XDLSConstants.TextItalicComplexKey ) )
            //      {
            //        ItalicComplex = reader.ReadByte( XDLSConstants.TextItalicComplexKey );
            //      }
            //      if( reader.HasAttribute( XDLSConstants.TextBoldComplexKey ) )
            //      {
            //        BoldComplex = reader.ReadByte( XDLSConstants.TextBoldComplexKey );
            //      }
            //      if( reader.HasAttribute( XDLSConstants.TextHiddenComplexAttr ) )
            //      {
            //        HiddenComplex = reader.ReadByte( XDLSConstants.TextHiddenComplexAttr );
            //      }
            //      if( reader.HasAttribute( XDLSConstants.TextDStrikeComplexAttr ) )
            //      {
            //        DoubleStrikeComplex = reader.ReadByte( XDLSConstants.TextDStrikeComplexAttr );
            //      }
            //      if( reader.HasAttribute( XDLSConstants.TextSmallCapsComplexAttr ) )
            //      {
            //        SmallCapsComplex = reader.ReadByte( XDLSConstants.TextSmallCapsComplexAttr );
            //      }
            //      if( reader.HasAttribute( XDLSConstants.TextStrikeComplexAttr ) )
            //      {
            //        m_strikeComplex = reader.ReadByte( XDLSConstants.TextStrikeComplexAttr );
            //      }
            #endregion
        }
        /// <summary>
        /// Overrides method : Write attributes to xml
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteXmlAttributes(IXDLSAttributeWriter writer)
        {
            base.WriteXmlAttributes(writer);

            if (HasKey(FontNameKey))
            {
                writer.WriteValue(XDLSConstants.TextFontNameAttr, FontName);
            }
            if (HasKey(FontNameBidiKey))
            {
                writer.WriteValue(XDLSConstants.TextFontNameBidiAttr, FontNameBidi);
            }
            if (HasKey(FontNameFarEastKey))
            {
                writer.WriteValue(XDLSConstants.TextFontNameFarEastAttr, FontNameFarEast);
            }
            if (HasKey(FontNameNonFarEastKey))
            {
                writer.WriteValue(XDLSConstants.TextFontNameNonFarEastAttr, FontNameNonFarEast);
            }
            if (HasKey(FontNameAsciiKey))
            {
                writer.WriteValue(XDLSConstants.TextFontNameAsciiAttr, FontNameAscii);
            }
            if (m_charStyleName != null)
            {
                writer.WriteValue(XDLSConstants.TextCharStyleName, m_charStyleName);
            }

            if (!SerializeAllData())
                return;

            if (LineBreak)
            {
                writer.WriteValue(XDLSConstants.TextLineBreakAttr, LineBreak);
            }

            if (!TextColor.IsEmpty)
            {
                writer.WriteValue(XDLSConstants.TextColorAttr, TextColor);
            }
            if (HasValue(FontSizeKey))
            {
                writer.WriteValue(XDLSConstants.TextFontSizeAttr, FontSize);
            }
            if (HasValue(BoldKey))
            {
                writer.WriteValue(XDLSConstants.TextBoldAttr, Bold);
            }
            if (HasValue(ItalicKey))
            {
                writer.WriteValue(XDLSConstants.TextItalicAttr, Italic);
            }
            if (HasValue(StrikeKey))
            {
                writer.WriteValue(XDLSConstants.TextStrikeAttr, Strikeout);
            }
            if (HasValue(DoubleStrikeKey))
            {
                writer.WriteValue(XDLSConstants.TextDoubleStrikeAttr, DoubleStrike);
            }
            if (HasValue(UnderlineKey))
            {
                writer.WriteValue(XDLSConstants.TextUnderlineAttr, (int)UnderlineStyle);
            }
            if (HasValue(SubSuperScriptKey))
            {
                writer.WriteValue(XDLSConstants.TextSubSuperScriptAttr, SubSuperScript);
            }
            if (HasValue(SpacingKey))
            {
                writer.WriteValue(XDLSConstants.TextLineSpacingAttr, CharacterSpacing);
            }
            if (HasValue(PositionKey))
            {
                writer.WriteValue(XDLSConstants.TextPositionAttr, Position);
            }
            if (HasValue(TextBkgColorKey))
            {
                writer.WriteValue(XDLSConstants.TextBackgroundColorAttr, TextBackgroundColor);
            }
            if (HasValue(ShadowKey))
            {
                writer.WriteValue(XDLSConstants.TextShadowAttr, Shadow);
            }
            if (HasValue(EmbossKey))
            {
                writer.WriteValue(XDLSConstants.TextEmbossAttr, Emboss);
            }
            if (HasValue(EngraveKey))
            {
                writer.WriteValue(XDLSConstants.TextEngraveAttr, Engrave);
            }
            if (HasValue(HiddenKey))
            {
                writer.WriteValue(XDLSConstants.TextHiddenAttr, Hidden);
            }
            if (HasValue(AllCapsKey))
            {
                writer.WriteValue(XDLSConstants.TextAllCapsAttr, AllCaps);
            }
            if (HasValue(SmallCapsKey))
            {
                writer.WriteValue(XDLSConstants.TextSmallCapsAttr, SmallCaps);
            }
            if (HasValue(BidiKey))
            {
                writer.WriteValue(XDLSConstants.TextBidiAttr, Bidi);
            }
            if (HasValue(BoldBidiKey))
            {
                writer.WriteValue(XDLSConstants.TextBoldBidiAttr, BoldBidi);
            }
            if (HasValue(ItalicBidiKey))
            {
                writer.WriteValue(XDLSConstants.TextItalicBidiAttr, ItalicBidi);
            }
            if (HasValue(FontSizeBidiKey))
            {
                writer.WriteValue(XDLSConstants.TextFontSizeBidiAttr, FontSizeBidi);
            }

            if (HasValue(HighlightColorKey))
            {
                writer.WriteValue(XDLSConstants.TextHighlightColorKey, HighlightColor);
            }
            if (HasValue(IdctHintKey))
            {
                writer.WriteValue(XDLSConstants.TextIdctHintAttr, IdctHint);
            }
            if (HasValue(LocaleIdASCIIKey))
            {
                writer.WriteValue(XDLSConstants.TextRgLid0Attr, LocaleIdASCIIKey);
            }
            if (HasValue(LocaleIdFarEastKey))
            {
                writer.WriteValue(XDLSConstants.TextRgLid1Attr, LocaleIdFarEastKey);
            }
            if (HasValue(RgLid3Key))
            {
                writer.WriteValue(XDLSConstants.TextRgLid3Attr, RgLid3);
            }
            if (HasValue(RgLid3_2Key))
            {
                writer.WriteValue(XDLSConstants.TextRgLid3_2Attr, RgLid3_2);
            }
            if (HasValue(LidKey))
            {
                writer.WriteValue(XDLSConstants.TextLidAttr, Lid);
            }
            if (HasValue(LidBiKey))
            {
                writer.WriteValue(XDLSConstants.TextLidBiAttr, LidBi);
            }
            if (HasValue(NoProofKey))
            {
                writer.WriteValue(XDLSConstants.TextNoProofAttr, NoProof);
            }
            if (HasValue(ForeColorKey))
            {
                writer.WriteValue(XDLSConstants.TextForeColorAttr, ForeColor);
            }
            if (HasValue(TextureStyleKey))
            {
                writer.WriteValue(XDLSConstants.TextTextureAttr, TextureStyle);
            }
            if (HasValue(OutlineKey))
            {
                writer.WriteValue(XDLSConstants.TextOutLineAttr, OutLine);
            }

            //Complex values
            if (HasValue(ItalicComplexKey))
            {
                WriteComplexAttr(writer, ItalicComplexKey, XDLSConstants.TextItalicComplexKey);
            }
            if (HasValue(AllCapsComplexKey))
            {
                WriteComplexAttr(writer, AllCapsComplexKey, XDLSConstants.TextAllCapsComplexKey);
            }
            if (HasValue(EmbossComplexKey))
            {
                WriteComplexAttr(writer, EmbossComplexKey, XDLSConstants.TextEmbossComplex);
            }
            if (HasValue(EngraveComplexKey))
            {
                WriteComplexAttr(writer, EngraveComplexKey, XDLSConstants.TextEngraveComplex);
            }
            if (HasValue(ShadowComplexKey))
            {
                WriteComplexAttr(writer, ShadowComplexKey, XDLSConstants.TextShadowComplexAttr);
            }
            if (HasValue(BoldComplexKey))
            {
                WriteComplexAttr(writer, BoldComplexKey, XDLSConstants.TextBoldComplexKey);
            }
            if (HasValue(HiddenComplexKey))
            {
                WriteComplexAttr(writer, HiddenComplexKey, XDLSConstants.TextHiddenComplexAttr);
            }
            if (HasValue(DoubleStrikeComplexKey))
            {
                WriteComplexAttr(writer, DoubleStrikeComplexKey, XDLSConstants.TextDStrikeComplexAttr);
            }
            if (HasValue(SmallCapsComplexKey))
            {
                WriteComplexAttr(writer, SmallCapsComplexKey, XDLSConstants.TextSmallCapsComplexAttr);
            }
            if (HasValue(StrikeComplexKey))
            {
                WriteComplexAttr(writer, StrikeComplexKey, XDLSConstants.TextStrikeComplexAttr);
            }

            #region Complex
            //      if( AllCapsComplex != byte.MaxValue )
            //      {
            //        writer.WriteValue( XDLSConstants.TextAllCapsComplexKey, AllCapsComplex );
            //      }
            //      if( m_embossComplex != byte.MaxValue )
            //      {
            //        writer.WriteValue( XDLSConstants.TextEmbossComplex, m_embossComplex );
            //      }
            //      if( m_engraveComplex != byte.MaxValue )
            //      {
            //        writer.WriteValue( XDLSConstants.TextEngraveComplex, m_engraveComplex );
            //      }
            //      if( ItalicComplex != byte.MaxValue )
            //      {
            //        writer.WriteValue( XDLSConstants.TextItalicComplexKey, ItalicComplex );
            //      }
            //      if( m_shadowComplex != byte.MaxValue )
            //      {
            //        writer.WriteValue( XDLSConstants.TextShadowComplexAttr, m_shadowComplex );
            //      }
            //      if( BoldComplex != byte.MaxValue )
            //      {
            //        writer.WriteValue( XDLSConstants.TextBoldComplexKey, BoldComplex );
            //      }
            //      if( HiddenComplex != byte.MaxValue )
            //      {
            //        writer.WriteValue( XDLSConstants.TextHiddenComplexAttr, HiddenComplex );
            //      }
            //      if( DoubleStrikeComplex != byte.MaxValue )
            //      {
            //        writer.WriteValue( XDLSConstants.TextDStrikeComplexAttr, DoubleStrikeComplex );
            //      }
            //      if( SmallCapsComplex != byte.MaxValue )
            //      {
            //        writer.WriteValue( XDLSConstants.TextSmallCapsComplexAttr, SmallCapsComplex );
            //      }
            //      if( m_strikeComplex != byte.MaxValue )
            //      {
            //        writer.WriteValue( XDLSConstants.TextStrikeComplexAttr, m_strikeComplex );
            //      }
            #endregion
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteXmlContent(IXDLSContentWriter writer)
        {
            base.WriteXmlContent(writer);
            if (m_sprms != null /*&& !Document.ConvertingToDocx*/ )
            {
                byte[] internalData = new byte[m_sprms.Length];
                m_sprms.Save(internalData, 0);
                writer.WriteChildBinaryElement(XDLSConstants.InternalDataTag, internalData);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        protected override bool ReadXmlContent(IXDLSContentReader reader)
        {
            bool retValue = base.ReadXmlContent(reader);

            if (reader.TagName == XDLSConstants.InternalDataTag)
            {
                byte[] internalData = reader.ReadChildBinaryElement();
                m_sprms = new SinglePropertyModifierArray(internalData);
                retValue = true;
                if (m_charProps != null)
                {
                    m_charProps.CharacterPropertyException.PropertyModifiers = m_sprms;
                }
            }

            return retValue;
        }
//#endif
        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        [DocumentationExclude()]
        new protected internal void ImportContainer(FormatBase format)
        {
            base.ImportContainer(format);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        protected override void ImportMembers(FormatBase format)
        {
            base.ImportMembers(format);

            WCharacterFormat charFormat = format as WCharacterFormat;

            if (charFormat != null)
            {
                m_charProps = null;

                if (Document.IsMailMerge || Document.IsCloning)
                {
                    if (charFormat.Sprms != null)
                    {
                        m_sprms = charFormat.Sprms.Clone();
                        m_hasClonedProps = true;
                        (format as WCharacterFormat).m_hasClonedProps = true;
                    }
                }
                else
                {
                    if (charFormat.Sprms != null)
                    {
                        m_sprms = charFormat.Sprms.Clone();
                    }
                }

                if (charFormat.HasKey(FontNameKey))
                    this[FontNameKey] = charFormat[FontNameKey];
                if (charFormat.HasKey(FontNameAsciiKey))
                    this[FontNameAsciiKey] = charFormat[FontNameAsciiKey];
                if (charFormat.HasKey(FontNameBidiKey))
                    this[FontNameBidiKey] = charFormat[FontNameBidiKey];
                if (charFormat.HasKey(FontNameFarEastKey))
                    this[FontNameFarEastKey] = charFormat[FontNameFarEastKey];
                if (charFormat.HasKey(FontNameNonFarEastKey))
                    this[FontNameNonFarEastKey] = charFormat[FontNameNonFarEastKey];

                //Clone chatacter style name
                string charStyleName = charFormat.CharStyleName;
                if (charStyleName != null)
                {
                    WordDocument doc = Document as WordDocument;
                    CharacterStyle foundStyle = doc.Styles.FindByName(charStyleName) as CharacterStyle;
                    if (foundStyle == null)
                    {
                        IStyle style = (charFormat.Document as WordDocument).Styles.FindByName(charStyleName);
                        if (style != null)
                            doc.Styles.Add(style.Clone());
                    }
                    m_charStyleName = charStyleName;
                }
            }
        }
        /// <summary>
        /// Action on format change.
        /// </summary>
        /// <param name="format">The format.</param>
        protected override void OnChange(FormatBase format, int propKey)
        {
            if (m_cancelOnChange)
                return;

            if (this.OwnerBase != null && this.OwnerBase.Document.IsOpening)
                return;

            int propertyKey = int.MinValue;

            if (format is Border || format is Borders)
            {
                propertyKey = BorderKey;
            }

            if (propertyKey != int.MinValue)
            {
                UpdateCharProps(propertyKey, this[propertyKey]);
            }
        }
        /// <summary>
        /// Apply base style
        /// </summary>
        /// <param name="baseFormat"></param>
        internal override void ApplyBase(FormatBase baseFormat)
        {
            //Updates complex boolean properties.
            Dictionary<int, bool> complexBooleanValues = new Dictionary<int, bool>();
            if (Document.IsCloning
                && Document != baseFormat.Document
                && !baseFormat.Document.ImportStyles)
            {
                //Copys the complex Boolean value defined as direct formatting, before applying destination base format.
                List<int> complexBooleanKeys = new List<int>(new int[] { FieldVanishKey, ComplexScriptKey, BoldKey, ItalicKey, StrikeKey, DoubleStrikeKey, ShadowKey, EmbossKey, EngraveKey, HiddenKey, AllCapsKey, SmallCapsKey, BidiKey, BoldBidiKey, ItalicBidiKey, IdctHintKey, OutlineKey, SpecialKey });
                foreach (int key in complexBooleanKeys)
                {
                    if(HasValue(key))
                        complexBooleanValues.Add(key, (bool)GetComplexBoolValue((short)key));
                }
                complexBooleanKeys.Clear();
            }
            base.ApplyBase(baseFormat);
            foreach (KeyValuePair<int, bool> keyValue in complexBooleanValues)
            {
                //Updates the sprm value to preserve complex Boolean property similar to the source document, after applying destination base format.
                if (keyValue.Value != (bool)GetComplexBoolValue((short)keyValue.Key))
                {
                    SinglePropertyModifierRecord sprm = Sprms[GetSprmOption(keyValue.Key)];
                    if (sprm != null)
                        sprm.ByteValue = (sprm.ByteValue == DEF_NEGCOMPLEX_VALUE) ? DEF_POSCOMPLEX_VALUE : DEF_NEGCOMPLEX_VALUE;
                }
            }
            complexBooleanValues.Clear();
            if (Document.IsOpening)
            {
                CharacterProperties props = (baseFormat as WCharacterFormat).CharacterProps;
                if (props == null)
                    return;
                CharacterProps.UpdateBaseCharacterProperties(props);
            }
# if !SILVERLIGHT && !WP
            UpdateUsedFontsCollection();
#endif
        }
# if !SILVERLIGHT && !WP
        /// <summary>
        /// 
        /// </summary>
        private void UpdateUsedFontsCollection()
        {
            string fontName = HasValue(FontNameAsciiKey) ? GetFontName(WCharacterFormat.FontNameAsciiKey) : null;

            if (string.IsNullOrEmpty(fontName))
                return;

            FontStyle fontStyle = new FontStyle();
            if (HasValue(BoldKey) && Bold)
            {
                fontStyle |= FontStyle.Bold;
            }
            if ((HasValue(ItalicKey) && Italic))
            {
                fontStyle |= FontStyle.Italic;
            }
            if (HasValue(UnderlineKey) && UnderlineStyle != UnderlineStyle.None)
            {
                fontStyle |= FontStyle.Underline;
            }
            if (HasValue(StrikeKey) && Strikeout)
            {
                fontStyle |= FontStyle.Strikeout;
            }
            Font font = null;
            try
            {
                font = new Font(fontName, 11, fontStyle);
            }
            catch (Exception ex)
            {
                FontFamily fontFamily = new FontFamily(fontName);
                if (fontFamily.IsStyleAvailable(FontStyle.Bold))
                    fontStyle |= FontStyle.Bold;
                if (fontFamily.IsStyleAvailable(FontStyle.Italic))
                    fontStyle |= FontStyle.Italic;
                if (fontFamily.IsStyleAvailable(FontStyle.Underline))
                    fontStyle |= FontStyle.Underline;
                if (fontFamily.IsStyleAvailable(FontStyle.Strikeout))
                    fontStyle |= FontStyle.Strikeout;
                font = new Font(fontName, 11, fontStyle);
            }
            if (!m_doc.UsedFontNames.Contains(font))
                m_doc.UsedFontNames.Add(font);
        }
#endif
        #endregion
        /// <summary>
        /// Updates the complex boolean value.
        /// </summary>
        /// <param name="format">The character format.</param>
        /// <param name="propKey">The property key.</param>
        /// <param name="val">Boolean value.</param>
        internal void UpdateComplexProperty(short propertyKey, bool value)
        {
            byte complexValue = 0;

            if (!(this.OwnerBase is WListLevel))
            {
                WCharacterFormat chFormat = this as WCharacterFormat;
                Style baseCharStyle = m_doc.Styles.FindByName(chFormat.CharStyleName) as Style;
                // Base format is character format of paragraph style.
                WCharacterFormat baseFormat = this.BaseFormat as WCharacterFormat;
                bool baseValue = false;

                if (baseCharStyle != null && baseCharStyle.CharacterFormat.HasValue(propertyKey))
                {
                    // If base value is not equal to current, then controvers complex value.
                    baseValue = (bool)baseCharStyle.CharacterFormat.GetPropertyValue(propertyKey);
                    complexValue = (value == baseValue) ? WCharacterFormat.DEF_POSCOMPLEX_VALUE : WCharacterFormat.DEF_NEGCOMPLEX_VALUE;
                }
                else if (baseFormat != null && baseFormat.HasValue(propertyKey))
                {
                    baseValue = (bool)baseFormat.GetPropertyValue(propertyKey);
                    complexValue = (value == baseValue) ? WCharacterFormat.DEF_POSCOMPLEX_VALUE : WCharacterFormat.DEF_NEGCOMPLEX_VALUE;
                }
                else
                {
                    complexValue = (value) ? WCharacterFormat.DEF_NEGCOMPLEX_VALUE : WCharacterFormat.DEF_POSCOMPLEX_VALUE;
                }
            }

            if (complexValue != 0)
            {
                this.SetComplexBoolValue(propertyKey, complexValue);
            }
        }
        #region Implementation / xml helper methods
        /// <summary>
        /// Determines whether the specified property key has value.
        /// </summary>
        /// <param name="propertyKey">The property key.</param>
        /// <returns>
        /// 	if the specified property key has value, set to <c>true</c>.
        /// </returns>
        internal override bool HasValue(int propertyKey)
        {
            if (HasKey(propertyKey))
                return true;

            if (m_sprms == null || m_sprms.Count == 0)
                return false;

            int sprmOptionKey = GetSprmOption(propertyKey);
            if (sprmOptionKey == int.MaxValue)
                return false;

            SinglePropertyModifierRecord sprm = m_sprms[sprmOptionKey];

            // Implementation for LocateIds
            if (propertyKey == LocaleIdFarEastKey && sprm == null)
            {
                sprm = m_sprms[WordSprmOptions.sprmCLidBi];
                if (sprm == null)
                    return false;
                else
                    return true;
            }

            if (sprm == null)
                return false;

            return true;
        }
        /// <summary>
        /// Gets the SPRM option.
        /// </summary>
        /// <param name="propertyKey">The WCharacterFormat property key.</param>
        /// <returns></returns>
        protected override int GetSprmOption(int propertyKey)
        {
            switch (propertyKey)
            {
                case FontNameKey:
                case FontNameAsciiKey:
                    return WordSprmOptions.sprmCRgFtc0;
                case FontNameFarEastKey:
                    return WordSprmOptions.sprmCRgFtc1;
                case FontNameNonFarEastKey:
                    return WordSprmOptions.sprmCRgFtc2;
                case FontNameBidiKey:
                    return WordSprmOptions.sprmCFtcBi;
                case BoldKey:
                case BoldComplexKey:
                    return WordSprmOptions.sprmCFBold;
                case ItalicKey:
                case ItalicComplexKey:
                    return WordSprmOptions.sprmCFItalic;
                case StrikeKey:
                case StrikeComplexKey:
                    return WordSprmOptions.sprmCFStrike;
                case UnderlineKey:
                    return WordSprmOptions.sprmCKul;
                case TextBkgColorKey:
                case ForeColorKey:
                case TextureStyleKey:
                    return WordSprmOptions.sprmCShd;
                case TextBkgColorNewKey:
                case ForeColorNewKey:
                case TextureStyleNewKey:
                    return WordSprmOptions.sprmCShdNew;
                case SubSuperScriptKey:
                    return WordSprmOptions.sprmCIss;
                case DoubleStrikeKey:
                case DoubleStrikeComplexKey:
                    return WordSprmOptions.sprmCFDStrike;
                case PositionKey:
                    return WordSprmOptions.sprmCHpsPos;
                case SpacingKey:
                    return WordSprmOptions.sprmCDxaSpace;
                case ShadowKey:
                case ShadowComplexKey:
                    return WordSprmOptions.sprmCFShadow;
                case EmbossKey:
                case EmbossComplexKey:
                    return WordSprmOptions.sprmCFEmboss;
                case EngraveKey:
                case EngraveComplexKey:
                    return WordSprmOptions.sprmCFImprint;
                case HiddenKey:
                case HiddenComplexKey:
                    return WordSprmOptions.sprmCFVanish;
                case AllCapsKey:
                case AllCapsComplexKey:
                    return WordSprmOptions.sprmCFCaps;
                case SmallCapsKey:
                case SmallCapsComplexKey:
                    return WordSprmOptions.sprmCFSmallCaps;
                case BidiKey:
                    return WordSprmOptions.sprmCFBiDi;
                case BoldBidiKey:
                    return WordSprmOptions.sprmCFBoldBi;
                case ItalicBidiKey:
                    return WordSprmOptions.sprmCFItalicBi;
                case FontSizeBidiKey:
                    return WordSprmOptions.sprmCHpsBi;
                case HighlightColorKey:
                    return WordSprmOptions.sprmCHighlight;
                case OutlineKey:
                    return WordSprmOptions.sprmCFOutline;
                case IdctHintKey:
                    return WordSprmOptions.sprmCIdctHint;
                case LocaleIdASCIIKey:
                    return WordSprmOptions.sprmCRgLid0;
                case LocaleIdFarEastKey:
                    return WordSprmOptions.sprmCRgLid1;
                case RgLid3Key:
                    return WordSprmOptions.sprmCRgLid3;
                case RgLid3_2Key:
                    return WordSprmOptions.sprmCRgLid3_2;
                case LidKey:
                    return WordSprmOptions.sprmCLid;
                case LidBiKey:
                    return WordSprmOptions.sprmCLidBi;
                case FontSizeKey:
                    return WordSprmOptions.sprmCHps;
                case TextColorKey:
                    return WordSprmOptions.sprmCIco;
                case TextColorExtKey:
                    return WordSprmOptions.sprmCIcoe;
                case ListPicIndexKey:
                    return WordSprmOptions.sprmCPbiImageIndex;
                case ListHasPicKey:
                    return WordSprmOptions.sprmCPbiHasImage;
                case ComplexScriptKey:
                    return WordSprmOptions.sprmCFComplexScripts;
                case FieldVanishKey:
                case FieldVanishCompKey:
                    return WordSprmOptions.sprmCFFldVanish;
                case PicLocationKey:
                    return WordSprmOptions.sprmCPicLocation;
                case ChangedFormatKey:
                    return WordSprmOptions.sprmCPropRMark;
                case InserteRevisionKey:
                    return WordSprmOptions.sprmCFRMark;
                case DeleteRevisionKey:
                    return WordSprmOptions.sprmCFRMarkDel;
                case BorderKey:
                    return WordSprmOptions.sprmCBrc;
                case NoProofKey:
                    return WordSprmOptions.sprmCFNoProof;
                case SpecialKey:
                    return WordSprmOptions.sprmCFSpec;
                default:
                    return int.MaxValue;
            }
        }
        /// <summary>
        /// Writes the complex attribute.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="propKey">The property key.</param>
        /// <param name="xdlsConstant">The XDLS constant string.</param>
        private void WriteComplexAttr(IXDLSAttributeWriter writer, int propKey, string xdlsConstant)
        {
            int sprmOptionKey = GetSprmOption(propKey);
            SinglePropertyModifierRecord sprm = m_sprms[sprmOptionKey];
            writer.WriteValue(xdlsConstant, sprm.ByteValue);
        }
        #endregion

        #region Internal properties converter
        /// <summary>
        /// Gets the font hint.
        /// </summary>
        /// <returns></returns>
        internal string GetFontHint()
        {
            string hint = string.Empty;
            switch (IdctHint)
            {
                case FontHintType.CS:
                    hint = "cs";
                    break;
                case FontHintType.EastAsia:
                    hint = "eastAsia";
                    break;
                default:
                    hint = "default";
                    break;
            }
            return hint;
        }
        ///<summary>
        ///Returns font from Hint
        /// </summary>
        internal string GetFontNameFromHint()
        {
            string fontName = "";
            float fontSize;
            switch (this.IdctHint)
            {
                case FontHintType.CS:
                    fontName = this.FontNameBidi;
                    fontSize = this.FontSizeBidi;
                    break;
                case FontHintType.EastAsia:
                    fontName = this.FontNameFarEast;
                    break;
                case FontHintType.Default:
                    fontName = this.FontNameNonFarEast;
                    break;
            }
            return fontName;
        }
        /// <summary>
        /// Converts character property to format property.
        /// </summary>
        /// <param name="popertyKey">The property key.</param>
        private void PropToFormat(int propertyKey)
        {
            SinglePropertyModifierRecord sprm = null;
            CharacterProperties baseProps = (this.BaseFormat != null) ?
             (this.BaseFormat as WCharacterFormat).CharacterProps : null;
            m_charProps.BaseProperties = (CharStyle != null && CharStyle.CharacterFormat.HasValue(propertyKey)) ?
              CharStyle.CharacterFormat.CharacterProps : baseProps;

            switch (propertyKey)
            {
                #region Boolean properties
                case WCharacterFormat.BoldKey:
                    if (m_charProps.Sprms[GetSprmOption(propertyKey)] != null)
                        this[BoldKey] = m_charProps.Bold;
                    break;
                case WCharacterFormat.ItalicKey:
                    if (m_charProps.Sprms[GetSprmOption(propertyKey)] != null)
                        this[ItalicKey] = m_charProps.Italic;
                    break;
                case WCharacterFormat.StrikeKey:
                    if (m_charProps.Sprms[GetSprmOption(propertyKey)] != null)
                        this[StrikeKey] = m_charProps.Strike;
                    break;
                case WCharacterFormat.UnderlineKey:
                    if (m_charProps.Sprms[GetSprmOption(propertyKey)] != null)
                        this[UnderlineKey] = (UnderlineStyle)m_charProps.UnderlineCode;
                    break;
                case WCharacterFormat.ShadowKey:
                    if (m_charProps.Sprms[GetSprmOption(propertyKey)] != null)
                        this[ShadowKey] = m_charProps.Shadow;
                    break;
                case WCharacterFormat.EmbossKey:
                    if (m_charProps.Sprms[GetSprmOption(propertyKey)] != null)
                        this[EmbossKey] = m_charProps.Emboss;
                    break;
                case WCharacterFormat.EngraveKey:
                    if (m_charProps.Sprms[GetSprmOption(propertyKey)] != null)
                        this[EngraveKey] = m_charProps.Engrave;
                    break;
                case WCharacterFormat.HiddenKey:
                    if (m_charProps.Sprms[GetSprmOption(propertyKey)] != null)
                        this[HiddenKey] = m_charProps.Hidden;
                    break;
                case WCharacterFormat.AllCapsKey:
                    if (m_charProps.Sprms[GetSprmOption(propertyKey)] != null)
                        this[AllCapsKey] = m_charProps.AllCaps;
                    break;
                case WCharacterFormat.SmallCapsKey:
                    if (m_charProps.Sprms[GetSprmOption(propertyKey)] != null)
                        this[SmallCapsKey] = m_charProps.SmallCaps;
                    break;
                case WCharacterFormat.BidiKey:
                    if (m_charProps.Sprms[GetSprmOption(propertyKey)] != null)
                        this[BidiKey] = m_charProps.Bidi;
                    break;
                case WCharacterFormat.BoldBidiKey:
                    if (m_charProps.Sprms[GetSprmOption(propertyKey)] != null)
                        this[BoldBidiKey] = m_charProps.BoldBi;
                    break;
                case WCharacterFormat.ItalicBidiKey:
                    if (m_charProps.Sprms[GetSprmOption(propertyKey)] != null)
                        this[ItalicBidiKey] = m_charProps.ItalicBi;
                    break;
                case WCharacterFormat.OutlineKey:
                    if (m_charProps.Sprms[GetSprmOption(propertyKey)] != null)
                        this[OutlineKey] = m_charProps.Outline;
                    break;
                case WCharacterFormat.IdctHintKey:
                    if (m_charProps.Sprms[GetSprmOption(propertyKey)] != null)
                        this[IdctHintKey] = m_charProps.IdctHint;
                    break;
                case WCharacterFormat.InserteRevisionKey:
                    if (m_charProps.Sprms[GetSprmOption(propertyKey)] != null)
                        this[InserteRevisionKey] = m_charProps.IsInsertRevision;
                    break;
                case WCharacterFormat.DeleteRevisionKey:
                    if (m_charProps.Sprms[GetSprmOption(propertyKey)] != null)
                        this[DeleteRevisionKey] = m_charProps.IsDeleteRevision;
                    break;
                case WCharacterFormat.ChangedFormatKey:
                    UpdateChangedFormat();
                    break;
                case WCharacterFormat.SpecialKey:
                    if (m_charProps.Sprms[GetSprmOption(propertyKey)] != null)
                        this[SpecialKey] = m_charProps.Special;
                    break;
                case WCharacterFormat.NoProofKey:
                    if (m_charProps.Sprms[GetSprmOption(propertyKey)] != null)
                        this[NoProofKey] = m_charProps.NoProof;
                    break;
                case WCharacterFormat.ListHasPicKey:
                    if (m_charProps.Sprms[GetSprmOption(propertyKey)] != null)
                        this[ListHasPicKey] = m_charProps.ListHasImage;
                    break;
                case WCharacterFormat.FieldVanishKey:
                    if (m_charProps.Sprms[GetSprmOption(propertyKey)] != null)
                        this[FieldVanishKey] = m_charProps.FldVanish;
                    break;
                case WCharacterFormat.ComplexScriptKey:
                    if (m_charProps.Sprms[GetSprmOption(propertyKey)] != null)
                        this[ComplexScriptKey] = m_charProps.ComplexScript;
                    break;
                #endregion

                #region Numeric, color, enum properties
                case WCharacterFormat.TextColorKey:
                    if (Sprms[WordSprmOptions.sprmCIcoe] != null)
                        this[TextColorKey] = m_charProps.FontColorExt;
                    else if (Sprms[WordSprmOptions.sprmCIco] != null)
                        this[TextColorKey] = WordColor.ConvertIdToColor(m_charProps.FontColor);
                    break;
                case WCharacterFormat.FontSizeKey:
                    if (m_charProps == null && m_doc.DefCharFormat != null)
                        this[FontSizeKey] = m_doc.DefCharFormat.FontSize;
                    else if (m_charProps.Sprms[GetSprmOption(propertyKey)] != null)
                        this[FontSizeKey] = m_charProps.FontSize;
                    break;
                case WCharacterFormat.TextBkgColorKey:
                case WCharacterFormat.ForeColorKey:
                case WCharacterFormat.TextureStyleKey:
                    sprm = m_charProps.Sprms[WordSprmOptions.sprmCShdNew];
                    if (sprm == null)
                    {
                        sprm = m_charProps.Sprms[WordSprmOptions.sprmCShd];
                    }
                    if (sprm != null)
                    {
                        ShadingDescriptor shading = m_charProps.GetShading(sprm);
                        this[TextBkgColorKey] = shading.BackColor;
                        this[ForeColorKey] = shading.ForeColor;
                        this[TextureStyleKey] = shading.Pattern;
                    }
                    break;
                case WCharacterFormat.SubSuperScriptKey:
                    if (m_charProps.Sprms[GetSprmOption(propertyKey)] != null)
                        this[SubSuperScriptKey] = (SubSuperScript)m_charProps.SubSuperScript;
                    break;
                case WCharacterFormat.DoubleStrikeKey:
                    if (m_charProps.Sprms[GetSprmOption(propertyKey)] != null)
                        this[DoubleStrikeKey] = m_charProps.DoubleStrike;
                    break;
                case WCharacterFormat.PositionKey:
                    if (m_charProps.Sprms[GetSprmOption(propertyKey)] != null)
                        this[PositionKey] = (float)m_charProps.Position / DLSConstants.ShortSize;
                    break;
                case WCharacterFormat.SpacingKey:
                    if (m_charProps.Sprms[GetSprmOption(propertyKey)] != null)
                        this[SpacingKey] = (float)m_charProps.LineSpacing / DLSConstants.TwipsInOnePoint;
                    break;
                case WCharacterFormat.FontSizeBidiKey:
                    if (m_charProps.Sprms[GetSprmOption(propertyKey)] != null)
                        this[FontSizeBidiKey] = (float)m_charProps.FontSizeBi;
                    break;
                case WCharacterFormat.HighlightColorKey:
                    if (m_charProps.Sprms[GetSprmOption(propertyKey)] != null)
                        this[HighlightColorKey] = WordColor.ColorsArray[(int)m_charProps.HighlightColor];
                    break;
                case WCharacterFormat.BorderKey:
                    m_cancelOnChange = true;
                    sprm = m_charProps.Sprms[WordSprmOptions.sprmCBrc];
                    if (sprm != null)
                    {
                        BorderCode brc = m_charProps.GetBorder(sprm);
                        ParagraphPropertiesConverter.ExportBorder(brc, (Border)this[BorderKey]);
                    }
                    m_cancelOnChange = false;
                    break;
                case WCharacterFormat.LocaleIdASCIIKey:
                    if (m_charProps.Sprms[GetSprmOption(propertyKey)] != null)
                        this[LocaleIdASCIIKey] = m_charProps.LocationIdASCII;
                    break;
                case WCharacterFormat.LocaleIdFarEastKey:
                    if (m_charProps.Sprms[GetSprmOption(propertyKey)] != null)
                        this[LocaleIdFarEastKey] = m_charProps.LocationIdFarEast;
                    break;
                case WCharacterFormat.RgLid3Key:
                    if (m_charProps.Sprms[GetSprmOption(propertyKey)] != null)
                        this[RgLid3Key] = m_charProps.RgLid3;
                    break;
                case WCharacterFormat.RgLid3_2Key:
                    if (m_charProps.Sprms[GetSprmOption(propertyKey)] != null)
                    this[RgLid3_2Key] = m_charProps.RgLid3_2;
                    break;
                case WCharacterFormat.LidKey:
                    if (m_charProps.Sprms[GetSprmOption(propertyKey)] != null)
                        this[LidKey] = m_charProps.Lid;
                    break;
                case WCharacterFormat.LidBiKey:
                    if (m_charProps.Sprms[GetSprmOption(propertyKey)] != null)
                        this[LidBiKey] = m_charProps.LidBi;
                    break;
                case WCharacterFormat.ListPicIndexKey:
                    if (m_charProps.Sprms[GetSprmOption(propertyKey)] != null)
                        this[ListPicIndexKey] = m_charProps.ListPictureIndex;
                    break;
                case WCharacterFormat.PicLocationKey:
                    if (m_charProps.Sprms[GetSprmOption(propertyKey)] != null)
                        this[PicLocationKey] = m_charProps.PicLocation;
                    break;
                case WCharacterFormat.FieldVanishCompKey:
                    if (m_charProps.Sprms[GetSprmOption(propertyKey)] != null)
                        this[FieldVanishCompKey] = m_charProps.FldVanishComplex;
                    break;
                #endregion
            }

            m_charProps.BaseProperties = null;
        }
        #endregion

        #region Implementation / Import contents
        /// <summary>
        /// Merges the format.
        /// </summary>
        /// <param name="destinationFormat">The destination format.</param>
        internal void MergeFormat(WCharacterFormat destinationFormat)
        {
            Dictionary<int, object> properties = new Dictionary<int, object>();
            //Updates the source format to be preserved in the destination, for import option - merge formatting.
            if (Bidi) 
                properties.Add(BidiKey, true);
            if (Bold)
                properties.Add(BoldKey, true);
            if (BoldBidi)
                properties.Add(BoldBidiKey, true);
            if (ComplexScript)
                properties.Add(ComplexScriptKey, true);
            if (Hidden)
                properties.Add(HiddenKey, true);
            if (Italic)
                properties.Add(ItalicKey, true);
            if (ItalicBidi)
                properties.Add(ItalicBidiKey, true);
            if (SubSuperScript != SubSuperScript.None)
                properties.Add(SubSuperScriptKey, SubSuperScript);
            if (UnderlineStyle != UnderlineStyle.None)
                properties.Add(UnderlineKey, UnderlineStyle);
            properties.Add(LocaleIdASCIIKey, LocaleIdASCII);
            properties.Add(LocaleIdFarEastKey, LocaleIdFarEast);

            //Clears the source format.
            if (m_sprms != null)
                m_sprms.Clear();
            CharStyleName = null;
            //Imports the destination format.
            ImportContainer(destinationFormat);
            CopyProperties(destinationFormat);
            //Applys base format.
            ApplyBase(destinationFormat.BaseFormat);
            //Updates the source format to be preserved in the destination, for import option - merge formatting.
            UpdateFormattings(properties);
            //Clears the key, values collection.
            properties.Clear();
        }
        /// <summary>
        /// Updates the formattings.
        /// </summary>
        /// <param name="properties">The properties.</param>
        private void UpdateFormattings(Dictionary<int, object> properties)
        {
            foreach (KeyValuePair<int, object> keyValue in properties)
            {
                switch (keyValue.Key)
                {
                    case BidiKey:
                        if (Bidi != (bool)keyValue.Value)
                            Bidi = (bool)keyValue.Value;
                        break;
                    case BoldKey:
                        if (Bold != (bool)keyValue.Value)
                            Bold = (bool)keyValue.Value;
                        break;
                    case BoldBidiKey:
                        if (BoldBidi != (bool)keyValue.Value)
                            BoldBidi = (bool)keyValue.Value;
                        break;
                    case ComplexScriptKey:
                        if (ComplexScript != (bool)keyValue.Value)
                            ComplexScript = (bool)keyValue.Value;
                        break;
                    case HiddenKey:
                        if (Hidden != (bool)keyValue.Value)
                            Hidden = (bool)keyValue.Value;
                        break;
                    case ItalicKey:
                        if (Italic != (bool)keyValue.Value)
                            Italic = (bool)keyValue.Value;
                        break;
                    case ItalicBidiKey:
                        if (ItalicBidi != (bool)keyValue.Value)
                            ItalicBidi = (bool)keyValue.Value;
                        break;
                    case SubSuperScriptKey:
                        if (SubSuperScript != (SubSuperScript)keyValue.Value)
                            SubSuperScript = (SubSuperScript)keyValue.Value;
                        break;
                    case UnderlineKey:
                        if (UnderlineStyle != (UnderlineStyle)keyValue.Value)
                            UnderlineStyle = (UnderlineStyle)keyValue.Value;
                        break;
                    case LocaleIdASCIIKey:
                        if (LocaleIdASCII != (short)keyValue.Value)
                            LocaleIdASCII = (short)keyValue.Value;
                        break;
                    case LocaleIdFarEastKey:
                        if (LocaleIdFarEast != (short)keyValue.Value)
                            LocaleIdFarEast = (short)keyValue.Value;
                        break;
                }
            }
        }
        /// <summary>
        /// Updates the source format.
        /// </summary>
        /// <param name="destBaseFormat">The dest base format.</param>
        internal void UpdateSourceFormat(WCharacterFormat destBaseFormat)
        {
            WCharacterFormat format = new WCharacterFormat(destBaseFormat.Document);
            //Imports the source format.
            format.ImportContainer(this);
            format.CopyProperties(this);
            format.ApplyBase(destBaseFormat);
            format.CharStyleName = null;
            //Updates direct formatting inorder to preseve the contents similar to source document.
            UpdateSourceFormatting(format);
            //Imports the source format.
            ImportContainer(format);
            CopyProperties(format);
            format.Close();
        }
        /// <summary>
        /// Updates the source formatting.
        /// </summary>
        /// <param name="format">The format.</param>
        private void UpdateSourceFormatting(WCharacterFormat format)
        {
            if (format.AllCaps != AllCaps)
                format.AllCaps = AllCaps;
            if (format.Bidi != Bidi)
                format.Bidi = Bidi;
            if (format.Bold != Bold)
                format.Bold = Bold;
            if (format.BoldBidi != BoldBidi)
                format.BoldBidi = BoldBidi;
            if (format.CharacterSpacing != CharacterSpacing)
                format.CharacterSpacing = CharacterSpacing;
            if (format.ComplexScript != ComplexScript)
                format.ComplexScript = ComplexScript;
            if (format.DoubleStrike != DoubleStrike)
                format.DoubleStrike = DoubleStrike;
            if (format.Emboss != Emboss)
                format.Emboss = Emboss;
            if (format.Engrave != Engrave)
                format.Engrave = Engrave;
            if (format.FieldVanish != FieldVanish)
                format.FieldVanish = FieldVanish;
            if (format.FieldVanishComplex != FieldVanishComplex)
                format.FieldVanishComplex = FieldVanishComplex;
            if (format.FontName != FontName)
                format.FontName = FontName;
            if (format.FontNameAscii != FontNameAscii)
                format.FontNameAscii = FontNameAscii;
            if (format.FontNameBidi != FontNameBidi)
                format.FontNameBidi = FontNameBidi;
            if (format.FontNameFarEast != FontNameFarEast)
                format.FontNameFarEast = FontNameFarEast;
            if (format.FontNameNonFarEast != FontNameNonFarEast)
                format.FontNameNonFarEast = FontNameNonFarEast;
            if (format.FontSize != FontSize)
                format.FontSize = FontSize;
            if (format.FontSizeBidi != FontSizeBidi)
                format.FontSizeBidi = FontSizeBidi;
            if (format.ForeColor != ForeColor)
                format.ForeColor = ForeColor;
            if (format.Hidden != Hidden)
                format.Hidden = Hidden;
            if (format.HighlightColor != HighlightColor)
                format.HighlightColor = HighlightColor;
            if (format.IdctHint != IdctHint)
                format.IdctHint = IdctHint;
            if (format.Italic != Italic)
                format.Italic = Italic;
            if (format.ItalicBidi != ItalicBidi)
                format.ItalicBidi = ItalicBidi;
            if (format.Lid != Lid)
                format.Lid = Lid;
            if (format.LidBi != LidBi)
                format.LidBi = LidBi;
            if (format.Ligatures != Ligatures)
                format.Ligatures = Ligatures;
            if (format.LineBreak != LineBreak)
                format.LineBreak = LineBreak;
            if (format.LocaleIdASCII != LocaleIdASCII)
                format.LocaleIdASCII = LocaleIdASCII;
            if (format.LocaleIdFarEast != LocaleIdFarEast)
                format.LocaleIdFarEast = LocaleIdFarEast;
            if (format.NoProof != NoProof)
                format.NoProof = NoProof;
            if (format.NumberForm != NumberForm)
                format.NumberForm = NumberForm;
            if (format.NumberSpacing != NumberSpacing)
                format.NumberSpacing = NumberSpacing;
            if (format.OutLine != OutLine)
                format.OutLine = OutLine;
            if (format.PicLocation != PicLocation)
                format.PicLocation = PicLocation;
            if (format.Position != Position)
                format.Position = Position;
            if (format.RgLid3 != RgLid3)
                format.RgLid3 = RgLid3;
            if (format.RgLid3_2 != RgLid3_2)
                format.RgLid3_2 = RgLid3_2;
            if (format.Shadow != Shadow)
                format.Shadow = Shadow;
            if (format.SmallCaps != SmallCaps)
                format.SmallCaps = SmallCaps;
            if (format.Special != Special)
                format.Special = Special;
            if (format.Strikeout != Strikeout)
                format.Strikeout = Strikeout;
            if (format.StylisticSet != StylisticSet)
                format.StylisticSet = StylisticSet;
            if (format.SubSuperScript != SubSuperScript)
                format.SubSuperScript = SubSuperScript;
            if (format.TextBackgroundColor != TextBackgroundColor)
                format.TextBackgroundColor = TextBackgroundColor;
            if (format.TextColor != TextColor)
                format.TextColor = TextColor;
            if (format.TextureStyle != TextureStyle)
                format.TextureStyle = TextureStyle;
            if (format.UnderlineStyle != UnderlineStyle)
                format.UnderlineStyle = UnderlineStyle;
            if (format.UseContextualAlternates != UseContextualAlternates)
                format.UseContextualAlternates = UseContextualAlternates;
            
            //Update composite format
            Border.UpdateSourceFormatting(format.Border);
        }
        #endregion
    }
}
