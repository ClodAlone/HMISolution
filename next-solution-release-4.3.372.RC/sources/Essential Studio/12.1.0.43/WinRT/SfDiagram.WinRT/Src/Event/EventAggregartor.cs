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
#if WINRT_USING
using System.Threading.Tasks; 
#endif

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
        public Event()
        {
            HasSubscripitons = false;
            IsSuspended = false;
        }

        private readonly List<Tuple<MethodInfo, WeakReference>> _mSubscriptions
            = new List<Tuple<MethodInfo, WeakReference>>();

        public bool HasSubscripitons { get; set; }

        public bool IsSuspended { get; set; }

        public void Publish(TEvent args)
        {
            if (IsSuspended)
            {
                return;
            }
            List<Tuple<MethodInfo, WeakReference>> dead = null;
            foreach (Tuple<MethodInfo, WeakReference> toPublish in _mSubscriptions)
            {
                if (!toPublish.Item2.IsAlive)
                {
                    if (dead == null)
                    {
                        dead = new List<Tuple<MethodInfo, WeakReference>>();
                    }
                    dead.Add(toPublish);
                    continue;
                }
                object[] arguments = new object[] {args};
                MethodInfo action = toPublish.Item1;
                object target = toPublish.Item2.Target;
                    action.Invoke(target, arguments);
            }
            while (dead != null && dead.Count!=0)
            {
                _mSubscriptions.Remove(dead[0]);
                dead.RemoveAt(0);
            }
        }

        public void Subscribe(Action<TEvent> action, bool root = false)
        {
            WeakReference refer = new WeakReference(action.Target);
            MethodInfo actionMethod = action.GetMethodInfo();
            var tup = new Tuple<MethodInfo, WeakReference>(actionMethod, refer);
            if (root)
            {
                _mSubscriptions.Insert(0, tup);
            }
            else
            {
                _mSubscriptions.Add(tup);
            }
            HasSubscripitons = true;
            //return new UnSubscriber<TEvent>(subscriptions, tup);
        }

        public void UnSubscribe(Action<TEvent> action)
        {
            MethodInfo actionMethod = action.GetMethodInfo();
            Tuple<MethodInfo, WeakReference> tup = null;
            foreach (var item in _mSubscriptions)
            {
                if (item.Item1 == actionMethod)
                {
                    tup = item;
                    break;
                }
            }
            _mSubscriptions.Remove(tup);
            if (_mSubscriptions.Count == 0)
            {
                HasSubscripitons = false;
            }
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
