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
using System.Threading.Tasks;
using System.ComponentModel;
using Syncfusion.JavaScript.Shared.Serializer;
using Syncfusion.JavaScript.Models;
namespace Syncfusion.JavaScript.Mobile.Models
{
    public class MobileScrollOptions<T> : ScrollOptionsBase<T> where T : class
    {

        private bool verscrolling = true;
        private bool horscrolling = false;
        private bool nativescroll = true;

        [JsonProperty("allowHorizontalScrolling")]
        [DefaultValue(false)]
        public bool AllowHorizontalScrolling
        {
            get { return this.horscrolling; }
            set { this.horscrolling = value; }
        }

        [JsonProperty("allowVerticalScrolling")]
        [DefaultValue(true)]
        public bool AllowVerticalScrolling
        {
            get { return this.verscrolling; }
            set { this.verscrolling = value; }
        }
        [JsonProperty("nativeScrolling")]
        [DefaultValue(true)]
        public bool NativeScrolling
        {
            get { return this.nativescroll; }
            set { this.nativescroll = value; }
        }
    }
}
