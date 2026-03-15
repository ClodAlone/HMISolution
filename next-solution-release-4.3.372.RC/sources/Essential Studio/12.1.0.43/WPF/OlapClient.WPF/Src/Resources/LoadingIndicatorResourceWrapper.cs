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
using System.Globalization;

namespace Syncfusion.Windows.Client.Olap.Resources
{
    internal sealed class LoadingIndicatorResourceWrapper
    {
        const string LOADING_INDICATOR_TEXT = "OlapClient_Loading_Indicator_Text";

        public LoadingIndicatorResourceWrapper()
        {
            CultureInfo ci = CultureInfo.CurrentUICulture;

            loadingIndicatorText = SR.GetString(ci, LOADING_INDICATOR_TEXT);
        }

        private string loadingIndicatorText;

        public string LoadingIndicatorText
        {
            get { return loadingIndicatorText; }
            set { loadingIndicatorText = value; }
        }
    }
}
