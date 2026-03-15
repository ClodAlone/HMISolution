using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MSModel;
using System.Data;
using Opc.Ua;
using UFInterfaces;
using System.Threading.Tasks;

namespace MSModel
{
    public class SimpleScheduledEvent
    {

        #region Properties
        private DateTime _LastTime = DateTime.UtcNow;
        public DateTime LastTime
        {
            get { return _LastTime; }
            set
            {
                _LastTime = value;
            }
        }

        private Guid _Guid;
        public Guid Guid
        {
            get
            {
                return _Guid;
            }
            set
            {
                _Guid = value;
            }
        }

        private NodeId _NodeId;
        public NodeId NodeId
        {
            get
            {
                return _NodeId;
            }
            set
            {
                _NodeId = value;
            }
        }
        private bool _Enable;
        public bool Enable
        {
            get
            {
                return _Enable;
            }
            set
            {
                _Enable = value;
            }
        }
        private bool _RuntimeEnable;
        public bool RuntimeEnable
        {
            get { return _RuntimeEnable; }
            set
            {
                _RuntimeEnable = value;
            }
        }

        private string _FullName;
        public string FullName
        {
            get { return _FullName; }
            set { _FullName = value; }
        }
        private string _ScheduleVariable;
        public string ScheduleVariable
        {
            get { return _ScheduleVariable; }
            set { _ScheduleVariable = value; }
        }

        private string _EnableVariable;
        public string EnableVariable
        {
            get { return _EnableVariable; }
            set { _EnableVariable = value; }
        }

        private ScheduleType _Type;
        public ScheduleType Type
        {
            get
            {
                return _Type;
            }
            set
            {
                _Type = value;
            }
        }

        private DateTime _Time;
        public DateTime Time
        {
            get
            {
                return _Time;
            }
            set
            {
                _Time = value;
            }
        }
        private DateTime _Date;
        public DateTime Date
        {
            get
            {
                return _Date;
            }
            set
            {
                _Date = value;
            }
        }

        private DateTime _TimeOff;
        public DateTime TimeOff
        {
            get
            {
                return _TimeOff;
            }
            set
            {
                _TimeOff = value;
            }
        }
        private DateTime _DateOff;
        public DateTime DateOff
        {
            get
            {
                return _DateOff;
            }
            set
            {
                _DateOff = value;
            }
        }
        private List<SimpleSchedulingDate> _WeeklyCalendar = new List<SimpleSchedulingDate>();
        public List<SimpleSchedulingDate> WeeklyCalendar
        {
            get
            {
                return _WeeklyCalendar;
            }
        }

        private List<SimpleSchedulingDate> _Calendar = new List<SimpleSchedulingDate>();
        public List<SimpleSchedulingDate> Calendar
        {
            get
            {
                return _Calendar;
            }
        }

        private List<SimpleSchedulingDate> _ExceptionsCalendar = new List<SimpleSchedulingDate>();
        public List<SimpleSchedulingDate> ExceptionsCalendar
        {
            get
            {
                return _ExceptionsCalendar;
            }
        }

        private string _ValueOn;
        public string ValueOn
        {
            get { return _ValueOn; }
            set
            {
                _ValueOn = value;
            }
        }

        private string _ValueOff;
        public string ValueOff
        {
            get { return _ValueOff; }
            set
            {
                _ValueOff = value;
            }
        }

        private bool _ExecOnAtStartup;
        public bool ExecOnAtStartup
        {
            get { return _ExecOnAtStartup; }
            set { _ExecOnAtStartup = value; }
        }
        private bool _ExecOffAtStartup;
        public bool ExecOffAtStartup
        {
            get { return _ExecOffAtStartup; }
            set { _ExecOffAtStartup = value; }
        }

        private String _AccessRole;
        public String AccessRole
        {
            get { return _AccessRole; }
            set { _AccessRole = value; }
        }
        private int _AccessLevels;
        public int AccessLevels
        {
            get { return _AccessLevels; }
            set { _AccessLevels = value; }
        }

        private int _AccessMasks;
        public int AccessMasks
        {
            get { return _AccessMasks; }
            set { _AccessMasks = value; }
        }

        #endregion

        #region Methods



