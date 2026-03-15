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
using Syncfusion.Win32;
using System.Drawing;
using System.Security;
using System.Timers;
using System.Diagnostics;

namespace Syncfusion.Drawing
{
	/// <summary>
	/// Utility class comes handy when you need to perform simple
	/// animations in your control.
	/// </summary>
	/// <remarks>
	/// <para>The <see cref="StartAnimation"/> method in this class allows you to specify the 
	/// number of animation positions (0 to N), the direction of animation
	/// (whether animation is from 0 towards N or from N towards 0), and
	/// the speed (X) at which animation is performed.</para>
	/// <para>
	/// When the <see cref="StartAnimation"/> method is called, the current animation position
	/// is set (<see cref="AnimationPosition"/> property) which gets reduced (or
	/// incremented based on the direction of animation) every X time
	/// interval specified in the <see cref="StartAnimation"/> method call, until it reaches
	/// the other extreme of the Animation position range. Every time
	/// the <see cref="AnimationPosition"/> value changes, an <see cref="AnimationPositionChanged"/> event
	/// is thrown. Upon reaching the last animation position, an 
	/// <see cref="AnimationDone"/> event is thrown.
	/// </para>
	/// </remarks>
	public class AnimationHelper
	{
		#region Data members

		private int maxAnimationPosition = 10;
		private int animationPosition = 10;
		private bool fireStopAnimation = false;
		private bool animationDirectionExpand = true;
		private bool animationOn = false;
		private bool mtflag = false;
		private WeakReference m_timer;

		#endregion

		/// <summary>
		/// Creates an instance of the AnimationHelper class.
		/// </summary>
		public AnimationHelper()
		{
		}

		private System.Windows.Forms.Timer Timer
		{
			get
			{
				System.Windows.Forms.Timer timer;

				if( null == m_timer || !m_timer.IsAlive )
				{
					timer = new System.Windows.Forms.Timer();
					timer.Tick += new EventHandler( this.TimerHandler );

					m_timer = new WeakReference( timer );
				}
				else
				{
					timer = (System.Windows.Forms.Timer)m_timer.Target;
				}

				return timer;
			}
		}

		private void TimerHandler(object sender, EventArgs e)
		{
			if(mtflag)
				return;
			this.mtflag = true;
			this.AnimateNext();
			this.mtflag = false;
		}

		/// <summary>
		/// Returns the maximum animation position specified in the
		/// <see cref="StartAnimation"/> method.
		/// </summary>
		/// <value>
		/// An integer value indicating the maximum animation position.
		/// </value>
		public int MaxAnimationPosition
		{
			get{return this.maxAnimationPosition;}
		}
		/// <summary>
		/// Returns the current animation position once animation is started.
		/// </summary>
		/// <value>An integer value some where in between (including) 
		/// 0 and <see cref="MaxAnimationPosition"/>.</value>
		public int AnimationPosition
		{
			get{return animationPosition;}
		}
		/// <summary>
		/// Indicates whether Animation is on.
		/// </summary>
		/// <value>True indicates Animation is on; False otherwise.
		/// </value>
		public bool AnimationOn
		{
			get{return this.animationOn;}
		}
		/// <summary>
		/// Indicates in which direction animation is performed.
		/// </summary>
		/// <value>True indicates animation is towards <see cref="MaxAnimationPosition"/>;
		/// False indicates animation is towards zero.</value>
		public bool AnimationDirectionExpand
		{
			get{return this.animationDirectionExpand;}
		}
	
		/// <summary>
		/// Indicates whether more animation positions are to be drawn to complete this animation.
		/// </summary>
		/// <returns>True indicates there is more to come; False otherwise.</returns>
		protected bool NeedAnimation()
		{
			if((animationPosition > 0 && !animationDirectionExpand)
				|| (animationPosition < this.maxAnimationPosition && animationDirectionExpand))
				return true;
			else
				return false;
		}
		/// <summary>
		/// Defines the animation range: 0 to maxPosition; specifies the
		/// direction of animation: 0 to maxPositon or maxPosition to 0;
		/// specifies the interval at which animation should be performed: interval,
		/// and starts animation.
		/// </summary>
		/// <param name="maxPosition">The integer value indicating
		/// the max position of animation.</param>
		/// <param name="directionExpand">The direction of animation.</param>
		/// <param name="interval">The frequency at which animation is performed.</param>
		/// <remarks>
		/// This class uses a <see cref="System.Windows.Forms.Timer"/> to trigger
		/// the <see cref="AnimationPositionChanged"/> event, which means events will
		/// be thrown within the same thread as this method call.
		/// </remarks>
		public void StartAnimation(int maxPosition, bool directionExpand, int interval)
		{
			this.maxAnimationPosition = maxPosition;
			if(directionExpand)
				this.animationPosition = 0;
			else
				this.animationPosition = this.maxAnimationPosition;

			this.animationDirectionExpand = directionExpand;
			this.animationOn = true;

		
			if(interval > 0)
			{
				System.Windows.Forms.Timer timer = this.Timer;

				timer.Interval = interval;
				timer.Start();
			}
			else
			{
				fireStopAnimation = true;
			}

			this.AnimateNext();
		}
		/// <summary>
		/// Allows you to stop animation abruptly (if it is currently on). 
		/// This will then throw the <see cref="AnimationDone"/> event.
		/// </summary>
		public void StopAnimation()
		{
			if(!this.animationOn)
				return;

			this.animationOn = false;
			this.Timer.Stop();

			if(this.AnimationDone != null)
			{
				this.AnimationDone(this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Triggers <see cref="AnimationPositionChanged"/> to force drawing next animation position.
		/// </summary>
		/// <remarks>
		/// <para>You can optionally use this method to force animation not waiting for the timer to break.</para>
		/// </remarks>
		public virtual void AnimateNext()
		{
			if( NeedAnimation() && !fireStopAnimation )
			{
				this.animationOn = true;

				if(!this.animationDirectionExpand)
				{
					this.animationPosition--;
				}
				else
				{
					this.animationPosition++;
				}

				if( this.AnimationPositionChanged != null )
				{
					this.AnimationPositionChanged( this, EventArgs.Empty );
				} 
			}
			else
			{
				if( this.AnimationPositionChanged != null )
				{
					AnimationPositionChanged( this, EventArgs.Empty );
				}

				this.StopAnimation();	
			}
		}
		/// <summary>
		/// Will be thrown as the <see cref="AnimationPosition"/> property changes
		/// during animation.
		/// </summary>
		/// <remarks>
		/// After calling <see cref="StartAnimation"/>, you should listen to this event to 
		/// repaint your control for each new animation position.
		/// </remarks>
		public event EventHandler AnimationPositionChanged;
		/// <summary>
		/// This will be called when animation is complete or when 
		/// <see cref="StopAnimation"/> is called.
		/// </summary>
		public event EventHandler AnimationDone;
	}
}