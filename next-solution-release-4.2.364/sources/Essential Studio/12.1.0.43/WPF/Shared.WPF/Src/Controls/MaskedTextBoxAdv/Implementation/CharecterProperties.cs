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
    /// 
    /// </summary>
    internal interface ICharacterProperties
    {
        /// <summary>
        /// Gets or sets the reg expression.
        /// </summary>
        /// <value>The reg expression.</value>
        string RegExpression { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is literal.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is literal; otherwise, <c>false</c>.
        /// </value>
        bool IsLiteral { get; set; }
        /// <summary>
        /// Gets or sets the is upper.
        /// </summary>
        /// <value>The is upper.</value>
        bool? IsUpper { get; set; }
        /// <summary>
        /// Gets or sets the is prompt character.
        /// </summary>
        /// <value>The is prompt character.</value>
        bool? IsPromptCharacter { get; set; }
    }


    public class CharacterProperties : ICharacterProperties
    {
        private string _regExpression;
        /// <summary>
        /// Gets or sets the reg expression.
        /// </summary>
        /// <value>The reg expression.</value>
        public string RegExpression
        {
            get { return _regExpression; }
            set { _regExpression = value; }
        }

        private bool _isLiteral;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is literal.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is literal; otherwise, <c>false</c>.
        /// </value>
        public bool IsLiteral
        {
            get { return _isLiteral; }
            set { _isLiteral = value; }
        }

        private bool? _isUpper;
        /// <summary>
        /// Gets or sets the is upper.
        /// </summary>
        /// <value>The is upper.</value>
        public bool? IsUpper
        {
            get { return _isUpper; }
            set { _isUpper = value; }
        }

        private bool? _isPromptchar;
        /// <summary>
        /// Gets or sets the is prompt character.
        /// </summary>
        /// <value>The is prompt character.</value>
        public bool? IsPromptCharacter
        {
            get { return _isPromptchar; }
            set { _isPromptchar = value; }
        }
    }
}
