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
using System.ComponentModel;
using Syncfusion.JavaScript.Shared.Serializer;

namespace Syncfusion.JavaScript.Olap
{
    public class OlapClientServiceMethods
    {
        #region PrivateVariables
        private string initialize = "InitializeOlapClient";
        private string removeSplitButton = "RemoveSplitButton";
        private string filterElement = "FilterElement";
        private string toolbarSelection = "ToolbarSelection";
        private string nodeDropped = "NodeDropped";
        private string fetchMemberTreeNodes = "FetchMemberTreeNodes";
        private string cubeChanged = "CubeChanged";
        private string toolbarServices = "ToolbarServices";
        private string memberExpand = "MemberExpand";
        private string saveReport = "SaveReport";
        private string fetchReportList = "FetchReportList";
        private string loadReport = "LoadReport";
        #endregion
        #region Properties
        [JsonProperty("initialize")]
        [DefaultValue("InitializeOlapClient")]
        public String Initialize
        {
            get { return this.initialize; }
            set { this.initialize = value; }
        }
        [JsonProperty("removeSplitButton")]
        [DefaultValue("RemoveSplitButton")]
        public string RemoveSplitButton
        {
            get { return this.removeSplitButton; }
            set { this.removeSplitButton = value; }
        }
        [JsonProperty("filterElement")]
        [DefaultValue("FilterElement")]
        public string FilterElement
        {
            get { return this.filterElement; }
            set { this.filterElement = value; }
        }
        [JsonProperty("toolbarSelection")]
        [DefaultValue("ToolbarSelection")]
        public string ToolbarSelection
        {
            get { return this.toolbarSelection; }
            set { this.toolbarSelection = value; }
        }
        [JsonProperty("nodeDropped")]
        [DefaultValue("NodeDropped")]
        public string NodeDropped
        {
            get { return this.nodeDropped; }
            set { this.nodeDropped = value; }
        }
        [JsonProperty("fetchMemberTreeNodes")]
        [DefaultValue("FetchMemberTreeNodes")]
        public string FetchMemberTreeNodes
        {
            get { return this.fetchMemberTreeNodes; }
            set { this.fetchMemberTreeNodes = value; }
        }
        [JsonProperty("cubeChanged")]
        [DefaultValue("CubeChanged")]
        public string CubeChanged
        {
            get { return this.cubeChanged; }
            set { this.cubeChanged = value; }
        }
        [JsonProperty("toolbarServices")]
        [DefaultValue("ToolbarServices")]
        public string ToolbarServices
        {
            get { return this.toolbarServices; }
            set { this.toolbarServices = value; }
        }
        [JsonProperty("memberExpand")]
        [DefaultValue("MemberExpand")]
        public string MemberExpand
        {
            get { return this.memberExpand; }
            set { this.memberExpand = value; }
        }
        [JsonProperty("saveReport")]
        [DefaultValue("SaveReport")]
        public string SaveReport
        {
            get { return this.saveReport; }
            set { this.saveReport = value; }
        }
        [JsonProperty("fetchReportList")]
        [DefaultValue("FetchReportList")]
        public string FetchReportList
        {
            get { return this.fetchReportList; }
            set { this.fetchReportList = value; }
        }
        [JsonProperty("loadReport")]
        [DefaultValue("LoadReport")]
        public string LoadReport
        {
            get { return this.loadReport; }
            set { this.loadReport = value; }
        }
        #endregion
    }
    public class OlapClientServiceMethodsBuilder
    {
        private OlapClientServiceMethods serviceMethods = new OlapClientServiceMethods();
        public OlapClientServiceMethodsBuilder(OlapClientServiceMethods serviceMethods)
        {
            this.serviceMethods = serviceMethods;
        }
        public OlapClientServiceMethodsBuilder Initialize(string initialize)
        {
            this.serviceMethods.Initialize = initialize;
            return this;
        }
        public OlapClientServiceMethodsBuilder RemoveSplitButton(string removeSplitButton)
        {
            this.serviceMethods.RemoveSplitButton = removeSplitButton;
            return this;
        }
        public OlapClientServiceMethodsBuilder FilterElement(string filterElement)
        {
            this.serviceMethods.FilterElement = filterElement;
            return this;
        }
        public OlapClientServiceMethodsBuilder ToolbarSelection(string toolbarSelection)
        {
            this.serviceMethods.ToolbarSelection = toolbarSelection;
            return this;
        }
        public OlapClientServiceMethodsBuilder NodeDropped(string nodeDropped)
        {
            this.serviceMethods.NodeDropped = nodeDropped;
            return this;
        }
        public OlapClientServiceMethodsBuilder FetchMemberTreeNodes(string fetchMemberTreeNodes)
        {
            this.serviceMethods.FetchMemberTreeNodes = fetchMemberTreeNodes;
            return this;
        }
        public OlapClientServiceMethodsBuilder CubeChanged(string cubeChanged)
        {
            this.serviceMethods.CubeChanged = cubeChanged;
            return this;
        }
        public OlapClientServiceMethodsBuilder ToolbarServices(string toolbarServices)
        {
            this.serviceMethods.ToolbarServices = toolbarServices;
            return this;
        }
        public OlapClientServiceMethodsBuilder MemberExpand(string memberExpand)
        {
            this.serviceMethods.MemberExpand = memberExpand;
            return this;
        }
        public OlapClientServiceMethodsBuilder SaveReport(string saveReport)
        {
            this.serviceMethods.SaveReport = saveReport;
            return this;
        }
        public OlapClientServiceMethodsBuilder FetchReportList(string fetchReportList)
        {
            this.serviceMethods.FetchReportList = fetchReportList;
            return this;
        }
        public OlapClientServiceMethodsBuilder LoadReport(string loadReport)
        {
            this.serviceMethods.LoadReport = loadReport;
            return this;
        }
    }
}
