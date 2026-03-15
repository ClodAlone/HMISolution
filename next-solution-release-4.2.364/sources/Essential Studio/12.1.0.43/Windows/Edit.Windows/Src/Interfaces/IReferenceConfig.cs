#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

using System;

namespace Syncfusion.Windows.Forms.Edit.Interfaces
{
	/// <summary>
	/// Reference description.
	/// </summary>
  public interface IReferenceConfig
  {
    /// <summary>
    /// ID of the configuration, reference is linked to.
    /// </summary>
    int RefID{ get; }
    /// <summary>
    /// Referenced lexem.
    /// </summary>
    IConfigLexem ReferencedLexem{ get; }
	}
}
