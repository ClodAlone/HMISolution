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
using Syncfusion.JavaScript.Models;

namespace Syncfusion.JavaScript
{
    public partial class EssentialJavaScript
    {
        public WaitingPopupPropertiesBuilder WaitingPopup(string id)
        {
            var model = new WaitingPopupProperties();
            var waitingPopUp = new WaitingPopup(id, model);
            return new WaitingPopupPropertiesBuilder(waitingPopUp);
        }
        public WaitingPopup WaitingPopup(String id, WaitingPopupProperties model)
        {
            var waitingPopUp = new WaitingPopup(id, model);
            return waitingPopUp;
        }


    }
}
