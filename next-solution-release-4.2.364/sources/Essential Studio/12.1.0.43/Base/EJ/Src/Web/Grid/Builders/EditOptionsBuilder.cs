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
using Syncfusion.JavaScript.Models;

namespace Syncfusion.JavaScript
{
    public class EditOptionsBuilder<T> where T:class
    {

        private EditOptions<T> editOption=new EditOptions<T>();
        public EditOptionsBuilder(EditOptions<T> edit)
        { editOption = edit; }
        public EditOptionsBuilder<T> AllowEditing()
        {
            
            this.editOption.AllowEditing = true;
            return this;
        }
        public EditOptionsBuilder<T> AllowEditing(bool allowEditing) 
        {
            this.editOption.AllowEditing = allowEditing;
            return this;
        }
        public EditOptionsBuilder<T> AllowAdding()
        {
            this.editOption.AllowAdding = true;
            return this;
        }
        public EditOptionsBuilder<T> AllowAdding(bool allowAdding)
        {
            this.editOption.AllowAdding = allowAdding;
            return this;
        }
        public EditOptionsBuilder<T> AllowDeleting()
        {
            this.editOption.AllowDeleting = true;
            return this;
        }
        public EditOptionsBuilder<T> AllowDeleting(bool allowDeleting)
        {
            this.editOption.AllowDeleting = allowDeleting;
            return this;
        }
        public EditOptionsBuilder<T> EditMode(EditMode editMode)
        {
            this.editOption.EditMode = editMode;
            return this;
        }
        public EditOptionsBuilder<T> DialogEditorTemplateId(String dialogEditorTemplateId)
        {
            editOption.DialogEditorTemplateId = dialogEditorTemplateId;
            return this;
        }
        public EditOptionsBuilder<T> ExternalFormTemplateId(String externalFormTemplateId)
        {
            editOption.DialogEditorTemplateId = externalFormTemplateId;
            return this;
        }
        public EditOptionsBuilder<T> InlineFormTemplateId(String inlineFormTemplateId)
        {
            editOption.DialogEditorTemplateId = inlineFormTemplateId;
            return this;
        }
        public EditOptionsBuilder<T> FormPosition(FormPosition position)
        {
            this.editOption.FormPosition = position;
            return this;
        }
    }
}
