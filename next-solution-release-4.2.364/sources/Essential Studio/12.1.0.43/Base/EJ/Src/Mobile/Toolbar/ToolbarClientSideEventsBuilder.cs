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
using Syncfusion.JavaScript.Mobile.Models;

namespace Syncfusion.JavaScript.Mobile
{
    public class MobileToolbarClientSideEventsBuilder
    {
        #region Fields
        private MobileToolbarProperties mobileToolbarModel;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileToolbarClientSideEventsBuilder"/> class.
        /// </summary>
        /// <param name="mobileToolbarModel">The mobile toolbar model.</param>
        public MobileToolbarClientSideEventsBuilder(MobileToolbarProperties mobileToolbarModel)
        {
            this.mobileToolbarModel = mobileToolbarModel;
        }
        #endregion

        #region Events

        /// <summary>
        /// Touches the start.
        /// </summary>
        /// <param name="touchStart">The touch start.</param>
        /// <returns></returns>
        public MobileToolbarClientSideEventsBuilder TouchStart(string touchStart)
        {
            mobileToolbarModel.TouchStart = touchStart;
            return this;
        }


        /// <summary>
        /// Touches the end.
        /// </summary>
        /// <param name="touchEnd">The touch end.</param>
        /// <returns></returns>
        public MobileToolbarClientSideEventsBuilder TouchEnd(string touchEnd)
        {
            mobileToolbarModel.TouchEnd = touchEnd;
            return this;
        }

        /// <summary>
        /// Creates the specified create.
        /// </summary>
        /// <param name="create">The create.</param>
        /// <returns></returns>
        public MobileToolbarClientSideEventsBuilder Create(string create)
        {
            mobileToolbarModel.Create = create;
            return this;
        }

        /// <summary>
        /// Destroys the specified destroy.
        /// </summary>
        /// <param name="destroy">The destroy.</param>
        /// <returns></returns>
        public MobileToolbarClientSideEventsBuilder Destroy(string destroy)
        {
            mobileToolbarModel.Destroy = destroy;
            return this;
        }

        #endregion
    }
}
