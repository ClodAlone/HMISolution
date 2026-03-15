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
using System.Globalization;
using System.ComponentModel;

namespace Syncfusion.Windows.Tools.Controls.Resources
{
    /// <summary>
    /// 
    /// </summary>
    public class ResourceWrapper :INotifyPropertyChanged
    {
        const string _QATShowBelow = "QATShowBelow";
        const string _QATShowAbove = "QATShowAbove";
        const string _CustomizeQuickAccessToolbar = "CustomizeQuickAccessToolbar";
        const string _AddToQuickAccessToolbar = "AddToQuickAccessToolbar";
        const string _RemoveFromQuickAccessToolbar = "RemoveFromQuickAccessToolbar";
        const string _QATAllCommands = "QATAllCommands";
        const string _DefaultRibbonBarCaption = "DefaultRibbonBarCaption";
        const string _DefaultRibbonButtonCaption = "DefaultRibbonButtonCaption";
        const string _DefaultRibbonTitle = "DefaultRibbonTitle";
        const string _MinimizeRibbon = "MinimizeRibbon";
        const string _MoreCommands = "MoreCommands";
        const string _Add = "Add";
        const string _Cancel = "Cancel";
        const string _ChooseOtherCommandsFrom = "ChooseOtherCommandsFrom";
        const string _DefaultRibbonTabCaption = "DefaultRibbonTabCaption";
        const string _OK = "OK";
        const string _Remove = "Remove";
        const string _Reset = "Reset";
        const string _QATPopupHeader = "QATPopupHeader";
        const string _BackStageHeader = "BackStageButtonHeader";

        /// <summary>
        /// 
        /// </summary>
        public ResourceWrapper()
        {
            CultureInfo ci = CultureInfo.CurrentUICulture;

            QATShowBelow = SR.GetString(ci, _QATShowBelow);
            QATShowAbove = SR.GetString(ci, _QATShowAbove);
            CustomizeQuickAccessToolbar = SR.GetString(ci, _CustomizeQuickAccessToolbar);
            AddToQuickAccessToolbar = SR.GetString(ci, _AddToQuickAccessToolbar);
            RemoveFromQuickAccessToolbar = SR.GetString(ci, _RemoveFromQuickAccessToolbar);
            QATAllCommands = SR.GetString(ci, _QATAllCommands);
            DefaultRibbonBarCaption = SR.GetString(ci, _DefaultRibbonBarCaption);
            DefaultRibbonButtonCaption = SR.GetString(ci, _DefaultRibbonButtonCaption);
            DefaultRibbonTitle = SR.GetString(ci, _DefaultRibbonTitle);
            MinimizeRibbon = SR.GetString(ci, _MinimizeRibbon);
            QATPopupHeader = SR.GetString(ci, _QATPopupHeader);
            MoreCommands = SR.GetString(ci, _MoreCommands);
            Remove = SR.GetString(ci, _Remove);
            OK = SR.GetString(ci, _OK);
            DefaultRibbonTabCaption = SR.GetString(ci, _DefaultRibbonTabCaption);
            ChooseOtherCommandsFrom = SR.GetString(ci, _ChooseOtherCommandsFrom);
            Cancel = SR.GetString(ci, _Cancel);
            Add = SR.GetString(ci, _Add);
            Reset = SR.GetString(ci, _Reset);
            BackStageButtonHeader = SR.GetString(ci, _BackStageHeader);
        }

        string reset;
        /// <summary>
        /// 
        /// </summary>
        public string Reset
        {
            get { return reset; }
            set 
            {
                reset = value;
                OnPropertyChanged("Reset");
            }
        }

        string add;
        /// <summary>
        /// 
        /// </summary>
        public string Add
        {
            get { return add; }
            set 
            {
                add = value;
                OnPropertyChanged("Add");
            }
        }

        string cancel;
        /// <summary>
        /// 
        /// </summary>
        public string Cancel 
        {
            get { return cancel; }
            set 
            {
                cancel = value;
                OnPropertyChanged("Cancel");
            }
        }

        string chooseOtherCommandsFrom;
        /// <summary>
        /// 
        /// </summary>
        public string ChooseOtherCommandsFrom 
        {
            get { return chooseOtherCommandsFrom; }
            set 
            {
                chooseOtherCommandsFrom = value;
                OnPropertyChanged("ChooseOtherCommandsFrom");
            }
        }

        string defaultRibbonTabCaption;
        /// <summary>
        /// 
        /// </summary>
        public string DefaultRibbonTabCaption
        {
            get { return defaultRibbonTabCaption; }
            set 
            {
                defaultRibbonTabCaption = value;
                OnPropertyChanged("DefaultRibbonTabCaption");
            }
        }

