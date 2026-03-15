#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Forms.Edit.Utils.Serializers {

    /// <summary>
    /// Format Serialization Writer
    /// </summary>
    public class FormatSerializationWriter : System.Xml.Serialization.XmlSerializationWriter, Syncfusion.XmlSerializersCreator.IXmlSerializationWriter {

        void Write1_Format(string n, string ns, Syncfusion.Windows.Forms.Edit.Implementation.Formatting.Format o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (!(t == typeof(Syncfusion.Windows.Forms.Edit.Implementation.Formatting.Format)))                    
                    throw CreateUnknownTypeException(o);
                
            }
            WriteStartElement(n, ns, o);
            if (needType) WriteXsiType(@"Format", @"");
            if ((System.Boolean)o.@UseCustomControl != false) {
                WriteAttribute(@"UseCustomControl", @"", System.Xml.XmlConvert.ToString((System.Boolean)(System.Boolean)o.@UseCustomControl));
            }
            WriteAttribute(@"name", @"", (System.String)o.@Name);
            WriteAttribute(@"Font", @"", (System.String)o.@XmlFont);
            if (o.ShouldSerializeXmlForeColor()) {
                WriteAttribute(@"ForeColor", @"", (System.String)o.@XmlForeColor);
            }
            if ((System.String)o.@XmlFontColor != @"Empty") {
                WriteAttribute(@"FontColor", @"", (System.String)o.@XmlFontColor);
            }
            if ((System.String)o.@XmlBackColor != @"") {
                WriteAttribute(@"BackColor", @"", (System.String)o.@XmlBackColor);
            }
            if ((System.Drawing.Drawing2D.HatchStyle)o.@HatchStyle != System.Drawing.Drawing2D.HatchStyle.@Horizontal) {
                WriteAttribute(@"style", @"", Write2_HatchStyle((System.Drawing.Drawing2D.HatchStyle)o.@HatchStyle));
            }
            if ((Syncfusion.Windows.Forms.Edit.Enums.UnderlineWeight)o.@UnderlineWeight != Syncfusion.Windows.Forms.Edit.Enums.UnderlineWeight.@Thin) {
                WriteAttribute(@"weight", @"", Write3_UnderlineWeight((Syncfusion.Windows.Forms.Edit.Enums.UnderlineWeight)o.@UnderlineWeight));
            }
            if ((Syncfusion.Windows.Forms.Edit.Enums.UnderlineStyle)o.@UnderlineStyle != Syncfusion.Windows.Forms.Edit.Enums.UnderlineStyle.@None) {
                WriteAttribute(@"underline", @"", Write4_UnderlineStyle((Syncfusion.Windows.Forms.Edit.Enums.UnderlineStyle)o.@UnderlineStyle));
            }
            if ((System.Boolean)o.@UseHatchFill != false) {
                WriteAttribute(@"UseHatchFill", @"", System.Xml.XmlConvert.ToString((System.Boolean)(System.Boolean)o.@UseHatchFill));
            }
            if ((System.String)o.@XmlLineColor != @"Black") {
                WriteAttribute(@"LineColor", @"", (System.String)o.@XmlLineColor);
            }
            if ((System.String)o.@XmlStrikeOutColor != @"") {
                WriteAttribute(@"StrikeOutColor", @"", (System.String)o.@XmlStrikeOutColor);
            }
            if ((System.String)o.@XmlBorderColor != @"") {
                WriteAttribute(@"BorderColor", @"", (System.String)o.@XmlBorderColor);
            }
            if ((Syncfusion.Windows.Forms.Edit.Enums.BorderWeight)o.@BorderWeight != (Syncfusion.Windows.Forms.Edit.Enums.BorderWeight.@Thin)) {
                WriteAttribute(@"BorderWeight", @"", Write5_BorderWeight((Syncfusion.Windows.Forms.Edit.Enums.BorderWeight)o.@BorderWeight));
            }
            if ((Syncfusion.Windows.Forms.Edit.Enums.FrameBorderStyle)o.@BorderStyle != Syncfusion.Windows.Forms.Edit.Enums.FrameBorderStyle.@None) {
                WriteAttribute(@"BorderStyle", @"", Write6_FrameBorderStyle((Syncfusion.Windows.Forms.Edit.Enums.FrameBorderStyle)o.@BorderStyle));
            }
            WriteEndElement(o);
        }

        string Write2_HatchStyle(System.Drawing.Drawing2D.HatchStyle v) {
            string s = null;
            switch (v) {
                case System.Drawing.Drawing2D.HatchStyle.@Horizontal: s = @"Horizontal"; break;
                case System.Drawing.Drawing2D.HatchStyle.@Vertical: s = @"Vertical"; break;
                case System.Drawing.Drawing2D.HatchStyle.@ForwardDiagonal: s = @"ForwardDiagonal"; break;
                case System.Drawing.Drawing2D.HatchStyle.@BackwardDiagonal: s = @"BackwardDiagonal"; break;
                case System.Drawing.Drawing2D.HatchStyle.@Cross: s = @"Cross"; break;
                case System.Drawing.Drawing2D.HatchStyle.@DiagonalCross: s = @"DiagonalCross"; break;
                case System.Drawing.Drawing2D.HatchStyle.@Percent05: s = @"Percent05"; break;
                case System.Drawing.Drawing2D.HatchStyle.@Percent10: s = @"Percent10"; break;
                case System.Drawing.Drawing2D.HatchStyle.@Percent20: s = @"Percent20"; break;
                case System.Drawing.Drawing2D.HatchStyle.@Percent25: s = @"Percent25"; break;
                case System.Drawing.Drawing2D.HatchStyle.@Percent30: s = @"Percent30"; break;
                case System.Drawing.Drawing2D.HatchStyle.@Percent40: s = @"Percent40"; break;
                case System.Drawing.Drawing2D.HatchStyle.@Percent50: s = @"Percent50"; break;
                case System.Drawing.Drawing2D.HatchStyle.@Percent60: s = @"Percent60"; break;
                case System.Drawing.Drawing2D.HatchStyle.@Percent70: s = @"Percent70"; break;
                case System.Drawing.Drawing2D.HatchStyle.@Percent75: s = @"Percent75"; break;
                case System.Drawing.Drawing2D.HatchStyle.@Percent80: s = @"Percent80"; break;
                case System.Drawing.Drawing2D.HatchStyle.@Percent90: s = @"Percent90"; break;
                case System.Drawing.Drawing2D.HatchStyle.@LightDownwardDiagonal: s = @"LightDownwardDiagonal"; break;
                case System.Drawing.Drawing2D.HatchStyle.@LightUpwardDiagonal: s = @"LightUpwardDiagonal"; break;
                case System.Drawing.Drawing2D.HatchStyle.@DarkDownwardDiagonal: s = @"DarkDownwardDiagonal"; break;
                case System.Drawing.Drawing2D.HatchStyle.@DarkUpwardDiagonal: s = @"DarkUpwardDiagonal"; break;
                case System.Drawing.Drawing2D.HatchStyle.@WideDownwardDiagonal: s = @"WideDownwardDiagonal"; break;
                case System.Drawing.Drawing2D.HatchStyle.@WideUpwardDiagonal: s = @"WideUpwardDiagonal"; break;
                case System.Drawing.Drawing2D.HatchStyle.@LightVertical: s = @"LightVertical"; break;
                case System.Drawing.Drawing2D.HatchStyle.@LightHorizontal: s = @"LightHorizontal"; break;
                case System.Drawing.Drawing2D.HatchStyle.@NarrowVertical: s = @"NarrowVertical"; break;
                case System.Drawing.Drawing2D.HatchStyle.@NarrowHorizontal: s = @"NarrowHorizontal"; break;
                case System.Drawing.Drawing2D.HatchStyle.@DarkVertical: s = @"DarkVertical"; break;
                case System.Drawing.Drawing2D.HatchStyle.@DarkHorizontal: s = @"DarkHorizontal"; break;
                case System.Drawing.Drawing2D.HatchStyle.@DashedDownwardDiagonal: s = @"DashedDownwardDiagonal"; break;
                case System.Drawing.Drawing2D.HatchStyle.@DashedUpwardDiagonal: s = @"DashedUpwardDiagonal"; break;
                case System.Drawing.Drawing2D.HatchStyle.@DashedHorizontal: s = @"DashedHorizontal"; break;
                case System.Drawing.Drawing2D.HatchStyle.@DashedVertical: s = @"DashedVertical"; break;
                case System.Drawing.Drawing2D.HatchStyle.@SmallConfetti: s = @"SmallConfetti"; break;
                case System.Drawing.Drawing2D.HatchStyle.@LargeConfetti: s = @"LargeConfetti"; break;
                case System.Drawing.Drawing2D.HatchStyle.@ZigZag: s = @"ZigZag"; break;
                case System.Drawing.Drawing2D.HatchStyle.@Wave: s = @"Wave"; break;
                case System.Drawing.Drawing2D.HatchStyle.@DiagonalBrick: s = @"DiagonalBrick"; break;
                case System.Drawing.Drawing2D.HatchStyle.@HorizontalBrick: s = @"HorizontalBrick"; break;
                case System.Drawing.Drawing2D.HatchStyle.@Weave: s = @"Weave"; break;
                case System.Drawing.Drawing2D.HatchStyle.@Plaid: s = @"Plaid"; break;
                case System.Drawing.Drawing2D.HatchStyle.@Divot: s = @"Divot"; break;
                case System.Drawing.Drawing2D.HatchStyle.@DottedGrid: s = @"DottedGrid"; break;
                case System.Drawing.Drawing2D.HatchStyle.@DottedDiamond: s = @"DottedDiamond"; break;
                case System.Drawing.Drawing2D.HatchStyle.@Shingle: s = @"Shingle"; break;
                case System.Drawing.Drawing2D.HatchStyle.@Trellis: s = @"Trellis"; break;
                case System.Drawing.Drawing2D.HatchStyle.@Sphere: s = @"Sphere"; break;
                case System.Drawing.Drawing2D.HatchStyle.@SmallGrid: s = @"SmallGrid"; break;
                case System.Drawing.Drawing2D.HatchStyle.@SmallCheckerBoard: s = @"SmallCheckerBoard"; break;
                case System.Drawing.Drawing2D.HatchStyle.@LargeCheckerBoard: s = @"LargeCheckerBoard"; break;
                case System.Drawing.Drawing2D.HatchStyle.@OutlinedDiamond: s = @"OutlinedDiamond"; break;
                case System.Drawing.Drawing2D.HatchStyle.@SolidDiamond: s = @"SolidDiamond"; break;
                default: s = ((System.Int64)v).ToString(); break;
            }
            return s;
        }

        string Write3_UnderlineWeight(Syncfusion.Windows.Forms.Edit.Enums.UnderlineWeight v) {
            string s = null;
            switch (v) {
                case Syncfusion.Windows.Forms.Edit.Enums.UnderlineWeight.@Thin: s = @"Thin"; break;
                case Syncfusion.Windows.Forms.Edit.Enums.UnderlineWeight.@Bold: s = @"Bold"; break;
                case Syncfusion.Windows.Forms.Edit.Enums.UnderlineWeight.@Double: s = @"Double"; break;
                case Syncfusion.Windows.Forms.Edit.Enums.UnderlineWeight.@DoubleBold: s = @"DoubleBold"; break;
                default: s = ((System.Int64)v).ToString(); break;
            }
            return s;
        }

        string Write4_UnderlineStyle(Syncfusion.Windows.Forms.Edit.Enums.UnderlineStyle v) {
            string s = null;
            switch (v) {
                case Syncfusion.Windows.Forms.Edit.Enums.UnderlineStyle.@None: s = @"None"; break;
                case Syncfusion.Windows.Forms.Edit.Enums.UnderlineStyle.@Solid: s = @"Solid"; break;
                case Syncfusion.Windows.Forms.Edit.Enums.UnderlineStyle.@DashDot: s = @"DashDot"; break;
                case Syncfusion.Windows.Forms.Edit.Enums.UnderlineStyle.@Dot: s = @"Dot"; break;
                case Syncfusion.Windows.Forms.Edit.Enums.UnderlineStyle.@Dash: s = @"Dash"; break;
                case Syncfusion.Windows.Forms.Edit.Enums.UnderlineStyle.@Wave: s = @"Wave"; break;
                default: s = ((System.Int64)v).ToString(); break;
            }
            return s;
        }

        string Write5_BorderWeight(Syncfusion.Windows.Forms.Edit.Enums.BorderWeight v) {
            string s = null;
            switch (v) {
                case Syncfusion.Windows.Forms.Edit.Enums.BorderWeight.@Thin: s = @"Thin"; break;
                case Syncfusion.Windows.Forms.Edit.Enums.BorderWeight.@Bold: s = @"Bold"; break;
                case Syncfusion.Windows.Forms.Edit.Enums.BorderWeight.@Double: s = @"Double"; break;
                default: s = FromEnum((System.Int64)v, new System.String[] {@"Thin", 
                    @"Bold", 
                    @"Double"}, new System.Int64[] {(System.Int64)Syncfusion.Windows.Forms.Edit.Enums.BorderWeight.@Thin, 
                    (System.Int64)Syncfusion.Windows.Forms.Edit.Enums.BorderWeight.@Bold, 
                    (System.Int64)Syncfusion.Windows.Forms.Edit.Enums.BorderWeight.@Double}); break;
            }
            return s;
        }

        string Write6_FrameBorderStyle(Syncfusion.Windows.Forms.Edit.Enums.FrameBorderStyle v) {
            string s = null;
            switch (v) {
                case Syncfusion.Windows.Forms.Edit.Enums.FrameBorderStyle.@None: s = @"None"; break;
                case Syncfusion.Windows.Forms.Edit.Enums.FrameBorderStyle.@Solid: s = @"Solid"; break;
                case Syncfusion.Windows.Forms.Edit.Enums.FrameBorderStyle.@DashDot: s = @"DashDot"; break;
                case Syncfusion.Windows.Forms.Edit.Enums.FrameBorderStyle.@Dot: s = @"Dot"; break;
                case Syncfusion.Windows.Forms.Edit.Enums.FrameBorderStyle.@Dash: s = @"Dash"; break;
                case Syncfusion.Windows.Forms.Edit.Enums.FrameBorderStyle.@Wave: s = @"Wave"; break;
                default: s = ((System.Int64)v).ToString(); break;
            }
            return s;
        }

        void Write7_Object(string n, string ns, System.Object o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (!(t == typeof(System.Object)))
                {
                    if (t == typeof(Syncfusion.Windows.Forms.Edit.Implementation.Formatting.Format))
                    {
                        Write1_Format(n, ns, (Syncfusion.Windows.Forms.Edit.Implementation.Formatting.Format)o, isNullable, true);
                        return;
                    }
                    else if (t == typeof(System.Drawing.Drawing2D.HatchStyle))
                    {
                        Writer.WriteStartElement(n, ns);
                        WriteXsiType(@"HatchStyle", @"");
                        Writer.WriteString(Write2_HatchStyle((System.Drawing.Drawing2D.HatchStyle)o));
                        Writer.WriteEndElement();
                        return;
                    }
                    else if (t == typeof(Syncfusion.Windows.Forms.Edit.Enums.UnderlineWeight))
                    {
                        Writer.WriteStartElement(n, ns);
                        WriteXsiType(@"UnderlineWeight", @"");
                        Writer.WriteString(Write3_UnderlineWeight((Syncfusion.Windows.Forms.Edit.Enums.UnderlineWeight)o));
                        Writer.WriteEndElement();
                        return;
                    }
                    else if (t == typeof(Syncfusion.Windows.Forms.Edit.Enums.UnderlineStyle))
                    {
                        Writer.WriteStartElement(n, ns);
                        WriteXsiType(@"UnderlineStyle", @"");
                        Writer.WriteString(Write4_UnderlineStyle((Syncfusion.Windows.Forms.Edit.Enums.UnderlineStyle)o));
                        Writer.WriteEndElement();
                        return;
                    }
                    else if (t == typeof(Syncfusion.Windows.Forms.Edit.Enums.BorderWeight))
                    {
                        Writer.WriteStartElement(n, ns);
                        WriteXsiType(@"BorderWeight", @"");
                        Writer.WriteString(Write5_BorderWeight((Syncfusion.Windows.Forms.Edit.Enums.BorderWeight)o));
                        Writer.WriteEndElement();
                        return;
                    }
                    else if (t == typeof(Syncfusion.Windows.Forms.Edit.Enums.FrameBorderStyle))
                    {
                        Writer.WriteStartElement(n, ns);
                        WriteXsiType(@"FrameBorderStyle", @"");
                        Writer.WriteString(Write6_FrameBorderStyle((Syncfusion.Windows.Forms.Edit.Enums.FrameBorderStyle)o));
                        Writer.WriteEndElement();
                        return;
                    }
                    else
                    {
                        WriteTypedPrimitive(n, ns, o, true);
                        return;
                    }
                }
            }
            WriteStartElement(n, ns, o);
            WriteEndElement(o);
        }
        /// <summary>
        /// Overrides the InitCallbacks
        /// </summary>
        protected override void InitCallbacks() {
        }

        /// <summary>
        /// Serializes given object.
        /// </summary>
        /// <param name="o">The object to serialize</param>
        public void WriteDataToXml(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteEmptyTag(@"format", @"");
                return;
            }
            TopLevelElement();
            Write1_Format(@"format", @"", ((Syncfusion.Windows.Forms.Edit.Implementation.Formatting.Format)o), false, false);
        }
    }

    /// <summary>
    /// Format Serialization reader.
    /// </summary>
    public class FormatSerializationReader : System.Xml.Serialization.XmlSerializationReader, Syncfusion.XmlSerializersCreator.IXmlSerializationReader {

        Syncfusion.Windows.Forms.Edit.Implementation.Formatting.Format Read1_Format(bool isNullable, bool checkType) {
            if (isNullable && ReadNull()) return null;
            if (checkType) {
                System.Xml.XmlQualifiedName t = GetXsiType();
                if (!(t == null || ((object) ((System.Xml.XmlQualifiedName)t).Name == (object)id1_Format && (object) ((System.Xml.XmlQualifiedName)t).Namespace == (object)id2_Item)))                    
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)t);
            }
            Syncfusion.Windows.Forms.Edit.Implementation.Formatting.Format o = new Syncfusion.Windows.Forms.Edit.Implementation.Formatting.Format();
            bool[] paramsRead = new bool[15];
            while (Reader.MoveToNextAttribute()) {
                if (!paramsRead[0] && ((object) Reader.LocalName == (object)id3_UseCustomControl && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o.@UseCustomControl = System.Xml.XmlConvert.ToBoolean(Reader.Value);
                    paramsRead[0] = true;
                }
                else if (!paramsRead[1] && ((object) Reader.LocalName == (object)id4_name && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o.@Name = Reader.Value;
                    paramsRead[1] = true;
                }
                else if (!paramsRead[2] && ((object) Reader.LocalName == (object)id5_Font && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o.@XmlFont = Reader.Value;
                    paramsRead[2] = true;
                }
                else if (!paramsRead[3] && ((object) Reader.LocalName == (object)id6_ForeColor && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o.@XmlForeColor = Reader.Value;
                    paramsRead[3] = true;
                }
                else if (!paramsRead[4] && ((object) Reader.LocalName == (object)id7_FontColor && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o.@XmlFontColor = Reader.Value;
                    paramsRead[4] = true;
                }
                else if (!paramsRead[5] && ((object) Reader.LocalName == (object)id8_BackColor && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o.@XmlBackColor = Reader.Value;
                    paramsRead[5] = true;
                }
                else if (!paramsRead[6] && ((object) Reader.LocalName == (object)id9_style && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o.@HatchStyle = Read2_HatchStyle(Reader.Value);
                    paramsRead[6] = true;
                }
                else if (!paramsRead[7] && ((object) Reader.LocalName == (object)id10_weight && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o.@UnderlineWeight = Read3_UnderlineWeight(Reader.Value);
                    paramsRead[7] = true;
                }
                else if (!paramsRead[8] && ((object) Reader.LocalName == (object)id11_underline && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o.@UnderlineStyle = Read4_UnderlineStyle(Reader.Value);
                    paramsRead[8] = true;
                }
                else if (!paramsRead[9] && ((object) Reader.LocalName == (object)id12_UseHatchFill && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o.@UseHatchFill = System.Xml.XmlConvert.ToBoolean(Reader.Value);
                    paramsRead[9] = true;
                }
                else if (!paramsRead[10] && ((object) Reader.LocalName == (object)id13_LineColor && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o.@XmlLineColor = Reader.Value;
                    paramsRead[10] = true;
                }
                else if (!paramsRead[11] && ((object) Reader.LocalName == (object)id14_StrikeOutColor && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o.@XmlStrikeOutColor = Reader.Value;
                    paramsRead[11] = true;
                }
                else if (!paramsRead[12] && ((object) Reader.LocalName == (object)id15_BorderColor && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o.@XmlBorderColor = Reader.Value;
                    paramsRead[12] = true;
                }
                else if (!paramsRead[13] && ((object) Reader.LocalName == (object)id16_BorderWeight && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o.@BorderWeight = Read5_BorderWeight(Reader.Value);
                    paramsRead[13] = true;
                }
                else if (!paramsRead[14] && ((object) Reader.LocalName == (object)id17_BorderStyle && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o.@BorderStyle = Read6_FrameBorderStyle(Reader.Value);
                    paramsRead[14] = true;
                }
                else if (!IsXmlnsAttribute(Reader.Name)) {
                    UnknownNode((object)o);
                }
            }
            Reader.MoveToElement();
            if (Reader.IsEmptyElement) {
                Reader.Skip();
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            while (Reader.NodeType != System.Xml.XmlNodeType.EndElement) {
                if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                    UnknownNode((object)o);
                }
                else {
                    UnknownNode((object)o);
                }
                Reader.MoveToContent();
            }
            ReadEndElement();
            return o;
        }

        System.Drawing.Drawing2D.HatchStyle Read2_HatchStyle(string s) {
            switch (s) {
                case @"Horizontal": return System.Drawing.Drawing2D.HatchStyle.@Horizontal;
                case @"Vertical": return System.Drawing.Drawing2D.HatchStyle.@Vertical;
                case @"ForwardDiagonal": return System.Drawing.Drawing2D.HatchStyle.@ForwardDiagonal;
                case @"BackwardDiagonal": return System.Drawing.Drawing2D.HatchStyle.@BackwardDiagonal;
                case @"Cross": return System.Drawing.Drawing2D.HatchStyle.@Cross;
                case @"DiagonalCross": return System.Drawing.Drawing2D.HatchStyle.@DiagonalCross;
                case @"Percent05": return System.Drawing.Drawing2D.HatchStyle.@Percent05;
                case @"Percent10": return System.Drawing.Drawing2D.HatchStyle.@Percent10;
                case @"Percent20": return System.Drawing.Drawing2D.HatchStyle.@Percent20;
                case @"Percent25": return System.Drawing.Drawing2D.HatchStyle.@Percent25;
                case @"Percent30": return System.Drawing.Drawing2D.HatchStyle.@Percent30;
                case @"Percent40": return System.Drawing.Drawing2D.HatchStyle.@Percent40;
                case @"Percent50": return System.Drawing.Drawing2D.HatchStyle.@Percent50;
                case @"Percent60": return System.Drawing.Drawing2D.HatchStyle.@Percent60;
                case @"Percent70": return System.Drawing.Drawing2D.HatchStyle.@Percent70;
                case @"Percent75": return System.Drawing.Drawing2D.HatchStyle.@Percent75;
                case @"Percent80": return System.Drawing.Drawing2D.HatchStyle.@Percent80;
                case @"Percent90": return System.Drawing.Drawing2D.HatchStyle.@Percent90;
                case @"LightDownwardDiagonal": return System.Drawing.Drawing2D.HatchStyle.@LightDownwardDiagonal;
                case @"LightUpwardDiagonal": return System.Drawing.Drawing2D.HatchStyle.@LightUpwardDiagonal;
                case @"DarkDownwardDiagonal": return System.Drawing.Drawing2D.HatchStyle.@DarkDownwardDiagonal;
                case @"DarkUpwardDiagonal": return System.Drawing.Drawing2D.HatchStyle.@DarkUpwardDiagonal;
                case @"WideDownwardDiagonal": return System.Drawing.Drawing2D.HatchStyle.@WideDownwardDiagonal;
                case @"WideUpwardDiagonal": return System.Drawing.Drawing2D.HatchStyle.@WideUpwardDiagonal;
                case @"LightVertical": return System.Drawing.Drawing2D.HatchStyle.@LightVertical;
                case @"LightHorizontal": return System.Drawing.Drawing2D.HatchStyle.@LightHorizontal;
                case @"NarrowVertical": return System.Drawing.Drawing2D.HatchStyle.@NarrowVertical;
                case @"NarrowHorizontal": return System.Drawing.Drawing2D.HatchStyle.@NarrowHorizontal;
                case @"DarkVertical": return System.Drawing.Drawing2D.HatchStyle.@DarkVertical;
                case @"DarkHorizontal": return System.Drawing.Drawing2D.HatchStyle.@DarkHorizontal;
                case @"DashedDownwardDiagonal": return System.Drawing.Drawing2D.HatchStyle.@DashedDownwardDiagonal;
                case @"DashedUpwardDiagonal": return System.Drawing.Drawing2D.HatchStyle.@DashedUpwardDiagonal;
                case @"DashedHorizontal": return System.Drawing.Drawing2D.HatchStyle.@DashedHorizontal;
                case @"DashedVertical": return System.Drawing.Drawing2D.HatchStyle.@DashedVertical;
                case @"SmallConfetti": return System.Drawing.Drawing2D.HatchStyle.@SmallConfetti;
                case @"LargeConfetti": return System.Drawing.Drawing2D.HatchStyle.@LargeConfetti;
                case @"ZigZag": return System.Drawing.Drawing2D.HatchStyle.@ZigZag;
                case @"Wave": return System.Drawing.Drawing2D.HatchStyle.@Wave;
                case @"DiagonalBrick": return System.Drawing.Drawing2D.HatchStyle.@DiagonalBrick;
                case @"HorizontalBrick": return System.Drawing.Drawing2D.HatchStyle.@HorizontalBrick;
                case @"Weave": return System.Drawing.Drawing2D.HatchStyle.@Weave;
                case @"Plaid": return System.Drawing.Drawing2D.HatchStyle.@Plaid;
                case @"Divot": return System.Drawing.Drawing2D.HatchStyle.@Divot;
                case @"DottedGrid": return System.Drawing.Drawing2D.HatchStyle.@DottedGrid;
                case @"DottedDiamond": return System.Drawing.Drawing2D.HatchStyle.@DottedDiamond;
                case @"Shingle": return System.Drawing.Drawing2D.HatchStyle.@Shingle;
                case @"Trellis": return System.Drawing.Drawing2D.HatchStyle.@Trellis;
                case @"Sphere": return System.Drawing.Drawing2D.HatchStyle.@Sphere;
                case @"SmallGrid": return System.Drawing.Drawing2D.HatchStyle.@SmallGrid;
                case @"SmallCheckerBoard": return System.Drawing.Drawing2D.HatchStyle.@SmallCheckerBoard;
                case @"LargeCheckerBoard": return System.Drawing.Drawing2D.HatchStyle.@LargeCheckerBoard;
                case @"OutlinedDiamond": return System.Drawing.Drawing2D.HatchStyle.@OutlinedDiamond;
                case @"SolidDiamond": return System.Drawing.Drawing2D.HatchStyle.@SolidDiamond;
                case @"LargeGrid": return System.Drawing.Drawing2D.HatchStyle.@LargeGrid;
                case @"Min": return System.Drawing.Drawing2D.HatchStyle.@Min;
                case @"Max": return System.Drawing.Drawing2D.HatchStyle.@Max;
                default: throw CreateUnknownConstantException(s, typeof(System.Drawing.Drawing2D.HatchStyle));
            }
        }

        Syncfusion.Windows.Forms.Edit.Enums.UnderlineWeight Read3_UnderlineWeight(string s) {
            switch (s) {
                case @"Thin": return Syncfusion.Windows.Forms.Edit.Enums.UnderlineWeight.@Thin;
                case @"Bold": return Syncfusion.Windows.Forms.Edit.Enums.UnderlineWeight.@Bold;
                case @"Double": return Syncfusion.Windows.Forms.Edit.Enums.UnderlineWeight.@Double;
                case @"DoubleBold": return Syncfusion.Windows.Forms.Edit.Enums.UnderlineWeight.@DoubleBold;
                default: throw CreateUnknownConstantException(s, typeof(Syncfusion.Windows.Forms.Edit.Enums.UnderlineWeight));
            }
        }

        Syncfusion.Windows.Forms.Edit.Enums.UnderlineStyle Read4_UnderlineStyle(string s) {
            switch (s) {
                case @"None": return Syncfusion.Windows.Forms.Edit.Enums.UnderlineStyle.@None;
                case @"Solid": return Syncfusion.Windows.Forms.Edit.Enums.UnderlineStyle.@Solid;
                case @"DashDot": return Syncfusion.Windows.Forms.Edit.Enums.UnderlineStyle.@DashDot;
                case @"Dot": return Syncfusion.Windows.Forms.Edit.Enums.UnderlineStyle.@Dot;
                case @"Dash": return Syncfusion.Windows.Forms.Edit.Enums.UnderlineStyle.@Dash;
                case @"Wave": return Syncfusion.Windows.Forms.Edit.Enums.UnderlineStyle.@Wave;
                default: throw CreateUnknownConstantException(s, typeof(Syncfusion.Windows.Forms.Edit.Enums.UnderlineStyle));
            }
        }

        [REDACTED] _BorderWeightValues;

        internal [REDACTED] BorderWeightValues {
            get {
                if ((object)_BorderWeightValues == null) {
                    [REDACTED] h = new [REDACTED]();
                    h.Add(@"Thin", (System.Int64)Syncfusion.Windows.Forms.Edit.Enums.BorderWeight.@Thin);
                    h.Add(@"Bold", (System.Int64)Syncfusion.Windows.Forms.Edit.Enums.BorderWeight.@Bold);
                    h.Add(@"Double", (System.Int64)Syncfusion.Windows.Forms.Edit.Enums.BorderWeight.@Double);
                    _BorderWeightValues = h;
                }
                return _BorderWeightValues;
            }
        }

        Syncfusion.Windows.Forms.Edit.Enums.BorderWeight Read5_BorderWeight(string s) {
            return (Syncfusion.Windows.Forms.Edit.Enums.BorderWeight)ToEnum(s, BorderWeightValues, @"Syncfusion.Windows.Forms.Edit.Enums.BorderWeight");
        }

        Syncfusion.Windows.Forms.Edit.Enums.FrameBorderStyle Read6_FrameBorderStyle(string s) {
            switch (s) {
                case @"None": return Syncfusion.Windows.Forms.Edit.Enums.FrameBorderStyle.@None;
                case @"Solid": return Syncfusion.Windows.Forms.Edit.Enums.FrameBorderStyle.@Solid;
                case @"DashDot": return Syncfusion.Windows.Forms.Edit.Enums.FrameBorderStyle.@DashDot;
                case @"Dot": return Syncfusion.Windows.Forms.Edit.Enums.FrameBorderStyle.@Dot;
                case @"Dash": return Syncfusion.Windows.Forms.Edit.Enums.FrameBorderStyle.@Dash;
                case @"Wave": return Syncfusion.Windows.Forms.Edit.Enums.FrameBorderStyle.@Wave;
                default: throw CreateUnknownConstantException(s, typeof(Syncfusion.Windows.Forms.Edit.Enums.FrameBorderStyle));
            }
        }

        System.Object Read7_Object(bool isNullable, bool checkType) {
            if (isNullable && ReadNull()) return null;
            if (checkType) {
                System.Xml.XmlQualifiedName t = GetXsiType();
                if (t == null)
                    return ReadTypedPrimitive(new System.Xml.XmlQualifiedName("anyType", "http://www.w3.org/2001/XMLSchema"));
                else if (((object) ((System.Xml.XmlQualifiedName)t).Name == (object)id1_Format && (object) ((System.Xml.XmlQualifiedName)t).Namespace == (object)id2_Item))
                    return Read1_Format(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)t).Name == (object)id18_HatchStyle && (object) ((System.Xml.XmlQualifiedName)t).Namespace == (object)id2_Item)) {
                    Reader.ReadStartElement();
                    object e = Read2_HatchStyle(Reader.ReadString());
                    ReadEndElement();
                    return e;
                }
                else if (((object) ((System.Xml.XmlQualifiedName)t).Name == (object)id19_UnderlineWeight && (object) ((System.Xml.XmlQualifiedName)t).Namespace == (object)id2_Item)) {
                    Reader.ReadStartElement();
                    object e = Read3_UnderlineWeight(Reader.ReadString());
                    ReadEndElement();
                    return e;
                }
                else if (((object) ((System.Xml.XmlQualifiedName)t).Name == (object)id20_UnderlineStyle && (object) ((System.Xml.XmlQualifiedName)t).Namespace == (object)id2_Item)) {
                    Reader.ReadStartElement();
                    object e = Read4_UnderlineStyle(Reader.ReadString());
                    ReadEndElement();
                    return e;
                }
                else if (((object) ((System.Xml.XmlQualifiedName)t).Name == (object)id16_BorderWeight && (object) ((System.Xml.XmlQualifiedName)t).Namespace == (object)id2_Item)) {
                    Reader.ReadStartElement();
                    object e = Read5_BorderWeight(Reader.ReadString());
                    ReadEndElement();
                    return e;
                }
                else if (((object) ((System.Xml.XmlQualifiedName)t).Name == (object)id21_FrameBorderStyle && (object) ((System.Xml.XmlQualifiedName)t).Namespace == (object)id2_Item)) {
                    Reader.ReadStartElement();
                    object e = Read6_FrameBorderStyle(Reader.ReadString());
                    ReadEndElement();
                    return e;
                }
                else
                    return ReadTypedPrimitive((System.Xml.XmlQualifiedName)t);
            }
            System.Object o = new System.Object();
            bool[] paramsRead = new bool[0];
            while (Reader.MoveToNextAttribute()) {
                if (!IsXmlnsAttribute(Reader.Name)) {
                    UnknownNode((object)o);
                }
            }
            Reader.MoveToElement();
            if (Reader.IsEmptyElement) {
                Reader.Skip();
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            while (Reader.NodeType != System.Xml.XmlNodeType.EndElement) {
                if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                    UnknownNode((object)o);
                }
                else {
                    UnknownNode((object)o);
                }
                Reader.MoveToContent();
            }
            ReadEndElement();
            return o;
        }
        /// <summary>
        /// Overrides the InitCallbacks
        /// </summary>
        protected override void InitCallbacks() {
        }

        /// <summary>
        /// Reads serialized data from Xml for Format
        /// </summary>
        /// <returns>returns object</returns>
        public object ReadDataFromXml() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id22_format && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read1_Format(false, true);
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null);
            }
            return (object)o;
        }

        System.String id16_BorderWeight;
        System.String id22_format;
        System.String id18_HatchStyle;
        System.String id14_StrikeOutColor;
        System.String id1_Format;
        System.String id15_BorderColor;
        System.String id19_UnderlineWeight;
        System.String id5_Font;
        System.String id4_name;
        System.String id9_style;
        System.String id17_BorderStyle;
        System.String id7_FontColor;
        System.String id6_ForeColor;
        System.String id3_UseCustomControl;
        System.String id20_UnderlineStyle;
        System.String id13_LineColor;
        System.String id8_BackColor;
        System.String id10_weight;
        System.String id12_UseHatchFill;
        System.String id2_Item;
        System.String id11_underline;
        System.String id21_FrameBorderStyle;
        /// <summary>
        /// Overrides the InitIDs
        /// </summary>
        protected override void InitIDs() {
            id16_BorderWeight = Reader.NameTable.Add(@"BorderWeight");
            id22_format = Reader.NameTable.Add(@"format");
            id18_HatchStyle = Reader.NameTable.Add(@"HatchStyle");
            id14_StrikeOutColor = Reader.NameTable.Add(@"StrikeOutColor");
            id1_Format = Reader.NameTable.Add(@"Format");
            id15_BorderColor = Reader.NameTable.Add(@"BorderColor");
            id19_UnderlineWeight = Reader.NameTable.Add(@"UnderlineWeight");
            id5_Font = Reader.NameTable.Add(@"Font");
            id4_name = Reader.NameTable.Add(@"name");
            id9_style = Reader.NameTable.Add(@"style");
            id17_BorderStyle = Reader.NameTable.Add(@"BorderStyle");
            id7_FontColor = Reader.NameTable.Add(@"FontColor");
            id6_ForeColor = Reader.NameTable.Add(@"ForeColor");
            id3_UseCustomControl = Reader.NameTable.Add(@"UseCustomControl");
            id20_UnderlineStyle = Reader.NameTable.Add(@"UnderlineStyle");
            id13_LineColor = Reader.NameTable.Add(@"LineColor");
            id8_BackColor = Reader.NameTable.Add(@"BackColor");
            id10_weight = Reader.NameTable.Add(@"weight");
            id12_UseHatchFill = Reader.NameTable.Add(@"UseHatchFill");
            id2_Item = Reader.NameTable.Add(@"");
            id11_underline = Reader.NameTable.Add(@"underline");
            id21_FrameBorderStyle = Reader.NameTable.Add(@"FrameBorderStyle");
        }
    }
}
