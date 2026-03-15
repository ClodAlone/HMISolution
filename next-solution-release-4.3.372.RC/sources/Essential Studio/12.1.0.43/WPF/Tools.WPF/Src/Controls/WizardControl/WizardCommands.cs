// <copyright file="WizardCommands.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Exposes a set of RoutedCommands that can be used to control the <see cref="WizardControl"/> UI.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class WizardCommands
    {
        /// <summary>
        /// Cancel Command
        /// </summary>
        private static RoutedCommand cancel = new RoutedCommand();
        
        /// <summary>
        /// Previous Command
        /// </summary>
        private static RoutedCommand previous = new RoutedCommand();
        
        /// <summary>
        /// Next Command
        /// </summary>
        private static RoutedCommand next = new RoutedCommand();
        
        /// <summary>
        /// Finish Command
        /// </summary>
        private static RoutedCommand finish = new RoutedCommand();
        
        /// <summary>
        /// Help Command
        /// </summary>
        private static RoutedCommand help = new RoutedCommand();
        
        /// <summary>
        /// Select page Command
        /// </summary>
        private static RoutedCommand selectPage = new RoutedCommand();

        /// <summary>
        /// Gets the cancel.
        /// </summary>
        /// <value>The cancel.</value>
        public static RoutedCommand Cancel 
        { 
            get 
            { 
                return cancel; 
            } 
        }

        /// <summary>
        /// Gets the previous.
        /// </summary>
        /// <value>The previous.</value>
        public static RoutedCommand Previous 
        { 
            get 
            { 
                return previous; 
            } 
        }
        
        /// <summary>
        /// Gets the next.
        /// </summary>
        /// <value>The next Command.</value>
        public static RoutedCommand Next 
        { 
            get 
            { 
                return next; 
            } 
        }

        /// <summary>
        /// Gets the finish.
        /// </summary>
        /// <value>The finish.</value>
        public static RoutedCommand Finish 
        { 
            get 
            { 
                return finish; 
            } 
        }

        /// <summary>
        /// Gets the help.
        /// </summary>
        /// <value>The help command.</value>
        public static RoutedCommand Help 
        { 
            get 
            { 
                return help; 
            } 
        }

        /// <summary>
        /// Gets the select page.
        /// </summary>
        /// <value>The select page.</value>
        public static RoutedCommand SelectPage 
        { 
            get 
            { 
                return selectPage; 
            } 
        }
    }
}

