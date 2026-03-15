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
    public class TabBaseItem
    {
        private MvcTemplate<TabBaseItem> template = null;


        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AccordionBaseItem"/> class.
        /// </summary>
        public TabBaseItem()
        {
            this.ID = string.Empty;
            this.Text = string.Empty;
            this.ContentTemplate = new MvcTemplate<TabBaseItem>();
        }
        #endregion

        #region Properties

        public string ID { get; set; }

        public string Text { get; set; }


        #endregion
        /// <summary>
        /// Gets or sets the content template.
        /// </summary>
        /// <value>The content template.</value>
        [ScriptIgnore]
        public MvcTemplate<TabBaseItem> ContentTemplate { get; set; }
    }
}

namespace Syncfusion.JavaScript
{

    public class TabBaseItemBuilder
    {
        #region Constructor
        internal TabBaseItem Item { get; set; }

        public TabBaseItemBuilder(TabBaseItem item)
        {
            this.Item = item;
        }
        #endregion


        public TabBaseItemBuilder ID(string id)
        {
            this.Item.ID = id;
            return this;
        }

        public TabBaseItemBuilder Text(string text)
        {
            this.Item.Text = text;
            return this;
        }

        public TabBaseItemBuilder ContentTemplate(Action<TabBaseItem> contentTemplate)
        {
            this.Item.ContentTemplate.WebFormDataTemplate = contentTemplate;
            return this;
        }

        public TabBaseItemBuilder ContentTemplate(Func<TabBaseItem, object> contentTemplate)
        {
            this.Item.ContentTemplate.RazorViewTemplate = contentTemplate;
            return this;
        }


    }

    public class TabBaseItemAdder
    {

        private List<TabBaseItem> ItemList;
        #region Constructor

        public TabBaseItemAdder(List<TabBaseItem> itemList)
        {
            this.ItemList = itemList;
        }

        #endregion

        public TabBaseItemBuilder Add()
        {
            TabBaseItem newTab = new TabBaseItem();
            this.ItemList.Add(newTab);
            return new TabBaseItemBuilder(newTab);
        }
    }

}