        public void UpdateAction(MSScheduledAction se, bool bRunningOnServer = false)
        {
            Guid = se.NodeId;
            if(se.Enable.HasValue)
                Enable = se.Enable.Value;
            //RuntimeEnable = se.RuntimeSelectable;
            FullName = se.FullName;
            ScheduleVariable = se.ScheduleItem;
            EnableVariable = se.EnableVariable;
            Type = se.Type;
            Time = se.Time;
            Date = se.Date;
            TimeOff = se.TimeOff;
            DateOff = se.DateOff;
            WeeklyCalendar.Clear();

            var bEditingWeeklyPlanOnWeb = bRunningOnServer && se.Type == ScheduleType.weeklyPlan;

            if (!bEditingWeeklyPlanOnWeb)
            {
                foreach (var w in se.WeeklyCalendar)
                {
                    WeeklyCalendar.Add(new SimpleSchedulingDate()
                    {
                        EndDate = w.EndDate,
                        StartDate = w.StartDate
                    });
                }
            }
                
            Calendar.Clear();
            foreach(var c in se.Calendar)
            {
                Calendar.Add(new SimpleSchedulingDate()
                {
                    DayOfWeek = c.DayOfWeek,
                    EndDate = c.EndDate,
                    Month = c.Month,
                    StartDate = c.StartDate,
                    Type = c.Type,
                    WeekOfMonth = c.WeekOfMonth,
                    DayOfMonth = c.DayOfMonth
                });
                if (bEditingWeeklyPlanOnWeb)
                {
                    WeeklyCalendar.Add(new SimpleSchedulingDate()
                    {
                        DayOfWeek = c.DayOfWeek,
                        EndDate = c.EndDate,
                        Month = CalendarMonthType.Any,
                        StartDate = c.StartDate,
                        Type = CalendarItemType.WeekNDate,
                        WeekOfMonth = CalendarWeekType.Any,
                        DayOfMonth = (int)MonthDayAny.NoDate
                    });
                }
            }

            ExceptionsCalendar.Clear();
            foreach (var c in se.ExceptionsCalendar)
            {
                ExceptionsCalendar.Add(new SimpleSchedulingDate()
                {
                    DayOfWeek = c.DayOfWeek,
                    EndDate = c.EndDate,
                    Month = c.Month,
                    StartDate = c.StartDate,
                    Type = c.Type,
                    WeekOfMonth = c.WeekOfMonth,
                    DayOfMonth = c.DayOfMonth
                });
            }

            ValueOn = se.ValueOn;
            ValueOff = se.ValueOff;
            if(se.ExecOnAtStartup.HasValue)
            ExecOnAtStartup = se.ExecOnAtStartup.Value;
            if(se.ExecOffAtStartup.HasValue)
            ExecOffAtStartup = se.ExecOffAtStartup.Value;
            AccessRole = se.AccessRole;
            AccessLevels = se.AccessLevels;
            AccessMasks = se.AccessMasks;
        }

        private object lockList = new object();
        public void Update(SimpleScheduledEvent se, bool bRunningOnServer = false)
        {
            LastTime = se.LastTime;
            NodeId = se.NodeId;
            Enable = se.Enable;
            FullName = se.FullName;
            ScheduleVariable = se.ScheduleVariable;
            EnableVariable = se.EnableVariable;
            Type = se.Type;
            Time = se.Time;
            Date = se.Date;
            TimeOff = se.TimeOff;
            DateOff = se.DateOff;

            ValueOn = se.ValueOn;
            ValueOff = se.ValueOff;
            ExecOnAtStartup = se.ExecOnAtStartup;
            ExecOffAtStartup = se.ExecOffAtStartup;
            AccessRole = se.AccessRole;
            AccessLevels = se.AccessLevels;
            AccessMasks = se.AccessMasks;

            Calendar.Clear();
            ExceptionsCalendar.Clear();
            WeeklyCalendar.Clear();

            //List<SimpleSchedulingDate> tWList = new List<SimpleSchedulingDate>(); ;
            //List<SimpleSchedulingDate> tCList = new List<SimpleSchedulingDate>();
            //if (se.WeeklyCalendar.Count > 0)
            //    tWList.AddRange(se.WeeklyCalendar);
            //if(se.Calendar.Count > 0)
            //    tCList.AddRange(se.Calendar);

            var bEditingWeeklyPlanOnWeb = bRunningOnServer && se.Type == ScheduleType.weeklyPlan;

            if (!bEditingWeeklyPlanOnWeb)
            {
                Parallel.ForEach(se.WeeklyCalendar, w =>
                {
                    var sd = new SimpleSchedulingDate()
                    {
                        DayOfWeek = w.DayOfWeek,
                        EndDate = w.EndDate,
                        StartDate = w.StartDate
                    };
                    lock (lockList)
                    {
                        WeeklyCalendar.Add(sd);
                    }
                });
            }

            Parallel.ForEach(se.Calendar, c => 
            {
                var sd = new SimpleSchedulingDate()
                {
                    DayOfWeek = c.DayOfWeek,
                    EndDate = c.EndDate,
                    Month = c.Month,
                    StartDate = c.StartDate,
                    Type = c.Type,
                    WeekOfMonth = c.WeekOfMonth,
                    DayOfMonth = c.DayOfMonth
                };
                lock(lockList)
                {
                    Calendar.Add(sd);
                    if (bEditingWeeklyPlanOnWeb)
                        WeeklyCalendar.Add(sd);
                }
            });

            Parallel.ForEach(se.ExceptionsCalendar, c =>
            {
                var sd = new SimpleSchedulingDate()
                {
                    DayOfWeek = c.DayOfWeek,
                    EndDate = c.EndDate,
                    Month = c.Month,
                    StartDate = c.StartDate,
                    Type = c.Type,
                    WeekOfMonth = c.WeekOfMonth,
                    DayOfMonth = c.DayOfMonth
                };
                lock (lockList)
                {
                    ExceptionsCalendar.Add(sd);
                }
            });
        }

