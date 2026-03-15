#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Web;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using Syncfusion.JavaScript.Shared.Serializer;
using Syncfusion.JavaScript.Mobile;
using Syncfusion.JavaScript.Mobile.Models;

namespace Syncfusion.JavaScript.Mobile
{
    /// <summary>
    /// Class for Menu
    /// </summary>
    public class Menu : Control
    {
        #region Fields
        private string pluginString = "data-ej-";

        /// <summary>
        /// Gets or sets the mobile menu model.
        /// </summary>
        /// <value>
        /// The mobile menu model.
        /// </value>
        public MobileMenuProperties MobileMenuModel
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the name of the tag.
        /// </summary>
        /// <value>
        /// The name of the tag.
        /// </value>
        public override string TagName
        {
            get
            {
                return "div";
            }
        }

        /// <summary>
        /// Gets the name of the plugin.
        /// </summary>
        /// <value>
        /// The name of the plugin.
        /// </value>
        public override string PluginName
        {
            get { return "ejmMenu"; }
        }

        /// <summary>
        /// Gets the model.
        /// </summary>
        /// <value>
        /// The model.
        /// </value>
        protected override object Model
        {
            get { return this.MobileMenuModel; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="Menu"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="propModel">The property model.</param>
        public Menu(string id, MobileMenuProperties propModel)
        {
            this.ID = id;
            this.MobileMenuModel = propModel;
        }
        #endregion

        #region Render
        /// <summary>
        /// Creates the container.
        /// </summary>
        /// <param name="controlId">The control identifier.</param>
        /// <returns></returns>
        public override HtmlString CreateContainer(string controlId)
        {
            HtmlTag container = new HtmlTag(TagName);
            container.Attributes("id", ID);
            container.Attributes(CreateUnObtrusiveDataDictionary(this.Model, controlId, pluginString));
            container.Add(RenderItemContainer(this.MobileMenuModel.Items));
            return new HtmlString(String.Format(container.ToString()));
        }
        /// <summary>
        /// Creates the un obtrusive container.
        /// </summary>
        /// <param name="controlId">The control identifier.</param>
        /// <returns></returns>
        public override HtmlString CreateUnObtrusiveContainer(string controlId)
        {
            HtmlTag container = new HtmlTag(TagName);
            container.Attributes("id", ID);
            container.Attributes("data-role", PluginName.ToLower());
            container.Attributes(CreateUnObtrusiveDataDictionary(this.Model, controlId, pluginString));
            container.Add(RenderItemContainer(this.MobileMenuModel.Items));
            return new HtmlString(container.ToString());
        }
        /// <summary>
        /// Renders the item container.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <returns></returns>
        internal HtmlTag RenderItemContainer(List<MobileMenuBaseItem> items)
        {
            HtmlTag tag = new HtmlTag("ul");
            foreach (MobileMenuBaseItem item in items)
            {
                HtmlTag liTag = new HtmlTag("li");
                liTag.Attributes(CreateUnObtrusiveDataDictionary(item, this.ID, pluginString)); 
                liTag.Attributes(pluginString + "ios7-buttoncolor", item.IOS7.ButtonColor.ToString().ToLower());
                liTag.Attributes(pluginString + "ios7-buttonstyle", item.IOS7.ButtonStyle.ToString().ToLower());
                liTag.Attributes(pluginString + "ios7-theme", item.IOS7.Theme.ToString().ToLower());
                tag.Add(liTag);
            }
            return tag;
        }
        #endregion
    }
}