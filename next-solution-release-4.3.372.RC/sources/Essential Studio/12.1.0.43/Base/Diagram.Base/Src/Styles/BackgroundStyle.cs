#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Imitate from FillStyle type that used by TextNode node.
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(FillStyleConverter))]
    public class BackgroundStyle
        : FillStyle
    {
        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="BackgroundStyle"/> class.
        /// </summary>
        public BackgroundStyle()
            : base()
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BackgroundStyle"/> class.
        /// </summary>
        /// <param name="src">The source instance.</param>
        public BackgroundStyle(BackgroundStyle src)
            : base(src)
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BackgroundStyle"/> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The context.</param>
        protected BackgroundStyle(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>The cloned object.</returns>
        public override object Clone()
        {
            return new BackgroundStyle(this);
        }

        /// <summary>
        /// Gets the name of the property container.
        /// </summary>
        /// <returns>The property container name.</returns>
        protected override string GetPropertyContainerName()
        {
            return "BackgroundStyle";
        }
        #endregion
    }
}
