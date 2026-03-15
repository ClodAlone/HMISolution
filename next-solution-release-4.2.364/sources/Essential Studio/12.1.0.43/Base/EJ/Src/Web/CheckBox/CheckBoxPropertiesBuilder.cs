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
    public class CheckBoxPropertiesBuilder
    {
        public CheckBox checkbox;

        public CheckBoxPropertiesBuilder(CheckBox checkbox)
        { this.checkbox = new CheckBox(checkbox.ID, checkbox.CheckBoxModel); }
        public CheckBoxPropertiesBuilder()
        {
        }
        //Boolean values
        public CheckBoxPropertiesBuilder Enabled()
        {
            checkbox.CheckBoxModel.Enabled = true;
            return this;
        }
        public CheckBoxPropertiesBuilder Enabled(bool enabled)
        {
            checkbox.CheckBoxModel.Enabled = enabled;
            return this;
        }
        public CheckBoxPropertiesBuilder Indeterminate()
        {
            checkbox.CheckBoxModel.Indeterminate = true;
            return this;
        }
        public CheckBoxPropertiesBuilder Indeterminate(bool Indeterminate)
        {
            checkbox.CheckBoxModel.Indeterminate = Indeterminate;
            return this;
        }
        public CheckBoxPropertiesBuilder IndeterminateState()
        {
            checkbox.CheckBoxModel.IndeterminateState = true;
            return this;
        }
        public CheckBoxPropertiesBuilder IndeterminateState(bool indeterminateState)
        {
            checkbox.CheckBoxModel.IndeterminateState = indeterminateState;
            return this;
        }
        public CheckBoxPropertiesBuilder RoundedCorner()
        {
            checkbox.CheckBoxModel.RoundedCorner = true;
            return this;
        }
        public CheckBoxPropertiesBuilder RoundedCorner(bool roundedCorner)
        {
            checkbox.CheckBoxModel.RoundedCorner = roundedCorner;
            return this;
        }

        public CheckBoxPropertiesBuilder Persist()
        {
            checkbox.CheckBoxModel.Persist = true;
            return this;
        }
        public CheckBoxPropertiesBuilder Persist(bool persist)
        {
            checkbox.CheckBoxModel.Persist = persist;
            return this;
        }
        public CheckBoxPropertiesBuilder Rtl()
        {
            checkbox.CheckBoxModel.Rtl = true;
            return this;
        }
        public CheckBoxPropertiesBuilder Rtl(bool rtl)
        {
            checkbox.CheckBoxModel.Rtl = rtl;
            return this;
        }
        public CheckBoxPropertiesBuilder Check()
        {
            checkbox.CheckBoxModel.Check = true;
            return this;
        }
        public CheckBoxPropertiesBuilder Check(bool check)
        {
            checkbox.CheckBoxModel.Check = check;
            return this;
        }
        //string values
        public CheckBoxPropertiesBuilder Id(String id)
        {
            checkbox.CheckBoxModel.Id = id;
            return this;
        }
        public CheckBoxPropertiesBuilder Name(String name)
        {
            checkbox.CheckBoxModel.Name = name;
            return this;
        }
        public CheckBoxPropertiesBuilder Value(String value)
        {
            checkbox.CheckBoxModel.Value = value;
            return this;
        }
        public CheckBoxPropertiesBuilder CssClass(String cssClass)
        {
            checkbox.CheckBoxModel.CssClass = cssClass;
            return this;
        }
        public CheckBoxPropertiesBuilder Text(String text)
        {
            checkbox.CheckBoxModel.Text = text;
            return this;
        }
        public CheckBoxPropertiesBuilder IdPrefix(String idPrefix)
        {
            checkbox.CheckBoxModel.IdPrefix = idPrefix;
            return this;
        }
        //Enum
        public CheckBoxPropertiesBuilder Size(Size size)
        {
            checkbox.CheckBoxModel.Size = size;
            return this;
        }
        //Events
        public CheckBoxPropertiesBuilder ClientSideEvents(Action<CheckBoxClientSideEventsBuilder> clientSideEvents)
        {
            var builder = new CheckBoxClientSideEventsBuilder(this.checkbox.CheckBoxModel);
            if (clientSideEvents != null)
                clientSideEvents.Invoke(builder);
            return this;
        }
        //Render
        public HtmlString Render()
        {
            return new HtmlString(checkbox.Render().ToString());
        }
        public override String ToString()
        {

            return Render().ToString();
        }
    }
}
