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

#region file using directives
using System.Diagnostics;
#endregion

namespace Syncfusion.Scripting
{
  /// <summary>
  /// Class help to simply version switching
  /// </summary>
  public class Library
  {
    /// <summary>
    /// Current version of library controls
    /// </summary>
    public const string Version = "Version=2.0.5.1";
    /// <summary>
    /// Culture for all controls
    /// </summary>
    public const string Culture = "Culture=neutral";
    /// <summary>
    /// Public key for all controls
    /// </summary>
    public const string PublicKey = "PublicKeyToken=3d67ed1f87d44c89";
    /// <summary>
    /// Version of current library with culture and public key
    /// </summary>
    public const string OwnVersion = "Syncfusion.Scripting.Engine, " + Version + ", " + Culture + ", " + PublicKey;
    /// <summary>
    /// Designer library version with culture and public key
    /// </summary>
    public const string DesignerVersion = "Syncfusion.Scripting.Design, " + Version + ", " + Culture + ", " + PublicKey;
    /// <summary>
    ///
    /// </summary>
    //public const string ImageIndexEditor = "System.Windows.Forms.Design.ImageIndexEditor, System.Design, Version=1.0.3300.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";
  }
}