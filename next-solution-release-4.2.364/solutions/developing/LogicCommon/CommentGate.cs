using LogicCore;
using Northwoods.GoXam;
using Northwoods.GoXam.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace LogicCommon
{
    public class CommentGate : GateData
    {
        #region Abstracts
        public override bool Execute(WireData[] links)
        {
            return false;
        }

        public override String Name
        {
            get
            {
#if WINDOWS_UWP
                return "Comment";
#else
                return Properties.Settings.Default.CommentName;
#endif
            }
        }
#if !WINDOWS_UWP && !NET_STANDARD
        public override DataTemplateDictionary GetDataTemplates()
        {
            var resourceDictionary = new ResourceDictionary();
            resourceDictionary.Source = new Uri("/LogicCommon;component/ResourceDictionary/CommentLibrary.xaml", UriKind.Relative);
            return resourceDictionary[NodeTemplateDictionary] as DataTemplateDictionary;
        }
        public override List<GateData> GetTypes()
        {
            var ret = base.GetTypes();
            ret.Add(new CommentGate() { Key = "Comment", Text = "Comment", GateDataTemplate = "Comment", Category = "Comment" });
            return ret;
        }
#endif
#endregion

#region Properties
#if !WINDOWS_UWP && !NET_STANDARD
        public double Width
        {
            get { return _Width; }
            set { if (_Width != value) { double old = _Width; _Width = value; RaisePropertyChanged("Width", old, value); } }
        }
        private double _Width = 100;

        public double Height
        {
            get { return _Height; }
            set { if (_Height != value) { double old = _Height; _Height = value; RaisePropertyChanged("Height", old, value); } }
        }
        private double _Height = 100;
#endif
#endregion

#region Overrides
// support standard reading/writing via Linq for XML
#if !WINDOWS_UWP && !NET_STANDARD
        public override XElement MakeXElement(XName n)
        {
            XElement e = base.MakeXElement(n);
            e.Add(XHelper.Attribute("Width", this.Width, 100));
            e.Add(XHelper.Attribute("Height", this.Height, 100));
            return e;
        }

        public override void LoadFromXElement(XElement e)
        {
            base.LoadFromXElement(e);
            this.Width = XHelper.Read("Width", e, 100);
            this.Height = XHelper.Read("Height", e, 100);
        }
#endif
#endregion
    }
}
