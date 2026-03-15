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
using Syncfusion.JavaScript.Mobile.Models;
namespace Syncfusion.JavaScript
{
    public class MobilePageOptionsBuilder<T> where T : class
    {

        private MobilePageOptions<T> pageOption = new MobilePageOptions<T>();
        public MobilePageOptionsBuilder(MobilePageOptions<T> page)
        {
            pageOption = page;
        }
        public MobilePageOptionsBuilder<T> PageSize(int pageSize)
        {
            pageOption.PageSize = pageSize;
            return this;
        }
        public MobilePageOptionsBuilder<T> CurrentPage(int currentPage)
        {
            pageOption.CurrentPage = currentPage;
            return this;
        }

        public MobilePageOptionsBuilder<T> TotalRecordsCount(int totalRecordsCount)
        {
            pageOption.TotalRecordsCount = totalRecordsCount;
            return this;
        }
        public MobilePageOptionsBuilder<T> Display(PagerDisplay display)
        {
            pageOption.Display = display;
            return this;
        }
        public MobilePageOptionsBuilder<T> Type(PagerType type)
        {
            pageOption.Type = type;
            return this;
        }
    }
}
