#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Forms.Edit.Utils.Serializers {

    /// <summary>
    /// Config Language Serialization Writer
    /// </summary>
    public class ConfigLanguageSerializationWriter : System.Xml.Serialization.XmlSerializationWriter, Syncfusion.XmlSerializersCreator.IXmlSerializationWriter {
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
                WriteNullTagLiteral(@"ConfigLanguage", @"");
                return;
            }
            TopLevelElement();
            WriteSerializable(((Syncfusion.Windows.Forms.Edit.Implementation.Config.ConfigLanguage)o), @"ConfigLanguage", @"", true);
        }
    }

    /// <summary>
    /// Config Language Serialization reader.
    /// </summary>
    public class ConfigLanguageSerializationReader : System.Xml.Serialization.XmlSerializationReader, Syncfusion.XmlSerializersCreator.IXmlSerializationReader {
        /// <summary>
        /// Overrides the InitCallbacks
        /// </summary>
        protected override void InitCallbacks() {
        }

        /// <summary>
        /// Reads serialized data from Xml for Config Language
        /// </summary>
        /// <returns>returns object</returns>
        public object ReadDataFromXml() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id1_ConfigLanguage && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = (Syncfusion.Windows.Forms.Edit.Implementation.Config.ConfigLanguage)ReadSerializable(new Syncfusion.Windows.Forms.Edit.Implementation.Config.ConfigLanguage());
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

        System.String id2_Item;
        System.String id1_ConfigLanguage;
        /// <summary>
        /// Overrides the InitIDs
        /// </summary>
        protected override void InitIDs() {
            id2_Item = Reader.NameTable.Add(@"");
            id1_ConfigLanguage = Reader.NameTable.Add(@"ConfigLanguage");
        }
    }
}
