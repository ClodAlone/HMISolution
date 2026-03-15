#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
#if WINRT_USING
using System.Threading.Tasks;
using Windows.Foundation;
#endif
#if !WPF
using Syncfusion.UI.Xaml.Diagram.Utility;
#endif

namespace Syncfusion.UI.Xaml.Diagram
{
    public class StraightSegment : ILineSegment
    {
        private Point? _mPoint;
        //private Editable _mEditable;

        public Point? Point
        {
            get { return _mPoint; }
            set
            {
                if (_mPoint != value)
                {
                    _mPoint = value;
                    OnPropertyChanged("Point");
                }
            }
        }

        /// <summary>
        /// Gets or sets Smooth
        /// </summary>
        private BezierSmoothness _Smooth;

        public BezierSmoothness BezierSmoothness
        {
            get { return _Smooth; }
            set { _Smooth = value; }
        }

        /// <summary>
        /// Gets or sets InheritSmooth
        /// </summary>
        private SegmentConstraints _constraints;

        public SegmentConstraints Constraints
        {
            get { return _constraints; }
            set { _constraints = value; }
        }
        
        //public Editable Editable
        //{
        //    get { return _mEditable; }
        //    set
        //    {
        //        if (_mEditable != value)
        //        {
        //            _mEditable = value;
        //            OnPropertyChanged("Editable");
        //        }
        //    }
        //}
        
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

    }

    public class LineSegmentLength : ILineSegmentLength
    {
        public LineSegmentLength()
        {
            _mLength.DesiredValue = 20;
            _mAngle.DesiredValue = 0;
        }

        private DoubleExt _mLength = new DoubleExt(double.NaN);
        private DoubleExt _mAngle = new DoubleExt(double.NaN);
        private RelativeMode _mAngleMode;
        private Editable _mEditable = Editable.All;

        public DoubleExt Length
        {
            get { return _mLength; }
            set
            {
                if (_mLength != value)
                {
                    _mLength = value;
                    OnPropertyChanged("Length");
                }
                else
                {
                    _mLength = value;
                }
            }
        }

        public DoubleExt Angle
        {
            get { return _mAngle; }
            set
            {
                if (_mAngle != value)
                {
                    _mAngle = value;
                    OnPropertyChanged("Angle");
                }
                else
                {
                    _mAngle = value;
                }
            }
        }

        public RelativeMode AngleMode
        {
            get { return _mAngleMode; }
            set
            {
                if (_mAngleMode != value)
                {
                    _mAngleMode = value;
                    OnPropertyChanged("AngleMode");
                }
            }
        }

        public Editable Editable
        {
            get { return _mEditable; }
            set
            {
                if (_mEditable != value)
                {
                    _mEditable = value;
                    OnPropertyChanged("Editable");
                }
            }
        }

        /// <summary>
        /// Gets or sets Smooth
        /// </summary>
        private BezierSmoothness _Smooth;

        public BezierSmoothness BezierSmoothness
        {
            get { return _Smooth; }
            set { _Smooth = value; }
        }

        /// <summary>
        /// Gets or sets InheritSmooth
        /// </summary>
        private SegmentConstraints _inheritSmooth;

