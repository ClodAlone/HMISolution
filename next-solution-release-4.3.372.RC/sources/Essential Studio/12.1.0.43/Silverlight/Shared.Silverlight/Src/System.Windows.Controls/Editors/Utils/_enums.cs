// <copyright file="_enums.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents the CurrencySymbol Position 
    /// </summary>
    public enum CurrencySymbolPosition
    {
        /// <summary>
        /// sets the CurrencySymbol position to Left
        /// </summary>
        Left,

        /// <summary>
        /// sets the CurrencySymbol position to Right
        /// </summary>
        Right
    }

    /// <summary>
    /// Specifies MinValue validation constraint for Numaric Editors controls.
    /// </summary>
    public enum MinValidation
    {
        /// <summary>
        /// 
        /// </summary>
        OnKeyPress = 0,
        /// <summary>
        /// 
        /// </summary>
        OnLostFocus = 1
    }

    /// <summary>
    /// Specifies MaxValue validation constraint for Numaric Editors controls.
    /// </summary>
    public enum MaxValidation
    {
        /// <summary>
        /// 
        /// </summary>
        OnKeyPress = 0,
        /// <summary>
        /// 
        /// </summary>
        OnLostFocus = 1
    }

    /// <summary>
    /// Specifies how watermark text in an editor control should hides.
    /// </summary>    
    public enum WatermarkTextMode
    {
        /// <summary>
        /// The WatermarkText hides when focus is moved to the control.
        /// </summary>
        HideTextOnFocus = 0,

        /// <summary>
        /// The WatermarkText hides when user start typing.
        /// </summary>
        HideTextOnTyping = 1
    }

    /// <summary>
    /// Specifies how to format the text inside the MaskedTextBox
    /// </summary>
    public enum MaskFormat
    {
        /// <summary>
        /// Return only text input by the user.
        /// </summary>
        ExcludePromptAndLiterals = 0,

        /// <summary>
        /// Return text input by the user as well as any instances of the prompt character.
        /// </summary>
        IncludePrompt = 1,

        /// <summary>
        /// Return text input by the user as well as any literal characters defined in the mask.
        /// </summary>
        IncludeLiterals = 2,

        /// <summary>
        /// Return text input by the user as well as any literal characters defined in the mask and any instances of the prompt character.
        /// </summary>
        IncludePromptAndLiterals = 3,
    }

    /// <summary>
    /// Specifies String Validation constraint for MaskedTextBox control.
    /// </summary>
    public enum StringValidation
    {
        /// <summary>
        /// 
        /// </summary>
        OnKeyPress = 0,
        /// <summary>
        /// 
        /// </summary>
        OnLostFocus = 1
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
    /// Specifies the way of <see cref="MaskedTextBox"/> reaction on wrong input data.
    /// </summary>
    public enum InvalidInputBehavior
    {
        /// <summary>
        /// Represents the way when there is no reaction onto invalid input.
        /// </summary>
        None,

        /// <summary>
        /// Raises displaying error message.
        /// </summary>
        DisplayErrorMessage,

        /// <summary>
        /// Resets text in editor.
        /// </summary>
        ResetValue
    }

    /// <summary>
    /// Specifies editing way of text in <see cref="PercentTextBox"/> control.
    /// </summary>
    public enum PercentEditMode
    {
        /// <summary>
        /// The values while editing are shown as percent values.
        /// </summary>
        PercentMode,

        /// <summary>
        /// The values while editing are shown as double values and it should be converted to percentage value once the control loses focus.
        /// </summary>
        DoubleMode
    }


    /// <summary>
    /// Represents the Flow Direction.
    /// </summary>
    public enum FlowDirection
    {
        /// <summary>
        /// Sets the CurrencySymbol FlowDirection as LeftToRight
        /// </summary>
        LeftToRight,
        /// <summary>
        /// Sets the CurrencySymbol FlowDirection as RightToLeft
        /// </summary>
        RightToLeft        
    }
}
