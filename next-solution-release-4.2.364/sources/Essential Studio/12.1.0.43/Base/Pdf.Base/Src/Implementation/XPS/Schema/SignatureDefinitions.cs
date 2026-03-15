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
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/xps/2005/06/signature-definitions")]
    [System.Xml.Serialization.XmlRootAttribute("SignatureDefinitions", Namespace="http://schemas.microsoft.com/xps/2005/06/signature-definitions", IsNullable=false)]
    public partial class SignatureDefinitionsType {
        
        private SignatureDefinitionType[] signatureDefinitionField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("SignatureDefinition")]
        public SignatureDefinitionType[] SignatureDefinition {
            get {
                return this.signatureDefinitionField;
            }
            set {
                this.signatureDefinitionField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/xps/2005/06/signature-definitions")]
    public partial class SignatureDefinitionType {
        
        private SpotLocationType spotLocationField;
        
        private string intentField;
        
        private System.DateTime signByField;
        
        private bool signByFieldSpecified;
        
        private string signingLocationField;
        
        private string spotIDField;
        
        private string signerNameField;
        
        private string langField;
        
        /// <remarks/>
        public SpotLocationType SpotLocation {
            get {
                return this.spotLocationField;
            }
            set {
                this.spotLocationField = value;
            }
        }
        
        /// <remarks/>
        public string Intent {
            get {
                return this.intentField;
            }
            set {
                this.intentField = value;
            }
        }
        
        /// <remarks/>
        public System.DateTime SignBy {
            get {
                return this.signByField;
            }
            set {
                this.signByField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool SignBySpecified {
            get {
                return this.signByFieldSpecified;
            }
            set {
                this.signByFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        public string SigningLocation {
            get {
                return this.signingLocationField;
            }
            set {
                this.signingLocationField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="ID")]
        public string SpotID {
            get {
                return this.spotIDField;
            }
            set {
                this.spotIDField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string SignerName {
            get {
                return this.signerNameField;
            }
            set {
                this.signerNameField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form=System.Xml.Schema.XmlSchemaForm.Qualified, Namespace="http://www.w3.org/XML/1998/namespace")]
        public string lang {
            get {
                return this.langField;
            }
            set {
                this.langField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/xps/2005/06/signature-definitions")]
    public partial class SpotLocationType {
        
        private string pageURIField;
        
        private double startXField;
        
        private double startYField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="anyURI")]
        public string PageURI {
            get {
                return this.pageURIField;
            }
            set {
                this.pageURIField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public double StartX {
            get {
                return this.startXField;
            }
            set {
                this.startXField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public double StartY {
            get {
                return this.startYField;
            }
            set {
                this.startYField = value;
            }
        }
    }
}
