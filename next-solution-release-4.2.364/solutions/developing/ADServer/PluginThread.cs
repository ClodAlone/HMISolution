using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ADPluginInterfaces;
using System.Threading;
using ADPluginBase;
using OPCUAViewModel;
using ViewModelLib;

namespace ADServer
{
    public class PluginThread : IDisposable  
    {
        public void Dispose()
        {
            StopThread();

            lock (lockListObj)
            {
                MessagesList.Clear();
            }

            lock (lockThreadObject)
            {
                if (StopWorkerThread != null)
                    StopWorkerThread.Dispose();
                if(PluginRunEvent != null)
                    PluginRunEvent.Dispose();
            }

            GC.SuppressFinalize(this);
        }

        ADUAServer currServer;
        private Thread WorkerThread;
        protected ManualResetEvent StopWorkerThread;
        protected readonly object lockThreadObject = new object();

        private object lockListObj = new object();
        private List<Message> _MessagesList = new List<Message>();
        public List<Message> MessagesList
        {
            get { return _MessagesList; }
        }

        private List<Message> _GroupMessagesList = new List<Message>();
        public List<Message> GroupMessagesList
        {
            get { return _GroupMessagesList; }
        }

        private List<Message> _WaitingMessagesList = new List<Message>();
        public List<Message> WaitingMessagesList
        {
            get { return _WaitingMessagesList; }
        }

        public void StopThread()
        {
            Thread thread = WorkerThread;
            lock (lockThreadObject)
            {
                if(PluginRunEvent != null)
                    PluginRunEvent.Set();
                if (StopWorkerThread != null)
                    StopWorkerThread.Set();
            }
            if (thread != null)
                thread.Join();
            if (pluginRef != null)
                pluginRef.StopPlugin();
        }

        public virtual bool Startup(ADUAServer server)
        {
            lock (lockThreadObject)
            {
                currServer = server;
                pluginRef.SendResultEvent += PluginThread_SendResultEvent;
                if (WorkerThread == null)
                    WorkerThread = new Thread(WorkingThread);

                if (StopWorkerThread == null)
                    StopWorkerThread = new ManualResetEvent(false);
                else
                    StopWorkerThread.Reset();

                if (PluginRunEvent == null)
                    PluginRunEvent = new ManualResetEvent(false);
                else
                    PluginRunEvent.Reset();

                if (!WorkerThread.IsAlive)
                    WorkerThread.Start(server);

                return WorkerThread != null && (WorkerThread.ThreadState & (ThreadState.Stopped | ThreadState.Unstarted)) == 0;
            }
        }

        protected ManualResetEvent PluginRunEvent;
        int newWorkTime = Timeout.Infinite;
        int NextWorkTime = Timeout.Infinite;

        protected virtual void WorkingThread(object data)
        {
            ADUAServer server = data as ADUAServer;
            if (server == null)
                return;
            List<Message> resultList = new List<Message>();
            List<Message> pendingList = new List<Message>();
            Message pendingmessage = null;
            int sleepCycle = 10;
            int result;

            while (true)
            {
                if (newWorkTime != Timeout.Infinite)
                {
                    //update the NextWorkTime
                    NextWorkTime = newWorkTime;
                    newWorkTime = Timeout.Infinite;
                }

                if (PluginRunEvent != null)
                {
                    PluginRunEvent.WaitOne(NextWorkTime);
                    PluginRunEvent.Reset();
                }

                lock (lockListObj)
                {

                    pendingmessage = null;
                    if (MessagesList.Count > 0)
                    {
                        int nextDelay = (int)(MessagesList[0].TimeStamp - DateTime.UtcNow).TotalMilliseconds;
                        if (nextDelay > 0)
                        {
                            NextWorkTime = nextDelay;
                        }
                        else
                        {
                            pendingmessage = MessagesList[0];
                            MessagesList.RemoveAt(0);
                        }
                    }
                        
                }

                if (pendingmessage != null)
                {
                    if (!string.IsNullOrEmpty(pendingmessage.GroupId) && pluginRef.SendMultiple())
                    {
                        //several messages
                        pendingList.Clear();
                        pendingList.Add(pendingmessage);
                        lock (lockListObj)
                        {
                            var list = (from m in MessagesList where m.GroupId == pendingmessage.GroupId select m).ToList();
                            if (list.Count > 0)
                                pendingList.AddRange(list);
                            foreach (var m in list)
                            {
                                if(m.UserId != Guid.Empty && m.ErrorCount > 0)
                                    server.UpdateData(m);
                                MessagesList.Remove(m);
                            }
                        }
                        result = pluginRef.OnSendMessage(pendingList);
                        pendingmessage = null;
                        /*
                         * results:
                         * 0 = NoError, message sent with success
                         * negative = error of some sort, retry until reach the try limit
                         * 11 = alarm acknowledged, if possible, send Ack to the I/O Server (onli for area alarms)
                         */
                    }
                    else
                    {
                        if (pendingmessage.UserId != Guid.Empty && pendingmessage.ErrorCount > 0)
                            server.UpdateData(pendingmessage);
                        result = pluginRef.OnSendMessage(pendingmessage);
                    }

                    resultList.Clear();
                    if (pendingmessage != null)
                    {
                        resultList.Add(pendingmessage);
                    }
                    else if (pendingList != null)
                    {
                        resultList.AddRange(pendingList);
                    }

                    if (result < 0)
                    {
                        //error
                        foreach(var pmsg in resultList)
                        {
                            if (++pmsg.ErrorCount > ErrorThreshold)
                            {
                                //delete with error
                                server.addMessage(pmsg, true);
                            }
                            else
                            {
                                //put in list, increment timestamp(, update persistence?)
                                pmsg.TimeStamp = DateTime.UtcNow + TimeSpan.FromSeconds(ErrorDelay);
                                lock (lockListObj)
                                {
                                    MessagesList.Add(pmsg);
                                }
                                server.addMessage(pmsg);
                            }
                        }
                        lock(lockListObj)
                        {
                            if (MessagesList.Count > 2)
                                MessagesList.Sort(CompareMessagesByTimeStamp);
                        }
                        
                    }
                    else if (result == PluginBase.OkOnCharge)
                    {
                        lock (lockListObj)
                        {
                            //message in charge to the plugin, will return the result of send asynchronously
                            WaitingMessagesList.AddRange(resultList);
                        }
                    }
                    else
                    {
                        //delete from persistence
                        foreach (var pmsg in resultList)
                            server.addMessage(pmsg, true);
                    }
                }
                if (StopWorkerThread.WaitOne(sleepCycle, false))
                    break;
                
                NextWorkTime = Timeout.Infinite;
                if (MessagesList.Count > 0)
                {
                    lock(lockListObj)
                    {
                        TestUpdateNextWorkTime(false);
                    }
                }
            }

            System.Diagnostics.Trace.TraceInformation("Terminating AD Working thread!");
        }

