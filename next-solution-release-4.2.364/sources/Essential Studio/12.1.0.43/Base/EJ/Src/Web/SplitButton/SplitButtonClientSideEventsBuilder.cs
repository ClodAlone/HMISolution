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
   public class SplitButtonClientSideEventsBuilder
    {
        private SplitButtonProperties splitbuttonModel;
        public SplitButtonClientSideEventsBuilder(SplitButtonProperties splitbuttonProp)
        {
            splitbuttonModel = splitbuttonProp;
        }
        //Events
        public SplitButtonClientSideEventsBuilder Create(String create)
        {
            splitbuttonModel.Create = create;
            return this;
        }
        public SplitButtonClientSideEventsBuilder Click(String click)
        {
            splitbuttonModel.Click = click;
            return this;
        }
        public SplitButtonClientSideEventsBuilder ItemMouseOver(String itemMouseOver)
        {
            splitbuttonModel.ItemMouseOver = itemMouseOver;
            return this;
        }
        public SplitButtonClientSideEventsBuilder ItemMouseOut(String itemMouseOut)
        {
            splitbuttonModel.ItemMouseOut = itemMouseOut;
            return this;
        }
        public SplitButtonClientSideEventsBuilder ItemSelected(String itemSelected)
        {
            splitbuttonModel.ItemSelected = itemSelected;
            return this;
        }
       public SplitButtonClientSideEventsBuilder Destroy(String destroy)
        {
            splitbuttonModel.Destroy = destroy;
            return this;
        }
    }
}
