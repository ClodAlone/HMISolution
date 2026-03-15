// <copyright file="CustomAnimationsCollection.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media.Animation;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents a list of custom animations for applying to the controls.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class CustomAnimationsCollection : List<CustomAnimation>
    {
        #region Private members
        /// <summary>
        /// Routed event upon which animations starts.
        /// </summary>
        private static RoutedEvent m_routedEvent;

        /// <summary>
        /// Name of the element in a visual tree that listens to the event.
        /// </summary>
        private static string m_sourceName;
        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomAnimationsCollection"/> class.
        /// </summary>
        public CustomAnimationsCollection()
        {
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Finds custom animation for given source name.
        /// </summary>
        /// <param name="sourceName">Source name to find animation for.</param>
        /// <param name="routedEvent">Routed event to find animation for.</param>
        /// <returns>
        /// Custom animation found for the given source name and routed event.
        /// </returns>
        public CustomAnimation FindAnimation(string sourceName, RoutedEvent routedEvent)
        {
            m_sourceName = sourceName;
            m_routedEvent = routedEvent;
            return Find(PredicateFindAnimationFor);
        }

        /// <summary>
        /// Initializes each contained custom animation. Method finds
        /// source and target elements, initializes target context and
        /// adds event handlers to source.
        /// </summary>
        /// <param name="ownerTemplateMappings">Any number of 
        /// structures containing owners and their templates in which
        /// source and target must be located.</param>
        public void Initialize(params OwnerTemlpateMapping[] ownerTemplateMappings)
        {
            foreach (CustomAnimation animation in this)
            {
                FrameworkElement source = null;
                FrameworkElement target = null;
                string targetName = Storyboard.GetTargetName(animation.Storyboard);

                foreach (OwnerTemlpateMapping mapping in ownerTemplateMappings)
                {
                    if (mapping.Template != null && mapping.Owner != null)
                    {
                        if (source == null)
                        {
                            source = mapping.Template.FindName(animation.SourceName, mapping.Owner) as FrameworkElement;
                        }

                        if (target == null)
                        {
                            target = mapping.Template.FindName(targetName, mapping.Owner) as FrameworkElement;
                        }
                    }
                }

                if (source == null)
                {
                    throw new NullReferenceException("Animation source cannot be found.");
                }

                if (target == null)
                {
                    throw new NullReferenceException("Animation target cannot be found.");
                }

                animation.InitializeTargetContext(target);
                source.AddHandler(animation.RoutedEvent, new RoutedEventHandler(OnCustomAnimationBegun));
            }
        }

        /// <summary>
        /// Initializes each contained custom animation finding source
        /// and target in the given owner's template.
        /// </summary>
        /// <param name="owner">Owner of the template.</param>
        /// <param name="template">Template in which to search for the source and target.</param>
        public void Initialize(FrameworkElement owner, FrameworkTemplate template)
        {
            OwnerTemlpateMapping mapping = new OwnerTemlpateMapping();
            mapping.Template = template;
            mapping.Owner = owner;

            Initialize(mapping);
        }

        /// <summary>
        /// Finds storyboard references by storyboard keys for each
        /// contained custom animation within given context.
        /// </summary>
        /// <param name="resourceContext">Context within which storyboards are located.</param>
        public void InitializeResources(FrameworkElement resourceContext)
        {
            foreach (CustomAnimation animation in this)
            {
                animation.InitializeStoryboard(resourceContext);
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Called when custom animation starts.
        /// </summary>
        /// <param name="sender">FrameworkElement object.</param>
        /// <param name="e">The instance containing the event data.</param>
        private void OnCustomAnimationBegun(object sender, RoutedEventArgs e)
        {
            FrameworkElement source = sender as FrameworkElement;
            CustomAnimation animation = FindAnimation(source.Name, e.RoutedEvent);
            animation.Storyboard.Begin(animation.TargetContext);
        }

        /// <summary>
        /// Predicate for finding custom animations.
        /// </summary>
        /// <param name="animation">Custom animation to check.</param>
        /// <returns>
        /// Value indicating whether given animations meets criteria.
        /// </returns>
        private static bool PredicateFindAnimationFor(CustomAnimation animation)
        {
            return animation.RoutedEvent == m_routedEvent && animation.SourceName == m_sourceName;
        }
        #endregion
    }
}
