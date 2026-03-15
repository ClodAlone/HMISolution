#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Forms.Edit.Utils.Serializers {

    /// <summary>
    /// Split Serialization Writer
    /// </summary>
    public class SplitSerializationWriter : System.Xml.Serialization.XmlSerializationWriter, Syncfusion.XmlSerializersCreator.IXmlSerializationWriter {

        void Write1_Split(string n, string ns, Syncfusion.Windows.Forms.Edit.Implementation.Config.Split o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (!(t == typeof(Syncfusion.Windows.Forms.Edit.Implementation.Config.Split)))
                    throw CreateUnknownTypeException(o);                
            }
            WriteStartElement(n, ns, o);
            if (needType) WriteXsiType(@"Split", @"");
            if ((System.Boolean)o.@IsRegex != false) {
                WriteAttribute(@"IsRegex", @"", System.Xml.XmlConvert.ToString((System.Boolean)(System.Boolean)o.@IsRegex));
            }
            {
                WriteValue(((System.String)o.@Text));
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
                    if (t == typeof(Syncfusion.Windows.Forms.Edit.Implementation.Config.Split))
                    {
                        Write1_Split(n, ns, (Syncfusion.Windows.Forms.Edit.Implementation.Config.Split)o, isNullable, true);
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
                WriteNullTagLiteral(@"split", @"");
                return;
            }
            TopLevelElement();
            Write1_Split(@"split", @"", ((Syncfusion.Windows.Forms.Edit.Implementation.Config.Split)o), true, false);
        }
    }

    /// <summary>
    /// Split Serialization reader.
    /// </summary>
    public class SplitSerializationReader : System.Xml.Serialization.XmlSerializationReader, Syncfusion.XmlSerializersCreator.IXmlSerializationReader {

        Syncfusion.Windows.Forms.Edit.Implementation.Config.Split Read1_Split(bool isNullable, bool checkType) {
            if (isNullable && ReadNull()) return null;
            if (checkType) {
                System.Xml.XmlQualifiedName t = GetXsiType();
                if (!(t == null || ((object) ((System.Xml.XmlQualifiedName)t).Name == (object)id1_Split && (object) ((System.Xml.XmlQualifiedName)t).Namespace == (object)id2_Item)))                
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)t);
            }
            Syncfusion.Windows.Forms.Edit.Implementation.Config.Split o = new Syncfusion.Windows.Forms.Edit.Implementation.Config.Split();
            bool[] paramsRead = new bool[2];
            while (Reader.MoveToNextAttribute()) {
                if (!paramsRead[1] && ((object) Reader.LocalName == (object)id3_IsRegex && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o.@IsRegex = System.Xml.XmlConvert.ToBoolean(Reader.Value);
                    paramsRead[1] = true;
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
                string t = null;
                if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                    UnknownNode((object)o);
                }
                else if (Reader.NodeType == System.Xml.XmlNodeType.Text || 
                Reader.NodeType == System.Xml.XmlNodeType.CDATA || 
                Reader.NodeType == System.Xml.XmlNodeType.Whitespace || 
                Reader.NodeType == System.Xml.XmlNodeType.SignificantWhitespace) {
                    t = ReadString(t);
                    o.@Text = t;
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
                else if (((object) ((System.Xml.XmlQualifiedName)t).Name == (object)id1_Split && (object) ((System.Xml.XmlQualifiedName)t).Namespace == (object)id2_Item))
                    return Read1_Split(isNullable, false);
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
        /// Reads serialized data from Xml for Split
        /// </summary>
        /// <returns>returns object</returns>
        public object ReadDataFromXml() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id4_split && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read1_Split(true, true);
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

        System.String id1_Split;
        System.String id3_IsRegex;
        System.String id2_Item;
        System.String id4_split;
        /// <summary>
        /// Overrides the InitIDs
        /// </summary>
        protected override void InitIDs() {
            id1_Split = Reader.NameTable.Add(@"Split");
            id3_IsRegex = Reader.NameTable.Add(@"IsRegex");
            id2_Item = Reader.NameTable.Add(@"");
            id4_split = Reader.NameTable.Add(@"split");
        }
    }
}
