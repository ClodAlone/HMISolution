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
    public class PageOptions<T> where T : class
    {
        private int pageSize = 12;
        private int currentPage = 1;
        private int totalPages = 0;
        private int pageCount = 8;
        private String localization = "EN-US";
        private int totalRecordsCount = 0;
        private bool enableQueryString = false;

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
        [JsonProperty("totalPages")]
        [DefaultValue(0)]
        public int TotalPages
        {
            get { return this.totalPages; }
            set { this.totalPages = value; }
        }
        [JsonProperty("pageCount")]
        [DefaultValue(8)]
        public int PageCount
        {
            get { return this.pageCount; }
            set { this.pageCount = value; }
        }
        [JsonProperty("localization")]
        [DefaultValue("EN-US")]
        public String Localization
        {
            get { return this.localization; }
            set { this.localization = value; }
        }
        [JsonProperty("totalRecordsCount")]
        [DefaultValue(0)]
        public int TotalRecordsCount
        {
            get { return this.totalRecordsCount; }
            set { this.totalRecordsCount = value; }
        }
        [JsonProperty("enableQueryString")]
        [DefaultValue(false)]
        public bool EnableQueryString
        {
            get { return this.enableQueryString; }
            set { this.enableQueryString = value; }
        }
    }
}
