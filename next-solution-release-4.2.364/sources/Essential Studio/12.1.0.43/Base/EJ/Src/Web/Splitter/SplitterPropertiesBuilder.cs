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
using Syncfusion.JavaScript.Models;


namespace Syncfusion.JavaScript
{
    public class SplitterPropertiesBuilder
    {
        public Splitter splitter;

        public SplitterPropertiesBuilder(Splitter splitter)
        { this.splitter = new Splitter(splitter.ID, splitter.SplitterModel); }

        public SplitterPropertiesBuilder()
        {
        }
        //Boolean values
        public SplitterPropertiesBuilder WindowResizing()
        {
            splitter.SplitterModel.WindowResizing = true;
            return this;
        }
        public SplitterPropertiesBuilder WindowResizing(bool windowResizing)
        {
            splitter.SplitterModel.WindowResizing = windowResizing;
            return this;
        }
        public SplitterPropertiesBuilder Rtl()
        {
            splitter.SplitterModel.Rtl = true;
            return this;
        }
        public SplitterPropertiesBuilder Rtl(bool rtl)
        {
            splitter.SplitterModel.Rtl = rtl;
            return this;
        }
        
        //EnumValues
        public SplitterPropertiesBuilder Orientation(Orientation orientation)
        {
            splitter.SplitterModel.Orientation = orientation;
            return this;
        }

               
        //String Values
        public SplitterPropertiesBuilder Height(String height)
        {
            splitter.SplitterModel.Height = height;
            return this;
        }
        public SplitterPropertiesBuilder Width(String width)
        {
            splitter.SplitterModel.Width = width;
            return this;
        }
        public SplitterPropertiesBuilder CssClass(String cssClass)
        {
            splitter.SplitterModel.CssClass = cssClass;
            return this;
        }
        //integer
        public SplitterPropertiesBuilder AnimationSpeed(int animationSpeed)
        {
            splitter.SplitterModel.AnimationSpeed = animationSpeed;
            return this;
        }

        public SplitterPropertiesBuilder PaneProperties(Action<PanePropertiesAdder> properties)
        {
            this.ItemsCollection = new List<PaneProperties>();
            PanePropertiesAdder splitterAdded = new PanePropertiesAdder(this.splitter.SplitterModel.PaneProperties);
            properties.Invoke(splitterAdded);
            return this as SplitterPropertiesBuilder;
        }
        //Events
        public SplitterPropertiesBuilder ClientSideEvents(Action<SplitterClientSideEventsBuilder> clientSideEvents)
        {
            var builder = new SplitterClientSideEventsBuilder(this.splitter.SplitterModel);
            if (clientSideEvents != null)
                clientSideEvents.Invoke(builder);
            return this;
        }
        //Render
        public HtmlString Render()
        {
            return new HtmlString(splitter.Render().ToString());
        }
        public override String ToString()
        {

            return Render().ToString();
        }
        public List<PaneProperties> ItemsCollection { get; set; }
    }
}
