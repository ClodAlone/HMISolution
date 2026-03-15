using DocumentManager.ComponentService;
using OPCUAViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UFUAEditor.ComponentService;
using Utilities;
using WPFUtilities;

namespace SmartControlUtilities
{
    public class Helpers
    {
        internal static void SetProperty(object value, string proName, object control)
        {
            //null is a valid value
            if (/*value == null || */proName == null || control == null)
                return;
            var propertyInfo = control.GetType().GetProperty(proName);
            propertyInfo?.SetValue(control, value, null);
        }
        internal static object GetPropertyValue(string proName, object control)
        {
            if (control == null || proName == null)
                return null;
            var propertyInfo = control.GetType().GetProperty(proName);
            return propertyInfo?.GetValue(control);
        }

        public static OPCUAEntityReference GetTagReference(string tagName, IDocument Document)
        {
            string instance;
            string name;
            IUFUAEditorManager UFUAEditor = Document?.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
            if (UFUAEditor == null)
                return null;
            SmartControlHelper.GetInstanceName(tagName, out instance, out name);
            var xml = UFUAEditor.GetTagEntityReference(Document, name, instance);
            if (String.IsNullOrEmpty(xml))
            {
                OPCUAEntityReference original;
                original = new OPCUAEntityReference(null);
                original.HumanReadable = original.RelativePath = tagName;
                original.ResolvedNodeId = null;
                return original;
            }
            else
            {
                var tag = xml.FromXml<OPCUAEntityReference>();
                if (tag != null)
                    return tag;
            }

            return null;
        }
    }
}
