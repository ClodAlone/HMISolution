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

namespace Syncfusion.ComponentModel
{
	/// <summary>
	/// This is a base class for Disposable. It implements the IDisposable interface
	/// as suggested in the .NET documentation using the Disposable pattern but it does not
	/// implement a finalizer. If you need finalization you need to derive from Disposable
	/// or add a finalizer to your derived class and manually call Dispose from the Finalizer.
	/// </summary>
	[Serializable]
	public class NonFinalizeDisposable : IDisposable
	{
		/// <overload>
		/// Releases all resources used by the Component.
		/// </overload>
		/// <summary>
		/// Releases all resources used by the Component.
		/// </summary>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Releases the unmanaged resources used by the Component and optionally releases the managed resources.
		/// </summary>
		/// <param name="disposing"><see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources.</param>
		/// <remarks>See the documentation for the <see cref="System.ComponentModel.Component"/> class and its Dispose member.</remarks>
		protected virtual void Dispose(bool disposing)
		{
		}
	}

	/// <summary>
	/// This class provides a base class that implements the IDisposable interface
	/// as suggested in the .NET documentation using the Disposable pattern.
	/// </summary>
	/// <remarks>If you derive from this class, you only need to override the protected
	/// Dispose method and check the disposing parameter.</remarks>
	[Serializable]
	public class Disposable : NonFinalizeDisposable
	{
		/// <summary>
		/// <see cref="Object.Finalize"/>.<para/>
		/// In C# and C++, finalizers are expressed using destructor syntax.
		/// </summary>
		~Disposable()
		{
			this.Dispose(false);
		}
	}


	/// <summary>
	/// This is a base class for DisposableWithDisposedProp. It implements the IDisposable interface
	/// as suggested in the .NET documentation using the Disposable pattern but it does not
	/// implement a finalizer. If you need finalization you need to derive from Disposable
	/// or add a finalizer to your derived class and manually call Dispose from the Finalizer.
	/// </summary>
	[Serializable]
	public class NonFinalizeDisposableWithDisposedProp : IDisposable
	{
		/// <summary>
		/// Storage for IsDisposed property. True - object is disposed, 
		/// otherwise object is still alive and available for user use.
		/// </summary>
		protected bool m_bDisposed;

		/// <summary>
		/// Allows to detect if object is disposed or not. True indicates object is disposed,
		/// otherwise indicates object is still alive and ready for use.
		/// </summary>
		protected bool IsDisposed
		{
			get
			{
				return m_bDisposed;
			}
		}

		/// <overload>
		/// Releases all resources used by the Component.
		/// </overload>
		/// <summary>
		/// Releases all resources used by the Component.
		/// </summary>
		public void Dispose()
		{
			if( !m_bDisposed )
			{
				this.Dispose( true );
			}
		}

		/// <summary>
		/// Releases the unmanaged resources used by the Component and optionally releases the managed resources.
		/// </summary>
		/// <param name="disposing"><see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources.</param>
		/// <remarks>See the documentation for the <see cref="System.ComponentModel.Component"/> class and its Dispose member.</remarks>
		protected virtual void Dispose( bool disposing )
		{
			if( disposing && !m_bDisposed )
			{
				m_bDisposed = true;
				GC.SuppressFinalize( this );
			}
		}
	}

	///	<summary>
	///	This class provides	a base class that implements the IDisposable interface
	///	as suggested in	the	.NET documentation using the Disposable	pattern.
	///	</summary>
	///	<remarks>If	you	derive from	this class,	you	only need to override the protected
	///	Dispose	method and check the disposing parameter.</remarks>
	[Serializable]
	public class DisposableWithDisposedProp	:	NonFinalizeDisposableWithDisposedProp
	{
		///	<summary>
		///	<see cref="Object.Finalize"/>.<para/>
		///	In C# and C++, finalizers are expressed	using destructor syntax.
		///	</summary>
		~DisposableWithDisposedProp()
		{
			this.Dispose(	!this.IsDisposed );
		}
	}
}
