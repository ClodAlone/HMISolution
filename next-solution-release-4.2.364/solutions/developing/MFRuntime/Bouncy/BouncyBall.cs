////////////////////////////////////////////////
// DESCRIPTION:
//    Abstract "Ball" support for "Bouncy" application
//
// Legal Notices:
//   Copyright (c) 2008, Telliam Consulting, LLC.
//   All rights reserved.
//
//   Redistribution and use in source and binary forms, with or without modification,
//   are permitted provided that the following conditions are met:
//
//   * Redistributions of source code must retain the above copyright notice, this list
//     of conditions and the following disclaimer.
//   * Redistributions in binary form must reproduce the above copyright notice, this
//     list of conditions and the following disclaimer in the documentation and/or other
//     materials provided with the distribution.
//   * Neither the name of Telliam Consulting nor the names of its contributors may be
//     used to endorse or promote products derived from this software without specific
//     prior written permission.
//
//   THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY
//   EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
//   OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT
//   SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
//   INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED
//   TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR 
//   BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
//   CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
//   ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH
//   DAMAGE. 
//

using System;
using System.Threading;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Text;

using Microsoft.SPOT;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Media;
using Microsoft.SPOT.Hardware;

namespace FusionWare.SPOT.Bouncy
{
    /// <summary>Simple class for representing a point in 2D space.</summary>
    public class Point
    {
        /// <summary>Creates a new point with specified coordinates</summary>
        /// <param name="x">x position</param>
        /// <param name="y">y position</param>
        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        /// <summary>Horizontal position of the point</summary>
        public int x;

        /// <summary>Vertical position of the point</summary>
        public int y;

        /// <summary>Adds 2 points together</summary>
        /// <param name="a">left hand side of addition</param>
        /// <param name="b">right hand side of the addition</param>
        /// <returns>New point with summed axes values</returns>
        /// <remarks>
        /// Provides operator overload for adding two points together 
        /// to form an offset position.
        /// </remarks>
        public static Point operator +(Point a, Point b)
        {
            return new Point(a.x + b.x, a.y + b.y);
        }

        /// <summary>Determines the distance between this point and an arbitraryt point</summary>
        /// <param name="P2">The point to determine the distance from</param>
        /// <returns>Distance between this point's position and the position of P2</returns>
        public int DistanceFrom(Point P2)
        {
            uint dx = (uint)System.Math.Abs(this.x - P2.x);
            uint dy = (uint)System.Math.Abs(this.y - P2.y);
            return isqrt((dx*dx) + (dy* dy));
        }
        
        /// <summary>Computes an integer square root</summary>
        /// <param name="a">value to take the quare root of</param>
        /// <returns>Square root of the parameter a</returns>
        /// <remarks>
        /// <para>Computes an integer square root. Unfortunately, the
        /// .Net Micro Framework does not have any Square root function
        /// in System.Math nor in Microsoft.SPOT.Math.</para>
        /// 
        /// <para>Algoritm from:</para>
        /// Jack Crenshaw, Embedded Systems Programming magazine
        /// </remarks>
        private static ushort isqrt(uint a)
        {
            uint rem = 0;
            uint root = 0;
            uint divisor = 0;
            for(int i = 0; i < 16; i++)
            {
                root <<= 1;
                rem = ((rem << 2) + (a >> 30));
                a <<= 2;
                divisor = (root << 1) + 1;
                if(divisor <= rem)
                {
                    rem -= divisor;
                    root++;
                }
            }
            return (ushort)(root);
        }
    }

    /// <summary>Implements functionality for a BALL in the <see cref="BouncyUniverse">BouncyUniverse</see></summary>
    /// <remarks>
    /// This class does not contain any direct UI support to isolate the
    /// representation of the ball from its behaviour. The purpose of this
    /// class is only to maintain the non gui state of the ball (E.g Its
    /// position and Radius.)
    /// All balls are treated as circular regardless of their UI representation.
    /// 
    /// GUI code using this class can associate bitmap or other info with 
    /// a ball using the Tag property.
    /// </remarks>
    public class BouncyBall
    {
        #region Constructors
        /// <summary>Creates a new ball</summary>
        /// <param name="Center">Center point of the "ball"</param>
        /// <param name="Radius">Radius of the ball</param>
        /// <remarks>
        /// This class represents a ball in the <see cref="BouncyUniverse">BouncyUnivers</see>.
        /// All balls are circular regardless of it's UI representation.
        /// </remarks>
        public BouncyBall(Point Center, int Radius)
        {
            this._Radius = Radius;
            MoveCenterToAbs(Center);
        }

        /// <summary>Creates a new ball</summary>
        /// <param name="Radius">Radius of the ball</param>
        /// <remarks>
        /// This version of the constructor will create a ball with center pint at (0,0) 
        /// </remarks>
        public BouncyBall(int Radius)
        {
            this._Radius = Radius;
            MoveCenterToAbs(new Point(0,0));
        }

        #endregion

        /// <summary>Moves the center point of the ball to a fixed absolute position</summary>
        /// <param name="Center">New position for the ball</param>
        public void MoveCenterToAbs(Point Center)
        {
            this._Pos = Center;
        }

        /// <summary>Moves the center point of the ball relative to its current position</summary>
        /// <param name="Offset">Offset to move the ball</param>
        public void MoveCenterRel(Point Offset)
        {
            _Pos += Offset;
        }

        #region Public Properties
        /// <summary>
        /// Current center position of the "Ball"
        /// </summary>
        public Point Position
        {
            get { return _Pos; }
        }
        private Point _Pos;

        /// <summary>Radius of the Ball for detecting colissions</summary>
        public int Radius
        {
            get { return _Radius; }
        }
        private int _Radius;

        /// <summary>Application specifc object reference to associate with a ball</summary>
        /// <value>object reference to associate with the ball</value>
        /// <remarks>
        /// This value is ignored within the class it is to allow application code to 
        /// associate some external data with a particular ball. This is typically used
        /// by GUI applications to store a bitmap for the ball.
        /// </remarks>
        public object Tag
        {
            get { return _Tag; }
            set { _Tag = value; }
        }
        private object _Tag;
	
        #endregion

     }
}