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
using System.Web;
using Syncfusion.JavaScript.DataVisualization.Models;

namespace Syncfusion.JavaScript.DataVisualization
{
    public class SymbolPalette : Control
    {
        public SymbolPaletteProperties SymbolPaletteModel
        {
            get;

            set;
        }
        public override string TagName
        {
            get
            {
                return "div";
            }
        }
        public override string PluginName
        {
            get { return "ejSymbolPalette"; }
        }
        protected override object Model
        {
            get
            {
                return this.SymbolPaletteModel;
            }
        }
        public SymbolPalette()
        {
            this.SymbolPaletteModel = new SymbolPaletteProperties();
        }
        public SymbolPalette(string id, SymbolPaletteProperties propModel)
        {
            this.ID = id;
            this.SymbolPaletteModel = propModel;
        }
        public override HtmlString CreateContainer(string controlId)
        {
            TagBuilder tag = new TagBuilder(TagName);
            tag.Attributes.Add("id", controlId);

            return new HtmlString(String.Format(tag.ToString()));
        }
    }
}
