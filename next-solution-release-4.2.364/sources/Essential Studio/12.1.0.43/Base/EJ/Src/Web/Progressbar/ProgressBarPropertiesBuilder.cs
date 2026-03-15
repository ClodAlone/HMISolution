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
    public class ProgressBarPropertiesBuilder
    {
        public ProgressBar progressBar;

        public ProgressBarPropertiesBuilder(ProgressBar progressBar)
        { this.progressBar = new ProgressBar(progressBar.ID, progressBar.ProgressBarModel); }
        
        public ProgressBarPropertiesBuilder()
        {
        }
        //int  Values
        public ProgressBarPropertiesBuilder Min(int min)
        {
            progressBar.ProgressBarModel.Min = min;
            return this;
        }
        public ProgressBarPropertiesBuilder Max(int max)
        {
            progressBar.ProgressBarModel.Max = max;
            return this;
        }
        public ProgressBarPropertiesBuilder Value(int value)
        {
            progressBar.ProgressBarModel.Value = value;
            return this;
        }
        public ProgressBarPropertiesBuilder Percentage(int percentage)
        {
            progressBar.ProgressBarModel.Percentage = percentage;
            return this;
        }
        //Boolean Values
        public ProgressBarPropertiesBuilder Enabled()
        {
            progressBar.ProgressBarModel.Enabled = true;
            return this;
        }
        public ProgressBarPropertiesBuilder Enabled(bool enabled)
        {
            progressBar.ProgressBarModel.Enabled = enabled;
            return this;
        }
        public ProgressBarPropertiesBuilder Persist()
        {
            progressBar.ProgressBarModel.Persist = true;
            return this;
        }
        public ProgressBarPropertiesBuilder Persist(bool persist)
        {
            progressBar.ProgressBarModel.Persist = persist;
            return this;
        }
        public ProgressBarPropertiesBuilder Rtl()
        {
            progressBar.ProgressBarModel.Rtl = true;
            return this;
        }
        public ProgressBarPropertiesBuilder Rtl(bool rtl)
        {
            progressBar.ProgressBarModel.Rtl = rtl;
            return this;
        }
        //String Values
        public ProgressBarPropertiesBuilder Width(String width)
        {
            progressBar.ProgressBarModel.Width = width;
            return this;
        }
        public ProgressBarPropertiesBuilder Height(String height)
        {
            progressBar.ProgressBarModel.Height = height;
            return this;
        }
        public ProgressBarPropertiesBuilder CssClass(String cssClass)
        {
            progressBar.ProgressBarModel.CssClass = cssClass;
            return this;
        }
        public ProgressBarPropertiesBuilder Text(String text)
        {
            progressBar.ProgressBarModel.Text = text;
            return this;
        }
        //Events
        public ProgressBarPropertiesBuilder ClientSideEvents(Action<ProgressBarClientSideEventsBuilder> clientSideEvents)
        {
            var builder = new ProgressBarClientSideEventsBuilder(this.progressBar.ProgressBarModel);
            if (clientSideEvents != null)
                clientSideEvents.Invoke(builder);
            return this;
        }
        //Render
        public HtmlString Render()
        {
            return new HtmlString(progressBar.Render().ToString());
        }
        public override String ToString()
        {

            return Render().ToString();
        }
    }
}
