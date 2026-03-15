// <copyright file="MainDocumentParams.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Runtime.Serialization;
using System.Windows;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Presents serialization's parameters for DocumentContainer.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    [Serializable]
    public class MainDocumentParams : DocumentParamsBase
    {
        #region Public properties
        /// <summary>
        /// Gets or sets a value indicating whether this instance is keep circle.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is keep circle; otherwise, <c>false</c>.
        /// </value>
        public bool IsKeepCircle
        {
            get;
            set;
        }
        
        /// <summary>
        /// Gets or sets the switch mode.
        /// </summary>
        /// <value>The switch mode.</value>
        public SwitchMode SwitchMode
        {
            get;
            set;
        }
        
        /// <summary>
        /// Gets or sets the mode.
        /// </summary>
        /// <value>The mode DocumentContainerMode.</value>
        public DocumentContainerMode Mode
        {
            get;
            set;
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether this instance can MDI maximize.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance can MDI maximize; otherwise, <c>false</c>.
        /// </value>
        public bool CanMDIMaximize
        {
            get;
            set;
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether this instance can MDI minimize.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance can MDI minimize; otherwise, <c>false</c>.
        /// </value>
        public bool CanMDIMinimize
        {
            get;
            set;
        }
        
        /// <summary>
        /// Gets or sets the delay preview time.
        /// </summary>
        /// <value>The delay preview time.</value>
        public TimeSpan DelayPreviewTime
        {
            get;
            set;
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether this instance is enabled scroll.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is enabled scroll; otherwise, <c>false</c>.
        /// </value>
        public bool IsEnabledScroll
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is allow MDI resize.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is allow MDI resize; otherwise, <c>false</c>.
        /// </value>
        public bool IsAllowMDIResize
        {
            get;
            set;
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes a new instance of the <see cref="MainDocumentParams"/> class.
        /// </summary>
        public MainDocumentParams()
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="MainDocumentParams"/> class.
        /// </summary>
        /// <param name="element">The element MainDocumentParams.</param>
        /// <param name="mode">The mode MainDocumentParams.</param>
        public MainDocumentParams(FrameworkElement element, PropertiesMode mode)
            : base(element, mode)
        {
            DocumentContainer owner = (DocumentContainer)element;

            IsKeepCircle = owner.IsKeepCircle;
            SwitchMode = owner.SwitchMode;
            Mode = owner.Mode;
            CanMDIMaximize = owner.CanMDIMaximize;
            CanMDIMinimize = owner.CanMDIMinimize;
            DelayPreviewTime = owner.DelayPreviewTime;
            IsEnabledScroll = owner.IsEnabledScroll;
            IsAllowMDIResize = owner.IsAllowMDIResize;
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="MainDocumentParams"/> class.
        /// </summary>
        /// <param name="info">The info MainDocumentParams.</param>
        /// <param name="context">The context MainDocumentParams.</param>
        public MainDocumentParams(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        #endregion
    }
}
