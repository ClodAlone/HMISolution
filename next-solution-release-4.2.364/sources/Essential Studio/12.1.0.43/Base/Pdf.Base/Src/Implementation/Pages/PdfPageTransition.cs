#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;

using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;

namespace Syncfusion.Pdf
{
    /// <summary>
    /// Represents parameters how to display the page in the presentation mode.
    /// </summary>
    public class PdfPageTransition :
        IPdfWrapper,
        ICloneable
    {
        #region Fields
        /// <summary>
        /// Internal variable to store dictionary.
        /// </summary>
        private PdfDictionary m_dictionary = new PdfDictionary();
        /// <summary>
        /// Internal variable to store transition style.
        /// </summary>
        private PdfTransitionStyle m_style = PdfTransitionStyle.Replace;
        /// <summary>
        /// Internal value to store transtion duration.
        /// </summary>
        private float m_duration = 1;
        /// <summary>
        /// Internal variable to store transition dimension.
        /// </summary>
        private PdfTransitionDimension m_dimension = PdfTransitionDimension.Horizontal;
        /// <summary>
        /// Internal variable to store transition motion.
        /// </summary>
        private PdfTransitionMotion m_motion = PdfTransitionMotion.Inward;
        /// <summary>
        /// Internal variable to store transition motion.
        /// </summary>
        private PdfTransitionDirection m_direction = PdfTransitionDirection.LeftToRight;
        /// <summary>
        /// Internal variable to store scale.
        /// </summary>
        private float m_scale = 1.0f;
        /// <summary>
        /// Internal variable to store page duration.
        /// </summary>
        private float m_pageDuration = 0.0f;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the transition style to use when moving to this page from another 
        /// during a presentation.
        /// </summary>
        /// <value>The style.</value>
        public PdfTransitionStyle Style
        {
            get
            {
                return m_style;
            }
            set
            {
                m_style = value;
                m_dictionary.SetProperty(DictionaryProperties.Style,
                    new PdfName(StyleToString(m_style)));
            }
        }

        /// <summary>
        /// Gets or sets the duration of the transition effect, in seconds.
        /// </summary>
        /// <value>The transition duration.</value>
        public float Duration
        {
            get
            {
                return m_duration;
            }
            set
            {
                m_duration = value;
                m_dictionary.SetProperty(DictionaryProperties.Duration, new PdfNumber(m_duration));
            }
        }

        /// <summary>
        /// Gets or sets the dimension in which the specified transition effect occurs.
        /// </summary>
        /// <value>The dimension.</value>
        public PdfTransitionDimension Dimension
        {
            get
            {
                return m_dimension;
            }
            set
            {
                m_dimension = value;
                m_dictionary.SetProperty(DictionaryProperties.Dimension,
                    new PdfName(DimensionToString(m_dimension)));
            }
        }

        /// <summary>
        /// Gets or sets the the direction of motion for the specified transition effect.
        /// </summary>
        /// <value>The motion.</value>
        public PdfTransitionMotion Motion
        {
            get
            {
                return m_motion;
            }
            set
            {
                m_motion = value;
                m_dictionary.SetProperty(DictionaryProperties.Motion,
                    new PdfName(MotionToString(m_motion)));
            }
        }

        /// <summary>
        /// The direction in which the specified transition effect moves, expressed in degrees counter 
        /// clockwise starting from a left-to-right direction. (This differs from the page object�s 
        /// Rotate property, which is measured clockwise from the top.)
        /// </summary>
        public PdfTransitionDirection Direction
        {
            get
            {
                return m_direction;
            }
            set
            {
                m_direction = value;
                m_dictionary.SetProperty(DictionaryProperties.Direction,
                    new PdfNumber((int)m_direction));
            }
        }

        /// <summary>
        /// Gets or sets the starting or ending scale at which the changes are drawn. 
        /// If Motion property specifies an inward transition, the scale of the changes drawn progresses 
        /// from Scale to 1.0 over the course of the transition. If Motion specifies an outward 
        /// transition, the scale of the changes drawn progresses from 1.0 to Scale over the course 
        /// of the transition.
        /// </summary>
        /// <remarks>
        /// This property has effect for Fly transition style only.
        /// </remarks>
        /// <value>The scale.</value>
        public float Scale
        {
            get
            {
                return m_scale;
            }
            set
            {
                m_scale = value;
                m_dictionary.SetProperty(DictionaryProperties.Scale, new PdfNumber(m_scale));
            }
        }

        /// <summary>
        /// Gets or sets The page�s display duration (also called its advance timing): the maximum 
        /// length of time, in seconds, that the page is displayed during presentations before 
        /// the viewer application automatically advances to the next page. By default, 
        /// the viewer does not advance automatically.
        /// </summary>
        /// <value>The page duration.</value>
        public float PageDuration
        {
            get
            {
                return m_pageDuration;
            }
            set
            {
                m_pageDuration = value;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfPageTransition"/> class.
        /// </summary>
        public PdfPageTransition()
        {
            m_dictionary.SetProperty(DictionaryProperties.Type,
                new PdfName(DictionaryProperties.Transition));
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Converts transition motion to string.
        /// </summary>
        /// <param name="motion">The motion.</param>
        /// <returns></returns>
        private string MotionToString(PdfTransitionMotion motion)
        {
            switch (motion)
            {
                case PdfTransitionMotion.Inward:
                default:
                    return "I";

                case PdfTransitionMotion.Outward:
                    return "O";
            }
        }

        /// <summary>
        /// Converts transition dimension to string.
        /// </summary>
        /// <param name="dimension">The dimension.</param>
        /// <returns></returns>
        private string DimensionToString(PdfTransitionDimension dimension)
        {
            switch (dimension)
            {
                case PdfTransitionDimension.Horizontal:
                default:
                    return "H";

                case PdfTransitionDimension.Vertical:
                    return "V";
            }
        }

        /// <summary>
        /// Converts style to string.
        /// </summary>
        /// <param name="style">The style.</param>
        /// <returns></returns>
        private string StyleToString(PdfTransitionStyle style)
        {
            if (style == PdfTransitionStyle.Replace)
            {
                return "R";
            }
            else
            {
                return style.ToString();
            }
        }
        #endregion

        #region IPdfWrapper Members
        /// <summary>
        /// Gets the element.
        /// </summary>
        /// <value></value>
        Syncfusion.Pdf.Primitives.IPdfPrimitive IPdfWrapper.Element
        {
            get
            {
                return m_dictionary;
            }
        }
        #endregion

        #region ICloneable Members
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            return MemberwiseClone();
        }
        #endregion
    }
}