        private static int CompareMessagesByTimeStamp(Message x, Message y)
        {
            if (x == null || x.TimeStamp == null)
            {
                if (y == null || y.TimeStamp == null)
                    return 0;
                else
                    return -1;
            }
            else
            {
                if (y == null || y.TimeStamp == null)
                    return 1;
                else
                    return (x.TimeStamp < y.TimeStamp ? -1 : (x.TimeStamp == y.TimeStamp ? 0 : 1));
            }
            

        }

        public void PostMessageGroupComplete()
        {
            lock (lockListObj)
            {
                if(GroupMessagesList.Count > 0)
                {
                    MessagesList.AddRange(GroupMessagesList);
                    if (MessagesList.Count > 2)
                        MessagesList.Sort(CompareMessagesByTimeStamp);
                    GroupMessagesList.Clear();
                    TestUpdateNextWorkTime();
                }
            }
        }
        public int PostMessage(Message m)
        {
            if (pluginRef == null || WorkerThread == null)
                return -1;

            lock(lockListObj)
            {
                if(string.IsNullOrEmpty(m.GroupId))
                {
                    MessagesList.Add(m);
                    if (MessagesList.Count > 2)
                        MessagesList.Sort(CompareMessagesByTimeStamp);
                    TestUpdateNextWorkTime();
                }
                else
                {
                    GroupMessagesList.Add(m);
                }
                
            }

            return 0;
        }

        protected void PluginThread_SendResultEvent(object sender, SendResultEventArgs e)
        {
            if (currServer == null)
                return;

            Message msg = null;
            lock (lockListObj)
            {
                int idx = WaitingMessagesList.FindIndex(o => { return o.NodeId == e.msgNodeId; });
                if (idx == -1)
                    return;
                msg = WaitingMessagesList[idx];
                WaitingMessagesList.RemoveAt(idx);
            }

            if (e.msgResult == PluginBase.OkSendResult)
            {
                //delete from persistence
                currServer.addMessage(msg, true);
            }
            else if(e.msgResult < PluginBase.OkSendResult)
            {
                //error
                if (++msg.ErrorCount > ErrorThreshold)
                {
                    //delete with error
                    currServer.addMessage(msg, true);
                }
                else
                {
                    //put in list, increment timestamp(, update persistence?)
                    msg.TimeStamp = DateTime.UtcNow + TimeSpan.FromSeconds(ErrorDelay);
                    lock (lockListObj)
                    {
                        MessagesList.Add(msg);
                        TestUpdateNextWorkTime();
                    }
                    currServer.addMessage(msg);
                }
            }
            else if(e.msgResult == PluginBase.OkAckServer)
            {
                if (!string.IsNullOrEmpty(msg.ServerAlarmStringId))
                    currServer.AcknowledgeAlarm(msg.ServerAlarmStringId);
                //delete from persistence
                currServer.addMessage(msg, true);
            }
        }


        void TestUpdateNextWorkTime(bool update = true)
        {
            int nextDelay = (int)(MessagesList[0].TimeStamp - DateTime.UtcNow).TotalMilliseconds;
            if (nextDelay > 0)
            {
                newWorkTime = nextDelay;
            }
                
            if (PluginRunEvent != null && (update || nextDelay <= 0) )
                PluginRunEvent.Set();
        }
        private IPlugin _PluginRef = null;
       public IPlugin pluginRef
       {
       	get	{ return _PluginRef; }
       	set
       	{
       		_PluginRef = value;
       	}
       }

       public int ErrorThreshold { get; set; }
       public uint ErrorDelay { get; set; }
       
    }
}
