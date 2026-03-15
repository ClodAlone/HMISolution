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
using System.IO;
using System.Text;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO.ReaderWriter.Biff_Records;
using Syncfusion.DocIO.ReaderWriter.Escher;
#if SILVERLIGHT || WP
using Image = Syncfusion.DocIO.DLS.Entities.Image;
using Syncfusion.DocIO.DLS.Entities;
#else
using Image = System.Drawing.Image;
using Metafile = System.Drawing.Imaging.Metafile;
using ImageFormat = System.Drawing.Imaging.ImageFormat;
#endif
#if !WINRT && !WP
using System.Drawing;
#endif
#endregion

namespace Syncfusion.DocIO.ReaderWriter.DataStreamParser.Escher
{
    /// <summary>
    /// Shape Container msofbtSpContainer
    /// A shape is the elemental object that composes a drawing. All graphical figures on a drawing 
    /// are shapes. Each shape has a list of properties, which is stored in an array. 
    /// </summary>
    internal class MsofbtSpContainer : BaseContainer
    {
        #region Class constants
        /// <summary>
        /// Constant value.
        /// </summary>
        public const int DEF_TXID_INCREMENT = 65536;

        /// <summary>
        /// Specifies Word Picture watermark.
        /// </summary>
        public const string DEF_PICTMARK_STRING = "WordPictureWatermark";

        /// <summary>
        /// Specifies Power plus water mark object.
        /// </summary>
        public const string DEF_TEXTMARK_STRING = "PowerPlusWaterMarkObject";
        /// Specifies Null string.
        /// </summary>
        public const string DEF_NULL_STRING = "\0";
        /// <summary>
        /// Specifies NotAllowInCell
        /// </summary>
        private const uint DEF_NOTALLOWINCELL = 2147483648;

        /// <summary>
        #endregion

        #region Class members
        /// <summary>
        /// 
        /// </summary>
        private MsofbtBSE m_bse;
        private bool m_isWatermark;
        #endregion