        public bool Compare(SimpleScheduledEvent se)
        {
            bool bEqual = NodeId == se.NodeId && Enable == se.Enable &&
                RuntimeEnable == se.RuntimeEnable && FullName == se.FullName &&
                ScheduleVariable == se.ScheduleVariable && EnableVariable == se.EnableVariable &&
                Type == se.Type && Time == se.Time && Date == se.Date &&
                TimeOff == se.TimeOff && DateOff == se.DateOff && ValueOn == se.ValueOn &&
                ValueOff == se.ValueOff && ExecOnAtStartup == se.ExecOnAtStartup &&
                ExecOffAtStartup == se.ExecOffAtStartup && AccessRole == se.AccessRole &&
                AccessLevels == se.AccessLevels && AccessMasks == se.AccessMasks;

            List<SimpleSchedulingDate> tmpW = new List<SimpleSchedulingDate>();
            List<SimpleSchedulingDate> tmpC = new List<SimpleSchedulingDate>();
            List<SimpleSchedulingDate> tmpEC = new List<SimpleSchedulingDate>();
            tmpC.AddRange(se.Calendar);
            tmpEC.AddRange(se.ExceptionsCalendar);
            tmpW.AddRange(se.WeeklyCalendar);

                bEqual = bEqual && WeeklyCalendar.Count == tmpW.Count &&
                    Calendar.Count == tmpC.Count && ExceptionsCalendar.Count == tmpEC.Count;
                
            if (bEqual)
            {
                Parallel.ForEach(tmpW, ci => 
                {
                    SimpleSchedulingDate eq = null;
                        eq = (from w in WeeklyCalendar
                              where w.DayOfWeek == ci.DayOfWeek &&
                              w.EndDate == ci.EndDate &&
                              w.StartDate == ci.StartDate
                              select w).FirstOrDefault();
                    
                    bEqual = bEqual && (eq != null);
                });
            }
            if(bEqual)
            {
                Parallel.ForEach(tmpC, ci =>
                {
                    SimpleSchedulingDate equal = null;
                        equal = (from c in Calendar
                                 where 
                                  c.EndDate == ci.EndDate &&
                                  c.StartDate == ci.StartDate &&
                                  (Type == ScheduleType.weeklyPlan ? true :
                                    (
                                        c.Month == ci.Month &&
                                        c.DayOfWeek == ci.DayOfWeek &&
                                        c.Type == ci.Type &&
                                        c.WeekOfMonth == ci.WeekOfMonth &&
                                        c.DayOfMonth == ci.DayOfMonth
                                    )
                                  )
                                 select c).FirstOrDefault();
                    
                    bEqual = bEqual && (equal != null);
                });
            }
            if (bEqual)
            {
                Parallel.ForEach(tmpEC, ci =>
                {
                    SimpleSchedulingDate equal = null;
                    equal = (from c in ExceptionsCalendar
                             where 
                              c.EndDate == ci.EndDate &&
                              c.StartDate == ci.StartDate &&
                              (Type == ScheduleType.weeklyPlan ? true :
                                (
                                    c.Month == ci.Month &&
                                    c.DayOfWeek == ci.DayOfWeek &&
                                    c.Type == ci.Type &&
                                    c.WeekOfMonth == ci.WeekOfMonth &&
                                    c.DayOfMonth == ci.DayOfMonth
                                )
                              )
                              select c).FirstOrDefault();

                    bEqual = bEqual && (equal != null);
                });
            }
            
            return bEqual;
        }

