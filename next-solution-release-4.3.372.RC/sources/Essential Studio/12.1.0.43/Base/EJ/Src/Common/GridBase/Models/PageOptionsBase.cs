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
    public class PageOptionsBase<T> where T : class
    {
        private int pageSize = 12;
        private int currentPage = 1;
        private int totalRecordsCount = 0;
       
        //Properties
        [JsonProperty("pageSize")]
        [DefaultValue(12)]
        public int PageSize
        {
            get { return this.pageSize; }
            set { this.pageSize = value; }
        }
        [JsonProperty("currentPage")]
        [DefaultValue(1)]
        public int CurrentPage
        {
            get { return this.currentPage; }
            set { this.currentPage = value; }
        }

        [JsonProperty("totalRecordsCount")]
        [DefaultValue(0)]
        public int TotalRecordsCount
        {
            get { return this.totalRecordsCount; }
            set { this.totalRecordsCount = value; }
        }
    }
}
