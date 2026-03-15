using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WinWrap.Basic;
using Opc.Ua;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Windows.Forms;
using Opc.Ua.Server;
using log4net;

namespace UFUAServerBase
{
    class ScriptManagerSeparateThread : ScriptManager
    {
        Thread thread;

        public ScriptManagerSeparateThread(String name, BaseInstanceState i, StandardServer s, ServerSystemContext sc, int[] breakpoints,
            Dictionary<String, BaseInstanceState> pn, CustomNodeManager2 nm) : base(name, i, s, sc, breakpoints, pn, nm)
        { }

        internal override void InitScriptEngine(String code)
        {
            if (thread != null)
                return;

            Code = code;
            bDisposing = false;
            bQuitRequest = false;
            bool bOnError = false;
            thread = new Thread(ob =>
            {
                if (Breakpoints != null && Breakpoints.Length > 0)
                    basicCtl = new BasicIdeObj();
                else
                    basicCtl = new BasicNoUIObj();
                InitBasicControl(this);
                try
                {
                    int counter = 0;
                    while (!bDisposing && !bQuitRequest && !bOnError)
                    {
                        if (++counter >= 5)
                        {
                            counter = 0;
                            //System.Diagnostics.Debug.WriteLine("Server Script Entering DoEvents");
                            try
                            {
                                manager.OnDoEvents(EventArgs.Empty);
                            }
                            catch (TerminatedException)
                            {
                                // script execution has been terminated
                            }
                            catch (Exception ex)
                            {
                                basicCtl.ReportError(ex);
                            }
                            //System.Diagnostics.Debug.WriteLine("Server Script Exiting DoEvents");
                        }

                        //System.Diagnostics.Debug.WriteLine("Server Script Entering Application.DoEvent");
                        Util.DoEvents();
                        //basicCtl.RunThis("DoEvents");
                        // Application.DoEvents();
                        //System.Diagnostics.Debug.WriteLine("Server Script Exiting Application.Doevents");
                        if (queue.Count > 0)
                        {
                            //System.Diagnostics.Debug.WriteLine("Server Script Entering Action");
                            var action = queue.Take();
                            action();
                            //System.Diagnostics.Debug.WriteLine("Server Script Exiting Action");
                        }

                        Thread.Sleep(100);
                    }

                    TerminateBasicControl(this, basicCtl);
                }
                catch (Exception ex)
                {
                    syslog.Error(ex.Message);
                }
                module = null;
            })
            {
                IsBackground = true
            };
            thread.Name = String.Format("Script Executer {0}", Name);
            thread.SetApartmentState(ApartmentState.STA);
        }

        internal override void Start()
        {
            if (thread == null)
                return;
            thread.Start();
        }
    }
}
