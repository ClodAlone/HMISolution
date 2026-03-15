using LogicCore;
using Northwoods.GoXam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LogicPortSample
{
    public class SampleAndGate : GateData
    {
        #region Abstracts
        public override bool Execute(WireData[] links)
        {
            if (links == null || links.Length == 0)
                return false;

            var ret = links[0].Value;
            foreach (var data in links)
                ret &= data.Value;
            return ret;
        }

        public override String Name
        {
            get
            {
#if WINDOWS_UWP
                return "SampleAnd";
#else
                return Properties.Settings.Default.SampleAndGateName;
#endif
            }
        }

#if !WINDOWS_UWP && !NET_STANDARD
        public override DataTemplateDictionary GetDataTemplates()
        {
            var resourceDictionary = new ResourceDictionary();
            resourceDictionary.Source = new Uri("/LogicPortSample;component/ResourceDictionary/SampleAndLibrary.xaml", UriKind.Relative);
            return resourceDictionary[NodeTemplateDictionary] as DataTemplateDictionary;
        }
        public override List<GateData> GetTypes()
        {
            var ret = base.GetTypes();
            ret.Add(new SampleAndGate() { Key = "SampleAnd", Text = "SampleAnd", GateDataTemplate = "SampleAnd", Category = "SampleAnd" });
            return ret;
        }

        public override String GetCategory() { return Properties.Resources.PortCategory; }
#endif
        #endregion
    }
}
