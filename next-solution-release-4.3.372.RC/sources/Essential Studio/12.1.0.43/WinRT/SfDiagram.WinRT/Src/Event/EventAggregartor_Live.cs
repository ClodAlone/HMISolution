#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Syncfusion.UI.Xaml.Diagram
{
    class EventAggregartor : IDisposable
    {
        private readonly Dictionary<Type, EventBase> _mEvents = new Dictionary<Type, EventBase>();

        public virtual TEventType GetEvent<TEventType>(params object[] param) where TEventType : EventBase
        {
            Type eventType = typeof(TEventType);
            if (_mEvents.ContainsKey(eventType))
            {
                return _mEvents[eventType] as TEventType;
            }
            else
            {
                TEventType newEvent = Activator.CreateInstance(typeof(TEventType), param) as TEventType;
                _mEvents.Add(eventType, newEvent);
                return newEvent;
            }
        }

        public void Dispose()
        {
            foreach (var item in _mEvents)
            {
                item.Value.Dispose();
            }
            _mEvents.Clear();
        }
    }

    abstract class EventBase : IDisposable
    {
        public abstract void Dispose();
    }

    class Event<TEvent> : EventBase, IDisposable
    {
        private readonly List<Tuple<MethodInfo, WeakReference, Action<TEvent>>> _mSubscriptions
            = new List<Tuple<MethodInfo, WeakReference, Action<TEvent>>>();

        public void Publish(TEvent args)
        {
            List<Tuple<MethodInfo, WeakReference, Action<TEvent>>> dead =
                new List<Tuple<MethodInfo, WeakReference, Action<TEvent>>>();
            foreach (Tuple<MethodInfo, WeakReference, Action<TEvent>> toPublish in _mSubscriptions)
            {
                if (!toPublish.Item2.IsAlive)
                {
                    dead.Add(toPublish);
                    continue;
                }
                object[] arguments = new object[] {args};
                MethodInfo action = toPublish.Item1;
                object target = toPublish.Item2.Target;
                action.Invoke(target, arguments);
                //toPublish.Item3.Invoke(args);
            }
            while (dead.Count!=0)
            {
                _mSubscriptions.Remove(dead[0]);
                dead.RemoveAt(0);
            }
        }

        public void Subscribe(Action<TEvent> action, bool root = false)
        {
            WeakReference refer = new WeakReference(action.Target);
            MethodInfo actionMethod = action.GetMethodInfo();
            var tup = new Tuple<MethodInfo, WeakReference, Action<TEvent>>(actionMethod, refer, action);
            if (root)
            {
                _mSubscriptions.Insert(0, tup);
            }
            else
            {
                _mSubscriptions.Add(tup);
            }
            //return new UnSubscriber<TEvent>(subscriptions, tup);
        }

        public void UnSubscribe(Action<TEvent> action)
        {
            MethodInfo actionMethod = action.GetMethodInfo();
            Tuple<MethodInfo, WeakReference, Action<TEvent>> tup = null;
            foreach (var item in _mSubscriptions)
            {
                if (item.Item1 == actionMethod)
                {
                    tup = item;
                    break;
                }
            }
            _mSubscriptions.Remove(tup);
        }

        /*
        class UnSubscriber<TDisposeEvent> : IDisposable
        {
            private readonly List<Tuple<MethodInfo, MethodInfo, WeakReference>> _mCollection;
            private readonly Tuple<MethodInfo, MethodInfo, WeakReference> _mItem;

            public UnSubscriber(List<Tuple<MethodInfo, MethodInfo, WeakReference>> sub,
                                Tuple<MethodInfo, MethodInfo, WeakReference> element)
            {
                _mCollection = sub;
                _mItem = element;
            }

            public void Dispose()
            {
                _mCollection.Remove(_mItem);
            }
        }
        */

        public override void Dispose()
        {
            _mSubscriptions.Clear();
        }
    }
}
