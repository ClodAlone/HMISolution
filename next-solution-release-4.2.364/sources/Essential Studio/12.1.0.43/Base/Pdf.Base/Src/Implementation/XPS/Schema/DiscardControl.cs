#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.XPS{
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/xps/2005/06/discard-control")]
    [System.Xml.Serialization.XmlRootAttribute("DiscardControl", Namespace="http://schemas.microsoft.com/xps/2005/06/discard-control", IsNullable=false)]
    public partial class DiscardControl {
        
        private Discard[] discardField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Discard")]
        public Discard[] Discard {
            get {
                return this.discardField;
            }
            set {
                this.discardField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/xps/2005/06/discard-control")]
    [System.Xml.Serialization.XmlRootAttribute("Discard", Namespace="http://schemas.microsoft.com/xps/2005/06/discard-control", IsNullable=false)]
    public partial class Discard {
        
        private string sentinelPageField;
        
        private string targetField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="anyURI")]
        public string SentinelPage {
            get {
                return this.sentinelPageField;
            }
            set {
                this.sentinelPageField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="anyURI")]
        public string Target {
            get {
                return this.targetField;
            }
            set {
                this.targetField = value;
            }
        }
    }
}
