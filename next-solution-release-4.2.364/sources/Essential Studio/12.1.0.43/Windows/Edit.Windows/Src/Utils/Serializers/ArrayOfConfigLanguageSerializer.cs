#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Forms.Edit.Utils.Serializers {

    /// <summary>
    /// Array of Config Language Serialization Writer
    /// </summary>
    public class ArrayOfConfigLanguageSerializationWriter : System.Xml.Serialization.XmlSerializationWriter, Syncfusion.XmlSerializersCreator.IXmlSerializationWriter {
        /// <summary>
        /// Overrides the InitCallbacks
        /// </summary>
        protected override void InitCallbacks() {
        }

        /// <summary>
        /// Serializes given object to xml.
        /// </summary>
        /// <param name="o">The object to serialize</param>
        public void WriteDataToXml(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteNullTagLiteral(@"ArrayOfConfigLanguage", @"");
                return;
            }
            TopLevelElement();
            {
                Syncfusion.Windows.Forms.Edit.Implementation.Config.ConfigLanguage[] a = (Syncfusion.Windows.Forms.Edit.Implementation.Config.ConfigLanguage[])((Syncfusion.Windows.Forms.Edit.Implementation.Config.ConfigLanguage[])o);
                if ((object)(a) == null) {
                    WriteNullTagLiteral(@"ArrayOfConfigLanguage", @"");
                }
                else {
                    WriteStartElement(@"ArrayOfConfigLanguage", @"");
                    for (int ia = 0; ia < a.Length; ia++) {
                        WriteSerializable(((Syncfusion.Windows.Forms.Edit.Implementation.Config.ConfigLanguage)a[ia]), @"ConfigLanguage", @"", true);
                    }
                    WriteEndElement();
                }
            }
        }
    }

    /// <summary>
    /// Array of Config Language Serialization reader
    /// </summary>
    public class ArrayOfConfigLanguageSerializationReader : System.Xml.Serialization.XmlSerializationReader, Syncfusion.XmlSerializersCreator.IXmlSerializationReader {
        /// <summary>
        /// Overrides the InitCallbacks
        /// </summary>
        protected override void InitCallbacks() {
        }

        /// <summary>
        /// Reads serialized data from Xml for Array of Config Language
        /// </summary>
        /// <returns>returns object</returns>
        public object ReadDataFromXml() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id1_ArrayOfConfigLanguage && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    if (!ReadNull()) {
                        Syncfusion.Windows.Forms.Edit.Implementation.Config.ConfigLanguage[] a_0_0 = null;
                        int ca_0_0 = 0;
                        if (Reader.IsEmptyElement) {
                            Reader.Skip();
                        }
                        else {
                            Reader.ReadStartElement();
                            Reader.MoveToContent();
                            while (Reader.NodeType != System.Xml.XmlNodeType.EndElement) {
                                if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                                    if (((object) Reader.LocalName == (object)id3_ConfigLanguage && (object) Reader.NamespaceURI == (object)id2_Item)) {
                                        a_0_0 = (Syncfusion.Windows.Forms.Edit.Implementation.Config.ConfigLanguage[])EnsureArrayIndex(a_0_0, ca_0_0, typeof(Syncfusion.Windows.Forms.Edit.Implementation.Config.ConfigLanguage));a_0_0[ca_0_0++] = (Syncfusion.Windows.Forms.Edit.Implementation.Config.ConfigLanguage)ReadSerializable(new Syncfusion.Windows.Forms.Edit.Implementation.Config.ConfigLanguage());
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
                        o = (Syncfusion.Windows.Forms.Edit.Implementation.Config.ConfigLanguage[])ShrinkArray(a_0_0, ca_0_0, typeof(Syncfusion.Windows.Forms.Edit.Implementation.Config.ConfigLanguage), false);
                    }
                    else {
                        Syncfusion.Windows.Forms.Edit.Implementation.Config.ConfigLanguage[] a_0_0 = null;
                        int ca_0_0 = 0;
                        o = (Syncfusion.Windows.Forms.Edit.Implementation.Config.ConfigLanguage[])ShrinkArray(a_0_0, ca_0_0, typeof(Syncfusion.Windows.Forms.Edit.Implementation.Config.ConfigLanguage), true);
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
        /// <summary>
        /// defines the Item
        /// </summary>
        System.String id2_Item;
        /// <summary>
        /// defines the array of config languages
        /// </summary>
        System.String id1_ArrayOfConfigLanguage;
        /// <summary>
        /// Defines the Config langugae
        /// </summary>
        System.String id3_ConfigLanguage;
        /// <summary>
        /// Overrides the InitIds
        /// </summary>
        protected override void InitIDs() {
            id2_Item = Reader.NameTable.Add(@"");
            id1_ArrayOfConfigLanguage = Reader.NameTable.Add(@"ArrayOfConfigLanguage");
            id3_ConfigLanguage = Reader.NameTable.Add(@"ConfigLanguage");
        }
    }
}
