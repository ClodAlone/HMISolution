using System;
using System.Collections;

namespace UFInterfaces
{
    public class GetFriendObjectsEventArgs : EventArgs
    {
        public String Name;
        public String Container;
        public Object friend;
        public IList friendList;
        public Type expectedType;
    }
}
