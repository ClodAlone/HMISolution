// ---------------------------------------------------------------------------
// Campari Software
//
// WindowsFoundationLibrary.cs
//
//
// ---------------------------------------------------------------------------
// Copyright (C) 2006-2007 Campari Software
// All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
// FITNESS FOR A PARTICULAR PURPOSE.
// ---------------------------------------------------------------------------
using System;

namespace Campari.Software
{
    #region enum WindowsFoundationLibrary
    /// <summary>
    /// Specifies the .NET 3.0 Windows Foundation Library
    /// </summary>
    public enum WindowsFoundationLibrary
    {
        /// <summary>
        /// Windows Communication Foundation
        /// </summary>
        WCF,

        /// <summary>
        /// Windows Presentation Foundation
        /// </summary>
        WPF,

        /// <summary>
        /// Windows Workflow Foundation
        /// </summary>
        WF,

        /// <summary>
        /// Windows CardSpace
        /// </summary>
        CardSpace,
    }
    #endregion
}
