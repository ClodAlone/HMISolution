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
using Syncfusion.JavaScript.Olap.Models;

namespace Syncfusion.JavaScript.Olap
{
    public class OlapClientClientSideEventsBuilder
    {
        private OlapClientProperties olapClientModel;
        public OlapClientClientSideEventsBuilder(OlapClientProperties olapClientProp)
        {
            olapClientModel = olapClientProp;
        }
        public OlapClientClientSideEventsBuilder BeforeServiceInvoke(string beforeServiceInvoke)
        {
            olapClientModel.BeforeServiceInvoke = beforeServiceInvoke;
            return this;
        }
        public OlapClientClientSideEventsBuilder AfterServiceInvoke(string afterServiceInvoke)
        {
            olapClientModel.AfterServiceInvoke = afterServiceInvoke;
            return this;
        }
        public OlapClientClientSideEventsBuilder Load(string load)
        {
            olapClientModel.Load = load;
            return this;
        }
        public OlapClientClientSideEventsBuilder ChartPreRender(string chartPreRender)
        {
            olapClientModel.ChartPreRender = chartPreRender;
            return this;
        }
        public OlapClientClientSideEventsBuilder ClientSuccess(string clientSuccess)
        {
            olapClientModel.ClientSuccess = clientSuccess;
            return this;
        }
        public OlapClientClientSideEventsBuilder ClientError(string clientError)
        {
            olapClientModel.ClientError = clientError;
            return this;
        }
        public OlapClientClientSideEventsBuilder ClientComplete(string clientComplete)
        {
            olapClientModel.ClientComplete = clientComplete;
            return this;
        }
        public OlapClientClientSideEventsBuilder Destroy(string destroy)
        {
            olapClientModel.Destroy = destroy;
            return this;
        }
    }
}
