using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace WPFUtilities.Extensions
{
    public static class DisposeExtension
    {
        public static void DisposeInApplicationIdle(this IDisposable obj)
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, (Action)(() => { obj.Dispose(); }));
        }
    }
}
