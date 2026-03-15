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
    public class ToggleButtonPropertiesBuilder
    {
         public ToggleButton togglebutton;

        public ToggleButtonPropertiesBuilder(ToggleButton togglebutton)
         { this.togglebutton = new ToggleButton(togglebutton.ID, togglebutton.ToggleButtonModel); }

        public ToggleButtonPropertiesBuilder()
        {
        }
       //Boolean Values
        public ToggleButtonPropertiesBuilder Enabled()
        {
            togglebutton.ToggleButtonModel.Enabled = true;
            return this;
        }
        public ToggleButtonPropertiesBuilder Enabled(bool enabled)
        {
            togglebutton.ToggleButtonModel.Enabled = enabled;
            return this;
        }
        public ToggleButtonPropertiesBuilder CheckedStatus()
        {
            togglebutton.ToggleButtonModel.CheckedStatus = true;
            return this;
        }
        public ToggleButtonPropertiesBuilder CheckedStatus(bool checkedStatus)
        {
            togglebutton.ToggleButtonModel.CheckedStatus = checkedStatus;
            return this;
        }
        public ToggleButtonPropertiesBuilder RoundedCorner()
        {
            togglebutton.ToggleButtonModel.RoundedCorner = true;
            return this;
        }
        public ToggleButtonPropertiesBuilder RoundedCorner(bool roundedCorner)
        {
            togglebutton.ToggleButtonModel.RoundedCorner = roundedCorner;
            return this;
        }
        public ToggleButtonPropertiesBuilder Persist()
        {
            togglebutton.ToggleButtonModel.Persist = true;
            return this;
        }
        public ToggleButtonPropertiesBuilder Persist(bool persist)
        {
            togglebutton.ToggleButtonModel.Persist = persist;
            return this;
        }
        public ToggleButtonPropertiesBuilder Rtl()
        {
            togglebutton.ToggleButtonModel.Rtl = true;
            return this;
        }
        public ToggleButtonPropertiesBuilder Rtl(bool rtl)
        {
            togglebutton.ToggleButtonModel.Rtl = rtl;
            return this;
        }

        //EnumValues
        public ToggleButtonPropertiesBuilder Size(ButtonSize size)
        {
            togglebutton.ToggleButtonModel.Size = size;
            return this;
        }
        public ToggleButtonPropertiesBuilder ContentType(Contents contentType)
        {
            togglebutton.ToggleButtonModel.ContentType = contentType;
            return this;
        }
        public ToggleButtonPropertiesBuilder ImagePosition(ImagePositions imagePosition)
        {
            togglebutton.ToggleButtonModel.ImagePosition = imagePosition;
            return this;
        }
        //string values
        public ToggleButtonPropertiesBuilder Height(String height)
        {
            togglebutton.ToggleButtonModel.Height = height;
            return this;
        }
        public ToggleButtonPropertiesBuilder Width(String width)
        {
            togglebutton.ToggleButtonModel.Width = width;
            return this;
        }
        public ToggleButtonPropertiesBuilder DefaultText(String defaultText)
        {
            togglebutton.ToggleButtonModel.DefaultText = defaultText;
            return this;
        }
        public ToggleButtonPropertiesBuilder ActiveText(String activeText)
        {
            togglebutton.ToggleButtonModel.ActiveText = activeText;
            return this;
        }
        public ToggleButtonPropertiesBuilder DefaultPrefixIcon(String defaultPrefixIcon)
        {
            togglebutton.ToggleButtonModel.DefaultPrefixIcon = defaultPrefixIcon;
            return this;
        }
        public ToggleButtonPropertiesBuilder DefaultSuffixIcon(String defaultSuffixIcon)
        {
            togglebutton.ToggleButtonModel.DefaultSuffixIcon = defaultSuffixIcon;
            return this;
        }
        public ToggleButtonPropertiesBuilder ActivePrefixIcon(String activePrefixIcon)
        {
            togglebutton.ToggleButtonModel.ActivePrefixIcon = activePrefixIcon;
            return this;
        }
        public ToggleButtonPropertiesBuilder ActiveSuffixIcon(String activeSuffixIcon)
        {
            togglebutton.ToggleButtonModel.ActiveSuffixIcon = activeSuffixIcon;
            return this;
        }
        public ToggleButtonPropertiesBuilder CssClass(String cssClass)
        {
            togglebutton.ToggleButtonModel.CssClass = cssClass;
            return this;
        }
        //Events
        public ToggleButtonPropertiesBuilder ClientSideEvents(Action<ToggleButtonClientSideEventsBuilder> clientSideEvents)
        {
            var builder = new ToggleButtonClientSideEventsBuilder(this.togglebutton.ToggleButtonModel);
            if (clientSideEvents != null)
                clientSideEvents.Invoke(builder);
            return this;
        }
        //Render
        public HtmlString Render()
        {
            return new HtmlString(togglebutton.Render().ToString());
        }
        public override String ToString()
        {

            return Render().ToString();
        }
    }
}
