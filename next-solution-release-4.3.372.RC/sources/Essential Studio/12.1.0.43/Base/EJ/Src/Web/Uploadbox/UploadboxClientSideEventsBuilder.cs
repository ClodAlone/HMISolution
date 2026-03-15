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
    public class UploadboxClientSideEventsBuilder
    {
        private UploadboxProperties uploadboxModel;
        public UploadboxClientSideEventsBuilder(UploadboxProperties uploadboxProp)
        {
            uploadboxModel = uploadboxProp;
        }
        //Events
        public UploadboxClientSideEventsBuilder Create(String create)
        {
            uploadboxModel.Create = create;
            return this;
        }
        public UploadboxClientSideEventsBuilder FileSelect(String fileSelect)
        {
            uploadboxModel.FileSelect = fileSelect;
            return this;
        }
        public UploadboxClientSideEventsBuilder Begin(String begin)
        {
            uploadboxModel.Begin = begin;
            return this;
        }
        public UploadboxClientSideEventsBuilder Cancel(String cancel)
        {
            uploadboxModel.Cancel = cancel;
            return this;
        }
        public UploadboxClientSideEventsBuilder Complete(String complete)
        {
            uploadboxModel.Complete = complete;
            return this;
        }
        public UploadboxClientSideEventsBuilder Remove(String remove)
        {
            uploadboxModel.Remove = remove;
            return this;
        }
        public UploadboxClientSideEventsBuilder Error(String error)
        {
            uploadboxModel.Error = error;
            return this;
        }
        public UploadboxClientSideEventsBuilder Destroy(String destroy)
        {
            uploadboxModel.Destroy = destroy;
            return this;
        }
    }
}
