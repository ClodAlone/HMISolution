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

namespace Syncfusion.Olap.MDXQueryParser
{
    public enum Orderss
    {
        ASC,
        BASC,
        BDESC,
        DESC
    }

    public enum ParseFilterCase
    {
        /// <summary>
        /// FilterCase of Type Greater than or equal to 
        /// </summary>
        GreaterThanOrEqualTo,

        /// <summary>
        /// FilterCase of Type Less that or Equal to
        /// </summary>
        LessThanOrEqualTo,

        /// <summary>
        /// FilterCase of Type Not Equals
        /// </summary>
        NotEquals,

        /// <summary>
        /// FilterCase of Type Greater than
        /// </summary>
        GreaterThan,

        /// <summary>
        /// FilterCase of Type Less than
        /// </summary>
        LessThan,

        /// <summary>
        /// FilterCase of Type Equal to
        /// </summary>
        EqualTo
    }

    public enum ParseOperators
    {
        /// <summary>
        /// Filter Operator AND
        /// </summary>
        AND,

        /// <summary>
        /// Filter Operator OR
        /// </summary>
        OR,

        /// <summary>
        /// Filter Operator NOT
        /// </summary>
        NOT,

        /// <summary>
        /// Filter Operator XOR
        /// </summary>
        XOR
    }
}
