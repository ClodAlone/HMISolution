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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.ComponentModel;
using System.Windows.Media;
using System.Globalization;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
#if SILVERLIGHT
using System.Threading;
using Syncfusion.Tools.Controls.Navigation;
#else
using System.Data;
using Syncfusion.Windows.Controls.Navigation;
using System.Diagnostics;
#endif

namespace Syncfusion.UI.Xaml.Schedule
{
    #region SfSchedule

    /// <summary>
    /// Represents the Schedule.
    /// </summary>
    public class SfSchedule : Control, IDisposable
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.UI.Xaml.Schedule.SfSchedule">SfSchedule</see> class. 
        /// </summary>
        public SfSchedule()
        {
            MouseRightButtonDown += SfSchedule_MouseRightButtonDown;
            ScheduleTimeLineItemsControl.MinValue = 0;
            ScheduleTimeLineItemsControl.MaxValue = 24;
#if SILVERLIGHT
            CommandBinding = new CommandHelper(this);
#endif
            DefaultStyleKey = typeof(SfSchedule);
#if WPF
            System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Critical;
#endif
            KeyUp += SfSchedule_KeyUp;
            reminderTimer = new DispatcherTimer();
            Reminder = new ScheduleReminderControl { schedule = this };
            Unloaded += SfSchedule_Unloaded;
            reminderTimer.Tick += reminderTimer_Tick;
            ScheduleResourceTypeCollection = new ObservableCollection<ResourceType>();
            ScheduleResourceTypeCollection.CollectionChanged += ScheduleResourceTypeCollection_CollectionChanged;
            Loaded += Schedule_Loaded;
            AppointmentStatusCollection = new ScheduleAppointmentStatusCollection { new ScheduleAppointmentStatus { Brush = new SolidColorBrush(Colors.Green), Status = "Free" }, new ScheduleAppointmentStatus { Brush = new SolidColorBrush(Colors.Orange), Status = "Tentative" }, new ScheduleAppointmentStatus { Brush = new SolidColorBrush(Colors.Red), Status = "Busy" }, new ScheduleAppointmentStatus { Brush = new SolidColorBrush(Colors.Black), Status = "Out Of Office" } };
            TimeZoneCollection = GetTimeZones();
            SelectedDates = new ObservableCollection<DateTime> { DateTime.Now.Date };
            CurrentVisibleSelectedDates = SelectedDates;
            NonWorkingDateCollection = new ObservableCollection<DayOfWeek> { DayOfWeek.Saturday, DayOfWeek.Sunday };
            Appointments = new ScheduleAppointmentCollection();
            RecursiveAppointments = new ScheduleAppointmentCollection();
            NonAccessibleBlocks = new NonAccessibleBlockCollection();
            NonAccessibleBlocks.CollectionChanged += NonAccessibleBlocks_CollectionChanged;
            Background = new SolidColorBrush(Color.FromArgb(100, 244, 244, 244));
            selectedResourcename = new List<Resource>();
            Dayviewrb = new ResizeBehavior
            {
                IsBottomDraggable = true,
                IsBottomLeftDraggable = false,
                IsBottomRightDraggable = false,
                IsLeftDraggable = false,
                IsRightDraggable = false,
                IsTopDraggable = true,
                IsTopLeftDraggable = false,
                IsTopRightDraggable = false,
                StayInParent = true
            };
            Monthviewrb = new ResizeBehavior
            {
                IsBottomDraggable = false,
                IsBottomLeftDraggable = false,
                IsBottomRightDraggable = false,
                IsLeftDraggable = true,
                IsRightDraggable = true,
                IsTopDraggable = false,
                IsTopLeftDraggable = false,
                IsTopRightDraggable = false,
                StayInParent = true
            };
            Timelineviewrb = new ResizeBehavior
            {
                IsBottomDraggable = false,
                IsBottomLeftDraggable = false,
                IsBottomRightDraggable = false,
                IsLeftDraggable = true,
                IsRightDraggable = true,
                IsTopDraggable = false,
                IsTopLeftDraggable = false,
                IsTopRightDraggable = false,
                StayInParent = true
            };

            #region KeyboardInteraction

            #region Delete
#if WPF
            KeyGesture deleteKey = new KeyGesture(Key.Delete);
            KeyBinding deleteKB = new KeyBinding(ScheduleCommands.Delete, deleteKey);
            this.InputBindings.Add(deleteKB);
            CommandBinding deleteCmd = new CommandBinding(ScheduleCommands.Delete, DeleteCmdExecuted, DeleteCmdCanExecute);
            this.CommandBindings.Add(deleteCmd);
#endif
#if SILVERLIGHT
            Command Delete = new Command();
            Delete.Text = "Delete";
            Delete.Key = Key.Delete;
            Delete.Execute += delegate() { SLDeleteCmdExecuted(this); };
            CommandBinding.AddCommand(Delete);
#endif
            #endregion

            #region Create

#if WPF
            KeyGesture CreateKey = new KeyGesture(Key.A, ModifierKeys.Control | ModifierKeys.Shift);
            KeyBinding CreateKB = new KeyBinding(ScheduleCommands.Create, CreateKey);
            this.InputBindings.Add(CreateKB);
            CommandBinding createCmd = new CommandBinding(ScheduleCommands.Create, CreateCmdExecuted, CreateCmdCanExecute);
            this.CommandBindings.Add(createCmd);
#endif
#if SILVERLIGHT
            Command Create = new Command();
            Create.Text = "Create";
            Create.Key = Key.A;
            Create.ModifierKeys = ModifierKeys.Control | ModifierKeys.Shift;
            Create.Execute += delegate() { SLCreateCmdExecuted(this); };
            CommandBinding.AddCommand(Create);

#endif
            #endregion

            #region Copy
#if WPF
            KeyGesture CopyKey = new KeyGesture(Key.C, ModifierKeys.Control);
            KeyBinding CopyKB = new KeyBinding(ScheduleCommands.Copy, CopyKey);
            this.InputBindings.Add(CopyKB);
            CommandBinding copyCmd = new CommandBinding(ScheduleCommands.Copy, CopyCmdExecuted, CopyCmdCanExecute);
            this.CommandBindings.Add(copyCmd);
#endif

#if SILVERLIGHT
            Command Copy = new Command();
            Copy.Text = "Copy";
            Copy.Key = Key.C;
            Copy.ModifierKeys = ModifierKeys.Control ;
            Copy.Execute += delegate() { SLCopyCmdExecuted(this); };
            CommandBinding.AddCommand(Copy);

#endif
            #endregion

            #region Paste
#if WPF
            KeyGesture PasteKey = new KeyGesture(Key.V, ModifierKeys.Control);
            KeyBinding PasteKB = new KeyBinding(ScheduleCommands.Paste, PasteKey);
            this.InputBindings.Add(PasteKB);
            CommandBinding pasteCmd = new CommandBinding(ScheduleCommands.Paste, PasteCmdExecuted, PasteCmdCanExecute);
            this.CommandBindings.Add(pasteCmd);
#endif
#if SILVERLIGHT
            Command Paste = new Command();
            Paste.Text = "Paste";
            Paste.Key = Key.V;
            Paste.ModifierKeys = ModifierKeys.Control ;
            Paste.Execute += delegate() { SLPasteCmdExecuted(this); };
            CommandBinding.AddCommand(Paste);

#endif
            #endregion

            #region Up
#if WPF
            KeyGesture UpKey = new KeyGesture(Key.Up);
            KeyBinding UpKB = new KeyBinding(ScheduleCommands.Up, UpKey);
            this.InputBindings.Add(UpKB);
            CommandBinding upCmd = new CommandBinding(ScheduleCommands.Up, UpCmdExecuted, UpCmdCanExecute);
            this.CommandBindings.Add(upCmd);
#endif
#if SILVERLIGHT
            Command Up = new Command();
            Up.Text = "Up";
            Up.Key = Key.Up;
            Up.Execute += delegate() { SLUpCmdExecuted(this); };
            CommandBinding.AddCommand(Up);

#endif
            #endregion

            #region Down
#if WPF
            KeyGesture DownKey = new KeyGesture(Key.Down);
            KeyBinding DownKB = new KeyBinding(ScheduleCommands.Down, DownKey);
            this.InputBindings.Add(DownKB);
            CommandBinding DownCmd = new CommandBinding(ScheduleCommands.Down, DownCmdExecuted, DownCmdCanExecute);
            this.CommandBindings.Add(DownCmd);
#endif
#if SILVERLIGHT
            Command Down = new Command();
            Down.Text = "Down";
            Down.Key = Key.Down;
            Down.Execute += delegate() { SLDownCmdExecuted(this); };
            CommandBinding.AddCommand(Down);

#endif
            #endregion

            #region Right
#if WPF
            KeyGesture RightKey = new KeyGesture(Key.Right);
            KeyBinding RightKB = new KeyBinding(ScheduleCommands.Right, RightKey);
            this.InputBindings.Add(RightKB);
            CommandBinding RightCmd = new CommandBinding(ScheduleCommands.Right, RightCmdExecuted, RightCmdCanExecute);
            this.CommandBindings.Add(RightCmd);
#endif
#if SILVERLIGHT
            Command Right = new Command();
            Right.Text = "Right";
            Right.Key = Key.Right;
            Right.Execute += delegate() { SLRightCmdExecuted(this); };
            CommandBinding.AddCommand(Right);

#endif
            #endregion

            #region Left
#if WPF
            KeyGesture LeftKey = new KeyGesture(Key.Left);
            KeyBinding LeftKB = new KeyBinding(ScheduleCommands.Left, LeftKey);
            this.InputBindings.Add(LeftKB);
            CommandBinding LeftCmd = new CommandBinding(ScheduleCommands.Left, LeftCmdExecuted, LeftCmdCanExecute);
            this.CommandBindings.Add(LeftCmd);
#endif
#if SILVERLIGHT
            Command Left = new Command();
            Left.Text = "Left";
            Left.Key = Key.Left;
            Left.Execute += delegate() { SLLeftCmdExecuted(this); };
            CommandBinding.AddCommand(Left);

#endif
            #endregion

            #region PgUp
#if WPF
            KeyGesture PgUpKey = new KeyGesture(Key.PageUp);
            KeyBinding PgUpKB = new KeyBinding(ScheduleCommands.PgUp, PgUpKey);
            this.InputBindings.Add(PgUpKB);
            CommandBinding PgupCmd = new CommandBinding(ScheduleCommands.PgUp, PgUpCmdExecuted, PgUpCmdCanExecute);
            this.CommandBindings.Add(PgupCmd);
#endif
#if SILVERLIGHT
            Command PgUp = new Command();
            PgUp.Text = "PgUp";
            PgUp.Key = Key.PageUp;
            PgUp.Execute += delegate() { SLPgUpCmdExecuted(this); };
            CommandBinding.AddCommand(PgUp);

#endif
            #endregion

            #region PgDown
#if WPF
            KeyGesture PgDownKey = new KeyGesture(Key.PageDown);
            KeyBinding PgDownKB = new KeyBinding(ScheduleCommands.PgDown, PgDownKey);
            this.InputBindings.Add(PgDownKB);
            CommandBinding PgDownCmd = new CommandBinding(ScheduleCommands.PgDown, PgDownCmdExecuted, PgDownCmdCanExecute);
            this.CommandBindings.Add(PgDownCmd);
#endif
#if SILVERLIGHT
            Command PgDown = new Command();
            PgDown.Text = "PgUp";
            PgDown.Key = Key.PageDown;
            PgDown.Execute += delegate() { SLPgDownCmdExecuted(this); };
            CommandBinding.AddCommand(PgDown);

#endif
            #endregion

            #region Home
#if WPF
            KeyGesture HomeKey = new KeyGesture(Key.Home);
            KeyBinding HomeKB = new KeyBinding(ScheduleCommands.Home, HomeKey);
            this.InputBindings.Add(RightKB);
            CommandBinding HomeCmd = new CommandBinding(ScheduleCommands.Home, HomeCmdExecuted, HomeCmdCanExecute);
            this.CommandBindings.Add(HomeCmd);
#endif
#if SILVERLIGHT
            Command Home = new Command();
            Home.Text = "Home";
            Home.Key = Key.Home;
            Home.Execute += delegate() { SLHomeCmdExecuted(this); };
            CommandBinding.AddCommand(Home);

#endif
            #endregion

            #region End
#if WPF
            KeyGesture EndKey = new KeyGesture(Key.End);
            KeyBinding EndKB = new KeyBinding(ScheduleCommands.End, EndKey);
            this.InputBindings.Add(EndKB);
            CommandBinding EndCmd = new CommandBinding(ScheduleCommands.End, EndCmdExecuted, EndCmdCanExecute);
            this.CommandBindings.Add(EndCmd);
#endif
#if SILVERLIGHT
            Command End = new Command();
            End.Text = "End";
            End.Key = Key.End;
            End.Execute += delegate() { SLEndCmdExecuted(this); };
            CommandBinding.AddCommand(End);

#endif
            #endregion


            #region FirstDayCurrentWeek
#if WPF
            KeyGesture FirstDayCurrentWeekKey = new KeyGesture(Key.Home, ModifierKeys.Alt);
            KeyBinding FirstDayCurrentWeekKB = new KeyBinding(ScheduleCommands.FirstDayCurrentWeek, FirstDayCurrentWeekKey);
            this.InputBindings.Add(FirstDayCurrentWeekKB);
            CommandBinding FirstDayCurrentWeekCmd = new CommandBinding(ScheduleCommands.FirstDayCurrentWeek, FirstDayCurrentWeekCmdExecuted, FirstDayCurrentWeekCmdCanExecute);
            this.CommandBindings.Add(FirstDayCurrentWeekCmd);
#endif
#if SILVERLIGHT
            Command FirstDayCurrentWeek = new Command();
            FirstDayCurrentWeek.Text = "FirstDayCurrentWeek";
            FirstDayCurrentWeek.Key = Key.F;
            FirstDayCurrentWeek.ModifierKeys = ModifierKeys.Shift;
            FirstDayCurrentWeek.Execute += delegate() { SLFirstDayCurrentWeekCmdExecuted(this); };
            CommandBinding.AddCommand(FirstDayCurrentWeek);

#endif
            #endregion

            #region LastDayCurrentWeek
#if WPF
            KeyGesture LastDayCurrentWeekKey = new KeyGesture(Key.End, ModifierKeys.Alt);
            KeyBinding LastDayCurrentWeekKB = new KeyBinding(ScheduleCommands.LastDayCurrentWeek, LastDayCurrentWeekKey);
            this.InputBindings.Add(LastDayCurrentWeekKB);
            CommandBinding LastDayCurrentWeekCmd = new CommandBinding(ScheduleCommands.LastDayCurrentWeek, LastDayCurrentWeekCmdExecuted, LastDayCurrentWeekCmdCanExecute);
            this.CommandBindings.Add(LastDayCurrentWeekCmd);
#endif
#if SILVERLIGHT
            Command LastDayCurrentWeek = new Command();
            LastDayCurrentWeek.Text = "LastDayCurrentWeek";
            LastDayCurrentWeek.Key = Key.L;
            LastDayCurrentWeek.ModifierKeys = ModifierKeys.Shift;
            LastDayCurrentWeek.Execute += delegate() { SLLastDayCurrentWeekCmdExecuted(this); };
            CommandBinding.AddCommand(LastDayCurrentWeek);

#endif
            #endregion

            #region SameDayPreviousWeek
#if WPF
            KeyGesture SameDayPreviousWeekKey = new KeyGesture(Key.Up, ModifierKeys.Alt);
            KeyBinding SameDayPreviousWeekKB = new KeyBinding(ScheduleCommands.SameDayPreviousWeek, SameDayPreviousWeekKey);
            this.InputBindings.Add(SameDayPreviousWeekKB);
            CommandBinding SameDayPreviousWeekCmd = new CommandBinding(ScheduleCommands.SameDayPreviousWeek, SameDayPreviousWeekCmdExecuted, SameDayPreviousWeekCmdCanExecute);
            this.CommandBindings.Add(SameDayPreviousWeekCmd);
#endif
#if SILVERLIGHT
            Command SameDayPreviousWeek = new Command();
            SameDayPreviousWeek.Text = "SameDayPreviousWeek";
            SameDayPreviousWeek.Key = Key.U;
            SameDayPreviousWeek.ModifierKeys = ModifierKeys.Shift;
            SameDayPreviousWeek.Execute += delegate() { SLSameDayPreviousWeekCmdExecuted(this); };
            CommandBinding.AddCommand(SameDayPreviousWeek);

#endif
            #endregion

            #region SameDayNextWeek
#if WPF
            KeyGesture SameDayNextWeekKey = new KeyGesture(Key.Down, ModifierKeys.Alt);
            KeyBinding SameDayNextWeekKB = new KeyBinding(ScheduleCommands.SameDayNextWeek, SameDayNextWeekKey);
            this.InputBindings.Add(SameDayNextWeekKB);
            CommandBinding SameDayNextWeekCmd = new CommandBinding(ScheduleCommands.SameDayNextWeek, SameDayNextWeekCmdExecuted, SameDayNextWeekCmdCanExecute);
            this.CommandBindings.Add(SameDayNextWeekCmd);
#endif
#if SILVERLIGHT
            Command SameDayNextWeek = new Command();
            SameDayNextWeek.Text = "SameDayNextWeek";
            SameDayNextWeek.Key = Key.D;
            SameDayNextWeek.ModifierKeys = ModifierKeys.Shift;
            SameDayNextWeek.Execute += delegate() { SLSameDayNextWeekCmdExecuted(this); };
            CommandBinding.AddCommand(SameDayNextWeek);

#endif
            #endregion

            #region ZoomIn
#if WPF
            KeyGesture ZoomInKey = new KeyGesture(Key.OemMinus, ModifierKeys.Alt);
            KeyBinding ZoomInKB = new KeyBinding(ScheduleCommands.ZoomIn, ZoomInKey);
            this.InputBindings.Add(ZoomInKB);
            CommandBinding ZoomInCmd = new CommandBinding(ScheduleCommands.ZoomIn, ZoomInCmdExecuted, ZoomInCmdCanExecute);
            this.CommandBindings.Add(ZoomInCmd);
#endif
#if SILVERLIGHT
            Command ZoomIn = new Command();
            ZoomIn.Text = "ZoomIn";
            ZoomIn.Key = Key.I;
            ZoomIn.ModifierKeys = ModifierKeys.Shift;
            ZoomIn.Execute += delegate() { SLZoomInCmdExecuted(this); };
            CommandBinding.AddCommand(ZoomIn);

#endif
            #endregion

            #region ZoomOut
#if WPF
            KeyGesture ZoomOutKey = new KeyGesture(Key.OemPlus, ModifierKeys.Alt);
            KeyBinding ZoomOutKB = new KeyBinding(ScheduleCommands.ZoomOut, ZoomOutKey);
            this.InputBindings.Add(ZoomOutKB);
            CommandBinding ZoomOutCmd = new CommandBinding(ScheduleCommands.ZoomOut, ZoomOutCmdExecuted, ZoomOutCmdCanExecute);
            this.CommandBindings.Add(ZoomOutCmd);
#endif
#if SILVERLIGHT
            Command ZoomOut = new Command();
            ZoomOut.Text = "ZoomOut";
            ZoomOut.Key = Key.O;
            ZoomOut.ModifierKeys = ModifierKeys.Shift;
            ZoomOut.Execute += delegate() { SLZoomOutCmdExecuted(this); };
            CommandBinding.AddCommand(ZoomOut);

#endif
            #endregion

            #region Previous Appointment
#if WPF
            KeyGesture PrevAppKey = new KeyGesture(Key.Tab, ModifierKeys.Shift);
            KeyBinding PrevAppKB = new KeyBinding(ScheduleCommands.PreviousAppointment, PrevAppKey);
            this.InputBindings.Add(PrevAppKB);
            CommandBinding PrevAppCmd = new CommandBinding(ScheduleCommands.PreviousAppointment, PreviousAppointmentCmdExecuted, PreviousAppointmentCmdCanExecute);
            this.CommandBindings.Add(PrevAppCmd);
#endif
#if SILVERLIGHT
            Command PreviousAppointment = new Command();
            PreviousAppointment.Text = "PreviousAppointment";
            PreviousAppointment.Key = Key.Tab;
            PreviousAppointment.ModifierKeys = ModifierKeys.Shift;
            PreviousAppointment.Execute += delegate() { SLPreviousAppointmentCmdExecuted(this); };
            CommandBinding.AddCommand(PreviousAppointment);

#endif
            #endregion

            #region Next Appointmetn
#if WPF
            KeyGesture NextAppKey = new KeyGesture(Key.Tab);
            KeyBinding NextAppKB = new KeyBinding(ScheduleCommands.NextAppointment, NextAppKey);
            this.InputBindings.Add(NextAppKB);
            CommandBinding NextAppCmd = new CommandBinding(ScheduleCommands.NextAppointment, NextAppointmentCmdExecuted, NextAppointmentCmdCanExecute);
            this.CommandBindings.Add(NextAppCmd);
#endif
#if SILVERLIGHT
            Command NextAppointment = new Command();
            NextAppointment.Text = "NextAppointment";
            NextAppointment.Key = Key.Tab;
            //NextAppointment.ModifierKeys = ModifierKeys.Shift;
            NextAppointment.Execute += delegate() { SLNextAppointmentCmdExecuted(this); };
            CommandBinding.AddCommand(NextAppointment);

#endif
            #endregion

            #region ResizeAppointmentUp
#if WPF
            KeyGesture ResizeAppointmentUpKey = new KeyGesture(Key.Up, ModifierKeys.Alt | ModifierKeys.Shift);
            KeyBinding ResizeAppointmentUpKB = new KeyBinding(ScheduleCommands.ResizeAppointmentUp, ResizeAppointmentUpKey);
            this.InputBindings.Add(ResizeAppointmentUpKB);
            CommandBinding ResizeAppointmentUpCmd = new CommandBinding(ScheduleCommands.ResizeAppointmentUp, ResizeAppointmentUpCmdExecuted, ResizeAppointmentUpCmdCanExecute);
            this.CommandBindings.Add(ResizeAppointmentUpCmd);
#endif
#if SILVERLIGHT
            Command ResizeAppointmentUp = new Command();
            ResizeAppointmentUp.Text = "ResizeAppointmentUp";
            ResizeAppointmentUp.Key = Key.S;
            ResizeAppointmentUp.ModifierKeys = ModifierKeys.Shift;
            ResizeAppointmentUp.Execute += delegate() { SLResizeAppointmentUpCmdExecuted(this); };
            CommandBinding.AddCommand(ResizeAppointmentUp);

#endif
            #endregion

            #region ResizeAppointmentDown
#if WPF
            KeyGesture ResizeAppointmentDownKey = new KeyGesture(Key.Down, ModifierKeys.Alt | ModifierKeys.Shift);
            KeyBinding ResizeAppointmentDownKB = new KeyBinding(ScheduleCommands.ResizeAppointmentDown, ResizeAppointmentDownKey);
            this.InputBindings.Add(ResizeAppointmentDownKB);
            CommandBinding ResizeAppointmentDownCmd = new CommandBinding(ScheduleCommands.ResizeAppointmentDown, ResizeAppointmentDownCmdExecuted, ResizeAppointmentDownCmdCanExecute);
            this.CommandBindings.Add(ResizeAppointmentDownCmd);
#endif
#if SILVERLIGHT
            Command ResizeAppointmentDown = new Command();
            ResizeAppointmentDown.Text = "ResizeAppointmentDown";
            ResizeAppointmentDown.Key = Key.E;
            ResizeAppointmentDown.ModifierKeys = ModifierKeys.Shift;
            ResizeAppointmentDown.Execute += delegate() { SLResizeAppointmentDownCmdExecuted(this); };
            CommandBinding.AddCommand(ResizeAppointmentDown);

#endif
            #endregion

            #region TimeSelectionUp
#if WPF
            KeyGesture TimeSelectionUpKey = new KeyGesture(Key.Up, ModifierKeys.Shift);
            KeyBinding TimeSelectionUpKB = new KeyBinding(ScheduleCommands.TimeSelectionUp, TimeSelectionUpKey);
            this.InputBindings.Add(TimeSelectionUpKB);
            CommandBinding TimeSelectionUpCmd = new CommandBinding(ScheduleCommands.TimeSelectionUp, TimeSelectionfUpCmdExecuted, TimeSelectionUpCmdCanExecute);
            this.CommandBindings.Add(TimeSelectionUpCmd);
#endif
#if SILVERLIGHT
            Command TimeSelectionUp = new Command();
            TimeSelectionUp.Text = "TimeSelectionUp";
            TimeSelectionUp.Key = Key.Up;
            TimeSelectionUp.ModifierKeys = ModifierKeys.Shift;
            TimeSelectionUp.Execute += delegate() { SLTimeSelectionfUpCmdExecuted(this); };
            CommandBinding.AddCommand(TimeSelectionUp);

#endif
            #endregion

            #region TimeSelectionDown
#if WPF
            KeyGesture TimeSelectionDownKey = new KeyGesture(Key.Down, ModifierKeys.Shift);
            KeyBinding TimeSelectionDownKB = new KeyBinding(ScheduleCommands.TimeSelectionDown, TimeSelectionDownKey);
            this.InputBindings.Add(TimeSelectionDownKB);
            CommandBinding TimeSelectionDownCmd = new CommandBinding(ScheduleCommands.TimeSelectionDown, TimeSelectionfDownCmdExecuted, TimeSelectionDownCmdCanExecute);
            this.CommandBindings.Add(TimeSelectionDownCmd);
#endif
#if SILVERLIGHT
            Command TimeSelectionDown = new Command();
            TimeSelectionDown.Text = "TimeSelectionDown";
            TimeSelectionDown.Key = Key.Down;
            TimeSelectionDown.ModifierKeys = ModifierKeys.Shift;
            TimeSelectionDown.Execute += delegate() { SLTimeSelectionfDownCmdExecuted(this); };
            CommandBinding.AddCommand(TimeSelectionDown);

#endif
            #endregion

            #region CreateAppOnSelection
#if WPF
            KeyGesture CreateAppOnSelectionKey = new KeyGesture(Key.Enter);
            KeyBinding CreateAppOnSelectionKB = new KeyBinding(ScheduleCommands.CreateAppOnMultiSelection, CreateAppOnSelectionKey);
            this.InputBindings.Add(CreateAppOnSelectionKB);
            CommandBinding CreateAppOnSelectionCmd = new CommandBinding(ScheduleCommands.CreateAppOnMultiSelection, CreateAppOnSelectionCmdExecuted, CreateAppOnSelectionCmdCanExecute);
            this.CommandBindings.Add(CreateAppOnSelectionCmd);
#endif
#if SILVERLIGHT
            Command CreateAppOnSelection = new Command();
            CreateAppOnSelection.Text = "CreateAppOnSelection";
            CreateAppOnSelection.Key = Key.Enter;
            //CreateAppOnSelection.ModifierKeys = ModifierKeys.Shift;
            CreateAppOnSelection.Execute += delegate() { SLCreateAppOnSelectionCmdExecuted(this); };
            CommandBinding.AddCommand(CreateAppOnSelection);

#endif
            #endregion

            #region TimeSelectionRight
#if WPF
            KeyGesture TimeSelectionRightKey = new KeyGesture(Key.Right, ModifierKeys.Shift);
            KeyBinding TimeSelectionRightKB = new KeyBinding(ScheduleCommands.TimeSelectionRight, TimeSelectionRightKey);
            this.InputBindings.Add(TimeSelectionRightKB);
            CommandBinding TimeSelectionRightCmd = new CommandBinding(ScheduleCommands.TimeSelectionRight, TimeSelectionRightCmdExecuted, TimeSelectionRightCmdCanExecute);
            this.CommandBindings.Add(TimeSelectionRightCmd);
#endif
#if SILVERLIGHT
            Command TimeSelectionRight = new Command();
            TimeSelectionRight.Text = "TimeSelectionRight";
            TimeSelectionRight.Key = Key.Right;
            TimeSelectionRight.ModifierKeys = ModifierKeys.Shift;
            TimeSelectionRight.Execute += delegate() { SLTimeSelectionRightCmdExecuted(this); };
            CommandBinding.AddCommand(TimeSelectionRight);

#endif
            #endregion

            #region TimeSelectionLeft
#if WPF
            KeyGesture TimeSelectionLeftKey = new KeyGesture(Key.Left, ModifierKeys.Shift);
            KeyBinding TimeSelectionLeftKB = new KeyBinding(ScheduleCommands.TimeSelectionLeft, TimeSelectionLeftKey);
            this.InputBindings.Add(TimeSelectionLeftKB);
            CommandBinding TimeSelectionLeftCmd = new CommandBinding(ScheduleCommands.TimeSelectionLeft, TimeSelectionLeftCmdExecuted, TimeSelectionLeftCmdCanExecute);
            this.CommandBindings.Add(TimeSelectionLeftCmd);
#endif
#if SILVERLIGHT
            Command TimeSelectionLeft = new Command();
            TimeSelectionLeft.Text = "TimeSelectionLeft";
            TimeSelectionLeft.Key = Key.Right;
            TimeSelectionLeft.ModifierKeys = ModifierKeys.Shift;
            TimeSelectionLeft.Execute += delegate() { SLTimeSelectionLeftCmdExecuted(this); };
            CommandBinding.AddCommand(TimeSelectionLeft);

#endif
            #endregion
            #endregion

            InitDoubleClickTimer();
            AllowDrop = true;
        }        

        #endregion

        #region Internal Fields

        internal bool ScheduleTypeChangedInternally;
        internal bool ScheduleTypeToDay;
        internal bool stopUpdate;
        internal AllDayAppointmentItemscontrol currentAllDaySelectedItem;
        internal bool ScheduleTypeToTimeline;
        internal bool IsVisibleDateSetInternally;
        internal bool IsRRuleSetInternally;
        internal bool IsDragEnabled { get; set; }
        internal bool AppointmentInResizeMode = false;
        internal DispatcherTimer reminderTimer;
        internal ScheduleAppointment CopiedAppointment;
        internal DateTime Currentselecteddate;
        internal ResizeBehavior Monthviewrb, Dayviewrb, Timelineviewrb;
        internal Canvas DragDropCanvas;
        internal Point SelectedPoint = new Point();
        internal Point Scrollpoint = new Point();
        internal bool isIntervalHeightset;
        internal bool IsResizeEnabled = false;
        internal bool isAppointment = true;
        internal Popup editpopup, addnewpopup;
        internal bool isscrollmoveondragging;
        internal List<Resource> selectedResourcename;
        internal bool allDayFlag;
        internal bool allDayTouch;
        internal bool contextMenuVisible = false;
        internal bool isContextMenuAltered;
        internal bool isTouchMenuHovered = false;
        internal ScheduleMonthDateContentControl cc;
        internal ScheduleMonthDateContentControl currentitem
        {
            get { return cc; }
            set{cc= value;}
        }
        internal bool isScrollResizeEnabled = false;
        internal ScrollViewer dayScrollViewer;
        internal ScheduleMonthAppointmentViewControl mvc;
        internal ScheduleDaysAppointmentViewControl dvc;
        internal ScheduleHorizontalAppointmentViewControl hvc;
        internal ScrollViewer timelineScrollViewer;
        internal ContentControl mainViewItem;
        internal Point currentpoint;
        internal Point Appointmentpoint;
        internal Size FloatingAppointmentSize;
        internal bool DayHeaderOderChange = false;
        internal ContentControl currentSelectedItem;
        internal bool needAutoFormat;
        internal bool exceedsMaxDate, exceedsMinDate;
        internal int MinResourceWidth = 250;
#if WPF
        internal bool ScrollManipulationCompleted=true;
        internal ContextMenu contextmenupopup, AddnewContextmenuPopup;
        internal bool allowEditorsToOpen = true;
        internal List<DependencyObject> hitTestList = null;
        internal Button Prev_Button, Next_Button;
        internal ItemsControl flipview;
        internal FrameworkElement flipviewselecteditem;
        internal ContentControl mainViewItem1;
        internal ContentControl mainViewItem2;
#else
        internal Popup contextmenupopup, AddnewContextmenuPopup;
#endif

        #endregion

        #region Private Fields
        
        HeaderTitleBarView headerTitleBarView;
        readonly ScheduleReminderControl Reminder;
        ScheduleMonthView month;
        ScheduleMonthView monthview
        {
            get { return month; }
            set { month = value; }
        }

        Visibility editorvisibility;
        ScheduleDaysView daysview;
        ScheduleTimeLineView timelineview;
        ObservableCollection<DateTime> CurrentVisibleSelectedDates;
        ObservableCollection<DateTime> dateColl;
        ObservableCollection<DateTime> PrevdateColl;
        ObservableCollection<DateTime> NextdateColl;
        bool isTemplateApplied;
        bool isMainViewItemClicked;
        bool isLoaded;
        ScheduleAppointment RecAppointment;
        ContentControl viewcontrol;
        AppointmentEditor appointmentEditor;
        OpenRecurringAppointment RecurringPopup;
#if WPF
        ContentControl  removedControl;
        bool isFlipitemvisibilityupdated;
        Grid Removeditem;
        readonly List<object> hitResultsList = new List<object>();   
        HeaderTitleBarView headerTitleBarView1, headerTitleBarView2;
        ScheduleMonthView monthview1;
        ScheduleMonthView monthview2;
        ScheduleMonthView monthview3;
        ScheduleDaysView daysview1;
        ScheduleDaysView daysview2;
        ScheduleDaysView daysview3;
        ScheduleTimeLineView timelineview1;
        ScheduleTimeLineView timelineview2;
        ScheduleTimeLineView timelineview3;
        DispatcherTimer doubleClickTimer;
        bool isDoubleTapped;
#else
        DispatcherTimer doubleClickTimer;
        bool isScheduleLoaded;
#endif
        readonly List<DateTime> totalselectedDates = new List<DateTime>();
        public Grid mainItem;
        #endregion

        #region CLR Properties

        #region SelectedAppointment

        public ScheduleAppointment SelectedAppointment { get; internal set; }

        #endregion

        #endregion

        #region DependencyProperties

        #region Public Properties

        #region CommandBinding
#if SILVERLIGHT
        public CommandHelper CommandBinding
        {
            get { return (CommandHelper)GetValue(CommandBindingProperty); }
            set { SetValue(CommandBindingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CommandBinding.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandBindingProperty =
            DependencyProperty.Register("CommandBinding", typeof(CommandHelper), typeof(SfSchedule), new PropertyMetadata(null));
#endif
        #endregion

        #region ScheduleType
        /// <summary>
        /// Gets or sets the type of schedule view.
        /// </summary>
        /// <example>
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.ScheduleType = ScheduleType.TimeLine;
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public ScheduleType ScheduleType
        {
            get { return (ScheduleType)GetValue(ScheduleTypeProperty); }
            set { SetValue(ScheduleTypeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ScheduleType.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ScheduleTypeProperty =
            DependencyProperty.Register("ScheduleType", typeof(ScheduleType), typeof(SfSchedule), new PropertyMetadata(ScheduleType.Day, ScheduleTypeChanged));

        private static void ScheduleTypeChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var schedule = dpo as SfSchedule;
            if (schedule != null)
            {
                schedule.needAutoFormat = false;
                // This condition check filters the visible date when ScheduleDateRange is set and ScheduleType changes
                if (schedule.ScheduleTypeChangedInternally || ( schedule.ScheduleDateRange != null &&(schedule.ScheduleType == ScheduleType.Day || schedule.ScheduleType == ScheduleType.TimeLine) 
                    && (args.OldValue.ToString() != "Week" && args.OldValue.ToString() != "WorkWeek" && args.OldValue.ToString() != "Month" && schedule.VisibleDates.Count == schedule.ScheduleDateRange.Count) ))
                {
                    schedule.ScheduleTypeChangedInternally = false;
                    schedule.ScheduleTypeToDay = true;
                    schedule.ScheduleTypeToTimeline = true;
                    schedule.SetSelectedDatesFromVisibleDates();
                }
                else
                {
                    schedule.ScheduleTypeToDay = false;
                    schedule.ScheduleTypeToTimeline = false;
                }
                schedule.ChangeScheduleType(schedule, args.OldValue.ToString(), args.NewValue.ToString());
            }
        }
        #endregion

        #region AllowEditing
        /// <summary>
        /// Gets or sets a value indicating whether the appointment editor should be allowed
        /// to create or edit appointment.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if ; otherwise, <see langword="false" />.
        /// </value>
        /// <example>
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleWinRT
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.AllowEditing = false;
        ///             Grid.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public bool AllowEditing
        {
            get { return (bool)GetValue(AllowEditingProperty); }
            set { SetValue(AllowEditingProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AllowEditing.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AllowEditingProperty =
            DependencyProperty.Register("AllowEditing", typeof(bool), typeof(SfSchedule), new PropertyMetadata(true));
        #endregion

        #region AppointmentTooltipVisibility
        /// <summary>
        /// Gets or sets the visibility of Schedule appointment's tooltip.
        /// </summary>
        /// <remarks>
        /// AppointmentTooltipTemplate of Schedule must be set to view the tooltip.
        /// </remarks>
        /// <example>
        /// using System;
        /// using System.Windows;
        /// using System.Windows.Controls;
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.Appointments.Add(new ScheduleAppointment
        ///             {
        ///                 Subject = "Meeting",
        ///                 StartTime = DateTime.Now.Date.AddHours(9),
        ///                 EndTime = DateTime.Now.Date.AddHours(12)
        ///             });
        ///             schedule.AppointmentTooltipVisibility = Visibility.Visible;
        ///             schedule.AppointmentToolTipTemplate = (ControlTemplate)this.Resources["AppointmentToolTipTemplate"];
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public Visibility AppointmentTooltipVisibility
        {
            get { return (Visibility)GetValue(AppointmentTooltipVisibilityProperty); }
            set { SetValue(AppointmentTooltipVisibilityProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AppointmentTooltipVisibility.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AppointmentTooltipVisibilityProperty =
            DependencyProperty.Register("AppointmentTooltipVisibility", typeof(Visibility), typeof(SfSchedule), new PropertyMetadata(Visibility.Collapsed));
        #endregion

        #region AppointmentToolTipTemplate
        /// <summary>
        /// Gets or sets the template for Schedule appointment's tooltip.
        /// </summary>
        /// <remarks>
        /// AppointmentTooltipVisibility of Schedule must be set as "Visible" to view the tooltip.
        /// </remarks>
        /// <example>
        /// using System;
        /// using System.Windows;
        /// using System.Windows.Controls;
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.Appointments.Add(new ScheduleAppointment
        ///             {
        ///                 Subject = "Meeting",
        ///                 StartTime = DateTime.Now.Date.AddHours(9),
        ///                 EndTime = DateTime.Now.Date.AddHours(12)
        ///             });
        ///             schedule.AppointmentTooltipVisibility = Visibility.Visible;
        ///             schedule.AppointmentToolTipTemplate = (ControlTemplate)this.Resources["AppointmentToolTipTemplate"];
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public ControlTemplate AppointmentToolTipTemplate
        {
            get { return (ControlTemplate)GetValue(AppointmentToolTipTemplateProperty); }
            set { SetValue(AppointmentToolTipTemplateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AppointmentToolTipTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AppointmentToolTipTemplateProperty =
            DependencyProperty.Register("AppointmentToolTipTemplate", typeof(ControlTemplate), typeof(SfSchedule), new PropertyMetadata(null));
        #endregion

        #region AppointmentSelectionBrush
        /// <summary>
        /// Gets or sets the color of appointment border while selecting an appointment.
        /// </summary>
        /// <example>
        /// using System;
        /// using System.Windows.Media;
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.Appointments.Add(new ScheduleAppointment
        ///             {
        ///                 Subject = "Meeting",
        ///                 StartTime = DateTime.Now.Date.AddHours(9),
        ///                 EndTime = DateTime.Now.Date.AddHours(12)
        ///             });
        ///             schedule.AppointmentSelectionBrush = new SolidColorBrush(Colors.Yellow);
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public Brush AppointmentSelectionBrush
        {
            get { return (Brush)GetValue(AppointmentSelectionBrushProperty); }
            set { SetValue(AppointmentSelectionBrushProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AppointmentSelectionBrush.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AppointmentSelectionBrushProperty =
            DependencyProperty.Register("AppointmentSelectionBrush", typeof(Brush), typeof(SfSchedule), new PropertyMetadata(new SolidColorBrush(Colors.Black)));
        #endregion

        #region AppointmentTemplate
        /// <summary>
        /// Gets or sets the template for customizing schedule appointment.
        /// </summary>
        /// <example>
        /// using System;
        /// using System.Windows;
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.Appointments.Add(new ScheduleAppointment
        ///             {
        ///                 Subject = "Meeting",
        ///                 StartTime = DateTime.Now.Date.AddHours(9),
        ///                 EndTime = DateTime.Now.Date.AddHours(12)
        ///             });
        ///             schedule.AppointmentTemplate =
        /// (DataTemplate)this.Resources["AppointmentTemplate"];
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public DataTemplate AppointmentTemplate
        {
            get { return (DataTemplate)GetValue(AppointmentTemplateProperty); }
            set { SetValue(AppointmentTemplateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AppointmentTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AppointmentTemplateProperty =
            DependencyProperty.Register("AppointmentTemplate", typeof(DataTemplate), typeof(SfSchedule), new PropertyMetadata(null));
        #endregion

        #region Appointments
        /// <summary>
        /// Gets or sets the schedule appointments.
        /// </summary>
        /// <example>
        /// using System;
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.Appointments.Add(new ScheduleAppointment
        ///             {
        ///                 Subject = "Meeting",
        ///                 StartTime = DateTime.Now.Date.AddHours(9),
        ///                 EndTime = DateTime.Now.Date.AddHours(12)
        ///             });
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Schedule.ScheduleAppointmentCollection"/>
        public ScheduleAppointmentCollection Appointments
        {
            get { return (ScheduleAppointmentCollection)GetValue(AppointmentsProperty); }
            set { SetValue(AppointmentsProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Appointments.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AppointmentsProperty =
            DependencyProperty.Register("Appointments", typeof(ScheduleAppointmentCollection), typeof(SfSchedule), new PropertyMetadata(null, AppointmentsChanged));

        private static void AppointmentsChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs arg)
        {
            var obj = dpo as SfSchedule;
            if (obj != null)
            {
                if (obj.Appointments != null)
                {
                    foreach (ScheduleAppointment item in obj.Appointments)
                    {
                        item.PropertyChanged += obj.item_PropertyChanged;
                        if (item.ReminderTime != ReminderTimeType.None)
                        {
                            item.ReminderDeliveryTime = item.InternalStartTime - item.MeasureTime(item.ReminderTime);
                        }
                    }
                }
                if (arg.OldValue != null)
                {
                    var scheduleAppointmentCollection = arg.OldValue as ScheduleAppointmentCollection;
                    if (scheduleAppointmentCollection != null)
                        scheduleAppointmentCollection.CollectionChanged -= obj.Appointments_CollectionChanged;
                }
                if (arg.NewValue != null)
                {
                    var scheduleAppointmentCollection = arg.NewValue as ScheduleAppointmentCollection;
                    if (scheduleAppointmentCollection != null)
                    {
                        scheduleAppointmentCollection.CollectionChanged += obj.Appointments_CollectionChanged;
                        obj.ProxyAppointments = new Dictionary<DateTime, ObservableCollection<ScheduleAppointment>>();
                        obj.SetProxyAppointments(obj.Appointments);
                        obj.SetCurrentVisibleAppointments(obj.CurrentSelectedDates);
                    }
                }
                obj.TriggerReminder();
            }
        }
        #endregion

        #region AppointmentStatusCollection
        /// <summary>
        /// Gets or sets the collection of status required for adding appointments.
        /// </summary>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Schedule.ScheduleAppointmentStatusCollection"/>
        /// <example>
        /// using System.Windows.Media;
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.AppointmentStatusCollection = new ScheduleAppointmentStatusCollection
        ///             {
        ///                 new ScheduleAppointmentStatus{Status="Idle", Brush = new SolidColorBrush(Colors.Pink)},
        ///                 new ScheduleAppointmentStatus{Status="Busy", Brush = new SolidColorBrush(Colors.Red)}
        ///             };
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// 
        /// </example>
        public ScheduleAppointmentStatusCollection AppointmentStatusCollection
        {
            get { return (ScheduleAppointmentStatusCollection)GetValue(AppointmentStatusCollectionProperty); }
            set { SetValue(AppointmentStatusCollectionProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AppointmentStatusCollection.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AppointmentStatusCollectionProperty =
            DependencyProperty.Register("AppointmentStatusCollection", typeof(ScheduleAppointmentStatusCollection), typeof(SfSchedule), new PropertyMetadata(null));
        #endregion

        #region ItemsSource
        /// <summary>
        /// Gets or sets the items collection for adding mapped appointments.
        /// </summary>
        /// <remarks>
        /// Attributes of ApppointmentMapping should be specified to add mapped
        /// appointments.
        /// </remarks>
        /// <example>
        /// using System;
        /// using System.Collections.ObjectModel;
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             ObservableCollection<MappedAppointment> mappedAppointments = new ObservableCollection<MappedAppointment>
        ///             {
        ///                 new MappedAppointment{MappedSubject = "Meeting", MappedStartTime = DateTime.Now.Date.AddHours(10), 
        ///                     MappedEndTime = DateTime.Now.Date.AddHours(13)},
        ///                 new MappedAppointment{MappedSubject = "Conference", MappedStartTime = DateTime.Now.Date.AddHours(15), 
        ///                     MappedEndTime = DateTime.Now.Date.AddHours(18)},
        ///             };
        ///             schedule.AppointmentMapping = new ScheduleAppointmentMapping
        ///             {
        ///                 SubjectMapping = "MappedSubject",
        ///                 StartTimeMapping = "MappedStartTime",
        ///                 EndTimeMapping = "MappedEndTime"
        ///             };
        ///             schedule.ItemsSource = mappedAppointments;
        ///             Layout.Children.Add(schedule);
        ///         }
        ///         public class MappedAppointment
        ///         {
        ///             public string MappedSubject { get; set; }
        ///             public DateTime MappedStartTime { get; set; }
        ///             public DateTime MappedEndTime { get; set; }
        ///         }
        ///     }
        /// }
        /// 
        /// </example>
        public object ItemsSource
        {
            get { return GetValue(ItemsSourceProperty); }
            set
            {
#if WPF
                if (value is DataTable)
                    SetValue(ItemsSourceProperty, (value as DataTable).Rows);
                else
#endif
                    SetValue(ItemsSourceProperty, value);
            }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(object), typeof(SfSchedule), new PropertyMetadata(null, OnItemsSourceChanged));

        private static void OnItemsSourceChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var schedule = dpo as SfSchedule;
            if (args.OldValue != null)
            {
                if (schedule != null)
                    schedule.UnwireItemSource(args.OldValue as IEnumerable);
            }
            if (schedule != null && schedule.Appointments != null)
            {
                schedule.Appointments.CollectionChanged -= schedule.Appointments_CollectionChanged;
            }
            if (schedule != null)
            {
                if (schedule.ItemsSource is IEnumerable)
                    schedule.WireItemSource(schedule.ItemsSource as IEnumerable);
                schedule.SetItemsSource();
                if (schedule.Appointments != null)
                {
                    schedule.Appointments.CollectionChanged += schedule.Appointments_CollectionChanged;
                }
            }
        }
        #endregion

        #region AppointmentMapping
        /// <summary>
        /// Gets or sets the AppointmentMapping attributes to map the properties in the
        /// underlying ItemsSource of Schedule.
        /// </summary>
        /// <example>
        /// using System;
        /// using System.Collections.ObjectModel;
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             ObservableCollection<MappedAppointment> mappedAppointments = new ObservableCollection<MappedAppointment>
        ///             {
        ///                 new MappedAppointment{MappedSubject = "Meeting", MappedStartTime = DateTime.Now.Date.AddHours(10), 
        ///                     MappedEndTime = DateTime.Now.Date.AddHours(13)},
        ///                 new MappedAppointment{MappedSubject = "Conference", MappedStartTime = DateTime.Now.Date.AddHours(15), 
        ///                     MappedEndTime = DateTime.Now.Date.AddHours(18)},
        ///             };
        ///             schedule.AppointmentMapping = new ScheduleAppointmentMapping
        ///             {
        ///                 SubjectMapping = "MappedSubject",
        ///                 StartTimeMapping = "MappedStartTime",
        ///                 EndTimeMapping = "MappedEndTime"
        ///             };
        ///             schedule.ItemsSource = mappedAppointments;
        ///             Layout.Children.Add(schedule);
        ///         }
        ///         public class MappedAppointment
        ///         {
        ///             public string MappedSubject { get; set; }
        ///             public DateTime MappedStartTime { get; set; }
        ///             public DateTime MappedEndTime { get; set; }
        ///         }
        ///     }
        /// }
        /// 
        /// </example>
        public ScheduleAppointmentMapping AppointmentMapping
        {
            get { return (ScheduleAppointmentMapping)GetValue(AppointmentMappingProperty); }
            set { SetValue(AppointmentMappingProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AppointmentMapping.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AppointmentMappingProperty =
            DependencyProperty.Register("AppointmentMapping", typeof(ScheduleAppointmentMapping), typeof(SfSchedule), new PropertyMetadata(null));
        #endregion

        #region MonthHeaderDateFormat
        /// <summary>
        /// Gets or sets the DateTime format for date displayed in every days of month view.
        /// </summary>
        /// <example>
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.ScheduleType = ScheduleType.Month;
        ///             schedule.MonthHeaderDateFormat = "MMM dd yyyy";
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public string MonthHeaderDateFormat
        {
            get { return (string)GetValue(MonthHeaderDateFormatProperty); }
            set { SetValue(MonthHeaderDateFormatProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MonthHeaderDateFormat.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MonthHeaderDateFormatProperty =
            DependencyProperty.Register("MonthHeaderDateFormat", typeof(string), typeof(SfSchedule), new PropertyMetadata("dd"));
        #endregion

        #region HeaderDateFormat
        /// <summary>
        /// Gets or sets the DateTime format for date displayed in header.
        /// </summary>
        /// <example>
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.HeaderDateFormat = "MMM dd yyyy";
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public string HeaderDateFormat
        {
            get { return (string)GetValue(HeaderDateFormatProperty); }
            set { SetValue(HeaderDateFormatProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for HeaderDateFormat.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HeaderDateFormatProperty =
            DependencyProperty.Register("HeaderDateFormat", typeof(string), typeof(SfSchedule), new PropertyMetadata("dddd dd", OnHeaderDateFormatChanged));

        private static void OnHeaderDateFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SfSchedule)
            {
                SfSchedule schedule = (d as SfSchedule);
                if (!schedule.needAutoFormat)
                    schedule.HeaderFormat = e.NewValue.ToString();
            }
        }
        #endregion

        #region MinorTickTimeFormat
        /// <summary>
        /// Gets or sets the DateTime format for minor ticks which represents minute in timeslot.
        /// </summary>
        /// <remarks>
        /// TimeInterval must be specified other than OneHour to view the minor ticks.
        /// </remarks>
        /// <example>
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.TimeInterval = TimeInterval.ThirtyMin;
        ///             schedule.MinorTickTimeFormat = "mm:ss";
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public string MinorTickTimeFormat
        {
            get { return (string)GetValue(MinorTickTimeFormatProperty); }
            set { SetValue(MinorTickTimeFormatProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MinorTickTimeFormat.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MinorTickTimeFormatProperty =
            DependencyProperty.Register("MinorTickTimeFormat", typeof(string), typeof(SfSchedule), new PropertyMetadata("mm"));
        #endregion

        #region MajorTickTimeFormat
        /// <summary>
        /// Gets or sets the DateTime format for major ticks which represents hour in timeslot.
        /// </summary>
        /// <example>
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.MajorTickTimeFormat = "hh:mm tt";
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public string MajorTickTimeFormat
        {
            get { return (string)GetValue(MajorTickTimeFormatProperty); }
            set { SetValue(MajorTickTimeFormatProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MajorTickTimeFormat.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MajorTickTimeFormatProperty =
            DependencyProperty.Register("MajorTickTimeFormat", typeof(string), typeof(SfSchedule), new PropertyMetadata("hh:mm tt"));
        #endregion

        #region MinorTickVisibility
        /// <summary>
        /// Gets or sets the visibility of minor ticks which represents minute in time slot.
        /// </summary>
        /// <remarks>
        /// TimeInterval must be specified other than OneHour to view the minor ticks.
        /// </remarks>
        /// <example>
        /// using System.Windows;
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.TimeInterval = TimeInterval.ThirtyMin;
        ///             schedule.MinorTickVisibility = Visibility.Collapsed;
        ///         }
        ///     }
        /// }
        /// </example>
        public Visibility MinorTickVisibility
        {
            get { return (Visibility)GetValue(MinorTickVisibilityProperty); }
            set { SetValue(MinorTickVisibilityProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MinorTickVisibility.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MinorTickVisibilityProperty =
            DependencyProperty.Register("MinorTickVisibility", typeof(Visibility), typeof(SfSchedule), new PropertyMetadata(Visibility.Visible));
        #endregion

        #region MajorTickVisibility
        /// <summary>
        /// Gets or sets the visibility of major ticks which represents hour in time slot.
        /// </summary>
        /// <example>
        /// using System.Windows;
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.MajorTickVisibility = Visibility.Collapsed;
        ///         }
        ///     }
        /// }
        /// </example>
        public Visibility MajorTickVisibility
        {
            get { return (Visibility)GetValue(MajorTickVisibilityProperty); }
            set { SetValue(MajorTickVisibilityProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MajorTickVisibility.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MajorTickVisibilityProperty =
            DependencyProperty.Register("MajorTickVisibility", typeof(Visibility), typeof(SfSchedule), new PropertyMetadata(Visibility.Visible));
        #endregion

        #region MinorTickStroke
        public Brush MinorTickStroke
        {
            get { return (Brush)GetValue(MinorTickStrokeProperty); }
            set { SetValue(MinorTickStrokeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinorTickStroke.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinorTickStrokeProperty =
            DependencyProperty.Register("MinorTickStroke", typeof(Brush), typeof(SfSchedule), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 225, 225, 225))));
        #endregion

        #region MajorTickStroke
        public Brush MajorTickStroke
        {
            get { return (Brush)GetValue(MajorTickStrokeProperty); }
            set { SetValue(MajorTickStrokeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MajorTickStroke.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MajorTickStrokeProperty =
            DependencyProperty.Register("MajorTickStroke", typeof(Brush), typeof(SfSchedule), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 225, 225, 225))));
        #endregion

        #region MinorTickLabelStroke
        public Brush MinorTickLabelStroke
        {
            get { return (Brush)GetValue(MinorTickLabelStrokeProperty); }
            set { SetValue(MinorTickLabelStrokeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinorTickStroke.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinorTickLabelStrokeProperty =
            DependencyProperty.Register("MinorTickLabelStroke", typeof(Brush), typeof(SfSchedule), new PropertyMetadata(new SolidColorBrush(Colors.Gray)));
        #endregion

        #region MajorTickLabelStroke
        public Brush MajorTickLabelStroke
        {
            get { return (Brush)GetValue(MajorTickLabelStrokeProperty); }
            set { SetValue(MajorTickLabelStrokeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MajorTickStroke.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MajorTickLabelStrokeProperty =
            DependencyProperty.Register("MajorTickLabelStroke", typeof(Brush), typeof(SfSchedule), new PropertyMetadata(new SolidColorBrush(Colors.Gray)));
        #endregion

        #region MinorTickStrokeDashArray
        public DoubleCollection MinorTickStrokeDashArray
        {
            get { return (DoubleCollection)GetValue(MinorTickStrokeDashArrayProperty); }
            set { SetValue(MinorTickStrokeDashArrayProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinorTickStrokeDashArray.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinorTickStrokeDashArrayProperty =
            DependencyProperty.Register("MinorTickStrokeDashArray", typeof(DoubleCollection), typeof(SfSchedule), new PropertyMetadata(new DoubleCollection()));
        #endregion

        #region MajorTickStrokeDashArray
        public DoubleCollection MajorTickStrokeDashArray
        {
            get { return (DoubleCollection)GetValue(MajorTickStrokeDashArrayProperty); }
            set { SetValue(MajorTickStrokeDashArrayProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MajorTickStrokeDashArray.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MajorTickStrokeDashArrayProperty =
            DependencyProperty.Register("MajorTickStrokeDashArray", typeof(DoubleCollection), typeof(SfSchedule), new PropertyMetadata(new DoubleCollection()));
        #endregion

        #region ShowAllDay
        /// <summary>
        /// Gets or sets a value indicating whether the AllDay panel should be shown.
        /// </summary>
        /// <remarks>
        /// AllDay panel is viewed only in day view and week view.
        /// </remarks>
        /// <value>
        /// <see langword="true" /> if ; otherwise, <see langword="false" />.
        /// </value>
        /// <example>
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.ShowAllDay = false;
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public bool ShowAllDay
        {
            get { return (bool)GetValue(ShowAllDayProperty); }
            set { SetValue(ShowAllDayProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ShowAllDay.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ShowAllDayProperty =
            DependencyProperty.Register("ShowAllDay", typeof(bool), typeof(SfSchedule), new PropertyMetadata(true));
        #endregion

        #region ShowAppointmentNavigationButtons
        /// <summary>
        /// Gets or sets a value indicating whether the appointment navigation buttons
        /// should be shown to view previous and next appointments from current view.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if ; otherwise, <see langword="false" />.
        /// </value>
        /// <example>
        /// using System;
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.Appointments.Add(new ScheduleAppointment
        ///             {
        ///                 Subject = "Meeting",
        ///                 StartTime = DateTime.Now.Date.SubractDays(2),
        ///                 EndTime = DateTime.Now.Date.SubractDays(2).AddHours(2)
        ///             });
        ///             schedule.ShowAppointmentNavigationButtons = true;
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public bool ShowAppointmentNavigationButtons
        {
            get { return (bool)GetValue(ShowAppointmentNavigationButtonsProperty); }
            set { SetValue(ShowAppointmentNavigationButtonsProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ShowAppointmentNavigationButtons.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ShowAppointmentNavigationButtonsProperty =
            DependencyProperty.Register("ShowAppointmentNavigationButtons", typeof(bool), typeof(SfSchedule), new PropertyMetadata(false));
        #endregion

        #region PreviousNavigationButtonTemplate
        /// <summary>
        /// Gets or sets the template for customizing button which navigates to previous
        /// appointments from current view.
        /// </summary>
        /// <remarks>
        /// To view the customized previous navigation button,
        /// ShowAppointmentNavigationButtons of Schedule must be enabled.
        /// </remarks>
        /// <example>
        /// using System;
        /// using System.Windows;
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.Appointments.Add(new ScheduleAppointment
        ///             {
        ///                 Subject = "Meeting",
        ///                 StartTime = DateTime.Now.Date.SubractDays(2),
        ///                 EndTime = DateTime.Now.Date.SubractDays(2).AddHours(2)
        ///             });
        ///             schedule.ShowAppointmentNavigationButtons = true;
        ///             schedule.PreviousNavigationButtonTemplate = (DataTemplate)this.Resources["PreviousNavigationButtonTemplate"];
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public DataTemplate PreviousNavigationButtonTemplate
        {
            get { return (DataTemplate)GetValue(PreviousNavigationButtonTemplateProperty); }
            set { SetValue(PreviousNavigationButtonTemplateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for PreviousNavigationButtonTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty PreviousNavigationButtonTemplateProperty =
            DependencyProperty.Register("PreviousNavigationButtonTemplate", typeof(DataTemplate), typeof(SfSchedule), new PropertyMetadata(null));
        #endregion

        #region NextNavigationButtonTemplate
        /// <summary>
        /// Gets or sets the template for customizing button which navigates to next
        /// appointments from current view.
        /// </summary>
        /// <remarks>
        /// To view the customized next navigation button, ShowAppointmentNavigationButtons
        /// of Schedule must be enabled.
        /// </remarks>
        /// <example>
        /// using System;
        /// using System.Windows;
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.Appointments.Add(new ScheduleAppointment
        ///             {
        ///                 Subject = "Meeting",
        ///                 StartTime = DateTime.Now.Date.AddDays(2),
        ///                 EndTime = DateTime.Now.Date.AddDays(2).AddHours(2)
        ///             });
        ///             schedule.ShowAppointmentNavigationButtons = true;
        ///             schedule.NextNavigationButtonTemplate = (DataTemplate)this.Resources["NextNavigationButtonTemplate"];
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public DataTemplate NextNavigationButtonTemplate
        {
            get { return (DataTemplate)GetValue(NextNavigationButtonTemplateProperty); }
            set { SetValue(NextNavigationButtonTemplateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for NextNavigationButtonTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty NextNavigationButtonTemplateProperty =
            DependencyProperty.Register("NextNavigationButtonTemplate", typeof(DataTemplate), typeof(SfSchedule), new PropertyMetadata(null));
        #endregion

        #region ScheduleResourceTypeCollection
        /// <summary>
        /// Gets or sets the ResourceType collection for defining various resource collection to Schedule.
        /// </summary>
        /// <remarks>
        /// TypeName of ResourceType differentiates the resource collection.
        /// </remarks>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Schedule.ResourceType"/>
        /// <example>
        /// using System.Collections.ObjectModel;
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             ResourceType resourceType = new ResourceType { TypeName = "Doctors" };
        ///             resourceType.ResourceCollection.Add(new Resource { ResourceName = "Resource1", TypeName = "Dr.John", DisplayName = "Dr.John, M.B.B.S" });
        ///             resourceType.ResourceCollection.Add(new Resource { ResourceName = "Resource2", TypeName = "Dr.Jessie", DisplayName = "Dr.Jessie, M.B.B.S" });
        ///             schedule.ScheduleResourceTypeCollection = new ObservableCollection<ResourceType> { resourceType };
        ///             schedule.Resource = "Doctors";
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public ObservableCollection<ResourceType> ScheduleResourceTypeCollection
        {
            get { return (ObservableCollection<ResourceType>)GetValue(ScheduleResourceTypeCollectionProperty); }
            set { SetValue(ScheduleResourceTypeCollectionProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ScheduleResourceTypeCollection.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ScheduleResourceTypeCollectionProperty =
            DependencyProperty.Register("ScheduleResourceTypeCollection", typeof(ObservableCollection<ResourceType>), typeof(SfSchedule), new PropertyMetadata(null, ScheduleResourceTypeCollectionPropertyChanged));

        private static void ScheduleResourceTypeCollectionPropertyChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs arg)
        {
            var sch = dpo as SfSchedule;
            if (sch != null)
            {
                if (sch.ScheduleResourceTypeCollection != null)
                {
                    sch.ScheduleResourceTypeCollection.CollectionChanged -= sch.ScheduleResourceTypeCollection_CollectionChanged;
                    sch.ScheduleResourceTypeCollection.CollectionChanged += sch.ScheduleResourceTypeCollection_CollectionChanged;
                }
                if (sch.ScheduleResourceTypeCollection != null && sch.ScheduleResourceTypeCollection.Count > 0)
                {
                    sch.ScheduleResourceType =
                        sch.ScheduleResourceTypeCollection.FirstOrDefault(res => (res.TypeName == sch.Resource));
                    if (sch.daysview != null)
                    {
                        #if WPF
                        if (sch.EnableTouch == false)
#endif
                            sch.daysview.GenerateHeaderItems();
#if WPF
                        else if (sch.daysview1 != null && sch.daysview2 != null && sch.daysview3 != null)
                        {
                            sch.daysview1.GenerateHeaderItems();
                            sch.daysview2.GenerateHeaderItems();
                            sch.daysview3.GenerateHeaderItems();
                        }
#endif
                    }
                }
#if WPF
                if (sch.appointmentEditor != null)
                    sch.appointmentEditor.ScheduleResourceTypeCollection = sch.ScheduleResourceTypeCollection;
#endif
            }
        }
        #endregion

        #region Resource
        /// <summary>
        /// Gets or sets the resource for Schedule from ScheduleResourceTypeCollection.
        /// </summary>
        /// <remarks>
        /// Resource must be assigned with TypeName of ScheduleResourceTypeCollection.
        /// </remarks>
        /// <example>
        /// using System.Collections.ObjectModel;
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             ResourceType resourceType = new ResourceType { TypeName = "Doctors" };
        ///             resourceType.ResourceCollection.Add(new Resource { ResourceName = "Resource1", TypeName = "Dr.John", DisplayName = "Dr.John, M.B.B.S" });
        ///             resourceType.ResourceCollection.Add(new Resource { ResourceName = "Resource2", TypeName = "Dr.Jessie", DisplayName = "Dr.Jessie, M.B.B.S" });
        ///             schedule.ScheduleResourceTypeCollection = new ObservableCollection<ResourceType> { resourceType };
        ///             schedule.Resource = "Doctors";
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public string Resource
        {
            get { return (string)GetValue(ResourceProperty); }
            set { SetValue(ResourceProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Resource.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ResourceProperty =
            DependencyProperty.Register("Resource", typeof(string), typeof(SfSchedule), new PropertyMetadata(string.Empty, OnResourceChanged));

        private static void OnResourceChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs arg)
        {
            var sch = dpo as SfSchedule;
            if (sch != null)
            {
                if (sch.ScheduleResourceTypeCollection != null && sch.ScheduleResourceTypeCollection.Count > 0)
                {
                    sch.ScheduleResourceType = sch.ScheduleResourceTypeCollection.FirstOrDefault(res => (res.TypeName == sch.Resource));
                }
                sch.SetNavigationTap();
            }
        }
        #endregion

        #region DayHeaderOrder
        /// <summary>
        /// Gets or sets the order by which resources have to be displayed.
        /// </summary>
        /// <example>
        /// using System.Collections.ObjectModel;
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             ResourceType resourceType = new ResourceType { TypeName = "Doctors" };
        ///             resourceType.ResourceCollection.Add(new Resource { ResourceName = "Resource1", TypeName = "Dr.John", DisplayName = "Dr.John, M.B.B.S" });
        ///             resourceType.ResourceCollection.Add(new Resource { ResourceName = "Resource2", TypeName = "Dr.Jessie", DisplayName = "Dr.Jessie, M.B.B.S" });
        ///             schedule.ScheduleResourceTypeCollection = new ObservableCollection<ResourceType> { resourceType };
        ///             schedule.Resource = "Doctors";
        ///             schedule.DayHeaderOrder = DayHeaderOrder.OrderByDate;
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public DayHeaderOrder DayHeaderOrder
        {
            get { return (DayHeaderOrder)GetValue(DayHeaderOrderProperty); }
            set { SetValue(DayHeaderOrderProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for DayHeaderOrder.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DayHeaderOrderProperty =
            DependencyProperty.Register("DayHeaderOrder", typeof(DayHeaderOrder), typeof(SfSchedule), new PropertyMetadata(DayHeaderOrder.OrderByResource, OnDayHeaderOrderChanged));

        private static void OnDayHeaderOrderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var scheduleInstance = d as SfSchedule;
            scheduleInstance.DayHeaderOderChange = true;
        }
        #endregion

        #region DayViewColumnCount
        /// <summary>
        /// Gets or sets the order by which resources have to be displayed.
        /// </summary>
        /// <example>
        /// using System.Collections.ObjectModel;
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             ResourceType resourceType = new ResourceType { TypeName = "Doctors" };
        ///             resourceType.ResourceCollection.Add(new Resource { ResourceName = "Resource1", TypeName = "Dr.John", DisplayName = "Dr.John, M.B.B.S" });
        ///             resourceType.ResourceCollection.Add(new Resource { ResourceName = "Resource2", TypeName = "Dr.Jessie", DisplayName = "Dr.Jessie, M.B.B.S" });
        ///             schedule.ScheduleResourceTypeCollection = new ObservableCollection<ResourceType> { resourceType };
        ///             schedule.Resource = "Doctors";
        ///             schedule.DayViewColumnCount = 2;
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public int DayViewColumnCount
        {
            get { return (int)GetValue(DayViewColumnCountProperty); }
            set { SetValue(DayViewColumnCountProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for DayViewColumnCount.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DayViewColumnCountProperty =
            DependencyProperty.Register("DayViewColumnCount", typeof(int), typeof(SfSchedule), new PropertyMetadata(0, OnDayViewColumnCountChanged));

        private static void OnDayViewColumnCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SfSchedule)
            {
                SfSchedule schedule = d as SfSchedule;
                schedule.DayViewColumnCount = (int)e.NewValue < 0 ? 0 : (int)e.NewValue;
            }
        }
        #endregion

        #region FocusedMonth
        /// <summary>
        /// Gets or sets the color for dates of selected month.
        /// </summary>
        /// <example>
        /// using System.Windows.Media;
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.ScheduleType = ScheduleType.Month;
        ///             schedule.FocusedMonth = new SolidColorBrush(Colors.Gray);
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public Brush FocusedMonth
        {
            get { return (Brush)GetValue(FocusedMonthProperty); }
            set { SetValue(FocusedMonthProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for FocusedMonth.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty FocusedMonthProperty =
            DependencyProperty.Register("FocusedMonth", typeof(Brush), typeof(SfSchedule), new PropertyMetadata(new SolidColorBrush(Colors.White)));
        #endregion

        #region NonFocusedMonth
        /// <summary>
        /// Gets or sets the color for dates of previous and next months.
        /// </summary>
        /// <example>
        /// using System.Windows.Media;
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.ScheduleType = ScheduleType.Month;
        ///             schedule.NonFocusedMonth = new SolidColorBrush(Colors.LightGray);
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public Brush NonFocusedMonth
        {
            get { return (Brush)GetValue(NonFocusedMonthProperty); }
            set { SetValue(NonFocusedMonthProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for NonFocusedMonth.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty NonFocusedMonthProperty =
            DependencyProperty.Register("NonFocusedMonth", typeof(Brush), typeof(SfSchedule), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xFF, 0xE8, 0xE8, 0xE8))));
        #endregion

        #region MonthViewLineStroke
        /// <summary>
        /// Gets or sets the color for lines in schedule.
        /// </summary>
        /// <example>
        /// using Windows.UI;
        /// using Windows.UI.Xaml.Media;
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.MonthViewLineStroke = new SolidColorBrush(Colors.Gray);
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public Brush MonthViewLineStroke
        {
            get { return (Brush)GetValue(MonthViewLineStrokeProperty); }
            set { SetValue(MonthViewLineStrokeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MonthViewLineStroke.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MonthViewLineStrokeProperty =
            DependencyProperty.Register("MonthViewLineStroke", typeof(Brush), typeof(SfSchedule), new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));
        #endregion

        #region DayViewVerticaLineStroke
        /// <summary>
        /// Gets or sets the color for lines in schedule.
        /// </summary>
        /// <example>
        /// using Windows.UI;
        /// using Windows.UI.Xaml.Media;
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.DayViewVerticaLineStroke = new SolidColorBrush(Colors.Gray);
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public Brush DayViewVerticaLineStroke
        {
            get { return (Brush)GetValue(DayViewVerticaLineStrokeProperty); }
            set { SetValue(DayViewVerticaLineStrokeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for DayViewVerticaLineStroke.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DayViewVerticaLineStrokeProperty =
            DependencyProperty.Register("DayViewVerticaLineStroke", typeof(Brush), typeof(SfSchedule), new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));
        #endregion

        #region HeaderBackground
        /// <summary>
        /// Gets or sets the background for schedule's header.
        /// </summary>
        /// <example>
        /// using System.Windows.Media;
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.HeaderBackground = new SolidColorBrush(Colors.Green);
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public Brush HeaderBackground
        {
            get { return (Brush)GetValue(HeaderBackgroundProperty); }
            set { SetValue(HeaderBackgroundProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for HeaderBackground.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HeaderBackgroundProperty =
            DependencyProperty.Register("HeaderBackground", typeof(Brush), typeof(SfSchedule), new PropertyMetadata(new SolidColorBrush(Colors.White)));
        #endregion

        #region CurrentDateBackground
        /// <summary>
        /// Gets or sets the background for current date in schedule.
        /// </summary>
        /// <example>
        /// using System.Windows.Media;
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.CurrentDateBackground = new SolidColorBrush(Colors.Blue);
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public Brush CurrentDateBackground
        {
            get { return (Brush)GetValue(CurrentDateBackgroundProperty); }
            set { SetValue(CurrentDateBackgroundProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for CurrentBackground.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CurrentDateBackgroundProperty =
            DependencyProperty.Register("CurrentDateBackground", typeof(Brush), typeof(SfSchedule), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xFF, 0x75, 0x75, 0x75))));
        #endregion

        #region NonWorkingDays
        /// <summary>
        /// Gets or sets the collection of non working days.
        /// </summary>
        /// <remarks>
        /// IsHighlightWorkingHours of schedule must be set as "True" to view non working days.
        /// </remarks>
        /// <example>
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.NonWorkingDays = "Thursday,Friday";
        ///             schedule.IsHighLightWorkingHours = true;
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public string NonWorkingDays
        {
            get { return (string)GetValue(NonWorkingDaysProperty); }
            set { SetValue(NonWorkingDaysProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for WorkingDays.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty NonWorkingDaysProperty =
            DependencyProperty.Register("NonWorkingDays", typeof(string), typeof(SfSchedule), new PropertyMetadata("Sunday,Saturday", OnNonWorkingDayChanged));

        private static void OnNonWorkingDayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var schedule = d as SfSchedule;
            if (schedule != null)
            {
                schedule.NonWorkingDateCollection = schedule.StringToDaysOfWeekConverter(schedule.NonWorkingDays, false);
                if (schedule.ScheduleType == ScheduleType.WorkWeek)
                {
                    schedule.UpdateScheduleType();
                }
            }
        }
        #endregion

        #region NonWorkingHourBrush
        /// <summary>
        /// Gets or sets the color for highlighting non working hours.
        /// </summary>
        /// <remarks>
        /// IsHighlightWorkingHours of schedule must be set as "True" to view non working hours.
        /// </remarks>
        /// <example>
        /// using System.Windows.Media;
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.NonWorkingHourBrush = new SolidColorBrush(Colors.Brown);
        ///             schedule.IsHighLightWorkingHours = true;
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public Brush NonWorkingHourBrush
        {
            get { return (Brush)GetValue(NonWorkingHourBrushProperty); }
            set { SetValue(NonWorkingHourBrushProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for NonWorkingHourBrush.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty NonWorkingHourBrushProperty =
            DependencyProperty.Register("NonWorkingHourBrush", typeof(Brush), typeof(SfSchedule), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xFF, 0xE8, 0xE8, 0xE8))));
        #endregion

        #region ShowNonWorkingHours
        /// <summary>
        /// Gets or sets a value indicating whether the non working hours should be shown.
        /// </summary>
        /// <example>
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.ShowNonWorkingHours = false;
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public bool ShowNonWorkingHours
        {
            get { return (bool)GetValue(ShowNonWorkingHoursProperty); }
            set { SetValue(ShowNonWorkingHoursProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ShowNonWorkingHours.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ShowNonWorkingHoursProperty =
            DependencyProperty.Register("ShowNonWorkingHours", typeof(bool), typeof(SfSchedule), new PropertyMetadata(true, OnShowNonWorkingHoursChanged));

        private static void OnShowNonWorkingHoursChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SfSchedule)
            {
                SfSchedule schedule = d as SfSchedule;
                if (schedule.DragDropCanvas != null && schedule.DragDropCanvas.Children.Count > 0)
                {
                    schedule.DragDropCanvas.Children.Clear();
                    schedule.ResetDragDropAppointmentOpacity();
                }
            }
        }
        #endregion

        #region WorkStartHour
        /// <summary>
        /// Gets or sets the start hour time of working hours.
        /// </summary>
        /// <remarks>
        /// IsHighlightWorkingHours of schedule must be set as "True" to view working hours.
        /// </remarks>
        /// <example>
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.WorkStartHour = 10;
        ///             schedule.IsHighLightWorkingHours = true;
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public int WorkStartHour
        {
            get { return (int)GetValue(WorkStartHourProperty); }
            set { SetValue(WorkStartHourProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for WorkStartHour.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty WorkStartHourProperty =
            DependencyProperty.Register("WorkStartHour", typeof(int), typeof(SfSchedule), new PropertyMetadata(9, OnWorkStartHourChanged));

        private static void OnWorkStartHourChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var schedule = d as SfSchedule;
            if (schedule != null)
            {
                schedule.WorkStartHour = (int)e.NewValue;
                schedule.IsTimeIntervalChanged = !schedule.IsTimeIntervalChanged;
                schedule.WorkStartHour = (schedule.WorkStartHour < 0) ? 0 : (schedule.WorkStartHour > schedule.WorkEndHour) ? schedule.WorkEndHour : schedule.WorkStartHour;
                if (schedule.timelineview != null)
                {
                    schedule.timelineview.GenerateNonworkingdaysItems();
                }
            }
        }
        #endregion

        #region WorkEndHour
        /// <summary>
        /// Gets or sets the end hour time of working hours.
        /// </summary>
        /// <remarks>
        /// IsHighlightWorkingHours of schedule must be set as "True" to view working hours.
        /// </remarks>
        /// <example>
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.WorkEndHour = 17;
        ///             schedule.IsHighLightWorkingHours = true;
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public int WorkEndHour
        {
            get { return (int)GetValue(WorkEndHourProperty); }
            set { SetValue(WorkEndHourProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for WorkEndHour.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty WorkEndHourProperty =
            DependencyProperty.Register("WorkEndHour", typeof(int), typeof(SfSchedule), new PropertyMetadata(18, OnWorkEndHourChanged));

        private static void OnWorkEndHourChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var schedule = d as SfSchedule;
            if (schedule != null)
            {
                schedule.WorkEndHour = (int)e.NewValue;
                schedule.IsTimeIntervalChanged = !schedule.IsTimeIntervalChanged;
                schedule.WorkEndHour = (schedule.WorkEndHour > 24) ? 24 : (schedule.WorkStartHour > schedule.WorkEndHour) ? schedule.WorkStartHour : schedule.WorkEndHour;
                if (schedule.timelineview != null)
                {
                    schedule.timelineview.GenerateNonworkingdaysItems();
                }
            }
        }
        #endregion

        #region IsHightLightWorkingHours
        /// <summary>
        /// Gets or sets a value indicating whether the working hours should be highlighted.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if this instance ; otherwise, <see langword="false" />.
        /// </value>
        /// <example>
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.IsHighLightWorkingHours = true;
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public bool IsHighLightWorkingHours
        {
            get { return (bool)GetValue(IsHighLightWorkingHoursProperty); }
            set { SetValue(IsHighLightWorkingHoursProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsHightLightWorkingHours.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsHighLightWorkingHoursProperty =
            DependencyProperty.Register("IsHighLightWorkingHours", typeof(bool), typeof(SfSchedule), new PropertyMetadata(false));
        #endregion

        #region TimeMode
        /// <summary>
        /// Gets or sets the time mode which may be 12 hrs or 24 hrs.
        /// </summary>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Schedule.TimeModes"></seealso>
        /// <example>
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.TimeMode = TimeModes.TwelveHours;
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public TimeModes TimeMode
        {
            get { return (TimeModes)GetValue(TimeModeProperty); }
            set { SetValue(TimeModeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for TimeMode.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TimeModeProperty =
            DependencyProperty.Register("TimeMode", typeof(TimeModes), typeof(SfSchedule), new PropertyMetadata(TimeModes.TwelveHours));
        #endregion

        #region TimeInterval
        /// <summary>
        /// Gets or sets the time interval which may differs based on enum "TimeInterval" of Schedule.
        /// </summary>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Schedule.TimeInterval"></seealso>
        /// <example>
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.TimeInterval = TimeInterval.ThirtyMin;
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public TimeInterval TimeInterval
        {
            get { return (TimeInterval)GetValue(TimeIntervalProperty); }
            set { SetValue(TimeIntervalProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for TimeInterval.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TimeIntervalProperty =
            DependencyProperty.Register("TimeInterval", typeof(TimeInterval), typeof(SfSchedule), new PropertyMetadata(TimeInterval.ThirtyMin, OnTimeIntervalChanged));

        private static void OnTimeIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var schedule = d as SfSchedule;
            if (schedule != null)
            {
                schedule.IsTimeIntervalChanged = !schedule.IsTimeIntervalChanged;
                if (schedule.timelineview != null)
                {
                    schedule.timelineview.GenerateNonworkingdaysItems();
                }
            }
        }
        #endregion

        #region IntervalHeight
        /// <summary>
        /// Gets or sets the height of interval set for Schedule.
        /// </summary>
        /// <example>
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.IntervalHeight = 30;
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public double IntervalHeight
        {
            get { return (double)GetValue(IntervalHeightProperty); }
            set { SetValue(IntervalHeightProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IntervalHeight.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IntervalHeightProperty =
            DependencyProperty.Register("IntervalHeight", typeof(double), typeof(SfSchedule), new PropertyMetadata(ScheduleTimeLineItemsControl.DefaultIntervalHeight, OnIntervalHeightChanged));

        private static void OnIntervalHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SfSchedule)
            {
                var sch = d as SfSchedule;
                sch.isIntervalHeightset = true;
                if ((double)e.NewValue < 0)
                {
                    (d as SfSchedule).IntervalHeight = 0;
                }
            }
        }
        #endregion

        #region EnableTouch
        /// <summary>
        /// Gets or sets a value indicating whether the touch interactivity for schedule
        /// should be enabled.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if ; otherwise, <see langword="false" />.
        /// </value>
        /// <example>
        /// using System;
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.EnableTouch = true;
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public bool EnableTouch
        {
            get { return (bool)GetValue(EnableTouchProperty); }
            set { SetValue(EnableTouchProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for EnableTouch.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty EnableTouchProperty =
            DependencyProperty.Register("EnableTouch", typeof(bool), typeof(SfSchedule), new PropertyMetadata(false, OnEnableTouchChanged));

        private static void OnEnableTouchChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SfSchedule schedule = d as SfSchedule;
#if WPF
            if (schedule != null&& schedule.isTemplateApplied)
            {

                if (schedule.EnableTouch)
                {
                    schedule.Next_Button.Visibility = Visibility.Visible;
                    schedule.Prev_Button.Visibility = Visibility.Visible;
                    schedule.headerTitleBarView.HeaderNavigationButtonVisibility = Visibility.Collapsed;
                    schedule.headerTitleBarView1.HeaderNavigationButtonVisibility = Visibility.Collapsed;
                    schedule.headerTitleBarView2.HeaderNavigationButtonVisibility = Visibility.Collapsed;
                    schedule.UpdateScheduleType();
                }
                else
                {
                    schedule.Next_Button.Visibility = Visibility.Collapsed;
                    schedule.Prev_Button.Visibility = Visibility.Collapsed; 
                    schedule.headerTitleBarView.HeaderNavigationButtonVisibility = Visibility.Visible;
                    schedule.headerTitleBarView1.HeaderNavigationButtonVisibility = Visibility.Visible;
                    schedule.headerTitleBarView2.HeaderNavigationButtonVisibility = Visibility.Visible;                
                }
                
            }
#endif
        }
        #endregion

        #region TouchMenuType
        /// <summary>
        /// Gets or sets the type of touch context menu which may be default menu or radial menu.
        /// </summary>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Schedule.MenuType"></seealso>
        /// <example>
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.TouchMenuType = MenuType.RadialMenu;
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public MenuType TouchMenuType
        {
            get { return (MenuType)GetValue(TouchMenuTypeProperty); }
            set { SetValue(TouchMenuTypeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for TouchMenuType.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TouchMenuTypeProperty =
            DependencyProperty.Register("TouchMenuType", typeof(MenuType), typeof(SfSchedule), new PropertyMetadata(MenuType.RadialMenu, OnTouchMenuTypeChanged));

        private static void OnTouchMenuTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var schedule = d as SfSchedule;
            if (schedule != null && schedule.EnableTouch && schedule.addnewpopup != null && schedule.editpopup != null)
            {
                schedule.addnewpopup.Child = null;
                schedule.editpopup.Child = null;
                if (schedule.TouchMenuType == MenuType.Default)
                {
                    var appcontrol = new AddAppintmentControl();
                    schedule.addnewpopup.Child = appcontrol;
                    var drag = new DragDropControl();
                    schedule.editpopup.Child = drag;
                }
                else
                {
                    var radialmenu = new AddRadialMenuControl();
                    var editradial = new EditRadialMenuControl();
                    schedule.addnewpopup.Child = radialmenu;
                    schedule.editpopup.Child = editradial;
                }
            }
        }
        #endregion

        #region CurrentTimeIndicatorTemplate
        /// <summary>
        /// Gets or sets the template for customizing current time indicator.
        /// </summary>
        /// <remarks>
        /// CurrentTimeIndicatorVisibility of Schedule should be set as Visible to view the
        /// current time indicator.
        /// </remarks>
        /// <example>
        /// using System.Windows;
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.CurrentTimeIndicatorVisibility = Visibility.Visible;
        ///             schedule.CurrentTimeIndicatorTemplate = (DataTemplate)this.Resources["CurrentTimeIndicatorTemplate"];
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public DataTemplate CurrentTimeIndicatorTemplate
        {
            get { return (DataTemplate)GetValue(CurrentTimeIndicatorTemplateProperty); }
            set { SetValue(CurrentTimeIndicatorTemplateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for CurrentTimeIndicatorTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        static readonly DependencyProperty CurrentTimeIndicatorTemplateProperty =
              DependencyProperty.Register("CurrentTimeIndicatorTemplate", typeof(DataTemplate), typeof(SfSchedule), new PropertyMetadata(null));
        #endregion

        #region CurrentTimeIndicatorVisibility
        /// <summary>
        /// Gets or sets the visibility of current time indicator.
        /// </summary>
        /// <example>
        /// using System.Windows;
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.CurrentTimeIndicatorVisibility = Visibility.Visible;
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public Visibility CurrentTimeIndicatorVisibility
        {
            get { return (Visibility)GetValue(CurrentTimeIndicatorVisibilityProperty); }
            set { SetValue(CurrentTimeIndicatorVisibilityProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for CurrentTimeIndicatorVisibility.  This enables animation, styling, binding, etc...
        /// </summary>
        static readonly DependencyProperty CurrentTimeIndicatorVisibilityProperty =
              DependencyProperty.Register("CurrentTimeIndicatorVisibility", typeof(Visibility), typeof(SfSchedule), new PropertyMetadata(Visibility.Collapsed));
        #endregion

        #region EnableReminderTimer
        /// <summary>
        /// Gets or sets a value indicating whether the reminder timer should be enabled.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if ; otherwise, <see langword="false" />.
        /// </value>
        /// <example>
        /// using System;
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.Appointments.Add(new ScheduleAppointment
        ///             {
        ///                 Subject = "Meeting",
        ///                 StartTime = DateTime.Now.AddHours(1),
        ///                 EndTime = DateTime.Now.AddHours(2),
        ///                 ReminderTime = ReminderTimeType.OneHour
        ///             });
        ///             schedule.EnableReminderTimer = false;
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public bool EnableReminderTimer
        {
            get { return (bool)GetValue(EnableReminderTimerProperty); }
            set { SetValue(EnableReminderTimerProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for EnableReminderTimer.  This enables animation, styling, binding, etc...
        /// </summary>
        static readonly DependencyProperty EnableReminderTimerProperty =
              DependencyProperty.Register("EnableReminderTimer", typeof(bool), typeof(SfSchedule), new PropertyMetadata(true, OnEnableReminderTimerChanged));

        private static void OnEnableReminderTimerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var scheduleInstance = d as SfSchedule;
            if (scheduleInstance != null)
                scheduleInstance.GenerateNotification();
        }
        #endregion

        #region EnableAutoFormat
        /// <summary>
        /// Gets or sets a value indicating whether the auto format should be enabled.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if ; otherwise, <see langword="false" />.
        /// </value>
        /// <example>
        /// using System;
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.EnableAutoFormat = false;
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public bool EnableAutoFormat
        {
            get { return (bool)GetValue(EnableAutoFormatProperty); }
            set { SetValue(EnableAutoFormatProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for EnableAutoFormat.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty EnableAutoFormatProperty =
            DependencyProperty.Register("EnableAutoFormat", typeof(bool), typeof(SfSchedule), new PropertyMetadata(true, OnEnableAutoFormatChanged));

        private static void OnEnableAutoFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SfSchedule)
            {
                SfSchedule schedule = (d as SfSchedule);
                schedule.HeaderFormat = schedule.EnableAutoFormat && schedule.needAutoFormat ? "ddd" : schedule.HeaderDateFormat;
            }
        }
        #endregion

        #region ScheduleDateRange
        /// <summary>
        /// Gets or sets the dates that need to be displayed in day view and timeline view of schedule.
        /// </summary>
        /// <remarks>
        /// CustomVisibleDate property helps user to view particular dates in a single view.
        /// </remarks>
        /// <example>
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.ScheduleDateRange = new ObservableCollection<DateTime> { new DateTime(2013, 11, 21), new DateTime(2013, 11, 23), new DateTime(2013, 11, 25), new DateTime(2013, 11, 27) };
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>


        public ObservableCollection<DateTime> ScheduleDateRange
        {
            get { return (ObservableCollection<DateTime>)GetValue(ScheduleDateRangeProperty); }
            set { SetValue(ScheduleDateRangeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ScheduleDateRange.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ScheduleDateRangeProperty =
            DependencyProperty.Register("ScheduleDateRange", typeof(ObservableCollection<DateTime>), typeof(SfSchedule), new PropertyMetadata(null, OnScheduleDateRangeChanged));

        private static void OnScheduleDateRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SfSchedule schedule = d as SfSchedule;
            if (schedule != null && schedule.ScheduleDateRange != null )
            {
                if( schedule.ScheduleDateRange.Count > 0)
                schedule.VisibleDates = new ObservableCollection<DateTime>(schedule.ScheduleDateRange.OrderBy(p => p.Date).Distinct());
                schedule.ScheduleDateRange.CollectionChanged -= schedule.ScheduleDateRange_CollectionChanged;
                schedule.ScheduleDateRange.CollectionChanged += schedule.ScheduleDateRange_CollectionChanged;
            }
        }

        #endregion

        #region NonAccessibleBlocks
        /// <summary>
        /// Gets or sets the non accessible blocks in schedule.
        /// </summary>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Schedule.NonAccessibleBlockCollection"/>
        /// <example>
        /// using System;
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.NonAccessibleBlocks.Add(new NonAccessibleBlock
        ///             {
        ///                 StartHour = 2,
        ///                 EndHour = 4,
        ///                 Label = "Main Block",
        ///             });
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public NonAccessibleBlockCollection NonAccessibleBlocks
        {
            get { return (NonAccessibleBlockCollection)GetValue(NonAccessibleBlocksProperty); }
            set { SetValue(NonAccessibleBlocksProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NonAccessibleBlocks.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NonAccessibleBlocksProperty =
            DependencyProperty.Register("NonAccessibleBlocks", typeof(NonAccessibleBlockCollection), typeof(SfSchedule), new PropertyMetadata(null, OnNonAccessibleBlocksChanged));

        private static void OnNonAccessibleBlocksChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SfSchedule)
            {
                var sfSchedule = d as SfSchedule;
                if (sfSchedule.NonAccessibleBlocks != null && sfSchedule.NonAccessibleBlocks != null)
                {
                    sfSchedule.NonAccessibleBlocks.CollectionChanged -= sfSchedule.NonAccessibleBlocks_CollectionChanged;
                    sfSchedule.NonAccessibleBlocks.CollectionChanged += sfSchedule.NonAccessibleBlocks_CollectionChanged;
                }
            }
        }
        #endregion

        #region NonAccessibleBlockTemplate
        /// <summary>
        /// Gets or sets the template for customizing non accessible blocks of schedule.
        /// </summary>
        /// <example>
        /// using System;
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.NonAccessibleBlocks.Add(new NonAccessibleBlock
        ///             {
        ///                 StartHour = 2,
        ///                 EndHour = 4,
        ///                 Label = "Main Block",
        ///             });
        ///             schedule.NonAccessibleBlockTemplate = (DataTemplate)this.Resources["NonAccessibleBlockTemplate"];
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public DataTemplate NonAccessibleBlockTemplate
        {
            get { return (DataTemplate)GetValue(NonAccessibleBlockTemplateProperty); }
            set { SetValue(NonAccessibleBlockTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NonAccessibleBlockTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NonAccessibleBlockTemplateProperty =
            DependencyProperty.Register("NonAccessibleBlockTemplate", typeof(DataTemplate), typeof(SfSchedule), new PropertyMetadata(null));
        #endregion

        #endregion

        #region Read-only Properties

        #region TimeZoneCollection
        /// <summary>
        /// Gets the collection of time zones available in schedule.
        /// </summary>
        /// <seealso
        /// cref="T:Syncfusion.UI.Xaml.Schedule.TimeZoneCollection">TimeZoneCollection</seealso>
        public TimeZoneCollection TimeZoneCollection
        {
            get { return (TimeZoneCollection)GetValue(TimeZoneCollectionProperty); }
            internal set { SetValue(TimeZoneCollectionProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for TimeZoneCollection.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TimeZoneCollectionProperty =
            DependencyProperty.Register("TimeZoneCollection", typeof(TimeZoneCollection), typeof(SfSchedule), new PropertyMetadata(null));
        #endregion

        #region NonWorkingDateCollection
        /// <summary>
        /// Gets the collection of non-working days.
        /// </summary>
        /// <seealso cref="T:System.DayOfWeek"/>
        public ObservableCollection<DayOfWeek> NonWorkingDateCollection
        {
            get { return (ObservableCollection<DayOfWeek>)GetValue(NonWorkingDateCollectionProperty); }
            internal set { SetValue(NonWorkingDateCollectionProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for NonWorkingDateCollection.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty NonWorkingDateCollectionProperty =
            DependencyProperty.Register("NonWorkingDateCollection", typeof(ObservableCollection<DayOfWeek>), typeof(SfSchedule), new PropertyMetadata(null));
        #endregion

        #region VisibleDates
        /// <summary>
        /// Gets the visible dates of Schedule's current view.
        /// </summary>
        /// <seealso cref="T:System.DateTime"></seealso>
        public ObservableCollection<DateTime> VisibleDates
        {
            get { return (ObservableCollection<DateTime>)GetValue(VisibleDatesProperty); }
            internal set { SetValue(VisibleDatesProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for VisibleDates.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty VisibleDatesProperty =
            DependencyProperty.Register("VisibleDates", typeof(ObservableCollection<DateTime>), typeof(SfSchedule), new PropertyMetadata(null, OnVisibleSelectedDatesChanged));
        private static void OnVisibleSelectedDatesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SfSchedule schedule = d as SfSchedule;
            if (schedule.ScheduleDateRange != null && !schedule.IsVisibleDateSetInternally)
            {
                if (schedule.ScheduleType != ScheduleType.Day && schedule.ScheduleType != ScheduleType.TimeLine)
                {
                    schedule.ScheduleTypeChangedInternally = true;
                    schedule.ScheduleType = ScheduleType.Day;
                }
                else if (schedule.ScheduleType == ScheduleType.Day)
                {
                    schedule.ScheduleTypeToDay = true;
                }
                else if (schedule.ScheduleType == ScheduleType.TimeLine)
                {
                    schedule.ScheduleTypeToTimeline = true;
                }
                schedule.SetSelectedDatesFromVisibleDates();
            }
        }
        #endregion

        #region VisibleAppointments
        /// <summary>
        /// Gets the collection of appointments that are visible.
        /// </summary>
        /// <seealso
        /// cref="T:Syncfusion.UI.Xaml.Schedule.ScheduleAppointmentCollection"></seealso>
        public ScheduleAppointmentCollection VisibleAppointments
        {
            get { return (ScheduleAppointmentCollection)GetValue(VisibleAppointmentsProperty); }
            internal set { SetValue(VisibleAppointmentsProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for VisibleAppointments.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty VisibleAppointmentsProperty =
            DependencyProperty.Register("VisibleAppointments", typeof(ScheduleAppointmentCollection), typeof(SfSchedule), new PropertyMetadata(null));
        #endregion

        #endregion

        #region Internal Properties

        #region ResourceHeaderVisiblity
        internal Visibility ResourceHeaderVisibility
        {
            get { return (Visibility)GetValue(ResourceHeaderVisibilityProperty); }
            set { SetValue(ResourceHeaderVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ResourceHeaderVisibility.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ResourceHeaderVisibilityProperty =
            DependencyProperty.Register("ResourceHeaderVisibility", typeof(Visibility), typeof(SfSchedule), new PropertyMetadata(Visibility.Visible));
        #endregion

        #region ScheduleResourceType
        internal ResourceType ScheduleResourceType
        {
            get { return (ResourceType)GetValue(ScheduleResourceTypeProperty); }
            set { SetValue(ScheduleResourceTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ScheduleResourceType.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ScheduleResourceTypeProperty =
            DependencyProperty.Register("ScheduleResourceType", typeof(ResourceType), typeof(SfSchedule), new PropertyMetadata(null));
        #endregion

        #region IsTimeIntervalChanged
        internal bool IsTimeIntervalChanged
        {
            get { return (bool)GetValue(IsTimeIntervalChangedProperty); }
            set { SetValue(IsTimeIntervalChangedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsTimeIntervalChanged.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty IsTimeIntervalChangedProperty =
            DependencyProperty.Register("IsTimeIntervalChanged", typeof(bool), typeof(SfSchedule), new PropertyMetadata(false));
        #endregion

        #region InternalSelectedDate
        internal DateTime InternalSelectedDate
        {
            get { return (DateTime)GetValue(InternalSelectedDateProperty); }
            set { SetValue(InternalSelectedDateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InternalSelectedDate.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty InternalSelectedDateProperty =
            DependencyProperty.Register("InternalSelectedDate", typeof(DateTime), typeof(SfSchedule), new PropertyMetadata(DateTime.Now));
        #endregion

        #region MinMaxSelection
        internal DateTime MinMaxSelection
        {
            get { return (DateTime)GetValue(MinMaxSelectionProperty); }
            set { SetValue(MinMaxSelectionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinMaxSelection.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty MinMaxSelectionProperty =
            DependencyProperty.Register("MinMaxSelection", typeof(DateTime), typeof(SfSchedule), new PropertyMetadata(new DateTime()));
        #endregion

        #region SelectedDate
        internal DateTime SelectedDate
        {
            get { return (DateTime)GetValue(SelectedDateProperty); }
            set { SetValue(SelectedDateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedDate.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty SelectedDateProperty =
            DependencyProperty.Register("SelectedDate", typeof(DateTime), typeof(SfSchedule), new PropertyMetadata(DateTime.Now.Date));
        #endregion

        #region SelectedDates
        internal ObservableCollection<DateTime> SelectedDates
        {
            get { return (ObservableCollection<DateTime>)GetValue(SelectedDatesProperty); }
            set { SetValue(SelectedDatesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedDates.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty SelectedDatesProperty =
            DependencyProperty.Register("SelectedDates", typeof(ObservableCollection<DateTime>), typeof(SfSchedule), new PropertyMetadata(null, SelectedDatesChanged));

        private static void SelectedDatesChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var schedule = dpo as SfSchedule;
            if (schedule != null)
            {
                schedule.CurrentSelectedDates = schedule.SelectedDates;
                schedule.SetNextDate();
                schedule.SetPrevDate();
            }
        }
        #endregion

        #region PrevSelectedDates
        internal ObservableCollection<DateTime> PrevSelectedDates
        {
            get { return (ObservableCollection<DateTime>)GetValue(PrevSelectedDatesProperty); }
            set { SetValue(PrevSelectedDatesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedDates.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty PrevSelectedDatesProperty =
            DependencyProperty.Register("PrevSelectedDates", typeof(ObservableCollection<DateTime>), typeof(SfSchedule), new PropertyMetadata(null, PrevSelectedDatesChanged));

        private static void PrevSelectedDatesChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var schedule = dpo as SfSchedule;
            if (schedule != null)
            {
                foreach (DateTime dt in schedule.PrevSelectedDates)
                {
                    schedule.AddRecursiveAppointmentsCopy(dt.Date);
                }
                schedule.SetPrevVisibleAppointments(schedule.PrevSelectedDates);
            }
        }
        #endregion

        #region CurrentSelectedDates
        internal ObservableCollection<DateTime> CurrentSelectedDates
        {
            get { return (ObservableCollection<DateTime>)GetValue(CurrentSelectedDatesProperty); }
            set { SetValue(CurrentSelectedDatesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedDates.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty CurrentSelectedDatesProperty =
            DependencyProperty.Register("CurrentSelectedDates", typeof(ObservableCollection<DateTime>), typeof(SfSchedule), new PropertyMetadata(null, CurrentSelectedDatesChanged));

        private static void CurrentSelectedDatesChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var schedule = dpo as SfSchedule;
            if (schedule != null)
            {
                foreach (DateTime dt in schedule.CurrentSelectedDates)
                {
                    schedule.AddRecursiveAppointmentsCopy(dt.Date);
                }
                schedule.SetCurrentVisibleAppointments(schedule.CurrentSelectedDates);
            }
        }
        #endregion

        #region NextSelectedDates
        internal ObservableCollection<DateTime> NextSelectedDates
        {
            get { return (ObservableCollection<DateTime>)GetValue(NextSelectedDatesProperty); }
            set { SetValue(NextSelectedDatesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedDates.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty NextSelectedDatesProperty =
            DependencyProperty.Register("NextSelectedDates", typeof(ObservableCollection<DateTime>), typeof(SfSchedule), new PropertyMetadata(null, NextSelectedDatesChanged));

        private static void NextSelectedDatesChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var schedule = dpo as SfSchedule;
            if (schedule != null)
            {
                foreach (DateTime dt in schedule.NextSelectedDates)
                {
                    schedule.AddRecursiveAppointmentsCopy(dt.Date);
                }
                schedule.SetNextVisibleAppointments(schedule.NextSelectedDates);
            }
        }
        #endregion

        #region ProxyAppointments
        internal Dictionary<DateTime, ObservableCollection<ScheduleAppointment>> ProxyAppointments
        {
            get { return (Dictionary<DateTime, ObservableCollection<ScheduleAppointment>>)GetValue(ProxyAppointmentsProperty); }
            set { SetValue(ProxyAppointmentsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ProxyAppointmentCollection.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ProxyAppointmentsProperty =
            DependencyProperty.Register("ProxyAppointments", typeof(Dictionary<DateTime, ObservableCollection<ScheduleAppointment>>), typeof(SfSchedule), new PropertyMetadata(new Dictionary<DateTime, ObservableCollection<ScheduleAppointment>>()));
        #endregion

        #region PrevVisibleappointments
        internal ScheduleAppointmentCollection PrevVisibleAppointments
        {
            get { return (ScheduleAppointmentCollection)GetValue(PrevVisibleAppointmentsProperty); }
            set { SetValue(PrevVisibleAppointmentsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VisibleAppointments.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty PrevVisibleAppointmentsProperty =
            DependencyProperty.Register("PrevVisibleAppointments", typeof(ScheduleAppointmentCollection), typeof(SfSchedule), new PropertyMetadata(null));
        #endregion

        #region CurrentVisibleAppointments
        internal ScheduleAppointmentCollection CurrentVisibleAppointments
        {
            get { return (ScheduleAppointmentCollection)GetValue(CuurentVisibleAppointmentsProperty); }
            set { SetValue(CuurentVisibleAppointmentsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VisibleAppointments.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty CuurentVisibleAppointmentsProperty =
            DependencyProperty.Register("CurrentVisibleAppointments", typeof(ScheduleAppointmentCollection), typeof(SfSchedule), new PropertyMetadata(null));
        #endregion

        #region NextVisibleAppointments
        internal ScheduleAppointmentCollection NextVisibleAppointments
        {
            get { return (ScheduleAppointmentCollection)GetValue(NextVisibleAppointmentsProperty); }
            set { SetValue(NextVisibleAppointmentsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VisibleAppointments.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty NextVisibleAppointmentsProperty =
            DependencyProperty.Register("NextVisibleAppointments", typeof(ScheduleAppointmentCollection), typeof(SfSchedule), new PropertyMetadata(null));
        #endregion

        #region RecursiveAppointments
        internal ScheduleAppointmentCollection RecursiveAppointments
        {
            get { return (ScheduleAppointmentCollection)GetValue(RecursiveAppointmentsProperty); }
            set { SetValue(RecursiveAppointmentsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RecursiveAppointments.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty RecursiveAppointmentsProperty =
            DependencyProperty.Register("RecursiveAppointments", typeof(ScheduleAppointmentCollection), typeof(SfSchedule), new PropertyMetadata(null));
        #endregion

        #region VisibleRecursiveAppointments
        internal ScheduleAppointmentCollection VisibleRecursiveAppointments
        {
            get { return (ScheduleAppointmentCollection)GetValue(VisibleRecursiveAppointmentsProperty); }
            set { SetValue(VisibleRecursiveAppointmentsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VisibleRecursiveAppointments.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty VisibleRecursiveAppointmentsProperty =
            DependencyProperty.Register("VisibleRecursiveAppointments", typeof(ScheduleAppointmentCollection), typeof(SfSchedule), new PropertyMetadata(null));
        #endregion

        #region RecursiveAddedDates
        internal Dictionary<double, ObservableCollection<DateTime>> RecursiveAddedDates
        {
            get { return (Dictionary<double, ObservableCollection<DateTime>>)GetValue(RecursiveAddedDatesProperty); }
            set { SetValue(RecursiveAddedDatesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VisibleRecursinveAppointments.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty RecursiveAddedDatesProperty =
            DependencyProperty.Register("RecursiveAddedDates", typeof(Dictionary<double, ObservableCollection<DateTime>>), typeof(SfSchedule), new PropertyMetadata(new Dictionary<double, ObservableCollection<DateTime>>()));
        #endregion

        #region HeaderFormat
        internal string HeaderFormat
        {
            get { return (string)GetValue(HeaderFormatProperty); }
            set { SetValue(HeaderFormatProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for HeaderFormat.  This enables animation, styling, binding, etc...
        /// </summary>
        internal static readonly DependencyProperty HeaderFormatProperty =
            DependencyProperty.Register("HeaderFormat", typeof(string), typeof(SfSchedule), new PropertyMetadata("dddd dd"));
        #endregion

        #endregion

        #endregion

        #region Implementation Methods

        #region Clearing Appointments

        private void ClearAppointments()
        {
            if (PrevVisibleAppointments != null)
            {
                PrevVisibleAppointments.Clear();
                PrevVisibleAppointments = new ScheduleAppointmentCollection();
            }
            if (CurrentVisibleAppointments != null)
            {
                CurrentVisibleAppointments.Clear();
                CurrentVisibleAppointments = new ScheduleAppointmentCollection();
            }
            if (NextVisibleAppointments != null)
            {
                NextVisibleAppointments.Clear();
                NextVisibleAppointments = new ScheduleAppointmentCollection();
            }
            if (ProxyAppointments != null)
            {
                ProxyAppointments.Clear();
                ProxyAppointments = new Dictionary<DateTime, ObservableCollection<ScheduleAppointment>>();
            }
            if (RecursiveAppointments != null)
            {
                RecursiveAppointments.Clear();
            }
        }

        #endregion

        #region Setting ItemsSource

        void SetItemsSource()
        {
            var handler = ItemsSourceChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
            if (AppointmentMapping != null)
            {
                CreateAppointmentsForItemsSource();
            }
        }

        void UnwireItemSource(IEnumerable source)
        {
            var collectionChanged = source as INotifyCollectionChanged;
            if (collectionChanged != null)
            {
                collectionChanged.CollectionChanged -= ItemsSource_CollectionChanged;
            }
            foreach (object o in source)
            {
                var changed = o as INotifyPropertyChanged;
                if (changed != null)
                {
                    changed.PropertyChanged -= ItemsSource_PropertyChanged;
                }
            }
        }

        void WireItemSource(IEnumerable source)
        {
            var changed = source as INotifyCollectionChanged;
            if (changed != null)
            {
                changed.CollectionChanged += ItemsSource_CollectionChanged;
            }
            foreach (object o in source)
            {
                var propertyChanged = o as INotifyPropertyChanged;
                if (propertyChanged != null)
                {
                    propertyChanged.PropertyChanged += ItemsSource_PropertyChanged;
                }
            }
        }

        internal bool CheckItemsInItemsSource()
        {
            if (ItemsSource is IEnumerable)
            {
                return (ItemsSource as IEnumerable).OfType<INotifyPropertyChanged>().Any();
            }
            return false;
        }

        internal void ChangeScheduleType(SfSchedule schedule, string OldValue, string NewValue)
        {
            if (schedule.DragDropCanvas != null && schedule.DragDropCanvas.Children != null && schedule.DragDropCanvas.Children.Count > 0)
            {
                schedule.Dayviewrb.Detach();
                schedule.Monthviewrb.Detach();
                schedule.Timelineviewrb.Detach();
                var control = schedule.DragDropCanvas.Children[0] as Control;
                if (control != null)
                {
                    control.MouseLeftButtonDown -= schedule.drag_app_MouseLeftButtonDown;
                    control.Loaded -= schedule.drag_app_Loaded;
                }
                schedule.DragDropCanvas.Children.Clear();
                schedule.ResetDragDropAppointmentOpacity();
            }
            if (OldValue != NewValue)
            {
                schedule.UpdateScheduleType();
                schedule.SetNavigationTap();
                if (schedule.editpopup != null && schedule.editpopup.IsOpen)
                    schedule.editpopup.IsOpen = false;
                if (schedule.addnewpopup != null && schedule.addnewpopup.IsOpen)
                    schedule.addnewpopup.IsOpen = false;
            }
        }

        #endregion

        #region DayView Binding

        void SetDayViewBinding()
        {
            var Scheduletypebinding = new Binding { Path = new PropertyPath("ScheduleResourceType"), Source = this };
            var showalldaybinding = new Binding { Path = new PropertyPath("ShowAllDay"), Source = this };
            var daysviewSelectedDateBinding = new Binding { Path = new PropertyPath("CurrentSelectedDates"), Source = this };
            var daysviewNextSelectedDateBinding = new Binding { Path = new PropertyPath("NextSelectedDates"), Source = this };
            var daysviewPreSelectedDateBinding = new Binding { Path = new PropertyPath("PrevSelectedDates"), Source = this };           
            var daysviewVisibleAppointmentsBinding = new Binding { Path = new PropertyPath("CurrentVisibleAppointments"), Source = this };
            var daysviewPrevVisibleAppointmentsBinding = new Binding { Path = new PropertyPath("PrevVisibleAppointments"), Source = this };
            var daysviewNextVisibleAppointmentsBinding = new Binding { Path = new PropertyPath("NextVisibleAppointments"), Source = this };
            var daysviewScheduleTypeBinding = new Binding { Path = new PropertyPath("ScheduleType"), Source = this };
            var daysviewTimeIntervaleBinding = new Binding { Path = new PropertyPath("TimeInterval"), Source = this };
            var daysviewTimeModeBinding = new Binding { Path = new PropertyPath("TimeMode"), Source = this };
            var daysviewIntervalHeightBinding = new Binding { Path = new PropertyPath("IntervalHeight"), Source = this };
            var daysviewTimelineHourDivisionVisibilityBinding = new Binding { Path = new PropertyPath("MajorTickVisibility"), Source = this };
            var daysviewTimelineVisibilityBinding = new Binding { Path = new PropertyPath("MinorTickVisibility"), Source = this };
            var dayviewcurrentdate = new Binding { Path = new PropertyPath("CurrentDateBackground"), Source = this };
            var workingddaycollection = new Binding { Path = new PropertyPath("NonWorkingDateCollection"), Source = this };
            var nonworkingdaysbackground = new Binding { Path = new PropertyPath("NonWorkingHourBrush"), Source = this };
            var timeinterval = new Binding { Path = new PropertyPath("IsTimeIntervalChanged"), Source = this };
            var highlighthoursbinding = new Binding { Path = new PropertyPath("IsHighLightWorkingHours"), Source = this };
            var dayViewVerticaLineStrokebinding = new Binding { Path = new PropertyPath("DayViewVerticaLineStroke"), Source = this };
            var majorTickStrokeDashArrayBinding = new Binding { Path = new PropertyPath("MajorTickStrokeDashArray"), Source = this };
            var minorTickStrokeDashArrayBinding = new Binding { Path = new PropertyPath("MinorTickStrokeDashArray"), Source = this };
            var MinorTickStroke = new Binding { Path = new PropertyPath("MinorTickStroke"), Source = this };
            var dayheaderbackground = new Binding { Path = new PropertyPath("HeaderBackground"), Source = this };
            var MajorTickStroke = new Binding { Path = new PropertyPath("MajorTickStroke"), Source = this };
            var majorTickLabelStroke = new Binding { Path = new PropertyPath("MajorTickLabelStroke"), Source = this };
            var minorTickLabelStroke = new Binding { Path = new PropertyPath("MinorTickLabelStroke"), Source = this };
            var DayResourceviewbinding = new Binding { Path = new PropertyPath("DayHeaderOrder"), Source = this };
            var dayViewColumnCountBinding = new Binding { Path = new PropertyPath("DayViewColumnCount"), Source = this };
            var appointmentselectionbrushbinding = new Binding { Path = new PropertyPath("AppointmentSelectionBrush"), Source = this };
            var appointmentTemplateBinding = new Binding { Path = new PropertyPath("AppointmentTemplate"), Source = this };
            var appointmentToolTipTemplateBinding = new Binding { Path = new PropertyPath("AppointmentToolTipTemplate"), Source = this };
            var appointmentTooltipVisibilityBinding = new Binding { Path = new PropertyPath("AppointmentTooltipVisibility"), Source = this };
            var showNavigationTapBinding = new Binding { Source = this, Path = new PropertyPath("ShowAppointmentNavigationButtons") };
            var prevNavigationTapTemplateBinding = new Binding { Source = this, Path = new PropertyPath("PreviousNavigationButtonTemplate") };
            var nextNavigationTapTemplateBinding = new Binding { Source = this, Path = new PropertyPath("NextNavigationButtonTemplate") };
            var showNonWorkingHoursBinding = new Binding { Path = new PropertyPath("ShowNonWorkingHours"), Source = this };
            var workStartHourBinding = new Binding { Path = new PropertyPath("WorkStartHour"), Source = this };
            var workEndHourBinding = new Binding { Path = new PropertyPath("WorkEndHour"), Source = this };
            var nonAccessibleBlocksBinding = new Binding { Path = new PropertyPath("NonAccessibleBlocks"), Source = this };
            var currentTimeIndicatorTemplateBinding = new Binding { Path = new PropertyPath("CurrentTimeIndicatorTemplate"), Source = this };
            var currentTimeIndicatorVisibilityBinding = new Binding { Path = new PropertyPath("CurrentTimeIndicatorVisibility"), Source = this };

            #region Binding daysView
            BindingOperations.SetBinding(daysview, ScheduleDaysView.ShowAllDayProperty, showalldaybinding);
            BindingOperations.SetBinding(daysview, ScheduleDaysView.IsHighLightWorkingHoursProperty, highlighthoursbinding);
            BindingOperations.SetBinding(daysview, ScheduleDaysView.HeaderBackgroundProperty, dayheaderbackground);
            BindingOperations.SetBinding(daysview, ScheduleDaysView.MajorTickStrokeProperty, MajorTickStroke);
            BindingOperations.SetBinding(daysview, ScheduleDaysView.MinorTickStrokeProperty, MinorTickStroke);
            BindingOperations.SetBinding(daysview, ScheduleDaysView.MajorTickLabelStrokeProperty, majorTickLabelStroke);
            BindingOperations.SetBinding(daysview, ScheduleDaysView.MinorTickLabelStrokeProperty, minorTickLabelStroke);
            BindingOperations.SetBinding(daysview, ScheduleDaysView.IsTimeIntervalChangedProperty, timeinterval);
            BindingOperations.SetBinding(daysview, ScheduleDaysView.NonWorkingHourBrushProperty, nonworkingdaysbackground);
            BindingOperations.SetBinding(daysview, ScheduleDaysView.CurrentDateBackgroundProperty, dayviewcurrentdate);
            BindingOperations.SetBinding(daysview, ScheduleDaysView.NonWorkingDateCollectionProperty, workingddaycollection);
            BindingOperations.SetBinding(daysview, ScheduleDaysView.SelectedDatesProperty, daysviewSelectedDateBinding);
            BindingOperations.SetBinding(daysview, ScheduleDaysView.CurrentScheduleTypeProperty, daysviewScheduleTypeBinding);
            BindingOperations.SetBinding(daysview, ScheduleDaysView.TimeIntervalProperty, daysviewTimeIntervaleBinding);
            BindingOperations.SetBinding(daysview, ScheduleDaysView.TimeModeProperty, daysviewTimeModeBinding);
            BindingOperations.SetBinding(daysview, ScheduleDaysView.IntervalHeightProperty, daysviewIntervalHeightBinding);
            BindingOperations.SetBinding(daysview, ScheduleDaysView.MajorTickVisibilityProperty, daysviewTimelineHourDivisionVisibilityBinding);
            BindingOperations.SetBinding(daysview, ScheduleDaysView.MinorTickVisibilityProperty, daysviewTimelineVisibilityBinding);
            BindingOperations.SetBinding(daysview, ScheduleDaysView.VisibleAppointmentsProperty, daysviewVisibleAppointmentsBinding);
            BindingOperations.SetBinding(daysview, ScheduleDaysView.DayHeaderOrderProperty, DayResourceviewbinding);
            BindingOperations.SetBinding(daysview, ScheduleDaysView.DayViewColumnCountProperty, dayViewColumnCountBinding);
            BindingOperations.SetBinding(daysview, ScheduleDaysView.AppointmentSelectionBrushProperty, appointmentselectionbrushbinding);
            BindingOperations.SetBinding(daysview, ScheduleDaysView.AppointmentTemplateProperty, appointmentTemplateBinding);
            BindingOperations.SetBinding(daysview, ScheduleDaysView.AppointmentToolTipTemplateProperty, appointmentToolTipTemplateBinding);
            BindingOperations.SetBinding(daysview, ScheduleDaysView.AppointmentTooltipVisibilityProperty, appointmentTooltipVisibilityBinding);
            BindingOperations.SetBinding(daysview, ScheduleDaysView.ScheduleResourceTypeProperty, Scheduletypebinding);
            BindingOperations.SetBinding(daysview, ScheduleDaysView.DayViewVerticaLineStrokeProperty, dayViewVerticaLineStrokebinding);
            BindingOperations.SetBinding(daysview, ScheduleDaysView.MajorTickStrokeDashArrayProperty, majorTickStrokeDashArrayBinding);
            BindingOperations.SetBinding(daysview, ScheduleDaysView.MinorTickStrokeDashArrayProperty, minorTickStrokeDashArrayBinding);
            BindingOperations.SetBinding(daysview, ScheduleDaysView.ShowAppointmentNavigationButtonsProperty, showNavigationTapBinding);
            BindingOperations.SetBinding(daysview, ScheduleDaysView.PreviousNavigationButtonTemplateProperty, prevNavigationTapTemplateBinding);
            BindingOperations.SetBinding(daysview, ScheduleDaysView.NextNavigationButtonTemplateProperty, nextNavigationTapTemplateBinding);
            BindingOperations.SetBinding(daysview, ScheduleDaysView.ShowNonWorkingHoursProperty, showNonWorkingHoursBinding);
            BindingOperations.SetBinding(daysview, ScheduleDaysView.WorkStartHourProperty, workStartHourBinding);
            BindingOperations.SetBinding(daysview, ScheduleDaysView.WorkEndHourProperty, workEndHourBinding);
            BindingOperations.SetBinding(daysview, ScheduleDaysView.NonAccessibleBlocksProperty, nonAccessibleBlocksBinding);
            BindingOperations.SetBinding(daysview, ScheduleDaysView.CurrentTimeIndicatorTemplateProperty, currentTimeIndicatorTemplateBinding);
            BindingOperations.SetBinding(daysview, ScheduleDaysView.CurrentTimeIndicatorVisibilityProperty, currentTimeIndicatorVisibilityBinding);
            #endregion
#if WPF
            #region Binding daysView1
            BindingOperations.SetBinding(daysview1, ScheduleDaysView.ShowAllDayProperty, showalldaybinding);
            BindingOperations.SetBinding(daysview1, ScheduleDaysView.IsHighLightWorkingHoursProperty, highlighthoursbinding);
            BindingOperations.SetBinding(daysview1, ScheduleDaysView.HeaderBackgroundProperty, dayheaderbackground);
            BindingOperations.SetBinding(daysview1, ScheduleDaysView.MajorTickStrokeProperty, MajorTickStroke);
            BindingOperations.SetBinding(daysview1, ScheduleDaysView.MinorTickStrokeProperty, MinorTickStroke);
            BindingOperations.SetBinding(daysview1, ScheduleDaysView.MajorTickLabelStrokeProperty, majorTickLabelStroke);
            BindingOperations.SetBinding(daysview1, ScheduleDaysView.MinorTickLabelStrokeProperty, minorTickLabelStroke);
            BindingOperations.SetBinding(daysview1, ScheduleDaysView.IsTimeIntervalChangedProperty, timeinterval);
            BindingOperations.SetBinding(daysview1, ScheduleDaysView.NonWorkingHourBrushProperty, nonworkingdaysbackground);
            BindingOperations.SetBinding(daysview1, ScheduleDaysView.CurrentDateBackgroundProperty, dayviewcurrentdate);
            BindingOperations.SetBinding(daysview1, ScheduleDaysView.NonWorkingDateCollectionProperty, workingddaycollection);
            BindingOperations.SetBinding(daysview1, ScheduleDaysView.SelectedDatesProperty, daysviewPreSelectedDateBinding);
            BindingOperations.SetBinding(daysview1, ScheduleDaysView.CurrentScheduleTypeProperty, daysviewScheduleTypeBinding);
            BindingOperations.SetBinding(daysview1, ScheduleDaysView.TimeIntervalProperty, daysviewTimeIntervaleBinding);
            BindingOperations.SetBinding(daysview1, ScheduleDaysView.TimeModeProperty, daysviewTimeModeBinding);
            BindingOperations.SetBinding(daysview1, ScheduleDaysView.IntervalHeightProperty, daysviewIntervalHeightBinding);
            BindingOperations.SetBinding(daysview1, ScheduleDaysView.MajorTickVisibilityProperty, daysviewTimelineHourDivisionVisibilityBinding);
            BindingOperations.SetBinding(daysview1, ScheduleDaysView.MinorTickVisibilityProperty, daysviewTimelineVisibilityBinding);
            BindingOperations.SetBinding(daysview1, ScheduleDaysView.VisibleAppointmentsProperty, daysviewPrevVisibleAppointmentsBinding);
            BindingOperations.SetBinding(daysview1, ScheduleDaysView.DayHeaderOrderProperty, DayResourceviewbinding);
            BindingOperations.SetBinding(daysview1, ScheduleDaysView.DayViewColumnCountProperty, dayViewColumnCountBinding);
            BindingOperations.SetBinding(daysview1, ScheduleDaysView.AppointmentSelectionBrushProperty, appointmentselectionbrushbinding);
            BindingOperations.SetBinding(daysview1, ScheduleDaysView.AppointmentTemplateProperty, appointmentTemplateBinding);
            BindingOperations.SetBinding(daysview1, ScheduleDaysView.ScheduleResourceTypeProperty, Scheduletypebinding);
            BindingOperations.SetBinding(daysview1, ScheduleDaysView.DayViewVerticaLineStrokeProperty, dayViewVerticaLineStrokebinding);
            BindingOperations.SetBinding(daysview1, ScheduleDaysView.MajorTickStrokeDashArrayProperty, majorTickStrokeDashArrayBinding);
            BindingOperations.SetBinding(daysview1, ScheduleDaysView.MinorTickStrokeDashArrayProperty, minorTickStrokeDashArrayBinding);
            BindingOperations.SetBinding(daysview1, ScheduleDaysView.ShowAppointmentNavigationButtonsProperty, showNavigationTapBinding);
            BindingOperations.SetBinding(daysview1, ScheduleDaysView.PreviousNavigationButtonTemplateProperty, prevNavigationTapTemplateBinding);
            BindingOperations.SetBinding(daysview1, ScheduleDaysView.NextNavigationButtonTemplateProperty, nextNavigationTapTemplateBinding);
            BindingOperations.SetBinding(daysview1, ScheduleDaysView.ShowNonWorkingHoursProperty, showNonWorkingHoursBinding);
            BindingOperations.SetBinding(daysview1, ScheduleDaysView.WorkStartHourProperty, workStartHourBinding);
            BindingOperations.SetBinding(daysview1, ScheduleDaysView.WorkEndHourProperty, workEndHourBinding);
            BindingOperations.SetBinding(daysview1, ScheduleDaysView.NonAccessibleBlocksProperty, nonAccessibleBlocksBinding);
            #endregion

            #region  Binding daysView2
            BindingOperations.SetBinding(daysview2, ScheduleDaysView.ShowAllDayProperty, showalldaybinding);
            BindingOperations.SetBinding(daysview2, ScheduleDaysView.IsHighLightWorkingHoursProperty, highlighthoursbinding);
            BindingOperations.SetBinding(daysview2, ScheduleDaysView.HeaderBackgroundProperty, dayheaderbackground);
            BindingOperations.SetBinding(daysview2, ScheduleDaysView.MajorTickStrokeProperty, MajorTickStroke);
            BindingOperations.SetBinding(daysview2, ScheduleDaysView.MinorTickStrokeProperty, MinorTickStroke);
            BindingOperations.SetBinding(daysview2, ScheduleDaysView.MajorTickLabelStrokeProperty, majorTickLabelStroke);
            BindingOperations.SetBinding(daysview2, ScheduleDaysView.MinorTickLabelStrokeProperty, minorTickLabelStroke);
            BindingOperations.SetBinding(daysview2, ScheduleDaysView.IsTimeIntervalChangedProperty, timeinterval);
            BindingOperations.SetBinding(daysview2, ScheduleDaysView.NonWorkingHourBrushProperty, nonworkingdaysbackground);
            BindingOperations.SetBinding(daysview2, ScheduleDaysView.CurrentDateBackgroundProperty, dayviewcurrentdate);
            BindingOperations.SetBinding(daysview2, ScheduleDaysView.NonWorkingDateCollectionProperty, workingddaycollection);
            BindingOperations.SetBinding(daysview2, ScheduleDaysView.SelectedDatesProperty, daysviewSelectedDateBinding);
            BindingOperations.SetBinding(daysview2, ScheduleDaysView.CurrentScheduleTypeProperty, daysviewScheduleTypeBinding);
            BindingOperations.SetBinding(daysview2, ScheduleDaysView.TimeIntervalProperty, daysviewTimeIntervaleBinding);
            BindingOperations.SetBinding(daysview2, ScheduleDaysView.TimeModeProperty, daysviewTimeModeBinding);
            BindingOperations.SetBinding(daysview2, ScheduleDaysView.IntervalHeightProperty, daysviewIntervalHeightBinding);
            BindingOperations.SetBinding(daysview2, ScheduleDaysView.MajorTickVisibilityProperty, daysviewTimelineHourDivisionVisibilityBinding);
            BindingOperations.SetBinding(daysview2, ScheduleDaysView.MinorTickVisibilityProperty, daysviewTimelineVisibilityBinding);
            BindingOperations.SetBinding(daysview2, ScheduleDaysView.VisibleAppointmentsProperty, daysviewVisibleAppointmentsBinding);
            BindingOperations.SetBinding(daysview2, ScheduleDaysView.DayHeaderOrderProperty, DayResourceviewbinding);
            BindingOperations.SetBinding(daysview2, ScheduleDaysView.DayViewColumnCountProperty, dayViewColumnCountBinding);
            BindingOperations.SetBinding(daysview2, ScheduleDaysView.AppointmentSelectionBrushProperty, appointmentselectionbrushbinding);
            BindingOperations.SetBinding(daysview2, ScheduleDaysView.AppointmentTemplateProperty, appointmentTemplateBinding);
            BindingOperations.SetBinding(daysview2, ScheduleDaysView.ScheduleResourceTypeProperty, Scheduletypebinding);
            BindingOperations.SetBinding(daysview2, ScheduleDaysView.DayViewVerticaLineStrokeProperty, dayViewVerticaLineStrokebinding);
            BindingOperations.SetBinding(daysview2, ScheduleDaysView.MajorTickStrokeDashArrayProperty, majorTickStrokeDashArrayBinding);
            BindingOperations.SetBinding(daysview2, ScheduleDaysView.MinorTickStrokeDashArrayProperty, minorTickStrokeDashArrayBinding);
            BindingOperations.SetBinding(daysview2, ScheduleDaysView.ShowAppointmentNavigationButtonsProperty, showNavigationTapBinding);
            BindingOperations.SetBinding(daysview2, ScheduleDaysView.PreviousNavigationButtonTemplateProperty, prevNavigationTapTemplateBinding);
            BindingOperations.SetBinding(daysview2, ScheduleDaysView.NextNavigationButtonTemplateProperty, nextNavigationTapTemplateBinding);
            BindingOperations.SetBinding(daysview2, ScheduleDaysView.ShowNonWorkingHoursProperty, showNonWorkingHoursBinding);
            BindingOperations.SetBinding(daysview2, ScheduleDaysView.WorkStartHourProperty, workStartHourBinding);
            BindingOperations.SetBinding(daysview2, ScheduleDaysView.WorkEndHourProperty, workEndHourBinding);
            BindingOperations.SetBinding(daysview2, ScheduleDaysView.NonAccessibleBlocksProperty, nonAccessibleBlocksBinding);
            #endregion

            #region Binding daysView3
            BindingOperations.SetBinding(daysview3, ScheduleDaysView.ShowAllDayProperty, showalldaybinding);
            BindingOperations.SetBinding(daysview3, ScheduleDaysView.IsHighLightWorkingHoursProperty, highlighthoursbinding);
            BindingOperations.SetBinding(daysview3, ScheduleDaysView.HeaderBackgroundProperty, dayheaderbackground);
            BindingOperations.SetBinding(daysview3, ScheduleDaysView.MajorTickStrokeProperty, MajorTickStroke);
            BindingOperations.SetBinding(daysview3, ScheduleDaysView.MinorTickStrokeProperty, MinorTickStroke);
            BindingOperations.SetBinding(daysview3, ScheduleDaysView.MajorTickLabelStrokeProperty, majorTickLabelStroke);
            BindingOperations.SetBinding(daysview3, ScheduleDaysView.MinorTickLabelStrokeProperty, minorTickLabelStroke);
            BindingOperations.SetBinding(daysview3, ScheduleDaysView.IsTimeIntervalChangedProperty, timeinterval);
            BindingOperations.SetBinding(daysview3, ScheduleDaysView.NonWorkingHourBrushProperty, nonworkingdaysbackground);
            BindingOperations.SetBinding(daysview3, ScheduleDaysView.CurrentDateBackgroundProperty, dayviewcurrentdate);
            BindingOperations.SetBinding(daysview3, ScheduleDaysView.NonWorkingDateCollectionProperty, workingddaycollection);
            BindingOperations.SetBinding(daysview3, ScheduleDaysView.SelectedDatesProperty, daysviewNextSelectedDateBinding);
            BindingOperations.SetBinding(daysview3, ScheduleDaysView.CurrentScheduleTypeProperty, daysviewScheduleTypeBinding);
            BindingOperations.SetBinding(daysview3, ScheduleDaysView.TimeIntervalProperty, daysviewTimeIntervaleBinding);
            BindingOperations.SetBinding(daysview3, ScheduleDaysView.TimeModeProperty, daysviewTimeModeBinding);
            BindingOperations.SetBinding(daysview3, ScheduleDaysView.IntervalHeightProperty, daysviewIntervalHeightBinding);
            BindingOperations.SetBinding(daysview3, ScheduleDaysView.MajorTickVisibilityProperty, daysviewTimelineHourDivisionVisibilityBinding);
            BindingOperations.SetBinding(daysview3, ScheduleDaysView.MinorTickVisibilityProperty, daysviewTimelineVisibilityBinding);
            BindingOperations.SetBinding(daysview3, ScheduleDaysView.VisibleAppointmentsProperty, daysviewNextVisibleAppointmentsBinding);
            BindingOperations.SetBinding(daysview3, ScheduleDaysView.DayHeaderOrderProperty, DayResourceviewbinding);
            BindingOperations.SetBinding(daysview3, ScheduleDaysView.DayViewColumnCountProperty, dayViewColumnCountBinding);
            BindingOperations.SetBinding(daysview3, ScheduleDaysView.AppointmentSelectionBrushProperty, appointmentselectionbrushbinding);
            BindingOperations.SetBinding(daysview3, ScheduleDaysView.AppointmentTemplateProperty, appointmentTemplateBinding);
            BindingOperations.SetBinding(daysview3, ScheduleDaysView.ScheduleResourceTypeProperty, Scheduletypebinding);
            BindingOperations.SetBinding(daysview3, ScheduleDaysView.DayViewVerticaLineStrokeProperty, dayViewVerticaLineStrokebinding);
            BindingOperations.SetBinding(daysview3, ScheduleDaysView.MajorTickStrokeDashArrayProperty, majorTickStrokeDashArrayBinding);
            BindingOperations.SetBinding(daysview3, ScheduleDaysView.MinorTickStrokeDashArrayProperty, minorTickStrokeDashArrayBinding);
            BindingOperations.SetBinding(daysview3, ScheduleDaysView.ShowAppointmentNavigationButtonsProperty, showNavigationTapBinding);
            BindingOperations.SetBinding(daysview3, ScheduleDaysView.PreviousNavigationButtonTemplateProperty, prevNavigationTapTemplateBinding);
            BindingOperations.SetBinding(daysview3, ScheduleDaysView.NextNavigationButtonTemplateProperty, nextNavigationTapTemplateBinding);
            BindingOperations.SetBinding(daysview3, ScheduleDaysView.ShowNonWorkingHoursProperty, showNonWorkingHoursBinding);
            BindingOperations.SetBinding(daysview3, ScheduleDaysView.WorkStartHourProperty, workStartHourBinding);
            BindingOperations.SetBinding(daysview3, ScheduleDaysView.WorkEndHourProperty, workEndHourBinding);
            BindingOperations.SetBinding(daysview3, ScheduleDaysView.NonAccessibleBlocksProperty, nonAccessibleBlocksBinding);
            #endregion
#endif
        }

        #endregion

        #region MonthView Binding

        void SetMonthViewBinding()
        {
            var Scheduletypebinding = new Binding { Path = new PropertyPath("ScheduleResourceType"), Source = this };
            var monthViewLineStrokebinding = new Binding { Path = new PropertyPath("MonthViewLineStroke"), Source = this };
            var monthviewMonthSelectedDateBinding = new Binding { Path = new PropertyPath("CurrentSelectedDates"), Source = this };
            var monthviewNextSelectedDateBinding = new Binding { Path = new PropertyPath("NextSelectedDates"), Source = this };
            var monthviewPreSelectedDateBinding = new Binding { Path = new PropertyPath("PrevSelectedDates"), Source = this };   
            var monthviewVisibleAppointmentsBinding = new Binding { Path = new PropertyPath("CurrentVisibleAppointments"), Source = this };
            var monthviewPrevVisibleAppointmentsBinding = new Binding { Path = new PropertyPath("PrevVisibleAppointments"), Source = this };
            var monthviewNextVisibleAppointmentsBinding = new Binding { Path = new PropertyPath("NextVisibleAppointments"), Source = this };
            var monthviewcurrentdate = new Binding { Path = new PropertyPath("CurrentDateBackground"), Source = this };
            var headerbackground = new Binding { Path = new PropertyPath("HeaderBackground"), Source = this };
            var nonfocusedmonth = new Binding { Path = new PropertyPath("NonFocusedMonth"), Source = this };
            var focusedmonth = new Binding { Path = new PropertyPath("FocusedMonth"), Source = this };
            var formatbinding = new Binding { Source = this, Path = new PropertyPath("MonthHeaderDateFormat") };
            var resourceheadervisibility = new Binding { Path = new PropertyPath("ResourceHeaderVisibility"), Source = this };
            var appointmentselectionbrushbinding = new Binding { Path = new PropertyPath("AppointmentSelectionBrush"), Source = this };
            var appointmentTemplateBinding = new Binding { Path = new PropertyPath("AppointmentTemplate"), Source = this };
            var appointmentToolTipTemplateBinding = new Binding { Path = new PropertyPath("AppointmentToolTipTemplate"), Source = this };
            var appointmentTooltipVisibilityBinding = new Binding { Path = new PropertyPath("AppointmentTooltipVisibility"), Source = this };
            var showNavigationTapBinding = new Binding { Source = this, Path = new PropertyPath("ShowAppointmentNavigationButtons") };
            var prevNavigationTapTemplateBinding = new Binding { Source = this, Path = new PropertyPath("PreviousNavigationButtonTemplate") };
            var nextNavigationTapTemplateBinding = new Binding { Source = this, Path = new PropertyPath("NextNavigationButtonTemplate") };

            #region Binding monthview
            BindingOperations.SetBinding(monthview, ScheduleMonthView.MonthHeaderDateFormatProperty, formatbinding);
            BindingOperations.SetBinding(monthview, ScheduleMonthView.MonthViewLineStrokeProperty, monthViewLineStrokebinding);
            BindingOperations.SetBinding(monthview, ScheduleMonthView.SelectedDatesProperty, monthviewMonthSelectedDateBinding);
            BindingOperations.SetBinding(monthview, ScheduleMonthView.VisibleAppointmentsProperty, monthviewVisibleAppointmentsBinding);
            BindingOperations.SetBinding(monthview, ScheduleMonthView.CurrentDateBackgroundProperty, monthviewcurrentdate);
            BindingOperations.SetBinding(monthview, ScheduleMonthView.HeaderBackgroundProperty, headerbackground);
            BindingOperations.SetBinding(monthview, ScheduleMonthView.NonFocusedMonthProperty, nonfocusedmonth);
            BindingOperations.SetBinding(monthview, ScheduleMonthView.FocusedMonthProperty, focusedmonth);
            BindingOperations.SetBinding(monthview, ScheduleMonthView.ResourceHeaderVisibilityProperty, resourceheadervisibility);
            BindingOperations.SetBinding(monthview, ScheduleMonthView.AppointmentSelectionBrushProperty, appointmentselectionbrushbinding);
            BindingOperations.SetBinding(monthview, ScheduleMonthView.AppointmentTemplateProperty, appointmentTemplateBinding);
            BindingOperations.SetBinding(monthview, ScheduleMonthView.AppointmentToolTipTemplateProperty, appointmentToolTipTemplateBinding);
            BindingOperations.SetBinding(monthview, ScheduleMonthView.AppointmentTooltipVisibilityProperty, appointmentTooltipVisibilityBinding);
            BindingOperations.SetBinding(monthview, ScheduleMonthView.ScheduleResourceTypeProperty, Scheduletypebinding);
            BindingOperations.SetBinding(monthview, ScheduleMonthView.ShowAppointmentNavigationButtonsProperty, showNavigationTapBinding);
            BindingOperations.SetBinding(monthview, ScheduleMonthView.PreviousNavigationButtonTemplateProperty, prevNavigationTapTemplateBinding);
            BindingOperations.SetBinding(monthview, ScheduleMonthView.NextNavigationButtonTemplateProperty, nextNavigationTapTemplateBinding);
            #endregion
#if WPF
            #region Binding monthview1
            BindingOperations.SetBinding(monthview1, ScheduleMonthView.MonthHeaderDateFormatProperty, formatbinding);
            BindingOperations.SetBinding(monthview1, ScheduleMonthView.MonthViewLineStrokeProperty, monthViewLineStrokebinding);
            BindingOperations.SetBinding(monthview1, ScheduleMonthView.SelectedDatesProperty, monthviewPreSelectedDateBinding);
            BindingOperations.SetBinding(monthview1, ScheduleMonthView.VisibleAppointmentsProperty, monthviewPrevVisibleAppointmentsBinding);
            BindingOperations.SetBinding(monthview1, ScheduleMonthView.CurrentDateBackgroundProperty, monthviewcurrentdate);
            BindingOperations.SetBinding(monthview1, ScheduleMonthView.HeaderBackgroundProperty, headerbackground);
            BindingOperations.SetBinding(monthview1, ScheduleMonthView.NonFocusedMonthProperty, nonfocusedmonth);
            BindingOperations.SetBinding(monthview1, ScheduleMonthView.FocusedMonthProperty, focusedmonth);
            BindingOperations.SetBinding(monthview1, ScheduleMonthView.ResourceHeaderVisibilityProperty, resourceheadervisibility);
            BindingOperations.SetBinding(monthview1, ScheduleMonthView.AppointmentSelectionBrushProperty, appointmentselectionbrushbinding);
            BindingOperations.SetBinding(monthview1, ScheduleMonthView.AppointmentTemplateProperty, appointmentTemplateBinding);
            BindingOperations.SetBinding(monthview1, ScheduleMonthView.AppointmentToolTipTemplateProperty, appointmentToolTipTemplateBinding);
            BindingOperations.SetBinding(monthview1, ScheduleMonthView.AppointmentTooltipVisibilityProperty, appointmentTooltipVisibilityBinding);
            BindingOperations.SetBinding(monthview1, ScheduleMonthView.ScheduleResourceTypeProperty, Scheduletypebinding);
            BindingOperations.SetBinding(monthview1, ScheduleMonthView.ShowAppointmentNavigationButtonsProperty, showNavigationTapBinding);
            BindingOperations.SetBinding(monthview1, ScheduleMonthView.PreviousNavigationButtonTemplateProperty, prevNavigationTapTemplateBinding);
            BindingOperations.SetBinding(monthview1, ScheduleMonthView.NextNavigationButtonTemplateProperty, nextNavigationTapTemplateBinding);
            #endregion
            #region Binding monthview2
            BindingOperations.SetBinding(monthview2, ScheduleMonthView.MonthHeaderDateFormatProperty, formatbinding);
            BindingOperations.SetBinding(monthview2, ScheduleMonthView.MonthViewLineStrokeProperty, monthViewLineStrokebinding);
            BindingOperations.SetBinding(monthview2, ScheduleMonthView.SelectedDatesProperty, monthviewMonthSelectedDateBinding);
            BindingOperations.SetBinding(monthview2, ScheduleMonthView.VisibleAppointmentsProperty, monthviewVisibleAppointmentsBinding);
            BindingOperations.SetBinding(monthview2, ScheduleMonthView.CurrentDateBackgroundProperty, monthviewcurrentdate);
            BindingOperations.SetBinding(monthview2, ScheduleMonthView.HeaderBackgroundProperty, headerbackground);
            BindingOperations.SetBinding(monthview2, ScheduleMonthView.NonFocusedMonthProperty, nonfocusedmonth);
            BindingOperations.SetBinding(monthview2, ScheduleMonthView.FocusedMonthProperty, focusedmonth);
            BindingOperations.SetBinding(monthview2, ScheduleMonthView.ResourceHeaderVisibilityProperty, resourceheadervisibility);
            BindingOperations.SetBinding(monthview2, ScheduleMonthView.AppointmentSelectionBrushProperty, appointmentselectionbrushbinding);
            BindingOperations.SetBinding(monthview2, ScheduleMonthView.AppointmentTemplateProperty, appointmentTemplateBinding);
            BindingOperations.SetBinding(monthview2, ScheduleMonthView.AppointmentToolTipTemplateProperty, appointmentToolTipTemplateBinding);
            BindingOperations.SetBinding(monthview2, ScheduleMonthView.AppointmentTooltipVisibilityProperty, appointmentTooltipVisibilityBinding);
            BindingOperations.SetBinding(monthview2, ScheduleMonthView.ScheduleResourceTypeProperty, Scheduletypebinding);
            BindingOperations.SetBinding(monthview2, ScheduleMonthView.ShowAppointmentNavigationButtonsProperty, showNavigationTapBinding);
            BindingOperations.SetBinding(monthview2, ScheduleMonthView.PreviousNavigationButtonTemplateProperty, prevNavigationTapTemplateBinding);
            BindingOperations.SetBinding(monthview2, ScheduleMonthView.NextNavigationButtonTemplateProperty, nextNavigationTapTemplateBinding);
            #endregion
            #region Binding monthview3
            BindingOperations.SetBinding(monthview3, ScheduleMonthView.MonthHeaderDateFormatProperty, formatbinding);
            BindingOperations.SetBinding(monthview3, ScheduleMonthView.MonthViewLineStrokeProperty, monthViewLineStrokebinding);
            BindingOperations.SetBinding(monthview3, ScheduleMonthView.SelectedDatesProperty, monthviewNextSelectedDateBinding);
            BindingOperations.SetBinding(monthview3, ScheduleMonthView.VisibleAppointmentsProperty, monthviewNextVisibleAppointmentsBinding);
            BindingOperations.SetBinding(monthview3, ScheduleMonthView.CurrentDateBackgroundProperty, monthviewcurrentdate);
            BindingOperations.SetBinding(monthview3, ScheduleMonthView.HeaderBackgroundProperty, headerbackground);
            BindingOperations.SetBinding(monthview3, ScheduleMonthView.NonFocusedMonthProperty, nonfocusedmonth);
            BindingOperations.SetBinding(monthview3, ScheduleMonthView.FocusedMonthProperty, focusedmonth);
            BindingOperations.SetBinding(monthview3, ScheduleMonthView.ResourceHeaderVisibilityProperty, resourceheadervisibility);
            BindingOperations.SetBinding(monthview3, ScheduleMonthView.AppointmentSelectionBrushProperty, appointmentselectionbrushbinding);
            BindingOperations.SetBinding(monthview3, ScheduleMonthView.AppointmentTemplateProperty, appointmentTemplateBinding);
            BindingOperations.SetBinding(monthview3, ScheduleMonthView.AppointmentToolTipTemplateProperty, appointmentToolTipTemplateBinding);
            BindingOperations.SetBinding(monthview3, ScheduleMonthView.AppointmentTooltipVisibilityProperty, appointmentTooltipVisibilityBinding);
            BindingOperations.SetBinding(monthview3, ScheduleMonthView.ScheduleResourceTypeProperty, Scheduletypebinding);
            BindingOperations.SetBinding(monthview3, ScheduleMonthView.ShowAppointmentNavigationButtonsProperty, showNavigationTapBinding);
            BindingOperations.SetBinding(monthview3, ScheduleMonthView.PreviousNavigationButtonTemplateProperty, prevNavigationTapTemplateBinding);
            BindingOperations.SetBinding(monthview3, ScheduleMonthView.NextNavigationButtonTemplateProperty, nextNavigationTapTemplateBinding);
            #endregion
#endif
        }

        #endregion

        #region TimelineView Binding

        void SetTimelineViewBinding()
        {
            var Scheduletypebinding = new Binding { Path = new PropertyPath("ScheduleResourceType"), Source = this };
            var daysviewSelectedDateBinding = new Binding { Path = new PropertyPath("CurrentSelectedDates"), Source = this };
            var TimeLineNextSelectedDateBinding = new Binding { Path = new PropertyPath("NextSelectedDates"), Source = this };
            var TimeLinePreSelectedDateBinding = new Binding { Path = new PropertyPath("PrevSelectedDates"), Source = this };
   
            var majorTickStrokeDashArrayBinding = new Binding { Path = new PropertyPath("MajorTickStrokeDashArray"), Source = this };
            var minorTickStrokeDashArrayBinding = new Binding { Path = new PropertyPath("MinorTickStrokeDashArray"), Source = this };
            var daysviewScheduleTypeBinding = new Binding { Path = new PropertyPath("ScheduleType"), Source = this };
            var daysviewTimeIntervaleBinding = new Binding { Path = new PropertyPath("TimeInterval"), Source = this };
            var daysviewTimeModeBinding = new Binding { Path = new PropertyPath("TimeMode"), Source = this };
            var daysviewIntervalHeightBinding = new Binding { Path = new PropertyPath("IntervalHeight"), Source = this };
            var daysviewTimelineHourDivisionVisibilityBinding = new Binding { Path = new PropertyPath("MajorTickVisibility"), Source = this };
            var daysviewTimelineVisibilityBinding = new Binding { Path = new PropertyPath("MinorTickVisibility"), Source = this };
            var daysviewVisibleAppointmentsBinding = new Binding { Path = new PropertyPath("CurrentVisibleAppointments"), Source = this };
            var daysviewPrevVisibleAppointmentsBinding = new Binding { Path = new PropertyPath("PrevVisibleAppointments"), Source = this };
            var daysviewNextVisibleAppointmentsBinding = new Binding { Path = new PropertyPath("NextVisibleAppointments"), Source = this };
            var dayviewcurrentdate = new Binding { Path = new PropertyPath("CurrentDateBackground"), Source = this };
            var workingddaycollection = new Binding { Path = new PropertyPath("NonWorkingDateCollection"), Source = this };
            var nonworkingdaysbackground = new Binding { Path = new PropertyPath("NonWorkingHourBrush"), Source = this };
            var timeinterval = new Binding { Path = new PropertyPath("IsTimeIntervalChanged"), Source = this };
            var highlighthoursbinding = new Binding { Path = new PropertyPath("IsHighLightWorkingHours"), Source = this };
            var MinorTickStroke = new Binding { Path = new PropertyPath("MinorTickStroke"), Source = this };
            var dayheaderbackground = new Binding { Path = new PropertyPath("HeaderBackground"), Source = this };
            var MajorTickStroke = new Binding { Path = new PropertyPath("MajorTickStroke"), Source = this };
            var majorTickLabelStroke = new Binding { Path = new PropertyPath("MajorTickLabelStroke"), Source = this };
            var minorTickLabelStroke = new Binding { Path = new PropertyPath("MinorTickLabelStroke"), Source = this };
            var showNavigationTapBinding = new Binding { Source = this, Path = new PropertyPath("ShowAppointmentNavigationButtons") };
            var prevNavigationTapTemplateBinding = new Binding { Source = this, Path = new PropertyPath("PreviousNavigationButtonTemplate") };
            var nextNavigationTapTemplateBinding = new Binding { Source = this, Path = new PropertyPath("NextNavigationButtonTemplate") };
            var appointmentselectionbrushbinding = new Binding { Path = new PropertyPath("AppointmentSelectionBrush"), Source = this };
            var appointmentTemplateBinding = new Binding { Path = new PropertyPath("AppointmentTemplate"), Source = this };
            var appointmentToolTipTemplateBinding = new Binding { Path = new PropertyPath("AppointmentToolTipTemplate"), Source = this };
            var appointmentTooltipVisibilityBinding = new Binding { Path = new PropertyPath("AppointmentTooltipVisibility"), Source = this };
            var showNonWorkingHoursBinding = new Binding { Path = new PropertyPath("ShowNonWorkingHours"), Source = this };
            var workStartHourBinding = new Binding { Path = new PropertyPath("WorkStartHour"), Source = this };
            var workEndHourBinding = new Binding { Path = new PropertyPath("WorkEndHour"), Source = this };
            var nonAccessibleBlocksBinding = new Binding { Path = new PropertyPath("NonAccessibleBlocks"), Source = this };
            var currentTimeIndicatorTemplateBinding = new Binding { Path = new PropertyPath("CurrentTimeIndicatorTemplate"), Source = this };
            var currentTimeIndicatorVisibilityBinding = new Binding { Path = new PropertyPath("CurrentTimeIndicatorVisibility"), Source = this };

            #region Binding timelineView
            BindingOperations.SetBinding(timelineview, ScheduleTimeLineView.IsHighLightWorkingHoursProperty, highlighthoursbinding);
            BindingOperations.SetBinding(timelineview, ScheduleTimeLineView.HeaderBackgroundProperty, dayheaderbackground);
            BindingOperations.SetBinding(timelineview, ScheduleTimeLineView.MajorTickStrokeProperty, MajorTickStroke);
            BindingOperations.SetBinding(timelineview, ScheduleTimeLineView.MinorTickStrokeProperty, MinorTickStroke);
            BindingOperations.SetBinding(timelineview, ScheduleTimeLineView.MajorTickLabelStrokeProperty, majorTickLabelStroke);
            BindingOperations.SetBinding(timelineview, ScheduleTimeLineView.MinorTickLabelStrokeProperty, minorTickLabelStroke);
            BindingOperations.SetBinding(timelineview, ScheduleTimeLineView.IsTimeIntervalChangedProperty, timeinterval);
            BindingOperations.SetBinding(timelineview, ScheduleTimeLineView.NonWorkingHourBrushProperty, nonworkingdaysbackground);
            BindingOperations.SetBinding(timelineview, ScheduleTimeLineView.CurrentDateBackgroundProperty, dayviewcurrentdate);
            BindingOperations.SetBinding(timelineview, ScheduleTimeLineView.NonWorkingDateCollectionProperty, workingddaycollection);
            BindingOperations.SetBinding(timelineview, ScheduleTimeLineView.SelectedDatesProperty, daysviewSelectedDateBinding);
            BindingOperations.SetBinding(timelineview, ScheduleTimeLineView.CurrentScheduleTypeProperty, daysviewScheduleTypeBinding);
            BindingOperations.SetBinding(timelineview, ScheduleTimeLineView.TimeIntervalProperty, daysviewTimeIntervaleBinding);
            BindingOperations.SetBinding(timelineview, ScheduleTimeLineView.TimeModeProperty, daysviewTimeModeBinding);
            BindingOperations.SetBinding(timelineview, ScheduleTimeLineView.IntervalHeightProperty, daysviewIntervalHeightBinding);
            BindingOperations.SetBinding(timelineview, ScheduleTimeLineView.MajorTickVisibilityProperty, daysviewTimelineHourDivisionVisibilityBinding);
            BindingOperations.SetBinding(timelineview, ScheduleTimeLineView.MinorTickVisibilityProperty, daysviewTimelineVisibilityBinding);
            BindingOperations.SetBinding(timelineview, ScheduleTimeLineView.VisibleAppointmentsProperty, daysviewVisibleAppointmentsBinding);
            BindingOperations.SetBinding(timelineview, ScheduleTimeLineView.ScheduleResourceTypeProperty, Scheduletypebinding);
            BindingOperations.SetBinding(timelineview, ScheduleTimeLineView.MajorTickStrokeDashArrayProperty, majorTickStrokeDashArrayBinding);
            BindingOperations.SetBinding(timelineview, ScheduleTimeLineView.MinorTickStrokeDashArrayProperty, minorTickStrokeDashArrayBinding);
            BindingOperations.SetBinding(timelineview, ScheduleTimeLineView.ShowAppointmentNavigationButtonsProperty, showNavigationTapBinding);
            BindingOperations.SetBinding(timelineview, ScheduleTimeLineView.PreviousNavigationButtonTemplateProperty, prevNavigationTapTemplateBinding);
            BindingOperations.SetBinding(timelineview, ScheduleTimeLineView.NextNavigationButtonTemplateProperty, nextNavigationTapTemplateBinding);
            BindingOperations.SetBinding(timelineview, ScheduleTimeLineView.AppointmentSelectionBrushProperty, appointmentselectionbrushbinding);
            BindingOperations.SetBinding(timelineview, ScheduleTimeLineView.AppointmentTemplateProperty, appointmentTemplateBinding);
            BindingOperations.SetBinding(timelineview, ScheduleTimeLineView.AppointmentToolTipTemplateProperty, appointmentToolTipTemplateBinding);
            BindingOperations.SetBinding(timelineview, ScheduleTimeLineView.AppointmentTooltipVisibilityProperty, appointmentTooltipVisibilityBinding);
            BindingOperations.SetBinding(timelineview, ScheduleTimeLineView.ShowNonWorkingHoursProperty, showNonWorkingHoursBinding);
            BindingOperations.SetBinding(timelineview, ScheduleTimeLineView.WorkStartHourProperty, workStartHourBinding);
            BindingOperations.SetBinding(timelineview, ScheduleTimeLineView.WorkEndHourProperty, workEndHourBinding);
            BindingOperations.SetBinding(timelineview, ScheduleTimeLineView.NonAccessibleBlocksProperty, nonAccessibleBlocksBinding);
            BindingOperations.SetBinding(timelineview, ScheduleTimeLineView.CurrentTimeIndicatorTemplateProperty, currentTimeIndicatorTemplateBinding);
            BindingOperations.SetBinding(timelineview, ScheduleTimeLineView.CurrentTimeIndicatorVisibilityProperty, currentTimeIndicatorVisibilityBinding);
            #endregion
#if WPF
            #region Binding timelineView1
            BindingOperations.SetBinding(timelineview1, ScheduleTimeLineView.IsHighLightWorkingHoursProperty, highlighthoursbinding);
            BindingOperations.SetBinding(timelineview1, ScheduleTimeLineView.HeaderBackgroundProperty, dayheaderbackground);
            BindingOperations.SetBinding(timelineview1, ScheduleTimeLineView.MajorTickStrokeProperty, MajorTickStroke);
            BindingOperations.SetBinding(timelineview1, ScheduleTimeLineView.MinorTickStrokeProperty, MinorTickStroke);
            BindingOperations.SetBinding(timelineview1, ScheduleTimeLineView.MajorTickLabelStrokeProperty, majorTickLabelStroke);
            BindingOperations.SetBinding(timelineview1, ScheduleTimeLineView.MinorTickLabelStrokeProperty, minorTickLabelStroke);
            BindingOperations.SetBinding(timelineview1, ScheduleTimeLineView.IsTimeIntervalChangedProperty, timeinterval);
            BindingOperations.SetBinding(timelineview1, ScheduleTimeLineView.NonWorkingHourBrushProperty, nonworkingdaysbackground);
            BindingOperations.SetBinding(timelineview1, ScheduleTimeLineView.CurrentDateBackgroundProperty, dayviewcurrentdate);
            BindingOperations.SetBinding(timelineview1, ScheduleTimeLineView.NonWorkingDateCollectionProperty, workingddaycollection);
            BindingOperations.SetBinding(timelineview1, ScheduleTimeLineView.SelectedDatesProperty, TimeLinePreSelectedDateBinding);
            BindingOperations.SetBinding(timelineview1, ScheduleTimeLineView.CurrentScheduleTypeProperty, daysviewScheduleTypeBinding);
            BindingOperations.SetBinding(timelineview1, ScheduleTimeLineView.TimeIntervalProperty, daysviewTimeIntervaleBinding);
            BindingOperations.SetBinding(timelineview1, ScheduleTimeLineView.TimeModeProperty, daysviewTimeModeBinding);
            BindingOperations.SetBinding(timelineview1, ScheduleTimeLineView.IntervalHeightProperty, daysviewIntervalHeightBinding);
            BindingOperations.SetBinding(timelineview1, ScheduleTimeLineView.MajorTickVisibilityProperty, daysviewTimelineHourDivisionVisibilityBinding);
            BindingOperations.SetBinding(timelineview1, ScheduleTimeLineView.MinorTickVisibilityProperty, daysviewTimelineVisibilityBinding);
            BindingOperations.SetBinding(timelineview1, ScheduleTimeLineView.VisibleAppointmentsProperty, daysviewPrevVisibleAppointmentsBinding);
            BindingOperations.SetBinding(timelineview1, ScheduleTimeLineView.ScheduleResourceTypeProperty, Scheduletypebinding);
            BindingOperations.SetBinding(timelineview1, ScheduleTimeLineView.MajorTickStrokeDashArrayProperty, majorTickStrokeDashArrayBinding);
            BindingOperations.SetBinding(timelineview1, ScheduleTimeLineView.MinorTickStrokeDashArrayProperty, minorTickStrokeDashArrayBinding);
            BindingOperations.SetBinding(timelineview1, ScheduleTimeLineView.ShowAppointmentNavigationButtonsProperty, showNavigationTapBinding);
            BindingOperations.SetBinding(timelineview1, ScheduleTimeLineView.PreviousNavigationButtonTemplateProperty, prevNavigationTapTemplateBinding);
            BindingOperations.SetBinding(timelineview1, ScheduleTimeLineView.NextNavigationButtonTemplateProperty, nextNavigationTapTemplateBinding);
            BindingOperations.SetBinding(timelineview1, ScheduleTimeLineView.AppointmentSelectionBrushProperty, appointmentselectionbrushbinding);
            BindingOperations.SetBinding(timelineview1, ScheduleTimeLineView.AppointmentTemplateProperty, appointmentTemplateBinding);
            BindingOperations.SetBinding(timelineview1, ScheduleTimeLineView.AppointmentToolTipTemplateProperty, appointmentToolTipTemplateBinding);
            BindingOperations.SetBinding(timelineview1, ScheduleTimeLineView.AppointmentTooltipVisibilityProperty, appointmentTooltipVisibilityBinding);
            BindingOperations.SetBinding(timelineview1, ScheduleTimeLineView.ShowNonWorkingHoursProperty, showNonWorkingHoursBinding);
            BindingOperations.SetBinding(timelineview1, ScheduleTimeLineView.WorkStartHourProperty, workStartHourBinding);
            BindingOperations.SetBinding(timelineview1, ScheduleTimeLineView.WorkEndHourProperty, workEndHourBinding);
            BindingOperations.SetBinding(timelineview1, ScheduleTimeLineView.NonAccessibleBlocksProperty, nonAccessibleBlocksBinding);
            #endregion

            #region Binding timelineView2
            BindingOperations.SetBinding(timelineview2, ScheduleTimeLineView.IsHighLightWorkingHoursProperty, highlighthoursbinding);
            BindingOperations.SetBinding(timelineview2, ScheduleTimeLineView.HeaderBackgroundProperty, dayheaderbackground);
            BindingOperations.SetBinding(timelineview2, ScheduleTimeLineView.MajorTickStrokeProperty, MajorTickStroke);
            BindingOperations.SetBinding(timelineview2, ScheduleTimeLineView.MinorTickStrokeProperty, MinorTickStroke);
            BindingOperations.SetBinding(timelineview2, ScheduleTimeLineView.MajorTickLabelStrokeProperty, majorTickLabelStroke);
            BindingOperations.SetBinding(timelineview2, ScheduleTimeLineView.MinorTickLabelStrokeProperty, minorTickLabelStroke);
            BindingOperations.SetBinding(timelineview2, ScheduleTimeLineView.IsTimeIntervalChangedProperty, timeinterval);
            BindingOperations.SetBinding(timelineview2, ScheduleTimeLineView.NonWorkingHourBrushProperty, nonworkingdaysbackground);
            BindingOperations.SetBinding(timelineview2, ScheduleTimeLineView.CurrentDateBackgroundProperty, dayviewcurrentdate);
            BindingOperations.SetBinding(timelineview2, ScheduleTimeLineView.NonWorkingDateCollectionProperty, workingddaycollection);
            BindingOperations.SetBinding(timelineview2, ScheduleTimeLineView.SelectedDatesProperty, daysviewSelectedDateBinding);
            BindingOperations.SetBinding(timelineview2, ScheduleTimeLineView.CurrentScheduleTypeProperty, daysviewScheduleTypeBinding);
            BindingOperations.SetBinding(timelineview2, ScheduleTimeLineView.TimeIntervalProperty, daysviewTimeIntervaleBinding);
            BindingOperations.SetBinding(timelineview2, ScheduleTimeLineView.TimeModeProperty, daysviewTimeModeBinding);
            BindingOperations.SetBinding(timelineview2, ScheduleTimeLineView.IntervalHeightProperty, daysviewIntervalHeightBinding);
            BindingOperations.SetBinding(timelineview2, ScheduleTimeLineView.MajorTickVisibilityProperty, daysviewTimelineHourDivisionVisibilityBinding);
            BindingOperations.SetBinding(timelineview2, ScheduleTimeLineView.MinorTickVisibilityProperty, daysviewTimelineVisibilityBinding);
            BindingOperations.SetBinding(timelineview2, ScheduleTimeLineView.VisibleAppointmentsProperty, daysviewVisibleAppointmentsBinding);
            BindingOperations.SetBinding(timelineview2, ScheduleTimeLineView.ScheduleResourceTypeProperty, Scheduletypebinding);
            BindingOperations.SetBinding(timelineview2, ScheduleTimeLineView.MajorTickStrokeDashArrayProperty, majorTickStrokeDashArrayBinding);
            BindingOperations.SetBinding(timelineview2, ScheduleTimeLineView.MinorTickStrokeDashArrayProperty, minorTickStrokeDashArrayBinding);
            BindingOperations.SetBinding(timelineview2, ScheduleTimeLineView.ShowAppointmentNavigationButtonsProperty, showNavigationTapBinding);
            BindingOperations.SetBinding(timelineview2, ScheduleTimeLineView.PreviousNavigationButtonTemplateProperty, prevNavigationTapTemplateBinding);
            BindingOperations.SetBinding(timelineview2, ScheduleTimeLineView.NextNavigationButtonTemplateProperty, nextNavigationTapTemplateBinding);
            BindingOperations.SetBinding(timelineview2, ScheduleTimeLineView.AppointmentSelectionBrushProperty, appointmentselectionbrushbinding);
            BindingOperations.SetBinding(timelineview2, ScheduleTimeLineView.AppointmentTemplateProperty, appointmentTemplateBinding);
            BindingOperations.SetBinding(timelineview2, ScheduleTimeLineView.AppointmentToolTipTemplateProperty, appointmentToolTipTemplateBinding);
            BindingOperations.SetBinding(timelineview2, ScheduleTimeLineView.AppointmentTooltipVisibilityProperty, appointmentTooltipVisibilityBinding);
            BindingOperations.SetBinding(timelineview2, ScheduleTimeLineView.ShowNonWorkingHoursProperty, showNonWorkingHoursBinding);
            BindingOperations.SetBinding(timelineview2, ScheduleTimeLineView.WorkStartHourProperty, workStartHourBinding);
            BindingOperations.SetBinding(timelineview2, ScheduleTimeLineView.WorkEndHourProperty, workEndHourBinding);
            BindingOperations.SetBinding(timelineview2, ScheduleTimeLineView.NonAccessibleBlocksProperty, nonAccessibleBlocksBinding);
            #endregion

            #region Binding timelineView3
            BindingOperations.SetBinding(timelineview3, ScheduleTimeLineView.IsHighLightWorkingHoursProperty, highlighthoursbinding);
            BindingOperations.SetBinding(timelineview3, ScheduleTimeLineView.HeaderBackgroundProperty, dayheaderbackground);
            BindingOperations.SetBinding(timelineview3, ScheduleTimeLineView.MajorTickStrokeProperty, MajorTickStroke);
            BindingOperations.SetBinding(timelineview3, ScheduleTimeLineView.MinorTickStrokeProperty, MinorTickStroke);
            BindingOperations.SetBinding(timelineview3, ScheduleTimeLineView.MajorTickLabelStrokeProperty, majorTickLabelStroke);
            BindingOperations.SetBinding(timelineview3, ScheduleTimeLineView.MinorTickLabelStrokeProperty, minorTickLabelStroke);
            BindingOperations.SetBinding(timelineview3, ScheduleTimeLineView.IsTimeIntervalChangedProperty, timeinterval);
            BindingOperations.SetBinding(timelineview3, ScheduleTimeLineView.NonWorkingHourBrushProperty, nonworkingdaysbackground);
            BindingOperations.SetBinding(timelineview3, ScheduleTimeLineView.CurrentDateBackgroundProperty, dayviewcurrentdate);
            BindingOperations.SetBinding(timelineview3, ScheduleTimeLineView.NonWorkingDateCollectionProperty, workingddaycollection);
            BindingOperations.SetBinding(timelineview3, ScheduleTimeLineView.SelectedDatesProperty,TimeLineNextSelectedDateBinding);
            BindingOperations.SetBinding(timelineview3, ScheduleTimeLineView.CurrentScheduleTypeProperty, daysviewScheduleTypeBinding);
            BindingOperations.SetBinding(timelineview3, ScheduleTimeLineView.TimeIntervalProperty, daysviewTimeIntervaleBinding);
            BindingOperations.SetBinding(timelineview3, ScheduleTimeLineView.TimeModeProperty, daysviewTimeModeBinding);
            BindingOperations.SetBinding(timelineview3, ScheduleTimeLineView.IntervalHeightProperty, daysviewIntervalHeightBinding);
            BindingOperations.SetBinding(timelineview3, ScheduleTimeLineView.MajorTickVisibilityProperty, daysviewTimelineHourDivisionVisibilityBinding);
            BindingOperations.SetBinding(timelineview3, ScheduleTimeLineView.MinorTickVisibilityProperty, daysviewTimelineVisibilityBinding);
            BindingOperations.SetBinding(timelineview3, ScheduleTimeLineView.VisibleAppointmentsProperty, daysviewNextVisibleAppointmentsBinding);
            BindingOperations.SetBinding(timelineview3, ScheduleTimeLineView.ScheduleResourceTypeProperty, Scheduletypebinding);
            BindingOperations.SetBinding(timelineview3, ScheduleTimeLineView.MajorTickStrokeDashArrayProperty, majorTickStrokeDashArrayBinding);
            BindingOperations.SetBinding(timelineview3, ScheduleTimeLineView.MinorTickStrokeDashArrayProperty, minorTickStrokeDashArrayBinding);
            BindingOperations.SetBinding(timelineview3, ScheduleTimeLineView.ShowAppointmentNavigationButtonsProperty, showNavigationTapBinding);
            BindingOperations.SetBinding(timelineview3, ScheduleTimeLineView.PreviousNavigationButtonTemplateProperty, prevNavigationTapTemplateBinding);
            BindingOperations.SetBinding(timelineview3, ScheduleTimeLineView.NextNavigationButtonTemplateProperty, nextNavigationTapTemplateBinding);
            BindingOperations.SetBinding(timelineview3, ScheduleTimeLineView.AppointmentSelectionBrushProperty, appointmentselectionbrushbinding);
            BindingOperations.SetBinding(timelineview3, ScheduleTimeLineView.AppointmentTemplateProperty, appointmentTemplateBinding);
            BindingOperations.SetBinding(timelineview3, ScheduleTimeLineView.AppointmentToolTipTemplateProperty, appointmentToolTipTemplateBinding);
            BindingOperations.SetBinding(timelineview3, ScheduleTimeLineView.AppointmentTooltipVisibilityProperty, appointmentTooltipVisibilityBinding);
            BindingOperations.SetBinding(timelineview3, ScheduleTimeLineView.ShowNonWorkingHoursProperty, showNonWorkingHoursBinding);
            BindingOperations.SetBinding(timelineview3, ScheduleTimeLineView.WorkStartHourProperty, workStartHourBinding);
            BindingOperations.SetBinding(timelineview3, ScheduleTimeLineView.WorkEndHourProperty, workEndHourBinding);
            BindingOperations.SetBinding(timelineview3, ScheduleTimeLineView.NonAccessibleBlocksProperty, nonAccessibleBlocksBinding);
            #endregion
#endif
        }

        #endregion

        #region All TimeZones

        TimeZoneCollection GetTimeZones()
        {
            var timezonecoll = new TimeZoneCollection
                {
                    new TimeZone {TimeZoneValue = "(UTC) Casablanca"},
                    new TimeZone {TimeZoneValue = "(UTC) Coordinated Universal Time"},
                    new TimeZone {TimeZoneValue = "(UTC) Dublin, Edinburgh, Lisbon, London"},
                    new TimeZone {TimeZoneValue = "(UTC) Monrovia, Reykjavik"},
                    new TimeZone {TimeZoneValue = "(UTC+01:00) Amsterdam, Berlin, Bern, Rome, Stockholm, Vienna"},
                    new TimeZone {TimeZoneValue = "(UTC+01:00) Belgrade, Bratislava, Budapest, Ljubljana, Prague"},
                    new TimeZone {TimeZoneValue = "(UTC+01:00) Brussels, Copenhagen, Madrid, Paris"},
                    new TimeZone {TimeZoneValue = "(UTC+01:00) Sarajevo, Skopje, Warsaw, Zagreb"},
                    new TimeZone {TimeZoneValue = "(UTC+01:00) West Central Africa"},
                    new TimeZone {TimeZoneValue = "(UTC+02:00) Amman"},
                    new TimeZone {TimeZoneValue = "(UTC+02:00) Athens, Bucharest, Istanbul"},
                    new TimeZone {TimeZoneValue = "(UTC+02:00) Beirut"},
                    new TimeZone {TimeZoneValue = "(UTC+02:00) Cairo"},
                    new TimeZone {TimeZoneValue = "(UTC+02:00) Harare, Pretoria"},
                    new TimeZone {TimeZoneValue = "(UTC+02:00) Helsinki, Kyiv, Riga, Sofia, Tallinn, Vilnius"},
                    new TimeZone {TimeZoneValue = "(UTC+02:00) Jerusalem"},
                    new TimeZone {TimeZoneValue = "(UTC+02:00) Minsk"},
                    new TimeZone {TimeZoneValue = "(UTC+02:00) Windhoek"},
                    new TimeZone {TimeZoneValue = "(UTC+03:00) Baghdad"},
                    new TimeZone {TimeZoneValue = "(UTC+03:00) Kuwait, Riyadh"},
                    new TimeZone {TimeZoneValue = "(UTC+03:00) Moscow, St. Petersburg, Volgograd"},
                    new TimeZone {TimeZoneValue = "(UTC+03:00) Nairobi"},
                    new TimeZone {TimeZoneValue = "(UTC+03:30) Tehran"},
                    new TimeZone {TimeZoneValue = "(UTC+04:00) Abu Dhabi, Muscat"},
                    new TimeZone {TimeZoneValue = "(UTC+04:00) Baku"},
                    new TimeZone {TimeZoneValue = "(UTC+04:00) Port Louis"},
                    new TimeZone {TimeZoneValue = "(UTC+04:00) Tbilisi"},
                    new TimeZone {TimeZoneValue = "(UTC+04:00) Yerevan"},
                    new TimeZone {TimeZoneValue = "(UTC+04:30) Kabul"},
                    new TimeZone {TimeZoneValue = "(UTC+05:00) Ekaterinburg"},
                    new TimeZone {TimeZoneValue = "(UTC+05:00) Islamabad, Karachi"},
                    new TimeZone {TimeZoneValue = "(UTC+05:00) Tashkent"},
                    new TimeZone {TimeZoneValue = "(UTC+05:30) Chennai, Kolkata, Mumbai, New Delhi"},
                    new TimeZone {TimeZoneValue = "(UTC+05:30) Sri Jayawardenepura"},
                    new TimeZone {TimeZoneValue = "(UTC+05:45) Kathmandu"},
                    new TimeZone {TimeZoneValue = "(UTC+06:00) Astana, Dhaka"},
                    new TimeZone {TimeZoneValue = "(UTC+06:00) Novosibirsk"},
                    new TimeZone {TimeZoneValue = "(UTC+06:30) Yangon (Rangoon)"},
                    new TimeZone {TimeZoneValue = "(UTC+07:00) Bangkok, Hanoi, Jakarta"},
                    new TimeZone {TimeZoneValue = "(UTC+07:00) Krasnoyarsk"},
                    new TimeZone {TimeZoneValue = "(UTC+08:00) Beijing, Chongqing, Hong Kong, Urumqi"},
                    new TimeZone {TimeZoneValue = "(UTC+08:00) Irkutsk"},
                    new TimeZone {TimeZoneValue = "(UTC+08:00) Kuala Lumpur, Singapore"},
                    new TimeZone {TimeZoneValue = "(UTC+08:00) Perth"},
                    new TimeZone {TimeZoneValue = "(UTC+08:00) Taipei"},
                    new TimeZone {TimeZoneValue = "(UTC+08:00) Ulaanbaatar"},
                    new TimeZone {TimeZoneValue = "(UTC+09:00) Osaka, Sapporo, Tokyo"},
                    new TimeZone {TimeZoneValue = "(UTC+09:00) Seoul"},
                    new TimeZone {TimeZoneValue = "(UTC+09:00) Yakutsk"},
                    new TimeZone {TimeZoneValue = "(UTC+09:30) Adelaide"},
                    new TimeZone {TimeZoneValue = "(UTC+09:30) Darwin"},
                    new TimeZone {TimeZoneValue = "(UTC+10:00) Brisbane"},
                    new TimeZone {TimeZoneValue = "(UTC+10:00) Canberra, Melbourne, Sydney"},
                    new TimeZone {TimeZoneValue = "(UTC+10:00) Guam, Port Moresby"},
                    new TimeZone {TimeZoneValue = "(UTC+10:00) Hobart"},
                    new TimeZone {TimeZoneValue = "(UTC+10:00) Vladivostok"},
                    new TimeZone {TimeZoneValue = "(UTC+11:00) Magadan, Solomon Is., New Caledonia"},
                    new TimeZone {TimeZoneValue = "(UTC+12:00) Auckland, Wellington"},
                    new TimeZone {TimeZoneValue = "(UTC+12:00) Fiji, Marshall Is."},
                    new TimeZone {TimeZoneValue = "(UTC+12:00) Petropavlovsk-Kamchatsky"},
                    new TimeZone {TimeZoneValue = "(UTC+13:00) Nuku'alofa"},
                    new TimeZone {TimeZoneValue = "(UTC-01:00) Azores"},
                    new TimeZone {TimeZoneValue = "(UTC-01:00) Cape Verde Is."},
                    new TimeZone {TimeZoneValue = "(UTC-02:00) Mid-Atlantic"},
                    new TimeZone {TimeZoneValue = "(UTC-03:00) Brasilia"},
                    new TimeZone {TimeZoneValue = "(UTC-03:00) Buenos Aires"},
                    new TimeZone {TimeZoneValue = "(UTC-03:00) Cayenne"},
                    new TimeZone {TimeZoneValue = "(UTC-03:00) Greenland"},
                    new TimeZone {TimeZoneValue = "(UTC-03:00) Montevideo"},
                    new TimeZone {TimeZoneValue = "(UTC-03:30) Newfoundland"},
                    new TimeZone {TimeZoneValue = "(UTC-04:00) Asuncion"},
                    new TimeZone {TimeZoneValue = "(UTC-04:00) Atlantic Time (Canada)"},
                    new TimeZone {TimeZoneValue = "(UTC-04:00) Georgetown, La Paz, San Juan"},
                    new TimeZone {TimeZoneValue = "(UTC-04:00) Manaus"},
                    new TimeZone {TimeZoneValue = "(UTC-04:00) Santiago"},
                    new TimeZone {TimeZoneValue = "(UTC-04:30) Caracas"},
                    new TimeZone {TimeZoneValue = "(UTC-05:00) Bogota, Lima, Quito"},
                    new TimeZone {TimeZoneValue = "(UTC-05:00) Eastern Time (US & Canada)"},
                    new TimeZone {TimeZoneValue = "(UTC-05:00) Indiana (East)"},
                    new TimeZone {TimeZoneValue = "(UTC-06:00) Central America"},
                    new TimeZone {TimeZoneValue = "(UTC-06:00) Central Time (US & Canada)"},
                    new TimeZone {TimeZoneValue = "(UTC-06:00) Guadalajara, Mexico City, Monterrey"},
                    new TimeZone {TimeZoneValue = "(UTC-06:00) Saskatchewan"},
                    new TimeZone {TimeZoneValue = "(UTC-07:00) Arizona"},
                    new TimeZone {TimeZoneValue = "(UTC-07:00) Chihuahua, La Paz, Mazatlan"},
                    new TimeZone {TimeZoneValue = "(UTC-07:00) Mountain Time (US & Canada)"},
                    new TimeZone {TimeZoneValue = "(UTC-08:00) Pacific Time (US & Canada)"},
                    new TimeZone {TimeZoneValue = "(UTC-08:00) Tijuana, Baja California"},
                    new TimeZone {TimeZoneValue = "(UTC-09:00) Alaska"},
                    new TimeZone {TimeZoneValue = "(UTC-10:00) Hawaii"},
                    new TimeZone {TimeZoneValue = "(UTC-11:00) Midway Island, Samoa"},
                    new TimeZone {TimeZoneValue = "(UTC-12:00) International Date Line West"}
                };
            return timezonecoll;

        }

        #endregion

        #region Reminder

        void EditToast(ScheduleAppointment newapp)
        {
            if (newapp.ReminderTime != ReminderTimeType.None)
            {
                newapp.ReminderDeliveryTime = newapp.InternalStartTime - newapp.MeasureTime(newapp.ReminderTime);
                GenerateNotification();
            }
        }

        void TriggerReminder()
        {
            GenerateNotification();
        }

        void GenerateNotification()
        {
            if (EnableReminderTimer)
            {
                DateTime? date = GetNearestReminder();
                if (date != null)
                {
                    TimeSpan timespan;
                    if (date <= DateTime.Now)
                    {
                        timespan = new TimeSpan(0, 0, 2);
                        reminderTimer.Interval = timespan;
                        reminderTimer.Start();
                    }
                    else
                    {
                        timespan = (DateTime)date - DateTime.Now;
                        reminderTimer.Interval = timespan;
                        reminderTimer.Start();

                    }
                }
            }
            else
            {
                if (reminderTimer != null)
                    reminderTimer.Stop();
            }
        }

        DateTime? GetNearestReminder()
        {
            DateTime? nearest = null;
            foreach (ObservableCollection<ScheduleAppointment> coll in ProxyAppointments.Values)
            {
                foreach (ScheduleAppointment app in coll)
                {
                    if (app.ReminderDeliveryTime != null && !app.IsSet)
                    {
                        if (nearest == null)
                            nearest = app.ReminderDeliveryTime;
                        else if (app.ReminderDeliveryTime <= nearest)
                            nearest = app.ReminderDeliveryTime;
                    }
                }
            }

            return nearest;
        }

        #endregion

        #region Converting String to Day

        ObservableCollection<DayOfWeek> StringToDaysOfWeekConverter(string workingDaysString, bool returnWorkingDays)
        {
            var weekDays = new[] { DayOfWeek.Sunday, DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday };
            var weekDaysInstr = "Sunday,Monday,Tuesday,Wednesday,Thursday,Friday,Saturday";
            string[] inputstring = workingDaysString.Split(',');
            string[] days = returnWorkingDays ? weekDaysInstr.Split(',').Except(workingDaysString.Split(',').ToList<string>()).ToArray() : workingDaysString.Split(',');     
            var titleCaseDays = new string[days.Count()];
            int i = 0;
            foreach (string daysName in days)
            {
                if (!daysName.Equals(""))
                {
                    string daysNames = daysName.ToLower().Substring(0, 1).ToUpper() + daysName.ToLower().Substring(1);
                    if (daysNames.Equals("Sunday") || daysNames.Equals("Monday") || daysNames.Equals("Tuesday") || daysNames.Equals("Wednesday") || daysNames.Equals("Thursday") || daysNames.Equals("Friday") || daysNames.Equals("Saturday") || daysNames.Equals("Sunday"))
                    {
                        titleCaseDays[i] = daysName.ToLower().Substring(0, 1).ToUpper() + daysName.ToLower().Substring(1);
                        var DayofWeek = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), titleCaseDays[i], false);
                        {
                            titleCaseDays[i] = CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(DayofWeek);
                        }
                        i = i + 1;
                    }
                }
            }

            var dayslist = (from d in titleCaseDays where CultureInfo.CurrentCulture.DateTimeFormat.DayNames.Contains(d) select weekDays[CultureInfo.CurrentCulture.DateTimeFormat.DayNames.ToList().IndexOf(d)]).ToList();
            dayslist.Sort();
            var collection = new ObservableCollection<DayOfWeek>();
            foreach (DayOfWeek d in dayslist)
            {
                collection.Add(d);
            }
            var coll = new ObservableCollection<DayOfWeek>(collection.Distinct());
            return coll;
        }

        #endregion

        #region Updating ScheduleType

        void UpdateScheduleType()
        {

            if (mainViewItem == null ) return;
            mainViewItem.Content = null;
#if WPF
            mainViewItem1.Content = null;
            mainViewItem2.Content = null;
#endif
            switch (ScheduleType)
            {
                case ScheduleType.Day:
                    {
                        monthview = null;
                        timelineview = null;
                        MoveToDayType();
                        daysview = new ScheduleDaysView();
#if WPF
                        monthview1 = null;
                        monthview2 = null;
                        monthview3 = null;
                        
                        timelineview1 = null;
                        timelineview2 = null;
                        timelineview3 = null;
                        
                        daysview1 = new ScheduleDaysView();
                        daysview2 = new ScheduleDaysView();
                        daysview3 = new ScheduleDaysView();
#endif
                        SetDayViewBinding();
                        break;
                    }
                case ScheduleType.Week:
                    {
                        monthview = null;
                        timelineview = null;
                        MoveToWeekType();
                        daysview = new ScheduleDaysView();
#if WPF
                        monthview1 = null;
                        monthview2 = null;
                        monthview3 = null;
                        
                        timelineview1 = null;
                        timelineview2 = null;
                        timelineview3 = null;
                       
                        daysview1 = new ScheduleDaysView();
                        daysview2 = new ScheduleDaysView();
                        daysview3 = new ScheduleDaysView();
#endif
                        SetDayViewBinding();
                        break;
                    }
                case ScheduleType.WorkWeek:
                    {
                        monthview = null;
                        timelineview = null;
                        MoveToWeekType();
                        daysview = new ScheduleDaysView();
#if WPF
                        monthview1 = null;
                        monthview2 = null;
                        monthview3 = null;
                        
                        timelineview1 = null;
                        timelineview2 = null;
                        timelineview3 = null;
                       
                        daysview1 = new ScheduleDaysView();
                        daysview2 = new ScheduleDaysView();
                        daysview3 = new ScheduleDaysView();
#endif
                        SetDayViewBinding();
                        break;
                    }
                case ScheduleType.Month:
                    {
                        daysview = null;
                        timelineview = null;
                        MoveToMonthType();
                        monthview = new ScheduleMonthView();
#if WPF
                        daysview1 = null;
                        daysview2= null;
                        daysview3 = null;
                        
                        timelineview1 = null;
                        timelineview2 = null;
                        timelineview3 = null;
                       
                        monthview1 = new ScheduleMonthView();
                        monthview2 = new ScheduleMonthView();
                        monthview3 = new ScheduleMonthView();
#endif
                        SetMonthViewBinding();
                        break;
                    }
                case ScheduleType.TimeLine:
                    {
                        daysview = null;
                        monthview = null;
                        MoveToTimeLineType();
                        timelineview = new ScheduleTimeLineView();
#if WPF
                        daysview1 = null;
                        daysview2 = null;
                        daysview3 = null;
                        
                        monthview1 = null;
                        monthview2 = null;
                        monthview3 = null;
                        
                        timelineview1 = new ScheduleTimeLineView();
                        timelineview2 = new ScheduleTimeLineView();
                        timelineview3 = new ScheduleTimeLineView();
#endif
                        SetTimelineViewBinding();
                        break;
                    }
            }
            
#if WPF
            if (isTemplateApplied)
            {
                UpdateMainViewItems();
                if (viewcontrol != null)
                {
                    switch (ScheduleType)
                    {
                        case ScheduleType.Day:
                        case ScheduleType.Week:
                            if (daysview2 != null && currentAllDaySelectedItem == null)
                                daysview2.UpdateSelection(false);
                            break;
                        case ScheduleType.TimeLine:
                            if (timelineview2 != null)
                                timelineview2.UpdateSelection(false);
                            break;
                    }
                }
            }
#else
            if (isTemplateApplied)
            {
                UpdateMainViewItems();
                if (currentSelectedItem != null && CurrentSelectedDates.Contains(SelectedDate))
                {
                    if (currentSelectedItem.Content is ScheduleDaysView)
                    {
                        (currentSelectedItem.Content as ScheduleDaysView).UpdateSelection(false);
                    }
                    else if (currentSelectedItem.Content is ScheduleTimeLineView)
                    {
                        (currentSelectedItem.Content as ScheduleTimeLineView).UpdateSelection(false);
                    }
                }
            }
#endif
        }

        void MoveToDayType()
        {
            ObservableCollection<DateTime> currentDateCollection = CurrentVisibleSelectedDates;
#if SILVERLIGHT
            DateTime firstDate = !isScheduleLoaded ? CurrentSelectedDates[0] : GetNavigationDate(currentDateCollection);
#else
            DateTime firstDate = !isLoaded ? CurrentSelectedDates[0] : GetNavigationDate(currentDateCollection);
#endif
            MoveToDate(firstDate);
        }

        void MoveToWeekType()
        {
            ObservableCollection<DateTime> currentDateCollection = CurrentVisibleSelectedDates;
#if SILVERLIGHT
            DateTime firstDate = !isScheduleLoaded ? CurrentSelectedDates[0] : GetNavigationDate(currentDateCollection);
#else
            DateTime firstDate = !isLoaded ? CurrentSelectedDates[0] : GetNavigationDate(currentDateCollection);
#endif
            MoveToDate(firstDate);
        }

        void MoveToMonthType()
        {
            ObservableCollection<DateTime> currentDateCollection = CurrentVisibleSelectedDates;
#if SILVERLIGHT
            DateTime firstDate = !isLoaded ? CurrentSelectedDates[0] : GetNavigationDate(currentDateCollection);
#else
            DateTime firstDate = !isTemplateApplied ? CurrentSelectedDates[0] : GetNavigationDate(currentDateCollection);
#endif
            MoveToDate(firstDate);
        }

        void MoveToTimeLineType()
        {
            ObservableCollection<DateTime> currentDateCollection = CurrentVisibleSelectedDates;
#if SILVERLIGHT
            DateTime firstDate = !isScheduleLoaded ? CurrentSelectedDates[0] : GetNavigationDate(currentDateCollection);
#else
            DateTime firstDate = !isLoaded ? CurrentSelectedDates[0] : GetNavigationDate(currentDateCollection);
#endif
            MoveToDate(firstDate);
        }

        internal void ClearContextmenu()
        {
            if (editpopup != null && editpopup.IsOpen)
            {
                editpopup.IsOpen = false;
            }
            if (addnewpopup != null && addnewpopup.IsOpen)
            {
                addnewpopup.IsOpen = false;
            }
            
        }

        void UpdateMainViewItems()
        {
#if WPF
            if (EnableTouch == false)
#endif
            {
                if (currentSelectedItem != null)
                {
                    switch (ScheduleType)
                    {
                        case ScheduleType.Day:
                    	case ScheduleType.WorkWeek:
                        case ScheduleType.Week:
                            currentSelectedItem.Content = daysview;
                            daysview.SetNavigationTapVisibility();
                            IsVisibleDateSetInternally = true;
                            VisibleDates = daysview.SelectedDates;
                            IsVisibleDateSetInternally = false;
                            CurrentVisibleSelectedDates = VisibleDates;
                            break;
                        case ScheduleType.Month:
                            currentSelectedItem.Content = monthview;
                            IsVisibleDateSetInternally = true;
                            VisibleDates = monthview.SelectedDates;
                            IsVisibleDateSetInternally = false;
                            CurrentVisibleSelectedDates = VisibleDates;
                            break;
                        case ScheduleType.TimeLine:
                            currentSelectedItem.Content = timelineview;
                            timelineview.SetNavigationTapVisibility();
                            IsVisibleDateSetInternally = true;
                            VisibleDates = timelineview.SelectedDates;
                            IsVisibleDateSetInternally = false;
                            CurrentVisibleSelectedDates = VisibleDates;
                            break;
                    }
                }
                if (mainItem != null)
                {
                    viewcontrol = currentSelectedItem;
                }
            }
#if WPF
            else
            {
                // flip 
                switch (ScheduleType)
                {
                    case ScheduleType.Day:
                    case ScheduleType.WorkWeek:
                    case ScheduleType.Week:
                        if (daysview2 != null && daysview1 != null && daysview3 != null)
                        {
                            mainViewItem1.Content = daysview2;
                            if (daysview2 != null)
                            {
                                SetNavigationTap();
                                IsVisibleDateSetInternally = true;
                                VisibleDates = daysview2.SelectedDates;
                                IsVisibleDateSetInternally = false;
                                CurrentVisibleSelectedDates = VisibleDates;
                            }
                            //await Task.Delay(50);
                            if (daysview3 != null)
                            {

                                mainViewItem2.Content = daysview3;
                            }
                            //await Task.Delay(50);
                            if (daysview1 != null)
                            {
                                mainViewItem.Content = daysview1;
                            }
                        }
                        break;
                    case ScheduleType.Month:
                        {
                            mainViewItem1.Content = monthview2;
                            SetNavigationTap();
                            IsVisibleDateSetInternally = true;
                            VisibleDates = monthview2.SelectedDates;
                            IsVisibleDateSetInternally = false;
                            CurrentVisibleSelectedDates = VisibleDates;
                            //await Task.Delay(50);
                            if (monthview3 != null)
                            {
                                mainViewItem2.Content = monthview3;
                            }
                            //await Task.Delay(50);
                            if (monthview1 != null)
                            {
                                mainViewItem.Content = monthview1;
                            }
                        }
                        break;
                    case ScheduleType.TimeLine:
                        {
                            mainViewItem1.Content = timelineview2;
                            SetNavigationTap();
                            IsVisibleDateSetInternally = true;
                            VisibleDates = timelineview2.SelectedDates;
                            IsVisibleDateSetInternally = false;
                            CurrentVisibleSelectedDates = VisibleDates;
                            //await Task.Delay(200);
                            if (timelineview3 != null)
                            {
                                mainViewItem2.Content = timelineview3;
                            }
                            //await Task.Delay(200);
                            if (timelineview1 != null)
                            {
                                mainViewItem.Content = timelineview1;
                            }
                        }
                        break;
                }

                if (!isFlipitemvisibilityupdated)
                {
                    if (flipview.Items != null)
                        foreach (UIElement item in flipview.Items)
                        {
                            if (item.Visibility == Visibility.Collapsed)
                                item.Visibility = Visibility.Visible;
                        }
                    isFlipitemvisibilityupdated = true;
                }
                if (flipviewselecteditem != null)
                {
                    viewcontrol = flipviewselecteditem.FindElementOfTypeWithName<ContentControl>("PART_MainViewItems");
                }
            
            }
#endif
        }

        #endregion

        #region Previous and Next items

        void SetNextDate()
        {
            if (SelectedDates != null)
            {
                var selectedDates = new ObservableCollection<DateTime>();
                switch (ScheduleType)
                {
                    case ScheduleType.Day:
                    case ScheduleType.Week:
                    case ScheduleType.TimeLine:
                        {
                            // Max Value 23:59:59.9999999, December 31, 9999,
                            // Min Value 00:00:00.0000000, January 1, 0001.
                            int daysCount = (SelectedDates[SelectedDates.Count - 1].Date - SelectedDates[0].Date).Days;
                            DateTime tempMaxDate = DateTime.MaxValue.AddDays(-(daysCount + 1));
                            exceedsMaxDate = SelectedDates.Any(date => date >= tempMaxDate);
                            if (!exceedsMaxDate)
                            {
                                foreach (DateTime date in SelectedDates)
                                {
                                    selectedDates.Add(date.AddDays(daysCount + 1));
                                }
                            }
                            NextSelectedDates = selectedDates;
                        }
                        break;
                    case ScheduleType.WorkWeek:
                        {
                            ObservableCollection<DayOfWeek> WorkDayCollection = StringToDaysOfWeekConverter(NonWorkingDays, true);
                            if (WorkDayCollection.Count >= 7 || WorkDayCollection.Count <= 0)
                                WorkDayCollection = StringToDaysOfWeekConverter("Saturday,Sunday", true);
                            if (SelectedDates.Count > 0)
                            {
                                var startdate1 = SelectedDates[0].StartOfWeek(DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek).AddDays(13);
                                for (int i = 0; i < WorkDayCollection.Count; i++)
                                {
                                    selectedDates.Add(startdate1.StartOfWeek(WorkDayCollection[i]));
                                }
                                NextSelectedDates = selectedDates;
                            }
                            break;
                        }
                    case ScheduleType.Month:
                        {
                            var firstDate = SelectedDates[7];
                            firstDate = firstDate.AddMonths(1);
                            var monthStart = new DateTime(firstDate.Year, firstDate.Month, 1);
                            var startDate = monthStart.AddDays(-(int)(monthStart.DayOfWeek));
                            for (int i = 0; i < 35; i++)
                            {
                                selectedDates.Add(startDate);
                                startDate = startDate.AddDays(1);
                            }
                            if (startDate.AddDays(-1).Month == monthStart.Month)
                            {
                                if (startDate.Month == monthStart.Month)
                                {
                                    for (int i = 0; i < 7; i++)
                                    {
                                        selectedDates.Add(startDate);
                                        startDate = startDate.AddDays(1);
                                    }
                                }
                            }
                            NextSelectedDates = selectedDates;
                        }
                        break;
                }
            }
        }

        void SetPrevDate()
        {
            if (SelectedDates != null)
            {
                var selectedDates = new ObservableCollection<DateTime>();
                switch (ScheduleType)
                {
                    case ScheduleType.Day:
                    case ScheduleType.TimeLine:
                    case ScheduleType.Week:
                        {
                            int daysCount = (SelectedDates[SelectedDates.Count - 1].Date - SelectedDates[0].Date).Days;
                            DateTime tempMinDate = DateTime.MinValue.AddDays(daysCount + 1);
                            exceedsMinDate = SelectedDates.Any(date => date <= tempMinDate);
                            if (!exceedsMinDate)
                            {
                                foreach (DateTime date in SelectedDates)
                                {
                                    selectedDates.Add(date.AddDays(-(daysCount + 1)));
                                }
                            }
                            PrevSelectedDates = selectedDates;
                        }
                        break;
                    case ScheduleType.WorkWeek:
                        {
                            ObservableCollection<DayOfWeek> WorkDayCollection = StringToDaysOfWeekConverter(NonWorkingDays, true);
                            if (WorkDayCollection.Count >= 7 || WorkDayCollection.Count <= 0)
                                WorkDayCollection = StringToDaysOfWeekConverter("Saturday,Sunday", true);
                            if (SelectedDates.Count > 0)
                            {
                                var startdate1 = SelectedDates[0].StartOfWeek(DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek).SubractDays(1);
                                for (int i = 0; i < WorkDayCollection.Count; i++)
                                {
                                    selectedDates.Add(startdate1.StartOfWeek(WorkDayCollection[i]));
                                }
                                PrevSelectedDates = selectedDates;
                            }
                            break;
                        }
                    case ScheduleType.Month:
                        {
                            var firstDate = SelectedDates[7];
                            firstDate = firstDate.AddMonths(-1);
                            var monthStart = new DateTime(firstDate.Year, firstDate.Month, 1);
                            var startDate = monthStart.AddDays(-(int)(monthStart.DayOfWeek));
                            for (int i = 0; i < 35; i++)
                            {
                                selectedDates.Add(startDate);
                                startDate = startDate.AddDays(1);
                            }
                            if (startDate.AddDays(-1).Month == monthStart.Month)
                            {
                                if (startDate.Month == monthStart.Month)
                                {
                                    for (int i = 0; i < 7; i++)
                                    {
                                        selectedDates.Add(startDate);
                                        startDate = startDate.AddDays(1);
                                    }
                                }
                            }
                            PrevSelectedDates = selectedDates;
                        }
                        break;
                }
            }
        }

        internal void UpdateNextItem()
        {
            if (exceedsMinDate)
                exceedsMinDate = false;
            if (!exceedsMaxDate && CurrentSelectedDates != null)
            {
                if (editpopup != null)
                {
                    editpopup.IsOpen = false;
                    addnewpopup.IsOpen = false;
                }
                var selectedDates = new ObservableCollection<DateTime>();
                switch (ScheduleType)
                {
                    case ScheduleType.Day:
                    case ScheduleType.Week:
                    case ScheduleType.TimeLine:
                        {                           
                            int daysCount = (CurrentSelectedDates[CurrentSelectedDates.Count - 1].Date - CurrentSelectedDates[0].Date).Days + 1;
                            DateTime tempMaxDate = DateTime.MaxValue.AddDays(-daysCount);
                            exceedsMaxDate = CurrentSelectedDates.Any(date => date >= tempMaxDate);
                            if (!exceedsMaxDate)
                            {
                                foreach (DateTime date in CurrentSelectedDates)
                                {
                                    selectedDates.Add(date.AddDays(daysCount));
                                }
                            }
                        }
                        break;
                    case ScheduleType.WorkWeek:
                        {
                            ObservableCollection<DayOfWeek> WorkDayCollection = StringToDaysOfWeekConverter(NonWorkingDays, true);
                            if (WorkDayCollection.Count >= 7 || WorkDayCollection.Count <= 0)
                                WorkDayCollection = StringToDaysOfWeekConverter("Saturday,Sunday", true);
                            if (CurrentSelectedDates.Count > 0)
                            {
                                var startdate1 = CurrentSelectedDates[0].StartOfWeek(DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek).AddDays(13);
                                for (int i = 0; i < WorkDayCollection.Count; i++)
                                {
                                    selectedDates.Add(startdate1.StartOfWeek(WorkDayCollection[i]));
                                }
                            }
                            break;
                        }
                    case ScheduleType.Month:
                        {
                            var firstDate = CurrentSelectedDates[7];
                            firstDate = firstDate.AddMonths(1);
                            var monthStart = new DateTime(firstDate.Year, firstDate.Month, 1);
                            var startDate = monthStart.AddDays(-(int)(monthStart.DayOfWeek));
                            for (int i = 0; i < 35; i++)
                            {
                                selectedDates.Add(startDate);
                                startDate = startDate.AddDays(1);
                            }
                            if (startDate.AddDays(-1).Month == monthStart.Month)
                            {
                                if (startDate.Month == monthStart.Month)
                                {
                                    for (int i = 0; i < 7; i++)
                                    {
                                        selectedDates.Add(startDate);
                                        startDate = startDate.AddDays(1);
                                    }
                                }
                            }
                        }
                        break;
                }
                SelectedDates = selectedDates;
                CurrentSelectedDates = selectedDates;
                CurrentVisibleSelectedDates = selectedDates;
            }
        }

        internal void UpdatePrevItem()
        {
            if (exceedsMaxDate)
                exceedsMaxDate = false;
            if (!exceedsMinDate && CurrentSelectedDates != null)
            {
                if (editpopup != null)
                {
                    editpopup.IsOpen = false;
                    addnewpopup.IsOpen = false;
                }
                var selectedDates = new ObservableCollection<DateTime>();
                switch (ScheduleType)
                {
                    case ScheduleType.Day:
                    case ScheduleType.Week:                    
                    case ScheduleType.TimeLine:
                        {                           
                            int daysCount = (CurrentSelectedDates[CurrentSelectedDates.Count - 1].Date - CurrentSelectedDates[0].Date).Days + 1;
                            DateTime tempMinDate = DateTime.MinValue.AddDays(daysCount);
                            exceedsMinDate = CurrentSelectedDates.Any(date => date <= tempMinDate);
                            if (!exceedsMinDate)
                            {
                                foreach (DateTime date in CurrentSelectedDates)
                                {
                                    selectedDates.Add(date.AddDays(-daysCount));
                                }
                            }
                        }
                        break;
                    case ScheduleType.WorkWeek:
                        {
                            ObservableCollection<DayOfWeek> WorkDayCollection = StringToDaysOfWeekConverter(NonWorkingDays, true);
                            if (WorkDayCollection.Count >= 7 || WorkDayCollection.Count <= 0)
                                WorkDayCollection = StringToDaysOfWeekConverter("Saturday,Sunday", true);
                            if (CurrentSelectedDates.Count > 0)
                            {
                                var startdate1 = CurrentSelectedDates[0].StartOfWeek(DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek).SubractDays(1);
                                for (int i = 0; i < WorkDayCollection.Count; i++)
                                {
                                    selectedDates.Add(startdate1.StartOfWeek(WorkDayCollection[i]));
                                }
                            }
                            break;
                        }
                    case ScheduleType.Month:
                        {
                            var firstDate = CurrentSelectedDates[7];
                            firstDate = firstDate.AddMonths(-1);
                            var monthStart = new DateTime(firstDate.Year, firstDate.Month, 1);
                            var startDate = monthStart.AddDays(-(int)(monthStart.DayOfWeek));
                            for (int i = 0; i < 35; i++)
                            {
                                selectedDates.Add(startDate);
                                startDate = startDate.AddDays(1);
                            }
                            if (startDate.AddDays(-1).Month == monthStart.Month)
                            {
                                if (startDate.Month == monthStart.Month)
                                {
                                    for (int i = 0; i < 7; i++)
                                    {
                                        selectedDates.Add(startDate);
                                        startDate = startDate.AddDays(1);
                                    }
                                }
                            }
                        }
                        break;

                }                               
                SelectedDates = selectedDates;
                CurrentSelectedDates = selectedDates;
                CurrentVisibleSelectedDates = selectedDates;
            }
        }

        internal void UpdateNextTouchItem(ObservableCollection<DateTime> CurrentDates, ObservableCollection<DateTime> Dates)
        {
            int selectedDateRef = 0;
            if (Dates.Equals(PrevSelectedDates))
            {
                selectedDateRef = 1;
            }
            else if (Dates.Equals(CurrentSelectedDates))
            {
                selectedDateRef = 2;
            }
            else if (Dates.Equals(NextSelectedDates))
            {
                selectedDateRef = 3;
            }
            if (CurrentDates != null)
            {
                var selectedDates = new ObservableCollection<DateTime>();
                switch (ScheduleType)
                {
                    case ScheduleType.Day:
                    case ScheduleType.Week:                   
                    case ScheduleType.TimeLine:
                        {
                            int daysCount = (CurrentDates[CurrentDates.Count - 1].Date - CurrentDates[0].Date).Days + 1;
                            DateTime tempMaxDate = DateTime.MaxValue.AddDays(-daysCount);
                            exceedsMaxDate = CurrentDates.Any(date => date >= tempMaxDate);
                            if (!exceedsMaxDate)
                            {
                                foreach (DateTime date in CurrentDates)
                                {
                                    selectedDates.Add(date.AddDays(daysCount));
                                }
                            }
                        }
                        break;
                    case ScheduleType.WorkWeek:
                        {
                            ObservableCollection<DayOfWeek> WorkDayCollection = StringToDaysOfWeekConverter(NonWorkingDays, true);
                            if (WorkDayCollection.Count >= 7 || WorkDayCollection.Count <= 0)
                                WorkDayCollection = StringToDaysOfWeekConverter("Saturday,Sunday", true);
                            var startdate1 = CurrentDates[0].StartOfWeek(DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek).AddDays(13);
                            for (int i = 0; i < WorkDayCollection.Count; i++)
                            {
                                selectedDates.Add(startdate1.StartOfWeek(WorkDayCollection[i]));
                            }
                            break;
                        }
                    case ScheduleType.Month:
                        {
                            var firstDate = CurrentDates[7];
                            firstDate = firstDate.AddMonths(1);
                            var monthStart = new DateTime(firstDate.Year, firstDate.Month, 1);
                            var startDate = monthStart.AddDays(-(int)(monthStart.DayOfWeek));
                            for (int i = 0; i < 35; i++)
                            {

                                selectedDates.Add(startDate);
                                startDate = startDate.AddDays(1);

                            }
                            if (startDate.AddDays(-1).Month == monthStart.Month)
                            {
                                if (startDate.Month == monthStart.Month)
                                {
                                    for (int i = 0; i < 7; i++)
                                    {

                                        selectedDates.Add(startDate);
                                        startDate = startDate.AddDays(1);

                                    }
                                }
                            }

                        }
                        break;
                }

                switch (selectedDateRef)
                {
                    case 1:
                        PrevSelectedDates = selectedDates;
                        break;
                    case 2:
                        CurrentSelectedDates = selectedDates;
                        break;
                    case 3:
                        NextSelectedDates = selectedDates;
                        break;
                }
            }
        }

        internal void UpdatePrevTouchItem(ObservableCollection<DateTime> CurrentDates, ObservableCollection<DateTime> Dates)
        {
            int selectedDateRef = 0;
            if (Dates.Equals(PrevSelectedDates))
            {
                selectedDateRef = 1;
            }
            else if (Dates.Equals(CurrentSelectedDates))
            {
                selectedDateRef = 2;
            }
            else if (Dates.Equals(NextSelectedDates))
            {
                selectedDateRef = 3;
            }
            if (CurrentDates != null)
            {
                var selectedDates = new ObservableCollection<DateTime>();
                switch (ScheduleType)
                {
                    case ScheduleType.Day:
                    case ScheduleType.Week:                   
                    case ScheduleType.TimeLine:
                        {
                            int daysCount = (CurrentDates[CurrentDates.Count - 1].Date - CurrentDates[0].Date).Days + 1;
                            DateTime tempMinDate = DateTime.MinValue.AddDays(daysCount);
                            exceedsMinDate = CurrentDates.Any(date => date <= tempMinDate);
                            if (!exceedsMinDate)
                            {
                                foreach (DateTime date in CurrentDates)
                                {
                                    selectedDates.Add(date.AddDays(-daysCount));
                                }
                            }
                        }
                        break;
                    case ScheduleType.WorkWeek:
                        {
                            ObservableCollection<DayOfWeek> WorkDayCollection = StringToDaysOfWeekConverter(NonWorkingDays, true);
                            if (WorkDayCollection.Count >= 7 || WorkDayCollection.Count <= 0)
                                WorkDayCollection = StringToDaysOfWeekConverter("Saturday,Sunday", true);
                            var startdate1 = CurrentDates[0].StartOfWeek(DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek).SubractDays(1);
                            for (int i = 0; i < WorkDayCollection.Count; i++)
                            {
                                selectedDates.Add(startdate1.StartOfWeek(WorkDayCollection[i]));
                            }
                            break;
                        }
                    case ScheduleType.Month:
                        {
                            var firstDate = CurrentDates[7];
                            firstDate = firstDate.AddMonths(-1);
                            var monthStart = new DateTime(firstDate.Year, firstDate.Month, 1);
                            var startDate = monthStart.AddDays(-(int)(monthStart.DayOfWeek));
                            for (int i = 0; i < 35; i++)
                            {
                                selectedDates.Add(startDate);
                                startDate = startDate.AddDays(1);
                            }
                            if (startDate.AddDays(-1).Month == monthStart.Month)
                            {
                                if (startDate.Month == monthStart.Month)
                                {
                                    for (int i = 0; i < 7; i++)
                                    {

                                        selectedDates.Add(startDate);
                                        startDate = startDate.AddDays(1);

                                    }
                                }
                            }

                        }
                        break;

                }
                switch (selectedDateRef)
                {
                    case 1:
                        PrevSelectedDates = selectedDates;
                        break;
                    case 2:
                        CurrentSelectedDates = selectedDates;
                        break;
                    case 3:
                        NextSelectedDates = selectedDates;
                        break;
                }
            }
        }

        #endregion

        #region Navigation Date

        DateTime GetNavigationDate(ObservableCollection<DateTime> currentDateCollection)
        {
            var NavigationDate = new DateTime();
            if (currentDateCollection != null)
            {
                if (currentDateCollection.Contains(SelectedDate))
                {
                    NavigationDate = SelectedDate;
                }
                else
                {
                    if (currentDateCollection.Count < 7)
                    {
                        NavigationDate = currentDateCollection[0].StartOfWeek(SelectedDate.DayOfWeek);
                    }
                    else if (currentDateCollection.Count == 7)
                    {
                        NavigationDate = currentDateCollection.FirstOrDefault(dt => dt.DayOfWeek.Equals(SelectedDate.DayOfWeek));
                    }
                    else if (currentDateCollection.Count > 7)
                    {
                        int selectedweekcount = SelectedDate.GetWeekOfMonth();
                        int currentweekcount = currentDateCollection[currentDateCollection.Count / 2].Date.WeeksInMonth();
                        if (selectedweekcount > currentweekcount)
                        {
                            selectedweekcount = currentweekcount;
                        }
                        DayOfWeek selecteddayofweek = SelectedDate.DayOfWeek;
                        NavigationDate = currentDateCollection.FirstOrDefault(dt => (dt.GetWeekOfMonth().Equals(selectedweekcount) && dt.DayOfWeek.Equals(selecteddayofweek)));
                    }
                    if (NavigationDate == new DateTime())
                    {
                        if (CurrentSelectedDates.Count < 7)
                        {
                            NavigationDate = CurrentSelectedDates[0].StartOfWeek(SelectedDate.DayOfWeek);
                        }
                        else if (CurrentSelectedDates.Count == 7)
                        {
                            NavigationDate = CurrentSelectedDates.FirstOrDefault(dt => dt.DayOfWeek.Equals(SelectedDate.DayOfWeek));
                        }
                        else if (currentDateCollection.Count > 7)
                        {
                            int selectedweekcount = SelectedDate.GetWeekOfMonth();
                            int currentweekcount = CurrentSelectedDates[CurrentSelectedDates.Count / 2].Date.WeeksInMonth();
                            if (selectedweekcount > currentweekcount)
                            {
                                selectedweekcount = currentweekcount;
                            }
                            DayOfWeek selecteddayofweek = SelectedDate.DayOfWeek;
                            NavigationDate = CurrentSelectedDates.FirstOrDefault(dt => (dt.GetWeekOfMonth().Equals(selectedweekcount) && dt.DayOfWeek.Equals(selecteddayofweek)));
                        }
                    }
                }
            }
            else
            {
                if (CurrentSelectedDates.Count < 7)
                {
                    NavigationDate = CurrentSelectedDates[0].StartOfWeek(SelectedDate.DayOfWeek);
                }
                else if (CurrentSelectedDates.Count == 7)
                {
                    NavigationDate = CurrentSelectedDates.FirstOrDefault(dt => dt.DayOfWeek.Equals(SelectedDate.DayOfWeek));
                }
            }
            if (NavigationDate == new DateTime())
            {
                NavigationDate = currentDateCollection != null ? currentDateCollection[0] : CurrentSelectedDates[0];
            }
            return NavigationDate;
        }

        void SetNavigationTap()
        {
            if (currentSelectedItem != null)
            {
                var contentControl = currentSelectedItem as ContentControl;
                if (contentControl != null)
                {
                    switch (ScheduleType)
                    {
                        case ScheduleType.Day:
                        case ScheduleType.WorkWeek:
                        case ScheduleType.Week:
                            if (contentControl.Content is ScheduleDaysView)
                            {
                                (contentControl.Content as ScheduleDaysView).SetNavigationTapVisibility();
                                if (!this.CurrentSelectedDates.Contains(this.SelectedDate.Date) && currentAllDaySelectedItem == null)
                                {
                                    (contentControl.Content as ScheduleDaysView).RectVisibility = System.Windows.Visibility.Collapsed;
                                }
                                else if(currentAllDaySelectedItem == null)
                                {
                                    (contentControl.Content as ScheduleDaysView).RectVisibility = System.Windows.Visibility.Visible;
                                }
                            }
                            break;
                        case ScheduleType.Month:
                            if (contentControl.Content is ScheduleMonthView)
                            {
                                (contentControl.Content as ScheduleMonthView).SetNavigationTapVisibility();
                            }
                            break;
                        case ScheduleType.TimeLine:
                            if (contentControl.Content is ScheduleTimeLineView)
                            {
                                (contentControl.Content as ScheduleTimeLineView).SetNavigationTapVisibility();                               
                            }
                            break;
                    }
                }
            }
        }

        #endregion

        #region Calculating Month Dates

        ObservableCollection<DateTime> CalculateMonth(DateTime firstDate)
        {
            var monthStart = new DateTime(firstDate.Year, firstDate.Month, 1);
            var startDate = monthStart.AddDays(-(int)monthStart.DayOfWeek);
            var monthend = monthStart.AddDays(DateTime.DaysInMonth(monthStart.Year, monthStart.Month) - 1);
            var enddate = monthend.AddDays(6 - (int)monthend.DayOfWeek);
            var newDates = new ObservableCollection<DateTime> { startDate };
            int totaldays = DateTime.DaysInMonth(monthStart.Year, monthStart.Month) + (monthStart - startDate).TotalDays + (enddate - monthend).TotalDays > 35 ? 42 : 35;
            for (int i = 1; i < totaldays; i++)
            {
                startDate = startDate.AddDays(1);
                newDates.Add(startDate);
            }
            return newDates;
        }

        #endregion

        #region Appointment Mapping Appointments

        void CreateAppointmentsForItemsSource()
        {
            if (!(ItemsSource is IEnumerable)) return;
            bool containitems = (ItemsSource as IEnumerable).Cast<object>().Any();
            if (containitems)
            {
                if (Appointments != null)
                    Appointments.Clear();
                else
                    Appointments = new ScheduleAppointmentCollection();
                IEnumerator a = (ItemsSource as IEnumerable).GetEnumerator();
                a.MoveNext();
                var appointments = new ScheduleAppointmentCollection();
#if SILVERLIGHT
                AddAppointments(appointments, a);
#else
                if (ItemsSource is DataRowCollection)
                {
                    var procoll = (from pf in AppointmentMapping.GetType().GetProperties() where (pf.GetValue(AppointmentMapping, null).ToString() != string.Empty && pf.Name.Contains("Mapping")) select pf).ToList();
                    var prostringcoll = procoll.Select(pr => pr.Name).ToList();
                    var properties = typeof(ScheduleAppointment).GetFields().Where(x => x.FieldType.Name == "DependencyProperty").Where(x => x.IsPublic).ToArray();
                    do
                    {
                        int i = 0;
                        var curapp = new ScheduleAppointment { ObjectID = a.Current.GetHashCode()};
                        foreach (var item in properties)
                        {
                            var depentproperty = (DependencyProperty)item.GetValue(null);
                            string str = item.Name.Insert(item.Name.Length - 8, "Mapping");
                            if (prostringcoll.Contains(str.Substring(0, str.Length - 8)))
                            {
                                int index = prostringcoll.IndexOf(str.Substring(0, str.Length - 8));
                                i++;
                                if (a.Current is DataRow)
                                    curapp.SetValue(depentproperty, (a.Current as DataRow)[procoll[index].GetValue(AppointmentMapping, null).ToString()]);
                                if (i == prostringcoll.Count)
                                    break;
                            }
                        }
                        appointments.Add(curapp);
                    } while (a.MoveNext());
                }
                else
                    AddAppointments(appointments, a);
#endif
                Appointments = appointments;
            }
        }

        private void AddAppointments(ScheduleAppointmentCollection appointments, IEnumerator a)
        {
            var accessors = new Dictionary<string, IPropertyAccessor>();
            Reflection(accessors, a.Current);
            do
            {
                var app = CreateAppointment(accessors, a.Current);
                appointments.Add(app);
            } while (a.MoveNext());
        }

        void Reflection(Dictionary<string, IPropertyAccessor> accessors, object obj)
        {
            var properties = typeof(ScheduleAppointmentMapping).GetFields();
            foreach (var item in properties)
            {
                var depentproperty = (DependencyProperty)item.GetValue(null);
                if (depentproperty == null) continue;
                PropertyInfo propertyInfo = obj.GetType().GetTypeInfo().GetDeclaredProperty((AppointmentMapping.GetValue(depentproperty)).ToString());
                if (propertyInfo == null) continue;
                IPropertyAccessor accessor = FastReflectionCaches.PropertyAccessorCache.Get(propertyInfo);
                accessors.Add(item.Name, accessor);
            }
        }

        void UpdateAppointment(ScheduleAppointment app, object obj, string property)
        {
            PropertyInfo propinfo = obj.GetType().GetTypeInfo().GetDeclaredProperty(property);
            if (propinfo == null) return;
            IPropertyAccessor accessor = FastReflectionCaches.PropertyAccessorCache.Get(propinfo);
            var properties = typeof(ScheduleAppointmentMapping).GetFields().Where(x => x.FieldType.Name == "DependencyProperty").Where(x => x.IsPublic);
            string propertyname = (from item in properties let depentproperty = (DependencyProperty)item.GetValue(null) where depentproperty != null where AppointmentMapping.GetValue(depentproperty).Equals(property) select item.Name).FirstOrDefault();

            var prop = typeof(ScheduleAppointment).GetFields().Where(x => x.FieldType.Name == "DependencyProperty").Where(x => x.IsPublic);
            foreach (var items in prop)
            {
                var depentproperty = (DependencyProperty)items.GetValue(null);
                string name = items.Name.Insert(items.Name.Length - 8, "Mapping");
                if (name.Equals(propertyname))
                {
                    app.SetValue(depentproperty, accessor.GetValue(obj));
                }
            }
        }

        ScheduleAppointment CreateAppointment(Dictionary<string, IPropertyAccessor> pd, object rec)
        {
            var app = new ScheduleAppointment { ObjectID = rec.GetHashCode() };
            var properties = typeof(ScheduleAppointment).GetFields().Where(x => x.FieldType.Name == "DependencyProperty").Where(x => x.IsPublic);
            foreach (var item in properties)
            {
                var depentproperty = (DependencyProperty)item.GetValue(null);
                string str = item.Name.Insert(item.Name.Length - 8, "Mapping");
                if (pd.Keys.Contains(str))
                {
                    if (pd[str].GetValue(rec) != null)
                        app.SetValue(depentproperty, pd[str].GetValue(rec));
                }
            }
            return app;
        }

        #endregion

        #region Adding Visible Appointments

        void SetPrevVisibleAppointments(IEnumerable<DateTime> selectedDates)
        {
            if (PrevVisibleAppointments != null)
            {
                PrevVisibleAppointments.Clear();
            }
            var tem1coll = new List<ScheduleAppointment>();
            var sappcoll = new ScheduleAppointmentCollection();
            if (ProxyAppointments.Count > 0)
            {
                if (ScheduleType == ScheduleType.Month)
                {
                    foreach (DateTime dt in selectedDates.OrderBy(p => p))
                    {
                        int i = 0;
                        if (ProxyAppointments.ContainsKey(dt.Date))
                        {
                            if (ScheduleResourceType != null && ScheduleResourceType.ResourceCollection.Count > 0)
                            {
                                tem1coll.AddRange(ProxyAppointments[dt.Date]);
                            }
                            else
                            {
                                foreach (ScheduleAppointment shedapp in ProxyAppointments[dt.Date])
                                {
                                    if (i > 2)
                                    {
                                        break;
                                    }
                                    tem1coll.Add(shedapp);
                                    if (shedapp.StartTime.Date == dt)
                                    {
                                        i++;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    tem1coll.AddRange(selectedDates.OrderBy(p => p).Where(dt => ProxyAppointments.ContainsKey(dt.Date)).SelectMany(dt => ProxyAppointments[dt.Date]));
                }
            }

            foreach (ScheduleAppointment app in tem1coll)
            {
                sappcoll.Add(app);
            }
            PrevVisibleAppointments = sappcoll;
        }

        void SetCurrentVisibleAppointments(IEnumerable<DateTime> selectedDates)
        {
            if (CurrentVisibleAppointments != null)
            {
                CurrentVisibleAppointments.Clear();
            }

            var tem1coll = new List<ScheduleAppointment>();
            var sappcoll = new ScheduleAppointmentCollection();
            if (ProxyAppointments.Count > 0)
            {
                if (ScheduleType == ScheduleType.Month)
                {
                    foreach (DateTime dt in selectedDates.OrderBy(p => p))
                    {
                        int i = 0;
                        if (ProxyAppointments.ContainsKey(dt.Date))
                        {
                            if (ScheduleResourceType != null && ScheduleResourceType.ResourceCollection.Count > 0)
                            {
                                tem1coll.AddRange(ProxyAppointments[dt.Date]);
                            }
                            else
                            {
                                foreach (ScheduleAppointment shedapp in ProxyAppointments[dt.Date])
                                {
                                    if (i > 2)
                                    {
                                        break;
                                    }
                                    tem1coll.Add(shedapp);
                                    if (shedapp.StartTime.Date == dt)
                                    {
                                        i++;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    tem1coll.AddRange(selectedDates.OrderBy(p => p).Where(dt => ProxyAppointments.ContainsKey(dt.Date)).SelectMany(dt => ProxyAppointments[dt.Date]));
                }
            }

            foreach (ScheduleAppointment app in tem1coll)
            {
                sappcoll.Add(app);
            }
            CurrentVisibleAppointments = sappcoll;
        }

        void SetNextVisibleAppointments(IEnumerable<DateTime> selectedDates)
        {
            if (NextVisibleAppointments != null)
            {
                NextVisibleAppointments.Clear();
            }

            var tem1coll = new List<ScheduleAppointment>();
            var sappcoll = new ScheduleAppointmentCollection();
            if (ProxyAppointments.Count > 0)
            {
                if (ScheduleType == ScheduleType.Month)
                {
                    foreach (DateTime dt in selectedDates.OrderBy(p => p))
                    {
                        int i = 0;
                        if (ProxyAppointments.ContainsKey(dt.Date))
                        {
                            if (ScheduleResourceType != null && ScheduleResourceType.ResourceCollection.Count > 0)
                            {
                                tem1coll.AddRange(ProxyAppointments[dt.Date]);
                            }
                            else
                            {
                                foreach (ScheduleAppointment shedapp in ProxyAppointments[dt.Date])
                                {
                                    if (i > 2)
                                    {
                                        break;
                                    }
                                    tem1coll.Add(shedapp);
                                    if (shedapp.StartTime.Date == dt)
                                    {
                                        i++;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    tem1coll.AddRange(selectedDates.OrderBy(p => p).Where(dt => ProxyAppointments.ContainsKey(dt.Date)).SelectMany(dt => ProxyAppointments[dt.Date]));
                }
            }

            foreach (ScheduleAppointment app in tem1coll)
            {
                sappcoll.Add(app);
            }
            NextVisibleAppointments = sappcoll;
        }

        #endregion

        #region Adding/Removing Proxy Appointments

        void AddRecursive()
        {
            foreach (DateTime dt in totalselectedDates.OrderBy(p => p))
            {
                AddRecursiveAppointmentsCopy(dt.Date);
            }
        }

        void Add(DateTime key, ScheduleAppointment thing)
        {
            if (thing.IsRecursive)
            {
                if (!RecursiveAppointments.Contains(thing))
                    RecursiveAppointments.Add(thing);
                if (!RecursiveAddedDates.ContainsKey(thing.RecurrenceID) && (thing.RecurrenceProperites != null || thing.RecurrenceRule != ""))
                {
                    ObservableRangeCollection<DateTime> recDateTime = null;
                    if (thing.RecurrenceProperites != null)
                    {
                        if (thing.RecurrenceProperites.IsRangeNoEndDate)
                        {
                            // call duplicate method
                            ScheduleAppointment dummyApp = CreateNoEndDateDummyAppointment(thing, CurrentSelectedDates);
                            if (thing.RecurrenceRule == string.Empty || thing.RecurrenceRule == "" || !(thing.RecurrenceRule.Contains("COUNT")))
                            {
                                if (dummyApp != null)
                                    recDateTime = (ObservableRangeCollection<DateTime>)ScheduleHelper.GetRecurrenceDateTimeCollection(ScheduleHelper.InternalRRuleGenerator(dummyApp.RecurrenceProperites, dummyApp.InternalStartTime, dummyApp.InternalEndTime), dummyApp.RecurrenceProperites.RangeStartDate);
                                dummyApp = CreateNoEndDateDummyAppointment(thing, PrevSelectedDates);
                                if (dummyApp != null)
                                    recDateTime.AddRange(ScheduleHelper.GetRecurrenceDateTimeCollection(ScheduleHelper.InternalRRuleGenerator(dummyApp.RecurrenceProperites, dummyApp.InternalStartTime, dummyApp.InternalEndTime), dummyApp.RecurrenceProperites.RangeStartDate));
                                dummyApp = CreateNoEndDateDummyAppointment(thing, NextSelectedDates);
                                if (dummyApp != null)
                                    recDateTime.AddRange(ScheduleHelper.GetRecurrenceDateTimeCollection(ScheduleHelper.InternalRRuleGenerator(dummyApp.RecurrenceProperites, dummyApp.InternalStartTime, dummyApp.InternalEndTime), dummyApp.RecurrenceProperites.RangeStartDate));
                            }
                            else
                            {
                                if (dummyApp != null)
                                    recDateTime = (ObservableRangeCollection<DateTime>)ScheduleHelper.GetRecurrenceDateTimeCollection(dummyApp.RecurrenceRule, dummyApp.RecurrenceProperites.RangeStartDate);
                                dummyApp = CreateNoEndDateDummyAppointment(thing, PrevSelectedDates);
                                recDateTime.AddRange(ScheduleHelper.GetRecurrenceDateTimeCollection(dummyApp.RecurrenceRule, dummyApp.RecurrenceProperites.RangeStartDate));
                                dummyApp = CreateNoEndDateDummyAppointment(thing, NextSelectedDates);
                                recDateTime.AddRange(ScheduleHelper.GetRecurrenceDateTimeCollection(dummyApp.RecurrenceRule, dummyApp.RecurrenceProperites.RangeStartDate));
                            }
                        }
                        else
                        {
                            if (thing.RecurrenceRule == string.Empty || thing.RecurrenceRule == "" || !(thing.RecurrenceRule.Contains("COUNT")))
                            {
                                recDateTime = (ObservableRangeCollection<DateTime>)ScheduleHelper.GetRecurrenceDateTimeCollection(ScheduleHelper.RRuleGenerator(thing.RecurrenceProperites, thing.InternalStartTime, thing.InternalEndTime), thing.RecurrenceProperites.RangeStartDate);
                            }
                            else
                            {
                                recDateTime = (ObservableRangeCollection<DateTime>)ScheduleHelper.GetRecurrenceDateTimeCollection(thing.RecurrenceRule, thing.RecurrenceProperites.RangeStartDate);
                            }
                        }
                    }
                    else
                    {
                        if (thing.RecurrenceRule == string.Empty || thing.RecurrenceRule == "" || !(thing.RecurrenceRule.Contains("COUNT")))
                        {
                            recDateTime = (ObservableRangeCollection<DateTime>)ScheduleHelper.GetRecurrenceDateTimeCollection(ScheduleHelper.RRuleGenerator(thing.RecurrenceProperites, thing.InternalStartTime, thing.InternalEndTime), thing.StartTime.Date);
                        }
                        else
                        {
                            recDateTime = (ObservableRangeCollection<DateTime>)ScheduleHelper.GetRecurrenceDateTimeCollection(thing.RecurrenceRule, thing.StartTime.Date);
                        }
                    }
                    RecursiveAddedDates.Add(thing.RecurrenceID, recDateTime);
                }
            }
            else
            {
                if (ProxyAppointments != null)
                {
                    for (DateTime dt = key; dt <= thing.InternalEndTime.Date; dt = dt.AddDays(1).Date)
                    {
                        if (ProxyAppointments.ContainsKey(dt))
                        {
                            if (!ProxyAppointments[dt].Contains(thing))
                                ProxyAppointments[dt].Add(thing);
                        }
                        else
                        {
                            ProxyAppointments.Add(dt, new ObservableCollection<ScheduleAppointment> { thing });
                        }
                    }
                }
            }
        }

        void AddRecursiveApp(DateTime key, ScheduleAppointment thing)
        {
            var recNewApp = new ScheduleAppointment();
            TimeSpan diff = key - thing.InternalStartTime.Date;
            recNewApp.StartTime = thing.StartTime.AddDays(diff.Days);
            recNewApp.EndTime = thing.EndTime.AddDays(diff.Days);
            recNewApp.Subject = thing.Subject;
            recNewApp.Notes = thing.Notes;
            recNewApp.RecurrenceProperites = thing.RecurrenceProperites;
            recNewApp.RecurrenceRule = thing.RecurrenceRule;
            recNewApp.IsRecursive = thing.IsRecursive;
            recNewApp.RecurrenceID = thing.RecurrenceID;
            recNewApp.Status = thing.Status;
            recNewApp.AllDay = thing.AllDay;
            recNewApp.AppointmentBackground = thing.AppointmentBackground;
            recNewApp.Location = thing.Location;
            recNewApp.ReadOnly = thing.ReadOnly;
            recNewApp.ReminderTime = thing.ReminderTime;
            recNewApp.ResourceCollection = thing.ResourceCollection;
            if (thing.ReminderTime != ReminderTimeType.None)
                recNewApp.ReminderDeliveryTime = thing.InternalStartTime - thing.MeasureTime(thing.ReminderTime);
            if (ProxyAppointments != null && ProxyAppointments.ContainsKey(key) && !ProxyAppointments[key].Contains(recNewApp))
            {
                ProxyAppointments[key].Add(recNewApp);
            }
            else if (ProxyAppointments != null && !ProxyAppointments.ContainsKey(key))
            {
                ProxyAppointments.Add(key, new ObservableCollection<ScheduleAppointment> { recNewApp });
            }
           // recNewApp.PropertyChanged += item_PropertyChanged;
        }

        void AddRecursiveAppointmentsCopy(DateTime key)
        {
            if (RecursiveAppointments != null && RecursiveAppointments.Count != 0)
            {
                AddDateRecurreceNoEndDate();

                var recApp = (from r in RecursiveAddedDates
                              where ((r.Value.Contains(key)))
                              select r.Key).ToList();
                List<ScheduleAppointment> recAppList = (from app in RecursiveAppointments where (recApp.Contains(app.RecurrenceID)) select app).ToList();
                foreach (ScheduleAppointment schApp in recAppList)
                {
                    if (ProxyAppointments.ContainsKey(key))
                    {
                        ScheduleAppointment app = schApp;
                        var recapp = (ProxyAppointments[key].Where(r => r.RecurrenceID.Equals(app.RecurrenceID))).ToList();
                        if (recapp.Count == 0)
                            AddRecursiveApp(key, schApp);
                    }
                    else
                    {
                        AddRecursiveApp(key, schApp);
                    }
                }
            }
        }

        ScheduleAppointment CreateNoEndDateDummyAppointment(ScheduleAppointment recApp, IEnumerable<DateTime> dateCollection)
        {
            var dummyapp = new ScheduleAppointment { RecurrenceProperites = new RecurrenceProperties() };
            var dateTimes = dateCollection as DateTime[] ?? dateCollection.ToArray();
            if (recApp.RecurrenceProperites.RangeStartDate.Date <= dateTimes.First() || (recApp.RecurrenceProperites.RangeStartDate.Date >= dateTimes.First() && recApp.RecurrenceProperites.RangeStartDate.Date <= dateTimes.Last()))
            {
                IsRRuleSetInternally = true;
                dummyapp.hasInternalRule = true;
                dummyapp.RecurrenceRule = recApp.RecurrenceRule;
                dummyapp.hasInternalRule = false;
                IsRRuleSetInternally = false;
                dummyapp.RecurrenceProperites.RecurrenceRule = recApp.RecurrenceProperites.RecurrenceRule;
                dummyapp.RecurrenceProperites.RecurrenceType = recApp.RecurrenceProperites.RecurrenceType;
                dummyapp.RecurrenceProperites.IsRangeRecurrenceCount = recApp.RecurrenceProperites.IsRangeRecurrenceCount;
                dummyapp.RecurrenceProperites.IsRangeEndDate = true;
                dummyapp.RecurrenceProperites.IsRangeNoEndDate = false;
                dummyapp.RecurrenceProperites.RangeStartDate = recApp.RecurrenceProperites.RangeStartDate.Date >= dateTimes.First() ? recApp.RecurrenceProperites.RangeStartDate : dateTimes.First();
                dummyapp.RecurrenceProperites.RangeEndDate = dateTimes.Last();
                dummyapp.RecurrenceProperites.RangeRecurrenceCount = recApp.RecurrenceProperites.RangeRecurrenceCount;
                dummyapp.RecurrenceProperites.IsDailyEveryNDays = recApp.RecurrenceProperites.IsDailyEveryNDays;
                dummyapp.RecurrenceProperites.DailyNDays = recApp.RecurrenceProperites.DailyNDays;
                dummyapp.RecurrenceProperites.WeeklyEveryNWeeks = recApp.RecurrenceProperites.WeeklyEveryNWeeks;
                dummyapp.RecurrenceProperites.IsWeeklySunday = recApp.RecurrenceProperites.IsWeeklySunday;
                dummyapp.RecurrenceProperites.IsWeeklyMonday = recApp.RecurrenceProperites.IsWeeklyMonday;
                dummyapp.RecurrenceProperites.IsWeeklyTuesday = recApp.RecurrenceProperites.IsWeeklyTuesday;
                dummyapp.RecurrenceProperites.IsWeeklyWednesday = recApp.RecurrenceProperites.IsWeeklyWednesday;
                dummyapp.RecurrenceProperites.IsWeeklyThursday = recApp.RecurrenceProperites.IsWeeklyThursday;
                dummyapp.RecurrenceProperites.IsWeeklyFriday = recApp.RecurrenceProperites.IsWeeklyFriday;
                dummyapp.RecurrenceProperites.IsWeeklySaturday = recApp.RecurrenceProperites.IsWeeklySaturday;
                dummyapp.RecurrenceProperites.MonthlyEveryNMonths = recApp.RecurrenceProperites.MonthlyEveryNMonths;
                dummyapp.RecurrenceProperites.IsMonthlySpecific = recApp.RecurrenceProperites.IsMonthlySpecific;
                dummyapp.RecurrenceProperites.MonthlySpecificMonthDay = recApp.RecurrenceProperites.MonthlySpecificMonthDay;
                dummyapp.RecurrenceProperites.MonthlyNthWeek = recApp.RecurrenceProperites.MonthlyNthWeek;
                dummyapp.RecurrenceProperites.MonthlyWeekDay = recApp.RecurrenceProperites.MonthlyWeekDay;
                dummyapp.RecurrenceProperites.YearlyEveryNYears = recApp.RecurrenceProperites.YearlyEveryNYears;
                dummyapp.RecurrenceProperites.IsYearlySpecific = recApp.RecurrenceProperites.IsYearlySpecific;
                dummyapp.RecurrenceProperites.YearlySpecificMonth = recApp.RecurrenceProperites.YearlySpecificMonth;
                dummyapp.RecurrenceProperites.YearlySpecificMonthDay = recApp.RecurrenceProperites.YearlySpecificMonthDay;
                dummyapp.RecurrenceProperites.YearlyNthWeek = recApp.RecurrenceProperites.YearlyNthWeek;
                dummyapp.RecurrenceProperites.YearlyWeekDay = recApp.RecurrenceProperites.YearlyWeekDay;
                dummyapp.RecurrenceProperites.YearlyGenericMonth = recApp.RecurrenceProperites.YearlyGenericMonth;
                return dummyapp;
            }
            return null;
        }

        void AddDateRecurreceNoEndDate()
        {
            foreach (ScheduleAppointment recapp in from app in RecursiveAppointments where (app.RecurrenceProperites != null && app.RecurrenceProperites.IsRangeNoEndDate) select app)
            {
                ScheduleAppointment dummyApp = CreateNoEndDateDummyAppointment(recapp, CurrentSelectedDates);
                if (dummyApp != null)
                {
                    ObservableRangeCollection<DateTime> recDate;
                    if (dummyApp.RecurrenceRule == string.Empty || dummyApp.RecurrenceRule == "" || !(dummyApp.RecurrenceRule.Contains("COUNT")))
                    {
                        recDate = (ObservableRangeCollection<DateTime>)ScheduleHelper.GetRecurrenceDateTimeCollection(ScheduleHelper.InternalRRuleGenerator(dummyApp.RecurrenceProperites, dummyApp.InternalStartTime, dummyApp.InternalEndTime), dummyApp.RecurrenceProperites.RangeStartDate);
                        dummyApp = CreateNoEndDateDummyAppointment(recapp, PrevSelectedDates);
                        if (dummyApp != null)
                            recDate.AddRange(ScheduleHelper.GetRecurrenceDateTimeCollection(ScheduleHelper.InternalRRuleGenerator(dummyApp.RecurrenceProperites, dummyApp.InternalStartTime, dummyApp.InternalEndTime), dummyApp.RecurrenceProperites.RangeStartDate));
                        dummyApp = CreateNoEndDateDummyAppointment(recapp, NextSelectedDates);
                        if (dummyApp != null)
                            recDate.AddRange(ScheduleHelper.GetRecurrenceDateTimeCollection(ScheduleHelper.InternalRRuleGenerator(dummyApp.RecurrenceProperites, dummyApp.InternalStartTime, dummyApp.InternalEndTime), dummyApp.RecurrenceProperites.RangeStartDate));
                    }
                    else
                    {
                        recDate = (ObservableRangeCollection<DateTime>)ScheduleHelper.GetRecurrenceDateTimeCollection(dummyApp.RecurrenceRule, dummyApp.RecurrenceProperites.RangeStartDate);
                        dummyApp = CreateNoEndDateDummyAppointment(recapp, PrevSelectedDates);
                        recDate.AddRange(ScheduleHelper.GetRecurrenceDateTimeCollection(dummyApp.RecurrenceRule, dummyApp.RecurrenceProperites.RangeStartDate));
                        dummyApp = CreateNoEndDateDummyAppointment(recapp, NextSelectedDates);
                        recDate.AddRange(ScheduleHelper.GetRecurrenceDateTimeCollection(dummyApp.RecurrenceRule, dummyApp.RecurrenceProperites.RangeStartDate));
                    }

                    foreach (DateTime dt in recDate)
                        RecursiveAddedDates[recapp.RecurrenceID].Add(dt);
                }
            }
        }

        void Remove(DateTime key, ScheduleAppointment thing)
        {
            for (DateTime dt = key; dt.Date <= thing.InternalEndTime.Date; dt = dt.AddDays(1).Date)
            {
                if (ProxyAppointments == null || !ProxyAppointments.ContainsKey(dt.Date)) continue;
                ProxyAppointments[dt.Date].Remove(thing);
                if (ProxyAppointments[dt.Date].Count == 0)
                {
                    ProxyAppointments.Remove(key.Date);
                }
            }
        }

        void RemoveRecursiveInProxy(ScheduleAppointment RecApp)
        {
            var DeleteDateList = new List<DateTime>();
            if (RecursiveAddedDates.ContainsKey(RecApp.RecurrenceID))
                DeleteDateList.AddRange(RecursiveAddedDates[RecApp.RecurrenceID]);
            foreach (DateTime dt in DeleteDateList)
            {
                if (!ProxyAppointments.ContainsKey(dt.Date))
                    continue;
                var deletApp = new List<ScheduleAppointment>();
                deletApp.AddRange(ProxyAppointments[dt.Date].Where(app => app.RecurrenceID.Equals(RecApp.RecurrenceID)));
                foreach (ScheduleAppointment app in deletApp)
                {
                    ProxyAppointments[dt.Date].Remove(app);
                }
            }
            if (RecursiveAddedDates.ContainsKey(RecApp.RecurrenceID))
                RecursiveAddedDates[RecApp.RecurrenceID].Clear();
        }

        void RemoveSingleRecursiveAppInProxy(ScheduleAppointment OneRecApp)
        {
            RecursiveAddedDates[OneRecApp.RecurrenceID].Remove(OneRecApp.InternalStartTime.Date);
            ProxyAppointments[OneRecApp.InternalStartTime.Date].Remove(OneRecApp);
        }

        void RemoveRecursive(ScheduleAppointment RecApp)
        {
            RemoveRecursiveInProxy(RecApp);
            RecursiveAddedDates.Remove(RecApp.RecurrenceID);
            RecursiveAppointments.Remove(RecApp);
            Appointments.Remove(RecApp);
        }

        void SetProxyAppointments(IEnumerable<ScheduleAppointment> appointmentcollection)
        {
            foreach (ScheduleAppointment scheduleAppointment in appointmentcollection)
            {
                Add(scheduleAppointment.InternalStartTime.Date, scheduleAppointment);
            }
            totalselectedDates.AddRange(CurrentSelectedDates);
            totalselectedDates.AddRange(PrevSelectedDates);
            totalselectedDates.AddRange(NextSelectedDates);
            AddRecursive();
        }

        #endregion

        #region Adding, Editing, Deleting & Resizing Appointment

        internal void AddNewAppointment()
        {
            if (AllowEditing)
            {
                if (SelectedAppointment != null)
                {
                    SelectedAppointment.IsSelected = false;
                    SelectedAppointment.AppointmentSelectionBrush = new SolidColorBrush(Colors.Transparent);
                    SelectedAppointment = null;
                }
                if (Currentselecteddate == new DateTime())
                {
                    Currentselecteddate = DateTime.Now;
                }
                var EndDate = new DateTime();
                if (ScheduleType == ScheduleType.Month)
                {
                    EndDate = Currentselecteddate;
                }
                else if ((ScheduleType == ScheduleType.Day || ScheduleType == ScheduleType.Week || ScheduleType == ScheduleType.WorkWeek || ScheduleType == ScheduleType.TimeLine))
                {
                    EndDate = Currentselecteddate.Add(GetTimeInterval());

                }
                appointmentEditor = new AppointmentEditor();
                appointmentEditor.SetNewAppointmentProperties(this, Currentselecteddate, EndDate);
                if (editorvisibility == Visibility.Visible)
                {
                    appointmentEditor.Closed += appointmentEditor_Closed;
#if SILVERLIGHT
                    appointmentEditor.Show();
#else
                    appointmentEditor.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    appointmentEditor.Save.Click += Save_Click;
                    appointmentEditor.Delete.Click += Delete_Click;
                    appointmentEditor.ShowDialog();
#endif
                }
            }
            if(addnewpopup != null)
                addnewpopup.IsOpen = false;
        }

        internal void EditAppointment()
        {
            if (AllowEditing)
            {
                appointmentEditor = new AppointmentEditor();
                if (SelectedAppointment != null)
                {
                    if (SelectedAppointment.IsRecursive)
                    {
                        appointmentEditor.Closed += appointmentEditor_Closed;
                        RecurringPopup = new OpenRecurringAppointment();
                        RecAppointment = SelectedAppointment;
#if SILVERLIGHT
                        RecurringPopup.Closed += RecurringPopup_Closed;
                        RecurringPopup.Show();
#else
                        RecurringPopup.Owner = this.FindParentElementOfType<Window>();
                        RecurringPopup.OpenOne.Click += OpenOne_Click;
                        //RecurringPopup.Series.Click += Series_Click;
                        RecurringPopup.ShowDialog();
#endif
                    }
                    else
                    {
                        appointmentEditor = new AppointmentEditor();
                        appointmentEditor.UpdateAppointmentProperties(this, SelectedAppointment);
                        if (editorvisibility == Visibility.Visible)
                        {
                            appointmentEditor.Closed += appointmentEditor_Closed;
#if SILVERLIGHT
                            appointmentEditor.Show();
#else
                            appointmentEditor.Owner = this.FindParentElementOfType<Window>();
                            appointmentEditor.Save.Click += Save_Click;
                            appointmentEditor.Delete.Click += Delete_Click;
                            appointmentEditor.ShowDialog();
#endif
                        }
                    }
                }
            }
            if(editpopup != null)
                editpopup.IsOpen = false;
        }

        internal void ResizeAppointment()
        {
            if (!AppointmentInResizeMode)
                EnableDragDrop();
            AppointmentInResizeMode = true;
            editpopup.IsOpen = false;
            contextmenupopup.IsOpen = false;
            AddnewContextmenuPopup.IsOpen = false;
            if (SelectedAppointment != null)
            {
                SelectedAppointment.AppointmentSelectionBrush = AppointmentSelectionBrush;
            }
        }

        internal void DeleteAppointment()
        {
            if (SelectedAppointment == null)
                return;
            if (appointmentEditor != null)
                SelectedAppointment.IsRecursiveModified = appointmentEditor.IsRecursiveModified;
            if (SelectedAppointment.IsRecursive)
            {
                if (Appointments.Contains(SelectedAppointment))
                {
                    Appointments.Remove(SelectedAppointment);
                }
                else
                {
                    RemoveRecursiveAppointment(SelectedAppointment);
                }
            }
            else
            {
                Appointments.Remove(SelectedAppointment);
                SelectedAppointment = null;
            }
        }

        internal void CopyAppointment()
        {
            if (SelectedAppointment != null)
            {
                CopiedAppointment = (ScheduleAppointment)AppointmentCloning(SelectedAppointment);
                CopiedAppointment.ObjectID = SelectedAppointment.GetHashCode();
            }
            if (editpopup != null)
                editpopup.IsOpen = false;
        }

        internal void PasteAppointment()
        {
            if (CopiedAppointment != null)
            {
                var app = (ScheduleAppointment)AppointmentCloning(CopiedAppointment);
                if (SelectedAppointment != null)
                {
                    SelectedAppointment.IsSelected = false;
                    SelectedAppointment.AppointmentSelectionBrush = new SolidColorBrush(Colors.Transparent);
                }
                DateTime date = Currentselecteddate;
                DateTime startime = CopiedAppointment.StartTime;
                DateTime endtime = CopiedAppointment.EndTime;

                if (app.ResourceCollection.Count == 0)
                {
                    foreach (Resource selectedresource in selectedResourcename)
                    {
                        app.ResourceCollection.Add(selectedresource);
                    }
                }
                else if (app.ResourceCollection.FirstOrDefault(res => res.TypeName == Resource) != null)
                {
                    foreach (Resource selectedresource in selectedResourcename)
                    {
                        var firstOrDefault = app.ResourceCollection.FirstOrDefault(res => res.TypeName == selectedresource.TypeName);
                        if (firstOrDefault != null)
                            firstOrDefault.ResourceName = selectedresource.ResourceName;
                    }
                }
                else
                {
                    foreach (Resource selectedresource in selectedResourcename)
                    {
                        app.ResourceCollection.Add(selectedresource);
                    }
                }
                if (ScheduleType == ScheduleType.Month)
                {
                    if (startime == endtime)
                    {
                        app.AllDay = true;
                    }
                    app.StartTime = Currentselecteddate.AddMinutes(startime.Minute);
                    app.EndTime = Currentselecteddate.Add(endtime - startime);
                }
                else if (allDayTouch)
                {
                    allDayTouch = false;
                    app.AllDay = true;
                    app.StartTime = date;
                    app.EndTime = date;
                }
                else
                {
                    app.StartTime = date;
                    if (endtime != startime)
                    {
                        app.EndTime = date.Add(endtime - startime);
                    }
                    else
                    {
                        app.EndTime = date.Add(GetTimeInterval());
                    }
                }
                SelectedAppointment = app;
                app.IsSelected = true;
                app.AppointmentSelectionBrush = AppointmentSelectionBrush;
                Appointments.Add(app);
            }
        }

        void SaveAppointment(AppointmentEditorClosedEventArgs closedEventArgs)
        {
            if (appointmentEditor.InternalStartTime < appointmentEditor.InternalEndTime || (appointmentEditor.AllDay && appointmentEditor.InternalStartTime <= appointmentEditor.InternalEndTime))
            {
                var rp = new RecurrenceProperties();
                if (SelectedAppointment != null)
                {
                    #region EditAppointment

                    var clonedAppointment = CloneSelectedAppointment() as ScheduleAppointment;
                    EditSelectedAppointment(clonedAppointment, rp);
                    closedEventArgs.EditedAppointment = clonedAppointment;
                    GetAppointmentWindowClosedEvents(closedEventArgs);
                    if (!closedEventArgs.Cancel)
                        EditSelectedAppointment(SelectedAppointment, rp);

                    #endregion
                }
                else
                {
                    #region NewAppointment
                    var NewAppointment = new ScheduleAppointment
                    {
                        StartTime = appointmentEditor.start.Date.Add(appointmentEditor.start1.TimeOfDay),
                        EndTime = appointmentEditor.end.Date.Add(appointmentEditor.end1.TimeOfDay),
                        Status = AppointmentStatusCollection.First(appointmentStatus => appointmentStatus.Status.Equals(appointmentEditor.Status.SelectedValue.ToString())),
                        Notes = appointmentEditor.Notes.Text,
                        Subject = appointmentEditor.Subject.Text,
                        AllDay = appointmentEditor.allDay.IsChecked != null && ((bool)(appointmentEditor.allDay.IsChecked)),
                        Location = appointmentEditor.Location.Text,
                        ReadOnly = appointmentEditor.readOnly.IsChecked != null && (bool)appointmentEditor.readOnly.IsChecked,
                    };
                    if (appointmentEditor.Starttimezone.SelectedItem != null)
                    {
                        NewAppointment.StartTimeZone = (appointmentEditor.Starttimezone.SelectedItem as TimeZone);
                    }
                    if (appointmentEditor.Endtimezone.SelectedItem != null)
                    {
                        NewAppointment.EndTimeZone = (appointmentEditor.Endtimezone.SelectedItem as TimeZone);
                    }

                    foreach (Resource selectedresrc in appointmentEditor.AppointmentResourceName)
                    {
                        NewAppointment.ResourceCollection.Add(selectedresrc);
                    }
                    if (appointmentEditor.Reminder.SelectedValue != null)
                        NewAppointment.ReminderTime = (ReminderTimeType)appointmentEditor.Reminder.SelectedValue;
                    if (NewAppointment.ReminderTime == ReminderTimeType.None)
                    {
                        NewAppointment.ReminderDeliveryTime = null;
                    }
                    else
                    {
                        NewAppointment.ReminderDeliveryTime = NewAppointment.InternalStartTime - NewAppointment.MeasureTime(NewAppointment.ReminderTime);
                    }

                    #region Recurrence
                    NewAppointment.IsRecursive = appointmentEditor.IsRecursive;
                    if (NewAppointment.IsRecursive)
                    {
                        //NewAppointment.RecurrenceProperites = new RecurrenceProperties();

                        #region daily
                        if (appointmentEditor.RecurrenceDaily)
                        {
                            int everyNDays = 1;
                            double days;
                            bool intCheck = false;
                            bool isDouble = Double.TryParse((appointmentEditor.EveryDayGap.Value).ToString(), out days);
                            if (isDouble)
                                intCheck = int.TryParse((Math.Round(days)).ToString(), out everyNDays);
                            if (!intCheck)
                            {
                                string NDays = (appointmentEditor.EveryDayGap.Value).ToString();
                                rp.DailyNDays = int.Parse(NDays);
                            }
                            else
                            {
                                rp.DailyNDays = everyNDays;
                            }
                            rp.RecurrenceType = RecurrenceType.Daily;
                            if (appointmentEditor.EveryXDays.IsChecked != null)
                                rp.IsDailyEveryNDays = (bool)appointmentEditor.EveryXDays.IsChecked;
                        }
                        #endregion

                        #region weekly
                        else if (appointmentEditor.RecurrenceWeekly)
                        {
                            int everyNWeeks = 1;
                            double weeks;
                            bool intCheck = false;
                            bool isDouble = Double.TryParse((appointmentEditor.EveryWeekGap.Value).ToString(), out weeks);
                            if (isDouble)
                                intCheck = int.TryParse((Math.Round(weeks)).ToString(), out everyNWeeks);
                            if (!intCheck)
                            {
                                string NWeeks = (appointmentEditor.EveryDayGap.Value).ToString();
                                rp.WeeklyEveryNWeeks = int.Parse(NWeeks);
                            }
                            else
                            {
                                rp.WeeklyEveryNWeeks = everyNWeeks;
                            }
                            rp.RecurrenceType = RecurrenceType.Weekly;
                            //rp.WeeklyEveryNWeeks = int.TryParse((Math.Round((double)EveryWeekGap.Value)).ToString(), out everyNWeeks) ? int.Parse((Math.Round((double)EveryWeekGap.Value)).ToString()) : (int)EveryWeekGap.Value;
                            if (appointmentEditor.sunday.IsChecked != null)
                                rp.IsWeeklySunday = (bool)appointmentEditor.sunday.IsChecked;
                            if (appointmentEditor.monday.IsChecked != null)
                                rp.IsWeeklyMonday = (bool)appointmentEditor.monday.IsChecked;
                            if (appointmentEditor.tuesday.IsChecked != null)
                                rp.IsWeeklyTuesday = (bool)appointmentEditor.tuesday.IsChecked;
                            if (appointmentEditor.wednesday.IsChecked != null)
                                rp.IsWeeklyWednesday = (bool)appointmentEditor.wednesday.IsChecked;
                            if (appointmentEditor.thursday.IsChecked != null)
                                rp.IsWeeklyThursday = (bool)appointmentEditor.thursday.IsChecked;
                            if (appointmentEditor.friday.IsChecked != null)
                                rp.IsWeeklyFriday = (bool)appointmentEditor.friday.IsChecked;
                            if (appointmentEditor.saturday.IsChecked != null)
                                rp.IsWeeklySaturday = (bool)appointmentEditor.saturday.IsChecked;

                        }
                        #endregion

                        #region monthly
                        else if (appointmentEditor.RecurrenceMonthly)
                        {
                            int everyNMonths = 1;
                            double months;
                            bool intCheck = false;
                            bool isDouble = Double.TryParse((appointmentEditor.EveryMonthGap.Value).ToString(), out months);
                            if (isDouble)
                                intCheck = int.TryParse((Math.Round(months)).ToString(), out everyNMonths);
                            if (!intCheck)
                            {
                                string NMonths = (appointmentEditor.EveryDayGap.Value).ToString();
                                rp.MonthlyEveryNMonths = int.Parse(NMonths);
                            }
                            else
                            {
                                rp.MonthlyEveryNMonths = everyNMonths;
                            }
                            rp.RecurrenceType = RecurrenceType.Monthly;
                            if (appointmentEditor.EveryXMonths.IsChecked != null)
                                rp.IsMonthlySpecific = (bool)appointmentEditor.EveryXMonths.IsChecked;
                            if (rp.IsMonthlySpecific)
                            {
                                rp.MonthlySpecificMonthDay = appointmentEditor.EveryMonthDay.SelectedIndex + 1;
                            }
                            else
                            {
                                rp.MonthlyNthWeek = appointmentEditor.Howmanyth.SelectedIndex + 1;
                                rp.MonthlyWeekDay = appointmentEditor.Weekday.SelectedIndex;
                            }
                        }
                        #endregion

                        #region yearly
                        else if (appointmentEditor.RecurrenceYearly)
                        {
                            int everyNYears = 1;
                            double years;
                            bool intCheck = false;
                            bool isDouble = Double.TryParse((appointmentEditor.EveryYearGap.Value).ToString(), out years);
                            if (isDouble)
                                intCheck = int.TryParse((Math.Round(years)).ToString(), out everyNYears);
                            if (!intCheck)
                            {
                                string NYears = (appointmentEditor.EveryDayGap.Value).ToString();
                                rp.YearlyEveryNYears = int.Parse(NYears);
                            }
                            else
                            {
                                rp.YearlyEveryNYears = everyNYears;
                            }
                            rp.RecurrenceType = RecurrenceType.Yearly;
                            if (appointmentEditor.SpecificDate.IsChecked != null)
                                rp.IsYearlySpecific = (bool)appointmentEditor.SpecificDate.IsChecked;
                            if (rp.IsYearlySpecific)
                            {
                                rp.YearlySpecificMonth = appointmentEditor.YrSpMonth.SelectedIndex + 1;
                                rp.YearlySpecificMonthDay = appointmentEditor.YrSpDate.SelectedIndex + 1;
                            }
                            else
                            {
                                rp.YearlyNthWeek = appointmentEditor.YrHowmanyth.SelectedIndex + 1;
                                rp.YearlyWeekDay = appointmentEditor.YrWeekday.SelectedIndex + 1;
                                rp.YearlyGenericMonth = appointmentEditor.YrMonth.SelectedIndex + 1;
                            }
                        }
                        #endregion

                        #region RecurrenceRange
                        rp.RangeStartDate = (DateTime)appointmentEditor.RangeStartdate.SelectedDate;
                        rp.IsRangeRecurrenceCount = (bool)appointmentEditor.EndAfter.IsChecked;
                        if (rp.IsRangeRecurrenceCount)
                        {
                            int recCount = 1;
                            double count;
                            bool intCheck = false;
                            bool isDouble = Double.TryParse((appointmentEditor.EndAfterCount.Value).ToString(), out count);
                            if (isDouble)
                                intCheck = int.TryParse((Math.Round(count)).ToString(), out recCount);
                            if (!intCheck)
                            {
                                string strCount = (appointmentEditor.EveryDayGap.Value).ToString();
                                rp.RangeRecurrenceCount = int.Parse(strCount);
                            }
                            else
                            {
                                rp.RangeRecurrenceCount = recCount;
                            }
                        }
                        rp.IsRangeEndDate = (bool)appointmentEditor.EndBy.IsChecked;
                        if (rp.IsRangeEndDate)
                        {
                            rp.RangeEndDate = (DateTime)appointmentEditor.RangeEnddate.SelectedDate;
                        }
                        rp.IsRangeNoEndDate = (bool)appointmentEditor.NoEndDate.IsChecked;
                        IsRRuleSetInternally = true;
                        NewAppointment.hasInternalRule = true;
                        NewAppointment.RecurrenceRule = ScheduleHelper.RRuleGenerator(rp, NewAppointment.StartTime, NewAppointment.EndTime);
                        NewAppointment.hasInternalRule = false;
                        IsRRuleSetInternally = false;
                        NewAppointment.RecurrenceProperites = rp;
                        #endregion
                    }

                    #endregion
                    if (Appointments == null)
                    {
                        Appointments = new ScheduleAppointmentCollection();
                    }

                    closedEventArgs.EditedAppointment = NewAppointment;
                    GetAppointmentWindowClosedEvents(closedEventArgs);
                    if (!closedEventArgs.Cancel)
                        Appointments.Add(NewAppointment);
                    #endregion
                }
            }
        }

        private void EditSelectedAppointment(ScheduleAppointment selectedAppointment, RecurrenceProperties rp)
        {
            //Checking weather the new value is not equal to old endtime value
            if (appointmentEditor.end.Date.Add(appointmentEditor.end1.TimeOfDay) != selectedAppointment.EndTime)
            {
                stopUpdate = true;
            }
            selectedAppointment.StartTime = appointmentEditor.start.Date.Add(appointmentEditor.start1.TimeOfDay);
            stopUpdate = false;
            selectedAppointment.EndTime = appointmentEditor.end.Date.Add(appointmentEditor.end1.TimeOfDay);
            selectedAppointment.Status = AppointmentStatusCollection.First(appointmentStatus => appointmentStatus.Status.Equals(appointmentEditor.Status.SelectedValue.ToString()));
            selectedAppointment.Notes = appointmentEditor.Notes.Text;
            selectedAppointment.Subject = appointmentEditor.Subject.Text;
            selectedAppointment.AllDay = appointmentEditor.allDay.IsChecked != null && ((bool)(appointmentEditor.allDay.IsChecked));
            selectedAppointment.Location = appointmentEditor.Location.Text;
            if (appointmentEditor.readOnly.IsChecked != null)
                selectedAppointment.ReadOnly = (bool)appointmentEditor.readOnly.IsChecked;
            selectedAppointment.IsRecursive = appointmentEditor.IsRecursive;
            selectedAppointment.IsRecursiveModified = appointmentEditor.IsRecursiveModified;
            if (appointmentEditor.Starttimezone.SelectedItem != null)
            {
                selectedAppointment.StartTimeZone = (appointmentEditor.Starttimezone.SelectedItem as TimeZone);
            }
            if (appointmentEditor.Endtimezone.SelectedItem != null)
            {
                selectedAppointment.EndTimeZone = (appointmentEditor.Endtimezone.SelectedItem as TimeZone);
            }
            if (appointmentEditor.Reminder.SelectedValue != null)
                selectedAppointment.ReminderTime = (ReminderTimeType)appointmentEditor.Reminder.SelectedValue;
            if (selectedAppointment.ReminderTime == ReminderTimeType.None)
            {
                selectedAppointment.ReminderDeliveryTime = null;
            }
            else
            {
                selectedAppointment.ReminderDeliveryTime = selectedAppointment.InternalStartTime - selectedAppointment.MeasureTime(selectedAppointment.ReminderTime);
            }

            #region Recurrence
            if (appointmentEditor.IsRecursive)
            {
                #region Daily
                if (appointmentEditor.RecurrenceDaily)
                {
                    int everyNDays = 1;
                    double days;
                    bool intCheck = false;
                    bool isDouble = Double.TryParse((appointmentEditor.EveryDayGap.Value).ToString(), out days);
                    if (isDouble)
                        intCheck = int.TryParse((Math.Round(days)).ToString(), out everyNDays);
                    if (!intCheck)
                    {
                        string NDays = (appointmentEditor.EveryDayGap.Value).ToString();
                        rp.DailyNDays = int.Parse(NDays);
                    }
                    else
                    {
                        rp.DailyNDays = everyNDays;
                    }
                    rp.RecurrenceType = RecurrenceType.Daily;
                    if (appointmentEditor.EveryXDays.IsChecked != null)
                        rp.IsDailyEveryNDays = (bool)appointmentEditor.EveryXDays.IsChecked;
                }
                #endregion

                #region Weekly
                else if (appointmentEditor.RecurrenceWeekly)
                {
                    int everyNWeeks = 1;
                    double weeks;
                    bool intCheck = false;
                    bool isDouble = Double.TryParse((appointmentEditor.EveryWeekGap.Value).ToString(), out weeks);
                    if (isDouble)
                        intCheck = int.TryParse((Math.Round(weeks)).ToString(), out everyNWeeks);
                    if (!intCheck)
                    {
                        string NWeeks = (appointmentEditor.EveryDayGap.Value).ToString();
                        rp.WeeklyEveryNWeeks = int.Parse(NWeeks);
                    }
                    else
                    {
                        rp.WeeklyEveryNWeeks = everyNWeeks;
                    }
                    rp.RecurrenceType = RecurrenceType.Weekly;
                    if (appointmentEditor.sunday.IsChecked != null)
                        rp.IsWeeklySunday = (bool)appointmentEditor.sunday.IsChecked;
                    if (appointmentEditor.monday.IsChecked != null)
                        rp.IsWeeklyMonday = (bool)appointmentEditor.monday.IsChecked;
                    if (appointmentEditor.tuesday.IsChecked != null)
                        rp.IsWeeklyTuesday = (bool)appointmentEditor.tuesday.IsChecked;
                    if (appointmentEditor.wednesday.IsChecked != null)
                        rp.IsWeeklyWednesday = (bool)appointmentEditor.wednesday.IsChecked;
                    if (appointmentEditor.thursday.IsChecked != null)
                        rp.IsWeeklyThursday = (bool)appointmentEditor.thursday.IsChecked;
                    if (appointmentEditor.friday.IsChecked != null)
                        rp.IsWeeklyFriday = (bool)appointmentEditor.friday.IsChecked;
                    if (appointmentEditor.saturday.IsChecked != null)
                        rp.IsWeeklySaturday = (bool)appointmentEditor.saturday.IsChecked;

                }
                #endregion

                #region Monthly
                else if (appointmentEditor.RecurrenceMonthly)
                {
                    int everyNMonths = 1;
                    double months;
                    bool intCheck = false;
                    bool isDouble = Double.TryParse((appointmentEditor.EveryMonthGap.Value).ToString(), out months);
                    if (isDouble)
                        intCheck = int.TryParse((Math.Round(months)).ToString(), out everyNMonths);
                    if (!intCheck)
                    {
                        string NMonths = (appointmentEditor.EveryDayGap.Value).ToString();
                        rp.MonthlyEveryNMonths = int.Parse(NMonths);
                    }
                    else
                    {
                        rp.MonthlyEveryNMonths = everyNMonths;
                    }
                    rp.RecurrenceType = RecurrenceType.Monthly;
                    if (appointmentEditor.EveryXMonths.IsChecked != null)
                        rp.IsMonthlySpecific = (bool)appointmentEditor.EveryXMonths.IsChecked;
                    if (rp.IsMonthlySpecific)
                    {
                        rp.MonthlySpecificMonthDay = appointmentEditor.EveryMonthDay.SelectedIndex + 1;
                    }
                    else
                    {
                        rp.MonthlyNthWeek = appointmentEditor.Howmanyth.SelectedIndex + 1;
                        rp.MonthlyWeekDay = appointmentEditor.Weekday.SelectedIndex;
                    }
                }
                #endregion

                #region Yearly
                else if (appointmentEditor.RecurrenceYearly)
                {
                    int everyNYears = 1;
                    double years;
                    bool intCheck = false;
#if SILVERLIGHT
                    bool isDouble = Double.TryParse((appointmentEditor.EveryYearGap.Value).ToString(), out years);
#else
                            bool isDouble = Double.TryParse((appointmentEditor.EveryYearGap.Value).ToString(), out years);
#endif
                    if (isDouble)
                        intCheck = int.TryParse((Math.Round(years)).ToString(), out everyNYears);
                    if (!intCheck)
                    {
                        string NYears = (appointmentEditor.EveryDayGap.Value).ToString();
                        rp.YearlyEveryNYears = int.Parse(NYears);
                    }
                    else
                    {
                        rp.YearlyEveryNYears = everyNYears;
                    }
                    rp.RecurrenceType = RecurrenceType.Yearly;
                    if (appointmentEditor.SpecificDate.IsChecked != null)
                        rp.IsYearlySpecific = (bool)appointmentEditor.SpecificDate.IsChecked;
                    if (rp.IsYearlySpecific)
                    {
                        rp.YearlySpecificMonth = appointmentEditor.YrSpMonth.SelectedIndex + 1;
                        rp.YearlySpecificMonthDay = appointmentEditor.YrSpDate.SelectedIndex + 1;
                    }
                    else
                    {
                        rp.YearlyNthWeek = appointmentEditor.YrHowmanyth.SelectedIndex + 1;
                        rp.YearlyWeekDay = appointmentEditor.YrWeekday.SelectedIndex + 1;
                        rp.YearlyGenericMonth = appointmentEditor.YrMonth.SelectedIndex + 1;
                    }
                }
                #endregion

                rp.RangeStartDate = (DateTime)appointmentEditor.RangeStartdate.SelectedDate;
                rp.IsRangeRecurrenceCount = (bool)appointmentEditor.EndAfter.IsChecked;
                if (rp.IsRangeRecurrenceCount)
                {
                    int recCount = 1;
                    double count;
                    bool intCheck = false;
                    bool isDouble = Double.TryParse((appointmentEditor.EndAfterCount.Value).ToString(), out count);
                    if (isDouble)
                        intCheck = int.TryParse((Math.Round(count)).ToString(), out recCount);
                    if (!intCheck)
                    {
                        string strCount = (appointmentEditor.EveryDayGap.Value).ToString();
                        rp.RangeRecurrenceCount = int.Parse(strCount);
                    }
                    else
                    {
                        rp.RangeRecurrenceCount = recCount;
                    }
                }
                rp.IsRangeEndDate = (bool)appointmentEditor.EndBy.IsChecked;
                if (rp.IsRangeEndDate)
                {
                    rp.RangeEndDate = (DateTime)appointmentEditor.RangeEnddate.SelectedDate;
                }
                else
                {
                    rp.canUpdateOtherProperty = false;
                    rp.RangeEndDate = rp.RangeStartDate.AddDays(1);
                    rp.canUpdateOtherProperty = true;
                }
                rp.IsRangeNoEndDate = (bool)appointmentEditor.NoEndDate.IsChecked;
                selectedAppointment.IsRecursive = appointmentEditor.IsRecursive;
                IsRRuleSetInternally = true;
                selectedAppointment.hasInternalRule = true;
                selectedAppointment.RecurrenceRule = ScheduleHelper.RRuleGenerator(rp, selectedAppointment.StartTime, selectedAppointment.EndTime);
                selectedAppointment.hasInternalRule = false;
                IsRRuleSetInternally = false;
                selectedAppointment.RecurrenceProperites = rp;

            }
            else
            {
                RecurrenceProperties newRP = new RecurrenceProperties();
                newRP.RangeStartDate = selectedAppointment.InternalStartTime.Date;
                newRP.RangeEndDate = newRP.RangeStartDate.Date.AddDays(1);
                selectedAppointment.RecurrenceProperites = newRP;
            }
            #endregion
        }

        void OpenSeries()
        {
            if (AllowEditing)
            {
                foreach (ScheduleAppointment app in RecursiveAppointments)
                {
                    if (!app.RecurrenceID.Equals(RecAppointment.RecurrenceID)) continue;
                    {
                        appointmentEditor = new AppointmentEditor();
                        appointmentEditor.recursiveModified = 0;
                        SelectedAppointment = app;
                        appointmentEditor.UpdateAppointmentProperties(this, app);
                        appointmentEditor.Closed += appointmentEditor_Closed;
#if SILVERLIGHT
                        appointmentEditor.Show();
#else
                        appointmentEditor.Owner = this.FindParentElementOfType<Window>();
                        appointmentEditor.Save.Click += Save_Click;
                        appointmentEditor.Delete.Click += Delete_Click;
                        appointmentEditor.ShowDialog();
#endif
                    }
                    break;
                }
            }
        }

        void OpenOne()
        {
            if (AllowEditing)
            {
                appointmentEditor = new AppointmentEditor();
                SelectedAppointment = RecAppointment;
                appointmentEditor.recursiveModified = 1;
                SelectedAppointment = RecAppointment;
                appointmentEditor.UpdateAppointmentProperties(this, RecAppointment);
                appointmentEditor.Closed += appointmentEditor_Closed;
#if SILVERLIGHT
                
                appointmentEditor.Show();
#else
                appointmentEditor.Owner = this.FindParentElementOfType<Window>();
                appointmentEditor.Save.Click += Save_Click;
                appointmentEditor.Delete.Click += Delete_Click;
                appointmentEditor.ShowDialog();
#endif
            }
        }

        #endregion

        #region Appointment Cloning

        internal object AppointmentCloning(ScheduleAppointment app)
        {
            Type t = app.GetType();
            var newapp = Activator.CreateInstance(t);

            foreach (PropertyInfo pro in app.GetType().GetProperties())
            {
                if (app.GetType().GetProperty(pro.Name) != null && (pro.DeclaringType == t || pro.DeclaringType == typeof(ScheduleAppointment)))
                {
                    if ((pro.Name != "ResourceCollection" && pro.Name != "AllDay") || (string.IsNullOrEmpty(Resource) && pro.Name == "ResourceCollection"))
                    {
                        var value = app.GetType().GetProperty(pro.Name).GetValue(app, new object[0]);
                        newapp.GetType().GetProperty(pro.Name).SetValue(newapp, value, new object[0]);
                    }
                }
            }
            return newapp;
        }

        internal object CloneSelectedAppointment()
        {
            if (SelectedAppointment != null)
            {
                Type t = SelectedAppointment.GetType();
                var newapp = Activator.CreateInstance(t);

                foreach (PropertyInfo pro in SelectedAppointment.GetType().GetProperties())
                {
                    if (SelectedAppointment.GetType().GetProperty(pro.Name) != null && (pro.DeclaringType == t || pro.DeclaringType == typeof(ScheduleAppointment)))
                    {
                        var value = SelectedAppointment.GetType().GetProperty(pro.Name).GetValue(SelectedAppointment, new object[0]);
                        newapp.GetType().GetProperty(pro.Name).SetValue(newapp, value, new object[0]);
                    }
                }
                return newapp;
            }
            return null;
        }

        #endregion

        #region Finding Intersected Appointment

        internal double GetInterSectedIndexValue(List<ScheduleAppointment> appointments, List<ScheduleAppointment> intersected, ScheduleAppointment app)
        {
            if (appointments == null)
                return 0;


            var appOrdered = (from res in appointments
                              orderby res.InternalStartTime
                              select res).ToList();

            var intrlist = (from a in appOrdered         
                            from i in intersected
                            where i.Equals(a)
                            orderby i.Equals(a)
                            select i).ToList();

            if (intrlist.Any()) return intrlist.IndexOf(app);

            return 0;
        }

        internal double GetInterSectedCountValue(List<ScheduleAppointment> intersected, ScheduleAppointment currentapp)
        {
            var intersectednewlist = new List<ScheduleAppointment>();

            if (Resource != string.Empty && ScheduleResourceType != null)
            {
                Resource currentAppRes = currentapp.ResourceCollection.FirstOrDefault(res => res.TypeName == Resource);
                if (currentAppRes != null)
                {
                    intersectednewlist.AddRange(from item in intersected where item != null let itemresource = item.ResourceCollection.FirstOrDefault(res => res.TypeName == Resource) where itemresource != null && itemresource.ResourceName == currentAppRes.ResourceName where item.InternalStartTime < currentapp.InternalEndTime && currentapp.InternalStartTime < item.InternalEndTime select item);
                }
            }
            else
            {
                intersectednewlist.AddRange(intersected.Where(item => item != null).Where(item => item.InternalStartTime < currentapp.InternalEndTime && currentapp.InternalStartTime < item.InternalEndTime));
            }
            int getmaxcntval = GetMaxColVal(intersectednewlist);

            var fnlintred = from r in intersectednewlist
                            where r.InternalStartTime == currentapp.InternalStartTime
                            select r;

            var scheduleAppointments = fnlintred as IList<ScheduleAppointment> ?? fnlintred.ToList();
            if (getmaxcntval < scheduleAppointments.Count()) getmaxcntval = scheduleAppointments.Count();

            return getmaxcntval;
        }

        internal int GetMaxColVal(List<ScheduleAppointment> intersectednewlist, ScheduleAppointment currentapp)
        {
            var grpstarttime = from res in intersectednewlist
                               group res by res.InternalStartTime into p
                               select p;

            return (from item in grpstarttime
                    let firstOrDefault = item.FirstOrDefault()
                    where item != null
                    select (from res in intersectednewlist
                            where firstOrDefault != null && ((res.InternalStartTime == firstOrDefault.InternalStartTime) || (res.InternalStartTime < firstOrDefault.InternalStartTime && res.InternalEndTime > firstOrDefault.InternalStartTime))
                            select res)
                        into getmaxcnt
                        select getmaxcnt.Count()).Concat(new[] { 0 }).Max();
        }

        int GetMaxColVal(List<ScheduleAppointment> intersectednewlist)
        {
            var grpstarttime = from res in intersectednewlist
                               group res by res.InternalStartTime into p
                               select p;

            return (from item in grpstarttime
                    let firstOrDefault = item.FirstOrDefault()
                    where item != null
                    select (from res in intersectednewlist
                            where firstOrDefault != null && ((res.InternalStartTime == firstOrDefault.InternalStartTime) || (res.InternalStartTime < firstOrDefault.InternalStartTime && res.InternalEndTime > firstOrDefault.InternalStartTime))
                            select res)
                        into getmaxcnt
                        select getmaxcnt.Count()).Concat(new[] { 0 }).Max();
        }

        #endregion

        #region Enabling Drag & Drop

        internal void EnableDragDrop()
        {
            editpopup.IsOpen = false;
            addnewpopup.IsOpen = false;
            if (viewcontrol == null)
            {
                viewcontrol = currentSelectedItem as ContentControl;
            }
            if (DragDropCanvas != null)
            {
                if (DragDropCanvas.Children.Count > 0)
                    DragDropCanvas.Children.Clear();
                DragDropCanvas.Clip = new RectangleGeometry { Rect = new Rect(0, ActualHeight - currentSelectedItem.ActualHeight, currentSelectedItem.ActualWidth, currentSelectedItem.ActualHeight) };
                if (currentSelectedItem != null)
                {
                    if (currentSelectedItem.Content is ScheduleDaysView && dvc != null)
                        (currentSelectedItem.Content as ScheduleDaysView).EnableDragDrop(this);
                    else if (currentSelectedItem.Content is ScheduleMonthView && mvc != null)
                        (currentSelectedItem.Content as ScheduleMonthView).EnableDragDrop(this);
                    else if (currentSelectedItem.Content is ScheduleTimeLineView && hvc != null)
                        (currentSelectedItem.Content as ScheduleTimeLineView).EnableDragDrop(this);
                }
            }
        }

        #endregion

        #region Resetting Drag & Drop Appointment Opacity

        internal void ResetDragDropAppointmentOpacity()
        {
            if (mvc != null)
            {
                mvc.Opacity = 1;
            }
            if (dvc != null)
            {
                dvc.Opacity = 1;
            }
            if (hvc != null)
            {
                hvc.Opacity = 1;
            }
            AppointmentInResizeMode = false;
        }

        #endregion

        #region Clearing DragDropCanvas

        void ClearDragDropCanvas()
        {
            if (DragDropCanvas != null)
            {
                if (DragDropCanvas.Children.Count > 0)
                {
                    Dayviewrb.Detach();
                    Monthviewrb.Detach();
                    Timelineviewrb.Detach();
                    var control = DragDropCanvas.Children[0] as Control;
                    if (control != null)
                    {
                        control.MouseLeftButtonDown -= drag_app_MouseLeftButtonDown;
                        control.Loaded -= drag_app_Loaded;
                    }
                }
                DragDropCanvas.Children.Clear();
                ResetDragDropAppointmentOpacity();
                AppointmentInResizeMode = false;
                if (SelectedAppointment != null)
                {
                    SelectedAppointment.AppointmentSelectionBrush = new SolidColorBrush(Colors.Transparent);
                    SelectedAppointment.IsSelected = false;
                    //if (ScheduleType == ScheduleType.TimeLine) // removed for copy paste issue with touch menu
                    //    SelectedAppointment = null;
                }
            }
            IsDragEnabled = false;
        }

        #endregion

        #region Finding Height of TimeSlot

        internal double GetTimeSlotHeight()
        {
            var height = ScheduleTimeLineItemsControl.MaxValue * IntervalHeight * ScheduleTimeLineItemsControl.IntervalCount[(int)TimeInterval];
            return height;
        }

        #endregion

        #region Calculate OverAll Leaf Count

        internal int CalculateLeafCount(ResourceType restype)
        {
            int leafchild = 1;
            ResourceType tempresotype = restype;
            while (tempresotype != null)
            {
                leafchild = leafchild * tempresotype.ResourceCollection.Count;
                tempresotype = tempresotype.SubResourceType;
            }
            return leafchild;
        }

        #endregion

        #region Finding & Adding Resource

        internal List<ResourceType> FindResourceList(ResourceType type)
        {
            ResourceType Restype = type;
            var returnlist = new List<ResourceType>();
            while (Restype != null)
            {
                returnlist.Add(Restype);
                Restype = Restype.SubResourceType;
            }
            return returnlist;
        }

        internal void AddResources(ScheduleAppointment appointment)
        {
            if (appointment.ResourceCollection.Count == 0)
            {
                foreach (Resource selectedresource in selectedResourcename)
                {
                    appointment.ResourceCollection.Add(selectedresource);
                }
            }
            else if (appointment.ResourceCollection.FirstOrDefault(res => res.TypeName == Resource) != null)
            {
                foreach (Resource selectedresource in selectedResourcename)
                {
                    var firstOrDefault = appointment.ResourceCollection.FirstOrDefault(res => res.TypeName == selectedresource.TypeName);
                    if (firstOrDefault != null)
                        firstOrDefault.ResourceName = selectedresource.ResourceName;
                }
            }
            else
            {
                foreach (Resource selectedresource in selectedResourcename)
                {
                    appointment.ResourceCollection.Add(selectedresource);
                }
            }
        }

        #endregion

        #region Remove appointment selection

        void RemoveAppointmentSelection()
        {
            if (SelectedAppointment != null)
            {
                SelectedAppointment.IsSelected = false;
                SelectedAppointment.AppointmentSelectionBrush = new SolidColorBrush(Colors.Transparent);
                SelectedAppointment = null;
            }
        }

        #endregion

        #region Getting Current Selected Date

        DateTime GetCurrentSelectedDate(MouseButtonEventArgs e)
        {
            var selectedDate = new DateTime();
            if (currentSelectedItem != null)
            {
                if (currentSelectedItem.Content is ScheduleDaysView && (ScheduleType == ScheduleType.Day || ScheduleType == ScheduleType.Week || ScheduleType == ScheduleType.WorkWeek))
                {
                    if (e.OriginalSource is FrameworkElement)
                    {
                        SetAllDayFlag(e.OriginalSource as FrameworkElement, true);
                    }
                    var scheduleDaysView = (currentSelectedItem.Content as ScheduleDaysView);
                    Point position = e.GetPosition(scheduleDaysView.FindElementOfType<ScheduleHorizontalTimeSlotItemsControl>());
                    selectedDate = scheduleDaysView.GetSelectedDate(position, false);
                }
                else if (currentSelectedItem.Content is ScheduleMonthView && ScheduleType == ScheduleType.Month)
                {
                    Point position = e.GetPosition(currentSelectedItem.FindElementOfType<ScheduleMonthViewItemsControl>());
                    var scheduleMonthView = currentSelectedItem.Content as ScheduleMonthView;
                    if (scheduleMonthView != null) selectedDate = scheduleMonthView.GetSelectedDate(position, false);
                }
                else if (currentSelectedItem.Content is ScheduleTimeLineView && ScheduleType == ScheduleType.TimeLine)
                {
                    var scheduleTimeLineView = currentSelectedItem.Content as ScheduleTimeLineView;
                    Point position = e.GetPosition(scheduleTimeLineView.FindElementOfType<ScheduleHorizontalTimeSlotControl>());
                    selectedDate = scheduleTimeLineView.GetSelectedDate(position, false);
                }
            }
            return selectedDate;
        }

        #endregion

        #region Getting Current Drop Location

        internal DateTime GetCurrentDropLocationForMouseButtonEventArgs(object sender, MouseButtonEventArgs e)
        {
            var selectedDate = new DateTime();
            if (sender is ContentControl)
            {
                var mainitem = sender as ContentControl;
                if (mainitem.Content is ScheduleDaysView && ScheduleType == ScheduleType.Day || ScheduleType == ScheduleType.Week || ScheduleType == ScheduleType.WorkWeek)
                {
                    var scheduleDayView = (mainitem.Content as ScheduleDaysView);
                    if (e.OriginalSource is FrameworkElement)
                    {
                        SetAllDayFlag(e.OriginalSource as FrameworkElement, false);
                    }
                    var positionOnApp = e.GetPosition(DragDropCanvas.Children[0] as ScheduleDaysAppointmentViewControl);
                    var positionOnTimeSlot = e.GetPosition(scheduleDayView.FindElementOfType<ScheduleHorizontalTimeSlotItemsControl>());
                    var position = new Point(positionOnTimeSlot.X, positionOnTimeSlot.Y - positionOnApp.Y);
                    if (scheduleDayView != null)
                        selectedDate = scheduleDayView.GetSelectedDate(position, true);
                    allDayFlag = positionOnTimeSlot.Y < 0;
                }
                else if (ScheduleType == ScheduleType.Month)
                {
                    var scheduleMonthView = (mainitem.Content as ScheduleMonthView);
                    var position = e.GetPosition(scheduleMonthView.FindElementOfType<ScheduleMonthViewItemsControl>());
                    if (scheduleMonthView != null)
                        selectedDate = scheduleMonthView.GetSelectedDate(position, true);
                }
                else if (ScheduleType == ScheduleType.TimeLine)
                {
                    var scheduleTimeLineView = mainitem.Content as ScheduleTimeLineView;
                    var timeslot = scheduleTimeLineView.FindElementOfType<ScheduleHorizontalTimeSlotControl>();
                    var positionOnapp = e.GetPosition(DragDropCanvas.Children[0] as ScheduleHorizontalAppointmentViewControl);
                    var positionOnTimeline = e.GetPosition(scheduleTimeLineView.FindElementOfType<ScheduleHorizontalTimeLineItemsControl>());
                    var tmslotPosition = e.GetPosition(timeslot);
                    var position = new Point((positionOnTimeline.X - positionOnapp.X), tmslotPosition.Y);
                    if (scheduleTimeLineView != null)
                        selectedDate = scheduleTimeLineView.GetSelectedDate(position, true);
                }
            }
            return selectedDate;
        }

        internal DateTime GetCurrentDropLocationForMouseEventArgs(object sender, MouseEventArgs e)
        {
            var mainitem = sender as ContentControl;
            var selectedDate = new DateTime();
            if (mainitem != null)
            {
                if (ScheduleType == ScheduleType.Month)
                {
                    var scheduleMonthView = (mainitem.Content as ScheduleMonthView);
#if WPF
                    Point Overallposition = e.GetPosition(this);
                    hitTestList = new List<DependencyObject>();
                    VisualTreeHelper.HitTest(this, null, CollectAllVisuals_Callback, new PointHitTestParameters(Overallposition));
                    hitTestList.Reverse();
                    var pointeritem = (ScheduleMonthViewItemsControl)hitTestList.FirstOrDefault(x => x.GetType() == typeof(ScheduleMonthViewItemsControl));

#else
                    Point Overallposition = e.GetPosition(this);
                    var item = VisualTreeHelper.FindElementsInHostCoordinates(Overallposition, this);
                    var pointeritem = item.FirstOrDefault(x => x.GetType() == typeof(ScheduleMonthViewItemsControl));
#endif

                    Point position = e.GetPosition(pointeritem);
                    if (scheduleMonthView != null)
                        selectedDate = scheduleMonthView.GetSelectedDate(position, true);
                }
                if ((ScheduleType == ScheduleType.Day || ScheduleType == ScheduleType.Week || ScheduleType == ScheduleType.WorkWeek))
                {
                    var scheduleDaysView = (mainitem.Content as ScheduleDaysView);
                    if (e.OriginalSource is FrameworkElement)
                    {
                        SetAllDayFlag(e.OriginalSource as FrameworkElement, false);
                    }
                    var positionOnApp = e.GetPosition(DragDropCanvas.Children[0] as ScheduleDaysAppointmentViewControl);
                    var positionOnTimeSlot = e.GetPosition(scheduleDaysView.FindElementOfType<ScheduleHorizontalTimeSlotItemsControl>());
                    var position = new Point(positionOnTimeSlot.X, positionOnTimeSlot.Y - positionOnApp.Y);
                    if (scheduleDaysView != null)
                        selectedDate = scheduleDaysView.GetSelectedDate(position, true);
                    allDayFlag = positionOnTimeSlot.Y < 0;
                }
                if (ScheduleType == ScheduleType.TimeLine)
                {
                    var scheduleTimeLineView = mainitem.Content as ScheduleTimeLineView;
                    var timeslot = scheduleTimeLineView.FindElementOfType<ScheduleHorizontalTimeSlotControl>();
                    var positionOnapp = e.GetPosition(DragDropCanvas.Children[0] as ScheduleHorizontalAppointmentViewControl);
                    var positionOnTimeline = e.GetPosition(scheduleTimeLineView.FindElementOfType<ScheduleHorizontalTimeLineItemsControl>());
                    var tmslotPosition = e.GetPosition(timeslot);
                    var position = new Point((positionOnTimeline.X - positionOnapp.X), tmslotPosition.Y);
                    if (scheduleTimeLineView != null)
                        selectedDate = scheduleTimeLineView.GetSelectedDate(position, true);
                }
            }
            return selectedDate;
        }

        internal DateTime GetDayViewCurrentTime(ScheduleHorizontalTimeSlotItemsControl timeSlot, MouseEventArgs e)
        {
            var positionOnapp = e.GetPosition(DragDropCanvas.Children[0] as ScheduleDaysAppointmentViewControl);
            var position = e.GetPosition(timeSlot);
            var returnMin = new DateTime();
            double hrHeight = timeSlot.ActualHeight / 24.0;
            double total = ((position.Y - positionOnapp.Y) / hrHeight);
            double time1 = (int)total;
            var timespan = (int)GetTimeInterval().TotalMinutes;
            double decimalvalue = (total - (int)total) * 60;
            double min = 0;
            if (timespan != 0)
            {
                min = (int)decimalvalue >= 0 ? (int)decimalvalue : 0;
            }
            return returnMin.AddHours(time1 >= 0 ? time1 : 0).AddMinutes(min);
        }

        internal DateTime GetTimeLineViewCurrentTime(ScheduleHorizontalTimeSlotControl horTimeSlot, MouseEventArgs e)
        {
            Point positionOnapp = e.GetPosition(DragDropCanvas.Children[0] as ScheduleHorizontalAppointmentViewControl);
            Point position = e.GetPosition(horTimeSlot);
            var returnMin = new DateTime();
            int min = 0;
            double hourHeight = horTimeSlot.ActualWidth * 24;
            double total = (((position.X - positionOnapp.X) / hourHeight) * 24) % 24;
            total = total < 0 ? 0 : total;
            double time = Math.Floor(total);
            if (time >= 0)
            {
                var timespan = (int)GetTimeInterval().TotalMinutes;
                double decimalvalue = (total - (int)total) * 60;
                if (timespan != 0)
                    min = (int)decimalvalue;
            }
            return returnMin.AddHours(time >= 0 ? time : 0).AddMinutes(min);
        }

        #endregion

        #region Getting Date for Corresponding Point

        internal DateTime GetDate(Point position)
        {
            var mainitem = viewcontrol;
            var selectedDate = new DateTime();
            if (mainitem != null)
            {
                switch (ScheduleType)
                {                    
                    case ScheduleType.Day:
                    case ScheduleType.WorkWeek:
                    case ScheduleType.Week:
                        if (mainitem.Content is ScheduleDaysView)
                        {
                            selectedDate = (mainitem.Content as ScheduleDaysView).GetSelectedDate(position, false);
                        }
                        break;
                    case ScheduleType.Month:
                        if (mainitem.Content is ScheduleMonthView)
                        {
                            selectedDate = (mainitem.Content as ScheduleMonthView).GetSelectedDate(position, false);
                        }
                        break;
                    case ScheduleType.TimeLine:
                        if (mainitem.Content is ScheduleTimeLineView)
                        {
                            selectedDate = (mainitem.Content as ScheduleTimeLineView).GetSelectedDate(position, false);
                        }
                        break;
                }
            }
            return selectedDate;
        }

        #endregion

        #region Getting Time Interval

        internal TimeSpan GetTimeInterval()
        {
            if (TimeInterval == TimeInterval.FifteenMin)
                return new TimeSpan(0, 15, 0);
            if (TimeInterval == TimeInterval.FiveMin)
                return new TimeSpan(0, 5, 0);
            if (TimeInterval == TimeInterval.OneHour)
                return new TimeSpan(1, 0, 0);
            if (TimeInterval == TimeInterval.SixMin)
                return new TimeSpan(0, 6, 0);
            if (TimeInterval == TimeInterval.TenMin)
                return new TimeSpan(0, 10, 0);
            if (TimeInterval == TimeInterval.ThirtyMin)
                return new TimeSpan(0, 30, 0);
            if (TimeInterval == TimeInterval.TwentyMin)
                return new TimeSpan(0, 20, 0);
            return new TimeSpan(0, 0, 0);
        }

        #endregion

        #region Finding Content Control

        FrameworkElement FindContentControl(FrameworkElement element)
        {
            var obj = element;
            string borderName = ((ScheduleType == ScheduleType.Day || ScheduleType == ScheduleType.Week || ScheduleType == ScheduleType.WorkWeek) ? "PART_DayAppointmentBorder" : (ScheduleType == ScheduleType.Month ? "PART_MonthAppointmentBorder" : "PART_TimeLineAppointmentBorder"));
            while (obj != null && obj.Name != borderName)
            {
                obj = (FrameworkElement)VisualTreeHelper.GetParent(obj);
            }
            if (obj != null && obj.Name == borderName)
            {
                return obj;
            }
            return element;
        }

        #endregion

        #region Setting All Day Flag

        private void SetAllDayFlag(FrameworkElement frameworkElement, bool setAllDayTouch)
        {
            var allDayPanel = frameworkElement.FindParentElementOfType<ScheduleAllDaysAppointmentItemsControl>();
            var headerPanel = frameworkElement.FindParentElementOfType<ScheduleDaysHeaderViewItemsControl>();
            var resourcePanel = frameworkElement.FindParentElementOfType<ResourceHeaderItemsControl>();
            allDayFlag = (allDayPanel != null || headerPanel != null || resourcePanel != null || 
                (frameworkElement is TextBlock && (frameworkElement as TextBlock).Text.Equals("All Day")));
            if (setAllDayTouch)
            {
                allDayTouch = (allDayPanel != null || headerPanel != null || resourcePanel != null ||
                    (frameworkElement is TextBlock && (frameworkElement as TextBlock).Text.Equals("All Day")));
            }
        }

        #endregion

        #region Getting Context Menu Icons

        object GetIcon(string imageName)
        {
#if SILVERLIGHT
            var image = new BitmapImage
            {
                UriSource = new Uri("/Syncfusion.SfSchedule.Silverlight;component/Assets/" + imageName, UriKind.Relative)
            };
            return image;
#else
            var image = new Image
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/Syncfusion.SfSchedule.WPF;component//Assets/" + imageName, UriKind.RelativeOrAbsolute))
            };
            return image;
#endif
        }

#if SILVERLIGHT
        private object GetContextMenuItemContent(string Header, string ImageName)
        {
            var menuItemContent = new StackPanel { Orientation = Orientation.Horizontal };
            menuItemContent.Children.Add(new Image { Source = (ImageSource)GetIcon(ImageName) });
            menuItemContent.Children.Add(new TextBlock { Text = Header, Margin = new Thickness(7, 0, 0, 0), VerticalAlignment = VerticalAlignment.Center });
            return menuItemContent;
        }
#else
        HitTestResultBehavior MyHitTestResult(HitTestResult result)
        {
            // Add the hit test result to the list that will be processed after the enumeration.
            hitResultsList.Add(result.VisualHit);
            // Set the behavior to return visuals at all z-order levels. 
            return HitTestResultBehavior.Continue;
        }

        internal HitTestResultBehavior CollectAllVisuals_Callback(HitTestResult result)
        {
            //This Is the HitTestResultCallBack method which returns the list of HitTest elements under Mouse Pointer.
            if (result == null || result.VisualHit == null)
                return HitTestResultBehavior.Stop;
            hitTestList.Add(result.VisualHit);
            return HitTestResultBehavior.Continue;
        }
#endif

        #endregion

        #region Set Date Collection For DateRange

        // Method to create a collection of dates that helps to generate CurrentSelectedDates, PrevSelectedDates, NextSelectedDates,
        // when SheduleDateRage has been set.

        internal void DateRangeDateCollection(DateTime date)
        {
            ObservableCollection<DateTime> tempDates = new ObservableCollection<DateTime>();
            ObservableCollection<DateTime> minDates = new ObservableCollection<DateTime>();
            ObservableCollection<DateTime> maxDates = new ObservableCollection<DateTime>();

            if (SelectedDates.Contains(date))
            {
                tempDates = SelectedDates;
            }
            else
            {
                if (CurrentVisibleSelectedDates.Contains(date))
                {
                    tempDates = CurrentVisibleSelectedDates;
                }
                else if (PrevSelectedDates.Contains(date))
                {
                    tempDates = PrevSelectedDates;
                }
                else if (NextSelectedDates.Contains(date))
                {
                    tempDates = NextSelectedDates;
                }
                else if(PrevSelectedDates.Count > 0 && CurrentVisibleSelectedDates.Count > 0 && NextSelectedDates.Count > 0)
                {
                    if (PrevSelectedDates[0].Date < CurrentVisibleSelectedDates[0].Date)
                    {
                        if (PrevSelectedDates[0].Date < NextSelectedDates[0].Date)
                        {
                            minDates = PrevSelectedDates;
                            if (CurrentVisibleSelectedDates[0].Date < NextSelectedDates[0].Date)
                            {
                                maxDates = NextSelectedDates;
                            }
                            else
                            {
                                maxDates = CurrentVisibleSelectedDates;
                            }
                        }
                        else
                        {
                            minDates = NextSelectedDates;
                            maxDates = CurrentVisibleSelectedDates;
                        }
                    }
                    else if (CurrentVisibleSelectedDates[0].Date < NextSelectedDates[0].Date)
                    {
                        minDates = CurrentVisibleSelectedDates;
                        if (NextSelectedDates[0].Date < PrevSelectedDates[0].Date)
                        {
                            maxDates = PrevSelectedDates;
                        }
                        else
                        {
                            maxDates = NextSelectedDates;
                        }
                    }
                    else
                    {
                        minDates = NextSelectedDates;
                        maxDates = PrevSelectedDates;
                    }
                }
                if (minDates.Count > 0 && date < minDates[0].Date)
                    tempDates = minDates;
                else if (maxDates.Count > 0 && date > maxDates[maxDates.Count - 1].Date)
                    tempDates = maxDates;

            }
            int i = tempDates.Count;
            int daysCount = (SelectedDates[SelectedDates.Count - 1].Date - SelectedDates[0].Date).Days + 1;
            DateTime currentdate = date;
            for (int j = 0; j < i; j++)
            {
                dateColl.Add(currentdate);

                DateTime tempMinDate = DateTime.MinValue.AddDays(daysCount);
                if (currentdate >= tempMinDate)
                    PrevdateColl.Add(currentdate.AddDays(-daysCount));
                else
                    exceedsMinDate = true;

                DateTime tempMaxDate = DateTime.MaxValue.AddDays(-daysCount);
                if (currentdate <= tempMaxDate)
                    NextdateColl.Add(currentdate.AddDays(daysCount));
                else
                    exceedsMaxDate = true;

                if(j+1 < i)
                {
                int intervalcount=(tempDates[j+1].Date - tempDates[j].Date).Days;
                currentdate = currentdate.AddDays(intervalcount);
                }
            }        
            tempDates = null;
            minDates = null;
            maxDates = null;
        }

        #endregion

        #endregion

        #region Public Methods

        #region Move to Previous and Next Views

        public void MoveToNextView()
        {
            if (contextmenupopup != null)
                contextmenupopup.IsOpen = false;
            if (AddnewContextmenuPopup != null)
                AddnewContextmenuPopup.IsOpen = false;
            UpdateNextItem();            
            SetNavigationTap();
            ClearDragDropCanvas();
        }

        public void MoveToPreviousView()
        {
            if (contextmenupopup != null)
                contextmenupopup.IsOpen = false;
            if (AddnewContextmenuPopup != null)
                AddnewContextmenuPopup.IsOpen = false;
            UpdatePrevItem();
            SetNavigationTap();
            ClearDragDropCanvas();
        }

        #endregion

        #region Move to Previous and Next Appointments

        public void MoveToNextAppointment()
        {
            var currentdate = new DateTime();
            switch (ScheduleType)
            {
                case ScheduleType.Week:
                case ScheduleType.WorkWeek:
                case ScheduleType.Day:
                    {
                        var grid = mainItem;
                        if (grid != null)
                        {
                            if (currentSelectedItem != null)
                            {
                                var scheduleDaysView = currentSelectedItem.Content as ScheduleDaysView;
                                if (scheduleDaysView != null)
                                    currentdate = scheduleDaysView.SelectedDates[scheduleDaysView.SelectedDates.Count - 1];
                            }
                        }
                    }
                    break;
                case ScheduleType.Month:
                    {
                        var grid = mainItem;
                        if (grid != null)
                        {
                            if (currentSelectedItem != null)
                            {
                                var scheduleMonthView = currentSelectedItem.Content as ScheduleMonthView;
                                if (scheduleMonthView != null)
                                {
                                    var filtercurrentmonth = scheduleMonthView.SelectedDates[scheduleMonthView.SelectedDates.Count / 2];
                                    currentdate = new DateTime(filtercurrentmonth.Year, filtercurrentmonth.Month, DateTime.DaysInMonth(filtercurrentmonth.Year, filtercurrentmonth.Month));
                                }
                            }
                        }
                    }
                    break;
                default:
                    {
                        Grid grid = mainItem;
                        if (grid != null)
                        {
                            if (currentSelectedItem != null)
                            {
                                var scheduleTimeLineView = currentSelectedItem.Content as ScheduleTimeLineView;
                                if (scheduleTimeLineView != null)
                                    currentdate = scheduleTimeLineView.SelectedDates[scheduleTimeLineView.SelectedDates.Count - 1];
                            }
                        }
                    }
                    break;
            }
            if (ScheduleResourceType != null && ScheduleResourceType.ResourceCollection.Count > 0)
            {
                var Filtereddays = ProxyAppointments.OrderByDescending(x => x.Key.Date).ToArray().FirstOrDefault(x => x.Key.Date < currentdate.Date && ((x.Value.FirstOrDefault(m => m.ResourceCollection.FirstOrDefault(res => res.TypeName == Resource) != null)) != null));
                if (Filtereddays.Key != new DateTime() && Filtereddays.Value != null)
                {
                    MoveToDate(Filtereddays.Key);
                }
            }
            else
            {
                var Filtereddays = ProxyAppointments.Keys.OrderBy(x => x.Date).ToArray().FirstOrDefault(x => x.Date > currentdate.Date);
                if (Filtereddays != new DateTime())
                {
                    MoveToDate(Filtereddays);
                }
            }


        }

        public void MoveToPreviousAppointment()
        {
            var currentdate = new DateTime();
            if (ScheduleType == ScheduleType.Day || ScheduleType == ScheduleType.Week || ScheduleType == ScheduleType.WorkWeek)
            {
                var grid = mainItem;
                if (grid != null)
                {
                    if (currentSelectedItem != null)
                    {
                        var scheduleDaysView = currentSelectedItem.Content as ScheduleDaysView;
                        if (scheduleDaysView != null)
                            currentdate = scheduleDaysView.SelectedDates[0];
                    }
                }
            }
            else if (ScheduleType == ScheduleType.Month)
            {
                var grid = mainItem;
                if (grid != null)
                {
                    if (currentSelectedItem != null)
                    {
                        var scheduleMonthView = currentSelectedItem.Content as ScheduleMonthView;
                        if (scheduleMonthView != null)
                        {
                            var filtercurrentmonth = scheduleMonthView.SelectedDates[scheduleMonthView.SelectedDates.Count / 2];
                            currentdate = new DateTime(filtercurrentmonth.Year, filtercurrentmonth.Month, 1);
                        }
                    }
                }
            }
            else
            {
                var grid = mainItem;
                if (grid != null)
                {
                    if (currentSelectedItem != null)
                    {
                        var scheduleTimeLineView = currentSelectedItem.Content as ScheduleTimeLineView;
                        if (scheduleTimeLineView != null)
                            currentdate = scheduleTimeLineView.SelectedDates[0];
                    }
                }
            }
            if (ScheduleResourceType != null && ScheduleResourceType.ResourceCollection.Count > 0)
            {
                var Filtereddays = ProxyAppointments.OrderByDescending(x => x.Key.Date).ToArray().FirstOrDefault(x => x.Key.Date < currentdate.Date && ((x.Value.FirstOrDefault(m => m.ResourceCollection.FirstOrDefault(res => res.TypeName == Resource) != null)) != null));
                if (Filtereddays.Key != new DateTime() && Filtereddays.Value != null)
                {
                    MoveToDate(Filtereddays.Key);
                }
            }
            else
            {
                var Filtereddays = ProxyAppointments.Keys.OrderByDescending(x => x.Date).ToArray().FirstOrDefault(x => x.Date < currentdate.Date);
                if (Filtereddays != new DateTime())
                {
                    MoveToDate(Filtereddays);
                }
            }
        }

        #endregion

        #region Move to Date

        public void MoveToDate(DateTime date)
        {
            if (SelectedDate.Date != date)
                SelectedDate = date.Add(SelectedDate.TimeOfDay);
            DateTime checkdate = CurrentSelectedDates != null ? CurrentSelectedDates[0].Date : DateTime.Now.Date;
            dateColl = new ObservableCollection<DateTime>();
            PrevdateColl = new ObservableCollection<DateTime>();
            NextdateColl = new ObservableCollection<DateTime>();
            if (ScheduleType == ScheduleType.Day)
            {
                if (ScheduleTypeToDay)
                {
                    DateRangeDateCollection(date);
                    ScheduleTypeChangedInternally = false;
                }
                else
                {
                    dateColl.Add(date);
                    PrevdateColl.Add(date.AddDays(-1));
                    NextdateColl.Add(date.AddDays(1));
                }
            }
            else if (ScheduleType == ScheduleType.Week)
            {
                var firstDate = date;
                if (DateTimeFormatInfo.CurrentInfo != null)
                {
                    var startDate = firstDate.StartOfWeek(DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek);
                    for (int i = 0; i < 7; i++)
                    {
                        dateColl.Add(startDate);
                        startDate = startDate.AddDays(1);
                    }
                }
                var startdate1 = dateColl[0].AddDays(7);
                for (int i = 0; i < 7; i++)
                {
                    NextdateColl.Add(startdate1);
                    startdate1 = startdate1.AddDays(1);
                }
                var startdate2 = dateColl[0].SubractDays(7);
                for (int i = 0; i < 7; i++)
                {
                    PrevdateColl.Add(startdate2);
                    startdate2 = startdate2.AddDays(1);
                }
            }
            else if (ScheduleType == ScheduleType.WorkWeek)
            {
                ObservableCollection<DayOfWeek> WorkDayCollection = StringToDaysOfWeekConverter(NonWorkingDays, true);
                if (WorkDayCollection.Count >= 7 || WorkDayCollection.Count <= 0)
                    WorkDayCollection = StringToDaysOfWeekConverter("Saturday,Sunday", true);
                if (DateTimeFormatInfo.CurrentInfo != null)
                {
                    var firstDate = date.StartOfWeek(DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek).AddDays(6);
                    List<DateTime> currentdates = new List<DateTime>();
                    for (int i = 0; i < WorkDayCollection.Count; i++)
                    {
                        currentdates.Add(firstDate.StartOfWeek(WorkDayCollection[i]));
                    }
                    foreach (DateTime curDate in currentdates.OrderBy(dt => dt))
                    {
                        dateColl.Add(curDate);
                    }                   
                }
                if (dateColl.Count > 0)
                {
                    var startdate1 = dateColl[0].StartOfWeek(DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek).AddDays(13);
                    List<DateTime> Nextdates = new List<DateTime>();
                    for (int i = 0; i < WorkDayCollection.Count; i++)
                    {
                        Nextdates.Add(startdate1.StartOfWeek(WorkDayCollection[i]));
                    }
                    foreach (DateTime curDate in Nextdates.OrderBy(dt => dt))
                    {
                        NextdateColl.Add(curDate);
                    }                    
                    var startdate2 = dateColl[0].StartOfWeek(DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek).SubractDays(1);
                    List<DateTime> Prevdates = new List<DateTime>();
                    for (int i = 0; i < WorkDayCollection.Count; i++)
                    {
                        Prevdates.Add(startdate2.StartOfWeek(WorkDayCollection[i]));
                    }
                    foreach (DateTime curDate in Prevdates.OrderBy(dt => dt))
                    {
                        PrevdateColl.Add(curDate);
                    }                   
                }
            }
            else if (ScheduleType == ScheduleType.Month)
            {
                var firstDate = date;
                dateColl = CalculateMonth(firstDate);

                var startdate1 = firstDate.AddMonths(1);
                NextdateColl = CalculateMonth(startdate1);

                var startdate2 = firstDate.AddMonths(-1);
                PrevdateColl = CalculateMonth(startdate2);
            }
            else
            {
                if (ScheduleTypeToTimeline)
                {
                    DateRangeDateCollection(date);
                }
                else
                {
                    if (CurrentSelectedDates != null && (CurrentSelectedDates.Count == 1 || CurrentSelectedDates.Count > 7))
                    {
                        dateColl.Add(date);
                        PrevdateColl.Add(date.AddDays(-1));
                        NextdateColl.Add(date.AddDays(1));
                    }
                    else
                    {
                        var firstDate = date;
                        if (DateTimeFormatInfo.CurrentInfo != null)
                        {
                            var startDate = firstDate.StartOfWeek(DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek);
                            for (int i = 0; i < 7; i++)
                            {
                                dateColl.Add(startDate);
                                startDate = startDate.AddDays(1);
                            }
                        }
                        var startdate1 = dateColl[0].AddDays(7);
                        for (int i = 0; i < 7; i++)
                        {
                            NextdateColl.Add(startdate1);
                            startdate1 = startdate1.AddDays(1);
                        }
                        var startdate2 = dateColl[0].SubractDays(7);
                        for (int i = 0; i < 7; i++)
                        {
                            PrevdateColl.Add(startdate2);
                            startdate2 = startdate2.AddDays(1);
                        }
                    }
                }
            }
            if (CurrentSelectedDates != null && checkdate == CurrentSelectedDates[0].Date)
            {
                SelectedDates = dateColl;
            }
            else if (checkdate == PrevSelectedDates[0].Date)
            {
                CurrentSelectedDates = NextdateColl;
                PrevSelectedDates = dateColl;
                NextSelectedDates = PrevdateColl;
            }
            else
            {
                CurrentSelectedDates = PrevdateColl;
                PrevSelectedDates = NextdateColl;
                NextSelectedDates = dateColl;
            }
            ClearDragDropCanvas();
            if (addnewpopup != null)
                addnewpopup.IsOpen = false;
            if (editpopup != null)
                editpopup.IsOpen = false;
            VisibleDates = dateColl;
            CurrentVisibleSelectedDates = dateColl;
            SetNavigationTap();
            dateColl = null;
            PrevdateColl = null;
            NextdateColl = null;
        }

        #endregion

        #region Set selected dates from visible dates
        void SetSelectedDatesFromVisibleDates()
        {
            var dateColl = VisibleDates;
            var PrevdateColl = new ObservableCollection<DateTime>();
            var NextdateColl = new ObservableCollection<DateTime>();
            DateTime checkdate;
            checkdate = CurrentSelectedDates != null ? CurrentSelectedDates[0].Date : DateTime.Now.Date;
            if (ScheduleType == ScheduleType.Day)
            {
                int visibleDateCount = VisibleDates.Count;
                foreach (DateTime dt in dateColl)
                {
                    PrevdateColl.Add(dt.AddDays(-(visibleDateCount + 1)));
                    NextdateColl.Add(dt.AddDays(visibleDateCount + 1));
                }

            }
            if (checkdate == CurrentSelectedDates[0].Date)
            {
                SelectedDates = dateColl;
            }
            else if (checkdate == PrevSelectedDates[0].Date)
            {
                CurrentSelectedDates = NextdateColl;
                PrevSelectedDates = dateColl;
                NextSelectedDates = PrevdateColl;
            }
            else
            {
                CurrentSelectedDates = PrevdateColl;
                PrevSelectedDates = NextdateColl;
                NextSelectedDates = dateColl;
            }
            if (SelectedDate.Date != dateColl[0])
                SelectedDate = dateColl[0].Add(SelectedDate.TimeOfDay);
            CurrentVisibleSelectedDates = dateColl;
            SetNavigationTap();
        }
        #endregion

        #region Select Previous Appointment
        internal ScheduleAppointment SelectPreviousAppointment()
        {
            if (SelectedAppointment != null)
            {
                ScheduleAppointment app = null;
                if (ScheduleType == Schedule.ScheduleType.Month)
                {
                    int index = ProxyAppointments[SelectedAppointment.StartTime.Date].OrderBy(x => (x.EndTime - x.StartTime)).ToList().IndexOf(SelectedAppointment);
                    if (Resource != string.Empty && ScheduleResourceType != null)
                    {
                        index = ProxyAppointments[SelectedAppointment.StartTime.Date].Where(CurrApp => CurrApp.ResourceCollection.FirstOrDefault(ReSrc => ReSrc.TypeName == Resource) != null).OrderBy(x => (x.EndTime - x.StartTime)).ToList().IndexOf(SelectedAppointment);
                    }
                    if (index < ProxyAppointments[SelectedAppointment.StartTime.Date].Count() - 1)
                    {
                        if (Resource != string.Empty && ScheduleResourceType != null)
                        {
                            app = ProxyAppointments[SelectedAppointment.StartTime.Date].Where(CurrApp => CurrApp.ResourceCollection.FirstOrDefault(ReSrc => ReSrc.TypeName == Resource) != null).OrderBy(x => (x.EndTime - x.StartTime)).ToList()[index + 1];
                        }
                        else
                        {
                            app = ProxyAppointments[SelectedAppointment.StartTime.Date].OrderBy(x => (x.EndTime - x.StartTime)).ToList()[index + 1];
                        }
                    }
                    if (app == null)
                    {
                        DateTime nextkey = ProxyAppointments.Keys.OrderBy(dt => dt.Date).LastOrDefault(k => k.Date < SelectedAppointment.StartTime.Date && ProxyAppointments[k].Count > 0 && ProxyAppointments[k].LastOrDefault(App =>!App.Equals(SelectedAppointment)) != null);
                        if (Resource != string.Empty && ScheduleResourceType != null)
                        {
                            nextkey = ProxyAppointments.Keys.OrderBy(dt => dt.Date).LastOrDefault(k => k.Date < SelectedAppointment.StartTime.Date && ProxyAppointments[k].Count > 0 && ProxyAppointments[k].LastOrDefault(App => App.ResourceCollection.FirstOrDefault(Res => Res.TypeName == Resource) != null && !App.Equals(SelectedAppointment)) != null);
                            while (nextkey != new DateTime() && !(ProxyAppointments[nextkey].Count > 0) && ProxyAppointments[nextkey].LastOrDefault(App => App.ResourceCollection.FirstOrDefault(Res => Res.TypeName == Resource) != null && !App.Equals(SelectedAppointment)) == null)
                            {
                                nextkey = ProxyAppointments.Keys.OrderBy(dt => dt.Date).LastOrDefault(k => k.Date < nextkey);
                            }
                        }
                        else
                        {
                            while (nextkey != new DateTime() && !(ProxyAppointments[nextkey].Count > 0) && ProxyAppointments[nextkey].LastOrDefault(App => !App.Equals(SelectedAppointment)) == null)
                            {
                                nextkey = ProxyAppointments.Keys.OrderBy(dt => dt.Date).LastOrDefault(k => k.Date < nextkey);
                            }
                        }
                        if (nextkey != new DateTime() && SelectedDates.Contains(nextkey) && !ProxyAppointments[nextkey][ProxyAppointments[nextkey].Count - 1].Equals(SelectedAppointment))
                        {
                            if (SelectedAppointment != null)
                            {
                                SelectedAppointment.IsSelected = false;
                                SelectedAppointment.AppointmentSelectionBrush = new SolidColorBrush(Colors.Transparent);
                            }
                            if (Resource != string.Empty && ScheduleResourceType != null)
                            {
                                SelectedAppointment = ProxyAppointments[nextkey].Where(CurrApp => CurrApp.ResourceCollection.FirstOrDefault(ReSrc => ReSrc.TypeName == Resource) != null).OrderBy(x => (x.EndTime - x.StartTime)).ToList()[0];
                            }
                            else
                            {
                                SelectedAppointment = ProxyAppointments[nextkey].OrderBy(x => (x.EndTime - x.StartTime)).ToList()[0];
                            }
                            SelectedAppointment.IsSelected = true;
                            SelectedAppointment.AppointmentSelectionBrush = AppointmentSelectionBrush;
                        }
                        else if (nextkey != new DateTime())
                        {
                            MoveToDate(nextkey);
                            if (SelectedAppointment != null)
                            {
                                SelectedAppointment.IsSelected = false;
                                SelectedAppointment.AppointmentSelectionBrush = new SolidColorBrush(Colors.Transparent);
                            }
                            if (Resource != string.Empty && ScheduleResourceType != null)
                            {
                                SelectedAppointment = ProxyAppointments[nextkey].Where(CurrApp => CurrApp.ResourceCollection.FirstOrDefault(ReSrc => ReSrc.TypeName == Resource) != null).OrderBy(x => (x.EndTime - x.StartTime)).ToList()[0];
                            }
                            else
                            {
                                SelectedAppointment = ProxyAppointments[nextkey].OrderBy(x => (x.EndTime - x.StartTime)).ToList()[0];
                            }
                            SelectedAppointment.IsSelected = true;
                            SelectedAppointment.AppointmentSelectionBrush = AppointmentSelectionBrush;
                        }
                    }
                    else
                    {
                        if (SelectedAppointment != null)
                        {
                            SelectedAppointment.IsSelected = false;
                            SelectedAppointment.AppointmentSelectionBrush = new SolidColorBrush(Colors.Transparent);
                        }
                        SelectedAppointment = app;
                        SelectedAppointment.IsSelected = true;
                        SelectedAppointment.AppointmentSelectionBrush = AppointmentSelectionBrush;
                    }
                }
                else
                {
                    if (SelectedAppointment.AllDay)
                    {
                        int index = ProxyAppointments[SelectedAppointment.StartTime.Date].OrderBy(x => x.StartTime).ToList().IndexOf(SelectedAppointment);
                        if (Resource != string.Empty && ScheduleResourceType != null)
                        {
                            index = ProxyAppointments[SelectedAppointment.StartTime.Date].Where(CurrApp => CurrApp.ResourceCollection.FirstOrDefault(ReSrc => ReSrc.TypeName == Resource) != null).OrderBy(x => x.StartTime).ToList().IndexOf(SelectedAppointment);
                        }                       
                        if (index > 0)
                        {
                            if (Resource != string.Empty && ScheduleResourceType != null)
                            {
                                app = ProxyAppointments[SelectedAppointment.StartTime.Date].Where(CurrApp => CurrApp.ResourceCollection.FirstOrDefault(ReSrc => ReSrc.TypeName == Resource) != null).OrderBy(x => x.StartTime).ToList()[index - 1];
                            }
                            else
                            {
                                app = ProxyAppointments[SelectedAppointment.StartTime.Date].OrderBy(x => x.StartTime).ToList()[index - 1];
                            }
                            if (app != null && !(app.AllDay))
                                app = null;
                        }
                    }
                    if (app == null)
                    {
                        DateTime selAppStartTime = new DateTime();
                        if (SelectedAppointment.AllDay)
                            selAppStartTime = SelectedAppointment.StartTime.Date;
                        else
                            selAppStartTime = SelectedAppointment.StartTime;
                        if (Resource != string.Empty && ScheduleResourceType != null)
                        {
                            app = ProxyAppointments[SelectedAppointment.StartTime.Date].Where(a => a.AllDay != true && a.ResourceCollection.FirstOrDefault(ReSrc => ReSrc.TypeName == Resource) != null).LastOrDefault(a => a.StartTime <= selAppStartTime && a != SelectedAppointment);
                        }
                        else
                        {
                            app = ProxyAppointments[SelectedAppointment.StartTime.Date].Where(a => a.AllDay != true).LastOrDefault(a => a.StartTime <= selAppStartTime && a != SelectedAppointment);
                        }
                        if (app == null)
                        {
                            if (!SelectedAppointment.AllDay)
                            {
                                int count = ProxyAppointments[SelectedAppointment.StartTime.Date].Where(a => a.AllDay == true).ToList().Count();
                                if (count > 0)
                                {
                                    app = ProxyAppointments[SelectedAppointment.StartTime.Date].OrderBy(x => x.StartTime).ToList()[count - 1];
                                    if (app != null && !(app.AllDay))
                                        app = null;
                                }
                            }
                            if (app == null)
                            {
                                DateTime nextkey = ProxyAppointments.Keys.OrderBy(dt => dt.Date).LastOrDefault(k => k.Date < SelectedAppointment.StartTime.Date && ProxyAppointments[k].Count > 0 && ProxyAppointments[k].LastOrDefault(App => !App.Equals(SelectedAppointment)) != null);
                                if (Resource != string.Empty && ScheduleResourceType != null)
                                {
                                    nextkey = ProxyAppointments.Keys.OrderBy(dt => dt.Date).LastOrDefault(k => k.Date < SelectedAppointment.StartTime.Date && ProxyAppointments[k].Count > 0 && ProxyAppointments[k].LastOrDefault(App => App.ResourceCollection.FirstOrDefault(Res => Res.TypeName == Resource) != null && !App.Equals(SelectedAppointment)) != null);
                                    while (nextkey != new DateTime() && !(ProxyAppointments[nextkey].Count > 0) && ProxyAppointments[nextkey].FirstOrDefault(App => App.ResourceCollection.FirstOrDefault(Res => Res.TypeName == Resource) != null && !App.Equals(SelectedAppointment)) == null)
                                    {
                                        nextkey = ProxyAppointments.Keys.OrderBy(dt => dt.Date).LastOrDefault(k => k.Date < nextkey);
                                    }
                                }
                                else
                                {
                                    while (nextkey != new DateTime() && !(ProxyAppointments[nextkey].Count > 0) && ProxyAppointments[nextkey].FirstOrDefault(App => !App.Equals(SelectedAppointment)) == null)
                                    {
                                        nextkey = ProxyAppointments.Keys.OrderBy(dt => dt.Date).LastOrDefault(k => k.Date < nextkey);
                                    }
                                }
                                if (nextkey != new DateTime() && SelectedDates.Contains(nextkey) && !ProxyAppointments[nextkey][ProxyAppointments[nextkey].Count - 1].Equals(SelectedAppointment))
                                {
                                    if (SelectedAppointment != null)
                                    {
                                        SelectedAppointment.IsSelected = false;
                                        SelectedAppointment.AppointmentSelectionBrush = new SolidColorBrush(Colors.Transparent);
                                    }
                                    if (ProxyAppointments[nextkey].Where(a => a.AllDay != true).ToList().Count > 0)
                                    {
                                        if (Resource != string.Empty && ScheduleResourceType != null)
                                        {
                                            app = ProxyAppointments[nextkey].Where(a => a.AllDay != true && a.ResourceCollection.FirstOrDefault(ReSrc => ReSrc.TypeName == Resource) != null).OrderBy(a => a.StartTime).ToList()[ProxyAppointments[nextkey].Where(a => a.AllDay != true && a.ResourceCollection.FirstOrDefault(ReSrc => ReSrc.TypeName == Resource) != null).ToList().Count - 1];
                                        }
                                        else
                                        {
                                            app = ProxyAppointments[nextkey].Where(a => a.AllDay != true).OrderBy(a => a.StartTime).ToList()[ProxyAppointments[nextkey].Where(a => a.AllDay != true).ToList().Count - 1];
                                        }
                                    }
                                    else
                                    {
                                        if (Resource != string.Empty && ScheduleResourceType != null)
                                        {
                                            app = ProxyAppointments[nextkey].Where(a => a.AllDay == true && a.ResourceCollection.FirstOrDefault(ReSrc => ReSrc.TypeName == Resource) != null).OrderBy(a => a.StartTime).ToList()[ProxyAppointments[nextkey].Where(a => a.AllDay == true && a.ResourceCollection.FirstOrDefault(ReSrc => ReSrc.TypeName == Resource) != null).ToList().Count - 1];
                                        }
                                        else
                                        {
                                            app = ProxyAppointments[nextkey].Where(a => a.AllDay == true).OrderBy(a => a.StartTime).ToList()[ProxyAppointments[nextkey].Where(a => a.AllDay == true).ToList().Count - 1];
                                        }
                                    }
                                    SelectedAppointment = app;
                                    SelectedAppointment.IsSelected = true;
                                    SelectedAppointment.AppointmentSelectionBrush = AppointmentSelectionBrush;
                                }
                                else if (nextkey != new DateTime())
                                {
                                    MoveToDate(nextkey);
                                    if (SelectedAppointment != null)
                                    {
                                        SelectedAppointment.IsSelected = false;
                                        SelectedAppointment.AppointmentSelectionBrush = new SolidColorBrush(Colors.Transparent);
                                    }
                                    if (ProxyAppointments[nextkey].Where(a => a.AllDay != true).ToList().Count > 0)
                                    {
                                        if (Resource != string.Empty && ScheduleResourceType != null)
                                        {
                                            app = ProxyAppointments[nextkey].Where(a => a.AllDay != true && a.ResourceCollection.FirstOrDefault(ReSrc => ReSrc.TypeName == Resource) != null).OrderBy(a => a.StartTime).ToList()[ProxyAppointments[nextkey].Where(a => a.AllDay != true && a.ResourceCollection.FirstOrDefault(ReSrc => ReSrc.TypeName == Resource) != null).ToList().Count - 1];
                                        }
                                        else
                                        {
                                            app = ProxyAppointments[nextkey].Where(a => a.AllDay != true).OrderBy(a => a.StartTime).ToList()[ProxyAppointments[nextkey].Where(a => a.AllDay != true).ToList().Count - 1];
                                        }
                                    }
                                    else
                                    {
                                        if (Resource != string.Empty && ScheduleResourceType != null)
                                        {
                                            app = ProxyAppointments[nextkey].Where(a => a.AllDay == true && a.ResourceCollection.FirstOrDefault(ReSrc => ReSrc.TypeName == Resource) != null).OrderBy(a => a.StartTime).ToList()[ProxyAppointments[nextkey].Where(a => a.AllDay == true && a.ResourceCollection.FirstOrDefault(ReSrc => ReSrc.TypeName == Resource) != null).ToList().Count - 1];
                                        }
                                        else
                                        {
                                            app = ProxyAppointments[nextkey].Where(a => a.AllDay == true).OrderBy(a => a.StartTime).ToList()[ProxyAppointments[nextkey].Where(a => a.AllDay == true).ToList().Count - 1];
                                        }
                                    }
                                    SelectedAppointment = app;
                                    SelectedAppointment.IsSelected = true;
                                    SelectedAppointment.AppointmentSelectionBrush = AppointmentSelectionBrush;
                                }
                            }
                            else
                            {
                                if (SelectedAppointment != null)
                                {
                                    SelectedAppointment.IsSelected = false;
                                    SelectedAppointment.AppointmentSelectionBrush = new SolidColorBrush(Colors.Transparent);
                                }
                                SelectedAppointment = app;
                                SelectedAppointment.IsSelected = true;
                                SelectedAppointment.AppointmentSelectionBrush = AppointmentSelectionBrush;
                            }
                        }
                        else
                        {
                            if (SelectedAppointment != null)
                            {
                                SelectedAppointment.IsSelected = false;
                                SelectedAppointment.AppointmentSelectionBrush = new SolidColorBrush(Colors.Transparent);
                            }
                            SelectedAppointment = app;
                            SelectedAppointment.IsSelected = true;
                            SelectedAppointment.AppointmentSelectionBrush = AppointmentSelectionBrush;
                        }

                    }
                    else
                    {
                        if (SelectedAppointment != null)
                        {
                            SelectedAppointment.IsSelected = false;
                            SelectedAppointment.AppointmentSelectionBrush = new SolidColorBrush(Colors.Transparent);
                        }
                        SelectedAppointment = app;
                        SelectedAppointment.IsSelected = true;
                        SelectedAppointment.AppointmentSelectionBrush = AppointmentSelectionBrush;
                    }
                }
            }
            else
            {
                DateTime CurrentSelectedDate;
                if (ScheduleType == Schedule.ScheduleType.Month)
                {
                    CurrentSelectedDate = SelectedDate;
                }
                else
                {
                    CurrentSelectedDate = InternalSelectedDate;
                }
                var Nextdate = ProxyAppointments.Keys.OrderBy(dt => dt.Date).LastOrDefault(k => k.Date <= CurrentSelectedDate.Date && ProxyAppointments[k].Count > 0 && ProxyAppointments[k].Count > 0 && ProxyAppointments[k].LastOrDefault(App => !App.Equals(SelectedAppointment)) != null);
                if (Resource != string.Empty && ScheduleResourceType != null)
                {
                    Nextdate = ProxyAppointments.Keys.OrderBy(dt => dt.Date).LastOrDefault(k => k.Date <= CurrentSelectedDate.Date && ProxyAppointments[k].Count > 0 && ProxyAppointments[k].Count > 0 && ProxyAppointments[k].LastOrDefault(App => App.ResourceCollection.FirstOrDefault(Res => Res.TypeName == Resource) != null && !App.Equals(SelectedAppointment)) != null);
                }
                if (Nextdate != new DateTime())
                {
                    if (!SelectedDates.Contains(Nextdate))
                    {
                        MoveToDate(Nextdate);
                    }
                    if (ProxyAppointments[Nextdate].Count > 0)
                    {
                        DateTime CurrentAppSelectedDate;
                        if (ScheduleType == Schedule.ScheduleType.TimeLine)
                        {
                            CurrentAppSelectedDate = SelectedDate;
                        }
                        else
                        {
                            CurrentAppSelectedDate = InternalSelectedDate;
                        }
                        ScheduleAppointment dayapp = ProxyAppointments[Nextdate].OrderBy(a => a.StartTime).LastOrDefault(a => a.StartTime <= CurrentAppSelectedDate);
                        ScheduleAppointment monthapp = ProxyAppointments[Nextdate].OrderBy(a => a.StartTime).ToList()[0];
                        if (Resource != string.Empty && ScheduleResourceType != null)
                        {
                            dayapp = ProxyAppointments[Nextdate].Where(App => App.ResourceCollection.FirstOrDefault(resrc => resrc.TypeName == Resource) != null).OrderBy(a => a.StartTime).LastOrDefault(a => a.StartTime <= CurrentAppSelectedDate);
                            monthapp = ProxyAppointments[Nextdate].Where(App => App.ResourceCollection.FirstOrDefault(resrc => resrc.TypeName == Resource) != null).OrderBy(a => a.StartTime).ToList()[0];
                        }
                        ScheduleAppointment app;
                        if (ScheduleType == ScheduleType.Month)
                        {
                            app = monthapp;
                        }
                        else
                        {
                            app = dayapp;
                        }
                        if (app != null)
                        {
                            if (SelectedAppointment != null)
                            {
                                SelectedAppointment.IsSelected = false;
                                SelectedAppointment.AppointmentSelectionBrush = new SolidColorBrush(Colors.Transparent);
                            }
                            SelectedAppointment = app;
                            SelectedAppointment.IsSelected = true;
                            SelectedAppointment.AppointmentSelectionBrush = AppointmentSelectionBrush;
                        }
                    }
                    else if (Currentselecteddate != new DateTime())
                    {
                        if (ProxyAppointments[Currentselecteddate.Date].Count > 0)
                        {
                            SelectedAppointment = ProxyAppointments[Currentselecteddate.Date][ProxyAppointments[Currentselecteddate.Date].Count - 1];
                            SelectedAppointment.IsSelected = true;
                            SelectedAppointment.AppointmentSelectionBrush = AppointmentSelectionBrush;
                        }
                    }
                }
            }

            return SelectedAppointment;
        }

        #endregion

        #region Select Next Appointment
        internal ScheduleAppointment SelectNextAppointment()
        {
            if (SelectedAppointment != null)
            {
                ScheduleAppointment app = null;
                if (ScheduleType == Schedule.ScheduleType.Month)
                {
                    int index = ProxyAppointments[SelectedAppointment.StartTime.Date].OrderBy(x => (x.EndTime - x.StartTime)).ToList().IndexOf(SelectedAppointment);
                    if (Resource != string.Empty && ScheduleResourceType != null)
                    {
                        index = ProxyAppointments[SelectedAppointment.StartTime.Date].Where(CurrApp=>CurrApp.ResourceCollection.FirstOrDefault(ReSrc=>ReSrc.TypeName == Resource)!= null).OrderBy(x => (x.EndTime - x.StartTime)).ToList().IndexOf(SelectedAppointment);
                    }
                    if (index > 0)
                    {
                        if (Resource != string.Empty && ScheduleResourceType != null)
                        {
                            app = ProxyAppointments[SelectedAppointment.StartTime.Date].Where(CurrApp => CurrApp.ResourceCollection.FirstOrDefault(ReSrc => ReSrc.TypeName == Resource) != null).OrderBy(x => (x.EndTime - x.StartTime)).ToList()[index - 1];
                        }
                        else
                        {
                            app = ProxyAppointments[SelectedAppointment.StartTime.Date].OrderBy(x => (x.EndTime - x.StartTime)).ToList()[index - 1];
                        }
                    }
                    if (app == null)
                    {
                        DateTime nextkey = ProxyAppointments.Keys.OrderBy(dt => dt.Date).FirstOrDefault(k => k.Date > SelectedAppointment.StartTime.Date && ProxyAppointments[k].Count > 0 && ProxyAppointments[k].FirstOrDefault(App => !App.Equals(SelectedAppointment)) != null);
                        if (Resource != string.Empty && ScheduleResourceType != null)
                        {
                            nextkey = ProxyAppointments.Keys.OrderBy(dt => dt.Date).FirstOrDefault(k => k.Date > SelectedAppointment.StartTime.Date && ProxyAppointments[k].Count > 0 && ProxyAppointments[k].FirstOrDefault(App => App.ResourceCollection.FirstOrDefault(Res => Res.TypeName == Resource) != null && !App.Equals(SelectedAppointment)) != null);
                            while (nextkey != new DateTime() && !(ProxyAppointments[nextkey].Count > 0) && ProxyAppointments[nextkey].FirstOrDefault(App => App.ResourceCollection.FirstOrDefault(Res => Res.TypeName == Resource) != null && !App.Equals(SelectedAppointment)) == null)
                            {
                                nextkey = ProxyAppointments.Keys.OrderBy(dt => dt.Date).FirstOrDefault(k => k.Date > nextkey);
                            }
                        }
                        else
                        {
                            while (nextkey != new DateTime() && !(ProxyAppointments[nextkey].Count > 0) && ProxyAppointments[nextkey].FirstOrDefault(App => !App.Equals(SelectedAppointment)) == null)
                            {
                                nextkey = ProxyAppointments.Keys.OrderBy(dt => dt.Date).FirstOrDefault(k => k.Date > nextkey);
                            }
                        }
                        if (nextkey != new DateTime() && SelectedDates.Contains(nextkey) && !ProxyAppointments[nextkey][0].Equals(SelectedAppointment))
                        {
                            if (SelectedAppointment != null)
                            {
                                SelectedAppointment.IsSelected = false;
                                SelectedAppointment.AppointmentSelectionBrush = new SolidColorBrush(Colors.Transparent);
                            }
                            if (Resource != string.Empty && ScheduleResourceType != null)
                            {
                                SelectedAppointment = ProxyAppointments[nextkey].Where(CurrApp => CurrApp.ResourceCollection.FirstOrDefault(ReSrc => ReSrc.TypeName == Resource) != null).OrderBy(x => (x.EndTime - x.StartTime)).ToList()[ProxyAppointments[nextkey].Count - 1];
                            }
                            else
                            {
                                SelectedAppointment = ProxyAppointments[nextkey].OrderBy(x => (x.EndTime - x.StartTime)).ToList()[ProxyAppointments[nextkey].Count - 1];
                            }
                            SelectedAppointment.IsSelected = true;
                            SelectedAppointment.AppointmentSelectionBrush = AppointmentSelectionBrush;
                        }
                        else if (nextkey != new DateTime())
                        {
                            MoveToDate(nextkey);
                            if (SelectedAppointment != null)
                            {
                                SelectedAppointment.IsSelected = false;
                                SelectedAppointment.AppointmentSelectionBrush = new SolidColorBrush(Colors.Transparent);
                            }
                            if (Resource != string.Empty && ScheduleResourceType != null)
                            {
                                SelectedAppointment = ProxyAppointments[nextkey].Where(CurrApp => CurrApp.ResourceCollection.FirstOrDefault(ReSrc => ReSrc.TypeName == Resource) != null).OrderBy(x => (x.EndTime - x.StartTime)).ToList()[ProxyAppointments[nextkey].Count - 1];
                            }
                            else
                            {
                                SelectedAppointment = ProxyAppointments[nextkey].OrderBy(x => (x.EndTime - x.StartTime)).ToList()[ProxyAppointments[nextkey].Count - 1];
                            }
                            SelectedAppointment.IsSelected = true;
                            SelectedAppointment.AppointmentSelectionBrush = AppointmentSelectionBrush;
                        }
                    }
                    else
                    {
                        if (SelectedAppointment != null)
                        {
                            SelectedAppointment.IsSelected = false;
                            SelectedAppointment.AppointmentSelectionBrush = new SolidColorBrush(Colors.Transparent);
                        }
                        SelectedAppointment = app;
                        SelectedAppointment.IsSelected = true;
                        SelectedAppointment.AppointmentSelectionBrush = AppointmentSelectionBrush;
                    }
                }
                else
                {
                    if (SelectedAppointment.AllDay)
                    {
                        int index = ProxyAppointments[SelectedAppointment.StartTime.Date].OrderBy(x => x.StartTime).ToList().IndexOf(SelectedAppointment);
                        if (Resource != string.Empty && ScheduleResourceType != null)
                        {
                            index = ProxyAppointments[SelectedAppointment.StartTime.Date].Where(CurrApp => CurrApp.ResourceCollection.FirstOrDefault(ReSrc => ReSrc.TypeName == Resource) != null).OrderBy(x => x.StartTime).ToList().IndexOf(SelectedAppointment);
                        }
                        if (index < ProxyAppointments[SelectedAppointment.StartTime.Date].Count() - 1)
                        {
                            if (Resource != string.Empty && ScheduleResourceType != null)
                            {
                                app = ProxyAppointments[SelectedAppointment.StartTime.Date].Where(CurrApp => CurrApp.ResourceCollection.FirstOrDefault(ReSrc => ReSrc.TypeName == Resource) != null).OrderBy(x => x.StartTime).ToList()[index + 1];
                            }
                            else
                            {
                                app = ProxyAppointments[SelectedAppointment.StartTime.Date].OrderBy(x => x.StartTime).ToList()[index + 1];
                            }
                            if (app != null && !(app.AllDay))
                                app = null;
                        }

                    }
                    if (app == null)
                    {
                        DateTime selAppStartTime = new DateTime();
                        if (SelectedAppointment.AllDay)
                            selAppStartTime = SelectedAppointment.StartTime.Date;
                        else
                            selAppStartTime = SelectedAppointment.StartTime;
                        if (Resource != string.Empty && ScheduleResourceType != null)
                        {
                            app = ProxyAppointments[SelectedAppointment.StartTime.Date].Where(a => a.AllDay != true && a.ResourceCollection.FirstOrDefault(ReSrc => ReSrc.TypeName == Resource) != null).OrderBy(x => x.StartTime).ToList().FirstOrDefault(a => a.StartTime >= selAppStartTime && a != SelectedAppointment);
                        }
                        else
                        {
                            app = ProxyAppointments[SelectedAppointment.StartTime.Date].Where(a => a.AllDay != true).OrderBy(x => x.StartTime).ToList().FirstOrDefault(a => a.StartTime >= selAppStartTime && a != SelectedAppointment);
                        }
                        if (app == null)
                        {

                            DateTime nextkey = ProxyAppointments.Keys.OrderBy(dt => dt.Date).FirstOrDefault(k => k.Date > SelectedAppointment.StartTime.Date && ProxyAppointments[k].Count > 0 && ProxyAppointments[k].FirstOrDefault(App => !App.Equals(SelectedAppointment)) != null);
                            if (Resource != string.Empty && ScheduleResourceType != null)
                            {
                                nextkey = ProxyAppointments.Keys.OrderBy(dt => dt.Date).FirstOrDefault(k => k.Date > SelectedAppointment.StartTime.Date && ProxyAppointments[k].Count > 0 && ProxyAppointments[k].FirstOrDefault(App => App.ResourceCollection.FirstOrDefault(Res => Res.TypeName == Resource) != null && !App.Equals(SelectedAppointment)) != null);
                                while (nextkey != new DateTime() && !(ProxyAppointments[nextkey].Count > 0) && ProxyAppointments[nextkey].FirstOrDefault(App => App.ResourceCollection.FirstOrDefault(Res => Res.TypeName == Resource) != null && !App.Equals(SelectedAppointment)) == null)
                                {
                                    nextkey = ProxyAppointments.Keys.OrderBy(dt => dt.Date).FirstOrDefault(k => k.Date > nextkey);
                                }
                            }
                            else
                            {
                                while (nextkey != new DateTime() && !(ProxyAppointments[nextkey].Count > 0) && ProxyAppointments[nextkey].FirstOrDefault(App=>!App.Equals(SelectedAppointment)) == null)
                                {
                                    nextkey = ProxyAppointments.Keys.OrderBy(dt => dt.Date).FirstOrDefault(k => k.Date > nextkey);
                                }
                            }
                            if (nextkey != new DateTime() && SelectedDates.Contains(nextkey) && !ProxyAppointments[nextkey][0].Equals(SelectedAppointment))
                            {
                                if (SelectedAppointment != null)
                                {
                                    SelectedAppointment.IsSelected = false;
                                    SelectedAppointment.AppointmentSelectionBrush = new SolidColorBrush(Colors.Transparent);
                                }
                                if (ProxyAppointments[nextkey].Where(a => a.AllDay == true).ToList().Count > 0)
                                {
                                    if (Resource != string.Empty && ScheduleResourceType != null)
                                    {
                                        app = ProxyAppointments[nextkey].Where(a => a.AllDay == true && a.ResourceCollection.FirstOrDefault(ReSrc => ReSrc.TypeName == Resource) != null).OrderBy(a => a.StartTime).ToList()[0];
                                    }
                                    else
                                    {
                                        app = ProxyAppointments[nextkey].Where(a => a.AllDay == true).OrderBy(a => a.StartTime).ToList()[0];
                                    }
                                }
                                if (app == null)
                                {
                                    if (Resource != string.Empty && ScheduleResourceType != null)
                                    {
                                        app = ProxyAppointments[nextkey].Where(a => a.AllDay != true && a.ResourceCollection.FirstOrDefault(ReSrc => ReSrc.TypeName == Resource) != null).OrderBy(a => a.StartTime).ToList()[0];
                                    }
                                    else
                                    {
                                        app = ProxyAppointments[nextkey].Where(a => a.AllDay != true).OrderBy(a => a.StartTime).ToList()[0];
                                    }
                                }
                                SelectedAppointment = app;
                                SelectedAppointment.IsSelected = true;
                                SelectedAppointment.AppointmentSelectionBrush = AppointmentSelectionBrush;
                            }
                            else if (nextkey != new DateTime())
                            {
                                MoveToDate(nextkey);
                                if (SelectedAppointment != null)
                                {
                                    SelectedAppointment.IsSelected = false;
                                    SelectedAppointment.AppointmentSelectionBrush = new SolidColorBrush(Colors.Transparent);
                                }
                                if (ProxyAppointments[nextkey].Where(a => a.AllDay == true).ToList().Count > 0)
                                {
                                    if (Resource != string.Empty && ScheduleResourceType != null)
                                    {
                                        app = ProxyAppointments[nextkey].Where(a => a.AllDay == true && a.ResourceCollection.FirstOrDefault(ReSrc => ReSrc.TypeName == Resource) != null).OrderBy(a => a.StartTime).ToList()[0];
                                    }
                                    else
                                    {
                                        app = ProxyAppointments[nextkey].Where(a => a.AllDay == true).OrderBy(a => a.StartTime).ToList()[0];
                                    }
                                }
                                if (app == null)
                                {                                    
                                    if (Resource != string.Empty && ScheduleResourceType != null)
                                    {
                                        app = ProxyAppointments[nextkey].Where(a => a.AllDay != true && a.ResourceCollection.FirstOrDefault(ReSrc => ReSrc.TypeName == Resource) != null).OrderBy(a => a.StartTime).ToList()[0];
                                    }
                                    else
                                    {
                                        app = ProxyAppointments[nextkey].Where(a => a.AllDay != true).OrderBy(a => a.StartTime).ToList()[0];
                                    }
                                }
                                SelectedAppointment = app;
                                SelectedAppointment.IsSelected = true;
                                SelectedAppointment.AppointmentSelectionBrush = AppointmentSelectionBrush;
                            }
                        }
                        else
                        {
                            if (SelectedAppointment != null)
                            {
                                SelectedAppointment.IsSelected = false;
                                SelectedAppointment.AppointmentSelectionBrush = new SolidColorBrush(Colors.Transparent);
                            }
                            SelectedAppointment = app;
                            SelectedAppointment.IsSelected = true;
                            SelectedAppointment.AppointmentSelectionBrush = AppointmentSelectionBrush;
                        }

                    }
                    else
                    {
                        if (SelectedAppointment != null)
                        {
                            SelectedAppointment.IsSelected = false;
                            SelectedAppointment.AppointmentSelectionBrush = new SolidColorBrush(Colors.Transparent);
                        }
                        SelectedAppointment = app;
                        SelectedAppointment.IsSelected = true;
                        SelectedAppointment.AppointmentSelectionBrush = AppointmentSelectionBrush;
                    }
                }
            }
            else
            {
                DateTime CurrentSelectedDate;
                if (ScheduleType == Schedule.ScheduleType.Month)
                {
                    CurrentSelectedDate = SelectedDate;
                }
                else
                {
                    CurrentSelectedDate = InternalSelectedDate;
                }
                var Nextdate = ProxyAppointments.Keys.OrderBy(dt => dt.Date).FirstOrDefault(k => k.Date >= CurrentSelectedDate.Date && ProxyAppointments[k].Count > 0 && ProxyAppointments[k].FirstOrDefault(App => !App.Equals(SelectedAppointment)) != null);
                if (Resource != string.Empty && ScheduleResourceType != null)                
                {
                    Nextdate = ProxyAppointments.Keys.OrderBy(dt => dt.Date).FirstOrDefault(k => k.Date >= CurrentSelectedDate.Date && ProxyAppointments[k].Count > 0 && ProxyAppointments[k].FirstOrDefault(App => App.ResourceCollection.FirstOrDefault(Res => Res.TypeName == Resource) != null && !App.Equals(SelectedAppointment)) != null);
                }

                if (Nextdate != new DateTime())
                {
                    if (!SelectedDates.Contains(Nextdate) && Nextdate != new DateTime())
                    {
                        MoveToDate(Nextdate);
                    }
                    if (ProxyAppointments[Nextdate].Count > 0)
                    {
                        DateTime CurrentAppSelectedDate;
                        if (ScheduleType == Schedule.ScheduleType.TimeLine)
                        {
                            CurrentAppSelectedDate = SelectedDate;
                        }
                        else
                        {
                            CurrentAppSelectedDate = InternalSelectedDate;
                        }

                        ScheduleAppointment dayapp = ProxyAppointments[Nextdate].FirstOrDefault(a => a.StartTime >= CurrentAppSelectedDate);
                        ScheduleAppointment monthapp=ProxyAppointments[Nextdate].OrderBy(a => a.StartTime).ToList()[0];
                        if (Resource != string.Empty && ScheduleResourceType != null)
                        {
                            dayapp = ProxyAppointments[Nextdate].Where(App=>App.ResourceCollection.FirstOrDefault(resrc=>resrc.TypeName == Resource)!=null).FirstOrDefault(a => a.StartTime >= CurrentAppSelectedDate);
                            monthapp = ProxyAppointments[Nextdate].Where(App => App.ResourceCollection.FirstOrDefault(resrc => resrc.TypeName == Resource) != null).OrderBy(a => a.StartTime).ToList()[0];
                        }
                        ScheduleAppointment app ;
                        if (ScheduleType == ScheduleType.Month)
                        {
                            app = monthapp;
                        }
                        else
                        {
                            app = dayapp;
                        }
                        
                        if (app != null)
                        {
                            if (SelectedAppointment != null)
                            {
                                SelectedAppointment.IsSelected = false;
                                SelectedAppointment.AppointmentSelectionBrush = new SolidColorBrush(Colors.Transparent);
                            }
                            SelectedAppointment = app;
                            SelectedAppointment.IsSelected = true;
                            SelectedAppointment.AppointmentSelectionBrush = AppointmentSelectionBrush;
                        }
                    }
                    else if(Currentselecteddate != new DateTime())
                    {
                        if (ProxyAppointments[Currentselecteddate.Date].Count > 0)
                        {
                            SelectedAppointment = ProxyAppointments[Currentselecteddate.Date][ProxyAppointments[Currentselecteddate.Date].Count - 1];
                            SelectedAppointment.IsSelected = true;
                            SelectedAppointment.AppointmentSelectionBrush = AppointmentSelectionBrush;
                        }
                    }
                }

            }

            return SelectedAppointment;
        }

        #endregion

        #region Import & Export

        public void ImportICS()
        {
            ScheduleHelper.ImportICS(this);
        }

        public void ExportICS()
        {
            ScheduleHelper.ExportICS(this);
        }

#if WPF
        public void Export(string fileName)
        {
            ScheduleHelper.Export(this,fileName);
        }

        public void Import(string fileName)
        {
            ScheduleHelper.Import(this, fileName);
        }
#endif

        #endregion

        #region Remove Recursive Appointment

        public void RemoveRecursiveAppointment(ScheduleAppointment recApp)
        {
            if (recApp.IsRecursive)
            {
                if (recApp.GetHashCode() == (int)recApp.RecurrenceID)
                {
                    RemoveRecursive(recApp);
                    SetProxyAppointments(Appointments);
                }
                else
                {
                    RemoveSingleRecursiveAppInProxy(recApp);
                }
                SetCurrentVisibleAppointments(CurrentSelectedDates);
                SetPrevVisibleAppointments(PrevSelectedDates);
                SetNextVisibleAppointments(NextSelectedDates);
                if (SelectedAppointment != null)
                {
                    if (recApp == SelectedAppointment || (recApp.IsRecursive && recApp.RecurrenceID.Equals(SelectedAppointment.RecurrenceID)))
                        SelectedAppointment = null;
                    if (SelectedAppointment == null)
                        editpopup.IsOpen = false;
                }
            }
        }

        #endregion

        #region Visible Textbox

        internal void VisibleTextbox()
        {
            if (ScheduleType == Schedule.ScheduleType.Day || ScheduleType == Schedule.ScheduleType.Week || ScheduleType == Schedule.ScheduleType.WorkWeek)
            {
                daysview.AppTextVisibility = System.Windows.Visibility.Visible;
                daysview.appTextBox.Height = daysview.RectHeight;
                daysview.appTextBox.Width = daysview.RectWidth;
                (daysview.appTextBox).Focus();
            }
            else if (ScheduleType == Schedule.ScheduleType.TimeLine)
            {
                timelineview.TimelineAppTextVisibility = System.Windows.Visibility.Visible;
                (timelineview.timelineAppTextBox).Focus();
            }
        }

        #endregion

        #region Create appointment on multi selection

        internal void CreateAppointmentOnSelection()
        {
            ScheduleAppointment app = new ScheduleAppointment();
            if (ScheduleType == Schedule.ScheduleType.Month)
            {
                if (MinMaxSelection >= Currentselecteddate)
                {
                    app.StartTime = Currentselecteddate;
                    app.EndTime = MinMaxSelection;
                }
                else
                {
                    app.StartTime = MinMaxSelection;
                    app.EndTime = Currentselecteddate;
                }
            }
            else
            {
                if (MinMaxSelection >= Currentselecteddate)
                {
                    app.StartTime = Currentselecteddate;
                    app.EndTime = MinMaxSelection.AddMinutes(GetTimeInterval().TotalMinutes);
                }
                else
                {
                    app.StartTime = MinMaxSelection;
                    app.EndTime = Currentselecteddate.AddMinutes(GetTimeInterval().TotalMinutes);
                }
            }
            MinMaxSelection = Currentselecteddate;
            if (ScheduleResourceType != null && ScheduleResourceType.ResourceCollection.Count > 0)
            {
                foreach (Resource selectedresrc in selectedResourcename)
                {
                    app.ResourceCollection.Add(selectedresrc);
                }
            }
            if (ScheduleType == Schedule.ScheduleType.Day || ScheduleType == Schedule.ScheduleType.Week || ScheduleType == Schedule.ScheduleType.WorkWeek)
            {
                app.Subject = (daysview.appTextBox).Text;
                (daysview.appTextBox).Text = "";
                daysview.AppTextVisibility = System.Windows.Visibility.Collapsed;
            }
            else if (ScheduleType == Schedule.ScheduleType.TimeLine)
            {
                app.Subject = (timelineview.timelineAppTextBox).Text;
                (timelineview.timelineAppTextBox).Text = "";
                timelineview.TimelineAppTextVisibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                app.AllDay = true;
            }
            Appointments.Add(app);
        }

        #endregion

        #region Key Gestures

#if SILVERLIGHT
        private void SLTimeSelectionLeftCmdExecuted(SfSchedule sfSchedule)
        {
            RemoveAppointmentSelection();
            if (ScheduleType == Schedule.ScheduleType.Month)
            {
                if (MinMaxSelection == new DateTime())
                    MinMaxSelection = Currentselecteddate;
                MinMaxSelection = MinMaxSelection.AddDays(-1);
                if (SelectedDates.Contains(MinMaxSelection))
                    monthview.MultiDateSelection(currentitem, MinMaxSelection);
            }
            else if (ScheduleType == Schedule.ScheduleType.TimeLine)
            {
                if (MinMaxSelection == new DateTime())
                    MinMaxSelection = Currentselecteddate;
                if (MinMaxSelection > Currentselecteddate)
                {
                    MinMaxSelection = MinMaxSelection.AddMinutes(-(this.GetTimeInterval().TotalMinutes));
                    timelineview.SubSelectionLeft(MinMaxSelection);
                }
                else if (MinMaxSelection <= Currentselecteddate)
                {
                    MinMaxSelection = MinMaxSelection.AddMinutes(-(this.GetTimeInterval().TotalMinutes));
                    timelineview.AddSelectionLeft(MinMaxSelection);
                }
            }
        }

        private void SLTimeSelectionRightCmdExecuted(SfSchedule sfSchedule)
        {
            RemoveAppointmentSelection();
            if (ScheduleType == Schedule.ScheduleType.Month)
            {
                //MultiDateSelection(currentitem, appDate);
                if (MinMaxSelection == new DateTime())
                    MinMaxSelection = Currentselecteddate;
                MinMaxSelection = MinMaxSelection.AddDays(1);
                if (SelectedDates.Contains(MinMaxSelection))
                    monthview.MultiDateSelection(currentitem, MinMaxSelection);

            }
            else if (ScheduleType == Schedule.ScheduleType.TimeLine)
            {
                if (MinMaxSelection == new DateTime())
                    MinMaxSelection = Currentselecteddate;
                if (MinMaxSelection >= Currentselecteddate)
                {
                    MinMaxSelection = MinMaxSelection.AddMinutes(this.GetTimeInterval().TotalMinutes);
                    timelineview.AddSelectionRight(MinMaxSelection);
                }
                else if (MinMaxSelection < Currentselecteddate)
                {
                    MinMaxSelection = MinMaxSelection.AddMinutes(this.GetTimeInterval().TotalMinutes);
                    timelineview.SubSelectionRight(MinMaxSelection);
                }
            }
        }

        private void SLCreateAppOnSelectionCmdExecuted(SfSchedule sfSchedule)
        {
            bool createAppointment = true;
            bool sameTime = false;
            if (MinMaxSelection == new DateTime())
            {
                MinMaxSelection = Currentselecteddate;
                sameTime = true;
            }
            double appStart, appEnd;
            if (MinMaxSelection.TimeOfDay.TotalMinutes < Currentselecteddate.TimeOfDay.TotalMinutes)
            {
                appStart = MinMaxSelection.TimeOfDay.TotalMinutes / 60;
                appEnd = Currentselecteddate.TimeOfDay.TotalMinutes / 60;
            }
            else if (sameTime)
            {
                appStart = Currentselecteddate.TimeOfDay.TotalMinutes / 60;
                appEnd = Currentselecteddate.TimeOfDay.TotalMinutes / 60;
            }
            else
            {
                appStart = Currentselecteddate.TimeOfDay.TotalMinutes / 60;
                appEnd = MinMaxSelection.TimeOfDay.TotalMinutes / 60;
            }
            if (NonAccessibleBlocks.Count > 0)
            {
                foreach (NonAccessibleBlock block in NonAccessibleBlocks)
                {
                    if ((block.StartHour < appStart && block.EndHour > appStart) || (block.StartHour < appEnd && block.EndHour > appEnd))
                    {
                        createAppointment = false;
                        break;
                    }
                }
            }

            if (SelectedAppointment == null)
            {
                if (createAppointment)
                {
                    if (ScheduleType == Schedule.ScheduleType.Day || ScheduleType == Schedule.ScheduleType.Week || ScheduleType == Schedule.ScheduleType.WorkWeek)
                    {
                        VisibleTextbox();
                    }
                    else if (ScheduleType == Schedule.ScheduleType.Month)
                    {
                        CreateAppointmentOnSelection();
                    }
                    else if (ScheduleType == Schedule.ScheduleType.TimeLine)
                    {
                        VisibleTextbox();
                    }
                }
            }
            else
            {
                EditAppointment();
            }
        }
       
        private void SLTimeSelectionfDownCmdExecuted(SfSchedule sfSchedule)
        {
            RemoveAppointmentSelection();
            if (ScheduleType == Schedule.ScheduleType.Day || ScheduleType == Schedule.ScheduleType.Week || ScheduleType == Schedule.ScheduleType.WorkWeek)
            {
                if (MinMaxSelection == new DateTime())
                    MinMaxSelection = Currentselecteddate;
                if (MinMaxSelection >= Currentselecteddate)
                {
                    MinMaxSelection = MinMaxSelection.AddMinutes(this.GetTimeInterval().TotalMinutes);
                    daysview.AddSelectionDown(MinMaxSelection);
                }
                else if (MinMaxSelection < Currentselecteddate)
                {
                    MinMaxSelection = MinMaxSelection.AddMinutes(this.GetTimeInterval().TotalMinutes);
                    daysview.SubSelectionDown(MinMaxSelection);
                }
            }
            else if (ScheduleType == Schedule.ScheduleType.Month)
            {
                if (MinMaxSelection == new DateTime())
                    MinMaxSelection = Currentselecteddate;
                MinMaxSelection = MinMaxSelection.AddDays(7);
                if (SelectedDates.Contains(MinMaxSelection))
                    monthview.MultiDateSelection(currentitem, MinMaxSelection);
            }
        }

        private void SLTimeSelectionfUpCmdExecuted(SfSchedule sfSchedule)
        {
            RemoveAppointmentSelection();
            if (ScheduleType == Schedule.ScheduleType.Day || ScheduleType == Schedule.ScheduleType.Week || ScheduleType == Schedule.ScheduleType.WorkWeek)
            {
                if (MinMaxSelection == new DateTime())
                    MinMaxSelection = Currentselecteddate;
                if (MinMaxSelection > Currentselecteddate)
                {
                    MinMaxSelection = MinMaxSelection.AddMinutes(-(this.GetTimeInterval().TotalMinutes));
                    daysview.SubSelectionUp(MinMaxSelection);
                }
                else if (MinMaxSelection <= Currentselecteddate)
                {
                    MinMaxSelection = MinMaxSelection.AddMinutes(-(this.GetTimeInterval().TotalMinutes));
                    daysview.AddSelectionUp(MinMaxSelection);
                }
            }
            else if (ScheduleType == Schedule.ScheduleType.Month)
            {
                if (MinMaxSelection == new DateTime())
                    MinMaxSelection = Currentselecteddate;
                MinMaxSelection = MinMaxSelection.AddDays(-7);
                if (SelectedDates.Contains(MinMaxSelection))
                    monthview.MultiDateSelection(currentitem, MinMaxSelection);
            }
        }


        private void SLResizeAppointmentDownCmdExecuted(SfSchedule sfSchedule)
        {
             if (SelectedAppointment != null)
            {
                if (ScheduleType == Schedule.ScheduleType.Day || ScheduleType == Schedule.ScheduleType.Week || ScheduleType == Schedule.ScheduleType.WorkWeek)
                {
                    daysview.MoveSelectionToDateTime(SelectedAppointment.InternalEndTime);
                }
                else if (ScheduleType == Schedule.ScheduleType.TimeLine)
                {
                    timelineview.MoveSelectionRectToDateTime(SelectedAppointment.InternalStartTime);
                }
                SelectedAppointment.InternalEndTime = SelectedAppointment.InternalEndTime.AddMinutes(GetTimeInterval().TotalMinutes);
            }
        }

        private void SLResizeAppointmentUpCmdExecuted(SfSchedule sfSchedule)
        {
        
            if (SelectedAppointment != null)
            {
                SelectedAppointment.InternalStartTime = SelectedAppointment.InternalStartTime.AddMinutes(-(GetTimeInterval().TotalMinutes));
                if (ScheduleType == Schedule.ScheduleType.Day || ScheduleType == Schedule.ScheduleType.Week || ScheduleType == Schedule.ScheduleType.WorkWeek)
                {
                    daysview.MoveSelectionToDateTime(SelectedAppointment.InternalStartTime);
                }
                else if (ScheduleType == Schedule.ScheduleType.TimeLine)
                {
                    timelineview.MoveSelectionRectToDateTime(SelectedAppointment.InternalStartTime);
                }
            }
        }

        private void SLNextAppointmentCmdExecuted(SfSchedule sfSchedule)
        {
            if (ScheduleType == Schedule.ScheduleType.Day || ScheduleType == Schedule.ScheduleType.Week || ScheduleType == Schedule.ScheduleType.WorkWeek)
            {
                if (daysview.AppTextVisibility == System.Windows.Visibility.Visible)
                    CreateAppointmentOnSelection();
            }
            else if (ScheduleType == Schedule.ScheduleType.TimeLine)
            {
                if (timelineview.TimelineAppTextVisibility == System.Windows.Visibility.Visible)
                    CreateAppointmentOnSelection();
            }
            DateTime appDate = SelectNextAppointment().InternalStartTime;
            if (ScheduleType == Schedule.ScheduleType.Day || ScheduleType == Schedule.ScheduleType.Week || ScheduleType == Schedule.ScheduleType.WorkWeek)
            {
                daysview.MoveSelectionToDateTime(appDate);
            }
            else if (ScheduleType == Schedule.ScheduleType.Month)
            {
                monthview.MoveToSelectedMonthDate(currentitem, appDate);
            }
            else if (ScheduleType == Schedule.ScheduleType.TimeLine)
            {
                timelineview.MoveSelectionRectToDateTime(appDate);
            }
        }

        private void SLPreviousAppointmentCmdExecuted(SfSchedule sfSchedule)
        {
            if (ScheduleType == Schedule.ScheduleType.Day || ScheduleType == Schedule.ScheduleType.Week || ScheduleType == Schedule.ScheduleType.WorkWeek)
            {
                if (daysview.AppTextVisibility == System.Windows.Visibility.Visible)
                    CreateAppointmentOnSelection();
            }
            else if (ScheduleType == Schedule.ScheduleType.TimeLine)
            {
                if (timelineview.TimelineAppTextVisibility == System.Windows.Visibility.Visible)
                    CreateAppointmentOnSelection();
            }
            DateTime appDate = SelectPreviousAppointment().InternalStartTime;
            if (ScheduleType == Schedule.ScheduleType.Day || ScheduleType == Schedule.ScheduleType.Week || ScheduleType == Schedule.ScheduleType.WorkWeek)
            {
                daysview.MoveSelectionToDateTime(appDate);
            }
            else if (ScheduleType == Schedule.ScheduleType.Month)
            {
                monthview.MoveToSelectedMonthDate(currentitem, appDate);
            }
            else if (ScheduleType == Schedule.ScheduleType.TimeLine)
            {
                timelineview.MoveSelectionRectToDateTime(appDate);
            }
        }

        private void SLZoomOutCmdExecuted(SfSchedule sfSchedule)
        {
            if (ScheduleType == Schedule.ScheduleType.Day)
            {
                ScheduleType = Schedule.ScheduleType.Week;
            }
            else if (ScheduleType == Schedule.ScheduleType.Week)
            {
                ScheduleType = Schedule.ScheduleType.Month;
            }
        }

        private void SLZoomInCmdExecuted(SfSchedule sfSchedule)
        {
            if (ScheduleType == Schedule.ScheduleType.Month)
            {
                ScheduleType = Schedule.ScheduleType.Week;
            }
            else if (ScheduleType == Schedule.ScheduleType.Week)
            {
                ScheduleType = Schedule.ScheduleType.Day;
            }
        }

        private void SLSameDayNextWeekCmdExecuted(SfSchedule sfSchedule)
        {
            RemoveAppointmentSelection();
            if (this.SelectedAppointment == null)
            {
                if (ScheduleType == Schedule.ScheduleType.Day || ScheduleType == Schedule.ScheduleType.Week || ScheduleType == Schedule.ScheduleType.WorkWeek)
                {
                    if (ScheduleType == Schedule.ScheduleType.Day &&  ScheduleDateRange != null && ScheduleDateRange.Count > 0)
                    {
                        int daysCount = (CurrentSelectedDates[CurrentSelectedDates.Count - 1].Date - CurrentSelectedDates[0].Date).Days + 1;
                        daysview.MoveSelectionToDateTime(Currentselecteddate.AddDays(daysCount));
                    }
                    else
                    {
                        daysview.MoveSelectionToDateTime(Currentselecteddate.AddDays(7));
                    }
                }
                else if (ScheduleType == Schedule.ScheduleType.Month)
                {
                    monthview.SameDayNextMonthSelection(currentitem);
                }
                else if (ScheduleType == Schedule.ScheduleType.TimeLine)
                {
                    timelineview.MoveSelectionRectToDateTime(Currentselecteddate.AddDays(7));
                }
            }
            else if (ScheduleType == Schedule.ScheduleType.Day || ScheduleType == Schedule.ScheduleType.Week || ScheduleType == Schedule.ScheduleType.WorkWeek)
            {
                MoveAppointmentDownCmdExecuted();
            }
        }

        private void SLSameDayPreviousWeekCmdExecuted(SfSchedule sfSchedule)
        {
            RemoveAppointmentSelection();
            if (this.SelectedAppointment == null)
            {
                if (ScheduleType == Schedule.ScheduleType.Day || ScheduleType == Schedule.ScheduleType.Week || ScheduleType == Schedule.ScheduleType.WorkWeek)
                {
                    if (ScheduleType == Schedule.ScheduleType.Day &&  ScheduleDateRange != null && ScheduleDateRange.Count > 0)
                    {
                        int daysCount = (CurrentSelectedDates[CurrentSelectedDates.Count - 1].Date - CurrentSelectedDates[0].Date).Days+1;
                        daysview.MoveSelectionToDateTime(Currentselecteddate.AddDays(-daysCount));
                    }
                    else
                    {
                        daysview.MoveSelectionToDateTime(Currentselecteddate.AddDays(-7));
                    }

                }
                else if (ScheduleType == Schedule.ScheduleType.Month)
                {
                    monthview.SameDayPreviousMonthSelection(currentitem);
                }
                else if (ScheduleType == Schedule.ScheduleType.TimeLine)
                {
                    timelineview.MoveSelectionRectToDateTime(Currentselecteddate.AddDays(-7));
                }
            }
            else if (ScheduleType == Schedule.ScheduleType.Day || ScheduleType == Schedule.ScheduleType.Week || ScheduleType == Schedule.ScheduleType.WorkWeek)
            {
                MoveAppointmentUpCmdExecuted();
            }
        }

        private void SLLastDayCurrentWeekCmdExecuted(SfSchedule sfSchedule)
        {
            RemoveAppointmentSelection();
            DateTime EndDay = Currentselecteddate.StartOfWeek(DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek);
            if (ScheduleType == Schedule.ScheduleType.Day || ScheduleType == Schedule.ScheduleType.Week )
            {
                EndDay = EndDay.AddDays(6).AddHours(Currentselecteddate.Hour).AddMinutes(Currentselecteddate.Minute);
                if (ScheduleType == Schedule.ScheduleType.Day &&  ScheduleDateRange != null && ScheduleDateRange.Count > 0)
                {
                    EndDay = CurrentSelectedDates[CurrentSelectedDates.Count - 1].Date.AddHours(Currentselecteddate.Hour).AddMinutes(Currentselecteddate.Minute);
                }
                daysview.MoveSelectionToDateTime(EndDay);
            }
            else if (ScheduleType == Schedule.ScheduleType.WorkWeek)
            {
                EndDay = CurrentSelectedDates[CurrentSelectedDates.Count - 1].Date.AddHours(Currentselecteddate.Hour).AddMinutes(Currentselecteddate.Minute);
                daysview.MoveSelectionToDateTime(EndDay);
            }
            else if (ScheduleType == Schedule.ScheduleType.Month)
            {
                monthview.WeekLastDaySelection(currentitem);
            }
            else if (ScheduleType == Schedule.ScheduleType.TimeLine)
            {
                EndDay = EndDay.AddDays(6);
                timelineview.MoveSelectionRectToDateTime(EndDay);
            }
        }

        private void SLFirstDayCurrentWeekCmdExecuted(SfSchedule sfSchedule)
        {
            RemoveAppointmentSelection();
            DateTime FirstDay = Currentselecteddate.StartOfWeek(DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek);
            if (ScheduleType == Schedule.ScheduleType.Day || ScheduleType == Schedule.ScheduleType.Week || ScheduleType == Schedule.ScheduleType.WorkWeek)
            {
                FirstDay = FirstDay.AddHours(Currentselecteddate.Hour).AddMinutes(Currentselecteddate.Minute);
                if (ScheduleType == Schedule.ScheduleType.Day && ScheduleDateRange != null && ScheduleDateRange.Count > 0)
                {
                    FirstDay = CurrentSelectedDates[0].Date.AddHours(Currentselecteddate.Hour).AddMinutes(Currentselecteddate.Minute);
                }
                daysview.MoveSelectionToDateTime(FirstDay);
            }
            else if (ScheduleType == Schedule.ScheduleType.Month)
            {
                monthview.WeekFirstDaySelection(currentitem);
            }
            else if (ScheduleType == Schedule.ScheduleType.TimeLine)
            {
                timelineview.MoveSelectionRectToDateTime(FirstDay);
            }
        }

        private void MoveAppointmentUpCmdExecuted()
        {
            stopUpdate = true;
            SelectedAppointment.InternalStartTime = SelectedAppointment.InternalStartTime.AddMinutes(-(GetTimeInterval().TotalMinutes));
            stopUpdate = false;
            SelectedAppointment.InternalEndTime = SelectedAppointment.InternalEndTime.AddMinutes(-(GetTimeInterval().TotalMinutes));
            daysview.MoveSelectionToDateTime(SelectedAppointment.InternalStartTime);
        }

        private void MoveAppointmentDownCmdExecuted()
        {
            stopUpdate = true;
            SelectedAppointment.InternalStartTime = SelectedAppointment.InternalStartTime.AddMinutes((GetTimeInterval().TotalMinutes));
            stopUpdate = false;
            SelectedAppointment.InternalEndTime = SelectedAppointment.InternalEndTime.AddMinutes((GetTimeInterval().TotalMinutes));
            daysview.MoveSelectionToDateTime(SelectedAppointment.InternalStartTime);
        }

        private void SLEndCmdExecuted(SfSchedule sfSchedule)
        {
            RemoveAppointmentSelection();
            TimeSpan interval = GetTimeInterval();
            if (ScheduleType == Schedule.ScheduleType.Day || ScheduleType == Schedule.ScheduleType.Week || ScheduleType == Schedule.ScheduleType.WorkWeek)
            {
                if (IsHighLightWorkingHours)
                {
                    daysview.MoveSelectionToDateTime(Currentselecteddate.Date.AddHours(WorkEndHour).AddMinutes(-interval.TotalMinutes));
                }
            }
            else if (ScheduleType == Schedule.ScheduleType.Month)
            {
                monthview.MonthLastDaySelection(currentitem);
            }
            else if (ScheduleType == Schedule.ScheduleType.TimeLine)
            {
                timelineview.MoveSelectionRectToDateTime(Currentselecteddate.Date.AddHours(WorkEndHour).AddMinutes(-interval.TotalMinutes));
            }
        }

        private void SLHomeCmdExecuted(SfSchedule sfSchedule)
        {
            RemoveAppointmentSelection();
            if (ScheduleType == Schedule.ScheduleType.Day || ScheduleType == Schedule.ScheduleType.Week || ScheduleType == Schedule.ScheduleType.WorkWeek)
            {
                if (IsHighLightWorkingHours)
                {
                    daysview.MoveSelectionToDateTime(Currentselecteddate.Date.AddHours(WorkStartHour));
                }
            }
            else if (ScheduleType == Schedule.ScheduleType.Month)
            {
                monthview.MonthFirstDaySelection(currentitem);
            }
            else if (ScheduleType == Schedule.ScheduleType.TimeLine)
            {
                timelineview.MoveSelectionRectToDateTime(Currentselecteddate.Date.AddHours(WorkStartHour));
            }
        }

        private void SLPgDownCmdExecuted(SfSchedule sfSchedule)
        {
            RemoveAppointmentSelection();
            if (ScheduleType == Schedule.ScheduleType.Day || ScheduleType == Schedule.ScheduleType.Week || ScheduleType == Schedule.ScheduleType.WorkWeek)
            {
                daysview.MoveSelectionPageDown();
            }
            else if (ScheduleType == Schedule.ScheduleType.Month)
            {
                monthview.SameDayNextMonthSelection(currentitem);
            }
        }

        private void SLPgUpCmdExecuted(SfSchedule sfSchedule)
        {
            RemoveAppointmentSelection();
            if (ScheduleType == Schedule.ScheduleType.Day || ScheduleType == Schedule.ScheduleType.Week || ScheduleType == Schedule.ScheduleType.WorkWeek)
            {
                daysview.MoveSelectionPageUp();
            }
            else if (ScheduleType == Schedule.ScheduleType.Month)
            {
                monthview.SameDayPreviousMonthSelection(currentitem);
            }
        }

        private void SLLeftCmdExecuted(SfSchedule sfSchedule)
        {
            RemoveAppointmentSelection();
            if (ScheduleType == Schedule.ScheduleType.Day || ScheduleType == Schedule.ScheduleType.Week || ScheduleType == Schedule.ScheduleType.WorkWeek)
                daysview.MoveLeftSelectionRectangle();
            else if (ScheduleType == Schedule.ScheduleType.Month)
            {
                monthview.MoveLeftSelection(currentitem);
            }
            else if (ScheduleType == Schedule.ScheduleType.TimeLine)
            {
                timelineview.MoveLeftSelectionRect(Currentselecteddate.AddMinutes(-GetTimeInterval().TotalMinutes));
            }
        }

        private void SLRightCmdExecuted(SfSchedule sfSchedule)
        {
            RemoveAppointmentSelection();
            if (ScheduleType == Schedule.ScheduleType.Day || ScheduleType == Schedule.ScheduleType.Week || ScheduleType == Schedule.ScheduleType.WorkWeek)
                daysview.MoveRightSelectionRectangle();
            else if (ScheduleType == Schedule.ScheduleType.Month)
            {
                monthview.MoveRightSelection(currentitem);
            }
            else if (ScheduleType == Schedule.ScheduleType.TimeLine)
            {
                timelineview.MoveSelectionRectToDateTime(Currentselecteddate.AddMinutes(GetTimeInterval().TotalMinutes));
            }
        }
        private void SLDownCmdExecuted(SfSchedule sfSchedule)
        {
            RemoveAppointmentSelection();
            if (ScheduleType == Schedule.ScheduleType.Day || ScheduleType == Schedule.ScheduleType.Week|| ScheduleType == Schedule.ScheduleType.WorkWeek)
                daysview.MoveBottomSelectionRectangle();
            else if (ScheduleType == Schedule.ScheduleType.Month)
            {
                if (Resource != string.Empty && ScheduleResourceType != null)
                {
                    ScheduleMonthViewItemsControl monthitem = currentSelectedItem.FindElementOfType<ScheduleMonthViewItemsControl>();
                    currentitem = monthview.GetCurrentItem(this, monthitem);
                }
                monthview.MoveDownSelection(currentitem);
            }
            else if (ScheduleType == Schedule.ScheduleType.TimeLine)
            {
                timelineview.MoveDownSelectionRect(Currentselecteddate);
            }
        }

        private void SLUpCmdExecuted(SfSchedule sfSchedule)
        {
            RemoveAppointmentSelection();
            if (ScheduleType == Schedule.ScheduleType.Day || ScheduleType == Schedule.ScheduleType.Week || ScheduleType == Schedule.ScheduleType.WorkWeek)
                daysview.MoveTopSelectionRectangle();
            else if (ScheduleType == Schedule.ScheduleType.Month)
            {
                if (Resource != string.Empty && ScheduleResourceType != null)
                {
                    ScheduleMonthViewItemsControl monthitem = currentSelectedItem.FindElementOfType<ScheduleMonthViewItemsControl>();
                    currentitem = monthview.GetCurrentItem(this, monthitem);
                }
                monthview.MoveUpSelection(currentitem);
            }
            else if (ScheduleType == Schedule.ScheduleType.TimeLine)
            {
                timelineview.MoveUpSelectionRect(Currentselecteddate);
            }
        }

        private void SLPasteCmdExecuted(SfSchedule sfSchedule)
        {
            if(CopiedAppointment != null)
            PasteAppointment();
        }

        private void SLCopyCmdExecuted(SfSchedule sfSchedule)
        {
            if (SelectedAppointment != null)
            {
                CopiedAppointment = (ScheduleAppointment)AppointmentCloning(SelectedAppointment);
                CopiedAppointment.ObjectID = SelectedAppointment.GetHashCode();
            }
        }
        private void SLCreateCmdExecuted(SfSchedule sfSchedule)
        {
            AddNewAppointment();
        }

        private void SLDeleteCmdExecuted(SfSchedule sfSchedule)
        {
            if(SelectedAppointment != null)
            DeleteAppointment();
        }
#else
        private void TimeSelectionLeftCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (ScheduleType == Schedule.ScheduleType.Month)
            {
                if (currentitem != null)
                    e.CanExecute = true;
            }
            else if (ScheduleType == Schedule.ScheduleType.TimeLine)
            {
                if (timelineview.RectWidth != 0)
                    e.CanExecute = true;
            }
        }

        private void TimeSelectionLeftCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            RemoveAppointmentSelection();
            if (ScheduleType == Schedule.ScheduleType.Month)
            {
                if (MinMaxSelection == new DateTime())
                    MinMaxSelection = Currentselecteddate;
                MinMaxSelection = MinMaxSelection.AddDays(-1);
                if (SelectedDates.Contains(MinMaxSelection))
                    monthview.MultiDateSelection(currentitem, MinMaxSelection);
            }
            else if (ScheduleType == Schedule.ScheduleType.TimeLine)
            {
                if (MinMaxSelection == new DateTime())
                    MinMaxSelection = Currentselecteddate;
                if (MinMaxSelection > Currentselecteddate)
                {
                    MinMaxSelection = MinMaxSelection.AddMinutes(-(this.GetTimeInterval().TotalMinutes));
                    timelineview.SubSelectionLeft(MinMaxSelection);
                }
                else if (MinMaxSelection <= Currentselecteddate)
                {
                    MinMaxSelection = MinMaxSelection.AddMinutes(-(this.GetTimeInterval().TotalMinutes));
                    timelineview.AddSelectionLeft(MinMaxSelection);
                }
            }
        }

        private void TimeSelectionRightCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (ScheduleType == Schedule.ScheduleType.Month)
            {
                if (currentitem != null)
                    e.CanExecute = true;
            }
            else if (ScheduleType == Schedule.ScheduleType.TimeLine)
            {
                if (timelineview.RectWidth != 0)
                    e.CanExecute = true;
            }
        }

        private void TimeSelectionRightCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            RemoveAppointmentSelection();
            if (ScheduleType == Schedule.ScheduleType.Month)
            {
                //MultiDateSelection(currentitem, appDate);
                if (MinMaxSelection == new DateTime())
                    MinMaxSelection = Currentselecteddate;
                MinMaxSelection = MinMaxSelection.AddDays(1);
                if (SelectedDates.Contains(MinMaxSelection))
                    monthview.MultiDateSelection(currentitem, MinMaxSelection);

            }
            else if (ScheduleType == Schedule.ScheduleType.TimeLine)
            {
                if (MinMaxSelection == new DateTime())
                    MinMaxSelection = Currentselecteddate;
                if (MinMaxSelection >= Currentselecteddate)
                {
                    MinMaxSelection = MinMaxSelection.AddMinutes(this.GetTimeInterval().TotalMinutes);
                    timelineview.AddSelectionRight(MinMaxSelection);
                }
                else if (MinMaxSelection < Currentselecteddate)
                {
                    MinMaxSelection = MinMaxSelection.AddMinutes(this.GetTimeInterval().TotalMinutes);
                    timelineview.SubSelectionRight(MinMaxSelection);
                }
            }
        }

        private void CreateAppOnSelectionCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (ScheduleType == Schedule.ScheduleType.Day || ScheduleType == Schedule.ScheduleType.Week || ScheduleType == Schedule.ScheduleType.WorkWeek)
            {
                if (daysview.RectHeight != 0)
                    e.CanExecute = true;
            }
            else if (ScheduleType == Schedule.ScheduleType.TimeLine)
            {
                if (timelineview.RectWidth != 0)
                    e.CanExecute = true;
            }
            else if (ScheduleType == Schedule.ScheduleType.Month)
            {
                if (currentitem != null)
                    e.CanExecute = true;
            }
        }

        private void CreateAppOnSelectionCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            bool createAppointment = true;
            bool sameTime = false;
            if (MinMaxSelection == new DateTime())
            {
                MinMaxSelection = Currentselecteddate;
                sameTime = true;
            }
            double appStart, appEnd;
            if (MinMaxSelection.TimeOfDay.TotalMinutes < Currentselecteddate.TimeOfDay.TotalMinutes)
            {
                appStart = MinMaxSelection.TimeOfDay.TotalMinutes / 60;
                appEnd = Currentselecteddate.TimeOfDay.TotalMinutes / 60;
            }
            else if (sameTime)
            {
                appStart = Currentselecteddate.TimeOfDay.TotalMinutes / 60;
                appEnd = Currentselecteddate.TimeOfDay.TotalMinutes / 60;
            }
            else
            {
                appStart = Currentselecteddate.TimeOfDay.TotalMinutes / 60;
                appEnd = MinMaxSelection.TimeOfDay.TotalMinutes / 60;
            }
            if (NonAccessibleBlocks.Count > 0)
            {
                foreach (NonAccessibleBlock block in NonAccessibleBlocks)
                {
                    if ((block.StartHour < appStart && block.EndHour > appStart) || (block.StartHour < appEnd && block.EndHour > appEnd))
                    {
                        createAppointment = false;
                        break;
                    }
                }
            }
           
            if (SelectedAppointment == null )
            {
                if(createAppointment)
                {
                    if (ScheduleType == Schedule.ScheduleType.Day || ScheduleType == Schedule.ScheduleType.Week || ScheduleType == Schedule.ScheduleType.WorkWeek)
                    {
                        VisibleTextbox();
                    }
                    else if (ScheduleType == Schedule.ScheduleType.Month)
                    {
                        CreateAppointmentOnSelection();
                    }
                    else if (ScheduleType == Schedule.ScheduleType.TimeLine)
                    {
                        VisibleTextbox();
                    }
                }
            }
            else
            {
                EditAppointment();
            }

        }

        private void TimeSelectionDownCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (ScheduleType == Schedule.ScheduleType.Day || ScheduleType == Schedule.ScheduleType.WorkWeek || ScheduleType == Schedule.ScheduleType.Week || ScheduleType == Schedule.ScheduleType.Month)
                e.CanExecute = true;
        }

        private void TimeSelectionfDownCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            RemoveAppointmentSelection();
            if (ScheduleType == Schedule.ScheduleType.Day || ScheduleType == Schedule.ScheduleType.Week || ScheduleType == Schedule.ScheduleType.WorkWeek)
            {
                if (MinMaxSelection == new DateTime())
                    MinMaxSelection = Currentselecteddate;
                if (MinMaxSelection >= Currentselecteddate)
                {
                    MinMaxSelection = MinMaxSelection.AddMinutes(this.GetTimeInterval().TotalMinutes);
                    daysview.AddSelectionDown(MinMaxSelection);
                }
                else if (MinMaxSelection < Currentselecteddate)
                {
                    MinMaxSelection = MinMaxSelection.AddMinutes(this.GetTimeInterval().TotalMinutes);
                    daysview.SubSelectionDown(MinMaxSelection);
                }
            }
            else if (ScheduleType == Schedule.ScheduleType.Month)
            {
                if (MinMaxSelection == new DateTime())
                    MinMaxSelection = Currentselecteddate;
                MinMaxSelection = MinMaxSelection.AddDays(7);
                if (SelectedDates.Contains(MinMaxSelection))
                    monthview.MultiDateSelection(currentitem, MinMaxSelection);
            }
        }

        private void TimeSelectionUpCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (ScheduleType == Schedule.ScheduleType.Day || ScheduleType == Schedule.ScheduleType.Week || ScheduleType == Schedule.ScheduleType.WorkWeek || ScheduleType == Schedule.ScheduleType.Month)
                e.CanExecute = true;
        }

        private void TimeSelectionfUpCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            RemoveAppointmentSelection();
            if (ScheduleType == Schedule.ScheduleType.Day || ScheduleType == Schedule.ScheduleType.Week || ScheduleType == Schedule.ScheduleType.WorkWeek)
            {
                if (MinMaxSelection == new DateTime())
                    MinMaxSelection = Currentselecteddate;
                if (MinMaxSelection > Currentselecteddate)
                {
                    MinMaxSelection = MinMaxSelection.AddMinutes(-(this.GetTimeInterval().TotalMinutes));
                    daysview.SubSelectionUp(MinMaxSelection);
                }
                else if (MinMaxSelection <= Currentselecteddate)
                {
                    MinMaxSelection = MinMaxSelection.AddMinutes(-(this.GetTimeInterval().TotalMinutes));
                    daysview.AddSelectionUp(MinMaxSelection);
                }
            }
            else if (ScheduleType == Schedule.ScheduleType.Month)
            {
                if (MinMaxSelection == new DateTime())
                    MinMaxSelection = Currentselecteddate;
                MinMaxSelection = MinMaxSelection.AddDays(-7);
                if (SelectedDates.Contains(MinMaxSelection))
                    monthview.MultiDateSelection(currentitem, MinMaxSelection);
            }
        }

        private void ResizeAppointmentDownCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (SelectedAppointment != null && (ScheduleType != Schedule.ScheduleType.Month))
                e.CanExecute = true;
        }

        private void ResizeAppointmentDownCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (SelectedAppointment != null)
            {
                if (ScheduleType == Schedule.ScheduleType.Day || ScheduleType == Schedule.ScheduleType.Week || ScheduleType == Schedule.ScheduleType.WorkWeek)
                {
                    daysview.MoveSelectionToDateTime(SelectedAppointment.InternalEndTime);
                }
                else if (ScheduleType == Schedule.ScheduleType.TimeLine)
                {
                    timelineview.MoveSelectionRectToDateTime(SelectedAppointment.InternalStartTime);
                }
                SelectedAppointment.InternalEndTime = SelectedAppointment.InternalEndTime.AddMinutes(GetTimeInterval().TotalMinutes);
            }
        }

        private void ResizeAppointmentUpCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (SelectedAppointment != null && (ScheduleType != Schedule.ScheduleType.Month))
                e.CanExecute = true;
        }

        private void ResizeAppointmentUpCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (SelectedAppointment != null)
            {
                SelectedAppointment.InternalStartTime = SelectedAppointment.InternalStartTime.AddMinutes(-(GetTimeInterval().TotalMinutes));
                if (ScheduleType == Schedule.ScheduleType.Day || ScheduleType == Schedule.ScheduleType.Week || ScheduleType == Schedule.ScheduleType.WorkWeek)
                {
                    daysview.MoveSelectionToDateTime(SelectedAppointment.InternalStartTime);
                }
                else if (ScheduleType == Schedule.ScheduleType.TimeLine)
                {
                    timelineview.MoveSelectionRectToDateTime(SelectedAppointment.InternalStartTime);
                }
            }
        }

        private void NextAppointmentCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (Appointments.Count > 0 || (daysview != null && daysview.AppTextVisibility == System.Windows.Visibility.Visible) || (timelineview != null && timelineview.TimelineAppTextVisibility == System.Windows.Visibility.Visible))
                e.CanExecute = true;
        }

        private void NextAppointmentCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (ScheduleType == Schedule.ScheduleType.Day || ScheduleType == Schedule.ScheduleType.Week || ScheduleType == Schedule.ScheduleType.WorkWeek)
            {
                if (daysview.AppTextVisibility == System.Windows.Visibility.Visible)
                    CreateAppointmentOnSelection();
            }
            else if (ScheduleType == Schedule.ScheduleType.TimeLine)
            {
                if (timelineview.TimelineAppTextVisibility == System.Windows.Visibility.Visible)
                    CreateAppointmentOnSelection();
            }
            ScheduleAppointment nextapp=SelectNextAppointment();
            if (nextapp != null)
            {
                DateTime appDate = nextapp.InternalStartTime;
                if (Resource != string.Empty && ScheduleResourceType != null && ScheduleType == Schedule.ScheduleType.Month)
                {
                    Resource resrc = SelectedAppointment.ResourceCollection.FirstOrDefault(res => res.TypeName == Resource);
                    if (resrc != null)
                    {
                        Resource nextresource= ScheduleResourceType.ResourceCollection.FirstOrDefault(Res => Res.ResourceName == resrc.ResourceName);
                        if (nextresource != null)
                        {
                            int nextItemIndex=ScheduleResourceType.ResourceCollection.IndexOf(nextresource);
                           ScheduleMonthViewItemsControl currentItemsContainer = (monthview.Resourcecontainer.Items[nextItemIndex] as FrameworkElement).FindElementOfType<ScheduleMonthViewItemsControl>();
                            if(currentItemsContainer != null && currentitem.FindParentElementOfType<ScheduleMonthViewItemsControl>() != currentItemsContainer)
                            {
                                var backgroundBinding = new Binding { Source = (viewcontrol.Content as ScheduleMonthView) };
                                if (!currentitem.IsCurrentDate)
                                {
                                    backgroundBinding.Path = currentitem.IsCurrentMonth ? new PropertyPath("FocusedMonth") : new PropertyPath("NonFocusedMonth");
                                    BindingOperations.SetBinding(currentitem, BackgroundProperty, backgroundBinding);
                                }
                                currentitem = currentItemsContainer.Items[0] as ScheduleMonthDateContentControl;
                            }
                        }
                    }
                }
                if (ScheduleType == Schedule.ScheduleType.Day || ScheduleType == Schedule.ScheduleType.Week || ScheduleType == Schedule.ScheduleType.WorkWeek)
                {
                    if (!SelectedAppointment.AllDay)
                        daysview.MoveSelectionToDateTime(appDate);
                }
                else if (ScheduleType == Schedule.ScheduleType.Month)
                {
                    monthview.MoveToSelectedMonthDate(currentitem, appDate);
                }
                else if (ScheduleType == Schedule.ScheduleType.TimeLine)
                {
                    if (!SelectedAppointment.AllDay)
                        timelineview.MoveSelectionRectToDateTime(appDate);
                }
            }
        }

        private void PreviousAppointmentCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (Appointments.Count > 0)
                e.CanExecute = true;
        }

        private void PreviousAppointmentCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (ScheduleType == Schedule.ScheduleType.Day || ScheduleType == Schedule.ScheduleType.Week || ScheduleType == Schedule.ScheduleType.WorkWeek)
            {
                if (daysview.AppTextVisibility == System.Windows.Visibility.Visible)
                    CreateAppointmentOnSelection();
            }
            else if (ScheduleType == Schedule.ScheduleType.TimeLine)
            {
                if (timelineview.TimelineAppTextVisibility == System.Windows.Visibility.Visible)
                    CreateAppointmentOnSelection();
            }
             ScheduleAppointment prevapp=SelectPreviousAppointment();
             if (prevapp != null)
             {
                 DateTime appDate = prevapp.InternalStartTime;
                 if (Resource != string.Empty && ScheduleResourceType != null && ScheduleType == Schedule.ScheduleType.Month)
                 {
                     Resource resrc = SelectedAppointment.ResourceCollection.FirstOrDefault(res => res.TypeName == Resource);
                     if (resrc != null)
                     {
                         Resource nextresource = ScheduleResourceType.ResourceCollection.FirstOrDefault(Res => Res.ResourceName == resrc.ResourceName);
                         if (nextresource != null)
                         {
                             int nextItemIndex = ScheduleResourceType.ResourceCollection.IndexOf(nextresource);
                             ScheduleMonthViewItemsControl currentItemsContainer = (monthview.Resourcecontainer.Items[nextItemIndex] as FrameworkElement).FindElementOfType<ScheduleMonthViewItemsControl>();
                             if (currentItemsContainer != null && currentitem.FindParentElementOfType<ScheduleMonthViewItemsControl>() != currentItemsContainer)
                             {
                                 var backgroundBinding = new Binding { Source = (viewcontrol.Content as ScheduleMonthView) };
                                 if (!currentitem.IsCurrentDate)
                                 {
                                     backgroundBinding.Path = currentitem.IsCurrentMonth ? new PropertyPath("FocusedMonth") : new PropertyPath("NonFocusedMonth");
                                     BindingOperations.SetBinding(currentitem, BackgroundProperty, backgroundBinding);
                                 }
                                 currentitem = currentItemsContainer.Items[0] as ScheduleMonthDateContentControl;
                             }
                         }
                     }
                 }
                 if (ScheduleType == Schedule.ScheduleType.Day || ScheduleType == Schedule.ScheduleType.Week || ScheduleType == Schedule.ScheduleType.WorkWeek)
                 {
                     if (!SelectedAppointment.AllDay)
                         daysview.MoveSelectionToDateTime(appDate);
                 }
                 else if (ScheduleType == Schedule.ScheduleType.Month)
                 {
                     monthview.MoveToSelectedMonthDate(currentitem, appDate);
                 }
                 else if (ScheduleType == Schedule.ScheduleType.TimeLine)
                 {
                     if (!SelectedAppointment.AllDay)
                         timelineview.MoveSelectionRectToDateTime(appDate);
                 }
             }
        }

        private void ZoomOutCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void ZoomOutCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            string oldvalue = string.Empty;
            string newvalue = string.Empty;
            if (ScheduleType == Schedule.ScheduleType.Day)
            {
                ScheduleType = Schedule.ScheduleType.Week;
            }
            else if (ScheduleType == Schedule.ScheduleType.Week)
            {
                ScheduleType = Schedule.ScheduleType.Month;
            }
            this.Focus();
        }

        private void ZoomInCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void ZoomInCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            string oldvalue = string.Empty;
            string newvalue = string.Empty;
            if (ScheduleType == Schedule.ScheduleType.Month)
            {
                ScheduleType = Schedule.ScheduleType.Week;
            }
            else if (ScheduleType == Schedule.ScheduleType.Week)
            {
                ScheduleType = Schedule.ScheduleType.Day;
            }
            this.Focus();
        }

        private void SameDayNextWeekCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void SameDayNextWeekCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            RemoveAppointmentSelection();
            if (this.SelectedAppointment == null)
            {
                if (ScheduleType == Schedule.ScheduleType.Day || ScheduleType == Schedule.ScheduleType.Week || ScheduleType == Schedule.ScheduleType.WorkWeek)
                {
                    if (ScheduleType == Schedule.ScheduleType.Day && ScheduleDateRange != null && ScheduleDateRange.Count > 0)
                    {
                        int daysCount = (CurrentSelectedDates[CurrentSelectedDates.Count - 1].Date - CurrentSelectedDates[0].Date).Days + 1;
                        daysview.MoveSelectionToDateTime(Currentselecteddate.AddDays(daysCount));
                    }
                    else
                    {
                        daysview.MoveSelectionToDateTime(Currentselecteddate.AddDays(7));
                    }
                }
                else if (ScheduleType == Schedule.ScheduleType.Month)
                {
                    monthview.SameDayNextMonthSelection(currentitem);
                }
                else if (ScheduleType == Schedule.ScheduleType.TimeLine)
                {
                    timelineview.MoveSelectionRectToDateTime(Currentselecteddate.AddDays(7));
                }
            }
            else if (ScheduleType == Schedule.ScheduleType.Day || ScheduleType == Schedule.ScheduleType.Week || ScheduleType == Schedule.ScheduleType.WorkWeek)
            {
                MoveAppointmentDownCmdExecuted(sender, e);
            }
        }

        private void SameDayPreviousWeekCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void SameDayPreviousWeekCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            RemoveAppointmentSelection();
            if (this.SelectedAppointment == null)
            {
                if (ScheduleType == Schedule.ScheduleType.Day || ScheduleType == Schedule.ScheduleType.Week || ScheduleType == Schedule.ScheduleType.WorkWeek)
                {
                    if (ScheduleType == Schedule.ScheduleType.Day && ScheduleDateRange != null && ScheduleDateRange.Count > 0)
                    {
                        int daysCount = (CurrentSelectedDates[CurrentSelectedDates.Count - 1].Date - CurrentSelectedDates[0].Date).Days + 1;
                        daysview.MoveSelectionToDateTime(Currentselecteddate.AddDays(-daysCount));
                    }
                    else
                    {
                        daysview.MoveSelectionToDateTime(Currentselecteddate.AddDays(-7));
                    }
                }
                else if (ScheduleType == Schedule.ScheduleType.Month)
                {
                    monthview.SameDayPreviousMonthSelection(currentitem);
                }
                else if (ScheduleType == Schedule.ScheduleType.TimeLine)
                {
                    timelineview.MoveSelectionRectToDateTime(Currentselecteddate.AddDays(-7));
                }
            }
            else if (ScheduleType == Schedule.ScheduleType.Day || ScheduleType == Schedule.ScheduleType.Week || ScheduleType == Schedule.ScheduleType.WorkWeek)
            {
                MoveAppointmentUpCmdExecuted(sender, e);
            }
        }

        private void MoveAppointmentUpCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            stopUpdate = true;
            SelectedAppointment.InternalStartTime = SelectedAppointment.InternalStartTime.AddMinutes(-(GetTimeInterval().TotalMinutes));
            stopUpdate = false;
            SelectedAppointment.InternalEndTime = SelectedAppointment.InternalEndTime.AddMinutes(-(GetTimeInterval().TotalMinutes));
            daysview.MoveSelectionToDateTime(SelectedAppointment.InternalStartTime);
        }

        private void MoveAppointmentDownCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            stopUpdate = true;
            SelectedAppointment.InternalStartTime = SelectedAppointment.InternalStartTime.AddMinutes((GetTimeInterval().TotalMinutes));
            stopUpdate = false;
            SelectedAppointment.InternalEndTime = SelectedAppointment.InternalEndTime.AddMinutes((GetTimeInterval().TotalMinutes));
            daysview.MoveSelectionToDateTime(SelectedAppointment.InternalStartTime);
        }

        private void LastDayCurrentWeekCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void LastDayCurrentWeekCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            RemoveAppointmentSelection();
            DateTime EndDay = Currentselecteddate.StartOfWeek(DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek);
            if (ScheduleType == Schedule.ScheduleType.Day || ScheduleType == Schedule.ScheduleType.Week )
            {
                EndDay = EndDay.AddDays(6).AddHours(Currentselecteddate.Hour).AddMinutes(Currentselecteddate.Minute);
                if (ScheduleType == Schedule.ScheduleType.Day && ScheduleDateRange != null && ScheduleDateRange.Count > 0)
                {
                    EndDay = CurrentSelectedDates[0].Date.AddHours(Currentselecteddate.Hour).AddMinutes(Currentselecteddate.Minute);
                }
                daysview.MoveSelectionToDateTime(EndDay);
            }
            else if (ScheduleType == Schedule.ScheduleType.WorkWeek)
            {
                EndDay = CurrentSelectedDates[CurrentSelectedDates.Count-1].Date.AddHours(Currentselecteddate.Hour).AddMinutes(Currentselecteddate.Minute);
                daysview.MoveSelectionToDateTime(EndDay);
            }
            else if (ScheduleType == Schedule.ScheduleType.Month)
            {
                monthview.WeekLastDaySelection(currentitem);
            }
            else if (ScheduleType == Schedule.ScheduleType.TimeLine)
            {
                EndDay = EndDay.AddDays(6);
                timelineview.MoveSelectionRectToDateTime(EndDay);
            }
        }

        private void FirstDayCurrentWeekCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void FirstDayCurrentWeekCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            RemoveAppointmentSelection();
            DateTime FirstDay = Currentselecteddate.StartOfWeek(DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek);
            if (ScheduleType == Schedule.ScheduleType.Day || ScheduleType == Schedule.ScheduleType.Week || ScheduleType == Schedule.ScheduleType.WorkWeek)
            {
                FirstDay = FirstDay.AddHours(Currentselecteddate.Hour).AddMinutes(Currentselecteddate.Minute);
                if (ScheduleType == Schedule.ScheduleType.Day && ScheduleDateRange != null && ScheduleDateRange.Count > 0)
                {
                    FirstDay = CurrentSelectedDates[0].Date.AddHours(Currentselecteddate.Hour).AddMinutes(Currentselecteddate.Minute);
                }
                daysview.MoveSelectionToDateTime(FirstDay);
            }
            else if (ScheduleType == Schedule.ScheduleType.Month)
            {
                monthview.WeekFirstDaySelection(currentitem);
            }
            else if (ScheduleType == Schedule.ScheduleType.TimeLine)
            {
                timelineview.MoveSelectionRectToDateTime(FirstDay);
            }
        }

        private void EndCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void EndCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            RemoveAppointmentSelection();
            TimeSpan interval = GetTimeInterval();
            if (ScheduleType == Schedule.ScheduleType.Day || ScheduleType == Schedule.ScheduleType.Week || ScheduleType == Schedule.ScheduleType.WorkWeek)
            {
                if (IsHighLightWorkingHours)
                {
                    daysview.MoveSelectionToDateTime(Currentselecteddate.Date.AddHours(WorkEndHour).AddMinutes(-interval.TotalMinutes));
                }
            }
            else if (ScheduleType == Schedule.ScheduleType.Month)
            {
                monthview.MonthLastDaySelection(currentitem);
            }
            else if (ScheduleType == Schedule.ScheduleType.TimeLine)
            {
                timelineview.MoveSelectionRectToDateTime(Currentselecteddate.Date.AddHours(WorkEndHour).AddMinutes(-interval.TotalMinutes));
            }
        }

        private void HomeCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void HomeCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            RemoveAppointmentSelection();
            if (ScheduleType == Schedule.ScheduleType.Day || ScheduleType == Schedule.ScheduleType.Week || ScheduleType == Schedule.ScheduleType.WorkWeek)
            {
                if (IsHighLightWorkingHours)
                {
                    daysview.MoveSelectionToDateTime(Currentselecteddate.Date.AddHours(WorkStartHour));
                }
            }
            else if (ScheduleType == Schedule.ScheduleType.Month)
            {
                monthview.MonthFirstDaySelection(currentitem);
            }
            else if (ScheduleType == Schedule.ScheduleType.TimeLine)
            {
                timelineview.MoveSelectionRectToDateTime(Currentselecteddate.Date.AddHours(WorkStartHour));
            }

        }

        private void PgDownCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void PgDownCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            RemoveAppointmentSelection();
            if (ScheduleType == Schedule.ScheduleType.Day || ScheduleType == Schedule.ScheduleType.Week || ScheduleType == Schedule.ScheduleType.WorkWeek)
            {
                daysview.MoveSelectionPageDown();
            }
            else if (ScheduleType == Schedule.ScheduleType.Month)
            {
                monthview.SameDayNextMonthSelection(currentitem);
            }
        }

        private void PgUpCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void PgUpCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            RemoveAppointmentSelection();
            if (ScheduleType == Schedule.ScheduleType.Day || ScheduleType == Schedule.ScheduleType.Week || ScheduleType == Schedule.ScheduleType.WorkWeek)
            {
                daysview.MoveSelectionPageUp();
            }
            else if (ScheduleType == Schedule.ScheduleType.Month)
            {
                monthview.SameDayPreviousMonthSelection(currentitem);
            }
        }

        private void LeftCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void LeftCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            RemoveAppointmentSelection();
            if (ScheduleType == Schedule.ScheduleType.Day || ScheduleType == Schedule.ScheduleType.Week || ScheduleType == Schedule.ScheduleType.WorkWeek)
                daysview.MoveLeftSelectionRectangle();
            else if (ScheduleType == Schedule.ScheduleType.Month)
            {
                monthview.MoveLeftSelection(currentitem);
            }
            else if (ScheduleType == Schedule.ScheduleType.TimeLine)
            {
                timelineview.MoveLeftSelectionRect(Currentselecteddate.AddMinutes(-GetTimeInterval().TotalMinutes));
            }
        }

        private void RightCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void RightCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            RemoveAppointmentSelection();
            if (ScheduleType == Schedule.ScheduleType.Day || ScheduleType == Schedule.ScheduleType.Week || ScheduleType == Schedule.ScheduleType.WorkWeek)
                daysview.MoveRightSelectionRectangle();
            else if (ScheduleType == Schedule.ScheduleType.Month)
            {
                monthview.MoveRightSelection(currentitem);
            }
            else if (ScheduleType == Schedule.ScheduleType.TimeLine)
            {
                timelineview.MoveSelectionRectToDateTime(Currentselecteddate.AddMinutes(GetTimeInterval().TotalMinutes));
            }
        }

        private void DownCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void DownCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            RemoveAppointmentSelection();
            if (ScheduleType == Schedule.ScheduleType.Day || ScheduleType == Schedule.ScheduleType.Week || ScheduleType == Schedule.ScheduleType.WorkWeek)
                daysview.MoveBottomSelectionRectangle();
            else if (ScheduleType == Schedule.ScheduleType.Month)
            {
                if (Resource != string.Empty && ScheduleResourceType != null)
                {
                    ScheduleMonthViewItemsControl monthViewItemsControl = currentSelectedItem.FindElementOfType<ScheduleMonthViewItemsControl>();
                    currentitem = monthview.GetCurrentItem(this, monthViewItemsControl);
                }
                monthview.MoveDownSelection(currentitem);
            }
            else if (ScheduleType == Schedule.ScheduleType.TimeLine)
            {
                timelineview.MoveDownSelectionRect(Currentselecteddate);
            }
        }

        private void UpCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void UpCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            RemoveAppointmentSelection();
            if (ScheduleType == Schedule.ScheduleType.Day || ScheduleType == Schedule.ScheduleType.Week || ScheduleType == Schedule.ScheduleType.WorkWeek)
                daysview.MoveTopSelectionRectangle();
            else if (ScheduleType == Schedule.ScheduleType.Month)
            {
                if (Resource != string.Empty && ScheduleResourceType != null)
                {
                    ScheduleMonthViewItemsControl monthitem = currentSelectedItem.FindElementOfType<ScheduleMonthViewItemsControl>();
                    currentitem = monthview.GetCurrentItem(this, monthitem);
                }
                monthview.MoveUpSelection(currentitem);
            }
            else if (ScheduleType == Schedule.ScheduleType.TimeLine)
            {
                timelineview.MoveUpSelectionRect(Currentselecteddate);
            }
        }

        private void PasteCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (CopiedAppointment != null)
                e.CanExecute = true;
            else
                e.CanExecute = false;
        }

        private void PasteCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            PasteAppointment();
        }

        private void CopyCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (SelectedAppointment != null)
                e.CanExecute = true;
            else
                e.CanExecute = false;
        }

        private void CopyCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            CopiedAppointment = (ScheduleAppointment)AppointmentCloning(SelectedAppointment);
            CopiedAppointment.ObjectID = SelectedAppointment.GetHashCode();
        }

        private void CreateCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CreateCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            AddNewAppointment();
        }

        private void DeleteCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (SelectedAppointment != null)
                e.CanExecute = true;
            else
                e.CanExecute = false;
        }

        private void DeleteCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            DeleteAppointment();
        }
#endif

        #endregion
#if WPF
        #region Update First and Last Items

        internal void UpdateLastItem(int selecteditem)
        {
            viewcontrol = VisualTreeHelper.GetChild((flipview.Items[selecteditem] as Grid), 1) as ContentControl;
            flipviewselecteditem = (flipview.Items[selecteditem] as Grid);
            if (DragDropCanvas != null)
            {
                DragDropCanvas.Clip = new RectangleGeometry { Rect = new Rect(0, ActualHeight - viewcontrol.ActualHeight, viewcontrol.ActualWidth, viewcontrol.ActualHeight) };
            }
            HeaderTitleBarView titlebar = null;
            HeaderTitleBarView curr_titlebar = null;
            Removeditem = null;
            int removedindex = 0;
            switch (selecteditem)
            {
                case 0:
                    removedindex = 2;
                    break;
                case 1:
                    removedindex = 0;
                    break;
                case 2:
                    removedindex = 1;
                    break;
            }
            titlebar = ((FrameworkElement)flipview.Items[removedindex]).FindElementOfType<HeaderTitleBarView>();
            curr_titlebar = ((FrameworkElement)flipview.Items[selecteditem]).FindElementOfType<HeaderTitleBarView>();
            Removeditem = flipview.Items[removedindex] as Grid;
            if (ScheduleType == ScheduleType.Day || ScheduleType == ScheduleType.Week || ScheduleType == Schedule.ScheduleType.WorkWeek)
            {
                if (Removeditem != null)
                {
                    var contentControl = Removeditem.Children[1] as ContentControl;
                    if (contentControl != null)
                    {
                        var daysview = contentControl.Content as ScheduleDaysView;
                        if (daysview != null)

                            daysview.scrollviewer.ScrollToHorizontalOffset(0d);

                    }
                }
                var scheduleDaysView = viewcontrol.Content as ScheduleDaysView;
                if (scheduleDaysView != null)
                    CurrentVisibleSelectedDates = scheduleDaysView.SelectedDates;

                if (this.CurrentVisibleSelectedDates.Contains(this.SelectedDate.Date))
                {
                    scheduleDaysView.RectVisibility = Visibility.Visible;
                }
                else
                {
                    scheduleDaysView.RectVisibility = Visibility.Collapsed;
                }
            }
            else if (ScheduleType == ScheduleType.Month)
            {
                var obj = VisualTreeHelper.GetChild(viewcontrol.Content as ScheduleMonthView, 0) as Grid;
                if (obj != null)
                {
                    var MonthScrollviewer = obj.FindName("ResourceScrollViewer") as ScrollViewer;
                    if (MonthScrollviewer != null)

                        MonthScrollviewer.ScrollToVerticalOffset(0d);

                }
                var scheduleMonthView = viewcontrol.Content as ScheduleMonthView;
                if (scheduleMonthView != null)
                    CurrentVisibleSelectedDates = scheduleMonthView.SelectedDates;
            }
            else
            {
                var scheduleTimeLineView = viewcontrol.Content as ScheduleTimeLineView;
                if (scheduleTimeLineView != null)
                    CurrentVisibleSelectedDates = scheduleTimeLineView.SelectedDates;
                if (this.CurrentVisibleSelectedDates.Contains(this.SelectedDate.Date))
                {
                    scheduleTimeLineView.RectVisibility = Visibility.Visible;
                }
                else
                {
                    scheduleTimeLineView.RectVisibility = Visibility.Collapsed;
                }
            }
            IsVisibleDateSetInternally = true;
            VisibleDates = CurrentVisibleSelectedDates;
            IsVisibleDateSetInternally = false;
            //VisibleDateChanged = true;
            if (curr_titlebar != null)
            {
#if WPF
                if (EnableTouch)
                    UpdatePrevTouchItem(curr_titlebar.SelectedDates, titlebar.SelectedDates);
                else
#endif
                    UpdatePrevItem();
            }
            //VisibleDateChanged = false;
            SetNavigationTap();
            ClearContextmenu();
        }
        internal void UpdateFirstItem(int selecteditem)
        {
            
            flipviewselecteditem = (flipview.Items[selecteditem] as Grid);
            viewcontrol = VisualTreeHelper.GetChild(flipviewselecteditem, 1) as ContentControl;
            if (DragDropCanvas != null)
            {
                DragDropCanvas.Clip = new RectangleGeometry { Rect = new Rect(0, ActualHeight - viewcontrol.ActualHeight, viewcontrol.ActualWidth, viewcontrol.ActualHeight) };
            }
            HeaderTitleBarView titlebar = null;
            HeaderTitleBarView curr_titlebar = null;
            Removeditem = null;
            int removedindex = 0;
            switch (selecteditem)
            {
                case 0:
                    removedindex = 1;
                    break;
                case 1:
                    removedindex = 2;
                    break;
                case 2:
                    removedindex = 0;
                    break;
            }
            titlebar = VisualTreeHelper.GetChild((flipview.Items[removedindex] as Grid), 0) as HeaderTitleBarView;
            curr_titlebar = VisualTreeHelper.GetChild((flipview.Items[selecteditem] as Grid), 0) as HeaderTitleBarView;
            Removeditem = flipview.Items[removedindex] as Grid;
            removedControl = VisualTreeHelper.GetChild(Removeditem, 1) as ContentControl;
            if (ScheduleType == ScheduleType.Day || ScheduleType == ScheduleType.Week|| ScheduleType == ScheduleType.WorkWeek)
            {
                if (Removeditem != null)
                {
                    var contentControl = Removeditem.Children[1] as ContentControl;
                    if (contentControl != null)
                    {
                        var daysview = contentControl.Content as ScheduleDaysView;
                        if (daysview != null)
                        {
                            if (!IsHighLightWorkingHours)
                            {

                                daysview.scrollviewer.ScrollToVerticalOffset(0d);

                            }

                            daysview.scrollviewer.ScrollToHorizontalOffset(0d);

                        }
                    }
                }
                var scheduleDaysView = viewcontrol.Content as ScheduleDaysView;
                if (scheduleDaysView != null)
                    CurrentVisibleSelectedDates = scheduleDaysView.SelectedDates;
                if (this.CurrentVisibleSelectedDates.Contains(this.SelectedDate.Date))
                {
                    scheduleDaysView.RectVisibility = Visibility.Visible;
                }
                else
                {
                    scheduleDaysView.RectVisibility = Visibility.Collapsed;
                }
            }
            else if (ScheduleType == ScheduleType.Month)
            {
                if (Removeditem != null)
                {
                    var contentControl = Removeditem.Children[1] as ContentControl;
                    if (contentControl != null)
                    {
                        var obj = VisualTreeHelper.GetChild(contentControl.Content as ScheduleMonthView, 0) as Grid;
                        if (obj != null)
                        {
                            var MonthScrollviewer = obj.FindName("ResourceScrollViewer") as ScrollViewer;
                            if (MonthScrollviewer != null)

                                MonthScrollviewer.ScrollToVerticalOffset(0d);

                        }
                    }
                }
                var scheduleMonthView = viewcontrol.Content as ScheduleMonthView;
                if (scheduleMonthView != null)
                    CurrentVisibleSelectedDates = scheduleMonthView.SelectedDates;
            }
            else
            {
                if (Removeditem != null)
                {
                    var contentControl = Removeditem.Children[1] as ContentControl;
                    if (contentControl != null)
                    {
                        var obj = VisualTreeHelper.GetChild(contentControl.Content as ScheduleTimeLineView, 0) as Grid;
                        if (obj != null)
                        {
                            var TimeLineScrollviewer = obj.FindName("PART_HorizontalTimeSlotScrollViewer") as ScrollViewer;
                            if (TimeLineScrollviewer != null)

                                TimeLineScrollviewer.ScrollToHorizontalOffset(0d);

                        }
                    }
                }
                var scheduleTimeLineView = viewcontrol.Content as ScheduleTimeLineView;
                if (scheduleTimeLineView != null)
                    CurrentVisibleSelectedDates = scheduleTimeLineView.SelectedDates;
                if (this.CurrentVisibleSelectedDates.Contains(this.SelectedDate.Date))
                {
                    scheduleTimeLineView.RectVisibility = Visibility.Visible;
                }
                else
                {
                    scheduleTimeLineView.RectVisibility = Visibility.Collapsed;
                }
            }
            IsVisibleDateSetInternally = true;
            VisibleDates = CurrentVisibleSelectedDates;
            IsVisibleDateSetInternally = false;
            //VisibleDateChanged = true;
            if (curr_titlebar != null)
            {
#if WPF
                if (EnableTouch)
                    UpdateNextTouchItem(curr_titlebar.SelectedDates, titlebar.SelectedDates);
                else
#endif
                    UpdateNextItem();
                //UpdateNextTouchItem((viewcontrol.Content as ScheduleDaysView).SelectedDates, (removedControl.Content as ScheduleDaysView).SelectedDates);
            }
            //VisibleDateChanged = false;
            SetNavigationTap();
            ClearContextmenu();
        }


        #endregion
#endif
        #region Dispose

        public void Dispose()
        {
            if (Appointments != null)
            {
                Appointments.CollectionChanged -= Appointments_CollectionChanged;
                foreach (ScheduleAppointment app in Appointments)
                {
                    app.PropertyChanged -= item_PropertyChanged;
                }
                Appointments.Clear();
                Appointments = null;
            }
            if (daysview != null)
                daysview.Dispose();
            if (monthview != null)
                monthview.Dispose();
            if (timelineview != null)
                timelineview.Dispose();
            if (ItemsSource != null)
            {
                UnwireItemSource(ItemsSource as IEnumerable);
                ItemsSource = null;
            }
            if (ScheduleDateRange != null)
            {
                ScheduleDateRange.CollectionChanged -= ScheduleDateRange_CollectionChanged;
                ScheduleDateRange.Clear();
                ScheduleDateRange = null;
            }
            if (NonAccessibleBlocks != null)
            {
                NonAccessibleBlocks.CollectionChanged -= NonAccessibleBlocks_CollectionChanged;
                NonAccessibleBlocks.Clear();
                NonAccessibleBlocks = null;
            }
            if (ScheduleResourceTypeCollection != null)
            {
                ScheduleResourceTypeCollection.CollectionChanged -= ScheduleResourceTypeCollection_CollectionChanged;
                ScheduleResourceTypeCollection.Clear();
                ScheduleResourceTypeCollection = null;
            }
            if (ItemsSource != null)
            {
                UnwireItemSource(ItemsSource as IEnumerable);
                ItemsSource = null;
            }
#if SILVERLIGHT
            if(appointmentEditor != null)
            {
                    appointmentEditor.Closed -= appointmentEditor_Closed;
                    appointmentEditor.Show();
            }
#else
            if (appointmentEditor != null)
            {
                appointmentEditor.Save.Click -= Save_Click;
                appointmentEditor.Delete.Click -= Delete_Click;
                appointmentEditor.Close();
            }
#endif
#if SILVERLIGHT
            if(RecurringPopup != null)
            {
                RecurringPopup.Closed += RecurringPopup_Closed;
                RecurringPopup.Show();
            }
#else
            if (RecurringPopup != null)
            {
                RecurringPopup.Owner = this.FindParentElementOfType<Window>();
                RecurringPopup.OpenOne.Click -= OpenOne_Click;
                RecurringPopup.Close();
            }
#endif

            if(doubleClickTimer != null)
            {
                doubleClickTimer.Tick += doubleClickTimer_Tick;
            }

            if (AddnewContextmenuPopup != null)
            {
#if WPF
                foreach (MenuItem item in AddnewContextmenuPopup.Items)
                {
                    item.Click -= item_Click;
                }
                AddnewContextmenuPopup.Items.Clear();
#endif
                AddnewContextmenuPopup = null;
            }
            if (contextmenupopup != null)
            {
#if WPF
                foreach (MenuItem item in contextmenupopup.Items)
                {
                    item.Click -= item_Click;
                }
                contextmenupopup.Items.Clear();
#endif
                contextmenupopup = null;
            }
            if (currentSelectedItem != null)
            {
#if WPF
                currentSelectedItem.PreviewMouseLeftButtonUp -= mainViewItem_MouseLeftButtonUp;
                currentSelectedItem.MouseDoubleClick -= SfSchedule_MouseDoubleClick;
#else
                mainViewItem.MouseLeftButtonUp -= mainViewItem_MouseLeftButtonUp;
                MouseDoubleClick -= SfSchedule_MouseDoubleClick;
#endif
            }
            if (DragDropCanvas != null)
            {
                DragDropCanvas.MouseLeftButtonDown -= DragDropCanvas_MouseLeftButtonDown;
                DragDropCanvas.MouseLeftButtonUp -= DragDropCanvas_MouseLeftButtonUp;
                DragDropCanvas.MouseMove -= DragDropCanvas_MouseMove;
            }
            AppointmentEditorOpening -= SfSchedule_AppointmentEditorOpening;
            ContextMenuOpening -= SfSchedule_ContextMenuOpening;
            MouseRightButtonDown -= SfSchedule_MouseRightButtonDown;
            KeyUp -= SfSchedule_KeyUp;
            Unloaded -= SfSchedule_Unloaded;
            reminderTimer.Tick -= reminderTimer_Tick;
            Loaded -= Schedule_Loaded;
        }

        #endregion

        #endregion

        #region Events

        #region Schedule KeyUp Event

        void SfSchedule_KeyUp(object sender, KeyEventArgs e)
        {
            if (DragDropCanvas.Children.Count > 0)
            {
                Dayviewrb.Detach();
                Monthviewrb.Detach();
                Timelineviewrb.Detach();
                var control = DragDropCanvas.Children[0] as Control;
                if (control != null)
                {
                    control.MouseLeftButtonDown -= drag_app_MouseLeftButtonDown;
                    control.Loaded -= drag_app_Loaded;
                }
            }
            DragDropCanvas.Children.Clear();
            ResetDragDropAppointmentOpacity();
            AppointmentInResizeMode = false;
            if (addnewpopup != null)
                addnewpopup.IsOpen = false;
            if (editpopup != null)
                editpopup.IsOpen = false;
        }

        #endregion

        #region NonAccessibleBlocks CollectionChanged

        void NonAccessibleBlocks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var nonAccessibleBlockTemplateBinding = new Binding { Source = this, Path = new PropertyPath("NonAccessibleBlockTemplate") };
            foreach (NonAccessibleBlock nonAccessibleBlock in NonAccessibleBlocks)
            {
                BindingOperations.SetBinding(nonAccessibleBlock, NonAccessibleBlock.CustomTemplateProperty, nonAccessibleBlockTemplateBinding);
            }
        }

        #endregion

        #region Schedule Loaded Events

        void Schedule_Loaded(object sender, RoutedEventArgs e)
        {
            isLoaded = true;
            #if WPF
            if (flipview != null)
            {
                flipview.FindElementOfType<Border>();


                if (flipview != null)
                {
                    double navButtonMargin = (flipviewselecteditem as Grid).FindElementOfType<HeaderTitleBarView>().ActualHeight / 6;

                    if (Prev_Button != null)
                    {
                        Prev_Button.VerticalAlignment = VerticalAlignment.Top;
                        Prev_Button.Margin = new Thickness(0, navButtonMargin, 0, 0);
                        Prev_Button.Width = 40;
                        Prev_Button.Click -= Navigation_Button_Click;
                        Prev_Button.Click += Navigation_Button_Click;
                    }
                    if (Next_Button != null)
                    {
                        Next_Button.VerticalAlignment = VerticalAlignment.Top;
                        Next_Button.Margin = new Thickness(0, navButtonMargin, 0, 0);
                        Next_Button.Width = 40;
                        Next_Button.Click -= Navigation_Button_Click;
                        Next_Button.Click += Navigation_Button_Click;
                    }

                }

            }
#endif
        }

        void SfSchedule_Unloaded(object sender, RoutedEventArgs e)
        {
            if (editpopup != null)
                editpopup.IsOpen = false;
            if (addnewpopup != null)
                addnewpopup.IsOpen = false;
            if (contextmenupopup != null)
                contextmenupopup.IsOpen = false;
            if (AddnewContextmenuPopup != null)
                AddnewContextmenuPopup.IsOpen = false;
#if WPF
            if (Reminder.IsActive)
            {
                Reminder.Close();
                var args = new ReminderControlClosedEventArgs();
                GetReminderControlClosedEvents(args);
            }
#else
            Reminder.Close();
            var args = new ReminderControlClosedEventArgs();
            GetReminderControlClosedEvents(args);
#endif
        }

        #endregion

        #region Navigation Button Click Event

        void Navigation_Button_Click(object sender, RoutedEventArgs e)
        {
            if (DragDropCanvas.Children.Count > 0)
            {
                Dayviewrb.Detach();
                Monthviewrb.Detach();
                Timelineviewrb.Detach();
                var control = DragDropCanvas.Children[0] as Control;
                if (control != null)
                {
                    control.MouseLeftButtonDown -= drag_app_MouseLeftButtonDown;
                    control.Loaded -= drag_app_Loaded;
                }
                DragDropCanvas.Children.Clear();
                ResetDragDropAppointmentOpacity();
            }
        }

        #endregion

        #region MainViewItem MouseLeftButtonUp Event       

        void mainViewItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is FrameworkElement && (e.OriginalSource as FrameworkElement).DataContext is NonAccessibleBlock)
            {
                return;
            }
            Currentselecteddate = GetCurrentSelectedDate(e);
            MinMaxSelection = new DateTime();
            var currentOriginalSource = e.OriginalSource;
            if (ItemsSource != null && AppointmentMapping != null && AppointmentTemplate != null && currentOriginalSource is FrameworkElement)
            {
                currentOriginalSource = FindContentControl(currentOriginalSource as FrameworkElement);
            }
            isMainViewItemClicked = true;

            if (contextmenupopup != null)
                contextmenupopup.IsOpen = false;
            if (AddnewContextmenuPopup != null)
                AddnewContextmenuPopup.IsOpen = false;
            if (!doubleClickTimer.IsEnabled)
            {
                doubleClickTimer.Start();
            }
            else
            {
                doubleClickTimer.Stop();
                OnMouseDoubleClick(e);
#if WPF
                SfSchedule_MouseDoubleClick(sender, e);
#endif
            }
            if (!IsDragEnabled)
                OnMouseLeftButtonDown(e);

            var mainitem = sender as ContentControl;
            if (mainitem != null)
            {
                if (mainitem.Content is ScheduleDaysView)
                {
                    var dayview = mainitem.Content as ScheduleDaysView;
                    Point position = e.GetPosition(dayview.FindElementOfType<ScheduleHorizontalTimeSlotItemsControl>());
                    SelectedPoint = position;                   
                    var frameworkElement = currentOriginalSource as FrameworkElement;
                    AllDayAppointmentItemscontrol allday = frameworkElement.FindParentElementOfType<AllDayAppointmentItemscontrol>();
                    if (currentAllDaySelectedItem != null)
                    {
#if WPF
                        currentAllDaySelectedItem.Background = new SolidColorBrush(Colors.Transparent);    
                        currentAllDaySelectedItem = null;
#elif SILVERLIGHT
                        if (allday != null)
                        {
                            currentAllDaySelectedItem.Background = new SolidColorBrush(Colors.LightGray);
                        }
                        else
                        {
                            currentAllDaySelectedItem.Background = new SolidColorBrush(Colors.Transparent);
                            currentAllDaySelectedItem = null;
                        }
#endif
                    }
                    dayview.UpdateSelection(true);
                    if (frameworkElement != null && !(frameworkElement.DataContext is ScheduleAppointment) && allday == null)
                    {
                        dayview.RectVisibility = Visibility.Visible;
                    }
                    else
                    {
                        dayview.RectVisibility = Visibility.Collapsed;
                    }                       
                }
                else if (mainitem.Content is ScheduleMonthView)
                {
                    var scheduleMonthView = mainitem.Content as ScheduleMonthView;
                    if (currentitem != null)
                    {
                        var backgroundBinding = new Binding { Source = (viewcontrol.Content as ScheduleMonthView) };
                        if (!currentitem.IsCurrentDate)
                        {
                            backgroundBinding.Path = currentitem.IsCurrentMonth ? new PropertyPath("FocusedMonth") : new PropertyPath("NonFocusedMonth");
                            BindingOperations.SetBinding(currentitem, BackgroundProperty, backgroundBinding);
                        }
                    }
#if WPF
                    Point Overallposition = e.GetPosition(monthview);
                    hitResultsList.Clear();
                    VisualTreeHelper.HitTest(this, null, MyHitTestResult, new PointHitTestParameters(Overallposition));
                    //ScheduleMonthViewItemsControl monthitem = (from item in hitResultsList where item.GetType() == typeof(Grid) select (item as Grid).FindElementOfType<ScheduleMonthViewItemsControl>()).FirstOrDefault();
                    ScheduleMonthViewItemsControl monthitem = currentSelectedItem.FindElementOfType<ScheduleMonthViewItemsControl>();
#else
                    var monthitem = scheduleMonthView.FindElementOfType<ScheduleMonthViewItemsControl>();
#endif
                    if (monthitem != null)
                    {
                        SelectedPoint = e.GetPosition(monthitem);
                        currentitem = scheduleMonthView.GetCurrentItem(this, monthitem);                     

                        if (currentitem != null && !currentitem.IsCurrentDate)
                        {
                            currentitem.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xD3, 0xD3, 0xD3));
                        }
                    }
                }
                else if (mainitem.Content is ScheduleTimeLineView)
                {
                    var scheduleTimeLineView = mainitem.Content as ScheduleTimeLineView;
                    Point position = e.GetPosition((mainitem.Content as ScheduleTimeLineView).FindElementOfType<ScheduleHorizontalTimeSlotControl>());
                    SelectedPoint = position;
                    scheduleTimeLineView.UpdateSelection(true);
                }

                if (!AppointmentInResizeMode && EnableTouch)
                {
                    var frameworkElement = currentOriginalSource as FrameworkElement;
                    if (frameworkElement != null && !(frameworkElement.DataContext is ScheduleAppointment))
                    {
                        var args = new ContextMenuOpeningEventArgs
                        {
                            CurrentEventArgs = e,
                            CurrentSelectedDate = Currentselecteddate,
                            Appointment = null,
                            SelectedResource = selectedResourcename
                        };
                        GetContextMenuOpeningEvent(args);

                        if (!isContextMenuAltered)
                        {
                            addnewpopup.Child.Visibility = !AllowEditing && CopiedAppointment == null ? Visibility.Collapsed : Visibility.Visible;
                            var addAppintmentControl = addnewpopup.Child as AddAppintmentControl;
                            if (e.GetPosition(this).X > ActualWidth / 2)
                            {
                                if (TouchMenuType == MenuType.Default)
                                {
                                    if (addAppintmentControl != null)
                                        addAppintmentControl.FlowDirection = FlowDirection.RightToLeft;
                                    addnewpopup.HorizontalOffset = e.GetPosition(this).X - 150;
                                }
                                else
                                {
#if WPF
                                    addnewpopup.HorizontalOffset = e.GetPosition(this).X;
#else
                                    addnewpopup.HorizontalOffset = e.GetPosition(this).X - 200;
#endif
                                }
                            }
                            else
                            {

                                if (TouchMenuType == MenuType.Default)
                                {
                                    if (addAppintmentControl != null)
                                        addAppintmentControl.FlowDirection = FlowDirection.LeftToRight;
                                    addnewpopup.HorizontalOffset = e.GetPosition(this).X + 50;
                                }
                                else
                                {
#if WPF
                                    addnewpopup.HorizontalOffset = e.GetPosition(this).X + 250;
#else
                                    addnewpopup.HorizontalOffset = e.GetPosition(this).X;
#endif
                                }
                            }

                            if (TouchMenuType == MenuType.Default)
                            {
                                if (e.GetPosition(this).Y > ActualHeight - 75)
                                {
                                    addnewpopup.VerticalOffset = e.GetPosition(this).Y - 140;
                                }
                                else
                                {
                                    addnewpopup.VerticalOffset = e.GetPosition(this).Y - 60;
                                }
                            }
                            else
                            {
                                if (e.GetPosition(this).Y > ActualHeight - 300)
                                {
#if WPF
                                    addnewpopup.VerticalOffset = e.GetPosition(this).Y - 100;
#else
                                    addnewpopup.VerticalOffset = e.GetPosition(this).Y - 200;
#endif
                                }
                                else
                                {
#if WPF
                                    addnewpopup.VerticalOffset = e.GetPosition(this).Y + 100;
#else
                                    addnewpopup.VerticalOffset = e.GetPosition(this).Y;
#endif
                                }
                            }
                            if (doubleClickTimer.IsEnabled)
                            {
                                addnewpopup.IsOpen = true;
                            }
#if WPF
                            else
                            {
                                isDoubleTapped = true;
                            }

                            if (TouchMenuType == MenuType.RadialMenu && !isDoubleTapped)
#else
                            if (TouchMenuType == MenuType.RadialMenu)
#endif
                            {
                                addnewpopup.IsOpen = true;
                                addnewpopup.Child.UpdateLayout();
                            }

                            var appintmentControl = addnewpopup.Child as AddAppintmentControl;
                            var addRadialMenuControl = addnewpopup.Child as AddRadialMenuControl;

                            if (appintmentControl != null && TouchMenuType == MenuType.Default && appintmentControl.Paste != null)
                            {
                                appintmentControl.Paste.Visibility = CopiedAppointment == null ? Visibility.Collapsed : Visibility.Visible;
                            }
                            else if (addRadialMenuControl != null && TouchMenuType == MenuType.RadialMenu && addRadialMenuControl.RadialMenu != null && 
                                addRadialMenuControl.RadialMenu.Items[3] is SfRadialMenuItem)
                            {
                                if (CopiedAppointment == null)
                                {
                                    (addRadialMenuControl.RadialMenu.Items[3] as SfRadialMenuItem).IsEnabled = false;
                                    (addRadialMenuControl.RadialMenu.Items[3] as SfRadialMenuItem).Opacity = 0.5;
                                }
                                else
                                {
                                    (addRadialMenuControl.RadialMenu.Items[3] as SfRadialMenuItem).IsEnabled = true;
                                    (addRadialMenuControl.RadialMenu.Items[3] as SfRadialMenuItem).Opacity = 1;
                                }
                            }

                            if (appintmentControl != null && TouchMenuType == MenuType.Default && appintmentControl.AddNew != null)
                            {
                                appintmentControl.AddNew.Visibility = AllowEditing ? Visibility.Visible : Visibility.Collapsed;
                            }
                            else if (addRadialMenuControl != null && TouchMenuType == MenuType.RadialMenu && addRadialMenuControl.RadialMenu != null &&
                                addRadialMenuControl.RadialMenu.Items[0] is SfRadialMenuItem)
                            {
                                if (!AllowEditing)
                                {
                                    (addRadialMenuControl.RadialMenu.Items[0] as SfRadialMenuItem).IsEnabled = false;
                                    (addRadialMenuControl.RadialMenu.Items[0] as SfRadialMenuItem).Opacity = 0.5;
                                }
                                else
                                {
                                    (addRadialMenuControl.RadialMenu.Items[0] as SfRadialMenuItem).IsEnabled = true;
                                    (addRadialMenuControl.RadialMenu.Items[0] as SfRadialMenuItem).Opacity = 1;
                                }
                            }
#if WPF
                            addnewpopup.Placement = PlacementMode.AbsolutePoint;
                            addnewpopup.PlacementTarget = this;

                            if (TouchMenuType == MenuType.Default && !isDoubleTapped)
#else
                            if (TouchMenuType == MenuType.Default )
#endif
                            {
                                addnewpopup.IsOpen = true;
                                addnewpopup.Child.UpdateLayout();

                            }

                            editpopup.IsOpen = false;
                            contextmenupopup.IsOpen = false;
                            AddnewContextmenuPopup.IsOpen = false;
                        }
                    }
                    else
                    {
                        var args = new ContextMenuOpeningEventArgs
                        {
                            CurrentEventArgs = e,
                            CurrentSelectedDate = Currentselecteddate,
                            Appointment = SelectedAppointment
                        };

                        if (SelectedAppointment != null)
                        {
                            args.SelectedResource = SelectedAppointment.ResourceCollection.ToList();
                            if (ItemsSource != null)
                            {
                                IEnumerable<object> source = null;
#if WPF
                                if (ItemsSource is DataRowCollection)
                                    source = (ItemsSource as DataRowCollection).OfType<DataRow>();
                                else
#endif
                                    source = (IEnumerable<object>)ItemsSource;
                                object obj = source.FirstOrDefault(x => x.GetHashCode() == (int)SelectedAppointment.ObjectID);
                                if (obj != null)
                                {
                                    args.Appointment = obj;
                                }
                            }
                        }
                        GetContextMenuOpeningEvent(args);
                    }
                }
            }
        }

        #endregion

        #region Schedule MouseRightButtonDown Event

        void SfSchedule_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is FrameworkElement && (e.OriginalSource as FrameworkElement).DataContext is NonAccessibleBlock)
            {
#if WPF
                if (ContextMenu != null)
                {
                    ContextMenu.IsOpen = false;
                    ContextMenu = null;
                }
#endif
                e.Handled = true;
                return;
            }
            ClearDragDropCanvas();
#if SILVERLIGHT
            var listBox = contextmenupopup.Child as ListBox;
#endif
            var currentOriginalSource = e.OriginalSource;
            if (ItemsSource != null && AppointmentMapping != null && AppointmentTemplate != null && currentOriginalSource is FrameworkElement)
            {
                currentOriginalSource = FindContentControl(currentOriginalSource as FrameworkElement);
            }
            if (!EnableTouch)
            {
                Currentselecteddate = GetCurrentSelectedDate(e);
                var frameworkElement = currentOriginalSource as FrameworkElement;
                if (frameworkElement != null && (frameworkElement.DataContext is ScheduleAppointment))
                {
                    SelectedAppointment = frameworkElement.DataContext as ScheduleAppointment;
                    SelectedAppointment.IsSelected = true;
                    SelectedAppointment.AppointmentSelectionBrush = AppointmentSelectionBrush;

                    var args = new ContextMenuOpeningEventArgs
                    {
                        CurrentEventArgs = e,
                        CurrentSelectedDate = Currentselecteddate,
                        SelectedResource = SelectedAppointment.ResourceCollection.ToList(),
                        Appointment = SelectedAppointment
                    };
                    if (ItemsSource != null)
                    {
                        IEnumerable<object> source = null;
#if WPF
                        if (ItemsSource is DataRowCollection)
                            source = (ItemsSource as DataRowCollection).OfType<DataRow>();
                        else
#endif
                            source = (IEnumerable<object>)ItemsSource;
                        if (source != null)
                        {
                            object obj = source.FirstOrDefault(x => x.GetHashCode() == (int)SelectedAppointment.ObjectID);
                            if (obj != null)
                            {
                                args.Appointment = obj;
                            }
                        }
                    }
                    GetContextMenuOpeningEvent(args);
                    if (!isContextMenuAltered)
                    {
                        if (SelectedAppointment.ReadOnly)
                        {
#if WPF
                            if (contextmenupopup.Items[3] is MenuItem)
                                (contextmenupopup.Items[3] as MenuItem).IsEnabled = false;
                            if (contextmenupopup.Items[4] is MenuItem)
                                (contextmenupopup.Items[4] as MenuItem).IsEnabled = false;
#else
                            if (listBox != null)
                            {
                                if (listBox.Items[3] is ListBoxItem)
                                    (listBox.Items[3] as ListBoxItem).IsEnabled = false;
                                if (listBox.Items[4] is ListBoxItem)
                                    (listBox.Items[4] as ListBoxItem).IsEnabled = false;
                            }
#endif
                        }
                        else
                        {
#if WPF
                            if (contextmenupopup.Items[3] is MenuItem)
                                (contextmenupopup.Items[3] as MenuItem).IsEnabled = true;
                            if (contextmenupopup.Items[4] is MenuItem)
                                (contextmenupopup.Items[4] as MenuItem).IsEnabled = true;
#else
                            if (listBox != null)
                            {
                                if (listBox.Items[3] is ListBoxItem)
                                    (listBox.Items[3] as ListBoxItem).IsEnabled = true;
                                if (listBox.Items[4] is ListBoxItem)
                                    (listBox.Items[4] as ListBoxItem).IsEnabled = true;
                            }
#endif
                        }

#if WPF                        
                        if (contextmenupopup.Items[0] is MenuItem)
                            (contextmenupopup.Items[0] as MenuItem).Visibility = AllowEditing ? Visibility.Visible : Visibility.Collapsed;
                        if (contextmenupopup.Items[2] is MenuItem)
                            (contextmenupopup.Items[2] as MenuItem).Visibility = AllowEditing ? Visibility.Visible : Visibility.Collapsed;

#else
                        if (listBox != null)
                        {
                            if (listBox.Items[0] is ListBoxItem)
                                (listBox.Items[0] as ListBoxItem).Visibility = AllowEditing ? Visibility.Visible : Visibility.Collapsed;
                            if (listBox.Items[2] is ListBoxItem)
                                (listBox.Items[2] as ListBoxItem).Visibility = AllowEditing ? Visibility.Visible : Visibility.Collapsed;
                        }
#endif

                        if (ScheduleType == ScheduleType.Month)
                        {
#if WPF
                            if (contextmenupopup.Items[4] is MenuItem)
                            {
                                (contextmenupopup.Items[4] as MenuItem).Header = "DragDrop";
                                (contextmenupopup.Items[4] as MenuItem).Icon = GetIcon("DragDrop.png");
                            }
#else
                            if (listBox != null && listBox.Items[4] is ListBoxItem)
                                (listBox.Items[4] as ListBoxItem).Content = GetContextMenuItemContent("DragDrop", "DragDrop.png");
#endif
                        }
                        else
                        {
#if WPF
                            if (contextmenupopup.Items[4] is MenuItem)
                            {
                                (contextmenupopup.Items[4] as MenuItem).Header = "Resize";
                                (contextmenupopup.Items[4] as MenuItem).Icon = GetIcon("Resize.png");
                            }
#else
                            if (listBox != null && listBox.Items[4] is ListBoxItem)
                                (listBox.Items[4] as ListBoxItem).Content = GetContextMenuItemContent("Resize", "Resize.png");
#endif
                        }
                        allDayFlag = SelectedAppointment.AllDay;
#if SILVERLIGHT
                        if (listBox != null && listBox.Items[1] is ListBoxItem)
                            (listBox.Items[1] as ListBoxItem).IsEnabled = !SelectedAppointment.IsRecursive;
#else
                        var menuItem = contextmenupopup.Items[1] as MenuItem;
                        if (menuItem != null)
                            menuItem.IsEnabled = !SelectedAppointment.IsRecursive;
#endif
#if SILVERLIGHT
                        contextmenupopup.IsOpen = true;
                        contextMenuVisible = true;
                        AddnewContextmenuPopup.IsOpen = false;
                        contextmenupopup.HorizontalOffset = e.GetPosition(this).X + 10;
                        contextmenupopup.VerticalOffset = e.GetPosition(this).Y;
#else
                        ContextMenu = contextmenupopup;
                        ContextMenu.IsOpen = true;
                        ContextMenu.Placement = PlacementMode.MousePoint;
#endif
                    }
                    e.Handled = true;
                }
                else
                {

                    if (doubleClickTimer != null)
                    {
                        doubleClickTimer.Stop();
                    }

                    mainViewItem_MouseLeftButtonUp(currentSelectedItem, e);

                    var args = new ContextMenuOpeningEventArgs
                    {
                        CurrentEventArgs = e,
                        CurrentSelectedDate = GetDate(SelectedPoint),
                        Appointment = null,
                        SelectedResource = selectedResourcename
                    };
                    GetContextMenuOpeningEvent(args);
                    if (!isContextMenuAltered)
                    {
#if WPF
                        if (AddnewContextmenuPopup.Items != null && AddnewContextmenuPopup.Items.Count > 1)
                        {
                            if(CopiedAppointment == null)
                                AddnewContextmenuPopup.Items.RemoveAt(1);
                            else
                                (AddnewContextmenuPopup.Items[0] as MenuItem).Visibility = AllowEditing ? Visibility.Visible : Visibility.Collapsed;
                        }

                        AddnewContextmenuPopup.Visibility = !AllowEditing && CopiedAppointment == null ? Visibility.Collapsed : Visibility.Visible;
                        ContextMenu = AddnewContextmenuPopup;
                        ContextMenu.IsOpen = true;
                        ContextMenu.Placement = PlacementMode.MousePoint;
#else
                        AddnewContextmenuPopup.HorizontalOffset = e.GetPosition(this).X + 10;
                        AddnewContextmenuPopup.VerticalOffset = e.GetPosition(this).Y;
                        if (AddnewContextmenuPopup.Child is ListBox && (AddnewContextmenuPopup.Child as ListBox).Items.Count > 1)
                        {
                            if (CopiedAppointment == null)
                                (AddnewContextmenuPopup.Child as ListBox).Items.RemoveAt(1);
                            else
                                ((AddnewContextmenuPopup.Child as ListBox).Items[0] as ListBoxItem).Visibility = AllowEditing ? Visibility.Visible : Visibility.Collapsed;
                        }
                        AddnewContextmenuPopup.Visibility = !AllowEditing && CopiedAppointment == null ? Visibility.Collapsed : Visibility.Visible;
                        AddnewContextmenuPopup.IsOpen = true;
                        contextmenupopup.IsOpen = false;
#endif
                    }
                    e.Handled = true;
                }
            }
        }


        #endregion

        #region Schedule MouseDoubleClick Event

        void SfSchedule_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is FrameworkElement && (e.OriginalSource as FrameworkElement).DataContext is NonAccessibleBlock)
            {
                return;
            }
#if WPF
            if (!AllowEditing || !(sender is ContentControl) || !allowEditorsToOpen)
#else
            if (!AllowEditing || !(sender is SfSchedule))
#endif
                return;
            addnewpopup.IsOpen = false;
            editpopup.IsOpen = false;
            if (appointmentEditor != null)
                appointmentEditor.recursiveModified = 0;
            var mainitem = currentSelectedItem;
            if ((e.OriginalSource is FrameworkElement && (e.OriginalSource as FrameworkElement).DataContext is ScheduleAppointment))
            {
                RecAppointment = (e.OriginalSource as FrameworkElement).DataContext as ScheduleAppointment;
                if (RecAppointment != null)
                {
                    if (RecAppointment.IsRecursive)
                    {
#if SILVERLIGHT
                        RecurringPopup = new OpenRecurringAppointment();
                        RecurringPopup.Closed += RecurringPopup_Closed;
                        RecurringPopup.Show();
#else
                        RecurringPopup = new OpenRecurringAppointment { Owner = this.FindParentElementOfType<Window>() };
                        RecurringPopup.OpenOne.Click += OpenOne_Click;
                        RecurringPopup.ShowDialog();
#endif
                    }
                    else
                    {
                        InvokeScheduleDoubleClickEvent();
                        appointmentEditor = new AppointmentEditor();
                        appointmentEditor.UpdateAppointmentProperties(this, RecAppointment);
                        appointmentEditor.Closed += appointmentEditor_Closed;
                        if (editorvisibility == Visibility.Visible)
                        {
                            SelectedAppointment = RecAppointment;
#if WPF
                            appointmentEditor.Owner = this.FindParentElementOfType<Window>();
                            appointmentEditor.Save.Click += Save_Click;
                            appointmentEditor.Delete.Click += Delete_Click;
                            appointmentEditor.ShowDialog();
#else
                            appointmentEditor.Show();
#endif
                        }
                    }
                }
            }
            else
            {
                if (SelectedAppointment != null)
                {
                    SelectedAppointment.IsSelected = false;
                    SelectedAppointment.AppointmentSelectionBrush = new SolidColorBrush(Colors.Transparent);
                    SelectedAppointment = null;
                }
                DateTime selectedDate = GetCurrentSelectedDate(e);
                var EndDate = new DateTime();
                if (mainitem != null && ScheduleType == ScheduleType.Month)
                {
                    EndDate = selectedDate.AddHours(12);
                }
                else if (mainitem != null && (ScheduleType == ScheduleType.Day || ScheduleType == ScheduleType.Week || ScheduleType == ScheduleType.WorkWeek || ScheduleType == ScheduleType.TimeLine))
                {
                    EndDate = selectedDate.Add(GetTimeInterval());

                }
                InvokeScheduleDoubleClickEvent();
                appointmentEditor = new AppointmentEditor();
                appointmentEditor.SetNewAppointmentProperties(this, selectedDate, EndDate);
                appointmentEditor.Closed += appointmentEditor_Closed;
                if (editorvisibility == Visibility.Visible)
                {
#if WPF
                    appointmentEditor.Owner = this.FindParentElementOfType<Window>();
                    appointmentEditor.Save.Click += Save_Click;
                    appointmentEditor.Delete.Click += Delete_Click;
                    appointmentEditor.ShowDialog();
#else
                    appointmentEditor.Show();
#endif
                }
            }
        }

        #endregion

        #region ItemsSource Related Changed Events

        public event EventHandler ItemsSourceChanged;

        void ItemsSource_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            object obj = sender;
            string property = e.PropertyName;
            foreach (ScheduleAppointment item in Appointments)
            {
                if (item.ObjectID.Equals(obj.GetHashCode()))
                {
                    UpdateAppointment(item, obj, property);
                }
            }

        }

        void ItemsSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                object obj = ((object[])e.NewItems.SyncRoot).First();
                if (obj is INotifyPropertyChanged)
                {
                    (obj as INotifyPropertyChanged).PropertyChanged += ItemsSource_PropertyChanged;
                }
                var accessors = new Dictionary<string, IPropertyAccessor>();
                Reflection(accessors, obj);
                ScheduleAppointment app = CreateAppointment(accessors, obj);
                Appointments.Add(app);
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                object obj = ((object[])e.OldItems.SyncRoot).First();
                foreach (ScheduleAppointment item in Appointments)
                {
                    if (item.ObjectID.Equals(obj.GetHashCode()))
                    {
                        Appointments.Remove(item);
                        break;
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                if (Appointments.Count > 0)
                    Appointments.Clear();// = new ScheduleAppointmentCollection();
                if (ItemsSource != null)
                {
                    WireItemSource(ItemsSource as IEnumerable);
                    SetItemsSource();
                }
                if (Appointments != null)
                {
                    Appointments.CollectionChanged += Appointments_CollectionChanged;
                }
            }
        }

        #endregion

        #region Appointment Editor Opening Event

        void SfSchedule_AppointmentEditorOpening(object sender, AppointmentEditorOpeningEventArgs e)
        {
            editorvisibility = (e.Cancel) ? Visibility.Collapsed : Visibility.Visible;
        }

        #endregion

        #region Drag & Drop Events

        #region DragDropCanvas MouseMove

        void DragDropCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (!IsDragEnabled)
            {
                return;
            }
#if SILVERLIGHT
            bool enableDrag = (IsDragEnabled && !CaptureMouse());
#else
            bool enableDrag = (IsDragEnabled && e.LeftButton == MouseButtonState.Released);
#endif
            if (dvc != null && currentSelectedItem.Content is ScheduleDaysView)
            {
                (currentSelectedItem.Content as ScheduleDaysView).MoveDragDrop(this, enableDrag, e);
            }
            else if (mvc != null && currentSelectedItem.Content is ScheduleMonthView)
            {
                (currentSelectedItem.Content as ScheduleMonthView).MoveDragDrop(this, enableDrag, e);
            }
            else if (hvc != null && currentSelectedItem.Content is ScheduleTimeLineView)
            {
                (currentSelectedItem.Content as ScheduleTimeLineView).MoveDragDrop(this, enableDrag, e);
            }
        }

        #endregion

        #region DragDropCanvas MouseLeftButtonUp

        void DragDropCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isscrollmoveondragging = false;

            #region DragDrop & Resize
            if (dvc != null && currentSelectedItem.Content is ScheduleDaysView)
            {
                if (IsDragEnabled)
                    (currentSelectedItem.Content as ScheduleDaysView).ReleaseDragDrop(this, e);
                if (IsResizeEnabled)
                    (currentSelectedItem.Content as ScheduleDaysView).ReleaseResize(this);
            }
            else if (mvc != null && currentSelectedItem.Content is ScheduleMonthView)
            {
                if (IsDragEnabled)
                {
                    var backgroundBinding = new Binding { Source = this };
                    if (currentitem != null && !currentitem.IsCurrentDate)
                    {
                        backgroundBinding.Path = currentitem.IsCurrentMonth ? new PropertyPath("FocusedMonth") : new PropertyPath("NonFocusedMonth");
                        BindingOperations.SetBinding(currentitem, BackgroundProperty, backgroundBinding);
                    }
                    (currentSelectedItem.Content as ScheduleMonthView).ReleaseDragDrop(this, e);
                }
                if (IsResizeEnabled)
                    (currentSelectedItem.Content as ScheduleMonthView).ReleaseResize(this);
            }
            else if (hvc != null && currentSelectedItem.Content is ScheduleTimeLineView)
            {
                if (IsDragEnabled)
                    (currentSelectedItem.Content as ScheduleTimeLineView).ReleaseDragDrop(this, e);
                if (IsResizeEnabled)
                    (currentSelectedItem.Content as ScheduleTimeLineView).ReleaseResize(this);
            }
            #endregion

            IsDragEnabled = false;
        }

        #endregion

        #region DragDropCanvas MouseLeftButtonDown

        void DragDropCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (currentSelectedItem.Content is ScheduleDaysView && (ScheduleType == ScheduleType.Week || ScheduleType == ScheduleType.Day))
            {
                var obj = VisualTreeHelper.GetChild(currentSelectedItem.Content as ScheduleDaysView, 0) as Grid;
                if (obj != null)
                {
                    var DaysScrollviewer = obj.FindName("Scrollviewer") as ScrollViewer;
                    if (DaysScrollviewer != null)
                    {
                        double CurrentPositionInScroll = e.GetPosition(DaysScrollviewer).Y;
                        if (CurrentPositionInScroll <= 0)
                            (currentSelectedItem.Content as ScheduleDaysView).dayCanvasFromAllday = true;
                    }
                }
            }
        }

        #endregion

        #region Drag Appointment MouseLeftButtonDown

        internal void drag_app_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource.GetType() != typeof(Ellipse))
            {
                IsResizeEnabled = false;
                IsDragEnabled = true;
            }
            else
            {
                IsDragEnabled = false;
                IsResizeEnabled = true;
            }
        }

        #endregion

        #region Drag Appointment Loaded

        internal void drag_app_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is ScheduleDaysAppointmentViewControl)
            {
#if SILVERLIGHT
                (sender as ScheduleDaysAppointmentViewControl).UpdateLayout();
#endif
                Dayviewrb.Attach((sender as ScheduleDaysAppointmentViewControl).FindElementOfType<Grid>());
            }
            else if (sender is ScheduleHorizontalAppointmentViewControl)
            {
#if SILVERLIGHT
                (sender as ScheduleHorizontalAppointmentViewControl).UpdateLayout();
#endif
                Timelineviewrb.Attach((sender as ScheduleHorizontalAppointmentViewControl).FindElementOfType<Grid>());
            }
        }

        #endregion

        #endregion

        #region Appointment Property Changed Events

        private void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var app = sender as ScheduleAppointment;
            if (app == null) return;
            if ((e.PropertyName == "RecurrenceProperites" || e.PropertyName == "RecurrenceRule") && !IsRRuleSetInternally)
            {
                if (RecursiveAppointments.Contains(app))
                {
                    RemoveRecursive(app);
                    Appointments.Add(app);
                    if (SelectedAppointment == null)
                        SelectedAppointment = app;
                    SetProxyAppointments(Appointments);
                    SetCurrentVisibleAppointments(CurrentSelectedDates);
                }
                else
                {
                    Remove(app.InternalEndTime, app);
                    SetProxyAppointments(Appointments);
                    SetCurrentVisibleAppointments(CurrentSelectedDates);
                }
                if (!app.IsRecursive)
                {
                    if (RecursiveAppointments.Contains(app))
                    {
                        RemoveRecursive(app);
                        Appointments.Add(app);
                    }
                }
            }
            if ((e.PropertyName == "InternalStartTime" || e.PropertyName == "InternalEndTime") && !stopUpdate)
            {

                foreach (DateTime dt in ProxyAppointments.Keys)
                {
                    if (ProxyAppointments[dt].Contains(app))
                    {
                        ProxyAppointments[dt].Remove(app);
                    }
                }
                for (DateTime dt = app.InternalStartTime.Date; dt <= app.InternalEndTime.Date; dt = dt.AddDays(1).Date)
                {
                    if (ProxyAppointments != null && ProxyAppointments.ContainsKey(dt))
                    {
                        if (!ProxyAppointments[dt].Contains(app))
                        {
                            ProxyAppointments[dt].Add(app);
                        }
                    }
                    else
                    {
                        ProxyAppointments.Add(dt, new ObservableCollection<ScheduleAppointment> { app });
                    }
                }
                SetCurrentVisibleAppointments(CurrentSelectedDates);
            }

            if (app.IsSet)
            {
                EditToast(app);
            }
            else if (app.ReminderTime != ReminderTimeType.None)
            {
                app.ReminderDeliveryTime = app.InternalStartTime - app.MeasureTime(app.ReminderTime);

            }
            GenerateNotification();
        }

        #endregion

        #region Appointment Collection Changed Event

        public event NotifyCollectionChangedEventHandler AppointmentCollectionChanged;

        void Appointments_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var newapp = ((object[])e.NewItems.SyncRoot).First() as ScheduleAppointment;
                if (newapp != null)
                {
                    newapp.PropertyChanged += item_PropertyChanged;

                    Add(newapp.InternalStartTime.Date, newapp);
                    if (newapp.IsRecursive)
                    {
                        AddRecursive();
                        SetProxyAppointments(Appointments);
                    }
                    if (newapp.ReminderTime != ReminderTimeType.None)
                    {
                        newapp.ReminderDeliveryTime = newapp.InternalStartTime - newapp.MeasureTime(newapp.ReminderTime);
                        GenerateNotification();
                    }
                }
                SetCurrentVisibleAppointments(CurrentSelectedDates);
                SetPrevVisibleAppointments(PrevSelectedDates);
                SetNextVisibleAppointments(NextSelectedDates);

            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                var newapp = ((object[])e.OldItems.SyncRoot).First() as ScheduleAppointment;
                if (newapp != null)
                {
                    if (CopiedAppointment != null && newapp.GetHashCode() == (CopiedAppointment.ObjectID))
                    {
                        CopiedAppointment = null;
                    }
                    if (newapp.IsRecursive)
                    {
                        RemoveRecursiveAppointment(newapp);
                    }
                    else
                    {
                        Remove(newapp.InternalStartTime.Date, newapp);
                        SetCurrentVisibleAppointments(CurrentSelectedDates);
                        SetPrevVisibleAppointments(PrevSelectedDates);
                        SetNextVisibleAppointments(NextSelectedDates);
                        if (newapp == SelectedAppointment || (newapp.IsRecursive && newapp.RecurrenceID.Equals(SelectedAppointment.RecurrenceID)))
                            SelectedAppointment = null;
                        if (SelectedAppointment == null && editpopup != null)
                            editpopup.IsOpen = false;
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                ClearAppointments();
            }
            GetAppointmentCollectionChangedEvent(e);
        }

        void GetAppointmentCollectionChangedEvent(NotifyCollectionChangedEventArgs e)
        {
            if (AppointmentCollectionChanged != null)
                AppointmentCollectionChanged(this, e);
        }

        #endregion

        #region ScheduleDateRange collection changed event

        void ScheduleDateRange_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (ScheduleDateRange != null && ScheduleDateRange.Count > 0)
            {
                VisibleDates = new ObservableCollection<DateTime>(ScheduleDateRange.OrderBy(p => p.Date));
            }
        }

        #endregion

        #region ResourceType Collection Changed Event

        void ScheduleResourceTypeCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ScheduleResourceType = ScheduleResourceTypeCollection.FirstOrDefault(res => (res.TypeName == Resource));
#if WPF
            if (appointmentEditor != null)
            {
                appointmentEditor.ScheduleResourceTypeCollection = ScheduleResourceTypeCollection;
            }
#endif
        }

        #endregion

        #region Reminder Events

        public delegate void ReminderControlOpeningEventHandler(object sender, ReminderControlOpeningEventArgs e);
        public delegate void ReminderControlClosedEventHandler(object sender, ReminderControlClosedEventArgs e);

        public event ReminderControlClosedEventHandler ReminderClosed;
        public event ReminderControlOpeningEventHandler ReminderOpening;

        void GetReminderControlOpeningEvents(ReminderControlOpeningEventArgs e)
        {
            if (ReminderOpening != null)
                ReminderOpening(this, e);
        }

        internal void GetReminderControlClosedEvents(ReminderControlClosedEventArgs e)
        {
            if (ReminderClosed != null)
                ReminderClosed(this, e);
        }

        void reminderTimer_Tick(object sender, EventArgs e)
        {
            reminderTimer.Stop();
            Reminder.ReminderAppointmentCollection = new ScheduleAppointmentCollection();
            foreach (ObservableCollection<ScheduleAppointment> coll in ProxyAppointments.Values)
            {
                foreach (ScheduleAppointment app in coll)
                {
                    if (app.ReminderTime != ReminderTimeType.None && app.ReminderDeliveryTime < DateTime.Now)
                    {
                        Reminder.ReminderAppointmentCollection.Add(app);
                    }
                }
            }

            if (Reminder.ReminderAppointmentCollection.Count > 0)
            {
                var args = new ReminderControlOpeningEventArgs();

                if (ItemsSource != null)
                {
                    var objCollection = new ObservableCollection<object>();
                    if (ItemsSource != null)
                    {
                        IEnumerable<object> source = null;
#if WPF
                        if (ItemsSource is DataRowCollection)
                            source = (ItemsSource as DataRowCollection).OfType<DataRow>();
                        else
#endif
                            source = (IEnumerable<object>)ItemsSource;
                        foreach (ScheduleAppointment schApp in Reminder.ReminderAppointmentCollection)
                        {
                            object obj = source.FirstOrDefault(x => x.GetHashCode() == (int)schApp.ObjectID);
                            if (obj != null)
                            {
                                objCollection.Add(obj);
                            }
                        }
                    }
                    args.RemindAppCollection = objCollection;
                }
                else
                {
                    args.RemindAppCollection = Reminder.ReminderAppointmentCollection;
                }
#if WPF
                if (!Reminder.IsActive)
                {

                    GetReminderControlOpeningEvents(args);
                    if (!args.Cancel)
                        Reminder.Show();

                }
#else
                GetReminderControlOpeningEvents(args);
                if (!args.Cancel)
                    Reminder.Show();
#endif
            }
            else
            {
                TriggerReminder();
            }
        }

        #endregion

        #region Appointment Editor Events

        public delegate void ScheduleAppointmentOpeningEventHandler(object sender, AppointmentEditorOpeningEventArgs e);
        public delegate void ScheduleAppointmentClosedEventHandler(object sender, AppointmentEditorClosedEventArgs e);

        public event ScheduleAppointmentClosedEventHandler AppointmentEditorClosed;
        public event ScheduleAppointmentOpeningEventHandler AppointmentEditorOpening;

        /// <summary>
        /// Gets the appointment window opening events.
        /// </summary>
        /// <param name="e">The <see cref="Syncfusion.UI.Xaml.Schedule.AppointmentEditorOpeningEventArgs"/> instance containing the event data.</param>
        internal void GetAppointmentWindowOpeningEvents(AppointmentEditorOpeningEventArgs e)
        {
            if (AppointmentEditorOpening != null)
                AppointmentEditorOpening(this, e);
        }

        /// <summary>
        /// Gets the appointment window closed events.
        /// </summary>
        /// <param name="e">The <see cref="Syncfusion.UI.Xaml.Schedule.AppointmentEditorClosedEventArgs"/> instance containing the event data.</param>
        internal void GetAppointmentWindowClosedEvents(AppointmentEditorClosedEventArgs e)
        {
            if (AppointmentEditorClosed != null)
                AppointmentEditorClosed(this, e);
        }

#if WPF
        void OpenOne_Click(object sender, RoutedEventArgs e)
        {
            if (editorvisibility == Visibility.Visible)
            {
                if (RecurringPopup.One.IsChecked != null && (bool)RecurringPopup.One.IsChecked)
                {
                    OpenOne();
                }
                else if (RecurringPopup.series.IsChecked != null && (bool)RecurringPopup.series.IsChecked)
                {
                    OpenSeries();
                }
            }
        }

        void Delete_Click(object sender, RoutedEventArgs e)
        {
            AppointmentEditorClosedEventArgs closedEventArgs = new AppointmentEditorClosedEventArgs
            {
                Action = EditorClosedAction.Delete,
                IsNew = appointmentEditor.isNew,
                OriginalAppointment = SelectedAppointment
            };
            GetAppointmentWindowClosedEvents(closedEventArgs);
            if(!closedEventArgs.Cancel)
                DeleteAppointment();
        }

        void Save_Click(object sender, RoutedEventArgs e)
        {
            AppointmentEditorClosedEventArgs closedEventArgs = new AppointmentEditorClosedEventArgs
            {
                Action = EditorClosedAction.Save,
                IsNew = appointmentEditor.isNew,
                OriginalAppointment = SelectedAppointment
            };
            SaveAppointment(closedEventArgs);
        }
#endif

        #endregion

        #region Context Menu Events

        public delegate void ContextMenuOpeningEventHandler(object sender, ContextMenuOpeningEventArgs e);
        public delegate void ContextMenuClosedEventHandler(object sender, ContextMenuClosedEventArgs e);

        public event ContextMenuClosedEventHandler ContextMenuClosed;
#if WPF
        public new event ContextMenuOpeningEventHandler ContextMenuOpening;
#else
        public event ContextMenuOpeningEventHandler ContextMenuOpening;
#endif

        /// <summary>
        /// Gets the context menu opening event.
        /// </summary>
        /// <param name="e">The <see cref="Syncfusion.UI.Xaml.Schedule.ContextMenuOpeningEventArgs"/> instance containing the event data.</param>
        void GetContextMenuOpeningEvent(ContextMenuOpeningEventArgs e)
        {
            if (ContextMenuOpening != null)
                ContextMenuOpening(this, e);
        }

        #endregion

        #region Schedule Click Events

        public delegate void ScheduleClickEventHandler(object sender, ScheduleClickEventArgs e);
        public delegate void ScheduleDoubleClickEventHandler(object sender, ScheduleClickEventArgs e);

        public event ScheduleClickEventHandler ScheduleClick;
        public event ScheduleDoubleClickEventHandler ScheduleDoubleClick;

        internal void GetScheduleClickEvent(ScheduleClickEventArgs e)
        {
            if (ScheduleClick != null)
                ScheduleClick(this, e);
        }

        internal void GetScheduleDoubleClickEvent(ScheduleClickEventArgs e)
        {
            if (ScheduleDoubleClick != null)
                ScheduleDoubleClick(this, e);
        }

        #endregion

        #region Context Menu MouseLeave Event
#if SILVERLIGHT
        void box_MouseLeave(object sender, MouseEventArgs e)
        {
            isTouchMenuHovered = false;
        }
#endif

        #endregion

        #region Context Menu Opening Event

        void SfSchedule_ContextMenuOpening(object sender, ContextMenuOpeningEventArgs e)
        {
            isContextMenuAltered = e.Cancel;
            if (e.Cancel)
            {
                AddnewContextmenuPopup.Visibility = Visibility.Collapsed;
                contextmenupopup.Visibility = Visibility.Collapsed;

                addnewpopup.Visibility = Visibility.Collapsed;
                addnewpopup.Child.Visibility = Visibility.Collapsed;

                editpopup.Visibility = Visibility.Collapsed;
                editpopup.Child.Visibility = Visibility.Collapsed;
            }
            else
            {
                AddnewContextmenuPopup.Visibility = Visibility.Visible;
                contextmenupopup.Visibility = Visibility.Visible;

                addnewpopup.Visibility = Visibility.Visible;
                addnewpopup.Child.Visibility = Visibility.Visible;

                editpopup.Visibility = Visibility.Visible;
                editpopup.Child.Visibility = Visibility.Visible;
            }
        }

        #endregion

#if SILVERLIGHT
        #region Context Menu SelectionChanged Event

        void box_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            isTouchMenuHovered = true;
            if (sender is ListBox && (sender as ListBox).SelectedIndex != -1)
            {
                var listBoxItem = (sender as ListBox).SelectedItem as ListBoxItem;
                if (listBoxItem != null)
                {
                    var stackPanel = listBoxItem.Content as StackPanel;
                    if (stackPanel != null)
                    {
                        var textBlock = stackPanel.Children[1] as TextBlock;
                        if (textBlock != null)
                        {
                            string str = textBlock.Text;
                            if (str.Equals("Edit"))
                            {
                                EditAppointment();
                            }
                            else if (str.Equals("Add New"))
                            {
                                AddNewAppointment();
                            }
                            else if (str.Equals("Delete"))
                            {
                                DeleteAppointment();
                            }
                            else if (str.Equals("Resize") || str.Equals("DragDrop"))
                            {
                                ResizeAppointment();

                            }
                            else if (str.Equals("Copy"))
                            {
                                CopiedAppointment = (ScheduleAppointment)AppointmentCloning(SelectedAppointment);
                                CopiedAppointment.ObjectID = SelectedAppointment.GetHashCode();
                                var listBox = AddnewContextmenuPopup.Child as ListBox;
                                if (listBox != null && listBox.Items.Count == 1)
                                {
                                    listBox.Items.Add(new ListBoxItem { Content = GetContextMenuItemContent("Paste", "Paste.png") });
                                }
                            }
                            else if (str.Equals("Paste"))
                            {
                                if (CopiedAppointment != null)
                                {
                                    var app = (ScheduleAppointment)AppointmentCloning(CopiedAppointment);
                                    if (SelectedAppointment != null)
                                    {
                                        SelectedAppointment.IsSelected = false;
                                        SelectedAppointment.AppointmentSelectionBrush = new SolidColorBrush(Colors.Transparent);
                                    }
                                    DateTime date = Currentselecteddate;
                                    DateTime startime = CopiedAppointment.StartTime;
                                    DateTime endtime = CopiedAppointment.EndTime;
                                    AddResources(app);
                                    if (ScheduleType == ScheduleType.Month)
                                    {
                                        if (startime == endtime)
                                        {
                                            app.AllDay = true;
                                        }
                                        app.StartTime = Currentselecteddate.AddMinutes(startime.Minute);
                                        app.EndTime = Currentselecteddate.Add(endtime - startime);
                                    }
                                    else if (allDayFlag)
                                    {
                                        allDayTouch = false;
                                        app.AllDay = true;
                                        app.StartTime = date;
                                        app.EndTime = date;
                                    }
                                    else
                                    {
                                        app.StartTime = date;
                                        if (endtime  != startime)
                                        {
                                            app.EndTime = date.Add(endtime - startime);
                                        }
                                        else                                        
                                        {
                                            app.EndTime = date.Add(GetTimeInterval());
                                        }
                                        
                                    }
                                    SelectedAppointment = app;
                                    app.IsSelected = true;
                                    app.AppointmentSelectionBrush = new SolidColorBrush(Colors.Black);
                                    Appointments.Add(app);
                                }

                            }
                        }
                    }
                }
                contextmenupopup.IsOpen = false;
                AddnewContextmenuPopup.IsOpen = false;
                (sender as ListBox).SelectedIndex = -1;
            }
        }

        #endregion

        #region MouseDoubleClick Custom Event

        void InitDoubleClickTimer()
        {
            doubleClickTimer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 0, 200) };
            doubleClickTimer.Tick += doubleClickTimer_Tick;
        }

        void doubleClickTimer_Tick(object sender, EventArgs e)
        {
            doubleClickTimer.Stop();
        }

        /// <summary>
        /// Occurs when mouse double clicked
        /// </summary>
        public event MouseButtonEventHandler MouseDoubleClick;

        /// <summary>
        ///  this virtual method is called when mouse double clicked
        /// </summary>
        /// <param name="e">An <see cref="T:System.Windows.Input.MouseButtonEventArgs"/>
        /// that contains the event data.</param>
        protected virtual void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            var handler = MouseDoubleClick;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        #endregion

        #region Appointment Related Events

        void RecurringPopup_Closed(object sender, EventArgs e)
        {
            if (RecurringPopup.One.IsChecked != null && ((bool)RecurringPopup.One.IsChecked && RecurringPopup.IsSeriesClicked == false))
            {
                OpenOne();
            }
            else if (RecurringPopup.series.IsChecked != null && (bool)RecurringPopup.series.IsChecked)
            {
                OpenSeries();
            }
        }       

        #endregion
#else
        void InitDoubleClickTimer()
        {
            doubleClickTimer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 0, 200) };
            doubleClickTimer.Tick += doubleClickTimer_Tick;
        }

        void doubleClickTimer_Tick(object sender, EventArgs e)
        {
            doubleClickTimer.Stop();

        }

        void item_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem)
            {
                string str = (sender as MenuItem).Header.ToString();
                if (str.Equals("Edit"))
                {
                    EditAppointment();
                }
                else if (str.Equals("Add New"))
                {
                    AddNewAppointment();
                }
                else if (str.Equals("Delete"))
                {
                    DeleteAppointment();
                }
                else if (str.Equals("Resize") || str.Equals("DragDrop"))
                {
                    ResizeAppointment();

                }
                else if (str.Equals("Copy"))
                {
                    CopiedAppointment = (ScheduleAppointment)AppointmentCloning(SelectedAppointment);
                    CopiedAppointment.ObjectID = SelectedAppointment.GetHashCode();
                    if (AddnewContextmenuPopup.Items.Count == 1)
                    {
                        var item = new MenuItem { Header = "Paste", Icon = GetIcon("Paste.png") };
                        item.Click += item_Click;
                        AddnewContextmenuPopup.Items.Add(item);
                    }
                }
                else if (str.Equals("Paste"))
                {
                    if (CopiedAppointment != null)
                    {
                        var app = (ScheduleAppointment)AppointmentCloning(CopiedAppointment);
                        if (SelectedAppointment != null)
                        {
                            SelectedAppointment.IsSelected = false;
                            SelectedAppointment.AppointmentSelectionBrush = new SolidColorBrush(Colors.Transparent);
                        }
                        DateTime date = Currentselecteddate;
                        DateTime startime = CopiedAppointment.StartTime;
                        DateTime endtime = CopiedAppointment.EndTime;
                        AddResources(app);
                        if (ScheduleType == ScheduleType.Month)
                        {
                            if (startime == endtime)
                            {
                                app.AllDay = true;
                            }
                            app.StartTime = Currentselecteddate.AddMinutes(startime.Minute);
                            app.EndTime = Currentselecteddate.Add(endtime - startime);
                        }
                        else if (allDayTouch)
                        {
                            allDayTouch = false;
                            app.AllDay = true;
                            app.StartTime = date;
                            app.EndTime = date;
                        }
                        else
                        {
                            app.StartTime = date;
                            if (endtime != startime)
                            {
                                app.EndTime = date.Add(endtime - startime);
                            }
                            else
                            {
                                app.EndTime = date.Add(GetTimeInterval());
                            }
                        }
                        SelectedAppointment = app;
                        app.IsSelected = true;
                        app.AppointmentSelectionBrush = new SolidColorBrush(Colors.Black);
                        Appointments.Add(app);
                    }
                }
                ContextMenu.IsOpen = false;
            }
        }
#endif

        #endregion

        #region Overrides

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            DragDropCanvas = GetTemplateChild("DragDropCanvas") as Canvas;
            mainItem = GetTemplateChild("PART_MainItem") as Grid;
            mainViewItem = GetTemplateChild("PART_MainViewItems") as ContentControl;
            headerTitleBarView = GetTemplateChild("PART_HeaderTitleBarView") as HeaderTitleBarView;
#if WPF
            
            mainViewItem1 = GetTemplateChild("PART_MainViewItems1") as ContentControl;
            mainViewItem2 = GetTemplateChild("PART_MainViewItems2") as ContentControl;
            headerTitleBarView1 = GetTemplateChild("PART_HeaderTitleBarView1") as HeaderTitleBarView;
            headerTitleBarView2 = GetTemplateChild("PART_HeaderTitleBarView2") as HeaderTitleBarView;
            Prev_Button = GetTemplateChild("PreviousButtonHorizontal") as Button;
            Next_Button = GetTemplateChild("NextButtonHorizontal") as Button;
            flipview = GetTemplateChild("Mainflip") as ItemsControl;
            flipviewselecteditem = flipview.Items[1] as FrameworkElement;
            currentSelectedItem = mainViewItem1;
#else
            currentSelectedItem = mainViewItem;
#endif
            if (headerTitleBarView != null)
            {
                headerTitleBarView.schedule = this;
#if WPF
                headerTitleBarView1.schedule = this;
                headerTitleBarView2.schedule = this;

                if (EnableTouch)
                {
                    Next_Button.Visibility = Visibility.Visible;
                    Prev_Button.Visibility = Visibility.Visible;
                    headerTitleBarView.HeaderNavigationButtonVisibility = Visibility.Collapsed;
                    headerTitleBarView1.HeaderNavigationButtonVisibility = Visibility.Collapsed;
                    headerTitleBarView2.HeaderNavigationButtonVisibility = Visibility.Collapsed;
                }
                else
                {
                    Next_Button.Visibility = Visibility.Collapsed;
                    Prev_Button.Visibility = Visibility.Collapsed;
                    headerTitleBarView.HeaderNavigationButtonVisibility = Visibility.Visible;
                    headerTitleBarView1.HeaderNavigationButtonVisibility = Visibility.Visible;
                    headerTitleBarView2.HeaderNavigationButtonVisibility = Visibility.Visible;
                }
#endif
            }

#if SILVERLIGHT
            contextmenupopup = GetTemplateChild("contextmenupopup") as Popup;
            AddnewContextmenuPopup = GetTemplateChild("AddnewContextmenuPopup") as Popup;

            var box = new ListBox { Width = 100 };
            box.SelectionChanged += box_SelectionChanged;
            box.MouseLeave += box_MouseLeave;
            box.Items.Add(new ListBoxItem { Content = GetContextMenuItemContent("Add New", "AddNew.png") });
            box.Items.Add(new ListBoxItem { Content = GetContextMenuItemContent("Copy", "Copy.png") });
            box.Items.Add(new ListBoxItem { Content = GetContextMenuItemContent("Edit", "Edit.png") });
            box.Items.Add(new ListBoxItem { Content = GetContextMenuItemContent("Delete", "Delete.png") });
            box.Items.Add(new ListBoxItem { Content = GetContextMenuItemContent("Resize", "Resize.png") });

            var box1 = new ListBox { Width = 100 };
            box1.SelectionChanged += box_SelectionChanged;
            box1.MouseLeave += box_MouseLeave;
            box1.Items.Add(new ListBoxItem { Content = GetContextMenuItemContent("Add New", "AddNew.png") });
            box1.SelectionChanged += box_SelectionChanged;

            if (contextmenupopup != null)
                contextmenupopup.Child = box;
            if (AddnewContextmenuPopup != null)
                AddnewContextmenuPopup.Child = box1;
#else
            if (appointmentEditor != null)
            {
                appointmentEditor.ScheduleResourceTypeCollection = ScheduleResourceTypeCollection;
                appointmentEditor.TimeZoneCollection = TimeZoneCollection;
            }
            contextmenupopup = new ContextMenu();
            contextmenupopup.Closed += ContextMenu_Closed;
            contextmenupopup.Items.Add(new MenuItem { Header = "Add New", Icon = GetIcon("AddNew.png") });
            contextmenupopup.Items.Add(new MenuItem { Header = "Copy", Icon = GetIcon("Copy.png") });
            contextmenupopup.Items.Add(new MenuItem { Header = "Edit", Icon = GetIcon("Edit.png") });
            contextmenupopup.Items.Add(new MenuItem { Header = "Delete", Icon = GetIcon("Delete.png") });
            contextmenupopup.Items.Add(new MenuItem { Header = "Resize", Icon = GetIcon("Resize.png") });

            AddnewContextmenuPopup = new ContextMenu();
            AddnewContextmenuPopup.Closed += ContextMenu_Closed;
            foreach (MenuItem item in contextmenupopup.Items)
            {
                item.Click += item_Click;
            }
            var menuitem = new MenuItem { Header = "Add New", Icon = GetIcon("AddNew.png") };
            menuitem.Click += item_Click;
            AddnewContextmenuPopup.Items.Add(menuitem);
#endif
            editpopup = GetTemplateChild("editpopup") as Popup;
            if(editpopup != null)
                editpopup.Closed += ContextMenu_Closed;
            addnewpopup = GetTemplateChild("addnewpopup") as Popup;
            if (addnewpopup != null)
                addnewpopup.Closed += ContextMenu_Closed;
#if WPF
            if (editpopup != null)
                editpopup.StaysOpen = false;
            if (addnewpopup != null)
                addnewpopup.StaysOpen = false;
#endif
            if (TouchMenuType == MenuType.Default)
            {
                var appcontrol = new AddAppintmentControl();
                if (addnewpopup != null)
                {
                    addnewpopup.Child = appcontrol;
                    addnewpopup.UpdateLayout();
                }
                var drag = new DragDropControl();
                if (editpopup != null)
                {
                    editpopup.Child = drag;
                    editpopup.UpdateLayout();
                }
            }
            else
            {
                if (addnewpopup != null)
                    addnewpopup.Child = new AddRadialMenuControl();
                if (editpopup != null)
                    editpopup.Child = new EditRadialMenuControl();
            }
            isTemplateApplied = true;
            UpdateScheduleType();
#if WPF
            if (mainViewItem1 != null)
            {

                mainViewItem1.PreviewMouseLeftButtonUp += mainViewItem_MouseLeftButtonUp;
                mainViewItem1.MouseDoubleClick += SfSchedule_MouseDoubleClick;
                mainViewItem.PreviewMouseLeftButtonUp += mainViewItem_MouseLeftButtonUp;
                mainViewItem.MouseDoubleClick += SfSchedule_MouseDoubleClick;
                mainViewItem2.PreviewMouseLeftButtonUp += mainViewItem_MouseLeftButtonUp;
                mainViewItem2.MouseDoubleClick += SfSchedule_MouseDoubleClick;
            
#else            
            if (mainViewItem != null)
            {
                currentSelectedItem.MouseLeftButtonUp += mainViewItem_MouseLeftButtonUp;
                MouseDoubleClick += SfSchedule_MouseDoubleClick;
#endif

            }

            if (DragDropCanvas == null) return;
            DragDropCanvas.MouseLeftButtonDown += DragDropCanvas_MouseLeftButtonDown;
            DragDropCanvas.MouseLeftButtonUp += DragDropCanvas_MouseLeftButtonUp;
            DragDropCanvas.MouseMove += DragDropCanvas_MouseMove;
            AppointmentEditorOpening += SfSchedule_AppointmentEditorOpening;
            ContextMenuOpening += SfSchedule_ContextMenuOpening;
#if SILVERLIGHT
            isScheduleLoaded = true;
#endif
        }

        void appointmentEditor_Closed(object sender, EventArgs e)
        {
#if SILVERLIGHT
            AppointmentEditorClosedEventArgs closedEventArgs = new AppointmentEditorClosedEventArgs
            {
                IsNew = appointmentEditor.isNew,
                OriginalAppointment = SelectedAppointment
            };
            if (appointmentEditor.DialogResult == true && appointmentEditor.IsDeleteClicked)
            {
                closedEventArgs.Action = EditorClosedAction.Delete;
                closedEventArgs.EditedAppointment = null;
                GetAppointmentWindowClosedEvents(closedEventArgs);
                if (!closedEventArgs.Cancel)
                    DeleteAppointment();

            }
            else if (appointmentEditor.DialogResult == true)
            {
                closedEventArgs.Action = EditorClosedAction.Save;
                SaveAppointment(closedEventArgs);
            }
            else
            {
                closedEventArgs.Action = EditorClosedAction.Cancel;
                closedEventArgs.EditedAppointment = null;
                GetAppointmentWindowClosedEvents(closedEventArgs);
            }
#else
            if (appointmentEditor.DialogResult == false)
            {
                AppointmentEditorClosedEventArgs closedEventArgs = new AppointmentEditorClosedEventArgs
                {
                    Action = EditorClosedAction.Cancel,
                    IsNew = appointmentEditor.isNew,
                    OriginalAppointment = SelectedAppointment,
                    EditedAppointment = null,
                };
                GetAppointmentWindowClosedEvents(closedEventArgs);
            }
#endif
        }

        void ContextMenu_Closed(object sender, EventArgs e)
        {
            if (ContextMenuClosed != null)
                ContextMenuClosed(this, new ContextMenuClosedEventArgs());
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            #region DragDrop

            if (IsDragEnabled && DragDropCanvas.Children.Count > 0)
            {
                if (currentSelectedItem.Content is ScheduleDaysView && dvc != null)
                {
                    (currentSelectedItem.Content as ScheduleDaysView).ReleaseDragDrop(this, e);
                }
                else if (currentSelectedItem.Content is ScheduleMonthView && mvc != null)
                {
#if WPF
                    Point Overallposition = e.GetPosition(monthview);
                    hitTestList = new List<DependencyObject>();
                    VisualTreeHelper.HitTest(monthview, null, CollectAllVisuals_Callback, new PointHitTestParameters(Overallposition));
                    hitTestList.Reverse();
#endif
                    (currentSelectedItem.Content as ScheduleMonthView).ReleaseDragDrop(this, e);

                }
                else if (currentSelectedItem.Content is ScheduleTimeLineView && hvc != null)
                {
                    (currentSelectedItem.Content as ScheduleTimeLineView).ReleaseDragDrop(this, e);
                }
            }

            var args = new ScheduleClickEventArgs();
            if (SelectedAppointment != null)
            {
                args.Appointment = SelectedAppointment;
                args.SelectedDate = Currentselecteddate;
                args.SelectedResource = SelectedAppointment.ResourceCollection.ToList();
            }
            else
            {
                args.SelectedDate = isMainViewItemClicked ? Currentselecteddate : SelectedDate;
                args.SelectedResource = selectedResourcename;
            }
            GetScheduleClickEvent(args);
            isMainViewItemClicked = false;

            #endregion

            var frameworkElement = e.OriginalSource as FrameworkElement;
            if (frameworkElement != null && (e.OriginalSource != null && !(frameworkElement.DataContext is ScheduleAppointment)))
            {
                if (DragDropCanvas.Children.Count > 0)
                {
                    if (SelectedAppointment != null && IsDragEnabled)
                    {
                        SelectedAppointment.AppointmentSelectionBrush = new SolidColorBrush(Colors.Transparent);
                        SelectedAppointment.IsSelected = false;
                        SelectedAppointment = null;
                        ClearDragDropCanvas();
                    }
                }
                else
                {
                    if (SelectedAppointment != null)
                    {
                        SelectedAppointment.AppointmentSelectionBrush = new SolidColorBrush(Colors.Transparent);
                        SelectedAppointment.IsSelected = false;
                        SelectedAppointment = null;
                        ClearDragDropCanvas();
                    }
                }
            }
        }

#if SILVERLIGHT
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            Thread.Sleep(100);
            if (!isTouchMenuHovered)
            {
                if (EnableTouch)
                {
                    editpopup.IsOpen = false;
                    addnewpopup.IsOpen = false;
                }
                else
                {
                    AddnewContextmenuPopup.IsOpen = false;
                    contextmenupopup.IsOpen = false;
                }
            }
        }

        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            OnMousePressDown(e);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            OnMousePressDown(e);
        }
#else
        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            OnMousePressDown(e);
        }
#endif

        private void InvokeScheduleDoubleClickEvent()
        {
            var args = new ScheduleClickEventArgs();
            if (SelectedAppointment != null)
            {
                args.Appointment = SelectedAppointment;
                args.SelectedDate = Currentselecteddate;
                args.SelectedResource = SelectedAppointment.ResourceCollection.ToList();
            }
            else
            {
                args.SelectedDate = isMainViewItemClicked ? Currentselecteddate : SelectedDate;
                args.SelectedResource = selectedResourcename;
            }
            GetScheduleDoubleClickEvent(args);
            isMainViewItemClicked = false;
        }

        internal void OnMousePressDown(MouseButtonEventArgs e)
        {
            var currentOriginalSource = e.OriginalSource;
            if (ItemsSource != null && AppointmentMapping != null && AppointmentTemplate != null && currentOriginalSource is FrameworkElement)
            {
                currentOriginalSource = FindContentControl(currentOriginalSource as FrameworkElement);
            }
            allDayFlag = false;
            AddnewContextmenuPopup.IsOpen = false;
            contextmenupopup.IsOpen = false;
            if (editpopup != null)
            {
                var frameworkElement = currentOriginalSource as FrameworkElement;
                if (frameworkElement != null && !(frameworkElement.DataContext is ScheduleAppointment))
                {
                    editpopup.IsOpen = false;
                    isAppointment = true;
                }
                else
                {
                    isAppointment = false;
                }
            }
            if (currentSelectedItem.Content is ScheduleDaysView && (ScheduleType == ScheduleType.Week || ScheduleType == ScheduleType.Day))
            {
                var scheduleDaysView = currentSelectedItem.Content as ScheduleDaysView;
                if (VisualTreeHelper.GetChildrenCount(scheduleDaysView) != 0)
                {
                    var obj = VisualTreeHelper.GetChild(scheduleDaysView, 0) as Grid;
                    if (obj != null)
                    {
                        var DaysScrollviewer = obj.FindName("Scrollviewer") as ScrollViewer;
                        if (DaysScrollviewer != null)
                        {
                            double CurrentPositionInScroll = e.GetPosition(DaysScrollviewer).Y;
                            if (CurrentPositionInScroll <= 0)
                            {
                                scheduleDaysView.dayCanvasFromAllday = true;
                            }
                        }
                    }
                }
            }
            if (addnewpopup != null && addnewpopup.IsOpen)
            {
                addnewpopup.IsOpen = false;
            }
            if ((currentOriginalSource is FrameworkElement && (currentOriginalSource as FrameworkElement).DataContext is ScheduleAppointment))
            {
                if (SelectedAppointment != null)
                {
                    SelectedAppointment.IsSelected = false;
                    SelectedAppointment.AppointmentSelectionBrush = new SolidColorBrush(Colors.Transparent);
                }

                SelectedAppointment = (currentOriginalSource as FrameworkElement).DataContext as ScheduleAppointment;
                if (SelectedAppointment != null)
                {
                    SelectedAppointment.IsSelected = true;
                    SelectedAppointment.AppointmentSelectionBrush = AppointmentSelectionBrush;
                }

                mvc = (currentOriginalSource as FrameworkElement).FindParentElementOfType<ScheduleMonthAppointmentViewControl>();
                dvc = (currentOriginalSource as FrameworkElement).FindParentElementOfType<ScheduleDaysAppointmentViewControl>();
                hvc = (currentOriginalSource as FrameworkElement).FindParentElementOfType<ScheduleHorizontalAppointmentViewControl>();
                if (mvc != null)
                {
                    SelectedAppointment = mvc.DataContext as ScheduleAppointment;
                    if (SelectedAppointment != null)
                    {
                        SelectedAppointment.AppointmentSelectionBrush = AppointmentSelectionBrush;
                        SelectedAppointment.IsSelected = true;
                    }
                    FloatingAppointmentSize = mvc.RenderSize;
                    Appointmentpoint = e.GetPosition(mvc);
                    currentpoint = e.GetPosition(DragDropCanvas);
                    SelectedPoint = Appointmentpoint;
                }
                else if (dvc != null)
                {
                    SelectedAppointment = dvc.DataContext as ScheduleAppointment;
                    currentpoint = e.GetPosition(DragDropCanvas);
                    Appointmentpoint = e.GetPosition(dvc);
                    Scrollpoint = e.GetPosition((currentSelectedItem.Content as ScheduleDaysView).scrollviewer);
                    if (SelectedAppointment != null)
                    {
                        SelectedAppointment.AppointmentSelectionBrush = AppointmentSelectionBrush;
                        SelectedAppointment.IsSelected = true;
                    }
                    if (SelectedAppointment.InternalEndTime.Date > SelectedAppointment.InternalStartTime.Date
                        || (currentpoint.Y - Appointmentpoint.Y <= DragDropCanvas.ActualHeight - (currentSelectedItem.Content as ScheduleDaysView).scrollviewer.ViewportHeight - 5)
                        || Scrollpoint.Y - Appointmentpoint.Y + FloatingAppointmentSize.Height > (currentSelectedItem.Content as ScheduleDaysView).scrollviewer.ViewportHeight)
                    {
                        TimeSpan diffTime = SelectedAppointment.InternalEndTime - SelectedAppointment.InternalStartTime;
                        (currentSelectedItem.Content as ScheduleDaysView).dragDropCanvasHeight = diffTime.TotalMinutes / GetTimeInterval().TotalMinutes * IntervalHeight;
                        if (SelectedAppointment.AllDay)
                            (currentSelectedItem.Content as ScheduleDaysView).dragDropCanvasHeight = 30;

                    }
                    else
                    {
                        (currentSelectedItem.Content as ScheduleDaysView).dragDropCanvasHeight = dvc.RenderSize.Height;
                    }
                    (currentSelectedItem.Content as ScheduleDaysView).visibleCanvasHeight = dvc.RenderSize.Height;
                    FloatingAppointmentSize = dvc.RenderSize;
                }
                else if (hvc != null)
                {
                    SelectedAppointment = hvc.DataContext as ScheduleAppointment;
                    Appointmentpoint = e.GetPosition(hvc);
                    Scrollpoint = e.GetPosition((viewcontrol.Content as ScheduleTimeLineView).timelinescroll);
                    currentpoint = e.GetPosition(DragDropCanvas);
                    if (SelectedAppointment != null)
                    {
                        SelectedAppointment.AppointmentSelectionBrush = AppointmentSelectionBrush;
                        SelectedAppointment.IsSelected = true;
                    }
                    double timelineIntervalHeight;
                    if (isIntervalHeightset)
                        timelineIntervalHeight = IntervalHeight;
                    else
                        timelineIntervalHeight = 80;
                    var timelineview = (viewcontrol.Content as ScheduleTimeLineView);
                    double leftend;
                    if (ScheduleResourceType != null && ScheduleResourceType.ResourceCollection.Count > 0)
                    {
                        double timeLineViewItemHeaderWidth = timelineview.FindElementOfType<TimeLineViewItemHeader>().ActualWidth;
                        leftend = DragDropCanvas.ActualWidth - timelineview.timelinescroll.ViewportWidth + timeLineViewItemHeaderWidth - 5;
                    }
                    else
                    {
                        leftend = DragDropCanvas.ActualWidth - timelineview.timelinescroll.ViewportWidth - 5;
                    }
                    if (currentpoint.X - Appointmentpoint.X <= leftend || Scrollpoint.X - Appointmentpoint.X + FloatingAppointmentSize.Width > timelineview.timelinescroll.ViewportWidth)
                    {
                        TimeSpan diffTime = SelectedAppointment.InternalEndTime - SelectedAppointment.InternalStartTime;
                        timelineview.dragDropCanvasWidth = diffTime.TotalMinutes / GetTimeInterval().TotalMinutes * timelineIntervalHeight;
                    }
                    else
                    {
                        timelineview.dragDropCanvasWidth = hvc.RenderSize.Width;
                    }
                    timelineview.visibleCanvasWidth = hvc.RenderSize.Width;
                    FloatingAppointmentSize = hvc.RenderSize;

                }
            }
            else
            {
                if (!contextMenuVisible)
                {
                    ClearDragDropCanvas();
                }
                else
                    contextMenuVisible = false;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
#if WPF
            if (EnableTouch && e.StylusDevice == null)
            {
                Prev_Button.Visibility = Visibility.Visible;
                Next_Button.Visibility = Visibility.Visible;
            }
            else
            {
                Prev_Button.Visibility = Visibility.Collapsed;
                Next_Button.Visibility = Visibility.Collapsed;
            }
#endif
            if (!IsDragEnabled || DragDropCanvas.Children.Count == 0)
            {
                return;
            }
            if (mvc != null && currentSelectedItem.Content is ScheduleMonthView)
            {
                isscrollmoveondragging = true;
                (currentSelectedItem.Content as ScheduleMonthView).StartDragDrop(this, e);
            }
            else if (dvc != null && currentSelectedItem.Content is ScheduleDaysView)
            {
                isscrollmoveondragging = true;
                (currentSelectedItem.Content as ScheduleDaysView).StartDragDrop(this, e);
            }
            else if (hvc != null && currentSelectedItem.Content is ScheduleTimeLineView)
            {
                isscrollmoveondragging = true;
                (currentSelectedItem.Content as ScheduleTimeLineView).StartDragDrop(this, e);
            }
            else
            {
                isscrollmoveondragging = false;
            }

            base.OnMouseMove(e);
        }

#if SILVERLIGHT
        protected override void OnKeyUp(KeyEventArgs e)
        {
            //base.OnKeyUp(e);
            if (!EnableTouch)
            {
                if (e.OriginalSource.GetType() == typeof(ScrollViewer) && (e.Key == Key.Down || e.Key == Key.Up || e.Key == Key.Right || e.Key == Key.Left || e.Key == Key.PageDown || e.Key == Key.PageUp || e.Key == Key.Home || e.Key == Key.End))
                {
                    e.Handled = true;
                    if (e.Key == Key.Up)
                    {
                        if (Keyboard.Modifiers == ModifierKeys.Shift)
                        {
                            SLTimeSelectionfUpCmdExecuted(this);
                        }
                        else
                        {
                            //CommandBinding.commands[0].Execute += delegate() { SLCreateCmdExecuted(this); };
                            SLUpCmdExecuted(this);
                            //SLResizeAppointmentUpCmdExecuted(this);
                        }
                    }
                    else if (e.Key == Key.Down)
                    {
                        if (Keyboard.Modifiers == ModifierKeys.Shift)
                        {
                            SLTimeSelectionfDownCmdExecuted(this);
                        }
                        else
                        {
                            //CommandBinding.commands[0].Execute += delegate() { SLCreateCmdExecuted(this); };
                            SLDownCmdExecuted(this);
                        }
                    }
                    else if (e.Key == Key.Right)
                    {
                        if (Keyboard.Modifiers == ModifierKeys.Shift)
                        {
                            SLTimeSelectionRightCmdExecuted(this);
                        }
                        else
                        {
                            SLRightCmdExecuted(this);
                        }
                    }
                    else if (e.Key == Key.Left)
                    {
                        if (Keyboard.Modifiers == ModifierKeys.Shift)
                        {
                            SLTimeSelectionLeftCmdExecuted(this);
                        }
                        else
                        {
                            SLLeftCmdExecuted(this);
                        }
                    }
                    else if (e.Key == Key.PageDown)
                    {
                        SLPgDownCmdExecuted(this);
                    }
                    else if (e.Key == Key.PageUp)
                    {
                        SLPgUpCmdExecuted(this);
                    }
                    else if (e.Key == Key.Home)
                    {
                        SLHomeCmdExecuted(this);
                    }
                    else if (e.Key == Key.End)
                    {
                        SLEndCmdExecuted(this);
                    }

                }
            }
            else
            {
                base.OnKeyUp(e);
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (!EnableTouch)
            {
                if (e.OriginalSource.GetType() == typeof(ScrollViewer) && (e.Key == Key.Unknown))
                {
                    e.Handled = true;
                    if (e.PlatformKeyCode == 189 && (Keyboard.Modifiers == ModifierKeys.Alt))
                    {
                        SLZoomOutCmdExecuted(this);
                    }
                    else if (e.PlatformKeyCode == 187 && (Keyboard.Modifiers == ModifierKeys.Alt))
                    {
                        SLZoomInCmdExecuted(this);
                    }
                    else if (e.PlatformKeyCode == 188 && (Keyboard.Modifiers == ModifierKeys.Control))
                    {
                        SLSameDayPreviousWeekCmdExecuted(this);
                    }
                    else if (e.PlatformKeyCode == 190 && (Keyboard.Modifiers == ModifierKeys.Control))
                    {
                        SLSameDayNextWeekCmdExecuted(this);
                    }
                }
            }
            else
            {
                base.OnKeyDown(e);
            }
        }

#endif

#if WPF
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (!EnableTouch)
            {
                if (e.OriginalSource.GetType() == typeof(ScrollViewer) && (e.Key == Key.Down || e.Key == Key.Up || e.Key == Key.Right || e.Key == Key.Left || e.Key == Key.PageDown || e.Key == Key.PageUp || e.Key == Key.Home || e.Key == Key.End))
                {
                    e.Handled = true;
                    if (e.Key == Key.Down)
                    {
                        if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                        {
                            ScheduleCommands.TimeSelectionDown.Execute(null, this);
                        }
                        else
                        {
                            ScheduleCommands.Down.Execute(null, this);
                        }
                    }
                    else if (e.Key == Key.Up)
                    {
                        if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                        {
                            ScheduleCommands.TimeSelectionUp.Execute(null, this);
                        }
                        else
                        {
                            ScheduleCommands.Up.Execute(null, this);
                        }
                    }
                    else if (e.Key == Key.Right)
                    {
                        if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                        {
                            ScheduleCommands.TimeSelectionRight.Execute(null, this);
                        }
                        else
                        {
                            ScheduleCommands.Right.Execute(null, this);
                        }
                    }
                    else if (e.Key == Key.Left)
                    {
                        if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                        {
                            ScheduleCommands.TimeSelectionLeft.Execute(null, this);
                        }
                        else
                        {
                            ScheduleCommands.Left.Execute(null, this);
                        }
                    }
                    else if (e.Key == Key.PageDown)
                    {
                        ScheduleCommands.PgDown.Execute(null, this);
                    }
                    else if (e.Key == Key.PageUp)
                    {
                        ScheduleCommands.PgUp.Execute(null, this);
                    }
                    else if (e.Key == Key.Home)
                    {
                        ScheduleCommands.Home.Execute(null, this);
                    }
                    else if (e.Key == Key.End)
                    {
                        ScheduleCommands.End.Execute(null, this);
                    }
                }
            }
            else
            {
                base.OnPreviewKeyDown(e);
            }
        }
#endif
        #endregion
    }

    #endregion

    #region Appointment Editor EventArgs

    public class AppointmentEditorOpeningEventArgs : CancelEventArgs
    {
        #region CLR Properties

        #region Appointment

        public object Appointment { get; set; }

        #endregion

        #region StartTime

        public DateTime StartTime { get; set; }

        #endregion

        #region Action

        public EditorAction Action { get; set; }

        #endregion

        #region SelectedResource

        public List<Resource> SelectedResource { get; set; }

        #endregion

        #endregion
    }

    public class AppointmentEditorClosedEventArgs : CancelEventArgs
    {
        #region CLR Properties

        #region OriginalAppointment

        public object OriginalAppointment { get; set; }

        #endregion

        #region EditedAppointment

        public object EditedAppointment { get; set; }

        #endregion

        #region IsNew

        public bool IsNew { get; set; }

        #endregion

        #region Action

        public EditorClosedAction Action { get; set; }

        #endregion

        #endregion
    }

    #endregion

    #region Reminder Control EventArgs

    public class ReminderControlOpeningEventArgs : CancelEventArgs
    {
        #region CLR Properties

        #region RemindAppCollection

        public object RemindAppCollection { get; set; }

        #endregion

        #endregion
    }

    public class ReminderControlClosedEventArgs : CancelEventArgs
    {

    }

    #endregion

    #region Context Menu EventArgs

    public class ContextMenuOpeningEventArgs : CancelEventArgs
    {
        #region CLR Properties

        #region Appointment

        public object Appointment { get; set; }

        #endregion

        #region CurrentSelectedDate

        public DateTime? CurrentSelectedDate { get; set; }

        #endregion

        #region CurrentEventArgs

        public MouseButtonEventArgs CurrentEventArgs { get; set; }

        #endregion

        #region SelectedResource

        public List<Resource> SelectedResource { get; set; }

        #endregion

        #endregion
    }

    public class ContextMenuClosedEventArgs : CancelEventArgs
    {

    }

    #endregion

    #region Appointment Click EventArgs

    public class ScheduleClickEventArgs
    {
        #region CLR Properties

        #region Appointment

        public object Appointment { get; set; }

        #endregion

        #region SelectedDate

        public DateTime? SelectedDate { get; set; }

        #endregion

        #region SelectedResource

        public List<Resource> SelectedResource { get; set; }

        #endregion

        #endregion
    }

    #endregion
}
