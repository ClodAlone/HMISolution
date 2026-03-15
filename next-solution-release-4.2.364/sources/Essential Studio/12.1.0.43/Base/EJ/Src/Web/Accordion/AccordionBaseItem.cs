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

namespace Syncfusion.JavaScript
{
    public class AccordionBaseItem
    {       

       // internal Action<object, HtmlTag> builder;
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AccordionBaseItem"/> class.
        /// </summary>
        public AccordionBaseItem()
        {
            this.ID = string.Empty;
            this.Text = string.Empty;
            this.ContentTemplate = new MvcTemplate<AccordionBaseItem>();
        }
        #endregion

        #region Properties

        public string ID { get; set; }

        public string Text { get; set; }
        
        /// <summary>
        /// Gets or sets the content template.
        /// </summary>
        /// <value>The content template.</value>
        [ScriptIgnore]
        public MvcTemplate<AccordionBaseItem> ContentTemplate
        {
            get;
            set;
        }
        #endregion
    }


    public class AccordionBaseItemBuilder
    {
        #region Constructor
        internal AccordionBaseItem Item { get; set; }

        public AccordionBaseItemBuilder(AccordionBaseItem item)
        {
            this.Item = item;
        }
        #endregion


        public AccordionBaseItemBuilder ID(string id)
        {
            this.Item.ID = id;
            return this;
        }

        public AccordionBaseItemBuilder Text(string text)
        {
            this.Item.Text = text;
            return this;
        }

        public AccordionBaseItemBuilder ContentTemplate(Action<AccordionBaseItem> contentTemplate)
        {
            this.Item.ContentTemplate.WebFormDataTemplate = contentTemplate;
            return this;
        }

        public AccordionBaseItemBuilder ContentTemplate(Func<AccordionBaseItem, object> contentTemplate)
        {
            this.Item.ContentTemplate.RazorViewTemplate = contentTemplate;
            return this;
        }


    }

    public class AccordionBaseItemAdder
    {

        private List<AccordionBaseItem> ItemList;
        #region Constructor

        public AccordionBaseItemAdder(List<AccordionBaseItem> itemList)
        {
            this.ItemList = itemList;
        }

        #endregion

        public AccordionBaseItemBuilder Add()
        {
            AccordionBaseItem newAccordion = new AccordionBaseItem();
            this.ItemList.Add(newAccordion);
            return new AccordionBaseItemBuilder(newAccordion);
        }
    }

}
