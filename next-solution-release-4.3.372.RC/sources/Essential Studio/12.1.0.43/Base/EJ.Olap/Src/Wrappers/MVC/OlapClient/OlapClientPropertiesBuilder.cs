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
using Syncfusion.JavaScript.Shared;
using System.Web;

namespace Syncfusion.JavaScript.Olap
{
    public class OlapClientPropertiesBuilder
    {
        public OlapClient olapClient;

        public OlapClientPropertiesBuilder(OlapClient olapClient)
        { this.olapClient = new OlapClient(olapClient.ID, olapClient.OlapClientModel); }

        public OlapClientPropertiesBuilder()
        {
        }
        public OlapClientPropertiesBuilder Url(string url)
        {
            this.olapClient.OlapClientModel.Url = url;
            return this;
        }
        public OlapClientPropertiesBuilder Title(string title)
        {
            this.olapClient.OlapClientModel.Title = title;
            return this;
        }
        public OlapClientPropertiesBuilder CssClass(string cssClass)
        {
            this.olapClient.OlapClientModel.CssClass = cssClass;
            return this;
        }
        public OlapClientPropertiesBuilder GridLayout(OlapGridLayout gridLayout)
        {
            this.olapClient.OlapClientModel.GridLayout = gridLayout;
            return this;
        }
        public OlapClientPropertiesBuilder DisplayOptions(Action<OlapClientDisplayOptionsBuilder> displayoptions)
        {
            var displayOptions = new OlapClientDisplayOptions();
            this.olapClient.OlapClientModel.DisplayOptions = displayOptions;
            var builder = new OlapClientDisplayOptionsBuilder(displayOptions);
            if (displayoptions != null)
                displayoptions.Invoke(builder);
            return this;
        }
        public OlapClientPropertiesBuilder ServiceMethods(Action<OlapClientServiceMethodsBuilder> servicemethods)
        {
            var serviceMethods = new OlapClientServiceMethods();
            this.olapClient.OlapClientModel.ServiceMethods = serviceMethods;
            var builder = new OlapClientServiceMethodsBuilder(serviceMethods);
            if (servicemethods != null)
                servicemethods.Invoke(builder);
            return this;
        }
        public OlapClientPropertiesBuilder CustomObject(Dictionary<String, Object> customObject)
        {
            this.olapClient.OlapClientModel.CustomObject = customObject;
            return this;
        }
        //Events
        public OlapClientPropertiesBuilder ClientSideEvents(Action<OlapClientClientSideEventsBuilder> clientSideEvents)
        {
            var builder = new OlapClientClientSideEventsBuilder(this.olapClient.OlapClientModel);
            if (clientSideEvents != null)
                clientSideEvents.Invoke(builder);
            return this;
        }
        //Render 
        public HtmlString Render()
        {
            return new HtmlString(olapClient.Render().ToString());
        }
        public override String ToString()
        {

            return Render().ToString();
        }
    }
}
