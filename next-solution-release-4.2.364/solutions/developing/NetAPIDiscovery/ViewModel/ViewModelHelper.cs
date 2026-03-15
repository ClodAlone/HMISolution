using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace NetAPIDiscovery.ViewModel
{
    internal static class ViewModelHelper
    {
        internal static BitmapImage GetControlImage(string image, bool bShared = false)
        {
            var bm = SharedResources.Helpers.ResourceManager.GetCommonImage("NetAPIDiscovery", image, bShared);
            return bm;
        }
    }
}
