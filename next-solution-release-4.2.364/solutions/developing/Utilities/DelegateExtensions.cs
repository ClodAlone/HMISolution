using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Utilities
{
    public static class DelegateExtensions
    {
        /// <summary>
        /// Detects whether an event has any subscribers
        /// </summary>
        [Conditional("DEBUG")]
        public static void CheckEventHasNoSubscribers(this Delegate eventDelegate)
        {
            if (eventDelegate != null)
            {
                // if the event has any subscribers, create an informative error message.
                if (eventDelegate.GetInvocationList().Length != 0)
                {
                    int subscriberCount = eventDelegate.GetInvocationList().Length;

                    // determine the consumers of this event
                    StringBuilder subscribers = new StringBuilder();
                    foreach (Delegate del in eventDelegate.GetInvocationList())
                    {
                        subscribers.Append((subscribers.Length != 0 ? ", " : "") + del.Target.ToString());
                    }

                    // name and shame them!
                    Debug.WriteLine(string.Format("Event: {0} still has {1} subscribers, with the following targets [{2}]",
                        eventDelegate.Method.Name, subscriberCount, subscribers.ToString()));
                }
            }
        }
    }
}
