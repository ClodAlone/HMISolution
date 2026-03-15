#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using System.ComponentModel;
using Syncfusion.JavaScript.DataSources;
using Syncfusion.JavaScript;
using Syncfusion.JavaScript.Shared.Serializer;


namespace Syncfusion.JavaScript.Models
{
    public class UploadboxProperties
    {

        #region Fields

        //Boolean Values
        private bool enabled = true;
        private bool asyncUpload = true;
        private bool multipleFilesSelect = false;
        private bool autoUpload = false;
        private bool showFileDetails = true;
        private bool rtl = false;
        //String Values
        private String browseButtonText = "Browse";
        private String uploadButtonText = "Upload";
        private String fileFormatAllow = "";
        private String fileFormatDeny = "";
        private String saveUrl = "";
        private String removeUrl = "";
        private String cssClass = "";

        //Events 
        private String create = null;
        private String fileSelect = null;
        private String begin = null;
        private String cancel = null;
        private String complete = null;
        private String remove = null;
        private String error = null;
        private String destroy = null;

        //Button 
        private Uploadbox uploadbox = new Uploadbox();

        #endregion

        public UploadboxProperties() {}
        //public UploadboxProperties(String id, Uploadbox uploadbox)
        //{
        //    uploadbox.ID = id;
        //    this.uploadbox = uploadbox;
        //}

        #region Properties
        [JsonProperty("enabled")]
        [DefaultValue(true)]
        public bool Enabled
        {
            get { return this.enabled; }
            set { this.enabled = value; }
        }
        [JsonProperty("asyncUpload")]
        [DefaultValue(true)]
        public bool AsyncUpload
        {
            get { return this.asyncUpload; }
            set { this.asyncUpload = value; }
        }
       
        [JsonProperty("multipleFilesSelect")]
        [DefaultValue(false)]
        public bool MultipleFilesSelect
        {
            get { return this.multipleFilesSelect; }
            set { this.multipleFilesSelect = value; }
        }
        [JsonProperty("autoUpload")]
        [DefaultValue(false)]
        public bool AutoUpload
        {
            get { return this.autoUpload; }
            set { this.autoUpload = value; }
        }
        [JsonProperty("showFileDetails")]
        [DefaultValue(true)]
        public bool ShowFileDetails
        {
            get { return this.showFileDetails; }
            set { this.showFileDetails = value; }
        }
        [JsonProperty("rtl")]
        [DefaultValue(false)]
        public bool Rtl
        {
            get { return this.rtl; }
            set { this.rtl = value; }
        }
       
        //string values
        [JsonProperty("browseButtonText")]
        [DefaultValue("Browse")]
        public String BrowseButtonText
        {
            get { return this.browseButtonText; }
            set { this.browseButtonText = value; }
        }
        [JsonProperty("uploadButtonText")]
        [DefaultValue("Upload")]
        public String UploadButtonText
        {
            get { return this.uploadButtonText; }
            set { this.uploadButtonText = value; }
        }
        [JsonProperty("fileFormatAllow")]
        [DefaultValue("")]
        public String FileFormatAllow
        {
            get { return this.fileFormatAllow; }
            set { this.fileFormatAllow = value; }
        }
        [JsonProperty("fileFormatDeny")]
        [DefaultValue(null)]
        public String FileFormatDeny
        {
            get { return this.fileFormatDeny; }
            set { this.fileFormatDeny = value; }
        }
        [JsonProperty("saveUrl")]
        [DefaultValue(null)]
        public String SaveUrl
        {
            get { return this.saveUrl; }
            set { this.saveUrl = value; }
        }
        [JsonProperty("removeUrl")]
        [DefaultValue(null)]
        public String RemoveUrl
        {
            get { return this.removeUrl; }
            set { this.removeUrl = value; }
        }
        [JsonProperty("cssClass")]
        [DefaultValue(null)]
        public String CssClass
        {
            get { return this.cssClass; }
            set { this.cssClass = value; }
        }
        //Events 
        [JsonProperty("create")]
        [DefaultValue(null)]
        public String Create
        {
            get { return this.create; }
            set { this.create = value; }
        }
        [JsonProperty("fileSelect")]
        [DefaultValue(null)]
        public String FileSelect
        {
            get { return this.fileSelect; }
            set { this.fileSelect = value; }
        }
        [JsonProperty("begin")]
        [DefaultValue(null)]
        public String Begin
        {
            get { return this.begin; }
            set { this.begin = value; }
        }
        [JsonProperty("cancel")]
        [DefaultValue(null)]
        public String Cancel
        {
            get { return this.cancel; }
            set { this.cancel = value; }
        }
        [JsonProperty("complete")]
        [DefaultValue(null)]
        public String Complete
        {
            get { return this.complete; }
            set { this.complete = value; }
        }
        [JsonProperty("error")]
        [DefaultValue(null)]
        public String Error
        {
            get { return this.error; }
            set { this.error = value; }
        }
        [JsonProperty("remove")]
        [DefaultValue(null)]
        public String Remove
        {
            get { return this.remove; }
            set { this.remove = value; }
        }
        [JsonProperty("destroy")]
        [DefaultValue(null)]
        public String Destroy
        {
            get { return this.destroy; }
            set { this.destroy = value; }
        }
        #endregion
        #region ShouldSerialize Methods

        #endregion
    }
}
