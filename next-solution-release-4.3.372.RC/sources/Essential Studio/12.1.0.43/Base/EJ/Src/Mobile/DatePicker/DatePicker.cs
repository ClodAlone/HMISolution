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
using System.Runtime.Serialization;

namespace Syncfusion.JavaScript.Mobile
{
    /// <summary>
    /// Class for Date Picker Control
    /// </summary>
    public class DatePicker : Control
    {
        #region Fields

        private string pluginString = "data-ej-";

        public MobileDatePickerProperties MDatePickerModel
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
                return "input";
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
            get { return "ejmDatePicker"; }
        }

        /// <summary>
        /// Gets the model.
        /// </summary>
        /// <value>
        /// The model.
        /// </value>
        protected override object Model
        {
            get { return this.MDatePickerModel; }
        }
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DatePicker"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="propModel">The property model.</param>
        public DatePicker(String id, MobileDatePickerProperties propModel)
        {
            this.ID = id;
            this.MDatePickerModel = propModel;
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

        #endregion
    }
    /// <summary>
    /// Assign Enum Values for Mode
    /// </summary>
    [DataContract]
    public enum DateType
    {
        [EnumMember(Value = "date")]
        Date,
        [EnumMember(Value = "month")]
        Month
    }
}
  