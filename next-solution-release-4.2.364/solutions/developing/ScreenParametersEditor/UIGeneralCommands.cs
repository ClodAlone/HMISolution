using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace ScreenParametersEditor
{
    public static class UIGeneralCommands
    {

        private static GeneralCommand _AddNewParameterItem = new GeneralCommand(
              Properties.UICommandResource.AddNewParameterItemName,
              Properties.UICommandResource.AddNewParameterItemText,
              Properties.UICommandResource.AddNewParameterItemGestures,
              Properties.UICommandResource.AddNewParameterItemGesturesDisplayText,
              Properties.UICommandResource.AddNewParameterItemTooltip,
              Properties.UICommandResource.AddNewParameterItemDescription,
              typeof(UIGeneralCommands));

        public static GeneralCommand AddNewParameterItem
        {
            get { return _AddNewParameterItem; }
        }

        private static GeneralCommand _ShowToolbar = new GeneralCommand(
             Properties.UICommandResource.ShowToolbarName,
             Properties.UICommandResource.ShowToolbarText,
             Properties.UICommandResource.ShowToolbarGestures,
             Properties.UICommandResource.ShowToolbarGesturesDisplayText,
             Properties.UICommandResource.ShowToolbarTooltip,
             Properties.UICommandResource.ShowToolbarDescription,
             typeof(UIGeneralCommands));

        public static GeneralCommand ShowToolbar
        {
            get { return _ShowToolbar; }
        }

    }
}
