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
    public class DivideGate : GateData
#if !WINDOWS_UWP && !NET_STANDARD
        , ICloneable
#endif
    {
#if !WINDOWS_UWP && !NET_STANDARD
#region ICloneable Members

        public override object Clone()
        {
            var ret = base.Clone() as DivideGate; 
            if (Tag != null)
            {
                var tagxml = Tag.ToXml();
                if (!String.IsNullOrEmpty(tagxml))
                    ret.Tag = tagxml.FromXml<OPCUAEntityReference>();
            }

            if (DivideValueTag != null)
            {
                var tagxml = DivideValueTag.ToXml();
                if (!String.IsNullOrEmpty(tagxml))
                    ret.DivideValueTag = tagxml.FromXml<OPCUAEntityReference>();
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
            _runtimeDivideValueTag = null;
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
            if (_runtimeDivideValueTag != null && _runtimeDivideValueTag.MonitoredItemViewModel != null &&
                _runtimeDivideValueTag.MonitoredItemViewModel.DataValue != null)
            {
                if (LogicCore.Helpers.StatusCodeHelper.IsGoodOrUncertainLastUsable(_runtimeDivideValueTag.MonitoredItemViewModel.DataValue.StatusCode))
                    bret2 = true;
            }

            return ret && bret1 && (bret2 || _runtimeDivideValueTag == null);
        }

        public override void WriteData()
        {
            base.WriteData();

            var divideValue = DivideValue;
            if (_runtimeDivideValueTag != null && _runtimeDivideValueTag.MonitoredItemViewModel != null &&
                _runtimeDivideValueTag.MonitoredItemViewModel.DataValue != null)
            {
                try
                {
                    try
                    {
                        divideValue = Convert.ToDouble(_runtimeDivideValueTag.MonitoredItemViewModel.DataValue.Value);
                    }
                    catch
                    {
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(String.Format(Properties.Resources.WriteVariableException, _runtimeDivideValueTag.HumanReadable));
                }
            }

            if (_runtimeTag != null && _runtimeTag.MonitoredItemViewModel != null &&
                _runtimeTag.MonitoredItemViewModel.DataValue != null)
            {
                try
                {
                    if (Value)
                    {
                        try
                        {
                            var sCompare = Convert.ToDouble(_runtimeTag.MonitoredItemViewModel.DataValue.Value) / divideValue;
                            _runtimeTag.MonitoredItemViewModel.WriteValue(sCompare);
                        }
                        catch
                        {
                        }
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

            if (_runtimeDivideValueTag != null)
                ret.Add(_runtimeDivideValueTag);
            else if (DivideValueTag != null)
            {
                var tagxml = DivideValueTag.ToXml();
                if (!String.IsNullOrEmpty(tagxml))
                    _runtimeDivideValueTag = tagxml.FromXml<OPCUAEntityReference>();

                if (_runtimeDivideValueTag != null)
                    ret.Add(_runtimeDivideValueTag);
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
                    DivideValueTag = list[1];
                    tagxml = Tag.ToXml();
                    if (!String.IsNullOrEmpty(tagxml))
                        _runtimeDivideValueTag = tagxml.FromXml<OPCUAEntityReference>();
                }
            }
        }
#endif
        public override String Name
        {
            get
            {
#if WINDOWS_UWP
                return "Divide";
#else
                return Properties.Settings.Default.DivideGateName;
#endif
            }
        }
#if !WINDOWS_UWP && !NET_STANDARD
        public override DataTemplateDictionary GetDataTemplates()
        {
            var resourceDictionary = new ResourceDictionary();
            resourceDictionary.Source = new Uri("/LogicCommon;component/ResourceDictionary/MathLibrary.xaml", UriKind.Relative);
            return resourceDictionary[NodeTemplateDictionary] as DataTemplateDictionary;
        }
        public override List<GateData> GetTypes()
        {
            var ret = base.GetTypes();
            ret.Add(new DivideGate() { Key = "Divide", Text = "Divide", GateDataTemplate = "Divide", Category = "Divide" });
            return ret;
        }

        public override String GetCategory() { return Properties.Resources.MathCategory; }
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

        OPCUAEntityReference _runtimeDivideValueTag;
        OPCUAEntityReference _DivideValueTag;
        public OPCUAEntityReference DivideValueTag
        {
            get { return _DivideValueTag; }
            set
            {
                var old = _DivideValueTag;
                if (old != value)
                {
                    _DivideValueTag = value;
                    RaisePropertyChanged("DivideValueTag", old, value);
                }
            }
        }

        public Double DivideValue
        {
            get { return _DivideValue; }
            set
            {
                var old = _DivideValue;
                if (old != value)
                {
                    _DivideValue = value;
                    RaisePropertyChanged("Divide", old, value);
                }
            }
        }
        private Double _DivideValue = 1.0;

        #endregion

        #region Overrides
        // support standard reading/writing via Linq for XML
#if !WINDOWS_UWP && !NET_STANDARD
        public override XElement MakeXElement(XName n)
        {
            XElement e = base.MakeXElement(n);
            e.Add(XHelper.Attribute("DivideValue", this.DivideValue, 1.0));
            if (this.Tag != null)
                e.Add(XHelper.Attribute("Tag", this.Tag.ToXml(), ""));
            if (this.DivideValueTag != null)
                e.Add(XHelper.Attribute("DivideValueTag", this.DivideValueTag.ToXml(), ""));
            return e;
        }
#endif
        public override void LoadFromXElement(XElement e)
        {
            base.LoadFromXElement(e);
            this.DivideValue = XHelper.Read("DivideValue", e, 1.0);
            var tagxml = XHelper.Read("Tag", e, "");
            if (!String.IsNullOrEmpty(tagxml))
                Tag = tagxml.FromXml<OPCUAEntityReference>();
            tagxml = XHelper.Read("DivideValueTag", e, "");
            if (!String.IsNullOrEmpty(tagxml))
                DivideValueTag = tagxml.FromXml<OPCUAEntityReference>();
        }
        #endregion
    }
}
