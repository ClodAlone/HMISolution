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
using Syncfusion.JavaScript.Mobile.Models;

namespace Syncfusion.JavaScript.Mobile
{
    /// <summary>
    /// Class for Auto Complete Client side events
    /// </summary>
    public class MobileAutoCompleteClientSideEventsBuilder
    {
        #region Fields
        private MobileAutoCompleteProperties mAutoCompleteModel;
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MobileAutoCompleteClientSideEventsBuilder"/> class.
        /// </summary>
        /// <param name="mAutoCompleteModel">The m automatic complete model.</param>
        public MobileAutoCompleteClientSideEventsBuilder(MobileAutoCompleteProperties mAutoCompleteModel)
        {
            this.mAutoCompleteModel = mAutoCompleteModel;
        }
        #endregion

        #region Events

        /// <summary>
        /// Touches the end.
        /// </summary>
        /// <param name="touchEnd">The touch end.</param>
        /// <returns></returns>
        public MobileAutoCompleteClientSideEventsBuilder TouchEnd(string touchEnd)
        {
            mAutoCompleteModel.TouchEnd = touchEnd;
            return this;
        }

        /// <summary>
        /// Keys the press.
        /// </summary>
        /// <param name="keyPress">The key press.</param>
        /// <returns></returns>
        public MobileAutoCompleteClientSideEventsBuilder KeyPress(string keyPress)
        {
            mAutoCompleteModel.KeyPress = keyPress;
            return this;
        }

        /// <summary>
        /// Selects the specified select.
        /// </summary>
        /// <param name="select">The select.</param>
        /// <returns></returns>
        public MobileAutoCompleteClientSideEventsBuilder Select(string select)
        {
            mAutoCompleteModel.Select = select;
            return this;
        }

        /// <summary>
        /// Changes the specified change.
        /// </summary>
        /// <param name="change">The change.</param>
        /// <returns></returns>
        public MobileAutoCompleteClientSideEventsBuilder Change(string change)
        {
            mAutoCompleteModel.Change = change;
            return this;
        }

        /// <summary>
        /// Focuses the in.
        /// </summary>
        /// <param name="focusIn">The focus in.</param>
        /// <returns></returns>
        public MobileAutoCompleteClientSideEventsBuilder FocusIn(string focusIn)
        {
            mAutoCompleteModel.FocusIn = focusIn;
            return this;
        }

        /// <summary>
        /// Focuses the out.
        /// </summary>
        /// <param name="focusOut">The focus out.</param>
        /// <returns></returns>
        public MobileAutoCompleteClientSideEventsBuilder FocusOut(string focusOut)
        {
            mAutoCompleteModel.FocusOut = focusOut;
            return this;
        }
        #endregion
    }
}
