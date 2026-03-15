using DocumentManager.ComponentService;
using LogicCore;
using Northwoods.GoXam;
using Northwoods.GoXam.Model;
using OPCUAViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using Utilities;

namespace LogicCommon
{
    public class TimerLatchGate : GateData
    {
        String oldText;

        #region Methods

        void ResetTimer()
        {
            StartedTimer = DateTime.MinValue;
            Text = oldText;
        }

        #endregion

        #region Abstracts

        public override void Init(IDocument doc)
        {
            base.Init(doc);
            oldText = Text;
        }
        public override void Terminate()
        {
            ResetTimer();
        }

        public override bool Execute(WireData[] links)
        {
            if (links == null || links.Length == 0)
                return false;
            if (!links[0].Value)
            {
                ResetTimer();
                return false;
            }
            var ret = false;

            if (links[0].Value)
            {
                if (StartedTimer == null || StartedTimer == DateTime.MinValue)
                    StartedTimer = DateTime.UtcNow;
            }

            if (StartedTimer != null && StartedTimer != DateTime.MinValue)
            {
                var diff = DateTime.UtcNow - StartedTimer;
                if (diff >= Timer)
                {
                    ret = false;
                    Text = String.Format("{0:0}", Timer.TotalMilliseconds);

                    if (!links[0].Value)
                        StartedTimer = DateTime.MinValue;
                }
                else
                {
                    ret = true;
                    Text = String.Format("{0:0}", diff.TotalMilliseconds);
                }
            }

            return ret;
        }

        public override String Name
        {
            get
            {
#if WINDOWS_UWP
                return "TimerLatch";
#else
                return Properties.Settings.Default.TimerLatchGateName;
#endif
            }
        }
#if !WINDOWS_UWP && !NET_STANDARD
        public override DataTemplateDictionary GetDataTemplates()
        {
            var resourceDictionary = new ResourceDictionary();
            resourceDictionary.Source = new Uri("/LogicCommon;component/ResourceDictionary/TimerLatchLibrary.xaml", UriKind.Relative);
            return resourceDictionary[NodeTemplateDictionary] as DataTemplateDictionary;
        }
        public override List<GateData> GetTypes()
        {
            var ret = base.GetTypes();
            ret.Add(new TimerLatchGate() { Key = "TimerLatch", Text = "Timer Latch", GateDataTemplate = "TimerLatch", Category = "TimerLatch" });
            return ret;
        }

        public override String GetCategory() { return Properties.Resources.TimerCategory; }
#endif
#endregion

#region Properties

        DateTime StartedTimer { get; set; }

        public TimeSpan Timer
        {
            get { return _Timer; }
            set
            {
                var old = _Timer;
                if (old != value)
                {
                    _Timer = value;
                    RaisePropertyChanged("Timer", old, value);
                }
            }
        }
        private TimeSpan _Timer;

#endregion

#region Overrides
// support standard reading/writing via Linq for XML
#if !WINDOWS_UWP && !NET_STANDARD
        public override XElement MakeXElement(XName n)
        {
            XElement e = base.MakeXElement(n);
            e.Add(XHelper.Attribute("Timer", this.Timer, new TimeSpan()));
            return e;
        }
#endif
        public override void LoadFromXElement(XElement e)
        {
            base.LoadFromXElement(e);
            this.Timer = XHelper.Read("Timer", e, new TimeSpan());
        }
#endregion
    }
}
