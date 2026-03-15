using System;
using System.Collections.Generic;
using System.Linq;

namespace MSEditor.Controls
{
    public class MyData : DevExpress.XtraScheduler.Appointment
    {

        #region Ctor
        public MyData()
        {

        }
        public MyData(DevExpress.XtraScheduler.AppointmentType type)
            : base(type)
        {

        }
        public MyData(DateTime start, TimeSpan duration, string subject)
            : base(start, duration, subject)
        {

        }
        public MyData(DevExpress.XtraScheduler.AppointmentType type, DateTime start, TimeSpan duration, string subject)
            : base(type, start, duration, subject)
        {

        }
        public MyData(DevExpress.XtraScheduler.AppointmentType type, DateTime start, TimeSpan duration, string subject, object id)
            : base(type, start, duration, subject, id)
        {

        }
        public MyData(DevExpress.XtraScheduler.AppointmentType type, DateTime start, DateTime end, string subject)
            : base(type, start, end, subject)
        {

        }
        public MyData(DevExpress.XtraScheduler.AppointmentType type, DateTime start, TimeSpan duration)
            : base(type, start, duration)
        {

        }
        public MyData(DevExpress.XtraScheduler.AppointmentType type, DateTime start, DateTime end)
            : base(type, start, end)
        {

        }
        public MyData(DateTime start, DateTime end, string subject)
            : base(start, end, subject)
        {

        }
        public MyData(DateTime start, DateTime end)
            : base(start, end)
        {

        }
        public MyData(DateTime start, TimeSpan duration)
            : base(start, duration)
        {

        }
        #endregion

        private string _Description;//Caption
        public string Description
        {
            get { return _Description; }
            set
            {
                _Description = value;
            }
        }

    }
}
