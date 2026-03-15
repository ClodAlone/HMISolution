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
using System.ComponentModel;
using Syncfusion.JavaScript.Shared.Serializer;
using System.Web;
using System.Web.Script.Serialization;
using System.IO;
using Syncfusion.JavaScript;
using Syncfusion.JavaScript.Models;

namespace Syncfusion.JavaScript.Models
{
    public class PaneProperties
    {
        #region Fields
        private String paneSize = null;
        private String minSize = null;
        private String maxSize = null;
        private Boolean collapsible = true;
        private Boolean resizable = true;
        private MvcTemplate<PaneProperties> template = new MvcTemplate<PaneProperties>();
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AccordionBaseItem"/> class.
        /// </summary>
        public PaneProperties()
        {
            this.ContentTemplate = new MvcTemplate<PaneProperties>();
        }
        #endregion
        /// <summary>
        /// Gets or sets the content template.
        /// </summary>
        /// <value>The content template.</value>
        [ScriptIgnore]
        [JsonIgnore]
        public MvcTemplate<PaneProperties> ContentTemplate { get; set; }
        #region Properties
        [JsonProperty("paneSize")]
        [DefaultValue(null)]
        public String PaneSize
        {
            get { return this.paneSize; }
            set { this.paneSize = value; }
        }
        [JsonProperty("minSize")]
        [DefaultValue(null)]
        public String MinSize
        {
            get { return this.minSize; }
            set { this.minSize = value; }
        }
        [JsonProperty("maxSize")]
        [DefaultValue(null)]
        public String MaxSize
        {
            get { return this.maxSize; }
            set { this.maxSize = value; }
        }
        [JsonProperty("resizable")]
        [DefaultValue(true)]
        public Boolean Resizable
        {
            get { return this.resizable; }
            set { this.resizable = value; }
        }

        [JsonProperty("collapsible")]
        [DefaultValue(true)]
        public Boolean Collapsible
        {
            get { return this.collapsible; }
            set { this.collapsible = value; }
        }

        #endregion
    }
}
namespace Syncfusion.JavaScript
{

    public class PanePropertiesBuilder
    {
        #region Constructor
        internal PaneProperties Item { get; set; }

        public PanePropertiesBuilder(PaneProperties item)
        {
            this.Item = item;
        }
        #endregion
        public PanePropertiesBuilder ContentTemplate(Action<PaneProperties> contentTemplate)
        {
            this.Item.ContentTemplate.WebFormDataTemplate = contentTemplate;
            return this;
        }

        public PanePropertiesBuilder ContentTemplate(Func<PaneProperties, object> contentTemplate)
        {
            this.Item.ContentTemplate.RazorViewTemplate = contentTemplate;
            return this;
        }
        public PanePropertiesBuilder()
        {
        }
        public PanePropertiesBuilder PaneSize(String paneSize)
        {
                this.Item.PaneSize = paneSize;
            return this;
        }
        public PanePropertiesBuilder MinSize(String minSize)
        {
            this.Item.MinSize = minSize;
            return this;
        }
        public PanePropertiesBuilder MaxSize(String maxSize)
        {
            this.Item.MaxSize = maxSize;
            return this;
        }
        public PanePropertiesBuilder Collapsible()
        {
            this.Item.Collapsible = true;
            return this;
        }
        public PanePropertiesBuilder Collapsible(Boolean collapsible)
        {
            this.Item.Collapsible = collapsible;
            return this;
        }
        public PanePropertiesBuilder Resizable()
        {
            this.Item.Resizable = true;
            return this;
        }

        public PanePropertiesBuilder Resizable(Boolean resizable)
        {
            this.Item.Resizable = resizable;
            return this;
        }

    }
    public class PanePropertiesAdder
    {
        private List<PaneProperties> ItemList;
        #region Constructor

        public PanePropertiesAdder(List<PaneProperties> itemList)
        {
            this.ItemList = itemList;
        }

        #endregion

        public PanePropertiesBuilder Add()
        {
            PaneProperties newSplitter = new PaneProperties();
            this.ItemList.Add(newSplitter);
            return new PanePropertiesBuilder(newSplitter);
        }
    }
}