        public SegmentConstraints Constraints
        {
            get { return _inheritSmooth; }
            set { _inheritSmooth = value; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

    }

    public class OrthogonalSegment : IOrthogonalSegment
    {
        public OrthogonalSegment()
        {
            _mLength.DesiredValue = 20;
        }

        private DoubleExt _mLength = new DoubleExt(double.NaN);
        private OrthogonalDirection _mDirection = OrthogonalDirection.Auto;
        private Editable _mEditable = Editable.None;

        public DoubleExt Length
        {
            get { return _mLength; }
            set
            {
                if (_mLength != value)
                {
                    _mLength = value;
                    OnPropertyChanged("Length");
                }
            }
        }

        public OrthogonalDirection Direction
        {
            get { return _mDirection; }
            set
            {
                if (_mDirection != value)
                {
                    _mDirection = value;
                    OnPropertyChanged("Direction");
                }
            }
        }

        public Editable Editable
        {
            get { return _mEditable; }
            set
            {
                if (_mEditable != value)
                {
                    _mEditable = value;
                    OnPropertyChanged("Editable");
                }
            }
        }

        /// <summary>
        /// Gets or sets Smooth
        /// </summary>
        private BezierSmoothness _Smooth;

        public BezierSmoothness BezierSmoothness
        {
            get { return _Smooth; }
            set { _Smooth = value; }
        }

        /// <summary>
        /// Gets or sets InheritSmooth
        /// </summary>
        private SegmentConstraints _inheritSmooth;

        public SegmentConstraints Constraints
        {
            get { return _inheritSmooth; }
            set { _inheritSmooth = value; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
        
    }


    public class CubicCurveSegment : ICubicCurveSegment
    {
        private Point? _mP1;
        private Point? _mP2;
        private Point? _mP3;
        private Editable _mEditable;
        private Vector? _vector1;
        private Vector? _vector2;
        public CubicCurveSegment()
        {
            Editable = Editable.None;
        }

        public Editable Editable
        {
            get { return _mEditable; }
            set
            {
                if (_mEditable != value)
                {
                    _mEditable = value;
                    OnPropertyChanged("Editable");
                }
            }
        }

        public Point? Point1
        {
            get { return _mP1; }
            set
            {
                if (_mP1 != value)
                {
                    _mP1 = value;
                    OnPropertyChanged("Point1");
                }
            }
        }

        public Point? Point2
        {
            get { return _mP2; }
            set
            {
                if (_mP2 != value)
                {
                    _mP2 = value;
                    OnPropertyChanged("Point2");
                }
            }
        }

        public Point? Point3
        {
            get { return _mP3; }
            set
            {
                if (_mP3 != value)
                {
                    _mP3 = value;
                    OnPropertyChanged("Point3");
                }
            }
        }

        /// <summary>
        /// Gets or sets the vector representation of Point1
        /// </summary>
        public Vector? Vector1
        {
            get { return _vector1; }
            set
            {
                if (_vector1 != value)
                {
                    _vector1 = value;
                    OnPropertyChanged("Vector1");
                }
            }
        }

        /// <summary>
        /// Gets or sets the vector representation of Point2
        /// </summary>
        public Vector? Vector2
        {
            get { return _vector2; }
            set
            {
                if (_vector2 != value)
                {
                    _vector2 = value;
                    OnPropertyChanged("Vector2");
                }
            }
        }

        /// <summary>
        /// Gets or sets Smooth
        /// </summary>
        private BezierSmoothness _Smooth;

        public BezierSmoothness BezierSmoothness
        {
            get { return _Smooth; }
            set { _Smooth = value; }
        }

        /// <summary>
        /// Gets or sets InheritSmooth
        /// </summary>
        private SegmentConstraints _inheritSmooth;

        public SegmentConstraints Constraints
        {
            get { return _inheritSmooth; }
            set { _inheritSmooth = value; }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }

    public class QuadraticCurveSegment : IQuadraticCurveSegment
    {
        private Point? _mP1;
        private Point? _mP2;
        private Editable _mEditable;

        public QuadraticCurveSegment()
        {
            Editable = Editable.None;
        }

        public Editable Editable
        {
            get { return _mEditable; }
            set
            {
                if (_mEditable != value)
                {
                    _mEditable = value;
                    OnPropertyChanged("Editable");
                }
            }
        }

        /// <summary>
        /// Gets or sets Smooth
        /// </summary>
        private BezierSmoothness _Smooth;

        public BezierSmoothness BezierSmoothness
        {
            get { return _Smooth; }
            set { _Smooth = value; }
        }

        /// <summary>
        /// Gets or sets InheritSmooth
        /// </summary>
        private SegmentConstraints _inheritSmooth;

        public SegmentConstraints Constraints
        {
            get { return _inheritSmooth; }
            set { _inheritSmooth = value; }
        }

        public Point? Point1
        {
            get { return _mP1; }
            set
            {
                if (_mP1 != value)
                {
                    _mP1 = value;
                    OnPropertyChanged("Point1");
                }
            }
        }

        public Point? Point2
        {
            get { return _mP2; }
            set
            {
                if (_mP2 != value)
                {
                    _mP2 = value;
                    OnPropertyChanged("Point2");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }

    
}