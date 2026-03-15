#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Forms.Edit.Utils.Serializers {

    /// <summary>
    /// Array of Macro Serialization Writer
    /// </summary>
    public class ArrayOfMacroSerializationWriter : System.Xml.Serialization.XmlSerializationWriter, Syncfusion.XmlSerializersCreator.IXmlSerializationWriter {

        void Write1_Macro(string n, string ns, Syncfusion.Windows.Forms.Edit.Implementation.Config.Internal.Macro o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (!(t == typeof(Syncfusion.Windows.Forms.Edit.Implementation.Config.Internal.Macro)))
                    throw CreateUnknownTypeException(o);
            }
            WriteStartElement(n, ns, o);
            if (needType) WriteXsiType(@"Macro", @"");
            WriteAttribute(@"Name", @"", (System.String)o.@Name);
            WriteAttribute(@"Regex", @"", (System.String)o.@Regex);
            WriteAttribute(@"Enabled", @"", System.Xml.XmlConvert.ToString((System.Boolean)(System.Boolean)o.@Enabled));
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
                    if (t == typeof(Syncfusion.Windows.Forms.Edit.Implementation.Config.Internal.Macro))
                    {
                        Write1_Macro(n, ns, (Syncfusion.Windows.Forms.Edit.Implementation.Config.Internal.Macro)o, isNullable, true);
                        return;
                    }
                    else if (t == typeof(Syncfusion.Windows.Forms.Edit.Implementation.Config.Internal.Macro[]))
                    {
                        Writer.WriteStartElement(n, ns);
                        WriteXsiType(@"ArrayOfMacro", @"");
                        {
                            Syncfusion.Windows.Forms.Edit.Implementation.Config.Internal.Macro[] a = (Syncfusion.Windows.Forms.Edit.Implementation.Config.Internal.Macro[])o;
                            if (a != null)
                            {
                                for (int ia = 0; ia < a.Length; ia++)
                                {
                                    Write1_Macro(@"Macro", @"", ((Syncfusion.Windows.Forms.Edit.Implementation.Config.Internal.Macro)a[ia]), true, false);
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
                WriteNullTagLiteral(@"ArrayOfMacro", @"");
                return;
            }
            TopLevelElement();
            {
                Syncfusion.Windows.Forms.Edit.Implementation.Config.Internal.Macro[] a = (Syncfusion.Windows.Forms.Edit.Implementation.Config.Internal.Macro[])((Syncfusion.Windows.Forms.Edit.Implementation.Config.Internal.Macro[])o);
                if ((object)(a) == null) {
                    WriteNullTagLiteral(@"ArrayOfMacro", @"");
                }
                else {
                    WriteStartElement(@"ArrayOfMacro", @"");
                    for (int ia = 0; ia < a.Length; ia++) {
                        Write1_Macro(@"Macro", @"", ((Syncfusion.Windows.Forms.Edit.Implementation.Config.Internal.Macro)a[ia]), true, false);
                    }
                    WriteEndElement();
                }
            }
        }
    }

    /// <summary>
    /// Array of Macro Serialization reader.
    /// </summary>
    public class ArrayOfMacroSerializationReader : System.Xml.Serialization.XmlSerializationReader, Syncfusion.XmlSerializersCreator.IXmlSerializationReader {

        Syncfusion.Windows.Forms.Edit.Implementation.Config.Internal.Macro Read1_Macro(bool isNullable, bool checkType) {
            if (isNullable && ReadNull()) return null;
            if (checkType) {
                System.Xml.XmlQualifiedName t = GetXsiType();
                if (!(t == null || ((object) ((System.Xml.XmlQualifiedName)t).Name == (object)id1_Macro && (object) ((System.Xml.XmlQualifiedName)t).Namespace == (object)id2_Item)))                
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)t);
            }
            Syncfusion.Windows.Forms.Edit.Implementation.Config.Internal.Macro o = new Syncfusion.Windows.Forms.Edit.Implementation.Config.Internal.Macro();
            bool[] paramsRead = new bool[3];
            while (Reader.MoveToNextAttribute()) {
                if (!paramsRead[0] && ((object) Reader.LocalName == (object)id3_Name && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o.@Name = Reader.Value;
                    paramsRead[0] = true;
                }
                else if (!paramsRead[1] && ((object) Reader.LocalName == (object)id4_Regex && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o.@Regex = Reader.Value;
                    paramsRead[1] = true;
                }
                else if (!paramsRead[2] && ((object) Reader.LocalName == (object)id5_Enabled && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o.@Enabled = System.Xml.XmlConvert.ToBoolean(Reader.Value);
                    paramsRead[2] = true;
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

        System.Object Read2_Object(bool isNullable, bool checkType) {
            if (isNullable && ReadNull()) return null;
            if (checkType) {
                System.Xml.XmlQualifiedName t = GetXsiType();
                if (t == null)
                    return ReadTypedPrimitive(new System.Xml.XmlQualifiedName("anyType", "http://www.w3.org/2001/XMLSchema"));
                else if (((object) ((System.Xml.XmlQualifiedName)t).Name == (object)id1_Macro && (object) ((System.Xml.XmlQualifiedName)t).Namespace == (object)id2_Item))
                    return Read1_Macro(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)t).Name == (object)id6_ArrayOfMacro && (object) ((System.Xml.XmlQualifiedName)t).Namespace == (object)id2_Item)) {
                    Syncfusion.Windows.Forms.Edit.Implementation.Config.Internal.Macro[] a = null;
                    if (!ReadNull()) {
                        Syncfusion.Windows.Forms.Edit.Implementation.Config.Internal.Macro[] z_0_0 = null;
                        int cz_0_0 = 0;
                        if (Reader.IsEmptyElement) {
                            Reader.Skip();
                        }
                        else {
                            Reader.ReadStartElement();
                            Reader.MoveToContent();
                            while (Reader.NodeType != System.Xml.XmlNodeType.EndElement) {
                                if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                                    if (((object) Reader.LocalName == (object)id1_Macro && (object) Reader.NamespaceURI == (object)id2_Item)) {
                                        z_0_0 = (Syncfusion.Windows.Forms.Edit.Implementation.Config.Internal.Macro[])EnsureArrayIndex(z_0_0, cz_0_0, typeof(Syncfusion.Windows.Forms.Edit.Implementation.Config.Internal.Macro));z_0_0[cz_0_0++] = Read1_Macro(true, true);
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
                        a = (Syncfusion.Windows.Forms.Edit.Implementation.Config.Internal.Macro[])ShrinkArray(z_0_0, cz_0_0, typeof(Syncfusion.Windows.Forms.Edit.Implementation.Config.Internal.Macro), false);
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
        /// <summary>
        /// Overrides the InitCallbacks
        /// </summary>
        protected override void InitCallbacks() {
        }

        /// <summary>
        /// Reads serialized data from Xml for Array of Macro
        /// </summary>
        /// <returns>returns object</returns>
        public object ReadDataFromXml() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id6_ArrayOfMacro && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    if (!ReadNull()) {
                        Syncfusion.Windows.Forms.Edit.Implementation.Config.Internal.Macro[] a_0_0 = null;
                        int ca_0_0 = 0;
                        if (Reader.IsEmptyElement) {
                            Reader.Skip();
                        }
                        else {
                            Reader.ReadStartElement();
                            Reader.MoveToContent();
                            while (Reader.NodeType != System.Xml.XmlNodeType.EndElement) {
                                if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                                    if (((object) Reader.LocalName == (object)id1_Macro && (object) Reader.NamespaceURI == (object)id2_Item)) {
                                        a_0_0 = (Syncfusion.Windows.Forms.Edit.Implementation.Config.Internal.Macro[])EnsureArrayIndex(a_0_0, ca_0_0, typeof(Syncfusion.Windows.Forms.Edit.Implementation.Config.Internal.Macro));a_0_0[ca_0_0++] = Read1_Macro(true, true);
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
                        o = (Syncfusion.Windows.Forms.Edit.Implementation.Config.Internal.Macro[])ShrinkArray(a_0_0, ca_0_0, typeof(Syncfusion.Windows.Forms.Edit.Implementation.Config.Internal.Macro), false);
                    }
                    else {
                        Syncfusion.Windows.Forms.Edit.Implementation.Config.Internal.Macro[] a_0_0 = null;
                        int ca_0_0 = 0;
                        o = (Syncfusion.Windows.Forms.Edit.Implementation.Config.Internal.Macro[])ShrinkArray(a_0_0, ca_0_0, typeof(Syncfusion.Windows.Forms.Edit.Implementation.Config.Internal.Macro), true);
                    }
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

        System.String id5_Enabled;
        System.String id4_Regex;
        System.String id3_Name;
        System.String id1_Macro;
        System.String id6_ArrayOfMacro;
        System.String id2_Item;
        /// <summary>
        /// Overrides the InitIDs
        /// </summary>
        protected override void InitIDs() {
            id5_Enabled = Reader.NameTable.Add(@"Enabled");
            id4_Regex = Reader.NameTable.Add(@"Regex");
            id3_Name = Reader.NameTable.Add(@"Name");
            id1_Macro = Reader.NameTable.Add(@"Macro");
            id6_ArrayOfMacro = Reader.NameTable.Add(@"ArrayOfMacro");
            id2_Item = Reader.NameTable.Add(@"");
        }
    }
}
