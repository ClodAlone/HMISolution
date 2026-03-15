using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DocumentManager.ComponentService
{
    public interface IToolbar
    {
#if !WINDOWS_UWP && !NET_STANDARD
        CommandBindingCollection BarCommandBindings
        {
            get;
        }
        System.Windows.FrameworkContentElement ToolbarMenuItem { get; set; }
        void Show();
        void Hide();
        void ShowMenuItem();
        bool HideMenuItem();
        bool IsVisible();
#endif
    }
}