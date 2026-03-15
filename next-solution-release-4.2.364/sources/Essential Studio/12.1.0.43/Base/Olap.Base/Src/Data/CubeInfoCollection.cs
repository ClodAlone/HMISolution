//-------------------------------------------------------------------------------------------------
// <copyright file="CubeInfoCollection.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if !SILVERLIGHT
namespace Syncfusion.Olap.Data
{
    /// <summary>
    /// Collection of cube info objects
    /// </summary>
    /// <remarks>
    /// Gets all the cube information from AdomdProvider's Current connection state
    /// </remarks>
    [Serializable]
    public class CubeInfoCollection : CollectionBase
    {
#else
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
namespace Syncfusion.OlapSilverlight.Data
{
    /// <summary>
    /// Collection of cube info objects
    /// </summary>
    /// <remarks>
    /// Gets all the cube information from AdomdProvider's Current connection state
    /// </remarks>
    [CollectionDataContract]
    public class CubeInfoCollection : Collection<CubeInfo>
    {
#endif
       
#if !SILVERLIGHT
        #region Public Properties
        /// <summary>
        /// Gets or sets the <see cref="Syncfusion.Olap.Data.CubeInfo"/> at the specified index.
        /// </summary>
        /// <value></value>

        public CubeInfo this[int index]
        {
            get{ return (CubeInfo)base.List[index]; }
            set{ base.List[index] = value; }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Adds the CubeInfo object to the collection
        /// </summary>
        /// <param name="cubeInfo">The cube info.</param>
        /// <returns>returns the index of the last value in CubeInfoCollection</returns>
        public int Add(CubeInfo cubeInfo)
        {
            return base.List.Add(cubeInfo);
        }

        /// <summary>
        /// Removes the specified cube info.
        /// </summary>
        /// <param name="cubeInfo">The cube info.</param>
        public void Remove(CubeInfo cubeInfo)
        {
            base.List.Remove(cubeInfo);
        }
         #endregion
#endif

    }
}
