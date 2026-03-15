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
    public class TimerDelayGate : GateData
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
            {
                ResetTimer();
                return false;
            }
            if (!links[0].Value)
            {
                ResetTimer();
                return false;
            }
            var ret = false;
            if (StartedTimer == null || StartedTimer == DateTime.MinValue)
                StartedTimer = DateTime.UtcNow;
            var diff = DateTime.UtcNow - StartedTimer;
            if (diff >= Timer)
            {
                ret = true;
                Text = String.Format("{0:0}", Timer.TotalMilliseconds);
            }
            else
                Text = String.Format("{0:0}", diff.TotalMilliseconds);

            return ret;
        }
        public override String Name
        {
            get
            {
#if WINDOWS_UWP && !NET_STANDARD
                return "TimerDelay";
#else
                return Properties.Settings.Default.TimerDelayGateName;
#endif
            }
        }
#endregion

#if !WINDOWS_UWP && !NET_STANDARD
        public override DataTemplateDictionary GetDataTemplates()
        {
            var resourceDictionary = new ResourceDictionary();
            resourceDictionary.Source = new Uri("/LogicCommon;component/ResourceDictionary/TimerDelayLibrary.xaml", UriKind.Relative);
            return resourceDictionary[NodeTemplateDictionary] as DataTemplateDictionary;
        }
        public override List<GateData> GetTypes()
        {
            var ret = base.GetTypes();
            ret.Add(new TimerDelayGate() { Key = "TimerDelay", Text = "Timer Delay", GateDataTemplate = "TimerDelay", Category = "TimerDelay" });
            return ret;
        }

        public override String GetCategory() { return Properties.Resources.TimerCategory; }
#endif
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
