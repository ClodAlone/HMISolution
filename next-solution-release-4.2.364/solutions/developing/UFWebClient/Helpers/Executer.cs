using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace UFWebClient.Helpers
{
    public static class Execute
    {
        private static Action<Action> _executor = action => action();

        public static void InitializeWithDispatcher()
        {
            var dispatcher = Deployment.Current.Dispatcher;

            _executor = action =>
            {
                if (dispatcher.CheckAccess())
                    action();
                else dispatcher.BeginInvoke(action);
            };
        }

        public static void OnUIThread(this Action action)
        {
            _executor(action);
        }
    }
}
