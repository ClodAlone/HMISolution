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
using Syncfusion.JavaScript.Shared;
using System.ComponentModel;
using Syncfusion.JavaScript.Shared.Serializer;

namespace Syncfusion.JavaScript.Olap
{
    public class OlapClientDisplayOptions
    {
        #region PrivateVariables
        private OlapClientDisplayMode mode = OlapClientDisplayMode.ChartAndGrid;
        private OlapClientControlPlacement controlPlacement = OlapClientControlPlacement.Tab;
        private OlapClientDefaultView defaultView = OlapClientDefaultView.Chart;
        private bool togglePanel = false;
        #endregion

        #region Properties
        [JsonProperty("mode")]
        [DefaultValue(OlapClientDisplayMode.ChartAndGrid)]
        [JsonConverter(typeof(StringEnumConverter))]
        public OlapClientDisplayMode Mode
        {
            get { return this.mode; }
            set { this.mode = value; }
        }
        [JsonProperty("controlPlacement")]
        [DefaultValue(OlapClientControlPlacement.Tab)]
        [JsonConverter(typeof(StringEnumConverter))]
        public OlapClientControlPlacement ControlPlacement
        {
            get { return this.controlPlacement; }
            set { this.controlPlacement = value; }
        }
        [JsonProperty("defaultView")]
        [DefaultValue(OlapClientDefaultView.Chart)]
        [JsonConverter(typeof(StringEnumConverter))]
        public OlapClientDefaultView DefaultView
        {
            get { return this.defaultView; }
            set { this.defaultView = value; }
        }
        [JsonProperty("togglePanel")]
        [DefaultValue(false)]
        public bool TogglePanel
        {
            get { return this.togglePanel; }
            set { this.togglePanel = value; }
        }
        #endregion
    }

    public class OlapClientDisplayOptionsBuilder
    {
        private OlapClientDisplayOptions displayOptions = new OlapClientDisplayOptions();
        public OlapClientDisplayOptionsBuilder(OlapClientDisplayOptions displayOptions)
        {
            this.displayOptions = displayOptions;
        }
        public OlapClientDisplayOptionsBuilder Mode(OlapClientDisplayMode mode)
        {
            this.displayOptions.Mode = mode;
            return this;
        }
        public OlapClientDisplayOptionsBuilder DefaultView(OlapClientDefaultView defaultView)
        {
            this.displayOptions.DefaultView = defaultView;
            return this;
        }
        public OlapClientDisplayOptionsBuilder ControlPlacement(OlapClientControlPlacement controlPlacement)
        {
            this.displayOptions.ControlPlacement = controlPlacement;
            return this;
        }
        public OlapClientDisplayOptionsBuilder TogglePanel(bool togglePanel)
        {
            this.displayOptions.TogglePanel = togglePanel;
            return this;
        }
    }
}
