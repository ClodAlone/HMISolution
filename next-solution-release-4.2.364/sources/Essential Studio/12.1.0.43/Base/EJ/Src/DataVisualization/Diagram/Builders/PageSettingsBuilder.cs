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
using Syncfusion.JavaScript.DataVisualization.DiagramEnums;
using Syncfusion.JavaScript.DataVisualization.Models;

namespace Syncfusion.JavaScript.DataVisualization.Builders
{
    public class PageSettingsBuilder
    {
        public DiagramProperties diagramModel;
        public PageSettingsBuilder(DiagramProperties model)
        {
            this.diagramModel = model;
        }

        public PageSettingsBuilder PageWidth(int pageWidth)
        {
            this.diagramModel.PageSettings.PageWidth = pageWidth;
            return this;
        }

        public PageSettingsBuilder PageHeight(int pageHeight)
        {
            this.diagramModel.PageSettings.PageHeight = pageHeight;
            return this;
        }

        public PageSettingsBuilder MultiplePage(bool multiplePage)
        {
            this.diagramModel.PageSettings.MultiplePage = multiplePage;
            return this;
        }

        public PageSettingsBuilder PageBorderWidth(int pageBorderWidth)
        {
            this.diagramModel.PageSettings.PageBorderWidth = pageBorderWidth;
            return this;
        }

        public PageSettingsBuilder PageBorderColor(string pageBorder)
        {
            this.diagramModel.PageSettings.PageBorderColor = pageBorder;
            return this;
        }

        public PageSettingsBuilder PageBackground(string pageBackground)
        {
            this.diagramModel.PageSettings.PageBackgroundColor = pageBackground;
            return this;
        }

        public PageSettingsBuilder PageMargin(int pageMargin)
        {
            this.diagramModel.PageSettings.PageMargin = pageMargin;
            return this;
        }

        public PageSettingsBuilder ShowPageBreaks(bool showPageBreaks)
        {
            this.diagramModel.PageSettings.ShowPageBreaks = showPageBreaks;
            return this;
        }

        public PageSettingsBuilder PageOrientation(Syncfusion.JavaScript.DataVisualization.DiagramEnums.Orientation pageOrientation)
        {
            this.diagramModel.PageSettings.PageOrientation = pageOrientation;
            return this;
        }
    }
}
