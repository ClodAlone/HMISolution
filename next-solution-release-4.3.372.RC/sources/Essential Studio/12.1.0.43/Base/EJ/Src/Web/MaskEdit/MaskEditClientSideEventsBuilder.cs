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
    public class MaskEditClientSideEventsBuilder
    {
        private MaskEditProperties maskEditModel;
        public MaskEditClientSideEventsBuilder(MaskEditProperties maskEditProp)
        {
            maskEditModel = maskEditProp;
        }
        //Events
        public MaskEditClientSideEventsBuilder Create(String create)
        {
            maskEditModel.Create = create;
            return this;
        }
        public MaskEditClientSideEventsBuilder OnKeyDown(String onKeyDown)
        {
            maskEditModel.OnKeyDown = onKeyDown;
            return this;
        }
        public MaskEditClientSideEventsBuilder KeyUp(String keyUp)
        {
            maskEditModel.KeyUp = keyUp;
            return this;
        }
        public MaskEditClientSideEventsBuilder KeyPress(String keyPress)
        {
            maskEditModel.KeyPress = keyPress;
            return this;
        }
        public MaskEditClientSideEventsBuilder Change(String change)
        {
            maskEditModel.Change = change;
            return this;
        }
        public MaskEditClientSideEventsBuilder MouseOver(String mouseOver)
        {
            maskEditModel.MouseOver = mouseOver;
            return this;
        }
        public MaskEditClientSideEventsBuilder MouseOut(String mouseOut)
        {
            maskEditModel.MouseOut = mouseOut;
            return this;
        }
        public MaskEditClientSideEventsBuilder FocusIn(String focusIn)
        {
            maskEditModel.FocusIn = focusIn;
            return this;
        }
        public MaskEditClientSideEventsBuilder FocusOut(String focusOut)
        {
            maskEditModel.FocusOut = focusOut;
            return this;
        }
        public MaskEditClientSideEventsBuilder Destroy(String destroy)
        {
            maskEditModel.Destroy = destroy;
            return this;
        }
    }
}
