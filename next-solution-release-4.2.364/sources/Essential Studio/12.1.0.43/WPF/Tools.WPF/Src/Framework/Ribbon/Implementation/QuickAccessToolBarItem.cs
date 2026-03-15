// <copyright file="QuickAccessToolBarItem.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Class represents wrapper for QuickAccessToolBar items.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    internal class QuickAccessToolBarItem : IRibbonControl
    {
        #region Private members
        /// <summary>
        /// Represents the Tooltip
        /// </summary>
        private object m_toolTip;

        /// <summary>
        /// Represents the Label
        /// </summary>
        private string m_label;

        /// <summary>
        /// Represents the Image
        /// </summary>
        private ImageSource m_image;

        /// <summary>
        /// Represents the source element
        /// </summary>
        private UIElement m_sourceElement;

        /// <summary>
        /// Represents the Cloned Element
        /// </summary>
        private UIElement m_clonedElement;

        /// <summary>
        /// Command associated with provider.
        /// </summary>
        private ICommand m_command;

        /// <summary>
        /// CommandParameter associated with command.
        /// </summary>
        private object m_commandParameter;

        /// <summary>
        /// Index of the QuickAccessToolBarItem int QuickAccessToolBar.
        /// </summary>
        private int index = int.MaxValue;

        private static Ribbon m_ribbon = null;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the command.
        /// </summary>
        /// <value>The command.</value>
        public ICommand Command
        {
            get
            {
                return m_command;
            }
        }

        /// <summary>
        /// Gets the CommandParameter.
        /// </summary>
        /// <value>The command parameter.</value>
        public object CommandParameter
        {
            get
            {
                return m_commandParameter;
            }
        }

        /// <summary>
        /// Gets the value of the Label property.
        /// </summary>
        /// <value></value>
        public string Label
        {
            get
            {
                return m_label;
            }
        }

        /// <summary>
        /// Gets the value of the Image property.
        /// </summary>
        /// <value></value>
        public ImageSource SmallIcon
        {
            get
            {
                return m_image;
            }
        }

        /// <summary>
        /// Gets the value of the ToolTip property.
        /// </summary>
        /// <value></value>
        public object ToolTip
        {
            get
            {
                return m_toolTip;
            }
        }

        /// <summary>
        /// Gets the source element.
        /// </summary>
        /// <value>The source element.</value>
        public UIElement SourceElement
        {
            get
            {
                return m_sourceElement;
            }
        }

        /// <summary>
        /// Gets the cloned element.
        /// </summary>
        /// <value>The cloned element.</value>
        public UIElement ClonedElement
        {
            get
            {
                return m_clonedElement;
            }
        }

        /// <summary>
        /// Gets or Sets the Index of this item in QuickAccessToolBar.
        /// </summary>
        public int Index
        {
            get { return index; }
            set { index = value; }
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="QuickAccessToolBarItem"/> class.
        /// </summary>
        /// <param name="provider">The provider.</param>
        public QuickAccessToolBarItem(RibbonCommandProvider provider)
        {
            m_command = provider.Command;
            m_commandParameter = provider.CommandParameter;
            m_toolTip = provider.ToolTip;
            m_image = provider.SmallIcon;
            m_label = provider.Label;
            RibbonButton button = new RibbonButton();
            button.SizeForm = SizeForm.ExtraSmall;
            button.SmallIcon = SmallIcon;
            button.Label = Label;
            button.ToolTip = ToolTip;
            button.Command = provider.Command;
            button.CommandParameter = provider.CommandParameter;
            m_clonedElement = button;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QuickAccessToolBarItem"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        public QuickAccessToolBarItem(UIElement source)
        {
            if (source is ICommandSource)
            {
                m_command = (source as ICommandSource).Command;
                m_commandParameter = (source as ICommandSource).CommandParameter;
            }

            if ((source as FrameworkElement).Name == "PART_DialogLauncherButton")
            {
                RibbonBar parentBar = (source as FrameworkElement).TemplatedParent as RibbonBar;

                if (parentBar != null)
                {
                    m_toolTip = parentBar.LauncherToolTip;
                }
            }

            if (source is IRibbonControl)
            {
                IRibbonControl qatItem = (IRibbonControl)source;
                if (source is BackStageCommandButton)
                {
                    BackStageCommandButton cmdbutton = (source as BackStageCommandButton);
                    m_label = cmdbutton.Header;
                    m_image = cmdbutton.Icon;
                }
                else
                {
                    m_label = qatItem.Label;
                    m_image = qatItem.SmallIcon;
                }
                m_toolTip = qatItem.ToolTip;
                if (Command != null && RibbonCommandManager.CommandDictionary.ContainsKey(Command))
                {
                    if (RibbonCommandManager.CommandDictionary[Command].SmallIcon != null)
                    {
                        m_image = RibbonCommandManager.CommandDictionary[Command].SmallIcon;
                    }

                    if (RibbonCommandManager.CommandDictionary[Command].Label != null)
                    {
                        m_label = RibbonCommandManager.CommandDictionary[Command].Label;
                    }

                    if (RibbonCommandManager.CommandDictionary[Command].ToolTip != null)
                    {
                        m_toolTip = RibbonCommandManager.CommandDictionary[Command].ToolTip;
                    }
                }
            }
            else
            {
                //m_label = source.ToString();

                if (source is RibbonMenuItem && (source as RibbonMenuItem) != null && (source as RibbonMenuItem).Header != null)
                    m_label = (source as RibbonMenuItem).Header.ToString();
                else
                    m_label = source.DependencyObjectType.Name;
            }

            m_sourceElement = source;
            DependencyObject parent = (source as FrameworkElement).Parent;
            if (parent != null && !(parent is QuickAccessToolBar))
            {
                m_clonedElement = CloneManager.CloneGeneral(source, true);                
            }
            else
            {
                m_clonedElement = source;
            }

            if (m_ribbon == null)
                m_ribbon = Syncfusion.Windows.Shared.VisualUtils.FindAncestor(m_clonedElement as Visual, typeof(Ribbon)) as Ribbon;

            if (m_ribbon != null && m_ribbon.ShowDefaultQATKeyTip && !m_ribbon.isQATDialogOpened)
            {
                if (Ribbon.GetKeyTip(m_clonedElement) == string.Empty && m_clonedElement is IRibbonControl)
                {
                    Ribbon.SetKeyTip(m_clonedElement, AutomaticKeys.Next());
                }
                else if (m_clonedElement != null && m_clonedElement is IRibbonControl && m_ribbon.QuickAccessToolBar!=null && !m_ribbon.QuickAccessToolBar.Items.Contains(m_clonedElement))
                {
                    Ribbon.SetKeyTip(m_clonedElement, AutomaticKeys.Next());
                }
            }
        }
        #endregion
    }
}
