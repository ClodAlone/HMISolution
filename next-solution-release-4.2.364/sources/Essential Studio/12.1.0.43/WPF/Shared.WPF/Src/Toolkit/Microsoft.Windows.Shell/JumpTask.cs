#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
/**************************************************************************\
    Copyright Microsoft Corporation. All Rights Reserved.
\**************************************************************************/

namespace Syncfusion.Windows
{
    /// <summary>
    /// 
    /// </summary>
    public class JumpTask : JumpItem
    {
        /// <summary>
        /// 
        /// </summary>
        public JumpTask()
        {}
        /// <summary>
        /// 
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ApplicationPath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Arguments { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string WorkingDirectory { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string IconResourcePath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int IconResourceIndex { get; set; }
    }
}
