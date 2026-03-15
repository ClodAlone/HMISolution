using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using OPCUAViewModel;
using UFInterfaces;
using Utilities;
using ViewModelLib;
using UFWebClientResponsive_HTML5;
using UFWebClient.HTML5;

// namespace UFWebClient.HTML5
// {
public class AlarmProvider
    {
        static HttpSessionState Session
        {
            get { return HttpContext.Current.Session; }
        }

        readonly static List<AlarmSink.AlarmSink> recycledSession = new List<AlarmSink.AlarmSink>();

        readonly static String cookie = "Alarm";
        public static SafeObservableCollection<ConditionStateViewModel> GetAlarms()
        {
            EnureSession();
            if (Session[cookie] == null)
                Session[cookie] = CreateAlarmSink();
            var sink = (AlarmSink.AlarmSink)Session[cookie];
            if (sink == null)
                return null;
            return sink.ConditionList;
        }

        public static void SetClientTimeOffset(int clientoffset)
        {
            EnureSession();
            if (Session[cookie] == null)
                Session[cookie] = CreateAlarmSink();
            var sink = (AlarmSink.AlarmSink)Session[cookie];
            if (sink == null)
                return;
            sink.SessionOffset = clientoffset;
        }

        public static int GetClientTimeOffset()
        {
            EnureSession();
            var sink = (AlarmSink.AlarmSink)Session[cookie];
            if (sink == null)
                return 0;
            return sink.SessionOffset;
        }

        public static bool IsConnected()
        {
            EnureSession();
            var sink = (AlarmSink.AlarmSink)Session[cookie];
            if (sink == null)
                return false;
            return sink.IsConnected;
        }

        public static void AckAll()
        {
            EnureSession();
            var sink = (AlarmSink.AlarmSink)Session[cookie];
            if (sink == null)
                return;
            sink.AckAll();
        }

        public static void AckSelected(String[] list)
        {
            EnureSession();
            var sink = (AlarmSink.AlarmSink)Session[cookie];
            if (sink == null)
                return;
            sink.AckSelected(list);
        }

        public static void ConfirmAll()
        {
            EnureSession();
            var sink = (AlarmSink.AlarmSink)Session[cookie];
            if (sink == null)
                return;
            sink.ConfirmAll();
        }

        public static void ConfirmSelected(String[] list)
        {
            EnureSession();
            var sink = (AlarmSink.AlarmSink)Session[cookie];
            if (sink == null)
                return;
            sink.ConfirmSelected(list);
        }

        public static void Refresh()
        {
            EnureSession();
            var sink = (AlarmSink.AlarmSink)Session[cookie];
            if (sink == null)
                return;
            sink.Refresh();
        }

        static void EnureSession()
        {
            var sink = (AlarmSink.AlarmSink)Session[cookie];
            if (sink == null)
                return;
            lock (recycledSession)
            {
                if (recycledSession.Contains(sink))
                {
                    recycledSession.Remove(sink);
                    Session[cookie] = null;
                }
            }
        }

        public static AlarmSink.AlarmSink CreateAlarmSink()
        {
            if (Global.projectDocument == null || Global.UFUAEditorComponent == null)
                return null;

            var server = Global.UFUAEditorComponent.GetServerEntityReference(Global.projectDocument);
            if (server == null)
                return null;
            var ret = new AlarmSink.AlarmSink(server, String.Format("{0}-{1}", Global.ClientSessionName, Guid.NewGuid()));
            ret.Recycling += (o, e) =>
                {
                    lock (recycledSession)
                    {
                        if (!recycledSession.Contains(ret))
                            recycledSession.Add(ret);
                    }
                };

            return ret;
        }
    }
// }