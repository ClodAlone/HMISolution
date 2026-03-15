using LogicCore;
using Northwoods.GoXam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LogicCommon
{
    public class XorGate : GateData
    {
        #region Abstracts
        public override bool Execute(WireData[] links)
        {
            if (links == null || links.Length == 0)
                return false;

            var ret = links[0].Value;
            for(int i = 1; i < links.Length; ++i)
                ret ^= links[i].Value;
            return ret;
        }

        public override String Name
        {
            get
            {
#if WINDOWS_UWP
                return "Xor";
#else
                return Properties.Settings.Default.XorName;
#endif
            }
        }
#if !WINDOWS_UWP && !NET_STANDARD
        public override DataTemplateDictionary GetDataTemplates()
        {
            var resourceDictionary = new ResourceDictionary();
            resourceDictionary.Source = new Uri("/LogicCommon;component/ResourceDictionary/XorLibrary.xaml", UriKind.Relative);
            return resourceDictionary[NodeTemplateDictionary] as DataTemplateDictionary;
        }
        public override List<GateData> GetTypes()
        {
            var ret = base.GetTypes();
            ret.Add(new XorGate() { Key = "Xor", Text = "Xor", GateDataTemplate = "Xor", Category = "Xor" });
            ret.Add(new XorGate() { Key = "Xor", Text = "Xor", GateDataTemplate = "XorFour", Category = "XorFour" });
            return ret;
        }

        public override String GetCategory() { return Properties.Resources.PortCategory; }
#endif
#endregion
    }
}
