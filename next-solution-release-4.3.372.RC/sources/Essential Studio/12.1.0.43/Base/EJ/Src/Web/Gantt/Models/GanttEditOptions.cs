#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Syncfusion.JavaScript.Shared.Serializer;

namespace Syncfusion.JavaScript.Models
{
    public class GanttEditOptions
    {
        private bool allowEditing = false;
        private bool allowAdding = false;
        private bool allowDeleting = false;
        private string editMode = "Normal";
        private String dialogEditorTemplateId = null;

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
        [DefaultValue("Normal")]
        public string EditMode
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
       
    }
}