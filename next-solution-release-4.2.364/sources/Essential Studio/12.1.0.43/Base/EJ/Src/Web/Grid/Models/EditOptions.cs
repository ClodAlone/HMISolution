#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Syncfusion.JavaScript.Shared.Serializer;


namespace Syncfusion.JavaScript.Models
{
    public class EditOptions<T> where T : class
    {
        private bool allowEditing = false;
        private bool allowAdding = false;
        private bool allowDeleting = false;
        private EditMode editMode = EditMode.Normal;
        private String dialogEditorTemplateId = null;
        private String externalFormTemplateId = null;
        private String inlineFormTemplateId = null;
        private FormPosition formPosition = FormPosition.BottomLeft;

        //Properties
        [JsonProperty("allowEditing")]
        [DefaultValue(false)]
        public bool AllowEditing
        {
            get { return this.allowEditing; }
            set { this.allowEditing = value; }
        }
        [JsonProperty("allowAdding")]
        [DefaultValue(false)]
        public bool AllowAdding
        {
            get { return this.allowAdding; }
            set { this.allowAdding = value; }
        }
        [JsonProperty("allowDeleting")]
        [DefaultValue(false)]
        public bool AllowDeleting
        {
            get { return this.allowDeleting; }
            set { this.allowDeleting = value; }
        }
        [JsonProperty("editMode")]
        [DefaultValue(EditMode.Normal)]
        [JsonConverter(typeof(StringEnumConverter))]
        public EditMode EditMode
        {
            get { return this.editMode; }
            set { this.editMode = value; }
        }
        [JsonProperty("dialogEditorTemplateId")]
        [DefaultValue(null)]
        public String DialogEditorTemplateId
        {
            get { return this.dialogEditorTemplateId; }
            set { this.dialogEditorTemplateId = value; }
        }
        [JsonProperty("externalFormTemplateId")]
        [DefaultValue(null)]
        public String ExternalFormTemplateId
        {
            get { return this.externalFormTemplateId; }
            set { this.externalFormTemplateId = value; }
        }
        [JsonProperty("inlineFormTemplateId")]
        [DefaultValue(null)]
        public String InlineFormTemplateId
        {
            get { return this.inlineFormTemplateId; }
            set { this.inlineFormTemplateId = value; }
        }
        [JsonProperty("formPosition")]
        [DefaultValue(FormPosition.BottomLeft)]
        [JsonConverter(typeof(StringEnumConverter))]
        public FormPosition FormPosition
        {
            get { return this.formPosition; }
            set { this.formPosition = value; }
        }
        //[JsonProperty("gridProperty")]
        //public GridRenderer Gridprop
        //{
        //    get { return this.gridprop; }
        //    set { this.gridprop = value; }
        //}
    }
}
