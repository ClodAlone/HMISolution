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
    public class WaitingPopupPropertiesBuilder
    {
        public WaitingPopup waitingPopUp;

        public WaitingPopupPropertiesBuilder(WaitingPopup waitingPopUp)
        { this.waitingPopUp = new WaitingPopup(waitingPopUp.ID, waitingPopUp.WaitingPopUpModel); }

        public WaitingPopupPropertiesBuilder()
        {
        }

        //Boolean values
        public WaitingPopupPropertiesBuilder AutoDisplay()
        {
            waitingPopUp.WaitingPopUpModel.AutoDisplay = true;
            return this;
        }
        public WaitingPopupPropertiesBuilder AutoDisplay(bool autoDisplay)
        {
            waitingPopUp.WaitingPopUpModel.AutoDisplay = autoDisplay;
            return this;
        }
        public WaitingPopupPropertiesBuilder ShowImage()
        {
            waitingPopUp.WaitingPopUpModel.ShowImage = true;
            return this;
        }
        public WaitingPopupPropertiesBuilder ShowImage(bool showImage)
        {
            waitingPopUp.WaitingPopUpModel.ShowImage = showImage;
            return this;
        }

        //String Values
        public WaitingPopupPropertiesBuilder Text(String text)
        {
            waitingPopUp.WaitingPopUpModel.Text = text;
            return this;
        }
        public WaitingPopupPropertiesBuilder CssClass(String cssClass)
        {
            waitingPopUp.WaitingPopUpModel.CssClass = cssClass;
            return this;
        }
        public WaitingPopupPropertiesBuilder Template(String template)
        {
            waitingPopUp.WaitingPopUpModel.Template = template;
            return this;
        }
        //Events
        public WaitingPopupPropertiesBuilder ClientSideEvents(Action<WaitingPopupClientSideEventsBuilder> clientSideEvents)
        {
            var builder = new WaitingPopupClientSideEventsBuilder(this.waitingPopUp.WaitingPopUpModel);
            if (clientSideEvents != null)
                clientSideEvents.Invoke(builder);
            return this;
        }
        //Render
        public HtmlString Render()
        {
            return new HtmlString(waitingPopUp.Render().ToString());
        }
        public override String ToString()
        {

            return Render().ToString();
        }
    }
}
