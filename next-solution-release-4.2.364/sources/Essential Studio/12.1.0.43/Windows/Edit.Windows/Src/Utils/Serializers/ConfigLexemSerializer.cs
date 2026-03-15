#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Forms.Edit.Utils.Serializers {

    /// <summary>
    /// Config Lexem Serialization Writer
    /// </summary>
    public class ConfigLexemSerializationWriter : System.Xml.Serialization.XmlSerializationWriter, Syncfusion.XmlSerializersCreator.IXmlSerializationWriter {

        void Write1_ConfigLexem(string n, string ns, Syncfusion.Windows.Forms.Edit.Implementation.Config.ConfigLexem o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (!(t == typeof(Syncfusion.Windows.Forms.Edit.Implementation.Config.ConfigLexem)))                
                    throw CreateUnknownTypeException(o);
            }
            WriteStartElement(n, ns, o);
            if (needType) WriteXsiType(@"ConfigLexem", @"");
            if ((System.String)o.@BeginBlock != @"") {
                WriteAttribute(@"BeginBlock", @"", (System.String)o.@BeginBlock);
            }
            if ((System.String)o.@EndBlock != @"") {
                WriteAttribute(@"EndBlock", @"", (System.String)o.@EndBlock);
            }
            if ((System.String)o.@ContinueBlock != @"") {
                WriteAttribute(@"ContinueBlock", @"", (System.String)o.@ContinueBlock);
            }
            if ((System.Int32)o.@Priority != 0) {
                WriteAttribute(@"Priority", @"", System.Xml.XmlConvert.ToString((System.Int32)(System.Int32)o.@Priority));
            }
            if ((System.String)o.@TypeXML != @"Text") {
                WriteAttribute(@"Type", @"", (System.String)o.@TypeXML);
            }
            if ((System.String)o.@TypeCollapsed != @"") {
                WriteAttribute(@"TypeCollapsed", @"", (System.String)o.@TypeCollapsed);
            }
            if ((System.String)o.@FormatName != @"Text") {
                WriteAttribute(@"FormatName", @"", (System.String)o.@FormatName);
            }
            if ((System.Boolean)o.@IsBeginRegex != false) {
                WriteAttribute(@"IsBeginRegex", @"", System.Xml.XmlConvert.ToString((System.Boolean)(System.Boolean)o.@IsBeginRegex));
            }
            if ((System.Boolean)o.@IsEndRegex != false) {
                WriteAttribute(@"IsEndRegex", @"", System.Xml.XmlConvert.ToString((System.Boolean)(System.Boolean)o.@IsEndRegex));
            }
            if ((System.Boolean)o.@IsContinueRegex != false) {
                WriteAttribute(@"IsContinueRegex", @"", System.Xml.XmlConvert.ToString((System.Boolean)(System.Boolean)o.@IsContinueRegex));
            }
            if ((System.Boolean)o.@IsComplex != false) {
                WriteAttribute(@"IsComplex", @"", System.Xml.XmlConvert.ToString((System.Boolean)(System.Boolean)o.@IsComplex));
            }
            if ((System.Boolean)o.@OnlyLocalSublexems != false) {
                WriteAttribute(@"OnlyLocalSublexems", @"", System.Xml.XmlConvert.ToString((System.Boolean)(System.Boolean)o.@OnlyLocalSublexems));
            }
            if ((System.Boolean)o.@IsCollapsable != false) {
                WriteAttribute(@"IsCollapsable", @"", System.Xml.XmlConvert.ToString((System.Boolean)(System.Boolean)o.@IsCollapsable));
            }
            if ((System.String)o.@AutoNameExpression != @"") {
                WriteAttribute(@"AutoNameExpression", @"", (System.String)o.@AutoNameExpression);
            }
            if ((System.String)o.@AutoNameTemplate != @"") {
                WriteAttribute(@"AutoNameTemplate", @"", (System.String)o.@AutoNameTemplate);
            }
            if ((System.Boolean)o.@IsCollapseAutoNamed != false) {
                WriteAttribute(@"IsCollapseAutoNamed", @"", System.Xml.XmlConvert.ToString((System.Boolean)(System.Boolean)o.@IsCollapseAutoNamed));
            }
            if (o.ShouldSerializeCollapseName()) {
                WriteAttribute(@"CollapseName", @"", (System.String)o.@CollapseName);
            }
            if ((System.String)o.@Condition != @"") {
                WriteAttribute(@"Condition", @"", (System.String)o.@Condition);
            }
            if (o.ShouldSerializeID()) {
                WriteAttribute(@"ID", @"", System.Xml.XmlConvert.ToString((System.Int32)(System.Int32)o.@ID));
            }
            if ((System.Boolean)o.@IsPseudoEnd != false) {
                WriteAttribute(@"IsPseudoEnd", @"", System.Xml.XmlConvert.ToString((System.Boolean)(System.Boolean)o.@IsPseudoEnd));
            }
            if ((System.Boolean)o.@Indent != false) {
                WriteAttribute(@"Indent", @"", System.Xml.XmlConvert.ToString((System.Boolean)(System.Boolean)o.@Indent));
            }
            if ((System.Int32)o.@NextID != 0) {
                WriteAttribute(@"NextID", @"", System.Xml.XmlConvert.ToString((System.Int32)(System.Int32)o.@NextID));
            }
            if ((System.Boolean)o.@DropContextChoiceList != false) {
                WriteAttribute(@"DropContextChoiceList", @"", System.Xml.XmlConvert.ToString((System.Boolean)(System.Boolean)o.@DropContextChoiceList));
            }
            if ((System.Boolean)o.@DropContextPrompt != false) {
                WriteAttribute(@"DropContextPrompt", @"", System.Xml.XmlConvert.ToString((System.Boolean)(System.Boolean)o.@DropContextPrompt));
            }
            if ((System.Boolean)o.@ContentDivider != false) {
                WriteAttribute(@"ContentDivider", @"", System.Xml.XmlConvert.ToString((System.Boolean)(System.Boolean)o.@ContentDivider));
            }
            if ((System.Boolean)o.@IndentationGuideline != false) {
                WriteAttribute(@"IndentationGuideline", @"", System.Xml.XmlConvert.ToString((System.Boolean)(System.Boolean)o.@IndentationGuideline));
            }
            if ((System.Boolean)o.@DefaultInGroup != false) {
                WriteAttribute(@"DefaultInGroup", @"", System.Xml.XmlConvert.ToString((System.Boolean)(System.Boolean)o.@DefaultInGroup));
            }
            if ((System.Boolean)o.@UseCustomControl != false) {
                WriteAttribute(@"UseCustomControl", @"", System.Xml.XmlConvert.ToString((System.Boolean)(System.Boolean)o.@UseCustomControl));
            }
            if ((System.Boolean)o.@AllowTriggers != true) {
                WriteAttribute(@"AllowTriggers", @"", System.Xml.XmlConvert.ToString((System.Boolean)(System.Boolean)o.@AllowTriggers));
            }
            if (o.ShouldSerializeSubLexems()) {
                {
                    System.Collections.ArrayList a = (System.Collections.ArrayList)((System.Collections.ArrayList)o.@SubLexems);
                    if (a != null){
                        WriteStartElement(@"SubLexems", @"");
                        for (int ia = 0; ia < a.Count; ia++) {
                            Write1_ConfigLexem(@"lexem", @"", ((Syncfusion.Windows.Forms.Edit.Implementation.Config.ConfigLexem)a[ia]), true, false);
                        }
                        WriteEndElement();
                    }
                }
            }
            if (o.ShouldSerializeReferences()) {
                {
                    System.Collections.ArrayList a = (System.Collections.ArrayList)((System.Collections.ArrayList)o.@References);
                    if (a != null){
                        WriteStartElement(@"References", @"");
                        for (int ia = 0; ia < a.Count; ia++) {
                            System.Object ai = a[ia];
                            {
                                if (ai is Syncfusion.Windows.Forms.Edit.Implementation.Config.ReferenceConfig) {
                                    Write3_ReferenceConfig(@"reference", @"", ((Syncfusion.Windows.Forms.Edit.Implementation.Config.ReferenceConfig)ai), true, false);
                                }
                                else if (ai is System.Object) {
                                    Write2_Object(@"References", @"", ((System.Object)ai), true, false);
                                }
                                else {
                                    if (ai != null) {
                                        throw CreateUnknownTypeException(ai);
                                    }
                                }
                            }
                        }
                        WriteEndElement();
                    }
                }
            }
            WriteEndElement(o);
        }

        void Write2_Object(string n, string ns, System.Object o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (!(t == typeof(System.Object)))
                {
                    if (t == typeof(Syncfusion.Windows.Forms.Edit.Implementation.Config.ConfigLexem))
                    {
                        Write1_ConfigLexem(n, ns, (Syncfusion.Windows.Forms.Edit.Implementation.Config.ConfigLexem)o, isNullable, true);
                        return;
                    }
                    else if (t == typeof(Syncfusion.Windows.Forms.Edit.Implementation.Config.ReferenceConfig))
                    {
                        Write3_ReferenceConfig(n, ns, (Syncfusion.Windows.Forms.Edit.Implementation.Config.ReferenceConfig)o, isNullable, true);
                        return;
                    }
                    else if (t == typeof(System.Collections.ArrayList))
                    {
                        Writer.WriteStartElement(n, ns);
                        WriteXsiType(@"ArrayOfConfigLexem", @"");
                        {
                            System.Collections.ArrayList a = (System.Collections.ArrayList)o;
                            if (a != null)
                            {
                                for (int ia = 0; ia < a.Count; ia++)
                                {
                                    Write1_ConfigLexem(@"lexem", @"", ((Syncfusion.Windows.Forms.Edit.Implementation.Config.ConfigLexem)a[ia]), true, false);
                                }
                            }
                        }
                        Writer.WriteEndElement();
                        return;
                    }
                    else if (t == typeof(System.Collections.ArrayList))
                    {
                        Writer.WriteStartElement(n, ns);
                        WriteXsiType(@"ArrayOfChoice1", @"");
                        {
                            System.Collections.ArrayList a = (System.Collections.ArrayList)o;
                            if (a != null)
                            {
                                for (int ia = 0; ia < a.Count; ia++)
                                {
                                    System.Object ai = a[ia];
                                    {
                                        if (ai is Syncfusion.Windows.Forms.Edit.Implementation.Config.ReferenceConfig)
                                        {
                                            Write3_ReferenceConfig(@"reference", @"", ((Syncfusion.Windows.Forms.Edit.Implementation.Config.ReferenceConfig)ai), true, false);
                                        }
                                        else if (ai is System.Object)
                                        {
                                            Write2_Object(@"References", @"", ((System.Object)ai), true, false);
                                        }
                                        else
                                        {
                                            if (ai != null)
                                            {
                                                throw CreateUnknownTypeException(ai);
                                            }
                                        }
                                    }
                                }
                            }
                        }
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

        void Write3_ReferenceConfig(string n, string ns, Syncfusion.Windows.Forms.Edit.Implementation.Config.ReferenceConfig o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (!(t == typeof(Syncfusion.Windows.Forms.Edit.Implementation.Config.ReferenceConfig)))                
                    throw CreateUnknownTypeException(o);
                
            }
            WriteStartElement(n, ns, o);
            if (needType) WriteXsiType(@"ReferenceConfig", @"");
            WriteAttribute(@"RefID", @"", System.Xml.XmlConvert.ToString((System.Int32)(System.Int32)o.@RefID));
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
                WriteNullTagLiteral(@"lexem", @"");
                return;
            }
            TopLevelElement();
            Write1_ConfigLexem(@"lexem", @"", ((Syncfusion.Windows.Forms.Edit.Implementation.Config.ConfigLexem)o), true, false);
        }
    }

    /// <summary>
    ///  Config Lexem Serialization reader.
    /// </summary>
    public class ConfigLexemSerializationReader : System.Xml.Serialization.XmlSerializationReader, Syncfusion.XmlSerializersCreator.IXmlSerializationReader {

        Syncfusion.Windows.Forms.Edit.Implementation.Config.ConfigLexem Read1_ConfigLexem(bool isNullable, bool checkType) {
            if (isNullable && ReadNull()) return null;
            if (checkType) {
                System.Xml.XmlQualifiedName t = GetXsiType();
                if (!(t == null || ((object) ((System.Xml.XmlQualifiedName)t).Name == (object)id1_ConfigLexem && (object) ((System.Xml.XmlQualifiedName)t).Namespace == (object)id2_Item)))                
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)t);
            }
            Syncfusion.Windows.Forms.Edit.Implementation.Config.ConfigLexem o = new Syncfusion.Windows.Forms.Edit.Implementation.Config.ConfigLexem();
            if ((object)(o.@SubLexems) == null) o.@SubLexems = new System.Collections.ArrayList();
            System.Collections.ArrayList a_12 = (System.Collections.ArrayList)o.@SubLexems;
            if ((object)(o.@References) == null) o.@References = new System.Collections.ArrayList();
            System.Collections.ArrayList a_13 = (System.Collections.ArrayList)o.@References;
            bool[] paramsRead = new bool[31];
            while (Reader.MoveToNextAttribute()) {
                if (!paramsRead[0] && ((object) Reader.LocalName == (object)id3_BeginBlock && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o.@BeginBlock = Reader.Value;
                    paramsRead[0] = true;
                }
                else if (!paramsRead[1] && ((object) Reader.LocalName == (object)id4_EndBlock && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o.@EndBlock = Reader.Value;
                    paramsRead[1] = true;
                }
                else if (!paramsRead[2] && ((object) Reader.LocalName == (object)id5_ContinueBlock && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o.@ContinueBlock = Reader.Value;
                    paramsRead[2] = true;
                }
                else if (!paramsRead[3] && ((object) Reader.LocalName == (object)id6_Priority && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o.@Priority = System.Xml.XmlConvert.ToInt32(Reader.Value);
                    paramsRead[3] = true;
                }
                else if (!paramsRead[4] && ((object) Reader.LocalName == (object)id7_Type && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o.@TypeXML = Reader.Value;
                    paramsRead[4] = true;
                }
                else if (!paramsRead[5] && ((object) Reader.LocalName == (object)id8_TypeCollapsed && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o.@TypeCollapsed = Reader.Value;
                    paramsRead[5] = true;
                }
                else if (!paramsRead[6] && ((object) Reader.LocalName == (object)id9_FormatName && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o.@FormatName = Reader.Value;
                    paramsRead[6] = true;
                }
                else if (!paramsRead[7] && ((object) Reader.LocalName == (object)id10_IsBeginRegex && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o.@IsBeginRegex = System.Xml.XmlConvert.ToBoolean(Reader.Value);
                    paramsRead[7] = true;
                }
                else if (!paramsRead[8] && ((object) Reader.LocalName == (object)id11_IsEndRegex && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o.@IsEndRegex = System.Xml.XmlConvert.ToBoolean(Reader.Value);
                    paramsRead[8] = true;
                }
                else if (!paramsRead[9] && ((object) Reader.LocalName == (object)id12_IsContinueRegex && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o.@IsContinueRegex = System.Xml.XmlConvert.ToBoolean(Reader.Value);
                    paramsRead[9] = true;
                }
                else if (!paramsRead[10] && ((object) Reader.LocalName == (object)id13_IsComplex && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o.@IsComplex = System.Xml.XmlConvert.ToBoolean(Reader.Value);
                    paramsRead[10] = true;
                }
                else if (!paramsRead[11] && ((object) Reader.LocalName == (object)id14_OnlyLocalSublexems && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o.@OnlyLocalSublexems = System.Xml.XmlConvert.ToBoolean(Reader.Value);
                    paramsRead[11] = true;
                }
                else if (!paramsRead[14] && ((object) Reader.LocalName == (object)id15_IsCollapsable && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o.@IsCollapsable = System.Xml.XmlConvert.ToBoolean(Reader.Value);
                    paramsRead[14] = true;
                }
                else if (!paramsRead[15] && ((object) Reader.LocalName == (object)id16_AutoNameExpression && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o.@AutoNameExpression = Reader.Value;
                    paramsRead[15] = true;
                }
                else if (!paramsRead[16] && ((object) Reader.LocalName == (object)id17_AutoNameTemplate && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o.@AutoNameTemplate = Reader.Value;
                    paramsRead[16] = true;
                }
                else if (!paramsRead[17] && ((object) Reader.LocalName == (object)id18_IsCollapseAutoNamed && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o.@IsCollapseAutoNamed = System.Xml.XmlConvert.ToBoolean(Reader.Value);
                    paramsRead[17] = true;
                }
                else if (!paramsRead[18] && ((object) Reader.LocalName == (object)id19_CollapseName && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o.@CollapseName = Reader.Value;
                    paramsRead[18] = true;
                }
                else if (!paramsRead[19] && ((object) Reader.LocalName == (object)id20_Condition && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o.@Condition = Reader.Value;
                    paramsRead[19] = true;
                }
                else if (!paramsRead[20] && ((object) Reader.LocalName == (object)id21_ID && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o.@ID = System.Xml.XmlConvert.ToInt32(Reader.Value);
                    paramsRead[20] = true;
                }
                else if (!paramsRead[21] && ((object) Reader.LocalName == (object)id22_IsPseudoEnd && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o.@IsPseudoEnd = System.Xml.XmlConvert.ToBoolean(Reader.Value);
                    paramsRead[21] = true;
                }
                else if (!paramsRead[22] && ((object) Reader.LocalName == (object)id23_Indent && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o.@Indent = System.Xml.XmlConvert.ToBoolean(Reader.Value);
                    paramsRead[22] = true;
                }
                else if (!paramsRead[23] && ((object) Reader.LocalName == (object)id24_NextID && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o.@NextID = System.Xml.XmlConvert.ToInt32(Reader.Value);
                    paramsRead[23] = true;
                }
                else if (!paramsRead[24] && ((object) Reader.LocalName == (object)id25_DropContextChoiceList && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o.@DropContextChoiceList = System.Xml.XmlConvert.ToBoolean(Reader.Value);
                    paramsRead[24] = true;
                }
                else if (!paramsRead[25] && ((object) Reader.LocalName == (object)id26_DropContextPrompt && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o.@DropContextPrompt = System.Xml.XmlConvert.ToBoolean(Reader.Value);
                    paramsRead[25] = true;
                }
                else if (!paramsRead[26] && ((object) Reader.LocalName == (object)id27_ContentDivider && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o.@ContentDivider = System.Xml.XmlConvert.ToBoolean(Reader.Value);
                    paramsRead[26] = true;
                }
                else if (!paramsRead[27] && ((object) Reader.LocalName == (object)id28_IndentationGuideline && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o.@IndentationGuideline = System.Xml.XmlConvert.ToBoolean(Reader.Value);
                    paramsRead[27] = true;
                }
                else if (!paramsRead[28] && ((object) Reader.LocalName == (object)id29_DefaultInGroup && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o.@DefaultInGroup = System.Xml.XmlConvert.ToBoolean(Reader.Value);
                    paramsRead[28] = true;
                }
                else if (!paramsRead[29] && ((object) Reader.LocalName == (object)id30_UseCustomControl && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o.@UseCustomControl = System.Xml.XmlConvert.ToBoolean(Reader.Value);
                    paramsRead[29] = true;
                }
                else if (!paramsRead[30] && ((object) Reader.LocalName == (object)id31_AllowTriggers && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o.@AllowTriggers = System.Xml.XmlConvert.ToBoolean(Reader.Value);
                    paramsRead[30] = true;
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
                    if (((object) Reader.LocalName == (object)id32_SubLexems && (object) Reader.NamespaceURI == (object)id2_Item)) {
                        if (!ReadNull()) {
                            if ((object)(o.@SubLexems) == null) o.@SubLexems = new System.Collections.ArrayList();
                            System.Collections.ArrayList a_12_0 = (System.Collections.ArrayList)o.@SubLexems;
                            if (Reader.IsEmptyElement) {
                                Reader.Skip();
                            }
                            else {
                                Reader.ReadStartElement();
                                Reader.MoveToContent();
                                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement) {
                                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                                        if (((object) Reader.LocalName == (object)id33_lexem && (object) Reader.NamespaceURI == (object)id2_Item)) {
                                            if ((object)(a_12_0) == null) Reader.Skip(); else a_12_0.Add(Read1_ConfigLexem(true, true));
                                        }
                                        else {
                                            UnknownNode(null);
                                        }
                                    }
                                    else {
                                        UnknownNode(null);
                                    }
                                    Reader.MoveToContent();
                                }
                            ReadEndElement();
                            }
                        }
                    }
                    else if (((object) Reader.LocalName == (object)id34_References && (object) Reader.NamespaceURI == (object)id2_Item)) {
                        if (!ReadNull()) {
                            if ((object)(o.@References) == null) o.@References = new System.Collections.ArrayList();
                            System.Collections.ArrayList a_13_0 = (System.Collections.ArrayList)o.@References;
                            if (Reader.IsEmptyElement) {
                                Reader.Skip();
                            }
                            else {
                                Reader.ReadStartElement();
                                Reader.MoveToContent();
                                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement) {
                                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                                        if (((object) Reader.LocalName == (object)id35_reference && (object) Reader.NamespaceURI == (object)id2_Item)) {
                                            if ((object)(a_13_0) == null) Reader.Skip(); else a_13_0.Add(Read3_ReferenceConfig(true, true));
                                        }
                                        else if (((object) Reader.LocalName == (object)id34_References && (object) Reader.NamespaceURI == (object)id2_Item)) {
                                            if ((object)(a_13_0) == null) Reader.Skip(); else a_13_0.Add(Read2_Object(true, true));
                                        }
                                        else {
                                            UnknownNode(null);
                                        }
                                    }
                                    else {
                                        UnknownNode(null);
                                    }
                                    Reader.MoveToContent();
                                }
                            ReadEndElement();
                            }
                        }
                    }
                    else {
                        UnknownNode((object)o);
                    }
                }
                else {
                    UnknownNode((object)o);
                }
                Reader.MoveToContent();
            }
            ReadEndElement();
            return o;
        }

        System.Object Read2_Object(bool isNullable, bool checkType) {
            if (isNullable && ReadNull()) return null;
            if (checkType) {
                System.Xml.XmlQualifiedName t = GetXsiType();
                if (t == null)
                    return ReadTypedPrimitive(new System.Xml.XmlQualifiedName("anyType", "http://www.w3.org/2001/XMLSchema"));
                else if (((object) ((System.Xml.XmlQualifiedName)t).Name == (object)id1_ConfigLexem && (object) ((System.Xml.XmlQualifiedName)t).Namespace == (object)id2_Item))
                    return Read1_ConfigLexem(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)t).Name == (object)id36_ReferenceConfig && (object) ((System.Xml.XmlQualifiedName)t).Namespace == (object)id2_Item))
                    return Read3_ReferenceConfig(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)t).Name == (object)id37_ArrayOfConfigLexem && (object) ((System.Xml.XmlQualifiedName)t).Namespace == (object)id2_Item)) {
                    System.Collections.ArrayList a = null;
                    if (!ReadNull()) {
                        if ((object)(a) == null) a = new System.Collections.ArrayList();
                        System.Collections.ArrayList z_0_0 = (System.Collections.ArrayList)a;
                        if (Reader.IsEmptyElement) {
                            Reader.Skip();
                        }
                        else {
                            Reader.ReadStartElement();
                            Reader.MoveToContent();
                            while (Reader.NodeType != System.Xml.XmlNodeType.EndElement) {
                                if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                                    if (((object) Reader.LocalName == (object)id33_lexem && (object) Reader.NamespaceURI == (object)id2_Item)) {
                                        if ((object)(z_0_0) == null) Reader.Skip(); else z_0_0.Add(Read1_ConfigLexem(true, true));
                                    }
                                    else {
                                        UnknownNode(null);
                                    }
                                }
                                else {
                                    UnknownNode(null);
                                }
                                Reader.MoveToContent();
                            }
                        ReadEndElement();
                        }
                    }
                    return a;
                }
                else if (((object) ((System.Xml.XmlQualifiedName)t).Name == (object)id38_ArrayOfChoice1 && (object) ((System.Xml.XmlQualifiedName)t).Namespace == (object)id2_Item)) {
                    System.Collections.ArrayList a = null;
                    if (!ReadNull()) {
                        if ((object)(a) == null) a = new System.Collections.ArrayList();
                        System.Collections.ArrayList z_0_0 = (System.Collections.ArrayList)a;
                        if (Reader.IsEmptyElement) {
                            Reader.Skip();
                        }
                        else {
                            Reader.ReadStartElement();
                            Reader.MoveToContent();
                            while (Reader.NodeType != System.Xml.XmlNodeType.EndElement) {
                                if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                                    if (((object) Reader.LocalName == (object)id35_reference && (object) Reader.NamespaceURI == (object)id2_Item)) {
                                        if ((object)(z_0_0) == null) Reader.Skip(); else z_0_0.Add(Read3_ReferenceConfig(true, true));
                                    }
                                    else if (((object) Reader.LocalName == (object)id34_References && (object) Reader.NamespaceURI == (object)id2_Item)) {
                                        if ((object)(z_0_0) == null) Reader.Skip(); else z_0_0.Add(Read2_Object(true, true));
                                    }
                                    else {
                                        UnknownNode(null);
                                    }
                                }
                                else {
                                    UnknownNode(null);
                                }
                                Reader.MoveToContent();
                            }
                        ReadEndElement();
                        }
                    }
                    return a;
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

        Syncfusion.Windows.Forms.Edit.Implementation.Config.ReferenceConfig Read3_ReferenceConfig(bool isNullable, bool checkType) {
            if (isNullable && ReadNull()) return null;
            if (checkType) {
                System.Xml.XmlQualifiedName t = GetXsiType();
                if (!(t == null || ((object) ((System.Xml.XmlQualifiedName)t).Name == (object)id36_ReferenceConfig && (object) ((System.Xml.XmlQualifiedName)t).Namespace == (object)id2_Item)))                
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)t);
            }
            Syncfusion.Windows.Forms.Edit.Implementation.Config.ReferenceConfig o = new Syncfusion.Windows.Forms.Edit.Implementation.Config.ReferenceConfig();
            bool[] paramsRead = new bool[1];
            while (Reader.MoveToNextAttribute()) {
                if (!paramsRead[0] && ((object) Reader.LocalName == (object)id39_RefID && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o.@RefID = System.Xml.XmlConvert.ToInt32(Reader.Value);
                    paramsRead[0] = true;
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
        /// <summary>
        /// Overrides the InitCallbacks
        /// </summary>
        protected override void InitCallbacks() {
        }

        /// <summary>
        /// Reads serialized data from Xml for Config Lexem
        /// </summary>
        /// <returns>returns object</returns>
        public object ReadDataFromXml() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id33_lexem && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read1_ConfigLexem(true, true);
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

        System.String id33_lexem;
        System.String id11_IsEndRegex;
        System.String id3_BeginBlock;
        System.String id9_FormatName;
        System.String id21_ID;
        System.String id30_UseCustomControl;
        System.String id13_IsComplex;
        System.String id31_AllowTriggers;
        System.String id37_ArrayOfConfigLexem;
        System.String id27_ContentDivider;
        System.String id32_SubLexems;
        System.String id5_ContinueBlock;
        System.String id4_EndBlock;
        System.String id36_ReferenceConfig;
        System.String id23_Indent;
        System.String id15_IsCollapsable;
        System.String id35_reference;
        System.String id16_AutoNameExpression;
        System.String id12_IsContinueRegex;
        System.String id34_References;
        System.String id26_DropContextPrompt;
        System.String id8_TypeCollapsed;
        System.String id10_IsBeginRegex;
        System.String id7_Type;
        System.String id38_ArrayOfChoice1;
        System.String id1_ConfigLexem;
        System.String id20_Condition;
        System.String id29_DefaultInGroup;
        System.String id2_Item;
        System.String id6_Priority;
        System.String id19_CollapseName;
        System.String id39_RefID;
        System.String id28_IndentationGuideline;
        System.String id14_OnlyLocalSublexems;
        System.String id24_NextID;
        System.String id18_IsCollapseAutoNamed;
        System.String id25_DropContextChoiceList;
        System.String id22_IsPseudoEnd;
        System.String id17_AutoNameTemplate;
        /// <summary>
        /// Overrides the InitIDs
        /// </summary>
        protected override void InitIDs() {
            id33_lexem = Reader.NameTable.Add(@"lexem");
            id11_IsEndRegex = Reader.NameTable.Add(@"IsEndRegex");
            id3_BeginBlock = Reader.NameTable.Add(@"BeginBlock");
            id9_FormatName = Reader.NameTable.Add(@"FormatName");
            id21_ID = Reader.NameTable.Add(@"ID");
            id30_UseCustomControl = Reader.NameTable.Add(@"UseCustomControl");
            id13_IsComplex = Reader.NameTable.Add(@"IsComplex");
            id31_AllowTriggers = Reader.NameTable.Add(@"AllowTriggers");
            id37_ArrayOfConfigLexem = Reader.NameTable.Add(@"ArrayOfConfigLexem");
            id27_ContentDivider = Reader.NameTable.Add(@"ContentDivider");
            id32_SubLexems = Reader.NameTable.Add(@"SubLexems");
            id5_ContinueBlock = Reader.NameTable.Add(@"ContinueBlock");
            id4_EndBlock = Reader.NameTable.Add(@"EndBlock");
            id36_ReferenceConfig = Reader.NameTable.Add(@"ReferenceConfig");
            id23_Indent = Reader.NameTable.Add(@"Indent");
            id15_IsCollapsable = Reader.NameTable.Add(@"IsCollapsable");
            id35_reference = Reader.NameTable.Add(@"reference");
            id16_AutoNameExpression = Reader.NameTable.Add(@"AutoNameExpression");
            id12_IsContinueRegex = Reader.NameTable.Add(@"IsContinueRegex");
            id34_References = Reader.NameTable.Add(@"References");
            id26_DropContextPrompt = Reader.NameTable.Add(@"DropContextPrompt");
            id8_TypeCollapsed = Reader.NameTable.Add(@"TypeCollapsed");
            id10_IsBeginRegex = Reader.NameTable.Add(@"IsBeginRegex");
            id7_Type = Reader.NameTable.Add(@"Type");
            id38_ArrayOfChoice1 = Reader.NameTable.Add(@"ArrayOfChoice1");
            id1_ConfigLexem = Reader.NameTable.Add(@"ConfigLexem");
            id20_Condition = Reader.NameTable.Add(@"Condition");
            id29_DefaultInGroup = Reader.NameTable.Add(@"DefaultInGroup");
            id2_Item = Reader.NameTable.Add(@"");
            id6_Priority = Reader.NameTable.Add(@"Priority");
            id19_CollapseName = Reader.NameTable.Add(@"CollapseName");
            id39_RefID = Reader.NameTable.Add(@"RefID");
            id28_IndentationGuideline = Reader.NameTable.Add(@"IndentationGuideline");
            id14_OnlyLocalSublexems = Reader.NameTable.Add(@"OnlyLocalSublexems");
            id24_NextID = Reader.NameTable.Add(@"NextID");
            id18_IsCollapseAutoNamed = Reader.NameTable.Add(@"IsCollapseAutoNamed");
            id25_DropContextChoiceList = Reader.NameTable.Add(@"DropContextChoiceList");
            id22_IsPseudoEnd = Reader.NameTable.Add(@"IsPseudoEnd");
            id17_AutoNameTemplate = Reader.NameTable.Add(@"AutoNameTemplate");
        }
    }
}
