#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
// All other rights reserved.
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
using System.ComponentModel;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public class CustomWindowProperties
    {
        /// <summary>
        /// 
        /// </summary>
        public CustomWindowProperties()
        {
            _buttonTexts = new string[3];
        }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>The dialog button.</value>
        public DialogButton DialogButton { get; set; }

        /// <summary>
        /// Gets or sets the dialog icon.
        /// </summary>
        /// <value>The dialog icon.</value>
        public DialogIcon DialogIcon { get; set; }

        /// <summary>
        /// Gets or sets the prompt text.
        /// </summary>
        /// <value>The prompt text.</value>
        public string PromptText { get; set; }

        /// <summary>
        /// Gets or sets the ImageSource.
        /// </summary>
        /// <value>The ImageSource.</value>
        public ImageSource ImageSource { get; set; }

        /// <summary>
        /// Gets or sets the Title.
        /// </summary>
        /// <value>The Title.</value>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the AnimationType.
        /// </summary>
        /// <value>The AnimationType.</value>
        public AnimationType Animation { get; set; }

        /// <summary>
        /// Gets or sets the ClosedEventHandler.
        /// </summary>
        /// <value>The ClosedEventHandler.</value>
        public ClosedEventHandler ClosedEventHandler { get; set; }

        /// <summary>
        /// Gets or sets the title bar Foreground..
        /// </summary>
        /// <value>The TitleBarBackground.</value>
        public Brush TitleBarBackground { get; set; }

        /// <summary>
        /// Gets or sets the title bar background..
        /// </summary>
        /// <value>The TitleBarBackground.</value>
        public Brush TitleBarForeground { get; set; }

        /// <summary>
        /// Gets or sets the title bar Visibility..
        /// </summary>
        /// <value>The Visibility.</value>
        public Visibility TitleBarVisibility { get; set; }

        /// <summary>
        /// Gets or sets the window startup location..
        /// </summary>
        /// <value>The WindowStartupLocation.</value>
        public WindowStartupLocation WindowStartupLocation { get; set; }

        /// <summary>
        /// Set the template for window
        /// </summary>
        /// <value>The ContentTemplate.</value>
        public DataTemplate ContentTemplate { get; set; }

        private string[] _buttonTexts;

        /// <summary>
        /// ButtonTexts value should be maximum of Three values
        /// </summary>        
        public string[] ButtonTexts
        {
            get
            {
                return _buttonTexts;
            }
        }

        internal WindowControl window;

        /// <summary>
        /// To close the window which created using template
        /// </summary>
        public void Close()
        {
            if (window != null)
            {
                window.Close();
                window = null;
            }
        }

    }
}