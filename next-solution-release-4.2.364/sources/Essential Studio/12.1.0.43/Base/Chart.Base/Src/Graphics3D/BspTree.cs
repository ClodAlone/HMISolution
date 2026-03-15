#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;


namespace Syncfusion.Windows.Forms.Chart
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
        private const double c_epsilon = 0.0005;
        #endregion

        #region Members
        private List<Polygon> m_polygons = new List<Polygon>();
        private BspNode m_root = null;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the <see cref="Syncfusion.Windows.Forms.Chart.Polygon"/> at the specified index.
        /// </summary>
        /// <value></value>
        public Polygon this[int index]
        {
            get
            {
                return m_polygons[index] as Polygon;
            }
        }

        /// <summary>
        /// Gets the count of polygons.
        /// </summary>
        /// <value>The count.</value>
        public int Count
        {
            get
            {
                return m_polygons.Count;
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Adds the specified poly.
        /// </summary>
        /// <param name="poly">The poly.</param>
        /// <returns></returns>
        public int Add(Polygon poly)
        {
            m_polygons.Add(poly);
            return m_polygons.Count - 1;
        }

        /// <summary>
        /// Builds this instance.
        /// </summary>
        /// <returns></returns>
        public BspNode Build()
        {
            return Build(m_polygons);
        }

        /// <summary>
        /// Builds the specified collection of polygons.
        /// </summary>
        /// <param name="arlist">The collection of polygons.</param>
        /// <returns></returns>
        public BspNode Build(List<Polygon> arlist)
        {
            BspNode root = new BspNode();
            Polygon plane = arlist[0];
            root.Plane = plane;
            bool isClip = plane.ClipPolygon;

            List<Polygon> arleft = new List<Polygon>(arlist.Count);
            List<Polygon> arright = new List<Polygon>(arlist.Count);

            for (int i = 1, len = arlist.Count; i < len; i++)
            {
                Polygon pln = arlist[i];

                if (pln != plane)
                {
                    ClassifyPolyResult r = this.ClassifyPolygon(plane, pln);

                    switch (r)
                    {
                        case ClassifyPolyResult.OnPlane:
                        case ClassifyPolyResult.ToRight:
                            arright.Add(pln);
                            break;

                        case ClassifyPolyResult.ToLeft:
                            if (!isClip)
                            {
                                arleft.Add(pln);
                            }
                            break;

                        case ClassifyPolyResult.Unknown:
                            Polygon[] ps1, ps2;
                            SplitPolygon(pln, plane, out ps1, out ps2);

                            if (!isClip)
                            {
                                arleft.AddRange(ps1);
                            }

                            arright.AddRange(ps2);
                            break;
                    }
                }
            }

            if (arleft.Count > 0)
            {
                root.Back = Build(arleft);
            }

            if (arright.Count > 0)
            {
                root.Front = Build(arright);
            }

            return root;
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

        /// <summary>
        /// Gets the node count.
        /// </summary>
        /// <returns></returns>
        public int GetNodeCount()
        {
            return GetNodeCount(m_root);
        }
        #endregion

        #region Helper methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pln">The Polygon.</param>
        /// <param name="plg">The Polygon.</param>
        /// <returns></returns>
        private ClassifyPolyResult ClassifyPolygon(Polygon pln, Polygon plg)
        {
            ClassifyPolyResult res = ClassifyPolyResult.Unknown;

            Vector3D[] points = plg.Points;

            if ((points == null) || plg.ClipPolygon)
                return res;

            int onBack = 0;
            int onFront = 0;
            int onPlane = 0;
            bool canBreak = !((plg is Path3D) || (plg is Path3DCollect) || (plg is Image3D) || (plg is PathGroup3D));
            Vector3D normal = pln.Normal;
            double d = pln.D;

            for (int i = 0, len = points.Length, len2 = len / 2 + 1; i < len; i++)
            {
                double r = -d - (points[i] & normal);

                if (r > c_epsilon)
                {
                    onBack++;
                }
                else if (r < -c_epsilon)
                {
                    onFront++;
                }
                else
                {
                    onPlane++;
                }

                if (canBreak)
                {
                    if ((onBack > 0) && (onFront > 0))
                    {
                        res = ClassifyPolyResult.Unknown;
                        break;
                    }
                }
                else
                {
                    if ((onBack >= len2) || (onFront >= len2))
                    {
                        break;
                    }
                }
            }

            if (!canBreak)
            {
                if (onFront < onBack)
                {
                    res = ClassifyPolyResult.ToLeft;
                }
                else
                {
                    res = ClassifyPolyResult.ToRight;
                }
            }
            else
            {
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
            }

            return res;
        }

        /// <summary>
        /// Classifies the point.
        /// </summary>
        /// <param name="pt">The pt.</param>
        /// <param name="pln">The PLN.</param>
        /// <returns></returns>
        private ClassifyPointResult ClassifyPoint(Vector3D pt, Polygon pln)
        {
            ClassifyPointResult res = ClassifyPointResult.OnPlane;
            double sv = -pln.D - (pt & pln.Normal);

            if (sv > c_epsilon)
            {
                res = ClassifyPointResult.OnBack;
            }
            else if (sv < -c_epsilon)
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
        private void SplitPolygon(Polygon poly, Polygon part, out Polygon[] backPoly, out Polygon[] frontPoly)
        {
            ArrayList backP = new ArrayList();
            ArrayList frontP = new ArrayList();

            // this code looks for points which lie on the part plane and divide polygon into two parts
            if ((poly.Points != null) && !poly.ClipPolygon)
            {
                ArrayList polyPoints = new ArrayList(poly.Points.Length + 4);
                ArrayList backPartPoints = new ArrayList(4);
                ArrayList frontPartPoints = new ArrayList(4);

                ArrayList outpts = new ArrayList();
                ArrayList inpts = new ArrayList();

                int count = poly.Points.Length;
                for (int i = 0; i < count; i++)
                {
                    Vector3D ptB = poly.Points[i];
                    Vector3D ptC = poly.Points[GetNext(i + 1, count)];
                    ClassifyPointResult sideB = ClassifyPoint(ptB, part);
                    ClassifyPointResult sideC = ClassifyPoint(ptC, part);

                    Vector3DWithIndexWithClassification vwiwcB = new Vector3DWithIndexWithClassification(ptB, polyPoints.Count, sideB);
                    polyPoints.Add(vwiwcB);

                    if ((sideB != sideC) && (sideB != ClassifyPointResult.OnPlane) && (sideC != ClassifyPointResult.OnPlane))
                    {
                        Vector3D v = ptB - ptC;
                        Vector3D dir = part.Normal * (-part.D) - ptC;

                        double sv = dir & part.Normal;
                        double sect = sv / (part.Normal & v);

                        Vector3D ptP = ptC + v * sect;

                        Vector3DWithIndexWithClassification vwiwc = new Vector3DWithIndexWithClassification(ptP, polyPoints.Count, ClassifyPointResult.OnPlane);

                        polyPoints.Add(vwiwc);
                        backPartPoints.Add(vwiwc);
                        frontPartPoints.Add(vwiwc);
                    }
                    else
                        if (sideB == ClassifyPointResult.OnPlane)
                        {
                            Vector3D ptA = poly.Points[GetNext(i - 1, count)];
                            ClassifyPointResult sideA = ClassifyPoint(ptA, part);
                            if ((sideA != sideC))
                            {
                                if ((sideA != ClassifyPointResult.OnPlane) && (sideC != ClassifyPointResult.OnPlane))
                                {
                                    backPartPoints.Add(vwiwcB);
                                    frontPartPoints.Add(vwiwcB);
                                }
                                else
                                    if (sideA == ClassifyPointResult.OnPlane)
                                    {
                                        if (sideC == ClassifyPointResult.OnBack)
                                        {
                                            backPartPoints.Add(vwiwcB);
                                        }
                                        else
                                            if (sideC == ClassifyPointResult.OnFront)
                                            {
                                                frontPartPoints.Add(vwiwcB);
                                            }
                                    }
                                    else
                                        if (sideC == ClassifyPointResult.OnPlane)
                                        {
                                            if (sideA == ClassifyPointResult.OnBack)
                                            {
                                                backPartPoints.Add(vwiwcB);
                                            }
                                            else
                                                if (sideA == ClassifyPointResult.OnFront)
                                                {
                                                    frontPartPoints.Add(vwiwcB);
                                                }
                                        }
                            }
                        }
                }

                Vector3D line = poly.Normal * part.Normal;
                if (!double.IsNaN(line.GetLength()))
                {
                    line = line * (1 / line.GetLength());
                }

                //        if( backPartPoints.Count == 0 )
                //        {
                //          frontPartPoints.Clear();
                //        }
                //        if( frontPartPoints.Count == 0 )
                //        {
                //          backPartPoints.Clear();
                //        }

                if ((frontPartPoints.Count != 0) || (backPartPoints.Count != 0))
                {
                    Vector3D sortPoint;
                    if (backPartPoints.Count != 0)
                    {
                        sortPoint = (backPartPoints[0] as Vector3DWithIndexWithClassification).Vector;
                    }
                    else
                    {
                        // here we sort this points in order in which they lie on the line ( the line of intersection of two planes )
                        sortPoint = (frontPartPoints[0] as Vector3DWithIndexWithClassification).Vector;
                    }
                    // here we sort this points in order in which they lie on the line ( the line of intersection of two planes )
                    PointsOnLineComparer polc = new PointsOnLineComparer(line, sortPoint);

                    // here we sort this points in order in which they lie on the line ( the line of intersection of two planes )
                    backPartPoints.Sort(polc);
                    frontPartPoints.Sort(polc);

                    Debug.Assert(backPartPoints.Count % 2 == 0);
                    Debug.Assert(frontPartPoints.Count % 2 == 0);

                    for (int i = 0; i < backPartPoints.Count - 1; i += 2)
                    {
                        Vector3DWithIndexWithClassification vwiwc1 = (backPartPoints[i] as Vector3DWithIndexWithClassification);
                        Vector3DWithIndexWithClassification vwiwc2 = (backPartPoints[i + 1] as Vector3DWithIndexWithClassification);
                        vwiwc1.CuttingBackPoint = true;
                        vwiwc2.CuttingBackPoint = true;
                        vwiwc1.CuttingBackPairIndex = vwiwc2.Index;
                        vwiwc2.CuttingBackPairIndex = vwiwc1.Index;
                    }
                    for (int i = 0; i < frontPartPoints.Count - 1; i += 2)
                    {
                        Vector3DWithIndexWithClassification vwiwc1 = (frontPartPoints[i] as Vector3DWithIndexWithClassification);
                        Vector3DWithIndexWithClassification vwiwc2 = (frontPartPoints[i + 1] as Vector3DWithIndexWithClassification);
                        vwiwc1.CuttingFrontPoint = true;
                        vwiwc2.CuttingFrontPoint = true;
                        vwiwc1.CuttingFrontPairIndex = vwiwc2.Index;
                        vwiwc2.CuttingFrontPairIndex = vwiwc1.Index;
                    }


                    for (int i = 0; i < backPartPoints.Count - 1; i++)
                    {
                        Vector3DWithIndexWithClassification vwiwc = (backPartPoints[i] as Vector3DWithIndexWithClassification);
                        if (!vwiwc.AlreadyCuttedBack)
                        {
                            CutOutBackPolygon(polyPoints, vwiwc, outpts);

                            if (outpts.Count > 2)
                                backP.Add(new Polygon((Vector3D[])outpts.ToArray(typeof(Vector3D)), poly));
                        }
                    }

                    for (int i = 0; i < frontPartPoints.Count - 1; i++)
                    {
                        Vector3DWithIndexWithClassification vwiwc = (frontPartPoints[i] as Vector3DWithIndexWithClassification);
                        if (!vwiwc.AlreadyCuttedFront)
                        {
                            CutOutFrontPolygon(polyPoints, vwiwc, inpts);
                            if (inpts.Count > 2)
                                frontP.Add(new Polygon((Vector3D[])inpts.ToArray(typeof(Vector3D)), poly));
                        }
                    }
                }
            }
            else
            {
                backP.Add(poly);
                frontP.Add(poly);
            }

            backPoly = (Polygon[])backP.ToArray(typeof(Polygon));
            frontPoly = (Polygon[])frontP.ToArray(typeof(Polygon));
        }

        /// <summary>
        /// Cuts the out back polygon.
        /// </summary>
        /// <param name="polyPoints">The poly points.</param>
        /// <param name="vwiwc">The vwiwc.</param>
        /// <param name="points">The points.</param>
        private void CutOutBackPolygon(ArrayList polyPoints, Vector3DWithIndexWithClassification vwiwc, ArrayList points)
        {
            points.Clear();

            Vector3DWithIndexWithClassification curVW = vwiwc;

            while (true)
            {
                curVW.AlreadyCuttedBack = true;
                points.Add(curVW.Vector);

                Vector3DWithIndexWithClassification curVWPair = (Vector3DWithIndexWithClassification)polyPoints[curVW.CuttingBackPairIndex];

                if (curVW.CuttingBackPoint)
                {
                    if (!curVWPair.AlreadyCuttedBack)
                    {
                        curVW = curVWPair;
                    }
                    else
                    {
                        Vector3DWithIndexWithClassification curVWPrev = (Vector3DWithIndexWithClassification)polyPoints[GetNext(curVW.Index - 1, polyPoints.Count)];
                        Vector3DWithIndexWithClassification curVWNext = (Vector3DWithIndexWithClassification)polyPoints[GetNext(curVW.Index + 1, polyPoints.Count)];

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
                    Vector3DWithIndexWithClassification curVWPrev = (Vector3DWithIndexWithClassification)polyPoints[GetNext(curVW.Index - 1, polyPoints.Count)];
                    Vector3DWithIndexWithClassification curVWNext = (Vector3DWithIndexWithClassification)polyPoints[GetNext(curVW.Index + 1, polyPoints.Count)];

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
        private void CutOutFrontPolygon(ArrayList polyPoints, Vector3DWithIndexWithClassification vwiwc, ArrayList points)
        {
            points.Clear();

            Vector3DWithIndexWithClassification curVW = vwiwc;

            while (true)
            {
                curVW.AlreadyCuttedFront = true;
                points.Add(curVW.Vector);

                Vector3DWithIndexWithClassification curVWPair = (Vector3DWithIndexWithClassification)polyPoints[curVW.CuttingFrontPairIndex];

                if (curVW.CuttingFrontPoint)
                {
                    if (!curVWPair.AlreadyCuttedFront)
                    {
                        curVW = curVWPair;
                    }
                    else
                    {
                        Vector3DWithIndexWithClassification curVWPrev = (Vector3DWithIndexWithClassification)polyPoints[GetNext(curVW.Index - 1, polyPoints.Count)];
                        Vector3DWithIndexWithClassification curVWNext = (Vector3DWithIndexWithClassification)polyPoints[GetNext(curVW.Index + 1, polyPoints.Count)];

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
                    Vector3DWithIndexWithClassification curVWPrev = (Vector3DWithIndexWithClassification)polyPoints[GetNext(curVW.Index - 1, polyPoints.Count)];
                    Vector3DWithIndexWithClassification curVWNext = (Vector3DWithIndexWithClassification)polyPoints[GetNext(curVW.Index + 1, polyPoints.Count)];

                    if ((curVWPrev.Result != ClassifyPointResult.OnBack) && !curVWPrev.AlreadyCuttedFront)
                    {
                        curVW = curVWPrev;
                    }
                    else
                        if ((curVWNext.Result != ClassifyPointResult.OnBack) && !curVWNext.AlreadyCuttedFront)
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
            else if (i < 0)
            {
                return i + count;
            }

            return i;
        }
        #endregion
    }

    /// <internalonly/>
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class Vector3DWithIndexWithClassification
    {
        private Vector3D v;
        private int index;
        private ClassifyPointResult result;

        private bool isCuttingBackPoint = false;
        private int cuttingBackPairIndex = 0;
        private bool alreadyCuttedBack = false;

        private bool isCuttingFrontPoint = false;
        private int cuttingFrontPairIndex = 0;
        private bool alreadyCuttedFront = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector3DWithIndexWithClassification"/> class.
        /// </summary>
        /// <param name="point">The Vector3D point.</param>
        /// <param name="ind">The index.</param>
        /// <param name="res">The ClassifyPointResult.</param>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        public Vector3DWithIndexWithClassification(Vector3D point, int ind, ClassifyPointResult res)
        {
            v = point;
            index = ind;
            result = res;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector3DWithIndexWithClassification"/> class.
        /// </summary>
        /// <param name="vectWW">The Vector3DWithIndexWithClassification argument.</param>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        public Vector3DWithIndexWithClassification(Vector3DWithIndexWithClassification vectWW)
        {
            v = vectWW.Vector;
            index = vectWW.Index;
            result = vectWW.Result;
            isCuttingBackPoint = vectWW.CuttingBackPoint;
            cuttingBackPairIndex = vectWW.cuttingBackPairIndex;
            alreadyCuttedBack = vectWW.AlreadyCuttedBack;
            isCuttingFrontPoint = vectWW.CuttingFrontPoint;
            cuttingFrontPairIndex = vectWW.cuttingFrontPairIndex;
            alreadyCuttedFront = vectWW.AlreadyCuttedFront;
        }

        /// <summary>
        /// Gets or sets the vector.
        /// </summary>
        /// <value>The vector.</value>
        public Vector3D Vector
        {
            get
            {
                return v;
            }

            set
            {
                v = value;
            }
        }

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
        [Syncfusion.Documentation.DocumentationExclude()]
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
        [Syncfusion.Documentation.DocumentationExclude()]
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
        [Syncfusion.Documentation.DocumentationExclude()]
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
        [Syncfusion.Documentation.DocumentationExclude()]
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
        [Syncfusion.Documentation.DocumentationExclude()]
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
        [Syncfusion.Documentation.DocumentationExclude()]
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

    /// <summary>
    /// Compares the points by distance to the eye.
    /// </summary>
    class PointsOnLineComparer : IComparer
    {
        #region Members
        private Vector3D dir;
        private Vector3D p;
        private const double EPSILON = 0.0001;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="PointsOnLineComparer"/> class.
        /// </summary>
        /// <param name="direction">The direction.</param>
        /// <param name="point">The point.</param>
        public PointsOnLineComparer(Vector3D direction, Vector3D point)
        {
            dir = direction;
            p = point;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>
        /// Value Condition Less than zero x is less than y. Zero x equals y. Greater than zero x is greater than y.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">Neither x nor y implements the <see cref="T:System.IComparable"></see> interface.-or- x and y are of different types and neither one can handle comparisons with the other. </exception>
        int IComparer.Compare(Object x, Object y)
        {
            Vector3D v1 = ((Vector3DWithIndexWithClassification)x).Vector - p;
            Vector3D v2 = ((Vector3DWithIndexWithClassification)y).Vector - p;

            double d1 = v1 & dir;
            double d2 = v2 & dir;

            if (d1 > d2)
            {
                return 1;
            }
            else if (d1 < d2)
            {
                return -1;
            }

            return 0;
        }
        #endregion
    }
}
