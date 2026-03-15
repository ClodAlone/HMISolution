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
    public class MobileScrollOptionsBuilder<T> where T : class
    {
        
        private MobileScrollOptions<T> scrollOptions=new MobileScrollOptions<T>();
        public MobileScrollOptionsBuilder(MobileScrollOptions<T> scroll)
        {
            scrollOptions = scroll;
        }
        public MobileScrollOptionsBuilder<T> Width(int width)
        {
            scrollOptions.Width = width;
            return this;
        }
        public MobileScrollOptionsBuilder<T> Height(int height)
        {
            scrollOptions.Height = height;
            return this;
        }
        public MobileScrollOptionsBuilder<T> AllowHorizontalScrolling()
        {
            scrollOptions.AllowHorizontalScrolling = true;
            return this;
        }
        public MobileScrollOptionsBuilder<T> AllowHorizontalScrolling(bool scrolling)
        {
            scrollOptions.AllowHorizontalScrolling = scrolling;
            return this;
        }
        public MobileScrollOptionsBuilder<T> AllowVerticalScrolling()
        {
            scrollOptions.AllowVerticalScrolling = true;
            return this;
        }
        public MobileScrollOptionsBuilder<T> AllowVerticalScrolling(bool scrolling)
        {
            scrollOptions.AllowVerticalScrolling = scrolling;
            return this;
        }
        public MobileScrollOptionsBuilder<T> NativeScrolling()
        {
            scrollOptions.NativeScrolling = true;
            return this;
        }
        public MobileScrollOptionsBuilder<T> NativeScrolling(bool scrolling)
        {
            scrollOptions.NativeScrolling = scrolling;
            return this;
        }
    }
}