        public bool Modified(MSScheduledAction sa, bool bRunningOnServer = false)
        {
            if (sa == null)
                return false;
            bool bEqual = sa.Date == Date && sa.DateOff == DateOff && sa.Time == Time && sa.TimeOff == TimeOff;

            List<WeeklyCalendarItem> tmpW = new List<WeeklyCalendarItem>();
            List<CalendarItem> tmpC = new List<CalendarItem>();
            List<ExceptionsCalendarItem> tmpEC = new List<ExceptionsCalendarItem>();
            tmpC.AddRange(sa.Calendar);
            tmpW.AddRange(sa.WeeklyCalendar);
            tmpEC.AddRange(sa.ExceptionsCalendar);

            if (Type == ScheduleType.weeklyPlan && Calendar.Count != WeeklyCalendar.Count && bRunningOnServer)
            {
                foreach (var c in WeeklyCalendar)
                {
                    Calendar.Add(new SimpleSchedulingDate()
                    {
                        DayOfWeek = c.DayOfWeek,
                        EndDate = c.EndDate,
                        Month = c.Month,
                        StartDate = c.StartDate,
                        Type = c.Type,
                        WeekOfMonth = c.WeekOfMonth,
                        DayOfMonth = c.DayOfMonth
                    });
                }
            }

            if (Type != ScheduleType.weeklyPlan)
                bEqual = bEqual && WeeklyCalendar.Count == tmpW.Count && Calendar.Count == tmpC.Count && ExceptionsCalendar.Count == tmpEC.Count;
            else
            {
                if (bRunningOnServer)
                    bEqual = bEqual && Calendar.Count == tmpC.Count && ExceptionsCalendar.Count == tmpEC.Count;
                else
                    bEqual = bEqual && WeeklyCalendar.Count == tmpW.Count && ExceptionsCalendar.Count == tmpEC.Count;
            }

            if (bEqual && !(Type == ScheduleType.weeklyPlan && bRunningOnServer))
            {
                Parallel.ForEach(tmpW, ci =>
                {
                    SimpleSchedulingDate eq = null;
                        eq = (from w in WeeklyCalendar
                              where w.EndDate == ci.EndDate &&
                              w.StartDate == ci.StartDate
                              select w).FirstOrDefault();
                    bEqual = bEqual && (eq != null);
                });
            }

            if (bEqual && (Type != ScheduleType.weeklyPlan || bRunningOnServer))
            {
                Parallel.ForEach(tmpC, ci =>
                {
                    SimpleSchedulingDate equal = null;
                    equal = (from c in Calendar
                                where c.DayOfWeek == ci.DayOfWeek &&
                                c.EndDate == ci.EndDate &&
                                c.StartDate == ci.StartDate &&
                                (Type == ScheduleType.weeklyPlan ? true : 
                                    (
                                        c.Month == ci.Month &&
                                        c.Type == ci.Type &&
                                        c.WeekOfMonth == ci.WeekOfMonth &&
                                        c.DayOfMonth == ci.DayOfMonth
                                    )
                                )
                                select c).FirstOrDefault();
                    bEqual = bEqual && (equal != null);
                });
            }

            if (bEqual)
            {
                Parallel.ForEach(tmpEC, ci =>
                {
                    SimpleSchedulingDate equal = null;
                    equal = (from c in ExceptionsCalendar
                             where 
                             c.EndDate == ci.EndDate &&
                             c.StartDate == ci.StartDate &&
                             (Type == ScheduleType.weeklyPlan ? true :
                                (
                                    c.Month == ci.Month &&
                                    c.DayOfWeek == ci.DayOfWeek &&
                                    c.Type == ci.Type &&
                                    c.WeekOfMonth == ci.WeekOfMonth &&
                                    c.DayOfMonth == ci.DayOfMonth
                                )
                             )
                             select c).FirstOrDefault();
                    bEqual = bEqual && (equal != null);
                });
            }
            return !bEqual;
        }
        #endregion

    }
}

