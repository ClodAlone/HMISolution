#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Forms.Edit.Utils.Serializers {

    /// <summary>
    /// Key Command List Binder Impl Serialization Writer
    /// </summary>
    public class KeyCommandListBinderImplSerializationWriter : System.Xml.Serialization.XmlSerializationWriter, Syncfusion.XmlSerializersCreator.IXmlSerializationWriter {

        void Write1_KeyCommandListBinderImpl(string n, string ns, Syncfusion.Shared.Utils.KeyBinding.Implementation.KeyProcessor.KeyCommandListBinderImpl o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (!(t == typeof(Syncfusion.Shared.Utils.KeyBinding.Implementation.KeyProcessor.KeyCommandListBinderImpl)))                
                    throw CreateUnknownTypeException(o);
            }
            WriteStartElement(n, ns, o);
            if (needType) WriteXsiType(@"KeyCommandListBinderImpl", @"");
            if ((System.String)o.@CommandName != @"") {
                WriteAttribute(@"Command", @"", (System.String)o.@CommandName);
            }
            if ((System.String)o.@KeyXML != @"None") {
                WriteAttribute(@"Key", @"", (System.String)o.@KeyXML);
            }
            {
                Syncfusion.Shared.Utils.KeyBinding.Implementation.KeyProcessor.KeyCommandBinderImpl[] a = (Syncfusion.Shared.Utils.KeyBinding.Implementation.KeyProcessor.KeyCommandBinderImpl[])((Syncfusion.Shared.Utils.KeyBinding.Implementation.KeyProcessor.KeyCommandBinderImpl[])o.@SubBundings);
                if ((object)(a) == null) {
                    WriteNullTagLiteral(@"Bindings", @"");
                }
                else {
                    WriteStartElement(@"Bindings", @"");
                    for (int ia = 0; ia < a.Length; ia++) {
                        Syncfusion.Shared.Utils.KeyBinding.Implementation.KeyProcessor.KeyCommandBinderImpl ai = a[ia];
                        {
                            if (ai is Syncfusion.Shared.Utils.KeyBinding.Implementation.KeyProcessor.KeyCommandListBinderImpl) {
                                Write1_KeyCommandListBinderImpl(@"BindingList", @"", ((Syncfusion.Shared.Utils.KeyBinding.Implementation.KeyProcessor.KeyCommandListBinderImpl)ai), true, false);
                            }
                            else if (ai is Syncfusion.Shared.Utils.KeyBinding.Implementation.KeyProcessor.KeyCommandBinderImpl) {
                                Write2_KeyCommandBinderImpl(@"Binding", @"", ((Syncfusion.Shared.Utils.KeyBinding.Implementation.KeyProcessor.KeyCommandBinderImpl)ai), true, false);
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
            WriteEndElement(o);
        }

        void Write2_KeyCommandBinderImpl(string n, string ns, Syncfusion.Shared.Utils.KeyBinding.Implementation.KeyProcessor.KeyCommandBinderImpl o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (!(t == typeof(Syncfusion.Shared.Utils.KeyBinding.Implementation.KeyProcessor.KeyCommandBinderImpl)))
                {
                    if (t == typeof(Syncfusion.Shared.Utils.KeyBinding.Implementation.KeyProcessor.KeyCommandListBinderImpl))
                    {
                        Write1_KeyCommandListBinderImpl(n, ns, (Syncfusion.Shared.Utils.KeyBinding.Implementation.KeyProcessor.KeyCommandListBinderImpl)o, isNullable, true);
                        return;
                    }
                    else
                    {
                        throw CreateUnknownTypeException(o);
                    }
                }
            }
            WriteStartElement(n, ns, o);
            if (needType) WriteXsiType(@"KeyCommandBinderImpl", @"");
            if ((System.String)o.@CommandName != @"") {
                WriteAttribute(@"Command", @"", (System.String)o.@CommandName);
            }
            if ((System.String)o.@KeyXML != @"None") {
                WriteAttribute(@"Key", @"", (System.String)o.@KeyXML);
            }
            WriteEndElement(o);
        }

        void Write3_Object(string n, string ns, System.Object o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (!(t == typeof(System.Object)))
                {
                    if (t == typeof(Syncfusion.Shared.Utils.KeyBinding.Implementation.KeyProcessor.KeyCommandBinderImpl))
                    {
                        Write2_KeyCommandBinderImpl(n, ns, (Syncfusion.Shared.Utils.KeyBinding.Implementation.KeyProcessor.KeyCommandBinderImpl)o, isNullable, true);
                        return;
                    }
                    else if (t == typeof(Syncfusion.Shared.Utils.KeyBinding.Implementation.KeyProcessor.KeyCommandListBinderImpl))
                    {
                        Write1_KeyCommandListBinderImpl(n, ns, (Syncfusion.Shared.Utils.KeyBinding.Implementation.KeyProcessor.KeyCommandListBinderImpl)o, isNullable, true);
                        return;
                    }
                    else if (t == typeof(Syncfusion.Shared.Utils.KeyBinding.Implementation.KeyProcessor.KeyCommandBinderImpl[]))
                    {
                        Writer.WriteStartElement(n, ns);
                        WriteXsiType(@"ArrayOfChoice1", @"");
                        {
                            Syncfusion.Shared.Utils.KeyBinding.Implementation.KeyProcessor.KeyCommandBinderImpl[] a = (Syncfusion.Shared.Utils.KeyBinding.Implementation.KeyProcessor.KeyCommandBinderImpl[])o;
                            if (a != null)
                            {
                                for (int ia = 0; ia < a.Length; ia++)
                                {
                                    Syncfusion.Shared.Utils.KeyBinding.Implementation.KeyProcessor.KeyCommandBinderImpl ai = a[ia];
                                    {
                                        if (ai is Syncfusion.Shared.Utils.KeyBinding.Implementation.KeyProcessor.KeyCommandListBinderImpl)
                                        {
                                            Write1_KeyCommandListBinderImpl(@"BindingList", @"", ((Syncfusion.Shared.Utils.KeyBinding.Implementation.KeyProcessor.KeyCommandListBinderImpl)ai), true, false);
                                        }
                                        else if (ai is Syncfusion.Shared.Utils.KeyBinding.Implementation.KeyProcessor.KeyCommandBinderImpl)
                                        {
                                            Write2_KeyCommandBinderImpl(@"Binding", @"", ((Syncfusion.Shared.Utils.KeyBinding.Implementation.KeyProcessor.KeyCommandBinderImpl)ai), true, false);
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
                WriteNullTagLiteral(@"BindingList", @"");
                return;
            }
            TopLevelElement();
            Write1_KeyCommandListBinderImpl(@"BindingList", @"", ((Syncfusion.Shared.Utils.KeyBinding.Implementation.KeyProcessor.KeyCommandListBinderImpl)o), true, false);
        }
    }

    /// <summary>
    /// Key Command List Binder Impl Serialization reader
    /// </summary>
    public class KeyCommandListBinderImplSerializationReader : System.Xml.Serialization.XmlSerializationReader, Syncfusion.XmlSerializersCreator.IXmlSerializationReader {

        Syncfusion.Shared.Utils.KeyBinding.Implementation.KeyProcessor.KeyCommandListBinderImpl Read1_KeyCommandListBinderImpl(bool isNullable, bool checkType) {
            if (isNullable && ReadNull()) return null;
            if (checkType) {
                System.Xml.XmlQualifiedName t = GetXsiType();
                if (!(t == null || ((object) ((System.Xml.XmlQualifiedName)t).Name == (object)id1_KeyCommandListBinderImpl && (object) ((System.Xml.XmlQualifiedName)t).Namespace == (object)id2_Item)))                
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)t);
            }
            Syncfusion.Shared.Utils.KeyBinding.Implementation.KeyProcessor.KeyCommandListBinderImpl o = new Syncfusion.Shared.Utils.KeyBinding.Implementation.KeyProcessor.KeyCommandListBinderImpl();
            bool[] paramsRead = new bool[3];
            while (Reader.MoveToNextAttribute()) {
                if (!paramsRead[0] && ((object) Reader.LocalName == (object)id3_Command && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o.@CommandName = Reader.Value;
                    paramsRead[0] = true;
                }
                else if (!paramsRead[1] && ((object) Reader.LocalName == (object)id4_Key && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o.@KeyXML = Reader.Value;
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
                    if (((object) Reader.LocalName == (object)id5_Bindings && (object) Reader.NamespaceURI == (object)id2_Item)) {
                        if (!ReadNull()) {
                            Syncfusion.Shared.Utils.KeyBinding.Implementation.KeyProcessor.KeyCommandBinderImpl[] a_2_0 = null;
                            int ca_2_0 = 0;
                            if (Reader.IsEmptyElement) {
                                Reader.Skip();
                            }
                            else {
                                Reader.ReadStartElement();
                                Reader.MoveToContent();
                                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement) {
                                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                                        if (((object) Reader.LocalName == (object)id6_Binding && (object) Reader.NamespaceURI == (object)id2_Item)) {
                                            a_2_0 = (Syncfusion.Shared.Utils.KeyBinding.Implementation.KeyProcessor.KeyCommandBinderImpl[])EnsureArrayIndex(a_2_0, ca_2_0, typeof(Syncfusion.Shared.Utils.KeyBinding.Implementation.KeyProcessor.KeyCommandBinderImpl));a_2_0[ca_2_0++] = Read2_KeyCommandBinderImpl(true, true);
                                        }
                                        else if (((object) Reader.LocalName == (object)id7_BindingList && (object) Reader.NamespaceURI == (object)id2_Item)) {
                                            a_2_0 = (Syncfusion.Shared.Utils.KeyBinding.Implementation.KeyProcessor.KeyCommandBinderImpl[])EnsureArrayIndex(a_2_0, ca_2_0, typeof(Syncfusion.Shared.Utils.KeyBinding.Implementation.KeyProcessor.KeyCommandBinderImpl));a_2_0[ca_2_0++] = Read1_KeyCommandListBinderImpl(true, true);
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
                            o.@SubBundings = (Syncfusion.Shared.Utils.KeyBinding.Implementation.KeyProcessor.KeyCommandBinderImpl[])ShrinkArray(a_2_0, ca_2_0, typeof(Syncfusion.Shared.Utils.KeyBinding.Implementation.KeyProcessor.KeyCommandBinderImpl), false);
                        }
                        else {
                            Syncfusion.Shared.Utils.KeyBinding.Implementation.KeyProcessor.KeyCommandBinderImpl[] a_2_0 = null;
                            int ca_2_0 = 0;
                            o.@SubBundings = (Syncfusion.Shared.Utils.KeyBinding.Implementation.KeyProcessor.KeyCommandBinderImpl[])ShrinkArray(a_2_0, ca_2_0, typeof(Syncfusion.Shared.Utils.KeyBinding.Implementation.KeyProcessor.KeyCommandBinderImpl), true);
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

        Syncfusion.Shared.Utils.KeyBinding.Implementation.KeyProcessor.KeyCommandBinderImpl Read2_KeyCommandBinderImpl(bool isNullable, bool checkType) {
            if (isNullable && ReadNull()) return null;
            if (checkType) {
                System.Xml.XmlQualifiedName t = GetXsiType();
                if (!(t == null || ((object)((System.Xml.XmlQualifiedName)t).Name == (object)id8_KeyCommandBinderImpl && (object)((System.Xml.XmlQualifiedName)t).Namespace == (object)id2_Item)))
                {
                    if (((object)((System.Xml.XmlQualifiedName)t).Name == (object)id1_KeyCommandListBinderImpl && (object)((System.Xml.XmlQualifiedName)t).Namespace == (object)id2_Item))
                        return Read1_KeyCommandListBinderImpl(isNullable, false);
                    else
                        throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)t);
                }
            }
            Syncfusion.Shared.Utils.KeyBinding.Implementation.KeyProcessor.KeyCommandBinderImpl o = new Syncfusion.Shared.Utils.KeyBinding.Implementation.KeyProcessor.KeyCommandBinderImpl();
            bool[] paramsRead = new bool[2];
            while (Reader.MoveToNextAttribute()) {
                if (!paramsRead[0] && ((object) Reader.LocalName == (object)id3_Command && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o.@CommandName = Reader.Value;
                    paramsRead[0] = true;
                }
                else if (!paramsRead[1] && ((object) Reader.LocalName == (object)id4_Key && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o.@KeyXML = Reader.Value;
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

        System.Object Read3_Object(bool isNullable, bool checkType) {
            if (isNullable && ReadNull()) return null;
            if (checkType) {
                System.Xml.XmlQualifiedName t = GetXsiType();
                if (t == null)
                    return ReadTypedPrimitive(new System.Xml.XmlQualifiedName("anyType", "http://www.w3.org/2001/XMLSchema"));
                else if (((object) ((System.Xml.XmlQualifiedName)t).Name == (object)id8_KeyCommandBinderImpl && (object) ((System.Xml.XmlQualifiedName)t).Namespace == (object)id2_Item))
                    return Read2_KeyCommandBinderImpl(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)t).Name == (object)id1_KeyCommandListBinderImpl && (object) ((System.Xml.XmlQualifiedName)t).Namespace == (object)id2_Item))
                    return Read1_KeyCommandListBinderImpl(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)t).Name == (object)id9_ArrayOfChoice1 && (object) ((System.Xml.XmlQualifiedName)t).Namespace == (object)id2_Item)) {
                    Syncfusion.Shared.Utils.KeyBinding.Implementation.KeyProcessor.KeyCommandBinderImpl[] a = null;
                    if (!ReadNull()) {
                        Syncfusion.Shared.Utils.KeyBinding.Implementation.KeyProcessor.KeyCommandBinderImpl[] z_0_0 = null;
                        int cz_0_0 = 0;
                        if (Reader.IsEmptyElement) {
                            Reader.Skip();
                        }
                        else {
                            Reader.ReadStartElement();
                            Reader.MoveToContent();
                            while (Reader.NodeType != System.Xml.XmlNodeType.EndElement) {
                                if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                                    if (((object) Reader.LocalName == (object)id6_Binding && (object) Reader.NamespaceURI == (object)id2_Item)) {
                                        z_0_0 = (Syncfusion.Shared.Utils.KeyBinding.Implementation.KeyProcessor.KeyCommandBinderImpl[])EnsureArrayIndex(z_0_0, cz_0_0, typeof(Syncfusion.Shared.Utils.KeyBinding.Implementation.KeyProcessor.KeyCommandBinderImpl));z_0_0[cz_0_0++] = Read2_KeyCommandBinderImpl(true, true);
                                    }
                                    else if (((object) Reader.LocalName == (object)id7_BindingList && (object) Reader.NamespaceURI == (object)id2_Item)) {
                                        z_0_0 = (Syncfusion.Shared.Utils.KeyBinding.Implementation.KeyProcessor.KeyCommandBinderImpl[])EnsureArrayIndex(z_0_0, cz_0_0, typeof(Syncfusion.Shared.Utils.KeyBinding.Implementation.KeyProcessor.KeyCommandBinderImpl));z_0_0[cz_0_0++] = Read1_KeyCommandListBinderImpl(true, true);
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
                        a = (Syncfusion.Shared.Utils.KeyBinding.Implementation.KeyProcessor.KeyCommandBinderImpl[])ShrinkArray(z_0_0, cz_0_0, typeof(Syncfusion.Shared.Utils.KeyBinding.Implementation.KeyProcessor.KeyCommandBinderImpl), false);
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
        /// Reads serialized data from Xml for Key Command List Binder
        /// </summary>
        /// <returns>returns object</returns>
        public object ReadDataFromXml() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id7_BindingList && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read1_KeyCommandListBinderImpl(true, true);
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

        System.String id3_Command;
        System.String id7_BindingList;
        System.String id5_Bindings;
        System.String id2_Item;
        System.String id1_KeyCommandListBinderImpl;
        System.String id9_ArrayOfChoice1;
        System.String id4_Key;
        System.String id8_KeyCommandBinderImpl;
        System.String id6_Binding;
        /// <summary>
        /// Overrides the InitIDs
        /// </summary>
        protected override void InitIDs() {
            id3_Command = Reader.NameTable.Add(@"Command");
            id7_BindingList = Reader.NameTable.Add(@"BindingList");
            id5_Bindings = Reader.NameTable.Add(@"Bindings");
            id2_Item = Reader.NameTable.Add(@"");
            id1_KeyCommandListBinderImpl = Reader.NameTable.Add(@"KeyCommandListBinderImpl");
            id9_ArrayOfChoice1 = Reader.NameTable.Add(@"ArrayOfChoice1");
            id4_Key = Reader.NameTable.Add(@"Key");
            id8_KeyCommandBinderImpl = Reader.NameTable.Add(@"KeyCommandBinderImpl");
            id6_Binding = Reader.NameTable.Add(@"Binding");
        }
    }
}
