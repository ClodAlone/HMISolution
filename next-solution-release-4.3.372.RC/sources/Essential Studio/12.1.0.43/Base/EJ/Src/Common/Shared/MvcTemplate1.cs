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
using System.Web;
using Syncfusion.JavaScript;
using System.IO;

namespace Syncfusion.JavaScript
{

    public class MvcTemplate<T> where T : class
    {
        private Action<T> webFormTemplate = null;

        private Func<T, object> razorViewTemplate = null;
        public Action<T, HtmlTag> builder;
        
        public Action<T> WebFormDataTemplate
        {
            get { return this.webFormTemplate; }
            set
            {
                this.webFormTemplate = value;
                if (value != null)
                {
                    builder = (data, element) => element.AddTemplate((tw) => WebFormDataTemplate(data));
                }
            }
        }
        
        public Func<T, object> RazorViewTemplate
        {
            get
            {
                return this.razorViewTemplate;
            }
            set
            {
                razorViewTemplate = value;
                builder = (data, element) => element.AddTemplate((tw) =>
                {
                    var result = RazorViewTemplate(data);

                    var helperResult = result as IHtmlString;
                    if (helperResult != null)
                    {
                        tw.Write(helperResult.ToString());
                        return;
                    }

                    if (result != null)
                    {
                        tw.Write(result.ToString());
                    }
                   
                });
            }
        }


    }
}
