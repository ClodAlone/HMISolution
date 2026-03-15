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

	#region FontListComboBoxIsDropDownOpenChangedCommand
	// FontListComboBoxIsDropDownOpenChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class FontListComboBoxIsDropDownOpenChangedCommand : ControlCommandBase<FontListComboBoxIsDropDownOpenChangedCommandBehavior, FontListComboBox>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class FontListComboBoxIsDropDownOpenChangedCommandBehavior : CommandBehaviorBase<FontListComboBox>
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
            TargetObject.IsDropDownOpenChanged += OnEventRaised;
        }
    }

	// FontListComboBoxIsDropDownOpenChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FontListComboBoxIsDropDownOpenChangedCommandBehavior<T> : FontListComboBoxIsDropDownOpenChangedCommandBehavior
    { }
	#endregion

	#region FontListComboBoxFontsSourceChangedCommand
	// FontListComboBoxFontsSourceChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class FontListComboBoxFontsSourceChangedCommand : ControlCommandBase<FontListComboBoxFontsSourceChangedCommandBehavior, FontListComboBox>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class FontListComboBoxFontsSourceChangedCommandBehavior : CommandBehaviorBase<FontListComboBox>
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

	// FontListComboBoxFontsSourceChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FontListComboBoxFontsSourceChangedCommandBehavior<T> : FontListComboBoxFontsSourceChangedCommandBehavior
    { }
	#endregion

	#region FontListComboBoxFocusedFontFamilyChangedCommand
	// FontListComboBoxFocusedFontFamilyChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class FontListComboBoxFocusedFontFamilyChangedCommand : ControlCommandBase<FontListComboBoxFocusedFontFamilyChangedCommandBehavior, FontListComboBox>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class FontListComboBoxFocusedFontFamilyChangedCommandBehavior : CommandBehaviorBase<FontListComboBox>
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

	// FontListComboBoxFocusedFontFamilyChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FontListComboBoxFocusedFontFamilyChangedCommandBehavior<T> : FontListComboBoxFocusedFontFamilyChangedCommandBehavior
    { }
	#endregion

	#region FontListComboBoxRecentlyUsedFontsChangedCommand
	// FontListComboBoxRecentlyUsedFontsChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class FontListComboBoxRecentlyUsedFontsChangedCommand : ControlCommandBase<FontListComboBoxRecentlyUsedFontsChangedCommandBehavior, FontListComboBox>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class FontListComboBoxRecentlyUsedFontsChangedCommandBehavior : CommandBehaviorBase<FontListComboBox>
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

	// FontListComboBoxRecentlyUsedFontsChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FontListComboBoxRecentlyUsedFontsChangedCommandBehavior<T> : FontListComboBoxRecentlyUsedFontsChangedCommandBehavior
    { }
	#endregion

	#region FontListComboBoxSelectedFontFamilyChangedCommand
	// FontListComboBoxSelectedFontFamilyChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class FontListComboBoxSelectedFontFamilyChangedCommand : ControlCommandBase<FontListComboBoxSelectedFontFamilyChangedCommandBehavior, FontListComboBox>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class FontListComboBoxSelectedFontFamilyChangedCommandBehavior : CommandBehaviorBase<FontListComboBox>
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

	// FontListComboBoxSelectedFontFamilyChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FontListComboBoxSelectedFontFamilyChangedCommandBehavior<T> : FontListComboBoxSelectedFontFamilyChangedCommandBehavior
    { }
	#endregion

	#region FontListComboBoxThemeFontsChangedCommand
	// FontListComboBoxThemeFontsChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class FontListComboBoxThemeFontsChangedCommand : ControlCommandBase<FontListComboBoxThemeFontsChangedCommandBehavior, FontListComboBox>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class FontListComboBoxThemeFontsChangedCommandBehavior : CommandBehaviorBase<FontListComboBox>
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

	// FontListComboBoxThemeFontsChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FontListComboBoxThemeFontsChangedCommandBehavior<T> : FontListComboBoxThemeFontsChangedCommandBehavior
    { }
	#endregion

	#region FontListComboBoxDisplayFontNamesInSystemFontChangedCommand
	// FontListComboBoxDisplayFontNamesInSystemFontChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class FontListComboBoxDisplayFontNamesInSystemFontChangedCommand : ControlCommandBase<FontListComboBoxDisplayFontNamesInSystemFontChangedCommandBehavior, FontListComboBox>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class FontListComboBoxDisplayFontNamesInSystemFontChangedCommandBehavior : CommandBehaviorBase<FontListComboBox>
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

	// FontListComboBoxDisplayFontNamesInSystemFontChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FontListComboBoxDisplayFontNamesInSystemFontChangedCommandBehavior<T> : FontListComboBoxDisplayFontNamesInSystemFontChangedCommandBehavior
    { }
	#endregion

	#region FontListComboBoxGroupHeaderStyleChangedCommand
	// FontListComboBoxGroupHeaderStyleChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class FontListComboBoxGroupHeaderStyleChangedCommand : ControlCommandBase<FontListComboBoxGroupHeaderStyleChangedCommandBehavior, FontListComboBox>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class FontListComboBoxGroupHeaderStyleChangedCommandBehavior : CommandBehaviorBase<FontListComboBox>
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

	// FontListComboBoxGroupHeaderStyleChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FontListComboBoxGroupHeaderStyleChangedCommandBehavior<T> : FontListComboBoxGroupHeaderStyleChangedCommandBehavior
    { }
	#endregion

	#region FontListComboBoxItemTemplateChangedCommand
	// FontListComboBoxItemTemplateChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class FontListComboBoxItemTemplateChangedCommand : ControlCommandBase<FontListComboBoxItemTemplateChangedCommandBehavior, FontListComboBox>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class FontListComboBoxItemTemplateChangedCommandBehavior : CommandBehaviorBase<FontListComboBox>
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

	// FontListComboBoxItemTemplateChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FontListComboBoxItemTemplateChangedCommandBehavior<T> : FontListComboBoxItemTemplateChangedCommandBehavior
    { }
	#endregion

	#region FontListComboBoxItemContainerStyleChangedCommand
	// FontListComboBoxItemContainerStyleChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class FontListComboBoxItemContainerStyleChangedCommand : ControlCommandBase<FontListComboBoxItemContainerStyleChangedCommandBehavior, FontListComboBox>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class FontListComboBoxItemContainerStyleChangedCommandBehavior : CommandBehaviorBase<FontListComboBox>
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

	// FontListComboBoxItemContainerStyleChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FontListComboBoxItemContainerStyleChangedCommandBehavior<T> : FontListComboBoxItemContainerStyleChangedCommandBehavior
    { }
	#endregion

	#region FontListComboBoxPopupDropDownHeightChangedCommand
	// FontListComboBoxPopupDropDownHeightChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class FontListComboBoxPopupDropDownHeightChangedCommand : ControlCommandBase<FontListComboBoxPopupDropDownHeightChangedCommandBehavior, FontListComboBox>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class FontListComboBoxPopupDropDownHeightChangedCommandBehavior : CommandBehaviorBase<FontListComboBox>
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
            TargetObject.PopupDropDownHeightChanged += OnEventRaised;
        }
    }

	// FontListComboBoxPopupDropDownHeightChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FontListComboBoxPopupDropDownHeightChangedCommandBehavior<T> : FontListComboBoxPopupDropDownHeightChangedCommandBehavior
    { }
	#endregion
}


