#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Tools.Controls;

namespace Syncfusion.Windows.Tools.MVVM
{

    #region FileUploadControlCanOverwriteChangedCommand
    // FileUploadControlCanOverwriteChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class FileUploadControlCanOverwriteChangedCommand : ControlCommandBase<FileUploadControlCanOverwriteChangedCommandBehavior, FileUploadControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class FileUploadControlCanOverwriteChangedCommandBehavior : CommandBehaviorBase<FileUploadControl>
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
            TargetObject.CanOverwriteChanged += OnEventRaised;
        }
    }

    // FileUploadControlCanOverwriteChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class FileUploadControlCanOverwriteChangedCommandBehavior<T> : FileUploadControlCanOverwriteChangedCommandBehavior
    { }
    #endregion

    #region FileUploadControlColumnsToHideChangedCommand
    // FileUploadControlColumnsToHideChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class FileUploadControlColumnsToHideChangedCommand : ControlCommandBase<FileUploadControlColumnsToHideChangedCommandBehavior, FileUploadControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class FileUploadControlColumnsToHideChangedCommandBehavior : CommandBehaviorBase<FileUploadControl>
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
            TargetObject.ColumnsToHideChanged += OnEventRaised;
        }
    }

    // FileUploadControlColumnsToHideChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class FileUploadControlColumnsToHideChangedCommandBehavior<T> : FileUploadControlColumnsToHideChangedCommandBehavior
    { }
    #endregion

    #region FileUploadControlEnableDetailsChangedCommand
    // FileUploadControlEnableDetailsChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class FileUploadControlEnableDetailsChangedCommand : ControlCommandBase<FileUploadControlEnableDetailsChangedCommandBehavior, FileUploadControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class FileUploadControlEnableDetailsChangedCommandBehavior : CommandBehaviorBase<FileUploadControl>
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
            TargetObject.EnableDetailsChanged += OnEventRaised;
        }
    }

    // FileUploadControlEnableDetailsChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class FileUploadControlEnableDetailsChangedCommandBehavior<T> : FileUploadControlEnableDetailsChangedCommandBehavior
    { }
    #endregion

    #region FileUploadControlEnableThumbnailChangedCommand
    // FileUploadControlEnableThumbnailChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class FileUploadControlEnableThumbnailChangedCommand : ControlCommandBase<FileUploadControlEnableThumbnailChangedCommandBehavior, FileUploadControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class FileUploadControlEnableThumbnailChangedCommandBehavior : CommandBehaviorBase<FileUploadControl>
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
            TargetObject.EnableThumbnailChanged += OnEventRaised;
        }
    }

    // FileUploadControlEnableThumbnailChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class FileUploadControlEnableThumbnailChangedCommandBehavior<T> : FileUploadControlEnableThumbnailChangedCommandBehavior
    { }
    #endregion

    #region FileUploadControlFileCountExceededEventCommand
    // FileUploadControlFileCountExceededEventCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class FileUploadControlFileCountExceededEventCommand : ControlCommandBase<FileUploadControlFileCountExceededEventCommandBehavior, FileUploadControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class FileUploadControlFileCountExceededEventCommandBehavior : CommandBehaviorBase<FileUploadControl>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, FilesEventArgs e)
        {
            ExecuteCommand();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.FileCountExceededEvent += OnEventRaised;
        }
    }

    // FileUploadControlFileCountExceededEventCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class FileUploadControlFileCountExceededEventCommandBehavior<T> : FileUploadControlFileCountExceededEventCommandBehavior
    { }
    #endregion

    #region FileUploadControlFileExistsEventCommand
    // FileUploadControlFileExistsEventCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class FileUploadControlFileExistsEventCommand : ControlCommandBase<FileUploadControlFileExistsEventCommandBehavior, FileUploadControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class FileUploadControlFileExistsEventCommandBehavior : CommandBehaviorBase<FileUploadControl>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, FileEventArgs e)
        {
            ExecuteCommand();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.FileExistsEvent += OnEventRaised;
        }
    }

    // FileUploadControlFileExistsEventCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class FileUploadControlFileExistsEventCommandBehavior<T> : FileUploadControlFileExistsEventCommandBehavior
    { }
    #endregion

    #region FileUploadControlFilesSelectedEventCommand
    // FileUploadControlFilesSelectedEventCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class FileUploadControlFilesSelectedEventCommand : ControlCommandBase<FileUploadControlFilesSelectedEventCommandBehavior, FileUploadControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class FileUploadControlFilesSelectedEventCommandBehavior : CommandBehaviorBase<FileUploadControl>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, FilesEventArgs e)
        {
            ExecuteCommand();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.FilesSelectedEvent += OnEventRaised;
        }
    }

    // FileUploadControlFilesSelectedEventCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class FileUploadControlFilesSelectedEventCommandBehavior<T> : FileUploadControlFilesSelectedEventCommandBehavior
    { }
    #endregion

    #region FileUploadControlFilesTooLargeEventCommand
    // FileUploadControlFilesTooLargeEventCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class FileUploadControlFilesTooLargeEventCommand : ControlCommandBase<FileUploadControlFilesTooLargeEventCommandBehavior, FileUploadControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class FileUploadControlFilesTooLargeEventCommandBehavior : CommandBehaviorBase<FileUploadControl>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, FilesEventArgs e)
        {
            ExecuteCommand();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.FilesTooLargeEvent += OnEventRaised;
        }
    }

    // FileUploadControlFilesTooLargeEventCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class FileUploadControlFilesTooLargeEventCommandBehavior<T> : FileUploadControlFilesTooLargeEventCommandBehavior
    { }
    #endregion

    #region FileUploadControlFileUploadSizeChangedCommand
    // FileUploadControlFileUploadSizeChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class FileUploadControlFileUploadSizeChangedCommand : ControlCommandBase<FileUploadControlFileUploadSizeChangedCommandBehavior, FileUploadControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class FileUploadControlFileUploadSizeChangedCommandBehavior : CommandBehaviorBase<FileUploadControl>
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
            TargetObject.FileUploadSizeChanged += OnEventRaised;
        }
    }

    // FileUploadControlFileUploadSizeChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class FileUploadControlFileUploadSizeChangedCommandBehavior<T> : FileUploadControlFileUploadSizeChangedCommandBehavior
    { }
    #endregion

    #region FileUploadControlFileUploadSizeUnitChangedCommand
    // FileUploadControlFileUploadSizeUnitChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class FileUploadControlFileUploadSizeUnitChangedCommand : ControlCommandBase<FileUploadControlFileUploadSizeUnitChangedCommandBehavior, FileUploadControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class FileUploadControlFileUploadSizeUnitChangedCommandBehavior : CommandBehaviorBase<FileUploadControl>
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
            TargetObject.FileUploadSizeUnitChanged += OnEventRaised;
        }
    }

    // FileUploadControlFileUploadSizeUnitChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class FileUploadControlFileUploadSizeUnitChangedCommandBehavior<T> : FileUploadControlFileUploadSizeUnitChangedCommandBehavior
    { }
    #endregion

    #region FileUploadControlFileUploadStartingEventCommand
    // FileUploadControlFileUploadStartingEventCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class FileUploadControlFileUploadStartingEventCommand : ControlCommandBase<FileUploadControlFileUploadStartingEventCommandBehavior, FileUploadControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class FileUploadControlFileUploadStartingEventCommandBehavior : CommandBehaviorBase<FileUploadControl>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, FileEventArgs e)
        {
            ExecuteCommand();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.FileUploadStartingEvent += OnEventRaised;
        }
    }

    // FileUploadControlFileUploadStartingEventCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class FileUploadControlFileUploadStartingEventCommandBehavior<T> : FileUploadControlFileUploadStartingEventCommandBehavior
    { }
    #endregion

    #region FileUploadControlFilterChangedCommand
    // FileUploadControlFilterChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class FileUploadControlFilterChangedCommand : ControlCommandBase<FileUploadControlFilterChangedCommandBehavior, FileUploadControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class FileUploadControlFilterChangedCommandBehavior : CommandBehaviorBase<FileUploadControl>
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
            TargetObject.FilterChanged += OnEventRaised;
        }
    }

    // FileUploadControlFilterChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class FileUploadControlFilterChangedCommandBehavior<T> : FileUploadControlFilterChangedCommandBehavior
    { }
    #endregion

    #region FileUploadControlFilterIndexChangedCommand
    // FileUploadControlFilterIndexChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class FileUploadControlFilterIndexChangedCommand : ControlCommandBase<FileUploadControlFilterIndexChangedCommandBehavior, FileUploadControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class FileUploadControlFilterIndexChangedCommandBehavior : CommandBehaviorBase<FileUploadControl>
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
            TargetObject.FilterIndexChanged += OnEventRaised;
        }
    }

    // FileUploadControlFilterIndexChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class FileUploadControlFilterIndexChangedCommandBehavior<T> : FileUploadControlFilterIndexChangedCommandBehavior
    { }
    #endregion

    #region FileUploadControlIsAutomaticUploadChangedCommand
    // FileUploadControlIsAutomaticUploadChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class FileUploadControlIsAutomaticUploadChangedCommand : ControlCommandBase<FileUploadControlIsAutomaticUploadChangedCommandBehavior, FileUploadControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class FileUploadControlIsAutomaticUploadChangedCommandBehavior : CommandBehaviorBase<FileUploadControl>
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
            TargetObject.IsAutomaticUploadChanged += OnEventRaised;
        }
    }

    // FileUploadControlIsAutomaticUploadChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class FileUploadControlIsAutomaticUploadChangedCommandBehavior<T> : FileUploadControlIsAutomaticUploadChangedCommandBehavior
    { }
    #endregion

    #region FileUploadControlIsMultiSelectChangedCommand
    // FileUploadControlIsMultiSelectChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class FileUploadControlIsMultiSelectChangedCommand : ControlCommandBase<FileUploadControlIsMultiSelectChangedCommandBehavior, FileUploadControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class FileUploadControlIsMultiSelectChangedCommandBehavior : CommandBehaviorBase<FileUploadControl>
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
            TargetObject.IsMultiSelectChanged += OnEventRaised;
        }
    }

    // FileUploadControlIsMultiSelectChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class FileUploadControlIsMultiSelectChangedCommandBehavior<T> : FileUploadControlIsMultiSelectChangedCommandBehavior
    { }
    #endregion

    #region FileUploadControlNoOfFilesAllowedChangedCommand
    // FileUploadControlNoOfFilesAllowedChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class FileUploadControlNoOfFilesAllowedChangedCommand : ControlCommandBase<FileUploadControlNoOfFilesAllowedChangedCommandBehavior, FileUploadControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class FileUploadControlNoOfFilesAllowedChangedCommandBehavior : CommandBehaviorBase<FileUploadControl>
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
            TargetObject.NoOfFilesAllowedChanged += OnEventRaised;
        }
    }

    // FileUploadControlNoOfFilesAllowedChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class FileUploadControlNoOfFilesAllowedChangedCommandBehavior<T> : FileUploadControlNoOfFilesAllowedChangedCommandBehavior
    { }
    #endregion

    #region FileUploadControlThemeChangedCommand
    // FileUploadControlThemeChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class FileUploadControlThemeChangedCommand : ControlCommandBase<FileUploadControlThemeChangedCommandBehavior, FileUploadControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class FileUploadControlThemeChangedCommandBehavior : CommandBehaviorBase<FileUploadControl>
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
            TargetObject.ThemeChanged += OnEventRaised;
        }
    }

    // FileUploadControlThemeChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class FileUploadControlThemeChangedCommandBehavior<T> : FileUploadControlThemeChangedCommandBehavior
    { }
    #endregion

    #region FileUploadControlTotalPercentageUploadedChangedCommand
    // FileUploadControlTotalPercentageUploadedChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class FileUploadControlTotalPercentageUploadedChangedCommand : ControlCommandBase<FileUploadControlTotalPercentageUploadedChangedCommandBehavior, FileUploadControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class FileUploadControlTotalPercentageUploadedChangedCommandBehavior : CommandBehaviorBase<FileUploadControl>
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
            TargetObject.TotalPercentageUploadedChanged += OnEventRaised;
        }
    }

    // FileUploadControlTotalPercentageUploadedChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class FileUploadControlTotalPercentageUploadedChangedCommandBehavior<T> : FileUploadControlTotalPercentageUploadedChangedCommandBehavior
    { }
    #endregion

    #region FileUploadControlTotalSizeUploadedChangedCommand
    // FileUploadControlTotalSizeUploadedChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class FileUploadControlTotalSizeUploadedChangedCommand : ControlCommandBase<FileUploadControlTotalSizeUploadedChangedCommandBehavior, FileUploadControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class FileUploadControlTotalSizeUploadedChangedCommandBehavior : CommandBehaviorBase<FileUploadControl>
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
            TargetObject.TotalSizeUploadedChanged += OnEventRaised;
        }
    }

    // FileUploadControlTotalSizeUploadedChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class FileUploadControlTotalSizeUploadedChangedCommandBehavior<T> : FileUploadControlTotalSizeUploadedChangedCommandBehavior
    { }
    #endregion

    #region FileUploadControlTotalUploadSizeChangedCommand
    // FileUploadControlTotalUploadSizeChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class FileUploadControlTotalUploadSizeChangedCommand : ControlCommandBase<FileUploadControlTotalUploadSizeChangedCommandBehavior, FileUploadControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class FileUploadControlTotalUploadSizeChangedCommandBehavior : CommandBehaviorBase<FileUploadControl>
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
            TargetObject.TotalUploadSizeChanged += OnEventRaised;
        }
    }

    // FileUploadControlTotalUploadSizeChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class FileUploadControlTotalUploadSizeChangedCommandBehavior<T> : FileUploadControlTotalUploadSizeChangedCommandBehavior
    { }
    #endregion

    #region FileUploadControlTotalUploadSizeExceededEventCommand
    // FileUploadControlTotalUploadSizeExceededEventCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class FileUploadControlTotalUploadSizeExceededEventCommand : ControlCommandBase<FileUploadControlTotalUploadSizeExceededEventCommandBehavior, FileUploadControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class FileUploadControlTotalUploadSizeExceededEventCommandBehavior : CommandBehaviorBase<FileUploadControl>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, FilesEventArgs e)
        {
            ExecuteCommand();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.TotalUploadSizeExceededEvent += OnEventRaised;
        }
    }

    // FileUploadControlTotalUploadSizeExceededEventCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class FileUploadControlTotalUploadSizeExceededEventCommandBehavior<T> : FileUploadControlTotalUploadSizeExceededEventCommandBehavior
    { }
    #endregion

    #region FileUploadControlTotalUploadSizeUnitChangedCommand
    // FileUploadControlTotalUploadSizeUnitChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class FileUploadControlTotalUploadSizeUnitChangedCommand : ControlCommandBase<FileUploadControlTotalUploadSizeUnitChangedCommandBehavior, FileUploadControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class FileUploadControlTotalUploadSizeUnitChangedCommandBehavior : CommandBehaviorBase<FileUploadControl>
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
            TargetObject.TotalUploadSizeUnitChanged += OnEventRaised;
        }
    }

    // FileUploadControlTotalUploadSizeUnitChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class FileUploadControlTotalUploadSizeUnitChangedCommandBehavior<T> : FileUploadControlTotalUploadSizeUnitChangedCommandBehavior
    { }
    #endregion

    #region FileUploadControlUploadFinishedEventCommand
    // FileUploadControlUploadFinishedEventCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class FileUploadControlUploadFinishedEventCommand : ControlCommandBase<FileUploadControlUploadFinishedEventCommandBehavior, FileUploadControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class FileUploadControlUploadFinishedEventCommandBehavior : CommandBehaviorBase<FileUploadControl>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, FileEventArgs e)
        {
            ExecuteCommand();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.UploadFinishedEvent += OnEventRaised;
        }
    }

    // FileUploadControlUploadFinishedEventCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class FileUploadControlUploadFinishedEventCommandBehavior<T> : FileUploadControlUploadFinishedEventCommandBehavior
    { }
    #endregion

    #region FileUploadControlUploadFolderChangedCommand
    // FileUploadControlUploadFolderChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class FileUploadControlUploadFolderChangedCommand : ControlCommandBase<FileUploadControlUploadFolderChangedCommandBehavior, FileUploadControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class FileUploadControlUploadFolderChangedCommandBehavior : CommandBehaviorBase<FileUploadControl>
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
            TargetObject.UploadFolderChanged += OnEventRaised;
        }
    }

    // FileUploadControlUploadFolderChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class FileUploadControlUploadFolderChangedCommandBehavior<T> : FileUploadControlUploadFolderChangedCommandBehavior
    { }
    #endregion

    #region FileUploadControlUploadPausedEventCommand
    // FileUploadControlUploadPausedEventCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class FileUploadControlUploadPausedEventCommand : ControlCommandBase<FileUploadControlUploadPausedEventCommandBehavior, FileUploadControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class FileUploadControlUploadPausedEventCommandBehavior : CommandBehaviorBase<FileUploadControl>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, FileEventArgs e)
        {
            ExecuteCommand();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.UploadPausedEvent += OnEventRaised;
        }
    }

    // FileUploadControlUploadPausedEventCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class FileUploadControlUploadPausedEventCommandBehavior<T> : FileUploadControlUploadPausedEventCommandBehavior
    { }
    #endregion

    #region FileUploadControlUploadResumedEventCommand
    // FileUploadControlUploadResumedEventCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class FileUploadControlUploadResumedEventCommand : ControlCommandBase<FileUploadControlUploadResumedEventCommandBehavior, FileUploadControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class FileUploadControlUploadResumedEventCommandBehavior : CommandBehaviorBase<FileUploadControl>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, FileEventArgs e)
        {
            ExecuteCommand();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.UploadResumedEvent += OnEventRaised;
        }
    }

    // FileUploadControlUploadResumedEventCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class FileUploadControlUploadResumedEventCommandBehavior<T> : FileUploadControlUploadResumedEventCommandBehavior
    { }
    #endregion
}
