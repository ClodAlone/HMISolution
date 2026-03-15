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
using Syncfusion.JavaScript.Models;

namespace Syncfusion.JavaScript
{
   public class RTEclientSideEventsBuilder
    {
       private RTEproperties rteModel;
       public RTEclientSideEventsBuilder(RTEproperties rteProp)
       {
           rteModel = rteProp;
       }
       //Events
       public RTEclientSideEventsBuilder Create(String create)
       {
           rteModel.Create = create;
           return this;
       }
       public RTEclientSideEventsBuilder Change(String change)
       {
           rteModel.Change = change;
           return this;
       }
       public RTEclientSideEventsBuilder Execute(String execute)
       {
           rteModel.Execute = execute;
           return this;
       }
       public RTEclientSideEventsBuilder Keyup(String keyup)
       {
           rteModel.Keyup = keyup;
           return this;
       }
       public RTEclientSideEventsBuilder Keydown(String keydown)
       {
           rteModel.Keydown= keydown;
           return this;
       }
       public RTEclientSideEventsBuilder Destroy(String destroy)
       {
           rteModel.Destroy = destroy;
           return this;
       }
    }
}
