using LogicCore;
using Northwoods.GoXam;
using Northwoods.GoXam.Model;
using Opc.Ua;
using OPCUAViewModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using Utilities;

namespace LogicCommon
{
    public class OutputGate : GateData
#if !WINDOWS_UWP && !NET_STANDARD
        , ICloneable
#endif
    {
#if !WINDOWS_UWP && !NET_STANDARD
#region ICloneable Members

        public override object Clone()
        {
            var ret = base.Clone() as OutputGate; 
            if (Tag != null)
            {
                var tagxml = Tag.ToXml();
                if (!String.IsNullOrEmpty(tagxml))
                    ret.Tag = tagxml.FromXml<OPCUAEntityReference>();
            }
            if (SetValueTag != null)
            {
                var tagxml = SetValueTag.ToXml();
                if (!String.IsNullOrEmpty(tagxml))
                    ret.SetValueTag = tagxml.FromXml<OPCUAEntityReference>();
            }

            return ret;
        }

#endregion
#endif

#region Abstracts
        public override bool Execute(WireData[] links)
        {
            if (links == null || links.Length == 0)
                return false;

            var ret = links[0].Value;
            return ret;
        }

        public override void Terminate()
        {
            _runtimeTag = null;
        }

        public override GateTypes GateType { get { return GateTypes.Output; } }

#if !WINDOWS_UWP && !NET_STANDARD
        public override bool CanDropTag() { return true; }

        public override bool DroppingTag(OPCUAEntityReference tag, String title)
        {
            Tag = tag;
            Text = title;
            return true;
        }
#endif
        public override bool ReadData()
        {
            bool ret = base.ReadData();
            var bret1 = false;
            var bret2 = false;
            if (_runtimeTag != null && _runtimeTag.MonitoredItemViewModel != null &&
                _runtimeTag.MonitoredItemViewModel.DataValue != null)
            {
                if (LogicCore.Helpers.StatusCodeHelper.IsGoodOrUncertainLastUsable(_runtimeTag.MonitoredItemViewModel.DataValue.StatusCode))
                    bret1 = true;
            }

            if (_runtimeSetValueTag != null && _runtimeSetValueTag.MonitoredItemViewModel != null &&
                _runtimeSetValueTag.MonitoredItemViewModel.DataValue != null)
            {
                if (LogicCore.Helpers.StatusCodeHelper.IsGoodOrUncertainLastUsable(_runtimeSetValueTag.MonitoredItemViewModel.DataValue.StatusCode))
                    bret2 = true;
            }

            return ret && (bret1 || _runtimeTag == null) && (bret2 || _runtimeSetValueTag == null);
        }

        public override void WriteData()
        {
            base.WriteData();

            var setValue = SetValue;
            if (_runtimeSetValueTag != null && _runtimeSetValueTag.MonitoredItemViewModel != null &&
                _runtimeSetValueTag.MonitoredItemViewModel.DataValue != null)
            {
                try
                {
                    try
                    {
                        setValue = Convert.ToString(_runtimeSetValueTag.MonitoredItemViewModel.DataValue.Value, CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(String.Format(Properties.Resources.WriteVariableException, _runtimeSetValueTag.HumanReadable));
                }
            }

            if (_runtimeTag != null && _runtimeTag.MonitoredItemViewModel != null &&
                _runtimeTag.MonitoredItemViewModel.DataValue != null)
            {
                try
                {
                    bool bWrite = true;
                    if (String.IsNullOrEmpty(setValue) || !Value)
                    {
                        try
                        {
                            var bCompare = Convert.ToBoolean(_runtimeTag.MonitoredItemViewModel.DataValue.Value);
                            if (bCompare == Value)
                                bWrite = false;
                        }
                        catch
                        {
                        }
                        if (bWrite)
                            _runtimeTag.MonitoredItemViewModel.WriteValue(Value);
                    }
                    else
                    {
                        try
                        {
                            var sCompare = Convert.ToString(_runtimeTag.MonitoredItemViewModel.DataValue.Value, CultureInfo.InvariantCulture); 
                            if (sCompare == setValue)
                                bWrite = false;
                        }
                        catch
                        {
                        }
                        if (bWrite)
                            _runtimeTag.MonitoredItemViewModel.WriteValue(setValue);
                    }
                }
                catch(Exception ex)
                {
                    throw new Exception(String.Format(Properties.Resources.WriteVariableException, _runtimeTag.HumanReadable));
                }
            }
        }

        public override List<OPCUAEntityReference> GetTagList()
        {
            var ret = base.GetTagList();
            if (_runtimeTag != null)
                ret.Add(_runtimeTag);
            else if (Tag != null)
            {
                var tagxml = Tag.ToXml();
                if (!String.IsNullOrEmpty(tagxml))
                    _runtimeTag = tagxml.FromXml<OPCUAEntityReference>();

                if (_runtimeTag != null)
                    ret.Add(_runtimeTag);
            }

            if (_runtimeSetValueTag != null)
                ret.Add(_runtimeSetValueTag);
            else if (SetValueTag != null)
            {
                var tagxml = SetValueTag.ToXml();
                if (!String.IsNullOrEmpty(tagxml))
                    _runtimeSetValueTag = tagxml.FromXml<OPCUAEntityReference>();

                if (_runtimeSetValueTag != null)
                    ret.Add(_runtimeSetValueTag);
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
                Tag = list[0];
                var tagxml = Tag.ToXml();
                if (!String.IsNullOrEmpty(tagxml))
                    _runtimeTag = tagxml.FromXml<OPCUAEntityReference>();

                if (list.Count >= 2)
                {
                    SetValueTag = list[1];
                    tagxml = SetValueTag.ToXml();
                    if (!String.IsNullOrEmpty(tagxml))
                        _runtimeSetValueTag = tagxml.FromXml<OPCUAEntityReference>();
                }
            }
        }
#endif
        public override String Name
        {
            get
            {
#if WINDOWS_UWP
                return "Output";
#else
                return Properties.Settings.Default.OutputGateName;
#endif
            }
        }
#if !WINDOWS_UWP && !NET_STANDARD
        public override DataTemplateDictionary GetDataTemplates()
        {
            var resourceDictionary = new ResourceDictionary();
            resourceDictionary.Source = new Uri("/LogicCommon;component/ResourceDictionary/OutputLibrary.xaml", UriKind.Relative);
            return resourceDictionary[NodeTemplateDictionary] as DataTemplateDictionary;
        }
        public override List<GateData> GetTypes()
        {
            var ret = base.GetTypes();
            ret.Add(new OutputGate() { Key = "Output", Text = "Output", GateDataTemplate = "Output", Category = "Output" });
            return ret;
        }
#endif
        #endregion

        #region Properties

        OPCUAEntityReference _runtimeTag;
        OPCUAEntityReference _Tag;
        public OPCUAEntityReference Tag
        {
            get { return _Tag; }
            set
            {
                var old = _Tag;
                if (old != value)
                {
                    _Tag = value;
                    RaisePropertyChanged("Tag", old, value);
                }
            }
        }

        OPCUAEntityReference _runtimeSetValueTag;
        OPCUAEntityReference _SetValueTag;
        public OPCUAEntityReference SetValueTag
        {
            get { return _SetValueTag; }
            set
            {
                var old = _SetValueTag;
                if (old != value)
                {
                    _SetValueTag = value;
                    RaisePropertyChanged("SetValueTag", old, value);
                }
            }
        }

        public String SetValue
        {
            get { return _SetValue; }
            set
            {
                var old = _SetValue;
                if (old != value)
                {
                    _SetValue = value;
                    RaisePropertyChanged("SetValue", old, value);
                }
            }
        }
        private String _SetValue;

#endregion

#region Overrides
// support standard reading/writing via Linq for XML
#if !WINDOWS_UWP && !NET_STANDARD
        public override XElement MakeXElement(XName n)
        {
            XElement e = base.MakeXElement(n);
            e.Add(XHelper.Attribute("SetValue", this.SetValue, ""));
            if (this.Tag != null)
                e.Add(XHelper.Attribute("Tag", this.Tag.ToXml(), ""));
            if (this.SetValueTag != null)
                e.Add(XHelper.Attribute("SetValueTag", this.SetValueTag.ToXml(), ""));
            return e;
        }
#endif
        public override void LoadFromXElement(XElement e)
        {
            base.LoadFromXElement(e);
            this.SetValue = XHelper.Read("SetValue", e, "");
            var tagxml = XHelper.Read("Tag", e, "");
            if (!String.IsNullOrEmpty(tagxml))
                Tag = tagxml.FromXml<OPCUAEntityReference>();
            tagxml = XHelper.Read("SetValueTag", e, "");
            if (!String.IsNullOrEmpty(tagxml))
                SetValueTag = tagxml.FromXml<OPCUAEntityReference>();
        }
        #endregion
    }
}
