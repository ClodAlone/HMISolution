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
using Syncfusion.JavaScript.DataVisualization;
using Syncfusion.JavaScript.DataVisualization.Models;

namespace Syncfusion.JavaScript
{
    public partial class EssentialJavaScript
    {
        public DiagramPropertiesBuilder Diagram(string id)
        {
            var model = new DiagramProperties();
            var diagram = new Diagram(id, model);
            return new DiagramPropertiesBuilder(diagram);
        }
        public Diagram Diagram(string id,DiagramProperties model)
        {
            var diagram = new Diagram(id, model);
            return diagram;
        }

        public SymbolPalettePropertiesBuilder SymbolPalette(string id)
        {
            var model = new SymbolPaletteProperties();
            var symbolPalette = new SymbolPalette(id, model);
            return new SymbolPalettePropertiesBuilder(symbolPalette);
        }

        public SymbolPalette SymbolPalette(string id, SymbolPaletteProperties model)
        {
            var symbolPalette = new SymbolPalette(id, model);
            return symbolPalette;
        }
         
    }
}
