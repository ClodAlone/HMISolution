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
    public class ButtonPropertiesBuilder
    {
        public Button button;

        public ButtonPropertiesBuilder(Button button)
        { this.button = new Button(button.ID, button.ButtonModel); }
        
        public ButtonPropertiesBuilder()
        {
        }
        //Boolean values
        public ButtonPropertiesBuilder Enabled()
        {
            button.ButtonModel.Enabled = true;
            return this;
        }
        public ButtonPropertiesBuilder Enabled(bool enabled)
        {
            button.ButtonModel.Enabled = enabled;
            return this;
        }
        public ButtonPropertiesBuilder RoundedCorner()
        {
            button.ButtonModel.RoundedCorner = true;
            return this;
        }
        public ButtonPropertiesBuilder RoundedCorner(bool roundedCorner)
        {
            button.ButtonModel.RoundedCorner = roundedCorner;
            return this;
        }
        public ButtonPropertiesBuilder Rtl()
        {
            button.ButtonModel.Rtl = true;
            return this;
        }
        public ButtonPropertiesBuilder Rtl(bool rtl)
        {
            button.ButtonModel.Rtl = rtl;
            return this;
        }
        public ButtonPropertiesBuilder RepeatButton()
        {
            button.ButtonModel.RepeatButton = true;
            return this;
        }
        public ButtonPropertiesBuilder RepeatButton(bool repeatButton)
        {
            button.ButtonModel.RepeatButton = repeatButton;
            return this;
        }
        //EnumValues
        public ButtonPropertiesBuilder Size(ButtonSize size)
        {
            button.ButtonModel.Size = size;
            return this;
        }
        public ButtonPropertiesBuilder ContentType(Contents contentType)
        {
            button.ButtonModel.ContentType = contentType;
            return this;
        }
        public ButtonPropertiesBuilder ImagePosition(ImagePositions imagePosition)
        {
            button.ButtonModel.ImagePosition = imagePosition;
            return this;
        }
        //String Values
        public ButtonPropertiesBuilder Height(String height)
        {
            button.ButtonModel.Height = height;
            return this;
        }
        public ButtonPropertiesBuilder Width(String width)
        {
            button.ButtonModel.Width = width;
            return this;
        }
        public ButtonPropertiesBuilder CssClass(String cssClass)
        {
            button.ButtonModel.CssClass = cssClass;
            return this;
        }
        public ButtonPropertiesBuilder Text(String text)
        {
            button.ButtonModel.Text = text;
            return this;
        }
        public ButtonPropertiesBuilder PrefixIcon(String prefixIcon)
        {
            button.ButtonModel.PrefixIcon = prefixIcon;
            return this;
        }
        public ButtonPropertiesBuilder SuffixIcon(String suffixIcon)
        {
            button.ButtonModel.SuffixIcon = suffixIcon;
            return this;
        }
        //Events
        public ButtonPropertiesBuilder ClientSideEvents(Action<ButtonClientSideEventsBuilder> clientSideEvents)
        {
            var builder = new ButtonClientSideEventsBuilder(this.button.ButtonModel);
            if (clientSideEvents != null)
                clientSideEvents.Invoke(builder);
            return this;
        }
        //Render
        public HtmlString Render()
        {
            return new HtmlString(button.Render().ToString());
        }
        public override String ToString()
        {

            return Render().ToString();
        }
    }
}
