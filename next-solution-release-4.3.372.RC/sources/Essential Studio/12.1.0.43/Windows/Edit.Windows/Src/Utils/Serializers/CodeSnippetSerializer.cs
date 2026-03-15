#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Forms.Edit.Utils.Serializers {

    /// <summary>
    /// Code Snippet Serialization writer
    /// </summary>
    public class CodeSnippetSerializationWriter : System.Xml.Serialization.XmlSerializationWriter, Syncfusion.XmlSerializersCreator.IXmlSerializationWriter {
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
                WriteNullTagLiteral(@"CodeSnippet", @"");
                return;
            }
            TopLevelElement();
            WriteSerializable(((Syncfusion.Windows.Forms.Edit.Utils.CodeSnippets.CodeSnippet)o), @"CodeSnippet", @"", true);
        }
    }

    /// <summary>
    /// Code Snippet Serialization reader
    /// </summary>
    public class CodeSnippetSerializationReader : System.Xml.Serialization.XmlSerializationReader, Syncfusion.XmlSerializersCreator.IXmlSerializationReader {
        /// <summary>
        /// Overrides the InitCallbacks
        /// </summary>
        protected override void InitCallbacks() {
        }

        /// <summary>
        /// Reads serialized data from Xml for Code Snippet
        /// </summary>
        /// <returns>returns  object</returns>
        public object ReadDataFromXml() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id1_CodeSnippet && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = (Syncfusion.Windows.Forms.Edit.Utils.CodeSnippets.CodeSnippet)ReadSerializable(new Syncfusion.Windows.Forms.Edit.Utils.CodeSnippets.CodeSnippet());
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

        System.String id1_CodeSnippet;
        System.String id2_Item;
        /// <summary>
        /// Overrides the InitIDs
        /// </summary>
        protected override void InitIDs() {
            id1_CodeSnippet = Reader.NameTable.Add(@"CodeSnippet");
            id2_Item = Reader.NameTable.Add(@"");
        }
    }
}
