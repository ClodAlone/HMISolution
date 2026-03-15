#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Syncfusion.Windows.Tools.Controls
{
  /// <summary>
    /// Represents the DropMode Enumeration.
    /// </summary>
    public enum DropMode
    {
        /// <summary>
        /// Dropping Up
        /// </summary>
        DropUp,

        /// <summary>
        /// Dropping Middle
        /// </summary>
        //DropMiddle,
        DropOver,

        /// <summary>
        /// Dropping Down
        /// </summary>
        DropDown               
    }
    /// <summary>
    /// Represents the Sort Mode Enumeration.
    /// </summary>
    public enum SortMode
    {
        /// <summary>
        /// Ascending flow
        /// </summary>
        Asc,

        /// <summary>
        /// Descending flow
        /// </summary>
        Desc
    }   

    /// <summary>
    /// Specifies the way of <see cref="MaskedTextBox"/> reaction on wrong input data.
    /// </summary>
    public enum InvalidInputBehavior
    {
        /// <summary>
        /// Represents the way when there is no reaction onto invalid input.
        /// </summary>
        None,

        /// <summary>
        /// Displays a error message. (NOTE: This is not yet implemented)
        /// </summary>
        DisplayErrorMessage,

        /// <summary>
        /// Resets text in editor.
        /// </summary>
        ResetValue
    }

    /// <summary>
    /// Specifies register of input symbols.
    /// </summary>    
    public enum ShiftStatus
    {
        /// <summary>
        /// No register changes after symbol was input.
        /// </summary>
        None,

        /// <summary>
        /// Register of typed symbol will be transferred to upper case.
        /// </summary>
        Uppercase,

        /// <summary>
        /// Register of typed symbol will be transferred to lower case.
        /// </summary>
        Lowercase
    }

    /// <summary>
    /// 
    /// </summary>
	public enum Modes
    {
        /// <summary>
        /// 
        /// </summary>
        Normal,

        /// <summary>
        /// 
        /// </summary>
        RadioGroup,

        /// <summary>
        /// 
        /// </summary>
        Checked
    }

    /// <summary>
    /// 
    /// </summary>
    public enum CheckedBoxAlignment
    {
        /// <summary>
        /// sets the CheckBox Alignment to Left
        /// </summary>
        Left,

        /// <summary>
        /// sets the CheckBox Alignment to Right
        /// </summary>
        Right
    }
   
}
