#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.ComponentModel;
using System.Windows;

using System.Windows.Data;
using System.Windows.Input;
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.Windows.Shared;


namespace Syncfusion.Windows.Tools.MVVM
{   

	#region FontListBoxListBoxBackgroundChangedCommand
	// FontListBoxListBoxBackgroundChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class FontListBoxListBoxBackgroundChangedCommand : ControlCommandBase<FontListBoxListBoxBackgroundChangedCommandBehavior, FontListBox>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class FontListBoxListBoxBackgroundChangedCommandBehavior : CommandBehaviorBase<FontListBox>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, DependencyPropertyChangedEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.ListBoxBackgroundChanged += OnEventRaised;
        }
    }

	// FontListBoxListBoxBackgroundChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FontListBoxListBoxBackgroundChangedCommandBehavior<T> : FontListBoxListBoxBackgroundChangedCommandBehavior
    { }
	#endregion

	#region FontListBoxFontsSourceChangedCommand
	// FontListBoxFontsSourceChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class FontListBoxFontsSourceChangedCommand : ControlCommandBase<FontListBoxFontsSourceChangedCommandBehavior, FontListBox>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class FontListBoxFontsSourceChangedCommandBehavior : CommandBehaviorBase<FontListBox>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, DependencyPropertyChangedEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.FontsSourceChanged += OnEventRaised;
        }
    }

	// FontListBoxFontsSourceChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FontListBoxFontsSourceChangedCommandBehavior<T> : FontListBoxFontsSourceChangedCommandBehavior
    { }
	#endregion

	#region FontListBoxFocusedFontFamilyChangedCommand
	// FontListBoxFocusedFontFamilyChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class FontListBoxFocusedFontFamilyChangedCommand : ControlCommandBase<FontListBoxFocusedFontFamilyChangedCommandBehavior, FontListBox>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class FontListBoxFocusedFontFamilyChangedCommandBehavior : CommandBehaviorBase<FontListBox>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, DependencyPropertyChangedEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.FocusedFontFamilyChanged += OnEventRaised;
        }
    }

	// FontListBoxFocusedFontFamilyChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FontListBoxFocusedFontFamilyChangedCommandBehavior<T> : FontListBoxFocusedFontFamilyChangedCommandBehavior
    { }
	#endregion

	#region FontListBoxRecentlyUsedFontsChangedCommand
	// FontListBoxRecentlyUsedFontsChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class FontListBoxRecentlyUsedFontsChangedCommand : ControlCommandBase<FontListBoxRecentlyUsedFontsChangedCommandBehavior, FontListBox>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class FontListBoxRecentlyUsedFontsChangedCommandBehavior : CommandBehaviorBase<FontListBox>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, DependencyPropertyChangedEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.RecentlyUsedFontsChanged += OnEventRaised;
        }
    }

	// FontListBoxRecentlyUsedFontsChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FontListBoxRecentlyUsedFontsChangedCommandBehavior<T> : FontListBoxRecentlyUsedFontsChangedCommandBehavior
    { }
	#endregion

	#region FontListBoxSelectedFontFamilyChangedCommand
	// FontListBoxSelectedFontFamilyChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class FontListBoxSelectedFontFamilyChangedCommand : ControlCommandBase<FontListBoxSelectedFontFamilyChangedCommandBehavior, FontListBox>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class FontListBoxSelectedFontFamilyChangedCommandBehavior : CommandBehaviorBase<FontListBox>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, DependencyPropertyChangedEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.SelectedFontFamilyChanged += OnEventRaised;
        }
    }

	// FontListBoxSelectedFontFamilyChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FontListBoxSelectedFontFamilyChangedCommandBehavior<T> : FontListBoxSelectedFontFamilyChangedCommandBehavior
    { }
	#endregion

	#region FontListBoxThemeFontsChangedCommand
	// FontListBoxThemeFontsChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class FontListBoxThemeFontsChangedCommand : ControlCommandBase<FontListBoxThemeFontsChangedCommandBehavior, FontListBox>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class FontListBoxThemeFontsChangedCommandBehavior : CommandBehaviorBase<FontListBox>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, DependencyPropertyChangedEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.ThemeFontsChanged += OnEventRaised;
        }
    }

	// FontListBoxThemeFontsChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FontListBoxThemeFontsChangedCommandBehavior<T> : FontListBoxThemeFontsChangedCommandBehavior
    { }
	#endregion

	#region FontListBoxHasFocusChangedCommand
	// FontListBoxHasFocusChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class FontListBoxHasFocusChangedCommand : ControlCommandBase<FontListBoxHasFocusChangedCommandBehavior, FontListBox>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class FontListBoxHasFocusChangedCommandBehavior : CommandBehaviorBase<FontListBox>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, DependencyPropertyChangedEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.HasFocusChanged += OnEventRaised;
        }
    }

	// FontListBoxHasFocusChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FontListBoxHasFocusChangedCommandBehavior<T> : FontListBoxHasFocusChangedCommandBehavior
    { }
	#endregion

	#region FontListBoxDisplayFontNamesInSystemFontChangedCommand
	// FontListBoxDisplayFontNamesInSystemFontChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class FontListBoxDisplayFontNamesInSystemFontChangedCommand : ControlCommandBase<FontListBoxDisplayFontNamesInSystemFontChangedCommandBehavior, FontListBox>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class FontListBoxDisplayFontNamesInSystemFontChangedCommandBehavior : CommandBehaviorBase<FontListBox>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, DependencyPropertyChangedEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.DisplayFontNamesInSystemFontChanged += OnEventRaised;
        }
    }

	// FontListBoxDisplayFontNamesInSystemFontChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FontListBoxDisplayFontNamesInSystemFontChangedCommandBehavior<T> : FontListBoxDisplayFontNamesInSystemFontChangedCommandBehavior
    { }
	#endregion

	#region FontListBoxGroupHeaderStyleChangedCommand
	// FontListBoxGroupHeaderStyleChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class FontListBoxGroupHeaderStyleChangedCommand : ControlCommandBase<FontListBoxGroupHeaderStyleChangedCommandBehavior, FontListBox>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class FontListBoxGroupHeaderStyleChangedCommandBehavior : CommandBehaviorBase<FontListBox>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, DependencyPropertyChangedEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.GroupHeaderStyleChanged += OnEventRaised;
        }
    }

	// FontListBoxGroupHeaderStyleChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FontListBoxGroupHeaderStyleChangedCommandBehavior<T> : FontListBoxGroupHeaderStyleChangedCommandBehavior
    { }
	#endregion

	#region FontListBoxItemTemplateChangedCommand
	// FontListBoxItemTemplateChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class FontListBoxItemTemplateChangedCommand : ControlCommandBase<FontListBoxItemTemplateChangedCommandBehavior, FontListBox>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class FontListBoxItemTemplateChangedCommandBehavior : CommandBehaviorBase<FontListBox>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, DependencyPropertyChangedEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.ItemTemplateChanged += OnEventRaised;
        }
    }

	// FontListBoxItemTemplateChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FontListBoxItemTemplateChangedCommandBehavior<T> : FontListBoxItemTemplateChangedCommandBehavior
    { }
	#endregion

	#region FontListBoxItemContainerStyleChangedCommand
	// FontListBoxItemContainerStyleChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class FontListBoxItemContainerStyleChangedCommand : ControlCommandBase<FontListBoxItemContainerStyleChangedCommandBehavior, FontListBox>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class FontListBoxItemContainerStyleChangedCommandBehavior : CommandBehaviorBase<FontListBox>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, DependencyPropertyChangedEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.ItemContainerStyleChanged += OnEventRaised;
        }
    }

	// FontListBoxItemContainerStyleChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FontListBoxItemContainerStyleChangedCommandBehavior<T> : FontListBoxItemContainerStyleChangedCommandBehavior
    { }
	#endregion
}


