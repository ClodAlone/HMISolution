// <copyright file="_enum.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents the TabScroll button Visibility
    /// </summary>
    public enum TabScrollButtonVisibility
    {
        /// <summary>
        /// It sets the TabScrollButtonVisibility to Auto 
        /// </summary>
        Auto,

        /// <summary>
        /// It sets the TabScrollButtonVisibility to Hidden 
        /// </summary>
        Hidden,

        /// <summary>
        /// It sets the TabScrollButtonVisibility to Visible 
        /// </summary>
        Visible
    }

    /// <summary>
    /// Represents the TabScroll Style for TabControlExt
    /// </summary>
    public enum TabScrollStyle
    {
        /// <summary>
        /// Set the TabScrollStyle to Normal Mode
        /// </summary>
        Normal,

        /// <summary>
        ///  Set the TabScrollStyle to Extended Mode
        /// </summary>
        Extended
    }

    /// <summary>
    /// Represents the TabItemLayoutType for TabControlExt
    /// </summary>
    public enum TabItemLayoutType
    {
        /// <summary>
        /// Set SingleLine for the Tab item layout
        /// </summary>
        SingleLine,

        /// <summary>
        /// Set MultiLine for the Tab item layout
        /// </summary>
        MultiLine,

        /// <summary>
        /// Set MultiLineWithFullWidth for the Tab item layout
        /// </summary>
        MultiLineWithFullWidth
    }

    /// <summary>
    /// Represents the TabItemSizeMode for TabControlExt
    /// </summary>
    public enum TabItemSizeMode
    {
        /// <summary>
        /// Set Normal for the Tab Item Size Mode
        /// </summary>
        Normal,

        /// <summary>
        /// Set ShrinkToFit for the Tab Item Size Mode
        /// </summary>
        ShrinkToFit
    }

    /// <summary>
    /// Represents the Image Alignment for the TabItemExt
    /// </summary>
    public enum ImageAlignment
    {
        /// <summary>
        /// Sets the Image Left of the Text
        /// </summary>
        LeftOfText,

        /// <summary>
        /// Sets the Image Above the Text
        /// </summary>
        AboveText,

        /// <summary>
        /// Sets the Image Right of the Text
        /// </summary>
        RightOfText,

        /// <summary>
        /// Sets the below the Text
        /// </summary>
        BelowText
    }

    /// <summary>
    /// Represents the close button type for TabControlExt
    /// </summary>
    public enum CloseButtonType
    {
        /// <summary>
        /// Set common as the close button type
        /// </summary>
        Common,

        /// <summary>
        /// Set as Individual as the close button type
        /// </summary>
        Individual,

        /// <summary>
        /// Set Both as the close button type
        /// </summary>
        Both,

        /// <summary>
        /// Set IndividualOnMouseOver as the close button type
        /// </summary>
        IndividualOnMouseOver,

        /// <summary>
        /// Set Hide as the close button type
        /// </summary>
        Hide,

        /// <summary>
        /// Set Extended as the close button type
        /// </summary>
        Extended
    }

    /// <summary>
    /// Represents the Adorner Alignment for the TabControlExt
    /// </summary>
    public enum AdornerAlignment
    {
        /// <summary>
        /// Set FirstSide
        /// </summary>
        FirstSide,

        /// <summary>
        /// Set SecondSide
        /// </summary>
        SecondSide
    }

    /// <summary>
    /// Represents the save mode for state persistence
    /// </summary>
    public enum SaveMode
    {
        /// <summary>
        /// Set as IsolatedStorage
        /// </summary>
        IsolatedSorage,

        /// <summary>
        /// Set as Registry
        /// </summary>
        Registry
    }

    /// <summary>
    /// Represents the TabStates for TabControlExt
    /// </summary>
    public enum TabStates
    {
        /// <summary>
        /// Set None as TabStates
        /// </summary>
        None,

        /// <summary>
        /// Set Hover as TabStates
        /// </summary>
        Hover,

        /// <summary>
        /// Set Selected as TabStates
        /// </summary>
        Selected,

        /// <summary>
        /// Set HoverSelected as TabStates
        /// </summary>
        HoverSelected
    }

    /// <summary>
    /// Represents enum for ScrollDirection
    /// </summary>
    internal enum ScrollDirection
    {
        /// <summary>
        /// Set ScrollDirection to NextTab
        /// </summary>
        NextTab,

        /// <summary>
        /// Set ScrollDirection to PrevTab
        /// </summary>
        PrevTab,

        /// <summary>
        /// Set ScrollDirection to NextPage
        /// </summary>
        NextPage,

        /// <summary>
        /// Set ScrollDirection to PrevPage
        /// </summary>
        PrevPage,

        /// <summary>
        /// Set ScrollDirection to FirstTab
        /// </summary>
        FirstTab,

        /// <summary>
        /// Set ScrollDirection to LastTab
        /// </summary>
        LastTab
    }

    public enum CloseMode
    {
        /// <summary>
        /// Hide the TabItemExt
        /// </summary>
        Hide,

        /// <summary>
        /// Delete the TabItemExt
        /// </summary>
        Delete
    }
}