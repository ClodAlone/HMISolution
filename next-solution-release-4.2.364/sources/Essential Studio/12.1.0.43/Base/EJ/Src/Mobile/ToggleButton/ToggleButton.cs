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
using System.Web;
using Syncfusion.JavaScript.Mobile.Models;
using Syncfusion.JavaScript.Models;

namespace Syncfusion.JavaScript.Mobile
{
    /// <summary>
    /// Class for Toggle Button
    /// </summary>
    public class ToggleButton : Control
    {
        #region fields

        private string pluginString = "data-ej-";

        /// <summary>
        /// Gets or sets the mobile toggle button model.
        /// </summary>
        /// <value>
        /// The mobile toggle button model.
        /// </value>
        public MobileToggleButtonProperties MobileToggleButtonModel
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
            get { return "ejmToggleButton"; }
        }

        /// <summary>
        /// Gets the model.
        /// </summary>
        /// <value>
        /// The model.
        /// </value>
        protected override object Model
        {
            get { return this.MobileToggleButtonModel; }
        }

        #endregion

        #region Contructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ToggleButton"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="propModel">The property model.</param>
        public ToggleButton(string id, MobileToggleButtonProperties propModel)
        {
            this.ID = id;
            this.MobileToggleButtonModel = propModel;
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
             HtmlTag container = new HtmlTag(TagName);
             container.Attributes(CreateUnObtrusiveDataDictionary(this.Model, controlId, pluginString));
             container.Attributes("id", ID);
             container.Attributes("data-role", PluginName.ToLower());
             return new HtmlString(container.ToString());
         }

        #endregion
    }
}
