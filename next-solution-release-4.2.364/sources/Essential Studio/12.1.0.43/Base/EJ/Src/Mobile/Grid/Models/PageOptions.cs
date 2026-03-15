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
    public class MobilePageOptions<T> : PageOptionsBase<T> where T : class
    {
        #region private fields


        private PagerDisplay display = PagerDisplay.Normal;
        private PagerType type = PagerType.Scrollbale;

        #endregion private fields

        #region properrties

        [JsonProperty("display")]
        [DefaultValue(PagerDisplay.Normal)]
        [JsonConverter(typeof(StringEnumConverter))]
        public PagerDisplay Display
        {
            get { return this.display; }
            set { this.display = value; }
        }
        [JsonProperty("display")]
        [DefaultValue(PagerType.Scrollbale)]
        [JsonConverter(typeof(StringEnumConverter))]
        public PagerType Type
        {
            get { return this.type; }
            set { this.type = value; }
        }

        #endregion properrties

    }
}
