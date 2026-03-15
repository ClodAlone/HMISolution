using DocumentManager.ComponentService;
using LogicCore;
using Northwoods.GoXam;
using Northwoods.GoXam.Model;
using Opc.Ua;
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
    public class PIDGate : GateData
#if !WINDOWS_UWP && !NET_STANDARD
        , ICloneable
#endif
    {
#if !WINDOWS_UWP && !NET_STANDARD
#region ICloneable Members

        public override object Clone()
        {
            var ret = base.Clone() as PIDGate;
            if (ProcessTag != null)
            {
                var tagxml = ProcessTag.ToXml();
                if (!String.IsNullOrEmpty(tagxml))
                    ret.ProcessTag = tagxml.FromXml<OPCUAEntityReference>();
            }

            if (ControlTag != null)
            {
                var tagxml = ControlTag.ToXml();
                if (!String.IsNullOrEmpty(tagxml))
                    ret.ControlTag = tagxml.FromXml<OPCUAEntityReference>();
            }

            if (SetPointTag != null)
            {
                var tagxml = SetPointTag.ToXml();
                if (!String.IsNullOrEmpty(tagxml))
                    ret.SetPointTag = tagxml.FromXml<OPCUAEntityReference>();
            }

            return ret;
        }

#endregion
#endif

        bool bProcessEnabled;
#region Abstracts
        public override bool Execute(WireData[] links)
        {
            if (links == null || links.Length == 0 || pidController == null)
                return false;

            bProcessEnabled = links[0].Value;
            return bProcessEnabled;
        }

        public override bool ReadData()
        {
            bool ret = base.ReadData();
            if (_runtimeProcessTag != null && _runtimeProcessTag.MonitoredItemViewModel != null &&
                _runtimeProcessTag.MonitoredItemViewModel.DataValue != null)
            {
                if (LogicCore.Helpers.StatusCodeHelper.IsGoodOrUncertainLastUsable(_runtimeProcessTag.MonitoredItemViewModel.DataValue.StatusCode))
                {
                    try
                    {
                        pidController.ProcessVariable = (float)Convert.ToDouble(_runtimeProcessTag.MonitoredItemViewModel.DataValue.Value);
                        ret = true;
                    }
                    catch
                    {

                    }
                }
                else
                    return false;
            }

            if (_runtimeSetPointTag != null && _runtimeSetPointTag.MonitoredItemViewModel != null &&
                _runtimeSetPointTag.MonitoredItemViewModel.DataValue != null)
            {
                if (LogicCore.Helpers.StatusCodeHelper.IsGoodOrUncertainLastUsable(_runtimeProcessTag.MonitoredItemViewModel.DataValue.StatusCode))
                {
                    try
                    {
                        pidController.SetPoint = (float)Convert.ToDouble(_runtimeSetPointTag.MonitoredItemViewModel.DataValue.Value);
                        ret = true;
                    }
                    catch
                    {

                    }
                }
                else
                    return false;
            }

            return ret;
        }

        public override GateTypes GateType { get { return GateTypes.Output; } }

        public override void WriteData()
        {
            base.WriteData();
            if (bProcessEnabled && _runtimeControlTag != null && _runtimeControlTag.MonitoredItemViewModel != null &&
                _runtimeControlTag.MonitoredItemViewModel.DataValue != null)
            {
                try
                {
                    var controlVar = pidController.ControlVariable;
                    var valueCompare = (float)Convert.ToDouble(_runtimeControlTag.MonitoredItemViewModel.DataValue.Value);
                    if (valueCompare != controlVar)
                        _runtimeControlTag.MonitoredItemViewModel.WriteValue(controlVar);
                }
                catch (Exception ex)
                {
                    throw new Exception(String.Format(Properties.Resources.WriteVariableException, _runtimeControlTag.HumanReadable));
                }
            }
        }

        PIDController.PidController pidController;
        public override void Init(IDocument doc)
        {
            base.Init(doc);
            pidController = new PIDController.PidController(GainProportional, GainIntegral, GainDerivative, OutputMax, OutputMin);
        }

        public override void Terminate()
        {
            _runtimeProcessTag = null;
            _runtimeControlTag = null;
            _runtimeSetPointTag = null;

            pidController = null;
        }

        public override List<OPCUAEntityReference> GetTagList()
        {
            var ret = base.GetTagList();
            if (_runtimeProcessTag != null)
                ret.Add(_runtimeProcessTag);
            else if (ProcessTag != null)
            {
                var tagxml = ProcessTag.ToXml();
                if (!String.IsNullOrEmpty(tagxml))
                    _runtimeProcessTag = tagxml.FromXml<OPCUAEntityReference>();

                if (_runtimeProcessTag != null)
                    ret.Add(_runtimeProcessTag);
            }

            if (_runtimeControlTag != null)
                ret.Add(_runtimeControlTag);
            else if (ControlTag != null)
            {
                var tagxml = ControlTag.ToXml();
                if (!String.IsNullOrEmpty(tagxml))
                    _runtimeControlTag = tagxml.FromXml<OPCUAEntityReference>();

                if (_runtimeControlTag != null)
                    ret.Add(_runtimeControlTag);
            }

            if (_runtimeSetPointTag != null)
                ret.Add(_runtimeSetPointTag);
            else if (SetPointTag != null)
            {
                var tagxml = SetPointTag.ToXml();
                if (!String.IsNullOrEmpty(tagxml))
                    _runtimeSetPointTag = tagxml.FromXml<OPCUAEntityReference>();

                if (_runtimeSetPointTag != null)
                    ret.Add(_runtimeSetPointTag);
            }

            return ret;
        }
#if !WINDOWS_UWP && !NET_STANDARD
        public override void UpdateTagList(List<OPCUAEntityReference> list)
        {
            if (list == null)
                return;
            if (list.Count >= 1)
            {
                if (ProcessTag != null)
                {
                    ProcessTag = list[0];
                    var tagxml = ProcessTag.ToXml();
                    if (!String.IsNullOrEmpty(tagxml))
                        _runtimeProcessTag = tagxml.FromXml<OPCUAEntityReference>();
                }
            }
            if (list.Count >= 2)
            {
                if (ControlTag != null)
                {
                    ControlTag = list[1];
                    var tagxml = ControlTag.ToXml();
                    if (!String.IsNullOrEmpty(tagxml))
                        _runtimeControlTag = tagxml.FromXml<OPCUAEntityReference>();
                }
            }
            if (list.Count >= 3)
            {
                if (SetPointTag != null)
                {
                    SetPointTag = list[2];
                    var tagxml = SetPointTag.ToXml();
                    if (!String.IsNullOrEmpty(tagxml))
                        _runtimeSetPointTag = tagxml.FromXml<OPCUAEntityReference>();
                }
            }
        }
#endif
        public override String Name
        {
            get
            {
#if WINDOWS_UWP && !NET_STANDARD
                return "PID";
#else
                return Properties.Settings.Default.PIDGateName;
#endif
            }
        }
#if !WINDOWS_UWP && !NET_STANDARD
        public override DataTemplateDictionary GetDataTemplates()
        {
            var resourceDictionary = new ResourceDictionary();
            resourceDictionary.Source = new Uri("/LogicCommon;component/ResourceDictionary/PIDLibrary.xaml", UriKind.Relative);
            return resourceDictionary[NodeTemplateDictionary] as DataTemplateDictionary;
        }
        public override List<GateData> GetTypes()
        {
            var ret = base.GetTypes();
            ret.Add(new PIDGate() { Key = "PID", Text = "PID", GateDataTemplate = "PID", Category = "PID" });
            return ret;
        }

        public override String GetCategory() { return Properties.Resources.PIDCategory; }
#endif
        #endregion

        #region Properties

        OPCUAEntityReference _runtimeProcessTag;
        OPCUAEntityReference _ProcessTag;
        public OPCUAEntityReference ProcessTag
        {
            get { return _ProcessTag; }
            set
            {
                var old = _ProcessTag;
                if (old != value)
                {
                    _ProcessTag = value;
                    RaisePropertyChanged("ProcessTag", old, value);
                }
            }
        }

        OPCUAEntityReference _runtimeControlTag;
        OPCUAEntityReference _ControlTag;
        public OPCUAEntityReference ControlTag
        {
            get { return _ControlTag; }
            set
            {
                var old = _ControlTag;
                if (old != value)
                {
                    _ControlTag = value;
                    RaisePropertyChanged("ControlTag", old, value);
                }
            }
        }

        OPCUAEntityReference _runtimeSetPointTag;
        OPCUAEntityReference _SetPointTag;
        public OPCUAEntityReference SetPointTag
        {
            get { return _SetPointTag; }
            set
            {
                var old = _SetPointTag;
                if (old != value)
                {
                    _SetPointTag = value;
                    RaisePropertyChanged("SetPointTag", old, value);
                }
            }
        }

        float _OutputMax = 100;
        public float OutputMax
        {
            get { return _OutputMax; }
            set
            {
                var old = _OutputMax;
                if (old != value)
                {
                    _OutputMax = value;
                    RaisePropertyChanged("OutputMax", old, value);
                }
            }
        }

        float _OutputMin = 0;
        public float OutputMin
        {
            get { return _OutputMin; }
            set
            {
                var old = _OutputMin;
                if (old != value)
                {
                    _OutputMin = value;
                    RaisePropertyChanged("OutputMin", old, value);
                }
            }
        }

        float _GainDerivative = 0.05f;
        public float GainDerivative
        {
            get { return _GainDerivative; }
            set
            {
                var old = _GainDerivative;
                if (old != value)
                {
                    _GainDerivative = value;
                    RaisePropertyChanged("GainDerivative", old, value);
                }
            }
        }

        float _GainIntegral = 0.008f;
        public float GainIntegral
        {
            get { return _GainIntegral; }
            set
            {
                var old = _GainIntegral;
                if (old != value)
                {
                    _GainIntegral = value;
                    RaisePropertyChanged("GainIntegral", old, value);
                }
            }
        }

        float _GainProportional = 0.1f;
        public float GainProportional
        {
            get { return _GainProportional; }
            set
            {
                var old = _GainProportional;
                if (old != value)
                {
                    _GainProportional = value;
                    RaisePropertyChanged("GainProportional", old, value);
                }
            }
        }

#endregion

#region Overrides

#if !WINDOWS_UWP && !NET_STANDARD
        public override bool CanDropTag() { return true; }

        public override bool DroppingTag(OPCUAEntityReference tag, String title)
        {
            ProcessTag = tag;
            Text = title;
            return true;
        }

        // support standard reading/writing via Linq for XML
        public override XElement MakeXElement(XName n)
        {
            XElement e = base.MakeXElement(n);
            e.Add(XHelper.Attribute("OutputMax", this.OutputMax, 100));
            e.Add(XHelper.Attribute("OutputMin", this.OutputMin, 0));
            e.Add(XHelper.Attribute("GainDerivative", this.GainDerivative, 0.05f));
            e.Add(XHelper.Attribute("GainIntegral", this.GainIntegral, 0.008f));
            e.Add(XHelper.Attribute("GainProportional", this.GainProportional, 0.1f));

            if (this.ProcessTag != null)
                e.Add(XHelper.Attribute("ProcessTag", this.ProcessTag.ToXml(), ""));
            if (this.ControlTag != null)
                e.Add(XHelper.Attribute("ControlTag", this.ControlTag.ToXml(), ""));
            if (this.SetPointTag != null)
                e.Add(XHelper.Attribute("SetPointTag", this.SetPointTag.ToXml(), ""));
            return e;
        }
#endif
        public override void LoadFromXElement(XElement e)
        {
            base.LoadFromXElement(e);
            this.OutputMax = XHelper.Read("OutputMax", e, 100);
            this.OutputMin = XHelper.Read("OutputMin", e, 0);
            this.GainDerivative = (float)XHelper.Read("GainDerivative", e, 0.05f);
            this.GainIntegral = (float)XHelper.Read("GainIntegral", e, 0.008f);
            this.GainProportional = (float)XHelper.Read("GainProportional", e, 0.1f);

            var tagxml = XHelper.Read("ProcessTag", e, "");
            if (!String.IsNullOrEmpty(tagxml))
                ProcessTag = tagxml.FromXml<OPCUAEntityReference>();
            tagxml = XHelper.Read("ControlTag", e, "");
            if (!String.IsNullOrEmpty(tagxml))
                ControlTag = tagxml.FromXml<OPCUAEntityReference>();
            tagxml = XHelper.Read("SetPointTag", e, "");
            if (!String.IsNullOrEmpty(tagxml))
                SetPointTag = tagxml.FromXml<OPCUAEntityReference>();
        }
#endregion
    }
}
