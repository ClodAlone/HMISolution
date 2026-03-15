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
    /// Class for Tab
    /// </summary>
    public class Tab : Control
    {
        #region Fields
        private string pluginString = "data-ej-";

        /// <summary>
        /// Gets or sets the mobile tab model.
        /// </summary>
        /// <value>
        /// The mobile tab model.
        /// </value>
        public MobileTabProperties MobileTabModel
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
            get { return "ejmTab"; }
        }
        /// <summary>
        /// Gets the model.
        /// </summary>
        /// <value>
        /// The model.
        /// </value>
        protected override object Model
        {
            get { return this.MobileTabModel; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="Tab"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="propModel">The property model.</param>
        public Tab(String id, MobileTabProperties propModel)
        {
            this.ID = id;
            this.MobileTabModel = propModel;
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
            container.Add(RenderItemContainer(this.MobileTabModel.Items));
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
            container.Attributes(CreateUnObtrusiveDataDictionary(this.Model, controlId, pluginString));
            container.Attributes("id", ID);
            container.Attributes("data-role", PluginName.ToLower());
            container.Add(RenderItemContainer(this.MobileTabModel.Items));
            return new HtmlString(container.ToString());
        }

        /// <summary>
        /// Renders the item container.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <returns></returns>
        internal HtmlTag RenderItemContainer(List<MobileTabBaseItem> items)
        {
            HtmlTag tag = new HtmlTag("ul");
            foreach (MobileTabBaseItem item in items)
            {
                HtmlTag liTag = new HtmlTag("li");
                liTag.Attributes(CreateUnObtrusiveDataDictionary(item, this.ID, pluginString)); 
                if (item.Content.builder != null && string.IsNullOrEmpty(item.Href))
                    item.Content.builder(item, liTag);
                tag.Add(liTag);
            }
            return tag;
        }
        #endregion
    }
}
