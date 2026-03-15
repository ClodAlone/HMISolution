using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
namespace AnimationExplorer
{
    public static class UIAnimationCommands
    {
        private static GeneralCommand _CopyAll = new GeneralCommand(
            Properties.UICommandResource.CopyAllName,
            Properties.UICommandResource.CopyAllText,
            Properties.UICommandResource.CopyAllGestures,
            Properties.UICommandResource.CopyAllNameGesturesDisplayText,
            Properties.UICommandResource.CopyAllTooltip,
            Properties.UICommandResource.CopyAllDescription,
            typeof(UIAnimationCommands)); 
        public static GeneralCommand CopyAll
        {
            get { return _CopyAll; }
        }
    }
}
