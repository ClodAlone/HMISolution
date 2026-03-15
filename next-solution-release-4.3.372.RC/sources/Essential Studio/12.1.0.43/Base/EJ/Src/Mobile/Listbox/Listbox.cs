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
using Syncfusion.JavaScript;
using Syncfusion.JavaScript.Mobile.Models;

namespace Syncfusion.JavaScript.Mobile
{
    public class MobileListbox : Control
    {
        #region Fields
        private string pluginString = "data-ej-";
        private string id;

        /// <summary>
        /// Gets or sets the mobile listbox model.
        /// </summary>
        /// <value>
        /// The mobile listbox model.
        /// </value>
        public MobileListboxProperties MobileListboxModel
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
            get { return "ejmListbox"; }
        }
        protected override object Model
        {
            get { return this.MobileListboxModel; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileListbox"/> class.
        /// </summary>
        public MobileListbox() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileListbox"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="propModel">The property model.</param>
        public MobileListbox(String id, MobileListboxProperties propModel)
        {
            this.ID = id;
            this.MobileListboxModel = propModel;
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
            return new HtmlString(String.Format(container.ToString()));
        }
        /// <summary>
        /// Creates the un obtrusive container.
        /// </summary>
        /// <param name="controlId">The control identifier.</param>
        /// <returns></returns>
        public override HtmlString CreateUnObtrusiveContainer(string controlId)
        {
            id = controlId;
            HtmlTag container = new HtmlTag(TagName);
            container.Attributes(CreateUnObtrusiveDataDictionary(this.Model, controlId, pluginString));
            container.Attributes("id", ID);
            container.Attributes("data-role", PluginName.ToLower());
            if (this.MobileListboxModel.GroupList && this.MobileListboxModel.Groups.Count > 0)
            {
                if (this.MobileListboxModel.RenderTemplate && this.MobileListboxModel.DataBinding)
                {
                    if (string.IsNullOrEmpty(this.MobileListboxModel.TemplateId))
                        this.MobileListboxModel.ContentTemplate.builder(this.MobileListboxModel, container);
                }
                else
                    RenderGroupItems(this.MobileListboxModel.Groups, container);
            }
            else if (this.MobileListboxModel.RenderTemplate && this.MobileListboxModel.Items.Count == 0)
            {
                if (string.IsNullOrEmpty(this.MobileListboxModel.TemplateId))
                {
                    if (this.MobileListboxModel.ContentTemplate.builder != null)
                        this.MobileListboxModel.ContentTemplate.builder(this.MobileListboxModel, container);
                }
            }
            else if (this.MobileListboxModel.Items.Count > 0)
            {
                RenderItemContainer(this.MobileListboxModel.Items, container, null);
            }
            return new HtmlString(container.ToString() + this.Data);
        }
        /// <summary>
        /// Renders the group items.
        /// </summary>
        /// <param name="groupItems">The group items.</param>
        /// <param name="container">The container.</param>
        internal void RenderGroupItems(List<MobileListboxGroupItem> groupItems, HtmlTag container)
        {

            foreach (MobileListboxGroupItem groupItem in groupItems)
            {
                RenderItemContainer(groupItem.GroupItems, container, groupItem);
            }

        }
        /// <summary>
        /// Renders the item container.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="container">The container.</param>
        /// <param name="groupItem">The group item.</param>
        internal void RenderItemContainer(List<MobileListboxItem> items, HtmlTag container, MobileListboxGroupItem groupItem)
        {
            HtmlTag ul = new HtmlTag("ul");
            if (this.MobileListboxModel.GroupList && this.MobileListboxModel.Groups.Count > 0)
            {
                if (!string.IsNullOrEmpty(groupItem.Title))
                    ul.Attributes(pluginString + "grouplisttitle", groupItem.Title);
            }
            foreach (MobileListboxItem item in items)
            {
                HtmlTag li = new HtmlTag("li");
                if (item.RenderTemplate && string.IsNullOrEmpty(item.TemplateId))
                {
                    if (item.ContentTemplate.builder != null)
                        item.ContentTemplate.builder(item, li);
                }
                else
                {
                    if (item.Children.Count > 0)
                        RenderItemContainer(item.Children, li, null);
                }
                ul.Add(RenderItem(item, li));
            }
            container.Add(ul);
        }

        /// <summary>
        /// Renders the item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="li">The li.</param>
        /// <returns></returns>
        private HtmlTag RenderItem(MobileListboxItem item, HtmlTag li)
        {
            li.Attributes(CreateUnObtrusiveDataDictionary(item, id, pluginString));
            return li;
        }

        #endregion
    }

}
