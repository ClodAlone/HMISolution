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
    public class NotGate : GateData
    {
        #region Abstracts
        public override bool Execute(WireData[] links)
        {
            if (links == null || links.Length == 0)
                return false;

            return !links[0].Value;
        }

        public override String Name
        {
            get
            {
#if WINDOWS_UWP
                return "Not";
#else
                return Properties.Settings.Default.NotGateName;
#endif
            }
        }

#if !WINDOWS_UWP && !NET_STANDARD
        public override DataTemplateDictionary GetDataTemplates()
        {
            var resourceDictionary = new ResourceDictionary();
            resourceDictionary.Source = new Uri("/LogicCommon;component/ResourceDictionary/NotLibrary.xaml", UriKind.Relative);
            return resourceDictionary[NodeTemplateDictionary] as DataTemplateDictionary;
        }
        public override List<GateData> GetTypes()
        {
            var ret = base.GetTypes();
            ret.Add(new NotGate() { Key = "Not", Text = "Not", GateDataTemplate = "Not", Category = "Not" });
            return ret;
        }

        public override String GetCategory() { return Properties.Resources.PortCategory; }
#endif
        #endregion
    }
}
