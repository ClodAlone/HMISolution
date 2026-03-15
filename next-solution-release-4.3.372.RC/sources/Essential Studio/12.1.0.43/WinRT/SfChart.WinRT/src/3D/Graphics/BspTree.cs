#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections.Generic;


namespace Syncfusion.UI.Xaml.Charts
{
    #region Enums
    /// <summary>
    /// Specifies the point location by the plane.
    /// </summary>
    internal enum ClassifyPointResult
    {
        /// <summary>
        /// Point is in the front of plane.
        /// </summary>
        OnFront,

        /// <summary>
        /// Point is at the back of plane.
        /// </summary>
        OnBack,

        /// <summary>
        /// Point is on the plane.
        /// </summary>
        OnPlane
    }

    /// <summary>
    /// Specifies the polygon location by the plane.
    /// </summary>
    internal enum ClassifyPolyResult
    {
        /// <summary>
        /// Polygon is on the plane.
        /// </summary>
        OnPlane,

        /// <summary>
        /// Polygon is from right of the plane.
        /// </summary>
        ToRight,

        /// <summary>
        /// Polygon is from left of the plane.
        /// </summary>
        ToLeft,

        /// <summary>
        /// Location of polygon is unknown.
        /// </summary>
        Unknown
    }
    #endregion

    /// <summary>
    /// This class contains methods to compute the Binary Space Partitioning (BSP) tree.
    /// </summary>
    internal sealed class BspTreeBuilder
    {
        #region Constants

        private const double EPSILON = 0.0005;

        #endregion

        #region Members

        internal readonly List<Polygon3D> Polygons = new List<Polygon3D>();

        #endregion

        #region Properties

        /// <summary>
        /// Gets the <see>
        /// <cref>PiePrototype.Polygon</cref>
        /// </see>
        /// at the specified index.
        /// </summary>
        /// <value>
        /// The <see cref="Polygon3D"/>.
        /// </value>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public Polygon3D this[int index]
        {
            get
            {
                return Polygons[index];
            }
        }

        #endregion

        #region Public methods
        /// <summary>
        /// Adds the specified poly.
        /// </summary>
        /// <param name="poly">The poly.</param>
        /// <returns></returns>
        public int Add(Polygon3D poly)
        {
            Polygons.Add(poly);
            return Polygons.Count - 1;
        }

        /// <summary>
        /// Removes the specified polygon.
        /// </summary>
        /// <param name="polygon">The polygon.</param>
        public void Remove(Polygon3D polygon)
        {
            Polygons.Remove(polygon);
        }

        public void Clear()
        {
            Polygons.Clear();
        }

        public int Count()
        {
           return Polygons.Count;
        }


        /// <summary>
        /// Builds this instance.
        /// </summary>
        /// <returns></returns>
        public BspNode Build()
        {
            return Build(Polygons);
        }

        /// <summary>
        /// Builds the specified collection of polygons.
        /// </summary>
        /// <param name="arlist">The collection of polygons.</param>
        /// <returns></returns>
        public BspNode Build(List<Polygon3D> arlist)
        {
            if (arlist.Count < 1) return null;
            var bspNode = new BspNode();
            var plane = arlist[0];
            bspNode.Plane = plane;
            var arleft = new List<Polygon3D>(arlist.Count);
            var arright = new List<Polygon3D>(arlist.Count);

            for (int i = 1, len = arlist.Count; i < len; i++)
            {
                var pln = arlist[i];

                if (pln == plane) continue;
                var r = ClassifyPolygon(plane, pln);

                switch (r)
                {
                    case ClassifyPolyResult.OnPlane:
                    case ClassifyPolyResult.ToRight:
                        arright.Add(pln);
                        break;

                    case ClassifyPolyResult.ToLeft:
                        arleft.Add(pln);
                        break;

                    case ClassifyPolyResult.Unknown:
                        if (pln is Line3D || pln is UIElement3D)
                        {
                            arleft.Add(pln);
                        }
                        else
                        {
                            Polygon3D[] ps1, ps2;
                            SplitPolygon(pln, plane, out ps1, out ps2);
                            arleft.AddRange(ps1);
                            arright.AddRange(ps2);
                        }
                        break;
                }
            }

            if (arleft.Count > 0)
            {
                bspNode.Back = Build(arleft);
            }

            if (arright.Count > 0)
            {
                bspNode.Front = Build(arright);
            }

            return bspNode;
        }

