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
using Syncfusion.JavaScript.Mobile;
using Syncfusion.JavaScript.Mobile.Models;

namespace Syncfusion.JavaScript.Mobile
{
    /// <summary>
    /// Class for Client Side Event Builder
    /// </summary>
    public class MobileTileClientSideEventsBuilder
    {
        #region Fields
        /// <summary>
        /// The m tile model
        /// </summary>
        private MobileTileProperties mTileModel;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileTileClientSideEventsBuilder"/> class.
        /// </summary>
        /// <param name="mTileModel">The m tile model.</param>
        public MobileTileClientSideEventsBuilder(MobileTileProperties mTileModel)
        {
            this.mTileModel = mTileModel;
        }
        #endregion

        #region Events

        /// <summary>
        /// Touches the start.
        /// </summary>
        /// <param name="touchStart">The touch start.</param>
        /// <returns></returns>
        public MobileTileClientSideEventsBuilder TouchStart(string touchStart)
        {
            mTileModel.TouchStart = touchStart;
            return this;
        }

        /// <summary>
        /// Touches the end.
        /// </summary>
        /// <param name="touchEnd">The touch end.</param>
        /// <returns></returns>
        public MobileTileClientSideEventsBuilder TouchEnd(string touchEnd)
        {
            mTileModel.TouchEnd = touchEnd;
            return this;
        }
        #endregion
    }
}
