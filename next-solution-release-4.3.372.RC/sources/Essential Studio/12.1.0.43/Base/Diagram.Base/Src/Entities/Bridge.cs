#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.Collections;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Template object that contain intersection information.
    /// </summary>
    [Serializable]
    public class Bridge
    {
        #region Class members
        private ConnectorBase m_connectorIntersecting;
        private int m_nSegmentIntersectingID;
        private float m_fOffset;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets the connector intersecting.
        /// </summary>
        /// <value>The connector intersecting.</value>
        public ConnectorBase ConnectorIntersecting
        {
            get { return m_connectorIntersecting; }
        }

        /// <summary>
        /// Gets the segment intersecting index.
        /// </summary>
        /// <value>The segment intersecting.</value>
        public int SegmentIntersectingID
        {
            get { return m_nSegmentIntersectingID; }
        }

        /// <summary>
        /// Gets or sets the offset.
        /// </summary>
        /// <value>The offset.</value>
        /// <remarks>
        /// Indicated offset bridge from segment start point 
        /// to center point of bridge.
        /// </remarks>
        public float Offset
        {
            get { return m_fOffset; }
            set { m_fOffset = value; }
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="Bridge"/> class.
        /// </summary>
        /// <param name="connector">The connector.</param>
        /// <param name="segment">The segment.</param>
        /// <param name="offset">The offset.</param>
        public Bridge(ConnectorBase connector, ConnectorLineSegment segment, float offset)
        {
            m_connectorIntersecting = connector;
            m_nSegmentIntersectingID = m_connectorIntersecting.LineSegments.IndexOf(segment);

            if (m_nSegmentIntersectingID == -1)
                throw new ArgumentException("Connector is not caontain given segment.");

            m_fOffset = offset;
        }
        #endregion
    }

    /// <summary>
    /// Helper class to comparer two Bridge objects by Offset.
    /// </summary>
    public class BridgeComparer
        : IComparer
    {
        #region IComparer Members
        /// <summary>
        /// Compares two objects and returns a value indicating whether one
        /// is less than, equal to or greater than the other.
        /// </summary>
        /// <param name="x">First object to compare.</param>
        /// <param name="y">Second object to compare.</param>
        /// <returns>
        /// <list type="table">
        /// <listheader>
        /// <term>Value</term>
        /// <description>Condition</description>
        /// </listheader>
        /// <item>
        /// <term> Less than zero</term>
        /// <description>
        /// <paramref name="x"/> is less than <paramref name="y"/>.</description>
        /// </item>
        /// <item>
        /// <term> Zero</term>
        /// <description>
        /// <paramref name="x"/> equals <paramref name="y"/>.</description>
        /// </item>
        /// <item>
        /// <term> Greater than zero</term>
        /// <description>
        /// <paramref name="x"/> is greater than <paramref name="y"/>.</description>
        /// </item>
        /// </list>
        /// </returns>
        /// <exception cref="T:System.ArgumentException">
        /// <para>Neither <paramref name="x"/> nor <paramref name="y"/> implements the <see cref="T:System.IComparable"/> interface.</para>
        /// <para>-or-</para>
        /// <para>
        /// <paramref name="x"/> and <paramref name="y"/> are of different types and neither one can handle comparisons with the other.</para>
        /// </exception>
        public int Compare(object x, object y)
        {
            if (x == null || !(x is Bridge))
                throw new ArgumentNullException("x");

            if (y == null || !(y is Bridge))
                throw new ArgumentNullException("y");

            Bridge bridgeX = (Bridge)x;
            Bridge bridgeY = (Bridge)y;

            return (int)(bridgeX.Offset - bridgeY.Offset);
        }
        #endregion
    }
}