        /// <summary>
        /// Gets the node count.
        /// </summary>
        /// <param name="el">The el.</param>
        /// <returns></returns>
        public int GetNodeCount(BspNode el)
        {
            return (el == null) ? 0 : 1 + GetNodeCount(el.Back) + GetNodeCount(el.Front);
        }

        #endregion

        #region Helper methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pln">The Polygon.</param>
        /// <param name="plg">The Polygon.</param>
        /// <returns></returns>
        private static ClassifyPolyResult ClassifyPolygon(Polygon3D pln, Polygon3D plg)
        {
            var res = ClassifyPolyResult.Unknown;
            var points = plg.Points;

            if (points == null)
                return res;
            var onBack = 0;
            var onFront = 0;
            var onPlane = 0;
            var normal = pln.Normal;
            var d = pln.D;

            for (int i = 0, len = points.Length; i < len; i++)
            {
                var r = -d - (points[i] & normal);

                if (r > EPSILON)
                {
                    onBack++;
                }
                else if (r < -EPSILON)
                {
                    onFront++;
                }
                else
                {
                    onPlane++;
                }

                if ((onBack > 0) && (onFront > 0))
                {
                    break;
                }
            }
            if (onPlane == points.Length)
            {
                res = ClassifyPolyResult.OnPlane;
            }
            else if (onFront + onPlane == points.Length)
            {
                res = ClassifyPolyResult.ToRight;
            }
            else if (onBack + onPlane == points.Length)
            {
                res = ClassifyPolyResult.ToLeft;
            }
            else
            {
                res = ClassifyPolyResult.Unknown;
            }
            return res;
        }

