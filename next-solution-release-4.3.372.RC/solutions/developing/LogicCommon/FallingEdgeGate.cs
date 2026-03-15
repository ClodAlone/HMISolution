using DocumentManager.ComponentService;
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
    public class FallingEdgeGate : GateData
    {
        #region Abstracts

        bool edgeDetected;
        public override void Init(IDocument doc)
        {
            base.Init(doc);

            edgeDetected = false;
        }
        public override void Terminate()
        {
            edgeDetected = false;
        }

        public override bool Execute(WireData[] links)
        {
            if (links == null || links.Length == 0)
                return false;

            if (!links[0].Value)
            {
                if (!edgeDetected)
                {
                    edgeDetected = true;
                    return true;
                }
            }
            else
                edgeDetected = false;

            return false;
        }

        public override String Name
        {
            get
            {
#if WINDOWS_UWP
                return "FallingEdge";
#else
                return Properties.Settings.Default.FallingEdgeGateName;
#endif
            }
        }

#if !WINDOWS_UWP && !NET_STANDARD
        public override DataTemplateDictionary GetDataTemplates()
        {
            var resourceDictionary = new ResourceDictionary();
            resourceDictionary.Source = new Uri("/LogicCommon;component/ResourceDictionary/EdgeLibrary.xaml", UriKind.Relative);
            return resourceDictionary[NodeTemplateDictionary] as DataTemplateDictionary;
        }
        public override List<GateData> GetTypes()
        {
            var ret = base.GetTypes();
            ret.Add(new FallingEdgeGate() { Key = "FallingEdge", Text = "Falling Edge", GateDataTemplate = "FallingEdge", Category = "FallingEdge" });
            return ret;
        }

        public override String GetCategory() { return Properties.Resources.PortCategory; }
#endif
        #endregion
    }
}
