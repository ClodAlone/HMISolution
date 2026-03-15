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
using Syncfusion.JavaScript.Olap.Models;

namespace Syncfusion.JavaScript.Olap
{
    public class OlapGridPropertiesBuilder
    {
        public OlapGrid olapGrid;

        public OlapGridPropertiesBuilder(OlapGrid olapGrid)
        { this.olapGrid = new OlapGrid(olapGrid.ID, olapGrid.OlapGridModel); }

        public OlapGridPropertiesBuilder()
        {
        }
        public OlapGridPropertiesBuilder Url(string url)
        {
            this.olapGrid.OlapGridModel.Url = url;
            return this;
        }
        public OlapGridPropertiesBuilder CssClass(string cssClass)
        {
            this.olapGrid.OlapGridModel.CssClass = cssClass;
            return this;
        }
        public OlapGridPropertiesBuilder GridLayout(OlapGridLayout gridLayout)
        {
            this.olapGrid.OlapGridModel.GridLayout = gridLayout;
            return this;
        }
        public OlapGridPropertiesBuilder EnableCellContext(bool enableCellContext)
        {
            this.olapGrid.OlapGridModel.EnableCellContext = enableCellContext;
            return this;
        }
        public OlapGridPropertiesBuilder EnableValueCellHyperlink(bool enableValueCellHyperlink)
        {
            this.olapGrid.OlapGridModel.EnableValueCellHyperlink = enableValueCellHyperlink;
            return this;
        }
        public OlapGridPropertiesBuilder EnableRowHeaderHyperlink(bool enableRowHeaderHyperlink)
        {
            this.olapGrid.OlapGridModel.EnableRowHeaderHyperlink = enableRowHeaderHyperlink;
            return this;
        }
        public OlapGridPropertiesBuilder EnableColumnHeaderHyperlink(bool enableColumnHeaderHyperlink)
        {
            this.olapGrid.OlapGridModel.EnableColumnHeaderHyperlink = enableColumnHeaderHyperlink;
            return this;
        }
        public OlapGridPropertiesBuilder EnableSummaryCellHyperlink(bool enableSummaryCellHyperlink)
        {
            this.olapGrid.OlapGridModel.EnableSummaryCellHyperlink = enableSummaryCellHyperlink;
            return this;
        }
        public OlapGridPropertiesBuilder EnableVirtualScrolling(bool enableVirtualScrolling)
        {
            this.olapGrid.OlapGridModel.EnableVirtualScrolling = enableVirtualScrolling;
            return this;
        }
        public OlapGridPropertiesBuilder ProgressMode(ProgressMode progressMode)
        {
            this.olapGrid.OlapGridModel.ProgressMode = progressMode;
            return this;
        }
        public OlapGridPropertiesBuilder CustomObject(Dictionary<string, object> customObject)
        {
            this.olapGrid.OlapGridModel.CustomObject = customObject;
            return this;
        }
        public OlapGridPropertiesBuilder serviceMethods(Action<OlapGridServiceMethodsBuilder> servicemethods)
        {
            var serviceMethods = new OlapGridServiceMethods();
            this.olapGrid.OlapGridModel.ServiceMethods = serviceMethods;
            var builder = new OlapGridServiceMethodsBuilder(serviceMethods);
            if (servicemethods != null)
                servicemethods.Invoke(builder);
            return this;
        }
        //Events
        public OlapGridPropertiesBuilder ClientSideEvents(Action<OlapGridClientSideEventsBuilder> clientSideEvents)
        {
            var builder = new OlapGridClientSideEventsBuilder(this.olapGrid.OlapGridModel);
            if (clientSideEvents != null)
                clientSideEvents.Invoke(builder);
            return this;
        }
        //Render 
        public HtmlString Render()
        {
            return new HtmlString(olapGrid.Render().ToString());
        }
        public override String ToString()
        {

            return Render().ToString();
        }
    }
}
