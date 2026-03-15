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
using System.Reflection;
using System.Threading.Tasks;
using System.ComponentModel;
using Syncfusion.JavaScript.DataSources;
using Syncfusion.JavaScript;
using Syncfusion.JavaScript.Shared.Serializer;
using Syncfusion.JavaScript.Shared;
using Syncfusion.JavaScript.Models;


namespace Syncfusion.JavaScript.Models
{
    public class ButtonText
    {
        #region Fields
        private String today = "Today";
        private String now = "Now";
        private String done = "Done";
        #endregion
        #region Properties
        [JsonProperty("today")]
        [DefaultValue("Today")]
        public String Today
        {
            get { return this.today; }
            set { this.today = value; }
        }
        [JsonProperty("now")]
        [DefaultValue("Now")]
        public String Now
        {
            get { return this.now; }
            set { this.now = value; }
        }
        [JsonProperty("done")]
        [DefaultValue("Done")]
        public String Done
        {
            get { return this.done; }
            set { this.done = value; }
        }
        #endregion
    }
}
namespace Syncfusion.JavaScript
{
    
    public class ButtonTextBuilder
    {
        private ButtonText buttonTexts = new ButtonText();
        ButtonText buttonText = new ButtonText();
        DateTimePickerProperties datetimeobj;
     
        public ButtonTextBuilder(DateTimePickerProperties buttonText)
        {
            this.datetimeobj = buttonText;
            this.buttonTexts = buttonText.DateTimePickerButtonText;
            }
        public ButtonTextBuilder Today(String today)
        {
           
            this.buttonText.Today = today;
            return this;
        }
        public ButtonTextBuilder Now(String now)
        {
            this.buttonText.Now = now;
            return this;
        }
        public ButtonTextBuilder Done(String done)
        {
            this.buttonText.Done = done;
            return this;
        }
    }
}
