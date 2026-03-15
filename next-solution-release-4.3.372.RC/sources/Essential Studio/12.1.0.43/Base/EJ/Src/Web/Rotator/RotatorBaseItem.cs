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
using Syncfusion.JavaScript;
using System.Web;
using System.Web.Script.Serialization;
using System.IO;
using Syncfusion.JavaScript.Models;

namespace Syncfusion.JavaScript.Models
{
    public class RotatorBaseItem
    {
        private MvcTemplate<RotatorBaseItem> template = null;

       
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AccordionBaseItem"/> class.
        /// </summary>
        public RotatorBaseItem()
        {

            this.Caption = string.Empty;
            this.Url = string.Empty;
            this.ContentTemplate = new MvcTemplate<RotatorBaseItem>();
        }
        #endregion

        #region Properties


        public string Caption { get; set; }

        public string Url { get; set; }


        #endregion
        /// <summary>
        /// Gets or sets the content template.
        /// </summary>
        /// <value>The content template.</value>
        [ScriptIgnore]
        public MvcTemplate<RotatorBaseItem> ContentTemplate
        {
            get;
            set;
        }
    }
}
namespace Syncfusion.JavaScript
{
  


    public class RotatorBaseItemBuilder
    {
        #region Constructor
        internal RotatorBaseItem Item { get; set; }

        public RotatorBaseItemBuilder(RotatorBaseItem item)
        {
            this.Item = item;
        }
        #endregion

        public RotatorBaseItemBuilder Caption(string caption)
        {
            this.Item.Caption = caption;
            return this;
        }
        public RotatorBaseItemBuilder Url(string url)
        {
            this.Item.Url= url;
            return this;
        }

        public RotatorBaseItemBuilder ContentTemplate(Action<RotatorBaseItem> contentTemplate)
        {
            this.Item.ContentTemplate.WebFormDataTemplate = contentTemplate;
            return this;
        }

        public RotatorBaseItemBuilder ContentTemplate(Func<RotatorBaseItem, object> contentTemplate)
        {
            this.Item.ContentTemplate.RazorViewTemplate = contentTemplate;
            return this;
        }


    }

    public class RotatorBaseItemAdder
    {

        private List<RotatorBaseItem> ItemList;
        #region Constructor

        public RotatorBaseItemAdder(List<RotatorBaseItem> itemList)
        {
            this.ItemList = itemList;
        }

        #endregion

        public RotatorBaseItemBuilder Add()
        {
            RotatorBaseItem newTab = new RotatorBaseItem();
            this.ItemList.Add(newTab);
            return new RotatorBaseItemBuilder(newTab);
        }
    }

}
