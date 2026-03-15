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

namespace Syncfusion.JavaScript
{
    public class RTEBaseItem
    {
        private MvcTemplate<RTEBaseItem> template = null;

        
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AccordionBaseItem"/> class.
        /// </summary>
        public RTEBaseItem()
        {
            this.ID = string.Empty;
            //this.Text = string.Empty;
            this.ContentTemplate = new MvcTemplate<RTEBaseItem>();
        }
        #endregion

        #region Properties

        public string ID { get; set; }

        //public string Text { get; set; }


        #endregion
        /// <summary>
        /// Gets or sets the content template.
        /// </summary>
        /// <value>The content template.</value>
        [ScriptIgnore]
        public MvcTemplate<RTEBaseItem> ContentTemplate { get; set; }
    }

    public class RTEBaseItemBuilder
    {
        #region Constructor
        internal RTEBaseItem Item { get; set; }

        public RTEBaseItemBuilder(RTEBaseItem item)
        {
            this.Item = item;
        }
        #endregion


        public RTEBaseItemBuilder ID(string id)
        {
            this.Item.ID = id;
            return this;
        }

        //public RTEBaseItemBuilder Text(string text)
        //{
        //    this.Item.Text = text;
        //    return this;
        //}

        public RTEBaseItemBuilder ContentTemplate(Action<RTEBaseItem> contentTemplate)
        {
            this.Item.ContentTemplate.WebFormDataTemplate = contentTemplate;
            return this;
        }

        public RTEBaseItemBuilder ContentTemplate(Func<RTEBaseItem, object> contentTemplate)
        {
            this.Item.ContentTemplate.RazorViewTemplate = contentTemplate;
            return this;
        }


    }

    public class RTEBaseItemAdder
    {

        private List<RTEBaseItem> ItemList;
        #region Constructor

        public RTEBaseItemAdder(List<RTEBaseItem> itemList)
        {
            this.ItemList = itemList;
        }

        #endregion

        public RTEBaseItemBuilder Add()
        {
            RTEBaseItem newTab = new RTEBaseItem();
            this.ItemList.Add(newTab);
            return new RTEBaseItemBuilder(newTab);
        }
    }

}
