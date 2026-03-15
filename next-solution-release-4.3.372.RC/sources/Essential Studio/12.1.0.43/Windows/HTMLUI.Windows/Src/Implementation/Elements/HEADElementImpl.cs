#region Copyright Syncfusion Inc. 2001 - 2014

////  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
////  Use of this code is subject to the terms of our license.
////  A copy of the current license can be obtained at any time by e-mailing
////  licensing@syncfusion.com. Re-distribution in any form is strictly
////  prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using Syncfusion.Windows.Forms.HTMLUI.Implementation.Utility;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI
{
    /// <summary>
    /// Class that is responsible for &lt;HEAD&gt; Tag.
    /// </summary>
    [ElementTag(TagName.Head)]
    public class HEADElementImpl : BaseElement
    {
        #region Class constants

        /// <summary>
        /// Name of the tag this class indicates.
        /// </summary>
        private const string DEF_TAG_NAME = TagName.Head;

        /// <summary>
        /// Supported events.
        /// </summary>
        private static string[] DEF_SUPP_EVENTS;

        /// <summary>
        /// Holds all events.
        /// </summary>
        private static Hashtable m_eventHash;
  #endregion

        #region Class Properties
        /// <summary>
        /// Returns an array of supported events.
        /// </summary>
        public override string[] SupportedEvents
        {
            get
            {
                return new string[] { };
            }
        }

        /// <summary>
        /// Overridden. Gets or sets the visibility of the attribute
        /// </summary>
        public override bool IsVisible
        {
            get
            {
                if (!this.Attributes.Contains(DEF_RUNTIME_VISIBLE))
                {
                    this.Attributes.Add(DEF_RUNTIME_VISIBLE);
                    ((HTMLAttributeImpl)this.Attributes[DEF_RUNTIME_VISIBLE]).Value = false.ToString();
                }

                return false;
            }
            set
            {
                if (value != true)
                {
                    //// throw new ArgumentException( "Visibility of this element must be False.", "IsVisible" );

                    if (!this.Attributes.Contains(DEF_RUNTIME_VISIBLE))
                    {
                        this.Attributes.Add(DEF_RUNTIME_VISIBLE);
                        ((HTMLAttributeImpl)this.Attributes[DEF_RUNTIME_VISIBLE]).Value = false.ToString();
                    }
                }
            }
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes static members of the HEADElementImpl class 
        /// </summary>
        static HEADElementImpl()
        {
            Type type = typeof(HEADElementImpl);
            DEF_SUPP_EVENTS = FillEventsInfo(type, out m_eventHash);
        }

        /// <summary>
        /// Initializes a new instance of the HEADElementImpl class
        /// </summary>
        /// <param name="parent">Parent element.</param>
        public HEADElementImpl(IHTMLElement parent)
            : base(parent, DEF_TAG_NAME)
        {
        }

        #endregion

        #region Class Overrides
        /// <summary>
        /// Overridden. Returns an instance of the event.
        /// </summary>
        /// <param name="name">Name of the event.</param>
        /// <returns>Event object.</returns>
        protected override IHTMLEvent CreateEventInternal(string name)
        {
            return null;
        }

        /// <summary>
        /// Overridden. Calculates the size of the element for rendering.
        /// </summary>
        /// <returns>Size object</returns>
        protected override Size CalculateSizeInternal()
        {
            return this.Size;
        }

        /// <summary>
        /// Overridden. Calculates the position of the element for rendering.
        /// </summary>
        protected override void CalculatePositionInternal()
        {
            // This method is not called from anywhere. CalculateChildPositions is used.
        }

        /// <summary>
        /// Overridden. Calculates the format of the element from the array of possible formats.
        /// </summary>
        protected override void CalculateFormatInternal()
        {
            DefaultCalculateFormatInternal();

            this.IsVisible = false;
        }
        #endregion
    }
}