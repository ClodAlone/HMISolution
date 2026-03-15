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
    public class ScrollerPropertiesBuilder
    {
        public Scroller scroller;

        public ScrollerPropertiesBuilder(Scroller scroller)
        { this.scroller = new Scroller(scroller.ID, scroller.ScrollerModel); }
        
        public ScrollerPropertiesBuilder()
        {
        }
        //Boolean values
        public ScrollerPropertiesBuilder Persist()
        {
            scroller.ScrollerModel.Persist = false;
            return this;
        }        
        
        //String Values
        public ScrollerPropertiesBuilder Rtl(String rtl)
        {
            scroller.ScrollerModel.Rtl = rtl;
            return this;
        }
		
		//Integer values
        public ScrollerPropertiesBuilder Width(int width)
        {
            scroller.ScrollerModel.Width = width;
            return this;
        }
        public ScrollerPropertiesBuilder Height(int height)
        {
            scroller.ScrollerModel.Height = height;
            return this;
        }
        public ScrollerPropertiesBuilder OneStep(int oneStep)
        {
            scroller.ScrollerModel.OneStep = oneStep;
            return this;
        }
        public ScrollerPropertiesBuilder ButtonSize(int buttonSize)
        {
            scroller.ScrollerModel.ButtonSize = buttonSize;
            return this;
        }
        public ScrollerPropertiesBuilder ScrollLeft(int scrollLeft)
        {
            scroller.ScrollerModel.ScrollLeft = scrollLeft;
            return this;
        }
		 public ScrollerPropertiesBuilder ScrollTop(int scrollTop)
        {
            scroller.ScrollerModel.ScrollTop = scrollTop;
            return this;
        }
        public ScrollerPropertiesBuilder ScrollerSize(int scrollerSize)
        {
            scroller.ScrollerModel.ScrollerSize = scrollerSize;
            return this;
        }        
        //Render
        public HtmlString Render()
        {
            return new HtmlString(scroller.Render().ToString());
        }
        public override String ToString()
        {

            return Render().ToString();
        }
    }
}
