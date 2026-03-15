// <copyright file="CustomAnimation.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media.Animation;
using System.Windows.Markup;
using System.Windows;
using System.Reflection;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// This class is used to store custom (user defined) animations.
    /// In order to store an animation properly, event source name,
    /// target name, routed event, storyboard or storyboard key must
    /// be set.
    /// </summary>
    public class CustomAnimation
    {
        #region Class members
        /// <summary>
        /// User defined storyboard key.
        /// </summary>
        private object m_storyboardKey;
        
        /// <summary>
        /// Any registered routed event.
        /// </summary>
        private RoutedEvent m_routedEvent;
        
        /// <summary>
        /// Name of the element in a visual tree which will listen to a routed event.
        /// </summary>
        private string m_sourceName;
        
        /// <summary>
        /// Context within which a target element is located.
        /// </summary>
        private FrameworkElement m_targetContext;
        
        /// <summary>
        /// User defined storyboard.
        /// </summary>
        private Storyboard m_storyboard;
        #endregion

        #region Class public properties
        /// <summary>
        /// Gets or sets routed event used for starting user defined animation.
        /// </summary>
        /// <value>
        /// Type: <see cref="RoutedEvent"/>
        /// </value>
        /// <seealso cref="RoutedEvent"/>
        public RoutedEvent RoutedEvent
        {
            get
            {
                return m_routedEvent;
            }

            set
            {
                m_routedEvent = value;
            }
        }
        
        /// <summary>
        /// Gets or sets a name of the element for listening to
        /// routed event to start user defined animation.
        /// </summary>
        /// <value>
        /// Type: <see cref="string"/>
        /// </value>
        /// <seealso cref="string"/>
        public string SourceName
        {
            get
            {
                return m_sourceName;
            }

            set
            {
                m_sourceName = value;
            }
        }
        
        /// <summary>
        /// Gets or sets a key of user defined storyboard.
        /// </summary>
        /// <value>
        /// Type: <see cref="object"/>
        /// </value>
        /// <seealso cref="object"/>
        public object StoryboardKey
        {
            get
            {
                return m_storyboardKey;
            }

            set
            {
                m_storyboardKey = value;
            }
        }
        
        /// <summary>
        /// Gets or sets user defined storyboard.
        /// </summary>
        /// <value>
        /// Type: <see cref="Storyboard"/>
        /// </value>
        /// <seealso cref="Storyboard"/>
        public Storyboard Storyboard
        {
            get
            {
                return m_storyboard;
            }

            set
            {
                m_storyboard = value;
            }
        }
        #endregion

        #region Class internal properties
        /// <summary>
        /// Gets context within which target element is located.
        /// </summary>
        /// <value>
        /// Type: <see cref="FrameworkElement"/>
        /// </value>
        /// <seealso cref="FrameworkElement"/>
        internal FrameworkElement TargetContext
        {
            get
            {
                return m_targetContext;
            }
        }
        
        /// <summary>
        /// Gets a value indicating whether user defined storyboard
        /// reference in present.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// </value>
        /// <seealso cref="bool"/>
        internal bool HasStoryboardValue
        {
            get
            {
                return m_storyboard != null;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomAnimation"/> class.
        /// </summary>
        public CustomAnimation()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomAnimation"/> class.
        /// </summary>
        /// <param name="sourceName">Name of the source.</param>
        /// <param name="routedEvent">The routed event.</param>
        /// <param name="storyboardKey">The storyboard key.</param>
        public CustomAnimation(string sourceName, RoutedEvent routedEvent, object storyboardKey)
        {
            m_sourceName = sourceName;
            m_routedEvent = routedEvent;
            m_storyboardKey = storyboardKey;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomAnimation"/> class.
        /// </summary>
        /// <param name="sourceName">Name of the source.</param>
        /// <param name="routedEvent">The routed event.</param>
        /// <param name="storyboard">The storyboard.</param>
        public CustomAnimation(string sourceName, RoutedEvent routedEvent, Storyboard storyboard)
        {
            m_storyboard = storyboard;
            m_sourceName = sourceName;
            m_routedEvent = routedEvent;
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Finds storyboard within the given context.
        /// </summary>
        /// <param name="storyboardContext">The context within which the
        /// storyboard is located.</param>
        internal void InitializeStoryboard(FrameworkElement storyboardContext)
        {
            if (m_storyboard == null)
            {
                m_storyboard = storyboardContext.FindResource(m_storyboardKey) as Storyboard;
            }
        }
        
        /// <summary>
        /// Stores the context within which the target element is located.
        /// </summary>
        /// <param name="targetContext">The context which should be stored.</param>
        internal void InitializeTargetContext(FrameworkElement targetContext)
        {
            m_targetContext = targetContext;
        }
        #endregion
    }
}
