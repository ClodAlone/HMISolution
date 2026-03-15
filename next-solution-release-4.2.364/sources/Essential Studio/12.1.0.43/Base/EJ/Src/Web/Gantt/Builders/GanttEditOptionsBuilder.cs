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
using System.Web;
using Syncfusion.JavaScript.Models;

namespace Syncfusion.JavaScript
{
    public class GanttEditOptionsBuilder
    {
        private GanttEditOptions editOption = new GanttEditOptions();

        public GanttEditOptionsBuilder(GanttEditOptions edit)
        { editOption = edit; }
        public GanttEditOptionsBuilder AllowEditing()
        {

            this.editOption.AllowEditing = true;
            return this;
        }
        public GanttEditOptionsBuilder AllowEditing(bool allowEditing)
        {
            this.editOption.AllowEditing = allowEditing;
            return this;
        }
        public GanttEditOptionsBuilder AllowAdding()
        {
            this.editOption.AllowAdding = true;
            return this;
        }
        public GanttEditOptionsBuilder AllowAdding(bool allowAdding)
        {
            this.editOption.AllowAdding = allowAdding;
            return this;
        }
        public GanttEditOptionsBuilder AllowDeleting()
        {
            this.editOption.AllowDeleting = true;
            return this;
        }
        public GanttEditOptionsBuilder AllowDeleting(bool allowDeleting)
        {
            this.editOption.AllowDeleting = allowDeleting;
            return this;
        }
        public GanttEditOptionsBuilder EditMode(string editMode)
        {
            this.editOption.EditMode = editMode;
            return this;
        }
        public GanttEditOptionsBuilder DialogEditorTemplateId(String dialogEditorTemplateId)
        {
            this.editOption.DialogEditorTemplateId = dialogEditorTemplateId;
            return this;
        }
       
    }
}