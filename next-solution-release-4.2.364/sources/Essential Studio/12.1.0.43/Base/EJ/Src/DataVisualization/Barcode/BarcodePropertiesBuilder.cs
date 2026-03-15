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
    public class BarcodePropertiesBuilder
    {
        public Barcode barcode;

        public BarcodePropertiesBuilder(Barcode barcode)
        { 
            this.barcode = new Barcode(barcode.ID, barcode.BarcodeModel); 
        }

        //public BarcodePropertiesBuilder()
        //{
        //}

        public BarcodePropertiesBuilder DisplayText(bool displayText)
        {
            barcode.BarcodeModel.DisplayText = displayText;
            return this;
        }

        public BarcodePropertiesBuilder Text(string text)
        {
            barcode.BarcodeModel.Text = text;
            return this;
        }

        public BarcodePropertiesBuilder SymbologyType(BarcodeSymbolType symbologyType)
        {
            barcode.BarcodeModel.SymbologyType = symbologyType;
            return this;
        }

        public BarcodePropertiesBuilder TextColor(String textColor)
        {
            barcode.BarcodeModel.TextColor = textColor;
            return this;
        }

        public BarcodePropertiesBuilder LightBarColor(String lightBarColor)
        {
            barcode.BarcodeModel.LightBarColor = lightBarColor;
            return this;
        }

        public BarcodePropertiesBuilder DarkBarColor(String darkBarColor)
        {
            barcode.BarcodeModel.DarkBarColor = darkBarColor;
            return this;
        }

        public BarcodePropertiesBuilder QuietZone(Action<QuietZoneBuilder> quietZone)
        {
            var obj = new QuietZone();
            barcode.BarcodeModel.QuietZone = quietZone;
            var builder = new QuietZoneBuilder(barcode, obj);
            if (quietZone != null)
                quietZone.Invoke(builder);
            return this;
        }

        public BarcodePropertiesBuilder NarrowBarWidth(int narrowBarWidth)
        {
            barcode.BarcodeModel.NarrowBarWidth = narrowBarWidth;
            return this;
        }

        public BarcodePropertiesBuilder WideBarWidth(int wideBarWidth)
        {
            barcode.BarcodeModel.WideBarWidth = wideBarWidth;
            return this;
        }

        public BarcodePropertiesBuilder BarHeight(int barHeight)
        {
            barcode.BarcodeModel.BarHeight = barHeight;
            return this;
        }

        public BarcodePropertiesBuilder BarcodeToTextGapHeight(int barcodeToTextGapHeight)
        {
            barcode.BarcodeModel.BarcodeToTextGapHeight = barcodeToTextGapHeight;
            return this;
        }

        public BarcodePropertiesBuilder XDimension(int xDimension)
        {
            barcode.BarcodeModel.XDimension = xDimension;
            return this;
        }

        public BarcodePropertiesBuilder EncodeStartStopSymbol(bool encodeStartStopSymbol)
        {
            barcode.BarcodeModel.EncodeStartStopSymbol = encodeStartStopSymbol;
            return this;
        }

        public BarcodePropertiesBuilder Enabled(bool enabled)
        {
            barcode.BarcodeModel.Enabled = enabled;
            return this;
        }
        
        //Events
        public BarcodePropertiesBuilder ClientSideEvents(Action<BarcodeClientSideEventsBuilder> clientSideEvents)
        {
            var builder = new BarcodeClientSideEventsBuilder(this.barcode.BarcodeModel);
            if (clientSideEvents != null)
                clientSideEvents.Invoke(builder);
            return this;
        }

        public HtmlString Render()
        {
            return new HtmlString(barcode.Render().ToString());
        }
        public override String ToString()
        {

            return Render().ToString();
        }
    }

    public class QuietZoneBuilder
    {
        private BarcodeProperties barcodeProp;
        private QuietZone m_quietZoneBuilder;
        public QuietZoneBuilder(Barcode barcode, QuietZone qZone)
        {
            this.m_quietZoneBuilder = qZone;
            this.barcodeProp = barcode.BarcodeModel;
            this.barcodeProp.QuietZone = qZone;
        }

        public QuietZoneBuilder(QuietZone options)
        {
            this.m_quietZoneBuilder = options;
        }

        public QuietZoneBuilder All(int all)
        {
            this.m_quietZoneBuilder.All = all;
            return this;
        }

        public QuietZoneBuilder Top(int top)
        {
            this.m_quietZoneBuilder.Top = top;
            return this;
        }

        public QuietZoneBuilder Left(int left)
        {
            this.m_quietZoneBuilder.Left = left;
            return this;
        }

        public QuietZoneBuilder Bottom(int bottom)
        {
            this.m_quietZoneBuilder.Bottom = bottom;
            return this;
        }

        public QuietZoneBuilder Right(int right)
        {
            this.m_quietZoneBuilder.Right = right;
            return this;
        }
    }
}
