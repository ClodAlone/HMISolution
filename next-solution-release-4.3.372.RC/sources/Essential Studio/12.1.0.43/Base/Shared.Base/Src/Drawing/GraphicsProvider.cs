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
using System.Drawing;
using System.Windows.Forms;

using Syncfusion.ComponentModel;
using Syncfusion.Runtime.InteropServices;

namespace Syncfusion.Drawing
{
	/// <summary>
	/// Provides data for the <see cref="GraphicsProvider.PrepareGraphics"/> event of
	/// a <see cref="GraphicsProvider"/> instance.
	/// </summary>
	/// <remarks>
	/// The event lets you apply custom settings for the Graphics object, before
	/// other routines draw to the object.
	/// </remarks>
	public class GraphicsEventArgs : SyncfusionEventArgs
	{
		Graphics graphics;

		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude]
		public new static readonly GraphicsEventArgs Empty = new GraphicsEventArgs();

		/// <overload>
		/// Initializes a new empty <see cref="GraphicsEventArgs"/>.
		/// </overload>
		/// <summary>
		/// Initializes a new <see cref="GraphicsEventArgs"/> with a <see cref="Graphics"/> object.
		/// </summary>
		/// <param name="graphics">The graphics object.</param>
		public GraphicsEventArgs(Graphics graphics)
		{
			this.graphics = graphics;
		}

		/// <summary>
		/// Initializes a new empty <see cref="GraphicsEventArgs"/>.
		/// </summary>
		public GraphicsEventArgs()
		{
		}

		/// <summary>
		/// Returns the window graphics object.
		/// </summary>
		public Graphics Graphics
		{
			get
			{
				return graphics;
			}
		}
	}


	/// <summary>
	/// Represents a method that handles a <see cref="GraphicsProvider.PrepareGraphics"/> event of
	/// a <see cref="GraphicsProvider"/> instance.
	/// </summary>
	public delegate void GraphicsEventHandler(object sender, GraphicsEventArgs e);

	/// <summary>
	/// Implements an interface that returns a graphics context when needed and raises a 
	/// <see cref="GraphicsProvider.PrepareGraphics"/> event to initialize the graphics object.
	/// </summary>
	public interface IGraphicsProvider
	{
		/// <summary>
		/// Creates and returns a cached graphics object.
		/// </summary>
		Graphics Graphics { get; }

		/// <summary>
		/// Called after a new <see cref="Graphics"/> object was created and gives the handler
		/// a chance to initialize the graphics context.
		/// </summary>
		event GraphicsEventHandler PrepareGraphics;
	}

	/// <summary>
	/// Returns a graphics context when needed and raises a 
	/// <see cref="GraphicsProvider.PrepareGraphics"/> event to initialize the graphics object.
	/// </summary>
	public class GraphicsProvider : Disposable, IGraphicsProvider
	{
		Graphics g = null;
		Control control = null;
		private DisplayDCGraphicsProvider dgp = null;
		private bool ownedGraphics = false;

		/// <summary>
		/// Called after a new <see cref="Graphics"/> object was created and gives the handler
		/// a chance to initialize the graphics context.
		/// </summary>
		public event GraphicsEventHandler PrepareGraphics;

		/// <overload>
		/// Initializes a new empty <see cref="GraphicsProvider"/>.
		/// </overload>
		/// <summary>
		/// Initializes a new <see cref="GraphicsProvider"/> with a <see cref="Graphics"/> object.
		/// </summary>
		/// <param name="g">The Graphics object that will be used.</param>
		/// <remarks>The <see cref="PrepareGraphics"/> event will not be fired when this type
		/// is instantiated via this constructor.</remarks>
		public GraphicsProvider(Graphics g)
		{
			this.g = g;
			this.ownedGraphics = false;
		}

		/// <summary>
		/// Initializes a new <see cref="GraphicsProvider"/> with a <see cref="Control"/> object.
		/// </summary>
		/// <param name="control">The control that will be used for creating the graphics object.</param>
		public GraphicsProvider(Control control)
		{
			this.control = control;
			this.ownedGraphics = false;
		}

		/// <summary>
		/// Initializes a new empty <see cref="GraphicsProvider"/>.
		/// </summary>
		public GraphicsProvider()
		{
		}

		/// <override/>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if(this.ownedGraphics)
				{
					if (g != null)
					{
						g.Dispose();
						g = null;
					}
				}
				// Just removing the reference will cause the DisplayDCGraphicsProvider to cleanup itself.
				this.dgp = null;
			}
			base.Dispose(disposing);
		}


		/// <summary>
		/// Creates and returns a cached graphics object.
		/// </summary>
		public Graphics Graphics
		{
			get
			{
				if (g == null)
				{
					if (control != null && control.IsHandleCreated)
					{
						g = control.CreateGraphics();
						this.ownedGraphics = true;
						OnPrepareGraphics(new GraphicsEventArgs(g));
					}
				}
				if (g == null)
				{
					// It's important that we hold a reference to this singleton
					// as long as this instance is alive.
					if(this.dgp == null)
					{
						this.dgp = DisplayDCGraphicsProvider.Singleton;
						OnPrepareGraphics(new GraphicsEventArgs(g));
					}

					return this.dgp.Graphics;
				}
				return g;
			}
		}

        /// <summary>
        /// Raises the <see cref="GraphicsProvider.PrepareGraphics"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GraphicsEventArgs" /> that contains the event data.</param>
		protected virtual void OnPrepareGraphics(GraphicsEventArgs e)
		{
			if (PrepareGraphics != null)
				PrepareGraphics(this, e);
		}

	}
	
	/// <exclude/>
	public class DisplayDCGraphicsProvider : Disposable
	{
		Graphics g = null;
		IntPtr hdc = IntPtr.Zero;
		[ThreadStatic()]
		private static WeakReference singletonRef = null;


		// Can only be used via the Singleton property.
		private DisplayDCGraphicsProvider(){}
		
		public Graphics Graphics
		{
			get
			{
				if(g == null)
				{
					hdc = NativeMethods.CreateDC("DISPLAY");
					g = Graphics.FromHdc(hdc);
				}
				return g;
			}
		}

		/// <override/>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (g != null)
				{
					g.Dispose();
					g = null;
				}
			}
			if (hdc != IntPtr.Zero)
			{
				NativeMethods.DeleteDC(hdc);
				hdc = IntPtr.Zero;
			}
			base.Dispose(disposing);
		}
		
		public static DisplayDCGraphicsProvider Singleton
		{
			get
			{
				DisplayDCGraphicsProvider gdp = null;
				if(singletonRef != null)
					gdp = singletonRef.Target as DisplayDCGraphicsProvider;

				if(gdp == null)
				{
					gdp = new DisplayDCGraphicsProvider();
					singletonRef = new WeakReference(gdp);
				}
				return gdp;
			}
		}
	}
}
