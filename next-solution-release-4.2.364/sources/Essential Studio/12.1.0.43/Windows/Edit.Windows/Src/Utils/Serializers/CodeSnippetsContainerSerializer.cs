#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Forms.Edit.Utils.Serializers {

    /// <summary>
    /// Code Snippets Container Serialization Writer
    /// </summary>
    public class CodeSnippetsContainerSerializationWriter : System.Xml.Serialization.XmlSerializationWriter, Syncfusion.XmlSerializersCreator.IXmlSerializationWriter {
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
                WriteNullTagLiteral(@"CodeSnippetsContainer", @"");
                return;
            }
            TopLevelElement();
            WriteSerializable(((Syncfusion.Windows.Forms.Edit.Utils.CodeSnippets.CodeSnippetsContainer)o), @"CodeSnippetsContainer", @"", true);
        }
    }

    /// <summary>
    /// Code Snippets Container Serialization reader
    /// </summary>
    public class CodeSnippetsContainerSerializationReader : System.Xml.Serialization.XmlSerializationReader, Syncfusion.XmlSerializersCreator.IXmlSerializationReader {
        /// <summary>
        /// Overrides the InitCallbacks
        /// </summary>
        protected override void InitCallbacks() {
        }

        /// <summary>
        /// Reads serialized data from Xml for Code Snippets Container
        /// </summary>
        /// <returns>returns object</returns>
        public object ReadDataFromXml() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id1_CodeSnippetsContainer && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = (Syncfusion.Windows.Forms.Edit.Utils.CodeSnippets.CodeSnippetsContainer)ReadSerializable(new Syncfusion.Windows.Forms.Edit.Utils.CodeSnippets.CodeSnippetsContainer());
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

        System.String id1_CodeSnippetsContainer;
        System.String id2_Item;
        /// <summary>
        /// Overrides the InitIDs
        /// </summary>
        protected override void InitIDs() {
            id1_CodeSnippetsContainer = Reader.NameTable.Add(@"CodeSnippetsContainer");
            id2_Item = Reader.NameTable.Add(@"");
        }
    }
}
