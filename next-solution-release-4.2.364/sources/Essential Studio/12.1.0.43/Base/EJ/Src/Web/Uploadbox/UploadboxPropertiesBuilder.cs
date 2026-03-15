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
using System.Data;
using System.Threading.Tasks;
using System.Web;
using Syncfusion.JavaScript.DataSources;
using Syncfusion.JavaScript;

namespace Syncfusion.JavaScript
{
    public class UploadboxPropertiesBuilder
    {
        public Uploadbox uploadbox;

        public UploadboxPropertiesBuilder(Uploadbox uploadbox)
        { this.uploadbox = new Uploadbox(uploadbox.ID, uploadbox.UploadboxModel); }

        public UploadboxPropertiesBuilder()
        {
        }
        //Boolean values
        public UploadboxPropertiesBuilder Enabled()
        {
            uploadbox.UploadboxModel.Enabled = true;
            return this;
        }
        public UploadboxPropertiesBuilder Enabled(bool enabled)
        {
            uploadbox.UploadboxModel.Enabled = enabled;
            return this;
        }
        public UploadboxPropertiesBuilder AsyncUpload()
        {
            uploadbox.UploadboxModel.AsyncUpload = true;
            return this;
        }
        public UploadboxPropertiesBuilder AsyncUpload(bool asyncUpload)
        {
            uploadbox.UploadboxModel.AsyncUpload = asyncUpload;
            return this;
        }
       
        public UploadboxPropertiesBuilder MultipleFilesSelect()
        {
            uploadbox.UploadboxModel.MultipleFilesSelect = true;
            return this;
        }
        public UploadboxPropertiesBuilder MultipleFilesSelect(bool multipleFilesSelect)
        {
            uploadbox.UploadboxModel.MultipleFilesSelect = multipleFilesSelect;
            return this;
        }
        public UploadboxPropertiesBuilder AutoUpload()
        {
            uploadbox.UploadboxModel.AutoUpload = true;
            return this;
        }
        public UploadboxPropertiesBuilder AutoUpload(bool autoUpload)
        {
            uploadbox.UploadboxModel.AutoUpload = autoUpload;
            return this;
        }
        public UploadboxPropertiesBuilder ShowFileDetails()
        {
            uploadbox.UploadboxModel.ShowFileDetails = true;
            return this;
        }
        public UploadboxPropertiesBuilder ShowFileDetails(bool showFileDetails)
        {
            uploadbox.UploadboxModel.ShowFileDetails = showFileDetails;
            return this;
        }
        public UploadboxPropertiesBuilder Rtl()
        {
            uploadbox.UploadboxModel.Rtl = true;
            return this;
        }
        public UploadboxPropertiesBuilder Rtl(bool rtl)
        {
            uploadbox.UploadboxModel.Rtl = rtl;
            return this;
        }
       
        //String Values
        public UploadboxPropertiesBuilder BrowseButtonText(String browseButtonText)
        {
            uploadbox.UploadboxModel.BrowseButtonText = browseButtonText;
            return this;
        }
        public UploadboxPropertiesBuilder UploadButtonText(String uploadButtonText)
        {
            uploadbox.UploadboxModel.UploadButtonText = uploadButtonText;
            return this;
        }
      
        public UploadboxPropertiesBuilder FileFormatAllow(String fileFormatAllow)
        {
            uploadbox.UploadboxModel.FileFormatAllow = fileFormatAllow;
            return this;
        }
        public UploadboxPropertiesBuilder FileFormatDeny(String fileFormatDeny)
        {
            uploadbox.UploadboxModel.FileFormatDeny = fileFormatDeny;
            return this;
        }
        public UploadboxPropertiesBuilder SaveUrl(String saveUrl)
        {
            uploadbox.UploadboxModel.SaveUrl = saveUrl;
            return this;
        }
        public UploadboxPropertiesBuilder RemoveUrl(String removeUrl)
        {
            uploadbox.UploadboxModel.RemoveUrl = removeUrl;
            return this;
        }
        public UploadboxPropertiesBuilder CssClass(String cssClass)
        {
            uploadbox.UploadboxModel.CssClass = cssClass;
            return this;
        }
        //Events
        public UploadboxPropertiesBuilder ClientSideEvents(Action<UploadboxClientSideEventsBuilder> clientSideEvents)
        {
            var builder = new UploadboxClientSideEventsBuilder(this.uploadbox.UploadboxModel);
            if (clientSideEvents != null)
                clientSideEvents.Invoke(builder);
            return this;
        }
        //Render
        public HtmlString Render()
        {
            return new HtmlString(uploadbox.Render().ToString());
        }
        public override String ToString()
        {

            return Render().ToString();
        }
    }
}
