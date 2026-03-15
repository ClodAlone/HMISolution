using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;

namespace KinectControls
{
    public class HandPointEventArgs : EventArgs
    {
        public Point3D Position { get; set; }
        public int UserID { get; set; }
        public int ID { get; set; }

        public HandPointEventArgs(int id, int userId, Point3D point)
        {
            this.Position = point;
            this.UserID = userId;
            this.ID = id;
        }
    }
}
