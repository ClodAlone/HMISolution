#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Syncfusion.JavaScript.DataVisualization.Models;
using Syncfusion.JavaScript.DataVisualization.Models.Controls;
using Syncfusion.JavaScript.DataVisualization.Models.Collections;

namespace Syncfusion.JavaScript.DataVisualization.Builders
{

    public interface ISymbolPalettesAdder
    {
        ISymbolPalettesAdder Add(Collection palettes);
        ISymbolPalettesAdder Add(Palette palette);
    }

    /// <summary>
    /// Implementation of DiagramModel properties using view formatting. 
    /// </summary>
    /// <remarks>Get properties from viewpage and assign these properties to DiagramModel model</remarks>
    public class SymbolPalettesAdder : ISymbolPalettesAdder
    {
        private SymbolPaletteProperties model;

        public SymbolPalettesAdder(SymbolPaletteProperties symbolPaletteModel)
        {
            this.model = symbolPaletteModel;
        }

        public ISymbolPalettesAdder Add(Collection palettes)
        {
            foreach (Palette palette in palettes)
                this.model.Palettes.Add(palette);
            return this;
        }

        public ISymbolPalettesAdder Add(Palette palette)
        {
            this.model.Palettes.Add(palette);
            return this;
        }

    }
}