        /// <summary>
        /// Classifies the point.
        /// </summary>
        /// <param name="pt">The pt.</param>
        /// <param name="pln">The PLN.</param>
        /// <returns></returns>
        private static ClassifyPointResult ClassifyPoint(Vector3D pt, Polygon3D pln)
        {
            var res = ClassifyPointResult.OnPlane;
            var sv = -pln.D - (pt & pln.Normal);

            if (sv > EPSILON)
            {
                res = ClassifyPointResult.OnBack;
            }
            else if (sv < -EPSILON)
            {
                res = ClassifyPointResult.OnFront;
            }

            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="poly"></param>
        /// <param name="part"></param>
        /// <param name="backPoly"></param>
        /// <param name="frontPoly"></param>
        /// <returns></returns>
        private void SplitPolygon(Polygon3D poly, Polygon3D part, out Polygon3D[] backPoly, out Polygon3D[] frontPoly)
        {
            var backP = new List<Polygon3D>();
            var frontP = new List<Polygon3D>();

            // this code looks for points which lie on the part plane and divide polygon into two parts
            if (poly.Points != null)
            {
                var polyPoints = new List<Vector3DIndexClassification>();
                var backPartPoints = new List<Vector3DIndexClassification>();
                var frontPartPoints = new List<Vector3DIndexClassification>();

                var outpts = new List<Vector3D>();
                var inpts = new List<Vector3D>();

                var count = poly.Points.Length;
                for (var i = 0; i < count;i++)
                {
                    var ptB = poly.Points[i];
                    var ptC = poly.Points[GetNext(i + 1, count)];
                    var sideB = ClassifyPoint(ptB, part);
                    var sideC = ClassifyPoint(ptC, part);

                    var vwiwcB = new Vector3DIndexClassification(ptB, polyPoints.Count, sideB);
                    polyPoints.Add(vwiwcB);

                    if ((sideB != sideC) && (sideB != ClassifyPointResult.OnPlane) &&
                        (sideC != ClassifyPointResult.OnPlane))
                    {
                        var v = ptB - ptC;
                        var dir = part.Normal*(-part.D) - ptC;

                        var sv = dir & part.Normal;
                        var sect =sv/(part.Normal & v);
                        var ptP = ptC + v* sect;
                        var vwiwc = new Vector3DIndexClassification(ptP, polyPoints.Count,
                            ClassifyPointResult.OnPlane);

                        polyPoints.Add(vwiwc);
                        backPartPoints.Add(vwiwc);
                        frontPartPoints.Add(vwiwc);
                    }
                    else
                        if (sideB == ClassifyPointResult.OnPlane)
                        {
                            var ptA = poly.Points[GetNext(i - 1, count)];
                            var sideA = ClassifyPoint(ptA, part);
                            if ((sideA == sideC)) continue;
                            if ((sideA != ClassifyPointResult.OnPlane) && (sideC != ClassifyPointResult.OnPlane))
                            {
                                backPartPoints.Add(vwiwcB);
                                frontPartPoints.Add(vwiwcB);
                            }
                            else
                                if (sideA == ClassifyPointResult.OnPlane)
                                {
                                    switch (sideC)
                                    {
                                        case ClassifyPointResult.OnBack:
                                            backPartPoints.Add(vwiwcB);
                                            break;
                                        case ClassifyPointResult.OnFront:
                                            frontPartPoints.Add(vwiwcB);
                                            break;
                                    }
                                }
                                else
                                    if (sideC == ClassifyPointResult.OnPlane)
                                    {
                                        switch (sideA)
                                        {
                                            case ClassifyPointResult.OnBack:
                                                backPartPoints.Add(vwiwcB);
                                                break;
                                            case ClassifyPointResult.OnFront:
                                                frontPartPoints.Add(vwiwcB);
                                                break;
                                        }
                                    }
                        }
                }

                if ((frontPartPoints.Count != 0) || (backPartPoints.Count != 0))
                {
                    for (var i = 0; i < backPartPoints.Count - 1; i += 2)
                    {
                        var vwiwc1 = backPartPoints[i];
                        var vwiwc2 = backPartPoints[i + 1];
                        vwiwc1.CuttingBackPoint = true;
                        vwiwc2.CuttingBackPoint = true;
                        vwiwc1.CuttingBackPairIndex = vwiwc2.Index;
                        vwiwc2.CuttingBackPairIndex = vwiwc1.Index;
                    }
                    for (var i = 0; i < frontPartPoints.Count - 1; i += 2)
                    {
                        var vwiwc1 = frontPartPoints[i];
                        var vwiwc2 = frontPartPoints[i + 1];
                        vwiwc1.CuttingFrontPoint = true;
                        vwiwc2.CuttingFrontPoint = true;
                        vwiwc1.CuttingFrontPairIndex = vwiwc2.Index;
                        vwiwc2.CuttingFrontPairIndex = vwiwc1.Index;
                    }


                    for (var i = 0; i < backPartPoints.Count - 1; i++)
                    {
                        var vwiwc = backPartPoints[i];
                        if (vwiwc.AlreadyCuttedBack) continue;
                        CutOutBackPolygon(polyPoints, vwiwc, outpts);

                        if (outpts.Count > 2)
                            backP.Add(new Polygon3D(outpts.ToArray(), poly));
                    }

                    for (var i = 0; i < frontPartPoints.Count - 1; i++)
                    {
                        var vwiwc = frontPartPoints[i];
                        if (vwiwc.AlreadyCuttedFront) continue;
                        CutOutFrontPolygon(polyPoints, vwiwc, inpts);
                        if (inpts.Count > 2)
                            frontP.Add(new Polygon3D(inpts.ToArray(), poly));
                    }
                }
            }
            else
            {
                backP.Add(poly);
                frontP.Add(poly);
            }

            backPoly = backP.ToArray();
            frontPoly = frontP.ToArray();
        }

        /// <summary>
        /// Cuts the out back polygon.
        /// </summary>
        /// <param name="polyPoints">The poly points.</param>
        /// <param name="vwiwc">The vwiwc.</param>
        /// <param name="points">The points.</param>
        private void CutOutBackPolygon(List<Vector3DIndexClassification> polyPoints, Vector3DIndexClassification vwiwc, ICollection<Vector3D> points)
        {
            points.Clear();

            var curVW = vwiwc;

            while (true)
            {
                curVW.AlreadyCuttedBack = true;
                points.Add(curVW.Vector);

                var curVWPair = polyPoints[curVW.CuttingBackPairIndex];

                if (curVW.CuttingBackPoint)
                {
                    if (!curVWPair.AlreadyCuttedBack)
                    {
                        curVW = curVWPair;
                    }
                    else
                    {
                        var curVWPrev = polyPoints[GetNext(curVW.Index - 1, polyPoints.Count)];
                        var curVWNext = polyPoints[GetNext(curVW.Index + 1, polyPoints.Count)];

                        if ((curVWPrev.Result == ClassifyPointResult.OnBack) && !curVWPrev.AlreadyCuttedBack)
                        {
                            curVW = curVWPrev;
                        }
                        else
                            if ((curVWNext.Result == ClassifyPointResult.OnBack) && !curVWNext.AlreadyCuttedBack)
                            {
                                curVW = curVWNext;
                            }
                            else
                            {
                                return;
                            }
                    }
                }
                else
                {
                    var curVWPrev = polyPoints[GetNext(curVW.Index - 1, polyPoints.Count)];
                    var curVWNext = polyPoints[GetNext(curVW.Index + 1, polyPoints.Count)];

                    if ((curVWPrev.Result != ClassifyPointResult.OnFront) && !curVWPrev.AlreadyCuttedBack)
                    {
                        curVW = curVWPrev;
                    }
                    else
                        if ((curVWNext.Result != ClassifyPointResult.OnFront) && !curVWNext.AlreadyCuttedBack)
                        {
                            curVW = curVWNext;
                        }
                        else
                        {
                            return;
                        }
                }
            }
        }

        /// <summary>
        /// Cuts the out front polygon.
        /// </summary>
        /// <param name="polyPoints">The poly points.</param>
        /// <param name="vwiwc">The vwiwc.</param>
        /// <param name="points">The points.</param>
        private void CutOutFrontPolygon(List<Vector3DIndexClassification> polyPoints, Vector3DIndexClassification vwiwc, List<Vector3D> points)
        {
            points.Clear();

            var curVW = vwiwc;

            while (true)
            {
                curVW.AlreadyCuttedFront = true;
                points.Add(curVW.Vector);

                var curVWPair = polyPoints[curVW.CuttingFrontPairIndex];

                if (curVW.CuttingFrontPoint)
                {
                    if (!curVWPair.AlreadyCuttedFront)
                    {
                        curVW = curVWPair;
                    }
                    else
                    {
                        var curVWPrev = polyPoints[GetNext(curVW.Index - 1, polyPoints.Count)];
                        var curVWNext = polyPoints[GetNext(curVW.Index + 1, polyPoints.Count)];

                        if ((curVWPrev.Result == ClassifyPointResult.OnFront) && !curVWPrev.AlreadyCuttedFront)
                        {
                            curVW = curVWPrev;
                        }
                        else
                            if ((curVWNext.Result == ClassifyPointResult.OnFront) && !curVWNext.AlreadyCuttedFront)
                            {
                                curVW = curVWNext;
                            }
                            else
                            {
                                return;
                            }
                    }
                }
                else
                {
                    var curPrev = polyPoints[GetNext(curVW.Index - 1, polyPoints.Count)];
                    var curNext = polyPoints[GetNext(curVW.Index + 1, polyPoints.Count)];

                    if ((curPrev.Result != ClassifyPointResult.OnBack) && !curPrev.AlreadyCuttedFront)
                    {
                        curVW = curPrev;
                    }
                    else
                        if ((curNext.Result != ClassifyPointResult.OnBack) && !curNext.AlreadyCuttedFront)
                        {
                            curVW = curNext;
                        }
                        else
                        {
                            return;
                        }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private int GetNext(int i, int count)
        {
            if (i >= count)
            {
                return i - count;
            }
            if (i < 0)
            {
                return i + count;
            }

            return i;
        }
        #endregion
    }

    internal class Vector3DIndexClassification
    {
        private int index;
        private ClassifyPointResult result;

        private bool isCuttingBackPoint;
        private int cuttingBackPairIndex;
        private bool alreadyCuttedBack;

        private bool isCuttingFrontPoint;
        private int cuttingFrontPairIndex;
        private bool alreadyCuttedFront;

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector3DIndexClassification"/> class.
        /// </summary>
        /// <param name="point">The Vector3D point.</param>
        /// <param name="ind">The index.</param>
        /// <param name="res">The ClassifyPointResult.</param>
        public Vector3DIndexClassification(Vector3D point, int ind, ClassifyPointResult res)
        {
            Vector = point;
            index = ind;
            result = res;
        }

        /// <summary>
        /// Gets or sets the vector.
        /// </summary>
        /// <value>The vector.</value>
        public Vector3D Vector { get; set; }

        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        /// <value>The index.</value>
        public int Index
        {
            get
            {
                return index;
            }

            set
            {
                if (index != value)
                {
                    index = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the classify result.
        /// </summary>
        /// <value>The classify result.</value>
        public ClassifyPointResult Result
        {
            get
            {
                return result;
            }

            set
            {
                if (result != value)
                {
                    result = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [cutting back point].
        /// </summary>
        /// <value><c>true</c> if [cutting back point]; otherwise, <c>false</c>.</value>
        /// <internalonly/>

        public bool CuttingBackPoint
        {
            get
            {
                return isCuttingBackPoint;
            }

            set
            {
                if (isCuttingBackPoint != value)
                {
                    isCuttingBackPoint = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [cutting front point].
        /// </summary>
        /// <value><c>true</c> if [cutting front point]; otherwise, <c>false</c>.</value>
        /// <internalonly/>

        public bool CuttingFrontPoint
        {
            get
            {
                return isCuttingFrontPoint;
            }

            set
            {
                if (isCuttingFrontPoint != value)
                {
                    isCuttingFrontPoint = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the index of the cutting back pair.
        /// </summary>
        /// <value>The index of the cutting back pair.</value>
        /// <internalonly/>

        public int CuttingBackPairIndex
        {
            get
            {
                return cuttingBackPairIndex;
            }

            set
            {
                if (cuttingBackPairIndex != value)
                {
                    cuttingBackPairIndex = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the index of the cutting front pair.
        /// </summary>
        /// <value>The index of the cutting front pair.</value>
        /// <internalonly/>

        public int CuttingFrontPairIndex
        {
            get
            {
                return cuttingFrontPairIndex;
            }

            set
            {
                if (cuttingFrontPairIndex != value)
                {
                    cuttingFrontPairIndex = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [already cutted back].
        /// </summary>
        /// <value><c>true</c> if [already cutted back]; otherwise, <c>false</c>.</value>
        /// <internalonly/>

        public bool AlreadyCuttedBack
        {
            get
            {
                return alreadyCuttedBack;
            }

            set
            {
                if (alreadyCuttedBack != value)
                {
                    alreadyCuttedBack = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [already cutted front].
        /// </summary>
        /// <value><c>true</c> if [already cutted front]; otherwise, <c>false</c>.</value>
        /// <internalonly/>

        public bool AlreadyCuttedFront
        {
            get
            {
                return alreadyCuttedFront;
            }

            set
            {
                if (alreadyCuttedFront != value)
                {
                    alreadyCuttedFront = value;
                }
            }
        }
    }

    internal sealed class BspNode
    {
        #region Properties

        /// <summary>
        /// Gets or sets the back node.
        /// </summary>
        /// <value>The back node.</value>
        public BspNode Back { get; set; }

        /// <summary>
        /// Gets or sets the front node.
        /// </summary>
        /// <value>The front node.</value>
        public BspNode Front { get; set; }

        /// <summary>
        /// Gets or sets the plane.
        /// </summary>
        /// <value>The plane.</value>
        public Polygon3D Plane { get; set; }

        #endregion
    }
}
