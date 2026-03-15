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
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Syncfusion.Windows.Tools.Controls
{

    /// <summary>
    /// Represents the Window params class.
    /// </summary>
    public class WindowParams
    {       
       #region Private members
        /// <summary>
        /// Window items.
        /// </summary>
        private List<WindowItems> windowItems;

        /// <summary>
        /// Active window name
        /// </summary>
        private string activeWindowName;

        /// <summary>
        /// Gets or Sets left autohidden panel's SideButton order
        /// </summary>
        private List<string> sideButtonLeftOrder;

        /// <summary>
        /// Gets or Sets right autohidden panel's SideButton order
        /// </summary>
        private List<string> sideButtonRightOrder;

        /// <summary>
        /// Gets or Sets top autohidden panel's SideButton order
        /// </summary>
        private List<string> sideButtonTopOrder;

        /// <summary>
        /// Gets or Sets bottom autohidden panel's SideButton order
        /// </summary>
        private List<string> sideButtonBottomOrder;
        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets Window items.
        /// </summary>
        /// <value>
        /// Type: <see cref="WindowItems"/>
        /// </value>
        /// <seealso cref="WindowItems"/>
        public List<WindowItems> WindowItems
        {
            get
            {
                return this.windowItems;
            }

            set
            {
                this.windowItems = value;
            }
        }

        /// <summary>
        /// Gets or Sets left autohidden panel's SideButton order
        /// </summary>
        public List<string> SideButtonLeftOrder
        {
            get
            {
                return this.sideButtonLeftOrder;
            }
            
            set
            {
                this.sideButtonLeftOrder = value;
            }
        }

        /// <summary>
        /// Gets or Sets right autohidden panel's SideButton order
        /// </summary>
        public List<string> SideButtomRightOrder
        {
            get
            {
                return this.sideButtonRightOrder;
            }
            set
            {
                this.sideButtonRightOrder = value;
            }
        }

        /// <summary>
        ///  Gets or Sets top autohidden panel's SideButton order
        /// </summary>
        public List<string> SideButtonTopOrder
        {
            get
            {
                return this.sideButtonTopOrder;
            }
            set
            {
                this.sideButtonTopOrder = value;
            }
        }

        /// <summary>
        ///  Gets or Sets bottom autohidden panel's SideButton order
        /// </summary>
        public List<string> SideButtonBottomOrder
        {
            get
            {
                return this.sideButtonBottomOrder;
            }
            set
            {
                this.sideButtonBottomOrder = value;
            }
        }

        /// <summary>
        /// Gets or Sets the active window name
        /// </summary>
        public string ActiveWindowName
        {
            get
            {
                return this.activeWindowName;
            }

            set
            {
                this.activeWindowName = value;
            }
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes new instance of the GroupBarParams class.
        /// </summary>
        public WindowParams()
        {
        }

        /// <summary>
        /// Initializes new instance of the GroupBarParams class.
        /// </summary>
        /// <param name="dockingManager">The docking manager.</param>
        public WindowParams(DockingManager dockingManager)
        {
            this.windowItems = new List<WindowItems>();
            List<Window> windowCollection = new List<Window>(dockingManager.WindowCollection.Values);
            foreach (Window item in windowCollection)
            {
                WindowItems itemParam = new WindowItems(item);
                this.windowItems.Add(itemParam);
            }

            this.sideButtonLeftOrder = new List<string>();
            this.sideButtonRightOrder = new List<string>();
            this.sideButtonTopOrder = new List<string>();
            this.sideButtonBottomOrder = new List<string>();

            foreach(var sideButton in dockingManager.btnPaneLeft.Children)
            {
                if(sideButton is SideButton)
                    this.sideButtonLeftOrder.Add((sideButton as SideButton).OwnWindow._Caption);
            }

            foreach (var sideButton in dockingManager.btnPaneRight.Children)
            {
                if (sideButton is SideButton)
                    this.sideButtonRightOrder.Add((sideButton as SideButton).OwnWindow._Caption);
            }

            foreach (var sideButton in dockingManager.btnPaneTop.Children)
            {
                if (sideButton is SideButton)
                    this.sideButtonTopOrder.Add((sideButton as SideButton).OwnWindow._Caption);
            }

            foreach (var sideButton in dockingManager.btnPaneBottom.Children)
            {
                if (sideButton is SideButton)
                    this.sideButtonBottomOrder.Add((sideButton as SideButton).OwnWindow._Caption);
            }

            if(dockingManager.ActiveWindow != null)
                this.ActiveWindowName = dockingManager.ActiveWindow._Caption;
        }

        #endregion
    }
}
