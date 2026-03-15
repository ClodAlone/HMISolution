// <copyright file="DiagramCommandManager.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
//using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml;
using System.Xml.Linq;
using System.Collections.ObjectModel;
using System.Windows.Shapes;

namespace Syncfusion.Windows.Diagram
{
    /// <summary>
    /// Represents the Diagram Command manager.
    /// </summary>
    public class DiagramCommandManager // : IDiagramCommandParameter  
    {
        internal static bool IsUnGroupCommand = false;

        internal static bool IsDeleteCommand = false;               

        #region Class Variables


        private static RoutedUICommand mSelectAllCommand = new RoutedUICommand("SelectAll","SelectAll",typeof(DiagramCommandManager));

        private static RoutedUICommand mFitToPageCommand = new RoutedUICommand("FitToPage", "FitToPage", typeof(DiagramCommandManager));
        /// <summary>
        /// AlignLeft Command
        /// </summary>
        private static RoutedUICommand alignleftcommand = new RoutedUICommand("AlignLeft", "AlignLeft", typeof(DiagramCommandManager));

        /// <summary>
        /// AlignLeft Command
        /// </summary>
        private static RoutedUICommand aligncentercommand = new RoutedUICommand("AlignCenter", "AlignCenter", typeof(DiagramCommandManager));

        /// <summary>
        /// AlignLeft Command
        /// </summary>
        private static RoutedUICommand alignrightcommand = new RoutedUICommand("AlignRight", "AlignRight", typeof(DiagramCommandManager));

        /// <summary>
        /// AlignLeft Command
        /// </summary>
        private static RoutedUICommand aligntopcommand = new RoutedUICommand("AlignTop", "AlignTop", typeof(DiagramCommandManager));

        /// <summary>
        /// AlignLeft Command
        /// </summary>
        private static RoutedUICommand alignmiddlecommand = new RoutedUICommand("AlignMiddle", "AlignMiddle", typeof(DiagramCommandManager));

        /// <summary>
        /// AlignLeft Command
        /// </summary>
        private static RoutedUICommand alignbottomcommand = new RoutedUICommand("AlignBottom", "AlignBottom", typeof(DiagramCommandManager));

        /// <summary>
        /// SpaceDown Command
        /// </summary>
        private static RoutedUICommand spacedowncommand = new RoutedUICommand("SpaceDown", "SpaceDown", typeof(DiagramCommandManager));

        /// <summary>
        /// SpaceAcross Command
        /// </summary>
        private static RoutedUICommand spaceacrosscommand = new RoutedUICommand("SpaceAcross", "SpaceAcross", typeof(DiagramCommandManager));

        /// <summary>
        /// SameSize Command
        /// </summary>
        private static RoutedUICommand samesizecommand = new RoutedUICommand("SameSize", "SameSize", typeof(DiagramCommandManager));

        /// <summary>
        /// SameWidth Command
        /// </summary>
        private static RoutedUICommand samewidthcommand = new RoutedUICommand("SameWidth", "SameWidth", typeof(DiagramCommandManager));

        /// <summary>
        /// SameHeight Command
        /// </summary>
        private static RoutedUICommand sameheightcommand = new RoutedUICommand("SameHeight", "SameHeight", typeof(DiagramCommandManager));

        /// <summary>
        /// Delete Command
        /// </summary>
        private static RoutedUICommand deletecommand = new RoutedUICommand("Delete", "Delete", typeof(DiagramCommandManager));

        /// <summary>
        /// Group Command
        /// </summary>
        private static RoutedUICommand groupcommand = new RoutedUICommand("Group", "Group", typeof(DiagramCommandManager));

        /// <summary>
        /// Ungroup Command
        /// </summary>
        private static RoutedUICommand ungroupcommand = new RoutedUICommand("Ungroup", "Ungroup", typeof(DiagramCommandManager));

        /// <summary>
        /// Move up Command
        /// </summary>
        private static RoutedUICommand mupcommand = new RoutedUICommand("MoveUp", "MoveUp", typeof(DiagramCommandManager));

        /// <summary>
        /// Move Down Command
        /// </summary>
        private static RoutedUICommand mdowncommand = new RoutedUICommand("MoveDown", "MoveDown", typeof(DiagramCommandManager));

        /// <summary>
        /// Move Left Command
        /// </summary>
        private static RoutedUICommand mleftcommand = new RoutedUICommand("MoveLeft", "MoveLeft", typeof(DiagramCommandManager));

        /// <summary>
        /// Move Right Command
        /// </summary>
        private static RoutedUICommand mrightcommand = new RoutedUICommand("MoveRight", "MoveRight", typeof(DiagramCommandManager));

        /// <summary>
        /// BringToFront command.
        /// </summary>
        private static RoutedUICommand mBringToFrontcommand = new RoutedUICommand("BringToFront", "BringToFront", typeof(DiagramCommandManager));

        /// <summary>
        /// SendToBack command.
        /// </summary>
        private static RoutedUICommand mSendToBackcommand = new RoutedUICommand("SendToBack", "SendToBack", typeof(DiagramCommandManager));

        /// <summary>
        /// MoveForward command.
        /// </summary>
        private static RoutedUICommand mMoveForwardcommand = new RoutedUICommand("MoveForward", "MoveForward", typeof(DiagramCommandManager));

        /// <summary>
        /// SendBackward command.
        /// </summary>
        private static RoutedUICommand mSendBackwardcommand = new RoutedUICommand("SendBackward", "SendBackward", typeof(DiagramCommandManager));

        /// <summary>
        /// Undo command
        /// </summary>
        private static RoutedUICommand mUndocommand = new RoutedUICommand("Undo", "Undo", typeof(DiagramCommandManager));

        /// <summary>
        /// Redo Command
        /// </summary>
        private static RoutedUICommand mRedocommand = new RoutedUICommand("Redo", "Redo", typeof(DiagramCommandManager));

        /// <summary>
        /// DiagramView instance.
        /// </summary>
        private DiagramView mDiagramView;

