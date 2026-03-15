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
using Syncfusion.JavaScript.Shared.Serializer;
using System.ComponentModel;
using Syncfusion.JavaScript.Mobile.Models;


namespace Syncfusion.JavaScript.Mobile.Models
{
    /// <summary>
    /// Class for the Dialog base item
    /// </summary>
    public class MobileDialogBaseItem
    {
        #region Properties
        
        public MvcTemplate<MobileDialogBaseItem> Content { get; set; }
       

        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileDialogBaseItem"/> class.
        /// </summary>
        public MobileDialogBaseItem()
        {
            this.Content = new MvcTemplate<MobileDialogBaseItem>();
        }
        #endregion
    }
}
namespace Syncfusion.JavaScript
{
    public class MobileDialogBaseItemBuilder
    {
        #region Field
        private MobileDialogBaseItem Item;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileDialogBaseItemBuilder"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        public MobileDialogBaseItemBuilder(MobileDialogBaseItem item)
        {
            this.Item = item;
        }
        #endregion

        #region Builder      
        
        /// <summary>
        /// Content for tab item
        /// </summary>
        /// <param name="content">if set to <c>true</c> [content].</param>
        /// <returns></returns>
        public MobileDialogBaseItemBuilder Content(Action<MobileDialogBaseItem> template)
        {
            this.Item.Content.WebFormDataTemplate = template;
            return this;
        }
        public MobileDialogBaseItemBuilder Content(Func<MobileDialogBaseItem, object> template)
        {
            this.Item.Content.RazorViewTemplate = template;
            return this;
        }
       

        #endregion

    }

    public class MobileDialogBaseItemAdder
    {
        #region Field
        private List<MobileDialogBaseItem> ItemList;
        #endregion

        #region Constructor
        public MobileDialogBaseItemAdder(List<MobileDialogBaseItem> itemList)
        {
            this.ItemList = itemList;
        }
        #endregion

        public MobileDialogBaseItemBuilder Add()
        {
            MobileDialogBaseItem newDialog = new MobileDialogBaseItem();
            this.ItemList.Add(newDialog);
            return new MobileDialogBaseItemBuilder(newDialog);
        }
    }
}