        #region Class properties
        /// <summary>
        /// 
        /// </summary>
        internal MsofbtBSE Bse
        {
            get
            {
                return m_bse;
            }
            set
            {
                m_bse = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal int Pib
        {
            get
            {
                if (ShapeOptions.Pib != null)
                {
                    return (int)ShapeOptions.Pib.Value;
                }
                else
                {
                    return -1;
                }
            }
            set
            {
                ShapeOptions.Pib.Value = ((uint)value);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal MsofbtSp Shape
        {
            get
            {
                return (FindContainerByType(typeof(MsofbtSp)) as MsofbtSp);
            }
            set
            {
                MsofbtSp shape = (MsofbtSp)FindContainerByType(typeof(MsofbtSp));
                shape = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal MsofbtOPT ShapeOptions
        {
            get
            {
                return (FindContainerByType(typeof(MsofbtOPT)) as MsofbtOPT);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal int Txid
        {
            get
            {
                //        if( ShapeOptions == null )
                //        {
                //          return 0;
                //        }
                //        
                //        FOPTEBid fopteBid = ShapeOptions.Properties[ DEF_TXID ] as FOPTEBid;
                //        
                //        if( fopteBid == null )
                //        {
                //          return 0;
                //        }
                //        return ( int )fopteBid.Value;

                if (ShapeOptions.Txid != null)
                {
                    return (int)ShapeOptions.Txid.Value;
                }
                else
                {
                    return -1;
                }
            }
            set
            {
                if (ShapeOptions == null)
                {
                    throw new ArgumentNullException("Shape options are null.");
                }

                FOPTEBid fopteBid = ShapeOptions.Properties[msofbtRGFOPTE.DEF_TXID] as FOPTEBid;

                if (fopteBid == null)
                {
                    throw new ArgumentException("Txid property does not exist.");
                }
                fopteBid.Value = ((uint)value);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal MsofbtTertiaryFOPT ShapePosition
        {
            get
            {
                return FindContainerByType(typeof(MsofbtTertiaryFOPT)) as MsofbtTertiaryFOPT;
            }
        }
        /// <summary>
        /// Defines if current shape container is watermark.
        /// </summary>
        internal bool IsWatermark
        {
            get
            {
                return m_isWatermark;
            }
            set
            {
                m_isWatermark = value;
            }
        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// 
        /// </summary>
        internal MsofbtSpContainer(WordDocument doc)
            : base(MSOFBT.msofbtSpContainer, doc)
        {
        }
        #endregion

        #region Implementation / common
        /// <summary>
        /// Get uint property value by key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public uint GetPropertyValue(int key)
        {
            MsofbtOPT containerOptions = this.ShapeOptions;
            if (containerOptions != null)
            {
                return containerOptions.GetPropertyValue(key);
            }
            return uint.MaxValue;
        }
        /// <summary>
        /// Get complex property value.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public byte[] GetComplexPropValue(int key)
        {
            MsofbtOPT containerOptions = this.ShapeOptions;
            if (containerOptions != null)
            {
                return containerOptions.GetComplexPropValue(key);
            }
            return null;
        }
        #endregion

        #region Implementation / background
        /// <summary>
        /// Defines if document has any background effects.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if [is filled background] [the specified back container]; otherwise, <c>false</c>.
        /// </returns>
        internal bool HasFillEffect()
        {
            bool isFilled = false;
            if (ShapeOptions == null)
                return false;
            if (!ShapeOptions.Properties.ContainsKey((int)FOPTEFillStyle.fNoFillHitTest))
            {
                return true;
            }

            uint prop = GetPropertyValue((int)FOPTEFillStyle.fNoFillHitTest);
            if (prop != uint.MaxValue)
            {
                isFilled = ((prop & 0x10) == 16) ? true : false;
            }
            return isFilled;
        }
        /// <summary>
        /// Gets the type of the background fill.
        /// </summary>
        /// <returns></returns>
        internal BackgroundFillType GetBackgroundFillType()
        {
            uint fillValue = GetPropertyValue((int)FOPTEFillStyle.fillType);
            if (fillValue != uint.MaxValue)
            {
                return (BackgroundFillType)fillValue;
            }

            return BackgroundFillType.msofillSolid;
        }
        /// <summary>
        /// Gets the type of the background.
        /// </summary>
        /// <returns></returns>
        internal BackgroundType GetBackgroundType()
        {
            uint fillValue = GetPropertyValue((int)FOPTEFillStyle.fillType);
            if (fillValue != uint.MaxValue)
            {
                BackgroundFillType fillType = (BackgroundFillType)fillValue;
                switch (fillType)
                {
                    case BackgroundFillType.msofillTexture:
                        return BackgroundType.Texture;
                    case BackgroundFillType.msofillPicture:
                        return BackgroundType.Picture;
                    case BackgroundFillType.msofillPattern:
                        return BackgroundType.NoBackground;
                    default:
                        return BackgroundType.Gradient;
                }
            }

            return BackgroundType.NoBackground;
        }
        /// <summary>
        /// Gets the background image or image bytes.
        /// </summary>
        /// <param name="escher">The escher.</param>
        /// <returns></returns>
        internal ImageRecord GetBackgroundImage(EscherClass escher)
        {
            uint prop = GetPropertyValue((int)FOPTEFillStyle.fillBlip);
            if (prop != uint.MaxValue && prop - 1 < escher.m_msofbtDggContainer.BstoreContainer.Children.Count)
            {
                MsofbtBSE bse = (MsofbtBSE)escher.m_msofbtDggContainer.BstoreContainer.Children[(int)prop - 1];
                if (bse.Blip != null)
                {
                    try
                    {
                        return bse.Blip.ImageRecord;
                    }
                    catch { }
                }
            }

            return null;
        }
        /// <summary>
        /// Gets the background image bytes.
        /// </summary>
        /// <param name="escher">The escher.</param>
        /// <returns></returns>
        internal byte[] GetBackgroundImBytes(EscherClass escher)
        {
            uint prop = GetPropertyValue((int)FOPTEFillStyle.fillBlip);
            if (prop != uint.MaxValue && prop - 1 < escher.m_msofbtDggContainer.BstoreContainer.Children.Count)
            {
                MsofbtBSE bse = (MsofbtBSE)escher.m_msofbtDggContainer.BstoreContainer.Children[(int)prop - 1];
                if (bse.Blip != null)
                {
                    try
                    {
                        return bse.Blip.ImageBytes;
                    }
                    catch { }
                }
            }

            return null;
        }
        /// <summary>
        /// Gets the color of the background.
        /// </summary>
        /// <param name="isPictureBackground">if set to <c>true</c> [returns picture background color].</param>
        /// <returns></returns>
        internal Color GetBackgroundColor(bool isPictureBackground)
        {
            Color backColor = Color.White;
            int propId = (isPictureBackground) ? (int)FOPTEFillStyle.fillBackColor : (int)FOPTEFillStyle.fillColor;
            uint prop = GetPropertyValue(propId);
            if (prop != uint.MaxValue)
            {
                backColor = WordColor.ConvertRGBToColor(prop);
            }

            return backColor;
        }

        /// <summary>
        /// Get shading style for gradient.
        /// </summary>
        /// <param name="fillType">Type of the fill.</param>
        /// <returns></returns>
        internal GradientShadingStyle GetGradientShadingStyle(BackgroundFillType fillType)
        {
            if (fillType == BackgroundFillType.msofillShadeCenter)
            {
                return GradientShadingStyle.FromCorner;
            }
            else if (fillType == BackgroundFillType.msofillShadeShape)
            {
                return GradientShadingStyle.FromCenter;
            }
            else
            {
                uint fillAngle = GetPropertyValue((int)FOPTEFillStyle.fillAngle);
                if (fillAngle != uint.MaxValue)
                {
                    switch (fillAngle)
                    {
                        case BackgroundGradient.DEF_VERTICAL_ANGLE:
                            {
                                return GradientShadingStyle.Vertical;
                            }
                        case BackgroundGradient.DEF_DIAGONALUP_ANGLE:
                            {
                                return GradientShadingStyle.DiagonalUp;
                            }
                        case BackgroundGradient.DEF_DIAGONALDOWN_ANGLE:
                            {
                                return GradientShadingStyle.DiagonalDown;
                            }
                    }
                }
                return GradientShadingStyle.Horizontal;
            }
        }
        /// <summary>
        /// Gets the shading variant.
        /// </summary>
        /// <param name="shadingStyle">The shading style.</param>
        /// <returns></returns>
        internal GradientShadingVariant GetGradientShadingVariant(GradientShadingStyle shadingStyle)
        {
            if (shadingStyle != GradientShadingStyle.FromCorner)
            {
                uint fillFocus = GetPropertyValue((int)FOPTEFillStyle.fillFocus);
                if (fillFocus != uint.MaxValue)
                {
                    switch (fillFocus)
                    {
                        case BackgroundGradient.DEF_SHADEOUT_VARIANT:
                            return GradientShadingVariant.ShadingOut;
                        case BackgroundGradient.DEF_SHADEMIDDLE_VARIANT:
                            return GradientShadingVariant.ShadingMiddle;
                        default:
                            return GradientShadingVariant.ShadingUp;
                    }
                }
                else
                {
                    return GradientShadingVariant.ShadingDown;
                }
            }
            else
            {
                return GetCornerStyleVariant();
            }
        }
        /// <summary>
        /// Create and init SpContainer of msosptRectangle shape type
        /// </summary>
        /// <returns></returns>
        internal MsofbtSpContainer CreateRectangleContainer()
        {
            //Init msofbtSpRect record
            MsofbtSp msofbtSpRect = new MsofbtSp(m_doc);
            msofbtSpRect.ShapeType = EscherShapeType.msosptRectangle;
            msofbtSpRect.HasAnchor = true;
            msofbtSpRect.HasShapeTypeProperty = true;
            msofbtSpRect.IsBackground = true;
            msofbtSpRect.ShapeId = 1025;

            //Set properties for textbox rectangle ( init MsofbtOPT )
            MsofbtOPT msofbtOPT = new MsofbtOPT(m_doc);
            msofbtOPT.Properties.Add(new FOPTEBid((int)FOPTEFillStyle.fNoFillHitTest, false, msofbtRGFOPTE.DEF_NO_COLOR_FILL));
            msofbtOPT.Properties.Add(new FOPTEBid((int)FOPTELineStyle.lineWidth, false, 0));
            msofbtOPT.Properties.Add(new FOPTEBid((int)FOPTELineStyle.lineStyleBooleanProperties, false, msofbtRGFOPTE.DEF_NO_LINE));
            msofbtOPT.Properties.Add(new FOPTEBid((int)FOPTEShape.bWMode, false, 9));
            msofbtOPT.Properties.Add(new FOPTEBid((int)FOPTEShape.fBackground, false, msofbtRGFOPTE.DEF_BACKGROND_SHAPE));
            msofbtOPT.Header.Instance = (msofbtOPT.Properties.Count);

            //Initialize clients data ( MsofbtClientData )
            MsofbtClientData clientData = new MsofbtClientData(m_doc);
            clientData.Data = new byte[4] { 1, 0, 0, 0 };
            //Initialize ShapePosition container
            MsofbtTertiaryFOPT msofbtShapePosition = new MsofbtTertiaryFOPT(m_doc);
            msofbtShapePosition.Unknown1 = 6291520;

            this.Children.Add(msofbtSpRect);
            this.Children.Add(msofbtOPT);
            this.Children.Add(msofbtShapePosition);
            this.Children.Add(clientData);

            return this;
        }
        /// <summary>
        /// Creates the background container.
        /// </summary>
        /// <param name="doc">The document.</param>
        /// <param name="background">The background.</param>
        internal void UpdateBackground(WordDocument doc, Background background)
        {
            if (background.Type == BackgroundType.NoBackground)
                return;

            CheckEscher(doc);
            EscherClass escher = doc.Escher;

            switch (background.Type)
            {
                case BackgroundType.Picture:
                case BackgroundType.Texture:
                    if (background.ImageRecord != null && background.ImageRecord.m_imageBytes != null)
                    {
                        m_bse = new MsofbtBSE(m_doc);
                        m_bse.Initialize(background.ImageRecord);

                        ShapeOptions.Properties.Remove((int)FOPTEFillStyle.fillBlip);
                        ShapeOptions.Properties.Remove((int)FOPTEFillStyle.fillType);

                        escher.m_msofbtDggContainer.BstoreContainer.Children.Add(m_bse);
                        int imageIndex = escher.m_msofbtDggContainer.BstoreContainer.Children.Count;
                        UpdateFillPicture(background, imageIndex);
                    }
                    break;
                case BackgroundType.Color:
                    UpdateFillColor(background.Color);
                    break;
                case BackgroundType.Gradient:
                    UpdateFillGradient(background.Gradient);
                    break;
            }

            if (background.Type != BackgroundType.Color)
                SetShapeOption(ShapeOptions.Properties, (uint)1310740, (int)FOPTEFillStyle.fNoFillHitTest, false);
        }
        /// <summary>
        /// Updates the fill gradient.
        /// </summary>
        /// <param name="gradient">The gradient.</param>
        internal void UpdateFillGradient(BackgroundGradient gradient)
        {
            //Set fill Color1 and Color2
            if (gradient.Color1 != Color.White)
            {
                uint fillColor = WordColor.ConvertColorToRGB(gradient.Color1);
                SetShapeOption(ShapeOptions.Properties, fillColor, (int)FOPTEFillStyle.fillColor, false);
            }
            if (gradient.Color2 != Color.White)
            {
                uint fillbackColor = WordColor.ConvertColorToRGB(gradient.Color2);
                SetShapeOption(ShapeOptions.Properties, fillbackColor, (int)FOPTEFillStyle.fillBackColor, false);
            }
            AddGradientFillAngle(gradient.ShadingStyle);
            //Set background fill type
            AddGradientFillType(gradient.ShadingStyle);
            //Set fill properies
            AddFillProperties(gradient.ShadingStyle, gradient.ShadingVariant);
            //Set gradient focus 
            AddGradientFocusFopte(gradient.ShadingStyle, gradient.ShadingVariant);
        }
        /// <summary>
        /// Updates the fill picture.
        /// </summary>
        /// <param name="fillBlipIndex">Index of the fill blip.</param>
        /// <param name="background">The background.</param>
        internal void UpdateFillPicture(Background background, int fillBlipIndex)
        {
            if (background.Type == BackgroundType.Picture)
            {
                SetShapeOption(ShapeOptions.Properties, (uint)BackgroundFillType.msofillPicture, (int)FOPTEFillStyle.fillType, false);
            }
            else
            {
                SetShapeOption(ShapeOptions.Properties, (uint)BackgroundFillType.msofillTexture, (int)FOPTEFillStyle.fillType, false);
            }

            if (background.PictureBackColor != Color.White)
            {
                uint backColor = WordColor.ConvertColorToRGB(background.PictureBackColor);
                SetShapeOption(ShapeOptions.Properties, backColor, (int)FOPTEFillStyle.fillBackColor, false);
            }

            SetShapeOption(ShapeOptions.Properties, (uint)fillBlipIndex, (int)FOPTEFillStyle.fillBlip, true);
            SetShapeOption(ShapeOptions.Properties, (uint)2, (int)FOPTEFillStyle.fillBlipFlags, false);
        }
        /// <summary>
        /// Updates the fill color of the container.
        /// </summary>
        /// <param name="color">The color.</param>
        internal void UpdateFillColor(Color color)
        {
            uint fillColor = Syncfusion.DocIO.DLS.WordColor.ConvertColorToRGB(color);
            if (fillColor == msofbtRGFOPTE.DEF_COLOR_EMPTY)
            {
                SetBoolShapeOption(ShapeOptions.Properties, (int)FOPTEFillStyle.fNoFillHitTest,
                  0x10, 4, 0, msofbtRGFOPTE.DEF_NO_COLOR_FILL);
            }
            else
            {
                uint defColor = Syncfusion.DocIO.DLS.WordColor.ConvertColorToRGB(Color.White, true);
                SetBoolShapeOption(ShapeOptions.Properties, (int)FOPTEFillStyle.fNoFillHitTest,
                  0x10, 4, 1, msofbtRGFOPTE.DEF_COLOR_FILL);
                if (fillColor != defColor)
                {
                    SetShapeOption(ShapeOptions.Properties, fillColor, (int)FOPTEFillStyle.fillColor, false);
                }

                // Set opacity
                uint opacity = GetOpacity(color.A);
                if (opacity != uint.MaxValue)
                {
                    SetShapeOption(ShapeOptions.Properties, opacity, (int)FOPTEFillStyle.fillOpacity, false);
                }
            }
        }
        #endregion

        #region Implementation / watermarks
        /// <summary>
        /// 
        /// </summary>
        /// <param name="watermarkNum"></param>
        /// <param name="textWatermark"></param>
        internal void CreateTextWatermarkContainer(int watermarkNum, TextWatermark textWatermark)
        {
            MsofbtSp msofbtSp = new MsofbtSp(m_doc);
            msofbtSp.ShapeType = EscherShapeType.msosptTextPlainText;
            msofbtSp.HasShapeTypeProperty = true;
            msofbtSp.HasAnchor = true;

            MsofbtOPT msofbtOPT = new MsofbtOPT(m_doc);
            if (textWatermark.Layout == WatermarkLayout.Diagonal)
            {
                msofbtOPT.Properties.Add(new FOPTEBid((int)FOPTETransform.rotation, false, 20643840));
            }

            //Add text record
            textWatermark.Text = textWatermark.Text == null ? string.Empty : textWatermark.Text;
            byte[] textData = Encoding.Unicode.GetBytes(textWatermark.Text.EndsWith(DEF_NULL_STRING) ? textWatermark.Text : textWatermark.Text + DEF_NULL_STRING);
            FOPTEComplex textValue = new FOPTEComplex((int)FOPTEGeoText.gtextUNICODE, false, textData.Length);
            textValue.Value = textData;
            msofbtOPT.Properties.Add(textValue);
            //Add text size record
            uint textSize = (uint)((int)textWatermark.Size << 16);
            msofbtOPT.Properties.Add(new FOPTEBid((int)FOPTEGeoText.gtextSize, false, textSize));
            //Add text font name record            
            byte[] fontData = Encoding.Unicode.GetBytes(textWatermark.FontName.EndsWith(DEF_NULL_STRING) ? textWatermark.FontName : textWatermark.FontName + DEF_NULL_STRING);
            FOPTEComplex fontValue = new FOPTEComplex((int)FOPTEGeoText.gtextFont, false, fontData.Length);
            fontValue.Value = fontData;
            msofbtOPT.Properties.Add(fontValue);
            //Add text color record
            uint textColor = Syncfusion.DocIO.DLS.WordColor.ConvertColorToRGB(textWatermark.Color);
            msofbtOPT.Properties.Add(new FOPTEBid((int)FOPTEFillStyle.fillColor, false, textColor));
            //Add semitransparent effect
            if (textWatermark.Semitransparent)
            {
                msofbtOPT.Properties.Add(new FOPTEBid((int)FOPTEFillStyle.fillOpacity, false, 32768));
            }
            //General options
            msofbtOPT.Properties.Add(new FOPTEBid((int)FOPTEFillStyle.fNoFillHitTest, false, 1048529));
            msofbtOPT.Properties.Add(new FOPTEBid((int)FOPTELineStyle.lineStyleBooleanProperties, false, 524288));
            msofbtOPT.Properties.Add(new FOPTEBid((int)FOPTEGroupShape.fPrint, false, 2097184));
            //Add shape name
            string spName = DEF_TEXTMARK_STRING + watermarkNum.ToString() + DEF_NULL_STRING;
            byte[] shapeNameData = Encoding.Unicode.GetBytes(spName);
            FOPTEComplex shapeName = new FOPTEComplex((int)FOPTEGroupShape.wzName, false, shapeNameData.Length);
            shapeName.Value = shapeNameData;
            msofbtOPT.Properties.Add(shapeName);
            msofbtOPT.Header.Instance = msofbtOPT.Properties.Count;

            MsofbtTertiaryFOPT msofbtShapePosition = new MsofbtTertiaryFOPT(m_doc);
            msofbtShapePosition.XAlign = (uint)HorizontalAlignment.Right;
            msofbtShapePosition.YAlign = (uint)VerticalAlignment.Bottom;
            msofbtShapePosition.XRelTo = (uint)0;
            msofbtShapePosition.YRelTo = (uint)0;
            msofbtShapePosition.AllowInTableCell = false;

            MsofbtClientAnchor clientAnchor = new MsofbtClientAnchor(m_doc);
            clientAnchor.Data = new byte[4] { 2, 0, 0, 0 };

            MsofbtClientData clientData = new MsofbtClientData(m_doc);
            clientData.Data = new byte[4] { 1, 0, 0, 0 };

            this.Children.Add(msofbtSp);
            this.Children.Add(msofbtOPT);
            this.Children.Add(msofbtShapePosition);
            this.Children.Add(clientAnchor);
            this.Children.Add(clientData);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="watermarkNum"></param>
        /// <param name="pictWatermark"></param>
        internal void CreatePictWatermarkContainer(int watermarkNum, PictureWatermark pictWatermark)
        {
            _Blip blip = null;
            MsofbtSp msofbtSp = new MsofbtSp(m_doc);
            msofbtSp.ShapeType = EscherShapeType.msosptPictureFrame;
            msofbtSp.HasShapeTypeProperty = true;
            msofbtSp.HasAnchor = true;

            MsofbtOPT msofbtOPT = new MsofbtOPT(m_doc);
            msofbtOPT.Properties.Add(new FOPTEBid((int)FOPTEBlip.pib, true, 1));
            msofbtOPT.Properties.Add(new FOPTEBid((int)FOPTEBlip.pibFlags, false, 2));
            //Add washout effect
            if (pictWatermark.Washout)
            {
                msofbtOPT.Properties.Add(new FOPTEBid((int)FOPTEBlip.pictureContrast, false, 19661));
                msofbtOPT.Properties.Add(new FOPTEBid((int)FOPTEBlip.pictureBrightness, false, 22938));
            }
            //General options
            msofbtOPT.Properties.Add(new FOPTEBid((int)FOPTELineStyle.lineStyleBooleanProperties, false, 524288));
            msofbtOPT.Properties.Add(new FOPTEBid((int)FOPTEGroupShape.fPrint, false, 2097184));
            //Add shape name
            string spName = DEF_PICTMARK_STRING + watermarkNum.ToString() + DEF_NULL_STRING;
            byte[] shapeNameData = Encoding.Unicode.GetBytes(spName);
            FOPTEComplex shapeName = new FOPTEComplex((int)FOPTEGroupShape.wzName, false, shapeNameData.Length);
            shapeName.Value = shapeNameData;
            msofbtOPT.Properties.Add(shapeName);
            msofbtOPT.Header.Instance = msofbtOPT.Properties.Count;

            //If OriginalPib != -1 then we use msofbtBse container from original document. 
            if (pictWatermark.OriginalPib == -1)
            {
                if (pictWatermark.WordPicture.IsMetaFile)
                {
                    blip = new MsofbtMetaFile(pictWatermark.WordPicture.ImageRecord, m_doc);
                }
                else
                {
                    bool isBitmap = IsBitmap(pictWatermark.Picture.RawFormat);
                    blip = new MsofbtImage(pictWatermark.WordPicture.ImageRecord, isBitmap, m_doc);
                }

                Guid guid1 = blip.Uid;

                MsofbtBSE msofbtBSE = new MsofbtBSE(m_doc);
                msofbtBSE.Header.Instance = ((int)blip.Type);
                msofbtBSE.Fbse.m_btWin32 = (int)blip.Type;
                msofbtBSE.Fbse.m_btMacOS = (int)blip.Type;
                msofbtBSE.Fbse.m_rgbUid = guid1.ToByteArray();
                msofbtBSE.Fbse.m_tag = 255;
                msofbtBSE.Fbse.m_cRef = 1;
                // .gif pictures doesn't work with it
                //msofbtBSE.IsInlineBlip = true;
                msofbtBSE.Blip = blip;
                this.Bse = msofbtBSE;
            }

            MsofbtTertiaryFOPT msofbtShapePosition = new MsofbtTertiaryFOPT(m_doc);
            //Apply Picture properties to the Shape Position
            ApplyPictureProperties(msofbtShapePosition, pictWatermark.WordPicture);
            msofbtShapePosition.AllowInTableCell = false;

            MsofbtClientAnchor clientAnchor = new MsofbtClientAnchor(m_doc);
            clientAnchor.Data = new byte[4] { 2, 0, 0, 0 };
            MsofbtClientData clientData = new MsofbtClientData(m_doc);
            clientData.Data = new byte[4] { 1, 0, 0, 0 };

            this.Children.Add(msofbtSp);
            this.Children.Add(msofbtOPT);
            this.Children.Add(clientAnchor);
            this.Children.Add(clientData);
            this.Children.Add(msofbtShapePosition);
        }
        /// <summary>
        /// Apply Picture properties to the Shape Position
        /// </summary>
        /// <param name="msofbtShapePosition"></param>
        /// <param name="picture"></param>
        private void ApplyPictureProperties(MsofbtTertiaryFOPT msofbtShapePosition, WPicture picture)
        {
            if (picture.HorizontalAlignment != ShapeHorizontalAlignment.None)
                msofbtShapePosition.XAlign = (uint)picture.HorizontalAlignment;
            if (picture.VerticalAlignment != ShapeVerticalAlignment.None)
                msofbtShapePosition.YAlign = (uint)picture.VerticalAlignment;
            if (picture.HorizontalOrigin == HorizontalOrigin.LeftMargin || picture.HorizontalOrigin == HorizontalOrigin.RightMargin
                || picture.HorizontalOrigin == HorizontalOrigin.InsideMargin || picture.HorizontalOrigin == HorizontalOrigin.OutsideMargin)
                msofbtShapePosition.XRelTo = (uint)HorizontalOrigin.Margin;
            else
                msofbtShapePosition.XRelTo = (uint)picture.HorizontalOrigin;
            msofbtShapePosition.YRelTo = (uint)picture.VerticalOrigin;
        }
        #endregion

        #region Implementation / textbox, image
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pict"></param>
        /// <param name="pictProps"></param>
        /// <returns></returns>
        public MsofbtSpContainer CreateImageContainer(WPicture pict, PictureShapeProps pictProps)
        {
            _Blip blip;
            if (pict.ImageRecord.IsMetafile)
            {
                blip = new MsofbtMetaFile(pict.ImageRecord, m_doc);
            }
            else
            {
                bool isBitmap = IsBitmap(pict.Image.RawFormat);
                blip = new MsofbtImage(pict.ImageRecord, isBitmap, m_doc);
            }
            MsofbtSp msofbtSp = new MsofbtSp(m_doc);
            msofbtSp.ShapeType = EscherShapeType.msosptPictureFrame;
            msofbtSp.HasShapeTypeProperty = true;
            msofbtSp.HasAnchor = true;
            this.Children.Add(msofbtSp);

            MsofbtOPT msofbtOPT = new MsofbtOPT(m_doc);
            if (pict.PictureShape.ShapeContainer != null
                && pict.PictureShape.ShapeContainer.ShapeOptions != null)
                msofbtOPT = pict.PictureShape.ShapeContainer.ShapeOptions.Clone() as MsofbtOPT;

            this.Children.Add(msofbtOPT);

            this.WritePictureOptions(pictProps, pict);
            msofbtOPT.Header.Instance = msofbtOPT.Properties.Count;

            MsofbtBSE msofbtBSE = new MsofbtBSE(m_doc);
            msofbtBSE.Header.Instance = ((int)blip.Type);
            msofbtBSE.Fbse.m_btWin32 = (int)blip.Type;
            msofbtBSE.Fbse.m_btMacOS = (int)blip.Type;
            msofbtBSE.Fbse.m_rgbUid = blip.Uid.ToByteArray();
            msofbtBSE.Fbse.m_tag = 255;
            msofbtBSE.Fbse.m_cRef = 1;

            msofbtBSE.Blip = blip;
            this.Bse = msofbtBSE;

            MsofbtClientAnchor clientAnchor = new MsofbtClientAnchor(m_doc);
            clientAnchor.Data = new byte[4] { 0, 0, 0, 0 };

            MsofbtClientData clientData = new MsofbtClientData(m_doc);
            clientData.Data = new byte[4] { 1, 0, 0, 0 };

            this.Children.Add(clientAnchor);
            this.Children.Add(clientData);
            this.Shape.ShapeId = pictProps.Spid;

            MsofbtTertiaryFOPT msofbtShapePosition = new MsofbtTertiaryFOPT(m_doc);
            this.Children.Add(msofbtShapePosition);

            this.ShapePosition.XAlign = (uint)pictProps.HorizontalAlignment;
            this.ShapePosition.XRelTo = (uint)pictProps.RelHrzPos;
            this.ShapePosition.YAlign = (uint)pictProps.VerticalAlignment;
            this.ShapePosition.YRelTo = (uint)pictProps.RelVrtPos;

            return this;
        }
        /// <summary>
        /// Creates the inline image container.
        /// </summary>
        /// <param name="pict">The picture.</param>
        /// <returns></returns>
        public MsofbtSpContainer CreateInlineImageContainer(WPicture pict)
        {
            _Blip blip;
            MsofbtSp msofbtSp = new MsofbtSp(m_doc);
            msofbtSp.ShapeType = (EscherShapeType.msosptPictureFrame);
            msofbtSp.HasShapeTypeProperty = true;
            msofbtSp.HasAnchor = true;
            MsofbtOPT msofbtOPT = new MsofbtOPT(m_doc);
            msofbtOPT.Properties.Add(new FOPTEBid(260, true, 1));
            msofbtOPT.Properties.Add(new FOPTEBid(262, false, 2));

            // Add picture alternative text
            if (!string.IsNullOrEmpty(pict.AlternativeText))
            {
                if (msofbtOPT.Properties.ContainsKey((int)FOPTEGroupShape.wzDescription))
                    msofbtOPT.Properties.Remove((int)FOPTEGroupShape.wzDescription);
                byte[] textData = Encoding.Unicode.GetBytes(pict.AlternativeText + "\0");
                FOPTEComplex textValue = new FOPTEComplex((int)FOPTEGroupShape.wzDescription, false, textData.Length);
                textValue.Value = textData;
                msofbtOPT.Properties.Add(textValue);
            }

            msofbtOPT.Header.Instance = msofbtOPT.Properties.Count;

            if (pict.ImageRecord.IsMetafile)
            {
                blip = new MsofbtMetaFile(pict.ImageRecord, m_doc);
            }
            else
            {
                bool isBitmap = IsBitmap(pict.ImageRecord.ImageFormat);
                blip = new MsofbtImage(pict.ImageRecord, isBitmap, m_doc);
            }

            Guid guid1 = blip.Uid;
            MsofbtBSE msofbtBSE = new MsofbtBSE(m_doc);
            msofbtBSE.Header.Instance = ((int)blip.Type);
            msofbtBSE.Fbse.m_btWin32 = (int)blip.Type;
            msofbtBSE.Fbse.m_btMacOS = (int)blip.Type;
            msofbtBSE.Fbse.m_rgbUid = guid1.ToByteArray();
            msofbtBSE.Fbse.m_tag = 255;
            msofbtBSE.Fbse.m_cRef = 1;
            msofbtBSE.IsInlineBlip = true;
            msofbtBSE.Blip = blip;

            MsofbtClientAnchor clientAnchor = new MsofbtClientAnchor(m_doc);
            clientAnchor.Data = new byte[4] { 0, 0, 0, 128 };

            this.Children.Add(msofbtSp);
            this.Children.Add(msofbtOPT);
            this.Children.Add(clientAnchor);

            if (pict.PictureShape.ShapeContainer != null && pict.PictureShape.ShapeContainer.ShapePosition != null)
                this.Children.Add(pict.PictureShape.ShapeContainer.ShapePosition);
            this.Bse = msofbtBSE;

            return this;
        }
        /// <summary>
        /// Creates the textbox container.
        /// </summary>
        /// <param name="txbxFormat">The TXBX format.</param>
        /// <returns></returns>
        public MsofbtSpContainer CreateTextBoxContainer(WTextBoxFormat txbxFormat)
        {
            MsofbtSp msofbtSp = new MsofbtSp(m_doc);
            msofbtSp.ShapeType = EscherShapeType.msosptTextBox;
            msofbtSp.HasShapeTypeProperty = true;
            msofbtSp.HasAnchor = true;
            this.Children.Add(msofbtSp);

            MsofbtOPT msofbtOPT = new MsofbtOPT(m_doc);
            this.Children.Add(msofbtOPT);
            this.WriteTextBoxOptions(txbxFormat);
            msofbtOPT.Header.Instance = msofbtOPT.Properties.Count;

            //Additional records initialization
            MsofbtClientAnchor clientAnchor = new MsofbtClientAnchor(m_doc);
            clientAnchor.Data = new byte[4] { 0, 0, 0, 0 };

            MsofbtClientData clientData = new MsofbtClientData(m_doc);
            clientData.Data = new byte[4] { 1, 0, 0, 0 };

            MsofbtClientTextbox clientTextbox = new MsofbtClientTextbox(m_doc);
            clientTextbox.Txid = (int)txbxFormat.TextBoxIdentificator;

            this.Children.Add(clientAnchor);
            this.Children.Add(clientData);
            this.Children.Add(clientTextbox);

            MsofbtTertiaryFOPT msofbtShapePosition = new MsofbtTertiaryFOPT(m_doc);
            this.Children.Add(msofbtShapePosition);
            this.ShapePosition.XAlign = (uint)txbxFormat.HorizontalAlignment;
            this.ShapePosition.YAlign = (uint)txbxFormat.VerticalAlignment;
            if (txbxFormat.TextWrappingStyle == TextWrappingStyle.Inline)
            {
                this.ShapePosition.XRelTo = (uint)HorizontalOrigin.Character;
                this.ShapePosition.YRelTo = (uint)VerticalOrigin.Line;
                this.ShapePosition.Unknown1 = 6291456;
                this.ShapePosition.Unknown2 = 65537;
            }
            else
            {
                if (txbxFormat.HorizontalOrigin == HorizontalOrigin.LeftMargin || txbxFormat.HorizontalOrigin == HorizontalOrigin.RightMargin 
                    || txbxFormat.HorizontalOrigin == HorizontalOrigin.InsideMargin || txbxFormat.HorizontalOrigin == HorizontalOrigin.OutsideMargin)
                    this.ShapePosition.XRelTo = (uint)HorizontalOrigin.Margin;
                else
                    this.ShapePosition.XRelTo = (uint)txbxFormat.HorizontalOrigin;
                if (txbxFormat.VerticalOrigin == VerticalOrigin.TopMargin || txbxFormat.VerticalOrigin == VerticalOrigin.BottomMargin || txbxFormat.VerticalOrigin == VerticalOrigin.InsideMargin || txbxFormat.VerticalOrigin == VerticalOrigin.OutsideMargin)
                    this.ShapePosition.YRelTo = (uint)VerticalOrigin.Page;
                else
                    this.ShapePosition.YRelTo = (uint)txbxFormat.VerticalOrigin;
            }
            this.ShapePosition.AllowInTableCell = true;
            this.Shape.ShapeId = txbxFormat.TextBoxShapeID;

            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="imageFormat"></param>
        /// <returns></returns>
        public bool IsMetafile(ImageFormat imageFormat)
        {
            return (imageFormat.Equals(ImageFormat.Emf) || imageFormat.Equals(ImageFormat.Wmf));
        }
        /// <summary>
        /// Determines whether the specified image format is bitmap.
        /// </summary>
        /// <param name="imageFormat">The image format.</param>
        /// <returns>
        /// 	<c>true</c> if the specified image format is bitmap; otherwise, <c>false</c>.
        /// </returns>
        private bool IsBitmap(ImageFormat imageFormat)
        {
            return (imageFormat.Equals(ImageFormat.Png) || imageFormat.Equals(ImageFormat.Bmp)
                || imageFormat.Equals(ImageFormat.MemoryBmp));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        public void WriteContainer(Stream stream)
        {
            this.WriteMsofbhWithRecord(stream);

            if (this.Bse != null)
            {
                this.Bse.WriteMsofbhWithRecord(stream);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal MsofbtSpContainer CreateInlineTxbxImageCont()
        {
            MsofbtSp msofbtSp = new MsofbtSp(m_doc);
            msofbtSp.ShapeType = EscherShapeType.msosptPictureFrame;
            msofbtSp.HasShapeTypeProperty = true;
            msofbtSp.HasAnchor = true;
            MsofbtOPT msofbtOPT = new MsofbtOPT(m_doc);
            msofbtOPT.Properties.Add(new FOPTEBid(127, false, 20971840));
            msofbtOPT.Header.Instance = msofbtOPT.Properties.Count;

            MsofbtClientAnchor clientAnchor = new MsofbtClientAnchor(m_doc);
            clientAnchor.Data = new byte[4] { 0, 0, 0, 128 };

            MsofbtTertiaryFOPT msofbtShapePosition = new MsofbtTertiaryFOPT(m_doc);
            msofbtShapePosition.Unknown2 = 65537;

            this.Children.Add(msofbtSp);
            this.Children.Add(msofbtOPT);
            this.Children.Add(msofbtShapePosition);
            this.Children.Add(clientAnchor);

            return this;
        }

        /// <summary>
        /// Checks the options container(fixed the problem of picture preservation for
        /// documents generated by old version of DocIO ).
        /// </summary>
        internal void CheckOptContainer()
        {
            if (this.ShapeOptions == null)
            {
                MsofbtOPT msofbtOPT = new MsofbtOPT(m_doc);
                msofbtOPT.Properties.Add(new FOPTEBid(260, true, 1));
                msofbtOPT.Properties.Add(new FOPTEBid(262, false, 2));
                msofbtOPT.Header.Instance = msofbtOPT.Properties.Count;
                this.Children.Add(msofbtOPT);
            }
        }
        #endregion

        #region Class static methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="length"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static MsofbtSpContainer ReadInlineImageContainers(int length, Stream stream, WordDocument doc)
        {
            ContainerCollection containerCollection = new ContainerCollection(doc);

            containerCollection.Read(stream, length);
            MsofbtSpContainer spContainer = containerCollection[0] as MsofbtSpContainer;

            if (spContainer != null && containerCollection.Count > 1)
            {
                spContainer.Bse = (containerCollection[1] as MsofbtBSE);
            }
            return spContainer;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="escherRecord"></param>
        /// <returns></returns>
        public static _Blip GetBlipFromShapeContainer(BaseEscherRecord escherRecord)
        {
            if (escherRecord == null)
            {
                throw new NullReferenceException("Container is null");
            }

            MsofbtSpContainer msofbtSpContainer = escherRecord as MsofbtSpContainer;
            if (msofbtSpContainer == null)
            {
                throw new ArgumentException("Container is not a shape container.");
            }

            MsofbtSp msofbtSp = msofbtSpContainer.Shape;
            if (msofbtSp.ShapeType != EscherShapeType.msosptPictureFrame || msofbtSpContainer.Bse == null)
            {
                return null;
            }

            return msofbtSpContainer.Bse.Blip;
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal override BaseEscherRecord Clone()
        {
            MsofbtSpContainer spContainer = (MsofbtSpContainer)base.Clone();
            if (m_bse != null)
            {
                spContainer.Bse = (MsofbtBSE)m_bse.Clone();
            }
            spContainer.m_doc = m_doc;
            return spContainer;
        }
        /// <summary>
        /// Clones the relations to.
        /// </summary>
        /// <param name="doc">The doc.</param>
        internal override void CloneRelationsTo(WordDocument doc)
        {
            m_doc = doc;
            Header.m_doc = doc;
            foreach (BaseEscherRecord record in Children)
            {
                record.m_doc = doc;
                record.Header.m_doc = doc;
            }
            if (Bse != null)
            {
                Bse.m_doc = doc;
                Bse.Header.m_doc = doc;
                if (Bse.Blip != null)
                {
                    Bse.Blip.m_doc = doc;
                    Bse.Blip.Header.m_doc = doc;
                    Size size = Bse.Blip.ImageRecord.Size;
                    ImageFormat imageFormat = Bse.Blip.ImageRecord.ImageFormat;
                    if (Bse.Blip is MsofbtMetaFile)
                        (Bse.Blip as MsofbtMetaFile).ImageRecord = doc.Images.LoadMetaFileImage(Bse.Blip.ImageRecord.m_imageBytes, true);
                    else
                        Bse.Blip.ImageRecord = doc.Images.LoadImage(Bse.Blip.ImageRecord.ImageBytes);
                    Bse.Blip.ImageRecord.Size = size;
                    Bse.Blip.ImageRecord.ImageFormat = imageFormat;
                }
            }
        }
        #endregion

        #region Textbox, image helper methods
        /// <summary>
        /// 
        /// </summary>
        internal void RemoveSpContainerOle()
        {
            Shape.IsOle = false;
            if (ShapeOptions.Properties.ContainsKey((int)FOPTEBlip.pictureId))
            {
                ShapeOptions.Properties.Remove((int)FOPTEBlip.pictureId);
            }
            FOPTEBid fopteBid = (FOPTEBid)ShapeOptions.Properties[(int)FOPTEBlip.pictureActive];
            if (fopteBid != null)
            {
                fopteBid.Value = (uint)BaseWordRecord.SetBitsByMask((int)fopteBid.Value, 0x01, 1, 0);
            }
        }
        /// <summary>
        /// Writes the textbox options.
        /// </summary>
        /// <param name="txbxFormat">The textbox format.</param>
        internal void WriteTextBoxOptions(WTextBoxFormat txbxFormat)
        {
            msofbtRGFOPTE shapeProps = this.ShapeOptions.Properties;

            //Set LockAgainsGrouping property
            if (txbxFormat.TextWrappingStyle == TextWrappingStyle.Inline)
            {
                SetBoolShapeOption(shapeProps, (int)FOPTEProtection.fLockAgainstGrouping,
                  0x40, 6, 0, 20971840);
            }
            //Read The text box wrap polygon vertices
            if (txbxFormat.TextWrappingStyle == TextWrappingStyle.Through || txbxFormat.TextWrappingStyle == TextWrappingStyle.Tight)
            {
                PointF[] data = txbxFormat.WrapPolygon.Vertices.ToArray();
                byte[] bytearray = new byte[6 + (data.Length * 8)];
                byte[] nElems = BitConverter.GetBytes((Int16)(data.Length));
                byte[] nElemsAlloc = nElems;
                byte[] cbElem = BitConverter.GetBytes((Int16)8);
                int offset = 0;
                Array.Copy(nElems, 0, bytearray, offset, 2);
                offset += 2;
                Array.Copy(nElemsAlloc, 0, bytearray, offset, 2);
                offset += 2;
                Array.Copy(cbElem, 0, bytearray, offset, 2);
                offset += 2;
                for (int i = 0; i < data.Length; i++)
                {
                    byte[] datax = BitConverter.GetBytes((Int32)data[i].X);
                    Array.Copy(datax, 0, bytearray, offset, datax.Length);
                    offset += datax.Length;
                    byte[] datay = BitConverter.GetBytes((Int32)data[i].Y);
                    Array.Copy(datay, 0, bytearray, offset, datay.Length);
                    offset += datay.Length;
                }

                if (shapeProps.ContainsKey((int)FOPTEGroupShape.pWrapPolygonVertices))
                    shapeProps.Remove((int)FOPTEGroupShape.pWrapPolygonVertices);

                FOPTEComplex wrapPolygon = new FOPTEComplex((int)FOPTEGroupShape.pWrapPolygonVertices, false, bytearray.Length);
                wrapPolygon.Value = bytearray;
                shapeProps.Add(wrapPolygon);

            }
            //Set TXID option
            if (shapeProps.ContainsKey(MsofbtOPT.DEF_TXID))
            {
                shapeProps.Remove(MsofbtOPT.DEF_TXID);
            }
            shapeProps.Add(new FOPTEBid(MsofbtOPT.DEF_TXID, false, (uint)txbxFormat.TextBoxIdentificator));

            //Set Next textbox option
            if (shapeProps.ContainsKey((int)FOPTEText.hspNext))
            {
                shapeProps.Remove((int)FOPTEText.hspNext);
                shapeProps.Add(new FOPTEBid((int)FOPTEText.hspNext, false, (uint)txbxFormat.TextBoxShapeID));
            }

            //Set LineWidth option
            float prop = txbxFormat.LineWidth * msofbtRGFOPTE.DEF_LINE_WIDTH_PT;
            if (txbxFormat.LineWidth != msofbtRGFOPTE.DEF_LINE_WIDTH)
            {
                SetShapeOption(shapeProps, (uint)prop, (int)FOPTELineStyle.lineWidth, false);
            }

            //Set LineDashing option
            if (txbxFormat.LineDashing != LineDashing.Solid)
            {
                SetShapeOption(shapeProps, (uint)txbxFormat.LineDashing, (int)FOPTELineStyle.lineDashing, false);
            }

            //Set LineStyle options
            if (txbxFormat.LineStyle != TextBoxLineStyle.Simple)
            {
                SetShapeOption(shapeProps, (uint)txbxFormat.LineStyle, (int)FOPTELineStyle.lineStyle, false);
            }
            //Set textbox text direction
            if (txbxFormat.TextDirection != TextDirection.Horizontal)
            {
                if (shapeProps.ContainsKey((int)FOPTEText.txflTextFlow))
                {
                    shapeProps.Remove((int)FOPTEText.txflTextFlow);
                }
                shapeProps.Add(new FOPTEBid((int)FOPTEText.txflTextFlow, false, (uint)txbxFormat.TextDirection));
            }
            //Set Line Color
            uint curColor = WordColor.ConvertColorToRGB(txbxFormat.LineColor);
            uint defColor = WordColor.ConvertColorToRGB(Color.Black);
            if (curColor != defColor)
            {
                curColor = WordColor.ConvertColorToRGB(txbxFormat.LineColor, true);
                SetShapeOption(shapeProps, curColor, (int)FOPTELineStyle.lineColor, false);
                uint lineOpacity = GetOpacity(txbxFormat.LineColor.A);
                if (lineOpacity != uint.MaxValue)
                {
                    SetShapeOption(shapeProps, lineOpacity, (int)FOPTELineStyle.lineOpacity, false);
                }
            }

            //Set NoLine option
            if (txbxFormat.NoLine)
            {
                SetBoolShapeOption(shapeProps, (int)FOPTELineStyle.lineStyleBooleanProperties, 0x08, 3,
                  0, msofbtRGFOPTE.DEF_NO_LINE);
            }
            //Set fLayoutInCell option
            if (!txbxFormat.AllowInCell)
            {
                //Set fUsefLayoutInCell option
                SetBoolShapeOption(shapeProps, (int)FOPTEGroupShape.fPrint, 0x7FFFFFFF, 31, 1, DEF_NOTALLOWINCELL);
            }
            //Set IsBelowText option
            if (txbxFormat.TextWrappingStyle == TextWrappingStyle.Inline)
            {
                SetBoolShapeOption(shapeProps, (int)FOPTEGroupShape.fPrint, 0x01, 0,
                  0, 2097152);
            }
            else if (txbxFormat.IsBelowText)
            {
                SetBoolShapeOption(shapeProps, (int)FOPTEGroupShape.fPrint, 0x20, 5,
                  1, msofbtRGFOPTE.DEF_BEHIND_DOC);
            }

            // Update textbox internal margins
            uint margin = 0;
            if (txbxFormat.InternalMargin.Left != InternalMargin.DEF_HORIZMARGIN)
            {
                margin = (uint)Math.Round(txbxFormat.InternalMargin.Left * DLSConstants.EmusPerPoint);
                SetShapeOption(shapeProps, margin, (int)FOPTEText.dxTextLeft, false);
            }
            if (txbxFormat.InternalMargin.Right != InternalMargin.DEF_HORIZMARGIN)
            {
                margin = (uint)Math.Round(txbxFormat.InternalMargin.Right * DLSConstants.EmusPerPoint);
                SetShapeOption(shapeProps, margin, (int)FOPTEText.dxTextRight, false);
            }
            if (txbxFormat.InternalMargin.Top != InternalMargin.DEF_VERTMARGIN)
            {
                margin = (uint)Math.Round(txbxFormat.InternalMargin.Top * DLSConstants.EmusPerPoint);
                SetShapeOption(shapeProps, margin, (int)FOPTEText.dyTextTop, false);
            }
            if (txbxFormat.InternalMargin.Bottom != InternalMargin.DEF_VERTMARGIN)
            {
                margin = (uint)Math.Round(txbxFormat.InternalMargin.Bottom * DLSConstants.EmusPerPoint);
                SetShapeOption(shapeProps, margin, (int)FOPTEText.dyTextBottom, false);
            }

            if (txbxFormat.Document != null)
            {
                UpdateBackground(txbxFormat.Document, txbxFormat.FillEfects);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pictProps"></param>
        internal void WritePictureOptions(PictureShapeProps pictProps, WPicture pic)
        {
            msofbtRGFOPTE shapeProps = this.ShapeOptions.Properties;
            if (pic.TextWrappingStyle != TextWrappingStyle .Inline)
            {
                this.ShapeOptions.DistanceFromBottom = (uint)(pic.DistanceFromBottom * DLSConstants.EmusPerPoint);
                this.ShapeOptions.DistanceFromLeft = (uint)(pic.DistanceFromLeft * DLSConstants.EmusPerPoint);
                this.ShapeOptions.DistanceFromRight = (uint)(pic.DistanceFromRight * DLSConstants.EmusPerPoint);
                this.ShapeOptions.DistanceFromTop = (uint)(pic.DistanceFromTop * DLSConstants.EmusPerPoint);
            }
            //read picture wrap polygon vertices.
            if (pic.TextWrappingStyle == TextWrappingStyle.Through || pic.TextWrappingStyle == TextWrappingStyle.Tight)
            {
                PointF[] data = pic.WrapPolygon.Vertices.ToArray();
                byte[] bytearray = new byte[6 + (data.Length * 8)];
                byte[] nElems = BitConverter.GetBytes((Int16)(data.Length));
                byte[] nElemsAlloc = nElems;
                byte[] cbElem = BitConverter.GetBytes((Int16)8);
                int offset = 0;
                Array.Copy(nElems, 0, bytearray, offset, 2);
                offset += 2;
                Array.Copy(nElemsAlloc, 0, bytearray, offset, 2);
                offset += 2;
                Array.Copy(cbElem, 0, bytearray, offset, 2);
                offset += 2;
                for (int i = 0; i < data.Length; i++)
                {
                    byte[] datax = BitConverter.GetBytes((Int32)data[i].X);
                    Array.Copy(datax, 0, bytearray, offset, datax.Length);
                    offset += datax.Length;
                    byte[] datay = BitConverter.GetBytes((Int32)data[i].Y);
                    Array.Copy(datay, 0, bytearray, offset, datay.Length);
                    offset += datay.Length;
                }

                if (shapeProps.ContainsKey((int)FOPTEGroupShape.pWrapPolygonVertices))
                    shapeProps.Remove((int)FOPTEGroupShape.pWrapPolygonVertices);

                FOPTEComplex wrapPolygon = new FOPTEComplex((int)FOPTEGroupShape.pWrapPolygonVertices, false, bytearray.Length);
                wrapPolygon.Value = bytearray;
                shapeProps.Add(wrapPolygon);
            }
            if (!shapeProps.ContainsKey(MsofbtOPT.DEF_PIB_ID))
            {
                SetShapeOption(shapeProps, 1, MsofbtOPT.DEF_PIB_ID, true);
            }
            if (!shapeProps.ContainsKey(MsofbtOPT.DEF_PIBFLAGS_ID))
            {
                SetShapeOption(shapeProps, 2, MsofbtOPT.DEF_PIBFLAGS_ID, false);
            }

            // Add picture alternative text
            if (!string.IsNullOrEmpty(pictProps.AlternativeText))
            {
                if (shapeProps.ContainsKey((int)FOPTEGroupShape.wzDescription))
                    shapeProps.Remove((int)FOPTEGroupShape.wzDescription);

                byte[] textData = Encoding.Unicode.GetBytes(pictProps.AlternativeText + "\0");
                FOPTEComplex textValue = new FOPTEComplex((int)FOPTEGroupShape.wzDescription, false, textData.Length);
                textValue.Value = textData;
                shapeProps.Add(textValue);
            }

            if (pictProps.IsBelowText)
            {
                SetBoolShapeOption(shapeProps, (int)FOPTEGroupShape.fPrint, 0x20, 5,
                  1, msofbtRGFOPTE.DEF_BEHIND_DOC);
            }
            //Set picture brightness
            //      if( pictProps.PictureBrightness != 50 )
            //      {
            //        uint value = 0;
            //        if( pictProps.PictureBrightness < 50 )
            //        {
            //          value = msofbtOptProperties.DEF_BRIGHTNESS_BORDER + 
            //            ( uint )pictProps.PictureBrightness * msofbtOptProperties.DEF_BRIGHTNESS_STEP;
            //        }
            //        else
            //        {
            //          value = ( uint )( pictProps.PictureBrightness - 50 ) * msofbtOptProperties.DEF_BRIGHTNESS_STEP;          
            //        }
            //        SetShapeOption( shapeProps, value, ( int )FOPTEBlip.pictureBrightness, false );
            //      }

            //Set picture contrast
            //      if( pictProps.PictureContrast != 50 )
            //      {
            //        uint value = 0;
            //        if( pictProps.PictureContrast < 50 )
            //        {
            //          value =  msofbtOptProperties.DEF_CONTRAST_STEP * ( uint )pictProps.PictureContrast;
            //        }
            //        SetShapeOption( shapeProps, value, ( int )FOPTEBlip.pictureContrast, false );
            //      }

            //Set picture Color
            //      if( pictProps.PictureColor != PictureColor.Automatic )
            //      {
            //        if( pictProps.PictureColor == PictureColor.Grayscale )
            //        {
            //          SetBoolShapeOption( shapeProps, ( int )FOPTEBlip.pictureActive, 0x04, 2, 1, 
            //            msofbtOptProperties.DEF_GRAYSCALE_COLOR );
            //        }
            //        if( pictProps.PictureColor == PictureColor.BlackAndWhite )
            //        {
            //          SetBoolShapeOption( shapeProps, ( int )FOPTEBlip.pictureActive, 0x02, 1, 1, 
            //            msofbtOptProperties.DEF_BLACKWHITE_COLOR );
            //          SetBoolShapeOption( shapeProps, ( int )FOPTEBlip.pictureActive, 0x04, 2, 1, 
            //            msofbtOptProperties.DEF_BLACKWHITE_COLOR );
            //        }
            //      }
            //Set picture crop
            //      float prop = 0;
            //      if( pictProps.CropFromLeft != 0 )
            //      {
            //        prop = pictProps.CropFromLeft * msofbtOptProperties.DEF_CROP_STEP;
            //        SetShapeOption( shapeProps, prop,( int )FOPTEBlip.cropFromLeft, false );
            //      }
            //
            //      if( pictProps.CropFromRight != 0 )
            //      {
            //        prop = pictProps.CropFromRight * msofbtOptProperties.DEF_CROP_STEP;
            //        SetShapeOption( shapeProps, prop,( int )FOPTEBlip.cropFromRight, false );
            //      }
            //
            //      if( pictProps.CropFromTop != 0 )
            //      {
            //        prop = pictProps.CropFromTop * msofbtOptProperties.DEF_CROP_STEP;
            //        SetShapeOption( shapeProps, prop,( int )FOPTEBlip.cropFromTop, false );
            //      }
            //
            //      if( pictProps.CropFromBottom != 0 )
            //      {
            //        prop = pictProps.CropFromBottom * msofbtOptProperties.DEF_CROP_STEP;
            //        SetShapeOption( shapeProps, prop,( int )FOPTEBlip.cropFromBottom, false );
            //      }
        }
        /// <summary>
        /// Set user defined textbox option.
        /// </summary>
        /// <param name="shapeProps">List of container's FOPTEs</param>
        /// <param name="curValue">User defined option value</param>
        /// <param name="fopteKey">Option's FOPTE key</param>
        /// <param name="isBid"></param>
        private void SetShapeOption(msofbtRGFOPTE shapeProps, uint curValue, int fopteKey, bool isBid)
        {         
            if (shapeProps.ContainsKey(fopteKey))
            {
                FOPTEBid fopteBid = (FOPTEBid)shapeProps[fopteKey];
                if (fopteBid.Value != (uint)curValue)
                {
                    fopteBid.Value = (uint)curValue;
                }
            }
            else shapeProps.Add(new FOPTEBid(fopteKey, isBid, (uint)curValue));
        }
        /// <summary>
        /// Set user defined boolean textbox property.
        /// </summary>
        /// <param name="shapeProps">Shape options</param>
        /// <param name="fopteKey">Option's FOPTE key</param>
        /// <param name="bitMask">Bit mask for option</param>
        /// <param name="startBit">Option's bit in bitfield</param>
        /// <param name="value">Options value</param>
        /// <param name="defValue">FopteBid's default value</param>
        private void SetBoolShapeOption(msofbtRGFOPTE shapeProps, int fopteKey,
          int bitMask, int startBit, int value, uint defValue)
        {
            if (shapeProps.ContainsKey(fopteKey))
            {
                FOPTEBid fopteBid = (FOPTEBid)shapeProps[fopteKey];
                int bitValue = (int)BaseWordRecord.GetBitsByMask(fopteBid.Value, bitMask, startBit);
                if (bitValue != value)
                {
                    fopteBid.Value = (uint)BaseWordRecord.SetBitsByMask((int)fopteBid.Value, bitMask, startBit, value);
                }
            }
            else shapeProps.Add(new FOPTEBid(fopteKey, false, defValue));
        }

        /// <summary>
        /// Gets the opacity.
        /// </summary>
        /// <param name="opacity">The opacity.</param>
        /// <returns></returns>
        private uint GetOpacity(byte opacity)
        {
            float alphaPercents = (float)(100 - (float)opacity / 2.55);
            if (alphaPercents != 0)
            {
                if (alphaPercents != 0)
                {
                    return (uint)Math.Round((float)(100 - alphaPercents) * 655.35);
                }
                return 0;
            }

            return uint.MaxValue;
        }
        #endregion

        #region Background helper methods
        /// <summary>
        /// Noes the background.
        /// </summary>
        /// <returns></returns>
        private void CheckEscher(WordDocument doc)
        {
            if (doc == null)
                return;

            EscherClass escher = doc.Escher;
            if ((escher != null && escher.m_dgContainers.Count == 0) || escher == null)
            {
                escher = new EscherClass(doc);
                escher.CreateDgForSubDocuments();
                doc.Escher = escher;
            }
            else if (escher.m_msofbtDggContainer.BstoreContainer == null)
            {
                escher.m_msofbtDggContainer.Children.Add(new MsofbtBstoreContainer(m_doc));
            }
        }
        /// <summary>
        /// Get shading variant for "FromCorner" shading style.
        /// </summary>
        /// <returns></returns>
        private GradientShadingVariant GetCornerStyleVariant()
        {
            uint fillToLeft = GetPropertyValue((int)FOPTEFillStyle.fillToLeft);
            uint fillToBottom = GetPropertyValue((int)FOPTEFillStyle.fillToBottom);
            if (fillToLeft == uint.MaxValue && fillToBottom == uint.MaxValue)
            {
                return GradientShadingVariant.ShadingUp;
            }
            else if (fillToLeft != uint.MaxValue && fillToBottom != uint.MaxValue)
            {
                return GradientShadingVariant.ShadingMiddle;
            }
            else if (fillToLeft != uint.MaxValue)
            {
                return GradientShadingVariant.ShadingDown;
            }
            else
            {
                return GradientShadingVariant.ShadingOut;
            }
        }
        /// <summary>
        /// Add fillAngle fopte to gradient container.
        /// </summary>
        /// <param name="shadingStyle"></param>
        private void AddGradientFillAngle(GradientShadingStyle shadingStyle)
        {
            if (shadingStyle != GradientShadingStyle.Horizontal)
            {
                uint angle = 0;
                switch (shadingStyle)
                {
                    case GradientShadingStyle.Vertical:
                        angle = BackgroundGradient.DEF_VERTICAL_ANGLE;
                        break;
                    case GradientShadingStyle.DiagonalUp:
                        angle = BackgroundGradient.DEF_DIAGONALUP_ANGLE;
                        break;
                    default:
                        angle = BackgroundGradient.DEF_DIAGONALDOWN_ANGLE;
                        break;
                }

                SetShapeOption(ShapeOptions.Properties, angle, (int)FOPTEFillStyle.fillAngle, false);
            }
        }
        /// <summary>
        /// Adds focus fopte to gradient container.
        /// </summary>
        /// <param name="shadingStyle"></param>
        /// <param name="shadingVariant"></param>
        /// <returns></returns>
        private void AddGradientFocusFopte(GradientShadingStyle shadingStyle, GradientShadingVariant shadingVariant)
        {
            uint focus = 0;
            if (shadingStyle != GradientShadingStyle.FromCorner)
            {
                switch (shadingVariant)
                {
                    case GradientShadingVariant.ShadingUp:
                        focus = BackgroundGradient.DEF_SHADEUP_VARIANT;
                        break;
                    case GradientShadingVariant.ShadingOut:
                        focus = BackgroundGradient.DEF_SHADEOUT_VARIANT;
                        break;
                    case GradientShadingVariant.ShadingMiddle:
                        focus = BackgroundGradient.DEF_SHADEMIDDLE_VARIANT;
                        break;
                }
            }
            else
            {
                focus = BackgroundGradient.DEF_SHADEUP_VARIANT;
            }

            if (focus != 0)
            {
                SetShapeOption(ShapeOptions.Properties, focus, (int)FOPTEFillStyle.fillFocus, false);
            }
        }
        /// <summary>
        /// Add fillType fopte to gradient container.
        /// </summary>
        /// <param name="shadingStyle"></param>
        private void AddGradientFillType(GradientShadingStyle shadingStyle)
        {
            uint shadeStyle = 0;
            switch (shadingStyle)
            {
                case GradientShadingStyle.FromCorner:
                    shadeStyle = (uint)BackgroundFillType.msofillShadeCenter;
                    break;
                case GradientShadingStyle.FromCenter:
                    shadeStyle = (uint)BackgroundFillType.msofillShadeShape;
                    break;
                default:
                    shadeStyle = (uint)BackgroundFillType.msofillShadeScale;
                    break;
            }

            SetShapeOption(ShapeOptions.Properties, shadeStyle, (int)FOPTEFillStyle.fillType, false);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="shadingStyle"></param>
        /// <param name="shadingVariant"></param>
        private void AddFillProperties(GradientShadingStyle shadingStyle, GradientShadingVariant shadingVariant)
        {
            uint fillToLeft = 0;
            uint fillToRight = 0;
            uint fillToTop = 0;
            uint fillToBottom = 0;
            switch (shadingStyle)
            {
                case GradientShadingStyle.FromCorner:
                    if (shadingVariant == GradientShadingVariant.ShadingDown)
                    {
                        fillToLeft = fillToRight = 65536;
                    }
                    if (shadingVariant == GradientShadingVariant.ShadingOut)
                    {
                        fillToTop = fillToBottom = 65536;
                    }
                    if (shadingVariant == GradientShadingVariant.ShadingMiddle)
                    {
                        fillToTop = fillToBottom = fillToLeft = fillToRight = 65536;
                    }
                    break;
                default:
                    fillToTop = fillToBottom = fillToLeft = fillToRight = 32768;
                    break;
            }

            if (fillToLeft != 0)
            {
                SetShapeOption(ShapeOptions.Properties, fillToLeft, (int)FOPTEFillStyle.fillToLeft, false);
            }
            if (fillToRight != 0)
            {
                SetShapeOption(ShapeOptions.Properties, fillToRight, (int)FOPTEFillStyle.fillToRight, false);
            }
            if (fillToTop != 0)
            {
                SetShapeOption(ShapeOptions.Properties, fillToTop, (int)FOPTEFillStyle.fillToTop, false);
            }
            if (fillToBottom != 0)
            {
                SetShapeOption(ShapeOptions.Properties, fillToBottom, (int)FOPTEFillStyle.fillToBottom, false);
            }
        }
        #endregion
    }
}
