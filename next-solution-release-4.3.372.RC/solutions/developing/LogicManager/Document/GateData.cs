using Northwoods.GoXam;
using Northwoods.GoXam.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LogicManager.Document
{
#if !SILVERLIGHT
    [Serializable]
#endif
    public class GateData : GraphLinksModelNodeData<String>
    {
        // the type of gate that this node represents; only set on initialization
        public String GateType { get; set; }

        // the shape of the node figure is bound to this property; only set on initialization
        public NodeFigure Figure { get; set; }

        // the current value of this gate;
        // the node color is bound to its Data's Value property and must update with it
        public Boolean Value
        {
            get { return _Value; }
            set
            {
                Boolean old = _Value;
                if (old != value)
                {
                    _Value = value;
                    RaisePropertyChanged("Value", old, value);
                }
            }
        }
        private Boolean _Value;

        // support standard reading/writing via Linq for XML
        public override XElement MakeXElement(XName n)
        {
            XElement e = base.MakeXElement(n);
            e.Add(XHelper.Attribute("GateType", this.GateType, ""));
            e.Add(XHelper.AttributeEnum<NodeFigure>("Figure", this.Figure, NodeFigure.Rectangle));
            e.Add(XHelper.Attribute("Value", this.Value, false));
            return e;
        }

        public override void LoadFromXElement(XElement e)
        {
            base.LoadFromXElement(e);
            this.GateType = XHelper.Read("GateType", e, "");
            this.Figure = XHelper.ReadEnum<NodeFigure>("Figure", e, NodeFigure.Rectangle);
            this.Value = XHelper.Read("Value", e, false);
        }
    }


#if !SILVERLIGHT
    [Serializable]
#endif
    public class WireData : GraphLinksModelLinkData<String, String>
    {
        // the link color is bound to its Data's Value property and must update with it
        public Boolean Value
        {
            get { return _Value; }
            set
            {
                Boolean old = _Value;
                if (old != value)
                {
                    _Value = value;
                    RaisePropertyChanged("Value", old, value);
                }
            }
        }
        private Boolean _Value;

        // the only additional property, Value, is not meant to be persistent
        // on "wires", so we don't need to override MakeXElement and LoadFromXElement
    }
}
