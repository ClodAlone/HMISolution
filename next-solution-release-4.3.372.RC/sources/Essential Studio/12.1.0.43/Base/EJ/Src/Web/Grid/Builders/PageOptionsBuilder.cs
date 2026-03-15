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
using Syncfusion.JavaScript.Models;
namespace Syncfusion.JavaScript
{
    public class PageOptionsBuilder<T> where T : class
    {

        private PageOptions<T> pageOption = new PageOptions<T>();
        public PageOptionsBuilder(PageOptions<T> page)
        {
            pageOption = page;
        }
        public PageOptionsBuilder<T> PageSize(int pageSize)
        {
            pageOption.PageSize = pageSize;
            return this;
        }
        public PageOptionsBuilder<T> PageCount(int pageCount)
        {
            pageOption.PageCount = pageCount;
            return this;
        }
        public PageOptionsBuilder<T> CurrentPage(int currentPage)
        {
            pageOption.CurrentPage = currentPage;
            return this;
        }
        public PageOptionsBuilder<T> TotalPages(int totalPages)
        {
            pageOption.TotalPages = totalPages;
            return this;
        }
        public PageOptionsBuilder<T> Localization(String localization)
        {
            pageOption.Localization = localization;
            return this;
        }
        public PageOptionsBuilder<T> TotalRecordsCount(int totalRecordsCount)
        {
            pageOption.TotalRecordsCount = totalRecordsCount;
            return this;
        }
        public PageOptionsBuilder<T> EnableQueryString()
        {
            pageOption.EnableQueryString = true;
            return this;
        }
        public PageOptionsBuilder<T> EnableQueryString(bool enableQueryString)
        {
            pageOption.EnableQueryString = enableQueryString;
            return this;
        }
    }
}