        /// <summary>
        /// Used to store the group count.
        /// </summary>
        private static int i = 1;

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="DiagramCommandManager"/> class.
        /// </summary>
        static DiagramCommandManager()
        {
            CommandBinding alignleftbinding = new CommandBinding(AlignLeft, OnAlignLeftCommand, CanExecuteAlignmentCommand);
            CommandBinding aligncenterbinding = new CommandBinding(AlignCenter, OnAlignCenterCommand, CanExecuteAlignmentCommand);
            CommandBinding alignrightbinding = new CommandBinding(AlignRight, OnAlignRightCommand, CanExecuteAlignmentCommand);
            CommandBinding aligntopbinding = new CommandBinding(AlignTop, OnAlignTopCommand, CanExecuteAlignmentCommand);
            CommandBinding alignmiddlebinding = new CommandBinding(AlignMiddle, OnAlignMiddleCommand, CanExecuteAlignmentCommand);
            CommandBinding alignbottombinding = new CommandBinding(AlignBottom, OnAlignBottomCommand, CanExecuteAlignmentCommand);

            CommandBinding spacedownbinding = new CommandBinding(SpaceDown, OnSpaceDownCommand, CanExecuteSpaceCommands);
            CommandBinding spaceacrossbinding = new CommandBinding(SpaceAcross, OnSpaceAcrossCommand, CanExecuteSpaceCommands);

            CommandBinding samesizebinding = new CommandBinding(SameSize, OnSameSizeCommand, CanExecuteSpaceCommands);
            CommandBinding samewidthbinding = new CommandBinding(SameWidth, OnSameWidthCommand, CanExecuteSpaceCommands);
            CommandBinding sameheightbinding = new CommandBinding(SameHeight, OnSameHeightCommand, CanExecuteSpaceCommands);

            CommandBinding deletebinding = new CommandBinding(Delete, OnDeleteCommand, CanExecuteDeleteCommand);
            CommandBinding moveupbinding = new CommandBinding(MoveUp, OnMoveUpCommand, CanExecuteNudgeCommands);
            CommandBinding movedownbinding = new CommandBinding(MoveDown, OnMoveDownCommand, CanExecuteNudgeCommands);
            CommandBinding moveleftbinding = new CommandBinding(MoveLeft, OnMoveLeftCommand, CanExecuteNudgeCommands);
            CommandBinding moverightbinding = new CommandBinding(MoveRight, OnMoveRightCommand, CanExecuteNudgeCommands);

            CommandBinding bringtofrontbinding = new CommandBinding(BringToFront, OnBringToFrontCommand, CanExecuteOrderCommands);
            CommandBinding sendtobackbinding = new CommandBinding(SendToBack, OnSendToBackCommand, CanExecuteOrderCommands);
            CommandBinding moveforwardbinding = new CommandBinding(MoveForward, OnMoveForwardCommand, CanExecuteOrderCommands);
            CommandBinding sendbackwardbinding = new CommandBinding(SendBackward, OnSendBackwardCommand, CanExecuteOrderCommands);

            CommandBinding groupbinding = new CommandBinding(Group, OnGroupCommand, CanExecuteGroupCommand);
            CommandBinding ungroupbinding = new CommandBinding(Ungroup, OnUngroupCommand, CanExecuteGroupCommand);

            CommandBinding undobinding = new CommandBinding(Undo, OnUndoCommand, CanExecuteUndoCommand);
            CommandBinding redobinding = new CommandBinding(Redo, OnRedoCommand, CanExecuteRedoCommand);
            CommandBinding BindSelect = new CommandBinding(SelectAll, SelectAllExecuted, SelectAllCanExecute);
            CommandBinding FitToPagecmd = new CommandBinding(FitToPage, FitToPageExecuted, FitToPageCanExecute);
            KeyGesture selectgeasture = new KeyGesture(Key.A, ModifierKeys.Control);
            InputBinding SelectInputBind = new InputBinding(mSelectAllCommand, selectgeasture);
            CommandManager.RegisterClassCommandBinding(typeof(DiagramView), FitToPagecmd);
            CommandManager.RegisterClassCommandBinding(typeof(DiagramView), BindSelect);
            CommandManager.RegisterClassInputBinding(typeof(DiagramView), SelectInputBind);
            CommandManager.RegisterClassCommandBinding(typeof(DiagramView), alignleftbinding);
            CommandManager.RegisterClassCommandBinding(typeof(DiagramView), aligncenterbinding);
            CommandManager.RegisterClassCommandBinding(typeof(DiagramView), alignrightbinding);
            CommandManager.RegisterClassCommandBinding(typeof(DiagramView), aligntopbinding);
            CommandManager.RegisterClassCommandBinding(typeof(DiagramView), alignmiddlebinding);
            CommandManager.RegisterClassCommandBinding(typeof(DiagramView), alignbottombinding);

            CommandManager.RegisterClassCommandBinding(typeof(DiagramView), spacedownbinding);
            CommandManager.RegisterClassCommandBinding(typeof(DiagramView), spaceacrossbinding);

            CommandManager.RegisterClassCommandBinding(typeof(DiagramView), samesizebinding);
            CommandManager.RegisterClassCommandBinding(typeof(DiagramView), samewidthbinding);
            CommandManager.RegisterClassCommandBinding(typeof(DiagramView), sameheightbinding);

            CommandManager.RegisterClassCommandBinding(typeof(DiagramView), deletebinding);

            CommandManager.RegisterClassCommandBinding(typeof(DiagramView), groupbinding);
            CommandManager.RegisterClassCommandBinding(typeof(DiagramView), ungroupbinding);

            CommandManager.RegisterClassCommandBinding(typeof(DiagramView), moveupbinding);
            CommandManager.RegisterClassCommandBinding(typeof(DiagramView), movedownbinding);
            CommandManager.RegisterClassCommandBinding(typeof(DiagramView), moveleftbinding);
            CommandManager.RegisterClassCommandBinding(typeof(DiagramView), moverightbinding);
            CommandManager.RegisterClassCommandBinding(typeof(DiagramView), bringtofrontbinding);
            CommandManager.RegisterClassCommandBinding(typeof(DiagramView), sendtobackbinding);
            CommandManager.RegisterClassCommandBinding(typeof(DiagramView), moveforwardbinding);
            CommandManager.RegisterClassCommandBinding(typeof(DiagramView), sendbackwardbinding);
            CommandManager.RegisterClassCommandBinding(typeof(DiagramView), undobinding);
            CommandManager.RegisterClassCommandBinding(typeof(DiagramView), redobinding);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DiagramCommandManager"/> class.
        /// </summary>
        /// <param name="view">The view instance.</param>
        public DiagramCommandManager(DiagramView view)
        {
            view.Loaded += new RoutedEventHandler(View_Loaded);
            mDiagramView = view;
            mDiagramView.CommandBindings.Add(new CommandBinding(ApplicationCommands.Delete, Delete_Executed, Delete_Enabled));

            mDiagramView.CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, OnCutCommand, CanExecuteCutCommand));
            mDiagramView.CommandBindings.Add(new CommandBinding(ApplicationCommands.Copy, OnCopyCommand, CanExecuteCopyCommand));
            mDiagramView.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, OnPasteCommand, CanExecutePasteCommand));

            //CommandBinding cutbinding = new CommandBinding(Cut,
            //CommandBinding copybinding = new CommandBinding(Copy, 
            //CommandBinding pastebinding = new CommandBinding(Paste,
            //Clipboard.Clear();
        }

        /// <summary>
        /// Handles the Loaded event of the view control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void View_Loaded(object sender, RoutedEventArgs e)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the AlignLeft RoutedUICommand.
        /// </summary>
        /// Type:<see cref="RoutedUICommand"/>
        /// <example>
        /// C#:
        /// <code language="C#">
        /// DiagramCommandManager.AlignLeft.Execute(diagramView.Page, diagramView);
        /// </code>
        /// </example>
        public static RoutedUICommand AlignLeft
        {
            get
            {
                return alignleftcommand;
            }
        }

        /// <summary>
        /// Gets the AlignCenter RoutedUICommand.
        /// </summary>
        /// Type:<see cref="RoutedUICommand"/>
        /// <example>
        /// C#:
        /// <code language="C#">
        /// DiagramCommandManager.AlignCenter.Execute(diagramView.Page, diagramView);
        /// </code>
        /// </example>
        public static RoutedUICommand AlignCenter
        {
            get
            {
                return aligncentercommand;
            }
        }

        /// <summary>
        /// Gets the AlignRight RoutedUICommand.
        /// </summary>
        /// Type:<see cref="RoutedUICommand"/>
        /// <example>
        /// C#:
        /// <code language="C#">
        /// DiagramCommandManager.AlignRight.Execute(diagramView.Page, diagramView);
        /// </code>
        /// </example>
        public static RoutedUICommand AlignRight
        {
            get
            {
                return alignrightcommand;
            }
        }

        /// <summary>
        /// Gets the AlignTop RoutedUICommand.
        /// </summary>
        /// Type:<see cref="RoutedUICommand"/>
        /// <example>
        /// C#:
        /// <code language="C#">
        /// DiagramCommandManager.AlignTop.Execute(diagramView.Page, diagramView);
        /// </code>
        /// </example>
        public static RoutedUICommand AlignTop
        {
            get
            {
                return aligntopcommand;
            }
        }

        /// <summary>
        /// Gets the AlignMiddle RoutedUICommand.
        /// </summary>
        /// Type:<see cref="RoutedUICommand"/>
        /// <example>
        /// C#:
        /// <code language="C#">
        /// DiagramCommandManager.AlignMiddle.Execute(diagramView.Page, diagramView);
        /// </code>
        /// </example>
        public static RoutedUICommand AlignMiddle
        {
            get
            {
                return alignmiddlecommand;
            }
        }

        /// <summary>
        /// Gets the AlignBottom RoutedUICommand.
        /// </summary>
        /// Type:<see cref="RoutedUICommand"/>
        /// <example>
        /// C#:
        /// <code language="C#">
        /// DiagramCommandManager.AlignBottom.Execute(diagramView.Page, diagramView);
        /// </code>
        /// </example>
        public static RoutedUICommand AlignBottom
        {
            get
            {
                return alignbottomcommand;
            }
        }

        /// <summary>
        /// Gets the SpaceDown RoutedUICommand.
        /// </summary>
        /// Type:<see cref="RoutedUICommand"/>
        /// <example>
        /// C#:
        /// <code language="C#">
        /// DiagramCommandManager.SpaceDown.Execute(diagramView.Page, diagramView);
        /// </code>
        /// </example>
        public static RoutedUICommand SpaceDown
        {
            get
            {
                return spacedowncommand;
            }
        }

        /// <summary>
        /// Gets the SpaceAcross RoutedUICommand.
        /// </summary>
        /// Type:<see cref="RoutedUICommand"/>
        /// <example>
        /// C#:
        /// <code language="C#">
        /// DiagramCommandManager.SpaceAcross.Execute(diagramView.Page, diagramView);
        /// </code>
        /// </example>
        public static RoutedUICommand SpaceAcross
        {
            get
            {
                return spaceacrosscommand;
            }
        }

        /// <summary>
        /// Gets the SameSize RoutedUICommand.
        /// </summary>
        /// Type:<see cref="RoutedUICommand"/>
        /// <example>
        /// C#:
        /// <code language="C#">
        /// DiagramCommandManager.SameSize.Execute(diagramView.Page, diagramView);
        /// </code>
        /// </example>
        public static RoutedUICommand SameSize
        {
            get
            {
                return samesizecommand;
            }
        }

        /// <summary>
        /// Gets the SameWidth RoutedUICommand.
        /// </summary>
        /// Type:<see cref="RoutedUICommand"/>
        /// <example>
        /// C#:
        /// <code language="C#">
        /// DiagramCommandManager.SameWidth.Execute(diagramView.Page, diagramView);
        /// </code>
        /// </example>
        public static RoutedUICommand SameWidth
        {
            get
            {
                return samewidthcommand;
            }
        }

        /// <summary>
        /// Gets the SameHeight RoutedUICommand.
        /// </summary>
        /// Type:<see cref="RoutedUICommand"/>
        /// <example>
        /// C#:
        /// <code language="C#">
        /// DiagramCommandManager.SameHeight.Execute(diagramView.Page, diagramView);
        /// </code>
        /// </example>
        public static RoutedUICommand SameHeight
        {
            get
            {
                return sameheightcommand;
            }
        }

        /// <summary>
        /// Gets the Delete RoutedUICommand.
        /// </summary>
        /// Type:<see cref="RoutedUICommand"/>
        /// <example>
        /// C#:
        /// <code language="C#">
        /// DiagramCommandManager.Delete.Execute(diagramView.Page, diagramView);
        /// </code>
        /// </example>
        public static RoutedUICommand Delete
        {
            get
            {
                return deletecommand;
            }
        }

        /// <summary>
        /// Gets the Group RoutedUICommand.
        /// </summary>
        /// Type:<see cref="RoutedUICommand"/>
        /// <example>
        /// C#:
        /// <code language="C#">
        /// DiagramCommandManager.Group.Execute(diagramView.Page, diagramView);
        /// </code>
        /// </example>
        public static RoutedUICommand Group
        {
            get
            {
                return groupcommand;
            }
        }

        /// <summary>
        /// Gets the Ungroup RoutedUICommand.
        /// </summary>
        /// Type:<see cref="RoutedUICommand"/>
        /// <example>
        /// C#:
        /// <code language="C#">
        /// DiagramCommandManager.Ungroup.Execute(diagramView.Page, diagramView);
        /// </code>
        /// </example>
        public static RoutedUICommand Ungroup
        {
            get
            {
                return ungroupcommand;
            }
        }

        /// <summary>
        /// Gets the MoveUp RoutedUICommand.
        /// </summary>
        /// Type:<see cref="RoutedUICommand"/>
        /// <example>
        /// C#:
        /// <code language="C#">
        /// DiagramCommandManager.MoveUp.Execute(diagramView.Page, diagramView);
        /// </code>
        /// </example>
        public static RoutedUICommand MoveUp
        {
            get
            {
                return mupcommand;
            }
        }

        /// <summary>
        /// Gets the MoveDown RoutedUICommand.
        /// </summary>
        /// Type:<see cref="RoutedUICommand"/>
        /// <example>
        /// C#:
        /// <code language="C#">
        /// DiagramCommandManager.MoveDown.Execute(diagramView.Page, diagramView);
        /// </code>
        /// </example>
        public static RoutedUICommand MoveDown
        {
            get
            {
                return mdowncommand;
            }
        }

        /// <summary>
        /// Gets the MoveLeft RoutedUICommand.
        /// </summary>
        /// Type:<see cref="RoutedUICommand"/>
        /// <example>
        /// C#:
        /// <code language="C#">
        /// DiagramCommandManager.MoveLeft.Execute(diagramView.Page, diagramView);
        /// </code>
        /// </example>
        public static RoutedUICommand MoveLeft
        {
            get
            {
                return mleftcommand;
            }
        }

        /// <summary>
        /// Gets the MoveRight RoutedUICommand.
        /// </summary>
        /// Type:<see cref="RoutedUICommand"/>
        /// <example>
        /// C#:
        /// <code language="C#">
        /// DiagramCommandManager.MoveRight.Execute(diagramView.Page, diagramView);
        /// </code>
        /// </example>
        public static RoutedUICommand MoveRight
        {
            get
            {
                return mrightcommand;
            }
        }

        /// <summary>
        /// Gets the BringToFront RoutedUICommand.
        /// </summary>
        /// Type:<see cref="RoutedUICommand"/>
        /// <example>
        /// C#:
        /// <code language="C#">
        /// DiagramCommandManager.BringToFront.Execute(diagramView.Page, diagramView);
        /// </code>
        /// </example>
        public static RoutedUICommand BringToFront
        {
            get
            {
                return mBringToFrontcommand;
            }
        }

        /// <summary>
        /// Gets the SendToBack RoutedUICommand.
        /// </summary>
        /// Type:<see cref="RoutedUICommand"/>
        /// <example>
        /// C#:
        /// <code language="C#">
        /// DiagramCommandManager.SendToBack.Execute(diagramView.Page, diagramView);
        /// </code>
        /// </example>
        public static RoutedUICommand SendToBack
        {
            get
            {
                return mSendToBackcommand;
            }
        }

        /// <summary>
        /// Gets the MoveForward RoutedUICommand.
        /// </summary>
        /// Type:<see cref="RoutedUICommand"/>
        /// <example>
        /// C#:
        /// <code language="C#">
        /// DiagramCommandManager.MoveForward.Execute(diagramView.Page, diagramView);
        /// </code>
        /// </example>
        public static RoutedUICommand MoveForward
        {
            get
            {
                return mMoveForwardcommand;
            }
        }

        /// <summary>
        /// Gets the SendBackward RoutedUICommand.
        /// </summary>
        /// Type:<see cref="RoutedUICommand"/>
        /// <example>
        /// C#:
        /// <code language="C#">
        /// DiagramCommandManager.SendBackward.Execute(diagramView.Page, diagramView);
        /// </code>
        /// </example>
        public static RoutedUICommand SendBackward
        {
            get
            {
                return mSendBackwardcommand;
            }
        }

        /// <summary>
        /// Gets the Undo RoutedUICommand.
        /// </summary>
        /// Type:<see cref="RoutedUICommand"/>
        /// <example>
        /// C#:
        /// <code language="C#">
        /// DiagramCommandManager.Undo.Execute(diagramView.Page, diagramView);
        /// </code>
        /// </example>
        public static RoutedUICommand Undo
        {
            get
            {
                return mUndocommand;
            }
        }


        public static RoutedUICommand SelectAll
        {
            get
            {
                return mSelectAllCommand;
            }
        }

        public static RoutedUICommand FitToPage
        {
            get
            {
                return mFitToPageCommand;
            }
        }

        /// <summary>
        /// Gets the Redo RoutedUICommand.
        /// </summary>
        /// Type:<see cref="RoutedUICommand"/>
        /// <example>
        /// C#:
        /// <code language="C#">
        /// DiagramCommandManager.Redo.Execute(diagramView.Page, diagramView);
        /// </code>
        /// </example>
        public static RoutedUICommand Redo
        {
            get
            {
                return mRedocommand;
            }
        }

        #endregion

        #region Delete Commands

        /// <summary>
        /// Calls Delete_Executed method of the instance, notifies of the sender value changes.
        /// </summary>
        /// <param name="sender">object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private void Delete_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (mDiagramView.IsPageEditable)
            {
                DiagramCommandManager.DeleteObjects(sender as DiagramView);
            }
        }

        /// <summary>
        /// Calls Delete_Enabled method of the instance, notifies of the sender value changes.
        /// </summary>
        /// <param name="sender">object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private void Delete_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = mDiagramView.SelectionList.Count > 0;
        }

        #endregion

        #region Implementation

        /// <summary>
        /// Deletes the objects.
        /// </summary>
        /// <param name="mDiagramView">The diagram view instance.</param>
        /// <remarks>Select the shape to be deleted and press the Delete key.</remarks>
        private static void DeleteObjects(DiagramView mDiagramView)
        {

            mDiagramView.DeleteCount = 0;
            mDiagramView.IsDeleteCommandExecuted = true;
            CollectionExt.Cleared = false;
            DiagramControl dc = DiagramPage.GetDiagramControl(mDiagramView) as DiagramControl;

            mDiagramView.DupDeleted = true;
            dc = DiagramPage.GetDiagramControl(mDiagramView) as DiagramControl;
           
            ////////////////////////////////////////////////////////
            List<IShape> li = new List<IShape>();
            foreach (IShape item in mDiagramView.SelectionList.OfType<IShape>())
            {
                li.Add(item);
            }
            foreach (IShape item in li)
            {
                if (item is Group)
                {
                    NodeDeleteRoutedEventArgs newEventArgsgroup = new NodeDeleteRoutedEventArgs(item as Node);
                    newEventArgsgroup.RoutedEvent = DiagramView.NodeDeletingEvent;
                    dc.View.RaiseEvent(newEventArgsgroup);
                    if (newEventArgsgroup.Cancel == false)
                    {
                        if ((item as Group).AllowDelete)
                        {
                            List<INodeGroup> INode = new List<INodeGroup>();
                            foreach (INodeGroup shape in (item as Group).NodeChildren)
                            {
                                INode.Add(shape);
                            }
                            foreach (INodeGroup shape in INode)
                            {
                                List<INodeGroup> IGroup = new List<INodeGroup>();
                                foreach (Group g in (shape as INodeGroup).Groups)
                                {
                                    IGroup.Add(g);
                                }
                                foreach (Group g in IGroup)
                                {
                                    if (g != (item as Group) && g.NodeChildren.Contains(shape as INodeGroup))
                                    {
                                        g.NodeChildren.Remove(shape);
                                    }

                                    if (g.NodeChildren.Count == 1)
                                    {
                                        if (g is Node)
                                        {
                                            dc.View.Isnodedeleted = true;

                                            NodeDeleteRoutedEventArgs newEventArgs = new NodeDeleteRoutedEventArgs(g as Node);
                                            newEventArgs.RoutedEvent = DiagramView.NodeDeletingEvent;
                                            dc.View.RaiseEvent(newEventArgs);
                                            if (newEventArgs.Cancel == false)
                                            {
                                                dc.Model.Nodes.Remove(g as Control);
                                                dc.View.Isnodedeleted = false;
                                            }
                                        }
                                    }
                                }

                                if (shape is Node && (shape as Node).AllowDelete && (shape as Node).Groups == null)
                                {
                                    dc.View.Isnodedeleted = true;
                                    (shape as Node).IsSelected = false;
                                    NodeDeleteRoutedEventArgs newEventArgs2 = new NodeDeleteRoutedEventArgs(shape as Node);
                                    newEventArgs2.RoutedEvent = DiagramView.NodeDeletingEvent;
                                    dc.View.RaiseEvent(newEventArgs2);
                                    if (newEventArgs2.Cancel == false)
                                    {
                                        dc.Model.Nodes.Remove(shape as Control);
                                        dc.View.Isnodedeleted = false;
                                    }
                                }
                                else if (shape is Node && (shape as Node).Groups != null)
                                {
                                    if ((shape as Node).AllowDelete)
                                    {
                                        dc.View.Isnodedeleted = true;
                                        (shape as Node).IsSelected = false;
                                        NodeDeleteRoutedEventArgs newEventArgs2 = new NodeDeleteRoutedEventArgs(shape as Node);
                                        newEventArgs2.RoutedEvent = DiagramView.NodeDeletingEvent;
                                        dc.View.RaiseEvent(newEventArgs2);
                                        if (newEventArgs2.Cancel == false)
                                        {
                                            dc.Model.Nodes.Remove(shape as Control);
                                            dc.View.Isnodedeleted = false;
                                        }
                                    }
                                }

                                else if (shape is LineConnector)
                                {
                                    if ((shape as LineConnector).AllowDelete)
                                    {
                                        dc.View.Islinedeleted = true;
                                        (shape as LineConnector).IsSelected = false;
                                        ConnectionDeleteRoutedEventArgs lnewEventArgs = new ConnectionDeleteRoutedEventArgs(shape as LineConnector);
                                        lnewEventArgs.RoutedEvent = DiagramView.ConnectorDeletingEvent;
                                        dc.View.RaiseEvent(lnewEventArgs);
                                        if (lnewEventArgs.Cancel == false)
                                        {
                                            dc.Model.Connections.Remove(shape as LineConnector);
                                            dc.View.Islinedeleted = false;
                                        }
                                    }
                                }
                            }
                            for (int i = 0; i < (item as Group).NodeChildren.Count; i++)

                            //foreach (INodeGroup shape in (item as Group).NodeChildren)
                            {
                                object shape = (item as Group).NodeChildren[i] as object;
                                if ((shape is Node) && !(shape as Node).AllowDelete)
                                {
                                    (shape as Node).Groups.Remove(item);

                                }

                            }
                        }
                        (item as Group).IsSelected = false;
                        dc.Model.Nodes.Remove(item as Control);
                        dc.View.Isnodedeleted = false;
                        continue;
                    }
                                        
                }
                List<Group> listGroup = new List<Group>();
                foreach (Group g in (item as Node).Groups)
                {
                    listGroup.Add(g);
                }

                foreach (Group g in listGroup)
                {                    
                    if (!g.AllowSingleChild)
                    {
                        if (g.NodeChildren.Count == 1)
                        {                            
                            NodeDeleteRoutedEventArgs newEventArgs = new NodeDeleteRoutedEventArgs(g as Node);
                            newEventArgs.RoutedEvent = DiagramView.NodeDeletingEvent;
                            dc.View.RaiseEvent(newEventArgs);
                            if (newEventArgs.Cancel == false)
                            {
                                IsDeleteCommand = true;
                                item.IsSelected = false;
                                g.NodeChildren.Remove((item as Node));
                                dc.View.Isnodedeleted = true;
                                g.IsSelected = false;
                                dc.Model.Nodes.Remove(g as Control);
                                dc.View.Isnodedeleted = false;
                            }
                        }
                    }
                }
                if ((item as Node).AllowDelete)
                {                    
                    NodeDeleteRoutedEventArgs newEventArgs3 = new NodeDeleteRoutedEventArgs(item as Node);
                    newEventArgs3.RoutedEvent = DiagramView.NodeDeletingEvent;
                    dc.View.RaiseEvent(newEventArgs3);
                    if (newEventArgs3.Cancel == false)
                    {
                        dc.View.Isnodedeleted = true;
                        item.IsSelected = false;
                        dc.Model.Nodes.Remove(item as Control);
                        dc.View.Isnodedeleted = false;
                        mDiagramView.SelectionList.Remove(item);
                        foreach (Group g in (item as Node).Groups)
                        {
                            g.NodeChildren.Remove(item as Node);
                        }
                    }
                }
            }
            /////////////////////////////////////////////////////////

            List<IEdge> liforconnector = new List<IEdge>();
            foreach (IEdge item in mDiagramView.SelectionList.OfType<IEdge>())
            {
                liforconnector.Add(item);
            }

            foreach (IEdge connection in liforconnector)
            {
                if ((connection as LineConnector).AllowDelete)
                {                    
                    ConnectionDeleteRoutedEventArgs newEventArgs = new ConnectionDeleteRoutedEventArgs(connection as LineConnector);
                    newEventArgs.RoutedEvent = DiagramView.ConnectorDeletingEvent;
                    dc.View.RaiseEvent(newEventArgs);
                    if (newEventArgs.Cancel == false)
                    {
                        dc.View.Islinedeleted = true;
                        (connection as LineConnector).IsSelected = false;
                        dc.Model.Connections.Remove(connection as LineConnector);
                        dc.View.Islinedeleted = false;
                        mDiagramView.SelectionList.Remove(connection);
                        foreach (Group g in (connection as LineConnector).Groups)
                        {
                            g.NodeChildren.Remove(connection as LineConnector);
                        }
                    }
                }
            }
//////////////////////////////////////////////////////

            //mDiagramView.SelectionList.Clear();
            List<UIElement> ordered = (from UIElement item in mDiagramView.Page.Children.OfType<ICommon>()
                                       orderby Canvas.GetZIndex(item as UIElement)
                                       select item as UIElement).ToList();

            for (int i = 0; i < ordered.Count; i++)
            {
                Canvas.SetZIndex(ordered[i], i + 1);
            }
            if (mDiagramView.UndoRedoEnabled)
            {
                mDiagramView.UndoStack.Push(mDiagramView.DeleteCount);
                mDiagramView.UndoStack.Push("DeleteCount");
                mDiagramView.IsDeleteCommandExecuted = false;
            }
        }

        /// <summary>
        /// Updates the ZOrder.
        /// </summary>
        internal static void UpdateZOrder(DiagramView mDiagramView)
        {
            List<UIElement> ordered = (from UIElement item in mDiagramView.Page.Children.OfType<ICommon>()
                                       orderby Canvas.GetZIndex(item as UIElement)
                                       select item as UIElement).ToList();

            for (int i = 0; i < ordered.Count; i++)
            {
                Canvas.SetZIndex(ordered[i], i+1);
            }
        }

        /// <summary>
        /// Performs Redo operation.
        /// </summary>
        /// <param name="mDiagramView">The DiagramView instance.</param>
        public static void RedoCommand(DiagramView mDiagramView)
        {
            DiagramControl dc;

            if (mDiagramView != null && mDiagramView.IsPageEditable && mDiagramView.RedoStack.Count != 0)
            {
                int deletecount = 1;
                int tdeletecount = 1;
                bool deletecommand = false;
                mDiagramView.Redone = true;
                dc = DiagramPage.GetDiagramControl(mDiagramView) as DiagramControl;
                if (dc != null && dc.View != null && dc.View.UndoRedoEnabled)
                {
                }
                else
                {
                    mDiagramView.UndoStack.Clear();
                    mDiagramView.RedoStack.Clear();
                    return;
                }

                object obj = mDiagramView.RedoStack.Pop();
                if (obj.ToString() == "DeleteCount")
                {
                    deletecount = (int)mDiagramView.RedoStack.Pop();
                    if (deletecount == 0)
                    {
                        deletecount = 1;
                    }

                    tdeletecount = deletecount;
                    obj = mDiagramView.RedoStack.Pop();
                    if (deletecount > 1)
                    {
                        deletecommand = true;
                    }
                }

                switch (obj as string)
                {
                    case "Dragged":
                        {
                            int nodecount = 1;
                            bool exe = false;
                            obj = mDiagramView.RedoStack.Pop();
                            if (obj is int)
                            {
                                nodecount = (int)obj;
                                mDiagramView.NodeDragCount = nodecount;
                                exe = true;
                            }
                            int tnodecount = nodecount;

                            while (nodecount > 0)
                            {
                                if (exe)
                                {
                                    if (mDiagramView.RedoStack.Count == 0)
                                    {
                                        break;
                                    }
                                    obj = mDiagramView.RedoStack.Pop();
                                    if (obj.ToString() == "Dragged")
                                    {
                                        mDiagramView.RedoStack.Pop();
                                        obj = mDiagramView.RedoStack.Pop();
                                    }
                                }

                                if (obj is Node)
                                {
                                    Node n = obj as Node;
                                    obj = mDiagramView.RedoStack.Pop();
                                    Point pos = new Point(n.PxOffsetX, n.PxOffsetY);
                                    mDiagramView.UndoStack.Push(pos);
                                    mDiagramView.UndoStack.Push(n);
                                    mDiagramView.UndoStack.Push(tnodecount);
                                    mDiagramView.UndoStack.Push("Dragged");
                                    if (obj is Point)
                                    {
                                        Point p = (Point)obj;
                                        n.PxOffsetX = mDiagramView.ConvertValue(p.X);
                                        n.PxOffsetY = mDiagramView.ConvertValue(p.Y);
                                    }
                                }

                                (mDiagramView.Page as DiagramPage).InvalidateMeasure();
                                (mDiagramView.Page as DiagramPage).InvalidateArrange();

                                if (obj is LineConnector)
                                {
                                    Node n = null;
                                    Point p = new Point();
                                    LineConnector l = obj as LineConnector;
                                    obj = mDiagramView.RedoStack.Pop();
                                    string s = (string)obj;
                                    obj = mDiagramView.RedoStack.Pop();

                                    if (obj is Node)
                                    {
                                        n = obj as Node;
                                    }
                                    else if (obj is Point)
                                    {
                                        p = (Point)obj;
                                    }
                                   
                                    if (s == "Headend")
                                    {
                                        if (l.HeadNode != null)
                                        {
                                            mDiagramView.UndoStack.Push(l.HeadNode);
                                        }
                                        else
                                        {
                                            Point sp = new Point(l.PxStartPointPosition.X, l.PxStartPointPosition.Y);
                                            mDiagramView.UndoStack.Push(sp);
                                        }
                                        mDiagramView.UndoStack.Push("Headend");
                                        mDiagramView.UndoStack.Push(l);
                                        mDiagramView.UndoStack.Push("Dragged");

                                        if (n != null)
                                        {
                                            l.HeadNode = n;
                                        }
                                        else
                                        {
                                            l.HeadNode = null;
                                            l.PxStartPointPosition = new Point(mDiagramView.ConvertValue(p.X), mDiagramView.ConvertValue(p.Y));

                                            if (l.IntermediatePoints != null && l.IntermediatePoints.Count >= 2 && l.ConnectorType == ConnectorType.Orthogonal)
                                            {
                                                if (l.IntermediatePoints[0].X == l.IntermediatePoints[1].X)
                                                {
                                                    l.IntermediatePoints[0] = new Point(l.IntermediatePoints[0].X, l.PxStartPointPosition.Y);
                                                }
                                                else
                                                {
                                                    l.IntermediatePoints[0] = new Point(l.PxStartPointPosition.X, l.IntermediatePoints[0].Y);
                                                }
                                            }
                                        }
                                    }
                                    else if (s == "Tailend")
                                    {
                                        if (l.TailNode != null)
                                        {
                                            mDiagramView.UndoStack.Push(l.TailNode);
                                        }
                                        else
                                        {
                                            Point sp = new Point(l.PxEndPointPosition.X, l.PxEndPointPosition.Y);
                                            mDiagramView.UndoStack.Push(sp);
                                        }
                                        mDiagramView.UndoStack.Push("Tailend");
                                        mDiagramView.UndoStack.Push(l);
                                        mDiagramView.UndoStack.Push("Dragged");

                                        if (n != null)
                                        {
                                            l.TailNode = n;
                                        }
                                        else
                                        {
                                            l.TailNode = null;
                                            l.PxEndPointPosition = new Point(mDiagramView.ConvertValue(p.X), mDiagramView.ConvertValue(p.Y));

                                            if (l.IntermediatePoints != null && l.IntermediatePoints.Count >= 2 && l.ConnectorType == ConnectorType.Orthogonal)
                                            {
                                                int finalPt = l.IntermediatePoints.Count - 1;
                                                if (l.IntermediatePoints[finalPt].X == l.IntermediatePoints[finalPt - 1].X)
                                                {
                                                    l.IntermediatePoints[finalPt] = new Point(l.IntermediatePoints[0].X, l.PxEndPointPosition.Y);
                                                }
                                                else
                                                {
                                                    l.IntermediatePoints[finalPt] = new Point(l.PxEndPointPosition.X, l.IntermediatePoints[0].Y);
                                                }
                                            }
                                        }
                                    }
                                    else if (s == "LineMoved")
                                    {

                                        if (l.HeadNode != null)
                                        {
                                            mDiagramView.UndoStack.Push(l.HeadNode);
                                        }
                                        else
                                        {
                                            Point sp = new Point(l.PxStartPointPosition.X, l.PxStartPointPosition.Y);
                                            mDiagramView.UndoStack.Push(sp);
                                        }

                                        if (l.TailNode != null)
                                        {
                                            mDiagramView.UndoStack.Push(l.TailNode);
                                        }
                                        else
                                        {
                                            Point sp = new Point(l.PxEndPointPosition.X, l.PxEndPointPosition.Y);
                                            mDiagramView.UndoStack.Push(sp);
                                        }
                                        mDiagramView.UndoStack.Push("LineMoved");
                                        mDiagramView.UndoStack.Push(l);
                                        mDiagramView.UndoStack.Push("Dragged");
                                        l.PxStartPointPosition = new Point(mDiagramView.ConvertValue(p.X), mDiagramView.ConvertValue(p.Y));
                                        obj = mDiagramView.RedoStack.Pop();
                                        if (obj is Point)
                                        {
                                            p = (Point)obj;
                                        }
                                        l.PxEndPointPosition = new Point(mDiagramView.ConvertValue(p.X), mDiagramView.ConvertValue(p.Y));
                                    }
                                    else if (s == "Vertex")
                                    {
                                        int i = (int)obj;
                                        while (i > 0)
                                        {
                                            int index = (int)mDiagramView.RedoStack.Pop();
                                            Point tempPt = l.IntermediatePoints[index];
                                            mDiagramView.UndoStack.Push(l.IntermediatePoints[index]);
                                            mDiagramView.UndoStack.Push(index);
                                            l.IntermediatePoints[index] = (Point)mDiagramView.RedoStack.Pop();
                                            i--;
                                        }
                                        mDiagramView.UndoStack.Push((int)obj);
                                        mDiagramView.UndoStack.Push("Vertex");
                                        mDiagramView.UndoStack.Push(l);
                                        mDiagramView.UndoStack.Push("Dragged");
                                    }

                                    l.InvalidateConnectorPathGeometry();
                                }

                                nodecount--;
                            }

                            mDiagramView.Redone = true;
                            break;
                        }

                    case "Added":
                        {
                            while (deletecount > 0)
                            {
                                mDiagramView.IsDeleteCommandExecuted = true;
                                obj = mDiagramView.RedoStack.Pop();
                                if (obj is Node)
                                {
                                    if ((obj as Node).IsGrouped)
                                    {
                                        foreach (Group g in (obj as Node).Groups)
                                        {
                                            g.NodeChildren.Remove(obj as Node);
                                        }
                                    }

                                    if (obj is Group)
                                    {
                                        foreach (INodeGroup element in (obj as Group).NodeChildren)
                                        {
                                            element.Groups.Remove(obj);
                                            if (element.Groups.Count == 0)
                                            {
                                                element.IsGrouped = false;
                                            }
                                        }
                                    }

                                    dc.Model.Nodes.Remove(obj as Node);
                                }
                                else if (obj is LineConnector)
                                {
                                    dc.Model.Connections.Remove(obj as LineConnector);
                                }

                                if (deletecommand && deletecount != 1)
                                {
                                    mDiagramView.RedoStack.Pop();
                                }

                                deletecount--;
                            }

                            mDiagramView.UndoStack.Push(tdeletecount);
                            mDiagramView.UndoStack.Push("DeleteCount");
                            break;
                        }

                    case "CSAdd":
                        LineConnector li = (LineConnector)mDiagramView.RedoStack.Pop();
                        if (li.ConnectorType == ConnectorType.Orthogonal)
                        {
                            int t = (int)mDiagramView.RedoStack.Pop();
                            Point pt1 = li.IntermediatePoints[t];
                            li.IntermediatePoints[t] = (Point)mDiagramView.RedoStack.Pop();
                            Point Pt2 = (Point)mDiagramView.RedoStack.Pop();
                            Point Pt3 = (Point)mDiagramView.RedoStack.Pop();
                            li.IntermediatePoints.RemoveAt(t + 1);
                            li.IntermediatePoints.RemoveAt(t + 1);
                            mDiagramView.UndoStack.Push(Pt3);
                            mDiagramView.UndoStack.Push(Pt2);
                            mDiagramView.UndoStack.Push(pt1);
                            mDiagramView.UndoStack.Push(t);
                            mDiagramView.UndoStack.Push(li);
                            mDiagramView.UndoStack.Push("CSDelete");
                        }
                        else if (li.ConnectorType == ConnectorType.Straight)
                        {
                            int t = (int)mDiagramView.RedoStack.Pop();
                            Point pt1 = li.IntermediatePoints[t];

                            li.IntermediatePoints.RemoveAt(t);

                            mDiagramView.UndoStack.Push((Point)mDiagramView.RedoStack.Pop());
                            mDiagramView.UndoStack.Push(t);
                            mDiagramView.UndoStack.Push(li);
                            mDiagramView.UndoStack.Push("CSDelete");
                        }
                        li.InvalidateConnectorPathGeometry();
                        break;
                    case "Deleted":
                        {
                            obj = mDiagramView.RedoStack.Pop();
                            int addcount = 1;
                            int taddcount = 0;
                            bool multi = false;
                            if (obj is bool)
                            {
                                addcount = (int)mDiagramView.RedoStack.Pop();
                                multi = true;
                                taddcount = addcount;
                            }
                            while (addcount > 0)
                            {
                                if (multi)
                                {
                                    mDiagramView.RedoStack.Pop();
                                    obj = mDiagramView.RedoStack.Pop();
                                }
                                addcount--;
                                if (obj is Point)
                                {
                                    Point p = (Point)obj;
                                    LineConnector l = (LineConnector)mDiagramView.RedoStack.Pop();
                                    int index = (int)mDiagramView.RedoStack.Pop();
                                    mDiagramView.UndoStack.Push(index);
                                    mDiagramView.UndoStack.Push(l);
                                    mDiagramView.UndoStack.Push(p);
                                    mDiagramView.UndoStack.Push("Added");

                                    if (l.ConnectorType == ConnectorType.Orthogonal)
                                    {
                                        l.IntermediatePoints.Insert(index, p);
                                        l.IntermediatePoints.Insert(index, p);
                                    }
                                    if (l.ConnectorType == ConnectorType.Straight)
                                    {
                                        l.IntermediatePoints.Insert(index, p);
                                    }
                                    l.InvalidateConnectorPathGeometry();
                                    break;
                                }
                                int zindex = (int)obj;
                                obj = mDiagramView.RedoStack.Pop();
                                if (obj is Node)
                                {
                                    Panel.SetZIndex(obj as UIElement, zindex);
                                    if (obj is Group)
                                    {
                                        foreach (INodeGroup element in (obj as Group).NodeChildren)
                                        {
                                            if (!element.Groups.Contains(obj))
                                            {
                                                element.Groups.Add(obj);
                                            }

                                            element.IsGrouped = true;
                                        }
                                    }
                                    if (!dc.Model.Nodes.Contains(obj))
                                        dc.Model.Nodes.Add(obj as Node);
                                    if ((obj as Node).IsGrouped)
                                    {
                                        foreach (Group g in (obj as Node).Groups)
                                        {
                                            g.NodeChildren.Add(obj as Node);
                                            foreach (ICommon shape in g.NodeChildren)
                                            {
                                                (shape as Node).IsGrouped = true;
                                            }
                                        }
                                    }
                                    if (!dc.Model.Nodes.Contains(obj))
                                        dc.Model.Nodes.Add(obj as Node);
                                }
                                else if (obj is LineConnector)
                                {
                                    dc.Model.Connections.Add(obj as LineConnector);
                                    Panel.SetZIndex(obj as LineConnector, zindex);
                                }

                                foreach (UIElement element in mDiagramView.Page.Children.OfType<ICommon>())
                                {
                                    if (element != (obj as UIElement) && (Panel.GetZIndex(element) >= zindex))
                                    {
                                        Panel.SetZIndex(element, (Panel.GetZIndex(element) + 1));
                                    }
                                }
                            }
                            if (multi)
                            {
                                mDiagramView.UndoStack.Push(taddcount);
                                mDiagramView.UndoStack.Push("Added");
                            }
                            break;
                        }

                    case "Rotated":
                        {
                            obj = mDiagramView.RedoStack.Pop();
                            int count = (int)obj;
                            int tempcount = (int)obj;
                            mDiagramView.NodeRotateCount = count;

                            while (count > 0)
                            {
                                obj = mDiagramView.RedoStack.Pop();
                                double angle = (double)obj;
                                obj = mDiagramView.RedoStack.Pop();
                                if (obj is Node)
                                {
                                    Node n = obj as Node;
                                    RotateTransform rotateTransform = n.RenderTransform as RotateTransform;
                                    rotateTransform.Angle = angle;
                                    n.RotateAngle = angle;
                                    foreach (LineConnector l in dc.Model.Connections)
                                    {
                                        if (l.HeadNode == n || l.TailNode == n)
                                        {
                                            l.InvalidateConnectorPathGeometry();
                                        }
                                    }
                                    if (count != 1)
                                    {
                                        mDiagramView.RedoStack.Pop();
                                        mDiagramView.RedoStack.Pop();
                                    }

                                    count--;
                                }
                            }

                            (mDiagramView.Page as DiagramPage).InvalidateMeasure();
                            (mDiagramView.Page as DiagramPage).InvalidateArrange();
                            break;
                        }

                    case "Resized":
                        {
                            int itemcount = 1;
                            obj = mDiagramView.RedoStack.Pop();
                            itemcount = (int)obj;
                            int tempcount = (int)obj;
                            object temp1 = null;
                            object temp2 = null;

                            while (itemcount >= 0)
                            {
                                if (mDiagramView.RedoStack.Count == 0)
                                {
                                    mDiagramView.RedoStack.Push(temp1);
                                    mDiagramView.RedoStack.Push(temp2);
                                    break;
                                }
                                Point portpos = new Point();
                                obj = mDiagramView.RedoStack.Pop();
                                if (obj is string)
                                {
                                    mDiagramView.UndoStack.Push(obj);


                                    break;
                                }

                                Point p = (Point)obj;

                                obj = mDiagramView.RedoStack.Pop();
                                Size s = (Size)obj;

                                obj = mDiagramView.RedoStack.Pop();
                                if (obj is Node)
                                {
                                    Node n = obj as Node;
                                    Size redosize = new Size(n.Width, n.Height);
                                    Point redoposition = new Point(n.PxOffsetX, n.PxOffsetY);
                                    object drag = mDiagramView.RedoStack.Pop();
                                    if (drag.ToString() == "Yes")
                                    {
                                        n.PxOffsetX = mDiagramView.ConvertValue(p.X);
                                        n.PxOffsetY = mDiagramView.ConvertValue(p.Y);
                                    }
                                    n.Width = s.Width;
                                    n.Height = s.Height;

                                    int count = n.Ports.Count;
                                    while (count > 0)
                                    {
                                        obj = mDiagramView.RedoStack.Pop();
                                        portpos = (Point)obj;
                                        obj = mDiagramView.RedoStack.Pop();
                                        ConnectionPort port = obj as ConnectionPort;
                                        mDiagramView.UndoStack.Push(port);
                                        mDiagramView.UndoStack.Push(new Point(port.Left, port.Top));
                                        port.PreviousPortPoint = new Point(port.Left, port.Top);
                                        port.Left = portpos.X;
                                        port.Top = portpos.Y;
                                        TranslateTransform tr = new TranslateTransform(port.Left-port.ActualWidth/2, port.Top-port.ActualHeight/2);
                                        port.RenderTransform = tr;
                                        count--;
                                    }
                                    dc.View.IsResizedUndone = true;
                                    dc.View.IsResizedRedone = true;
                                    mDiagramView.UndoStack.Push(drag.ToString());
                                    mDiagramView.UndoStack.Push(n);
                                    mDiagramView.UndoStack.Push(redosize);
                                    mDiagramView.UndoStack.Push(redoposition);
                                    mDiagramView.UndoStack.Push(tempcount);
                                    mDiagramView.UndoStack.Push("Resized");
                                    (mDiagramView.Page as DiagramPage).InvalidateMeasure();
                                    (mDiagramView.Page as DiagramPage).InvalidateArrange();
                                }

                                if (mDiagramView.RedoStack.Count != 0 && itemcount > 1)
                                {
                                    temp1 = mDiagramView.RedoStack.Pop();
                                    temp2 = mDiagramView.RedoStack.Pop();
                                }

                                itemcount--;
                                if (itemcount < 1 || mDiagramView.RedoStack.Count == 0)
                                {
                                    break;
                                }

                            }

                            break;
                        }


                    case "Order":
                        {
                            obj = mDiagramView.RedoStack.Pop();
                            int count = (int)obj;
                            int tempcount = (int)obj;
                            while (count > 0)
                            {
                                obj = mDiagramView.RedoStack.Pop();
                                UIElement element = (UIElement)obj;
                                obj = mDiagramView.RedoStack.Pop();
                                int index = (int)obj;
                                mDiagramView.UndoStack.Push(Panel.GetZIndex(element));
                                mDiagramView.UndoStack.Push(element);
                                Panel.SetZIndex(element, index);
                                count--;
                            }

                            mDiagramView.UndoStack.Push(tempcount);
                            mDiagramView.UndoStack.Push("Order");
                            break;
                        }
                    ////mDiagramView.Redone = false;
                    case "":
                        mDiagramView.RedoStack.Pop();
                        break;

                    case "Visible":
                        obj = mDiagramView.RedoStack.Pop();
                        if (obj is bool)
                        {
                            Layer l = mDiagramView.RedoStack.Pop() as Layer;
                            mDiagramView.UndoStack.Push(l);
                            bool tf = l.Visible;
                            mDiagramView.UndoStack.Push(tf);
                            l.Visible = (bool)obj;
                            mDiagramView.UndoStack.Push("Visible");
                        }
                        break;

                    //case "Active":
                    //    obj = mDiagramView.UndoStack.Pop();
                    //    if (obj is bool)
                    //    {
                    //        Layer l = mDiagramView.UndoStack.Pop() as Layer;
                    //        mDiagramView.RedoStack.Push(l);
                    //        bool tf = l.Active;
                    //        mDiagramView.RedoStack.Push(tf);
                    //        l.Active = (bool)obj;
                    //        mDiagramView.RedoStack.Push("Active");
                    //    }
                    //    break;

                    case "LineRA":
                        break;

                    case "NodeRA":
                        break;

                    case "NodeAdded":
                        obj = mDiagramView.RedoStack.Pop();
                        if (obj is ObservableCollection<Node>)
                        {
                            Layer l = mDiagramView.RedoStack.Pop() as Layer;
                            mDiagramView.UndoStack.Push(l);
                            mDiagramView.UndoStack.Push(obj);
                            mDiagramView.UndoStack.Push("NodeRemoved");
                            foreach (Node n in (obj as ObservableCollection<Node>))
                            {
                                l.Nodes.Remove(n);
                            }
                        }
                        break;

                    case "NodeRemoved":
                        obj = mDiagramView.RedoStack.Pop();
                        if (obj is ObservableCollection<Node>)
                        {
                            Layer l = mDiagramView.RedoStack.Pop() as Layer;
                            mDiagramView.UndoStack.Push(l);
                            mDiagramView.UndoStack.Push(obj);
                            mDiagramView.UndoStack.Push("NodeAdded");
                            foreach (Node n in (obj as ObservableCollection<Node>))
                            {
                                l.Nodes.Add(n);
                            }
                        }
                        break;

                    case "LineAdded":
                        obj = mDiagramView.RedoStack.Pop();
                        if (obj is ObservableCollection<LineConnector>)
                        {
                            Layer l = mDiagramView.RedoStack.Pop() as Layer;
                            mDiagramView.UndoStack.Push(l);
                            mDiagramView.UndoStack.Push(obj);
                            mDiagramView.UndoStack.Push("NodeRemoved");
                            foreach (LineConnector ln in (obj as ObservableCollection<LineConnector>))
                            {
                                l.Lines.Remove(ln);
                            }
                        }
                        break;

                    case "LineRemoved":
                        obj = mDiagramView.RedoStack.Pop();
                        if (obj is ObservableCollection<LineConnector>)
                        {
                            Layer l = mDiagramView.RedoStack.Pop() as Layer;
                            mDiagramView.UndoStack.Push(l);
                            mDiagramView.UndoStack.Push(obj);
                            mDiagramView.UndoStack.Push("NodeAdded");
                            foreach (LineConnector ln in (obj as ObservableCollection<LineConnector>))
                            {
                                l.Lines.Add(ln);
                            }
                        }
                        break;

                    case "LayerAdded":
                        obj = mDiagramView.RedoStack.Pop();
                        if (obj is ObservableCollection<Layer>)
                        {
                            ObservableCollection<Layer> lc = mDiagramView.RedoStack.Pop() as ObservableCollection<Layer>;
                            mDiagramView.UndoStack.Push(lc);
                            mDiagramView.UndoStack.Push(obj);
                            mDiagramView.UndoStack.Push("LayerRemoved");
                            foreach (Layer l in (obj as ObservableCollection<Layer>))
                            {
                                lc.Remove(l);
                            }
                        }
                        break;
                    case "LayerRemoved":
                        obj = mDiagramView.RedoStack.Pop();
                        if (obj is ObservableCollection<Layer>)
                        {
                            ObservableCollection<Layer> lc = mDiagramView.RedoStack.Pop() as ObservableCollection<Layer>;
                            mDiagramView.UndoStack.Push(lc);
                            mDiagramView.UndoStack.Push(obj);
                            mDiagramView.UndoStack.Push("LayerAdded");
                            foreach (Layer l in (obj as ObservableCollection<Layer>))
                            {
                                lc.Add(l);
                            }
                        }
                        break;
                }

                mDiagramView.Redone = false;
            }
        }

        public static void SelectAllCommand(DiagramView mDiagramView)
        {

            if (mDiagramView != null)
            {
              DiagramControl  dc = DiagramPage.GetDiagramControl(mDiagramView) as DiagramControl;
              if (dc.View.IsPageEditable)
              {
                  foreach (Node node in dc.Model.Nodes)
                  {
                      if (node.AllowSelect)
                      {
                          mDiagramView.SelectionList.Add(node);
                      }
                  }
                  foreach (LineConnector line in dc.Model.Connections)
                  {
                      if (line.AllowSelect)
                      {
                          mDiagramView.SelectionList.Add(line);
                      }
                  }
              }
            }
        }


        public static void FitToPageCommand(DiagramView mDiagramView)
        {
            mDiagramView.FitToPage();       
        }
        /// <summary>
        /// Performs Undo operation.
        /// </summary>
        /// <param name="mDiagramView">The DiagramView instance.</param>
        public static void UndoCommand(DiagramView mDiagramView)
        {
            int deletecount = 1;
            int tdeletecount = 1;
            bool deletecommand = false;
            DiagramControl dc;
            if (mDiagramView != null && mDiagramView.IsPageEditable && mDiagramView.UndoStack.Count != 0)
            {
                mDiagramView.Undone = true;
                dc = DiagramPage.GetDiagramControl(mDiagramView) as DiagramControl;
                if (dc != null && dc.View != null && dc.View.UndoRedoEnabled)
                {
                }
                else
                {
                    mDiagramView.UndoStack.Clear();
                    mDiagramView.RedoStack.Clear();
                    return;
                }
                object obj = mDiagramView.UndoStack.Pop();
                if (obj.ToString() == "DeleteCount")
                {
                    deletecount = (int)mDiagramView.UndoStack.Pop();
                    if (deletecount == 0)
                    {
                        deletecount = 1;
                    }

                    tdeletecount = deletecount;
                    obj = mDiagramView.UndoStack.Pop();
                    if (deletecount > 1)
                    {
                        deletecommand = true;
                    }
                }

                switch (obj as string)
                {
                    case "Deleted":
                        {
                            while (deletecount > 0)
                            {
                                obj = mDiagramView.UndoStack.Pop();
                                int zindex = (int)obj;
                                obj = mDiagramView.UndoStack.Pop();
                                if (obj is Node)
                                {
                                    Panel.SetZIndex(obj as UIElement, zindex);
                                    if (obj is Group)
                                    {
                                        foreach (INodeGroup element in (obj as Group).NodeChildren)
                                        {
                                            if (!element.Groups.Contains(obj))
                                            {
                                                element.Groups.Add(obj);
                                            }

                                            element.IsGrouped = true;
                                        }
                                    }

                                    if (!dc.Model.Nodes.Contains(obj))
                                        dc.Model.Nodes.Add(obj as Node);
                                    if ((obj as Node).IsGrouped)
                                    {
                                        foreach (Group g in (obj as Node).Groups)
                                        {
                                            g.NodeChildren.Add(obj as Node);
                                            foreach (ICommon shape in g.NodeChildren)
                                            {
                                                (shape as INodeGroup).IsGrouped = true;
                                            }
                                        }
                                    }

                                    mDiagramView.RedoStack.Push(obj as Node);
                                    mDiagramView.RedoStack.Push("Added");
                                }
                                else if (obj is LineConnector)
                                {
                                    dc.Model.Connections.Add(obj as LineConnector);
                                    Panel.SetZIndex(obj as LineConnector, zindex);
                                    mDiagramView.RedoStack.Push(obj as LineConnector);
                                    mDiagramView.RedoStack.Push("Added");
                                }

                                foreach (UIElement element in mDiagramView.Page.Children.OfType<ICommon>())
                                {
                                    if (element != (obj as UIElement) && (Panel.GetZIndex(element) >= zindex))
                                    {
                                        Panel.SetZIndex(element, (Panel.GetZIndex(element) + 1));
                                    }
                                }

                                if (deletecommand && deletecount != 1)
                                {
                                    mDiagramView.UndoStack.Pop();
                                }

                                deletecount--;
                            }

                            mDiagramView.RedoStack.Push(tdeletecount);
                            mDiagramView.RedoStack.Push("DeleteCount");
                            break;
                        }

                    case "Dragged":
                        {
                            double x = 0, y = 0;
                            int nodecount = 1;
                            int tnodecount = 1;
                            bool exe = false;
                            obj = mDiagramView.UndoStack.Pop();
                            if (obj is int)
                            {
                                nodecount = (int)obj;
                                tnodecount = (int)obj;
                                exe = true;
                            }

                            while (nodecount > 0)
                            {
                                if (exe)
                                {
                                    if (mDiagramView.UndoStack.Count == 0)
                                    {
                                        break;
                                    }
                                    obj = mDiagramView.UndoStack.Pop();
                                    if (obj.ToString() == "Dragged")
                                    {
                                        mDiagramView.UndoStack.Pop();
                                        obj = mDiagramView.UndoStack.Pop();
                                    }
                                }

                                if (obj is Node)
                                {
                                    Node n = obj as Node;
                                    Point pos = new Point(n.PxOffsetX, n.PxOffsetY);
                                    mDiagramView.RedoStack.Push(pos);
                                    mDiagramView.RedoStack.Push(n);
                                    mDiagramView.RedoStack.Push(tnodecount);
                                    mDiagramView.RedoStack.Push("Dragged");

                                    obj = mDiagramView.UndoStack.Pop();
                                    if (obj is Point)
                                    {
                                        Point p = (Point)obj;

                                        n.PxOffsetX = mDiagramView.ConvertValue(p.X);
                                        n.PxOffsetY = mDiagramView.ConvertValue(p.Y);
                                    }
                                    else
                                    {
                                        if (obj.ToString() == "offsetx")
                                        {
                                            obj = mDiagramView.UndoStack.Pop();
                                            x = (double)obj;

                                            n.PxOffsetX = mDiagramView.ConvertValue(x);
                                            List<object> tempstack = mDiagramView.UndoStack.ToList();
                                        }
                                        else
                                        {
                                            obj = mDiagramView.UndoStack.Pop();
                                            y = (double)obj;

                                            n.PxOffsetY = mDiagramView.ConvertValue(y);

                                            List<object> tempstack = mDiagramView.UndoStack.ToList();
                                        }
                                    }
                                    
                                    (mDiagramView.Page as DiagramPage).InvalidateMeasure();
                                    (mDiagramView.Page as DiagramPage).InvalidateArrange();
                                }

                                if (obj is LineConnector)
                                {
                                    Node n = null;
                                    ConnectionPort port = null;
                                    Point p = new Point();
                                    LineConnector l = obj as LineConnector;
                                    obj = mDiagramView.UndoStack.Pop();
                                    string s = (string)obj;
                                    obj = mDiagramView.UndoStack.Pop();

                                    if (obj is Node)
                                    {
                                        n = obj as Node;
                                    }
                                    else if (obj is Point)
                                    {
                                        p = (Point)obj;
                                    }
                                    else if (obj is ConnectionPort)
                                    {
                                        port = obj as ConnectionPort;
                                        n = port.Node;
                                    }

                                    if (s == "Headend" )
                                    {
                                        if (l.HeadNode != null)
                                        {
                                            mDiagramView.RedoStack.Push(l.HeadNode);
                                        }
                                        else
                                        {
                                            Point sp = new Point(l.PxStartPointPosition.X, l.PxStartPointPosition.Y);
                                            mDiagramView.RedoStack.Push(sp);
                                        }

                                        mDiagramView.RedoStack.Push("Headend");
                                        mDiagramView.RedoStack.Push(l);
                                        mDiagramView.RedoStack.Push("Dragged");

                                        if (n != null)
                                        {
                                            l.HeadNode = n;
                                            if (port != null)
                                            {
                                                l.ConnectionHeadPort = port;
                                            }
                                        }
                                        else
                                        {
                                            l.PxStartPointPosition = new Point(mDiagramView.ConvertValue(p.X), mDiagramView.ConvertValue(p.Y));
                                            if (l.IntermediatePoints != null && l.IntermediatePoints.Count >= 2 && l.ConnectorType == ConnectorType.Orthogonal)
                                            {
                                                if (l.IntermediatePoints[0].X == l.IntermediatePoints[1].X)
                                                {
                                                    l.IntermediatePoints[0] = new Point(l.IntermediatePoints[0].X, l.PxStartPointPosition.Y);
                                                }
                                                else
                                                {
                                                    l.IntermediatePoints[0] = new Point(l.PxStartPointPosition.X, l.IntermediatePoints[0].Y);
                                                }
                                            }
                                            l.HeadNode = null;
                                        }
                                    }
                                    else if (s == "Tailend")
                                    {
                                        if (l.TailNode != null)
                                        {
                                            mDiagramView.RedoStack.Push(l.TailNode);
                                        }
                                        else
                                        {
                                            Point ep = new Point(l.PxEndPointPosition.X, l.PxEndPointPosition.Y);
                                            mDiagramView.RedoStack.Push(ep);
                                        }

                                        mDiagramView.RedoStack.Push("Tailend");
                                        mDiagramView.RedoStack.Push(l);
                                        mDiagramView.RedoStack.Push("Dragged");

                                        if (n != null)
                                        {
                                            l.TailNode = n;
                                            if (port != null)
                                            {
                                                l.ConnectionTailPort = port;
                                            }
                                        }
                                        else
                                        {
                                            l.TailNode = null;
                                            l.PxEndPointPosition = new Point(mDiagramView.ConvertValue(p.X), mDiagramView.ConvertValue(p.Y));
                                            if (l.IntermediatePoints != null && l.IntermediatePoints.Count >= 2 && l.ConnectorType == ConnectorType.Orthogonal)
                                            {
                                                int finalPt = l.IntermediatePoints.Count - 1;
                                                if (l.IntermediatePoints[finalPt].X == l.IntermediatePoints[finalPt - 1].X)
                                                {
                                                    l.IntermediatePoints[finalPt] = new Point(l.IntermediatePoints[0].X, l.PxEndPointPosition.Y);
                                                }
                                                else
                                                {
                                                    l.IntermediatePoints[finalPt] = new Point(l.PxEndPointPosition.X, l.IntermediatePoints[0].Y);
                                                }
                                            }
                                        }
                                    }
                                    else if (s == "LineMoved")
                                    {
                                        if (l.TailNode != null)
                                        {
                                            mDiagramView.RedoStack.Push(l.TailNode);
                                        }
                                        else
                                        {
                                            Point sp = new Point(l.PxEndPointPosition.X, l.PxEndPointPosition.Y);
                                            mDiagramView.RedoStack.Push(sp);
                                        }

                                        if (l.HeadNode != null)
                                        {
                                            mDiagramView.RedoStack.Push(l.HeadNode);
                                        }
                                        else
                                        {
                                            Point sp = new Point(l.PxStartPointPosition.X, l.PxStartPointPosition.Y);
                                            mDiagramView.RedoStack.Push(sp);
                                        }
                                        mDiagramView.RedoStack.Push("LineMoved");
                                        mDiagramView.RedoStack.Push(l);
                                        mDiagramView.RedoStack.Push("Dragged");
                                        l.PxEndPointPosition = new Point(mDiagramView.ConvertValue(p.X), mDiagramView.ConvertValue(p.Y));
                                        obj = mDiagramView.UndoStack.Pop();
                                        if (obj is Point)
                                        {
                                            p = (Point)obj;
                                        }
                                        l.StartPointPosition = new Point(mDiagramView.ConvertValue(p.X), mDiagramView.ConvertValue(p.Y));
                                    }
                                    else if (s == "Vertex")
                                    {
                                        int i = (int)obj;
                                        while (i > 0)
                                        {
                                            int index = (int)mDiagramView.UndoStack.Pop();
                                            Point tempPt = l.IntermediatePoints[index];
                                            mDiagramView.RedoStack.Push(l.IntermediatePoints[index]);
                                            mDiagramView.RedoStack.Push(index);
                                            l.IntermediatePoints[index] = (Point)mDiagramView.UndoStack.Pop();
                                            i--;
                                        }
                                        mDiagramView.RedoStack.Push((int)obj);
                                        mDiagramView.RedoStack.Push("Vertex");
                                        mDiagramView.RedoStack.Push(l);
                                        mDiagramView.RedoStack.Push("Dragged");
                                    }

                                    l.InvalidateConnectorPathGeometry();
                                }

                                nodecount--;
                            }

                            break;
                        }

                    case "Added":
                        {
                            obj = mDiagramView.UndoStack.Pop();
                            int addCount = 1;
                            int taddCount;
                            bool multiple = false;
                            if (obj is int)
                            {
                                addCount = (int)obj;
                                taddCount = addCount;
                                multiple = true;
                            }
                            taddCount = addCount;
                            while (addCount > 0)
                            {
                                if (multiple)
                                {
                                    obj = mDiagramView.UndoStack.Pop();
                                    obj = mDiagramView.UndoStack.Pop();
                                }
                                addCount--;
                                if (obj is Node)
                                {
                                    if ((obj as Node).IsGrouped)
                                    {
                                        foreach (Group g in (obj as Node).Groups)
                                        {
                                            g.NodeChildren.Remove(obj as Node);
                                        }
                                    }

                                    if (obj is Group)
                                    {
                                        (obj as Group).NodeChildren.Remove(obj);
                                    }

                                    dc.Model.Nodes.Remove(obj as Node);
                                    mDiagramView.RedoStack.Push(obj as Node);
                                    mDiagramView.RedoStack.Push(Panel.GetZIndex(obj as Node));
                                    mDiagramView.RedoStack.Push("Deleted");
                                }
                                else if (obj is LineConnector)
                                {
                                    dc.Model.Connections.Remove(obj as LineConnector);
                                    mDiagramView.SelectionList.Remove(obj as LineConnector);
                                    mDiagramView.RedoStack.Push(obj as LineConnector);
                                    mDiagramView.RedoStack.Push(Panel.GetZIndex(obj as LineConnector));
                                    mDiagramView.RedoStack.Push("Deleted");
                                }
                                else if (obj is Point)
                                {
                                    Point p = (Point)obj;
                                    LineConnector l = (LineConnector)mDiagramView.UndoStack.Pop();
                                    int index = (int)mDiagramView.UndoStack.Pop();
                                    mDiagramView.RedoStack.Push(index);
                                    mDiagramView.RedoStack.Push(l);
                                    mDiagramView.RedoStack.Push(p);
                                    mDiagramView.RedoStack.Push("Deleted");
                                    if (l.ConnectorType == ConnectorType.Orthogonal)
                                    {
                                        l.IntermediatePoints.RemoveAt(index);
                                        l.IntermediatePoints.RemoveAt(index);
                                    }
                                    if (l.ConnectorType == ConnectorType.Straight)
                                    {
                                        l.IntermediatePoints.RemoveAt(index);
                                    }
                                    l.InvalidateConnectorPathGeometry();
                                }
                            }
                            if (multiple)
                            {
                                mDiagramView.RedoStack.Push(taddCount);
                                mDiagramView.RedoStack.Push(multiple);
                                mDiagramView.RedoStack.Push("Deleted");
                            }
                            break;
                        }

                    case "CSDelete":
                        LineConnector li = (LineConnector)mDiagramView.UndoStack.Pop();
                        if (li.ConnectorType == ConnectorType.Orthogonal)
                        {
                            int t = (int)mDiagramView.UndoStack.Pop();
                            Point pt = li.IntermediatePoints[t];
                            li.IntermediatePoints[t] = (Point)mDiagramView.UndoStack.Pop();

                            {
                                li.IntermediatePoints.Insert(t + 1, (Point)mDiagramView.UndoStack.Pop());
                                li.IntermediatePoints.Insert(t + 2, (Point)mDiagramView.UndoStack.Pop());
                            }

                            mDiagramView.RedoStack.Push(li.IntermediatePoints[t + 2]);
                            mDiagramView.RedoStack.Push(li.IntermediatePoints[t + 1]);
                            mDiagramView.RedoStack.Push(pt);
                            mDiagramView.RedoStack.Push(t);
                            mDiagramView.RedoStack.Push(li);
                            mDiagramView.RedoStack.Push("CSAdd");
                            li.InvalidateConnectorPathGeometry();
                        }
                        else if (li.ConnectorType == ConnectorType.Straight)
                        {

                            int t = (int)mDiagramView.UndoStack.Pop();
                            Point pt = (Point)mDiagramView.UndoStack.Pop();

                            li.IntermediatePoints.Insert(t, pt); ;
                            mDiagramView.RedoStack.Push(pt);
                            mDiagramView.RedoStack.Push(t);
                            mDiagramView.RedoStack.Push(li);
                            mDiagramView.RedoStack.Push("CSAdd");
                            li.InvalidateConnectorPathGeometry();
                        }
                        break;
                    case "Resized":
                        {
                            obj = mDiagramView.UndoStack.Pop();
                            int itemcount = (int)obj;
                            int tempcount = (int)obj;

                            while (itemcount >= 0)
                            {
                                if (mDiagramView.UndoStack.Count == 0)
                                {
                                    break;
                                }
                                Point portpos = new Point();
                                obj = mDiagramView.UndoStack.Pop();
                                if (obj is string)
                                {
                                    mDiagramView.UndoStack.Push(obj);


                                    break;
                                }
                                Point p = (Point)obj;

                                obj = mDiagramView.UndoStack.Pop();
                                Size s = (Size)obj;

                                obj = mDiagramView.UndoStack.Pop();
                                if (obj is Node)
                                {
                                    Node n = obj as Node;
                                    n.tempcount = tempcount;
                                    dc.View.undostak.Add((obj as Node).Name);
                                    Size redosize = new Size(n.Width, n.Height);
                                    object drag = mDiagramView.UndoStack.Pop();
                                    Point redoposition = new Point(n.PxOffsetX, n.PxOffsetY);
                                    if(drag.ToString()=="Yes")
                                    {
                                    n.PxOffsetX = mDiagramView.ConvertValue(p.X);
                                    n.PxOffsetY = mDiagramView.ConvertValue(p.Y);
                                    }
                                    else if (drag is Point)
                                    {
                                        mDiagramView.UndoStack.Push(drag);
                                    }
                                    n.Width = s.Width;
                                    n.Height = s.Height;
                                    mDiagramView.IsResizedUndone = true;
                                    n.onceResized = true;
                                    int count = n.Ports.Count;
                                    while (count > 0)
                                    {

                                        if (mDiagramView.UndoStack.Count == 0)
                                        {
                                            break;

                                        }

                                        obj = mDiagramView.UndoStack.Pop();
                                        if (obj is string)
                                        {
                                            mDiagramView.UndoStack.Push(obj);
                                            break;
                                        }
                                        portpos = (Point)obj;
                                        obj = mDiagramView.UndoStack.Pop();
                                        ConnectionPort port = obj as ConnectionPort;
                                        mDiagramView.RedoStack.Push(port);
                                        mDiagramView.RedoStack.Push(new Point(port.Left, port.Top));
                                        port.Left = portpos.X;
                                        port.Top = portpos.Y;
                                        TranslateTransform tr = new TranslateTransform(port.Left-port.ActualWidth/2, port.Top-port.ActualHeight/2);
                                        port.RenderTransform = tr;
                                        count--;

                                    }
                                    mDiagramView.RedoStack.Push(drag.ToString());
                                    mDiagramView.RedoStack.Push(n);
                                    mDiagramView.RedoStack.Push(redosize);
                                    mDiagramView.RedoStack.Push(redoposition);
                                    mDiagramView.RedoStack.Push(tempcount);
                                    mDiagramView.RedoStack.Push("Resized");
                                    (mDiagramView.Page as DiagramPage).InvalidateMeasure();
                                    (mDiagramView.Page as DiagramPage).InvalidateArrange();
                                }


                                if (mDiagramView.UndoStack.Count != 0)
                                {
                                    obj = mDiagramView.UndoStack.Peek();
                                }

                               
                                if (obj.ToString() == "Resized" && itemcount > 1)
                                {
                                    mDiagramView.UndoStack.Pop();
                                    mDiagramView.UndoStack.Pop();
                                }

                                itemcount--;
                                if (itemcount < 1)
                                {
                                    break;
                                }

                            }
                            break;
                        }


                    case "Rotated":
                        {
                            obj = mDiagramView.UndoStack.Pop();
                            int count = (int)obj;
                            int tempcount = (int)obj;
                            mDiagramView.NodeRotateCount = count;

                            while (count > 0)
                            {
                                obj = mDiagramView.UndoStack.Pop();
                                double angle = (double)obj;
                                obj = mDiagramView.UndoStack.Pop();
                                if (obj is Node)
                                {
                                    Node n = obj as Node;
                                    mDiagramView.RedoStack.Push(n);
                                    mDiagramView.RedoStack.Push(n.RotateAngle);
                                    mDiagramView.RedoStack.Push(tempcount);
                                    mDiagramView.RedoStack.Push("Rotated");
                                    n.RotateAngle = angle;
                                    RotateTransform rotateTransform = n.RenderTransform as RotateTransform;
                                    rotateTransform.Angle = angle;
                                    foreach (LineConnector l in dc.Model.Connections)
                                    {
                                        if (l.HeadNode == n || l.TailNode == n)
                                        {
                                            l.InvalidateConnectorPathGeometry();
                                        }
                                    }
                                    if (count != 1)
                                    {
                                        mDiagramView.UndoStack.Pop();
                                        mDiagramView.UndoStack.Pop();
                                    }

                                    count--;
                                }
                            }

                            (mDiagramView.Page as DiagramPage).InvalidateMeasure();
                            (mDiagramView.Page as DiagramPage).InvalidateArrange();
                            break;
                        }

                    case "Order":
                        {
                            obj = mDiagramView.UndoStack.Pop();
                            int count = (int)obj;
                            int tempcount = (int)obj;
                            while (count > 0)
                            {
                                obj = mDiagramView.UndoStack.Pop();
                                UIElement element = (UIElement)obj;
                                obj = mDiagramView.UndoStack.Pop();
                                int index = (int)obj;
                                mDiagramView.RedoStack.Push(Panel.GetZIndex(element));
                                mDiagramView.RedoStack.Push(element);
                                Panel.SetZIndex(element, index);
                                count--;
                            }

                            mDiagramView.RedoStack.Push(tempcount);
                            mDiagramView.RedoStack.Push("Order");
                            break;
                        }

                    case "":
                        mDiagramView.UndoStack.Pop();
                        break;

                    case "Visible":
                        obj = mDiagramView.UndoStack.Pop();
                        if (obj is bool)
                        {
                            Layer l = mDiagramView.UndoStack.Pop() as Layer;
                            mDiagramView.RedoStack.Push(l);
                            bool tf = l.Visible;
                            mDiagramView.RedoStack.Push(tf);
                            l.Visible = (bool)obj;
                            mDiagramView.RedoStack.Push("Visible");
                        }
                        break;

                    //case "Active":
                    //    obj = mDiagramView.UndoStack.Pop();
                    //    if (obj is bool)
                    //    {
                    //        Layer l = mDiagramView.UndoStack.Pop() as Layer;
                    //        mDiagramView.RedoStack.Push(l);
                    //        bool tf = l.Active;
                    //        mDiagramView.RedoStack.Push(tf);
                    //        l.Active = (bool)obj;
                    //        mDiagramView.RedoStack.Push("Active");
                    //    }
                    //    break;

                    case "LineRA":
                        break;

                    case "NodeRA":
                        break;

                    case "NodeAdded":
                        obj = mDiagramView.UndoStack.Pop();
                        if (obj is ObservableCollection<Node>)
                        {
                            Layer l = mDiagramView.UndoStack.Pop() as Layer;
                            mDiagramView.RedoStack.Push(l);
                            mDiagramView.RedoStack.Push(obj);
                            mDiagramView.RedoStack.Push("NodeRemoved");
                            foreach (Node n in (obj as ObservableCollection<Node>))
                            {
                                l.Nodes.Remove(n);
                            }
                        }
                        break;

                    case "NodeRemoved":
                        obj = mDiagramView.UndoStack.Pop();
                        if (obj is ObservableCollection<Node>)
                        {
                            Layer l = mDiagramView.UndoStack.Pop() as Layer;
                            mDiagramView.RedoStack.Push(l);
                            mDiagramView.RedoStack.Push(obj);
                            mDiagramView.RedoStack.Push("NodeAdded");
                            foreach (Node n in (obj as ObservableCollection<Node>))
                            {
                                l.Nodes.Add(n);
                            }
                        }
                        break;

                    case "LineAdded":
                        obj = mDiagramView.UndoStack.Pop();
                        if (obj is ObservableCollection<LineConnector>)
                        {
                            Layer l = mDiagramView.UndoStack.Pop() as Layer;
                            mDiagramView.RedoStack.Push(l);
                            mDiagramView.RedoStack.Push(obj);
                            mDiagramView.RedoStack.Push("NodeRemoved");
                            foreach (LineConnector ln in (obj as ObservableCollection<LineConnector>))
                            {
                                l.Lines.Remove(ln);
                            }
                        }
                        break;

                    case "LineRemoved":
                        obj = mDiagramView.UndoStack.Pop();
                        if (obj is ObservableCollection<LineConnector>)
                        {
                            Layer l = mDiagramView.UndoStack.Pop() as Layer;
                            mDiagramView.RedoStack.Push(l);
                            mDiagramView.RedoStack.Push(obj);
                            mDiagramView.RedoStack.Push("NodeAdded");
                            foreach (LineConnector ln in (obj as ObservableCollection<LineConnector>))
                            {
                                l.Lines.Add(ln);
                            }
                        }
                        break;

                    case "LayerAdded":
                        obj = mDiagramView.UndoStack.Pop();
                        if (obj is ObservableCollection<Layer>)
                        {
                            ObservableCollection<Layer> lc = mDiagramView.UndoStack.Pop() as ObservableCollection<Layer>;
                            mDiagramView.RedoStack.Push(lc);
                            mDiagramView.RedoStack.Push(obj);
                            mDiagramView.RedoStack.Push("LayerRemoved");
                            foreach (Layer l in (obj as ObservableCollection<Layer>))
                            {
                                lc.Remove(l);
                            }
                        }
                        break;
                    case "LayerRemoved":
                        obj = mDiagramView.UndoStack.Pop();
                        if (obj is ObservableCollection<Layer>)
                        {
                            ObservableCollection<Layer> lc = mDiagramView.UndoStack.Pop() as ObservableCollection<Layer>;
                            mDiagramView.RedoStack.Push(lc);
                            mDiagramView.RedoStack.Push(obj);
                            mDiagramView.RedoStack.Push("LayerAdded");
                            foreach (Layer l in (obj as ObservableCollection<Layer>))
                            {
                                lc.Add(l);
                            }
                        }
                        break;
                }

                mDiagramView.Undone = false;
            }
        }

        #endregion

        #region Command Handlers


        #region Command Handlers for SelectAll
        private static void SelectAllCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private static void SelectAllExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            DiagramView mDiagramView = sender as DiagramView;
            DiagramCommandManager.SelectAllCommand(mDiagramView);
        }

        private static void FitToPageCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private static void FitToPageExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            DiagramView mDiagramView = sender as DiagramView;
            DiagramCommandManager.FitToPageCommand(mDiagramView);
        }

        #endregion Command Handlers
        
        /// <summary>
        /// Determines when the Cut Commands are to be executed.
        /// </summary>
        /// <param name="sender">The DiagramView.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        public static void CanExecuteCutCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            DiagramView mDiagramView = sender as DiagramView;
            if (mDiagramView.IsPageEditable && mDiagramView.IsCutEnabled)
            {
                if (mDiagramView.SelectionList.Count > 0)
                {
                    e.CanExecute = true;
                }
            }
        }

        /// <summary>
        /// Determines when the Copy Commands are to be executed.
        /// </summary>
        /// <param name="sender">The DiagramView.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        public static void CanExecuteCopyCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            DiagramView mDiagramView = sender as DiagramView;
            if (mDiagramView.IsPageEditable && mDiagramView.IsCopyEnabled)
            {
                if (mDiagramView.SelectionList.Count > 0)
                {
                    e.CanExecute = true;
                }
            }
        }

        /// <summary>
        /// Determines when the Paste Commands are to be executed.
        /// </summary>
        /// <param name="sender">The DiagramView.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        public static void CanExecutePasteCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            DiagramView mDiagramView = sender as DiagramView;
            if (mDiagramView.IsPageEditable && mDiagramView.IsPasteEnabled)
            {
                if (CopyPasteManager.IsValidClipboardContent)
                {
                    e.CanExecute = true;
                }
            }
        }

        /// <summary>
        /// Determines when the Alignment Commands are to be executed.
        /// </summary>
        /// <param name="sender">The DiagramView.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        public static void CanExecuteAlignmentCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            DiagramView mDiagramView = sender as DiagramView;
            if (mDiagramView.IsPageEditable)
            {
                e.CanExecute = true;
            }
        }

        /// <summary>
        /// Specifies when the Delete Command is to be executed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        public static void CanExecuteDeleteCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        /// <summary>
        /// Specifies when the Group Command is to be executed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        public static void CanExecuteGroupCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        /// <summary>
        /// Specifies when the Nudge Commands are to be executed.
        /// </summary>
        /// <param name="sender">object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        public static void CanExecuteNudgeCommands(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        /// <summary>
        /// Specifies whether the Order Commands can be executed.
        /// </summary>
        /// <param name="sender">object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        public static void CanExecuteOrderCommands(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        /// <summary>
        /// Determines whether the Space Commands can be executed
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        public static void CanExecuteSpaceCommands(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        /// <summary>
        /// Determines whether the Undo Command can be executed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        public static void CanExecuteUndoCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        /// <summary>
        /// Determines whether the Redo Command can be executed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        public static void CanExecuteRedoCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        /// <summary>
        /// Invoked when the Delete Command is executed.
        /// </summary>
        /// <param name="sender">The diagram view.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        public static void OnDeleteCommand(object sender, ExecutedRoutedEventArgs e)
        {
            DiagramCommandManager.DeleteObjects(sender as DiagramView);
        }

        #region Group Commands

        /// <summary>
        /// Invoked when the Group Command is executed.
        /// </summary>
        /// <param name="sender">object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        public static void OnGroupCommand(object sender, ExecutedRoutedEventArgs e)
        {
            DiagramView view = sender as DiagramView;
            DiagramControl dc = DiagramPage.GetDiagramControl(view);
           
            if (view.SelectionList.Count > 1)
            {
                Group g = new Group();
                foreach (INodeGroup shape in view.SelectionList)
                {
                    if (shape is LineConnector)
                    {
                        //if ((shape as LineConnector).HeadNode == null && (shape as LineConnector).TailNode == null)
                        //{
                            GroupEventArgs newEventArgs = new GroupEventArgs(g, null, shape as LineConnector);
                            newEventArgs.RoutedEvent = DiagramView.GroupingEvent;
                            dc.View.RaiseEvent(newEventArgs);
                            g.AddChild(shape);

                        //}

                        continue;
                    }
                    if (shape is Node)
                    {
                        GroupEventArgs newEventArgs = new GroupEventArgs(g, shape as Node,null);
                        newEventArgs.RoutedEvent = DiagramView.GroupingEvent;
                        dc.View.RaiseEvent(newEventArgs);
                    }
                                           
                    g.AddChild(shape);

                }

                if (string.IsNullOrEmpty(g.Name))
                {
                    g.Name = "sync_dgm_group" + i;
                    i++;
                }
                GroupEventArgs newEventArgs1 = new GroupEventArgs(g, null,null);
                newEventArgs1.RoutedEvent = DiagramView.GroupedEvent;
                dc.View.RaiseEvent(newEventArgs1);



                if (!dc.Model.Nodes.Contains(g))
                    dc.Model.Nodes.Add(g);
                dc.View.SelectionList.Clear();
                dc.View.SelectionList.Add(g);
            }
        }

        /// <summary>
        /// Invoked when the Ungroup Command is executed.
        /// </summary>
        /// <param name="sender">object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        public static void OnUngroupCommand(object sender, ExecutedRoutedEventArgs e)
        {
            DiagramView view = sender as DiagramView;
            DiagramControl dc = DiagramPage.GetDiagramControl(view);
            IsUnGroupCommand = true;
            foreach (ICommon shape in view.SelectionList)
            {
              
                if (shape is Group)
                {
                   
                    foreach (INodeGroup element in (shape as Group).NodeChildren)
                    {
                        if (element is Node)
                        {
                            UnGroupEventArgs newEventArgs = new UnGroupEventArgs(shape as Group, element as Node, null);
                            newEventArgs.RoutedEvent = DiagramView.UngroupingEvent;
                            dc.View.RaiseEvent(newEventArgs);
                        }
                        else if (element is LineConnector)
                        {
                            UnGroupEventArgs newEventArgs = new UnGroupEventArgs(shape as Group, null, element as LineConnector);
                            newEventArgs.RoutedEvent = DiagramView.UngroupingEvent;
                            dc.View.RaiseEvent(newEventArgs);
                        }
                        if (element.Groups.Count == 0)
                        {
                            element.IsGrouped = false;
                        }
                    }
                    UnGroupEventArgs newEventArgs1 = new UnGroupEventArgs(shape as Group, null,null);
                    newEventArgs1.RoutedEvent = DiagramView.UngroupedEvent;
                    dc.View.RaiseEvent(newEventArgs1);
                    dc.Model.Nodes.Remove(shape);
                    (shape as Group).NodeChildren.Clear();
                    CollectionExt.Cleared = false;
                   
                }
            }

            dc.View.SelectionList.Clear();
        }

        #endregion

        #region Nudge commands

        /// <summary>
        /// Invoked when the MoveUp Command is executed.
        /// </summary>
        /// <param name="sender">object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        public static void OnMoveUpCommand(object sender, ExecutedRoutedEventArgs e)
        {
            DiagramView.MoveUp(sender as DiagramView);
        }

        /// <summary>
        /// Invoked when the MoveDown Command is executed.
        /// </summary>
        /// <param name="sender">object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        public static void OnMoveDownCommand(object sender, ExecutedRoutedEventArgs e)
        {
            DiagramView.MoveDown(sender as DiagramView);
        }

        /// <summary>
        /// Invoked when the MoveLeft Command is executed.
        /// </summary>
        /// <param name="sender">object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        public static void OnMoveLeftCommand(object sender, ExecutedRoutedEventArgs e)
        {
            DiagramView.MoveLeft(sender as DiagramView);
        }

        /// <summary>
        /// Invoked when the MoveRight Command is executed.
        /// </summary>
        /// <param name="sender">object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        public static void OnMoveRightCommand(object sender, ExecutedRoutedEventArgs e)
        {
            DiagramView.MoveRight(sender as DiagramView);
        }

        #endregion

        #region Order commands

        /// <summary>
        /// Invoked when the BringToFront Command is executed.
        /// </summary>
        /// <param name="sender">object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        public static void OnBringToFrontCommand(object sender, ExecutedRoutedEventArgs e)
        {
            DiagramView view = sender as DiagramView;
            if (view.SelectionList.Count > 0)
            {
                if (view.UndoRedoEnabled)
                {
                    foreach (UIElement element in view.Page.Children.OfType<ICommon>())
                    {
                        view.UndoStack.Push(Panel.GetZIndex(element));
                        view.UndoStack.Push(element);
                    }

                    view.UndoStack.Push(view.Page.Children.OfType<ICommon>().Count<ICommon>());
                    view.UndoStack.Push("Order");
                }
                List<UIElement> ordered = (from UIElement item in view.Page.Children.OfType<ICommon>()
                                           orderby Panel.GetZIndex(item as UIElement)
                                           select item as UIElement).ToList();

                if (view.SelectionList.Count == 1)
                {
                    foreach (ICommon shape in view.SelectionList)
                    {
                        int oldindex = Panel.GetZIndex(shape as UIElement);
                        Panel.SetZIndex(shape as UIElement, view.Page.Children.Count);
                        int newindex = Panel.GetZIndex(shape as UIElement);

                        foreach (UIElement element in view.Page.Children)
                        {
                            if (!view.SelectionList.Contains(element) && Panel.GetZIndex(element) > oldindex)
                            {
                                Panel.SetZIndex(element, Panel.GetZIndex(element) - 1);
                            }
                        }
                    }
                }
                else
                {
                    int childcount = view.Page.Children.OfType<ICommon>().Count<ICommon>();
                    int initialindexvalue = childcount - view.SelectionList.Count;
                    List<UIElement> selectionordered = (from UIElement item in view.SelectionList
                                                        orderby Panel.GetZIndex(item as UIElement)
                                                        select item as UIElement).ToList();
                    for (int i = 0; i < view.SelectionList.Count; i++)
                    {
                        (selectionordered[i] as ICommon).OldZIndex = Panel.GetZIndex((selectionordered[i] as ICommon) as UIElement);
                        Panel.SetZIndex(selectionordered[i], initialindexvalue++);
                        (selectionordered[i] as ICommon).NewZIndex = Panel.GetZIndex((selectionordered[i] as ICommon) as UIElement);
                    }

                    for (int i = 0; i < view.SelectionList.Count - 1; i++)
                    {
                        foreach (UIElement element in view.Page.Children.OfType<ICommon>())
                        {
                            if (!selectionordered.Contains(element))
                            {
                                int g = Panel.GetZIndex(element);
                                if (Panel.GetZIndex(element) > (selectionordered[i] as ICommon).OldZIndex && Panel.GetZIndex(element) < (selectionordered[i + 1] as ICommon).OldZIndex)
                                {
                                    Panel.SetZIndex(element, Panel.GetZIndex(element) - (i + 1));
                                }

                                int h = Panel.GetZIndex(element);
                            }
                        }
                    }

                    foreach (UIElement element in view.Page.Children.OfType<ICommon>())
                    {
                        if (!selectionordered.Contains(element))
                        {
                            if (Panel.GetZIndex(element) > (selectionordered[view.SelectionList.Count - 1] as ICommon).OldZIndex)
                            {
                                Panel.SetZIndex(element, Panel.GetZIndex(element) - view.SelectionList.Count);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Invoked when the SendToBack Command is executed.
        /// </summary>
        /// <param name="sender">object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        public static void OnSendToBackCommand(object sender, ExecutedRoutedEventArgs e)
        {
            DiagramView view = sender as DiagramView;
            if (view.SelectionList.Count > 0)
            {
                if (view.UndoRedoEnabled)
                {
                    foreach (UIElement element in view.Page.Children.OfType<ICommon>())
                    {
                        view.UndoStack.Push(Panel.GetZIndex(element));
                        view.UndoStack.Push(element);
                    }

                    view.UndoStack.Push(view.Page.Children.OfType<ICommon>().Count<ICommon>());
                    view.UndoStack.Push("Order");
                }

                List<UIElement> ordered = (from UIElement item in view.Page.Children.OfType<ICommon>()
                                           orderby Panel.GetZIndex(item as UIElement)
                                           select item as UIElement).ToList();

                if (view.SelectionList.Count == 1)
                {
                    foreach (ICommon shape in view.SelectionList)
                    {
                        int oldindex = Panel.GetZIndex(shape as UIElement);
                        Panel.SetZIndex(shape as UIElement, 1);
                        int newindex = Panel.GetZIndex(shape as UIElement);

                        foreach (UIElement element in view.Page.Children.OfType<ICommon>())
                        {
                            if (!view.SelectionList.Contains(element) && Panel.GetZIndex(element) < oldindex)
                            {
                                Panel.SetZIndex(element, Panel.GetZIndex(element) + 1);
                            }
                        }
                    }
                }
                else
                {
                    int childcount = view.Page.Children.OfType<ICommon>().Count<ICommon>();
                    int initialindexvalue = 0;
                    List<UIElement> selectionordered = (from UIElement item in view.SelectionList
                                                        orderby Panel.GetZIndex(item as UIElement)
                                                        select item as UIElement).ToList();
                    for (int i = 0; i < view.SelectionList.Count; i++)
                    {
                        (selectionordered[i] as ICommon).OldZIndex = Panel.GetZIndex((selectionordered[i] as ICommon) as UIElement);
                        Panel.SetZIndex(selectionordered[i], initialindexvalue++);
                        (selectionordered[i] as ICommon).NewZIndex = Panel.GetZIndex((selectionordered[i] as ICommon) as UIElement);
                    }

                    for (int i = 0; i < view.SelectionList.Count - 1; i++)
                    {
                        foreach (UIElement element in view.Page.Children.OfType<ICommon>())
                        {
                            if (!selectionordered.Contains(element))
                            {
                                int g = Panel.GetZIndex(element);
                                if (Panel.GetZIndex(element) < (selectionordered[0] as ICommon).OldZIndex)
                                {
                                    Panel.SetZIndex(element, Panel.GetZIndex(element) + view.SelectionList.Count);
                                }
                                else
                                    if (Panel.GetZIndex(element) > (selectionordered[i] as ICommon).OldZIndex && Panel.GetZIndex(element) < (selectionordered[i + 1] as ICommon).OldZIndex)
                                    {
                                        Panel.SetZIndex(element, Panel.GetZIndex(element) + (view.SelectionList.Count - 1));
                                    }

                                int h = Panel.GetZIndex(element);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Invoked when the MoveForward Command is executed.
        /// </summary>
        
        private static Point[] GetIntersectionPoints(Geometry g1, Geometry g2)
        {
            Geometry og1 = g1.GetWidenedPathGeometry(new Pen(Brushes.Black, 1.0));
            Geometry og2 = g2.GetWidenedPathGeometry(new Pen(Brushes.Black, 1.0));

            CombinedGeometry cg = new CombinedGeometry(GeometryCombineMode.Intersect, og1, og2);

            PathGeometry pg = cg.GetFlattenedPathGeometry();
            Point[] result = new Point[pg.Figures.Count];
            for (int i = 0; i < pg.Figures.Count; i++)
            {
                Rect fig = new PathGeometry(new PathFigure[] { pg.Figures[i] }).Bounds;
                result[i] = new Point(fig.Left + fig.Width / 2.0, fig.Top + fig.Height / 2.0);
            }
            return result;
        }

        private static Path path, path1;       
        private static Rect rect, rect1;
        public static void OnMoveForwardCommand(object sender, ExecutedRoutedEventArgs e)
        {
            DiagramView view = sender as DiagramView;
            if (view.SelectionList.Count > 0)
            {
                if (view.UndoRedoEnabled)
                {
                    foreach (UIElement element in view.Page.Children.OfType<ICommon>())
                    {
                        view.UndoStack.Push(Panel.GetZIndex(element));
                        view.UndoStack.Push(element);
                    }

                    view.UndoStack.Push(view.Page.Children.OfType<ICommon>().Count<ICommon>());
                    view.UndoStack.Push("Order");
                }

                List<UIElement> ordered = (from UIElement item in view.Page.Children.OfType<ICommon>()
                                           orderby Panel.GetZIndex(item as UIElement)
                                           select item as UIElement).ToList();
                bool mTemp = false;
                if (view.SelectionList.Count == 1)
                {
                    foreach (UIElement shape in view.SelectionList)
                    {
                        int oldindex = Panel.GetZIndex(shape as UIElement);
                        if (view.ZOrderMode == ZOrderModes.Visual)
                        {
                            foreach (UIElement element in view.Page.Children.OfType<ICommon>())
                            {
                                for (int newindex = oldindex + 1; newindex <= view.Page.Children.Count; newindex++)
                                {
                                    if (Panel.GetZIndex(element) == newindex)
                                    {
                                        Point[] pts = new Point[0];
                                        if (shape is Node)
                                        {                                            
                                            rect = new Rect((shape as Node).PxOffsetX, (shape as Node).PxOffsetY, (shape as Node).PxWidth, (shape as Node).PxHeight);
                                            path = new Path();
                                            path.Data = new RectangleGeometry(rect);                                                                                   
                                        }
                                        else if (shape is LineConnector)
                                        {
                                            var c = VisualTreeHelper.GetChild(shape as LineConnector, 0) as Canvas;
                                            if (c != null)
                                            {
                                                path = c.FindName("PART_ConnectionPath") as Path;                                                
                                            }                                            
                                        }

                                        if (element is Node)
                                        { 
                                            rect1 = new Rect((element as Node).PxOffsetX, (element as Node).PxOffsetY, (element as Node).PxWidth, (element as Node).PxHeight);
                                            path1 = new Path();
                                            path1.Data = new RectangleGeometry(rect1);                                            
                                        }
                                        else if (element is LineConnector)
                                        {
                                            var c = VisualTreeHelper.GetChild(element as LineConnector, 0) as Canvas;
                                            if (c != null)
                                            {
                                                path1 = c.FindName("PART_ConnectionPath") as Path;
                                            }
                                        }
                                        pts = GetIntersectionPoints(path.Data,path1.Data);
                                        if (pts.Count() != 0)
                                        {
                                            foreach (UIElement ele in view.Page.Children.OfType<ICommon>())
                                            {
                                                if (Panel.GetZIndex(ele) > oldindex && Panel.GetZIndex(ele) <= newindex)
                                                {
                                                    Panel.SetZIndex(ele, Panel.GetZIndex(ele) - 1);
                                                }
                                            }
                                            Panel.SetZIndex((shape), newindex);
                                            mTemp = true;
                                            break;
                                        }
                                    }
                                }
                                if (mTemp == true)
                                {
                                    mTemp = false;
                                    break;
                                }
                            }
                        }

                        else if (view.ZOrderMode == ZOrderModes.Index)
                        {
                            if (oldindex < view.Page.Children.Count)
                            {
                                Panel.SetZIndex(shape as UIElement, oldindex + 1);
                            }                       

                            foreach (UIElement element in view.Page.Children.OfType<ICommon>())
                            {
                                if (!view.SelectionList.Contains(element) && Panel.GetZIndex(element) == oldindex + 1)
                                {
                                    Panel.SetZIndex(element, Panel.GetZIndex(element) - 1);
                                }
                            }
                        }
                    }
                }
                else
                {
                    int childcount = view.Page.Children.OfType<ICommon>().Count<ICommon>();
                    int initialindexvalue = childcount - view.SelectionList.Count;
                    List<UIElement> selectionordered = (from UIElement item in view.SelectionList
                                                        orderby Panel.GetZIndex(item as UIElement)
                                                        select item as UIElement).ToList();
                    for (int i = 0; i < view.SelectionList.Count; i++)
                    {
                        (selectionordered[i] as ICommon).OldZIndex = Panel.GetZIndex((selectionordered[i] as ICommon) as UIElement);
                        if ((selectionordered[i] as ICommon).OldZIndex != childcount - 1)
                        {
                            Panel.SetZIndex(selectionordered[i], (selectionordered[i] as ICommon).OldZIndex + 1);
                        }
                        else
                        {
                            for (int h = view.SelectionList.Count - 1; h >= 0; h--)
                            {
                                foreach (ICommon icom in view.SelectionList)
                                {
                                    if (icom != (selectionordered[h] as ICommon) && (Panel.GetZIndex(icom as UIElement) == Panel.GetZIndex(selectionordered[h] as UIElement)))
                                    {
                                        Panel.SetZIndex(icom as UIElement, icom.NewZIndex - 1);
                                    }
                                }
                            }
                        }

                        (selectionordered[i] as ICommon).NewZIndex = Panel.GetZIndex((selectionordered[i] as ICommon) as UIElement);
                    }

                    for (int i = 0; i < view.SelectionList.Count; i++)
                    {
                        foreach (UIElement element in view.Page.Children.OfType<ICommon>())
                        {
                            if (!selectionordered.Contains(element))
                            {
                                if (Panel.GetZIndex(element) == (selectionordered[i] as ICommon).NewZIndex)
                                {
                                    Panel.SetZIndex(element, Panel.GetZIndex(element) - 1);
                                    DiagramCommandManager.CompareIndex(element, view, selectionordered);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Invoked when the SendBackward Command is executed.
        /// </summary>
        /// <param name="sender">object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        public static void OnSendBackwardCommand(object sender, ExecutedRoutedEventArgs e)
        {
            DiagramView view = sender as DiagramView;
            if (view.SelectionList.Count > 0)
            {
                if (view.UndoRedoEnabled)
                {
                    foreach (UIElement element in view.Page.Children.OfType<ICommon>())
                    {
                        view.UndoStack.Push(Panel.GetZIndex(element));
                        view.UndoStack.Push(element);
                    }

                    view.UndoStack.Push(view.Page.Children.OfType<ICommon>().Count<ICommon>());
                    view.UndoStack.Push("Order");
                }

                List<UIElement> ordered = (from UIElement item in view.Page.Children.OfType<ICommon>()
                                           orderby Panel.GetZIndex(item as UIElement)
                                           select item as UIElement).ToList();
                bool mTemp = false;
                if (view.SelectionList.Count == 1)
                {
                    foreach (ICommon shape in view.SelectionList)
                    {
                        int oldindex = Panel.GetZIndex(shape as UIElement);
                        if (view.ZOrderMode == ZOrderModes.Visual)
                        {
                            for (int newindex = oldindex - 1; newindex >= 0; newindex--)
                            {
                                foreach (UIElement element in view.Page.Children.OfType<ICommon>())
                                {
                                    if (Panel.GetZIndex(element) == newindex)
                                    {
                                        Point[] pts = new Point[0];
                                        if (shape is Node)
                                        {
                                            rect = new Rect((shape as Node).PxOffsetX, (shape as Node).PxOffsetY, (shape as Node).PxWidth, (shape as Node).PxHeight);
                                            path = new Path();
                                            path.Data = new RectangleGeometry(rect);
                                        }
                                        else if (shape is LineConnector)
                                        {
                                            var c = VisualTreeHelper.GetChild(shape as LineConnector, 0) as Canvas;
                                            if (c != null)
                                            {
                                                path = c.FindName("PART_ConnectionPath") as Path;
                                            }
                                        }

                                        if (element is Node)
                                        {
                                            rect1 = new Rect((element as Node).PxOffsetX, (element as Node).PxOffsetY, (element as Node).PxWidth, (element as Node).PxHeight);
                                            path1 = new Path();
                                            path1.Data = new RectangleGeometry(rect1);
                                        }
                                        else if (element is LineConnector)
                                        {
                                            var c = VisualTreeHelper.GetChild(element as LineConnector, 0) as Canvas;
                                            if (c != null)
                                            {
                                                path1 = c.FindName("PART_ConnectionPath") as Path;
                                            }
                                        }
                                        pts = GetIntersectionPoints(path.Data, path1.Data);
                                        if (pts.Count() != 0)
                                        {
                                            foreach (UIElement ele in view.Page.Children.OfType<ICommon>())
                                            {
                                                if (Panel.GetZIndex(ele) < oldindex && Panel.GetZIndex(ele) >= newindex)
                                                {
                                                    Panel.SetZIndex(ele, Panel.GetZIndex(ele) + 1);
                                                }
                                            }
                                            Panel.SetZIndex((shape as UIElement), newindex);
                                            mTemp = true;
                                            break;
                                        }
                                    }
                                }
                                if (mTemp == true)
                                {
                                    mTemp = false;
                                    break;
                                }
                            }
                        }
                        else if (view.ZOrderMode == ZOrderModes.Index)
                        {
                            if (oldindex > 1)
                            {
                                Panel.SetZIndex(shape as UIElement, oldindex - 1);
                            }

                            int newindex = Panel.GetZIndex(shape as UIElement);

                            foreach (UIElement element in view.Page.Children.OfType<ICommon>())
                            {
                                if (!view.SelectionList.Contains(element) && Panel.GetZIndex(element) == oldindex - 1)
                                {
                                    Panel.SetZIndex(element, Panel.GetZIndex(element) + 1);
                                }
                            }
                        }
                    }                    
                }
                else
                {
                    int childcount = view.Page.Children.OfType<ICommon>().Count<ICommon>();
                    int initialindexvalue = childcount - view.SelectionList.Count;
                    List<UIElement> selectionordered = (from UIElement item in view.SelectionList
                                                        orderby Panel.GetZIndex(item as UIElement)
                                                        select item as UIElement).ToList();
                    for (int i = view.SelectionList.Count - 1; i >= 0; i--)
                    {
                        (selectionordered[i] as ICommon).OldZIndex = Panel.GetZIndex((selectionordered[i] as ICommon) as UIElement);
                        if ((selectionordered[i] as ICommon).OldZIndex != 0)
                        {
                            Panel.SetZIndex(selectionordered[i], (selectionordered[i] as ICommon).OldZIndex - 1);
                        }
                        else
                        {
                            for (int h = 0; h <= view.SelectionList.Count - 1; h++)
                            {
                                foreach (ICommon icom in view.SelectionList)
                                {
                                    if (icom != (selectionordered[h] as ICommon) && (Panel.GetZIndex(icom as UIElement) == Panel.GetZIndex(selectionordered[h] as UIElement)))
                                    {
                                        Panel.SetZIndex(icom as UIElement, icom.NewZIndex + 1);
                                    }
                                }
                            }
                        }

                        (selectionordered[i] as ICommon).NewZIndex = Panel.GetZIndex((selectionordered[i] as ICommon) as UIElement);
                    }

                    for (int i = 0; i < view.SelectionList.Count - 1; i++)
                    {
                        foreach (UIElement element in view.Page.Children.OfType<ICommon>())
                        {
                            if (!selectionordered.Contains(element))
                            {
                                if (Panel.GetZIndex(element) == (selectionordered[i] as ICommon).NewZIndex)
                                {
                                    Panel.SetZIndex(element, Panel.GetZIndex(element) + 1);
                                    DiagramCommandManager.CompareBackwardIndex(element, view, selectionordered);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Compares the index.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="view">The view instance.</param>
        /// <param name="selectionordered">The ordered list.</param>
        private static void CompareIndex(UIElement element, DiagramView view, List<UIElement> selectionordered)
        {
            for (int i = 0; i < view.SelectionList.Count - 1; i++)
            {
                if (Panel.GetZIndex(element) == (selectionordered[i] as ICommon).NewZIndex)
                {
                    Panel.SetZIndex(element, Panel.GetZIndex(element) - 1);
                    DiagramCommandManager.CompareBackwardIndex(element, view, selectionordered);
                }
            }
        }

        /// <summary>
        /// Compares the index of the backward.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="view">The view instance.</param>
        /// <param name="selectionordered">The ordered list.</param>
        private static void CompareBackwardIndex(UIElement element, DiagramView view, List<UIElement> selectionordered)
        {
            for (int i = 0; i < view.SelectionList.Count; i++)
            {
                if (Panel.GetZIndex(element) == (selectionordered[i] as ICommon).NewZIndex)
                {
                    Panel.SetZIndex(element, Panel.GetZIndex(element) + 1);
                    DiagramCommandManager.CompareBackwardIndex(element, view, selectionordered);
                }
            }
        }

        #endregion

        #region CopyCommand

        /// <summary>
        /// Invoked when the Copy Command is executed.
        /// </summary>
        /// <param name="sender">The DiagramView.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        public static void OnCutCommand(object sender, ExecutedRoutedEventArgs e)
        {
            DiagramView view = sender as DiagramView;
            if (view != null)
            {
                view.CPManager.cut();
            }
        }

        /// <summary>
        /// Invoked when the Copy Command is executed.
        /// </summary>
        /// <param name="sender">The DiagramView.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        public static void OnCopyCommand(object sender, ExecutedRoutedEventArgs e)
        {
            DiagramView view = sender as DiagramView;
            if (view != null)
            {
                view.CPManager.copy();
            }
        }

        /// <summary>
        /// Invoked when the Copy Command is executed.
        /// </summary>
        /// <param name="sender">The DiagramView.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        public static void OnPasteCommand(object sender, ExecutedRoutedEventArgs e)
        {
            DiagramView view = sender as DiagramView;
            if (view != null)
            {
                view.CPManager.paste();
            }
        }

        #endregion

        #region Alignment Commands

        /// <summary>
        /// Invoked when the AlignLeft Command is executed.
        /// </summary>
        /// <param name="sender">The DiagramView.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        public static void OnAlignLeftCommand(object sender, ExecutedRoutedEventArgs e)
        {            
            bool donotexecute = false;
            double refvalue = 0;
            DiagramView view = sender as DiagramView;
            
            if (view.SelectionList.Count > 0)
            {
                var nodes = view.SelectionList.OfType<IShape>();
                view.NodeDragCount = nodes.Count() - 1;
                if (view.SelectionList[0] is Node)
                {
                    refvalue = (view.SelectionList[0] as Node).PxOffsetX;
                }
                else if (view.SelectionList[0] is LineConnector && (view.SelectionList[0] as LineConnector).HeadNode == null && (view.SelectionList[0] as LineConnector).TailNode == null)
                {
                    refvalue = (view.SelectionList[0] as LineConnector).GetBounds().Left;// MeasureUnitsConverter.FromPixels((view.SelectionList[0] as LineConnector).StartPointPosition.X, (view.Page as DiagramPage).MeasurementUnits);
                }
                else
                {
                    donotexecute = true;
                }

                if (!donotexecute)
                {
                    foreach (ICommon shape in view.SelectionList)
                    {
                        if (shape is Group)
                        {
                            double oldx = (shape as Group).PxOffsetX;
                            (shape as Group).PxOffsetX = refvalue;
                            double translateoffset = (shape as Group).PxOffsetX - oldx;
                            foreach (INodeGroup child in (shape as Group).NodeChildren)
                            {
                                if (child is Node)
                                {
                                    (child as Node).PxOffsetX += translateoffset;
                                }
                                else if (child is LineConnector && (child as LineConnector).HeadNode == null && (child as LineConnector).TailNode == null)
                                {
                                    (child as LineConnector).PxEndPointPosition = new Point((child as LineConnector).PxEndPointPosition.X + translateoffset, (child as LineConnector).PxEndPointPosition.Y);
                                    (child as LineConnector).PxStartPointPosition = new Point((child as LineConnector).PxStartPointPosition.X + translateoffset, (child as LineConnector).PxStartPointPosition.Y);
                                    if ((child as LineConnector).IntermediatePoints != null)
                                    {
                                        for (int i = 0; i < (child as LineConnector).IntermediatePoints.Count; i++)
                                        {
                                            (child as LineConnector).IntermediatePoints[i] = new Point((child as LineConnector).IntermediatePoints[i].X + translateoffset, (child as LineConnector).IntermediatePoints[i].Y);
                                        }
                                    }
                                    (child as LineConnector).InvalidateConnectorPathGeometry();
                                }
                            }
                        }
                        else if (shape is Node)
                        {
                            (shape as Node).PxOffsetX = refvalue;
                        }
                        else if (shape is LineConnector && (shape as LineConnector).HeadNode == null && (shape as LineConnector).TailNode == null)
                        {                           
                            (shape as LineConnector).UndoRedoOperation();
                            double translateval = (shape as LineConnector).GetBounds().Left - refvalue;// MeasureUnitsConverter.ToPixels(refvalue, (view.Page as DiagramPage).MeasurementUnits);
                            (shape as LineConnector).PxStartPointPosition = new Point((shape as LineConnector).PxStartPointPosition.X-translateval, (shape as LineConnector).PxStartPointPosition.Y);
                            (shape as LineConnector).PxEndPointPosition = new Point((shape as LineConnector).PxEndPointPosition.X - translateval, (shape as LineConnector).PxEndPointPosition.Y);
                            if ((shape as LineConnector).IntermediatePoints != null)
                            {
                                for (int i = 0; i < (shape as LineConnector).IntermediatePoints.Count; i++)
                                {                                        
                                    (shape as LineConnector).IntermediatePoints[i] = new Point((shape as LineConnector).IntermediatePoints[i].X - translateval, (shape as LineConnector).IntermediatePoints[i].Y);
                                }
                            }

                          
                            (shape as LineConnector).InvalidateConnectorPathGeometry();
                        }
                    }
                }
            }
            (view.Page as DiagramPage).InvalidateMeasure();
            (view.Page as DiagramPage).InvalidateArrange();
        }

       

        /// <summary>
        /// Invoked when the AlignCenter Command is executed.
        /// </summary>
        /// <param name="sender">The DiagramView.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        public static void OnAlignCenterCommand(object sender, ExecutedRoutedEventArgs e)
        {
            bool donotexecute = false;
            double refvalue = 0;
            DiagramView view = sender as DiagramView;
            if (view.SelectionList.Count > 0)
            {
                var nodes = view.SelectionList.OfType<IShape>();
                view.NodeDragCount = nodes.Count() - 1;
                if (view.SelectionList[0] is Node)
                {
                    refvalue = (view.SelectionList[0] as Node).PxOffsetX + (view.SelectionList[0] as Node).Width / 2;// (MeasureUnitsConverter.FromPixels((view.SelectionList[0] as Node).Width, (view.Page as DiagramPage).MeasurementUnits) / 2);
                }
                else if (view.SelectionList[0] is LineConnector && (view.SelectionList[0] as LineConnector).HeadNode == null && (view.SelectionList[0] as LineConnector).TailNode == null)
                {
                    refvalue = ((view.SelectionList[0] as LineConnector).GetBounds().Left+((view.SelectionList[0] as LineConnector).GetBounds().Right))/2;// MeasureUnitsConverter.FromPixels((view.SelectionList[0] as LineConnector).DropPoint.X, (view.Page as DiagramPage).MeasurementUnits);
                }
                else
                {
                    donotexecute = true;
                }

                if (!donotexecute)
                {
                    foreach (ICommon shape in view.SelectionList)
                    {
                        if (shape is Group)
                        {
                            double oldx = (shape as Group).PxOffsetX;
                            (shape as Group).PxOffsetX = refvalue - (shape as Group).Width / 2;// (MeasureUnitsConverter.FromPixels((shape as Group).Width, (view.Page as DiagramPage).MeasurementUnits) / 2);
                            double translateoffset = (shape as Group).PxOffsetX - oldx;
                            foreach (INodeGroup child in (shape as Group).NodeChildren)
                            {
                                if (child is Node)
                                {
                                    (child as Node).PxOffsetX += translateoffset;
                                }
                                else if (child is LineConnector && (child as LineConnector).HeadNode == null && (child as LineConnector).TailNode == null)
                                {
                                    (child as LineConnector).PxEndPointPosition = new Point((child as LineConnector).PxEndPointPosition.X + translateoffset, (child as LineConnector).PxEndPointPosition.Y);
                                    (child as LineConnector).PxStartPointPosition = new Point((child as LineConnector).PxStartPointPosition.X + translateoffset, (child as LineConnector).PxStartPointPosition.Y);
                                    if ((child as LineConnector).IntermediatePoints != null)
                                    {
                                        for (int i = 0; i < (child as LineConnector).IntermediatePoints.Count; i++)
                                        {
                                            (child as LineConnector).IntermediatePoints[i] = new Point((child as LineConnector).IntermediatePoints[i].X + translateoffset, (child as LineConnector).IntermediatePoints[i].Y);
                                        }
                                    }
                                    (child as LineConnector).InvalidateConnectorPathGeometry();
                                }
                            }
                        }
                        else if (shape is Node)
                        {
                            (shape as Node).PxOffsetX = refvalue - (shape as Node).Width / 2;// (MeasureUnitsConverter.FromPixels((shape as Node).Width, (view.Page as DiagramPage).MeasurementUnits) / 2);
                        }
                        else if (shape is LineConnector && (shape as LineConnector).HeadNode == null && (shape as LineConnector).TailNode == null)
                        {
                            (shape as LineConnector).UndoRedoOperation();                        
                            double translateval = ((shape as LineConnector).GetBounds().Left + (shape as LineConnector).GetBounds().Right) / 2 - refvalue;// MeasureUnitsConverter.ToPixels(refvalue, (view.Page as DiagramPage).MeasurementUnits);
                            (shape as LineConnector).PxStartPointPosition = new Point((shape as LineConnector).PxStartPointPosition.X - translateval, (shape as LineConnector).PxStartPointPosition.Y);
                            (shape as LineConnector).PxEndPointPosition = new Point((shape as LineConnector).PxEndPointPosition.X - translateval, (shape as LineConnector).PxEndPointPosition.Y);
                            if ((shape as LineConnector).IntermediatePoints != null)
                            {
                                for (int i = 0; i < (shape as LineConnector).IntermediatePoints.Count; i++)
                                {
                                    //translateval = ((shape as LineConnector).PxStartPointPosition.X + (shape as LineConnector).IntermediatePoints[i].X) / 2 - refvalue;
                                    (shape as LineConnector).IntermediatePoints[i] = new Point((shape as LineConnector).IntermediatePoints[i].X-translateval, (shape as LineConnector).IntermediatePoints[i].Y);
                                }
                            }
                            
                            (shape as LineConnector).InvalidateConnectorPathGeometry();
                        }                        
                    }
                }
            }

            (view.Page as DiagramPage).InvalidateMeasure();
            (view.Page as DiagramPage).InvalidateArrange();
        }

        /// <summary>
        /// Invoked when the AlignRight Command is executed.
        /// </summary>
        /// <param name="sender">The DiagramView.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        public static void OnAlignRightCommand(object sender, ExecutedRoutedEventArgs e)
        {
            bool donotexecute = false;
            double refvalue = 0;
            DiagramView view = sender as DiagramView;
            if (view.SelectionList.Count > 0)
            {
                var nodes = view.SelectionList.OfType<IShape>();
                view.NodeDragCount = nodes.Count() - 1;
                if (view.SelectionList[0] is Node)
                {
                    refvalue = (view.SelectionList[0] as Node).PxOffsetX + (view.SelectionList[0] as Node).Width;// MeasureUnitsConverter.FromPixels((view.SelectionList[0] as Node).Width, (view.Page as DiagramPage).MeasurementUnits);
                }
                else if (view.SelectionList[0] is LineConnector && (view.SelectionList[0] as LineConnector).HeadNode == null && (view.SelectionList[0] as LineConnector).TailNode == null)
                {
                    refvalue = (view.SelectionList[0] as LineConnector).GetBounds().Right; ;// MeasureUnitsConverter.FromPixels((view.SelectionList[0] as LineConnector).StartPointPosition.X, (view.Page as DiagramPage).MeasurementUnits);                    
                }
                else
                {
                    donotexecute = true;
                }

                if (!donotexecute)
                {
                    foreach (ICommon shape in view.SelectionList)
                    {
                        if (shape is Group)
                        {
                            double oldx = (shape as Group).PxOffsetX;
                            (shape as Group).PxOffsetX = refvalue - (shape as Node).Width;// MeasureUnitsConverter.FromPixels((shape as Node).Width, (view.Page as DiagramPage).MeasurementUnits);
                            double translateoffset = (shape as Group).PxOffsetX - oldx;

                            foreach (INodeGroup child in (shape as Group).NodeChildren)
                            {
                                if (child is Node)
                                {
                                    (child as Node).PxOffsetX += translateoffset;
                                }
                                else if (child is LineConnector && (child as LineConnector).HeadNode == null && (child as LineConnector).TailNode == null)
                                {
                                    (child as LineConnector).PxEndPointPosition = new Point((child as LineConnector).PxEndPointPosition.X + translateoffset, (child as LineConnector).PxEndPointPosition.Y);
                                    (child as LineConnector).PxStartPointPosition = new Point((child as LineConnector).PxStartPointPosition.X + translateoffset, (child as LineConnector).PxStartPointPosition.Y);
                                    if ((child as LineConnector).IntermediatePoints != null)
                                    {
                                        for (int i = 0; i < (child as LineConnector).IntermediatePoints.Count; i++)
                                        {
                                            (child as LineConnector).IntermediatePoints[i] = new Point((child as LineConnector).IntermediatePoints[i].X + translateoffset, (child as LineConnector).IntermediatePoints[i].Y);
                                        }
                                    }
                                    (child as LineConnector).InvalidateConnectorPathGeometry();
                                }
                            }
                        }
                        else if (shape is Node)
                        {
                            (shape as Node).PxOffsetX = refvalue - (shape as Node).Width;// MeasureUnitsConverter.FromPixels((shape as Node).Width, (view.Page as DiagramPage).MeasurementUnits);
                        }
                        else if (shape is LineConnector && (shape as LineConnector).HeadNode == null && (shape as LineConnector).TailNode == null)
                        {
                            (shape as LineConnector).UndoRedoOperation();                        
                            double translateval = (shape as LineConnector).GetBounds().Right - refvalue;// MeasureUnitsConverter.ToPixels(refvalue, (view.Page as DiagramPage).MeasurementUnits);
                            (shape as LineConnector).PxStartPointPosition = new Point((shape as LineConnector).PxStartPointPosition.X - translateval, (shape as LineConnector).PxStartPointPosition.Y);
                            (shape as LineConnector).PxEndPointPosition = new Point((shape as LineConnector).PxEndPointPosition.X - translateval, (shape as LineConnector).PxEndPointPosition.Y);
                            if ((shape as LineConnector).IntermediatePoints != null)
                            {
                                for (int i = 0; i < (shape as LineConnector).IntermediatePoints.Count; i++)
                                {                                        
                                    (shape as LineConnector).IntermediatePoints[i] = new Point((shape as LineConnector).IntermediatePoints[i].X - translateval, (shape as LineConnector).IntermediatePoints[i].Y);
                                }
                            }
                            
                            (shape as LineConnector).InvalidateConnectorPathGeometry();
                        }                        
                    }
                }
            }

            (view.Page as DiagramPage).InvalidateMeasure();
            (view.Page as DiagramPage).InvalidateArrange();
        }

        /// <summary>
        /// Invoked when the AlignTop Command is executed.
        /// </summary>
        /// <param name="sender">The DiagramView.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        public static void OnAlignTopCommand(object sender, ExecutedRoutedEventArgs e)
        {
            bool donotexecute = false;
            double refvalue = 0;
            DiagramView view = sender as DiagramView;
            if (view.SelectionList.Count > 0)
            {
                var nodes = view.SelectionList.OfType<IShape>();
                view.NodeDragCount = nodes.Count() - 1;
                if (view.SelectionList[0] is Node)
                {
                    refvalue = (view.SelectionList[0] as Node).PxOffsetY;
                }
                else if (view.SelectionList[0] is LineConnector && (view.SelectionList[0] as LineConnector).HeadNode == null && (view.SelectionList[0] as LineConnector).TailNode == null)
                {
                    refvalue = (view.SelectionList[0] as LineConnector).GetBounds().Top;// MeasureUnitsConverter.FromPixels((view.SelectionList[0] as LineConnector).StartPointPosition.Y, (view.Page as DiagramPage).MeasurementUnits);                   
                }
                else
                {
                    donotexecute = true;
                }

                if (!donotexecute)
                {
                    foreach (ICommon shape in view.SelectionList)
                    {
                        if (shape is Group)
                        {
                            double oldy = (shape as Group).PxOffsetY;
                            (shape as Group).PxOffsetY = refvalue;
                            double translateoffset = (shape as Group).PxOffsetY - oldy;
                            foreach (INodeGroup child in (shape as Group).NodeChildren)
                            {
                                if (child is Node)
                                {
                                    (child as Node).PxOffsetY += translateoffset;
                                }
                                else if (child is LineConnector && (child as LineConnector).HeadNode == null && (child as LineConnector).TailNode == null)
                                {
                                    (child as LineConnector).PxStartPointPosition = new Point((child as LineConnector).PxStartPointPosition.X, (child as LineConnector).PxStartPointPosition.Y + translateoffset);//MeasureUnitsConverter.ToPixels(translateoffset, (view.Page as DiagramPage).MeasurementUnits));
                                    (child as LineConnector).PxEndPointPosition = new Point((child as LineConnector).PxEndPointPosition.X, (child as LineConnector).PxEndPointPosition.Y + translateoffset);// MeasureUnitsConverter.ToPixels(translateoffset, (view.Page as DiagramPage).MeasurementUnits));
                                    if ((child as LineConnector).IntermediatePoints != null)
                                    {
                                        for (int i = 0; i < (child as LineConnector).IntermediatePoints.Count; i++)
                                        {
                                            (child as LineConnector).IntermediatePoints[i] = new Point((child as LineConnector).IntermediatePoints[i].X, (child as LineConnector).IntermediatePoints[i].Y + translateoffset);
                                        }
                                    }
                                    (child as LineConnector).InvalidateConnectorPathGeometry();
                                }
                            }
                        }
                        else if (shape is Node)
                        {
                            (shape as Node).PxOffsetY = refvalue;
                        }
                        else if (shape is LineConnector && (shape as LineConnector).HeadNode == null && (shape as LineConnector).TailNode == null)
                        {
                            (shape as LineConnector).UndoRedoOperation();                       
                            double translateval = (shape as LineConnector).GetBounds().Top - refvalue;// MeasureUnitsConverter.ToPixels(refvalue, (view.Page as DiagramPage).MeasurementUnits);
                            (shape as LineConnector).PxStartPointPosition = new Point((shape as LineConnector).PxStartPointPosition.X, (shape as LineConnector).PxStartPointPosition.Y - translateval);//MeasureUnitsConverter.ToPixels(refvalue, (view.Page as DiagramPage).MeasurementUnits));
                            (shape as LineConnector).PxEndPointPosition = new Point((shape as LineConnector).PxEndPointPosition.X, (shape as LineConnector).PxEndPointPosition.Y - translateval);
                            if ((shape as LineConnector).IntermediatePoints != null)
                            {
                                for (int i = 0; i < (shape as LineConnector).IntermediatePoints.Count; i++)
                                {
                                    (shape as LineConnector).IntermediatePoints[i] = new Point((shape as LineConnector).IntermediatePoints[i].X, (shape as LineConnector).IntermediatePoints[i].Y - translateval);
                                }
                            }
                            
                            (shape as LineConnector).InvalidateConnectorPathGeometry();
                        }                        
                    }
                }
            }

            (view.Page as DiagramPage).InvalidateMeasure();
            (view.Page as DiagramPage).InvalidateArrange();
        }

        /// <summary>
        /// Invoked when the AlignMiddle Command is executed.
        /// </summary>
        /// <param name="sender">The DiagramView.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        public static void OnAlignMiddleCommand(object sender, ExecutedRoutedEventArgs e)
        {
            bool donotexecute = false;
            double refvalue = 0;
            DiagramView view = sender as DiagramView;
            if (view.SelectionList.Count > 0)
            {
                var nodes = view.SelectionList.OfType<IShape>();
                view.NodeDragCount = nodes.Count() - 1;
                if (view.SelectionList[0] is Node)
                {
                    refvalue = (view.SelectionList[0] as Node).PxOffsetY + (view.SelectionList[0] as Node).Height / 2;// (MeasureUnitsConverter.FromPixels((view.SelectionList[0] as Node).Height, (view.Page as DiagramPage).MeasurementUnits) / 2);
                }
                else if (view.SelectionList[0] is LineConnector && (view.SelectionList[0] as LineConnector).HeadNode == null && (view.SelectionList[0] as LineConnector).TailNode == null)
                {
                    refvalue = ((view.SelectionList[0] as LineConnector).GetBounds().Top + (view.SelectionList[0] as LineConnector).GetBounds().Bottom)/2;// MeasureUnitsConverter.FromPixels((view.SelectionList[0] as LineConnector).DropPoint.Y, (view.Page as DiagramPage).MeasurementUnits);
                }
                else
                {
                    donotexecute = true;
                }

                if (!donotexecute)
                {
                    foreach (ICommon shape in view.SelectionList)
                    {
                        if (shape is Group)
                        {
                            double oldy = (shape as Group).PxOffsetY;
                            (shape as Group).PxOffsetY = refvalue - ((shape as Node).Height / 2);
                            double translateoffset = (shape as Group).PxOffsetY - oldy;
                            foreach (INodeGroup child in (shape as Group).NodeChildren)
                            {
                                if (child is Node)
                                {
                                    (child as Node).PxOffsetY += translateoffset;
                                }
                                else if (child is LineConnector && (child as LineConnector).HeadNode == null && (child as LineConnector).TailNode == null)
                                {
                                    (child as LineConnector).PxStartPointPosition = new Point((child as LineConnector).PxStartPointPosition.X, (child as LineConnector).PxStartPointPosition.Y + translateoffset);//MeasureUnitsConverter.ToPixels(translateoffset, (view.Page as DiagramPage).MeasurementUnits));
                                    (child as LineConnector).PxEndPointPosition = new Point((child as LineConnector).PxEndPointPosition.X, (child as LineConnector).PxEndPointPosition.Y + translateoffset);// MeasureUnitsConverter.ToPixels(translateoffset, (view.Page as DiagramPage).MeasurementUnits));
                                    if ((child as LineConnector).IntermediatePoints != null)
                                    {
                                        for (int i = 0; i < (child as LineConnector).IntermediatePoints.Count; i++)
                                        {
                                            (child as LineConnector).IntermediatePoints[i] = new Point((child as LineConnector).IntermediatePoints[i].X, (child as LineConnector).IntermediatePoints[i].Y + translateoffset);
                                        }
                                    }
                                    (child as LineConnector).InvalidateConnectorPathGeometry();
                                }
                            }
                        }
                        else if (shape is Node)
                        {
                            (shape as Node).PxOffsetY = refvalue - (shape as Node).Height / 2;// (MeasureUnitsConverter.FromPixels((shape as Node).Height, (view.Page as DiagramPage).MeasurementUnits) / 2);
                        }
                        else if (shape is LineConnector && (shape as LineConnector).HeadNode == null && (shape as LineConnector).TailNode == null)
                        {
                            (shape as LineConnector).UndoRedoOperation();                        
                            double translateval = ((shape as LineConnector).GetBounds().Top + (shape as LineConnector).GetBounds().Bottom) / 2 - refvalue;
                            (shape as LineConnector).PxStartPointPosition = new Point((shape as LineConnector).PxStartPointPosition.X, (shape as LineConnector).PxStartPointPosition.Y - translateval);
                            (shape as LineConnector).PxEndPointPosition = new Point((shape as LineConnector).PxEndPointPosition.X, (shape as LineConnector).PxEndPointPosition.Y - translateval);
                            if ((shape as LineConnector).IntermediatePoints != null)
                            {
                                for (int i = 0; i < (shape as LineConnector).IntermediatePoints.Count; i++)
                                {
                                    //translateval = ((shape as LineConnector).PxStartPointPosition.Y + (shape as LineConnector).IntermediatePoints[i].Y) / 2 - refvalue;
                                    (shape as LineConnector).IntermediatePoints[i] = new Point((shape as LineConnector).IntermediatePoints[i].X, (shape as LineConnector).IntermediatePoints[i].Y - translateval);
                                }
                            }
                            
                            (shape as LineConnector).InvalidateConnectorPathGeometry();
                        }                        
                    }
                }
            }

            (view.Page as DiagramPage).InvalidateMeasure();
            (view.Page as DiagramPage).InvalidateArrange();
        }

        /// <summary>
        /// Invoked when the AlignBottom Command is executed.
        /// </summary>
        /// <param name="sender">The DiagramView.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        public static void OnAlignBottomCommand(object sender, ExecutedRoutedEventArgs e)
        {
            bool donotexecute = false;
            double refvalue = 0;
            DiagramView view = sender as DiagramView;
            if (view.SelectionList.Count > 0)
            {
                var nodes = view.SelectionList.OfType<IShape>();
                view.NodeDragCount = nodes.Count() - 1;
                if (view.SelectionList[0] is Node)
                {
                    refvalue = (view.SelectionList[0] as Node).PxOffsetY + (view.SelectionList[0] as Node).Height;
                }
                else if (view.SelectionList[0] is LineConnector && (view.SelectionList[0] as LineConnector).HeadNode == null && (view.SelectionList[0] as LineConnector).TailNode == null)
                {                    
                    refvalue = (view.SelectionList[0] as LineConnector).GetBounds().Bottom;// MeasureUnitsConverter.FromPixels((view.SelectionList[0] as LineConnector).StartPointPosition.Y, (view.Page as DiagramPage).MeasurementUnits);                    
                }
                else
                {
                    donotexecute = true;
                }

                if (!donotexecute)
                {
                    foreach (ICommon shape in view.SelectionList)
                    {
                        if (shape is Group)
                        {
                            double oldy = (shape as Group).PxOffsetY;
                            (shape as Group).PxOffsetY = refvalue - (shape as Node).Height;
                            double translateoffset = (shape as Group).PxOffsetY - oldy;
                            foreach (INodeGroup child in (shape as Group).NodeChildren)
                            {
                                if (child is Node)
                                {
                                    (child as Node).PxOffsetY += translateoffset;
                                }
                                else if (child is LineConnector && (child as LineConnector).HeadNode == null && (child as LineConnector).TailNode == null)
                                {
                                    (child as LineConnector).PxStartPointPosition = new Point((child as LineConnector).PxStartPointPosition.X, (child as LineConnector).PxStartPointPosition.Y + translateoffset);//MeasureUnitsConverter.ToPixels(translateoffset, (view.Page as DiagramPage).MeasurementUnits));
                                    (child as LineConnector).PxEndPointPosition = new Point((child as LineConnector).PxEndPointPosition.X, (child as LineConnector).PxEndPointPosition.Y + translateoffset);//MeasureUnitsConverter.ToPixels(translateoffset, (view.Page as DiagramPage).MeasurementUnits));
                                    if ((child as LineConnector).IntermediatePoints != null)
                                    {
                                        for (int i = 0; i < (child as LineConnector).IntermediatePoints.Count; i++)
                                        {
                                            (child as LineConnector).IntermediatePoints[i] = new Point((child as LineConnector).IntermediatePoints[i].X, (child as LineConnector).IntermediatePoints[i].Y + translateoffset);
                                        }
                                    }
                                    (child as LineConnector).InvalidateConnectorPathGeometry();
                                }
                            }
                        }
                        else if (shape is Node)
                        {
                           (shape as Node).PxOffsetY = refvalue - (shape as Node).Height;
                        }
                        else if (shape is LineConnector && (shape as LineConnector).HeadNode == null && (shape as LineConnector).TailNode == null)
                        {
                            (shape as LineConnector).UndoRedoOperation();
                            double translateval = (shape as LineConnector).GetBounds().Bottom - refvalue;
                            (shape as LineConnector).PxStartPointPosition = new Point((shape as LineConnector).PxStartPointPosition.X, (shape as LineConnector).PxStartPointPosition.Y - translateval);
                            (shape as LineConnector).PxEndPointPosition = new Point((shape as LineConnector).PxEndPointPosition.X, (shape as LineConnector).PxEndPointPosition.Y - translateval);
                            if ((shape as LineConnector).IntermediatePoints != null)
                            {
                                for (int i = 0; i < (shape as LineConnector).IntermediatePoints.Count; i++)
                                {                                        
                                    (shape as LineConnector).IntermediatePoints[i] = new Point((shape as LineConnector).IntermediatePoints[i].X, (shape as LineConnector).IntermediatePoints[i].Y - translateval);
                                }
                            }
                              
                            (shape as LineConnector).InvalidateConnectorPathGeometry();
                        }                        
                    }
                }
            }

            (view.Page as DiagramPage).InvalidateMeasure();
            (view.Page as DiagramPage).InvalidateArrange();
        }

        #endregion

        #region Space Commands

        /// <summary>
        /// Invoked when the SpaceDown Command is executed.
        /// </summary>
        /// <param name="sender">The DiagramView.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        public static void OnSpaceDownCommand(object sender, ExecutedRoutedEventArgs e)
        {
            DiagramView view = sender as DiagramView;
            IDiagramCommandParameter param = e.Parameter as IDiagramCommandParameter;
            List<ICommon> tempList = new List<ICommon>();
            if (e.Parameter is IDiagramCommandParameter)
            {
                param = e.Parameter as IDiagramCommandParameter;
                switch (param.SelectionFilter)
                {
                    case SelectionFilter.FilterLines:
                        {
                            foreach (Node n in view.SelectionList.OfType<Node>())
                            {
                                tempList.Add(n);
                            }
                            OnSpaceDownFilter(tempList);
                            (view.Page as DiagramPage).InvalidateMeasure();
                            (view.Page as DiagramPage).InvalidateArrange();
                            break;
                        }
                    case SelectionFilter.FilterNodes:
                        {
                            foreach (LineConnector l in view.SelectionList.OfType<LineConnector>())
                            {
                                tempList.Add(l);                               
                            }                            
                            OnSpaceDownFilter(tempList);
                            (view.Page as DiagramPage).InvalidateMeasure();
                            (view.Page as DiagramPage).InvalidateArrange();
                            break;
                        }
                    default:
                        {
                            OnSpaceDownFilter(view.SelectionList.OfType<ICommon>().ToList());
                            (view.Page as DiagramPage).InvalidateMeasure();
                            (view.Page as DiagramPage).InvalidateArrange();
                            break;
                        }
                }
            }
            else
            {
                OnSpaceDownFilter(view.SelectionList.OfType<ICommon>().ToList());
                (view.Page as DiagramPage).InvalidateMeasure();
                (view.Page as DiagramPage).InvalidateArrange();
            }
        }
        private static void OnSpaceDownFilter(List<ICommon> tempList)
        {           
            bool donotexecute = false;
            double topvalue = 0;
            double bottomvalue = 0;

            if (tempList.Count > 2)
            {
                if (tempList[0] is Node)
                {
                    topvalue = (tempList[0] as Node).PxOffsetY;
                }
                else if (tempList[0] is LineConnector && (tempList[0] as LineConnector).HeadNode == null && (tempList[0] as LineConnector).TailNode == null)
                {
                    if ((tempList[0] as LineConnector).PxStartPointPosition.Y < (tempList[0] as LineConnector).PxEndPointPosition.Y)
                    {
                        //topvalue = MeasureUnitsConverter.FromPixels((tempList[0] as LineConnector).StartPointPosition.Y, (view.Page as DiagramPage).MeasurementUnits);
                        topvalue = (tempList[0] as LineConnector).PxStartPointPosition.Y;
                        (tempList[0] as LineConnector).InvalidateConnectorPathGeometry();
                    }
                    else
                    {
                        //topvalue = MeasureUnitsConverter.FromPixels((tempList[0] as LineConnector).EndPointPosition.Y, (view.Page as DiagramPage).MeasurementUnits);
                        topvalue = (tempList[0] as LineConnector).PxEndPointPosition.Y;
                        (tempList[0] as LineConnector).InvalidateConnectorPathGeometry();
                    }
                }
                else
                {
                    donotexecute = true;
                }

                if (tempList[tempList.Count - 1] is Node)
                {
                    bottomvalue = (tempList[tempList.Count - 1] as Node).PxOffsetY;
                }
                else if (tempList[tempList.Count - 1] is LineConnector && (tempList[tempList.Count - 1] as LineConnector).HeadNode == null && (tempList[tempList.Count - 1] as LineConnector).TailNode == null)
                {
                    if ((tempList[tempList.Count - 1] as LineConnector).PxStartPointPosition.Y < (tempList[tempList.Count - 1] as LineConnector).PxEndPointPosition.Y)
                    {
                        //bottomvalue = MeasureUnitsConverter.FromPixels((tempList[tempList.Count - 1] as LineConnector).StartPointPosition.Y, (view.Page as DiagramPage).MeasurementUnits);
                        bottomvalue = (tempList[tempList.Count - 1] as LineConnector).PxStartPointPosition.Y;
                        (tempList[tempList.Count - 1] as LineConnector).InvalidateConnectorPathGeometry();
                    }
                    else
                    {
                        //bottomvalue = MeasureUnitsConverter.FromPixels((tempList[tempList.Count - 1] as LineConnector).EndPointPosition.Y, (view.Page as DiagramPage).MeasurementUnits);
                        bottomvalue = (tempList[tempList.Count - 1] as LineConnector).PxEndPointPosition.Y;
                        (tempList[tempList.Count - 1] as LineConnector).InvalidateConnectorPathGeometry();
                    }
                }
                else
                {
                    donotexecute = true;
                }

                if (!donotexecute)
                {
                    double count = tempList.Count - 1;
                    double refvalue = (bottomvalue - topvalue) / count;
                    for (int i = 1; i < tempList.Count - 1; i++)
                    {
                        if (tempList[i] is Group)
                        {
                            double oldy = (tempList[i] as Group).PxOffsetY;
                            (tempList[i] as Group).PxOffsetY = topvalue + (i * refvalue);
                            double translateoffset = (tempList[i] as Group).PxOffsetY - oldy;
                            foreach (INodeGroup child in (tempList[i] as Group).NodeChildren)
                            {
                                if (child is Node)
                                {
                                    (child as Node).PxOffsetY += translateoffset;
                                }
                                else if (child is LineConnector && (child as LineConnector).HeadNode == null && (child as LineConnector).TailNode == null)
                                {
                                    //(child as LineConnector).StartPointPosition = new Point((child as LineConnector).StartPointPosition.X, (child as LineConnector).StartPointPosition.Y + MeasureUnitsConverter.ToPixels(translateoffset, (view.Page as DiagramPage).MeasurementUnits));
                                    //(child as LineConnector).EndPointPosition = new Point((child as LineConnector).EndPointPosition.X, (child as LineConnector).EndPointPosition.Y + MeasureUnitsConverter.ToPixels(translateoffset, (view.Page as DiagramPage).MeasurementUnits));
                                    (child as LineConnector).PxStartPointPosition = new Point((child as LineConnector).PxStartPointPosition.X, (child as LineConnector).PxStartPointPosition.Y + translateoffset);
                                    (child as LineConnector).PxEndPointPosition = new Point((child as LineConnector).PxEndPointPosition.X, (child as LineConnector).PxEndPointPosition.Y + translateoffset);
                                    (child as LineConnector).InvalidateConnectorPathGeometry();
                                }
                            }
                        }
                        else if (tempList[i] is Node)
                        {
                            (tempList[i] as Node).PxOffsetY = topvalue + (i * refvalue);
                        }
                        else if (tempList[i] is LineConnector && (tempList[i] as LineConnector).HeadNode == null && (tempList[i] as LineConnector).TailNode == null)
                        {
                            if ((tempList[i] as LineConnector).PxStartPointPosition.Y < (tempList[i] as LineConnector).PxEndPointPosition.Y)
                            {
                                double oldy = (tempList[i] as LineConnector).PxStartPointPosition.Y;
                                (tempList[i] as LineConnector).PxStartPointPosition = new Point((tempList[i] as LineConnector).PxStartPointPosition.X, topvalue + (i * refvalue));
                                double offset = (tempList[i] as LineConnector).PxStartPointPosition.Y - oldy;
                                (tempList[i] as LineConnector).PxEndPointPosition = new Point((tempList[i] as LineConnector).PxEndPointPosition.X, (tempList[i] as LineConnector).PxEndPointPosition.Y + offset);
                                (tempList[i] as LineConnector).InvalidateConnectorPathGeometry();
                            }
                            else
                            {
                                double oldy = (tempList[i] as LineConnector).PxEndPointPosition.Y;
                                (tempList[i] as LineConnector).PxEndPointPosition = new Point((tempList[i] as LineConnector).PxEndPointPosition.X, topvalue + (i * refvalue));
                                double offset = (tempList[i] as LineConnector).PxEndPointPosition.Y - oldy;
                                (tempList[i] as LineConnector).PxStartPointPosition = new Point((tempList[i] as LineConnector).PxStartPointPosition.X, (tempList[i] as LineConnector).PxStartPointPosition.Y + offset);
                                (tempList[i] as LineConnector).InvalidateConnectorPathGeometry();
                            }
                        }
                    }
                }
            }            
        }

        /// <summary>
        /// Invoked when the SpaceAcross Command is executed.
        /// </summary>
        /// <param name="sender">The DiagramView.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        public static void OnSpaceAcrossCommand(object sender, ExecutedRoutedEventArgs e)
        {
             DiagramView view = sender as DiagramView;
             IDiagramCommandParameter param = e.Parameter as IDiagramCommandParameter;
             List<ICommon> tempList = new List<ICommon>();
             if (e.Parameter is IDiagramCommandParameter)
             {
                 switch (param.SelectionFilter)
                 {
                     case SelectionFilter.FilterLines:
                         {
                             foreach (Node n in view.SelectionList.OfType<Node>())
                             {
                                 tempList.Add(n);
                             }
                             OnSpaceAcrossFilter(tempList);
                             (view.Page as DiagramPage).InvalidateMeasure();
                             (view.Page as DiagramPage).InvalidateArrange();
                             break;
                         }
                     case SelectionFilter.FilterNodes:
                         {
                             foreach (LineConnector l in view.SelectionList.OfType<LineConnector>())
                             {
                                 tempList.Add(l);                                 
                             }
                             OnSpaceAcrossFilter(tempList);
                             (view.Page as DiagramPage).InvalidateMeasure();
                             (view.Page as DiagramPage).InvalidateArrange();
                             break;
                         }
                     default:
                         {
                             OnSpaceAcrossFilter(view.SelectionList.OfType<ICommon>().ToList());
                             (view.Page as DiagramPage).InvalidateMeasure();
                             (view.Page as DiagramPage).InvalidateArrange();
                             break;
                         }
                 }
             }
             else
             {
                 OnSpaceAcrossFilter(view.SelectionList.OfType<ICommon>().ToList());
                 (view.Page as DiagramPage).InvalidateMeasure();
                 (view.Page as DiagramPage).InvalidateArrange();
             }

        }

        private static void OnSpaceAcrossFilter(List<ICommon> tempList)
        {
            bool donotexecute = false;
            double leftvalue = 0;
            double rightvalue = 0;
            if (tempList.Count > 2)
            {
                if (tempList[0] is Node)
                {
                    leftvalue = (tempList[0] as Node).PxOffsetX;
                }
                else if (tempList[0] is LineConnector && (tempList[0] as LineConnector).HeadNode == null && (tempList[0] as LineConnector).TailNode == null)
                {
                    if ((tempList[0] as LineConnector).PxStartPointPosition.X < (tempList[0] as LineConnector).PxEndPointPosition.X)
                    {
                        //leftvalue = MeasureUnitsConverter.FromPixels((tempList[0] as LineConnector).StartPointPosition.X, (view.Page as DiagramPage).MeasurementUnits);
                        leftvalue = (tempList[0] as LineConnector).PxStartPointPosition.X;
                        (tempList[0] as LineConnector).InvalidateConnectorPathGeometry();
                    }
                    else
                    {
                        //leftvalue = MeasureUnitsConverter.FromPixels((tempList[0] as LineConnector).EndPointPosition.X, (view.Page as DiagramPage).MeasurementUnits);
                        leftvalue = (tempList[0] as LineConnector).PxEndPointPosition.X;
                        (tempList[0] as LineConnector).InvalidateConnectorPathGeometry();
                    }
                }
                else
                {
                    donotexecute = true;
                }

                if (tempList[tempList.Count - 1] is Node)
                {
                    rightvalue = (tempList[tempList.Count - 1] as Node).PxOffsetX;
                }
                else if (tempList[tempList.Count - 1] is LineConnector && (tempList[tempList.Count - 1] as LineConnector).HeadNode == null && (tempList[tempList.Count - 1] as LineConnector).TailNode == null)
                {
                    if ((tempList[tempList.Count - 1] as LineConnector).PxStartPointPosition.X < (tempList[tempList.Count - 1] as LineConnector).PxEndPointPosition.X)
                    {
                        //rightvalue = MeasureUnitsConverter.FromPixels((tempList[tempList.Count - 1] as LineConnector).StartPointPosition.X, (view.Page as DiagramPage).MeasurementUnits);
                        rightvalue = (tempList[tempList.Count - 1] as LineConnector).PxStartPointPosition.X;
                        (tempList[tempList.Count - 1] as LineConnector).InvalidateConnectorPathGeometry();
                    }
                    else
                    {
                        //rightvalue = MeasureUnitsConverter.FromPixels((tempList[view.SelectionList.Count - 1] as LineConnector).EndPointPosition.X, (view.Page as DiagramPage).MeasurementUnits);
                        rightvalue = (tempList[tempList.Count - 1] as LineConnector).PxEndPointPosition.X;
                        (tempList[tempList.Count - 1] as LineConnector).InvalidateConnectorPathGeometry();
                    }
                }
                else
                {
                    donotexecute = true;
                }

                if (!donotexecute)
                {
                    double count = tempList.Count - 1;
                    double refvalue = (rightvalue - leftvalue) / count;
                    for (int i = 1; i < tempList.Count - 1; i++)
                    {
                        if (tempList[i] is Group)
                        {
                            double oldx = (tempList[i] as Group).PxOffsetX;
                            (tempList[i] as Group).PxOffsetX = leftvalue + (i * refvalue);
                            double translateoffset = (tempList[i] as Group).PxOffsetX - oldx;
                            foreach (INodeGroup child in (tempList[i] as Group).NodeChildren)
                            {
                                if (child is Node)
                                {
                                    (child as Node).PxOffsetX += translateoffset;
                                }
                                else if (child is LineConnector && (child as LineConnector).HeadNode == null && (child as LineConnector).TailNode == null)
                                {
                                    //(child as LineConnector).StartPointPosition = new Point((child as LineConnector).StartPointPosition.X + MeasureUnitsConverter.ToPixels(translateoffset, (view.Page as DiagramPage).MeasurementUnits), (child as LineConnector).StartPointPosition.Y);
                                    //(child as LineConnector).EndPointPosition = new Point((child as LineConnector).EndPointPosition.X + MeasureUnitsConverter.ToPixels(translateoffset, (view.Page as DiagramPage).MeasurementUnits), (child as LineConnector).EndPointPosition.Y);
                                    (child as LineConnector).PxStartPointPosition = new Point((child as LineConnector).PxStartPointPosition.X + translateoffset, (child as LineConnector).PxStartPointPosition.Y);
                                    (child as LineConnector).PxEndPointPosition = new Point((child as LineConnector).PxEndPointPosition.X + translateoffset, (child as LineConnector).PxEndPointPosition.Y);
                                    (child as LineConnector).InvalidateConnectorPathGeometry();
                                }
                            }
                        }
                        else if (tempList[i] is Node)
                        {
                            (tempList[i] as Node).PxOffsetX = leftvalue + (i * refvalue);
                        }
                        else if (tempList[i] is LineConnector && (tempList[i] as LineConnector).HeadNode == null && (tempList[i] as LineConnector).TailNode == null)
                        {
                            if ((tempList[i] as LineConnector).PxStartPointPosition.X < (tempList[i] as LineConnector).PxEndPointPosition.X)
                            {
                                double oldx = (tempList[i] as LineConnector).PxStartPointPosition.X;
                                (tempList[i] as LineConnector).PxStartPointPosition = new Point(leftvalue + (i * refvalue), (tempList[i] as LineConnector).PxStartPointPosition.Y);
                                double offset = (tempList[i] as LineConnector).PxStartPointPosition.X - oldx;
                                (tempList[i] as LineConnector).PxEndPointPosition = new Point((tempList[i] as LineConnector).PxEndPointPosition.X + offset, (tempList[i] as LineConnector).PxEndPointPosition.Y);
                                (tempList[i] as LineConnector).InvalidateConnectorPathGeometry();
                            }
                            else
                            {
                                double oldx = (tempList[i] as LineConnector).PxEndPointPosition.X;
                                (tempList[i] as LineConnector).PxEndPointPosition = new Point(leftvalue + (i * refvalue), (tempList[i] as LineConnector).PxEndPointPosition.Y);
                                double offset = (tempList[i] as LineConnector).PxEndPointPosition.X - oldx;
                                (tempList[i] as LineConnector).PxStartPointPosition = new Point((tempList[i] as LineConnector).PxStartPointPosition.X + offset, (tempList[i] as LineConnector).PxStartPointPosition.Y);
                                (tempList[i] as LineConnector).InvalidateConnectorPathGeometry();
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region Size Commands

        /// <summary>
        /// Invoked when the SameSize Command is executed.
        /// </summary>
        /// <param name="sender">The DiagramView.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        public static void OnSameSizeCommand(object sender, ExecutedRoutedEventArgs e)
        {
            bool donotexecute = false;
            double widthrefvalue = 0;
            double heightrefvalue = 0;
            DiagramView view = sender as DiagramView;
            if (view.SelectionList.Count > 1)
            {
                if (view.SelectionList[0] is Node)
                {
                    widthrefvalue = (view.SelectionList[0] as Node).Width;
                    heightrefvalue = (view.SelectionList[0] as Node).Height;
                }
                else if (view.SelectionList[0] is LineConnector && (view.SelectionList[0] as LineConnector).HeadNode == null && (view.SelectionList[0] as LineConnector).TailNode == null)
                {
                    widthrefvalue = Math.Abs((view.SelectionList[0] as LineConnector).PxEndPointPosition.X - (view.SelectionList[0] as LineConnector).PxStartPointPosition.X);
                    heightrefvalue = Math.Abs((view.SelectionList[0] as LineConnector).PxEndPointPosition.Y - (view.SelectionList[0] as LineConnector).PxStartPointPosition.Y);
                }
                else
                {
                    donotexecute = true;
                }
            }
            else
            {
                donotexecute = true;
            }

            if (!donotexecute)
            {
                foreach (ICommon shape in view.SelectionList)
                {
                    if (shape is Group && (view.SelectionList[0] != (shape as Group)))
                    {
                        foreach (INodeGroup child in (shape as Group).NodeChildren)
                        {
                            if (child is Node)
                            {
                                (child as Node).Oldsize = new Size((child as Node).Width, (child as Node).Height);
                                (child as Node).Width = widthrefvalue;
                                (child as Node).Height = heightrefvalue;
                                Size m_newsize = new Size((child as Node).Width, (child as Node).Height);
                                foreach (ConnectionPort port in (child as Node).Ports)
                                {
                                    if ((child as Node).Oldsize.Width != 0 && (child as Node).Oldsize.Height != 0)
                                    {
                                        port.PreviousPortPoint = new Point(port.Left, port.Top);
                                        port.Left = (m_newsize.Width / (child as Node).Oldsize.Width) * port.PreviousPortPoint.X;
                                        port.Top = (m_newsize.Height / (child as Node).Oldsize.Height) * port.PreviousPortPoint.Y;
                                        TranslateTransform tr = new TranslateTransform(port.Left, port.Top);
                                        port.RenderTransform = tr;
                                    }
                                }
                            }
                            else if (child is LineConnector && (child as LineConnector).HeadNode == null && (child as LineConnector).TailNode == null)
                            {
                                double linewidth = Math.Abs((child as LineConnector).PxEndPointPosition.X - (child as LineConnector).PxStartPointPosition.X);
                                double extrawidth = widthrefvalue - linewidth;
                                double lineheight = Math.Abs((child as LineConnector).PxEndPointPosition.Y - (child as LineConnector).PxStartPointPosition.Y);
                                double extraheight = heightrefvalue - lineheight;
                                if ((child as LineConnector).PxStartPointPosition.X < (child as LineConnector).PxEndPointPosition.X)
                                {
                                    (child as LineConnector).PxEndPointPosition = new Point((child as LineConnector).PxEndPointPosition.X + extrawidth, (child as LineConnector).PxEndPointPosition.Y + extraheight);
                                }
                                else
                                {
                                    (child as LineConnector).PxStartPointPosition = new Point((child as LineConnector).PxStartPointPosition.X + extrawidth, (child as LineConnector).PxStartPointPosition.Y + extraheight);
                                }
                                (child as LineConnector).InvalidateConnectorPathGeometry();
                            }
                        }
                    }
                    else if (shape is Node)
                    {
                        (shape as Node).Oldsize = new Size((shape as Node).Width, (shape as Node).Height);
                        (shape as Node).Width = widthrefvalue;
                        (shape as Node).Height = heightrefvalue;
                        IDiagramPage mPage = VisualTreeHelper.GetParent(shape as Node) as IDiagramPage;
                        IEnumerable<Node> selectedNodes = mPage.SelectionList.OfType<Node>();
                        Point oldposition = new Point((shape as Node).PxOffsetX, (shape as Node).PxOffsetY);
                        foreach (ConnectionPort port in (shape as Node).Ports)
                        {
                            port.PreviousPortPoint = new Point(port.Left, port.Top);
                            if (view != null && view.UndoRedoEnabled && selectedNodes.Count() > 0)
                            {
                                view.UndoStack.Push(port);
                                view.UndoStack.Push(port.PreviousPortPoint);
                            }
                        }
                        view.UndoStack.Push(shape as Node);
                        view.UndoStack.Push((shape as Node).Oldsize);
                        view.UndoStack.Push(oldposition);
                        view.UndoStack.Push(selectedNodes.Count());
                        view.UndoStack.Push("Resized");

                        Size m_newsize = new Size((shape as Node).Width, (shape as Node).Height);
                        foreach (ConnectionPort port in (shape as Node).Ports)
                        {
                            if ((shape as Node).Oldsize.Width != 0 && (shape as Node).Oldsize.Height != 0)
                            {
                                port.PreviousPortPoint = new Point(port.Left, port.Top);
                                port.Left = (m_newsize.Width / (shape as Node).Oldsize.Width) * port.PreviousPortPoint.X;
                                port.Top = (m_newsize.Height / (shape as Node).Oldsize.Height) * port.PreviousPortPoint.Y;
                                TranslateTransform tr = new TranslateTransform(port.Left, port.Top);
                                port.RenderTransform = tr;
                            }
                        }
                    }
                    else if (shape is LineConnector && (shape as LineConnector).HeadNode == null && (shape as LineConnector).TailNode == null)
                    {
                        double linewidth = Math.Abs((shape as LineConnector).PxEndPointPosition.X - (shape as LineConnector).PxStartPointPosition.X);
                        double extrawidth = widthrefvalue - linewidth;
                        double lineheight = Math.Abs((shape as LineConnector).PxEndPointPosition.Y - (shape as LineConnector).PxStartPointPosition.Y);
                        double extraheight = heightrefvalue - lineheight;
                        if ((shape as LineConnector).PxStartPointPosition.X < (shape as LineConnector).PxEndPointPosition.X)
                        {
                            (shape as LineConnector).PxEndPointPosition = new Point((shape as LineConnector).PxEndPointPosition.X + extrawidth, (shape as LineConnector).PxEndPointPosition.Y + extraheight);
                        }
                        else
                        {
                            (shape as LineConnector).PxStartPointPosition = new Point((shape as LineConnector).PxStartPointPosition.X + extrawidth, (shape as LineConnector).PxStartPointPosition.Y + extraheight);
                        }
                        (shape as LineConnector).InvalidateConnectorPathGeometry();
                    }
                }
            }

            (view.Page as DiagramPage).InvalidateMeasure();
            (view.Page as DiagramPage).InvalidateArrange();
        }

        /// <summary>
        /// Invoked when the SameWidth Command is executed.
        /// </summary>
        /// <param name="sender">The DiagramView.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        public static void OnSameWidthCommand(object sender, ExecutedRoutedEventArgs e)
        {
            bool donotexecute = false;
            double refvalue = 0;
            DiagramView view = sender as DiagramView;
            if (view.SelectionList.Count > 1)
            {
                if (view.SelectionList[0] is Node)
                {
                    refvalue = (view.SelectionList[0] as Node).Width;
                }
                else if (view.SelectionList[0] is LineConnector && (view.SelectionList[0] as LineConnector).HeadNode == null && (view.SelectionList[0] as LineConnector).TailNode == null)
                {
                    refvalue = Math.Abs((view.SelectionList[0] as LineConnector).PxEndPointPosition.X - (view.SelectionList[0] as LineConnector).PxStartPointPosition.X);
                }
                else
                {
                    donotexecute = true;
                }
            }
            else
            {
                donotexecute = true;
            }

            if (!donotexecute)
            {
                foreach (ICommon shape in view.SelectionList)
                {
                    if (shape is Group && (view.SelectionList[0] != (shape as Group)))
                    {
                        foreach (INodeGroup child in (shape as Group).NodeChildren)
                        {
                            if (child is Node)
                            {
                                (child as Node)._sizeCommandsApplied = true;
                                (child as Node).Oldsize = new Size((child as Node).Width, (child as Node).Height);
                                (child as Node).Width = refvalue;
                                Size m_newsize = new Size((child as Node).Width, (child as Node).Height);
                                foreach (ConnectionPort port in (child as Node).Ports)
                                {
                                    if ((child as Node).Oldsize.Width != 0 && (child as Node).Oldsize.Height != 0)
                                    {
                                        port.PreviousPortPoint = new Point(port.Left, port.Top);
                                        port.Left = (m_newsize.Width / (child as Node).Oldsize.Width) * port.PreviousPortPoint.X;
                                        port.Top = (m_newsize.Height / (child as Node).Oldsize.Height) * port.PreviousPortPoint.Y;
                                        TranslateTransform tr = new TranslateTransform(port.Left - port.ActualWidth / 2, port.Top - port.ActualHeight / 2);
                                        port.RenderTransform = tr;
                                    }
                                }
                            }
                            else if (child is LineConnector && (child as LineConnector).HeadNode == null && (child as LineConnector).TailNode == null)
                            {
                                double linewidth = Math.Abs((child as LineConnector).PxEndPointPosition.X - (child as LineConnector).PxStartPointPosition.X);
                                double extrasize = refvalue - linewidth;
                                if ((child as LineConnector).PxStartPointPosition.X < (child as LineConnector).PxEndPointPosition.X)
                                {
                                    (child as LineConnector).PxEndPointPosition = new Point((child as LineConnector).PxEndPointPosition.X + extrasize, (child as LineConnector).PxEndPointPosition.Y);
                                }
                                else
                                {
                                    (child as LineConnector).PxStartPointPosition = new Point((child as LineConnector).PxStartPointPosition.X + extrasize, (child as LineConnector).PxStartPointPosition.Y);
                                }
                                (child as LineConnector).InvalidateConnectorPathGeometry();
                            }
                        }
                    }
                    else if (shape is Node)
                    {
                        (shape as Node)._sizeCommandsApplied = true;
                        (shape as Node).Oldsize = new Size((shape as Node).Width, (shape as Node).Height);
                        (shape as Node).Width = refvalue;
                        IDiagramPage mPage = VisualTreeHelper.GetParent(shape as Node) as IDiagramPage;
                        IEnumerable<Node> selectedNodes = mPage.SelectionList.OfType<Node>();
                        Point oldposition = new Point((shape as Node).PxOffsetX, (shape as Node).PxOffsetY);
                        foreach (ConnectionPort port in (shape as Node).Ports)
                        {
                            port.PreviousPortPoint = new Point(port.Left, port.Top);
                            if (view != null && view.UndoRedoEnabled && selectedNodes.Count() > 0)
                            {
                                view.UndoStack.Push(port);
                                view.UndoStack.Push(port.PreviousPortPoint);
                            }
                        }
                        view.UndoStack.Push(shape as Node);
                        view.UndoStack.Push((shape as Node).Oldsize);
                        view.UndoStack.Push(oldposition);
                        view.UndoStack.Push(selectedNodes.Count());
                        view.UndoStack.Push("Resized");
                        Size m_newsize = new Size((shape as Node).Width, (shape as Node).Height);
                        foreach (ConnectionPort port in (shape as Node).Ports)
                        {
                            if ((shape as Node).Oldsize.Width != 0 && (shape as Node).Oldsize.Height != 0)
                            {
                                port.PreviousPortPoint = new Point(port.Left, port.Top);
                                port.Left = (m_newsize.Width / (shape as Node).Oldsize.Width) * port.PreviousPortPoint.X;
                                port.Top = (m_newsize.Height / (shape as Node).Oldsize.Height) * port.PreviousPortPoint.Y;
                                TranslateTransform tr = new TranslateTransform(port.Left - port.ActualWidth / 2, port.Top - port.ActualHeight / 2);
                                port.RenderTransform = tr;
                            }
                        }
                    }
                    else if (shape is LineConnector && (shape as LineConnector).HeadNode == null && (shape as LineConnector).TailNode == null)
                    {
                        double linewidth = Math.Abs((shape as LineConnector).PxEndPointPosition.X - (shape as LineConnector).PxStartPointPosition.X);
                        double extrasize = refvalue - linewidth;
                        if ((shape as LineConnector).PxStartPointPosition.X < (shape as LineConnector).PxEndPointPosition.X)
                        {
                            (shape as LineConnector).PxEndPointPosition = new Point((shape as LineConnector).PxEndPointPosition.X + extrasize, (shape as LineConnector).PxEndPointPosition.Y);
                        }
                        else
                        {
                            (shape as LineConnector).PxStartPointPosition = new Point((shape as LineConnector).PxStartPointPosition.X + extrasize, (shape as LineConnector).PxStartPointPosition.Y);
                        }
                        (shape as LineConnector).InvalidateConnectorPathGeometry();
                    }
                }
            }

            (view.Page as DiagramPage).InvalidateMeasure();
            (view.Page as DiagramPage).InvalidateArrange();
        }

        /// <summary>
        /// Invoked when the SameHeight Command is executed.
        /// </summary>
        /// <param name="sender">The DiagramView.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        public static void OnSameHeightCommand(object sender, ExecutedRoutedEventArgs e)
        {
            bool donotexecute = false;
            double refvalue = 0;
            DiagramView view = sender as DiagramView;
            if (view.SelectionList.Count > 1)
            {
                if (view.SelectionList[0] is Node)
                {
                    refvalue = (view.SelectionList[0] as Node).Height;
                }
                else if (view.SelectionList[0] is LineConnector && (view.SelectionList[0] as LineConnector).HeadNode == null && (view.SelectionList[0] as LineConnector).TailNode == null)
                {
                    refvalue = Math.Abs((view.SelectionList[0] as LineConnector).PxEndPointPosition.Y - (view.SelectionList[0] as LineConnector).PxStartPointPosition.Y);
                }
                else
                {
                    donotexecute = true;
                }
            }
            else
            {
                donotexecute = true;
            }

            if (!donotexecute)
            {
                foreach (ICommon shape in view.SelectionList)
                {
                    if (shape is Group && (view.SelectionList[0] != (shape as Group)))
                    {
                        foreach (INodeGroup child in (shape as Group).NodeChildren)
                        {
                            if (child is Node)
                            {
                                (child as Node)._sizeCommandsApplied = true;
                                (child as Node).Oldsize = new Size((child as Node).Width, (child as Node).Height);
                                (child as Node).Height = refvalue;
                                Size m_newsize = new Size((child as Node).Width, (child as Node).Height);
                                foreach (ConnectionPort port in (child as Node).Ports)
                                {
                                    if ((child as Node).Oldsize.Width != 0 && (child as Node).Oldsize.Height != 0)
                                    {
                                        port.PreviousPortPoint = new Point(port.Left, port.Top);
                                        port.Left = (m_newsize.Width / (child as Node).Oldsize.Width) * port.PreviousPortPoint.X;
                                        port.Top = (m_newsize.Height / (child as Node).Oldsize.Height) * port.PreviousPortPoint.Y;
                                        TranslateTransform tr = new TranslateTransform(port.Left - port.ActualWidth / 2, port.Top - port.ActualHeight / 2);
                                        port.RenderTransform = tr;
                                    }
                                }
                            }
                            else if (child is LineConnector && (child as LineConnector).HeadNode == null && (child as LineConnector).TailNode == null)
                            {
                                double lineheight = Math.Abs((child as LineConnector).PxEndPointPosition.Y - (child as LineConnector).PxStartPointPosition.Y);
                                double extrasize = refvalue - lineheight;
                                if ((child as LineConnector).PxStartPointPosition.Y < (child as LineConnector).PxEndPointPosition.Y)
                                {
                                    (child as LineConnector).PxEndPointPosition = new Point((child as LineConnector).PxEndPointPosition.X, (child as LineConnector).PxEndPointPosition.Y + extrasize);
                                }
                                else
                                {
                                    (child as LineConnector).PxStartPointPosition = new Point((child as LineConnector).PxStartPointPosition.X, (child as LineConnector).PxStartPointPosition.Y + extrasize);
                                }
                                (child as LineConnector).InvalidateConnectorPathGeometry();
                            }
                        }
                    }
                    else if (shape is Node)
                    {
                        (shape as Node).Oldsize = new Size((shape as Node).Width, (shape as Node).Height);
                        (shape as Node).Height = refvalue;
                        IDiagramPage mPage = VisualTreeHelper.GetParent(shape as Node) as IDiagramPage;
                        IEnumerable<Node> selectedNodes = mPage.SelectionList.OfType<Node>();

                        Point oldposition = new Point((shape as Node).PxOffsetX, (shape as Node).PxOffsetY);
                        foreach (ConnectionPort port in (shape as Node).Ports)
                        {
                            port.PreviousPortPoint = new Point(port.Left, port.Top);
                            if (view != null && view.UndoRedoEnabled && selectedNodes.Count() > 0)
                            {
                                view.UndoStack.Push(port);
                                view.UndoStack.Push(port.PreviousPortPoint);
                            }
                        }
                        (shape as Node)._sizeCommandsApplied = true;
                        view.UndoStack.Push(shape as Node);
                        view.UndoStack.Push((shape as Node).Oldsize);
                        view.UndoStack.Push(oldposition);
                        view.UndoStack.Push(selectedNodes.Count());
                        view.UndoStack.Push("Resized");

                        Size m_newsize = new Size((shape as Node).Width, (shape as Node).Height);
                        foreach (ConnectionPort port in (shape as Node).Ports)
                        {
                            if ((shape as Node).Oldsize.Width != 0 && (shape as Node).Oldsize.Height != 0)
                            {
                                port.PreviousPortPoint = new Point(port.Left, port.Top);
                                port.Left = (m_newsize.Width / (shape as Node).Oldsize.Width) * port.PreviousPortPoint.X;
                                port.Top = (m_newsize.Height / (shape as Node).Oldsize.Height) * port.PreviousPortPoint.Y;
                                TranslateTransform tr = new TranslateTransform(port.Left - port.ActualWidth / 2, port.Top - port.ActualHeight / 2);
                                port.RenderTransform = tr;
                            }
                        }
                    }
                    else if (shape is LineConnector && (shape as LineConnector).HeadNode == null && (shape as LineConnector).TailNode == null)
                    {
                        double lineheight = Math.Abs((shape as LineConnector).PxEndPointPosition.Y - (shape as LineConnector).PxStartPointPosition.Y);
                        double extrasize = refvalue - lineheight;
                        if ((shape as LineConnector).PxStartPointPosition.X < (shape as LineConnector).PxEndPointPosition.X)
                        {
                            (shape as LineConnector).PxEndPointPosition = new Point((shape as LineConnector).PxEndPointPosition.X, (shape as LineConnector).PxEndPointPosition.Y + extrasize);
                        }
                        else
                        {
                            (shape as LineConnector).PxStartPointPosition = new Point((shape as LineConnector).PxStartPointPosition.X, (shape as LineConnector).PxStartPointPosition.Y + extrasize);
                        }
                        (shape as LineConnector).InvalidateConnectorPathGeometry();
                    }
                }
            }

            (view.Page as DiagramPage).InvalidateMeasure();
            (view.Page as DiagramPage).InvalidateArrange();
        }

        #endregion

        #region Undo/Redo Commands

        /// <summary>
        /// Invoked when the UndoCommand  is executed.
        /// </summary>
        /// <param name="sender">The DiagramView.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        public static void OnUndoCommand(object sender, ExecutedRoutedEventArgs e)
        {
            DiagramCommandManager.UndoCommand(sender as DiagramView);
        }

        /// <summary>
        /// Invoked when the RedoCommand  is executed.
        /// </summary>
        /// <param name="sender">The DiagramView.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        public static void OnRedoCommand(object sender, ExecutedRoutedEventArgs e)
        {
            DiagramCommandManager.RedoCommand(sender as DiagramView);
        }

        #endregion

        #endregion
    }
}
