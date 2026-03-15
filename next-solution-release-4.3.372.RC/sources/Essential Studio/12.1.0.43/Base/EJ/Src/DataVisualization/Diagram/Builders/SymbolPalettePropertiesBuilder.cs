#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using System.Web;
using Syncfusion.JavaScript.DataSources;
using Syncfusion.JavaScript;
using Syncfusion.JavaScript.Models;
using Syncfusion.JavaScript.DataVisualization.Models.Collections;
using Syncfusion.JavaScript.DataVisualization.Builders;

namespace Syncfusion.JavaScript.DataVisualization
{
    public class SymbolPalettePropertiesBuilder
    {
        public SymbolPalette symbolPalette;

        public SymbolPalettePropertiesBuilder(SymbolPalette symbolPalette)
        { 
            this.symbolPalette = new SymbolPalette(symbolPalette.ID, symbolPalette.SymbolPaletteModel); 
        }

        public SymbolPalettePropertiesBuilder()
        {

        }

        public SymbolPalettePropertiesBuilder Height(String height)
        {
            symbolPalette.SymbolPaletteModel.Height = height;
            return this;
        }
        public SymbolPalettePropertiesBuilder Width(String width)
        {
            symbolPalette.SymbolPaletteModel.Width = width;
            return this;
        }

        public SymbolPalettePropertiesBuilder PaletteItemWidth(int paletteItemWidth)
        {
            symbolPalette.SymbolPaletteModel.PaletteItemWidth = paletteItemWidth;
            return this;
        }

        public SymbolPalettePropertiesBuilder PaletteItemHeight(int paletteItemHeight)
        {
            symbolPalette.SymbolPaletteModel.PaletteItemHeight = paletteItemHeight;
            return this;    
        }
        public SymbolPalettePropertiesBuilder ShowPaletteItemText(bool showPaletteItemText)
        {
            symbolPalette.SymbolPaletteModel.ShowPaletteItemText = showPaletteItemText;
            return this;
        }
        public SymbolPalettePropertiesBuilder AllowDrag(bool allowDrag)
        {
            symbolPalette.SymbolPaletteModel.AllowDrag = allowDrag;
            return this;
        }
        public SymbolPalettePropertiesBuilder DiagramId(string diagramId)
        {
            symbolPalette.SymbolPaletteModel.DiagramId = diagramId;
            return this;
        }
        public SymbolPalettePropertiesBuilder HeaderHeight(int headerHeight)
        {
            symbolPalette.SymbolPaletteModel.HeaderHeight = headerHeight;
            return this;
        }
        public SymbolPalettePropertiesBuilder SelectedPaletteIndex(int selectedPaletteIndex)
        {
            symbolPalette.SymbolPaletteModel.SelectedPaletteIndex = selectedPaletteIndex;
            return this;
        }
        public virtual SymbolPalettePropertiesBuilder Palettes(Action<ISymbolPalettesAdder> palettes)
        {
            symbolPalette.SymbolPaletteModel.Palettes = new Collection();
            SymbolPalettesAdder addPalettes = new SymbolPalettesAdder(symbolPalette.SymbolPaletteModel);
            palettes.Invoke(addPalettes);
            return this;
        }
        //Render
        public HtmlString Render()
        {
            return new HtmlString(symbolPalette.Render().ToString());
        }
        public override String ToString()
        {
            return Render().ToString();
        }
    }
}
