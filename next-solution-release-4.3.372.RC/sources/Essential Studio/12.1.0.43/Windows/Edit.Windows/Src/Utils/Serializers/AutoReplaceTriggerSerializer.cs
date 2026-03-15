#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Forms.Edit.Utils.Serializers {

    /// <summary>
    /// Auto Replace Trigger Serialization Writer
    /// </summary>
    public class AutoReplaceTriggerSerializationWriter : System.Xml.Serialization.XmlSerializationWriter, Syncfusion.XmlSerializersCreator.IXmlSerializationWriter {

        void Write1_AutoReplaceTrigger(string n, string ns, Syncfusion.Windows.Forms.Edit.Utils.AutoReplaceTrigger o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (!(t == typeof(Syncfusion.Windows.Forms.Edit.Utils.AutoReplaceTrigger)))                    
                    throw CreateUnknownTypeException(o);
            }
            WriteStartElement(n, ns, o);
            if (needType) WriteXsiType(@"AutoReplaceTrigger", @"");
            WriteAttribute(@"From", @"", (System.String)o.@From);
            WriteAttribute(@"To", @"", (System.String)o.@To);
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
                    if (t == typeof(Syncfusion.Windows.Forms.Edit.Utils.AutoReplaceTrigger))
                    {
                        Write1_AutoReplaceTrigger(n, ns, (Syncfusion.Windows.Forms.Edit.Utils.AutoReplaceTrigger)o, isNullable, true);
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
                WriteNullTagLiteral(@"AutoReplaceTrigger", @"");
                return;
            }
            TopLevelElement();
            Write1_AutoReplaceTrigger(@"AutoReplaceTrigger", @"", ((Syncfusion.Windows.Forms.Edit.Utils.AutoReplaceTrigger)o), true, false);
        }
    }

    /// <summary>
    /// Auto Replace Trigger Serialization reader.
    /// </summary>
    public class AutoReplaceTriggerSerializationReader : System.Xml.Serialization.XmlSerializationReader, Syncfusion.XmlSerializersCreator.IXmlSerializationReader {

        Syncfusion.Windows.Forms.Edit.Utils.AutoReplaceTrigger Read1_AutoReplaceTrigger(bool isNullable, bool checkType) {
            if (isNullable && ReadNull()) return null;
            if (checkType) {
                System.Xml.XmlQualifiedName t = GetXsiType();
                if (!(t == null || ((object) ((System.Xml.XmlQualifiedName)t).Name == (object)id1_AutoReplaceTrigger && (object) ((System.Xml.XmlQualifiedName)t).Namespace == (object)id2_Item)))                
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)t);
            }
            Syncfusion.Windows.Forms.Edit.Utils.AutoReplaceTrigger o = new Syncfusion.Windows.Forms.Edit.Utils.AutoReplaceTrigger();
            bool[] paramsRead = new bool[2];
            while (Reader.MoveToNextAttribute()) {
                if (!paramsRead[0] && ((object) Reader.LocalName == (object)id3_From && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o.@From = Reader.Value;
                    paramsRead[0] = true;
                }
                else if (!paramsRead[1] && ((object) Reader.LocalName == (object)id4_To && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o.@To = Reader.Value;
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
                else if (((object) ((System.Xml.XmlQualifiedName)t).Name == (object)id1_AutoReplaceTrigger && (object) ((System.Xml.XmlQualifiedName)t).Namespace == (object)id2_Item))
                    return Read1_AutoReplaceTrigger(isNullable, false);
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
        /// Reads serialized data from Xml for Auto Replace Trigger 
        /// </summary>
        /// <returns>returns object</returns>
        public object ReadDataFromXml() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id1_AutoReplaceTrigger && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read1_AutoReplaceTrigger(true, true);
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

        System.String id3_From;
        System.String id1_AutoReplaceTrigger;
        System.String id4_To;
        System.String id2_Item;
        /// <summary>
        /// Overrides the InitIDs
        /// </summary>
        protected override void InitIDs() {
            id3_From = Reader.NameTable.Add(@"From");
            id1_AutoReplaceTrigger = Reader.NameTable.Add(@"AutoReplaceTrigger");
            id4_To = Reader.NameTable.Add(@"To");
            id2_Item = Reader.NameTable.Add(@"");
        }
    }
}
