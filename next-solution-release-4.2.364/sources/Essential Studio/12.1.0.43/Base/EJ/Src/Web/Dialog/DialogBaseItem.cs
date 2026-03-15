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
    public class DialogBaseItem
    {
        private MvcTemplate<DialogBaseItem> template = null;
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AccordionBaseItem"/> class.
        /// </summary>
        public DialogBaseItem()
        {
            this.Title = string.Empty;
            this.ContentTemplate = new MvcTemplate<DialogBaseItem>();
        }
        #endregion

        #region Properties

        public string Title { get; set; }

        #endregion
        /// <summary>
        /// Gets or sets the content template.
        /// </summary>
        /// <value>The content template.</value>
        [ScriptIgnore]
        public MvcTemplate<DialogBaseItem> ContentTemplate { get; set; }

    }
}
namespace Syncfusion.JavaScript
{



    public class DialogBaseItemBuilder
    {
        #region Constructor
        internal DialogBaseItem Item { get; set; }

        public DialogBaseItemBuilder(DialogBaseItem item)
        {
            this.Item = item;
        }
        #endregion
        public DialogBaseItemBuilder Title(string title)
        {
            this.Item.Title = title;
            return this;
        }

        public DialogBaseItemBuilder ContentTemplate(Action<DialogBaseItem> contentTemplate)
        {
            this.Item.ContentTemplate.WebFormDataTemplate = contentTemplate;
            return this;
        }

        public DialogBaseItemBuilder ContentTemplate(Func<DialogBaseItem, object> contentTemplate)
        {
            this.Item.ContentTemplate.RazorViewTemplate = contentTemplate;
            return this;
        }


    }

    //public class DialogBaseItemAdder
    //{

    //    private List<DialogBaseItem> ItemList;
    //    #region Constructor

    //    public DialogBaseItemAdder(List<DialogBaseItem> itemList)
    //    {
    //        this.ItemList = itemList;
    //    }

    //    #endregion

    //    public DialogBaseItemBuilder Add()
    //    {
    //        DialogBaseItem newDialog = new DialogBaseItem();
    //        this.ItemList.Add(newDialog);
    //        return new DialogBaseItemBuilder(newDialog);
    //    }
    //}

}
