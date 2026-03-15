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

namespace Syncfusion.Windows.Forms.Tools
{
	/// <summary>
	/// The delegate for handling the ValidationError event.
	/// </summary>
	/// <remarks>
	/// Refer to the <see cref="MaskedEditBox.ValidationError"/> event for more information.
	/// </remarks>
	public delegate void ValidationErrorEventHandler(object sender, ValidationErrorArgs e);
	
	/// <summary>
	/// The ValidationErrorEventArgs class is used to send event data for a <see cref="MaskedEditBox.ValidationError"/>
	/// event.
	/// </summary>
	/// <remarks>
	/// The required pieces of information for the <see cref="MaskedEditBox.ValidationError"/> event 
	/// are the invalid text and the position of the error text within the invalid text.
	/// </remarks>
	public class ValidationErrorArgs : EventArgs
	{
		/// <summary>
		/// The invalid text.
		/// </summary>
		private string invalidText;


		/// <summary>
		/// The start position of the error.
		/// </summary>
		private int startPosition;

		/// <summary>
		/// The error message.
		/// </summary>
		private string errorMessage = "Invalid content.";

		/// <summary>
		/// Overloaded. Creates an object of type ValidationErrorArgs.
		/// </summary>
		/// <param name="invalidText">The invalid text that would have resulted if this error had not been intercepted.</param>
		/// <param name="startPosition">The index position with the invalid text where the change occurred.</param>
		public ValidationErrorArgs(string invalidText, int startPosition)
		{
			this.invalidText = invalidText;
			this.startPosition = startPosition;
		}

		
		/// <summary>
		/// Creates an object of type ValidationErrorArgs.
		/// </summary>
		/// <param name="invalidText">The invalid text that would have resulted if this error had not been intercepted.</param>
		/// <param name="startPosition">The index position with the invalid text where the change occurred.</param>
		/// <param name="errorMessage">The error message.</param>
		public ValidationErrorArgs(string invalidText, int startPosition, string errorMessage)
		{
			this.invalidText = invalidText;
			this.startPosition = startPosition;
			this.errorMessage = errorMessage;
		}

		/// <summary>
		/// Returns the invalid text as it would have been if the error had not intercepted it.
		/// </summary>
		public string InvalidText
		{
			get{return this.invalidText;}
		}

		/// <summary>
		/// Returns the location of the invalid input within the
		/// invalid text.
		/// </summary>
		public int StartPosition
		{
			get{return this.startPosition;}
		}

		/// <summary>
		/// Returns the error message.
		/// </summary>
		public string ErrorMessage
		{
			get{return this.errorMessage;}
		}
	}
}
