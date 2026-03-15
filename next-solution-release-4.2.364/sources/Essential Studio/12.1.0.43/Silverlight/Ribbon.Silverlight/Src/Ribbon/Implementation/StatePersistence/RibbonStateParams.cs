#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;
using System.Linq;
using Syncfusion.Windows.Shared;
using System.Collections.ObjectModel;
using Syncfusion.Windows.Tools.Controls;
using System.IO.IsolatedStorage;


namespace Syncfusion.Windows.Tools
{

    /// <summary>
    /// Used to persisting state of Ribbon elements.
    /// </summary>
    public class RibbonStateParams
    {

        /// <summary>
        /// Store the QAT Bar status whether it is placed above or below of ribbon.
        /// </summary>
        private bool isQATBelow;

        /// <summary>
        /// Ribon State.
        /// </summary>
        private RibbonState ribbonState;

        /// <summary>
        /// Gets or sets the index of the checked items.
        /// </summary>
        /// <value>The index of the checked items.</value>
        public string  CheckedItemsIndex { get; set; }

        /// <summary>
        /// Gets or sets the removed QAT static items indices.
        /// </summary>
        /// <value>The removed QAT static items indices.</value>
        public string RemovedQATStaticItemsIndices { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is QAT below.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is QAT below; otherwise, <c>false</c>.
        /// </value>
        public bool IsQATBelow
        {
            get { return this.isQATBelow; }
            set { this.isQATBelow = value; }
        }

        /// <summary>
        /// Gets or sets the state of the ribbon.
        /// </summary>
        /// <value>The state of the ribbon.</value>
        public RibbonState RibbonState
        {
            get { return this.ribbonState; }
            set { this.ribbonState = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public List<object> WindowCoordinates
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the QAT items string.
        /// </summary>
        /// <value>The QAT items string.</value>
        public string QATItemsString { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RibbonStateParams"/> class.
        /// </summary>
        public RibbonStateParams()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RibbonStateParams"/> class.
        /// </summary>
        /// <param name="qatItemsString">The qat items string.</param>
        /// <param name="QATBelow">if set to <c>true</c> [QAT below].</param>
        /// <param name="ribbonState">State of the ribbon.</param>
        /// <param name="ribbon">The ribbon.</param>
        /// <param name="removedStaticItems"></param>
        /// <param name="windowStates"></param>
        public RibbonStateParams(string qatItemsString,bool QATBelow, RibbonState ribbonState,Ribbon ribbon,string removedStaticItems, List<object> windowStates)
        {          
            this.IsQATBelow = QATBelow;
            this.RibbonState = ribbonState;
            this.QATItemsString = qatItemsString;
            this.RemovedQATStaticItemsIndices = removedStaticItems;

            HandleMenuItems(ribbon);

            this.WindowCoordinates = new List<object>();

            foreach (var item in windowStates)
                this.WindowCoordinates.Add(item);
        }

        /// <summary>
        /// Handles the menu items.
        /// </summary>
        /// <param name="ribbon">The ribbon.</param>
        private void HandleMenuItems(Ribbon ribbon)
        {
            StringBuilder temp = new StringBuilder();
            foreach (var item in ribbon.SynchronizedCommands)
                if (item.IsSynchronizedwithQAT)
                    temp.Append(ribbon.SynchronizedCommands.IndexOf(item).ToString() + ',');
            
            this.CheckedItemsIndex = temp.ToString();
        }
    }
}
