#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;

/// <summary>
/// The Syncfusion.Pdf.Interactive namespace contains classes used to create interactive elements.
/// </summary>
namespace Syncfusion.Pdf.Interactive
{
    /// <summary>
    /// Represents the lighting to apply for the 3D artwork.
    /// </summary>
    /// <example>
    /// <code lang = "C#">
    /// //Create a new Pdf3DAnimation
    /// Pdf3DAnimation animation = new Pdf3DAnimation(PDF3DAnimationType.Linear);
    /// animation.Type = PDF3DAnimationType.Linear;
    /// </code>
    /// <code lang="VB">
    /// 'Create a new Pdf3DAnimation
    /// Dim animation As Pdf3DAnimation  = New Pdf3DAnimation(PDF3DAnimationType.Linear)
    /// animation.Type = PDF3DAnimationType.Linear;
    /// </code>
    /// </example> 
    public class Pdf3DAnimation : IPdfWrapper
    {
        #region Fields
        private PDF3DAnimationType m_type;
        private int m_playCount;
        private float m_timeMultiplier;
        private PdfDictionary m_dictionary = new PdfDictionary();
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the type of the animation.
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new Pdf3DAnimation
        /// Pdf3DAnimation animation = new Pdf3DAnimation(PDF3DAnimationType.Linear);
        /// animation.Type = PDF3DAnimationType.Linear;
        /// </code>
        /// <code lang="VB">
        /// 'Create a new Pdf3DAnimation
        /// Dim animation As Pdf3DAnimation  = New Pdf3DAnimation(PDF3DAnimationType.Linear)
        /// animation.Type = PDF3DAnimationType.Linear
        /// </code>
        /// </example> 
        public PDF3DAnimationType Type
        {
            get
            {
                return this.m_type;
            }

            set
            {
                this.m_type = value;
            }
        }

        /// <summary>
        /// Gets or sets the play count. 
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new Pdf3DAnimation
        /// Pdf3DAnimation animation = new Pdf3DAnimation(PDF3DAnimationType.Linear);
        /// animation.PlayCount =10;
        /// </code>
        /// <code lang="VB">
        /// 'Create a new Pdf3DAnimation
        /// Dim animation As Pdf3DAnimation  = New Pdf3DAnimation(PDF3DAnimationType.Linear)
        /// animation.PlayCount =10
        /// </code>
        /// </example> 
        public int PlayCount
        {
            get
            {
                return this.m_playCount;
            }

            set
            {
                this.m_playCount = value;
            }
        }

        /// <summary>
        /// Gets or sets the rendering opacity.
        /// <remarks>A positive number specifying the time multiplier to be used when running the animation. A value greater than one shortens the time it takes to play the animation, or effectively speeds up the animation.</remarks>
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new Pdf3DAnimation
        /// Pdf3DAnimation animation = new Pdf3DAnimation(PDF3DAnimationType.Linear);
        /// animation.Type = 10f;
        /// </code>
        /// <code lang="VB">
        /// 'Create a new Pdf3DAnimation
        /// Dim animation As Pdf3DAnimation  = New Pdf3DAnimation(PDF3DAnimationType.Linear)
        /// animation.Type = 10f
        /// </code>
        /// </example> 
        public float TimeMultiplier
        {
            get
            {
                return this.m_timeMultiplier;
            }

            set
            {
                this.m_timeMultiplier = value;
            }
        }

        /// <summary>
        /// Gets the dictionary.
        /// </summary>
        /// <value>The dictionary.</value>
        internal PdfDictionary Dictionary
        {
            get
            {
                return this.m_dictionary;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Pdf3DAnimation"/> class.
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new Pdf3DAnimation
        /// Pdf3DAnimation animation = new Pdf3DAnimation();
        /// animation.Type = PDF3DAnimationType.Linear;
        /// </code>
        /// <code lang="VB">
        /// 'Create a new Pdf3DAnimation
        /// Dim animation As Pdf3DAnimation  = New Pdf3DAnimation()
        /// animation.Type = PDF3DAnimationType.Linear
        /// </code>
        /// </example> 
        public Pdf3DAnimation()
        {
            this.Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Pdf3DAnimation"/> class.
        /// </summary>
        /// <param name="type">PDF 3D Animation Type.</param>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new Pdf3DAnimation
        /// Pdf3DAnimation animation = new Pdf3DAnimation(PDF3DAnimationType.Linear);
        /// animation.Type = PDF3DAnimationType.Linear;
        /// </code>
        /// <code lang="VB">
        /// 'Create a new Pdf3DAnimation
        /// Dim animation As Pdf3DAnimation  = New Pdf3DAnimation(PDF3DAnimationType.Linear)
        /// animation.Type = PDF3DAnimationType.Linear
        /// </code>
        /// </example> 
        public Pdf3DAnimation(PDF3DAnimationType type)
            : this()
        {
            this.m_type = type;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Initializes annotation object.
        /// </summary>
        protected virtual void Initialize()
        {
            this.m_dictionary.BeginSave += new SavePdfPrimitiveEventHandler(this.Dictionary_BeginSave);
            this.m_dictionary.SetProperty(DictionaryProperties.Type, new PdfName(DictionaryProperties._3DAnimationStyle));
        }

        /// <summary>
        /// Handles the BeginSave event of the Dictionary.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="ars">The <see cref="Syncfusion.Pdf.Primitives.SavePdfPrimitiveEventArgs"/> instance containing the event data.</param>
        private void Dictionary_BeginSave(object sender, SavePdfPrimitiveEventArgs ars)
        {
            this.Save();
        }

        /// <summary>
        /// Saves an annotation.
        /// </summary>
        protected virtual void Save()
        {
            this.Dictionary[DictionaryProperties.Subtype] = new PdfName(this.m_type);
            this.Dictionary.SetProperty(DictionaryProperties.PC, new PdfNumber(this.m_playCount));
            this.Dictionary.SetProperty(DictionaryProperties.TM, new PdfNumber(this.m_timeMultiplier));
        }
        #endregion

        #region IPdfWrapper Members
        /// <summary>
        /// Gets the element.
        /// </summary>
        /// <value></value>
        IPdfPrimitive IPdfWrapper.Element
        {
            get
            {
                return this.m_dictionary;
            }
        }
        #endregion
    }
}
