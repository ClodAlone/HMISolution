#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using System.Web;
using Syncfusion.JavaScript.DataSources;
using Syncfusion.JavaScript.Mobile;
using Syncfusion.JavaScript.Mobile.Models;

namespace Syncfusion.JavaScript.Mobile
{
    public class MobileGroupButtonPropertiesBuilder
    {
        #region Fields

        private MobileGroupButton mobileGroupButton;
       
        internal List<MobileGroupButtonBaseItem> ItemsCollection { get; set; }
        
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MobileGroupButtonPropertiesBuilder"/> class.
        /// </summary>
        /// <param name="GroupButton">The mobileGroupButton.</param>
        public MobileGroupButtonPropertiesBuilder(MobileGroupButton mobileGroupButton)
        {
            this.mobileGroupButton = new MobileGroupButton(mobileGroupButton.ID, mobileGroupButton.MobileGroupButtonModel);

        }

        #endregion

        #region Builder

        /// <summary>
        /// Buttonses the specified button.
        /// </summary>
        /// <param name="button">The button.</param>
        /// <returns></returns>
        public MobileGroupButtonPropertiesBuilder Buttons(Action<MobileGroupButtonBaseItemAdder> button)
        {
            this.ItemsCollection = new List<MobileGroupButtonBaseItem>();
            MobileGroupButtonBaseItemAdder mobileGroupButtonAdder = new MobileGroupButtonBaseItemAdder(mobileGroupButton.MobileGroupButtonModel.Buttons);
            button.Invoke(mobileGroupButtonAdder);
            return this;
        }


       /// <summary>
       /// Renders the mode.
       /// </summary>
       /// <param name="renderMode">The render mode.</param>
       /// <returns></returns>
       public MobileGroupButtonPropertiesBuilder RenderMode(RenderMode renderMode)
        {
            mobileGroupButton.MobileGroupButtonModel.RenderMode = renderMode;
            return this;
        }


       /// <summary>
       /// Themes the specified theme.
       /// </summary>
       /// <param name="theme">The theme.</param>
       /// <returns></returns>
       public MobileGroupButtonPropertiesBuilder Theme(Theme theme)
        {
            mobileGroupButton.MobileGroupButtonModel.Theme = theme;
            return this;
        }

       /// <summary>
       /// Identifiers the specified identifier.
       /// </summary>
       /// <param name="id">The identifier.</param>
       /// <returns></returns>
       public MobileGroupButtonPropertiesBuilder Name(string name)
       {
           mobileGroupButton.MobileGroupButtonModel.Name = name;
           return this;
       }

       /// <summary>
       /// Groups the type of the button.
       /// </summary>
       /// <param name="groupButtonType">Type of the group button.</param>
       /// <returns></returns>
       public MobileGroupButtonPropertiesBuilder GroupButtonType(GroupButtonType groupButtonType)
       {
           mobileGroupButton.MobileGroupButtonModel.GroupButtonType = groupButtonType;
           return this;
       }

        /// <summary>
        /// Clients the side events.
        /// </summary>
        /// <param name="clientSideEvents">The client side events.</param>
        /// <returns></returns>
       public MobileGroupButtonPropertiesBuilder ClientSideEvents(Action<MobileGroupButtonClientSideEventsBuilder> clientSideEvents)
       {
           var builder = new MobileGroupButtonClientSideEventsBuilder(this.mobileGroupButton.MobileGroupButtonModel);
           if (clientSideEvents != null)
               clientSideEvents.Invoke(builder);
           return this;
       }

        #endregion

        #region Render

        /// <summary>
        /// Renders this instance.
        /// </summary>
        /// <returns></returns>
        public HtmlString Render()
        {
            return new HtmlString(mobileGroupButton.Render().ToString());
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override String ToString()
        {
            return Render().ToString();
        }
        #endregion
    }
}
