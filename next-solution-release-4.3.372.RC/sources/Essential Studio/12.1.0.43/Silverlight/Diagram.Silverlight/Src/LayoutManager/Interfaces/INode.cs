// <copyright file="INodeGroup.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Syncfusion.Windows.Diagram
{
    /// <summary>
    /// Represents node collection properties.
    /// </summary>
    public interface INode
    {
        #region Properties
       
        double OffsetX
        {
            get;
            set;
        }

        double OffsetY
        {
            get;
            set;
        }

        double Width
        {
            get;
            set;
        }

        double Height
        {
            get;
            set;
        }

        Shapes Shape
        {
            get;
            set;
        }

        int Zorder
        {
            get;
            set;
        }
        #endregion
    }
}
