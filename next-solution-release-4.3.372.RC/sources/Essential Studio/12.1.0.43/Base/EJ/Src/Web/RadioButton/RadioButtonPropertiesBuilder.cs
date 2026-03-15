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
    public class RadioButtonPropertiesBuilder
    {

        public RadioButton button;

        public RadioButtonPropertiesBuilder(RadioButton button)
        { 
            this.button = new RadioButton(button.ID, button.RadioButtonModel); }
        
        public RadioButtonPropertiesBuilder()
        {
        }
        //Boolean values
        public RadioButtonPropertiesBuilder Enabled()
        {
            button.RadioButtonModel.Enabled = true;
            return this;
        }
        public RadioButtonPropertiesBuilder Enabled(bool enabled)
        {
            button.RadioButtonModel.Enabled = enabled;
            return this;
        }
        public RadioButtonPropertiesBuilder Check()
        {
            button.RadioButtonModel.Check = true;
            return this;
        }
        public RadioButtonPropertiesBuilder Check(bool check)
        {
            button.RadioButtonModel.Check = check;
            return this;
        }
        public RadioButtonPropertiesBuilder Rtl()
        {
            button.RadioButtonModel.Rtl = true;
            return this;
        }
        public RadioButtonPropertiesBuilder Rtl(bool rtl)
        {
            button.RadioButtonModel.Rtl = rtl;
            return this;
        }
        public RadioButtonPropertiesBuilder Persist()
        {
            button.RadioButtonModel.Persist = true;
            return this;
        }
        public RadioButtonPropertiesBuilder Persist(bool persist)
        {
            button.RadioButtonModel.Persist = persist;
            return this;
        }
        //EnumValues
        public RadioButtonPropertiesBuilder Size(RadioButtonSize size)
        {
            button.RadioButtonModel.Size = size;
            return this;
        }
      
        //String Values
        public RadioButtonPropertiesBuilder Id(String id)
        {
            button.RadioButtonModel.Id = id;
            return this;
        }
        public RadioButtonPropertiesBuilder Name(String name)
        {
            button.RadioButtonModel.Name = name;
            return this;
        }
        public RadioButtonPropertiesBuilder CssClass(String cssClass)
        {
            button.RadioButtonModel.CssClass = cssClass;
            return this;
        }
        public RadioButtonPropertiesBuilder Text(String text)
        {
            button.RadioButtonModel.Text = text;
            return this;
        }
        public RadioButtonPropertiesBuilder IdPrefix(String idPrefix)
        {
            button.RadioButtonModel.IdPrefix = idPrefix;
            return this;
        }
        public RadioButtonPropertiesBuilder Value(String value)
        {
            button.RadioButtonModel.Value = value;
            return this;
        }
        //Events
        public RadioButtonPropertiesBuilder ClientSideEvents(Action<RadioButtonClientSideEventsBuilder> clientSideEvents)
        {
            var builder = new RadioButtonClientSideEventsBuilder(this.button.RadioButtonModel);
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
