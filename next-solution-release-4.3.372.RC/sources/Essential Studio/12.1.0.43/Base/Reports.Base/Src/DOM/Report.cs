#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.ComponentModel;

namespace Syncfusion.RDL.DOM
{
#if !SILVERLIGHT
    [Serializable]
#endif
    [XmlRoot("Report")]
    public class ReportDefinition
    {
        public ReportDefinition()
        {
        }
        
        public ReportSections ReportSections { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public int AutoRefresh { get; set; }
        public DataSources DataSources { get; set; }
        public DataSets DataSets { get; set; }
        public Body Body { get; set; }
        public ReportParameters ReportParameters { get; set; }
        public CustomProperties CustomProperties { get; set; }
        public string Code { get; set; }
        public Size Width { get; set; }
        public Page Page { get; set; }
        public EmbeddedImages EmbeddedImages { get; set; }
        public string Language { get; set; }
        public CodeModules CodeModules { get; set; }
        public Classes Classes { get; set; }
        public Variables Variables { get; set; }
        public bool DeferVariableEvaluation { get; set; }
        public bool ConsumeContainerWhitespace { get; set; }
        public string DataTransform { get; set; }
        public string DataSchema { get; set; }
        public string DataElementName { get; set; }

        public DataElementStyles DataElementStyle { get; set; }

        [XmlIgnore()]
        public RDLType RDLType { get; set; }

        [XmlElement(Namespace = "http://schemas.microsoft.com/SQLServer/reporting/reportdesigner")]
        public string ReportUnitType { get; set; }

        #region Serialization methods

        public bool ShouldSerializeReportSections()
        {
            return this.ReportSections != null && this.ReportSections.Count > 0;
        }

        public void ResetReportSections()
        {
            this.ReportSections = new ReportSections();
        }

        public bool ShouldSerializeVariables()
        {
            return this.Variables != null && this.Variables.Count > 0;
        }

        public void ResetVariables()
        {
            this.Variables = new Variables();
        }

        public bool ShouldSerializeDataSources()
        {
            return this.DataSources != null && this.DataSources.Count > 0;
        }

        public void ResetDataSources()
        {
            this.DataSources = new DataSources();
        }

        public bool ShouldSerializeDataSets()
        {
            return this.DataSets != null && this.DataSets.Count > 0;
        }

        public void ResetDataSets()
        {
            this.DataSets = new DataSets();
        }

        public bool ShouldSerializeReportParameters()
        {
            return this.ReportParameters != null && this.ReportParameters.Count > 0;
        }

        public void ResetReportParameters()
        {
            this.ReportParameters = new ReportParameters();
        }
        
        public bool ShouldSerializeCustomProperties()
        {
            return this.CustomProperties != null && this.CustomProperties.Count > 0;
        }

        public void ResetCustomProperties()
        {
            this.CustomProperties = new CustomProperties();
        }

        public bool ShouldSerializeEmbeddedImages()
        {
            return this.EmbeddedImages != null && this.EmbeddedImages.Count > 0;
        }

        public void ResetEmbeddedImages()
        {
            this.EmbeddedImages = new EmbeddedImages();
        }

        public bool ShouldSerializeCodeModules()
        {
            return this.CodeModules != null && this.CodeModules.Count > 0;
        }

        public void ResetCodeModules()
        {
            this.CodeModules = new CodeModules();
        }

        public bool ShouldSerializeClasses()
        {
            return this.Classes != null && this.Classes.Count > 0;
        }

        public void ResetClasses()
        {
            this.Classes = new Classes();
        }

        public bool ShouldSerializeAutoRefresh()
        {
            return this.AutoRefresh != 0;
        }

        public void ResetAutoRefresh()
        {
            this.AutoRefresh = 0;
        }

        public bool ShouldSerializeReportUnitType()
        {
            return !string.IsNullOrEmpty(this.ReportUnitType);
        }

        public void ResetReportUnitType()
        {
            this.ReportUnitType = "Inch";
        }

        #endregion        
    }
}
