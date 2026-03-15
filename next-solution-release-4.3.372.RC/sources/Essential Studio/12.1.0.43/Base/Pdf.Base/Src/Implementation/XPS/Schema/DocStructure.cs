#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.XPS {
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/xps/2005/06/documentstructure")]
    [System.Xml.Serialization.XmlRootAttribute("DocumentStructure", Namespace="http://schemas.microsoft.com/xps/2005/06/documentstructure", IsNullable=false)]
    public partial class DocumentStructure {
        
        private Outline documentStructureOutlineField;
        
        private Story[] storyField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("DocumentStructure.Outline")]
        public Outline DocumentStructureOutline {
            get {
                return this.documentStructureOutlineField;
            }
            set {
                this.documentStructureOutlineField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Story")]
        public Story[] Story {
            get {
                return this.storyField;
            }
            set {
                this.storyField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/xps/2005/06/documentstructure")]
    [System.Xml.Serialization.XmlRootAttribute("DocumentStructure.Outline", Namespace="http://schemas.microsoft.com/xps/2005/06/documentstructure", IsNullable=false)]
    public partial class Outline {
        
        private DocumentOutline documentOutlineField;
        
        /// <remarks/>
        public DocumentOutline DocumentOutline {
            get {
                return this.documentOutlineField;
            }
            set {
                this.documentOutlineField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/xps/2005/06/documentstructure")]
    [System.Xml.Serialization.XmlRootAttribute("DocumentOutline", Namespace="http://schemas.microsoft.com/xps/2005/06/documentstructure", IsNullable=false)]
    public partial class DocumentOutline {
        
        private OutlineEntry[] outlineEntryField;
        
        private string langField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("OutlineEntry")]
        public OutlineEntry[] OutlineEntry {
            get {
                return this.outlineEntryField;
            }
            set {
                this.outlineEntryField = value;
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
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/xps/2005/06/documentstructure")]
    [System.Xml.Serialization.XmlRootAttribute("OutlineEntry", Namespace="http://schemas.microsoft.com/xps/2005/06/documentstructure", IsNullable=false)]
    public partial class OutlineEntry {
        
        private int outlineLevelField;
        
        private string outlineTargetField;
        
        private string descriptionField;
        
        private string langField;
        
        public OutlineEntry() {
            this.outlineLevelField = 1;
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(1)]
        public int OutlineLevel {
            get {
                return this.outlineLevelField;
            }
            set {
                this.outlineLevelField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="anyURI")]
        public string OutlineTarget {
            get {
                return this.outlineTargetField;
            }
            set {
                this.outlineTargetField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Description {
            get {
                return this.descriptionField;
            }
            set {
                this.descriptionField = value;
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
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/xps/2005/06/documentstructure")]
    [System.Xml.Serialization.XmlRootAttribute("Story", Namespace="http://schemas.microsoft.com/xps/2005/06/documentstructure", IsNullable=false)]
    public partial class Story {
        
        private StoryFragmentReference[] storyFragmentReferenceField;
        
        private string storyNameField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("StoryFragmentReference")]
        public StoryFragmentReference[] StoryFragmentReference {
            get {
                return this.storyFragmentReferenceField;
            }
            set {
                this.storyFragmentReferenceField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string StoryName {
            get {
                return this.storyNameField;
            }
            set {
                this.storyNameField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/xps/2005/06/documentstructure")]
    [System.Xml.Serialization.XmlRootAttribute("StoryFragmentReference", Namespace="http://schemas.microsoft.com/xps/2005/06/documentstructure", IsNullable=false)]
    public partial class StoryFragmentReference {
        
        private string fragmentNameField;
        
        private int pageField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string FragmentName {
            get {
                return this.fragmentNameField;
            }
            set {
                this.fragmentNameField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int Page {
            get {
                return this.pageField;
            }
            set {
                this.pageField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/xps/2005/06/documentstructure")]
    [System.Xml.Serialization.XmlRootAttribute("StoryFragments", Namespace="http://schemas.microsoft.com/xps/2005/06/documentstructure", IsNullable=false)]
    public partial class StoryFragments {
        
        private StoryFragment[] storyFragmentField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("StoryFragment")]
        public StoryFragment[] StoryFragment {
            get {
                return this.storyFragmentField;
            }
            set {
                this.storyFragmentField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/xps/2005/06/documentstructure")]
    [System.Xml.Serialization.XmlRootAttribute("StoryFragment", Namespace="http://schemas.microsoft.com/xps/2005/06/documentstructure", IsNullable=false)]
    public partial class StoryFragment {
        
        private Break storyBreakField;
        
        private object[] itemsField;
        
        private Break storyBreak1Field;
        
        private string storyNameField;
        
        private string fragmentNameField;
        
        private FragmentType fragmentTypeField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=0)]
        public Break StoryBreak {
            get {
                return this.storyBreakField;
            }
            set {
                this.storyBreakField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("FigureStructure", typeof(Figure), Order=1)]
        [System.Xml.Serialization.XmlElementAttribute("ListStructure", typeof(List), Order=1)]
        [System.Xml.Serialization.XmlElementAttribute("ParagraphStructure", typeof(Paragraph), Order=1)]
        [System.Xml.Serialization.XmlElementAttribute("SectionStructure", typeof(Section), Order=1)]
        [System.Xml.Serialization.XmlElementAttribute("TableStructure", typeof(Table), Order=1)]
        public object[] Items {
            get {
                return this.itemsField;
            }
            set {
                this.itemsField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("StoryBreak", Order=2)]
        public Break StoryBreak1 {
            get {
                return this.storyBreak1Field;
            }
            set {
                this.storyBreak1Field = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string StoryName {
            get {
                return this.storyNameField;
            }
            set {
                this.storyNameField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string FragmentName {
            get {
                return this.fragmentNameField;
            }
            set {
                this.fragmentNameField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public FragmentType FragmentType {
            get {
                return this.fragmentTypeField;
            }
            set {
                this.fragmentTypeField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/xps/2005/06/documentstructure")]
    [System.Xml.Serialization.XmlRootAttribute("StoryBreak", Namespace="http://schemas.microsoft.com/xps/2005/06/documentstructure", IsNullable=false)]
    public partial class Break {
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/xps/2005/06/documentstructure")]
    [System.Xml.Serialization.XmlRootAttribute("FigureStructure", Namespace="http://schemas.microsoft.com/xps/2005/06/documentstructure", IsNullable=false)]
    public partial class Figure {
        
        private NamedElement[] itemsField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("NamedElement")]
        public NamedElement[] Items {
            get {
                return this.itemsField;
            }
            set {
                this.itemsField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/xps/2005/06/documentstructure")]
    [System.Xml.Serialization.XmlRootAttribute("NamedElement", Namespace="http://schemas.microsoft.com/xps/2005/06/documentstructure", IsNullable=false)]
    public partial class NamedElement {
        
        private string nameReferenceField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string NameReference {
            get {
                return this.nameReferenceField;
            }
            set {
                this.nameReferenceField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/xps/2005/06/documentstructure")]
    [System.Xml.Serialization.XmlRootAttribute("ListStructure", Namespace="http://schemas.microsoft.com/xps/2005/06/documentstructure", IsNullable=false)]
    public partial class List {
        
        private ListItem[] itemsField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ListItemStructure")]
        public ListItem[] Items {
            get {
                return this.itemsField;
            }
            set {
                this.itemsField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/xps/2005/06/documentstructure")]
    [System.Xml.Serialization.XmlRootAttribute("ListItemStructure", Namespace="http://schemas.microsoft.com/xps/2005/06/documentstructure", IsNullable=false)]
    public partial class ListItem {
        
        private object[] itemsField;
        
        private string markerField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("FigureStructure", typeof(Figure))]
        [System.Xml.Serialization.XmlElementAttribute("ListStructure", typeof(List))]
        [System.Xml.Serialization.XmlElementAttribute("ParagraphStructure", typeof(Paragraph))]
        [System.Xml.Serialization.XmlElementAttribute("TableStructure", typeof(Table))]
        public object[] Items {
            get {
                return this.itemsField;
            }
            set {
                this.itemsField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="ID")]
        public string Marker {
            get {
                return this.markerField;
            }
            set {
                this.markerField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/xps/2005/06/documentstructure")]
    [System.Xml.Serialization.XmlRootAttribute("ParagraphStructure", Namespace="http://schemas.microsoft.com/xps/2005/06/documentstructure", IsNullable=false)]
    public partial class Paragraph {
        
        private NamedElement[] itemsField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("NamedElement")]
        public NamedElement[] Items {
            get {
                return this.itemsField;
            }
            set {
                this.itemsField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/xps/2005/06/documentstructure")]
    [System.Xml.Serialization.XmlRootAttribute("TableStructure", Namespace="http://schemas.microsoft.com/xps/2005/06/documentstructure", IsNullable=false)]
    public partial class Table {
        
        private TableRowGroup[] itemsField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("TableRowGroupStructure")]
        public TableRowGroup[] Items {
            get {
                return this.itemsField;
            }
            set {
                this.itemsField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/xps/2005/06/documentstructure")]
    [System.Xml.Serialization.XmlRootAttribute("TableRowGroupStructure", Namespace="http://schemas.microsoft.com/xps/2005/06/documentstructure", IsNullable=false)]
    public partial class TableRowGroup {
        
        private TableRow[] itemsField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("TableRowStructure")]
        public TableRow[] Items {
            get {
                return this.itemsField;
            }
            set {
                this.itemsField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/xps/2005/06/documentstructure")]
    [System.Xml.Serialization.XmlRootAttribute("TableRowStructure", Namespace="http://schemas.microsoft.com/xps/2005/06/documentstructure", IsNullable=false)]
    public partial class TableRow {
        
        private TableCell[] itemsField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("TableCellStructure")]
        public TableCell[] Items {
            get {
                return this.itemsField;
            }
            set {
                this.itemsField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/xps/2005/06/documentstructure")]
    [System.Xml.Serialization.XmlRootAttribute("TableCellStructure", Namespace="http://schemas.microsoft.com/xps/2005/06/documentstructure", IsNullable=false)]
    public partial class TableCell {
        
        private object[] itemsField;
        
        private ItemsChoiceType[] itemsElementNameField;
        
        private int rowSpanField;
        
        private int columnSpanField;
        
        public TableCell() {
            this.rowSpanField = 1;
            this.columnSpanField = 1;
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("FigureStructure", typeof(Figure))]
        [System.Xml.Serialization.XmlElementAttribute("ListStructure", typeof(List))]
        [System.Xml.Serialization.XmlElementAttribute("ParagraphStructure", typeof(Paragraph))]
        [System.Xml.Serialization.XmlElementAttribute("TableStructure", typeof(Table))]
        [System.Xml.Serialization.XmlChoiceIdentifierAttribute("ItemsElementName")]
        public object[] Items {
            get {
                return this.itemsField;
            }
            set {
                this.itemsField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ItemsElementName")]
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public ItemsChoiceType[] ItemsElementName {
            get {
                return this.itemsElementNameField;
            }
            set {
                this.itemsElementNameField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(1)]
        public int RowSpan {
            get {
                return this.rowSpanField;
            }
            set {
                this.rowSpanField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(1)]
        public int ColumnSpan {
            get {
                return this.columnSpanField;
            }
            set {
                this.columnSpanField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/xps/2005/06/documentstructure", IncludeInSchema=false)]
    public enum ItemsChoiceType {
        
        /// <remarks/>
        FigureStructure,
        
        /// <remarks/>
        ListStructure,
        
        /// <remarks/>
        ParagraphStructure,
        
        /// <remarks/>
        TableStructure,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/xps/2005/06/documentstructure")]
    [System.Xml.Serialization.XmlRootAttribute("SectionStructure", Namespace="http://schemas.microsoft.com/xps/2005/06/documentstructure", IsNullable=false)]
    public partial class Section {
        
        private object[] itemsField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("FigureStructure", typeof(Figure))]
        [System.Xml.Serialization.XmlElementAttribute("ListStructure", typeof(List))]
        [System.Xml.Serialization.XmlElementAttribute("ParagraphStructure", typeof(Paragraph))]
        [System.Xml.Serialization.XmlElementAttribute("TableStructure", typeof(Table))]
        public object[] Items {
            get {
                return this.itemsField;
            }
            set {
                this.itemsField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/xps/2005/06/documentstructure")]
    public enum FragmentType {
        
        /// <remarks/>
        Content,
        
        /// <remarks/>
        Header,
        
        /// <remarks/>
        Footer,
    }
}
