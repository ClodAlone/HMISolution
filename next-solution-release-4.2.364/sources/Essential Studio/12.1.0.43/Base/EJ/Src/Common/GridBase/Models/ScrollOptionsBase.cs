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
namespace Syncfusion.JavaScript.Models
{
    public class ScrollOptionsBase<T> where T : class
    {
        private int width;
        private int height;
      

        [JsonProperty("width")]
        [DefaultValue(0)]
        public int Width
        {
            get { return this.width; }
            set { this.width = value; }
        }
       
        [JsonProperty("height")]
        [DefaultValue(0)]
        public int Height
        {
            get { return this.height; }
            set { this.height = value; }
        }

    }
}
