#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;

namespace Syncfusion.Documentation
{
    /// <exclude/>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Delegate
         | AttributeTargets.Enum | AttributeTargets.Event | AttributeTargets.Field
        | AttributeTargets.Interface | AttributeTargets.Method | AttributeTargets.Property
        | AttributeTargets.Struct | AttributeTargets.Constructor, AllowMultiple = false, Inherited = true),
    DocumentationExclude()]
    public class DocumentationExcludeAttribute : Attribute
    {
        public DocumentationExcludeAttribute()
        {
        }
    }
}