        string oK;
        /// <summary>
        /// 
        /// </summary>
        public string OK
        {
            get { return oK; }
            set 
            {
                oK = value;
                OnPropertyChanged("OK");
            }
        }

        string remove;
        /// <summary>
        /// 
        /// </summary>
        public string Remove
        {
            get { return remove; }
            set 
            { 
                remove = value;
                OnPropertyChanged("Remove");
            }
        }

        string qATPopupHeader;
        /// <summary>
        /// 
        /// </summary>
        public string QATPopupHeader
        {
            get { return qATPopupHeader; }
            set 
            {
                qATPopupHeader = value;
                OnPropertyChanged("QATPopupHeader");
            }
        }

        string qATShowBelow;
        /// <summary>
        /// 
        /// </summary>
        public string QATShowBelow
        {
            get { return qATShowBelow; }
            set 
            { 
                qATShowBelow = value;
                OnPropertyChanged("QATShowBelow");
            }
        }

        string qATShowAbove;
        /// <summary>
        /// 
        /// </summary>
        public string QATShowAbove
        {
            get { return qATShowAbove; }
            set 
            { 
                qATShowAbove = value;
                OnPropertyChanged("QATShowAbove");
            }
        }

        string moreCommands;
        /// <summary>
        /// 
        /// </summary>
        public string MoreCommands
        {
            get { return moreCommands; }
            set 
            {
                moreCommands = value;
                OnPropertyChanged("MoreCommands");
            }
        }

        string minimizeRibbon;
        /// <summary>
        /// 
        /// </summary>
        public string MinimizeRibbon
        {
            get { return minimizeRibbon; }
            set 
            { 
                minimizeRibbon = value;
                OnPropertyChanged("MinimizeRibbon");
            }
        }

        string customizeQuickAccessToolbar;
        /// <summary>
        /// 
        /// </summary>
        public string CustomizeQuickAccessToolbar
        {
            get { return customizeQuickAccessToolbar; }
            set 
            { 
                customizeQuickAccessToolbar = value;
                OnPropertyChanged("CustomizeQuickAccessToolBar");
            }
        }

        string addToQuickAccessToolbar;
        /// <summary>
        /// 
        /// </summary>
        public string AddToQuickAccessToolbar
        {
            get { return addToQuickAccessToolbar; }
            set
            {
                addToQuickAccessToolbar = value;
                OnPropertyChanged("AddToQuickAccessToolbar");
            }
        }

        string removeFromQuickAccessToolbar;
        /// <summary>
        /// 
        /// </summary>
        public string RemoveFromQuickAccessToolbar
        {
            get { return removeFromQuickAccessToolbar; }
            set
            {
                removeFromQuickAccessToolbar = value;
                OnPropertyChanged("RemoveFromQuickAccessToolbar");
            }
        }

        string qATAllCommands;
        /// <summary>
        /// 
        /// </summary>
        public string QATAllCommands
        {
            get { return qATAllCommands; }
            set
            {
                qATAllCommands = value;
                OnPropertyChanged("QATAllCommands");
            }
        }

        string defaultRibbonBarCaption;
        /// <summary>
        /// 
        /// </summary>
        public string DefaultRibbonBarCaption
        {
            get { return defaultRibbonBarCaption; }
            set 
            { 
                defaultRibbonBarCaption = value;
                OnPropertyChanged("DefaultRibbonBarCaption");
            }
        }

        string defaultRibbonButtonCaption;
        /// <summary>
        /// 
        /// </summary>
        public string DefaultRibbonButtonCaption
        {
            get { return defaultRibbonButtonCaption; }
            set 
            {
                defaultRibbonButtonCaption = value;
                OnPropertyChanged("DefaultRibbonButtonCaption");
            }
        }

        string defaultRibbonTitle;
        /// <summary>
        /// 
        /// </summary>
        public string DefaultRibbonTitle
        {
            get { return defaultRibbonTitle; }
            set 
            { 
                defaultRibbonTitle = value;
                OnPropertyChanged("DefaultRibbonTitle");
            }
        }

        string backstagebuttonheader;
        /// <summary>
        /// 
        /// </summary>
        public string BackStageButtonHeader
        {
            get
            {
                return backstagebuttonheader;
            }
            set
            {
                backstagebuttonheader = value;
                OnPropertyChanged("BackStageButtonHeader");
            }
        }

        internal void OnPropertyChanged(string propertyname)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
