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
using Syncfusion.JavaScript;
using Syncfusion.JavaScript.Models;

namespace Syncfusion.JavaScript
{
    public class EditorClientSideEventsBuilder
    {
        private EditorProperties editorModel;
        public EditorClientSideEventsBuilder(EditorProperties editorProp)
        {
            editorModel = editorProp;
        }
        //Events
        public EditorClientSideEventsBuilder Create(String create)
        {
            editorModel.Create = create;
            return this;
        }
        public EditorClientSideEventsBuilder Change(String change)
        {
            editorModel.Change = change;
            return this;
        }
        public EditorClientSideEventsBuilder FocusIn(String focusIn)
        {
            editorModel.FocusIn = focusIn;
            return this;
        }
        public EditorClientSideEventsBuilder FocusOut(String focusOut)
        {
            editorModel.FocusOut = focusOut;
            return this;
        }
        public EditorClientSideEventsBuilder Destroy(String destroy)
        {
            editorModel.Destroy = destroy;
            return this;
        }
    }
}
