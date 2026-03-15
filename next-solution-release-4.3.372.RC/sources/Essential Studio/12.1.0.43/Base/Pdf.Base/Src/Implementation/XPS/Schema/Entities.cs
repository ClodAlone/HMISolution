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
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/xps/2005/06")]
    [System.Xml.Serialization.XmlRootAttribute("MatrixTransform", Namespace="http://schemas.microsoft.com/xps/2005/06", IsNullable=false)]
    public partial class MatrixTransform {
        
        private string matrixField;
        
        private string keyField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Matrix {
            get {
                return this.matrixField;
            }
            set {
                this.matrixField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form=System.Xml.Schema.XmlSchemaForm.Qualified, Namespace="http://schemas.microsoft.com/xps/2005/06/resourcedictionary-key")]
        public string Key {
            get {
                return this.keyField;
            }
            set {
                this.keyField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/xps/2005/06")]
    [System.Xml.Serialization.XmlRootAttribute("SolidColorBrush", Namespace="http://schemas.microsoft.com/xps/2005/06", IsNullable=false)]
    public partial class SolidColorBrush {
        
        private double opacityField;
        
        private string keyField;
        
        private string colorField;
        
        public SolidColorBrush() {
            this.opacityField = 1;
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(1)]
        public double Opacity {
            get {
                return this.opacityField;
            }
            set {
                this.opacityField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form=System.Xml.Schema.XmlSchemaForm.Qualified, Namespace="http://schemas.microsoft.com/xps/2005/06/resourcedictionary-key")]
        public string Key {
            get {
                return this.keyField;
            }
            set {
                this.keyField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Color {
            get {
                return this.colorField;
            }
            set {
                this.colorField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/xps/2005/06")]
    [System.Xml.Serialization.XmlRootAttribute("ImageBrush", Namespace="http://schemas.microsoft.com/xps/2005/06", IsNullable=false)]
    public partial class ImageBrush {
        
        private Transform imageBrushTransformField;
        
        private double opacityField;
        
        private string keyField;
        
        private string transformField;
        
        private string viewboxField;
        
        private string viewportField;
        
        private TileMode tileModeField;
        
        private ViewUnits viewboxUnitsField;
        
        private ViewUnits viewportUnitsField;
        
        private string imageSourceField;
        
        public ImageBrush() {
            this.opacityField = 1;
            this.tileModeField = TileMode.None;
            this.viewboxUnitsField = ViewUnits.Absolute;
            this.viewportUnitsField = ViewUnits.Absolute;
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ImageBrush.Transform")]
        public Transform ImageBrushTransform {
            get {
                return this.imageBrushTransformField;
            }
            set {
                this.imageBrushTransformField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(1)]
        public double Opacity {
            get {
                return this.opacityField;
            }
            set {
                this.opacityField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form=System.Xml.Schema.XmlSchemaForm.Qualified, Namespace="http://schemas.microsoft.com/xps/2005/06/resourcedictionary-key")]
        public string Key {
            get {
                return this.keyField;
            }
            set {
                this.keyField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Transform {
            get {
                return this.transformField;
            }
            set {
                this.transformField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Viewbox {
            get {
                return this.viewboxField;
            }
            set {
                this.viewboxField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Viewport {
            get {
                return this.viewportField;
            }
            set {
                this.viewportField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(TileMode.None)]
        public TileMode TileMode {
            get {
                return this.tileModeField;
            }
            set {
                this.tileModeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public ViewUnits ViewboxUnits {
            get {
                return this.viewboxUnitsField;
            }
            set {
                this.viewboxUnitsField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public ViewUnits ViewportUnits {
            get {
                return this.viewportUnitsField;
            }
            set {
                this.viewportUnitsField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ImageSource {
            get {
                return this.imageSourceField;
            }
            set {
                this.imageSourceField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/xps/2005/06")]
    [System.Xml.Serialization.XmlRootAttribute("ImageBrush.Transform", Namespace="http://schemas.microsoft.com/xps/2005/06", IsNullable=false)]
    public partial class Transform {
        
        private MatrixTransform matrixTransformField;
        
        /// <remarks/>
        public MatrixTransform MatrixTransform {
            get {
                return this.matrixTransformField;
            }
            set {
                this.matrixTransformField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/xps/2005/06")]
    public enum TileMode {
        
        /// <remarks/>
        None,
        
        /// <remarks/>
        Tile,
        
        /// <remarks/>
        FlipX,
        
        /// <remarks/>
        FlipY,
        
        /// <remarks/>
        FlipXY,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/xps/2005/06")]
    public enum ViewUnits {
        
        /// <remarks/>
        Absolute,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/xps/2005/06")]
    [System.Xml.Serialization.XmlRootAttribute("VisualBrush", Namespace="http://schemas.microsoft.com/xps/2005/06", IsNullable=false)]
    public partial class VisualBrush {
        
        private Transform visualBrushTransformField;
        
        private Visual visualBrushVisualField;
        
        private double opacityField;
        
        private string keyField;
        
        private string transformField;
        
        private string viewboxField;
        
        private string viewportField;
        
        private TileMode tileModeField;
        
        private ViewUnits viewboxUnitsField;
        
        private ViewUnits viewportUnitsField;
        
        private string visualField;
        
        public VisualBrush() {
            this.opacityField = 1;
            this.tileModeField = TileMode.None;
            this.viewboxUnitsField = ViewUnits.Absolute;
            this.viewportUnitsField = ViewUnits.Absolute;
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("VisualBrush.Transform")]
        public Transform VisualBrushTransform {
            get {
                return this.visualBrushTransformField;
            }
            set {
                this.visualBrushTransformField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("VisualBrush.Visual")]
        public Visual VisualBrushVisual {
            get {
                return this.visualBrushVisualField;
            }
            set {
                this.visualBrushVisualField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(1)]
        public double Opacity {
            get {
                return this.opacityField;
            }
            set {
                this.opacityField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form=System.Xml.Schema.XmlSchemaForm.Qualified, Namespace="http://schemas.microsoft.com/xps/2005/06/resourcedictionary-key")]
        public string Key {
            get {
                return this.keyField;
            }
            set {
                this.keyField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Transform {
            get {
                return this.transformField;
            }
            set {
                this.transformField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Viewbox {
            get {
                return this.viewboxField;
            }
            set {
                this.viewboxField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Viewport {
            get {
                return this.viewportField;
            }
            set {
                this.viewportField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(TileMode.None)]
        public TileMode TileMode {
            get {
                return this.tileModeField;
            }
            set {
                this.tileModeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public ViewUnits ViewboxUnits {
            get {
                return this.viewboxUnitsField;
            }
            set {
                this.viewboxUnitsField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public ViewUnits ViewportUnits {
            get {
                return this.viewportUnitsField;
            }
            set {
                this.viewportUnitsField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Visual {
            get {
                return this.visualField;
            }
            set {
                this.visualField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/xps/2005/06")]
    [System.Xml.Serialization.XmlRootAttribute("VisualBrush.Visual", Namespace="http://schemas.microsoft.com/xps/2005/06", IsNullable=false)]
    public partial class Visual {
        
        private object itemField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Canvas", typeof(Canvas))]
        [System.Xml.Serialization.XmlElementAttribute("Glyphs", typeof(Glyphs))]
        [System.Xml.Serialization.XmlElementAttribute("Path", typeof(Path))]
        public object Item {
            get {
                return this.itemField;
            }
            set {
                this.itemField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/xps/2005/06")]
    [System.Xml.Serialization.XmlRootAttribute("Canvas", Namespace="http://schemas.microsoft.com/xps/2005/06", IsNullable=false)]
    public partial class Canvas {
        
        private Resources canvasResourcesField;
        
        private Transform canvasRenderTransformField;
        
        private Geometry canvasClipField;
        
        private Brush canvasOpacityMaskField;
        
        private object[] itemsField;
        
        private string renderTransformField;
        
        private string clipField;
        
        private double opacityField;
        
        private string opacityMaskField;
        
        private string nameField;
        
        private EdgeMode renderOptionsEdgeModeField;
        
        private bool renderOptionsEdgeModeFieldSpecified;
        
        private string fixedPageNavigateUriField;
        
        private string langField;
        
        private string keyField;
        
        private string automationPropertiesNameField;
        
        private string automationPropertiesHelpTextField;

        internal object m_parent;
        
        public Canvas() {
            this.opacityField = 1;
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Canvas.Resources")]
        public Resources CanvasResources {
            get {
                return this.canvasResourcesField;
            }
            set {
                this.canvasResourcesField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Canvas.RenderTransform")]
        public Transform CanvasRenderTransform {
            get {
                return this.canvasRenderTransformField;
            }
            set {
                this.canvasRenderTransformField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Canvas.Clip")]
        public Geometry CanvasClip {
            get {
                return this.canvasClipField;
            }
            set {
                this.canvasClipField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Canvas.OpacityMask")]
        public Brush CanvasOpacityMask {
            get {
                return this.canvasOpacityMaskField;
            }
            set {
                this.canvasOpacityMaskField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Canvas", typeof(Canvas))]
        [System.Xml.Serialization.XmlElementAttribute("Glyphs", typeof(Glyphs))]
        [System.Xml.Serialization.XmlElementAttribute("Path", typeof(Path))]
        public object[] Items {
            get {
                return this.itemsField;
            }
            set {
                this.itemsField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string RenderTransform {
            get {
                return this.renderTransformField;
            }
            set {
                this.renderTransformField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Clip {
            get {
                return this.clipField;
            }
            set {
                this.clipField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(1)]
        public double Opacity {
            get {
                return this.opacityField;
            }
            set {
                this.opacityField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string OpacityMask {
            get {
                return this.opacityMaskField;
            }
            set {
                this.opacityMaskField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="ID")]
        public string Name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("RenderOptions.EdgeMode")]
        public EdgeMode RenderOptionsEdgeMode {
            get {
                return this.renderOptionsEdgeModeField;
            }
            set {
                this.renderOptionsEdgeModeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool RenderOptionsEdgeModeSpecified {
            get {
                return this.renderOptionsEdgeModeFieldSpecified;
            }
            set {
                this.renderOptionsEdgeModeFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("FixedPage.NavigateUri", DataType="anyURI")]
        public string FixedPageNavigateUri {
            get {
                return this.fixedPageNavigateUriField;
            }
            set {
                this.fixedPageNavigateUriField = value;
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
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form=System.Xml.Schema.XmlSchemaForm.Qualified, Namespace="http://schemas.microsoft.com/xps/2005/06/resourcedictionary-key")]
        public string Key {
            get {
                return this.keyField;
            }
            set {
                this.keyField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("AutomationProperties.Name")]
        public string AutomationPropertiesName {
            get {
                return this.automationPropertiesNameField;
            }
            set {
                this.automationPropertiesNameField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("AutomationProperties.HelpText")]
        public string AutomationPropertiesHelpText {
            get {
                return this.automationPropertiesHelpTextField;
            }
            set {
                this.automationPropertiesHelpTextField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/xps/2005/06")]
    [System.Xml.Serialization.XmlRootAttribute("Canvas.Resources", Namespace="http://schemas.microsoft.com/xps/2005/06", IsNullable=false)]
    public partial class Resources {
        
        private ResourceDictionary resourceDictionaryField;
        
        /// <remarks/>
        public ResourceDictionary ResourceDictionary {
            get {
                return this.resourceDictionaryField;
            }
            set {
                this.resourceDictionaryField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/xps/2005/06")]
    [System.Xml.Serialization.XmlRootAttribute("ResourceDictionary", Namespace="http://schemas.microsoft.com/xps/2005/06", IsNullable=false)]
    public partial class ResourceDictionary {
        
        private object[] itemsField;
        
        private string sourceField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Canvas", typeof(Canvas))]
        [System.Xml.Serialization.XmlElementAttribute("Glyphs", typeof(Glyphs))]
        [System.Xml.Serialization.XmlElementAttribute("ImageBrush", typeof(ImageBrush))]
        [System.Xml.Serialization.XmlElementAttribute("LinearGradientBrush", typeof(LinearGradientBrush))]
        [System.Xml.Serialization.XmlElementAttribute("MatrixTransform", typeof(MatrixTransform))]
        [System.Xml.Serialization.XmlElementAttribute("Path", typeof(Path))]
        [System.Xml.Serialization.XmlElementAttribute("PathGeometry", typeof(PathGeometry))]
        [System.Xml.Serialization.XmlElementAttribute("RadialGradientBrush", typeof(RadialGradientBrush))]
        [System.Xml.Serialization.XmlElementAttribute("SolidColorBrush", typeof(SolidColorBrush))]
        [System.Xml.Serialization.XmlElementAttribute("VisualBrush", typeof(VisualBrush))]
        public object[] Items {
            get {
                return this.itemsField;
            }
            set {
                this.itemsField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="anyURI")]
        public string Source {
            get {
                return this.sourceField;
            }
            set {
                this.sourceField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/xps/2005/06")]
    [System.Xml.Serialization.XmlRootAttribute("Glyphs", Namespace="http://schemas.microsoft.com/xps/2005/06", IsNullable=false)]
    public partial class Glyphs {
        
        private Transform glyphsRenderTransformField;
        
        private Geometry glyphsClipField;
        
        private Brush glyphsOpacityMaskField;
        
        private Brush glyphsFillField;
        
        private string bidiLevelField;
        
        private string caretStopsField;
        
        private string deviceFontNameField;
        
        private string fillField;
        
        private double fontRenderingEmSizeField;
        
        private string fontUriField;
        
        private double originXField;
        
        private double originYField;
        
        private bool isSidewaysField;
        
        private string indicesField;
        
        private string unicodeStringField;
        
        private StyleSimulations styleSimulationsField;
        
        private string renderTransformField;
        
        private string clipField;
        
        private double opacityField;
        
        private string opacityMaskField;
        
        private string nameField;
        
        private string fixedPageNavigateUriField;
        
        private string langField;
        
        private string keyField;
        
        public Glyphs() {
            this.bidiLevelField = "0";
            this.isSidewaysField = false;
            this.styleSimulationsField = StyleSimulations.None;
            this.opacityField = 1;
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Glyphs.RenderTransform")]
        public Transform GlyphsRenderTransform {
            get {
                return this.glyphsRenderTransformField;
            }
            set {
                this.glyphsRenderTransformField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Glyphs.Clip")]
        public Geometry GlyphsClip {
            get {
                return this.glyphsClipField;
            }
            set {
                this.glyphsClipField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Glyphs.OpacityMask")]
        public Brush GlyphsOpacityMask {
            get {
                return this.glyphsOpacityMaskField;
            }
            set {
                this.glyphsOpacityMaskField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Glyphs.Fill")]
        public Brush GlyphsFill {
            get {
                return this.glyphsFillField;
            }
            set {
                this.glyphsFillField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="integer")]
        [System.ComponentModel.DefaultValueAttribute("0")]
        public string BidiLevel {
            get {
                return this.bidiLevelField;
            }
            set {
                this.bidiLevelField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string CaretStops {
            get {
                return this.caretStopsField;
            }
            set {
                this.caretStopsField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string DeviceFontName {
            get {
                return this.deviceFontNameField;
            }
            set {
                this.deviceFontNameField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Fill {
            get {
                return this.fillField;
            }
            set {
                this.fillField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public double FontRenderingEmSize {
            get {
                return this.fontRenderingEmSizeField;
            }
            set {
                this.fontRenderingEmSizeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="anyURI")]
        public string FontUri {
            get {
                return this.fontUriField;
            }
            set {
                this.fontUriField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public double OriginX {
            get {
                return this.originXField;
            }
            set {
                this.originXField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public double OriginY {
            get {
                return this.originYField;
            }
            set {
                this.originYField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool IsSideways {
            get {
                return this.isSidewaysField;
            }
            set {
                this.isSidewaysField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Indices {
            get {
                return this.indicesField;
            }
            set {
                this.indicesField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string UnicodeString {
            get {
                return this.unicodeStringField;
            }
            set {
                this.unicodeStringField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(StyleSimulations.None)]
        public StyleSimulations StyleSimulations {
            get {
                return this.styleSimulationsField;
            }
            set {
                this.styleSimulationsField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string RenderTransform {
            get {
                return this.renderTransformField;
            }
            set {
                this.renderTransformField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Clip {
            get {
                return this.clipField;
            }
            set {
                this.clipField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(1)]
        public double Opacity {
            get {
                return this.opacityField;
            }
            set {
                this.opacityField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string OpacityMask {
            get {
                return this.opacityMaskField;
            }
            set {
                this.opacityMaskField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="ID")]
        public string Name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("FixedPage.NavigateUri", DataType="anyURI")]
        public string FixedPageNavigateUri {
            get {
                return this.fixedPageNavigateUriField;
            }
            set {
                this.fixedPageNavigateUriField = value;
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
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form=System.Xml.Schema.XmlSchemaForm.Qualified, Namespace="http://schemas.microsoft.com/xps/2005/06/resourcedictionary-key")]
        public string Key {
            get {
                return this.keyField;
            }
            set {
                this.keyField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/xps/2005/06")]
    [System.Xml.Serialization.XmlRootAttribute("Glyphs.Clip", Namespace="http://schemas.microsoft.com/xps/2005/06", IsNullable=false)]
    public partial class Geometry {
        
        private PathGeometry pathGeometryField;
        
        /// <remarks/>
        public PathGeometry PathGeometry {
            get {
                return this.pathGeometryField;
            }
            set {
                this.pathGeometryField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/xps/2005/06")]
    [System.Xml.Serialization.XmlRootAttribute("PathGeometry", Namespace="http://schemas.microsoft.com/xps/2005/06", IsNullable=false)]
    public partial class PathGeometry {
        
        private Transform pathGeometryTransformField;
        
        private PathFigure[] pathFigureField;
        
        private string figuresField;
        
        private FillRule fillRuleField;
        
        private string transformField;
        
        private string keyField;
        
        public PathGeometry() {
            this.fillRuleField = FillRule.EvenOdd;
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("PathGeometry.Transform")]
        public Transform PathGeometryTransform {
            get {
                return this.pathGeometryTransformField;
            }
            set {
                this.pathGeometryTransformField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("PathFigure")]
        public PathFigure[] PathFigure {
            get {
                return this.pathFigureField;
            }
            set {
                this.pathFigureField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Figures {
            get {
                return this.figuresField;
            }
            set {
                this.figuresField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(FillRule.EvenOdd)]
        public FillRule FillRule {
            get {
                return this.fillRuleField;
            }
            set {
                this.fillRuleField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Transform {
            get {
                return this.transformField;
            }
            set {
                this.transformField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form=System.Xml.Schema.XmlSchemaForm.Qualified, Namespace="http://schemas.microsoft.com/xps/2005/06/resourcedictionary-key")]
        public string Key {
            get {
                return this.keyField;
            }
            set {
                this.keyField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/xps/2005/06")]
    [System.Xml.Serialization.XmlRootAttribute("PathFigure", Namespace="http://schemas.microsoft.com/xps/2005/06", IsNullable=false)]
    public partial class PathFigure {
        
        private object[] itemsField;
        
        private bool isClosedField;
        
        private string startPointField;
        
        private bool isFilledField;
        
        public PathFigure() {
            this.isClosedField = false;
            this.isFilledField = true;
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ArcSegment", typeof(ArcSegment))]
        [System.Xml.Serialization.XmlElementAttribute("PolyBezierSegment", typeof(PolyBezierSegment))]
        [System.Xml.Serialization.XmlElementAttribute("PolyLineSegment", typeof(PolyLineSegment))]
        [System.Xml.Serialization.XmlElementAttribute("PolyQuadraticBezierSegment", typeof(PolyQuadraticBezierSegment))]
        public object[] Items {
            get {
                return this.itemsField;
            }
            set {
                this.itemsField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool IsClosed {
            get {
                return this.isClosedField;
            }
            set {
                this.isClosedField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string StartPoint {
            get {
                return this.startPointField;
            }
            set {
                this.startPointField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool IsFilled {
            get {
                return this.isFilledField;
            }
            set {
                this.isFilledField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/xps/2005/06")]
    [System.Xml.Serialization.XmlRootAttribute("ArcSegment", Namespace="http://schemas.microsoft.com/xps/2005/06", IsNullable=false)]
    public partial class ArcSegment {
        
        private string pointField;
        
        private string sizeField;
        
        private double rotationAngleField;
        
        private bool isLargeArcField;
        
        private SweepDirection sweepDirectionField;
        
        private bool isStrokedField;
        
        public ArcSegment() {
            this.isStrokedField = true;
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Point {
            get {
                return this.pointField;
            }
            set {
                this.pointField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Size {
            get {
                return this.sizeField;
            }
            set {
                this.sizeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public double RotationAngle {
            get {
                return this.rotationAngleField;
            }
            set {
                this.rotationAngleField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool IsLargeArc {
            get {
                return this.isLargeArcField;
            }
            set {
                this.isLargeArcField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public SweepDirection SweepDirection {
            get {
                return this.sweepDirectionField;
            }
            set {
                this.sweepDirectionField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool IsStroked {
            get {
                return this.isStrokedField;
            }
            set {
                this.isStrokedField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/xps/2005/06")]
    public enum SweepDirection {
        
        /// <remarks/>
        Clockwise,
        
        /// <remarks/>
        Counterclockwise,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/xps/2005/06")]
    [System.Xml.Serialization.XmlRootAttribute("PolyBezierSegment", Namespace="http://schemas.microsoft.com/xps/2005/06", IsNullable=false)]
    public partial class PolyBezierSegment {
        
        private string pointsField;
        
        private bool isStrokedField;
        
        public PolyBezierSegment() {
            this.isStrokedField = true;
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Points {
            get {
                return this.pointsField;
            }
            set {
                this.pointsField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool IsStroked {
            get {
                return this.isStrokedField;
            }
            set {
                this.isStrokedField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/xps/2005/06")]
    [System.Xml.Serialization.XmlRootAttribute("PolyLineSegment", Namespace="http://schemas.microsoft.com/xps/2005/06", IsNullable=false)]
    public partial class PolyLineSegment {
        
        private string pointsField;
        
        private bool isStrokedField;
        
        public PolyLineSegment() {
            this.isStrokedField = true;
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Points {
            get {
                return this.pointsField;
            }
            set {
                this.pointsField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool IsStroked {
            get {
                return this.isStrokedField;
            }
            set {
                this.isStrokedField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/xps/2005/06")]
    [System.Xml.Serialization.XmlRootAttribute("PolyQuadraticBezierSegment", Namespace="http://schemas.microsoft.com/xps/2005/06", IsNullable=false)]
    public partial class PolyQuadraticBezierSegment {
        
        private string pointsField;
        
        private bool isStrokedField;
        
        public PolyQuadraticBezierSegment() {
            this.isStrokedField = true;
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Points {
            get {
                return this.pointsField;
            }
            set {
                this.pointsField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool IsStroked {
            get {
                return this.isStrokedField;
            }
            set {
                this.isStrokedField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/xps/2005/06")]
    public enum FillRule {
        
        /// <remarks/>
        EvenOdd,
        
        /// <remarks/>
        NonZero,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/xps/2005/06")]
    [System.Xml.Serialization.XmlRootAttribute("Glyphs.OpacityMask", Namespace="http://schemas.microsoft.com/xps/2005/06", IsNullable=false)]
    public partial class Brush {
        
        private object itemField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ImageBrush", typeof(ImageBrush))]
        [System.Xml.Serialization.XmlElementAttribute("LinearGradientBrush", typeof(LinearGradientBrush))]
        [System.Xml.Serialization.XmlElementAttribute("RadialGradientBrush", typeof(RadialGradientBrush))]
        [System.Xml.Serialization.XmlElementAttribute("SolidColorBrush", typeof(SolidColorBrush))]
        [System.Xml.Serialization.XmlElementAttribute("VisualBrush", typeof(VisualBrush))]
        public object Item {
            get {
                return this.itemField;
            }
            set {
                this.itemField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/xps/2005/06")]
    [System.Xml.Serialization.XmlRootAttribute("LinearGradientBrush", Namespace="http://schemas.microsoft.com/xps/2005/06", IsNullable=false)]
    public partial class LinearGradientBrush {
        
        private Transform linearGradientBrushTransformField;
        
        private GradientStop[] linearGradientBrushGradientStopsField;
        
        private double opacityField;
        
        private string keyField;
        
        private ClrIntMode colorInterpolationModeField;
        
        private SpreadMethod spreadMethodField;
        
        private MappingMode mappingModeField;
        
        private string transformField;
        
        private string startPointField;
        
        private string endPointField;
        
        public LinearGradientBrush() {
            this.opacityField = 1;
            this.colorInterpolationModeField = ClrIntMode.SRgbLinearInterpolation;
            this.spreadMethodField = SpreadMethod.None;
            this.mappingModeField = MappingMode.Absolute;
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("LinearGradientBrush.Transform")]
        public Transform LinearGradientBrushTransform {
            get {
                return this.linearGradientBrushTransformField;
            }
            set {
                this.linearGradientBrushTransformField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute("LinearGradientBrush.GradientStops")]
        [System.Xml.Serialization.XmlArrayItemAttribute("GradientStop", IsNullable=false)]
        public GradientStop[] LinearGradientBrushGradientStops {
            get {
                return this.linearGradientBrushGradientStopsField;
            }
            set {
                this.linearGradientBrushGradientStopsField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(1)]
        public double Opacity {
            get {
                return this.opacityField;
            }
            set {
                this.opacityField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form=System.Xml.Schema.XmlSchemaForm.Qualified, Namespace="http://schemas.microsoft.com/xps/2005/06/resourcedictionary-key")]
        public string Key {
            get {
                return this.keyField;
            }
            set {
                this.keyField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(ClrIntMode.SRgbLinearInterpolation)]
        public ClrIntMode ColorInterpolationMode {
            get {
                return this.colorInterpolationModeField;
            }
            set {
                this.colorInterpolationModeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(SpreadMethod.Pad)]
        public SpreadMethod SpreadMethod {
            get {
                return this.spreadMethodField;
            }
            set {
                this.spreadMethodField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public MappingMode MappingMode {
            get {
                return this.mappingModeField;
            }
            set {
                this.mappingModeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Transform {
            get {
                return this.transformField;
            }
            set {
                this.transformField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string StartPoint {
            get {
                return this.startPointField;
            }
            set {
                this.startPointField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string EndPoint {
            get {
                return this.endPointField;
            }
            set {
                this.endPointField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/xps/2005/06")]
    [System.Xml.Serialization.XmlRootAttribute("GradientStop", Namespace="http://schemas.microsoft.com/xps/2005/06", IsNullable=false)]
    public partial class GradientStop {
        
        private string colorField;
        
        private double offsetField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Color {
            get {
                return this.colorField;
            }
            set {
                this.colorField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public double Offset {
            get {
                return this.offsetField;
            }
            set {
                this.offsetField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/xps/2005/06")]
    public enum ClrIntMode {
        
        /// <remarks/>
        ScRgbLinearInterpolation,
        
        /// <remarks/>
        SRgbLinearInterpolation,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/xps/2005/06")]
    public enum SpreadMethod {
        
        /// <remarks/>
        Pad,
        
        /// <remarks/>
        Reflect,
        
        /// <remarks/>
        Repeat,
        None,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/xps/2005/06")]
    public enum MappingMode {
        
        /// <remarks/>
        Absolute,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/xps/2005/06")]
    [System.Xml.Serialization.XmlRootAttribute("RadialGradientBrush", Namespace="http://schemas.microsoft.com/xps/2005/06", IsNullable=false)]
    public partial class RadialGradientBrush {
        
        private Transform radialGradientBrushTransformField;
        
        private GradientStop[] radialGradientBrushGradientStopsField;
        
        private double opacityField;
        
        private string keyField;
        
        private ClrIntMode colorInterpolationModeField;
        
        private SpreadMethod spreadMethodField;
        
        private MappingMode mappingModeField;
        
        private string transformField;
        
        private string centerField;
        
        private string gradientOriginField;
        
        private double radiusXField;
        
        private double radiusYField;
        
        public RadialGradientBrush() {
            this.opacityField = 1;
            this.colorInterpolationModeField = ClrIntMode.SRgbLinearInterpolation;
            this.spreadMethodField = SpreadMethod.Pad;
            this.mappingModeField = MappingMode.Absolute;
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("RadialGradientBrush.Transform")]
        public Transform RadialGradientBrushTransform {
            get {
                return this.radialGradientBrushTransformField;
            }
            set {
                this.radialGradientBrushTransformField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute("RadialGradientBrush.GradientStops")]
        [System.Xml.Serialization.XmlArrayItemAttribute("GradientStop", IsNullable=false)]
        public GradientStop[] RadialGradientBrushGradientStops {
            get {
                return this.radialGradientBrushGradientStopsField;
            }
            set {
                this.radialGradientBrushGradientStopsField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(1)]
        public double Opacity {
            get {
                return this.opacityField;
            }
            set {
                this.opacityField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form=System.Xml.Schema.XmlSchemaForm.Qualified, Namespace="http://schemas.microsoft.com/xps/2005/06/resourcedictionary-key")]
        public string Key {
            get {
                return this.keyField;
            }
            set {
                this.keyField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(ClrIntMode.SRgbLinearInterpolation)]
        public ClrIntMode ColorInterpolationMode {
            get {
                return this.colorInterpolationModeField;
            }
            set {
                this.colorInterpolationModeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(SpreadMethod.Pad)]
        public SpreadMethod SpreadMethod {
            get {
                return this.spreadMethodField;
            }
            set {
                this.spreadMethodField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public MappingMode MappingMode {
            get {
                return this.mappingModeField;
            }
            set {
                this.mappingModeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Transform {
            get {
                return this.transformField;
            }
            set {
                this.transformField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Center {
            get {
                return this.centerField;
            }
            set {
                this.centerField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string GradientOrigin {
            get {
                return this.gradientOriginField;
            }
            set {
                this.gradientOriginField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public double RadiusX {
            get {
                return this.radiusXField;
            }
            set {
                this.radiusXField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public double RadiusY {
            get {
                return this.radiusYField;
            }
            set {
                this.radiusYField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/xps/2005/06")]
    public enum StyleSimulations {
        
        /// <remarks/>
        None,
        
        /// <remarks/>
        ItalicSimulation,
        
        /// <remarks/>
        BoldSimulation,
        
        /// <remarks/>
        BoldItalicSimulation,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/xps/2005/06")]
    [System.Xml.Serialization.XmlRootAttribute("Path", Namespace="http://schemas.microsoft.com/xps/2005/06", IsNullable=false)]
    public partial class Path {
        
        private Transform pathRenderTransformField;
        
        private Geometry pathClipField;
        
        private Brush pathOpacityMaskField;
        
        private Brush pathFillField;
        
        private Brush pathStrokeField;
        
        private Geometry pathDataField;
        
        private string dataField;
        
        private string fillField;
        
        private string renderTransformField;
        
        private string clipField;
        
        private double opacityField;
        
        private string opacityMaskField;
        
        private string strokeField;
        
        private string strokeDashArrayField;
        
        private DashCap strokeDashCapField;
        
        private double strokeDashOffsetField;
        
        private LineCap strokeEndLineCapField;
        
        private LineCap strokeStartLineCapField;
        
        private LineJoin strokeLineJoinField;
        
        private double strokeMiterLimitField;
        
        private double strokeThicknessField;
        
        private string nameField;
        
        private string fixedPageNavigateUriField;
        
        private string langField;
        
        private string keyField;
        
        private string automationPropertiesNameField;
        
        private string automationPropertiesHelpTextField;
        
        private bool snapsToDevicePixelsField;
        
        private bool snapsToDevicePixelsFieldSpecified;
        
        public Path() {
            this.opacityField = 1;
            this.strokeDashCapField = DashCap.Flat;
            this.strokeDashOffsetField = 0;
            this.strokeEndLineCapField = LineCap.Flat;
            this.strokeStartLineCapField = LineCap.Flat;
            this.strokeLineJoinField = LineJoin.Miter;
            this.strokeMiterLimitField = 10;
            this.strokeThicknessField = 1;
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Path.RenderTransform")]
        public Transform PathRenderTransform {
            get {
                return this.pathRenderTransformField;
            }
            set {
                this.pathRenderTransformField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Path.Clip")]
        public Geometry PathClip {
            get {
                return this.pathClipField;
            }
            set {
                this.pathClipField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Path.OpacityMask")]
        public Brush PathOpacityMask {
            get {
                return this.pathOpacityMaskField;
            }
            set {
                this.pathOpacityMaskField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Path.Fill")]
        public Brush PathFill {
            get {
                return this.pathFillField;
            }
            set {
                this.pathFillField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Path.Stroke")]
        public Brush PathStroke {
            get {
                return this.pathStrokeField;
            }
            set {
                this.pathStrokeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Path.Data")]
        public Geometry PathData {
            get {
                return this.pathDataField;
            }
            set {
                this.pathDataField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Data {
            get {
                return this.dataField;
            }
            set {
                this.dataField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Fill {
            get {
                return this.fillField;
            }
            set {
                this.fillField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string RenderTransform {
            get {
                return this.renderTransformField;
            }
            set {
                this.renderTransformField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Clip {
            get {
                return this.clipField;
            }
            set {
                this.clipField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(1)]
        public double Opacity {
            get {
                return this.opacityField;
            }
            set {
                this.opacityField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string OpacityMask {
            get {
                return this.opacityMaskField;
            }
            set {
                this.opacityMaskField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Stroke {
            get {
                return this.strokeField;
            }
            set {
                this.strokeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string StrokeDashArray {
            get {
                return this.strokeDashArrayField;
            }
            set {
                this.strokeDashArrayField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(DashCap.Flat)]
        public DashCap StrokeDashCap {
            get {
                return this.strokeDashCapField;
            }
            set {
                this.strokeDashCapField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(0)]
        public double StrokeDashOffset {
            get {
                return this.strokeDashOffsetField;
            }
            set {
                this.strokeDashOffsetField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(LineCap.Flat)]
        public LineCap StrokeEndLineCap {
            get {
                return this.strokeEndLineCapField;
            }
            set {
                this.strokeEndLineCapField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(LineCap.Flat)]
        public LineCap StrokeStartLineCap {
            get {
                return this.strokeStartLineCapField;
            }
            set {
                this.strokeStartLineCapField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(LineJoin.Miter)]
        public LineJoin StrokeLineJoin {
            get {
                return this.strokeLineJoinField;
            }
            set {
                this.strokeLineJoinField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(10)]
        public double StrokeMiterLimit {
            get {
                return this.strokeMiterLimitField;
            }
            set {
                this.strokeMiterLimitField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(1)]
        public double StrokeThickness {
            get {
                return this.strokeThicknessField;
            }
            set {
                this.strokeThicknessField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="ID")]
        public string Name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("FixedPage.NavigateUri", DataType="anyURI")]
        public string FixedPageNavigateUri {
            get {
                return this.fixedPageNavigateUriField;
            }
            set {
                this.fixedPageNavigateUriField = value;
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
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form=System.Xml.Schema.XmlSchemaForm.Qualified, Namespace="http://schemas.microsoft.com/xps/2005/06/resourcedictionary-key")]
        public string Key {
            get {
                return this.keyField;
            }
            set {
                this.keyField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("AutomationProperties.Name")]
        public string AutomationPropertiesName {
            get {
                return this.automationPropertiesNameField;
            }
            set {
                this.automationPropertiesNameField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("AutomationProperties.HelpText")]
        public string AutomationPropertiesHelpText {
            get {
                return this.automationPropertiesHelpTextField;
            }
            set {
                this.automationPropertiesHelpTextField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool SnapsToDevicePixels {
            get {
                return this.snapsToDevicePixelsField;
            }
            set {
                this.snapsToDevicePixelsField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool SnapsToDevicePixelsSpecified {
            get {
                return this.snapsToDevicePixelsFieldSpecified;
            }
            set {
                this.snapsToDevicePixelsFieldSpecified = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/xps/2005/06")]
    public enum DashCap {
        
        /// <remarks/>
        Flat,
        
        /// <remarks/>
        Round,
        
        /// <remarks/>
        Square,
        
        /// <remarks/>
        Triangle,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/xps/2005/06")]
    public enum LineCap {
        
        /// <remarks/>
        Flat,
        
        /// <remarks/>
        Round,
        
        /// <remarks/>
        Square,
        
        /// <remarks/>
        Triangle,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/xps/2005/06")]
    public enum LineJoin {
        
        /// <remarks/>
        Miter,
        
        /// <remarks/>
        Bevel,
        
        /// <remarks/>
        Round,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/xps/2005/06")]
    public enum EdgeMode {
        
        /// <remarks/>
        Aliased,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/xps/2005/06")]
    [System.Xml.Serialization.XmlRootAttribute("FixedPage", Namespace="http://schemas.microsoft.com/xps/2005/06", IsNullable=false)]
    public partial class FixedPage {
        
        private Resources fixedPageResourcesField;
        
        private object[] itemsField;
        
        private double widthField;
        
        private double heightField;
        
        private string contentBoxField;
        
        private string bleedBoxField;
        
        private string langField;
        
        private string nameField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("FixedPage.Resources")]
        public Resources FixedPageResources {
            get {
                return this.fixedPageResourcesField;
            }
            set {
                this.fixedPageResourcesField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Canvas", typeof(Canvas))]
        [System.Xml.Serialization.XmlElementAttribute("Glyphs", typeof(Glyphs))]
        [System.Xml.Serialization.XmlElementAttribute("Path", typeof(Path))]
        public object[] Items {
            get {
                return this.itemsField;
            }
            set {
                this.itemsField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public double Width {
            get {
                return this.widthField;
            }
            set {
                this.widthField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public double Height {
            get {
                return this.heightField;
            }
            set {
                this.heightField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ContentBox {
            get {
                return this.contentBoxField;
            }
            set {
                this.contentBoxField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string BleedBox {
            get {
                return this.bleedBoxField;
            }
            set {
                this.bleedBoxField = value;
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
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="ID")]
        public string Name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/xps/2005/06")]
    [System.Xml.Serialization.XmlRootAttribute("FixedDocument", Namespace="http://schemas.microsoft.com/xps/2005/06", IsNullable=false)]
    public partial class FixedDocument {
        
        private PageContent[] pageContentField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("PageContent")]
        public PageContent[] PageContent {
            get {
                return this.pageContentField;
            }
            set {
                this.pageContentField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/xps/2005/06")]
    [System.Xml.Serialization.XmlRootAttribute("PageContent", Namespace="http://schemas.microsoft.com/xps/2005/06", IsNullable=false)]
    public partial class PageContent {
        
        private LinkTarget[] pageContentLinkTargetsField;
        
        private string sourceField;
        
        private double widthField;
        
        private bool widthFieldSpecified;
        
        private double heightField;
        
        private bool heightFieldSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute("PageContent.LinkTargets")]
        [System.Xml.Serialization.XmlArrayItemAttribute("LinkTarget", IsNullable=false)]
        public LinkTarget[] PageContentLinkTargets {
            get {
                return this.pageContentLinkTargetsField;
            }
            set {
                this.pageContentLinkTargetsField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="anyURI")]
        public string Source {
            get {
                return this.sourceField;
            }
            set {
                this.sourceField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public double Width {
            get {
                return this.widthField;
            }
            set {
                this.widthField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool WidthSpecified {
            get {
                return this.widthFieldSpecified;
            }
            set {
                this.widthFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public double Height {
            get {
                return this.heightField;
            }
            set {
                this.heightField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool HeightSpecified {
            get {
                return this.heightFieldSpecified;
            }
            set {
                this.heightFieldSpecified = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/xps/2005/06")]
    [System.Xml.Serialization.XmlRootAttribute("LinkTarget", Namespace="http://schemas.microsoft.com/xps/2005/06", IsNullable=false)]
    public partial class LinkTarget {
        
        private string nameField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="ID")]
        public string Name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/xps/2005/06")]
    [System.Xml.Serialization.XmlRootAttribute("FixedDocumentSequence", Namespace="http://schemas.microsoft.com/xps/2005/06", IsNullable=false)]
    public partial class FixedDocumentSequence {
        
        private DocumentReference[] documentReferenceField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("DocumentReference")]
        public DocumentReference[] DocumentReference {
            get {
                return this.documentReferenceField;
            }
            set {
                this.documentReferenceField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/xps/2005/06")]
    [System.Xml.Serialization.XmlRootAttribute("DocumentReference", Namespace="http://schemas.microsoft.com/xps/2005/06", IsNullable=false)]
    public partial class DocumentReference {
        
        private string sourceField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="anyURI")]
        public string Source {
            get {
                return this.sourceField;
            }
            set {
                this.sourceField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/xps/2005/06")]
    [System.Xml.Serialization.XmlRootAttribute("PageContent.LinkTargets", Namespace="http://schemas.microsoft.com/xps/2005/06", IsNullable=false)]
    public partial class LinkTargets {
        
        private LinkTarget[] linkTargetField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("LinkTarget")]
        public LinkTarget[] LinkTarget {
            get {
                return this.linkTargetField;
            }
            set {
                this.linkTargetField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/xps/2005/06")]
    [System.Xml.Serialization.XmlRootAttribute("LinearGradientBrush.GradientStops", Namespace="http://schemas.microsoft.com/xps/2005/06", IsNullable=false)]
    public partial class GradientStops {
        
        private GradientStop[] gradientStopField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("GradientStop")]
        public GradientStop[] GradientStop {
            get {
                return this.gradientStopField;
            }
            set {
                this.gradientStopField = value;
            }
        }
    }
}
