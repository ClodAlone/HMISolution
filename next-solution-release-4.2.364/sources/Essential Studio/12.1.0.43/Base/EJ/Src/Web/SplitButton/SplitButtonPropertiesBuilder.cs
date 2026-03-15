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
   public class SplitButtonPropertiesBuilder
    {
       public SplitButton splitbutton;

        public SplitButtonPropertiesBuilder(SplitButton splitbutton)
        { this.splitbutton = new SplitButton(splitbutton.ID, splitbutton.SplitButtonModel); }
        
        public SplitButtonPropertiesBuilder()
        {
        }
       //Boolean Values
        public SplitButtonPropertiesBuilder Enabled()
        {
            splitbutton.SplitButtonModel.Enabled = true;
            return this;
        }
        public SplitButtonPropertiesBuilder Enabled(bool enabled)
        {
            splitbutton.SplitButtonModel.Enabled = enabled;
            return this;
        }
        public SplitButtonPropertiesBuilder RoundedCorner()
        {
            splitbutton.SplitButtonModel.RoundedCorner = true;
            return this;
        }
        public SplitButtonPropertiesBuilder RoundedCorner(bool roundedCorner)
        {
            splitbutton.SplitButtonModel.RoundedCorner = roundedCorner;
            return this;
        }
        public SplitButtonPropertiesBuilder Rtl()
        {
            splitbutton.SplitButtonModel.Rtl = true;
            return this;
        }
        public SplitButtonPropertiesBuilder Rtl(bool rtl)
        {
            splitbutton.SplitButtonModel.Rtl = rtl;
            return this;
        }

        //EnumValues
        public SplitButtonPropertiesBuilder Size(ButtonSize size)
        {
            splitbutton.SplitButtonModel.Size = size;
            return this;
        }
        public SplitButtonPropertiesBuilder ContentType(Contents contentType)
        {
            splitbutton.SplitButtonModel.ContentType = contentType;
            return this;
        }
        public SplitButtonPropertiesBuilder ImagePosition(ImagePositions imagePosition)
        {
            splitbutton.SplitButtonModel.ImagePosition = imagePosition;
            return this;
        }
        //String Values
        public SplitButtonPropertiesBuilder Height(String height)
        {
            splitbutton.SplitButtonModel.Height = height;
            return this;
        }
        public SplitButtonPropertiesBuilder Width(String width)
        {
            splitbutton.SplitButtonModel.Width = width;
            return this;
        }
        public SplitButtonPropertiesBuilder CssClass(String cssClass)
        {
            splitbutton.SplitButtonModel.CssClass = cssClass;
            return this;
        }
        public SplitButtonPropertiesBuilder Text(String text)
        {
            splitbutton.SplitButtonModel.Text = text;
            return this;
        }
        public SplitButtonPropertiesBuilder PrefixIcon(String prefixIcon)
        {
            splitbutton.SplitButtonModel.PrefixIcon = prefixIcon;
            return this;
        }
        public SplitButtonPropertiesBuilder SuffixIcon(String suffixIcon)
        {
            splitbutton.SplitButtonModel.SuffixIcon = suffixIcon;
            return this;
        }
        public SplitButtonPropertiesBuilder TargetId(String targetId)
        {
            splitbutton.SplitButtonModel.TargetId = targetId;
            return this;
        }
        //Events
        public SplitButtonPropertiesBuilder ClientSideEvents(Action<SplitButtonClientSideEventsBuilder> clientSideEvents)
        {
            var builder = new SplitButtonClientSideEventsBuilder(this.splitbutton.SplitButtonModel);
            if (clientSideEvents != null)
                clientSideEvents.Invoke(builder);
            return this;
        }
        //Render
        public HtmlString Render()
        {
            return new HtmlString(splitbutton.Render().ToString());
        }
        public override String ToString()
        {

            return Render().ToString();
        }
     }
}
