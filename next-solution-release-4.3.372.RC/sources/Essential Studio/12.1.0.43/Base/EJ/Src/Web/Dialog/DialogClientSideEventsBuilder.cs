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
    public class DialogClientSideEventsBuilder
    {
        private DialogProperties dialogModel;
        public DialogClientSideEventsBuilder(DialogProperties dialogProp)
        {
            dialogModel = dialogProp;
        }
        //Events
        public DialogClientSideEventsBuilder Create(String create)
        {
            dialogModel.Create = create;
            return this;
        }
        public DialogClientSideEventsBuilder BeforeClose(String beforeClose)
        {
            dialogModel.BeforeClose = beforeClose;
            return this;
        }
        public DialogClientSideEventsBuilder Close(String close)
        {
            dialogModel.Close = close;
            return this;
        }
        public DialogClientSideEventsBuilder BeforeOpen(String beforeOpen)
        {
            dialogModel.BeforeOpen = beforeOpen;
            return this;
        }
        public DialogClientSideEventsBuilder Open(String open)
        {
            dialogModel.Open = open;
            return this;
        }
        public DialogClientSideEventsBuilder Drag(String drag)
        {
            dialogModel.Drag = drag;
            return this;
        }
        public DialogClientSideEventsBuilder DragStart(String dragStart)
        {
            dialogModel.DragStart = dragStart;
            return this;
        }
        public DialogClientSideEventsBuilder DragStop(String dragStop)
        {
            dialogModel.DragStop = dragStop;
            return this;
        }
        public DialogClientSideEventsBuilder Resize(String resize)
        {
            dialogModel.Resize = resize;
            return this;
        }
        public DialogClientSideEventsBuilder ResizeStart(String resizeStart)
        {
            dialogModel.ResizeStart = resizeStart;
            return this;
        }
        public DialogClientSideEventsBuilder ResizeStop(String resizeStop)
        {
            dialogModel.ResizeStop = resizeStop;
            return this;
        }
        public DialogClientSideEventsBuilder Load(String load)
        {
            dialogModel.Load = load;
            return this;
        }
        public DialogClientSideEventsBuilder AjaxSuccess(String ajaxSuccess)
        {
            dialogModel.AjaxSuccess = ajaxSuccess;
            return this;
        }
        public DialogClientSideEventsBuilder AjaxError(String ajaxError)
        {
            dialogModel.AjaxError = ajaxError;
            return this;
        }
        public DialogClientSideEventsBuilder Destroy(String destroy)
        {
            dialogModel.Destroy = destroy;
            return this;
        }
    }
}
