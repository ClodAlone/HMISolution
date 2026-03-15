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
using System.Text.RegularExpressions;

namespace Syncfusion.Windows.Forms.Edit.Interfaces
{
  /// <summary>
  /// Interface, that must be supported by lexem configurations.
  /// It describes behaviour of the lexem, that can be collapsed.
  /// </summary>
	public interface ICollapsableConfigLexem : IConfigLexem
	{
    /// <summary>
    /// GET checks whether this config supports collapsing.
    /// </summary>
    /// <remarks>
    /// If lexem is not collapsabe, all other properties does not have
    /// any influence to it`s behaviour.
    /// </remarks>
    bool IsCollapsable{ get; }
    /// <summary>
    /// GET flag, that specifies, whether lexem
    /// can read it`s text from sub-lexems.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If this property is false, then CollapseName will
    /// be used to name the region.
    /// </para>
    /// <para>
    /// Note: you should know that is this property is true, then on
    /// by-lexem reading you`ll have to wait while sub-lexems will be loaded,
    /// then processed by RegEx and only then you`ll have your collapsible lexem.
    /// </para>
    /// </remarks>
    bool IsCollapseAutoNamed{ get; }
    /// <summary>
    /// Expression for auto-naming.
    /// </summary>
    /// <remarks>
    /// When parsing collapsed region, parser reads text from stream until text matches given expression.
    /// You can specifie different named groups here to use them later in AutoNameTemplate.
    /// </remarks>
    string AutoNameExpression{ get; }
    /// <summary>
    /// Template of resulting text for auto-naming of collapse.
    /// </summary>
    /// <remarks>
    /// To specifie "$" symbol you must use $$.
    /// To use result of some named group, use ${name}.
    /// </remarks>
    string AutoNameTemplate{ get; }
    /// <summary>
    /// Regular expression instance for AutoNameExpression.
    /// </summary>
    Regex AutoNameRegex{ get; }
    /// <summary>
    /// Name of the collapsed region.
    /// </summary>
    /// <remarks>
    /// If <see cref="IsCollapseAutoNamed"/> is true, then
    /// this property`s value will be used just when
    /// found name is empty.
    /// </remarks>
    string CollapseName{ get; }
	}
}
