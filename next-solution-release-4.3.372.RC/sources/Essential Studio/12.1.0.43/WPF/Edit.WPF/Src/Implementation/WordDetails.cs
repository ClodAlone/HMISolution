// <copyright file="WordDetails.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System.Windows;
using System.Windows.Media;

namespace Syncfusion.Windows.Edit
{
    /// <summary>
    /// Word Details class is used to store values related to tokens in the line
    /// </summary>
    /// <remarks>
    /// <para>The word details class contain the word properties like StartIndex,
    /// EndIndex, Foreground, Font etc.</para>
    /// </remarks>
    /// <example>
    /// <para></para>
    /// <para><b>XAML</b></para>
    /// <para></para>
    /// <para>&lt;Window x:Class=&quot;SampleApplication.Window1&quot; </para>
    /// <para> xmlns=&quot;http://
    /// schemas.microsoft.com/winfx/2006/xaml/presentation&quot; xmlns:x=&quot;http://
    /// schemas.microsoft.com/winfx/2006/xaml&quot; Title=&quot;Window1&quot;
    /// Height=&quot;300&quot; Width=&quot;300&quot;
    /// xmlns:syncfusion=&quot;http://schemas.syncfusion.com/wpf&quot;&gt;</para>
    /// <para>    &lt;Grid&gt;</para>
    /// <para>        &lt;syncfusion:WordDetails StartIndex=&quot;0&quot;
    /// EndIndex=&quot;12&quot; FontSize=&quot;12&quot; /&gt;</para>
    /// <para>    &lt;/Grid&gt;</para>
    /// <para>&lt;/Window&gt;</para>
    /// <para></para>
    /// <para><b>C#</b></para>
    /// <para></para>
    /// <para>WordDetails WordDetails = new WordDetails();</para>
    /// <para>WordDetails.StartIndex = 0;</para>
    /// <para>WordDetails.EndIndex = 10;</para>
    /// <para>WordDetails.FontSize = 12;</para>
    /// </example>
#if SyncfusionFramework4_0

    using System.ComponentModel;

    [DesignTimeVisible(false)]
#endif
    public class WordDetails
    {
        #region Local Variables

        /// <summary>
        /// instance for Text property.
        /// </summary>
        private string mtext;

        /// <summary>
        /// instance for StartIndex property. property.
        /// </summary>
        private int mstartindex;

        /// <summary>
        /// instance for EndIndex property.
        /// </summary>
        private int mendindex;

        /// <summary>
        /// instance for StartPosition property.
        /// </summary>
        private Point mstartlocation;

        /// <summary>
        /// Point object for EndPosition property.
        /// </summary>
        private Point mendlocation;

        /// <summary>
        /// Brush object for Foreground property.
        /// </summary>
        private Brush mforeground;

        /// <summary>
        /// FontFamily object for Font property.
        /// </summary>
        private FontFamily mfont = null;

        /// <summary>
        /// instance for FontSize property.
        /// </summary>
        private double mfontsize = double.NaN;

        #endregion Local Variables

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="WordDetails"/> class.
        /// </summary>
        public WordDetails()
        {
        }

        #endregion Initialization

        #region Properties

        /// <summary>
        /// Gets or sets Token's text
        /// </summary>
        /// <value>
        /// Type: System.String
        /// </value>
        public string Text
        {
            get
            {
                return mtext;
            }

            set
            {
                mtext = value;
            }
        }

        /// <summary>
        /// Gets or sets Token's Start Index with respect to LineItem
        /// </summary>
        /// <value>
        /// Type: System.Int32
        /// </value>
        public int StartIndex
        {
            get
            {
                return mstartindex;
            }

            set
            {
                mstartindex = value;
            }
        }

        /// <summary>
        /// Gets or sets Token's End Index with respect to LineItem
        /// </summary>
        /// <value>
        /// Type: System.Int32
        /// </value>
        public int EndIndex
        {
            get
            {
                return mendindex;
            }

            set
            {
                mendindex = value;
            }
        }

        /// <summary>
        /// Gets or sets Token's Start Position with the screen points
        /// </summary>
        public Point StartPosition
        {
            get
            {
                return mstartlocation;
            }

            set
            {
                mstartlocation = value;
            }
        }

        /// <summary>
        /// Gets or sets Token's End Position with respect to screen points
        /// </summary>
        public Point EndPosition
        {
            get
            {
                return mendlocation;
            }

            set
            {
                mendlocation = value;
            }
        }

        /// <summary>
        /// Gets or sets token's foreground brush
        /// </summary>
        public Brush Foreground
        {
            get
            {
                return mforeground;
            }

            set
            {
                mforeground = value;
            }
        }

        /// <summary>
        /// Gets or sets token's FontFamily
        /// </summary>
        public FontFamily Font
        {
            get
            {
                return mfont;
            }

            set
            {
                mfont = value;
            }
        }

        /// <summary>
        /// Gets or sets token's FontSize
        /// </summary>
        /// <value>
        /// Type: System.Double
        /// </value>
        public double FontSize
        {
            get
            {
                return mfontsize;
            }

            set
            {
                mfontsize = value;
            }
        }

        #endregion Properties
    }
}