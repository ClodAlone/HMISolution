#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace Syncfusion.XlsIO.Implementation
{
    public static class UIDispatcher
    {
        private static CoreDispatcher dispatcher;

        public static void Initialize()
        {
            dispatcher = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;
        }

        //public static async void BeginExecute(Action action)
        //{
        //    if (dispatcher.HasThreadAccess)
        //        action();

        //    else dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => action());
        //}

        public static void Execute(Action action)
        {
            InnerExecute(action).Wait();
        }

        private static async Task InnerExecute(Action action)
        {
            if (dispatcher.HasThreadAccess)
                action();

            else await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => action());
        }
    }
}