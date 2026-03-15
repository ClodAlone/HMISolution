//-------------------------------------------------------------------------------------------------
// <copyright file="ModelCommon.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Runtime.Serialization;

namespace Syncfusion.Olap.Manager
{
    /// <summary>
    /// OlapDataManagerException raised when user tries to perform
    /// restricted operation in OlapDataManager.
    /// </summary>
    [Serializable]
    public class OlapDataManagerException
      : ApplicationException
    {
        #region Class constants
        /// <summary>
        /// Default message to show when exception fired.
        /// </summary>
        private const string C_message = @"This operation is invalid in current context.";
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Constructor for OlapDataManagerException
        /// </summary>
        public OlapDataManagerException()
            : this(C_message)
        {
        }

        /// <summary>
        /// constructor for OlapDataManagerException
        /// </summary>
        /// <param name="innerExc">inner exeption.</param>
        public OlapDataManagerException(Exception innerExc)
            : this(C_message, innerExc)
        {
        }

        /// <summary>
        /// constructor for OlapDataManagerException
        /// </summary>
        /// <param name="message">message to show.</param>
        public OlapDataManagerException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// constructor for OlapDataManagerException
        /// </summary>
        /// <param name="message">message to show.</param>
        /// <param name="innerExc">inner exception.</param>
        public OlapDataManagerException(string message, Exception innerExc)
            : base(message, innerExc)
        {
        }

        /// <summary>
        /// constructor for OlapDataManagerException
        /// </summary>
        /// <param name="info">info serialization</param>
        /// <param name="context">context streaming</param>
        public OlapDataManagerException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        #endregion
    }

    /// <summary>
    /// An extension class.
    /// </summary>
    public static class Extension
    {
        #region Public Methods
        /// <summary>
        /// Returns true when Current object is matched with ANY param.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static bool In<T>(this T obj, params T[] values)
        {
            return values.Any(o => object.Equals(obj, o));
            //return Array.Exists(values, o => object.Equals(obj, o));
        }
        #endregion
    }
}
