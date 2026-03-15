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
    public class MenuClientSideEventsBuilder
    {
        private MenuProperties menuModel;
        public MenuClientSideEventsBuilder(MenuProperties menuProp)
        {
            menuModel = menuProp;
        }
        //Events
        public MenuClientSideEventsBuilder Create(String create)
        {
            menuModel.Create = create;
            return this;
        }
        public MenuClientSideEventsBuilder BeforeContextOpen(String beforeContextOpen)
        {
            menuModel.BeforeContextOpen = beforeContextOpen;
            return this;
        }
        public MenuClientSideEventsBuilder ContextOpen(String contextOpen)
        {
            menuModel.ContextOpen = contextOpen;
            return this;
        }
        public MenuClientSideEventsBuilder ContextClose(String contextClose)
        {
            menuModel.ContextClose = contextClose;
            return this;
        }
        public MenuClientSideEventsBuilder MouseOver(String mouseOver)
        {
            menuModel.MouseOver = mouseOver;
            return this;
        }
        public MenuClientSideEventsBuilder MouseOut(String mouseOut)
        {
            menuModel.MouseOut = mouseOut;
            return this;
        }
        public MenuClientSideEventsBuilder KeyDown(String keyDown)
        {
            menuModel.KeyDown = keyDown;
            return this;
        }
        public MenuClientSideEventsBuilder Click(String click)
        {
            menuModel.Click = click;
            return this;
        }
        public MenuClientSideEventsBuilder Destroy(String destroy)
        {
            menuModel.Destroy = destroy;
            return this;
        }
    }
}
